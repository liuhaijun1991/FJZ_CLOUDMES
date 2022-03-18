using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.Packing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module.HWD;
using static MESDataObject.Common.EnumExtensions;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.MESStation.MESReturnView.Station;



namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckShipping
    {
        /// <summary>
        /// HWT Shipping Station Check 系統沒有配置是否需要生產ASN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTControlASNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_C_CONTROL TCC = new T_C_CONTROL(Station.SFCDB, Station.DBType);
            C_CONTROL control = TCC.GetControlObject(Station.SFCDB, "SHIPPING", "ASN");
            if (control == null)
            {
                //throw new MESReturnMessage("系統沒有配置是否需要生產ASN！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155629"));
            }
        }

        /// <summary>
        /// HWT 楊兵要求(華為系統有卡關同一條ASN裡面，同一個TO同一個SO_NO同一個SO_ITEM_NO行只能存在一個DN，重複出現就會預約失敗)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTTOAndSOMappingChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionDNNO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDNNO == null || sessionDNNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionTONO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTONO == null || sessionTONO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            string dn_no = sessionDNNO.Value.ToString();
            string to_no = sessionTONO.Value.ToString();
            #region 原邏輯
            //SELECT COUNT(*)
            // INTO var_rowcount
            // FROM(SELECT(so_no || so_item_no), COUNT(DISTINCT a.dn_no) AS tt
            //         FROM r_dn_detail a, r_to_detail b
            //        WHERE a.dn_no = b.dn_no
            //          AND b.to_no = var_tono
            //        GROUP BY so_no || so_item_no)
            //WHERE tt > 1;
            //         IF var_rowcount > 0 THEN
            //           SELECT so_no, so_item_no
            //             INTO var_so_no, var_so_item_no
            //             FROM r_dn_detail
            //            WHERE dn_no = var_dnno;
            //         var_mes:= '同TO' || var_tono || '同SO_NO' || var_so_no || '同SO_ITEM_NO' ||
            //                   var_so_item_no || '存在多個DN,需開立唯一DN,請聯係交管重開DN&TO!';
            //         RAISE l_exit;
            //         END IF;
            #endregion
            var list = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_TO_DETAIL_HWT, MESDataObject.Module.HWT.R_DN_DETAIL>((rtdh, rdd) => rtdh.DN_NO == rdd.DN_NO)
                .Where((rtdh, rdd) => rtdh.TO_NO == to_no).Select((rtdh, rdd) => new { SO = rdd.SO_NO, SO_ITEM = rdd.SO_ITEM_NO, DN = rdd.DN_NO }).ToList()
                .GroupBy(r => r.SO).Distinct().ToList();
            if (list.Count > 1)
            {
                MESDataObject.Module.HWT.R_DN_DETAIL dnDetail = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_DN_DETAIL>().Where(r => r.DN_NO == dn_no).ToList().FirstOrDefault();
                //throw new MESReturnMessage("'同TO " + to_no + " 同SO_NO " + dnDetail.SO_NO + " 同SO_ITEM_NO " + dnDetail.SO_ITEM_NO + "存在多個DN,需開立唯一DN,請聯係交管重開DN&TO！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814155941", new string[] { to_no, dnDetail.SO_NO, dnDetail.SO_ITEM_NO }));

            }
        }

        /// <summary>
        /// HWT Shipping Station確認此DN是否有配置HW對應的PO  NN HWT 沒有啟用這個邏輯，故沒有寫完
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTShippingModeChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionDNNO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDNNO == null || sessionDNNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionTONO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTONO == null || sessionTONO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            string dn_no = sessionDNNO.Value.ToString();
            string to_no = sessionTONO.Value.ToString();
            string shipMode = "B25F";
            MESDataObject.Module.HWT.T_R_TO_DETAIL_HWT TRTD = new MESDataObject.Module.HWT.T_R_TO_DETAIL_HWT(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_C_SHIPPING_MODE TCSM = new MESDataObject.Module.HWT.T_C_SHIPPING_MODE(sfcdb, dbtype);

            MESDataObject.Module.HWT.R_TO_DETAIL_HWT dn = TRTD.GetDetailByDNNO(sfcdb, dn_no);
            if (dn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { dn_no }));
            }

            MESDataObject.Module.HWT.C_SHIPPING_MODE shipppingMode = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_TO_DETAIL_HWT, MESDataObject.Module.HWT.C_SHIPPING_MODE>
                ((rtd, csm) => rtd.DN_CUSTOMER == csm.SKUNO).Where(rtd => rtd.DN_NO == dn_no).Select((rtd, csm) => csm).ToList().FirstOrDefault();

            if (shipppingMode != null)
            {
                shipMode = shipppingMode.SHIPMODE;
            }


        }
        /// <summary>
        /// HWT RMA機種出貨卡環保屬性
        /// RMA出貨,一個TO只能有一個環保屬性ROHS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTRMAShippingPalletROHSStatusChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSkuno = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSkuno == null || sessionSkuno.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionTONO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTONO == null || sessionTONO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionPallteNo = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallteNo == null || sessionPallteNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            string skuno = sessionSkuno.Value.ToString();
            string to_no = sessionTONO.Value.ToString();
            string pallet = sessionPallteNo.Value.ToString();
            LogicObject.SKU skuObject = new LogicObject.SKU();
            if (skuObject.IsRMASkuno(sfcdb, skuno))
            {
                List<R_SHIP_DETAIL> listShipDetail = sfcdb.ORM.Queryable<R_SHIP_DETAIL, MESDataObject.Module.HWT.R_TO_DETAIL_HWT>((rsd, rtdh) => rsd.DN_NO == rtdh.DN_NO)
                    .Where((rsd, rtdh) => rtdh.TO_NO == to_no).Select((rsd, rtdh) => rsd).ToList();
                if (listShipDetail.Count > 0)
                {
                    List<string> listTOROHS = sfcdb.ORM.Queryable<R_WO_BASE, R_SN, R_SHIP_DETAIL, MESDataObject.Module.HWT.R_TO_DETAIL_HWT>
                       ((rwb, rs, rsd, rtdh) => rwb.WORKORDERNO == rs.WORKORDERNO && rs.SN == rsd.SN && rsd.DN_NO == rtdh.DN_NO)
                       .Where((rwb, rs, rsd, rtdh) => rs.VALID_FLAG == "1" && rtdh.TO_NO == to_no).Select((rwb, rs, rsd, rtdh) => rwb.ROHS)
                       .ToList().Distinct().ToList();
                    if (listTOROHS.Count > 1)
                    {
                        //throw new MESReturnMessage("已入TO:" + to_no + "的SN有不同的環保屬性！");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160039", new string[] { to_no }));

                    }
                    List<string> listPalletROHS = sfcdb.ORM.Queryable<R_WO_BASE, R_SN, R_PACKING, R_PACKING, R_SN_PACKING>
                      ((rwb, rs, rp, rpg, rsp) => rwb.WORKORDERNO == rs.WORKORDERNO && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && rp.PARENT_PACK_ID == rpg.ID)
                      .Where((rwb, rs, rp, rpg, rsp) => rs.VALID_FLAG == "1" && rpg.PACK_NO == pallet).Select((rwb, rs, rp, rpg, rsp) => rwb.ROHS)
                      .ToList().Distinct().ToList();
                    if (listPalletROHS.Count > 1)
                    {
                        //throw new MESReturnMessage("棧板:" + pallet + "的SN有不同的環保屬性！");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160108", new string[] { pallet }));

                    }
                    if (listTOROHS.FirstOrDefault() != listPalletROHS.FirstOrDefault())
                    {
                        //throw new MESReturnMessage("該棧板對應工單" + listPalletROHS.FirstOrDefault() + "屬性與已入TO工單" + listTOROHS.FirstOrDefault() + "屬性不一致！");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160129", new string[] { listPalletROHS.FirstOrDefault(), listTOROHS.FirstOrDefault() }));

                    }
                }
            }
        }
        /// <summary>
        /// HWT Shipping TO Info Checker
        ///  Check PM whether config ship address or not 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTShippingTOInfoChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionTOHeadObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionTOHeadObject == null || sessionTOHeadObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;

            MESDataObject.Module.HWT.T_R_TO_HEAD_HWT TRTH = new MESDataObject.Module.HWT.T_R_TO_HEAD_HWT(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_TO_DETAIL_HWT TRTD = new MESDataObject.Module.HWT.T_R_TO_DETAIL_HWT(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_DN_DETAIL TRDD = new MESDataObject.Module.HWT.T_R_DN_DETAIL(sfcdb, dbtype);
            MESDataObject.Module.HWT.R_TO_HEAD_HWT objTOHead = (MESDataObject.Module.HWT.R_TO_HEAD_HWT)sessionTOHeadObject.Value;
            //if (objTOHead == null)
            //{
            //    throw new MESReturnMessage(to_no + " The To_No Is Invalid!");
            //}
            string dnCustomer = "";
            string to_no = objTOHead.TO_NO;
            if (objTOHead.ABNORMITY_FLAG == "1")
            {
                throw new MESReturnMessage(to_no + " The To_No Is Lock!");
            }

            if (!string.IsNullOrEmpty(objTOHead.EXTERNAL_NO))
            {
                dnCustomer = objTOHead.EXTERNAL_NO;
            }
            else
            {
                List<string> listDNCustomer = TRTD.GetDetailByTONO(sfcdb, to_no).Select(r => r.DN_CUSTOMER).Distinct().ToList();
                if (listDNCustomer.Count > 1)
                {
                    throw new MESReturnMessage(to_no + "對應的機種有多個SHIPCODE,請找交管處理!");
                }
                dnCustomer = listDNCustomer.FirstOrDefault();
            }

            List<string> listAddress = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_TO_DETAIL_HWT, MESDataObject.Module.HWT.R_DN_DETAIL,
                MESDataObject.Module.HWT.R_SKUNO_ADDRESS, MESDataObject.Module.HWT.R_ASN_ADDRESS>
                ((rtd, rdd, rsa, raa) => rtd.DN_NO == rdd.DN_NO && rdd.P_NO == rsa.FSKUNO && rsa.SHIPTOCODE == raa.SHIPTOCODE)
                .Where((rtd, rdd, rsa, raa) => rtd.TO_NO == to_no && rsa.DATA2 == dnCustomer)
                .Select((rtd, rdd, rsa, raa) => rsa.DELIVER_ADDRESS).ToList().Distinct().ToList();
            if (listAddress.Count == 0)
            {
                //throw new MESReturnMessage(to_no + "對應的機種送貨地址沒有配置,請找交管處理!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160241", new string[] { to_no }));

            }
            if (listAddress.Count > 1)
            {
                //throw new MESReturnMessage(to_no + "對應的機種有多個送貨地址,請找交管處理!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160306", new string[] { to_no }));

            }

            List<MESDataObject.Module.HWT.R_DN_DETAIL> listDNDetail = TRDD.GetDNDetailListByTO(sfcdb, to_no);
            MESDataObject.Module.HWT.R_SKUNO_ADDRESS objAddress = null;
            foreach (var d in listDNDetail)
            {
                objAddress = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_SKUNO_ADDRESS>()
                    .Where(r => r.FSKUNO == d.P_NO && r.DATA2 == dnCustomer).ToList().FirstOrDefault();
                if (objAddress == null)
                {
                    //throw new MESReturnMessage(d.P_NO + "沒有配置出貨地址,請找交管配置!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160338", new string[] { d.P_NO }));

                }
                if (string.IsNullOrEmpty(objAddress.DELIVER_ADDRESS))
                {
                    //throw new MESReturnMessage(d.P_NO + "出貨地址為空,請找交管配置!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160417", new string[] { d.P_NO }));

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTShippingDNInfoChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionDNNO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDNNO == null || sessionDNNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionDNITEM = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionDNITEM == null || sessionDNITEM.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            string dn_no = sessionDNNO.Value.ToString();
            string dn_item = sessionDNITEM.Value.ToString();
            MESDataObject.Module.HWT.T_R_DN_DETAIL TRDD = new MESDataObject.Module.HWT.T_R_DN_DETAIL(sfcdb, dbtype);
            MESDataObject.Module.HWT.R_DN_DETAIL objDNDetail = TRDD.GetDetailByDNNO(sfcdb, dn_no, dn_item);
            if (objDNDetail == null)
            {
                throw new MESReturnMessage(dn_no + " The DN_NO Is Invalid!");
            }
            if (objDNDetail.P_NO_QTY == objDNDetail.REAL_QTY || objDNDetail.DN_ITEM_FLAG == "1")
            {
                throw new MESReturnMessage(dn_no + " The Dn_Item Has Been Finished!");
            }
        }
        /// <summary>
        /// HWT Shipping Pallet PCB Lock Checker
        /// 檢查扣板是否在HW系統中被鎖定 
        /// 檢查扣板是否已出貨（有出貨記錄）
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTShipPalletPCBLockChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionPalletObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletObject == null || sessionPalletObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            R_PACKING palletObject = (R_PACKING)sessionPalletObject.Value;
            T_R_SN TRS = new T_R_SN(sfcdb, dbtype);
            T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(sfcdb, dbtype);

            string sql = "";
            DataTable dt = new DataTable();
            LogicObject.SKU skuObject = new LogicObject.SKU();
            List<string> listKeyPart = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN, R_SN_KP>
                ((rp, rpg, rsp, rs, rsk) => rp.ID == rpg.PARENT_PACK_ID && rpg.ID == rsp.PACK_ID && rsp.SN_ID == rs.ID && rs.SN == rsk.SN)
                .Where((rp, rpg, rsp, rs, rsk) => rp.PACK_NO == palletObject.PACK_NO && rsk.STATION == "ASSY" && rsk.KP_NAME.StartsWith("PCB S/N"))
                .Select((rp, rpg, rsp, rs, rsk) => rsk.VALUE).ToList();
            foreach (string keyPart in listKeyPart)
            {
                //判斷扣板是否在HW系統中被鎖定 
                sql = $@"SELECT * FROM sfcruntime.t_btp_locked_sn_inface @hwems WHERE barcode = '{keyPart}' AND lock_flag = 'Y'";
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    //有鎖定記錄的話，再看PQE最近有沒有上傳解鎖信息，沒有的話就報錯
                    sql = $@"SELECT * FROM (SELECT * FROM (SELECT *  FROM t_btp_locked_sn_inface@hwems WHERE (barcode = var_cserialno)
                           ORDER BY updated_date DESC) WHERE rownum = 1) WHERE lock_flag = 'N'";
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        //throw new MESReturnMessage(palletObject.PACK_NO + "  " + keyPart + " 序列號(棧板,卡通,外箱)中有SN上的扣板已經在HW系統被鎖定!請聯繫PQE解鎖!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160443", new string[] { palletObject.PACK_NO, keyPart }));

                    }
                }

                //正常品非委託收貨和共管的扣板可以再次組到整機上進行出貨 ,做委託收貨的機種的標示是CS0001,GG0001是共管
                if (!skuObject.IsRMASkuno(sfcdb, palletObject.SKUNO))
                {
                    R_SN keyPartSNObject = TRS.LoadData(keyPart, sfcdb);
                    if (keyPartSNObject != null)
                    {
                        MESDataObject.Module.HWT.R_TO_HEAD_HWT objTOHead = sfcdb.ORM.Queryable<R_SHIP_DETAIL, MESDataObject.Module.HWT.R_TO_DETAIL_HWT, MESDataObject.Module.HWT.R_TO_HEAD_HWT>
                            ((rsd, rtd, rth) => rsd.DN_NO == rtd.DN_NO && rtd.TO_NO == rth.TO_NO)
                            .Where((rsd, rtd, rth) => rsd.SN == keyPart && (rth.EXTERNAL_NO == "GG0001" || rth.EXTERNAL_NO == "CS0001"))
                            .Select((rsd, rtd, rth) => rth).ToList().FirstOrDefault();
                        if (TRSD.IsExists(keyPart, sfcdb) && keyPartSNObject != null)
                        {
                            //throw new MESReturnMessage(palletObject.PACK_NO + " 序列號(棧板,卡通,外箱)中有SN上的扣板已經出貨");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160531", new string[] { palletObject.PACK_NO }));

                        }
                    }
                }
            }
        }

        public static void CheckJNPTONOonOpen(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrTO = "";
            Row_R_JUNIPER_TRUCKLOAD_TO rowTono = null;
            T_R_JUNIPER_TRUCKLOAD_TO JNPGenerateTO = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            try
            {
                rowTono = JNPGenerateTO.GetByTONo(Station.SFCDB);
                StrTO = rowTono.TO_NO;
                if (StrTO != null && StrTO != "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210327144725"));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void CheckTOqtyISNull(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null || TONO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession TOFLAG = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOFLAG == null)
            {
                TOFLAG = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = Paras[1].VALUE };
                Station.StationSession.Add(TOFLAG);
            }
            else
            {
                TOFLAG.Value = Paras[1].VALUE;
            }

            R_JUNIPER_TRUCKLOAD_TO rowTono = null;
            T_R_JUNIPER_TRUCKLOAD_TO JNPCheckTOqty = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            try
            {
                rowTono = JNPCheckTOqty.CheckTOqty(TONO.Value.ToString(), Station.BU, Station.SFCDB);
                int TOqty = Convert.ToInt32(rowTono.QTY);

                if (TOqty == 0)
                {
                    TOFLAG.Value = "N";
                }
                else
                {
                    TOFLAG.Value = "Y";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public static void CheckJNPShipUS(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 7)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null || TONO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession TOQTY = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOQTY == null || TOQTY.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession COUNTRYCODE = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (COUNTRYCODE == null)
            {
                COUNTRYCODE = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = Paras[2].VALUE };
                Station.StationSession.Add(COUNTRYCODE);
            }
            else
            {
                COUNTRYCODE.Value = Paras[2].VALUE;
            }
            MESStationSession COUNTRYCODETYPE = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (COUNTRYCODETYPE == null)
            {
                COUNTRYCODETYPE = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, SessionKey = Paras[6].SESSION_KEY, Value = Paras[6].VALUE };
                Station.StationSession.Add(COUNTRYCODETYPE);
            }

            MESStationSession PACKNO = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (PACKNO == null || PACKNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }
            MESStationSession TOWO = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (TOWO == null)
            {
                TOWO = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, Value = Paras[4].VALUE };
                Station.StationSession.Add(TOWO);
            }
            else
            {
                TOWO.Value = Paras[4].VALUE;
            }
            MESStationSession TOSKUNO = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (TOSKUNO == null)
            {
                TOSKUNO = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY, Value = Paras[5].VALUE };
                Station.StationSession.Add(TOSKUNO);
            }
            else
            {
                TOSKUNO.Value = Paras[5].VALUE;
            }

            string TOtype = "", CheckUS = "N";
            string ErrMessage = "";
            string StrUS = "", Strwo = "", StrfirstUS = "", Strskuno = "";
            string sql = "", sql1 = "";
            OleExec sfcdb = Station.SFCDB;
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            T_C_CONTROL UScontrol = new T_C_CONTROL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_CONTROL RowUS = null;

            if (COUNTRYCODETYPE.Value == null)
            {
                StrfirstUS = "";
            }
            else
            {
                StrfirstUS = COUNTRYCODETYPE.Value.ToString();
                if (StrfirstUS == "US")
                {
                    TOtype = "US";
                }
                else
                {
                    TOtype = "NOUS";
                }
            }

            try
            {
                sql = $@"select  workorderno,SKUNO from r_sn where id in(
                select sn_id from r_sn_packing where pack_id in(
                select ID from r_packing where parent_pack_id in(
                select ID from r_packing where pack_no ='{PACKNO.Value.ToString()}'))) and rownum <2 ";
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {

                    Strwo = dt.Rows[0]["WORKORDERNO"].ToString();
                    Strskuno = dt.Rows[0]["SKUNO"].ToString();
                    if (Strwo == "" && Strwo == null)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329145221", new string[] { PACKNO.Value.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    if (Strskuno == "" && Strskuno == null)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329151356", new string[] { PACKNO.Value.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    TOWO.Value = Strwo;
                    TOSKUNO.Value = Strskuno;

                    sql1 = $@"select distinct shiptopartycountry from r_i282 where messageid in(
                    select preasn from o_order_main where prewo='{Strwo}') and rownum <2";
                    dt1 = sfcdb.ExecSelect(sql1).Tables[0];
                    if (dt1.Rows.Count > 0)
                    {
                        StrUS = dt1.Rows[0]["shiptopartycountry"].ToString();
                        if (StrfirstUS == "" || StrfirstUS == null)
                        {
                            StrfirstUS = dt1.Rows[0]["shiptopartycountry"].ToString();
                            if (StrfirstUS.Trim() == "US")
                            {
                                TOtype = "US";
                                COUNTRYCODETYPE.Value = TOtype;
                            }
                            else
                            {
                                TOtype = "NOUS";
                                COUNTRYCODETYPE.Value = TOtype;
                            }
                        }

                        if (StrUS == "" || StrUS == null)
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329151805", new string[] { PACKNO.Value.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        else
                        {
                            if (COUNTRYCODE.Value == null)
                            {
                                COUNTRYCODE.Value = StrUS;
                            }
                            else if (COUNTRYCODE.Value != null && StrfirstUS != StrUS && TOtype == "US")
                            {
                                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329152209", new string[] { PACKNO.Value.ToString() });
                                throw new MESReturnMessage(ErrMessage);
                            }
                        }
                    }
                    else
                    {
                        COUNTRYCODETYPE.Value = "NULL";
                    }

                    RowUS = UScontrol.GetControlByName("TRUCKLOAD_CHECKUS", Station.SFCDB);
                    CheckUS = RowUS.CONTROL_VALUE.ToString().Trim();

                    if (COUNTRYCODETYPE.Value.ToString() == "US" && StrUS != "US" && CheckUS == "Y")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329152209", new string[] { PACKNO.Value.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    else if (COUNTRYCODETYPE.Value.ToString() == "NOUS" && StrUS == "US" && CheckUS == "Y")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329152209", new string[] { PACKNO.Value.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
                else
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329152646", new string[] { PACKNO.Value.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void CheckFinishDNNO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 6)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PACKNO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PACKNO == null || PACKNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession TOWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOWO == null || TOWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession TOSKUNO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TOSKUNO == null || TOSKUNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            MESStationSession DELIVERYNUM = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (DELIVERYNUM == null)
            {
                DELIVERYNUM = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = Paras[3].VALUE };
                Station.StationSession.Add(DELIVERYNUM);
            }
            else
            {
                DELIVERYNUM.Value = Paras[3].VALUE;
            }
            MESStationSession DELIVERYNUMQTY = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (DELIVERYNUMQTY == null)
            {
                DELIVERYNUMQTY = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, Value = Paras[4].VALUE };
                Station.StationSession.Add(DELIVERYNUMQTY);
            }
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (TONO == null || TONO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            string StrWO = "", StrDNNO = "", StrDnQty = "";
            string Strsql = "", Strsql1 = "";
            string ErrMessage = "";

            StrWO = TOWO.Value.ToString();

            Strsql = $@"SELECT DISTINCT DELIVERYNUMBER
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
            dt = sfcdb.ExecSelect(Strsql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                StrDNNO = dt.Rows[0]["DELIVERYNUMBER"].ToString();
                if (StrDNNO == "" || StrDNNO == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329162803", new string[] { PACKNO.Value.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }
                if (DELIVERYNUM.Value == null)
                {
                    DELIVERYNUM.Value = StrDNNO;
                }
                else
                {
                    StrDNNO = DELIVERYNUM.Value.ToString();
                }


                Strsql1 = $@"SELECT COUNT(*) as qtys FROM (
                SELECT DISTINCT PACK_NO FROM R_PACKING WHERE ID IN(
                SELECT PARENT_PACK_ID FROM R_PACKING WHERE ID IN(
                SELECT PACK_ID FROM R_SN_PACKING WHERE SN_ID IN(
                SELECT ID FROM R_SN WHERE WORKORDERNO IN('{StrWO}')))AND PARENT_PACK_ID IS NOT NULL))";

                dt1 = sfcdb.ExecSelect(Strsql1).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    StrDnQty = dt1.Rows[0]["qtys"].ToString();
                    DELIVERYNUMQTY.Value = StrDnQty.ToString();
                }
                else
                {
                    StrDnQty = "0";
                    DELIVERYNUMQTY.Value = StrDnQty.ToString();
                }
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210329162803", new string[] { PACKNO.Value.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            int x = Convert.ToInt32(DELIVERYNUMQTY.Value.ToString());
            if (x > 1)
            {
                x = x - 1;
                DELIVERYNUMQTY.Value = Convert.ToString(x);
            }
            else
            {
                x = 0;
                DELIVERYNUMQTY.Value = Convert.ToString(x);
            }

            //if (x!=0)
            //{
            //    if (StrDNNO!= DELIVERYNUMQTY.Value.ToString())
            //    {
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210405153302", new string[] { PACKNO.Value.ToString() });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //}

        }

        public static void CheckPLExists(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PACKNO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PACKNO == null || PACKNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string ErrMessage = "";
            OleExec sfcdb = Station.SFCDB;
            R_JUNIPER_TRUCKLOAD_DETAIL ExistsPL = null;
            T_R_JUNIPER_TRUCKLOAD_DETAIL PLDetail = new T_R_JUNIPER_TRUCKLOAD_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            ExistsPL = PLDetail.CheckPallet(PACKNO.Value.ToString(), Station.BU, Station.SFCDB);

            if (ExistsPL != null)
            {
                string TOPallet = ExistsPL.PACK_NO.ToString().Trim();

                if (TOPallet != null || TOPallet != "")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210413164841", new string[] { PACKNO.Value.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }
            }
        }

        public static void LoadNewPallet(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession TrailerNumber = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TrailerNumber == null || TrailerNumber.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession Newpallet = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Newpallet == null)
            {
                Newpallet = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = Paras[1].VALUE };
                Station.StationSession.Add(Newpallet);
            }
            else
            {
                Newpallet.Value = Paras[1].VALUE;
            }
            MESStationSession NewpalletQty = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NewpalletQty == null)
            {
                NewpalletQty = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = Paras[2].VALUE };
                Station.StationSession.Add(NewpalletQty);
            }
            else
            {
                NewpalletQty.Value = Paras[2].VALUE;
            }
            MESStationSession NewpalletList = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (NewpalletList == null)
            {
                NewpalletList = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = new List<string>() };
                Station.StationSession.Add(NewpalletList);
            }

            string ErrMessage = "";
            string StrTrailNum = TrailerNumber.Value.ToString();
            string StrNewPL = "";
            string StrNewPLQTY = "0";

            UIInputData I = new UIInputData()
            {
                MustConfirm = false,
                Timeout = 3000000,
                IconType = IconType.None,
                UIArea = new string[] { "90%", "90%" },
                //Message = "SN",
                Tittle = "Generate NewPallet",
                Type = UIInputType.String,
                //Name = "NewPallet",
                ErrMessage = "No input",
                CBMessage = ""
            };

            if (NewpalletQty.Value == null)
            {
                NewpalletQty.Value = StrNewPLQTY;
            }

            if (Newpallet.Value == null || Newpallet.Value.ToString() == "")
            {
                //I.CBMessage = "NewPallet";
                I.Name = "Pallet";
                I.Message = "Pallet";
                var ret = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                if (ret.ToUpper().Trim() == "")
                {
                    throw new Exception($@"Please Scan PalletID");
                }
                else
                {
                    StrNewPL = ret.ToUpper().Trim().ToString();
                    if (StrNewPL.Substring(0, 2) != "PL")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210420102943", new string[] { StrNewPL });
                        I.CBMessage = ErrMessage;
                        Newpallet.Value = null;
                        throw new Exception(I.CBMessage);
                    }
                    else
                    {
                        StrNewPL = StrNewPL.Replace("PL", "PLJ");
                        Newpallet.Value = StrNewPL;
                        Newpallet.InputValue = StrNewPL;
                        try
                        {
                            if (NewpalletList.Value == null)
                            {
                                NewpalletList.Value = new List<string>();
                            }
                            var ls = (List<string>)NewpalletList.Value;
                            ls.Add(StrNewPL);
                            NewpalletList.Value = ls;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210420102214", new string[] { Newpallet.Value.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void CheckOpenFlag(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            MESStationSession OPENFLAG = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (OPENFLAG == null)
            {
                OPENFLAG = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = Paras[1].VALUE };
                Station.StationSession.Add(OPENFLAG);
            }
            else
            {
                OPENFLAG.Value = Paras[1].VALUE;
            }

            string ErrMessage = "", StrTONO = "";
            string Strsql = "", Flagqty = "0", flag = "S";
            OleExec sfcdb = Station.SFCDB;
            DataTable dt = new DataTable();
            Row_R_JUNIPER_TRUCKLOAD_TO OpenTONO = null;
            T_R_JUNIPER_TRUCKLOAD_TO OPENflag = new T_R_JUNIPER_TRUCKLOAD_TO(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            Strsql = $@"SELECT count(*) as qty FROM (
            SELECT * FROM R_JUNIPER_TRUCKLOAD_TO ORDER BY 2 DESC
            ) WHERE rownum<2 and closed='0'";
            dt = sfcdb.ExecSelect(Strsql).Tables[0];

            if (TONO.Value == null)
            {
                if (dt.Rows.Count > 0)
                {
                    Flagqty = dt.Rows[0]["qty"].ToString();
                    if (Flagqty == "1")
                    {
                        flag = "Y";
                        OpenTONO = OPENflag.GetOpenTONo(Station.SFCDB);
                        StrTONO = OpenTONO.TO_NO.ToString().Trim().ToUpper();
                        TONO.Value = StrTONO;
                    }
                    else if (Flagqty == "0")
                    {
                        flag = "N";
                    }
                }
                OPENFLAG.Value = flag.ToString();
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210426175831", new string[] { TONO.Value.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void CheckTODNMustall(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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

            string ErrMessage = "", StrTO = "", Strsql = "", StrPalletList = "";
            DataTable dt = new DataTable();
            OleExec sfcdb = Station.SFCDB;

            StrTO = TONO.Value.ToString();
            if (TOFLAG.Value.ToString() == "Y")
            {
                if (StrTO != "")
                {
                    Strsql = $@"SELECT distinct pack_no FROM r_packing WHERE id in(
                    SELECT parent_pack_id FROM r_packing WHERE id in(
                    SELECT pack_id FROM r_sn_packing WHERE sn_id in(
                    SELECT ID FROM r_sn WHERE WORKORDERNO in(
                    SELECT distinct WORKORDERNO FROM r_juniper_mfpackinglist WHERE invoiceno='{StrTO}'))))  
                    and pack_no not in(SELECT pack_no FROM r_juniper_truckload_detail WHERE to_no='{StrTO}')";

                    dt = sfcdb.ExecSelect(Strsql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        List<string> ListPallet = new List<string>(dt.Rows.Count);
                        foreach (DataRow row in dt.Rows)
                        {
                            ListPallet.Add((string)row["PACK_NO"]);
                        }
                        StrPalletList = string.Join(",", ListPallet.ToArray());
                    }
                }
            }

            if (StrPalletList != "")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210429182736", new string[] { StrPalletList.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void CheckTOMasterPallet(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession TONO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var tono = TONO.Value.ToString();
            var toObj = Station.SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_TO>().Where(t => t.TO_NO == tono).First();
            if (toObj == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143020", new string[] { "TONO:" + TONO }));
            }
            var sql = $@"SELECT wm_concat(POLINES) POLINE,
                                   SALESORDER,
                                   wm_concat(SALESORDERLINEITEM) SOLINE,
                                   wm_concat(SKUNO) SKUNO,
                                   wm_concat(GROUPID) GROUPID,
                                   PALLETID
                              FROM (SELECT DISTINCT O.PONO||'.'||O.POLINE POLINES,
                                                    H.SALESORDERNUMBER SALESORDER,
                                                    I.SALESORDERLINEITEM,
                                                    A.SKUNO,
                                                    A.GROUPID,
                                                    A.PALLETID
                                      FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN O, O_I137_ITEM I,O_I137_HEAD H
                                     WHERE A.WORKORDERNO = O.PREWO
                                       AND O.ITEMID = I.ID
                                       AND I.TRANID = H.TRANID
                                       AND INVOICENO = '{tono}'
                                       AND PALLETID IN (SELECT PALLETID
                                                          FROM (SELECT DISTINCT PALLETID, SALESORDER
                                                                  FROM R_JUNIPER_MFPACKINGLIST
                                                                 WHERE INVOICENO = '{tono}')
                                                         GROUP BY PALLETID
                                                        HAVING COUNT(1) = 1)) T
                             GROUP BY SALESORDER, PALLETID
                             ORDER BY SALESORDER, PALLETID";
            var pallets = Station.SFCDB.ORM.Ado.SqlQuery<MasterPallet>(sql);
            var scanPallet = new List<string>();
            UIInputData I = new UIInputData()
            {
                MustConfirm = false,
                Timeout = 3000000,
                IconType = IconType.None,
                UIArea = new string[] { "90%", "90%" },
                Tittle = "Check Master Pallet Label",
                Type = UIInputType.String,
                ErrMessage = "No input",
                CBMessage = ""
            };

            var MasterPallet = "";
            while (pallets.Count > scanPallet.Count)
            {
                if (MasterPallet == "")
                {
                    I.OutInputs.Clear();
                    I.OutInputs.Add(new DisplayOutPut { DisplayType = "Text", Name = "QTY", Value = $@"{scanPallet.Count.ToString()}/{pallets.Count.ToString()}" });
                    I.Name = "MasterPallet";
                    I.Message = "Please Scan Master Pallet Label No.";
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                    if (ret == null)
                    {
                        I.CBMessage = $@"Please Scan Master Pallet Label No.";
                    }
                    else
                    {
                        string check_value = ret.ToString().Trim();
                        if (string.IsNullOrEmpty(check_value))
                        {
                            I.CBMessage = $@"Please Scan Master Pallet Label No.";
                        }
                        else if (check_value.Equals("No input"))
                        {
                            MasterPallet = "";
                            throw new Exception("User Cancel");
                        }
                        else if (scanPallet.Contains(check_value))
                        {
                            I.CBMessage = $@"This master pallet no has been scaned,Please scan another one.";
                        }
                        else if (pallets.Find(t => t.PALLETID == check_value) != null)
                        {
                            I.CBMessage = "";
                            MasterPallet = check_value;
                            continue;
                        }
                        else
                        {
                            MasterPallet = "";
                            I.CBMessage = $@"The master pallet no not exists!.";
                        }
                    }
                }
                else
                {
                    I.Name = "Unit";
                    I.Message = "Scan one unit on the physical pallet.";
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                    if (ret == null)
                    {
                        I.CBMessage = $@"Scan one unit SN on the physical pallet.";
                    }
                    else
                    {
                        var palletList = Station.SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL>()
                            .Where(t => t.NEW_PACK_NO == MasterPallet)
                            .Select(t => t.PACK_NO)
                            .ToList();
                        var snlist = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((SN, SP, CT, PL) => SN.ID == SP.SN_ID && SP.PACK_ID == CT.ID && CT.PARENT_PACK_ID == PL.ID)
                            .Where((SN, SP, CT, PL) => palletList.Contains(PL.PACK_NO))
                            .Select((SN, SP, CT, PL) => SN.SN)
                            .ToList();
                        string check_value = ret.ToString().Trim();
                        if (string.IsNullOrEmpty(check_value))
                        {
                            I.CBMessage = $@"Please scan one unit on the physical pallet.";
                        }
                        else if (check_value.Equals("No input"))
                        {
                            throw new Exception("User Cancel");
                        }
                        else if (snlist.Contains(check_value.Substring(1)) || palletList.Contains(check_value))
                        {
                            scanPallet.Add(MasterPallet);
                            I.CBMessage = "";
                            MasterPallet = "";
                            continue;
                        }
                        else
                        {
                            I.CBMessage = $@"The unit SN is not on the physical pallet.";
                        }
                    }
                }
            }
        }

        public static void CheckPLunits(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PACKNO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PACKNO == null || PACKNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string ErrMessage = "", Strsql = "", Strpackno = "", Strstation = "";
            OleExec sfcdb = Station.SFCDB;
            DataTable dt = new DataTable();

            Strpackno = PACKNO.Value.ToString().Trim();
            if (Strpackno != "")
            {
                Strsql = $@"SELECT * FROM (
                SELECT DISTINCT NEXT_STATION FROM R_SN WHERE ID IN(
                SELECT SN_ID FROM R_SN_PACKING WHERE PACK_ID IN(
                SELECT ID FROM R_PACKING WHERE PARENT_PACK_ID IN(
                SELECT ID FROM R_PACKING WHERE PACK_NO IN('{Strpackno}'))))) WHERE ROWNUM<2";

                dt = sfcdb.ExecSelect(Strsql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Strstation = dt.Rows[0]["NEXT_STATION"].ToString();
                    if (Strstation == "CBS2" || Strstation == "CBS")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210513100143", new string[] { PACKNO.Value.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                }
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210513101010", new string[] { PACKNO.Value.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void CheckDNNOinOneTO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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

            string ErrMessage = "", StrTO = "", Strsql = "", StrPalletList = "";
            DataTable dt = new DataTable();
            OleExec sfcdb = Station.SFCDB;
            StrTO = TONO.Value.ToString().Trim();

            if (TOFLAG.Value.ToString() == "Y")
            {
                if (StrTO != "")
                {
                    Strsql = $@"SELECT DISTINCT PACK_NO
                                  FROM R_PACKING
                                 WHERE ID IN
                                       (SELECT PARENT_PACK_ID
                                          FROM R_PACKING
                                         WHERE ID IN
                                               (SELECT PACK_ID
                                                  FROM R_SN_PACKING
                                                 WHERE SN_ID IN
                                                       (SELECT ID
                                                          FROM R_SN
                                                         WHERE WORKORDERNO IN
                                                               (SELECT DISTINCT PREWO
                                                                  FROM O_ORDER_MAIN
                                                                 WHERE PREASN IN
                                                                       (SELECT DISTINCT MESSAGEID AS ASNNUMBER
                                                                          FROM R_I282
                                                                         WHERE DELIVERYNUMBER IN
                                                                               (SELECT DISTINCT SALESORDER
                                                                                  FROM R_JUNIPER_MFPACKINGLIST
                                                                                 WHERE INVOICENO = '{StrTO}')
                                                                        UNION ALL
                                                                        SELECT DISTINCT ASNNUMBER
                                                                          FROM R_JNP_DOA_SHIPMENTS_ACK
                                                                         WHERE DELIVERYNUMBER IN
                                                                               (SELECT DISTINCT SALESORDER
                                                                                  FROM R_JUNIPER_MFPACKINGLIST
                                                                                 WHERE INVOICENO = '{StrTO}')))))
                                           AND PARENT_PACK_ID IS NOT NULL)
                                   AND PACK_NO NOT IN (SELECT DISTINCT PACK_NO
                                                         FROM R_JUNIPER_TRUCKLOAD_DETAIL
                                                        WHERE TO_NO = '{StrTO}')";

                    dt = sfcdb.ExecSelect(Strsql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        List<string> ListPallet = new List<string>(dt.Rows.Count);
                        foreach (DataRow row in dt.Rows)
                        {
                            ListPallet.Add((string)row["PACK_NO"]);
                        }
                        StrPalletList = string.Join(",", ListPallet.ToArray());
                    }
                }
            }

            if (StrPalletList != "")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210513112355", new string[] { StrPalletList.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        /// <summary>
        /// Combine shipout check packno and dn status
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackNoAndDnStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            var rPacking = new PalletBase(packNo, Station.SFCDB);

            int packSnQty = rPacking.GetSnCount(Station.SFCDB);

            if (packSnQty == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { packNo }));
            }
            int totalDNQty = 0;
            int totalShipQty = 0;
            foreach (DataRow row in shipDataTable.Rows)
            {
                var dnNo = row["DN_NO"].ToString();
                var dnLine = row["DN_ITEM"].ToString();
                var skuNo = row["SKU_NO"].ToString();

                #region 經管要求NPI幾種必須以N結尾的料號進行生產，以不帶N結尾料號出貨
                /*2021-1-28 10:56:06 LJD            
                解決方案為：
                1、PD以N結尾料號生產入庫；
                2、PM手動做N料號費領，用不帶N料號費退；
                3、WHS用不帶N料料號出貨；
                系統修改方案：
                判斷廠別為NLEZ，料號為VT開頭N結尾，出貨判斷料號時把結尾N去掉再進行比對。
                 */
                var dnObj = Station.SFCDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine ).ToList().FirstOrDefault();
                if (dnObj == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731133647", new string[] { dnNo, dnLine }));
                }
                var packsku = rPacking.DATA.SKUNO;
                if (dnObj.DN_PLANT == "NLEZ" && packsku.EndsWith("N") && packsku.StartsWith("VT"))
                {
                    if (skuNo.EndsWith("N"))
                        //M.M Requires that NPI must be shipped with a part number that does not end with ‘N’.
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210129082612", new string[] { packNo, packsku, dnNo, dnLine, skuNo }));
                    if (!packsku.Substring(0, packsku.Length - 1).Equals(skuNo))
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801085546", new string[] { packNo, packsku, dnNo, dnLine, skuNo }));
                }
                else if (!packsku.Equals(skuNo))
                {
                    DataTable dt = Station.SFCDB.ORM.Ado.GetDataTable("select *from all_tables where table_name='R_PRE_WO_HEAD' and owner='SFCRUNTIME'");
                    bool checkGourpID = false;
                    if (Station.BU.Equals("VNJUNIPER"))
                    {
                        MESDataObject.Module.Juniper.R_PRE_WO_HEAD grouidObj = Station.SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_PRE_WO_HEAD, R_SN, R_SN_PACKING, R_PACKING>
                            ((w, s, rsp, rpc) => w.WO == s.WORKORDERNO && s.ID == rsp.SN_ID && rsp.PACK_ID == rpc.ID)
                            .Where((w, s, rsp, rpc) => rpc.PARENT_PACK_ID == rPacking.DATA.ID && s.VALID_FLAG == "1")
                            .Select((w, s, rsp, rpc) => w).ToList().FirstOrDefault();

                        if (grouidObj != null)
                        {
                            checkGourpID = true;
                            if (!grouidObj.GROUPID.Equals(skuNo))
                            {
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801085546", new string[] { packNo, grouidObj.PID, dnNo, dnLine, skuNo }));
                            }
                        }
                    }

                    if (!checkGourpID)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801085546", new string[] { packNo, packsku, dnNo, dnLine, skuNo }));
                    }
                }
                #endregion                
                var dnShipDetail = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine).ToList();
                totalDNQty = totalDNQty + Convert.ToInt32(dnObj.QTY);
                totalShipQty = totalShipQty + dnShipDetail.Count;
            }

            if (packSnQty > totalDNQty - totalShipQty)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801091520", new string[] { packNo, packSnQty.ToString(), (totalDNQty - totalShipQty).ToString() }));
            }
            var rShipDetail = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_PACKING, R_SN_PACKING>((rsd, rp, rsp) =>
                rsd.ID == rsp.SN_ID && rp.ID == rsp.PACK_ID && rp.PARENT_PACK_ID == rPacking.DATA.ID).Select((rsd, rp, rsp) => rsd).ToList();
            if (rShipDetail.Count > 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180802105858", new string[] { packNo }));
        }
        /// <summary>
        /// Combine shipout check packing FIFO
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CombineShipoutFIFOChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            //料號要一樣
            var skuData = shipDataTable.AsEnumerable();
            IEnumerable<string> skuRow = skuData.Select(s => s.Field<string>("SKU_NO")).Distinct().ToList();
            if (skuRow.Count() > 1)
            {
                throw new Exception($@"more skuno in the waiting list.");
            }
            int totalDnQty = 0; //獲取DN數量
            int totalRealQty = 0;
            foreach (DataRow row in shipDataTable.Rows)
            {
                totalDnQty += Convert.ToInt32(row["GT_QTY"].ToString());
                totalRealQty += Convert.ToInt32(row["REAL_QTY"].ToString());
            }
            int balanceQty = totalDnQty - totalRealQty;
            string sku = skuRow.First();
            var SkuStockQty = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                    .Where((s, sp, rp1, rp2) => s.SKUNO == sku && s.SHIPPED_FLAG == MesBool.No.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && s.CURRENT_STATION == "CBS")
                    .Select((s, sp, rp1, rp2) => new { s.SN, s.COMPLETED_TIME, rp2.PACK_NO }).ToList(); //取得機種庫存數量


            var packobj = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == packNo).ToList().FirstOrDefault();
            if (Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where((r) => r.VALUE == packobj.SKUNO && r.FUNCTIONNAME == "NOCHECKSKU_SHIPPING" && r.CONTROLFLAG == "Y").Any())
                return;
            if (!Station.SFCDB.ORM.Queryable<R_PACKING_FIFO>().Any(t => t.PACKNO == packNo && t.STATUS == MesBool.Yes.ExtValue()))
            {

                if (balanceQty > SkuStockQty.Count) //DN出貨數量＞庫存時，掃描按棧板入庫時間順序卡關
                {
                    var packfirstsn = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                        .Where((s, sp, rp1, rp2) => rp2.PACK_NO == packNo).OrderBy((s, sp, rp1, rp2) => s.COMPLETED_TIME, SqlSugar.OrderByType.Asc).Select((s, sp, rp1, rp2) => s).ToList().FirstOrDefault();
                    var earlypackno = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                        .Where((s, sp, rp1, rp2) => s.SKUNO == packfirstsn.SKUNO && s.SHIPPED_FLAG == MesBool.No.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && rp2.PACK_NO != packNo && s.CURRENT_STATION == "CBS")
                        .OrderBy((s, sp, rp1, rp2) => s.COMPLETED_TIME, SqlSugar.OrderByType.Asc).Select((s, sp, rp1, rp2) => new { s.SN, s.COMPLETED_TIME, rp2.PACK_NO }).ToList().FirstOrDefault();

                    if (earlypackno != null && packfirstsn.COMPLETED_TIME > earlypackno.COMPLETED_TIME)
                        //throw new Exception($@"Pallet FIFO Alart: packno {packNo} sn: {packfirstsn.SN} completedtime is {packfirstsn.COMPLETED_TIME.ToString()} > packno {earlypackno.PACK_NO} sn {earlypackno.SN} completedtime {earlypackno.COMPLETED_TIME}");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151746", new string[] { packNo, packfirstsn.SN, packfirstsn.COMPLETED_TIME.ToString(), earlypackno.PACK_NO, earlypackno.SN, earlypackno.COMPLETED_TIME.ToString() }));
                }
                else if (balanceQty < SkuStockQty.Count) //DN出貨數量<庫存時，庫存入庫時間最早的數量并且在DN出貨數量範圍值內不卡關，超出后的掃描時卡FIFO
                {
                    string strSql = $@" select s.*
                                            from R_SN s, R_SN_PACKING sp, R_PACKING rp1, R_PACKING rp2
                                            where s.ID = sp.SN_ID
                                            and sp.PACK_ID = rp1.ID
                                            and rp1.PARENT_PACK_ID = rp2.ID
                                            and rp2.PACK_NO = '{packNo}'
                                            and s.sn not in
                                                (select sn
                                                    from (select s.SN,
                                                                    s.COMPLETED_TIME,
                                                                    rp2.PACK_NO,
                                                                    ROW_NUMBER() over(order by COMPLETED_TIME asc) as rowindex
                                                            from R_SN s, R_SN_PACKING sp, R_PACKING rp1, R_PACKING rp2
                                                            where s.ID = sp.SN_ID
                                                                and sp.PACK_ID = rp1.ID
                                                                and rp1.PARENT_PACK_ID = rp2.ID
                                                                and s.SKUNO = '{sku}'
                                                                and s.SHIPPED_FLAG = '0'
                                                                and s.COMPLETED_FLAG = '1'
                                                                AND s.current_station = 'CBS')
                                                    where rowindex between 1 and '{balanceQty}')";

                    DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        var packfirstsn = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                            .Where((s, sp, rp1, rp2) => rp2.PACK_NO == packNo).OrderBy((s, sp, rp1, rp2) => s.COMPLETED_TIME, SqlSugar.OrderByType.Asc).Select((s, sp, rp1, rp2) => s).ToList().FirstOrDefault();
                        var earlypackno = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                            .Where((s, sp, rp1, rp2) => s.SKUNO == packfirstsn.SKUNO && s.SHIPPED_FLAG == MesBool.No.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && rp2.PACK_NO != packNo && s.CURRENT_STATION == "CBS")
                            .OrderBy((s, sp, rp1, rp2) => s.COMPLETED_TIME, SqlSugar.OrderByType.Asc).Select((s, sp, rp1, rp2) => new { s.SN, s.COMPLETED_TIME, rp2.PACK_NO }).ToList().FirstOrDefault();
                        if (earlypackno != null && packfirstsn.COMPLETED_TIME > earlypackno.COMPLETED_TIME)
                            //throw new Exception($@"Pallet FIFO Alart: packno {packNo} sn: {packfirstsn.SN} completedtime is {packfirstsn.COMPLETED_TIME.ToString()} > packno {earlypackno.PACK_NO} sn {earlypackno.SN} completedtime {earlypackno.COMPLETED_TIME}");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151746", new string[] { packNo, packfirstsn.SN, packfirstsn.COMPLETED_TIME.ToString(), earlypackno.PACK_NO, earlypackno.SN, earlypackno.COMPLETED_TIME.ToString() }));

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

            }
        }
        public static void CombineShipoutPackSNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            //料號要一樣
            var skuData = shipDataTable.AsEnumerable();
            IEnumerable<string> skuRow = skuData.Select(s => s.Field<string>("SKU_NO")).Distinct().ToList();
            if (skuRow.Count() > 1)
            {
                throw new Exception($@"more skuno in the waiting list.");
            }
            int totalDnQty = 0; //獲取DN數量
            int totalRealQty = 0;
            foreach (DataRow row in shipDataTable.Rows)
            {
                var dn = row["DN_NO"].ToString();
                var line = row["DN_ITEM"].ToString();
                var dnObj = Station.SFCDB.ORM.Queryable<R_DN_STATUS>().Where(r => r.DN_NO == dn && r.DN_LINE == line).ToList().FirstOrDefault();
                if(dnObj==null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731133647", new string[] { dn, line }));
                }
                var dnShippedList = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(r => r.DN_NO == dn && r.DN_LINE == line).ToList();
                
                totalDnQty += (int)dnObj.QTY;
                totalRealQty += dnShippedList.Count;
            }
            int balanceQty = totalDnQty - totalRealQty;

            UIInputData O = new UIInputData() { 
                Timeout = 100000, 
                IconType = IconType.Warning, 
                UIArea = new string[] { "50%", "45%" }, 
                Message = "", 
                Tittle = "Please input sn.", 
                Type = UIInputType.String, 
                Name = "SN", 
                ErrMessage = "Please input sn."
            };

            var shippingSnList = Station.SFCDB.ORM
                .Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((sn, rsp, carton, pallet) => pallet.ID == carton.PARENT_PACK_ID && carton.ID == rsp.PACK_ID && rsp.SN_ID == sn.ID)
                .Where((sn, rsp, carton, pallet) => sn.VALID_FLAG == "1" && pallet.PACK_NO == packNo).Select((sn, rsp, carton, pallet) => sn).ToList();
            if (shippingSnList == null || shippingSnList.Count == 0)
            {
                //throw new Exception($@"棧板:{packNo} 實物為空");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144918", new string[] { packNo }));
            }
            var shippingCartonList = Station.SFCDB.ORM
                .Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((carton, pallet, sn, rsp) => pallet.ID == carton.PARENT_PACK_ID && carton.ID == rsp.PACK_ID && rsp.SN_ID == sn.ID)
                .Where((carton, pallet, sn, rsp) => sn.VALID_FLAG == "1" && pallet.PACK_NO == packNo).Select((carton, pallet, sn, rsp) => carton).ToList();
            if (shippingCartonList == null)
            {
                //throw new Exception($@"棧板:{packNo} 實物為空");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144918", new string[] { packNo }));
            }
            var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station, (res) =>
            {
                if (shippingSnList.FindAll(it => it.SN == res.ToString()).Count > 0)
                {
                    return true;
                }
                else if (shippingCartonList.FindAll(it => it.PACK_NO == res.ToString()).Count > 0)
                {
                    return true;
                }
                else
                {
                    O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145249", new string[] { res, packNo });                    
                }
                return false;
            });
        }

        #region  R_SN_DELIVER_INFO
        /// <summary>
        /// 檢查R_SN_DELIVER_INFO  ORDERNO 是否存在
        /// </summary>
        public static void DeliverOrdernoChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionOrderNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOrderNo == null || sessionOrderNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string OrderNo = sessionOrderNo.Value.ToString();
            List<R_SN_DELIVER_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.ORDERNO == OrderNo && t.VALID_FLAG == "1").ToList();
            if (TGMESlist.Count == 0)
            {  
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150929", new string[] { OrderNo }));
            }


            //List<R_SN_DELIVER_INFO> DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.ORDERNO == OrderNo && t.CHECK_FLAG == "0" && t.VALID_FLAG == "1").ToList();
            //if (DELIVERlist.Count == 0)
            //{
            //    throw new MESReturnMessage($@"ORDERNO{OrderNo}已經全部OQC_PASS");

            //}
        }
        /// <summary>
        /// 檢查Pallet_Sn是否是這個ORDERNO的
        /// </summary>
        public static void DeliverPallet_SnChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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


            List<R_SN_DELIVER_INFO> DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.PALLET_SN == Pallet_Sn && t.VALID_FLAG == "1").ToList();
            if (DELIVERlist.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154143", new string[] { Pallet_Sn }));

            }

            //DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.PALLET_SN == Pallet_Sn && t.CHECK_FLAG == "0" && t.VALID_FLAG == "1").ToList();
            //if (DELIVERlist.Count == 0)
            //{
            //    throw new MESReturnMessage($@"棧板號{Pallet_Sn}已經全部OQC_PASS");

            //}

            List<R_SN_DELIVER_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.ORDERNO == OrderNo && t.PALLET_SN == Pallet_Sn && t.VALID_FLAG == "1").ToList();
            if (TGMESlist.Count == 0)
            {
                 throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211203153757", new string[] { Pallet_Sn, OrderNo }));
               

                 
            }
        }

        /// <summary>
        /// 檢查CARTON_SN是否是這個Pallet_Sn的
        /// </summary>
        public static void DeliverCarton_SnChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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


            List<R_SN_DELIVER_INFO> DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.CARTON_SN == Carton_Sn && t.VALID_FLAG == "1").ToList();
            if (DELIVERlist.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154143", new string[] { Carton_Sn, Carton_Sn }));

            }
 
            DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.CARTON_SN == Carton_Sn && t.CHECK_FLAG == "0" && t.VALID_FLAG == "1").ToList();
            if (DELIVERlist.Count == 0)
            {
                throw new MESReturnMessage($@"卡通號{Carton_Sn}已經全部OQC_PASS");

            }

            List<R_SN_DELIVER_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.ORDERNO == OrderNo && t.PALLET_SN == Pallet_Sn && t.CARTON_SN == Carton_Sn && t.VALID_FLAG == "1").ToList();
            if (TGMESlist.Count == 0)
            {
                 throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211203153757", new string[] { Carton_Sn, Pallet_Sn }));
                
            }
        }

        /// <summary>
        /// 檢查IMEI是否是這個CARTON_SN的
        /// </summary>
        public static void DeliverIMEIChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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


            List<R_SN_DELIVER_INFO> DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t =>  t.IMEI == IMEI && t.VALID_FLAG == "1").ToList();
            if (DELIVERlist.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154143", new string[] { IMEI, Carton_Sn }));

            }

            DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.IMEI == IMEI && t.CHECK_FLAG == "1" && t.VALID_FLAG == "1").ToList();
            if (DELIVERlist.Count == 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211207163205", new string[] { IMEI, Station.StationName }));

            }

            DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.ORDERNO == OrderNo && t.PALLET_SN == Pallet_Sn && t.CARTON_SN == Carton_Sn && t.IMEI == IMEI && t.VALID_FLAG == "1").ToList();
            if (DELIVERlist.Count == 0)
            {
                 throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211203153757", new string[] { IMEI, Carton_Sn }));
                 
            }
            
        }

        /// <summary>
        /// 檢查Order是否已全部OQC PASS 
        /// </summary>
        public static void DeliverOrderOqcAllPassChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionOrderNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOrderNo == null || sessionOrderNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string OrderNo = sessionOrderNo.Value.ToString(); 

            string sql = string.Empty;
            sql = $@"SELECT A.QTY||'/'||B.QTY FROM(SELECT COUNT(DISTINCT PALLET_SN)QTY
                      FROM R_SN_DELIVER_INFO
                     WHERE ORDERNO = '{OrderNo}'
                       AND VALID_FLAG = '1'
                       AND NEXT_STATION = 'DELIVER_CHECK')A ,
                    (SELECT COUNT(DISTINCT PALLET_SN)QTY
                      FROM R_SN_DELIVER_INFO
                     WHERE ORDERNO = '{OrderNo}'
                       AND VALID_FLAG = '1')B";
            DataTable dt = new DataTable();
            dt = Station.SFCDB.ExecSelect(sql).Tables[0];

            if (dt.Rows.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150929", new string[] { OrderNo }));
            }

            MESStationSession sessionPL_QTY = Station.StationSession.Find(T => T.MESDataType == "PL_QTY" && T.SessionKey == "1");
            if (sessionPL_QTY == null)
            {
                sessionPL_QTY = new MESStationSession() { MESDataType = "PL_QTY", SessionKey = "1" };
                Station.StationSession.Add(sessionPL_QTY);
            }
            sessionPL_QTY.Value = dt.Rows[0][0].ToString();



            List<R_SN_DELIVER_INFO> DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.ORDERNO == OrderNo && t.CHECK_FLAG == "0" && t.VALID_FLAG == "1").ToList();
            if (DELIVERlist.Count == 0)
            {
                //  throw new MESReturnMessage($@"訂單{OrderNo}已全部OQC_PASS");
                Station.StationMessages.Add(new StationMessage() { Message = $@"訂單{OrderNo}已全部OQC_PASS" });

                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == "ORDERNO");
                if (input != null)
                {
                    Station.NextInput = input;
                }
            }
            else
            {
                Station.StationMessages.Add(new StationMessage() { Message = $@"訂單{OrderNo}OK,可以繼續掃描" });
            }
        }

        /// <summary>
        /// 檢查Order是否已全部DeliverCheck 
        /// </summary>
        public static void DeliverCheckAllPassChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionOrderNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOrderNo == null || sessionOrderNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string OrderNo = sessionOrderNo.Value.ToString();

            List<R_SN_DELIVER_INFO> DELIVERlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.ORDERNO == OrderNo && t.DELIVER_FLAG == "0" && t.VALID_FLAG == "1").ToList();
            if (DELIVERlist.Count == 0)
            {
                //  throw new MESReturnMessage($@"訂單{OrderNo}已全部OQC_PASS");
                Station.StationMessages.Add(new StationMessage() { Message = $@"訂單{OrderNo}已全部DELIVER_CHECK" });

                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == "ORDERNO");
                if (input != null)
                {
                    Station.NextInput = input;
                }
            }
            else
            {
                Station.StationMessages.Add(new StationMessage() { Message = $@"訂單{OrderNo}OK,可以繼續掃描" });
            }
        }

        /// <summary>
        /// 檢查R_SN_DELIVER_INFO  ORDERNO 是否存在,訂單可逗號隔開
        /// </summary>
        public static void DeliverCheckOrdernoChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionOrderNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOrderNo == null || sessionOrderNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string OrderNo = sessionOrderNo.Value.ToString();
            //List<R_SN_DELIVER_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_DELIVER_INFO>().Where(t => t.ORDERNO == OrderNo && t.VALID_FLAG == "1").ToList();

            string OrderNos = OrderNo.Replace(",","','");
            OrderNos= "'"+ OrderNo + "'";

            string sql = string.Empty;
            sql = $@"SELECT * FROM R_SN_DELIVER_INFO WHERE ORDERNO in({OrderNos}) and VALID_FLAG='1'";
            DataTable dt = new DataTable();
                dt = Station.SFCDB.ExecSelect(sql).Tables[0];

            if (dt.Rows.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150929", new string[] { OrderNo }));
            }

            sql = $@"SELECT * FROM R_SN_DELIVER_INFO WHERE ORDERNO in({OrderNos}) and VALID_FLAG='1' AND NEXT_STATION='DELIVER_CHECK'";
              
            dt = Station.SFCDB.ExecSelect(sql).Tables[0];

            if (dt.Rows.Count == 0)
            {
                throw new MESReturnMessage($@"訂單{OrderNo}沒有待過DELIVER_CHECK的棧板");
            } 
        }

        /// <summary>
        /// 檢查Pallet_Sn是否是這個ORDERNO的
        /// </summary>
        public static void DeliverCheckPallet_SnChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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

            string sql = string.Empty;
            sql = $@"SELECT * FROM R_SN_DELIVER_INFO WHERE ORDERNO in({OrderNos}) and VALID_FLAG='1' and Pallet_Sn='{Pallet_Sn}'";
            DataTable dt = new DataTable();
            dt = Station.SFCDB.ExecSelect(sql).Tables[0];

            if (dt.Rows.Count == 0)
            {
                throw new MESReturnMessage($@"棧板{Pallet_Sn}不是訂單{OrderNo}的數據"); 
            }

              sql = string.Empty;
            sql = $@"SELECT * FROM R_SN_DELIVER_INFO WHERE ORDERNO in({OrderNos}) and VALID_FLAG='1' and Pallet_Sn='{Pallet_Sn}' and DELIVER_FLAG='1'";
            
            dt = Station.SFCDB.ExecSelect(sql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                throw new MESReturnMessage($@"棧板{Pallet_Sn}已過DELIVER_CHECK工站");
            }
        }


        #endregion



        class MasterPallet
        {
            public string PONO { get; set; }
            public string POLINE { get; set; }
            public string SALESORDER { get; set; }
            public string SOLINE { get; set; }
            public string SKUNO { get; set; }
            public string GROUPID { get; set; }
            public string PALLETID { get; set; }
        }
    }
}
