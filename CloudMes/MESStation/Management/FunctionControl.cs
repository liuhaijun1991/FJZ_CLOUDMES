using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Management
{
    public class FunctionControl: MesAPIBase
    {

        protected APIInfo FGetFUNCTIONNAME = new APIInfo()
        {
            FunctionName = "GetFUNCTIONNAME",
            Description = "获取可选用的配置类型",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetCATEGORY = new APIInfo()
        {
            FunctionName = "GetGetCATEGORY",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSettingValueByFUNCTIONNAME = new APIInfo()
         {
             FunctionName = "GetSettingValueByFUNCTIONNAME",
             Description = "选择类型后做对每个输入栏位做动作",
             Parameters = new List<APIInputInfo>()
             {
             },
             Permissions = new List<MESPermission>() { }
         };

        protected APIInfo FCheckSystemFunctionExist = new APIInfo()
        {
            FunctionName = "CheckSystemFunctionExist",
            Description = "检查配置是否存在",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "FUNCTIONNAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" },

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FCheckUserFunctionExist = new APIInfo()
        {
            FunctionName = "CheckUserFunctionExist",
            Description = "检查用户配置是否存在",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "FUNCTIONNAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VALUENAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL3", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL4", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL5", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL6", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddNewSystemFunction = new APIInfo()
        {
            FunctionName = "AddNewSystemFunction",
            Description = "新增页面点击提交新增系統類型",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "FUNCTIONNAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FUNCTIONDEC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" }, 
                new APIInputInfo() {InputName = "CATEGORYDEC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VALUENAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "USERCONTROL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL3", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL4", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL5", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL6", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddNewUserFunction = new APIInfo()
        {
            FunctionName = "AddNewUserFunction",
            Description = "新增页面点击提交新增用戶配置數據",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "FUNCTIONNAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VALUE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL3", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL4", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL5", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL6", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL1NAME", InputType = "string", DefaultValue = ""},
                new APIInputInfo() {InputName = "EXTVAL2NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL3NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL4NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL5NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL6NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteFunctionControl = new APIInfo()
        {
            FunctionName = "DeleteFunctionControl",
            Description = "刪除動作將controlflag改為N",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSystemConfigMenuList = new APIInfo()
        {
            FunctionName = "GetSystemConfigMenuList",
            Description = "获取主页面报表数据",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetUserConfigMenuList = new APIInfo()
        {
            FunctionName = "GetUserConfigMenuList",
            Description = "获取用户配置界面报表数据",
            Parameters = new List<APIInputInfo>()
            {
                
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateFunctionControl = new APIInfo()
        {
            FunctionName = "UpdateFunctionControl",
            Description = "更新rFunctionControl_系統配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "FUNCTIONNAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FUNCTIONDEC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORYDEC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VALUENAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "USERCONTROL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL3", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL4", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL5", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL6", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateUserConfig = new APIInfo()
        {
            FunctionName = "UpdateUserConfig",
            Description = "更新rFunctionControl_用戶配置",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "FUNCTIONNAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VALUE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL2", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL3", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL4", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL5", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXTVAL6", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FCheckPermission = new APIInfo()
        {
            FunctionName = "CheckPermission",
            Description = "檢查操作工號是否有9級權限，否則不能操作系統配置",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUploadFunctionTypeData = new APIInfo()
        {
            FunctionName = "UploadFunctionTypeData",
            Description = "依照Functionname批量上傳",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetPanelFailStation = new APIInfo()
        {
            FunctionName = "GetPanelFailStation",
            Description = "GetPanelFailStation",
            Parameters = new List<APIInputInfo>()
            {                 
            },
            Permissions = new List<MESPermission>() { }
        };
        public FunctionControl()
        {
            this.Apis.Add(FGetFUNCTIONNAME.FunctionName, FGetFUNCTIONNAME);
            this.Apis.Add(FGetCATEGORY.FunctionName, FGetCATEGORY);
            this.Apis.Add(FGetSettingValueByFUNCTIONNAME.FunctionName, FGetSettingValueByFUNCTIONNAME);
            this.Apis.Add(FCheckSystemFunctionExist.FunctionName, FCheckSystemFunctionExist);
            this.Apis.Add(FCheckUserFunctionExist.FunctionName, FCheckUserFunctionExist);
            this.Apis.Add(FAddNewSystemFunction.FunctionName, FAddNewSystemFunction);
            this.Apis.Add(FAddNewUserFunction.FunctionName, FAddNewUserFunction);
            this.Apis.Add(FDeleteFunctionControl.FunctionName, FDeleteFunctionControl);
            this.Apis.Add(FUpdateFunctionControl.FunctionName, FUpdateFunctionControl);
            this.Apis.Add(FUpdateUserConfig.FunctionName, FUpdateUserConfig);
            this.Apis.Add(FGetSystemConfigMenuList.FunctionName, FGetSystemConfigMenuList);
            this.Apis.Add(FGetUserConfigMenuList.FunctionName, FGetUserConfigMenuList);
            this.Apis.Add(FCheckPermission.FunctionName, FCheckPermission);
            this.Apis.Add(FGetPanelFailStation.FunctionName, FGetPanelFailStation);
        }


        public void GetFUNCTIONNAME(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                T_c_user Tcu = new T_c_user(db, DB_TYPE_ENUM.Oracle);
                T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(db, DB_TYPE_ENUM.Oracle);
                bool Cpms = Tcu.CheckIfLevel9(this.LoginUser.EMP_NO, db);
                StationReturn.Data = Cpms? TRFC.GetAllFUNCTIONNAME(db) : TRFC.GetUserFUNCTIONNAME(db);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetCATEGORY(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            string FUNCTIONNAME = (Data["FUNCTIONNAME"].ToString()).Trim();
            try
            {
                T_c_user Tcu = new T_c_user(db, DB_TYPE_ENUM.Oracle);
                bool Cpms = Tcu.CheckIfLevel9(this.LoginUser.EMP_NO, db);
                T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(db, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = Cpms ? TRFC.GetAllCATEGORY(FUNCTIONNAME, db) : TRFC.GetUserCATEGORY(FUNCTIONNAME, db);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetSettingValueByFUNCTIONNAME(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
         {
            OleExec db = DBPools["SFCDB"].Borrow();
            string CATEGORY = Data["CATEGORY"].ToString();
            string FUNCTIONNAME = Data["FUNCTIONNAME"].ToString();
             try
             {
                 T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(db, DB_TYPE_ENUM.Oracle);
                 StationReturn.Data = TRFC.GetSettingValueByFUNCTIONNAME(FUNCTIONNAME, CATEGORY,db);
                 StationReturn.Status = StationReturnStatusValue.Pass;
             }
             catch (Exception e)
             {  
                 throw e;
             }
             finally
             {
                StationReturn.Message = "";
                DBPools["SFCDB"].Return(db);
             }
         }
        public void AddNewSystemFunction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FUNCTION_CONTROL rFunctionControl = null;
            T_R_FUNCTION_CONTROL_EX rFunctionControlEX = null;
            OleExec sfcdb = null;
            for (int i = 1; i < 6; i++)
            {
                string DataName = "EXTVAL" + i.ToString();
                if (Data[DataName].ToString() == "")
                {   
                    for(int n = 1; n < 5 - i; n++) {
                        string aaa = DataName;
                        string aa1 = Data[DataName].ToString();
                        string bbb= "EXTVAL" + (i + n).ToString();
                        string bb1= Data["EXTVAL" + (i + n).ToString()].ToString();
                        if (Data["EXTVAL" + (i + n).ToString()].ToString() != "")
                        {
                            StationReturn.MessageCode = "MSGCODE20200413150100";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                            return;
                        }     
                     } 
                }
            }

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rFunctionControl = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_FUNCTION_CONTROL r = (Row_R_FUNCTION_CONTROL)rFunctionControl.NewRow();


                rFunctionControlEX = new T_R_FUNCTION_CONTROL_EX(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_FUNCTION_CONTROL_EX e = (Row_R_FUNCTION_CONTROL_EX)rFunctionControlEX.NewRow();


                r.ID = rFunctionControl.GetNewID(this.BU, sfcdb);
                r.FUNCTIONNAME = (Data["FUNCTIONNAME"].ToString()).Trim();
                r.CATEGORY = (Data["CATEGORY"].ToString()).Trim();
                r.FUNCTIONDEC = (Data["FUNCTIONDEC"].ToString()).Trim();
                r.CATEGORYDEC = (Data["CATEGORYDEC"].ToString()).Trim();
                r.VALUE = (Data["VALUENAME"].ToString()).Trim();
                r.EXTVAL = (Data["EXTVAL1"].ToString()).Trim();
                r.CONTROLFLAG = "Y";
                r.CREATEBY = this.LoginUser.EMP_NO;
                r.CREATETIME = GetDBDateTime();
                r.EDITBY = this.LoginUser.EMP_NO;
                r.EDITTIME = GetDBDateTime();
                r.FUNCTIONTYPE = "SYSTEM";
                r.USERCONTROL = (Data["USERCONTROL"].ToString()).Trim();

                

                for (int i = 2; i <= 6; i++) {
                    string DataName = "EXTVAL" + i.ToString();
                    if (Data[DataName].ToString() != "")
                    {
                        e.ID = rFunctionControlEX.GetNewID(this.BU, sfcdb);
                        e.SEQ_NO = (double?)i-1;
                        e.NAME = "";
                        e.VALUE = Data[DataName].ToString();
                        e.DETAIL_ID = r.ID;
                        sfcdb.ExecSQL(e.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
            }

                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));

                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "Successfully";
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
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void AddNewUserFunction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FUNCTION_CONTROL rFunctionControl = null;
            T_R_FUNCTION_CONTROL_EX rFunctionControlEX = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                #region VNDCN Keep Link Relation To Rework: if there is REWORK_UPDATE_SHIPFLAG config info then update csn shipped_flag=0
                if (this.BU.Equals("VNDCN"))
                {
                    string funName = Data["FUNCTIONNAME"].ToString().Trim();                    
                    if (funName.Equals("REWORK_UPDATE_SHIPFLAG"))
                    {
                        var userFunc = sfcdb.ORM.Queryable<C_USER_FUNCTION>().Where(t => t.FUNCTIONNAME == "REWORK_UPDATE_SHIPFLAG" && t.USERID == this.LoginUser.ID).Any();
                        if (!userFunc)
                            throw new Exception($@"Have no permission to config REWORK_UPDATE_SHIPFLAG, please call PE 孟玲/Manh Linh!");

                        if (!Data["VALUE"].ToString().Trim().ToUpper().Equals("APOLLO"))//限定只有APOLLO可以這麼幹免得PE亂搞
                        {
                            throw new Exception($@"Only Apollo Series can config REWORK_UPDATE_SHIPFLAG!");
                        }
                        string sn = Data["EXTVAL2"].ToString().Trim();
                        string csn = Data["EXTVAL1"].ToString().Trim();                        
                        var isShip = sfcdb.ORM.Queryable<R_SHIP_DETAIL>().Where(t => t.SN == csn).Any();
                        if (isShip)
                        {
                            throw new Exception($@"CSN: {csn} has been shipped!");
                        }
                        var isLink = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == csn && t.SN == sn && t.VALID_FLAG == 1).Any();
                        if (isLink)
                        {
                            var snObj = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == csn && t.VALID_FLAG == "1" && t.SHIPPED_FLAG == "1").ToList().FirstOrDefault();
                            if (snObj != null)
                            {
                                snObj.SHIPPED_FLAG = "0";
                                sfcdb.ORM.Updateable(snObj).Where(t => t.SN == snObj.SN && t.VALID_FLAG == "1" && t.SHIPPED_FLAG == "1").ExecuteCommand();

                                T_R_SN_LOG t_R_SN_LOG = new T_R_SN_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                                R_SN_LOG snLog = new R_SN_LOG
                                {
                                    ID = t_R_SN_LOG.GetNewID(this.BU, sfcdb),
                                    SNID = snObj.ID,
                                    LOGTYPE = "REWORK_UPDATE_SHIPFLAG",
                                    DATA1 = sn,
                                    DATA2 = csn,
                                    FLAG = "Y",
                                    CREATETIME = t_R_SN_LOG.GetDBDateTime(sfcdb),
                                    CREATEBY = this.LoginUser.EMP_NO
                                };
                                t_R_SN_LOG.Save(sfcdb, snLog);
                            }
                        }
                    }
                }
                #endregion

                rFunctionControl = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_FUNCTION_CONTROL r = (Row_R_FUNCTION_CONTROL)rFunctionControl.NewRow();

                rFunctionControlEX = new T_R_FUNCTION_CONTROL_EX(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_FUNCTION_CONTROL_EX e = (Row_R_FUNCTION_CONTROL_EX)rFunctionControlEX.NewRow();

                r.ID = rFunctionControl.GetNewID(this.BU, sfcdb);
                r.FUNCTIONNAME = (Data["FUNCTIONNAME"].ToString()).Trim();
                r.CATEGORY = (Data["CATEGORY"].ToString()).Trim();
                r.FUNCTIONDEC = "";
                r.CATEGORYDEC = "";
                r.VALUE = (Data["VALUE"].ToString()).Trim();
                r.EXTVAL = (Data["EXTVAL1"].ToString()).Trim();
                r.CONTROLFLAG = "Y";
                r.CREATEBY = this.LoginUser.EMP_NO;
                r.CREATETIME = GetDBDateTime();
                r.EDITBY = this.LoginUser.EMP_NO;
                r.EDITTIME = GetDBDateTime();
                r.FUNCTIONTYPE = "NOSYSTEM";
                r.USERCONTROL = "";
                
                for (int i = 2; i <= 6; i++)
                {
                    string DataValue = "EXTVAL" + i.ToString();
                    string DataNAME = "EXTVAL" + i.ToString()+"NAME";
                    if (Data[DataValue].ToString() != "")
                    {
                        e.ID = rFunctionControlEX.GetNewID(this.BU, sfcdb);
                        e.SEQ_NO = (double?)i - 1;
                        e.NAME = Data[DataNAME].ToString();
                        e.VALUE = Data[DataValue].ToString();
                        e.DETAIL_ID = r.ID;
                        sfcdb.ExecSQL(e.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                }

                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "Successfully";
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
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void CheckSystemFunctionExist(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = null;
            db = this.DBPools["SFCDB"].Borrow();
            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(db, DB_TYPE_ENUM.Oracle);
            string FUNCTIONNAME = (Data["FUNCTIONNAME"].ToString()).Trim();
            string CATEGORY = (Data["CATEGORY"].ToString()).Trim();
            try
            {
                StationReturn.Data = TRFC.CheckSystemFunctionExist(FUNCTIONNAME, CATEGORY, db);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        
         }
        public void CheckUserFunctionExist(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = null;
            db = this.DBPools["SFCDB"].Borrow();
            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(db, DB_TYPE_ENUM.Oracle);
            string FUNCTIONNAME = (Data["FUNCTIONNAME"].ToString()).Trim();
            string CATEGORY = (Data["CATEGORY"].ToString()).Trim();
            string VALUE = (Data["VALUE"].ToString()).Trim();
            string EXTVAL1 = (Data["EXTVAL1"].ToString()).Trim();
            string EXTVAL2 = (Data["EXTVAL2"].ToString()).Trim();
            string EXTVAL3 = (Data["EXTVAL3"].ToString()).Trim();
            string EXTVAL4 = (Data["EXTVAL4"].ToString()).Trim();
            string EXTVAL5 = (Data["EXTVAL5"].ToString()).Trim();
            string EXTVAL6 = (Data["EXTVAL6"].ToString()).Trim();
            try
            {
                if( EXTVAL3 == ""&& EXTVAL2 != "") {
                    StationReturn.Data = TRFC.CheckUserFunctionExist(FUNCTIONNAME, CATEGORY, VALUE, EXTVAL1, EXTVAL2, db);
                }
                else if(EXTVAL4 == "" && EXTVAL3 != "")
                {
                    StationReturn.Data = TRFC.CheckUserFunctionExist(FUNCTIONNAME, CATEGORY, VALUE, EXTVAL1, EXTVAL2, EXTVAL3, db);
                }
                else if (EXTVAL5 == "" && EXTVAL3 != "")
                {
                    StationReturn.Data = TRFC.CheckUserFunctionExist(FUNCTIONNAME, CATEGORY, VALUE, EXTVAL1, EXTVAL2, EXTVAL3, EXTVAL4,db);
                }
                else if (EXTVAL6 == "" && EXTVAL3 != "")
                {
                    StationReturn.Data = TRFC.CheckUserFunctionExist(FUNCTIONNAME, CATEGORY, VALUE, EXTVAL1, EXTVAL2, EXTVAL3,EXTVAL4, EXTVAL5,db);
                }
                else if (EXTVAL6 != "")
                {
                    StationReturn.Data = TRFC.CheckUserFunctionExist(FUNCTIONNAME, CATEGORY, VALUE, EXTVAL1, EXTVAL2, EXTVAL3, EXTVAL4, EXTVAL5, EXTVAL6, db);
                }
                else
                {
                    StationReturn.Data = TRFC.CheckUserFunctionExist(FUNCTIONNAME, CATEGORY, VALUE, EXTVAL1, db);
                }


                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }

        }
        public void DeleteFunctionControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FUNCTION_CONTROL rFunctionControl = null;
            OleExec sfcdb = null;
            string EDIT_EMP = this.LoginUser.EMP_NO;
            DateTime EDIT_TIME = GetDBDateTime();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rFunctionControl = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                int strRet = rFunctionControl.DeleteFunctionControl((Data["ID"].ToString()).Trim(), EDIT_EMP, EDIT_TIME,sfcdb);
                if (strRet > 0)
                {
                    StationReturn.Message = "Successfully";
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
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void UpdateFunctionControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FUNCTION_CONTROL rFunctionControl = null;
            T_R_FUNCTION_CONTROL_EX rFunctionControlEX = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rFunctionControl = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                R_F_CONTROL r = rFunctionControl.GET_byID((Data["ID"].ToString()).Trim(),sfcdb);
                rFunctionControlEX = new T_R_FUNCTION_CONTROL_EX(sfcdb, DB_TYPE_ENUM.Oracle);
             


                r.FUNCTIONNAME = (Data["FUNCTIONNAME"].ToString()).Trim();
                r.CATEGORY = (Data["CATEGORY"].ToString()).Trim();
                r.FUNCTIONDEC = (Data["FUNCTIONDEC"].ToString()).Trim();
                r.CATEGORYDEC = (Data["CATEGORYDEC"].ToString()).Trim();
                r.VALUE = (Data["VALUENAME"].ToString()).Trim();
                r.USERCONTROL = (Data["USERCONTROL"].ToString()).Trim();
                r.EXTVAL = (Data["EXTVAL1"].ToString()).Trim();
                r.EDITBY = this.LoginUser.EMP_NO;
                r.EDITTIME = GetDBDateTime();
                r.CONTROLFLAG = r.CONTROLFLAG;
                r.CREATEBY = r.CREATEBY;
                r.CREATETIME = r.CREATETIME;
                r.FUNCTIONTYPE = r.FUNCTIONTYPE;

                double? searchSeq;
                string EXTVALNAME;

                for (searchSeq = 1; searchSeq < 6; searchSeq++) {
                    R_F_CONTROL_EX e = rFunctionControlEX.ByDETAIL_ID_SEQ((Data["ID"].ToString()).Trim(), searchSeq, sfcdb);
                    EXTVALNAME = "EXTVAL" + (searchSeq + 1).ToString();
                    if (Data[EXTVALNAME].ToString() != "") {
                        if (e!= null)
                        { 
                            e.VALUE = Data[EXTVALNAME].ToString();
                            rFunctionControlEX.AddOrUpdateRFCEX("UPDATE", e, sfcdb);                     
                        }
                        else
                        {
                            Row_R_FUNCTION_CONTROL_EX en = (Row_R_FUNCTION_CONTROL_EX)rFunctionControlEX.NewRow();
                            en.ID = rFunctionControlEX.GetNewID(this.BU, sfcdb);
                            en.SEQ_NO = (double?)searchSeq;
                            en.NAME = "";
                            en.VALUE = Data[EXTVALNAME].ToString();
                            en.DETAIL_ID = Data["ID"].ToString();
                            sfcdb.ExecSQL(en.GetInsertString(DB_TYPE_ENUM.Oracle));
                        }
                    }
                    else if (Data[EXTVALNAME].ToString() == ""&& e!=null)
                    {
                       e.DETAIL_ID = "#"+Data["ID"].ToString();
                       rFunctionControlEX.AddOrUpdateRFCEX("UPDATE", e, sfcdb); 
                    }
                }
                

               

                int strRet=rFunctionControl.AddOrUpdateRFC("UPDATE", r, sfcdb);


                if (strRet > 0)
                {
                    StationReturn.Message = "Successfully";
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
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void UpdateUserConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FUNCTION_CONTROL rFunctionControl = null;
            T_R_FUNCTION_CONTROL_EX rFunctionControlEX = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rFunctionControl = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                R_F_CONTROL r = rFunctionControl.GET_byID((Data["ID"].ToString()).Trim(), sfcdb);
                rFunctionControlEX = new T_R_FUNCTION_CONTROL_EX(sfcdb, DB_TYPE_ENUM.Oracle);


                r.FUNCTIONNAME = (Data["FUNCTIONNAME"].ToString()).Trim();
                r.CATEGORY = (Data["CATEGORY"].ToString()).Trim();
                r.FUNCTIONDEC = r.FUNCTIONDEC;
                r.CATEGORYDEC = r.FUNCTIONDEC;
                r.VALUE = (Data["VALUE"].ToString()).Trim();
                r.USERCONTROL = r.USERCONTROL;
                r.EXTVAL = (Data["EXTVAL1"].ToString()).Trim();
                r.EDITBY = this.LoginUser.EMP_NO;
                r.EDITTIME = GetDBDateTime();
                r.CONTROLFLAG = r.CONTROLFLAG;
                r.CREATEBY = r.CREATEBY;
                r.CREATETIME = r.CREATETIME;
                r.FUNCTIONTYPE = r.FUNCTIONTYPE;

                double? searchSeq;
                string EXTVALNAME;

                for (searchSeq = 1; searchSeq < 6; searchSeq++)
                {
                    R_F_CONTROL_EX e = rFunctionControlEX.ByDETAIL_ID_SEQ((Data["ID"].ToString()).Trim(), searchSeq, sfcdb);
                    EXTVALNAME = "EXTVAL" + (searchSeq + 1).ToString();
                    if (Data[EXTVALNAME].ToString() != "")
                    {
                        if (e != null)
                        {
                            e.VALUE = Data[EXTVALNAME].ToString();
                            rFunctionControlEX.AddOrUpdateRFCEX("UPDATE", e, sfcdb);
                        }
                        else
                        {
                            Row_R_FUNCTION_CONTROL_EX en = (Row_R_FUNCTION_CONTROL_EX)rFunctionControlEX.NewRow();
                            en.ID = rFunctionControlEX.GetNewID(this.BU, sfcdb);
                            en.SEQ_NO = (double?)searchSeq;
                            en.NAME = "";
                            en.VALUE = Data[EXTVALNAME].ToString();
                            en.DETAIL_ID = Data["ID"].ToString();
                            sfcdb.ExecSQL(en.GetInsertString(DB_TYPE_ENUM.Oracle));
                        }
                    }
                    else if (Data[EXTVALNAME].ToString() == "" && e != null)
                    {
                        e.DETAIL_ID = "#" + Data["ID"].ToString();
                        rFunctionControlEX.AddOrUpdateRFCEX("UPDATE", e, sfcdb);
                    }
                }




                int strRet = rFunctionControl.AddOrUpdateRFC("UPDATE", r, sfcdb);


                if (strRet > 0)
                {
                    StationReturn.Message = "Successfully";
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
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetSystemConfigMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FUNCTION_CONTROL rFunctionControl = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rFunctionControl = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = rFunctionControl.GetSystemConfigMenuList(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetUserConfigMenuList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FUNCTION_CONTROL rFunctionControl = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                rFunctionControl = new T_R_FUNCTION_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = rFunctionControl.GetUserConfigMenuList(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void CheckPermission(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            bool UserCtl=false;
            bool Cpms;
            OleExec db = DBPools["SFCDB"].Borrow();
            string FUNCTIONNAME = (Data["FUNCTIONNAME"].ToString()).Trim();
            string CATEGORY = (Data["CATEGORY"].ToString()).Trim();
            try
            {   
                T_c_user Tcu = new T_c_user(db, DB_TYPE_ENUM.Oracle);
                T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(db, DB_TYPE_ENUM.Oracle);
             
                List<R_F_CONTROL> ListRFC = new List<R_F_CONTROL>();
                
                ListRFC = TRFC.GetSystemListbyFuncCate(FUNCTIONNAME, CATEGORY, db);
                Cpms = Tcu.CheckIfLevel9(this.LoginUser.EMP_NO, db);
                if (ListRFC.Count!=0) { 
                    UserCtl = ListRFC[0].USERCONTROL == "N";
                }
                StationReturn.Status = StationReturnStatusValue.Pass;

                if (UserCtl&&!Cpms)
                {
                    StationReturn.MessageCode = "MSGCODE20200414091000";
                    StationReturn.Data = false; 
                }
                else
                {
                    StationReturn.MessageCode = "Successfully";
                    StationReturn.Data = true;
                }  
            }
            catch (Exception e)
            {   
                throw e;
            }
            finally
            {   
                DBPools["SFCDB"].Return(db);
            }
        }
        public void GetPanelFailStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;            
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                List<string> list = new List<string>() { ""};
                List<string> listTemp = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "PanelFailStation" && r.CATEGORY == "Station"
                  && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM").Select(r => r.VALUE).ToList();
                list.AddRange(listTemp);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = null;
                StationReturn.Data = list;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }                
            }
        }
        public void UploadFunctionTypeData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                {
                    //throw new Exception("Please Input Excel Data");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429	", new string[] { "Excel Data" }));
                }
                if (Data["FileName"] == null)
                {
                    //throw new Exception("Please Input FileName");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429	", new string[] { "FileName" }));
                }
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);

                string filePath = System.IO.Directory.GetCurrentDirectory() + "\\UploadFile\\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                {
                    //throw new Exception($@"上傳的Excel中沒有內容!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111744"));

                }




                string result = "";

                #region 写入数据库

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string FUNCTION_NAME = dt.Rows[i]["FUNCTIONNAME"].ToString().Trim();
                    string CATEGORY = dt.Rows[i]["CATEGORY"].ToString().Trim();
                    string FUNCTIONDEC = dt.Rows[i]["FUNCTIONDEC"].ToString().Trim();
                    string CATEGORYDEC = dt.Rows[i]["CATEGORYDEC"].ToString().Trim();
                    string VALUE = dt.Rows[i]["VALUE"].ToString().Trim();
                    string EXTVAL = dt.Rows[i]["EXTVAL"].ToString().Trim();
                    if (FUNCTION_NAME == "" && FUNCTION_NAME == "")
                    {
                        return;
                    }
                    try
                    {
                        if (SFCDB == null)
                        {
                            SFCDB = this.DBPools["SFCDB"].Borrow();
                        }
                        T_R_FUNCTION_CONTROL rFunctionControl = null;
                        T_R_FUNCTION_CONTROL_EX rFunctionControlEX = null;
                        rFunctionControl = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
                        Row_R_FUNCTION_CONTROL d = (Row_R_FUNCTION_CONTROL)rFunctionControl.NewRow();

                        rFunctionControlEX = new T_R_FUNCTION_CONTROL_EX(SFCDB, DB_TYPE_ENUM.Oracle);
                        Row_R_FUNCTION_CONTROL_EX e = (Row_R_FUNCTION_CONTROL_EX)rFunctionControlEX.NewRow();
                        var CategoryExist = SFCDB.ORM.Queryable<R_F_CONTROL>()
                                           .Where(r => r.FUNCTIONNAME == FUNCTION_NAME && r.CATEGORY == CATEGORY )
                                           .ToList();
                        if (i == 0 && CategoryExist.Count>0 && (FUNCTION_NAME.Contains("CTOWOWIP") || FUNCTION_NAME.Contains("BTSWOWIP")))
                        {

                            for (int k = 0; k< CategoryExist.Count(); k++)
                            {
                                var aa = CategoryExist[k].ID;
                                SFCDB.ORM.Deleteable<R_F_CONTROL_EX>()
                                 .Where(sd => sd.DETAIL_ID == CategoryExist[k].ID).ExecuteCommand();
                            }
                            SFCDB.ORM.Deleteable<R_F_CONTROL>()
                                     .Where(sd => sd.FUNCTIONNAME == FUNCTION_NAME && sd.CATEGORY == CATEGORY).ExecuteCommand();
                        }
                        d.ID = rFunctionControl.GetNewID(this.BU, SFCDB);
                        d.FUNCTIONNAME = FUNCTION_NAME;
                        d.CATEGORY = CATEGORY;
                        d.FUNCTIONDEC = FUNCTIONDEC;
                        d.CATEGORYDEC = CATEGORYDEC;
                        d.VALUE = VALUE;
                        d.EXTVAL = EXTVAL;
                        d.CONTROLFLAG = "Y";
                        d.CREATEBY = this.LoginUser.EMP_NO;
                        d.CREATETIME = GetDBDateTime();
                        d.EDITBY = this.LoginUser.EMP_NO;
                        d.EDITTIME = GetDBDateTime();
                        d.FUNCTIONTYPE = "NOSYSTEM";
                        d.USERCONTROL = "Y";
                        string strRet = SFCDB.ExecSQL(d.GetInsertString(DB_TYPE_ENUM.Oracle));
                        for (int j = 1; j < 6; j++)//目前只支持6位擴展，如果需要增加，則+1
                        {
                            var Exval = "EXTVAL" + j;
                            var Funex = dt.Rows[i][Exval].ToString().Trim();
                            if (Funex == "")
                            {
                                continue;
                            }
                            SFCDB.ORM.Insertable<R_F_CONTROL_EX>(new R_F_CONTROL_EX()
                            {
                                ID = rFunctionControlEX.GetNewID(BU, SFCDB),
                                SEQ_NO = j,
                                NAME = CATEGORY,
                                VALUE = Funex,
                                DETAIL_ID = d.ID
                            }).ExecuteCommand();
                        }
                        if (Convert.ToInt32(strRet) > 0)
                        {
                            StationReturn.Message = "Successfully";
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            StationReturn.Data = "";
                        }
                        else
                        {
                            StationReturn.MessageCode = "MES00000036";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        result += VALUE + "," + ex.Message + ";";
                    }

                }


                if (result == "")
                {
                    result = "All Upload OK ! ";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    result = "Upload Fail:" + result;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                #endregion

                StationReturn.Message = result;

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }

    }
}
