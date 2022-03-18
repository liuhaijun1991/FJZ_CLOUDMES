using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation.Label;
using MESPubLab.MESStation.SNMaker;
using MESStation.Config.DCN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.DCN
{
    public class DCNOverPackValueGroup : LabelValueGroup
    {
        string[] SKUTYPE1 = new string[] { "HP-6558-0002","HP-6558-0001","XHP-6558-0001","XHP-6558-0002","HP-6558-0000","XHP-6558-0000",
                  "HP-6559-0000","ER-7000-0846","XHP-6559-0000","ER-7000-0599","ER-7000-0612","XHP-G648-0000","XHP-G648-0001",
                  "XHP-G648-0002","XHP-G649-0000","HP-G648-0010","HP-G648-0011 ","HP-G648-0012 ","XHP-G648-0000","XHP-G648-0001","XHP-G648-0002",
                  "ER-7000-0809","ER-7000-0808","ER-7000-0850","ER-7000-0837","DL-MXG610S-0006"};
        public DCNOverPackValueGroup()
        {
            ConfigGroup = "DCNOverPackValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetJSON_CUST_PO_DATA", Description = "Get Json format CUST_PO_DATA by DN", Paras = new List<string>() { "DN_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetDN_SKU", Description = "Get date on database system", Paras = new List<string>() { "DN_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_TOADDR0", Description = "", Paras = new List<string>() { "JSON_PO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_TOADDR1", Description = "", Paras = new List<string>() { "JSON_PO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_TOADDR2", Description = "", Paras = new List<string>() { "JSON_PO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_TOADDR3", Description = "", Paras = new List<string>() { "JSON_PO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_TOADDR4", Description = "", Paras = new List<string>() { "JSON_PO", "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_orderno", Description = "SALES_ORDER_NUMBER", Paras = new List<string>() { "JSON_PO"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_CUSTPO_INF", Description = "", Paras = new List<string>() { "JSON_PO", "NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_SKU_DETAIL", Description = "", Paras = new List<string>() { "SKUNO", "CATEGORY", "CATEGORY_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_SN_KP", Description = "", Paras = new List<string>() { "SN", "KP_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_SN_KPPN", Description = "", Paras = new List<string>() { "SN", "KP_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_PACK_SN", Description = "", Paras = new List<string>() { "DN_NO", "PACK_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_PACK_TIME", Description = "", Paras = new List<string>() { "DN_NO", "PACK_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_PO_NO", Description = "", Paras = new List<string>() { "JSON_PO"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_Shipdate", Description = "", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_BOX_QTY", Description = "", Paras = new List<string>() { "pkg_list_qty", "skuno" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetOverpackQty", Description = "", Paras = new List<string>() { "DN", "CURRENT_PACK_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_PKGQTY", Description = "", Paras = new List<string>() { "JSON_PO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_PKGID", Description = "", Paras = new List<string>() { "RULE_NAME", "PKGID_LAST_PREFIX", "BOX_QTY" } }); 
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSalesOrderLine", Description = "", Paras = new List<string>() { "JSON_PO" } });
            
        }
        public string Get_SKU_DETAIL(OleExec SFCDB, string SKUNO, string CATEGORY,string CATEGORY_NAME)
        {
            var CG = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY == CATEGORY && t.CATEGORY_NAME == CATEGORY_NAME).First();
            return CG?.VALUE;
        }
      
        public string Get_PACK_TIME(OleExec SFCDB, string DN_NO, string PACK_NO)
        {
            var sn = SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(t => t.DN_NO == DN_NO && t.PACK_NO == int.Parse(PACK_NO)).First();
            return sn?.EDIT_TIME.ToString();
        }
        public string Get_PACK_SN(OleExec SFCDB, string DN_NO, string PACK_NO)
        {
            var sn = SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(t => t.DN_NO == DN_NO && t.PACK_NO == int.Parse(PACK_NO)).First();
            return sn?.SN;
        }
        public string Get_SN_KP(OleExec SFCDB, string SN, string KP_NAME)
        {
            var KP = SFCDB.ORM.Queryable<R_SN_KP>().Where(t=>t.SN == SN && t.KP_NAME == KP_NAME && t.VALID_FLAG == 1).First();
            return KP?.VALUE;
        }
        public string Get_SN_KPPN(OleExec SFCDB, string SN, string KP_NAME)
        {
            var KP = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.KP_NAME == KP_NAME && t.VALID_FLAG == 1).First();
            return KP?.PARTNO;
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
                        var data =  JsonSave.GetFromDB<Object>(PO_LINE.CUST_PO_NO+"."+PO_LINE.LINE_NO, "BroadcomCustPOLine", SFCDB);
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
        public string GetDN_SKU(OleExec SFCDB, string DN_NO)
        {
            var DN_PO = SFCDB.ORM.Queryable<R_DN_CUST_PO>().Where(t => t.DN_NO == DN_NO).First();
            if (DN_PO == null)
            {
                return "NO DATA";
            }
            return DN_PO.DN_SKUNO;
        }
        public string Get_TOADDR0(OleExec SFCDB, string JSON_PO,string SKUNO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);
         
            if (PO != null)
            {
                return PO.CUSTOMER_NAME;
            }
            return "NO DATA";
        }

        public string Get_TOADDR1(OleExec SFCDB, string JSON_PO,string SKUNO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);
            if (PO != null)
            {
                return PO.SHIP_TO_ADDRESS1;
            }
            return "NO DATA";
        }
        public string Get_TOADDR2(OleExec SFCDB, string JSON_PO, string SKUNO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);
            
            if (PO != null)
            {
                bool inSkutype = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "DCNOverPackControl" && r.CATEGORY == "SKUTYPE"
                  && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == SKUNO).Any();
                if (inSkutype)
                {
                    var len = GetLenght(PO.SHIP_TO_ADDRESS2) + GetLenght(PO.SHIP_TO_ADDRESS3) + GetLenght(PO.SHIP_TO_ADDRESS4);
                    if (len <= 50)
                    {
                        return $@"{PO.SHIP_TO_ADDRESS2} {PO.SHIP_TO_ADDRESS3} {PO.SHIP_TO_ADDRESS4}";
                    }
                    len = GetLenght(PO.SHIP_TO_ADDRESS2) + GetLenght(PO.SHIP_TO_ADDRESS3);
                    if (len <= 51)
                    {
                        return $@"{PO.SHIP_TO_ADDRESS2} {PO.SHIP_TO_ADDRESS3}";
                    }
                    if (GetLenght(PO.SHIP_TO_ADDRESS2) <= 51)
                    {
                        return PO.SHIP_TO_ADDRESS2;
                    }
                    return "1.shiptoaddress is too long, pls call it！";
                }

                return "SKU not in TYPE List";

            }
            return "NO PO DATA";
        }

        public string Get_TOADDR3(OleExec SFCDB, string JSON_PO, string SKUNO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                bool inSkutype = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "DCNOverPackControl" && r.CATEGORY == "SKUTYPE"
                  && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == SKUNO).Any();
                if (inSkutype)
                {
                    var len = GetLenght(PO.SHIP_TO_ADDRESS2) + GetLenght(PO.SHIP_TO_ADDRESS3) + GetLenght(PO.SHIP_TO_ADDRESS4);
                    if (len <= 50)
                    {
                        return $@"{PO.SHIP_TO_CITY} {PO.SHIP_TO_STATE} {PO.SHIP_TO_POSTAL_CODE}";
                    }
                    len = GetLenght(PO.SHIP_TO_ADDRESS2) + GetLenght(PO.SHIP_TO_ADDRESS3);
                    if (len <= 51)
                    {
                        len = GetLenght(PO.SHIP_TO_ADDRESS4);
                        if (len <= 52)
                        {
                            return PO.SHIP_TO_ADDRESS4;
                        }
                        else
                        {
                            return "4.shiptoaddress is too long,pls call it！";
                        }

                        
                    }
                    if (PO.SHIP_TO_ADDRESS2.Length <= 52)
                    {
                        len = GetLenght(PO.SHIP_TO_ADDRESS3) + GetLenght(PO.SHIP_TO_ADDRESS4);
                        if (len <= 51)
                        {
                            return $@"{PO.SHIP_TO_ADDRESS3} {PO.SHIP_TO_ADDRESS4}";
                        }
                        else
                        {
                            return "5.shiptoaddress is too long,pls call it！";
                        }
                    }
                    return "2.shiptoaddress is too long,pls call it！";
                }

                return "SKU not in TYPE List";

            }
            return "NO PO DATA";
        }

        public string Get_TOADDR4(OleExec SFCDB, string JSON_PO, string SKUNO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                bool inSkutype = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "DCNOverPackControl" && r.CATEGORY == "SKUTYPE"
                  && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.VALUE == SKUNO).Any();
                if (inSkutype)
                {
                    var len = GetLenght(PO.SHIP_TO_ADDRESS2) + GetLenght(PO.SHIP_TO_ADDRESS3) + GetLenght(PO.SHIP_TO_ADDRESS4);
                    if (len <= 50)
                    {
                        return $@"{PO.SHIP_TO_STATE} {PO.SHIP_TO_COUNTRY}";
                    }
                    len = GetLenght(PO.SHIP_TO_ADDRESS2) + GetLenght(PO.SHIP_TO_ADDRESS3);
                    if (len <= 51)
                    {
                        return $@"{PO.SHIP_TO_CITY} {PO.SHIP_TO_STATE} {PO.SHIP_TO_POSTAL_CODE} {PO.SHIP_TO_STATE} {PO.SHIP_TO_COUNTRY} ";

                    }
                    if (GetLenght(PO.SHIP_TO_ADDRESS2) <= 52)
                    {
                        return $@"{PO.SHIP_TO_CITY} {PO.SHIP_TO_STATE} {PO.SHIP_TO_POSTAL_CODE} {PO.SHIP_TO_STATE} {PO.SHIP_TO_COUNTRY} ";
                    }
                    return "3.shiptoaddress is too long,pls call it！";
                }

                return "SKU not in TYPE List";

            }
            return "NO PO DATA";
        }

        public string Get_CUSTPO_INF(OleExec SFCDB, string JSON_PO, string NAME)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);
            Type T = PO.GetType();
            var P = T.GetProperty(NAME);
            var Value = P.GetValue(PO).ToString();
            return Value;
        }

        public string Get_PO_NO(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {

                return PO.CUSTOMER_PO_NUMBER;

            }
            return "NO PO DATA";
        }
        public string GetSalesOrderLine(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {

                return PO.SALES_ORDER_LINE;

            }
            return "NO PO DATA";
        }
        public string Get_Shipdate(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            //if (PO != null)
            //{

            //    return PO.SUGGESTED_SHIP_DATE;

            //}
            return (SFCDB.ORM.GetDate()).ToString("MMM-dd-yyyy", new System.Globalization.CultureInfo("en-us"));
            //return "NO PO DATA";
        }
        public string Get_orderno(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                
                return PO.SALES_ORDER_NUMBER;

            }
            return "NO PO DATA";
        }

        public string Get_CUSTPRODDESC(OleExec SFCDB, string JSON_PO, string SKUNO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                var V = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY == "LABEL" && t.CATEGORY_NAME == "PRINTLABELDESC").First();
                return V.VALUE;

            }
            return "NO DATA PLs config C_SKU_DETAIL CATEGORY =LABEL, CATEGORY_NAME == PRINTLABELDESC ";
        }

        public string Get_VERSION(OleExec SFCDB, string JSON_PO, string SKUNO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);

            if (PO != null)
            {
                var V = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY == "LABEL" && t.CATEGORY_NAME == "LABELVER").First();
                return V.VALUE;

            }
            return "NO DATA PLs config C_SKU_DETAIL CATEGORY =LABEL, CATEGORY_NAME == LABELVER ";
        }
        public string Get_BOX_QTY(OleExec SFCDB, string pkg_list_qty, string skuno)
        {
            string output = "";
            var packObj = SFCDB.ORM.Queryable<C_PACKING>().Where(r => r.SKUNO == skuno && r.PACK_TYPE == "CARTON").ToList().FirstOrDefault();
            if (packObj == null)
            {
                throw new Exception($@"{skuno} Not Setting Packing Number");
            }
            int order_qty = Convert.ToInt32(pkg_list_qty);
            int carton_qty = Convert.ToInt32(packObj.MAX_QTY);
            int total_qty = order_qty / carton_qty;
            if (order_qty % carton_qty != 0)
            {
                total_qty += 1;
            }
            output = total_qty.ToString();
            return output;
        }

        public string GetOverpackQty(OleExec SFCDB, string DN,string CURRENT_PACK_NO)
        {
            string output = "";
            double? packno = Convert.ToDouble(CURRENT_PACK_NO);
            var list = SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(r => r.DN_NO == DN && r.PACK_NO == packno).Select(r => r.SN).ToList();
            output = list.Count.ToString();
            return output;
        }
        public string Get_PKGQTY(OleExec SFCDB, string JSON_PO)
        {
            var PO = Newtonsoft.Json.JsonConvert.DeserializeObject<BroadcomCustPOLine>(JSON_PO);
            if (PO != null)
            {               
                return PO.ORDERED_QUANTITY;
            }
            return "NO DATA PLs config C_SKU_DETAIL CATEGORY =LABEL, CATEGORY_NAME == LABELVER ";
        }
        public string Get_PKGID(OleExec SFCDB, string RULE_NAME,string PKGID_LAST_PREFIX, string BOX_QTY)
        {
            string output = "";
            string pkgid = SNmaker.GetNextSN(RULE_NAME, SFCDB);
            output = pkgid + PKGID_LAST_PREFIX + BOX_QTY;
            return output;
        }
        
        int GetLenght(string str)
        {
            if (str == null)
            {
                return 0;
            }
            return str.Length;
        }

    }
}
