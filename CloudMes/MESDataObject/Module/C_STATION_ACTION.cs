using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_c_station_action : DataObjectTable
    {
        public T_c_station_action(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_c_station_action(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_c_station_action);
            TableName = "c_station_action".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckDataExist(string FuntionNmae, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where FUNCTION_NAME ='{FuntionNmae}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public List<c_station_action> Querycstationaction(string ID, string ActionType, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_station_action> stationaction = new List<c_station_action>();
            sql = $@"SELECT * FROM C_Station_Action where 1=1 ";
            if (ID != "")
            {
                sql = sql + $@" AND ID = '{ID}'";
            }
            if (ActionType != "")
            {
                sql = sql + $@" AND ACTION_TYPE = '{ActionType}'";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                stationaction.Add(CreatecstationactionClass(dr));
            }
            return stationaction;
        }
        public c_station_action CreatecstationactionClass(DataRow dr)
        {
            Row_c_station_action row = (Row_c_station_action)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        public c_station_action GetActionByID(string ID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            c_station_action stationaction = new c_station_action();
            sql = $@"SELECT * FROM C_Station_Action where ID='{ID}' AND ROWNUM=1 ";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                stationaction=CreatecstationactionClass(dr);
            }
            return stationaction;
        }


    }
    public class Row_c_station_action : DataObjectBase
    {
        public Row_c_station_action(DataObjectInfo info) : base(info)
        {

        }
        public c_station_action GetDataObject()
        {
            c_station_action DataObject = new c_station_action();
            DataObject.ID = this.ID;
            DataObject.ACTION_TYPE = this.ACTION_TYPE;
            DataObject.ACTION_NAME = this.ACTION_NAME;
            DataObject.DLL_NAME = this.DLL_NAME;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.FUNCTION_NAME = this.FUNCTION_NAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.BU = this.BU;
            DataObject.USE_STATION = this.USE_STATION;
            DataObject.SORTING = this.SORTING;
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
        public string ACTION_TYPE
        {
            get
            {
                return (string)this["ACTION_TYPE"];
            }
            set
            {
                this["ACTION_TYPE"] = value;
            }
        }
        public string ACTION_NAME
        {
            get
            {
                return (string)this["ACTION_NAME"];
            }
            set
            {
                this["ACTION_NAME"] = value;
            }
        }
        public string DLL_NAME
        {
            get
            {
                return (string)this["DLL_NAME"];
            }
            set
            {
                this["DLL_NAME"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string FUNCTION_NAME
        {
            get
            {
                return (string)this["FUNCTION_NAME"];
            }
            set
            {
                this["FUNCTION_NAME"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
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

        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string SORTING
        {
            get
            {
                return (string)this["SORTING"];
            }
            set
            {
                this["SORTING"] = value;
            }
        }
        public string USE_STATION
        {
            get
            {
                return (string)this["USE_STATION"];
            }
            set
            {
                this["USE_STATION"] = value;
            }
        }


    }
    public class c_station_action
    {
        public string ID{get;set;}
        public string ACTION_TYPE{get;set;}
        public string ACTION_NAME{get;set;}
        public string DLL_NAME{get;set;}
        public string CLASS_NAME{get;set;}
        public string FUNCTION_NAME{get;set;}
        public string DESCRIPTION{get;set;}
        public string BU{get;set;}
        public string SORTING{get;set;}
        public string USE_STATION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}