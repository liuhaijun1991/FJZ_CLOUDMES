using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_WO_MPN : DataObjectTable
    {
        public T_C_WO_MPN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_WO_MPN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_WO_MPN);
            TableName = "C_WO_MPN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_WO_MPN : DataObjectBase
    {
        public Row_C_WO_MPN(DataObjectInfo info) : base(info)
        {

        }
        public C_WO_MPN GetDataObject()
        {
            C_WO_MPN DataObject = new C_WO_MPN();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.MPN = this.MPN;
            DataObject.MFRCODE = this.MFRCODE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
            }
        }
        public string MFRCODE
        {
            get
            {
                return (string)this["MFRCODE"];
            }
            set
            {
                this["MFRCODE"] = value;
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
    }
    public class C_WO_MPN
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string PARTNO { get; set; }
        public string MPN { get; set; }
        public string MFRCODE { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}