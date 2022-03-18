using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class ActionCodeList : ReportBase
    {
        ReportInput inputActionCode = new ReportInput() { Name = "Code", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        
        public ActionCodeList()
        {
            Inputs.Add(inputActionCode);
        }

        public override void Init()
        {
            base.Init();

            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public override void Run()
        {
            try
            {
                base.Run();
                string actionCode = inputActionCode.Value == null ? inputActionCode.Value.ToString() : inputActionCode.Value.ToString().Trim().ToUpper();

                string runSql = $@"SELECT ACTION_CODE, ENGLISH_DESCRIPTION, CHINESE_DESCRIPTION AS SPANISH_DESCRIPTION FROM SFCBASE.C_ACTION_CODE WHERE 1 = 1";

                if (!String.IsNullOrEmpty(actionCode))
                {
                    runSql += $@" AND ACTION_CODE LIKE '%{actionCode}%'";
                }

                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                try
                {
                    RunSqls.Add(runSql);
                    DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("No Data!");
                    }
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(dt, null);
                    retTab.Tittle = $@"{actionCode} Action Code List";
                    Outputs.Add(retTab);
                }
                catch (Exception exception)
                {
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    Outputs.Add(new ReportAlart(exception.Message));
                }
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}