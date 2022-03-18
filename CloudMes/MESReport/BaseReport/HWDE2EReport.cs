using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.BaseReport
{
    //SN 信息報表
    public class HWDE2EReport : ReportBase
    {        
        ReportInput typeInput = new ReportInput()
        {
            Name = "Type",
            InputType = "Select",
            Value = "SMT",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = new string[] { "SMT" }
        };
        public HWDE2EReport()
        {
            Inputs.Add(typeInput);                       
        }

        public override void Run()
        {
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            OleExec apdb = DBPools["APDB"].Borrow();
            DateTime onlineTime = DateTime.MinValue;
            try
            {
                DataTable Data = new DataTable();
                //Data.Columns.Add("SMT線體");
                //Data.Columns.Add("工單號");
                //Data.Columns.Add("料號");
                //Data.Columns.Add("工單批量");
                //Data.Columns.Add("UPH");
                //Data.Columns.Add("Release時間");
                //Data.Columns.Add("工單上線及時性預警");
                //Data.Columns.Add("下線時間");
                //Data.Columns.Add("完成數量");
                //Data.Columns.Add("完成率");
                //Data.Columns.Add("SMT完工預警");
                //Data.Columns.Add("測試上線時間預警");
                //Data.Columns.Add("測試完成數量");
                //Data.Columns.Add("測試完成時間預警");
                //Data.Columns.Add("SMT工單關結及時預警");
                //Data.Columns.Add("任務令");

                Data.Columns.Add("SMTLine");
                Data.Columns.Add("Workordeno");
                Data.Columns.Add("Skuno");
                Data.Columns.Add("WorkordenoBatch");
                Data.Columns.Add("UPH");
                Data.Columns.Add("ReleaseDate");
                Data.Columns.Add("WorkordenoOnlineWarning");
                Data.Columns.Add("OfflineDate");
                Data.Columns.Add("FinishQty");
                Data.Columns.Add("FinishRate");
                Data.Columns.Add("SMTFinishWarning");
                Data.Columns.Add("TestOnlineWarning");
                Data.Columns.Add("TestFinishQty");
                Data.Columns.Add("TestFinishWarning");
                Data.Columns.Add("SMTWorkordeClosedWarning");
                Data.Columns.Add("TaskNo");


                //查詢未關閉的工單 //只查SMT工單怎么查？
                string strSql = @"SELECT WORKORDERNO, SKUNO, RELEASE_DATE, WORKORDER_QTY, FINISHED_QTY, CUST_ORDER_NO, SYSDATE
                              FROM R_WO_BASE
                             WHERE SUBSTR(WORKORDERNO, 1, 5) IN('00251', '00254')
                               AND CLOSED_FLAG = 0
                               AND RELEASE_DATE > TO_DATE('2018/05/01','YYYY/MM/DD')
                               --AND INPUT_QTY > 0
                             ORDER BY WORKORDERNO";

                foreach (DataRow dr in sfcdb.RunSelect(strSql).Tables[0].Rows)
                {
                    DataRow data = Data.NewRow();
                    Data.Rows.Add(data);
                    string strWo = dr["WORKORDERNO"].ToString();
                    //data["工單號"] = strWo;
                    //data["料號"] = dr["SKUNO"].ToString();
                    //data["工單批量"] = dr["WORKORDER_QTY"].ToString();
                    //data["Release時間"] = dr["RELEASE_DATE"].ToString

                    data["Workordeno"] = strWo;
                    data["Skuno"] = dr["SKUNO"].ToString();
                    data["WorkordenoBatch"] = dr["WORKORDER_QTY"].ToString();
                    data["ReleaseDate"] = dr["RELEASE_DATE"].ToString();

                    if (dr["RELEASE_DATE"].ToString().Trim() == "")
                    {
                        // data["Release時間"] = "Err:無Release時間";
                        data["ReleaseDate"] = "Err:NO ReleaseDate record";
                        continue;
                    }

                    DateTime releaseTime = DateTime.Parse(dr["RELEASE_DATE"].ToString());
                    DateTime nowTime = DateTime.Parse(dr["SYSDATE"].ToString());

                   // data["任務令"] = dr["CUST_ORDER_NO"].ToString();
                    data["TaskNo"] = dr["CUST_ORDER_NO"].ToString();

                    strSql = "SELECT COUNT(DISTINCT SN) FROM R_SN WHERE WORKORDERNO='" + strWo + "' AND COMPLETED_FLAG='1'";
                    //data["測試完成數量"] = sfcdb.ExecSqlReturn(strSql);
                    data["TestFinishQty"] = sfcdb.ExecSqlReturn(strSql);


                    strSql = "SELECT COUNT(DISTINCT SN) FROM R_SN_STATION_DETAIL WHERE WORKORDERNO='" + strWo + "' AND STATION_NAME='BIP'";
                    int bipFinishQty = Int32.Parse(sfcdb.ExecSqlReturn(strSql));
                    //data["完成數量"] = bipFinishQty.ToString();
                    //data["完成率"] = ConvertTimeHour((((float)bipFinishQty / Int32.Parse(dr["WORKORDER_QTY"].ToString())) * 100).ToString()) + "%";
                    data["FinishQty"] = bipFinishQty.ToString();
                    data["FinishRate"] = ConvertTimeHour((((float)bipFinishQty / Int32.Parse(dr["WORKORDER_QTY"].ToString())) * 100).ToString()) + "%";



                    strSql = $@"SELECT MIN(LINE) LINE, MIN(EDIT_TIME) ONLINE_TIME
                          FROM R_SN_STATION_DETAIL
                         WHERE WORKORDERNO = '{strWo}'
                           AND STATION_NAME IN('PRINT1', 'LINK')
                           AND EDIT_TIME IN
                               (SELECT MIN(EDIT_TIME)
                                  FROM R_SN_STATION_DETAIL
                                 WHERE WORKORDERNO = '{strWo}'
                                   AND STATION_NAME IN('PRINT1', 'LINK'))";
                    //上線超時計算
                    DataTable dt = sfcdb.RunSelect(strSql).Tables[0];
                    //增加ONLINE_TIME不為空判斷，否則導致異常 fyq20180523
                    if (dt.Rows.Count > 0 && dt.Rows[0]["ONLINE_TIME"].ToString() != "")
                    {
                        //data["SMT線體"] = dt.Rows[0]["LINE"].ToString();

                        data["SMTLine"] = dt.Rows[0]["LINE"].ToString();

                        onlineTime = DateTime.Parse(dt.Rows[0]["ONLINE_TIME"].ToString());

                        if (dt.Rows[0]["LINE"].ToString() == "")
                        {
                            //data["工單上線及時性預警"] = "未上線";

                            data["WorkordenoOnlineWarning"] = "NO Online";

                            //TimeSpan ts = DateTime.Now - relTime;
                            TimeSpan ts = nowTime - releaseTime;
                            if (ts.TotalHours > 16)
                            {
                                
                                //data["工單上線及時性預警"] = data["工單上線及時性預警"].ToString() +
                                //    "(已超時" + ConvertTimeHour((ts.TotalHours - 16).ToString()) + "小時)";

                                data["WorkordenoOnlineWarning"] = data["WorkordenoOnlineWarning"].ToString() +
                                  "(overtime" + ConvertTimeHour((ts.TotalHours - 16).ToString()) + "hours)";
                            }
                        }
                        else
                        {
                            // data["工單上線及時性預警"] = "已上線";

                            data["WorkordenoOnlineWarning"] = "Online";

                            TimeSpan ts = onlineTime - releaseTime;
                            if (ts.TotalHours > 16)
                            {
                                //data["工單上線及時性預警"] = data["工單上線及時性預警"].ToString() + "(已超時)";
                                data["WorkordenoOnlineWarning"] = data["WorkordenoOnlineWarning"].ToString() + "(overtime)";
                            }
                        }
                    }

                    strSql = $@"SELECT STATION_NAME
                              FROM (SELECT A.STATION_NAME, A.SEQ_NO
                                      FROM C_ROUTE_DETAIL A,
                                           R_WO_BASE B,
                                           (SELECT M.ROUTE_ID, M.STATION_NAME, M.SEQ_NO
                                              FROM C_ROUTE_DETAIL M, R_WO_BASE N
                                             WHERE M.ROUTE_ID = N.ROUTE_ID
                                               AND M.STATION_NAME IN ('P_FQC', 'SMT_FQC')
                                               AND N.WORKORDERNO = '{strWo}') C
                                     WHERE A.ROUTE_ID = B.ROUTE_ID
                                       AND A.ROUTE_ID = C.ROUTE_ID(+)
                                       AND (A.SEQ_NO < C.SEQ_NO OR
                                           A.STATION_NAME IN ('VI1', 'VI2', 'BIP'))
                                       AND B.WORKORDERNO = '{strWo}'
                                     ORDER BY A.SEQ_NO DESC)
                             WHERE ROWNUM = 1";
                    //獲取下線工站
                    string strOfflineStation = sfcdb.ExecSqlReturn(strSql);

                    strSql = $@"SELECT TO_CHAR(MAX(EDIT_TIME), 'YYYY/MM/DD HH24:MI:SS') OFFLINE_TIME
                              FROM R_SN_STATION_DETAIL
                             WHERE WORKORDERNO = '{strWo}'
                               AND STATION_NAME = '{strOfflineStation}'";
                    //  data["下線時間"] = sfcdb.ExecSqlReturn(strSql);

                    data["OfflineDate"] = sfcdb.ExecSqlReturn(strSql);

                    


                    strSql = $@"SELECT COUNT(DISTINCT SN) OFFQTY
                              FROM R_SN_STATION_DETAIL
                             WHERE WORKORDERNO = '{strWo}'
                               AND STATION_NAME = '{strOfflineStation}'
                               AND VALID_FLAG = '1'";
                    int offlineCount = Int32.Parse(sfcdb.ExecSqlReturn(strSql));

                    //  if (data["SMT線體"].ToString() != "")
                    if (data["SMTLine"].ToString() != "")
                    {
                        //strSql = $@"SELECT MAX(UPH_QTY)
                        //      FROM MES1.C_PNO_UPH
                        //     WHERE P_NO = '{data["料號"].ToString()}'
                        //       AND LINE_NAME = '{data["SMT線體"].ToString()}'
                        //       AND PROCESS_FLAG = 'B/T'";
                        strSql = $@"SELECT MAX(UPH_QTY)
                              FROM MES1.C_PNO_UPH
                             WHERE P_NO = '{data["Skuno"].ToString()}'
                               AND LINE_NAME = '{data["SMTLine"].ToString()}'
                               AND PROCESS_FLAG = 'B/T'";

                        data["UPH"] = apdb.ExecSqlReturn(strSql);

                        string strUPH = data["UPH"].ToString();
                        int indexPoint = strUPH.IndexOf('.');
                        if (indexPoint >= 0)
                        {
                            data["UPH"] = strUPH.Substring(0, indexPoint);
                        }

                        if (data["UPH"].ToString() == "")
                        {
                            //data["UPH"] = "未配置UPH";
                            data["UPH"] = "Not configured UPH";
                        }
                    }

                    try
                    {
                        double uph = double.Parse(data["UPH"].ToString());
                        //double uphTime = Int32.Parse(data["工單批量"].ToString()) / (uph * 0.8);
                        double uphTime = Int32.Parse(data["WorkordenoBatch"].ToString()) / (uph * 0.8);
                        
                       // if (offlineCount == Int32.Parse(data["工單批量"].ToString())
                      if (offlineCount == Int32.Parse(data["WorkordenoBatch"].ToString()))
                        {
                            // data["SMT完工預警"] = "已完工";

                            data["SMTFinishWarning"] = "Completed";
                        }
                        else
                        {
                            //data["SMT完工預警"] = "未完工";
                            data["SMTFinishWarning"] = "unfinished";
                        }

                        // if (data["下線時間"].ToString() != "" && onlineTime != DateTime.MinValue)
                        if (data["OfflineDate"].ToString() != "" && onlineTime != DateTime.MinValue)
                        {
                            // DateTime offlineTime = DateTime.Parse(data["下線時間"].ToString());
                            DateTime offlineTime = DateTime.Parse(data["OfflineDate"].ToString());
                            TimeSpan ts = offlineTime - onlineTime;
                            if (ts.TotalHours - uphTime > 3)
                            {
                                // data["SMT完工預警"] = data["SMT完工預警"] + "(已超時" + ConvertTimeHour((ts.TotalHours - uphTime - 3).ToString()) + "小時)";
                                data["SMTFinishWarning"] = data["SMTFinishWarning"] + "(overtime" + ConvertTimeHour((ts.TotalHours - uphTime - 3).ToString()) + "hours)";
                            }
                        }
                    }
                    catch
                    { }

                    strSql = $@"SELECT COUNT(SN)
                              FROM R_SN
                             WHERE SN IN(SELECT SN
                                            FROM R_SN_STATION_DETAIL
                                           WHERE STATION_NAME = 'BIP'
                                             AND WORKORDERNO = '{strWo}'
                                             AND SYSDATE - EDIT_TIME > 0.5)
                               AND COMPLETED_FLAG = '0'";

                    //data["測試完成時間預警"] = sfcdb.ExecSqlReturn(strSql); //時間為什么是數量呢？

                    data["TestFinishWarning"] = sfcdb.ExecSqlReturn(strSql); //時間為什么是數量呢？

                    strSql = $@"SELECT SN, EDIT_TIME
                              FROM R_SN_STATION_DETAIL
                             WHERE EDIT_TIME IN (SELECT MIN(EDIT_TIME)
                                                   FROM R_SN_STATION_DETAIL
                                                  WHERE STATION_NAME = 'BIP'
                                                    AND WORKORDERNO = '{strWo}'
                                                    AND (SYSDATE - EDIT_TIME) > (1 / 6))
                               AND WORKORDERNO = '{strWo}'";

                    DataTable dtFirstBIP = sfcdb.RunSelect(strSql).Tables[0];

                    if (dtFirstBIP.Rows.Count > 0)
                    {
                        DateTime fistBIPTime = DateTime.Parse(dtFirstBIP.Rows[0]["EDIT_TIME"].ToString());
                        //TimeSpan ts = DateTime.Now - fistBIPTime;
                        TimeSpan ts = nowTime - fistBIPTime;

                        //增加工單條件，因為一個SN可對應兩工單(SMT工單和BIP分板工單) fyq20180522
                        strSql = $@"SELECT COMPLETED_FLAG
                                  FROM R_SN
                                 WHERE SN = '{dtFirstBIP.Rows[0]["SN"].ToString()}'
                                   AND WORKORDERNO = '{strWo}'";
                        if (sfcdb.ExecSqlReturn(strSql) != "1")
                        {
                            if (ts.TotalHours > 8)
                            {
                                //data["測試上線時間預警"] = "超時" + ConvertTimeHour((ts.TotalHours - 8).ToString()) + "小時 未上線";
                                data["TestOnlineWarning"] = "overtime" + ConvertTimeHour((ts.TotalHours - 8).ToString()) + "hours Not online";
                            }
                            else
                            {
                                // data["測試上線時間預警"] = "測試未完成";

                                data["TestOnlineWarning"] = "Test not completed";
                            }
                        }
                        else
                        {
                            //data["測試上線時間預警"] = "測試完成";

                            data["TestOnlineWarning"] = "Test completed";

                            //下面這個sql沒被調用？初衷是要取fistFinishTime？ 先屏蔽了fyq20180522                       
                            //strSql = $@"SELECT MAX(EDIT_TIME) EDIT_TIME 
                            //              FROM R_SN_STATION_DETAIL
                            //             WHERE SN = '{dtFirstBIP.Rows[0]["SN"].ToString()}'
                            //               AND WORKORDERNO = '{strWo}'
                            //               AND STATION_NAME IN ('P_FQC', 'SMT_FQC')";

                            try
                            {
                                DateTime fistFinishTime = DateTime.Parse(dtFirstBIP.Rows[0]["EDIT_TIME"].ToString());//為什么fistFinishTime要取值于fistBIPTime？
                                ts = fistFinishTime - fistBIPTime;
                                if (ts.TotalHours > 8)
                                {
                                    // data["測試上線時間預警"] = "測試完成,超時" + ConvertTimeHour((ts.TotalHours - 8).ToString()) + "小時)";
                                    data["TestOnlineWarning"] = "Test completed,overtime" + ConvertTimeHour((ts.TotalHours - 8).ToString()) + "hours)";
                                }
                                else
                                {
                                    // data["測試上線時間預警"] = "測試完成";
                                    data["TestOnlineWarning"] = "Test completed";
                                }
                            }
                            catch
                            {
                                //data["測試上線時間預警"] = "測試完成" + "(ERR)";
                                data["TestOnlineWarning"] = "Test completed" + "(ERR)";
                            }

                        }
                        strSql = "SELECT COUNT(1) FROM R_SN WHERE WORKORDERNO='" + strWo + "' AND COMPLETED_FLAG='1'";
                        string jobfinishCount = sfcdb.ExecSqlReturn(strSql);

                        //if (jobfinishCount == data["工單批量"].ToString())
                        if (jobfinishCount == data["WorkordenoBatch"].ToString())
                        {
                            
                            //data["SMT工單關結及時預警"] = "已完工";
                            data["SMTWorkordeClosedWarning"] = "Completed";
                            try
                            {
                                strSql = $@"SELECT MAX(EDIT_TIME) FROM R_SN_STATION_DETAIL WHERE WORKORDERNO = '{strWo}' AND STATION_NAME IN('P_FQC', 'SMT_FQC')";
                                DateTime lastFinishTime = DateTime.Parse(sfcdb.ExecSqlReturn(strSql));

                                strSql = $@"SELECT MAX(EDIT_TIME) FROM R_SN_STATION_DETAIL WHERE WORKORDERNO = '{strWo}' AND STATION_NAME = 'BIP'";
                                DateTime lastBIPTime = DateTime.Parse(sfcdb.ExecSqlReturn(strSql));
                                ts = lastFinishTime - lastBIPTime;
                                if (ts.TotalHours > 24)
                                {
                                    // data["SMT工單關結及時預警"] = "已完工(超時" + ConvertTimeHour((ts.TotalHours - 24).ToString()) + "小時)";

                                    data["SMTWorkordeClosedWarning"] = "Completed(overtime" + ConvertTimeHour((ts.TotalHours - 24).ToString()) + "hours)";
                                }
                            }
                            catch
                            { }
                        }
                        else
                        {
                            //data["SMT工單關結及時預警"] = "未完工";
                            data["SMTWorkordeClosedWarning"] = "unfinished";
                            if (ts.TotalHours > 24)
                            {
                                //data["SMT工單關結及時預警"] = "未完工(超時" + ConvertTimeHour((ts.TotalHours - 24).ToString()) + "小時)";
                                data["SMT工單關結及時預警"] = "unfinished(overtime" + ConvertTimeHour((ts.TotalHours - 24).ToString()) + "hours)";
                            }
                        }
                    }
                }

               

                ReportTable retTab = new ReportTable();
                retTab.LoadData(Data, null);
                retTab.Tittle = "HWD SMT E2E Report";
                Outputs.Add(retTab);
            }
            catch (Exception ex)
            {               
                Outputs.Add(new ReportAlart(ex.Message));
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
                DBPools["APDB"].Return(apdb);
            }
           
        }

        string ConvertTimeHour(string s)
        {
            int point = s.IndexOf('.');
            if (point >= 0)
            {
                if (s.Length <= point + 3)
                {
                    return s;
                }
                else
                {
                    return s.Substring(0, point + 3);
                }
            }
            else
            {
                return s;
            }
        }

    }
}
