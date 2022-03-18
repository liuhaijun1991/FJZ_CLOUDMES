using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_Input_Action : DataObjectTable
    {
        public T_R_Input_Action(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_Input_Action(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_Input_Action);
            TableName = "R_Input_Action".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_Input_Action> GetActionByInputID(string _InputID,  OleExec DB)
        {
            string strsql = "";
            List<R_Input_Action> RET = new List<R_Input_Action>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from R_Input_Action r where r.input_id = '{_InputID}' ";
                DataSet res1 = DB.ExecSelect(strsql);
                for (int i = 0; i < res1.Tables[0].Rows.Count; i++)
                {
                    Row_R_Input_Action R = (Row_R_Input_Action)this.NewRow();
                    R.loadData(res1.Tables[0].Rows[i]);
                    RET.Add(R.GetDataObject());
                }
                return RET;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public bool CheckDataExist(string InputID,string StationActionID, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select ID from {TableName} where INPUT_ID ='{InputID}' and C_STATION_ACTION_ID='{StationActionID}'" ;
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

        public List<R_Input_Action> QueryInput(string ID, string InputID, string StationActionID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_Input_Action> inputaction = new List<R_Input_Action>();
            sql = $@"SELECT * FROM R_Input_Action where 1=1 ";
            if (ID != "")
            {
                sql = sql + $@" AND ID = '{ID}'";
            }
            if (InputID != "")
            {
                sql = sql + $@" AND INPUT_ID = '{InputID}'";
            }

            if (StationActionID != "")
            {
                sql = sql + $@" AND C_STATION_ACTION_ID = '{StationActionID}'";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                inputaction.Add(CreateInputActionClass(dr));
            }
            return inputaction;
        }

        public DataTable QueryInput( string InputID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_Input_Action> inputaction = new List<R_Input_Action>();
            sql = $@"select * from  R_Input_Action a, C_Station_Action  b where a.C_STATION_ACTION_ID=b.id and  input_id='{InputID}'  ";

            dt = DB.ExecSelect(sql).Tables[0];
            return dt;
        }
        public R_Input_Action CreateInputActionClass(DataRow dr)
        {
            Row_R_Input_Action row = (Row_R_Input_Action)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        public void DeleteInputActionByInputID(string InputID, OleExec DB)
        {
            string sql = string.Empty;
            sql = $@"DELETE   R_Input_Action where INPUT_ID='{InputID}' ";
            DB.ExecSQL(sql);
        }
    }
    public class Row_R_Input_Action : DataObjectBase
    {
        public Row_R_Input_Action(DataObjectInfo info) : base(info)
        {

        }
        public R_Input_Action GetDataObject()
        {
            R_Input_Action DataObject = new R_Input_Action();
            DataObject.ID = this.ID;
            DataObject.INPUT_ID = this.INPUT_ID;
            DataObject.C_STATION_ACTION_ID = this.C_STATION_ACTION_ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.CONFIG_TYPE = this.CONFIG_TYPE;
            DataObject.CONFIG_VALUE = this.CONFIG_VALUE;
            DataObject.ADD_FLAG = this.ADD_FLAG;
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
        public string INPUT_ID
        {
            get
            {
                return (string)this["INPUT_ID"];
            }
            set
            {
                this["INPUT_ID"] = value;
            }
        }
        public string C_STATION_ACTION_ID
        {
            get
            {
                return (string)this["C_STATION_ACTION_ID"];
            }
            set
            {
                this["C_STATION_ACTION_ID"] = value;
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
        public string CONFIG_TYPE
        {
            get
            {
                return (string)this["CONFIG_TYPE"];
            }
            set
            {
                this["CONFIG_TYPE"] = value;
            }
        }
        public string CONFIG_VALUE
        {
            get
            {
                return (string)this["CONFIG_VALUE"];
            }
            set
            {
                this["CONFIG_VALUE"] = value;
            }
        }
        public double? ADD_FLAG
        {
            get
            {
                return (double?)this["ADD_FLAG"];
            }
            set
            {
                this["ADD_FLAG"] = value;
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
    public class R_Input_Action : IComparer<R_Input_Action>, IComparable<R_Input_Action>
    {
        public string ID{get;set;}
        public string INPUT_ID{get;set;}
        public string C_STATION_ACTION_ID{get;set;}
        public double? SEQ_NO{get;set;}
        public string CONFIG_TYPE{get;set;}
        public string CONFIG_VALUE{get;set;}
        public double? ADD_FLAG{get;set;}
        public DateTime EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public int Compare(R_Input_Action x, R_Input_Action y)
        {
            if (x.SEQ_NO > y.SEQ_NO)
            {
                return 1;
            }
            else if (x.SEQ_NO == y.SEQ_NO)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public int CompareTo(R_Input_Action other)
        {
            //if (SEQ_NO == other.SEQ_NO)
            if (SEQ_NO > other.SEQ_NO)
            {
                return 1;
            }
            else if (SEQ_NO == other.SEQ_NO)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}