using Blockchain.NET.Blockchain.Network.Communication;
using Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network
{
    public class NetworkHandler
    {
        private readonly BlockChain _blockChain;

        public NetworkHandler(BlockChain blockChain)
        {
            _blockChain = blockChain;
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
