using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.DCN
{
    public class EACU_Label_Setting: MesAPIBase
    {
        protected APIInfo FGetSettingData = new APIInfo
        {
            FunctionName = "GetSettingData",
            Description = "Get Setting Data",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddNewRecord = new APIInfo
        {
            FunctionName = "AddNewRecord",
            Description = "Add New Record",
            Parameters = new List<APIInputInfo>()
            {               
                new APIInputInfo() {InputName = "CATEGORY", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ENGLISH", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RUSSIAN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FEditRecord = new APIInfo
        {
            FunctionName = "EditRecord",
            Description = "Edit Record",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ENGLISH", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RUSSIAN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDelete = new APIInfo
        {
            FunctionName = "Delete",
            Description = "Delete",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public EACU_Label_Setting()
        {
            this.Apis.Add(FGetSettingData.FunctionName, FGetSettingData);
            this.Apis.Add(FAddNewRecord.FunctionName, FAddNewRecord);
            this.Apis.Add(FEditRecord.FunctionName, FEditRecord);
            this.Apis.Add(FDelete.FunctionName, FDelete);
        }

        public void GetSettingData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string type = Data["TYPE"].ToString();
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (type.Trim().ToUpper().Equals("TYPE"))
                {
                    var list_type = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "EACU_LABEL_SETTING" && r.CONTROL_TYPE == type)
                        .Select(r => new {r.ID, CATEGORY=r.CONTROL_TYPE, TYPE = r.CONTROL_VALUE, PREFIX = r.CONTROL_LEVEL, RUSSIAN = r.CONTROL_DESC, r.EDIT_TIME, r.EDIT_EMP })
                        .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();

                    StationReturn.Data = list_type;
                }
                else
                {
                    var list_coo = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "EACU_LABEL_SETTING" && r.CONTROL_TYPE == type)
                        .Select(r => new {r.ID, CATEGORY=r.CONTROL_TYPE, COO = r.CONTROL_VALUE, ENGLISH = r.CONTROL_LEVEL, RUSSIAN = r.CONTROL_DESC, r.EDIT_TIME, r.EDIT_EMP })
                        .OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();

                    StationReturn.Data = list_coo;
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
                StationReturn.MessageCode = "MES00000026";
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
        public void AddNewRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {                
                string category = Data["CATEGORY"].ToString();
                string name = Data["NAME"].ToString().Trim();
                string english = Data["ENGLISH"].ToString().Trim();
                string russian = Data["RUSSIAN"].ToString().Trim();
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_C_CONTROL t_c_control = new T_C_CONTROL(SFCDB, this.DBTYPE);
                C_CONTROL control = new C_CONTROL();
                control.ID = t_c_control.GetNewID(this.BU, SFCDB);
                control.CONTROL_NAME = "EACU_LABEL_SETTING";                
                control.CONTROL_TYPE = category;
                control.CONTROL_VALUE = name;
                control.CONTROL_LEVEL = english;
                control.CONTROL_DESC = russian;
                control.EDIT_EMP = LoginUser.EMP_NO;
                control.EDIT_TIME = SFCDB.ORM.GetDate();
                int result = SFCDB.ORM.Insertable<C_CONTROL>(control).ExecuteCommand();
                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                    StationReturn.MessageCode = "MES00000026";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Save Data Fail";                    
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

        public void EditRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string id = Data["ID"].ToString().Trim();
                string name = Data["NAME"].ToString().Trim();
                string english = Data["ENGLISH"].ToString().Trim();
                string russian = Data["RUSSIAN"].ToString().Trim();
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_C_CONTROL t_c_control = new T_C_CONTROL(SFCDB, this.DBTYPE);
                C_CONTROL control = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.ID == id && r.CONTROL_NAME == "EACU_LABEL_SETTING").ToList().FirstOrDefault();
                if (control == null)
                {
                    throw new Exception($@"{id} Not Exist![C_CONTROL]");
                }
                control.CONTROL_VALUE = name;
                control.CONTROL_LEVEL = english;
                control.CONTROL_DESC = russian;
                control.EDIT_EMP = LoginUser.EMP_NO;
                control.EDIT_TIME = SFCDB.ORM.GetDate();
                int result = SFCDB.ORM.Updateable<C_CONTROL>(control).Where(r => r.ID == control.ID).ExecuteCommand();
                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                    StationReturn.MessageCode = "MES00000026";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Save Data Fail";
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
        public void Delete(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string id = Data["ID"].ToString();
                SFCDB = this.DBPools["SFCDB"].Borrow();
                int result = SFCDB.ORM.Deleteable<C_CONTROL>().Where(r => r.ID == id).ExecuteCommand();
                if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                    StationReturn.MessageCode = "MES00000026";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Delete Data Fail";
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
    }
}
