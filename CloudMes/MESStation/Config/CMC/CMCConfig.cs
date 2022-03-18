
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config.CMC
{
    public class CMCConfig : MesAPIBase
    {
        protected APIInfo FGetCMCListByHostNAME = new APIInfo()
        {
            FunctionName = "GetCMCListByHostNAME",
            Description = "GetCMCListByHostNAME",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "HOSTNAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CMCConfig()
        {
            Apis.Add(FGetCMCListByHostNAME.FunctionName, FGetCMCListByHostNAME);
        }

        public void GetCMCListByHostNAME(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            string HostName = Data["HOSTNAME"].ToString();
            var hosts = SFCDB.ORM.Queryable<R_CMCHOST>().Where(t => t.HOST_NAME == HostName).ToList();
            if (hosts.Count > 0)
            {
                string hostID = hosts[0].ID;
                var cmcs = SFCDB.ORM.Queryable<R_CMCHOST_DETAIL>().Where(t => t.HOST_ID == hostID).ToList();
                StationReturn.Data = new { HOST = hosts[0], CMCS = cmcs };
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = $@"Can not get '{HostName}'";
            }

        }

        public void UndoDlStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            string Station_No = Data["Station_No"].ToString();
            string SN = Data["SN"].ToString();
            //var hosts = SFCDB.ORM.Queryable<R_CMCHOST>().Where(t => t.HOST_NAME == Station_No).ToList();
            if (SN == "SMT1" || SN == "SMT2")
            {
                T_R_AP_TEMP tap = new T_R_AP_TEMP(SFCDB, 0);
                tap.UpdateApStation(SFCDB, Station_No, SN);
                StationReturn.Status = StationReturnStatusValue.Pass;

            }

        }
    }

    
}
