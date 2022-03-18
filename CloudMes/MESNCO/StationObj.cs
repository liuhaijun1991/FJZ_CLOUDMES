using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace MESNCO
{

    [Guid("4BC7759F-5DBA-4810-922F-55A393338EDC")]
    [ComVisible(true)]
    public interface IStationObj
    {
       
        string SetConnPara(string _MESConnStr, string _MES_USER, string _MES_PWD);
        string SetStation(string StationName, string Line);
        string StationInput(string InputName, string Value);
        string GetSNStationKPList(string SN);
        string ScanKP(string SN, string KP_Item);
        string ScanKPTest(string SN, string KP_Item);
        
        string TEST();
        string TEST1();
        string GetSNKP(string SN);
        string GetCurrStationObj();

        void CloseConn();
        string GetWWN_Datasharing(string WSN, string VSSN, string CSSN);
        string UpdateWWN_Datasharing(string JsonData);
        string UploadDCNTestData(string TestData);

        string SQLQUERY(string SQL);

        string HWDGetLast100DaySkuList();
        string GetSQLQUERY(string Select, string TableName);
        string GetSNFromOldDB(string SN);

        string TE_PASSSTATION(string SN, string TE_SESTATION);

        //與其DCN Beacon網站使用超鏈接打開MES系統報表經常出現莫名其妙的問題導致頁面無法打開,不如寫個方法把數據丟給他們自己玩 Add By ZHB 2020年10月26日13:56:29
        string GetFPY_Condition(string Type);
        string GetFPY_Data(string Type, string From, string To, string Skuno, string Station);
        string UpdateJuniperTest(string JsonData);
        string UpdateJuniperSnList(string JsonData);
        string UpdateRotationDetail(string JsonData);

        string CallTest(string JsonData);
    }

    [Guid("75699C57-F789-406F-A13D-76DB7001DD7F")]
    [ComVisible(true)]
    [ProgId("MESNCO.Application")]
    [ClassInterface( ClassInterfaceType.None)]
    public class StationObj: IObjectSafety, IStationObj
    {
        #region IObjectSafety 成员
        private const string _IID_IDispatch = "{00020400-0000-0000-C000-000000000046}";

        private const string _IID_IDispatchEx = "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}";

        private const string _IID_IPersistStorage = "{0000010A-0000-0000-C000-000000000046}";

        private const string _IID_IPersistStream = "{00000109-0000-0000-C000-000000000046}";

        private const string _IID_IPersistPropertyBag = "{37D84F60-42CB-11CE-8135-00AA004BB851}";



        private const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001;

        private const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002;

        private const int S_OK = 0;

        private const int E_FAIL = unchecked((int)0x80004005);

        private const int E_NOINTERFACE = unchecked((int)0x80004002);



        private bool _fSafeForScripting = true;

        private bool _fSafeForInitializing = true;
        public int GetInterfaceSafetyOptions(ref Guid riid, [MarshalAs(UnmanagedType.U4)] ref int pdwSupportedOptions, [MarshalAs(UnmanagedType.U4)] ref int pdwEnabledOptions)
        {
            int Rslt = E_FAIL;



            string strGUID = riid.ToString("B");

            pdwSupportedOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA;

            switch (strGUID)

            {

                case _IID_IDispatch:

                case _IID_IDispatchEx:

                    Rslt = S_OK;

                    pdwEnabledOptions = 0;

                    if (_fSafeForScripting == true)

                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER;

                    break;

                case _IID_IPersistStorage:

                case _IID_IPersistStream:

                case _IID_IPersistPropertyBag:

                    Rslt = S_OK;

                    pdwEnabledOptions = 0;

                    if (_fSafeForInitializing == true)

                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_DATA;

                    break;

                default:

                    Rslt = E_NOINTERFACE;

                    break;

            }



            return Rslt;
        }

        public int SetInterfaceSafetyOptions(ref Guid riid, [MarshalAs(UnmanagedType.U4)] int dwOptionSetMask, [MarshalAs(UnmanagedType.U4)] int dwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");

            switch (strGUID)

            {

                case _IID_IDispatch:

                case _IID_IDispatchEx:

                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_CALLER) && (_fSafeForScripting == true))

                        Rslt = S_OK;

                    break;

                case _IID_IPersistStorage:

                case _IID_IPersistStream:

                case _IID_IPersistPropertyBag:

                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_DATA) && (_fSafeForInitializing == true))

                        Rslt = S_OK;

                    break;

                default:

                    Rslt = E_NOINTERFACE;

                    break;

            }



            return Rslt;
        }
        #endregion

        
        public string StationName;
        public JToken JStation;
        public JToken CurrStationInput;
        /// <summary>
        /// 远程访问超时时间
        /// </summary>
        public int TimeOut = 90000;
        public  MESAPIClient MESAPI;


        public string GetCurrStationObj()
        {
            int a = 0;
            return JStation.ToString();
        }

        public StationObj()
        {
            WebSocket MESWebSocket = null;
        }

        public string TEST()
        {
            return "OK";
        }

        public string TEST1()
        {
            return "Test1";
        }

        public string ConnectToServer()
        {
            try
            {
                MESAPI.Connect();
                return "OK";
            }
            catch (Exception ee)
            {
                return ee.Message;
            }
        }
        /// <summary>
        /// 设置工站
        /// </summary>
        /// <param name="StationName"></param>
        /// <param name="Line"></param>
        public string SetStation(string _StationName, string Line)
        {
            JStation = null;
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Stations.CallStation";
            mesdata.Function = "InitStation";
            mesdata.Data = new { DisplayStationName = _StationName, Line = Line };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                JStation = JO["Data"]["Station"];
                StationName = _StationName;
            }
            else
            {
                //throw new Exception(JO["Message"].ToString());
                return JO["Message"].ToString();
            }
            return "OK";
        }

        /// <summary>
        /// 工站输入
        /// </summary>
        /// <param name="InputName"></param>
        /// <param name="Value"></param>
        public string StationInput(string InputName, string Value)
        {
            string strRET = "";
            try
            {
                for (int i = 0; i < JStation["Inputs"].Count(); i++)
                {
                    if (JStation["Inputs"][i]["DisplayName"].ToString() == InputName)
                    {
                        CurrStationInput = JStation["Inputs"][i];
                        CurrStationInput["Value"] = Value;
                        MESAPIData mesdata = new MESAPIData();
                        mesdata.Class = "MESStation.Stations.CallStation";
                        mesdata.Function = "StationInput";
                        mesdata.Data = new { Station = JStation, Input = CurrStationInput, ScanType = "Pass" };
                        JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);

                        if (JO["Status"].ToString() == "Pass")
                        {

                            JStation = JO["Data"]["Station"];

                            for (int j = 0; j < JStation["StationMessages"].Count(); j++)
                            {
                                strRET += JStation["StationMessages"][j]["Message"].ToString() + "\r\n";
                            }

                            if (JO["Message"].ToString().EndsWith("Input not successfull."))
                            {
                                throw new Exception(strRET);
                            }


                            for (int j = 0; j < JStation["StationMessages"].Count(); j++)
                            {
                                strRET += JStation["StationMessages"][i]["Message"].ToString() + "\r\n";
                            }

                            if (JO["Data"]["Station"]["ScanKP"].Count() > 0)
                            {
                                throw new Exception("Err: Please Scan KP!");
                            }

                        }
                        else
                        {
                            throw new Exception(JO["Message"].ToString());
                        }

                        return "OK/r/n"+strRET;
                    }
                }
            }
            catch (Exception ee)
            {
                return ee.Message;
            }

            return $@"Err:Input '{InputName}' not find ";
        }

        /// <summary>
        /// 获取SN当前KP List
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public string GetSNStationKPList(string SN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.KeyPart.KPScan";
            mesdata.Function = "GetSNStationKPList";
            mesdata.Data = new { SN = SN, STATION = StationName };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n"+JO["Message"].ToString();
                //throw new Exception(JO["Message"].ToString());
            }
        }

        /// <summary>
        /// 扫描工站KP
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="KP_Item"></param>
        public string ScanKP(string SN,string KP_Items)
        {
            var J_KP_Item = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(KP_Items);
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.KeyPart.KPScan";
            mesdata.Function = "ScanKPItem";
            mesdata.Data = new { SN = SN, STATION = StationName, KPITEM = J_KP_Item, ISTEST = "FALSE" };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                //return JO["Data"].ToString();
                return "OK";
            }
            else
            {
                return JO["Message"].ToString();
            }
        }

        /// <summary>
        /// 测试扫描KP
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="KP_Item"></param>
        public string ScanKPTest(string SN, string KP_Items)
        {
            var J_KP_Item = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(KP_Items);
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.KeyPart.KPScan";
            mesdata.Function = "ScanKPItem";
            mesdata.Data = new { SN = SN, STATION = StationName, KPITEM = J_KP_Item, ISTEST="TRUE"};
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                //return JO["Data"].ToString();
                return "OK";
            }
            else
            {
                return JO["Message"].ToString();
            }
        }

        /// <summary>
        /// 初始化设置连接参数
        /// </summary>
        /// <param name="_MESConnStr"></param>
        /// <param name="_MES_USER"></param>
        /// <param name="_MES_PWD"></param>
        public string SetConnPara(string _MESConnStr, string _MES_USER, string _MES_PWD)
        {
            try
            {
                if (MESAPI != null)
                {
                    MESAPI.DisConnect();
                }
                MESAPI = new MESAPIClient(_MESConnStr, _MES_USER, _MES_PWD);

                MESAPI.Login();
            }
            catch(Exception ee)
            {
                return ee.Message;
            }
            return "OK";
        }

        public void CloseConn()
        {
            if (MESAPI != null)
            {
                MESAPI.DisConnect();
            }
            MESAPI = null;
        }

        public string UploadDCNTestData(string TestData)
        {
            
            var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject(TestData);
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateR_TEST_BRCD";
            mesdata.Data = new { JsonData = jobj };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
                //throw new Exception(JO["Message"].ToString());
            }
        }

        public string GetSNKP(string SN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.Ate";
            mesdata.Function = "GetSNKP";
            mesdata.Data = new { SN = SN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
                //throw new Exception(JO["Message"].ToString());
            }
        }

        public string GetWWN_Datasharing(string WSN, string VSSN, string CSSN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "GetWWNDatasharing";
            mesdata.Data = new { WSN = WSN, VSSN= VSSN, CSSN = CSSN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
                //throw new Exception(JO["Message"].ToString());
            }
        }

        public string UpdateWWN_Datasharing(string JsonData)
        {
            var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData);
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateWWNDatasharing";
            mesdata.Data = new { JsonData = jobj };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
                //throw new Exception(JO["Message"].ToString());
            }
        }

        public string SQLQUERY(string SQL)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "SQLQUERY";
            mesdata.Data = new { SQL= SQL };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
            }
        }

        public string HWDGetLast100DaySkuList()
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.SkuConfig";
            mesdata.Function = "HWDGetLast100DaySkuList";
            mesdata.Data = new { Sku = "" };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();               
            }
        }

        public string  GetSQLQUERY(string strSelect, string TableName)
        {
            //SQL = "SELECT * FROM DUAL";
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "SQLQUERYByObj";
            var Select = Newtonsoft.Json.JsonConvert.DeserializeObject(strSelect);
            mesdata.Data = new { Select, TableName };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return  JO["Data"].ToString();
               
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
            }
        }

        public string GetSNFromOldDB(string SN)
        {

            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Interface.DCN.DCNDataTrans";
            mesdata.Function = "GetDCNSNDataFromOlddb";
            mesdata.Data = new { SN = SN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            return JO.ToString();
            
        }

        public string TE_PASSSTATION(string SN, string TE_SESTATION)
        {
            try
            {
                MESAPIData mesdata = new MESAPIData();
                mesdata.Class = "MESStation.ATE.DCN.DCNTE";
                mesdata.Function = "GetTEStationMapping";
                mesdata.Data = new { TESTATION = TE_SESTATION };
                JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
                string SFC_Station = JO["Data"].ToString(); //JO.ToString();
                if (SFC_Station != "")
                {
                    SetStation(SFC_Station, "Line1");
                    return StationInput("SN", SN);
                }
                else
                {
                    return "Err:Can't get TEStationMapping";
                }
            }
            catch (Exception ee)
            {
                return "Err:" + ee.Message;
            }

        }

        public string GetFPY_Condition(string Type)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.DCN.ForBeaconShow";
            mesdata.Function = "GetFpyCondition";
            mesdata.Data = new { ConditionType = Type };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
            }
        }

        public string GetFPY_Data(string Type, string From, string To, string Skuno, string Station)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.DCN.ForBeaconShow";
            mesdata.Function = "GetFpyData";
            mesdata.Data = new { DateType = Type, DateFrom = From, DateTo = To, SkuNo = Skuno, Station = Station };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
            }
        }

        public string UpdateJuniperTest(string JsonData)
        {
            var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData);
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateR_TEST_JUNIPER";
            mesdata.Data = new { JsonData = jobj };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return "ERROR:\r\n" + JO["Message"].ToString();
                //throw new Exception(JO["Message"].ToString());
            }
        }

        public string UpdateJuniperSnList(string JsonData)
        {
            var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData);
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateR_TEST_JSNLIST";
            mesdata.Data = new { JsonData = jobj };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                string rmsg = JO["Message"].ToString() == "" ? JO["Data"][0].ToString() : JO["Message"].ToString();
                //return "ERROR:\r\n" + JO["Message"].ToString();
                return "ERROR:\r\n" + rmsg;
                //throw new Exception(JO["Message"].ToString());
            }
        }
        public string UpdateRotationDetail(string JsonData)
        {
            var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData);
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateRotationDetail";
            mesdata.Data = new { JsonData = jobj };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return JO.ToString();                
            }
        }

        public string CallTest(string JsonData)
        {

            //MESAPIData mesdata = new MESAPIData();
            //mesdata.Class = "MESStation.Test.APITest";
            //mesdata.Function = "CallTest";
            //mesdata.Data = new { JsonData = msg };
            var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject(JsonData);
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Test.APITest";
            mesdata.Function = "CallTest";
            mesdata.Data = new { JsonData = jobj };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return JO["Data"].ToString();
            }
            else
            {
                return JO.ToString();
            }
        }
    }
}
