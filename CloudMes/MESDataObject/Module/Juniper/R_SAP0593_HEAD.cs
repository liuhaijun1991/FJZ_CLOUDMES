using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SAP0593_HEAD : DataObjectTable
    {
        public T_R_SAP0593_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP0593_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP0593_HEAD);
            TableName = "R_SAP0593_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP0593_HEAD : DataObjectBase
    {
        public Row_R_SAP0593_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP0593_HEAD GetDataObject()
        {
            R_SAP0593_HEAD DataObject = new R_SAP0593_HEAD();
            DataObject.ID = this.ID;
            DataObject.KEYVALUE = this.KEYVALUE;
            DataObject.MANTD = this.MANTD;
            DataObject.WERKS = this.WERKS;
            DataObject.MATNR = this.MATNR;
            DataObject.BASDAY = this.BASDAY;
            DataObject.PLSCN = this.PLSCN;
            DataObject.MAKTX = this.MAKTX;
            DataObject.ERNAM = this.ERNAM;
            DataObject.UPLOADDATE = this.UPLOADDATE;
            DataObject.UPLOADTIME = this.UPLOADTIME;
            DataObject.CHANGENAME = this.CHANGENAME;
            DataObject.CHANGEDATA = this.CHANGEDATA;
            DataObject.CHANGETIME = this.CHANGETIME;
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
        public string KEYVALUE
        {
            get
            {
                return (string)this["KEYVALUE"];
            }
            set
            {
                this["KEYVALUE"] = value;
            }
        }
        public string MANTD
        {
            get
            {
                return (string)this["MANTD"];
            }
            set
            {
                this["MANTD"] = value;
            }
        }
        public string WERKS
        {
            get
            {
                return (string)this["WERKS"];
            }
            set
            {
                this["WERKS"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string BASDAY
        {
            get
            {
                return (string)this["BASDAY"];
            }
            set
            {
                this["BASDAY"] = value;
            }
        }
        public string PLSCN
        {
            get
            {
                return (string)this["PLSCN"];
            }
            set
            {
                this["PLSCN"] = value;
            }
        }
        public string MAKTX
        {
            get
            {
                return (string)this["MAKTX"];
            }
            set
            {
                this["MAKTX"] = value;
            }
        }
        public string ERNAM
        {
            get
            {
                return (string)this["ERNAM"];
            }
            set
            {
                this["ERNAM"] = value;
            }
        }
        public string UPLOADDATE
        {
            get
            {
                return (string)this["UPLOADDATE"];
            }
            set
            {
                this["UPLOADDATE"] = value;
            }
        }
        public string UPLOADTIME
        {
            get
            {
                return (string)this["UPLOADTIME"];
            }
            set
            {
                this["UPLOADTIME"] = value;
            }
        }
        public string CHANGENAME
        {
            get
            {
                return (string)this["CHANGENAME"];
            }
            set
            {
                this["CHANGENAME"] = value;
            }
        }
        public string CHANGEDATA
        {
            get
            {
                return (string)this["CHANGEDATA"];
            }
            set
            {
                this["CHANGEDATA"] = value;
            }
        }
        public string CHANGETIME
        {
            get
            {
                return (string)this["CHANGETIME"];
            }
            set
            {
                this["CHANGETIME"] = value;
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
    public class R_SAP0593_HEAD
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string KEYVALUE { get; set; }
        public string MANTD { get; set; }
        public string WERKS { get; set; }
        public string MATNR { get; set; }
        public string BASDAY { get; set; }
        public string PLSCN { get; set; }
        public string MAKTX { get; set; }
        public string ERNAM { get; set; }
        public string UPLOADDATE { get; set; }
        public string UPLOADTIME { get; set; }
        public string CHANGENAME { get; set; }
        public string CHANGEDATA { get; set; }
        public string CHANGETIME { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}