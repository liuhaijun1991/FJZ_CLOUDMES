using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESReport.BaseReport
{
    public class RMABonepileSNList : ReportBase
    {
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = DateTime.Today.ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStatus = new ReportInput() { Name = "Status", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Open", "Close" } };
        ReportInput inputSeries = new ReportInput() { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSubSeries = new ReportInput() { Name = "SubSeries", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuno = new ReportInput() { Name = "Skuno", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputLotno = new ReportInput() { Name = "LOTNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        ReportTable reportTable = null;
        public RMABonepileSNList()
        {
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);
            Inputs.Add(inputStatus);
            Inputs.Add(inputSeries);
            Inputs.Add(inputSubSeries);
            Inputs.Add(inputSkuno);
            Inputs.Add(inputLotno);
            Inputs.Add(inputSN);
        }
        public override void Init()
        {
            string sql = "";
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = new DataTable();
                sql = "select distinct series_name from c_series";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> listSubSeries = new List<string>();
                listSubSeries.Add("ALL");
                for (int l = 0; l < dt.Rows.Count; l++)
                {
                    listSubSeries.Add(dt.Rows[l][0].ToString());
                }
                inputSubSeries.ValueForUse = listSubSeries;

                sql = "select distinct customer_name from C_CUSTOMER";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> listSeries = new List<string>();
                listSeries.Add("ALL");
                for (int l = 0; l < dt.Rows.Count; l++)
                {
                    listSeries.Add(dt.Rows[l][0].ToString());
                }
                inputSeries.ValueForUse = listSeries;

                sql = "select distinct skuno from c_sku where skuno is not null order by skuno";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> listProduct = new List<string>();
                listProduct.Add("ALL");
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    listProduct.Add(dt.Rows[n][0].ToString());
                }
                inputSkuno.ValueForUse = listProduct;
                reportTable = new ReportTable();
                reportTable.ColNames = new List<string> {
                "LOT_NO","SKUNO","SKU_NAME","SUB_SERIES","SERIES","SN","RECEIVED_DATE","LASTPACK_DATE","FAIL_STATION","FAILURE_SYMPTOM","FAILURE_TYPES","OWNER",
                "REMARK","VALUABLE","RMA_TIMES","FUNCTION_TIMES","COSMETIC_TIMES","UPLOAD_TIME","UPLOAD_EMP","SN_STATUS","CLOSED_DATE"};
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
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = GetData(SFCDB);
                if (PaginationServer)
                {
                    reportTable.MakePagination(dt, null, PageNumber, PageSize);
                }
                else
                {
                    reportTable.LoadData(dt, null);
                }
                reportTable.Tittle = "RMA Bonepile SN List Report";
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
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = GetData(SFCDB);
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "RMASN_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
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
            string dateFrom = inputDateFrom.Value.ToString();
            string dateTo = inputDateTo.Value.ToString();
            string status = inputStatus.Value.ToString();          
            string series = inputSeries.Value.ToString();
            string sub_series = inputSubSeries.Value.ToString();
            string skuno = inputSkuno.Value.ToString();
            string lotno = inputLotno.Value.ToString();
            string sn = inputSN.Value.ToString();
            string runSql = $@"select rma.LOT_NO,rma.SKUNO,sc.sku_name,sc.series_name as SUB_SERIES,sc.customer_name as SERIES,rma.SN,rma.RECEIVED_DATE,rma.LASTPACK_DATE,rma.FAIL_STATION,rma.FAILURE_SYMPTOM,rma.FAILURE_TYPES,
                            rma.OWNER,rma.REMARK,rma.VALUABLE,rma.RMA_TIMES,rma.FUNCTION_TIMES,rma.COSMETIC_TIMES,rma.UPLOAD_TIME,rma.UPLOAD_EMP,
                            case when rma.CLOSED_FLAG = '0' then 'Open'  when rma.CLOSED_FLAG = '1' then 'Close'  else 'Open' end as SN_STATUS,rma.CLOSED_DATE
                                 from r_rma_bonepile rma left join
                                  (select distinct s.skuno, s.sku_name, c.series_name,cc.customer_name from c_sku s 
                                    left join c_series c on s.c_series_id= c.id left join C_CUSTOMER cc on cc.id=c.customer_id) sc
                                   on rma.skuno = sc.skuno where 1=1";
            if (dateFrom.Length > 0)
            {
                if (Convert.ToInt64(Convert.ToDateTime(dateFrom).ToString("yyyyMMdd")) > Convert.ToInt64(Convert.ToDateTime(dateTo).ToString("yyyyMMdd")))
                {
                    throw new Exception("Date From不能大於Date To!");
                }
            }
            if (status.ToUpper() == "OPEN" && dateFrom.Length > 0)
            {
                throw new Exception("查詢Open狀態時Date From必須為空!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816165518")); 
            }
            if (status.ToUpper() != "OPEN" && dateFrom.Length == 0)
            {
                throw new Exception("查詢非Open狀態時Date From不能空!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816165925")); 
            }
            string toSql = $@" to_date('{dateTo}','yyyy/mm/dd hh24:mi:ss')+1";
           
            if (sub_series.ToUpper() != "ALL")
            {
                runSql = runSql + $@" and and sc.series_name='{sub_series}'";
            }
            if (series.ToUpper() != "ALL")
            {
                runSql = runSql + $@" and and sc.customer_name='{series}'";
            }
            if (skuno.ToUpper() != "ALL")
            {
                runSql = runSql + $@" and rma.skuno='{skuno}'";
            }
            if (lotno.Length > 0)
            {
                runSql = runSql + $@" and rma.lot_no = '{lotno}'";
            }
            if (lotno.Length > 0)
            {
                runSql = runSql + $@" and rma.lot_no = '{lotno}'";
            }
            if (sn.Length > 0)
            {
                runSql = runSql + $@" and rma.sn = '{sn}'";
            }
            else
            {
                switch (status.ToUpper())
                {
                    case "OPEN":
                        {
                            runSql = runSql + $@" and rma.UPLOAD_TIME<{toSql} and (rma.closed_date >= {toSql} or rma.closed_date is null or rma.closed_date='')";
                        }
                        break;
                    case "CLOSE":
                        {
                            runSql = runSql + $@" and  rma.UPLOAD_TIME<{toSql} and  rma.UPLOAD_TIME>= to_date('{dateFrom}','yyyy/mm/dd hh24:mi:ss')";
                        }
                        break;
                    case "ALL":
                        {
                            runSql = runSql + $@" and  rma.UPLOAD_TIME<{toSql} and  rma.UPLOAD_TIME>= to_date('{dateFrom}','yyyy/mm/dd hh24:mi:ss')";
                            string sql_temp = runSql;

                            runSql = runSql + $@" and  rma.UPLOAD_TIME<{toSql} and (rma.closed_date >= {toSql} or rma.closed_date is null or rma.closed_date='')"
                                + " union " + sql_temp + $@" and rma.UPLOAD_TIME<{toSql} and  rma.UPLOAD_TIME>= to_date('{dateFrom}','yyyy/mm/dd hh24:mi:ss')";
                        }
                        break;
                }
            }
            runSql = $@"select distinct LOT_NO,SKUNO,SKU_NAME,SUB_SERIES,SERIES,SN,RECEIVED_DATE,LASTPACK_DATE,FAIL_STATION,FAILURE_SYMPTOM,FAILURE_TYPES,
                            OWNER,REMARK,VALUABLE,RMA_TIMES,FUNCTION_TIMES,COSMETIC_TIMES,UPLOAD_TIME,UPLOAD_EMP,SN_STATUS,CLOSED_DATE from ({runSql})";
            dt = SFCDB.RunSelect(runSql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new Exception("No Data!");
            }
            return dt;
        }
    }
}
