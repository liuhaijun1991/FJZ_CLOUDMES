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
    public class SFC_FQA_REPORT : ReportBase
    {
        ReportInput _SKUNO = new ReportInput() { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput _ORDER_BY = new ReportInput() { Name = "ORDER_BY", InputType = "Select", Value = "PALLET", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "PALLET", "CARTON" } };

        public SFC_FQA_REPORT()
        {
            Inputs.Add(_SKUNO);
            Inputs.Add(_ORDER_BY);
        }
        public override void Run()
        {

            //DataRow linkDataRow = null;
            string SKUNO = _SKUNO.Value.ToString();
            string ORDER_BY = _ORDER_BY.Value.ToString();
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                string sqlString = "";
                string bu = SFCDB.ORM.Queryable<MESDataObject.Module.C_BU>().Select(r => r.BU).ToList().Distinct().FirstOrDefault();
                if (ORDER_BY == "PALLET")
                {
                    if (bu == "VNJUNIPER")
                    {
                        var WOlist = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.SKUNO == SKUNO).ToList();
                        if (WOlist.Count == 0)
                        {
                            sqlString = $@"select GROUPID SKUNO, pallet, count(1) qty,max(edit_time) edit_time
                                      from (  select r.Sn SN, R_WO.GROUPID, p.pack_no Carton, pl.pack_no pallet, r.edit_time
                                             from r_sn r
                                             inner join r_pre_wo_head R_WO
                                                on R.WORKORDERNO = R_WO.WO
                                            inner join r_sn_packing sp
                                                on r.id = sp.sn_id
                                             inner join r_packing p
                                               on sp.pack_id = p.id
                                            inner join r_packing pl
                                                on p.parent_pack_id = pl.id
                                             WHERE 
                                             r.completed_flag = 1
                                               and r.shipped_flag = 0
                                               and r.current_station in  ('CBS','CBS2')
                                              and r.valid_flag = 1
                                               and (R_WO.GROUPID = '{SKUNO}' or r.skuno = '{SKUNO}')
                                             order by 3, 2)
                                     group by GROUPID, pallet
                                     order by edit_time";
                        }
                        else if (WOlist[0].PLANT.ToString() == "VUEA" && WOlist.Count != 0)
                        {
                            sqlString = $@"SELECT  SKUNO, pallet, count(1) qty,max(edit_time) edit_time
                                      from (  select r.Sn SN,R.SKUNO, p.pack_no Carton, pl.pack_no pallet, r.edit_time
                                             from r_sn r
                                            inner join r_sn_packing sp
                                                on r.id = sp.sn_id
                                             inner join r_packing p
                                               on sp.pack_id = p.id
                                            inner join r_packing pl
                                                on p.parent_pack_id = pl.id
                                             WHERE 
                                             r.completed_flag = 1
                                               and r.shipped_flag = 0
                                               and r.current_station in  ('CBS','CBS2')
                                              and r.valid_flag = 1
                                               and r.skuno = '{SKUNO}'
                                             order by 3, 2)
                                     group by SKUNO, pallet
                                     order by edit_time";

                        }
                        else
                        {
                            sqlString = $@"select GROUPID SKUNO, pallet, count(1) qty,max(edit_time) edit_time
                                      from (  select r.Sn SN, R_WO.GROUPID, p.pack_no Carton, pl.pack_no pallet, r.edit_time
                                             from r_sn r
                                             inner join r_pre_wo_head R_WO
                                                on R.WORKORDERNO = R_WO.WO
                                            inner join r_sn_packing sp
                                                on r.id = sp.sn_id
                                             inner join r_packing p
                                               on sp.pack_id = p.id
                                            inner join r_packing pl
                                                on p.parent_pack_id = pl.id
                                             WHERE 
                                             r.completed_flag = 1
                                               and r.shipped_flag = 0
                                               and r.current_station in  ('CBS','CBS2')
                                              and r.valid_flag = 1
                                               and (R_WO.GROUPID = '{SKUNO}' or r.skuno = '{SKUNO}')
                                             order by 3, 2)
                                     group by GROUPID, pallet
                                     order by edit_time";
                        }
                    }
                    else if (bu == "VERTIV")
                    {
                        sqlString = $@"select aa.*,wo.workorder_type from (
                                select skuno, workorderno, pallet, count(1) qty,max(edit_time) edit_time
                                  from(select r.Sn SN, r.skuno, r.workorderno, p.pack_no Carton, pl.pack_no pallet, r.edit_time
                                         from r_sn r
                                        inner
                                         join r_sn_packing sp
                                       on r.id = sp.sn_id
                                    inner
                                         join r_packing p
                                     on sp.pack_id = p.id
                                  inner
                                         join r_packing pl
                                       on p.parent_pack_id = pl.id                                       
                                         where r.completed_flag = 1
                                           and r.shipped_flag = 0
                                           and r.current_station in ('CBS', 'CBS2')
                                           and r.valid_flag = 1
                                           and r.skuno = '{SKUNO}'                                          
                                         order by 3, 2)
                                 group by skuno ,pallet,workorderno
                                order by edit_time) aa left join r_wo_type wo on instr(aa.workorderno, wo.prefix, 1) > 0";
                    }
                    else
                    {
                        sqlString = $@"SELECT a.*,c.WH_NAME FROM  (select skuno, pallet, count(1) qty,max(edit_time) edit_time
                              from (select r.Sn SN,r.skuno, p.pack_no Carton, pl.pack_no pallet,r.edit_time
                                     from r_sn r
                                    inner join r_sn_packing sp
                                        on r.id = sp.sn_id
                                     inner join r_packing p
                                       on sp.pack_id = p.id
                                    inner join r_packing pl
                                        on p.parent_pack_id = pl.id
                                     where r.completed_flag = 1
                                       and r.shipped_flag = 0
                                       and r.current_station in  ('CBS','CBS2')
                                      and r.valid_flag = 1
                                       and r.skuno = '{SKUNO}'
                                     order by 3, 2)
                             group by skuno ,pallet
                             order by edit_time) a  LEFT JOIN SFCBASE.C_WAREHOUSE_PALLET_POSITION_T b ON a.pallet=b.PALLET_NO AND b.OUT_FLAG=0
                             LEFT JOIN SFCBASE.C_WAREHOUSE_CONFIG_T c ON b.WH_ID=c.wh_id ORDER by a.edit_time asc";
                    }
                }
                else
                {
                    if (bu == "VERTIV")
                    {
                        sqlString = $@"select aa.*,wo.workorder_type from (
                                select skuno ,workorderno,Carton, count(1) qty,max(edit_time) edit_time
                                  from (select r.Sn SN,r.skuno,r.workorderno, p.pack_no Carton, pl.pack_no pallet,r.edit_time
                                          from r_sn r
                                         inner join r_sn_packing sp
                                           on r.id = sp.sn_id
                                        inner join r_packing p
                                           on sp.pack_id = p.id
                                         inner join r_packing pl
                                           on p.parent_pack_id = pl.id                                        
                                         where r.completed_flag = 1
                                          and r.shipped_flag = 0
                                           and r.current_station  in  ('CBS','CBS2')
                                          and r.valid_flag = 1
                                           and r.skuno = '{SKUNO}'                                           
                                        order by 3, 2)
                                 group by skuno, Carton,workorderno
                                order by edit_time ) aa left join r_wo_type wo on instr(aa.workorderno,wo.prefix,1)>0 ";

                    }
                    else
                    {
                        sqlString = $@"select skuno , Carton, count(1) qty,max(edit_time) edit_time
                          from (select r.Sn SN,r.skuno, p.pack_no Carton, pl.pack_no pallet,r.edit_time
                                  from r_sn r
                                 inner join r_sn_packing sp
                                   on r.id = sp.sn_id
                                inner join r_packing p
                                   on sp.pack_id = p.id
                                 inner join r_packing pl
                                   on p.parent_pack_id = pl.id
                                 where r.completed_flag = 1
                                  and r.shipped_flag = 0
                                   and r.current_station  in  ('CBS','CBS2')
                                  and r.valid_flag = 1
                                   and r.skuno = '{SKUNO}'
                                order by 3, 2)
                         group by skuno, Carton
                         order by edit_time";
                    }
                }
                if (bu == "VERTIV")
                {
                    DataTable resultDt = SFCDB.RunSelect(sqlString).Tables[0];
                    DataTable showTable = new DataTable();
                    foreach (DataColumn c in resultDt.Columns)
                    {
                        showTable.Columns.Add(c.ColumnName);
                    }
                    showTable.Columns.Add("HOLD");
                    foreach (DataRow row in resultDt.Rows)
                    {
                        DataRow newRow = showTable.NewRow();
                        foreach (DataColumn col in resultDt.Columns)
                        {
                            newRow[col.ColumnName] = row[col.ColumnName].ToString();
                        }

                        string holdSql = "";
                        if (ORDER_BY == "PALLET")
                        {
                            holdSql = $@"select sn.* from r_packing p,r_packing c,r_sn_packing sp,r_sn sn,r_sn_lock l
                                            where p.id=c.parent_pack_id and c.id=sp.pack_id and sp.sn_id=sn.id and (sn.sn=l.sn or sn.id=l.sn)
                                            and c.pack_no='{row["PALLET"].ToString()}' and l.lock_status=1";
                        }
                        else
                        {
                            holdSql = $@"select sn.* from r_packing c,r_sn_packing sp,r_sn sn,r_sn_lock l
                                        where c.id=sp.pack_id and sp.sn_id=sn.id and (sn.sn=l.sn or sn.id=l.sn)
                                        and c.pack_no='{row["CARTON"].ToString()}' and l.lock_status=1";
                        }
                        DataTable temp = SFCDB.RunSelect(holdSql).Tables[0];
                        bool bLock = SFCDB.ORM.Queryable<R_SN_LOCK>().Any(r => r.WORKORDERNO == row["WORKORDERNO"].ToString() && r.LOCK_STATUS == "1");

                        if (temp.Rows.Count > 0 || bLock)
                        {
                            newRow["HOLD"] = "YES";
                        }
                        else
                        {
                            newRow["HOLD"] = "NO";
                        }
                        showTable.Rows.Add(newRow);
                    }
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(showTable, null);
                    retTab.Tittle = "SFC_FQA_REPORT";
                    Outputs.Add(retTab);
                }
                else
                {
                    var ret = SFCDB.RunSelect(sqlString);
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(ret.Tables[0], null);
                    retTab.Tittle = "SFC_FQA_REPORT";
                    Outputs.Add(retTab);
                }

            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }
    }
}
