using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;

namespace MESInterface
{
  public  class WOBase
    {
        R_WO_BASE WO;
        SAPMappingBase _SAPMapping;
        RouteBase _Route;

        public WOBase(string wo,OleExec sfcdb)
        {
            T_R_WO_BASE TR_WO_BASE = new T_R_WO_BASE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
             WO= TR_WO_BASE.GetWoByWoNo(wo, sfcdb);
            if (WO == null)
            {
                throw new Exception("工單 " + wo + " 不存在！");
            }
            else
            {
                if (WO.SKUNO == null||WO.SKUNO.Trim().Length<=0)
                {
                    throw new Exception("工單 " + wo + " 未配置機種！");
                }
                _SAPMapping = new SAPMappingBase(WO.SKUNO,sfcdb);
                if (WO.ROUTE_ID == null || WO.ROUTE_ID.Trim().Length <= 0)
                {
                    throw new Exception("工單 " + wo + " 未配置路由ID！");
                }
                _Route = new RouteBase(WO.ROUTE_ID.Trim(),sfcdb);
            }

        }
        public SAPMappingBase SAPMapping
        {
            get
            {
                return _SAPMapping;
            }
        }
        public RouteBase Route
        {
            get
            {
                return _Route;
            }
        }
        public string ID
        {
            get
            {
                return (WO==null||WO.ID==null)?null:WO.ID;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return  (WO == null || WO.WORKORDERNO == null) ? null : WO.WORKORDERNO ;
            }
          
        }
        public string PLANT
        {
            get
            {
                return (WO == null || WO.PLANT == null) ? null : WO.PLANT;
            }
        }
        public DateTime? RELEASE_DATE
        {
            get
            {
                return (WO == null) ? null : WO.RELEASE_DATE;
            }
           
        }
        public DateTime? DOWNLOAD_DATE
        {
            get
            {
                return (WO == null) ? null :WO.DOWNLOAD_DATE;
            }
        }
        public string PRODUCTION_TYPE
        {
            get
            {
                return (WO == null || WO.PRODUCTION_TYPE == null) ? null : WO.PRODUCTION_TYPE;
            }
        }
        public string WO_TYPE
        {
            get
            {
                return (WO == null || WO.WO_TYPE == null) ? null : WO.WO_TYPE;
            }
          
        }
        public string SKUNO
        {
            get
            {
                return (WO == null || WO.SKUNO == null) ? null : WO.SKUNO;
            }
           
        }
        public string SKU_VER
        {
            get
            {
                return (WO == null || WO.SKU_VER == null) ? null : WO.SKU_VER;
            }
        }
        public string SKU_SERIES
        {
            get
            {
                return (WO == null || WO.SKU_SERIES == null) ? null : WO.SKU_SERIES;
            }
        }
        public string SKU_NAME
        {
            get
            {
                return (WO == null || WO.SKU_NAME == null) ? null : WO.SKU_NAME;
            }
        }
        public string SKU_DESC
        {
            get
            {
                return (WO == null || WO.SKU_DESC == null) ? null : WO.SKU_DESC;
            }
        }
        public string CUST_PN
        {
            get
            {
                return (WO == null || WO.CUST_PN == null) ? null : WO.CUST_PN;
            }
        }
        public string CUST_PN_VER
        {
            get
            {
                return (WO == null || WO.CUST_PN_VER == null) ? null : WO.CUST_PN_VER;
            }
        }
        public string CUSTOMER_NAME
        {
            get
            {
                return (WO == null || WO.CUSTOMER_NAME == null) ? null : WO.CUSTOMER_NAME;
            }
            
        }
        public string START_STATION
        {
            get
            {
                return (WO == null || WO.START_STATION == null) ? null : WO.START_STATION;
            }
        }
        public string KP_LIST_ID
        {
            get
            {
                return (WO == null || WO.KP_LIST_ID == null) ? null : WO.KP_LIST_ID;
            }
        }
        public string CLOSED_FLAG
        {
            get
            {
                return (WO == null || WO.CLOSED_FLAG == null) ? null : WO.CLOSED_FLAG;
            }
        }
        public DateTime? CLOSE_DATE
        {
            get
            {
                return (WO == null) ? null : WO.CLOSE_DATE;
            }
        }
        public double? WORKORDER_QTY
        {
            get
            {
                return (WO == null || WO.WORKORDER_QTY == null) ? 0 : WO.WORKORDER_QTY;
            }
        }
        public double? INPUT_QTY
        {
            get
            {
                return (WO == null || WO.INPUT_QTY == null) ? 0 : WO.INPUT_QTY;
            }
           
        }
        public double? FINISHED_QTY
        {
            get
            {
                return (WO == null || WO.FINISHED_QTY == null) ? 0 : WO.FINISHED_QTY;
            }
        }
        public double? SCRAPED_QTY
        {
            get
            {
                return (WO == null || WO.SCRAPED_QTY == null) ? 0 : WO.SCRAPED_QTY;
            }
           
        }
        public string STOCK_LOCATION
        {
            get
            {
                return (WO == null || WO.STOCK_LOCATION == null) ? null : WO.STOCK_LOCATION;
            }
            
        }
        public string PO_NO
        {
            get
            {
                return (WO == null || WO.PO_NO == null) ? null : WO.PO_NO;
            }
           
        }
        public string CUST_ORDER_NO
        {
            get
            {
                return (WO == null || WO.CUST_ORDER_NO == null) ? null : WO.CUST_ORDER_NO;
            }
        }
        public string ROHS
        {
            get
            {
                return (WO == null || WO.ROHS == null) ? null : WO.ROHS;
            }
            
        }
        public string EDIT_EMP
        {
            get
            {
                return (WO == null || WO.EDIT_EMP == null) ? null : WO.EDIT_EMP;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (WO == null || WO.EDIT_TIME == null) ? null : WO.EDIT_TIME;
            }
        }
        public int GetStationPassCount(string StationName, bool isContailFail,OleExec sfcdb)
        {
            int result = 0;
            T_R_SN_STATION_DETAIL TR_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(sfcdb,MESDataObject.DB_TYPE_ENUM.Oracle);
            if (isContailFail)
            {
                result = TR_SN_STATION_DETAIL.GetCountByWOAndStation(WO.WORKORDERNO, StationName, sfcdb);
            }
            else
            {
                result = TR_SN_STATION_DETAIL.GetCountByWOAndStationNotContailFail(WO.WORKORDERNO, StationName, sfcdb);
            }
                return result;
        }
    }
}
