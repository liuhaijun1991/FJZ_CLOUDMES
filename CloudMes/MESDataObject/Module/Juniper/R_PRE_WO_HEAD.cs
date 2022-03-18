using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.Juniper
{
    public class T_R_PRE_WO_HEAD : DataObjectTable
    {
        public T_R_PRE_WO_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PRE_WO_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PRE_WO_HEAD);
            TableName = "R_PRE_WO_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_PRE_WO_HEAD GetObjectByWo(OleExec SFCDB,string wo)
        {
            return SFCDB.ORM.Queryable<R_PRE_WO_HEAD>().Where(r => r.WO == wo).ToList().FirstOrDefault();
        }
    }
    public class Row_R_PRE_WO_HEAD : DataObjectBase
    {
        public Row_R_PRE_WO_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_PRE_WO_HEAD GetDataObject()
        {
            R_PRE_WO_HEAD DataObject = new R_PRE_WO_HEAD();
            //DataObject.VALID = this.VALID;
            DataObject.PLANT = this.PLANT;
            DataObject.CUSTPN = this.CUSTPN;
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.GROUPID = this.GROUPID;
            DataObject.WOQTY = this.WOQTY;
            DataObject.PONO = this.PONO;
            DataObject.POLINE = this.POLINE;
            DataObject.PID = this.PID;
            DataObject.TOTUNITPRICE = this.TOTUNITPRICE;
            DataObject.SAPFLAG = this.SAPFLAG;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.MAINID = this.MAINID;
            DataObject.CREATETIME = this.CREATETIME;
            return DataObject;
        }
        public string VALID
        {
            get
            {
                return (string)this["VALID"];
            }
            set
            {
                this["VALID"] = value;
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
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string CUSTPN
        {
            get
            {
                return (string)this["CUSTPN"];
            }
            set
            {
                this["CUSTPN"] = value;
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
        public string WOQTY
        {
            get
            {
                return (string)this["WOQTY"];
            }
            set
            {
                this["WOQTY"] = value;
            }
        }
        public string PONO
        {
            get
            {
                return (string)this["PONO"];
            }
            set
            {
                this["PONO"] = value;
            }
        }
        public string POLINE
        {
            get
            {
                return (string)this["POLINE"];
            }
            set
            {
                this["POLINE"] = value;
            }
        }
        public string PID
        {
            get
            {
                return (string)this["PID"];
            }
            set
            {
                this["PID"] = value;
            }
        }
        public string TOTUNITPRICE
        {
            get
            {
                return (string)this["TOTUNITPRICE"];
            }
            set
            {
                this["TOTUNITPRICE"] = value;
            }
        }
        public string SAPFLAG
        {
            get
            {
                return (string)this["SAPFLAG"];
            }
            set
            {
                this["SAPFLAG"] = value;
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
        public string MAINID
        {
            get
            {
                return (string)this["MAINID"];
            }
            set
            {
                this["MAINID"] = value;
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
    public class R_PRE_WO_HEAD
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string WO { get; set; }
        public string GROUPID { get; set; }
        public string WOQTY { get; set; }
        public string PONO { get; set; }
        public string POLINE { get; set; }
        public string PID { get; set; }
        public string TOTUNITPRICE { get; set; }
        public string SAPFLAG { get; set; }
        public string DESCRIPTION { get; set; }
        public string MAINID { get; set; }
        public string PLANT { get; set; }
        public string CUSTPN { get; set; }
        public DateTime? CREATETIME { get; set; }
    }


    public enum ENUM_R_PRE_WO_HEAD
    {
        [EnumValue("0")]
        PREONE_UPLOADSAP_NO,
        [EnumValue("1")]
        PREONE_UPLOADSAP_YES,
        [EnumValue("2")]
        PRESEC_UPLOADSAP_NO,
        [EnumValue("3")]
        PRESEC_UPLOADSAP_YES,
        [EnumValue("4")]
        CREATEWO_NO,
        [EnumValue("5")]
        CREATEWO_YES,
    }

    public enum ENUM_J_WOTYPE
    {
        [EnumValue("007A")]
        [EnumName("ZJ09")]
        ZJ09,
        [EnumValue("007B")]
        [EnumName("ZJ10")]
        ZJ10,
        [EnumValue("007C")]
        [EnumName("ZJ11")]
        ZJ11,
        [EnumValue("007D")]
        [EnumName("ZJ12")]
        ZJ12,
        [EnumValue("007L")]
        [EnumName("ZJ13")]
        ZJ13,        
        [EnumName("ZVJ1")]
        [EnumValue("007G")]
        ZVJ1,
        [EnumValue("007H")]
        [EnumName("ZVJ2")]
        ZVJ2,
        [EnumValue("007I")]
        [EnumName("ZVJ3")]
        ZVJ3
    }
}