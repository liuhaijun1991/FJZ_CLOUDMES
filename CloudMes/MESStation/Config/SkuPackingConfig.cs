using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;

namespace MESStation.Config
{
    public class SkuPackingConfig : MesAPIBase
    {
        protected APIInfo FAddSkuPackingRule = new APIInfo()
        {
            FunctionName = "AddSkuPackingRule",
            Description = "AddSkuPackingRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SCANTYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REGEX", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FModifySkuPackingRule = new APIInfo()
        {
            FunctionName = "ModifySkuPackingRule",
            Description = "ModifySkuPackingRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REGEX", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteSkuPackingRule = new APIInfo()
        {
            FunctionName = "DeleteSkuPackingRule",
            Description = "DeleteSkuPackingRule",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "IDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        private APIInfo FAllCSKU = new APIInfo()
        {
            FunctionName = "GetAllCSku",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo FGetSKUByCSKU = new APIInfo()
        {
            FunctionName = "GetSkuBySkuNo",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
            { }
        };
        public SkuPackingConfig()
        {
            this.Apis.Add(FAllCSKU.FunctionName, FAllCSKU);
            this.Apis.Add(FGetSKUByCSKU.FunctionName, FGetSKUByCSKU);

            //this.Apis.Add(FQueryPackingRule.FunctionName, FQueryPackingRule);
            this.Apis.Add(FAddSkuPackingRule.FunctionName, FAddSkuPackingRule);
            this.Apis.Add(FModifySkuPackingRule.FunctionName, FModifySkuPackingRule);
            this.Apis.Add(FDeleteSkuPackingRule.FunctionName, FDeleteSkuPackingRule);
        }

        public void GetSkuBySkuNo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_PACKING_RULE> SkuList = new List<C_PACKING_RULE>();
            T_C_PACKING_RULE Table = null;
            string skuNo = Data["SKUNO"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_PACKING_RULE(sfcdb, DBTYPE);
                SkuList = Table.GetPackingBySku(skuNo, sfcdb);
                if (SkuList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = SkuList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetAllCSku(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_SKU> SkuList = new List<C_SKU>();
            T_C_SKU Table = null;
            string skuNo = Data["SKUNO"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_SKU(sfcdb, DBTYPE);
                SkuList = Table.GetAllCSku(sfcdb);
                if (SkuList.Count() == 0)
                {
                    //沒有獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    //獲取到數據
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(SkuList.Count().ToString());
                    StationReturn.Data = SkuList;
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void AddSkuPackingRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string skuNo = Data["SKUNO"].ToString().Trim(), ScanType = Data["SCANTYPE"].ToString().Trim(), REGEX = Data["REGEX"].ToString().Trim();
            OleExec oleDB = null;
            T_C_PACKING_RULE cKpRule = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpRule = new T_C_PACKING_RULE(oleDB, DBTYPE);
                if (cKpRule.IsExistsFTX(oleDB, skuNo, ScanType, REGEX))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    string id = cKpRule.GenarateID(oleDB);
                    if (id.Length > 0 && !"ERROR".Contains(id))
                    {
                        cKpRule.InsertData(oleDB, id, skuNo, ScanType, REGEX, this.LoginUser.EMP_NO);
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000002";
                        StationReturn.Data = "";
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        StationReturn.Data = "";
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void ModifySkuPackingRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString().Trim(),
                   REGEX = Data["REGEX"].ToString().Trim();
            OleExec oleDB = null;
            T_C_PACKING_RULE cKpCheck = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpCheck = new T_C_PACKING_RULE(oleDB, DBTYPE);
                Row_C_PACKING_RULE row = cKpCheck.GetPackingInfoBySku(id, oleDB);
                if (row == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Data = "";
                }
                else
                {
                    row.REGEX = REGEX;
                    row.EDIT_EMP = this.LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime();
                    cKpCheck.UpdateDate(oleDB, id, REGEX, GetDBDateTime(), this.LoginUser.EMP_NO);
                    //oleDB.ThrowSqlExeception = true;
                    //oleDB.ExecSQL(row.GetUpdateString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000003";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                oleDB.ThrowSqlExeception = false;
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void DeleteSkuPackingRule(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string RWOKPREPLACEIDS = Data["IDS"].ToString().Trim();
            ArrayList DelIds = new ArrayList(RWOKPREPLACEIDS.Split(','));
            DelIds.Remove("");
            OleExec oleDB = null;
            T_C_PACKING_RULE cKpRule = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                cKpRule = new T_C_PACKING_RULE(oleDB, DBTYPE);
                oleDB.BeginTrain();
                foreach (var VARIABLE in (string[])DelIds.ToArray(typeof(string)))
                {
                    //oleDB.ThrowSqlExeception = true;
                    //Row_C_PACKING_RULE row = cKpCheck.GetPackingInfoBySku(VARIABLE, oleDB);
                    cKpRule.DeleteSku(oleDB, VARIABLE);
                    // oleDB.ExecSQL(row.DeleteSku(DBTYPE, VARIABLE));
                }
                oleDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
            }
            catch (Exception e)
            {
                oleDB.RollbackTrain();
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

    }
}
