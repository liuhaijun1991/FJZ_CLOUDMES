using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using MESStation.LogicObject;
using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Linq;

namespace MESStation.Label.ORACLE
{
    public class X7_2_CIS_P1 : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "string", StationSessionType = "SN", StationSessionKey = "1", Value = "" };
        LabelOutput O_MAC0 = new LabelOutput() { Name = "MAC0", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MAC1 = new LabelOutput() { Name = "MAC1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MAC2 = new LabelOutput() { Name = "MAC2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MAC3 = new LabelOutput() { Name = "MAC3", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MAC_QTY0 = new LabelOutput() { Name = "MAC_QTY0", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MAC_QTY1 = new LabelOutput() { Name = "MAC_QTY1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MAC_QTY2 = new LabelOutput() { Name = "MAC_QTY2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MAC_QTY3 = new LabelOutput() { Name = "MAC_QTY3", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MMAC0 = new LabelOutput() { Name = "MMAC0", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MMAC1 = new LabelOutput() { Name = "MMAC1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MMAC2 = new LabelOutput() { Name = "MMAC2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MMAC3 = new LabelOutput() { Name = "MMAC3", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MODEL = new LabelOutput() { Name = "MODEL", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MSN0 = new LabelOutput() { Name = "MSN0", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MSN1 = new LabelOutput() { Name = "MSN1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MSN2 = new LabelOutput() { Name = "MSN2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MSN3 = new LabelOutput() { Name = "MSN3", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_OMAGE_REV = new LabelOutput() { Name = "OMAGE_REV", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_P1 = new LabelOutput() { Name = "P1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_P2 = new LabelOutput() { Name = "P2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PAGE1 = new LabelOutput() { Name = "PAGE1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PAGE2 = new LabelOutput() { Name = "PAGE2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SERIAL = new LabelOutput() { Name = "SERIAL", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUDCRP = new LabelOutput() { Name = "SKUDCRP", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SSN = new LabelOutput() { Name = "SSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CISFLAG = new LabelOutput() { Name = "CISFLAG", Type = LabelOutPutTypeEnum.String, Description = "CISFLAG", Value = "" };

        public X7_2_CIS_P1()
        {
            this.Inputs.Add(I_SN);

            this.Outputs.Add(O_MAC0);
            this.Outputs.Add(O_MAC1);
            this.Outputs.Add(O_MAC2);
            this.Outputs.Add(O_MAC3);
            this.Outputs.Add(O_MAC_QTY0);
            this.Outputs.Add(O_MAC_QTY1);
            this.Outputs.Add(O_MAC_QTY2);
            this.Outputs.Add(O_MAC_QTY3);
            this.Outputs.Add(O_MMAC0);
            this.Outputs.Add(O_MMAC1);
            this.Outputs.Add(O_MMAC2);
            this.Outputs.Add(O_MMAC3);
            this.Outputs.Add(O_MODEL);//
            this.Outputs.Add(O_MSN0);
            this.Outputs.Add(O_MSN1);
            this.Outputs.Add(O_MSN2);
            this.Outputs.Add(O_MSN3);
            this.Outputs.Add(O_OMAGE_REV);
            this.Outputs.Add(O_P1);
            this.Outputs.Add(O_P2);
            this.Outputs.Add(O_PAGE1);
            this.Outputs.Add(O_PAGE2);
            this.Outputs.Add(O_SERIAL);//
            this.Outputs.Add(O_SKUNO);//
            this.Outputs.Add(O_SKUDCRP);
            this.Outputs.Add(O_SSN);//
            this.Outputs.Add(O_CISFLAG);

        }
        public override void MakeLabel(OleExec DB)
        {
            string strSN = I_SN.Value.ToString();
            SN SN = new SN(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            SKU sku = new SKU();
            //WorkOrder WO = new WorkOrder();
            //WO.Init(SN.WorkorderNo, DB, MESDataObject.DB_TYPE_ENUM.Oracle);

            //add by James zhu 08/22/2019 for oracle request to show differnt sku from cis page
            string strSKUSQL = $" SELECT case when B.SKUNO is not null then B.value3 else A.SKUNO end SKUNO FROM (SELECT* FROM C_SKU A WHERE A.SKUNO ='{SN.SkuNo}' )A LEFT JOIN C_SNBOM_PN_Mapping B on A.SKUNO = B.SKUNO ";
            DataTable dt2 = DB.ExecSelect(strSKUSQL).Tables[0];
            String strSKUNO = dt2.Rows[0]["skuno"].ToString();

            string strWo = SN.WorkorderNo;
            T_C_SERIES t_c_series = new T_C_SERIES(DB, DB_TYPE_ENUM.Oracle);
            sku.InitBySn(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            C_SERIES c_series = t_c_series.GetDetailById(DB, sku.CSeriesId);//sku.CSeriesId
            if (c_series == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SERIES" }));
            }
            O_MODEL.Value = SN.SkuNo;
            O_SSN.Value = SN.baseSN.SN;
            O_SKUNO.Value = strSKUNO;
            //O_SKUNO.Value = SN.SkuNo;
            O_SERIAL.Value = c_series.SERIES_NAME;
            O_SKUDCRP.Value = sku.Description;
            //O_SKUDCRP.Value = sku.Description.Replace(",","/L");
            //List<C_MMPRODMASTER> mm = DB.ORM.Queryable<C_MMPRODMASTER>().Where(t => t.PARTNO == sku.SkuNo).ToList();
            //if (mm.Count > 0)
            //{
            //    O_SKUDCRP.Value = mm[0].DESCRIPTION;
            //}

            var strsku = SN.SkuNo;
            //Softsare
            //var SoftVer = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == strsku && t.CATEGORY == "PRINT" && t.CATEGORY_NAME == "SOFTWARE_VERS").ToList();
            //if (SoftVer.Count > 0)
            //{
            //    O_P1.Value = SoftVer[0].VALUE;
            //    O_P2.Value = SoftVer[0].EXTEND;
            //    O_OMAGE_REV.Value = SoftVer[0].VERSION;
            //}
            //get Software from correct table vince_20191007
            string strSWSQL = $" select * from C_SNBOM_EXTEND where ctype = 'SOFTWARE_VERS' and skuno = '{strsku}' ";
            DataTable Softdt = DB.ExecSelect(strSWSQL).Tables[0];
            if (Softdt.Rows.Count > 0)
            {
                string partno = Softdt.Rows[0]["VALUE1"].ToString().Trim();
                O_P1.Value = partno;
                var des = DB.ORM.Queryable<C_ORACLE_MFASSEMBLYDATA>().Where((m) => m.CUSTPARTNO==partno).ToList();
              //  var des = DB.ORM.Queryable<C_MMPRODMASTER>().Where(t => t.PARTNO == partno).ToList();
                if (des.Count > 0)
                { O_P2.Value = des[0].DESCRIPTION.Replace("?", ""); }                   
                //O_OMAGE_REV.Value = SoftVer[0].VERSION;
            }

            #region Get Mac in CloudMES
            /*
            //Get MAC
            T_R_SN_KP TRKP = new T_R_SN_KP(DB, DB_TYPE_ENUM.Oracle);

            List<R_SN_KP> TempSnkp = TRKP.GetKPRecordBySnID(SN.ID, DB);

            List<R_SN_KP> AllKP = new List<R_SN_KP>();
            List<string> KP_SN_IDs;
            while (TempSnkp.Count > 0)
            {
                AddList(AllKP, TempSnkp);
                KP_SN_IDs= makeValueList(TempSnkp);
                var kpsns = DB.ORM.Queryable<R_SN>().Where(t => KP_SN_IDs.Contains(t.ID) && t.VALID_FLAG == "1").ToList();
                
                TempSnkp.Clear();
                for (int i = 0; i < kpsns.Count; i++)
                {
                    var T = TRKP.GetKPRecordBySnID(kpsns[i].ID, DB);
                    AddList(TempSnkp, T);
                }
                
            }

            var macs = AllKP.FindAll(t => t.SCANTYPE == "MAC");
            var MAC_SKU = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "PRINT" && t.CATEGORY_NAME == "MTHRBD_PN" && t.SKUNO == strsku).ToList();
            var PREWOH = DB.ORM.Queryable<R_MFPRESETWOHEAD>().Where(t => t.WO == strWo).ToList();
            List<string> mac_sku = new List<string>();
            if (PREWOH.Count > 0)
            {
                mac_sku.Add(PREWOH[0].PID);
            }

            for (int i = 0; i < MAC_SKU.Count; i++)
            {
                mac_sku.Add(MAC_SKU[i].VALUE);
            }
            KP_SN_IDs = makeValueList(macs);
            var mac_kp_sn = DB.ORM.Queryable<R_SN>().Where(t => KP_SN_IDs.Contains(t.ID) && t.VALID_FLAG == "1" && mac_sku.Contains(t.SKUNO)).ToList();

            for (int i = 0; i < mac_kp_sn.Count; i++)
            {
                if (i == 0)
                {
                    var temp = macs.Find(t => t.SN == mac_kp_sn[i].SN);
                    O_MMAC0.Value = temp.VALUE;
                    O_MSN0.Value = temp.SN;
                    O_MAC0.Value = String.Format("{0}:{1}:{2}:{3}:{4}:{5}", 
                        temp.VALUE.Substring(0,2), 
                        temp.VALUE.Substring(2, 2),
                        temp.VALUE.Substring(4, 2),
                        temp.VALUE.Substring(6, 2),
                        temp.VALUE.Substring(8, 2),
                        temp.VALUE.Substring(10, 2));
                    var MAC_QTY = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "PRINT" && t.CATEGORY_NAME == "MAC_QTY" && t.SKUNO == temp.PARTNO).ToList();
                    if (MAC_QTY.Count == 0)
                    {
                        throw new Exception($@"{temp.PARTNO}:MAC_QTY not config !");
                    }
                    O_MAC_QTY0.Value = MAC_QTY[0].VALUE;

                }

                if (i == 1)
                {
                    var temp = macs.Find(t => t.SN == mac_kp_sn[i].SN);
                    O_MMAC1.Value = temp.VALUE;
                    O_MSN1.Value = temp.SN;
                    O_MAC1.Value = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                        temp.VALUE.Substring(0, 2),
                        temp.VALUE.Substring(2, 2),
                        temp.VALUE.Substring(4, 2),
                        temp.VALUE.Substring(6, 2),
                        temp.VALUE.Substring(8, 2),
                        temp.VALUE.Substring(10, 2));
                    var MAC_QTY = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "PRINT" && t.CATEGORY_NAME == "MAC_QTY" && t.SKUNO == temp.PARTNO).ToList();
                    if (MAC_QTY.Count == 0)
                    {
                        throw new Exception($@"{temp.PARTNO}:MAC_QTY not config !");
                    }
                    O_MAC_QTY1.Value = MAC_QTY[0].VALUE;

                }
                if (i == 2)
                {
                    var temp = macs.Find(t => t.SN == mac_kp_sn[i].SN);
                    O_MMAC2.Value = temp.VALUE;
                    O_MSN2.Value = temp.SN;
                    O_MAC2.Value = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                        temp.VALUE.Substring(0, 2),
                        temp.VALUE.Substring(2, 2),
                        temp.VALUE.Substring(4, 2),
                        temp.VALUE.Substring(6, 2),
                        temp.VALUE.Substring(8, 2),
                        temp.VALUE.Substring(10, 2));
                    var MAC_QTY = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "PRINT" && t.CATEGORY_NAME == "MAC_QTY" && t.SKUNO == temp.PARTNO).ToList();
                    if (MAC_QTY.Count == 0)
                    {
                        throw new Exception($@"{temp.PARTNO}:MAC_QTY not config !");
                    }
                    O_MAC_QTY2.Value = MAC_QTY[0].VALUE;

                }
                if (i == 3)
                {
                    var temp = macs.Find(t => t.SN == mac_kp_sn[i].SN);
                    O_MMAC3.Value = temp.VALUE;
                    O_MSN3.Value = temp.SN;
                    O_MAC3.Value = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                        temp.VALUE.Substring(0, 2),
                        temp.VALUE.Substring(2, 2),
                        temp.VALUE.Substring(4, 2),
                        temp.VALUE.Substring(6, 2),
                        temp.VALUE.Substring(8, 2),
                        temp.VALUE.Substring(10, 2));
                    var MAC_QTY = DB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "PRINT" && t.CATEGORY_NAME == "MAC_QTY" && t.SKUNO == temp.PARTNO).ToList();
                    if (MAC_QTY.Count == 0)
                    {
                        throw new Exception($@"{temp.PARTNO}:MAC_QTY not config !");
                    }
                    O_MAC_QTY3.Value = MAC_QTY[0].VALUE;

                }


            }
            */
            #endregion End get Mac in SFC

            #region get Mac in SFC
            T_R_SN_KP TRKP = new T_R_SN_KP(DB, DB_TYPE_ENUM.Oracle);

            List<R_SN_KP> MesKp;
            // add by James Zhu 20190623 for X7-2C CIS Print
            if (c_series.SERIES_NAME=="X7-2C" || c_series.SERIES_NAME == "E1-2C")
            {
                List<R_SN_KP>  MesKp_temp = DB.ORM.Queryable<R_SN_KP>()
              .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("Server Module") && k.SCANTYPE == "SMSN")
              .OrderBy(t => t.ITEMSEQ)
              .OrderBy(t => t.SCANSEQ)
              .OrderBy(t => t.DETAILSEQ)
              .ToList();
          
                var smModule = MesKp_temp.Select(s => s.VALUE).ToList();
                MesKp = DB.ORM.Queryable<R_SN_KP>()
                   .Where((x) => smModule.Contains(x.SN) && x.SCANTYPE== "SystemSN" ).ToList();
            }
            else if (c_series.SERIES_NAME.Contains("ODA_HA")) //CIS for ODA-HA vince_20190827
            {            
                List<R_SN_KP> MesKp_temp = DB.ORM.Queryable<R_SN_KP>()
                .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("ODA_HA") && k.SCANTYPE.Contains("SMSN")) //Maybe change to HASN
                .OrderBy(t => t.ITEMSEQ)
                .OrderBy(t => t.SCANSEQ)
                .OrderBy(t => t.DETAILSEQ)
                .ToList();

                var smModule = MesKp_temp.Select(s => s.VALUE).ToList();
                MesKp = DB.ORM.Queryable<R_SN_KP>()
                    .Where((x) => smModule.Contains(x.SN) && x.SCANTYPE == "SystemSN").ToList();

                if (Stations.StationActions.ActionRunners.LabelPrintAction.countCIS == 2)
                {
                     MesKp_temp = DB.ORM.Queryable<R_SN_KP>()
                    .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("ODA_HA-0") && k.SCANTYPE == "SMSN") //Maybe change to HASN
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .ToList();

                    smModule = MesKp_temp.Select(s => s.VALUE).ToList();
                    MesKp = DB.ORM.Queryable<R_SN_KP>()
                        .Where((x) => smModule.Contains(x.SN) && x.SCANTYPE == "SystemSN").ToList();
                    O_SSN.Value = smModule[0].ToString();
                }
                if (Stations.StationActions.ActionRunners.LabelPrintAction.countCIS == 3)
                {
                    MesKp_temp = DB.ORM.Queryable<R_SN_KP>()
                    .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("ODA_HA-1") && k.SCANTYPE == "SMSN") //Maybe change to HASN
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .ToList();

                    smModule = MesKp_temp.Select(s => s.VALUE).ToList();
                    MesKp = DB.ORM.Queryable<R_SN_KP>()
                        .Where((x) => smModule.Contains(x.SN) && x.SCANTYPE == "SystemSN").ToList();
                    O_SSN.Value = smModule[0].ToString();
                }
            } //end
            else
            { 
                MesKp = DB.ORM.Queryable<R_SN_KP>()
                    .Where(k => k.R_SN_ID == SN.ID && !k.KP_NAME.Contains("CHASSIS") && k.SCANTYPE== "SystemSN")
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .ToList();

                if (MesKp.Count==0)
                {
                    MesKp = DB.ORM.Queryable<R_SN_KP>()
                       .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("CHASSIS") && k.SCANTYPE == "SystemSN")
                       .OrderBy(t => t.ITEMSEQ)
                       .OrderBy(t => t.SCANSEQ)
                       .OrderBy(t => t.DETAILSEQ)
                       .ToList();
                }
                if (MesKp.Count == 0)
                {
                    MesKp = DB.ORM.Queryable<R_SN_KP>()
                       .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("MBX") && k.SCANTYPE == "SN")
                       .OrderBy(t => t.ITEMSEQ)
                       .OrderBy(t => t.SCANSEQ)
                       .OrderBy(t => t.DETAILSEQ)
                       .ToList();
                }
            }
            //TRKP.GetKPRecordBySnID(SN.ID, DB);
            List<mfsyscserial> TempMesKp = new List<mfsyscserial>();
            List<mfsyscserial> SfcKp = new List<mfsyscserial>();

            List<string> keys = MesKp.Select(t => t.VALUE).ToList();

            var TempMesKp1 = DB.ORM.Queryable<mfsyscserial>().Where((m) => keys.Contains(m.SYSSERIALNO)).ToList();
            TempMesKp.AddRange(TempMesKp1);

            List<string> KP_SN_IDs;
            while (TempMesKp.Count > 0)
            {
                AddList(SfcKp, TempMesKp);
                KP_SN_IDs = makeValueList(TempMesKp);
                var kpsns = DB.ORM.Queryable<mfsyscserial>().Where(t => KP_SN_IDs.Contains(t.SYSSERIALNO)).ToList();

                var tempkps = kpsns.Select(t => t.CSERIALNO).ToList();

                TempMesKp.Clear();
                if (tempkps.Count > 0)
                {
                    var T = DB.ORM.Queryable<mfsyscserial>().Where((e) => tempkps.Contains(e.SYSSERIALNO)).ToList();
                    AddList(TempMesKp, T);
                }
            }
            var macs = SfcKp.FindAll(t => t.EEECODE.Contains("MAC")).OrderBy(t => t.SCANDT).Distinct().ToList();

            var groupQty = macs.GroupBy(t => t.SYSSERIALNO).ToList();
            for (int i = 0; i < macs.Count; i++)
            {
                if (i == 0)
                {
                    var temp = macs[i];
                    O_MMAC0.Value = temp.CSERIALNO;
                    O_MSN0.Value = temp.SYSSERIALNO;
                    O_MAC0.Value = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                        temp.CSERIALNO.Substring(0, 2),
                        temp.CSERIALNO.Substring(2, 2),
                        temp.CSERIALNO.Substring(4, 2),
                        temp.CSERIALNO.Substring(6, 2),
                        temp.CSERIALNO.Substring(8, 2),
                        temp.CSERIALNO.Substring(10, 2));

                    O_MAC_QTY0.Value = groupQty.Find(s => s.Key == macs[i].SYSSERIALNO).ToList().Count;

                }

                if (i == 1)
                {
                    var temp = macs[i];
                    O_MMAC1.Value = temp.CSERIALNO;
                    O_MSN1.Value = temp.SYSSERIALNO;
                    O_MAC1.Value = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                        temp.CSERIALNO.Substring(0, 2),
                        temp.CSERIALNO.Substring(2, 2),
                        temp.CSERIALNO.Substring(4, 2),
                        temp.CSERIALNO.Substring(6, 2),
                        temp.CSERIALNO.Substring(8, 2),
                        temp.CSERIALNO.Substring(10, 2));
                    O_MAC_QTY1.Value = groupQty.Find(s => s.Key == macs[i].SYSSERIALNO).ToList().Count;

                }
                if (i == 2)
                {
                    var temp = macs[i];
                    O_MMAC2.Value = temp.CSERIALNO;
                    O_MSN2.Value = temp.SYSSERIALNO;
                    O_MAC2.Value = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                        temp.CSERIALNO.Substring(0, 2),
                        temp.CSERIALNO.Substring(2, 2),
                        temp.CSERIALNO.Substring(4, 2),
                        temp.CSERIALNO.Substring(6, 2),
                        temp.CSERIALNO.Substring(8, 2),
                        temp.CSERIALNO.Substring(10, 2));
                    O_MAC_QTY2.Value = groupQty.Find(s => s.Key == macs[i].SYSSERIALNO).ToList().Count;

                }
                if (i == 3)
                {
                    var temp = macs[i];
                    O_MMAC3.Value = temp.CSERIALNO;
                    O_MSN3.Value = temp.SYSSERIALNO;
                    O_MAC3.Value = String.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                        temp.CSERIALNO.Substring(0, 2),
                        temp.CSERIALNO.Substring(2, 2),
                        temp.CSERIALNO.Substring(4, 2),
                        temp.CSERIALNO.Substring(6, 2),
                        temp.CSERIALNO.Substring(8, 2),
                        temp.CSERIALNO.Substring(10, 2));
                    O_MAC_QTY3.Value = groupQty.Find(s => s.Key == macs[i].SYSSERIALNO).ToList().Count;

                }
            }
            #endregion End get Mac in SFC

        }

        List<string> makeValueList(List<R_SN_KP> R)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < R.Count; i++)
            {
                ret.Add(R[i].VALUE);
            }

            return ret;
        }

        List<string> makeValueList(List<mfsyscserial> R)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < R.Count; i++)
            {
                ret.Add(R[i].SYSSERIALNO);
            }
            return ret;
        }

        void AddList(List<R_SN_KP> Record, List<R_SN_KP> Add)
        {
            for (int i = 0; i < Add.Count; i++)
            {
                Record.Add(Add[i]);
            }
        }
        void AddList(List<mfsyscserial> Record, List<mfsyscserial> Add)
        {
            for (int i = 0; i < Add.Count; i++)
            {
                Record.Add(Add[i]);
            }
        }
    }
}
