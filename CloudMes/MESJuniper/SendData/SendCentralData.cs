using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.ModuleHelp;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.Common;
using MESPubLab.MesBase;
using SqlSugar;
using MESDBHelper;
using MESPubLab.MesException;
using static MESDataObject.Common.EnumExtensions;
using System.Data;
using MESPubLab.MESStation;
using MESJuniper.OrderManagement;
using static MESDataObject.Constants.PublicConstants;
using MESJuniper.Api;

namespace MESJuniper.SendData
{
    public class SendCentralData
    {
        public List<OrderCenTralModle> GetMESOrderData(string dbstr)
        {
            using (var db = OleExec.GetSqlSugarClient(dbstr, false))
            {
                var noQstatus = @"12,14,31,28,29,31,32".Split(',');
                var orderList = db.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_PO_STATUS_MAP_J, O_ORDER_HOLD, I137_I, I137_H, O_EXCEPTION_DATA, R_WO_GROUPID>((m, s, j, h, i, oh, e, f) =>
                    new object[] { JoinType.Inner,m.ID == s.POID  && s.VALIDFLAG == MesBool.Yes.ExtValue() && !noQstatus.Contains(s.STATUSID) ,JoinType.Left,s.STATUSID==j.NAME,JoinType.Left,m.ITEMID==h.ITEMID,
                          JoinType.Inner, m.ITEMID == i.ID,JoinType.Inner, i.TRANID==oh.TRANID, JoinType.Left,m.UPOID==e.UPOID && e.STATUS == JuniperErrStatus.Open.ExtValue(),
                      JoinType.Left,m.PREWO == f.WO})
                 .Where((m, s, j, h, i, oh, e, f) => m.CUSTOMER == Customer.JUNIPER.ExtValue() && m.CANCEL == MesBool.No.ExtValue() )
                 //.WhereIF(plant != "ALL", (m, s, j, h, i, oh, e, f) => m.PLANT == plant)
                 .OrderBy((m, s, j, h, i, oh, e, f) => m.PONO, OrderByType.Asc)
                 .Select((m, s, j, h, i, oh, e, f) =>
                     new OrderCenTralModle
                     {
                         PLANT = m.PLANT,
                         HEADERSCHSTATUS = oh.HEADERSCHEDULINGSTATUS,
                         LINESCHSTATUS = i.LINESCHEDULINGSTATUS,
                         COMPLETEDELIVER = oh.COMPLETEDELIVERY,
                         UPOID = m.UPOID,
                         POTYPE = m.POTYPE,
                         PONO = m.PONO,
                         POLINE = m.POLINE,
                         VERSION = m.VERSION,
                         POSTATUS = j.DESCRIPTION,
                         HOLD = h.HOLDREASON,
                         QTY = m.QTY,
                         UNITPRICE = m.UNITPRICE,
                         PREWO = m.PREWO,
                         GROUPID = f.GROUPID,
                         PID = m.PID,
                         JUNIPERPID = i.PN,
                         DELIVERY = m.DELIVERY,
                         CRSD = i.CUSTREQSHIPDATE,
                         SO = oh.SALESORDERNUMBER,
                         SOLN = i.SALESORDERLINEITEM,
                         USERITEMTYPE = m.USERITEMTYPE,
                         OFFERINGTYPE = m.OFFERINGTYPE,
                         LASTCHANGETIME = m.LASTCHANGETIME,
                         COMPLETED = m.COMPLETED,
                         COMPLETIME = m.COMPLETIME,
                         CLOSED = m.CLOSED,
                         CLOSETIME = m.CLOSETIME,
                         CANCEL = m.CANCEL,
                         CANCELTIME = m.CANCELTIME,
                         CUSTOMSW = i.SWTYPE,
                         ECO_FCO = oh.ECO_FCO,
                         COUNTRYSPECIFICLABE = i.COUNTRYSPECIFICLABEL,
                         CARTONLABEL1 = i.CARTONLABEL1,
                         CARTONLABEL2 = i.CARTONLABEL2,
                         PACKOUTLABEL = i.PACKOUTLABEL,
                         CREATETIME = m.CREATETIME,
                         EDITTIME = m.EDITTIME,
                         //ORIGINALID = m.ORIGINALID,
                         //ORIGINALITEMID = m.ORIGINALITEMID,
                         //ITEMID = m.ITEMID,
                         //ORDERTYPE = m.ORDERTYPE,
                         //EXCEPTIONINFO = e.EXCEPTIONINFO,
                         TAA = i.TAAINDICATOR,
                         DELIVERYPRIORITY = i.DELIVERYPRIORITY,
                         PODOCTYPE = m.ORDERTYPE,
                         SOID = i.SOID
                     }).PartitionBy(m => m.UPOID).Distinct().ToList();
                var bundles = orderList.Where(t => t.SOID != "000000").GroupBy(t => new { t.PONO, t.SO, t.SOID }).Select(x => new { x.Key.PONO, x.Key.SO, x.Key.SOID, Count = x.Count() }).Where(p => p.Count > 1).ToList();   
                var otherinfo = db.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_I137_HEAD, O_I138>((m, s, h, o) => new object[] {JoinType.Inner, s.POID == m.ID && s.VALIDFLAG==MesBool.Yes.ExtValue() && !noQstatus.Contains(s.STATUSID),JoinType.Inner,m.PONO == h.PONUMBER
                    , JoinType.Left,m.PONO == o.PONUMBER && m.POLINE == o.POITEMNUMBER }).Where((m, s, h, o) => m.CUSTOMER == Customer.JUNIPER.ExtValue() && m.CANCEL == MesBool.No.ExtValue())
                    .GroupBy((m, s, h, o) => new { m.UPOID })
                    .Select((m, s, h, o) => new { m.UPOID, i138date = SqlFunc.AggregateMax(o.CREATETIME), fdate = SqlFunc.AggregateMin(h.LASTCHANGEDATETIME) })
                    .ToList();
                foreach (var item in orderList)
                {
                    if (bundles.FindAll(t => t.PONO == item.PONO && t.SOID == item.SOID).Any())
                        item.BUNDLE = "Yes";
                    else
                        item.SO_HIGHLINEID = "";
                    var other = otherinfo.FindAll(t => t.UPOID == item.UPOID).FirstOrDefault();
                    item.PO_ITEM = $@"{item.PONO}-{item.POLINE}";
                    item.SO_LN = $@"{item.SO}-{item.SOLN}";
                    item.SO_SS = item.COMPLETEDELIVER == "NA" ? $@"{item.SO}-{item.SOLN}" : $@"{item.SO}-{item.COMPLETEDELIVER}";
                    item.SO_HIGHLINEID = $@"{item.SO}-{item.SOID}";
                    item.LAST138SUBMISSION = other == null ? null : other.i138date;
                    item.CREATION_DATE = other == null ? null : Convert.ToDateTime(other.fdate).ToString(MES_CONST_DATETIME_FORMAT.Normal.ExtValue());
                }
                return orderList;
            }
        }

