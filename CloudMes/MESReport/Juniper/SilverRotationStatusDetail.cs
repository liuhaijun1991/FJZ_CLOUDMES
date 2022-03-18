using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MESReport.Juniper
{
    public class SilverRotationStatusDetail : ReportBase
    {
        ReportInput inputPN = new ReportInput { Name = "PN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput { Name = "TYPE", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SilverRotationStatusDetail()
        {
            this.Inputs.Add(inputPN);
            this.Inputs.Add(inputType);
        }

        public override void Init()
        {
            //base.Init();
            this.Sqls.Add("ControlDay", "select value  from r_function_control where functionname = 'SilverRotationControl' and category = 'ControlDay' and controlflag = 'Y' and functiontype = 'NOSYSTEM'");
            this.Sqls.Add("ControlTime", "select value from r_function_control where functionname = 'SilverRotationControl' and category = 'ControlTime' and controlflag = 'Y' and functiontype = 'NOSYSTEM'");
        }

        public override void Run()
        {
            //base.Run();
            string sqlPN = "";
            string type = "";
            string controlDay = "";
            string controlTime = "";
            string runSql = "";
            DataTable controlTable = new DataTable();
            DataTable showTable = new DataTable();
            if (inputPN.Value == null)
            {
                ReportAlart alart = new ReportAlart("Please input PN");
                Outputs.Add(alart);
                return;
            }
            if (inputType.Value == null)
            {
                ReportAlart alart = new ReportAlart("Please input TYPE");
                Outputs.Add(alart);
                return;
            }
            sqlPN = $@" and rsr.skuno = '{inputPN.Value.ToString()}' ";
            type = inputType.Value.ToString();
            OleExec db = this.DBPools["SFCDB"].Borrow();
            try
            {
                controlTable = db.RunSelect(this.Sqls["ControlDay"]).Tables[0];
                if (controlTable.Rows.Count == 0 || controlTable.Rows[0][0].ToString() == "")
                {
                    throw new Exception(" the control day can't setting!");
                }
                else
                {
                    controlDay = controlTable.Rows[0][0].ToString();
                }

                controlTable = db.RunSelect(this.Sqls["ControlTime"]).Tables[0];
                if (controlTable.Rows.Count == 0 || controlTable.Rows[0][0].ToString() == "")
                {
                    throw new Exception(" the control time can't setting!");
                }
                else
                {
                    controlTime = controlTable.Rows[0][0].ToString();
                }
                runSql = $@"select pn,
                                       sn,
                                       FIRST_ROTATION_TIME as FIRST_ROTATION_TIME,
                                       total_tested_times,
                                       ({controlTime} - total_tested_times) as remaining_test_time,
                                       total_tested_day,
                                       ({controlDay} - total_tested_day) as remaining_test_day,
                                       CHECK_OUT_TIME as CHECK_OUT_TIME,
                                       CHECK_OUT_EMP as CHECK_OUT_EMP
                                  from (select rsr.skuno as pn,
                                               rsr.sn,
                                               decode(rsr.TOLAL_ROTATION_TIMES, null, 0, rsr.TOLAL_ROTATION_TIMES) total_tested_times,
                                               ROUND(TO_NUMBER(sysdate - rsr.FIRST_ROTATION_TIME)) total_tested_day,
                                               rsr.FIRST_ROTATION_TIME,
                                               rsr.CHECK_OUT_TIME,
                                               rsr.CHECK_OUT_EMP
                                          from r_silver_rotation rsr
                                         where rsr.status = '0' {sqlPN} ";
                if (type == "0") //TYPE=0 表示要查TOTAL_QTY
                {
                    runSql = runSql + " )";                   
                }
                else if (type == "1") //TYPE=1 表示要查IN_TESTING_QTY
                {
                    runSql = runSql + $@"  and exists (select *
                                                  from r_silver_rotation_detail rsrd
                                                 where rsrd.csn = rsr.sn
                                                   and rsrd.endtime is null))";                   
                }
                else if (type == "2") //TYPE=2 表示要查AVAILABLE_QTY
                {
                    runSql = runSql + $@"  and not exists (select *
                                                  from r_silver_rotation_detail rsrd
                                                 where rsrd.csn = rsr.sn
                                                   and rsrd.endtime is null))";
                }
                else
                {
                    ReportAlart alart = new ReportAlart("TYPE error !");
                    Outputs.Add(alart);
                    return;
                }

                showTable = db.RunSelect(runSql).Tables[0];
                if (db != null)
                {
                    this.DBPools["SFCDB"].Return(db);
                }
                if (showTable.Rows.Count == 0)
                {
                    throw new Exception("No data");
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(showTable, null);
                reportTable.Tittle = "Silver Rotation Status Detail Report";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                if (db != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }  
        }
    }
}
