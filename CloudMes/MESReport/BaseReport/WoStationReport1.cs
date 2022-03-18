using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;

namespace MESReport.BaseReport
{
    class WoStationReport1:ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "002520000001", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Station = new ReportInput()
        {
            Name = "Station",
            InputType = "Select",
            Value = "ALL",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = new string[] { "ALL", "SMT1", "HIPOT.GND", "FINALTEST", "P-BOX", "BFT2", "MAC", "FQC2", "FCA", "SMTLOADING",
                "PRESS-FIT2", "STOCKIN", "OBA", "ESS", "BURNIN", "RUNIN", "SHIPOUT", "CARTON", "PRESS-FIT1", "VI", "PACKOUT", "PFT",
                "MC1", "MA1", "CBS", "FT1", "MC", "MC2", "WEIGHT-CHECK", "ICT", "BFT", "PRESS-FIT", "5DX", "OBA2", "SMT2", "FT", "MA2",
                "PGTEST", "FQC1", "MINI-LTT", "BFT1", "FQA2", "SNAKE-TEST", "SILOADING", "PRE-ASSY", "FT2", "PRE-BURNIN", "GLUE", "ST" }
        };
        public WoStationReport1()
        {
            Inputs.Add(WO);
            Inputs.Add(Station);
        //    string GetWoSattion = @"select sn,a.skuno,a.workorderno,a.plant,current_station,next_station,started_flag,start_time,completed_flag,
        //                        completed_time,packed_flag,shipped_flag,shipdate,repair_failed_flag,
        //                        scraped_flag,scraped_time,product_status,rework_count,valid_flag,a.edit_time
        //                        from r_sn_station_detail a,r_wo_base b,c_route_detail c
        //                        where (a.workorderno='{0}') and (current_station='{1}') 
        //                        and a.workorderno=b.workorderno(+)
        //                        and b.ROUTE_ID=c.ROUTE_ID
        //                        and a.current_station=c.station_name(+)";

        //    Sqls.Add("GetWoSattion", GetWoSattion);
        }
        public override void Init()
        {
            base.Init();
        }
        public override void Run()
        {
            string wo = WO.Value.ToString();
            string station = Station.Value.ToString();
            string GetWoSattion = $@"select sn,a.skuno,a.workorderno,a.plant,current_station,next_station,started_flag,start_time,completed_flag,
                                completed_time,packed_flag,shipped_flag,shipdate,repair_failed_flag,
                                scraped_flag,scraped_time,product_status,rework_count,valid_flag,a.edit_time
                                from r_sn_station_detail a,r_wo_base b,c_route_detail c
                                where (a.workorderno='{wo}')  
                                and a.workorderno=b.workorderno(+)
                                and b.ROUTE_ID=c.ROUTE_ID
                                and a.current_station=c.station_name(+)";
            if (WO.Value == null)
            {
                throw new Exception("WO Can not be null");
            }
            if (station != "ALL")
            {
                GetWoSattion = GetWoSattion + $@" and (current_station='{station}') ";
            }
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable runSql = new DataTable();
            try
            {
                runSql = sfcdb.RunSelect(GetWoSattion).Tables[0];
                DataSet res = SFCDB.RunSelect(GetWoSattion);
                ReportTable retTab = new ReportTable();
                DataTable dt = res.Tables[0].Copy();


                //ReportLink link;

                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&=ALL&WO=" + row["workorderno"].ToString();
                    string linkURL1 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["sn"].ToString();
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "WORKORDERNO")
                        {
                            linkRow[dc.ColumnName] = linkURL;
                            //linkRow[dc.ColumnName] = linkURL1;
                        }
                        else if (dc.ColumnName.ToString().ToUpper() == "SN")
                        {
                            //linkRow[dc.ColumnName] = linkURL;
                            linkRow[dc.ColumnName] = linkURL1;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }


                retTab.LoadData(res.Tables[0], linkTable);

                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(runSql, linkTable);
                reportTable.Tittle = "WO Station detail";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
                //return;
            }
            finally
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }
    }
}
