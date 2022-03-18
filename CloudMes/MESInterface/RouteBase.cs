using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;
using MESDataObject.Module;

namespace MESInterface
{
   public class RouteBase
    {
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
        public RouteBase(string RouteId, OleExec sfcdb)
        {
            try
            {
                T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb,MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_ROUTE_DETAIL TC_ROUTE_DETAIL = new T_C_ROUTE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ROUTE getRoute = new C_ROUTE();
                List<C_ROUTE_DETAIL> getRouteDetail = new List<C_ROUTE_DETAIL>();
                getRoute = TC_ROUTE.GetById(RouteId, sfcdb);
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
                            RouteDetail newRouteDetail = new RouteDetail(getRouteDetail[i].ID, sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                            this._DETAIL.Add(newRouteDetail);
                        }
                    }
                    else
                    {
                        throw new Exception(RouteId + " 工站不存在");
                    }
                }
                else
                {
                    throw new Exception(RouteId+" 不存在");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetLastStation()
        {
            return _DETAIL[_DETAIL.Count - 1].STATION_NAME;
        }
        public bool CkeckRouteContain(string StationName)
        {
            bool isContain = false;
            foreach (RouteDetail ST in _DETAIL)
            {
                if (ST.STATION_NAME == StationName)
                {
                    isContain = true;
                    break;
                }
            }
            return isContain;
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
   
}
