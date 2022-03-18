using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    /// <summary>
    /// R_SN_TGMES_INFO：存放用戶自行上傳客戶提供的信息，包含SN箱號棧板號任務令等
    /// </summary>
    public class T_R_SN_TGMES_INFO : DataObjectTable
    {
        public T_R_SN_TGMES_INFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_TGMES_INFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_TGMES_INFO);
            TableName = "R_SN_TGMES_INFO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        
        /// <summary>
        /// 多個TGMES SN批量過站，需要處於相同狀態
        /// </summary>
        public void LotsPassStation(List<R_SN_TGMES_INFO> TGMESlist, string Line, string StationName, string EmpNo, string BU, OleExec DB)
        {
            T_R_SN_TGMES_INFO T_TGMES = new T_R_SN_TGMES_INFO(DB, DB_TYPE_ENUM.Oracle);
            Row_R_SN_TGMES_INFO R_TGMES = (Row_R_SN_TGMES_INFO)NewRow();

            foreach (R_SN_TGMES_INFO TGMES in TGMESlist)
            {
                if (TGMES != null)
                {
                    if (!TGMES.NEXT_STATION.Equals(StationName))
                    {
                        throw new MESReturnMessage($@"The Next Station Of '{TGMES.PCBA_BARCODE}' Is Not Equal To {StationName}");
                    }
                    ChangeSnStatus(TGMES, StationName, EmpNo, DB);   
                    RecordPassStationDetail(TGMES, Line, StationName, EmpNo, BU, DB);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { TGMES.PCBA_BARCODE }));
                }
            }
        }

        /// <summary>
        /// 更新TGMES SN狀態
        /// </summary>
        public void ChangeSnStatus(R_SN_TGMES_INFO TGMES, string StationName, string EmpNo, OleExec DB)
        {
            if (TGMES != null)
            {
                if (StationName.Contains("CBS"))
                {
                    TGMES.COMPLETED_FLAG = "1";
                    TGMES.COMPLETED_TIME = GetDBDateTime(DB);
                    TGMES.CURRENT_STATION = StationName;
                    TGMES.NEXT_STATION = "TGMES_SHIPOUT";
                }
                else if (StationName.Contains("SHIPOUT"))
                {
                    TGMES.SHIPPED_FLAG = "1";
                    TGMES.SHIPDATE = GetDBDateTime(DB);
                    TGMES.CURRENT_STATION = StationName;
                    TGMES.NEXT_STATION = "TGMES_SHIPFINISH";
                }
                else if (StationName.Contains("RETURN"))
                {
                    TGMES.SHIPPED_FLAG = "0";
                    TGMES.CURRENT_STATION = "TGMES_CBS";
                    TGMES.NEXT_STATION = "TGMES_SHIPOUT";
                }                             
                TGMES.EDIT_TIME = GetDBDateTime(DB);
                TGMES.EDIT_EMP = EmpNo;
                DB.ORM.Updateable(TGMES).Where(t => t.ID == TGMES.ID).ExecuteCommand();
            }
        }

        /// <summary>
        /// 寫TGMES過站記錄
        /// </summary>
        public void RecordPassStationDetail(R_SN_TGMES_INFO TGMES, string Line, string StationName, string EmpNo, string BU, OleExec DB)
        {
            if (TGMES.VALID_FLAG == "1")
            {
                R_SN_STATION_DETAIL DETAIL = new R_SN_STATION_DETAIL()
                {
                    ID = MesDbBase.GetNewID(DB.ORM, BU, "R_SN_STATION_DETAIL"),
                    R_SN_ID = TGMES.ID,
                    SN = TGMES.PCBA_BARCODE,
                    SKUNO = TGMES.ITEM_SALES,
                    WORKORDERNO = TGMES.WORKORDERNO,
                    PLANT = "WDN1",
                    LINE = Line,
                    STARTED_FLAG = "1",
                    START_TIME = TGMES.DATALOAD_TIME,
                    COMPLETED_FLAG = TGMES.COMPLETED_FLAG,
                    COMPLETED_TIME = TGMES.COMPLETED_TIME,
                    CURRENT_STATION = TGMES.CURRENT_STATION,
                    NEXT_STATION = TGMES.NEXT_STATION,
                    DEVICE_NAME = StationName,
                    STATION_NAME = StationName,
                    EDIT_EMP = EmpNo,
                    EDIT_TIME = GetDBDateTime(DB)
                };
                DB.ORM.Insertable(DETAIL).ExecuteCommand();
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { TGMES.PCBA_BARCODE }));
            }
        }
    }
    public class Row_R_SN_TGMES_INFO : DataObjectBase
    {
        public Row_R_SN_TGMES_INFO(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_TGMES_INFO GetDataObject()
        {
            R_SN_TGMES_INFO DataObject = new R_SN_TGMES_INFO();
            DataObject.ID = this.ID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.QTY_M = this.QTY_M;
            DataObject.PCBA_BARCODE = this.PCBA_BARCODE;
            DataObject.PRODUCT_BARCODE = this.PRODUCT_BARCODE;
            DataObject.COMPLETED_FLAG = this.COMPLETED_FLAG;
            DataObject.COMPLETED_TIME = this.COMPLETED_TIME;
            DataObject.SHIPPED_FLAG = this.SHIPPED_FLAG;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.IMEI = this.IMEI;
            DataObject.PHYSICSNO = this.PHYSICSNO;
            DataObject.MAC_1 = this.MAC_1;
            DataObject.SPECIAL_SN_ID = this.SPECIAL_SN_ID;
            DataObject.WIFI_KEY = this.WIFI_KEY;
            DataObject.PACKING2 = this.PACKING2;
            DataObject.PACKINGWEIGHT2 = this.PACKINGWEIGHT2;
            DataObject.PACKING3 = this.PACKING3;
            DataObject.EMS_PCBA_PART_NO = this.EMS_PCBA_PART_NO;
            DataObject.HW_PCBA_PART_NO = this.HW_PCBA_PART_NO;
            DataObject.ITEM_SALES = this.ITEM_SALES;
            DataObject.ITEM_BOM = this.ITEM_BOM;
            DataObject.LOTNO = this.LOTNO;
            DataObject.MODEL = this.MODEL;
            DataObject.ORIGIN_CN = this.ORIGIN_CN;
            DataObject.ORIGIN_EN = this.ORIGIN_EN;
            DataObject.PRODUCT_DATE = this.PRODUCT_DATE;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.REMARK = this.REMARK;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.DATALOAD_EMP = this.DATALOAD_EMP;
            DataObject.DATALOAD_TIME = this.DATALOAD_TIME;
            DataObject.PLANT = this.PLANT;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA5 = this.DATA5;
            DataObject.PACKING_LIST_NO = this.PACKING_LIST_NO;
            DataObject.SSID = this.SSID;
            DataObject.SSID3 = this.SSID3;
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
        public double? QTY_M
        {
            get
            {
                return (double?)this["QTY_M"];
            }
            set
            {
                this["QTY_M"] = value;
            }
        }
        public string PCBA_BARCODE
        {
            get
            {
                return (string)this["PCBA_BARCODE"];
            }
            set
            {
                this["PCBA_BARCODE"] = value;
            }
        }
        public string PRODUCT_BARCODE
        {
            get
            {
                return (string)this["PRODUCT_BARCODE"];
            }
            set
            {
                this["PRODUCT_BARCODE"] = value;
            }
        }
        public string COMPLETED_FLAG
        {
            get
            {
                return (string)this["COMPLETED_FLAG"];
            }
            set
            {
                this["COMPLETED_FLAG"] = value;
            }
        }
        public DateTime? COMPLETED_TIME
        {
            get
            {
                return (DateTime?)this["COMPLETED_TIME"];
            }
            set
            {
                this["COMPLETED_TIME"] = value;
            }
        }
        public string SHIPPED_FLAG
        {
            get
            {
                return (string)this["SHIPPED_FLAG"];
            }
            set
            {
                this["SHIPPED_FLAG"] = value;
            }
        }
        public DateTime? SHIPDATE
        {
            get
            {
                return (DateTime?)this["SHIPDATE"];
            }
            set
            {
                this["SHIPDATE"] = value;
            }
        }
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
            }
        }
        public string NEXT_STATION
        {
            get
            {
                return (string)this["NEXT_STATION"];
            }
            set
            {
                this["NEXT_STATION"] = value;
            }
        }
        public string IMEI
        {
            get
            {
                return (string)this["IMEI"];
            }
            set
            {
                this["IMEI"] = value;
            }
        }
        public string PHYSICSNO
        {
            get
            {
                return (string)this["PHYSICSNO"];
            }
            set
            {
                this["PHYSICSNO"] = value;
            }
        }
        public string MAC_1
        {
            get
            {
                return (string)this["MAC_1"];
            }
            set
            {
                this["MAC_1"] = value;
            }
        }
        public string SPECIAL_SN_ID
        {
            get
            {
                return (string)this["SPECIAL_SN_ID"];
            }
            set
            {
                this["SPECIAL_SN_ID"] = value;
            }
        }
        public string WIFI_KEY
        {
            get
            {
                return (string)this["WIFI_KEY"];
            }
            set
            {
                this["WIFI_KEY"] = value;
            }
        }
        public string PACKING2
        {
            get
            {
                return (string)this["PACKING2"];
            }
            set
            {
                this["PACKING2"] = value;
            }
        }
        public string PACKINGWEIGHT2
        {
            get
            {
                return (string)this["PACKINGWEIGHT2"];
            }
            set
            {
                this["PACKINGWEIGHT2"] = value;
            }
        }
        public string PACKING3
        {
            get
            {
                return (string)this["PACKING3"];
            }
            set
            {
                this["PACKING3"] = value;
            }
        }
        public string EMS_PCBA_PART_NO
        {
            get
            {
                return (string)this["EMS_PCBA_PART_NO"];
            }
            set
            {
                this["EMS_PCBA_PART_NO"] = value;
            }
        }
        public string HW_PCBA_PART_NO
        {
            get
            {
                return (string)this["HW_PCBA_PART_NO"];
            }
            set
            {
                this["HW_PCBA_PART_NO"] = value;
            }
        }
        public string ITEM_SALES
        {
            get
            {
                return (string)this["ITEM_SALES"];
            }
            set
            {
                this["ITEM_SALES"] = value;
            }
        }
        public string ITEM_BOM
        {
            get
            {
                return (string)this["ITEM_BOM"];
            }
            set
            {
                this["ITEM_BOM"] = value;
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
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
            }
        }
        public string ORIGIN_CN
        {
            get
            {
                return (string)this["ORIGIN_CN"];
            }
            set
            {
                this["ORIGIN_CN"] = value;
            }
        }
        public string ORIGIN_EN
        {
            get
            {
                return (string)this["ORIGIN_EN"];
            }
            set
            {
                this["ORIGIN_EN"] = value;
            }
        }
        public string PRODUCT_DATE
        {
            get
            {
                return (string)this["PRODUCT_DATE"];
            }
            set
            {
                this["PRODUCT_DATE"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
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
        public string DATALOAD_EMP
        {
            get
            {
                return (string)this["DATALOAD_EMP"];
            }
            set
            {
                this["DATALOAD_EMP"] = value;
            }
        }
        public DateTime? DATALOAD_TIME
        {
            get
            {
                return (DateTime?)this["DATALOAD_TIME"];
            }
            set
            {
                this["DATALOAD_TIME"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
        public string DATA4
        {
            get
            {
                return (string)this["DATA4"];
            }
            set
            {
                this["DATA4"] = value;
            }
        }
        public string DATA5
        {
            get
            {
                return (string)this["DATA5"];
            }
            set
            {
                this["DATA5"] = value;
            }
        }
        public string PACKING_LIST_NO
        {
            get
            {
                return (string)this["PACKING_LIST_NO"];
            }
            set
            {
                this["PACKING_LIST_NO"] = value;
            }
        }
        public string SSID
        {
            get
            {
                return (string)this["SSID"];
            }
            set
            {
                this["SSID"] = value;
            }
        }
        public string SSID3
        {
            get
            {
                return (string)this["SSID3"];
            }
            set
            {
                this["SSID3"] = value;
            }
        }
    }
    public class R_SN_TGMES_INFO
    {
        public string ID { get; set; }
        public string WORKORDERNO { get; set; }
        public double? QTY_M { get; set; }
        public string PCBA_BARCODE { get; set; }
        public string PRODUCT_BARCODE { get; set; }
        public string COMPLETED_FLAG { get; set; }
        public DateTime? COMPLETED_TIME { get; set; }
        public string SHIPPED_FLAG { get; set; }
        public DateTime? SHIPDATE { get; set; }
        public string CURRENT_STATION { get; set; }
        public string NEXT_STATION { get; set; }
        public string IMEI { get; set; }
        public string PHYSICSNO { get; set; }
        public string MAC_1 { get; set; }
        public string SPECIAL_SN_ID { get; set; }
        public string WIFI_KEY { get; set; }
        public string PACKING2 { get; set; }
        public string PACKINGWEIGHT2 { get; set; }
        public string PACKING3 { get; set; }
        public string EMS_PCBA_PART_NO { get; set; }
        public string HW_PCBA_PART_NO { get; set; }
        public string ITEM_SALES { get; set; }
        public string ITEM_BOM { get; set; }
        public string LOTNO { get; set; }
        public string MODEL { get; set; }
        public string ORIGIN_CN { get; set; }
        public string ORIGIN_EN { get; set; }
        public string PRODUCT_DATE { get; set; }
        public string VALID_FLAG { get; set; }
        public string REMARK { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string DATALOAD_EMP { get; set; }
        public DateTime? DATALOAD_TIME { get; set; }
        public string PLANT { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
        public string PACKING_LIST_NO { get; set; }
        public string SSID { get; set; }
        public string SSID3 { get; set; }
    }
}
