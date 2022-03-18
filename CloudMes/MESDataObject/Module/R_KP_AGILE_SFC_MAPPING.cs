using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_KP_AGILE_SFC_MAPPING : DataObjectTable
    {
        public T_R_KP_AGILE_SFC_MAPPING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_KP_AGILE_SFC_MAPPING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_KP_AGILE_SFC_MAPPING);
            TableName = "R_KP_AGILE_SFC_MAPPING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_KP_AGILE_SFC_MAPPING : DataObjectBase
    {
        public Row_R_KP_AGILE_SFC_MAPPING(DataObjectInfo info) : base(info)
        {

        }
        public R_KP_AGILE_SFC_MAPPING GetDataObject()
        {
            R_KP_AGILE_SFC_MAPPING DataObject = new R_KP_AGILE_SFC_MAPPING();
            DataObject.CUST_KP_NO = this.CUST_KP_NO;
            DataObject.FOX_KP_NO = this.FOX_KP_NO;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string CUST_KP_NO
        {
            get
            {
                return (string)this["CUST_KP_NO"];
            }
            set
            {
                this["CUST_KP_NO"] = value;
            }
        }
        public string FOX_KP_NO
        {
            get
            {
                return (string)this["FOX_KP_NO"];
            }
            set
            {
                this["FOX_KP_NO"] = value;
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
    public class R_KP_AGILE_SFC_MAPPING
    {
        public string CUST_KP_NO { get; set; }
        public string FOX_KP_NO { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}