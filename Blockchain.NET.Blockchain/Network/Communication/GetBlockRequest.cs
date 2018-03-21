using Network;
using Network.Attributes;
using Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Communication
{
    public class GetBlockRequest : RequestPacket
    {
        public GetBlockRequest()
        {

        }

        public GetBlockRequest(int blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public int BlockNumber { get; set; }
    }
}
