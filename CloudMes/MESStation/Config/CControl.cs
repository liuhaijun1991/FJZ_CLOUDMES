using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject;
using MESDBHelper;
using System.Data;

namespace MESStation.Config
{
    public class CControl : MesAPIBase
    {
        protected APIInfo _FSetBackflushTime = new APIInfo
        {
            FunctionName = "SetBackflushTime",
            Description = "Setting Backflush time and date",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "StartTime", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EndDate", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BackflushDate", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _FGetReplaceWOList = new APIInfo
        {
            FunctionName = "GetReplaceWOList",
            Description = "Get replace WO list",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _FAddReplaceWOControl = new APIInfo
        {
            FunctionName = "AddReplaceWOControl",
            Description = "Add replace WO control",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo _FDeleteReplaceWOControl = new APIInfo
        {
            FunctionName = "DeleteReplaceWOControl",
            Description = "Delete replace WO control",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetHWDReplaceSetting = new APIInfo
        {
            FunctionName = "GetHWDReplaceSetting",
            Description = "Get HWD Replace Setting",
            Parameters = new List<APIInputInfo>()
            {               
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FHWDReplaceSetting = new APIInfo
        {
            FunctionName = "HWDReplaceSetting",
            Description = "HWD Replace Setting",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "NUMBER", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };


        public CControl() {
            this.Apis.Add(_FSetBackflushTime.FunctionName, _FSetBackflushTime);
            this.Apis.Add(_FGetReplaceWOList.FunctionName, _FGetReplaceWOList);
            this.Apis.Add(_FAddReplaceWOControl.FunctionName, _FAddReplaceWOControl);
            this.Apis.Add(_FDeleteReplaceWOControl.FunctionName, _FDeleteReplaceWOControl);
            this.Apis.Add(FGetHWDReplaceSetting.FunctionName, FGetHWDReplaceSetting);
            this.Apis.Add(FHWDReplaceSetting.FunctionName, FHWDReplaceSetting);
        }

        public void SetBackflushTime(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string startTime = Data["StartTime"].ToString();
            string endTime = Data["EndTime"].ToString();
            string backflushDate = Data["BackflushDate"].ToString();
            DateTime _backflushDate;
            string result = "";
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                DateTime dtStart = DateTime.ParseExact(startTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                DateTime dtEnd = DateTime.ParseExact(endTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                if (DateTime.Compare(dtStart, dtEnd) >= 0)
                {
                    //throw new MESReturnMessage("開始時間不能大於等於結束時間");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816095130"));
                }

                _backflushDate = DateTime.ParseExact(backflushDate, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);

                T_C_CONTROL t_c_control = new T_C_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_C_CONTROL rowControl;
                Row_C_CONTROL rowPostControl;
                var backflush = SFCDB.ORM.Queryable<C_CONTROL>().Where(c => c.CONTROL_NAME == "BACKFLUSH").ToList().FirstOrDefault();
                if (backflush == null)
                {
                    rowControl = (Row_C_CONTROL)t_c_control.NewRow();
                    rowControl.ID = t_c_control.GetNewID(BU, SFCDB);
                    rowControl.CONTROL_NAME = "BACKFLUSH";
                    rowControl.CONTROL_VALUE = startTime + "~" + endTime;
                    rowControl.CONTROL_TYPE = "SINGLE";
                    rowControl.CONTROL_LEVEL = "0";
                    rowControl.CONTROL_DESC = "該時間段為月結時間，應經管要求拋賬程序禁止拋賬";
                    rowControl.EDIT_EMP = LoginUser.EMP_NO;
                    rowControl.EDIT_TIME = GetDBDateTime();
                    result = SFCDB.ExecSQL(rowControl.GetInsertString(DB_TYPE_ENUM.Oracle));
                }
                else
                {
                    rowControl = (Row_C_CONTROL)t_c_control.GetObjByID(backflush.ID,SFCDB);
                    rowControl.CONTROL_VALUE = startTime + "~" + endTime;
                    rowControl.EDIT_EMP = LoginUser.EMP_NO;
                    rowControl.EDIT_TIME = GetDBDateTime();
                    result = SFCDB.ExecSQL(rowControl.GetUpdateString(DB_TYPE_ENUM.Oracle));
                }
                if(Convert.ToInt32(result)<=0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "C_CONTROL" }));
                }
                var backflushPostDate = SFCDB.ORM.Queryable<C_CONTROL>().Where(c => c.CONTROL_NAME == "BACKFLUSHPOSTEDATE").ToList().FirstOrDefault();
                if (backflushPostDate == null)
                {
                    rowPostControl = (Row_C_CONTROL)t_c_control.NewRow();
                    rowPostControl.ID = t_c_control.GetNewID(BU, SFCDB);
                    rowPostControl.CONTROL_NAME = "BACKFLUSHPOSTEDATE";
                    rowPostControl.CONTROL_VALUE = backflushDate;
                    rowPostControl.CONTROL_DESC = "MM/DD/YYYY,月結后，把月結期間的賬拋到該日期";
                    rowPostControl.EDIT_EMP = LoginUser.EMP_NO;
                    rowPostControl.EDIT_TIME = GetDBDateTime();
                    result = SFCDB.ExecSQL(rowPostControl.GetInsertString(DB_TYPE_ENUM.Oracle));
                }
                else
                {
                    rowPostControl = (Row_C_CONTROL)t_c_control.GetObjByID(backflushPostDate.ID, SFCDB);
                    rowPostControl.CONTROL_VALUE = _backflushDate.ToString("MM/dd/yyyy");
                    rowPostControl.EDIT_EMP = LoginUser.EMP_NO;
                    rowPostControl.EDIT_TIME = GetDBDateTime();
                    result = SFCDB.ExecSQL(rowPostControl.GetUpdateString(DB_TYPE_ENUM.Oracle));
                }
                if (Convert.ToInt32(result) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "C_CONTROL" }));
                }
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Data = "";
                StationReturn.Message = "";
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);                
            }            
        }

