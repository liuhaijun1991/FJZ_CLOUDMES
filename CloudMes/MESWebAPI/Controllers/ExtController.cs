
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
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
    public class ExtController : ApiController
    {
        static MESAPIClient MESAPI = new MESAPIClient(ConfigurationManager.AppSettings["API_URL"],
        ConfigurationManager.AppSettings["API_USER"],
        ConfigurationManager.AppSettings["API_PWD"]);
        int TimeOut = 50000;

        public ExtController()
        {
            MESAPI.Login();
        }

        /// <summary>
        /// ICAR DATA
        /// </summary>
        /// <returns></returns>
        [Route("ICAR_DATA")]
        public ServiceResModel<object> GetICarDataByFloor(string floor)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.DCN.FnnDcnExtApi";
            mesdata.Function = "GetIcarDataWithFnnDcn";
            mesdata.Data = new { Floor = floor };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = ServiceResStatus.Success
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
        /// ICAR DATA
        /// </summary>
        /// <returns></returns>
        [Route("ICAR_DATA_FAIL")]
        public ServiceResModel<object> GetICarFailDataByFloor(string floor)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.DCN.FnnDcnExtApi";
            mesdata.Function = "GetIcarFailDataWithFnnDcn";
            mesdata.Data = new { Floor = floor };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = ServiceResStatus.Success
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
        /// Get750and711WipData
        /// </summary>
        /// <returns></returns>
        [Route("Pn750and711WipData")]
        public ServiceResModel<object> Get750and711WipData(string PlantCode)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESJuniper.Api.JnpReportApi";
            mesdata.Function = "Get750and711WipData";
            mesdata.Data = new { PlantCode = PlantCode };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<object>()
                {
                    ResData = JO["Data"],
                    Status = ServiceResStatus.Success
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
