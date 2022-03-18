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
    class REPRINT_L11_LABEL : LabelBase
    {

        //OleExec DB;
        //LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "string", StationSessionType = "SN", StationSessionKey = "1", Value = "" };
        LabelInputValue I_PART_NO = new LabelInputValue() { Name = "SKU", Type = "string", StationSessionType = "SKU", StationSessionKey = "1", Value = "" };
        LabelInputValue I_SEQ = new LabelInputValue() { Name = "SEQ", Type = "string", StationSessionType = "SEQ", StationSessionKey = "1", Value = "" };
        //rack or power type cable
        LabelInputValue I_CABLETYPE = new LabelInputValue() { Name = "CABLETYPE", Type = "string", StationSessionType = "CABLETYPE", StationSessionKey = "", Value = "" };

        LabelOutput O_RU_FIELD = new LabelOutput() { Name = "RU_FIELD", Type = LabelOutPutTypeEnum.String, Description = "RU_FIELD", Value = null };
        LabelOutput O_PDU_FIELD = new LabelOutput() { Name = "PDU_FIELD", Type = LabelOutPutTypeEnum.String, Description = "PDU_FIELD", Value = null };
        LabelOutput O_CABLE_FIELD = new LabelOutput() { Name = "CABLE_FIELD", Type = LabelOutPutTypeEnum.String, Description = "CABLE_FIELD", Value = null };


        public REPRINT_L11_LABEL()
        {
            //this.Inputs.Add(I_SN);
            this.Inputs.Add(I_PART_NO);
            this.Inputs.Add(I_SEQ);
            this.Inputs.Add(I_CABLETYPE);
            this.Outputs.Add(O_RU_FIELD);
            this.Outputs.Add(O_PDU_FIELD);
            this.Outputs.Add(O_CABLE_FIELD);
        }

        public override void MakeLabel(OleExec DB)
        {

            // string rackSN = I_SN.Value.ToString();
            string rackPartno = I_PART_NO.Value.ToString();
            string cabletype = I_CABLETYPE.Value.ToString();
            int rackSeq = Convert.ToInt32(I_SEQ.Value.ToString());

            string ruVar1, ruVar2, portVar1, portVar2, cableLen, cableType, cablePN, emptyVar1, emptyVar2;
            //fetching Label data from DB

            if (cabletype == "RACK")
            {

                string strCableQL = $" select * from SFCBASE.C_L11_RACK_CABLE where partno = '{rackPartno}' and seq = '{rackSeq}'";
                DataTable dt2 = DB.ExecSelect(strCableQL).Tables[0];
                emptyVar1 = " ";
                emptyVar2 = "   ";
                ruVar1 = dt2.Rows[0]["FROM_RU"].ToString();
                ruVar2 = dt2.Rows[0]["FROM_PORT"].ToString();
                portVar1 = dt2.Rows[0]["TO_RU"].ToString();
                portVar2 = dt2.Rows[0]["TO_PORT"].ToString();
                cableLen = dt2.Rows[0]["CABLE_LENGTH"].ToString();
                cableType = dt2.Rows[0]["CABLE_TYPE"].ToString();
                cablePN = dt2.Rows[0]["CABLE_PN"].ToString();
                cablePN = string.Concat("(", cablePN, ")");
                O_RU_FIELD.Value = string.Concat(ruVar1, emptyVar1, ruVar2);
                O_PDU_FIELD.Value = string.Concat(portVar1, emptyVar1, portVar2);
                O_CABLE_FIELD.Value = string.Concat(cableLen, emptyVar1, cableType, emptyVar2, cablePN);
            }

            else if (cabletype == "POWER")
            {

                string strCableQL = $" select * from SFCBASE.C_L11_POWER_CABLE where partno = '{rackPartno}' and seq = '{rackSeq}'";
                DataTable dt2 = DB.ExecSelect(strCableQL).Tables[0];
                emptyVar1 = " ";
                emptyVar2 = "   ";
                ruVar1 = dt2.Rows[0]["RU"].ToString();
                ruVar2 = dt2.Rows[0]["DEVICE_PORT"].ToString();
                portVar1 = dt2.Rows[0]["PDU"].ToString();
                portVar2 = dt2.Rows[0]["PDU_PORT"].ToString();
                cableLen = dt2.Rows[0]["CABLE_LENGTH"].ToString();
                cableType = dt2.Rows[0]["CABLE_TYPE"].ToString();
                cablePN = dt2.Rows[0]["CABLE_PN"].ToString();
                cablePN = string.Concat("(", cablePN, ")");
                O_RU_FIELD.Value = string.Concat(ruVar1, emptyVar1, ruVar2);
                O_PDU_FIELD.Value = string.Concat(portVar1, emptyVar1, portVar2);
                O_CABLE_FIELD.Value = string.Concat(cableLen, emptyVar1, cableType, emptyVar2, cablePN);
            }

        }
    }


}

