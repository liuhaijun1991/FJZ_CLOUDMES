using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject;
using System.Data;

namespace MESStation.Label.HWD
{
    public class PrintLab : LabelBase
    {
        LabelInputValue inputSku = new LabelInputValue() { Name = "SKU", Type = "STRING", Value = "", StationSessionType = "SKU", StationSessionKey = "1" };
        LabelOutput outputSN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputSkuno = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputSkuName = new LabelOutput() { Name = "SKUNAME", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputCustPartno = new LabelOutput() { Name = "CUSTPARTNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput outputSubSN = new LabelOutput() { Name = "SUBSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        public PrintLab() {
            this.Inputs.Add(inputSku);
            this.Outputs.Add(outputSkuno);
            this.Outputs.Add(outputSkuName);
            this.Outputs.Add(outputCustPartno);
            this.Outputs.Add(outputSN);
            this.Outputs.Add(outputSubSN);
        }

        public override void MakeLabel(OleExec DB)
        {
            //base.MakeLabel(DB);
            try
            {
                string skuno = inputSku.Value.ToString();
                C_SKU skuObject = DB.ORM.Queryable<C_SKU>().Where(k => k.SKUNO == skuno).ToList().FirstOrDefault();
                if (skuObject == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { skuno }));
                }
                if (skuObject.CUST_PARTNO.ToString() == "")
                {
                    throw new MESReturnMessage($@"{skuObject.SKUNO}  CUST_PARTNO Is Null!");
                }
                if (skuObject.SKU_NAME.ToString() == "")
                {
                    throw new MESReturnMessage($@"{skuObject.SKUNO}  SKU_NAME Is Null!");
                }
                string CUSTPARTNO = skuObject.CUST_PARTNO;
                string PX = "DW" + CUSTPARTNO.Substring(CUSTPARTNO.Length - 4);
                string sn = MakeSn.GetSN(PX, DB);
                outputSkuName.Value = skuObject.SKU_NAME;
                outputCustPartno.Value = CUSTPARTNO.Substring(CUSTPARTNO.Length - 3, 3);
                outputSkuno.Value = skuObject.SKUNO.Substring(0, 1) == "A" ? "BS" : "CS";
                outputSN.Value = sn;
                outputSubSN.Value = sn.Substring(sn.Length - 6);
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

    }
}
