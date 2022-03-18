using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.MESStation.Label;
using System.Data;

namespace MESStation.Label
{
    class JNP_MC_TCIF : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PN = new LabelOutput() { Name = "PN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_COO = new LabelOutput() { Name = "COO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CSPN = new LabelOutput() { Name = "CS PN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_FIELD1 = new LabelOutput() { Name = "FIELD1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_FIELD2 = new LabelOutput() { Name = "FIELD2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_FIELD3 = new LabelOutput() { Name = "FIELD3", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SO = new LabelOutput() { Name = "SO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PRINTDATE = new LabelOutput() { Name = "PRINTDATE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        
        T_R_SN t_r_sn;
        T_c_label_ex c_label_ex;
        T_R_SN_KP t_r_sn_kp;
        public JNP_MC_TCIF()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);
            Outputs.Add(O_SN);
            Outputs.Add(O_PN);
            Outputs.Add(O_COO);
            Outputs.Add(O_CSPN);
            Outputs.Add(O_FIELD1);
            Outputs.Add(O_FIELD2);
            Outputs.Add(O_FIELD3);
            Outputs.Add(O_VER);
            Outputs.Add(O_SO);
            Outputs.Add(O_PRINTDATE);
 
        }
        public override void MakeLabel(OleExec DB)
        {
            t_r_sn = new T_R_SN(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
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

            if (r_sn == null)
            {
                return;
            }


            O_SN.Value = r_sn.SN;
            O_PN.Value = r_sn.SKUNO;

            R_WO_BASE r_wo_base = null;
            r_wo_base = DB.ORM.Queryable<R_WO_BASE>().Where(wo => wo.WORKORDERNO == r_sn.WORKORDERNO).ToList().FirstOrDefault();
            O_VER.Value= r_wo_base.SKU_VER;
            O_PRINTDATE.Value = DateTime.Now.ToString("MM/dd/yyyy");


            List<R_SN_KP> KPList = new List<R_SN_KP>();
            List<c_label_ex> C_LABEL_EX_List = new List<c_label_ex>();
            t_r_sn_kp = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            c_label_ex = new T_c_label_ex(DB, MESDataObject.DB_TYPE_ENUM.Oracle);

            KPList = t_r_sn_kp.GetKPRecordBySnID(r_sn.ID, DB);
            foreach (R_SN_KP k in KPList)
            {
                foreach (LabelOutput o in Outputs)
                {
                    if (o.Name == k.KP_NAME)
                    {
                        o.Value = k.VALUE;
                    }
                }
            }

            T_C_SKU_Label TCSL = new T_C_SKU_Label(DB, DB_TYPE_ENUM.Oracle);
            List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(r_sn.SKUNO, I_STATION.Value.ToString(), DB);

            C_LABEL_EX_List = c_label_ex.GetKPRecordBySnID(labs[0].ID, DB);

            foreach (c_label_ex m in C_LABEL_EX_List)
            {
                foreach (LabelOutput o in Outputs)
                {
                    if (o.Name == m.NAME)
                    {
                        o.Value = m.VALUE;
                    }
                }
            }

            string strSql = $@"SELECT c.pono FROM r_sn a,r_pre_wo_head b ,R_PO c   WHERE a.sn='{r_sn.SN}' AND a.workorderno=b.wo AND b.pono=c.pono";
            DataSet res = DB.RunSelect(strSql);
            O_SO.Value = res.Tables[0].Rows[0]["pono"].ToString();

        }

    }
}
