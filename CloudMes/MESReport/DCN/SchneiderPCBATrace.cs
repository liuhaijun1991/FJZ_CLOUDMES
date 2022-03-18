using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    public class SchneiderPCBATrace : ReportBase
    {
        // SerialNumber  PCBA  OrderNo
        ReportInput inputSerialNumber = new ReportInput() { Name = "SerialNumber", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputPCBA = new ReportInput() { Name = "PCBA", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputOrderNo = new ReportInput() { Name = "OrderNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SchneiderPCBATrace()
        {
            this.Inputs.Add(inputSerialNumber);
            this.Inputs.Add(inputPCBA);
            this.Inputs.Add(inputOrderNo);
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
                string serialNumber = inputSerialNumber.Value.ToString();
                string pcba = inputPCBA.Value.ToString();
                string orderNo = inputOrderNo.Value.ToString();
               
                if (string.IsNullOrEmpty(serialNumber) && string.IsNullOrEmpty(pcba) && string.IsNullOrEmpty(orderNo))
                {
                    throw new Exception("Please input SerialNumber or PCBA or OrderNo");
                }
                MES_DCN.Schneider.SchneiderAction scheider = new MES_DCN.Schneider.SchneiderAction();
                DataTable showTable = scheider.GetPCBATrace(serialNumber, pcba, orderNo);
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

                        if (c.ToString() == "SERIALNUMBER")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + showTable.Rows[i]["SERIALNUMBER"].ToString();
                        }
                        else if (c.ToString() == "PCBA")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + showTable.Rows[i]["PCBA"].ToString();
                        }
                        else if (c.ToString() == "ORDERNO")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + showTable.Rows[i]["ORDERNO"].ToString() + "&CloseFlag=ALL";
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
