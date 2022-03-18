using MESDataObject;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Public;
using System;
using System.Collections.Generic;
using System.Data;


namespace MESStation.MESUserManager
{
    class UserPrivilege : MesAPIBase
    {
        static Random rand = new Random();
        ///List<APIInfo> TCodes = new List<APIInfo>();           

        protected APIInfo FCreatePrivilegeID = new APIInfo()
        {
            FunctionName = "CreatePrivilegeID",
            Description = "創建站位權限對用的ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SYSTEM_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectPrivilegeID = new APIInfo()
        {
            FunctionName = "SelectPrivilegeID",
            Description = "查詢站位權限對用的ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SYSTEM_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRIVILEGE_DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FLoadInformation = new APIInfo()
        {
            FunctionName = "LoadInformation",
            Description = "查詢站位權限對用的ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PageRow", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PageCount", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "emp_no", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectUserPrivilege = new APIInfo()
        {
            FunctionName = "SelectUserPrivilege",
            Description = "查詢用戶角色權限",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LoginUserEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EditEmp", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectEditPrivilege = new APIInfo()
        {
            FunctionName = "SelectEditPrivilege",
            Description = "查詢用戶角色權限",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LoginUserEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EditEmp", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };


        protected APIInfo FCreatMenuId = new APIInfo()
        {
            FunctionName = "CreatMenuId",
            Description = "創建系統菜單配置ID",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "MENU_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LANGUAGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PAGE_PATH", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PARENT_CODE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SORT", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STYLE_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CLASS_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LANGUAGE_ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MENU_DESC", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FCreatUserPrivilege = new APIInfo()
        {
            FunctionName = "CreatUserPrivilege",
            Description = "添加用戶權限！",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "GEMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SEMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ID_ITEMS", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeleteEditPrivilege = new APIInfo()
        {
            FunctionName = "DeleteEditPrivilege",
            Description = "添加用戶權限！",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LoginUserEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EditEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRS", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FPrivilegeForModelType = new APIInfo()
        {
            FunctionName = "PrivilegeForModelType",
            Description = "為用戶添加ModelType權限！",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LoginUserEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EditEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PRS", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSelectModelTypePrivilege = new APIInfo()
        {
            FunctionName = "SelectModelTypePrivilege",
            Description = "查詢該用戶已有的ModelType權限!",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LoginUserEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EditEmp", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FAddUserPrivilegeForModelType = new APIInfo()
        {
            FunctionName = "AddUserPrivilegeForModelType",
            Description = "為用戶添加ModelType權限!",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LoginUserEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EditEmpID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ID_ITEMS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDeleteModelUserPrivilege = new APIInfo()
        {
            FunctionName = "DeleteModelUserPrivilege",
            Description = "為用戶添加ModelType權限!",

            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "LoginUserEmp", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EditEmpID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ID_ITEMS", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        public UserPrivilege()
        {
            this.Apis.Add(FCreatePrivilegeID.FunctionName, FCreatePrivilegeID);
            this.Apis.Add(FSelectPrivilegeID.FunctionName, FSelectPrivilegeID);
            this.Apis.Add(FLoadInformation.FunctionName, FLoadInformation);
            this.Apis.Add(FSelectUserPrivilege.FunctionName, FSelectUserPrivilege);
            this.Apis.Add(FSelectEditPrivilege.FunctionName, FSelectEditPrivilege);
            this.Apis.Add(FDeleteEditPrivilege.FunctionName, FDeleteEditPrivilege);
            this.Apis.Add(FCreatUserPrivilege.FunctionName, FCreatUserPrivilege);
            this.Apis.Add(FPrivilegeForModelType.FunctionName, FPrivilegeForModelType);
            this.Apis.Add(FSelectModelTypePrivilege.FunctionName, FSelectModelTypePrivilege);
            this.Apis.Add(FAddUserPrivilegeForModelType.FunctionName, FAddUserPrivilegeForModelType);
            this.Apis.Add(FDeleteModelUserPrivilege.FunctionName, FDeleteModelUserPrivilege);
        }

        /// <summary>
        /// 創建權限對應的ID
        /// </summary>
        public void CreatePrivilegeID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_PRIVILEGE RolerPrivilege = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_PRIVILEGE RolerPrivilegeRow = (MESDataObject.Module.Row_C_PRIVILEGE)RolerPrivilege.NewRow();

            string PRIVILEGE_ID = Data["PRIVILEGE_ID"].ToString();
            string PRIVILEGE_NAME = Data["PRIVILEGE_NAME"].ToString();

            DataTable StrRes = new DataTable();
            StrRes = RolerPrivilege.CheckPrivilegeID(PRIVILEGE_ID, PRIVILEGE_NAME, SFCDB, this.DBTYPE);
            if (StrRes.Rows.Count != 0)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529134226");
            }
            else
            {
                RolerPrivilegeRow.ID = RolerPrivilege.GetNewID(BU, SFCDB);
                RolerPrivilegeRow.MENU_ID = Data["MENU_ID"].ToString();
                RolerPrivilegeRow.PRIVILEGE_NAME = Data["PRIVILEGE_NAME"].ToString();
                RolerPrivilegeRow.PRIVILEGE_DESC = Data["PRIVILEGE_DESC"].ToString();
                RolerPrivilegeRow.EDIT_TIME = DateTime.Now;
                RolerPrivilegeRow.EDIT_EMP = Data["EDIT_EMP"].ToString();

                string STRRES = SFCDB.ExecSQL(RolerPrivilegeRow.GetInsertString(this.DBTYPE));

                if (STRRES == "1")
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529134511");
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529134628");
                }

            }

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 查詢權限對應的ID
        /// </summary>
        public void SelectPrivilegeID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            LoginReturn lr = new LoginReturn();
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_C_PRIVILEGE RolerPrivilege = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, DB_TYPE_ENUM.Oracle);
            MESDataObject.Module.Row_C_PRIVILEGE RolerPrivilegeRow = (MESDataObject.Module.Row_C_PRIVILEGE)RolerPrivilege.NewRow();

            DataTable TablePrivilege = new DataTable();
            TablePrivilege = RolerPrivilege.SelectPrivilegeID(SFCDB, DB_TYPE_ENUM.Oracle);

            List<object> Privilegesid = new List<object>();
            if (TablePrivilege.Rows.Count > 0)
            {
                foreach (DataRow item in TablePrivilege.Rows)
                {
                    List<string> menu = new List<string>();

                    Privilegesid.Add(new
                    {
                        PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                        PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                        PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()
                    });
                }
            }
            else
            {
                Privilegesid.Add(null);
            }

            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529134817", new[] { "Privilege" });
            StationReturn.Data = Privilegesid;

            this.DBPools["SFCDB"].Return(SFCDB);
        }

        /// <summary>
        /// 查詢用戶角色權限
        /// </summary>
        public void SelectUserPrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string LoginUserEmp = Data["LoginUserEmp"].ToString().ToUpper();
            string EditEmp = Data["EditEmp"].ToString().ToUpper();
            List<MESDataObject.Module.PrivilegeEditModel> list = new List<MESDataObject.Module.PrivilegeEditModel>();
            try
            {
                MESDataObject.Module.T_C_PRIVILEGE tcp = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, this.DBTYPE);
                list = tcp.GetUserRolePrivilege(LoginUserEmp, EditEmp, this.LoginUser.EMP_LEVEL, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529134817", new[] { "Privilege" });
                StationReturn.Data = list;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000034");
                StationReturn.Data = ex.Message.ToString(); ;
            }
            finally
            {

                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void CreatUserPrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string GEMP_NO = Data["GEMP_NO"].ToString().ToUpper();
            string SEMP_NO = Data["SEMP_NO"].ToString().ToUpper();
            //string EMP_ID = "", P_code = "";
            Int32 Counter = 0;
            SFCDB.BeginTrain();
            try
            {
                //MESDataObject.Module.T_c_user GetInformation = new MESDataObject.Module.T_c_user(SFCDB, this.DBTYPE);
                MESDataObject.Module.T_C_USER_PRIVILEGE tcup = new MESDataObject.Module.T_C_USER_PRIVILEGE(SFCDB, this.DBTYPE);
                //MESDataObject.Module.Row_C_USER_PRIVILEGE rcup = (MESDataObject.Module.Row_C_USER_PRIVILEGE)tcup.NewRow();
                //MESDataObject.Module.T_C_MENU tcm = new MESDataObject.Module.T_C_MENU(SFCDB, this.DBTYPE);
                //MESDataObject.Module.Row_C_MENU rcm = (MESDataObject.Module.Row_C_MENU)tcm.NewRow();
                //DataTable dt = GetInformation.SelectC_Userbyempno(SEMP_NO, SFCDB, this.DBTYPE);
                //EMP_ID = dt.Rows[0]["ID"].ToString();
                //string insql = "";
                //if (dt.Rows.Count > 0)
                //{
                foreach (string item in Data["ID_ITEMS"])
                {
                    string p_id = item.Trim('\'').Trim('\"');
                    tcup.Add(SEMP_NO, null, p_id, LoginUser.BU, SystemName, LoginUser.EMP_NO, ref Counter, SFCDB);

                    //rcup.ID = tcup.GetNewID(BU, SFCDB);
                    //rcup.SYSTEM_NAME = SystemName;
                    //rcup.USER_ID = EMP_ID;
                    //rcup.PRIVILEGE_ID = p_id;
                    //rcup.EDIT_EMP = GEMP_NO;
                    //rcup.EDIT_TIME = DateTime.Now;
                    //insql += rcup.GetInsertString(this.DBTYPE) + ";\n";
                    //P_code += p_id + ",";
                    //do
                    //{
                    //    MESDataObject.Module.T_C_PRIVILEGE t_c_privilege = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, this.DBTYPE);
                    //    MESDataObject.Module.Row_C_PRIVILEGE rowPrivilege = t_c_privilege.getC_PrivilegebyID(p_id, SFCDB);
                    //    rcm = tcm.getC_MenubyID(rowPrivilege.MENU_ID, SFCDB);
                    //    //rcm = tcm.getC_MenubyID(p_id, SFCDB);
                    //    if (rcm.PARENT_CODE != "0")
                    //    {
                    //        p_id = rcm.PARENT_CODE;                                
                    //        if (P_code.IndexOf(rcm.PARENT_CODE) < 0 && tcup.getC_PrivilegebyIDemp(rcm.PARENT_CODE, SEMP_NO, SFCDB) == null)
                    //        {
                    //            rcup.ID = tcup.GetNewID(BU, SFCDB);
                    //            rcup.SYSTEM_NAME = SystemName;
                    //            rcup.USER_ID = EMP_ID;
                    //            rcup.PRIVILEGE_ID = rcm.PARENT_CODE;
                    //            rcup.EDIT_EMP = GEMP_NO;
                    //            rcup.EDIT_TIME = DateTime.Now;
                    //            insql += rcup.GetInsertString(this.DBTYPE) + ";\n";
                    //            P_code += rcm.PARENT_CODE + ",";
                    //        }
                    //    }
                    //} while (rcm.PARENT_CODE != "0");
                }
                //SFCDB.ExecSQL("Begin\n" + insql + "End;");
                if (Counter > 0)
                {
                    SFCDB.CommitTrain();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000002");
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529135502");
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000021");
                StationReturn.Data = ex.Message.ToString();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void SelectEditPrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string LoginUserEmp = Data["LoginUserEmp"].ToString().ToUpper();
            string EditEmp = Data["EditEmp"].ToString().ToUpper();
            List<MESDataObject.Module.PrivilegeEditModel> list = new List<MESDataObject.Module.PrivilegeEditModel>();
            try
            {
                MESDataObject.Module.T_C_PRIVILEGE tcp = new MESDataObject.Module.T_C_PRIVILEGE(SFCDB, this.DBTYPE);
                list = tcp.GetUserEditPrivilege(LoginUserEmp, EditEmp, this.LoginUser.EMP_LEVEL, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529134817");
                StationReturn.Data = list;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000034");
                StationReturn.Data = ex.Message.ToString(); ;
            }
            finally
            {

                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// 點擊新增時,查詢用戶沒有權限的ModelType:從登陸用戶有的權限列表中,只取受分配員工沒有權限的列表
        /// </summary>
        public void PrivilegeForModelType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string LoginUserEmp = Data["LoginUserEmp"].ToString().ToUpper();
            string EditEmp = Data["EditEmp"].ToString().ToUpper();
            List<MESDataObject.Module.ModelTypePrivilege> list = new List<MESDataObject.Module.ModelTypePrivilege>();
            try
            {
                MESDataObject.Module.T_C_MODEL_TYPE tCModelType = new MESDataObject.Module.T_C_MODEL_TYPE(SFCDB, this.DBTYPE);
                list = tCModelType.GetModelTypeForUser(LoginUserEmp, EditEmp, this.LoginUser.EMP_LEVEL, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529134817"); 
                StationReturn.Data = list;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000034");
                StationReturn.Data = ex.Message.ToString();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        /// <summary>
        /// 點擊刪除時,查詢該用戶已有的ModelType權限
        /// </summary>
        public void SelectModelTypePrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string LoginUserEmp = Data["LoginUserEmp"].ToString().ToUpper();
            string EditEmp = Data["EditEmp"].ToString().ToUpper();
            List<MESDataObject.Module.ModelTypePrivilege> list = new List<MESDataObject.Module.ModelTypePrivilege>();
            try
            {
                MESDataObject.Module.T_C_MODEL_TYPE tCModelType = new MESDataObject.Module.T_C_MODEL_TYPE(SFCDB, this.DBTYPE);
                list = tCModelType.GetPrivilegeForUser(LoginUserEmp, EditEmp, this.LoginUser.EMP_LEVEL, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20190529134817");
                StationReturn.Data = list;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000034");
                StationReturn.Data = ex.Message.ToString(); ;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// 點擊新增權限保存按鈕時,為用戶添加ModelType權限
        /// </summary>
        public void AddUserPrivilegeForModelType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string EditEmpID = Data["EditEmpID"].ToString().ToUpper();
            string LoginUserEmp = Data["LoginUserEmp"].ToString().ToUpper();
            string insql = "";
            SFCDB.BeginTrain();
            try
            {
                MESDataObject.Module.T_C_MODEL_USER tCModelUser = new MESDataObject.Module.T_C_MODEL_USER(SFCDB, this.DBTYPE);
                MESDataObject.Module.Row_C_MODEL_USER rCModelUser = (MESDataObject.Module.Row_C_MODEL_USER)tCModelUser.NewRow();
                foreach (string item in Data["ID_ITEMS"])
                {
                    //去除C_Model_Type.ID的前后引號
                    string strTypeID = item.Trim('\'').Trim('\"');
                    rCModelUser.TYPE_ID = strTypeID;
                    rCModelUser.USER_ID = EditEmpID;
                    rCModelUser.EDIT_EMP = LoginUserEmp;
                    rCModelUser.EDIT_TIME = DateTime.Now;
                    rCModelUser.ID = tCModelUser.GetNewID(BU, SFCDB);
                    insql += rCModelUser.GetInsertString(this.DBTYPE) + ";\n";
                }
                SFCDB.ExecSQL("Begin\n" + insql + "End;");
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000002");
                SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000021");
                StationReturn.Data = ex.Message.ToString();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        /// <summary>
        /// 點擊刪除權限保存按鈕時,為用戶刪除ModelType權限
        /// </summary>
        public void DeleteModelUserPrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string EditEmpID = Data["EditEmpID"].ToString().ToUpper();
            string LoginUserEmp = Data["LoginUserEmp"].ToString().ToUpper();
            string insql = "";
            SFCDB.BeginTrain();
            try
            {
                MESDataObject.Module.T_C_MODEL_USER tCModelUser = new MESDataObject.Module.T_C_MODEL_USER(SFCDB, this.DBTYPE);
                MESDataObject.Module.Row_C_MODEL_USER rCModelUser = (MESDataObject.Module.Row_C_MODEL_USER)tCModelUser.NewRow();
                foreach (string item in Data["ID_ITEMS"])
                {
                    //去除C_Model_Type.ID的前后引號
                    string strTypeID = item.Trim('\'').Trim('\"');
                    rCModelUser = (MESDataObject.Module.Row_C_MODEL_USER)tCModelUser.GetObjByID(strTypeID, SFCDB);
                    insql += rCModelUser.GetDeleteString(this.DBTYPE) + ";\n";
                }
                SFCDB.ExecSQL("Begin\n" + insql + "End;");
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000004");
                SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000023");
                StationReturn.Data = ex.Message.ToString();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        /// <summary>
        /// 刪除用戶指定的權限
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeleteEditPrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();
            string LoginUserEmp = Data["LoginUserEmp"].ToString().ToUpper();
            string EditEmp = Data["EditEmp"].ToString().ToUpper();
            //string deleteSQL = "";
            Int32 Counter = 0;
            SFCDB.BeginTrain();
            try
            {
                MESDataObject.Module.T_C_USER_PRIVILEGE tcp = new MESDataObject.Module.T_C_USER_PRIVILEGE(SFCDB, this.DBTYPE);
                //MESDataObject.Module.Row_C_USER_PRIVILEGE rcp = (MESDataObject.Module.Row_C_USER_PRIVILEGE)tcp.NewRow();
                foreach (string item in Data["PRS"])
                {
                    tcp.Delete(EditEmp, null, item, ref Counter, SFCDB);
                    //rcp = tcp.getC_PrivilegebyID(item, SFCDB);
                    //deleteSQL += rcp.GetDeleteString(this.DBTYPE) + ";\n";
                }
                //SFCDB.ExecSQL("Begin\n" + deleteSQL + "End;");
                if (Counter > 0)
                {
                    SFCDB.CommitTrain();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000004");
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000023");
                StationReturn.Data = ex.Message.ToString();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
