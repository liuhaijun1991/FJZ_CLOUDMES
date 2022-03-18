
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_JNP_LINE_STOCK_DETAIL : DataObjectTable
    {
        public T_R_JNP_LINE_STOCK_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JNP_LINE_STOCK_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JNP_LINE_STOCK_DETAIL);
            TableName = "R_JNP_LINE_STOCK_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JNP_LINE_STOCK_DETAIL : DataObjectBase
    {
        public Row_R_JNP_LINE_STOCK_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_JNP_LINE_STOCK_DETAIL GetDataObject()
        {
            R_JNP_LINE_STOCK_DETAIL DataObject = new R_JNP_LINE_STOCK_DETAIL();
            DataObject.ID = this.ID;
            DataObject.STOCK_LOCATION = this.STOCK_LOCATION;
            DataObject.PN = this.PN;
            DataObject.QTY = this.QTY;
            DataObject.OPTION_TYPE = this.OPTION_TYPE;
            DataObject.FROM_LOCATION = this.FROM_LOCATION;
            DataObject.TO_LOCATION = this.TO_LOCATION;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.DETAIL1 = this.DETAIL1;
            DataObject.DETAIL2 = this.DETAIL2;
            DataObject.DETAIL3 = this.DETAIL3;
            DataObject.DETAIL4 = this.DETAIL4;
            DataObject.DETAIL5 = this.DETAIL5;
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
        public string STOCK_LOCATION
        {
            get
            {
                return (string)this["STOCK_LOCATION"];
            }
            set
            {
                this["STOCK_LOCATION"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public double? OPTION_TYPE
        {
            get
            {
                return (double?)this["OPTION_TYPE"];
            }
            set
            {
                this["OPTION_TYPE"] = value;
            }
        }
        public string FROM_LOCATION
        {
            get
            {
                return (string)this["FROM_LOCATION"];
            }
            set
            {
                this["FROM_LOCATION"] = value;
            }
        }
        public string TO_LOCATION
        {
            get
            {
                return (string)this["TO_LOCATION"];
            }
            set
            {
                this["TO_LOCATION"] = value;
            }
        }
        public double? SAP_FLAG
        {
            get
            {
                return (double?)this["SAP_FLAG"];
            }
            set
            {
                this["SAP_FLAG"] = value;
            }
        }
        public string DETAIL1
        {
            get
            {
                return (string)this["DETAIL1"];
            }
            set
            {
                this["DETAIL1"] = value;
            }
        }
        public string DETAIL2
        {
            get
            {
                return (string)this["DETAIL2"];
            }
            set
            {
                this["DETAIL2"] = value;
            }
        }
        public string DETAIL3
        {
            get
            {
                return (string)this["DETAIL3"];
            }
            set
            {
                this["DETAIL3"] = value;
            }
        }
        public string DETAIL4
        {
            get
            {
                return (string)this["DETAIL4"];
            }
            set
            {
                this["DETAIL4"] = value;
            }
        }
        public string DETAIL5
        {
            get
            {
                return (string)this["DETAIL5"];
            }
            set
            {
                this["DETAIL5"] = value;
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
    public class R_JNP_LINE_STOCK_DETAIL
    {
        public string ID { get; set; }
        public string STOCK_LOCATION { get; set; }
        public string PN { get; set; }
        public double? QTY { get; set; }
        //0（發料）;1（消耗）;2（工單取消）;3（產線退料）
        public double? OPTION_TYPE { get; set; }
        public string FROM_LOCATION { get; set; }
        public string TO_LOCATION { get; set; }
        public double? SAP_FLAG { get; set; }
        public string DETAIL1 { get; set; }
        public string DETAIL2 { get; set; }
        public string DETAIL3 { get; set; }
        public string DETAIL4 { get; set; }
        public string DETAIL5 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}

