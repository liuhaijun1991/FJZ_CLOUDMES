using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESReport.BaseReport
{
    public class OBATestTEReport : ReportBase
    {
        ReportInput Style = new ReportInput()
        {
            Name = "STYLE",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            // ValueForUse = new string[] { "ALL", "待抽检", "PASS", "FAIL" }
            ValueForUse = new string[] { "ALL", "OBA_ONLINE" }
        };
        ReportInput skuInput = new ReportInput
        {
            Name = "SKUNO",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput statusInput = new ReportInput()
        {
            Name = "STATUS",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            // ValueForUse = new string[] { "ALL", "待抽检", "PASS", "FAIL" }
            ValueForUse = new string[] { "ALL", "PASS", "FAIL" }
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
        public OBATestTEReport()
        {
            this.Inputs.Add(Style);
            this.Inputs.Add(skuInput);
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
                string STYLE = Style.Value?.ToString();
                string skuno = skuInput.Value?.ToString();
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

                if (STYLE != "OBA_ONLINE")
                {
                    //string sql = $@" select lot_no,skuno,aql_type,lot_qty,reject_qty,sample_station,line,sample_qty,pass_qty,fail_qty,
                    //decode(closed_flag, 0, '待关闭', 1, '待抽检',2,'抽检完成') as closed,decode(lot_status_flag,0,'待抽檢',1,'PASS',2,'FAIL') as lotstatus,
                    //edit_time from r_lot_status where 1=1 ";
                    string sql = $@" SELECT a.CUSTOMER_NAME, a.SKUNO, a.sn,a.STATE,a.TESTATION,a.MESSTATION,a.DEVICE,a.STARTTIME,a.ENDTIME,a.DETAILTABLE,a.EDIT_EMP,a.EDIT_TIME FROM (SELECT cc.CUSTOMER_NAME, b.SKUNO, a.sn,a.STATE,a.TESTATION,a.MESSTATION,a.DEVICE,a.STARTTIME,a.ENDTIME,a.DETAILTABLE,a.EDIT_EMP,a.EDIT_TIME, ROW_NUMBER() over(partition by a.sn order by a.EDIT_TIME desc) as rownums  FROM SFCRUNTIME.R_TEST_RECORD a,r_sn B , C_SKU csk, SFCBASE.C_SERIES cs,SFCBASE.C_CUSTOMER cc WHERE b.SKUNO=csk.SKUNO AND csk.C_SERIES_ID= cs.ID AND cs.CUSTOMER_ID= cc.ID AND a.sn=b.sn AND a.SN IN (
                            SELECT SN FROM R_SN WHERE   VALID_FLAG=1 AND NEXT_STATION='OBA') AND a.TESTATION='OBA' AND a.MESSTATION='OBA' ";

                    StringBuilder condi = new StringBuilder(sql);

                    if (!string.IsNullOrEmpty(skuno))
                    {
                        condi.Append($@" and b.skuno='{skuno}' ");
                    }
                    if (!string.IsNullOrEmpty(status) && status != "ALL")
                    {
                        condi.Append($@" and a.STATE='{status}' ");
                    }
                    

                    if (!string.IsNullOrEmpty(start))
                    {
                        condi.Append($@" and a.edit_time >= to_date('{start}', 'yyyy/mm/dd HH24:mi:ss') ");
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        condi.Append($@" and a.edit_time <= to_date('{end}','yyyy/mm/dd HH24:mi:ss') ");
                    }
                    condi.Append(") a WHERE a.rownums=1");
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
                        linkTable.Columns.Add("CUSTOMER_NAME");
                        linkTable.Columns.Add("SKUNO");
                        linkTable.Columns.Add("sn");
                        linkTable.Columns.Add("STATE");
                        linkTable.Columns.Add("TESTATION");
                        linkTable.Columns.Add("MESSTATION");
                        linkTable.Columns.Add("DEVICE");
                        linkTable.Columns.Add("STARTTIME");
                        linkTable.Columns.Add("ENDTIME");
                        linkTable.Columns.Add("DETAILTABLE");
                        linkTable.Columns.Add("EDIT_EMP");
                        linkTable.Columns.Add("EDIT_TIME");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            linkRow = linkTable.NewRow();
                            //linkRow["LOT_NO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.LotNoDetailReport&RunFlag=1&LotNo=" + dt.Rows[i]["LOT_NO"].ToString();
                            linkRow["CUSTOMER_NAME"] = "";
                            linkRow["SKUNO"] = "";
                            linkRow["sn"] = "";
                            linkRow["STATE"] = "";
                            linkRow["TESTATION"] = "";
                            linkRow["MESSTATION"] = "";
                            linkRow["DEVICE"] = "";
                            linkRow["STARTTIME"] = "";
                            linkRow["ENDTIME"] = "";
                            linkRow["DETAILTABLE"] = "";
                            linkRow["EDIT_EMP"] = "";
                            linkRow["EDIT_TIME"] = "";

                            linkTable.Rows.Add(linkRow);
                        }
                    }

                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(dt, linkTable);
                    retTab.Tittle = "OBA REPORT";
                    Outputs.Add(retTab);
                }
                else
                {
                    string sql = $@"SELECT a.SN,b.WORKORDERNO,a.SKUNO,cc.CUSTOMER_NAME,b.CREATE_DATE,b.STATUS,b.EDIT_EMP,b.EDIT_TIME FROM SFCRUNTIME.R_LOT_DETAIL b, r_sn a ,C_SKU csk,C_SERIES cs,C_CUSTOMER cc  WHERE a.sn= b.sn and  b.sn IN (
                                        SELECT SN FROM r_sn where VALID_FLAG=1 ) AND a.SKUNO=CSK.SKUNO AND CSK.C_SERIES_ID=cs.ID AND cs.CUSTOMER_ID=cc.ID ";

                    StringBuilder condi = new StringBuilder(sql);
                    if (!string.IsNullOrEmpty(skuno))
                    {
                        condi.Append($@" and a.skuno='{skuno}' ");
                    }
                    if (!string.IsNullOrEmpty(status) && status != "ALL")
                    {
                        condi.Append($@" and b.STATUS = '{(status=="PASS"?"1":"0")}' ");
                    }

                    if (!string.IsNullOrEmpty(start))
                    {
                        condi.Append($@" and b.edit_time >= to_date('{start}', 'yyyy/mm/dd HH24:mi:ss') ");
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        condi.Append($@" and b.edit_time <= to_date('{end}','yyyy/mm/dd HH24:mi:ss') ");
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
                        linkTable.Columns.Add("SN");
                        linkTable.Columns.Add("WORKORDERNO");
                        linkTable.Columns.Add("SKUNO");
                        linkTable.Columns.Add("CUSTOMER_NAME");
                        linkTable.Columns.Add("CREATE_DATE");
                        linkTable.Columns.Add("STATUS");
                        linkTable.Columns.Add("EDIT_EMP");
                        linkTable.Columns.Add("EDIT_TIME");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            linkRow = linkTable.NewRow();
                            //linkRow["LOT_NO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.LotNoDetailReport&RunFlag=1&LotNo=" + dt.Rows[i]["LOT_NO"].ToString();
                            linkRow["SN"] = "";
                            linkRow["WORKORDERNO"] = "";
                            linkRow["SKUNO"] = "";
                            linkRow["CUSTOMER_NAME"] = "";
                            linkRow["CREATE_DATE"] = "";
                            linkRow["STATUS"] = "";
                            linkRow["EDIT_EMP"] = "";
                            linkRow["EDIT_TIME"] = "";
                            linkTable.Rows.Add(linkRow);
                        }
                    }

                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(dt, linkTable);
                    retTab.Tittle = "OBAONLINE REPORT";
                    Outputs.Add(retTab);
                }
            }
        }
    }
}
