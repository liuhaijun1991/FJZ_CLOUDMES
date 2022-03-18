using MESDataObject.Module.Juniper;
using MESPubLab.MesClient;
using MESWebAPI.Controllers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MESInterface.JUNIPER
{
    public class ICTDataUpload : taskBase
    {
        public bool IsRuning = false;
        public string path, url, user, pwd;
        MESAPIClient MESAPI;
        public static Dictionary<string, string> ICTFailCode01 = new Dictionary<string, string>() {
            {"01","Failed" },
            {"02","Failed_Pin_Test" },
            {"03","Failed_In_Learn" },
            {"04","Failed_In_Shorts" },
            {"06","Failed_In_Analog" },
            {"07","Failed_In_Power_Supplies" },
            {"08","Failed_In_Digital" },
            {"09","Failed_In_Functional" },
            {"10","Failed_In_Preshorts" },
            {"14","Failed_In_VectorlessTest" },
            {"15","Failed_In_Polarity_Check" },
            {"16","Failed_In_ConnectCheck" },
            {"17","Failed_In_Analog_Cluster" },
            {"18","Failed_In_Flash" },
            {"19","Failed_In_BScan_SiNails" },
            {"20","Failed_In_SW_Programming" },
            {"21","Failed_In_SW_Flash" },
            {"22","Failed_In_Verify_Grounds" },
            {"23","Failed_In_Cover_Extend" },
            {"24","Failed_In_Pre_Powered" },
            {"25","Failed_In_FXT_Power_Supplies" },
            {"26","Failed_In_CET_FXT_Power_Supplies" },
        };

        public static int IntervalSince1970(DateTime? t = null)
        {
            if (t == null)
            {
                return (int)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            }
            else
            {
                DateTime tt = (DateTime)t;
                return (int)((tt.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
            }
        }
        public override void init()
        {
            try
            {
                path = ConfigGet("PATH");
                url = ConfigGet("URL");
                user = ConfigGet("USER");
                pwd = ConfigGet("PWD");
                Output.UI = new ICTDataUploadUI(this);
                MESAPI = new MESAPIClient(url, user, pwd);
            }
            catch (Exception)
            {
            }
        }
        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("The task is in progress. Please try again later...");
            }
            IsRuning = true;
            try
            {
                try
                {
                    MESAPI.DisConnect();
                }
                catch
                { }
                MESAPI.Connect();
                if (!Directory.Exists(path))
                {
                    throw new Exception($@"No dir:{path}");
                }
                var files = Directory.GetFiles(path);
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        var Ext = files[i].Substring(files[i].Length - 4);
                        if (Ext.ToUpper() == ".LOG")
                        {
                            FileStream FS = null;
                            StreamReader FR = null;
                            try
                            {
                                R_TEST_JUNIPER data = new R_TEST_JUNIPER();
                                FS = new FileStream(files[i], FileMode.Open);
                                FR = new StreamReader(FS);
                                string L1 = FR.ReadLine();
                                string L2 = FR.ReadLine();
                                var S1 = L1.Split(new char[] { '|' }, StringSplitOptions.None);
                                var S2 = L2.Split(new char[] { '|' }, StringSplitOptions.None);
                                data.SERIAL_NUMBER = S2[1];
                                data.PART_NUMBER = S1[1];
                                data.TESTERNO = S1[9];
                                data.TEST_STATION_NAME = S1[6];
                                string startTime = S2[10];
                                string temp = "20" + startTime.Substring(0, 2) + "-" + startTime.Substring(2, 2) + "-" + startTime.Substring(4, 2) + " " +
                                    startTime.Substring(6, 2) + ":" + startTime.Substring(8, 2) + ":" + startTime.Substring(10, 2);
                                data.TATIME = DateTime.Parse(temp);
                                startTime = S2[3];
                                temp = "20" + startTime.Substring(0, 2) + "-" + startTime.Substring(2, 2) + "-" + startTime.Substring(4, 2) + " " +
                                    startTime.Substring(6, 2) + ":" + startTime.Substring(8, 2) + ":" + startTime.Substring(10, 2);
                                data.TESTDATE = DateTime.Parse(temp);
                                data.STATUS = S2[2] == "00" ? "PASS" : "FAIL";
                                data.EVENTNAME = data.TEST_STATION_NAME;
                                data.SYSSERIALNO = data.SERIAL_NUMBER;
                                data.TEST_START_TIMESTAMP = IntervalSince1970(data.TATIME).ToString();
                                if (ICTFailCode01.ContainsKey(S2[2]))
                                {
                                    data.FAILCODE = ICTFailCode01[S2[2]];
                                }

                                var ret = UpdateR_TEST_JUNIPER(new R_TEST_JUNIPER[] { data });
                                FS.Close();
                                File.Delete(files[i]);
                            }
                            catch (Exception ee)
                            {
                                throw ee;
                            }
                            finally
                            {
                                try
                                {
                                    FS.Close();
                                }
                                catch
                                { }
                            }
                        } 
                        else if (Ext.ToUpper() == ".TXT")
                        {
                            string strReg = "^(\\S+)_(\\S+)_(\\S+)_([P,F]{1}).TXT$";
                            var m = Regex.Match(files[i], strReg);
                            if (m.Success == false)
                            { continue; }
                            var STATION = m.Groups[1].Value;
                            var P_NO = m.Groups[2].Value;
                            var SN = m.Groups[3].Value;
                            var rst = m.Groups[4].Value;
                            R_TEST_JUNIPER data = new R_TEST_JUNIPER();

                            data.SERIAL_NUMBER = SN;
                            data.PART_NUMBER = P_NO;
                            data.TESTERNO = "DEFAULT";
                            data.TEST_STATION_NAME = STATION;
                            data.TATIME = DateTime.Now;
                            data.TESTDATE = DateTime.Now; ;
                            data.STATUS = rst == "P" ? "PASS" : "FAIL";
                            data.EVENTNAME = data.TEST_STATION_NAME;
                            data.SYSSERIALNO = data.SERIAL_NUMBER;
                            var ret = UpdateR_TEST_JUNIPER(new R_TEST_JUNIPER[] { data });
                            File.Delete(files[i]);

                        }
                    }
                }

            }
            catch(Exception ee)
            {
                throw ee;
            }
            finally
            {
                IsRuning = false;
            }
            
        }

        public ServiceResModel<object> UpdateR_TEST_JUNIPER(R_TEST_JUNIPER[] Select)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateR_TEST_JUNIPER";
            mesdata.Data = new { JsonData = Select };
            JObject JO = MESAPI.CallMESAPISync(mesdata, 10000);
            if (JO["Status"].ToString() == "Pass" || JO["Status"].ToString() == "PartialSuccess")
            {
                try
                {
                    StationObjDotNET station = new StationObjDotNET();
                    station.MESAPI = MESAPI;
                    for (int i = 0; i < Select.Count(); i++)
                    {
                        if (Select[i].STATUS.ToUpper() == "PASS")
                        {
                            station.TE_PASSSTATION(Select[i].SYSSERIALNO, Select[i].EVENTNAME);
                        }
                    }
                }
                catch (Exception ee)
                { }
                if (JO["Status"].ToString() == "Pass" || JO["Status"].ToString() == "PartialSuccess")
                {
                    return new ServiceResModel<object>()
                    {
                        ResData = JO["Data"],
                        Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                    };
                }
                else
                {
                    var resdata = JO["Data"].Value<List<string>>();
                    var resdata1 = JO["Data"].Value<List<string>>();
                    for (int i = 0; i < resdata.Count(); i++)
                    {
                        if (resdata[i].StartsWith("OK:ID"))
                        {
                            resdata[i] = null;
                        }
                        else
                        {
                            resdata1[i] = null;
                        }
                    }
                    return new ServiceResModel<object>()
                    {
                        ResData = resdata1,
                        Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.PartialSuccess,
                        ErrorMessage = resdata
                    };
                }
            }
            else
            {
                return new ServiceResModel<object>()
                {
                    ErrorMessage = JO["Message"].ToString(),
                    Status = ServiceResStatus.Exception
                };
            }
        }



    }
}
