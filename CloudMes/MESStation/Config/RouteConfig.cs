using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESDBHelper;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime;
using System.Data;
using SqlSugar;
using MESDataObject;

namespace MESStation.Config
{
    public class RouteConfig : MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetRouteByRouteName = new APIInfo()
        {
            FunctionName = "GetRouteByRouteName",
            Description = "通過路由名獲取路由信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RouteName" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetRouteByRouteId = new APIInfo()
        {
            FunctionName = "GetRouteByRouteId",
            Description = "通過路由ID取路由信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "RouteId" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAddRoute = new APIInfo()
        {
            FunctionName = "AddRoute",
            Description = "添加新路由",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "RouteJsonString" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDeleteRouteByRouteName = new APIInfo()
        {
            FunctionName = "DeleteRouteByRouteName",
            Description = "通過路由名刪除路由信息",
            Parameters = new List<APIInputInfo>() {
                 new APIInputInfo() { InputName = "RouteName" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDeleteRouteByRouteId = new APIInfo()
        {
            FunctionName = "DeleteRouteByRouteId",
            Description = "通過路由名刪除路由信息",
            Parameters = new List<APIInputInfo>() {
                 new APIInputInfo() { InputName = "RouteId" }
            },
            Permissions = new List<MESPermission>()
        }; 
        protected APIInfo FUpdateRoute_DeleteOldAndAddNew = new APIInfo()
        {
            FunctionName = "UpdateRoute_DeleteOldAndAddNew",
            Description = "通過路由ID更新路由，刪除原來的路由，然後添加新的路由，路由ID會變",
            Parameters = new List<APIInputInfo>() {
               new APIInputInfo() { InputName = "RouteJsonString" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateRoute = new APIInfo()
        {
            FunctionName = "UpdateRoute",
            Description = "通過路由ID更新路由，新舊兩個路由對比，保持原來的路由ID",
            Parameters = new List<APIInputInfo>() {
               new APIInputInfo() { InputName = "RouteJsonString" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetRouteMainMessage = new APIInfo()
        {
            FunctionName = "GetRouteMainMessage",
            Description = "按照路由名，模糊查詢路由的主要信息,RouteName為空的時候默認查詢所有路由主要信息",
            Parameters = new List<APIInputInfo>() {
                 new APIInputInfo() { InputName = "RouteName" },
                 new APIInputInfo() { InputName = "PageNumber"},
                 new APIInputInfo() { InputName = "PageSize"},
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FCopyRouteByRouteName = new APIInfo()
        {
            FunctionName = "CopyRouteByRouteName",
            Description = "通過路由名稱複製出一個新的路由",
            Parameters = new List<APIInputInfo>() {
                 new APIInputInfo() { InputName = "FromRouteName" },
                 new APIInputInfo() { InputName = "ToRouteName" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetNewRouteName = new APIInfo()
        {
            FunctionName = "GetNewRouteName",
            Description = "获取默认的新的路由名称",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAllStationType = new APIInfo()
        {
            FunctionName = "GetAllStationType",
            Description = "获取工站所有類型",
            Parameters = new List<APIInputInfo>() {                
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FSaveSampleteStation = new APIInfo()
        {
            FunctionName = "SaveSampleteStation",
            Description = "設置為SampleteStation",
            Parameters = new List<APIInputInfo>() {
                 new APIInputInfo() { InputName = "SampleteList" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FCheckSampleteStation = new APIInfo()
        {
            FunctionName = "CheckSampleteStation",
            Description = "",
            Parameters = new List<APIInputInfo>() {
                 new APIInputInfo() { InputName = "DetailID" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetStationBy90Days = new APIInfo()
        {
            FunctionName = "GetStationBy90Days",
            Description = "",
            Parameters = new List<APIInputInfo>() {
            },
            Permissions = new List<MESPermission>() { }
        };

        #endregion 方法信息集合 end
        public RouteConfig()
        {
            this.Apis.Add(FGetRouteByRouteName.FunctionName, FGetRouteByRouteName);
            this.Apis.Add(FGetRouteByRouteId.FunctionName, FGetRouteByRouteId);
            this.Apis.Add(FAddRoute.FunctionName, FAddRoute);
            this.Apis.Add(FDeleteRouteByRouteName.FunctionName, FDeleteRouteByRouteName);
            this.Apis.Add(FDeleteRouteByRouteId.FunctionName, FDeleteRouteByRouteId);
            this.Apis.Add(FUpdateRoute_DeleteOldAndAddNew.FunctionName, FUpdateRoute_DeleteOldAndAddNew);
            this.Apis.Add(FUpdateRoute.FunctionName, FUpdateRoute);
            this.Apis.Add(FGetRouteMainMessage.FunctionName, FGetRouteMainMessage);
            this.Apis.Add(FCopyRouteByRouteName.FunctionName, FCopyRouteByRouteName);
            this.Apis.Add(FGetNewRouteName.FunctionName, FGetNewRouteName);          
            this.Apis.Add(FGetAllStationType.FunctionName, FGetAllStationType);
            this.Apis.Add(FSaveSampleteStation.FunctionName, FSaveSampleteStation);
            this.Apis.Add(FCheckSampleteStation.FunctionName, FCheckSampleteStation);
        }
        private Route SelectRouteByRouteName(string RouteName, OleExec sfcdb)
        {           
            try
            {
                Route SelectRoute = new Route();               
                T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                SelectRoute.MainMessage = TC_ROUTE.GetByRouteName(RouteName, sfcdb);
                if (SelectRoute.MainMessage != null)
                {
                    T_C_ROUTE_DETAIL TC_ROUTE_DETAIL = new T_C_ROUTE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    List<C_ROUTE_DETAIL> SelectRouteDetail = TC_ROUTE_DETAIL.GetByRouteIdOrderBySEQASC(SelectRoute.MainMessage.ID, sfcdb);
                    T_C_ROUTE_DETAIL_DIRECTLINK TC_ROUTE_DETAIL_DIRECTLINK = new T_C_ROUTE_DETAIL_DIRECTLINK(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_C_ROUTE_DETAIL_RETURN TC_ROUTE_DETAIL_RETURN = new T_C_ROUTE_DETAIL_RETURN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    if (SelectRouteDetail != null && SelectRouteDetail.Count > 0)
                    {
                        #region 循環添加c_route_detail
                        for (int i = 0; i < SelectRouteDetail.Count; i++)
                        {
                            RouteDetailItem DetailItem = new RouteDetailItem();
                            DetailItem.LoadC_ROUTE_Detail(SelectRouteDetail[i]);
                            if (i < (SelectRouteDetail.Count - 1))
                            {
                                C_ROUTE_DETAIL nextstation = new C_ROUTE_DETAIL();
                                nextstation = SelectRouteDetail[i + 1];
                                DetailItem.NextStation = nextstation;
                            }
                            else
                            {
                                DetailItem.NextStation = null;
                            }
                            int count = TC_ROUTE_DETAIL_DIRECTLINK.GetCountByDetailId(SelectRouteDetail[i].ID, sfcdb);
                            if (count > 0)
                            {
                                List<C_ROUTE_DETAIL_DIRECTLINK> detailitemlist = TC_ROUTE_DETAIL_DIRECTLINK.GetByDetailId(SelectRouteDetail[i].ID, sfcdb);
                                for (int n = 0; n < detailitemlist.Count; n++)
                                {
                                    DetailItem.DirectLinkStations.Add(TC_ROUTE_DETAIL.GetById(detailitemlist[n].DIRECTLINK_ROUTE_DETAIL_ID, sfcdb));
                                }
                            }
                            if (SelectRouteDetail[i].RETURN_FLAG == "N")
                            {                               
                                DetailItem.ReturnItems = null;
                            }
                            else if (SelectRouteDetail[i].RETURN_FLAG == "Y")
                            { 
                                List<C_ROUTE_DETAIL> itemreturn = new List<C_ROUTE_DETAIL>();
                                List<C_ROUTE_DETAIL_RETURN> cdetailitemreturn = new List<C_ROUTE_DETAIL_RETURN>();
                                cdetailitemreturn = TC_ROUTE_DETAIL_RETURN.GetByRoute_DetailId(SelectRouteDetail[i].ID, sfcdb);
                                if (cdetailitemreturn!=null&&cdetailitemreturn.Count > 0)
                                {
                                    for (int j = 0; j < cdetailitemreturn.Count; j++)
                                    {
                                        itemreturn.Add(TC_ROUTE_DETAIL.GetById(cdetailitemreturn[j].RETURN_ROUTE_DETAIL_ID, sfcdb));
                                    }
                                }
                                DetailItem.ReturnItems = itemreturn;                               
                            }//end else if
                            SelectRoute.Detail.Add(DetailItem);
                        }//end for
                        #endregion 循環添加c_route_detail end
                    }//end if                   
                }//end if 
                return SelectRoute;
            }
            catch (Exception ex)
            {               
                throw new Exception(ex.Message);
            }           
        }
        private Route SelectRouteByRouteId(string RouteId, OleExec sfcdb)
        {
            try
            {               
                Route SelectRoute = new Route();
                T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                SelectRoute.MainMessage = TC_ROUTE.GetById(RouteId, sfcdb);
                if (SelectRoute.MainMessage != null)
                {
                    T_C_ROUTE_DETAIL TC_ROUTE_DETAIL = new T_C_ROUTE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    List<C_ROUTE_DETAIL> SelectRouteDetail = TC_ROUTE_DETAIL.GetByRouteIdOrderBySEQASC(SelectRoute.MainMessage.ID, sfcdb);
                    T_C_ROUTE_DETAIL_DIRECTLINK TC_ROUTE_DETAIL_DIRECTLINK = new T_C_ROUTE_DETAIL_DIRECTLINK(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_C_ROUTE_DETAIL_RETURN TC_ROUTE_DETAIL_RETURN = new T_C_ROUTE_DETAIL_RETURN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    if (SelectRouteDetail != null && SelectRouteDetail.Count > 0)
                    {
                        #region 循環添加c_route_detail
                        for (int i = 0; i < SelectRouteDetail.Count; i++)
                        {
                            RouteDetailItem DetailItem = new RouteDetailItem();
                            DetailItem.LoadC_ROUTE_Detail(SelectRouteDetail[i]);
                            if (i < (SelectRouteDetail.Count - 1))
                            {
                                C_ROUTE_DETAIL nextstation = new C_ROUTE_DETAIL();
                                nextstation = SelectRouteDetail[i + 1];
                                DetailItem.NextStation = nextstation;
                            }
                            else
                            {
                                DetailItem.NextStation = null;
                            }
                            int count = TC_ROUTE_DETAIL_DIRECTLINK.GetCountByDetailId(SelectRouteDetail[i].ID, sfcdb);
                            if (count > 0)
                            {
                                List<C_ROUTE_DETAIL_DIRECTLINK> detailitemlist = TC_ROUTE_DETAIL_DIRECTLINK.GetByDetailId(SelectRouteDetail[i].ID, sfcdb);
                                for (int n = 0; n < detailitemlist.Count; n++)
                                {
                                    DetailItem.DirectLinkStations.Add(TC_ROUTE_DETAIL.GetById(detailitemlist[n].DIRECTLINK_ROUTE_DETAIL_ID, sfcdb));
                                }
                            }
                            if (SelectRouteDetail[i].RETURN_FLAG == "N")
                            {
                                DetailItem.ReturnItems = null;
                            }
                            else if (SelectRouteDetail[i].RETURN_FLAG == "Y")
                            {
                                List<C_ROUTE_DETAIL> itemreturn = new List<C_ROUTE_DETAIL>();
                                List<C_ROUTE_DETAIL_RETURN> cdetailitemreturn = new List<C_ROUTE_DETAIL_RETURN>();
                                cdetailitemreturn = TC_ROUTE_DETAIL_RETURN.GetByRoute_DetailId(SelectRouteDetail[i].ID, sfcdb);
                                if (cdetailitemreturn!=null&&cdetailitemreturn.Count > 0)
                                {
                                    for (int j = 0; j < cdetailitemreturn.Count; j++)
                                    {
                                        itemreturn.Add(TC_ROUTE_DETAIL.GetById(cdetailitemreturn[j].RETURN_ROUTE_DETAIL_ID, sfcdb));
                                    }
                                }
                                DetailItem.ReturnItems = itemreturn;
                            }//end else if
                            SelectRoute.Detail.Add(DetailItem);
                        }//end for
                        #endregion 循環添加c_route_detail end
                    }//end if                   
                }//end if               
                return SelectRoute;
            }//end try
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void GetStationItems(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            T_C_ROUTE_DETAIL RouteDetailTable = null;
            List<C_ROUTE_DETAIL> RouteDetails = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string sn = Data["SN"].ToString(); ;
                List<string> RepairItemsList = new List<string>();
                T_R_SN R_SN = new T_R_SN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                R_SN rSn = R_SN.LoadSN(sn, sfcdb);
                RouteDetailTable = new T_C_ROUTE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(rSn.ROUTE_ID, sfcdb);

                StationReturn.Data = RouteDetails;
                
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        private void AddNewRoute(Route newRoute, OleExec sfcdb, MESStationReturn StationReturn)
        {
            Route checkRoute = new Route();
            string buName = BU;
            string strNULL = "";
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
            if (newRoute == null)
            {
                //throw new Exception("路由不能NULL");
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("RouteJsonString");
                return;
            }
            if (newRoute.MainMessage == null || newRoute.MainMessage.ROUTE_NAME == null || newRoute.MainMessage.ROUTE_NAME.Trim().Length <= 0)
            {
                //throw new Exception("路由名不能為空");
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("ROUTE_NAME");
                return;
            }
            else
            {
                checkRoute = SelectRouteByRouteName(newRoute.MainMessage.ROUTE_NAME.Trim(),sfcdb);
                if (checkRoute.MainMessage != null)
                {
                    //throw new Exception(newRoute.MainMessage.ROUTE_NAME + "已存在！");
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(newRoute.MainMessage.ROUTE_NAME);
                    return;
                }
                else
                {
                    newRoute.MainMessage.ROUTE_NAME = newRoute.MainMessage.ROUTE_NAME.Trim();
                    if (newRoute.MainMessage.DEFAULT_SKUNO == null)
                    {
                        newRoute.MainMessage.DEFAULT_SKUNO = strNULL;
                    }
                    else
                    {
                        newRoute.MainMessage.DEFAULT_SKUNO = newRoute.MainMessage.DEFAULT_SKUNO.Trim();
                    }
                    if (newRoute.MainMessage.ROUTE_TYPE == null)
                    {
                        newRoute.MainMessage.ROUTE_TYPE = strNULL;
                    }
                    else
                    {
                        newRoute.MainMessage.ROUTE_TYPE = newRoute.MainMessage.ROUTE_TYPE.Trim();
                    }
                    newRoute.MainMessage.EDIT_EMP = LoginUser.EMP_NO;
                    newRoute.MainMessage.EDIT_TIME = GetDBDateTime();
                    int result = 0;
                    string olddetailid = "";
                    T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_C_ROUTE_DETAIL TC_ROUTE_DETAIL = new T_C_ROUTE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_C_ROUTE_DETAIL_RETURN TC_ROUTE_DETAIL_RETURN = new T_C_ROUTE_DETAIL_RETURN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_C_ROUTE_DETAIL_DIRECTLINK TC_ROUTE_DETAIL_DIRECTLINK = new T_C_ROUTE_DETAIL_DIRECTLINK(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_C_STATION TC_STATION = new T_C_STATION(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    List<C_STATION_DETAIL> checkStationNameList = new List<C_STATION_DETAIL>();
                    List<string> DetailIdList = new List<string>();
                    List<string> OldDetailIdList = new List<string>();
                    List<double?> SEQList = new List<double?>();
                    try
                    {
                        newRoute.MainMessage.ID = TC_ROUTE.GetNewID(buName, sfcdb);
                        result = TC_ROUTE.Add(newRoute.MainMessage, sfcdb);
                        if (result > 0)
                        {
                            if (newRoute.Detail != null && newRoute.Detail.Count > 0)
                            {
                                #region 循環添加Detail
                                for (int i = 0; i < newRoute.Detail.Count; i++)
                                {
                                    if (newRoute.Detail[i] != null)
                                    {
                                        if (newRoute.Detail[i].ID == null || newRoute.Detail[i].ID.Trim().Length <= 0)
                                        {
                                            // throw new Exception("路由Detail的ID不能為空");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000006";
                                            StationReturn.MessagePara.Add("Detail_ID");
                                            return;
                                        }
                                        else
                                        {
                                            newRoute.Detail[i].ID = newRoute.Detail[i].ID.Trim();
                                            if (OldDetailIdList.Contains(newRoute.Detail[i].ID))
                                            {
                                                StationReturn.Status = StationReturnStatusValue.Fail;
                                                StationReturn.MessageCode = "MES00000024";
                                                StationReturn.MessagePara.Add("Detail_ID");
                                                return;
                                            }
                                            else
                                            {
                                                OldDetailIdList.Add(newRoute.Detail[i].ID);
                                            }
                                        }
                                        if (newRoute.Detail[i].SEQ_NO == null)
                                        {
                                            //throw new Exception("路由序號不能為NULL，只能是數字");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000006";
                                            StationReturn.MessagePara.Add("SEQ_NO");
                                            return;
                                        }
                                        if (SEQList.Contains(newRoute.Detail[i].SEQ_NO))
                                        {
                                            //throw new Exception("同一個路由序號必須唯一");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000024";
                                            StationReturn.MessagePara.Add("SEQ_NO");
                                            return;
                                        }
                                        if (newRoute.Detail[i].STATION_NAME == null || newRoute.Detail[i].STATION_NAME.Trim().Length <= 0)
                                        {
                                            //throw new Exception("工站名不能為空");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000006";
                                            StationReturn.MessagePara.Add("STATION_NAME");
                                            return;
                                        }
                                        else
                                        {
                                            newRoute.Detail[i].STATION_NAME = newRoute.Detail[i].STATION_NAME.Trim();
                                            //檢查工站名
                                            checkStationNameList = TC_STATION.GetDataByColumn("station_name", newRoute.Detail[i].STATION_NAME, sfcdb);
                                            if (checkStationNameList == null || checkStationNameList.Count <= 0)
                                            {
                                                // throw new Exception("工站名不存在");
                                                StationReturn.Status = StationReturnStatusValue.Fail;
                                                StationReturn.MessageCode = "MES00000007";
                                                StationReturn.MessagePara.Add(newRoute.Detail[i].STATION_NAME);
                                                return;
                                            }
                                        }
                                        if (newRoute.Detail[i].STATION_TYPE == null || newRoute.Detail[i].STATION_TYPE.Trim().Length <= 0)
                                        {
                                            //throw new Exception("工站類型不能為空");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000006";
                                            StationReturn.MessagePara.Add("STATION_TYPE");
                                            return;
                                        }
                                        else
                                        {
                                            newRoute.Detail[i].STATION_TYPE = newRoute.Detail[i].STATION_TYPE.Trim();
                                            //類型判斷
                                            if (!Enum.IsDefined(typeof(STATIONTYPE), newRoute.Detail[i].STATION_TYPE))
                                            {
                                                StationReturn.Status = StationReturnStatusValue.Fail;
                                                StationReturn.MessageCode = "MES00000020";
                                                StationReturn.MessagePara.Add("STATION_TYPE");
                                                string stationType = "";
                                                foreach (string typeString in Enum.GetNames(typeof(STATIONTYPE)))
                                                {
                                                    if(stationType=="")
                                                    {
                                                        stationType = stationType + typeString;
                                                    }
                                                    else
                                                    {
                                                        stationType = stationType+"," + typeString;
                                                    }
                                                }
                                                StationReturn.MessagePara.Add(stationType);
                                                return;
                                            }
                                        }
                                        if (newRoute.Detail[i].RETURN_FLAG == null || newRoute.Detail[i].RETURN_FLAG.Trim().Length <= 0)
                                        {
                                            //throw new Exception("工站是否可進維修標識不能為空,只能是‘N’或者‘Y’");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000006";
                                            StationReturn.MessagePara.Add("RETURN_FLAG");
                                            return;
                                        }
                                        else
                                        {
                                            newRoute.Detail[i].RETURN_FLAG = newRoute.Detail[i].RETURN_FLAG.Trim();
                                            if (newRoute.Detail[i].RETURN_FLAG.Trim() != "N")
                                            {
                                                if (newRoute.Detail[i].RETURN_FLAG.Trim() != "Y")
                                                {
                                                    //throw new Exception("工站是否可進維修標識只能是‘N’或者‘Y’");
                                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                                    StationReturn.MessageCode = "MES00000020";
                                                    StationReturn.MessagePara.Add("RETURN_FLAG");
                                                    StationReturn.MessagePara.Add("N/Y");
                                                    return;
                                                }
                                            }
                                        }
                                        olddetailid = newRoute.Detail[i].ID;
                                        newRoute.Detail[i].ROUTE_ID = newRoute.MainMessage.ID;
                                        newRoute.Detail[i].ID = TC_ROUTE_DETAIL.GetNewID(buName, sfcdb);
                                        DetailIdList.Add(newRoute.Detail[i].ID);
                                        SEQList.Add(newRoute.Detail[i].SEQ_NO);
                                        result = TC_ROUTE_DETAIL.Add(newRoute.Detail[i].getDetailC_ROUTE_DetailObject(), sfcdb);
                                        if (result > 0)
                                        {
                                            #region 循環路由Detail，修改其他工站引用此工站信息中的ID成正式ID
                                            for (int j = 0; j < newRoute.Detail.Count; j++)
                                            {
                                                if (newRoute.Detail[j] != null && newRoute.Detail[j].RETURN_FLAG == "Y")
                                                {
                                                    if (newRoute.Detail[j].ReturnItems != null && newRoute.Detail[j].ReturnItems.Count > 0)
                                                    {
                                                        for (int m = 0; m < newRoute.Detail[j].ReturnItems.Count; m++)
                                                        {
                                                            if (newRoute.Detail[j].ReturnItems[m] != null && newRoute.Detail[j].ReturnItems[m].ID != null)
                                                            {
                                                                if (newRoute.Detail[j].ReturnItems[m].ID.Trim() == olddetailid)
                                                                {
                                                                    newRoute.Detail[j].ReturnItems[m].ID = newRoute.Detail[i].ID;
                                                                }//end if
                                                            }//end if
                                                        }//end for
                                                    }//end if
                                                }//end if
                                                if (newRoute.Detail[j].DirectLinkStations != null && newRoute.Detail[j].DirectLinkStations.Count > 0)
                                                {
                                                    for (int n = 0; n < newRoute.Detail[j].DirectLinkStations.Count; n++)
                                                    {
                                                        if (newRoute.Detail[j].DirectLinkStations[n] != null && newRoute.Detail[j].DirectLinkStations[n].ID != null)
                                                        {
                                                            if (newRoute.Detail[j].DirectLinkStations[n].ID.Trim() == olddetailid)
                                                            {
                                                                newRoute.Detail[j].DirectLinkStations[n].ID = newRoute.Detail[i].ID;
                                                            }//end if
                                                        }//end if
                                                    }//end for
                                                }//end if
                                                if (newRoute.Detail[j].NextStation != null && newRoute.Detail[j].NextStation.ID != null)
                                                {
                                                    if (newRoute.Detail[j].NextStation.ID.Trim() == olddetailid)
                                                    {
                                                        newRoute.Detail[j].NextStation.ID = newRoute.Detail[i].ID;
                                                    }//end if
                                                }//rnd if
                                            }//end for
                                            #endregion 循環路由Detail，修改其他工站引用此工站信息中的ID成正式ID end
                                        }//end if
                                        else
                                        {
                                            // throw new Exception("添加c_route_detail 失敗！");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000021";
                                            StationReturn.MessagePara.Add("C_ROUTE_DETAIL");
                                            return;
                                        }
                                    }
                                }//end for

                                for (int i = 0; i < newRoute.Detail.Count(); i++)
                                {
                                    #region 循環路由，添加每個工站的return,directlink信息
                                    if (newRoute.Detail[i] != null && newRoute.Detail[i].RETURN_FLAG == "Y")
                                    {
                                        if (newRoute.Detail[i].ReturnItems != null && newRoute.Detail[i].ReturnItems.Count > 0)
                                        {
                                            for (int j = 0; j < newRoute.Detail[i].ReturnItems.Count; j++)
                                            {
                                                if (newRoute.Detail[i].ReturnItems[j] != null)
                                                {
                                                    if (newRoute.Detail[i].ReturnItems[j].ID == null || newRoute.Detail[i].ReturnItems[j].ID.Trim().Length <= 0)
                                                    {
                                                        // throw new Exception("工站維修返回工站ID不能為空！");
                                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                                        StationReturn.MessageCode = "MES00000006";
                                                        StationReturn.MessagePara.Add("ReturnItems_ID");
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        if (!DetailIdList.Contains(newRoute.Detail[i].ReturnItems[j].ID))
                                                        {
                                                            // throw new Exception("工站維修返回工站ID不存在！");
                                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                                            StationReturn.MessageCode = "MES00000007";
                                                            StationReturn.MessagePara.Add("ReturnItems_ID");
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            C_ROUTE_DETAIL_RETURN newc_route_detail_return = new C_ROUTE_DETAIL_RETURN();
                                                            newc_route_detail_return.ID = TC_ROUTE_DETAIL_RETURN.GetNewID(buName, sfcdb);
                                                            newc_route_detail_return.ROUTE_DETAIL_ID = newRoute.Detail[i].ID;
                                                            newc_route_detail_return.RETURN_ROUTE_DETAIL_ID = newRoute.Detail[i].ReturnItems[j].ID;
                                                            result = TC_ROUTE_DETAIL_RETURN.Add(newc_route_detail_return, sfcdb);
                                                            if (result <= 0)
                                                            {
                                                                //throw new Exception("添加c_route_detail_return 失敗！");
                                                                StationReturn.Status = StationReturnStatusValue.Fail;
                                                                StationReturn.MessageCode = "MES00000021";
                                                                StationReturn.MessagePara.Add("C_ROUTE_DETAIL_RETURN");
                                                                return;
                                                            }
                                                        }//end else
                                                    }
                                                }//end if
                                            }//end for
                                        }//end if
                                    }//end if
                                    if (newRoute.Detail[i] != null && newRoute.Detail[i].DirectLinkStations != null && newRoute.Detail[i].DirectLinkStations.Count > 0)
                                    {
                                        for (int n = 0; n < newRoute.Detail[i].DirectLinkStations.Count; n++)
                                        {
                                            if (newRoute.Detail[i].DirectLinkStations[n] != null)
                                            {
                                                if (newRoute.Detail[i].DirectLinkStations[n].ID == null || newRoute.Detail[i].DirectLinkStations[n].ID.Trim().Length <= 0)
                                                {
                                                    // throw new Exception("工站跳站的ID不能為空！");
                                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                                    StationReturn.MessageCode = "MES00000006";
                                                    StationReturn.MessagePara.Add("DirectLinkStations_ID");
                                                    return;
                                                }
                                                else
                                                {
                                                    if (!DetailIdList.Contains(newRoute.Detail[i].DirectLinkStations[n].ID))
                                                    {
                                                        //throw new Exception("工站跳站的ID不存在！");
                                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                                        StationReturn.MessageCode = "MES00000007";
                                                        StationReturn.MessagePara.Add("DirectLinkStations_ID");
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        C_ROUTE_DETAIL_DIRECTLINK newc_route_detail_directlink = new C_ROUTE_DETAIL_DIRECTLINK();
                                                        newc_route_detail_directlink.ID = TC_ROUTE_DETAIL_DIRECTLINK.GetNewID(buName, sfcdb);
                                                        newc_route_detail_directlink.C_ROUTE_DETAIL_ID = newRoute.Detail[i].ID;
                                                        newc_route_detail_directlink.DIRECTLINK_ROUTE_DETAIL_ID = newRoute.Detail[i].DirectLinkStations[n].ID;
                                                        result = TC_ROUTE_DETAIL_DIRECTLINK.Add(newc_route_detail_directlink, sfcdb);
                                                        if (result <= 0)
                                                        {
                                                            // throw new Exception("添加c_route_detail_directlink 失敗！");
                                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                                            StationReturn.MessageCode = "MES00000021";
                                                            StationReturn.MessagePara.Add("C_ROUTE_DETAIL_DIRECTLINK");
                                                            return;
                                                        }
                                                    }//end else
                                                }
                                            }//end if
                                        }//end for
                                    }//end if
                                    #endregion 循環路由，添加每個工站的return,directlink信息
                                }//end for
                                #endregion 循環添加Detail end
                            }
                        }//end if
                        else
                        {
                            //throw new Exception("add to c_route FAIL");
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000021";
                            StationReturn.MessagePara.Add("C_ROUTE");
                            return;
                        }//end else
                    }//end try
                    catch (Exception ex)
                    {
                        // throw new Exception(ex.Message);
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000037";
                        StationReturn.MessagePara.Add(ex.Message);
                        return;
                    }//end catch
                }//end else
            }//end else
        }
        private void DeleteRoute(Route deleteRoute, OleExec sfcdb, MESStationReturn StationReturn)
        {
            Route getRoute = deleteRoute;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
            T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_SKU_ROUTE TR_SKU_ROUTE = new T_R_SKU_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);//漏掉了R_SKU_ROUTE表沒刪除，現在加上  Add By ZHB 2020年9月18日14:22:58
            T_C_ROUTE_DETAIL TC_ROUTE_DETAIL = new T_C_ROUTE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_ROUTE_DETAIL_DIRECTLINK TC_ROUTE_DETAIL_DIRECTLINK = new T_C_ROUTE_DETAIL_DIRECTLINK(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_ROUTE_DETAIL_RETURN TC_ROUTE_DETAIL_RETURN = new T_C_ROUTE_DETAIL_RETURN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            int result = 0;
            if (getRoute.MainMessage == null)
            {
                //throw new Exception("路由不存在"); 
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000007";
                StationReturn.MessagePara.Add("ROUTE");
                return;
            }
            else
            {
                try
                {
                    bool bWO = sfcdb.ORM.Queryable<R_WO_BASE>().Any(r => r.ROUTE_ID == getRoute.MainMessage.ID && r.CLOSED_FLAG == "0");
                    bool bSN = sfcdb.ORM.Queryable<R_SN>().Any(r => r.ROUTE_ID == getRoute.MainMessage.ID && r.VALID_FLAG == "1" && (r.COMPLETED_FLAG != "1" || r.SHIPPED_FLAG != "1"));
                    if (bWO || bSN)
                    {
                        throw new Exception($@"{getRoute.MainMessage.ID} already used.");
                    }
                    #region 刪除動作                   
                    if (getRoute.Detail.Count > 0)
                    {
                        for (int i = 0; i < getRoute.Detail.Count; i++)
                        {
                            if (getRoute.Detail[i].DirectLinkStations.Count > 0)
                            {
                                result = TC_ROUTE_DETAIL_DIRECTLINK.DeleteByDetailId(getRoute.Detail[i].ID, sfcdb);
                                if (result < 0)
                                {
                                    //throw new Exception("刪除c_route_detail_directlink失敗！");
                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                    StationReturn.MessageCode = "MES00000023";
                                    StationReturn.MessagePara.Add("C_ROUTE_DETAIL_DIRECTLINK");
                                    return;
                                }//end if                               
                            }//end if
                            if (getRoute.Detail[i].RETURN_FLAG == "Y")
                            {
                                if (getRoute.Detail[i].ReturnItems.Count > 0)
                                {
                                    result = TC_ROUTE_DETAIL_RETURN.DeleteByDetailId(getRoute.Detail[i].ID, sfcdb);
                                    if (result < 0)
                                    {
                                        // throw new Exception("刪除c_route_detail_return失敗！");
                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                        StationReturn.MessageCode = "MES00000023";
                                        StationReturn.MessagePara.Add("C_ROUTE_DETAIL_RETURN");
                                        return;
                                    }//end if
                                }//end if
                            }//end if
                        }//end for
                        result = TC_ROUTE_DETAIL.DeleteByRouteId(getRoute.MainMessage.ID, sfcdb);
                        if (result < 0)
                        {
                            // throw new Exception("刪除c_route_detail失敗！");
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000023";
                            StationReturn.MessagePara.Add("C_ROUTE_DETAIL");
                            return;
                        }//end if
                    }//end if
                    result = TC_ROUTE.DeleteById(getRoute.MainMessage.ID, sfcdb);
                    if (result < 0)
                    {
                        //throw new Exception("刪除c_route失敗！");
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000023";
                        StationReturn.MessagePara.Add("C_ROUTE");
                        return;
                    }//end if
                    //漏掉了R_SKU_ROUTE表沒刪除，現在加上  Add By ZHB 2020年9月18日14:22:58
                    var skuRoute = sfcdb.ORM.Queryable<R_SKU_ROUTE>().Where(t => t.ROUTE_ID == getRoute.MainMessage.ID).First();
                    if (skuRoute != null)
                    {
                        TR_SKU_ROUTE.DeleteMapping(skuRoute.ID, sfcdb);
                    }
                    #endregion 刪除動作 end                  
                }//end try
                catch (Exception ex)
                {
                    // throw new Exception(ex.Message);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(ex.Message);
                    return;
                }//end catch
            }//end else
        }
        private void UpdateRouteById_DeleteOldAndAddNew(Route newRoute, OleExec sfcdb, MESStationReturn StationReturn)
        {
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
            if (newRoute == null)
            {
                //throw new Exception("新路由不能為空");
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("NEWROUTE");
                return;
            }
            if (newRoute.MainMessage == null || newRoute.MainMessage.ID == null || newRoute.MainMessage.ID.Trim().Length <= 0)
            {
                // throw new Exception("未指定要修改的路由");
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("MainMessage_ID");
                return;
            }
            newRoute.MainMessage.ID = newRoute.MainMessage.ID.Trim();
            Route oldRoute = SelectRouteByRouteId(newRoute.MainMessage.ID,sfcdb);
            if (oldRoute.MainMessage == null)
            {
                // throw new Exception("路由不存在");
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000007";
                StationReturn.MessagePara.Add("C_ROUTE ID:" + newRoute.MainMessage.ID);
                return;
            }
            try
            {
                DeleteRoute(oldRoute, sfcdb, StationReturn);
                if (StationReturn.Status == StationReturnStatusValue.Pass)
                {
                    AddNewRoute(newRoute, sfcdb, StationReturn);
                }
            }
            catch (Exception ex)
            {
                // throw new Exception(ex.Message);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
                return;
            }
        }
        private void UpdateRouteById(Route newRoute, OleExec sfcdb, MESStationReturn StationReturn)
        {
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000001";
            string strNULL = "";
            newRoute.MainMessage.EDIT_EMP = LoginUser.EMP_NO;
            newRoute.MainMessage.EDIT_TIME = GetDBDateTime();
            List<string> DetailIdList = new List<string>();
            List<double?> SEQList = new List<double?>();
            T_C_STATION TC_STATION = new T_C_STATION(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<C_STATION_DETAIL> checkStationNameList = new List<C_STATION_DETAIL>();
            T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_ROUTE_DETAIL TC_ROUTE_DETAIL = new T_C_ROUTE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_ROUTE_DETAIL_RETURN TC_ROUTE_DETAIL_RETURN = new T_C_ROUTE_DETAIL_RETURN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_ROUTE_DETAIL_DIRECTLINK TC_ROUTE_DETAIL_DIRECTLINK = new T_C_ROUTE_DETAIL_DIRECTLINK(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            C_ROUTE_DETAIL checkCRouteDetail = new C_ROUTE_DETAIL();
            T_R_SKU_ROUTE TR_SKU_ROUTE = new T_R_SKU_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<C_SKU> getSKUList = new List<C_SKU>();
            string olddetailid = "";
            int result = 0;
            bool isDeleteItem = false;
            bool isAdditem = false;
            if (newRoute == null)
            {
                //throw new Exception("路由不能NULL");
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("NEWROUTE");
                return;
            }
            if (newRoute.MainMessage == null || newRoute.MainMessage.ROUTE_NAME == null || newRoute.MainMessage.ROUTE_NAME.Trim().Length <= 0)
            {
                //throw new Exception("路由名不能為空");
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("ROUTE_NAME");
                return;
            }
            else
            {
                if (newRoute.MainMessage.ID == null || newRoute.MainMessage.ID.Trim().Length < 0)
                {
                    // throw new Exception("路由ID不能為空");
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("MainMessage_ID");
                    return;
                }
                else
                {
                    newRoute.MainMessage.ID = newRoute.MainMessage.ID.Trim();
                    Route oldRoute = SelectRouteByRouteId(newRoute.MainMessage.ID,sfcdb);
                    if (oldRoute.MainMessage == null)
                    {
                        // throw new Exception("路由不存在！");
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000007";
                        StationReturn.MessagePara.Add("C_ROUTE ID:" + newRoute.MainMessage.ID);
                        return;
                    }
                    newRoute.MainMessage.ROUTE_NAME = newRoute.MainMessage.ROUTE_NAME.Trim();
                    if (newRoute.MainMessage.DEFAULT_SKUNO == null)
                    {
                        newRoute.MainMessage.DEFAULT_SKUNO = strNULL;
                    }
                    else
                    {
                        newRoute.MainMessage.DEFAULT_SKUNO = newRoute.MainMessage.DEFAULT_SKUNO.Trim();
                        //查看路由是否被其他機種引用，如果已經被其他機種引用，則不能設置當前機種
                        if (newRoute.MainMessage.DEFAULT_SKUNO.Length > 0)
                        {
                            getSKUList = TR_SKU_ROUTE.GetSkuListByMappingRouteID(newRoute.MainMessage.ID, sfcdb);
                            if (getSKUList != null && getSKUList.Count > 0)
                            {
                                for (int i = 0; i < getSKUList.Count; i++)
                                {
                                    if (getSKUList[i].SKUNO!=null&&getSKUList[i].SKUNO.Trim() != newRoute.MainMessage.DEFAULT_SKUNO)
                                    {
                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                        StationReturn.MessageCode = "MES00000031";
                                        StationReturn.MessagePara.Add( newRoute.MainMessage.ROUTE_NAME);
                                        StationReturn.MessagePara.Add(getSKUList[i].SKUNO);
                                        StationReturn.MessagePara.Add(newRoute.MainMessage.DEFAULT_SKUNO);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    if (newRoute.MainMessage.ROUTE_TYPE == null)
                    {
                        newRoute.MainMessage.ROUTE_TYPE = strNULL;
                    }
                    else
                    {
                        newRoute.MainMessage.ROUTE_TYPE = newRoute.MainMessage.ROUTE_TYPE.Trim();
                    }
                    #region 檢查c_route                    
                    if (newRoute.MainMessage.ROUTE_NAME != oldRoute.MainMessage.ROUTE_NAME.Trim())
                    {
                        //檢查新的路由名是否存在
                        Route checkRoute = new Route();
                        checkRoute = SelectRouteByRouteName(newRoute.MainMessage.ROUTE_NAME,sfcdb);
                        if (checkRoute.MainMessage != null)
                        {
                            //throw new Exception("新路由名已經存在");
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000008";
                            StationReturn.MessagePara.Add(newRoute.MainMessage.ROUTE_NAME);
                            return;
                        }
                    }//end if                   
                    #endregion 檢查檢查c_route  end                 
                    #region 檢查新的路由是否合法，id是否存在並且唯一，序號是否唯一，維修標識是否合法，工站名是否合法
                    if (newRoute.Detail != null && newRoute.Detail.Count > 0)
                    {
                        for (int j = 0; j < newRoute.Detail.Count; j++)
                        {
                            if (newRoute.Detail[j] != null)
                            {
                                newRoute.Detail[j].ROUTE_ID = newRoute.MainMessage.ID;
                                //detail id
                                if (newRoute.Detail[j].ID != null && newRoute.Detail[j].ID.Trim().Length > 0)
                                {
                                    newRoute.Detail[j].ID = newRoute.Detail[j].ID.Trim();
                                    if (DetailIdList.Contains(newRoute.Detail[j].ID))
                                    {
                                        // throw new Exception("更新工站ID不能相同");
                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                        StationReturn.MessageCode = "MES00000024";
                                        StationReturn.MessagePara.Add("Detail_ID");
                                        return;
                                    }
                                    else
                                    {
                                        DetailIdList.Add(newRoute.Detail[j].ID);
                                    }
                                }
                                else
                                {
                                    // throw new Exception("更新工站ID不能為空");
                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                    StationReturn.MessageCode = "MES00000006";
                                    StationReturn.MessagePara.Add("Detail_ID");
                                    return;
                                }
                                //detail 序號
                                if (newRoute.Detail[j].SEQ_NO != null)
                                {
                                    if (SEQList.Contains(newRoute.Detail[j].SEQ_NO))
                                    {
                                        // throw new Exception("更新工站序號不能相同");
                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                        StationReturn.MessageCode = "MES00000024";
                                        StationReturn.MessagePara.Add("SEQ_NO");
                                        return;
                                    }
                                    else
                                    {
                                        SEQList.Add(newRoute.Detail[j].SEQ_NO);
                                    }
                                }
                                else
                                {
                                    // throw new Exception("更新工站序號不能為空");
                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                    StationReturn.MessageCode = "MES00000006";
                                    StationReturn.MessagePara.Add("SEQ_NO");
                                    return;
                                }
                                // detail returnflag
                                if (newRoute.Detail[j].RETURN_FLAG != null)
                                {
                                    if (newRoute.Detail[j].RETURN_FLAG != "Y")
                                    {
                                        if (newRoute.Detail[j].RETURN_FLAG != "N")
                                        {
                                            //throw new Exception("是否進維修標識只能是‘N’或者‘Y’");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000020";
                                            StationReturn.MessagePara.Add("RETURN_FLAG");
                                            StationReturn.MessagePara.Add("N/Y");
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    //  throw new Exception("工站是否可進維修標識不能為空,只能是‘N’或者‘Y’");
                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                    StationReturn.MessageCode = "MES00000006";
                                    StationReturn.MessagePara.Add("RETURN_FLAG");
                                    return;
                                }
                                //detail 名稱的合法性
                                if (newRoute.Detail[j].STATION_NAME != null && newRoute.Detail[j].STATION_NAME.Trim().Length > 0)
                                {
                                    newRoute.Detail[j].STATION_NAME = newRoute.Detail[j].STATION_NAME.Trim();
                                    checkStationNameList = TC_STATION.GetDataByColumn("station_name", newRoute.Detail[j].STATION_NAME, sfcdb);
                                    if (checkStationNameList == null || checkStationNameList.Count <= 0)
                                    {
                                        //throw new Exception("工站名不存在");
                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                        StationReturn.MessageCode = "MES00000007";
                                        StationReturn.MessagePara.Add(newRoute.Detail[j].STATION_NAME);
                                        return;
                                    }
                                }
                                else
                                {
                                    // throw new Exception("工站名不能為空！");
                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                    StationReturn.MessageCode = "MES00000006";
                                    StationReturn.MessagePara.Add("STATION_NAME");
                                    return;
                                }
                               // station type, 
                                if (newRoute.Detail[j].STATION_TYPE != null && newRoute.Detail[j].STATION_TYPE.Trim().Length > 0)
                                {
                                    newRoute.Detail[j].STATION_TYPE = newRoute.Detail[j].STATION_TYPE.Trim();
                                    //類型判斷
                                    if (!Enum.IsDefined(typeof(STATIONTYPE), newRoute.Detail[j].STATION_TYPE))
                                    {
                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                        StationReturn.MessageCode = "MES00000020";
                                        StationReturn.MessagePara.Add("STATION_TYPE");
                                        string stationType = "";
                                        foreach (string typeString in Enum.GetNames(typeof(STATIONTYPE)))
                                        {
                                            if (stationType == "")
                                            {
                                                stationType = stationType + typeString;
                                            }
                                            else
                                            {
                                                stationType = stationType + "," + typeString;
                                            }
                                        }
                                        StationReturn.MessagePara.Add(stationType);
                                        return;
                                    }
                                }
                                else
                                {
                                    // throw new Exception("工站類型不能為空！");
                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                    StationReturn.MessageCode = "MES00000006";
                                    StationReturn.MessagePara.Add("STATION_TYPE");
                                    return;
                                }
                            }
                            else
                            {
                                //throw new Exception("更新工站不能為null");
                                StationReturn.Status = StationReturnStatusValue.Fail;
                                StationReturn.MessageCode = "MES00000006";
                                StationReturn.MessagePara.Add("Detail["+j+"]");
                                return;
                            }
                        }
                        //檢查return 和directlink是否合法
                        for (int i = 0; i < newRoute.Detail.Count; i++)
                        {
                            if (newRoute.Detail[i].RETURN_FLAG == "Y" && newRoute.Detail[i].ReturnItems != null && newRoute.Detail[i].ReturnItems.Count > 0)
                            {
                                for (int j = 0; j < newRoute.Detail[i].ReturnItems.Count; j++)
                                {
                                    if (newRoute.Detail[i].ReturnItems[j] != null)
                                    {
                                        if (newRoute.Detail[i].ReturnItems[j].ID != null && newRoute.Detail[i].ReturnItems[j].ID.Trim().Length > 0)
                                        {
                                            newRoute.Detail[i].ReturnItems[j].ID = newRoute.Detail[i].ReturnItems[j].ID.Trim();
                                            if (!DetailIdList.Contains(newRoute.Detail[i].ReturnItems[j].ID))
                                            {
                                                //throw new Exception("工站維修返回工站ID不存在！");
                                                StationReturn.Status = StationReturnStatusValue.Fail;
                                                StationReturn.MessageCode = "MES00000007";
                                                StationReturn.MessagePara.Add("ReturnItems_ID:" + newRoute.Detail[i].ReturnItems[j].ID);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            // throw new Exception("更新工站維修返回工站不能為null或者ID不能為空");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000006";
                                            StationReturn.MessagePara.Add("ReturnItems_ID");
                                            return;
                                        }
                                    }//end if
                                }//end for
                            }//end if
                            if (newRoute.Detail[i].DirectLinkStations != null && newRoute.Detail[i].DirectLinkStations.Count > 0)
                            {
                                for (int j = 0; j < newRoute.Detail[i].DirectLinkStations.Count; j++)
                                {
                                    if (newRoute.Detail[i].DirectLinkStations[j] != null )
                                    {
                                        if (newRoute.Detail[i].DirectLinkStations[j].ID != null && newRoute.Detail[i].DirectLinkStations[j].ID.Trim().Length > 0)
                                        {
                                            newRoute.Detail[i].DirectLinkStations[j].ID = newRoute.Detail[i].DirectLinkStations[j].ID.Trim();
                                            if (!DetailIdList.Contains(newRoute.Detail[i].DirectLinkStations[j].ID))
                                            {
                                                //throw new Exception("跳站工站ID不存在！");
                                                StationReturn.Status = StationReturnStatusValue.Fail;
                                                StationReturn.MessageCode = "MES00000007";
                                                StationReturn.MessagePara.Add("DirectLinkStations_ID:" + newRoute.Detail[i].ReturnItems[j].ID);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            //throw new Exception("跳站不能為null或者ID不能為空");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000006";
                                            StationReturn.MessagePara.Add("DirectLinkStations_ID");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion 檢查新的路由是否合法，id是否存在並且唯一，序號是否唯一，維修標識是否合法，工站名是否合法 end                  
                    try
                    {
                        //update c_route
                        result = TC_ROUTE.UpdateById(newRoute.MainMessage, sfcdb);
                        if (result > 0)
                        {
                            #region 刪除detail
                            if (oldRoute.Detail.Count > 0)
                            {
                                for (int i = 0; i < oldRoute.Detail.Count; i++)
                                {
                                    isDeleteItem = true;
                                    if (newRoute.Detail != null && newRoute.Detail.Count > 0)
                                    {
                                        for (int j = 0; j < newRoute.Detail.Count; j++)
                                        {
                                            if (oldRoute.Detail[i].ID.Trim() == newRoute.Detail[j].ID)
                                            {
                                                isDeleteItem = false;
                                                break;
                                            }
                                        }
                                    }
                                    if (isDeleteItem)
                                    {
                                        result = TC_ROUTE_DETAIL.DeleteById(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                        if (result > 0)
                                        {
                                            //刪除return
                                            result = TC_ROUTE_DETAIL_RETURN.GetCountByRoute_DetailId(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                            if (result > 0)
                                            {
                                                result = TC_ROUTE_DETAIL_RETURN.DeleteByDetailId(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                                if (result <= 0)
                                                {
                                                    // throw new Exception("刪除C_ROUTE_DETAIL_RETURN失敗！");
                                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                                    StationReturn.MessageCode = "MES00000023";
                                                    StationReturn.MessagePara.Add("C_ROUTE_DETAIL_RETURN");
                                                    return;
                                                }
                                            }
                                            result = TC_ROUTE_DETAIL_RETURN.GetCountByReturn_Route_Detail_Id(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                            if (result > 0)
                                            {
                                                result = TC_ROUTE_DETAIL_RETURN.DeleteByReturn_Route_DetailId(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                                if (result <= 0)
                                                {
                                                    // throw new Exception("刪除C_ROUTE_DETAIL_RETURN失敗！");
                                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                                    StationReturn.MessageCode = "MES00000023";
                                                    StationReturn.MessagePara.Add("C_ROUTE_DETAIL_RETURN");
                                                    return;
                                                }
                                            }
                                            //刪除directlink
                                            result = TC_ROUTE_DETAIL_DIRECTLINK.GetCountByDetailId(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                            if (result > 0)
                                            {
                                                result = TC_ROUTE_DETAIL_DIRECTLINK.DeleteByDetailId(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                                if (result <= 0)
                                                {
                                                    // throw new Exception("刪除C_ROUTE_DETAIL_DIRECTLINK失敗！");
                                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                                    StationReturn.MessageCode = "MES00000023";
                                                    StationReturn.MessagePara.Add("C_ROUTE_DETAIL_DIRECTLINK");
                                                    return;
                                                }
                                            }
                                            result = TC_ROUTE_DETAIL_DIRECTLINK.GetCountByDirectlinkId(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                            if (result > 0)
                                            {
                                                result = TC_ROUTE_DETAIL_DIRECTLINK.DeleteByDirectlinkId(oldRoute.Detail[i].ID.Trim(), sfcdb);
                                                if (result <= 0)
                                                {
                                                    //throw new Exception("刪除C_ROUTE_DETAIL_DIRECTLINK失敗！");
                                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                                    StationReturn.MessageCode = "MES00000023";
                                                    StationReturn.MessagePara.Add("C_ROUTE_DETAIL_DIRECTLINK");
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //throw new Exception("刪除C_ROUTE_DETAIL失敗！");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000023";
                                            StationReturn.MessagePara.Add("C_ROUTE_DETAIL");
                                            return;
                                        }
                                    }
                                }
                            }
                            #endregion 刪除detail end
                            if (newRoute.Detail != null && newRoute.Detail.Count() > 0)
                            {
                                #region add new c_route_detail
                                for (int i = 0; i < newRoute.Detail.Count; i++)
                                {
                                    checkCRouteDetail = TC_ROUTE_DETAIL.GetById(newRoute.Detail[i].ID, sfcdb);
                                    if (checkCRouteDetail == null)
                                    {
                                        olddetailid = newRoute.Detail[i].ID;
                                        newRoute.Detail[i].ID = TC_ROUTE_DETAIL.GetNewID(BU, sfcdb);
                                        result = TC_ROUTE_DETAIL.Add(newRoute.Detail[i].getDetailC_ROUTE_DetailObject(), sfcdb);
                                        if (result <= 0)
                                        {
                                            //throw new Exception("添加C_ROUTE_DETAIL失敗！");
                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                            StationReturn.MessageCode = "MES00000021";
                                            StationReturn.MessagePara.Add("C_ROUTE_DETAIL");
                                            return;
                                        }
                                        else
                                        {
                                            #region 循環路由Detail，修改其他工站引用此工站信息中的ID成正式ID
                                            for (int j = 0; j < newRoute.Detail.Count; j++)
                                            {
                                                if (newRoute.Detail[j].RETURN_FLAG == "Y")
                                                {
                                                    if (newRoute.Detail[j].ReturnItems != null && newRoute.Detail[j].ReturnItems.Count > 0)
                                                    {
                                                        for (int m = 0; m < newRoute.Detail[j].ReturnItems.Count; m++)
                                                        {
                                                            if (newRoute.Detail[j].ReturnItems[m]!=null&&newRoute.Detail[j].ReturnItems[m].ID.Trim() == olddetailid)
                                                            {
                                                                newRoute.Detail[j].ReturnItems[m].ID = newRoute.Detail[i].ID;
                                                            }//end if
                                                        }//end for
                                                    }//end if
                                                }//end if
                                                if (newRoute.Detail[j].DirectLinkStations != null && newRoute.Detail[j].DirectLinkStations.Count > 0)
                                                {
                                                    for (int n = 0; n < newRoute.Detail[j].DirectLinkStations.Count; n++)
                                                    {
                                                        if (newRoute.Detail[j].DirectLinkStations[n]!=null&&newRoute.Detail[j].DirectLinkStations[n].ID.Trim() == olddetailid)
                                                        {
                                                            newRoute.Detail[j].DirectLinkStations[n].ID = newRoute.Detail[i].ID;
                                                        }//end if
                                                    }//end for
                                                }//end if                                              
                                            }//end for
                                            #endregion 循環路由Detail，修改其他工站引用此工站信息中的ID成正式ID end
                                        }
                                    }
                                }
                                #endregion add new c_route_detail end
                                #region update c_route_detail
                                if (oldRoute.Detail.Count > 0)
                                {
                                    for (int j = 0; j < newRoute.Detail.Count; j++)
                                    {
                                        for (int i = 0; i < oldRoute.Detail.Count; i++)
                                        {
                                            if (newRoute.Detail[j].ID == oldRoute.Detail[i].ID)
                                            {
                                                //update c_route_detail
                                                if (newRoute.Detail[j].SEQ_NO != oldRoute.Detail[i].SEQ_NO ||
                                                   newRoute.Detail[j].STATION_NAME != oldRoute.Detail[i].STATION_NAME ||
                                                   newRoute.Detail[j].STATION_TYPE != oldRoute.Detail[i].STATION_TYPE ||
                                                   newRoute.Detail[j].RETURN_FLAG != oldRoute.Detail[i].RETURN_FLAG)
                                                {
                                                    result = TC_ROUTE_DETAIL.UpdateById(newRoute.Detail[j].getDetailC_ROUTE_DetailObject(), sfcdb);
                                                    if (result <= 0)
                                                    {
                                                        // throw new Exception("更新C_ROUTE_DETAIL失敗！");
                                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                                        StationReturn.MessageCode = "MES00000025";
                                                        StationReturn.MessagePara.Add("C_ROUTE_DETAIL");
                                                        return;
                                                    }
                                                }
                                                #region update c_route_detail_return
                                                if (newRoute.Detail[j].RETURN_FLAG == "Y")
                                                {
                                                    if (oldRoute.Detail[i].RETURN_FLAG == "N")
                                                    {
                                                        if (newRoute.Detail[j].ReturnItems != null && newRoute.Detail[j].ReturnItems.Count > 0)
                                                        {
                                                            for (int n = 0; n < newRoute.Detail[j].ReturnItems.Count; n++)
                                                            {
                                                                if (newRoute.Detail[j].ReturnItems[n] != null)
                                                                {
                                                                    C_ROUTE_DETAIL_RETURN addnewreturnitem = new C_ROUTE_DETAIL_RETURN();
                                                                    addnewreturnitem.ID = TC_ROUTE_DETAIL_RETURN.GetNewID(BU, sfcdb);
                                                                    addnewreturnitem.ROUTE_DETAIL_ID = newRoute.Detail[j].ID;
                                                                    addnewreturnitem.RETURN_ROUTE_DETAIL_ID = newRoute.Detail[j].ReturnItems[n].ID;
                                                                    result = TC_ROUTE_DETAIL_RETURN.Add(addnewreturnitem, sfcdb);
                                                                    if (result <= 0)
                                                                    {
                                                                        // throw new Exception("添加C_ROUTE_DETAIL_RETURN失敗！");
                                                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                                                        StationReturn.MessageCode = "MES00000021";
                                                                        StationReturn.MessagePara.Add("C_ROUTE_DETAIL_RETURN");
                                                                        return;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //添加新的return
                                                        if (newRoute.Detail[j].ReturnItems != null && newRoute.Detail[j].ReturnItems.Count > 0)
                                                        {
                                                            for (int n = 0; n < newRoute.Detail[j].ReturnItems.Count; n++)
                                                            {
                                                                isAdditem = true;
                                                                if (newRoute.Detail[j].ReturnItems[n] != null)
                                                                {
                                                                    if (oldRoute.Detail[i].ReturnItems != null && oldRoute.Detail[i].ReturnItems.Count > 0)
                                                                    {
                                                                        for (int m = 0; m < oldRoute.Detail[i].ReturnItems.Count; m++)
                                                                        {
                                                                            if (newRoute.Detail[j].ReturnItems[n].ID == oldRoute.Detail[i].ReturnItems[m].ID)
                                                                            {
                                                                                isAdditem = false;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (isAdditem)
                                                                    {
                                                                        C_ROUTE_DETAIL_RETURN addnewretirnitem = new C_ROUTE_DETAIL_RETURN();
                                                                        addnewretirnitem.ID = TC_ROUTE_DETAIL_RETURN.GetNewID(BU, sfcdb);
                                                                        addnewretirnitem.ROUTE_DETAIL_ID = newRoute.Detail[j].ID;
                                                                        addnewretirnitem.RETURN_ROUTE_DETAIL_ID = newRoute.Detail[j].ReturnItems[n].ID;
                                                                        result = TC_ROUTE_DETAIL_RETURN.Add(addnewretirnitem, sfcdb);
                                                                        if (result <= 0)
                                                                        {
                                                                            // throw new Exception("添加C_ROUTE_DETAIL_RETURN失敗！");
                                                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                                                            StationReturn.MessageCode = "MES00000021";
                                                                            StationReturn.MessagePara.Add("C_ROUTE_DETAIL_RETURN");
                                                                            return;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        //刪除不要的return
                                                        if (oldRoute.Detail[i].ReturnItems != null && oldRoute.Detail[i].ReturnItems.Count > 0)
                                                        {
                                                            List<C_ROUTE_DETAIL_RETURN> oldRouteReturnItems = new List<C_ROUTE_DETAIL_RETURN>();
                                                            oldRouteReturnItems = TC_ROUTE_DETAIL_RETURN.GetByRoute_DetailId(oldRoute.Detail[i].ID, sfcdb);
                                                            if (oldRouteReturnItems != null && oldRouteReturnItems.Count > 0)
                                                            {
                                                                for (int n = 0; n < oldRouteReturnItems.Count; n++)
                                                                {
                                                                    isDeleteItem = true;
                                                                    if (newRoute.Detail[j].ReturnItems != null && newRoute.Detail[j].ReturnItems.Count > 0)
                                                                    {
                                                                        for (int m = 0; m < newRoute.Detail[j].ReturnItems.Count; m++)
                                                                        {
                                                                            if (newRoute.Detail[j].ReturnItems[m]!=null&&newRoute.Detail[j].ReturnItems[m].ID == oldRouteReturnItems[n].RETURN_ROUTE_DETAIL_ID)
                                                                            {
                                                                                isDeleteItem = false;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (isDeleteItem)
                                                                    {
                                                                        result = TC_ROUTE_DETAIL_RETURN.DeleteByDetailIdAndReturnId(oldRouteReturnItems[n].ROUTE_DETAIL_ID, oldRouteReturnItems[n].RETURN_ROUTE_DETAIL_ID, sfcdb);
                                                                        if (result <= 0)
                                                                        {
                                                                            // throw new Exception("刪除C_ROUTE_DETAIL_RETURN失敗！");
                                                                            StationReturn.Status = StationReturnStatusValue.Fail;
                                                                            StationReturn.MessageCode = "MES00000023";
                                                                            StationReturn.MessagePara.Add("C_ROUTE_DETAIL_RETURN");
                                                                            return;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (oldRoute.Detail[i].RETURN_FLAG == "Y")
                                                    {
                                                        result = TC_ROUTE_DETAIL_RETURN.GetCountByRoute_DetailId(oldRoute.Detail[i].ID, sfcdb);
                                                        if (result > 0)
                                                        {
                                                            result = TC_ROUTE_DETAIL_RETURN.DeleteByDetailId(oldRoute.Detail[i].ID, sfcdb);
                                                            if (result <= 0)
                                                            {
                                                                //throw new Exception("刪除C_ROUTE_DETAIL_RETURN失敗！");
                                                                StationReturn.Status = StationReturnStatusValue.Fail;
                                                                StationReturn.MessageCode = "MES00000023";
                                                                StationReturn.MessagePara.Add("C_ROUTE_DETAIL_RETURN");
                                                                return;
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion update c_route_detail_return end

                                                #region update c_route_detail_directlink
                                                if (newRoute.Detail[j].DirectLinkStations != null && newRoute.Detail[j].DirectLinkStations.Count > 0)
                                                {
                                                    if (oldRoute.Detail[i].DirectLinkStations.Count > 0)
                                                    {
                                                        //添加新的
                                                        for (int n = 0; n < newRoute.Detail[j].DirectLinkStations.Count; n++)
                                                        {
                                                            isAdditem = true;
                                                            if (newRoute.Detail[j].DirectLinkStations[n] != null)
                                                            {
                                                                for (int m = 0; m < oldRoute.Detail[i].DirectLinkStations.Count; m++)
                                                                {
                                                                    if (newRoute.Detail[j].DirectLinkStations[n].ID == oldRoute.Detail[i].DirectLinkStations[m].ID)
                                                                    {
                                                                        isAdditem = false;
                                                                        break;
                                                                    }
                                                                }
                                                                if (isAdditem)
                                                                {
                                                                    C_ROUTE_DETAIL_DIRECTLINK addnewdirectlinkitem = new C_ROUTE_DETAIL_DIRECTLINK();
                                                                    addnewdirectlinkitem.ID = TC_ROUTE_DETAIL_DIRECTLINK.GetNewID(BU, sfcdb);
                                                                    addnewdirectlinkitem.C_ROUTE_DETAIL_ID = newRoute.Detail[j].ID;
                                                                    addnewdirectlinkitem.DIRECTLINK_ROUTE_DETAIL_ID = newRoute.Detail[j].DirectLinkStations[n].ID;
                                                                    result = TC_ROUTE_DETAIL_DIRECTLINK.Add(addnewdirectlinkitem, sfcdb);
                                                                    if (result <= 0)
                                                                    {
                                                                        //throw new Exception("添加C_ROUTE_DETAIL_DIRECTLINK失敗！");
                                                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                                                        StationReturn.MessageCode = "MES00000021";
                                                                        StationReturn.MessagePara.Add("C_ROUTE_DETAIL_DIRECTLINK");
                                                                        return;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        //刪除不要的
                                                        List<C_ROUTE_DETAIL_DIRECTLINK> oldRouteDirectItems = new List<C_ROUTE_DETAIL_DIRECTLINK>();
                                                        oldRouteDirectItems = TC_ROUTE_DETAIL_DIRECTLINK.GetByDetailId(oldRoute.Detail[i].ID, sfcdb);
                                                        if (oldRouteDirectItems != null && oldRouteDirectItems.Count > 0)
                                                        {
                                                            for (int n = 0; n < oldRouteDirectItems.Count; n++)
                                                            {
                                                                isDeleteItem = true;
                                                                if (newRoute.Detail[j].DirectLinkStations != null && newRoute.Detail[j].DirectLinkStations.Count > 0)
                                                                {
                                                                    for (int m = 0; m < newRoute.Detail[j].DirectLinkStations.Count; m++)
                                                                    {
                                                                        if (newRoute.Detail[j].DirectLinkStations[m]!=null&&newRoute.Detail[j].DirectLinkStations[m].ID == oldRouteDirectItems[n].DIRECTLINK_ROUTE_DETAIL_ID)
                                                                        {
                                                                            isDeleteItem = false;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                if (isDeleteItem)
                                                                {
                                                                    result = TC_ROUTE_DETAIL_DIRECTLINK.DeleteByDetailIdAndDirectlinkId(oldRouteDirectItems[n].C_ROUTE_DETAIL_ID, oldRouteDirectItems[n].DIRECTLINK_ROUTE_DETAIL_ID, sfcdb);
                                                                    if (result <= 0)
                                                                    {
                                                                        //throw new Exception("刪除C_ROUTE_DETAIL_DIRECTLINK失敗！");
                                                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                                                        StationReturn.MessageCode = "MES00000023";
                                                                        StationReturn.MessagePara.Add("C_ROUTE_DETAIL_DIRECTLINK");
                                                                        return;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int n = 0; n < newRoute.Detail[j].DirectLinkStations.Count; n++)
                                                        {
                                                            if (newRoute.Detail[j].DirectLinkStations[n]!=null)
                                                            {
                                                                C_ROUTE_DETAIL_DIRECTLINK addnewdirectlinkitem = new C_ROUTE_DETAIL_DIRECTLINK();
                                                                addnewdirectlinkitem.ID = TC_ROUTE_DETAIL_DIRECTLINK.GetNewID(BU, sfcdb);
                                                                addnewdirectlinkitem.C_ROUTE_DETAIL_ID = newRoute.Detail[j].ID;
                                                                addnewdirectlinkitem.DIRECTLINK_ROUTE_DETAIL_ID = newRoute.Detail[j].DirectLinkStations[n].ID;
                                                                result = TC_ROUTE_DETAIL_DIRECTLINK.Add(addnewdirectlinkitem, sfcdb);
                                                                if (result <= 0)
                                                                {
                                                                    // throw new Exception("添加C_ROUTE_DETAIL_DIRECTLINK失敗！");
                                                                    StationReturn.Status = StationReturnStatusValue.Fail;
                                                                    StationReturn.MessageCode = "MES00000021";
                                                                    StationReturn.MessagePara.Add("C_ROUTE_DETAIL_DIRECTLINK");
                                                                    return;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (oldRoute.Detail[i].DirectLinkStations.Count > 0)
                                                    {
                                                        result = TC_ROUTE_DETAIL_DIRECTLINK.GetCountByDetailId(oldRoute.Detail[i].ID, sfcdb);
                                                        if (result > 0)
                                                        {
                                                            result = TC_ROUTE_DETAIL_DIRECTLINK.DeleteByDetailId(oldRoute.Detail[i].ID, sfcdb);
                                                            if (result <= 0)
                                                            {
                                                                //throw new Exception("刪除C_ROUTE_DETAIL_DIRECTLINK失敗！");
                                                                StationReturn.Status = StationReturnStatusValue.Fail;
                                                                StationReturn.MessageCode = "MES00000023";
                                                                StationReturn.MessagePara.Add("C_ROUTE_DETAIL_DIRECTLINK");
                                                                return;
                                                            }
                                                        }
                                                    }
                                                }
                                                #endregion update c_route_detail_directlink end
                                            }
                                        }
                                    }
                                }
                                #endregion update c_route_detail end
                            }
                        }
                        else
                        {
                            // throw new Exception("更新C_ROUTE失敗！");
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000025";
                            StationReturn.MessagePara.Add("C_ROUTE");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        //throw ex;
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000037";
                        StationReturn.MessagePara.Add(ex.Message);
                        return;
                    }
                }
            }
        }
        /// <summary>
        ///将JSON字符串转换成对象 Transform The JSON String Into an Instance
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="str">JSON String</param>
        /// <returns></returns>
        private T ParsesJSON<T>(string str)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(ms);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通過路由名獲取路由
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRouteByRouteName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Route SelectRoute = new Route();
                string RouteName = Data["RouteName"].ToString();
                SelectRoute = SelectRouteByRouteName(RouteName,sfcdb);
                if (SelectRoute.MainMessage != null)
                {
                    StationReturn.Data = SelectRoute;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(RouteName);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        /// <summary>
        ///通過路由ID獲取路由
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRouteByRouteId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Route SelectRoute = new Route();
                string RouteId = Data["RouteId"].ToString();
                SelectRoute = SelectRouteByRouteId(RouteId,sfcdb);
                if (SelectRoute.MainMessage != null)
                {
                    StationReturn.Data = SelectRoute;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(RouteId);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        /// <summary>
        /// 添加新的路由
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AddRoute(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            Route newRoute = new Route();
            try
            {
                string strJsonNewRoute = Data["RouteJsonString"].ToString();
                newRoute = ParsesJSON<Route>(strJsonNewRoute);
            }
            catch
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000022";
                StationReturn.MessagePara.Add("RouteJsonString");
                return;
            }
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                AddNewRoute(newRoute, sfcdb, StationReturn);
                if (StationReturn.Status != StationReturnStatusValue.Pass)
                {
                    sfcdb.RollbackTrain();
                }
                else
                {
                    sfcdb.CommitTrain();
                    StationReturn.Data = SelectRouteByRouteName(newRoute.MainMessage.ROUTE_NAME,sfcdb);
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {      
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        /// <summary>
        ///更新路由，刪除原來的路由，添加新的路由
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateRoute_DeleteOldAndAddNew(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            Route newRoute = new Route();
            try
            {
                string strJsonNewRoute = Data["RouteJsonString"].ToString();
                newRoute = ParsesJSON<Route>(strJsonNewRoute);
            }
            catch
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000022";
                StationReturn.MessagePara.Add("RouteJsonString");
                return;
            }
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                UpdateRouteById_DeleteOldAndAddNew(newRoute, sfcdb, StationReturn);
                if (StationReturn.Status == StationReturnStatusValue.Pass)
                {
                    sfcdb.CommitTrain();
                    StationReturn.Data = SelectRouteByRouteName(newRoute.MainMessage.ROUTE_NAME.Trim(),sfcdb);
                }
                else
                {
                    sfcdb.RollbackTrain();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        /// <summary>
        ///更新路由，新的路由和原來的路由對比進行更新
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateRoute(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            Route newRoute = new Route();
            try
            {
                string strJsonNewRoute = Data["RouteJsonString"].ToString();
                newRoute = ParsesJSON<Route>(strJsonNewRoute);
            }
            catch
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000022";
                StationReturn.MessagePara.Add("RouteJsonString");
                return;
            }
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                UpdateRouteById(newRoute, sfcdb, StationReturn);
                if (StationReturn.Status == StationReturnStatusValue.Pass)
                {
                    sfcdb.CommitTrain();
                    StationReturn.Data = SelectRouteByRouteName(newRoute.MainMessage.ROUTE_NAME.Trim(),sfcdb);
                }
                else
                {
                    sfcdb.RollbackTrain();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        /// <summary>
        /// 通過路由名刪除路由
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeleteRouteByRouteName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {                     
            OleExec sfcdb = null;
            try
            {
                string RouteName = Data["RouteName"].ToString();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Route getRoute = SelectRouteByRouteName(RouteName, sfcdb);
                if (getRoute.MainMessage == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(RouteName);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }               
                sfcdb.BeginTrain();
                DeleteRoute(getRoute, sfcdb, StationReturn);
                if (StationReturn.Status != StationReturnStatusValue.Pass)
                {
                    sfcdb.RollbackTrain();
                }
                else
                {
                    sfcdb.CommitTrain();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        /// <summary>
        /// 通過路由ID刪除路由
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void DeleteRouteByRouteId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
          
            OleExec sfcdb =null;
            try
            {
                string RouteId = Data["RouteId"].ToString();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Route getRoute = SelectRouteByRouteId(RouteId,sfcdb);
                if (getRoute.MainMessage == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(RouteId);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                sfcdb.BeginTrain();
                DeleteRoute(getRoute, sfcdb, StationReturn);
                if (StationReturn.Status == StationReturnStatusValue.Pass)
                {
                    sfcdb.CommitTrain();
                }
                else
                {
                    sfcdb.RollbackTrain();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        /// <summary>
        /// 獲取所有的路由主信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        //public void GetRouteMainMessage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        //{
        //    OleExec sfcdb =null;
        //    try
        //    {               
        //        string strRouteName=Data["RouteName"].ToString().Trim().ToUpper();
        //        int intCurrentPage=Convert.ToInt32(Data["PageNumber"].ToString().Trim());
        //        int intPageSize = Convert.ToInt32(Data["PageSize"].ToString().Trim());
        //        int intTotal = 0;
        //        RouteMainPage RoutePage = new RouteMainPage();
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //        T_R_SKU_ROUTE TR_SKU_ROUTE = new T_R_SKU_ROUTE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //        List<C_ROUTE> getRoute = TC_ROUTE.GetByNameForPagination(sfcdb, strRouteName, intCurrentPage, intPageSize, out intTotal);
        //        List<C_SKU> getSKUList = new List<C_SKU>();            
        //        if (getRoute != null && getRoute.Count > 0)
        //        {
        //            if (RoutePage.MainData == null)
        //            {
        //                RoutePage.MainData = new List<RouteMainItem>();
        //            }
        //            for (int i = 0; i < getRoute.Count; i++)
        //            {
        //                RouteMainItem newmainitem = new RouteMainItem();
        //                newmainitem.LoadC_ROUTE(getRoute[i]);
        //                getSKUList = TR_SKU_ROUTE.GetSkuListByMappingRouteID(getRoute[i].ID,sfcdb);
        //                if (getSKUList == null)
        //                {
        //                    newmainitem.SKUCOUNT = 0;
        //                }
        //                else
        //                {
        //                    newmainitem.SKUCOUNT = getSKUList.Count;
        //                }
        //                RoutePage.MainData.Add(newmainitem);
        //            }
        //        }
        //        RoutePage.Total = intTotal;
        //        RoutePage.CurrentPage = intCurrentPage;
        //        RoutePage.PageSize = intPageSize;
        //        if (intPageSize != 0)
        //        {
        //            double doubleTotal = intTotal;
        //            double Countpage = doubleTotal / intPageSize;
        //            RoutePage.CountPage =Convert.ToInt32(Math.Ceiling(Countpage));
        //        }
        //        StationReturn.Data = RoutePage;
        //        StationReturn.Status = StationReturnStatusValue.Pass;
        //        StationReturn.MessageCode = "MES00000001";
        //        this.DBPools["SFCDB"].Return(sfcdb);
        //    }
        //    catch (Exception ex)
        //    {
        //        StationReturn.Status = StationReturnStatusValue.Fail;
        //        StationReturn.MessageCode = "MES00000037";
        //        StationReturn.MessagePara.Add(ex.Message);
        //    }
        //    finally
        //    {
        //        if (sfcdb != null)
        //            this.DBPools["SFCDB"].Return(sfcdb);
        //    }
        //}

        public void GetRouteMainMessage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                string strRouteName = Data["RouteName"].ToString().Trim().ToUpper();
                int intCurrentPage = Convert.ToInt32(Data["PageNumber"].ToString().Trim());
                int intPageSize = Convert.ToInt32(Data["PageSize"].ToString().Trim());
                RouteMainPage RoutePage = new RouteMainPage();
                sfcdb = this.DBPools["SFCDB"].Borrow();

                string sqlrun = $@" AND UPPER(CR.ROUTE_NAME) LIKE'%{strRouteName}%'";
                string sqlall = $@"SELECT JJ.ID,JJ.ROUTE_NAME,REPLACE( JJ.STATION,',','->') STATION,JJ.DEFAULT_SKUNO,JJ.ROUTE_TYPE,JJ.EDIT_TIME,JJ.EDIT_EMP,DECODE( KK.SKUCOUNT,null,0,KK.SKUCOUNT ) SKUCOUNT FROM (                      
                                    SELECT DISTINCT CR.ID,CR.ROUTE_NAME,CR.DEFAULT_SKUNO,CR.ROUTE_TYPE,CR.EDIT_TIME,CR.EDIT_EMP,
                                    TO_CHAR(WM_CONCAT(CRD.STATION_NAME)
                                    OVER(PARTITION BY CR.ROUTE_NAME ORDER BY CRD.SEQ_NO
                                    ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED  FOLLOWING)) STATION
                                    FROM C_ROUTE CR,C_ROUTE_DETAIL CRD 
                                    WHERE CR.ID=CRD.ROUTE_ID";
                if (strRouteName != "")
                {
                    sqlall = sqlall+sqlrun;
                }
                sqlall = sqlall+ $@" )JJ LEFT JOIN(
                                SELECT CR.ROUTE_NAME,COUNT(CSK.SKUNO) SKUCOUNT
                                FROM R_SKU_ROUTE RS,C_SKU CSK,C_ROUTE CR 
                                WHERE RS.SKU_ID=CSK.ID 
                                AND CR.ID=RS.ROUTE_ID GROUP BY CR.ROUTE_NAME)KK
                                ON JJ.ROUTE_NAME=KK.ROUTE_NAME ORDER BY JJ.STATION,KK.SKUCOUNT DESC ";
                DataTable total_dt = sfcdb.ExecSelect(sqlall, null).Tables[0];
                if (total_dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");
                }
                StationReturn.Data = total_dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessagePara.Add(total_dt.Rows.Count);
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }



        /// <summary>
        /// 複製路由
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CopyRouteByRouteName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string fromRouteName = Data["FromRouteName"].ToString().Trim();
            string toRouteName = Data["ToRouteName"].ToString().Trim();
            Route newRoute = new Route();
            if (fromRouteName.Length <= 0)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                // StationReturn.Message = "來源路由名不能為空！";                
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("FromRouteName");
                return;
            }
            if (toRouteName.Length <= 0)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                //StationReturn.Message = "新路由名不能為空！";
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara.Add("ToRouteName");
                return;
            }
            if (toRouteName == fromRouteName)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                // StationReturn.Message = "新路由名不能與來源路由名一樣！";
                StationReturn.MessageCode = "MES00000027";
                StationReturn.MessagePara.Add("FromRouteName");
                StationReturn.MessagePara.Add("ToRouteName");
                return;
            }
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                newRoute = SelectRouteByRouteName(fromRouteName,sfcdb);
                if (newRoute.MainMessage == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    // StationReturn.Message = "來源路由不存在！";
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(fromRouteName);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }               
                newRoute.MainMessage.ROUTE_NAME = toRouteName;
                //1 填好操作者和操作時間
                newRoute.MainMessage.EDIT_EMP = LoginUser.EMP_NO;
                newRoute.MainMessage.EDIT_TIME = GetDBDateTime();//獲取數據庫時間
                sfcdb.BeginTrain();
                AddNewRoute(newRoute, sfcdb, StationReturn);
                if (StationReturn.Status == StationReturnStatusValue.Pass)
                {
                    sfcdb.CommitTrain();
                    StationReturn.Data = SelectRouteByRouteName(newRoute.MainMessage.ROUTE_NAME,sfcdb);
                }
                else
                {
                    sfcdb.RollbackTrain();
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        /// <summary>
        /// 獲取新的默認路由名
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetNewRouteName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {           
            OleExec sfcdb =null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ROUTE TC_ROUTE = new T_C_ROUTE(sfcdb,MESDataObject.DB_TYPE_ENUM.Oracle);
                StationReturn.Data=TC_ROUTE.GetNewRouteName(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }      
        /// <summary>
        /// 獲取所有的工站類型
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetAllStationType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {           
            try
            {
                List<string> stationTypeList = new List<string>();
                foreach (string typeString in Enum.GetNames(typeof(STATIONTYPE)))
                {
                    stationTypeList.Add(typeString);
                }             
                StationReturn.Data = stationTypeList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";               
            }
            catch (Exception ex)
            {               
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

        public void CheckSampleteStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                if (BU == "DCN" || BU == "VNDCN" || BU == "DCN_VN")
                {
                    if (Data["DetailID"] == null)
                    {
                        //throw new Exception("Please Input StationID");                            
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143429", new string[] { "DetailID" }));
                    }

                    string detailID = Data["DetailID"].ToString().Trim();
                    C_ROUTE_DETAIL_EX routeDetailEX = sfcdb.ORM.Queryable<C_ROUTE_DETAIL_EX>()
                        .Where(r => r.DETAIL_ID == detailID).ToList().FirstOrDefault();
                    if (routeDetailEX != null)
                    {
                        StationReturn.Data = new {ShowSampleteStation = "YES", IsSampleteStation = "YES"};
                    }
                    else
                    {
                        StationReturn.Data = new {ShowSampleteStation = "YES", IsSampleteStation = "NO"};
                    }
                }
                else
                {
                    StationReturn.Data = new {ShowSampleteStation = "NO", IsSampleteStation = ""};
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {

                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetStationBy90Days(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
          
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();

                var res = sfcdb.ORM
                    .Queryable<R_WO_BASE, C_ROUTE_DETAIL>((r, c) =>
                        r.ROUTE_ID == c.ROUTE_ID && r.EDIT_TIME > DateTime.Now.AddDays(-90)).OrderBy((r, c) => c.STATION_NAME, OrderByType.Asc)
                    .GroupBy((r, c) => c.STATION_NAME).Select((r, c) => c.STATION_NAME).ToList();
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Data = "";
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void SaveSampleteStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {                
                if (Data["SampleteList"] != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
                    List<SampleteStation> list= (List<SampleteStation>)JsonConvert.Deserialize(Data["SampleteList"].ToString(), typeof(List<SampleteStation>));
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    T_C_ROUTE_DETAIL_EX t_c_route_detail_ex = new T_C_ROUTE_DETAIL_EX(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                    C_ROUTE_DETAIL routeDetail = null;
                    C_ROUTE_DETAIL_EX routeEX = null;
                   
                    int result = 0;
                    foreach (SampleteStation s in list)
                    {                       
                        routeDetail = t_c_route_detail.GetById(s.DETIAL_ID, sfcdb);
                        if (routeDetail != null)
                        {
                            routeEX = t_c_route_detail_ex.GetObject(sfcdb, s.DETIAL_ID, "SAMPLETESTLOT", "SAMPLETESTLOT");
                            if (routeEX != null && s.IsSampleteStation == "NO")
                            {
                                result= t_c_route_detail_ex.Delete(sfcdb, s.DETIAL_ID, "SAMPLETESTLOT", "SAMPLETESTLOT");
                                if (result == 0)
                                {
                                    throw new Exception("Delete SAMPLETESTLOT Error!");
                                }
                            }
                            else if (routeEX == null && s.IsSampleteStation == "YES")
                            {
                                routeEX = new C_ROUTE_DETAIL_EX();
                                routeEX.ID = t_c_route_detail_ex.GetNewID(this.BU, sfcdb);
                                routeEX.SEQ_NO = 10;
                                routeEX.NAME = "SAMPLETESTLOT";
                                routeEX.VALUE = "SAMPLETESTLOT";
                                routeEX.DETAIL_ID = routeDetail.ID;
                                result = t_c_route_detail_ex.Save(sfcdb, routeEX);
                                if (result == 0)
                                {
                                    throw new Exception("Save SAMPLETESTLOT Error!");
                                }
                            }
                        }
                    }
                }
                StationReturn.Data = "OK!";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                if (sfcdb != null)
                    this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
    public class Route
    {
        public C_ROUTE MainMessage;
        public List<RouteDetailItem> Detail = new List<RouteDetailItem>();       
    }        
    public class RouteDetailItem
    {
        public string ID;
        public double? SEQ_NO;
        public string ROUTE_ID;
        public string STATION_NAME;
        public string STATION_TYPE;
        public string RETURN_FLAG;
        public List<C_ROUTE_DETAIL> ReturnItems = new List<C_ROUTE_DETAIL>();
        public C_ROUTE_DETAIL NextStation;
        public List<C_ROUTE_DETAIL> DirectLinkStations = new List<C_ROUTE_DETAIL>();
        public void LoadC_ROUTE_Detail(C_ROUTE_DETAIL cRouteDetail)
        {
            this.ID = cRouteDetail.ID;
            this.SEQ_NO = cRouteDetail.SEQ_NO;
            this.ROUTE_ID = cRouteDetail.ROUTE_ID;
            this.STATION_NAME = cRouteDetail.STATION_NAME;
            this.STATION_TYPE = cRouteDetail.STATION_TYPE;
            this.RETURN_FLAG = cRouteDetail.RETURN_FLAG;
        }
        public C_ROUTE_DETAIL getDetailC_ROUTE_DetailObject()
        {
            C_ROUTE_DETAIL cRouteDetail = new C_ROUTE_DETAIL();
            cRouteDetail.ID = this.ID;
            cRouteDetail.SEQ_NO = this.SEQ_NO;
            cRouteDetail.ROUTE_ID= this.ROUTE_ID;
            cRouteDetail.STATION_NAME = this.STATION_NAME;
            cRouteDetail.STATION_TYPE = this.STATION_TYPE;
            cRouteDetail.RETURN_FLAG = this.RETURN_FLAG;
            return cRouteDetail;
        }
    }
    public class RouteMainPage
    {
        public List<RouteMainItem> MainData=new List<RouteMainItem>();       
        public int Total { get; set;}
        public int CurrentPage { get; set;}
        public int PageSize { get; set;}
        public int CountPage { get; set;}
    }
    public class RouteMainItem
    {
        public string ID;
        public string ROUTE_NAME;
        public string DEFAULT_SKUNO;
        public string ROUTE_TYPE;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
        public int? SKUCOUNT;
      
        public void LoadC_ROUTE(C_ROUTE cRoute)
        {
            this.ID = cRoute.ID;
            this.ROUTE_NAME = cRoute.ROUTE_NAME;
            this.DEFAULT_SKUNO = cRoute.DEFAULT_SKUNO;
            this.ROUTE_TYPE = cRoute.ROUTE_TYPE;
            this.EDIT_TIME = cRoute.EDIT_TIME;
            this.EDIT_EMP = cRoute.EDIT_EMP;
        }
    }
    public enum STATIONTYPE
    {
        JOBSTART, NORMAL, JOBFINISH,SHIPFINISH,JOBSTOCKIN
    }

    public class SampleteStation
    {
        public string DETIAL_ID { get; set; }
        public string IsSampleteStation { get; set; }
    }
}
