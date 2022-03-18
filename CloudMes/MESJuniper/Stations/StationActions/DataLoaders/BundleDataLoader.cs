using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using MESPubLab.MESStation.SNMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Constants.PublicConstants;

namespace MESJuniper.Stations.StationActions.DataLoaders
{
    public class BundleDataLoader
    {
        /// <summary>
        /// Get SO List That SO Need To Scan Bundle(BNDL & Has Linecard Item)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetBundleSOList(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SessionSO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSO == null)
            {
                SessionSO = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = new List<string>() };
                Station.StationSession.Add(SessionSO);
            }
            if (SessionSO.Value == null)
            {
                SessionSO.Value = new List<string>();
            }
            var solist = Station.SFCDB.ORM.Ado.SqlQuery<string>($@"SELECT T.SALESORDERNUMBER
                                                                      FROM (SELECT HH.SALESORDERNUMBER, II.SOID, COUNT(1)
                                                                              FROM O_ORDER_MAIN OO
                                                                              LEFT JOIN O_I137_ITEM II
                                                                                ON OO.ITEMID = II.ID
                                                                              LEFT JOIN O_I137_HEAD HH
                                                                                ON II.TRANID = HH.TRANID
                                                                             WHERE (HH.SALESORDERNUMBER, II.SOID) IN
                                                                                   (SELECT H.SALESORDERNUMBER, I.SOID
                                                                                      FROM O_ORDER_MAIN O
                                                                                      LEFT JOIN C_SKU K
                                                                                        ON O.PID = K.SKUNO
                                                                                      LEFT JOIN C_SERIES C
                                                                                        ON K.C_SERIES_ID = C.ID
                                                                                      LEFT JOIN O_I137_ITEM I
                                                                                        ON O.ITEMID = I.ID
                                                                                      LEFT JOIN O_I137_HEAD H
                                                                                        ON I.TRANID = H.TRANID
                                                                                      LEFT JOIN O_PO_STATUS S
                                                                                        ON S.POID = O.ID
                                                                                       AND S.VALIDFLAG = '1'
                                                                                       AND S.STATUSID = '9'
                                                                                     WHERE I.SOID <> '000000'
                                                                                       AND C.SERIES_NAME = 'Juniper-FRU'
                                                                                       AND O.CANCEL = '0'
                                                                                     GROUP BY H.SALESORDERNUMBER, I.SOID)
                                                                             GROUP BY HH.SALESORDERNUMBER, II.SOID
                                                                            HAVING COUNT(1) > 1) T");

            SessionSO.Value = solist;
        }

        /// <summary>
        /// Get BNDL SOID List
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetBundleSOIDList(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SessionSO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSO == null || SessionSO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession SessionSOID = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (SessionSOID == null)
            {
                SessionSOID = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = new List<string>() };
                Station.StationSession.Add(SessionSOID);
            }
            if (SessionSOID.Value == null)
            {
                SessionSOID.Value = new List<string>();
            }
            string SO = SessionSO.Value.ToString();
            var soidList = Station.SFCDB.ORM.Ado.SqlQuery<string>($@"SELECT DISTINCT I.SOID
                                                                          FROM O_ORDER_MAIN O, O_I137_ITEM I, O_I137_HEAD H, O_PO_STATUS S
                                                                         WHERE O.ITEMID = I.ID
                                                                           AND I.TRANID = H.TRANID
                                                                           AND S.POID = O.ID
                                                                           AND S.VALIDFLAG = '1'
                                                                           AND S.STATUSID = '9'
                                                                           AND I.SOID <> '000000'
                                                                           AND O.CANCEL = '0'
                                                                           AND H.SALESORDERNUMBER = '{SO}'");
            SessionSOID.Value = soidList;
        }

