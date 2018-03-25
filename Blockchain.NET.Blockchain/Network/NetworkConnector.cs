using Blockchain.NET.Blockchain.Network.Communication;
using Blockchain.NET.Blockchain.Network.Helpers;
using Blockchain.NET.Blockchain.Network.Settings;
using Network;
using Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Blockchain.NET.Blockchain.Network
{
    public class NetworkConnector
    {
        private readonly NetworkSynchronizer _networkSynchronizer;
        private readonly NodeList _nodeList;
        private readonly IPAddress _localIPAddress;

        private List<ClientConnectionContainer> _clientConnections;

        private ServerConnectionContainer _serverConnection;

        public int ServerPort { get; set; } = 1234;

        private bool _isRunning;

        public NetworkConnector(NetworkSynchronizer networkSynchronizer)
        {
            _localIPAddress = NetworkHelper.GetLocalIPAddress();
            _networkSynchronizer = networkSynchronizer;
            _nodeList = NodeList.Load();
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _clientConnections = new List<ClientConnectionContainer>();
                _isRunning = true;
                if (_serverConnection == null)
                {
                    _serverConnection = ConnectionFactory.CreateServerConnectionContainer(ServerPort, false);
                    _serverConnection.ConnectionEstablished += connectionEstablished;
                    _serverConnection.ConnectionLost += connectionLost;
                    _serverConnection.AllowUDPConnections = false;
                    _serverConnection.Start();
                    new Thread(establishConnections).Start();
                }
            }
        }

        public void Stop()
        {
            _serverConnection.Stop();
            foreach (var clientConnection in _clientConnections)
            {
                clientConnection.Shutdown(CloseReason.ServerClosed);
            }
            _isRunning = false;
        }

        private void connectionEstablished(Connection connection, ConnectionType type)
        {
            Console.WriteLine($"{connection.IPRemoteEndPoint.Address} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");

            connection.RegisterStaticPacketHandler<SyncBlockChainRequest>(_networkSynchronizer.SyncBlockChainReceived);
            connection.RegisterStaticPacketHandler<GetBlockRequest>(_networkSynchronizer.GetBlockRequestReceived);

            _networkSynchronizer.Connections.Add(connection);

            if (!_nodeList.Nodes.Any(n => n.IPAddress == connection.IPRemoteEndPoint.Address))
            {
                _nodeList.Nodes.Add(new NetworkNode() { IPAddress = connection.IPRemoteEndPoint.Address });
                _nodeList.Save();
            }
        }

        private void connectionLost(Connection connection, ConnectionType type, CloseReason reason)
        {
            Console.WriteLine($"{connection.IPRemoteEndPoint.Address} {connection.GetType()} connection lost");

            _networkSynchronizer.Connections.Remove(connection);

            _clientConnections.Remove(_clientConnections.First(c => c.TcpConnection.IPRemoteEndPoint.Address == connection.IPRemoteEndPoint.Address));

            if (!_nodeList.Nodes.Any(n => n.IPAddress == connection.IPRemoteEndPoint.Address))
            {
                _nodeList.Nodes.Add(new NetworkNode() { IPAddress = connection.IPRemoteEndPoint.Address });
                _nodeList.Save();
            }
        }

        private void establishConnections()
        {
            while (_isRunning)
            {
                foreach (var networkNode in _nodeList.Nodes.Where(nl => nl.IPAddress != _localIPAddress))
                {
                    if (!_networkSynchronizer.Connections.Any(c => c.IPRemoteEndPoint.Address == networkNode.IPAddress))
                    {
                        if (networkNode.LastConnectionAttempt < DateTime.Now.AddSeconds(-60))
                        {
                            if (NetworkHelper.IsPortOpen(networkNode.IPAddress, ServerPort))
                            {
                                var _connectionContainer = ConnectionFactory.CreateClientConnectionContainer(networkNode.IPAddress.ToString(), ServerPort);

                                _connectionContainer.ConnectionEstablished += connectionEstablished;
                                _connectionContainer.ConnectionLost += connectionLost;

                                _clientConnections.Add(_connectionContainer);
                            }
                            networkNode.LastConnectionAttempt = DateTime.Now;
                        }
                    }
                }
                Thread.Sleep(4000);
            }
            _nodeList.Save();
        }
    }
}
