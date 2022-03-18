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
    public class LinkControlSetting:MesAPIBase
    {
        protected APIInfo FAddNewControl = new APIInfo
        {
            FunctionName = "AddNewControl",
            Description = "add new lint control",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "MAIN_ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SUB_ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteLinkControl = new APIInfo
        {
            FunctionName = "DeleteLinkControl",
            Description = "delete one or more lint control",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateLinkControl = new APIInfo
        {
            FunctionName = "UpdateLinkControl",
            Description = "update one lint control",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetLinkControlList = new APIInfo
        {
            FunctionName = "GetLinkControlList",
            Description = "Get link control list by type and main_item or sub_itme",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "MAIN_ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SUB_ITEM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public LinkControlSetting()
        {
            this.Apis.Add(FAddNewControl.FunctionName, FAddNewControl);
            this.Apis.Add(FDeleteLinkControl.FunctionName, FDeleteLinkControl);
            this.Apis.Add(FUpdateLinkControl.FunctionName, FUpdateLinkControl);
            this.Apis.Add(FGetLinkControlList.FunctionName, FGetLinkControlList);
        }

        public void AddNewControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_LINK_CONTROL t_r_link_control = null;
            OleExec sfcdb = null;
            try
            {
                string result;
                string type = Data["TYPE"].ToString().Trim().ToUpper();
                string mainItem = Data["MAIN_ITEM"].ToString().Trim().ToUpper();
                string mainRev = Data["MAIN_REV"].ToString().Trim().ToUpper();
                string subItem = Data["SUB_ITEM"].ToString().Trim().ToUpper();
                string subRev = Data["SUB_REV"].ToString().Trim().ToUpper();
                string category = Data["CATEGORY"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                t_r_link_control = new T_R_LINK_CONTROL(sfcdb, DBTYPE);
                if (t_r_link_control.IsLinkControl(type, mainItem, subItem, category, sfcdb))//增加category判斷數據唯一性
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { type + "," + mainItem + "," + subItem }));
                }
                Row_R_LINK_CONTROL rowControl = (Row_R_LINK_CONTROL)t_r_link_control.NewRow();
                rowControl.ID = t_r_link_control.GetNewID(this.BU, sfcdb);
                rowControl.CONTROL_TYPE = type;
                rowControl.MAIN_ITEM = mainItem;
                rowControl.MAIN_REV = mainRev;
                rowControl.SUB_ITEM = subItem;
                rowControl.SUB_REV = subRev;
                rowControl.CATEGORY = category;
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

        public void DeleteLinkControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_LINK_CONTROL t_r_link_control = null;
            Row_R_LINK_CONTROL rowControl = null;
            OleExec sfcdb = null;
            try
            {                
                Newtonsoft.Json.Linq.JArray idArray = (Newtonsoft.Json.Linq.JArray)Data["ID"];
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                t_r_link_control = new T_R_LINK_CONTROL(sfcdb, DBTYPE);
                if (idArray.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ID");
                    StationReturn.Data = "";
                    return;
                }

                for (int i = 0; i < idArray.Count; i++)
                {
                    rowControl = (Row_R_LINK_CONTROL)t_r_link_control.GetObjByID(idArray[i].ToString(), sfcdb, DBTYPE);
                    sfcdb.ExecSQL(rowControl.GetDeleteString(DBTYPE));
                }   
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "MES00000004";
                StationReturn.Data = "";
                this.DBPools["SFCDB"].Return(sfcdb); 
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000023";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void UpdateLinkControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_LINK_CONTROL t_r_link_control = null;
            Row_R_LINK_CONTROL rowControl = null;
            OleExec sfcdb = null;
            string result;
            try
            {                
                string id = Data["ID"].ToString().Trim().ToUpper();
                string mainItem = Data["MAIN_ITEM"].ToString().Trim().ToUpper();
                string mainRev = Data["MAIN_REV"].ToString().Trim().ToUpper();
                string subItem = Data["SUB_ITEM"].ToString().Trim().ToUpper();
                string subRev = Data["SUB_REV"].ToString().Trim().ToUpper();
                string category = Data["CATEGORY"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                t_r_link_control = new T_R_LINK_CONTROL(sfcdb, DBTYPE);
                rowControl = (Row_R_LINK_CONTROL)t_r_link_control.GetObjByID(id, sfcdb, DBTYPE);
                var cObj = sfcdb.ORM.Queryable<R_LINK_CONTROL>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(category), r => r.CATEGORY == category)
                    .Where(r => r.CONTROL_TYPE == rowControl.CONTROL_TYPE && r.MAIN_ITEM == mainItem 
                        && r.SUB_ITEM == subItem && r.ID!=rowControl.ID)
                        .ToList().FirstOrDefault();
                if (cObj!=null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000008", new string[] { mainItem + "," + subItem + "," + category }));
                }
                rowControl.MAIN_REV = mainRev;
                rowControl.SUB_ITEM = subItem;
                rowControl.SUB_REV = subRev;
                rowControl.CATEGORY = category;
                rowControl.EDITBY = this.LoginUser.EMP_NO;
                rowControl.EDITTIME = GetDBDateTime();
                result = sfcdb.ExecSQL(rowControl.GetUpdateString(DBTYPE));
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
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000025";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void GetLinkControlList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_LINK_CONTROL t_r_link_control = null;            
            OleExec sfcdb = null;            
            try
            {
                string type = Data["TYPE"].ToString().Trim().ToUpper();
                string item = Data["SEARCH_DATA"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                t_r_link_control = new T_R_LINK_CONTROL(sfcdb, this.DBTYPE);
                List<R_LINK_CONTROL> list = t_r_link_control.GetControlList(type, item, sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
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

      
    }
}
