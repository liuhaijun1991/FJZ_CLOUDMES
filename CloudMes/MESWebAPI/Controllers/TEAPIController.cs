using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MESDataObject.Module.DCN;
using MESDataObject.Module;
using System.Configuration;
using MESPubLab.MesClient;
using MESDataObject.Module.Juniper;

namespace MESWebAPI.Controllers
{
    [RoutePrefix("MESAPI")]
    public class TEAPIController : ApiController
    {
        static List<string> getApiServers()
        {
            var ret = new List<string>();
            ret.Add(ConfigurationManager.AppSettings["API_URL"]);
            int api_url_count = ConfigurationManager.AppSettings["API_URL_COUNT"] == null ? 3 : Convert.ToInt32(ConfigurationManager.AppSettings["API_URL_COUNT"].ToString());
            for (int i = 1; i<= api_url_count; i++)
            {
                if (ConfigurationManager.AppSettings["API_URL" + i.ToString()] != null)
                {
                    ret.Add(ConfigurationManager.AppSettings["API_URL" + i.ToString()]);
                }                
            }
            return ret;
        }

        static MESAPIClient MESAPI = 
            new MESAPIClient(ConfigurationManager.AppSettings["API_URL"],
            ConfigurationManager.AppSettings["API_USER"], 
            ConfigurationManager.AppSettings["API_PWD"]);
        int TimeOut = 50000;

        public TEAPIController()
        {
            MESAPI.Login();
        }

        public void PP()
        { }

        ~TEAPIController()
        {
            //MESAPI.DisConnect();
        }


        [HttpPost]
        [Route("Update_Test_Hours_Juniper_Silver_Wip")]
        public object Update_Test_Hours_Juniper_Silver_Wip(R_JUNIPER_SILVER_WIP Select)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "Update_Test_Hours_Juniper_Silver_Wip";
            mesdata.Data = new { JsonData = Select };

            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);

