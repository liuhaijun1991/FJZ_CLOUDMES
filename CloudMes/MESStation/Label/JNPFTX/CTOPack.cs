using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.MESStation.Label;

namespace MESStation.Label.JNPFTX
{
    class CTOPack : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        //CTO TCIF label
        LabelOutput O_PNO = new LabelOutput() { Name = "PNO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CLEI = new LabelOutput() { Name = "Clei", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PID = new LabelOutput() { Name = "PID", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_Qty = new LabelOutput() { Name = "Qty", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_Ver = new LabelOutput() { Name = "Ver", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PN = new LabelOutput() { Name = "PN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SO = new LabelOutput() { Name = "SO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CPN = new LabelOutput() { Name = "CPN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_Date = new LabelOutput() { Name = "Date", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_COO = new LabelOutput() { Name = "COO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };

        //CTO Child Label
        LabelOutput C_SN = new LabelOutput() { Name = "SN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput C_PNO = new LabelOutput() { Name = "PNO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput C_CPN = new LabelOutput() { Name = "CPN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput C_CLEI = new LabelOutput() { Name = "Clei", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };


        public CTOPack()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);
            Outputs.Add(O_PNO);
            Outputs.Add(O_CLEI);
            Outputs.Add(O_PID);
            Outputs.Add(O_SN);
            Outputs.Add(O_Qty);
            Outputs.Add(O_Ver);
            Outputs.Add(O_PN);
            Outputs.Add(O_SO);
            Outputs.Add(O_CPN);
            Outputs.Add(O_Date);
            Outputs.Add(O_COO);

            Outputs.Add(C_SN);
            Outputs.Add(C_PNO);
            Outputs.Add(C_CPN);
            Outputs.Add(C_CLEI);
        }

        C_MMPRODMASTER c_mmprodmaster = null;
        ASBUILTITEM Asbuild_item = null;
        ASBUILTHEAD Asbuild_head = null;
        PO_ITEM po_item = null;


        

        T_R_SN t_r_sn;
        T_R_SN_KP t_r_sn_kp;
        T_C_SKU_VER_MAPPING t_c_sku_ver_mapping;

        public override void MakeLabel(OleExec DB)
        {
            //base.MakeLabel(DB);
            t_r_sn = new T_R_SN(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            C_SKU_Label labelName = null;
            LogicObject.SN snObj;
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
            labelName = DB.ORM.Queryable<C_SKU_Label>().Where(l => l.SKUNO == r_sn.SKUNO && l.STATION == I_STATION.Value.ToString()).ToList().FirstOrDefault();

            switch (labelName.STATION)
            {
                case "ASSY":
                    GetTcifChildLabel(r_sn, DB);
                    break;

                case "CARTON":
                    GetTCIFCartonValue(r_sn, DB);
                    break;

                default:
                    break;
            }
        }

        private void GetTcifChildLabel(R_SN r_sn, OleExec DB)
        {
            c_mmprodmaster = DB.ORM.Queryable<C_MMPRODMASTER>().Where(s => s.PRODUCTNUMBER == r_sn.SKUNO).ToList().FirstOrDefault();
            O_PNO.Value = c_mmprodmaster.PARTNUMBER;
            O_CLEI.Value = c_mmprodmaster.CLEI;
        }

        private void GetTCIFCartonValue(R_SN r_sn, OleExec DB)
        {
            Asbuild_item = DB.ORM.Queryable<ASBUILTITEM>().Where(s => s.PARENT_MODEL_NUMBER == r_sn.SKUNO).ToList().FirstOrDefault();
            Asbuild_head = DB.ORM.Queryable<ASBUILTHEAD>().Where(s => s.MESSAGEID == Asbuild_item.MESSAGEID).ToList().FirstOrDefault();
            po_item = DB.ORM.Queryable<PO_ITEM>().Where(s => s.LINENO == Asbuild_item.SALES_ORDER_LINE_NUMBER&&s.SO==Asbuild_head.SALESORDERNUMBER).ToList().FirstOrDefault();
            c_mmprodmaster = DB.ORM.Queryable<C_MMPRODMASTER>().Where(s => s.PRODUCTNUMBER == r_sn.SKUNO).ToList().FirstOrDefault();

            O_PID.Value = r_sn.SKUNO;
            O_PNO.Value = c_mmprodmaster.PARTNUMBER;
            O_SN.Value = r_sn.SN;
            O_Qty.Value = "1";
            O_PN.Value = Asbuild_item.SUB_ASSEMBLY_NUMBER;
            O_Ver.Value = Asbuild_item.PARENT_MODEL_NUMBER;
            O_CLEI.Value = Asbuild_item.CLEI_CODE;
            O_SO.Value = Asbuild_head.SALESORDERNUMBER;
            O_CPN.Value = po_item.CUSTOMERPRODID;
            O_Date.Value = "181004";
            O_COO.Value = Asbuild_item.COUNTRY_OF_ORIGIN;

        }
    }
}
