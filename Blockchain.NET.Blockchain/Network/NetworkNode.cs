using Blockchain.NET.Blockchain.Network.Communication;
using Network;
using Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network
{
    public class NetworkNode
    {
        private readonly ServerConnectionContainer _serverConnectionContainer;
        private readonly BlockChain _blockChain;

        public NetworkNode(BlockChain blockChain)
        {
            _blockChain = blockChain;
            _serverConnectionContainer = ConnectionFactory.CreateServerConnectionContainer(1234, false);

            _serverConnectionContainer.ConnectionEstablished += connectionEstablished;
            _serverConnectionContainer.AllowUDPConnections = false;

            _serverConnectionContainer.Start();

            Console.WriteLine("Node started..");
        }

        private void connectionEstablished(Connection connection, ConnectionType type)
        {
            Console.WriteLine($"{_serverConnectionContainer.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");

            connection.RegisterStaticPacketHandler<SyncBlockChainRequest>(syncBlockChainReceived);
            connection.RegisterStaticPacketHandler<GetBlockRequest>(getBlockRequestReceived);
        }

        private void syncBlockChainReceived(SyncBlockChainRequest packet, Connection connection)
        {
            var lastBlock = _blockChain.LastBlock();
            connection.Send(new SyncBlockChainResponse(lastBlock == null ? 0 : lastBlock.Height, packet));
        }

        private void getBlockRequestReceived(GetBlockRequest packet, Connection connection)
        {
            var foundBlock = _blockChain.GetBlock(packet.BlockNumber);
            connection.Send(new GetBlockResponse(foundBlock, packet));
        }
    }
}
