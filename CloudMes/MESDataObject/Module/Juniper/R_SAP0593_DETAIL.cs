using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
 namespace MESDataObject.Module.Juniper
{
    public class T_R_SAP0593_DETAIL : DataObjectTable
    {
        public T_R_SAP0593_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP0593_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP0593_DETAIL);
            TableName = "R_SAP0593_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP0593_DETAIL : DataObjectBase
    {
        public Row_R_SAP0593_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP0593_DETAIL GetDataObject()
        {
            R_SAP0593_DETAIL DataObject = new R_SAP0593_DETAIL();
            DataObject.ID = this.ID;
            DataObject.HEADID = this.HEADID;
            DataObject.MATNR = this.MATNR;
            DataObject.DAYS = this.DAYS;
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
        public string HEADID
        {
            get
            {
                return (string)this["HEADID"];
            }
            set
            {
                this["HEADID"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string DAYS
        {
            get
            {
                return (string)this["DAYS"];
            }
            set
            {
                this["DAYS"] = value;
            }
        }
    }
    public class R_SAP0593_DETAIL
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string HEADID { get; set; }
        public string MATNR { get; set; }
        public string DAYS { get; set; }
        public string QTY { get; set; }
        public string DAYS_C { get; set; }        
    }
}