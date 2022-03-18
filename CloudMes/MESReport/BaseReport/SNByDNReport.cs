using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SNByDNReport: ReportBase
    {
        ReportInput SkuObj = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput DNObj = new ReportInput { Name = "DNNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null};
        ReportInput SNObj = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2017/02/01 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018/02/12 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };

        ReportTable reportTable = null;

        public SNByDNReport() {
            Inputs.Add(SkuObj);
            Inputs.Add(DNObj);
            Inputs.Add(SNObj);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
        }
        public override void Init()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string sqlcustomer = $@"select*From c_customer";
            DataTable dcc = SFCDB.RunSelect(sqlcustomer).Tables[0];
            if (dcc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
            {
                StartTime.Value = DateTime.Now.AddDays(-1);
                EndTime.Value = DateTime.Now;
                // base.Init();
                reportTable = new ReportTable();
                reportTable.ColNames.Add("SN");
                reportTable.ColNames.Add("WORKORDERNO");
                reportTable.ColNames.Add("SKUNO");
                reportTable.ColNames.Add("GROUPID");
                reportTable.ColNames.Add("DN_NO");
                reportTable.ColNames.Add("DN_CUSTOMER");
                reportTable.ColNames.Add("PO_NO");
                reportTable.ColNames.Add("SO_NO");
                reportTable.ColNames.Add("SHIPDATE");
                reportTable.ColNames.Add("CREATEBY");
                Outputs.Add(new ReportColumns(reportTable.ColNames));
                PaginationServer = true;
            }
            else
            {
                StartTime.Value = DateTime.Now.AddDays(-1);
                EndTime.Value = DateTime.Now;
                // base.Init();
                reportTable = new ReportTable();
                reportTable.ColNames.Add("SN");
                reportTable.ColNames.Add("WORKORDERNO");
                reportTable.ColNames.Add("SKUNO");
                reportTable.ColNames.Add("DN_NO");
                reportTable.ColNames.Add("DN_CUSTOMER");
                reportTable.ColNames.Add("PO_NO");
                reportTable.ColNames.Add("SO_NO");
                reportTable.ColNames.Add("SHIPDATE");
                reportTable.ColNames.Add("CREATEBY");
                Outputs.Add(new ReportColumns(reportTable.ColNames));
                PaginationServer = true;
            }

            DBPools["SFCDB"].Return(SFCDB);
        }
        public override void Run()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                GetRunSql();
                DataTable showTable = new DataTable();
                string sql = "";
                if (PaginationServer && PageSize != 0 && PageNumber != 0)
                {
                    sql = $@"select count(*) from ({Sqls["RunSql"]})";
                    string total_rows = SFCDB.RunSelect(sql).Tables[0].Rows[0][0].ToString();
                    reportTable.TotalRows = Convert.ToInt32(total_rows);
                    if (reportTable.TotalRows == 0)
                    {
                        throw new Exception("No Data!");
                    }
                    string sqlcustomer = $@"select*From c_customer";
                    DataTable dcc = SFCDB.RunSelect(sqlcustomer).Tables[0];
                    if (dcc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
                    {
                        sql = $@"select SN,WORKORDERNO,SKUNO,GROUPID,DN_NO,DN_CUSTOMER,PO_NO,SO_NO,SHIPDATE,CREATEBY
                                    from (select temp.*,rownum as rn from ({Sqls["RunSql"]} ) temp )
                                    where rn> ({PageNumber} - 1) * {PageSize} and rn<= {PageNumber} * {PageSize}";
                    }
                    else
                    {
                        sql = $@"select SN,WORKORDERNO,SKUNO,DN_NO,DN_CUSTOMER,PO_NO,SO_NO,SHIPDATE,CREATEBY
                                    from (select temp.*,rownum as rn from ({Sqls["RunSql"]} ) temp )
                                    where rn> ({PageNumber} - 1) * {PageSize} and rn<= {PageNumber} * {PageSize}";
                    }
                        
                    showTable = SFCDB.RunSelect(sql).Tables[0];
                    reportTable.PaginationServer = PaginationServer;
                }
                else
                {
                    showTable = SFCDB.RunSelect(Sqls["RunSql"]).Tables[0];
                    if (showTable.Rows.Count == 0)
                    {
                        throw new Exception("No Data!");
                    }
                }

                reportTable.LoadData(showTable, null);
                reportTable.Tittle = "DN Report";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public override void DownFile()
        {           
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                GetRunSql();
                DataTable dt = SFCDB.RunSelect(Sqls["RunSql"]).Tables[0];
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "DNReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                Outputs.Add(new ReportFile(fileName, content));
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        public string strsn(string sn)
        {

            string[] temp = sn.Split(',');
            sn = "";
            for (int i = 0; i < temp.Length; i++)
            {
                sn += "'" + temp[i].Replace("\r\n", "") + "',";

            }

            sn = sn.Substring(0, sn.Length - 1);

            return sn;

        }

        private void GetRunSql()
        {
            if (StartTime.Value == null || EndTime.Value == null)
            {
                throw new Exception("Please input start time and end time!");
            }
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string runSql;
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            string skuno = SkuObj.Value.ToString();
            string dnno = DNObj.Value.ToString();
            string sn = SNObj.Value.ToString();
            string sqlcustomer = $@"select*From c_customer";
            DataTable dcc = SFCDB.RunSelect(sqlcustomer).Tables[0];
            if (dcc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
            {
                runSql = $@"select a.sn,rsn.workorderno,a.skuno,rph.groupid,a.dn_no,c.dn_customer,b.po_no,b.so_no,a.SHIPDATE,a.CREATEBY 
                                from r_ship_detail a , r_dn_status b, r_to_detail c,r_sn rsn , r_pre_wo_head rph     where a.DN_NO = b.DN_NO and 
                                b.DN_NO=c.DN_NO and a.sn = rsn.sn(+) and rsn.VALID_FLAG=1  AND rsn.workorderno=rph.wo(+)
                                and a.SHIPDATE BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS')  ";
            }
            else
            {
                runSql = $@"select a.sn,rsn.workorderno,a.skuno,a.dn_no,c.dn_customer,b.po_no,b.so_no,a.SHIPDATE,a.CREATEBY 
                                from r_ship_detail a , r_dn_status b, r_to_detail c,r_sn rsn  where a.DN_NO = b.DN_NO and 
                                b.DN_NO=c.DN_NO and a.sn = rsn.sn(+) and rsn.VALID_FLAG=1 
                                and a.SHIPDATE BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS') ";
            }
                
            if (skuno != "")
            {
                runSql = runSql + $@" and a.skuno = '{skuno}' ";
            }
            if (dnno != "")
            {
                runSql = runSql + $@" and a.dn_no  in({strsn(dnno)})";
            }
            if (sn != "")
            {
                runSql = runSql + $@" and a.sn = '{sn}'";
            }
            runSql += $@" order by a.skuno,a.SHIPDATE ";
            RunSqls.Add(runSql);
            Sqls.Add("RunSql", runSql);
            DBPools["SFCDB"].Return(SFCDB);
        }
    }
}
