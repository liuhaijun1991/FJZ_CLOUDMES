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
    class SingleRegularTCIF:LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        LabelOutput O_PNO = new LabelOutput() { Name = "PNO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CLEI = new LabelOutput() { Name = "CLEI", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PID = new LabelOutput() { Name = "PID", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_Qty = new LabelOutput() { Name = "Qty", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PN = new LabelOutput() { Name = "PN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SO = new LabelOutput() { Name = "SO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CPN = new LabelOutput() { Name = "CPN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_Date = new LabelOutput() { Name = "Date", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_COO = new LabelOutput() { Name = "COO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
       
        public SingleRegularTCIF()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);
            Outputs.Add(O_PNO);
            Outputs.Add(O_CLEI);
            Outputs.Add(O_PID);
            Outputs.Add(O_SN);
            Outputs.Add(O_Qty);
            Outputs.Add(O_PN);
            Outputs.Add(O_SO);
            Outputs.Add(O_CPN);
            Outputs.Add(O_Date);
            Outputs.Add(O_COO);
        }

        C_MMPRODMASTER c_mmprodmaster = null;
        R_SN_KP r_sn_kp = null;
        R_PRE_WO_HEAD pre_wo_head = null;
        PO_HEAD po_head = null;
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
                    GetTcifAssyLabel(r_sn, DB);
                    break;

                case "CARTON":
                    GetTCIFCartonValue(r_sn, DB);
                    break;

                default:
                    break;
            }
        }

        private void GetTcifAssyLabel(R_SN r_sn,OleExec DB)
        {
            c_mmprodmaster = DB.ORM.Queryable<C_MMPRODMASTER>().Where(s => s.PRODUCTNUMBER == r_sn.SKUNO).ToList().FirstOrDefault();
            O_PNO.Value = c_mmprodmaster.PARTNUMBER;
            O_CLEI.Value = c_mmprodmaster.CLEI;
        }

        private void GetTCIFCartonValue(R_SN r_sn,OleExec DB)
        {
            //Outputs.Add(O_PNO);
            //Outputs.Add(O_CLEI);
            //Outputs.Add(O_PID);
            //Outputs.Add(O_SN);
            //Outputs.Add(O_Qty);
            //Outputs.Add(O_PN);
            //Outputs.Add(O_SO);
            //Outputs.Add(O_CPN);
            //Outputs.Add(O_Date);
            //Outputs.Add(O_COO);

            c_mmprodmaster = DB.ORM.Queryable<C_MMPRODMASTER>().Where(s => s.PRODUCTNUMBER == r_sn.SKUNO).ToList().FirstOrDefault();

            r_sn_kp = DB.ORM.Queryable<R_SN_KP>().Where(s => s.SN == r_sn.SN && s.STATION == "CARTON" && s.KP_NAME == "GlobalSN").ToList().FirstOrDefault();


            pre_wo_head = DB.ORM.Queryable<R_PRE_WO_HEAD>().Where(s => s.WO == r_sn.WORKORDERNO).ToList().FirstOrDefault();

            po_head = DB.ORM.Queryable<PO_HEAD>().Where(s => s.PO == pre_wo_head.PONO).OrderBy(o => o.CREATEDT).ToList().LastOrDefault(); //***need to look for the latest record

            po_item = DB.ORM.Queryable<PO_ITEM>().Where(s => s.PO == pre_wo_head.PONO).ToList().FirstOrDefault();

            O_PID.Value = r_sn.SKUNO;
            O_Qty.Value = DB.ORM.Queryable<R_SN>().Where(s => s.SN == r_sn.SN).ToList().Count;   
            O_SN.Value = r_sn.SN;
            O_SO.Value = po_head.SALESORDERNUMBER;  
            O_CLEI.Value = c_mmprodmaster.CLEI;
            O_CPN.Value = po_item.CUSTOMERPRODID;           
            O_Date.Value = DateTime.Now.ToString("yyMMdd"); //181003           
            O_PN.Value = r_sn_kp.PARTNO;
            O_COO.Value = DB.ORM.Queryable<R_SN_KP>().Where(s => s.SN == r_sn.SN && s.STATION == "CARTON" && s.SCANTYPE == "COO").ToList().FirstOrDefault().VALUE;

        }
    }
}
