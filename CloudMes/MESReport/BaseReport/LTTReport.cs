using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using MESDBHelper;

namespace MESReport.BaseReport
{
    class LTTReport : ReportBase
    {
        ReportInput STATUS = new ReportInput() { Name = "STATUS", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "WAIT_TEST", "ON_TEST", "OFF_TEST", "CANCEL" } };
        ReportInput SKUNO = new ReportInput() { Name = "SKUNO", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public LTTReport()
        {
            Inputs.Add(STATUS);
            Inputs.Add(SKUNO);
        }
        public override void Run()
        {
            string _status = STATUS.Value?.ToString();
            string _skuno = SKUNO.Value?.ToString();
            string sql = string.Empty;
            string flag = string.Empty;
            if (_skuno == "")
            {
                throw new Exception("Pls input skuno！");

            }
            else
            {
                switch (_status)
                {
                    
                    case "WAIT_TEST":
                        flag = "0";
                        break;
                    case "ON_TEST":
                        flag = "1";
                        break;
                    case "OFF_TEST":
                        flag = "2";
                        break;
                    case "CANCEL":
                        flag = "3";
                        break;
                    default:
                        break;


                }
                
            }
            sql = $"select aa.skuno SKU,aa.workorderno WO,aa.sn SN,aa.status STATUS,aa.createtime CREATETIME," +
                    $"bb.skuno TOPSKU, bb.workorderno TOPWO, bb.SN TOPSN, bb.next_station STATION from(" +
                    $"select b.skuno, b.workorderno, a.sn, a.station, a.createtime, 'WAIT_TEST' status" +
                    $"from r_llt a, r_sn b where a.r_sn_id = b.id and b.skuno = '" + _skuno + "' and a.status = '" + flag + "')aa" +
                    $"left join(select a.value, a.SN, b.skuno, b.workorderno, b.next_station from r_sn_kp a, r_sn b" +
                    $"where a.value = b.sn)bb on aa.sn = bb.value";

            StringBuilder condi = new StringBuilder(sql);
          
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            DataTable dt = sfcdb.RunSelect(condi.ToString()).Tables[0];
            DataTable linkTable = new DataTable();
            DataRow linkRow = null;
            if (sfcdb != null)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            if (dt.Rows.Count > 0)
            {
                linkTable.Columns.Add("SKU");
                linkTable.Columns.Add("WO");
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("STATUS");
                linkTable.Columns.Add("CREATETIME");
                linkTable.Columns.Add("TOPSKU");
                linkTable.Columns.Add("TOPWO");
                linkTable.Columns.Add("TOPSN");
                linkTable.Columns.Add("STATION");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SKU"] = "";
                    linkRow["WO"] = "";
                    linkRow["SN"] = "";
                    linkRow["STATUS"] = "";
                    linkRow["CREATETIME"] = "";
                    linkRow["TOPSKU"] = "";
                    linkRow["TOPWO"] = "";
                    linkRow["TOPSN"] = "";
                    linkRow["STATION"] = "";
                    linkTable.Rows.Add(linkRow);
                }
            }

            ReportTable retTab = new ReportTable();
            retTab.LoadData(dt, linkTable);
            retTab.Tittle = "OBA REPORT";
            Outputs.Add(retTab);
        }
    }
}
