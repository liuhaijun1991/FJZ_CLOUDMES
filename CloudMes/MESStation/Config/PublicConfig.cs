using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class PublicConfig : MesAPIBase
    {
        protected APIInfo FGetCurrentUser = new APIInfo()
        {
            FunctionName = "GetCurrentUser",
            Description = "Get Current User Info",
            Parameters = new List<APIInputInfo>(){},
            Permissions = new List<MESPermission>() { }
        };
        public PublicConfig()
        {
            this.Apis.Add(FGetCurrentUser.FunctionName, FGetCurrentUser);
        }

        public void GetCurrentUser(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000033";
            StationReturn.MessagePara.Add(1);
            StationReturn.Data = this.LoginUser;
        }

    }
}
