using MESDBHelper;
using MESPubLab.MESStation.Label;
using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.Public
{
    public class CustomerPoValueGroup : LabelValueGroup
    {
        public CustomerPoValueGroup()
        {
            ConfigGroup = "CustomerPoValueGroup";
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetCustomerPoByDN",
                    Description = "Get Customer Po By DN From r_function_control",
                    Paras = new List<string>() { "DN" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetNotPrintFlagByDN",
                    Description = "Get NotPrint Flag By DN And Label Name",
                    Paras = new List<string>() { "DN", "LABEL_NAME" }
                });
            Functions.Add(
                new LabelValueFunctionConfig()
                {
                    FunctionName = "GetCustomerPoQtyByDN",
                    Description = "Get Customer Po Qty By DN From r_function_control",
                    Paras = new List<string>() { "DN","PALLETNO" }
                });
        }

        public string GetCustomerPoByDN(OleExec SFCDB, string DN)
        {            
            string output = "";            
            List<R_F_CONTROL_EX> listQty = new List<R_F_CONTROL_EX>();            
            var poObj = SFCDB.ORM.Queryable<R_F_CONTROL>()
                .Where(r => r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.FUNCTIONNAME == "PalletList_Values" && r.CATEGORY == "DN Info" && r.VALUE == DN)
                .ToList().FirstOrDefault();
            if (poObj != null)
            {
                output = poObj.EXTVAL;               
                
            }           
            return output;
        }

        public string GetNotPrintFlagByDN(OleExec SFCDB, string DN,string LABEL_NAME)
        {
            string NotPrint = "";
            List<R_F_CONTROL_EX> listQty = new List<R_F_CONTROL_EX>();
            var poObj = SFCDB.ORM.Queryable<R_F_CONTROL>()
                .Where(r => r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.FUNCTIONNAME == "PalletList_Values" && r.CATEGORY == "DN Info" && r.VALUE == DN)
                .ToList().FirstOrDefault();

            var categoyLabel = SFCDB.ORM.Queryable<R_F_CONTROL>()
                .Where(r => r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "SYSTEM" && r.FUNCTIONNAME == "PalletList_Values" && r.CATEGORY == "DN Info")
                .ToList().FirstOrDefault();
            string labelName = categoyLabel == null ? "" : categoyLabel.CATEGORYDEC;

            if (poObj != null)
            {                
                if (LABEL_NAME == labelName)
                {
                    NotPrint = "FALSE";
                }
                else
                {
                    NotPrint = "TRUE";
                }
            } 
            else
            {
                if (LABEL_NAME == labelName)
                {
                    NotPrint = "TRUE";
                }
                else
                {
                    NotPrint = "FALSE";
                }                
            }
            return NotPrint;
        }


        public string GetCustomerPoQtyByDN(OleExec SFCDB, string DN,string PALLETNO)
        {
            string output = "";

            List<R_F_CONTROL_EX> listQty = new List<R_F_CONTROL_EX>();
            string start_qty = "", end_qty = "", total_qty = "";
            var poObj = SFCDB.ORM.Queryable<R_F_CONTROL>()
                .Where(r => r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.FUNCTIONNAME == "PalletList_Values" && r.CATEGORY == "DN Info" && r.VALUE == DN)
                .ToList().FirstOrDefault();
            if (poObj != null)            {
               
                listQty = SFCDB.ORM.Queryable<R_F_CONTROL_EX>().Where(r => r.DETAIL_ID == poObj.ID).ToList();

            }
            if (listQty.Count > 0)
            {
                start_qty = listQty.Find(r => r.NAME == "Start Number") == null ? "" : listQty.Find(r => r.NAME == "Start Number").VALUE;
                end_qty = listQty.Find(r => r.NAME == "End Number") == null ? "" : listQty.Find(r => r.NAME == "End Number").VALUE;
                total_qty = listQty.Find(r => r.NAME == "Total Qty") == null ? "" : listQty.Find(r => r.NAME == "Total Qty").VALUE;
                List<R_SHIP_DETAIL> listShipping = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN, R_SHIP_DETAIL>
                    ((pallet, carton, rsp, sn, rsd) => pallet.ID == carton.PARENT_PACK_ID && carton.ID == rsp.PACK_ID && rsp.SN_ID == sn.ID && sn.SN == rsd.SN)
                    .Where((pallet, carton, rsp, sn, rsd) => pallet.PACK_NO == PALLETNO && sn.VALID_FLAG == "1")
                    .Select((pallet, carton, rsp, sn, rsd) => rsd)
                    .Distinct().ToList();
                if (!string.IsNullOrEmpty(total_qty))
                {
                    if (string.IsNullOrEmpty(start_qty) && string.IsNullOrEmpty(end_qty))
                    {
                        output += $@"0001-{listShipping.Count.ToString().PadLeft(4, '0')} / {total_qty}";
                    }
                    else if (!string.IsNullOrEmpty(start_qty) && start_qty != "0")
                    {
                        output += $@"{(Convert.ToInt32(start_qty)).ToString().PadLeft(4, '0')}-{(Convert.ToInt32(start_qty) - 1 + listShipping.Count).ToString().PadLeft(4, '0')} / {total_qty}";
                    }
                }
            }            
            return output;
        }

    }
}
