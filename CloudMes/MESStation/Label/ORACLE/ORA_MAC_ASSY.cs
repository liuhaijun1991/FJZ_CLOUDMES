using MESStation.LogicObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace MESStation.Label.ORACLE
{
    class ORA_MAC_ASSY : LabelBase
    {
        //OleExec DB;
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "string", StationSessionType = "SN", StationSessionKey = "1", Value = "" };

        LabelOutput O_SKU = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "SKUNO", Value = "" };
        LabelOutput O_MAC = new LabelOutput() { Name = "MAC", Type = LabelOutPutTypeEnum.String, Description = "MAC", Value = null };
        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Description = "SN", Value = null };
        public ORA_MAC_ASSY()
        {
            this.Inputs.Add(I_SN);
            this.Outputs.Add(O_SKU);
            this.Outputs.Add(O_MAC);
            this.Outputs.Add(O_SN);

        }
        public override void MakeLabel(OleExec DB)
        {

            string strSN = I_SN.Value.ToString();
            SN SN = new SN(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);

            string strWO = SN.WorkorderNo;
            WorkOrder WO = new WorkOrder();
            T_mfsyscserial t_mfsyscserial = new T_mfsyscserial(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            mfsyscserial getMACresult = new mfsyscserial();
            T_R_SN_KP TRKP = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);

            WO.Init(strWO, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> KPLIST = TRKP.GetKPRecordBySnID(SN.ID, DB);


            foreach (var kp in KPLIST)
            {
                if (kp.SCANTYPE == "MAC")
                {
                    O_MAC.Value = kp.VALUE;
                }
            }

            //20190306 patty added for X8-8 product

            if (this.LabelName.Contains("SIDE_A") || this.LabelName.Contains("SIDE_B"))
            {
                getMACresult = t_mfsyscserial.GetFNNMAC(strSN, this.LabelName, DB);
                if (getMACresult == null)
                {
                    throw new Exception("MAC address not found, please check L6 side A & B SN!");
                }
                O_MAC.Value = getMACresult.CSERIALNO;

            }

            //Add by James for ODAHA 2 MAC label and leverage variable MAC but it acutally is S/N
             if (this.LabelName.Contains("ODA_HA_0"))
            {
                O_MAC.Value = SN.SerialNo +  "-0";
            }
            if (this.LabelName.Contains("ODA_HA_1"))
            {
                O_MAC.Value = SN.SerialNo + "-1";
            }

            //07242019 James Add for ODA series to only display first 7 digit in sku
            string strSKUSQL = $" SELECT case when B.SKUNO is not null then B.value3 else A.SKUNO end SKUNO FROM (SELECT* FROM C_SKU A WHERE A.SKUNO ='{WO.SkuNO}' )A LEFT JOIN C_SNBOM_PN_Mapping B on A.SKUNO = B.SKUNO ";           
              DataTable dt2 = DB.ExecSelect(strSKUSQL).Tables[0];
              String strSKUNO = dt2.Rows[0]["skuno"].ToString();
               O_SKU.Value = strSKUNO;


            //if (strSKUNO.Contains("-") && (strSKUNO.Length - strSKUNO.IndexOf("-")) < 3)
            // {
            //    O_SKU.Value = strSKUNO.Substring(1, strSKUNO.IndexOf("-") - 1);
            // }
            // else
            // {
            //     O_SKU.Value = strSKUNO;
            // }

            O_SN.Value = SN.SerialNo;
        }
    }
}
