using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class T_C_ORA_L6_MAPPING : DataObjectTable
    {
        public T_C_ORA_L6_MAPPING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ORA_L6_MAPPING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ORA_L6_MAPPING);
            TableName = "C_ORA_L6_MAPPING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<C_ORA_L6_MAPPING> GetL6MappingData(string StrPType,string station, OleExec SFCDB)
        {
            List<C_ORA_L6_MAPPING> C_ORA_L6_MAPPING = new List<C_ORA_L6_MAPPING>();
            C_ORA_L6_MAPPING = SFCDB.ORM.Queryable<C_ORA_L6_MAPPING>().Where(t => t.PRODUCT_TYPE == StrPType && t.PN_LOCATION == "FNN" && t.VERIFICATION_STATION == station).ToList();
            return C_ORA_L6_MAPPING;
        }

    }
    public class Row_C_ORA_L6_MAPPING : DataObjectBase
    {
        public Row_C_ORA_L6_MAPPING(DataObjectInfo info) : base(info)
        {

        }
        public C_ORA_L6_MAPPING GetDataObject()
        {
            C_ORA_L6_MAPPING DataObject = new C_ORA_L6_MAPPING();
            DataObject.MAP_ID = this.MAP_ID;
            DataObject.PRODUCT_TYPE = this.PRODUCT_TYPE;
            DataObject.COMPONENT_PN = this.COMPONENT_PN;
            DataObject.QTY = this.QTY;
            DataObject.PN_LOCATION = this.PN_LOCATION;
            DataObject.PN_LEVEL = this.PN_LEVEL;
            DataObject.LEVEL_1_PN = this.LEVEL_1_PN;
            DataObject.LEVEL_2_PN = this.LEVEL_2_PN;
            DataObject.LEVEL_3_PN = this.LEVEL_3_PN;
            DataObject.LEVEL_4_PN = this.LEVEL_4_PN;
            DataObject.LEVEL_5_PN = this.LEVEL_5_PN;
            DataObject.VERIFICATION_STATION = this.VERIFICATION_STATION;
            DataObject.SN_BOM_FILE = this.SN_BOM_FILE;
            DataObject.PN_CONTROL_TYPE = this.PN_CONTROL_TYPE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string MAP_ID
        {
            get
            {
                return (string)this["MAP_ID"];
            }
            set
            {
                this["MAP_ID"] = value;
            }
        }
        public string PRODUCT_TYPE
        {
            get
            {
                return (string)this["PRODUCT_TYPE"];
            }
            set
            {
                this["PRODUCT_TYPE"] = value;
            }
        }
     
        public string COMPONENT_PN
        {
            get
            {
                return (string)this["COMPONENT_PN"];
            }
            set
            {
                this["COMPONENT_PN"] = value;
            }
        }
        public int QTY
        {
            get
            {
                return (int)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string PN_LOCATION
        {
            get
            {
                return (string)this["PN_LOCATION"];
            }
            set
            {
                this["PN_LOCATION"] = value;
            }
        }
        public int PN_LEVEL
        {
            get
            {
                return (int)this["PN_LEVEL"];
            }
            set
            {
                this["PN_LEVEL"] = value;
            }
        }
        public string LEVEL_1_PN
        {
            get
            {
                return (string)this["LEVEL_1_PN"];
            }
            set
            {
                this["LEVEL_1_PN"] = value;
            }
        }
        public string LEVEL_2_PN
        {
            get
            {
                return (string)this["LEVEL_2_PN"];
            }
            set
            {
                this["LEVEL_2_PN"] = value;
            }
        }
        public string LEVEL_3_PN
        {
            get
            {
                return (string)this["LEVEL_3_PN"];
            }
            set
            {
                this["LEVEL_3_PN"] = value;
            }
        }
        public string LEVEL_4_PN
        {
            get
            {
                return (string)this["LEVEL_4_PN"];
            }
            set
            {
                this["LEVEL_4_PN"] = value;
            }
        }
        public string LEVEL_5_PN
        {
            get
            {
                return (string)this["LEVEL_5_PN"];
            }
            set
            {
                this["LEVEL_5_PN"] = value;
            }
        }
        public string VERIFICATION_STATION
        {
            get
            {
                return (string)this["VERIFICATION_STATION"];
            }
            set
            {
                this["VERIFICATION_STATION"] = value;
            }
        }
        public string SN_BOM_FILE
        {
            get
            {
                return (string)this["SN_BOM_FILE"];
            }
            set
            {
                this["SN_BOM_FILE"] = value;
            }
        }
        public string PN_CONTROL_TYPE
        {
            get
            {
                return (string)this["PN_CONTROL_TYPE"];
            }
            set
            {
                this["PN_CONTROL_TYPE"] = value;
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
    public class C_ORA_L6_MAPPING
    {
        public string MAP_ID { get; set; }
        public string PRODUCT_TYPE { get; set; }        
        public string COMPONENT_PN { get; set; }
        public int QTY { get; set; }
        public string PN_LOCATION { get; set; }
        public int PN_LEVEL { get; set; }
        public string LEVEL_1_PN { get; set; }
        public string LEVEL_2_PN { get; set; }
        public string LEVEL_3_PN { get; set; }
        public string LEVEL_4_PN { get; set; }
        public string LEVEL_5_PN { get; set; }
        public string VERIFICATION_STATION { get; set; }
        public string SN_BOM_FILE { get; set; }
        public string PN_CONTROL_TYPE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}
