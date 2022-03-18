using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.Vertiv
{
    public class OffLineHMILabel: LabelBase
    {
        LabelInputValue inputWO = new LabelInputValue() { Name = "WO", Type = "STRING", Value = "", StationSessionType = "WO", StationSessionKey = "1" };
        LabelInputValue inputQty = new LabelInputValue() { Name = "QTY", Type = "STRING", Value = "", StationSessionType = "QTY", StationSessionKey = "1" };
        LabelInputValue inputEmpNo = new LabelInputValue() { Name = "EMP_NO", Type = "STRING", Value = "", StationSessionType = "", StationSessionKey = "" };

        LabelOutput outputHMISN = new LabelOutput() { Name = "HMISN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = "" };
        LabelOutput outputSKU1 = new LabelOutput() { Name = "SKU1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputVERSION = new LabelOutput() { Name = "VERSION", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
       
        public OffLineHMILabel()
        {
            Inputs.Add(inputWO);
            Inputs.Add(inputQty);
            Inputs.Add(inputEmpNo);
            Outputs.Add(outputHMISN);
            Outputs.Add(outputSKU1);
            Outputs.Add(outputVERSION);           
        }

        public override void MakeLabel(OleExec DB)
        {
            VToffLineLabel offLine = new VToffLineLabel();
            string wo = inputWO.Value.ToString();

            outputSKU1.Value = offLine.GetSkuLast8CharByWO(DB, wo);
            outputVERSION.Value = offLine.GetCustomerVersionByWO(DB, wo);           
            int qty = int.Parse(inputQty.Value.ToString());
            List<string> snList = new List<string>();
            for (int i = 0; i < qty; i++)
            {
                string sn = offLine.GetOffLineSNByRuleName(DB, "HMISNRule");               

                LabelBase labelBase = new LabelBase();
                labelBase.LabelName = this.LabelName;
                labelBase.FileName = this.FileName;
                labelBase.PrintQTY = this.PrintQTY;
                labelBase.PrinterIndex = this.PrinterIndex;
                labelBase.ALLPAGE = this.ALLPAGE;
                labelBase.PAGE = this.PAGE;
                labelBase.Outputs.Add(outputSKU1);
                labelBase.Outputs.Add(outputVERSION);           
                LabelOutput outputSave = new LabelOutput() { Name = "HMISN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = "" };
                outputSave.Value = new List<string>() { sn };
                labelBase.Outputs.Add(outputSave);

                string str = Newtonsoft.Json.JsonConvert.SerializeObject(labelBase, Newtonsoft.Json.Formatting.Indented);

                offLine.SavePrintJson(DB, sn, wo, "VERTIV", inputEmpNo.Value.ToString(), "OffLineHMILabel", Encoding.Unicode.GetBytes(str));
                snList.Add(sn);
            }
            outputHMISN.Value = snList;
        }
    }
}
