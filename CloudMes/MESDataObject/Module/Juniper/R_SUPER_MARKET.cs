using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SUPER_MARKET : DataObjectTable
    {
        public T_R_SUPER_MARKET(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SUPER_MARKET(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SUPER_MARKET);
            TableName = "R_SUPER_MARKET".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SUPER_MARKET : DataObjectBase
    {
        public Row_R_SUPER_MARKET(DataObjectInfo info) : base(info)
        {

        }
        public R_SUPER_MARKET GetDataObject()
        {
            R_SUPER_MARKET DataObject = new R_SUPER_MARKET();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.IN_TIME = this.IN_TIME;
            DataObject.IN_BY = this.IN_BY;
            DataObject.R_SAP_MOVEMENTS_ID = this.R_SAP_MOVEMENTS_ID;
            DataObject.MOVEMENT_TYPE = this.MOVEMENT_TYPE;
            DataObject.OUT_TIME = this.OUT_TIME;
            DataObject.OUT_BY = this.OUT_BY;
            DataObject.STATUS = this.STATUS;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
            }
        }
        public DateTime? IN_TIME
        {
            get
            {
                return (DateTime?)this["IN_TIME"];
            }
            set
            {
                this["IN_TIME"] = value;
            }
        }
        public string IN_BY
        {
            get
            {
                return (string)this["IN_BY"];
            }
            set
            {
                this["IN_BY"] = value;
            }
        }
        public string R_SAP_MOVEMENTS_ID
        {
            get
            {
                return (string)this["R_SAP_MOVEMENTS_ID"];
            }
            set
            {
                this["R_SAP_MOVEMENTS_ID"] = value;
            }
        }
        public string MOVEMENT_TYPE
        {
            get
            {
                return (string)this["MOVEMENT_TYPE"];
            }
            set
            {
                this["MOVEMENT_TYPE"] = value;
            }
        }
        public DateTime? OUT_TIME
        {
            get
            {
                return (DateTime?)this["OUT_TIME"];
            }
            set
            {
                this["OUT_TIME"] = value;
            }
        }
        public string OUT_BY
        {
            get
            {
                return (string)this["OUT_BY"];
            }
            set
            {
                this["OUT_BY"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
    }
    public class R_SUPER_MARKET
    {
        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public DateTime? IN_TIME { get; set; }
        public string IN_BY { get; set; }
        public string R_SAP_MOVEMENTS_ID { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public DateTime? OUT_TIME { get; set; }
        public string OUT_BY { get; set; }
        public string STATUS { get; set; }
    }
}