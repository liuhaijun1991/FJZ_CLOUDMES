using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class StationWipReport:ReportBase
    {
        ReportInput SkunoInput = new ReportInput()
        {
            Name = "SKUNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput StationInput = new ReportInput()
        {
            Name = "STATION",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput StartDateInput = new ReportInput()
        {
            Name = "START_DATE",
            InputType = "DateTime",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput EndDateInput = new ReportInput()
        {
            Name = "END_DATE",
            InputType = "DateTime",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        public StationWipReport()
        {
            Inputs.Add(SkunoInput);
            Inputs.Add(StationInput);
            Inputs.Add(StartDateInput);
            Inputs.Add(EndDateInput);
        }
        public override void Init()
        {
            //base.Init();
            StartDateInput.Value = DateTime.Now.AddDays(-1);
            EndDateInput.Value = DateTime.Now;
        }

        public override void Run()
        {
            string skuno = SkunoInput.Value.ToString();
            string station = StationInput.Value.ToString();
            string startDate = StartDateInput.Value.ToString();
            string endDate = EndDateInput.Value.ToString();
            OleExec sfcdb = null;
            string sqlSkuno = "";
            string sqlDate = "";
            string sqlRun = "";
            if (startDate == "" || startDate == "")
            {
                ReportAlart alart = new ReportAlart("Please input start or end date");
                Outputs.Add(alart);
                return;
            }
            if (station == "")
            {
                ReportAlart alart = new ReportAlart("Please input station");
                Outputs.Add(alart);
                return;
            }
            try
            {
                DateTime dtStart = DateTime.ParseExact(startDate, "yyyy/M/d HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime dtEnd = DateTime.ParseExact(endDate, "yyyy/M/d HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);                
                if (DateTime.Compare(dtStart, dtEnd) > 0)
                {
                    throw new Exception("Start date cannot be greater than end date");
                }                
                if (skuno != "")
                {
                    sqlSkuno = $@" and rsn.skuno='{skuno}' ";
                }
                sqlDate = $@" and rsn.edit_time between  to_date('{dtStart}','yyyy/mm/dd hh24:mi:ss') and  to_date('{dtEnd}','yyyy/mm/dd hh24:mi:ss') ";  
                switch (station)
                {
                    case "OBA":
                        sqlRun = $@" select sntable.skuno,sntable.workorderno,packtable.pack_no,sntable.next_station,count(*) qty,packtable.edit_time from 
                                    (select rpp.pack_no, rsnp.sn_id, rpp.edit_time from r_packing rpc, r_sn_packing rsnp, r_packing rpp
                                    where rpc.id = rsnp.pack_id and rpp.id = rpc.parent_pack_id  and rpp.closed_flag='1' ) packtable,
                                    (select rsn.id, rsn.skuno,rwo.workorderno,rsn.sn,rsn.next_station from r_sn rsn, r_wo_base rwo where rsn.next_station = '{station}'
                                    and rsn.valid_flag = '1' and substr(rsn.sn,0,1) not in ('*', '#', '~')
                                    and rsn.workorderno = rwo.workorderno and rwo.closed_flag = '0' {sqlSkuno} {sqlDate}) sntable where packtable.sn_id = sntable.id
                                    group by sntable.skuno,sntable.workorderno,packtable.pack_no,sntable.next_station,packtable.edit_time order by sntable.workorderno";
                        break;
                    default:
                        sqlRun = $@"select rsn.skuno,rwo.workorderno,rsn.sn,rsn.next_station,rsn.edit_time from r_sn rsn,r_wo_base rwo where rsn.next_station='{station}'
                                    and rsn.valid_flag = '1' and substr(rsn.sn,0,1) not in ('*', '#', '~') 
                                    and rsn.workorderno = rwo.workorderno and rwo.closed_flag = '0' {sqlSkuno} {sqlDate} ";
                        break;
                }
                sfcdb = DBPools["SFCDB"].Borrow();
                DataTable dt = sfcdb.RunSelect(sqlRun).Tables[0];
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
                reportTable.Tittle = "Station WIP Report";
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
