using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using MESStation.LogicObject;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
namespace MESStation.Label.ORACLE
{
    public class X7_2_CIS_P2 : LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "string", StationSessionType = "SN", StationSessionKey = "1", Value = "" };

        LabelOutput O_SERIAL = new LabelOutput() { Name = "SERIAL", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MODEL = new LabelOutput() { Name = "MODEL", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SSN = new LabelOutput() { Name = "SSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUDCRP = new LabelOutput() { Name = "SKUDCRP", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PAGE1 = new LabelOutput() { Name = "PAGE1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PAGE2 = new LabelOutput() { Name = "PAGE2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        LabelOutput O_CSN = new LabelOutput() { Name = "CSN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_PN = new LabelOutput() { Name = "PN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_DES = new LabelOutput() { Name = "DES", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };

        LabelOutput O_CISFLAG = new LabelOutput() { Name = "CISFLAG", Type = LabelOutPutTypeEnum.String, Description = "CISFLAG", Value = "" };

        public X7_2_CIS_P2()
        {
            this.Inputs.Add(I_SN);

            this.Outputs.Add(O_SERIAL);
            this.Outputs.Add(O_MODEL);
            this.Outputs.Add(O_SSN);
            this.Outputs.Add(O_SKUNO);
            this.Outputs.Add(O_SKUDCRP);
            this.Outputs.Add(O_PAGE1);
            this.Outputs.Add(O_PAGE2);
            this.Outputs.Add(O_CSN);
            this.Outputs.Add(O_PN);
            this.Outputs.Add(O_DES);

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

            //add by James zhu 08/21/2019 for oracle request to show differnt sku from cis page
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
            string BaseSkuNo = SN.SkuNo;
            if (BaseSkuNo.Contains("-"))
            {
                BaseSkuNo = BaseSkuNo.Substring(0, BaseSkuNo.IndexOf("-"));
            }
            #region Get Data For CloudMES
            /*
            List<string> ComponentPN = DB.ORM.Queryable<C_ORA_L6_MAPPING>().Where((e) => e.PRODUCT_TYPE == c_series.SERIES_NAME)
                .Select((e) => e.COMPONENT_PN).ToList();
            var ALLKP = DB.ORM.Queryable<R_SN_KP>().Where((k) => k.R_SN_ID == SN.ID && k.SCANTYPE != "MPN" && k.SCANTYPE != "PN" && !k.KP_NAME.StartsWith("MAC") && !k.STATION.Contains("PACKOUT"))
                .Select((k) => new { PVALUE = k.SN, VALUE = k.VALUE, TYPE = k.KP_NAME, PARTNO = k.PARTNO, EDIT_TIME = SqlSugar.SqlFunc.ToString(k.EDIT_TIME) })
                .ToList();
            var MESKP = ALLKP.FindAll(t => !t.TYPE.ToUpper().Contains("CABLE") && !t.TYPE.Contains("CONFIG_LBL")).ToList();
            var TempMesKp = ALLKP.ConvertAll(k => { return new { PVALUE = SqlSugar.SqlFunc.Trim(k.PVALUE), VALUE = SqlSugar.SqlFunc.Trim(k.VALUE), TYPE = SqlSugar.SqlFunc.Trim(k.TYPE), PARTNO = SqlSugar.SqlFunc.Trim(k.PARTNO), EDIT_TIME = k.EDIT_TIME }; });
            List<string> KP_SN_IDs = new List<string>();
            ALLKP.Clear();
            while (TempMesKp.Count > 0)
            {
                ALLKP.AddRange(TempMesKp);
                KP_SN_IDs = TempMesKp.Select(k => k.VALUE).ToList();
                var kpsns = DB.ORM.Queryable<mfsyscserial>().Where(t => KP_SN_IDs.Contains(t.SYSSERIALNO)).Select(t => t.CSERIALNO).ToList();
                TempMesKp.Clear();

                var T = DB.ORM.Queryable<mfsyscserial>().Where((e) => kpsns.Contains(e.SYSSERIALNO))
                    .Select((m) => new { PVALUE = SqlSugar.SqlFunc.Trim(m.SYSSERIALNO), VALUE = SqlSugar.SqlFunc.Trim(m.CSERIALNO), TYPE = SqlSugar.SqlFunc.Trim(m.EEECODE), PARTNO = SqlSugar.SqlFunc.Trim(m.PARTNO), EDIT_TIME = m.SCANDT })
                    .OrderBy((m) => m.EDIT_TIME)
                    .ToList();
                if (T.Count > 0)
                {
                    TempMesKp.AddRange(T);
                }
                KP_SN_IDs.Clear();
            }
            var res = ALLKP.FindAll(t => (ComponentPN.Contains(t.TYPE) || ComponentPN.Contains(t.PARTNO)) && !t.TYPE.Contains("MAC") && t.PVALUE != strSN);
            res.AddRange(MESKP);
            res = res.OrderBy(t => t.EDIT_TIME).ToList();
            var kps = res.Select(t => t.PARTNO).ToList();
            var des = DB.ORM.Queryable<C_MMPRODMASTER>().Where((m) => kps.Contains(m.PARTNO)).ToList();
            for (int i = 0; i < res.Count; i++)
            {
                ((List<string>)O_CSN.Value).Add(res[i].VALUE);
                ((List<string>)O_PN.Value).Add(res[i].PARTNO);

                string strDesc = "";
                try
                {
                    strDesc = des.Find(t => t.PARTNO == res[i].PARTNO).DESCRIPTION;
                }
                catch
                { }
                ((List<string>)O_DES.Value).Add(strDesc);
            }
            */
            #endregion End Get Data For CloudMES

            string SQL = "";
            string type = c_series.SERIES_NAME.Substring(c_series.SERIES_NAME.Length - 2, 2);
            switch (type)
            {
                case "-2":
                    SQL = @"select * from TABLE(SFC.FTX_ORA_SN_BOM('{0}'))";
                    break;
                case "2L":
                    SQL = @"select * from TABLE(SFC.FTX_ORA_SN_BOM('{0}'))";
                    break;
                case "X7":
                    SQL = @"select * from TABLE(SFC.FTX_ORA_SN_BOM('{0}'))";
                    break;
                case "2C":
                      if(c_series.SERIES_NAME=="E2-2C")
                    {
                        SQL = @"select * from TABLE(SFC.FTX_ORA_SN_BOM('{0}'))";
                    }
                      else
                    { 
                    SQL = @"select * from TABLE (SFC.FTX_ORA_SNBOM_7X2C_Chassis('{0}')) union
                            select * from TABLE (SFC.FTX_ORA_SNBOM_7X2C_Module0('{0}')) union
                            select * from TABLE (SFC.FTX_ORA_SNBOM_7X2C_Module1('{0}'))";
                    }
                    break;
                case "-8":
                    SQL = @"select * from TABLE (SFC.FTX_ORA_SNBOM_X88_SIDEA('{0}')) union
                            select * from TABLE (SFC.FTX_ORA_SNBOM_X88_SIDEB('{0}'))";
                    break;
                default:
                    throw new System.Exception(c_series.SERIES_NAME + " Not function support to get CIS data!");
            }

            //CIS for ODA-HA vince_20190827
            if (c_series.SERIES_NAME.Contains("ODA_HA"))
            {
                if (Stations.StationActions.ActionRunners.LabelPrintAction.countCIS == 2)
                {
                    List<R_SN_KP> MesKp_temp = DB.ORM.Queryable<R_SN_KP>()
                    .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("ODA_HA-0") && k.SCANTYPE == "SMSN") //Maybe change to HASN
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .ToList();

                    var smModule = MesKp_temp.Select(s => s.VALUE).ToList();
                    strSN = smModule[0].ToString();
                    O_SSN.Value = strSN.Replace("_","-");
                }
                if (Stations.StationActions.ActionRunners.LabelPrintAction.countCIS == 3)
                {
                    List<R_SN_KP> MesKp_temp = DB.ORM.Queryable<R_SN_KP>()
                    .Where(k => k.R_SN_ID == SN.ID && k.KP_NAME.Contains("ODA_HA-1") && k.SCANTYPE == "SMSN") //Maybe change to HASN
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .ToList();

                    var smModule = MesKp_temp.Select(s => s.VALUE).ToList();
                    strSN = smModule[0].ToString();
                    O_SSN.Value = strSN.Replace("_","-");
                }
            }         
            //end
            SQL = string.Format(SQL, strSN);
            SQL = SQL.Replace("r\n", " ").Replace('\n', ' ').Replace('\t', ' ');
            DataSet ds = DB.ExecSelect(SQL);
            DataTable dt = null;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (dt == null)
                {
                    dt = ds.Tables[i].Clone();
                }
                dt.Merge(ds.Tables[i]);

            }
            List<string> ExcludeStaiton = new List<string> { "ASSY1", "ASSY2", "ASSY3", "ASSY4", "ASSY5", "ASSY6", "VI1", "PACKOUT","OBA" };
            List<string> ExcludeScanType = new List<string> { "PN", "MPN", "MAC_CHECK", "MAC1", "MAC2", "MAC3" };
            DataTable OtherDt = DB.ORM.Queryable<R_SN_KP>()
                .Where(t => t.SN == strSN && !ExcludeStaiton.Contains(t.STATION) && !ExcludeScanType.Contains(t.SCANTYPE))
                .Select(t => new { WORKORDERNO = "GG", SN = t.VALUE, SKUNO = t.PARTNO, DESCRIPTION = "" })
                .ToDataTable();
            dt.Merge(OtherDt);
            DataView dv = new DataView(dt);
            DataTable data = dv.ToTable(true);
            var kps = (from d in data.AsEnumerable() select d.Field<string>("SKUNO")).ToList();

            //changed by James Zhu 02052020 to get all description from C_ORACLE_MFASSEMBLYDATA Table           
            var des = DB.ORM.Queryable<C_ORACLE_MFASSEMBLYDATA>().Where((m) => kps.Contains(m.CUSTPARTNO)).ToList();
            //var des = DB.ORM.Queryable<C_MMPRODMASTER>().Where((m) => kps.Contains(m.PARTNO)).ToList();

            List<string> uniquesn = new List<string>(); //add by James Zhu to fix X7-2C duplicated componnet issue by 2020/3/17

            for (int i = 0; i < data.Rows.Count; i++)
            {
                //exclude software from component list
                if ((data.Rows[i]["WORKORDERNO"].ToString() != "S") && (!uniquesn.Contains(data.Rows[i]["SN"].ToString())) )
                {
                    ((List<string>)O_CSN.Value).Add(data.Rows[i]["SN"].ToString().Replace("_","-"));
                    ((List<string>)O_PN.Value).Add(data.Rows[i]["SKUNO"].ToString());

                    string strDesc = "";
                    try
                    {
                        string partno = data.Rows[i]["SKUNO"].ToString();
                       // strDesc = des.Find(t => t.PARTNO == partno).DESCRIPTION.Replace("?", "");
                        strDesc = des.Find(t => t.CUSTPARTNO  == partno).DESCRIPTION.Replace("?", "");
                    }
                    catch
                    { }
                ((List<string>)O_DES.Value).Add(strDesc);
                    uniquesn.Add(data.Rows[i]["SN"].ToString());
                    uniquesn.Remove(" ");
                }               
            }
        }
    }
}
