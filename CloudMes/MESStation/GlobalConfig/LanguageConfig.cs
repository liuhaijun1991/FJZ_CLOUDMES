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
    public class LanguageConfig : MESPubLab.MESStation.MesAPIBase
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
        public LanguageConfig()
        {
            _MastLogin = false;
            this.Apis.Add(AllLANGUAGE.FunctionName, AllLANGUAGE);
        }
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
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = LanguageList;  
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
    }
}

