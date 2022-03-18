using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="HWDRepartReport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-1-27 </date>
    /// <summary>
    /// HWDRepartReport
    /// </summary>
    public class HWDRepairReport : ReportBase
    {       
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        // ReportInput type = new ReportInput() { Name = "Type", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "外觀", "功能" } };
        ReportInput type = new ReportInput() { Name = "Type", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Appearance", "Function" } };
        //ReportInput type = new ReportInput() { Name = "Type", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Exterior", "Funtion" } };
        ReportInput skuno = new ReportInput() { Name = "Skuno", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput line = new ReportInput() { Name = "Line", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] 
        { "ALL", "B32S1", "B32S2", "B32S3", "B32S4", "B32S5", "B32S6", "B32S7", "B32S8", "B32S9", "B32SA", "B32SB" } };
        ReportInput wo = new ReportInput() { Name = "WO", InputType = "TXT", Value = "002510026153", Enable = true, SendChangeEvent = false, ValueForUse = null };        

        public HWDRepairReport()
        {
            Inputs.Add(startTime);
            Inputs.Add(endTime);
            Inputs.Add(type);
            Inputs.Add(skuno);
            Inputs.Add(line);
            Inputs.Add(wo);
            string sqlGetSkuno = "select 'ALL' as skuno from dual union select skuno from c_sku order by skuno";
            Sqls.Add("SqlGetSkuno", sqlGetSkuno);
        }

        public override void Init()
        {
            startTime.Value = DateTime.Now.AddDays(-1);
            endTime.Value = DateTime.Now;
            skuno.ValueForUse = GetSkunoArray();
        }
        private string[] GetSkunoArray() {            
            List<string> listSkuno = new List<string>();
            RunSqls.Add(Sqls["SqlGetSkuno"]);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet dsSkuno = SFCDB.RunSelect(Sqls["SqlGetSkuno"]);
                foreach (DataRow row in dsSkuno.Tables[0].Rows)
                {
                    listSkuno.Add(row[0].ToString());
                }
                return listSkuno.ToArray();
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
            string sqlFailCode = "";
            string sqlSkuno = "";
            string sqlLine = "";
            string sqlWO = "";
            string sqlRun = "";
            if (type.Value.ToString() != "ALL" && !string.IsNullOrEmpty(type.Value.ToString()))
            {
                
               // if (type.Value.ToString() == "功能
               if (type.Value.ToString() == "Function")
                {
                    sqlFailCode = " and j.fail_code not like 'SMT%' ";
                }
                else
                {
                    sqlFailCode = " and j.fail_code like 'SMT%' ";
                }
            }
            if (skuno.Value.ToString() != "ALL" && !string.IsNullOrEmpty(skuno.Value.ToString()))
            {
                sqlSkuno = $@" and b.skuno = '{skuno.Value.ToString()}'";
            }
            if (line.Value.ToString() != "ALL" && !string.IsNullOrEmpty(line.Value.ToString()))
            {
                sqlLine = $@" and c.line = '{line.Value.ToString()}'";
            }
            if (wo.Value != null && !string.IsNullOrEmpty(wo.Value.ToString()))
            {
                sqlWO = $@" and b.workorderno = '{wo.Value.ToString()}'";
            }

            //sqlRun = $@"select workorderno 工單,skuno 料號,line 線別,workorder_qty 工單數量,failcount 不良數量,
            //           decode(failcount/workorder_qty,0,'0',to_char(round(failcount/workorder_qty * 100, 2),'fm9999990.9999')) || '%' as 不良率,
            //           fail_location 不良位置,chinese_description 不良原因,count(sn) 數量,fail_code 不良代碼
            //            from (select b.workorderno,d.workorder_qty,i.skuno,g.failcount,
            //                c.line,a.sn,a.create_time,a.edit_time,a.fail_code,h.chinese_description,
            //                a.fail_location from r_repair_failcode a
            //                    inner join r_sn b
            //                        on a.sn = b.sn
            //                    left join r_sn_station_detail c
            //                        on a.sn = c.sn
            //                    inner join r_wo_base d
            //                        on b.workorderno = d.workorderno
            //            inner join (select f.workorderno, count(1) failcount
            //                                from r_repair_failcode e
            //                                inner join r_sn f
            //                                    on e.sn = f.sn
            //                                where e.edit_time between {dateFrom} and {dateTO}
            //                                --and e.fail_category = 'DEFECT'
            //                                group by f.workorderno) g
            //            on g.workorderno = b.workorderno 
            //            inner join c_error_code h
            //                on a.fail_code = h.error_code
            //            inner join r_wo_base i
            //                on b.workorderno = i.workorderno
            //            inner join r_repair_failcode j
            //                on j.sn = a.sn and j.create_time = a.create_time                           
            //            where --a.fail_category = 'DEFECT'
            //                    --and j.fail_category = 'SYMPTOM'
            //                    --and c.current_station = 'AOI1'
            //                    --and
            //                    a.edit_time between  {dateFrom} and {dateTO} {sqlFailCode} {sqlSkuno} {sqlLine} {sqlWO}
            //                    order by b.workorderno,c.line,a.fail_location,a.fail_code)
            //            group by workorderno,workorder_qty,failcount,line,fail_location,fail_code,
            //            chinese_description,skuno order by line, workorderno, count(1) desc";


            sqlRun = $@"select workorderno ,skuno ,line ,workorder_qty ,failcount ,
                       decode(failcount/workorder_qty,0,'0',to_char(round(failcount/workorder_qty * 100, 2),'fm9999990.9999')) || '%' as DefectiveRate,
                       fail_location ,chinese_description ,count(sn) Quantity,fail_code 
                        from (select b.workorderno,d.workorder_qty,i.skuno,g.failcount,
                            c.line,a.sn,a.create_time,a.edit_time,a.fail_code,h.chinese_description,
                            a.fail_location from r_repair_failcode a
                                inner join r_sn b
                                    on a.sn = b.sn
                                left join r_sn_station_detail c
                                    on a.sn = c.sn
                                inner join r_wo_base d
                                    on b.workorderno = d.workorderno
                        inner join (select f.workorderno, count(1) failcount
                                            from r_repair_failcode e
                                            inner join r_sn f
                                                on e.sn = f.sn
                                            where e.edit_time between {dateFrom} and {dateTO}
                                            --and e.fail_category = 'DEFECT'
                                            group by f.workorderno) g
                        on g.workorderno = b.workorderno 
                        inner join c_error_code h
                            on a.fail_code = h.error_code
                        inner join r_wo_base i
                            on b.workorderno = i.workorderno
                        inner join r_repair_failcode j
                            on j.sn = a.sn and j.create_time = a.create_time                           
                        where --a.fail_category = 'DEFECT'
                                --and j.fail_category = 'SYMPTOM'
                                --and c.current_station = 'AOI1'
                                --and
                                a.edit_time between  {dateFrom} and {dateTO} {sqlFailCode} {sqlSkuno} {sqlLine} {sqlWO}
                                order by b.workorderno,c.line,a.fail_location,a.fail_code)
                        group by workorderno,workorder_qty,failcount,line,fail_location,fail_code,
                        chinese_description,skuno order by line, workorderno, count(1) desc";

            #region 原報表查詢語句
            //select  workorderno 工單,
            //       skuno 料號,
            //       productionline 線別,
            //       workorderqty 工單數量,
            //       failcount 不良數量,
            //       round((failcount / workorderqty) * 100, 2) || '%' 不良率,
            //       faillocation 不良位置,
            //       symptomdesc1 不良原因,
            //       count(sysserialno) 數量 ,failcode
            //     from (select b.workorderno,
            //                  d.workorderqty,
            //                  i.skuno,
            //                  g.failcount,
            //                  c.productionline,
            //                  a.sysserialno,
            //                  a.createdate,
            //                  a.lasteditdt,
            //                  a.failcode,
            //                  h.symptomdesc1,
            //                  a.faillocation
            //             from sfcrepairfailcode a
            //            inner join mfworkstatus b
            //               on a.sysserialno = b.sysserialno
            //             left join mfsysevent c
            //               on a.sysserialno = c.sysserialno
            //            inner join mfworkorder d
            //               on b.workorderno = d.workorderno
            //            inner join (select f.workorderno, count(1) failcount
            //                         from sfcrepairfailcode e
            //                        inner join mfworkstatus f
            //                           on e.sysserialno = f.sysserialno
            //                        where e.lasteditdt between
            //                              to_date('', 'yyyy-MM-dd hh24:mi:ss') and
            //                              to_date('', 'yyyy-MM-dd hh24:mi:ss')
            //                          and e.failcategory = 'DEFECT'
            //                        group by f.workorderno) g
            //               on g.workorderno = b.workorderno
            //            inner join SFCCONFIG.SFCFAILURESYMPTOMINFO h
            //               on a.failcode = h.symptomname
            //            inner join mfworkorder i
            //               on b.workorderno = i.workorderno
            //            inner join sfcrepairfailcode j
            //               on j.sysserialno = a.sysserialno and j.createdate = a.createdate
            //            where a.failcategory = 'DEFECT'
            //              and j.failcategory = 'SYMPTOM'
            //              and c.eventname = 'AOI1'
            //              and a.lasteditdt between
            //                              to_date('', 'yyyy-MM-dd hh24:mi:ss') and
            //                              to_date('', 'yyyy-MM-dd hh24:mi:ss') 
            //                               and j.failcode not like 'SMT%'
            //                               and b.skuno = ''
            //                               and c.productionline = ''
            //                               and b.workorderno = ''
            //                                 order by b.workorderno,
            //                      c.productionline,
            //                      a.faillocation,
            //                      a.failcode)
            //     group by workorderno,
            //              workorderqty,
            //              failcount,
            //              productionline,
            //              faillocation,
            //              failcode,
            //              symptomdesc1,
            //              skuno
            //     order by productionline, workorderno, count(1) desc
            #endregion

            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet dsRepair = SFCDB.RunSelect(sqlRun);
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportTable repairReportTable = new ReportTable();
                repairReportTable.LoadData(dsRepair.Tables[0], null);
                repairReportTable.Tittle = "RepairReportTable";                
                Outputs.Add(repairReportTable);              
            }
            catch (Exception exception)
            {                
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }

        }

    }
}
