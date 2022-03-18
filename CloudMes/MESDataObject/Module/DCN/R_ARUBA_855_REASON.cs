using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_ARUBA_855_REASON : DataObjectTable
    {
        public T_R_ARUBA_855_REASON(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ARUBA_855_REASON(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ARUBA_855_REASON);
            TableName = "R_ARUBA_855_REASON".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ARUBA_855_REASON : DataObjectBase
    {
        public Row_R_ARUBA_855_REASON(DataObjectInfo info) : base(info)
        {

        }
        public R_ARUBA_855_REASON GetDataObject()
        {
            R_ARUBA_855_REASON DataObject = new R_ARUBA_855_REASON();
            DataObject.ID = this.ID;
            DataObject.CODE = this.CODE;
            DataObject.DESCRIP = this.DESCRIP;
            DataObject.CATEGORY = this.CATEGORY;
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
        public string CODE
        {
            get
            {
                return (string)this["CODE"];
            }
            set
            {
                this["CODE"] = value;
            }
        }
        public string DESCRIP
        {
            get
            {
                return (string)this["DESCRIP"];
            }
            set
            {
                this["DESCRIP"] = value;
            }
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
    }
    public class R_ARUBA_855_REASON
    {
        public string ID { get; set; }
        public string CODE { get; set; }
        public string DESCRIP { get; set; }
        public string CATEGORY { get; set; }
    }
}