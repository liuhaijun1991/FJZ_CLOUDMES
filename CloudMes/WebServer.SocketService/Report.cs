using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using MESStation;
using MESJuniper;
using MESPubLab;
using System.Xml.Linq;
using System.Reflection;
using MESDBHelper;
using MESDataObject;
using MESPubLab.MESStation;
using MESPubLab.MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Public;
using System.Threading;
using MESPubLab.Common;

namespace WebServer.SocketService
{
    public class ServerSession
    {
        public User user;
        public Report serverObj;
    }

    public class CallHis
    {
        public string ClientIP;
        public string ClientPort;
        
        public DateTime StartTime;
        public DateTime EndTime;
        public Newtonsoft.Json.Linq.JObject Request;
        public string Response;
        public string APIName;
        public string APIClass;
        public override string ToString()
        {
            return APIName;
        }

    }

    public class Report : ServiceBase
    {
        public static object ServerForm = null;
        public delegate void CallFinishEventHander(Report sender, CallHis his);
        public event CallFinishEventHander CallFinishEvent;
        public event CallFinishEventHander CallStartEvent;
        public static MESDBHelper.OleExecPool SFCDBPool = null;// new OleExecPool("SFCDB", true);
        public static MESDBHelper.OleExecPool APDBPool = null;// new OleExecPool("APDB", true);
        public static Dictionary<string, ServerSession> LoginUsers = new Dictionary<string, ServerSession>();
        //public static Dictionary<string, Report> LoginUsers = new Dictionary<string, User>();
        public string Token = null;
        public List<CallHis> HIS = new List<CallHis>();
        public int RecordHisCount = 2;
        public static List<Report> Reports = new List<Report>();

        public List<MESStationBase> CurrRunningStation = new List<MESStationBase>();

        public List<MesAPIBase> apis = new List<MesAPIBase>();

        public List<OleExec> _UseDbs = new List<OleExec>();

        static bool setThreadPool = false;
        public List<OleExec> UseDbs
        {
            get { return _UseDbs; }
            set { _UseDbs = value; }
        }

        public Report()
        {
            if (SFCDBPool == null)
            {
                SFCDBPool = new OleExecPool("SFCDB", true);
            }

            if (APDBPool == null)
            {
                APDBPool = new OleExecPool("APDB", true);
            }

            CallFinishEvent += Report_CallFinishEvent;
            CallStartEvent += Report_CallStartEvent;
            OnSocketClose += Report_OnSocketClose;
            Reports.Add(this);
            if (!setThreadPool)
            {
                ThreadPool.SetMinThreads(500, 500);
                ThreadPool.SetMaxThreads(500, 500);
                setThreadPool = true;
            }
            
        }

        private void Report_OnSocketClose(object sender, CloseEventArgs e)
        {
            var s = (Report)sender;
            //ReportDistory();
            Report.Reports.Remove(s);
        }

        private void Report_CallStartEvent(Report sender, CallHis his)
        {
            if (ServerForm != null)
            {
                try
                {
                    var T = ServerForm.GetType();
                    var M = T.GetMethod("OnCall");
                    M.Invoke(ServerForm, new object[] { sender, his });
                }
                catch
                { }
            }
        }

        private void Report_CallFinishEvent(Report sender, CallHis his)
        {
            while (HIS.Count >= RecordHisCount && HIS.Count > 0)
            {
                HIS.RemoveAt(0);
            }
            HIS.Add(his);
        }

        void CallFunction(object args)
        {
            MethodInfo _MethodInfo = (MethodInfo)((object[])args)[0];
            object inst = ((object[])args)[1];
            object[] para = (object[])((object[])args)[2];
            List<Exception> ex = (List<Exception>)((object[])args)[3];
            try
            {
                _MethodInfo.Invoke(inst, para);
                //var capi = (MesAPIBase)inst;
                //var r = (Report)capi.ServerBase;
                //r.apis.Remove((MesAPIBase)inst);
            }
            catch (Exception ee)
            {
                if (ee.InnerException != null)
                    ex.Add(ee.InnerException);
                else
                    ex.Add(ee);
            }
        }

