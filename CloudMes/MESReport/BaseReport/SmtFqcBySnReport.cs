using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SmtFqcBySnReport : ReportBase
    {
        #region
        ReportInput snInput = new ReportInput()
        {
            Name = "SN",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        
        ReportInput woInput = new ReportInput()
        {
            Name = "WO",
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
            ValueForUse = new string[] { "ALL", "待抽檢", "PASS", "FAIL", "TOPASS", "TOFAIL" }
        };
        /*ReportInput reworkSelect = new ReportInput()
        {
            Name = "REWORK",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = new string[] { "ALL", "Rework", "UnRework" }
        };*/
        ReportInput fromDate = new ReportInput()
        {
            Name = "From",
            InputType = "DateTime",
            //Value = "2018-01-01",
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
        public SmtFqcBySnReport()
        {
            Inputs.Add(snInput);
            Inputs.Add(woInput);
            Inputs.Add(statusInput);
            //Inputs.Add(reworkSelect);
            Inputs.Add(fromDate);
            Inputs.Add(toDate);
        }

        public override void Run()
        {
            string sn = snInput.Value?.ToString();
            string wo = woInput.Value?.ToString();
            string lotstatus = statusInput.Value?.ToString();
            //string rework = reworkSelect.Value?.ToString();
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

            string baseSQL = $@"select ls.lot_no,ld.sn,ld.workorderno,
                decode(ld.sampling,'0','Wait_Sampling','1','PASS','2','FAIL','3','TOPASS','4','TOFAIL') as status,
                decode(ld.status, 1, 'LOCK', 0, '/') lockstatus,ld.fail_code,
                ld.fail_location,ld.description,ld.create_date from r_lot_detail ld,r_lot_status ls where 1=1  and ld.lot_id=ls.id ";

            StringBuilder condi = new StringBuilder(baseSQL);
            if (!string.IsNullOrEmpty(sn))
            {
                condi.Append($@"and ld.sn='{sn}' ");
            }
            if (!string.IsNullOrEmpty(wo))
            {
                condi.Append($@"and ld.workorderno='{wo}' ");
            }
            if (!string.IsNullOrEmpty(lotstatus) && !"ALL".Equals(lotstatus))
            {
                string flag = "0";
                switch (lotstatus.Trim())
                {
                    case "Wait_Sampling": flag = "0"; break;
                    case "PASS": flag = "1"; break;
                    case "FAIL": flag = "2"; break;
                    case "TOPASS": flag = "3"; break;
                    case "TOFAIL": flag = "4"; break;
                    default:
                        break;
                }
                condi.Append($@"and ld.sampling='{flag}' ");
            }
            //+日期篩選
            
            condi.Append($@"and ld.create_date >= to_date('{start}','yyyy/mm/dd hh24:mi:ss') ");
            
            condi.Append($@"and ld.create_date <= to_date('{end}','yyyy/mm/dd hh24:mi:ss') ");
            
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = sfcdb.RunSelect(condi.ToString()).Tables[0];
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt, null);
                retTab.Tittle = "SMTFQC BY SN REPORT";
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
