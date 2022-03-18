using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MESStation.Config.Vertiv
{
    public class MacRangeConfig : MesAPIBase
    {
        protected APIInfo FSelectMacRange = new APIInfo()
        {
            FunctionName = "SelectMacRange",
            Description = "SelectMacRange",
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetAllMacRuleName = new APIInfo()
        {
            FunctionName = "GetAllMacRuleName",
            Description = "GetAllMacRuleName",
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddMacRange = new APIInfo()
        {
            FunctionName = "AddMacRange",
            Description = "AddMacRange",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "RuleName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MinRange", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MaxRange", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Qty", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpdateMacRange = new APIInfo()
        {
            FunctionName = "UpdateMacRange",
            Description = "UpdateMacRange",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RuleName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MinRange", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MaxRange", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Qty", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteMacRange = new APIInfo()
        {
            FunctionName = "DeleteMacRange",
            Description = "DeleteMacRange",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "IDs", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSelectRangeDetail = new APIInfo()
        {
            FunctionName = "SelectRangeDetail",
            Description = "SelectRangeDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "RuleID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSelectMacRangeDetail = new APIInfo()
        {
            FunctionName = "SelectMacRangeDetail",
            Description = "SelectMacRangeDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "RuleID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Wo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Seq", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public MacRangeConfig()
        {
            this.Apis.Add(FSelectMacRange.FunctionName, FSelectMacRange);
            this.Apis.Add(FGetAllMacRuleName.FunctionName, FGetAllMacRuleName);
            this.Apis.Add(FAddMacRange.FunctionName, FAddMacRange);
            this.Apis.Add(FUpdateMacRange.FunctionName, FUpdateMacRange);
            this.Apis.Add(FDeleteMacRange.FunctionName, FDeleteMacRange);
            this.Apis.Add(FSelectRangeDetail.FunctionName, FSelectRangeDetail);
            this.Apis.Add(FSelectMacRangeDetail.FunctionName, FSelectMacRangeDetail);
        }
        public void SelectMacRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_RANGE_RULE T_Rule = new T_R_RANGE_RULE(SFCDB, DB_TYPE_ENUM.Oracle);
                DataTable dtRule = T_Rule.GetAllData(SFCDB);
                DataTable dtReturn = dtRule.Copy();
                dtReturn.Columns.Add("RULENAME");
                dtReturn.Columns.Add("PRINTED");
                for (int i = 0; i < dtRule.Rows.Count; i++)
                {
                    var ruleId = dtRule.Rows[i]["RULEID"].ToString();
                    var snRule = SFCDB.ORM.Queryable<C_SN_RULE>().Where(t => t.ID == ruleId).ToList().FirstOrDefault();
                    dtReturn.Rows[i]["RULENAME"] = snRule.NAME;

                    var id = dtRule.Rows[i]["ID"].ToString();
                    var detailList = SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.RULEID == id).ToList();
                    dtReturn.Rows[i]["PRINTED"] = detailList.Count.ToString();
                }
                StationReturn.MessageCode = "MSGCODE20210814161629！！";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = dtReturn;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetAllMacRuleName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var list = SFCDB.ORM.Queryable<C_SN_RULE>().Where(t => SqlSugar.SqlFunc.StartsWith(t.NAME, "MAC_")).OrderBy(t => t.ID, SqlSugar.OrderByType.Asc).ToList();
                if (list.Count == 0)
                {
                    StationReturn.MessageCode = "MSGCODE20210814165854！";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MSGCODE20210814161629！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void AddMacRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string ruleName = Data["RuleName"].ToString().Trim().ToUpper();
                string minRange = Data["MinRange"].ToString().Trim().ToUpper();
                string maxRange = Data["MaxRange"].ToString().Trim().ToUpper();
                string qty = Data["Qty"].ToString().Trim();
                T_R_RANGE_RULE T_Rule = new T_R_RANGE_RULE(SFCDB, DB_TYPE_ENUM.Oracle);
                if (T_Rule.CheckExisted(SFCDB, minRange, maxRange))
                {
                    StationReturn.MessageCode = "MSGCODE20210814165905!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                else
                {
                    Row_R_RANGE_RULE r = (Row_R_RANGE_RULE)T_Rule.NewRow();
                    r.ID = T_Rule.GetNewID(this.BU, SFCDB);
                    r.RULEID = SFCDB.ORM.Queryable<C_SN_RULE>().Where(t => t.NAME == ruleName).Select(t => t.ID).ToList().FirstOrDefault();
                    r.MIN = minRange;
                    r.MAX = maxRange;
                    r.QTY = Convert.ToDouble(qty);
                    r.VALID = "Y";
                    r.COMPLITED = "N";
                    r.CURVAL = (Convert.ToInt32(minRange, 16) - 1).ToString("x6").ToUpper();//此處當前值需-1,這樣下一個生成的值才是最小區間的值,x6表示填充6位,不夠6位補0
                    r.CREATEBY = this.LoginUser.EMP_NO;
                    r.CREATETIME = GetDBDateTime();
                    r.EDITBY = this.LoginUser.EMP_NO;
                    r.EDITTIME = GetDBDateTime();
                    string strRet = SFCDB.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (Convert.ToInt32(strRet) > 0)
                    {
                        StationReturn.MessageCode = "MES00000002！！";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Data = "";
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000036";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                    }
                }                
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                throw e;
            }
        }
        public void UpdateMacRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_RANGE_RULE T_Rule = new T_R_RANGE_RULE(SFCDB, DB_TYPE_ENUM.Oracle);
                string id = Data["ID"].ToString().Trim();
                string ruleName = Data["RuleName"].ToString().Trim().ToUpper();
                string minRange = Data["MinRange"].ToString().Trim().ToUpper();
                string maxRange = Data["MaxRange"].ToString().Trim().ToUpper();
                string qty = Data["Qty"].ToString().Trim().ToUpper();
                string ruleId = SFCDB.ORM.Queryable<C_SN_RULE>().Where(t => t.NAME == ruleName).Select(t => t.ID).ToList().FirstOrDefault();
                R_RANGE_RULE r = SFCDB.ORM.Queryable<R_RANGE_RULE>().Where(t => t.ID == id && t.RULEID == ruleId).ToList().FirstOrDefault();
                r.MIN = minRange;
                r.MAX = maxRange;
                r.CURVAL = (Convert.ToInt32(minRange, 16) - 1).ToString("x6").ToUpper();//此處當前值需-1,這樣下一個生成的值才是最小區間的值,x6表示填充6位,不夠6位補0
                r.EDITBY = this.LoginUser.EMP_NO;
                r.EDITTIME = GetDBDateTime();
                r.QTY = Convert.ToDouble(qty);
                int strRet = SFCDB.ORM.Updateable(r).Where(t => t.ID == r.ID).ExecuteCommand();
                if (strRet > 0)
                {
                    StationReturn.MessageCode = "MES00000003！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                throw e;
            }
        }
        public void DeleteMacRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string[] idArry = Data["IDs"].ToString().Trim().ToUpper().Split(',');
                for (int i = 0; i < idArry.Length; i++)
                {
                    string id = idArry[i];
                    bool printed = SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.RULEID == id).Any();
                    if (printed)
                    {
                        //throw new Exception("Rule ID：" + idArry[i] + "已有Mac打印記錄，不能刪除！");

                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814170527", new string[] { idArry[i] }));
                    }
                    int res = SFCDB.ORM.Deleteable<R_RANGE_RULE>().Where(t => t.ID == id).ExecuteCommand();
                    if (res > 0)
                    {
                        StationReturn.MessageCode = "MES00000004！！";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Data = "";
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000036";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                    }
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                throw e;
            }
        }
        public void SelectRangeDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string ruleId = Data["RuleID"].ToString();
                //T_R_RANGE_DETAIL T_Detail = new T_R_RANGE_DETAIL(SFCDB, DB_TYPE_ENUM.Oracle);
                //List<R_RANGE_DETAIL> detailList = T_Detail.GetAllDataByRuleId(SFCDB, ruleId);
                //更改顯示樣式：按工單顯示打印起始Mac和結束Mac，防止用戶判斷同工單下Mac由第幾批次打印
                string runSql = $@"
                select ruleid,
                       ext4,
                       ext1,
                       min(value) mac_start,
                       max(value) mac_end,
                       count(*) qty,
                       ext2,
                       ext3
                  from r_range_detail
                 where ruleid = '{ruleId}'
                 group by ruleid, ext4, ext1, ext2, ext3
                 order by mac_start";
                DataTable resDt = SFCDB.RunSelect(runSql).Tables[0];

                StationReturn.MessageCode = "MSGCODE20210814161629！！";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = resDt;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void SelectMacRangeDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string ruleId = Data["RuleID"].ToString();
                string wo = Data["Wo"].ToString();
                string seq = Data["Seq"].ToString();
                List<R_RANGE_DETAIL> detailList = SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.RULEID == ruleId && t.EXT1 == wo && t.EXT4 == seq).OrderBy(t => t.VALUE).ToList();

                StationReturn.MessageCode = "MSGCODE20210814161629！！";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = detailList;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
