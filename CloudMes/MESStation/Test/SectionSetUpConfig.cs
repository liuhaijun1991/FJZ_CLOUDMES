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

namespace MESStation.Test
{
    public class SectionSetUpConfig : MESPubLab.MESStation.MesAPIBase
    {
        public SectionSetUpConfig()
        {
            this.Apis.Add(FSelectByColumnName.FunctionName, FSelectByColumnName);
            this.Apis.Add(FSectionInsert.FunctionName, FSectionInsert);
            this.Apis.Add(FSectionDelete.FunctionName, FSectionDelete);
            this.Apis.Add(FShowAllData.FunctionName, FShowAllData);
        }

        protected APIInfo FShowAllData = new APIInfo()
        {
            FunctionName = "ShowAllData",
            Description = "SectionSetUpTest",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectByColumnName = new APIInfo()
        {
            FunctionName = "SelectByColumnName",
            Description = "SectionSetUpTest",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ColumnName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ColumnValue", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSectionInsert = new APIInfo()
        {
            FunctionName = "SectionInsert",
            Description = "SectionSetUpForInsert",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SectionName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DESCRIPTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSectionDelete = new APIInfo()
        {
            FunctionName = "DeleteSectionByID",
            Description = "SectionSetUpForDelete",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public void SectionInsert(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_SECTION cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SECTION r = (Row_C_SECTION)cSection.NewRow();
                r.ID = cSection.GetNewID(Data["BU"].ToString(), sfcdb);
                r.SECTION_NAME = (Data["SectionName"].ToString()).Trim();
                r.DESCRIPTION = (Data["DESCRIPTION"].ToString()).Trim();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                    StationReturn.Data = strRet;
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void ShowAllData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_SECTION cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SECTION_DETAIL> list = new List<C_SECTION_DETAIL>();
                list = cSection.ShowAllData(sfcdb);
                if (list.Count > 0)
                { 
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "No data found";
                }

            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void SelectByColumnName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            T_C_SECTION cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
            List<C_SECTION_DETAIL> list = new List<C_SECTION_DETAIL>();
            try
            {
                string data = Data["ColumnValue"].ToString();
                string column = Data["ColumnName"].ToString();
                list = cSection.GetDataByColumn(column, data, sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "No data found";
                }

            }
            catch (Exception)
            {
                //StationReturn.Message = e.Message;
                StationReturn.Message = "ERROE";
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DeleteSectionByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_SECTION cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SECTION r = (Row_C_SECTION)cSection.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                    StationReturn.Data = strRet;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Delete Fail or Data Not Exist";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
}

