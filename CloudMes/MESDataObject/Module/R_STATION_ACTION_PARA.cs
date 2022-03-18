using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_Station_Action_Para : DataObjectTable
    {
        public T_R_Station_Action_Para(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_Station_Action_Para(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_Station_Action_Para);
            TableName = "R_Station_Action_Para".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_Station_Action_Para> GetActionParaByInputActionID(string _ID, OleExec DB)
        {
            string strsql = "";
            List<R_Station_Action_Para> RET = new List<R_Station_Action_Para>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from R_Station_Action_Para r where r.r_input_action_id='{_ID}' order by seq_no";
                DataSet res1 = DB.ExecSelect(strsql);
                for (int i = 0; i < res1.Tables[0].Rows.Count; i++)
                {
                    Row_R_Station_Action_Para R = (Row_R_Station_Action_Para)this.NewRow();
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
        public List<R_Station_Action_Para> GetActionParaByStationActionID(string _ID, OleExec DB)
        {
            string strsql = "";
            List<R_Station_Action_Para> RET = new List<R_Station_Action_Para>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from R_Station_Action_Para r where r.r_station_action_id='{_ID}' order by seq_no";
                DataSet res1 = DB.ExecSelect(strsql);
                for (int i = 0; i < res1.Tables[0].Rows.Count; i++)
                {
                    Row_R_Station_Action_Para R = (Row_R_Station_Action_Para)this.NewRow();
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

        //public void DeleteByStationID(string RStationID, OleExec DB)
        //{
        //    string sql = string.Empty;
        //    sql = $@"delete r_station_action_para where r_station_action_id in ";
        //    sql = sql + $@"( select id from r_station_action where r_station_input_id in ";
        //    sql =sql+ $@"(select id from r_station_input where station_id='{RStationID}'))";

        //    DB.ExecSQL(sql);
        //}
    }
    public class Row_R_Station_Action_Para : DataObjectBase
    {
        public Row_R_Station_Action_Para(DataObjectInfo info) : base(info)
        {

        }
        public R_Station_Action_Para GetDataObject()
        {
            R_Station_Action_Para DataObject = new R_Station_Action_Para();
            DataObject.ID = this.ID;
            DataObject.R_STATION_ACTION_ID = this.R_STATION_ACTION_ID;
            DataObject.R_INPUT_ACTION_ID = this.R_INPUT_ACTION_ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.SESSION_TYPE = this.SESSION_TYPE;
            DataObject.SESSION_KEY = this.SESSION_KEY;
            DataObject.VALUE = this.VALUE;
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
        public string R_STATION_ACTION_ID
        {
            get
            {
                return (string)this["R_STATION_ACTION_ID"];
            }
            set
            {
                this["R_STATION_ACTION_ID"] = value;
            }
        }
        public string R_INPUT_ACTION_ID
        {
            get
            {
                return (string)this["R_INPUT_ACTION_ID"];
            }
            set
            {
                this["R_INPUT_ACTION_ID"] = value;
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
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
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
    public class R_Station_Action_Para
    {
        public string ID{get;set;}
        public string R_STATION_ACTION_ID{get;set;}
        public string R_INPUT_ACTION_ID{get;set;}
        public double? SEQ_NO{get;set;}
        public string SESSION_TYPE{get;set;}
        public string SESSION_KEY{get;set;}
        public string VALUE{get;set;}
        public DateTime EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}