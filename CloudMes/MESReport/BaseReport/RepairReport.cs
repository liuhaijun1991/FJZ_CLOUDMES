using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;

namespace MESReport.BaseReport
{
    //維修報表

    public class RepairReport : ReportBase
    {

        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2017/02/01 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018/02/12 12:00:00", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SkuNo = new ReportInput() { Name = "SkuNo", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL" } };
        ReportInput STATION = new ReportInput() { Name = "STATION", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput WO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ROOTCAUSE = new ReportInput() { Name = "ROOTCAUSE", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput LOCATION = new ReportInput() { Name = "LOCATION", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput KP_NO = new ReportInput() { Name = "KP_NO", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput REPAIR_EMP = new ReportInput() { Name = "REPAIR_EMP", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public RepairReport()
        {
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
            Inputs.Add(SN);
            Inputs.Add(SkuNo);
            Inputs.Add(STATION);
            Inputs.Add(WO);
            Inputs.Add(ROOTCAUSE);
            Inputs.Add(LOCATION);
            Inputs.Add(KP_NO);
            Inputs.Add(REPAIR_EMP);
        }

        public override void Init()
        {
            OleExec SFCDB = null;
            try
            {
                StartTime.Value = DateTime.Now.AddDays(-1);
                EndTime.Value = DateTime.Now;
                SFCDB = DBPools["SFCDB"].Borrow();
                InitSkuno(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }
        public override void Run()
        {
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                //string runSql = $@"select rrm.sn,
                //       rrm.fail_station,
                //       rra.action_code,
                //       rrf.fail_code,
                //       rrm.workorderno,
                //       rrm.skuno,                      
                //       rra.process,
                //       rra.reason_code as RootCause,
                //       cec.chinese_description,
                //       rra.fail_location,
                //       rra.kp_no,
                //       rra.date_code,
                //       rra.lot_code,
                //       rra.mfr_name,
                //       rra.description,
                //       rra.repair_emp,
                //       rrm.CREATE_TIME,
                //       rrm.edit_time,
                //       rra.ID as Attachment
                //  from r_repair_main     rrm,
                //       r_repair_failcode rrf,
                //       c_error_code      cec,
                //       r_repair_action   rra
                // where 1=1
                //   --and rrm.sn = rrf.sn
                //   and cec.error_code = rra.reason_code(+)
                //   and rrf.id=rra.repair_failcode_id
                //   and rrf.repair_main_id=rrm.id
                //   --and rra.sn = rrf.sn
                //   and rrm.EDIT_TIME BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') 
                //   AND TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS') ";----LDL--2022-01-06


                string runSql = $@"select rrm.sn,
                       rrm.fail_station,
                       rra.action_code,
                       rrf.fail_code,
                       rtdv.ERROR_CODE,
                       rrm.workorderno,
                       rrm.skuno,                      
                       rra.process,
                       rra.reason_code as RootCause,
                       cec.chinese_description,
                       rra.fail_location,
                       rra.kp_no,
                       rra.date_code,
                       rra.lot_code,
                       rra.mfr_name,
                       rra.description,
                       rra.repair_emp,
                       rrm.CREATE_TIME,
                       rrm.edit_time,
                       rra.ID as Attachment
                  from r_repair_main     rrm,
                       r_repair_failcode rrf,
                       c_error_code      cec,
                       r_repair_action   rra,
                       (select *
                              from (select a.*,
                                           ROW_NUMBER() OVER(PARTITION BY sn ORDER BY CREATETIME DESC) rn
                                      from r_test_detail_vertiv a where a.state='FAIL')
                             where rn = 1) rtdv
                 where 1=1
                   --and rrm.sn = rrf.sn
                   and cec.error_code = rra.reason_code(+)
                   and rrf.id=rra.repair_failcode_id
                   and rrf.repair_main_id=rrm.id
                   --and rra.sn = rrf.sn
                   and rrm.sn = rtdv.sn(+)
                   and rrm.fail_station=rtdv.station(+)
                   and rrm.EDIT_TIME BETWEEN TO_DATE ('{svalue}','YYYY/MM/DD HH24:MI:SS') 
                   AND TO_DATE ('{evalue}', 'YYYY/MM/DD HH24:MI:SS') ";




                if (SN.Value.ToString() != "ALL"&& SN.Value.ToString() != string.Empty)
                {
                    runSql += $@" and rrm.SN = '{ SN.Value.ToString()}'";
                }
                if (SkuNo.Value.ToString() != "ALL"&& SkuNo.Value.ToString() != string.Empty)
                {
                    runSql += $@" and rrm.skuno = '{SkuNo.Value.ToString()}'";
                }
                if (STATION.Value.ToString() != "ALL" && STATION.Value.ToString() != string.Empty)
                {
                    runSql += $@" and  rrm.fail_station = '{ STATION.Value.ToString()}'";
                }
                if (WO.Value.ToString() != "ALL" && WO.Value.ToString() != string.Empty)
                {
                    runSql += $@" and  rrm.workorderno = '{ WO.Value.ToString()}'";
                }
                if (ROOTCAUSE.Value.ToString() != "ALL" && ROOTCAUSE.Value.ToString() != string.Empty)
                {
                    runSql += $@" and rra.reason_code = '{ ROOTCAUSE.Value.ToString()}'";
                }
                if (LOCATION.Value.ToString() != "ALL" && LOCATION.Value.ToString() != string.Empty)
                {
                    runSql += $@" and rra.fail_location = '{ LOCATION.Value.ToString()}'";
                }
                if (KP_NO.Value.ToString() != "ALL" && KP_NO.Value.ToString() != string.Empty)
                {
                    runSql += $@" and rra.kp_no = '{ KP_NO.Value.ToString()}'";
                }
                if (REPAIR_EMP.Value.ToString() != "ALL" && REPAIR_EMP.Value.ToString() != string.Empty)
                {
                    runSql += $@" and rra.repair_emp = '{ REPAIR_EMP.Value.ToString()}'";
                }

                runSql += $@" order by sn ,workorderno,CREATE_TIME";
                RunSqls.Add(runSql);
                DataSet res = SFCDB.RunSelect(runSql);
                ReportTable retTab = new ReportTable();

                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }

                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = 
                        "Attachment#{Class:'MESStation.Config.JNP.JuniperRepairStation',Function:'GetRepairAttachment',Paras:{ACTIONID:'"+ 
                        row["Attachment"].ToString() + "'}}";
                    linkRow = linkTable.NewRow();

                    
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        
                        if (dc.ColumnName.ToString().ToUpper() == "Attachment".ToUpper())
                        {
                            linkRow["Attachment"] = linkURL;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }

                retTab.LoadData(res.Tables[0], linkTable);
                retTab.Tittle = "Repair Report";
                Outputs.Add(retTab);
                ///"Attachment#{Class:'MESSTATIIN.dddddd',Function:'',Params:{ACTIONID:''}"
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }

        }

        public void InitSkuno(OleExec db)
        {
            List<string> skuno = new List<string>();
            //DataTable dt = new DataTable();
            //T_C_SKU sku = new T_C_SKU(db, DB_TYPE_ENUM.Oracle);
            //dt = sku.GetALLSkuno(db);
            
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string strSql = $@"select distinct skuno from c_sku order by skuno";
            try {

                DataTable dt = SFCDB.ExecuteDataTable(strSql, CommandType.Text);
                skuno.Add("ALL");
                foreach (DataRow dr in dt.Rows)
                {
                    skuno.Add(dr["SKUNO"].ToString());

                }
                if (SFCDB != null) {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                SkuNo.ValueForUse = skuno;
            } catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            } 
        }
    }
}
