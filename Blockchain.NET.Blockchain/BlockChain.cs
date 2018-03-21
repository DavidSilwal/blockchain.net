using Blockchain.NET.Blockchain.Network;
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
        public int SecondsToGenerate { get; private set; }
        public decimal MiningReward { get; set; }
        public string MiningRewardAddress { get; set; } = "blockchain1";
        public int DifficultyCorrectureInterval { get; private set; } = 10;
        public int DifficultyTimeTarget { get; private set; } = 20;

        public NetworkNode Server { get; set; }

        List<TimeSpan> statisticsBlockGeneration = new List<TimeSpan>();

        private bool _isMining;
        private List<Transaction> _pendingTransactions;
        private DirectoryInfo _rootDirInfo;

        public BlockChain(string rootLocation, decimal miningReward, int secondsToGenerate)
        {
            MiningReward = miningReward;
            SecondsToGenerate = secondsToGenerate;
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
                    genesisBlock.MineBlock(6);
                    AddBlock(genesisBlock);
                }
                else
                {
                    //Dummy Transactions
                    Random rndm = new Random();
                    for (int i = 0; i < rndm.Next(200, 1000); i++)
                    {
                        _pendingTransactions.Add(new Transaction() { Signature = i.ToString() });
                    }
                    //Demo Transaction
                    Block nextBlock = null;
                    lock (_pendingTransactions)
                    {
                        _pendingTransactions.Add(new Transaction(null, new string[] { }, MiningReward, 100) { Signature = "AA" });
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

            var allBlocks = _rootDirInfo.GetFiles().ToList();

            foreach (var actBlock in allBlocks)
            {
                var block = JsonConvert.DeserializeObject<Block>(File.ReadAllText(actBlock.FullName));
                //foreach (var transaction in block.Transactions)
                //{
                //    if (transaction.FromAddress == address)
                //    {
                //        balance -= transaction.Amount;
                //    }
                //    if (transaction.ToAddress == address)
                //    {
                //        balance += transaction.Amount;
                //    }
                //}
            }
            return balance;
        }
        #endregion

        #region Wallet
        public Tuple<string, string> CreateKeyPair()
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024, cspParams);

            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            return new Tuple<string, string>(privateKey, publicKey);
        }
        public byte[] Encrypt(string publicKey, string data)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));

            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = rsaProvider.Encrypt(plainBytes, false);

            return encryptedBytes;
        }
        public string Decrypt(string privateKey, byte[] encryptedBytes)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));

            byte[] plainBytes = rsaProvider.Decrypt(encryptedBytes, false);

            string plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);

            return plainText;
        }

        public string GenerateAddress()
        {
            return string.Empty;
        }
        #endregion
    }
}
