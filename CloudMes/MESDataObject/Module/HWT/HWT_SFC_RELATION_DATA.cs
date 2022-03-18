using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_HWT_SFC_RELATION_DATA : DataObjectTable
    {
        public T_HWT_SFC_RELATION_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_HWT_SFC_RELATION_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_HWT_SFC_RELATION_DATA);
            TableName = "HWT_SFC_RELATION_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int SaveRelationData(OleExec sfcdb, HWT_SFC_RELATION_DATA relationData)
        {
            return sfcdb.ORM.Insertable<HWT_SFC_RELATION_DATA>(relationData).ExecuteCommand();
        }
    }
    public class Row_HWT_SFC_RELATION_DATA : DataObjectBase
    {
        public Row_HWT_SFC_RELATION_DATA(DataObjectInfo info) : base(info)
        {

        }
        public HWT_SFC_RELATION_DATA GetDataObject()
        {
            HWT_SFC_RELATION_DATA DataObject = new HWT_SFC_RELATION_DATA();
            DataObject.ID = this.ID;
            DataObject.CARTON_LINE_NO = this.CARTON_LINE_NO;
            DataObject.ASN = this.ASN;
            DataObject.SHIP_TO = this.SHIP_TO;
            DataObject.SHIPPING_DATE = this.SHIPPING_DATE;
            DataObject.ARRIVE_TIME = this.ARRIVE_TIME;
            DataObject.TO_NO = this.TO_NO;
            DataObject.PO = this.PO;
            DataObject.PO_ITEM = this.PO_ITEM;
            DataObject.HWPN = this.HWPN;
            DataObject.HHPN = this.HHPN;
            DataObject.DN = this.DN;
            DataObject.SHIPPED_QTY = this.SHIPPED_QTY;
            DataObject.UOM = this.UOM;
            DataObject.CARTON_ID = this.CARTON_ID;
            DataObject.TOTALBOXQTY = this.TOTALBOXQTY;
            DataObject.CARTON_QTY = this.CARTON_QTY;
            DataObject.LOTNO = this.LOTNO;
            DataObject.ROHS_STATUS = this.ROHS_STATUS;
            DataObject.SHIPPING_TIME = this.SHIPPING_TIME;
            DataObject.CANBESENT = this.CANBESENT;
            DataObject.FLAG_1 = this.FLAG_1;
            DataObject.FLAG_2 = this.FLAG_2;
            DataObject.FLAG_3 = this.FLAG_3;
            DataObject.EDI_SENDFLAG = this.EDI_SENDFLAG;
            DataObject.SHIPTOCODE = this.SHIPTOCODE;
            DataObject.F_LOCATION = this.F_LOCATION;
            DataObject.CARTONCBSTIME = this.CARTONCBSTIME;
            DataObject.TOSTARTTIME = this.TOSTARTTIME;
            DataObject.SN = this.SN;
            DataObject.HWREPAIRPN = this.HWREPAIRPN;
            DataObject.ORIGIN_COUNTRY = this.ORIGIN_COUNTRY;
            DataObject.LEGAL_INSPECTION = this.LEGAL_INSPECTION;
            DataObject.CUSTOMER_KP_NO_VER = this.CUSTOMER_KP_NO_VER;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string CARTON_LINE_NO
        {
            get
            {
                return (string)this["CARTON_LINE_NO"];
            }
            set
            {
                this["CARTON_LINE_NO"] = value;
            }
        }
        public string ASN
        {
            get
            {
                return (string)this["ASN"];
            }
            set
            {
                this["ASN"] = value;
            }
        }
        public string SHIP_TO
        {
            get
            {
                return (string)this["SHIP_TO"];
            }
            set
            {
                this["SHIP_TO"] = value;
            }
        }
        public DateTime? SHIPPING_DATE
        {
            get
            {
                return (DateTime?)this["SHIPPING_DATE"];
            }
            set
            {
                this["SHIPPING_DATE"] = value;
            }
        }
        public DateTime? ARRIVE_TIME
        {
            get
            {
                return (DateTime?)this["ARRIVE_TIME"];
            }
            set
            {
                this["ARRIVE_TIME"] = value;
            }
        }
        public string TO_NO
        {
            get
            {
                return (string)this["TO_NO"];
            }
            set
            {
                this["TO_NO"] = value;
            }
        }
        public string PO
        {
            get
            {
                return (string)this["PO"];
            }
            set
            {
                this["PO"] = value;
            }
        }
        public string PO_ITEM
        {
            get
            {
                return (string)this["PO_ITEM"];
            }
            set
            {
                this["PO_ITEM"] = value;
            }
        }
        public string HWPN
        {
            get
            {
                return (string)this["HWPN"];
            }
            set
            {
                this["HWPN"] = value;
            }
        }
        public string HHPN
        {
            get
            {
                return (string)this["HHPN"];
            }
            set
            {
                this["HHPN"] = value;
            }
        }
        public string DN
        {
            get
            {
                return (string)this["DN"];
            }
            set
            {
                this["DN"] = value;
            }
        }
        public double? SHIPPED_QTY
        {
            get
            {
                return (double?)this["SHIPPED_QTY"];
            }
            set
            {
                this["SHIPPED_QTY"] = value;
            }
        }
        public string UOM
        {
            get
            {
                return (string)this["UOM"];
            }
            set
            {
                this["UOM"] = value;
            }
        }
        public string CARTON_ID
        {
            get
            {
                return (string)this["CARTON_ID"];
            }
            set
            {
                this["CARTON_ID"] = value;
            }
        }
        public double? TOTALBOXQTY
        {
            get
            {
                return (double?)this["TOTALBOXQTY"];
            }
            set
            {
                this["TOTALBOXQTY"] = value;
            }
        }
        public double? CARTON_QTY
        {
            get
            {
                return (double?)this["CARTON_QTY"];
            }
            set
            {
                this["CARTON_QTY"] = value;
            }
        }
        public string LOTNO
        {
            get
            {
                return (string)this["LOTNO"];
            }
            set
            {
                this["LOTNO"] = value;
            }
        }
        public string ROHS_STATUS
        {
            get
            {
                return (string)this["ROHS_STATUS"];
            }
            set
            {
                this["ROHS_STATUS"] = value;
            }
        }
        public DateTime? SHIPPING_TIME
        {
            get
            {
                return (DateTime?)this["SHIPPING_TIME"];
            }
            set
            {
                this["SHIPPING_TIME"] = value;
            }
        }
        public string CANBESENT
        {
            get
            {
                return (string)this["CANBESENT"];
            }
            set
            {
                this["CANBESENT"] = value;
            }
        }
        public string FLAG_1
        {
            get
            {
                return (string)this["FLAG_1"];
            }
            set
            {
                this["FLAG_1"] = value;
            }
        }
        public string FLAG_2
        {
            get
            {
                return (string)this["FLAG_2"];
            }
            set
            {
                this["FLAG_2"] = value;
            }
        }
        public string FLAG_3
        {
            get
            {
                return (string)this["FLAG_3"];
            }
            set
            {
                this["FLAG_3"] = value;
            }
        }
        public string EDI_SENDFLAG
        {
            get
            {
                return (string)this["EDI_SENDFLAG"];
            }
            set
            {
                this["EDI_SENDFLAG"] = value;
            }
        }
        public string SHIPTOCODE
        {
            get
            {
                return (string)this["SHIPTOCODE"];
            }
            set
            {
                this["SHIPTOCODE"] = value;
            }
        }
        public string F_LOCATION
        {
            get
            {
                return (string)this["F_LOCATION"];
            }
            set
            {
                this["F_LOCATION"] = value;
            }
        }
        public DateTime? CARTONCBSTIME
        {
            get
            {
                return (DateTime?)this["CARTONCBSTIME"];
            }
            set
            {
                this["CARTONCBSTIME"] = value;
            }
        }
        public DateTime? TOSTARTTIME
        {
            get
            {
                return (DateTime?)this["TOSTARTTIME"];
            }
            set
            {
                this["TOSTARTTIME"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string HWREPAIRPN
        {
            get
            {
                return (string)this["HWREPAIRPN"];
            }
            set
            {
                this["HWREPAIRPN"] = value;
            }
        }
        public string ORIGIN_COUNTRY
        {
            get
            {
                return (string)this["ORIGIN_COUNTRY"];
            }
            set
            {
                this["ORIGIN_COUNTRY"] = value;
            }
        }
        public string LEGAL_INSPECTION
        {
            get
            {
                return (string)this["LEGAL_INSPECTION"];
            }
            set
            {
                this["LEGAL_INSPECTION"] = value;
            }
        }
        public string CUSTOMER_KP_NO_VER
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO_VER"];
            }
            set
            {
                this["CUSTOMER_KP_NO_VER"] = value;
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
    public class HWT_SFC_RELATION_DATA
    {
        public string ID { get; set; }
        public string CARTON_LINE_NO { get; set; }
        public string ASN { get; set; }
        public string SHIP_TO { get; set; }
        public DateTime? SHIPPING_DATE { get; set; }
        public DateTime? ARRIVE_TIME { get; set; }
        public string TO_NO { get; set; }
        public string PO { get; set; }
        public string PO_ITEM { get; set; }
        public string HWPN { get; set; }
        public string HHPN { get; set; }
        public string DN { get; set; }
        public double? SHIPPED_QTY { get; set; }
        public string UOM { get; set; }
        public string CARTON_ID { get; set; }
        public double? TOTALBOXQTY { get; set; }
        public double? CARTON_QTY { get; set; }
        public string LOTNO { get; set; }
        public string ROHS_STATUS { get; set; }
        public DateTime? SHIPPING_TIME { get; set; }
        public string CANBESENT { get; set; }
        public string FLAG_1 { get; set; }
        public string FLAG_2 { get; set; }
        public string FLAG_3 { get; set; }
        public string EDI_SENDFLAG { get; set; }
        public string SHIPTOCODE { get; set; }
        public string F_LOCATION { get; set; }
        public DateTime? CARTONCBSTIME { get; set; }
        public DateTime? TOSTARTTIME { get; set; }
        public string SN { get; set; }
        public string HWREPAIRPN { get; set; }
        public string ORIGIN_COUNTRY { get; set; }
        public string LEGAL_INSPECTION { get; set; }
        public string CUSTOMER_KP_NO_VER { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}