using Blockchain.NET.Blockchain.Network.Settings;
using Blockchain.NET.Core.Mining;
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

        private Thread _syncBlockChainThread;
        private Thread _connectNodesThread;
        private Thread _transactionsThread;

        public bool IsSyncing { get; set; }
        private static Random rnd = new Random();

        private NodeList _nodeList;
        private List<NodeConnection> _connections;

        public NetworkSynchronizer(BlockChain blockChain)
        {
            _blockChain = blockChain;
            _nodeList = NodeList.Load();
        }

        public void Start()
        {
            if (!IsSyncing)
            {
                IsSyncing = true;
                if (_syncBlockChainThread == null || _syncBlockChainThread.ThreadState != ThreadState.Running)
                {
                    _syncBlockChainThread = new Thread(syncBlocksThread);
                    _connectNodesThread = new Thread(connectNodesThread);
                    _transactionsThread = new Thread(syncTransactionsThread);
                }
                _syncBlockChainThread.Start();
                _connectNodesThread.Start();
            }
        }

        public void Stop()
        {
            IsSyncing = false;
            _syncBlockChainThread.Abort();
        }

        private async void connectNodesThread()
        {
            _connections = new List<NodeConnection>();
            while (true)
            {
                if (_connections.Count < 5)
                    foreach (var node in _nodeList.Nodes.Where(n => !_connections.Select(c => c.NodeAddress).Contains(n.NodeAddress)))
                    {
                        if (node.LastConnectionAttempt.HasValue)
                        {
                            if (node.LastConnectionAttempt.Value < DateTime.Now.AddSeconds(30))
                            {
                                var newConnection = new NodeConnection(node.NodeAddress);
                                if (await newConnection.Health())
                                {
                                    _connections.Add(newConnection);
                                    node.LastConnectionAttempt = null;
                                }
                            }
                        }
                    }

                var lostConnections = new List<NodeConnection>();
                foreach (var connection in _connections)
                {
                    if (!await connection.Health())
                    {
                        lostConnections.Add(connection);
                    }
                }
                lostConnections.ForEach(lc => _connections.Remove(lc));

                await Task.Delay(4000);
            }
        }

        private async void syncBlocksThread()
        {
            while (IsSyncing)
            {
                var lastBlock = _blockChain.LastBlock();
                var nextBlockHeight = lastBlock == null ? 1 : _blockChain.LastBlock().Height + 1;
                var resultRemoteChainStates = new List<Tuple<NodeConnection, int>>();
                foreach (var connection in _connections.ToList())
                {
                    var lastBlockHeight = await connection.LastBlockHeight();
                    if (lastBlockHeight > nextBlockHeight)
                    {
                        resultRemoteChainStates.Add(new Tuple<NodeConnection, int>(connection, lastBlockHeight));
                        break;
                    }
                }
                if (resultRemoteChainStates.Count > 0)
                {
                    var from = nextBlockHeight;
                    var to = resultRemoteChainStates.Select(t => t.Item2).Max();
                    if (to > from + 40)
                        to = from + 40;
                    loadBlocks(resultRemoteChainStates, from, to);
                }
                else
                    await Task.Delay(8000);
            }
        }

        private void loadBlocks(List<Tuple<NodeConnection, int>> resultRemoteChainStates, int from, int to)
        {
            var tasks = new List<Task<Block>>();
            for (int i = from; i <= to; i++)
            {
                var connection = resultRemoteChainStates.Where(rc => rc.Item2 >= i).OrderBy(x => rnd.Next()).FirstOrDefault();
                if (connection != null)
                {
                    tasks.Add(connection.Item1.GetBlock(i));
                }
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var taskResult in tasks.Select(t => t.Result))
            {
                if (taskResult != null)
                    _blockChain.AddBlock(taskResult);
            }
        }

        private async void syncTransactionsThread()
        {
            while (IsSyncing)
            {
                var localMempoolHashes = _blockChain.MemPool.Select(t => t.GenerateHash());
                var resultRemoteChainStates = new List<Tuple<NodeConnection, List<string>>>();
                foreach (var connection in _connections.ToList())
                {
                    var mempoolHashes = await connection.MempoolHashes();

                    var notExistingTransactions = mempoolHashes.Where(mp => !localMempoolHashes.Any(lmp => lmp == mp)).ToList();

                    if (notExistingTransactions.Count > 0)
                    {
                        resultRemoteChainStates.Add(new Tuple<NodeConnection, List<string>>(connection, notExistingTransactions));
                        break;
                    }
                }
                loadTransactions(resultRemoteChainStates);
                await Task.Delay(8000);
            }
        }

        private void loadTransactions(List<Tuple<NodeConnection, List<string>>> resultRemoteChainStates)
        {
            var tasks = new List<Task<List<Transaction>>>();
            foreach (var connection in resultRemoteChainStates)
            {
                tasks.Add(connection.Item1.GetTransactions(connection.Item2));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (var taskResult in tasks.Select(t => t.Result))
            {
                if (taskResult != null)
                {
                    foreach (var transaction in taskResult)
                    {
                        _blockChain.AddTransaction(transaction);
                    }
                }
            }
        }

        public async void BroadcastBlock(Block block)
        {
            if (_connections != null)
                foreach (var connection in _connections.ToList())
                {
                    await connection.PushBlock(block);
                }
        }

        public async void BroadcastTransaction(Transaction transaction)
        {
            if (_connections != null)
                foreach (var connection in _connections.ToList())
            {
                await connection.PushTransaction(transaction);
            }
        }
    }
}
