using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation.Label;
using MESDBHelper;
using MESStation.LogicObject;
using MESDataObject.Module;
using MESDataObject.Module.ORACLE;
using System.Data;

namespace MESStation.Label.ORACLE
{
    public class ORA_X7_ATO_CARTON : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "string", StationSessionType = "SN", StationSessionKey = "1", Value = "" };

        //LabelOutput O_4L = new LabelOutput() { Name = "4L", Type = LabelOutPutTypeEnum.String, Description = "", Value = "US" };
        //LabelOutput O_PID = new LabelOutput() { Name = "PID", Type = LabelOutPutTypeEnum.String, Description = "PID", Value = "" };
        LabelOutput O_PN = new LabelOutput() { Name = "PN", Type = LabelOutPutTypeEnum.String, Description = "PN", Value = null };
        LabelOutput O_PO = new LabelOutput() { Name = "PO", Type = LabelOutPutTypeEnum.String, Description = "PO", Value = null };
        //LabelOutput O_PPN = new LabelOutput() { Name = "PPN", Type = LabelOutPutTypeEnum.String, Description = "PPN", Value = null };
        LabelOutput O_Q1 = new LabelOutput() { Name = "Q1", Type = LabelOutPutTypeEnum.String, Description = "Q1", Value = "1" };
        LabelOutput O_Q2 = new LabelOutput() { Name = "Q2", Type = LabelOutPutTypeEnum.String, Description = "Q2", Value = "1" };
        LabelOutput O_QQTY = new LabelOutput() { Name = "QQTY", Type = LabelOutPutTypeEnum.String, Description = "QQTY", Value = null };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "QTY", Value = null };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "skuno", Type = LabelOutPutTypeEnum.String, Description = "skuno", Value = null };
        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Description = "SN", Value = null };
        LabelOutput O_SSN = new LabelOutput() { Name = "SSN", Type = LabelOutPutTypeEnum.String, Description = "SSN", Value = null }; 
        //patty_20190108 added sku serial, COO and VenerNO variables
        LabelOutput O_SKUSER = new LabelOutput() { Name = "SKUSer", Type = LabelOutPutTypeEnum.String, Description = "SKUSer", Value = "" };
        LabelOutput O_COO = new LabelOutput() { Name = "COO", Type = LabelOutPutTypeEnum.String, Description = "COO", Value = "" };
        LabelOutput O_VN = new LabelOutput() { Name = "VenderNO", Type = LabelOutPutTypeEnum.String, Description = "VenderNO", Value = "" };
        //kalai added subsku/inbox pn for Oracle Direct ship PTO label 10/21/2019
        LabelOutput O_InBoxPN = new LabelOutput() { Name = "InBoxPN", Type = LabelOutPutTypeEnum.String, Description = "InBoxPN", Value = "" };
        
        //JianDong 2019-3-18 10:58:51 add DIMM Type in PACKOUT LABEL
        LabelOutput O_DIMM_TYPE = new LabelOutput() { Name = "DIMM_TYPE", Type = LabelOutPutTypeEnum.String, Description = "DIMM_TYPE", Value = "" };

        public ORA_X7_ATO_CARTON()
        {
            this.Inputs.Add(I_SN);
            //this.Outputs.Add(O_PID);
            this.Outputs.Add(O_PN);
            this.Outputs.Add(O_PO);
            //this.Outputs.Add(O_PPN);
            this.Outputs.Add(O_Q1);
            this.Outputs.Add(O_Q2);
            this.Outputs.Add(O_QQTY);
            this.Outputs.Add(O_QTY);
            this.Outputs.Add(O_SKUNO);
            this.Outputs.Add(O_SN);
            this.Outputs.Add(O_SSN);
            this.Outputs.Add(O_SKUSER);
            this.Outputs.Add(O_COO);
            this.Outputs.Add(O_VN);
         
            this.Outputs.Add(O_InBoxPN);
            
            this.Outputs.Add(O_DIMM_TYPE);
        }
        public override void MakeLabel(OleExec DB)
        {
            string strSN = I_SN.Value.ToString();
            SN sn = new SN(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string strWO = sn.WorkorderNo;
            WorkOrder WO = new WorkOrder();
            WO.Init(strWO, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            SKU sku = new SKU();
            sku.Init(WO.SkuNO, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            //sku.InitBySn(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            var prewo = DB.ORM.Queryable<R_MFPRESETWOHEAD>().Where(t => t.WO == strWO).ToList();
            //add by James Zhu to exclude intel dimm 11/09/2019
            var intelPart = DB.ORM.Queryable<C_SKU_MPN>().Where(m => m.MFRCODE == "INTEL").Select(x => x.PARTNO).ToList();
            

            if (prewo.Count > 0)
            {
                //O_SKUNO.Value = Oracle_PO_Line.BuyerPartNumber;
                //O_SKUSER.Value = prewo[0].PID;
                O_SKUNO.Value = sku.SkuBase.SKUNO;
                O_SKUSER.Value = WO.SKU_SERIES;
                //trim SKUNO for L11 vince_20200205
                if (WO.SKU_SERIES == "ORACLE_RACK")
                {
                    O_SKUSER.Value = sku.SkuBase.SKUNO.ToString().Substring(0, sku.SkuBase.SKUNO.ToString().LastIndexOf("-")); 
                }

                O_PO.Value = prewo[0].PO;
                O_SN.Value = strSN;
                O_QTY.Value = "1";
                O_COO.Value = "US";
                O_VN.Value = "465136";
     
                O_InBoxPN.Value = "";

                O_DIMM_TYPE.Value = "";
                //kalai added subsku/inbox pn for Oracle Direct ship PTO label 10/21/2019 
                string strSKUSQL = $" SELECT distinct SKUNO from C_SNBOM_PN_MAPPING WHERE SKUNO ='{sku.SkuBase.SKUNO}'";
                DataTable dt3 = DB.ExecSelect(strSKUSQL).Tables[0];
                if (dt3.Rows.Count > 0)
                {
                    string strPNSQL = $" SELECT distinct VALUE3 from C_SNBOM_PN_MAPPING WHERE SKUNO ='{sku.SkuBase.SKUNO}'";
                    DataTable dt2 = DB.ExecSelect(strPNSQL).Tables[0];
                    O_InBoxPN.Value = dt2.Rows[0][0].ToString();
                }  

                try
                {
                    R_SN_KP KPDIMM = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.R_SN_ID == sn.ID && k.KP_NAME == "DIMM"  && !intelPart.Contains(k.PARTNO) && SqlSugar.SqlFunc.Length(k.MPN) > 0).First();
                    // R_SN_KP KPDIMM = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.R_SN_ID == sn.ID && k.KP_NAME == "DIMM" && SqlSugar.SqlFunc.Length(k.MPN) > 0).First();
                    C_SKU_MPN MPN = DB.ORM.Queryable<C_SKU_MPN>().Where((m) => m.SKUNO == sku.SkuBase.SKU_NAME && m.PARTNO == KPDIMM.PARTNO && m.MPN == KPDIMM.MPN).First();
                   // C_SKU_MPN MPN = DB.ORM.Queryable<C_SKU_MPN>().Where((m) => m.SKUNO == sku.SkuBase.SKU_NAME && KPDIMM.Contains(m.PARTNO)  && m.MPN == KPDIMM.MPN).First();
                    O_DIMM_TYPE.Value = MPN.MFRCODE.ToUpper();
                }
                catch
                {
                }
            }
            else
            {
                //patty_20190108 modified label variables for PTO 
                //O_PID.Value = sku.SkuBase.SKUNO;
                //O_PN.Value = sku.SkuBase.SKUNO;
                //O_PO.Value = "";
                O_SKUNO.Value = sku.SkuBase.SKUNO;
                O_SKUSER.Value = WO.SKU_SERIES;

                //trim SKUNO for L11 vince_20200205
                if (WO.SKU_SERIES == "ORACLE_RACK")
                {
                    O_SKUSER.Value = sku.SkuBase.SKUNO.ToString().Substring(0, sku.SkuBase.SKUNO.ToString().LastIndexOf("-"));
                }

                O_SN.Value = strSN;
                O_QTY.Value = "1";
                O_COO.Value = "US";
                O_VN.Value = "465136";
            
                O_InBoxPN.Value = "";
                O_DIMM_TYPE.Value = "";

                //  kalai added subsku/inbox pn for Oracle Direct ship PTO label 10/21/2019  
                string strSKUSQL = $" SELECT distinct SKUNO from C_SNBOM_PN_MAPPING WHERE SKUNO ='{sku.SkuBase.SKUNO}'";
                DataTable dt4 = DB.ExecSelect(strSKUSQL).Tables[0];
             
                if (dt4.Rows.Count > 0)
                {
                    string strPNSQL = $" SELECT distinct VALUE3 from C_SNBOM_PN_MAPPING WHERE SKUNO ='{sku.SkuBase.SKUNO}'";
                    DataTable dt2 = DB.ExecSelect(strPNSQL).Tables[0];
                    O_InBoxPN.Value = dt2.Rows[0][0].ToString();
                }

                try
                {
                    R_SN_KP KPDIMM = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.R_SN_ID == sn.ID && k.KP_NAME == "DIMM" && !intelPart.Contains(k.PARTNO) && SqlSugar.SqlFunc.Length(k.MPN) > 0).First();
                    C_SKU_MPN MPN = DB.ORM.Queryable<C_SKU_MPN>().Where((m) => m.SKUNO == sku.SkuBase.SKUNO && m.PARTNO == KPDIMM.PARTNO && m.MPN == KPDIMM.MPN).First();
                    O_DIMM_TYPE.Value = MPN.MFRCODE.ToUpper();
             
          
                }
                catch
                {
                }
            }
        }
    }
}
