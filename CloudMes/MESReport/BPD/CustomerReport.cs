using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BPD
{
    class CustomerReport : ReportBase
    {
        OleExec SFCDB = null;  
        ReportInput Customer = new ReportInput() { Name = "Customer", InputType = "Select", Enable = true, EnterSubmit = false, SendChangeEvent = false, Value = "", ValueForUse=new List<string> { ""} };
        ReportInput StartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime", Enable = true, EnterSubmit = false, SendChangeEvent = false,Value=DateTime.Now.ToString("yyyy-MM-dd") };
        ReportInput EndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime", Enable = true, EnterSubmit = false, SendChangeEvent = false, Value = DateTime.Now.ToString("yyyy-MM-dd") };
        ReportInput Skuno = new ReportInput() { Name = "Skuno", InputType = "Autocomplete", Value = "", API = "MESStation.Config.SkuConfig.GetAllSkuno", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };
        ReportInput Workorder = new ReportInput() { Name = "Workorder", InputType = "TXT", EnterSubmit = false, Enable = true, SendChangeEvent = false, Value = "" };

        public CustomerReport()
        {
            this.Inputs.Add(Customer);
            this.Inputs.Add(StartDate);
            this.Inputs.Add(EndDate);
            this.Inputs.Add(Skuno);
            this.Inputs.Add(Workorder);
        }

        public override void Init()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            ((List<string>)Customer.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_CUSTOMER>().Select(t => t.CUSTOMER_NAME.Substring(0, 1)).ToList());
            base.Init();
        }

        public override void Run()
        {
            string CustomerStr = Customer.Value.ToString();
            if (CustomerStr.Length == 0)
            {
                Outputs.Add(new ReportAlart("請選擇客戶"));
                return;
            }
            string LinkUrl = "/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.SnDetailReport&RunFlag=1";
            bool valid = false;
            DateTime tempDatetime;
            DataTable dt = null;
            if (StartDate != null && StartDate.Value.ToString().Length > 0)
            {
                valid = DateTime.TryParse(StartDate.Value.ToString(), out tempDatetime);
                if (!valid)
                {
                    Outputs.Add(new ReportAlart("請輸入正確的日期格式"));
                    return;
                }
            }
            if (EndDate != null && EndDate.Value.ToString().Length > 0)
            {
                valid = DateTime.TryParse(EndDate.Value.ToString(), out tempDatetime);
                if (!valid)
                {
                    Outputs.Add(new ReportAlart("請輸入正確的日期格式"));
                    return;
                }
            }

            var SqlSugarQueryResult = SFCDB.ORM.Queryable<R_UPH_DETAIL, C_SKU, C_SERIES, C_CUSTOMER,R_SKU_ROUTE,C_ROUTE_DETAIL>(
                (rud, csku, cseries, ccustomer,rsr,crd) => new object[] {
                    SqlSugar.JoinType.Inner,rud.SKUNO == csku.SKUNO,
                    SqlSugar.JoinType.Inner,csku.C_SERIES_ID == cseries.ID,
                    SqlSugar.JoinType.Inner,cseries.CUSTOMER_ID == ccustomer.ID,
                    SqlSugar.JoinType.Left,csku.ID==rsr.SKU_ID,
                    SqlSugar.JoinType.Inner,rsr.ROUTE_ID==crd.ROUTE_ID,
                    SqlSugar.JoinType.Left,rud.STATION_NAME==crd.STATION_NAME
                })
                    .Where((rud, csku, cseries, ccustomer) => ccustomer.CUSTOMER_NAME.StartsWith(CustomerStr))
                    .WhereIF(StartDate != null && StartDate.Value.ToString().Length > 0, $@"rud.work_date>=to_date('{((DateTime)StartDate.Value).ToString("yyyy/MM/dd")}','YYYY/MM/DD')")
                    .WhereIF(EndDate != null && EndDate.Value.ToString().Length > 0, $@"rud.work_date<to_date('{((DateTime)EndDate.Value).ToString("yyyy/MM/dd")}','YYYY/MM/DD')");
            if (CustomerStr.Equals("A"))
            {
                dt = SqlSugarQueryResult.WhereIF(Workorder != null && Workorder.Value.ToString().Length > 0, rud => rud.WORKORDERNO == Workorder.Value.ToString())
                    .WhereIF(Skuno != null && Skuno.Value.ToString().Length > 0, rud => rud.SKUNO == Skuno.Value.ToString())
                    .Where((rud, csku, cseries, ccustomer, rsr, crd)=>rud.STATION_NAME==crd.STATION_NAME)
                    .Select(@"distinct rud.workorderno,crd.seq_no,rud.station_name,sum(rud.total_fresh_build_qty) fresh_build,sum(rud.total_fresh_pass_qty) fresh_pass,sum(rud.total_fresh_fail_qty) fresh_fail,
                            case sum(rud.total_fresh_build_qty)
                                 when 0  then
                                  '0'
                                 else
                                  round(sum(rud.total_fresh_pass_qty) /
                                        sum(rud.total_fresh_build_qty),
                                        4) * 100 || '%'
                               end yield_rate")
                    .GroupBy("rud.workorderno,crd.seq_no,rud.station_name")
                    .OrderBy("rud.workorderno,crd.seq_no")
                    .ToDataTable();
            }
            else
            {
                dt = SqlSugarQueryResult.WhereIF(Skuno!=null && Skuno.Value.ToString().Length>0,rud=>rud.SKUNO==Skuno.Value.ToString())
                    .Where((rud, csku, cseries, ccustomer, rsr, crd)=>rud.STATION_NAME==crd.STATION_NAME)
                    .Select(@"distinct rud.skuno,crd.seq_no,rud.station_name,sum(rud.total_fresh_build_qty) fresh_build,sum(rud.total_fresh_pass_qty) fresh_pass,sum(rud.total_fresh_fail_qty) fresh_fail,
                            case sum(rud.total_fresh_build_qty)
                                 when 0  then
                                  '0'
                                 else
                                  round(sum(rud.total_fresh_pass_qty) /
                                        sum(rud.total_fresh_build_qty),
                                        4) * 100 || '%'
                               end yield_rate")
                    .GroupBy("rud.skuno,crd.seq_no,rud.station_name")
                    .OrderBy("rud.skuno,crd.seq_no")
                    .ToDataTable(); ;
            }
            

            ReportTable rt = new ReportTable();
            rt.Tittle = CustomerStr+" 客戶報表";
            foreach (DataColumn dc in dt.Columns)
            {
                rt.ColNames.Add(dc.ColumnName);
            }
            TableRowView RowView = new TableRowView();
            TableColView ColView = new TableColView();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["FRESH_PASS"].ToString().Equals("0"))
                {
                    continue;
                }
                RowView = new TableRowView();
                foreach (DataColumn dc in dt.Columns)
                {
                    ColView = new TableColView()
                    {
                        ColName = dc.ColumnName,
                        Value = dr[dc.ColumnName].ToString()
                    };
                    if (dc.ColumnName.Equals("FRESH_FAIL") && !dr[dc.ColumnName].ToString().Equals("0"))
                    {
                        ColView.LinkType = "Link";
                        if (CustomerStr.Equals("A"))
                        {
                            ColView.LinkData = LinkUrl + "&Workorder="+dr["WORKORDERNO"]+"&Station="+dr["STATION_NAME"];
                        }
                        else 
                        {
                            ColView.LinkData = LinkUrl + "&Skuno="+dr["SKUNO"]+ "&Station=" + dr["STATION_NAME"];
                        }
                        ColView.LinkData += "&StartDate=" + ((DateTime)StartDate.Value).ToString("yyyy/MM/dd") + "&EndDate=" + ((DateTime)EndDate.Value).ToString("yyyy/MM/dd");
                    }
                    RowView.RowData.Add(ColView.ColName, ColView);
                }
                rt.Rows.Add(RowView);
            }
            this.Outputs.Add(rt);



        }
    }

    class SnDetailReport : ReportBase
    {
        OleExec SFCDB = null;
        ReportInput StartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime", Enable = true, EnterSubmit = false, SendChangeEvent = false };
        ReportInput EndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime", Enable = true, EnterSubmit = false, SendChangeEvent = false };
        ReportInput Skuno = new ReportInput() { Name = "Skuno", InputType = "Autocomplete", Value = "", API = "MESStation.Config.SkuConfig.GetAllSkuno", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };
        ReportInput Workorder = new ReportInput() { Name = "Workorder", InputType = "TXT", EnterSubmit = false, Enable = true, SendChangeEvent = false, Value = "" };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "TXT", Enable = true, EnterSubmit = false, SendChangeEvent = false, Value = "" };

        public SnDetailReport()
        {
            this.Inputs.Add(StartDate);
            this.Inputs.Add(EndDate);
            this.Inputs.Add(Skuno);
            this.Inputs.Add(Workorder);
            this.Inputs.Add(Station);
        }

        public override void Run()
        {
            DataTable dt = null;
            bool flag = false;
            SFCDB = DBPools["SFCDB"].Borrow();
            C_STATION CStation = SFCDB.ORM.Queryable<C_STATION>().Where(t => t.STATION_NAME == Station.Value.ToString()).ToList().FirstOrDefault();
            if (CStation != null)
            {
                if (CStation.TYPE.Equals("TEST"))
                {
                    dt = SFCDB.ORM.Queryable<R_TEST_RECORD, R_SN>((rtr, rs) => rtr.SN == rs.SN)
                        .WhereIF(Skuno != null && Skuno.Value.ToString().Length > 0, (rtr, rs) => rs.SKUNO == Skuno.Value.ToString())
                        .WhereIF(Workorder != null && Workorder.Value.ToString().Length > 0, (rtr, rs) => rs.WORKORDERNO == Workorder.Value.ToString())
                        .Where(rtr => rtr.MESSTATION == Station.Value.ToString() && rtr.STATE == "FAIL")
                        .Where($@"rtr.edit_time between to_date('{((DateTime)StartDate.Value).ToString("yyyy/MM/dd")}','YYYY/MM/DD') and to_date('{((DateTime)EndDate.Value).ToString("yyyy/MM/dd")}','YYYY/MM/DD')")
                        .Select("distinct to_char(rtr.edit_time,'YYYY/MM/DD') work_date,rs.skuno,rs.workorderno,rs.sn,rtr.messtation,rtr.device,rtr.edit_time,rtr.edit_emp")
                        .OrderBy("rs.skuno,rs.workorderno,rtr.messtation,edit_time")
                        .ToDataTable();
                    if (dt.Rows.Count == 0)
                    {
                        flag = true;
                    }
                }
                if((CStation.TYPE.Equals("TEST") && flag)||!CStation.TYPE.Equals("TEST"))
                {
                    dt = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL, R_REPAIR_MAIN, R_REPAIR_FAILCODE, R_REPAIR_ACTION, C_ERROR_CODE, C_ACTION_CODE>
                        ((rssd, rrm, rrf, rra, cec, cac) => new object[] {
                            SqlSugar.JoinType.Inner,rssd.SN == rrm.SN,
                            SqlSugar.JoinType.Inner,rrm.ID == rrf.MAIN_ID,
                            SqlSugar.JoinType.Left,rrf.ID == rra.R_FAILCODE_ID,
                            SqlSugar.JoinType.Inner,rrf.FAIL_CODE == cec.ERROR_CODE,
                            SqlSugar.JoinType.Left,rra.ACTION_CODE == cac.ACTION_CODE })
                        .Where((rssd, rrm, rrf, rra, cec, cac) => rssd.REPAIR_FAILED_FLAG == "1" && rssd.PRODUCT_STATUS == "FRESH")
                        .WhereIF(Skuno != null && Skuno.Value.ToString().Length > 0, rssd => rssd.SKUNO == Skuno.Value.ToString())
                        .WhereIF(Workorder != null && Workorder.Value.ToString().Length > 0, rssd => rssd.WORKORDERNO == Workorder.Value.ToString())
                        .Where($@"rssd.edit_time>= to_date('{((DateTime)StartDate.Value).ToString("yyyy/MM/dd")}','YYYY/MM/DD') and rssd.edit_time<to_date('{((DateTime)EndDate.Value).ToString("yyyy/MM/dd")}','YYYY/MM/DD')")
                        .Where((rssd, rrm) => rrm.FAIL_STATION == Station.Value.ToString())
                        .Select(@"distinct to_char(rssd.edit_time,'YYYY/MM/DD') work_date,rssd.skuno,rssd.workorderno,rssd.sn,rssd.station_name,rssd.plant,rssd.class_name,rssd.line,rssd.edit_time,rssd.edit_emp,
                        decode(rrm.closed_flag,'1','Close','0','Open') Status,rrf.fail_code,cec.chinese_description,rrf.fail_location,rrf.description,decode(rrf.repair_flag,'1','Repaired','0','Repairing') process,
                        rra.action_code,cac.chinese_description,rra.repair_emp,rra.repair_time,rra.section_id,rra.process,rra.description,rra.kp_no,rra.tr_sn,rra.mfr_code,rra.mfr_name,rra.date_code,rra.lot_code")
                        .OrderBy("rssd.skuno,rssd.workorderno,rssd.station_name,rssd.edit_time")
                        .ToDataTable();
                }
            }

            ReportTable rt = new ReportTable();
            rt.Tittle = "SN 詳細記錄";
            rt.LoadData(dt);
            this.Outputs.Add(rt);
            
        }
    }
}
