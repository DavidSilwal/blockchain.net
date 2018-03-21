using Blockchain.NET.Core;
using Network;
using Network.Attributes;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Communication
{
    [PacketRequest(typeof(GetBlockRequest))]
    public class GetBlockResponse : ResponsePacket
    {
        public GetBlockResponse()
        {

        }

        public GetBlockResponse(Block block, RequestPacket request)
            : base(request)
        {
            Block = block;
        }

        public Block Block { get; set; }
    }
}
