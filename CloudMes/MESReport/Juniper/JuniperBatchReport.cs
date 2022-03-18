using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class JuniperBatchReport : ReportBase
    {
        ReportInput inputStartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSn = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWo = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSku = new ReportInput() { Name = "SKU", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };

        public JuniperBatchReport()
        {
            
            Inputs.Add(inputStartDate);
            Inputs.Add(inputEndDate);
            Inputs.Add(inputSn);
            Inputs.Add(inputWo);
            Inputs.Add(inputSku);
        }

        public override void Init()
        {
            inputStartDate.Value = DateTime.Now.AddDays(-30).ToString("yyyy/MM/dd");
            inputEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd");
        }

        public override void Run()
        {
            string SN = inputSn.Value.ToString();
            string wo = inputWo.Value.ToString();
            string skuno = inputSku.Value.ToString();
            DateTime sTime = Convert.ToDateTime(inputStartDate.Value);
            DateTime eTime = Convert.ToDateTime(inputEndDate.Value).AddDays(1);
            string sValue = sTime.ToString("yyyy-MM-dd");
            string eValue = eTime.ToString("yyyy-MM-dd");
            string sqlRun = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow linkRow = null;

            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            try
            {
                //string[] Sns = SN.Split(new char[] { ',' }); string[] Wos = wo.Split(new char[] { ',' }); string[] Skus = skuno.Split(new char[] { ',' });
                SN = SN.Replace(",", "','"); SN = SN.Replace(" ", "");
                wo = wo.Replace(",", "','"); wo = wo.Replace(" ", "");
                skuno = skuno.Replace(",", "','"); skuno = skuno.Replace(" ", "");

                sqlRun = $@"SELECT SN,WORKORDERNO,SKUNO,CASE PACKED_FLAG WHEN '1' THEN 'Y' ELSE 'N' END AS PACKED,
                            CASE COMPLETED_FLAG WHEN '1' THEN 'Y' ELSE 'N' END AS COMPLETED,
                            CASE SHIPPED_FLAG WHEN '1' THEN 'Y' ELSE 'N' END AS SHIPPED,
                            CASE REPAIR_FAILED_FLAG WHEN '1' THEN 'Y' ELSE 'N' END AS REPAIR_FAILED, CURRENT_STATION,NEXT_STATION
                            FROM R_SN WHERE START_TIME>TO_DATE('{sValue}','YYYY/MM/DD') AND START_TIME<TO_DATE('{eValue}','YYYY/MM/DD')                      
                            AND ((ASCII(SUBSTR(SN,1,1))>65 AND ASCII(SUBSTR(SN,1,1))<97) OR (ASCII(SUBSTR(SN,1,1))>47 AND ASCII(SUBSTR(SN,1,1))<58)) 
                            AND (SN NOT LIKE 'REVERSE%' OR WORKORDERNO NOT LIKE 'REVERSE%')   ";
                if (SN != "")
                {
                    sqlRun = sqlRun + $@" AND SN IN('{SN}')";
                }
                if (wo != "")
                {
                    sqlRun = sqlRun + $@" AND WORKORDERNO IN('{wo}')";
                }
                if (skuno != "")
                {
                    sqlRun = sqlRun + $@" AND SKUNO IN('{skuno}')";
                }
                sqlRun = sqlRun + $@" GROUP BY SN,WORKORDERNO,SKUNO,PACKED_FLAG,COMPLETED_FLAG,SHIPPED_FLAG,REPAIR_FAILED_FLAG, CURRENT_STATION,NEXT_STATION 
                                      ORDER BY SKUNO,WORKORDERNO,SN ";
                RunSqls.Add(sqlRun);
                snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("WORKORDERNO");
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("PACKED_FLAG");
                linkTable.Columns.Add("COMPLETED_FLAG");
                linkTable.Columns.Add("SHIPPED_FLAG");
                linkTable.Columns.Add("REPAIR_FAILED_FLAG");
                linkTable.Columns.Add("CURRENT_STATION");
                linkTable.Columns.Add("NEXT_STATION");
                for (int i = 0; i < snListTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SN"] = "";
                    linkRow["WORKORDERNO"] = "";
                    linkRow["SKUNO"] = "";
                    linkRow["PACKED_FLAG"] = "";
                    linkRow["COMPLETED_FLAG"] = "";
                    linkRow["SHIPPED_FLAG"] = "";
                    linkRow["REPAIR_FAILED_FLAG"] = "";
                    linkRow["CURRENT_STATION"] = "";
                    linkRow["NEXT_STATION"] = "";
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(snListTable, linkTable);
                reportTable.Tittle = "SNList";
                //reportTable.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);

            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
