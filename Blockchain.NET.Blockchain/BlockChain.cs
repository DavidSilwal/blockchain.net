using Blockchain.NET.Blockchain.Network;
using Blockchain.NET.Core;
using Blockchain.NET.Core.Helpers;
using Blockchain.NET.Core.Helpers.Calculations;
using Blockchain.NET.Core.Mining;
using Blockchain.NET.Core.Store;
using Blockchain.NET.Core.Wallet;
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
        public int DifficultyCorrectureInterval { get; private set; } = 10;
        public int DifficultyTimeTarget { get; private set; } = 20;

        public Wallet Wallet { get; set; }

        private bool _isMining;
        private List<Transaction> _pendingTransactions;

        public BlockChain(Wallet wallet)
        {
            Wallet = wallet;
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

                Block nextBlock = null;
                lock (_pendingTransactions)
                {
                    var miningAddress = Wallet.NewAddress();
                    AddTransaction(new Transaction(null, Wallet, new[] { new Output(miningAddress.Key, MiningReward) }.ToList()));
                    nextBlock = lastBlock == null ? new Block(1, null, _pendingTransactions) : new Block(lastBlock.Height + 1, lastBlock.GenerateHash(), _pendingTransactions);
                    _pendingTransactions = new List<Transaction>();
                }
                var difficulty = lastBlock == null ? 5 : lastBlock.Difficulty;
                //TODO: Reactivate audo difficulty calculation
                //if (lastBlock != null && lastBlock.Height > 1 && lastBlock.Height % DifficultyCorrectureInterval == 1)
                //{
                //    var lowTime = GetBlock(lastBlock.Height - DifficultyCorrectureInterval).TimeStamp;
                //    var highTime = GetBlock(lastBlock.Height).TimeStamp;

                //    difficulty = Convert.ToInt32(lastBlock.Difficulty * DifficultyTimeTarget / ((highTime - lowTime).TotalSeconds / DifficultyCorrectureInterval));
                //}
                nextBlock.MineBlock(difficulty);
                AddBlock(nextBlock);
                BlockchainConsole.WriteLine($"MINED BLOCK: {nextBlock}", ConsoleEventType.MINEDBLOCK);
            }
        }

        #endregion

        #region MODIFICATION
        public void AddBlock(Block block)
        {
            if (block == null)
                return;
            //Check if Proof of Work is correct
            if (block.GenerateHash().Substring(0, block.Difficulty).All(c => c == '0'))
            {
                //Check if MerkleTree from transactions is correct
                if (block.MerkleTreeHash == block.CreateMerkleTreeHash())
                {
                    //Check if only one Coinbase transaction
                    if (block.Transactions.Count(t => t.Inputs == null || (t.Inputs != null && t.Inputs.Count == 0)) == 1)
                    {
                        //Check signatures and validity of transactions
                        bool transactionsValid = true;
                        if (block.Transactions != null)
                            foreach (var transaction in block.Transactions)
                            {
                                if (!transaction.Verify())
                                {
                                    transactionsValid = false;
                                    break;
                                }
                            }
                        if (transactionsValid)
                        {
                            using (BlockchainDbContext db = new BlockchainDbContext())
                            {
                                db.Blocks.Add(block);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction.Verify())
            {
                // Check if 
                if (transaction.Outputs != null)
                {
                    // If Coinbase transaction no validity
                    if (transaction.Inputs == null)
                    {
                        _pendingTransactions.Add(transaction);
                    }
                    else
                    {
                        decimal balance = BalanceHelper.GetBalanceOfAddresses(transaction.Inputs.Select(i => i.Key).ToArray());
                        // Check if enough balance on inputs
                        if (balance >= transaction.Outputs.Select(o => o.Amount).Sum())
                        {
                            bool everUsedAsInput = BalanceHelper.EverUsedAsInput(transaction.Inputs.Select(i => i.Key).ToArray());
                            // Check if ever used as input before
                            if (!everUsedAsInput)
                            {
                                _pendingTransactions.Add(transaction);
                            }
                            else
                            {
                                throw new Exception("Eine der Adressen wurde bereits als Input verwendet!");
                            }
                        }
                        else
                        {
                            throw new Exception("Nicht genug Guthaben!");
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Nicht valide Transaktion!");
            }
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