            if (JO["Status"].ToString() == "Pass" || JO["Status"].ToString() == "PartialSuccess")
            {
                try
                {
                    StationObjDotNET station = new StationObjDotNET();
                    station.MESAPI = MESAPI;                   
                }
                catch (Exception ee)
                {

                }                

                if (JO["Status"].ToString() == "Pass")
                {
                    return new ServiceResModel<object>()
                    {
                        ResData = JO["Data"],
                        Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                        
                    };
                }
                else if (JO["Status"].ToString() == "PartialSuccess")
                {
                    return new ServiceResModel<object>()
                    {
                        ResData = JO["Data"],
                        Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.PartialSuccess

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


        [Route("GetJuniper_Silver_Wip")]
        public object GetJuniper_Silver_Wip(string SN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "GetJuniper_Silver_Wip";
            mesdata.Data = new { SN };            
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        [Route("GetJuniper_Error_Codes")]
        public object GetJuniper_Error_Codes()  
            {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "GetJuniper_Error_Codes";
            //mesdata.Data = new { SN };
            mesdata.Data = new { SN = "" };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        [Route("GetAllPartsLocations")]
        public object GetAllPartsLocations(string SKUNO)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "GetAllPartsLocations";
            mesdata.Data = new { SKUNO };
            //mesdata.Data = new { SKUNO = "" };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        [Route("GetNextStation")]
        public object GetNextStation(string SN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "GetNextStation";
            mesdata.Data = new { SN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        [Route("GetSNFromOldDB")]
        public object GetSNFromOldDB(string SN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Interface.DCN.DCNDataTrans";
            mesdata.Function = "GetDCNSNDataFromOlddb";
            mesdata.Data = new { SN = SN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        [Route("GetLastTestStatus")]
        public object GetLastTestStatus(string SN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "GetLastTestStatus";
            mesdata.Data = new { SN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        [HttpGet]
        [Route("IsSNInRepair")]
        public object IsSNInRepair(string SN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "GetRepairStatus";
            mesdata.Data = new { SN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        [Route("GetLockStatus")]
        public object GetLockStatus(string SN/*, string WO*/)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "GetLockStatus";
            mesdata.Data = new { SN, /*WO*/ };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        //[HttpPost]
        [Route("SNKP")]
        public object GetSNKP(string SN)
        {

            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.Ate";
            mesdata.Function = "GetSNKP";
            mesdata.Data = new { SN = SN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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
        [Route("QUERY")]
        public object GetSQLQUERY(string SQL)
        {
            //SQL = "SELECT * FROM DUAL";
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "SQLQUERY";
            mesdata.Data = new { SQL = SQL };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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
        [HttpPost]
        [Route("QUERY")]
        public object GetSQLQUERY( object Select,string TableName)
        {
            //SQL = "SELECT * FROM DUAL";
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "SQLQUERYByObj";
            mesdata.Data = new { Select, TableName };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
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

        [HttpPost]
        [Route("UPDATE_R_TEST_BRCD")]
        public object UPDATE_R_TEST_BRCD(R_TEST_BRCD[] Select)
        {
            //SQL = "SELECT * FROM DUAL";
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateR_TEST_BRCD";
            mesdata.Data = new { JsonData = Select };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
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
            } else
            {
                return new ServiceResModel<object>()
                {
                    ErrorMessage = JO["Message"].ToString(),
                    Status = ServiceResStatus.Exception
                };
            }

        }          


        [Route("WWN")]
        public ServiceResModel<WWN_DATASHARING[]> GetWWN(string WSN, string VSSN, string CSSN)
        {

            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "GetWWNDatasharing";
            mesdata.Data = new { WSN = WSN, VSSN = VSSN, CSSN = CSSN };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<WWN_DATASHARING[]>()
                {
                    ResData = Newtonsoft.Json.JsonConvert.DeserializeObject<WWN_DATASHARING[]>(JO["Data"].ToString()),
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
            }
            else
            {
                return new ServiceResModel<WWN_DATASHARING[]>()
                {
                    ErrorMessage = JO["Message"].ToString(),
                    Status = ServiceResStatus.Exception
                };
            }
        }

        [HttpPost]
        [Route("WWN")]
        public ServiceResModel<object> UpdateWWN(WWN_DATASHARING[] JsonData)
        {

            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateWWNDatasharing";
            mesdata.Data = new { JsonData = JsonData };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
            }
            else if (JO["Status"].ToString() == "PartialSuccess")
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
            else
            {
                return new ServiceResModel<object>()
                {
                    ErrorMessage = JO["Message"].ToString(),
                    Status = ServiceResStatus.Exception
                };
            }
        }


        [HttpPost]
        [Route("UpdateR_TEST_JUNIPER")]
        public object UpdateR_TEST_JUNIPER(R_TEST_JUNIPER[] Select)
        {
            lock (MESAPI)
            {
                MESAPIData mesdata = new MESAPIData();
                mesdata.Class = "MESStation.ATE.DCN.DCNTE";
                mesdata.Function = "UpdateR_TEST_JUNIPER";
                mesdata.Data = new { JsonData = Select };
                JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
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
                                var ret = station.TE_PASSSTATION(Select[i].SYSSERIALNO, Select[i].EVENTNAME);
                                if (ret.StartsWith("Err"))
                                {

                                }
                            }
                        }
                    }
                    catch (Exception ee)
                    {

                    }
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

        [HttpPost]
        [Route("LockUnitSN")]
        public object LockUnitSN(R_SN_LOCK Select)
        {
            lock (MESAPI)
            {
                MESAPIData mesdata = new MESAPIData();
                mesdata.Class = "MESStation.Config.HWD.TeApi";
                mesdata.Function = "LockUnitSN";
                mesdata.Data = new { JsonData = Select };

                JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
                if (JO["Status"].ToString() == "Pass" || JO["Status"].ToString() == "PartialSuccess")
                {
                    try
                    {
                        StationObjDotNET station = new StationObjDotNET();
                        station.MESAPI = MESAPI;
                    }
                    catch (Exception ee)
                    {

                    }

                    if (JO["Status"].ToString() == "Pass")
                    {
                        return new ServiceResModel<object>()
                        {
                            ResData = JO["Data"],
                            Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success

                        };
                    }
                    else if (JO["Status"].ToString() == "PartialSuccess")
                    {
                        return new ServiceResModel<object>()
                        {
                            ResData = JO["Data"],
                            Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.PartialSuccess

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

        [HttpPost]
        [Route("UpdateR_TEST_JUNIPER_New")]
        public object UpdateR_TEST_JUNIPER_New(JuniperTestRecord[] Select)
        {
            lock (MESAPI)
            {
                MESAPIData mesdata = new MESAPIData();
                mesdata.Class = "MESStation.ATE.DCN.DCNTE";
                mesdata.Function = "UpdateR_TEST_JUNIPER_New";
                mesdata.Data = new { JsonData = Select };
                JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
                if (JO["Status"].ToString() == "Pass" || JO["Status"].ToString() == "PartialSuccess")
                {
                    try
                    {
                        StationObjDotNET station = new StationObjDotNET();
                        station.MESAPI = MESAPI;
                        for (int i = 0; i < Select.Count(); i++)
                        {
                            if (Select[i].DESC_DATA.STATUS.ToUpper() == "PASS")
                            {
                                var ret = station.TE_PASSSTATION(Select[i].DESC_DATA.SYSSERIALNO, Select[i].DESC_DATA.EVENTNAME);
                                if (ret.StartsWith("Err"))
                                {

                                }
                            }
                            else
                            {
                                var ret = station.TE_FAILSTATION(Select[i].DESC_DATA.SYSSERIALNO, Select[i].DESC_DATA.EVENTNAME, Select[i].FailInfo);
                                if (ret.StartsWith("Err"))
                                {

                                }
                            }
                        }
                    }
                    catch (Exception ee)
                    {

                    }
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

        [HttpPost]
        [Route("UpdateR_TEST_JSNLIST")]
        public object UpdateR_TEST_JSNLIST(R_TEST_JSNLIST[] Select)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateR_TEST_JSNLIST";
            mesdata.Data = new { JsonData = Select };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass" || JO["Status"].ToString() == "PartialSuccess")
            {
                try
                {
                    StationObjDotNET station = new StationObjDotNET();
                    station.MESAPI = MESAPI;
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

        [HttpPost]
        [Route("UpdateRotationDetail")]
        public object UpdateRotationDetail(R_SILVER_ROTATION_DETAIL[] JsonData)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.ATE.DCN.DCNTE";
            mesdata.Function = "UpdateRotationDetail";
            mesdata.Data = new { JsonData = JsonData };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
            }
            else if (JO["Status"].ToString() == "PartialSuccess")
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
            else
            {
                return new ServiceResModel<object>()
                {
                    ErrorMessage = JO,
                    Status = ServiceResStatus.Exception
                };
            }
        }
        [HttpPost]
        [Route("LoadTest")]
        public object LoadTest(string msg)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Test.APITest";
            mesdata.Function = "CallTest";
            mesdata.Data = new { JsonData = msg };
            JObject JO = MESAPI.CallMESAPISync(mesdata, 999999999);
            if (JO["Status"].ToString() == "Pass")
            {

                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
            }
            else if (JO["Status"].ToString() == "PartialSuccess")
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
            else
            {
                return new ServiceResModel<object>()
                {
                    ErrorMessage = JO,
                    Status = ServiceResStatus.Exception
                };
            }
        }

        [HttpPost]
        [Route("SimplenessPassStation")]
        public object SimplenessPassStation(SimplenessPassStationData Data)
        {
            lock (MESAPI)
            {
                StationObjDotNET station = new StationObjDotNET();
                station.MESAPI = MESAPI;
                var ret = station.TE_PASSSTATION(Data.SN, Data.StationName);
                if (ret.StartsWith("Err"))
                {
                    return new ServiceResModel<object>()
                    {
                        ErrorMessage = ret,
                        ResData = ret,
                        Status = ServiceResStatus.Exception
                    };
                }
                else
                {
                    return new ServiceResModel<string>()
                    {
                       
                        ResData = ret,
                        Status = ServiceResStatus.Exception
                    };
                }

                
            }
        }

        [HttpGet]
        [Route("SimplenessPassStation")]
        public object SimplenessPassStation(string SN, string StationName,string Line)
        {
            lock (MESAPI)
            {
                StationObjDotNET station = new StationObjDotNET();
                station.MESAPI = MESAPI;
                var ret = station.PASSSTATION(SN, StationName, Line);
                if (ret.StartsWith("Err"))
                {
                    return new ServiceResModel<object>()
                    {
                        ErrorMessage = ret,
                        ResData = ret,
                        Status = ServiceResStatus.Exception
                    };
                }
                else
                {
                    return new ServiceResModel<string>()
                    {

                        ResData = ret,
                        Status = ServiceResStatus.Success
                    };
                }


            }
        }

    }

    


public class SimplenessPassStationData
    {
        public string StationName { get; set; }
        public string SN { get; set; }
        public string Line { get; set; }
    }

    public class JuniperTestRecord
    {
        public R_TEST_JUNIPER DESC_DATA;
        public List<TestFailInfo> FailInfo = new List<TestFailInfo>();
    }

    

    public class ServiceResModel<T>
    {
        /// <summary>
        /// Success = 0
        /// NoData = 1,
        /// Exception = 6,
        /// </summary>
        public ServiceResStatus Status;
        public T ResData;
        public object ErrorMessage;
    }
    public enum ServiceResStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 成功 无数据
        /// </summary>
        NoData = 1,
        /// <summary>
        /// 部分成功
        /// </summary>
        PartialSuccess = 2,
        /// <summary>
        /// 异常
        /// </summary>
        Exception = 6,
        /// <summary>
        /// 控制异常
        /// </summary>
        ControlException = 7
    }
}
