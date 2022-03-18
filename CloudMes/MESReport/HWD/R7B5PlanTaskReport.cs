using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.HWD
{
    public class R7B5PlanTaskReport : ReportBase
    {

        ReportInput taskNo = new ReportInput() { Name = "TaskNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput item = new ReportInput() { Name = "ITEM", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput model = new ReportInput() { Name = "Model", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput cancelFlag = new ReportInput { Name = "CancelFlag", InputType = "Select", Value = "N", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        ReportInput taskType = new ReportInput { Name = "TASK_NO_TYPE", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "BS", "CS" } };

        ReportInput receiveDateFrom = new ReportInput() { Name = "ReceiveDateFrom", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput receiveDateTo = new ReportInput() { Name = "ReceiveDateTo", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput planDateFrom = new ReportInput() { Name = "PlanDateFrom", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput planDateTo = new ReportInput() { Name = "PlanDateTo", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public R7B5PlanTaskReport()
        {           
            Inputs.Add(taskNo);
            Inputs.Add(item);
            Inputs.Add(model);
            Inputs.Add(cancelFlag);
            Inputs.Add(taskType);
            Inputs.Add(receiveDateFrom);
            Inputs.Add(receiveDateTo);
            Inputs.Add(planDateFrom);
            Inputs.Add(planDateTo);
            string sqlGetModel = "select 'ALL' as model from dual union select distinct model from r_7b5_xml_t order by model";
            Sqls.Add("SqlGetModel", sqlGetModel);
        }

        public override void Init()
        {
            base.Init();
            receiveDateFrom.Value = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            receiveDateTo.Value = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            planDateFrom.Value = DateTime.Now.ToString("yyyy-MM-dd");
            planDateTo.Value = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");
            model.ValueForUse = GetModel();
        }
        public override void Run()
        {
            base.Run();
            string sql = "select a.TASK_NO,a.ITEM,b.MODEL,b.QTY as TASK_QTY, a.PLAN_QTY,to_char(a.PLAN_DT,'yyyy/mm/dd') PLAN_DT,a.LASTEDITBY,a.CANCEL_FLAG,a.LASTEDITDT,b.LASTEDITDT RECEIVE_DATE from r_7b5_plan_task a,r_7b5_xml_t b where a.task_no=b.task_no  ";
            string sqlTaskNo = "", sqlItem = "", sqlCancelFlag = "", sqlTaskType = "", sqlModel = "", sqlReceiveDate = "", sqlPlanDate = "";
            if (taskNo.Value != null && !string.IsNullOrEmpty(taskNo.Value.ToString()))
            {
                sqlTaskNo = $@" and a.TASK_NO ='{taskNo.Value.ToString()}' ";
            }
            if (item.Value != null && !string.IsNullOrEmpty(item.Value.ToString()))
            {
                sqlItem = $@" and a.ITEM ='{item.Value.ToString()}' ";
            }
            if (cancelFlag.Value.ToString() != "ALL")
            {
                sqlCancelFlag = $@" and a.cancel_flag ='{cancelFlag.Value.ToString()}' ";
            }
            if (taskType.Value.ToString() == "BS")
            {
                sqlTaskType = $@"  AND LEFT(a.TASK_NO,2) = 'BS' ";
            }
            else if (taskType.Value.ToString() == "CS")
            {
                sqlTaskType = $@"  AND LEFT(a.TASK_NO,2) <> 'BS' ";
            }
            if (model.Value.ToString() != "ALL")
            {
                sqlModel = $@"  AND b.model = '{model.Value.ToString()}' ";
            }

            if (receiveDateFrom.Value != null && !string.IsNullOrEmpty(receiveDateFrom.Value.ToString())&& receiveDateTo.Value != null && !string.IsNullOrEmpty(receiveDateTo.Value.ToString()))
            {
                sqlReceiveDate = $@" AND  b.LASTEDITDT  BETWEEN TO_DATE('{receiveDateFrom.ToString()}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('{receiveDateTo.ToString()}','YYYY/MM/DD HH24:MI:SS')";
            }
            if (planDateFrom.Value != null && !string.IsNullOrEmpty(planDateFrom.Value.ToString()) && planDateTo.Value != null && !string.IsNullOrEmpty(planDateTo.Value.ToString()))
            {
                sqlPlanDate = $@" AND  a.PLAN_DT  BETWEEN TO_DATE('{planDateFrom.ToString()}','YYYY/MM/DD') AND TO_DATE('{planDateTo.ToString()}','YYYY/MM/DD')";
            }

            sql = sql + $@" {sqlTaskNo} {sqlItem} {sqlCancelFlag} {sqlTaskType} {sqlModel} {sqlReceiveDate} {sqlPlanDate} order by b.lasteditdt,a.plan_dt ";
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
                reportTable.Tittle = "R7B5PlanTask";
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

        private string[] GetModel()
        {
            List<string> listModel = new List<string>();          
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet dsSkuno = SFCDB.RunSelect(Sqls["SqlGetModel"]);
                foreach (DataRow row in dsSkuno.Tables[0].Rows)
                {
                    listModel.Add(row[0].ToString());
                }
                return listModel.ToArray();
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
