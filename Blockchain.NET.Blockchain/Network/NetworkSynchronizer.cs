using Blockchain.NET.Blockchain.Network.Communication;
using Blockchain.NET.Blockchain.Network.Helpers;
using Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Blockchain.NET.Blockchain.Network
{
    public class NetworkSynchronizer
    {
        private readonly BlockChain _blockChain;
        private readonly NetworkConnector _networkConnector;

        public List<Connection> Connections { get; set; }

        public NetworkSynchronizer(BlockChain blockChain)
        {
            _blockChain = blockChain;
            _networkConnector = new NetworkConnector(this);
            Connections = new List<Connection>();
        }

        public void SyncBlockChainReceived(SyncBlockChainRequest packet, Connection connection)
        {
            var lastBlock = _blockChain.LastBlock();
            connection.Send(new SyncBlockChainResponse(lastBlock == null ? 0 : lastBlock.Height, packet));
        }

        public void GetBlockRequestReceived(GetBlockRequest packet, Connection connection)
        {
            var foundBlock = _blockChain.GetBlock(packet.BlockNumber);
            connection.Send(new GetBlockResponse(foundBlock, packet));
        }
    }
}
