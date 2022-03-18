using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;

namespace MESStation.Label.Public
{
    public class WOValueGroup : LabelValueGroup
    {
        public WOValueGroup()
        {
            ConfigGroup = "WOValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWO", Description = "Get Workorder By SN", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWO_SKUVER", Description = "Get Workorder Sku Version", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetR_WO_BASE_VALUE", Description = "Get Value From R_WO_BASE Table By Column Name", Paras = new List<string>() { "WO", "COLUMN_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWONewSNRange", Description = "Get WO SN Range For Kitting Print", Paras = new List<string>() { "WO", "QTY", "STATION", "EMP_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWONewSNReprint", Description = "Get WO SN Witch Kitting Printed", Paras = new List<string>() { "WO", "SN", "STATION", "EMP_NO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWO_PO", Description = "Get Workorder PO", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSKUNewSNRange",Description = "Get SKU SN Range For Kitting Print", Paras = new List<string>() { "SKU","QTY", "STATION" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSkuAndVer", Description = "Get SKU and Skuver For Kitting Print", Paras = new List<string>() { "WO" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWOHwVersion", Description = "Get Workorder HW Version(WITH OS)", Paras = new List<string>() { "SN" } });
        }

        public string GetSkuAndVer(OleExec SFCDB, string WO)
        {
            var strSql = $@"SELECT C.SKUNO||'R'||C.SKU_VER AS SKUANDVER FROM R_WO_BASE C WHERE C.WORKORDERNO='{WO}'";
            try
            {
                var res = SFCDB.RunSelect(strSql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    return res.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public string GetWO(OleExec SFCDB, string SN)
        {
            var Ver = "No data record";
            var s = SFCDB.ORM.Queryable<R_SN>()
            .Where(S => S.SN == SN && S.VALID_FLAG == "1")
            .Select(S => S.WORKORDERNO)
            .ToList();
            if (s.Count > 0)
            {
                Ver = s[0];
            }
            return Ver;
        }

        public string GetWO_SKUVER(OleExec SFCDB, string SN)
        {
            var Ver = "No data record";
            var s = SFCDB.ORM.Queryable<R_WO_BASE, R_SN>((W, S) => new object[] { SqlSugar.JoinType.Left, W.WORKORDERNO == S.WORKORDERNO })
            .Where((W, S) => S.SN == SN && S.VALID_FLAG == "1")
            .Select((W, S) => W.SKU_VER)
            .ToList();
            if (s.Count > 0)
            {
                Ver = s[0];
            }
            return Ver;
        }

        public string GetR_WO_BASE_VALUE(OleExec SFCDB, string WO, string COLUMN_NAME)
        {
            string value = "No data record";
            List<string> sl = SFCDB.ORM.Queryable<R_WO_BASE>()
                .Where(t => t.WORKORDERNO == WO)
                .Select<string>(COLUMN_NAME)
                .ToList();
            if (sl.Count > 0)
            {
                value = sl[0];
            }
            return value;
        }

        public List<string> GetWONewSNRange(OleExec SFCDB, string WO, string QTY, string STATION, string EMP_NO)
        {
            List<string> sns = new List<string>();
            List<R_SN_STATION_DETAIL> sds = new List<R_SN_STATION_DETAIL>();
            var rules = SFCDB.ORM.Queryable<R_WO_BASE, C_SKU, C_SN_RULE>((W, S, R) => new object[] { SqlSugar.JoinType.Left, W.SKUNO == S.SKUNO, SqlSugar.JoinType.Left, S.SN_RULE == R.NAME })
                .Where((W, S, R) => W.WORKORDERNO == WO)
                .Select((W, S, R) => new { R.ID, W.WORKORDER_QTY })
                .ToList();
            var qty = int.Parse(QTY);
            if (rules[0].ID == null)
            {
                sns.Add("Need SN Rule!");
            }
            else
            {
                for (int i = 0; i < qty; i++)
                {
                    string sn = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(rules[0].ID, SFCDB);
                    R_SN_STATION_DETAIL sd = new R_SN_STATION_DETAIL() { WORKORDERNO = WO, SN = sn, EDIT_EMP = EMP_NO, EDIT_TIME = DateTime.Now, STATION_NAME = STATION };
                    sds.Add(sd);
                    sns.Add(sn);
                }
                try
                {
                    int n = SFCDB.ORM.Insertable<R_SN_STATION_DETAIL>(sds).ExecuteCommand();
                    if (n < sds.Count)
                    {
                        sns.Clear();
                        sns.Add("Insert Log Fail!");
                    }
                }
                catch (Exception)
                {
                    sns.Add("Insert Log Fail!");
                }
            }
            return sns;
        }

        public List<string> GetSKUNewSNRange(OleExec SFCDB, string SKU, string QTY, string STATION/*, string EMP_NO*/)
        {
            List<string> sns = new List<string>();
            List<R_SN_STATION_DETAIL> sds = new List<R_SN_STATION_DETAIL>();
            var rules = SFCDB.ORM.Queryable<C_SKU, C_SN_RULE>((S, R) => new object[] { SqlSugar.JoinType.Left, S.SN_RULE == R.NAME })
                .Where((S, R) =>S.SKUNO == SKU)
                .Select((S, R) => new { R.ID})
                .ToList();
            var qty = int.Parse(QTY);
            if (rules[0].ID == null)
            {
                sns.Add("Need SN Rule!");
            }
            else
            {
                for (int i = 0; i < qty; i++)
                {
                    string sn = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(rules[0].ID, SFCDB);
                    R_SN_STATION_DETAIL sd = new R_SN_STATION_DETAIL() {SN = sn, SKUNO= rules[0].ID, EDIT_TIME = DateTime.Now, STATION_NAME = STATION };
                    sds.Add(sd);
                    sns.Add(sn);
                }
                try
                {
                    int n = SFCDB.ORM.Insertable<R_SN_STATION_DETAIL>(sds).ExecuteCommand();
                    if (n < sds.Count)
                    {
                        sns.Clear();
                        sns.Add("Insert Log Fail!");
                    }
                }
                catch (Exception)
                {
                    sns.Add("Insert Log Fail!");
                }
            }
            return sns;
        }

        public List<string> GetWONewSNReprint(OleExec SFCDB, string WO, string SN, string STATION, string EMP_NO) 
        {
            List<string> sns = new List<string>();
            List<R_SN_STATION_DETAIL> sds = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.WORKORDERNO == WO && t.SN == SN && t.STATION_NAME == "KIT_PRINT").ToList();
            //重工工單補打LABEL不需要卡
            var wotype = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WO && t.WO_TYPE == "REWORK").ToList(); 
            if (sds.Count == 0&& wotype.Count==0)
            {
                sns.Add("WO Not Contains This SN!");
            }
            else
            {
                R_SN_STATION_DETAIL sd = new R_SN_STATION_DETAIL() { WORKORDERNO = WO, SN = SN, EDIT_EMP = EMP_NO, EDIT_TIME = DateTime.Now, STATION_NAME = STATION };
                sds.Add(sd);
                sns.Add(SN);
                try
                {
                    SFCDB.ORM.Insertable<R_SN_STATION_DETAIL>(sd).ExecuteCommand();
                }
                catch (Exception)
                {
                    sns.Add("Insert Log Fail!");
                }
            }
            return sns;
        }

        public string GetWO_PO(OleExec SFCDB, string SN)
        {
            var PO = "No data record";

            var s = SFCDB.ORM.Queryable<R_F_CONTROL, R_SN>((rfc, rs) => rfc.VALUE == rs.WORKORDERNO).Where((rfc, rs) => rs.SN == SN).Select((rfc, rs) => rfc.EXTVAL).ToList();

            if (s.Count > 0)
            {
                PO = s[0];
            }
            return PO;


        }

        public string GetWOHwVersion(OleExec SFCDB, string SN)
        {
            var hwVersion = "No data record";
            var s = SFCDB.ORM.Queryable<R_F_CONTROL, R_SN>((rfc, rs) => rfc.VALUE == rs.WORKORDERNO)
                .Where((rfc, rs) => rs.SN == SN && rfc.FUNCTIONNAME == "WITH OS" && rfc.CONTROLFLAG == "Y" && rfc.FUNCTIONTYPE == "NOSYSTEM").Select((rfc, rs) => rfc.EXTVAL).ToList();
            if (s.Count > 0)
            {
                hwVersion = s[0];
            }
            return hwVersion;
        }
    }
}
