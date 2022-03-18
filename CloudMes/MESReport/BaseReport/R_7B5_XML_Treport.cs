using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="R_7B5_XML_Treport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-1-27 </date>
    /// <summary>
    /// R_7B5_XML_Treport
    /// </summary>
    public class R_7B5_XML_Treport : ReportBase
    {
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput taskNo = new ReportInput() { Name = "TaskNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput item = new ReportInput() { Name = "Item", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput productLine = new ReportInput()
        {
            Name = "ProductLine",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = new string[]{ "ALL" }
        };

        public R_7B5_XML_Treport()
        {
            Inputs.Add(startTime);
            Inputs.Add(endTime);
            Inputs.Add(taskNo);
            Inputs.Add(item);
            Inputs.Add(productLine);
            //現在數據庫中沒有r_7b5_xml_t這個表,故直接DB Link 舊的數據庫
            string sqlGetProductLine = "select 'ALL' as product_line from dual union select distinct product_line from r_7b5_xml_t@hwd";
            Sqls.Add("SqlGetProductLine", sqlGetProductLine);
        }

        public override void Init()
        {
            startTime.Value = DateTime.Now.AddDays(-60);
            endTime.Value = DateTime.Now;
            productLine.ValueForUse = GetProductLine();
        }
        private string[] GetProductLine()
        {
            List<string> listProductLine = new List<string>();
            RunSqls.Add(Sqls["SqlGetProductLine"]);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet dsSkuno = SFCDB.RunSelect(Sqls["SqlGetProductLine"]);
                foreach (DataRow row in dsSkuno.Tables[0].Rows)
                {
                    listProductLine.Add(row[0].ToString());
                }
                return listProductLine.ToArray();
            }
            catch (Exception ex)
            {
                //DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
            finally
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public override void Run()
        {
            DateTime startDT = (DateTime)startTime.Value;
            DateTime endDT = (DateTime)endTime.Value;
            string dateFrom = $@"to_date('{startDT.ToString("yyyy-MM-dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
            string dateTO = $@"to_date('{endDT.ToString("yyyy-MM-dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";            
            string sqlTaskNo = "";
            string sqlHWPN = "";
            string sqlProductLine = "";
            string sqlRun = "";
            if (taskNo.Value != null && !string.IsNullOrEmpty(taskNo.Value.ToString()))
            {
                sqlTaskNo = $@" and TASK_NO ='{taskNo.Value.ToString()}' ";
            }
            if (item.Value != null && !string.IsNullOrEmpty(item.Value.ToString()))
            {
                sqlHWPN = $@" AND ITEM = '{item.Value.ToString()}' ";
            }
            if (productLine.Value.ToString() != "ALL" && !string.IsNullOrEmpty(productLine.Value.ToString()))
            {
                sqlProductLine = $@" and PRODUCT_LINE = '{productLine.Value.ToString()}'";
            }
            //現在數據庫中沒有r_7b5_xml_t這個表,故直接DB Link 舊的數據庫
            sqlRun = $@"select * from r_7b5_xml_t@hwd where 1=1 and lasteditdt between {dateFrom} and {dateTO} {sqlTaskNo} {sqlHWPN} {sqlProductLine} 
                        order by TASK_NO,lasteditdt";
            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet ds7B5 = SFCDB.RunSelect(sqlRun);
                if(SFCDB!=null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(ds7B5.Tables[0], null);
                reportTable.Tittle = "7B5Table";                
                Outputs.Add(reportTable);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
    }
}