        void CallFunctionTimeOut(MethodInfo _MethodInfo, object inst, object[] para, int timeout)
        {
            Thread T = new Thread(new ParameterizedThreadStart(CallFunction));
            List<Exception> ex = new List<Exception>();
            T.Start(new object[] { _MethodInfo, inst, para, ex });
            T.Join(timeout);
            if (T.ThreadState != ThreadState.Stopped)
            {
                throw new Exception("RunMethodTimeOut");
            }
            else
            {
                if (ex.Count > 0)
                {
                    throw ex[0];
                }
            }
        }

        //delegate void DoOnMessageDelegate(MessageEventArgs e);
        private void DoOnMessage(object arg)
        {
            MessageEventArgs e = (MessageEventArgs)arg;
            //this.Sessions.CloseSession(this.ID);
            MESStationReturn StationReturn = null;// new MESStationReturn();
            string[] Para = null; //add by LLF 2017-1-4
            Newtonsoft.Json.Linq.JObject Request = null;
            string CallApi = "";
            string CallApiClass = "";
            DateTime StartCallTime = DateTime.Now;
            CallHis his = null;
            try
            {
                //處理JSON
                //Newtonsoft.Json.Linq.JObject Request = (Newtonsoft.Json.Linq.JObject) Newtonsoft.Json.JsonConvert.DeserializeObject(
                //"{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiClassList\", DATA:{ } }");

                //Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject("{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiFunctionsList\", DATA:{ CLASSNAME:\"MESStation.ApiHelper\" } }");
                //Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(
                //    "{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiFunctionsList\", DATA:{ CLASSNAME:\"MESStation.UserManager\" } }");

                Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(e.Data, 
                    new Newtonsoft.Json.JsonSerializerSettings { DateParseHandling = Newtonsoft.Json.DateParseHandling.None });
                string CLASS = Request["Class"].ToString();
                string FUNCTION = Request["Function"].ToString();
                string TOKEN = Request["Token"].ToString();
                string MsgID = Request["MessageID"]?.ToString();
                string ClientID = Request["ClientID"]?.ToString();
                Request.Add("IP", Newtonsoft.Json.Linq.JToken.Parse("{Value:\"" + this.ClientIP + "\"}"));
                CallApi = FUNCTION;

                his = new CallHis() { APIClass = CallApiClass, APIName = CallApi, Request = Request, StartTime = StartCallTime , ClientIP = this.ClientIP, ClientPort = this.ClientPort };
                CallStartEvent(this, his);
                StationReturn = new MESStationReturn(MsgID, ClientID);
                //反射加載
                var nsps = CLASS.Split(new char[] { '.' });
                string NameSpace = "";
                if (nsps.Length > 0)
                {
                    NameSpace = nsps[0];
                }

                //ApiHelper api = new ApiHelper();
                Type APIType;
                //加載類庫
                Assembly assembly = Assembly.Load(NameSpace);
                CallApiClass = CLASS;
                APIType = assembly.GetType(CLASS);
                object API_CLASS = assembly.CreateInstance(CLASS);
                MesAPIBase API = (MesAPIBase)API_CLASS;
                API.ServerBase = this;
                API.ClientMsgID = MsgID;
                API.ClientID = ClientID;
                API.Token = TOKEN;
                if (!API.DBPools.ContainsKey("SFCDB"))
                {
                    API.DBPools.Add("SFCDB", SFCDBPool);
                }
                if (!API.DBPools.ContainsKey("APDB"))
                {
                    API.DBPools.Add("APDB", APDBPool);
                }
                //apis.Add(API);
                //API.BU = "HWD";
                //API.BU = "VERTIV";
                ((MesAPIBase)API_CLASS).IP = this.ClientIP;

                API.Language = "CHINESE";  //CHINESE,CHINESE_TW,ENGLISH;

                //初始化異常類型的數據庫連接池
                MESReturnMessage.SetSFCDBPool(SFCDBPool);
                //獲取調用函數
                MethodInfo Function = APIType.GetMethod(FUNCTION);

                int timeout = 3000000;
                try
                {
                    timeout =((MesAPIBase)API_CLASS).Apis[FUNCTION].TimeOut;
                }
                catch
                { }
                //
                bool CheckLogin = false;
                if (LoginUsers.ContainsKey(TOKEN))
                {
                    User lu = LoginUsers[TOKEN].user;
                    ((MesAPIBase)API_CLASS).LoginUser = lu;
                    CheckLogin = true;
                    API.BU = lu.BU;
                }
                else
                {
                    if (FUNCTION.IndexOf("Login") < 0 && ((MesAPIBase)API_CLASS).MastLogin)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = "No Login !";

                    }
                    else
                    {
                        if (FUNCTION.IndexOf("Login") >= 0)
                        {
                            CheckLogin = true;
                        }
                    }
                }
                if (CheckLogin)
                {
                    CallFunctionTimeOut(Function, API_CLASS, new object[] { Request, Request["Data"], StationReturn }, timeout);

                    //Function.Invoke(API_CLASS, new object[] { Request, Request["Data"], StationReturn });
                    if (FUNCTION.IndexOf("Login") >= 0)
                    {
                        if (StationReturn.Status == "Pass")
                        {
                            LoginReturn r = (LoginReturn)StationReturn.Data;
                            User lu = ((MesAPIBase)API_CLASS).LoginUser;
                            if (this.Token != null)
                            {
                                Report.LoginUsers.Remove(Token);
                                MESStation.Stations.CallStation.logout(Token);
                            }
                            string NewToken = r.Token;
                            Token = r.Token;
                            if (LoginUsers.ContainsKey(NewToken))
                            {
                                LoginUsers[NewToken] = new ServerSession() { user = lu, serverObj = this };
                            }
                            else
                            {
                                LoginUsers.Add(NewToken, new ServerSession() { user = lu, serverObj = this });
                            }
                        }
                    }
                }//函數不要求登錄
                else if (!((MesAPIBase)API_CLASS).MastLogin)
                {
                    CallFunctionTimeOut(Function, API_CLASS, new object[] { Request, Request["Data"], StationReturn }, timeout);
                    //Function.Invoke(API_CLASS, new object[] { Request, Request["Data"], StationReturn });
                    if (FUNCTION.IndexOf("Login") >= 0)
                    {
                        if (StationReturn.Status == "Pass")
                        {
                            LoginReturn r = (LoginReturn)StationReturn.Data;
                            User lu = ((MesAPIBase)API_CLASS).LoginUser;
                            string NewToken = r.Token;
                            if (LoginUsers.ContainsKey(NewToken))
                            {
                                LoginUsers[NewToken] = new ServerSession() { user = lu, serverObj = this };
                            }
                            else
                            {
                                LoginUsers.Add(NewToken, new ServerSession() { user = lu, serverObj = this });
                            }
                        }
                    }
                }

                //add by LLF 2017-12-27
                if (StationReturn.MessageCode != null)
                {
                    if (StationReturn.MessageCode.Length > 0)
                    {
                        if (StationReturn.MessagePara != null)
                        {
                            if (StationReturn.MessagePara.Count > 0)
                            {
                                Para = new string[StationReturn.MessagePara.Count];
                                for (int i = 0; i < StationReturn.MessagePara.Count; i++)
                                {
                                    Para[i] = StationReturn.MessagePara[i].ToString();
                                }
                            }
                        }
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage(StationReturn.MessageCode, Para);
                    }
                }
            }
            catch (MESReturnMessage ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                if (ee.InnerException != null)
                {
                    StationReturn.Data = ee.InnerException.Message;
                }
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                if (ee.InnerException != null)
                {
                    StationReturn.Data = ee.InnerException.Message;
                }
            }



