using Network;
using Network.Attributes;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Communication
{
    public class LoadBlockRequest : RequestPacket
    {
        public LoadBlockRequest(int blockHeight)
        {
            BlockHeight = blockHeight;
        }

        public int BlockHeight { get; set; }
    }
}
