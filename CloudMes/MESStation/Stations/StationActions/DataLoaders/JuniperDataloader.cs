using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.DataLoaders
{
    class JuniperDataloader
    {
        public static void WoIsBundleDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession IsBundleSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (IsBundleSession == null)
            {
                IsBundleSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(IsBundleSession);
            }

            var SFCDB = Station.SFCDB;
            var strWO = WOSession.Value.ToString();
            var om = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == strWO).First();
            if (om == null)
            {
                IsBundleSession.Value = null;
                return;
            }
            if (IsLinecardBundle(SFCDB, strWO))
            {
                IsBundleSession.Value = "Linecard Bundle";
            }
            else
            {
                IsBundleSession.Value = IsBundle(SFCDB, strWO, om.ITEMID) ? "Bundle" : null;
            }



        }
        static bool IsBundle(OleExec SFCDB, string wo, string item_id)
        {
            bool res = false;
            //var itemObj = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN, O_I137_ITEM>((r, o, i) => r.ORIGINALID == o.ID && o.ITEMID == i.ID)
            //    .Where((r, o, i) => o.PREWO == wo && r.VALID == "1").Select((r, o, i) => i).ToList().FirstOrDefault();
            var itemObj = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == item_id).ToList().FirstOrDefault();
            if (itemObj == null)
            {
                throw new Exception($@"WO:{wo}/ID:{item_id} not in o_i137_item!");
            }
            if (!string.IsNullOrEmpty(itemObj.MATERIALID))
            {
                res = true;
            }
            return res;
        }
        static bool IsLinecardBundle(OleExec SFCDB, string wo)
        {
            bool res = false;
            //var itemObj = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN, O_I137_ITEM>((r, o, i) => r.ORIGINALID == o.ID && o.ITEMID == i.ID)
            //    .Where((r, o, i) => o.PREWO == wo && r.VALID == "1").Select((r, o, i) => i).ToList().FirstOrDefault();
            var itemObj = SFCDB.ORM.Queryable<O_ORDER_MAIN, I137_I, I137_H>((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
                .Where((O, I, H) => O.PREWO == wo)
                .Select((O, I, H) => new { I.SOID, H.SALESORDERNUMBER })
                .First();
            if (itemObj == null)
            {
                throw new Exception($@"WO:{wo} not in o_i137_item!");
            }
            var bndlorder = SFCDB.ORM.Queryable<O_ORDER_MAIN, C_SKU, C_SERIES, O_I137_ITEM, O_I137_HEAD>
                ((O, S, C, I, H) => O.PID == S.SKUNO && S.C_SERIES_ID == C.ID && O.ITEMID == I.ID && I.TRANID == H.TRANID)
                .Where((O, S, C, I, H) => C.SERIES_NAME == "Juniper-FRU" && H.SALESORDERNUMBER == itemObj.SALESORDERNUMBER && I.SOID == itemObj.SOID)
                .Select((O, S, C, I, H) => O)
                .First();

            var bndlorders = SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>
                ((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
                .Where((O, I, H) => H.SALESORDERNUMBER == itemObj.SALESORDERNUMBER && I.SOID == itemObj.SOID)
                .Select((O, I, H) => O)
                .ToList();
            if (bndlorder != null && bndlorders.Count > 1)
            {
                res = true;
            }

            return res;
        }

        public static void GetTrsnDataFromFJZAPDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession StrTRSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StrTRSNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession TRSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TRSNSession == null)
            {
                TRSNSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(TRSNSession);
            }
            var db = Station.SFCDB;
            string strwo = WOSession.Value.ToString();
            string strTrsn = StrTRSNSession.Value.ToString();

            var order_main = db.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == strwo).First();
            var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(order_main.PREWO, "JuniperAutoKPConfig", db);

            if (kpl == null || kpl.Count == 0)
            {
                ErrMessage = $@"Can't get JuniperAutoKpConfig !";
                throw new MESReturnMessage(ErrMessage);
            }
            var P_NO = kpl[0].PN_7XX;


            DataTable apdataT = null;
            var strSql = "";
            if (Station.BU == "VNJUNIPER")
            {
                //VN ALLPART 沒有COO信息
                strSql = $@"select a.CUST_KP_NO P_NO, a.DATE_CODE,a.LOT_CODE ,b.MFR_NAME VENDOR,a.MFR_KP_NO MPN,a.qty ORIG_QTY,'' COO 
                    From mes4.r_tr_Sn a,mes1.c_mfr_config b where a.tr_sn='{strTrsn}' and a.mfr_Code=b.mfr_code";
            }
            if (Station.BU == "FJZ")
            {
                strSql = $@"SELECT CONTAINER_ID,VENDOR_MATL_NO MPN, COMPONENT_MATL_NO P_NO, DATE_CODE,LOT_CODE,ORIG_QTY  ,ORIGIN COO,MANUFACTURER VENDOR
                            FROM DCAF.COMPONENT_TRACE @DCAF CT, DCAF.COMPONENT_TRACE_CONTAINERS @DCAF CTC
                            WHERE CT.UTN = CTC.UTN  AND container_id = '{strTrsn}'";
                //Data Source = 10.14.253.219:1521 / jalp; User ID = AP3; Password = nsd0170ap3; pooling = false
                string strApconn = "User Id = AP3; Password = nsd0170ap3; Data Source = (DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.14.253.219)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=jalp)))";
                using (OracleConnection conn = new OracleConnection(strApconn))
                {
                    conn.Open();
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = strSql;
                        cmd.Parameters.AddRange(new OracleParameter[] { });
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        apdataT = new DataTable();
                        adapter.Fill(apdataT);
                    }
                    conn.Close();
                }
                if (apdataT.Rows.Count == 0)
                {
                    strSql = $@"select a.CUST_KP_NO P_NO, a.DATE_CODE,a.LOT_CODE ,b.MFR_NAME VENDOR,a.MFR_KP_NO MPN,a.qty ORIG_QTY,wafer_no COO 
                    From mes4.r_tr_Sn a,mes1.c_mfr_config b where a.tr_sn='{strTrsn}' and a.mfr_Code=b.mfr_code";
                    apdataT = null;
                }

            }

            var apdb = Station.APDB;
            if (apdataT == null)
            {
                apdataT = apdb.RunSelect(strSql).Tables[0];
            }

            if (apdataT.Rows.Count == 0)
            {
                ErrMessage = $@"[DCAF/AP Fail]:Check TR_SN Fail";
                throw new MESReturnMessage(ErrMessage);
            }
            var trsndata = apdataT.Rows[0];
            var VENDOR = trsndata["VENDOR"].ToString();
            var dc = trsndata["DATE_CODE"].ToString();
            var lc = trsndata["LOT_CODE"].ToString();
            var qty = trsndata["ORIG_QTY"].ToString();
            var coo = trsndata["COO"].ToString();
            var pn = trsndata["P_NO"].ToString();
            var mpn = trsndata["MPN"].ToString();
            if (pn != P_NO)
            {
                ErrMessage = $@"TR_SN P_NO:'{pn}' != WO PN:'{P_NO}'";
                throw new MESReturnMessage(ErrMessage);
            }

            var trkp = db.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strTrsn && t.KP_NAME == "TR_SN" && t.VALID_FLAG == 1 && t.DETAILSEQ == 1).First();
            if (trkp == null)
            {
                R_SN_KP kp = new R_SN_KP()
                {
                    ID = MesDbBase.GetNewID(db.ORM, Station.BU, "R_SN_KP"),
                    ITEMSEQ = 1,
                    SCANSEQ = 1,
                    DETAILSEQ = 1,
                    KP_NAME = "TR_SN",
                    PARTNO = P_NO,
                    MPN = mpn,
                    SN = strTrsn,
                    VALUE = VENDOR,
                    STATION = Station.StationName,
                    EXKEY1 = "COO",
                    EXVALUE1 = coo,
                    EXKEY2 = "QTY",
                    EXVALUE2 = qty,
                    SCANTYPE = "VENDOR",
                    VALID_FLAG = 1,
                    EDIT_EMP = Station.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now
                };
                R_SN_KP kp1 = new R_SN_KP()
                {
                    ID = MesDbBase.GetNewID(db.ORM, Station.BU, "R_SN_KP"),
                    ITEMSEQ = 1,
                    SCANSEQ = 1,
                    DETAILSEQ = 2,
                    KP_NAME = "TR_SN",
                    PARTNO = P_NO,
                    MPN = mpn,
                    SN = strTrsn,
                    VALUE = dc,
                    STATION = Station.StationName,
                    EXKEY1 = "LOT_CODE",
                    EXVALUE1 = lc,
                    SCANTYPE = "DATE_CODE",
                    VALID_FLAG = 1,
                    EDIT_EMP = Station.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now
                };
                db.ORM.Insertable(kp).ExecuteCommand();
                db.ORM.Insertable(kp1).ExecuteCommand();
                trkp = kp;
            }
            TRSNSession.Value = trkp;



        }

        public static void JIATypeDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession TypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TypeSession == null)
            {
                TypeSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(TypeSession);
            }

            TypeSession.Value = "No JIA";
            var WO = (WorkOrder)WOSession.Value;
            string strWO = WO.WorkorderNo;
            var db = Station.SFCDB;

            var res = db.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((m, i, h) => m.ITEMID == i.ID && i.TRANID == h.TRANID).Where
                ((m, i, h) => m.PREWO == strWO).Select((m, i, h) => new { h.SHIPTOCOMPANY, h.SHIPTOCOUNTRYCODE, m.POTYPE }).First();

            if (res == null)
            {
                throw new Exception($@"Can't get SHIPTOCOMPANY and SHIPTOCOUNTRYCODE by wo:{strWO}");
            }

            //if (res.POTYPE.Trim().ToUpper() == "BTS")
            //    return;

            var custs = db.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "JUNIPER_JIA_CUST" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE == "NOSYSTEM")
                .Select(t => t.VALUE).ToList();

            string _shiptocompany = res.SHIPTOCOMPANY.Trim().ToUpper();
            string _shiptocountrycode = res.SHIPTOCOUNTRYCODE.Trim().ToUpper();

            foreach (var item in custs)
            {
                if (_shiptocompany.Contains(item.Trim().ToUpper()))
                {
                    TypeSession.Value = "Is JIA";
                    break;
                }
            }

            if (_shiptocountrycode == "JP")
            {
                TypeSession.Value = "Is JIA";
            }

        }

        public static void TR_SN_WHS_DataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession StrTRSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (StrTRSNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession Original_QTY = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Original_QTY == null)
            {
                Original_QTY = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(Original_QTY);
            }

            string strTrsn = StrTRSNSession.Value.ToString();
            Original_QTY.Value = "0";

            DataTable apdataT = null;
            var strSql = "";
            if (Station.BU == "FJZ")
            {
                //string strApconn = "User Id = AP3; Password = nsd0170ap3; Data Source = (DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.14.253.219)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=jalp)))";
                string strApconn = "User Id = FUSER; Password = Fuser#01; Data Source = (DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.14.253.219)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=jalp)))";

                string sql = string.Format(@"
                        SELECT a.ID, a.TR_SN, a.ORIGINAL_QTY, a.CHECKOUT_QTY, a.CHECKOUT_BY, a.CHECKOUT_TIME, a.RETURN_QTY, a.RETURN_BY, a.RETURN_TIME
                        FROM MES4.R_WHS_MATL_WIP_CTRL a where a.TR_SN = '{0}'", strTrsn);

                DataTable checkDt = null;

                using (OracleConnection conn = new OracleConnection(strApconn))
                {
                    conn.Open();
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.AddRange(new OracleParameter[] { });
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        checkDt = new DataTable();
                        adapter.Fill(checkDt);
                    }
                    conn.Close();
                }

                if (checkDt != null)
                {
                    if (checkDt.Rows.Count > 0)
                    {
                        ErrMessage = " TR_SN: " + strTrsn + "  " + MESReturnMessage.GetMESReturnMessage("MSGCODE20210815130448");
                        throw new MESReturnMessage(ErrMessage);
                    }
                }

                strSql = $@"SELECT CONTAINER_ID,VENDOR_MATL_NO MPN, COMPONENT_MATL_NO P_NO, DATE_CODE,LOT_CODE,ORIG_QTY  ,ORIGIN COO,MANUFACTURER VENDOR
                            FROM DCAF.COMPONENT_TRACE @DCAF CT, DCAF.COMPONENT_TRACE_CONTAINERS @DCAF CTC
                            WHERE CT.UTN = CTC.UTN  AND container_id = '{strTrsn}'";

                using (OracleConnection conn = new OracleConnection(strApconn))
                {
                    conn.Open();
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = strSql;
                        cmd.Parameters.AddRange(new OracleParameter[] { });
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        apdataT = new DataTable();
                        adapter.Fill(apdataT);
                    }
                    conn.Close();
                }
            }

            if (apdataT.Rows.Count == 0)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210815131350", new string[] { strTrsn });
                throw new MESReturnMessage(ErrMessage);
            }
            var trsndata = apdataT.Rows[0];
            var qty = trsndata["ORIG_QTY"].ToString();


            Original_QTY.Value = qty;

        }

    }
}
