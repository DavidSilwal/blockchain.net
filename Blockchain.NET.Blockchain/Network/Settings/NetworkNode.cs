using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Settings
{
    public class NetworkNode
    {
        public string NodeEnpointAddress { get; set; }

        [JsonIgnore]
        public IPAddress IPAddress
        {
            get { return string.IsNullOrEmpty(NodeEnpointAddress) ? null : IPAddress.Parse(NodeEnpointAddress); }
            set
            {
                NodeEnpointAddress = value == null ? null : value.ToString();
            }
        }

        public DateTime LastConnectionAttempt { get; set; }

        [JsonIgnore]
        public bool IsConnected { get; set; }
    }
}
