using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_SD_TO_DETAIL : DataObjectTable
    {
        public T_SD_TO_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_SD_TO_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_SD_TO_DETAIL);
            TableName = "SD_TO_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_SD_TO_DETAIL : DataObjectBase
    {
        public Row_SD_TO_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public SD_TO_DETAIL GetDataObject()
        {
            SD_TO_DETAIL DataObject = new SD_TO_DETAIL();
            DataObject.ORT01 = this.ORT01;
            DataObject.LAND1 = this.LAND1;
            DataObject.LFART = this.LFART;
            DataObject.KUNNR = this.KUNNR;
            DataObject.TPNUM = this.TPNUM;
            DataObject.TKNUM = this.TKNUM;
            DataObject.VBELN = this.VBELN;
            return DataObject;
        }
        public string ORT01
        {
            get
            {
                return (string)this["ORT01"];
            }
            set
            {
                this["ORT01"] = value;
            }
        }
        public string LAND1
        {
            get
            {
                return (string)this["LAND1"];
            }
            set
            {
                this["LAND1"] = value;
            }
        }
        public string LFART
        {
            get
            {
                return (string)this["LFART"];
            }
            set
            {
                this["LFART"] = value;
            }
        }
        public string KUNNR
        {
            get
            {
                return (string)this["KUNNR"];
            }
            set
            {
                this["KUNNR"] = value;
            }
        }
        public string TPNUM
        {
            get
            {
                return (string)this["TPNUM"];
            }
            set
            {
                this["TPNUM"] = value;
            }
        }
        public string TKNUM
        {
            get
            {
                return (string)this["TKNUM"];
            }
            set
            {
                this["TKNUM"] = value;
            }
        }
        public string VBELN
        {
            get
            {
                return (string)this["VBELN"];
            }
            set
            {
                this["VBELN"] = value;
            }
        }
    }
    public class SD_TO_DETAIL
    {
        public string ORT01{ get; set; }
        public string LAND1{ get; set; }
        public string LFART{ get; set; }
        public string KUNNR{ get; set; }
        public string TPNUM{ get; set; }
        public string TKNUM{ get; set; }
        public string VBELN{ get; set; }
    }
}