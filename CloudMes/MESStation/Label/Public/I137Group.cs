using MESDataObject.Module.Juniper;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module.OM;
using MESDataObject.Module;

namespace MESStation.Label.Public
{
    public class I137Group : LabelValueGroup
    {
        public I137Group()
        {
            ConfigGroup = "I137Group";
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetTranIdByWO",
                    Description = "Get Tran Id From I137_ITEM By WO.",
                    Paras = new List<string>() { "WO" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetItemIdByWO",
                    Description = "Get Item Id From I137_ITEM By WO.",
                    Paras = new List<string>() { "WO" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetI137ItemValueByUPOID",
                    Description = "Get Column Value From O_I137_ITEM By UPOID",
                    Paras = new List<string>() { "UPOID", "ColumnName" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetOI137ValueByID",
                    Description = "Get Column Value From O_I137_ITEM By TranId",
                    Paras = new List<string>() { "ID", "ColumnName" }
                });

            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetNotPrintFlagByUPOID",
                    Description = "Get Not Print Flag From O_I137_ITEM By ColumnName",
                    Paras = new List<string>() { "UPOID", "ColumnName", "PrintType" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetNotPrintFlagByID",
                    Description = "Get Not Print Flag From O_I137_ITEM By ColumnName",
                    Paras = new List<string>() { "ID", "ColumnName", "PrintType" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetSalesOrderNumber",
                    Description = "Get Sales Order Number From O_I137_HEAD,No Leading Zeros.",
                    Paras = new List<string>() { "ITEM_ID" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetTCIFParentSalesOrderNumber",
                    Description = "Get TCIF Parent Sales Order Number From O_I137_HEAD,No Leading Zeros.",
                    Paras = new List<string>() { "WO" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetShippingLabelSO",
                    Description = "Get Shipping Label Sales Order Number From O_I137_HEAD,No Leading Zeros.",
                    Paras = new List<string>() { "TranId" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetOI137HeadValueByTranId",
                    Description = "Get Column Value From O_I137_HEAD By TranId",
                    Paras = new List<string>() { "TranId", "ColumnName" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetShippLabelLine2",
                    Description = "Get Shipping Label Line2",
                    Paras = new List<string>() { "TranId" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetOI137ItemValueById",
                    Description = "",
                    Paras = new List<string>() { "Id", "ColumnName" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetTCIFParentBundleSalesOrderNumber",
                    Description = "",
                    Paras = new List<string>() { "WO" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetRegionFromI137H",
                    Description = "",
                    Paras = new List<string>() { "WO" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetRegionFromI282",
                    Description = "",
                    Paras = new List<string>() { "DN" }
                });
        }
        public string GetTranIdByWO(OleExec SFCDB, string WO)
        {
            string output = "";
            output = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN, I137_I>((rw, oom, oii) => rw.ORIGINALID == oom.ID && oom.ITEMID == oii.ID)
                .Where((rw, oom, oii) => rw.WO == WO && rw.VALID == "1").Select<string>((rw, oom, oii) => oii.TRANID).ToList().FirstOrDefault();
            if (string.IsNullOrEmpty(output))
            {
                throw new Exception("Get WO Info Fail!R_ORDER_WO,O_ORDER_MAIN,I137_I");
            }
            return output;
        }
        public string GetItemIdByWO(OleExec SFCDB, string WO)
        {
            string output = "";
            output = SFCDB.ORM.Queryable<R_ORDER_WO, O_ORDER_MAIN>((rw, oom) => rw.ORIGINALID == oom.ID)
                .Where((rw, oom) => rw.WO == WO && rw.VALID == "1").Select<string>((rw, oom) => oom.ITEMID).ToList().FirstOrDefault();
            if (string.IsNullOrEmpty(output))
            {
                throw new Exception($@"Get WO Info Fail!R_ORDER_WO,O_ORDER_MAIN(WO={WO})");
            }
            return output;
        }

        /// <summary>
        /// 獲取O_I137_ITEM對應欄位的值,沒有該欄位則拋異常
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="tranid"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetI137ItemValueByUPOID(OleExec SFCDB, string UPOID, string ColumnName)
        {
            string output = "";
            List<string> listValue = SFCDB.ORM.Queryable<I137_I>().Where(r => r.UPOID == UPOID).Select<string>(ColumnName.ToUpper()).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            output = listValue[0];
            return output;
        }

        /// <summary>
        /// 獲取O_I137_ITEM對應欄位的值,沒有該欄位則拋異常
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="tranid"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetOI137ValueByID(OleExec SFCDB, string ID, string ColumnName)
        {
            string output = "";
            List<string> listValue = SFCDB.ORM.Queryable<I137_I>().Where(r => r.ID == ID).Select<string>(ColumnName.ToUpper()).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            switch (ColumnName.ToUpper())
            {
                case "CUSTPRODID":
                    //Juniper_Overpack_Label1,Juniper_TCIF_Parent_Label如果CUSTPRODID=NA則返回空值以便顯示空白
                    output = listValue[0] == "NA" ? "" : listValue[0];
                    break;
                default:
                    output = listValue[0];
                    break;
            }
            return output;
        }
        /// <summary>
        /// 默認返回值為TRUE，即不打印該LABEL
        /// 傳入的PrintType等於O_I137_ITEM中ColumnName欄位的值則進行打印
        /// 目前Juniper 的China Label，KCC Lable 需要通過判斷COUNTRYSPECIFICLABEL欄位的值來管控是否打印
        /// 目前Juniper 的EACU Lable 需要通過判斷CARTONLABEL1欄位的值來管控是否打印
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="ITEM"></param>
        /// <param name="tranid"></param>
        /// <returns></returns>
        public string GetNotPrintFlagByUPOID(OleExec SFCDB, string ColumnName, string PrintType, string UPOID)
        {
            string output = "TRUE";
            List<string> listValue = SFCDB.ORM.Queryable<I137_I>().Where(r => r.UPOID == UPOID).Select<string>(ColumnName.ToUpper()).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            if (listValue[0].ToUpper().Equals(PrintType.ToUpper().Trim()))
            {
                output = "FALSE";
            }
            return output;
        }

        /// <summary>
        /// 默認返回值為TRUE，即不打印該LABEL
        /// 傳入的PrintType等於O_I137_ITEM中ColumnName欄位的值則進行打印
        /// 目前Juniper 的China Label，KCC Lable 需要通過判斷COUNTRYSPECIFICLABEL欄位的值來管控是否打印
        /// 目前Juniper 的EACU Lable 需要通過判斷CARTONLABEL1欄位的值來管控是否打印
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="ITEM"></param>
        /// <param name="tranid"></param>
        /// <returns></returns>
        public string GetNotPrintFlagByID(OleExec SFCDB, string ID, string ColumnName, string PrintType)
        {
            string output = "TRUE";
            List<string> listValue = SFCDB.ORM.Queryable<I137_I>().Where(r => r.ID == ID).Select<string>(ColumnName.ToUpper()).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            if (!string.IsNullOrEmpty(listValue[0]) && listValue[0].ToUpper().Equals(PrintType.ToUpper().Trim()))
            {
                output = "FALSE";
            }
            return output;
        }

        /// <summary>
        /// Juniper獲取Sales Order Number
        /// 注意，該方法傳回的值是已經去掉前面的0,且帶有前面去掉0的SALESORDERLINEITEM
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="tranid"></param>
        /// <returns></returns>
        public string GetSalesOrderNumber(OleExec SFCDB, string ITEM_ID)
        {
            string output = "";
            I137_I i137 = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == ITEM_ID).ToList().FirstOrDefault();
            if (i137 == null)
            {
                throw new Exception($@"{ITEM_ID} ITEM_ID i137 Data");
            }
            I137_H i137head = SFCDB.ORM.Queryable<I137_H>().Where(r => r.TRANID == i137.TRANID).ToList().FirstOrDefault();
            if (i137head == null)
            {
                throw new Exception("I137 Head Error!");
            }
            output = $@"{ i137head.SALESORDERNUMBER.TrimStart('0')}.{i137.SALESORDERLINEITEM.TrimStart('0')}";
            return output;
        }

        public string GetTCIFParentBundleSalesOrderNumber(OleExec SFCDB, string WO)
        {
            string output = "";

            var oMain = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO>((o, r) => o.PREWO == r.WO)
                .Where((o, r) => r.WO == WO && r.VALID == "1").Select((o, r) => o).ToList().FirstOrDefault();
            if (oMain == null)
            {
                throw new Exception($@"{WO} Error!O_ORDER_MAIN");
            }
            I137_I i137 = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == oMain.ITEMID).ToList().FirstOrDefault();
            if (i137 == null)
            {
                throw new Exception($@"{WO} No i137 Data");
            }
            I137_H i137head = SFCDB.ORM.Queryable<I137_H>().Where(r => r.TRANID == i137.TRANID).ToList().FirstOrDefault();
            if (i137head == null)
            {
                throw new Exception("I137 Head Error!");
            }
            var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                    .Where((C, S, W) => W.WORKORDERNO == WO)
                    .Select((C, S, W) => C)
                    .First();

            JuniperGroup juniperLabelGroup = new JuniperGroup();
            bool isBundle = juniperLabelGroup.IsBundle(SFCDB, oMain.PREWO, oMain.ITEMID);
            output = $@"{ i137head.SALESORDERNUMBER.TrimStart('0')}.{i137.SOID.TrimStart('0')}";

            return output;
        }

        public string GetTCIFParentSalesOrderNumber(OleExec SFCDB, string WO)
        {
            string output = "";

            var oMain = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO>((o, r) => o.PREWO == r.WO)
                .Where((o, r) => r.WO == WO && r.VALID == "1").Select((o, r) => o).ToList().FirstOrDefault();
            if (oMain == null)
            {
                throw new Exception($@"{WO} Error!O_ORDER_MAIN");
            }
            I137_I i137 = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == oMain.ITEMID).ToList().FirstOrDefault();
            if (i137 == null)
            {
                throw new Exception($@"{WO} No i137 Data");
            }
            I137_H i137head = SFCDB.ORM.Queryable<I137_H>().Where(r => r.TRANID == i137.TRANID).ToList().FirstOrDefault();
            if (i137head == null)
            {
                throw new Exception("I137 Head Error!");
            }
            var cs = SFCDB.ORM.Queryable<C_SERIES, C_SKU, R_WO_BASE>((C, S, W) => C.ID == S.C_SERIES_ID && S.SKUNO == W.SKUNO)
                    .Where((C, S, W) => W.WORKORDERNO == WO)
                    .Select((C, S, W) => C)
                    .First();

            JuniperGroup juniperLabelGroup = new JuniperGroup();
            bool isBundle = juniperLabelGroup.IsBundle(SFCDB, oMain.PREWO, oMain.ITEMID);
            if (isBundle && cs.SERIES_NAME == "Juniper-Optics")
            {
                //BNDL SALES ORDER LINE ITEM 取SOID
                if (string.IsNullOrEmpty(i137.SOID))
                {
                    throw new Exception($@"{WO} is BNDL but No SOID");
                }
                output = $@"{ i137head.SALESORDERNUMBER.TrimStart('0')}.{i137.SOID.TrimStart('0')}";
            }
            else
            {
                output = $@"{ i137head.SALESORDERNUMBER.TrimStart('0')}.{i137.SALESORDERLINEITEM.TrimStart('0')}";
            }
            return output;
        }

        /// <summary>
        /// Juniper獲取Sales Order Number
        /// 注意，該方法傳回的值是已經去掉前面的0,不帶有SALESORDERLINEITEM
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="tranid"></param>
        /// <returns></returns>
        public string GetShippingLabelSO(OleExec SFCDB, string TranId)
        {
            string output = "";
            I137_H i137head = SFCDB.ORM.Queryable<I137_H>().Where(ih => ih.TRANID == TranId).ToList().FirstOrDefault();
            if (i137head == null)
            {
                throw new Exception("I137 Head Error!");
            }
            //var i137item = SFCDB.ORM.Queryable<I137_I>()
            //    .Where(i=> i.TRANID == TranId && i.PONUMBER == PO&&i.ITEM== POITEM).ToList().FirstOrDefault();
            //if (i137item == null)
            //{
            //    throw new Exception("I137 Item Error!");
            //}
            output = $@"{ i137head.SALESORDERNUMBER.TrimStart('0')}";
            return output;
        }
        /// <summary>
        /// 獲取O_I137_HEAD對應欄位的值,沒有該欄位則拋異常
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="tranid"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetOI137HeadValueByTranId(OleExec SFCDB, string TranId, string ColumnName)
        {
            string output = "";
            List<string> listValue = SFCDB.ORM.Queryable<I137_H>().Where(r => r.TRANID == TranId).Select<string>(ColumnName.ToUpper()).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            switch (ColumnName.ToUpper())
            {
                case "SHIPTOCONTACTPHONE":
                    //Juniper_Shipping_Label如果SHIPTOCONTACTPHONE=NA則返回空值以便顯示空白
                    output = listValue[0] == "NA" ? "" : listValue[0];
                    break;
                default:
                    output = listValue[0];
                    break;
            }
            return output;
        }


        /// <summary>
        /// 獲取O_I137_HEAD對應欄位的值,沒有該欄位則拋異常
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="Id"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetOI137ItemValueById(OleExec SFCDB, string Id, string ColumnName)
        {
            string output = "";
            List<string> listValue = SFCDB.ORM.Queryable<I137_I>().Where(r => r.ID == Id).Select<string>(ColumnName.ToUpper()).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            output = listValue[0];

            return output;
        }
        /// <summary>
        /// Shipping Label Line2
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="TranId"></param>
        /// <returns></returns>
        public string GetShippLabelLine2(OleExec SFCDB, string TranId)
        {
            string out_put = "TRUE";
            O_I137_HEAD o = SFCDB.ORM.Queryable<O_I137_HEAD>().Where(t => t.TRANID == TranId).ToList().FirstOrDefault();
            if (o == null)
            {
                throw new Exception("Get Address Fail!O_I137_HEAD");
            }
            if (string.IsNullOrEmpty(o.SHIPTOSTREETNAME))
            {
                //空值多加了個空格
                if (string.IsNullOrEmpty(o.SHIPTOREGIONCODE))
                {
                    out_put = o.SHIPTOCITYNAME + "  " + o.SHIPTOSTREETPOSTALCODE + "\n" + o.SHIPTOCOUNTRYCODE;
                }
                else
                {
                    out_put = o.SHIPTOCITYNAME + "  " + o.SHIPTOREGIONCODE + "  " + o.SHIPTOSTREETPOSTALCODE + "\n" + o.SHIPTOCOUNTRYCODE;
                }
            }
            else
            {
                //空值多加了個空格
                if (string.IsNullOrEmpty(o.SHIPTOREGIONCODE))
                {
                    out_put = o.SHIPTOSTREETNAME + "\n" + o.SHIPTOCITYNAME + "  " + o.SHIPTOSTREETPOSTALCODE + "\n" + o.SHIPTOCOUNTRYCODE;
                }
                else
                {
                    out_put = o.SHIPTOSTREETNAME + "\n" + o.SHIPTOCITYNAME + "  " + o.SHIPTOREGIONCODE + "  " + o.SHIPTOSTREETPOSTALCODE + "\n" + o.SHIPTOCOUNTRYCODE;
                }

            }
            //
            return out_put;
        }

        public string GetShippingLabelSoldBy(OleExec SFCDB, string TranId)
        {
            string sold_by = "";
            O_I137_HEAD ohead = SFCDB.ORM.Queryable<O_I137_HEAD>().Where(r => r.TRANID == TranId).ToList().FirstOrDefault();
            if (ohead == null)
            {
                throw new Exception($@"TRANID[{TranId}] Not Exist O_I137_HEAD");
            }
            switch (ohead.POBILLTOCOMPANY)
            {
                case "1000":
                case "1010":
                    sold_by = "Juniper Networks Inc.\n1133 Innovation Way\nSunnyvale, CA 94089\nUSA";
                    break;
                case "3230":
                    string ShipToCountry = ohead.SHIPTOCOUNTRYCODE;
                    if (ShipToCountry != "AU" && ShipToCountry != "IN" && ShipToCountry != "UK/GB")
                    {
                        sold_by = "Juniper Networks International B.V.\nBoeing Avenue 240\n1119 PZ Schiphol-Rijk\nAmsterdam\nNetherlands";
                    }
                    else if (ShipToCountry == "UK" || ShipToCountry == "GB")
                    {
                        sold_by = "Juniper Networks (UK) Limited\nBuilding 1, Aviator Park, Station Rd\nAddlestone KT15 2PG\nUK";
                    }
                    else if (ShipToCountry == "AU")
                    {
                        sold_by = "Juniper Networks Australia Pty Limited\nLevel 26/55 Collins Street\nMelbourne VIC 3000\nAustralia";
                    }
                    else if (ShipToCountry == "IN")
                    {
                        sold_by = "Juniper Networks Solution India Private Limited\nSurvey No.111/1 to 115/4, Wing A &B\nAmane Belandur Khane Village\n2nd Floor, Elnath-Exora Business Park\nMarathahalli, Sarjapur Outer Ring Road, Bangalore -560 -103";
                    }
                    else
                    {
                        throw new Exception($@"3230 {ShipToCountry} Not Setting!");
                    }
                    break;
                case "3011":
                    sold_by = "Juniper Networks (UK) Limited\nBuilding 1, Aviator Park, Station Rd\nAddlestone KT15 2PG\nUK";
                    break;
                case "2810":
                    sold_by = "Juniper Networks Australia Pty Limited\nLevel 26/55 Collins Street\nMelbourne VIC 3000\nAustralia";
                    break;
                case "2380":
                    sold_by = "Juniper Networks Solution India Private Limited\nSurvey No.111/1 to 115/4, Wing A &B\nAmane Belandur Khane Village\n2nd Floor, Elnath-Exora Business Park\nMarathahalli, Sarjapur Outer Ring Road, Bangalore -560 -103";
                    break;
                default:
                    throw new Exception("PO BILL TO COMPANY ERROR!");
            }
            return sold_by;
        }
        public string GetShippingLabelBillTo(OleExec SFCDB, string TranId)
        {
            string bill_to = "";
            O_I137_HEAD ohead = SFCDB.ORM.Queryable<O_I137_HEAD>().Where(r => r.TRANID == TranId).ToList().FirstOrDefault();
            if (ohead == null)
            {
                throw new Exception($@"TRANID[{TranId}] Not Exist O_I137_HEAD");
            }

            //select
            //    BILLTOCOMPANYNAME,
            //    BILLTOID,--Customer Number Line 1
            //    BILLTOCOMPANY,--Customer Number Line 2
            //    BILLTOCOUNTRYCODE,--國家代碼 Country Line 6
            //    BILLTOREGIONCODE,--Line 5
            //    BILLTOSTREETPOSTALCODE,--Line 5
            //    BILLTOCITYNAME,--Line 5
            //    BILLTOSTREETNAME,--Line 4
            //    BILLTOHOUSEID,--Line 3
            //    BILLTOPERSONNAME
            //    from o_i137_head;
            string line1 = ohead.BILLTOID;
            string line2 = ohead.BILLTOCOMPANY;
            string line3 = ohead.BILLTOHOUSEID;
            string line4 = ohead.BILLTOSTREETNAME;
            string city = ohead.BILLTOSTREETNAME;
            string region_code = ohead.BILLTOREGIONCODE == "NA" ? "" : ohead.BILLTOREGIONCODE;//區域代碼
            string postal_code = ohead.BILLTOSTREETPOSTALCODE;//郵政編碼
            string line5 = $@"{city}  {region_code}  {postal_code}";
            string line6 = ohead.BILLTOCOUNTRYCODE;
            if (!string.IsNullOrEmpty(line4))
            {
                bill_to = $@"{line1}\n{line2}\n{line3}\n{line4}\n{line5}\n{line6}";
            }
            else
            {
                bill_to = $@"{line1}\n{line2}\n{line3}\n{line5}\n{line6}";
            }
            return bill_to;
        }

        public string GetShippingLabelShipTo(OleExec SFCDB, string TranId)
        {
            string bill_to = "";
            O_I137_HEAD ohead = SFCDB.ORM.Queryable<O_I137_HEAD>().Where(r => r.TRANID == TranId).ToList().FirstOrDefault();
            if (ohead == null)
            {
                throw new Exception($@"TRANID[{TranId}] Not Exist O_I137_HEAD");
            }
            //select
            //    SHIPTOFAXL,--傳真
            //    SHIPTOEMAILURI,--郵件
            //    SHIPTODEVIATINGFULLNAME,--暫時不管
            //    SHIPTOCONTACTPHONE,--暫時不管
            //    SHIPTOHOUSEID,--Line 3
            //    SHIPTOSTREETNAME,--Line 4
            //    SHIPTOCITYNAME,--Line 5
            //    SHIPTOSTREETPOSTALCODE,--Line 5
            //    SHIPTOREGIONCODE,--Line 5
            //    SHIPTOCOUNTRYCODE,--國家代碼 Country Line 6
            //    SHIPTOCOMPANY,--公司名稱 Customer Name Line 2
            //    SHIPTOID--Customer Number Line 1
            //    from o_i137_head;
            string line1 = ohead.SHIPTOID;
            string line2 = ohead.SHIPTOCOMPANY;
            string line3 = ohead.SHIPTOHOUSEID;
            string line4 = ohead.SHIPTOSTREETNAME;
            string city = ohead.SHIPTOCITYNAME;
            string region_code = ohead.SHIPTOREGIONCODE == "NA" ? "" : ohead.SHIPTOREGIONCODE;//區域代碼
            string postal_code = ohead.SHIPTOSTREETPOSTALCODE;//郵政編碼
            string line5 = $@"{city}  {region_code}  {postal_code}";
            string line6 = ohead.SHIPTOCOUNTRYCODE;
            if (!string.IsNullOrEmpty(line4))
            {
                bill_to = $@"{line1}\n{line2}\n{line3}\n{line4}\n{line5}\n{line6}";
            }
            else
            {
                bill_to = $@"{line1}\n{line2}\n{line3}\n{line5}\n{line6}";
            }
            return bill_to;
        }

        public string GetRegionFromI137H(OleExec SFCDB, string WO)
        {
            var ohead = SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((O, I, H) => new object[]
            {
                SqlSugar.JoinType.Left, O.ITEMID == I.ID,
                SqlSugar.JoinType.Left, I.TRANID == H.TRANID
            }).Where((O, I, H) => O.PREWO == WO)
            .Select((O, I, H) => H)
            .First();
            if (ohead == null)
            {
                throw new Exception($@"WO[{WO}] Not Exist O_I137_HEAD");
            }
            if (string.IsNullOrEmpty(ohead.SHIPTOCOUNTRYCODE))
            {
                throw new Exception($@"Fail to get SHIPTOCOUNTRYCODE from i137 head");
            }
            var rgm = SFCDB.ORM.Queryable<R_GEOGRAPHIES_MAP>().Where(t => t.COUNTRYCODE == ohead.SHIPTOCOUNTRYCODE).First();
            if (rgm == null)
            {
                throw new Exception($@"miss [{ohead.SHIPTOCOUNTRYCODE}] geographies mapping");
            }
            return rgm.REGION1;
        }

        public string GetRegionFromI282(OleExec SFCDB, string DN)
        {
            var i282 = SFCDB.ORM.Queryable<R_I282>().Where(t => t.DELIVERYNUMBER.EndsWith(DN)).First();
            if (i282 == null)
            {
                throw new Exception($@"DN[{DN}] Not Exist I282");
            }
            if (string.IsNullOrEmpty(i282.SHIPTOPARTYCOUNTRY))
            {
                throw new Exception($@"Fail to get SHIPTOCOUNTRYCODE from i137 head");
            }
            var rgm = SFCDB.ORM.Queryable<R_GEOGRAPHIES_MAP>().Where(t => t.COUNTRYCODE == i282.SHIPTOPARTYCOUNTRY).First();
            if (rgm == null)
            {
                throw new Exception($@"miss [{i282.SHIPTOPARTYCOUNTRY}] geographies mapping");
            }
            return rgm.REGION1;
        }
    }
}
