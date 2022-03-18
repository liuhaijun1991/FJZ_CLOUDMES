using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BACKFLUSH_CHECK : DataObjectTable
    {
        public T_R_BACKFLUSH_CHECK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BACKFLUSH_CHECK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BACKFLUSH_CHECK);
            TableName = "R_BACKFLUSH_CHECK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public DataSet GetTableConstruct(OleExec sfcdb)
        {
            string strSQL = "select * from r_backflush_check where 1<1";
            DataSet dtset = sfcdb.ExecuteDataSet(strSQL, CommandType.Text);
            return dtset;
        }
    }
    public class Row_R_BACKFLUSH_CHECK : DataObjectBase
    {
        public Row_R_BACKFLUSH_CHECK(DataObjectInfo info) : base(info)
        {

        }
        public R_BACKFLUSH_CHECK GetDataObject()
        {
            R_BACKFLUSH_CHECK DataObject = new R_BACKFLUSH_CHECK();
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
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SAP_STATION = this.SAP_STATION;
            DataObject.WORKORDER_QTY = this.WORKORDER_QTY;
            return DataObject;
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
    }
    public class R_BACKFLUSH_CHECK
    {
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
        public string WORKORDERNO{get;set;}
        public string SKUNO{get;set;}
        public string SAP_STATION{get;set;}
        public double? WORKORDER_QTY{get;set;}
    }
}
