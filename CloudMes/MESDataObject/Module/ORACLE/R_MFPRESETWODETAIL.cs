using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.ORACLE
{
    public class T_R_MFPRESETWODETAIL : DataObjectTable
    {
        public T_R_MFPRESETWODETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MFPRESETWODETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MFPRESETWODETAIL);
            TableName = "R_MFPRESETWODETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MFPRESETWODETAIL : DataObjectBase
    {
        public Row_R_MFPRESETWODETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_MFPRESETWODETAIL GetDataObject()
        {
            R_MFPRESETWODETAIL DataObject = new R_MFPRESETWODETAIL();
            DataObject.CUSTOMERID = this.CUSTOMERID;
            DataObject.WO = this.WO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.REQUESTQTY = this.REQUESTQTY;
            DataObject.UNITPRICE = this.UNITPRICE;
            DataObject.UNITWEIGHT = this.UNITWEIGHT;
            DataObject.PACKAGEFLAG = this.PACKAGEFLAG;
            DataObject.PARTNOTYPE = this.PARTNOTYPE;
            DataObject.CREATETIME = this.CREATETIME;
            return DataObject;
        }
        public string CUSTOMERID
        {
            get
            {
                return (string)this["CUSTOMERID"];
            }
            set
            {
                this["CUSTOMERID"] = value;
            }
        }
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
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
        public string REQUESTQTY
        {
            get
            {
                return (string)this["REQUESTQTY"];
            }
            set
            {
                this["REQUESTQTY"] = value;
            }
        }
        public string UNITPRICE
        {
            get
            {
                return (string)this["UNITPRICE"];
            }
            set
            {
                this["UNITPRICE"] = value;
            }
        }
        public string UNITWEIGHT
        {
            get
            {
                return (string)this["UNITWEIGHT"];
            }
            set
            {
                this["UNITWEIGHT"] = value;
            }
        }
        public string PACKAGEFLAG
        {
            get
            {
                return (string)this["PACKAGEFLAG"];
            }
            set
            {
                this["PACKAGEFLAG"] = value;
            }
        }
        public string PARTNOTYPE
        {
            get
            {
                return (string)this["PARTNOTYPE"];
            }
            set
            {
                this["PARTNOTYPE"] = value;
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
    public class R_MFPRESETWODETAIL
    {
        public string CUSTOMERID { get; set; }
        public string WO{get;set;}
        public string PARTNO{get;set;}
        public string REQUESTQTY{get;set;}
        public string UNITPRICE{get;set;}
        public string UNITWEIGHT{get;set;}
        public string PACKAGEFLAG{get;set;}
        public string PARTNOTYPE{get;set;}
        public DateTime? CREATETIME{get;set;}
    }
}