using Blockchain.NET.Blockchain;
using Blockchain.NET.Blockchain.Network;
using Blockchain.NET.Core.Wallet;
using System;

namespace Blockchain.NET.Client
{
    class Program
    {
        public static Wallet Wallet { get; set; }

        public static BlockChain BlockChain { get; set; }

        static void Main(string[] args)
        {
            Wallet = Wallet.Load("test123");
            BlockChain = new BlockChain(Wallet);
            BlockChain.StartSyncronizing();
            BlockChain.StartMining();

            Console.ReadLine();
        }
    }
}
