using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using System.Data;

namespace MESStation.Management
{
    public class OrderManager : MesAPIBase
    {
        protected APIInfo FGetOrderList = new APIInfo
        {
            FunctionName = "GetOrderList",
            Description = "Get Order List",
            Parameters = new List<APIInputInfo>() {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetReworkOrderList = new APIInfo
        {
            FunctionName = "GetReworkOrderList",
            Description = "Get Rework Order List",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetSkunoRouteDetail = new APIInfo
        {
            FunctionName = "GetSkunoRouteDetail",
            Description = "Get Skuno Route Detail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "VER", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FVTConvertWO = new APIInfo
        {
            FunctionName = "VTConvertWO",
            Description = "VTConvertWO",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ROUTE_ID", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "START_STATION", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FVTRestartOrder = new APIInfo
        {
            FunctionName = "VTRestartOrder",
            Description = "VTRestartOrder",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WoList", InputType = "STRING", DefaultValue = ""}
                
            },
            Permissions = new List<MESPermission>()
        }; 
        public OrderManager()
        {
            this.Apis.Add(FGetOrderList.FunctionName, FGetOrderList);
            this.Apis.Add(FGetReworkOrderList.FunctionName, FGetReworkOrderList);
            this.Apis.Add(FGetSkunoRouteDetail.FunctionName, FGetSkunoRouteDetail);
            this.Apis.Add(FVTConvertWO.FunctionName, FVTConvertWO);
            this.Apis.Add(FVTRestartOrder.FunctionName, FVTRestartOrder);
        }

        public void GetOrderList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string type = Data["Type"].ToString().ToUpper();
            string status = Data["Status"].ToString();
            List<string> snList = new List<string>();
            OleExec sfcdb = null;
            List<R_SN_LOCK> lockList = new List<R_SN_LOCK>();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(sfcdb, DBTYPE);
                if (status.ToUpper() == "ALL")
                {
                    status = "";
                }
                if (type == "BYWO")
                {
                    lockList = t_r_sn_lock.GetLockList("", "", "", Data["Data"].ToString(), status, sfcdb);
                }
                else if (type == "BYLOT")
                {
                    lockList = t_r_sn_lock.GetLockList("", Data["Data"].ToString(), "", "", status, sfcdb);
                }
                else if (type == "BYSN")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["Data"];
                    for (int i = 0; i < arraySN.Count; i++)
                    {
                        lockList.AddRange(t_r_sn_lock.GetLockList("", "", arraySN[i].ToString(), "", status, sfcdb));
                    }
                }
                else if (type == "BYID")
                {
                    Newtonsoft.Json.Linq.JArray arraySN = (Newtonsoft.Json.Linq.JArray)Data["Data"];
                    for (int i = 0; i < arraySN.Count; i++)
                    {
                        lockList.AddRange(t_r_sn_lock.GetLockList(arraySN[i].ToString(), "", "", "", status, sfcdb));
                    }
                }

                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = lockList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetReworkOrderList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                List<string> listType = sfcdb.ORM.Queryable<R_WO_TYPE>().Where(r => r.WORKORDER_TYPE == "REWORK" || r.WORKORDER_TYPE == "RMA").Select(r => r.PREFIX).ToList();

                List<R_WO_HEADER> listWO = new List<R_WO_HEADER>();
                if (listType.Count > 0)
                {
                    foreach (string l in listType)
                    {
                        List<R_WO_HEADER> listTemp = sfcdb.ORM.Queryable<R_WO_HEADER>().Where(r => SqlSugar.SqlFunc.StartsWith(r.AUFNR, l)
                        && !SqlSugar.SqlFunc.Subqueryable<R_WO_BASE>().Where(t => t.WORKORDERNO == r.AUFNR).Any()).ToList();
                        listWO.AddRange(listTemp);
                    }
                    listWO.OrderByDescending(r => r.FTRMI);
                }
                if (listWO.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                StationReturn.Data = listWO;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
                StationReturn.Data = ex.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetSkunoRouteDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {            
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                R_WO_HEADER r_wo_header = Data["ROW"] == null ? null : Data["ROW"].ToObject<R_WO_HEADER>();
                List<C_SKU> listSkuObj = null;
                string skuno = r_wo_header.MATNR;
                string ver = r_wo_header.REVLV;
                string wo = r_wo_header.AUFNR;
                //Vertiv NL07類型為RMA工單，PC要求RMA工單取機種的最新版本
                bool isRma = sfcdb.ORM.Queryable<R_WO_TYPE>().Where(r => r.ORDER_TYPE == "NL07" && SqlSugar.SqlFunc.StartsWith(wo, r.PREFIX)).Any();
                if (isRma && string .IsNullOrEmpty(r_wo_header.REVLV))
                {                    
                    listSkuObj = sfcdb.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == skuno).ToList();
                    if (listSkuObj.Count == 0)
                    {
                        throw new Exception($@"{skuno} Not Exist!");
                    }
                    ver = listSkuObj.Max(r => r.VERSION);
                }

                string sql = $@"select * from c_route_detail where route_id in (
                                select route_id from r_sku_route where sku_id in (select id from c_sku where skuno='{skuno}' and version='{ver}' and rownum=1))
                                order by seq_no";
                
                DataTable dt = sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception($@"{skuno},{ver},Not Setting Route!");
                }
                StationReturn.Data = dt;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
                StationReturn.Data = ex.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void VTConvertWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                string wo = Data["WO"].ToString().Trim(); 
                string route_id = Data["ROUTE_ID"].ToString().Trim();
                string start_station = Data["START_STATION"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();               
                T_R_WO_HEADER t_r_wo_header = new T_R_WO_HEADER(sfcdb, DBTYPE);
               

               
                T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DBTYPE);
                T_C_SERIES t_c_series = new T_C_SERIES(sfcdb, DBTYPE);
                T_R_WO_TYPE t_r_wo_type = new T_R_WO_TYPE(sfcdb, DBTYPE);
                T_R_WO_BASE t_wo_base = new T_R_WO_BASE(sfcdb, DBTYPE);
                T_C_ROUTE t_c_route = new T_C_ROUTE(sfcdb, DBTYPE);
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, DBTYPE);
                T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(sfcdb, DBTYPE);
                T_R_MES_LOG TRML = new T_R_MES_LOG(sfcdb, DBTYPE);
                T_R_FAI TRF = new T_R_FAI(sfcdb, DBTYPE);
                T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(sfcdb, DBTYPE);

                R_WO_TYPE rowWOType = null;
                Row_R_WO_BASE rowWOBase = null;
                C_SERIES C_Series = null;
                C_SKU Sku = null;
                R_FAI rfai = new R_FAI();
                List<R_F_CONTROL> RFC;
                List<string> keypartIDList = null;
               
                string sql = "";
                string series = "";
                bool WoIsExistInFai;
                string result = string.Empty;
                string result1 = string.Empty;
                string result2 = string.Empty;
                string ver = "";               
                List<C_SKU> listSkuObj = null;
                sql = $@"select * from r_wo_header a where a.aufnr='{wo}' and not exists (select 1 from r_wo_base b where a.aufnr=b.workorderno) 
                        and trunc(sysdate) - trunc(to_date(DECODE(ftrmi,'0000-00-00','9999-11-11',ftrmi),'yyyy-mm-dd'))>=0";
                DataTable dtConvertWO = sfcdb.ExecuteDataTable(sql, CommandType.Text, null);

                
                //Vertiv NL07類型為RMA工單，PC要求RMA工單取機種的最新版本
                bool isRma = sfcdb.ORM.Queryable<R_WO_TYPE>().Where(r => r.ORDER_TYPE == "NL07" && SqlSugar.SqlFunc.StartsWith(wo, r.PREFIX)).Any();
                if (isRma)
                {
                    listSkuObj = sfcdb.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == dtConvertWO.Rows[0]["MATNR"].ToString().Trim()).ToList();
                    if (listSkuObj.Count == 0)
                    {
                        throw new Exception($@"{dtConvertWO.Rows[0]["MATNR"].ToString().Trim()} Not Exist!");
                    }
                    Sku = listSkuObj.OrderByDescending(r => r.VERSION).ToList().FirstOrDefault();                   
                }
                else
                {
                    Sku = t_c_sku.GetSku(dtConvertWO.Rows[0]["MATNR"].ToString().Trim(), dtConvertWO.Rows[0]["REVLV"].ToString().Trim(), sfcdb);                    
                }                
                if (Sku == null)
                {
                    throw new Exception(" sku " + dtConvertWO.Rows[0]["MATNR"].ToString().Trim() + ",version " + ver + " not exist");
                }
                ver = Sku.VERSION;
                if (Sku.C_SERIES_ID != null && Sku.C_SERIES_ID.ToString() != "")
                {
                    C_Series = t_c_series.GetDetailById(sfcdb, Sku.C_SERIES_ID);
                    if (C_Series == null)
                    {
                        throw new Exception(" the series of " + dtConvertWO.Rows[0]["MATNR"].ToString().Trim() + " not exist");
                    }
                    series = C_Series.SERIES_NAME;
                }
                else
                {
                    series = "VERTIV_DEFAULT";
                }
                rowWOType = t_r_wo_type.GetWOTypeByWO(sfcdb, dtConvertWO.Rows[0]["AUART"].ToString());
                if (rowWOType == null)
                {
                    throw new Exception("get wo type fail");
                }
                
