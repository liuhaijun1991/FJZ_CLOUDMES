using MESDBHelper;
using System;
using System.Data;
using System.Text;

namespace MESReport.BaseReport
{
    public class SmtFqcByLotReport : ReportBase
    {
        #region
        ReportInput lotnoInput = new ReportInput()
        {
            Name = "LOTNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput skuInput = new ReportInput()
        {
            Name = "SKUNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput statusInput = new ReportInput()
        {
            Name = "STATUS",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = new string[] { "ALL", "待入批次", "待抽檢", "抽檢完成" }
        };
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
            //Value = "2018-03-01",
            Value = DateTime.Today.ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        #endregion

        public SmtFqcByLotReport()
        {
            this.Inputs.Add(lotnoInput);
            this.Inputs.Add(skuInput);
            this.Inputs.Add(statusInput);
            this.Inputs.Add(fromDate);
            this.Inputs.Add(toDate);
        }

        public override void Run()
        {
            string lotno = lotnoInput.Value?.ToString();
            string skuno = skuInput.Value?.ToString();
            string lotstatus = statusInput.Value?.ToString();
            //string start = fromDate.Value?.ToString();
            //string end = toDate.Value?.ToString();
            string start = null;
            string end = null;
            if (fromDate.Value != null && fromDate.Value.ToString() != "")
            {
                try
                {
                    start = Convert.ToDateTime(fromDate.Value.ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                }
                catch (Exception)
                {
                    throw new Exception("The date format is incorrect!");
                }
                
            }
            if (toDate.Value != null && toDate.Value.ToString() != "")
            {
                try
                {
                    end = Convert.ToDateTime(toDate.Value.ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                }
                catch (Exception)
                {
                    throw new Exception("The date format is incorrect!");
                }
                
            }

            string sql = $@"select lot_no,skuno,aql_type,lot_qty,reject_qty,sample_station,line,sample_qty,pass_qty,fail_qty,
                decode(closed_flag, 0, 'N', 1, 'Y') as closed,decode(lot_status_flag,0,'Wait_CheckInLot',1,'Wait_Sampling',2,'Completed') as lotstatus,
                edit_time from r_lot_status where 1=1 ";

            StringBuilder condi = new StringBuilder(sql);
            if (!string.IsNullOrEmpty(lotno))
            {
                condi.Append($@"and lot_no='{lotno.Replace("'","''")}' ");
            }
            if (!string.IsNullOrEmpty(skuno))
            {
                condi.Append($@"and skuno='{skuno.Replace("'","''")}' ");
            }
            if (!string.IsNullOrEmpty(lotstatus) && !"ALL".Equals(lotstatus))
            {
                string flag = "0";
                switch (lotstatus.Trim())
                {
                    case "Wait_CheckInLot": flag = "0"; break;
                    case "Wait_Sampling": flag = "1"; break;
                    case "Completed": flag = "2"; break;
                    default: break;
                }
                condi.Append($@"and lot_status_flag='{flag}' ");
            }
            if (!string.IsNullOrEmpty(start))
            {
                condi.Append($@"and edit_time >= to_date('{start}', 'yyyy/mm/dd hh:mi:ss') ");
            }
            if (!string.IsNullOrEmpty(end))
            {
                condi.Append($@"and edit_time <= to_date('{end}','yyyy/mm/dd hh:mi:ss') ");
            }
            //sql += condi.ToString();
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = sfcdb.RunSelect(condi.ToString()).Tables[0];
                DataTable linkTable = new DataTable();
                DataRow linkRow = null;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                if (dt.Rows.Count > 0)
                {
                    linkTable.Columns.Add("LOT_NO");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("AQL_TYPE");
                    linkTable.Columns.Add("LOT_QTY");
                    linkTable.Columns.Add("REJECT_QTY");
                    linkTable.Columns.Add("SAMPLE_STATION");
                    linkTable.Columns.Add("LINE");
                    linkTable.Columns.Add("SAMPLE_QTY");
                    linkTable.Columns.Add("PASS_QTY");
                    linkTable.Columns.Add("FAIL_QTY");
                    linkTable.Columns.Add("CLOSED");
                    linkTable.Columns.Add("LOTSTATUS");
                    linkTable.Columns.Add("EDIT_TIME");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        linkRow = linkTable.NewRow();
                        linkRow["LOT_NO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.LotNoDetailReport&RunFlag=1&LotNo=" + dt.Rows[i]["LOT_NO"].ToString();
                        linkRow["SKUNO"] = "";
                        linkRow["AQL_TYPE"] = "";
                        linkRow["LOT_QTY"] = "";
                        linkRow["REJECT_QTY"] = "";
                        linkRow["SAMPLE_STATION"] = "";
                        linkRow["LINE"] = "";
                        linkRow["SAMPLE_QTY"] = "";
                        linkRow["PASS_QTY"] = "";
                        linkRow["FAIL_QTY"] = "";
                        linkRow["CLOSED"] = "";
                        linkRow["LOTSTATUS"] = "";
                        linkRow["EDIT_TIME"] = "";
                        linkTable.Rows.Add(linkRow);
                    }
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt, linkTable);
                retTab.Tittle = "SMTFQC BY LOT REPORT";
                Outputs.Add(retTab);
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                if (sfcdb != null) DBPools["SFCDB"].Return(sfcdb);
            }

        }

    }
}
