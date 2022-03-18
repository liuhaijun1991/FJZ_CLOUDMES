using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.HWD
{
    public class R7B5PGIReport : ReportBase
    {
        ReportInput dateFrom = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput dateTo = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput taskNo = new ReportInput() { Name = "TaskNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput hh_item = new ReportInput() { Name = "HH_ITEM", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput lotNo = new ReportInput() { Name = "LOT_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput dn = new ReportInput() { Name = "DN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput so = new ReportInput() { Name = "SO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };


        public R7B5PGIReport()
        {            
            Inputs.Add(taskNo);
            Inputs.Add(hh_item);
            Inputs.Add(lotNo);
            Inputs.Add(dn);
            Inputs.Add(so);
            Inputs.Add(dateFrom);
            Inputs.Add(dateTo);
        }

        public override void Init()
        {
            base.Init();
            dateFrom.Value = DateTime.Now.ToString("yyyy-MM-dd");
            dateTo.Value = DateTime.Now.ToString("yyyy-MM-dd");
        }
        public override void Run()
        {
            base.Run();
            string sql = "SELECT * FROM R_7B5_PGI_DATA WHERE 1=1 ";
            string sqlTaskNo = "", sqlItem = "", sqlLotNo = "", sqlDN = "", sqlSO = "", sqlDate = "";
            if (taskNo.Value != null && !string.IsNullOrEmpty(taskNo.Value.ToString()))
            {
                sqlTaskNo = $@" and TASK_NO ='{taskNo.Value.ToString()}' ";
            }
            if (hh_item.Value != null && !string.IsNullOrEmpty(hh_item.Value.ToString()))
            {
                sqlItem = $@" and SKUNO ='{hh_item.Value.ToString()}' ";
            }
            if (lotNo.Value != null && !string.IsNullOrEmpty(lotNo.Value.ToString()))
            {
                sqlLotNo = $@" and LOT_NO ='{lotNo.Value.ToString()}' ";
            }
            if (dn.Value != null && !string.IsNullOrEmpty(dn.Value.ToString()))
            {
                sqlDN = $@" and DN ='{dn.Value.ToString()}' ";
            }
            if (so.Value != null && !string.IsNullOrEmpty(so.Value.ToString()))
            {
                sqlSO = $@" and SO ='{so.Value.ToString()}' ";
            }
            if (dateFrom.Value != null && !string.IsNullOrEmpty(dateFrom.Value.ToString()) && dateTo.Value != null && !string.IsNullOrEmpty(dateTo.Value.ToString()))
            {
                sqlDate = $@" AND POST_DATE  >= '{dateFrom}' AND POST_DATE<= '{dateTo}' ";
            }
            sql = sql + $@"{sqlTaskNo} {sqlItem} {sqlLotNo} {sqlDN} {sqlSO} {sqlDate}";
            RunSqls.Add(sql);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet ds7B5PGI = SFCDB.RunSelect(sql);
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                if (ds7B5PGI.Tables[0].Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(ds7B5PGI.Tables[0], null);
                reportTable.Tittle = "R7B5PGI";
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportAlart alart = new ReportAlart(exception.Message);
                Outputs.Add(alart);
            }
        }
    }
}
