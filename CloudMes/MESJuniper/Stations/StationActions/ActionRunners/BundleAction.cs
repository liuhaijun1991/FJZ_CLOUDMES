using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDataObject;
using MESDataObject.Common;
using System.Data;
using MESDBHelper;
using MESDataObject.Module.OM;
using MESDataObject.Module.Juniper;
using SqlSugar;

namespace MESJuniper.Stations.StationActions.ActionRunners
{
    public class BundleAction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void BundleSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            if (SessionSOID == null || SessionSOID.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession SessionBndlNo = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (SessionBndlNo == null || SessionBndlNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            MESStationSession SessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (SessionSN == null || SessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }
            MESStationSession SessionPrintSwitch = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (SessionPrintSwitch == null)
            {
                SessionPrintSwitch = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, Value = "FALSE" };
                Station.StationSession.Add(SessionPrintSwitch);
            }
            string so = SessionSO.Value.ToString();
            string soid = SessionSOID.Value.ToString();
            string bndlno = SessionBndlNo.Value.ToString();
            string sn = SessionSN.Value.ToString();
            var bndlinfo = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD, R_SN>((O, I, H, S) => O.ITEMID == I.ID && I.TRANID == H.TRANID && S.WORKORDERNO == O.PREWO)
                .Where((O, I, H, S) => I.SOID == soid && H.SALESORDERNUMBER == so && S.VALID_FLAG == "1" && S.SN == sn)
                .Select((O, I, H, S) => new { O, I, H, S })
                .First();
            var totalQty = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
                .Where((O, I, H) => I.SOID == soid && H.SALESORDERNUMBER == so)
                .Select<int>((O, I, H) => SqlFunc.AggregateSum(Convert.ToInt32(I.QUANTITY)))
                .First();
            var x = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                    .Where(t => t.SALEORDER == so && t.SOID == soid && t.VALID_FLAG == "1")
                    .ToList();
            if (x.Count == totalQty)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_BNDL_FINISH", new string[] { so, soid }));
            }
            var bndl = new R_JUNIPER_BUNDLE()
            {
                ID = MesDbBase.GetNewID<R_JUNIPER_BUNDLE>(Station.SFCDB.ORM, Station.BU),
                BNDL_NO = bndlno,
                R_SN_ID = bndlinfo.S.ID,
                SN = bndlinfo.S.SN,
                WORKORDERNO = bndlinfo.S.WORKORDERNO,
                SKUNO = bndlinfo.I.PN,
                PONUMBER = bndlinfo.O.PONO,
                POLINE = bndlinfo.O.POLINE,
                SALEORDER = bndlinfo.H.SALESORDERNUMBER,
                SOITEM = bndlinfo.I.SALESORDERLINEITEM,
                SOID = bndlinfo.I.SOID,
                SCANBY = Station.LoginUser.EMP_NO,
                BNDLDATETIME = DateTime.Now,
                VALID_FLAG = "1"
            };
            Station.SFCDB.ORM.Insertable(bndl).ExecuteCommand();
            x = Station.SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                    .Where(t => t.BNDL_NO == bndlno && t.VALID_FLAG == "1")
                    .ToList();
            if (x.Count == (totalQty / Convert.ToInt32(Convert.ToDouble(bndlinfo.I.SOQTY))))
            {
                SessionPrintSwitch.Value = "TRUE";
            }
        }
    }
}
