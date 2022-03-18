using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class SilverRotationStatusReport : MESReport.ReportBase
    {
        //ReportInput inputSelect = new ReportInput { Name = "BY", InputType = "Select", Value = "SKUNO", Enable = true, SendChangeEvent = false, ValueForUse = new string[] {"SKUNO","PN" } };
        ReportInput inputPN = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SilverRotationStatusReport()
        {
            //this.Inputs.Add(inputSelect);
            this.Inputs.Add(inputPN);
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            //base.Run();
            string sqlPN = "";
            string sqlRun = "";
            //string selectBy = inputSelect.Value.ToString();
            string linkUrl = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.Juniper.SilverRotationStatusDetail&RunFlag=1";
            #region 2018年邏輯
            //if (inputPN.Value.ToString() != "" && selectBy == "PN")
            //{
            //    sqlPN = $@"and total.pn = '{inputPN.Value.ToString()}'";
            //}
            //if (inputPN.Value.ToString() != "" && selectBy == "SKUNO")
            //{
            //    sqlPN = $@" where skuno='{inputPN.Value.ToString()}'";
            //}

            //if (selectBy == "SKUNO")
            //{
            //    sqlRun = $@" select skuno,
            //                        value as PN,
            //                        total_qty,
            //                        In_Testing_Qty,
            //                        (total_qty - In_Testing_Qty) as Available_qty
            //                   From (select aa.skuno,
            //                                aa.value,
            //                                aa.total_qty,
            //                                decode(bb.In_Testing_Qty, null, 0, In_Testing_Qty) In_Testing_Qty
            //                           From (select a.skuno, a.value, count(b.csn) as total_qty
            //                                   From c_sku_detail a, R_SILVER_ROTATION b
            //                                  where a.value = b.pn
            //                                    and a.category = 'Rotation'
            //                                    and a.category_name = 'ControlKp'
            //                                    and b.status = 0
            //and b.valid_flag='1'
            //                                  group by a.skuno, a.value) aa,
            //                                (select a.skuno, a.value, count(c.csn) as In_Testing_Qty
            //                                   From c_sku_detail             a,
            //                                        R_SILVER_ROTATION        b,
            //                                        r_Silver_Rotation_Detail c
            //                                  where a.value = b.pn
            //                                    and b.csn = c.csn
            //                                    and a.category = 'Rotation'
            //                                    and a.category_name = 'ControlKp'
            //                                    and b.status = 0
            //                                    and b.seqno = c.seqno
            //and b.valid_flag='1'
            //and c.valid_flag='1'
            //                                    and c.endtime is null
            //                                  group by a.skuno, a.value) bb
            //                          where aa.value = bb.value(+) ) {sqlPN}";
            //}
            //else
            //{
            //    sqlRun = $@"select pn,
            //                   total_qty,
            //                   In_Testing_Qty,
            //                   (total_qty - In_Testing_Qty) as Available_qty
            //              from (select total.pn,
            //                           total.total_qty,
            //                           decode(testing.testing_qty, null, 0, testing.testing_qty) In_Testing_Qty
            //                      from (select pn, count(*) as total_qty
            //                              from r_silver_rotation
            //                             where status = '0'
            //                             group by pn) total,
            //                           (select rsr.pn, count(*) as Testing_qty
            //                              from r_silver_rotation_detail rsrd, r_silver_rotation rsr
            //                             where rsrd.csn = rsr.csn and rsrd.valid_flag='1' and rsr.valid_flag='1'
            //                               and rsrd.endtime is null
            //                             group by rsr.pn) testing
            //                     where 1 = 1 {sqlPN} and total.pn = testing.pn(+))";
            //}
            #endregion

            sqlPN = string.IsNullOrEmpty(inputPN.Value.ToString()) ? "" :$@" and skuno='{inputPN.Value.ToString()}'";
            sqlRun = $@" select skuno as PN,
                               total_qty,
                               In_Testing_Qty,
                               (total_qty - In_Testing_Qty) as Available_qty
                          From (select aa.skuno,
                                       aa.total_qty,
                                       decode(bb.In_Testing_Qty, null, 0, In_Testing_Qty) In_Testing_Qty
                                  From (select a.skuno, count(a.sn) as total_qty
                                          From R_SILVER_ROTATION a
                                         where a.valid_flag = '1'
                                           and a.status = '0'
                                         group by a.skuno) aa,
                                       (select b.skuno, count(b.sn) as In_Testing_Qty
                                          From R_SILVER_ROTATION b
                                         where b.valid_flag = '1'
                                           and b.status = '0'
                                           and b.rotation_flag = '1'
                                         group by b.skuno) bb
                                 where aa.skuno = bb.skuno(+)) where 1=1 {sqlPN}";

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = new DataTable();
                DataTable linkTable = new DataTable();
                dt = sfcdb.RunSelect(sqlRun).Tables[0];
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                //if (selectBy == "SKUNO")
                //{
                //    reportTable.LoadData(dt, null);
                //}
                //else
                //{
                    linkTable.Columns.Add("PN");
                    linkTable.Columns.Add("TOTAL_QTY");
                    linkTable.Columns.Add("IN_TESTING_QTY");
                    linkTable.Columns.Add("AVAILABLE_QTY");
                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow linkRow = linkTable.NewRow();
                        linkRow["PN"] = "";
                        //TYPE=0 表示要查TOTAL_QTY
                        linkRow["TOTAL_QTY"] = row["TOTAL_QTY"].ToString() == "0" ? "" : linkUrl + "&PN=" + row["PN"].ToString() + "&TYPE=0";
                        //TYPE=1 表示要查IN_TESTING_QTY
                        linkRow["IN_TESTING_QTY"] = row["IN_TESTING_QTY"].ToString() == "0" ? "" : linkUrl + "&PN=" + row["PN"].ToString() + "&TYPE=1";
                        //TYPE=2 表示要查AVAILABLE_QTY
                        linkRow["AVAILABLE_QTY"] = row["AVAILABLE_QTY"].ToString() == "0" ? "" : linkUrl + "&PN=" + row["PN"].ToString() + "&TYPE=2";
                        linkTable.Rows.Add(linkRow);
                    }
                    reportTable.LoadData(dt, linkTable);
                //}
                reportTable.Tittle = "Silver Rotation Status Report";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}
