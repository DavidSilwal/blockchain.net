using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Settings
{
    public class NetworkNode
    {
        public string NodeAddress { get; set; }

        public DateTime? LastConnectionAttempt { get; set; }
    }
}
