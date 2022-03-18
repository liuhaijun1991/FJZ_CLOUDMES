using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using MESDataObject.Module;
using MESDBHelper;
using System.Data;
using System.Globalization;

namespace MESJuniper.Api
{
    public class MetricsDashboardApi : MesAPIBase
    {
        protected APIInfo FGetTrackList = new APIInfo
        {
            FunctionName = "GetTrackList",
            Description = "GetTrackList",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetTrackDate = new APIInfo
        {
            FunctionName = "GetTrackDate",
            Description = "GetTrackDate",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetTrackData = new APIInfo
        {
            FunctionName = "GetTrackData",
            Description = "GetTrackData",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetBonePileDetail = new APIInfo
        {
            FunctionName = "GetBonePileDetail",
            Description = "GetBonePileDetail",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetBonePile = new APIInfo
        {
            FunctionName = "GetBonePile",
            Description = "GetBonePile",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAingGroupByWO = new APIInfo
        {
            FunctionName = "GetAingGroupByWO",
            Description = "GetAingGroupByWO",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAingGroupByStation = new APIInfo
        {
            FunctionName = "GetAingGroupByStation",
            Description = "GetAingGroupByStation",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAingGroupBySkuno = new APIInfo
        {
            FunctionName = "GetAingGroupBySkuno",
            Description = "GetAingGroupBySkuno",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAgingDetail = new APIInfo
        {
            FunctionName = "GetAgingDetail",
            Description = "GetAgingDetail",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        public MetricsDashboardApi()
        {
            this._MastLogin = false;
            this.Apis.Add(FGetTrackList.FunctionName, FGetTrackList);
            this.Apis.Add(FGetTrackDate.FunctionName, FGetTrackDate);
            this.Apis.Add(FGetTrackData.FunctionName, FGetTrackData);
            this.Apis.Add(FGetBonePileDetail.FunctionName, FGetBonePileDetail);
            this.Apis.Add(FGetBonePile.FunctionName, FGetBonePile);
            this.Apis.Add(FGetAingGroupByWO.FunctionName, FGetAingGroupByWO);
            this.Apis.Add(FGetAingGroupByStation.FunctionName, FGetAingGroupByStation);
            this.Apis.Add(FGetAingGroupBySkuno.FunctionName, FGetAingGroupBySkuno);
            this.Apis.Add(FGetAgingDetail.FunctionName, FGetAgingDetail);
        }

        #region Track
        public void GetTrackList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var bu = Data["BU"].ToString();
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var dt = SFCDB.ORM.Queryable<R_DASHBOARDTRACKLIST>()
                    .OrderBy(t => t.SEQ, SqlSugar.OrderByType.Asc)
                    .Where(t => t.BU == bu).Select(t => new { t.TRACK, t.TRACKTYPE })
                    .ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = dt;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetTrackDate(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var Days = Data["Days"].ToString();
                var bu = Data["BU"].ToString();
                List<string> res = new List<string>();
                int d = Convert.ToInt32(Days);
                var dateStr = DateTime.Now.AddDays(-d).ToString("yyyy-MM-dd");

                string sqlString = "SELECT DISTINCT TRACKDATE FROM R_DashboardTrackHisData A WHERE TrackDate >= '{0}' AND BU='{1}' ORDER BY TrackDate DESC";
                sqlString = string.Format(sqlString, dateStr, bu);
                var dt = SFCDB.ORM.Ado.GetDataTable(sqlString);

                DateTime tddate = DateTime.Now;
                var td = tddate.ToString("d-MMM", CultureInfo.GetCultureInfo("en-US"));
                res.Add(td);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] t = dt.Rows[i][0].ToString().Split('/', '-');
                    DateTime dtime = new DateTime(Convert.ToInt32(t[0]), Convert.ToInt32(t[1]), Convert.ToInt32(t[2]));
                    var dtstr = dtime.ToString("d-MMM", CultureInfo.GetCultureInfo("en-US"));
                    res.Add(dtstr);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetTrackData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var TrackName = Data["TrackName"].ToString();
                var TrackType = Data["TrackType"].ToString();
                var BU = Data["BU"].ToString();
                var Days = Data["Days"].ToString();
                string sqlString = string.Format("select * from R_DashboardTrackList where track='{0}' and TrackType='{1}' and bu='{2}'", TrackName, TrackType, BU);
                DataTable dt = SFCDB.ORM.Ado.GetDataTable(sqlString);
                TrackData res = new TrackData();
                if (dt.Rows.Count > 0)
                {
                    res = new TrackData()
                    {
                        Track = dt.Rows[0]["Track"].ToString(),
                        TrackType = dt.Rows[0]["TrackType"].ToString(),
                        BU = dt.Rows[0]["BU"].ToString(),
                        Process = dt.Rows[0]["Process"].ToString(),
                        Report = dt.Rows[0]["Report"].ToString(),
                        Measure = dt.Rows[0]["Measure"].ToString(),
                        DataSource = dt.Rows[0]["DataSource"].ToString(),
                        Goal = dt.Rows[0]["Goal"].ToString(),
                        Owner = dt.Rows[0]["Owner"].ToString(),
                        LinkPage = dt.Rows[0]["LinkPage"].ToString()
                    };
                    res.Params = new List<string>(dt.Rows[0]["Params"].ToString().Split(','));
                    res.data = GetTrackHisData(Days, dt.Rows[0]["Track"].ToString(), dt.Rows[0]["TrackType"].ToString(), dt.Rows[0]["BU"].ToString(), SFCDB);
                    string rtsql = dt.Rows[0]["RealtimeSQL"].ToString();
                    if (rtsql.Trim().Length > 0)
                    {
                        try
                        {

                            DataObject rt = GetRealTimeData(rtsql, SFCDB);
                            if (rt != null)
                            {
                                res.data.Add(rt);
                            }
                        }
                        catch (Exception)
                        {
                            var rt = new DataObject()
                            {
                                date = DateTime.Now.ToString("d-MMM", CultureInfo.CreateSpecificCulture("en-GB")),
                                value = "Exception"
                            };
                            res.data.Add(rt);
                        }
                    }
                    else
                    {
                        DataObject rt = new DataObject() { date = DateTime.Now.ToString("d-MMM", CultureInfo.CreateSpecificCulture("en-GB")), value = "N/A" };
                        res.data.Add(rt);
                    }
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        private List<DataObject> GetTrackHisData(string Days, string TrackName, string TrackType, string BU, OleExec DB)
        {
            try
            {
                List<DataObject> res = new List<DataObject>();
                int d = Convert.ToInt32(Days);
                var dateStr = DateTime.Now.AddDays(-d).ToString("yyyy-MM-dd");
                string sqlString = "SELECT *\n" +
                "  FROM (SELECT A.*,\n" +
                "               ROW_NUMBER() OVER(PARTITION BY TrackDate ORDER BY CreateTime DESC) NUMBS\n" +
                "          FROM R_DashboardTrackHisData A\n" +
                "         WHERE TRACK='{0}' AND TRACKTYPE='{1}' AND BU='{2}' AND TrackDate >= '{3}') T\n" +
                " WHERE NUMBS = 1";

                sqlString = string.Format(sqlString, TrackName, TrackType, BU, dateStr);
                var data = DB.ORM.Ado.GetDataTable(sqlString);
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var md = data.Rows[i]["TrackDate"].ToString();
                    DateTime ddd = DateTime.Parse(md);
                    res.Add(new DataObject()
                    {
                        date = ddd.ToString("d-MMM", CultureInfo.CreateSpecificCulture("en-GB")),
                        value = data.Rows[i]["Data"].ToString()
                    });
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get Dashboard Real Time Data
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private DataObject GetRealTimeData(string sql, OleExec DB)
        {
            try
            {
                DataObject res = new DataObject() { date = DateTime.Now.ToString("d-MMM", CultureInfo.CreateSpecificCulture("en-GB")) };
                var data = DB.ORM.Ado.GetDataTable(sql);
                res.value = data.Rows[0][0].ToString();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region BonePile
        public void GetBonePileDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                #region sql
                string sqlString = $@"SELECT SN.SKUNO,
                                           SN.WORKORDERNO,
                                           SN.SN,
                                           SN.NEXT_STATION,
                                           RM.FAIL_STATION,
                                           RM.FAIL_TIME,
                                           RMF.FAIL_STATION FirstFailStation,
                                           RMF.FAIL_TIME FirstFailTime,
                                           RT.IN_TIME,
                                           RT.OUT_TIME,
                                           CASE
                                             WHEN BGA.REPAIR_ID IS NULL AND
                                                  RM.FAIL_STATION IN
                                                  ('SMT1', 'SMT2', 'AOI1', 'AOI2', 'AOI3', 'AOI4', 'ICT', '5DX') THEN
                                              'SMT'
                                             WHEN BGA.REPAIR_ID IS NOT NULL THEN
                                              'BGA'
                                             ELSE
                                              'NONAL'
                                           END FAILTYPE,
                                           DECODE(RT.CLOSED_FLAG, NULL, 'FAIL', 0, 'REPAIR', 'WIP') STATUS,
                                           DECODE(RT.CLOSED_FLAG,
                                                  NULL,
                                                  CEIL((SYSDATE - RM.FAIL_TIME) * 24),
                                                  0,
                                                  CEIL((SYSDATE - RT.IN_TIME) * 24),
                                                  1,
                                                  CEIL((SYSDATE - RT.OUT_TIME) * 24)) StatusAgingHours,
                                           CEIL((SYSDATE - RM.FAIL_TIME) * 24) TotalAgingHours,
                                           CASE
                                             WHEN CEIL((SYSDATE - RM.FAIL_TIME) * 24) <= 8 THEN
                                              '<48H'
                                             WHEN CEIL((SYSDATE - RM.FAIL_TIME) * 24) <= 24 THEN
                                              '8H~24H'
                                             WHEN CEIL((SYSDATE - RM.FAIL_TIME) * 24) <= 48 THEN
                                              '24H~48H'
                                             WHEN CEIL((SYSDATE - RM.FAIL_TIME) * 24) > 48 THEN
                                              '>48H'
                                           END BONEPILEAGINGTYPE
                                      FROM R_SN SN
                                      LEFT JOIN (SELECT *
                                                   FROM (SELECT RMT.*,
                                                                ROW_NUMBER() OVER(PARTITION BY SN ORDER BY CREATE_TIME DESC) numbs
                                                           FROM R_REPAIR_MAIN RMT
                                                          WHERE EXISTS
                                                          (SELECT *
                                                                   FROM R_SN SNT
                                                                  WHERE SNT.SN = RMT.SN
                                                                    AND SNT.WORKORDERNO = RMT.WORKORDERNO
                                                                    AND (SNT.REPAIR_FAILED_FLAG = 1 OR
                                                                        (EXISTS
                                                                         (SELECT *
                                                                             FROM R_REPAIR_MAIN RMTT
                                                                            WHERE SNT.SN = RMTT.SN
                                                                              AND SNT.WORKORDERNO = RMTT.WORKORDERNO
                                                                              AND SNT.NEXT_STATION =
                                                                                  RMTT.FAIL_STATION) AND
                                                                         SNT.REPAIR_FAILED_FLAG = 0))))
                                                  WHERE numbs = 1) RM
                                        ON SN.SN = RM.SN
                                       AND SN.WORKORDERNO = RM.WORKORDERNO
                                      LEFT JOIN R_REPAIR_TRANSFER RT
                                        ON RM.ID = RT.REPAIR_MAIN_ID
                                      LEFT JOIN (SELECT *
                                                   FROM (SELECT RMT.*,
                                                                ROW_NUMBER() OVER(PARTITION BY SN ORDER BY CREATE_TIME ASC) numbs
                                                           FROM R_REPAIR_MAIN RMT
                                                          WHERE EXISTS
                                                          (SELECT *
                                                                   FROM R_SN SNT
                                                                  WHERE SNT.SN = RMT.SN
                                                                    AND SNT.WORKORDERNO = RMT.WORKORDERNO
                                                                    AND (SNT.REPAIR_FAILED_FLAG = 1 OR
                                                                        (EXISTS
                                                                         (SELECT *
                                                                             FROM R_REPAIR_MAIN RMTT
                                                                            WHERE SNT.SN = RMTT.SN
                                                                              AND SNT.WORKORDERNO = RMTT.WORKORDERNO
                                                                              AND SNT.NEXT_STATION =
                                                                                  RMTT.FAIL_STATION) AND
                                                                         SNT.REPAIR_FAILED_FLAG = 0))))
                                                  WHERE numbs = 1) RMF
                                        ON SN.SN = RMF.SN
                                       AND SN.WORKORDERNO = RMF.WORKORDERNO
                                      LEFT JOIN SFCRUNTIME.R_BGA_DETAIL BGA
                                        ON RM.ID = BGA.REPAIR_ID
                                     WHERE SN.VALID_FLAG = '1'
                                       AND (SCRAPED_FLAG <> '1' OR SCRAPED_FLAG IS NULL)
                                       AND (SN.REPAIR_FAILED_FLAG = 1 OR EXISTS
                                            (SELECT *
                                               FROM R_REPAIR_MAIN RMTT
                                              WHERE SN.SN = RMTT.SN
                                                AND SN.WORKORDERNO = RMTT.WORKORDERNO
                                                AND SN.NEXT_STATION = RMTT.FAIL_STATION))
                                     ORDER BY TOTALAGINGHOURS DESC";
                #endregion

                var res = SFCDB.ORM.Ado.GetDataTable(sqlString);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetBonePile(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var BonePileType = Data["BonePileType"].ToString();
                #region sql
                var whrstr = "";
                switch (BonePileType)
                {
                    case "ALL":
                        whrstr = "";
                        break;
                    case "FUNCTIONAL":
                        whrstr = "WHERE FAILTYPE ='FUNCTIONAL' AND STATUS='REPAIR'";
                        break;
                    case "SMT":
                        whrstr = "WHERE FAILTYPE = 'SMT' AND STATUS='REPAIR'";
                        break;
                    case "BGA":
                        whrstr = "WHERE FAILTYPE = 'BGA' AND STATUS='REPAIR'";
                        break;
                    case "WIP":
                        whrstr = "WHERE STATUS IN('FAIL','WIP')";
                        break;
                    default:
                        whrstr = "";
                        break;
                }
                string sqlString = $@"
                                SELECT FAILTYPE, STATUS, FAIL_STATION,BONEPILEAGINGTYPE, COUNT(1) QTY
                                  FROM (SELECT SN.SKUNO,
                                               SN.WORKORDERNO,
                                               SN.SN,
                                               SN.NEXT_STATION,
                                               RM.FAIL_STATION,
                                               RM.FAIL_TIME,
                                               RMF.FAIL_STATION FirstFailStation,
                                               RMF.FAIL_TIME FirstFailTime,
                                               RT.IN_TIME,
                                               RT.OUT_TIME,
                                               CASE
                                                 WHEN BGA.REPAIR_ID IS NULL AND
                                                      RM.FAIL_STATION IN ('SMT1',
                                                                          'SMT2',
                                                                          'AOI1',
                                                                          'AOI2',
                                                                          'AOI3',
                                                                          'AOI4',
                                                                          'ICT',
                                                                          '5DX',
                                                                          '5DX2') THEN
                                                  'SMT'
                                                 WHEN BGA.REPAIR_ID IS NOT NULL THEN
                                                  'BGA'
                                                 ELSE
                                                  'FUNCTIONAL'
                                               END FAILTYPE,
                                               DECODE(RT.CLOSED_FLAG, NULL, 'FAIL', 0, 'REPAIR', 'WIP') STATUS,
                                               DECODE(RT.CLOSED_FLAG,
                                                      NULL,
                                                      CEIL((SYSDATE - RM.FAIL_TIME) * 24),
                                                      0,
                                                      CEIL((SYSDATE - RT.IN_TIME) * 24),
                                                      1,
                                                      CEIL((SYSDATE - RT.OUT_TIME) * 24)) StatusAgingHours,
                                               CEIL((SYSDATE - RM.FAIL_TIME) * 24) TotalAgingHours,
                                               CASE
                                                 WHEN CEIL((SYSDATE - RM.FAIL_TIME) * 24) <= 8 THEN
                                                  'H8'
                                                 WHEN CEIL((SYSDATE - RM.FAIL_TIME) * 24) <= 24 THEN
                                                  'H24'
                                                 WHEN CEIL((SYSDATE - RM.FAIL_TIME) * 24) <= 48 THEN
                                                  'H48'
                                                 WHEN CEIL((SYSDATE - RM.FAIL_TIME) * 24) > 48 THEN
                                                  'HM48'
                                               END BONEPILEAGINGTYPE
                                          FROM R_SN SN
                                          LEFT JOIN (SELECT *
                                                      FROM (SELECT RMT.*,
                                                                   ROW_NUMBER() OVER(PARTITION BY SN ORDER BY CREATE_TIME DESC) numbs
                                                              FROM R_REPAIR_MAIN RMT
                                                             WHERE EXISTS
                                                             (SELECT *
                                                                      FROM R_SN SNT
                                                                     WHERE SNT.SN = RMT.SN
                                                                       AND SNT.WORKORDERNO = RMT.WORKORDERNO
                                                                       AND (SNT.REPAIR_FAILED_FLAG = 1 OR
                                                                           (EXISTS (SELECT *
                                                                                       FROM R_REPAIR_MAIN RMTT
                                                                                      WHERE SNT.SN = RMTT.SN
                                                                                        AND SNT.WORKORDERNO =
                                                                                            RMTT.WORKORDERNO
                                                                                        AND SNT.NEXT_STATION =
                                                                                            RMTT.FAIL_STATION) AND
                                                                            SNT.REPAIR_FAILED_FLAG = 0))))
                                                     WHERE numbs = 1) RM
                                            ON SN.SN = RM.SN
                                           AND SN.WORKORDERNO = RM.WORKORDERNO
                                          LEFT JOIN R_REPAIR_TRANSFER RT
                                            ON RM.ID = RT.REPAIR_MAIN_ID
                                          LEFT JOIN (SELECT *
                                                      FROM (SELECT RMT.*,
                                                                   ROW_NUMBER() OVER(PARTITION BY SN ORDER BY CREATE_TIME ASC) numbs
                                                              FROM R_REPAIR_MAIN RMT
                                                             WHERE EXISTS
                                                             (SELECT *
                                                                      FROM R_SN SNT
                                                                     WHERE SNT.SN = RMT.SN
                                                                       AND SNT.WORKORDERNO = RMT.WORKORDERNO
                                                                       AND (SNT.REPAIR_FAILED_FLAG = 1 OR
                                                                           (EXISTS (SELECT *
                                                                                       FROM R_REPAIR_MAIN RMTT
                                                                                      WHERE SNT.SN = RMTT.SN
                                                                                        AND SNT.WORKORDERNO =
                                                                                            RMTT.WORKORDERNO
                                                                                        AND SNT.NEXT_STATION =
                                                                                            RMTT.FAIL_STATION) AND
                                                                            SNT.REPAIR_FAILED_FLAG = 0))))
                                                     WHERE numbs = 1) RMF
                                            ON SN.SN = RMF.SN
                                           AND SN.WORKORDERNO = RMF.WORKORDERNO
                                          LEFT JOIN SFCRUNTIME.R_BGA_DETAIL BGA
                                            ON RM.ID = BGA.REPAIR_ID
                                         WHERE SN.VALID_FLAG = '1'
                                           AND (SCRAPED_FLAG <> '1' OR SCRAPED_FLAG IS NULL)
                                           AND (SN.REPAIR_FAILED_FLAG = 1 OR EXISTS
                                                (SELECT *
                                                   FROM R_REPAIR_MAIN RMTT
                                                  WHERE SN.SN = RMTT.SN
                                                    AND SN.WORKORDERNO = RMTT.WORKORDERNO
                                                    AND SN.NEXT_STATION = RMTT.FAIL_STATION))) T
                                 {whrstr}
                                 GROUP BY FAILTYPE, STATUS, FAIL_STATION, BONEPILEAGINGTYPE
                                 ORDER BY FAILTYPE, STATUS, FAIL_STATION";
                #endregion
                var res = SFCDB.ORM.Ado.GetDataTable(sqlString);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        #endregion

        #region Aging
        public void GetAingGroupByWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var AgingType = Data["AgingType"].ToString();
                var Days = Data["Days"].ToString();
                string whrstr;
                string whrday;
                switch (AgingType)
                {
                    case "SMT":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE='SMT'";
                        break;
                    case "SI":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE='SI'";
                        break;
                    case "DOF":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE = 'DOF'";
                        break;
                    case "FG":
                        whrstr = "AND ProdStatus = 'FG'";
                        break;
                    default:
                        whrstr = "";
                        break;
                }

                switch (Days)
                {
                    case "30":
                        whrday = "AND AgingDays<=30";
                        break;
                    case "60":
                        whrday = "AND AgingDays>30 AND  AgingDays<=60 ";
                        break;
                    case "90":
                        whrday = "AND AgingDays>60 AND  AgingDays<=90 ";
                        break;
                    case "120":
                        whrday = "AND AgingDays>90 AND  AgingDays<=120 ";
                        break;
                    case "180":
                        whrday = "AND AgingDays>120 AND  AgingDays<=180 ";
                        break;
                    case "270":
                        whrday = "AND AgingDays>180 AND  AgingDays<=270 ";
                        break;
                    case "360":
                        whrday = "AND AgingDays>270 AND  AgingDays<=360 ";
                        break;
                    case "M360":
                        whrday = "AND AgingDays>360";
                        break;
                    default:
                        whrday = "";
                        break;
                }

                #region sql
                string sqlString = $@"SELECT PRODTYPE, workorderno, ProdStatus, Aging, COUNT(1) Qty
                                      FROM (SELECT DECODE(E.STATION_NAME,
                                                          'SMTLOADING',
                                                          'SMT',
                                                          'SILOADING',
                                                          DECODE(SUBSTR(S.WORKORDERNO, 0, 4), '006A', 'DOF', 'SI')) PRODTYPE,
                                                   S.sn,
                                                   S.workorderno,
                                                   S.SKUNO,
                                                   case
                                                     when S.COMPLETED_FLAG = 1 then
                                                      'FG'
                                                     else
                                                      'PROD'
                                                   end ProdStatus,
                                                   S.CURRENT_STATION,
                                                   S.NEXT_STATION,
                                                   E.EDIT_TIME LoadingTime,
                                                   CASE
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 30 then
                                                      'DAY30'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 60 then
                                                      'DAY60'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 90 then
                                                      'DAY90'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 120 then
                                                      'DAY120'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 180 then
                                                      'DAY180'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 270 then
                                                      'DAY270'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 360 then
                                                      'DAY360'
                                                     WHEN (SYSDATE - E.EDIT_TIME) > 360 then
                                                      'DAYM360'
                                                   END Aging,
                                                   (SYSDATE - E.EDIT_TIME) AgingDays
                                              FROM R_SN S, R_SN_STATION_DETAIL E
                                             WHERE S.ID = E.R_SN_ID
                                               AND S.SHIPPED_FLAG = 0
                                               AND S.SCRAPED_FLAG is null
                                               AND S.REPAIR_FAILED_FLAG = 0
                                               AND S.Current_Station <> 'MRB'
                                               AND S.VALID_FLAG = 1
                                               AND E.STATION_NAME IN ('SMTLOADING', 'SILOADING')
                                               AND E.VALID_FLAG = 1
                                               AND exists (select *
                                                      from R_WO_BASE W
                                                     where W.WORKORDERNO = S.WORKORDERNO
                                                       AND W.CLOSED_FLAG = 0)) TTTT
                                     WHERE 1 = 1 {whrstr} {whrday}
                                     GROUP BY PRODTYPE, workorderno, ProdStatus, Aging";
                #endregion
                var res = SFCDB.ORM.Ado.GetDataTable(sqlString);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetAingGroupByStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var AgingType = Data["AgingType"].ToString();
                var Days = Data["Days"].ToString();
                string whrstr;
                string whrday;
                switch (AgingType)
                {
                    case "SMT":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE='SMT'";
                        break;
                    case "SI":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE='SI'";
                        break;
                    case "DOF":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE = 'DOF'";
                        break;
                    case "FG":
                        whrstr = "AND ProdStatus = 'FG'";
                        break;
                    default:
                        whrstr = "";
                        break;
                }

                switch (Days)
                {
                    case "30":
                        whrday = "AND AgingDays<=30";
                        break;
                    case "60":
                        whrday = "AND AgingDays>30 AND  AgingDays<=60 ";
                        break;
                    case "90":
                        whrday = "AND AgingDays>60 AND  AgingDays<=90 ";
                        break;
                    case "120":
                        whrday = "AND AgingDays>90 AND  AgingDays<=120 ";
                        break;
                    case "180":
                        whrday = "AND AgingDays>120 AND  AgingDays<=180 ";
                        break;
                    case "270":
                        whrday = "AND AgingDays>180 AND  AgingDays<=270 ";
                        break;
                    case "360":
                        whrday = "AND AgingDays>270 AND  AgingDays<=360 ";
                        break;
                    case "M360":
                        whrday = "AND AgingDays>360";
                        break;
                    default:
                        whrday = "";
                        break;
                }

                #region sql
                string sqlString = $@"SELECT PRODTYPE, NEXT_STATION, ProdStatus, Aging, COUNT(1) Qty
                                      FROM (SELECT DECODE(E.STATION_NAME,
                                                          'SMTLOADING',
                                                          'SMT',
                                                          'SILOADING',
                                                          DECODE(SUBSTR(S.WORKORDERNO, 0, 4), '006A', 'DOF', 'SI')) PRODTYPE,
                                                   S.sn,
                                                   S.workorderno,
                                                   S.SKUNO,
                                                   case
                                                     when S.COMPLETED_FLAG = 1 then
                                                      'FG'
                                                     else
                                                      'PROD'
                                                   end ProdStatus,
                                                   S.CURRENT_STATION,
                                                   S.NEXT_STATION,
                                                   E.EDIT_TIME LoadingTime,
                                                   CASE
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 30 then
                                                      'DAY30'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 60 then
                                                      'DAY60'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 90 then
                                                      'DAY90'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 120 then
                                                      'DAY120'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 180 then
                                                      'DAY180'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 270 then
                                                      'DAY270'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 360 then
                                                      'DAY360'
                                                     WHEN (SYSDATE - E.EDIT_TIME) > 360 then
                                                      'DAYM360'
                                                   END Aging,
                                                   (SYSDATE - E.EDIT_TIME) AgingDays
                                              FROM R_SN S, R_SN_STATION_DETAIL E
                                             WHERE S.ID = E.R_SN_ID
                                               AND S.SHIPPED_FLAG = 0
                                               AND S.SCRAPED_FLAG is null
                                               AND S.REPAIR_FAILED_FLAG = 0
                                               AND S.Current_Station <> 'MRB'
                                               AND S.VALID_FLAG = 1
                                               AND E.STATION_NAME IN ('SMTLOADING', 'SILOADING')
                                               AND E.VALID_FLAG = 1
                                               AND exists (select *
                                                      from R_WO_BASE W
                                                     where W.WORKORDERNO = S.WORKORDERNO
                                                       AND W.CLOSED_FLAG = 0)) TTTT
                                     WHERE 1 = 1 {whrstr} {whrday}
                                     GROUP BY PRODTYPE, NEXT_STATION, ProdStatus, Aging";
                #endregion
                var res = SFCDB.ORM.Ado.GetDataTable(sqlString);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetAingGroupBySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var AgingType = Data["AgingType"].ToString();
                var Days = Data["Days"].ToString();
                string whrstr;
                string whrday;
                switch (AgingType)
                {
                    case "SMT":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE='SMT'";
                        break;
                    case "SI":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE='SI'";
                        break;
                    case "DOF":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE = 'DOF'";
                        break;
                    case "FG":
                        whrstr = "AND ProdStatus = 'FG'";
                        break;
                    default:
                        whrstr = "";
                        break;
                }

                switch (Days)
                {
                    case "30":
                        whrday = "AND AgingDays<=30";
                        break;
                    case "60":
                        whrday = "AND AgingDays>30 AND  AgingDays<=60 ";
                        break;
                    case "90":
                        whrday = "AND AgingDays>60 AND  AgingDays<=90 ";
                        break;
                    case "120":
                        whrday = "AND AgingDays>90 AND  AgingDays<=120 ";
                        break;
                    case "180":
                        whrday = "AND AgingDays>120 AND  AgingDays<=180 ";
                        break;
                    case "270":
                        whrday = "AND AgingDays>180 AND  AgingDays<=270 ";
                        break;
                    case "360":
                        whrday = "AND AgingDays>270 AND  AgingDays<=360 ";
                        break;
                    case "M360":
                        whrday = "AND AgingDays>360";
                        break;
                    default:
                        whrday = "";
                        break;
                }

                #region sql
                string sqlString = $@"SELECT PRODTYPE, SKUNO, ProdStatus, Aging, COUNT(1) Qty
                                      FROM (SELECT DECODE(E.STATION_NAME,
                                                          'SMTLOADING',
                                                          'SMT',
                                                          'SILOADING',
                                                          DECODE(SUBSTR(S.WORKORDERNO, 0, 4), '006A', 'DOF', 'SI')) PRODTYPE,
                                                   S.sn,
                                                   S.workorderno,
                                                   S.SKUNO,
                                                   case
                                                     when S.COMPLETED_FLAG = 1 then
                                                      'FG'
                                                     else
                                                      'PROD'
                                                   end ProdStatus,
                                                   S.CURRENT_STATION,
                                                   S.NEXT_STATION,
                                                   E.EDIT_TIME LoadingTime,
                                                   CASE
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 30 then
                                                      'DAY30'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 60 then
                                                      'DAY60'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 90 then
                                                      'DAY90'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 120 then
                                                      'DAY120'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 180 then
                                                      'DAY180'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 270 then
                                                      'DAY270'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 360 then
                                                      'DAY360'
                                                     WHEN (SYSDATE - E.EDIT_TIME) > 360 then
                                                      'DAYM360'
                                                   END Aging,
                                                   (SYSDATE - E.EDIT_TIME) AgingDays
                                              FROM R_SN S, R_SN_STATION_DETAIL E
                                             WHERE S.ID = E.R_SN_ID
                                               AND S.SHIPPED_FLAG = 0
                                               AND S.SCRAPED_FLAG is null
                                               AND S.REPAIR_FAILED_FLAG = 0
                                               AND S.Current_Station <> 'MRB'
                                               AND S.VALID_FLAG = 1
                                               AND E.STATION_NAME IN ('SMTLOADING', 'SILOADING')
                                               AND E.VALID_FLAG = 1
                                               AND exists (select *
                                                      from R_WO_BASE W
                                                     where W.WORKORDERNO = S.WORKORDERNO
                                                       AND W.CLOSED_FLAG = 0)) TTTT
                                     WHERE 1 = 1 {whrstr} {whrday}
                                     GROUP BY PRODTYPE, SKUNO, ProdStatus, Aging";
                #endregion

                var res = SFCDB.ORM.Ado.GetDataTable(sqlString);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetAgingDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            var AgingType = "";
            var Days = "";
            var SKUNO = "";
            var WO = "";
            var STATION = "";
            try
            {
                AgingType = Data["AgingType"].ToString();
                Days = Data["Days"].ToString();
                SKUNO = Data["SKUNO"].ToString();
                WO = Data["WO"].ToString();
                STATION = Data["STATION"].ToString();
                string whrstr;
                string whrday;
                string whrsku = "";
                string whrwo = "";
                string whrst = "";
                if (SKUNO != null && SKUNO != "null" && SKUNO != "")
                {
                    var skus = SKUNO.Split(',');
                    for (int i = 0; i < skus.Length; i++)
                    {
                        if (i == 0)
                        {
                            whrsku = " AND SKUNO IN('" + skus[i] + "',";
                        }
                        else
                        {
                            whrsku += "'" + skus[i] + "',";
                        }
                    }
                    if (whrsku.Length > 0)
                    {
                        whrsku = whrsku.Substring(0, whrsku.Length - 1);
                        whrsku += ")";
                    }
                }
                if (WO != null && WO != "null" && WO != "")
                {
                    var wos = WO.Split(',');
                    for (int i = 0; i < wos.Length; i++)
                    {
                        if (i == 0)
                        {
                            whrwo = " AND WORKORDERNO IN('" + wos[i] + "',";
                        }
                        else
                        {
                            whrwo += "'" + wos[i] + "',";
                        }
                    }
                    if (whrwo.Length > 0)
                    {
                        whrwo = whrwo.Substring(0, whrwo.Length - 1);
                        whrwo += ")";
                    }
                }
                if (STATION != null && STATION != "null" && STATION != "")
                {
                    var sts = STATION.Split(',');
                    for (int i = 0; i < sts.Length; i++)
                    {
                        if (i == 0)
                        {
                            whrst = " AND NEXT_STATION IN('" + sts[i] + "',";
                        }
                        else
                        {
                            whrst += "'" + sts[i] + "',";
                        }
                    }
                    if (whrst.Length > 0)
                    {
                        whrst = whrst.Substring(0, whrst.Length - 1);
                        whrst += ")";
                    }
                }

                switch (AgingType)
                {
                    case "SMT":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE='SMT'";
                        break;
                    case "SI":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE='SI'";
                        break;
                    case "DOF":
                        whrstr = "AND ProdStatus = 'PROD' AND PRODTYPE = 'DOF'";
                        break;
                    case "FG":
                        whrstr = "AND ProdStatus = 'FG'";
                        break;
                    default:
                        whrstr = "";
                        break;
                }

                switch (Days)
                {
                    case "30":
                        whrday = "AND AgingDays<=30";
                        break;
                    case "60":
                        whrday = "AND AgingDays>30 AND  AgingDays<=60 ";
                        break;
                    case "90":
                        whrday = "AND AgingDays>60 AND  AgingDays<=90 ";
                        break;
                    case "120":
                        whrday = "AND AgingDays>90 AND  AgingDays<=120 ";
                        break;
                    case "180":
                        whrday = "AND AgingDays>120 AND  AgingDays<=180 ";
                        break;
                    case "270":
                        whrday = "AND AgingDays>180 AND  AgingDays<=270 ";
                        break;
                    case "360":
                        whrday = "AND AgingDays>270 AND  AgingDays<=360 ";
                        break;
                    case "M360":
                        whrday = "AND AgingDays>360";
                        break;
                    default:
                        whrday = "";
                        break;
                }

                #region sql
                string sqlString = $@"SELECT *
                                      FROM (SELECT DECODE(E.STATION_NAME,
                                                          'SMTLOADING',
                                                          'SMT',
                                                          'SILOADING',
                                                          DECODE(SUBSTR(S.WORKORDERNO, 0, 4), '006A', 'DOF', 'SI')) PRODTYPE,
                                                   S.sn,
                                                   S.workorderno,
                                                   S.SKUNO,
                                                   case
                                                     when S.COMPLETED_FLAG = 1 then
                                                      'FG'
                                                     else
                                                      'PROD'
                                                   end ProdStatus,
                                                   S.CURRENT_STATION,
                                                   S.NEXT_STATION,
                                                   E.EDIT_TIME LoadingTime,
                                                   CASE
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 30 then
                                                      '<30 Days'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 60 then
                                                      '30~60 Days'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 90 then
                                                      '60~90 Days'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 120 then
                                                      '90~120 Days'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 180 then
                                                      '120~180 Days'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 270 then
                                                      '180~270 Days'
                                                     WHEN (SYSDATE - E.EDIT_TIME) <= 360 then
                                                      '270~360 Days'
                                                     WHEN (SYSDATE - E.EDIT_TIME) > 360 then
                                                      '>360 Days'
                                                   END Aging,
                                                  round(SYSDATE - E.EDIT_TIME,2) AgingDays
                                              FROM R_SN S, R_SN_STATION_DETAIL E
                                             WHERE S.ID = E.R_SN_ID
                                               AND S.SHIPPED_FLAG = 0
                                               AND S.SCRAPED_FLAG is null
                                               AND S.REPAIR_FAILED_FLAG = 0
                                               AND S.Current_Station <> 'MRB'
                                               AND S.VALID_FLAG = 1
                                               AND E.STATION_NAME IN ('SMTLOADING', 'SILOADING')
                                               AND E.VALID_FLAG = 1
                                               AND exists (select *
                                                      from R_WO_BASE W
                                                     where W.WORKORDERNO = S.WORKORDERNO
                                                       AND W.CLOSED_FLAG = 0)) TTTT
                                     WHERE 1 = 1 
                                     {whrstr}
                                     {whrday}
                                     {whrsku}
                                     {whrst} ";
                #endregion

                var res = SFCDB.ORM.Ado.GetDataTable(sqlString);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        #endregion

    }
    public class TrackData
    {
        public string Track { get; set; }
        public string TrackType { get; set; }
        public string Process { get; set; }
        public string BU { get; set; }
        public string Measure { get; set; }
        public string DataSource { get; set; }
        public string Report { get; set; }
        public string Owner { get; set; }
        public string Goal { get; set; }
        public string LinkPage { get; set; }
        public List<string> Params { get; set; }
        public List<DataObject> data { get; set; }
    }

    public class DataObject
    {
        public string date { get; set; }
        public string value { get; set; }
    }
}
