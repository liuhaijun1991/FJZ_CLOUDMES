using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WHS_SN : DataObjectTable
    {
        public T_R_WHS_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WHS_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WHS_SN);
            TableName = "R_WHS_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_WHS_SN : DataObjectBase
    {
        public Row_R_WHS_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_WHS_SN GetDataObject()
        {
            R_WHS_SN DataObject = new R_WHS_SN();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SOURCE = this.SOURCE;
            DataObject.IN_TIME = this.IN_TIME;
            DataObject.IN_BY = this.IN_BY;
            DataObject.IN_MOV_TYPE = this.IN_MOV_TYPE;
            DataObject.IN_R_SAP_MOV_ID = this.IN_R_SAP_MOV_ID;
            DataObject.OUT_TIME = this.OUT_TIME;
            DataObject.OUT_BY = this.OUT_BY;
            DataObject.OUT_MOV_TYPE = this.OUT_MOV_TYPE;
            DataObject.OUT_R_SAP_MOV_ID = this.OUT_R_SAP_MOV_ID;
            DataObject.STATUS = this.STATUS;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
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
        public string SOURCE
        {
            get
            {
                return (string)this["SOURCE"];
            }
            set
            {
                this["SOURCE"] = value;
            }
        }
        public DateTime? IN_TIME
        {
            get
            {
                return (DateTime?)this["IN_TIME"];
            }
            set
            {
                this["IN_TIME"] = value;
            }
        }
        public string IN_BY
        {
            get
            {
                return (string)this["IN_BY"];
            }
            set
            {
                this["IN_BY"] = value;
            }
        }
        public string IN_MOV_TYPE
        {
            get
            {
                return (string)this["IN_MOV_TYPE"];
            }
            set
            {
                this["IN_MOV_TYPE"] = value;
            }
        }
        public string IN_R_SAP_MOV_ID
        {
            get
            {
                return (string)this["IN_R_SAP_MOV_ID"];
            }
            set
            {
                this["IN_R_SAP_MOV_ID"] = value;
            }
        }
        public DateTime? OUT_TIME
        {
            get
            {
                return (DateTime?)this["OUT_TIME"];
            }
            set
            {
                this["OUT_TIME"] = value;
            }
        }
        public string OUT_BY
        {
            get
            {
                return (string)this["OUT_BY"];
            }
            set
            {
                this["OUT_BY"] = value;
            }
        }
        public string OUT_MOV_TYPE
        {
            get
            {
                return (string)this["OUT_MOV_TYPE"];
            }
            set
            {
                this["OUT_MOV_TYPE"] = value;
            }
        }
        public string OUT_R_SAP_MOV_ID
        {
            get
            {
                return (string)this["OUT_R_SAP_MOV_ID"];
            }
            set
            {
                this["OUT_R_SAP_MOV_ID"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
    }
    public class R_WHS_SN
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string SOURCE { get; set; }
        public DateTime? IN_TIME { get; set; }
        public string IN_BY { get; set; }
        public string IN_MOV_TYPE { get; set; }
        public string IN_R_SAP_MOV_ID { get; set; }
        public DateTime? OUT_TIME { get; set; }
        public string OUT_BY { get; set; }
        public string OUT_MOV_TYPE { get; set; }
        public string OUT_R_SAP_MOV_ID { get; set; }
        public string STATUS { get; set; }
    }
}