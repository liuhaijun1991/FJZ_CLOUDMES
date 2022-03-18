using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESJuniper.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Constants.PublicConstants;

namespace MESJuniper.OrderManagement
{
    public class JuniperOmBase
    {
        /// <summary>
        /// check order hold;
        /// </summary>
        /// <param name="mainid">O_ORDER_MAIN.ID</param>
        /// <param name="control">ENUM_O_ORDER_HOLD_CONTROLTYPE</param>
        /// <returns></returns>
        public static JuniperHoldResult JuniperHoldCheck(string mainid, ENUM_O_ORDER_HOLD_CONTROLTYPE control, SqlSugarClient db)
        {
            var holdobjs = db.Queryable<O_ORDER_MAIN, O_ORDER_HOLD>((m, h) => m.ITEMID == h.ITEMID).Where((m, h) => m.ID == mainid && h.HOLDFLAG == MesBool.Yes.ExtValue()).Select((m, h) => h).ToList();
            foreach (var item in holdobjs)
            {
                var holdreasons = item.HOLDREASON.Split(',');
                var holdobj = new List<O_J_HOLD_C>();
                switch (control)
                {
                    case ENUM_O_ORDER_HOLD_CONTROLTYPE.CREATEWO:
                        holdobj = db.Queryable<O_J_HOLD_C>().Where(t => holdreasons.Contains(t.HOLDCODE) && t.CREATEWO == MesBool.No.ExtValue()).ToList();
                        break;
                    case ENUM_O_ORDER_HOLD_CONTROLTYPE.PRODUCTION:
                        holdobj = db.Queryable<O_J_HOLD_C>().Where(t => holdreasons.Contains(t.HOLDCODE) && t.PRODUCTION == MesBool.No.ExtValue()).ToList();
                        break;
                    case ENUM_O_ORDER_HOLD_CONTROLTYPE.PREASN:
                        holdobj = db.Queryable<O_J_HOLD_C>().Where(t => holdreasons.Contains(t.HOLDCODE) && t.PREASN == MesBool.No.ExtValue()).ToList();
                        break;
                    case ENUM_O_ORDER_HOLD_CONTROLTYPE.FINALASN:
                        holdobj = db.Queryable<O_J_HOLD_C>().Where(t => holdreasons.Contains(t.HOLDCODE) && t.FINALASN == MesBool.No.ExtValue()).ToList();
                        break;
                    case ENUM_O_ORDER_HOLD_CONTROLTYPE.I138:
                        holdobj = db.Queryable<O_J_HOLD_C>().Where(t => holdreasons.Contains(t.HOLDCODE) && t.I138 == MesBool.No.ExtValue()).ToList();
                        break;
                    default:
                        break;
                }
                if (holdobj.Count > 0)
                {
                    return new JuniperHoldResult()
                    {
                        HoldFlag = true,
                        HoldReason = new Func<string>(() =>
                        {
                            var holdreaion = "";
                            foreach (var holdi in holdobj)
                                holdreaion += (string.IsNullOrEmpty(holdreaion) ? "" : ",") + holdi.HOLDCODE;
                            return holdreaion;
                        })(),
                        ControlType = control
                    };
                }
            }
            return new JuniperHoldResult()
            {
                HoldFlag = false,
                HoldReason = string.Empty,
                ControlType = control
            };
        }

