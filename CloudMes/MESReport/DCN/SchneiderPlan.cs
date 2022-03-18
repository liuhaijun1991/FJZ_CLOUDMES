using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    public class SchneiderPlan : ReportBase
    {
        ReportInput inputSkuno = new ReportInput() { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "WORKORDERNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SchneiderPlan()
        {
            this.Inputs.Add(inputSkuno);
            this.Inputs.Add(inputWO);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string skuno = inputSkuno.Value.ToString();
                string wo = inputWO.Value.ToString();

                if (string.IsNullOrEmpty(skuno) && string.IsNullOrEmpty(wo))
                {
                    throw new Exception("Please input SKUNO or WORKORDERNO");
                }
                MES_DCN.Schneider.SchneiderAction scheider = new MES_DCN.Schneider.SchneiderAction();
                DataTable showTable = scheider.GetPlanTable(skuno, wo);
                DataTable linkTable = new DataTable();
                DataRow linkRow = null;
                foreach (var c in showTable.Columns)
                {
                    linkTable.Columns.Add(c.ToString());
                }
                for (int i = 0; i < showTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    foreach (var c in linkTable.Columns)
                    {

                        if (c.ToString() == "WORKORDERNO")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + showTable.Rows[i]["WORKORDERNO"].ToString() + "&CloseFlag=ALL";
                        }
                        else if (c.ToString() == "SKUNO")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SkuReport&RunFlag=1&Skuno=" + showTable.Rows[i]["SKUNO"].ToString() + "&CloseFlag=ALL&&Series=ALL";
                        }
                        else
                        {
                            linkRow[c.ToString()] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(showTable, linkTable);
                reportTable.Tittle = "PCBATrace";
                reportTable.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
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
