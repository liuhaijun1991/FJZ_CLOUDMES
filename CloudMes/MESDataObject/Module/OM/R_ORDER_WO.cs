using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_R_ORDER_WO : DataObjectTable
    {
        public T_R_ORDER_WO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORDER_WO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORDER_WO);
            TableName = "R_ORDER_WO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORDER_WO : DataObjectBase
    {
        public Row_R_ORDER_WO(DataObjectInfo info) : base(info)
        {

        }
        public R_ORDER_WO GetDataObject()
        {
            R_ORDER_WO DataObject = new R_ORDER_WO();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.UPOID = this.UPOID;
            DataObject.ORIGINALID = this.ORIGINALID;
            DataObject.DISMANTLE = this.DISMANTLE;
            DataObject.VALID = this.VALID;
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
        public string UPOID
        {
            get
            {
                return (string)this["UPOID"];
            }
            set
            {
                this["UPOID"] = value;
            }
        }
        public string ORIGINALID
        {
            get
            {
                return (string)this["ORIGINALID"];
            }
            set
            {
                this["ORIGINALID"] = value;
            }
        }
        public string DISMANTLE
        {
            get
            {
                return (string)this["DISMANTLE"];
            }
            set
            {
                this["DISMANTLE"] = value;
            }
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
    public class R_ORDER_WO
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string UPOID { get; set; }
        public string ORIGINALID { get; set; }
        public string DISMANTLE { get; set; }
        public string VALID { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
    }
}