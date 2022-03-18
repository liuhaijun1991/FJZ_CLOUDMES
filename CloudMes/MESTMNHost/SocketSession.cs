using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MESCMCHost
{
    public class SocketSession
    {
        public Socket ClientSocket { get; set; }
        public string TK { get; set; }
        public string IP { get; set; }

        public SocketSession(Socket _Socket, string _TK, string _IP)
        {
            this.ClientSocket = _Socket;
            this.TK = _TK;
            this.IP = _IP;
        }

        public string GetStringIp()
        {
            string Ip = ((IPEndPoint)ClientSocket.RemoteEndPoint).Address.ToString();
            return Ip;
        }
    }
}
