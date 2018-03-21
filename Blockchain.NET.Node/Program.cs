using Blockchain.NET.Blockchain;
using Blockchain.NET.Blockchain.Network;
using Blockchain.NET.Core;
using Blockchain.NET.Core.Helpers;
using Blockchain.NET.Core.Helpers.Cryptography;
using Blockchain.NET.Core.Mining;
using Blockchain.NET.Core.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Blockchain.NET.Node
{
    class Program
    {
        static void Main(string[] args)
        {
            var wallet = Wallet.Load("test123");
            var blockChain = new BlockChain(wallet);

            //var foundAddress = wallet.Addresses.FirstOrDefault(add => add.Key.Contains("605614"));

            //var newAddress = wallet.NewAddress();

            //var testTransaction = new Transaction(new[] { newAddress.Key }, newAddress.PrivateKey, newAddress.PublicKey, 10, new[] { "fac462ef4f07400698c81d920a19f8fcdd75609d", wallet.NewAddress().Key });

            //blockChain.AddTransaction(testTransaction, new[] { newAddress.PublicKey });

            Console.WriteLine($"Wallet Balance is: {wallet.GetBalance()}");

            //blockChain.StartMining();

            //Console.WriteLine($"The Blockchain is: {(blockChain.IsChainValid() ? "valid" : "invalid")}");

            Console.ReadLine();
        }
    }
}