                keypartIDList = t_c_kp_list.GetListIDBySkuno(Sku.SKUNO, sfcdb);
                if (keypartIDList.Count > 0 && keypartIDList.Count != 1)
                {
                    throw new Exception("skuno:" + dtConvertWO.Rows[0]["MATNR"].ToString() + " have more keypart id");
                }

                T_R_WO_LOG TRWOLOG = new T_R_WO_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                bool CheckKp = TRWOLOG.CheckKpSetCount(dtConvertWO.Rows[0]["AUFNR"].ToString(), sfcdb);
                bool checkBU = TRFC.CheckUserFunctionExist("WOKPComfirm", "WOKPComfirm", this.BU, sfcdb);
                if (!CheckKp && checkBU)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200624162658"));
                }

                bool Csd = TRFC.CheckUserFunctionExist("FAI_CONFIG", "FAI_CONFIG", dtConvertWO.Rows[0]["MATNR"].ToString(), sfcdb);

                if (!Csd)
                {
                    RFC = TRFC.GetListByFcv("FAI_CONFIG", "FAI_CONFIG", dtConvertWO.Rows[0]["MATNR"].ToString(), sfcdb);
                    if (RFC.Count > 0)
                    {
                        WoIsExistInFai = TRF.CheckWoHaveDoneFai(dtConvertWO.Rows[0]["AUFNR"].ToString(), RFC[0].EXTVAL, sfcdb);
                        if (WoIsExistInFai)
                        {
                            rfai.ID = TRF.GetNewID(BU, sfcdb);
                            rfai.FAITYPE = "WORKORDER";
                            rfai.STATUS = "0";
                            rfai.WORKORDERNO = dtConvertWO.Rows[0]["AUFNR"].ToString();
                            rfai.CREATEBY = LoginUser.EMP_NO;
                            rfai.CREATETIME = t_wo_base.GetDBDateTime(sfcdb);
                            result = sfcdb.ORM.Insertable<R_FAI>(rfai).ExecuteCommand().ToString();

                            T_R_FAI_STATION RFAIS = new T_R_FAI_STATION(sfcdb, DBTYPE);
                            R_FAI_STATION rfais = new R_FAI_STATION();
                            rfais.ID = RFAIS.GetNewID(BU, sfcdb);
                            rfais.FAIID = rfai.ID;
                            rfais.STARTSTATION = "";
                            rfais.FAISTATION = RFC[0].EXTVAL;
                            rfais.CREATEBY = LoginUser.EMP_NO; 
                            rfais.CREATETIME = t_wo_base.GetDBDateTime(sfcdb);
                            result1 = sfcdb.ORM.Insertable<R_FAI_STATION>(rfais).ExecuteCommand().ToString();

                            T_R_FAI_DETAIL RFAISD = new T_R_FAI_DETAIL(sfcdb, DBTYPE);
                            R_FAI_DETAIL rfaisd = new R_FAI_DETAIL();
                            rfaisd.ID = RFAISD.GetNewID(BU, sfcdb);
                            rfaisd.FAISTATIONID = rfais.ID;
                            rfaisd.STATUS = "0";
                            rfaisd.CREATEBY = LoginUser.EMP_NO; 
                            rfaisd.CREATETIME = t_wo_base.GetDBDateTime(sfcdb);
                            result2 = sfcdb.ORM.Insertable<R_FAI_DETAIL>(rfaisd).ExecuteCommand().ToString();

                        }
                    }
                }

