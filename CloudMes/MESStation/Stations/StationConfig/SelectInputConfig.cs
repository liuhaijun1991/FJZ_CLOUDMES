using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationConfig
{
    public class SelectInputConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo FGetTEStationBySN = new APIInfo()
        {
            FunctionName = "GetTEStationBySN",
            Description = "Get TE Station By SN",
            Parameters = new List<APIInputInfo>()
            {               
                new APIInputInfo() { InputName = "SN",InputType = "string",DefaultValue = ""}                
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo FGetReprintStationByEmpNO = new APIInfo()
        {
            FunctionName = "GetReprintStationByEmpNO",
            Description = "Get reprint station  by empno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "EmpNo",InputType = "string",DefaultValue = ""},
                new APIInputInfo() { InputName = "SKU",InputType = "string",DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo FGetLabelTypeBySkuAndStation = new APIInfo {
            FunctionName = "GetLabelTypeBySkuAndStation",
            Description = "Get label type by sku and station",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "Station",InputType = "string",DefaultValue = ""},
                new APIInputInfo() { InputName = "SKU",InputType = "string",DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo FHWTGetWHSStockList = new APIInfo
        {
            FunctionName = "HWTGetWHSStockList",
            Description = "HWT Get WHS Stock List",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "TYPE",InputType = "string",DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo FGetKpListBySN = new APIInfo()
        {
            FunctionName = "GetKpListBySN",
            Description = "獲取SN對應機種的Keypart",
            Parameters = new List<APIInputInfo>(){new APIInputInfo() { InputName = "SN",InputType = "string",DefaultValue = ""} },
            Permissions = new List<MESPermission>(){}
        };
        private APIInfo FGetPcbaSNBySN = new APIInfo()
        {
            FunctionName = "GetPcbaSNBySN",
            Description = "獲取SN的PCBASN",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>() { }
        };
        private APIInfo FGetReturnStation = new APIInfo()
        {
            FunctionName = "GetReturnStation",
            Description = "通過SN獲取路由中配置的維修返回工站,如果沒有配置,則返回本身下一站",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SN", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>() { }
        };
        public SelectInputConfig()
        {
            this.Apis.Add(FGetTEStationBySN.FunctionName, FGetTEStationBySN);
            this.Apis.Add(FGetReprintStationByEmpNO.FunctionName, FGetReprintStationByEmpNO);
            this.Apis.Add(FHWTGetWHSStockList.FunctionName, FHWTGetWHSStockList);
            this.Apis.Add(FGetKpListBySN.FunctionName, FGetKpListBySN);
            this.Apis.Add(FGetPcbaSNBySN.FunctionName, FGetPcbaSNBySN);
            this.Apis.Add(FGetReturnStation.FunctionName, FGetReturnStation);
        }

        public void GetTEStationBySN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {          
            OleExec sfcdb = null;
            string sn = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sn = (Data["SN"].ToString()).Trim().ToUpper();
                List<string> stationList = new List<string>();
                List<string> list = new List<string>();
                R_SN snObject = sfcdb.ORM.Queryable<R_SN>().Where(r_sn => r_sn.SN == sn && r_sn.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (snObject != null)
                {
                    list = sfcdb.ORM.Queryable<C_ROUTE_DETAIL>().Where(route => route.ROUTE_ID == snObject.ROUTE_ID && SqlSugar.SqlFunc.Subqueryable<C_TEMES_STATION_MAPPING>()
                            .Where(map => map.MES_STATION == route.STATION_NAME).Any()).OrderBy(route => route.SEQ_NO).Select(route => route.STATION_NAME).ToList();
                }
                stationList.Add("");
                stationList.AddRange(list);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = stationList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void GetReprintStationByEmpNO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string empno = "";
            string sku = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                empno = (Data["EmpNo"].ToString()).Trim().ToUpper();
                sku = (Data["SKU"].ToString()).Trim();
                List<string> stationList = new List<string>();
                List<string> list = new List<string>();
                list = sfcdb.ORM.Queryable<C_SKU_Label>().Where(label => label.SKUNO == sku).OrderBy(label => label.STATION)
                    .GroupBy(lable => lable.STATION).Select(label => label.STATION).ToList();
                this.DBPools["SFCDB"].Return(sfcdb);
                stationList.Add("");
                stationList.AddRange(list);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = stationList;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void GetLabelTypeBySkuAndStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string station = "";
            string sku = "";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                station = (Data["StationName"].ToString()).Trim().ToUpper();
                sku = (Data["SKU"].ToString()).Trim().ToUpper();
                List<string> typeList = new List<string>();
                List<string> list = new List<string>();
                list = sfcdb.ORM.Queryable<C_SKU_Label>().Where(label => label.STATION == station && label.SKUNO == sku).OrderBy(label => label.LABELTYPE)
                    .GroupBy(lable => lable.LABELTYPE).Select(label => label.LABELTYPE).ToList();
                this.DBPools["SFCDB"].Return(sfcdb);
                typeList.Add("");
                typeList.Add("ALL");
                typeList.AddRange(list);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = typeList;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void HWTGetWHSStockList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;           
            string inputType = "CONFIG";
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                if (Data["TYPE"] != null)
                {
                    inputType = (Data["TYPE"].ToString()).Trim().ToUpper();
                }
                List<string> list = sfcdb.ORM.Queryable<R_WH_STOCK_LIST>().Where(r => r.TYPE == inputType)
                    .OrderBy(r => r.LOCATION, SqlSugar.OrderByType.Asc).Select(r => r.LOCATION).ToList();
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = list;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        /// <summary>
        /// 獲取SN對應機種的Keypart
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetKpListBySN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sn = Data["SN"].ToString().Trim().ToUpper();
                List<string> returnList = new List<string>();
                List<string> kpList = new List<string>();
                if (string.IsNullOrEmpty(sn))
                {
                    return;
                }
                R_SN objSN = SFCDB.ORM.Queryable<R_SN>().Where(r_sn => r_sn.SN == sn && r_sn.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (objSN != null)
                {
                    kpList = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn && t.VALID_FLAG == 1).OrderBy(t => t.PARTNO).Select(t => t.PARTNO).ToList();
                }
                //如果沒有Kp返回本身
                if (kpList.Count == 0)
                {
                    kpList.Add(objSN.SKUNO);
                }
                returnList.Add("");
                returnList.AddRange(kpList);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = returnList;
                //this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception e)
            {
                
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        /// <summary>
        /// 獲取SN的PCBASN
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetPcbaSNBySN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sn = Data["SN"].ToString().Trim().ToUpper();
                List<string> returnList = new List<string>();
                List<string> kpList = new List<string>();


                R_SN objSN = SFCDB.ORM.Queryable<R_SN>().Where(r_sn => r_sn.SN == sn && r_sn.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (objSN != null)
                {
                    //fix bug edit by zhb 20200827
                    var x = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.WSN == sn || t.VSSN == sn || t.CSSN == sn).OrderBy(t => t.WSN).Select(t => t.WSN).ToList();
                    if (x.Count != 0)
                    {
                        foreach (var xx in x)
                        {
                            kpList.Add(xx);
                        }
                    }
                    //取不到就取本身
                    else
                    {
                        kpList.Add(sn);
                    }
                    //var a = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(r => r.WSN == sn).Select(r => r.WSN).ToList().FirstOrDefault();
                    //var b = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(r => r.VSSN == sn).Select(r => r.WSN).ToList().FirstOrDefault();
                    //var c = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(r => r.CSSN == sn).Select(r => r.WSN).ToList().FirstOrDefault();
                    //var d = SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.SN == sn && r.SCANTYPE == "PCBA S/N").Select(r => r.VALUE).ToList().FirstOrDefault();
                    //if (a != null) kpList.Add(a);
                    //if (b != null) kpList.Add(b);
                    //if (c != null) kpList.Add(c);
                    //if (d != null) kpList.Add(d);
                }
                this.DBPools["SFCDB"].Return(SFCDB);
                returnList.Add("");
                returnList.AddRange(kpList);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = returnList;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void GetPcbaSNBySNNew(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sn = Data["SN"].ToString().Trim().ToUpper();
                List<string> returnList = new List<string>();
                List<string> kpList = new List<string>();

                var objPCB = SFCDB.ORM.Queryable<R_SN_KP>().Where(r_sn => r_sn.SN == sn && (r_sn.SCANTYPE == "SystemSN" || r_sn.SCANTYPE == "KEEP_SN")).Select(r_sn => r_sn.VALUE).ToList();
                
                if (objPCB.Count != 0)
                {
                    foreach (string pcb in objPCB) {
                        kpList.Add(pcb);
                    }
                    returnList.Add("");
                    returnList.AddRange(kpList);
                }
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = returnList;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        /// <summary>
        /// 通過SN獲取路由中配置的維修返回工站
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetReturnStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sn = Data["SN"].ToString().Trim().ToUpper();
                List<string> returnList = new List<string>();
                List<string> stationList = new List<string>();


                R_SN objSN = SFCDB.ORM.Queryable<R_SN>().Where(r_sn => r_sn.SN == sn && r_sn.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (objSN != null)
                {
                    stationList = SFCDB.ORM.Queryable<C_ROUTE_DETAIL, C_ROUTE_DETAIL_RETURN, C_ROUTE_DETAIL>((r, s, t) => r.ID == s.ROUTE_DETAIL_ID && s.RETURN_ROUTE_DETAIL_ID==t.ID)
                        .Where((r, s, t) => r.ROUTE_ID == objSN.ROUTE_ID && r.STATION_NAME == objSN.NEXT_STATION).Select((r, s, t) => t.STATION_NAME).ToList();
                    if (stationList.Count == 0)
                    {
                        stationList.Add(objSN.NEXT_STATION);
                    }

                    //20200922 RE要求功能維修一律返回工站為BFT/ST
                    var objRRM = SFCDB.ORM.Queryable<R_REPAIR_MAIN>().Where(t => t.SN == sn && t.CLOSED_FLAG == "0").ToList().FirstOrDefault();
                    var routeList = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == objSN.ROUTE_ID).ToList();
                    //var routeBFT = routeList.Find(t => SqlSugar.SqlFunc.Contains(t.STATION_NAME, "BFT"));
                    var routeBFT = routeList.Find(t => t.STATION_NAME == "BFT");
                    if (routeBFT != null && objRRM.FAIL_STATION != "COSMETIC-FAILURE")
                    {
                        //如果掃進維修的工站在路由中的序號大於等於BFT工站的序號
                        if (routeList.Find(t => t.STATION_NAME == objRRM.FAIL_STATION) != null)
                        {
                            if (routeList.Find(t => t.STATION_NAME == objRRM.FAIL_STATION).SEQ_NO >= routeBFT.SEQ_NO)
                            {
                                stationList.Clear();
                                stationList.Add(routeBFT.STATION_NAME);
                            }
                        }
                    }
                    //var routeST = routeList.Find(t => SqlSugar.SqlFunc.Contains(t.STATION_NAME, "ST"));
                    var routeST = routeList.Find(t => t.STATION_NAME =="ST");//防止取到STOCKIN工站
                    if (routeST != null && objRRM.FAIL_STATION != "COSMETIC-FAILURE")
                    {
                        //如果掃進維修的工站在路由中的序號大於等於ST工站的序號
                        if (routeList.Find(t => t.STATION_NAME == objRRM.FAIL_STATION) != null)
                        {
                            if (routeList.Find(t => t.STATION_NAME == objRRM.FAIL_STATION).SEQ_NO >= routeST.SEQ_NO)
                            {
                                stationList.Clear();
                                stationList.Add(routeST.STATION_NAME);
                            }
                        }
                    }
                    //根據QE 胡航舟需求增加個開關 如果有配置 維修checkout出來後板子回到當階的第一個測試工站 20201116
                    bool bol = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(R => R.FUNCTIONNAME == "REPAIR_RETURN" && SqlSugar.SqlFunc.Contains(R.VALUE, objSN.SKUNO)).Any();
                    if (bol)
                    {
                        List<string> MES_STATION = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Select(r => r.MES_STATION).ToList();
                        MES_STATION.Add("FT");

                        List<string> Route_list = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == objSN.ROUTE_ID && MES_STATION.Contains(r.STATION_NAME)).OrderBy(r=>r.SEQ_NO,SqlSugar.OrderByType.Asc).Select(r => r.STATION_NAME).ToList();

                        //檢查權限
                        var prs = SFCDB.ORM.Queryable<C_USER, C_USER_PRIVILEGE, C_PRIVILEGE>((U, UP, P) => new object[] {
                                    SqlSugar.JoinType.Left, U.ID == UP.USER_ID,
                                    SqlSugar.JoinType.Left, P.ID == UP.PRIVILEGE_ID })
                                   .Where((U, UP, P) => U.EMP_NO ==this.LoginUser.EMP_NO && P.PRIVILEGE_NAME == "RepairReturn")
                                   .ToList();
                        if (prs.Count == 0)  //沒有權限返回測試工站第一個工站
                        {

                            stationList.Clear();
                            stationList.Add(Route_list.ToList()[0]);

                        }
                        else  //有RepairReturn權限就返回所有的測試工站
                        {

                            stationList.Clear();
                            for (int i =0;i< Route_list.Count;i++)
                            {
                                stationList.Add(Route_list.ToList()[i]);
                            }

                        }
                    }


                }
                this.DBPools["SFCDB"].Return(SFCDB);
                returnList.Add("");
                returnList.AddRange(stationList);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = returnList;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void SupermarketSelectLocationTo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string strStorage, strStorageName;
                SFCDB = this.DBPools["SFCDB"].Borrow();
                List<string> concList = new List<string>();
                List<string> returnList = new List<string>();
                returnList.Add("Select Location");

                var objStorage = SFCDB.ORM.Queryable<C_STORAGE_CODE>().Where(s => s.CATEGORY == "SUPERMARKET").ToList();
                
                foreach(var storage in objStorage)
                {
                    strStorage = storage.STORAGE_CODE;
                    strStorageName = storage.STORAGE_NAME;
                    returnList.Add(strStorageName  + " ~ " + strStorage);
                }

                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = returnList;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void WarehouseSNSelectLocation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string strStorage, strStorageName;
                SFCDB = this.DBPools["SFCDB"].Borrow();
                List<string> concList = new List<string>();
                List<string> returnList = new List<string>();
                returnList.Add("Select Location");

                var objStorage = SFCDB.ORM.Queryable<C_STORAGE_CODE>().Where(s => s.CATEGORY == "WAREHOUSE").ToList();

                foreach (var storage in objStorage)
                {
                    strStorage = storage.STORAGE_CODE;
                    strStorageName = storage.STORAGE_NAME;
                    returnList.Add(strStorageName + " ~ " + strStorage);
                }

                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = returnList;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }

        public void SelectReverseType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> returnList = new List<string>();
            returnList.Add("Please Select Reverse Type");
            returnList.Add("WO");
            returnList.Add("SN");

            StationReturn.Message = "";
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Data = returnList;
        }

            public void StationLoader(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                List<string> returnList = new List<string>() { "","SWTEST", "FST_SPARES" };
                //returnList.AddRange(returnList);
                

                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = returnList;
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
        }
    }
}
