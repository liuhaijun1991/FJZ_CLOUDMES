using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
   public class Touch_UpReport :MESReport.ReportBase
    {
        ReportInput inputSN = new ReportInput { Name = "PCAB_SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public Touch_UpReport()
        {
            this.Inputs.Add(inputSN);
        }
        public override void Init()
        {
            //base.Init();
        }
        public override void Run()
        {
            //base.Run();
            string sqlSN = "";
            DataTable showTable = new DataTable();
            if (inputSN.Value.ToString() != "")
            {
                sqlSN = $@" and data1='{inputSN.Value.ToString()}'";
            }
            sqlSN = $@"select data1 PCBA_SN,data2 Location,data3 Description,edittime Time,editemp EMP_NO from r_mes_ext where 1=1 {sqlSN} order by data1";
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                showTable = sfcdb.RunSelect(sqlSN).Tables[0];
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                if (showTable.Rows.Count == 0)
                {

                    throw new Exception("No data");
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(showTable, null);
                reportTable.Tittle = "Touch-Up Report";
                Outputs.Add(reportTable);
            }
            catch(Exception ex)
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);

            }

        }
    }
}
