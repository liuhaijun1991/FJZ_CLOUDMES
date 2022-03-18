using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.Public
{
    public class OOrderGroup : LabelValueGroup
    {
        public OOrderGroup()
        {
            ConfigGroup = "OOrderGroup";
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetOOrderMainColumnValue",
                    Description = "Get Column Value From o_order_main",
                    Paras = new List<string>() { "WO", "ColumnName" }
                });
            Functions.Add(
                            new LabelValueFunctionConfig()
                            {
                                FunctionName = "GetMainColumnValueByPOAndLine",
                                Description = "Get Column Value From o_order_main By Po And Po Line",
                                Paras = new List<string>() { "PO", "LINE", "ColumnName" }
                            });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetTranIdByPOAndLine",
                    Description = "Get TranId By Po And Po Line",
                    Paras = new List<string>() { "PO", "LINE" }
                });
            
        }

        /// <summary>
        /// 獲取O_ORDER_MAIN對應欄位的值,沒有該欄位則拋異常
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="WO"></param>       
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetOOrderMainColumnValue(OleExec SFCDB, string WO, string ColumnName)
        {
            string output = "";            
            //List<string> listValue = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(r => r.PREWO == WO).Select<string>(ColumnName.ToUpper()).ToList();
            List<string> listValue = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO>((o, r) => o.ID == r.ORIGINALID)
                .Where((o, r) => r.WO == WO && r.VALID == "1").Select((o, r) => o).Select<string>(ColumnName.ToUpper()).ToList();
                        
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            output = listValue[0];
            return output;
        }
        /// <summary>
        /// 獲取O_ORDER_MAIN對應欄位的值,沒有該欄位則拋異常
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="PO"></param>
        /// <param name="LINE"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public string GetMainColumnValueByPOAndLine(OleExec SFCDB, string PO, string LINE,string ColumnName)
        {
            string output = "";
            List<string> listValue = SFCDB.ORM.Queryable<O_ORDER_MAIN, R_ORDER_WO>((o, r) => o.ID == r.ORIGINALID)
                .Where((o, r) => o.PONO == PO && o.POLINE == LINE && r.VALID == "1").Select<string>(ColumnName.ToUpper()).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            output = listValue[0];
            return output;
        }
        public string GetTranIdByPOAndLine(OleExec SFCDB, string PO, string LINE)
        {
            string output = "";
            List<O_I137_ITEM> listValue = SFCDB.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, R_ORDER_WO>((o, r, w) => o.ITEMID == r.ID && w.ORIGINALID == o.ID)
                .Where((o, r, w) => o.PONO == PO && o.POLINE == LINE && w.VALID == "1").Select((o, r, w) => r).ToList();
            if (listValue.Count == 0)
            {
                throw new Exception("Column Name Error!");
            }
            bool bCancel = listValue.Where(r => r.ACTIONCODE == "02").Any();
            if (bCancel)
            {
                throw new Exception($@"{PO},{LINE} Already Cancel!");
            }
            output = listValue.FirstOrDefault().TRANID;
            return output;
        }
    }
}
