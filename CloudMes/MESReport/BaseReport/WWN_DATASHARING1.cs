using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MESReport.BaseReport
{
    class WWN_DATASHARING1:ReportBase
    {

        ReportInput MAC = new ReportInput { Name = "SearchByMac", InputType = "TextArea", Value = "8C3BAD6AFC58", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public WWN_DATASHARING1()
        {
            Inputs.Add(MAC);
        }
        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            //base.Run();

            //string SSN1 = Data["SSN1"].ToString().Trim();
            //SSN1 = $@"'{SSN1.Replace(",", "','")}'";
            string mac = MAC.Value.ToString().Trim();
            mac= $@"'{mac.Replace("\n", "',\n'")}'";
            string runSql = $@" select WSN,SKU,VSSN,VSKU,CSSN,CSKU,MAC,WWN,MAC_BLOCK_SIZE,LASTEDITBY,LASTEDITDT from WWN_DATASHARING where (mac in ({mac} ) or WSN in ({mac} ) or VSSN in ({mac} ) or CSSN in ({mac} ) )";
            if (mac == null)
            {
                ReportAlart alart = new ReportAlart("Please input mac or sn need query");
                Outputs.Add(alart);
                return;
            }

            //OleExec sfcdb = DBPools["SFCDB"].Borrow();
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            //OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dt = new DataTable();
            try
            {
                dt = sfcdb.RunSelect(runSql).Tables[0];
                //if (sfcdb != null)
                //{
                //    DBPools["SFCDB"].Return(sfcdb);
                //}
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                //ReportLink link;
                dt = sfcdb.RunSelect(runSql).Tables[0];
                DataSet res = sfcdb.RunSelect(runSql);
                ReportTable retTab = new ReportTable();
                DataTable dt1 = res.Tables[0].Copy();
                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["WSN"].ToString();
                    string linkURL1 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["VSSN"].ToString();
                    string linkURL2 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["CSSN"].ToString();
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                         if (dc.ColumnName.ToString().ToUpper() == "WSN")
                        {
                            linkRow[dc.ColumnName] = linkURL;
                            //linkRow[dc.ColumnName] = linkURL1;
                        }
                        else if (dc.ColumnName.ToString().ToUpper() == "VSSN")
                        {
                            linkRow[dc.ColumnName] = linkURL1;
                            //linkRow[dc.ColumnName] = linkURL1;
                        }
                        else if (dc.ColumnName.ToString().ToUpper() == "CSSN")
                        {
                            linkRow[dc.ColumnName] = linkURL2;
                            //linkRow[dc.ColumnName] = linkURL1;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, linkTable);
                reportTable.Tittle = "SN MAC detail";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
                return;
            }
            finally
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }
}
