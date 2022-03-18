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
    public class StationSetUpConfig : MESPubLab.MESStation.MesAPIBase
    {
        public StationSetUpConfig()
        {
            this.Apis.Add(FSelectByColumnName.FunctionName, FSelectByColumnName);
            this.Apis.Add(FStationInsert.FunctionName, FStationInsert);
            this.Apis.Add(FStationDelete.FunctionName, FStationDelete);
            this.Apis.Add(FShowAllData.FunctionName, FShowAllData);
        }

        protected APIInfo FShowAllData = new APIInfo()
        {
            FunctionName = "ShowAllData",
            Description = "StationSetUpTest",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectByColumnName = new APIInfo()
        {
            FunctionName = "SelectByColumnName",
            Description = "StationSetUpTest",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ColumnName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ColumnValue", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FStationInsert = new APIInfo()
        {
            FunctionName = "StationInsert",
            Description = "SectionSetUpForInsert",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "StationName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FStationDelete = new APIInfo()
        {
            FunctionName = "DeleteStationByID",
            Description = "StationSetUpForDelete",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public void ShowAllData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_STATION_DETAIL> list = new List<C_STATION_DETAIL>();
                list = cStation.ShowAllData(sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = "1";
                    StationReturn.Message = "OK";
                }
                else
                {
                    StationReturn.Status = "0";
                    StationReturn.Message = "No data found";
                }

            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = "0";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void StationInsert(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_STATION rStation = (Row_C_STATION)cStation.NewRow();
                rStation.ID = cStation.GetNewID(Data["BU"].ToString(), sfcdb);
                rStation.STATION_NAME = Data["StationName"].ToString();
                rStation.TYPE = Data["TYPE"].ToString();
                string strRet = sfcdb.ExecSQL(rStation.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = "1";
                    StationReturn.Message = "OK";
                    StationReturn.Data = strRet;
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = "0";
                StationReturn.Message = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void SelectByColumnName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                string data = (Data["ColumnValue"].ToString()).Trim();
                string column = (Data["ColumnName"].ToString()).Trim();
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_STATION_DETAIL> list = new List<C_STATION_DETAIL>();
                list = cStation.GetDataByColumn(column, data, sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = "1";
                    StationReturn.Message = "OK";
                }
                else
                {
                    StationReturn.Status = "0";
                    StationReturn.Message = "No data found";
                }

            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = "0";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DeleteStationByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_STATION cStation = new T_C_STATION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_STATION r = (Row_C_STATION)cStation.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = "1";
                    StationReturn.Message = "OK";
                    StationReturn.Data = strRet;
                }
                else
                {
                    StationReturn.Status = "0";
                    StationReturn.Message = "Delete Fail or Data Not Exist";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = "0";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        private C_STATION_DETAIL RowToEmp(DataRow item)
        {
            C_STATION_DETAIL ep = new C_STATION_DETAIL();
            ep.Station_Name = item["Station_Name"].ToString();
            ep.Type = item["Type"].ToString();
            return ep;
        }
    }
}
