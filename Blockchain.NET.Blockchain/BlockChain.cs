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
        public int NextBlockHeight
        {
            get
            {
                if (_nextBlock == null)
                {
                    _nextBlock = LastBlock();
                }
                return _nextBlock == null ? 1 : _nextBlock.Height;
            }
        }

        public List<Transaction> MemPool
        {
            get
            {
                return _memPool;
            }
            set
            {
                _memPool = value;
            }
        }

        public Wallet Wallet { get; set; }

        private bool _isMining;
        private List<Transaction> _memPool;
        private NetworkSynchronizer _networkSynchronizer;
        private Block _nextBlock;

        public BlockChain(Wallet wallet)
        {
            Wallet = wallet;
            _memPool = new List<Transaction>();
            BlockchainDbContext.InitializeMigrations();
            _networkSynchronizer = new NetworkSynchronizer(this);
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

                lock (_memPool)
                {
                    var miningAddress = Wallet.NewAddress();
                    AddTransaction(new Transaction(null, Wallet, new[] { new Output(miningAddress.Key, MiningReward) }.ToList()));
                    using (BlockchainDbContext db = new BlockchainDbContext())
                    {
                        var mempoolHashes = _memPool.Select(m => m.GenerateHash());
                        var existingTransactionsInBlockchain = db.Transactions.Where(t => mempoolHashes.Contains(t.Hash)).ToList();
                        existingTransactionsInBlockchain.ForEach(t => _memPool.Remove(_memPool.First(m => m.Hash == t.Hash)));
                        _nextBlock = lastBlock == null ? new Block(1, null, _memPool.ToList()) : new Block(lastBlock.Height + 1, lastBlock.GenerateHash(), _memPool.ToList());
                    }

                }
                var difficulty = lastBlock == null ? 5 : lastBlock.Difficulty;
                //TODO: Reactivate audo difficulty calculation
                //if (lastBlock != null && lastBlock.Height > 1 && lastBlock.Height % DifficultyCorrectureInterval == 1)
                //{
                //    var lowTime = GetBlock(lastBlock.Height - DifficultyCorrectureInterval).TimeStamp;
                //    var highTime = GetBlock(lastBlock.Height).TimeStamp;

                //    difficulty = Convert.ToInt32(lastBlock.Difficulty * DifficultyTimeTarget / ((highTime - lowTime).TotalSeconds / DifficultyCorrectureInterval));
                //}
                _nextBlock.MineBlock(difficulty);
                AddBlock(_nextBlock);
                BlockchainConsole.WriteLine($"MINED BLOCK: {_nextBlock}", ConsoleEventType.MINEDBLOCK);
            }
        }

        #endregion

        #region Syncronizing

        public void StartSyncronizing()
        {
            if (!_networkSynchronizer.IsSyncing)
            {
                _networkSynchronizer.Start();
            }
        }

        public void StopSyncronizing()
        {
            _networkSynchronizer.Stop();
        }

        #endregion

        #region MODIFICATION

        public bool PushBlock(Block block)
        {
            if (AddBlock(block))
            {
                if (_nextBlock != null)
                    _nextBlock.StopMining();
                return true;
            }
            return false;
        }

        public bool AddBlock(Block block)
        {
            if (block == null)
                return false;
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
                                var existingBlock = db.Blocks.FirstOrDefault(b => b.Height == block.Height);
                                if (existingBlock == null)
                                {
                                    var findLastBlock = block.Height == 1 ? null : db.Blocks.FirstOrDefault(b => b.Height == block.Height - 1);
                                    if (block.Height == 1 || (findLastBlock != null && findLastBlock.GenerateHash() == block.PreviousHash))
                                    {
                                        db.Blocks.Add(block);
                                        db.SaveChanges();
                                        foreach (var transactionToDelete in block.Transactions)
                                        {
                                            var foundTransaction = _memPool.FirstOrDefault(t => t.GenerateHash() == transactionToDelete.GenerateHash());
                                            if (foundTransaction != null)
                                                _memPool.Remove(foundTransaction);
                                        }
                                        block.StopMining();
                                        _networkSynchronizer.BroadcastBlock(block);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
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
                        _memPool.Add(transaction);
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
                                if (!_memPool.Select(mp => mp.GenerateHash()).Any(t => t == transaction.GenerateHash()))
                                {
                                    _memPool.Add(transaction);
                                    _networkSynchronizer.BroadcastTransaction(transaction);
                                }
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

        public Block GetBlock(int height, bool includeTransactions = false)
        {
            using (BlockchainDbContext db = new BlockchainDbContext())
            {
                if (includeTransactions)
                {
                    return db.Blocks.Include("Transactions.Outputs").Include("Transactions.Inputs").FirstOrDefault(b => b.Height == height);
                }
                return db.Blocks.FirstOrDefault(b => b.Height == height);
            }
        }

        public List<Block> GetBlocks(List<int> blockHeights, bool includeTransactions = false)
        {
            using (BlockchainDbContext db = new BlockchainDbContext())
            {
                if (includeTransactions)
                {
                    return db.Blocks.Include("Transactions.Outputs").Include("Transactions.Inputs").Where(b => blockHeights.Contains(b.Height)).ToList();
                }
                return db.Blocks.Where(b => blockHeights.Contains(b.Height)).ToList();
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
