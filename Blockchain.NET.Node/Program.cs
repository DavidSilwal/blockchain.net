using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Blockchain.NET.Blockchain;
using Blockchain.NET.Core.Helpers.Calculations;
using Blockchain.NET.Core.Helpers.Cryptography;
using Blockchain.NET.Core.Wallet;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Blockchain.NET.Node
{
    public class Program
    {
        public static Wallet Wallet { get; set; }

        public static BlockChain BlockChain { get; set; }

        public static void Main(string[] args)
        {
            Wallet = Wallet.Load("test123");
            BlockChain = new BlockChain(Wallet);

            //var difficulty = Math.Pow(2, 256);

            //int nonce = 1;

            //while(true)
            //{
            //    var blockHashBytes = HashHelper.Sha256Bytes(nonce.ToString());
            //    var hashValue = BitConverter.ToDouble(blockHashBytes, 0);
            //    if (hashValue >= 0 && hashValue < difficulty)
            //    {
            //        Console.WriteLine("test");
            //    }
            //    nonce++;
            //}



            //Console.ReadLine();

            //var newAddress = Wallet.Addresses.FirstOrDefault();

            //var testTransaction = new Transaction(new[] { new Input(newAddress.Key) }.ToList(), wallet, new[] { new Output("11x0ab02006ec824714f78fab52d4b153f9a0dbebf9", 200) }.ToList());

            //blockChain.AddTransaction(testTransaction);

            //Console.WriteLine($"Wallet Balance is: {Wallet.GetBalance()}");

           BlockChain.StartMining();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
