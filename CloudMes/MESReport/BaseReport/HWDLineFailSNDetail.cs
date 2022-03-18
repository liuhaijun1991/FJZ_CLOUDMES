using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class HWDLineFailSNDetail : ReportBase
    {
        ReportInput inputLine = new ReportInput() { Name = "LINE", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "STATION", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuname = new ReportInput() { Name = "SKU_NAME", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public HWDLineFailSNDetail()
        {
            Inputs.Add(inputLine);
            Inputs.Add(inputStation);
            Inputs.Add(inputSkuname);
            Inputs.Add(inputSku);
            Inputs.Add(startTime);
            Inputs.Add(endTime);
        }

        public override void Init()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                startTime.Value = DateTime.Now.AddDays(-1);
                endTime.Value = DateTime.Now;
                List<string> lineList = new List<string>();
                lineList.Add("ALL");
                List<string> stationList = new List<string>();
                stationList.Add("ALL");
                string sql = "select distinct value from r_function_control where functionname='PanelFailStation' and controlflag = 'Y' and functiontype = 'NOSYSTEM'";
                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    stationList.Add(dr["VALUE"].ToString());
                }
                inputStation.ValueForUse = stationList;

                sql = "select distinct line_name from c_line";
                dt = SFCDB.RunSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    lineList.Add(dr["LINE_NAME"].ToString());
                }
                inputLine.ValueForUse = lineList;

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }


        }

        public override void Run()
        {
            DateTime startDT = (DateTime)startTime.Value;
            DateTime endDT = (DateTime)endTime.Value;
            string dateFrom = $@"to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
            string dateTO = $@"to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
            string line = inputLine.Value.ToString();
            string sku_name = inputSkuname.Value.ToString();
            string sku = inputSku.Value.ToString();
            string station = inputStation.Value.ToString();
          
            string sql_fail_line = "";           
            string sql_fail_sku = "";
            string sql_sku_name = "";        
            string sql_fail_station = "";

            if (!string.IsNullOrEmpty(line) && line.ToUpper() != "ALL")
            {              
                sql_fail_line = $@" and rm.fail_line='{line}'";
            }
            if (!string.IsNullOrEmpty(sku))
            {                
                sql_fail_sku = $@" and rm.skuno='{sku}'";
            }
            if (!string.IsNullOrEmpty(station) && station.ToUpper() != "ALL")
            {                
                sql_fail_station = $@" and rm.fail_station ='{station}'";
            }
            else
            {               
                sql_fail_station = $@" and rm.fail_station in (select distinct value from r_function_control where functionname='PanelFailStation' 
                         and controlflag='Y' and functiontype='NOSYSTEM')";
            }
            if (!string.IsNullOrEmpty(sku_name))
            {
                sql_sku_name = $@" and s.sku_name='{sku_name}'";
            }

            string sqlRun = $@"select s.sku_name,rm.skuno,rm.fail_line, rm.fail_station ,rm.sn,ra.description
                        from r_repair_main rm,r_repair_failcode ra,c_sku s 
                        where rm.id=ra.repair_main_id and rm.skuno=s.skuno {sql_fail_station} {sql_fail_line} {sql_fail_sku} {sql_sku_name}
                        and rm.fail_time between to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss')
                        and to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy-MM-dd hh24:mi:ss') ";

            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet dsLineFial = SFCDB.RunSelect(sqlRun);
                if (dsLineFial.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }                
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dsLineFial.Tables[0], null);
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                ReportAlart alart = new ReportAlart(exception.Message);
                Outputs.Add(alart);
            }
        }
    }
}
