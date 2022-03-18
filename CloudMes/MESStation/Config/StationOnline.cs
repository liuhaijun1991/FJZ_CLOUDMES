using MESDBHelper;
using MESPubLab.MESStation;
using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using System.Data;
using MESPubLab.MESStation.MESReturnView.Public;
using MESPubLab.MESStation.LogicObject;
using MESDataObject.Common;
using SqlSugar;

namespace MESStation.Config
{
    class StationOnline : MesAPIBase
    {
        DateTime DateTime1 = DateTime.Parse(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd 8:00:00"));
        DateTime DateTime2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 20:00:00"));
        DateTime DateTime3 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        DateTime DateTime4 = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 8:00:00"));
        DateTime DateTime5 = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 20:00:00"));
        DateTime DateTime6 = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd 8:00:00"));

        DateTime N_Date_from, Ddate_Date_from, D_Date_from, N_Night_To, DNight_To, D_Night_To;

        public void CheckLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                string line = Data["Line"] == null ? "" : Data["Line"].ToString();
                T_R_STATION_OPERATOR t_r_station_operator = new T_R_STATION_OPERATOR(sfcdb, DBTYPE);
                R_STATION_OPERATOR r_station_operator = t_r_station_operator.CheckLine(line, sfcdb);
                if (r_station_operator == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210702105107", new string[] { line }));
                }
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = "請掃描工站";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void CheckStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_STATION_OPERATOR t_r_station_operator = new T_R_STATION_OPERATOR(sfcdb, DBTYPE);
                R_STATION_OPERATOR r_station_operator = t_r_station_operator.CheckStation(Data["Line"].ToString().ToUpper().Trim(), Data["Station"].ToString().ToUpper().Trim(), sfcdb);
                if (r_station_operator == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210702172050", new string[] { Data["Station"].ToString().ToUpper().Trim() }));
                }
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = "請掃描工號";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetLine(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                T_R_STATION_OPERATOR TCLT = new T_R_STATION_OPERATOR(sfcdb, DB_TYPE_ENUM.Oracle);
                StationReturn.Data = TCLT.GetLine(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void CheckUser(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string sql = string.Empty;
            string IsOnline = string.Empty;
            string Emp_Nmae = string.Empty;
            sfcdb = this.DBPools["SFCDB"].Borrow();
            sfcdb.ThrowSqlExeception = true;
            string Line = Data["Line"].ToString().ToUpper().Trim();
            string Station = Data["Station"].ToString().ToUpper().Trim();
            string User = Data["User"].ToString().ToUpper().Trim();
            string UserLevel = LoginUser.EMP_LEVEL;
            string Logonname = LoginUser.EMP_NO;
            T_R_STATION_SCAN_LOG UserScan = new T_R_STATION_SCAN_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
            Row_R_STATION_SCAN_LOG rowScan = (Row_R_STATION_SCAN_LOG)UserScan.NewRow();
            try
            {
                //掃描上線
                if (Station != "")
                {
                    //判斷工站類別
                    var ObjJob = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.LINE == Line && c.STATION == Station).ToList();
                    string IsTest = ObjJob[0].ISTEST;
                    string isEngneer = ObjJob[0].ISENGINEER;

                    //判斷掃進來的用戶是否在綫,'0'除外
                    var ObjOnline = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.OPERATOR == User && c.OPERATOR != "0").ToList();

                    //判斷該工站是否已經有人員在綫
                    var ObjScan = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.LINE == Line && c.STATION == Station && c.OPERATOR != null && !c.LINE.Contains("ZY")).ToList();
                    if (ObjScan.Count() == 1)
                    {
                        IsOnline = ObjScan[0].OPERATOR;
                    }
                    else
                    {
                        IsOnline = null;
                    }

                    //判斷該工站是否已經有人員在綫
                    var ObjUser = sfcdb.ORM.Queryable<C_USER>().Where(c => c.EMP_NO == User).ToList();

                    if (ObjUser.Count() > 0)
                    {
                        Emp_Nmae = ObjUser[0].EMP_NAME;
                        //判斷為產線員工
                        if (IsTest == "0" && isEngneer == "0")
                        {
                            if (ObjOnline.Count == 0 && ObjScan.Count == 0)
                            {
                                //支援線上最小且未有人員上線的線體
                                if (Line.Contains("ZY"))
                                {
                                    ObjScan = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.LINE == Line).OrderBy(c => c.ID, SqlSugar.OrderByType.Asc).ToList();
                                    for (int i = 0; i < ObjScan.Count(); i++)
                                    {
                                        string ZyStation = ObjScan[i].STATION;
                                        var ObjScanZY = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.LINE == Line && c.STATION == ZyStation && c.OPERATOR == null).ToList();
                                        if (ObjScanZY.Count > 0)
                                        {
                                            sfcdb.ORM.Updateable<R_STATION_OPERATOR>().SetColumns(r => new R_STATION_OPERATOR
                                            {
                                                OPERATOR = User,
                                                OPERATORNAME = Emp_Nmae,
                                                ONLINEDT = System.DateTime.Now
                                            })
                                            .Where(r => r.LINE == Line && r.STATION == ZyStation).ExecuteCommand();

                                            rowScan.SCANKEY = "STATIONONLINE";
                                            rowScan.LINE = Line;
                                            rowScan.STATION = Station;
                                            rowScan.CREATEBY = User;
                                            rowScan.CREATETIME = System.DateTime.Now;
                                            rowScan.CLASSNAME = Logonname;

                                            sfcdb.BeginTrain();
                                            sfcdb.ExecSQL(rowScan.GetInsertString(DB_TYPE_ENUM.Oracle));
                                            sfcdb.CommitTrain();
                                            //BingData(StationReturn);
                                            StationReturn.Status = StationReturnStatusValue.Pass;
                                            StationReturn.Data = "上線成功";
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    sfcdb.ORM.Updateable<R_STATION_OPERATOR>().SetColumns(r => new R_STATION_OPERATOR
                                    {
                                        OPERATOR = User,
                                        OPERATORNAME = Emp_Nmae,
                                        ONLINEDT = System.DateTime.Now
                                    })
                                   .Where(r => r.LINE == Line && r.STATION == Station).ExecuteCommand();

                                    rowScan.SCANKEY = "STATIONONLINE";
                                    rowScan.LINE = Line;
                                    rowScan.STATION = Station;
                                    rowScan.CREATEBY = User;
                                    rowScan.CREATETIME = System.DateTime.Now;
                                    rowScan.CLASSNAME = Logonname;

                                    sfcdb.BeginTrain();
                                    sfcdb.ExecSQL(rowScan.GetInsertString(DB_TYPE_ENUM.Oracle));
                                    sfcdb.CommitTrain();
                                    StationReturn.Status = StationReturnStatusValue.Pass;
                                    StationReturn.Data = "上線成功";
                                }
                            }
                            else
                            {
                                if (ObjOnline.Count() == 1)
                                {
                                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210706165740", new string[] { User }));
                                }
                                if (ObjScan.Count() == 1)
                                {
                                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210706170038", new string[] { Station }));
                                }
                            }
                        }
                        else if (isEngneer == "1")
                        {
                            if (UserLevel != "9")//非超級權限的用戶需要使用本人賬號登入
                            {
                                if (User == Logonname)
                                {
                                    LineLinkuser(IsOnline, Emp_Nmae, User, Line, Station, Logonname, StationReturn);
                                }
                                else
                                {
                                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210707105106", new string[] { User }));
                                }
                            }
                            else
                            {
                                LineLinkuser(IsOnline, Emp_Nmae, User, Line, Station, Logonname, StationReturn);
                            }
                        }
                        else
                        {
                            LineLinkuser(IsOnline, Emp_Nmae, User, Line, Station, Logonname, StationReturn);
                        }
                    }
                    else
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210706171235", new string[] { User }));
                    }
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210706171140", new string[] { Line }));

                }
                //BingData(StationReturn);
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;

                //BingData(StationReturn);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void LineLinkuser(string IsOnline, string Emp_Nmae, string User, string Line, string Station, string Logonname, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                if (IsOnline == null)
                {
                    sfcdb.ORM.Updateable<R_STATION_OPERATOR>().SetColumns(r => new R_STATION_OPERATOR
                    {
                        OPERATOR = User,
                        OPERATORNAME = Emp_Nmae,
                        ONLINEDT = System.DateTime.Now
                    })
                   .Where(r => r.LINE == Line && r.STATION == Station).ExecuteCommand();

                    T_R_STATION_SCAN_LOG UserScan = new T_R_STATION_SCAN_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                    Row_R_STATION_SCAN_LOG rowScan = (Row_R_STATION_SCAN_LOG)UserScan.NewRow();

                    rowScan.SCANKEY = "STATIONONLINE";
                    rowScan.LINE = Line;
                    rowScan.STATION = Station;
                    rowScan.CREATEBY = User;
                    rowScan.CREATETIME = System.DateTime.Now;
                    rowScan.CLASSNAME = Logonname;

                    sfcdb.BeginTrain();
                    sfcdb.ExecSQL(rowScan.GetInsertString(DB_TYPE_ENUM.Oracle));
                    sfcdb.CommitTrain();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "上線成功";
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210706170038", new string[] { Station }));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void BingData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                string sql = string.Empty;
                var result = 0;
                T_R_STATION_SCAN_LOG UserScan = new T_R_STATION_SCAN_LOG(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_R_STATION_SCAN_LOG rowScan = (Row_R_STATION_SCAN_LOG)UserScan.NewRow();

                //VN白晚班時間不一樣
                if (BU == "VNDCN" || BU == "VNJUNIPER")
                {
                    DateTime1 = DateTime.Parse(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd 7:30:00"));
                    DateTime2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 19:30:00"));
                    DateTime4 = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 7:30:00"));
                    DateTime5 = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 19:30:00"));
                    DateTime6 = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd 7:30:00"));
                }
                //自動下線白班人員
                if (DateTime3 > DateTime1 && DateTime3 < DateTime2)
                {
                    var ObjRator = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.ONLINEDT < DateTime1).ToList();

                    for (int j = 0; j < ObjRator.Count(); j++)
                    {
                        sfcdb.ORM.Updateable<R_STATION_OPERATOR>().SetColumns(r => new R_STATION_OPERATOR
                        {
                            OPERATOR = "",
                            OPERATORNAME = "",
                            ONLINEDT = null
                        })
                        .Where(r => r.OPERATOR == ObjRator[j].OPERATOR && r.ONLINEDT < DateTime1).ExecuteCommand();

                        sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{ObjRator[j].OPERATOR}' and  CREATETIME  between TO_DATE ('{DateTime5}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime1}','YYYY/MM/DD HH24:MI:SS') ";
                        result = sfcdb.ExecSqlNoReturn(sql, null);
                    }
                }
                //自動下線晚班上半夜
                else if (DateTime3 > DateTime2 && DateTime3 < DateTime6)
                {
                    var ObjRator = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.ONLINEDT < DateTime2).ToList();
                    for (int j = 0; j < ObjRator.Count(); j++)
                    {
                        sfcdb.ORM.Updateable<R_STATION_OPERATOR>().SetColumns(r => new R_STATION_OPERATOR
                        {
                            OPERATOR = "",
                            OPERATORNAME = "",
                            ONLINEDT = null
                        })
                        .Where(r => r.OPERATOR == ObjRator[j].OPERATOR && r.ONLINEDT < DateTime2).ExecuteCommand();

                        sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{ObjRator[j].OPERATOR}' and  CREATETIME between TO_DATE ('{DateTime1}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime2}','YYYY/MM/DD HH24:MI:SS') ";
                        result = sfcdb.ExecSqlNoReturn(sql, null);

                    }
                }
                //自動下線晚班下半夜
                else if (DateTime3 > DateTime5 && DateTime3 < DateTime1)
                {
                    var ObjRator = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.ONLINEDT < DateTime5).ToList();
                    for (int j = 0; j < ObjRator.Count(); j++)
                    {
                        sfcdb.ORM.Updateable<R_STATION_OPERATOR>().SetColumns(r => new R_STATION_OPERATOR
                        {
                            OPERATOR = "",
                            OPERATORNAME = "",
                            ONLINEDT = null
                        })
                        .Where(r => r.OPERATOR == ObjRator[j].OPERATOR && r.ONLINEDT < DateTime5).ExecuteCommand();

                        sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{ObjRator[j].OPERATOR}' and  CREATETIME between TO_DATE ('{DateTime4}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime5}','YYYY/MM/DD HH24:MI:SS') ";
                        result = sfcdb.ExecSqlNoReturn(sql, null);

                    }

                }

                if (DateTime3 > DateTime1 && DateTime3 < DateTime2)
                {
                    sql = $@"SELECT ID, Line,Station,Operator WorkUser,operatorName WorkUserName FROM r_station_operator WHERE onlinedt BETWEEN TO_DATE ('{DateTime1}','YYYY/MM/DD HH24:MI:SS')  AND TO_DATE ('{DateTime2}','YYYY/MM/DD HH24:MI:SS')  order by line desc";
                }
                else
                {
                    sql = $@"SELECT ID, Line,Station,Operator WorkUser,operatorName WorkUserName FROM r_station_operator WHERE onlinedt < TO_DATE ('{DateTime6}','YYYY/MM/DD HH24:MI:SS')  order by line desc";
                }
                DataTable total_dt = sfcdb.ExecSelect(sql, null).Tables[0];
                //if (total_dt.Rows.Count == 0)
                //{
                //    throw new Exception($@"No Data!");
                //}
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(total_dt.Rows.Count);
                //StationReturn.Data = new { Total = total_dt.Rows.Count, Rows = total_dt };
                StationReturn.Data = total_dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void LsData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                string Line = Data["LineData"].ToString();
                string User = Data["UserData"].ToString();
                string Shift = Data["ShiftData"].ToString();
                string Sn = Data["SnData"].ToString();
                string sql = "";
                string DateFromTime = Data["DateFrom"].ToString();
                string DateToTime = Data["DateTo"].ToString();
                string DEFECT = Data["DEFECT"].ToString().ToUpper();
             

                if (DateFromTime != "" && DateToTime != "")
                {



                    //DateTime Ndate_from = DateTime.Parse(Convert.ToDateTime(Data["DateFrom"].ToString().ToUpper().Trim() + " 07:59:59").AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                    N_Date_from = DateTime.Parse(Convert.ToDateTime(Data["DateFrom"].ToString().ToUpper().Trim() + " 20:00:00").AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                    Ddate_Date_from = DateTime.Parse(Convert.ToDateTime(Data["DateFrom"].ToString().ToUpper().Trim() + " 20:00:00").ToString("yyyy-MM-dd HH:mm:ss"));
                    D_Date_from = DateTime.Parse(Convert.ToDateTime(Data["DateFrom"].ToString().ToUpper().Trim() + " 08:00:00").ToString("yyyy-MM-dd HH:mm:ss"));
                    //DateTime NNight_To = DateTime.Parse(Convert.ToDateTime(Data["DateTo"].ToString().ToUpper().Trim() + " 07:59:59").AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                    N_Night_To = DateTime.Parse(Convert.ToDateTime(Data["DateTo"].ToString().ToUpper().Trim() + " 08:00:00").AddDays(+1).ToString("yyyy-MM-dd HH:mm:ss"));
                    DNight_To = DateTime.Parse(Convert.ToDateTime(Data["DateTo"].ToString().ToUpper().Trim() + " 20:00:00").ToString("yyyy-MM-dd HH:mm:ss"));
                    D_Night_To = DateTime.Parse(Convert.ToDateTime(Data["DateTo"].ToString().ToUpper().Trim() + " 08:00:00").ToString("yyyy-MM-dd HH:mm:ss"));

                    //VN白晚班時間不一樣
                    if (BU == "VNDCN" || BU == "VNJUNIPER")
                    {
                        N_Date_from = DateTime.Parse(Convert.ToDateTime(Data["DateFrom"].ToString().ToUpper().Trim() + " 19:30:00").AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                        Ddate_Date_from = DateTime.Parse(Convert.ToDateTime(Data["DateFrom"].ToString().ToUpper().Trim() + " 19:30:00").ToString("yyyy-MM-dd HH:mm:ss"));
                        D_Date_from = DateTime.Parse(Convert.ToDateTime(Data["DateFrom"].ToString().ToUpper().Trim() + " 07:30:00").ToString("yyyy-MM-dd HH:mm:ss"));
                        N_Night_To = DateTime.Parse(Convert.ToDateTime(Data["DateTo"].ToString().ToUpper().Trim() + " 07:30:00").AddDays(+1).ToString("yyyy-MM-dd HH:mm:ss"));
                        DNight_To = DateTime.Parse(Convert.ToDateTime(Data["DateTo"].ToString().ToUpper().Trim() + " 19:30:00").ToString("yyyy-MM-dd HH:mm:ss"));
                        D_Night_To = DateTime.Parse(Convert.ToDateTime(Data["DateTo"].ToString().ToUpper().Trim() + " 07:30:00").ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }

                if (Sn == "" && DateFromTime == "" && DateToTime == "" && DEFECT=="")
                {
                    throw new Exception($@" Too many query conditions,Please select a new query condition !");
                }


                //check SN 檢查SN
                if (Sn == "")
                {

                    sql = $@"select Line,
                               Station,
                               CREATEBY   AS WorkUser,
                               emp_name as WorkUserName,
                               DEFECT1,
                               DEFECT2,
                               CREATETIME AS OnTime,
                               bbb.ACTIONNAME AS OffTime
                          from r_station_scan_log bbb, c_user b
                         where bbb.SCANKEY = 'STATIONONLINE'
                           AND bbb.CREATEBY = b.emp_no  ";
                }
                else
                {

                    sql = $@"SELECT distinct bbb.Line,
                                    bbb.Station,
                                    bbb.CREATEBY   WorkUser,
                                    c.emp_name WorkUserName,
                                    bbb.createtime OnTime,
                                    bbb.ACTIONNAME OffTime
                      FROM (SELECT SN,
                                   case
                                     when to_char(EDIT_TIME, 'HH') >= 8 and
                                          to_char(EDIT_TIME, 'HH') >= 20 then
                                      to_char(EDIT_TIME, 'YYYY/MM/DD')
                                     when to_char(EDIT_TIME, 'HH') < 8 then
                                      to_char(EDIT_TIME, 'YYYY/MM/DD')
                                     when to_char(EDIT_TIME, 'HH') >= 20 then
                                      to_char(EDIT_TIME + 1, 'YYYY/MM/DD')
                                   end onlinetime,
                                   LINE
                              from (SELECT a.*,
                                           RANK() OVER(PARTITION BY LINE, STATION_NAME ORDER BY EDIT_TIME ASC) tt
                                      FROM r_sn_station_detail a
                                     WHERE sn = '" + Sn + "') aa "
                            + @" where aa.tt = '1') aaa
                      LEFT JOIN (select bb.line,
                                        bb.station,
                                        bb.createtime,
                                        bb.ACTIONNAME,
                                        bb.CREATEBY,
                                        case
                                          when to_char(CREATETIME, 'HH') >= 8 and
                                               to_char(CREATETIME, 'HH') >= 20 then
                                           to_char(CREATETIME, 'YYYY/MM/DD')
                                          when to_char(CREATETIME, 'HH') < 8 then
                                           to_char(CREATETIME, 'YYYY/MM/DD')
                                          when to_char(CREATETIME, 'HH') >= 20 then
                                           to_char(CREATETIME + 1, 'YYYY/MM/DD')
                                        end OffTime
                                   from (select b.*, CREATETIME AS OffTime
                                           from r_station_scan_log b
                                          where SCANKEY = 'STATIONONLINE'
                                            and ACTIONNAME IS NOT null) bb) bbb
                        ON aaa.onlinetime = bbb.offtime
                       AND aaa.line = bbb.line
                      LEFT JOIN c_user c
                        on bbb.CREATEBY = c.emp_no
                     where bbb.Line is not null ";

                }

                // check User  查檢員工

                if (User != "")

                {
                    sql = sql + $@" and bbb.CREATEBY='{User}' ";

                }

                //check Line 查檢腺體

                if (Line != "ALL")

                {
                    sql = sql + $@" and bbb.LINE='{Line}'";

                }
                //檢查時間
                if (DateFromTime != "" && DateToTime != "")
                {
                    //check Shift  檢查班別
                    if (Shift == "ALL")
                    {
                        sql = sql + $@" and bbb.CREATETIME BETWEEN TO_DATE ('{D_Date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{N_Night_To}','YYYY/MM/DD HH24:MI:SS')  ";

                    }
                    else
                    {

                        // check Date 白班
                        if (Shift == "Date")
                        {

                            sql = sql + $@" and bbb.CREATETIME BETWEEN TO_DATE ('{D_Date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{DNight_To}','YYYY/MM/DD HH24:MI:SS')   ";

                        }
                        else // check Night 夜班
                        {

                            sql = sql + $@" and bbb.CREATETIME BETWEEN TO_DATE ('{N_Date_from}','YYYY/MM/DD HH24:MI:SS') AND TO_DATE ('{D_Night_To}','YYYY/MM/DD HH24:MI:SS')   ";

                        }
                    }
                }


                // check DEFECT  查檢不良備註

                if (DEFECT != "")

                {
                    if (DEFECT == "ALL") {

                        sql = sql + $@" and ( bbb.DEFECT1  is not  null   OR  bbb.DEFECT2 is not  null  ) ";
                    }
                    else
                    {
                        sql = sql + $@" and ( bbb.DEFECT1 like '%{DEFECT}%'  OR  bbb.DEFECT2 like '%{DEFECT}%'  ) ";
                    }

                }



                sql = sql + $@" order by bbb.line";

                DataTable total_dt = sfcdb.ExecSelect(sql, null).Tables[0];

                if (total_dt.Rows.Count == 0)
                {
                    throw new Exception($@"No Data!");

                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara.Add(total_dt.Rows.Count);
                StationReturn.Data = total_dt;
            }
            catch (Exception ee)
            {

                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void Delete(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                string sql = string.Empty;
                var result = 0;
                string strLine = Data["LINE"] == null ? "" : Data["LINE"].ToString();
                string strStation = Data["STATION"] == null ? "" : Data["STATION"].ToString();
                string strUser = Data["WORKUSER"] == null ? "" : Data["WORKUSER"].ToString();
                string strName = LoginUser.EMP_NO;


                var ObjJob = sfcdb.ORM.Queryable<R_STATION_OPERATOR>().Where(c => c.LINE == strLine && c.STATION == strStation).ToList();
                string IsTest = ObjJob[0].ISTEST;
                string isEngneer = ObjJob[0].ISENGINEER;

                //VN白晚班時間不一樣
                if (BU == "VNDCN" || BU == "VNJUNIPER")
                {
                    DateTime1 = DateTime.Parse(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd 7:30:00"));
                    DateTime2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 19:30:00"));
                    DateTime4 = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 7:30:00"));
                    DateTime5 = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 19:30:00"));
                    DateTime6 = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd 7:30:00"));
                }

                if (IsTest == "0" && isEngneer == "0")
                {
                    sfcdb.ORM.Updateable<R_STATION_OPERATOR>().SetColumns(r => new R_STATION_OPERATOR
                    {
                        OPERATOR = "",
                        OPERATORNAME = "",
                        ONLINEDT = null
                    })
                    .Where(r => r.OPERATOR == strUser && r.LINE == strLine && r.STATION == strStation).ExecuteCommand();

                    if (DateTime3 > DateTime1 && DateTime3 < DateTime2)//白班
                    {
                        sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{strUser}' and line='{strLine}' and station='{strStation}' and  CREATETIME  between TO_DATE ('{DateTime1}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime2}','YYYY/MM/DD HH24:MI:SS') ";

                    }
                    if (DateTime3 > DateTime2 && DateTime3 < DateTime6)//晚班上半夜
                    {
                        sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{strUser}' and line='{strLine}' and station='{strStation}' and  CREATETIME  between TO_DATE ('{DateTime2}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime6}','YYYY/MM/DD HH24:MI:SS') ";
                    }
                    if (DateTime3 > DateTime5 && DateTime3 < DateTime1)//晚班下半夜
                    {
                        sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{strUser}' and line='{strLine}' and station='{strStation}' and  CREATETIME  between TO_DATE ('{DateTime5}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime1}','YYYY/MM/DD HH24:MI:SS') ";
                    }
                    result = sfcdb.ExecSqlNoReturn(sql, null);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "用戶" + strUser + ",線體" + strLine + "，工站" + strStation + "已成功下線";

                }
                else
                {
                    if (strUser == strName)
                    {
                        sfcdb.ORM.Updateable<R_STATION_OPERATOR>().SetColumns(r => new R_STATION_OPERATOR
                        {
                            OPERATOR = "",
                            OPERATORNAME = "",
                            ONLINEDT = null
                        })
                        .Where(r => r.OPERATOR == strUser && r.LINE == strLine && r.STATION == strStation).ExecuteCommand();
                        if (DateTime3 > DateTime1 && DateTime3 < DateTime2)//白班
                        {
                            sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{strUser}' and line='{strLine}' and station='{strStation}' and  CREATETIME  between TO_DATE ('{DateTime1}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime2}','YYYY/MM/DD HH24:MI:SS') ";
                            result = sfcdb.ExecSqlNoReturn(sql, null);
                        }
                        if (DateTime3 > DateTime2 && DateTime3 < DateTime6)//晚班上半夜
                        {
                            sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{strUser}' and line='{strLine}' and station='{strStation}' and  CREATETIME  between TO_DATE ('{DateTime2}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime6}','YYYY/MM/DD HH24:MI:SS') ";
                            result = sfcdb.ExecSqlNoReturn(sql, null);
                        }
                        if (DateTime3 > DateTime5 && DateTime3 < DateTime1)//晚班下半夜
                        {
                            sql = $@"update r_station_scan_log set ACTIONNAME='{DateTime3}' where SCANKEY='STATIONONLINE' and CREATEBY='{strUser}' and line='{strLine}' and station='{strStation}' and  CREATETIME  between TO_DATE ('{DateTime5}','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('{DateTime1}','YYYY/MM/DD HH24:MI:SS') ";
                            result = sfcdb.ExecSqlNoReturn(sql, null);
                        }
                        result = sfcdb.ExecSqlNoReturn(sql, null);
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Data = "用戶" + strUser + ",線體" + strLine + "，工站" + strStation + "已成功下線";
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "只能下線本人上線的線體工站";
                    }
                }
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }
        public void Edit(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec sfcdb = null;
            DateTime DateFromTime;
            DateTime DateToTime;
            string UserLevel = LoginUser.EMP_LEVEL;
            string Logonname = LoginUser.EMP_NO;

            try
            {
                string line = Data["LINE"].ToString().Trim();
                string station = Data["STATION"].ToString().Trim();
                string workuser = Data["WORKUSER"].ToString().Trim();
                string worksername = Data["WORKUSERNAME"].ToString().Trim();
                string defect1 = Data["DEFECT1"].ToString().Trim();
                string defect2 = Data["DEFECT2"].ToString().Trim();

                if (Data["ONTIME"].ToString() != "")
                {
                     DateFromTime = DateTime.Parse(Convert.ToDateTime(Data["ONTIME"].ToString().ToUpper().Trim()).ToString("yyyy-MM-dd HH:mm:ss"));//CREATETIME
                }
                else
                {
                     DateFromTime = DateTime.Parse(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd 8:00:00"));
                }
                if (Data["OFFTIME"].ToString() != "")
                {
   
                     DateToTime = DateTime.Parse(Convert.ToDateTime(Data["OFFTIME"].ToString().ToUpper().Trim()).ToString("yyyy-MM-dd HH:mm:ss"));//ACTIONNAME
                }
                else
                {
                  
                     DateToTime = DateTime.Parse(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd 20:00:00"));//ACTIONNAME
                }

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                string sql = string.Empty;
                var result = 0;

                sql = $@"update r_station_scan_log set  DEFECT1='"+defect1 +"',DEFECT2='" + defect2 + "'" + ",CLASSNAME='"+ Logonname + "'" +
                            " where SCANKEY='STATIONONLINE' and LINE='" + line + "' and station='"+ station + "' and CREATEBY='"+workuser+"'  and  " +
                            " CREATETIME  between TO_DATE ('"+ DateFromTime + "','YYYY/MM/DD HH24:MI:SS') and TO_DATE ('"+ DateToTime + "','YYYY/MM/DD HH24:MI:SS') ";

                result = sfcdb.ExecSqlNoReturn(sql, null);


                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "Workuser:" + workuser + ", Line:" + line + ", Station:" + station + ",   Edit  OK !";


            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                StationReturn.Data = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }


        }

        /// <summary>
        ///檢查編輯權限  check privilege    C_USER_PRIVILEGE  StationOnlineEdit
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CheckEditPrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_USER_PRIVILEGE t_c_user_privilege = new T_C_USER_PRIVILEGE(SFCDB, this.DBTYPE);
                bool bPrivilege = t_c_user_privilege.CheckpPivilegeByName(SFCDB, "StationOnlineEdit", LoginUser.EMP_NO);
                if (LoginUser.EMP_LEVEL == "9" || bPrivilege)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "No Privilege Edit {StationOnlineEdit}!";
                }
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
    }
}
