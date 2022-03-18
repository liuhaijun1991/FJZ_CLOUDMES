using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BRCD_EXSN : DataObjectTable
    {
        public T_R_BRCD_EXSN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BRCD_EXSN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BRCD_EXSN);
            TableName = "R_BRCD_EXSN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_BRCD_EXSN : DataObjectBase
    {
        public Row_R_BRCD_EXSN(DataObjectInfo info) : base(info)
        {

        }
        public R_BRCD_EXSN GetDataObject()
        {
            R_BRCD_EXSN DataObject = new R_BRCD_EXSN();
            DataObject.ID = this.ID;
            DataObject.SERVICE_NO = this.SERVICE_NO;
            DataObject.SN = this.SN;
            DataObject.USE_FLAG = this.USE_FLAG;
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
        public string SERVICE_NO
        {
            get
            {
                return (string)this["SERVICE_NO"];
            }
            set
            {
                this["SERVICE_NO"] = value;
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
        public string USE_FLAG
        {
            get
            {
                return (string)this["USE_FLAG"];
            }
            set
            {
                this["USE_FLAG"] = value;
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
    public class R_BRCD_EXSN
    {
        public string ID { get; set; }
        public string SERVICE_NO { get; set; }
        public string SN { get; set; }
        public string USE_FLAG { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}