using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;
using SqlSugar;


namespace MESStation.Config
{
    public class CUserControl : MesAPIBase
    {
        protected APIInfo FGetUserFunction = new APIInfo()
        {
            FunctionName = "GetUserFunction",
            Description = "GetUserFunction",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetUserLevel = new APIInfo()
        {
            FunctionName = "GetUserLevel",
            Description = "GetUserLevel",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        public CUserControl()
        {
            this.Apis.Add(FGetUserFunction.FunctionName, FGetUserFunction);
            this.Apis.Add(FGetUserLevel.FunctionName, FGetUserLevel);
        }

        public void GetUserFunction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string functionName = Data["FUNCTIONNAME"].ToString().Trim().ToUpper();
                var res = sfcdb.ORM.Queryable<C_USER_FUNCTION>().Where(t=>t.USERID==LoginUser.ID&&t.FUNCTIONNAME== functionName).ToList().FirstOrDefault();
                if (res!=null)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = res;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetUserLevel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            StationReturn.Message = "OK";
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Data = LoginUser.EMP_LEVEL;
        }
    }
}
