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
    public class StationObjDotNET 
    {



        public string StationName;
        public JToken JStation;
        public JToken CurrStationInput;
        /// <summary>
        /// 远程访问超时时间
        /// </summary>
        public int TimeOut = 20000;
        public MESAPIClient MESAPI;

       


        public string GetCurrStationObj()
        {
            int a = 0;
            return JStation.ToString();
        }

        public StationObjDotNET()
        {
            WebSocket MESWebSocket = null;
        }

        public string TEST()
        {
            return "OK";
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

                        return "OK/r/n" + strRET;
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
                return "ERROR:\r\n" + JO["Message"].ToString();
                //throw new Exception(JO["Message"].ToString());
            }
        }

        /// <summary>
        /// 扫描工站KP
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="KP_Item"></param>
        public string ScanKP(string SN, string KP_Items)
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
            mesdata.Data = new { SN = SN, STATION = StationName, KPITEM = J_KP_Item, ISTEST = "TRUE" };
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
            catch (Exception ee)
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
            mesdata.Data = new { WSN = WSN, VSSN = VSSN, CSSN = CSSN };
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
            mesdata.Data = new { SQL = SQL };
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

        public string GetSQLQUERY(string strSelect, string TableName)
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

                return JO["Data"].ToString();

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
                string SFC_Station = JO["Data"].ToString();
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
    }
}
