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
    class SingleBulkOverPack:LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };       
        LabelOutput O_PID = new LabelOutput() { Name = "PID", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };    
        LabelOutput O_Qty = new LabelOutput() { Name = "Qty", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SO = new LabelOutput() { Name = "SO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };       
        LabelOutput O_Date = new LabelOutput() { Name = "Date", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_COO = new LabelOutput() { Name = "COO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CPN = new LabelOutput() { Name = "CPN", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        //LabelOutput O_count = new LabelOutput() { Name = "COUNT", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        //LabelOutput O_total = new LabelOutput() { Name = "TOTAL", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LableOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        public SingleBulkOverPack()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);           
            Outputs.Add(O_PID);           
            Outputs.Add(O_Qty);           
            Outputs.Add(O_SO);           
            Outputs.Add(O_Date);
            Outputs.Add(O_COO);
            Outputs.Add(O_CPN);
            Outputs.Add(O_SN);
            //Outputs.Add(O_count);
            //Outputs.Add(O_total);
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
           // C_SKU_Label labelName = null;
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
            // labelName = DB.ORM.Queryable<C_SKU_Label>().Where(l => l.SKUNO == r_sn.SKUNO && l.STATION == I_STATION.Value.ToString()).ToList().FirstOrDefault();

            
            GetOverPackCartonValue(r_sn, this.LabelName, DB);
        }

       
        private void GetOverPackCartonValue(R_SN r_sn,string labelName, OleExec DB)
        {

            c_mmprodmaster = DB.ORM.Queryable<C_MMPRODMASTER>().Where(s => s.PRODUCTNUMBER == r_sn.SKUNO).ToList().FirstOrDefault();

            r_sn_kp = DB.ORM.Queryable<R_SN_KP>().Where(s => s.SN == r_sn.SN && s.STATION == "CARTON" && s.KP_NAME == "GlobalSN").ToList().FirstOrDefault();
            pre_wo_head = DB.ORM.Queryable<R_PRE_WO_HEAD>().Where(s => s.WO == r_sn.WORKORDERNO).ToList().FirstOrDefault();
            po_head = DB.ORM.Queryable<PO_HEAD>().Where(s => s.PO == pre_wo_head.PONO).ToList().OrderBy(o => o.CREATEDT).LastOrDefault();
            po_item = DB.ORM.Queryable<PO_ITEM>().Where(s => s.PO == pre_wo_head.PONO).ToList().OrderBy(o => o.CREATEDT).LastOrDefault();

            O_PID.Value = r_sn.SKUNO;
            O_Qty.Value = DB.ORM.Queryable<R_SN>().Where(s => s.WORKORDERNO == r_sn.WORKORDERNO && s.SKUNO == r_sn.SKUNO).ToList().Count;    
            O_SO.Value = po_head.SALESORDERNUMBER;         
            O_CPN.Value = po_item.CUSTOMERPRODID;           
            O_Date.Value = DateTime.Now.ToString("yyMMdd"); //format: 181003         
            O_COO.Value = "TW"; //DB.ORM.Queryable<R_SN_KP>().Where(s => s.SN == r_sn.SN && s.STATION == "CARTON" && s.SCANTYPE == "COO").ToList().FirstOrDefault().VALUE;

            /////
            if (labelName == "JUNIPER_OVERPACK_32")
            {
                //O_total.Value = "1";
                //O_count.Value = "1";
                //  O_SN.Value = DB.ORM.Queryable<R_SN>().Where(s => s.WORKORDERNO == r_sn.WORKORDERNO).ToList().ToString().Cast<String>().ToList();
                O_SN.Value = t_r_sn.GetSNStrList(r_sn.WORKORDERNO, DB);
                if (((List<string>)O_SN.Value).Count == 0)
                {
                    throw new Exception("SN is empty");
                }
               
            }
            /////

        }

    }
}
