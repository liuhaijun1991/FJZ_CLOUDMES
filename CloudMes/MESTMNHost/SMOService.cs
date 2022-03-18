using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MESCMCHost
{
    public class SMOService
    {
        int Port { get; set; }
        string IP { get; set; }
        //Socket sok;

        public SMOService(string _IP, int _Port)
        {
            Port= _Port;
            IP = _IP;
        }
        

        public string GetStringIp(Socket _socket)
        {
            string Ip = ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
            return Ip;
        }
    }
}
