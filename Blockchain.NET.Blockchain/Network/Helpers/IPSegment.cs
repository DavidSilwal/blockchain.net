using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Helpers
{
    public class IPSegment
    {
        private UInt32 _ip;
        private UInt32 _mask;

        public IPSegment(string ip, string mask)
        {
            _ip = NetworkHelper.ParseIp(ip);
            _mask = NetworkHelper.ParseIp(mask);
        }

        public UInt32 NumberOfHosts
        {
            get { return ~_mask + 1; }
        }

        public UInt32 NetworkAddress
        {
            get { return _ip & _mask; }
        }

        public UInt32 BroadcastAddress
        {
            get { return NetworkAddress + ~_mask; }
        }

        public IEnumerable<UInt32> Hosts()
        {
            for (var host = NetworkAddress + 1; host < BroadcastAddress; host++)
            {
                yield return host;
            }
        }
    }
}
