using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;



namespace MESStation.LogicObject
{
    public class Route
    {
        #region 屬性
        private string _ID;

        private string _ROUTE_NAME;

        private string _DEFAULT_SKUNO;

        private string _ROUTE_TYPE;

        private List<RouteDetail> _DETAIL;

        public string ID
        {
            get { return _ID; }
        }

        public string ROUTE_NAME
        {
            get { return _ROUTE_NAME; }
        }

        public string DEFAULT_SKUNO
        {
            get { return _DEFAULT_SKUNO; }
        }

        public string ROUTE_TYPE
        {
            get { return _ROUTE_TYPE; }
        }

        public List<RouteDetail> DETAIL
        {
            get { return _DETAIL; }
        }
        #endregion 屬性 end
        public Route()
        {
        }
        public Route(string parametvalue, GetRouteType type, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            switch (type)
            {
                case GetRouteType.ROUTEID: getByRouteId(parametvalue.Trim(), sfcdb, dbtype); break;
                case GetRouteType.ROUTENAME: getByRouteName(parametvalue.Trim(), sfcdb, dbtype); break;
                case GetRouteType.SNNO: getBySN(parametvalue.Trim(), sfcdb, dbtype); break;
                case GetRouteType.WONO: getByWO(parametvalue.Trim(), sfcdb, dbtype); break;
                default: throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "GetRouteType:" + type }));
            }
        }
        public void Init(string parametvalue, GetRouteType type, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            switch (type)
            {
                case GetRouteType.ROUTEID: getByRouteId(parametvalue.Trim(), sfcdb, dbtype); break;
                case GetRouteType.ROUTENAME: getByRouteName(parametvalue.Trim(), sfcdb, dbtype); break;
                case GetRouteType.SNNO: getBySN(parametvalue.Trim(), sfcdb, dbtype); break;
                case GetRouteType.WONO: getByWO(parametvalue.Trim(), sfcdb, dbtype); break;
                default: throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "GetRouteType:" + type }));
            }
        }
        public void Reflesh(OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            if (this.ID != null && this.ID.Trim().Length > 0)
            {
                getByRouteId(this.ID.Trim(), sfcdb, dbtype);
                return;
            }
            if (this.ROUTE_NAME != null && this.ROUTE_NAME.Trim().Length > 0)
            {
                getByRouteName(this.ROUTE_NAME.Trim(), sfcdb, dbtype);
                return;
            }
        }
        public override string ToString()
        {
            if (this.ROUTE_NAME != null)
            {
                return this.ROUTE_NAME;
            }
            else
            {
                return null;
            }
        }
        private void getByRouteId(string id, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            getByIdOrRouteName("ID", id, sfcdb, dbtype);
        }
        private void getBySN(string sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            //get sn route_id
            T_R_SN TR_SN = new T_R_SN(sfcdb, dbtype);
            R_SN getSN = TR_SN.GetDetailBySN(sn,sfcdb);
            string routeid = "";
            if (getSN != null && getSN .ROUTE_ID!= null)
            {
                routeid = getSN.ROUTE_ID.Trim();
            }       
            getByIdOrRouteName("ID", routeid, sfcdb, dbtype);
        }
        private void getByWO(string wo, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            //get wo route_id
            Row_R_WO_BASE RBaseWo;
            string routeid = "";
            T_R_WO_BASE TRWB = new T_R_WO_BASE(sfcdb, dbtype);         
            RBaseWo = TRWB.GetWo(wo, sfcdb);
            if (RBaseWo!=null&&RBaseWo.ROUTE_ID != null)
            {
                routeid = RBaseWo.ROUTE_ID.Trim();
            }           
            getByIdOrRouteName("ID", routeid, sfcdb, dbtype);
        }
        //一個機種可能會有多個路由，暫時不寫getBySKUNO
        //private void getBySKUNO(string skuno, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        //{
        //    //get skuno route_id
        //    string id = "";
        //    getByIdOrRouteName("ID", id, sfcdb, dbtype);
        //}
        private void getByRouteName(string routename, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            getByIdOrRouteName("ROUTENAME", routename, sfcdb, dbtype);
        }
        private void getByIdOrRouteName(string parametName, string parametValue, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            try
            {
                T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb, dbtype);
                T_C_ROUTE_DETAIL TC_ROUTE_DETAIL = new T_C_ROUTE_DETAIL(sfcdb, dbtype);
                C_ROUTE getRoute = new C_ROUTE();
                List<C_ROUTE_DETAIL> getRouteDetail = new List<C_ROUTE_DETAIL>();
                // List<RouteDetail> RouteDetailList = new List<RouteDetail>();
                //T_C_ROUTE_DETAIL_DIRECTLINK TC_ROUTE_DETAIL_DIRECTLINK = new T_C_ROUTE_DETAIL_DIRECTLINK(sfcdb, dbtype);
                //T_C_ROUTE_DETAIL_RETURN TC_ROUTE_DETAIL_RETURN = new T_C_ROUTE_DETAIL_RETURN(sfcdb, dbtype);
                //List<C_ROUTE_DETAIL_RETURN> getDetailReturn = new List<C_ROUTE_DETAIL_RETURN>();
                //List<C_ROUTE_DETAIL_DIRECTLINK> getDetailDirectLink = new List<C_ROUTE_DETAIL_DIRECTLINK>();

                if (parametName.Trim() == "ID")
                {
                    getRoute = TC_ROUTE.GetById(parametValue.Trim(), sfcdb);
                }
                else if (parametName.Trim() == "ROUTENAME")
                {
                    getRoute = TC_ROUTE.GetByRouteName(parametValue.Trim(), sfcdb);
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007",new string[] { "getByIdOrRouteName:"+parametName }));
                }
                if (getRoute != null)
                {
                    this._ID = getRoute.ID;
                    this._ROUTE_NAME = getRoute.ROUTE_NAME;
                    this._DEFAULT_SKUNO = getRoute.DEFAULT_SKUNO;
                    this._ROUTE_TYPE = getRoute.ROUTE_TYPE;
                    getRouteDetail = TC_ROUTE_DETAIL.GetByRouteIdOrderBySEQASC(getRoute.ID, sfcdb);
                    if (getRouteDetail != null && getRouteDetail.Count > 0)
                    {
                        this._DETAIL = new List<RouteDetail>();
                        for (int i = 0; i < getRouteDetail.Count; i++)
                        {
                            RouteDetail newRouteDetail = new RouteDetail(getRouteDetail[i].ID, sfcdb, dbtype);
                            #region 寫法一
                            /* 
                             RouteDetail newRouteDetail = new RouteDetail();
                             newRouteDetail.ID = getRouteDetail[i].ID;
                             newRouteDetail.SEQ_NO = getRouteDetail[i].SEQ_NO;
                             newRouteDetail.ROUTE_ID = getRouteDetail[i].ROUTE_ID;
                             newRouteDetail.STATION_NAME = getRouteDetail[i].STATION_NAME;
                             newRouteDetail.STATION_TYPE = getRouteDetail[i].STATION_TYPE;
                             newRouteDetail.RETURN_FLAG = getRouteDetail[i].RETURN_FLAG;
                             if (getRouteDetail[i].RETURN_FLAG == "Y")
                             {
                                 getDetailReturn = TC_ROUTE_DETAIL_RETURN.GetByRoute_DetailId(newRouteDetail.ID, sfcdb);
                                 if (getDetailReturn != null && getDetailReturn.Count > 0)
                                 {
                                     List<C_ROUTE_DETAIL> newReturnList = new List<C_ROUTE_DETAIL>();
                                     for (int j = 0; j < getDetailReturn.Count; j++)
                                     {
                                         newReturnList.Add(TC_ROUTE_DETAIL.GetById(getDetailReturn[j].RETURN_ROUTE_DETAIL_ID, sfcdb));
                                     }
                                     newRouteDetail.RETURNLIST = newReturnList;
                                 }
                             }
                             else
                             {
                                 newRouteDetail.RETURNLIST = null;
                             }
                             getDetailDirectLink = TC_ROUTE_DETAIL_DIRECTLINK.GetByDetailId(newRouteDetail.ID, sfcdb);
                             if (getDetailDirectLink != null && getDetailDirectLink.Count > 0)
                             {
                                 List<C_ROUTE_DETAIL> newDirectLinkList = new List<C_ROUTE_DETAIL>();
                                 for (int n = 0; n < getDetailDirectLink.Count; n++)
                                 {
                                     newDirectLinkList.Add(TC_ROUTE_DETAIL.GetById(getDetailDirectLink[n].DIRECTLINK_ROUTE_DETAIL_ID, sfcdb));
                                 }
                                 newRouteDetail.DIRECTLINKLIST = newDirectLinkList;
                             }
                             else
                             {
                                 newRouteDetail.DIRECTLINKLIST = null;
                             }
                             if (this.DETAIL == null)
                             {
                                 this.DETAIL = new List<RouteDetail>();
                             }*/
                            #endregion 寫法一 end                           
                            this._DETAIL.Add(newRouteDetail);
                        }
                    }
                    else
                    {
                        this._DETAIL = null;
                    }
                }
                else
                {
                    this._ID = "";
                    this._ROUTE_NAME = "";
                    this._DEFAULT_SKUNO = "";
                    this._ROUTE_TYPE = "";
                    this._DETAIL = null;
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "Route_"+ parametName + ":" + parametValue }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class RouteDetail
    {
        #region 屬性
        private string _ID;
        private double? _SEQ_NO;
        private string _ROUTE_ID;
        private string _STATION_NAME;
        private string _STATION_TYPE;
        private string _RETURN_FLAG;
        private C_ROUTE_DETAIL _NEXTSTATION;
        private List<C_ROUTE_DETAIL> _RETURNLIST;
        private List<C_ROUTE_DETAIL> _DIRECTLINKLIST;      
        public string ID
        {
            get { return _ID; }
        }
        public double? SEQ_NO
        {
            get { return _SEQ_NO; }
        }
        public string ROUTE_ID
        {
            get { return _ROUTE_ID; }
        }
        public string STATION_NAME
        {
            get { return _STATION_NAME; }
        }
        public string STATION_TYPE
        {
            get { return _STATION_TYPE; }
        }
        public string RETURN_FLAG
        {
            get { return _RETURN_FLAG; }
        }
        public C_ROUTE_DETAIL NEXTSTATION
        {
            get { return _NEXTSTATION; }
        }
        public List<C_ROUTE_DETAIL> RETURNLIST
        {
            get { return _RETURNLIST; }
        }
        public List<C_ROUTE_DETAIL> DIRECTLINKLIST
        {
            get { return _DIRECTLINKLIST; }
        }      
        #endregion 屬性 end
        public RouteDetail()
        {
        }
        public RouteDetail(string id, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            getById(id, sfcdb, dbtype);
        }
        public override string ToString()
        {            
            return this.STATION_NAME;
        }
        private void getById(string id, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbtype)
        {
            T_C_ROUTE_DETAIL TC_ROUTE_DETAIL = new T_C_ROUTE_DETAIL(sfcdb, dbtype);
            T_C_ROUTE_DETAIL_DIRECTLINK TC_ROUTE_DETAIL_DIRECTLINK = new T_C_ROUTE_DETAIL_DIRECTLINK(sfcdb, dbtype);
            T_C_ROUTE_DETAIL_RETURN TC_ROUTE_DETAIL_RETURN = new T_C_ROUTE_DETAIL_RETURN(sfcdb, dbtype);
            C_ROUTE_DETAIL getRouteDetail = new C_ROUTE_DETAIL();
            List<C_ROUTE_DETAIL_RETURN> getDetailReturn = new List<C_ROUTE_DETAIL_RETURN>();
            List<C_ROUTE_DETAIL_DIRECTLINK> getDetailDirectLink = new List<C_ROUTE_DETAIL_DIRECTLINK>();
            getRouteDetail = TC_ROUTE_DETAIL.GetById(id, sfcdb);
            if (getDetailReturn != null)
            {
                this._ID = getRouteDetail.ID;
                this._SEQ_NO = getRouteDetail.SEQ_NO;
                this._ROUTE_ID = getRouteDetail.ROUTE_ID;
                this._STATION_NAME = getRouteDetail.STATION_NAME;
                this._STATION_TYPE = getRouteDetail.STATION_TYPE;
                this._RETURN_FLAG = getRouteDetail.RETURN_FLAG;
                if (getRouteDetail.RETURN_FLAG == "Y")
                {
                    getDetailReturn = TC_ROUTE_DETAIL_RETURN.GetByRoute_DetailId(getRouteDetail.ID, sfcdb);
                    if (getDetailReturn != null && getDetailReturn.Count > 0)
                    {
                        List<C_ROUTE_DETAIL> newReturnList = new List<C_ROUTE_DETAIL>();
                        for (int j = 0; j < getDetailReturn.Count; j++)
                        {
                            newReturnList.Add(TC_ROUTE_DETAIL.GetById(getDetailReturn[j].RETURN_ROUTE_DETAIL_ID, sfcdb));
                        }
                        this._RETURNLIST = newReturnList;
                    }
                }
                else
                {
                    this._RETURNLIST = null;
                }
                getDetailDirectLink = TC_ROUTE_DETAIL_DIRECTLINK.GetByDetailId(getRouteDetail.ID, sfcdb);
                if (getDetailDirectLink != null && getDetailDirectLink.Count > 0)
                {
                    List<C_ROUTE_DETAIL> newDirectLinkList = new List<C_ROUTE_DETAIL>();
                    for (int n = 0; n < getDetailDirectLink.Count; n++)
                    {
                        newDirectLinkList.Add(TC_ROUTE_DETAIL.GetById(getDetailDirectLink[n].DIRECTLINK_ROUTE_DETAIL_ID, sfcdb));
                    }
                    this._DIRECTLINKLIST = newDirectLinkList;
                }
                else
                {
                    this._DIRECTLINKLIST = null;
                }
            }
            else
            {
                this._ID = "";
                this._SEQ_NO = null;
                this._ROUTE_ID = "";
                this._STATION_NAME = "";
                this._STATION_TYPE = "";
                this._RETURN_FLAG = "";
                this._DIRECTLINKLIST = null;
                this._RETURNLIST = null;
                this._NEXTSTATION = null;
            }
        }
    }
    public enum GetRouteType
    {
        ROUTEID,
        ROUTENAME,
        SNNO,
        WONO
    }

}
