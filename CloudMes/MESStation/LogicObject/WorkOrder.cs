using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;

namespace MESStation.LogicObject
{
    public class WorkOrder
    {      

        public string ID
        {
            get
            {
                return BaseWo.ID;
            }
        }
        public string WorkorderNo
        {
            get
            {
                return BaseWo.WORKORDERNO;
            }
        }

        public string PLANT
        {
            get
            {
                return BaseWo.PLANT;
            }
        }

        public DateTime? RELEASE_DATE
        {
            get
            {
                return BaseWo.RELEASE_DATE;
            }
        }

        public DateTime? DOWNLOAD_DATE
        {
            get
            {
                return BaseWo.DOWNLOAD_DATE;
            }
        }

        public string PRODUCTION_TYPE
        {
            get
            {
                return BaseWo.PRODUCTION_TYPE;
            }
        }

        public string WO_TYPE
        {
            get
            {
                return BaseWo.WO_TYPE;
            }
        }
        
        public string SkuNO
        {
            get
            {
                return BaseWo.SKUNO;
            }
        }

        public string SKU_VER
        {
            get
            {
                return BaseWo.SKU_VER;
            }
        }

        public string SKU_SERIES
        {
            get
            {
                return BaseWo.SKU_SERIES;
            }
        }

        public string SKU_NAME
        {
            get
            {
                return BaseWo.SKU_NAME;
            }
        }

        public string SKU_DESC
        {
            get
            {
                return BaseWo.SKU_DESC;
            }
        }

        public string CUST_PN
        {
            get
            {
                return BaseWo.CUST_PN;
            }
        }

        public string CUST_PN_VER
        {
            get
            {
                return BaseWo.CUST_PN_VER;
            }
        }

        public string CUSTOMER_NAME
        {
            get
            {
                return BaseWo.CUSTOMER_NAME;
            }
        }

        public string RouteID
        {
            get
            { return BaseWo.ROUTE_ID; }
        }

        public string START_STATION
        {
            get
            { return BaseWo.START_STATION; }
        }

        public string KP_LIST_ID
        {
            get
            { return BaseWo.KP_LIST_ID; }
        }

        public string CLOSED_FLAG
        {
            get
            { return BaseWo.CLOSED_FLAG; }
        }

        public DateTime? CLOSE_DATE
        {
            get
            { return BaseWo.CLOSE_DATE; }
        }

        public double? WORKORDER_QTY
        {
            get
            { return BaseWo.WORKORDER_QTY; }
        }

        public double? INPUT_QTY
        {
            get
            { return BaseWo.INPUT_QTY; }
        }

        public double? FINISHED_QTY
        {
            get
            { return BaseWo.FINISHED_QTY; }
        }

        public double? SCRAPED_QTY
        {
            get
            { return BaseWo.FINISHED_QTY; }
        }

        public string STOCK_LOCATION
        {
            get
            { return BaseWo.STOCK_LOCATION; }
        }

        public string PO_NO
        {
            get
            { return BaseWo.PO_NO; }
        }

        public string CUST_ORDER_NO
        {
            get
            { return BaseWo.CUST_ORDER_NO; }
        }

        public string ROHS
        {
            get
            { return BaseWo.ROHS; }
        }

        public string EDIT_EMP
        {
            get
            { return BaseWo.EDIT_EMP; }
        }

        public DateTime? EDIT_TIME
        {
            get
            { return BaseWo.EDIT_TIME; }
        }       

        public Route ROUTE
        {
            get
            {
                return _Route;
            }
            
        }

        //public List<R_SN> SNNO
        //{
        //    get
        //    {
        //        return _SNNO;
        //    }
        //}

        public List<Route> SkuRoutes;

        public List<string> ProcessingWo; //線上工單

        public C_PACKING Packing; //包裝參數

        public Dictionary<string, string> LabelPaths;

        Route _Route;
        //List<R_SN> _SNNO=new List<R_SN>();
        Row_R_WO_BASE RBaseWo;
        R_WO_BASE BaseWo;
        Row_R_SN strWo;
        MESDataObject.DB_TYPE_ENUM DBType;
        //WorkOrder wo = new WorkOrder();

        public  void Init(string strWo, MESDBHelper.OleExec SFCDB,MESDataObject.DB_TYPE_ENUM _DBType)
        {
            DBType = _DBType;
            T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBType);
            //T_R_SN TSN = new T_R_SN(SFCDB, DBType);
            //_SNNO = TSN.GETSN(strWo, SFCDB);
            RBaseWo = TRWB.GetWo(strWo, SFCDB);
            BaseWo = RBaseWo.GetDataObject();
            _Route = new Route(RBaseWo.ROUTE_ID, GetRouteType.ROUTEID,SFCDB,DBType); 
        }
        public void InitSn(string strSn, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            DBType = _DBType;
            //T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBType);
            T_R_SN TSN = new T_R_SN(SFCDB, DBType);
            strWo = TSN.GetWoBySn(strSn, SFCDB);
            //RBaseWo = TRWB.GetWo(strSn, SFCDB);
            //BaseWo = RBaseWo.GetDataObject();
            //_Route = new Route(RBaseWo.ROUTE_ID, GetRouteType.ROUTEID, SFCDB, DBType);

        }

        public WorkOrder Initwo(string strWo, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            DBType = _DBType;
            T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBType);
           
            RBaseWo = TRWB.GetWo(strWo, SFCDB);
            BaseWo = RBaseWo.GetDataObject();
            _Route = new Route(RBaseWo.ROUTE_ID, GetRouteType.ROUTEID, SFCDB, DBType);
            return this;
        }

        public Row_R_WO_BASE GetWoMode(string strWo, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            DBType = _DBType;
            T_R_WO_BASE TRWB = new T_R_WO_BASE(SFCDB, DBType);
            RBaseWo = TRWB.GetWo(strWo, SFCDB);
            return RBaseWo;

        }

        public void Init(string strWo, MESDBHelper.OleExec SFCDB)
        {
            Init(strWo, SFCDB, DBType);
        }

        public override string ToString()
        {
            return WorkorderNo;
        }

        
    }
}
