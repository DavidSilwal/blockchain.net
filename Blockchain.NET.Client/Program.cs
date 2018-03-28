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

        public static NetworkSynchronizer NetworkSynchronizer { get; set; }

        static void Main(string[] args)
        {
            Wallet = Wallet.Load("test123");
            BlockChain = new BlockChain(Wallet);

            NetworkConnector.IsMainNode = false;
            NetworkSynchronizer = new NetworkSynchronizer(BlockChain);
            NetworkSynchronizer.Start();

            Console.ReadLine();
        }
    }
}
