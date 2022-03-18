using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace MESReport.BaseReport.ORACLE
{
    public class ORA_SKUSettingReport: ReportBase
    {
        ReportInput SKU = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        public ORA_SKUSettingReport()
        {
            Inputs.Add(SKU);
            string strGetSKURep = @"select * from table(SFC.FTX_SKU_STATUS_F('{0}'))";

            Sqls.Add("strGetSKURep", strGetSKURep);

            string strGetNEWPN = @"select distinct W.WO,W.SKUNO, B.MATNR, 'NEW PN' STATUS, 'Please maintain new PN to proper list: ' || B.MATNR MESSAGE
                                from R_MFPRESETWOHEAD W inner join R_WO_BOM B on W.WO = B.AUFNR 
                                where (B.MATNR not in (select distinct PARTNO from C_KP_GROUP_PARTNO) 
                                and B.MATNR not in (select distinct PARTNO from C_PARTNO_EXCEPTION where EXCEPTIONTYPE = 'NoNeedScan') )
                                and B.MATNR not like '%-A' and W.SKUNO like '%{0}%' order by  SKUNO,WO";

            Sqls.Add("strGetNEWPN", strGetNEWPN);
            

            string strGetLocationIssue = @"select distinct W.workorderno,W.SKUNO, B.MATNR,MP.description, 'Location Issue' STATUS, 'Please check location setup for PN: ' || B.MATNR MESSAGE"+
                                " from R_WO_BASE W inner join R_WO_BOM B on W.workorderno = B.AUFNR " +
                                " inner join C_MMPRODMASTER MP on MP.PARTNO = B.MATNR " +
                                " left join c_oracle_mfassemblydata m on m.configheaderid = W.SKUNO and m.custpartno = MP.PARTNO" +
                                " where(B.MATNR in (select distinct PARTNO from C_KP_GROUP_PARTNO) and " +
                                " B.MATNR not in (select distinct PARTNO from C_PARTNO_EXCEPTION where EXCEPTIONTYPE = 'SkipAssyLocationChk') )" +
                                " and(M.LOCATION is null or M.QTY <> B.MENGE / W.WORKORDER_QTY) and W.closed_flag = 0 "+ 
                                "and W.INPUT_QTY = 0 and W.SKUNO like '%%' order by  SKUNO, workorderno ";

            Sqls.Add("strGetLocationIssue", strGetLocationIssue);

            string strGetKPRuleIssue= @"select distinct P.PARTNO, D.SCANTYPE, 'KP Rule Issue' STATUS, 'Please maintain KP Rule for PN: ' ||P.PARTNO MESSAGE " +
                                        " from C_KP_LIST K inner join C_KP_LIST_ITEM I on K.ID = I.LIST_ID " +
                                        " inner join C_KP_GROUP G on G.GROUPNAME = I.KP_PARTNO " +
                                        " inner join C_KP_GROUP_PARTNO P on P.KP_GROUP_ID = G.ID " +
                                        " inner join C_KP_LIST_ITEM_DETAIL D on D.ITEM_ID = I.ID " + 
                                        " left join C_KP_RULE R on R.PARTNO = P.PARTNO and R.SCANTYPE = D.SCANTYPE " +
                                        " where R.PARTNO is null and K.FLAG='1' ";

            Sqls.Add("strGetKPRuleIssue", strGetKPRuleIssue);

            string strGetMPNIssue = @"select distinct K.SKUNO PF,P.PARTNO, 'MPN setup Issue' STATUS, 'Please maintain MPN setup for PN: ' ||P.PARTNO MESSAGE " +
                                        " from C_KP_LIST K inner join C_KP_LIST_ITEM I on K.ID = I.LIST_ID " +
                                        " inner join C_KP_GROUP G on G.GROUPNAME = I.KP_PARTNO " +
                                        " inner join C_KP_GROUP_PARTNO P on P.KP_GROUP_ID = G.ID " +
                                        " left join C_SKU_MPN M on M.SKUNO = K.SKUNO and M.PARTNO = P.PARTNO " +
                                        " where M.PARTNO is null  and K.FLAG='1 order by PF, PARTNO ";

            Sqls.Add("strGetMPNIssue", strGetMPNIssue);



        }

        public override void Run()
        {
            //string runSql = string.Format(Sqls["strGetSN"], SKU.Value.ToString());
            string runSql = string.Format(Sqls["strGetSKURep"], SKU.Value.ToString());
            string runSql1 = string.Format(Sqls["strGetNEWPN"], SKU.Value.ToString());
            string runSql2 = string.Format(Sqls["strGetLocationIssue"], SKU.Value.ToString());
            string runSql3 = string.Format(Sqls["strGetKPRuleIssue"]);
            string runSql4 = string.Format(Sqls["strGetMPNIssue"]);

            RunSqls.Add(runSql);
            RunSqls.Add(runSql1);
            RunSqls.Add(runSql2);
            RunSqls.Add(runSql3);
            RunSqls.Add(runSql4);

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet res = SFCDB.RunSelect(runSql);
                DataSet res1 = SFCDB.RunSelect(runSql1);
                DataSet res2 = SFCDB.RunSelect(runSql2);
                DataSet res3 = SFCDB.RunSelect(runSql3);
                DataSet res4 = SFCDB.RunSelect(runSql4);

                ReportTable retTab = new ReportTable();
                DataTable dt = res.Tables[0].Copy();
                //DataTable linkTable = new DataTable();
                //DataRow linkRow;
                //foreach (DataColumn column in res.Tables[0].Columns)
                //{
                //    linkTable.Columns.Add(column.ColumnName);
                //}
                //foreach (DataRow row in res.Tables[0].Rows)
                //{

                //    linkRow = linkTable.NewRow();
                //    foreach (DataColumn dc in linkTable.Columns)
                //    {
                //        if (dc.ColumnName.ToString().ToUpper() == "SKU")
                //        {


                //        }
                //    }
                //    linkTable.Rows.Add(linkRow);
                //}

                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "SKU_Status";
                Outputs.Add(retTab);

                ReportTable retTab1 = new ReportTable();
                retTab1.LoadData(res1.Tables[0], null);
                retTab1.Tittle = "NEW_PN_Issue";                
                //retTab1.ColNames.RemoveAt(0);
                Outputs.Add(retTab1);

                ReportTable retTab2 = new ReportTable();
                retTab2.LoadData(res2.Tables[0], null);
                retTab2.Tittle = "Location_Issue";
                //retTab1.ColNames.RemoveAt(0);
                Outputs.Add(retTab2);

                ReportTable retTab3 = new ReportTable();
                retTab3.LoadData(res3.Tables[0], null);
                retTab3.Tittle = "KP_RULE_Issue";
                //retTab1.ColNames.RemoveAt(0);
                Outputs.Add(retTab3);

                ReportTable retTab4 = new ReportTable();
                retTab4.LoadData(res4.Tables[0], null);
                retTab4.Tittle = "MPN_Setup_Issue";
                //retTab1.ColNames.RemoveAt(0);
                Outputs.Add(retTab4);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }
    }
}
