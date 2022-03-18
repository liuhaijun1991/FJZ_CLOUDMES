using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.Common
{
    public class PublicHelp
    {
        //获取内网IP
        public static IPAddress GetInternalIP()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics)
            {
                foreach (var uni in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (uni.Address.AddressFamily == AddressFamily.InterNetwork && uni.Address.ToString().StartsWith("10."))
                    {
                        return uni.Address;
                    }
                }
            }
            return null;
        }
    }
}