            //System.Web.Script.Serialization.JavaScriptSerializer JsonMaker = new System.Web.Script.Serialization.JavaScriptSerializer();
            //JsonMaker.MaxJsonLength = int.MaxValue;
            //Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //};

            string json = "";// Newtonsoft.Json.JsonConvert.SerializeObject(StationReturn, setting);
            //((MESPubLab.MESStation.MESReturnView.Station.CallStationReturn)(StationReturn.Data)).Station.LabelStillPrint.Clear();
            //string json = JsonMaker.Serialize(StationReturn);
            //JavaScriptSerializer 實例在序列化對象的時候，遇到 DateTime 類型會序列化出不可讀的數據，
            //因此改用 Newtonsoft 的 JsonConvert 來進行序列化，序列化出來的 DateTime 形如 2017-12-06T11:14:37
            //另外如果遇到無法將 System.DBNull 類型轉換成 string 類型的，可以手動檢測下值的類型，
            //如果是 System.DBNull，直接將值改為 null 即可。
            //實在無法實現你所需要的功能，可將下面這句註釋掉。
            //
            // modify by 張官軍 2017/12/06
            try
            {
                //變更時間格式  modify by Wuq 2018/01/25
                json = Newtonsoft.Json.JsonConvert.SerializeObject(StationReturn, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"});
                //json = Newtonsoft.Json.JsonConvert.SerializeObject(StationReturn);
                Send(json);
                his.Response = json;
                his.EndTime = DateTime.Now;
                CallFinishEvent(this, his);
            }
            catch (Exception mese)
            {

                //MesLog.Error($@"Err:{mese.Message};Data:{e.Data}");
            }
        }

