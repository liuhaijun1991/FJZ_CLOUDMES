using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_MCSN : DataObjectTable
    {
        public T_C_MCSN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_MCSN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_MCSN);
            TableName = "C_MCSN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_MCSN : DataObjectBase
    {
        public Row_C_MCSN(DataObjectInfo info) : base(info)
        {

        }
        public C_MCSN GetDataObject()
        {
            C_MCSN DataObject = new C_MCSN();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.TYPE = this.TYPE;
            DataObject.SERVICE = this.SERVICE;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
        public string SERVICE
        {
            get
            {
                return (string)this["SERVICE"];
            }
            set
            {
                this["SERVICE"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
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
    public class C_MCSN
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string TYPE { get; set; }
        public string SERVICE { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}