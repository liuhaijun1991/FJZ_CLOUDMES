using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="SNListByWo.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-03-07 </date>
    /// <summary>
    /// SNListByWo
    /// </summary>
    public class SNListByWo:ReportBase
    {
        ReportInput inputWo = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEventName = new ReportInput() { Name = "EventName", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SNListByWo() {
            Inputs.Add(inputWo);
            Inputs.Add(inputEventName);           
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            //base.Run();
            string wo = inputWo.Value.ToString();
            string eventName = inputEventName.Value.ToString().ToUpper();
            string sqlRun = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow linkRow = null;

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                if (eventName.Equals("REPAIRWIP"))
                {
                    //sqlRun = $@"select distinct sn,next_station  as station,edit_time from r_sn where REPAIR_FAILED_FLAG = 1 and workorderno ='{wo}' and valid_flag='1'";
                    //排除掉在MRB的那些數量，因為MRB數量查詢已經包含這些既在維修又在MRB的了
                    sqlRun = $@"select distinct a.sn,a.next_station  as station,a.edit_time from r_sn a where a.repair_failed_flag = 1 and a.workorderno ='{wo}' and a.valid_flag='1'
                                and not exists (select * from r_mrb b where a.sn = b.sn and b.rework_wo is null)";
                }
                //Why have not Failwip? 2021.12.9
                else if (eventName.Equals("FAILWIP"))
                {
                    sqlRun = $@" select distinct a.sn,a.next_station  as station,a.edit_time from r_sn a where a.workorderno = '{wo}' and  exists(select * from r_test_record t where t.R_sn_ID = a.id and t.messtation = a.next_station and t.state = 'FAIL' ) and (a.REPAIR_FAILED_FLAG <> 1 or a.REPAIR_FAILED_FLAG is null)";
                }
                else if (eventName.Equals("MRB"))
                {
                    sqlRun = $@"select distinct sn,'MRB' as station,edit_time  from r_mrb where workorderno = '{wo}'   and rework_wo is null /* and mrb_flag='1' */ ";
                }
                else if (eventName.ToUpper().Equals("SCRAPED"))
                {
                    sqlRun = $@"select a.sn,a.current_station as station,edit_time from r_sn a where a.workorderno='{wo}' and a.valid_flag='1' and a.scraped_flag='1'";
                }
                else if (eventName.Equals("ORT"))
                {
                    sqlRun = $@"select distinct c.sn,c.next_station as station,a.edit_time as edit_time from r_lot_detail a, r_lot_status b,r_sn c
                                where  b.id = a.lot_id and a.sn=c.sn
                                and b.SAMPLE_STATION in('ORT','ORT-FT2')
                                and c.valid_flag=1
                                and not EXISTS(select a2.sn from r_test_record a2 where a2.sn=a.sn and a2.testation in('ORT','ORT-FT2') and a2.state='PASS' 
                                and a2.endtime >(select max(CREATE_DATE) from r_lot_detail a3 where a3.SN=a.sn  ) ) and b.skuno ='{wo}'
                                UNION ALL 
								select  DISTINCT A.SN,A.next_station  as station,a.edit_time as edit_time From r_sn a,r_ort b where a.sn=b.sn  and valid_flag='1' and a.workorderno='{wo}'
                                 and b.ORTEVENT='ORTIN' AND A.workorderno=B.workorderno and not exists (select 1 from r_ort c where b.sn=c.sn and c.ORTEVENT='ORTOUT') ";
                }
                else
                {
                    //sqlRun = $@"select sn,next_station as station,edit_time  from r_sn where workorderno='{wo}' and next_station='{eventName}'";
                    //sqlRun = $@"select a.sn,a.next_station as station,a.edit_time,b.panel  from r_sn a,r_panel_sn b where a.workorderno='{wo}' and a.next_station='{eventName}' and a.sn=b.sn";
                    if (eventName.Equals("REWORK"))
                    {
                        //sqlRun = $@" select a.sn,a.next_station as station,a.edit_time,b.panel, a.valid_flag as flag  from r_sn a left join r_panel_sn b on a.sn=b.sn where a.workorderno='{wo}' and (a.repair_failed_flag='0' or a.repair_failed_flag is null) and a.next_station='{eventName}' and valid_flag='0' ";
                        sqlRun = $@"select a.sn,
                                            a.next_station as station,
                                            a.edit_time,
                                            b.panel,
                                            a.valid_flag   as flag
                                        from r_sn a
                                        left join r_panel_sn b
                                        on a.sn = b.sn
                                        where a.workorderno = '{wo}'
                                        and a.next_station = '{eventName}'
                                        and valid_flag = '0' ";
                    }
                    else if(eventName.Equals("JOBFINISH"))
                    {
                        sqlRun = $@" select a.sn,a.next_station as station,a.edit_time,b.panel,CASE a.valid_flag when '0' then 'Invalid' when '1' then 'Valid' end as flag from r_sn a left join r_panel_sn b on a.sn=b.sn where a.workorderno='{wo}' and (a.repair_failed_flag='0' or a.repair_failed_flag is null) and a.next_station='{eventName}' ";
                    }
                    else
                    {
                        sqlRun = $@" select a.sn,a.next_station as station,a.edit_time,b.panel, a.valid_flag as flag   from r_sn a left join r_panel_sn b on a.sn=b.sn where a.workorderno='{wo}' and (a.repair_failed_flag='0' or a.repair_failed_flag is null) and a.next_station='{eventName}' and valid_flag in ('1','2') and not exists(select*From r_ort bb  where a.sn=bb.sn and a.id=bb.id) ";
                    }
                }

                RunSqls.Add(sqlRun);
                //OleExec SFCDB = DBPools["SFCDB"].Borrow();
                //try
                //{
                snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                DBPools["SFCDB"].Return(SFCDB);
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("STATION");
                linkTable.Columns.Add("EDIT_TIME");
                linkTable.Columns.Add("PANEL");
                linkTable.Columns.Add("VALID");
                for (int i = 0; i < snListTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + snListTable.Rows[i]["SN"].ToString();
                    linkRow["STATION"] = "";
                    linkRow["EDIT_TIME"] = "";
                    linkRow["PANEL"] = "";
                    linkRow["VALID"] = "";
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
        }
    }
}
