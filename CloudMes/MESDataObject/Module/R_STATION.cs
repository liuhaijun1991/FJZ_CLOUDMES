using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_Station : DataObjectTable
    {
        public T_R_Station(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_Station(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_Station);
            TableName = "R_Station".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public Row_R_Station GetRowByDisplayName(string DisplayName, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from {TableName} where Display_Station_Name = '{DisplayName.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "Station" + DisplayName });
                    throw new MESReturnMessage(errMsg);
                }
                Row_R_Station R = (Row_R_Station)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public bool CheckDataExist(string DisplayStationName, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where Display_Station_Name ='{DisplayStationName}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }
        public bool CheckDataExistByID(string ID, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where ID ='{ID}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }
        /// <summary>
        /// 用于刪除工站，即刪除所有的工站相關表數據
        /// </summary>
        /// <param name="RStationID">工站ID</param>
        /// <param name="DB">DB對象</param>
        public void DeleteByRStationID(string RStationID, OleExec DB)
        {
            String sql = "";
            sql = $@"DELETE R_Station_Output where  R_STATION_ID='{RStationID}' ";
            DB.ExecSQL(sql);

            sql = $@"delete r_station_action_para where r_station_action_id in ";
            sql = sql + $@"( select id from r_station_action where r_station_input_id in ";
            sql = sql + $@"(select id from r_station_input where station_id='{RStationID}'))";
            DB.ExecSQL(sql);

            sql = $@"delete r_station_action where r_station_input_id in ";
            sql = sql + $@"(select id from r_station_input where station_id='{RStationID}') ";
            DB.ExecSQL(sql);

            sql = $@"delete r_station_input where station_id='{RStationID}'";
            DB.ExecSQL(sql);

            sql = $@"DELETE R_Station WHERE ID='{RStationID}'";
            DB.ExecSQL(sql);

        }

        public List<R_Station> QueryFailStation(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_Station> stationaction = new List<R_Station>();
            sql = $@"SELECT * FROM r_Station where  fail_station_flag=1 ";

            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                stationaction.Add(CreatecstationactionClass(dr));
            }
            return stationaction;
        }
    

        public List<R_Station> Queryrstation(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_Station> stationaction = new List<R_Station>();
            sql = $@"SELECT * FROM r_Station ORDER BY STATION_NAME";

            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                stationaction.Add(CreatecstationactionClass(dr));
            }
            return stationaction;
        }      
        public R_Station CreatecstationactionClass(DataRow dr)
        {
            Row_R_Station row = (Row_R_Station)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        public bool StationIsExist(OleExec DB, string stationName)
        {
            return DB.ORM.Queryable<R_Station>().Where(r => r.STATION_NAME == stationName).Any();
        }
    }
    public class Row_R_Station : DataObjectBase
    {
        public Row_R_Station(DataObjectInfo info) : base(info)
        {

        }
        public R_Station GetDataObject()
        {
            R_Station DataObject = new R_Station();
            DataObject.ID = this.ID;
            DataObject.DISPLAY_STATION_NAME = this.DISPLAY_STATION_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.FAIL_STATION_ID = this.FAIL_STATION_ID;
            DataObject.FAIL_STATION_FLAG = this.FAIL_STATION_FLAG;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string DISPLAY_STATION_NAME
        {
            get
            {
                return (string)this["DISPLAY_STATION_NAME"];
            }
            set
            {
                this["DISPLAY_STATION_NAME"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string FAIL_STATION_ID
        {
            get
            {
                return (string)this["FAIL_STATION_ID"];
            }
            set
            {
                this["FAIL_STATION_ID"] = value;
            }
        }
        public double? FAIL_STATION_FLAG
        {
            get
            {
                return (double?)this["FAIL_STATION_FLAG"];
            }
            set
            {
                this["FAIL_STATION_FLAG"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
    }
    public class R_Station
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{get;set;}
        public string DISPLAY_STATION_NAME{get;set;}
        public string STATION_NAME{get;set;}
        public string FAIL_STATION_ID{get;set;}
        public double? FAIL_STATION_FLAG{get;set;}
        public DateTime EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}