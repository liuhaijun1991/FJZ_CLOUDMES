using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MESStation.Label.Public
{
    public class JuniperGroup : LabelValueGroup
    {
        public JuniperGroup()
        {
            ConfigGroup = "JuniperGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFRevision", Description = "Get TCIF Label Revision", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFPartNumber", Description = "Get TCIF Label Part Number Value", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFCLEI", Description = "Get TCIF Label CLEI Value", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFParentLabelSN", Description = "Get TCIF Parent Label SN", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFUPCLabelSN", Description = "Get TCIF Parent Label SN", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKCCNumber", Description = "Get KCC Label KCC Number Value", Paras = new List<string>() { "WO", "PLANT" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCurrentCartonSeqByWO", Description = "Get Current Carton Qty By WO", Paras = new List<string>() { "WO", "CARTONNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTotalCartonQtyByWO", Description = "Get Total Carton Qty By WO", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetOverpackNotPrint", Description = "Get Overpack Label Not Print Flag", Paras = new List<string>() { "WO", "CARTON" } });
            //public string GetOverpackNotPrintWithSNLen(OleExec SFCDB, string WO, string CARTON , List<string> SNs,string MinLen , string MaxLen)
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetOverpackNotPrintWithSNLen", Description = "Get Overpack Label Not Print Flag WithSNLen", Paras = new List<string>() { "WO", "CARTON", "SN", "MinLen", "MaxLen" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFParentNotPrint", Description = "Get TCIF Parent Label Not Print Flag", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFChildNotPrint", Description = "Get TCIF Child Label Not Print Flag", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetUPCNotPrint", Description = "Get UPC Label Not Print Flag", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDeliveryNumberByPO", Description = "Get Delivery Number From r_i282 By PO And Poline", Paras = new List<string>() { "PO", "POLINE" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDeliveryNumberByPONoZero", Description = "Get No Zero Delivery Number From r_i282 By PO And Poline", Paras = new List<string>() { "PO", "POLINE" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetShippingLabelPOSO", Description = "Get Shipping Label 2D POSO", Paras = new List<string>() { "SO", "DN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCOO", Description = "Get UPC TCIF Label COO", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get4LCOO", Description = "Get UPC TCIF Label 4LCOO", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetProductNumber", Description = "Get Product Number By WO", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFParentLabelQty", Description = "Get TCIF Parent Label Qty By SN", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFParentBundleLabelQty", Description = "Get TCIF Parent Bundle Label Qty By SN", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTotalCartonQtyByDN", Description = "Get Total Carton Qty By DN", Paras = new List<string>() { "DN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFClidLabelPN", Description = "Get TCIF Clid Label PN", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFClidLabelSN", Description = "Get TCIF Clid Label SN", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFClidLabelCLEI", Description = "Get TCIF Clid Label CLEI", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFClidLabelProdID", Description = "Get TCIF Clid Label Prod ID", Paras = new List<string>() { "WO", "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCartonListByDN", Description = "Get Carton List By DN", Paras = new List<string>() { "DN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetEACULabelCOO", Description = "Get EACU Label COO", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetEACULabelModel", Description = "Get EACU Label Model", Paras = new List<string>() { "SN", "PLANT" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetEACULabelType", Description = "Get EACU Label Type", Paras = new List<string>() { "SN", "PLANT" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPackingListNotPrint", Description = "Get Packing List NotPrint", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperPNByWO", Description = "Get Juniper PN By WO", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperPNBySku", Description = "Get Juniper PN By Sku", Paras = new List<string>() { "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCLEIBySku", Description = "Get CLEI By Sku", Paras = new List<string>() { "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetVerByWo", Description = "Get Ver ByWo", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetGroupIDByWo", Description = "Get GroupID ByWo", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWoQtyByWo", Description = "Get WoQty ByWo", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPoLineByWo", Description = "Get PoLine ByWo", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSoByWo", Description = "Get So ByWo", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPoLineQtyByPo", Description = "Get PoLineQt ByPo", Paras = new List<string>() { "PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPoLineDetailByPo", Description = "Get PoLineDetail ByPo", Paras = new List<string>() { "PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetComplete_DeliveryByPo", Description = "Get Complete_Delivery ByPo", Paras = new List<string>() { "PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetO_I137_ITEM_ByWO", Description = "O_I137_ITEM ByWO", Paras = new List<string>() { "WO", "CName" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetO_ORDER_MAIN_ByWO", Description = "O_ORDER_MAIN ByWO", Paras = new List<string>() { "WO", "CName" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTotalSoLine_ByWO", Description = "GetTotalSoLine_ByWO", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSoLineInfo_ByWO", Description = "GetSoLineInfo_ByWO", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetAgileRevBywo", Description = "GetAgileRevBywo", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSkuNameRevSNMac", Description = "GetSkuNameRevSNMac", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetInSilverWipTime", Description = "GetInSilverWipTime", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get711SkuAndVer", Description = "Get711SkuAndVer", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get750SkuAndVer", Description = "Get750SkuAndVer", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperPNorCustPIDByWO", Description = "GetJuniperPNorCustPIDByWO", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTCIFParentBundleNotPrint", Description = "", Paras = new List<string>() { "WO" } });

            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperMasterPalletDN", Description = "", Paras = new List<string>() { "PALLETNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperMasterPalletSO", Description = "", Paras = new List<string>() { "PALLETNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperMasterPalletPAGE", Description = "", Paras = new List<string>() { "PALLETNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperMasterPalletALLPAGE", Description = "", Paras = new List<string>() { "PALLETNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperMasterPalletSOLineList", Description = "", Paras = new List<string>() { "PALLETNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperMasterPalletSKUList", Description = "", Paras = new List<string>() { "PALLETNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperMasterPalletQTYList", Description = "", Paras = new List<string>() { "PALLETNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperIsLinecardBundle", Description = "", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperBundleSOLine", Description = "Get Bundle SO.SOID", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperBundleSN", Description = "Get Bundle SN List", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperBundlePN", Description = "Get PN For Bundle SN List", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperBundleCID", Description = "Get Custmor Product ID For Bundle SN List", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJuniperBundleCLEI", Description = "Get CLEI For Bundle SN List", Paras = new List<string>() { "SN" } });
        }

        public string Get711SkuAndVer(OleExec SFCDB, string SN)
        {
            var strSql = $@"SELECT R.SKUNO||'R'||C.SKU_VER AS SKUANDVER FROM R_SN R,R_WO_BASE C WHERE R.SKUNO=C.SKUNO AND R.WORKORDERNO = C.WORKORDERNO  AND R.SN ='{SN}' AND R.VALID_FLAG=2 AND SUBSTR( R.SKUNO,1,3)= '711' AND ROWNUM=1 ORDER BY R.EDIT_TIME DESC ";
            try
            {
                var res = SFCDB.RunSelect(strSql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    return res.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public string Get750SkuAndVer(OleExec SFCDB, string SN)
        {
            var strSql = $@"SELECT R.SKUNO||'R'||C.SKU_VER AS SKUANDVER FROM R_SN R,R_WO_BASE C WHERE R.SKUNO=C.SKUNO AND R.WORKORDERNO = C.WORKORDERNO  AND R.SN ='{SN}'  AND SUBSTR( R.SKUNO,1,3) in('740','750','760') AND R.VALID_FLAG=1  AND ROWNUM=1 ORDER BY R.EDIT_TIME DESC  ";
            try
            {
                var res = SFCDB.RunSelect(strSql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    return res.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public string GetInSilverWipTime(OleExec SFCDB, string SN)
        {
            var strSql = $@"select start_time from r_juniper_silver_wip where sn = '{SN}'";
            try
            {
                var res = SFCDB.RunSelect(strSql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    return ((DateTime)(res.Tables[0].Rows[0][0])).ToString("yyyy-MM-dd");
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public string GetO_ORDER_MAIN_ByWO(OleExec SFCDB, string WO, string CName)
        {
            var strSql = $@"select O_ORDER_MAIN.{CName} from O_ORDER_MAIN where prewo = '{WO}'";
            try
            {
                var res = SFCDB.RunSelect(strSql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    if (res.Tables[0].Rows[0][0].GetType() == typeof(DateTime))
                    {
                        return ((DateTime)res.Tables[0].Rows[0][0]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        return res.Tables[0].Rows[0][0].ToString();
                    }

                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }

        }

        public string GetSoLineInfo_ByWO(OleExec SFCDB, string WO)
        {
            string strSql = $@"select ii.salesorderlineitem soline, ii.Custreqshipdate SDATE
              from o_i137_head hh, O_I137_ITEM ii
             where hh.tranid = ii.tranid
               and hh.salesordernumber in
                   (select h.salesordernumber
                      from O_ORDER_MAIN m, O_I137_ITEM i, o_i137_head h
                     where m.itemid = i.id
                       and i.tranid = h.tranid
                       and m.prewo = '{WO}')
            and exists (select * from O_ORDER_MAIN oo where oo.itemid = ii.id)";

            try
            {
                var res = SFCDB.RunSelect(strSql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    string strRet = "";
                    for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                    {
                        var date = (DateTime)res.Tables[0].Rows[i]["SDATE"];

                        strRet += $@"{ res.Tables[0].Rows[i]["soline"]} ({date.Month}/{date.Day}),";
                    }
                    if (strRet.EndsWith(","))
                    {
                        strRet = strRet.Substring(0, strRet.Length - 1);
                    }
                    return strRet;
                }
                else
                {
                    return "No Data";
                }
            }
            catch (Exception ee)
            {
                return ee.Message;
            }
        }

        public string GetTotalSoLine_ByWO(OleExec SFCDB, string WO)
        {
            var strSql = $@"select ii.salesorderlineitem soline, ii.Custreqshipdate SDATE
                  from o_i137_head hh, O_I137_ITEM ii
                 where hh.tranid = ii.tranid
                   and hh.salesordernumber in
                       (select h.salesordernumber
                          from O_ORDER_MAIN m, O_I137_ITEM i, o_i137_head h
                         where m.itemid = i.id
                           and i.tranid = h.tranid
                           and m.prewo = '{WO}')
                   and exists (select * from O_ORDER_MAIN oo where oo.itemid = ii.id)";
            try
            {
                var res = SFCDB.RunSelect(strSql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    var str = res.Tables[0].Rows.Count.ToString();
                    return str;
                }
                else
                {
                    return "0";
                }
            }
            catch
            {
                return "0";
            }
        }


        /// <summary>
        /// GetO_I137_ITEM_ByWO
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="WO"></param>
        /// <param name="CName"></param>
        /// <returns></returns>
        public string GetO_I137_ITEM_ByWO(OleExec SFCDB, string WO, string CName)
        {
            var strSql = $@"select O_I137_ITEM.{CName} from O_ORDER_MAIN m,O_I137_ITEM where m.itemid = O_I137_ITEM.id and m.prewo = '{WO}'";
            try
            {
                var res = SFCDB.RunSelect(strSql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    if (res.Tables[0].Rows[0][0].GetType() == typeof(DateTime))
                    {
                        var str = ((DateTime)res.Tables[0].Rows[0][0]).ToString("yyyy-MM-dd");
                        return str;
                    }
                    else
                    {
                        var str = res.Tables[0].Rows[0][0].ToString();
                        return str;
                    }

                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// Get TCIF Label Revision Value
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="UPOID"></param>
        /// <param name="WO"></param>       
        /// <returns></returns>
        public string GetTCIFRevision(OleExec SFCDB, string WO, string SN)
        {
            string output = "";
            O_ORDER_MAIN orderMain = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.WO == o.PREWO).Where((r, o) => r.VALID == "1" && r.WO == WO)
                .Select((r, o) => o).ToList().FirstOrDefault();
            //var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
            //        .Where((C, S, W) => W.WORKORDERNO == WO)
            //        .Select((C, S, W) => C)
            //        .First();
            //bool isBundle = IsBundle(SFCDB, WO, orderMain.ITEMID);
            //if (isBundle && cs.SERIES_NAME == "Juniper-Optics")
            //{
            //    //BNDL另外處理
            //    output = "";
            //}
            //else 
            if (orderMain.POTYPE == "BTS")
            {
                List<R_SN_KP> list = SFCDB.ORM.Queryable<R_SN_KP>()
                        .Where(k => k.SN == SN && k.VALUE == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALID_FLAG == 1 && k.EXKEY1 == "REV").ToList();
                if (list.Count == 0)
                {
                    //throw new Exception("REV IS NULL!");
                    output = "";
                }
                else
                {
                    output = $@"{list.FirstOrDefault().EXVALUE1}";
                }
            }
            else if (orderMain.POTYPE == "CTO")
            {
                bool printBTS = false;
                var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                    .Where((c, s) => c.SKUNO == orderMain.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                    .ToList().FirstOrDefault();
                bool sku_control = cseries == null ? false : true;
                bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                      && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == WO).Any();
                printBTS = sku_control ? sku_control : wo_control;
                if (printBTS)
                {
                    List<R_SN_KP> list = SFCDB.ORM.Queryable<R_SN_KP>()
                            .Where(k => k.SN == SN && k.VALUE == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALID_FLAG == 1 && k.EXKEY1 == "REV").ToList();
                    if (list.Count == 0)
                    {
                        //throw new Exception("REV IS NULL!");
                        output = "";
                    }
                    else
                    {
                        output = $@"{list.FirstOrDefault().EXVALUE1}";
                    }
                }
                else
                {
                    List<MESDataObject.Module.Juniper.R_I244> list244 = SFCDB.ORM.Queryable<I137_I, O_I137_HEAD, MESDataObject.Module.Juniper.R_I244>
                            ((ii, ih, ri) => ii.TRANID == ih.TRANID && SqlSugar.SqlFunc.EndsWith(ii.SALESORDERLINEITEM, ri.SALESORDERLINENUMBER)
                            && SqlSugar.SqlFunc.EndsWith(ih.SALESORDERNUMBER, ri.SALESORDERNUMBER) && ri.PNTYPE == "Parent")
                            .Where((ii, ih, ri) => ii.ID == orderMain.ITEMID).Select((ii, ih, ri) => ri).ToList();
                    if (list244.Count == 0)
                    {
                        throw new Exception($@"CTO Get {WO} I244 Fail!");
                    }
                    output = list244.FirstOrDefault().REVISION;
                }
            }
            return output;
        }

        /// <summary>
        /// Get TCIF Label Part Number Value
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="UPOID"></param>
        /// <param name="WO"></param>       
        /// <returns></returns>
        public string GetTCIFPartNumber(OleExec SFCDB, string WO, string SN)
        {
            string output = "";

            O_ORDER_MAIN orderMain = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.WO == o.PREWO).Where((r, o) => r.VALID == "1" && r.WO == WO)
                .Select((r, o) => o).ToList().FirstOrDefault();
            //var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
            //        .Where((C, S, W) => W.WORKORDERNO == WO)
            //        .Select((C, S, W) => C)
            //        .First();
            //bool isBundle = IsBundle(SFCDB, WO, orderMain.ITEMID);
            //if (isBundle && cs.SERIES_NAME == "Juniper-Optics")
            //{
            //    //BNDL另外處理
            //    output = "";
            //}
            //else 
            if (orderMain.POTYPE == "BTS")
            {
                List<R_SN_KP> list = SFCDB.ORM.Queryable<R_SN_KP>()
                                      .Where(k => k.SN == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALUE == SN && k.VALID_FLAG == 1).ToList();
                if (list.Count == 0)
                {
                    throw new Exception("7XX Part Number IS NULL!");
                }
                output = $@"{list.FirstOrDefault().MPN}";
            }
            else if (orderMain.POTYPE == "CTO")
            {
                bool printBTS = false;
                var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                    .Where((c, s) => c.SKUNO == orderMain.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                    .ToList().FirstOrDefault();
                bool sku_control = cseries == null ? false : true;
                bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                      && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == WO).Any();
                printBTS = sku_control ? sku_control : wo_control;
                if (printBTS)
                {
                    List<R_SN_KP> list = SFCDB.ORM.Queryable<R_SN_KP>()
                                           .Where(k => k.SN == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALUE == SN && k.VALID_FLAG == 1).ToList();
                    if (list.Count == 0)
                    {
                        throw new Exception("7XX Part Number IS NULL!");
                    }
                    output = $@"{list.FirstOrDefault().MPN}";
                }
                else
                {
                    List<MESDataObject.Module.Juniper.R_I244> list244 = SFCDB.ORM.Queryable<I137_I, O_I137_HEAD, MESDataObject.Module.Juniper.R_I244>
                                        ((ii, ih, ri) => ii.TRANID == ih.TRANID && SqlSugar.SqlFunc.EndsWith(ii.SALESORDERLINEITEM, ri.SALESORDERLINENUMBER)
                                        && SqlSugar.SqlFunc.EndsWith(ih.SALESORDERNUMBER, ri.SALESORDERNUMBER) && ri.PNTYPE == "Parent")
                                        .Where((ii, ih, ri) => ii.ID == orderMain.ITEMID).Select((ii, ih, ri) => ri).ToList();
                    if (list244.Count == 0)
                    {
                        throw new Exception($@"CTO Get {WO} I244 Fail!");
                    }
                    output = list244.FirstOrDefault().SUBASSEMBLYNUMBER;
                }
            }
            return output;
        }

        /// <summary>
        /// Get TCIF Label CLEI Value
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="UPOID"></param>
        /// <param name="WO"></param>       
        /// <returns></returns>
        public string GetTCIFCLEI(OleExec SFCDB, string WO, string SN)
        {
            string output = "";

            O_ORDER_MAIN orderMain = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.WO == o.PREWO).Where((r, o) => r.VALID == "1" && r.WO == WO)
                            .Select((r, o) => o).ToList().FirstOrDefault();
            //var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
            //        .Where((C, S, W) => W.WORKORDERNO == WO)
            //        .Select((C, S, W) => C)
            //        .First();
            //bool isBundle = IsBundle(SFCDB, WO, orderMain.ITEMID);
            //if (isBundle && cs.SERIES_NAME == "Juniper-Optics")
            //{
            //    //BNDL另外處理
            //    output = "";
            //}
            //else 
            if (orderMain.POTYPE == "BTS")
            {
                List<R_SN_KP> list = SFCDB.ORM.Queryable<R_SN_KP>()
                      .Where(k => k.SN == SN && k.VALUE == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALID_FLAG == 1 && k.EXKEY2 == "CLEI" && k.SCANTYPE == "SN").ToList();
                if (list.Count == 0)
                {
                    //throw new Exception("7XX Part Number IS NULL!");
                    output = "";
                }
                else
                {
                    output = $@"{list.FirstOrDefault().EXVALUE2}";
                }
            }
            else if (orderMain.POTYPE == "CTO")
            {
                bool printBTS = false;
                var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                    .Where((c, s) => c.SKUNO == orderMain.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                    .ToList().FirstOrDefault();
                bool sku_control = cseries == null ? false : true;
                bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                      && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == WO).Any();
                printBTS = sku_control ? sku_control : wo_control;
                if (printBTS)
                {
                    List<R_SN_KP> list = SFCDB.ORM.Queryable<R_SN_KP>()
                          .Where(k => k.SN == SN && k.VALUE == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALID_FLAG == 1 && k.EXKEY2 == "CLEI" && k.SCANTYPE == "SN").ToList();
                    if (list.Count == 0)
                    {
                        //throw new Exception("7XX Part Number IS NULL!");
                        output = "";
                    }
                    else
                    {
                        output = $@"{list.FirstOrDefault().EXVALUE2}";
                    }
                }
                else
                {
                    List<MESDataObject.Module.Juniper.R_I244> list244 = SFCDB.ORM.Queryable<I137_I, O_I137_HEAD, MESDataObject.Module.Juniper.R_I244>
                           ((ii, ih, ri) => ii.TRANID == ih.TRANID && SqlSugar.SqlFunc.EndsWith(ii.SALESORDERLINEITEM, ri.SALESORDERLINENUMBER)
                           && SqlSugar.SqlFunc.EndsWith(ih.SALESORDERNUMBER, ri.SALESORDERNUMBER) && ri.PNTYPE == "Parent")
                           .Where((ii, ih, ri) => ii.ID == orderMain.ITEMID).Select((ii, ih, ri) => ri).ToList();
                    if (list244.Count == 0)
                    {
                        throw new Exception($@"CTO Get {WO} I244 Fail!");
                    }
                    output = list244.FirstOrDefault().CLEICODE;
                }
            }
            return output;
        }
        public string GetTCIFParentLabelQty(OleExec SFCDB, string SN)
        {
            string output = "";
            //目前暫定固定值為1
            output = "1";
            return output;
        }

        public string GetTCIFParentBundleLabelQty(OleExec SFCDB, string SN)
        {
            string output = "1";
            var WO = SFCDB.ORM.Queryable<R_SN>()
                .Where(t => t.SN == SN && t.VALID_FLAG == "1")
                .Select(t => t.WORKORDERNO)
                .First();
            if (WO == null)
            {
                throw new Exception($@"Cann't get SN data!SN:{SN}");
            }
            var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                        .Where((C, S, W) => W.WORKORDERNO == WO)
                        .Select((C, S, W) => new { S, C })
                        .First();
            var o_main = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1")
                .Select((r, o) => o)
                .First();
            if (o_main == null)
            {
                throw new Exception($@"{WO} Error!");
            }
            var bndl = IsBundle(SFCDB, WO, o_main.ITEMID);

            //Optics Bundle : Qty=SoQty, Non-Optics Bundle : Qty=1
            if (cs.C.SERIES_NAME == "Juniper-Optics" && bndl)
            {
                //var i_item = SFCDB.ORM.Queryable<O_I137_ITEM, O_I137_HEAD>((I, H) => I.TRANID == H.TRANID)
                //    .Where(r => r.ID == o_main.ITEMID)
                //    .First();
                var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(t => t.ID == o_main.ITEMID).First();
                if (i_item == null)
                {
                    throw new Exception($@"Cann't get I137 Data {WO}!");
                }
                output = i_item.SOQTY.IndexOf('.') > 0 ? i_item.SOQTY.Substring(0, i_item.SOQTY.IndexOf('.')) : i_item.SOQTY;
            }
            return output;
        }

        public string GetTCIFParentLabelSN(OleExec SFCDB, string WO, string SN)
        {
            string output = "";
            O_ORDER_MAIN orderMain = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.WO == o.PREWO).Where((r, o) => r.VALID == "1" && r.WO == WO)
                .Select((r, o) => o).ToList().FirstOrDefault();
            var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                    .Where((C, S, W) => W.WORKORDERNO == WO)
                    .Select((C, S, W) => new { S, C })
                    .First();
            //临时解决方案，无SN的FRU不打SN
            if (cs.C.SERIES_NAME == "Juniper-FRU" && cs.S.SN_RULE == "JUNIPER_11_DIGIT_LABEL")
            {
                output = "";
            }
            else
            {
                output = SN;
            }
            return output;
        }
        public string GetTCIFUPCLabelSN(OleExec SFCDB, string WO, string SN)
        {
            string output = "";
            O_ORDER_MAIN orderMain = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.WO == o.PREWO).Where((r, o) => r.VALID == "1" && r.WO == WO)
                .Select((r, o) => o).ToList().FirstOrDefault();
            if (orderMain.POTYPE == "BTS")
            {
                var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                        .Where((C, S, W) => W.WORKORDERNO == WO)
                        .Select((C, S, W) => new { S, C })
                        .First();
                if (cs.C.SERIES_NAME == "Juniper-FRU" && cs.S.SKU_TYPE == "DOF")
                {
                    output = "";
                }
                else
                {
                    output = SN;
                }
            }
            return output;
        }

        /// <summary>
        /// Get KCC Number From O_AGILE_ATTR,If No Value Return Error Msg
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="UPOID"></param>
        /// <param name="WO"></param>
        /// <returns></returns>
        public string GetKCCNumber(OleExec SFCDB, string WO, string PLANT)
        {
            string output = "";
            //O_AGILE_ATTR o = SFCDB.ORM.Queryable<O_AGILE_ATTR, I137_I>((a, i) => a.ITEM_NUMBER == i.ITEM && a.REV == i.VERSION)
            //            .Where((a, i) => i.TRANID == TranId).Select((a, i) => a).ToList().FirstOrDefault();
            //string item = SFCDB.ORM.Queryable<R_SN, R_SN_KP>((rs, rk) => rs.ID == rk.R_SN_ID).Where((rs, rk) => rs.WORKORDERNO == WO && rk.SN == rk.VALUE && rk.SCANTYPE=="SN" && rk.KP_NAME== "AutoKP")
            //    .Select((rs, rk) => rk).ToList().FirstOrDefault().MPN.ToString();

            string item = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == WO).Select(r => r.SKUNO).ToList().FirstOrDefault();

            var o = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(r => r.ITEM_NUMBER == item && r.PLANT == PLANT)
                .OrderBy(r => r.DATE_CREATED, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (o == null)
            {
                throw new Exception("Get KCC Error!");
            }
            if (string.IsNullOrEmpty(o.KCC_CERT_NUMBER))
            {
                //throw new Exception("Get KCC Error!");
            }
            output = o.KCC_CERT_NUMBER;
            return output;
        }
        /// <summary>
        /// Get Current Carton Qty By WO
        /// Use for Overpack Label1 
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="WO"></param>
        /// <returns></returns>
        public string GetCurrentCartonSeqByWO(OleExec SFCDB, string WO, string CARTONNO)
        {
            string output = "";
            List<string> listCarton = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((s, r, p) => s.ID == r.SN_ID && r.PACK_ID == p.ID)
                        .Where((s, r, p) => s.WORKORDERNO == WO && s.VALID_FLAG == "1")
                        .OrderBy((s, r, p) => p.CREATE_TIME, SqlSugar.OrderByType.Asc).Select((s, r, p) => p.PACK_NO).ToList();
            listCarton = listCarton.Distinct().ToList();
            if (listCarton.Count == 0)
            {
                throw new Exception($@"{WO} Not Packing!");
            }
            var curren_carton = listCarton.Find(l => l == CARTONNO);

            int i = 0;
            bool bexists = false;
            foreach (var c in listCarton)
            {
                i++;
                if (c == CARTONNO)
                {
                    bexists = true;
                    break;
                }
            }
            if (!bexists)
            {
                throw new Exception($@"{CARTONNO},{WO} Inconsistent");
            }
            output = i.ToString();
            return output;
        }
        /// <summary>
        /// Get Total Carton Qty By WO
        /// Use for Overpack Label1 
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="WO"></param>
        /// <returns></returns>
        public string GetTotalCartonQtyByWO(OleExec SFCDB, string WO)
        {
            string output = "";
            List<R_PACKING> listCarton = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((s, r, p) => s.ID == r.SN_ID && r.PACK_ID == p.ID)
                        .Where((s, r, p) => s.WORKORDERNO == WO && s.VALID_FLAG == "1").Select((s, r, p) => p)
                        .ToList();
            listCarton = listCarton.OrderBy(p => p.CREATE_TIME).ToList();
            R_WO_BASE woObj = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == WO).ToList().FirstOrDefault();
            if (listCarton.Count == 0)
            {
                throw new Exception($@"{WO} Not Packing!");
            }
            if (woObj == null)
            {
                throw new Exception($@"{WO} Error!");
            }
            if (woObj.WORKORDER_QTY == null || woObj.WORKORDER_QTY == 0)
            {
                throw new Exception($@"{WO} WORKORDER_QTY Error!");
            }
            if (listCarton.FirstOrDefault().MAX_QTY == 0)
            {
                throw new Exception($@"{listCarton.FirstOrDefault().PACK_NO} MAX_QTY Error!");
            }
            int wo_qty = Convert.ToInt32(woObj.WORKORDER_QTY);
            int carton_qty = Convert.ToInt32(listCarton.FirstOrDefault().MAX_QTY);
            if (wo_qty % carton_qty == 0)
            {
                output = (wo_qty / carton_qty).ToString();
            }
            else
            {
                output = (wo_qty / carton_qty + 1).ToString();
            }
            return output;
        }

        /// <summary>
        /// Shiping Label DN/Delivery Number
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="PO"></param>
        /// <param name="POLINE"></param>
        /// <returns></returns>
        public string GetDeliveryNumberByPONoZero(OleExec SFCDB, string PO, string POLINE)
        {
            string out_put = "";
            var o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO>((o, r) => o.ID == r.ORIGINALID)
                .Where((o, r) => o.PONO == PO && o.POLINE == POLINE).Select((o, r) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{PO},{POLINE} Not Exists O_ORDER_MAIN");
            }
            if (string.IsNullOrEmpty(o_main.PREASN))
            {
                throw new Exception($@"{PO},{POLINE} Not Creat PREASN Or Cancel PREASN!");
            }
            if (o_main.PREASN == "0")
            {
                throw new Exception($@"{PO},{POLINE} Not Creat PREASN Or Cancel PREASN!");
            }
            if (!o_main.PREASN.StartsWith("PRESHIP"))
            {
                throw new Exception($@"{PO},{POLINE} PREASN ERROR!");
            }
            if (o_main.ORDERTYPE == ENUM_I137_PoDocType.IDOA.ExtValue())
            {
                var list = SFCDB.ORM.Queryable<R_JNP_DOA_SHIPMENTS_ACK>()
                    .Where(r => r.ASNNUMBER == o_main.PREASN)
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                    .ToList();
                if (list.Count == 0)
                {
                    throw new Exception($@"[{o_main.PREASN}] No Shipment File Reply Data!");
                }
                if (list.FirstOrDefault().DELIVERYNUMBER == null)
                {
                    throw new Exception($@"[{o_main.PREASN}] DN Number IS NULL");
                }
                out_put = list.FirstOrDefault().DELIVERYNUMBER.TrimStart('0');
                return out_put;
            }
            else
            {
                var list_i139 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I139>()
                    .Where(r => r.ASNNUMBER == o_main.PREASN).OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc).ToList();
                if (list_i139.FirstOrDefault().DELIVERYCODE == "03")
                {
                    throw new Exception($@"{PO},{POLINE},{o_main.PREASN} PREASN CANCEL!");
                }
                else if (list_i139.FirstOrDefault().DELIVERYCODE != "01")
                {
                    throw new Exception($@"{PO},{POLINE},{o_main.PREASN} DELIVERYCODE ERROR!R_I139");
                }
                List<MESDataObject.Module.Juniper.R_I282> list = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I282>()
                    .Where(r => r.ASNNUMBER == o_main.PREASN && SqlSugar.SqlFunc.IsNullOrEmpty(r.ERRORCODE) == true)
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                    .ToList();
                if (list.Count == 0)
                {
                    throw new Exception($@"[{o_main.PREASN}] No I282 Data!");
                }
                if (list.FirstOrDefault().DELIVERYNUMBER == null)
                {
                    throw new Exception($@"[{o_main.PREASN}] DN Number IS NULL");
                }
                out_put = list.FirstOrDefault().DELIVERYNUMBER.TrimStart('0');
                return out_put;
            }
        }
        public string GetDeliveryNumberByPO(OleExec SFCDB, string PO, string POLINE)
        {
            string out_put = "";
            var o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO>((o, r) => o.ID == r.ORIGINALID)
                .Where((o, r) => o.PONO == PO && o.POLINE == POLINE).Select((o, r) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{PO},{POLINE} Not Exists O_ORDER_MAIN");
            }
            if (string.IsNullOrEmpty(o_main.PREASN))
            {
                throw new Exception($@"{PO},{POLINE} Not Creat PREASN Or Cancel PREASN!");
            }
            if (o_main.PREASN == "0")
            {
                throw new Exception($@"{PO},{POLINE} Not Creat PREASN Or Cancel PREASN!");
            }
            if (!o_main.PREASN.StartsWith("PRESHIP"))
            {
                throw new Exception($@"{PO},{POLINE} PREASN ERROR!");
            }
            if (o_main.ORDERTYPE == ENUM_I137_PoDocType.IDOA.ExtValue())
            {

                var list = SFCDB.ORM.Queryable<R_JNP_DOA_SHIPMENTS_ACK>()
                    .Where(r => r.ASNNUMBER == o_main.PREASN)
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                    .ToList();
                if (list.Count == 0)
                {
                    throw new Exception($@"[{o_main.PREASN}] No Shipment File Reply Data!");
                }
                if (list.FirstOrDefault().DELIVERYNUMBER == null)
                {
                    throw new Exception($@"[{o_main.PREASN}] DN Number IS NULL");
                }
                out_put = list.FirstOrDefault().DELIVERYNUMBER;
                return out_put;
            }
            else
            {
                var list_i139 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I139>()
                    .Where(r => r.ASNNUMBER == o_main.PREASN)
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                    .ToList();
                if (list_i139.FirstOrDefault().DELIVERYCODE == "03")
                {
                    throw new Exception($@"{PO},{POLINE},{o_main.PREASN} PREASN CANCEL!");
                }
                else if (list_i139.FirstOrDefault().DELIVERYCODE != "01")
                {
                    throw new Exception($@"{PO},{POLINE},{o_main.PREASN} DELIVERYCODE ERROR!R_I139");
                }

                List<MESDataObject.Module.Juniper.R_I282> list = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I282>()
                    .Where(r => r.ASNNUMBER == o_main.PREASN && SqlSugar.SqlFunc.IsNullOrEmpty(r.ERRORCODE) == true)
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                    .ToList();
                if (list.Count == 0)
                {
                    throw new Exception($@"[{o_main.PREASN}] No I282 Data!");
                }
                if (list.FirstOrDefault().DELIVERYNUMBER == null)
                {
                    throw new Exception($@"[{o_main.PREASN}] DN Number IS NULL");
                }
                out_put = list.FirstOrDefault().DELIVERYNUMBER;
                return out_put;
            }
        }
        /// <summary>
        /// Shiping Label 2D POSO 
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="PO"></param>
        /// <param name="POLINE"></param>
        /// <returns></returns>
        public string GetShippingLabelPOSO(OleExec SFCDB, string SO, string DN)
        {
            return $@"SO:{SO},DN:{DN}";
        }
        public string GetASNByPO(OleExec SFCDB, string PO, string POLINE)
        {
            string out_put = "";
            //List<MESDataObject.Module.Juniper.R_I139> list = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I139>()
            //    .Where(i139 => i139.PONUMBER == PO && i139.ITEM == POLINE &&i139.DELIVERYCODE=="01" ).ToList();
            //if (list.Count == 0)
            //{
            //    throw new Exception("I139 Error!");
            //}
            //out_put = list.FirstOrDefault().ASNNUMBER;           
            var o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO>((o, r) => o.ID == r.ORIGINALID)
                .Where((o, r) => o.PONO == PO && o.POLINE == POLINE).Select((o, r) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{PO},{POLINE} Not Exists O_ORDER_MAIN");
            }
            if (string.IsNullOrEmpty(o_main.PREASN))
            {
                throw new Exception($@"{PO},{POLINE} Not Creat PREASN Or Cancel PREASN!");
            }
            if (o_main.PREASN == "0")
            {
                throw new Exception($@"{PO},{POLINE} Not Creat PREASN Or Cancel PREASN!");
            }
            if (!o_main.PREASN.StartsWith("PRESHIP"))
            {
                throw new Exception($@"{PO},{POLINE} PREASN ERROR!");
            }
            out_put = o_main.PREASN;
            return out_put;
        }
        public string GetShippingTotalWeight(OleExec SFCDB, string asn)
        {
            string out_put = "";
            List<MESDataObject.Module.Juniper.R_I139> list = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I139>()
                .Where(r => r.ASNNUMBER == asn && r.DELIVERYCODE == "01").ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"{asn} Not In R_I139");
            }
            if (string.IsNullOrEmpty(list.FirstOrDefault().GROSSWEIGHT))
            {
                throw new Exception($@"{asn} GROSSWEIGHT Is Null Or Empty!");
            }
            //1kg=2.046lb
            double kg = Convert.ToDouble(list.FirstOrDefault().GROSSWEIGHT);
            decimal lb = Math.Round((decimal)(kg * 2.046), 2, MidpointRounding.AwayFromZero);
            out_put = $@"{kg}/{lb}";
            return out_put;
        }
        public string GetShippingLabelNetWeight(OleExec SFCDB, string asn)
        {
            string out_put = "";
            List<MESDataObject.Module.Juniper.R_I139> list = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I139>()
                .Where(r => r.ASNNUMBER == asn && r.DELIVERYCODE == "01").ToList();
            if (list.Count == 0)
            {
                throw new Exception($@"{asn} Not In R_I139");
            }
            if (string.IsNullOrEmpty(list.FirstOrDefault().NETWEIGHT))
            {
                throw new Exception($@"{asn} GROSSWEIGHT Is Null Or Empty!");
            }
            //1kg=2.046lb
            double kg = Convert.ToDouble(list.FirstOrDefault().NETWEIGHT);
            decimal lb = Math.Round((decimal)(kg * 2.046), 2, MidpointRounding.AwayFromZero);
            out_put = $@"{kg}/{lb}";
            return out_put;
        }

        public string GetTotalPiecesNumber(OleExec SFCDB, string asn)
        {
            string out_put = "";
            List<MESDataObject.Module.Juniper.R_I139> list = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I139>()
               .Where(i139 => i139.ASNNUMBER == asn && i139.DELIVERYCODE == "01").ToList();
            if (list.Count == 0)
            {
                throw new Exception("I139 Error!");
            }
            out_put = list.Count().ToString();
            return out_put;
        }

        public string GetTotalCartonsNumber(OleExec SFCDB, string asn)
        {
            string out_put = "";
            var o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREASN == asn).First();
            if (o_main == null)
            {
                throw new Exception("Get order main data fail!");
            }
            if (o_main.ORDERTYPE == ENUM_I137_PoDocType.IDOA.ExtValue())
            {
                var list = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, O_ORDER_MAIN, R_JNP_DOA_SHIPMENTS_ACK>
                       ((rs, rsp, rp, o, sa) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && o.PREWO == rs.WORKORDERNO && o.PREASN == sa.ASNNUMBER)
                       .Where((rs, rsp, rp, o, sa) => o.PREASN == asn && rs.VALID_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(sa.MESSAGE_CODE))
                       .Select((rs, rsp, rp, o, sa) => rp);
                out_put = list.Select(r => r.ID).Distinct().Count().ToString();
                return out_put;
            }
            else
            {
                //var list = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING, MESDataObject.Module.Juniper.R_I282>
                //    ((rsd, rs, rsp, rp, ri) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && rsd.DN_NO == ri.DELIVERYNUMBER)
                //    .Where((rsd, rs, rsp, rp, ri) => ri.ASNNUMBER == asn && rs.VALID_FLAG == "1").Select((rsd, rs, rsp, rp, ri) => rsp);
                var list = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I139, R_SN, R_SN_PACKING, R_PACKING, MESDataObject.Module.Juniper.R_I282>
                    ((i139, rs, rsp, rp, ri) => i139.SERIALID == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && i139.ASNNUMBER == ri.ASNNUMBER)
                    .Where((i139, rs, rsp, rp, ri) => ri.ASNNUMBER == asn && rs.VALID_FLAG == "1" && i139.DELIVERYCODE == "01" && SqlSugar.SqlFunc.IsNullOrEmpty(ri.ERRORCODE))
                    .Select((i139, rs, rsp, rp, ri) => rp);
                out_put = list.Select(r => r.ID).Distinct().Count().ToString();
                return out_put;
            }
        }
        public string GetPackingItemList(OleExec SFCDB, string asn)
        {
            string out_put = "";
            var o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREASN == asn).First();
            if (o_main == null)
            {
                throw new Exception("Get order main data fail!");
            }
            if (o_main.ORDERTYPE == ENUM_I137_PoDocType.IDOA.ExtValue())
            {
                var list = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, O_ORDER_MAIN, R_JNP_DOA_SHIPMENTS_ACK>
                       ((rs, rsp, rp, o, sa) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && o.PREWO == rs.WORKORDERNO && o.PREASN == sa.ASNNUMBER)
                       .Where((rs, rsp, rp, o, sa) => o.PREASN == asn && rs.VALID_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(sa.MESSAGE_CODE))
                       .Select((rs, rsp, rp, o, sa) => rsp);
                out_put = list.Count().ToString();
                return out_put;
            }
            else
            {
                //var list = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING, MESDataObject.Module.Juniper.R_I282>
                //    ((rsd, rs, rsp, rp, ri) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && rsd.DN_NO == ri.DELIVERYNUMBER)
                //    .Where((rsd, rs, rsp, rp, ri) => ri.ASNNUMBER == asn && rs.VALID_FLAG == "1").Select((rsd, rs, rsp, rp, ri) => rsp);
                var list = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I139, R_SN, R_SN_PACKING, R_PACKING, MESDataObject.Module.Juniper.R_I282>
                    ((i139, rs, rsp, rp, ri) => i139.SERIALID == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && i139.ASNNUMBER == ri.ASNNUMBER)
                    .Where((i139, rs, rsp, rp, ri) => ri.ASNNUMBER == asn && rs.VALID_FLAG == "1" && i139.DELIVERYCODE == "01" && ri.ERRORCODE == "")
                    .Select((i139, rs, rsp, rp, ri) => rsp);
                out_put = list.Count().ToString();
                return out_put;
            }
        }

        public string GetCOO(OleExec SFCDB, string SN)
        {
            string out_put = "";
            List<MESDataObject.Module.Juniper.R_COO_MAP> list = SFCDB.ORM.Queryable<R_SN_KP, MESDataObject.Module.Juniper.R_COO_MAP>((k, m) => k.LOCATION == m.CODE)
                .Where((k, m) => k.SN == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALUE == SN && k.VALID_FLAG == 1).Select((k, m) => m).ToList();
            if (list.Count == 0)
            {
                throw new Exception("COO IS NULL!");
            }
            out_put = $@"{list.FirstOrDefault().CODE}-{list.FirstOrDefault().COUNTRY}";
            return out_put;
        }
        public string Get4LCOO(OleExec SFCDB, string SN)
        {
            string out_put = "";
            List<R_SN_KP> list = SFCDB.ORM.Queryable<R_SN_KP>()
                .Where(k => k.SN == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALUE == SN && k.VALID_FLAG == 1).ToList();
            if (list.Count == 0)
            {
                throw new Exception("COO IS NULL!");
            }
            out_put = $@"{list.FirstOrDefault().LOCATION}";
            return out_put;
        }
        /// <summary>
        /// TCIF Parent Label
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="WO"></param>
        /// <returns></returns>
        public string GetProductNumber(OleExec SFCDB, string WO)
        {
            string out_put = "";
            var oMain = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO>((o, r) => o.PREWO == r.WO)
                .Where((o, r) => r.WO == WO && r.VALID == "1").Select((o, r) => o).ToList().FirstOrDefault();
            if (oMain == null)
            {
                throw new Exception($@"{WO} Error!O_ORDER_MAIN");
            }
            #region 2021.09.27 屏蔽 by FGG 作爲通用的TCIF PARENT LABEL 中的ProductNumber取值,先不區分Bundle和非Bundle
            //bool isBundle = IsBundle(SFCDB, WO, oMain.ITEMID);
            //if (isBundle)
            //{
            //    //BNDL 取I137 MaterialID in I137 Item
            //    I137_I i137 = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == oMain.ITEMID).ToList().FirstOrDefault();
            //    if (i137 == null)
            //    {
            //        throw new Exception($@"{WO} No i137 Data");
            //    }
            //    if (string.IsNullOrEmpty(i137.MATERIALID))
            //    {
            //        if (!(i137.CARTONLABEL2.ToUpper() == "BULK"))
            //        {
            //            throw new Exception($@"{WO} is BNDL but No MATERIALID");
            //        }
            //        O_AGILE_ATTR aglie1 = SFCDB.ORM.Queryable<R_WO_BASE, O_AGILE_ATTR>((r, o) => r.SKUNO == o.ITEM_NUMBER)
            //       .Where((r, o) => r.WORKORDERNO == WO && o.PLANT == oMain.PLANT).OrderBy((r, o) => o.DATE_CREATED, SqlSugar.OrderByType.Desc)
            //       .Select((r, o) => o).ToList().FirstOrDefault();
            //        if (aglie1 == null)
            //        {
            //            throw new Exception($@"SKU OF {WO} NO MAPPING IN O_AGILE_ATTR!");
            //        }
            //        if (string.IsNullOrEmpty(aglie1.CUSTPARTNO))
            //        {
            //            throw new Exception($@"JUNIPER PN MAPPING IN O_AGILE_ATTR IS NULL!");
            //        }
            //        out_put = aglie1.CUSTPARTNO;

            //    }
            //    else
            //    {
            //        out_put = i137.MATERIALID;
            //    }


            //}
            //else
            //{
            //    O_AGILE_ATTR aglie = SFCDB.ORM.Queryable<R_WO_BASE, O_AGILE_ATTR>((r, o) => r.SKUNO == o.ITEM_NUMBER)
            //        .Where((r, o) => r.WORKORDERNO == WO && o.PLANT == oMain.PLANT).OrderBy((r, o) => o.DATE_CREATED, SqlSugar.OrderByType.Desc)
            //        .Select((r, o) => o).ToList().FirstOrDefault();
            //    if (aglie == null)
            //    {
            //        throw new Exception($@"SKU OF {WO} NO MAPPING IN O_AGILE_ATTR!");
            //    }
            //    if (string.IsNullOrEmpty(aglie.CUSTPARTNO))
            //    {
            //        throw new Exception($@"JUNIPER PN MAPPING IN O_AGILE_ATTR IS NULL!");
            //    }
            //    out_put = aglie.CUSTPARTNO;
            //}
            #endregion

            O_AGILE_ATTR aglie = SFCDB.ORM.Queryable<R_WO_BASE, O_AGILE_ATTR>((r, o) => r.SKUNO == o.ITEM_NUMBER)
                    .Where((r, o) => r.WORKORDERNO == WO && o.PLANT == oMain.PLANT).OrderBy((r, o) => o.DATE_CREATED, SqlSugar.OrderByType.Desc)
                    .Select((r, o) => o).ToList().FirstOrDefault();
            if (aglie == null)
            {
                throw new Exception($@"SKU OF {WO} NO MAPPING IN O_AGILE_ATTR!");
            }
            if (string.IsNullOrEmpty(aglie.CUSTPARTNO))
            {
                throw new Exception($@"JUNIPER PN MAPPING IN O_AGILE_ATTR IS NULL!");
            }
            out_put = aglie.CUSTPARTNO;
            if (string.IsNullOrEmpty(out_put))
            {
                throw new Exception($@"Get {WO} ProductNumber Fail!");
            }
            return out_put;
        }

        public string GetTotalCartonQtyByDN(OleExec SFCDB, string DN)
        {
            string output = "";
            var o_main = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_JNP_DOA_SHIPMENTS_ACK>((O, A) => O.PREASN == A.ASNNUMBER)
                .Where((O, A) => A.DELIVERYNUMBER == DN)
                .Select((O, A) => O)
                .First();
            if (o_main != null)
            {
                output = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, O_ORDER_MAIN, R_JNP_DOA_SHIPMENTS_ACK>
                       ((rs, rsp, rp, o, sa) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && o.PREWO == rs.WORKORDERNO && o.PREASN == sa.ASNNUMBER)
                       .Where((rs, rsp, rp, o, sa) => sa.DELIVERYNUMBER == DN && rs.VALID_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(sa.MESSAGE_CODE))
                       .Select((rs, rsp, rp, o, sa) => rp.ID)
                       .Distinct()
                       .Count()
                       .ToString();
                //output = list.Select(rs => rs.ID).Distinct().Count().ToString();
                return output;
            }
            else
            {
                var listValue = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I282, MESDataObject.Module.Juniper.R_I139, R_SN, R_SN_PACKING, R_PACKING>
                    ((i282, i39, r, rsp, rp) => i282.ASNNUMBER == i39.ASNNUMBER && i39.SERIALID == r.SN && r.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                    .Where((i282, i39, r, rsp, rp) => i282.DELIVERYNUMBER == DN && i39.DELIVERYCODE == "01" && r.VALID_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(i282.ERRORCODE))
                    .Select((i282, i39, r, rsp, rp) => rp).ToList();

                output = listValue.Select(r => r.PACK_NO).Distinct().ToList().Count().ToString();
                return output;
            }
        }
        public List<string> GetTCIFClidLabelPN(OleExec SFCDB, string WO, string SN)
        {
            List<string> output = new List<string>();
            List<O_ORDER_MAIN> listValue = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception($@"{WO} Error!");
            }
            O_ORDER_MAIN orderMain = listValue.FirstOrDefault();
            string po = orderMain.PONO;
            string poline = orderMain.POLINE;
            string tranid = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == orderMain.ITEMID).ToList().FirstOrDefault().TRANID;
            bool isBundle = IsBundle(SFCDB, WO, orderMain.ITEMID);
            if (isBundle)
            {
                //output = SFCDB.ORM.Queryable<R_SN_KP>()
                //    .Where(r => r.SN == SN && r.SCANTYPE == "SN" && r.KP_NAME == "AutoKP" && r.VALUE == SN && r.VALID_FLAG == 1)
                //    .Select(r => r.PARTNO)
                //    .ToList();
                //直接取I054，否則還要轉成客戶料號

                //传了多次I054的情况
                string trainid = SFCDB.RunSelect($@"select tranid  from(
select r.*, rank() over(partition by parentsn order by createtime) row_number from r_i054 r where r.parentsn = '{SN}' )
where row_number = 1").Tables[0].Rows[0]?[0].ToString();


                output = SFCDB.ORM.Queryable<R_I054>()
                    .Where(r => r.PARENTSN == SN && r.PNTYPE == "Parent" && r.TRANID == trainid)
                    .Select(r => r.PARENTMODEL)
                    .ToList();
            }
            else if (orderMain.POTYPE == "CTO")
            {
                bool printBTS = false;
                var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                    .Where((c, s) => c.SKUNO == orderMain.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                    .ToList().FirstOrDefault();
                bool sku_control = cseries == null ? false : true;
                bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                      && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == WO).Any();

                printBTS = sku_control ? sku_control : wo_control;
                if (printBTS)
                {
                    var last_tranid = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == SN).OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                        .Select(r => r.TRANID).ToList().FirstOrDefault();
                    if (!string.IsNullOrEmpty(last_tranid))
                    {
                        List<R_I054> list_i054 = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == SN && r.TRANID == last_tranid && r.PNTYPE == "Child"
                        && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false).ToList();
                        foreach (var l in list_i054)
                        {
                            output.Add(l.CHILDMATERIAL);
                        }
                    }
                }
                else
                {
                    O_I137_HEAD i137Head = SFCDB.ORM.Queryable<O_I137_HEAD>().Where(r => r.TRANID == tranid).ToList().FirstOrDefault();
                    i137Head.SALESORDERNUMBER = i137Head.SALESORDERNUMBER.TrimStart('0');


                    var temp1 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I244>().Where(r => r.PARENTSN == SN && r.PNTYPE == "Child"
                     && r.SALESORDERNUMBER == i137Head.SALESORDERNUMBER && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false).Select(r => new { r.FILENAME, r.CREATETIME })
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc).Distinct().ToList();

                    if (temp1.Count > 0)
                    {
                        var filename = temp1[0].FILENAME;
                        if (string.IsNullOrEmpty(filename))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20220121152825"));
                        }

                        var list_i244 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I244>()
                            .Where(r => r.PARENTSN == SN && r.PNTYPE == "Child"
                            && r.SALESORDERNUMBER == i137Head.SALESORDERNUMBER && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false
                            && r.FILENAME == filename)
                            .OrderBy(r => r.SALESORDERLINENUMBER, SqlSugar.OrderByType.Asc).ToList();
                        foreach (var l in list_i244)
                        {
                            output.Add(l.CHILDMATERIAL);
                        }
                    }
                    else {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20220121152825"));
                    }
                    

                    
                }
                //string sql = $@"select CHILDMATERIAL from r_i244 where PARENTSN='{SN}' and PNTYPE = 'Child' and SALESORDERNUMBER='{i137Head.SALESORDERNUMBER}'
                //            and SALESORDERLINENUMBER is not null order by SALESORDERLINENUMBER ";
                //System.Data.DataTable dt = SFCDB.ORM.Ado.GetDataTable(sql);
                //for (int j = 0; j < dt.Rows.Count; j++)
                //{
                //    output.Add(dt.Rows[j]["CHILDMATERIAL"].ToString());
                //}
            }

            return output;
        }
        public List<string> GetTCIFClidLabelSN(OleExec SFCDB, string WO, string SN)
        {
            List<string> output = new List<string>();
            List<O_ORDER_MAIN> listValue = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception($@"{WO} Error!");
            }
            O_ORDER_MAIN orderMain = listValue.FirstOrDefault();
            string po = orderMain.PONO;
            string poline = orderMain.POLINE;
            string tranid = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == orderMain.ITEMID).ToList().FirstOrDefault().TRANID;
            bool isBundle = IsBundle(SFCDB, WO, orderMain.ITEMID);
            if (isBundle)
            {
                output = SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.SN == SN && r.SCANTYPE == "SN" && r.KP_NAME == "AutoKP" && r.VALUE == SN && r.VALID_FLAG == 1).Select(r => r.VALUE).ToList();
            }
            else if (orderMain.POTYPE == "CTO")
            {
                bool printBTS = false;
                var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                    .Where((c, s) => c.SKUNO == orderMain.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                    .ToList().FirstOrDefault();
                bool sku_control = cseries == null ? false : true;
                bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                      && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == WO).Any();
                printBTS = sku_control ? sku_control : wo_control;
                if (printBTS)
                {
                    var last_tranid = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == SN).OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                        .Select(r => r.TRANID).ToList().FirstOrDefault();
                    if (!string.IsNullOrEmpty(last_tranid))
                    {
                        List<R_I054> list_i054 = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == SN && r.TRANID == last_tranid && r.PNTYPE == "Child"
                        && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false).ToList();
                        foreach (var l in list_i054)
                        {
                            output.Add(l.SN);
                        }
                    }
                }
                else
                {
                    O_I137_HEAD i137Head = SFCDB.ORM.Queryable<O_I137_HEAD>().Where(r => r.TRANID == tranid).ToList().FirstOrDefault();
                    i137Head.SALESORDERNUMBER = i137Head.SALESORDERNUMBER.TrimStart('0');
                    var temp1 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I244>().Where(r => r.PARENTSN == SN && r.PNTYPE == "Child"
                     && r.SALESORDERNUMBER == i137Head.SALESORDERNUMBER && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false).Select(r => new { r.FILENAME, r.CREATETIME })
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc).Distinct().ToList();

                    var filename = temp1[0].FILENAME;

                    var list_i244 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I244>()
                        .Where(r => r.PARENTSN == SN && r.PNTYPE == "Child"
                        && r.SALESORDERNUMBER == i137Head.SALESORDERNUMBER && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false
                        && r.FILENAME == filename)
                        .OrderBy(r => r.SALESORDERLINENUMBER, SqlSugar.OrderByType.Asc).ToList();
                    foreach (var l in list_i244)
                    {
                        output.Add(l.SN);
                    }
                    //string sql = $@"select SN from r_i244 where PARENTSN='{SN}' and PNTYPE = 'Child' and SALESORDERNUMBER='{i137Head.SALESORDERNUMBER}'
                    //            and SALESORDERLINENUMBER is not null order by SALESORDERLINENUMBER ";
                    //System.Data.DataTable dt = SFCDB.ORM.Ado.GetDataTable(sql);
                    //for (int j = 0; j < dt.Rows.Count; j++)
                    //{
                    //    output.Add(dt.Rows[j]["SN"].ToString());
                    //}
                }
            }

            return output;
        }
        public List<string> GetTCIFClidLabelCLEI(OleExec SFCDB, string WO, string SN)
        {
            List<string> output = new List<string>();
            List<O_ORDER_MAIN> listValue = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception($@"{WO} Error!");
            }
            O_ORDER_MAIN orderMain = listValue.FirstOrDefault();
            string po = orderMain.PONO;
            string poline = orderMain.POLINE;
            string tranid = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == orderMain.ITEMID).ToList().FirstOrDefault().TRANID;
            bool isBundle = IsBundle(SFCDB, WO, orderMain.ITEMID);
            if (isBundle)
            {
                var listkp = SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.SN == SN && r.SCANTYPE == "SN" && r.KP_NAME == "AutoKP" && r.VALUE == SN && r.VALID_FLAG == 1).ToList();
                foreach (var k in listkp)
                {
                    //&& r.EXKEY2 == "CLEI".Select(r => r.EXVALUE2)
                    if (k.EXKEY2 == "CLEI" && !string.IsNullOrEmpty(k.EXVALUE2))
                    {
                        output.Add(k.EXVALUE2);
                    }
                    else
                    {
                        output.Add("");
                    }
                }

            }
            else if (orderMain.POTYPE == "CTO")
            {
                bool printBTS = false;
                var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                    .Where((c, s) => c.SKUNO == orderMain.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                    .ToList().FirstOrDefault();
                bool sku_control = cseries == null ? false : true;
                bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                      && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == WO).Any();
                printBTS = sku_control ? sku_control : wo_control;
                if (printBTS)
                {
                    var last_tranid = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == SN).OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                        .Select(r => r.TRANID).ToList().FirstOrDefault();
                    if (!string.IsNullOrEmpty(last_tranid))
                    {
                        List<R_I054> list_i054 = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == SN && r.TRANID == last_tranid && r.PNTYPE == "Child"
                        && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false).ToList();
                        foreach (var l in list_i054)
                        {
                            output.Add(l.CLEICODE);
                        }
                    }
                }
                else
                {
                    O_I137_HEAD i137Head = SFCDB.ORM.Queryable<O_I137_HEAD>().Where(r => r.TRANID == tranid).ToList().FirstOrDefault();
                    i137Head.SALESORDERNUMBER = i137Head.SALESORDERNUMBER.TrimStart('0');

                    var temp1 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I244>().Where(r => r.PARENTSN == SN && r.PNTYPE == "Child"
                     && r.SALESORDERNUMBER == i137Head.SALESORDERNUMBER && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false).Select(r => new { r.FILENAME, r.CREATETIME })
                    .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc).Distinct().ToList();

                    var filename = temp1[0].FILENAME;

                    var list_i244 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I244>()
                        .Where(r => r.PARENTSN == SN && r.PNTYPE == "Child"
                        && r.SALESORDERNUMBER == i137Head.SALESORDERNUMBER && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false
                        && r.FILENAME == filename)
                        .OrderBy(r => r.SALESORDERLINENUMBER, SqlSugar.OrderByType.Asc).ToList();
                    foreach (var l in list_i244)
                    {
                        output.Add(l.CLEICODE);
                    }
                }
                //string sql = $@"select CLEICODE from r_i244 where PARENTSN='{SN}' and PNTYPE = 'Child' and SALESORDERNUMBER='{i137Head.SALESORDERNUMBER}'
                //                and SALESORDERLINENUMBER is not null order by SALESORDERLINENUMBER ";
                //System.Data.DataTable dt = SFCDB.ORM.Ado.GetDataTable(sql);
                //for (int j = 0; j < dt.Rows.Count; j++)
                //{
                //    output.Add(dt.Rows[j]["CLEICODE"].ToString());
                //}
            }

            return output;
        }
        public List<string> GetTCIFClidLabelProdID(OleExec SFCDB, string WO, string SN)
        {
            List<string> output = new List<string>();
            List<O_ORDER_MAIN> listValue = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception($@"{WO} Error!");
            }
            O_ORDER_MAIN orderMain = listValue.FirstOrDefault();
            string po = orderMain.PONO;
            string poline = orderMain.POLINE;
            string tranid = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == orderMain.ITEMID).ToList().FirstOrDefault().TRANID;
            bool isBundle = IsBundle(SFCDB, WO, orderMain.ITEMID);
            if (isBundle)
            {
                List<R_SN_KP> listKP = SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.SN == SN && r.SCANTYPE == "SN" && r.KP_NAME == "AutoKP" && r.SN == r.VALUE && r.VALID_FLAG == 1).ToList();
                foreach (var kp in listKP)
                {
                    //var o_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(o => o.TRANID == tranid && o.PN == kp.PARTNO).ToList().FirstOrDefault();
                    var o_item = SFCDB.ORM.Queryable<O_I137_DETAIL>().Where(o => o.TRANID == tranid && o.COMPONENTID == kp.PARTNO).ToList().FirstOrDefault();
                    if (o_item == null)
                    {
                        output.Add("");
                    }
                    else if (string.IsNullOrEmpty(o_item.COMCUSTPRODID))
                    {
                        output.Add("");
                    }
                    else
                    {
                        output.Add(o_item.COMCUSTPRODID);
                    }
                }
            }
            else if (orderMain.POTYPE == "CTO")
            {
                bool printBTS = false;
                var cseries = SFCDB.ORM.Queryable<C_SKU, C_SERIES>((c, s) => c.C_SERIES_ID == s.ID)
                    .Where((c, s) => c.SKUNO == orderMain.PID && SqlSugar.SqlFunc.StartsWith(s.SERIES_NAME, "JNP-ODM")).Select((c, s) => s)
                    .ToList().FirstOrDefault();
                bool sku_control = cseries == null ? false : true;
                bool wo_control = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "JuniperLabelControl" && r.CATEGORY == "CTOPrintBTS"
                      && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == WO).Any();
                printBTS = sku_control ? sku_control : wo_control;
                if (printBTS)
                {
                    var last_tranid = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == SN).OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc)
                        .Select(r => r.TRANID).ToList().FirstOrDefault();
                    if (!string.IsNullOrEmpty(last_tranid))
                    {
                        List<R_I054> list_i054 = SFCDB.ORM.Queryable<R_I054>().Where(r => r.PARENTSN == SN && r.TRANID == last_tranid && r.PNTYPE == "Child"
                        && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false).ToList();
                        foreach (var l in list_i054)
                        {
                            O_I137_DETAIL od = SFCDB.ORM.Queryable<O_I137_DETAIL>()
                           .Where(o => o.TRANID == tranid && o.COMPONENTID == l.CHILDMATERIAL && o.PONUMBER == po && o.ITEM == poline && o.COMSALESORDERLINEITEM == l.SALESORDERLINENUMBER)
                           .ToList().FirstOrDefault();
                            if (od == null)
                            {
                                output.Add("");
                            }
                            else if (string.IsNullOrEmpty(od.COMCUSTPRODID))
                            {
                                output.Add("");
                            }
                            else
                            {
                                output.Add(od.COMCUSTPRODID);
                            }
                        }
                    }
                }
                else
                {
                    O_I137_HEAD i137Head = SFCDB.ORM.Queryable<O_I137_HEAD>().Where(r => r.TRANID == tranid).ToList().FirstOrDefault();
                    string salesordernumber = i137Head.SALESORDERNUMBER.TrimStart('0');
                    i137Head.SALESORDERNUMBER = i137Head.SALESORDERNUMBER.TrimStart('0');
                    var temp1 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I244>().Where(r => r.PARENTSN == SN && r.PNTYPE == "Child"
                    && r.SALESORDERNUMBER == i137Head.SALESORDERNUMBER && SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false).Select(r => new { r.FILENAME, r.CREATETIME })
                   .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc).Distinct().ToList();

                    var filename = temp1[0].FILENAME;

                    var list_i244 = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I244>()
                        .Where(r => r.PARENTSN == SN && r.PNTYPE == "Child" && r.SALESORDERNUMBER == salesordernumber &&
                        SqlSugar.SqlFunc.IsNullOrEmpty(r.SALESORDERLINENUMBER) == false &&
                        r.FILENAME == filename
                        )
                        .OrderBy(r => r.SALESORDERLINENUMBER, SqlSugar.OrderByType.Asc).ToList();
                    foreach (var l in list_i244)
                    {
                        O_I137_DETAIL od = SFCDB.ORM.Queryable<O_I137_DETAIL>()
                            .Where(o => o.TRANID == tranid && o.COMPONENTID == l.CHILDMATERIAL && o.PONUMBER == po && o.ITEM == poline && o.COMSALESORDERLINEITEM == i137Head.SALESORDERNUMBER)
                            .ToList().FirstOrDefault();
                        if (od == null)
                        {
                            output.Add("");
                        }
                        else if (string.IsNullOrEmpty(od.COMCUSTPRODID))
                        {
                            output.Add("");
                        }
                        else
                        {
                            output.Add(od.COMCUSTPRODID);
                        }
                    }
                }
                //string sql = $@"select CHILDMATERIAL from r_i244 where PARENTSN='{SN}' and PNTYPE = 'Child' and SALESORDERNUMBER='{i137Head.SALESORDERNUMBER}'
                //            and SALESORDERLINENUMBER is not null order by SALESORDERLINENUMBER ";
                //System.Data.DataTable dt = SFCDB.ORM.Ado.GetDataTable(sql);
                //for (int j = 0; j < dt.Rows.Count; j++)
                //{
                //    string childmaterial = dt.Rows[j]["CHILDMATERIAL"].ToString();
                //    O_I137_DETAIL od = SFCDB.ORM.Queryable<O_I137_DETAIL>().Where(o => o.TRANID == tranid && o.COMPONENTID == childmaterial).ToList().FirstOrDefault();
                //    if (od == null)
                //    {
                //        output.Add("");
                //    }
                //    else if (string.IsNullOrEmpty(od.COMCUSTPRODID))
                //    {
                //        output.Add("");
                //    }
                //    else
                //    {
                //        output.Add(od.COMCUSTPRODID);
                //    }                    
                //}
            }

            return output;
        }

        public List<string> GetCartonListByDN(OleExec SFCDB, string DN)
        {

            //var listValue = SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I282, MESDataObject.Module.Juniper.R_I139, R_SN, R_SN_PACKING, R_PACKING>
            //    ((i282, i39, r, rsp, rp) => i282.ASNNUMBER == i39.ASNNUMBER && i39.SERIALID == r.SN && r.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
            //    .Where((i282, i39, r, rsp, rp) => i282.DELIVERYNUMBER == DN && r.VALID_FLAG == "1" &&i39.DELIVERYCODE=="01" && SqlSugar.SqlFunc.IsNullOrEmpty(i282.ERRORCODE))
            //    .Select((i282, i39, r, rsp, rp) => rp.PACK_NO).ToList();
            //return listValue.Distinct().ToList();
            //为了兼容I139不传SN的方法
            var strSql = $@" select distinct p.pack_no from r_sn s,r_sn_packing sp,r_packing p where workorderno in (
                                     select prewo from o_order_main o where (o.pono,o.poline) in(
                                        SELECT i139.ponumber, i139.item
                                          FROM R_I282 I282, R_I139 I139
                                         WHERE I282.deliverynumber = '{DN}'
                                           and I139.Asnnumber = I282.Asnnumber
                                        union
                                        select sm.po_number as ponumber, sm.po_line_no as item
                                          from r_jnp_doa_shipments sm, r_jnp_doa_shipments_ack smack
                                         where smack.deliverynumber = '{DN}'
                                           and sm.asnnumber = smack.asnnumber
                                         )
                                     )
                                     and s.id = sp.sn_id and sp.pack_id = p.id";
            var res = SFCDB.RunSelect(strSql);
            var ret = new List<string>();
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                ret.Add(res.Tables[0].Rows[i]["pack_no"].ToString());
            }
            return ret;
        }

        public string GetEACULabelCOO(OleExec SFCDB, string SN)
        {
            string out_put = "";
            List<R_SN_KP> listKp = SFCDB.ORM.Queryable<R_SN_KP>().Where(k => k.SN == SN && k.SCANTYPE == "SN" && k.KP_NAME == "AutoKP" && k.VALUE == SN && k.VALID_FLAG == 1).ToList();
            string coo = "";
            if (listKp.Count != 0)
            {
                coo = listKp.FirstOrDefault().LOCATION;
                var control = SFCDB.ORM.Queryable<C_CONTROL>()
                    .Where(r => r.CONTROL_NAME == "EACU_LABEL_SETTING" && r.CONTROL_TYPE == "COO" && r.CONTROL_VALUE == coo).ToList().FirstOrDefault();
                if (control != null)
                {
                    out_put = control.CONTROL_DESC;
                }
                #region 代碼寫死
                //switch (coo)
                //{
                //    case "CN":
                //        //Assembled in China 
                //        out_put = "Сделано в Китае";
                //        break;
                //    case "US":
                //        //Assembled in USA 
                //        out_put = "Сделано в США";
                //        break;
                //    case "MX":
                //        //Assembled in Mexico 
                //        out_put = "Сделано в Мексике";
                //        break;
                //    case "MY":
                //        //Assembled in Malaysia 
                //        out_put = "Сделано в Малайзии";
                //        break;
                //    case "TW":
                //        //Assembled in Taiwan 
                //        out_put = "Сделано в Тайвани";
                //        break;
                //    case "VN":
                //        // Assembled in Vietnam
                //        out_put = "Сделано в Вьетнам";
                //        break;
                //    default:
                //        out_put = "";
                //        break;
                //}
                #endregion
            }
            return out_put;
        }
        public string GetEACULabelModel(OleExec SFCDB, string SN, string PLANT)
        {
            string out_put = "";
            var oaa = SFCDB.ORM.Queryable<O_AGILE_ATTR, R_SN>((o, r) => o.ITEM_NUMBER == r.SKUNO)
                .Where((o, r) => r.SN == SN && r.VALID_FLAG == "1" && o.PLANT == PLANT).OrderBy((o, r) => o.DATE_CREATED, SqlSugar.OrderByType.Desc)
                .Select((o, r) => o).ToList().FirstOrDefault();
            if (oaa != null)
            {
                out_put = oaa.REGULATORY_MODEL;
            }
            return out_put;
        }
        public string GetEACULabelType(OleExec SFCDB, string SN, string PLANT)
        {
            string out_put = "";
            var oaa = SFCDB.ORM.Queryable<O_AGILE_ATTR, R_SN>((o, r) => o.ITEM_NUMBER == r.SKUNO)
                .Where((o, r) => r.SN == SN && r.VALID_FLAG == "1" && o.PLANT == PLANT).OrderBy((o, r) => o.DATE_CREATED, SqlSugar.OrderByType.Desc)
                .Select((o, r) => o).ToList().FirstOrDefault();
            string remodel = "";
            string type = "";
            if (oaa != null && oaa.REGULATORY_MODEL != null)
            {
                remodel = oaa.REGULATORY_MODEL.TrimStart();

                var control = SFCDB.ORM.Queryable<C_CONTROL>()
                    .Where(r => r.CONTROL_NAME == "EACU_LABEL_SETTING" && r.CONTROL_TYPE == "TYPE" && SqlSugar.SqlFunc.StartsWith(remodel, r.CONTROL_LEVEL)).ToList().FirstOrDefault();
                if (control != null)
                {
                    out_put = control.CONTROL_DESC;
                }
                #region 代碼寫死
                //if (remodel.StartsWith("NSM4000") || remodel.StartsWith("NS-SM-A-BSE") || remodel.StartsWith("NS-SM-A-CM") || remodel.StartsWith("NS-SM-S-BSE")
                //    || remodel.StartsWith("NS-SM-XL-A-BSE") || remodel.StartsWith("SPC1500-A-BSE"))
                //{
                //    type = "Management system";
                //}
                //else if (remodel.StartsWith("EX") || remodel.StartsWith("SRX") || remodel.StartsWith("ACX") || remodel.StartsWith("MX")
                //    || remodel.StartsWith("Т") || remodel.StartsWith("QFX") || remodel.StartsWith("LN") || remodel.StartsWith("NFX")
                //    || remodel.StartsWith("OCX") || remodel.StartsWith("C2000") || remodel.StartsWith("C3000") || remodel.StartsWith("C4000")
                //    || remodel.StartsWith("C5000") || remodel.StartsWith("CTP150") || remodel.StartsWith("CTP2K"))
                //{
                //    type = "Switch (L2) and router (L3)";
                //}
                //else if (remodel.StartsWith("JSA") || remodel.StartsWith("IC") || remodel.StartsWith("WLC"))
                //{
                //    type = "Switch (L2)";
                //}
                //else if (remodel.StartsWith("PTX") || remodel.StartsWith("E") || remodel.StartsWith("M") || remodel.StartsWith("J-series")
                //    || remodel.StartsWith("SSG") || remodel.StartsWith("WLC2800"))
                //{
                //    type = "Router (L3)";
                //}
                //else if (remodel.StartsWith("WLA"))
                //{
                //    type = "WLAN access point";
                //}
                //else if (remodel.StartsWith("TCA"))
                //{
                //    type = "Timing client";
                //}
                //else if (remodel.StartsWith("BTI"))
                //{
                //    type = "Packet Optical Transport System";
                //}
                //else if (remodel.StartsWith("EX-RPS"))
                //{
                //    type = "Redundant Power System";
                //}
                //else if (remodel.StartsWith("EX-BTI7020")) {
                //    type = "Passive Shelf";
                //}
                //switch (type)
                //{
                //    //case "NSM4000":
                //    //case "NS-SM-A-BSE":
                //    //case "NS-SM-A-CM":
                //    //case "NS-SM-S-BSE":
                //    //case "NS-SM-XL-A-BSE":
                //    //case "SPC1500-A-BSE":
                //    case "Management system":
                //        //Management system
                //        out_put = "Оборудование  автоматизированной системы управления и мониторинга";
                //        break;
                //    //case "EX":
                //    //case "SRX":
                //    //case "ACX":
                //    //case "MX":
                //    //case "Т":
                //    //case "QFX":
                //    //case "LN":
                //    //case "NFX":
                //    //case "OCX":
                //    //case "C2000":
                //    //case "C3000":
                //    //case "C4000":
                //    //case "C5000":
                //    //case "CTP150":
                //    //case "CTP2K":
                //    case "Switch (L2) and router (L3)":
                //        //Switch (L2) and router (L3)
                //        out_put = "Оборудование коммутации и маршрутизации пакетов информации сетей передачи данных";
                //        break;
                //    //case "JSA":
                //    //case "IC":
                //    //case "WLC":                    
                //    case "Switch (L2)":
                //        //Switch (L2)
                //        out_put = "Оборудование коммутации пакетов информации сетей передачи данных";
                //        break;
                //    //case "PTX":
                //    //case "E":
                //    //case "M":
                //    //case "J-series":
                //    //case "SSG":
                //    //case "WLC2800":
                //    case "Router (L3)":
                //          //Router (L3)
                //          out_put = "Оборудование маршрутизации пакетов информации сетей передачи данных";
                //        break;
                //    //case "WLA":
                //    case "WLAN access point":
                //        //WLAN access point
                //        out_put = "Оборудование радиодоступа";
                //        break;
                //    //case "TCA":
                //    case "Timing client":
                //        //Timing client
                //        out_put = "Оборудование тактовой сетевой синхронизации";
                //        break;
                //    //case "BTI":
                //    case "Packet Optical Transport System":
                //        //Packet Optical Transport System
                //        out_put = "Оборудование оптических многоканальных систем передачи со спектральным разделением каналов";
                //        break;
                //    //case "EX-RPS":
                //    case "Redundant Power System":
                //        //Redundant Power System
                //        out_put = "Установка питания постоянного тока";
                //        break;
                //    //case "EX-BTI7020":
                //    case "Passive Shelf":
                //        //Passive Shelf
                //        out_put = "Оптический мультиплексор (демультиплексор)";
                //        break;
                //    default:
                //        out_put = "";
                //        break;
                //}
                #endregion
            }
            return out_put;
        }

        public string GetUPCNotPrint(OleExec SFCDB, string WO)
        {
            string output = "TRUE";
            var o_main = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{WO} Error!");
            }
            var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == o_main.ITEMID).ToList().FirstOrDefault();
            if (o_main.POTYPE == "BTS")
            {
                bool upc = i_item.PACKOUTLABEL == "UPC" ? true : false;
                bool bull = (i_item.CARTONLABEL2 == "Bulk" && Convert.ToDouble(o_main.QTY) > 9) ? true : false;
                bool bndl = IsBundle(SFCDB, WO, o_main.ITEMID);
                if (upc && !bull && !bndl)
                {
                    output = "FALSE";
                }
            }
            return output;
        }

        public string GetTCIFParentBundleNotPrint(OleExec SFCDB, string WO)
        {
            string output = "FALSE";
            var o_main = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{WO} Error!");
            }
            var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == o_main.ITEMID).ToList().FirstOrDefault();
            if (o_main.POTYPE == "BTS")
            {
                var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                    .Where((C, S, W) => W.WORKORDERNO == WO)
                    .Select((C, S, W) => C)
                    .First();
                bool bndl = IsBundle(SFCDB, WO, o_main.ITEMID);
                if (cs.SERIES_NAME == "Juniper-Optics")
                {
                    bool tcif = i_item.PACKOUTLABEL == "TCIF" ? true : false;
                    bool bull = (i_item.CARTONLABEL2 == "Bulk" && Convert.ToDouble(o_main.QTY) > 9) ? true : false;
                    // && t.SOID == i_item.SOID
                    var polines = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(t => t.PONUMBER == i_item.PONUMBER).Count();

                    if (tcif)
                    {
                        if (bndl & polines > 1)
                        {
                            output = "FALSE";
                        }
                        else
                        {
                            output = "TRUE";
                        }
                    }
                    else
                    {
                        output = "TRUE";
                    }
                }
                else
                {
                    if (!bndl)
                    {
                        output = "TRUE";
                    }
                }


            }
            else
            {
                //if (i_item.PACKOUTLABEL == "UPC")
                //{
                //    throw new Exception($@"CTO WO[{WO}] PACKOUTLABEL Is UPC");
                //}

            }
            return output;
        }

        public string GetTCIFParentNotPrint(OleExec SFCDB, string WO)
        {
            //只有Bulk 不打
            string output = "FALSE";
            var o_main = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{WO} Error!");
            }
            var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == o_main.ITEMID).ToList().FirstOrDefault();
            if (o_main.POTYPE == "BTS")
            {
                #region v 1.0
                //var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                //    .Where((C, S, W) => W.WORKORDERNO == WO)
                //    .Select((C, S, W) => C)
                //    .First();
                //if (cs.SERIES_NAME == "Juniper-Optics")
                //{
                //    bool tcif = i_item.PACKOUTLABEL == "TCIF" ? true : false;
                //    bool bull = (i_item.CARTONLABEL2 == "Bulk" && Convert.ToDouble(o_main.QTY) > 9) ? true : false;
                //    bool bndl = IsBundle(SFCDB, WO, o_main.ITEMID);
                //    if (bull)
                //    {
                //        //Bulk 不打
                //        output = "TRUE";
                //        if (i_item.PACKOUTLABEL == "UPC")
                //        {
                //            //2021.01.30 Tat-Ho（達豪）要求先拿掉卡關，不報錯
                //            //throw new Exception($@"BTS Bulk WO[{WO}] PACKOUTLABEL Is UPC");
                //        }
                //    }
                //    //Tat 移除
                //    //else if (bndl)
                //    //{
                //    //    output = "TRUE";
                //    //    if (i_item.PACKOUTLABEL == "UPC")
                //    //    {
                //    //        throw new Exception($@"BNDL WO[{WO}] PACKOUTLABEL Is UPC");
                //    //    }
                //    //}
                //    else if (!tcif)
                //    {
                //        output = "TRUE";
                //    }
                //}
                #endregion

                #region v 1.1 LJD 2021-11-30
                var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                    .Where((C, S, W) => W.WORKORDERNO == WO)
                    .Select((C, S, W) => C)
                    .First();
                bool tcif = (i_item.PACKOUTLABEL == "TCIF" || string.IsNullOrEmpty(i_item.PACKOUTLABEL)) ? true : false;
                if (!tcif)
                {
                    //not TCIF no print
                    output = "TRUE";
                    return output;
                }
                bool bndl = IsBundle(SFCDB, WO, o_main.ITEMID);
                if (cs.SERIES_NAME == "Juniper-Optics")//只有Optics才需要判斷是否Bundle
                {
                    if (!bndl)//只有非Bundle才需要判斷是否Bulk
                    {
                        bool bull = (i_item.CARTONLABEL2 == "Bulk" && Convert.ToDouble(o_main.QTY) > 9) ? true : false;
                        if (bull)
                        {
                            //Bulk no need print
                            output = "TRUE";
                        }
                        else
                        {
                            output = "FALSE";
                        }
                    }
                    else
                    {
                        //Optics bundle no need print
                        //output = "TRUE";
                        output = "FALSE";//Optics bundle 又說要打印了 2021-12-16
                    }
                }
                else
                {
                    #region Non-Optics時不管是BNDL還是BULK, 都要打印
                    output = "FALSE";

                    //if (!bndl)
                    //{
                    //    output = "FALSE";
                    //}
                    //else if (bndl && o_main.PLANT == "FJZ")
                    //{
                    //    output = "FALSE";
                    //}
                    //else
                    //{
                    //    output = "TRUE";
                    //}
                    #endregion
                }
                #endregion
            }
            else
            {
                if (i_item.PACKOUTLABEL == "UPC")
                {
                    output = "TRUE";
                    //throw new Exception($@"CTO WO[{WO}] PACKOUTLABEL Is UPC");
                }
            }
            return output;
        }
        public string GetTCIFChildNotPrint(OleExec SFCDB, string WO)
        {
            //只有 BNDL 和CTO才會打TCIF Child Label
            string output = "TRUE";
            var o_main = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{WO} Error!");
            }
            var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == o_main.ITEMID).ToList().FirstOrDefault();
            if (i_item.PACKOUTLABEL == "UPC")
            {
                output = "TRUE";
                return output;
            }
            if (o_main.POTYPE == "BTS")
            {
                #region V1.0
                //bool bull = (i_item.CARTONLABEL2 == "Bulk" && Convert.ToDouble(o_main.QTY) > 9) ? true : false;
                //if (bull)
                //{
                //    throw new Exception($@"BTS Bulk WO[{WO}] PACKOUTLABEL Is UPC");
                //}
                //bool bndl = IsBundle(SFCDB, WO, o_main.ITEMID);
                //if (bndl)
                //{
                //    throw new Exception($@"BNDL WO[{WO}] PACKOUTLABEL Is UPC");
                //}
                //if (bndl)
                //{
                //    //BNDL 打 for optics bundle only
                //    //output = "FALSE";
                //    var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                //    .Where((C, S, W) => W.WORKORDERNO == WO)
                //    .Select((C, S, W) => C)
                //    .First();
                //    if (cs.SERIES_NAME == "Juniper-Optics")
                //    {
                //        output = "TRUE";
                //    }
                //    else
                //    {
                //        output = "FALSE";
                //    }
                //}
                #endregion

                #region V1.1 LJD 2021-11-30 only CTO need print child label
                var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                .Where((C, S, W) => W.WORKORDERNO == WO)
                .Select((C, S, W) => C)
                .First();
                if (cs.SERIES_NAME == "Juniper-Optics")
                {
                    output = "TRUE";
                }
                else
                {
                    bool bndl = IsBundle(SFCDB, WO, o_main.ITEMID);
                    if (!bndl)
                    {
                        output = "TRUE";
                    }
                    else if (bndl && o_main.PLANT == "FVN")
                    {
                        output = "TRUE";
                    }
                }
                #endregion
            }
            else
            {
                output = "FALSE";
            }
            return output;
        }
        public string GetOverpackNotPrint(OleExec SFCDB, string WO, string CARTON)
        {
            string output = "TRUE";
            var o_main = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{WO} Error!");
            }
            var pack_Obj = SFCDB.ORM.Queryable<R_PACKING>().Where(p => p.PACK_NO == CARTON).ToList().FirstOrDefault();
            if (pack_Obj == null)
            {
                throw new Exception($@"{CARTON} Error!");
            }

            if (o_main.POTYPE == "BTS")
            {
                #region v 1.0
                //var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == o_main.ITEMID).ToList().FirstOrDefault();
                //var pack_list = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING>((s, r) => s.ID == r.SN_ID)
                //    .Where((s, r) => r.PACK_ID == pack_Obj.ID && s.VALID_FLAG == "1")
                //    .Select((s, r) => s.SN).Distinct().ToList();

                //var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                //    .Where((C, S, W) => W.WORKORDERNO == WO)
                //    .Select((C, S, W) => C)
                //    .First();

                //bool bull = (i_item.CARTONLABEL2 == "Bulk" && Convert.ToDouble(o_main.QTY) >= 1) ? true : false;
                //bool bndl = IsBundle(SFCDB, WO, o_main.ITEMID);
                //if (cs.SERIES_NAME == "Juniper-FRU")
                //{
                //    output = "TRUE";
                //}
                //else if (bull)
                //{
                //    output = "FALSE";
                //}
                //else if (bndl)
                //{
                //    //BNDL 不打
                //    //又說要打了
                //    output = "FALSE";
                //    //output = "TRUE";
                //}
                //else if (pack_list.Count == 1)
                //{
                //    //卡通裡面的數量=1不打
                //    //output = "TRUE";
                //    //又說要打了
                //    output = "FALSE";
                //}
                //else if (pack_list.Count > 1)
                //{
                //    output = "FALSE";
                //}
                #endregion

                #region v 1.1 LJD 2021-11-30
                var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == o_main.ITEMID).ToList().FirstOrDefault();
                var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                    .Where((C, S, W) => W.WORKORDERNO == WO)
                    .Select((C, S, W) => C)
                    .First();
                if (cs.SERIES_NAME == "Juniper-Optics")
                {
                    output = "FALSE";
                }
                else
                {
                    output = "TRUE";
                    return output;
                }
                #endregion
            }
            else
            {
                //CTO 不打
                output = "TRUE";
            }
            return output;
        }

        public string GetOverpackNotPrintWithSNLen(OleExec SFCDB, string WO, string CARTON, List<string> SNs, string MinLen, string MaxLen)
        {
            string output = "TRUE";

            var minLen = int.Parse(MinLen);
            var maxLen = int.Parse(MaxLen);


            for (int i = 0; i < SNs.Count; i++)
            {
                var l = SNs[i].Length;
                if (l < minLen || l > maxLen)
                {
                    output = "TRUE";
                    return output;
                }
            }

            var o_main = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.ORIGINALID == o.ID)
                .Where((r, o) => r.WO == WO && r.VALID == "1").Select((r, o) => o).ToList().FirstOrDefault();
            if (o_main == null)
            {
                throw new Exception($@"{WO} Error!");
            }
            var pack_Obj = SFCDB.ORM.Queryable<R_PACKING>().Where(p => p.PACK_NO == CARTON).ToList().FirstOrDefault();
            if (pack_Obj == null)
            {
                throw new Exception($@"{CARTON} Error!");
            }

            if (o_main.POTYPE == "BTS")
            {
                var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == o_main.ITEMID).ToList().FirstOrDefault();
                var pack_list = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING>((s, r) => s.ID == r.SN_ID)
                    .Where((s, r) => r.PACK_ID == pack_Obj.ID && s.VALID_FLAG == "1")
                    .Select((s, r) => s.SN).Distinct().ToList();

                var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                    .Where((C, S, W) => W.WORKORDERNO == WO)
                    .Select((C, S, W) => C)
                    .First();

                //bool bull = (i_item.CARTONLABEL2 == "Bulk" && Convert.ToDouble(o_main.QTY) >= 1) ? true : false;
                bool bull = (i_item.CARTONLABEL2 == "Bulk" && Convert.ToDouble(o_main.QTY) >= 9) ? true : false;//Tat-Ho確認要 > 9     2021-12-16 
                bool bndl = IsBundle(SFCDB, WO, o_main.ITEMID);
                if (cs.SERIES_NAME == "Juniper-FRU")
                {
                    output = "TRUE";
                }
                else if (bull)
                {
                    output = "FALSE";
                }
                else if (bndl)
                {
                    //BNDL 不打
                    //又說要打了
                    output = "FALSE";
                    //output = "TRUE";
                }
                else if (pack_list.Count == 1)
                {
                    //卡通裡面的數量=1不打
                    //output = "TRUE";
                    //又說要打了
                    output = "FALSE";
                }
                else if (pack_list.Count > 1)
                {
                    output = "FALSE";
                }
                #region LJD 2021-9-21
                //var i_item = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == o_main.ITEMID).ToList().FirstOrDefault();
                //var serial = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                //    .Where((C, S, W) => W.WORKORDERNO == WO)
                //    .Select((C, S, W) => C)
                //    .First();

                //bool isBNDL = IsBundle(SFCDB, WO, o_main.ITEMID);
                //bool isMIDNull = (i_item.MATERIALID == null || i_item.MATERIALID == "") ? true : false;
                //bool isBulk = i_item.CARTONLABEL2 == "Bulk" ? true : false;
                //bool isFNSB = o_main.OFFERINGTYPE == "Fixed Nonstockable Bundle" ? true : false;
                //bool isFRU = o_main.OFFERINGTYPE == "FRU" ? true : false;
                //bool isOptics = serial.SERIES_NAME == "Juniper-Optics" ? true : false;
                //if (!isBNDL && isFRU && isMIDNull && isBulk && isOptics)
                //{
                //    output = "FALSE";
                //}
                //else if (isBNDL && isFNSB && !isMIDNull && !isBulk && isOptics)
                //{
                //    output = "FALSE";
                //}
                //else if (isBNDL && isFNSB && !isMIDNull && !isBulk && !isOptics)
                //{
                //    output = "TRUE";
                //}
                #endregion

            }
            else
            {
                //CTO 不打
                output = "TRUE";
            }
            return output;
        }


        public string GetPackingListNotPrint(OleExec SFCDB, string WO)
        {
            return "TRUE";
        }
        public string GetJuniperPNByWO(OleExec SFCDB, string WO)
        {
            string output = "";
            var last_release = SFCDB.ORM.Queryable<R_WO_BASE, O_AGILE_ATTR>((r, o) => r.SKUNO == o.ITEM_NUMBER && r.PLANT == o.PLANT)
                .Where((r, o) => r.WORKORDERNO == WO).OrderBy((r, o) => o.DATE_CREATED, SqlSugar.OrderByType.Desc)
                .Select((r, o) => o).ToList().FirstOrDefault();
            O_AGILE_ATTR aglie = last_release;
            //if (last_release != null)
            //{
            //    DateTime? last_dt = last_release.RELEASE_DATE;
            //    aglie = SFCDB.ORM.Queryable<R_WO_BASE, O_AGILE_ATTR>((r, o) => r.SKUNO == o.ITEM_NUMBER && r.PLANT == o.PLANT)
            //    .Where((r, o) => r.WORKORDERNO == WO && o.RELEASE_DATE == last_dt)
            //    .OrderBy((r, o) => o.DATE_CREATED, SqlSugar.OrderByType.Desc).Select((r, o) => o)
            //    .ToList().FirstOrDefault();
            //}

            if (aglie == null)
            {
                throw new Exception($@"SKU OF {WO} NO MAPPING IN O_AGILE_ATTR!");
            }
            if (string.IsNullOrEmpty(aglie.CUSTPARTNO))
            {
                throw new Exception($@"JUNIPER PN MAPPING IN O_AGILE_ATTR IS NULL!");
            }
            output = aglie.CUSTPARTNO;
            return output;
        }

        public string GetJuniperPNBySku(OleExec SFCDB, string SKUNO)
        {
            string output = "";
            var last_release = SFCDB.ORM.Queryable<O_AGILE_ATTR>()
                .Where(o => o.ITEM_NUMBER == SKUNO).OrderBy(o => o.DATE_CREATED, SqlSugar.OrderByType.Desc)
                .ToList().FirstOrDefault();
            O_AGILE_ATTR aglie = last_release;
            if (last_release != null)
            {
                //DateTime? last_dt = last_release.RELEASE_DATE;
                //aglie = SFCDB.ORM.Queryable<O_AGILE_ATTR>()
                //.Where(o => o.ITEM_NUMBER == SKUNO).OrderBy(o => o.RELEASE_DATE, SqlSugar.OrderByType.Desc)
                //.OrderBy( o => o.DATE_CREATED, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            }
            if (aglie == null)
            {
                throw new Exception($@"SKU [{SKUNO}] NO MAPPING IN O_AGILE_ATTR!");
            }
            if (string.IsNullOrEmpty(aglie.CUSTPARTNO))
            {
                throw new Exception($@"JUNIPER PN MAPPING IN O_AGILE_ATTR IS NULL!");
            }
            output = aglie.CUSTPARTNO;
            return output;
        }
        public string GetCLEIBySku(OleExec SFCDB, string SKUNO)
        {
            string output = "";
            var last_release = SFCDB.ORM.Queryable<O_AGILE_ATTR>()
                .Where(o => o.ITEM_NUMBER == SKUNO && o.ACTIVED == "1").OrderBy(o => o.RELEASE_DATE, SqlSugar.OrderByType.Desc)
                .ToList().FirstOrDefault();
            O_AGILE_ATTR aglie = last_release;
            if (last_release != null)
            {
                //DateTime? last_dt = last_release.RELEASE_DATE;
                //aglie = SFCDB.ORM.Queryable<O_AGILE_ATTR>()
                //.Where(o => o.ITEM_NUMBER == SKUNO).OrderBy(o => o.RELEASE_DATE, SqlSugar.OrderByType.Desc)
                //.OrderBy(o => o.DATE_CREATED, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            }
            if (aglie == null)
            {
                throw new Exception($@"SKU [{SKUNO}] NO MAPPING IN O_AGILE_ATTR!");
            }
            if (string.IsNullOrEmpty(aglie.CLEI_CODE))
            {
                throw new Exception($@"SKU [{SKUNO}] CLEI IN O_AGILE_ATTR IS NULL!");
            }
            output = aglie.CLEI_CODE;
            return output;
        }
        public string GetAgileRevBywo(OleExec SFCDB, string WO)
        {
            string output = "";
            R_WO_BASE WONumber = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WO).ToList().FirstOrDefault();
            var last_release = SFCDB.ORM.Queryable<O_AGILE_ATTR, R_WO_BASE>((OAA, RWB) => OAA.ITEM_NUMBER == RWB.SKUNO)
                .Where((OAA, RWB) => RWB.DOWNLOAD_DATE > OAA.DATE_CREATED && RWB.WORKORDERNO == WO).OrderBy((OAA, RWB) => OAA.DATE_CREATED, SqlSugar.OrderByType.Desc).Select((OAA, RWB) => OAA)
                .ToList().FirstOrDefault();
            O_AGILE_ATTR aglie = last_release;
            if (aglie == null)
            {

                throw new Exception($@"SKU [{WONumber.SKUNO}] NO MAPPING IN O_AGILE_ATTR!");
            }
            if (string.IsNullOrEmpty(aglie.REV))
            {
                throw new Exception($@"SKU  [{WONumber.SKUNO}]  REV IN O_AGILE_ATTR IS NULL!");
            }
            output = aglie.REV.Substring(0, 1);
            return output;
        }
        public string GetVerByWo(OleExec SFCDB, string WO)
        {
            //var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            LogicObject.WorkOrder wo = new LogicObject.WorkOrder();
            //.Initwo(WO, SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            wo.Init(WO, SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            var skuno = wo.SkuNO;
            var strsql = $@"select * from o_agile_attr where item_number ='{skuno}' order by effective_date desc";
            var ret = SFCDB.RunSelect(strsql);
            return ret.Tables[0].Rows[0]["REV"].ToString().Substring(0, 1);
        }
        public string GetGroupIDByWo(OleExec SFCDB, string WO)
        {
            //var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var GROUPID = SFCDB.ORM.Queryable<R_PRE_WO_HEAD>().Where(t => t.WO == WO).ToList();
            if (GROUPID == null)
            {
                return "WO NO FOUND";
            }
            return GROUPID[0].GROUPID;

        }
        public string GetWoQtyByWo(OleExec SFCDB, string WO)
        {
            //var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);
            var strsql = $@"select*From R_PRE_WO_HEAD where WO='{WO}'";
            var ret = SFCDB.RunSelect(strsql);
            var qty = ret.Tables[0].Rows[0]["WOQTY"].ToString();
            return qty.Substring(0, qty.IndexOf('.'));
        }
        public string GetPoLineByWo(OleExec SFCDB, string WO)
        {
            //var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);
            var poline = SFCDB.ORM.Queryable<R_PRE_WO_HEAD>().Where(t => t.WO == WO).ToList();
            if (poline == null)
            {
                return "WO NO FOUND";
            }
            return poline[0].POLINE;
        }
        public string GetSoByWo(OleExec SFCDB, string WO)
        {
            //var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);
            var SO = SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((om, oi, oh) => om.ITEMID == oi.ID && oi.TRANID == oh.TRANID).
                Where((om, oi, oh) => om.PREWO == WO).Select((om, oi, oh) => oh.SALESORDERNUMBER).ToList();

            if (SO == null)
            {
                return "SO NO FOUND";
            }
            return SO[0].ToString();
        }
        public string GetPoLineQtyByPo(OleExec SFCDB, string PO)
        {
            //var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);
            var PoLineQty = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == PO).ToList();

            if (PoLineQty == null)
            {
                return "PO NO FOUND";
            }
            return PoLineQty.Count.ToString();
        }
        public string GetPoLineDetailByPo(OleExec SFCDB, string PO)
        {
            //var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);
            var PoLineDetail = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == PO).ToList();
            var Detail = "";
            if (PoLineDetail == null)
            {
                return "PO NO FOUND";
            }
            if (PoLineDetail.Count > 1)
            {
                string[] Poline = new string[PoLineDetail.Count];
                for (int i = 0; i < PoLineDetail.Count; i++)
                {
                    Poline[i] = PoLineDetail[i].POLINE.ToString();
                }

                Detail = string.Join(",", Poline);


                //foreach (O_ORDER_MAIN OM in PoLineDetail)
                //{
                //    string[] skus = OM.POLINE.Split(',');
                //    Detail += skus[0].ToString();
                //    Detail.Split(',');
                //}
            }
            else
            {
                Detail = PoLineDetail[0].POLINE;
            }

            return Detail.ToString();
        }
        public string GetComplete_DeliveryByPo(OleExec SFCDB, string WO)
        {
            //var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);
            var Complete_Delivery = SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((om, oi, oh) => om.ITEMID == oi.ID && oi.TRANID == oh.TRANID).
                Where((om, oi, oh) => om.PREWO == WO).Select((om, oi, oh) => oh.COMPLETEDELIVERY).ToList();

            if (Complete_Delivery == null)
            {
                return "PO NO FOUND";
            }
            return Complete_Delivery[0].ToString();
        }
        public string GetSkuNameRevSNMac(OleExec SFCDB, string SN)
        {

            string output = "MAC IS NULL";
            R_SN rsn = SFCDB.ORM.Queryable<R_SN>().Where(x => x.SN == SN && x.VALID_FLAG == "1").First();
            string rev = SFCDB.ORM.Queryable<C_SKU>().Where(x => x.SKUNO == rsn.SKUNO).Select(x => x.VERSION).First();
            string mac = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                   .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.MAC != "" || t.MAC != null)
                   .Select(t => t.MAC)
                   .First();
            if (string.IsNullOrEmpty(mac))
            {
                return output;
            }
            output = rsn.SKUNO + " /" + rev + ", " + SN + ", " + mac;
            return output;
        }
        public bool IsBundle(OleExec SFCDB, string wo, string item_id)
        {
            bool isBundle = false;

            var order = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == wo).First();
            if (order == null)
            {
                throw new Exception($@"WO:{wo} not in O_ORDER_MAIN!");
            }
            if (order.OFFERINGTYPE == "Fixed Nonstockable Bundle")
            {
                var itemObj = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == item_id).ToList().FirstOrDefault();
                if (itemObj == null)
                {
                    throw new Exception($@"WO:{wo}/ID:{item_id} not in o_i137_item!");
                }

                if (!string.IsNullOrEmpty(itemObj.MATERIALID))
                {
                    isBundle = true;
                }
            }
            #region 不能通過BULK判斷是否Bundle
            //else
            //{
            //    var itemObj = SFCDB.ORM.Queryable<O_I137_ITEM>().Where(r => r.ID == item_id).ToList().FirstOrDefault();
            //    if (itemObj == null)
            //    {
            //        throw new Exception($@"WO:{wo}/ID:{item_id} not in o_i137_item!");
            //    }

            //    if (itemObj.CARTONLABEL2 != null && itemObj.CARTONLABEL2.ToUpper() == "BULK")
            //    {
            //        isBundle = true;
            //    }
            //}
            #endregion

            return isBundle;
        }        
        public string GetJuniperPNorCustPIDByWO(OleExec SFCDB, string WO)
        {

            string output = "";
            try
            {
                O_ORDER_MAIN orderMain = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((r, o) => r.WO == o.PREWO).Where((r, o) => r.VALID == "1" && r.WO == WO)
                   .Select((r, o) => o).ToList().FirstOrDefault();
                if (orderMain == null)
                {
                    return output = "No data record";
                }
                bool isBundle = IsBundle(SFCDB, WO, orderMain.ITEMID);
                if (isBundle)
                {
                    output = orderMain.CUSTPID;
                }
                else
                {
                    var last_release = SFCDB.ORM.Queryable<R_WO_BASE, O_AGILE_ATTR>((r, o) => r.SKUNO == o.ITEM_NUMBER && r.PLANT == o.PLANT)
                        .Where((r, o) => r.WORKORDERNO == WO).OrderBy((r, o) => o.DATE_CREATED, SqlSugar.OrderByType.Desc)
                        .Select((r, o) => o).ToList().FirstOrDefault();
                    O_AGILE_ATTR aglie = last_release;
                    if (aglie == null)
                    {
                        throw new Exception($@"SKU OF {WO} NO MAPPING IN O_AGILE_ATTR!");
                    }
                    if (string.IsNullOrEmpty(aglie.CUSTPARTNO))
                    {
                        throw new Exception($@"JUNIPER PN MAPPING IN O_AGILE_ATTR IS NULL!");
                    }
                    output = aglie.CUSTPARTNO;
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }


        public string GetJuniperMasterPalletDN(OleExec SFCDB, string PALLETNO)
        {

            string output = "";
            try
            {

                string sql = $@"SELECT DISTINCT I.DELIVERYNUMBER
                                  FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN O, R_I282 I
                                 WHERE A.WORKORDERNO = O.PREWO
                                   AND O.PREASN = I.ASNNUMBER
                                   AND PALLETID = '{PALLETNO}'";
                var res = SFCDB.ORM.Ado.GetString(sql);
                if (res == null)
                {
                    output = "Not Data";
                }
                else
                {
                    output = res;
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }
        public string GetJuniperMasterPalletSO(OleExec SFCDB, string PALLETNO)
        {
            string output = "";
            try
            {
                string sql = $@"SELECT DISTINCT H.SALESORDERNUMBER
                                  FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN O, O_I137_ITEM I,O_I137_HEAD H
                                 WHERE A.WORKORDERNO = O.PREWO
                                   AND O.ITEMID = I.ID
                                   AND I.TRANID=H.TRANID
                                   AND PALLETID = '{PALLETNO}'";
                var res = SFCDB.ORM.Ado.GetString(sql);
                if (res == null)
                {
                    output = "Not Data";
                }
                else
                {
                    output = res;
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }
        public string GetJuniperMasterPalletPAGE(OleExec SFCDB, string PALLETNO)
        {

            string output = "";
            try
            {
                string sql = $@"SELECT TT.*, ROWNUM PAGE
                                  FROM (SELECT wm_concat(POLINES) POLINE,
                                               SALESORDER,
                                               wm_concat(SALESORDERLINEITEM) SOLINE,
                                               wm_concat(SKUNO) SKUNO,
                                               wm_concat(GROUPID) GROUPID,
                                               PALLETID
                                          FROM (SELECT DISTINCT O.PONO||'.'||O.POLINE POLINES,
                                                                A.SALESORDER,
                                                                I.SALESORDERLINEITEM,
                                                                A.SKUNO,
                                                                A.GROUPID,
                                                                A.PALLETID
                                                  FROM R_JUNIPER_MFPACKINGLIST A,
                                                       O_ORDER_MAIN            O,
                                                       O_I137_ITEM             I
                                                 WHERE A.WORKORDERNO = O.PREWO
                                                   AND O.ITEMID = I.ID
                                                   AND INVOICENO IN
                                                       (SELECT INVOICENO
                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                         WHERE PALLETID = '{PALLETNO}')
                                                   AND PALLETID IN
                                                       (SELECT PALLETID
                                                          FROM (SELECT DISTINCT PALLETID, SALESORDER
                                                                  FROM R_JUNIPER_MFPACKINGLIST
                                                                 WHERE INVOICENO IN
                                                                       (SELECT INVOICENO
                                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                                         WHERE PALLETID = '{PALLETNO}'))
                                                         GROUP BY PALLETID
                                                        HAVING COUNT(1) = 1)
                                                   AND A.SALESORDER IN
                                                       (SELECT SALESORDER
                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                         WHERE PALLETID = '{PALLETNO}')) T
                                         GROUP BY SALESORDER, PALLETID) TT
                                 ORDER BY SALESORDER, PALLETID";
                var res = SFCDB.ORM.Ado.GetDataTable(sql);
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    if (res.Rows[i]["PALLETID"].ToString() == PALLETNO)
                    {
                        output = res.Rows[i]["PAGE"].ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }
        public string GetJuniperMasterPalletALLPAGE(OleExec SFCDB, string PALLETNO)
        {
            string output = "";
            try
            {
                string sql = $@"SELECT TT.*, ROWNUM PAGE
                                  FROM (SELECT wm_concat(POLINES) POLINE,
                                               SALESORDER,
                                               wm_concat(SALESORDERLINEITEM) SOLINE,
                                               wm_concat(SKUNO) SKUNO,
                                               wm_concat(GROUPID) GROUPID,
                                               PALLETID
                                          FROM (SELECT DISTINCT O.PONO||'.'||O.POLINE POLINES,
                                                                A.SALESORDER,
                                                                I.SALESORDERLINEITEM,
                                                                A.SKUNO,
                                                                A.GROUPID,
                                                                A.PALLETID
                                                  FROM R_JUNIPER_MFPACKINGLIST A,
                                                       O_ORDER_MAIN            O,
                                                       O_I137_ITEM             I
                                                 WHERE A.WORKORDERNO = O.PREWO
                                                   AND O.ITEMID = I.ID
                                                   AND INVOICENO IN
                                                       (SELECT INVOICENO
                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                         WHERE PALLETID = '{PALLETNO}')
                                                   AND PALLETID IN
                                                       (SELECT PALLETID
                                                          FROM (SELECT DISTINCT PALLETID, SALESORDER
                                                                  FROM R_JUNIPER_MFPACKINGLIST
                                                                 WHERE INVOICENO IN
                                                                       (SELECT INVOICENO
                                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                                         WHERE PALLETID = '{PALLETNO}'))
                                                         GROUP BY PALLETID
                                                        HAVING COUNT(1) = 1)
                                                   AND A.SALESORDER IN
                                                       (SELECT SALESORDER
                                                          FROM R_JUNIPER_MFPACKINGLIST
                                                         WHERE PALLETID = '{PALLETNO}')) T
                                         GROUP BY SALESORDER, PALLETID) TT
                                 ORDER BY SALESORDER, PALLETID";
                var res = SFCDB.ORM.Ado.GetDataTable(sql);
                output = res.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return output;
        }
        public List<string> GetJuniperMasterPalletSOLineList(OleExec SFCDB, string PALLETNO)
        {
            List<string> output = new List<string>();
            try
            {
                string sql = $@"SELECT H.SALESORDERNUMBER, I.SALESORDERLINEITEM, A.SKUNO, SUM(A.QUANTITY) QTY
                                  FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN O, O_I137_ITEM I,O_I137_HEAD H
                                 WHERE A.WORKORDERNO = O.PREWO
                                   AND O.ITEMID = I.ID
                                   AND I.TRANID = H.TRANID
                                   AND PALLETID = '{PALLETNO}'
                                 GROUP BY H.SALESORDERNUMBER, I.SALESORDERLINEITEM, A.SKUNO
                                 order by H.SALESORDERNUMBER, I.SALESORDERLINEITEM, A.SKUNO,QTY";
                var res = SFCDB.ORM.Ado.GetDataTable(sql);
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    var soline = res.Rows[i]["SALESORDERNUMBER"].ToString() + "." + res.Rows[i]["SALESORDERLINEITEM"].ToString();
                    output.Add(soline);
                }
            }
            catch
            {
            }
            return output;
        }
        public List<string> GetJuniperMasterPalletSKUList(OleExec SFCDB, string PALLETNO)
        {
            List<string> output = new List<string>();
            try
            {
                string sql = $@"SELECT A.SALESORDER, I.SALESORDERLINEITEM, A.SKUNO, SUM(A.QUANTITY) QTY
                                  FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN O, O_I137_ITEM I
                                 WHERE A.WORKORDERNO = O.PREWO
                                   AND O.ITEMID = I.ID
                                   AND PALLETID = '{PALLETNO}'
                                 GROUP BY A.SALESORDER, I.SALESORDERLINEITEM, A.SKUNO
                                 order by A.SALESORDER, I.SALESORDERLINEITEM,A.SKUNO,QTY";
                var res = SFCDB.ORM.Ado.GetDataTable(sql);
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    var soline = res.Rows[i]["SKUNO"].ToString();
                    output.Add(soline);
                }
            }
            catch
            {
            }
            return output;
        }
        public List<string> GetJuniperMasterPalletQTYList(OleExec SFCDB, string PALLETNO)
        {
            List<string> output = new List<string>();
            try
            {
                string sql = $@"SELECT A.SALESORDER, I.SALESORDERLINEITEM, A.SKUNO, SUM(A.QUANTITY) QTY
                                  FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN O, O_I137_ITEM I
                                 WHERE A.WORKORDERNO = O.PREWO
                                   AND O.ITEMID = I.ID
                                   AND PALLETID = '{PALLETNO}'
                                 GROUP BY A.SALESORDER, I.SALESORDERLINEITEM, A.SKUNO
                                 order by A.SALESORDER, I.SALESORDERLINEITEM,A.SKUNO,QTY";
                var res = SFCDB.ORM.Ado.GetDataTable(sql);
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    var soline = res.Rows[i]["QTY"].ToString();
                    output.Add(soline);
                }
            }
            catch
            {
            }
            return output;
        }
        public string GetJuniperIsLinecardBundle(OleExec SFCDB, string WO)
        {
            var output = "";
            var itemObj = SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((O, I, H) => new object[] {
                SqlSugar.JoinType.Left,O.ITEMID == I.ID,
                SqlSugar.JoinType.Left, I.TRANID == H.TRANID
            })
               .Where((O, I, H) => O.PREWO == WO)
               .Select((O, I, H) => new { I, H })
               .First();
            if (itemObj == null)
            {
                throw new Exception($@"WO:{WO} not in o_i137_item!");
            }
            var bndlorder = SFCDB.ORM.Queryable<O_ORDER_MAIN, C_SKU, C_SERIES, O_I137_ITEM, O_I137_HEAD>
                ((O, S, C, I, H) => O.PID == S.SKUNO && S.C_SERIES_ID == C.ID && O.ITEMID == I.ID && I.TRANID == H.TRANID)
                .Where((O, S, C, I, H) => C.SERIES_NAME == "Juniper-FRU" && H.SALESORDERNUMBER == itemObj.H.SALESORDERNUMBER && I.SOID == itemObj.I.SOID && I.SOID != "000000")
                .Select((O, S, C, I, H) => O)
                .First();

            var bndlorders = SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>
                ((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
                .Where((O, I, H) => H.SALESORDERNUMBER == itemObj.H.SALESORDERNUMBER && I.SOID == itemObj.I.SOID)
                .Select((O, I, H) => O)
                .ToList();
            if (bndlorder != null && bndlorders.Count > 1)
            {
                output = "Y";
            }
            else
            {
                output = "N";
            }
            return output;
        }


        public string GetJuniperBundleSOLine(OleExec SFCDB, string SN)
        {
            var res = "";
            var bd = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.SN == SN && t.VALID_FLAG == "1")
                .First();
            res = bd.SALEORDER.TrimStart('0') + "." + bd.SOID.TrimStart('0');
            return res;
        }
        public List<string> GetJuniperBundleSN(OleExec SFCDB, string SN)
        {
            var res = new List<string>();
            var bd = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.SN == SN && t.VALID_FLAG == "1")
                .First();
            var bds = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.BNDL_NO == bd.BNDL_NO && t.VALID_FLAG == "1")
                .OrderBy(t => t.SN, SqlSugar.OrderByType.Asc)
                .ToList();
            for (int i = 0; i < bds.Count; i++)
            {
                res.Add(bds[i].SN);
            }
            return res;
        }
        public List<string> GetJuniperBundlePN(OleExec SFCDB, string SN)
        {
            var res = new List<string>();
            var bd = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.SN == SN && t.VALID_FLAG == "1")
                .First();
            var bds = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.BNDL_NO == bd.BNDL_NO && t.VALID_FLAG == "1")
                .OrderBy(t => t.SN, SqlSugar.OrderByType.Asc)
                .ToList();
            for (int i = 0; i < bds.Count; i++)
            {
                res.Add(bds[i].SKUNO);
            }
            return res;
        }
        public List<string> GetJuniperBundleCID(OleExec SFCDB, string SN)
        {
            var res = new List<string>();
            var bd = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.SN == SN && t.VALID_FLAG == "1")
                .First();
            var obj = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.BNDL_NO == bd.BNDL_NO && t.VALID_FLAG == "1");
            var idlist = obj.Select(t => t.R_SN_ID).ToList();
            var snlist = obj.OrderBy(t => t.SN, SqlSugar.OrderByType.Asc).Select(t => t.SN).ToList();

            var item = SFCDB.ORM.Queryable<R_SN, R_SN_KP, O_ORDER_MAIN, O_I137_ITEM, O_I137_DETAIL>((S, K, O, I, D) =>
            new object[]
            {
                SqlSugar.JoinType.Left,S.ID == K.R_SN_ID,
                SqlSugar.JoinType.Left,S.WORKORDERNO == O.PREWO,
                SqlSugar.JoinType.Left,O.ITEMID == I.ID,
                SqlSugar.JoinType.Left,I.TRANID == D.TRANID,
                SqlSugar.JoinType.Left,D.COMPONENTID == K.PARTNO
            })
                .Where((S, K, O, I, D) => K.SN == K.VALUE && K.VALID_FLAG == 1 && S.VALID_FLAG == "1" && S.SN == SN)
                .OrderBy((S, K, O, I, D) => S.SN, SqlSugar.OrderByType.Asc)
                .Select((S, K, O, I, D) => new { S.SN, D.COMCUSTPRODID })
                .ToList();

            for (int i = 0; i < snlist.Count; i++)
            {
                var temp = item.Find(t => t.SN == snlist[i]);
                if (temp == null)
                {
                    res.Add("");
                }
                else if (string.IsNullOrEmpty(temp.COMCUSTPRODID))
                {
                    res.Add("");
                }
                else
                {
                    res.Add(temp.COMCUSTPRODID);
                }
            }
            return res;
        }
        public List<string> GetJuniperBundleCLEI(OleExec SFCDB, string SN)
        {
            var res = new List<string>();
            var bd = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>()
                .Where(t => t.SN == SN && t.VALID_FLAG == "1")
                .First();
            var bndldata = SFCDB.ORM.Queryable<R_JUNIPER_BUNDLE>().Where(t => t.BNDL_NO == bd.BNDL_NO && t.VALID_FLAG == "1").ToList();
            var soinfoList = SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((O, I, H) => O.ITEMID == I.ID && I.TRANID == H.TRANID)
            .Where((O, I, H) => I.SOID == bd.SOID && H.SALESORDERNUMBER == bd.SALEORDER)
            .Select((O, I, H) => new { O, I, H })
            .Distinct()
            .ToList();
            var snid = bndldata.Select(t => t.R_SN_ID).ToList();
            var snkps = SFCDB.ORM.Queryable<R_SN_KP>()
                .Where(t => snid.Contains(t.R_SN_ID) && t.KP_NAME == "AutoKP" && t.SN == t.VALUE && t.VALID_FLAG == 1)
                .OrderBy(t => t.SN, SqlSugar.OrderByType.Asc)
                .ToList();
            for (int i = 0; i < snkps.Count; i++)
            {
                if (string.IsNullOrEmpty(snkps[i].EXVALUE2))
                {
                    res.Add("");
                }
                else
                {
                    res.Add(snkps[i].EXVALUE2);
                }
            }
            return res;
        }

    }
}
