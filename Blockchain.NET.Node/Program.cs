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

            var newAddress = wallet.Addresses.FirstOrDefault();

            //var testTransaction = new Transaction(new[] { new Input(newAddress.Key) }.ToList(), wallet, new[] { new Output("11x0ab02006ec824714f78fab52d4b153f9a0dbebf9", 200) }.ToList());

            //blockChain.AddTransaction(testTransaction);

            Console.WriteLine($"Wallet Balance is: {wallet.GetBalance()}");

            //blockChain.StartMining();

            //Console.WriteLine($"The Blockchain is: {(blockChain.IsChainValid() ? "valid" : "invalid")}");

            Console.ReadLine();
        }
    }
}
