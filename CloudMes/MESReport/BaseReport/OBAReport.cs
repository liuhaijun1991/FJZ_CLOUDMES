using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;


namespace MESReport.BaseReport
{
    //OBA 報表
    public class OBAReport : ReportBase
    {
        #region
        ReportInput lotnoInput = new ReportInput()
        {
            Name = "LOTNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput skuInput = new ReportInput()
        {
            Name = "SKUNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput LotStatusInput = new ReportInput()
        {
            Name = "LOTSTATUS",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            // ValueForUse = new string[] { "ALL", "待入批次", "待抽检", "抽检完成" }
            ValueForUse = new string[] { "ALL", "BatchImporte", "WaitSamplInginspection", "SamplingInspectionCompleted" }
        };
        ReportInput statusInput = new ReportInput()
        {
            Name = "STATUS",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            // ValueForUse = new string[] { "ALL", "待抽检", "PASS", "FAIL" }
            ValueForUse = new string[] { "ALL", "WaitSamplInginspection", "PASS", "FAIL" }
        };
        ReportInput fromDate = new ReportInput()
        {
            Name = "From",
            InputType = "DateTime",
            //Value = "2018-02-01",
            Value = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput toDate = new ReportInput()
        {
            Name = "To",
            InputType = "DateTime",
            //Value = "2018-03-01",
            Value = DateTime.Today.ToString("yyyy-MM-dd"),
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        #endregion

        public OBAReport()
        {
            this.Inputs.Add(lotnoInput);
            this.Inputs.Add(skuInput);
            this.Inputs.Add(LotStatusInput);
            this.Inputs.Add(statusInput);
            this.Inputs.Add(fromDate);
            this.Inputs.Add(toDate);
        }

        public override void Init()
        {
            fromDate.Value = DateTime.Now.AddDays(-30);
            toDate.Value = DateTime.Now;

        }
        public override void Run()
        {
            {
                string lotno = lotnoInput.Value?.ToString();
                string skuno = skuInput.Value?.ToString();
                string lotstatus = LotStatusInput.Value?.ToString();
                string status = statusInput.Value?.ToString();
                string start = null;
                string end = null;
                if (fromDate.Value != null && fromDate.Value.ToString() != "")
                {
                    try
                    {
                        start = Convert.ToDateTime(fromDate.Value.ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    catch (Exception)
                    {
                        //throw new Exception("日期格式不正確！");

                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816154813"));
                    }

                }
                if (toDate.Value != null && toDate.Value.ToString() != "")
                {
                    try
                    {
                        end = Convert.ToDateTime(toDate.Value.ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    catch (Exception)
                    {
                        // throw new Exception("日期格式不正確！");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816154813"));
                    }

                }

                //string sql = $@" select lot_no,skuno,aql_type,lot_qty,reject_qty,sample_station,line,sample_qty,pass_qty,fail_qty,
                //decode(closed_flag, 0, '待关闭', 1, '待抽检',2,'抽检完成') as closed,decode(lot_status_flag,0,'待抽檢',1,'PASS',2,'FAIL') as lotstatus,
                //edit_time from r_lot_status where 1=1 ";
                string sql = $@" select lot_no,skuno,aql_type,lot_qty,reject_qty,sample_station,line,sample_qty,pass_qty,fail_qty,
                decode(closed_flag, 0, 'WaitClosed', 1, 'WaitSamplInginspection',2,'SamplingInspectionCompleted') as closed,decode(lot_status_flag,0,'WaitSamplInginspection',1,'PASS',2,'FAIL') as lotstatus,
                edit_time from r_lot_status where 1=1 ";

                StringBuilder condi = new StringBuilder(sql);
                if (!string.IsNullOrEmpty(lotno))
                {
                    condi.Append($@"and lot_no='{lotno.Replace("'", "''")}' ");
                }
                if (!string.IsNullOrEmpty(skuno))
                {
                    condi.Append($@"and skuno='{skuno.Replace("'", "''")}' ");
                }
                if (!string.IsNullOrEmpty(status) && !"ALL".Equals(status))
                {
                    string flag = "0";
                    switch (status.Trim())
                    {
                        //case "待抽检": flag = "0"; break;
                        //case "PASS": flag = "1"; break;
                        //case "FAIL": flag = "2"; break;
                        //default: break;
                        case "WaitSamplInginspection": flag = "0"; break;
                        case "PASS": flag = "1"; break;
                        case "FAIL": flag = "2"; break;
                        default: break;
                    }
                    condi.Append($@"and lot_status_flag='{flag}' ");
                }
                if (!string.IsNullOrEmpty(lotstatus) && !"ALL".Equals(lotstatus))
                {
                    string closesflag = "0";
                    switch (lotstatus.Trim())
                    {
                        //case "待入批次": closesflag = "0"; break;
                        //case "待抽检": closesflag = "1"; break;
                        //case "抽检完成": closesflag = "2"; break;
                        //default: break;
                        case "BatchImporte": closesflag = "0"; break;
                        case "WaitSamplInginspection": closesflag = "1"; break;
                        case "SamplingInspectionCompleted": closesflag = "2"; break;
                        default: break;
                    }
                    condi.Append($@"and closed_flag='{closesflag}' ");
                }
                if (!string.IsNullOrEmpty(start))
                {
                    condi.Append($@"and edit_time >= to_date('{start}', 'yyyy/mm/dd HH24:mi:ss') ");
                }
                if (!string.IsNullOrEmpty(end))
                {
                    condi.Append($@"and edit_time <= to_date('{end}','yyyy/mm/dd HH24:mi:ss') ");
                }
                //sql += condi.ToString();
                OleExec sfcdb = DBPools["SFCDB"].Borrow();
                DataTable dt = sfcdb.RunSelect(condi.ToString()).Tables[0];
                DataTable linkTable = new DataTable();
                DataRow linkRow = null;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                if (dt.Rows.Count > 0)
                {
                    linkTable.Columns.Add("LOT_NO");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("AQL_TYPE");
                    linkTable.Columns.Add("LOT_QTY");
                    linkTable.Columns.Add("REJECT_QTY");
                    linkTable.Columns.Add("SAMPLE_STATION");
                    linkTable.Columns.Add("LINE");
                    linkTable.Columns.Add("SAMPLE_QTY");
                    linkTable.Columns.Add("PASS_QTY");
                    linkTable.Columns.Add("FAIL_QTY");
                    linkTable.Columns.Add("CLOSED");
                    linkTable.Columns.Add("LOTSTATUS");
                    linkTable.Columns.Add("EDIT_TIME");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        linkRow = linkTable.NewRow();
                        //linkRow["LOT_NO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.LotNoDetailReport&RunFlag=1&LotNo=" + dt.Rows[i]["LOT_NO"].ToString();
                        linkRow["LOT_NO"] = "";
                        linkRow["SKUNO"] = "";
                        linkRow["AQL_TYPE"] = "";
                        linkRow["LOT_QTY"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.OBAByLotReport&RunFlag=1&LotNo=" + dt.Rows[i]["LOT_NO"].ToString();
                        linkRow["REJECT_QTY"] = "";
                        linkRow["SAMPLE_STATION"] = "";
                        linkRow["LINE"] = "";
                        linkRow["SAMPLE_QTY"] = "";
                        linkRow["PASS_QTY"] = "";
                        linkRow["FAIL_QTY"] = "";
                        linkRow["CLOSED"] = "";
                        linkRow["LOTSTATUS"] = "";
                        linkRow["EDIT_TIME"] = "";
                        linkTable.Rows.Add(linkRow);
                    }
                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt, linkTable);
                retTab.Tittle = "OBA REPORT";
                Outputs.Add(retTab);
            }

            //DateTime stime = Convert.ToDateTime(fromDate.Value);
            //DateTime etime = Convert.ToDateTime(toDate.Value);
            //string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            //string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            //OleExec SFCDB = DBPools["SFCDB"].Borrow();
            //try
            //{
            //    string sqlOba = $@"  SELECT* FROM R_LOT_STATUS WHERE EDIT_TIME BETWEEN TO_DATE('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
            //                      AND TO_DATE('{evalue}', 'YYYY/MM/DD HH24:MI:SS')";

            //    DataSet res = SFCDB.RunSelect(sqlOba);

            //    ReportTable retTab = new ReportTable();

            //    retTab.LoadData(res.Tables[0], null);
            //    retTab.Tittle = "OBA";
            //    retTab.ColNames.RemoveAt(0);//不顯示ID列 add by fgg 2018.03.09
            //    Outputs.Add(retTab);
            //    DBPools["SFCDB"].Return(SFCDB);
            //}
            //catch (Exception ee)
            //{
            //    DBPools["SFCDB"].Return(SFCDB);
            //    throw ee;
            //}
        }
    }
}
