using Blockchain.NET.Blockchain.Network.Communication;
using Blockchain.NET.Blockchain.Network.Helpers;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blockchain.NET.Blockchain.Network
{
    public class NetworkSynchronizer
    {
        private readonly BlockChain _blockChain;
        private readonly NetworkConnector _networkConnector;

        private Thread _syncBlockChainThread;

        public List<Connection> Connections { get; set; }

        public bool IsActive { get; set; }
        private static Random rnd = new Random();

        public NetworkSynchronizer(BlockChain blockChain)
        {
            _blockChain = blockChain;
            _networkConnector = new NetworkConnector(this);
            Connections = new List<Connection>();
        }

        public void Start()
        {
            if (!IsActive)
            {
                IsActive = true;
                _networkConnector.Start();
                if (_syncBlockChainThread == null || _syncBlockChainThread.ThreadState != ThreadState.Running)
                    _syncBlockChainThread = new Thread(syncBlockChainThread);
                _syncBlockChainThread.Start();
            }
        }

        public void Stop()
        {
            IsActive = false;
            _networkConnector.Stop();
            _syncBlockChainThread.Abort();
        }

        public void SyncBlockChainReceived(SyncBlockChainRequest packet, Connection connection)
        {
            var lastBlock = _blockChain.LastBlock();
            connection.Send(new SyncBlockChainResponse(lastBlock == null ? 0 : lastBlock.Height, packet));
        }

        public void LoadBlockReceived(LoadBlockRequest packet, Connection connection)
        {
            var foundBlock = _blockChain.GetBlock(packet.BlockHeight);
            connection.Send(new LoadBlockResponse(foundBlock, packet));
        }

        private async void syncBlockChainThread()
        {
            while (IsActive)
            {
                List<Tuple<Connection, int>> resultRemoteChainStates = new List<Tuple<Connection, int>>();
                var lastBlock = _blockChain.LastBlock();
                var actualBlockHeight = lastBlock == null ? 0 : _blockChain.LastBlock().Height;
                foreach (var connection in Connections.ToList())
                {
                   var response = await connection.SendAsync<SyncBlockChainResponse>(new SyncBlockChainRequest());
                    if(response.LastBlockHeight > actualBlockHeight)
                    {
                        resultRemoteChainStates.Add(new Tuple<Connection, int>(connection, response.LastBlockHeight));
                        break;
                    }
                }
                if(resultRemoteChainStates.Count > 0)
                {
                    var from = actualBlockHeight;
                    if (from == 0)
                        from = 1;
                    var to = resultRemoteChainStates.Select(t => t.Item2).Max();
                    if (to > from + 100)
                        to = from + 100;
                    loadBlocks(resultRemoteChainStates, from, to);
                }
                await Task.Delay(10000);
            }
        }

        private async void loadBlocks(List<Tuple<Connection, int>> resultRemoteChainStates, int from, int to)
        {
            List<Task<LoadBlockResponse>> tasks = new List<Task<LoadBlockResponse>>();
            for (int i = from; i <= to; i++)
            {
                var connection = resultRemoteChainStates.OrderBy(x => rnd.Next()).FirstOrDefault();
                if(connection != null)
                {
                    var resul = await connection.Item1.SendAsync<LoadBlockResponse>(new LoadBlockRequest(i));
                    tasks.Add(connection.Item1.SendAsync<LoadBlockResponse>(new LoadBlockRequest(i)));
                }
            }
            Task.WaitAll(tasks.ToArray());
            foreach(var taskResult in tasks.Select(t => t.Result))
            {
                _blockChain.AddBlock(taskResult.Block);
            }
        }
    }
}
