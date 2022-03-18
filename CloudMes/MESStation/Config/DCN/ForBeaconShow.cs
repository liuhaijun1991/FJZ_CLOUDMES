using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.DCN
{
    public class ForBeaconShow : MesAPIBase
    {
        protected APIInfo _GetFpyCondition = new APIInfo()
        {
            FunctionName = "GetFpyCondition",
            Description = "獲取查詢FPY數據的條件如機種工站等",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName = "Type", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetFpyData = new APIInfo()
        {
            FunctionName = "GetFpyData",
            Description = "獲取FPY數據供BEACON顯示報表",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DateType", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DateFrom", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DateTo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SkuNo", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Station", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public ForBeaconShow()
        {
            this.Apis.Add(_GetFpyCondition.FunctionName, _GetFpyCondition);
            this.Apis.Add(_GetFpyData.FunctionName, _GetFpyData);
        }

        public void GetFpyCondition(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = string.Empty;
                string type = Data["ConditionType"].ToString();
                                
                switch (type.ToUpper())
                {
                    case "SKUNO":
                        sql = $@"select distinct skuno from c_sku where id in (
                                select sku_id from r_sku_route where route_id in (
                                select distinct route_id from c_route_detail where station_name in (select mes_station from c_temes_station_mapping)))
                                order by skuno";
                        break;
                    case "STATION":
                        sql = $@"select distinct mes_station as station_name  from c_temes_station_mapping  order by mes_station";
                        break;
                    default:
                        break;
                }

                DataTable dt = SFCDB.RunSelect(sql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception($@"Get Type:{type} Data Fail, No Data!");
                }
                StationReturn.Data = dt;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            StationReturn.Status = StationReturnStatusValue.Pass;
        }

        public void GetFpyData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string tempSql, skuSql;
                string dateType = Data["DateType"].ToString();
                string dateFrom = Data["DateFrom"].ToString();
                string dateTo = Data["DateTo"].ToString();
                string skuno = Data["SkuNo"].ToString();
                string station = Data["Station"].ToString();
                
                if (dateFrom == "" && dateTo == "")
                {
                    throw new Exception("Please Input DateFrom And DateTo!");
                }
                if (skuno.ToUpper().Equals("ALL"))
                {
                    skuSql = $@" skuno in (select distinct skuno from c_sku where id in (
                                    select sku_id from r_sku_route where route_id in (
                                    select distinct route_id from c_route_detail where station_name in (select mes_station from c_temes_station_mapping))))";
                }
                else
                {
                    skuSql = $@" skuno='{skuno}'  ";
                }
                if (station == "SMT1" || station == "SMT2")
                {
                    tempSql = $@" {skuSql}  and station_name='{station}' and edit_time between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS') ";
                }
                else
                {

                    tempSql = $@" sn in (select sn from r_sn where {skuSql} ) and messtation='{station}' and starttime between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS') ";
                }
                string runSql = "";
                switch (dateType)
                {

                    case "ByHours":
                        if (station == "SMT1" || station == "SMT2")
                        {
                            runSql = $@"select to_char(edit_time,'YYYY/MM/DD HH24') as datetime ,count(*) as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                                from r_sn_station_detail where {tempSql} group by to_char(edit_time,'YYYY/MM/DD HH24'),repair_failed_flag ORDER BY datetime";
                        }
                        else
                        {
                            runSql = $@"select to_char(starttime,'YYYY/MM/DD HH24') as datetime ,count(*) as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY/MM/DD HH24'),state ORDER BY datetime";
                        }

                        break;
                    case "ByDay":
                        if (station == "SMT1" || station == "SMT2")
                        {
                            runSql = $@"select to_char(edit_time,'YYYY/MM/DD') as datetime ,count(*) as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                                from r_sn_station_detail where {tempSql} group by to_char(edit_time,'YYYY/MM/DD'),repair_failed_flag ORDER BY datetime";
                        }
                        else
                        {
                            runSql = $@"select to_char(starttime,'YYYY/MM/DD') as datetime ,count(*) as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY/MM/DD'),state ORDER BY datetime";
                        }
                        break;
                    case "ByWeeks":
                        if (station == "SMT1" || station == "SMT2")
                        {
                            runSql = $@"select to_char(edit_time,'IW') || 'Week' as datetime ,count(*) as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status 
                                from r_sn_station_detail where {tempSql} group by to_char(edit_time,'IW'),repair_failed_flag ORDER BY datetime";
                        }
                        else
                        {
                            runSql = $@"select to_char(starttime,'IW') || 'Week' as datetime ,count(*) as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'IW'),state ORDER BY datetime";
                        }
                        break;
                    case "ByMonth":
                        if (station == "SMT1" || station == "SMT2")
                        {
                            runSql = $@"select to_char(edit_time,'YYYY/MM') as datetime ,count(*) as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                                where {tempSql} group by to_char(edit_time,'YYYY/MM'),repair_failed_flag  ORDER BY datetime";
                        }
                        else
                        {
                            runSql = $@"select to_char(starttime,'YYYY/MM') as datetime ,count(*) as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY/MM'),state ORDER BY datetime";
                        }
                        break;
                    case "ByQuarter":
                        if (station == "SMT1" || station == "SMT2")
                        {
                            runSql = $@"select to_char(edit_time,'q') || 'Quarter' as datetime ,count(*) as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                                where {tempSql} group by to_char(edit_time,'q'),repair_failed_flag  ORDER BY datetime";
                        }
                        else
                        {
                            runSql = $@"select to_char(starttime,'q') || 'Quarter' as datetime ,count(*) as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'q'),state ORDER BY datetime";
                        }
                        break;
                    case "ByYears":
                        if (station == "SMT1" || station == "SMT2")
                        {
                            runSql = $@"select to_char(edit_time,'YYYY') || 'Year' as datetime ,count(*) as qty,decode(repair_failed_flag,'0','PASS','1','FAIL') as status from r_sn_station_detail 
                                where {tempSql} group by to_char(edit_time,'YYYY'),repair_failed_flag  ORDER BY datetime";
                        }
                        else
                        {
                            runSql = $@"select to_char(starttime,'YYYY') || 'Year' as datetime ,count(*) as qty,decode(state,'P','PASS','F','FAIL',state) as status 
                                from r_test_record where {tempSql} group by to_char(starttime,'YYYY'),state ORDER BY datetime";
                        }
                        break;
                    default:
                        throw new Exception("DateType Error!");
                }

                DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("SkuNo:[" + skuno + "], Station:[" + station + "] No Data!");
                }

                //根據機種工站取得QE設置的達成率,未設置的默認98：QE謝志明要求不是所有機種達成率都是98 Edit By ZHB 2020年9月23日15:16:03
                double targetValue = 98;
                var targetSql = $@"
                select b.value
                  from r_function_control a, r_function_control_ex b
                 where a.id = b.detail_id
                   and a.functionname = 'FPYREPORT_TARGET'
                   and a.controlflag = 'Y'
                   and a.functiontype = 'NOSYSTEM'
                   and b.name = 'TARGET'
                   and a.value = '{skuno}'
                   and a.extval = '{station}'";
                var targetDt = SFCDB.RunSelect(targetSql).Tables[0];
                if (targetDt.Rows.Count > 0)
                {
                    targetValue = !string.IsNullOrEmpty(targetDt.Rows[0][0].ToString()) ? Convert.ToDouble(targetDt.Rows[0][0]) : targetValue;
                }

                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                List<FPYReportModel> listFPY = new List<FPYReportModel>();
                string date = "";
                int passQty = 0;
                int failQty = 0;

                DataTable dateTimeTable = dt.DefaultView.ToTable(true, "datetime");
                DataRow[] passRow;
                DataRow[] failRow;
                foreach (DataRow row in dateTimeTable.Rows)
                {
                    date = row["datetime"].ToString();
                    passRow = dt.Select(" status= 'PASS' and datetime='" + row["datetime"].ToString() + "'");
                    failRow = dt.Select(" status= 'FAIL' and datetime='" + row["datetime"].ToString() + "'");
                    passQty = passRow.Count() > 0 ? Convert.ToInt32(passRow[0]["QTY"].ToString()) : 0;
                    failQty = failRow.Count() > 0 ? Convert.ToInt32(failRow[0]["QTY"].ToString()) : 0;
                    listFPY.Add(new FPYReportModel(date, passQty, failQty, targetValue));
                }
                DataTable FPYTable = new DataTable();
                FPYTable.Columns.Add(new DataColumn("Item\\Date"));

                List<object> passData = new List<object>();
                List<object> failData = new List<object>();
                List<object> totalData = new List<object>();
                List<object> fpyData = new List<object>();
                List<object> goalsData = new List<object>();
                List<string> categories = new List<string>();

                foreach (FPYReportModel fpy in listFPY)
                {
                    FPYTable.Columns.Add(new DataColumn(fpy.Date));
                    passData.Add(fpy.PassQty);
                    failData.Add(fpy.FailQty);
                    totalData.Add(fpy.TotalQty);
                    categories.Add(fpy.Date);
                    fpyData.Add(fpy.FPYRate);
                    goalsData.Add(fpy.Goals);
                }

                List<ItemModel> listItem = new List<ItemModel>
                {
                    new ItemModel { Name = "TotalQty", TempName = "Total Qty(pcs)" },
                    new ItemModel { Name = "PassQty", TempName = "Pass Qty(pcs)" },
                    new ItemModel { Name = "FailQty", TempName = "Fail Qty(pcs)" },
                    new ItemModel { Name = "Goals", TempName = "Goals(%)" },
                    new ItemModel { Name = "FPYRate", TempName = "FPY Rate(%)" }
                };
                DataRow dr;
                foreach (ItemModel im in listItem)
                {
                    dr = FPYTable.NewRow();
                    dr["Item\\Date"] = im.TempName;
                    for (int j = 1; j < FPYTable.Columns.Count; j++)
                    {
                        foreach (FPYReportModel f in listFPY)
                        {
                            if (FPYTable.Columns[j].ColumnName == f.Date)
                            {
                                switch (im.Name)
                                {
                                    case "TotalQty":
                                        dr[FPYTable.Columns[j].ColumnName] = f.TotalQty.ToString();
                                        break;
                                    case "PassQty":
                                        dr[FPYTable.Columns[j].ColumnName] = f.PassQty.ToString();
                                        break;
                                    case "FailQty":
                                        dr[FPYTable.Columns[j].ColumnName] = f.FailQty.ToString();
                                        break;
                                    case "Goals":
                                        dr[FPYTable.Columns[j].ColumnName] = f.Goals.ToString();
                                        break;
                                    case "FPYRate":
                                        dr[FPYTable.Columns[j].ColumnName] = f.FPYRate;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    FPYTable.Rows.Add(dr);
                }
                StationReturn.Data = FPYTable;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            StationReturn.Status = StationReturnStatusValue.Pass;
        }

        public class FPYReportModel
        {
            public string Date { get; } = "";
            public int TotalQty { get; }
            public int PassQty { get; } = 0;
            public int FailQty { get; } = 0;
            public double Goals
            {
                //get { return 98; }  //QE謝志明要求不是所有機種達成率都是98 Edit By ZHB 2020年9月23日15: 16:03
                get;
            }
            public double FPYRate { get; }
            public double FailRate { get; }

            public FPYReportModel(string date, int passQty, int failQty, double goals)
            {
                Date = date;
                PassQty = passQty;
                FailQty = failQty;
                TotalQty = PassQty + FailQty;
                FailRate = Math.Round(Convert.ToDouble(FailQty) / Convert.ToDouble(TotalQty) * 100, 2);
                FPYRate = Math.Round(Convert.ToDouble(PassQty) / Convert.ToDouble(TotalQty) * 100, 2);
                Goals = goals;
            }
        }

        public class ItemModel
        {
            public string Name { get; set; }
            public string TempName { get; set; }
        }
    }
}
