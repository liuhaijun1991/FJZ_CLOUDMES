
using MESDataObject.Module;
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
    public class AgileController : ApiController
    {
        static MESAPIClient MESAPI = new MESAPIClient(ConfigurationManager.AppSettings["API_URL"],
        ConfigurationManager.AppSettings["API_USER"],
        ConfigurationManager.AppSettings["API_PWD"]);
        int TimeOut = 50000;

        public AgileController()
        {
            MESAPI.Login();
        }

        /// <summary>
        /// 传送单条Agile数据
        /// </summary>
        /// <param name="singleItemObj">單個R_AGILE_ATTR對象</param>
        /// <param name="PLANT">廠別</param>
        /// <returns></returns>
        [HttpPost]
        [Route("JuniperAgileItemSynForSingle")]
        public ServiceResModel<object> JuniperAgileItemSynForSingle(O_AGILE_ATTR singleItemObj,string PLANT)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESJuniper.Api.AgileApi";
            mesdata.Function = "AddNewAgileItemForSingle";
            mesdata.Data = new { JsonData = singleItemObj , PLANT = PLANT };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<object>()
                {
                    Status =ServiceResStatus.Success
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
        /// 传送多条Agile数据
        /// </summary>
        /// <param name="multipleItemObjs">R_AGILE_ATTR的集合</param>
        /// <param name="PLANT">廠別</param>
        /// <returns></returns>
        [HttpPost]
        [Route("JuniperAgileItemSynForMultiple")]
        public ServiceResModel<object> JuniperAgileItemSynForMultiple(O_AGILE_ATTR[] multipleItemObjs, string PLANT)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESJuniper.Api.AgileApi";
            mesdata.Function = "AddNewAgileItemForMultiple";
            mesdata.Data = new { JsonData = multipleItemObjs , PLANT= PLANT };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<object>()
                {
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
        /// 根据ITEM_NUMBER,REV,PLANT获取Agile记录
        /// </summary>
        /// <param name="ITEM_NUMBER">料號</param>
        /// <param name="REV">版本</param>
        /// <param name="PLANT">廠別</param>
        /// <returns></returns>
        [Route("GetAgileItemFromOm")]
        public ServiceResModel<O_AGILE_ATTR[]> GetAgileItemFromOm(string ITEM_NUMBER,string REV, string PLANT)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESJuniper.Api.AgileApi";
            mesdata.Function = "GetAgileItemFromOm";
            mesdata.Data = new { ITEM_NUMBER = ITEM_NUMBER,REV=REV,PLANT = PLANT };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<O_AGILE_ATTR[]>()
                {
                    ResData = JO["Data"].ToObject<O_AGILE_ATTR[]>(),
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
            }
            else
            {
                return new ServiceResModel<O_AGILE_ATTR[]>()
                {
                    ErrorMessage = JO["Message"].ToString(),
                    Status = ServiceResStatus.Exception
                };
            }
        }

        /// <summary>
        /// 传送多条ECO数据
        /// </summary>
        /// <param name="multipleItemObjs">R_JNP_ECNPAGE的集合</param>
        /// <param name="PLANT">廠別</param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddEcnPageForMultiple")]
        public ServiceResModel<object> AddEcnPageForMultiple(R_JNP_ECNPAGE[] multipleItemObjs, string PLANT)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESJuniper.Api.AgileApi";
            mesdata.Function = "AddEcnPageForMultiple";
            mesdata.Data = new { JsonData = multipleItemObjs, PLANT = PLANT };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<object>()
                {
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
        /// 根据CHANGENUMBER,PLANT获取ECO记录
        /// </summary>
        /// <param name="CHANGENO">料號</param>
        /// <returns></returns>
        [Route("GetEcnPageFromOm")]
        public ServiceResModel<R_JNP_ECNPAGE[]> GetEcnPageFromOm(string CHANGENO)
        {
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESJuniper.Api.AgileApi";
            mesdata.Function = "GetEcnPageFromOm";
            mesdata.Data = new { CHANGENO = CHANGENO };
            JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                return new ServiceResModel<R_JNP_ECNPAGE[]>()
                {
                    ResData = JO["Data"].ToObject<R_JNP_ECNPAGE[]>(),
                    Status = JO["Data"] == null ? ServiceResStatus.NoData : ServiceResStatus.Success
                };
            }
            else
            {
                return new ServiceResModel<R_JNP_ECNPAGE[]>()
                {
                    ErrorMessage = JO["Message"].ToString(),
                    Status = ServiceResStatus.Exception
                };
            }
        }

    }
}