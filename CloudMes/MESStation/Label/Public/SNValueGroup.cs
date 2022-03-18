using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using MESPubLab.MESStation.SNMaker;
using System;
using System.Collections.Generic;

namespace MESStation.Label.Public
{
    public class SNValueGroup : LabelValueGroup
    {
        public SNValueGroup()
        {
            ConfigGroup = "SNValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKeyPartValue", Description = "Get Keypart Value List By Keypart Type", Paras = new List<string>() { "SN", "PN", "KP_TYPE" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKeyPartValueByKPName", Description = "Get Keypart Value List By Keypart Name", Paras = new List<string>() { "SN", "KPName" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKeyPartNoByKPName", Description = "Get Keypart PartNo List By Keypart Name", Paras = new List<string>() { "SN", "KPName" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKeyPartExtValue", Description = "Get Keypart Ext Value List By Keypart Partno & KP_Type", Paras = new List<string>() { "SN", "PN", "KP_TYPE", "ExtKey" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKeyPartValueByLocation", Description = "Get Keypart Value By Location & ScanType", Paras = new List<string>() { "SN", "LOCATION", "SCANTYPE" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSNByCSN", Description = "Get R_SN_LINK SN Value By CSN", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKPValueByTypeAndSeq", Description = "Get Keypart Value By ScanType & ScanSeq", Paras = new List<string>() { "SN", "SCANTYPE", "SCANSEQ" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetboxsnBySN", Description = "GetboxsnBySN", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKPValueByKpNameAndType", Description = "GetKPValueByKpNameAndType", Paras = new List<string>() { "SN", "KP_NAME", "ScanType" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetKPValueByNameAndTypeAndPartno", Description = "GetKPValueByKpNameAndType", Paras = new List<string>() { "SN", "KP_NAME", "ScanType","Partno" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetNewSNByWoQty", Description = "", Paras = new List<string>() { "WO", "QTY" } });

        }
        public List<string> GetNewSNByWoQty(OleExec SFCDB, string WO, string QTY)
        {
            var ret = new List<string>();

            var r_wo = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WO).First();

            if (WO != null)
            {
                var c_sku = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == r_wo.SKUNO).First();
                int q = int.Parse(QTY);
                for (int i = 0; i < q; i++)
                {
                    var sn = SNmaker.GetNextSN(c_sku.SN_RULE, SFCDB.ORM);
                    ret.Add(sn);
                }
            }

            return ret;
        }

        public string GetKPValueByTypeAndSeq(OleExec SFCDB, string SN, string ScanType, string ScanSeq)
        {
            var s = SFCDB.ORM.Queryable<R_SN_KP>()
               .Where(t => t.SN == SN && t.SCANTYPE == ScanType && t.SCANSEQ == Convert.ToInt32(ScanSeq) && t.VALID_FLAG == 1)
               .Select(t => t.VALUE)
               .First();
            var SQL = SFCDB.ORM.Queryable<R_SN_KP>()
               .Where(t => t.SN == SN && t.SCANTYPE == ScanType && t.SCANSEQ == Convert.ToInt32(ScanSeq) && t.VALID_FLAG == 1)
               .Select(t => t.VALUE).ToSql();
            return s ?? "";
        }

        public string GetKeyPartValueByLocation(OleExec SFCDB, string SN, string Location, string ScanType)
        {
            var s = SFCDB.ORM.Queryable<R_SN_KP>()
               .Where(t => t.SN == SN && t.LOCATION == Location && t.SCANTYPE == ScanType && t.VALID_FLAG == 1)
               .Select(t => t.VALUE)
               .First();

            if (Location.Equals("HP SN") && ScanType.Equals("TEMP S/N1"))
            {
                var rev = SFCDB.ORM.Queryable<C_SKU_DETAIL, R_SN>((t1, t2) => t1.SKUNO == t2.SKUNO).Where((t1, t2) => t2.SN == SN && t2.VALID_FLAG == "1" && t1.CATEGORY_NAME == "REV").Select((t1, t2) => t1.VALUE).First();
                if (rev != null && s != null)
                {
                    return s + "-" + rev;
                }
            }
            return s ?? "";
        }

        public List<string> GetKeyPartValue(OleExec SFCDB, string SN, string PN, string KP_TYPE)
        {
            var s = SFCDB.ORM.Queryable<R_SN_KP>()
               .Where(t => t.SN == SN && t.PARTNO == PN && t.SCANTYPE == KP_TYPE && t.VALID_FLAG == 1)
               .Select(t => t.VALUE)
               .ToList();
            return s;
        }

        public List<string> GetKeyPartValueByKPName(OleExec SFCDB, string SN, string KPName)
        {
            var s = SFCDB.ORM.Queryable<R_SN_KP>()
               .Where(t => t.SN == SN && t.KP_NAME == KPName && t.VALID_FLAG == 1)
               .OrderBy(t=>t.ITEMSEQ,SqlSugar.OrderByType.Asc)
               .OrderBy(t=>t.SCANSEQ,SqlSugar.OrderByType.Asc)
               .OrderBy(t=>t.DETAILSEQ,SqlSugar.OrderByType.Asc)
               .Select(t => t.VALUE)
               .ToList();
            return s;
        }
        public List<string> GetKeyPartNoByKPName(OleExec SFCDB, string SN, string KPName)
        {
            var s = SFCDB.ORM.Queryable<R_SN_KP>()
               .Where(t => t.SN == SN && t.KP_NAME == KPName && t.VALID_FLAG == 1)
               .OrderBy(t => t.ITEMSEQ, SqlSugar.OrderByType.Asc)
               .OrderBy(t => t.SCANSEQ, SqlSugar.OrderByType.Asc)
               .OrderBy(t => t.DETAILSEQ, SqlSugar.OrderByType.Asc)
               .Select(t => t.PARTNO)
               .ToList();
            return s;
        }

        public List<string> GetKeyPartExtValue(OleExec SFCDB, string SN, string PN, string KP_TYPE, string ExtKey)
        {
            var s = SFCDB.ORM.Queryable<R_SN_KP>()
               .Where(t => t.SN == SN && t.PARTNO == PN && t.SCANTYPE == KP_TYPE && t.VALID_FLAG == 1)
               .Select(t => new {t.EXKEY1,t.EXVALUE1,t.EXKEY2,t.EXVALUE2 })
               .ToList();
            var res = new List<string>();
            for (int i = 0; i < s.Count; i++)
            {
                if (s[i].EXKEY1==ExtKey)
                {
                    res.Add(s[i].EXVALUE1);
                }
                else if (s[i].EXKEY2 == ExtKey)
                {
                    res.Add(s[i].EXVALUE2);
                }
                
            }
            return new List<string>();
        }

        public string GetSNByCSN(OleExec SFCDB, string CSN)
        {
            string SN = "";
            try
            {
                SN = SFCDB.ORM.Queryable<R_SN_LINK>().Where(t => t.CSN == CSN && t.VALIDFLAG == "1").Select(t => t.SN).First();
                if (SN == null)
                {
                    SN = "No data record";
                }
            }
            catch (Exception ex)
            {
                SN = ex.Message;
            }
            return SN;
        }

        public string GetboxsnBySN(OleExec SFCDB, string SN)
        {
            string boxsn = "";
            try
            {
                boxsn = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").Select(t => t.BOXSN).First();
                if (boxsn == null)
                {
                    boxsn = "No data record";
                }
            }
            catch (Exception ex)
            {
                boxsn = ex.Message;
            }
            return boxsn;
        }

        public string GetKPValueByKpNameAndType(OleExec SFCDB, string SN,string KP_NAME,string ScanType)
        {
            string output = "";
            List<string> listKP = SFCDB.ORM.Queryable<R_SN_KP>()
                .Where(r => r.SN == SN && r.KP_NAME == KP_NAME && r.SCANTYPE == ScanType && r.VALID_FLAG == 1)
                .Select(r=>r.VALUE).ToList();
            if (listKP.Count > 0)
            {
                output = listKP[0];
            }
            return output;
        }

        public string GetKPValueByNameAndTypeAndPartno(OleExec SFCDB, string SN, string KP_NAME, string ScanType,string Partno)
        {
            string output = "";
            List<string> listKP = SFCDB.ORM.Queryable<R_SN_KP>()
                .Where(r => r.SN == SN && r.KP_NAME == KP_NAME && r.SCANTYPE == ScanType && r.VALID_FLAG == 1 &&r.PARTNO== Partno)
                .Select(r => r.VALUE).ToList();
            if (listKP.Count > 0)
            {
                output = listKP[0];
            }
            return output;
        }
    }
}
