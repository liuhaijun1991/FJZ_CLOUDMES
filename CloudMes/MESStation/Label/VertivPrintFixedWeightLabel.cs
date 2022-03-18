using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label
{
    public class VertivPrintFixedWeightLabel : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_WEIGHT_LBS = new LabelOutput() { Name = "LBS", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_WEIGHT_KG = new LabelOutput() { Name = "KG", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        public VertivPrintFixedWeightLabel()
        {
            Inputs.Add(I_SN);
            Outputs.Add(O_VER);
            Outputs.Add(O_QTY);
            Outputs.Add(O_WEIGHT_LBS);
            Outputs.Add(O_WEIGHT_KG);           
        }
        public override void MakeLabel(OleExec DB)
        {
            //Packing.PalletBase pallet = new Packing.PalletBase(I_PALLETNO.Value.ToString(), DB);
            string sn1 = I_SN.Value.ToString();
            R_SN snObj = DB.ORM.Queryable<R_SN>().Where(r => r.SN == sn1 && r.VALID_FLAG == "1").ToList().FirstOrDefault();
            if (snObj == null)
            {
                throw new Exception($@"{I_SN.Value.ToString()} Not Exist!");
            }
            List<string> snList = DB.ORM.Queryable<R_SN_PACKING, R_SN_PACKING>((rs, sp) => rs.PACK_ID == sp.PACK_ID).Where((rs, sp) => rs.SN_ID == snObj.ID).Select((rs, sp) => sp.SN_ID).ToList();
            if (snList.Count == 0)
            {
                throw new Exception("Carton Is Empty");
            }
            C_SKU_VER_MAPPING verMapping = new C_SKU_VER_MAPPING();
            T_C_SKU_VER_MAPPING t_c_sku_ver_mapping = new T_C_SKU_VER_MAPPING(DB, DB_TYPE_ENUM.Oracle);            
            LogicObject.WorkOrder wo = new LogicObject.WorkOrder();
            wo.Init(snObj.WORKORDERNO, DB, DB_TYPE_ENUM.Oracle);            
            O_QTY.Value = ((List<string>)snList).Count.ToString();
          
            verMapping = t_c_sku_ver_mapping.GetMappingBySkuAndVersion(wo.SkuNO, wo.SKU_VER, DB);
            if (verMapping != null)
            {
                O_VER.Value = verMapping.FOX_VERSION2;
            }
            else
            {
                O_VER.Value = wo.SKU_VER;              
            }
            switch (O_QTY.Value.ToString())
            {
                case "1":
                    O_WEIGHT_LBS.Value = "1.17";
                    O_WEIGHT_KG.Value = "0.64";
                    break;
                case "2":
                    O_WEIGHT_LBS.Value = "1.56";
                    O_WEIGHT_KG.Value = "0.82";
                    break;
                case "3":
                    O_WEIGHT_LBS.Value = "1.95";
                    O_WEIGHT_KG.Value = "1.0";
                    break;
                case "4":
                    O_WEIGHT_LBS.Value = " 2.34";
                    O_WEIGHT_KG.Value = "1.18";
                    break;
                case "5":
                    O_WEIGHT_LBS.Value = " 2.73";
                    O_WEIGHT_KG.Value = "1.36";
                    break;
                case "6":
                    O_WEIGHT_LBS.Value = "3.12";
                    O_WEIGHT_KG.Value = "1.54";
                    break;
                case "7":
                    O_WEIGHT_LBS.Value = "3.51";
                    O_WEIGHT_KG.Value = "1.72";
                    break;
                case "8":
                    O_WEIGHT_LBS.Value = "3.9";
                    O_WEIGHT_KG.Value = "1.9";
                    break;
                default:
                    throw new Exception("Pallet Qty Error");
            }
        }
    }
}
