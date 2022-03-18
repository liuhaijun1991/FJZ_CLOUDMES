using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation.Label;
using MESDBHelper;
using MESStation.LogicObject;
using MESDataObject.Module;

namespace MESStation.Label.ORACLE
{
    public class ORA_LOT_SILOADING : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "string", StationSessionType = "SN", StationSessionKey = "1", Value = "" };

        LabelOutput O_WO = new LabelOutput() { Name = "WO", Type = LabelOutPutTypeEnum.String, Description = "WorkorderNO", Value = "" };
        LabelOutput O_SKU = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "SKUNO", Value = "" };
        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Description = "SN", Value = null };
        LabelOutput O_DESC = new LabelOutput() { Name = "DESC", Type = LabelOutPutTypeEnum.String, Description = "DESC", Value = null };

        public ORA_LOT_SILOADING()
        {
            this.Inputs.Add(I_SN);
            this.Outputs.Add(O_WO);
            this.Outputs.Add(O_SKU);
            this.Outputs.Add(O_SN);
            this.Outputs.Add(O_DESC);

        }
        public override void MakeLabel(OleExec DB)
        {

            string strSN = I_SN.Value.ToString();
            SN SN = new SN(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);

            string strWO = SN.WorkorderNo;
            WorkOrder WO = new WorkOrder();
            WO.Init(strWO, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            O_WO.Value = WO.WorkorderNo;
            O_SKU.Value = WO.SkuNO;
            O_SN.Value = SN.baseSN.SN;

            SKU sku = new SKU();
            sku.InitBySn(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            O_DESC.Value = sku.Description;
        }
    }
}
