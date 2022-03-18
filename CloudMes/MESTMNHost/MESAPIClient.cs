using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using static MESCMCHost.CMC503Scanda;
using System.Threading;

namespace MESCMCHost
{
    public class MESAPIClient
    {
        public static string Language = "ENGLISH";
        public static string BU_NAME = "HWD";
        public string MESConnStr = "";
        public string MES_USER = "";
        public string MES_PWD = "";
        public string Token = "";
        Random rand = new Random();
        WebSocket MESWebSocket;
        Dictionary<string, CatchObject> dirMesCallback = new Dictionary<string, CatchObject>();

        public delegate void OnMessageEventHandler(MESAPIClient sender, MesClientOnmessageEventArgs EventArg);

        public event OnMessageEventHandler OnMessage;

        public MESAPIClient(string _MESConnStr)
        {
            MESConnStr = _MESConnStr;
        }
        public MESAPIClient(string _MESConnStr, string _MES_USER, string _MES_PWD)
        {
            MESConnStr = _MESConnStr;
            MES_USER = _MES_USER;
            MES_PWD = _MES_PWD;
        }
        public void Connect()
        {
            if (MESWebSocket == null)
            {
                MESWebSocket = new WebSocket(MESConnStr);
                MESWebSocket.OnMessage += MESWebSocket_OnMessage;
                MESWebSocket.OnClose += MESWebSocket_OnClose;
            }
            try
            {
                MESWebSocket.Connect();
            }
            catch (Exception e)
            {
                MESWebSocket.OnMessage -= MESWebSocket_OnMessage;
                MESWebSocket.OnClose -= MESWebSocket_OnClose;
                MESWebSocket = null;
                throw e;
            }

        }

        public void DisConnect()
        {
            if (MESWebSocket == null)
            {
                return;
            }
            try
            {
                MESWebSocket.OnMessage -= MESWebSocket_OnMessage;
            }
            catch
            { }
            try
            {
                MESWebSocket.OnClose -= MESWebSocket_OnClose;
            }
            catch
            { }
            MESWebSocket?.Close();
        }

        private void MESWebSocket_OnClose(object sender, CloseEventArgs e)
        {
            MESWebSocket.OnMessage -= MESWebSocket_OnMessage;
            MESWebSocket.OnClose -= MESWebSocket_OnClose;
            MESWebSocket = null;
        }

        private void MESWebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            string msgid = "";
            try
            {

                try
                {
                    JObject o = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(e.Data);
                    msgid = o["MessageID"].ToString();
                    if (dirMesCallback[msgid] != null)
                    {
                        MESCallBack call = dirMesCallback[msgid].Callback;

                        MesClientOnmessageEventArgs args = new MesClientOnmessageEventArgs();
                        args.Call = call;
                        args.Data = o;
                        args.Scanda = dirMesCallback[msgid].Scanda;
                        if (dirMesCallback[msgid].SyncFlag == true)
                        {
                            dirMesCallback[msgid].ReturnData = o;
                        }
                        else if (args.Call == null && OnMessage != null)
                        {
                            OnMessage(this, args);
                        }
                        else
                        {
                            args.Call?.Invoke(args.Scanda, args.Data);
                        }
                    }

                }
                catch (Exception)
                {

                }
                finally
                {
                    if (msgid != "")
                    {
                        dirMesCallback.Remove(msgid);
                    }
                }
            }
            catch
            { }
        }

        public void CallMESAPI(MESAPIData data, MESCallBack callback, CMC503Scanda Scanda)
        {
            if (MESWebSocket == null)
            {
                Connect();
            }
            data.MessageID = "MSG" + DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next(10000, 99999).ToString();
            data.Token = this.Token;
            dirMesCallback.Add(data.MessageID, new CatchObject() { Callback = callback, Scanda = Scanda });
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            MESWebSocket.Send(json);
        }

        public JObject CallMESAPISync(MESAPIData data, int Timeout)
        {
            if (MESWebSocket == null)
            {
                Connect();
            }
            data.MessageID = "MSG" + DateTime.Now.ToString("yyyyMMddHHmmss") + rand.Next(10000, 99999).ToString();
            data.Token = this.Token;
            CatchObject CatchObject = new CatchObject() { Callback = null, Scanda = null, SyncFlag = true };
            dirMesCallback.Add(data.MessageID, CatchObject);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            MESWebSocket.Send(json);

            int T = Timeout;
            while (T > 0)
            {
                Thread.Sleep(100);
                if (CatchObject.ReturnData != null)
                {
                    break;
                }
                T -= 100;
            }
            if (T <= 0)
            {
                throw new Exception("Call MESAPI Time Out!");
            }

            return CatchObject.ReturnData;

        }

        public void Login(string UserName, string PWD, MESCallBack callback, CMC503Scanda Scanda)
        {
            MES_USER = UserName;
            MES_PWD = PWD;
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.MESUserManager.UserManager";
            mesdata.Function = "Login";
            mesdata.Data = new { EMP_NO = MES_USER, Password = MES_PWD, Language = Language, BU_NAME = BU_NAME };
            CallMESAPI(mesdata, callback, Scanda);
            if (callback == null)
            {
                this.OnMessage += LoginCallBack;
            }
        }
        public void Login()
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.MESUserManager.UserManager";
            mesdata.Function = "Login";
            mesdata.Data = new { EMP_NO = MES_USER, Password = MES_PWD, Language = Language, BU_NAME = BU_NAME };
            //CallMESAPI(mesdata, null, null);
            var retData = CallMESAPISync(mesdata, 1000);

            if (retData["Status"].ToString() == "Pass")
            {
                Token = retData["Data"]["Token"].ToString();
            }
            else
            {
                throw new Exception(retData["Message"].ToString());
            }

        }

        public void LoginTest()
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.MESUserManager.UserManager";
            mesdata.Function = "LoginForSMO";
            mesdata.Data = new { EMP_NO = MES_USER, Password = MES_PWD, Language = Language, BU_NAME = BU_NAME };
            //CallMESAPI(mesdata, null, null);
            var retData = CallMESAPISync(mesdata, 1000);

            if (retData["Status"].ToString() == "Pass")
            {
                Token = retData["Data"]["Token"].ToString();
            }
            else
            {
                throw new Exception(retData["Message"].ToString());
            }

        }

        public JObject GetHostInfo(string HostName)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.CMC.CMCConfig";
            mesdata.Function = "GetCMCListByHostNAME";
            mesdata.Data = new { HOSTNAME = HostName };
            var retData = CallMESAPISync(mesdata, 1000);
            if (retData["Status"].ToString() == "Pass")
            {
                //Token = retData["Data"]["Token"].ToString();
                //retData = retData["Data"];
            }
            else
            {
                throw new Exception(retData["Message"].ToString());
            }
            return retData;
        }

        public void LoginCallBack(MESAPIClient sender, MesClientOnmessageEventArgs EventArg)
        {
            try
            {
                JObject JO = (JObject)EventArg.Data;
                if (JO["Status"].ToString() == "Pass")
                {
                    Token = JO["Data"]["Token"].ToString();
                }
                else
                {
                    throw new Exception(JO["Message"].ToString());
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                this.OnMessage -= LoginCallBack;
            }
        }

    }
    public class MesClientOnmessageEventArgs : EventArgs
    {
        public MESCallBack Call;
        public JObject Data;
        public CMC503Scanda Scanda;
    }

    public class CatchObject
    {
        public MESCallBack Callback;
        public CMC503Scanda Scanda;
        public bool SyncFlag = false;
        public JObject ReturnData;
    }
}
