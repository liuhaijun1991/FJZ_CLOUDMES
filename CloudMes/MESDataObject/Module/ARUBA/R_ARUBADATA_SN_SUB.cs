using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_R_ARUBADATA_SN_SUB : DataObjectTable
    {
        public T_R_ARUBADATA_SN_SUB(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ARUBADATA_SN_SUB(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ARUBADATA_SN_SUB);
            TableName = "R_ARUBADATA_SN_SUB".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ARUBADATA_SN_SUB : DataObjectBase
    {
        public Row_R_ARUBADATA_SN_SUB(DataObjectInfo info) : base(info)
        {

        }
        public R_ARUBADATA_SN_SUB GetDataObject()
        {
            R_ARUBADATA_SN_SUB DataObject = new R_ARUBADATA_SN_SUB();
            DataObject.ID = this.ID;
            DataObject.SNID = this.SNID;
            DataObject.SEQ = this.SEQ;
            DataObject.MODULETYPE = this.MODULETYPE;
            DataObject.GENERICCATEGORY = this.GENERICCATEGORY;
            DataObject.CPN = this.CPN;
            DataObject.SPN = this.SPN;
            DataObject.SN = this.SN;
            DataObject.CT_DC = this.CT_DC;
            DataObject.HREVISION = this.HREVISION;
            DataObject.FREVISION = this.FREVISION;
            DataObject.SUPPLIERNAME = this.SUPPLIERNAME;
            DataObject.ETSTATUS = this.ETSTATUS;
            DataObject.OPERATION = this.OPERATION;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.PARENTPRODUCT = this.PARENTPRODUCT;
            DataObject.FAMILY = this.FAMILY;
            DataObject.PARTCATEGORY = this.PARTCATEGORY;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.EATRAINFO = this.EATRAINFO;
            DataObject.LOCALIZATION = this.LOCALIZATION;
            DataObject.FEATURECODE = this.FEATURECODE;
            DataObject.FEATUREVALUE = this.FEATUREVALUE;
            DataObject.OTHEROPTIONS = this.OTHEROPTIONS;
            DataObject.FUTURE = this.FUTURE;
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
        public string SNID
        {
            get
            {
                return (string)this["SNID"];
            }
            set
            {
                this["SNID"] = value;
            }
        }
        public double? SEQ
        {
            get
            {
                return (double?)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
            }
        }
        public string MODULETYPE
        {
            get
            {
                return (string)this["MODULETYPE"];
            }
            set
            {
                this["MODULETYPE"] = value;
            }
        }
        public string GENERICCATEGORY
        {
            get
            {
                return (string)this["GENERICCATEGORY"];
            }
            set
            {
                this["GENERICCATEGORY"] = value;
            }
        }
        public string CPN
        {
            get
            {
                return (string)this["CPN"];
            }
            set
            {
                this["CPN"] = value;
            }
        }
        public string SPN
        {
            get
            {
                return (string)this["SPN"];
            }
            set
            {
                this["SPN"] = value;
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
        public string CT_DC
        {
            get
            {
                return (string)this["CT_DC"];
            }
            set
            {
                this["CT_DC"] = value;
            }
        }
        public string HREVISION
        {
            get
            {
                return (string)this["HREVISION"];
            }
            set
            {
                this["HREVISION"] = value;
            }
        }
        public string FREVISION
        {
            get
            {
                return (string)this["FREVISION"];
            }
            set
            {
                this["FREVISION"] = value;
            }
        }
        public string SUPPLIERNAME
        {
            get
            {
                return (string)this["SUPPLIERNAME"];
            }
            set
            {
                this["SUPPLIERNAME"] = value;
            }
        }
        public string ETSTATUS
        {
            get
            {
                return (string)this["ETSTATUS"];
            }
            set
            {
                this["ETSTATUS"] = value;
            }
        }
        public string OPERATION
        {
            get
            {
                return (string)this["OPERATION"];
            }
            set
            {
                this["OPERATION"] = value;
            }
        }
        public double? QUANTITY
        {
            get
            {
                return (double?)this["QUANTITY"];
            }
            set
            {
                this["QUANTITY"] = value;
            }
        }
        public string PARENTPRODUCT
        {
            get
            {
                return (string)this["PARENTPRODUCT"];
            }
            set
            {
                this["PARENTPRODUCT"] = value;
            }
        }
        public string FAMILY
        {
            get
            {
                return (string)this["FAMILY"];
            }
            set
            {
                this["FAMILY"] = value;
            }
        }
        public string PARTCATEGORY
        {
            get
            {
                return (string)this["PARTCATEGORY"];
            }
            set
            {
                this["PARTCATEGORY"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string EATRAINFO
        {
            get
            {
                return (string)this["EATRAINFO"];
            }
            set
            {
                this["EATRAINFO"] = value;
            }
        }
        public string LOCALIZATION
        {
            get
            {
                return (string)this["LOCALIZATION"];
            }
            set
            {
                this["LOCALIZATION"] = value;
            }
        }
        public string FEATURECODE
        {
            get
            {
                return (string)this["FEATURECODE"];
            }
            set
            {
                this["FEATURECODE"] = value;
            }
        }
        public string FEATUREVALUE
        {
            get
            {
                return (string)this["FEATUREVALUE"];
            }
            set
            {
                this["FEATUREVALUE"] = value;
            }
        }
        public string OTHEROPTIONS
        {
            get
            {
                return (string)this["OTHEROPTIONS"];
            }
            set
            {
                this["OTHEROPTIONS"] = value;
            }
        }
        public string FUTURE
        {
            get
            {
                return (string)this["FUTURE"];
            }
            set
            {
                this["FUTURE"] = value;
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
    public class R_ARUBADATA_SN_SUB
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SNID { get; set; }
        public double? SEQ { get; set; }
        public string MODULETYPE { get; set; }
        public string GENERICCATEGORY { get; set; }
        public string CPN { get; set; }
        public string SPN { get; set; }
        public string SN { get; set; }
        public string CT_DC { get; set; }
        public string HREVISION { get; set; }
        public string FREVISION { get; set; }
        public string SUPPLIERNAME { get; set; }
        public string ETSTATUS { get; set; }
        public string OPERATION { get; set; }
        public double? QUANTITY { get; set; }
        public string PARENTPRODUCT { get; set; }
        public string FAMILY { get; set; }
        public string PARTCATEGORY { get; set; }
        public string DESCRIPTION { get; set; }
        public string EATRAINFO { get; set; }
        public string LOCALIZATION { get; set; }
        public string FEATURECODE { get; set; }
        public string FEATUREVALUE { get; set; }
        public string OTHEROPTIONS { get; set; }
        public string FUTURE { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}