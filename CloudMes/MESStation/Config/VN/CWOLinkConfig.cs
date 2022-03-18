
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config.VN
{
    public class CWOLinkConfig: MesAPIBase
    {
        protected APIInfo FGetMappingList = new APIInfo()
        {
            FunctionName = "GetMappingList",
            Description = "Get wo link setting",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddNewMapping = new APIInfo()
        {
            FunctionName = "AddNewMapping",
            Description = "add a new wo link mapping",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LINKWO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FModifyMapping = new APIInfo()
        {
            FunctionName = "ModifyMapping",
            Description = "Modify wo link mapping by skuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LINKWO", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDeleteMapping = new APIInfo()
        {
            FunctionName = "DeleteMapping",
            Description = "delete wo link mapping list by id",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        public CWOLinkConfig()
        {
            this.Apis.Add(FGetMappingList.FunctionName, FGetMappingList);
            this.Apis.Add(FAddNewMapping.FunctionName, FAddNewMapping);
            this.Apis.Add(FModifyMapping.FunctionName, FModifyMapping);
            this.Apis.Add(FDeleteMapping.FunctionName, FDeleteMapping);
        }

        public void GetMappingList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_LINK rwl = null;
            OleExec sfcdb = null;
            var wo = Data["WO"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                rwl = new T_R_WO_LINK(sfcdb, DB_TYPE_ENUM.Oracle);

                List<R_WO_LINK> woLinksList = new List<R_WO_LINK>();
                if (string.IsNullOrEmpty(wo))
                {
                    woLinksList = sfcdb.ORM.Queryable<R_WO_LINK>().OrderBy(t => t.WO).OrderBy(t => t.CREATETIME).ToList();            
                }
                else
                {
                    woLinksList = sfcdb.ORM.Queryable<R_WO_LINK>().Where(t => t.WO == wo).OrderBy(t => t.WO).OrderBy(t => t.CREATETIME).ToList();
                }
                if (woLinksList.Count > 0)
                {
                    StationReturn.MessageCode = "MSGCODE20210814161629！！";
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
            T_R_WO_LINK TRWL = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                bool bol=false;
                string wo = Data["WO"].ToString().Trim().ToUpper();
                string wolink = Data["LINKWO"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                TRWL = new T_R_WO_LINK (sfcdb, DB_TYPE_ENUM.Oracle);
                bol = sfcdb.ORM.Queryable<R_WO_LINK>().Any(t => t.WO == wo&& t.LINKWO == wolink);
                if (bol)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { wo+"---"+wolink }));
                }
                Row_R_WO_LINK rowLink = (Row_R_WO_LINK)TRWL.NewRow();
                rowLink.ID = TRWL.GetNewID(this.BU, sfcdb);
                rowLink.LINKTYPE = "SMT_SI";
                rowLink.WO = wo;
                rowLink.LINKWO = wolink;
                rowLink.CREATEBY = this.LoginUser.EMP_NO;
                rowLink.CREATETIME = GetDBDateTime();
                result = sfcdb.ExecSQL(rowLink.GetInsertString(this.DBTYPE));

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
            T_R_WO_LINK tRwl = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                string wo = Data["WO"].ToString().Trim().ToUpper();
                string linkwo = Data["LINKWO"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                tRwl = new T_R_WO_LINK(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_WO_LINK rowLink = (Row_R_WO_LINK)tRwl.GetObjByID(id, sfcdb);

                sfcdb.ORM.Updateable<R_WO_LINK>();
                rowLink.WO = wo;
                rowLink.LINKWO = linkwo;
                rowLink.LINKTYPE = "SMT_SI";
                rowLink.CREATEBY = this.LoginUser.EMP_NO;
                rowLink.CREATETIME = GetDBDateTime();
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
            T_R_WO_LINK tRwl = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                tRwl = new T_R_WO_LINK(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_WO_LINK rowMapping = (Row_R_WO_LINK)tRwl.GetObjByID(id, sfcdb);
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
