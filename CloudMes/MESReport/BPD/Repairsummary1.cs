using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;

namespace MESReport.BPD
{
    class Repairsummary1:ReportBase
    {
        OleExec SFCDB = null;
        ReportInput StartTime = new ReportInput { Name = "StartTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput { Name = "EndTime", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput Skuno = new ReportInput { Name = "Skuno", InputType = "TXT", Value = "", Enable = true, ValueForUse = null };
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, ValueForUse = null };
        ReportInput Station = new ReportInput { Name = "Station", InputType = "Select", Value = "", Enable = true, ValueForUse = null };

        public Repairsummary1()
        {
            this.Inputs.Add(StartTime);
            this.Inputs.Add(EndTime);
            this.Inputs.Add(Skuno);
            this.Inputs.Add(WO);
            this.Inputs.Add(Station);
        }

        public override void Init()
        {
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                List<object> stations = new List<object> { "" };
                stations.AddRange(
                       SFCDB.ORM.Queryable<C_ROUTE_DETAIL, R_WO_BASE>((a, b) => a.ROUTE_ID == b.ROUTE_ID)
                      .WhereIF(Skuno.Value.ToString() != "", (a, b) => b.SKUNO == Skuno.Value.ToString())
                      .WhereIF(Skuno.Value.ToString() != "", (a, b) => b.WORKORDERNO == WO.Value.ToString())
                      .Select(@"a.station_name").ToList()
                      );
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            
        }

        public override void Run()
        {
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable SumarryTable =new DataTable(); 
                ReportTable retTable= new ReportTable();
                DataRow linkRow= SumarryTable.NewRow();

                SumarryTable.Columns.Clear();
                SumarryTable.Columns.Add("FAIL");
                SumarryTable.Columns.Add("CHECKIN");
                SumarryTable.Columns.Add("REPAIRING");
                SumarryTable.Columns.Add("REPAIRED");
                SumarryTable.Columns.Add("INCHECKOUT");
                SumarryTable.Columns.Add("CHECKOUT");
                DataTable dt = SumarryTable.Clone();

                //dt.Rows[0][0] = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t=>t.REPAIR_FAILED_FLAG=="1").Select(@" Count(1) ") ;
                //dt.Rows[0][1] = SFCDB.ORM.Queryable<R_REPAIR_TRANSFER>().Select(@" Count(1) ");
                //dt.Rows[0][2] = SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(t => t.REPAIR_FLAG=="0") .Select(@" Count(1) ");
                //dt.Rows[0][3] = SFCDB.ORM.Queryable<r_repair_action>().Select(@" Count(1) ");
                //dt.Rows[0][4] = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.REPAIR_FAILED_FLAG == "1").Select(@" Count(1) ");
                //dt.Rows[0][5] = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.REPAIR_FAILED_FLAG == "1").Select(@" Count(1) ");

                foreach (DataRow dr in SumarryTable.Rows)
                {
                    dr["fail"] = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.REPAIR_FAILED_FLAG == "1").Select(@" Count(1) ");
                    dr["CHECKIN"] = SFCDB.ORM.Queryable<R_REPAIR_TRANSFER>().Select(@" Count(1) ");
                    dr["REPAIRING"] = SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(t => t.REPAIR_FLAG == "0").Select(@" Count(1) ");
                    dr["REPAIRED"] = SFCDB.ORM.Queryable<r_repair_action>().Select(@" Count(1) ");
                    dr["INCHECKOUT"] = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.REPAIR_FAILED_FLAG == "1").Select(@" Count(1) ");
                    dr["CHECKOUT"] = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.REPAIR_FAILED_FLAG == "1").Select(@" Count(1) ");
                }

                string linkURL1 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.DefectiveReport&RunFlag=1&=ALL&SKUNO=" + 1 + "&STATION=" +2 + "";
                string linkURL2 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.DefectiveReport&RunFlag=1&=ALL&SKUNO=" +1 + "&STATION=" +1+ "";
                string linkURL3 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.DefectiveReport&RunFlag=1&=ALL&SKUNO=" + 1 + "&STATION=" +2 + "";
                string linkURL4 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.DefectiveReport&RunFlag=1&=ALL&SKUNO=" +2 + "&STATION=" + 2+ "";
                string linkURL5 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.DefectiveReport&RunFlag=1&=ALL&SKUNO=" + 1 + "&STATION=" + 2 + "";
                string linkURL6 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.DefectiveReport&RunFlag=1&=ALL&SKUNO=" + 2+ "&STATION=" +2 + "";

                foreach (DataColumn dc in SumarryTable.Columns)
                {
                    if (dc.ColumnName.ToString().ToUpper() == "FAIL")
                    {
                        
                        linkRow[dc.ColumnName] = linkURL1;
                     
                    }
                    else if (dc.ColumnName.ToString().ToUpper() == "CHECKIN")
                    {
                        linkRow[dc.ColumnName] = linkURL2;
                    }
                    else if (dc.ColumnName.ToString().ToUpper() == "REPAIRING")
                    {
                        linkRow[dc.ColumnName] = linkURL3;
                    }
                    else if (dc.ColumnName.ToString().ToUpper() == "REPAIRED")
                    {
                        linkRow[dc.ColumnName] = linkURL4;
                    }
                    else if (dc.ColumnName.ToString().ToUpper() == "INCHECKOUT")
                    {
                        linkRow[dc.ColumnName] = linkURL5;
                    }
                    else if (dc.ColumnName.ToString().ToUpper() == "CHECKOUT")
                    {
                        linkRow[dc.ColumnName] = linkURL6;
                    }
                    else
                    {
                        linkRow[dc.ColumnName] = "";
                    }
                }

                SumarryTable.Rows.Add(linkRow);
                retTable.LoadData(dt, SumarryTable);
                retTable.Tittle = "RepairSummary Report";
                Outputs.Add(retTable);
               // DBPools["SFCDB"].Return(SFCDB);

            }
            catch (Exception e)
            {
                throw e;
            }
            //finally
            //{
            //    DBPools["SFCDB"].Return(SFCDB);
            //}
               
        }
    }
}
