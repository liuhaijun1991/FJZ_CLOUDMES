using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_137_COO_LABEL : DataObjectTable
    {
        public T_O_137_COO_LABEL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_137_COO_LABEL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_137_COO_LABEL);
            TableName = "O_137_COO_LABEL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_137_COO_LABEL : DataObjectBase
    {
        public Row_O_137_COO_LABEL(DataObjectInfo info) : base(info)
        {

        }
        public O_137_COO_LABEL GetDataObject()
        {
            O_137_COO_LABEL DataObject = new O_137_COO_LABEL();
            DataObject.ID = this.ID;
            DataObject.COOVALUE = this.COOVALUE;
            DataObject.PARTNO = this.PARTNO;
            DataObject.DESCRIPTIONS = this.DESCRIPTIONS;
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
        public string COOVALUE
        {
            get
            {
                return (string)this["COOVALUE"];
            }
            set
            {
                this["COOVALUE"] = value;
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
        public string DESCRIPTIONS
        {
            get
            {
                return (string)this["DESCRIPTIONS"];
            }
            set
            {
                this["DESCRIPTIONS"] = value;
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
    public class O_137_COO_LABEL
    {
        public string ID { get; set; }
        public string COOVALUE { get; set; }
        public string PARTNO { get; set; }
        public string DESCRIPTIONS { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}