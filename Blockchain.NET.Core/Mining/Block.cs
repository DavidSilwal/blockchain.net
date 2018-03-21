using Blockchain.NET.Core.Helpers;
using Blockchain.NET.Core.Helpers.Cryptography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.NET.Core.Mining
{
    public class Block
    {
        [Key]
        public int Height { get; set; }

        public string PreviousHash { get; set; }

        public DateTime TimeStamp { get; set; }

        public long Nonce { get; set; }

        public int Difficulty { get; set; }

        public string MerkleTreeHash { get; set; }

        public List<Transaction> Transactions { get; set; }

        private bool _isMining;

        public Block() { }

        public Block(int height, string previousHash, List<Transaction> transactions)
        {
            Height = height;
            TimeStamp = DateTime.Now;
            PreviousHash = previousHash;
            Transactions = transactions;
            if (transactions.Count > 0)
            {
                var merkleTreeList = transactions.Select(t => t.GenerateHash()).ToList();
                while (merkleTreeList.Count > 1)
                {
                    merkleTreeList.Add(HashHelper.Sha256(merkleTreeList[0] + merkleTreeList[1]));
                    merkleTreeList.RemoveRange(0, 2);
                }
                MerkleTreeHash = merkleTreeList[0];
            }
        }

        public void MineBlock(int difficulty)
        {
            if (!_isMining)
            {
                Difficulty = difficulty;
                _isMining = true;

                var totalHashesCounter = 0;
                DateTime startTime = DateTime.Now;

                Task[] miningTasks = new Task[Environment.ProcessorCount];
                var difficultyValue = string.Join("0", new string[difficulty + 1]);
                for (int i = 0; i < miningTasks.Length; i++)
                {
                    var startNonce = i;
                    miningTasks[i] = Task.Factory.StartNew(() =>
                    {
                        var taskHash = GenerateHash(startNonce);
                        while (taskHash.Substring(0, difficulty) != difficultyValue)
                        {
                            if (!_isMining)
                            {
                                taskHash = string.Empty;
                                break;
                            }
                            startNonce = startNonce + miningTasks.Length;
                            taskHash = GenerateHash(startNonce);
                            totalHashesCounter++;
                            if(totalHashesCounter % 200000 == 0)
                            {
                                var currentElapsedTime = DateTime.Now - startTime;
                                BlockchainConsole.WriteLive($"MINING BLOCK - ELAPSED TIME: {currentElapsedTime.TotalSeconds} Seconds, DIFFICULTY: {difficulty}, HASH RATE: {Convert.ToInt64(totalHashesCounter / currentElapsedTime.TotalSeconds)}/Seconds");
                            }
                        }
                        if (!string.IsNullOrEmpty(taskHash))
                        {
                            Nonce = startNonce;
                            _isMining = false;
                        }
                    });
                }
                Task.WaitAll(miningTasks);
                var elapsedTime = DateTime.Now - startTime;
                BlockchainConsole.WriteLive(string.Empty);
                BlockchainConsole.WriteLine($"ELAPSED TIME: {elapsedTime.TotalSeconds} Seconds, DIFFICULTY: {difficulty}, HASH RATE: {Convert.ToInt64(totalHashesCounter / elapsedTime.TotalSeconds)}/Seconds                              ", ConsoleEventType.Elapsed);
            }
        }

        public string GenerateHash(long nonce = -1)
        {
            if (nonce < 0)
                return HashHelper.Sha256(Height + PreviousHash + TimeStamp + Nonce + Difficulty + MerkleTreeHash);
            else
                return HashHelper.Sha256(Height + PreviousHash + TimeStamp + nonce + Difficulty + MerkleTreeHash);
        }

        public void StopMining()
        {
            _isMining = false;
        }

        public override string ToString()
        {
            return $"BlockNumber: {Height}, Hash: {GenerateHash()}";
        }
    }
}
