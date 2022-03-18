using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System.Collections.Generic;

namespace MESStation.Label.Public
{
    public class SKUValueGroup : LabelValueGroup
    {
        public SKUValueGroup()
        {
            ConfigGroup = "SKUValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSKU", Description = "获取SKUNO", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetC_SKU_VALUE", Description = "获取C_SKU表的字段值", Paras = new List<string>() { "SKUNO", "COLUMN_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetC_SKU_DETAIL_VALUE", Description = "获取C_SKU_DETAIL对应CATEGORY_NAME的VALUE字段值", Paras = new List<string>() { "SKUNO", "CATEGORY_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetC_SKU_VERSION_MAPPING", Description = "获取SKUNO版本对应关系", Paras = new List<string>() { "SKUNO" ,"VERSION"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSKU_VERSION", Description = "获取SKUNO版本", Paras = new List<string>() { "SKUNO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GET_CHARGE_TEST", Description = "获取CHARGE抽測標誌", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GET_MINILTT_TEST", Description = "获取MINI-LTT抽測標誌", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "Get_SKU_FROM_DETAIL", Description = "Get_sku_r_staion_detail", Paras = new List<string>() { "SN" } });
        }

        public string GetSKU(OleExec SFCDB, string SN)
        {
            string SKUNO = "No data record";
            List<R_SN> SNLIST = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").ToList();
            if (SNLIST.Count > 0)
            {
                SKUNO = SNLIST[0].SKUNO;
            }
            return SKUNO;
        }

        public string GetC_SKU_VALUE(OleExec SFCDB, string SKU, string COLUMN_NAME)
        {
            string value = "No data record";
            List<string> sl = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SKU).Select<string>(COLUMN_NAME).ToList();
            if (sl.Count > 0)
            {
                value = sl[0];
            }
            return value;
        }
        public string GET_CHARGE_TEST(OleExec SFCDB, string SN)
        {
            string value = "No data record";
            List<R_SN_LOG> sl = SFCDB.ORM.Queryable<R_SN_LOG>().Where(t => t.SN == SN && t.FLAG == "Y" && t.LOGTYPE == "CHARGE-SAMPLE").ToList();
            if (sl.Count > 0)
            {
                value = "Dis-Charge    Re-Charge";
            }
            else
            {
                value = "";
            }
            return value;
        }

        public string GET_MINILTT_TEST(OleExec SFCDB, string SN)
        {
            string value = "No data record";
            List<R_SN_LOG> sl = SFCDB.ORM.Queryable<R_SN_LOG>().Where(t => t.SN == SN && t.FLAG == "Y" && t.LOGTYPE == "MINI-LTT-SAMPLE").ToList();
            if (sl.Count > 0)
            {
                value = "MINI-LTT";
            }
            else
            {
                value = "";
            }
            return value;
        }

        public string GetC_SKU_VERSION_MAPPING(OleExec SFCDB, string SKU, string FoxVersion)
        {
            var s = SFCDB.ORM.Queryable<C_SKU_VER_MAPPING>()
            .Where(t => t.FOX_SKUNO == SKU && (t.FOX_VERSION1 == FoxVersion || t.FOX_VERSION2 == FoxVersion))
            .Select(t => t.CUSTOMER_VERSION)
            .First();
            return s == null ? "" : s;
        }
        public string GetSKU_VERSION(OleExec SFCDB, string SKU)
        {
            var s = SFCDB.ORM.Queryable<C_SKU>()
            .Where(t =>t.SKUNO== SKU)
            .Select(t => t.VERSION)
            .First();
            return s == null ? "" : s;
        }

        public string GetC_SKU_DETAIL_VALUE(OleExec SFCDB, string SKU, string CATEGORY_NAME)
        {
            string value = "No data record";
            List<string> sl = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKU && t.CATEGORY_NAME == CATEGORY_NAME).Select(t => t.VALUE).ToList();
            if (sl.Count > 0)
            {
                value = sl[0];
            }
            return value;
        }
        public string Get_SKU_FROM_DETAIL(OleExec SFCDB, string SN)
        {
            string SKUNO = "No data record";
            List<R_SN_DETAIL> SNLIST = SFCDB.ORM.Queryable<R_SN_DETAIL>().Where(t => t.SN == SN ).ToList();
            if (SNLIST.Count > 0)
            {
                SKUNO = SNLIST[0].SKUNO;
            }
            return SKUNO;
        }
    }
}