        public static void JuniperI054AckCheck(string sn, SqlSugarClient db)
        {
            string msg = string.Empty;
            var r_sn = db.Ado.SqlQuery<R_SN>(@"
                        SELECT *
                          FROM R_SN S
                         WHERE EXISTS (SELECT 1
                                  FROM o_order_main wo, o_sku_config sku
                                 WHERE S.workorderno = wo.prewo
                                   AND wo.useritemtype = sku.useritemtype
                                   AND wo.potype = sku.producttype
                                   AND wo.offeringtype = sku.offeringtype
                                   AND sku.i054 = 'Y'
                                union
                                SELECT 1
                                  FROM C_SKU, C_SERIES SER
                                 WHERE C_SKU.C_SERIES_ID = SER.ID
                                   AND C_SKU.SKUNO = S.SKUNO
                                   AND SER.SERIES_NAME LIKE 'JNP-ODM%')
                           AND S.sn = @sn", new { sn })
                           .FirstOrDefault();
            if (r_sn == null)
            {
                return;
            }
            var r_i054_ack = db.Ado.SqlQuery<R_I054_ACK>(@"
                        SELECT A.*
                          FROM (SELECT *
                                  FROM (SELECT C.*,
                                               ROW_NUMBER() OVER(PARTITION BY PARENTSN ORDER BY CREATETIME DESC) NUMBS
                                          FROM R_I054 C
                                         WHERE C.PNTYPE = 'Parent'
                                           AND C.PARENTSN = @sn)
                                 WHERE NUMBS = 1) D
                          LEFT JOIN R_I054_ACK A
                            ON D.TRANID = A.TRANID", new { sn })
                         .FirstOrDefault();
            if (r_i054_ack == null)
            {
                var log = db.Ado.SqlQuery<R_MES_LOG>(@"
                                SELECT *
                                  FROM r_mes_log 
                                 WHERE PROGRAM_NAME = 'MESInterface'
                                   AND FUNCTION_NAME = 'GenerateAS_Build_Data'
                                   AND DATA1=@sn
                                   AND DATA4 IS NULL",
                                   new { sn })
                                   .ToList();
                if (log.Count > 0)
                {
                    throw new MESReturnMessage("Generate I054 Data With Error: " + log[0].LOG_MESSAGE);
                }
                msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210118093153", new string[] { "Create I054" });
                throw new MESReturnMessage(msg);
            }
            if (r_i054_ack.RESPONSEMESSAGE == "" || r_i054_ack.RESPONSEMESSAGE == null)
            {
                //SN Has Not Received {0} Data,Please Wait.
                msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210118093153", new string[] { "Receive Juniper I054 ACK" });
                throw new MESReturnMessage(msg);
            }
            if (r_i054_ack.RESPONSEMESSAGE != "Success")
            {
                //Receive {0} Exception 【{1}】,Please Go To {2} To Handle.
                msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210118093511", new string[] { "Juniper I054 ACK", r_i054_ack.RESPONSEMESSAGE, "[OM=>I054 ACK]" });
                throw new MESReturnMessage(msg);
            }
        }

        public static void JuniperI244Check(string sn, SqlSugarClient db)
        {
            string msg = string.Empty;
            var sns = db.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").First();
            var ordermain = db.Queryable<R_SN, O_ORDER_MAIN>((S, O) => S.WORKORDERNO == O.PREWO).Where((S, O) => S.SN == sn).Select((S, O) => O).First();

            var sku = db.Queryable<C_SKU, C_SERIES>((S, C) => S.C_SERIES_ID == C.ID)
                        .Where((S, C) => S.SKUNO == sns.SKUNO)
                        .Select((S, C) => new { S, C })
                        .First();

            //VN ODM產品不用檢查I244 Asked By PE&PM 2021-11-29; Add PCBA Type no need to check I244 2022-01-05
            if (ordermain == null && (sku.C.SERIES_NAME.StartsWith("JNP-ODM") || sku.S.SKU_TYPE.Equals("PCBA")))
            {
                return;
            }

            if (ordermain == null && sku.C.SERIES_NAME != "Juniper-Configurable System MA-FVN")
            {
                msg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { "O_ORDER_MAIN", "DATA" });
                throw new MESReturnMessage(msg);
            }

            if (sku.C.SERIES_NAME != "Juniper-Configurable System MA-FVN")
            {
                var h = db.Queryable<I137_I, I137_H>((I, H) => I.TRANID == H.TRANID)
                        .Where((I, H) => I.ID == ordermain.ITEMID)
                        .Select((I, H) => new { H, I })
                        .First();
                //non-bndl 
                if (h.I.SOID == "000000")
                {
                    if (sku.S.SKU_TYPE == "CTO" && (sku.C.SERIES_NAME == "Juniper-FRU" || sku.C.SERIES_NAME == "Juniper-Configurable System"))
                    {
                        if (!db.Queryable<R_I244>().Where(t => t.PARENTSN == sn).Any())
                        {
                            throw new Exception($@"i244 of parent SN {sn} is not yet received");
                        }
                        else 
                        {
                            var i = db.Queryable<R_I244>().Where(t => (t.PARENTSN == sn || t.SN == sn)).First();
                            if (!h.H.SALESORDERNUMBER.Contains(i.SALESORDERNUMBER) && !h.I.SALESORDERLINEITEM.Contains(i.SALESORDERLINENUMBER))
                            {
                                throw new Exception($@"i244 of parent SN {sn} is not yet received");
                            }
                        }
                    }
                }
                //bndl
                else
                {
                    var bndlPid = db.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
                        .Where((O, I, H) => I.SOID == h.I.SOID && H.SALESORDERNUMBER == h.H.SALESORDERNUMBER)
                        .Select((O, I, H) => O.PID)
                        .ToList();
                    var bndlSku = db.Queryable<C_SKU, C_SERIES>((S, C) => S.C_SERIES_ID == C.ID)
                        .Where((S, C) => bndlPid.Contains(S.SKUNO))
                        .Select((S, C) => new { S, C }).ToList();
                    //同一個SO下混裝Optics與CTO才需要檢查I244,否則不檢查
                    if (bndlSku != null && (bndlSku.Any(t => t.S.SKU_TYPE == "OPTICS")) && (bndlSku.Any(t => t.S.SKU_TYPE == "CTO")))
                    {
                        if (!db.Queryable<R_I244>().Where(t => t.SN == sn).Any())
                        {
                            throw new Exception($@"i244 of parent SN {sn} is not yet received");
                        }
                        else
                        {
                            var i = db.Queryable<R_I244>().Where(t => (t.PARENTSN == sn || t.SN == sn)).First();
                            if (!h.H.SALESORDERNUMBER.Contains(i.SALESORDERNUMBER) && !h.I.SALESORDERLINEITEM.Contains(i.SALESORDERLINENUMBER))
                            {
                                throw new Exception($@"i244 of parent SN {sn} is not yet received");
                            }
                        }
                    }
                    else
                    {
                        foreach (var BndInfo in bndlPid)
                        {
                            var bndlSkuFor = db.Queryable<C_SKU, C_SERIES>((S, C) => S.C_SERIES_ID == C.ID)
                                            .Where((S, C) => S.SKUNO==BndInfo)
                                            .Select((S, C) => new { S, C }).ToList();
                            if (bndlSkuFor != null && (bndlSkuFor.Any(t => t.C.SERIES_NAME == "Juniper-Configurable System" && t.S.SKU_TYPE == "CTO") || bndlSkuFor.Any(t => t.C.SERIES_NAME == "Juniper-FRU" && t.S.SKU_TYPE == "CTO")))
                            {
                                if (!db.Queryable<R_I244>().Where(t => t.SN == sn).Any())
                                {
                                    throw new Exception($@"i244 of parent SN {sn} is not yet received");
                                }
                                else
                                {
                                    var i = db.Queryable<R_I244>().Where(t => (t.PARENTSN == sn || t.SN == sn)).First();
                                    if (!h.H.SALESORDERNUMBER.Contains(i.SALESORDERNUMBER) && !h.I.SALESORDERLINEITEM.Contains(i.SALESORDERLINENUMBER))
                                    {
                                        throw new Exception($@"i244 of parent SN {sn} is not yet received");
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
