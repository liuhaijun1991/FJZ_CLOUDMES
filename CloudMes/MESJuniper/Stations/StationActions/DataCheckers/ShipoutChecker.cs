using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;

namespace MESJuniper.Stations.StationActions.DataCheckers
{
    public class ShipoutChecker
    {
        /// <summary>
        /// The Same Sale Region
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TheSameRegionChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession InputNewPalletOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputNewPalletOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession TPalletSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TPalletSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            var newPalletno = InputNewPalletOSession.Value.ToString();
            var palletno = TPalletSession.Value.ToString();
            var CurrentRegion = Station.SFCDB.ORM.Queryable<R_JUNIPER_MFPACKINGLIST, R_I282, R_GEOGRAPHIES_MAP>((j, d, g) => j.SALESORDER == d.DELIVERYNUMBER && d.SHIPTOPARTYCOUNTRY == g.COUNTRYCODE)
                .Where((j, d, g) => j.PALLETID == newPalletno)
                .Select((j, d, g) => g)
                .First();
            if (CurrentRegion == null)
            {
                return;
            }
            var plShiptoCountry = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING, O_ORDER_MAIN, R_I282>(
                (s, sp, ct, pl, wo, d) => s.ID == sp.SN_ID && sp.PACK_ID == ct.ID && ct.PARENT_PACK_ID == pl.ID && wo.PREWO == s.WORKORDERNO && wo.PREASN == d.ASNNUMBER)
                .Where((s, sp, ct, pl, wo, d) => pl.PACK_NO == palletno)
                .Select((s, sp, ct, pl, wo, d) => d.SHIPTOPARTYCOUNTRY)
                .First();
            if (string.IsNullOrEmpty(plShiptoCountry))
            {
                throw new Exception($@"Fail to get SHIPTOCOUNTRYCODE from i282 head");
            }
            var rgm = Station.SFCDB.ORM.Queryable<R_GEOGRAPHIES_MAP>().Where(t => t.COUNTRYCODE == plShiptoCountry).First();
            if (rgm == null)
            {
                throw new Exception($@"missing [{plShiptoCountry}] geographies mapping");
            }
            if (rgm.REGION1 != CurrentRegion.REGION1)
            {
                throw new Exception($@"Region is different,please scan the pallet for region as " + CurrentRegion.REGION1);
            }
        }

        public static void TrcukLoadTOClosedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession TOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TOSession == null || TOSession.Value == null || TOSession.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            var strTO = TOSession.Value.ToString();
            var tolist = Station.SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_TO>().Where(t => t.TO_NO == strTO && t.CLOSED == "0").ToList();
            if (tolist.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_TONOCLOSED", new string[] { strTO }));
            }
        }

        public static void TruckLoadFinishChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession TrailerSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TrailerSession == null || TrailerSession.Value == null || TrailerSession.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            #region check all TO# of trailer,the content of TO# can't be empty. 
            var trailer = TrailerSession.Value.ToString();
            var NofFinishTolist = Station.SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_TO>().Where(t => t.TRAILER_NUM == trailer && t.QTY==0 && t.CLOSED == "0").ToList();
            if (NofFinishTolist.Count > 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "The TO " + NofFinishTolist[0].TO_NO + " " }));
            }
            #endregion

            var tolist = Station.SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_TO>().Where(t => t.TRAILER_NUM == trailer && t.CLOSED == "0").ToList();
            string ErrMessage = "", StrTO = "", Strsql = "", StrPalletList = "";
            for (int i = 0; i < tolist.Count; i++)
            {
                #region If the unit is the same WO,need to scan on the same TO#
                Strsql = $@"SELECT distinct pack_no
                                  FROM r_packing
                                 WHERE id in
                                       (SELECT parent_pack_id
                                          FROM r_packing
                                         WHERE id in (SELECT pack_id
                                                        FROM r_sn_packing
                                                       WHERE sn_id in
                                                             (SELECT ID
                                                                FROM r_sn
                                                               WHERE WORKORDERNO in
                                                                     (SELECT distinct WORKORDERNO
                                                                        FROM r_juniper_mfpackinglist
                                                                       WHERE invoiceno = '{StrTO}'))))
                                   and pack_no not in (SELECT pack_no
                                                         FROM r_juniper_truckload_detail
                                                        WHERE to_no = '{StrTO}')";

                var dt = Station.SFCDB.ORM.Ado.GetDataTable(Strsql);

                if (dt.Rows.Count > 0)
                {
                    List<string> ListPallet = new List<string>(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
                    {
                        ListPallet.Add((string)row["PACK_NO"]);
                    }
                    StrPalletList = string.Join(",", ListPallet.ToArray());
                }

                if (StrPalletList != "")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210429182736", new string[] { StrPalletList.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }
                #endregion

                #region If the unit is the same DN#,need to Scan on the same TO# 
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

                dt = Station.SFCDB.ORM.Ado.GetDataTable(Strsql);

                if (dt.Rows.Count > 0)
                {
                    List<string> ListPallet = new List<string>(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
                    {
                        ListPallet.Add((string)row["PACK_NO"]);
                    }
                    StrPalletList = string.Join(",", ListPallet.ToArray());
                }
                if (StrPalletList != "")
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210513112355", new string[] { StrPalletList.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }
                #endregion
            }
        }
    }
}
