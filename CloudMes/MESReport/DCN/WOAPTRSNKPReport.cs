using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;

namespace MESReport.DCN
{
    public class WOAPTRSNKPReport : ReportBase
    {

        ReportInput inputData = new ReportInput { Name = "WO", InputType = "TextArea", Value = "002618000713\002618000714", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public WOAPTRSNKPReport()
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
                ReportAlart alart = new ReportAlart("Please input WO need query");
                Outputs.Add(alart);
                return;
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                runSql = $@"select a.workorderno,
                                   a.skuno,
                                   c.station,
                                   c.kp_partno,
                                   d.location,
                                   e.mpn,
                                   e.mfrcode,
                                   f.tr_sn,
                                   f.qty,
                                   g.mfr_code,
                                   g.MFR_KP_NO,
                                   g.ext_qty
                              from r_wo_base a
                             inner join c_kp_list b
                                on a.skuno = b.skuno
                               and b.flag = 1
                             inner join c_kp_list_item c
                                on b.id = c.list_id
                             inner join c_kp_list_item_detail d
                                on c.id = d.item_id
                              left join C_SKU_MPN e
                                on e.skuno = b.skuno
                               and c.kp_partno = e.partno
                              left join mes4.r_kitting_scan_detail@vndcnap f
                                on f.FROM_LOCATION = a.workorderno
                               and f.CUST_KP_NO = c.kp_partno
                               and f.MOVE_TYPE = 'c'
                              left join mes4.r_tr_sn@vndcnap g
                                on g.tr_sn = f.tr_sn
                             where a.workorderno in ({indata})
                               and d.scantype = 'APTRSN'
                             order by a.workorderno, a.skuno, c.station, c.kp_partno";
                dt = sfcdb.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "WO APTRSN Kp Report";
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

