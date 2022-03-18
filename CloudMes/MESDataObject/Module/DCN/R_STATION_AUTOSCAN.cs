using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_STATION_AUTOSCAN : DataObjectTable
    {
        public T_R_STATION_AUTOSCAN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_STATION_AUTOSCAN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_STATION_AUTOSCAN);
            TableName = "R_STATION_AUTOSCAN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_STATION_AUTOSCAN : DataObjectBase
    {
        public Row_R_STATION_AUTOSCAN(DataObjectInfo info) : base(info)
        {

        }
        public R_STATION_AUTOSCAN GetDataObject()
        {
            R_STATION_AUTOSCAN DataObject = new R_STATION_AUTOSCAN();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.ROUTEID = this.ROUTEID;
            DataObject.PACK_TYPE = this.PACK_TYPE;
            DataObject.LABEL_TYPE = this.LABEL_TYPE;
            DataObject.CURRENTPOINT = this.CURRENTPOINT;
            DataObject.NEXTPOINT = this.NEXTPOINT;
            DataObject.PALLETQTY = this.PALLETQTY;
            DataObject.PRODUCTNAME = this.PRODUCTNAME;
            DataObject.FIELD1 = this.FIELD1;
            DataObject.PPID_FLAG = this.PPID_FLAG;
            DataObject.LABEL_FLAG = this.LABEL_FLAG;
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
        public string ROUTEID
        {
            get
            {
                return (string)this["ROUTEID"];
            }
            set
            {
                this["ROUTEID"] = value;
            }
        }
        public string PACK_TYPE
        {
            get
            {
                return (string)this["PACK_TYPE"];
            }
            set
            {
                this["PACK_TYPE"] = value;
            }
        }
        public string LABEL_TYPE
        {
            get
            {
                return (string)this["LABEL_TYPE"];
            }
            set
            {
                this["LABEL_TYPE"] = value;
            }
        }
        public string CURRENTPOINT
        {
            get
            {
                return (string)this["CURRENTPOINT"];
            }
            set
            {
                this["CURRENTPOINT"] = value;
            }
        }
        public string NEXTPOINT
        {
            get
            {
                return (string)this["NEXTPOINT"];
            }
            set
            {
                this["NEXTPOINT"] = value;
            }
        }
        public string PALLETQTY
        {
            get
            {
                return (string)this["PALLETQTY"];
            }
            set
            {
                this["PALLETQTY"] = value;
            }
        }
        public string PRODUCTNAME
        {
            get
            {
                return (string)this["PRODUCTNAME"];
            }
            set
            {
                this["PRODUCTNAME"] = value;
            }
        }
        public string FIELD1
        {
            get
            {
                return (string)this["FIELD1"];
            }
            set
            {
                this["FIELD1"] = value;
            }
        }
        public string PPID_FLAG
        {
            get
            {
                return (string)this["PPID_FLAG"];
            }
            set
            {
                this["PPID_FLAG"] = value;
            }
        }
        public string LABEL_FLAG
        {
            get
            {
                return (string)this["LABEL_FLAG"];
            }
            set
            {
                this["LABEL_FLAG"] = value;
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
    public class R_STATION_AUTOSCAN
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string ROUTEID { get; set; }
        public string PACK_TYPE { get; set; }
        public string LABEL_TYPE { get; set; }
        public string CURRENTPOINT { get; set; }
        public string NEXTPOINT { get; set; }
        public string PALLETQTY { get; set; }
        public string PRODUCTNAME { get; set; }
        public string FIELD1 { get; set; }
        public string PPID_FLAG { get; set; }
        public string LABEL_FLAG { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}