using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_GROUPID : DataObjectTable
    {
        public T_R_WO_GROUPID(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_GROUPID(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_GROUPID);
            TableName = "R_WO_GROUPID".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_WO_GROUPID : DataObjectBase
    {
        public Row_R_WO_GROUPID(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_GROUPID GetDataObject()
        {
            R_WO_GROUPID DataObject = new R_WO_GROUPID();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.GROUPID = this.GROUPID;
            DataObject.RUTYPE = this.RUTYPE;
            DataObject.UNITPRICE = this.UNITPRICE;
            DataObject.UNITWEIGHT = this.UNITWEIGHT;
            DataObject.DWFLAG = this.DWFLAG;
            DataObject.TRANTIME = this.TRANTIME;
            DataObject.DW_ID = this.DW_ID;
            DataObject.TYPE_IU = this.TYPE_IU;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.EDITTIME = this.EDITTIME;
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
        public string GROUPID
        {
            get
            {
                return (string)this["GROUPID"];
            }
            set
            {
                this["GROUPID"] = value;
            }
        }
        public string RUTYPE
        {
            get
            {
                return (string)this["RUTYPE"];
            }
            set
            {
                this["RUTYPE"] = value;
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
        public string DWFLAG
        {
            get
            {
                return (string)this["DWFLAG"];
            }
            set
            {
                this["DWFLAG"] = value;
            }
        }
        public string TRANTIME
        {
            get
            {
                return (string)this["TRANTIME"];
            }
            set
            {
                this["TRANTIME"] = value;
            }
        }
        public string DW_ID
        {
            get
            {
                return (string)this["DW_ID"];
            }
            set
            {
                this["DW_ID"] = value;
            }
        }
        public string TYPE_IU
        {
            get
            {
                return (string)this["TYPE_IU"];
            }
            set
            {
                this["TYPE_IU"] = value;
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
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
    }
    public class R_WO_GROUPID
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string GROUPID { get; set; }
        public string RUTYPE { get; set; }
        public string UNITPRICE { get; set; }
        public string UNITWEIGHT { get; set; }
        public string DWFLAG { get; set; }
        public string TRANTIME { get; set; }
        public string DW_ID { get; set; }
        public string TYPE_IU { get; set; }
        public string SKUNO { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
    }
}