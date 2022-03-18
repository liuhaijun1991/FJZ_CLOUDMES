using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;

namespace MESReport.BaseReport
{
    public class TestRecordReport : ReportBase
    {

        ReportInput date = new ReportInput() { Name = "Date", InputType = "TXT", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput line = new ReportInput() { Name = "Line", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput skuno = new ReportInput() { Name = "Skuno", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput skuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput shift = new ReportInput() { Name = "Shift", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        string sqlGetLine = "";
        string sqlGetSkuno = "";
        string sqlGetShift = "";
        List<string> lineALL;
        DataTable tableUPH;
        public TestRecordReport()
        {
            Inputs.Add(date);
            Inputs.Add(line);
            Inputs.Add(skuno);
           // Inputs.Add(shift);
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
            lineALL = GetLine();
            List<string> tempList = new List<string>();
            tempList.Add("ALL");
            foreach (string line in lineALL)
            {
                tempList.Add(line);
            }
            line.ValueForUse = tempList;
            //skuno.ValueForUse = GetSkuno();
          //  shift.ValueForUse = GetShift();
            line.Change += LineChange;
        }



        public override void Run()
        {
            base.Run();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                System.Globalization.DateTimeFormatInfo format = new System.Globalization.DateTimeFormatInfo();
                format.ShortDatePattern = "yyyy/MM/dd";
                DateTime runDate = Convert.ToDateTime(date.Value.ToString(), format);
                string runLine = line.Value.ToString();
                string runSkuno = skuno.Value.ToString();
                //string runShift = shift.Value.ToString();
                string runSqlUPH = $@"SELECT d.line, d.class_name,d.work_time,d.station_name,nvl(sum(d.pass),0) passQty,nvl(sum(d.fail),0) failQty
                                       FROM (select b.line,b.class_name,to_char(A.edit_time, 'HH24') as work_time, a.messtation as station_name,
                                       case
                                         when a.state = 'PASS' then
                                          1
                                         WHEN A.STATE = 'FAIL' THEN
                                          0
                                       end AS PASS,
                                       case
                                         when a.state = 'PASS' then
                                          0
                                         WHEN A.STATE = 'FAIL' THEN
                                          1
                                       end AS FAIL from r_test_record a ,r_sn_station_detail b  where a.sn = b.sn and a.messtation = b.station_name";
                if (runSkuno.ToUpper() != "ALL")
                {
                    runSqlUPH = runSqlUPH + $@" and b.skuno = '{runSkuno}'";
                }
                if (runLine.ToUpper() != "ALL")
                {
                    runSqlUPH = runSqlUPH + $@"  and b.LINE='{runLine}' ";
                    lineALL = new List<string>();
                    lineALL.Add(runLine);
                }
                //if (runShift.ToUpper() != "ALL")
                //{
                //    runSqlUPH = runSqlUPH + $@" and b.class_name ='{runShift}' ";
                //}
                //else
                //{
                //    runSqlUPH = runSqlUPH + $@" and exists(select * from c_work_class wc where b.class_name=wc.name) ";
                //}
                runSqlUPH = runSqlUPH + $@" and to_char(a.edit_time,'YYYY/MM/DD')='{runDate.ToString("yyyy/MM/dd")}')d
                                            group by  d.line, d.class_name, d.work_time, d.station_name
                                            order by d.line, d.class_name, d.work_time, d.station_name";
                tableUPH = SFCDB.RunSelect(runSqlUPH).Tables[0];
                if (tableUPH.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                Outputs.Add(GetData(SFCDB));
            }
            catch (Exception exception)
            {               
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public object GetData1(OleExec DB)
        {
            List<LineRecord> lineList = new List<LineRecord>();
            DataRow[] stationRow;
            DataRow[] uphRow;
            //DataView dv = tableUPH.DefaultView;
            foreach (DataRow row  in tableUPH.Rows)
            {
                //tableUPH = dv.ToTable(true, new string[] { "STATION_NAME" });
                stationRow = tableUPH.Select(" LINE= '" + row["LINE"].ToString() + "'");

                //tableUPH = dv.ToTable(true, new string[] { "STATION_NAME" });
                LineRecord line = new LineRecord();
                line.LineName = row["LINE"].ToString();
                line.station = new List<StationRecord>();
                foreach (DataRow sRow in stationRow)
                {
                    StationRecord sr = new StationRecord();
                    sr.IntStationObj(sRow["station_name"].ToString(), DB);
                    uphRow = tableUPH.Select(" LINE= '" + sRow["LINE"].ToString() + "' and STATION_NAME='" + sRow["STATION_NAME"].ToString() + "'");
                    foreach (var s in sr.UPH)
                    {
                        s.PassQty = 0;
                        s.FailQty = 0;
                        //foreach (var uRow in uphRow)
                        //{   //20:00-21:00
                        //    if (s.Time == uRow[""].ToString())
                        //    {
                        //        s.PassQty = 0;
                        //        s.FailQty = 0;
                        //        break;
                        //    }
                        //}
                    }
                    sr.AddShiftCount();
                    line.station.Add(sr);
                }
                lineList.Add(line);
            }
            return lineList;
        }
        public object GetData(OleExec DB)
        {
            List<LineRecord> lineList = new List<LineRecord>();
            LineRecord lineRecord;
            StationRecord stationRecord;
            var data = tableUPH.AsEnumerable();
            var line = data.Select(d => d.Field<string>("LINE")).Distinct();            
            foreach (var l in line)
            {
                lineRecord = new LineRecord();
                lineRecord.LineName = l;
                lineRecord.station = new List<StationRecord>();
                var station = data.Where(d => d.Field<string>("LINE") == l).Select(s => s.Field<string>("STATION_NAME")).Distinct();
                foreach (var s in station)
                {
                    stationRecord = new StationRecord();
                    stationRecord.IntStationObj(s, DB);
                    var uph = data.Where(d => d.Field<string>("LINE") == l && d.Field<string>("STATION_NAME") == s);
                    foreach (var u in stationRecord.UPH)
                    {
                        foreach (var h in uph)
                        {
                            if (h.Field<string>("WORK_TIME") == u.Time.Split('-')[0].Split(':')[0])
                            {
                                u.PassQty = Convert.ToDouble(h.Field<decimal>("PASSQTY"));
                                u.FailQty = Convert.ToDouble(h.Field<decimal>("FAILQTY"));
                                break;
                            }
                        }                        
                    }
                    stationRecord.AddShiftCount();
                    lineRecord.station.Add(stationRecord);
                }                
                lineList.Add(lineRecord);
            }            
            return lineList;
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
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
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
    public class TimeObj
    {        
        public double PassQty { get; set; }
        public double FailQty { get; set; }
        public string Shift { get; set; }
        public string Time { get; set; }
    }
    public class StationRecord
    {
        private string _stationName;
        private List<TimeObj> _timeObjList;
        public string station_name {
            get { return _stationName; }
            set { value = _stationName; }
        } 
        public List<TimeObj> UPH
        {
            get { return _timeObjList; }
            set { value = _timeObjList; }
        }
        public void IntStationObj(string name, OleExec DB)
        {
            _stationName = name;
            List<Shift> shiftList = Shift.GetShift("ALL", DB);
            TimeObj timeObj;
            _timeObjList = new List<TimeObj>();

            foreach (var s in shiftList)
            {
                foreach (var t in s.ShiftTime)
                {
                    timeObj = new TimeObj();
                    timeObj.PassQty = 0;
                    timeObj.FailQty = 0;
                    timeObj.Shift = s.ShiftType;
                    timeObj.Time = t;
                    _timeObjList.Add(timeObj);
                }
            }
        }

        public void AddShiftCount()
        {
            if (_timeObjList.Count == 0)
            {
                throw new Exception("Please initialize first！");
            }
            double shift1TotalPassQty = _timeObjList.FindAll(s => s.Shift.ToUpper() == "SHIFT1").Sum(ss => ss.PassQty);
            double shift1TotalFailQty = _timeObjList.FindAll(s => s.Shift.ToUpper() == "SHIFT1").Sum(ss => ss.FailQty);
            double shift2TotalPassQty = _timeObjList.FindAll(s => s.Shift.ToUpper() == "SHIFT2").Sum(ss => ss.PassQty);
            double shift2TotalFailQty = _timeObjList.FindAll(s => s.Shift.ToUpper() == "SHIFT2").Sum(ss => ss.FailQty);

            _timeObjList.Insert(_timeObjList.FindLastIndex(ti => ti.Shift.ToUpper() == "SHIFT1") + 1, 
                new TimeObj { PassQty = shift1TotalPassQty, FailQty = shift1TotalFailQty, Shift = "SHIFT1 TOTAL", Time = "SHIFT1 TOTAL" });

            _timeObjList.Insert(_timeObjList.FindLastIndex(ti => ti.Shift.ToUpper() == "SHIFT2") + 1, 
                new TimeObj { PassQty = shift2TotalPassQty, FailQty = shift2TotalFailQty, Shift = "SHIFT2 TOTAL", Time = "SHIFT2 TOTAL" });
            _timeObjList.Add(new TimeObj { PassQty = shift1TotalPassQty + shift2TotalPassQty, FailQty = shift1TotalFailQty + shift2TotalFailQty, Shift = "SHIFT", Time = "TOTAL" });

        }
    }
    public class LineRecord
    {
        private string line_name;
        public List<StationRecord> station { get; set; }
        public string LineName
        {
            get { return line_name; }
            set { line_name = value; }
        }
    }
    public class Shift
    {
        public string ShiftType
        {
            get;
            set;
        }
        public List<string> ShiftTime
        {
            get;
            set;
        }
        public static List<Shift> GetShift(string shiftType, OleExec DB)
        {
            string runSqlShift1 = $@"select * from  c_work_class where name='SHIFT 1'  order by start_time";
            string runSqlShift21 = $@"select  * from c_work_class where name='SHIFT 2' and start_time>='20:00' order by start_time";
            string runSqlShift22 = $@"select  * from c_work_class where name='SHIFT 2' and start_time<'20:00' order by start_time";
            DataTable tableShift1 = DB.RunSelect(runSqlShift1).Tables[0];
            DataTable tableShift21 = DB.RunSelect(runSqlShift21).Tables[0];
            DataTable tableShift22 = DB.RunSelect(runSqlShift22).Tables[0];
           
            List<Shift> shiftList = new List<Shift>();
            if (shiftType.ToUpper() != "ALL")
            {
                Shift shift = new Shift();
                shift.ShiftType = shiftType.ToUpper();
                shift.ShiftTime = new List<string>();
                if (shiftType.ToUpper() == "SHIFT 1")
                {
                    for (int i = 0; i < tableShift1.Rows.Count; i++)
                    {
                        shift.ShiftTime.Add(tableShift1.Rows[i]["START_TIME"].ToString() + "-" + tableShift1.Rows[i]["END_TIME"].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < tableShift21.Rows.Count; i++)
                    {
                        shift.ShiftTime.Add(tableShift21.Rows[i]["START_TIME"].ToString() + "-" + tableShift21.Rows[i]["END_TIME"].ToString());
                    }
                    for (int i = 0; i < tableShift22.Rows.Count; i++)
                    {
                        shift.ShiftTime.Add(tableShift22.Rows[i]["START_TIME"].ToString() + "-" + tableShift22.Rows[i]["END_TIME"].ToString());
                    }
                }
                shiftList.Add(shift);
            }
            else
            {
                Shift shift1Object = new Shift();
                shift1Object.ShiftType = "SHIFT1";
                shift1Object.ShiftTime = new List<string>();
                for (int i = 0; i < tableShift1.Rows.Count; i++)
                {
                    shift1Object.ShiftTime.Add(tableShift1.Rows[i]["START_TIME"].ToString() + "-" + tableShift1.Rows[i]["END_TIME"].ToString());
                }
                shiftList.Add(shift1Object);

                Shift shift2Object = new Shift();
                shift2Object.ShiftType = "SHIFT2";
                shift2Object.ShiftTime = new List<string>();
                for (int i = 0; i < tableShift21.Rows.Count; i++)
                {
                    shift2Object.ShiftTime.Add(tableShift21.Rows[i]["START_TIME"].ToString() + "-" + tableShift21.Rows[i]["END_TIME"].ToString());
                }
                for (int i = 0; i < tableShift22.Rows.Count; i++)
                {
                    shift2Object.ShiftTime.Add(tableShift22.Rows[i]["START_TIME"].ToString() + "-" + tableShift22.Rows[i]["END_TIME"].ToString());
                }
                shiftList.Add(shift2Object);
            }
            return shiftList;
        }
    }   
}
