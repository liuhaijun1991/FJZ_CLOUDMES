using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation.MESReturnView.Public;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject;
using System.Data;

namespace MESStation.GlobalConfig
{
    class SystemMenuConfig : MesAPIBase
    {
        protected APIInfo FGetMenuInformation = new APIInfo()
        {
            FunctionName = "GetMenuInformation",
            Description = "或取单条菜单信息",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "MenuId", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetMenu = new APIInfo()
        {
            FunctionName = "GetMenu",
            Description = "查詢用戶菜單",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FCreatMenu = new APIInfo()
        {
            FunctionName = "CreatMenu",
            Description = "添加菜單！",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "MENU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PAGE_PATH", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARENT_CODE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STYLE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLASS_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LANGUAGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MENU_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" },

            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeletetMenu = new APIInfo()
        {
            FunctionName = "DeletetMenu",
            Description = "刪除菜單！",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FUpdateMenu = new APIInfo()
        {
            FunctionName = "UpdateMenu",
            Description = "编辑菜單！",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MENU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PAGE_PATH", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARENT_CODE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STYLE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLASS_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LANGUAGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MENU_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" },

            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FOrderbyMenu = new APIInfo()
        {
            FunctionName = "OrderbyMenu",
            Description = "排序菜單！",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARENTID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MENUIDS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        public SystemMenuConfig()
        {
            this.Apis.Add(FGetMenuInformation.FunctionName, FGetMenuInformation);
            this.Apis.Add(FGetMenu.FunctionName, FGetMenu);
            this.Apis.Add(FCreatMenu.FunctionName, FCreatMenu);
            this.Apis.Add(FDeletetMenu.FunctionName, FDeletetMenu);
            this.Apis.Add(FUpdateMenu.FunctionName, FUpdateMenu);
            this.Apis.Add(FOrderbyMenu.FunctionName, FOrderbyMenu);
        }

        public void GetMenuInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string m_id = Data["MenuId"].ToString();
            try
            {
                MESDataObject.Module.T_C_MENU tcm = new MESDataObject.Module.T_C_MENU(SFCDB, this.DBTYPE);
                MESDataObject.Module.MenuInformation rcm = new MESDataObject.Module.MenuInformation();
                rcm = tcm.getC_Menu(m_id,SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110208"); //"获取菜單成功！！！";
                StationReturn.Data = rcm; 
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110253"); // "获取菜單失敗！";
            }
        }

        public void GetMenu(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string User_Name = Data["EMP_NO"].ToString();
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            List<MESDataObject.Module.MENUS> pi = new List<MESDataObject.Module.MENUS>();
            try
            {
                MESDataObject.Module.T_C_MENU tcm = new MESDataObject.Module.T_C_MENU(SFCDB, this.DBTYPE);
                pi = tcm.GetMenu(this.LoginUser.EMP_LEVEL, User_Name, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110208"); 
                StationReturn.Data = pi;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110253"); // "获取菜單失敗！";
                StationReturn.Data = ex.Message.ToString();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void CreatMenu(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            MESDataObject.Module.T_C_MENU tcm = new MESDataObject.Module.T_C_MENU(SFCDB, this.DBTYPE);
            MESDataObject.Module.Row_C_MENU rcm = (MESDataObject.Module.Row_C_MENU)tcm.NewRow();
            string m_name = Data["MENU_NAME"].ToString();
            string m_desc = Data["MENU_DESC"].ToString();
            string m_emp = Data["EDIT_EMP"].ToString();
            string PARENT_CODE = Data["PARENT_CODE"].ToString();
            string UpdateSQL = ""; 
            SFCDB.BeginTrain();
            try
            {
                if (tcm.Check_MENU_NAME(m_name, SFCDB))
                {
                    string m_id = tcm.GetNewID(BU, SFCDB);
                    Dictionary<string, string> id = new Dictionary<string, string>();
                    id.Add("ID", m_id);
                    rcm.ID = m_id;
                    rcm.SYSTEM_NAME = SystemName;
                    rcm.MENU_NAME = m_name;
                    rcm.PAGE_PATH = Data["PAGE_PATH"].ToString();
                    rcm.PARENT_CODE = PARENT_CODE;
                    rcm.SORT = tcm.GetmaxSORT(PARENT_CODE, SFCDB);
                    rcm.STYLE_NAME = Data["STYLE_NAME"].ToString();
                    rcm.CLASS_NAME = Data["CLASS_NAME"].ToString();
                    rcm.LANGUAGE_ID = Data["LANGUAGE_ID"].ToString();
                    rcm.MENU_DESC = m_desc;
                    rcm.EDIT_TIME = DateTime.Now;
                    rcm.EDIT_EMP = m_emp;
                    UpdateSQL += rcm.GetInsertString(this.DBTYPE) + ";\n";
                    MESDataObject.Module.T_C_PRIVILEGE tcp = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, this.DBTYPE);
                    MESDataObject.Module.Row_C_PRIVILEGE rcp = (MESDataObject.Module.Row_C_PRIVILEGE)tcp.NewRow();
                    rcp.ID = tcp.GetNewID(BU, SFCDB);
                    rcp.MENU_ID= m_id;
                    rcp.SYSTEM_NAME = SystemName;
                    rcp.PRIVILEGE_NAME = m_name;
                    rcp.PRIVILEGE_DESC = m_desc;
                    rcp.EDIT_TIME = DateTime.Now;
                    rcp.EDIT_EMP = m_emp;
                    rcp.BASECONFIG_FLAG = "Y";
                    UpdateSQL += rcp.GetInsertString(this.DBTYPE) + ";\n";
                    SFCDB.ExecSQL("Begin\n" + UpdateSQL + "End;");
                    SFCDB.CommitTrain();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110511");  //"添加菜單成功！！！";
                    StationReturn.Data = id;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000005"); // "要建立的菜單已經存在！！";
                }
                this.DBPools["SFCDB"].Return(SFCDB);

            }
            catch (Exception ex)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000021");  //"建立菜單失敗！";
                StationReturn.Data = ex.Message.ToString();
            }
        }

        public void DeletetMenu(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            MESDataObject.Module.T_C_MENU tcm = new MESDataObject.Module.T_C_MENU(SFCDB, this.DBTYPE);
            MESDataObject.Module.Row_C_MENU rcm = (MESDataObject.Module.Row_C_MENU)tcm.NewRow();
            string ID = Data["ID"].ToString();
            string deleteSQL = "";
            SFCDB.BeginTrain();
            try
            {
                if (tcm.Check_PARENT(ID, SFCDB))
                {
                    rcm = tcm.getC_MenubyID(ID, SFCDB);
                    deleteSQL += rcm.GetDeleteString(this.DBTYPE) + ";\n";
                    MESDataObject.Module.T_C_PRIVILEGE tcp = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, this.DBTYPE);
                    MESDataObject.Module.Row_C_PRIVILEGE rcp = (MESDataObject.Module.Row_C_PRIVILEGE)tcp.NewRow();
                    //rcp = tcp.getC_PrivilegebyMenuID(ID, SFCDB);
                    tcp.DeleteByMenuId(ID, SFCDB);
                    //deleteSQL += rcp.GetDeleteString(this.DBTYPE) + ";\n";
                    SFCDB.ExecSQL("Begin\n" + deleteSQL + "End;");
                    SFCDB.CommitTrain();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110919"); //"刪除菜單及其權限成功！！！";
                    StationReturn.Data = ID;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111007"); //"要刪除的菜單存在子菜單！！不可刪除！";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111135"); // "刪除菜單失敗！";
                StationReturn.Data = ex.Message.ToString();
            }
        }

        public void UpdateMenu(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            MESDataObject.Module.T_C_MENU tcm = new MESDataObject.Module.T_C_MENU(SFCDB, this.DBTYPE);
            MESDataObject.Module.Row_C_MENU rcm = (MESDataObject.Module.Row_C_MENU)tcm.NewRow();
            string UpdateSQL = "";
            SFCDB.BeginTrain();
            try
            {
                string m_name = Data["MENU_NAME"].ToString();
                string m_desc = Data["MENU_DESC"].ToString();
                string m_emp = Data["EDIT_EMP"].ToString();
                string m_id = Data["ID"].ToString();
                rcm = tcm.getC_MenubyID(m_id, SFCDB);
                rcm.MENU_NAME = m_name;
                rcm.PAGE_PATH = Data["PAGE_PATH"].ToString();
                rcm.STYLE_NAME = Data["STYLE_NAME"].ToString();
                rcm.CLASS_NAME = Data["CLASS_NAME"].ToString();
                rcm.LANGUAGE_ID = Data["LANGUAGE_ID"].ToString();
                rcm.MENU_DESC = m_desc;
                rcm.EDIT_TIME = DateTime.Now;
                rcm.EDIT_EMP = m_emp;
                UpdateSQL += rcm.GetUpdateString(this.DBTYPE) + ";\n";

                MESDataObject.Module.T_C_PRIVILEGE tcp = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, this.DBTYPE);
                MESDataObject.Module.C_PRIVILEGE rcp  = tcp.getC_PrivilegebyMenuID(m_id, SFCDB);
                tcp.Update(rcp, SFCDB);
                //rcp.PRIVILEGE_NAME = m_name;
                //rcp.PRIVILEGE_DESC = m_desc;
                //rcp.EDIT_TIME = DateTime.Now;
                //rcp.EDIT_EMP = m_emp;
                //UpdateSQL += rcp.GetUpdateString(this.DBTYPE) + ";\n";
                SFCDB.ExecSQL("Begin\n" + UpdateSQL + "End;");
                SFCDB.CommitTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111251");  //"编辑菜單成功！！";
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message =MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111325");  //编辑失敗
                StationReturn.Data = ex.Message.ToString();
            }
        }

        public void OrderbyMenu(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string PARENTID = Data["PARENTID"].ToString();
            int index = 10;
            string sql = "";
            SFCDB.BeginTrain();
            try
            {
                MESDataObject.Module.T_C_MENU tcm = new MESDataObject.Module.T_C_MENU(SFCDB, this.DBTYPE);
                MESDataObject.Module.Row_C_MENU rcm = (MESDataObject.Module.Row_C_MENU)tcm.NewRow();
                    foreach (string item in Data["MENUIDS"])
                {
                    rcm = tcm.getC_MenubyIDandPARENT(item.Trim('\'').Trim('\"'), PARENTID, SFCDB);
                    if (rcm != null)
                    { 
                        rcm.SORT = index;
                        sql += rcm.GetUpdateString(this.DBTYPE) + ";\n";
                        index += 10;
                    }
                }
                 SFCDB.ExecSQL("Begin\n" + sql + "End;");  
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111504");  //排序成功
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            { 

                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111625");  //排序失敗
                StationReturn.Data = ex.Message.ToString();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
