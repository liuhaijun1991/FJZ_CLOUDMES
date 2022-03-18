using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESJuniper.Stations.StationActions.DataCheckers
{
    public class BundleChecker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckBundleSN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 4)
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
            MESStationSession SessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            MESStationSession SessionBndlNo = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (SessionBndlNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }
            string so = SessionSO.Value.ToString();
            string soid = SessionSOID.Value.ToString();
            string sn = SessionSN.Value.ToString();
            string bndlNo = SessionBndlNo.Value.ToString();
            var bndlsn = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>().Where(t => t.SN == sn && t.VALID_FLAG == "1").First();
            if (bndlsn != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154006", new string[] { sn, "Bundle" }));
            }
            var snobj = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD, R_SN>((O, I, H, S) => O.ITEMID == I.ID && I.TRANID == H.TRANID && S.WORKORDERNO == O.PREWO)
                .Where((O, I, H, S) => I.SOID == soid && H.SALESORDERNUMBER == so && S.VALID_FLAG == "1" && S.SN == sn)
                .Select((O, I, H, S) => new { O, I, H, S })
                .First();
            var RequestQty = Convert.ToInt32(double.Parse(snobj.I.QUANTITY) / double.Parse(snobj.I.SOQTY));
            if (snobj == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_BNDL_SNNOTEXISTS", new string[] { sn }));
            }
            if (snobj.S.NEXT_STATION != "CBS")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_BNDL_SNSTATUS", new string[] { snobj.S.SN, snobj.S.NEXT_STATION }));
            }
            var bndl = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.BNDL_NO == bndlNo && t.WORKORDERNO == snobj.O.PREWO && t.VALID_FLAG == "1")
                .ToList();
            if (bndl.Count == RequestQty)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_BNDL_SNQTYMORE", new string[] { snobj.S.SKUNO, RequestQty.ToString() }));
            }
        }
        public static void CheckSNBundleStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null || SessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            
            string sn = SessionSN.Value.ToString();
            var bndlsn = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>().Where(t => t.SN == sn && t.VALID_FLAG == "1").First();
            if (bndlsn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_BNDL_NOTFINISH", new string[] { sn }));
            }
            var bndldata = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>().Where(t => t.BNDL_NO == bndlsn.BNDL_NO && t.VALID_FLAG == "1").ToList();
            var soinfoList = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
            .Where((O, I, H) => I.SOID == bndlsn.SOID && H.SALESORDERNUMBER == bndlsn.SALEORDER)
            .Select((O, I, H) => new { O, I, H })
            .Distinct()
            .ToList();
            for (int i = 0; i < soinfoList.Count; i++)
            {
                var bCount = bndldata.Where(t => t.WORKORDERNO == soinfoList[i].O.PREWO).ToList().Count;
                var NeedQty = Convert.ToInt32(double.Parse(soinfoList[i].I.QUANTITY) / double.Parse(soinfoList[i].I.SOQTY));
                if (bCount != NeedQty)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_BNDL_SNQTYMEET", new string[] { soinfoList[i].O.PID }));
                }
            }
        }

    }
}

