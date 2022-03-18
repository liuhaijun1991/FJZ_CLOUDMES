using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.HWT
{
    public class AgingFinishReport:ReportBase
    {
        ReportInput inputSkuno = new ReportInput() { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCabinet = new ReportInput(){ Name = "CABINET", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false,ValueForUse = null};
        ReportInput inputShelf = new ReportInput() { Name = "SHELF", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputTool = new ReportInput() { Name = "TOOL", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputFromDate = new ReportInput()
        {
            Name = "FROMDATE",
            InputType = "DateTime",
            Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput inputToDate = new ReportInput()
        {
            Name = "TODATE",
            InputType = "DateTime",
            Value = DateTime.Today.ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        string sqlFinish = $@"select sn,workorderno,itemcode,starttime,endtime,startempno,realfinishtime,endempno,ipaddress,Floor,cabinetno,shelfno,tools_flag,
                                toolsno,slotno,lot_no,remark  from r_sn_aging_info where work_Flag='5' and sn not in ('EMPTY','ERROR')";
        public AgingFinishReport()
        {
            Inputs.Add(inputSkuno);
            Inputs.Add(inputCabinet);
            Inputs.Add(inputShelf);
            Inputs.Add(inputTool);
            Inputs.Add(inputWO);
            Inputs.Add(inputSN);
            Inputs.Add(inputFromDate);
            Inputs.Add(inputToDate);
            Sqls.Add("SqlGetFin", sqlFinish);
        }

        public override void Init()
        {
        }
        public override void Run()
        {
            string skuno = inputSkuno.Value?.ToString();
            string cabinet = inputSkuno.Value?.ToString();
            string shelf = inputShelf.Value?.ToString();
            string tool = inputTool.Value?.ToString();
            string wo = inputWO.Value?.ToString();
            string sn = inputSN.Value?.ToString();
            string fromData = inputFromDate.Value?.ToString().Replace("/", "-");
            string toData = inputToDate.Value?.ToString().Replace("/", "-");
            if (string.IsNullOrEmpty(fromData))
            {
                ReportAlart alart = new ReportAlart("請輸入開始時間!");
                Outputs.Add(alart);
                return;
            }
            if (string.IsNullOrEmpty(toData))
            {
                ReportAlart alart = new ReportAlart("請輸入結束時間!");
                Outputs.Add(alart);
                return;
            }
            fromData = fromData.Substring(0, fromData.Length - 3);
            toData = toData.Substring(0, toData.Length - 3);
            sqlFinish = sqlFinish + $@" and realfinishtime BETWEEN TRUNC (TO_DATE ('{fromData}','YYYY/MM/DD HH24:MI:SS')) AND TRUNC (TO_DATE ('{toData}','YYYY/MM/DD HH24:MI:SS')+1) ";
            if (!string.IsNullOrEmpty(skuno))
            {
                sqlFinish = sqlFinish + $@" AND itemcode like '%{skuno}%'";
            }
            if (!string.IsNullOrEmpty(cabinet))
            {
                sqlFinish = sqlFinish + $@" AND cabinetno like '%{cabinet}%'";
            }
            if (!string.IsNullOrEmpty(shelf))
            {
                sqlFinish = sqlFinish + $@" AND shelfno like '%{shelf}%'";
            }
            if (!string.IsNullOrEmpty(tool))
            {
                sqlFinish = sqlFinish + $@" AND toolsno like '%{tool}%'";
            }
            if (!string.IsNullOrEmpty(wo))
            {
                sqlFinish = sqlFinish + $@" AND workorderno like '%{wo}%'";
            }
            if (!string.IsNullOrEmpty(sn))
            {
                sqlFinish = sqlFinish + $@" AND sn like '%{sn}%'";
            }
            sqlFinish = sqlFinish + " order by realfinishtime ";

            RunSqls.Add(sqlFinish);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = SFCDB.ExecuteDataTable(sqlFinish, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt, null);
                retTab.Tittle = "Aging Finish Report";                
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);                
            }
        }
    }
}
