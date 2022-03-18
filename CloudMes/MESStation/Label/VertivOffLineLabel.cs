using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.MESStation.Label;
using MESPubLab.MESStation;

namespace MESStation.Label
{
    public class VertivOffLineLabel : LabelBase
    {
        LabelInputValue I_WO = new LabelInputValue() { Name = "WO", Type = "STRING", Value = "", StationSessionType = "WO", StationSessionKey = "1" };
        LabelInputValue I_QTY = new LabelInputValue() { Name = "QTY", Type = "STRING", Value = "", StationSessionType = "QTY", StationSessionKey = "1" };

        LabelOutput O_SKU = new LabelOutput() { Name = "WO_SKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "WO_VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput C_SKU = new LabelOutput() { Name = "SKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput C_VER = new LabelOutput() { Name = "SKU_VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_DATE = new LabelOutput() { Name = "DATE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        public VertivOffLineLabel()
        {
            Inputs.Add(I_WO);
            Inputs.Add(I_QTY);
            Outputs.Add(O_SKU);
            Outputs.Add(O_VER);
            Outputs.Add(C_SKU);
            Outputs.Add(C_VER);
            Outputs.Add(O_QTY);
            Outputs.Add(O_DATE);
        }
        public override void MakeLabel(OleExec DB)
        {
            //页面输入打印Label数量
            PrintQTY = Convert.ToInt32(I_QTY.Value.ToString());
            LogicObject.WorkOrder wo = new LogicObject.WorkOrder();

            wo.Init(I_WO.Value.ToString(), DB, DB_TYPE_ENUM.Oracle);
            C_SKU c_sku = new T_C_SKU(DB, DB_TYPE_ENUM.Oracle).GetSku(wo.SkuNO, DB);
            O_SKU.Value = wo.SkuNO;
            O_VER.Value = wo.SKU_VER;
            O_QTY.Value = wo.WORKORDER_QTY;
            C_SKU.Value = c_sku.SKUNO;
            C_VER.Value = c_sku.VERSION;

            T_C_PACKING odate = new T_C_PACKING(DB, DB_TYPE_ENUM.Oracle);
            O_DATE.Value = odate.GetDBDateTime(DB).ToString("MM/dd/yyyy");
        }
    }
}
