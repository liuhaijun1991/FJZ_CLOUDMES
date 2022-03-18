using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.OM
{
    public class T_O_ORDER_OPTION : DataObjectTable
    {
        public T_O_ORDER_OPTION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_ORDER_OPTION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_ORDER_OPTION);
            TableName = "O_ORDER_OPTION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_ORDER_OPTION : DataObjectBase
    {
        public Row_O_ORDER_OPTION(DataObjectInfo info) : base(info)
        {

        }
        public O_ORDER_OPTION GetDataObject()
        {
            O_ORDER_OPTION DataObject = new O_ORDER_OPTION();
            DataObject.ID = this.ID;
            DataObject.MAINID = this.MAINID;
            DataObject.PARTNO = this.PARTNO;
            DataObject.QTY = this.QTY;
            DataObject.PITEMID = this.PITEMID;
            DataObject.CITEMID = this.CITEMID;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string PITEMID
        {
            get
            {
                return (string)this["PITEMID"];
            }
            set
            {
                this["PITEMID"] = value;
            }
        }
        public string CITEMID
        {
            get
            {
                return (string)this["CITEMID"];
            }
            set
            {
                this["CITEMID"] = value;
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
    public class O_ORDER_OPTION
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string MAINID { get; set; }
        public string PARTNO { get; set; }
        public double? QTY { get; set; }
        public string PITEMID { get; set; }
        public string CITEMID { get; set; }
        public string OPTIONTYPE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string FOXPN { get; set; }        
    }

    public enum ENUM_O_ORDER_OPTION
    {
        [EnumValue("CTO-OPTION")]
        CTO,
        [EnumValue("POWERCODE")]
        POWERCODE,
        [EnumValue("BNDL")]
        BNDL,
    }
}