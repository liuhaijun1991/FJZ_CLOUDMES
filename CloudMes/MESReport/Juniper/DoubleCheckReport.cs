using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class DoubleCheckReport : ReportBase
    {
        ReportInput inputSKU = new ReportInput() { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput() { Name = "Type", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL", "CheckSN", "CheckKeypart" } };

        ReportTable reportTable = null;
        string sql = "";
        public DoubleCheckReport()
        {
            Inputs.Add(inputSKU);
            Inputs.Add(inputWO);            
            Inputs.Add(inputSN);
            Inputs.Add(inputStation);
            Inputs.Add(inputType);
        }

        public override void Init()
        {
            base.Init();
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = new DataTable();
                sql = "select distinct station_name from c_station order by station_name";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> list_station = new List<string>();
                list_station.Add("ALL");
                for (int l = 0; l < dt.Rows.Count; l++)
                {
                    list_station.Add(dt.Rows[l][0].ToString());
                }
                inputStation.ValueForUse = list_station;

                reportTable = new ReportTable();
                reportTable.ColNames = new List<string> {"SKUNO", "WORKORDERNO", "SN","PARTNO","CHECK_VALUE", "CHECK_STATION", "CHECK_TYPE","CHECK_TIME","CHECK_EMP"};
                Outputs.Add(new ReportColumns(reportTable.ColNames));
                PaginationServer = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public override void Run()
        {
            base.Run();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = GetData(SFCDB);
                if (PaginationServer)
                {
                    reportTable.MakePagination(dt, null, PageNumber, PageSize);
                }
                else
                {
                    reportTable.LoadData(dt, null);
                }
                reportTable.Tittle = "Double Check Report";
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public override void DownFile()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = GetData(SFCDB);
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "DoubleCheck_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                Outputs.Add(new ReportFile(fileName, content));
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        private DataTable GetData(OleExec SFCDB)
        {
            DataTable dt = new DataTable();
            try
            {
                string sku = inputSKU.Value.ToString();
                string wo = inputWO.Value.ToString();
                string sn = inputSN.Value.ToString();
                string station = inputStation.Value.ToString();
                string type = inputType.Value.ToString();
                string sql = $@" select b.skuno,b.workorderno,a.sn,a.data1 as check_type,a.data2 as partno,a.data3 as check_value,a.data4 as check_station,a.createtime as check_time,a.createby check_emp
                                    from r_sn_log a,r_sn b WHERE a.sn=b.sn and a.logtype='JuniperDoubleCheck' and a.snid=b.id ";
                if (!string.IsNullOrEmpty(sku))
                {
                    sql = $@"{sql} and b.skuno='{sku}'";
                }
                if (!string.IsNullOrEmpty(wo))
                {
                    sql = $@"{sql} and b.workorderno='{wo}'";
                }
                if (!string.IsNullOrEmpty(sn))
                {
                    sql = $@"{sql} and b.sn='{sn}'";
                }
                if (station.ToUpper() != "ALL" && !string.IsNullOrEmpty(station))
                {
                    sql = $@"{sql} and a.data5='{station}'";
                }
                if (type.ToUpper() != "ALL" && !string.IsNullOrEmpty(type))
                {
                    sql = $@"{sql} and a.data1='{type}'";
                }
                dt = SFCDB.ORM.Ado.GetDataTable(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

    }
}
