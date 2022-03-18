using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.VN
{
    public class CRepairControlConfig : MesAPIBase
    {
        protected APIInfo FGetControlList = new APIInfo()
        {
            FunctionName = "GetControlList",
            Description = "Get Repair Control Setting List",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddNewControl = new APIInfo()
        {
            FunctionName = "AddNewControl",
            Description = "add a new repair control",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NUM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REASON", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FModifyControl = new APIInfo()
        {
            FunctionName = "ModifyControl",
            Description = "Modify repair control by sn and station",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NUM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REASON", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteControl = new APIInfo()
        {
            FunctionName = "DeleteControl",
            Description = "delete repair control list by id",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        public CRepairControlConfig()
        {
            this.Apis.Add(FGetControlList.FunctionName, FGetControlList);
            this.Apis.Add(FAddNewControl.FunctionName, FAddNewControl);
            this.Apis.Add(FModifyControl.FunctionName, FModifyControl);
            this.Apis.Add(FDeleteControl.FunctionName, FDeleteControl);
        }

        public void GetControlList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            var sn = Data["SN"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                
                List<C_REPAIR_SN_CONTROL> controlList = new List<C_REPAIR_SN_CONTROL>();
                if (string.IsNullOrEmpty(sn))
                {
                    controlList = sfcdb.ORM.Queryable<C_REPAIR_SN_CONTROL>().OrderBy(t => t.SN).OrderBy(t => t.EDITTIME).ToList();
                }
                else
                {
                    controlList = sfcdb.ORM.Queryable<C_REPAIR_SN_CONTROL>().Where(t => t.SN == sn).OrderBy(t => t.SN).OrderBy(t => t.EDITTIME).ToList();
                }
                if (controlList.Count > 0)
                {
                    StationReturn.MessageCode = "MSGCODE20210814161629！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = controlList;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void AddNewControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_REPAIR_SN_CONTROL TRWL = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                bool bol = false;
                string sn = Data["SN"].ToString().Trim().ToUpper();
                string station = Data["STATION"].ToString().Trim().ToUpper();
                double num = double.Parse(Data["NUM"].ToString());
                string reason = Data["REASON"].ToString().Trim().ToUpper();

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                TRWL = new T_C_REPAIR_SN_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                bol = sfcdb.ORM.Queryable<C_REPAIR_SN_CONTROL>().Any(t => t.SN == sn && t.STATION == station);
                if (bol)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { sn + "---" + station }));
                }
                bol = sfcdb.ORM.Queryable<R_SN>().Any(t => t.SN == sn && t.VALID_FLAG == "1");
                if (!bol)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { sn }));
                }
                Row_C_REPAIR_SN_CONTROL rowControl = (Row_C_REPAIR_SN_CONTROL)TRWL.NewRow();
                rowControl.ID = TRWL.GetNewID(this.BU, sfcdb);
                rowControl.SN = sn;
                rowControl.STATION = station;
                rowControl.REPAIRCOUNT = num;
                rowControl.REASON = reason;
                rowControl.EDITBY = this.LoginUser.EMP_NO;
                rowControl.EDITTIME = GetDBDateTime();
                result = sfcdb.ExecSQL(rowControl.GetInsertString(this.DBTYPE));

                if (Convert.ToInt32(result) > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.MessageCode = "MES00000021";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000021";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void ModifyControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_REPAIR_SN_CONTROL tRwl = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                double num = double.Parse(Data["NUM"].ToString());
                string reason = Data["REASON"].ToString().Trim().ToUpper();

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                tRwl = new T_C_REPAIR_SN_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_REPAIR_SN_CONTROL rowControl = (Row_C_REPAIR_SN_CONTROL)tRwl.GetObjByID(id, sfcdb);

                sfcdb.ORM.Updateable<C_REPAIR_SN_CONTROL>();
                rowControl.REPAIRCOUNT = num;
                rowControl.REASON = reason;
                rowControl.EDITBY = this.LoginUser.EMP_NO;
                rowControl.EDITTIME = GetDBDateTime();
                result = sfcdb.ExecSQL(rowControl.GetUpdateString(this.DBTYPE));

                if (Convert.ToInt32(result) > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(Convert.ToInt32(result));
                    StationReturn.MessageCode = "MES00000035";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.MessageCode = "MES00000025";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Message = e.Message;
                StationReturn.MessageCode = "MES00000025";
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
        }

        public void DeleteControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_REPAIR_SN_CONTROL tRwl = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                tRwl = new T_C_REPAIR_SN_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_REPAIR_SN_CONTROL rowControl = (Row_C_REPAIR_SN_CONTROL)tRwl.GetObjByID(id, sfcdb);
                result = sfcdb.ExecSQL(rowControl.GetDeleteString(this.DBTYPE));

                if (Convert.ToInt32(result) > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.MessageCode = "MES00000004";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Message = "";
                    StationReturn.MessagePara.Add(result);
                    StationReturn.MessageCode = "MES00000023";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000023";
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
        }
    }
}
