using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;
using WebSocketSharp;

namespace MESHelper.Common
{
    public class MESSocketClient
    {
        public string ClientID;
        public string Token;
        public string Message;

        string _MESServer = "";
        string UserName = "";
        string PWD = "";
        string BU = "";
        bool Open = false;
        WebSocket ws = null;
        public MESSocketClient(string _Server, string _user, string _pwd, string _bu)
        {
            _MESServer = _Server;
            UserName = _user;
            PWD = _pwd;
            BU = _bu;
            ws = new WebSocket("ws://" + _MESServer + "/ReportService");

            ws.OnClose += Ws_OnClose;
            ws.OnOpen += Ws_OnOpen;
            ws.Connect();
            Login(_user, _pwd, _bu, OnLogin);
        }

        public void Renew() {
            ws = new WebSocket("ws://" + _MESServer + "/ReportService");
            ws.OnClose += Ws_OnClose;
            ws.OnOpen += Ws_OnOpen;
            ws.Connect();
            Login(UserName, PWD, BU, OnLogin);
        }
        
        private void Ws_OnOpen(object sender, EventArgs e)
        {
            Open = true;
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Open = false;
        }

        private void OnLogin(object sender, MessageEventArgs e)
        {
            WebSocket w = (WebSocket)sender;
            w.OnMessage -= OnLogin;
            JObject Request = (JObject)JsonConvert.DeserializeObject(e.Data);
            if (Request["Status"].ToString() == "Pass")
            {
                JObject Data = (JObject)Request["Data"];
                this.Token = Data["Token"].ToString();
            }
            else
            {
                Message = Request["Message"].ToString();
            }
        }

        private void CallFunction(string ClassName, string FunctionName, object Data, EventHandler<MessageEventArgs> _Handler)
        {
            string MessageID = DateTime.Now.ToLongTimeString();
            CallFunction(ClassName, FunctionName, MessageID, Data, _Handler);
        }

        private void CallFunction(string ClassName, string FunctionName, string MessageID, object Data, EventHandler<MessageEventArgs> _Handler)
        {
            var data = new { Token = this.Token, ClientID = this.ClientID, MessageID = MessageID, Class = ClassName, Function = FunctionName, Data = Data };
            string dataStr = JsonConvert.SerializeObject(data);
            if (ws.ReadyState != WebSocketState.Open)
            {
                ws.Connect();
                Login(UserName, PWD, BU, OnLogin);
            }
            ws.OnMessage += _Handler;
            ws.Send(dataStr);
        }

        private void CallFunctionSync(string ClassName, string FunctionName, object Data, EventHandler<MessageEventArgs> _Handler)
        {
            string MessageID = DateTime.Now.ToLongTimeString();
            var data = new { Token = this.Token, ClientID = this.ClientID, MessageID = MessageID, Class = ClassName, Function = FunctionName, Data = Data };
            string dataStr = JsonConvert.SerializeObject(data);
            if (ws.ReadyState != WebSocketState.Open)
            {
                ws.Connect();
                Login(UserName, PWD, BU, OnLogin);
            }
            if (this.Token == null)
            {
                Login(UserName, PWD, BU, OnLogin);
            }
            ws.OnMessage += _Handler;
            ws.SyncRequest = null;
            ws.Send(dataStr);
            System.Threading.Thread T = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Wait));
            T.Start(ws);
            T.Join(100000);
            if (ws.SyncRequest == null)
            {
                //throw new Exception($@"Call Funtion Time Out Function:'{FunctionName}' Data:'{dataStr}'");
            }

        }

        void Wait(object e)
        {
            WebSocket ws = (WebSocket)e;
            while (ws.SyncRequest == null)
            {
                System.Threading.Thread.Sleep(100);
            }
        }


        private void Login(string _User, string _PWD, string _BU, EventHandler<MessageEventArgs> _Handler)
        {
            var data = new { EMP_NO = _User, Password = _PWD, Language = "CHINESE", BU_NAME = _BU };
            CallFunction("MESStation.MESUserManager.UserManager", "Login", data, _Handler);
        }

        public void Login()
        {
            Login(UserName, PWD, BU, OnLogin);
        }

        public void GetFile(string Name, string Type, EventHandler<MessageEventArgs> _handler)
        {
            var data = new { Name = Name, UseType = Type };
            CallFunctionSync("MESStation.FileUpdate.FileUpload", "GetFileByName", data, _handler);
        }

        public string MESServer
        {
            get { return _MESServer; }
        }

    }
}
