using Blockchain.NET.Blockchain.Network.Communication;
using Network;
using Network.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Blockchain.NET.Blockchain.Network
{
    public class NetworkConnector
    {
        private readonly BlockChain _blockChain;
        private readonly IPAddress _localIPAddress;
        private readonly NetworkHandler _networkHandler;

        private List<ClientConnectionContainer> _clientConnections;

        private ServerConnectionContainer _serverConnection;

        public int ServerPort { get; set; } = 1234;

        private bool _isRunning;

        public NetworkConnector(BlockChain blockChain)
        {
            _blockChain = blockChain;
            _networkHandler = new NetworkHandler(_blockChain);
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                if(_serverConnection == null)
                {
                    _serverConnection = ConnectionFactory.CreateServerConnectionContainer(ServerPort, false);
                    _serverConnection.ConnectionEstablished += connectionEstablished;
                    _serverConnection.AllowUDPConnections = false;
                    _serverConnection.Start();
                }
            }
        }

        public void Stop()
        {
            _serverConnection.Stop();
            _isRunning = false;
        }

        private void connectionEstablished(Connection connection, ConnectionType type)
        {
            Console.WriteLine($"{_serverConnection.Count} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");

            connection.RegisterStaticPacketHandler<SyncBlockChainRequest>(_networkHandler.SyncBlockChainReceived);
            connection.RegisterStaticPacketHandler<GetBlockRequest>(_networkHandler.GetBlockRequestReceived);
        }
    }
}
