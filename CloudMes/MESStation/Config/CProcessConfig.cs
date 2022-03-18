using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESDBHelper;

namespace MESStation.Config
{
   public class CProcessConfig : MesAPIBase
    {
        protected APIInfo FGetAllProcess = new APIInfo()
        {
            FunctionName = "GetAllProcess",
            Description = "獲取FailProcess信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "" } },
            Permissions = new List<MESPermission>()
        };

        public CProcessConfig()
        {
            this.Apis.Add(FGetAllProcess.FunctionName,FGetAllProcess);
        }
        public void GetAllProcess(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                List<string> ProcessList = new List<string>();
 
                T_C_PROCESS TC_PROCESS = new T_C_PROCESS(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                ProcessList = TC_PROCESS.GetAllProcess(sfcdb);
                StationReturn.Data = ProcessList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
    }
}
