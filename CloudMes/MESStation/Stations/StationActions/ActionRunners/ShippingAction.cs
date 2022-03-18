using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module.ARUBA;
using MESDataObject.Module.DCN;
using System.Data;
using MESPubLab.MESInterface;
using MESPubLab.SAP_RFC;
using System.Configuration;
using MESDataObject.Module.OM;
using static MESDataObject.Constants.PublicConstants;
using static MESDataObject.Common.EnumExtensions;
using System.Text.RegularExpressions;
using MESPubLab.MESStation.MESReturnView.Station;
using MESDataObject.Constants;
using MESDataObject.Module.HWD;
using MESPubLab.MesBase;
using MESStation.Packing;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class ShippingAction
    {
        /// <summary>
        /// HWT出貨后恢復儲位容量
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTResetWHStockAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionTOHeadObject = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTOHeadObject == null || sessionTOHeadObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            string pallet = sessionPalletNo.Value.ToString();


            MESDataObject.Module.HWT.T_R_WH_STOCK_LIST TRWS = new MESDataObject.Module.HWT.T_R_WH_STOCK_LIST(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_TO_HEAD_HWT TRTH = new MESDataObject.Module.HWT.T_R_TO_HEAD_HWT(sfcdb, dbtype);
            MESDataObject.Module.HWT.R_TO_HEAD_HWT objTOHead = (MESDataObject.Module.HWT.R_TO_HEAD_HWT)sessionTOHeadObject.Value;

            List<MESDataObject.Module.HWT.R_WH_STOCK_LIST> listStock = TRWS.GetStockListByTypeAndArea(sfcdb, "DETAIL", pallet);
            if (listStock.Count > 0)
            {
                sfcdb.ORM.Updateable<MESDataObject.Module.HWT.R_WH_STOCK_LIST>()
                    .UpdateColumns(r => new MESDataObject.Module.HWT.R_WH_STOCK_LIST { CURRENT_QTY = r.CURRENT_QTY - 1, LESS_QTY = r.LESS_QTY + 1 })
                    .Where(r => r.LOCATION == listStock.First().LOCATION && r.TYPE == "CONFIG").ExecuteCommand();
                //共管第一次shipping的給一個臨時儲位，其他的直接刪除
                if (objTOHead.EXTERNAL_NO == "GG0001")
                {
                    sfcdb.ORM.Updateable<MESDataObject.Module.HWT.R_WH_STOCK_LIST>()
                        .UpdateColumns(r => new MESDataObject.Module.HWT.R_WH_STOCK_LIST { LOCATION = "JIT", EDIT_EMP = Station.LoginUser.EMP_NO, EDIT_TIME = SqlSugar.SqlFunc.GetDate() })
                        .Where(r => r.AREA == pallet && r.TYPE == "DETAIL").ExecuteCommand();
                }
                else
                {
                    sfcdb.ORM.Deleteable<MESDataObject.Module.HWT.R_WH_STOCK_LIST>().Where(r => r.TYPE == "DETAIL" && r.AREA == pallet).ExecuteCommand();
                }
            }
        }

        public static void SaveHWTSFCRelationData(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionPalletObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletObject == null || sessionPalletObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionTOHeadObject = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTOHeadObject == null || sessionTOHeadObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionDNItemObject = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionDNItemObject == null || sessionDNItemObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            string BU = Station.BU;
            DateTime sysdateTime = Station.GetDBDateTime();
            int saveResult = 0;
            MESDataObject.Module.HWT.R_TO_HEAD_HWT objTOHead = (MESDataObject.Module.HWT.R_TO_HEAD_HWT)sessionTOHeadObject.Value;
            MESDataObject.Module.HWT.R_DN_DETAIL objDNItem = (MESDataObject.Module.HWT.R_DN_DETAIL)sessionDNItemObject.Value;

            LogicObject.Packing objPallet = (LogicObject.Packing)sessionPalletObject.Value;

            MESDataObject.Module.HWT.T_R_TO_DETAIL_HWT TRTD = new MESDataObject.Module.HWT.T_R_TO_DETAIL_HWT(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_DN_DETAIL TRDD = new MESDataObject.Module.HWT.T_R_DN_DETAIL(sfcdb, dbtype);

            MESDataObject.Module.HWT.T_HWT_SFC_RELATION_DATA THSR = new MESDataObject.Module.HWT.T_HWT_SFC_RELATION_DATA(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_RELATION_DATA TRRD = new MESDataObject.Module.HWT.T_R_RELATION_DATA(sfcdb, dbtype);

            T_R_WO_BASE TRWB = new T_R_WO_BASE(sfcdb, dbtype);


            objDNItem.PO_NO.Count(c => c == '-');
            string sql = "";
            string lotno = "";
            string rhosStatus = "";
            string packNo = "";
            string pn_no = "";
            string asnid = "";
            //string hwpn = "";
            R_WO_BASE objWO = null;
            R_PACKING objCarton = null;
            System.Data.DataTable dataTable = new System.Data.DataTable();

            #region 還原po_no 將po_no，把第二個‘-‘後面的字符去除 begin
            if (objDNItem.PO_NO.Count(c => c == '-') > 1)
            {
                //字符'-' 出現兩次及兩次以上判斷為直發PO 去除后七位字符
                pn_no = objDNItem.PO_NO.Substring(0, objDNItem.PO_NO.Length - 7);
            }
            else if (objDNItem.PO_NO.Count(c => c == '-') == 1)
            {
                //字符'-' 出現一次，先判斷是否是直發，如果是也要去除後七位字符
                if (objDNItem.PO_NO.Substring(objDNItem.PO_NO.IndexOf('-'), objDNItem.PO_NO.Length - objDNItem.PO_NO.IndexOf('-')).Length == 7)
                {
                    pn_no = objDNItem.PO_NO.Substring(objDNItem.PO_NO.IndexOf('-'), objDNItem.PO_NO.Length - objDNItem.PO_NO.IndexOf('-'));
                }
                else
                {
                    pn_no = objDNItem.PO_NO;
                }
            }
            else
            {
                pn_no = objDNItem.PO_NO;
            }
            #endregion

            //獲取SO_ITEM_NO的值,插入PO_ITEM欄位用於ASN預約排序
            int po_item = 0;
            if (int.TryParse(objDNItem.SO_ITEM_NO, out po_item))
            {
                po_item = po_item / 10;
            }
            else
            {
                throw new MESReturnMessage("GET SO_ITEM_NO From " + objDNItem.DN_NO + " Fail!");
            }
            #region 中箱CARTON 打印邏輯沒有寫
            //        --如果CARTONRELATION有var_packno的記錄,說明CARTON_ID有轉換,使用新的CARTON_ID add by taylor  20160125
            //SELECT COUNT(*)
            //  INTO var_rowcount
            //  FROM cartonrelation
            // WHERE location = var_packno;

            //        IF var_rowcount > 0 THEN
            //          --SELECT carton
            //  --INTO var_packno
            // -- FROM cartonrelation
            //  --WHERE LOCATION = var_packno AND ROWNUM = 1;

            //        --------begin - 抓取最后一次打印的中箱CARTON(上面的語句會抓取較早的中箱CARTON造成交管預約異常)MODIFY BY HGB 20160425
            //  SELECT carton
            //    INTO var_packno
            //    FROM(SELECT *
            //            FROM cartonrelation
            //           WHERE location = var_packno
            //           ORDER BY data2 DESC)
            //   WHERE rownum = 1;
            //        --------begin - 抓取最后一次打印的中箱CARTON(上面的語句會抓取較早的中箱CARTON造成交管預約異常)MODIFY BY HGB 20160425
            //END IF;

            //        --end by taylor
            #endregion




            foreach (R_SN sn in objPallet.SNList)
            {
                objWO = TRWB.LoadWorkorder(sn.WORKORDERNO, sfcdb).GetDataObject();
                switch (objWO.ROHS.ToUpper())
                {
                    case "R5":
                    case "NR":
                        rhosStatus = "N";
                        break;
                    case "R6":
                        rhosStatus = "Y";
                        break;
                    default:
                        rhosStatus = objWO.ROHS.ToUpper();
                        break;
                }

                objCarton = sfcdb.ORM.Queryable<R_PACKING, R_SN_PACKING>((rp, rsp) => rp.ID == rsp.PACK_ID)
                    .Where((rp, rsp) => rsp.ID == sn.ID).Select((rp, rsp) => rp).ToList().FirstOrDefault();
                if (objCarton.MAX_QTY == 1)
                {
                    packNo = sn.BOXSN;
                }
                else
                {
                    packNo = objCarton.PACK_NO;
                }
                DateTime dt = (DateTime)objCarton.EDIT_TIME;
                lotno = dt.ToString("yyyyMMddHHmmss");

                sql = "select  rpad(sfc.ASNID.nextval,12, '0')  from dual";
                dataTable = sfcdb.RunSelect(sql).Tables[0];
                asnid = dataTable.Rows[0][0].ToString();

                MESDataObject.Module.HWT.HWT_SFC_RELATION_DATA relationData = null;
                relationData = new MESDataObject.Module.HWT.HWT_SFC_RELATION_DATA();
                relationData.ID = THSR.GetNewID(Station.BU, sfcdb, dbtype);
                relationData.CARTON_LINE_NO = asnid;
                relationData.SHIPPING_DATE = sysdateTime;
                relationData.TO_NO = objTOHead.TO_NO;
                relationData.PO = pn_no;
                relationData.PO_ITEM = po_item.ToString();
                relationData.HWPN = "";
                relationData.HHPN = "";
                relationData.DN = "";
                relationData.CARTON_ID = packNo;
                relationData.LOTNO = lotno;
                relationData.ROHS_STATUS = rhosStatus;
                relationData.SHIPPING_TIME = sysdateTime;
                relationData.CARTON_QTY = objCarton.QTY;
                relationData.CANBESENT = "";
                relationData.EDI_SENDFLAG = "";
                relationData.EDIT_TIME = sysdateTime;
                relationData.SHIPTOCODE = "";
                relationData.F_LOCATION = "";
                //relationData.CARTONCBSTIME = "";
                //relationData.TOSTARTTIME = "";
                relationData.SN = TRRD.GetParentSNByPCBSN(sfcdb, dbtype, sn);
                relationData.CUSTOMER_KP_NO_VER = "";
                saveResult = THSR.SaveRelationData(sfcdb, relationData);
            }
        }

        /// <summary>
        /// Vertiv 退SHIPPING
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReturnShppingByTypeAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionType = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionType == null || sessionType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionInputValue = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionInputValue == null || sessionInputValue.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionDNLine = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);

            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL TRSSD = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_R_DN_STATUS TRDS = new T_R_DN_STATUS(Station.SFCDB, Station.DBType);
            T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(Station.SFCDB, Station.DBType);

            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
            T_R_WO_BASE TRWB = new T_R_WO_BASE(Station.SFCDB, Station.DBType);

            string type = sessionType.Value.ToString().ToUpper().Trim();
            string inputValue = sessionInputValue.Value.ToString().ToUpper().Trim();
            int result = 0;
            string dn_no = "";
            string dn_line = "";
            string current_station = "";
            List<C_ROUTE_DETAIL> listRoute = new List<C_ROUTE_DETAIL>();
            List<R_SN> listSN = new List<R_SN>();
            R_SN objSN;
            R_SHIP_DETAIL objShipDetail;
            R_SN_STATION_DETAIL objStationDetail;
            R_DN_STATUS objDNStatus;
            DateTime sysDateTime = TRS.GetDBDateTime(Station.SFCDB);
            switch (type)
            {
                case "SN":
                    #region Cancel Ship Out By SN
                    objSN = TRS.LoadData(inputValue, Station.SFCDB);
                    objShipDetail = TRSD.GetShipDetailBySN(Station.SFCDB, objSN.SN);
                    if (objSN == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { inputValue }));
                    }
                    if (objSN.NEXT_STATION != "SHIPFINISH" && objSN.CURRENT_STATION != "SHIPOUT")
                    {
                        throw new MESReturnMessage(objSN.SN + " Hasn't Been Shipped Yet!");
                    }
                    if (objShipDetail == null)
                    {
                        throw new MESReturnMessage(objSN.SN + " No Shipping Record!");
                    }
                    dn_no = objShipDetail.DN_NO;
                    dn_line = objShipDetail.DN_LINE;
                    objDNStatus = TRDS.GetStatusByNOAndLine(Station.SFCDB, dn_no, dn_line);
                    if (objDNStatus.DN_FLAG == "3")
                    {
                        //做完GT不給退SHIPPING
                        throw new Exception("This " + dn_no + "," + dn_line + " Has Done GT!");
                    }

                    listRoute = TCRD.GetLastStations(objSN.ROUTE_ID, "SHIPOUT", Station.SFCDB).OrderByDescending(r => r.SEQ_NO).ToList();
                    objSN.CURRENT_STATION = listRoute.FirstOrDefault().STATION_NAME;
                    objSN.NEXT_STATION = "SHIPOUT";
                    objSN.SHIPPED_FLAG = "0";
                    objSN.SHIPDATE = null;
                    objSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                    objSN.EDIT_TIME = sysDateTime;
                    result = TRS.Update(objSN, Station.SFCDB);
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                    }

                    result = TRSD.CancelShip(Station.SFCDB, objShipDetail);
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL" }));
                    }

                    objStationDetail = TRSSD.GetDetailBySnAndStation(objSN.SN, "SHIPOUT", Station.SFCDB);
                    objStationDetail.SN = "RS_" + objStationDetail.SN;
                    result = TRSSD.Update(objStationDetail, Station.SFCDB);
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL" }));
                    }

                    objDNStatus.DN_FLAG = "0";
                    objDNStatus.EDITTIME = sysDateTime;
                    result = TRDS.Update(Station.SFCDB, objDNStatus);
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                    }
                    #endregion
                    break;
                case "PALLET":
                    #region Cancel Ship Out By Pallet
                    R_PACKING objPack = TRP.GetPackingByPackNo(inputValue, Station.SFCDB);
                    if (objPack == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { inputValue }));
                    }
                    listSN = TRP.GetSnListByPalletID(objPack.ID, Station.SFCDB);
                    if (listSN.Count == 0)
                    {
                        throw new MESReturnMessage($@"The Pallet {inputValue} Is Empty!");
                    }

                    listRoute = TCRD.GetLastStations(listSN.FirstOrDefault().ROUTE_ID, "SHIPOUT", Station.SFCDB).OrderByDescending(r => r.SEQ_NO).ToList();
                    current_station = listRoute.FirstOrDefault().STATION_NAME;
                    foreach (R_SN sn in listSN)
                    {
                        objShipDetail = TRSD.GetShipDetailBySN(Station.SFCDB, sn.SN);
                        if (objShipDetail == null)
                        {
                            throw new MESReturnMessage(sn.SN + " No Shipping Record!");
                        }
                        if (sn.NEXT_STATION != "SHIPFINISH" && sn.CURRENT_STATION != "SHIPOUT")
                        {
                            throw new MESReturnMessage(sn.SN + " Hasn't Been Shipped Yet!");
                        }
                        sn.CURRENT_STATION = current_station;
                        sn.NEXT_STATION = "SHIPOUT";
                        sn.SHIPPED_FLAG = "0";
                        sn.SHIPDATE = null;
                        sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                        sn.EDIT_TIME = sysDateTime;
                        result = TRS.Update(sn, Station.SFCDB);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN " + sn.SN }));
                        }

                        R_SN_STATION_DETAIL objStationDetail_P = TRSSD.GetDetailBySnAndStation(sn.SN, "SHIPOUT", Station.SFCDB);
                        objStationDetail_P.SN = "RS_" + objStationDetail_P.SN;
                        result = TRSSD.Update(objStationDetail_P, Station.SFCDB);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL " + sn.SN }));
                        }

                        dn_no = objShipDetail.DN_NO;
                        dn_line = objShipDetail.DN_LINE;
                        objDNStatus = TRDS.GetStatusByNOAndLine(Station.SFCDB, dn_no, dn_line);
                        if (objDNStatus.DN_FLAG == "3")
                        {
                            //做完GT不給退SHIPPING
                            throw new Exception("This " + dn_no + "," + dn_line + " Has Done GT!");
                        }

                        result = TRSD.CancelShip(Station.SFCDB, objShipDetail);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL " + sn.SN }));
                        }
                    }

                    objDNStatus = TRDS.GetStatusByNOAndLine(Station.SFCDB, dn_no, dn_line);
                    objDNStatus.DN_FLAG = "0";
                    objDNStatus.EDITTIME = sysDateTime;
                    result = TRDS.Update(Station.SFCDB, objDNStatus);
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                    }

                    #endregion
                    break;
                case "DN":
                    #region Cancel Ship Out By DN
                    if (sessionDNLine == null || sessionDNLine.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
                    }
                    dn_line = sessionDNLine.Value.ToString();
                    dn_no = inputValue;
                    objDNStatus = TRDS.GetStatusByNOAndLine(Station.SFCDB, dn_no, dn_line);
                    if (objDNStatus.DN_FLAG == "3")
                    {
                        //做完GT不給退SHIPPING
                        throw new Exception("This " + dn_no + "," + dn_line + " Has Done GT!");
                    }
                    List<R_SHIP_DETAIL> listShip = TRSD.GetShipDetailByDN(Station.SFCDB, dn_no, dn_line);
                    if (listShip.Count == 0)
                    {
                        throw new MESReturnMessage(dn_no + " No Shipping Record!");
                    }
                    foreach (R_SHIP_DETAIL sd in listShip)
                    {
                        objSN = TRS.LoadData(sd.SN, Station.SFCDB);
                        if (objSN.NEXT_STATION != "SHIPFINISH" && objSN.CURRENT_STATION != "SHIPOUT")
                        {
                            throw new MESReturnMessage(objSN.SN + " Hasn't Been Shipped Yet!");
                        }

                        listRoute = TCRD.GetLastStations(objSN.ROUTE_ID, "SHIPOUT", Station.SFCDB).OrderByDescending(r => r.SEQ_NO).ToList();
                        current_station = listRoute.FirstOrDefault().STATION_NAME;

                        objSN.CURRENT_STATION = current_station;
                        objSN.NEXT_STATION = "SHIPOUT";
                        objSN.SHIPPED_FLAG = "0";
                        objSN.SHIPDATE = null;
                        objSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                        objSN.EDIT_TIME = sysDateTime;
                        result = TRS.Update(objSN, Station.SFCDB);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN " + objSN.SN }));
                        }

                        objStationDetail = TRSSD.GetDetailBySnAndStation(objSN.SN, "SHIPOUT", Station.SFCDB);
                        objStationDetail.SN = "RS_" + objStationDetail.SN;
                        result = TRSSD.Update(objStationDetail, Station.SFCDB);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_STATION_DETAIL " + objSN.SN }));
                        }

                        result = TRSD.CancelShip(Station.SFCDB, sd);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SHIP_DETAIL " + objSN.SN }));
                        }
                    }
                    objDNStatus.DN_FLAG = "0";
                    objDNStatus.EDITTIME = sysDateTime;
                    result = TRDS.Update(Station.SFCDB, objDNStatus);
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_DN_STATUS" }));
                    }
                    #endregion
                    break;
                default:
                    throw new MESReturnMessage(type + ",Input Type Error!");
                    //break;
            }
            MESPubLab.WriteLog.WriteIntoMESLog(Station.SFCDB, Station.BU, "RETURN_SHIPPING", "ShippingAction", "ReturnShppingByTypeAction",
                        "Return Shpping", "", Station.LoginUser.EMP_NO, inputValue, "");

            Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
            {
                Message = "OK",
                State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
            });
        }

        public static void InsertHpeEdi856(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string packNo = string.Empty;
            if (Paras.Count == 0)
            {
                packNo = Input.Value.ToString();
            }
            else
            {
                MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (PackNoSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                }
                packNo = PackNoSession.Value.ToString();
            }
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
                   dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString(),
                   skuNo = Station.DisplayOutput.Find(t => t.Name == "SKU_NO").Value.ToString();
            if (dnNo.Length == 0 && dnLine.Length == 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801113040"));

            OleExec oleDB = null;
            HPE_SHIP_DATA hsd = new HPE_SHIP_DATA();
            R_DN_STATUS rds = new R_DN_STATUS();
            R_PACKING rp = new R_PACKING();
            R_SN rsn = new R_SN();
            List<string> snIdList = new List<string>();
            T_HPE_EDI_856 the856 = null;
            double? newFID = 0;
            MesAPIBase mesAPI = new MesAPIBase();
            bool check850 = false;
            string strRet = string.Empty;
            try
            {
                oleDB = Station.SFCDB;
                rds = oleDB.ORM.Queryable<R_DN_STATUS>().Where(r => r.DN_NO == dnNo && Convert.ToInt32(r.DN_LINE) == Convert.ToInt32(dnLine)).First();
                //check850 = oleDB.ORM.Queryable<HPE_EDI_850>().Where(h => h.F_PO == rds.PO_NO && SqlSugar.SqlFunc.ToInt32(h.F_LINE) == SqlSugar.SqlFunc.ToInt32(rds.PO_LINE)).Any();
                check850 = oleDB.ORM.Queryable<HPE_EDI_850>().Where(h => h.F_PO == rds.PO_NO).Any();
                if (check850)
                {
                    the856 = new T_HPE_EDI_856(oleDB, DB_TYPE_ENUM.Oracle);
                    Row_HPE_EDI_856 row = (Row_HPE_EDI_856)the856.NewRow();
                    hsd = oleDB.ORM.Queryable<HPE_SHIP_DATA>().Where(h => h.F_TO_DN == dnNo && Convert.ToInt32(h.F_TO_DN_LINE) == Convert.ToInt32(dnLine)).First();
                    if (hsd == null)
                        //throw new Exception("交管未維護該DN的HPE_SHIP_DATA信息，請聯繫交管維護。");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152116"));
                    var main = oleDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == hsd.F_PO_NO && t.POLINE == hsd.F_PO_LINE_NO).ToList().FirstOrDefault();
                    if (main.CANCEL == MesBool.Yes.ExtValue())
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152316", new string[] { hsd.F_PO_NO }));
                    //throw new Exception($@"po: {hsd.F_PO_NO} is cancel,pls check!");

                    var packagedata = oleDB.ORM.Queryable<R_SAP_PACKAGE>().Where(t => t.PN == main.PID).ToList().FirstOrDefault();
                    if (packagedata == null)
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152422", new string[] { main.PID }));
                    //throw new Exception($@"機種{main.PID} 在sap 未維護包規重量數據或Mes系統未下載,pls check!");
                    var dnpack = oleDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING>((sd, s, sp, p) => sd.SN == s.SN && s.ID == sp.SN_ID && sp.PACK_ID == p.ID).
                        Where((sd, s, sp, p) => s.VALID_FLAG == MesBool.Yes.ExtValue() && sd.DN_NO == rds.DN_NO && sd.DN_LINE == rds.DN_LINE).Select((sd, s, sp, p) => new { p, s }).ToList();
                    //when all ship then Send856
                    if (int.Parse(hsd.F_PO_LINE_QTY) != dnpack.Select(t => t.s.SN).Distinct().Count())
                        return;
                    var cartonList = dnpack.Select(t => t.p.PACK_NO).Distinct();
                    var palletList = dnpack.Select(t => t.p.PARENT_PACK_ID).Distinct();
                    double boxvol = 1, palletvol = 1;
                    packagedata.C_GROES.Split('*').ToList().ForEach(t => { boxvol = boxvol * (Convert.ToDouble(t) / 100); });
                    packagedata.P_GROES.Split('*').ToList().ForEach(t => { palletvol = palletvol * (Convert.ToDouble(t) / 100); });
                    //單一箱子毛重
                    var box_gw = Convert.ToDouble(packagedata.BOX_GW) - Convert.ToDouble(packagedata.PCS_GW);
                    //->get data from sap:净重*數量
                    var TO_NETWEIGHT = (Convert.ToDouble(packagedata.PCS_NT) * Convert.ToDouble(rds.QTY)).ToString("f3");
                    //->get data from sap:單一sn毛重*數量+箱子重量*箱數+棧板重量*棧板數
                    var TO_GROSSWEIGHT = (Convert.ToDouble(rds.QTY) * Convert.ToDouble(packagedata.PCS_GW) + box_gw * cartonList.Count() + Convert.ToDouble(packagedata.P_NULLWG) * palletList.Count()).ToString("f3");
                    //->get data from sap; 箱子體積* 箱數+棧板體積
                    //var TO_VOLUME = (boxvol * cartonList.Count() + boxvol * palletList.Count()).ToString("f3");
                    var TO_VOLUME = (boxvol * cartonList.Count() + +palletvol * palletList.Count()).ToString("f3");
                    var filename = $@"{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}{dnNo}";
                    foreach (var item in palletList)
                    {
                        var ctList = oleDB.ORM.Queryable<R_PACKING>().Where(r => r.PARENT_PACK_ID == item && r.PACK_TYPE == "CARTON").ToList();
                        for (int i = 0; i < ctList.Count(); i++)
                        {
                            snIdList = oleDB.ORM.Queryable<R_SN_PACKING>().Where(r => r.PACK_ID == ctList[i].ID).Select(r => r.SN_ID).ToList();
                            for (int j = 0; j < snIdList.Count; j++)
                            {
                                rsn = oleDB.ORM.Queryable<R_SN>().Where(r => r.ID == snIdList[j] && r.VALID_FLAG == "1").First();
                                string sn856 = rsn.SN.Split(' ')[0];//客人要求傳資料時去掉空格和PN，只傳SN  2022-02-12
                                newFID = oleDB.ORM.Queryable<HPE_EDI_856>().OrderBy(h => h.F_ID, SqlSugar.OrderByType.Desc).Select(h => h.F_ID).First();
                                newFID = newFID == null ? 1 : newFID + 1;

                                var obj = new HPE_EDI_856()
                                {
                                    ID = MesDbBase.GetNewID<HPE_EDI_856>(oleDB.ORM, Station.BU),
                                    F_ID = newFID,
                                    F_FILENAME = filename,//待寫取值，先設為空
                                    //F_TO_NO = hsd.F_TO_NO,
                                    //F_TO_NO= $@"{dnNo}{hsd.F_TO_DN_LINE}",
                                    F_TO_NO = $@"{dnNo}",
                                    F_TO_DATE = hsd.F_TO_DATE,
                                    F_TO_SHIPDATE = hsd.F_TO_SHIPDATE,
                                    F_TO_DN = dnNo,
                                    F_TO_DN_LINE = hsd.F_TO_DN_LINE,
                                    F_TO_PKG_QTY = ctList.Count().ToString("f3"),
                                    //F_TO_NETWEIGHT = (10).ToString("f3"),//待寫取值，先設為10 ->get data from sap:净重*數量
                                    F_TO_NETWEIGHT = TO_NETWEIGHT,
                                    //F_TO_GROSSWEIGHT = (27.5).ToString("f3"),//待寫取值，先設為27.5->get data from sap:單一sn毛重*數量+箱子重量*箱數
                                    F_TO_GROSSWEIGHT = TO_GROSSWEIGHT,
                                    //F_TO_VOLUME = (5.4).ToString("f3"),//待寫取值，先設為5.4->get data from sap;箱子體積*箱數+棧板體積
                                    F_TO_VOLUME = TO_VOLUME,
                                    F_TO_TRAILERNO = hsd.F_TO_TRAILERNO,
                                    F_CARRIER_TYPE = hsd.F_CARRIER_TYPE,
                                    F_CARRIER_CODE = hsd.F_CARRIER_CODE,
                                    F_CARRIER_TRAN_TYPE = hsd.F_CARRIER_TRAN_TYPE,
                                    F_CARRIER_REF_NO = hsd.F_CARRIER_REF_NO,
                                    F_CARRIER_TRAILER_NO = hsd.F_CARRIER_TRAILER_NO,
                                    F_ST_NAME = hsd.F_ST_NAME,
                                    F_ST_CONTACT = hsd.F_ST_CONTACT,
                                    F_ST_CONTACT_MAIL = hsd.F_ST_CONTACT_MAIL,
                                    F_ST_CUSTOMERCODE = hsd.F_ST_CUSTOMERCODE,
                                    F_ST_ADDRESS = hsd.F_ST_ADDRESS,
                                    F_ST_CITY = hsd.F_ST_CITY,
                                    F_ST_POSTCODE = hsd.F_ST_POSTCODE,
                                    F_ST_STATE_CODE = hsd.F_ST_STATE_CODE,
                                    F_ST_COUNTRY_CODE = hsd.F_ST_COUNTRY_CODE,
                                    F_PO_NO = hsd.F_PO_NO,
                                    F_PO_LINE_NO = hsd.F_PO_LINE_NO,
                                    F_PO_LINE_QTY = Convert.ToDecimal(hsd.F_PO_LINE_QTY).ToString("f3"),
                                    F_PO_DATE = hsd.F_PO_DATE,
                                    F_PKG_ID = ctList[i].PACK_NO,
                                    F_PKG_QTY = snIdList.Count.ToString("f3"),
                                    //F_PKG_GROSS_WEIGHT = (2.5).ToString("f3"),//待寫取值，先設為2.5->get data from sap;SN毛重+箱子毛重
                                    F_PKG_GROSS_WEIGHT = (snIdList.Count() * Convert.ToDouble(packagedata.PCS_GW) + box_gw).ToString("f3"),
                                    //F_PKG_DIMENSION = "240*500*150",//待寫取值，先設為空->get data from sap;
                                    F_PKG_DIMENSION = packagedata.C_GROES,
                                    F_ITEM_MPN = rsn.SKUNO,//先取機種
                                    F_ITEM_CPN = rsn.SKUNO,//先取機種
                                    //F_ITEM_SN = rsn.SN,
                                    F_ITEM_SN = sn856,//客人要求傳資料時去掉空格和PN，只傳SN  2022-02-12
                                    F_ITEM_COO = "VN",//後續有新的再作修改
                                    F_LASTEDITDT = mesAPI.GetDBDateTime(),
                                    F_INCOTERM = hsd.F_INCOTERM
                                };
                                //HPE 856數據只允許一個DN開一條line;
                                if (oleDB.ORM.Queryable<HPE_EDI_856>().Any(t => t.F_TO_NO == dnNo))
                                    throw new Exception($@"HPE856 DATA already exists DNNO:{dnNo},Pls check!");
                                oleDB.ORM.Insertable(obj).ExecuteCommand();
                            }
                        }
                    }
                    main.CLOSED = MesBool.Yes.ExtValue();
                    main.CLOSETIME = DateTime.Now;
                    #region PO status
                    oleDB.ORM.Updateable<O_PO_STATUS>().SetColumns(t => new O_PO_STATUS() { VALIDFLAG = MesBool.No.ExtValue() }).Where(t => t.POID == main.ID).ExecuteCommand();
                    var cpostatus = new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(oleDB.ORM, Customer.ARUBA.ExtValue()),
                        STATUSID = ENUM_ARUBA_PO_STATUS.ShipOut.ExtValue(),
                        VALIDFLAG = MesBool.Yes.ExtValue(),
                        CREATETIME = DateTime.Now,
                        EDITTIME = DateTime.Now,
                        POID = main.ID
                    };
                    oleDB.ORM.Insertable(cpostatus).ExecuteCommand();
                    oleDB.ORM.Updateable(main).ExecuteCommand();
                    #endregion
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //add by Winster in 2021/03/25,For Insert into data in JNPB2B R_JUNIPER_MFPACKINGLIST table
        public static void InsertJNPB2B(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession TrailerNumber = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrailerNumber == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession StrTONO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StrTONO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession TOWO = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (TOWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            MESStationSession Newpallet = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (Newpallet == null)
            {
                Newpallet.Value = PackNo.Value.ToString();
            }

            int ICtnqty = 0;
            string StrWeight = "0";
            string StrCOO = "", sql = "", StrNewpl = "";
            OleExec sfcdb = Station.SFCDB;
            DataTable dt = new DataTable();

            T_R_PACKING trpack = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_WEIGHT PLweight = new T_R_WEIGHT(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_JUNIPER_MFPACKINGLIST JNPpackLinst = new T_R_JUNIPER_MFPACKINGLIST(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            StrNewpl = Newpallet.Value.ToString();

            //Check and Get Pallet the CartonQTY
            ICtnqty = trpack.JNPGetCTNqty(PackNo.Value.ToString(), Station.BU, Station.SFCDB);
            if (ICtnqty == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210406172820"));
            }

            //Sum all Carton Weight in PalletID
            StrWeight = PLweight.GetallCTNweight(PackNo.Value.ToString(), Station.BU, Station.SFCDB);
            if (StrWeight == "0" || StrWeight == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210406172820"));
            }

            //Get COO,if PalletID have more COO,Then take the first one COO 

            sql = $@"SELECT * FROM (
            SELECT distinct LOCATION FROM r_sn_kp WHERE r_sn_id in(
            SELECT sn_id FROM R_SN_PACKING WHERE PACK_ID IN(
            SELECT ID FROM r_packing WHERE parent_pack_id in(
            SELECT ID FROM r_packing WHERE pack_no ='{PackNo.Value.ToString()}')
            ))and partno in( SELECT PID FROM o_order_main WHERE prewo='{TOWO.Value.ToString()}' ) 
            ) A WHERE rownum<2  ";
            dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                StrCOO = dt.Rows[0]["LOCATION"].ToString();
            }
            else
            {
                StrCOO = "";
            }

            if (StrCOO == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210406172820"));
            }

            bool InsertFlag = JNPpackLinst.InsertB2Bdata(Station.BU, StrTONO.Value.ToString(), ICtnqty, StrWeight, StrCOO, TrailerNumber.Value.ToString(), PackNo.Value.ToString(), StrNewpl, Station.LoginUser.EMP_NO, Station.SFCDB);

            if (InsertFlag == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329153055"));
            }
        }

        public static void ClosedB2BFlag(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession TOFLAG = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOFLAG == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            string StrTO = "";
            bool UpadteB2BFlag = false;
            T_R_JUNIPER_MFPACKINGLIST B2BFLAG = new T_R_JUNIPER_MFPACKINGLIST(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            StrTO = TONO.Value.ToString();

            if (TOFLAG.Value.ToString() == "Y")
            {
                UpadteB2BFlag = B2BFLAG.ClosedTOB2BFlag(StrTO, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);


                if (UpadteB2BFlag == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329154548"));
                }
                else
                {
                    Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                    {
                        Message = "OK",
                        State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                    });
                }
            }
            else
            {
                Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                {
                    Message = "OK",
                    State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                });
            }
        }

        public static void GenerateTONOtruck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null)
            {
                TONO = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = Paras[0].VALUE };
                Station.StationSession.Add(TONO);
            }
            else
            {
                TONO.Value = Paras[0].VALUE;
            }

            MESStationSession TOQTY = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOQTY == null)
            {
                TOQTY = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = Paras[1].VALUE };
                Station.StationSession.Add(TOQTY);
            }
            else
            {
                TOQTY.Value = Paras[1].VALUE;
            }

            try
            {
                Row_R_JUNIPER_TRUCKLOAD_TO rowTono = null;
                T_R_JUNIPER_TRUCKLOAD_TO JNPGenerateTO = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                bool GenerateFlag = JNPGenerateTO.GenerateTONO(Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);

                if (GenerateFlag == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329153753"));
                }
                else
                {
                    rowTono = JNPGenerateTO.GetByTONo(Station.SFCDB);
                }

                TONO.Value = rowTono.TO_NO;
                TOQTY.Value = rowTono.QTY;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void OpenTONOtruck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            T_R_JUNIPER_TRUCKLOAD_TO JNPGenerateTO = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            bool GenerateFlag = JNPGenerateTO.UpdateTONO("OPEN", TONO.Value.ToString(), Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);

            if (GenerateFlag == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329154030"));
            }
            else
            {
                Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                {
                    Message = "OK",
                    State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                });
            }
        }

        public static void ClosedTONOtruck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            string StrTO = "";
            StrTO = TONO.Value.ToString();
            Row_R_JUNIPER_TRUCKLOAD_TO rowTono = null;
            T_R_JUNIPER_TRUCKLOAD_TO JNPGenerateTO = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            if (StrTO == null || StrTO == "" || StrTO.Length <= 0)
            {
                rowTono = JNPGenerateTO.GetByTONo(Station.SFCDB);
                StrTO = rowTono.TO_NO;
                if (StrTO == null || StrTO == "" || StrTO.Length <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329154311"));
                }
            }
            bool GenerateFlag = JNPGenerateTO.UpdateTONO("CLOSED", StrTO, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);

            if (GenerateFlag == false)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329154548"));
            }
            else
            {
                Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                {
                    Message = "OK",
                    State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                });
            }
        }

        public static void InsertruckDetail(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 9)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession TrailerNumber = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrailerNumber == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TONO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession TOQTY = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (TOQTY == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            MESStationSession TOWO = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (TOWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }

            MESStationSession TOSKUNO = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (TOSKUNO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
            }
            MESStationSession DELIVERYNUM = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (DELIVERYNUM == null)
            {
                DELIVERYNUM = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, SessionKey = Paras[6].SESSION_KEY, Value = Paras[6].VALUE };
                Station.StationSession.Add(DELIVERYNUM);
            }
            else
            {
                DELIVERYNUM.Value = Paras[6].VALUE;
            }
            MESStationSession Newpallet = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            if (Newpallet == null)
            {
                Newpallet.Value = PackNo.Value.ToString();
            }
            MESStationSession NewpalletQty = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            if (NewpalletQty == null)
            {
                NewpalletQty.Value = "0";
            }

            int qtys = 0;
            string ErrMessage = "", StrNewPL = "", StrNewPLQTY = "";
            string StrWO = "", sql = "", StrDNNO = "", Strqty = "", PLqty = "";
            OleExec sfcdb = Station.SFCDB;
            DataTable dt = new DataTable();
            R_JUNIPER_TRUCKLOAD_TO TOqty = null;
            T_R_JUNIPER_TRUCKLOAD_DETAIL JNPTruckDetail = new T_R_JUNIPER_TRUCKLOAD_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_JUNIPER_TRUCKLOAD_TO JNPTruckUpdateTO = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            StrWO = TOWO.Value.ToString();
            Strqty = TOQTY.Value.ToString();
            StrNewPL = Newpallet.Value.ToString();
            StrNewPLQTY = NewpalletQty.Value.ToString();
            TOqty = JNPTruckUpdateTO.CheckTOqty(TONO.Value.ToString(), Station.BU, Station.SFCDB);

            if (StrNewPLQTY != "0")
            {
                StrNewPLQTY = Convert.ToString(JNPTruckDetail.GetNewPLQty(TONO.Value.ToString(), StrNewPL, Station.BU, Station.SFCDB));
                StrNewPLQTY = Convert.ToString(Convert.ToInt32(StrNewPLQTY) + 1);
            }
            else
            {
                StrNewPLQTY = Convert.ToString(Convert.ToInt32(StrNewPLQTY) + 1);
            }

            if (TOqty == null)
            {
                PLqty = "0";
            }
            else
            {
                PLqty = TOqty.QTY.ToString();
            }

            if (Strqty != PLqty)
            {
                qtys = Convert.ToInt32(PLqty) + 1;
            }
            else
            {
                qtys = Convert.ToInt32(Strqty) + 1;
            }

            sql = $@"SELECT DISTINCT DELIVERYNUMBER
                      FROM R_I282
                     WHERE MESSAGEID IN
                           (SELECT PREASN FROM O_ORDER_MAIN WHERE PREWO = '{StrWO}')
                       AND ROWNUM < 2
                    UNION ALL
                    SELECT DISTINCT DELIVERYNUMBER
                      FROM R_JNP_DOA_SHIPMENTS_ACK
                     WHERE ASNNUMBER IN
                           (SELECT PREASN FROM O_ORDER_MAIN WHERE PREWO = '{StrWO}')
                       AND ROWNUM < 2";
            dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                StrDNNO = dt.Rows[0]["DELIVERYNUMBER"].ToString();
                if (StrDNNO == "" && StrDNNO == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329162803", new string[] { PackNo.Value.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }
                DELIVERYNUM.Value = StrDNNO;

                bool InserDetailFlag = JNPTruckDetail.InserTODetail(TONO.Value.ToString(), PackNo.Value.ToString(), TOSKUNO.Value.ToString(), TrailerNumber.Value.ToString(), StrDNNO, StrNewPL, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);

                if (InserDetailFlag == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329162018"));
                }

                bool UpdateTOFlag = JNPTruckUpdateTO.UpdateTOQTY(TONO.Value.ToString(), qtys, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);

                if (UpdateTOFlag == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329162253"));
                }

                if (InserDetailFlag == true && UpdateTOFlag == true)
                {
                    Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                    {
                        Message = "OK",
                        State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                    });
                    TOQTY.Value = qtys.ToString();
                    NewpalletQty.Value = StrNewPLQTY.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329162443"));
                }
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329162803", new string[] { PackNo.Value.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void TruckLoadCall311(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession TOFLAG = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOFLAG == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            string msg = "";
            string StrTO = "", StrBU = "", StrPlant = "";
            DataTable dt = new DataTable();
            R_JUNIPER_TRUCKLOAD_TO DetailData = null;
            T_R_JUNIPER_TRUCKLOAD_TO JNPDetail = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            StrTO = TONO.Value.ToString();
            DetailData = JNPDetail.GetDetailData(StrTO, Station.BU, Station.SFCDB);

            if (DetailData != null)
            {
                StrBU = DetailData.BU;
                StrPlant = DetailData.PLANT;
            }

            if (StrBU == "")
            {
                StrBU = "FJZ";
            }
            if (StrPlant == "")
            {
                StrPlant = "MBGA";
            }

            if (TOFLAG.Value.ToString() == "Y")
            {
                var logdb = Station.DBS["SFCDB"].Borrow();
                try
                {
                    if (InterfacePublicValues.IsMonthly(Station.SFCDB, DB_TYPE_ENUM.Oracle))
                    {
                        R_MES_LOG log = new R_MES_LOG()
                        {
                            ID = MesDbBase.GetNewID<R_MES_LOG>(logdb.ORM, StrBU),
                            PROGRAM_NAME = "TruckLoadMonthly",
                            CLASS_NAME = "MESStation.Stations.StationActions.ActionRunners",
                            FUNCTION_NAME = "ShippingAction.TruckLoadCall311",
                            MAILFLAG = "N",
                            LOG_MESSAGE = msg,
                            DATA1 = StrTO,
                            DATA2 = ConfigurationManager.AppSettings[Station.BU + "_TRUCK311FROM"],
                            DATA3 = ConfigurationManager.AppSettings[Station.BU + "_TRUCK311TO"],
                            EDIT_EMP = Station.LoginUser.EMP_NO,
                            EDIT_TIME = DateTime.Now,
                            DATA4 = "N"
                        };
                        logdb.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                        Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                        {
                            Message = "OK,Monthly!SAP 311 In Shipout Confirm!",
                            State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                        });
                    }
                    else
                    {


                        msg = Gt(StrBU, StrTO, StrPlant, Station.SFCDB, out dt);

                        if (msg.StartsWith("OK"))
                        {
                            R_MES_LOG log = new R_MES_LOG()
                            {
                                ID = MesDbBase.GetNewID<R_MES_LOG>(logdb.ORM, StrBU),
                                PROGRAM_NAME = "TruckLoadGT",
                                CLASS_NAME = "MESStation.Stations.StationActions.ActionRunners",
                                FUNCTION_NAME = "ShippingAction.TruckLoadCall311",
                                MAILFLAG = "N",
                                LOG_MESSAGE = msg,
                                DATA1 = StrTO,
                                EDIT_EMP = Station.LoginUser.EMP_NO,
                                EDIT_TIME = DateTime.Now,
                                DATA4 = "Y"
                            };
                            logdb.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();

                            Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                            {
                                Message = "OK" + msg,
                                State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                            });
                        }
                        else
                        {
                            R_MES_LOG log = new R_MES_LOG()
                            {
                                ID = MesDbBase.GetNewID<R_MES_LOG>(logdb.ORM, StrBU),
                                PROGRAM_NAME = "TruckLoadGT",
                                CLASS_NAME = "MESStation.Stations.StationActions.ActionRunners",
                                FUNCTION_NAME = "ShippingAction.TruckLoadCall311",
                                MAILFLAG = "N",
                                LOG_MESSAGE = msg,
                                DATA1 = StrTO,
                                EDIT_EMP = Station.LoginUser.EMP_NO,
                                EDIT_TIME = DateTime.Now,
                                DATA4 = "N"
                            };
                            logdb.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();

                            throw new MESReturnMessage("FAIL" + msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage("FAIL" + ex + msg);
                }
                finally
                {
                    Station.DBS["SFCDB"].Return(logdb);
                }
            }
            else
            {
                Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                {
                    Message = "OK",
                    State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                });
            }
        }

        public static void RemovePallet(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNo == null || PackNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession newPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (newPalletNo == null || newPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TONO == null || TONO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession TOQTY = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (TOQTY == null)
            {
                TOQTY = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = "0" };
                Station.StationSession.Add(TOQTY);
            }

            MESStationSession NewpalletQty = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (NewpalletQty == null)
            {
                NewpalletQty = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, Value = "0" };
                Station.StationSession.Add(NewpalletQty);
            }
            var tono = TONO.Value.ToString();
            var packno = PackNo.Value.ToString();
            var newpackno = newPalletNo.Value.ToString();
            var _db = Station.SFCDB.ORM;
            var toHead = _db.Queryable<R_JUNIPER_TRUCKLOAD_TO>().Where(t => t.TO_NO == tono).First();
            if (toHead == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { "TO_NO ", tono });
                throw new MESReturnMessage(ErrMessage);
            }
            if (toHead.CLOSED != "0")
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE_TRUCK_TOSTATUS", new string[] { tono, " been closed" });
                throw new MESReturnMessage(ErrMessage);
            }
            var toDetail = _db.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL>().Where(t => t.TO_NO == tono && t.PACK_NO == packno).First();
            if (toDetail == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144328", new string[] { "PACKNO:" + packno, "TO_NO:" + tono });
            }
            var isugar = _db.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((S, SP, P1, P2) => S.ID == SP.SN_ID && SP.PACK_ID == P1.ID && P1.PARENT_PACK_ID == P2.ID)
                .Where((S, SP, P1, P2) => P2.PACK_NO == packno);
            var wo = isugar.Select((S, SP, P1, P2) => S.WORKORDERNO).First();
            var qty = isugar.Select((S, SP, P1, P2) => S.SN).Distinct().Count();
            UIInputData snInput = new UIInputData()
            {
                MustConfirm = true,
                Timeout = 30000,
                IconType = IconType.None,
                UIArea = new string[] { "30%", "25%" },
                Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162335"),
                Tittle = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162335"),
                Type = UIInputType.String,
                Name = "SN",
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153928"),
            };
            string sn = null;
            while (sn == null)
            {
                var strsn = snInput.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                if (strsn != "NO INPUT")
                {
                    var rmovePrfex = strsn.Substring(1);
                    sn = isugar.Where((S, SP, P1, P2) => S.SN == strsn || S.SN == rmovePrfex).Select((S, SP, P1, P2) => S.SN).First();
                    if (sn == null)
                    {
                        snInput.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20181008115525", new string[] { strsn });
                    }
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102505"));
                }
            }

            var coo = _db.Queryable<R_SN_KP>().Where(t => t.SN == sn && t.SCANTYPE == "SN" && t.KP_NAME == "AutoKP").Select(t => t.LOCATION).First();
            var b2bData = _db.Queryable<R_JUNIPER_MFPACKINGLIST>().Where(t => t.INVOICENO == tono && t.PALLETID == newpackno && t.WORKORDERNO == wo && t.QUANTITY == qty && t.COO == coo).First();

            try
            {
                toHead.QTY = toHead.QTY - 1;
                _db.Updateable(toHead).ExecuteCommand();
                _db.Deleteable(toDetail).ExecuteCommand();
                _db.Deleteable(b2bData).ExecuteCommand();

                R_MES_LOG log = new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(_db, Station.BU),
                    PROGRAM_NAME = "TruckLoadRemovePallet",
                    CLASS_NAME = "MESStation.Stations.StationActions.ActionRunners",
                    FUNCTION_NAME = "ShippingAction.RemovePallet",
                    MAILFLAG = "N",
                    LOG_MESSAGE = "",
                    DATA1 = tono,
                    DATA2 = newpackno,
                    DATA3 = packno,
                    EDIT_EMP = Station.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now,
                    DATA4 = "N"
                };
                _db.Insertable<R_MES_LOG>(log).ExecuteCommand();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Gt(string bu, string TO_NO, string Plant, OleExec SFCDB, out DataTable odt)
        {
            if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
            {
                odt = new DataTable();
                return "This time is monthly,can't BackFlush";
            }
            else
            {
                var dt = SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL, R_PRE_WO_HEAD, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((T, W, S, SP, P1, P2) =>
                     T.PACK_NO == P2.PACK_NO &&
                     W.WO == S.WORKORDERNO &&
                     S.ID == SP.SN_ID &&
                     SP.PACK_ID == P1.ID &&
                     P1.PARENT_PACK_ID == P2.ID)
                .Where((T, W, S, SP, P1, P2) => T.TO_NO == TO_NO)
                .GroupBy((T, W, S, SP, P1, P2) => new { T.TRAILER_NUM, W.GROUPID })
                .Select((T, W, S, SP, P1, P2) => new { T.TRAILER_NUM, W.GROUPID, QUANTITY = SqlSugar.SqlFunc.AggregateCount<string>(S.SN) })
                .ToDataTable();
                ZCMM_NSBG_0013 rfc = new ZCMM_NSBG_0013(bu);

                string postday = InterfacePublicValues.GetPostDate(SFCDB);

                string regex = "(\\d+)/(\\d+)/(\\d+)";
                var m = Regex.Match(postday, regex);
                if (m.Success)
                {
                    postday = m.Groups[3].Value + "-" + m.Groups[1].Value + "-" + m.Groups[2].Value;
                }

                rfc.SetValue(TO_NO, ConfigurationManager.AppSettings[bu + "_TRUCK311FROM"], ConfigurationManager.AppSettings[bu + "_TRUCK311TO"], Plant, postday, dt);
                rfc.CallRFC();
                var flag = rfc.GetValue("O_FLAG");
                odt = rfc.GetTableValue("OUT_TAB");
                if (flag == "0")
                {
                    var doc = rfc.GetValue("O_MBLNR");
                    odt = rfc.GetTableValue("OUT_TAB");
                    return "OK," + doc;
                }
                else
                {
                    var msg = rfc.GetValue("O_MESSAGE");
                    return msg;
                }
            }
        }

        public static void CombineShipoutAction(MESStationBase Station,MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession packNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (packNoSession == null || packNoSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession shipDataSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (shipDataSession == null || shipDataSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            string packNo = packNoSession.Value.ToString();
            DataTable shipDataTable = (DataTable)shipDataSession.Value;
            if (shipDataTable.Rows.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801113040"));
            }

            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            var rSnList = new List<R_SN>();
            t_r_packing.GetSnListByPackNo(packNo, ref rSnList, Station.SFCDB);
            rSnList.OrderBy(r => r.EDIT_TIME);
            int startIndex = 0;
            
            foreach (DataRow row in shipDataTable.Rows)
            {
                var dnNo = row["DN_NO"].ToString();
                var dnLine = row["DN_ITEM"].ToString();
                var dnObj = Station.SFCDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine).ToList().FirstOrDefault();
                if (dnObj == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731133647", new string[] { dnNo, dnLine }));
                }
                if(dnObj.DN_FLAG.Equals(ENUM_R_DN_STATUS.DN_WAIT_CQA.ExtValue()))
                {
                    continue;
                }
                int dnQty = Convert.ToInt32(dnObj.QTY);
                int shipedQty = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(x => x.DN_NO == dnObj.DN_NO && x.DN_LINE == dnObj.DN_LINE).ToList().Count();
                int balanceQty = dnQty - shipedQty;
                if (balanceQty == 0)
                {
                    continue;
                }
                List<R_SN> dnSnList = rSnList.Skip(startIndex).Take(balanceQty).ToList();
                if (dnSnList.Count == 0)
                {
                    break;
                }
                startIndex += balanceQty;
                DateTime sysdate = Station.SFCDB.ORM.GetDate();
                foreach (var snObj in dnSnList)
                {
                    Station.SFCDB.ORM.Insertable<R_SHIP_DETAIL>(new R_SHIP_DETAIL()
                    {
                        ID = snObj.ID,
                        SN = snObj.SN,
                        SKUNO = snObj.SKUNO,
                        DN_NO = dnObj.DN_NO,
                        DN_LINE = dnObj.DN_LINE,
                        SHIPDATE = sysdate,
                        CREATEBY = Station.LoginUser.EMP_NO,
                        SHIPORDERID = dnObj.ID
                    }).ExecuteCommand();
                }

                var rShipDetails = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(x => x.DN_NO == dnObj.DN_NO && x.DN_LINE == dnObj.DN_LINE).ToList();
                if (rShipDetails.Count > dnObj.QTY)
                {
                    throw new Exception("shipping sn qty more then dn qty.");
                }                  
                else if (rShipDetails.Count == dnObj.QTY)
                {
                    dnObj.DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_CQA.ExtValue();
                    dnObj.EDITTIME = sysdate;
                    Station.SFCDB.ORM.Updateable(dnObj).Where(r => r.ID == dnObj.ID).ExecuteCommand();
                    Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180802154903", new string[] { dnNo, dnLine }) });
                }
                t_r_sn.LotsPassStation(dnSnList, Station.Line, Station.StationName, Station.StationName, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);

                #region combine log
                R_MES_LOG log = new R_MES_LOG();
                log.ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_MES_LOG");
                log.PROGRAM_NAME = "MESStation";
                log.CLASS_NAME = "MESStation.Stations.StationActions.ActionRunners.ShippingAction";
                log.FUNCTION_NAME = "CombineShipoutAction";
                log.LOG_MESSAGE = "CombineShipout";
                log.EDIT_EMP = Station.LoginUser.EMP_NO;
                log.EDIT_TIME = sysdate;
                log.DATA1 = dnNo;
                log.DATA2 = dnLine;
                log.DATA3 = packNo;
                Station.SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                #endregion
            }

            #region   update output table data
            DataTable newShipDataTable = shipDataTable.Copy();
            newShipDataTable.Clear();
            int shippedDnTotal = 0;
            foreach (DataRow row in shipDataTable.Rows)
            {
                var dn = row["DN_NO"].ToString();
                var line = row["DN_ITEM"].ToString();
                var dnData = Station.SFCDB.ORM.Queryable<R_DN_STATUS, R_TO_DETAIL>((rds, rtd) => rds.DN_NO == rtd.DN_NO)
                    .Where((rds, rtd) => rds.DN_NO == dn && rds.DN_LINE == line)
                    .Select((rds, rtd) => new { rds, rtd.TO_NO }).ToList();
                var shipDetail = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(x => x.DN_NO == dn && x.DN_LINE == line).ToList();
                DataRow newRow = newShipDataTable.NewRow();
                newRow["TO_NO"] = dnData.FirstOrDefault().TO_NO;
                newRow["DN_NO"] = dnData.FirstOrDefault().rds.DN_NO;
                newRow["DN_ITEM"] = dnData.FirstOrDefault().rds.DN_LINE;
                newRow["SKU_NO"] = dnData.FirstOrDefault().rds.SKUNO;
                newRow["GT_QTY"] = dnData.FirstOrDefault().rds.QTY;
                newRow["REAL_QTY"] = shipDetail.Count;
                newShipDataTable.Rows.Add(newRow);
                if (dnData.FirstOrDefault().rds.QTY.Equals(shipDetail.Count))
                {
                    shippedDnTotal++;
                }
            }
            if(newShipDataTable.Rows.Count.Equals(shippedDnTotal))
            {
                //All DN ship finish,reset output data
                newShipDataTable.Clear();
                var res = Station.SFCDB.ORM
                    .Queryable<R_TO_HEAD, R_TO_DETAIL, R_DN_STATUS>((rth, rtd, rds) =>
                        rth.TO_NO == rtd.TO_NO && rtd.DN_NO == rds.DN_NO && rds.DN_FLAG == "0")
                    .OrderBy((rth) => rth.TO_CREATETIME, SqlSugar.OrderByType.Desc)
                    .GroupBy((rth, rtd, rds) => new { rth.TO_NO, rtd.TO_ITEM_NO, rth.TO_CREATETIME, rds.DN_NO, rds.SKUNO })
                    .Select((rth, rtd, rds) => new { rth.TO_NO, rtd.TO_ITEM_NO, rth.TO_CREATETIME, rds.DN_NO, rds.SKUNO })
                    .Distinct().ToList();

                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == "TO_LIST");
                s.Value = res;
            }            
            shipDataSession.Value = newShipDataTable;
            Station.NextInput = Station.Inputs.Find(t => t.DisplayName == "PACKNO");
            #endregion
        }

        public static void AutoCombineShipoutAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                string packNo = string.Empty;
                if (Paras.Count == 0)
                {
                    packNo = Input.Value.ToString();
                }
                else
                {
                    MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                    if (PackNoSession == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                    }
                    packNo = PackNoSession.Value.ToString();
                }
                string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
                    dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString(),
                     skuNo = Station.DisplayOutput.Find(t => t.Name == "SKU_NO").Value.ToString();


                var inputDnObj = Station.SFCDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine).ToList().FirstOrDefault();
                if (inputDnObj.DN_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180802154903", new string[] { dnNo, dnLine }));
                }

                T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
                var snList = new List<R_SN>();
                TRP.GetSnListByPackNo(packNo, ref snList, Station.SFCDB);
                if (snList.Count == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { packNo }));
                }
                List<R_DN_STATUS> dnList = new List<R_DN_STATUS>();
                T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                dnList.Add(inputDnObj);
                T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
                bool bCombinedSkuno = t_r_function_control.CheckUserFunctionExist("CombineDNToShipout", "SKUNO", skuNo, Station.SFCDB);
                bool bCombinedAll = t_r_function_control.CheckUserFunctionExist("CombineDNToShipout", "SKUNO", "ALL", Station.SFCDB);
                if (bCombinedAll || bCombinedSkuno)
                {
                    var toObj = Station.SFCDB.ORM.Queryable<R_TO_DETAIL>().Where(r => r.DN_NO == dnNo).ToList().FirstOrDefault();
                    var otherDnList = Station.SFCDB.ORM.Queryable<R_DN_STATUS, R_TO_DETAIL>((d, t) => d.DN_NO == t.DN_NO)
                        .Where((d, t) => d.SKUNO == skuNo && d.DN_NO != dnNo  && t.TO_NO == toObj.TO_NO && d.DN_FLAG == "0")
                        .OrderBy((d, t) => t.TO_ITEM_NO).Select((d, t) => d).ToList();
                    dnList.AddRange(otherDnList);
                }
                List<R_SN> shippedList = new List<R_SN>();
                string currentDN = "", currentDNLine = "", currentQTQty = "";
                foreach (var item in dnList)
                {
                    List<R_SN> balanceSNList = snList.FindAll(r => !shippedList.Contains(r)).ToList();
                    var dnShippedList = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(x => x.DN_NO == item.DN_NO && x.DN_LINE == item.DN_LINE).ToList();
                    int shipToDnQty = (int)inputDnObj.QTY - dnShippedList.Count;
                    List<R_SN> shipToDnList = balanceSNList.Take(shipToDnQty).ToList();
                    if (shipToDnList.Count == 0)
                    {
                        break;
                    }
                    rSn.DoPalletShipOutRecord(packNo, shipToDnList, Station.LoginUser.EMP_NO, Station.Line, Station.BU, Station.StationName, item, Station.SFCDB);
                    if (item.DN_FLAG == "1")
                    {
                        Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180802154903", new string[] { dnNo, dnLine }) });
                    }
                    currentDN = item.DN_NO;
                    currentDNLine = item.DN_LINE;
                    currentQTQty = item.QTY.ToString();
                    shippedList.AddRange(shipToDnList);
                }
                if (shippedList.Count != snList.Count)
                {                    
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20220103152630", new string[] { packNo, snList.Count.ToString() }));
                }

                Station.StationSession.Find(x => x.MESDataType == "DN_NO").Value = currentDN;
                Station.StationSession.Find(x => x.MESDataType == "DN_ITEM").Value = currentDNLine;
                Station.StationSession.Find(x => x.MESDataType == "GT_QTY").Value = currentQTQty;
                Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value = currentDN ;
                Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value = currentDNLine;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HWD OQC_CHECK過站Action
        /// </summary>
        public static void OQCCheckPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            MESStationSession sessionOrderNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOrderNo == null || sessionOrderNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string OrderNo = sessionOrderNo.Value.ToString();

            MESStationSession sessionPallet_Sn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPallet_Sn == null || sessionPallet_Sn.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string Pallet_Sn = sessionPallet_Sn.Value.ToString();

            MESStationSession sessionCarton_Sn = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionCarton_Sn == null || sessionCarton_Sn.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            string Carton_Sn = sessionCarton_Sn.Value.ToString();

            MESStationSession sessionIMEI = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionIMEI == null || sessionIMEI.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            string IMEI = sessionIMEI.Value.ToString();

            MESStationSession ClearInputSession = null;
            if (Paras.Count >= 5)
            {
                ClearInputSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[4].SESSION_TYPE) && t.SessionKey.Equals(Paras[4].SESSION_KEY));
                if (ClearInputSession == null)
                {
                    ClearInputSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                    Station.StationSession.Add(ClearInputSession);
                }
            }



            try
            {
                T_R_SN_DELIVER_INFO T_DELIVER = new T_R_SN_DELIVER_INFO(Station.SFCDB, Station.DBType);
                Station.SFCDB.ORM.Updateable<R_SN_DELIVER_INFO>().Where(t => t.PALLET_SN == Pallet_Sn && t.VALID_FLAG == "1");

                List<R_SN_DELIVER_INFO> DELIVERList = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.PALLET_SN == Pallet_Sn && t.VALID_FLAG == "1").ToList();
                if (DELIVERList.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { Pallet_Sn }));
                }
               
                if (DELIVERList.FindAll(t => t.NEXT_STATION != Station.StationName).Count > 0)
                {
                    throw new MESReturnMessage(Pallet_Sn + "中SN待過站錯誤!");
                }
               

                foreach (R_SN_DELIVER_INFO DELIVER in DELIVERList)
                {
                    T_DELIVER.ChangeSnStatus(DELIVER, Station.StationName, Station.LoginUser.EMP_NO, Station.SFCDB);

                }

                //Station.StationMessages.Add(new StationMessage()
                //{
                //    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20211203161554",
                //    new string[] { IMEI, Station.StationName }),
                //    State = StationMessageState.Message
                //});


                if (ClearInputSession != null)
                {
                    ClearInputSession.Value = "true";
                }
                 
            }
            catch (Exception ex)
            {
                throw new Exception("DELIVERCBSPassAction Error!" + ex.Message);
            }
        }

        /// <summary>
        /// HWD DELIVER_CHECK過站Action
        /// </summary>
        public static void DeliverCheckPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionOrderNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOrderNo == null || sessionOrderNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string OrderNo = sessionOrderNo.Value.ToString();

            MESStationSession sessionPallet_Sn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPallet_Sn == null || sessionPallet_Sn.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string Pallet_Sn = sessionPallet_Sn.Value.ToString();

            string OrderNos = OrderNo.Replace(",", "','");
            OrderNos = "'" + OrderNo + "'";

            try
            {
                T_R_SN_DELIVER_INFO T_DELIVER = new T_R_SN_DELIVER_INFO(Station.SFCDB, Station.DBType);
                Station.SFCDB.ORM.Updateable<R_SN_DELIVER_INFO>().Where(t => t.PALLET_SN == Pallet_Sn && t.VALID_FLAG == "1");

                List<R_SN_DELIVER_INFO> DELIVERList = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.PALLET_SN == Pallet_Sn && t.VALID_FLAG == "1").ToList();
                if (DELIVERList.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { Pallet_Sn }));
                }

                if (DELIVERList.FindAll(t => t.NEXT_STATION != Station.StationName).Count > 0)
                {
                    throw new MESReturnMessage(Pallet_Sn + "中SN待過站錯誤!");
                }


                foreach (R_SN_DELIVER_INFO DELIVER in DELIVERList)
                {
                    T_DELIVER.ChangeSnStatus(DELIVER, Station.StationName, Station.LoginUser.EMP_NO, Station.SFCDB);

                } 

            }
            catch (Exception ex)
            {
                throw new Exception("DELIVERCBSPassAction Error!" + ex.Message);
            }
        }


    }
}
