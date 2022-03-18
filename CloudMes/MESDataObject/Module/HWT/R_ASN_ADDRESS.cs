using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_ASN_ADDRESS : DataObjectTable
    {
        public T_R_ASN_ADDRESS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ASN_ADDRESS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ASN_ADDRESS);
            TableName = "R_ASN_ADDRESS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ASN_ADDRESS : DataObjectBase
    {
        public Row_R_ASN_ADDRESS(DataObjectInfo info) : base(info)
        {

        }
        public R_ASN_ADDRESS GetDataObject()
        {
            R_ASN_ADDRESS DataObject = new R_ASN_ADDRESS();
            DataObject.ID = this.ID;
            DataObject.ADDRESS = this.ADDRESS;
            DataObject.ASN_FIRST = this.ASN_FIRST;
            DataObject.SHIPTOCODE = this.SHIPTOCODE;
            DataObject.DATA2 = this.DATA2;
            DataObject.DELIVER_ATTN = this.DELIVER_ATTN;
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
        public string ASN_FIRST
        {
            get
            {
                return (string)this["ASN_FIRST"];
            }
            set
            {
                this["ASN_FIRST"] = value;
            }
        }
        public string SHIPTOCODE
        {
            get
            {
                return (string)this["SHIPTOCODE"];
            }
            set
            {
                this["SHIPTOCODE"] = value;
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
        public string DELIVER_ATTN
        {
            get
            {
                return (string)this["DELIVER_ATTN"];
            }
            set
            {
                this["DELIVER_ATTN"] = value;
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
    public class R_ASN_ADDRESS
    {
        public string ID { get; set; }
        public string ADDRESS { get; set; }
        public string ASN_FIRST { get; set; }
        public string SHIPTOCODE { get; set; }
        public string DATA2 { get; set; }
        public string DELIVER_ATTN { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}