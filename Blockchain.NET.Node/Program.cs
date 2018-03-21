using Blockchain.NET.Blockchain;
using Blockchain.NET.Blockchain.Network;
using Blockchain.NET.Core;
using Blockchain.NET.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Blockchain.NET.Node
{
    class Program
    {
        static void Main(string[] args)
        {
            var blockChain = new BlockChain("CoreBlocks", 1000, 20);

            blockChain.StartMining();

            Console.WriteLine($"The Blockchain is: {(blockChain.IsChainValid() ? "valid" : "invalid")}");


            //var keyValuePair = blockChain.CreateKeyPair();
            //string toEncode = "test";

            //var encoded = blockChain.Encrypt(keyValuePair.Item2, toEncode);

            //var decoded = blockChain.Decrypt(keyValuePair.Item1, encoded);

            //var wallet = new Wallet("test123");
            //wallet.Addresses = new List<Address>();
            //wallet.Addresses.Add(new Address() { Id = "1", PrivateKey = "2", PublicKey = "3" });

            //wallet.Save();

            //var decrypted = Wallet.Load("test123");

            //blockChain.StartMining();

            //Console.WriteLine($"Balance address1 {blockChain.GetBalanceOfAddress("address1")}");
            //Console.WriteLine($"Balance address2 {blockChain.GetBalanceOfAddress("address2")}");

            //var server = new Server();


            //var clientThread = new Thread(CreateClient);

            //clientThread.Start();

            //var client = new Client(blockChain);
            //Difficulty 200000 = 1 Second


            //Console.WriteLine($"The Blockchain is: {(blockChain.IsChainValid() ? "valid" : "invalid")}");
            //Random rndm = new Random();
            //for (int i = 0; i < 10000; i++)
            //{
            //    var blockDatas = new List<BlockData>();
            //    var dataCount = rndm.Next(20, 120);
            //    for (int i2 = 0; i2 < dataCount; i2++)
            //    {
            //        var nextBlockData = new BlockData()
            //        {
            //            FirstName = $"Vorname {i + 1} - {i2 + 1}",
            //            Name = $"Nachname {i + 1} - {i2 + 1}"
            //        };
            //        blockDatas.Add(nextBlockData);
            //    }

            //    var nextBlock = blockChain.NextBlock();

            //    nextBlock.Datas = blockDatas.ToArray();

            //    blockChain.AddBlock(nextBlock);

            //    Console.WriteLine(nextBlock);
            //}

            Console.ReadLine();
        }
    }
}
