using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class FPYReportDetail : ReportBase
    {
        ReportInput inputDateType = new ReportInput() { Name = "DateType", InputType = "Select", Value = "ByHours", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ByHours", "ByDay", "ByWeeks", "ByMonth", "ByQuarter", "ByYears" } };
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuno = new ReportInput() { Name = "Skuno", InputType = "Select", Value = "All", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "All", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStatus = new ReportInput { Name = "Status", InputType = "Select", Value = "PASS", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "PASS", "FAIL" } };
        ReportInput inputDate = new ReportInput { Name = "Date", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        //ReportInput inputDateType = new ReportInput() { Name = "DateType", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse =null };
        //ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputSkuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputStatus = new ReportInput { Name = "Status", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        //ReportInput inputDate = new ReportInput { Name = "Date", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public FPYReportDetail()
        {
            Inputs.Add(inputDateType);
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);
            Inputs.Add(inputSkuno);
            Inputs.Add(inputStation);
            Inputs.Add(inputStatus);
            Inputs.Add(inputDate);

            string sqlGetSkuno = $@"select 'ALL' as skuno from dual union select distinct skuno from c_sku where id in (
                                    select sku_id from r_sku_route where route_id in (
                                    select distinct route_id from c_route_detail where station_name in (select mes_station from c_temes_station_mapping)))
                                    order by skuno";
            string sqlGetStation = $@"select distinct mes_station as station_name  from c_temes_station_mapping  order by mes_station";
            Sqls.Add("GetSkuno", sqlGetSkuno);
            Sqls.Add("GetStation", sqlGetStation);
        }

        public override void Init()
        {
            base.Init();
            inputDateFrom.Value = DateTime.Now.AddDays(-7);
            inputDateTo.Value = DateTime.Now;

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dtSkuno = SFCDB.RunSelect(Sqls["GetSkuno"]).Tables[0];
            DataTable dtStation = SFCDB.RunSelect(Sqls["GetStation"]).Tables[0];

            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            List<string> skunoList = new List<string>();
            foreach (DataRow row in dtSkuno.Rows)
            {
                skunoList.Add(row["skuno"].ToString());
            }

            List<string> stationList = new List<string>();
            stationList.Add("SMT1");
            stationList.Add("SMT2");
            foreach (DataRow row in dtStation.Rows)
            {
                stationList.Add(row["station_name"].ToString());
            }
            inputSkuno.ValueForUse = skunoList;
            inputStation.ValueForUse = stationList;
        }

        public override void Run()
        {
            base.Run();
            string skuno = inputSkuno.Value.ToString();
            string station = inputStation.Value.ToString();
            string status = inputStatus.Value.ToString();
            if (status == "ALL") {

                status = "FAIL','PASS";
            }

            string date = inputDate.Value.ToString().Replace("%20"," ").Replace("Week","").Replace("Quarter","").Replace("Year","");
            string dateType = inputDateType.Value.ToString();

            string dateFrom = Convert.ToDateTime(inputDateFrom.Value).ToString("yyyy/MM/dd HH:mm:ss");
            string dateTo = Convert.ToDateTime(inputDateTo.Value).ToString("yyyy/MM/dd HH:mm:ss");


            string tempSql, skuSql;

            OleExec SFCDB01 = DBPools["SFCDB"].Borrow();
            if (SFCDB01 != null)
            {
                DBPools["SFCDB"].Return(SFCDB01);
            }
            string bu = SFCDB01.ORM.Queryable<MESDataObject.Module.C_BU>().Select(r => r.BU).ToList().Distinct().FirstOrDefault();


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

                tempSql = $@" a.sn in (select distinct sn from r_sn where {skuSql} ) and messtation='{station}'  and a.starttime between to_date('{dateFrom}','YYYY/MM/DD HH24:MI:SS') and  to_date('{dateTo}','YYYY/MM/DD  HH24:MI:SS')";
            }
            string runSql = "";
            switch (dateType)
            {

                case "ByHours":
                    //if (station == "SMT1" || station == "SMT2")
                    //{
                    //    runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                    //            from r_sn_station_detail where {tempSql} and repair_failed_flag=1 and to_char(edit_time, 'YYYY/MM/DD HH24')='{date}' ORDER BY edit_time";
                    //}
                    //else
                    //{
                    //runSql = $@"select distinct b.skuno,a.sn,a.messtation,decode(a.state,'P','PASS','F','FAIL',a.state) as status ,a.EDIT_TIME
                    //        from r_test_record a,r_sn b where a.sn=b.sn and b.valid_flag=1 and {tempSql} and a.state='{status}' and to_char(a.EDIT_TIME, 'YYYY/MM/DD HH24')='{date}' ORDER BY a.EDIT_TIME";
                    //}

                    if (station == "SMT1" || station == "SMT2")
                    {
                        runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                                from r_sn_station_detail where {tempSql} and to_char(edit_time, 'YYYY/MM/DD HH24')='{date}' ORDER BY edit_time";
                    }
                    else
                    {

                        if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                        {

                            runSql = $@"select SKUNO,a.SN,MESSTATION, STATE STATUS,a.EDIT_TIME
                                       from(select *
                                               from(select c.*,
                                                            ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                       from r_test_record c) cc
                                              where rn = 1   and state in ('Pass','PASS','Fail','FAIL')) a,
                                            SFCRUNTIME.R_SN b
                                      where a.sn = b.sn
                                        and {tempSql}
                                      
                                        and to_char(a.starttime, 'YYYY/MM/DD HH24') ='{date}'
                                        AND UPPER(STATE) IN ('{status}')
                                        and VALID_FLAG = 1   ORDER BY SKUNO,STATE, a.EDIT_TIME ASC ";
                        }
                        else
                        {

                            runSql = $@"select distinct b.skuno,a.sn,a.messtation,decode(a.state,'P','PASS','F','FAIL',a.state) as status ,a.EDIT_TIME
                                from r_test_record a,r_sn b where a.sn=b.sn and b.valid_flag=1 and {tempSql} and a.state='{status}' and to_char(a.EDIT_TIME, 'YYYY/MM/DD HH24')='{date}' ORDER BY a.EDIT_TIME";

                        }
                    }
                    break;
                case "ByDay":
                    //if (station == "SMT1" || station == "SMT2")
                    //{
                    //    runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                    //            from r_sn_station_detail where {tempSql} and repair_failed_flag=1 and to_char(edit_time, 'YYYY/MM/DD')='{date}' ORDER BY edit_time";
                    //}            
                    //else
                    //{
                    //runSql = $@"select distinct b.skuno,a.sn,a.messtation,decode(a.state,'P','PASS','F','FAIL',a.state) as status ,a.EDIT_TIME
                    //      from r_test_record a,r_sn b  where a.sn=b.sn and b.valid_flag=1 and {tempSql} and a.state='{status}' and to_char(a.EDIT_TIME, 'YYYY/MM/DD')='{date}' ORDER BY a.EDIT_TIME";


                    //runSql = $@" SELECT SKUNO,SN,MESSTATION,STATUS,EDIT_TIME FROM( 
                    //                SELECT b.SKUNO, a.SN,  a.MESSTATION, a.STATE status, a.EDIT_TIME,
                    //                ROW_NUMBER() OVER (PARTITION BY a.SN ORDER BY a.EDIT_TIME ASC) AS RN 
                    //                FROM SFCRUNTIME.R_TEST_RECORD a, SFCRUNTIME.R_SN b
                    //                WHERE    a.sn = b.sn
                    //                and {tempSql})
                    //            WHERE RN = 1 AND UPPER(status) IN ('{status}')
                    //            and to_char(EDIT_TIME, 'YYYY/MM/DD')='{date}'
                    //            ORDER BY EDIT_TIME ASC";
                    //}


                    if (station == "SMT1" || station == "SMT2")
                    {
                        runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                                from r_sn_station_detail where {tempSql}  and to_char(edit_time, 'YYYY/MM/DD')='{date}' ORDER BY edit_time";
                    }
                    else
                    {

                        if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                        {

                            runSql = $@"select SKUNO,a.SN,MESSTATION, STATE STATUS,a.EDIT_TIME
                                       from(select *
                                               from(select c.*,
                                                            ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                       from r_test_record c) cc
                                              where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) a,
                                            SFCRUNTIME.R_SN b
                                      where a.sn = b.sn
                                        and {tempSql}
                                      
                                        and to_char(a.starttime, 'YYYY/MM/DD') ='{date}'
                                        AND UPPER(STATE) IN ('{status}')
                                        and VALID_FLAG = 1   ORDER BY SKUNO,STATE, a.EDIT_TIME ASC ";
                        }
                        else
                        {

                            runSql = $@" SELECT SKUNO,SN,MESSTATION,STATUS,EDIT_TIME FROM( 
                                        SELECT b.SKUNO, a.SN,  a.MESSTATION, a.STATE status, a.EDIT_TIME,
                                        ROW_NUMBER() OVER (PARTITION BY a.SN ORDER BY a.EDIT_TIME ASC) AS RN 
                                        FROM SFCRUNTIME.R_TEST_RECORD a, SFCRUNTIME.R_SN b
                                        WHERE    a.sn = b.sn
                                        and {tempSql})
                                    WHERE RN = 1 AND UPPER(status) IN ('{status}')
                                    and to_char(EDIT_TIME, 'YYYY/MM/DD')='{date}'
                                    ORDER BY EDIT_TIME ASC";

                        }

                    }






                    break;
                case "ByWeeks":
                    //if (station == "SMT1" || station == "SMT2")
                    //{     
                    //    runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                    //            from r_sn_station_detail where {tempSql} and repair_failed_flag=1 and to_char(edit_time, 'IW')='{date}' ORDER BY edit_time";
                    //}
                    //else
                    //{
                    //runSql = $@"SELECT SKUNO,SN,MESSTATION,STATUS,EDIT_TIME FROM( 
                    //                SELECT b.SKUNO, a.SN,  a.MESSTATION, a.STATE status, a.EDIT_TIME,
                    //                ROW_NUMBER() OVER (PARTITION BY a.SN ORDER BY a.EDIT_TIME ASC) AS RN 
                    //                FROM SFCRUNTIME.R_TEST_RECORD a, SFCRUNTIME.R_SN b
                    //                WHERE    a.sn = b.sn
                    //                and {tempSql})
                    //            WHERE RN = 1 AND UPPER(status) IN ('{status}')
                    //            and to_char(EDIT_TIME, 'IW')='{date}'
                    //            ORDER BY EDIT_TIME ASC";
                    //}

                    if (station == "SMT1" || station == "SMT2")
                    {
                        runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                                from r_sn_station_detail where {tempSql}  and to_char(edit_time, 'IW')='{date}' ORDER BY edit_time";
                    }
                    else
                    {
                        if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                        {

                            runSql = $@"select SKUNO,a.SN,MESSTATION, STATE STATUS,a.EDIT_TIME
                                       from(select *
                                               from(select c.*,
                                                            ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                       from r_test_record c) cc
                                              where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) a,
                                            SFCRUNTIME.R_SN b
                                      where a.sn = b.sn
                                        and {tempSql}
                                       
                                        and to_char(a.starttime, 'IW') ='{date}'
                                        AND UPPER(STATE) IN ('{status}')
                                        and VALID_FLAG = 1   ORDER BY SKUNO,STATE, a.EDIT_TIME ASC ";
                        }
                        else
                        {

                            runSql = $@"SELECT SKUNO,SN,MESSTATION,STATUS,EDIT_TIME FROM( 
                                        SELECT b.SKUNO, a.SN,  a.MESSTATION, a.STATE status, a.EDIT_TIME,
                                        ROW_NUMBER() OVER (PARTITION BY a.SN ORDER BY a.EDIT_TIME ASC) AS RN 
                                        FROM SFCRUNTIME.R_TEST_RECORD a, SFCRUNTIME.R_SN b
                                        WHERE    a.sn = b.sn
                                        and {tempSql})
                                    WHERE RN = 1 AND UPPER(status) IN ('{status}')
                                    and to_char(EDIT_TIME, 'IW')='{date}'
                                    ORDER BY EDIT_TIME ASC";


                        }

                    }



                    break;
                case "ByMonth":
                    //if (station == "SMT1" || station == "SMT2")
                    //{  
                    //    runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                    //            from r_sn_station_detail where {tempSql} and repair_failed_flag=1 and to_char(edit_time, 'YYYY/MM')='{date}' ORDER BY edit_time";
                    //}
                    //else
                    //{                       
                    //runSql = $@"SELECT SKUNO,SN,MESSTATION,STATUS,EDIT_TIME FROM( 
                    //                SELECT b.SKUNO, a.SN,  a.MESSTATION, a.STATE status, a.EDIT_TIME,
                    //                ROW_NUMBER() OVER (PARTITION BY a.SN ORDER BY a.EDIT_TIME ASC) AS RN 
                    //                FROM SFCRUNTIME.R_TEST_RECORD a, SFCRUNTIME.R_SN b
                    //                WHERE    a.sn = b.sn
                    //                and {tempSql})
                    //            WHERE RN = 1 AND UPPER(status) IN ('{status}')
                    //            AND to_char(EDIT_TIME, 'YYYY/MM')='{date}'
                    //            ORDER BY EDIT_TIME ASC";
                    //}


                    if (station == "SMT1" || station == "SMT2")
                    {
                        runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                                from r_sn_station_detail where {tempSql}  and to_char(edit_time, 'YYYY/MM')='{date}' ORDER BY edit_time";
                    }
                    else
                    {
                        if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                        {

                            runSql = $@"select SKUNO,a.SN,MESSTATION, STATE STATUS,a.EDIT_TIME
                                       from(select *
                                               from(select c.*,
                                                            ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                       from r_test_record c) cc
                                              where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) a,
                                            SFCRUNTIME.R_SN b
                                      where a.sn = b.sn
                                        and {tempSql}
                                        and to_char(a.starttime, 'YYYY/MM') ='{date}'
                                        AND UPPER(STATE) IN ('{status}')
                                        and VALID_FLAG = 1    ORDER BY SKUNO,STATE, a.EDIT_TIME ASC  ";
                        }
                        else
                        {
                            runSql = $@"SELECT SKUNO,SN,MESSTATION,STATUS,EDIT_TIME FROM( 
                                        SELECT b.SKUNO, a.SN,  a.MESSTATION, a.STATE status, a.EDIT_TIME,
                                        ROW_NUMBER() OVER (PARTITION BY a.SN ORDER BY a.EDIT_TIME ASC) AS RN 
                                        FROM SFCRUNTIME.R_TEST_RECORD a, SFCRUNTIME.R_SN b
                                        WHERE    a.sn = b.sn
                                        and {tempSql})
                                    WHERE RN = 1 AND UPPER(status) IN ('{status}')
                                    AND to_char(EDIT_TIME, 'YYYY/MM')='{date}'
                                    ORDER BY EDIT_TIME ASC ";
                        }
                    }
                    break;
                case "ByQuarter":
                    //if (station == "SMT1" || station == "SMT2")
                    //{  
                    //    runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                    //            from r_sn_station_detail where {tempSql} and repair_failed_flag=1 and to_char(edit_time, 'q')='{date}' ORDER BY edit_time";
                    //}
                    //else
                    //{
                    //runSql = $@"select DISTNCT b.skuno,a.sn,a.messtation,decode(a.state,'P','PASS','F','FAIL',a.state) as status ,a.EDIT_TIME
                    //        from r_test_record a,r_sn b where a.sn=b.sn and b.valid_flag=1 and {tempSql} and a.state='{status}' and to_char(a.EDIT_TIME, 'q')='{date}' ORDER BY a.EDIT_TIME";
                    //}


                    if (station == "SMT1" || station == "SMT2")
                    {
                        runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                                from r_sn_station_detail where {tempSql}  and to_char(edit_time, 'q')='{date}' ORDER BY edit_time";
                    }
                    else
                    {
                        if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                        {

                            runSql = $@"select SKUNO,a.SN,MESSTATION, STATE STATUS,a.EDIT_TIME
                                       from(select *
                                               from(select c.*,
                                                            ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                       from r_test_record c) cc
                                              where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) a,
                                            SFCRUNTIME.R_SN b
                                      where a.sn = b.sn
                                        and {tempSql}
                                        and to_char(a.starttime, 'q') ='{date}'
                                        AND UPPER(STATE) IN ('{status}')
                                        and VALID_FLAG = 1   ORDER BY SKUNO,STATE, a.EDIT_TIME ASC ";
                        }
                        else
                        {
                            runSql = $@"select DISTNCT b.skuno,a.sn,a.messtation,decode(a.state,'P','PASS','F','FAIL',a.state) as status ,a.EDIT_TIME
                                from r_test_record a,r_sn b where a.sn=b.sn and b.valid_flag=1 and {tempSql} and a.state='{status}' and to_char(a.EDIT_TIME, 'q')='{date}' ORDER BY a.EDIT_TIME";
                        }
                    }
                    break;
                case "ByYears":
                    //if (station == "SMT1" || station == "SMT2")
                    //{
                    //    runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                    //            from r_sn_station_detail where {tempSql} and repair_failed_flag=1 and to_char(edit_time, 'YYYY')='{date}' ORDER BY edit_time";
                    //}
                    //else
                    //{
                    //runSql = $@"select distinct b.skuno,a.sn,a.messtation,decode(a.state,'P','PASS','F','FAIL',a.state) as status ,a.EDIT_TIME
                    //        from r_test_record a, r_sn b where a.sn=b.sn and b.valid_flag=1 and {tempSql} and a.state='{status}' and to_char(EDIT_TIME, 'YYYY')='{date}' ORDER BY a.EDIT_TIME";
                    //}

                    if (station == "SMT1" || station == "SMT2")
                    {
                        runSql = $@"select distinct skuno,sn,station_name,decode(repair_failed_flag,'0','PASS','1','FAIL') as status ,edit_time 
                                from r_sn_station_detail where {tempSql}  and to_char(edit_time, 'YYYY')='{date}' ORDER BY edit_time";
                    }
                    else
                    {
                        if (bu == "VNJUNIPER")//VN DCN QE: Zena Ruan 要求同一SN同一工站無論PASS/FAIL只抓一次數據--BY G6007338--2022-02-22
                        {

                            runSql = $@"select SKUNO,a.SN,MESSTATION, STATE STATUS,a.EDIT_TIME
                                       from(select *
                                               from(select c.*,
                                                            ROW_NUMBER() OVER(PARTITION BY sn, TESTATION ORDER BY STARTTIME ASC) rn
                                                       from r_test_record c) cc
                                              where rn = 1  and state in ('Pass','PASS','Fail','FAIL') ) a,
                                            SFCRUNTIME.R_SN b
                                      where a.sn = b.sn
                                        and {tempSql}
                                        and to_char(a.starttime, 'YYYY') ='{date}'
                                        AND UPPER(STATE) IN ('{status}')
                                        and VALID_FLAG = 1   ORDER BY SKUNO,STATE, a.EDIT_TIME ASC ";
                        }
                        else
                        {
                            runSql = $@"select distinct b.skuno,a.sn,a.messtation,decode(a.state,'P','PASS','F','FAIL',a.state) as status ,a.EDIT_TIME
                                from r_test_record a, r_sn b where a.sn=b.sn and b.valid_flag=1 and {tempSql} and a.state='{status}' and to_char(EDIT_TIME, 'YYYY')='{date}' ORDER BY a.EDIT_TIME";
                        }
                    }
                    break;
                default:
                    throw new Exception("DateType Error!");
            }
           OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                RunSqls.Add(runSql);                
                DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("No Data!");
                }
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dt, null);
                retTab.Tittle = $@"{skuno} {station} FPY Report Detail";
                Outputs.Add(retTab);
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
    }
}
