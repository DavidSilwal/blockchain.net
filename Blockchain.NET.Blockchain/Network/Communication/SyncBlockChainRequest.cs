using Network;
using Network.Attributes;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Communication
{
    public class SyncBlockChainRequest : RequestPacket
    {
        public SyncBlockChainRequest()
        {

        }

        public SyncBlockChainRequest(int lastBlockNumber)
        {
            this.LastBlockNumber = lastBlockNumber;
        }

        public int LastBlockNumber { get; set; }
    }
}
