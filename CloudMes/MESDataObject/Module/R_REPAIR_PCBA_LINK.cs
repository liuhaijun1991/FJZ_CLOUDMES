using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_PCBA_LINK : DataObjectTable
    {
        public T_R_REPAIR_PCBA_LINK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_PCBA_LINK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_PCBA_LINK);
            TableName = "R_REPAIR_PCBA_LINK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int INRepairPCBALink(R_REPAIR_PCBA_LINK RepairPCBALinkINFO, OleExec DB)
        {
            return DB.ORM.Insertable<R_REPAIR_PCBA_LINK>(RepairPCBALinkINFO).ExecuteCommand();
        }
    }
    public class Row_R_REPAIR_PCBA_LINK : DataObjectBase
    {
        public Row_R_REPAIR_PCBA_LINK(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_PCBA_LINK GetDataObject()
        {
            R_REPAIR_PCBA_LINK DataObject = new R_REPAIR_PCBA_LINK();
            DataObject.ID = this.ID;
            DataObject.SN_ID = this.SN_ID;
            DataObject.SN = this.SN;
            DataObject.KP_SN = this.KP_SN;
            DataObject.CREATE_DATE = this.CREATE_DATE;
            DataObject.PROCESS = this.PROCESS;
            DataObject.LOCATION = this.LOCATION;
            DataObject.ROOT_CAUSE = this.ROOT_CAUSE;
            DataObject.DATA_CODE = this.DATA_CODE;
            DataObject.LOT_CODE = this.LOT_CODE;
            DataObject.COMPONENT_CODE = this.COMPONENT_CODE;
            DataObject.VENDOR_CODE = this.VENDOR_CODE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.REPAIR_EMP = this.REPAIR_EMP;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string SN_ID
        {
            get
            {
                return (string)this["SN_ID"];
            }
            set
            {
                this["SN_ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string KP_SN
        {
            get
            {
                return (string)this["KP_SN"];
            }
            set
            {
                this["KP_SN"] = value;
            }
        }
        public DateTime? CREATE_DATE
        {
            get
            {
                return (DateTime?)this["CREATE_DATE"];
            }
            set
            {
                this["CREATE_DATE"] = value;
            }
        }
        public string PROCESS
        {
            get
            {
                return (string)this["PROCESS"];
            }
            set
            {
                this["PROCESS"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public string ROOT_CAUSE
        {
            get
            {
                return (string)this["ROOT_CAUSE"];
            }
            set
            {
                this["ROOT_CAUSE"] = value;
            }
        }
        public string DATA_CODE
        {
            get
            {
                return (string)this["DATA_CODE"];
            }
            set
            {
                this["DATA_CODE"] = value;
            }
        }
        public string LOT_CODE
        {
            get
            {
                return (string)this["LOT_CODE"];
            }
            set
            {
                this["LOT_CODE"] = value;
            }
        }
        public string COMPONENT_CODE
        {
            get
            {
                return (string)this["COMPONENT_CODE"];
            }
            set
            {
                this["COMPONENT_CODE"] = value;
            }
        }
        public string VENDOR_CODE
        {
            get
            {
                return (string)this["VENDOR_CODE"];
            }
            set
            {
                this["VENDOR_CODE"] = value;
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
        public string REPAIR_EMP
        {
            get
            {
                return (string)this["REPAIR_EMP"];
            }
            set
            {
                this["REPAIR_EMP"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_REPAIR_PCBA_LINK
    {
        public string ID { get; set; }
        public string SN_ID { get; set; }
        public string SN { get; set; }
        public string KP_SN { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string PROCESS { get; set; }
        public string LOCATION { get; set; }
        public string ROOT_CAUSE { get; set; }
        public string DATA_CODE { get; set; }
        public string LOT_CODE { get; set; }
        public string COMPONENT_CODE { get; set; }
        public string VENDOR_CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string REPAIR_EMP { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}