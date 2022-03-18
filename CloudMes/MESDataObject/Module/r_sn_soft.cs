using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_r_sn_soft : DataObjectTable
    {
        public T_r_sn_soft(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_r_sn_soft(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_r_sn_soft);
            TableName = "r_sn_soft".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_r_sn_soft : DataObjectBase
    {
        public Row_r_sn_soft(DataObjectInfo info) : base(info)
        {

        }
        public r_sn_soft GetDataObject()
        {
            r_sn_soft DataObject = new r_sn_soft();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WO = this.WO;
            DataObject.SOFT_ITEM_CODE = this.SOFT_ITEM_CODE;
            DataObject.SOFT_LOCATION = this.SOFT_LOCATION;
            DataObject.SOFT_REVISION = this.SOFT_REVISION;
            DataObject.DATA1 = this.DATA1;
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
        public string SOFT_ITEM_CODE
        {
            get
            {
                return (string)this["SOFT_ITEM_CODE"];
            }
            set
            {
                this["SOFT_ITEM_CODE"] = value;
            }
        }
        public string SOFT_LOCATION
        {
            get
            {
                return (string)this["SOFT_LOCATION"];
            }
            set
            {
                this["SOFT_LOCATION"] = value;
            }
        }
        public string SOFT_REVISION
        {
            get
            {
                return (string)this["SOFT_REVISION"];
            }
            set
            {
                this["SOFT_REVISION"] = value;
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
    public class r_sn_soft
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string WO { get; set; }
        public string SOFT_ITEM_CODE { get; set; }
        public string SOFT_LOCATION { get; set; }
        public string SOFT_REVISION { get; set; }
        public string DATA1 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}