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

        public SyncBlockChainResponse(int lastBlockHeight, RequestPacket request)
            : base(request)
        {
            this.LastBlockHeight = lastBlockHeight;
        }

        public int LastBlockHeight { get; set; }
    }
}
