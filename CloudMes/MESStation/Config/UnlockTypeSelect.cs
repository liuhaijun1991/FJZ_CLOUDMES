using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class UnlockTypeSelect:MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetFQCUnlockType = new APIInfo()
        {
            FunctionName = "GetFQCUnlockType",
            Description = "獲取FQC解鎖類型",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "TypeName" } },
            Permissions = new List<MESPermission>()
        };

        #endregion 方法信息集合 end

        public UnlockTypeSelect()
        {
            this.Apis.Add(FGetFQCUnlockType.FunctionName, FGetFQCUnlockType);
        }

        /// <summary>
        /// SMTFQC_UNLOCK工站獲取解鎖類型
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetFQCUnlockType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            List<string> unlockTypeList = new List<string>();
            unlockTypeList.Add("LotNo");
            unlockTypeList.Add("SN");
            StationReturn.Data = unlockTypeList;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
        }
    }
}
