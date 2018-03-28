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
using System.Threading.Tasks;

namespace Blockchain.NET.Blockchain.Network
{
    public class NetworkConnector
    {
        private readonly NetworkSynchronizer _networkSynchronizer;
        private readonly NodeList _nodeList;
        private readonly IPAddress _localIPAddress;

        private ServerConnectionContainer _serverConnection;

        public static bool IsMainNode { get; set; }

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
                _isRunning = true;
                if (_serverConnection == null && IsMainNode)
                {
                    _serverConnection = ConnectionFactory.CreateServerConnectionContainer(ServerPort, false);
                    _serverConnection.ConnectionEstablished += connectionEstablished;
                    _serverConnection.ConnectionLost += connectionLost;
                    _serverConnection.AllowUDPConnections = false;
                    _serverConnection.Start();
                }
                if (!IsMainNode)
                    new Thread(establishConnections).Start();
            }
        }

        public void Stop()
        {
            _serverConnection.Stop();
            foreach (var connection in _networkSynchronizer.Connections)
            {
                connection.Close(CloseReason.ServerClosed);
            }

            _isRunning = false;
        }

        private void connectionEstablished(Connection connection, ConnectionType type)
        {
            if (type == ConnectionType.TCP)
            {
                Console.WriteLine($"{connection.IPRemoteEndPoint.Address} {connection.GetType()} connected on port {connection.IPRemoteEndPoint.Port}");

                connection.RegisterStaticPacketHandler<SyncBlockChainRequest>(_networkSynchronizer.SyncBlockChainReceived);
                connection.RegisterStaticPacketHandler<LoadBlockRequest>(_networkSynchronizer.LoadBlockReceived);

                _networkSynchronizer.Connections.Add(connection);
            }
        }

        private void connectionLost(Connection connection, ConnectionType type, CloseReason reason)
        {
            if (type == ConnectionType.TCP && reason != CloseReason.InvalidUdpRequest)
            {
                _networkSynchronizer.Connections.Remove(connection);

                //if (connection != null && connection.IPRemoteEndPoint != null)
                //{
                //    var foundNetworknode = _nodeList.Nodes.FirstOrDefault(n => n.NodeEnpointAddress == connection.IPRemoteEndPoint.Address.ToString());
                //    if (foundNetworknode != null)
                //        foundNetworknode.IsConnected = false;
                //}
            }
        }

        private async void establishConnections()
        {
            while (_isRunning)
            {
                foreach (var networkNode in _nodeList.Nodes.Where(nl => !nl.IsConnected))
                {
                    if (networkNode.LastConnectionAttempt < DateTime.Now.AddSeconds(-60))
                    {
                        if (NetworkHelper.IsPortOpen(networkNode.IPAddress, ServerPort))
                        {
                            var _connectionContainer = ConnectionFactory.CreateClientConnectionContainer(networkNode.IPAddress.ToString(), ServerPort);

                            _connectionContainer.ConnectionEstablished += connectionEstablished;
                            _connectionContainer.ConnectionLost += connectionLost;

                            networkNode.IsConnected = true;
                        }
                        //networkNode.LastConnectionAttempt = DateTime.Now;
                    }

                }
                await Task.Delay(4000);
            }
        }
    }
}
