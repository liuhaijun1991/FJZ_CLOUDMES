using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="UPHReport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-05-05 </date>
    /// <summary>
    /// UPHReport
    /// </summary>
    public class UPHReport:ReportBase
    {
        ReportInput date = new ReportInput() { Name = "Date", InputType = "TXT", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput line = new ReportInput() { Name = "Line", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput skuno = new ReportInput() { Name = "Skuno", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput skuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "AHH024MBQ-A", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput shift = new ReportInput() { Name = "Shift", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };

        string sqlGetLine = "";
        string sqlGetSkuno = "";
        string sqlGetShift = "";
        List<string> lineList;
        public UPHReport()
        {
            Inputs.Add(date);
            Inputs.Add(line);
            Inputs.Add(skuno);
            Inputs.Add(shift);
            //sqlGetLine = $@"select line_name from (select 'ALL' as line_name from dual union select distinct line_name from c_line )order by line_name";
            sqlGetLine = $@" select distinct line_name from c_line order by line_name";
            //sqlGetSkuno = $@"select distinct skuno from r_uph_detail where work_date=to_date('{date.Value}','YYYY/MM/DD') order by skuno";
            //sqlGetShift = $@"select distinct class_name from r_uph_detail where work_date=to_date('{date.Value}','YYYY/MM/DD') order by class_name";
            Sqls.Add("SqlGetLine", sqlGetLine);
            //Sqls.Add("SqlGetSkuno", sqlGetSkuno);
            //Sqls.Add("SqlGetShift", sqlGetShift);

        }

        public override void Init()
        {
            //base.Init();
            date.Value = DateTime.Now.ToString("yyyy-MM-dd");
            lineList = GetLine();
            List<string> tempList = new List<string>();
            tempList.Add("ALL");
            foreach(string line in lineList)
            {
                tempList.Add(line);
            }
            line.ValueForUse = tempList;
            //skuno.ValueForUse = GetSkuno();
            shift.ValueForUse = GetShift();
            line.Change += LineChange;
        }
        


        public override void Run()
        {
            //base.Run();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                System.Globalization.DateTimeFormatInfo format = new System.Globalization.DateTimeFormatInfo();
                format.ShortDatePattern = "yyyy/MM/dd";
                DateTime runDate = Convert.ToDateTime(date.Value.ToString(), format);
                string runLine = line.Value.ToString();
                string runSkuno = skuno.Value.ToString();
                string runShift = shift.Value.ToString();                
                string runSqlUPH = $@"select uph.production_line,uph.class_name,uph.work_time,uph.station_name,(sum(uph.total_fresh_build_qty)+sum(uph.total_rework_build_qty)) as qty 
                                        from r_uph_detail uph where 1=1 ";
                string runSqlStation = $@"select detail.station_name from r_sku_route route,c_sku sku,c_route_detail detail where sku.skuno='{runSkuno}'
                                            and sku.id=route.sku_id and route.route_id=detail.route_id  order by detail.seq_no";
                string runSqlWorkClass = $@"select * from c_work_class";

                string runSqlShift1 = $@"select * from  c_work_class where name='SHIFT 1'  order by start_time";
                string runSqlShift21 = $@"select  * from c_work_class where name='SHIFT 2' and start_time>='20:00' order by start_time";
                string runSqlShift22 = $@"select  * from c_work_class where name='SHIFT 2' and start_time<'20:00' order by start_time";

                if (string.IsNullOrEmpty(runSkuno) || runSkuno == "")
                {
                    throw new Exception("Please input skuno!");
                }
                runSqlUPH = runSqlUPH + $@" and uph.skuno='{runSkuno}'  ";
                if (runLine.ToUpper() != "ALL")
                {
                    runSqlUPH = runSqlUPH + $@"  and uph.production_line='{runLine}' ";
                    lineList = new List<string>();
                    lineList.Add(runLine);
                }
                else
                {
                    runSqlUPH = runSqlUPH + $@" and exists(select * from c_line cl where cl.line_name=uph.production_line) ";
                }
                if (runShift.ToUpper() != "ALL")
                {
                    runSqlUPH = runSqlUPH + $@" and uph.class_name ='{runShift}' ";                    
                }
                else
                {
                    runSqlUPH = runSqlUPH + $@" and exists(select * from c_work_class wc where uph.class_name=wc.name) ";
                }
                runSqlUPH = runSqlUPH + $@" and uph.work_date=to_date('{runDate.ToString("yyyy/MM/dd")}','yyyy/mm/dd')
                                            group by uph.work_time,uph.station_name,uph.production_line,uph.class_name
                                            order by uph.work_time,uph.station_name,uph.production_line";
                DataTable tableUPH = SFCDB.RunSelect(runSqlUPH).Tables[0];
                DataTable stationTable = SFCDB.RunSelect(runSqlStation).Tables[0];
                DataTable workClassTable = SFCDB.RunSelect(runSqlWorkClass).Tables[0];
                DataTable tableShift1 = SFCDB.RunSelect(runSqlShift1).Tables[0];
                DataTable tableShift21 = SFCDB.RunSelect(runSqlShift21).Tables[0];
                DataTable tableShift22 = SFCDB.RunSelect(runSqlShift22).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }

                if (tableUPH.Rows.Count == 0)
                {                    
                    throw new Exception("No Data!");
                }
                if (stationTable.Rows.Count == 0)
                {                    
                    throw new Exception("This shuno no station!");
                }
                if (stationTable.Rows.Count == 0)
                {                    
                    throw new Exception("Cant't setting the work class!");
                }
                DataRow[] stationRow;
                UPHObject skuUPH = new UPHObject();
                LineUPHObject lineUPH;
                SationUPH stationUPH;
                TimeUPH workTimeUPH;                
                skuUPH.Skuno = runSkuno;
                skuUPH.LineUPH = new List<LineUPHObject>();
                List<WorkClassObject> WorkClass = new List<WorkClassObject>();

                if(runShift.ToUpper() != "ALL")
                {                   
                    WorkClassObject wcObject = new WorkClassObject();
                    wcObject.WorkClassType = runShift.ToUpper();
                    wcObject.WorkTime = new List<string>();
                    if (runShift.ToUpper()== "SHIFT 1")
                    {
                        for (int i = 0; i < tableShift1.Rows.Count; i++)
                        {
                            wcObject.WorkTime.Add(tableShift1.Rows[i]["START_TIME"].ToString() + "-" + tableShift1.Rows[i]["END_TIME"].ToString());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tableShift21.Rows.Count; i++)
                        {
                            wcObject.WorkTime.Add(tableShift21.Rows[i]["START_TIME"].ToString() + "-" + tableShift21.Rows[i]["END_TIME"].ToString());
                        }
                        for (int i = 0; i < tableShift22.Rows.Count; i++)
                        {
                            wcObject.WorkTime.Add(tableShift22.Rows[i]["START_TIME"].ToString() + "-" + tableShift22.Rows[i]["END_TIME"].ToString());
                        }
                    }
                    WorkClass.Add(wcObject);
                }
                else
                {
                    WorkClassObject shift1Object = new WorkClassObject();
                    shift1Object.WorkClassType = "SHIFT1";
                    shift1Object.WorkTime = new List<string>();
                    for (int i = 0; i < tableShift1.Rows.Count; i++)
                    {
                        shift1Object.WorkTime.Add(tableShift1.Rows[i]["START_TIME"].ToString() + "-" + tableShift1.Rows[i]["END_TIME"].ToString());
                    }
                    WorkClass.Add(shift1Object);

                    WorkClassObject shift2Object = new WorkClassObject();
                    shift2Object.WorkClassType = "SHIFT2";
                    shift2Object.WorkTime = new List<string>();
                    for (int i = 0; i < tableShift21.Rows.Count; i++)
                    {
                        shift2Object.WorkTime.Add(tableShift21.Rows[i]["START_TIME"].ToString() + "-" + tableShift21.Rows[i]["END_TIME"].ToString());
                    }
                    for (int i = 0; i < tableShift22.Rows.Count; i++)
                    {
                        shift2Object.WorkTime.Add(tableShift22.Rows[i]["START_TIME"].ToString() + "-" + tableShift22.Rows[i]["END_TIME"].ToString());
                    }
                    WorkClass.Add(shift2Object);
                }
                Outputs.Add(WorkClass);

                foreach (string line in lineList)
                {
                    lineUPH = new LineUPHObject();
                    lineUPH.LineName = line;
                    lineUPH.StationUPHQTY = new List<SationUPH>();
                    foreach (DataRow row in stationTable.Rows)
                    {
                        stationUPH = new SationUPH();
                        stationUPH.StationName = row["STATION_NAME"].ToString();
                        stationUPH.Shift1Total = 0;
                        stationUPH.Shift2Total = 0;
                        stationUPH.Total = 0;
                        stationUPH.UPH = new List<TimeUPH>();
                        stationRow = tableUPH.Select(" PRODUCTION_LINE= '" + line + "' and STATION_NAME='"+ row["STATION_NAME"].ToString() + "'");
                        if (stationRow.Length > 0)
                        {                            
                            for (int j = 0; j < workClassTable.Rows.Count; j++)
                            {
                                workTimeUPH = new TimeUPH();
                                workTimeUPH.Shift = workClassTable.Rows[j]["NAME"].ToString();
                                workTimeUPH.Time = workClassTable.Rows[j]["START_TIME"].ToString() + "-" + workClassTable.Rows[j]["END_TIME"].ToString();
                                workTimeUPH.Qty = "0";
                                for (int i = 0; i < stationRow.Length; i++)
                                {  
                                    if (workClassTable.Rows[j]["START_TIME"].ToString().Split(':')[0].Equals(stationRow[i]["WORK_TIME"].ToString()))
                                    {                                      
                                        workTimeUPH.Qty = stationRow[i]["QTY"].ToString();                                       
                                        break;                                      
                                    } 
                                }
                                if(workTimeUPH.Shift== "SHIFT 1")
                                {
                                    stationUPH.Shift1Total = Convert.ToInt32(stationUPH.Shift1Total) + Convert.ToInt32(workTimeUPH.Qty);
                                }
                                if (workTimeUPH.Shift == "SHIFT 2")
                                {
                                    stationUPH.Shift2Total = Convert.ToInt32(stationUPH.Shift2Total) + Convert.ToInt32(workTimeUPH.Qty);
                                }
                               
                                stationUPH.UPH.Add(workTimeUPH);
                            }
                           
                        }
                        else
                        {
                            for (int j = 0; j < workClassTable.Rows.Count; j++)
                            {
                                workTimeUPH = new TimeUPH();
                                workTimeUPH.Shift = workClassTable.Rows[j]["NAME"].ToString();
                                workTimeUPH.Time= workClassTable.Rows[j]["START_TIME"].ToString() + "-" + workClassTable.Rows[j]["END_TIME"].ToString();
                                workTimeUPH.Qty = "0";
                                stationUPH.UPH.Add(workTimeUPH);
                            }
                        }
                        stationUPH.Total = stationUPH.Shift2Total + stationUPH.Shift1Total;
                        lineUPH.StationUPHQTY.Add(stationUPH);
                    }                    
                    skuUPH.LineUPH.Add(lineUPH);
                }
                if (skuUPH.LineUPH.Count > 0)
                {
                }
                Outputs.Add(skuUPH);
            }
            catch (Exception exception)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                Outputs.Add(new ReportAlart(exception.Message));                
            }
            
        }       

        private List<string> GetLine()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dtLine = SFCDB.RunSelect(Sqls["SqlGetLine"]).Tables[0];
                List<string> lineList = new List<string>();

                if (dtLine.Rows.Count > 0)
                {
                    foreach (DataRow row in dtLine.Rows)
                    {
                        lineList.Add(row["line_name"].ToString());
                    }
                }
                else
                {
                    throw new Exception("no line in system!");
                }
                return lineList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
        private List<string> GetShift()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {                
                //System.Globalization.DateTimeFormatInfo format = new System.Globalization.DateTimeFormatInfo();
                //format.ShortDatePattern = "yyyy/MM/dd";
                //DateTime workDate = Convert.ToDateTime(date.Value.ToString(), format);

                //sqlGetShift = $@"select class_name from (select 'ALL' as class_name from dual union
                //            select distinct class_name from r_uph_detail where work_date=to_date('{workDate.ToString("yyyy/MM/dd")}','YYYY/MM/DD')) order by class_name";
                sqlGetShift = "select class_name from (select 'ALL' as class_name from dual union select distinct name as class_name  from c_work_class  ) order by class_name";
                Sqls.Add("SqlGetShift", sqlGetShift);
                DataTable dtShift = SFCDB.RunSelect(sqlGetShift).Tables[0];
                List<string> shiftList = new List<string>();
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                if (dtShift.Rows.Count > 0)
                {
                    foreach (DataRow row in dtShift.Rows)
                    {
                        shiftList.Add(row["class_name"].ToString());
                    }
                }
                else
                {
                    throw new Exception("no shift in system!");
                }
                return shiftList;
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }

        private List<string> GetSkuno()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                System.Globalization.DateTimeFormatInfo format = new System.Globalization.DateTimeFormatInfo();
                format.ShortDatePattern = "yyyy/MM/dd";
                DateTime workDate = Convert.ToDateTime(date.Value.ToString(), format);

                sqlGetSkuno = $@"select distinct skuno from r_uph_detail where work_date=to_date('{workDate.ToString("yyyy/MM/dd")}' ,'yyyy/mm/dd') order by skuno";
                //RunSqls.Add(sqlGetSkuno);
                DataTable dtSkuno = SFCDB.RunSelect(sqlGetSkuno).Tables[0];
                List<string> skunoList = new List<string>();
                skunoList.Add("ALL");
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                if (dtSkuno.Rows.Count > 0)
                {
                    foreach (DataRow row in dtSkuno.Rows)
                    {
                        skunoList.Add(row["skuno"].ToString());
                    }
                }
                else
                {
                    throw new Exception("no skuno in system!");
                }
                return skunoList;
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw ex;
            }
        }
        private void LineChange(object sender, ReportInputChangeArgs e)
        {

        }
    }

    public class UPHObject
    {   
        public string Skuno
        {
            get;
            set;
        }
        public List<LineUPHObject> LineUPH
        {
            get;
            set;
        }   
    }

    public class LineUPHObject
    {
        public string LineName { get; set; }
        public List<SationUPH> StationUPHQTY
        {
            get;
            set;
        }       
    }    

    public class SationUPH
    {
        public string StationName { get; set; }
        public int Shift1Total { get; set; }
        public int Shift2Total { get; set; }
        public int Total { get; set; }
        public List<TimeUPH> UPH
        {
            get;
            set;
        }
    }

    public class TimeUPH
    {
        private string _qty;
        private string _shift;
        private string _time;
        public string Qty {
            get { return _qty; }
            set { _qty = value; }                  
        }
        public string Shift {
            get { return _shift; }
            set { _shift = value; }
        }
        public string Time {
            get { return _time; }
            set { _time = value; }
        }

        public void IntTimeUPH(string workTime,string qty,DataTable ClassTabel)
        {
            string startTime = "";
            foreach (DataRow classRow in ClassTabel.Rows)
            {
                startTime = classRow["START_TIME"].ToString().Split(':')[0];
                _shift = classRow["NAME"].ToString();
                _qty = "0";
                _time = classRow["START_TIME"].ToString() + "-" + classRow["END_TIME"].ToString();
                if (startTime.Equals(workTime))
                {
                    _qty = qty;
                    _time = classRow["START_TIME"].ToString() + "-" + classRow["END_TIME"].ToString();
                    break;                
                }                
            }
        }
    }

    public class WorkClassObject
    {
        public string WorkClassType
        {
            get;
            set;
        }
        public List<string> WorkTime
        {
            get;
            set;
        }
    }
}
