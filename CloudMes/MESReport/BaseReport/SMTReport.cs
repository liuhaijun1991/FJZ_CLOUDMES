using MESDBHelper;
using System;
using System.Data;

namespace MESReport.BaseReport
{
    /// <summary>
    /// SMT生產信息報表
    /// </summary>
    public class SMTReport : ReportBase
    {
        ReportInput WO = new ReportInput()
        {
            Name = "WO",
            InputType = "TXT",
            Value = "002510032120",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        public SMTReport()
        {
            this.Inputs.Add(WO);
        }


        public override void Run()
        {
            DataTable dt = null;
            DataTable dt2 = null;
            DataTable dt3 = null;
            string workorderno = null;
            string route_id = null;
            string skuno = null;
            string release = null;
            string workorderqty = null;
            string finish_date = null;
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            OleExec apdb = DBPools["APDB"].Borrow();
            try
            {
                string logicSQL = null;
                //string baseSQL = $@"select wo.cust_order_no 工令,wo.workorderno 工單,'' 預計完工日,wo.workorder_qty 總數,wo.finished_qty 完工數,wo.skuno 機種,wo.sku_ver 版本,wo.release_date,wo.route_id,
                //        (case when wo.workorder_qty = 0 or wo.workorder_qty is null then 0 else round(nvl(wo.finished_qty, 0)*100/wo.workorder_qty, 2) end) as 完工率 
                //        from r_wo_base wo where 1=1 ";
                string baseSQL = $@"select wo.cust_order_no Task,wo.workorderno Wo,'' Estimated_completion_date,wo.workorder_qty Total,wo.finished_qty completedQty,wo.skuno Skuno,wo.sku_ver Ver,wo.release_date,wo.route_id,
                        (case when wo.workorder_qty = 0 or wo.workorder_qty is null then 0 else round(nvl(wo.finished_qty, 0)*100/wo.workorder_qty, 2) end) as Completion_rate 
                        from r_wo_base wo where 1=1 ";

                if (WO.Value == null || WO.Value.ToString() == "")
                {
                    //輸入工單為空時 查詢工單前綴為00251且沒有關閉的工單信息
                    baseSQL += $@"and wo.workorderno like '00251%' and closed_flag = '0' ";
                }
                else
                {
                    //輸入工單查詢時  查詢指定工單的信息
                    baseSQL += $@"and wo.workorderno='{WO.Value.ToString()}'";
                }
                dt = sfcdb.RunSelect(baseSQL).Tables[0];
                dt.Columns.Add("uph");
                dt.Columns.Add("OnlineTime");
                dt.Columns.Add("OnlineAlert");
                dt.Columns.Add("OfflinePoint");
                dt.Columns.Add("OfflineTime");
                dt.Columns.Add("OfflineQty");
                dt.Columns.Add("OfflineAlert");
                dt.Columns.Add("FirstP_FQCTime");
                dt.Columns.Add("LastP_FQCTime");
                dt.Columns.Add("P_FQCQty");
                dt.Columns.Add("P_FQCAlert");
                foreach (DataRow dr in dt.Rows)
                {
                    workorderno = dr["Task"].ToString();
                    route_id = dr["route_id"].ToString();
                    skuno = dr["Skuno"].ToString();
                    release = dr["release_date"].ToString();
                    workorderqty = dr["Total"].ToString();
                    finish_date = dr["Estimated_completion_date"].ToString();
                    string uph = null;
                    if (string.IsNullOrEmpty(route_id))
                    {
                        continue;
                    }
                    logicSQL = $@"select * from c_route_detail where route_id='{route_id}' and station_name in ('PRINT1','LINK') ";
                    dt2 = sfcdb.RunSelect(logicSQL).Tables[0];
                    if (dt2.Rows.Count > 1)
                    {
                        dr["uph"] = "The PE must confirm the route";
                        dr["OnlineTime"] = "N/A";
                        dr["OnlineAlert"] = "The PE must confirm the route";
                    }
                    else if (dt2.Rows.Count > 0)
                    {
                        //SMTUPH
                        #region
                        string stationName = dt2.Rows[0]["station_name"].ToString();
                        if ("PRINT1".Equals(stationName))
                        {
                            //check Allpart表中的UPH
                            logicSQL = $@"select avg(uph_qty) uph from mes1.c_pno_uph where p_no = '{skuno}'";
                            //dt3 = apdb.RunSelect(logicSQL).Tables[0];
                            //string uph = dt3.Rows[0][0].ToString();
                            uph = apdb.ExecSelectOneValue(logicSQL)?.ToString();
                            if (string.IsNullOrEmpty(uph))
                            {
                                dr["uph"] = "Configure IE in Allpart";
                            }
                            else
                            {
                                dr["uph"] = Math.Round(Convert.ToDouble(uph) * 0.95, 2).ToString();
                            }

                        }
                        else if ("LINK".Equals(stationName))
                        {
                            dr["uph"] = "No SMT process";
                        }
                        #endregion
                        //OnlineTime & OnlineAlert
                        #region
                        logicSQL = $@"select current_station,min(start_time) onlinetime from r_sn_station_detail 
                        where workorderno='{workorderno}' and current_station in ('PRINT1','LINK') group by current_station ";
                        dt3 = sfcdb.RunSelect(logicSQL).Tables[0];
                        if (dt3.Rows.Count > 0)
                        {
                            //已上線
                            string current_station = dt3.Rows[0][0].ToString();
                            string onlinetime = dt3.Rows[0][1].ToString();
                            if (!string.IsNullOrEmpty(release) && !string.IsNullOrEmpty(onlinetime))
                            {
                                DateTime Online_Time = DateTime.Parse(onlinetime);
                                DateTime Release_Time = DateTime.Parse(release);
                                if (Release_Time >= Online_Time)
                                {
                                    dr["OnlineTime"] = Online_Time;
                                    dr["OnlineAlert"] = "It has been online on time.";
                                }
                                if (Release_Time.AddHours(12) < Online_Time)
                                {
                                    TimeSpan a = Online_Time - Release_Time.AddHours(12);

                                    dr["OnlineTime"] = Online_Time;
                                    dr["OnlineAlert"] = $@"Has extended{Math.Round(a.TotalHours, 2)}Hours online";
                                }
                            }
                        }
                        else
                        {
                            //工單未上線
                            if (!string.IsNullOrEmpty(release))
                            {
                                DateTime Release_Time = DateTime.Parse(release);
                                if (DateTime.Now < Release_Time.AddHours(12))
                                {
                                    TimeSpan a = Release_Time.AddHours(12) - DateTime.Now;
                                    dr["OnlineTime"] = "N/A";
                                    dr["OnlineAlert"] = $@"Will expire after {Math.Round(a.TotalHours, 2)} hours";
                                }
                                else
                                {
                                    TimeSpan b = DateTime.Now - Release_Time.AddHours(12);
                                    dr["OnlineTime"] = "N/A";
                                    dr["OnlineAlert"] = $@"Expired{Math.Round(b.TotalHours, 2)}hours";
                                }
                            }

                        }
                        #endregion

                    }

                    //檢查'P_FQC'的前一個站
                    #region
                    logicSQL = $@"select station_name,seq_no,station_type from c_route_detail where route_id='{route_id}' and seq_no=(
                        select max(seq_no) seq_no from c_route_detail where route_id='{route_id}' 
                        and seq_no < (select seq_no from c_route_detail where route_id='{route_id}' and station_name='P_FQC') ) ";
                    dt2 = sfcdb.RunSelect(logicSQL).Tables[0];
                    if (dt2.Rows.Count > 0)
                    {
                        string station_name = dt2.Rows[0][0].ToString();
                        logicSQL = $@"select max(start_time) offlinetime,count(*) maxqty from r_sn_station_detail where workorderno='{workorderno}' and current_station='{station_name}' ";
                        dt3 = sfcdb.RunSelect(logicSQL).Tables[0];
                        if (dt3.Rows.Count > 0)
                        {
                            string OfflineTime = dt3.Rows[0][0].ToString();
                            string OfflineQty = dt3.Rows[0][1].ToString();
                            string OfflineAlert = null;
                            //完成比率判斷
                            double _workorderqty = (workorderqty == null || workorderqty == "") ? 0 : Convert.ToDouble(workorderqty);
                            double _offlineqty = (OfflineQty == null || OfflineQty == "") ? 0 : Convert.ToDouble(OfflineQty);
                            double CompleteRate = _workorderqty == 0 ? 0 : Math.Round(_offlineqty * 100 / _workorderqty, 0);
                            if (CompleteRate < 98)
                            {
                                //取PCAS SMTUPH
                                //是否存在PRINT1&LINK工站
                                if ("It has been online on time.".Equals(dr["OnlineAlert"].ToString()))
                                {
                                    //string onlineTimeStr = dr["OnlineTime"].ToString();
                                    DateTime onlineTime = Convert.ToDateTime(dr["OnlineTime"].ToString());
                                    TimeSpan diff = DateTime.Now - onlineTime;
                                    if (!string.IsNullOrEmpty(uph))
                                    {
                                        double a = _workorderqty / Convert.ToDouble(uph);
                                        double b = (DateTime.Now - onlineTime).TotalHours;
                                        if (a > b)
                                        {
                                            OfflineAlert = $@"Will expire after {Math.Round(a - b, 2)} hours";
                                        }
                                        else
                                        {
                                            OfflineAlert = $@"Expired {Math.Round(b - a, 2)} hours";
                                        }
                                    }
                                }
                                else
                                {
                                    OfflineAlert = "No SMT process";
                                }
                            }
                            else
                            {
                                OfflineAlert = "offline completed!";
                            }
                            dr["OfflinePoint"] = station_name;
                            dr["OfflineTime"] = OfflineTime;
                            dr["OfflineQty"] = _offlineqty;
                            dr["OfflineAlert"] = OfflineAlert;
                        }

                    }
                    else
                    {
                        dr["OfflinePoint"] = "N/A";
                        dr["OfflineTime"] = "N/A";
                        dr["OfflineQty"] = "N/A";
                        dr["OfflineAlert"] = "Please check the route of PE";
                    }
                    #endregion

                    //檢查'P_FQC'工站
                    #region
                    logicSQL = $@"select station_name,seq_no,station_type from c_route_detail where route_id='{route_id}' and station_name='P_FQC' ";
                    dt2 = sfcdb.RunSelect(logicSQL).Tables[0];
                    if (dt2.Rows.Count > 0)
                    {
                        logicSQL = $@"select count(*) maxqty,min(start_time) firsttime,max(start_time) lasttime from r_sn_station_detail 
                        where workorderno='{workorderno}' and current_station='P_FQC' ";
                        dt3 = sfcdb.RunSelect(logicSQL).Tables[0];
                        if (dt3.Rows.Count > 0)
                        {
                            string qty = dt3.Rows[0][0].ToString();
                            string firstTime = dt3.Rows[0][1].ToString();
                            string lastTime = dt3.Rows[0][2].ToString();
                            string alert = null;
                            //完成比率判斷
                            double _workorderqty = (workorderqty == null || workorderqty == "") ? 0 : Convert.ToDouble(workorderqty);
                            double _qty = (qty == null || qty == "") ? 0 : Convert.ToDouble(qty);
                            double CompleteRate = _workorderqty == 0 ? 0 : Math.Round(_qty * 100 / _workorderqty, 0);
                            if (CompleteRate < 98)
                            {
                                if (string.IsNullOrEmpty(finish_date))
                                {
                                    alert = "No maintenance completion date";
                                }
                                else
                                {
                                    DateTime finishTime = Convert.ToDateTime(finish_date);
                                    if (finishTime > DateTime.Now)
                                    {
                                        alert = $@"Will expire after{Math.Round((finishTime - DateTime.Now).TotalHours, 2)} hours";
                                    }
                                    else
                                    {
                                        alert = $@"Expired {Math.Round((DateTime.Now - finishTime).TotalHours, 2)} hours";
                                    }
                                }
                            }
                            else
                            {
                                alert = "Test Completed";
                            }
                            dr["FirstP_FQCTime"] = firstTime;
                            dr["LastP_FQCTime"] = lastTime;
                            dr["P_FQCQty"] = _qty;
                            dr["P_FQCAlert"] = alert;
                        }

                    }
                    else
                    {
                        dr["FirstP_FQCTime"] = "N/A";
                        dr["LastP_FQCTime"] = "N/A";
                        dr["P_FQCQty"] = "N/A";
                        dr["P_FQCAlert"] = "No SMT test process";
                    }
                    #endregion

                }

                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt, null);
                retTab.Tittle = "SMT Manufacture Report";
                retTab.ColNames.Remove("ROUTE_ID");
                Outputs.Add(retTab);

            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                if (apdb != null) DBPools["APDB"].Return(apdb);
                if (sfcdb != null) DBPools["SFCDB"].Return(sfcdb);
            }
        }

    }
}
