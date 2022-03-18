using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class SNBeenLinkReport:ReportBase
    {
        ReportInput woObj = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput snObj = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SNBeenLinkReport()
        {
            Inputs.Add(woObj);
            Inputs.Add(snObj);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            //base.Run();
            string sn = snObj.Value.ToString();
            string wo = woObj.Value.ToString();
            string sqlNoLinkSN = "";
            string sqlNoLinkWO = "";
            string sqlLinkSN_1 = "";
            string sqlLinkSN_2 = "";
            string sqlLinkSN_3 = "";
            string sqlLinkWO_1 = "";
            string sqlLinkWO_2 = "";
            string sqlNoLink = "";
            string sqlLink = "";
            if (sn == "" && wo == "")
            {
                ReportAlart alart = new ReportAlart("Please input a sn or wo");
                Outputs.Add(alart);
                return;
            }
            if (sn != "")
            {
                sqlNoLinkSN = $@" and (a.sn='{sn}' or a.boxsn='{sn}') ";
                sqlLinkSN_1 = $@" and (a.sn='{sn}' or a.boxsn='{sn}') ";
                sqlLinkSN_2 = $@" and (d.sn='{sn}' or d.boxsn='{sn}') ";
                sqlLinkSN_3 = $@" and kp.value = '{sn}'";
            }
            if (wo != "")
            {
                sqlNoLinkWO = $@" and a.workorderno='{wo}' ";
                sqlLinkWO_1 = $@" and a.workorderno='{wo}' ";
                sqlLinkWO_2 = $@" and d.workorderno='{wo}' ";
            }
            sqlNoLink = $@"select a.sn,a.skuno,a.workorderno,a.completed_flag,a.completed_time,a.shipped_flag,a.shipdate,a.current_station,a.next_station,a.edit_time from r_sn a
                             where 1=1 {sqlNoLinkSN} {sqlNoLinkWO} and a.valid_flag='1' and not exists (select * from r_sn_kp b where a.sn=b.value)
                             and not exists  (select * from r_sn_keypart_detail c where a.sn=c.keypart_sn) ";
            //當sqlLinkSN_3為空時,最後一個查詢語句的查詢速度非常慢，故去掉
            if (string.IsNullOrEmpty(sqlLinkSN_3))
            {
                sqlLink = $@"select c.workorderno as wo,c.sn, a.workorderno as keypart_wo,a.sn as keypart_sn,b.station as link_station,b.edit_time as link_time,b.edit_emp as link_by
                         from r_sn a,r_sn_kp b,r_sn c where a.sn=b.value and c.id=b.r_sn_id and b.valid_flag='1'
                         {sqlLinkSN_1} {sqlLinkWO_1} and a.valid_flag='1' and not exists (select * from r_sn_keypart_detail n where a.sn=n.keypart_sn)
                        union
                        select distinct f.workorderno as wo,f.sn, d.workorderno as keypart_wo,d.sn as keypart_sn,e.station_name  as link_station,e.edit_time as link_time,e.edit_emp as link_by
                         from r_sn d,r_sn_keypart_detail e,r_sn f where d.sn=e.keypart_sn and e.sn=f.sn and f.valid_flag='1' and d.valid_flag='1'
                          {sqlLinkSN_2} {sqlLinkWO_2} ";
            }
            else
            {
                sqlLink = $@"select c.workorderno as wo,c.sn, a.workorderno as keypart_wo,a.sn as keypart_sn,b.station as link_station,b.edit_time as link_time,b.edit_emp as link_by
                         from r_sn a,r_sn_kp b,r_sn c where a.sn=b.value and c.id=b.r_sn_id and b.valid_flag='1'
                         {sqlLinkSN_1} {sqlLinkWO_1} and a.valid_flag='1' and not exists (select * from r_sn_keypart_detail n where a.sn=n.keypart_sn)
                        union
                        select distinct f.workorderno as wo,f.sn, d.workorderno as keypart_wo,d.sn as keypart_sn,e.station_name  as link_station,e.edit_time as link_time,e.edit_emp as link_by
                         from r_sn d,r_sn_keypart_detail e,r_sn f where d.sn=e.keypart_sn and e.sn=f.sn and f.valid_flag='1' and d.valid_flag='1'
                          {sqlLinkSN_2} {sqlLinkWO_2} 
                         union
                        select s.workorderno as wo,
                               s.sn,
                               '' keypart_wo,
                               kp.value keypart_sn,
                               kp.station link_station,
                               kp.edit_time link_time,
                               kp.edit_emp link_by
                          from r_sn s, r_sn_kp kp
                         where s.sn = kp.sn
                           and s.id = kp.r_sn_id
                            {sqlLinkSN_3} 
                           and kp.valid_flag = 1
                           and not exists (select * from r_sn r where r.sn = kp.value)
                        union
                        select sn.workorderno as wo,
                               sn.sn,
                               '' as keypart_wo,
                               kp.value as keypart_sn,
                               kp.station as link_station,
                               kp.edit_time as link_time,
                               kp.edit_emp as link_by
                          from r_sn sn,
                               (select *
                                  from r_sn_kp rsnkp
                                 where rsnkp.valid_flag='1' and not exists
                                 (select * from r_sn rsn where rsnkp.value = rsn.sn)) kp
                         where sn.id = kp.r_sn_id {sqlLinkSN_3}  ";
            }
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            DataTable dtLink = new DataTable();
            DataTable dtNoLnk = new DataTable();
            try
            {
                dtNoLnk = sfcdb.RunSelect(sqlNoLink).Tables[0];
                dtLink = sfcdb.RunSelect(sqlLink).Tables[0];
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
               
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dtNoLnk, null);
                reportTable.Tittle = "SN no link detail";
                Outputs.Add(reportTable);

                ReportTable retTab2 = new ReportTable();
                retTab2.LoadData(dtLink, null);
                retTab2.Tittle = "SN link detail";
                Outputs.Add(retTab2);
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
