using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject;
using MESDBHelper;

namespace MESReport.Juniper
{
    public class JuniperShipFVNReport : ReportBase
    {
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = DateTime.Now.AddDays(-1), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = DateTime.Now, Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput TONO = new ReportInput { Name = "TO_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public JuniperShipFVNReport()
        {
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
        }

        public override void Run()
        {
            DataTable dt = new DataTable();
            string runSql = "";
            string st = Convert.ToDateTime(StartTime.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
            string et = Convert.ToDateTime(EndTime.Value, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            try
            {
                //I282 返回多行的时候无法准确定义临时用 and i282.productcode = om.pid
                runSql = $@"select sn.workorderno,
                           sd.skuno skuno,
                           om.custpid CUSTPID,
                           dn.skuno GROUPID,
                           Attr.DESCRIPTION,
                           pack.pack_no,
                           count(1) QUANTITY,
                           --sn.sn R_SN_SN,
                           --sd.sn SD_SN,
                           w.weight GROSSWEIGHT,
                           kp.location COO,
                           dn.po_no,
                           i282.deliverynumber DNNO
                           --,dn.id dn_id,sd.id sd_id,sn.id sn_id,snp.id snp_id
                           --,pack.id pack_id,w.id w_id,om.id OM_ID,kp.id kp_id
                           --,om.preasn
                      from r_dn_status dn,
                           r_ship_detail sd,
                           r_sn sn,
                           r_sn_packing snp,
                           r_packing pack,
                           r_weight w,
                           o_order_main om,
                           r_sn_kp kp,
                           ( select distinct asnnumber , deliverynumber from   r_i282 where  errorcode is null and deliverynumber is not null )i282,
                           (select *
                              from o_agile_attr
                             where rowid in (select max(rowid)
                                               from o_agile_attr
                                              where DESCRIPTION is not null
                                              group by item_number)) Attr,
                           (select distinct dn_no A
                              from r_ship_detail
                             where shipdate between to_date('{st}', 'yyyy-mm-dd hh24:mi:ss') and
                                                           to_date('{et}', 'yyyy-mm-dd hh24:mi:ss')) B
                     where dn.dn_no = sd.dn_no
                           and dn.dn_line = sd.dn_line
                       and sd.sn = sn.sn
                       and sn.valid_flag = 1
                       and sn.id = snp.sn_id
                       and snp.pack_id = pack.id
                       and w.snid = pack.id
                       and sn.workorderno = om.prewo
                       and sn.id = kp.r_sn_id
                       and kp.kp_name = 'AutoKP'
                       and i282.asnnumber = om.preasn
                       --and i282.productcode = om.custpid
                       and attr.item_number = dn.skuno
                       and dn.dn_no = b.A
                       and dn.dn_flag in (1, 2, 3)
                       --and sn.workorderno = '007G00013789'
                     group by sn.workorderno,
                              sd.skuno,
                              om.custpid,
                              dn.skuno,
                              Attr.DESCRIPTION,
                              pack.pack_no,
                              w.weight,
                              kp.location,
                              dn.po_no,
                              i282.deliverynumber";

                dt = SFCDB.RunSelect(runSql).Tables[0];
                
                if (dt.Rows.Count == 0 )
                {
                    throw new Exception("NO data");
                }
                ReportTable reportTable = new ReportTable();
                //ReportTable reportTable1 = new ReportTable();
                reportTable.LoadData(dt, null);
                //reportTable1.LoadData(dt1, null);
                reportTable.Tittle = "Juniper TruckLoad TONO Data";
                //reportTable1.Tittle = "Juniper TruckLoad SHIP Data";
                //reportTable1.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);
                //Outputs.Add(reportTable1);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
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
