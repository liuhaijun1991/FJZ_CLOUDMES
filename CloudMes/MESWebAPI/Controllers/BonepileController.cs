
using MESPubLab.MesClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MESWebAPI.Controllers
{
    [RoutePrefix("MESAPI")]
    public class BonepileController : ApiController
    {
        MESAPIClient MESAPI = new MESAPIClient(ConfigurationManager.AppSettings["API_URL"],
           ConfigurationManager.AppSettings["API_USER"],
           ConfigurationManager.AppSettings["API_PWD"]);
        int TimeOut = 50000;

        public BonepileController()
        {
            MESAPI.Login();
        }

        [Route("VertivCollectBonepileWeekData")]
        public object VertivCollectBonepileWeekData()
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.BonepileConfig";
            mesdata.Function = "GetBonepileSummaryReportData";
            mesdata.Data = new { Type = "WK", Date = "" };
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

        [Route("VertivCollectBonepileMothData")]
        public object VertivCollectBonepileMothData()
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.BonepileConfig";
            mesdata.Function = "GetBonepileSummaryReportData";
            mesdata.Data = new { Type = "MO", Date = "" };
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

    }
}
