using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MDS_STR : DataObjectTable
    {
        public T_R_MDS_STR(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MDS_STR(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MDS_STR);
            TableName = "R_MDS_STR".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MDS_STR : DataObjectBase
    {
        public Row_R_MDS_STR(DataObjectInfo info) : base(info)
        {

        }
        public R_MDS_STR GetDataObject()
        {
            R_MDS_STR DataObject = new R_MDS_STR();
            DataObject.ID = this.ID;
            DataObject.DATAPOINT = this.DATAPOINT;
            DataObject.RECORD_CREATION_DATE = this.RECORD_CREATION_DATE;
            DataObject.CM_CODE = this.CM_CODE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SYSSERIALNO = this.SYSSERIALNO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.CSERIALNO = this.CSERIALNO;
            DataObject.VENDORID = this.VENDORID;
            DataObject.VENDORNAME = this.VENDORNAME;
            DataObject.STRUC_DATE = this.STRUC_DATE;
            DataObject.CORRECTTYPE = this.CORRECTTYPE;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.WORKORDERTYPE = this.WORKORDERTYPE;
            DataObject.ATTRIBUTE1 = this.ATTRIBUTE1;
            DataObject.ATTRIBUTE2 = this.ATTRIBUTE2;
            DataObject.ATTRIBUTE3 = this.ATTRIBUTE3;
            DataObject.ATTRIBUTE4 = this.ATTRIBUTE4;
            DataObject.HEADID = this.HEADID;
            DataObject.CREATETIME = this.CREATETIME;
            return DataObject;
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
        public string SYSSERIALNO
        {
            get
            {
                return (string)this["SYSSERIALNO"];
            }
            set
            {
                this["SYSSERIALNO"] = value;
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
        public string CSERIALNO
        {
            get
            {
                return (string)this["CSERIALNO"];
            }
            set
            {
                this["CSERIALNO"] = value;
            }
        }
        public string VENDORID
        {
            get
            {
                return (string)this["VENDORID"];
            }
            set
            {
                this["VENDORID"] = value;
            }
        }
        public string VENDORNAME
        {
            get
            {
                return (string)this["VENDORNAME"];
            }
            set
            {
                this["VENDORNAME"] = value;
            }
        }
        public string STRUC_DATE
        {
            get
            {
                return (string)this["STRUC_DATE"];
            }
            set
            {
                this["STRUC_DATE"] = value;
            }
        }
        public string CORRECTTYPE
        {
            get
            {
                return (string)this["CORRECTTYPE"];
            }
            set
            {
                this["CORRECTTYPE"] = value;
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
        public string WORKORDERTYPE
        {
            get
            {
                return (string)this["WORKORDERTYPE"];
            }
            set
            {
                this["WORKORDERTYPE"] = value;
            }
        }
        public string ATTRIBUTE1
        {
            get
            {
                return (string)this["ATTRIBUTE1"];
            }
            set
            {
                this["ATTRIBUTE1"] = value;
            }
        }
        public string ATTRIBUTE2
        {
            get
            {
                return (string)this["ATTRIBUTE2"];
            }
            set
            {
                this["ATTRIBUTE2"] = value;
            }
        }
        public string ATTRIBUTE3
        {
            get
            {
                return (string)this["ATTRIBUTE3"];
            }
            set
            {
                this["ATTRIBUTE3"] = value;
            }
        }
        public string ATTRIBUTE4
        {
            get
            {
                return (string)this["ATTRIBUTE4"];
            }
            set
            {
                this["ATTRIBUTE4"] = value;
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
    }
    public class R_MDS_STR
    {
        public string ID { get; set; }
        public string DATAPOINT { get; set; }
        public string RECORD_CREATION_DATE { get; set; }
        public string CM_CODE { get; set; }
        public string SKUNO { get; set; }
        public string SYSSERIALNO { get; set; }
        public string PARTNO { get; set; }
        public string CSERIALNO { get; set; }
        public string VENDORID { get; set; }
        public string VENDORNAME { get; set; }
        public string STRUC_DATE { get; set; }
        public string CORRECTTYPE { get; set; }
        public string WORKORDERNO { get; set; }
        public string WORKORDERTYPE { get; set; }
        public string ATTRIBUTE1 { get; set; }
        public string ATTRIBUTE2 { get; set; }
        public string ATTRIBUTE3 { get; set; }
        public string ATTRIBUTE4 { get; set; }
        public string HEADID { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}