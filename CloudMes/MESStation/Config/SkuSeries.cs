using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    class SkuSeries : MesAPIBase
    {
        protected APIInfo _FetchCurrentSeries = new APIInfo()
        {
            FunctionName = "FetchCurrentSeries",
            Description = "通過傳入的Field（'BU','CustomerName','SeriesName'）及其值Value獲取系列集",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Field" }, new APIInputInfo() { InputName = "Value" } },
            //Permissions = new List<MESPermission>()
        };
        protected APIInfo _DeleteSeriesById = new APIInfo()
        {
            FunctionName = "DeleteSeriesById",
            Description = "通過主鍵刪除",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SeriesId" } },
            //Permissions = new List<MESPermission>()
        };
        protected APIInfo _AddNewSeries = new APIInfo()
        {
            FunctionName = "AddNewSeries",
            Description = "添加新系列",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "CUSTOMER_NAME" },
                new APIInputInfo() { InputName = "SERIES_NAME" },
                new APIInputInfo() { InputName = "DESCRIPTION" }
            },
            //Permissions = new List<MESPermission>()
        };
        protected APIInfo _UpdateSeries = new APIInfo()
        {
            FunctionName = "UpdateSeries",
            Description = "更新系列",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "ID" },
                new APIInputInfo() { InputName = "CUSTOMER_NAME" },
                new APIInputInfo() { InputName = "SERIES_NAME" },
                new APIInputInfo() { InputName = "DESCRIPTION" }
            }
        };
        public SkuSeries()
        {
            this.Apis.Add(_FetchCurrentSeries.FunctionName, _FetchCurrentSeries);
            this.Apis.Add(_DeleteSeriesById.FunctionName, _DeleteSeriesById);
            this.Apis.Add(_AddNewSeries.FunctionName, _AddNewSeries);
            Apis.Add(_UpdateSeries.FunctionName, _UpdateSeries);
        }

        public void FetchCurrentSeries(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            //BU,CustomerName,SeriesName
            string field = Data["Field"].ToString();
            string value = Data["Value"].ToString();

            OleExec sfcdb = null;
            DataTable dataTable = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                dataTable = new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle).GetQueryAll(field, value, sfcdb);
                
                StationReturn.Status = StationReturnStatusValue.Pass;
                //StationReturn.Message = "獲取成功";
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = ConvertToJson.DataTableToJson(dataTable);
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }
            
            
        }

        public void DeleteSeriesById(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string strid = "";
            T_C_SERIES DeleteInformation;
            Newtonsoft.Json.Linq.JArray seriesId = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                DeleteInformation = new T_C_SERIES(sfcdb, DBTYPE);
                for (int i = 0; i < seriesId.Count; i++)
                {
                    strid = seriesId[i].ToString();
                    Row_C_SERIES row = (Row_C_SERIES)DeleteInformation.GetObjByID(strid, sfcdb);
                    DeleteSql += row.GetDeleteString(DBTYPE) + ";\n";
                }
                DeleteSql = "begin\n" + DeleteSql + "end;";
                sfcdb.ExecSQL(DeleteSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                sfcdb.CommitTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

            //string seriesId = Data["SeriesId"].ToString();
            //if (string.IsNullOrEmpty(seriesId))
            //{
            //    StationReturn.Status = StationReturnStatusValue.Fail;
            //    StationReturn.MessageCode = "MES00000006";
            //    StationReturn.MessagePara = new List<Object>() { "Series" };
            //    StationReturn.Data = null;
            //    return;
            //}

            //OleExec sfcdb = DBPools["SFCDB"].Borrow();
            //Row_C_SERIES c_series = (Row_C_SERIES) new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle).NewRow();
            //string[] ids = seriesId.Split(',');
            //try
            //{
            //    sfcdb.BeginTrain();
            //    string deleteString = null;
            //    foreach (string id in ids)
            //    {
            //        deleteString = c_series.GetDeleteString(DB_TYPE_ENUM.Oracle, id);
            //        sfcdb.ExecuteNonQuery(deleteString, CommandType.Text, null);
            //    }
            //    sfcdb.CommitTrain();
            //}
            //catch (Exception ex)
            //{
            //    sfcdb.RollbackTrain();
            //    if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
            //    throw ex;
            //}
            //StationReturn.Status = StationReturnStatusValue.Pass;
            //StationReturn.MessageCode = "MES00000004";
            //StationReturn.Data = "";
            //if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
            
        }
        
        public void AddNewSeries(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            //CUSTOMER_ID,SERIES_NAME,DESCRIPTION
            if (Data == null || string.IsNullOrEmpty(Data["SERIES_NAME"].ToString()))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<Object>() { "Series" };
                StationReturn.Data = null;
                return;
            }
            OleExec sfcdb = null;
            Row_C_SERIES c_series = null;
            OleDbParameter[] paras = null;
            string sql = null;
            string customerName = Data["CUSTOMER_NAME"].ToString();
            string seriesName = Data["SERIES_NAME"].ToString();
            try
            {
                sfcdb = DBPools["SFCDB"].Borrow();
                c_series = (Row_C_SERIES)new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle).NewRow();
                //c_series.ID = GetNextId(sfcdb);//GetNewId
                c_series.ID = new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle).GetNewID(this.BU ,sfcdb);
                
                string CustomerID = new T_C_CUSTOMER(sfcdb, DB_TYPE_ENUM.Oracle).GetCustomerID(sfcdb, BU, customerName);
                if (string.IsNullOrEmpty(CustomerID))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara = new List<Object>() { "Customer" };
                    StationReturn.Data = Data["CUSTOMER_Name"].ToString();
                    if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle).isExist(sfcdb, seriesName, CustomerID))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara = new List<Object>() { "SERIES_NAME" };
                    StationReturn.Data = seriesName;
                    if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                c_series.CUSTOMER_ID = CustomerID;
                c_series.SERIES_NAME = seriesName;
                c_series.DESCRIPTION = Data["DESCRIPTION"].ToString();
                c_series.EDIT_EMP = this.LoginUser.EMP_NO;
                c_series.EDIT_TIME = this.GetDBDateTime();
                sql = c_series.GetInsertString(DB_TYPE_ENUM.Oracle);

                sfcdb.ExecuteNonQuery(sql, CommandType.Text, paras);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Data = null;
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }
            
        }

        public void UpdateSeries(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            if (Data == null)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<Object>() { "Series" };
                StationReturn.Data = null;
                return;
            }
            string serieid = Data["ID"].ToString();
            if (string.IsNullOrEmpty(serieid))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<Object>() { "SeriesID" };
                StationReturn.Data = null;
                return;
            }
            OleExec sfcdb = null;
            Row_C_SERIES c_series = null;
            string sql = null;
            string seriesName = Data["SERIES_NAME"].ToString();
            try
            {
                sfcdb = DBPools["SFCDB"].Borrow();
                c_series = (Row_C_SERIES)new T_C_SERIES(sfcdb, DB_TYPE_ENUM.Oracle).GetObjByID(serieid, sfcdb);
                if (c_series == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara = new List<Object>() { "Series" };
                    StationReturn.Data = null;
                    return;
                }
                
                string CustomerID = new T_C_CUSTOMER(sfcdb, DB_TYPE_ENUM.Oracle).GetCustomerID(sfcdb, BU, Data["CUSTOMER_NAME"].ToString());
                if (string.IsNullOrEmpty(CustomerID))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara = new List<Object>() { "Customer" };
                    StationReturn.Data = Data["CUSTOMER_Name"].ToString();
                    return;
                }
                
                c_series.ID = serieid;
                c_series.CUSTOMER_ID = CustomerID;
                c_series.SERIES_NAME = seriesName;
                c_series.DESCRIPTION = Data["DESCRIPTION"].ToString();
                c_series.EDIT_EMP = this.LoginUser.EMP_NO;
                c_series.EDIT_TIME = this.GetDBDateTime();
                sql = c_series.GetUpdateString(DB_TYPE_ENUM.Oracle);

                sfcdb.ExecuteNonQuery(sql, CommandType.Text);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Data = c_series.GetDataObject();

                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }
        }


        string GetNextId(OleExec sfcdb)
        {
            string sql = $@"select max(substr(id, 4, 8))+1 max_id from c_series ";
            string nextNum = sfcdb.ExecuteScalar(sql, CommandType.Text, null);
            //int len = nextNum.Length;
            while (nextNum.Length < 6)
            {
                nextNum = "0" + nextNum;
                //len++;
            }
            return this.BU + nextNum;
        }
    }
}
