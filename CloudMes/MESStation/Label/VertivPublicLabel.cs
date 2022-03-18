using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.MESStation.Label;

namespace MESStation.Label
{
    public class VertivPublicLabel : LabelBase
    {
        LabelInputValue I_SN= new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_WO = new LabelOutput() { Name = "WO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKU = new LabelOutput() { Name = "WO_SKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "WO_VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput C_SKU = new LabelOutput() { Name = "SKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput C_VER = new LabelOutput() { Name = "SKU_VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_DATE = new LabelOutput() { Name = "DATE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        public VertivPublicLabel()
        {
            Inputs.Add(I_SN);
            Outputs.Add(O_WO);
            Outputs.Add(O_SKU);
            Outputs.Add(O_VER);
            Outputs.Add(C_SKU);
            Outputs.Add(C_VER);
            Outputs.Add(O_QTY);
            Outputs.Add(O_DATE);
        }
        T_R_SN t_r_sn;
        public override void MakeLabel(OleExec DB)
        {

            t_r_sn = new T_R_SN(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            LogicObject.SN snObj;
            LogicObject.WorkOrder wo = new LogicObject.WorkOrder();
            
            R_SN r_sn;
            if (I_SN.Value is string)
            {
                r_sn = t_r_sn.LoadSN(I_SN.Value.ToString(), DB);
            }
            else if (typeof(LogicObject.SN) == I_SN.Value.GetType())
            {
                snObj = (LogicObject.SN)I_SN.Value;
                r_sn = t_r_sn.LoadSN(snObj.SerialNo, DB);
            }
            else if (typeof(R_SN) == I_SN.Value.GetType())
            {
                r_sn = (R_SN)I_SN.Value;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { I_SN.Value.ToString() }));
            }

            if (r_sn == null)
            {
                return;
            }

            wo.Init(r_sn.WORKORDERNO, DB, DB_TYPE_ENUM.Oracle);
            C_SKU c_sku = new T_C_SKU(DB, DB_TYPE_ENUM.Oracle).GetSku(wo.SkuNO, DB);
            
            O_SN.Value = r_sn.SN;
            O_WO.Value = wo.WorkorderNo;
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