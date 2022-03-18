using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_JNP_CREM : DataObjectTable
    {
        public T_R_JNP_CREM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JNP_CREM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JNP_CREM);
            TableName = "R_JNP_CREM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JNP_CREM : DataObjectBase
    {
        public Row_R_JNP_CREM(DataObjectInfo info) : base(info)
        {

        }
        public R_JNP_CREM GetDataObject()
        {
            R_JNP_CREM DataObject = new R_JNP_CREM();
            DataObject.ID = this.ID;
            DataObject.TCCODE = this.TCCODE;
            DataObject.TC_NUMBER = this.TC_NUMBER;
            DataObject.PLANT = this.PLANT;
            DataObject.PLANTCODE = this.PLANTCODE;
            DataObject.ITEM_NUMBER = this.ITEM_NUMBER;
            DataObject.BU_NAME = this.BU_NAME;
            DataObject.ROHS_FLAG = this.ROHS_FLAG;
            DataObject.STARTDATE = this.STARTDATE;
            DataObject.QTY = this.QTY;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string TCCODE
        {
            get
            {
                return (string)this["TCCODE"];
            }
            set
            {
                this["TCCODE"] = value;
            }
        }
        public string TC_NUMBER
        {
            get
            {
                return (string)this["TC_NUMBER"];
            }
            set
            {
                this["TC_NUMBER"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string PLANTCODE
        {
            get
            {
                return (string)this["PLANTCODE"];
            }
            set
            {
                this["PLANTCODE"] = value;
            }
        }
        public string ITEM_NUMBER
        {
            get
            {
                return (string)this["ITEM_NUMBER"];
            }
            set
            {
                this["ITEM_NUMBER"] = value;
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
        public string ROHS_FLAG
        {
            get
            {
                return (string)this["ROHS_FLAG"];
            }
            set
            {
                this["ROHS_FLAG"] = value;
            }
        }
        public string STARTDATE
        {
            get
            {
                return (string)this["STARTDATE"];
            }
            set
            {
                this["STARTDATE"] = value;
            }
        }
        public string QTY
        {
            get
            {
                return (string)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
    }
    public class R_JNP_CREM
    {
        public string ID { get; set; }
        public string TCCODE { get; set; }
        public string TC_NUMBER { get; set; }
        public string PLANT { get; set; }
        public string PLANTCODE { get; set; }
        public string ITEM_NUMBER { get; set; }
        public string BU_NAME { get; set; }
        public string ROHS_FLAG { get; set; }
        public string STARTDATE { get; set; }
        public string QTY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}