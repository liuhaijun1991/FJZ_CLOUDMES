using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.HWT
{
    public class PalletStatus_Report : MESReport.ReportBase
    {

        ReportInput inputPN = new ReportInput { Name = "PACK_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public PalletStatus_Report()
        {
            this.Inputs.Add(inputPN);
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            //base.Run();
            //string sqlPN = "";
            string sqlRun = "";
            string packno = inputPN.Value.ToString();

            //string linkUrl = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.HWT.PalletStatusDetail&RunFlag=1";


            if (packno == "")
            {
                sqlRun = $@" SELECT pack_no pallet_No ,edit_time,skuno,max_qty,qty,decode(closed_flag,'1','Yes','0','No') as Closed,edit_emp 
                fROM R_PACKING where pack_type='PALLET' and rownum<20 order by edit_time";
            }
            else if (packno.Substring(0, 1) == "P")
            {
                sqlRun = $@"select a.pack_No pallet_No , b.pack_no carton_no,b.edit_time,b.skuno,b.max_qty,b.qty,decode(b.closed_flag,'1','Yes','0','No') as Closed,b.edit_emp 
                            From r_packing a, r_packing b where a.pack_no='{packno}' and a.id=b.parent_pack_id";
            }
            else if (packno.Substring(0, 1) == "C")
            {
                sqlRun = $@"select b.pack_No pallet_No , a.pack_no carton_no,a.edit_time,a.skuno,a.max_qty,a.qty,
                         decode(a.closed_flag,'1','Yes','0','No') as Closed,a.edit_emp From r_packing a, r_packing b 
                         where a.pack_no='{packno}' and a.parent_pack_id=b.id";
            }
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = new DataTable();
                DataTable linkTable = new DataTable();
                dt = sfcdb.RunSelect(sqlRun).Tables[0];
                ReportTable reportTable = new ReportTable();
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }

                reportTable.LoadData(dt, null);
                reportTable.Tittle = "Pallet Status Report";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
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