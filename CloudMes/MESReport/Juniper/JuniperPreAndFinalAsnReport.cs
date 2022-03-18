using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;

namespace MESReport.Juniper
{
    public class JuniperPreAndFinalAsnReport : ReportBase
    {
        ReportInput inputPreAsnStartDate = new ReportInput() { Name = "PreAsnStartDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputPreAsnEndDate = new ReportInput() { Name = "PreAsnEndDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputFinalAsnStartDate = new ReportInput() { Name = "FinalAsnStartDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputFinalAsnEndDate = new ReportInput() { Name = "FinalAsnEndDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public JuniperPreAndFinalAsnReport()
        {
            Inputs.Add(inputPreAsnStartDate);
            Inputs.Add(inputPreAsnEndDate);
            Inputs.Add(inputFinalAsnStartDate);
            Inputs.Add(inputFinalAsnEndDate);
        }
        public override void Init()
        {
            base.Init();
            //inputPreAsnStartDate.Value = DateTime.Now.AddDays(-1);
            //inputPreAsnEndDate.Value = DateTime.Now;
            inputFinalAsnStartDate.Value = DateTime.Now.AddDays(-2);
            inputFinalAsnEndDate.Value = DateTime.Now.AddHours(1);
        }

        public override void Run()
        {
            //base.Run();
            DataTable dt = new DataTable();
            string runSql = "";
            DateTime preStartTime = DateTime.Now, preEndTime = DateTime.Now, finalStartTime = DateTime.Now, finalEndTime = DateTime.Now;

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                if (inputPreAsnStartDate.Value.ToString() != "" || inputPreAsnEndDate.Value.ToString() != "")
                {
                    if (inputPreAsnStartDate.Value.ToString() == "" || inputPreAsnEndDate.Value.ToString() == "")
                    {
                        throw new Exception("Please input PreAsn StartTime and EndTime");
                    }
                    preStartTime = (DateTime)inputPreAsnStartDate.Value;
                    preEndTime = (DateTime)inputPreAsnEndDate.Value;
                }
                if (inputFinalAsnStartDate.Value.ToString() != "" || inputFinalAsnEndDate.Value.ToString() != "")
                {
                    if (inputFinalAsnStartDate.Value.ToString() == "" || inputFinalAsnEndDate.Value.ToString() == "")
                    {
                        throw new Exception("Please input FinalAsn StartTime and EndTime");
                    }
                    finalStartTime = (DateTime)inputFinalAsnStartDate.Value;
                    finalEndTime = (DateTime)inputFinalAsnEndDate.Value;
                }

                //runSql = $@"select a.pono,
                //                   a.poline,
                //                   a.potype,
                //                   a.prewo,
                //                   a.qty,
                //                   a.pid,
                //                   b.groupid,
                //                   a.custpid,
                //                   a.preasn,
                //                   a.preasntime,
                //                   a.finalasn,
                //                   a.finalasntime
                //              from o_order_main a
                //              left join r_wo_groupid b
                //                on a.prewo = b.wo
                //             where a.preasn <> '0'";
                runSql = $@"select distinct a.pono,
                                            a.poline,
                                            a.potype,
                                            a.prewo,
                                            a.qty,
                                            a.pid,
                                            b.groupid,
                                            c.pn as custpid,
                                            a.preasn,
                                            a.preasntime,
                                            a.finalasn,
                                            a.finalasntime,
                                            c.podeliverydate as deliverydate,
                                            case
                                              when c.podeliverydate is null then
                                               'No Delivery Date'
                                              when a.finalasntime is null or a.finalasn = '0' then
                                               case
                                                 when trunc(c.podeliverydate) >= trunc(sysdate) then
                                                  'No FinalASN'
                                                 when trunc(c.podeliverydate) < trunc(sysdate) then
                                                  'False'
                                               end
                                              when trunc(c.podeliverydate) >= trunc(a.finalasntime) then
                                               'True'
                                              when trunc(c.podeliverydate) < trunc(a.finalasntime) then
                                               'False'
                                              else
                                               'Error'
                                            end as OnTimeDelivery,
                                            d.salesordernumber as so,
                                            c.salesorderlineitem as soline,
                                            e.freightinvoiceid as dn
                              from o_order_main a
                             inner join o_i137_item c
                                on a.itemid = c.id
                             inner join o_i137_head d
                                on c.tranid = d.tranid
                              left join r_wo_groupid b
                                on a.prewo = b.wo
                              left join r_i139 e
                                on a.finalasn = e.asnnumber
                             where a.preasn <> '0'";
                if (inputPreAsnStartDate.Value.ToString() != "")
                {
                    runSql = runSql + $@" and a.preasntime between to_date('{preStartTime.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss') and to_date('{preEndTime.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')";
                }
                if (inputFinalAsnStartDate.Value.ToString() != "")
                {
                    runSql = runSql + $@" and a.finalasntime between to_date('{finalStartTime.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss') and to_date('{finalEndTime.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')";
                }
                runSql = runSql + "  order by a.pono, a.poline, a.preasntime, a.finalasntime";

                dt = sfcdb.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "Juniper PreAsn And FinalAsn Detail";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

    }
}
