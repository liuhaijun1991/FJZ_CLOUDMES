using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_TO_DETAIL : DataObjectTable
    {
        public T_R_TO_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TO_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TO_DETAIL);
            TableName = "R_TO_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_TO_DETAIL : DataObjectBase
    {
        public Row_R_TO_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_TO_DETAIL GetDataObject()
        {
            R_TO_DETAIL DataObject = new R_TO_DETAIL();
            DataObject.ID = this.ID;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.DN_CUSTOMER = this.DN_CUSTOMER;
            DataObject.DN_NO = this.DN_NO;
            DataObject.TO_ITEM_NO = this.TO_ITEM_NO;
            DataObject.TO_NO = this.TO_NO;
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
        public string DN_CUSTOMER
        {
            get
            {
                return (string)this["DN_CUSTOMER"];
            }
            set
            {
                this["DN_CUSTOMER"] = value;
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
        public string TO_ITEM_NO
        {
            get
            {
                return (string)this["TO_ITEM_NO"];
            }
            set
            {
                this["TO_ITEM_NO"] = value;
            }
        }
        public string TO_NO
        {
            get
            {
                return (string)this["TO_NO"];
            }
            set
            {
                this["TO_NO"] = value;
            }
        }
    }
    public class R_TO_DETAIL
    {
        public string ID{ get; set; }
        public DateTime? CREATETIME{ get; set; }
        public string DN_CUSTOMER{ get; set; }
        public string DN_NO{ get; set; }
        public string TO_ITEM_NO{ get; set; }
        public string TO_NO{ get; set; }
    }
}