using Blockchain.NET.Core.Mining;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Communication
{
    public class PushBlockRequest : RequestPacket
    {
        public PushBlockRequest()
        {

        }

        public PushBlockRequest(Block block)
        {
            Block = block;
        }

        public Block Block { get; set; }
    }
}
