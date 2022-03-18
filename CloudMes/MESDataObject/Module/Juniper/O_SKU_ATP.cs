using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_O_SKU_ATP : DataObjectTable
    {
        public T_O_SKU_ATP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_SKU_ATP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_SKU_ATP);
            TableName = "O_SKU_ATP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_SKU_ATP : DataObjectBase
    {
        public Row_O_SKU_ATP(DataObjectInfo info) : base(info)
        {

        }
        public O_SKU_ATP GetDataObject()
        {
            O_SKU_ATP DataObject = new O_SKU_ATP();
            DataObject.ID = this.ID;
            DataObject.CUSTPN = this.CUSTPN;
            DataObject.FOXPN = this.FOXPN;
            DataObject.ATPTYPE = this.ATPTYPE;
            DataObject.PARTNO = this.PARTNO;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string FOXPN
        {
            get
            {
                return (string)this["FOXPN"];
            }
            set
            {
                this["FOXPN"] = value;
            }
        }
        public string ATPTYPE
        {
            get
            {
                return (string)this["ATPTYPE"];
            }
            set
            {
                this["ATPTYPE"] = value;
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
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
    }
    public class O_SKU_ATP
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string CUSTPN { get; set; }
        public string FOXPN { get; set; }
        public string ATPTYPE { get; set; }
        public string PARTNO { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}