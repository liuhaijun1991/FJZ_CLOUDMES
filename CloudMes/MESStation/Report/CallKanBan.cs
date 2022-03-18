using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Report
{
    public class CallKanBan : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo FGetOracleTestStatus = new APIInfo()
        {
            FunctionName = "GetOracleTestStatus",
            Description = "Get Oracle Test Status From TDMS",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetHWDAOIStatus = new APIInfo()
        {
            FunctionName = "GetHWDAOIStatus",
            Description = "HWD AOI 看板数据源",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){InputType="string" , InputName="LINE" , DefaultValue=""},
                new APIInputInfo(){InputType="string" , InputName="START_TIME" , DefaultValue=""},
                new APIInputInfo(){InputType="string" , InputName="END_TIME" , DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetDefectData = new APIInfo()
        {
            FunctionName = "GetDefectData",
            Description = "VT Defect 看板数据源",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){InputType="string" , InputName="DateFrom" , DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetDefectByUserData = new APIInfo()
        {
            FunctionName = "GetDefectByUserData",
            Description = "VT Defect 看板数据源",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){InputType="string" , InputName="DateFrom" , DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _GetDefectByLineData = new APIInfo()
        {
            FunctionName = "GetDefectByLineData",
            Description = "VT Defect 看板数据源",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){InputType="string" , InputName="DateFrom" , DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        public CallKanBan()
        {
            _MastLogin = false;//看板不需要登录
            this.Apis.Add(FGetOracleTestStatus.FunctionName, FGetOracleTestStatus);
            this.Apis.Add(_GetHWDAOIStatus.FunctionName, _GetHWDAOIStatus);
            this.Apis.Add(_GetDefectData.FunctionName, _GetDefectData);
            this.Apis.Add(_GetDefectByUserData.FunctionName, _GetDefectByUserData);
            this.Apis.Add(_GetDefectByLineData.FunctionName, _GetDefectByLineData);
        }

        public void GetOracleTestStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var MachineList = sfcdb.ORM.Queryable<R_TEST_STATUS>()
                    .GroupBy(S => new { S.MACHINE_NO })
                    .Select(S => new { S.MACHINE_NO })
                    .OrderBy(S => S.MACHINE_NO, SqlSugar.OrderByType.Asc)
                    .ToList();
                var TestData = sfcdb.ORM.Queryable<R_TEST_STATUS, R_TDMS_MDV_DATA>((S, D) => new object[] { SqlSugar.JoinType.Left, S.SLOT_NO == D.LOCATION })
                    //.Where((S, D) =>)
                    .Select((S, D) => new { S.MACHINE_NO, S.SLOT_NO, STATUS = D.STATUS, S.SEQNO })
                    .OrderBy(S => S.SEQNO)
                    .ToList();

                StationReturn.Data = new { MACHINE_NO = MachineList, R_TEST_STATUS = TestData };
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;

                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void GetHWDAOIStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            if (!DBPools.ContainsKey("HWDAOI"))
            {
                OleExecPool p1 = new OleExecPool("Data Source = 10.120.191.134:1527 / HWD3SMT; User ID = smtuser; Password = smttest");
                DBPools.Add("HWDAOI",p1);
            }
            OleExec db = DBPools["HWDAOI"].Borrow();
            try
            {
                string LINE = Data["LINE"].ToString();
                string START_TIME = Data["START_TIME"].ToString();
                string END_TIME = Data["END_TIME"].ToString();

                if (START_TIME == null || START_TIME == "")
                {
                    START_TIME = DateTime.Now.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss");
                    END_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                string strSql =
                    $@"select *
  from (select a.error_position,
               substr( a.error_code,0,2) error_code,
               count(1) FailCount,
               a.topbutton,
               substr(a.line_id,0,5) line_id,
               substr(a.line_id,6) workstation,
               a.wo,
               a.sku_name,
               a.p_no,
                substr(a.station,6) station,
               a.slot_no,
               a.kp_no,
               a.feeder_sn,
               a.y_cpk,
               a.repair_time
          from AOI_SL a
         where line_id = '{LINE}AOI1'
           and to_date(time, 'yyyy-mm-dd hh24:mi:ss') >to_date('{START_TIME}','yyyy-mm-dd hh24:mi:ss')
           and to_date(time, 'yyyy-mm-dd hh24:mi:ss') <=to_date('{END_TIME}','yyyy-mm-dd hh24:mi:ss')
           and resultstatus = 'fail'
         group by a.error_position,
                  a.error_code,
                  a.topbutton,
                  a.line_id,
                  a.wo,
                  a.sku_name,
                  a.p_no,
                  a.station,
                  a.slot_no,
                  a.kp_no,
                  a.feeder_sn,
                  a.y_cpk,
                  a.repair_time)
 order by FailCount desc
";
                var data = db.RunSelect(strSql);
                var data1 = new List<object>();
                Dictionary<string, Dictionary<string, string>> temp = new Dictionary<string, Dictionary<string, string>>();
                for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                {
                    List<string> item = new List<string>();
                    item.Add(data.Tables[0].Rows[i]["error_position"].ToString());
                    item.Add(data.Tables[0].Rows[i]["FailCount"].ToString());
                    data1.Add(item);
                    var error_code = data.Tables[0].Rows[i]["error_code"].ToString();
                    if (!temp.ContainsKey(data.Tables[0].Rows[i]["error_code"].ToString()))
                    {
                        var I = new Dictionary<string, string>();
                        I.Add("name", error_code);
                        I.Add("value", "0");
                        temp.Add(error_code, I);
                    }
                    temp[error_code]["value"] = (int.Parse(temp[error_code]["value"]) +
                        int.Parse(data.Tables[0].Rows[i]["FailCount"].ToString())).ToString();

                }
                var data2 = new Dictionary<string, string>[temp.Values.Count];
                temp.Values.CopyTo(data2, 0);

                strSql =
                    $@"select *
  from (select a.error_position,
               substr( a.error_code,0,2) error_code,
               count(1) FailCount,
               a.topbutton,
               substr(a.line_id,0,5) line_id,
               substr(a.line_id,6) workstation,
               a.wo,
               a.sku_name,
               a.p_no,
                substr(a.station,6) station,
               a.slot_no,
               a.kp_no,
               a.feeder_sn,
               a.y_cpk,
               a.repair_time
          from AOI_SL a
         where line_id = '{LINE}AOI2'
           and to_date(time, 'yyyy-mm-dd hh24:mi:ss') >to_date('{START_TIME}','yyyy-mm-dd hh24:mi:ss')
           and to_date(time, 'yyyy-mm-dd hh24:mi:ss') <=to_date('{END_TIME}','yyyy-mm-dd hh24:mi:ss')
           and resultstatus = 'fail'
         group by a.error_position,
                  a.error_code,
                  a.topbutton,
                  a.line_id,
                  a.wo,
                  a.sku_name,
                  a.p_no,
                  a.station,
                  a.slot_no,
                  a.kp_no,
                  a.feeder_sn,
                  a.y_cpk,
                  a.repair_time)
 order by FailCount desc
";

                data = db.RunSelect(strSql);
                var data3 = new List<object>();
                temp = new Dictionary<string, Dictionary<string, string>>();
                for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                {
                    List<string> item = new List<string>();
                    item.Add(data.Tables[0].Rows[i]["error_position"].ToString());
                    item.Add(data.Tables[0].Rows[i]["FailCount"].ToString());
                    data3.Add(item);
                    var error_code = data.Tables[0].Rows[i]["error_code"].ToString();
                    if (!temp.ContainsKey(data.Tables[0].Rows[i]["error_code"].ToString()))
                    {
                        var I = new Dictionary<string, string>();
                        I.Add("name", error_code);
                        I.Add("value", "0");
                        temp.Add(error_code, I);
                    }
                    temp[error_code]["value"] = (int.Parse(temp[error_code]["value"]) +
                        int.Parse(data.Tables[0].Rows[i]["FailCount"].ToString())).ToString();

                }
                var data4 = new Dictionary<string, string>[temp.Values.Count];
                temp.Values.CopyTo(data4, 0);

                strSql =
                    $@"select *
  from (select a.error_position,
               substr( a.error_code,0,2) error_code,
               count(1) FailCount,
               a.topbutton,
               substr(a.line_id,0,5) line_id,
               substr(a.line_id,6) workstation,
               a.wo,
               a.sku_name,
               a.p_no,
                substr(a.station,6) station,
               a.slot_no,
               a.kp_no,
               a.feeder_sn,
               a.y_cpk,
               a.repair_time
          from AOI_SL a
         where line_id in ('{LINE}AOI1','{LINE}AOI2')
           and to_date(time, 'yyyy-mm-dd hh24:mi:ss') >to_date('{START_TIME}','yyyy-mm-dd hh24:mi:ss')
           and to_date(time, 'yyyy-mm-dd hh24:mi:ss') <=to_date('{END_TIME}','yyyy-mm-dd hh24:mi:ss')
           and resultstatus = 'fail'
         group by a.error_position,
                  a.error_code,
                  a.topbutton,
                  a.line_id,
                  a.wo,
                  a.sku_name,
                  a.p_no,
                  a.station,
                  a.slot_no,
                  a.kp_no,
                  a.feeder_sn,
                  a.y_cpk,
                  a.repair_time)
 order by FailCount desc
";

                data = db.RunSelect(strSql);
                var T = ConvertToJson.DataTableToJson(data.Tables[0]);
                StationReturn.Data = new { bar1 = data1, bar2 = data3,pie1=data2,pie2=data4 ,table = T};
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;

            }
            catch (Exception e)
            {
                
                throw e;
            }
            finally
            {
                this.DBPools["HWDAOI"].Return(db);
            }
        }

        public void GetPthWip(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = this.DBPools["SFCDB"].Borrow();
            try
            {
                string strSql =$@"";               

                strSql =$@"select skuno,workorderno,SUM(PTH) PTH,SUM(FQA) FQA,SUM(PRESS_FIT) PRESS_FIT,SUM(DX2) DX2,SUM(ICT) ICT,
                        SUM(STOCKIN) STOCKIN,SUM(JOBFINISH) JOBFINISH,SUM(FAILWIP) FAILWIP,SUM(REPAIRWIP) REPAIRWIP ,SUM(TRANSFORMATION) TRANSFORMATION from (
                        select skuno,workorderno, case station when 'PTH' then count(sn) else 0 end AS PTH,
                        case station when 'FQA' then count(sn) else 0 end AS FQA,
                            case station when 'PRESS_FIT' then count(sn) else 0 end AS PRESS_FIT,
                            case station when '5DX2' then count(sn) else 0 end AS DX2,
                                case station when 'ICT' then count(sn) else 0 end AS ICT,
                                case station when 'STOCKIN' then count(sn) else 0 end AS STOCKIN,
                                    case station when 'JOBFINISH' then count(sn) else 0 end AS JOBFINISH,
                                    case station when 'FAILWIP' then count(sn) else 0 end AS FAILWIP,
                                        case station when 'REPAIRWIP' then count(sn) else 0 end AS REPAIRWIP,
                                            case station when 'TRANSFORMATION' then count(sn) else 0 end AS TRANSFORMATION from (
                        select skuno,workorderno,next_station station,SN from r_sn where workorderno in('009100000847','009100000849','009100000853','009100000854') and repair_failed_flag =0 and valid_flag =1  
                        union 
                        select skuno,workorderno,'FAILWIP' station,SN from r_sn s where s.workorderno in('009100000847','009100000849','009100000853','009100000854') and s.repair_failed_flag =1
                        and not exists (select * from r_repair_transfer r where s.sn=r.sn and s.next_station=r.station_name and r.closed_flag=0)
                        union 
                        select skuno,workorderno,'REPAIRWIP'station,SN from r_sn s where s.workorderno in('009100000847','009100000849','009100000853','009100000854') and s.repair_failed_flag =1
                        and exists (select * from r_repair_transfer r where s.sn=r.sn and s.next_station=r.station_name and r.closed_flag=0)
                        union
                        select skuno,workorderno,'TRANSFORMATION'station,SN from r_sn s where s.workorderno in('009100000847','009100000849','009100000853','009100000854') and valid_flag=2)aa 
                        group by skuno,workorderno,station)bb GROUP BY skuno,workorderno";

                var data = db.RunSelect(strSql);
                var T = ConvertToJson.DataTableToJson(data.Tables[0]);
                StationReturn.Data = new { table = T };
                StationReturn.Message = "";
                StationReturn.Status = StationReturnStatusValue.Pass;

            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(db);
            }
        }
        public void GetDefectData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string DateFromTime = Data["DateFrom"].ToString();
                string Sql = $@"select Line, Station,
                               CREATEBY   AS WorkUser,
                               emp_name as WorkUserName,
                               DEFECT1,
                               DEFECT2,
                               CREATETIME AS OnTime,
                               ACTIONNAME AS OffTime  From r_station_scan_log r, c_user c where 
       r.CREATEBY = c.emp_no and r.SCANKEY = 'STATIONONLINE' and CREATETIME > to_date('{DateFromTime}' ,'YYYY/MM/DD HH24:MI:SS')
       and  r.DEFECT1 is not null or r.DEFECT2 is not null";
                var dt = sfcdb.ExecSelect(Sql, null).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");

                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dt;

            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }


        }
        public void GetDefectByUserData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string DateFromTime = Data["DateFrom"].ToString();
                string Sql = $@"select B.EMP_NAME,count(B.EMP_NAME) num from r_station_scan_log A ,C_USER  B where A.CREATEBY = B.EMP_NO AND 
                            (A.DEFECT1 is not null or A.DEFECT2 is not null) and   A.CREATETIME >to_date('{DateFromTime}','yyyy-mm-dd hh24:mi:ss')
                             and rownum <16 GROUP BY B.EMP_NAME  order by count(B.EMP_NAME) ";
                var dt = sfcdb.ExecSelect(Sql, null).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");

                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dt;

            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetDefectByLineData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string DateFromTime = Data["DateFrom"].ToString();
                string Sql = $@"select LINE,count(LINE) num from r_station_scan_log  where (DEFECT1 is not null or DEFECT2 is not null) and 
                                CREATETIME >to_date('{DateFromTime}','yyyy-mm-dd hh24:mi:ss')  and rownum <16 
                                GROUP BY LINE order by count(LINE) ";
                var dt = sfcdb.ExecSelect(Sql, null).Tables[0];

                if (dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");

                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = dt;

            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
}
