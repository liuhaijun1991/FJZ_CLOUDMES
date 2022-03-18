using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.JNP
{
    public class StockManage : MesAPIBase
    {
        #region APIInfo
        protected APIInfo _GetStockGroup = new APIInfo()
        {
            FunctionName = "GetStockGroup",
            Description = "_GetStockGroup",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        #endregion

        public StockManage()
        {
            this.Apis.Add(_GetStockGroup.FunctionName, _GetStockGroup);
        }

        public void GetStockGroup(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();


            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void TEST(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                

            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
