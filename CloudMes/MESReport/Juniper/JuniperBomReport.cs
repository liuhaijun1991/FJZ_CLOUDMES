using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;

namespace MESReport.Juniper
{
    public class JuniperBomReport : ReportBase
    {

        ReportInput inputData = new ReportInput { Name = "PO_Or_WO", InputType = "TextArea", Value = "4500000001\n4500000002", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public JuniperBomReport()
        {
            Inputs.Add(inputData);
        }
        public override void Init()
        {
            base.Init();                       
        }

        public override void Run()
        {
            //base.Run();
            DataTable dt = new DataTable();
            string runSql = "";

            string indata = inputData.Value.ToString().Trim();
            indata = $@"'{indata.Replace("\n", "',\n'")}'";
            if (indata == null || indata == "''")
            {
                ReportAlart alart = new ReportAlart("Please input PO or WO need query");
                Outputs.Add(alart);
                return;
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                runSql = $@"select a.pono,
                                   a.poline,
                                   a.pid,
                                   c.groupid,
                                   b.wo,
                                   a.qty as WOQty,
                                   a.qty * nvl(b.requestqty, 0) as WORequestQty,
                                   b.partno,
                                   b.requestqty,
                                   b.unitprice,
                                   b.unitweight,
                                   b.packageflag,
                                   b.partnotype,
                                   b.createtime
                              from o_order_main a
                              left join r_pre_wo_detail b
                                on a.prewo = b.wo
                              left join r_wo_groupid c
                                on a.prewo = c.wo
                             where a.pono in ({indata})
                                or a.prewo in ({indata})
                             order by a.pono, a.poline, a.pid, b.wo, b.partno";
                dt = sfcdb.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "Juniper Bom Detail";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }
}
