using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WHEN_SHIPPED : DataObjectTable
    {
        public T_R_WHEN_SHIPPED(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WHEN_SHIPPED(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WHEN_SHIPPED);
            TableName = "R_WHEN_SHIPPED".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public DataTable gettable(OleExec db)
        {
            DataTable dt = null;
            string sql = $@"select * from SFCRUNTIME.r_when_shipped ";
            dt = db.ExecuteDataTable(sql, CommandType.Text, null);
            return dt;

        }
    }
    public class Row_R_WHEN_SHIPPED : DataObjectBase
    {
        public Row_R_WHEN_SHIPPED(DataObjectInfo info) : base(info)
        {

        }
        public R_WHEN_SHIPPED GetDataObject()
        {
            R_WHEN_SHIPPED DataObject = new R_WHEN_SHIPPED();
            DataObject.ID = this.ID;
            DataObject.DN_NO = this.DN_NO;
            DataObject.PO_NO = this.PO_NO;
            DataObject.FILENAME = this.FILENAME;
            DataObject.SHIP_TO_ADDRESS = this.SHIP_TO_ADDRESS;
            DataObject.REMARK = this.REMARK;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.UPLOAD_EMP = this.UPLOAD_EMP;
            DataObject.UPLOAD_TIME = this.UPLOAD_TIME;
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
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
            }
        }
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
            }
        }
        public string SHIP_TO_ADDRESS
        {
            get
            {
                return (string)this["SHIP_TO_ADDRESS"];
            }
            set
            {
                this["SHIP_TO_ADDRESS"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
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
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
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
        public string UPLOAD_EMP
        {
            get
            {
                return (string)this["UPLOAD_EMP"];
            }
            set
            {
                this["UPLOAD_EMP"] = value;
            }
        }
        public DateTime? UPLOAD_TIME
        {
            get
            {
                return (DateTime?)this["UPLOAD_TIME"];
            }
            set
            {
                this["UPLOAD_TIME"] = value;
            }
        }
    }
    public class R_WHEN_SHIPPED
    {
        public string ID { get; set; }
        public string DN_NO { get; set; }
        public string PO_NO { get; set; }
        public string FILENAME { get; set; }
        public string SHIP_TO_ADDRESS { get; set; }
        public string REMARK { get; set; }
        public string VALID_FLAG { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string UPLOAD_EMP { get; set; }
        public DateTime? UPLOAD_TIME { get; set; }
    }
}