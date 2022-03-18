using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    public class SchneiderPCBAMaster : ReportBase
    {
        // SerialNumber  PartNo
        ReportInput inputSerialNumber = new ReportInput() { Name = "SerialNumber", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputPartNo = new ReportInput() { Name = "PartNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
       
        public SchneiderPCBAMaster()
        {
            this.Inputs.Add(inputSerialNumber);
            this.Inputs.Add(inputPartNo);            
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
                string partno = inputPartNo.Value.ToString();
                
                if (string.IsNullOrEmpty(serialNumber) && string.IsNullOrEmpty(partno))
                {
                    throw new Exception("Please input SerialNumber or PartNo");
                }
                MES_DCN.Schneider.SchneiderAction scheider = new MES_DCN.Schneider.SchneiderAction();
                DataTable showTable = scheider.GetPCBAMaster(serialNumber, partno);
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

                        if (c.ToString() == "PCBA_SERIALNUMBER")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + showTable.Rows[i]["PCBA_SERIALNUMBER"].ToString();
                        }                        
                        else if (c.ToString() == "PCBA_PARTNO")
                        {
                            linkRow[c.ToString()] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SkuReport&RunFlag=1&Skuno=" + showTable.Rows[i]["PCBA_PARTNO"].ToString() + "&CloseFlag=ALL&&Series=ALL";
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
