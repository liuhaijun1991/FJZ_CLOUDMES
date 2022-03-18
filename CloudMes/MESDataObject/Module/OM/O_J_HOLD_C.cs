using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_J_HOLD_C : DataObjectTable
    {
        public T_O_J_HOLD_C(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_J_HOLD_C(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_J_HOLD_C);
            TableName = "O_J_HOLD_C".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_J_HOLD_C : DataObjectBase
    {
        public Row_O_J_HOLD_C(DataObjectInfo info) : base(info)
        {

        }
        public O_J_HOLD_C GetDataObject()
        {
            O_J_HOLD_C DataObject = new O_J_HOLD_C();
            DataObject.ID = this.ID;
            DataObject.HOLDCODE = this.HOLDCODE;
            DataObject.CREATEWO = this.CREATEWO;
            DataObject.PRODUCTION = this.PRODUCTION;
            DataObject.PREASN = this.PREASN;
            DataObject.FINALASN = this.FINALASN;
            DataObject.I138 = this.I138;
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
        public string HOLDCODE
        {
            get
            {
                return (string)this["HOLDCODE"];
            }
            set
            {
                this["HOLDCODE"] = value;
            }
        }
        public string CREATEWO
        {
            get
            {
                return (string)this["CREATEWO"];
            }
            set
            {
                this["CREATEWO"] = value;
            }
        }
        public string PRODUCTION
        {
            get
            {
                return (string)this["PRODUCTION"];
            }
            set
            {
                this["PRODUCTION"] = value;
            }
        }
        public string PREASN
        {
            get
            {
                return (string)this["PREASN"];
            }
            set
            {
                this["PREASN"] = value;
            }
        }
        public string FINALASN
        {
            get
            {
                return (string)this["FINALASN"];
            }
            set
            {
                this["FINALASN"] = value;
            }
        }
        public string I138
        {
            get
            {
                return (string)this["I138"];
            }
            set
            {
                this["I138"] = value;
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
    public class O_J_HOLD_C
    {
        public string ID { get; set; }
        public string HOLDCODE { get; set; }
        public string CREATEWO { get; set; }
        public string PRODUCTION { get; set; }
        public string PREASN { get; set; }
        public string FINALASN { get; set; }
        public string I138 { get; set; }
        public DateTime? CREATETIME { get; set; }
    }

    public enum ENUM_O_ORDER_HOLD_CONTROLTYPE
    {
        CREATEWO,
        PRODUCTION,
        PREASN,
        FINALASN,
        I138
    }
}
