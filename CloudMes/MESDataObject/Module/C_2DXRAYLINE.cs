using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_2DXRAYLINE : DataObjectTable
    {
        public T_C_2DXRAYLINE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_2DXRAYLINE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_2DXRAYLINE);
            TableName = "C_2DXRAYLINE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_2DXRAYLINE : DataObjectBase
    {
        public Row_C_2DXRAYLINE(DataObjectInfo info) : base(info)
        {

        }
        public C_2DXRAYLINE GetDataObject()
        {
            C_2DXRAYLINE DataObject = new C_2DXRAYLINE();
            DataObject.LINENAME = this.LINENAME;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.ID = this.ID;
            DataObject.CUSTOMER = this.CUSTOMER;
            DataObject.BUILDING = this.BUILDING;
            return DataObject;
        }
        public string LINENAME
        {
            get
            {
                return (string)this["LINENAME"];
            }
            set
            {
                this["LINENAME"] = value;
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
        public string CUSTOMER
        {
            get
            {
                return (string)this["CUSTOMER"];
            }
            set
            {
                this["CUSTOMER"] = value;
            }
        }
        public string BUILDING
        {
            get
            {
                return (string)this["BUILDING"];
            }
            set
            {
                this["BUILDING"] = value;
            }
        }
    }
    public class C_2DXRAYLINE
    {
        public string LINENAME { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string ID { get; set; }
        public string CUSTOMER { get; set; }
        public string BUILDING { get; set; }
    }
}