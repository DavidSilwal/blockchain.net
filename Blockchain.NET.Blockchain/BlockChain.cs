﻿using Blockchain.NET.Blockchain.Network;
using Blockchain.NET.Blockchain.Store;
using Blockchain.NET.Core;
using Blockchain.NET.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Blockchain.NET.Blockchain
{
    public class BlockChain
    {
        public decimal MiningReward { get; set; } = 1000;
        public string MiningRewardAddress { get; set; } = "blockchain1";
        public int DifficultyCorrectureInterval { get; private set; } = 10;
        public int DifficultyTimeTarget { get; private set; } = 20;

        public Wallet Wallet { get; set; }

        public NetworkNode Server { get; set; }

        private bool _isMining;
        private List<Transaction> _pendingTransactions = new List<Transaction>();

        public BlockChain(Wallet wallet)
        {
            Wallet = wallet;
            //Server = new NetworkNode(this);
            _pendingTransactions = new List<Transaction>();
            BlockchainDbContext.InitializeMigrations();
        }

        #region MINING

        public void StartMining()
        {
            if (!_isMining)
            {
                _isMining = true;
                new Thread(miningBlocks).Start();
            }
        }

        public void StopMining()
        {
            _isMining = false;
        }

        private void miningBlocks()
        {
            while (_isMining)
            {
                var lastBlock = LastBlock();

                if (lastBlock == null)
                {
                    var genesisBlock = new Block(1, null, new List<Transaction>());
                    genesisBlock.MineBlock(1);
                    AddBlock(genesisBlock);
                }
                else
                {
                    //Dummy Transactions
                    Random rndm = new Random();
                    for (int i = 0; i < rndm.Next(200, 1000); i++)
                    {
                        _pendingTransactions.Add(new Transaction() { PublicKey = i.ToString() });
                    }
                    //Demo Transaction
                    Block nextBlock = null;
                    lock (_pendingTransactions)
                    {
                        var miningAddress = Wallet.GetNewAddress();
                        _pendingTransactions.Add(new Transaction(null, miningAddress.PublicKey, MiningReward));
                        nextBlock = new Block(lastBlock.Height + 1, lastBlock.GenerateHash(), _pendingTransactions);
                        _pendingTransactions = new List<Transaction>();
                    }
                    var difficulty = lastBlock.Difficulty;
                    if(lastBlock.Height > 1 && lastBlock.Height % DifficultyCorrectureInterval == 1)
                    {
                        var lowTime = GetBlock(lastBlock.Height - DifficultyCorrectureInterval).TimeStamp;
                        var highTime = GetBlock(lastBlock.Height).TimeStamp;

                        difficulty = Convert.ToInt32(lastBlock.Difficulty * DifficultyTimeTarget / ((highTime - lowTime).TotalSeconds / DifficultyCorrectureInterval));
                    }
                    nextBlock.MineBlock(difficulty);
                    AddBlock(nextBlock);
                    BlockchainConsole.WriteLine($"MINED BLOCK: {nextBlock}", ConsoleEventType.MINEDBLOCK);
                }
            }
        }

        #endregion

        #region MODIFICATION
        public void AddBlock(Block block)
        {
            if (block.GenerateHash().Substring(0, block.Difficulty).All(c => c == '0'))
            {
                using (BlockchainDbContext db = new BlockchainDbContext())
                {
                    db.Blocks.Add(block);
                    db.SaveChanges();
                }
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            _pendingTransactions.Add(transaction);
        }

        #endregion

        #region VALIDITY / READ
        public Block LastBlock()
        {
            using (BlockchainDbContext db = new BlockchainDbContext())
            {
                return db.Blocks.OrderBy(b => b.Height).LastOrDefault();
            }
        }

        public Block IsBlockValid(Block block)
        {
            if (block == null)
                return block;
            using (BlockchainDbContext db = new BlockchainDbContext())
            {
                var chainUnderBlock = db.Blocks.Where(b => b.Height < block.Height).OrderByDescending(b => b.Height).ToList();

                if (chainUnderBlock.Count() > 0)
                {
                    var baseHash = block.PreviousHash;
                    foreach (var actBlock in chainUnderBlock)
                    {
                        var hashFromFile = actBlock.GenerateHash();

                        if (hashFromFile != baseHash)
                            return actBlock;
                        baseHash = actBlock.PreviousHash;
                    }
                }
            }
            return null;
        }

        public Block GetBlock(int height)
        {
            using (BlockchainDbContext db = new BlockchainDbContext())
            {
                return db.Blocks.FirstOrDefault(b => b.Height == height);
            }
        }

        public bool IsChainValid()
        {
            var lastBlock = LastBlock();

            return IsBlockValid(lastBlock) == null ? true : false;
        }

        #endregion

        #region Balance
        public decimal GetBalanceOfAddress(string address)
        {
            decimal balance = 0;

            //var allBlocks = _rootDirInfo.GetFiles().ToList();

            //foreach (var actBlock in allBlocks)
            //{
            //    var block = JsonConvert.DeserializeObject<Block>(File.ReadAllText(actBlock.FullName));
            //    //foreach (var transaction in block.Transactions)
            //    //{
            //    //    if (transaction.FromAddress == address)
            //    //    {
            //    //        balance -= transaction.Amount;
            //    //    }
            //    //    if (transaction.ToAddress == address)
            //    //    {
            //    //        balance += transaction.Amount;
            //    //    }
            //    //}
            //}
            return balance;
        }
        #endregion
    }
}
