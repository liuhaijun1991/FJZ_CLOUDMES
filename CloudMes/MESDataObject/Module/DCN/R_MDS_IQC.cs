using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MDS_IQC : DataObjectTable
    {
        public T_R_MDS_IQC(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MDS_IQC(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MDS_IQC);
            TableName = "R_MDS_IQC".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MDS_IQC : DataObjectBase
    {
        public Row_R_MDS_IQC(DataObjectInfo info) : base(info)
        {

        }
        public R_MDS_IQC GetDataObject()
        {
            R_MDS_IQC DataObject = new R_MDS_IQC();
            DataObject.ID = this.ID;
            DataObject.DATAPOINT = this.DATAPOINT;
            DataObject.RECORD_CREATION_DATE = this.RECORD_CREATION_DATE;
            DataObject.CM_CODE = this.CM_CODE;
            DataObject.INSPECTION_DATE = this.INSPECTION_DATE;
            DataObject.REJECTCODE = this.REJECTCODE;
            DataObject.PARTNO = this.PARTNO;
            DataObject.MPN = this.MPN;
            DataObject.MANUFACTURER = this.MANUFACTURER;
            DataObject.LOTNO = this.LOTNO;
            DataObject.DATECODE = this.DATECODE;
            DataObject.LOTCODE = this.LOTCODE;
            DataObject.RECEIVEQTY = this.RECEIVEQTY;
            DataObject.SAMPLESIZE = this.SAMPLESIZE;
            DataObject.ACCEPTQTY = this.ACCEPTQTY;
            DataObject.REJECTQTY = this.REJECTQTY;
            DataObject.AVLSTATUS = this.AVLSTATUS;
            DataObject.FIRSTINCOMING = this.FIRSTINCOMING;
            DataObject.INSPECTOR = this.INSPECTOR;
            DataObject.RMANO = this.RMANO;
            DataObject.ACTIO = this.ACTIO;
            DataObject.TEMP1 = this.TEMP1;
            DataObject.TEMP2 = this.TEMP2;
            DataObject.TEMP3 = this.TEMP3;
            DataObject.TEMP4 = this.TEMP4;
            DataObject.TEMP5 = this.TEMP5;
            DataObject.HEADID = this.HEADID;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string DATAPOINT
        {
            get
            {
                return (string)this["DATAPOINT"];
            }
            set
            {
                this["DATAPOINT"] = value;
            }
        }
        public string RECORD_CREATION_DATE
        {
            get
            {
                return (string)this["RECORD_CREATION_DATE"];
            }
            set
            {
                this["RECORD_CREATION_DATE"] = value;
            }
        }
        public string CM_CODE
        {
            get
            {
                return (string)this["CM_CODE"];
            }
            set
            {
                this["CM_CODE"] = value;
            }
        }
        public string INSPECTION_DATE
        {
            get
            {
                return (string)this["INSPECTION_DATE"];
            }
            set
            {
                this["INSPECTION_DATE"] = value;
            }
        }
        public string REJECTCODE
        {
            get
            {
                return (string)this["REJECTCODE"];
            }
            set
            {
                this["REJECTCODE"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
            }
        }
        public string MANUFACTURER
        {
            get
            {
                return (string)this["MANUFACTURER"];
            }
            set
            {
                this["MANUFACTURER"] = value;
            }
        }
        public string LOTNO
        {
            get
            {
                return (string)this["LOTNO"];
            }
            set
            {
                this["LOTNO"] = value;
            }
        }
        public string DATECODE
        {
            get
            {
                return (string)this["DATECODE"];
            }
            set
            {
                this["DATECODE"] = value;
            }
        }
        public string LOTCODE
        {
            get
            {
                return (string)this["LOTCODE"];
            }
            set
            {
                this["LOTCODE"] = value;
            }
        }
        public string RECEIVEQTY
        {
            get
            {
                return (string)this["RECEIVEQTY"];
            }
            set
            {
                this["RECEIVEQTY"] = value;
            }
        }
        public string SAMPLESIZE
        {
            get
            {
                return (string)this["SAMPLESIZE"];
            }
            set
            {
                this["SAMPLESIZE"] = value;
            }
        }
        public string ACCEPTQTY
        {
            get
            {
                return (string)this["ACCEPTQTY"];
            }
            set
            {
                this["ACCEPTQTY"] = value;
            }
        }
        public string REJECTQTY
        {
            get
            {
                return (string)this["REJECTQTY"];
            }
            set
            {
                this["REJECTQTY"] = value;
            }
        }
        public string AVLSTATUS
        {
            get
            {
                return (string)this["AVLSTATUS"];
            }
            set
            {
                this["AVLSTATUS"] = value;
            }
        }
        public string FIRSTINCOMING
        {
            get
            {
                return (string)this["FIRSTINCOMING"];
            }
            set
            {
                this["FIRSTINCOMING"] = value;
            }
        }
        public string INSPECTOR
        {
            get
            {
                return (string)this["INSPECTOR"];
            }
            set
            {
                this["INSPECTOR"] = value;
            }
        }
        public string RMANO
        {
            get
            {
                return (string)this["RMANO"];
            }
            set
            {
                this["RMANO"] = value;
            }
        }
        public string ACTIO
        {
            get
            {
                return (string)this["ACTIO"];
            }
            set
            {
                this["ACTIO"] = value;
            }
        }
        public string TEMP1
        {
            get
            {
                return (string)this["TEMP1"];
            }
            set
            {
                this["TEMP1"] = value;
            }
        }
        public string TEMP2
        {
            get
            {
                return (string)this["TEMP2"];
            }
            set
            {
                this["TEMP2"] = value;
            }
        }
        public string TEMP3
        {
            get
            {
                return (string)this["TEMP3"];
            }
            set
            {
                this["TEMP3"] = value;
            }
        }
        public string TEMP4
        {
            get
            {
                return (string)this["TEMP4"];
            }
            set
            {
                this["TEMP4"] = value;
            }
        }
        public string TEMP5
        {
            get
            {
                return (string)this["TEMP5"];
            }
            set
            {
                this["TEMP5"] = value;
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
        public string HEADID
        {
            get
            {
                return (string)this["HEADID"];
            }
            set
            {
                this["HEADID"] = value;
            }
        }
    }
    public class R_MDS_IQC
    {
        public string ID { get; set; }
        public string DATAPOINT { get; set; }
        public string RECORD_CREATION_DATE { get; set; }
        public string CM_CODE { get; set; }
        public string INSPECTION_DATE { get; set; }
        public string REJECTCODE { get; set; }
        public string PARTNO { get; set; }
        public string MPN { get; set; }
        public string MANUFACTURER { get; set; }
        public string LOTNO { get; set; }
        public string DATECODE { get; set; }
        public string LOTCODE { get; set; }
        public string RECEIVEQTY { get; set; }
        public string SAMPLESIZE { get; set; }
        public string ACCEPTQTY { get; set; }
        public string REJECTQTY { get; set; }
        public string AVLSTATUS { get; set; }
        public string FIRSTINCOMING { get; set; }
        public string INSPECTOR { get; set; }
        public string RMANO { get; set; }
        public string ACTIO { get; set; }
        public string TEMP1 { get; set; }
        public string TEMP2 { get; set; }
        public string TEMP3 { get; set; }
        public string TEMP4 { get; set; }
        public string TEMP5 { get; set; }
        public string HEADID { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}