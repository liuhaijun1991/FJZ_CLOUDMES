using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESReport.HWD
{
    public class HWDBIPFailReport : ReportBase
    {
        ReportInput fromDate = new ReportInput()
        {
            Name = "From",
            InputType = "DateTime",
            //Value = "2018-02-01",
            Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput toDate = new ReportInput()
        {
            Name = "To",
            InputType = "DateTime",
            Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput Sku = new ReportInput()
        {
            Name = "SKUNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput Line = new ReportInput()
        {
            Name = "LINE",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        public HWDBIPFailReport()
        {
            this.Inputs.Add(fromDate);
            this.Inputs.Add(toDate);
            this.Inputs.Add(Sku);
            this.Inputs.Add(Line);
        }

        public override void Run()
        {
            var DB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string fd = ((DateTime)fromDate.Value).ToString("yyyyMMdd HHmmss");
                string td = ((DateTime)toDate.Value).ToString("yyyyMMdd HHmmss");
                string sku = Sku.Value.ToString();
                string line = Line.Value.ToString();
                string strsql = $@"select * from r_repair_main r where fail_station ='BIP'
and r.fail_time between to_date('{fd}','yyyymmdd hh24miss') and to_date('{td}','yyyymmdd hh24miss')";
                if (sku != null && sku != "")
                {
                    strsql += $@"and r.workorderno in (select w.workorderno from r_wo_base w where w.skuno = '{sku}' )";
                }
                if (line != null && line != "")
                {
                    strsql += $@"and r.fail_line = '{line}'";
                }
                var res = DB.RunSelect(strsql);

                ///-----------------------
                ReportTable retTab = new ReportTable();
                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&=ALL&SN=";
                    string linkWoURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&=ALL&WO=";
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "SN")
                        {
                            linkRow[dc.ColumnName] = linkURL+ row["SN"].ToString();
                        }
                        else if (dc.ColumnName.ToString().ToUpper() == "WORKORDERNO")
                        {
                            linkRow[dc.ColumnName] = linkWoURL + row["WORKORDERNO"].ToString();
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }

                ///------------------------------------

                retTab.LoadData(res.Tables[0], linkTable);
                retTab.Tittle = "BIPFAIL REPORT";
                Outputs.Add(retTab);
            }
            catch
            { }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }


        }
    }
}
