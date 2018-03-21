using Blockchain.NET.Blockchain.Network.Communication;
using Blockchain.NET.Blockchain.Network.Helpers;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blockchain.NET.Blockchain.Network
{
    public class Client
    {
        private readonly BlockChain _blockChain;
        private readonly IPAddress _localIPAddress;
        private readonly IPAddress _subnetMask;

        private List<ClientConnectionContainer> _connections;

        private const int ServerPort = 1234;

        public Client(BlockChain blockChain)
        {
            _blockChain = blockChain;
            _localIPAddress = NetworkHelper.GetLocalIPAddress();
            _subnetMask = NetworkHelper.GetSubnetMask(_localIPAddress);
            _connections = new List<ClientConnectionContainer>();
            new Thread(establishConnections).Start();
        }

        private async void syncThread()
        {
            while (true)
            {
                foreach (var connection in _connections)
                {
                    var syncResponse = await connection.SendAsync<SyncBlockChainResponse>(new SyncBlockChainRequest(450));

                    var lastBlock = _blockChain.LastBlock();
                    if (lastBlock.Height < syncResponse.LastBlockNumber)
                    {
                        //Parallel.For(lastBlock.Index + 1, syncResponse.LastBlockNumber, index =>
                        //  {
                        //      var blockResponse = connection.SendAsync<GetBlockResponse>(new GetBlockRequest(index)).Result;
                        //      _blockChain.AddBlock(blockResponse.Block, blockResponse.Block.Hash);
                        //  });
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void establishConnections()
        {
            var possibleIpAddresses = NetworkHelper.GetAllPossibleIPAddresses(_localIPAddress, _subnetMask);

            while (true)
            {
                List<IPAddress> foundAddresses = new List<IPAddress>();

                Parallel.ForEach(possibleIpAddresses, (possibleIPAddress) =>
                {
                    if (possibleIPAddress != _localIPAddress && NetworkHelper.IsPortOpen(_localIPAddress, ServerPort))
                    {
                        foundAddresses.Add(possibleIPAddress);
                    }
                });

                Parallel.ForEach(foundAddresses, (foundAddress) =>
                {
                    var _connectionContainer = ConnectionFactory.CreateClientConnectionContainer(foundAddress.ToString(), ServerPort);

                    _connectionContainer.ConnectionEstablished += (connection, type) =>
                    {

                        Console.WriteLine($"{type.ToString()} Connection established");
                        new Thread(syncThread).Start();
                    };
                });

                Thread.Sleep(2000);
            }
        }
    }
}
