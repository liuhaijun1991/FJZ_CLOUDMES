using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.Vertiv
{
    public class SkuRangeConfig : MesAPIBase
    {
        protected APIInfo FSelectRange = new APIInfo()
        {
            FunctionName = "SelectRange",
            Description = "SelectRange",
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetAllMacRange = new APIInfo()
        {
            FunctionName = "GetAllMacRange",
            Description = "GetAllMacRange",
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddRange = new APIInfo()
        {
            FunctionName = "AddRange",
            Description = "AddRange",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "RuleID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CType", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Value", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpdateRange = new APIInfo()
        {
            FunctionName = "UpdateRange",
            Description = "UpdateRange",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RuleId", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CType", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Value", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteRange = new APIInfo()
        {
            FunctionName = "DeleteRange",
            Description = "DeleteRange",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "IDs", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public SkuRangeConfig()
        {
            this.Apis.Add(FSelectRange.FunctionName, FSelectRange);
            this.Apis.Add(FGetAllMacRange.FunctionName, FGetAllMacRange);
            this.Apis.Add(FAddRange.FunctionName, FAddRange);
            this.Apis.Add(FUpdateRange.FunctionName, FUpdateRange);
            this.Apis.Add(FDeleteRange.FunctionName, FDeleteRange);
        }
        public void SelectRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var rangeList = SFCDB.ORM.Queryable<R_RANGE, R_RANGE_RULE>((a, b) => a.RULEID == b.ID)
                    .Where((a, b) => a.VALID == "Y" && b.VALID == "Y").OrderBy((a, b) => a.ID, SqlSugar.OrderByType.Asc)
                    .Select((a, b) => new { a.ID, a.RULEID, b.MIN, b.MAX, a.CTYPE, a.VUL, a.VALID, a.CREATEBY, a.CREATETIME, a.EDITBY, a.EDITTIME })
                    .ToList();
                StationReturn.MessageCode = "MSGCODE20210814161629！！";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = rangeList;
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
        public void GetAllMacRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var list = SFCDB.ORM.Queryable<R_RANGE_RULE>().OrderBy(t => t.ID, SqlSugar.OrderByType.Asc).ToList();
                if (list.Count == 0)
                {
                    StationReturn.MessageCode = "MSGCODE20210814171135！";
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
        public void AddRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_RANGE _Range = null;
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string ruleID = Data["RuleID"].ToString().Trim().ToUpper();
                string cType = Data["CType"].ToString().Trim().ToUpper();
                string value = Data["Value"].ToString().Trim().ToUpper();
                _Range = new T_R_RANGE(SFCDB, DB_TYPE_ENUM.Oracle);
                if (_Range.CheckExist(SFCDB, ruleID, cType, value))
                {
                    StationReturn.Message = value + " MSGCODE20210814171308！";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                else
                {
                    if (cType == "SKU")
                    {
                        T_C_SKU _Sku = new T_C_SKU(SFCDB, DB_TYPE_ENUM.Oracle);
                        if (!_Sku.CheckSku(value, SFCDB))
                        {
                            throw new Exception(value + " 不存在與系統中！[C_SKU]");
                        }
                    }
                    Row_R_RANGE r = (Row_R_RANGE)_Range.NewRow();
                    r.ID = _Range.GetNewID(this.BU, SFCDB);
                    r.RULEID = ruleID;
                    r.CTYPE = cType;
                    r.VUL = value;
                    r.VALID = "Y";
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
        public void UpdateRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_RANGE T_Rule = new T_R_RANGE(SFCDB, DB_TYPE_ENUM.Oracle);
                string id = Data["ID"].ToString().Trim().ToUpper();
                string ruleId = Data["RuleId"].ToString().Trim().ToUpper();
                string cType = Data["CType"].ToString().Trim().ToUpper();
                string value = Data["Value"].ToString().Trim().ToUpper();
                R_RANGE r = SFCDB.ORM.Queryable<R_RANGE>().Where(t => t.ID == id && t.CTYPE == cType && t.VUL == value && t.VALID == "Y").ToList().FirstOrDefault();
                r.RULEID = ruleId;
                r.EDITBY = this.LoginUser.EMP_NO;
                r.EDITTIME = GetDBDateTime();
                int strRet = SFCDB.ORM.Updateable(r).Where(t => t.ID == id && t.CTYPE == cType && t.VUL == value && t.VALID == "Y").ExecuteCommand();
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
        public void DeleteRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string[] idArry = Data["IDs"].ToString().Trim().ToUpper().Split(',');
                for (int i = 0; i < idArry.Length; i++)
                {
                    string id = idArry[i];
                    int res = SFCDB.ORM.Deleteable<R_RANGE>().Where(t => t.ID == id).ExecuteCommand();
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
    }
}
