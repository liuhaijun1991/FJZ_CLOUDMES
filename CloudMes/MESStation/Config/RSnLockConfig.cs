using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    class RSnLockConfig : MesAPIBase
    {
        protected APIInfo FAddRSnLock = new APIInfo()
        {
            FunctionName = "AddRSnLock",
            Description = "添加RSnLock",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LOCK_LOT", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCK_STATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCK_STATUS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCK_REASON", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectRSnLock = new APIInfo()
        {
            FunctionName = "SelectRSnLock",
            Description = "查询RSnLock",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LOCK_LOT", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WORKORDERNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteRSnLock = new APIInfo()
        {
            FunctionName = "DeleteRSnLock",
            Description = "删除RSnLock",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public RSnLockConfig()
        {
            this.Apis.Add(FAddRSnLock.FunctionName, FAddRSnLock);
            this.Apis.Add(FDeleteRSnLock.FunctionName, FDeleteRSnLock);
            this.Apis.Add(FSelectRSnLock.FunctionName, FSelectRSnLock);
        }

        public void AddRSnLock(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SN_LOCK LOCK = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                LOCK = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_SN_LOCK r = (Row_R_SN_LOCK)LOCK.NewRow();
                LogicObject.LotNo lot = new LogicObject.LotNo();
                r.ID = LOCK.GetNewID(this.BU, sfcdb);
                r.LOCK_LOT = lot.GetNewLotNo("LOCKLOT",sfcdb);
                r.SN = (Data["SN"].ToString()).Trim();
                r.TYPE = (Data["TYPE"].ToString()).Trim();
                r.WORKORDERNO = (Data["WORKORDERNO"].ToString()).Trim();
                r.LOCK_STATION = (Data["LOCK_STATION"].ToString()).Trim();
                r.LOCK_STATUS = (Data["LOCK_STATUS"].ToString()).Trim();
                r.LOCK_REASON = (Data["LOCK_REASON"].ToString()).Trim();
                r.LOCK_TIME = GetDBDateTime();
                r.LOCK_EMP = this.LoginUser.EMP_NO;
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "添加成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteRSnLock(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SN_LOCK LOCK = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                LOCK = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_SN_LOCK r = (Row_R_SN_LOCK)LOCK.GetObjByID((Data["ID"].ToString()).Trim() ,sfcdb);
                string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "删除成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void SelectRSnLock(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_SN_LOCK LOCK = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                LOCK = new T_R_SN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle);
                List<R_SN_LOCK> list = LOCK.GetLockList((Data["LOCK_LOT"].ToString()).Trim(), (Data["SN"].ToString()).Trim(), (Data["WORKORDERNO"].ToString()).Trim(),sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
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