        public static void GetLastBundleInfo(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SessionSO = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSO == null || SessionSO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession SessionSOID = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (SessionSOID == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession SessionBundleNo = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (SessionBundleNo == null)
            {
                SessionBundleNo = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(SessionBundleNo);
            }
            MESStationSession SessionBundleInfo = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (SessionBundleInfo == null)
            {
                SessionBundleInfo = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(SessionBundleInfo);
            }
            MESStationSession SessionBalance = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (SessionBalance == null)
            {
                SessionBalance = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                Station.StationSession.Add(SessionBalance);
            }
            string so = SessionSO.Value.ToString();
            string soid = SessionSOID.Value.ToString();
            var soinfoList = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
                .Where((O, I, H) => I.SOID == soid && H.SALESORDERNUMBER == so && O.CANCEL == MesBool.No.ExtValue())
                .Select((O, I, H) => new { O, I, H })
                .Distinct()
                .ToList();
            List<BundleInfo> bndl = new List<BundleInfo>();

            int totalQty = 0;

            var lastBP = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.SALEORDER == so && t.SOID == soid)
                .OrderBy(t => t.BNDLDATETIME, SqlSugar.OrderByType.Desc)
                .First();
            var bndl_no = "";
            if (lastBP != null)
            {
                bndl_no = lastBP.BNDL_NO;
            }
            else if (SessionBundleNo.Value != null)
            {
                bndl_no = SessionBundleNo.Value.ToString();
            }
            else
            {
                bndl_no = SNmaker.GetNextSN("BNDL", Station.DBS["SFCDB"]);
            }
            var BndlRequestQty = 0;
            for (int i = 0; i < soinfoList.Count; i++)
            {
                totalQty += Convert.ToInt32(double.Parse(soinfoList[i].I.QUANTITY));
                var NeedQty = Convert.ToInt32(double.Parse(soinfoList[i].I.QUANTITY) / double.Parse(soinfoList[i].I.SOQTY));
                BndlRequestQty += NeedQty;
                var bpack = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                    .Where(t => t.BNDL_NO == bndl_no && t.WORKORDERNO == soinfoList[i].O.PREWO)
                    .OrderBy(t => t.BNDLDATETIME, SqlSugar.OrderByType.Desc)
                    .First();
                List<R_JUNIPER_BUNDLE> bndlList = new List<R_JUNIPER_BUNDLE>();
                if (bpack != null)
                {
                    bndlList = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>().Where(t => t.BNDL_NO == bpack.BNDL_NO && t.WORKORDERNO == soinfoList[i].O.PREWO).ToList();
                }
                var bndlitem = new BundleInfo()
                {
                    WO = soinfoList[i].O.PREWO,
                    SKUNO = soinfoList[i].I.PN,
                    SO = soinfoList[i].H.SALESORDERNUMBER,
                    SOITEM = soinfoList[i].I.SALESORDERLINEITEM,
                    RequestQty = NeedQty,
                    PackedQty = bndlList.Count
                };
                bndl.Add(bndlitem);


            }

            var scanedQty = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.SALEORDER == so && t.SOID == soid && t.VALID_FLAG == "1")
                .ToList().Count;
            var bndlQty = (scanedQty - (scanedQty % BndlRequestQty)) / BndlRequestQty;
            var balance = Convert.ToInt64(double.Parse(soinfoList[0].I.SOQTY)) - bndlQty;
            if (!bndl.Where(t => t.PackedQty < t.RequestQty).Any())
            {
                bndl_no = SNmaker.GetNextSN("BNDL", Station.DBS["SFCDB"]);
                for (int i = 0; i < bndl.Count; i++)
                {
                    bndl[i].PackedQty = 0;
                }
            }
            SessionBalance.Value = balance;
            SessionBundleNo.Value = bndl_no;
            SessionBundleInfo.Value = bndl;
        }

        class BundleInfo
        {
            public string WO { get; set; }
            public string SKUNO { get; set; }
            public string SO { get; set; }
            public string SOITEM { get; set; }
            public int RequestQty { get; set; }
            public int PackedQty { get; set; }
        }

    }
}
