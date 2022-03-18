using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_PRE_WO_DETAIL : DataObjectTable
    {
        public T_R_PRE_WO_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PRE_WO_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PRE_WO_DETAIL);
            TableName = "R_PRE_WO_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PRE_WO_DETAIL : DataObjectBase
    {
        public Row_R_PRE_WO_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_PRE_WO_DETAIL GetDataObject()
        {
            R_PRE_WO_DETAIL DataObject = new R_PRE_WO_DETAIL();
            //DataObject.CUSTPARTNO = this.CUSTPARTNO;
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.PARTNO = this.PARTNO;
            //DataObject.REQUESTQTY = this.REQUESTQTY;
            DataObject.UNITPRICE = this.UNITPRICE;
            DataObject.UNITWEIGHT = this.UNITWEIGHT;
            DataObject.PACKAGEFLAG = this.PACKAGEFLAG;
            DataObject.PARTNOTYPE = this.PARTNOTYPE;
            DataObject.CREATETIME = this.CREATETIME;
            return DataObject;
        }
        public string CUSTPARTNO
        {
            get
            {
                return (string)this["CUSTPARTNO"];
            }
            set
            {
                this["CUSTPARTNO"] = value;
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
        public double? REQUESTQTY
        {
            get
            {
                return (double?)this["REQUESTQTY"];
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
    [Serializable]
    public class R_PRE_WO_DETAIL
    {
        //public string CUSTPARTNO { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string WO { get; set; }
        public string PARTNO { get; set; }
        public string REQUESTQTY { get; set; }
        public string UNITPRICE { get; set; }
        public string UNITWEIGHT { get; set; }
        public string PACKAGEFLAG { get; set; }
        public string PARTNOTYPE { get; set; }
        public DateTime? CREATETIME { get; set; }
    }

    public enum ENUM_R_PRE_WO_DETAIL
    {
        [EnumValue("SAPEX")]
        SAPEX,
        [EnumValue("POWERCODE")]
        POWERCODE,
        [EnumValue("BNDL")]
        BNDL,
        [EnumValue("COUNTRYLABEL")]
        COUNTRYLABEL,
        [EnumValue("CARTONLABEL")]
        CARTONLABEL,
        [EnumValue("PACKAGE")]
        PACKAGE,
        [EnumValue("COMPONENTS")]
        COMPONENTS,
        [EnumValue("BASE")]
        BASE,
        [EnumValue("OTHER")]
        OTHER
    }

}