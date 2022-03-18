using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    class JuniperSOWaitShip : ReportBase
    {
        //ReportInput inputSn = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };

        public JuniperSOWaitShip()
        {
            //Inputs.Add(inputSn);
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            //string SN = inputSn.Value.ToString();
            string sqlRun = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow linkRow = null;

            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            try
            {

                sqlRun = $@"select salesordernumber,salesorderlineitem,pono,poline,unitprice,skuno,wo,Serial,Qty,CBS,CRSD,podeliverydate,EarlyShipDate,completedelivery,
                            (case when completedelivery='X' then 'SO not yet Completed'
                            else 'Ready to Pull In'
                            end) Comments,postatus
                            from (
                            select salesordernumber,salesorderlineitem,pono,poline,unitprice,qq.skuno,wo,Serial,Qty,count(distinct sn) CBS,custreqshipdate CRSD,podeliverydate,EarlyShipDate,completedelivery,postatus
                            from (
                            select sj.salesordernumber,sj.salesorderlineitem,sj.pono,sj.poline,sj.unitprice,sj.skuno,sj.wo,sj.Serial,sj.Qty,sj.podeliverydate,custreqshipdate,
                            (case 
                             when sj.podeliverydate=sj.custreqshipdate then sj.custreqshipdate                                           
                             when sj.podeliverydate<sj.custreqshipdate then sj.podeliverydate                                       
                             when sj.podeliverydate-sj.custreqshipdate<=4 then sj.custreqshipdate  
                             else sj.podeliverydate-4
                             end) EarlyShipDate,completedelivery,postatus from (
                            select ih.salesordernumber,ii.salesorderlineitem,om.pono,om.poline,om.unitprice,om.pid skuno,om.prewo wo,se.series_name Serial,sum(om.qty)Qty ,ii.podeliverydate,ii.custreqshipdate,ih.completedelivery,mj.description postatus
                            from o_i137_head ih,o_i137_item ii,o_order_main om,c_sku sk,c_series se,o_po_status ps,o_po_status_map_j mj
                            where om.pid=sk.skuno
                            and sk.c_series_id=se.id
                            and ih.tranid=ii.tranid 
                            and ii.id=om.itemid 
                            and om.id=ps.poid
                            and ps.statusid=mj.name
                            and ps.validflag='1'
                            --and sk.sku_type in('DOF','CTO')
                            and ih.salesordernumber in(
                            select salesordernumber from o_i137_head 
                            where --salesordernumber in('0015969158') and 
                            tranid in(
                            select tranid from o_i137_item 
                            where id in(
                            select itemid from o_order_main where id in(
                            select poid from o_po_status where statusid='11'AND VALIDFLAG=1)))) 
                            group by ih.salesordernumber,ii.salesorderlineitem,om.pono,om.poline,om.unitprice,om.pid,om.prewo,se.series_name,ii.podeliverydate,ii.custreqshipdate,ih.completedelivery,mj.description)sj)qq
                            left join r_sn_station_detail sd on qq.wo=sd.workorderno and sd.station_name in('CBS','CBS2')
                            group by salesordernumber,salesorderlineitem,pono,poline,unitprice,qq.skuno,wo,Serial,Qty,podeliverydate,EarlyShipDate,completedelivery,custreqshipdate,postatus)jk
                            where (COMPLETEDELIVERY !='X' and Qty=CBS) OR COMPLETEDELIVERY ='X'
                            order by salesordernumber,salesorderlineitem";
                RunSqls.Add(sqlRun);
                snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                linkTable.Columns.Add("SALESORDERNUMBER");
                linkTable.Columns.Add("SALESORDERLINEITEM");
                linkTable.Columns.Add("PONO");
                linkTable.Columns.Add("POLINE");
                linkTable.Columns.Add("UNITPRICE");
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("WO");
                linkTable.Columns.Add("SERIAL");
                linkTable.Columns.Add("QTY");
                linkTable.Columns.Add("CBS");
                linkTable.Columns.Add("CRSD");
                linkTable.Columns.Add("PODELIVERYDATE");
                linkTable.Columns.Add("EARLYSHIPDATE");
                linkTable.Columns.Add("COMPLETEDELIVERY");
                linkTable.Columns.Add("COMMENTS");
                linkTable.Columns.Add("POSTATUS");
                for (int i = 0; i < snListTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SALESORDERNUMBER"] = "";
                    linkRow["SALESORDERLINEITEM"] = "";
                    linkRow["PONO"] = "";
                    linkRow["POLINE"] = "";
                    linkRow["UNITPRICE"] = "";
                    linkRow["SKUNO"] = "";
                    linkRow["WO"] = "";
                    linkRow["SERIAL"] = "";
                    linkRow["QTY"] = "";
                    linkRow["CBS"] = "";
                    linkRow["CRSD"] = "";
                    linkRow["PODELIVERYDATE"] = "";
                    linkRow["EARLYSHIPDATE"] = "";
                    linkRow["COMPLETEDELIVERY"] = "";
                    linkRow["COMMENTS"] = "";
                    linkRow["POSTATUS"] = "";
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(snListTable, linkTable);
                reportTable.Tittle = "SO LIST";
                //reportTable.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);

            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
