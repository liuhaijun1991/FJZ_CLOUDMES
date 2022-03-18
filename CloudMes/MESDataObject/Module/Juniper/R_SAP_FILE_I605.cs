using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_SAP_FILE_I605 : DataObjectTable
    {
        public T_R_SAP_FILE_I605(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_FILE_I605(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_FILE_I605);
            TableName = "R_SAP_FILE_I605".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_FILE_I605 : DataObjectBase
    {
        public Row_R_SAP_FILE_I605(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_FILE_I605 GetDataObject()
        {
            R_SAP_FILE_I605 DataObject = new R_SAP_FILE_I605();
            DataObject.ID = this.ID;
            DataObject.FILE_ID = this.FILE_ID;
            DataObject.ITEM_NAME = this.ITEM_NAME;
            DataObject.ORGANIZATION_CODE = this.ORGANIZATION_CODE;
            DataObject.SR_INSTANCE_CODE = this.SR_INSTANCE_CODE;
            DataObject.NEW_ORDER_QUANTITY = this.NEW_ORDER_QUANTITY;
            DataObject.SUBINVENTORY_CODE = this.SUBINVENTORY_CODE;
            DataObject.LOT_NUMBER = this.LOT_NUMBER;
            DataObject.EXPIRATION_DATE = this.EXPIRATION_DATE;
            DataObject.DELETED_FLAG = this.DELETED_FLAG;
            DataObject.CM_PART_NUMBER = this.CM_PART_NUMBER;
            DataObject.ITEM_TYPE = this.ITEM_TYPE;
            DataObject.OWNERSHIP = this.OWNERSHIP;
            DataObject.NETTABLE_FLAG = this.NETTABLE_FLAG;
            DataObject.GROUP_CODE = this.GROUP_CODE;
            DataObject.FREE_ATTR1 = this.FREE_ATTR1;
            DataObject.FREE_ATTR2 = this.FREE_ATTR2;
            DataObject.FREE_ATTR3 = this.FREE_ATTR3;
            DataObject.FREE_ATTR4 = this.FREE_ATTR4;
            DataObject.FREE_ATTR5 = this.FREE_ATTR5;
            DataObject.FREE_ATTR6 = this.FREE_ATTR6;
            DataObject.FREE_ATTR7 = this.FREE_ATTR7;
            DataObject.FREE_ATTR8 = this.FREE_ATTR8;
            DataObject.FREE_ATTR9 = this.FREE_ATTR9;
            DataObject.FREE_ATTR10 = this.FREE_ATTR10;
            DataObject.FREE_ATTR11 = this.FREE_ATTR11;
            DataObject.FREE_ATTR12 = this.FREE_ATTR12;
            DataObject.FREE_ATTR13 = this.FREE_ATTR13;
            DataObject.FREE_ATTR14 = this.FREE_ATTR14;
            DataObject.FREE_ATTR15 = this.FREE_ATTR15;
            DataObject.FREE_ATTR16 = this.FREE_ATTR16;
            DataObject.FREE_ATTR17 = this.FREE_ATTR17;
            DataObject.FREE_ATTR18 = this.FREE_ATTR18;
            DataObject.FREE_ATTR19 = this.FREE_ATTR19;
            DataObject.FREE_ATTR20 = this.FREE_ATTR20;
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
        public string FILE_ID
        {
            get
            {
                return (string)this["FILE_ID"];
            }
            set
            {
                this["FILE_ID"] = value;
            }
        }
        public string ITEM_NAME
        {
            get
            {
                return (string)this["ITEM_NAME"];
            }
            set
            {
                this["ITEM_NAME"] = value;
            }
        }
        public string ORGANIZATION_CODE
        {
            get
            {
                return (string)this["ORGANIZATION_CODE"];
            }
            set
            {
                this["ORGANIZATION_CODE"] = value;
            }
        }
        public string SR_INSTANCE_CODE
        {
            get
            {
                return (string)this["SR_INSTANCE_CODE"];
            }
            set
            {
                this["SR_INSTANCE_CODE"] = value;
            }
        }
        public double? NEW_ORDER_QUANTITY
        {
            get
            {
                return (double?)this["NEW_ORDER_QUANTITY"];
            }
            set
            {
                this["NEW_ORDER_QUANTITY"] = value;
            }
        }
        public string SUBINVENTORY_CODE
        {
            get
            {
                return (string)this["SUBINVENTORY_CODE"];
            }
            set
            {
                this["SUBINVENTORY_CODE"] = value;
            }
        }
        public string LOT_NUMBER
        {
            get
            {
                return (string)this["LOT_NUMBER"];
            }
            set
            {
                this["LOT_NUMBER"] = value;
            }
        }
        public DateTime? EXPIRATION_DATE
        {
            get
            {
                return (DateTime?)this["EXPIRATION_DATE"];
            }
            set
            {
                this["EXPIRATION_DATE"] = value;
            }
        }
        public string DELETED_FLAG
        {
            get
            {
                return (string)this["DELETED_FLAG"];
            }
            set
            {
                this["DELETED_FLAG"] = value;
            }
        }
        public string CM_PART_NUMBER
        {
            get
            {
                return (string)this["CM_PART_NUMBER"];
            }
            set
            {
                this["CM_PART_NUMBER"] = value;
            }
        }
        public string ITEM_TYPE
        {
            get
            {
                return (string)this["ITEM_TYPE"];
            }
            set
            {
                this["ITEM_TYPE"] = value;
            }
        }
        public string OWNERSHIP
        {
            get
            {
                return (string)this["OWNERSHIP"];
            }
            set
            {
                this["OWNERSHIP"] = value;
            }
        }
        public string NETTABLE_FLAG
        {
            get
            {
                return (string)this["NETTABLE_FLAG"];
            }
            set
            {
                this["NETTABLE_FLAG"] = value;
            }
        }
        public string GROUP_CODE
        {
            get
            {
                return (string)this["GROUP_CODE"];
            }
            set
            {
                this["GROUP_CODE"] = value;
            }
        }
        public string FREE_ATTR1
        {
            get
            {
                return (string)this["FREE_ATTR1"];
            }
            set
            {
                this["FREE_ATTR1"] = value;
            }
        }
        public string FREE_ATTR2
        {
            get
            {
                return (string)this["FREE_ATTR2"];
            }
            set
            {
                this["FREE_ATTR2"] = value;
            }
        }
        public string FREE_ATTR3
        {
            get
            {
                return (string)this["FREE_ATTR3"];
            }
            set
            {
                this["FREE_ATTR3"] = value;
            }
        }
        public string FREE_ATTR4
        {
            get
            {
                return (string)this["FREE_ATTR4"];
            }
            set
            {
                this["FREE_ATTR4"] = value;
            }
        }
        public string FREE_ATTR5
        {
            get
            {
                return (string)this["FREE_ATTR5"];
            }
            set
            {
                this["FREE_ATTR5"] = value;
            }
        }
        public string FREE_ATTR6
        {
            get
            {
                return (string)this["FREE_ATTR6"];
            }
            set
            {
                this["FREE_ATTR6"] = value;
            }
        }
        public string FREE_ATTR7
        {
            get
            {
                return (string)this["FREE_ATTR7"];
            }
            set
            {
                this["FREE_ATTR7"] = value;
            }
        }
        public string FREE_ATTR8
        {
            get
            {
                return (string)this["FREE_ATTR8"];
            }
            set
            {
                this["FREE_ATTR8"] = value;
            }
        }
        public string FREE_ATTR9
        {
            get
            {
                return (string)this["FREE_ATTR9"];
            }
            set
            {
                this["FREE_ATTR9"] = value;
            }
        }
        public string FREE_ATTR10
        {
            get
            {
                return (string)this["FREE_ATTR10"];
            }
            set
            {
                this["FREE_ATTR10"] = value;
            }
        }
        public string FREE_ATTR11
        {
            get
            {
                return (string)this["FREE_ATTR11"];
            }
            set
            {
                this["FREE_ATTR11"] = value;
            }
        }
        public string FREE_ATTR12
        {
            get
            {
                return (string)this["FREE_ATTR12"];
            }
            set
            {
                this["FREE_ATTR12"] = value;
            }
        }
        public string FREE_ATTR13
        {
            get
            {
                return (string)this["FREE_ATTR13"];
            }
            set
            {
                this["FREE_ATTR13"] = value;
            }
        }
        public string FREE_ATTR14
        {
            get
            {
                return (string)this["FREE_ATTR14"];
            }
            set
            {
                this["FREE_ATTR14"] = value;
            }
        }
        public string FREE_ATTR15
        {
            get
            {
                return (string)this["FREE_ATTR15"];
            }
            set
            {
                this["FREE_ATTR15"] = value;
            }
        }
        public string FREE_ATTR16
        {
            get
            {
                return (string)this["FREE_ATTR16"];
            }
            set
            {
                this["FREE_ATTR16"] = value;
            }
        }
        public string FREE_ATTR17
        {
            get
            {
                return (string)this["FREE_ATTR17"];
            }
            set
            {
                this["FREE_ATTR17"] = value;
            }
        }
        public string FREE_ATTR18
        {
            get
            {
                return (string)this["FREE_ATTR18"];
            }
            set
            {
                this["FREE_ATTR18"] = value;
            }
        }
        public string FREE_ATTR19
        {
            get
            {
                return (string)this["FREE_ATTR19"];
            }
            set
            {
                this["FREE_ATTR19"] = value;
            }
        }
        public string FREE_ATTR20
        {
            get
            {
                return (string)this["FREE_ATTR20"];
            }
            set
            {
                this["FREE_ATTR20"] = value;
            }
        }
    }
    public class R_SAP_FILE_I605
    {
        public string ID { get; set; }
        public string FILE_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string ORGANIZATION_CODE { get; set; }
        public string SR_INSTANCE_CODE { get; set; }
        public double? NEW_ORDER_QUANTITY { get; set; }
        public string SUBINVENTORY_CODE { get; set; }
        public string LOT_NUMBER { get; set; }
        public DateTime? EXPIRATION_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CM_PART_NUMBER { get; set; }
        public string ITEM_TYPE { get; set; }
        public string OWNERSHIP { get; set; }
        public string NETTABLE_FLAG { get; set; }
        public string GROUP_CODE { get; set; }
        public string FREE_ATTR1 { get; set; }
        public string FREE_ATTR2 { get; set; }
        public string FREE_ATTR3 { get; set; }
        public string FREE_ATTR4 { get; set; }
        public string FREE_ATTR5 { get; set; }
        public string FREE_ATTR6 { get; set; }
        public string FREE_ATTR7 { get; set; }
        public string FREE_ATTR8 { get; set; }
        public string FREE_ATTR9 { get; set; }
        public string FREE_ATTR10 { get; set; }
        public string FREE_ATTR11 { get; set; }
        public string FREE_ATTR12 { get; set; }
        public string FREE_ATTR13 { get; set; }
        public string FREE_ATTR14 { get; set; }
        public string FREE_ATTR15 { get; set; }
        public string FREE_ATTR16 { get; set; }
        public string FREE_ATTR17 { get; set; }
        public string FREE_ATTR18 { get; set; }
        public string FREE_ATTR19 { get; set; }
        public string FREE_ATTR20 { get; set; }
    }
}