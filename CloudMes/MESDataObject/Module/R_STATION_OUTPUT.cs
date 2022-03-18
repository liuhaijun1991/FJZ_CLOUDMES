using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_Station_Output : DataObjectTable
    {
        public T_R_Station_Output(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_Station_Output(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_Station_Output);
            TableName = "R_Station_Output".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_Station_Output> GetStationOutputByStationID(string _StationID, OleExec DB,bool Retr)
        {
            string strsql = "";
            List<R_Station_Output> RET = new List<R_Station_Output>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from R_Station_Output where R_station_id = '{_StationID}' order by SEQ_NO";
                DataSet res1 = DB.ExecSelect(strsql);
                for (int i = 0; i < res1.Tables[0].Rows.Count; i++)
                {
                    Row_R_Station_Output R = (Row_R_Station_Output)this.NewRow();
                    R.loadData(res1.Tables[0].Rows[i]);
                    RET.Add(R.GetDataObject());
                }

                return RET;

                //Row_R_Station R = (Row_R_Station)this.GetObjByID(ID, DB);
                //return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public List<Row_R_Station_Output> GetStationOutputByStationID(string _StationID, OleExec DB)
        {
            string strsql = "";
            List<Row_R_Station_Output> RET = new List<Row_R_Station_Output>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from R_Station_Output where R_station_id = '{_StationID}' order by SEQ_NO";
                DataSet res1 = DB.ExecSelect(strsql);
                for (int i = 0; i < res1.Tables[0].Rows.Count; i++)
                {
                    Row_R_Station_Output R = (Row_R_Station_Output)this.NewRow();
                    R.loadData(res1.Tables[0].Rows[i]);
                    RET.Add(R);
                }

                return RET;

                //Row_R_Station R = (Row_R_Station)this.GetObjByID(ID, DB);
                //return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public bool CheckDataExist(string RStationID,string Name, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where  R_STATION_ID='{RStationID}' and name='{Name}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public bool CheckDataExistByid(string ID, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where  ID='{ID}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }

        //public void DeleteByRStationID(string RStationID, OleExec DB)
        //{
        //    string sql = string.Empty;
        //    sql = $@"select ID from {TableName} where  R_STATION_ID='{RStationID}' ";
        //    DB.ExecSQL(sql);
        //}
    }

    
    public class Row_R_Station_Output : DataObjectBase
    {
        public Row_R_Station_Output(DataObjectInfo info) : base(info)
        {

        }
        public R_Station_Output GetDataObject()
        {
            R_Station_Output DataObject = new R_Station_Output();
            DataObject.ID = this.ID;
            DataObject.R_STATION_ID = this.R_STATION_ID;
            DataObject.NAME = this.NAME;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.SESSION_TYPE = this.SESSION_TYPE;
            DataObject.SESSION_KEY = this.SESSION_KEY;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.DISPLAY_TYPE = this.DISPLAY_TYPE;
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
        public string R_STATION_ID
        {
            get
            {
                return (string)this["R_STATION_ID"];
            }
            set
            {
                this["R_STATION_ID"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string SESSION_TYPE
        {
            get
            {
                return (string)this["SESSION_TYPE"];
            }
            set
            {
                this["SESSION_TYPE"] = value;
            }
        }
        public string SESSION_KEY
        {
            get
            {
                return (string)this["SESSION_KEY"];
            }
            set
            {
                this["SESSION_KEY"] = value;
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
        public string DISPLAY_TYPE
        {
            get
            {
                return (string)this["DISPLAY_TYPE"];
            }
            set
            {
                this["DISPLAY_TYPE"] = value;
            }
        }
    }
    public class R_Station_Output
    {
        public string ID{get;set;}
        public string R_STATION_ID{get;set;}
        public string NAME{get;set;}
        public double? SEQ_NO{get;set;}
        public string SESSION_TYPE{get;set;}
        public string SESSION_KEY{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string DISPLAY_TYPE{get;set;}
    }
}