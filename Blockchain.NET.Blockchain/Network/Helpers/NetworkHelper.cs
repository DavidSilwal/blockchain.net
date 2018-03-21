using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Blockchain.NET.Blockchain.Network.Helpers
{
    public class NetworkHelper
    {
        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }

        public static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            return null;
        }

        public static bool IsPortOpen(IPAddress host, int port)
        {
            var timeout = 1000;
            var result = false;
            using (var client = new TcpClient())
            {
                try
                {
                    client.ReceiveTimeout = timeout * 1000;
                    client.SendTimeout = timeout * 1000;
                    var asyncResult = client.BeginConnect(host, port, null, null);
                    var waitHandle = asyncResult.AsyncWaitHandle;
                    try
                    {
                        if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeout), false))
                        {
                            client.Close();
                        }
                        else
                        {
                            result = client.Connected;
                        }
                        client.EndConnect(asyncResult);
                    }
                    finally
                    {
                        waitHandle.Close();
                    }
                }
                catch { }
            }
            return result;
        }

        public static List<IPAddress> GetAllPossibleIPAddresses(IPAddress ip, IPAddress subnetmask)
        {
            var possibleIPAddresses = new List<IPAddress>();

            var ipSegment = new IPSegment(ip.ToString(), subnetmask.ToString());

            foreach (var host in ipSegment.Hosts())
            {
                possibleIPAddresses.Add(IPAddress.Parse(ToIpString(host)));
            }

            return possibleIPAddresses;
        }

        public static string ToIpString(UInt32 value)
        {
            var bitmask = 0xff000000;
            var parts = new string[4];
            for (var i = 0; i < 4; i++)
            {
                var masked = (value & bitmask) >> ((3 - i) * 8);
                bitmask >>= 8;
                parts[i] = masked.ToString(CultureInfo.InvariantCulture);
            }
            return String.Join(".", parts);
        }

        public static UInt32 ParseIp(string ipAddress)
        {
            var splitted = ipAddress.Split('.');
            UInt32 ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = (ip << 8) + UInt32.Parse(splitted[i]);
            }
            return ip;
        }
    }
}
