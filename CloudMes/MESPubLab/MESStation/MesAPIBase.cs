using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using System.Reflection;
using System.Data;
using WebSocketSharp.Server;
using System.Threading;

namespace MESPubLab.MESStation
{
  
    /// <summary>
    /// 作為所有API的基礎類
    /// </summary>
    public class MesAPIBase
    {
        static Random rand = new Random();
        public static Dictionary<string, CallUIFunctionReturnCatch> UIReturn = new Dictionary<string, CallUIFunctionReturnCatch>();

        protected static Dictionary<string, OleExecPool> _DBPools = new Dictionary<string, OleExecPool>();


        public MESStation.LogicObject.User LoginUser;
        public string BU;
        public string Language= "CHINESE";
        public String SystemName = "MES";
        public DB_TYPE_ENUM DBTYPE = DB_TYPE_ENUM.Oracle;
        protected bool _MastLogin = true;
        public string IP = "";
        public object ServerBase;
        public bool isdistory = false;

        public string ClientMsgID;
        public string ClientID;
        public string Token;

        public List<OleExec> UseDBS_UI =null;

        public object CallUIFunctionSync(object data, UIInput _FunctionType,int TimeOut,I_LockThread lockObj= null, UIInputData.DelegateDone callBack = null)
        {
            OleExec sfcdb = null , apdb = null;

            MESCallUIFunction Data = new MESCallUIFunction() { ClientID = "SERVER", CallTime = DateTime.Now , FunctionType = _FunctionType,
                MessageID = ClientMsgID, Data = data};
            Data.ServerMessageID = DateTime.Now.ToString("yyyyMMddHHmmss")+ rand.Next(10000, 99999).ToString();

            CallUIFunctionReturnCatch ret = new CallUIFunctionReturnCatch() { CallData = Data, ReturnData = null };
            UIReturn.Add(Data.ServerMessageID, ret);
            if (lockObj != null)
            {
                lockObj.ServerMessageID = Data.ServerMessageID;
            }

            Type T_ServiceBase = ServerBase.GetType();
            var M_SendDataToClient = T_ServiceBase.GetMethod("SendDataToClient");

            var P_UseDBS = T_ServiceBase.GetProperty("UseDbs");            
            UseDBS_UI = (List<OleExec>)P_UseDBS.GetValue(ServerBase);

            if (lockObj is MESStationBase && Data.FunctionType == UIInput.CanPrint)
            {
                MESStationBase Station = (MESStationBase)lockObj;
                Data.Labels = new Dictionary<string, object>();
                Data.Labels.Add("LabelPrint", Station.LabelPrint);
                Data.Labels.Add("LabelStillPrint", Station.LabelStillPrint);
                Data.Labels.Add("LabelPrints", Station.LabelPrints);
                
            }
            if (lockObj is MESStationBase)
            {
                MESStationBase Station = (MESStationBase)lockObj;
                UseDBS_UI.Add(Station.SFCDB);
                UseDBS_UI.Add(Station.APDB);
                sfcdb = Station.SFCDB;
                apdb = Station.APDB;
            }

            //MESStationReturn SR = new MESStationReturn(ClientMsgID, "SERVER");
            //SR.Data = data;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            M_SendDataToClient.Invoke(ServerBase, new object[] { json });

            int T = 0;
            while (T <= TimeOut)
            {
                if(isdistory)
                    throw new Exception("Threed Distory!");
                if (ret.IsTheadCancel)
                {
                    UIReturn.Remove(Data.MessageID);
                    throw new Exception("Tread Cancel");
                }
                switch (ret.StationLayerReturnType)
                {
                    case StationLayerReturnType.Cancel:
                        if (sfcdb != null)
                        {
                            UseDBS_UI.Remove(sfcdb);
                        }
                        if (apdb != null)
                        {
                            UseDBS_UI.Remove(apdb);
                        }
                        throw new Exception(ret.ReturnData.ToString());
                    case StationLayerReturnType.Reply:
                        UIReturn.Remove(Data.MessageID);
                        if (callBack != null && callBack(ret.ReturnData.ToString()))
                        {
                            if (sfcdb != null)
                            {
                                UseDBS_UI.Remove(sfcdb);
                            }
                            if (apdb != null)
                            {
                                UseDBS_UI.Remove(apdb);
                            }
                            return ret.ReturnData;
                        }
                        else if (callBack == null)
                        {
                            if (sfcdb != null)
                            {
                                UseDBS_UI.Remove(sfcdb);
                            }
                            if (apdb != null)
                            {
                                UseDBS_UI.Remove(apdb);
                            }
                            return ret.ReturnData;
                        }
                        else
                        {
                            ret.StationLayerReturnType = StationLayerReturnType.None;
                            json = Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                            M_SendDataToClient.Invoke(ServerBase, new object[] { json });
                        }
                        break;
                    case StationLayerReturnType.Close:
                        UIReturn.Remove(Data.MessageID);
                        if (sfcdb != null)
                        {
                            UseDBS_UI.Remove(sfcdb);
                        }
                        if (apdb != null)
                        {
                            UseDBS_UI.Remove(apdb);
                        }
                        return ret.ReturnData;
                    default:
                        break;
                }
                Thread.Sleep(100);
                T += 100;
            }
            if(((UIInputData)data).Type== UIInputType.Alart)
            {
                UIReturn.Remove(Data.MessageID);
                if (sfcdb != null)
                {
                    UseDBS_UI.Remove(sfcdb);
                }
                if (apdb != null)
                {
                    UseDBS_UI.Remove(apdb);
                }

                return ret.ReturnData;
            }