        public List<WoDetailReleased> GetWoDetailReleased(string dbstr)
        {
            using (var db = OleExec.GetSqlSugarClient(dbstr, false))
            {
                return db.Ado.SqlQuery<WoDetailReleased>($@"
                        select * from (
                        SELECT A.PONO,A.POLINE,A.PID,W.GROUPID,A.PREWO,A.QTY,
                        CASE WHEN D.REQUESTQTY IS NULL THEN 0
                          ELSE TO_NUMBER(A.QTY)*TO_NUMBER(D.REQUESTQTY) END WOREQUESTQTY,
                        CASE WHEN C.SFC_QTY IS NULL then 0 else TO_NUMBER(C.SFC_QTY) end SFC_QTY,D.PARTNO,D.REQUESTQTY,D.PARTNOTYPE,SYSDATE,O.OFFERING_TYPE FROM O_ORDER_MAIN A INNER JOIN O_PO_STATUS P ON A.ID=P.POID LEFT JOIN (
                        SELECT * FROM (
                        SELECT A1.*, ROW_NUMBER() OVER(PARTITION BY SKUNO ORDER BY SAP_STATION_CODE DESC) NUMBS FROM C_SAP_STATION_MAP A1) WHERE NUMBS='1') B ON A.PID=B.SKUNO LEFT JOIN 
                        (SELECT* FROM (
                        SELECT  A2.*, ROW_NUMBER() OVER(PARTITION BY SKUNO,SAP_STATION ORDER BY BACK_DATE DESC) NUMBS FROM R_BACKFLUSH_HISTORY A2 WHERE A2.RESULT='Y') WHERE  NUMBS='1') C 
                        ON A.PREWO=C.WORKORDERNO AND B.SAP_STATION_CODE=C.SAP_STATION  
                        LEFT JOIN R_WO_GROUPID W ON A.PREWO=W.WO 
                        LEFT JOIN R_PRE_WO_DETAIL D ON A.PREWO=D.WO
                        LEFT JOIN O_AGILE_ATTR O ON D.PARTNO=O.ITEM_NUMBER
                        WHERE A.CANCEL='0' AND P.VALIDFLAG='1' and a.ordertype<>'ZRMQ' AND P.STATUSID IN ('9','10','11') AND O.ACTIVED='1' ) r where  QTY<>r.sfc_qty
                        ").ToList();
                //var Qstatus = @"9,10,11".Split(',');
                //var orderList = db.Queryable<O_ORDER_MAIN, O_PO_STATUS, R_WO_GROUPID,R_PRE_WO_DETAIL,O_AGILE_ATTR>((m,s,g,w,a) =>
                //    new object[] { JoinType.Inner,m.ID == s.POID  && s.VALIDFLAG == MesBool.Yes.ExtValue() && Qstatus.Contains(s.STATUSID),JoinType.Left,m.PREWO==g.WO,JoinType.Left,g.WO==w.WO,
                //    JoinType.Left,w.PARTNO==a.ITEM_NUMBER && a.ACTIVED == MesBool.Yes.ExtValue()})
                // .Where((m, s, g, w,a) => m.CUSTOMER == Customer.JUNIPER.ExtValue() && m.CANCEL == MesBool.No.ExtValue() && m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ExtValue())
                // .Select((m, s, g, w,a) =>
                //     new WoDetailReleased
                //     {
                //         PONO = m.PONO,
                //         POLINE = m.POLINE,
                //         PID = m.PID,
                //         GROUPID = g.GROUPID,
                //         WO = m.PREWO,
                //         WOQTY = m.QTY,
                //         PARTNO = w.PARTNO,
                //         REQUESTQTY = w.REQUESTQTY,
                //         PARTNOTYPE = w.PARTNOTYPE,
                //         CREATETIME = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.Normal.ExtValue()),
                //         OfferingType = a.OFFERING_TYPE
                //     }).Distinct().ToList();
                //var res = new List<WoDetailReleased>();
                //var wobackflush = db.Queryable<O_ORDER_MAIN, O_PO_STATUS,R_BACKFLUSH_HISTORY>((m, s,b) =>
                //     new object[] { JoinType.Inner,m.ID == s.POID  && s.VALIDFLAG == MesBool.Yes.ExtValue() && Qstatus.Contains(s.STATUSID) ,JoinType.Left,m.PREWO == b.WORKORDERNO})
                // .Where((m, s, b) => m.CUSTOMER == Customer.JUNIPER.ExtValue() && m.CANCEL == MesBool.No.ExtValue() && m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ExtValue())
                // .Select((m, s, b) =>new { m.PREWO,});
                //foreach (var item in orderList)
                //{                     
                //    res.Add(item);
                //}
                //return orderList;
            }
        }

        public List<WoDetailNoReleased> GetWoDetailNoReleased(string dbstr)
        {
            using (var db = OleExec.GetSqlSugarClient(dbstr, false))
            {
                return db.Ado.SqlQuery<WoDetailNoReleased>($@"                        
                        SELECT A.PONO,A.POLINE,A.PID,W.GROUPID,A.PREWO,A.QTY,
                        CASE WHEN D.REQUESTQTY IS NULL THEN 0
                          ELSE TO_NUMBER(A.QTY)*TO_NUMBER(D.REQUESTQTY) END WOREQUESTQTY,D.PARTNO,D.REQUESTQTY,D.PARTNOTYPE,SYSDATE,O.OFFERING_TYPE FROM O_ORDER_MAIN A INNER JOIN O_PO_STATUS P ON A.ID=P.POID 
                        LEFT JOIN R_WO_GROUPID W ON A.PREWO=W.WO 
                        LEFT JOIN R_PRE_WO_DETAIL D ON A.PREWO=D.WO
                        LEFT JOIN O_AGILE_ATTR O ON D.PARTNO=O.ITEM_NUMBER
                        WHERE A.CANCEL='0' AND P.VALIDFLAG='1'  AND P.STATUSID IN ('0','1','2','3','4','5','6','7','8','13') AND O.ACTIVED='1' 
                        ").ToList();
            }
        }

        
        public class OrderCenTralModle
        {
            //public string ID { get; set; }
            public string SOID { get; set; }
            [PropertiesDesc("POTYPE")]
            public string POTYPE { get; set; }
            [PropertiesDesc("PONO")]
            public string PONO { get; set; }
            [PropertiesDesc("POLINE")]
            public string POLINE { get; set; }
            [PropertiesDesc("PO-ITEM")]
            public string PO_ITEM { get; set; }
            [PropertiesDesc("PREWO")]
            public string PREWO { get; set; }
            [PropertiesDesc("JUNIPERPID")]
            public string JUNIPERPID { get; set; }
            [PropertiesDesc("PLANT")]
            public string PLANT { get; set; }
            [PropertiesDesc("HEADERSCHSTATUS")]
            public string HEADERSCHSTATUS { get; set; }
            [PropertiesDesc("LINESCHSTATUS")]
            public string LINESCHSTATUS { get; set; }
            [PropertiesDesc("COMPLETEDELIVER")]
            public string COMPLETEDELIVER { get; set; }
            [PropertiesDesc("LAST138SUBMISSION")]
            public DateTime? LAST138SUBMISSION { get; set; }
            [PropertiesDesc("UPOID")]
            public string UPOID { get; set; }
            [PropertiesDesc("VERSION")]
            public string VERSION { get; set; }
            [PropertiesDesc("POSTATUS")]
            public string POSTATUS { get; set; }
            [PropertiesDesc("HOLD")]
            public string HOLD { get; set; }
            [PropertiesDesc("QTY")]
            public string QTY { get; set; }
            [PropertiesDesc("UNITPRICE")]
            public string UNITPRICE { get; set; }
            [PropertiesDesc("GROUPID")]
            public string GROUPID { get; set; }
            [PropertiesDesc("PLANTCODE")]
            public string PLANTCODE { get; set; }
            [PropertiesDesc("PID")]
            public string PID { get; set; }
            [PropertiesDesc("DELIVERY")]
            public DateTime? DELIVERY { get; set; }
            [PropertiesDesc("CRSD")]
            public DateTime? CRSD { get; set; }
            [PropertiesDesc("SO")]
            public string SO { get; set; }
            [PropertiesDesc("SOLN")]
            public string SOLN { get; set; }
            [PropertiesDesc("SO-LN")]
            public string SO_LN { get; set; }
            [PropertiesDesc("USERITEMTYPE")]
            public string USERITEMTYPE { get; set; }
            [PropertiesDesc("OFFERINGTYPE")]
            public string OFFERINGTYPE { get; set; }
            [PropertiesDesc("LASTCHANGETIME")]
            public string LASTCHANGETIME { get; set; }
            [PropertiesDesc("COMPLETED")]
            public string COMPLETED { get; set; }
            [PropertiesDesc("COMPLETIME")]
            public DateTime? COMPLETIME { get; set; }
            [PropertiesDesc("CLOSED")]
            public string CLOSED { get; set; }
            [PropertiesDesc("CLOSETIME")]
            public DateTime? CLOSETIME { get; set; }
            [PropertiesDesc("CANCEL")]
            public string CANCEL { get; set; }
            [PropertiesDesc("CANCELTIME")]
            public DateTime? CANCELTIME { get; set; }
            [PropertiesDesc("CUSTOMSW")]
            public string CUSTOMSW { get; set; }
            [PropertiesDesc("ECO_FCO")]
            public string ECO_FCO { get; set; }
            [PropertiesDesc("COUNTRYSPECIFICLABE")]
            public string COUNTRYSPECIFICLABE { get; set; }
            [PropertiesDesc("CARTONLABEL1")]
            public string CARTONLABEL1 { get; set; }
            [PropertiesDesc("CARTONLABEL2")]
            public string CARTONLABEL2 { get; set; }
            [PropertiesDesc("PACKOUTLABEL")]
            public string PACKOUTLABEL { get; set; }
            [PropertiesDesc("CREATETIME")]
            public DateTime? CREATETIME { get; set; }
            [PropertiesDesc("EDITTIME")]
            public DateTime? EDITTIME { get; set; }
            [PropertiesDesc("TAA")]
            public string TAA { get; set; }
            [PropertiesDesc("Bundle")]
            public string BUNDLE { get; set; }
            [PropertiesDesc("DELIVERYPRIORITY")]
            public string DELIVERYPRIORITY { get; set; }
            [PropertiesDesc("PODOCTYPE")]
            public string PODOCTYPE { get; set; }
            [PropertiesDesc("SO-SS")]
            public string SO_SS { get; set; }
            [PropertiesDesc("SO-HIGHLINEID")]
            public string SO_HIGHLINEID { get; set; }
            [PropertiesDesc("1st_VERSION_CREATION_DATE")]
            public string CREATION_DATE { get; set; }
        }

        public class WoDetailReleased
        {
            [PropertiesDesc("PONO")]
            public string PONO { get; set; }
            [PropertiesDesc("POLINE")]
            public string POLINE { get; set; }
            [PropertiesDesc("PID")]
            public string PID { get; set; }
            [PropertiesDesc("GROUPID")]
            public string GROUPID { get; set; }
            [PropertiesDesc("WO")]
            public string WO { get; set; }
            [PropertiesDesc("WOQTY")]
            public string WOQTY { get; set; }
            [PropertiesDesc("WOREQUESTQTY")]
            public string WOREQUESTQTY { get; set; }
            [PropertiesDesc("QUANTITY_WITHDRAWN")]
            public string QuantityWithdrawn { get; set; }
            [PropertiesDesc("PARTNO")]
            public string PARTNO { get; set; }
            [PropertiesDesc("REQUESTQTY")]
            public string REQUESTQTY { get; set; }
            [PropertiesDesc("PARTNOTYPE")]
            public string PARTNOTYPE { get; set; }
            [PropertiesDesc("CREATETIME")]
            public string CREATETIME { get; set; }
            [PropertiesDesc("OFFERING_TYPE")]
            public string OFFERING_TYPE { get; set; }

        }
        public class WoDetailNoReleased
        {
            [PropertiesDesc("PONO")]
            public string PONO { get; set; }
            [PropertiesDesc("POLINE")]
            public string POLINE { get; set; }
            [PropertiesDesc("PID")]
            public string PID { get; set; }
            [PropertiesDesc("GROUPID")]
            public string GROUPID { get; set; }
            [PropertiesDesc("WO")]
            public string WO { get; set; }
            [PropertiesDesc("WOQTY")]
            public string WOQTY { get; set; }
            [PropertiesDesc("WOREQUESTQTY")]
            public string WOREQUESTQTY { get; set; }
            [PropertiesDesc("PARTNO")]
            public string PARTNO { get; set; }
            [PropertiesDesc("REQUESTQTY")]
            public string REQUESTQTY { get; set; }
            [PropertiesDesc("PARTNOTYPE")]
            public string PARTNOTYPE { get; set; }
            [PropertiesDesc("CREATETIME")]
            public string CREATETIME { get; set; }
            [PropertiesDesc("OFFERING_TYPE")]
            public string OFFERING_TYPE { get; set; }

        }
    }

}
