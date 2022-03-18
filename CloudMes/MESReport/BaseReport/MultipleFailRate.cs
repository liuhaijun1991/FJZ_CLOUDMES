using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using MESDBHelper;
using MESReport.Common;
using MESDataObject;

namespace MESReport.BaseReport
{
    /// <summary>
    /// skuno目前只能獲取一個，等待前台添加標籤中
    /// </summary>
  public  class MultipleFailRate: ReportBase
    {
        ReportInput Year = new ReportInput() { Name = "Year", InputType = "TXT", Value = "2018", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Week = new ReportInput() { Name = "Week", InputType = "TXT", Value = "5", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public MultipleFailRate()
        {
            Inputs.Add(Year);
            Inputs.Add(Week);
            Inputs.Add(SkuNo);
        }
        public override void Init()
        {
            string strSql = @"select distinct(skuno) from c_sku ";
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable table = SFCDB.ExecuteDataTable(strSql, CommandType.Text);
                string[] SkunoAssy;
                if (table.Rows.Count > 0)
                {
                    SkunoAssy = new string[table.Rows.Count+1];
                    SkunoAssy[0] = "ALL";
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        SkunoAssy[i+1] = (table.Rows[i][0] == null) ? "" : table.Rows[i][0].ToString();
                    }
                }
                else
                {
                    // throw new Exception("獲取SkuNo失敗或者沒有數據");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816150051"));
                }
                SkuNo.ValueForUse = SkunoAssy;               
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }
        public override void Run()
        {
            string strYear = "";
            int IntWeek =0;
            string strSkuno = "";
            DateTime FromDate = DateTime.Now;
            string strFromDate = "";
            DateTime ToDate = DateTime.Now;
            string strToDate = "";
            if (Year.Value == null || Year.Value.ToString().Trim().Length <= 0)
            {
                strYear = DateTime.Now.Year.ToString();
                Year.Value = strYear;
            }
            else
            {
                strYear = Year.Value.ToString();
                try
                {
                    if (Convert.ToDateTime(strYear+"/03/09 00:00:00").Year.ToString()!=strYear)
                    {
                       // throw new Exception("請填寫正確的Year");

                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816151328"));
                    }
                }
                catch
                {
                    //throw new Exception("請填寫正確的Year");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816151328"));
                }
            }
            if (Week.Value == null || Week.Value.ToString().Trim().Length <= 0)
            {
               // throw new Exception("Week 不能為空");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816151527"));
            }
            else
            {
                try
                {
                    IntWeek =Convert.ToInt32(Week.Value.ToString().Trim());
                    if (IntWeek <= 0)
                    {
                        // throw new Exception("Week 必須是大於0數字");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816151725"));
                    }
                }
                catch
                {
                    // throw new Exception("Week 必須是數字");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816151844"));
                }
            }
            if (SkuNo.Value == null || SkuNo.Value.ToString().Trim().Length <= 0)
            {
                //throw new Exception("SkuNo不能為空");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816151954"));
            }
            else
            {
                strSkuno = SkuNo.Value.ToString().Trim();
                if (strSkuno.ToUpper() == "ALL")
                {
                    strSkuno = "select distinct(skuno) from c_sku";
                }
                else
                {
                    strSkuno="'"+ strSkuno+"'";
                }
            }
            FromDate = ConverDate.GetWeekStartDate(strYear,IntWeek);
            strFromDate = FromDate.Year.ToString() + "-" + FromDate.Month.ToString() + "-" + FromDate.Day.ToString() + " ";
            strFromDate = strFromDate + FromDate.Hour.ToString() + ":" + FromDate.Minute.ToString() + ":" + FromDate.Second.ToString();
            ToDate = FromDate.AddDays(6);
            strToDate = ToDate.Year.ToString() + "-" + ToDate.Month.ToString() + "-" + ToDate.Day.ToString() + " ";
            strToDate = strToDate + ToDate.Hour.ToString() + ":" + ToDate.Minute.ToString() + ":" + ToDate.Second.ToString();
            string runSql = $@"select skuno, datestr, sum(totalfail) as totalfail, sum(duplicatefail) as duplicatefail from(
                                        select b.skuno, TO_CHAR(A.CREATE_TIME - 7.5 / 24, 'YYYY-MM-DD') as datestr, count(SN) as totalfail,
                                        (case
                                            when(count(Sn)) = 1 then 0
                                            when(count(Sn)) > 1 then count(Sn) - 1 
                                            else 999
                                            end
                                        ) as duplicatefail
                                            from r_repair_main a, r_wo_base b
                                            where a.workorderno = b.workorderno and b.skuno in({strSkuno})
                                            and a.CREATE_TIME >= TO_DATE('{strFromDate}', 'YYYY-MM-DD HH24:MI:SS')
                                            and a.CREATE_TIME <= to_date('{strToDate}', 'YYYY-MM-DD HH24:MI:SS')
                                            group by b.skuno, TO_CHAR(A.CREATE_TIME - 7.5 / 24, 'YYYY-MM-DD'), sn
                                    )
                                group by skuno,datestr
                                order by skuno, datestr";          
            RunSqls.Add(runSql);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable res = SFCDB.ExecuteDataTable(runSql, CommandType.Text);
                ReportTable retTab = new ReportTable();
                retTab.LoadData(res, null);
                retTab.Tittle = "MultipleFailRate";
                retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }
    }
}
