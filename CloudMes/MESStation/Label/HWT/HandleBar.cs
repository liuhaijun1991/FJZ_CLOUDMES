using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.MESStation.Label;

namespace MESStation.Label.HWT
{
    public class HandleBar : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        LabelOutput O_PANELSSN = new LabelOutput() { Name = "PANELSSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SNVERSION = new LabelOutput() { Name = "SNVERSION", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        public HandleBar()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);
            Outputs.Add(O_PANELSSN);
            Outputs.Add(O_SNVERSION);
        }
        T_R_SN t_r_sn;
        //T_R_SN_KP t_r_sn_kp;
        //T_C_SKU_VER_MAPPING t_c_sku_ver_mapping;
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
            LogicObject.SKU SKU = new LogicObject.SKU();
            SKU.InitBySn(r_sn.SN, DB, DB_TYPE_ENUM.Oracle);
            O_SNVERSION.Value = SKU.Version;
            var strSN = r_sn.SN;
            strSN = strSN.Substring(0, strSN.Length - 6) + "6" + strSN.Substring(strSN.Length - 5, 5);
            O_PANELSSN.Value = strSN;


        }
    }
}
