using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation.Label;

namespace MESStation.Label
{
    public class TestLabel2 : LabelBase
    {
        LabelInputValue I_SKU = new LabelInputValue() { Name = "SKU", Type = "STRING", Value = "" , StationSessionType="SKU", StationSessionKey="1" };
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_MAC = new LabelOutput() { Name = "MAC", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_CARTION = new LabelOutput() { Name = "CARTION", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        public TestLabel2()
        {
            this.Inputs.Add(I_SKU);

            Inputs.Add(I_SN);

            Outputs.Add(O_SN);
            Outputs.Add(O_MAC);
            Outputs.Add(O_CARTION);
            Outputs.Add(O_SKUNO);
            Outputs.Add(O_QTY);
        }
        public override void MakeLabel(OleExec DB)
        {
            this.LabelName = "TESTLAB1";
            this.PrintQTY = 1;
            List<string> SNS = (List<string>)O_SN.Value;
            SNS.Add("SN1");
            SNS.Add("SN2");
            SNS.Add("SN3");
            List<string> MACS = (List<string>)O_MAC.Value;
            MACS.Add("DE:12:45:19");
            MACS.Add("DE:12:45:29");
            MACS.Add("DE:12:45:39");
            O_CARTION.Value = "C0000001";
            O_SKUNO.Value = "XXXXXXX";
            O_QTY.Value = "3";
        }
    }
}
