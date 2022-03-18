using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Label
{
    /// <summary>
    /// 輸入參數：PARENTPACKNO 1 棧板號
    /// 
    /// 輸出參數：CARTONS 所有的Carton
    /// </summary>
    public class BPD_Pallet: LabelBase
    {
        T_R_PACKING PackingTool = null;
        T_C_SKU TCK = null;
        LabelInputValue I_PALLET = new LabelInputValue() { Name = "PALLET", Type = "string", StationSessionType = "PARENTPACKNO", StationSessionKey = "1", Value = "" };

        LabelOutput O_PALLET = new LabelOutput() { Name = "PALLET", Type = LabelOutPutTypeEnum.String, Description = "Pallet Number", Value = "" };
        LabelOutput O_SKU = new LabelOutput() { Name = "SKU", Type = LabelOutPutTypeEnum.String, Description = "Sku number", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "VERSION", Type = LabelOutPutTypeEnum.String, Description = "Sku version", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "Quantity of products", Value = "" };
        LabelOutput O_CARTONLIST = new LabelOutput() { Name = "CARTONS", Type = LabelOutPutTypeEnum.StringArry, Description = "Carton in Pallet", Value = new List<string>() };

        public BPD_Pallet()
        {
            this.Inputs.Add(I_PALLET);
            this.Outputs.Add(O_CARTONLIST);
            this.Outputs.Add(O_SKU);
            this.Outputs.Add(O_VER);
            this.Outputs.Add(O_QTY);
            this.Outputs.Add(O_PALLET);
        }

        public override void MakeLabel(OleExec DB)
        {
            PackingTool = new T_R_PACKING(DB, DB_TYPE_ENUM.Oracle);
            TCK = new T_C_SKU(DB, DB_TYPE_ENUM.Oracle);
            
            //int qty = 0;
            string PalletNo = string.Empty;
            List<R_SN> SnList = new List<R_SN>();
            if (I_PALLET.Value != null && I_PALLET.Value.ToString().Length > 0)
            {
                PalletNo = I_PALLET.Value.ToString();
                O_PALLET.Value = PalletNo;
                PackingTool.GetSnListByPackNo(PalletNo,ref SnList, DB);
                //PackingTool.GetQtyByPackNo(PalletNo, ref qty, DB);
                O_QTY.Value = SnList.Count;
                List<R_PACKING> Packs = PackingTool.GetChildPacks(PalletNo, DB); 
                if (Packs.Count > 0)
                {
                    O_SKU.Value = Packs[0].SKUNO;
                    C_SKU Sku = TCK.GetSku(Packs[0].SKUNO,DB);
                    O_VER.Value = Sku.VERSION;
                    O_CARTONLIST.Value = Packs.Select(t => t.PACK_NO).ToList();
                }
            }
        }
    }
}
