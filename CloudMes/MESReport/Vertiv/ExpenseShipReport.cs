using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Vertiv
{
    public class ExpenseShipReport : ReportBase
    {
        ReportInput ExpenseNo = new ReportInput() { Name = "ExpenseNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput PackNo = new ReportInput() { Name = "PackNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public ExpenseShipReport()
        {
            Inputs.Add(ExpenseNo);
            Inputs.Add(PackNo);
            Inputs.Add(SN);
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            //base.Run();
            string expenseNo = ExpenseNo.Value.ToString();
            string packNo = PackNo.Value.ToString();
            string sn = SN.Value.ToString();
            
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sql = string.Empty;
                sql = $@"
                    select a.dn_no           expense_no,
                           e.pack_no         pallet,
                           d.pack_no         carton,
                           a.sn,
                           b.current_station,
                           b.next_station,
                           a.skuno,
                           b.workorderno,
                           a.shipdate,
                           a.createby
                      from r_ship_detail a, r_sn b, r_sn_packing c, r_packing d, r_packing e
                     where a.sn = b.sn
                       and b.valid_flag = 1
                       and b.id = c.sn_id
                       and c.pack_id = d.id
                       and d.parent_pack_id = e.id
                       Sql_ExpenseNo
                       Sql_PackNo
                       Sql_SN
                     order by a.shipdate, e.pack_no, d.pack_no, a.sn";
                if (!string.IsNullOrEmpty(expenseNo))
                {
                    sql = sql.Replace("Sql_ExpenseNo", $@" and a.dn_no = '{expenseNo}' ");
                }
                else
                {
                    sql = sql.Replace("Sql_ExpenseNo", "");
                }
                if (!string.IsNullOrEmpty(packNo))
                {
                    sql = sql.Replace("Sql_PackNo", $@" and e.pack_no = '{packNo}' ");
                }
                else
                {
                    sql = sql.Replace("Sql_PackNo", "");
                }
                if (!string.IsNullOrEmpty(sn))
                {
                    sql = sql.Replace("Sql_SN", $@" and a.sn = '{sn}' ");
                }
                else
                {
                    sql = sql.Replace("Sql_SN", "");
                }
                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportTable rt = new ReportTable();

                rt.LoadData(dt, null);
                rt.Tittle = "Expense Ship Report";
                Outputs.Add(rt);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}
