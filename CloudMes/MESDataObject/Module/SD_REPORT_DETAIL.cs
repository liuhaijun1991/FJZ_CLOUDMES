using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_SD_REPORT_DETAIL : DataObjectTable
    {
        public T_SD_REPORT_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_SD_REPORT_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_SD_REPORT_DETAIL);
            TableName = "SD_REPORT_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_SD_REPORT_DETAIL : DataObjectBase
    {
        public Row_SD_REPORT_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public SD_REPORT_DETAIL GetDataObject()
        {
            SD_REPORT_DETAIL DataObject = new SD_REPORT_DETAIL();
            DataObject.LOCATION = this.LOCATION;
            DataObject.STR_SUPPL3 = this.STR_SUPPL3;
            DataObject.STR_SUPPL2 = this.STR_SUPPL2;
            DataObject.STR_SUPPL1 = this.STR_SUPPL1;
            DataObject.NAME4 = this.NAME4;
            DataObject.NAME3 = this.NAME3;
            DataObject.NAME2 = this.NAME2;
            DataObject.INCO2 = this.INCO2;
            DataObject.INCO1 = this.INCO1;
            DataObject.GEWEI = this.GEWEI;
            DataObject.CITY2 = this.CITY2;
            DataObject.ORT01 = this.ORT01;
            DataObject.BEZEI = this.BEZEI;
            DataObject.NATIO = this.NATIO;
            DataObject.CITY1 = this.CITY1;
            DataObject.BUILDING = this.BUILDING;
            DataObject.STREET = this.STREET;
            DataObject.ROOMNUM = this.ROOMNUM;
            DataObject.FLOOR = this.FLOOR;
            DataObject.NAME1 = this.NAME1;
            DataObject.TPNUM = this.TPNUM;
            DataObject.TKNUM = this.TKNUM;
            DataObject.VBELN = this.VBELN;
            return DataObject;
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public string STR_SUPPL3
        {
            get
            {
                return (string)this["STR_SUPPL3"];
            }
            set
            {
                this["STR_SUPPL3"] = value;
            }
        }
        public string STR_SUPPL2
        {
            get
            {
                return (string)this["STR_SUPPL2"];
            }
            set
            {
                this["STR_SUPPL2"] = value;
            }
        }
        public string STR_SUPPL1
        {
            get
            {
                return (string)this["STR_SUPPL1"];
            }
            set
            {
                this["STR_SUPPL1"] = value;
            }
        }
        public string NAME4
        {
            get
            {
                return (string)this["NAME4"];
            }
            set
            {
                this["NAME4"] = value;
            }
        }
        public string NAME3
        {
            get
            {
                return (string)this["NAME3"];
            }
            set
            {
                this["NAME3"] = value;
            }
        }
        public string NAME2
        {
            get
            {
                return (string)this["NAME2"];
            }
            set
            {
                this["NAME2"] = value;
            }
        }
        public string INCO2
        {
            get
            {
                return (string)this["INCO2"];
            }
            set
            {
                this["INCO2"] = value;
            }
        }
        public string INCO1
        {
            get
            {
                return (string)this["INCO1"];
            }
            set
            {
                this["INCO1"] = value;
            }
        }
        public string GEWEI
        {
            get
            {
                return (string)this["GEWEI"];
            }
            set
            {
                this["GEWEI"] = value;
            }
        }
        public string CITY2
        {
            get
            {
                return (string)this["CITY2"];
            }
            set
            {
                this["CITY2"] = value;
            }
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
        public string BEZEI
        {
            get
            {
                return (string)this["BEZEI"];
            }
            set
            {
                this["BEZEI"] = value;
            }
        }
        public string NATIO
        {
            get
            {
                return (string)this["NATIO"];
            }
            set
            {
                this["NATIO"] = value;
            }
        }
        public string CITY1
        {
            get
            {
                return (string)this["CITY1"];
            }
            set
            {
                this["CITY1"] = value;
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
        public string STREET
        {
            get
            {
                return (string)this["STREET"];
            }
            set
            {
                this["STREET"] = value;
            }
        }
        public string ROOMNUM
        {
            get
            {
                return (string)this["ROOMNUM"];
            }
            set
            {
                this["ROOMNUM"] = value;
            }
        }
        public string FLOOR
        {
            get
            {
                return (string)this["FLOOR"];
            }
            set
            {
                this["FLOOR"] = value;
            }
        }
        public string NAME1
        {
            get
            {
                return (string)this["NAME1"];
            }
            set
            {
                this["NAME1"] = value;
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
    public class SD_REPORT_DETAIL
    {
        public string LOCATION{ get; set; }
        public string STR_SUPPL3{ get; set; }
        public string STR_SUPPL2{ get; set; }
        public string STR_SUPPL1{ get; set; }
        public string NAME4{ get; set; }
        public string NAME3{ get; set; }
        public string NAME2{ get; set; }
        public string INCO2{ get; set; }
        public string INCO1{ get; set; }
        public string GEWEI{ get; set; }
        public string CITY2{ get; set; }
        public string ORT01{ get; set; }
        public string BEZEI{ get; set; }
        public string NATIO{ get; set; }
        public string CITY1{ get; set; }
        public string BUILDING{ get; set; }
        public string STREET{ get; set; }
        public string ROOMNUM{ get; set; }
        public string FLOOR{ get; set; }
        public string NAME1{ get; set; }
        public string TPNUM{ get; set; }
        public string TKNUM{ get; set; }
        public string VBELN{ get; set; }
    }
}