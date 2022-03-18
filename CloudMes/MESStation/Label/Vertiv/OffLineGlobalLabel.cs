using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.Vertiv
{
    public class OffLineGlobalLabel : LabelBase
    {
        LabelInputValue inputWO = new LabelInputValue() { Name = "WO", Type = "STRING", Value = "", StationSessionType = "WO", StationSessionKey = "1" };
        LabelInputValue inputQty = new LabelInputValue() { Name = "QTY", Type = "STRING", Value = "", StationSessionType = "QTY", StationSessionKey = "1" };
        LabelInputValue inputEmpNo = new LabelInputValue() { Name = "EMP_NO", Type = "STRING", Value = "", StationSessionType = "", StationSessionKey = "" };

        LabelOutput outputSn = new LabelOutput() { Name = "GLOBALSN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = "" };
        public OffLineGlobalLabel()
        {
            Inputs.Add(inputWO);
            Inputs.Add(inputQty);
            Inputs.Add(inputEmpNo);
            Outputs.Add(outputSn);
        }
        public override void MakeLabel(OleExec DB)
        {
            VToffLineLabel offLine = new VToffLineLabel();
            string wo = inputWO.Value.ToString();
            int qty = int.Parse(inputQty.Value.ToString());
            List<string> snList = new List<string>();
            for (int i = 0; i < qty; i++)
            {
                string sn = offLine.GetGlobalSNByWO(DB, wo, "GlobalLabelSnRule");               

                LabelBase labelBase = new LabelBase();
                labelBase.LabelName = this.LabelName;
                labelBase.FileName = this.FileName;
                labelBase.PrintQTY = this.PrintQTY;
                labelBase.PrinterIndex = this.PrinterIndex;
                labelBase.ALLPAGE = this.ALLPAGE;
                labelBase.PAGE = this.PAGE;
                LabelOutput outputSave = new LabelOutput() { Name = "GLOBALSN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = "" };
                outputSave.Value = new List<string>() { sn };
                labelBase.Outputs.Add(outputSave);

                string str = Newtonsoft.Json.JsonConvert.SerializeObject(labelBase, Newtonsoft.Json.Formatting.Indented);

                offLine.SavePrintJson(DB, sn, wo, "VERTIV", inputEmpNo.Value.ToString(), "OffLineGlobalLabel", Encoding.Unicode.GetBytes(str));
                snList.Add(sn);
            }
            outputSn.Value = snList;
        }
    }
}
