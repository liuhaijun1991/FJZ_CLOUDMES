using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.Vertiv
{
    public class OffLineFTLabel : LabelBase
    {
        LabelInputValue inputWO = new LabelInputValue() { Name = "WO", Type = "STRING", Value = "", StationSessionType = "WO", StationSessionKey = "1" };
        LabelInputValue inputQty = new LabelInputValue() { Name = "QTY", Type = "STRING", Value = "", StationSessionType = "QTY", StationSessionKey = "1" };
        LabelInputValue inputEmpNo = new LabelInputValue() { Name = "EMP_NO", Type = "STRING", Value = "", StationSessionType = "", StationSessionKey = "" };

        LabelOutput outputFTSN = new LabelOutput() { Name = "FTSN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = "" };
        LabelOutput outputRUSKU = new LabelOutput() { Name = "RUSKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputVERSION1 = new LabelOutput() { Name = "VERSION1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputDATE = new LabelOutput() { Name = "DATE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        public OffLineFTLabel()
        {
            Inputs.Add(inputWO);
            Inputs.Add(inputQty);
            Inputs.Add(inputEmpNo);
            Outputs.Add(outputFTSN);
            Outputs.Add(outputRUSKU);
            Outputs.Add(outputVERSION1);
            Outputs.Add(outputDATE);
        }

        public override void MakeLabel(OleExec DB)
        {
            VToffLineLabel offLine = new VToffLineLabel();
            string wo = inputWO.Value.ToString();

            outputRUSKU.Value = offLine.GetSkuFTLabelModelByWO(DB, wo);
            outputVERSION1.Value = offLine.GetCustomerVersionByWO(DB, wo);
            outputDATE.Value = DB.ORM.GetDate().ToString("yyyyMMdd");
            int qty = int.Parse(inputQty.Value.ToString());
            List<string> snList = new List<string>();
            for (int i = 0; i < qty; i++)
            {
                string sn = offLine.GetOffLineSNByRuleName(DB, "FTSNRule");

                LabelBase labelBase = new LabelBase();
                labelBase.LabelName = this.LabelName;
                labelBase.FileName = this.FileName;
                labelBase.PrintQTY = this.PrintQTY;
                labelBase.PrinterIndex = this.PrinterIndex;
                labelBase.ALLPAGE = this.ALLPAGE;
                labelBase.PAGE = this.PAGE;
                labelBase.Outputs.Add(outputRUSKU);
                labelBase.Outputs.Add(outputVERSION1);
                labelBase.Outputs.Add(outputDATE);
                LabelOutput outputSave = new LabelOutput() { Name = "FTSN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = "" };
                outputSave.Value = new List<string>() { sn };
                labelBase.Outputs.Add(outputSave);

                string str = Newtonsoft.Json.JsonConvert.SerializeObject(labelBase, Newtonsoft.Json.Formatting.Indented);

                offLine.SavePrintJson(DB, sn, wo, "VERTIV", inputEmpNo.Value.ToString(), "OffLineFTLabel", Encoding.Unicode.GetBytes(str));
                snList.Add(sn);
            }
            outputFTSN.Value = snList;
        }
    }
}
