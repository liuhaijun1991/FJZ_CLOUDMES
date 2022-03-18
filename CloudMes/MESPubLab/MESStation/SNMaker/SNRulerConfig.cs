using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;

namespace MESPubLab.MESStation.SNMaker
{
    public class SNRulerConfig : MesAPIBase
    {
        private APIInfo _GetSNRulers = new APIInfo()
        {
            FunctionName = "GetSNRulers",
            Description = "獲取可用的SN生成規則",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetSNRulerDetail = new APIInfo()
        {
            FunctionName = "GetSNRulerDetail",
            Description = "獲取SN生成規則的詳細資料",
            Parameters = new List<APIInputInfo>()
            { new APIInputInfo() { InputName="Name", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };
        public SNRulerConfig()
        {
            Apis.Add(_GetSNRulers.FunctionName, _GetSNRulers);
            Apis.Add(_GetSNRulerDetail.FunctionName, _GetSNRulerDetail);
        }

        public void GetSNRulers(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_C_SN_RULE TCSR = new T_C_SN_RULE(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TCSR.GetAllData(db);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetSNRulerDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                string rulerName = Data["Name"].ToString();
                T_C_SN_RULE TCSR = new T_C_SN_RULE(db, DB_TYPE_ENUM.Oracle);
                C_SN_RULE c = TCSR.GetDataByName(rulerName, db);
                T_C_SN_RULE_DETAIL TCSRD = new T_C_SN_RULE_DETAIL(db, DB_TYPE_ENUM.Oracle);
                List<C_SN_RULE_DETAIL> rs = TCSRD.GetDataByRuleID(c.ID, db);
                List<C_SN_RULE_DETAIL> ret = new List<C_SN_RULE_DETAIL>();
                for (int i = 0; i < rs.Count; i++)
                {
                    ret.Add(rs[i]);
                }
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
    }
}
