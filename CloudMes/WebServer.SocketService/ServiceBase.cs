using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace WebServer.SocketService
{
    public class ServiceBase: WebSocketBehavior
    {
        public string ClientIP { get; set; }
        public string ClientPort { get; set; }

        public delegate void SocketOpen(object sender, EventArgs e);
        public delegate void SocketClose(object sender, CloseEventArgs e);
        public delegate void SocketError(object sender, ErrorEventArgs e);
        public delegate void SocketMessage(object sender, MessageEventArgs e);
        public event SocketOpen OnSocketOpen;
        public event SocketClose OnSocketClose;
        public event SocketError OnSocketError;
        public event SocketMessage OnSocketMessage;

        public List<OleExec> UseDbs { get; set; } = new List<OleExec>();

        //public List<IWebSocketSession> sessions { get { return this.Sessions.Sessions.ToList(); } }

        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "TEST"
                      ? "I've been balused already..."
                      : "Hello client!";

            Send(msg);
            if (OnSocketMessage != null)
            {
                OnSocketMessage(this, e);
            }
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            ClientIP = this.Context.UserEndPoint.Address.ToString();
            #region 增加客戶端連接數限制,add by Eden
            try
            {
                int maxconnum = Convert.ToInt32(ConfigurationManager.AppSettings["ClientMaxConnection"]);
                string[] serveraddress = ConfigurationManager.AppSettings["serveraddress"].Split(',');
                if (!serveraddress.Contains(ClientIP) && this.Sessions.Sessions.ToList().FindAll(t => t.Context.UserEndPoint.Address.ToString() == ClientIP)
                        .Count > maxconnum && maxconnum > 0)
                {
                    Context.WebSocket.Close(CloseStatusCode.ServerError, $@"The maximum number of client connections is {maxconnum}, please contact the administrator!");
                    return;
                }
            }
            catch{}
            #endregion
            ClientPort = this.Context.UserEndPoint.Port.ToString();
            if (OnSocketOpen != null)
            {
                OnSocketOpen(this, new EventArgs());
            }
            
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            if (OnSocketClose != null)
            {
                OnSocketClose(this, e);
            }
            ClientIP = null;
            ClientPort = null;
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            if (OnSocketError != null)
            {
                OnSocketError(this, e);
            }
        }
        public void SendDataToClient(string data)
        {
            Send(data);
        }

    }
}
