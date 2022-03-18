using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.HWD
{
    public class R7B5XMLTUpdateList : ReportBase
    {       
        ReportInput input_type = new ReportInput() { Name = "Type", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput input_task_no = new ReportInput() { Name = "TaskNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput input_date_from = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput input_date_to = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public R7B5XMLTUpdateList()
        {
            Inputs.Add(input_type);
            Inputs.Add(input_task_no);
            Inputs.Add(input_date_from);
            Inputs.Add(input_date_to);          
            string sqlGetType = "select 'ALL' as Type from dual union select distinct type from R_7B5_UPDATE_LIST order by type";
            Sqls.Add("SqlGetType", sqlGetType);
        }

        public override void Init()
        {
            base.Init();
            input_date_from.Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            input_date_to.Value = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            input_type.ValueForUse = GetSeleteType();
        }

        public override void Run()
        {
            base.Run();
            string sql = $@"SELECT  TASK_NO, PLAN_FLAG as PLAN_FLAG_OLD, QTY as QTY_OLD,DATA1 as QTY_NEW,PO_FLAG as PO_FLAG_OLD,CANCEL_FLAG as CANCEL_FLAG_OLD,
                            REMARK,DATA2 as TASK_CHANGE_CONFIRM_OLD,DATA3 as TASK_CHANGE_CONFIRM_NEW,LASTEDITBY as EDIT_BY,LASTEDITdt as EDIT_DT  FROM R_7B5_UPDATE_LIST WHERE 1 = 1 ";
            if (input_type.Value != null && !string.IsNullOrEmpty(input_type.Value.ToString()))
            {
                sql = sql + $@" and TYPE ='{input_task_no.Value.ToString()}' ";
            }
            if (input_task_no.Value != null && !string.IsNullOrEmpty(input_task_no.Value.ToString()))
            {
                sql = sql + $@" and TASK_NO ='{input_task_no.Value.ToString()}' ";
            }            

            if (input_date_from.Value != null && !string.IsNullOrEmpty(input_date_from.Value.ToString()) && input_date_to.Value != null && !string.IsNullOrEmpty(input_date_to.Value.ToString()))
            {
                sql = sql + $@" AND LASTEDITDT BETWEEN TO_DATE('{input_date_from.Value.ToString()}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{input_date_to.Value.ToString()}','YYYY/MM/DD HH24:MI:SS')";
            }

            sql = sql + $@" order by lasteditdt ";
            RunSqls.Add(sql);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet ds7B5Plan = SFCDB.RunSelect(sql);
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                if (ds7B5Plan.Tables[0].Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(ds7B5Plan.Tables[0], null);
                reportTable.Tittle = "R7B5XMLTUpdateList";
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportAlart alart = new ReportAlart(exception.Message);
                Outputs.Add(alart);
            }
        }

        private string[] GetSeleteType()
        {
            List<string> listType = new List<string>();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet dsSkuno = SFCDB.RunSelect(Sqls["SqlGetType"]);
                foreach (DataRow row in dsSkuno.Tables[0].Rows)
                {
                    listType.Add(row[0].ToString());
                }
                return listType.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
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
