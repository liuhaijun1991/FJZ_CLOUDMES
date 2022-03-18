using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MATERIAL_STAGE_CONFIRM : DataObjectTable
    {
        public T_R_MATERIAL_STAGE_CONFIRM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MATERIAL_STAGE_CONFIRM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MATERIAL_STAGE_CONFIRM);
            TableName = "R_MATERIAL_STAGE_CONFIRM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MATERIAL_STAGE_CONFIRM : DataObjectBase
    {
        public Row_R_MATERIAL_STAGE_CONFIRM(DataObjectInfo info) : base(info)
        {

        }
        public R_MATERIAL_STAGE_CONFIRM GetDataObject()
        {
            R_MATERIAL_STAGE_CONFIRM DataObject = new R_MATERIAL_STAGE_CONFIRM();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.STATION = this.STATION;
            DataObject.ROUTE = this.ROUTE;
            DataObject.FILENAME = this.FILENAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.TOTALNUM = this.TOTALNUM;
            DataObject.USEDNUM = this.USEDNUM;
            DataObject.REMAINNUM = this.REMAINNUM;
            DataObject.UPLOADBY = this.UPLOADBY;
            DataObject.UPLOADTIME = this.UPLOADTIME;
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.FLAG = this.FLAG;
            DataObject.REASON = this.REASON;
            DataObject.SIGNBY = this.SIGNBY;
            DataObject.SIGNTIME = this.SIGNTIME;
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
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string ROUTE
        {
            get
            {
                return (string)this["ROUTE"];
            }
            set
            {
                this["ROUTE"] = value;
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
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string TOTALNUM
        {
            get
            {
                return (string)this["TOTALNUM"];
            }
            set
            {
                this["TOTALNUM"] = value;
            }
        }
        public string USEDNUM
        {
            get
            {
                return (string)this["USEDNUM"];
            }
            set
            {
                this["USEDNUM"] = value;
            }
        }
        public string REMAINNUM
        {
            get
            {
                return (string)this["REMAINNUM"];
            }
            set
            {
                this["REMAINNUM"] = value;
            }
        }
        public string UPLOADBY
        {
            get
            {
                return (string)this["UPLOADBY"];
            }
            set
            {
                this["UPLOADBY"] = value;
            }
        }
        public DateTime? UPLOADTIME
        {
            get
            {
                return (DateTime?)this["UPLOADTIME"];
            }
            set
            {
                this["UPLOADTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
            }
        }
        public string REASON
        {
            get
            {
                return (string)this["REASON"];
            }
            set
            {
                this["REASON"] = value;
            }
        }
        public string SIGNBY
        {
            get
            {
                return (string)this["SIGNBY"];
            }
            set
            {
                this["SIGNBY"] = value;
            }
        }
        public DateTime? SIGNTIME
        {
            get
            {
                return (DateTime?)this["SIGNTIME"];
            }
            set
            {
                this["SIGNTIME"] = value;
            }
        }
    }
    public class R_MATERIAL_STAGE_CONFIRM
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string WORKORDERNO { get; set; }
        public string STATION { get; set; }
        public string ROUTE { get; set; }
        public string FILENAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string TOTALNUM { get; set; }
        public string USEDNUM { get; set; }
        public string REMAINNUM { get; set; }
        public string UPLOADBY { get; set; }
        public DateTime? UPLOADTIME { get; set; }
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string FLAG { get; set; }
        public string REASON { get; set; }
        public string SIGNBY { get; set; }
        public DateTime? SIGNTIME { get; set; }
    }
}