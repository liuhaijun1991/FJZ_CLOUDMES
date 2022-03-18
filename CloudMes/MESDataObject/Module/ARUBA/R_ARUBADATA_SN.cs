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
    public class T_R_ARUBADATA_SN : DataObjectTable
    {
        public T_R_ARUBADATA_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ARUBADATA_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ARUBADATA_SN);
            TableName = "R_ARUBADATA_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ARUBADATA_SN : DataObjectBase
    {
        public Row_R_ARUBADATA_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_ARUBADATA_SN GetDataObject()
        {
            R_ARUBADATA_SN DataObject = new R_ARUBADATA_SN();
            DataObject.ID = this.ID;
            DataObject.HEADID = this.HEADID;
            DataObject.RECORDTYPE = this.RECORDTYPE;
            DataObject.SN = this.SN;
            DataObject.PN = this.PN;
            DataObject.RECORDORIGIN = this.RECORDORIGIN;
            DataObject.SUBFAORIGIN = this.SUBFAORIGIN;
            DataObject.LOCALIZATION = this.LOCALIZATION;
            DataObject.WARRANTY = this.WARRANTY;
            DataObject.OTHER = this.OTHER;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.ASSETTAG = this.ASSETTAG;
            DataObject.FUTURE = this.FUTURE;
            DataObject.TESTRESULT = this.TESTRESULT;
            DataObject.NBSUBMODULES = this.NBSUBMODULES;
            DataObject.OSN = this.OSN;
            DataObject.OPN = this.OPN;
            DataObject.SEQ = this.SEQ;
            DataObject.PARENTID = this.PARENTID;
            DataObject.PARENTSN = this.PARENTSN;
            DataObject.TOTALLENGTH = this.TOTALLENGTH;
            DataObject.DN = this.DN;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.SNPN = this.SNPN;
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
        public string RECORDTYPE
        {
            get
            {
                return (string)this["RECORDTYPE"];
            }
            set
            {
                this["RECORDTYPE"] = value;
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
        public string RECORDORIGIN
        {
            get
            {
                return (string)this["RECORDORIGIN"];
            }
            set
            {
                this["RECORDORIGIN"] = value;
            }
        }
        public string SUBFAORIGIN
        {
            get
            {
                return (string)this["SUBFAORIGIN"];
            }
            set
            {
                this["SUBFAORIGIN"] = value;
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
        public string WARRANTY
        {
            get
            {
                return (string)this["WARRANTY"];
            }
            set
            {
                this["WARRANTY"] = value;
            }
        }
        public string OTHER
        {
            get
            {
                return (string)this["OTHER"];
            }
            set
            {
                this["OTHER"] = value;
            }
        }
        public DateTime? SHIPDATE
        {
            get
            {
                return (DateTime?)this["SHIPDATE"];
            }
            set
            {
                this["SHIPDATE"] = value;
            }
        }
        public string ASSETTAG
        {
            get
            {
                return (string)this["ASSETTAG"];
            }
            set
            {
                this["ASSETTAG"] = value;
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
        public string TESTRESULT
        {
            get
            {
                return (string)this["TESTRESULT"];
            }
            set
            {
                this["TESTRESULT"] = value;
            }
        }
        public double? NBSUBMODULES
        {
            get
            {
                return (double?)this["NBSUBMODULES"];
            }
            set
            {
                this["NBSUBMODULES"] = value;
            }
        }
        public string OSN
        {
            get
            {
                return (string)this["OSN"];
            }
            set
            {
                this["OSN"] = value;
            }
        }
        public string OPN
        {
            get
            {
                return (string)this["OPN"];
            }
            set
            {
                this["OPN"] = value;
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
        public string PARENTID
        {
            get
            {
                return (string)this["PARENTID"];
            }
            set
            {
                this["PARENTID"] = value;
            }
        }
        public string PARENTSN
        {
            get
            {
                return (string)this["PARENTSN"];
            }
            set
            {
                this["PARENTSN"] = value;
            }
        }
        public double? TOTALLENGTH
        {
            get
            {
                return (double?)this["TOTALLENGTH"];
            }
            set
            {
                this["TOTALLENGTH"] = value;
            }
        }
        public string DN
        {
            get
            {
                return (string)this["DN"];
            }
            set
            {
                this["DN"] = value;
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
        public string SNPN
        {
            get
            {
                return (string)this["SNPN"];
            }
            set
            {
                this["SNPN"] = value;
            }
        }
    }
    public class R_ARUBADATA_SN
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string HEADID { get; set; }
        public string RECORDTYPE { get; set; }
        public string SN { get; set; }
        public string PN { get; set; }
        public string RECORDORIGIN { get; set; }
        public string SUBFAORIGIN { get; set; }
        public string LOCALIZATION { get; set; }
        public string WARRANTY { get; set; }
        public string OTHER { get; set; }
        public DateTime? SHIPDATE { get; set; }
        public string ASSETTAG { get; set; }
        public string FUTURE { get; set; }
        public string TESTRESULT { get; set; }
        public double? NBSUBMODULES { get; set; }
        public string OSN { get; set; }
        public string OPN { get; set; }
        public double? SEQ { get; set; }
        public string PARENTID { get; set; }
        public string PARENTSN { get; set; }
        public double? TOTALLENGTH { get; set; }
        public string DN { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string SNPN { get; set; }
    }
}