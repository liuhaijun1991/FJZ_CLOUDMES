using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_OBA_CONTROL : DataObjectTable
    {
        public T_R_OBA_CONTROL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_OBA_CONTROL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_OBA_CONTROL);
            TableName = "R_OBA_CONTROL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_OBA_CONTROL : DataObjectBase
    {
        public Row_R_OBA_CONTROL(DataObjectInfo info) : base(info)
        {

        }
        public R_OBA_CONTROL GetDataObject()
        {
            R_OBA_CONTROL DataObject = new R_OBA_CONTROL();
            DataObject.ID = this.ID;
            DataObject.LOTNO = this.LOTNO;
            DataObject.PHASE = this.PHASE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SN = this.SN;
            DataObject.ADDRESS = this.ADDRESS;
            DataObject.CONTROL_DATE = this.CONTROL_DATE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_DATE = this.EDIT_DATE;
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
        public string LOTNO
        {
            get
            {
                return (string)this["LOTNO"];
            }
            set
            {
                this["LOTNO"] = value;
            }
        }
        public string PHASE
        {
            get
            {
                return (string)this["PHASE"];
            }
            set
            {
                this["PHASE"] = value;
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
        public string ADDRESS
        {
            get
            {
                return (string)this["ADDRESS"];
            }
            set
            {
                this["ADDRESS"] = value;
            }
        }
        public DateTime? CONTROL_DATE
        {
            get
            {
                return (DateTime?)this["CONTROL_DATE"];
            }
            set
            {
                this["CONTROL_DATE"] = value;
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
        public DateTime? EDIT_DATE
        {
            get
            {
                return (DateTime?)this["EDIT_DATE"];
            }
            set
            {
                this["EDIT_DATE"] = value;
            }
        }
    }
    public class R_OBA_CONTROL
    {
        public string ID { get; set; }
        public string LOTNO { get; set; }
        public string PHASE { get; set; }
        public string SKUNO { get; set; }
        public string SN { get; set; }
        public string ADDRESS { get; set; }
        public DateTime? CONTROL_DATE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_DATE { get; set; }
    }
}