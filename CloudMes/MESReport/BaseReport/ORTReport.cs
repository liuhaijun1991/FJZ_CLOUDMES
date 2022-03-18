using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    class ORTReport: ReportBase
    {
       
        ReportInput woObj = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput snObj = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public ORTReport()
        {
           
            Inputs.Add(woObj);
            Inputs.Add(snObj);
            Inputs.Add(startTime);
            Inputs.Add(endTime);
        }
        public override void Init()
        {
            startTime.Value = DateTime.Now.AddDays(-1);
            endTime.Value = DateTime.Now;
            //base.Init();
        }
        public override void Run()
        {
            DateTime stime = Convert.ToDateTime(startTime.Value);
            DateTime etime = Convert.ToDateTime(endTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            string sn = snObj.Value.ToString();
            string wo = woObj.Value.ToString();
            string runSql = $@"select distinct c.skuno,  
                            a.workorderno,      
                            a.sn,
                            b.sample_station,
                            lot_qty          as ToltalQty,
                            b.sample_qty,
                            PASS_QTY,
                            case when EXISTS ( select*from  (select*From r_test_record aa where  aa.State='R' order by aa.starttime desc )a1 where  rownum=1 
                            and not exists(select*From r_test_record b where a.sn=b.sn and b.testation in('ORT','ORT-FT2') and b.state='PASS' AND b.starttime>a1.starttime ) and a1.sn=a.sn ) then 'RUNIN'
                            when EXISTS(select a2.sn from r_test_record a2 where a2.sn=a.sn and a2.testation in('ORT','ORT-FT2') and a2.state='PASS' 
                            and a2.endtime >(select max(CREATE_DATE) from r_lot_detail a3 where a3.SN=a.sn  ) ) then 'PASS' else 'WAITING' END STATUS,
                            decode(a.edit_emp,'Webservice','SYSTEM',a.edit_emp) as edit_emp,
                            a.edit_time
                            from r_lot_detail a, r_lot_status b,r_sn c
                            where  b.id = a.lot_id and a.sn=c.sn
                            and b.SAMPLE_STATION in('ORT','ORT-FT2')
                            and a.edit_time BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') 
                            and TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS') ";

            if (sn != "")
            {
                runSql = runSql + $@" and (a.sn='"+ sn + "')";
            }
            if (wo != "")
            {
                runSql = runSql + $@" and a.workorderno='"+ wo + "'";
            }
            runSql = runSql + $@"order by c.skuno,a.workorderno,a.sn,a.edit_time ";
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            DataTable dt = new DataTable();
            try
            {
                dt = sfcdb.RunSelect(runSql).Tables[0];
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "OrtLot Report";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);

            }
        }
    }
}
