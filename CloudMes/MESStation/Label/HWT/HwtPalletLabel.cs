using MESDataObject.Module;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;

namespace MESStation.Label.HWT
{
    public class HwtPalletLabel : LabelBase
    {
        LabelInputValue I_PALLETNO = new LabelInputValue() { Name = "PLNO", Type = "STRING", Value = "", StationSessionType = "PRINT_PL", StationSessionKey = "1" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_DATE = new LabelOutput() { Name = "DATE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CVER = new LabelOutput() { Name = "CVER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PLNO = new LabelOutput() { Name = "PLNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        public HwtPalletLabel()
        {
            Inputs.Add(I_PALLETNO);
            Outputs.Add(O_VER);
            Outputs.Add(O_QTY);
            Outputs.Add(O_SKUNO);
            Outputs.Add(O_DATE);
            Outputs.Add(O_CVER);
            Outputs.Add(O_PLNO);
        }
        public override void MakeLabel(OleExec DB)
        {
            //base.MakeLabel(DB);

            Packing.PalletBase pallet = new Packing.PalletBase(I_PALLETNO.Value.ToString(), DB);
            List<string> snList = pallet.GetSNList(DB);
            C_SKU_VER_MAPPING verMapping = new C_SKU_VER_MAPPING();
            T_C_SKU_VER_MAPPING t_c_sku_ver_mapping = new T_C_SKU_VER_MAPPING(DB, DB_TYPE_ENUM.Oracle);
            if (snList.Count == 0)
            {
                throw new Exception("Pallet is empty");
            }

            LogicObject.SN sn = new LogicObject.SN(snList[0], DB, DB_TYPE_ENUM.Oracle);
            LogicObject.WorkOrder wo = new LogicObject.WorkOrder();
            wo.Init(sn.WorkorderNo, DB, DB_TYPE_ENUM.Oracle);
            //O_VER.Value = wo.SKU_VER;
            O_QTY.Value = ((List<string>)snList).Count.ToString();
            O_SKUNO.Value = "PARTNO: " + wo.SkuNO;
            verMapping = t_c_sku_ver_mapping.GetMappingBySkuAndVersion(wo.SkuNO, wo.SKU_VER, DB);
            if (verMapping != null)
            {
                O_VER.Value = verMapping.FOX_VERSION2;
                O_CVER.Value = verMapping.CUSTOMER_VERSION;
            }
            else
            {
                O_VER.Value = wo.SKU_VER;
                O_CVER.Value = wo.SKU_VER;
            }
            T_C_PACKING odate = new T_C_PACKING(DB, DB_TYPE_ENUM.Oracle);
            O_DATE.Value = odate.GetDBDateTime(DB).ToString("MM/dd/yyyy");
            O_PLNO.Value = I_PALLETNO.Value.ToString();
        }
    }
}
