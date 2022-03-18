using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_PCBA_RELATIONSHIP : DataObjectTable
    {
        public T_R_REPAIR_PCBA_RELATIONSHIP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_PCBA_RELATIONSHIP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_PCBA_RELATIONSHIP);
            TableName = "R_REPAIR_PCBA_RELATIONSHIP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_REPAIR_PCBA_RELATIONSHIP : DataObjectBase
    {
        public Row_R_REPAIR_PCBA_RELATIONSHIP(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_PCBA_RELATIONSHIP GetDataObject()
        {
            R_REPAIR_PCBA_RELATIONSHIP DataObject = new R_REPAIR_PCBA_RELATIONSHIP();
            DataObject.ID = this.ID;
            DataObject.REPAIR_ACTION_ID = this.REPAIR_ACTION_ID;
            DataObject.PARENT_SN = this.PARENT_SN;
            DataObject.REPAIRED_SN = this.REPAIRED_SN;
            DataObject.WO_FLAG = this.WO_FLAG;
            DataObject.WORKORDERNO_ID = this.WORKORDERNO_ID;
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
        public string REPAIR_ACTION_ID
        {
            get
            {
                return (string)this["REPAIR_ACTION_ID"];
            }
            set
            {
                this["REPAIR_ACTION_ID"] = value;
            }
        }
        public string PARENT_SN
        {
            get
            {
                return (string)this["PARENT_SN"];
            }
            set
            {
                this["PARENT_SN"] = value;
            }
        }
        public string REPAIRED_SN
        {
            get
            {
                return (string)this["REPAIRED_SN"];
            }
            set
            {
                this["REPAIRED_SN"] = value;
            }
        }

        public string WO_FLAG
        {
            get
            {
                return (string)this["WO_FLAG"];
            }
            set
            {
                this["WO_FLAG"] = value;
            }
        }

        public string WORKORDERNO_ID
        {
            get
            {
                return (string)this["WORKORDERNO_ID"];
            }
            set
            {
                this["WORKORDERNO_ID"] = value;
            }
        }
    }
    public class R_REPAIR_PCBA_RELATIONSHIP
    {
        public string ID { get; set; }
        public string REPAIR_ACTION_ID { get; set; }
        public string PARENT_SN { get; set; }
        public string REPAIRED_SN { get; set; }
        public string WO_FLAG { get; set; }
        public string WORKORDERNO_ID { get; set; }

    }
}