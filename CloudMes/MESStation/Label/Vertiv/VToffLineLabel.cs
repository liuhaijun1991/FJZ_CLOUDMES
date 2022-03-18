using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.Vertiv
{
    public class VToffLineLabel : LabelValueGroup
    {
        public VToffLineLabel()
        {
            ConfigGroup = "VToffLineLabel";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetGlobalSNByWO", Description = "Get Global SN By WO", Paras = new List<string>() { "wo", "prefixRule" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetENESByGlobalSN", Description = "Get ENES SN By Global SN", Paras = new List<string>() { "globalSN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetOffLineSNByRuleName", Description = "Get Off Line SN By Rule Name", Paras = new List<string>() { "ruleName" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetOffLineSNListByRuleName", Description = "Get Off Line SN List By Rule Name", Paras = new List<string>() { "ruleName","qty" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSkuFTLabelModelByWO", Description = "Get Sku Model By WO", Paras = new List<string>() { "wo" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetCustomerVersionByWO", Description = "GetCustomerVersionByWO", Paras = new List<string>() { "wo" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSoftwareModelBySku", Description = "Ge Software Model By Sku", Paras = new List<string>() { "sku" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSoftwareVersionBySku", Description = "Ge Software Version By Sku", Paras = new List<string>() { "sku" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSoftwareLabelData", Description = "GetSoftwareLabelData", Paras = new List<string>() { "wo" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetVersionAndDate", Description = "Ge Version And Date", Paras = new List<string>() { "version" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSkuLast8CharByWO", Description = "Get Sku Last 8 Char By WO", Paras = new List<string>() { "wo" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "SavePrintLog", Description = "SavePrintLog", Paras = new List<string>() { "SN", "WO", "EMP_NO", "LABEL_TYPE","PRINT_TYPE" } });
        }

        public string GetGlobalSNByWO(OleExec SFCDB, string wo,string prefixRule)
        {
            var value = "No data record";

            value = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(prefixRule.Trim(), SFCDB);

            var skuDetail = SFCDB.ORM.Queryable<R_WO_BASE, C_SKU_DETAIL>((r, c) => r.SKUNO == c.SKUNO)
                .Where((r, c) => r.WORKORDERNO == wo && SqlSugar.SqlFunc.ToUpper(c.CATEGORY) == "OFFLINELABEL" && SqlSugar.SqlFunc.ToUpper(c.CATEGORY_NAME) == "GLOBALLABELMODEL")
                .Select((r, c) => c).ToList().FirstOrDefault();
            if(skuDetail != null)
            {
                value += skuDetail.VALUE.Trim();
            }

            var ver3 = SFCDB.ORM.Queryable<R_WO_BASE, C_SKU_VER_MAPPING>((r, c) => r.SKUNO == c.FOX_SKUNO && r.SKU_VER == c.FOX_VERSION1)
                .Where((r, c) => r.WORKORDERNO == wo).Select((r, c) => c).ToList().FirstOrDefault();
            if(ver3!=null)
            {
                value += ver3.FOX_VERSION2.Trim();
            }
            return value;
        }

        public string GetENESByGlobalSN(string globalSN)
        {
            return globalSN.Length > 11 ? $@"ENES{globalSN.Substring(0, 11)}" : $@"ENES{globalSN}";
        }

        public string GetOffLineSNByRuleName(OleExec SFCDB, string ruleName)
        {
            var value = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(ruleName, SFCDB);
            return value;
        }
        public List<string> GetOffLineSNListByRuleName(OleExec SFCDB, string ruleName, string qty)
        {
            var list = new List<string>();
            int q = int.Parse(qty);
            for (int i = 0; i < q; i++)
            {
                var sn = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(ruleName, SFCDB);
                list.Add(sn);
            }
            return list;
        }
        public string GetSkuFTLabelModelByWO(OleExec SFCDB, string wo)
        {
            var value = "No data record";
            var skuDetail = SFCDB.ORM.Queryable<R_WO_BASE, C_SKU_DETAIL>((r, c) => r.SKUNO == c.SKUNO)
                .Where((r, c) => r.WORKORDERNO == wo && SqlSugar.SqlFunc.ToUpper(c.CATEGORY) == "OFFLINELABEL" && SqlSugar.SqlFunc.ToUpper(c.CATEGORY_NAME) == "FTLABELMODEL")
                .Select((r, c) => c).ToList().FirstOrDefault();
            if(skuDetail!=null)
            {
                value = skuDetail.VALUE.Trim();
            }
            return value;
        }
        public string GetCustomerVersionByWO(OleExec SFCDB, string wo)
        {
            var value = "No data record";
            var ver3 = SFCDB.ORM.Queryable<R_WO_BASE, C_SKU_VER_MAPPING>((r, c) => r.SKUNO == c.FOX_SKUNO && r.SKU_VER == c.FOX_VERSION1)
               .Where((r, c) => r.WORKORDERNO == wo).Select((r, c) => c).ToList().FirstOrDefault();
            if (ver3 != null)
            {
                value = ver3.CUSTOMER_VERSION.Trim();
            }
            return value;
        }
        public string GetSoftwareModelBySku(OleExec SFCDB, string sku)
        {
            var value = "No data record";
            var skuDetail = SFCDB.ORM.Queryable< C_SKU_DETAIL>()
                .Where( c => c.SKUNO== sku && SqlSugar.SqlFunc.ToUpper(c.CATEGORY) == "OFFLINELABEL" && SqlSugar.SqlFunc.ToUpper(c.CATEGORY_NAME) == "SOFTWAREMODEL")
                .ToList().FirstOrDefault();
            if (skuDetail != null)
            {
                value = skuDetail.VALUE.Trim();
            }
            return value;
        }
        public string GetSoftwareVersionBySku(OleExec SFCDB, string sku)
        {
            var value = "No data record";
            var skuDetail = SFCDB.ORM.Queryable<C_SKU_DETAIL>()
                           .Where(c => c.SKUNO == sku && SqlSugar.SqlFunc.ToUpper(c.CATEGORY) == "OFFLINELABEL" && SqlSugar.SqlFunc.ToUpper(c.CATEGORY_NAME) == "SOFTWAREVERSION")
                           .ToList().FirstOrDefault();
            if (skuDetail != null)
            {
                value = skuDetail.VALUE.Trim();
            }
            return value;
        }

        public string GetSoftwareLabelData(OleExec SFCDB,string wo)
        {
            
            DateTime sysdate = SFCDB.ORM.GetDate();
            var yy = sysdate.ToString("yy");
            System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();            
            var ww = gc.GetWeekOfYear(sysdate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();           
            return yy + ww;
        }

        public string GetVersionAndDate(OleExec SFCDB, string version)
        {
            return version + SFCDB.ORM.GetDate().ToString("yyMM");
        }

        public string GetSkuLast8CharByWO(OleExec SFCDB,string wo)
        {
            var value = "No data record";
            var woObj = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r=>r.WORKORDERNO==wo).ToList().FirstOrDefault();
            if (woObj != null)
            {
                value = woObj.SKUNO.Substring(woObj.SKUNO.Length - 8, 8);
            }
            return value;
        }
        public string SavePrintLog(OleExec SFCDB, string SN, string WO, string BU, string EMP_NO, string LABEL_TYPE,string PRINT_TYPE)
        {
            string value = "OK";
            try
            {
                R_PRINT_LOG log = new R_PRINT_LOG()
                {
                    ID = MESDataObject.MesDbBase.GetNewID(SFCDB.ORM, BU, "R_PRINT_LOG"),
                    SN = SN,
                    STATION = WO,
                    LABTYPE = LABEL_TYPE,
                    CTYPE = PRINT_TYPE,
                    EDITTIME = SFCDB.ORM.GetDate(),
                    EDITBY = EMP_NO
                };
                int m = SFCDB.ORM.Insertable(log).ExecuteCommand();
                if (m <= 0)
                {
                    throw new System.Exception("Insert R_PRINT_LOG Fail");
                }
            }
            catch (System.Exception ex)
            {
                value = ex.Message;
            }
            return value;
        }

        public void SavePrintJson(OleExec SFCDB, string SN, string WO, string BU, string EMP_NO, string LABEL_TYPE,byte [] blobdata)
        {
            R_JSON json = SFCDB.ORM.Queryable<R_JSON>()
                .Where(r => r.NAME == SN && r.TYPE == "PrintOffLineLabel" && r.INDEX1 == WO && r.INDEX2 == LABEL_TYPE).ToList().FirstOrDefault();
            if(json!=null)
            {
                json.BLOBDATA = blobdata;
                json.EDIT_EMP = EMP_NO;
                json.EDIT_TIME = SFCDB.ORM.GetDate();
                SFCDB.ORM.Updateable<R_JSON>(json).Where(r => r.ID == json.ID).ExecuteCommand();
            }
            else
            {
                json = new R_JSON();
                json.ID = MESDataObject.MesDbBase.GetNewID(SFCDB.ORM, BU, "R_JSON");
                json.NAME = SN;
                json.BLOBDATA = blobdata;
                json.TYPE = "PrintOffLineLabel";
                json.INDEX1 = WO;
                json.INDEX2 = LABEL_TYPE;
                json.EDIT_EMP = EMP_NO;
                json.EDIT_TIME = SFCDB.ORM.GetDate();
                SFCDB.ORM.Insertable<R_JSON>(json).ExecuteCommand();
            }
        }
    }
}
