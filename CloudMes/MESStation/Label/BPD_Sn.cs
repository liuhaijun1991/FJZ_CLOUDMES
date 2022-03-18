using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESStation.LogicObject;

namespace MESStation.Label
{
    public class BPD_Sn: LabelBase
    {
        private LabelInputValue I_WO = new LabelInputValue() { Name = "WO", StationSessionType = "WO", StationSessionKey = "1", Type = "STRING", Value = "" };
        private LabelInputValue I_SN = new LabelInputValue() { Name = "SN", StationSessionType = "SN", StationSessionKey = "1", Type = "STRING", Value = "" };

        private LabelOutput O_SKU = new LabelOutput() { Name = "SKU", Type = LabelOutPutTypeEnum.String, Value = "", Description = "機種" };
        private LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Value = "", Description = "版本" };
        private LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Value = "", Description = "SN" };
        public BPD_Sn()
        {
            this.Inputs.Add(I_WO);
            this.Inputs.Add(I_SN);

            this.Outputs.Add(O_SKU);
            this.Outputs.Add(O_VER);
            this.Outputs.Add(O_SN);
        }


        public override void MakeLabel(OleExec DB)
        {
            string WO = I_WO.Value.ToString();
            string SN = I_SN.Value.ToString();

            WorkOrder WoObject = new WorkOrder();
            WoObject.Init(WO, DB);
            O_SKU.Value = WoObject.SkuNO;
            O_VER.Value = WoObject.SKU_VER;
            O_SN.Value = SN;
        }
    }
}
