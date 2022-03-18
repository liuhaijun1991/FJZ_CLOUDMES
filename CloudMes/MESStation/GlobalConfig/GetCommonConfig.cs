using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.GlobalConfig
{
   public  class GetCommonConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo AllLANGUAGE = new APIInfo()
        {
            FunctionName = "GetLanguageType",
            Description = "獲取語言種類",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        protected APIInfo FGetBU = new APIInfo()
        {
            FunctionName = "GetBU",
            Description = "Get BU List",
            Parameters = new List<APIInputInfo>(){},
            Permissions = new List<MESPermission>() { }
        };
        public GetCommonConfig()
        {
            _MastLogin = false;
            this.Apis.Add(AllLANGUAGE.FunctionName, AllLANGUAGE);
            this.Apis.Add(FGetBU.FunctionName, FGetBU);
        }
        /// <summary>
        /// 獲取語言種類
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetLanguageType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            Dictionary<String, String> LanguageList = new Dictionary<String, String>();
            T_C_LANGUAGE language = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                language = new T_C_LANGUAGE(sfcdb, DBTYPE);
                LanguageList = language.GetLanguageType(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode ="MES00000001";
                StationReturn.Data = LanguageList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        /// <summary>
        /// Get BU list
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetBU(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;            
            List<string> buList = new List<string>();
            T_C_BU bu = null;
            try
            {
            oleDB = this.DBPools["SFCDB"].Borrow();
                bu = new T_C_BU(oleDB, DBTYPE);
                buList = bu.GetAllBU(oleDB);
                if (buList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(buList.Count);
                    StationReturn.Data = buList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch(Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                throw exception;
            }
            
        }
    }
}
