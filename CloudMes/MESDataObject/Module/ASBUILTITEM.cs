using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_ASBUILTITEM : DataObjectTable
    {
        public T_ASBUILTITEM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_ASBUILTITEM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_ASBUILTITEM);
            TableName = "ASBUILTITEM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_ASBUILTITEM : DataObjectBase
    {
        public Row_ASBUILTITEM(DataObjectInfo info) : base(info)
        {

        }
        public ASBUILTITEM GetDataObject()
        {
            ASBUILTITEM DataObject = new ASBUILTITEM();
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.SALES_ORDER_LINE_NUMBER = this.SALES_ORDER_LINE_NUMBER;
            DataObject.PARENT_MODEL_NUMBER = this.PARENT_MODEL_NUMBER;
            DataObject.PARENT_SERIAL_NUMBER = this.PARENT_SERIAL_NUMBER;
            DataObject.PARENT_QTY = this.PARENT_QTY;
            DataObject.MODEL_NUMBER_REVISION = this.MODEL_NUMBER_REVISION;
            DataObject.COUNTRY_OF_ORIGIN = this.COUNTRY_OF_ORIGIN;
            DataObject.CLEI_CODE = this.CLEI_CODE;
            DataObject.MAC_ADDRESS = this.MAC_ADDRESS;
            DataObject.BUILT_SITE = this.BUILT_SITE;
            DataObject.BUILD_DATE = this.BUILD_DATE;
            DataObject.SOFTWARE_VERSION = this.SOFTWARE_VERSION;
            DataObject.SUB_ASSEMBLY_NUMBER = this.SUB_ASSEMBLY_NUMBER;
            DataObject.SUB_ASSEMBLY_REVISION = this.SUB_ASSEMBLY_REVISION;
            DataObject.IBM_SERIAL_NUMBER = this.IBM_SERIAL_NUMBER;
            DataObject.FUTURE1 = this.FUTURE1;
            DataObject.FUTURE2 = this.FUTURE2;
            DataObject.FUTURE3 = this.FUTURE3;
            DataObject.FUTURE4 = this.FUTURE4;
            DataObject.FUTURE5 = this.FUTURE5;
            DataObject.CREATEDT = this.CREATEDT;
            DataObject.LOAD_SFC_DB = this.LOAD_SFC_DB;
            return DataObject;
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
        public string MESSAGEID
        {
            get
            {
                return (string)this["MESSAGEID"];
            }
            set
            {
                this["MESSAGEID"] = value;
            }
        }
        public string SALES_ORDER_LINE_NUMBER
        {
            get
            {
                return (string)this["SALES_ORDER_LINE_NUMBER"];
            }
            set
            {
                this["SALES_ORDER_LINE_NUMBER"] = value;
            }
        }
        public string PARENT_MODEL_NUMBER
        {
            get
            {
                return (string)this["PARENT_MODEL_NUMBER"];
            }
            set
            {
                this["PARENT_MODEL_NUMBER"] = value;
            }
        }
        public string PARENT_SERIAL_NUMBER
        {
            get
            {
                return (string)this["PARENT_SERIAL_NUMBER"];
            }
            set
            {
                this["PARENT_SERIAL_NUMBER"] = value;
            }
        }
        public string PARENT_QTY
        {
            get
            {
                return (string)this["PARENT_QTY"];
            }
            set
            {
                this["PARENT_QTY"] = value;
            }
        }
        public string MODEL_NUMBER_REVISION
        {
            get
            {
                return (string)this["MODEL_NUMBER_REVISION"];
            }
            set
            {
                this["MODEL_NUMBER_REVISION"] = value;
            }
        }
        public string COUNTRY_OF_ORIGIN
        {
            get
            {
                return (string)this["COUNTRY_OF_ORIGIN"];
            }
            set
            {
                this["COUNTRY_OF_ORIGIN"] = value;
            }
        }
        public string CLEI_CODE
        {
            get
            {
                return (string)this["CLEI_CODE"];
            }
            set
            {
                this["CLEI_CODE"] = value;
            }
        }
        public string MAC_ADDRESS
        {
            get
            {
                return (string)this["MAC_ADDRESS"];
            }
            set
            {
                this["MAC_ADDRESS"] = value;
            }
        }
        public string BUILT_SITE
        {
            get
            {
                return (string)this["BUILT_SITE"];
            }
            set
            {
                this["BUILT_SITE"] = value;
            }
        }
        public string BUILD_DATE
        {
            get
            {
                return (string)this["BUILD_DATE"];
            }
            set
            {
                this["BUILD_DATE"] = value;
            }
        }
        public string SOFTWARE_VERSION
        {
            get
            {
                return (string)this["SOFTWARE_VERSION"];
            }
            set
            {
                this["SOFTWARE_VERSION"] = value;
            }
        }
        public string SUB_ASSEMBLY_NUMBER
        {
            get
            {
                return (string)this["SUB_ASSEMBLY_NUMBER"];
            }
            set
            {
                this["SUB_ASSEMBLY_NUMBER"] = value;
            }
        }
        public string SUB_ASSEMBLY_REVISION
        {
            get
            {
                return (string)this["SUB_ASSEMBLY_REVISION"];
            }
            set
            {
                this["SUB_ASSEMBLY_REVISION"] = value;
            }
        }
        public string IBM_SERIAL_NUMBER
        {
            get
            {
                return (string)this["IBM_SERIAL_NUMBER"];
            }
            set
            {
                this["IBM_SERIAL_NUMBER"] = value;
            }
        }
        public string FUTURE1
        {
            get
            {
                return (string)this["FUTURE1"];
            }
            set
            {
                this["FUTURE1"] = value;
            }
        }
        public string FUTURE2
        {
            get
            {
                return (string)this["FUTURE2"];
            }
            set
            {
                this["FUTURE2"] = value;
            }
        }
        public string FUTURE3
        {
            get
            {
                return (string)this["FUTURE3"];
            }
            set
            {
                this["FUTURE3"] = value;
            }
        }
        public string FUTURE4
        {
            get
            {
                return (string)this["FUTURE4"];
            }
            set
            {
                this["FUTURE4"] = value;
            }
        }
        public string FUTURE5
        {
            get
            {
                return (string)this["FUTURE5"];
            }
            set
            {
                this["FUTURE5"] = value;
            }
        }
        public string CREATEDT
        {
            get
            {
                return (string)this["CREATEDT"];
            }
            set
            {
                this["CREATEDT"] = value;
            }
        }
        public string LOAD_SFC_DB
        {
            get
            {
                return (string)this["LOAD_SFC_DB"];
            }
            set
            {
                this["LOAD_SFC_DB"] = value;
            }
        }
    }
    public class ASBUILTITEM
    {
        public string FILENAME;
        public string MESSAGEID;
        public string SALES_ORDER_LINE_NUMBER;
        public string PARENT_MODEL_NUMBER;
        public string PARENT_SERIAL_NUMBER;
        public string PARENT_QTY;
        public string MODEL_NUMBER_REVISION;
        public string COUNTRY_OF_ORIGIN;
        public string CLEI_CODE;
        public string MAC_ADDRESS;
        public string BUILT_SITE;
        public string BUILD_DATE;
        public string SOFTWARE_VERSION;
        public string SUB_ASSEMBLY_NUMBER;
        public string SUB_ASSEMBLY_REVISION;
        public string IBM_SERIAL_NUMBER;
        public string FUTURE1;
        public string FUTURE2;
        public string FUTURE3;
        public string FUTURE4;
        public string FUTURE5;
        public string CREATEDT;
        public string LOAD_SFC_DB;
    }
}