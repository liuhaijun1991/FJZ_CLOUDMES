using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

//RuRun
//check  exist
//check overlap zone

namespace MESStation.WOManager
{
    public class SNRangeConfig : MESPubLab.MESStation.MesAPIBase
    {
        public SNRangeConfig()
        {
            this.Apis.Add(FCheckOverlapZone.FunctionName, FCheckOverlapZone);
            this.Apis.Add(FAddWORange.FunctionName, FAddWORange);
            this.Apis.Add(FDeleteWoRange.FunctionName, FDeleteWoRange);
            this.Apis.Add(FQueryAllWoRange.FunctionName, FQueryAllWoRange);
            this.Apis.Add(FModifyWORange.FunctionName, FModifyWORange);
            this.Apis.Add(FQueryWoRangebyWONO.FunctionName, FQueryWoRangebyWONO);
        }

        protected APIInfo FQueryAllWoRange = new APIInfo()
        {
            FunctionName = "QueryAllWoRange",
            Description = "Show data of all WorkOrder Range",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName="SearchType",InputType="string",DefaultValue=""},
                new APIInputInfo() {InputName = "WorkorderNo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PageNumber", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PageSize", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        private APIInfo FQueryWoRangebyWONO = new APIInfo()
        {
            FunctionName = "QueryWoRangebyWONO",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo FCheckOverlapZone = new APIInfo()
        {
            FunctionName = "CheckOverlapZone",
            Description = "Check overlap zone of new range",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "MIN_SN", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>()
            { }

        };

        protected APIInfo FModifyWORange = new APIInfo()
        {
            FunctionName = "ModifyWORange",
            Description = "Edit data of WorkOrder selected",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "QTY", InputType = "string", DefaultValue = "" },// by sdl 20180319
                new APIInputInfo() {InputName = "MIN_SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAX_SN", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddWORange = new APIInfo()
        {
            FunctionName = "AddWORange",
            Description = "Add new WorkOrder Range, max_sn will be auto enter",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "QTY", InputType = "string", DefaultValue = "" },  ///by sdl 20180319
                new APIInputInfo() {InputName = "MIN_SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAX_SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteWoRange = new APIInfo()
        {
            FunctionName = "DeleteWoRange",
            Description = "Mark WorkOrder what was pending",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public void CheckOverlapZone(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_R_WO_REGION sec = null;
            string MIN_SN = string.Empty;
            List<R_WO_REGION> list = new List<R_WO_REGION>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                sec = new T_R_WO_REGION(oleDB, DBTYPE);
                MIN_SN = Data["MIN_SN"].ToString().Trim();
                list = sec.CheckZone(MIN_SN, oleDB);
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception exception)
            {
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
                //this.DBPools["SFCDB"].Return(oleDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = exception.Message;
                StationReturn.Data = "";
            }
        }
        public void QueryAllWoRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string searchType = Data["SearchType"].ToString();
                string strWorkOrder = Data["WorkorderNo"] == null ? "" : Data["WorkorderNo"].ToString().Trim().ToUpper();
                int intCurrentPage = Convert.ToInt32(Data["PageNumber"] == null ? "1" : Data["PageNumber"].ToString().Trim());
                int intPageSize = Convert.ToInt32(Data["PageSize"] == null ? "20" : Data["PageSize"].ToString().Trim());
                int intTotal = 0;
                WoRangeMainPage WoRangePage = new WoRangeMainPage();

                T_R_WO_REGION TC_RANGE = new T_R_WO_REGION(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<R_WO_REGION> getWoRangeData = new List<R_WO_REGION>();
                if (searchType == "WO")
                {
                    getWoRangeData = TC_RANGE.ShowAllDataAndShowPage(sfcdb, strWorkOrder, intCurrentPage, intPageSize, out intTotal);
                }
                else
                {
                    getWoRangeData = TC_RANGE.ShowWORegionBySN(strWorkOrder, sfcdb);
                    intTotal = getWoRangeData.Count;
                }
                if (getWoRangeData != null && getWoRangeData.Count > 0)
                {
                    if (WoRangePage.WoRangeData == null)
                    {
                        WoRangePage.WoRangeData = new List<R_WO_REGION>();
                    }

                    foreach (var item in getWoRangeData)
                    {
                        WoRangePage.WoRangeData.Add(item);
                    }
                }

                WoRangePage.Total = intTotal;
                WoRangePage.CurrentPage = intCurrentPage;
                WoRangePage.PageSize = intPageSize;
                if (intPageSize != 0)
                {
                    double doubleTotal = intTotal;
                    double Countpage = doubleTotal / intPageSize;
                    WoRangePage.CountPage = Convert.ToInt32(Math.Ceiling(Countpage));
                }
                StationReturn.Data = WoRangePage;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";

            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void AddWORange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_R_WO_BASE StWoBase = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE RowWoBase = null;
                RowWoBase = StWoBase.GetWo((Data["WORKORDERNO"].ToString()).Trim(), sfcdb);
                T_R_WO_REGION wo = new T_R_WO_REGION(sfcdb, DB_TYPE_ENUM.Oracle);
                string minSN = (Data["MIN_SN"].ToString()).Trim();
                string maxSN = (Data["MAX_SN"].ToString()).Trim();
                string checkOut = "";
                string _wo = "";
                if (!wo.InputIsStringOrNum(minSN, out checkOut))
                {
                    StationReturn.MessageCode = "MES00000258";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (!wo.InputIsStringOrNum(maxSN, out checkOut))
                {
                    StationReturn.MessageCode = "MES00000258";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                C_SKU objectSku = t_c_sku.GetSku(RowWoBase.SKUNO, sfcdb);
                if (!wo.InputIsMatchSkuRule(minSN, objectSku))
                {
                    StationReturn.MessageCode = "MES00000259";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (!wo.InputIsMatchSkuRule(maxSN, objectSku))
                {
                    StationReturn.MessageCode = "MES00000259";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (wo.CheckSNInRange(minSN, ref _wo, sfcdb))
                {
                    StationReturn.MessageCode = "MSGCODE20181114162339";
                    StationReturn.MessagePara.Add(minSN);
                    StationReturn.MessagePara.Add(_wo);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (wo.CheckSNInRange(maxSN, ref _wo, sfcdb))
                {
                    StationReturn.MessageCode = "MSGCODE20181114162339";
                    StationReturn.MessagePara.Add(maxSN);
                    StationReturn.MessagePara.Add(_wo);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (this.BU == "HWD" && !wo.CompareLastCode(maxSN, minSN, 6))
                {
                    StationReturn.MessageCode = "MSGCODE20181115093730";
                    StationReturn.MessagePara.Add(maxSN);
                    StationReturn.MessagePara.Add(minSN);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                Row_R_WO_REGION r = (Row_R_WO_REGION)wo.NewRow();
                r.ID = wo.GetNewID(BU, sfcdb, DBTYPE);
                r.WORKORDERNO = (Data["WORKORDERNO"].ToString()).Trim();
                r.SKUNO = RowWoBase.SKUNO;
                r.QTY = RowWoBase.WORKORDER_QTY;
                r.MIN_SN = minSN;
                r.MAX_SN = maxSN;
                r.EDIT_EMP = LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = strRet;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void ModifyWORange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_REGION rWo = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rWo = new T_R_WO_REGION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_WO_REGION r = (Row_R_WO_REGION)rWo.NewRow();
                T_R_WO_BASE StWoBase = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE RowWoBase = null;
                RowWoBase = StWoBase.GetWo((Data["WORKORDERNO"].ToString()).Trim(), sfcdb);

                string minSN = (Data["MIN_SN"].ToString()).Trim();
                string maxSN = (Data["MAX_SN"].ToString()).Trim();
                string checkOut = "";
                string _wo = "";
                if (!rWo.InputIsStringOrNum(minSN, out checkOut))
                {
                    StationReturn.MessageCode = "MES00000258";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (!rWo.InputIsStringOrNum(maxSN, out checkOut))
                {
                    StationReturn.MessageCode = "MES00000258";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                C_SKU objectSku = t_c_sku.GetSku(RowWoBase.SKUNO, sfcdb);
                if (!rWo.InputIsMatchSkuRule(minSN, objectSku))
                {
                    StationReturn.MessageCode = "MES00000259";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (!rWo.InputIsMatchSkuRule(maxSN, objectSku))
                {
                    StationReturn.MessageCode = "MES00000259";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (this.BU == "HWD" && !rWo.CompareLastCode(maxSN, minSN, 6))
                {
                    StationReturn.MessageCode = "MSGCODE20181115093730";
                    StationReturn.MessagePara.Add(maxSN);
                    StationReturn.MessagePara.Add(minSN);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                r = (Row_R_WO_REGION)rWo.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                if (!rWo.CheckSNInWoRange(minSN, r.WORKORDERNO, sfcdb, this.BU))
                {
                    if (rWo.CheckSNInRange(minSN, ref _wo, sfcdb))
                    {
                        StationReturn.MessageCode = "MSGCODE20181114162339";
                        StationReturn.MessagePara.Add(minSN);
                        StationReturn.MessagePara.Add(_wo);
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                }
                if (!rWo.CheckSNInWoRange(maxSN, r.WORKORDERNO, sfcdb, this.BU))
                {
                    if (rWo.CheckSNInRange(maxSN, ref _wo, sfcdb))
                    {
                        StationReturn.MessageCode = "MSGCODE20181114162339";
                        StationReturn.MessagePara.Add(maxSN);
                        StationReturn.MessagePara.Add(_wo);
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                }
                r.ID = (Data["ID"].ToString()).Trim();
                r.WORKORDERNO = (Data["WORKORDERNO"].ToString()).Trim();
                r.SKUNO = RowWoBase.SKUNO;
                r.QTY = RowWoBase.WORKORDER_QTY;
                //r.SKUNO = (Data["SKUNO"].ToString()).Trim();
                //r.QTY = int.Parse(Data["QTY"].ToString());
                r.MIN_SN = minSN;
                r.MAX_SN = maxSN;
                r.EDIT_EMP = LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.MessageCode = "MES00000003";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                //this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                //this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        public void QueryWoRangebyWONO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<R_WO_REGION> list = new List<R_WO_REGION>();
            T_R_WO_REGION rwo;
            string WO = Data["WORKORDERNO"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rwo = new T_R_WO_REGION(sfcdb, DBTYPE);
                list = rwo.GetWObyWONO(WO, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES000000016";
                StationReturn.Data = list;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        public void DeleteWoRange(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_WO_REGION wo = new T_R_WO_REGION(sfcdb, DB_TYPE_ENUM.Oracle);
                foreach (var item in Data["ID"])
                {
                    Row_R_WO_REGION r = (Row_R_WO_REGION)wo.GetObjByID(item.ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                    string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                    if (Convert.ToInt32(strRet) > 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000004";
                        StationReturn.Data = "";
                        //writeLog
                        T_R_MES_LOG mesLog = new T_R_MES_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                        string id = mesLog.GetNewID(this.BU, sfcdb);
                        Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
                        rowMESLog.ID = id;
                        rowMESLog.PROGRAM_NAME = "Web";
                        rowMESLog.CLASS_NAME = this.GetType().ToString();
                        rowMESLog.FUNCTION_NAME = "DeleteWoRange";
                        rowMESLog.LOG_MESSAGE = "Success";
                        rowMESLog.LOG_SQL = r.GetDeleteString(DB_TYPE_ENUM.Oracle);
                        rowMESLog.EDIT_EMP = this.LoginUser.EMP_NO;
                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                        sfcdb.ThrowSqlExeception = true;
                        sfcdb.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "NotLatestData";
                    }
                }
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
//RuRun

