using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
namespace MESDataObject.Module.Juniper
{
    public class T_R_I137_DETAIL : DataObjectTable
    {
        public T_R_I137_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I137_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I137_DETAIL);
            TableName = "R_I137_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I137_DETAIL : DataObjectBase
    {
        public Row_R_I137_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_I137_DETAIL GetDataObject()
        {
            R_I137_DETAIL DataObject = new R_I137_DETAIL();
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.TRANID = this.TRANID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.PONUMBER = this.PONUMBER;
            DataObject.ITEM = this.ITEM;
            DataObject.COMPONENTID = this.COMPONENTID;
            DataObject.COMCUSTPRODID = this.COMCUSTPRODID;
            DataObject.COMSALESORDERLINEITEM = this.COMSALESORDERLINEITEM;
            DataObject.COMPONENTQTY = this.COMPONENTQTY;
            DataObject.F_LASTEDITDT = this.F_LASTEDITDT;
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
        public double? F_ID
        {
            get
            {
                return (double?)this["F_ID"];
            }
            set
            {
                this["F_ID"] = value;
            }
        }
        public string TRANID
        {
            get
            {
                return (string)this["TRANID"];
            }
            set
            {
                this["TRANID"] = value;
            }
        }
        public string F_PLANT
        {
            get
            {
                return (string)this["F_PLANT"];
            }
            set
            {
                this["F_PLANT"] = value;
            }
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
            }
        }
        public string MESSAGEID
        {
            get
            {
                return (string)this["MESSAGEID"];
            }
            set
            {
                this["MESSAGEID"] = value;
            }
        }
        public string PONUMBER
        {
            get
            {
                return (string)this["PONUMBER"];
            }
            set
            {
                this["PONUMBER"] = value;
            }
        }
        public string ITEM
        {
            get
            {
                return (string)this["ITEM"];
            }
            set
            {
                this["ITEM"] = value;
            }
        }
        public string COMPONENTID
        {
            get
            {
                return (string)this["COMPONENTID"];
            }
            set
            {
                this["COMPONENTID"] = value;
            }
        }
        public string COMCUSTPRODID
        {
            get
            {
                return (string)this["COMCUSTPRODID"];
            }
            set
            {
                this["COMCUSTPRODID"] = value;
            }
        }
        public string COMSALESORDERLINEITEM
        {
            get
            {
                return (string)this["COMSALESORDERLINEITEM"];
            }
            set
            {
                this["COMSALESORDERLINEITEM"] = value;
            }
        }
        public string COMPONENTQTY
        {
            get
            {
                return (string)this["COMPONENTQTY"];
            }
            set
            {
                this["COMPONENTQTY"] = value;
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
    public class R_I137_DETAIL
    {
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string COMPONENTID { get; set; }
        public string COMCUSTPRODID { get; set; }
        public string COMSALESORDERLINEITEM { get; set; }
        public string COMPONENTQTY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}
