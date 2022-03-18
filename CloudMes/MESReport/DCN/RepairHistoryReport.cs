using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    /// <summary>
    /// 維修報表:Repair Report(FailDate) For DCN:DCN一大堆各種報表,照抄過來
    /// </summary>
    public class RepairHistoryReport : ReportBase
    {
        ReportInput RouteID = new ReportInput() { Name = "RouteID", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput Old_kp_no = new ReportInput() { Name = "Old_kp_no", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ComponentID = new ReportInput() { Name = "ComponentID", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Location = new ReportInput() { Name = "Location", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SerailNo = new ReportInput() { Name = "SerailNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };        
        ReportInput StartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime2", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime2", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairHistoryReport()
        {
            Inputs.Add(RouteID);
            Inputs.Add(SkuNo);
            Inputs.Add(Station);            
            Inputs.Add(Old_kp_no);
            Inputs.Add(ComponentID);
            Inputs.Add(Location);
            Inputs.Add(SerailNo);
            Inputs.Add(StartDate);
            Inputs.Add(EndDate);
        }

        public override void Init()
        {
            try
            {
                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                InitRouteID(SFCDB);
                InitSkuNo(SFCDB);
                InitStation(SFCDB);
                StartDate.Value = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                EndDate.Value = DateTime.Now.ToString("yyyy-MM-dd");
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Run()
        {
            DateTime sTime = Convert.ToDateTime(StartDate.Value);
            DateTime eTime = Convert.ToDateTime(EndDate.Value).AddDays(1);
            string sValue = sTime.ToString("yyyy/MM/dd");
            string eValue = eTime.ToString("yyyy/MM/dd");

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string skuByRoute = "";
                if (RouteID.Value.ToString() != "ALL")
                {
                    skuByRoute = GetSkuByRouteID(SFCDB, RouteID.Value.ToString());
                }

                #region SQL語句
                #region old
                string runSql = $@"
                select t8.customer_name customer,
                       t3.skuno,
                       t2.sn,
                       t3.fail_station station,
                       t2.FAILURE_SYMPTOM,
                       t2.fail_location location,
                       t2.fail_time,
                       t2.ERROR_CODE,
                       t4.english_description ROOT_CAUSE_DESC,
                       t5.english_description ACTIONDESC,
                       t2.repair_emp,
                       t2.repair_time,
                       case
                         when t2.fail_location = 'FAB' then
                          (select case
                                    when instr(replace(value, 'N/A', ''), '/') > 0 then
                                     substr(replace(value, 'N/A', ''),
                                            0,
                                            instr(replace(value, 'N/A', ''), '/') - 1)
                                    else
                                     value
                                  end date_code
                             from r_sn_kp
                            where sn = t2.sn
                              and scantype = 'FAB'
                              and rownum = 1)
                         else
                          t2.date_code
                       end date_code,
                       case
                         when t2.fail_location = 'FAB' then
                          (select case
                                    when instr(replace(value, 'N/A', ''), '/') > 0 then
                                     substr(replace(value, 'N/A', ''),
                                            instr(replace(value, 'N/A', ''), '/') + 1)
                                    else
                                     value
                                  end lot_code
                             from r_sn_kp
                            where sn = t2.sn
                              and scantype = 'FAB'
                              and rownum = 1)
                         else
                          t2.lot_code
                       end lot_code,
                          t2.compomentid,
                       case
                         when t2.next_station = t3.fail_station then
                          'FAIL'
                         else
                          'PASS'
                       end STATUS
                  from (select t1.*, c.next_station
                          from (select a.sn,
                                       a.action_code,
                                       a.process,
                                       a.reason_code,
                                       a.fail_location,
                                       a.fail_code,
                                       a.date_code,
                                       a.lot_code,
                                       a.repair_emp,
                                       a.repair_time,
                                       b.repair_main_id,
                                       b.fail_time,
                                       b.description FAILURE_SYMPTOM,
                                       a.compomentid
                                  from r_repair_action a, r_repair_failcode b
                                 where a.repair_failcode_id = b.id
                                   and a.sn = b.sn
                                   and a.repair_time between
                                       to_date('{sValue}', 'yyyy-mm-dd') and
                                       to_date('{eValue}', 'yyyy-mm-dd')) t1
                          left join r_sn c
                            on t1.sn = c.sn
                           and c.valid_flag = 1) t2,
                       r_repair_main t3,
                       c_error_code t4,
                       c_action_code t5,
                       c_sku t6,
                       c_series t7,
                       c_customer t8
                 where t2.repair_main_id = t3.id
                   and t2.sn = t3.sn
                   and t2.reason_code = t4.error_code
                   and t2.action_code = t5.action_code
                   and t3.skuno = t6.skuno
                   and t6.c_series_id = t7.id
                   and t7.customer_id = t8.id
                   TEMP_SKUSQL
                   TEMP_STASQL
                   TEMP_SNSQL
                 order by 1, 2, 6";
                #endregion
                runSql = $@"select distinct t8.customer_name customer,
                       t7.series_name,
                       t3.skuno,
                       t2.sn,
                       t3.fail_station station,
                       t2.FAILURE_SYMPTOM,
                       t2.fail_location location,
                       t2.fail_time,
                       t4.ERROR_CODE,
                       t4.english_description ROOT_CAUSE_DESC,
                       t5.english_description ACTIONDESC,
                       t2.repair_emp,
                       t2.repair_time,
					   t2.old_kp_no,
                       case
                         when t2.fail_location = 'FAB' then
                          (select case
                                    when instr(replace(value, 'N/A', ''), '/') > 0 then
                                     substr(replace(value, 'N/A', ''),
                                            0,
                                            instr(replace(value, 'N/A', ''), '/') - 1)
                                    else
                                     value
                                  end date_code
                             from r_sn_kp
                            where sn = t2.sn
                              and scantype = 'FAB'
                              and rownum = 1)
                         else
                          t2.old_date_code
                       end old_date_code,
                       case
                         when t2.fail_location = 'FAB' then
                          (select case
                                    when instr(replace(value, 'N/A', ''), '/') > 0 then
                                     substr(replace(value, 'N/A', ''),
                                            instr(replace(value, 'N/A', ''), '/') + 1)
                                    else
                                     value
                                  end lot_code
                             from r_sn_kp
                            where sn = t2.sn
                              and scantype = 'FAB'
                              and rownum = 1)
                         else
                          t2.old_lot_code
                       end old_lot_code,
											 t2.old_mfr_code,t2.old_mfr_name,
											                                        t2.new_kp_no,
                                       t2.new_date_code,
                                       t2.new_lot_code,
                                       t2.new_mfr_code,
                                       t2.new_mfr_name,
                          t2.compomentid,
                       case
                         when t2.next_station = t3.fail_station then
                          'FAIL'
                         else
                          'PASS'
                       end STATUS
                  from (select t1.*, c.next_station
                          from (select a.sn,
                                       a.action_code,
                                       a.process,
                                       a.reason_code,
                                       a.fail_location,
                                       a.fail_code,
									   case when a.tr_sn is null
											    then a.kp_no 
											else
												''
									   end keypart_kp_no, 
                                       case when a.tr_sn is not null
                                            then a.kp_no 
                                         else
                                            ''
                                        end old_kp_no,
                                       a.date_code as old_date_code,
                                       a.lot_code as old_lot_code,
									   a.mfr_code as old_mfr_code,
									   a.mfr_name as old_mfr_name,
									   a.new_kp_no as new_kp_no,
                                       a.new_date_code as new_date_code,
                                       a.new_lot_code as new_lot_code,
                                       a.new_mfr_code as new_mfr_code,
                                       a.new_mfr_name as new_mfr_name,
                                       a.repair_emp,
                                       a.repair_time,
                                       b.repair_main_id,
                                       b.fail_time,
                                       b.description FAILURE_SYMPTOM,
                                       a.compomentid
                                  from r_repair_action a, r_repair_failcode b
                                 where a.repair_failcode_id = b.id
                                   and a.sn = b.sn
                                   and a.repair_time between
                                       to_date('{sValue}', 'yyyy-mm-dd') and
                                       to_date('{eValue}', 'yyyy-mm-dd')
																	 ) t1
                          left join r_sn c
                            on t1.sn = c.sn
                           and c.valid_flag = 1) t2,
                       r_repair_main t3,
                       c_error_code t4,
                       c_action_code t5,
                       c_sku t6,
                       c_series t7,
                       c_customer t8
                 where t2.repair_main_id = t3.id
                   and t2.sn = t3.sn
                   and t2.reason_code = t4.error_code
                   and t2.action_code = t5.action_code
                   and t3.skuno = t6.skuno
                   and t6.c_series_id = t7.id
                   and t7.customer_id = t8.id
                   TEMP_SKUSQL
                   TEMP_STASQL
                   TEMP_KPSQL
                   TEMP_COMIDSQL
                   TEMP_LOCSQL
                   TEMP_SNSQL
                 order by 1, 2, 6";

                if (SkuNo.Value.ToString() != "ALL")
                {
                    runSql = runSql.Replace("TEMP_SKUSQL", $@" and t3.skuno = '{SkuNo.Value.ToString()}' ");
                }
                else
                {
                    if (RouteID.Value.ToString() != "ALL" && skuByRoute != "")
                    {
                        runSql = runSql.Replace("TEMP_SKUSQL", $@" and t3.skuno = '{skuByRoute}' ");
                    }
                    else
                    {
                        runSql = runSql.Replace("TEMP_SKUSQL", " ");
                    }
                }
                if (Station.Value.ToString() != "ALL")
                {
                    runSql = runSql.Replace("TEMP_STASQL", $@" and t3.fail_station = '{Station.Value.ToString()}' ");
                }
                else
                {
                    runSql = runSql.Replace("TEMP_STASQL", " ");
                }
                if (!string.IsNullOrEmpty(Old_kp_no.Value.ToString()))
                {
                    runSql = runSql.Replace("TEMP_KPSQL", $@" and t2.old_kp_no = '{Old_kp_no.Value.ToString()}' ");
                }
                else
                {
                    runSql = runSql.Replace("TEMP_KPSQL", " ");
                }
                if (!string.IsNullOrEmpty(ComponentID.Value.ToString()))
                {
                    runSql = runSql.Replace("TEMP_COMIDSQL", $@" and t2.compomentid = '{ComponentID.Value.ToString()}' ");
                }
                else
                {
                    runSql = runSql.Replace("TEMP_COMIDSQL", " ");
                }
                if (!string.IsNullOrEmpty(Location.Value.ToString()))
                {
                    runSql = runSql.Replace("TEMP_LOCSQL", $@" and t2.fail_location = '{Location.Value.ToString()}' ");
                }
                else
                {
                    runSql = runSql.Replace("TEMP_LOCSQL", " ");
                }
                if (!string.IsNullOrEmpty(SerailNo.Value.ToString()))
                {
                    runSql = runSql.Replace("TEMP_SNSQL", $@" and t2.sn = '{SerailNo.Value.ToString()}' ");
                }
                else
                {
                    runSql = runSql.Replace("TEMP_SNSQL", " ");
                }
                

                #endregion

                DataTable resDT = SFCDB.RunSelect(runSql).Tables[0];
                ReportTable retTab = new ReportTable();
                retTab.LoadData(resDT, null);
                retTab.Tittle = "Repair History Report";
                Outputs.Add(retTab);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }

        public void InitRouteID(OleExec db)
        {
            List<string> routeList = new List<string>();
            DataTable routeDT = db.ORM.Queryable<C_ROUTE, R_SKU_ROUTE>((cr, rsr) => cr.ID == rsr.ROUTE_ID)
                .GroupBy((cr, rsr) => cr.ROUTE_NAME).Select((cr, rsr)=>cr.ROUTE_NAME).OrderBy("ROUTE_NAME").ToDataTable();
            routeList.Add("ALL");
            foreach (DataRow dr in routeDT.Rows)
            {
                routeList.Add(dr["ROUTE_NAME"].ToString());
            }
            RouteID.ValueForUse = routeList;
        }

        public void InitSkuNo(OleExec db)
        {
            List<string> skuList = new List<string>();
            T_C_SKU T_CSKU = new T_C_SKU(db, DB_TYPE_ENUM.Oracle);
            DataTable skuDT = T_CSKU.GetALLSkuno(db);
            skuList.Add("ALL");
            foreach (DataRow dr in skuDT.Rows)
            {
                skuList.Add(dr["SKUNO"].ToString());
            }
            SkuNo.ValueForUse = skuList;
        }

        public void InitStation(OleExec db)
        {
            List<string> staList = new List<string>();
            T_C_ROUTE_DETAIL T_CRD = new T_C_ROUTE_DETAIL(db, DB_TYPE_ENUM.Oracle);
            DataTable staDT = T_CRD.GetALLStation(db);
            staList.Add("ALL");
            foreach (DataRow dr in staDT.Rows)
            {
                staList.Add(dr["STATION_NAME"].ToString());
            }
            Station.ValueForUse = staList;
        }

        public string GetSkuByRouteID(OleExec db, string routeID)
        {
            DataTable skuDT = db.ORM.Queryable<C_SKU, R_SKU_ROUTE, C_ROUTE>((cs, rsr, cr) => cs.ID == rsr.SKU_ID && rsr.ROUTE_ID == cr.ID)
                .Where((cs, rsr, cr) => cr.ROUTE_NAME == routeID).GroupBy((cs, rsr, cr) => cs.SKUNO).Select((cs, rsr, cr) => cs.SKUNO).ToDataTable();
            if (skuDT.Rows.Count == 0)
            {
                //throw new Exception(routeID + " 對應機種不存在,請確認!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816162631", new string[] { routeID }));
            }

            return skuDT.Rows[0]["SKUNO"].ToString();
        }
    }
}
