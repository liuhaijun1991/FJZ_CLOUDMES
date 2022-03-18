using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BPD
{
    class ShipReport:ReportBase
    {
        ReportInput DN = new ReportInput { Name = "DN", InputType = "TXT", EnterSubmit = false, Value = "" };
        ReportInput ShipDate = new ReportInput { Name = "ShipDate", InputType = "TXT", EnterSubmit = false, Value = DateTime.Now.ToString("yyyy/MM/dd") };
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", EnterSubmit = false, Value = "" };
        OleExec SFCDB = null;

        public override void Init()
        {
            this.Inputs.Add(DN);
            this.Inputs.Add(ShipDate);
            this.Inputs.Add(SN);
        }

        public override void Run()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            //if (DN.Value.ToString().Equals(""))
            //{
            //    ReportAlart alert = new ReportAlart("請輸入 DN 單號");
            //    Outputs.Add(alert);
            //    return;
            //}

            DateTime ShipTime = new DateTime();
            if (!ShipDate.Value.ToString().Equals(""))
            {
                if (!DateTime.TryParse(ShipDate.Value.ToString(), out ShipTime))
                {
                    ReportAlart alert = new ReportAlart("請輸入正確的時間格式");
                    Outputs.Add(alert);
                    return;
                }
            }

            string linkURL = "/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.ShipDetailReport&RunFlag=1&DN=";
            var ShipTotalTable = SFCDB.ORM.Queryable<R_SHIP_DETAIL>()
                .WhereIF(!DN.Value.ToString().Equals(""),t=>t.DN_NO==DN.Value.ToString().Trim())
                .WhereIF(!ShipDate.Value.ToString().Equals(""),$@"to_char(shipdate,'YYYY/MM/DD')='{ShipDate.Value.ToString()}'")
                .WhereIF(!SN.Value.ToString().Equals(""),$@"dn_no||dn_line in (select dn_no||dn_line from r_ship_detail where sn='{SN.Value.ToString()}' and rownum=1)")
                .GroupBy("dn_no,dn_line,skuno,to_char(shipdate,'YYYY/MM/DD'),createby")
                .Select("dn_no,dn_line,skuno,count(sn) ship_qty,to_char(shipdate,'YYYY/MM/DD') shipdate,createby shipby")
                .ToDataTable();
            if (ShipTotalTable.Rows.Count == 0)
            {
                ReportAlart alert = new ReportAlart("無數據！");
                Outputs.Add(alert);
                return;
            }

            ReportTable ShipTotalReport = new ReportTable();
            foreach (DataColumn dc in ShipTotalTable.Columns)
            {
                ShipTotalReport.ColNames.Add(dc.ColumnName);
            }

            TableRowView RowView = null;
            TableColView ColView = null;
            foreach (DataRow dr in ShipTotalTable.Rows)
            {
                RowView = new TableRowView();
                foreach (DataColumn dc in ShipTotalTable.Columns)
                {
                    ColView = new TableColView()
                    {
                        ColName = dc.ColumnName,
                        Value = dr[dc.ColumnName].ToString()
                    };
                    if (dc.ColumnName.ToUpper().Equals("SHIP_QTY"))
                    {
                        ColView.LinkType = "Link";
                        ColView.LinkData = linkURL + dr["DN_NO"]+"&DN_LINE="+dr["DN_LINE"];
                    }
                    RowView.RowData.Add(ColView.ColName, ColView);
                }
                ShipTotalReport.Rows.Add(RowView);
            }
            ShipTotalReport.Tittle = "出貨總覽";
            this.Outputs.Add(ShipTotalReport);
        }
    }

    class ShipDetailReport : ReportBase
    {
        ReportInput DN = new ReportInput { Name = "DN", InputType = "TXT", EnterSubmit = false, Value = "" };
        ReportInput DN_LINE = new ReportInput() { Name = "DN_LINE", InputType = "TXT", EnterSubmit = false, Value = "" };
        OleExec SFCDB = null;

        public override void Init()
        {
            this.Inputs.Add(DN);
        }

        public override void Run()
        {
            string ColumnName = string.Empty;
            List<object> datas = new List<object>();

            SFCDB = DBPools["SFCDB"].Borrow();
            var ShipDetail = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_WO_BASE, R_SN_PACKING, R_PACKING, R_PACKING>
                ((rsd, rs, rwb, rsp, rp1, rp2) => rsd.SN == rs.SN && rs.WORKORDERNO == rwb.WORKORDERNO && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                .WhereIF(!DN.Value.ToString().Equals(""),rsd=>rsd.DN_NO==DN.Value.ToString())
                .WhereIF(!DN_LINE.Value.ToString().Equals(""), rsd => rsd.DN_LINE == DN_LINE.Value.ToString())
                .Select("rs.sn,rs.workorderno,rwb.skuno,rwb.sku_ver,rp1.pack_no carton,rp2.pack_no pallet")
                .ToDataTable();

            ReportTable ShipDetailTable = new ReportTable();
            ShipDetailTable.Tittle = "出貨明細";
            ShipDetailTable.LoadData(ShipDetail);
            this.Outputs.Add(ShipDetailTable);

            #region 饼状图
            var ShipPack = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_SN_PACKING, R_PACKING>((rsd, rs, rsp, rp) => rsd.SN == rs.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                .WhereIF(!DN.Value.ToString().Equals(""), rsd => rsd.DN_NO.Equals(DN.Value.ToString()))
                .WhereIF(!DN_LINE.Value.ToString().Equals(""), rsd => rsd.DN_LINE.Equals(DN_LINE.Value.ToString()))
                .GroupBy("rp.pack_no")
                .Select("rp.pack_no carton,count(rsd.sn) count")
                .ToDataTable();

            if (ShipPack.Rows.Count > 0)
            {
                pieChart pc = new pieChart();
                pc.ChartTitle = "出貨產品Carton分佈圖";
                pc.Tittle = "出貨產品Carton分佈";
                List<ChartData> _ChartDatas = new List<ChartData>();
                ChartData _ChartData = new ChartData();
                _ChartData.name = "Carton分佈";
                _ChartData.type = ChartType.pie.ToString();
                foreach (DataRow dr in ShipPack.Rows)
                {
                    datas.Add(new List<object>() { dr["CARTON"], Convert.ToInt64(dr["COUNT"].ToString()) });
                }
                _ChartData.data = datas;
                _ChartData.colorByPoint = true;
                _ChartDatas.Add(_ChartData);
                pc.ChartDatas = _ChartDatas;
                this.Outputs.Add(pc);
            }


            #endregion

            
        }
    }
}
