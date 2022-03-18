using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;

namespace MESReport.BaseReport
{
    class DCNORTReport:ReportBase
    {
        ReportInput woObj = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput snObj = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public DCNORTReport()
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
            string runSql = $@"select D.CUSTOMER_NAME,A.SKUNO,A.workorderno,A.SN,
                                CASE WHEN EXISTS(SELECT*fROM R_TEST_BRCD BB WHERE BB.SYSSERIALNO=A.SN AND BB.EVENTNAME ='POST-ORT' AND BB.STATUS='FAIL')
	                              AND NOT EXISTS(SELECT*fROM R_TEST_BRCD BB WHERE BB.SYSSERIALNO=A.SN AND BB.EVENTNAME ='POST-ORT' AND BB.STATUS='PASS') AND NOT EXISTS(SELECT * FROM r_sn_log r_log WHERE r_log.sn=a.SN AND r_log.LOGTYPE ='RE_TEST_ORT' AND r_log.DATA1='ORT' AND r_log.FLAG=1 AND r_log.CREATETIME > (SELECT max(bb.TATIME) FROM SFCRUNTIME.R_TEST_BRCD bb WHERE BB.SYSSERIALNO=A.SN AND BB.EVENTNAME ='POST-ORT' AND BB.STATUS='FAIL' )) THEN 'FAIL'
	                              WHEN  EXISTS(SELECT * FROM r_sn_log r_log WHERE r_log.sn=a.SN AND r_log.LOGTYPE ='RE_TEST_ORT' AND r_log.DATA1='ORT' AND r_log.FLAG=1) THEN 'RE_TEST'
	                              WHEN EXISTS(SELECT*fROM R_ORT BB WHERE BB.SN=A.SN AND BB.ORTEVENT='ORTOUT' ) THEN 'PASS'     
 WHEN EXISTS(SELECT*fROM R_TEST_BRCD BB WHERE BB.SYSSERIALNO=A.SN AND BB.EVENTNAME ='POST-ORT' AND BB.STATUS='PASS')  THEN 'TESTING'
		                            WHEN EXISTS(SELECT*fROM R_SN_PASS BB WHERE BB.SN=A.SN AND PASS_STATION='ORT' AND BB.STATUS='1') THEN 'BYPASS' ELSE '' END STATUS , 
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORTIN') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORTIN') ELSE '' end ORTIN,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT1') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT1') ELSE '' end ORT1,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT2') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT2') ELSE '' end ORT2,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT3') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT3') ELSE '' end ORT3,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT4') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT4') ELSE '' end ORT4,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT5') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT5') ELSE '' end ORT5,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT6') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT6') ELSE '' end ORT6,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT7') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT7') ELSE '' end ORT7,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT8') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT8') ELSE '' end ORT8,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT9') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT9') ELSE '' end ORT9,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT10') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORT10') ELSE '' end ORT10,
                                CASE WHEN EXISTS(SELECT*FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORTOUT') 
                                     then (SELECT to_char(AA.MDSTIME,'YYYY-MM-DD HH24:MI:SS') FROM R_ORT AA WHERE AA.SN=A.SN  AND AA.ORTEVENT='ORTOUT') ELSE '' end ORTOUT
                                From r_ort A,C_SKU B,C_SERIES C, C_CUSTOMER D,r_sn e where  A.SKUNO=B.SKUNO AND B.C_SERIES_ID=C.ID AND C.CUSTOMER_ID=D.ID  and A.ORTEVENT ='ORTIN'
                                and a.sn=e.sn and e.valid_flag='1'
                                and A.WORKTIME BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS')  
                                and TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS')";

            if (sn != "")
            {
                runSql = runSql + $@" and (a.sn='" + sn + "')";
            }
            if (wo != "")
            {
                runSql = runSql + $@" and a.workorderno='" + wo + "'";
            }
            runSql = runSql + $@"order by D.CUSTOMER_NAME,A.skuno,A.workorderno,A.sn,A.WORKTIME ";
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
                reportTable.Tittle = "Ort Report";
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