        public void GetBackflushSetting(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sql = $@"select decode(control_name,'BACKFLUSH','Month End Duration','BACKFLUSHPOSTEDATE','Next Month Backflush Date',control_name) as control_name,CONTROL_VALUE, EDIT_EMP, EDIT_TIME
                                from c_control where control_name  in ('BACKFLUSHPOSTEDATE', 'BACKFLUSH')";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Message = "";
                    StationReturn.Data = "";
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void GetReplaceWOList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {          
            string wo = Data["WO"].ToString();
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_C_CONTROL t_c_control = new T_C_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
                List<C_CONTROL> list = t_c_control.GetControlList("REPLACE_WO", wo, SFCDB);
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }                
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Message = "";
                    StationReturn.Data = "";
                }
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void AddReplaceWOControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string wo = Data["WO"].ToString().Trim().ToUpper();
            OleExec SFCDB = null;
            try
            {
                int result;
                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.ThrowSqlExeception = true;
                T_C_CONTROL t_c_control = new T_C_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
                if (wo == "")
                {
                    throw new Exception("Please input WO ");
                }
                if (t_c_control.ValueIsExist("REPLACE_WO", wo, SFCDB))
                {
                    throw new Exception($@"The {wo} has been configured!");
                }

                result = t_c_control.AddNewControl(this.BU, "REPLACE_WO", wo, "WO", "", "用於SN版本升級后重工的工單", LoginUser.EMP_NO, SFCDB);
                this.DBPools["SFCDB"].Return(SFCDB);
                if (result > 0)
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
            }
            catch (Exception ee)
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
        }

        public void DeleteReplaceWOControl(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {        
            OleExec sfcdb = null;
            try
            {
                int result;
                string id = Data["ID"].ToString().Trim().ToUpper();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                result = t_c_control.DeleteControlByID(id, sfcdb);
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
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
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Message = e.Message;
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.MessageCode = "MES00000023";
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
        }

        public void GetHWDReplaceSetting(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sql = $@"select id,control_name,control_value,edit_emp,edit_time from c_control where control_name in ('REPLACE_TIME_OLD_SN','REPLACE_TIME_NEW_SN')";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);               
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(dt.Rows.Count);
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Message = "";
                    StationReturn.Data = "";
                }
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

        public void HWDReplaceSetting(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string id = Data["ID"] == null ? "" : Data["ID"].ToString();
                string number = Data["NUMBER"] == null ? "" : Data["NUMBER"].ToString();
                string emp = LoginUser.EMP_NO;
                DateTime dt = SFCDB.ORM.GetDate();
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("Please Input ID!");
                }
                if (string.IsNullOrEmpty(number))
                {
                    throw new Exception("Please Input Number!");
                }
                int result = SFCDB.ORM.Updateable<C_CONTROL>().UpdateColumns(r => new C_CONTROL { CONTROL_VALUE = number, EDIT_EMP = emp, EDIT_TIME = dt })
                    .Where(r => r.ID == id).ExecuteCommand();

                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "C_CONTROL" }));
                }
                StationReturn.Data = "";
                StationReturn.Message = "";
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Status = StationReturnStatusValue.Pass;
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
