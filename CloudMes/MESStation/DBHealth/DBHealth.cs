using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using MESDataObject.Module.DBHealth;

namespace MESStation.DBHealth
{
    public class DBHealth : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo loaddata = new APIInfo()
        {
            FunctionName = "LoadData",
            Description = "LoadData",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo insertdata = new APIInfo()
        {
            FunctionName = "InsertData",
            Description = "InsertData",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo updatedata = new APIInfo()
        {
            FunctionName = "UpdateData",
            Description = "UpdateData",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo deletedata = new APIInfo()
        {
            FunctionName = "DeleteData",
            Description = "DeleteData",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        public DBHealth()
        {
            this.Apis.Add(loaddata.FunctionName, loaddata);
            this.Apis.Add(insertdata.FunctionName, insertdata);
            this.Apis.Add(updatedata.FunctionName, updatedata);
            this.Apis.Add(deletedata.FunctionName, deletedata);
        }

        public void LoadData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_DBA_TABLE_T r_data = new T_C_DBA_TABLE_T();
            DataTable dt = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                dt = r_data.GetDataTable(sfcdb);
                StationReturn.Data = dt;
                StationReturn.Message = "Success!";
                StationReturn.Status = StationReturnStatusValue.Pass;

                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
       
        public void InsertData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_DBA_TABLE_T r_data = new T_C_DBA_TABLE_T();
            string status = "";
            //MESDataObject.Module.TESTD.R_SN_EX obj;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string APTYPE = Data["AP_TYPE"].ToString();
                string TABLENAME = Data["TABLE_NAME"].ToString();
                string TABLEATTRIBUTE = Data["TABLE_ATTRIBUTE"].ToString();
                string TABLEKPI = Data["TABLE_KPI"].ToString();
                string EMP_NO = LoginUser.EMP_NO;
                status = r_data.Insert(APTYPE, TABLENAME, TABLEATTRIBUTE, TABLEKPI, EMP_NO, sfcdb);
                if (status == "FAIL")
                {
                    StationReturn.Message = "DUPLICATE TABLE NAME";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Data = status;
                    StationReturn.Message = "Success!";
                    StationReturn.Status = StationReturnStatusValue.Pass;

                    this.DBPools["SFCDB"].Return(sfcdb);
                }

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        public void UpdateData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_DBA_TABLE_T r_data = new T_C_DBA_TABLE_T();
            string status = "";
            //MESDataObject.Module.TESTD.R_SN_EX obj;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string APTYPE = Data["AP_TYPE"].ToString();
                string TABLENAME = Data["TABLE_NAME"].ToString();
                string TABLEATTRIBUTE = Data["TABLE_ATTRIBUTE"].ToString();
                string TABLEKPI = Data["TABLE_KPI"].ToString();
                string EMP_NO = LoginUser.EMP_NO;
                status = r_data.Update(APTYPE, TABLENAME, TABLEATTRIBUTE, TABLEKPI, EMP_NO, sfcdb);
                StationReturn.Data = status;
                StationReturn.Message = "Success!";
                StationReturn.Status = StationReturnStatusValue.Pass;

                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_DBA_TABLE_T r_data = new T_C_DBA_TABLE_T();
            string status = "";
            //MESDataObject.Module.TESTD.R_SN_EX obj;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                //string APTYPE = Data["AP_TYPE"].ToString();
                string TABLENAME = Data["TABLENAME"].ToString();
                status = r_data.Delete(TABLENAME, sfcdb);
                StationReturn.Data = status;
                StationReturn.Message = "Success!";
                StationReturn.Status = StationReturnStatusValue.Pass;

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
