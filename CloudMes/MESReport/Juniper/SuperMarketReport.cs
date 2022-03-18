
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class SuperMarketReport : MESReport.ReportBase
    {        
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SKU = new ReportInput { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput STATUS = new ReportInput { Name = "STATUS", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "OPEN", "CLOSED" } };
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2022-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2022-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SuperMarketReport()
        {            
            Inputs.Add(SN);
            Inputs.Add(WO);
            Inputs.Add(SKU);
            Inputs.Add(STATUS);
            Inputs.Add(startTime);
            Inputs.Add(endTime);
        }
        public override void Init()
        {
            startTime.Value = DateTime.Now.AddDays(-30).ToString("yyyy/MM/dd HH:mm:ss");
            endTime.Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        public override void Run()
        {
            DateTime stime = Convert.ToDateTime(startTime.Value);
            DateTime etime = Convert.ToDateTime(endTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");

            var strSql = $@"
                    SELECT nvl(b.sn, a.r_sn_id) sn,
                           nvl(b.SKUNO, a.in_r_sap_mov_id) skuno,
                           nvl2(b.sn, 'MakePart', 'BuyPart') Type,
                           b.WORKORDERNO,
                           b.CURRENT_STATION,
                           b.NEXT_STATION,
                           a.IN_TIME,
                           a.IN_BY,
                           a.OUT_TIME,
                           a.OUT_BY,
                           a.out_r_sap_mov_id location,
                           decode(a.status, 0, 'CLOSED', 1, 'OPEN') status
                      FROM R_SUPERMARKET a
                      left join r_sn b
                        on a.R_SN_ID = b.id
                       and b.valid_flag = 1
                     where a.IN_TIME between
                           to_date('{svalue}', 'yyyy/mm/dd hh24:mi:ss') and
                           to_date('{evalue}', 'yyyy/mm/dd hh24:mi:ss')";
            
            if (!string.IsNullOrEmpty(SKU.Value.ToString()))
            {
                strSql += $@" and (b.SKUNO='{SKU.Value.ToString().Trim().ToUpper()}' or a.in_r_sap_mov_id='{SKU.Value.ToString().Trim().ToUpper()}') ";
            }
            if (!string.IsNullOrEmpty(SN.Value.ToString()))
            {
                strSql += $@" and (b.sn='{SN.Value.ToString().Trim().ToUpper()}' or a.r_sn_id='{SN.Value.ToString().Trim().ToUpper()}') ";
            }
            if (!string.IsNullOrEmpty(WO.Value.ToString()))
            {
                strSql += $@" and b.WORKORDERNO= '{WO.Value.ToString().Trim().ToUpper()}' ";
            }
            if (!STATUS.Value.ToString().Equals("ALL"))
            {
                strSql += STATUS.Value.ToString() == "OPEN" ? "and a.status = 1" : $@" and a.status = 0 ";
            }

            strSql += " order by nvl(b.sn, a.r_sn_id), a.IN_TIME desc ";

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = sfcdb.RunSelect(strSql).Tables[0];
                ReportTable retTab = new ReportTable();
                DataTable linkTable = new DataTable();
                retTab.LoadData(dt,null);
                retTab.Tittle = "SuperMarKet Report";
                Outputs.Add(retTab);
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
}
