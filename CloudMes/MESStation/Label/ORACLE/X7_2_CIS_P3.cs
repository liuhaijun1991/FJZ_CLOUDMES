using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using MESStation.LogicObject;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;

namespace MESStation.Label.ORACLE
{
    public class X7_2_CIS_P3 : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "string", StationSessionType = "SN", StationSessionKey = "1", Value = "" };

        LabelOutput O_SERIAL = new LabelOutput() { Name = "SERIAL", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MODEL = new LabelOutput() { Name = "MODEL", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SSN = new LabelOutput() { Name = "SSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUDCRP = new LabelOutput() { Name = "SKUDCRP", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PAGE1 = new LabelOutput() { Name = "PAGE1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PAGE2 = new LabelOutput() { Name = "PAGE2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_PN = new LabelOutput() { Name = "PN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_PN_DEC = new LabelOutput() { Name = "PN_DEC", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };

        LabelOutput O_CISFLAG = new LabelOutput() { Name = "CISFLAG", Type = LabelOutPutTypeEnum.String, Description = "CISFLAG", Value = "" };
        public X7_2_CIS_P3()
        {
            this.Inputs.Add(I_SN);

            this.Outputs.Add(O_SERIAL);
            this.Outputs.Add(O_MODEL);
            this.Outputs.Add(O_SSN);
            this.Outputs.Add(O_SKUNO);
            this.Outputs.Add(O_SKUDCRP);
            this.Outputs.Add(O_PAGE1);
            this.Outputs.Add(O_PAGE2);

            this.Outputs.Add(O_QTY);
            this.Outputs.Add(O_PN);
            this.Outputs.Add(O_PN_DEC);

            this.Outputs.Add(O_CISFLAG);
        }

        public override void MakeLabel(OleExec DB)
        {
            string strSN = I_SN.Value.ToString();
            SN SN = new SN(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            SKU sku = new SKU();
            sku.InitBySn(strSN, DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string strWo = SN.WorkorderNo;
            T_C_SERIES t_c_series = new T_C_SERIES(DB, DB_TYPE_ENUM.Oracle);
            C_SERIES c_series = t_c_series.GetDetailById(DB, sku.CSeriesId);//sku.CSeriesId
            if (c_series == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "SERIES" }));
            }
            //add by James zhu 08/22/2019 for oracle request to show differnt sku from cis page
            string strSKUSQL = $" SELECT case when B.SKUNO is not null then B.value3 else A.SKUNO end SKUNO FROM (SELECT* FROM C_SKU A WHERE A.SKUNO ='{SN.SkuNo}' )A LEFT JOIN C_SNBOM_PN_Mapping B on A.SKUNO = B.SKUNO ";
            DataTable dt2 = DB.ExecSelect(strSKUSQL).Tables[0];
            String strSKUNO = dt2.Rows[0]["skuno"].ToString();

            O_MODEL.Value = SN.SkuNo;
            O_SSN.Value = SN.baseSN.SN;
            O_SKUNO.Value = strSKUNO;
            //O_SKUNO.Value = SN.SkuNo;
            O_SERIAL.Value = c_series.SERIES_NAME;
            O_SKUDCRP.Value = sku.Description;

            //List<C_MMPRODMASTER> mm = DB.ORM.Queryable<C_MMPRODMASTER>().Where(t => t.PARTNO == sku.SkuNo).ToList();
            //if (mm.Count > 0)
            //{
            //    O_SKUDCRP.Value = mm[0].DESCRIPTION;
            //}
            //string BaseSkuNo = SN.SkuNo.Substring(0, SN.SkuNo.IndexOf("-"));
            string BaseSkuNo = SN.SkuNo;
            if (BaseSkuNo.Contains("-"))
            {
                BaseSkuNo = BaseSkuNo.Substring(0, BaseSkuNo.IndexOf("-"));
            }
            // add by James Zhu @8/21/2019 to exclude some label kp name list
            var noCisKPlist = DB.ORM.Queryable<C_CONTROL>().Where((L) => L.CONTROL_NAME == "SKIPCISLIST").Select((t) => t.CONTROL_VALUE).ToList();
            //var res = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.R_SN_ID == SN.baseSN.ID && k.STATION == "PACKOUT" && k.DETAILSEQ == 1)
            //    .GroupBy((k) => k.PARTNO).GroupBy((k) => k.KP_NAME)
            //    .Select((k) => new { k.PARTNO, k.KP_NAME, Count = SqlSugar.SqlFunc.AggregateCount(1) })
            //    .ToList();

            var res = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.R_SN_ID == SN.baseSN.ID && k.STATION == "PACKOUT" && k.DETAILSEQ == 1 && !noCisKPlist.Contains(k.KP_NAME))
               .GroupBy((k) => k.PARTNO).GroupBy((k) => k.KP_NAME)
               .Select((k) => new { k.PARTNO, k.KP_NAME, Count = SqlSugar.SqlFunc.AggregateCount(1) })
               .ToList();

            //CIS for ODA-HA vince_20190827
            if (c_series.SERIES_NAME.Contains("ODA_HA"))
            {
                if (Stations.StationActions.ActionRunners.LabelPrintAction.countCIS == 1)
                {
                    //remove strSN as empty to split accessory kit to child vince_20190926
                    res = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.SN == "empty" && k.STATION == "PACKOUT" && k.DETAILSEQ == 1 && !noCisKPlist.Contains(k.KP_NAME))
                   .GroupBy((k) => k.PARTNO).GroupBy((k) => k.KP_NAME)
                   .Select((k) => new { k.PARTNO, k.KP_NAME, Count = SqlSugar.SqlFunc.AggregateCount(1) / 2 }) //split to 2
                   .ToList();
                    O_SSN.Value = strSN;
                }
                if (Stations.StationActions.ActionRunners.LabelPrintAction.countCIS == 2)
                {
                    List<R_SN_KP> MesKp_temp = DB.ORM.Queryable<R_SN_KP>()
                    .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("ODA_HA-0") && k.SCANTYPE == "SMSN") 
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .ToList();

                    var smModule = MesKp_temp.Select(s => s.VALUE).ToList();
                    //remove to split accessory kit to child vince_20190926
                    string strTopSN = strSN;
                    strSN = smModule[0].ToString();   
                    res = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.SN == strTopSN && k.STATION == "PACKOUT" && k.DETAILSEQ == 1 && !noCisKPlist.Contains(k.KP_NAME))
                   .GroupBy((k) => k.PARTNO).GroupBy((k) => k.KP_NAME)
                   .Select((k) => new { k.PARTNO, k.KP_NAME, Count = SqlSugar.SqlFunc.AggregateCount(1) / 2 }) //split to 2
                   .ToList();
                    O_SSN.Value = strSN;
                }
                if (Stations.StationActions.ActionRunners.LabelPrintAction.countCIS == 3)
                {
                    List<R_SN_KP> MesKp_temp = DB.ORM.Queryable<R_SN_KP>()
                    .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("ODA_HA-1") && k.SCANTYPE == "SMSN")
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .ToList();

                    var smModule = MesKp_temp.Select(s => s.VALUE).ToList();
                    //remove to split accessory kit to child vince_20190926
                    string strTopSN = strSN;
                    strSN = smModule[0].ToString();
                    res = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.SN == strTopSN && k.STATION == "PACKOUT" && k.DETAILSEQ == 1 && !noCisKPlist.Contains(k.KP_NAME))
                    .GroupBy((k) => k.PARTNO).GroupBy((k) => k.KP_NAME)
                    .Select((k) => new { k.PARTNO, k.KP_NAME, Count = SqlSugar.SqlFunc.AggregateCount(1) / 2 })  //split to 2
                    .ToList();
                    O_SSN.Value = strSN;
                }
            }            
            //end

            var kp = res.Select(t => t.PARTNO).ToList();
            //changed by James Zhu 02052020 to get all description from C_ORACLE_MFASSEMBLYDATA Table
            // var des = DB.ORM.Queryable<C_MMPRODMASTER>().Where((m) => kp.Contains(m.PARTNO)).ToList();
            var des = DB.ORM.Queryable<C_ORACLE_MFASSEMBLYDATA>().Where((m) => kp.Contains(m.CUSTPARTNO)).ToList();

            for (int i = 0; i < res.Count; i++)
            {
                ((List<string>)O_QTY.Value).Add(res[i].Count.ToString());
                ((List<string>)O_PN.Value).Add(res[i].PARTNO.ToString());

                string strDesc = "";
                try
                {
                    string partno = res[i].PARTNO.ToString();
                  //  strDesc = des.Find(t => t.PARTNO == partno).DESCRIPTION.Replace("?","");
                    strDesc = des.Find(t => t.CUSTPARTNO  == partno).DESCRIPTION.Replace("?", "");
                }
                catch
                { }
                ((List<string>)O_PN_DEC.Value).Add(strDesc);
            }
        }

    }
}
