using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MESPubLab.MesClient;
using System.Configuration;
using MESWebAPI.Interceptor;

namespace MESWebAPI.Controllers
{
    public class VertivController : ApiController
    {
        MESAPIClient MESAPI =
            new MESAPIClient(ConfigurationManager.AppSettings["API_URL"],
            ConfigurationManager.AppSettings["API_USER"],
            ConfigurationManager.AppSettings["API_PWD"]);
        int TimeOut = 50000;

        public VertivController()
        {
            MESAPI.Login();
        }

        /// <summary>
        /// 單板SmtLoading 
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="TRSN"></param>
        /// <param name="SN"></param>
        /// <param name="LINE"></param>
        /// <param name="UNO"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SmtLoading")]
        public  ServiceResModel<object> SmtLoading(string WO, string TRSN, string SN, string LINE, string UNO)
        {
            lock (MESAPI)
            {
                try
                {
                    StationObjDotNET station = new StationObjDotNET();
                    station.MESAPI = MESAPI;
                    var stationinputs = new List<EventInputData>();
                    stationinputs.Add(new EventInputData() { name = "WO", value = WO });
                    stationinputs.Add(new EventInputData() { name = "TR_SN", value = TRSN });
                    stationinputs.Add(new EventInputData() { name = "SN", value = SN });
                    var stationEventRes = station.DoStationEvent("PCBA_SMTLOADING", LINE, stationinputs, false);
                    if (stationEventRes.status)
                        return new ServiceResModel<object>() { Status = ServiceResStatus.Success };
                    else
                        return new ServiceResModel<object>() { Status = ServiceResStatus.ControlException, ErrorMessage = stationEventRes.msgcode };
                }
                catch (Exception ee)
                {
                    return new ServiceResModel<object>() { Status = ServiceResStatus.Exception, ErrorMessage = ee.Message };
                }
                finally
                {
                    MESAPI.DisConnect();
                }
            }
        }

        /// <summary>
        /// 多連板Smtloading
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="TRSN"></param>
        /// <param name="LINKQTY"></param>
        /// <param name="SN"></param>
        /// <param name="LINE"></param>
        /// <param name="UNO"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SmtLoadingWithPanel")]
        public ServiceResModel<object> SmtLoadingWithPanel(string WO, string TRSN, string LINKQTY, string SN, string LINE, string UNO)
        {
            lock (MESAPI)
            {
                try
                {
                    StationObjDotNET station = new StationObjDotNET();
                    station.MESAPI = MESAPI;
                    var stationinputs = new List<EventInputData>();
                    stationinputs.Add(new EventInputData() { name = "WO", value = WO });
                    stationinputs.Add(new EventInputData() { name = "TR_SN", value = TRSN });
                    stationinputs.Add(new EventInputData() { name = "LinkQty", value = LINKQTY });
                    stationinputs.Add(new EventInputData() { name = "PanelSN", value = SN });
                    var stationEventRes = station.DoStationEvent("SFC_SMT_LOADING", LINE, stationinputs, false);
                    if (stationEventRes.status)
                        return new ServiceResModel<object>() { Status = ServiceResStatus.Success };
                    else
                        return new ServiceResModel<object>() { Status = ServiceResStatus.ControlException, ErrorMessage = stationEventRes.msgcode };
                }
                catch (Exception ee)
                {
                    return new ServiceResModel<object>() { Status = ServiceResStatus.Exception, ErrorMessage = ee.Message };
                }
                finally
                {
                    MESAPI.DisConnect();
                }
            }
        }
    }
}