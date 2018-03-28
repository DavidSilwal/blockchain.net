using Blockchain.NET.Core.Mining;
using Network;
using Network.Attributes;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Communication
{
    [PacketRequest(typeof(LoadBlockRequest))]
    public class LoadBlockResponse : ResponsePacket
    {
        public LoadBlockResponse()
        {

        }

        public LoadBlockResponse(Block block, RequestPacket request)
            : base(request)
        {
            Block = block;
        }

        public Block Block { get; set; }
    }
}
