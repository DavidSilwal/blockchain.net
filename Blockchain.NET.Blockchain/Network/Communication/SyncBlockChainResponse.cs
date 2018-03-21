using Network;
using Network.Attributes;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Communication
{
    [PacketRequest(typeof(SyncBlockChainRequest))]
    public class SyncBlockChainResponse : ResponsePacket
    {
        public SyncBlockChainResponse()
        {

        }

        public SyncBlockChainResponse(int lastBlockNumber, RequestPacket request)
            : base(request)
        {
            this.LastBlockNumber = lastBlockNumber;
        }

        public int LastBlockNumber { get; set; }
    }
}