        public void Close()
        {
            this.Sessions.CloseSession(ID);
        }

        void ReportDistory()
        {
            foreach (var item in apis)            
                item.isdistory = true;            
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            //Thread T = new Thread(new ParameterizedThreadStart(DoOnMessage));

            //T.Start(e);
           

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoOnMessage), e);
            return;
            //this.Sessions.CloseSession(this.ID);
            MESStationReturn StationReturn = null;// new MESStationReturn();
            string[] Para = null; //add by LLF 2017-1-4
            Newtonsoft.Json.Linq.JObject Request = null;
            string CallApi = "";
            string CallApiClass="";
            DateTime StartCallTime = DateTime.Now;
            try
            {
                //處理JSON
                //Newtonsoft.Json.Linq.JObject Request = (Newtonsoft.Json.Linq.JObject) Newtonsoft.Json.JsonConvert.DeserializeObject(
                //"{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiClassList\", DATA:{ } }");

                //Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject("{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiFunctionsList\", DATA:{ CLASSNAME:\"MESStation.ApiHelper\" } }");
                //Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(
                //    "{ TOKEN:null, CLASS: \"MESStation.ApiHelper\", FUNCTION:\"GetApiFunctionsList\", DATA:{ CLASSNAME:\"MESStation.UserManager\" } }");

                Request = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(e.Data);
                string CLASS = Request["Class"].ToString();
                string FUNCTION = Request["Function"].ToString();
                string TOKEN = Request["Token"].ToString();
                string MsgID = Request["MessageID"]?.ToString();
                string ClientID = Request["ClientID"]?.ToString();
                Request.Add("IP", Newtonsoft.Json.Linq.JToken.Parse("{Value:\""+this.ClientIP +"\"}"));
                CallApi = FUNCTION;

                StationReturn = new MESStationReturn(MsgID, ClientID);
                //反射加載
                var nsps = CLASS.Split(new char[] { '.' });
                string NameSpace = "";
                if (nsps.Length > 0)
                {
                    NameSpace = nsps[0];
                }

                //ApiHelper api = new ApiHelper();
                Type APIType;
                //加載類庫
                Assembly assembly = Assembly.Load(NameSpace);
                CallApiClass = CLASS;
                APIType = assembly.GetType(CLASS);
                object API_CLASS = assembly.CreateInstance(CLASS);
                MesAPIBase API = (MesAPIBase)API_CLASS;
                API.ServerBase = this;
                API.ClientMsgID = MsgID;
                API.ClientID = ClientID;
                API.Token = TOKEN;
                if (!API.DBPools.ContainsKey("SFCDB"))
                {
                    API.DBPools.Add("SFCDB", SFCDBPool);
                }
                if (!API.DBPools.ContainsKey("APDB"))
                {
                    API.DBPools.Add("APDB", APDBPool);
                }
                //API.BU = "HWD";
                //API.BU = "VERTIV";
                ((MesAPIBase)API_CLASS).IP = this.ClientIP;

                API.Language = "CHINESE";  //CHINESE,CHINESE_TW,ENGLISH;

                //初始化異常類型的數據庫連接池
                MESReturnMessage.SetSFCDBPool(SFCDBPool);
                //獲取調用函數
                MethodInfo Function = APIType.GetMethod(FUNCTION);
                //
                bool CheckLogin = false;
                if (LoginUsers.ContainsKey(TOKEN))
                {
                    User lu = LoginUsers[TOKEN].user;
                    ((MesAPIBase)API_CLASS).LoginUser = lu;
                    CheckLogin = true;
                    API.BU = lu.BU;
                }
                else
                {
                    if (FUNCTION.IndexOf("Login") < 0 && ((MesAPIBase)API_CLASS).MastLogin)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Message = "No Login !";

                    }
                    else
                    {
                        if (FUNCTION.IndexOf("Login") >= 0)
                        {
                            CheckLogin = true;
                        }
                    }
                }
                if (CheckLogin)
                {

                    Function.Invoke(API_CLASS, new object[] { Request, Request["Data"], StationReturn });
                    if (FUNCTION.IndexOf("Login") >= 0)
                    {
                        if (StationReturn.Status == "Pass")
                        {
                            LoginReturn r = (LoginReturn)StationReturn.Data;
                            User lu = ((MesAPIBase)API_CLASS).LoginUser;
                            if (this.Token != null)
                            {
                                Report.LoginUsers.Remove(Token);
                                MESStation.Stations.CallStation.logout(Token);
                            }
                            string NewToken = r.Token;
                            Token = r.Token;
                            if (LoginUsers.ContainsKey(NewToken))
                            {
                                LoginUsers[NewToken] = new ServerSession() { user = lu, serverObj = this };
                            }
                            else
                            {
                                LoginUsers.Add(NewToken, new ServerSession() { user = lu, serverObj = this });
                            }
                        }

                    }
                }//函數不要求登錄
                else if (!((MesAPIBase)API_CLASS).MastLogin)
                {
                    Function.Invoke(API_CLASS, new object[] { Request, Request["Data"], StationReturn });
                    if (FUNCTION.IndexOf("Login") >= 0)
                    {
                        if (StationReturn.Status == "Pass")
                        {
                            LoginReturn r = (LoginReturn)StationReturn.Data;
                            User lu = ((MesAPIBase)API_CLASS).LoginUser;
                            string NewToken = r.Token;
                            if (LoginUsers.ContainsKey(NewToken))
                            {
                                LoginUsers[NewToken] = new ServerSession() { user = lu, serverObj = this };
                            }
                            else
                            {
                                LoginUsers.Add(NewToken, new ServerSession() { user = lu, serverObj = this });
                            }
                        }
                    }
                }

