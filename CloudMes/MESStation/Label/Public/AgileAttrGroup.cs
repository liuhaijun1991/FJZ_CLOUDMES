using MESDataObject.Module.Juniper;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module.OM;

namespace MESStation.Label.Public
{
    public class AgileAttrGroup : LabelValueGroup
    {
        public AgileAttrGroup()
        {
            ConfigGroup = "AgileAttrGroup";
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetAgileAttrColumnValue",
                    Description = "Get Column Value From o_agile_attr",
                    Paras = new List<string>() { "itemNumber", "rev", "columnName", "PLANT" }
                });
            Functions.Add(
                  new LabelValueFunctionConfig()
                  {
                      FunctionName = "GetUPCLabel2DCode",
                      Description = "Get Juniper UPC Label 2D Code",
                      Paras = new List<string>() { "itemNumber", "rev", "pn", "version", "PLANT", "SN" }
                  });
        }

        /// <summary>
        /// 獲取O_Agile_Attr對應欄位的值,沒有該欄位則拋異常
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="ItemNumber"></param>
        /// <param name="Rev"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetAgileAttrColumnValue(OleExec SFCDB, string itemNumber, string rev, string columnName, string PLANT)
        {
            string output = "";
            //List<string> listValue = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(r => r.ITEM_NUMBER == itemNumber && r.REV== rev).Select<string>(columnName.ToUpper()).ToList();
            //不放版本，分組排序取第一位
            List<string> listValue = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(r => r.ITEM_NUMBER == itemNumber && r.PLANT == PLANT)
                .OrderBy(r => r.DATE_CREATED, SqlSugar.OrderByType.Desc)
                .Select<string>(columnName.ToUpper()).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            if (columnName == "UPC_CODE")
            {
                if (listValue[0] == "" || listValue[0] == null)
                {
                    output = listValue[0] = "NOT DATA";
                }
                else
                {
                    output = listValue[0].Substring(0, 11);
                }

            }
            else
            {
                output = listValue[0];
            }

            return output;
        }

        public string GetUPCLabel2DCode(OleExec SFCDB, string itemNumber, string rev, string pn, string version, string PLANT, string SN)
        {
            string output = "";
            //List<string> listValue = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(r => r.ITEM_NUMBER == itemNumber && r.REV == rev).Select(r=>r.CLEI_CODE).ToList();
            List<string> listValue = SFCDB.ORM.Queryable<O_AGILE_ATTR>().Where(r => r.ITEM_NUMBER == itemNumber && r.PLANT == PLANT)
                .OrderBy(r => r.DATE_CREATED, SqlSugar.OrderByType.Desc)
                .Select(r => r.CLEI_CODE).ToList();

            List<MESDataObject.Module.R_SN_KP> listKP = SFCDB.ORM.Queryable<MESDataObject.Module.R_SN_KP>()
                .Where(r => r.SN == SN && r.VALUE == r.SN && r.VALID_FLAG == 1).OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Asc).ToList();
            string clei = "";
            string upc_pn = "";
            string upc_ver = "";
            //if (listValue.Count == 0)
            //{
            //    throw new Exception("CLEI CODE In O_AGILE_ATTR Error!");
            //}
            //if (string.IsNullOrEmpty(listValue[0]))     
            //{                
            //    clei = "";
            //}
            //else
            //{
            //    clei = listValue[0].Trim();
            //}
            //2D Barcode with (30P) Mfg Part Number, (2P) REV and(7P) CLEI
            //- Part number is identified in eFox WO Creation(see the logic in slide #2)
            //- REV of part number shall be captured by eFox scanning.It shall be 2 digits only
            //- CLEI is pushed from Foxconn Agile and tied to Model Number(TBC)
            //Example: 30P740-98750 2P09 7PCOM7YUM08
            //output = $@"30P{pn} 2P{version} 7P{clei}";

            if (listKP.Count > 0)
            {
                var kp = listKP.FirstOrDefault();
                upc_pn = string.IsNullOrEmpty(kp.MPN) ? "" : kp.MPN;

                var kp_ver = listKP.Find(r => r.EXKEY1 == "REV");
                if (kp_ver != null)
                {
                    upc_ver = string.IsNullOrEmpty(kp.EXVALUE1) ? "" : kp.EXVALUE1;
                }
                var kp_clei = listKP.Find(r => r.EXKEY2 == "CLEI");
                if (kp_clei != null)
                {
                    clei = string.IsNullOrEmpty(kp.EXVALUE2) ? "" : kp.EXVALUE2;
                }
            }
            output = $@"30P{upc_pn} 2P{upc_ver} 7P{clei}";
            return output;
        }
    }
}
