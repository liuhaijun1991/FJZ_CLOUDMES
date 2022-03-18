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

    public class YieldRateReport : ReportBase
    {
        ReportInput Customer = new ReportInput() { InputType = "Select", Name = "Customer", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL" } };
        ReportInput Type = new ReportInput() { InputType = "Select", Name = "Type", Value = "ALL", ValueForUse = new string[] { "ALL", "Week", "Day", "Hour" } };
        ReportInput Skuno = new ReportInput() { InputType= "Autocomplete",Name="Skuno",Value="", API= "MESStation.Config.SkuConfig.GetAllSkuno", APIPara="",RefershType=RefershTypeEnum.Default.ToString() };
        ReportInput StartDate = new ReportInput() { InputType = "TXT", Name = "StartDate", Value = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd") };
        ReportInput EndDate = new ReportInput() { InputType = "TXT", Name = "EndDate", Value = DateTime.Now.ToString("yyyy/MM/dd") };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "Autocomplete", Value = "", API= "MESStation.Config.CStationConfig.GetAllStation", APIPara="",RefershType=RefershTypeEnum.Default.ToString() };
        ReportInput Line = new ReportInput() { Name="Line",InputType = "Select", Value="ALL", Enable = true, SendChangeEvent = false, ValueForUse = new List<string> { "ALL" } };
        ReportInput Shift = new ReportInput() { InputType = "Select", Name = "Shift", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "SHIFT 1", "SHIFT 2" } };
        ReportInput Wo = new ReportInput() { InputType = "TXT", Name = "Wo", Value = "" };
        ReportInput WoType = new ReportInput() { InputType = "Select", Name = "WoType", Value = "REGULAR", ValueForUse = new string[] { "ALL", "REGULAR" } };
        OleExec SFCDB = null;

        public YieldRateReport()
        {
            this.Inputs.Add(Customer);
            this.Inputs.Add(Skuno);
            this.Inputs.Add(StartDate);
            this.Inputs.Add(EndDate);
            this.Inputs.Add(Station);
            this.Inputs.Add(Wo);
            this.Inputs.Add(Shift);
            this.Inputs.Add(Line);
            this.Inputs.Add(Type);
            this.Inputs.Add(WoType);
        }

        public override void Init()
        {
            SFCDB = DBPools["SFCDB"].Borrow();
            ((List<string>)Line.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_LINE>().OrderBy(t => t.LINE_NAME).Select(t => t.LINE_NAME).ToList());

            //((List<string>)Skuno.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((csku, cseries, ccustomer) => csku.C_SERIES_ID == cseries.ID && cseries.CUSTOMER_ID == ccustomer.ID)
            //    .OrderBy((csku, cseries, ccustomer) => ccustomer.CUSTOMER_NAME).Select((csku, cseries, ccustomer) => csku.SKUNO).ToList());
            ((List<string>)Customer.ValueForUse).AddRange(SFCDB.ORM.Queryable<C_CUSTOMER>().Select(t => t.CUSTOMER_NAME.Substring(0, 1)).ToList());
        }

        public override void Run()
        {
            DataTable dt = new DataTable();
            DateTime _StartDate = new DateTime();
            DateTime _EndDate = new DateTime();        
            string Byquery = Type.Value.ToString();
            string customer = Customer.Value.ToString();
            string sku = Skuno.Value.ToString();
            string shift = Shift.Value.ToString();
            string wo = Wo.Value.ToString();
            string line = Line.Value.ToString();
            string station = Station.Value.ToString().ToUpper();
            string WoTypeValue = WoType.Value.ToString();
            String LinkUrl = "/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.YieldRateDetailReport&RunFlag=1";
            if (!DateTime.TryParse(StartDate.Value.ToString(), out _StartDate) || !DateTime.TryParse(EndDate.Value.ToString(), out _EndDate))
            {
                Outputs.Add(new ReportAlart("Date format is invalid!"));
                return;
            }
            //基本數據
           

            var result = SFCDB.ORM.Queryable<R_UPH_DETAIL, C_SKU, C_SERIES, C_CUSTOMER,R_WO_BASE>((ryrd, csku, cseries, ccustomer,rwb) => ryrd.SKUNO == csku.SKUNO && csku.C_SERIES_ID == cseries.ID && cseries.CUSTOMER_ID == ccustomer.ID && ryrd.WORKORDERNO==rwb.WORKORDERNO)
                    .WhereIF(customer != "ALL", (ryrd, csku, cseries, ccustomer) => ccustomer.CUSTOMER_NAME.StartsWith(customer))
                    .WhereIF(sku != "", (ryrd, csku, cseries, ccustomer) => ryrd.SKUNO == sku)
                    .WhereIF(shift != "ALL", (ryrd, csku, cseries, ccustomer) => ryrd.CLASS_NAME == shift)
                    .WhereIF(station != "", (ryrd, csku, cseries, ccustomer) => ryrd.STATION_NAME == station)
                    .WhereIF(wo != "", (ryrd, csku, cseries, ccustomer) => ryrd.WORKORDERNO == wo)
                    .WhereIF(line !="ALL",(ryrd,csku,cseries,ccustomer)=>ryrd.PRODUCTION_LINE==line)
                    .WhereIF(!WoTypeValue.Equals("ALL"), (ryrd, csku, cseries, ccustomer,rwb) => rwb.WO_TYPE.Equals(WoTypeValue))
                    //.Where($@"ryrd.edit_time>=to_date('{start}','yyyy-mm-dd hh24:mi:ss') and ryrd.edit_time<to_date('{end}','yyyy-mm-dd hh24:mi:ss')") //add by htz 20190306 
                    .Where($@"ryrd.work_date>=to_date('{StartDate.Value.ToString()}','YYYY/MM/DD') and ryrd.work_date<to_date('{EndDate.Value.ToString()}','YYYY/MM/DD')")
                    //.OrderBy("work_date")
                    .Select(@"to_char(work_date,'YYYY/MM/DD') work_date,
                               ryrd.skuno,
                               ryrd.work_time,
                               ryrd.workorderno,
                               ryrd.PRODUCTION_LINE,
                               station_name,
                               total_fresh_build_qty fresh_build,
                               total_fresh_pass_qty fresh_pass,
                               total_fresh_fail_qty fresh_fail,
                               CASE  SUM(TOTAL_FRESH_BUILD_QTY) WHEN 0 THEN '0' ELSE ROUND(SUM(TOTAL_FRESH_PASS_QTY)/SUM(TOTAL_FRESH_BUILD_QTY),4)*100||'%' END yield_rate");

            ReportTable retTab = new ReportTable();
            StringBuilder Condition = new StringBuilder();
            if (Byquery != "ALL")
            {
                #region 根据条件筛选 
                //by 周                  
                if (Byquery == "Week")
                {
                    dt = result.Select(@"to_char(work_date,'YYYYWW') week,ryrd.skuno,ryrd.workorderno,station_name,ryrd.PRODUCTION_LINE,sum(total_fresh_build_qty) fresh_build,sum(total_fresh_pass_qty) fresh_pass,sum(total_fresh_fail_qty) fresh_fail,
                        case sum(total_fresh_build_qty)
                                 when 0  then
                                  '0'
                                 else
                                  round(sum(total_fresh_pass_qty) /
                                        sum(total_fresh_build_qty),
                                        4) * 100 || '%'
                               end yield_rate").GroupBy("to_char(work_date,'YYYYWW'),ryrd.skuno,ryrd.workorderno,station_name,ryrd.PRODUCTION_LINE").OrderBy("to_char(work_date,'YYYYWW'),ryrd.skuno,ryrd.PRODUCTION_LINE").ToDataTable();
                }
                //by 天
                else if (Byquery == "Day")
                {
                    dt = result.Select(@"to_char(work_date,'YYYY/MM/DD') work_date,ryrd.skuno,ryrd.workorderno,station_name,ryrd.PRODUCTION_LINE,sum(total_fresh_build_qty) fresh_build,sum(total_fresh_pass_qty) fresh_pass,sum(total_fresh_fail_qty) fresh_fail,
                        case sum(total_fresh_build_qty)
                                 when 0  then
                                  '0'
                                 else
                                  round(sum(total_fresh_pass_qty) /
                                        sum(total_fresh_build_qty),
                                        4) * 100 || '%'
                               end yield_rate").GroupBy("to_char(work_date,'YYYY/MM/DD'),ryrd.skuno,ryrd.workorderno,station_name,ryrd.PRODUCTION_LINE").OrderBy("work_date,ryrd.skuno,ryrd.PRODUCTION_LINE").ToDataTable();
                }
                //by 小時
                else if (Byquery == "Hour")
                {
                    dt = result.Select(@"to_char(work_date,'YYYY/MM/DD') work_date,ryrd.skuno,work_time,ryrd.workorderno,station_name,ryrd.PRODUCTION_LINE
                               total_fresh_build_qty fresh_build,
                               total_fresh_pass_qty fresh_pass,
                               total_fresh_fail_qty fresh_fail,
                               case total_fresh_build_qty
                                 when 0  then
                                  '0'
                                 else
                                  round(total_fresh_pass_qty /
                                        total_fresh_build_qty,
                                        4) * 100 || '%'
                               end yield_rate").OrderBy("work_date,ryrd.skuno,work_time").ToDataTable();
                }
                #endregion
            }
            else
            {
                dt = result.GroupBy("work_date,ryrd.skuno,ryrd.work_time,ryrd.workorderno,station_name,ryrd.PRODUCTION_LINE,total_fresh_build_qty,total_fresh_pass_qty,total_fresh_fail_qty")
                    .OrderBy("work_date,ryrd.work_time")//,ryrd.skuno,ryrd.work_time,ryrd.production_line
                    .ToDataTable();
            }
            DataRow TotalRow = dt.NewRow();
            TotalRow[0] = "Total";
            dt.Rows.InsertAt(TotalRow, 0);

            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dr[dc.ColumnName] is DBNull )
                    {
                        if (dc.ColumnName.Equals("FRESH_BUILD") || dc.ColumnName.Equals("FRESH_PASS") || dc.ColumnName.Equals("FRESH_FAIL") || dc.ColumnName.Equals("YIELD_RATE"))
                        {
                            dr[dc.ColumnName] = "0";
                        }    
                    }
                }
            }
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.Equals("FRESH_BUILD") || dc.ColumnName.Equals("FRESH_PASS") || dc.ColumnName.Equals("FRESH_FAIL"))
                {
                    int total = 0;
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        total += Int32.Parse(dr[dc.ColumnName].ToString());
                    }
                    TotalRow[dc.ColumnName] = total;
                }
                
                if (dc.ColumnName.Equals("YIELD_RATE"))
                {
                    TotalRow[dc.ColumnName] = Math.Round((double.Parse(TotalRow["FRESH_PASS"].ToString()) / double.Parse(TotalRow["FRESH_BUILD"].ToString())), 4) * 100 + "%";
                }
            }
    
            if (dt.Rows.Count == 0)
            {
                Outputs.Add(new ReportAlart("No Data"));
                return;
            }
            retTab.Tittle = "各時段工站良率";
            foreach (DataColumn dc in dt.Columns)
            {
                retTab.ColNames.Add(dc.ColumnName);
            }
            TableRowView RowView = null;
            TableColView ColView = null;
            foreach (DataRow dr in dt.Rows)
            {
                if (!dr["FRESH_BUILD"].ToString().Equals("0"))
                {
                    RowView = new TableRowView();
                    foreach (DataColumn dc in dt.Columns)
                    {

                        ColView = new TableColView()
                        {
                            ColName = dc.ColumnName,
                            Value = dr[dc.ColumnName].ToString()
                        };
                        if (dc.ColumnName.Equals("WORK_TIME") && dr[dc.ColumnName].ToString().Length > 0)
                        {
                            ColView.Value = dr[dc.ColumnName].ToString() + ":00:00 - " + dr[dc.ColumnName].ToString() + ":59:59";
                        }

                        if ((dc.ColumnName.ToUpper().Equals("FRESH_PASS") || dc.ColumnName.ToUpper().Equals("FRESH_FAIL")) && !dr[dc.ColumnName].ToString().Equals("0"))
                        {
                            Condition.Clear();
                            ColView.LinkType = "Link";
                            if (dr[0].ToString().Equals("Total"))
                            {
                                Condition = Condition.Append("&SKUNO=" + sku).Append("&WORKORDERNO=" + wo).Append("&WO_TYPE=" + WoTypeValue).Append("&StartDate=" + StartDate.Value.ToString()).Append("&EndDate=" + EndDate.Value.ToString()).Append("&STATION_NAME=" + station).Append("&PASS=" + (dc.ColumnName.ToUpper() == "FRESH_PASS" ? "1" : "0"));
                                ColView.LinkData = LinkUrl + Condition.ToString();
                            }
                            else
                            {

                                Condition = Condition.Append("&SKUNO=" + dr["SKUNO"]).Append("&WORKORDERNO=" + dr["WORKORDERNO"]).Append("&WO_TYPE=" + WoTypeValue).Append("&STATION_NAME=" + dr["STATION_NAME"]).Append("&PRODUCTION_LINE=" + dr["PRODUCTION_LINE"]).Append("&PASS=" + (dc.ColumnName.ToUpper() == "FRESH_PASS" ? "1" : "0"));
                                if (Byquery.Equals("Week"))
                                {
                                    Condition = Condition.Append("&WEEK=" + dr["WEEK"]);
                                }
                                else if (Byquery.Equals("Day"))
                                {
                                    Condition = Condition.Append("&WORK_DATE=" + dr["WORK_DATE"]);
                                }
                                else
                                {
                                    Condition = Condition.Append("&WORK_DATE=" + dr["WORK_DATE"] + "&WORK_TIME=" + dr["WORK_TIME"]);
                                }

                                ColView.LinkData = LinkUrl + Condition.ToString();
                            }
                        }
                        RowView.RowData.Add(ColView.ColName, ColView);
                    }
                    retTab.Rows.Add(RowView);
                }
            }
            Outputs.Add(retTab);
        }
    }
    public class YieldRateDetailReport : ReportBase
    {
        ReportInput WEEK = new ReportInput() { Name = "WEEK", InputType = "TXT", Value = "" };
        ReportInput StartDate = new ReportInput() { Name = "StartDate", InputType = "TXT", Value = "" };
        ReportInput EndDate = new ReportInput() { Name = "EndDate", InputType = "TXT", Value = "" };
        //ReportInput STATION = new ReportInput() { Name = "STATION", InputType = "TXT", Value = "" };
        ReportInput WORK_DATE = new ReportInput() { Name = "WORK_DATE", InputType = "TXT", Value = "" };
        ReportInput WORK_TIME = new ReportInput() { Name = "WORK_TIME", InputType = "TXT", Value = "" };
        ReportInput SKUNO = new ReportInput() { InputType = "TXT", Name = "SKUNO", Value = "" };
        ReportInput WORKORDERNO = new ReportInput() { InputType = "TXT", Name = "WORKORDERNO", Value = "" };
        ReportInput STATION_NAME = new ReportInput() { Name = "STATION_NAME", InputType = "TXT", Value = "" };
        ReportInput PRODUCTION_LINE = new ReportInput() { Name = "PRODUCTION_LINE", InputType = "TXT", Value = "" };
        ReportInput PASS = new ReportInput() { Name = "PASS", InputType = "TXT", Value = "" };
        ReportInput WO_TYPE = new ReportInput() { Name = "WO_TYPE", InputType = "Select", ValueForUse = new string[]{ "ALL","REGULAR"},Value="ALL" };



        OleExec SFCDB = null;

        public override void Init()
        {
            this.Inputs.Add(WEEK);
            this.Inputs.Add(StartDate);
            this.Inputs.Add(EndDate);
            //this.Inputs.Add(STATION);
            this.Inputs.Add(WORK_DATE);
            this.Inputs.Add(WORK_TIME);
            this.Inputs.Add(SKUNO);
            this.Inputs.Add(WORKORDERNO);
            this.Inputs.Add(STATION_NAME);
            this.Inputs.Add(PRODUCTION_LINE);
            this.Inputs.Add(PASS);
            this.Inputs.Add(WO_TYPE);
        }

        public string LineMapping(string DeviceName)
        {
            switch (DeviceName.ToUpper())
            {
                case "3DX_01":
                    return "A13XRAY3DX01";
                case "3DX_02":
                    return "A13XRAY3DX02";
                case "RXI_01":
                    return "A13XRAYRXI01";
                case "RXI_02":
                    return "A13XRAYRXI02";
                case "FXSICT1":
                    return "A13ICT";
                case "S1_01":
                    return "A13ICTS101";
                case "S1_02":
                    return "A13ICTS102";
                default:
                    return DeviceName;
            }
        }

        public override void Run()
        {
            DataTable dt = null;
            DateTime WorkDate = new DateTime();
            if (WORK_DATE.Value != null && !WORK_DATE.Value.ToString().Equals(""))
            {
                if (!DateTime.TryParse(WORK_DATE.Value.ToString(), out WorkDate))
                {
                    this.Outputs.Add(new ReportAlart(""));
                    return;
                }
            }

            string WorkTime = string.Empty;
            if (WORK_TIME.Value != null && !WORK_TIME.Value.ToString().Equals(""))
            {
                WorkTime = WORK_TIME.Value.ToString().PadLeft(2, '0');
            }

            string StationName = string.Empty;
            if (STATION_NAME.Value != null && !STATION_NAME.Value.ToString().Equals(""))
            {
                StationName = STATION_NAME.Value.ToString().Trim();
            }
            //string Station_name = string.Empty;
            //if (STATION.Value != null && !STATION.Value.ToString().Equals(""))
            //{
            //    Station_name = STATION.Value.ToString().Trim();
            //}
            string WoType = WO_TYPE.Value.ToString();
            string Pass = string.Empty;
            String LinkUrl = "/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.SNWipReport&RunFlag=1";
            bool flag = false;
            SFCDB = DBPools["SFCDB"].Borrow();
            C_STATION Station = SFCDB.ORM.Queryable<C_STATION>().Where(t => t.STATION_NAME.Equals(StationName)).ToList().FirstOrDefault();
            if (Station != null)
            {
                if (Station.TYPE.ToUpper().Equals("TEST"))
                {
                    Pass = PASS.Value.ToString() == "1" ? "PASS" : "FAIL";
                    //dt = SFCDB.ORM.Queryable<R_TEST_RECORD, R_SN,R_WO_BASE>((rtr, rs,rwb) => rtr.SN == rs.SN && rs.WORKORDERNO==rwb.WORKORDERNO)
                    //    .WhereIF(WEEK.Value != null && !WEEK.Value.ToString().Equals(""), $@"to_char(rtr.EDIT_TIME,'YYYYWW')={WEEK.Value.ToString()}")
                    //    .WhereIF(WORK_DATE.Value != null && !WORK_DATE.Value.ToString().Equals(""), $@"to_char(rtr.EDIT_TIME,'YYYY/MM/DD')='{WORK_DATE.Value.ToString()}'")
                    //    .WhereIF(WORK_TIME.Value != null && !WORK_TIME.Value.ToString().Equals(""), $@"to_char(rtr.EDIT_TIME,'HH24')='{WORK_TIME.Value.ToString()}'")
                    //    .WhereIF(SKUNO.Value != null && !SKUNO.Value.ToString().Equals(""), (rtr, rs) => rs.SKUNO == SKUNO.Value.ToString())
                    //    .WhereIF(WORKORDERNO.Value != null && !WORKORDERNO.Value.ToString().Equals(""), (rtr, rs) => rs.WORKORDERNO == WORKORDERNO.Value.ToString())
                    //    .WhereIF(STATION_NAME.Value != null && !STATION_NAME.Value.ToString().Equals(""), rtr => rtr.MESSTATION == STATION_NAME.Value.ToString())
                    //    .WhereIF(PRODUCTION_LINE.Value != null && !PRODUCTION_LINE.Value.ToString().Equals(""), rtr => rtr.DEVICE == LineMapping(PRODUCTION_LINE.Value.ToString()))
                    //    .WhereIF(!WoType.Equals("ALL"),(rtr,rs,rwb)=>rwb.WO_TYPE.Equals(WoType))
                    //    .Where(rtr => rtr.STATE.Equals(Pass))
                    //    .Select(@"RTR.SN,RS.SKUNO,RS.WORKORDERNO,RTR.DEVICE,RTR.MESSTATION,RTR.EDIT_TIME")
                    //    .OrderBy("rtr.sn,rtr.edit_time")
                    //    .ToDataTable();

                    dt = SFCDB.ORM.Queryable<R_TEST_RECORD, R_SN_STATION_DETAIL, R_WO_BASE>((rtr, rssd, rwb) => rtr.SN == rssd.SN && rtr.MESSTATION == rssd.STATION_NAME && rssd.WORKORDERNO == rwb.WORKORDERNO && rtr.DEVICE==rssd.DEVICE_NAME)
                         .WhereIF(WEEK.Value != null && !WEEK.Value.ToString().Equals(""), $@"to_char(rtr.EDIT_TIME,'YYYYWW')={WEEK.Value.ToString()}")
                         .WhereIF(WORK_DATE.Value != null && !WORK_DATE.Value.ToString().Equals(""), $@"to_char(rtr.EDIT_TIME,'YYYY/MM/DD')='{WORK_DATE.Value.ToString()}'")
                         .WhereIF(WORK_TIME.Value != null && !WORK_TIME.Value.ToString().Equals(""), $@"to_char(rtr.EDIT_TIME,'HH24')='{WORK_TIME.Value.ToString()}'")
                         .WhereIF(SKUNO.Value != null && !SKUNO.Value.ToString().Equals(""), (rtr, rssd) => rssd.SKUNO == SKUNO.Value.ToString())
                         .WhereIF(WORKORDERNO.Value != null && !WORKORDERNO.Value.ToString().Equals(""), (rtr, rssd) => rssd.WORKORDERNO == WORKORDERNO.Value.ToString())
                         .WhereIF(STATION_NAME.Value != null && !STATION_NAME.Value.ToString().Equals(""), rtr => rtr.MESSTATION == STATION_NAME.Value.ToString())
                         .WhereIF(PRODUCTION_LINE.Value != null && !PRODUCTION_LINE.Value.ToString().Equals(""), rtr => rtr.DEVICE == LineMapping(PRODUCTION_LINE.Value.ToString()))
                         .WhereIF(StartDate.Value!=null && StartDate.Value.ToString().Length>0  && EndDate.Value!=null && EndDate.Value.ToString().Length>0, $@"rtr.EDIT_TIME BETWEEN TO_DATE('{StartDate.Value.ToString()}','YYYY/MM/DD') AND  TO_DATE('{EndDate.Value.ToString()}','YYYY/MM/DD')")
                         .WhereIF(WoType.Equals("REGULAR"), (rtr, rssd, rwb) => rssd.PRODUCT_STATUS.Equals("FRESH"))
                         .Where("ABS(CEIL((RSSD.EDIT_TIME-RTR.EDIT_TIME)*24*60*60))<10") //過站記錄的時間和自動測試的時間之間的差值小於 10 秒則表示是同一次測試過程
                         .Where(rtr => rtr.STATE.Equals(Pass))
                         .Select(@"RTR.SN,RSSD.SKUNO,RSSD.WORKORDERNO,RTR.DEVICE,RTR.MESSTATION,RTR.EDIT_TIME")
                         .OrderBy("rtr.sn,rtr.edit_time")
                         .ToDataTable();


                    if (dt.Rows.Count == 0)
                    {
                        flag = true;
                    }
                }
                if ((Station.TYPE.Equals("TEST") && flag) || !Station.TYPE.ToUpper().Equals("TEST"))
                {
                    Pass = PASS.Value.ToString() == "1" ? "0" : "1";
                    dt = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL,R_WO_BASE>((rssd,rwb)=>rssd.WORKORDERNO==rwb.WORKORDERNO)
                        .WhereIF(WEEK.Value != null && !WEEK.Value.ToString().Equals(""), $@"to_char(rssd.EDIT_TIME,'YYYYWW')={WEEK.Value.ToString()}")
                        .WhereIF(WORK_DATE.Value != null && !WORK_DATE.Value.ToString().Equals(""), $@"to_char(rssd.EDIT_TIME,'YYYY/MM/DD')='{WORK_DATE.Value.ToString()}'")
                        .WhereIF(WORK_TIME.Value != null && !WORK_TIME.Value.ToString().Equals(""), $@"to_char(rssd.EDIT_TIME,'HH24')='{WORK_TIME.Value.ToString()}'")
                        .WhereIF(SKUNO.Value != null && !SKUNO.Value.ToString().Equals(""), rssd => rssd.SKUNO.Equals(SKUNO.Value.ToString()))
                        .WhereIF(WORKORDERNO.Value != null && !WORKORDERNO.Value.ToString().Equals(""), rssd => rssd.WORKORDERNO.Equals(WORKORDERNO.Value.ToString()))
                        .WhereIF(STATION_NAME.Value != null && !STATION_NAME.Value.ToString().Equals(""), rssd => rssd.STATION_NAME.Equals(STATION_NAME.Value.ToString()))
                        .WhereIF(PRODUCTION_LINE.Value != null && !PRODUCTION_LINE.Value.ToString().Equals(""), rssd => rssd.LINE.Equals(PRODUCTION_LINE.Value.ToString()))
                        .WhereIF(StartDate.Value!=null && StartDate.Value.ToString().Length>0  && EndDate.Value!=null && EndDate.Value.ToString().Length>0, $@"rssd.EDIT_TIME BETWEEN TO_DATE('{StartDate.Value.ToString()}','YYYY/MM/DD') AND  TO_DATE('{EndDate.Value.ToString()}','YYYY/MM/DD')")
                        .WhereIF(WoType.Equals("REGULAR"), (rssd, rwb) => rssd.PRODUCT_STATUS.Equals("FRESH"))
                        .Where(rssd  => rssd.REPAIR_FAILED_FLAG.Equals(Pass))
                        .Select(@"rssd.SN,rssd.SKUNO,rssd.WORKORDERNO,rssd.LINE,STATION_NAME,rssd.EDIT_TIME")
                        .OrderBy("rssd.SN,rssd.EDIT_TIME")
                        .ToDataTable();

                } 
            }
            //else
            //{
            //    C_STATION Stations = SFCDB.ORM.Queryable<C_STATION>().Where(t => t.STATION_NAME.Equals(Station_name)).ToList().FirstOrDefault();
            //    if (Stations!=null && Stations.TYPE.ToUpper().Equals("TEST"))
            //    {
            //        Pass = PASS.Value.ToString() == "1" ? "PASS" : "FAIL";
            //        //dt = SFCDB.ORM.SqlQueryable<object>($@"SELECT A.SN,B.SKUNO,B.WORKORDERNO,A.DEVICE,A.MESSTATION,A.EDIT_TIME
            //        //  FROM R_TEST_RECORD A,R_SN B
            //        // WHERE A.EDIT_TIME BETWEEN TO_DATE('{StartDate.Value.ToString()}','YYYY/MM/DD') AND  TO_DATE('{EndDate.Value.ToString()}','YYYY/MM/DD') 
            //        // and A.MESSTATION='{Station_name}'
            //        // AND A.STATE='{Pass}'
            //        // AND A.SN=B.SN
            //        // AND B.WORKORDERNO={WORKORDERNO.Value.ToString()}
            //        // ORDER BY A.SN, A.EDIT_TIME").ToDataTable();

            //        //dt = SFCDB.ORM.Queryable<R_TEST_RECORD, R_SN,R_WO_BASE>((rtr, rs,rwb) => rtr.SN == rs.SN && rs.WORKORDERNO==rwb.WORKORDERNO)
            //        //    .Where($@"rtr.EDIT_TIME BETWEEN TO_DATE('{StartDate.Value.ToString()}','YYYY/MM/DD') AND  TO_DATE('{EndDate.Value.ToString()}','YYYY/MM/DD')")
            //        //    .Where((rtr,rs)=>rtr.MESSTATION==Station_name && rtr.STATE==Pass)
            //        //    .WhereIF(SKUNO.Value != null && !SKUNO.Value.ToString().Equals(""), (rtr,rs,rwb) => rwb.SKUNO.Equals(SKUNO.Value.ToString()))
            //        //    .WhereIF(WORKORDERNO.Value!=null && !WORKORDERNO.Value.ToString().Equals(""),(rtr,rs)=>rs.WORKORDERNO==WORKORDERNO.Value.ToString())
            //        //    .WhereIF(!WoType.Equals("ALL"), (rtr, rs, rwb) => rwb.WO_TYPE.Equals(WoType))
            //        //    .Select("rtr.sn,rs.skuno,rs.workorderno,rtr.device,rtr.messtation,rtr.edit_time")
            //        //    .OrderBy("rtr.sn,rtr.edit_time").ToDataTable();

            //        dt = SFCDB.ORM.Queryable<R_TEST_RECORD, R_SN_STATION_DETAIL,R_WO_BASE>((rtr, rssd,rwb) => rtr.SN == rssd.SN && rtr.MESSTATION == rssd.STATION_NAME && rtr.DEVICE==rssd.LINE && rssd.WORKORDERNO==rwb.WORKORDERNO)
            //            .Where("to_char(rtr.edit_time,'YYYYMMDD HH24')=to_char(rssd.edit_time,'YYYYMMDD HH24')")
            //            .Where($@"rtr.EDIT_TIME BETWEEN TO_DATE('{StartDate.Value.ToString()}','YYYY/MM/DD') AND  TO_DATE('{EndDate.Value.ToString()}','YYYY/MM/DD')")
            //            .WhereIF(SKUNO.Value != null && !SKUNO.Value.ToString().Equals(""), (rtr, rssd) => rssd.SKUNO.Equals(SKUNO.Value.ToString()))
            //            .WhereIF(WORKORDERNO.Value != null && !WORKORDERNO.Value.ToString().Equals(""), (rtr, rssd) => rssd.WORKORDERNO == WORKORDERNO.Value.ToString())
            //            .WhereIF(WoType.Equals("REGULAR"), (rtr, rssd,rwb) =>rssd.PRODUCT_STATUS=="FRESH")
            //            .Where(rtr=>rtr.MESSTATION==Station_name && rtr.STATE==Pass)
            //            .Select("rtr.sn,rssd.skuno,rssd.workorderno,rtr.device,rtr.messtation,rtr.edit_time")
            //            .OrderBy("rtr.sn,rtr.edit_time")
            //            .ToDataTable();
            //    }
            //    else
            //    {
            //        Pass = PASS.Value.ToString() == "1" ? "0" : "1";
            //        //dt = SFCDB.ORM.SqlQueryable<object>($@"SELECT SN, SKUNO, WORKORDERNO, LINE, STATION_NAME, EDIT_TIME
            //        //  FROM R_SN_STATION_DETAIL
            //        // WHERE EDIT_TIME BETWEEN TO_DATE('{StartDate.Value.ToString()}','YYYY/MM/DD') AND  TO_DATE('{EndDate.Value.ToString()}','YYYY/MM/DD') 
            //        // and station_name='{Station_name}'
            //        // AND REPAIR_FAILED_FLAG='{Pass}'
            //        // ORDER BY SN, EDIT_TIME").ToDataTable();
            //        dt = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL,R_WO_BASE>((rssd,rwb)=>rssd.WORKORDERNO==rwb.WORKORDERNO)
            //            .Where($@"rssd.EDIT_TIME BETWEEN TO_DATE('{StartDate.Value.ToString()}','YYYY/MM/DD') AND  TO_DATE('{EndDate.Value.ToString()}','YYYY/MM/DD') ")
            //            .Where(rssd => rssd.STATION_NAME == Station_name)
            //            .Where(rssd => rssd.REPAIR_FAILED_FLAG == Pass)
            //            .WhereIF(SKUNO.Value != null && !SKUNO.Value.ToString().Equals(""), rssd => rssd.SKUNO.Equals(SKUNO.Value.ToString()))
            //            .WhereIF(WORKORDERNO.Value != null && !WORKORDERNO.Value.ToString().Equals(""), rssd => rssd.WORKORDERNO == WORKORDERNO.Value.ToString())
            //            .WhereIF(!WoType.Equals("ALL"), (rssd, rwb) => rwb.WO_TYPE.Equals(WoType))
            //            .OrderBy("rssd.sn,rssd.edit_time")
            //            .Select("rssd.SN, rssd.SKUNO, rssd.WORKORDERNO, rssd.LINE, rssd.STATION_NAME, rssd.EDIT_TIME").ToDataTable();
            //    }
            //}
            ReportTable retTab = new ReportTable();
            StringBuilder Condition = new StringBuilder();
            retTab.Tittle = "良率明細";
            foreach (DataColumn dc in dt.Columns)
            {
                retTab.ColNames.Add(dc.ColumnName);
            }
            TableRowView RowView = null;
            TableColView ColView = null;
            foreach (DataRow dr in dt.Rows)
            {
                RowView = new TableRowView();
                foreach (DataColumn dc in dt.Columns)
                {
                    ColView = new TableColView()
                    {
                        ColName = dc.ColumnName,
                        Value = dr[dc.ColumnName].ToString()
                    };
                    if (dc.ColumnName.ToUpper().Equals("SN"))
                    {
                        Condition.Clear();
                        ColView.LinkType = "Link";
                        Condition = Condition.Append("&SN=" + dr["SN"]);
                        ColView.LinkData = LinkUrl + Condition.ToString();
                    }
                    RowView.RowData.Add(ColView.ColName, ColView);
                }
                retTab.Rows.Add(RowView);
            }
            Outputs.Add(retTab);
        }
    }
}