                rowWOBase = (Row_R_WO_BASE)t_wo_base.NewRow();
                rowWOBase.ID = t_wo_base.GetNewID(BU, sfcdb);
                rowWOBase.WORKORDERNO = dtConvertWO.Rows[0]["AUFNR"].ToString();
                rowWOBase.PLANT = dtConvertWO.Rows[0]["WERKS"].ToString();
                rowWOBase.RELEASE_DATE = t_wo_base.GetDBDateTime(sfcdb);
                rowWOBase.DOWNLOAD_DATE = t_wo_base.GetDBDateTime(sfcdb);
                rowWOBase.PRODUCTION_TYPE = "BTO";//沒有確定先寫死                       
                rowWOBase.WO_TYPE = rowWOType.WORKORDER_TYPE;
                rowWOBase.SKUNO = dtConvertWO.Rows[0]["MATNR"].ToString();
                rowWOBase.SKU_VER = ver;
                rowWOBase.SKU_SERIES = series;
                rowWOBase.SKU_NAME = Sku.SKU_NAME;
                rowWOBase.SKU_DESC = Sku.DESCRIPTION;
                rowWOBase.CUST_PN = Sku.CUST_PARTNO;
                rowWOBase.CUST_PN_VER = "";
                rowWOBase.CUSTOMER_NAME = Sku.CUST_SKU_CODE;
                rowWOBase.ROUTE_ID = route_id;
                rowWOBase.START_STATION = start_station;
                rowWOBase.KP_LIST_ID = (keypartIDList != null && keypartIDList.Count > 0) ? keypartIDList[0] : "";
                rowWOBase.CLOSED_FLAG = "0";
                rowWOBase.WORKORDER_QTY = Convert.ToDouble(dtConvertWO.Rows[0]["GAMNG"]);
                rowWOBase.INPUT_QTY = 0;
                rowWOBase.FINISHED_QTY = 0;
                rowWOBase.SCRAPED_QTY = 0;
                rowWOBase.STOCK_LOCATION = dtConvertWO.Rows[0]["LGORT"].ToString();
                rowWOBase.PO_NO = "";
                rowWOBase.CUST_ORDER_NO = dtConvertWO.Rows[0]["ABLAD"].ToString();
                rowWOBase.ROHS = dtConvertWO.Rows[0]["ROHS_VALUE"].ToString();
                rowWOBase.EDIT_EMP = LoginUser.EMP_NO; 
                rowWOBase.EDIT_TIME = t_wo_base.GetDBDateTime(sfcdb);
                sfcdb.ThrowSqlExeception = true;
                sql = rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle);
                sfcdb.ExecSQL(rowWOBase.GetInsertString(DB_TYPE_ENUM.Oracle));

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
                StationReturn.Data = ex.Message;
            }
            finally
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    
        public void VTRestartOrder(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                Newtonsoft.Json.Linq.JArray arrayPo = (Newtonsoft.Json.Linq.JArray)Data["WoList"];
                string msg = "";
                for (int i = 0; i < arrayPo.Count; i++)
                {
                    string wo = arrayPo[i].ToString();
                    if (!string.IsNullOrWhiteSpace(wo))
                    {
                        bool bLoading = SFCDB.ORM.Queryable<R_SN>().Any(r => r.WORKORDERNO == wo);
                        if(bLoading)
                        {
                            msg += $@"{wo},";
                            continue;
                        }
                        R_WO_BASE woObj = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == wo).ToList().FirstOrDefault();
                        if(woObj!=null)
                        {
                            woObj.WORKORDERNO = "*" + woObj.WORKORDERNO;
                            woObj.SKUNO = "*" + woObj.SKUNO;
                            woObj.SKU_NAME = "*" + woObj.SKU_NAME;
                            woObj.SKU_SERIES = "*" + woObj.SKU_SERIES;
                            woObj.CUST_PN = "*" + woObj.CUST_PN;
                            SFCDB.ORM.Updateable<R_WO_BASE>(woObj).Where(r => r.ID == woObj.ID).ExecuteCommand();
                        }
                        
                    }
                }

                StationReturn.Data = msg==""?"OK":$@"{msg} already loading,please check.";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
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
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
    }
}
