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
    class JNP_CARTON_LABEL : LabelBase
    {
        LabelInputValue I_CARTON_NO = new LabelInputValue() { Name = "CARTON_NO", Type = "STRING", Value = "", StationSessionType = "PRINT_CARTON", StationSessionKey = "1" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_SO = new LabelOutput() { Name = "SO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_INDEX = new LabelOutput() { Name = "INDEX", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_TOTAL_CARTON = new LabelOutput() { Name = "TOTAL_CARTON", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        T_R_SN t_r_sn;
        public JNP_CARTON_LABEL()
        {
            Inputs.Add(I_CARTON_NO);
            Outputs.Add(O_SN);
            Outputs.Add(O_SO);
            Outputs.Add(O_INDEX);
            Outputs.Add(O_TOTAL_CARTON);

        }
        public override void MakeLabel(OleExec DB)
        {
            List<string> snlist = DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING>((s, p, sp) => s.ID == sp.SN_ID && sp.PACK_ID == p.ID)
                .Where((s, p, sp) => p.PACK_NO == I_CARTON_NO.Value.ToString()).Select((s, p, sp) => s.SN).ToList();

            t_r_sn = new T_R_SN(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_SN r_sn = t_r_sn.LoadSN(snlist[0].ToString(), DB);

            string strSql = $@"SELECT c.pono FROM r_sn a,r_pre_wo_head b ,R_PO c   WHERE a.sysserialno='{r_sn.SN}' AND a.workorderno=b.wo AND b.pono=c.pono";
            DataSet res = DB.RunSelect(strSql);
            O_SO.Value = res.Tables[0].Rows[0]["pono"].ToString();

            strSql = $@"SELECT * FROM  r_wo_base WHERE workorderno='{r_sn.WORKORDERNO.ToString()}'";
            res = DB.RunSelect(strSql);
            Int32 wo_qty = Convert.ToInt32(res.Tables[0].Rows[0]["workorder_qty"].ToString());

            strSql = $@"SELECT * FROM c_packing WHERE skuno='{r_sn.SKUNO}' AND pack_type='CARTON' ";
            res = DB.RunSelect(strSql);
            Int32 carton_uom = Convert.ToInt32(res.Tables[0].Rows[0]["MAX_QTY"].ToString());

            if (wo_qty % carton_uom == 0)
            {
                O_TOTAL_CARTON.Value = wo_qty / carton_uom;
            }
            else
            {
                O_TOTAL_CARTON.Value = wo_qty / carton_uom + 1;
            }

            List<string> carton_list = DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING>((s, p, sp) => s.ID == sp.SN_ID && sp.PACK_ID == p.ID)
                .Where((s, p, sp) => s.WORKORDERNO == r_sn.WORKORDERNO.ToString()).GroupBy((s, p, sp) => p.PACK_NO).OrderBy((s, p, sp) => p.PACK_NO).Select((s, p, sp) => p.PACK_NO).ToList();

            Int32 k = 0;
            foreach (string m in carton_list)
            {
                k = k + 1;
                if (m == I_CARTON_NO.Value.ToString())
                {
                    O_INDEX.Value = k;
                    break;
                }
                    
            }
        }
    }
}
