using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.VN
{
    public class SFCWorkOrderStatusConfig : MesAPIBase
    {
        protected APIInfo FGetWoSNStatusList = new APIInfo()
        {
            FunctionName = "GetWoSNStatusList",
            Description = "Get wosn status setting",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        public SFCWorkOrderStatusConfig()
        {
            this.Apis.Add(FGetWoSNStatusList.FunctionName, FGetWoSNStatusList);
        }

        public void GetWoSNStatusList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SN t_R_SN = null;
            OleExec sfcdb = null;
            var wo = Data["WO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                t_R_SN = new T_R_SN(sfcdb, DB_TYPE_ENUM.Oracle);

                if (string.IsNullOrEmpty(wo))
                {                   
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
                }
                
                 var snList  = sfcdb.ORM.Queryable<R_SN,R_WO_BASE>((rsn,rwo)=>rsn.WORKORDERNO == rwo.WORKORDERNO).Where((rsn,rwo) => rsn.WORKORDERNO == wo && rsn.VALID_FLAG=="1")
                    .OrderBy((rsn,rwo) => rsn.SN).OrderBy((rsn,rwo)=>rsn.EDIT_TIME)
                    .Select((rsn, rwo) => 
                    new { rsn.SN,rsn.CURRENT_STATION,rsn.NEXT_STATION,COMPLETED_FLAG = SqlSugar.SqlFunc.IIF(rsn.COMPLETED_FLAG=="1","YES","NO")
                    , REPAIR_FAILED_FLAG = SqlSugar.SqlFunc.IIF(rsn.REPAIR_FAILED_FLAG == "1", "YES", "NO"),rwo.WORKORDERNO,rwo.SKUNO,rwo.WORKORDER_QTY
                    ,rwo.FINISHED_QTY,rwo.SKU_VER,rwo.PLANT,rwo.PRODUCTION_TYPE,rwo.CLOSE_DATE, CLOSED_FLAG=SqlSugar.SqlFunc.IIF(rwo.CLOSED_FLAG=="1","YES","NO")
                    ,rwo.WO_TYPE
                    }).ToList();
                
                if (snList != null)
                {
                    StationReturn.MessageCode = "MSGCODE20210814161629！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = snList;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }
    }
}
