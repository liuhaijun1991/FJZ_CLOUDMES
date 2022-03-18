using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using System.Data;
using MESDataObject.Module.OM;
using SqlSugar;
using MESStation.LogicObject;

namespace MESStation.Management
{
    public class LockManager : MesAPIBase
    {
        protected APIInfo FLock = new APIInfo
        {
            FunctionName = "DoLock",
            Description = "Do lock by wo or lot_no or sn",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "LockType" },
                new APIInputInfo() { InputName = "LockData" },
                new APIInputInfo() { InputName = "LockReason" },
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUnlock = new APIInfo
        {
            FunctionName = "DoUnlock",
            Description = "Do unlock by id",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "ID"},
                new APIInputInfo(){ InputName = "UnlockReason"}
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetLockStation = new APIInfo
        {
            FunctionName = "GetLockStation",
            Description = "Get lock station",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){InputName = "LockType" },
                new APIInputInfo(){InputName = "LockData"}
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetLockInfo = new APIInfo
        {
            FunctionName = "GetLockInfo",
            Description = "Get lock info by wo or sn or lot",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "Type"},
                new APIInputInfo(){ InputName = "Data"},
                new APIInputInfo(){ InputName = "Status"}
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FByPassSN = new APIInfo
        {
            FunctionName = "ByPassSN",
            Description = "Lock ByPass SN",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "SN"},
                new APIInputInfo(){ InputName = "REASON"},
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FByPassWO = new APIInfo
        {
            FunctionName = "ByPassWO",
            Description = "Lock ByPass WO",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "WO"},
                new APIInputInfo(){ InputName = "REASON"},
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetByPassData = new APIInfo
        {
            FunctionName = "GetByPassData",
            Description = "GetByPassData",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "SN"},
                new APIInputInfo(){ InputName = "WO"}
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUNDOByPass = new APIInfo
        {
            FunctionName = "UNDOByPass",
            Description = "UNDOByPass",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "ID"},
            },
            Permissions = new List<MESPermission>()
        };

        //獲取所有的處於Locked中的數據
        protected APIInfo FGetLockedAllInfo = new APIInfo
        {
            FunctionName = "GetLockedAllInfo",
            Description = "Get All Locked Info",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetBypassInfo = new APIInfo
        {
            FunctionName = "GetBypassInfo",
            Description = "Get All Bypass Info",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo(){ InputName = "Type"},
                new APIInputInfo(){ InputName = "Data"},
                new APIInputInfo(){ InputName = "Status"}
            },
            Permissions = new List<MESPermission>()
        };

        public LockManager()
        {
            this.Apis.Add(FLock.FunctionName, FLock);
            this.Apis.Add(FUnlock.FunctionName, FUnlock);
            this.Apis.Add(FGetLockStation.FunctionName, FGetLockStation);
            this.Apis.Add(FGetLockInfo.FunctionName, FGetLockInfo);
            this.Apis.Add(FGetLockedAllInfo.FunctionName, FGetLockedAllInfo);
            this.Apis.Add(FByPassSN.FunctionName, FByPassSN);
            this.Apis.Add(FByPassWO.FunctionName, FByPassWO);
            this.Apis.Add(FGetByPassData.FunctionName, FGetByPassData);
            this.Apis.Add(FUNDOByPass.FunctionName, FUNDOByPass);
            this.Apis.Add(FGetBypassInfo.FunctionName, FGetBypassInfo);
        }

        //GetLockedAllInfo顯示所有的正處於Locked的數據
        public void GetLockedAllInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_SN_LOCK>().Where(rsl => rsl.LOCK_STATUS == "1" && rsl.LOCK_EMP != "EcnLock")
                    .OrderBy(rsl => rsl.LOCK_TIME, SqlSugar.OrderByType.Desc)
                    .OrderBy(rsl => rsl.SN, SqlSugar.OrderByType.Asc).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        
        public void GetLockedByEcn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.LOCK_STATUS == "1" && t.LOCK_EMP == "EcnLock")
                    .OrderBy(rsl => rsl.LOCK_TIME, SqlSugar.OrderByType.Desc)
                    .OrderBy(rsl => rsl.SN, SqlSugar.OrderByType.Asc).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetLockInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            //Patty 20190625 update query for FTX

            string type = Data["Type"].ToString().ToUpper();
            string status = Data["Status"].ToString();
            List<string> snList = new List<string>();
            OleExec sfcdb = null;
            List<R_SN_LOCK> lockList = new List<R_SN_LOCK>();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(sfcdb, DBTYPE);
                if (status.ToUpper() == "ALL")
                {
                    status = "";
                }
                if (type == "BYWO")
                {
                    if (LoginUser.FACTORY == "FTX")
                    {
                        lockList = t_r_sn_lock.FTXGetLockList("", Data["Data"].ToString(), status, sfcdb);

                    }
                    else
                    {
                        lockList = t_r_sn_lock.GetLockList("", "", "", Data["Data"].ToString(), status, sfcdb);

                    }
                }
                else if (type == "BYLOT")
                {
                    lockList = t_r_sn_lock.GetLockList("", Data["Data"].ToString(), "", "", status, sfcdb);
                }
                else if (type == "BYSN")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["Data"];
                    if (LoginUser.FACTORY == "FTX")
                    {
                        for (int i = 0; i < arraySN.Count; i++)
                        {
                            lockList.AddRange(t_r_sn_lock.FTXGetLockList(arraySN[i].ToString(), "", status, sfcdb));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < arraySN.Count; i++)
                        {
                            lockList.AddRange(t_r_sn_lock.GetLockList("", "", arraySN[i].ToString(), "", status, sfcdb));
                        }
                    }

                }
                else if (type == "BYID")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["Data"];
                    for (int i = 0; i < arraySN.Count; i++)
                    {
                        lockList.AddRange(t_r_sn_lock.GetLockList(arraySN[i].ToString(), "", "", "", status, sfcdb));
                    }
                }
                else if (type == "BYSKU")
                {
                    string sku = Data["Data"].ToString().Trim().ToUpper();
                    lockList = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(r => r.TYPE == "SKU" && r.WORKORDERNO == sku).WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(status), r => r.LOCK_STATUS == status).ToList();
                }
                else if (type == "BYPN") //by Component PN
                {
                    string sku = Data["Data"].ToString().Trim().ToUpper();
                    lockList = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(r => r.TYPE == "PN" && r.WORKORDERNO.ToUpper() == sku).WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(status), r => r.LOCK_STATUS == status).ToList();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = lockList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetLockStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string lockType = Data["LockType"].ToString().Trim();
            string lockData = Data["LockData"].ToString().Trim();
            DataTable routeTable = new DataTable();
            List<string> stationList = new List<string>() { "ALL" };
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DBTYPE);
                if (lockType == "LockByWo")
                {
                    T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, DBTYPE);
                    Row_R_WO_BASE rowWo = t_r_wo_base.GetWo(lockData, sfcdb);
                    if (rowWo == null)
                    {
                        throw new Exception(lockData + " Not Exists!");
                    }
                    R_WO_BASE r_wo_base = rowWo.GetDataObject();
                    stationList.AddRange(t_c_route_detail.GetByRouteIdOrderBySEQASC(r_wo_base.ROUTE_ID, sfcdb).Select(route => route.STATION_NAME).ToList());
                }
                else if (lockType == "LockBySku")
                {
                    T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DBTYPE);
                    C_SKU c_sku = t_c_sku.GetSku(lockData, sfcdb);
                    if (c_sku == null)
                    {
                        throw new Exception(lockData + " Not Exists!");
                    }

                    R_SKU_ROUTE skuRoute = sfcdb.ORM.Queryable<R_SKU_ROUTE>().Where(r => r.SKU_ID == c_sku.ID).OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).First();

                    if (skuRoute == null)
                    {
                        throw new Exception("The SKUNO: " + c_sku.SKUNO + " dont have ROUTING, pls contact to PE!");
                    }

                    var x = t_c_route_detail.GetByRouteIdOrderBySEQASC(skuRoute.ROUTE_ID, sfcdb).Select(route => route.STATION_NAME).ToList();
                    stationList.AddRange(x);

                }
                else if (lockType == "LockByLot")
                {
                    T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(sfcdb, DBTYPE);
                    Row_R_LOT_STATUS rowLotStatus = t_r_lot_status.GetByLotNo(lockData, sfcdb);
                    if (rowLotStatus.ID == null)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000161", new string[] { }));
                    }
                    R_LOT_STATUS r_lot_status = rowLotStatus.GetDataObject();
                    T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DBTYPE);
                    C_SKU c_sku = t_c_sku.GetSku(r_lot_status.SKUNO, sfcdb);
                    T_R_SKU_ROUTE t_r_sku_route = new T_R_SKU_ROUTE(sfcdb, DBTYPE);
                    List<R_SKU_ROUTE> r_sku_route_list = t_r_sku_route.GetMappingBySkuId(c_sku.ID, sfcdb);
                    if (r_sku_route_list.Count > 0)
                    {
                        //t_c_route_detail.GetByRouteIdOrderBySEQASC(r_sku_route_list[0].ROUTE_ID, sfcdb);
                        stationList.AddRange(t_c_route_detail.GetByRouteIdOrderBySEQASC(r_sku_route_list[0].ROUTE_ID, sfcdb).Select(route => route.STATION_NAME).ToList());
                    }
                    else
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000179", new string[] { }));
                    }

                }
                else if (lockType == "LockByPOLine")
                {
                    var POLineInfo = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.UPOID == lockData).First();
                    if (POLineInfo == null)
                    {
                        throw new Exception(lockData + " Not Exists!");
                    }
                    stationList.Clear();
                    stationList.Add("CONVERTWO");
                }
                else if (lockType == "LockBySO")
                {
                    stationList.Add("CONVERTWO");
                }
                else
                {
                    routeTable = t_c_route_detail.GetALLStation(sfcdb);
                    foreach (DataRow row in routeTable.Rows)
                    {
                        stationList.Add(row["station_name"].ToString());
                    }
                    stationList.Sort();
                }

                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = stationList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DoLock(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string lockType = Data["LockType"].ToString().Trim();
            string lockReason = Data["LockReason"].ToString().Trim();
            string lockStation = Data["LockStation"].ToString().Trim();
            OleExec sfcdb = null;
            T_R_SN_LOCK t_r_sn_lock = null;
            Row_R_SN_LOCK rowSNLock = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                t_r_sn_lock = new T_R_SN_LOCK(sfcdb, DBTYPE);
                if (lockType == "LockByWo")
                {
                    if (t_r_sn_lock.IsUnLock("", "", Data["LockData"].ToString().Trim(), lockStation, sfcdb))
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["LockData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNLock = (Row_R_SN_LOCK)t_r_sn_lock.NewRow();
                    rowSNLock.ID = t_r_sn_lock.GetNewID(this.BU, sfcdb);
                    rowSNLock.WORKORDERNO = Data["LockData"].ToString().Trim();
                    rowSNLock.TYPE = "WO";
                    rowSNLock.LOCK_STATION = lockStation;
                    rowSNLock.LOCK_REASON = lockReason;
                    rowSNLock.LOCK_STATUS = "1";
                    rowSNLock.LOCK_EMP = this.LoginUser.EMP_NO;
                    rowSNLock.LOCK_TIME = GetDBDateTime();
                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNLock.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (lockType == "LockByLot")
                {
                    if (t_r_sn_lock.IsUnLock(Data["LockData"].ToString().Trim(), "", "", lockStation, sfcdb))
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["LockData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNLock = (Row_R_SN_LOCK)t_r_sn_lock.NewRow();
                    rowSNLock.ID = t_r_sn_lock.GetNewID(this.BU, sfcdb);
                    rowSNLock.LOCK_LOT = Data["LockData"].ToString().Trim();
                    rowSNLock.TYPE = "LOT";
                    rowSNLock.LOCK_STATION = lockStation;
                    rowSNLock.LOCK_REASON = lockReason;
                    rowSNLock.LOCK_STATUS = "1";
                    rowSNLock.LOCK_EMP = this.LoginUser.EMP_NO;
                    rowSNLock.LOCK_TIME = GetDBDateTime();
                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNLock.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (lockType == "LockBySn")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["LockData"];
                    for (int i = 0; i < arraySN.Count; i++)
                    {
                        if (!t_r_sn_lock.IsUnLock(arraySN[i].ToString(), "", "", lockStation, sfcdb))
                        {
                            rowSNLock = (Row_R_SN_LOCK)t_r_sn_lock.NewRow();
                            rowSNLock.ID = t_r_sn_lock.GetNewID(this.BU, sfcdb);
                            rowSNLock.SN = arraySN[i].ToString().ToUpper();
                            rowSNLock.TYPE = "SN";
                            rowSNLock.LOCK_STATION = lockStation;
                            rowSNLock.LOCK_REASON = lockReason;
                            rowSNLock.LOCK_STATUS = "1";
                            rowSNLock.LOCK_EMP = this.LoginUser.EMP_NO;
                            rowSNLock.LOCK_TIME = GetDBDateTime();
                            sfcdb.ThrowSqlExeception = true;
                            sfcdb.ExecSQL(rowSNLock.GetInsertString(DBTYPE));
                        }
                    }
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (lockType == "LockBySku")
                {
                    string skuno = Data["LockData"]["Sku"].ToString().Trim();
                    string skurev = Data["LockData"]["SkuRev"].ToString().Trim().Equals("")?"ALL": Data["LockData"]["SkuRev"].ToString().Trim();
                    var EffeciveTime = Data["LockData"]["EffeciveTime"].ToString().Trim().Equals("")? GetDBDateTime():Convert.ToDateTime(Data["LockData"]["EffeciveTime"].ToString().Trim());
                    R_SN_LOCK lockObj = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(r => r.TYPE == "SKU" && r.WORKORDERNO == skuno &&  r.SN== skurev && r.LOCK_STATUS == "1").ToList().FirstOrDefault();
                    if (lockObj != null)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["LockData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNLock = (Row_R_SN_LOCK)t_r_sn_lock.NewRow();
                    rowSNLock.ID = t_r_sn_lock.GetNewID(this.BU, sfcdb);
                    rowSNLock.WORKORDERNO = skuno;
                    rowSNLock.TYPE = "SKU";
                    rowSNLock.SN = skurev;
                    rowSNLock.LOCK_STATION = lockStation;
                    rowSNLock.LOCK_REASON = lockReason;
                    rowSNLock.LOCK_STATUS = "1";
                    rowSNLock.LOCK_EMP = this.LoginUser.EMP_NO;
                    rowSNLock.LOCK_TIME = EffeciveTime;
                    rowSNLock.CREATETIME = DateTime.Now;
                    rowSNLock.CREATEBY = this.LoginUser.EMP_NO;

                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNLock.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (lockType == "LockByPn")
                {
                    string partno = Data["LockData"].ToString().Trim();
                    R_SN_LOCK lockObj = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(r => r.TYPE == "PN" && r.WORKORDERNO == partno && r.LOCK_STATUS == "1").ToList().FirstOrDefault();
                    if (lockObj != null)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["LockData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNLock = (Row_R_SN_LOCK)t_r_sn_lock.NewRow();
                    rowSNLock.ID = t_r_sn_lock.GetNewID(this.BU, sfcdb);
                    rowSNLock.WORKORDERNO = partno;
                    rowSNLock.TYPE = "PN";
                    rowSNLock.LOCK_STATION = lockStation;
                    rowSNLock.LOCK_REASON = lockReason;
                    rowSNLock.LOCK_STATUS = "1";
                    rowSNLock.LOCK_EMP = this.LoginUser.EMP_NO;
                    rowSNLock.LOCK_TIME = GetDBDateTime();
                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNLock.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (lockType == "LockByPOLine")
                {
                    string poline = Data["LockData"].ToString().Trim();
                    R_SN_LOCK lockObj = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(r => r.TYPE == "POLine" && r.WORKORDERNO == poline && r.LOCK_STATUS == "1").ToList().FirstOrDefault();
                    if (lockObj != null)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["LockData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNLock = (Row_R_SN_LOCK)t_r_sn_lock.NewRow();
                    rowSNLock.ID = t_r_sn_lock.GetNewID(this.BU, sfcdb);
                    rowSNLock.WORKORDERNO = poline;
                    rowSNLock.TYPE = "POLine";
                    rowSNLock.LOCK_STATION = lockStation;
                    rowSNLock.LOCK_REASON = lockReason;
                    rowSNLock.LOCK_STATUS = "1";
                    rowSNLock.LOCK_EMP = this.LoginUser.EMP_NO;
                    rowSNLock.LOCK_TIME = GetDBDateTime();
                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNLock.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else if (lockType == "LockBySO")
                {
                    string so = Data["LockData"].ToString().Trim();
                    R_SN_LOCK lockObj = sfcdb.ORM.Queryable<R_SN_LOCK>().Where(r => r.TYPE == "SO" && r.WORKORDERNO == so && r.LOCK_STATUS == "1").First();
                    if (lockObj != null)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20180730134109";
                        StationReturn.MessagePara.Add(Data["LockData"].ToString().Trim());
                        StationReturn.Data = "";
                        return;
                    }
                    rowSNLock = (Row_R_SN_LOCK)t_r_sn_lock.NewRow();
                    rowSNLock.ID = t_r_sn_lock.GetNewID(this.BU, sfcdb);
                    rowSNLock.WORKORDERNO = so;
                    rowSNLock.TYPE = "SO";
                    rowSNLock.LOCK_STATION = lockStation;
                    rowSNLock.LOCK_REASON = lockReason;
                    rowSNLock.LOCK_STATUS = "1";
                    rowSNLock.LOCK_EMP = this.LoginUser.EMP_NO;
                    rowSNLock.LOCK_TIME = GetDBDateTime();
                    sfcdb.ThrowSqlExeception = true;
                    sfcdb.ExecSQL(rowSNLock.GetInsertString(DBTYPE));
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { "LockType" }));
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void DoUnlock(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            string unlockReason = Data["UnlockReason"].ToString();
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(sfcdb, DBTYPE);

                for (int i = 0; i < arraySN.Count; i++)
                {
                    Row_R_SN_LOCK rowLock = (Row_R_SN_LOCK)t_r_sn_lock.GetObjByID(arraySN[i].ToString(), sfcdb);
                    if (rowLock.LOCK_STATUS == "1")
                    {
                        rowLock.LOCK_STATUS = "0";
                        rowLock.UNLOCK_REASON = unlockReason;
                        rowLock.UNLOCK_EMP = this.LoginUser.EMP_NO;
                        rowLock.UNLOCK_TIME = GetDBDateTime();
                        sfcdb.ExecSQL(rowLock.GetUpdateString(DBTYPE));
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = "";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="type">Input data type:SKU/WO/SN/CSN/PALLET</param>
        /// <param name="station">Current Station</param>
        /// <param name="_db">SqlSugarClient database connection object</param>
        /// <returns></returns>
        public static List<R_SN_LOCK> CheckLock(string data, string type, string station, OleExec db)
        {
            SqlSugarClient _db = db.ORM;
            List<R_SN_LOCK> locks = new List<R_SN_LOCK>();
            O_ORDER_MAIN poinfo = null;
            switch (type)
            {
                case "SKU":
                    locks = _db.Queryable<R_SN_LOCK>()
                        .Where((r) =>
                        r.LOCK_STATUS == "1"
                        && r.TYPE == "SKU"
                        && r.WORKORDERNO == data
                        && (r.LOCK_STATION == station
                            || r.LOCK_STATION == "ALL")
                        && (r.SN == null || r.SN == "")
                        )
                        .Distinct()
                        .ToList();
                    break;
                case "WO":
                    var wo = _db.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == data).First();
                    var woBypass = _db.Queryable<R_LOCK_BYPASS>().Where(t => t.TYPE == "WO" && t.VALUE1 == data && t.BYPASS_STATUS == 1).First();
                    if (woBypass != null)
                    {
                        break;
                    }

                    try
                    {
                        poinfo = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == data).First();
                    }
                    catch (Exception)
                    {
                    }
                    if (wo == null && poinfo == null)
                    {
                        return locks;
                    }
                    if (poinfo == null && wo != null)
                    {
                        locks = _db.Queryable<R_SN_LOCK>()
                            .Where((r) =>
                            r.LOCK_STATUS == "1"
                            && (r.LOCK_STATION == station || r.LOCK_STATION == "ALL")
                            && ((r.WORKORDERNO == wo.WORKORDERNO && r.TYPE == "WO")
                                || (r.WORKORDERNO == wo.SKUNO && r.TYPE == "SKU" && (r.SN == null || r.SN == "")))
                            )
                        .Distinct()
                        .ToList();
                    }
                    else
                    {
                        var so = _db.Queryable<O_I137_HEAD, O_I137_ITEM>((H, I) => new object[] { JoinType.Left, H.TRANID == I.TRANID }).Where((H, I) => I.ID == poinfo.ITEMID).Select((H, I) => H.SALESORDERNUMBER).First();
                        var pn = _db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == poinfo.ID).Select(t => t.FOXPN).Distinct().ToList();
                        locks = _db.Queryable<R_SN_LOCK>()
                            .Where((r) =>
                            r.LOCK_STATUS == "1"
                            && (r.LOCK_STATION == station || r.LOCK_STATION == "ALL")
                            && ((r.WORKORDERNO == poinfo.PREWO && r.TYPE == "WO")
                                || (r.WORKORDERNO == poinfo.PID && r.TYPE == "SKU" && (r.SN == null || r.SN == ""))
                                //|| (r.WORKORDERNO == poinfo.PID && r.TYPE == "SKU" &&  r.SN == wo.SKU_VER)
                                || (r.WORKORDERNO == poinfo.UPOID && r.TYPE == "POLine")
                                || (r.WORKORDERNO == so && r.TYPE == "SO")
                                || (pn.Contains(r.WORKORDERNO) && r.TYPE == "PN"))//增加检查PN锁
                            )
                        .Distinct()
                        .ToList();
                    }
                    break;
                case "SN":
                    SN.LockCheck(data, db);
                    #region f
                    //var sn = _db.Queryable<R_SN>()
                    //    .Where(t => t.SN == data && t.VALID_FLAG == "1")
                    //    .First();
                    //if (sn == null)
                    //{
                    //    locks = _db.Queryable<R_SN_LOCK>()
                    //        .Where((r) =>
                    //        r.SN == data
                    //        && r.TYPE == "SN"
                    //        && r.LOCK_STATUS == "1"
                    //        && (r.LOCK_STATION == station
                    //            || r.LOCK_STATION == "ALL")
                    //        )
                    //    .Distinct()
                    //    .ToList();
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        poinfo = _db.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == sn.WORKORDERNO).First();
                    //    }
                    //    catch (Exception)
                    //    {
                    //    }
                    //    var wo1 = _db.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == sn.WORKORDERNO).First();
                    //    if (poinfo == null)
                    //    {

                    //        locks = _db.Queryable<R_SN_LOCK>()
                    //            .Where((r) =>
                    //            r.LOCK_STATUS == "1"
                    //            && (r.LOCK_STATION == station || r.LOCK_STATION == "ALL")
                    //            && ((r.SN == sn.ID && r.TYPE == "SN")
                    //                || (r.SN == sn.SN && r.TYPE == "SN")
                    //                || (r.WORKORDERNO == sn.WORKORDERNO && r.TYPE == "WO")
                    //                //|| (r.WORKORDERNO == sn.SKUNO && r.TYPE == "SKU"))
                    //                || (r.WORKORDERNO == sn.SKUNO && r.TYPE == "SKU" && (r.SN == null || r.SN == ""))
                    //            || (r.WORKORDERNO == sn.SKUNO && r.TYPE == "SKU" && r.SN == wo1.SKU_VER)
                    //            )
                    //            )
                    //        .Distinct()
                    //        .ToList();
                    //    }
                    //    else
                    //    {
                    //        var so = _db.Queryable<O_I137_HEAD, O_I137_ITEM>((H, I) => new object[] { JoinType.Left, H.TRANID == I.TRANID }).Where((H, I) => I.ID == poinfo.ITEMID).Select((H, I) => H.SALESORDERNUMBER).First();
                    //        var pn = _db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == poinfo.ID).Select(t => t.FOXPN).Distinct().ToList();
                    //        locks = _db.Queryable<R_SN_LOCK>()
                    //            .Where((r) =>
                    //            r.LOCK_STATUS == "1"
                    //            && (r.LOCK_STATION == station || r.LOCK_STATION == "ALL")
                    //            && ((r.SN == sn.ID && r.TYPE == "SN")
                    //                || (r.SN == sn.SN && r.TYPE == "SN")
                    //                || (r.WORKORDERNO == sn.WORKORDERNO && r.TYPE == "WO")
                    //                || (r.WORKORDERNO == sn.SKUNO && r.TYPE == "SKU" && (r.SN == null || r.SN == ""))
                    //                || (r.WORKORDERNO == sn.SKUNO && r.TYPE == "SKU" && r.SN == wo1.SKU_VER)
                    //                || (r.WORKORDERNO == poinfo.UPOID && r.TYPE == "POLine")
                    //                || (r.WORKORDERNO == so && r.TYPE == "SO")
                    //                || (pn.Contains(r.WORKORDERNO) && r.TYPE == "PN"))//增加检查PN锁
                    //            )
                    //          .Distinct()
                    //          .ToList();
                    //    }
                    //}
                    #endregion
                    break;
                case "CSN":
                    var csn = _db.Queryable<R_SN>()
                        .Where(t => t.SN == data && t.VALID_FLAG == "1")
                        .First();
                    if (csn == null)
                    {
                        locks = _db.Queryable<R_SN_LOCK>()
                            .Where((r) =>
                            r.LOCK_STATUS == "1"
                            && r.SN == data
                            && r.TYPE == "SN"
                            && (r.LOCK_STATION == station
                                || r.LOCK_STATION == "ALL")
                            )
                        .Distinct()
                        .ToList();
                    }
                    else
                    {
                        locks = _db.Queryable<R_SN_LOCK>()
                            .Where((r) =>
                            r.LOCK_STATUS == "1"
                            && (r.LOCK_STATION == station || r.LOCK_STATION == "ALL")
                            && ((r.SN == csn.SN && r.TYPE == "SN")
                                || (r.WORKORDERNO == csn.SKUNO && r.TYPE == "PN"))
                            )
                        .Distinct()
                        .ToList();
                    }
                    break;
                case "PALLET":
                    #region f
                    //var squgar = _db.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((SN, SP, PC, PL) => new object[] {
                    //        SqlSugar.JoinType.Left,SN.ID==SP.SN_ID,
                    //        SqlSugar.JoinType.Left,SP.PACK_ID==PC.ID,
                    //        SqlSugar.JoinType.Left,PC.PARENT_PACK_ID==PL.ID
                    //    })
                    //    .Where((SN, SP, PC, PL) => PL.PACK_NO == data);
                    //List<string> sns = new List<string>();
                    //sns = squgar.Select((SN, SP, PC, PL) => SN.SN).ToList();
                    //List<string> wos = new List<string>();
                    //wos = squgar.Select((SN, SP, PC, PL) => SN.WORKORDERNO).ToList();
                    //List<string> skus = new List<string>();
                    //skus = squgar.Select((SN, SP, PC, PL) => SN.SKUNO).ToList();

                    //try
                    //{
                    //    poinfo = _db.Queryable<O_ORDER_MAIN>().Where(t => wos.Contains(t.PREWO)).First();
                    //}
                    //catch (Exception)
                    //{
                    //}
                    //if (poinfo == null)
                    //{
                    //    locks = _db.Queryable<R_SN_LOCK>()
                    //        .Where((r) =>
                    //        r.LOCK_STATUS == "1"
                    //        && (r.LOCK_STATION == station || r.LOCK_STATION == "ALL")
                    //        && (sns.Contains(r.SN)
                    //            || wos.Contains(r.WORKORDERNO)
                    //            || skus.Contains(r.WORKORDERNO)
                    //        ))
                    //    .Distinct()
                    //    .ToList();
                    //}
                    //else
                    //{
                    //    var so = _db.Queryable<O_I137_HEAD, O_I137_ITEM>((H, I) => new object[] { JoinType.Left, H.TRANID == I.TRANID }).Where((H, I) => I.ID == poinfo.ITEMID).Select((H, I) => H.SALESORDERNUMBER).First();
                    //    locks = _db.Queryable<R_SN_LOCK>()
                    //        .Where((r) => (r.LOCK_STATUS == "1"
                    //        && (r.LOCK_STATION == station || r.LOCK_STATION == "ALL")
                    //        && ((sns.Contains(r.SN) && r.TYPE == "SN")
                    //            || (wos.Contains(r.WORKORDERNO) && r.TYPE == "WO")
                    //            //|| (skus.Contains(r.WORKORDERNO) && r.TYPE == "SKU")
                    //            || (skus.Contains(r.WORKORDERNO) && r.TYPE == "SKU" && (r.SN == null || r.SN == ""))
                    //                //|| (skus.Contains(r.WORKORDERNO) && r.TYPE == "SKU" && r.SN == wo1.SKU_VER)
                    //            || (r.WORKORDERNO == poinfo.UPOID && r.TYPE == "POLine")
                    //            || (r.WORKORDERNO == so && r.TYPE == "SO"))
                    //        ))
                    //    .Distinct()
                    //    .ToList();
                    //}
                    #endregion
                    var squgar1 = _db.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((SN, SP, PC, PL) => new object[] {
                            SqlSugar.JoinType.Left,SN.ID==SP.SN_ID,
                            SqlSugar.JoinType.Left,SP.PACK_ID==PC.ID,
                            SqlSugar.JoinType.Left,PC.PARENT_PACK_ID==PL.ID
                        })
                        .Where((SN, SP, PC, PL) => PL.PACK_NO == data);
                    List<string> sns1 = new List<string>();
                    sns1 = squgar1.Select((SN, SP, PC, PL) => SN.SN).ToList();

                    for (int i = 0; i < sns1.Count; i++)
                    {
                        SN.LockCheck(sns1[i], db);
                    }

                    break;
                #region 不是這麼用的，輸入的data是什麼，case這裡就是什麼類型,PN鎖檢查已經放到WO和SN里面檢查
                //case "PN":
                //    var cseries = _db.Queryable<O_ORDER_MAIN, C_SKU, C_SERIES>((o, c, s) => c.C_SERIES_ID == s.ID)
                //    .Where((o, c, s) => c.SKUNO == o.PID && o.PREWO == data).Select((o, c, s) => s)
                //    .ToList().FirstOrDefault();
                //    if (cseries != null)
                //    {
                //        //检查物料被鎖
                //        var PnLock = _db.Queryable<R_SN_LOCK, O_ORDER_MAIN, O_ORDER_OPTION>
                //            ((SL, OM, OO) => SL.WORKORDERNO == OO.FOXPN && OM.ID == OO.MAINID)
                //            .Where((SL, OM, OO) => OM.PREWO == data
                //            && (SL.LOCK_STATION == station || SL.LOCK_STATION == "ALL")
                //            && SL.LOCK_STATUS == "1"
                //            && SL.TYPE == "PN"
                //            ).Distinct().Select((SL, OM, OO) => new
                //            {
                //                LOCK_REASON = SL.LOCK_REASON,
                //                LOCK_EMP = SL.LOCK_EMP,
                //                MaterialNo = SL.WORKORDERNO
                //            }).ToList();
                //        for (int j = 0; j < PnLock.Count(); j++)
                //        {
                //            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { PnLock[j].MaterialNo, PnLock[j].LOCK_EMP, PnLock[j].LOCK_REASON }));
                //        }
                //    }
                //    break;
                #endregion
                case "PN":
                    //检查物料被鎖
                    var PnLock = _db.Queryable<R_SN_LOCK>()
                        .Where(t =>
                        (t.LOCK_STATION == station || t.LOCK_STATION == "ALL")
                        && t.LOCK_STATUS == "1"
                        && t.TYPE == "PN")
                        .Select(t => new
                        {
                            LOCK_REASON = t.LOCK_REASON,
                            LOCK_EMP = t.LOCK_EMP,
                            MaterialNo = t.WORKORDERNO
                        })
                        .Distinct()
                        .ToList();
                    break;
                default:
                    break;
            }
            return locks;
        }
        public void GetBypassInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var type = Data["Type"].ToString();
            var data = Data["Data"].ToString();
            var status = Data["Status"].ToString();
            OleExec db = null;
            
            List<R_LOCK_BYPASS> passList = new List<R_LOCK_BYPASS>();
            try
            {
                db = this.DBPools["SFCDB"].Borrow();
                var query =db.ORM.Queryable<R_LOCK_BYPASS>();
                T_R_LOCK_BYPASS trlb = new T_R_LOCK_BYPASS(db, DBTYPE);
                if (type=="ALL") {

                    passList.AddRange(trlb.GetPassList("ALL", "", "", "", status, db));
                }
                if (type== "ByWo") {
                  
                    passList.AddRange(trlb.GetPassList("","", "", Data["Data"].ToString(), status, db));
                }
                if (type == "BySn")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["Data"];
                    for (int i = 0;i<arraySN.Count;i++) {
                        var SN = arraySN[i].ToString();
                        passList.AddRange(trlb.GetPassList("", "", SN, "", status, db));
                    }
                    
                }
                if (type == "BYID")
                {
                    Newtonsoft.Json.Linq.JArray arrayID = (Newtonsoft.Json.Linq.JArray)Data["Data"];
                    for (int i = 0; i < arrayID.Count; i++)
                    {
                        var id = arrayID[i].ToString();
                        passList.AddRange(trlb.GetPassList("",id, "", "", status, db));
                    }
                    
                }
                StationReturn.Data = passList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(db);
            }
        }
        public void DoPass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string passType = Data["PassType"].ToString().Trim();
            string passData = Data["PassData"].ToString();
            string passReason = Data["PassReason"].ToString().Trim().ToUpper();
            OleExec db = null;
            T_R_LOCK_BYPASS trlb = null;
            try
            {
                db = this.DBPools["SFCDB"].Borrow();
                trlb = new T_R_LOCK_BYPASS(db, DBTYPE);
                if (passType == "PassByWo")
                {
                    if (trlb.IsPass("WO","", passData, db))
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20220318153506";
                        StationReturn.MessagePara.Add(passData);
                        StationReturn.Data = "";
                        return;
                    }
                     R_LOCK_BYPASS item = new R_LOCK_BYPASS()
                    {
                        ID = MesDbBase.GetNewID(db.ORM, this.BU, "R_LOCK_BYPASS"),
                        TYPE = "WO",
                        KEY1 = "WO",
                        VALUE1 = passData,
                        BYPASS_EMP = LoginUser.EMP_NO,
                        REASON = passReason,
                        BYPASS_TIME = DateTime.Now,
                        EDIT_EMP = LoginUser.EMP_NO,
                        EDIT_TIME = DateTime.Now,
                        BYPASS_STATUS = 1
                    };
                    db.ORM.Insertable(item).ExecuteCommand();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";

                }
              else  if (passType == "PassBySn")
                {
                    Newtonsoft.Json.Linq.JArray arrays = (Newtonsoft.Json.Linq.JArray)Data["PassData"];
                    for (int i = 0; i < arrays.Count; i++)
                    {
                        if (trlb.IsPass("SN",arrays[i].ToString(), "", db))
                        {
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MSGCODE20220318153506";
                            StationReturn.MessagePara.Add(passData);
                            StationReturn.Data = "";
                            return;
                        }
                        R_LOCK_BYPASS item = new R_LOCK_BYPASS()
                        {
                            ID = MesDbBase.GetNewID(db.ORM, this.BU, "R_LOCK_BYPASS"),
                            TYPE = "SN",
                            KEY1 = "SN",
                            VALUE1 = arrays[i].ToString().Trim().ToUpper(),
                            BYPASS_EMP = LoginUser.EMP_NO,
                            REASON = passReason,
                            BYPASS_TIME = DateTime.Now,
                            EDIT_EMP = LoginUser.EMP_NO,
                            EDIT_TIME = DateTime.Now,
                            BYPASS_STATUS = 1
                        };
                        db.ORM.Insertable(item).ExecuteCommand();
                    }

                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Data = "";
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { "PassType" }));
                }
                this.DBPools["SFCDB"].Return(db);
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (db != null)
                {
                    this.DBPools["SFCDB"].Return(db);
                }
            }
            finally
            {
                if (db != null)
                {
                    this.DBPools["SFCDB"].Return(db);
                }
            }
        }
        public void ByPassSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SN = Data["SN"].ToString();
            var REASON = Data["REASON"].ToString();
            var db = this.DBPools["SFCDB"].Borrow();
            try
            {
                var ret = ByPassSNRun(SN, REASON, db);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = ret;
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(db);
            }
        }

        public  List<R_LOCK_BYPASS> ByPassSNRun(string SN,string REASON ,OleExec db)
        {
            List<R_LOCK_BYPASS> ret = new List<R_LOCK_BYPASS>();
            var sns = SN.Split(new string[] { ",", "\r\n" },StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sns.Count(); i++)
            {
                R_LOCK_BYPASS item = new R_LOCK_BYPASS()
                {
                    ID = MesDbBase.GetNewID(db.ORM, this.BU, "R_LOCK_BYPASS"),
                    TYPE = "SN",
                    KEY1 = "SN",
                    VALUE1 = sns[i].Trim().ToUpper(),
                    BYPASS_EMP = LoginUser.EMP_NO,
                    REASON = REASON,
                    BYPASS_TIME = DateTime.Now,
                    EDIT_EMP = LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now,
                    BYPASS_STATUS = 1
                };
                db.ORM.Insertable(item).ExecuteCommand();
                ret.Add(item);
            }
            return ret;
        }
        public void ByPassWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var WO = Data["WO"].ToString();
            var REASON = Data["REASON"].ToString();
            var db = this.DBPools["SFCDB"].Borrow();
            try
            {
                var ret = ByPassWORun(WO, REASON, db);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = ret;
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(db);
            }
        }

        public List<R_LOCK_BYPASS> ByPassWORun(string WO, string REASON, OleExec db)
        {
            List<R_LOCK_BYPASS> ret = new List<R_LOCK_BYPASS>();
            var wos = WO.Split(new string[] { ",", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < wos.Count(); i++)
            {
                R_LOCK_BYPASS item = new R_LOCK_BYPASS()
                {
                    ID = MesDbBase.GetNewID(db.ORM, this.BU, "R_LOCK_BYPASS"),
                    TYPE = "WO",
                    KEY1 = "WO",
                    VALUE1 = wos[i].Trim().ToUpper(),
                    BYPASS_EMP = LoginUser.EMP_NO,
                    REASON = REASON,
                    BYPASS_TIME = DateTime.Now,
                    EDIT_EMP = LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now,
                    BYPASS_STATUS = 1
                };
                db.ORM.Insertable(item).ExecuteCommand();
                ret.Add(item);
            }
            return ret;
        }

        public void GetByPassData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SN = Data["SN"].ToString();
            var WO = Data["WO"].ToString();
            var db = this.DBPools["SFCDB"].Borrow();
            try
            {
                var ret = GetByPassDataRun(SN,WO, db);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = ret;
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(db);
            }
        }
        public List<R_LOCK_BYPASS> GetByPassDataRun(string SN, string WO,  OleExec db)
        {
            var qurey = db.ORM.Queryable<R_LOCK_BYPASS>();
            if (SN != null && SN.Trim() != "")
            {
                qurey = qurey.Where(t => t.TYPE == "SN" && t.VALUE1.Contains(SN));
            }
            if (WO != null && WO.Trim() != "")
            {
                qurey = qurey.Where(t => t.TYPE == "WO" && t.VALUE1.Contains(WO));
            }
            var ret = qurey.ToList();
            return ret;
        }

        public void UNDOByPass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            var db = this.DBPools["SFCDB"].Borrow();
            try
            {
                var ret = UNDOByPassRun(ID,  db);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = ret;
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(db);
            }
        }
        
        public List<R_LOCK_BYPASS> UNDOByPassRun(Newtonsoft.Json.Linq.JArray ID, OleExec db)
        {
            List<R_LOCK_BYPASS> ret = new List<R_LOCK_BYPASS>();
            for (int i = 0; i < ID.Count; i++) {
                var uID = ID[i].ToString();
                var unPassItem = db.ORM.Queryable<R_LOCK_BYPASS>().Where(t => t.ID == uID).First();
                if (unPassItem != null)
                {
                    unPassItem.BYPASS_STATUS = 0;
                    unPassItem.EDIT_EMP = LoginUser.EMP_NO;
                    unPassItem.EDIT_TIME = DateTime.Now;
                    db.ORM.Updateable(unPassItem).Where(t => t.ID == unPassItem.ID).ExecuteCommand();
                }
                ret.Add(unPassItem);
            }  
            return ret;
        }


    }
}
