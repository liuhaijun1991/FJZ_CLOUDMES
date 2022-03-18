using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;

namespace MESReport.BaseReport
{
    class WoStationReport : ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "002520000001", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false,
            ValueForUse = new string[] { "ALL", "SMT1", "HIPOT.GND", "FINALTEST", "P-BOX", "BFT2", "MAC", "FQC2", "FCA", "SMTLOADING",
                "PRESS-FIT2", "STOCKIN", "OBA", "ESS", "BURNIN", "RUNIN", "SHIPOUT", "CARTON", "PRESS-FIT1", "VI", "PACKOUT", "PFT",
                "MC1", "MA1", "CBS", "FT1", "MC", "MC2", "WEIGHT-CHECK", "ICT", "BFT", "PRESS-FIT", "5DX", "OBA2", "SMT2", "FT", "MA2",
                "PGTEST", "FQC1", "MINI-LTT", "BFT1", "FQA2", "SNAKE-TEST", "SILOADING", "PRE-ASSY", "FT2", "PRE-BURNIN", "GLUE", "ST" } };
        public WoStationReport()
        {
            Inputs.Add(WO);
            Inputs.Add(Station);
            string GetWoSattion = @"select sn,a.skuno,a.workorderno,a.plant,current_station,next_station,started_flag,start_time,completed_flag,
                                completed_time,packed_flag,shipped_flag,shipdate,repair_failed_flag,
                                scraped_flag,scraped_time,product_status,rework_count,valid_flag,a.edit_time
                                from r_sn_station_detail a,r_wo_base b,c_route_detail c
                                where (a.workorderno='{0}') and (current_station='{1}') 
                                and a.workorderno=b.workorderno(+)
                                and b.ROUTE_ID=c.ROUTE_ID
                                and a.current_station=c.station_name(+)";
           
            Sqls.Add("GetWoSattion", GetWoSattion);
        }
        public override void Run()
        {
            if (WO.Value == null)
            {
                throw new Exception("WO Can not be null");
            }
            string runSql = string.Format(Sqls["GetWoSattion"], WO.Value.ToString(), Station.Value.ToString());
            RunSqls.Add(runSql);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet res = SFCDB.RunSelect(runSql);
                ReportTable retTab = new ReportTable();
                DataTable dt = res.Tables[0].Copy();
                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&=ALL&WO=" + row["workorderno"].ToString();
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "WORKORDERNO")
                        {
                            linkRow[dc.ColumnName] = linkURL;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }
                

                retTab.LoadData(res.Tables[0], linkTable);

                retTab.Tittle = "WODETAIL";
                retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                ReportTable retTab1 = new ReportTable();
                retTab1.LoadData(res.Tables[0], null);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception)
            {
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }

    }
}

