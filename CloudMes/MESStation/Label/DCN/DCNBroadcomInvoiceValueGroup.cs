using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation.Label;
using MESStation.Config.DCN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;


namespace MESStation.Label.DCN
{
    class DCNBroadcomInvoiceValueGroup : LabelValueGroup
    {
        public DCNBroadcomInvoiceValueGroup()
        {
            ConfigGroup = "DCNBroadcomInvoiceValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJSON_CUST_PO_DATA", Description = "Get Json format CUST_PO_DATA by DN", Paras = new List<string>() { "DN_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_Orderno_line", Description = "Get_orderno_line", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_Orderno", Description = "SALES_ORDER_NUMBER", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_SHIP_TIME", Description = "Get_SHIP_TIME", Paras = new List<string>() { "SALES_ORDER_NUMBER", "solineno" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_COMPANY_NAME", Description = "SELLING_ENTITY_COMPANY_NAME", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_ADDRESS1", Description = "SELLING_ENTITY_ADDRESS1", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_ADDRESS2", Description = "SELLING_ENTITY_ADDRESS2", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_ADDRESS3", Description = "SELLING_ENTITY_ADDRESS3", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_ADDRESS4", Description = "SELLING_ENTITY_ADDRESS4", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_CITY", Description = "SELLING_ENTITY_CITY", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_STATE", Description = "SELLING_ENTITY_STATE", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_POSTAL_CODE", Description = "SELLING_ENTITY_POSTAL_CODE", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SELLING_ENTITY_COUNTRY", Description = "SELLING_ENTITY_COUNTRY", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_COMPANY_NAME", Description = "SHIP_TO_COMPANY_NAME", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_ADDRESS1", Description = "SHIP_TO_ADDRESS1", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_ADDRESS2", Description = "SHIP_TO_ADDRESS2", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_ADDRESS3", Description = "SHIP_TO_ADDRESS3", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_ADDRESS4", Description = "SHIP_TO_ADDRESS4", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_CITY", Description = "SHIP_TO_CITY", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_STATE", Description = "SHIP_TO_STATE", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_POSTAL_CODE", Description = "SHIP_TO_POSTAL_CODE", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_COUNTRY", Description = "SHIP_TO_COUNTRY", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_COMPANY_NAME", Description = "BILL_TO_COMPANY_NAME", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_ADDRESS1", Description = "BILL_TO_ADDRESS1", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_ADDRESS2", Description = "BILL_TO_ADDRESS2", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_ADDRESS3", Description = "BILL_TO_ADDRESS3", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_ADDRESS4", Description = "BILL_TO_ADDRESS4", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_CITY", Description = "BILL_TO_CITY", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_STATE", Description = "BILL_TO_STATE", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_POSTAL_CODE", Description = "BILL_TO_POSTAL_CODE", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "BILL_TO_COUNTRY", Description = "BILL_TO_COUNTRY", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "CUSTOMER_PO_NUMBER", Description = "CUSTOMER_PO_NUMBER", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIPPING_METHOD", Description = "SHIPPING_METHOD", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIPPING_NOTE", Description = "SHIPPING_NOTE", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SPECIAL_INSTRUCTION", Description = "SPECIAL_INSTRUCTION", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "INCO_TERM", Description = "INCO_TERM", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "US_ECCN", Description = "US_ECCN", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SHIP_TO_HTS", Description = "SHIP_TO_HTS", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "US_LICENSE", Description = "US_LICENSE", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "ORDERED_QUANTITY", Description = "ORDERED_QUANTITY", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_UOM", Description = "Get_UOM", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "UNIT_PRICE", Description = "UNIT_PRICE", Paras = new List<string>() { "JSON_PO"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPalletByDn", Description = "獲取棧板", Paras = new List<string>() { "DN_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDnByPallet", Description = "獲取DN", Paras = new List<string>() { "PACKNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Itemdesc", Description = "Itemdesc", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "CUSTOMER_ITEM", Description = "CUSTOMER_ITEM", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPALLET_NET", Description = "獲取棧板净重", Paras = new List<string>() { "PACKNO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPL_SN_QTY", Description = "获取栈板内SN的数量", Paras = new List<string>() { "PACKNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPACK_QTY", Description = "获取R_PACKING表的QTY字段值", Paras = new List<string>() { "PACKNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPCS_NT", Description = "獲取板子重量", Paras = new List<string>() { "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPALLET_GW", Description = "獲取棧板毛重", Paras = new List<string>() { "PACKNO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPalletSum", Description = "獲取棧板数量", Paras = new List<string>() {  "DN_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTotal_Net_Weight_sum", Description = "獲取棧板總凈重", Paras = new List<string>() { "DN_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTotal_Gross_Weight_sum", Description = "獲取棧板總毛重", Paras = new List<string>() { "DN_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPallet_Qty_sum", Description = "獲取棧板數量", Paras = new List<string>() { "PACKNO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetBox_qty_sum", Description = "獲取箱子數量", Paras = new List<string>() { "PACKNO", "SKUNO"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTotalPallet_Qty_sum", Description = "獲取棧板總數量", Paras = new List<string>() { "DN_NO", "SKUNO"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTotalBox_qty_sum", Description = "獲取箱子總數量", Paras = new List<string>() { "DN_NO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTotal_Volumetric_Weight", Description = "獲取毛重", Paras = new List<string>() { "DN_NO" ,"PACKNO","SKUNO"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCountry_of_Origin", Description = "獲取原产国", Paras = new List<string>() { "PACKNO" } });

        }
        public string GetJSON_CUST_PO_DATA(OleExec SFCDB, string DN_NO)
        {
            var DN_PO = SFCDB.ORM.Queryable<R_DN_CUST_PO>().Where(t => t.DN_NO == DN_NO).First();
            if (DN_PO != null)
            {
                var PO_LINE = SFCDB.ORM.Queryable<R_CUST_PO_DETAIL>().Where(t => t.CUST_PO_NO == DN_PO.CUST_PO_NO && t.LINE_NO == DN_PO.CUST_PO_LINE_NO).First();
                if (PO_LINE != null)
                {
                    try
                    {
                        var data = JsonSave.GetFromDB<Object>(PO_LINE.CUST_PO_NO + "." + PO_LINE.LINE_NO, "BroadcomCustPOLine", SFCDB);
                        return data.ToString();
                    }
                    catch
                    {
                        return "NO CUST PO DATA";
                    }
                }
            }
            return "NO DATA";
        }



        public string Get_Orderno(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);
            if (PO != null)
            {

                return PO.SALES_ORDER_NUMBER;

            }
            return "NO PO DATA";
        }
        public string Get_Orderno_line(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);
            if (PO != null)
            {

                return PO.SALES_ORDER_LINE;

            }
            return "NO PO DATA";
        }
        public string Get_SHIP_TIME(OleExec SFCDB, string SO, string LINE_NO)
        {
         
            var sn = SFCDB.ORM.Queryable<R_CUST_PO_DETAIL>().Where(t => t.CUST_PO_NO == SO && t.LINE_NO == LINE_NO).First();
            if (sn != null)
            {
                DateTime t = (DateTime)sn.NEED_BY_DATE;
               var SHIPDATE=t.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-GB"));
                return SHIPDATE.ToString();
            }
            return "NO PO DATA";

        }
      
        public string SELLING_ENTITY_COMPANY_NAME(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_COMPANY_NAME;
            }
            return "NO DATA";
        }
        public string SELLING_ENTITY_ADDRESS1(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_ADDRESS1;
            }
            return "NO DATA";
        }
        public string SELLING_ENTITY_ADDRESS2(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_ADDRESS2;
            }
            return "NO DATA";
        }
        public string SELLING_ENTITY_ADDRESS3(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_ADDRESS3;
            }
            return "NO DATA";
        }
        public string SELLING_ENTITY_ADDRESS4(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_ADDRESS4;
            }
            return "NO DATA";
        }
        public string SELLING_ENTITY_CITY(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_CITY;
            }
            return "NO DATA";
        }
        public string SELLING_ENTITY_STATE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_STATE;
            }
            return "NO DATA";
        }
        public string SELLING_ENTITY_COUNTRY(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_COUNTRY;
            }
            return "NO DATA";
        }
        public string SELLING_ENTITY_POSTAL_CODE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SELLING_ENTITY_POSTAL_CODE;
            }
            return "NO DATA";
        }
        public string SHIP_TO_COMPANY_NAME(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_COMPANY_NAME;
            }
            return "NO DATA";
        }
        public string SHIP_TO_ADDRESS1(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_ADDRESS1;
            }
            return "NO DATA";
        }
        public string SHIP_TO_ADDRESS2(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_ADDRESS2;
            }
            return "NO DATA";
        }
        public string SHIP_TO_ADDRESS3(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_ADDRESS3;
            }
            return "NO DATA";
        }
        public string SHIP_TO_ADDRESS4(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_ADDRESS4;
            }
            return "NO DATA";
        }
        public string SHIP_TO_CITY(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_CITY;
            }
            return "NO DATA";
        }
        public string SHIP_TO_STATE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_STATE;
            }
            return "NO DATA";
        }
        public string SHIP_TO_POSTAL_CODE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_POSTAL_CODE;
            }
            return "NO DATA";
        }
        public string SHIP_TO_COUNTRY(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_COUNTRY;
            }
            return "NO DATA";
        }
        public string BILL_TO_COMPANY_NAME(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_COMPANY_NAME;
            }
            return "NO DATA";
        }
        public string BILL_TO_ADDRESS1(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_ADDRESS1;
            }
            return "NO DATA";
        }
        public string BILL_TO_ADDRESS2(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_ADDRESS2;
            }
            return "NO DATA";
        }
        public string BILL_TO_ADDRESS3(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_ADDRESS3;
            }
            return "NO DATA";
        }
        public string BILL_TO_ADDRESS4(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_ADDRESS4;
            }
            return "NO DATA";
        }
        public string BILL_TO_CITY(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_CITY;
            }
            return "NO DATA";
        }
        public string BILL_TO_STATE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_STATE;
            }
            return "NO DATA";
        }
        public string BILL_TO_POSTAL_CODE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_POSTAL_CODE;
            }
            return "NO DATA";
        }
        public string BILL_TO_COUNTRY(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.BILL_TO_COUNTRY;
            }
            return "NO DATA";
        }
        public string CUSTOMER_PO_NUMBER(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.CUSTOMER_PO_NUMBER;
            }
            return "NO DATA";
        }
        public string SHIPPING_METHOD(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIPPING_METHOD;
            }
            return "NO DATA";
        }
        public string SHIPPING_NOTE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIPPING_NOTE;
            }
            return "NO DATA";
        }
        public string SPECIAL_INSTRUCTION(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SPECIAL_INSTRUCTION;
            }
            return "NO DATA";
        }
        public string INCO_TERM(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.INCO_TERM;
            }
            return "NO DATA";
        }
        public string US_ECCN(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.US_ECCN;
            }
            return "NO DATA";
        }
        public string SHIP_TO_HTS(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.SHIP_TO_HTS;
            }
            return "NO DATA";
        }
        public string US_LICENSE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.US_LICENSE;
            }
            return "NO DATA";
        }
        public string ORDERED_QUANTITY(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.ORDERED_QUANTITY;
            }
            return "NO DATA";
        }
        public string Get_UOM(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.UOM;
            }
            return "NO DATA";
        }
        public string UNIT_PRICE(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.UNIT_PRICE;
            }
            return "NO DATA";
        }
        public string Itemdesc(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.ITEM_DESCRIPTION;
            }
            return "NO DATA";
        }
        public string CUSTOMER_ITEM(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                return PO.CUSTOMER_ITEM;
            }
            return "NO DATA";
        }
        public List<string> GetPalletByDn(OleExec SFCDB, string DN_NO)
        {
            
            var s1 = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN,R_SHIP_DETAIL>((P, SP, S,RD) => new object[] {
                SqlSugar.JoinType.Left, P.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID,
                SqlSugar.JoinType.Left,RD.SN==S.SN
            })
            .Where((P, SP, S,RD) =>RD.DN_NO== DN_NO&& S.SN == RD.SN && P.PACK_TYPE == "PALLET" && S.VALID_FLAG == "1")
            .Select((P, SP, S,RD) => P.PACK_NO).Distinct()
            .ToList();

            var s2 = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN,R_SHIP_DETAIL>((P1, P2, SP, S,RD) => new object[] {
                SqlSugar.JoinType.Left,P1.ID==P2.PARENT_PACK_ID,
                SqlSugar.JoinType.Left, P2.ID == SP.PACK_ID,
                SqlSugar.JoinType.Left, SP.SN_ID == S.ID,
                SqlSugar.JoinType.Left,RD.SN==S.SN
            })
            .Where((P1, P2, SP, S,RD) => RD.DN_NO == DN_NO && S.SN == RD.SN && P1.PACK_TYPE == "PALLET" && S.VALID_FLAG == "1")
            .Select((P1, P2, SP, S,RD) => P1.PACK_NO).Distinct()
            .ToList();

            s1.AddRange(s2);
           
            return s1;
        }
        public string GetPACK_QTY(OleExec SFCDB, string PACKNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            var pack = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).First();
            if (pack == null)
            {
                return "PACK NO FOUND";
            }
            var pack_sn = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING>((rp, rsp) => rp.ID == rsp.PACK_ID)
                .Where((rp, rsp) => rp.PARENT_PACK_ID == pack.ID).Select((rp, rsp) => rsp).ToList();
            //return pack.QTY.ToString();
            return pack_sn.Count().ToString();
        }
        public string GetDN_QTY(OleExec SFCDB, string DN_NO)
        {
            
            var DN= SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == DN_NO).First();
            if (DN == null)
            {
                return "PACK NO FOUND";
            }

            return DN.QTY.ToString();
        }
        public string GetDnByPallet(OleExec SFCDB, string PACKNO)
        {
            var DN = SFCDB.ORM.Queryable<R_SN_PACKING, R_PACKING, R_SN_OVERPACK, R_PACKING>((RSP, RP1, RSO, RP2)=>RSP.PACK_ID==RP1.ID&&RP1.PARENT_PACK_ID==RP2.ID&&RSP.SN_ID==RSO.SN_ID)
                .Where((RSP, RP1, RSO, RP2) => RP2.PACK_NO == PACKNO).Select((RSP, RP1, RSO, RP2) => RSO.DN_NO).ToList().FirstOrDefault();

            //var DN = SFCDB.ORM.Queryable<R_SN_PACKING, R_PACKING, R_SN_OVERPACK, R_PACKING>((RSP, RP1, RSO, RP2) => new object[] {
            //    SqlSugar.JoinType.Left,RSP.PACK_ID==RP1.ID,
            //    SqlSugar.JoinType.Left, RP1.PARENT_PACK_ID == RP2.ID,
            //    SqlSugar.JoinType.Left, RSP.SN_ID == RSO.SN_ID
            //}).Where((RSP, RP1, RSO, RP2) => RP2.PACK_NO == PACKNO).Select((RSP, RP1, RSO, RP2) => RSO.DN_NO).First();


            if (DN == null)
            {
                return "DN NO FOUND";
            }
            return DN.ToString();
        }
        public string GetPL_SN_QTY(OleExec SFCDB, string PACKNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            var pack = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKNO).First();
            if (pack == null)
            {
                return "PACK NO FOUND";
            }
            var ctns = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == pack.ID).ToList();
            if (ctns.Count == 0)
            {
                return pack.QTY.ToString();
            }

            int c = 0;
            for (int i = 0; i < ctns.Count; i++)
            {
                c += (int)ctns[i].QTY;
            }

            return c.ToString();
        }
        public string GetPALLET_NET(OleExec SFCDB, string PACKNO,string SKUNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            var qty = GetPL_SN_QTY(SFCDB, PACKNO);
            var cpw= SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
            if (cpw == null)
            {
                return "Weight information is not Configured!";
            }

            var PALLET_NET = Convert.ToDecimal(cpw.PCS_NT) * Convert.ToInt32(qty);
   
            return PALLET_NET.ToString()+"kg";

        }
        public string GetPalletSum(OleExec SFCDB, string DN_NO)
        {
            int PalletSum=0 ;
            string Strsql = $@"  select distinct e.pack_no,e.skuno From r_ship_detail  a,r_sn b,r_sn_packing c,R_PACKING d,R_PACKING e 
                                                    where a.dn_no='{DN_NO}' and a.sn=b.sn and b.id=c.SN_ID and c.PACK_ID=d.id and d.PARENT_PACK_ID=e.id";
            var Pallet = SFCDB.ExecuteDataTable(Strsql, CommandType.Text, null);
            PalletSum = Pallet.Rows.Count;
            return PalletSum.ToString();
        }
        public string GetPCS_NT(OleExec SFCDB, string SKUNO)
        {
            var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
            if (cpw == null)
            {
                return "Weight information is not Configured!";
            }
            return cpw.PCS_NT.ToString() + "kg";

        }
        public string GetPALLET_GW(OleExec SFCDB, string PACKNO, string SKUNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            var PALLET_GW = "";
            var WEIGHT_REMNANT = "";
            int PALLET_REMNANT=0;
            int BOX_REMNANT = 0;
            var qty = GetPL_SN_QTY(SFCDB, PACKNO);
            var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
            if (cpw == null)
            {
                return "Weight information is not Configured!";
            }
            if (qty == cpw.PCS_P)
            {
                if (!string.IsNullOrEmpty(cpw.P_GW.ToString()) && (System.Text.RegularExpressions.Regex.IsMatch(cpw.P_GW.ToString(), @"^[1-9]\d*|0$")
                    || System.Text.RegularExpressions.Regex.IsMatch(cpw.P_GW.ToString(), @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$")))
                {
                    PALLET_GW = Convert.ToDecimal(cpw.P_GW).ToString("F2");
                }
              
            }
            else
            {
                int  BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
                int  PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
                if (PCS_B_REMNANT > 0)
                {
                    BOX_REMNANT = 1;
                }
                if (BOX_FULL_REMNAN >= 4)
                {
                    var W_RT = BOX_FULL_REMNAN * Convert.ToDecimal(cpw.BOX_GW) + PCS_B_REMNANT * Convert.ToDecimal(cpw.PCS_GW);
                    if (W_RT >= 40)
                    {
                        PALLET_REMNANT = 1;
                    }
                    WEIGHT_REMNANT = W_RT.ToString();

                }
                else
                {
                    var W_RT = BOX_FULL_REMNAN * Convert.ToDecimal(cpw.BOX_GW) + PCS_B_REMNANT * Convert.ToDecimal(cpw.PCS_GW);
                    WEIGHT_REMNANT = W_RT.ToString();
                }
               var  P_GW = Convert.ToDecimal(WEIGHT_REMNANT) + PALLET_REMNANT * Convert.ToDecimal(cpw.P_NULLWG) + BOX_REMNANT * (Convert.ToDecimal(cpw.BOX_GW) - Convert.ToDecimal(cpw.BOX_NT));

                if (!string.IsNullOrEmpty(P_GW.ToString()) &&(System.Text.RegularExpressions.Regex.IsMatch(P_GW.ToString(), @"^[1-9]\d*|0$") 
                    ||System.Text.RegularExpressions.Regex.IsMatch(P_GW.ToString(), @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$")))
                {
                    PALLET_GW = Convert.ToDecimal(P_GW).ToString("F2");
                }

                
            }

            return PALLET_GW.ToString() + "kg";

        }
        public string GetPallet_Qty_sum(OleExec SFCDB, string PACKNO, string SKUNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            int PALLET_REMNANT = 0;
            var qty = GetPL_SN_QTY(SFCDB, PACKNO);
            var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
   
            int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
                int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
                if (BOX_FULL_REMNAN >= 4)
                {
                    var W_RT = BOX_FULL_REMNAN * Convert.ToDecimal(cpw.BOX_GW) + PCS_B_REMNANT * Convert.ToDecimal(cpw.PCS_GW);
                    if (W_RT >= 40)
                    {
                        PALLET_REMNANT = 1;
                    }
    
                }
            var Pallet_Qty_sum = PALLET_REMNANT ;
            return Pallet_Qty_sum.ToString();

        }
        public string GetBox_qty_sum(OleExec SFCDB, string PACKNO, string SKUNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            int BOX_REMNANT = 0;
            int BOX_SINGLE = 0;
            var qty = GetPL_SN_QTY(SFCDB, PACKNO);
            var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
            int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
            int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
            if (PCS_B_REMNANT > 0)
            {
                BOX_SINGLE = 1;
            }

            BOX_REMNANT = BOX_FULL_REMNAN + BOX_SINGLE;
            int Box_qty_sum = BOX_REMNANT;
            return Box_qty_sum.ToString();

        }
        public string GetTotalPallet_Qty_sum(OleExec SFCDB, string DN_NO, string SKUNO)
        {
            int TotalPallet_Qty_sum = 0;
            var PalletQty = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rsd, rs, rsp, rp1, rp2) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                .Where((rsd, rs, rsp, rp1, rp2) => rs.VALID_FLAG == "1" && rsd.DN_NO == DN_NO).Select((rsd, rs, rsp, rp1, rp2) => rp2.PACK_NO).ToList();
            for (int i = 0; i < PalletQty.Distinct().ToList().Count; i++)
            {
                int PALLET_REMNANT = 0;
                var PACKNO = PalletQty.Distinct().ToList()[i].ToString();
                var qty = GetPL_SN_QTY(SFCDB,PACKNO);
                var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
                int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
                int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
                if (BOX_FULL_REMNAN >= 4)
                {
                    var W_RT = BOX_FULL_REMNAN * Convert.ToDecimal(cpw.BOX_GW) + PCS_B_REMNANT * Convert.ToDecimal(cpw.PCS_GW);
                    if (W_RT >= 40)
                    {
                        PALLET_REMNANT = 1;
                    }

                }
                int Pallet_Qty_sum = PALLET_REMNANT ;

                TotalPallet_Qty_sum += Pallet_Qty_sum;
            }
            return TotalPallet_Qty_sum.ToString();

        }
        public string GetTotalBox_qty_sum(OleExec SFCDB, string DN_NO, string SKUNO)
        {
            int TotalBox_Qty_sum = 0;
            int BOX_REMNANT = 0;
            int BOX_SINGLE = 0;
            var PalletQty = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rsd, rs, rsp, rp1, rp2) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                .Where((rsd, rs, rsp, rp1, rp2) => rs.VALID_FLAG == "1" && rsd.DN_NO == DN_NO).Select((rsd, rs, rsp, rp1, rp2) => rp2.PACK_NO).ToList();
            for (int i = 0; i < PalletQty.Distinct().ToList().Count; i++)
            {
               
                var PACKNO = PalletQty.Distinct().ToList()[i].ToString();
                var qty = GetPL_SN_QTY(SFCDB, PACKNO);
                var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
                int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
                int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
                if (PCS_B_REMNANT > 0)
                {
                    BOX_SINGLE = 1;
                }

                BOX_REMNANT = BOX_FULL_REMNAN + BOX_SINGLE;
                int Box_qty_sum = BOX_REMNANT ;

                TotalBox_Qty_sum += Box_qty_sum;
            }
            return TotalBox_Qty_sum.ToString();   

        }
        public string GetTotal_Net_Weight_sum(OleExec SFCDB, string DN_NO)
        {

            float Total_Net_Weight_sum =0;
            string Strsql = $@"  select distinct e.pack_no,e.skuno From r_ship_detail  a,r_sn b,r_sn_packing c,R_PACKING d,R_PACKING e 
                                                    where a.dn_no='{DN_NO}' and a.sn=b.sn and b.id=c.SN_ID and c.PACK_ID=d.id and d.PARENT_PACK_ID=e.id";
            var Pallet = SFCDB.ExecuteDataTable(Strsql, CommandType.Text, null);

            for (int i = 0; i < Pallet.Rows.Count; i++)
            {
                var PALLET_NET = GetPALLET_NET (SFCDB, Pallet.Rows[i][0].ToString(), Pallet.Rows[i][1].ToString());
                PALLET_NET = PALLET_NET.Replace("kg", "").ToString();
                Total_Net_Weight_sum += float.Parse(PALLET_NET);
            }
         

            return Total_Net_Weight_sum.ToString() + "kg";
        }
        public string GetTotal_Gross_Weight_sum(OleExec SFCDB, string DN_NO)
        {

            float Total_Gross_Weight_sum = 0;
            string Strsql = $@"  select distinct e.pack_no,e.skuno From r_ship_detail  a,r_sn b,r_sn_packing c,R_PACKING d,R_PACKING e 
                                                    where a.dn_no='{DN_NO}' and a.sn=b.sn and b.id=c.SN_ID and c.PACK_ID=d.id and d.PARENT_PACK_ID=e.id";
            var Pallet = SFCDB.ExecuteDataTable(Strsql, CommandType.Text, null);

            for (int i = 0; i < Pallet.Rows.Count; i++)
            {
               var  PALLET_GW = GetPALLET_GW(SFCDB,Pallet.Rows[i][0].ToString(), Pallet.Rows[i][1].ToString());
                PALLET_GW = PALLET_GW.Replace("kg", "").ToString();
                Total_Gross_Weight_sum += float.Parse( PALLET_GW);
            }
            string Total_Gross_Weight_Sum = Total_Gross_Weight_sum.ToString();
            if (!string.IsNullOrEmpty(Total_Gross_Weight_Sum) && (System.Text.RegularExpressions.Regex.IsMatch(Total_Gross_Weight_Sum, @"^[1-9]\d*|0$")
                 || System.Text.RegularExpressions.Regex.IsMatch(Total_Gross_Weight_Sum, @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$")))
            {
                Total_Gross_Weight_Sum = (Convert.ToDecimal(Total_Gross_Weight_Sum).ToString("F2"));
            }

            return Total_Gross_Weight_Sum + "kg";
        }
        public string GetTotal_Volumetric_Weight(OleExec SFCDB, string DN_NO,string PACKNO,string SKUNO)
        {
            PACKNO = PACKNO.Replace("MBO", "").ToString();
            decimal Pallet_vol_sum;
            var WEIGHT_REMNANT = "";
            int PALLET_REMNANT = 0;
            decimal PALLET_VOL =1;
            decimal CARTON_VOL = 1;
            decimal BOX_VOL = 1;
            decimal VOL_REMNANT=0 ;
            decimal W_RT=0;
            int BOX_SINGLE = 1;
            var qty = GetPL_SN_QTY(SFCDB, PACKNO);
            var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
            int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
            int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
            var cps = SFCDB.ORM.Queryable<C_PACKING_SIZE>().Where(t => t.SKUNO == SKUNO).First();
            if (cps == null)
            {
                return "PackSize information is not Configured!";
            }
            else
            {
                string[] arr = cps.PALLET_SIZE.Split('X');
                foreach (string item in arr)
                {
                    PALLET_VOL *=  Decimal.Parse(item);
                }
                string[] ass= cps.CARTON_SIZE.Split('X');
                foreach (string item in ass)
                {
                    CARTON_VOL *=  Decimal.Parse(item);
                }
                string[] asp = cps.BOX_SIZE.Split('X');
                foreach (string item in asp)
                {
                    BOX_VOL *= Decimal.Parse(item);
                }
            }
            if (cpw == null)
            {
                return "Weight information is not Configured!";
            }
            if (qty == cpw.PCS_P)
            {
                Pallet_vol_sum = PALLET_VOL + BOX_FULL_REMNAN * CARTON_VOL;
            }
            else
            {
                var config = JsonSave.GetFromDB<List<OverPackConfig>>("OVERPACKCONFIG", "SKUCONFIG", SFCDB);
                var aa= config.FindAll(t => t.Skuno == SKUNO&&t.PrintType=="LIST");
               
                    if (aa.Count > 0)
                    {
                        if (PCS_B_REMNANT == 0)
                        {
                            BOX_SINGLE = 0;
                        }
                        W_RT = BOX_FULL_REMNAN *decimal.Parse(cpw.BOX_GW) + PCS_B_REMNANT * decimal.Parse(cpw.PCS_GW)+ BOX_SINGLE*(decimal.Parse(cpw.BOX_GW)- decimal.Parse(cpw.BOX_NT));
                        VOL_REMNANT = CARTON_VOL * BOX_FULL_REMNAN + BOX_SINGLE * CARTON_VOL;
                    }
                    else
                    {


                        VOL_REMNANT = BOX_FULL_REMNAN * CARTON_VOL + PCS_B_REMNANT * BOX_VOL;
                        W_RT = BOX_FULL_REMNAN * decimal.Parse(cpw.BOX_GW) + PCS_B_REMNANT * decimal.Parse(cpw.PCS_GW);
                    }
                    if (W_RT >= 40)
                    {
                        PALLET_REMNANT = 1;
                    }
                    WEIGHT_REMNANT = W_RT.ToString();
                    Pallet_vol_sum = VOL_REMNANT + PALLET_VOL * PALLET_REMNANT;
            }
            var Total_Volumetric_Weigh = decimal.Round (Pallet_vol_sum / 5000,3);
            return Total_Volumetric_Weigh.ToString() +"kg";
        }

        public string GetTotal_Volumetric_Weight_sum(OleExec SFCDB, string DN_NO, string SKUNO)
        {
            
            decimal Pallet_vol_sum;
            var WEIGHT_REMNANT = "";
            int PALLET_REMNANT = 0;
            decimal PALLET_VOL = 1;
            decimal CARTON_VOL = 1;
            decimal BOX_VOL = 1;
            decimal VOL_REMNANT = 0;
            decimal W_RT = 0;
            decimal Total_Volumetric_Weigh = 0;
            decimal Total_Volumetric_Weigh_sum = 0;
            int BOX_SINGLE = 1;
            var cps = SFCDB.ORM.Queryable<C_PACKING_SIZE>().Where(t => t.SKUNO == SKUNO).First();
            if (cps == null)
            {
                return "PackSize information is not Configured!";
            }
            else
            {
                string[] arr = cps.PALLET_SIZE.Split('X');
                foreach (string item in arr)
                {
                    PALLET_VOL *= Decimal.Parse(item);
                }
                string[] ass = cps.CARTON_SIZE.Split('X');
                foreach (string item in ass)
                {
                    CARTON_VOL *= Decimal.Parse(item);
                }
                string[] asp = cps.BOX_SIZE.Split('X');
                foreach (string item in asp)
                {
                    BOX_VOL *= Decimal.Parse(item);
                }
            }
            var PalletQty = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rsd, rs, rsp, rp1, rp2) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
              .Where((rsd, rs, rsp, rp1, rp2) => rs.VALID_FLAG == "1" && rsd.DN_NO == DN_NO).Select((rsd, rs, rsp, rp1, rp2) => rp2.PACK_NO).ToList();
            for (int i = 0; i < PalletQty.Distinct().ToList().Count; i++)
            {
                var PACKNO = PalletQty.Distinct().ToList()[i].ToString();
                var qty = GetPL_SN_QTY(SFCDB, PACKNO);
                var cpw = SFCDB.ORM.Queryable<C_PACKOUT_WEIGHT>().Where(t => t.PN == SKUNO).First();
                int BOX_FULL_REMNAN = Convert.ToInt32(qty) / Convert.ToInt32(cpw.PCS_B);
                int PCS_B_REMNANT = Convert.ToInt32(qty) % Convert.ToInt32(cpw.PCS_B);
               
                if (cpw == null)
                {
                    return "Weight information is not Configured!";
                }
                if (qty == cpw.PCS_P)
                {
                    Pallet_vol_sum = PALLET_VOL + BOX_FULL_REMNAN * CARTON_VOL;
                }
                else
                {
                    var config = JsonSave.GetFromDB<List<OverPackConfig>>("OVERPACKCONFIG", "SKUCONFIG", SFCDB);
                    var aa = config.FindAll(t => t.Skuno == SKUNO && t.PrintType == "LIST");

                    if (aa.Count > 0)
                    {
                        if (PCS_B_REMNANT == 0)
                        {
                            BOX_SINGLE = 0;
                        }
                        W_RT = BOX_FULL_REMNAN * decimal.Parse(cpw.BOX_GW) + PCS_B_REMNANT * decimal.Parse(cpw.PCS_GW) + BOX_SINGLE * (decimal.Parse(cpw.BOX_GW) - decimal.Parse(cpw.BOX_NT));
                        VOL_REMNANT = CARTON_VOL * BOX_FULL_REMNAN + BOX_SINGLE * CARTON_VOL;
                    }
                    else
                    {


                        VOL_REMNANT = BOX_FULL_REMNAN * CARTON_VOL + PCS_B_REMNANT * BOX_VOL;
                        W_RT = BOX_FULL_REMNAN * decimal.Parse(cpw.BOX_GW) + PCS_B_REMNANT * decimal.Parse(cpw.PCS_GW);
                    }
                    if (W_RT >= 40)
                    {
                        PALLET_REMNANT = 1;
                    }
                    WEIGHT_REMNANT = W_RT.ToString();
                    Pallet_vol_sum = VOL_REMNANT + PALLET_VOL * PALLET_REMNANT;
                }
                 Total_Volumetric_Weigh = decimal.Round(Pallet_vol_sum / 5000, 3);

                Total_Volumetric_Weigh_sum += Total_Volumetric_Weigh;
            }
            return Total_Volumetric_Weigh_sum.ToString() + "kg";
        }
        public string GetCountry_of_Origin(OleExec SFCDB, string PACKNO)
        {
            return "VN";
        }









    }
}
