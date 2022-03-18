using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BACKFLUSH_HISTORY : DataObjectTable
    {
        public T_R_BACKFLUSH_HISTORY(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BACKFLUSH_HISTORY(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BACKFLUSH_HISTORY);
            TableName = "R_BACKFLUSH_HISTORY".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int Add(R_BACKFLUSH_HISTORY NewHis, OleExec sfcdb)
        {
            Row_R_BACKFLUSH_HISTORY NewHisRow = (Row_R_BACKFLUSH_HISTORY)NewRow();
            NewHisRow.WORKORDERNO = NewHis.WORKORDERNO;
            NewHisRow.SKUNO = NewHis.SKUNO;
            NewHisRow.SAP_STATION = NewHis.SAP_STATION;
            NewHisRow.WORKORDER_QTY = NewHis.WORKORDER_QTY;
            NewHisRow.BACKFLUSH_QTY = NewHis.BACKFLUSH_QTY;
            NewHisRow.SFC_QTY = NewHis.SFC_QTY;
            NewHisRow.DIFF_QTY = NewHis.DIFF_QTY;
            NewHisRow.SFC_STATION = NewHis.SFC_STATION;
            NewHisRow.HAND_QTY = NewHis.HAND_QTY;
            NewHisRow.LAST_SFC_QTY = NewHis.LAST_SFC_QTY;
            NewHisRow.DIFF_QTY1 = NewHis.DIFF_QTY1;
            NewHisRow.DIFF_QTY2 = NewHis.DIFF_QTY2;
            NewHisRow.HISTORY_HAND_QTY = NewHis.HISTORY_HAND_QTY;
            NewHisRow.MRB_QTY = NewHis.MRB_QTY;
            NewHisRow.REC_TYPE = NewHis.REC_TYPE;
            NewHisRow.BACK_DATE = NewHis.BACK_DATE;
            NewHisRow.RESULT = NewHis.RESULT;
            NewHisRow.TIMES = NewHis.TIMES;
            NewHisRow.TOSAP = NewHis.TOSAP;
            int result = sfcdb.ExecuteNonQuery(NewHisRow.GetInsertString(DBType), CommandType.Text);
            return result;
        }
        public DataSet GetBackFlushData(string wono,string sapstation,OleExec sfcdb)
        {
            string sqlString = $@"select nvl(sum(r.diff_qty),0) C  
                             from r_backflush_history r
                             where r.workorderno = '{wono}'
                             and r.sap_station = '{sapstation}'
                             and r.result='Y'";
            DataSet dtset=sfcdb.ExecuteDataSet(sqlString,CommandType.Text);
            return dtset;
        }
    }
    public class Row_R_BACKFLUSH_HISTORY : DataObjectBase
    {
        public Row_R_BACKFLUSH_HISTORY(DataObjectInfo info) : base(info)
        {

        }
        public R_BACKFLUSH_HISTORY GetDataObject()
        {
            R_BACKFLUSH_HISTORY DataObject = new R_BACKFLUSH_HISTORY();
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SAP_STATION = this.SAP_STATION;
            DataObject.WORKORDER_QTY = this.WORKORDER_QTY;
            DataObject.BACKFLUSH_QTY = this.BACKFLUSH_QTY;
            DataObject.SFC_QTY = this.SFC_QTY;
            DataObject.DIFF_QTY = this.DIFF_QTY;
            DataObject.SFC_STATION = this.SFC_STATION;
            DataObject.HAND_QTY = this.HAND_QTY;
            DataObject.LAST_SFC_QTY = this.LAST_SFC_QTY;
            DataObject.DIFF_QTY1 = this.DIFF_QTY1;
            DataObject.DIFF_QTY2 = this.DIFF_QTY2;
            DataObject.HISTORY_HAND_QTY = this.HISTORY_HAND_QTY;
            DataObject.MRB_QTY = this.MRB_QTY;
            DataObject.REC_TYPE = this.REC_TYPE;
            DataObject.BACK_DATE = this.BACK_DATE;
            DataObject.RESULT = this.RESULT;
            DataObject.TIMES = this.TIMES;
            DataObject.TOSAP = this.TOSAP;
            return DataObject;
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
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
        public string SAP_STATION
        {
            get
            {
                return (string)this["SAP_STATION"];
            }
            set
            {
                this["SAP_STATION"] = value;
            }
        }
        public double? WORKORDER_QTY
        {
            get
            {
                return (double?)this["WORKORDER_QTY"];
            }
            set
            {
                this["WORKORDER_QTY"] = value;
            }
        }
        public double? BACKFLUSH_QTY
        {
            get
            {
                return (double?)this["BACKFLUSH_QTY"];
            }
            set
            {
                this["BACKFLUSH_QTY"] = value;
            }
        }
        public double? SFC_QTY
        {
            get
            {
                return (double?)this["SFC_QTY"];
            }
            set
            {
                this["SFC_QTY"] = value;
            }
        }
        public double? DIFF_QTY
        {
            get
            {
                return (double?)this["DIFF_QTY"];
            }
            set
            {
                this["DIFF_QTY"] = value;
            }
        }
        public string SFC_STATION
        {
            get
            {
                return (string)this["SFC_STATION"];
            }
            set
            {
                this["SFC_STATION"] = value;
            }
        }
        public double? HAND_QTY
        {
            get
            {
                return (double?)this["HAND_QTY"];
            }
            set
            {
                this["HAND_QTY"] = value;
            }
        }
        public double? LAST_SFC_QTY
        {
            get
            {
                return (double?)this["LAST_SFC_QTY"];
            }
            set
            {
                this["LAST_SFC_QTY"] = value;
            }
        }
        public double? DIFF_QTY1
        {
            get
            {
                return (double?)this["DIFF_QTY1"];
            }
            set
            {
                this["DIFF_QTY1"] = value;
            }
        }
        public double? DIFF_QTY2
        {
            get
            {
                return (double?)this["DIFF_QTY2"];
            }
            set
            {
                this["DIFF_QTY2"] = value;
            }
        }
        public double? HISTORY_HAND_QTY
        {
            get
            {
                return (double?)this["HISTORY_HAND_QTY"];
            }
            set
            {
                this["HISTORY_HAND_QTY"] = value;
            }
        }
        public double? MRB_QTY
        {
            get
            {
                return (double?)this["MRB_QTY"];
            }
            set
            {
                this["MRB_QTY"] = value;
            }
        }
        public string REC_TYPE
        {
            get
            {
                return (string)this["REC_TYPE"];
            }
            set
            {
                this["REC_TYPE"] = value;
            }
        }
        public DateTime? BACK_DATE
        {
            get
            {
                return (DateTime?)this["BACK_DATE"];
            }
            set
            {
                this["BACK_DATE"] = value;
            }
        }
        public string RESULT
        {
            get
            {
                return (string)this["RESULT"];
            }
            set
            {
                this["RESULT"] = value;
            }
        }
        public double? TIMES
        {
            get
            {
                return (double?)this["TIMES"];
            }
            set
            {
                this["TIMES"] = value;
            }
        }
        public string TOSAP
        {
            get
            {
                return (string)this["TOSAP"];
            }
            set
            {
                this["TOSAP"] = value;
            }
        }
    }
    public class R_BACKFLUSH_HISTORY
    {
        public string WORKORDERNO{get;set;}
        public string SKUNO{get;set;}
        public string SAP_STATION{get;set;}
        public double? WORKORDER_QTY{get;set;}
        public double? BACKFLUSH_QTY{get;set;}
        public double? SFC_QTY{get;set;}
        public double? DIFF_QTY{get;set;}
        public string SFC_STATION{get;set;}
        public double? HAND_QTY{get;set;}
        public double? LAST_SFC_QTY{get;set;}
        public double? DIFF_QTY1{get;set;}
        public double? DIFF_QTY2{get;set;}
        public double? HISTORY_HAND_QTY{get;set;}
        public double? MRB_QTY{get;set;}
        public string REC_TYPE{get;set;}
        public DateTime? BACK_DATE{get;set;}
        public string RESULT{get;set;}
        public double? TIMES{get;set;}
        public string TOSAP{get;set;}
    }
}