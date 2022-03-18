using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class BonepileAgingDayReport : ReportBase
    {
        ReportInput inputDate = new ReportInput() { Name = "Date", InputType = "DateTime", Value = DateTime.Today.ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputOwner = new ReportInput() { Name = "Owner", InputType = "Select", Value = "RE", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "RE" } };
        public BonepileAgingDayReport()
        {
            Inputs.Add(inputDate);       
            Inputs.Add(inputOwner);
        }

        public override void Init()
        {
            base.Init();
        }
        public override void Run()
        {
            string date = inputDate.Value.ToString();
            string owner = inputOwner.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                
                if(this.LoginBU.ToUpper().Equals("VERTIV"))
                {
                    ForVertivRE(SFCDB,date, owner);
                }
                else
                {
                    throw new Exception("Login BU Error.");
                }
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

        private void ForVertivRE(OleExec SFCDB,string date,string owner)
        {
            string strDate = Convert.ToDateTime(date).ToString("yyyy/MM/dd");
            string sqlDate = $@"{strDate} 23:59:59";
            string sql = "";
            sql = $@"select a.*  from r_repair_transfer a,r_sn b where a.sn=b.sn and b.valid_flag='1' and a.closed_flag = 0 
                            AND A.CREATE_TIME <= TO_DATE('{sqlDate}','YYYY/MM/DD HH24:MI:SS') and a.in_time is not null";
            var transferList = SFCDB.ORM.Ado.SqlQuery<R_REPAIR_TRANSFER>(sql);
            if (transferList.Count == 0)
            {
                throw new Exception("No Data.");
            }
            NormalBonepileSNList bonepile = new NormalBonepileSNList();
            DateTime sysdate = SFCDB.ORM.GetDate();
            SFCDB.ORM.Deleteable<R_SN_LOG>().Where(r => r.LOGTYPE == "BonepileAgingDayReport" && r.DATA3 == strDate).ExecuteCommand();
            foreach (var item in transferList)
            {
                DataTable bonepileData = bonepile.GetData(SFCDB, "", sqlDate, "OPEN", "ALL", "ALL", "ALL", "ALL", "ALL", item.SN);
                if (bonepileData.Rows.Count > 0)
                {
                    if (bonepileData.Rows[0]["OWNER"].ToString().Equals(owner))
                    {
                        DateTime failDate = Convert.ToDateTime(bonepileData.Rows[0]["FAIL_DATE"].ToString());
                        TimeSpan ts = sysdate - failDate;
                        R_SN_LOG log = new R_SN_LOG();
                        log.ID = MesDbBase.GetNewID<R_SN_LOG>(SFCDB.ORM, this.LoginBU);
                        log.SNID = item.ID;
                        log.LOGTYPE = "BonepileAgingDayReport";
                        log.SN = item.SN;
                        log.FLAG = "1";
                        log.CREATEBY = "SYSTEM";
                        log.CREATETIME = sysdate;
                        log.DATA2 = ts.Days.ToString();
                        log.DATA3 = strDate;
                        if (ts.Days < 30)
                        {
                            log.DATA1 = "LESS_THEN_30";
                        }
                        else if (30 <= ts.Days && ts.Days <= 59)
                        {
                            log.DATA1 = "DATE_30_TO_59";
                        }
                        else if (60 <= ts.Days && ts.Days <= 89)
                        {
                            log.DATA1 = "DATE_60_TO_89";
                        }
                        else
                        {
                            log.DATA1 = "MORE_THEN_90";
                        }
                        SFCDB.ORM.Insertable<R_SN_LOG>(log).ExecuteCommand();
                    }
                }
            }

            List<R_SN_LOG> lessThen30 = SFCDB.ORM.Queryable<R_SN_LOG>().Where(r => r.LOGTYPE == "BonepileAgingDayReport" && r.DATA3 == strDate && r.DATA1 == "LESS_THEN_30").ToList();
            List<R_SN_LOG> d30to59 = SFCDB.ORM.Queryable<R_SN_LOG>().Where(r => r.LOGTYPE == "BonepileAgingDayReport" && r.DATA3 == strDate && r.DATA1 == "DATE_30_TO_59").ToList();
            List<R_SN_LOG> d60to89 = SFCDB.ORM.Queryable<R_SN_LOG>().Where(r => r.LOGTYPE == "BonepileAgingDayReport" && r.DATA3 == strDate && r.DATA1 == "DATE_60_TO_89").ToList();
            List<R_SN_LOG> moreThen90 = SFCDB.ORM.Queryable<R_SN_LOG>().Where(r => r.LOGTYPE == "BonepileAgingDayReport" && r.DATA3 == strDate && r.DATA1 == "MORE_THEN_90").ToList();
            string linkUrl = $@"Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.BonepileAgingDayDetail&RunFlag=1&Date={strDate}";
            List<string> columns = new List<string> { "All of Bonepile", "<30 Day", "30~59 Day", "60~89 Day", ">90 Day" };
            #region make table
            string md = Convert.ToDateTime(date).ToString("MM/dd");
            DataTable showTable = new DataTable();
            DataTable lineTable = new DataTable();
            showTable.Columns.Add("Days");
            showTable.Columns.Add(md);
            lineTable.Columns.Add("Days");
            lineTable.Columns.Add(md);

            DataRow sRow1 = showTable.NewRow();            
            sRow1[0] = columns[0];
            sRow1[1] = lessThen30.Count + d30to59.Count + d60to89.Count + moreThen90.Count;
            showTable.Rows.Add(sRow1);
            DataRow lRow1 = lineTable.NewRow();
            lRow1[0] = "";
            lRow1[1] = $@"{linkUrl}&Type=ALL";
            lineTable.Rows.Add(lRow1);

            DataRow sRow2 = showTable.NewRow();
            sRow2[0] = columns[1];
            sRow2[1] = lessThen30.Count;
            showTable.Rows.Add(sRow2);
            DataRow lRow2 = lineTable.NewRow();
            lRow2[0] = "";
            lRow2[1] = $@"{linkUrl}&Type=LESS_THEN_30";
            lineTable.Rows.Add(lRow2);

            DataRow sRow3 = showTable.NewRow();
            sRow3[0] = columns[2];
            sRow3[1] = d30to59.Count;
            showTable.Rows.Add(sRow3);
            DataRow lRow3 = lineTable.NewRow();
            lRow3[0] = "";
            lRow3[1] = $@"{linkUrl}&Type=DATE_30_TO_59";
            lineTable.Rows.Add(lRow3);

            DataRow sRow4 = showTable.NewRow();
            sRow4[0] = columns[3];
            sRow4[1] = d60to89.Count;
            showTable.Rows.Add(sRow4);
            DataRow lRow4 = lineTable.NewRow();
            lRow4[0] = "";
            lRow4[1] = $@"{linkUrl}&Type=DATE_60_TO_89";
            lineTable.Rows.Add(lRow4);

            DataRow sRow5 = showTable.NewRow();
            sRow5[0] = columns[4];
            sRow5[1] = moreThen90.Count;
            showTable.Rows.Add(sRow5);
            DataRow lRow5 = lineTable.NewRow();
            lRow5[0] = "";
            lRow5[1] = $@"{linkUrl}&Type=MORE_THEN_90";
            lineTable.Rows.Add(lRow5);
            #endregion

            ReportTable reportTable = new ReportTable();
            reportTable.LoadData(showTable, lineTable);
            reportTable.Tittle = "Bonepile Aging Day Report";
            Outputs.Add(reportTable);


            columnChart chartColumn = new columnChart();
            chartColumn.Tittle = "iPower SRE Bonepile Aging Days Tracking";
            //chartColumn.ChartTitle = strDate + " iPower SRE Bonepile Aging Days Tracking 2";

            //chartColumn.ChartSubTitle = strDate;
            XAxis xAxis = new XAxis();

            //xAxis.Title = "SKuno " + strDate;
            xAxis.Categories = columns;
            //_XAxis.XAxisType = XAxisType.datetime;
            chartColumn.XAxis = xAxis;
            chartColumn.Tooltip = "Pic";

            Yaxis yAxis = new Yaxis();
            //yAxis.Title = "Number of input";
            chartColumn.YAxis = yAxis;

            ChartData ChartData1 = new ChartData();           
            ChartData1.name = strDate + " iPower SRE Bonepile Aging Days Tracking";
            ChartData1.type = ChartType.column.ToString();
            ChartData1.colorByPoint = true;
            List<object> chartDataSourse = new List<object>();
            for (int i = 0; i < showTable.Rows.Count; i++)
            {
                columnData columnData = new columnData();               
                columnData.name = showTable.Rows[i]["Days"].ToString();                
                columnData.y = Convert.ToInt32(showTable.Rows[i][md]);
                chartDataSourse.Add(columnData);
            }
            ChartData1.data = chartDataSourse;
            List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
            chartColumn.ChartDatas = _ChartDatas;
            Outputs.Add(chartColumn);
        }
    }
}
