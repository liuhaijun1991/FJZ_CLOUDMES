using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class DNReport: ReportBase
    {
        ReportInput inputDN = new ReportInput { Name = "DN_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputPO = new ReportInput { Name = "PO_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputTO = new ReportInput { Name = "TO_NO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public DNReport()
        {
            this.Inputs.Add(inputDN);
            Inputs.Add(inputPO);
            Inputs.Add(inputTO);
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
                string dn = inputDN.Value.ToString();
                string po = inputPO.Value.ToString();
                string to = inputTO.Value.ToString();
                string sqlDN = "",sqlPO="",sqlTO="";

                if (string.IsNullOrWhiteSpace(dn) && string.IsNullOrWhiteSpace(po) && string.IsNullOrWhiteSpace(to))
                {
                    throw new Exception("Please input DN_NO or PO_NO or TO_NO.");
                }
                if(!string.IsNullOrWhiteSpace(dn))
                {
                    sqlDN = $@" and d.dn_no = '{dn}' ";
                }
                if (!string.IsNullOrWhiteSpace(po))
                {
                    sqlPO = $@" and d.po_no = '{po}' ";                   
                }
                if (!string.IsNullOrWhiteSpace(to))
                {
                    sqlTO = $@" and t.to_no = '{to}' ";                   
                }
                string sql = $@"select dd.DN_NO,
                               dd.DN_LINE,
                               dd.PO_NO,
                               dd.PO_LINE,
                               dd.SO_NO,
                               dd.SKUNO,
                               dd.QTY,
                               count(s.sn) as SHIPPED_QTY,
                               dd.GTTYPE,
                               dd.GT_STATUS,
                               dd.GTDATE,
                               dd.DN_STATUS,
                               dd.DN_PLANT,
                               dd.TO_NO,
                               dd.TO_ITEM_NO,
                               dd.DN_CUSTOMER,
                               dd.CREATETIME,
                               dd.EDITTIME       
                          from (select d.dn_no,
                                       d.dn_line,
                                       d.po_no,
                                       d.po_line,
                                       d.so_no,
                                       d.skuno,
                                       d.qty,
                                       d.gttype,
                                       decode(d.gt_flag,
                                              '0',
                                              'WAIT GT',
                                              '1',
                                              'GT FINISH',
                                              '2',
                                              'LOCKGT') GT_STATUS,
                                       GTDATE,
                                       decode(d.DN_FLAG,
                                              '0',
                                              'WAIT SHIP',
                                              '1',
                                              'WAIT CQA',
                                              '2',
                                              'WAIT GT',
                                              '3',
                                              'GT FINISH') DN_STATUS,
                                       d.dn_plant,
                                       t.to_no,
                                       t.to_item_no,
                                       t.dn_customer,
                                       d.createtime,
                                       d.edittime
                                  from r_dn_status d, r_to_detail t
                                 where d.dn_no = t.dn_no {sqlDN} {sqlPO} {sqlTO}
                                   ) dd
                          left join r_ship_detail s
                            on dd.dn_no = s.dn_no
                            group by dd.DN_NO,
                               dd.DN_LINE,
                               dd.PO_NO,
                               dd.PO_LINE,
                               dd.SO_NO,
                               dd.SKUNO,
                               dd.QTY,      
                               dd.GTTYPE,
                               dd.GT_STATUS,
                               dd.GTDATE,
                               dd.DN_STATUS,
                               dd.DN_PLANT,
                               dd.TO_NO,
                               dd.TO_ITEM_NO,
                               dd.DN_CUSTOMER,
                               dd.CREATETIME,
                               dd.EDITTIME";

                DataTable dt = SFCDB.ORM.Ado.GetDataTable(sql);
                if(dt.Rows.Count==0)
                {
                    throw new Exception("NO Data.");
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt, null);
                retTab.Tittle = "DNReport";
                Outputs.Add(retTab);
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
