using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MDS_IQC_AP : DataObjectTable
    {
        public T_R_MDS_IQC_AP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MDS_IQC_AP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MDS_IQC_AP);
            TableName = "R_MDS_IQC_AP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MDS_IQC_AP : DataObjectBase
    {
        public Row_R_MDS_IQC_AP(DataObjectInfo info) : base(info)
        {

        }
        public R_MDS_IQC_AP GetDataObject()
        {
            R_MDS_IQC_AP DataObject = new R_MDS_IQC_AP();
            DataObject.TEMP4 = this.TEMP4;
            DataObject.TEMP5 = this.TEMP5;
            DataObject.ID = this.ID;
            DataObject.MDSDATE = this.MDSDATE;
            DataObject.DATAPOINT = this.DATAPOINT;
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
            DataObject.RACTION = this.RACTION;
            DataObject.TEMP1 = this.TEMP1;
            DataObject.TEMP2 = this.TEMP2;
            DataObject.TEMP3 = this.TEMP3;
            return DataObject;
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
        public DateTime? MDSDATE
        {
            get
            {
                return (DateTime?)this["MDSDATE"];
            }
            set
            {
                this["MDSDATE"] = value;
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
        public double? RECEIVEQTY
        {
            get
            {
                return (double?)this["RECEIVEQTY"];
            }
            set
            {
                this["RECEIVEQTY"] = value;
            }
        }
        public double? SAMPLESIZE
        {
            get
            {
                return (double?)this["SAMPLESIZE"];
            }
            set
            {
                this["SAMPLESIZE"] = value;
            }
        }
        public double? ACCEPTQTY
        {
            get
            {
                return (double?)this["ACCEPTQTY"];
            }
            set
            {
                this["ACCEPTQTY"] = value;
            }
        }
        public double? REJECTQTY
        {
            get
            {
                return (double?)this["REJECTQTY"];
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
        public string RACTION
        {
            get
            {
                return (string)this["RACTION"];
            }
            set
            {
                this["RACTION"] = value;
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
    }
    public class R_MDS_IQC_AP
    {
        public string ID { get; set; }
        public DateTime? MDSDATE { get; set; }
        public string DATAPOINT { get; set; }
        public string REJECTCODE { get; set; }
        public string PARTNO { get; set; }
        public string MPN { get; set; }
        public string MANUFACTURER { get; set; }
        public string LOTNO { get; set; }
        public string DATECODE { get; set; }
        public string LOTCODE { get; set; }
        public double? RECEIVEQTY { get; set; }
        public double? SAMPLESIZE { get; set; }
        public double? ACCEPTQTY { get; set; }
        public double? REJECTQTY { get; set; }
        public string AVLSTATUS { get; set; }
        public string FIRSTINCOMING { get; set; }
        public string INSPECTOR { get; set; }
        public string RMANO { get; set; }
        public string RACTION { get; set; }
        public string TEMP1 { get; set; }
        public string TEMP2 { get; set; }
        public string TEMP3 { get; set; }
        public string TEMP4 { get; set; }
        public string TEMP5 { get; set; }
    }
}