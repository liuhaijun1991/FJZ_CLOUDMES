using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class TEST_IN_OUT_Report : ReportBase
    {
        ReportInput inputFromDate = new ReportInput { Name = "FromDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputToDate = new ReportInput { Name = "ToDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput { Name = "Type", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] {"ALL", "MODEL", "DOF" } };
        //ReportInput inputStation = new ReportInput { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputGroupBy = new ReportInput { Name = "GroupBy", InputType = "Select", Value = "NULL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "NULL", "WORKORDERNO", "SKUNO" } };
        public List<string> stationList = new List<string>();// { "PICTEST", "RIT", "FST_SPARES", "FPCTEST", "BFT1", "BFT2", "SLT", "SWTEST", "RE_FCT2", "SCBTEST", "SAT" };
        public List<string> skuList = new List<string>();
        public override void Init()
        {
            OleExec db = null;
            try
            {
                this.Inputs.Add(inputFromDate);
                this.Inputs.Add(inputToDate);
                this.Inputs.Add(inputType);
                //this.Inputs.Add(inputStation);
                this.Inputs.Add(inputGroupBy);
                db = DBPools["SFCDB"].Borrow();
                DateTime now = db.ORM.GetDate();
                inputFromDate.Value = now.AddDays(-1).ToString("yyyy/MM/dd") + " 06:00:00";
                inputToDate.Value = now.ToString("yyyy/MM/dd") + " 05:59:59";
                //List<string> list = new List<string>() { "ALL" };
                //list.AddRange(stationList);
                //inputStation.ValueForUse = list.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
            
        }
        public override void Run()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable data = GetSearchData(SFCDB);
                ReportTable report = new ReportTable();
                report.LoadData(data, null);                
                report.Tittle = "TEST_IN_OUT";
                Dictionary<string, List<TableHeader>> dicTableHeader = new Dictionary<string, List<TableHeader>>();
                List<string> _stationList = new List<string>();
                string type = inputType.Value.ToString();
                if (type.Equals("ALL"))
                {
                    stationList = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "TEST_IN_OUT_REPORT_STATION")
                        .OrderBy(r => r.CONTROL_TYPE).OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE).ToList();
                }
                else
                {
                    stationList = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "TEST_IN_OUT_REPORT_STATION" && r.CONTROL_TYPE == type)
                        .OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE).ToList();
                }
                foreach (var s in stationList)
                {
                    var headerIn = report.TableHeaders[0].Find(t => t.title == $@"{s}/In");
                    var headerOut = report.TableHeaders[0].Find(t => t.title == $@"{s}/Out");
                    var headerDelta = report.TableHeaders[0].Find(t => t.title == $@"{s}/Delta");
                    if (headerIn != null)
                    {
                        _stationList.Add(s);
                        headerIn.colspan = 3;
                        headerIn.title = s;
                        headerIn.field = null;
                        report.TableHeaders[0].Remove(headerOut);
                        report.TableHeaders[0].Remove(headerDelta);

                        List<TableHeader> rowTowHeader = null;

                        rowTowHeader = new List<TableHeader>();
                        TableHeader tHeaderIn = new TableHeader();
                        tHeaderIn.title = "In";
                        tHeaderIn.field = $@"{s}/In";
                        rowTowHeader.Add(tHeaderIn);
                        TableHeader tHeaderOut = new TableHeader();
                        tHeaderOut.title = "Out";
                        tHeaderOut.field = $@"{s}/Out";
                        rowTowHeader.Add(tHeaderOut);

                        TableHeader tHeaderDelta = new TableHeader();
                        tHeaderDelta.title = "Delta";
                        tHeaderDelta.field = $@"{s}/Delta";
                        rowTowHeader.Add(tHeaderDelta);
                        dicTableHeader.Add(s, rowTowHeader);
                    }
                }
                //else
                //{
                //    var headerIn = report.TableHeaders[0].Find(t => t.title == station + "_In");
                //    var headerOut = report.TableHeaders[0].Find(t => t.title == station + "_Out");
                //    var headerDelta = report.TableHeaders[0].Find(t => t.title == station + "_Delta");
                //    if (headerIn != null)
                //    {
                //        _stationList.Add(station);
                //        headerIn.colspan = 3;
                //        headerIn.title = station;
                //        headerIn.field = null;
                //        report.TableHeaders[0].Remove(headerOut);
                //        report.TableHeaders[0].Remove(headerDelta);
                //        List<TableHeader> rowTowHeader = null;

                //        rowTowHeader = new List<TableHeader>();
                //        TableHeader tHeaderIn = new TableHeader();
                //        tHeaderIn.title = "In";
                //        tHeaderIn.field = station + "_In";
                //        rowTowHeader.Add(tHeaderIn);

                //        TableHeader tHeaderOut = new TableHeader();
                //        tHeaderOut.title = "Out";
                //        tHeaderOut.field = station + "_Out";
                //        rowTowHeader.Add(tHeaderOut);

                //        TableHeader tHeaderDelta = new TableHeader();
                //        tHeaderDelta.title = "Delta";
                //        tHeaderDelta.field = station + "_Delta";
                //        rowTowHeader.Add(tHeaderDelta);
                //        dicTableHeader.Add(station, rowTowHeader);
                //    }
                //}

                for (int j = 0; j < report.TableHeaders[0].Count; j++)
                {
                    if (_stationList.Contains(report.TableHeaders[0][j].title))
                    {
                        if (report.TableHeaders.Count == 1)
                        {
                            report.TableHeaders.Add(new List<TableHeader>());
                        }
                        var dicTitle = dicTableHeader[report.TableHeaders[0][j].title];
                        for (int k = 0; k < dicTitle.Count; k++)
                        {
                            report.TableHeaders[1].Add(dicTitle[k]);
                        }
                    }
                }

                if (report.TableHeaders.Count > 1)
                {
                    for (int j = 0; j < report.TableHeaders[0].Count; j++)
                    {
                        if (report.TableHeaders[0][j].colspan == 0)
                        {
                            report.TableHeaders[0][j].rowspan = 2;
                        }
                    }
                }
                Outputs.Add(report);
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.ToString()));                
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public override void DownFile()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable data = GetSearchData(SFCDB);                
                string fileName = $@"TEST_IN_OUT_From({inputFromDate.Value.ToString()})_To({inputToDate.Value.ToString()})_Type({inputType.Value.ToString()})_GroupBy({inputGroupBy.Value.ToString()})).xlsx";
                string content = MESPubLab.Common.ExcelHelp.TEST_IN_OUTExportExcel(data);
                Outputs.Add(new ReportFile(fileName, content));
            }
            catch (Exception e)
            {
                Outputs.Add(new ReportAlart(e.ToString()));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public DataTable GetSearchData(OleExec SFCDB)
        {
            DataTable showTable = new DataTable("TEST_IN_OUT");
            try
            {
                string groupBy = inputGroupBy.Value.ToString().ToUpper();
                string type = inputType.Value.ToString().ToUpper();
                DataTable inData = new DataTable();
                DataTable outData = new DataTable();
                List<string> showStation = new List<string>();
                GetInAndOutData(SFCDB,ref inData,ref outData);
                if (groupBy.Equals("WORKORDERNO")) 
                {
                    showTable.Columns.Add($@"WORKORDERNO");
                    showTable.Columns.Add($@"SKUNO");
                }
                else if(groupBy.Equals("SKUNO"))
                {
                    showTable.Columns.Add($@"SKUNO");
                }
                if (type.Equals("ALL"))
                {
                    showStation = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "TEST_IN_OUT_REPORT_STATION")
                        .OrderBy(r => r.CONTROL_DESC).Select(r => r.CONTROL_VALUE).ToList();
                }
                else
                {
                    showStation = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "TEST_IN_OUT_REPORT_STATION" && r.CONTROL_TYPE == type)
                        .OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE).ToList();
                }
                foreach (var s in showStation)
                {
                    showTable.Columns.Add($@"{s}/In");
                    showTable.Columns.Add($@"{s}/Out");
                    showTable.Columns.Add($@"{s}/Delta");
                }
                if (groupBy.Equals("WORKORDERNO"))
                {
                    List<string> inWoList = inData.AsEnumerable().Select(d => d.Field<string>("WORKORDERNO")).Distinct().ToList().OrderBy(r => r).ToList();
                    List<string> outWoList = outData.AsEnumerable().Select(d => d.Field<string>("WORKORDERNO")).Distinct().ToList().OrderBy(r => r).ToList();
                    List<string> woList = new List<string>();
                    woList.AddRange(inWoList);
                    woList.AddRange(outWoList);
                    woList = woList.Distinct().ToList();
                    foreach (var wo in woList)
                    {
                        DataRow row = showTable.NewRow();
                        string sku = "";
                        var inSkuRow = inData.Select($" WORKORDERNO='{wo}'");
                        var outSkuRow = outData.Select($" WORKORDERNO='{wo}'");

                        if (inSkuRow.Count() > outSkuRow.Count())
                        {
                            sku = inSkuRow[0]["SKUNO"].ToString();
                        }
                        else if (inSkuRow.Count() < outSkuRow.Count())
                        {
                            sku = outSkuRow[0]["SKUNO"].ToString();
                        }
                        else
                        {
                            sku = inSkuRow[0]["SKUNO"].ToString();
                        }
                        row["WORKORDERNO"] = wo;
                        row["SKUNO"] = sku;
                        
                        foreach (var s in showStation)
                        {                            
                            var inRow = inData.Select($" WORKORDERNO='{wo}' AND STATION='{s}'");
                            int inQty = 0;
                           
                            if (inRow.Length > 0)
                            {
                                inQty = Convert.ToInt32(inRow[0]["IN_QTY"].ToString());                                
                            }
                            var outRow = outData.Select($" WORKORDERNO='{wo}' AND STATION='{s}'");
                            int outQty = 0;
                            if (outRow.Length > 0)
                            {
                                outQty = Convert.ToInt32(outRow[0]["OUT_QTY"].ToString());                                
                            }
                            if (inRow.Length == 0 && outRow.Length == 0)
                            {
                                row[$"{s}/In"] = 0;
                                row[$"{s}/Out"] = 0;
                                row[$"{s}/Delta"] = inQty - outQty;
                            }
                            else
                            {
                                row[$"{s}/In"] = inQty;
                                row[$"{s}/Out"] = outQty;
                                row[$"{s}/Delta"] = inQty - outQty;
                            }
                        }                       
                        showTable.Rows.Add(row);
                    }
                }
                else if (groupBy.Equals("SKUNO"))
                {
                    List<string> inSkuList = inData.AsEnumerable().Select(d => d.Field<string>("SKUNO")).Distinct().ToList().OrderBy(r => r).ToList();
                    List<string> outSkuList = outData.AsEnumerable().Select(d => d.Field<string>("SKUNO")).Distinct().ToList().OrderBy(r => r).ToList();
                    List<string> skuList = new List<string>();
                    skuList.AddRange(inSkuList);
                    skuList.AddRange(outSkuList);
                    skuList = skuList.Distinct().ToList();
                    foreach (var sku in skuList)
                    {
                        DataRow row = showTable.NewRow();
                        row["SKUNO"] = sku;
                        foreach (var s in showStation)
                        {                            
                            var inRow = inData.Select($" SKUNO='{sku}' AND STATION='{s}'");
                            int inQty = 0;                           
                            if (inRow.Length > 0)
                            {
                                inQty = Convert.ToInt32(inRow[0]["IN_QTY"].ToString());                                
                            }
                            var outRow = outData.Select($" SKUNO='{sku}' AND STATION='{s}'");
                            int outQty = 0;
                            if (outRow.Length > 0)
                            {
                                outQty = Convert.ToInt32(outRow[0]["OUT_QTY"].ToString());                               
                            }
                            row[$"{s}/In"] = inQty;
                            row[$"{s}/Out"] = outQty;
                            row[$"{s}/Delta"] = inQty - outQty;                            
                        }
                        showTable.Rows.Add(row);
                    }
                }
                else
                {
                    DataRow row = showTable.NewRow();
                    foreach (var s in showStation)
                    {                       
                        var inRow = inData.Select($" STATION='{s}'");
                        int inQty = 0;                        
                        if (inRow.Length > 0)
                        {
                            inQty = Convert.ToInt32(inRow[0]["IN_QTY"].ToString());                           
                        }
                        var outRow = outData.Select($" STATION='{s}'");
                        int outQty = 0;
                        if (outRow.Length > 0)
                        {
                            outQty = Convert.ToInt32(outRow[0]["OUT_QTY"].ToString());                            
                        }     
                        row[$"{s}/In"] = inQty;
                        row[$"{s}/Out"] = outQty;
                        row[$"{s}/Delta"] = inQty - outQty;                        
                    }
                    showTable.Rows.Add(row);
                }

                if (groupBy.Equals("WORKORDERNO") || groupBy.Equals("SKUNO"))
                {
                    DataRow dr = showTable.NewRow();
                    dr["SKUNO"] = "Total:";
                    foreach (var c in showTable.Columns)
                    {
                        if (c.ToString() != "WORKORDERNO" && c.ToString() != "SKUNO")
                        {
                            var temp = 0;
                            for (int i = 0; i < showTable.Rows.Count; i++)
                            {
                                if (showTable.Rows[i][c.ToString()].ToString().Length > 0)
                                {
                                    temp = temp + Convert.ToInt32(showTable.Rows[i][c.ToString()]);
                                }
                            }
                            dr[c.ToString()] = temp.ToString();
                        }
                    }
                    showTable.Rows.Add(dr);
                }
                return showTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetInAndOutData(OleExec SFCDB, ref DataTable inData, ref DataTable outData)
        {
            try
            {
                DateTime fromDateTime;
                DateTime toDateTime;
                string fromValue = "";
                string toValue = "";
                string fromTitle = "";
                string toTitle = "";
                try
                {
                    if (inputFromDate.Value != null && !string.IsNullOrWhiteSpace(inputFromDate.Value.ToString()))
                    {
                        fromDateTime = Convert.ToDateTime(inputFromDate.Value);
                        fromValue = fromDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                        fromTitle = fromDateTime.ToString("MM-dd");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($@"FromDate error.{e.Message}");
                }
                try
                {
                    if (inputToDate.Value != null && !string.IsNullOrWhiteSpace(inputToDate.Value.ToString()))
                    {
                        toDateTime = Convert.ToDateTime(inputToDate.Value);
                        toValue = toDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                        toTitle = toDateTime.ToString("MM-dd");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($@"ToDate error.{e.Message}");
                }
                string type = inputType.Value.ToString();
                string groupBy = inputGroupBy.Value.ToString();
                string sqlEditTime = "";
                string sqlInTime = "";
                string sqlIn = "";
                string sqlOut = "";
                string tableName = "";
                if (fromValue != "" && toValue != "")
                {
                    sqlEditTime = $@" and edit_time between to_date('{fromValue}','yyyy/mm/dd hh24:mi:ss') and to_date('{toValue}','yyyy/mm/dd hh24:mi:ss')";
                    sqlInTime = $@" and rr.in_time between to_date('{fromValue}','yyyy/mm/dd hh24:mi:ss') and to_date('{toValue}','yyyy/mm/dd hh24:mi:ss')";
                    tableName = $@"({fromTitle}~{toTitle})";
                }
                else if (fromValue == "" && toValue != "")
                {
                    sqlEditTime = $@" and edit_time <=to_date('{toValue}','yyyy/mm/dd hh24:mi:ss')";
                    sqlInTime = $@" and rr.in_time <=to_date('{toValue}','yyyy/mm/dd hh24:mi:ss')";
                    tableName = $@"(before {toTitle})";
                }
                else if (fromValue != "" && toValue == "")
                {
                    sqlEditTime = $@" and edit_time >= to_date('{fromValue}','yyyy/mm/dd hh24:mi:ss') ";
                    sqlInTime = $@" and rr.in_time >= to_date('{fromValue}','yyyy/mm/dd hh24:mi:ss') ";
                    tableName = $@"({fromTitle}~{SFCDB.ORM.GetDate().ToString("yyyy/MM/dd")})";
                }
                else
                {
                    sqlEditTime = "";
                    sqlInTime = "";
                    tableName = $@"(before {SFCDB.ORM.GetDate().ToString("yyyy/MM/dd")})";
                }
                if (type.Equals("ALL"))
                {
                    stationList = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "TEST_IN_OUT_REPORT_STATION")
                        .OrderBy(r => r.CONTROL_TYPE).OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE).ToList();
                }
                else
                {
                    stationList = SFCDB.ORM.Queryable<C_CONTROL>().Where(r => r.CONTROL_NAME == "TEST_IN_OUT_REPORT_STATION" && r.CONTROL_TYPE == type)
                        .OrderBy(r => r.CONTROL_LEVEL).Select(r => r.CONTROL_VALUE).ToList();
                }
                string sqlStation = "";
                foreach (var s in stationList)
                {
                    sqlStation += $",'{s}'";
                }
                sqlStation = sqlStation.Length > 0 ? sqlStation.Substring(1) : sqlStation;

                if (groupBy.ToUpper().Equals("NULL"))
                {
                    #region no group by                         
                    sqlIn = $@"select  next_station as station ,count(distinct sn) as in_qty from r_sn_station_detail where next_station in ({sqlStation})
                                    and REPAIR_FAILED_FLAG=0  {sqlEditTime} 
                                    and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                    and VALID_FLAG=1 group by next_station";

                    sqlOut = $@"select station_name as station ,count(distinct sn) as out_qty
                                from( select * from r_sn_station_detail where station_name in ({sqlStation})
                                and REPAIR_FAILED_FLAG = 0   
                                and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                and VALID_FLAG=1 {sqlEditTime}
                                union
                                select * from r_sn_station_detail rs where station_name in ({sqlStation})
                                and REPAIR_FAILED_FLAG = 1  {sqlEditTime}
                                and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                and VALID_FLAG=1
                                and exists(select * from r_repair_transfer rr where rr.sn = rs.sn and rr.station_name = rs.station_name
                                and rr.closed_flag = 0 {sqlInTime}
                                )) group by station_name";

                    inData = SFCDB.ORM.Ado.GetDataTable(sqlIn);
                    outData = SFCDB.ORM.Ado.GetDataTable(sqlOut);
                    #endregion
                }
                else if (groupBy.ToUpper().Equals("WORKORDERNO"))
                {
                    #region group by workorderno
                    sqlIn = $@"select workorderno, skuno,next_station as station ,count(distinct sn) as in_qty from r_sn_station_detail where next_station in ({sqlStation})
                                    and REPAIR_FAILED_FLAG=0  {sqlEditTime}  
                                    and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                    and VALID_FLAG=1 group by workorderno,skuno,next_station";

                    sqlOut = $@"select workorderno,skuno,station_name as station ,count(distinct sn) as out_qty
                                from( select * from r_sn_station_detail where station_name in ({sqlStation})
                                and REPAIR_FAILED_FLAG = 0 {sqlEditTime} 
                                and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                and VALID_FLAG=1
                                union
                                select * from r_sn_station_detail rs where station_name in ({sqlStation})
                                and REPAIR_FAILED_FLAG = 1  {sqlEditTime}
                                and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                and VALID_FLAG=1
                                and exists(select * from r_repair_transfer rr where rr.sn = rs.sn and rr.station_name = rs.station_name
                                and rr.closed_flag = 0 {sqlInTime}
                                )) group by workorderno,skuno,station_name";

                    inData = SFCDB.ORM.Ado.GetDataTable(sqlIn);
                    outData = SFCDB.ORM.Ado.GetDataTable(sqlOut);
                    #endregion
                }
                else
                {
                    #region group by skuno

                    sqlIn = $@"select skuno,next_station as station ,count(distinct sn) as in_qty from r_sn_station_detail where next_station in ({sqlStation})
                                    and REPAIR_FAILED_FLAG=0  {sqlEditTime}                                    
                                    and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                    and VALID_FLAG=1 group by skuno,next_station  ";

                    sqlOut = $@"select skuno,station_name as station ,count(distinct sn) as out_qty
                                from( select * from r_sn_station_detail where station_name in ({sqlStation})
                                and REPAIR_FAILED_FLAG = 0 
                                and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                and VALID_FLAG=1 {sqlEditTime}
                                union
                                select * from r_sn_station_detail rs where station_name in ({sqlStation})
                                and REPAIR_FAILED_FLAG = 1  
                                and ((ascii(substr(sn,1,1))>65 and ascii(substr(sn,1,1))<97) or (ascii(substr(sn,1,1))>47 and ascii(substr(sn,1,1))<58)) 
                                and VALID_FLAG=1 {sqlEditTime}
                                and exists(select * from r_repair_transfer rr where rr.sn = rs.sn and rr.station_name = rs.station_name
                                and rr.closed_flag = 0 {sqlInTime}
                                )) group by skuno,station_name";

                    inData = SFCDB.ORM.Ado.GetDataTable(sqlIn);
                    outData = SFCDB.ORM.Ado.GetDataTable(sqlOut);
                    #endregion
                }
                //else
                //{
                //    string s = station;
                //    if (groupBy.ToUpper().Equals("NULL"))
                //    {
                //        #region no group by
                //        sqlIn = $@"select  next_station as station ,count(distinct sn) as in_qty from r_sn_station_detail where next_station='{s}'
                //                    and REPAIR_FAILED_FLAG=0  {sqlEditTime} group by next_station";

                //        sqlOut = $@"select station_name as station ,count(distinct sn) as out_qty
                //                from( select * from r_sn_station_detail where station_name='{s}'
                //                and REPAIR_FAILED_FLAG = 0 {sqlEditTime}
                //                union
                //                select * from r_sn_station_detail rs where station_name ='{s}'
                //                and REPAIR_FAILED_FLAG = 1  {sqlEditTime}
                //                and exists(select * from r_repair_transfer rr where rr.sn = rs.sn and rr.station_name = rs.station_name
                //                and rr.closed_flag = 0 {sqlInTime}
                //                )) group by station_name";

                //        inData = SFCDB.ORM.Ado.GetDataTable(sqlIn);
                //        outData = SFCDB.ORM.Ado.GetDataTable(sqlOut);
                //        #endregion
                //    }
                //    else if (groupBy.ToUpper().Equals("WORKORDERNO"))
                //    {
                //        #region group by workorderno
                //        sqlIn = $@"select workorderno,skuno, next_station as station ,count(distinct sn) as in_qty from r_sn_station_detail where next_station='{s}'
                //                    and REPAIR_FAILED_FLAG=0  {sqlEditTime} group by workorderno,skuno,next_station";

                //        //sqlOut = $@"select workorderno,skuno,station_name as station ,count(distinct sn) as out_qty
                //        //        from( select * from r_sn_station_detail where station_name='{s}'
                //        //        and REPAIR_FAILED_FLAG = 0 {sqlEditTime}
                //        //        union
                //        //        select * from r_sn_station_detail rs where station_name ='{s}'
                //        //        and REPAIR_FAILED_FLAG = 1  {sqlEditTime}
                //        //        and exists(select * from r_repair_transfer rr where rr.sn = rs.sn and rr.station_name = rs.station_name
                //        //        and rr.closed_flag = 0 {sqlInTime}
                //        //        )) group by workorderno,skuno,station_name";

                //        sqlOut = $@"select workorderno,skuno,station_name as station ,count(distinct sn) as out_qty
                //                from( select * from r_sn_station_detail where station_name='{s}'
                //                and REPAIR_FAILED_FLAG = 0 {sqlEditTime}
                //                union
                //                select * from r_sn_station_detail rs where station_name ='{s}'
                //                and REPAIR_FAILED_FLAG = 1  {sqlEditTime}
                //                and exists (select * from r_Sn rn where rs.sn=rn.sn and rn.repair_failed_flag=1)
                //                ) group by workorderno,skuno,station_name";

                //        inData = SFCDB.ORM.Ado.GetDataTable(sqlIn);
                //        outData = SFCDB.ORM.Ado.GetDataTable(sqlOut);
                //        #endregion
                //    }
                //    else
                //    {
                //        #region group by skuno
                //        sqlIn = $@"select skuno, next_station as station ,count(distinct sn) as in_qty from r_sn_station_detail where next_station='{s}'
                //                    and REPAIR_FAILED_FLAG=0  {sqlEditTime} group by skuno,next_station";

                //        sqlOut = $@"select skuno,station_name as station ,count(distinct sn) as out_qty
                //                from( select * from r_sn_station_detail where station_name='{s}'
                //                and REPAIR_FAILED_FLAG = 0 {sqlEditTime}
                //                union
                //                select * from r_sn_station_detail rs where station_name ='{s}'
                //                and REPAIR_FAILED_FLAG = 1  {sqlEditTime}
                //                and exists(select * from r_repair_transfer rr where rr.sn = rs.sn and rr.station_name = rs.station_name
                //                and rr.closed_flag = 0 {sqlInTime}
                //                )) group by skuno,station_name";

                //        inData = SFCDB.ORM.Ado.GetDataTable(sqlIn);
                //        outData = SFCDB.ORM.Ado.GetDataTable(sqlOut);
                //        #endregion
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }      
    }   
}
