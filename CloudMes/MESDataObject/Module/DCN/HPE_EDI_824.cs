using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_HPE_EDI_824 : DataObjectTable
    {
        public T_HPE_EDI_824(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_HPE_EDI_824(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_HPE_EDI_824);
            TableName = "HPE_EDI_824".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_HPE_EDI_824 : DataObjectBase
    {
        public Row_HPE_EDI_824(DataObjectInfo info) : base(info)
        {

        }
        public HPE_EDI_824 GetDataObject()
        {
            HPE_EDI_824 DataObject = new HPE_EDI_824();
            DataObject.ID = this.ID;
            DataObject.F_SITE = this.F_SITE;
            DataObject.F_ADV_ID = this.F_ADV_ID;
            DataObject.F_ADV_DT = this.F_ADV_DT;
            DataObject.F_OTI_CODE = this.F_OTI_CODE;
            DataObject.F_OTI_DOCID = this.F_OTI_DOCID;
            DataObject.F_OTI_TRAN_TYPE = this.F_OTI_TRAN_TYPE;
            DataObject.F_OTI_TRAN_AC_CODE = this.F_OTI_TRAN_AC_CODE;
            DataObject.F_OTI_TRAN_DT = this.F_OTI_TRAN_DT;
            DataObject.F_OTI_TED_TEXT = this.F_OTI_TED_TEXT;
            DataObject.F_OTI_NTE_MSG = this.F_OTI_NTE_MSG;
            DataObject.F_FILENAME = this.F_FILENAME;
            DataObject.F_LASTEDITDT = this.F_LASTEDITDT;
            DataObject.F_EDI_FILENAME = this.F_EDI_FILENAME;
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
        public string F_SITE
        {
            get
            {
                return (string)this["F_SITE"];
            }
            set
            {
                this["F_SITE"] = value;
            }
        }
        public string F_ADV_ID
        {
            get
            {
                return (string)this["F_ADV_ID"];
            }
            set
            {
                this["F_ADV_ID"] = value;
            }
        }
        public DateTime? F_ADV_DT
        {
            get
            {
                return (DateTime?)this["F_ADV_DT"];
            }
            set
            {
                this["F_ADV_DT"] = value;
            }
        }
        public string F_OTI_CODE
        {
            get
            {
                return (string)this["F_OTI_CODE"];
            }
            set
            {
                this["F_OTI_CODE"] = value;
            }
        }
        public string F_OTI_DOCID
        {
            get
            {
                return (string)this["F_OTI_DOCID"];
            }
            set
            {
                this["F_OTI_DOCID"] = value;
            }
        }
        public string F_OTI_TRAN_TYPE
        {
            get
            {
                return (string)this["F_OTI_TRAN_TYPE"];
            }
            set
            {
                this["F_OTI_TRAN_TYPE"] = value;
            }
        }
        public string F_OTI_TRAN_AC_CODE
        {
            get
            {
                return (string)this["F_OTI_TRAN_AC_CODE"];
            }
            set
            {
                this["F_OTI_TRAN_AC_CODE"] = value;
            }
        }
        public DateTime? F_OTI_TRAN_DT
        {
            get
            {
                return (DateTime?)this["F_OTI_TRAN_DT"];
            }
            set
            {
                this["F_OTI_TRAN_DT"] = value;
            }
        }
        public string F_OTI_TED_TEXT
        {
            get
            {
                return (string)this["F_OTI_TED_TEXT"];
            }
            set
            {
                this["F_OTI_TED_TEXT"] = value;
            }
        }
        public string F_OTI_NTE_MSG
        {
            get
            {
                return (string)this["F_OTI_NTE_MSG"];
            }
            set
            {
                this["F_OTI_NTE_MSG"] = value;
            }
        }
        public string F_FILENAME
        {
            get
            {
                return (string)this["F_FILENAME"];
            }
            set
            {
                this["F_FILENAME"] = value;
            }
        }
        public DateTime? F_LASTEDITDT
        {
            get
            {
                return (DateTime?)this["F_LASTEDITDT"];
            }
            set
            {
                this["F_LASTEDITDT"] = value;
            }
        }
        public string F_EDI_FILENAME
        {
            get
            {
                return (string)this["F_EDI_FILENAME"];
            }
            set
            {
                this["F_EDI_FILENAME"] = value;
            }
        }
    }

    [SqlSugar.SugarTable("hpe.TB_EDI824")]
    public class B2B_HPE_EDI_824
    { 
        public string F_ID { get; set; }
        public string F_SITE { get; set; }
        public string F_ADV_ID { get; set; }
        public DateTime? F_ADV_DT { get; set; }
        public string F_OTI_CODE { get; set; }
        public string F_OTI_DOCID { get; set; }
        public string F_OTI_TRAN_TYPE { get; set; }
        public string F_OTI_TRAN_AC_CODE { get; set; }
        public DateTime? F_OTI_TRAN_DT { get; set; }
        public string F_OTI_TED_TEXT { get; set; }
        public string F_OTI_NTE_MSG { get; set; }
        public string F_FILENAME { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public string F_EDI_FILENAME { get; set; }
    }

    public class HPE_EDI_824
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string F_ID { get; set; }
        public string F_SITE { get; set; }
        public string F_ADV_ID { get; set; }
        public DateTime? F_ADV_DT { get; set; }
        public string F_OTI_CODE { get; set; }
        public string F_OTI_DOCID { get; set; }
        public string F_OTI_TRAN_TYPE { get; set; }
        public string F_OTI_TRAN_AC_CODE { get; set; }
        public DateTime? F_OTI_TRAN_DT { get; set; }
        public string F_OTI_TED_TEXT { get; set; }
        public string F_OTI_NTE_MSG { get; set; }
        public string F_FILENAME { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public string F_EDI_FILENAME { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}