                //add by LLF 2017-12-27
                if (StationReturn.MessageCode!=null)
                {
                    if (StationReturn.MessageCode.Length > 0)
                    {
                        if (StationReturn.MessagePara != null)
                        {
                            if (StationReturn.MessagePara.Count > 0)
                            {
                                Para = new string[StationReturn.MessagePara.Count];
                                for (int i = 0; i < StationReturn.MessagePara.Count; i++)
                                {
                                    Para[i] = StationReturn.MessagePara[i].ToString();
                                }
                            }
                        }
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage(StationReturn.MessageCode, Para);
                    }
                }
            }
            catch (MESReturnMessage ee)
            {
                StationReturn.Status =StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                if (ee.InnerException != null)
                {
                    StationReturn.Data = ee.InnerException.Message;
                }
            }
            catch (Exception ee)
            {
                StationReturn.Status =StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                if (ee.InnerException != null)
                {
                    StationReturn.Data = ee.InnerException.Message;
                }
            }


            
            System.Web.Script.Serialization.JavaScriptSerializer JsonMaker = new System.Web.Script.Serialization.JavaScriptSerializer();
            JsonMaker.MaxJsonLength = int.MaxValue;
            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
            
           // string json = Newtonsoft.Json.JsonConvert.SerializeObject(StationReturn, setting);
            //((MESPubLab.MESStation.MESReturnView.Station.CallStationReturn)(StationReturn.Data)).Station.LabelStillPrint.Clear();
            //string json = JsonMaker.Serialize(StationReturn);
            //JavaScriptSerializer 實例在序列化對象的時候，遇到 DateTime 類型會序列化出不可讀的數據，
            //因此改用 Newtonsoft 的 JsonConvert 來進行序列化，序列化出來的 DateTime 形如 2017-12-06T11:14:37
            //另外如果遇到無法將 System.DBNull 類型轉換成 string 類型的，可以手動檢測下值的類型，
            //如果是 System.DBNull，直接將值改為 null 即可。
            //實在無法實現你所需要的功能，可將下面這句註釋掉。
            //
            // modify by 張官軍 2017/12/06

            //變更時間格式  modify by Wuq 2018/01/25
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(StationReturn, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            //json = Newtonsoft.Json.JsonConvert.SerializeObject(StationReturn);

            Send(json);
            var his = new CallHis() { APIClass = CallApiClass, APIName = CallApi, EndTime = DateTime.Now, Request = Request, Response = json , StartTime = StartTime };
            CallFinishEvent(this,his);
            

        }

    }
}
