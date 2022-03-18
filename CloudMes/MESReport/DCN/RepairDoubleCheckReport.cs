using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    public class RepairDoubleCheckReport : ReportBase
    {
        ReportInput inputSKU = new ReportInput() { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        ReportTable reportTable = null;       
        public RepairDoubleCheckReport()
        {
            Inputs.Add(inputSKU);
            Inputs.Add(inputWO);
            Inputs.Add(inputSN);            
        }

        public override void Init()
        {
            base.Init();
            OleExec SFCDB = null;
            try
            {
                reportTable = new ReportTable();
                reportTable.ColNames = new List<string> { "SKUNO", "WORKORDERNO", "SN", "PARTNO", "CHECK_VALUE", "KP_NAME", "SCANTYPE", "STATIOIN", "LOCATION", "CHECK_TIME", "CHECK_EMP" };
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
                string sql = $@" select b.skuno,b.workorderno,a.sn,a.data1 as check_type,a.data2 as partno,a.data3 as check_value,a.data4 as KP_NAME,a.data5 as SCANTYPE,
                                a.data6 as STATIOIN,a.data7 as LOCATION, a.createtime as check_time,a.createby check_emp
                                    from r_sn_log a,r_sn b WHERE a.sn=b.sn and a.logtype='RepairCheckOut' and a.snid=b.id ";
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
