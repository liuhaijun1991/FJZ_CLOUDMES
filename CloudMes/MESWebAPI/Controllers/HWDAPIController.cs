//using MESNCO;
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
    public class HWDAPIController : ApiController
    {
        MESAPIClient MESAPI = new MESAPIClient(ConfigurationManager.AppSettings["API_URL"],
            ConfigurationManager.AppSettings["API_USER"],
            ConfigurationManager.AppSettings["API_PWD"]);
        int TimeOut = 50000;

        public HWDAPIController()
        {
            MESAPI.Login();
        }

        [Route("GetSkuList")]
        public object GetSkuList()
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.SkuConfig";
            mesdata.Function = "HWDGetLast100DaySkuList";
            mesdata.Data = new { Sku = "" };
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("GetWoInfoBySn01A")]
        public object GetWoInfoBySn01A(string SN)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.TeApi";
            mesdata.Function = "GetWoInfoBySn01A";
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
        [Route("Get7B5PlanData")]
        public object Get7B5PlanData()
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.HWD.R7b5PlanData";
            mesdata.Function = "Get7B5PlanData";
            mesdata.Data = new { Sku = "" };
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