            if (sfcdb != null)
            {
                UseDBS_UI.Remove(sfcdb);
            }
            if (apdb != null)
            {
                UseDBS_UI.Remove(apdb);
            }
            UIReturn.Remove(Data.MessageID);
            throw new Exception("CallUIFunction TimeOut");

        }

        public void SendDataToClient(object data)
        {
            Type T_ServiceBase = ServerBase.GetType();
            var M_SendDataToClient = T_ServiceBase.GetMethod("SendDataToClient");
            MESStationReturn SR = new MESStationReturn(ClientMsgID,"SERVER");
            SR.Data = data;
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(SR, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            M_SendDataToClient.Invoke(ServerBase, new object[] { json });
        }

        public void CallUIFunction(object data)
        {

        }

        public bool MastLogin
        {
            get
            {
                return _MastLogin;
            }
        }

        public Dictionary<string, OleExecPool> DBPools
        {
            
            get
            {
                return _DBPools;
            }
        }




        Dictionary<string, APIInfo> _Apis= new Dictionary<string ,APIInfo>();
        public Dictionary<string, APIInfo> Apis
        {
            get
            {
                
                return _Apis;
            }
        }

        public DateTime GetDBDateTime()
        {
            OleExec sfcdb = _DBPools["SFCDB"].Borrow();
            try
            {
                string strSql = "select sysdate from dual";
                if (DBTYPE == DB_TYPE_ENUM.Oracle)
                {
                    strSql = "select sysdate from dual";
                }
                else if (DBTYPE == DB_TYPE_ENUM.SqlServer)
                {
                    strSql = "select get_date() ";
                }
                else
                {
                    throw new Exception(DBTYPE.ToString() + " not Work");
                }
                DateTime DBTime = (DateTime)sfcdb.ExecSelectOneValue(strSql);
                _DBPools["SFCDB"].Return(sfcdb);
                return DBTime;
            }
            catch (Exception e)
            {
                _DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public static System.Data.DataTable GetApiListTable()
        {
            Assembly assenbly = Assembly.Load("MESStation");
            Type tagType = typeof(MesAPIBase);
            Type[] t = assenbly.GetTypes();
            DataTable ret = new DataTable();
            ret.Columns.Add("No");
            ret.Columns.Add("廠區");
            ret.Columns.Add("API名稱");
            ret.Columns.Add("API位置");
            ret.Columns.Add("API內容");
            ret.Columns.Add("負責人員");
            ret.Columns.Add("主要用戶單位");
            ret.Columns.Add("主要用戶");
            ret.Columns.Add("使用者訪問頻率");
            ret.Columns.Add("備註");

            int count = 1;
            for (int i = 0; i < t.Length; i++)
            {
                TypeInfo ti = t[i].GetTypeInfo();
                Type baseType = ti.BaseType;
                if (baseType == tagType)
                {
                    object obj = assenbly.CreateInstance(ti.FullName);
                    MesAPIBase API = (MesAPIBase)obj;
                    string[] keys = API.Apis.Keys.ToArray();
                    foreach (var I in keys)
                    {
                        DataRow dr = ret.NewRow();
                        dr[0] = count++;
                        dr[1] = "NN";
                        dr[2] = ti.FullName + "." + API.Apis[I].FunctionName;
                        dr[3] = "10.120.115.142";
                        dr[4] = API.Apis[I].Description;
                        dr[5] = "黃和關";
                        dr[6] = "IT";
                        dr[7] = "Cloud MES";
                        dr[8] = "高";
                        ret.Rows.Add(dr);
                        // API.Apis[I]
                    }

                }
            }

            return ret;
        }
        

    }

   
}
