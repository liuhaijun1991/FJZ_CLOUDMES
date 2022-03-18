using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class ServiceMCSNConfig : MesAPIBase
    {
        protected APIInfo FGetMappingList = new APIInfo()
        {
            FunctionName = "GetMappingList",
            Description = "get mcsn setting",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddNewMapping = new APIInfo()
        {
            FunctionName = "AddNewMapping",
            Description = "add a new service",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VVALUE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "INPUT", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FModifyMapping = new APIInfo()
        {
            FunctionName = "ModifyMapping",
            Description = "Modify Service ",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VVALUE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "INPUT", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteMapping = new APIInfo()
        {
            FunctionName = "DeleteMapping",
            Description = "delete service",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        public ServiceMCSNConfig()
        {
            this.Apis.Add(FGetMappingList.FunctionName, FGetMappingList);
            this.Apis.Add(FAddNewMapping.FunctionName, FAddNewMapping);
            this.Apis.Add(FModifyMapping.FunctionName, FModifyMapping);
            this.Apis.Add(FDeleteMapping.FunctionName, FDeleteMapping);
        }
        public void GetMappingList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_MCSN rwl = null;
            OleExec sfcdb = null;
            var val = Data["VAL"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                rwl = new T_C_MCSN(sfcdb, DB_TYPE_ENUM.Oracle);

                List<C_MCSN> woLinksList = new List<C_MCSN>();
                if (string.IsNullOrEmpty(val))
                {
                    woLinksList = sfcdb.ORM.Queryable<C_MCSN>().OrderBy(t => t.SKUNO).OrderBy(t => t.EDIT_TIME).ToList();
                }
                else
                {
                    woLinksList = sfcdb.ORM.Queryable<C_MCSN>().Where(t => t.SKUNO == val || t.SN == val).OrderBy(t => t.EDIT_TIME).ToList();
                }
                if (woLinksList.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = woLinksList;
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

        public void AddNewMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_MCSN TRWL = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                bool bol = false;
                string type = Data["TYPE"].ToString().Trim().ToUpper();
                string val = Data["VVALUE"].ToString().Trim().ToUpper();
                string input = Data["INPUT"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                TRWL = new T_C_MCSN(sfcdb, DB_TYPE_ENUM.Oracle);
                if (type == "SERVICE_SN" && type.Length > 0)
                {
                    bol = sfcdb.ORM.Queryable<C_MCSN>().Any(t => t.SN == val && t.SERVICE == input);
                }
                else if (type == "TAG_CODE" && type.Length > 0)
                {
                    bol = sfcdb.ORM.Queryable<C_MCSN>().Any(t => t.SKUNO == val && t.SERVICE == input);
                }

                if (bol)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { val + "---" + input }));
                }
                Row_C_MCSN rowLink = (Row_C_MCSN)TRWL.NewRow();
                rowLink.ID = TRWL.GetNewID(this.BU, sfcdb);
                if (type == "SERVICE_SN")
                {
                    rowLink.TYPE = type;
                    rowLink.SN = val;
                    rowLink.SERVICE = input;
                    rowLink.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowLink.EDIT_TIME = GetDBDateTime();
                    result = sfcdb.ExecSQL(rowLink.GetInsertString(this.DBTYPE));
                }
                else
                {
                    rowLink.TYPE = type;
                    rowLink.SKUNO = val;
                    rowLink.SERVICE = input;
                    rowLink.EDIT_EMP = this.LoginUser.EMP_NO;
                    rowLink.EDIT_TIME = GetDBDateTime();
                    result = sfcdb.ExecSQL(rowLink.GetInsertString(this.DBTYPE));
                }
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

        public void ModifyMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_MCSN tRwl = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                string input = Data["INPUT"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                tRwl = new T_C_MCSN(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_MCSN rowLink = (Row_C_MCSN)tRwl.GetObjByID(id, sfcdb);

                sfcdb.ORM.Updateable<C_MCSN>();
                rowLink.SERVICE = input;
                rowLink.EDIT_EMP = this.LoginUser.EMP_NO;
                rowLink.EDIT_TIME = GetDBDateTime();
                result = sfcdb.ExecSQL(rowLink.GetUpdateString(this.DBTYPE));

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

        public void DeleteMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_MCSN tRwl = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                tRwl = new T_C_MCSN(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_MCSN rowMapping = (Row_C_MCSN)tRwl.GetObjByID(id, sfcdb);
                result = sfcdb.ExecSQL(rowMapping.GetDeleteString(this.DBTYPE));

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
