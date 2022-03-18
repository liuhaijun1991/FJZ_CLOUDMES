using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_NETGEAR_PTM_DATA : DataObjectTable
    {
        public T_R_NETGEAR_PTM_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_NETGEAR_PTM_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_NETGEAR_PTM_DATA);
            TableName = "R_NETGEAR_PTM_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_NETGEAR_PTM_DATA : DataObjectBase
    {
        public Row_R_NETGEAR_PTM_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_NETGEAR_PTM_DATA GetDataObject()
        {
            R_NETGEAR_PTM_DATA DataObject = new R_NETGEAR_PTM_DATA();
            DataObject.PA_SN = this.PA_SN;
            DataObject.PA_ITEM_NUMBER = this.PA_ITEM_NUMBER;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.ID = this.ID;
            DataObject.SHIPORDERID = this.SHIPORDERID;
            DataObject.PACKAGENO = this.PACKAGENO;
            DataObject.ORDERLINENO = this.ORDERLINENO;
            DataObject.SEQNO = this.SEQNO;
            DataObject.PALLET_ID = this.PALLET_ID;
            DataObject.MASTER_CARTON_ID = this.MASTER_CARTON_ID;
            DataObject.ITEM_NUMBER = this.ITEM_NUMBER;
            DataObject.SERIAL_NUMBER = this.SERIAL_NUMBER;
            DataObject.TOP_SERIAL_NUMBER = this.TOP_SERIAL_NUMBER;
            DataObject.MAC_ID = this.MAC_ID;
            DataObject.ASN_NUMBER = this.ASN_NUMBER;
            DataObject.INVOICE_NUMBER = this.INVOICE_NUMBER;
            DataObject.PACKING_SLIP_NUMBER = this.PACKING_SLIP_NUMBER;
            DataObject.CONTAINER_NUMBER = this.CONTAINER_NUMBER;
            DataObject.PO_NUMBER = this.PO_NUMBER;
            DataObject.PO_LINE_NUMBER = this.PO_LINE_NUMBER;
            DataObject.XFACTORY_DATE = this.XFACTORY_DATE;
            DataObject.MANUFACTURER_NAME = this.MANUFACTURER_NAME;
            DataObject.AS_DATE_OF_MANUFACTURE = this.AS_DATE_OF_MANUFACTURE;
            DataObject.COUNTRY_OF_ORIGIN = this.COUNTRY_OF_ORIGIN;
            DataObject.ORG_CODE = this.ORG_CODE;
            DataObject.IMEI_NUMBER = this.IMEI_NUMBER;
            DataObject.MASTERLOCK_NUMBER = this.MASTERLOCK_NUMBER;
            DataObject.NETWORKLOCK_NUMBER = this.NETWORKLOCK_NUMBER;
            DataObject.SERVICELOCK_NUMBER = this.SERVICELOCK_NUMBER;
            DataObject.FA_NUMBER_LEVEL_REV = this.FA_NUMBER_LEVEL_REV;
            DataObject.FA_NUMBER = this.FA_NUMBER;
            DataObject.ITEM_NUMBER_LEVEL_REV = this.ITEM_NUMBER_LEVEL_REV;
            DataObject.WEP_KEY = this.WEP_KEY;
            DataObject.WIFI_ID = this.WIFI_ID;
            DataObject.ACCESS_CODE = this.ACCESS_CODE;
            DataObject.PRIMARY_SSID = this.PRIMARY_SSID;
            DataObject.WPA_KEY = this.WPA_KEY;
            DataObject.MAC_ID_CABLE = this.MAC_ID_CABLE;
            DataObject.MAC_ID_EMTA = this.MAC_ID_EMTA;
            DataObject.HARDWARE_VERSION = this.HARDWARE_VERSION;
            DataObject.FIRMWARE_VERSION = this.FIRMWARE_VERSION;
            DataObject.EAN_CODE = this.EAN_CODE;
            DataObject.SOFTWARE_VERSION = this.SOFTWARE_VERSION;
            DataObject.SRM_PASSWORD = this.SRM_PASSWORD;
            DataObject.RF_MAC_ID = this.RF_MAC_ID;
            DataObject.MACID_IN_MTA = this.MACID_IN_MTA;
            DataObject.MTA_MAN_ROUTER_MAC = this.MTA_MAN_ROUTER_MAC;
            DataObject.MTADATA_MAC = this.MTADATA_MAC;
            DataObject.ETHERNET_MAC = this.ETHERNET_MAC;
            DataObject.USB_MAC = this.USB_MAC;
            DataObject.PRIMARYSSID_PASSPHRASE = this.PRIMARYSSID_PASSPHRASE;
            DataObject.CMCI_MAC = this.CMCI_MAC;
            DataObject.LAN_MAC = this.LAN_MAC;
            DataObject.WAN_MAC = this.WAN_MAC;
            DataObject.DEVICE_MAC = this.DEVICE_MAC;
            DataObject.WIRELESS_MAC = this.WIRELESS_MAC;
            DataObject.WIFI_MAC_SSID1 = this.WIFI_MAC_SSID1;
            DataObject.SSID1 = this.SSID1;
            DataObject.SSID1_PASSPHRASE = this.SSID1_PASSPHRASE;
            DataObject.WPA_PASSPHRASE = this.WPA_PASSPHRASE;
            DataObject.WPS_PIN_CODE = this.WPS_PIN_CODE;
            DataObject.PPPOA_USERNAME = this.PPPOA_USERNAME;
            DataObject.PPPOA_PASSPHRASE = this.PPPOA_PASSPHRASE;
            DataObject.TR069_UNIQUE_KEY_64_BIT = this.TR069_UNIQUE_KEY_64_BIT;
            DataObject.FON_KEY = this.FON_KEY;
            DataObject.TA=this.TA;
            return DataObject;
        }
        public string PA_SN
        {
            get
            {
                return (string)this["PA_SN"];
            }
            set
            {
                this["PA_SN"] = value;
            }
        }
        public string PA_ITEM_NUMBER
        {
            get
            {
                return (string)this["PA_ITEM_NUMBER"];
            }
            set
            {
                this["PA_ITEM_NUMBER"] = value;
            }
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
            }
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
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
        public string SHIPORDERID
        {
            get
            {
                return (string)this["SHIPORDERID"];
            }
            set
            {
                this["SHIPORDERID"] = value;
            }
        }
        public double? PACKAGENO
        {
            get
            {
                return (double?)this["PACKAGENO"];
            }
            set
            {
                this["PACKAGENO"] = value;
            }
        }
        public string ORDERLINENO
        {
            get
            {
                return (string)this["ORDERLINENO"];
            }
            set
            {
                this["ORDERLINENO"] = value;
            }
        }
        public double? SEQNO
        {
            get
            {
                return (double?)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public string PALLET_ID
        {
            get
            {
                return (string)this["PALLET_ID"];
            }
            set
            {
                this["PALLET_ID"] = value;
            }
        }
        public string MASTER_CARTON_ID
        {
            get
            {
                return (string)this["MASTER_CARTON_ID"];
            }
            set
            {
                this["MASTER_CARTON_ID"] = value;
            }
        }
        public string ITEM_NUMBER
        {
            get
            {
                return (string)this["ITEM_NUMBER"];
            }
            set
            {
                this["ITEM_NUMBER"] = value;
            }
        }
        public string SERIAL_NUMBER
        {
            get
            {
                return (string)this["SERIAL_NUMBER"];
            }
            set
            {
                this["SERIAL_NUMBER"] = value;
            }
        }
        public string TOP_SERIAL_NUMBER
        {
            get
            {
                return (string)this["TOP_SERIAL_NUMBER"];
            }
            set
            {
                this["TOP_SERIAL_NUMBER"] = value;
            }
        }
        public string MAC_ID
        {
            get
            {
                return (string)this["MAC_ID"];
            }
            set
            {
                this["MAC_ID"] = value;
            }
        }
        public string ASN_NUMBER
        {
            get
            {
                return (string)this["ASN_NUMBER"];
            }
            set
            {
                this["ASN_NUMBER"] = value;
            }
        }
        public string INVOICE_NUMBER
        {
            get
            {
                return (string)this["INVOICE_NUMBER"];
            }
            set
            {
                this["INVOICE_NUMBER"] = value;
            }
        }
        public string PACKING_SLIP_NUMBER
        {
            get
            {
                return (string)this["PACKING_SLIP_NUMBER"];
            }
            set
            {
                this["PACKING_SLIP_NUMBER"] = value;
            }
        }
        public string CONTAINER_NUMBER
        {
            get
            {
                return (string)this["CONTAINER_NUMBER"];
            }
            set
            {
                this["CONTAINER_NUMBER"] = value;
            }
        }
        public string PO_NUMBER
        {
            get
            {
                return (string)this["PO_NUMBER"];
            }
            set
            {
                this["PO_NUMBER"] = value;
            }
        }
        public string PO_LINE_NUMBER
        {
            get
            {
                return (string)this["PO_LINE_NUMBER"];
            }
            set
            {
                this["PO_LINE_NUMBER"] = value;
            }
        }
        public string XFACTORY_DATE
        {
            get
            {
                return (string)this["XFACTORY_DATE"];
            }
            set
            {
                this["XFACTORY_DATE"] = value;
            }
        }
        public string MANUFACTURER_NAME
        {
            get
            {
                return (string)this["MANUFACTURER_NAME"];
            }
            set
            {
                this["MANUFACTURER_NAME"] = value;
            }
        }
        public string AS_DATE_OF_MANUFACTURE
        {
            get
            {
                return (string)this["AS_DATE_OF_MANUFACTURE"];
            }
            set
            {
                this["AS_DATE_OF_MANUFACTURE"] = value;
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
        public string ORG_CODE
        {
            get
            {
                return (string)this["ORG_CODE"];
            }
            set
            {
                this["ORG_CODE"] = value;
            }
        }
        public string IMEI_NUMBER
        {
            get
            {
                return (string)this["IMEI_NUMBER"];
            }
            set
            {
                this["IMEI_NUMBER"] = value;
            }
        }
        public string MASTERLOCK_NUMBER
        {
            get
            {
                return (string)this["MASTERLOCK_NUMBER"];
            }
            set
            {
                this["MASTERLOCK_NUMBER"] = value;
            }
        }
        public string NETWORKLOCK_NUMBER
        {
            get
            {
                return (string)this["NETWORKLOCK_NUMBER"];
            }
            set
            {
                this["NETWORKLOCK_NUMBER"] = value;
            }
        }
        public string SERVICELOCK_NUMBER
        {
            get
            {
                return (string)this["SERVICELOCK_NUMBER"];
            }
            set
            {
                this["SERVICELOCK_NUMBER"] = value;
            }
        }
        public string FA_NUMBER_LEVEL_REV
        {
            get
            {
                return (string)this["FA_NUMBER_LEVEL_REV"];
            }
            set
            {
                this["FA_NUMBER_LEVEL_REV"] = value;
            }
        }
        public string FA_NUMBER
        {
            get
            {
                return (string)this["FA_NUMBER"];
            }
            set
            {
                this["FA_NUMBER"] = value;
            }
        }
        public string ITEM_NUMBER_LEVEL_REV
        {
            get
            {
                return (string)this["ITEM_NUMBER_LEVEL_REV"];
            }
            set
            {
                this["ITEM_NUMBER_LEVEL_REV"] = value;
            }
        }
        public string WEP_KEY
        {
            get
            {
                return (string)this["WEP_KEY"];
            }
            set
            {
                this["WEP_KEY"] = value;
            }
        }
        public string WIFI_ID
        {
            get
            {
                return (string)this["WIFI_ID"];
            }
            set
            {
                this["WIFI_ID"] = value;
            }
        }
        public string ACCESS_CODE
        {
            get
            {
                return (string)this["ACCESS_CODE"];
            }
            set
            {
                this["ACCESS_CODE"] = value;
            }
        }
        public string PRIMARY_SSID
        {
            get
            {
                return (string)this["PRIMARY_SSID"];
            }
            set
            {
                this["PRIMARY_SSID"] = value;
            }
        }
        public string WPA_KEY
        {
            get
            {
                return (string)this["WPA_KEY"];
            }
            set
            {
                this["WPA_KEY"] = value;
            }
        }
        public string MAC_ID_CABLE
        {
            get
            {
                return (string)this["MAC_ID_CABLE"];
            }
            set
            {
                this["MAC_ID_CABLE"] = value;
            }
        }
        public string MAC_ID_EMTA
        {
            get
            {
                return (string)this["MAC_ID_EMTA"];
            }
            set
            {
                this["MAC_ID_EMTA"] = value;
            }
        }
        public string HARDWARE_VERSION
        {
            get
            {
                return (string)this["HARDWARE_VERSION"];
            }
            set
            {
                this["HARDWARE_VERSION"] = value;
            }
        }
        public string FIRMWARE_VERSION
        {
            get
            {
                return (string)this["FIRMWARE_VERSION"];
            }
            set
            {
                this["FIRMWARE_VERSION"] = value;
            }
        }
        public string EAN_CODE
        {
            get
            {
                return (string)this["EAN_CODE"];
            }
            set
            {
                this["EAN_CODE"] = value;
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
        public string SRM_PASSWORD
        {
            get
            {
                return (string)this["SRM_PASSWORD"];
            }
            set
            {
                this["SRM_PASSWORD"] = value;
            }
        }
        public string RF_MAC_ID
        {
            get
            {
                return (string)this["RF_MAC_ID"];
            }
            set
            {
                this["RF_MAC_ID"] = value;
            }
        }
        public string MACID_IN_MTA
        {
            get
            {
                return (string)this["MACID_IN_MTA"];
            }
            set
            {
                this["MACID_IN_MTA"] = value;
            }
        }
        public string MTA_MAN_ROUTER_MAC
        {
            get
            {
                return (string)this["MTA_MAN_ROUTER_MAC"];
            }
            set
            {
                this["MTA_MAN_ROUTER_MAC"] = value;
            }
        }
        public string MTADATA_MAC
        {
            get
            {
                return (string)this["MTADATA_MAC"];
            }
            set
            {
                this["MTADATA_MAC"] = value;
            }
        }
        public string ETHERNET_MAC
        {
            get
            {
                return (string)this["ETHERNET_MAC"];
            }
            set
            {
                this["ETHERNET_MAC"] = value;
            }
        }
        public string USB_MAC
        {
            get
            {
                return (string)this["USB_MAC"];
            }
            set
            {
                this["USB_MAC"] = value;
            }
        }
        public string PRIMARYSSID_PASSPHRASE
        {
            get
            {
                return (string)this["PRIMARYSSID_PASSPHRASE"];
            }
            set
            {
                this["PRIMARYSSID_PASSPHRASE"] = value;
            }
        }
        public string CMCI_MAC
        {
            get
            {
                return (string)this["CMCI_MAC"];
            }
            set
            {
                this["CMCI_MAC"] = value;
            }
        }
        public string LAN_MAC
        {
            get
            {
                return (string)this["LAN_MAC"];
            }
            set
            {
                this["LAN_MAC"] = value;
            }
        }
        public string WAN_MAC
        {
            get
            {
                return (string)this["WAN_MAC"];
            }
            set
            {
                this["WAN_MAC"] = value;
            }
        }
        public string DEVICE_MAC
        {
            get
            {
                return (string)this["DEVICE_MAC"];
            }
            set
            {
                this["DEVICE_MAC"] = value;
            }
        }
        public string WIRELESS_MAC
        {
            get
            {
                return (string)this["WIRELESS_MAC"];
            }
            set
            {
                this["WIRELESS_MAC"] = value;
            }
        }
        public string WIFI_MAC_SSID1
        {
            get
            {
                return (string)this["WIFI_MAC_SSID1"];
            }
            set
            {
                this["WIFI_MAC_SSID1"] = value;
            }
        }
        public string SSID1
        {
            get
            {
                return (string)this["SSID1"];
            }
            set
            {
                this["SSID1"] = value;
            }
        }
        public string SSID1_PASSPHRASE
        {
            get
            {
                return (string)this["SSID1_PASSPHRASE"];
            }
            set
            {
                this["SSID1_PASSPHRASE"] = value;
            }
        }
        public string WPA_PASSPHRASE
        {
            get
            {
                return (string)this["WPA_PASSPHRASE"];
            }
            set
            {
                this["WPA_PASSPHRASE"] = value;
            }
        }
        public string WPS_PIN_CODE
        {
            get
            {
                return (string)this["WPS_PIN_CODE"];
            }
            set
            {
                this["WPS_PIN_CODE"] = value;
            }
        }
        public string PPPOA_USERNAME
        {
            get
            {
                return (string)this["PPPOA_USERNAME"];
            }
            set
            {
                this["PPPOA_USERNAME"] = value;
            }
        }
        public string PPPOA_PASSPHRASE
        {
            get
            {
                return (string)this["PPPOA_PASSPHRASE"];
            }
            set
            {
                this["PPPOA_PASSPHRASE"] = value;
            }
        }
        public string TR069_UNIQUE_KEY_64_BIT
        {
            get
            {
                return (string)this["TR069_UNIQUE_KEY_64_BIT"];
            }
            set
            {
                this["TR069_UNIQUE_KEY_64_BIT"] = value;
            }
        }
        public string FON_KEY
        {
            get
            {
                return (string)this["FON_KEY"];
            }
            set
            {
                this["FON_KEY"] = value;
            }
        }
        public string TA
{
            get
            {
                return (string)this["TA#"];
            }
            set
            {
                this["TA#"] = value;
            }
        }
    }
    public class R_NETGEAR_PTM_DATA
    {
        public string PA_SN { get; set; }
        public string PA_ITEM_NUMBER { get; set; }
        public string LASTEDITBY { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SHIPORDERID { get; set; }
        public double? PACKAGENO { get; set; }
        public string ORDERLINENO { get; set; }
        public double? SEQNO { get; set; }
        public string PALLET_ID { get; set; }
        public string MASTER_CARTON_ID { get; set; }
        public string ITEM_NUMBER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string TOP_SERIAL_NUMBER { get; set; }
        public string MAC_ID { get; set; }
        public string ASN_NUMBER { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public string PACKING_SLIP_NUMBER { get; set; }
        public string CONTAINER_NUMBER { get; set; }
        public string PO_NUMBER { get; set; }
        public string PO_LINE_NUMBER { get; set; }
        public string XFACTORY_DATE { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string AS_DATE_OF_MANUFACTURE { get; set; }
        public string COUNTRY_OF_ORIGIN { get; set; }
        public string ORG_CODE { get; set; }
        public string IMEI_NUMBER { get; set; }
        public string MASTERLOCK_NUMBER { get; set; }
        public string NETWORKLOCK_NUMBER { get; set; }
        public string SERVICELOCK_NUMBER { get; set; }
        public string FA_NUMBER_LEVEL_REV { get; set; }
        public string FA_NUMBER { get; set; }
        public string ITEM_NUMBER_LEVEL_REV { get; set; }
        public string WEP_KEY { get; set; }
        public string WIFI_ID { get; set; }
        public string ACCESS_CODE { get; set; }
        public string PRIMARY_SSID { get; set; }
        public string WPA_KEY { get; set; }
        public string MAC_ID_CABLE { get; set; }
        public string MAC_ID_EMTA { get; set; }
        public string HARDWARE_VERSION { get; set; }
        public string FIRMWARE_VERSION { get; set; }
        public string EAN_CODE { get; set; }
        public string SOFTWARE_VERSION { get; set; }
        public string SRM_PASSWORD { get; set; }
        public string RF_MAC_ID { get; set; }
        public string MACID_IN_MTA { get; set; }
        public string MTA_MAN_ROUTER_MAC { get; set; }
        public string MTADATA_MAC { get; set; }
        public string ETHERNET_MAC { get; set; }
        public string USB_MAC { get; set; }
        public string PRIMARYSSID_PASSPHRASE { get; set; }
        public string CMCI_MAC { get; set; }
        public string LAN_MAC { get; set; }
        public string WAN_MAC { get; set; }
        public string DEVICE_MAC { get; set; }
        public string WIRELESS_MAC { get; set; }
        public string WIFI_MAC_SSID1 { get; set; }
        public string SSID1 { get; set; }
        public string SSID1_PASSPHRASE { get; set; }
        public string WPA_PASSPHRASE { get; set; }
        public string WPS_PIN_CODE { get; set; }
        public string PPPOA_USERNAME { get; set; }
        public string PPPOA_PASSPHRASE { get; set; }
        public string TR069_UNIQUE_KEY_64_BIT { get; set; }
        public string FON_KEY { get; set; }
        public string TA{get;set;}
}
}