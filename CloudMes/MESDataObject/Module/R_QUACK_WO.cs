using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_WO : DataObjectTable
    {
        public T_R_QUACK_WO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_WO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_WO);
            TableName = "R_QUACK_WO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// ZC 根據QUACK_DATE,BU_NAME DEL R_QUACK_WO表中數據
        /// </summary>
        /// <param name="QUACK_DATE"></param>
        /// <param name="BU_NAME"></param>
        /// <returns></returns>
        public string DelByBuName(string QUACK_DATE,string BU_NAME,OleExec DB)
        {
            // DateTime datatime = Convert.ToDateTime(QUACK_DATE);
            //string sql= $@"DELETE FROM R_QUACK_WO  WHERE QUACK_DATE=TO_DATE('{QUACK_DATE}','yyyy/mm/dd') AND BU_NAME='{BU_NAME}';";
            //string  result = DB.ExecSQL(sql);
            //return result;
            Row_R_QUACK_WO row = null;
            DataTable dt = new DataTable();
            string DeleteString = string.Empty;
            string result = string.Empty;
            string sql = $@"SELECT * FROM R_QUACK_WO  WHERE QUACK_DATE=TO_DATE('{QUACK_DATE}','yyyy/mm/dd') AND  BU_NAME='{BU_NAME}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
               foreach(DataRow dr in dt.Rows)
                {
                    row = (Row_R_QUACK_WO)NewRow();
                    row.loadData(dr);
                    DeleteString = row.GetDeleteString(DBType);
                    result=DB.ExecSQL(DeleteString);
                }
            }
            else
            {
                result = "0";
            }
            return result;

        }
    
        public void InsertRow(Dictionary<string, object> rw,string bigBu, OleExec DB)
        {
            string result = string.Empty;
            string insertString = string.Empty;
            Row_R_QUACK_WO row = null;
            row = (Row_R_QUACK_WO)NewRow();
            row.ID=GetNewID(bigBu, DB);
            row.WO = rw["WO"].ToString();
            row.SKUNO = rw["SKUNO"].ToString();
            row.CODE_NAME = rw["CODENAME"].ToString();
            row.SHIFT_1_QTY = Convert.ToDouble(rw["SHIFT_1_QTY"]);
            row.SHIFT_2_QTY = Convert.ToDouble(rw["SHIFT_2_QTY"]);
            row.WO_QTY = int.Parse(rw["WO_QTY"].ToString());
            row.QUACK_DATE= Convert.ToDateTime(rw["QUACK_DATE"].ToString());            
            row.BU_NAME = rw["BUNAME"].ToString();
            insertString = row.GetInsertString(DBType);
            result = DB.ExecSQL(insertString);
           // return result;
        }

    }

    public class Row_R_QUACK_WO : DataObjectBase
    {
        public Row_R_QUACK_WO(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_WO GetDataObject()
        {
            R_QUACK_WO DataObject = new R_QUACK_WO();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CODE_NAME = this.CODE_NAME;
            DataObject.SHIFT_1_QTY = this.SHIFT_1_QTY;
            DataObject.SHIFT_2_QTY = this.SHIFT_2_QTY;
            DataObject.WO_QTY = this.WO_QTY;
            DataObject.STATION_CODE = this.STATION_CODE;
            DataObject.QUACK_DATE = this.QUACK_DATE;
            DataObject.BU_NAME = this.BU_NAME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string CODE_NAME
        {
            get
            {
                return (string)this["CODE_NAME"];
            }
            set
            {
                this["CODE_NAME"] = value;
            }
        }
        public double? SHIFT_1_QTY
        {
            get
            {
                return (double?)this["SHIFT_1_QTY"];
            }
            set
            {
                this["SHIFT_1_QTY"] = value;
            }
        }
        public double? SHIFT_2_QTY
        {
            get
            {
                return (double?)this["SHIFT_2_QTY"];
            }
            set
            {
                this["SHIFT_2_QTY"] = value;
            }
        }
        public double? WO_QTY
        {
            get
            {
                return (double?)this["WO_QTY"];
            }
            set
            {
                this["WO_QTY"] = value;
            }
        }
        public string STATION_CODE
        {
            get
            {
                return (string)this["STATION_CODE"];
            }
            set
            {
                this["STATION_CODE"] = value;
            }
        }
        public DateTime? QUACK_DATE
        {
            get
            {
                return (DateTime?)this["QUACK_DATE"];
            }
            set
            {
                this["QUACK_DATE"] = value;
            }
        }
        public string BU_NAME
        {
            get
            {
                return (string)this["BU_NAME"];
            }
            set
            {
                this["BU_NAME"] = value;
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
    }
    public class R_QUACK_WO
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string SKUNO { get; set; }
        public string CODE_NAME { get; set; }
        public double? SHIFT_1_QTY { get; set; }
        public double? SHIFT_2_QTY { get; set; }
        public double? WO_QTY { get; set; }
        public string STATION_CODE { get; set; }
        public DateTime? QUACK_DATE { get; set; }
        public string BU_NAME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}