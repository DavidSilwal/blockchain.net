using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Blockchain.NET.Blockchain;
using Blockchain.NET.Blockchain.Network;
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
            BlockChain.StartSyncronizing();

            BlockChain.StartMining();

            //NetworkConnector.IsMainNode = true;

            //RSAHelper.test();
            //Console.ReadLine();



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
#if DEBUG
#else
            BlockChain.StartMining();
#endif

            //Console.ReadLine();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
