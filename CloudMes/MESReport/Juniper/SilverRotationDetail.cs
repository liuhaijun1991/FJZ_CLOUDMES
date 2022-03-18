using MESDataObject;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class SilverRotationDetail : ReportBase
    {
        ReportInput inputSKUNO = new ReportInput { Name = "SKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCSKUNO = new ReportInput { Name = "CSKUNO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCWO = new ReportInput { Name = "CWO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputCSN = new ReportInput { Name = "CSN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SilverRotationDetail()
        {
            this.Inputs.Add(inputSKUNO);
            this.Inputs.Add(inputCSKUNO);
            this.Inputs.Add(inputWO);
            this.Inputs.Add(inputCWO);
            this.Inputs.Add(inputSN);
            this.Inputs.Add(inputCSN);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = DBPools["SFCDB"].Borrow();
                DataTable dt = GetData(sfcdb);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "Silver Rotation Detail Report";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {                
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public override void DownFile()
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = DBPools["SFCDB"].Borrow();
                DataTable dt = GetData(sfcdb);
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "Silver_Rotation_Detail_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                Outputs.Add(new ReportFile(fileName, content));
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        private DataTable GetData(OleExec sfcdb)
        {
            sfcdb = DBPools["SFCDB"].Borrow();
            string skuno = inputSKUNO.Value.ToString();
            string cskuno = inputCSKUNO.Value.ToString();
            string wo = inputWO.Value.ToString();
            string cwo = inputCWO.Value.ToString();
            string sn = inputSN.Value.ToString();
            string csn = inputCSN.Value.ToString();

            string sql = $@"select a.skuno,
                                   a.workorderno as wo,
                                   a.sn,
                                   b.skuno        as cskuno,
                                   b.workorderno  as cwo,
                                   b.sn           as csn,
                                   c.station_name,
                                   c.seqno,
                                   c.starttime,
                                   c.startby,
                                   c.endtime,
                                   c.endby
                              from r_sn a, r_sn b, r_silver_rotation_detail c
                             where a.sn = c.sn
                               and b.sn = c.csn
                               and a.valid_flag = '1'
                               and b.valid_flag = '1' ";
            if (!string.IsNullOrEmpty(skuno))
            {
                sql = sql + $@" and a.skuno='{skuno}'";
            }
            if (!string.IsNullOrEmpty(cskuno))
            {
                sql = sql + $@" and b.skuno='{cskuno}'";
            }
            if (!string.IsNullOrEmpty(wo))
            {
                sql = sql + $@" and a.workorderno='{wo}'";
            }
            if (!string.IsNullOrEmpty(cwo))
            {
                sql = sql + $@" and b.workorderno='{cwo}'";
            }
            if (!string.IsNullOrEmpty(sn))
            {
                sql = sql + $@" and c.sn='{sn}'";
            }
            if (!string.IsNullOrEmpty(csn))
            {
                sql = sql + $@" and c.csn='{csn}'";
            }
            sql = sql + $@" order by c.sn,c.csn,c.seqno,c.endtime desc";
            return sfcdb.ORM.Ado.GetDataTable(sql);
        }
    }
}
