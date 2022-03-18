using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_TEST_RECORD : DataObjectTable
    {
        public T_R_TEST_RECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_RECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_RECORD);
            TableName = "R_TEST_RECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 取Sn loading之後每個測試工站的最後一筆測試記錄
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="snId"></param>
        /// <param name="StartTime"></param>
        /// <returns></returns>
        public List<R_TEST_RECORD> GetTestDataByTimeBefor(OleExec DB, string sn, DateTime StartTime)
        {
            List<R_TEST_RECORD> l = new List<R_TEST_RECORD>();
            //string strSql = $@" select * from (
            //                select a.*,RANK() over(partition by messtation order by endtime desc) as rk from r_test_record a
            //                 where r_sn_id ='{snId}' and endtime>to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk=1 ";

            string strSql = $@" select * from (
                            select a.*,RANK() over(partition by messtation order by endtime desc) as rk from r_test_record a
                             where sn ='{sn}' and endtime>to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk=1 ";

            //ARUBA有個神邏輯：MES系統SN不帶PN後綴,Label和測試記錄帶PN後綴,加個邏輯：如果是ARUBA的就Like查詢  Request By PE 吳忠義 2022-01-29
            var isAruba = DB.ORM.Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER>((r, sku, se, cus) => r.SKUNO == sku.SKUNO && sku.C_SERIES_ID == se.ID && se.CUSTOMER_ID == cus.ID)
                .Where((r, sku, se, cus) => r.SN == sn && cus.CUSTOMER_NAME.ToUpper() == "ARUBA").Select((r, sku, se, cus) => cus).Any();
            if (isAruba)
            {
                strSql = $@"
                select *
                  from (select a.*,
                               RANK() over(partition by messtation order by endtime desc) as rk
                          from r_test_record a
                         where sn like '{sn}%'
                           and endtime >
                               to_date('{StartTime:yyyy-MM-dd HH:mm:ss}', 'yyyy/mm/dd hh24:mi:ss'))
                 where rk = 1";
            }
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Row_R_TEST_RECORD r = (Row_R_TEST_RECORD)this.NewRow();
                r.loadData(dr);
                l.Add(r.GetDataObject());
            }
            return l;
        }

        public List<R_TEST_RECORD> GetTestDataByChassisBBT(OleExec DB, string sn, DateTime StartTime)
        {
            List<R_TEST_RECORD> l = new List<R_TEST_RECORD>();

            //string strSql = $@" select * from (
            //                select a.*,RANK() over(partition by messtation order by endtime desc) as rk from r_test_record a
            //                 where sn ='{sn}' and endtime>to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk=1 ";

            string strSql = $@" select * from ( select a.*,RANK() over(partition by SN order by endtime desc) as rk 
                            from (select b.exvalue1, a.* from  r_test_record a inner join R_SN_KP b on a.sn = b.value and b.exvalue1 in ('SM0','SM1') where b.SN = '{sn}' and a.MESSTATION = 'SFT' ) a 
                            where endtime>to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk=1 ";

            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Row_R_TEST_RECORD r = (Row_R_TEST_RECORD)this.NewRow();
                r.loadData(dr);
                l.Add(r.GetDataObject());
            }
            return l;
        }

        public List<R_TEST_RECORD> GetSOLTTestDataByTopLevel(OleExec DB, string sn, DateTime StartTime)
        {
            List<R_TEST_RECORD> l = new List<R_TEST_RECORD>();

            string strSql = $@" select * from ( select a.*,RANK() over(partition by SN order by endtime desc) as rk 
                                from (  select * from r_test_record where sn in
                                (select sysserialno from R_TEST_DETAIL_Oracle where vsn = '{sn}') and testation = 'SOLT' and state = 'PASS' ) a 
                                where endtime>to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk=1 ";

            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Row_R_TEST_RECORD r = (Row_R_TEST_RECORD)this.NewRow();
                r.loadData(dr);
                l.Add(r.GetDataObject());
            }
            return l;
        }

        public List<R_TEST_RECORD> GetTestDataByForODAVanilla(OleExec DB, string sn, DateTime StartTime)
        {
            List<R_TEST_RECORD> l = new List<R_TEST_RECORD>();

            //string strSql = $@" select * from (
            //                select a.*,RANK() over(partition by messtation order by endtime desc) as rk from r_test_record a
            //                 where sn ='{sn}' and endtime>to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk=1 ";

            string strSql = $@" select ID, KP_NAME as R_SN_ID, SN, STATE, TEGROUP, TESTATION, MESSTATION, DEVICE, STARTTIME, ENDTIME, DETAILTABLE, EDIT_TIME, EDIT_EMP from (select a.*,RANK() over(partition by SN order by endtime desc) as rk 
                                from (select b.KP_NAME, a.* from r_test_record a inner
                                join R_SN_KP b on a.sn = b.value
                                and trim(b.KP_NAME) in ('ODA_HA-0', 'ODA_HA-1') where b.SN = '{sn}' and a.MESSTATION = 'SFT' ) a
                                where endtime > to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk = 1 ";

            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Row_R_TEST_RECORD r = (Row_R_TEST_RECORD)this.NewRow();
                r.loadData(dr);
                l.Add(r.GetDataObject());
            }
            return l;
        }

        public List<R_TEST_RECORD> GetTestDataByForODAChoco(OleExec DB, string sn, DateTime StartTime)
        {
            List<R_TEST_RECORD> l = new List<R_TEST_RECORD>();

            string strSql = $@" select * from ( select a.*,RANK() over(partition by SN order by endtime desc) as rk 
                            from (select * from  r_test_record  where SN like '{sn}%' and MESSTATION = 'FSC' ) a 
                            where endtime>to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk=1 order by starttime ";

            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Row_R_TEST_RECORD r = (Row_R_TEST_RECORD)this.NewRow();
                r.loadData(dr);
                l.Add(r.GetDataObject());
            }
            return l;
        }

        public R_TEST_RECORD GetSNLastRITPassDetail(string SN, OleExec DB)
        {
            R_TEST_RECORD R_Sn_Test_Detail = null;
            Row_R_TEST_RECORD Rows = (Row_R_TEST_RECORD)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select*from (select*from R_TEST_RECORD where sn='{SN}' and testation='RIT'  order by edit_time desc)a where rownum=1 and state='PASS' ";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Test_Detail = Rows.GetDataObject();
            }

            return R_Sn_Test_Detail;
        }
        public R_TEST_RECORD GetSNLastORTPassDetail(string SN, string CREATE_DATE, OleExec DB)
        {
            R_TEST_RECORD R_Sn_Test_Detail = null;
            Row_R_TEST_RECORD Rows = (Row_R_TEST_RECORD)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;
            //避免出現多筆記錄,改為抓取create_date最大那筆
            //StrSql = $@"select*from (select*from R_TEST_RECORD where sn='{SN}' and testation IN('ORT','ORT-FT2') and starttime>(select create_date From r_lot_detail where sn='{SN}') order by edit_time desc)a where rownum=1 and state='PASS' ";
            StrSql = $@"select*from (select*from R_TEST_RECORD where sn='{SN}' and testation IN('ORT','ORT-FT2') and starttime>(select max(create_date) from r_lot_detail d where d.sn='{SN}' and exists 
                        (select * from r_lot_status s where s.id=d.lot_id and s.skuno=d.workorderno and s.sample_station='ORT')) order by edit_time desc)a where rownum=1 and state='PASS' ";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Test_Detail = Rows.GetDataObject();
            }

            return R_Sn_Test_Detail;
        }


        /// <summary>
        /// add by fgg 2018.5.11
        /// </summary>
        /// <param name="snID">snID</param>
        /// <param name="sn">sn</param>
        /// <param name="station">station</param>
        /// <param name="db">db</param>
        /// <returns></returns>
        public bool CheckTestBySNAndStation(string snID, string station, OleExec db)
        {
            string sql = $@" select * from r_test_record rt where rt.r_sn_id='{snID}' 
                            and exists (select * from c_temes_station_mapping tmap where tmap.te_station=rt.testation 
                            and tmap.mes_station='{station}') 
                            and exists (select 1 from R_SN rs where rt.r_sn_id=rs.id and rt.endtime>rs.start_time  ) order by endtime desc";
            DataTable passDT = db.ExecSelect(sql).Tables[0];
            if (passDT.Rows.Count > 0)
            {
                if (passDT.Rows[0]["STATE"].ToString() == "PASS")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// add by HGB 2019.5.23
        /// </summary>
        /// <param name="snID">snID</param>      
        /// <param name="station">station</param>
        /// <param name="db">db</param>
        /// <returns></returns>
        public bool CheckTestBySNAndStation2(string snID, string station, OleExec db)
        {
            string sql = $@" select * from r_test_record rt where rt.r_sn_id='{snID}' 
                            and exists (select * from c_temes_station_mapping tmap where tmap.te_station=rt.testation 
                            and tmap.mes_station='{station}') 
                            and exists (select 1 from R_SN rs where rt.r_sn_id=rs.id    ) order by endtime desc";
            DataTable passDT = db.ExecSelect(sql).Tables[0];
            if (passDT.Rows.Count > 0)
            {
                if (passDT.Rows[0]["STATE"].ToString() == "PASS")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void CheckAllTestBySNStation(string snID, string station,string bu,bool allowByPass, OleExec db)
        {
            try
            {
                var snObj = db.ORM.Queryable<R_SN>().Where(t => t.ID == snID).First();
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(db, DB_TYPE_ENUM.Oracle);
                #region 檢查當前工站之前的所有測試工站的最後一筆測試記錄是否PASS
                List<C_ROUTE_DETAIL> cRouteDetailList = t_c_route_detail.GetTestStationByNameBefor(db, snObj.ROUTE_ID, station);
                //List<R_TEST_RECORD> td = t_r_test_record.GetTestDataByTimeBefor(sfcdb, snObj.ID, snObj.StartTime ?? DateTime.Now);
                // check the latest test record              
                var starttime = new Func<DateTime>(()=> {
                    var startstations = new List<string>() { "RETURN", "REWORK", "SILOADING", "SMTLOADING" };
                    var sstation = db.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == snObj.SN && startstations.Contains(t.STATION_NAME)).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                    if(sstation==null)
                        return DateTime.Now;
                    else
                        return Convert.ToDateTime(sstation.EDIT_TIME);
                })();
                List<R_TEST_RECORD> td = this.GetTestDataByTimeBefor(db, snObj.SN, snObj.START_TIME ?? DateTime.Now);
                List<R_TEST_RECORD> tdcurrent = this.GetTestDataByTimeBefor(db, snObj.SN, starttime);

                List<R_SN_PASS> rsp = new List<R_SN_PASS>();
                T_R_LOT_STATUS RowLotStatus = new T_R_LOT_STATUS(db, DB_TYPE_ENUM.Oracle);
                R_LOT_STATUS getLotNo;
                getLotNo = RowLotStatus.GetLotBySNAndWo(snObj.SN, snObj.WORKORDERNO, db);
                if (getLotNo != null)
                {
                    rsp =db.ORM.Queryable<R_SN_PASS>().Where(t => (t.WORKORDERNO == snObj.WORKORDERNO || t.WORKORDERNO == snObj.SKUNO || t.SN == snObj.SN || t.LOTNO == getLotNo.LOT_NO) && t.STATUS == "1").ToList();
                }
                else
                {
                    rsp =db.ORM.Queryable<R_SN_PASS>().Where(t => (t.WORKORDERNO == snObj.WORKORDERNO || t.WORKORDERNO == snObj.SKUNO || t.SN == snObj.SN) && t.STATUS == "1").ToList();
                }
                T_R_MES_LOG mesLog = new T_R_MES_LOG(db, DB_TYPE_ENUM.Oracle);
                //string id = mesLog.GetNewID("ORACLE", sfcdb);
                Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();

                //檢查這個SN是否存在不需要抽測MINI-LTT的記錄 Add By ZHB 20200807
                var noNeedMiniLTT = db.ORM.Queryable<R_SN_LOG>().Where(t => t.SN == snObj.SN && t.LOGTYPE == "MINI-LTT-SAMPLE" && t.FLAG == "N").Select(t => t.SN).Any();

                foreach (var item in cRouteDetailList)
                {
                    var checktd = td;
                    if (station == item.STATION_NAME)
                        checktd = tdcurrent;
                    //Add to check 2C product, Chassis BBT first and SM SFT last vince_20190318
                    if (checktd.FindAll(t => t.TESTATION == "BBT" && t.STATE.Equals("PASS")).Count != 0 && item.STATION_NAME.ToString() == "SFT")
                    {
                        List<R_TEST_RECORD> td_BBT = this.GetTestDataByChassisBBT(db, snObj.SN, snObj.START_TIME ?? DateTime.Now);

                        if (td_BBT.Count != 2)
                        {
                            //missing SM test data
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_01", new string[] { snObj.SN, item.STATION_NAME }));
                        }
                        else
                        {
                            foreach (var SM in td_BBT)
                            {
                                if (checktd.FindAll(t => t.TESTATION == "BBT" && t.ENDTIME <= SM.ENDTIME).Count == 0)
                                {
                                    //writeLog for test data missing  vince_20190305
                                    db.BeginTrain();
                                    //rowMESLog.ID = id;
                                    rowMESLog.ID = mesLog.GetNewID(bu, db);
                                    rowMESLog.PROGRAM_NAME = "CloudMES";
                                    rowMESLog.CLASS_NAME = "Check SN";
                                    rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                    rowMESLog.LOG_MESSAGE = "SM Test Data before Chassis " + snObj.SN + " in station " + item.STATION_NAME;
                                    rowMESLog.LOG_SQL = "";
                                    rowMESLog.EDIT_EMP = "OracleStation";
                                    rowMESLog.EDIT_TIME = System.DateTime.Now;
                                    rowMESLog.DATA1 = snObj.SN;
                                    db.ThrowSqlExeception = true;
                                    db.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                    db.CommitTrain();

                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_02", new string[] { snObj.SN, item.STATION_NAME }));
                                }
                                else if (checktd.FindAll(t => t.TESTATION == "BBT" && SM.STATE.Equals("PASS")).Count == 0)
                                {
                                    //writeLog for test data missing  vince_20190305
                                    db.BeginTrain();
                                    //rowMESLog.ID = id;
                                    rowMESLog.ID = mesLog.GetNewID(bu, db);
                                    rowMESLog.PROGRAM_NAME = "CloudMES";
                                    rowMESLog.CLASS_NAME = "Check SN";
                                    rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                    rowMESLog.LOG_MESSAGE = "SM Test Pass Data not exists for " + snObj.SN + " in station " + item.STATION_NAME;
                                    rowMESLog.LOG_SQL = "";
                                    rowMESLog.EDIT_EMP = "OracleStation";
                                    rowMESLog.EDIT_TIME = System.DateTime.Now;
                                    rowMESLog.DATA1 = snObj.SN;
                                    db.ThrowSqlExeception = true;
                                    db.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                    db.CommitTrain();

                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_01", new string[] { snObj.SN, item.STATION_NAME }));
                                }
                            }
                        }
                    }
                    //end vince_20190318

                    //Add to check ODA_HA product, Vanilla SFT first and Chocolate FSC last vince_20190909

                    //td.FindAll(t => t.TESTATION == "BBT" && t.STATE.Equals("PASS")).Count != 0 && 
                    if (item.STATION_NAME.ToString() == "FSC")
                    {
                        T_C_SKU T_C_SKU = new T_C_SKU(db, DB_TYPE_ENUM.Oracle);
                        SkuObject SKU = T_C_SKU.GetSkuBySn(snObj.SN.ToString(), db);
                        T_C_SERIES t_c_series = new T_C_SERIES(db, DB_TYPE_ENUM.Oracle);
                        C_SERIES c_series = t_c_series.GetDetailById(db, SKU.CSeriesId);//sku.CSeriesId
                        if (c_series.SERIES_NAME.Contains("ODA_HA"))
                        {

                            List<R_TEST_RECORD> td_Choco = this.GetTestDataByForODAChoco(db, snObj.SN, snObj.START_TIME ?? DateTime.Now);
                            List<R_TEST_RECORD> td_Vanilla = this.GetTestDataByForODAVanilla(db, snObj.SN, snObj.START_TIME ?? DateTime.Now);

                            if (td_Choco.FindAll(t => t.TESTATION == "FSC" && t.STATE.Equals("PASS")).Count != 2)
                            {
                                //missing choco test data
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_03", new string[] { snObj.SN, item.STATION_NAME }));
                            }
                            else if (td_Vanilla.FindAll(t => t.TESTATION == "SFT" && t.STATE.Equals("PASS")).Count != 2)
                            {
                                //missing vanilla test data
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_05", new string[] { snObj.SN, item.STATION_NAME }));
                            }
                            else
                            {
                                foreach (var SM in td_Vanilla)
                                {
                                    if (td_Choco.FindAll(t => t.ENDTIME <= SM.ENDTIME && SM.STATE.Equals("PASS")).Count != 0)
                                    {
                                        //writeLog for test data missing  vince_20190305
                                        db.BeginTrain();
                                        //rowMESLog.ID = id;
                                        rowMESLog.ID = mesLog.GetNewID(bu, db);
                                        rowMESLog.PROGRAM_NAME = "CloudMES";
                                        rowMESLog.CLASS_NAME = "Check SN";
                                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                        rowMESLog.LOG_MESSAGE = "Vanilla Test Data is after Chocolate " + snObj.SN + " in station " + item.STATION_NAME;
                                        rowMESLog.LOG_SQL = "";
                                        rowMESLog.EDIT_EMP = "OracleStation";
                                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                                        rowMESLog.DATA1 = snObj.SN;
                                        db.ThrowSqlExeception = true;
                                        db.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                        db.CommitTrain();

                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_04", new string[] { snObj.SN, item.STATION_NAME }));
                                    }
                                    else if (td_Choco.FindAll(t => t.TESTATION == "FSC" && SM.TESTATION == "SFT" && SM.R_SN_ID.Substring(SM.R_SN_ID.Length - 2, 2).ToString() == t.SN.Substring(t.SN.Length - 2, 2).ToString() && SM.STATE.Equals("PASS")).Count == 0)
                                    {
                                        //writeLog for test data missing  vince_20190305
                                        db.BeginTrain();
                                        //rowMESLog.ID = id;
                                        rowMESLog.ID = mesLog.GetNewID(bu, db);
                                        rowMESLog.PROGRAM_NAME = "CloudMES";
                                        rowMESLog.CLASS_NAME = "Check SN";
                                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                        rowMESLog.LOG_MESSAGE = "Vanilla Test Pass Data not exists for " + snObj.SN + " in station " + item.STATION_NAME;
                                        rowMESLog.LOG_SQL = "";
                                        rowMESLog.EDIT_EMP = "OracleStation";
                                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                                        rowMESLog.DATA1 = snObj.SN;
                                        db.ThrowSqlExeception = true;
                                        db.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                        db.CommitTrain();

                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_05", new string[] { snObj.SN, item.STATION_NAME }));
                                    }
                                }
                            }
                            checktd = td_Choco;
                        }
                    }
                    //end vince_20190909

                    //Add SOLT test station check for L11 vince_20200128
                    if (item.STATION_NAME.ToString() == "SOLT")
                    {
                        //string SOLTKPString = "";
                        List<R_TEST_RECORD> td_TestSOLT = this.GetSOLTTestDataByTopLevel(db, snObj.SN, snObj.START_TIME ?? DateTime.Now);
                        T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(db, DB_TYPE_ENUM.Oracle);
                        List<R_SN_KP> td_ScanSOLT = t_r_sn_kp.GetSOLTScanDataByTopLevel(db, snObj.SN);
                        //T_C_CONTROL TCC = new T_C_CONTROL(Station.SFCDB, Station.DBType);
                        //List<C_CONTROL> controls = TCC.GetControlList("SOLTTestRecordChk", Station.SFCDB);
                        DateTime minDate = DateTime.MaxValue;
                        DateTime maxDate = DateTime.MinValue;
                        foreach (R_TEST_RECORD TestComponent in td_TestSOLT)
                        {
                            DateTime date = DateTime.Parse(TestComponent.STARTTIME.ToString());
                            if (date < minDate)
                                minDate = date;
                            if (date > maxDate)
                                maxDate = date;
                        }

                        TimeSpan duration = maxDate - minDate;
                        if (duration.TotalHours > 1)
                        {
                            //timespan not within 1hr
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_06", new string[] { snObj.SN, item.STATION_NAME }));
                        }
                        foreach (R_SN_KP ScanComponent in td_ScanSOLT)
                        {
                            if (td_TestSOLT.FindAll(t => t.SN == ScanComponent.VALUE && t.STATE.Equals("PASS")).Count < 1)
                            {
                                //missing component test data
                                //writeLog for test data missing  vince_20190305
                                db.BeginTrain();
                                rowMESLog.ID = mesLog.GetNewID(bu, db);
                                //rowMESLog.ID = id;
                                rowMESLog.PROGRAM_NAME = "CloudMES";
                                rowMESLog.CLASS_NAME = "Check SN";
                                rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                                rowMESLog.LOG_MESSAGE = "Test Pass Data not exists for " + ScanComponent.VALUE + " in station " + item.STATION_NAME;
                                rowMESLog.LOG_SQL = "";
                                rowMESLog.EDIT_EMP = "OracleStation";
                                rowMESLog.EDIT_TIME = System.DateTime.Now;
                                rowMESLog.DATA1 = snObj.SN;
                                db.ThrowSqlExeception = true;
                                db.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                                db.CommitTrain();

                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_07", new string[] { snObj.SN, item.STATION_NAME }));
                            }
                        }

                        if (td_ScanSOLT.Count < td_TestSOLT.Count)
                        {
                            //more component test data than scan
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244_08", new string[] { snObj.SN, item.STATION_NAME }));
                        }
                        else
                        {
                            //writeLog for successfully pass    vince_20190305
                            //rowMESLog.ID = id;
                            rowMESLog.ID = mesLog.GetNewID(bu, db);
                            rowMESLog.PROGRAM_NAME = "CloudMES";
                            rowMESLog.CLASS_NAME = "Check SN";
                            rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                            rowMESLog.LOG_MESSAGE = "Test pass successfully for " + snObj.SN + " in station " + item.STATION_NAME;
                            rowMESLog.LOG_SQL = "";
                            rowMESLog.EDIT_EMP = "OracleStation";
                            rowMESLog.EDIT_TIME = System.DateTime.Now;
                            rowMESLog.DATA1 = snObj.SN;
                            db.ThrowSqlExeception = true;
                            db.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                        }
                    }

                    //end vince_20200128
                    var bypass = false;
                    if (rsp.FindAll(r => r.PASS_STATION == item.STATION_NAME).Count > 0 && allowByPass)
                    {
                        bypass = true;
                    }

                    if (checktd.FindAll(t => t.MESSTATION == item.STATION_NAME && t.STATE.Equals("PASS")).Count == 0 
                        && item.STATION_NAME.ToString() != "SOLT"
                        && !bypass)
                    {
                        if (item.STATION_NAME == "MINI-LTT" && noNeedMiniLTT)
                        {
                            continue;   //如果循環路由裡面工站為MINI-LTT且存在不需要抽測MINI-LTT的記錄，就不用檢查這個SN的MINI-LTT測試記錄了  Add By ZHB 20200807
                        }
                        //writeLog for test data missing  vince_20190305
                        db.BeginTrain();
                        rowMESLog.ID = mesLog.GetNewID(bu, db);
                        //rowMESLog.ID = id;
                        rowMESLog.PROGRAM_NAME = "CloudMES";
                        rowMESLog.CLASS_NAME = "Check SN";
                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                        rowMESLog.LOG_MESSAGE = "Test Pass Data not exists for " + snObj.SN + " in station " + item.STATION_NAME;
                        rowMESLog.LOG_SQL = "";
                        rowMESLog.EDIT_EMP = "OracleStation";
                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                        rowMESLog.DATA1 = snObj.SN;
                        db.ThrowSqlExeception = true;
                        db.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                        db.CommitTrain();

                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { snObj.SN, item.STATION_NAME }));
                    }
                    else
                    {
                        //writeLog for successfully pass    vince_20190305
                        //rowMESLog.ID = id;
                        rowMESLog.ID = mesLog.GetNewID(bu, db);
                        rowMESLog.PROGRAM_NAME = "CloudMES";
                        rowMESLog.CLASS_NAME = "Check SN";
                        rowMESLog.FUNCTION_NAME = "Check SN Test Data";
                        rowMESLog.LOG_MESSAGE = "Test pass successfully for " + snObj.SN + " in station " + item.STATION_NAME;
                        rowMESLog.LOG_SQL = "";
                        rowMESLog.EDIT_EMP = "OracleStation";
                        rowMESLog.EDIT_TIME = System.DateTime.Now;
                        rowMESLog.DATA1 = snObj.SN;
                        db.ThrowSqlExeception = true;
                        db.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                }

                #endregion
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertTestRecord(R_TEST_RECORD record, OleExec DB)
        {
            return DB.ORM.Insertable<R_TEST_RECORD>(record).ExecuteCommand();
        }

        public bool CheckTestRecord(string Sn, string Station, OleExec DB)
        {
            if (DB.ORM.Queryable<R_TEST_RECORD>().Any(t => t.SN == Sn && t.MESSTATION == Station && t.STATE.ToUpper() == "PASS"))
            {
                return true;
            }
            else
            {
                T_R_PANEL_SN TRPS = new T_R_PANEL_SN(DB, DBType);
                var Panel = TRPS.GetPanelBySn(Sn, DB);
                if (Panel == null)
                {
                    return false;
                }
                return (DB.ORM.Queryable<R_TEST_RECORD>().Any(t => t.SN == Panel.PANEL && t.MESSTATION == Station && t.STATE.ToUpper() == "PASS"));
            }
        }

        public R_TEST_RECORD GetLastTestRecord(string Sn, string Station, OleExec DB)
        {
            return DB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == Sn && t.MESSTATION == Station).OrderBy(t => t.ENDTIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }

        public R_TEST_RECORD GetLastTestRecord(string Sn, OleExec DB)
        {
            R_TEST_RECORD RTR;

            RTR = DB.ORM.Queryable<R_TEST_RECORD>()
                .Where(t => t.SN == Sn)
                .OrderBy(t => t.ENDTIME, SqlSugar.OrderByType.Desc)
                .ToList()
                .FirstOrDefault();
            return RTR;

        }




        /// <summary>
        /// 根據 C_CONTROL 中的配置檢查 DOM 資料，C_CONTROL 配置的是客戶名
        /// </summary>
        /// <param name="Sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckDom(string Sn, OleExec DB)
        {
            C_CONTROL control = DB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "CHECK_DOM").ToList().FirstOrDefault();
            C_CUSTOMER Customer = DB.ORM.Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER>((t1, t2, t3, t4) => t1.SKUNO == t2.SKUNO && t2.C_SERIES_ID == t3.ID && t3.CUSTOMER_ID == t4.ID)
                .Where((t1, t2, t3, t4) => t1.SN == Sn).Select((t1, t2, t3, t4) => t4).ToList().FirstOrDefault();
            R_SN RSN = DB.ORM.Queryable<R_SN>().Where(t => t.SN == Sn && t.VALID_FLAG == "1").ToList().FirstOrDefault();
            if (control != null && Customer != null && RSN != null)
            {
                if (control.CONTROL_VALUE.Contains(Customer.CUSTOMER_NAME) || control.CONTROL_VALUE.Contains(RSN.SKUNO.Substring(0, 2)))
                {
                    return CheckTestRecord(Sn, "DOM", DB);
                }
            }
            return true;
        }

        public void ReplaceSnTestRecord(string OldSn, string NewSn, OleExec DB)
        {
            DB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = NewSn }).Where(t => t.SN == OldSn).ExecuteCommand();
        }

        /// <summary>
        /// 判斷某個機種的某個站位是不是自動測試站位，即 R_TEST_RECORD 中有沒有測試記錄
        /// 如果存在有這個機種的任意一片有這個站位的測試記錄就表示這個機種這個站位是自動測試站位
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="StationName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckStationIsTest(string Skuno, string StationName, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN, R_TEST_RECORD>((rs, rtr) => rs.SN == rtr.SN).Where((rs, rtr) => rs.SKUNO == Skuno && rtr.MESSTATION == StationName).Select((rs, rtr) => rtr).Any();
        }
    }
    public class Row_R_TEST_RECORD : DataObjectBase
    {
        public Row_R_TEST_RECORD(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_RECORD GetDataObject()
        {
            R_TEST_RECORD DataObject = new R_TEST_RECORD();
            DataObject.DETAILTABLE = this.DETAILTABLE;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.STARTTIME = this.STARTTIME;
            DataObject.DEVICE = this.DEVICE;
            DataObject.MESSTATION = this.MESSTATION;
            DataObject.TESTATION = this.TESTATION;
            DataObject.TEGROUP = this.TEGROUP;
            DataObject.STATE = this.STATE;
            DataObject.SN = this.SN;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string DETAILTABLE
        {
            get
            {
                return (string)this["DETAILTABLE"];
            }
            set
            {
                this["DETAILTABLE"] = value;
            }
        }
        public DateTime? ENDTIME
        {
            get
            {
                return (DateTime?)this["ENDTIME"];
            }
            set
            {
                this["ENDTIME"] = value;
            }
        }
        public DateTime? STARTTIME
        {
            get
            {
                return (DateTime?)this["STARTTIME"];
            }
            set
            {
                this["STARTTIME"] = value;
            }
        }
        public string DEVICE
        {
            get
            {
                return (string)this["DEVICE"];
            }
            set
            {
                this["DEVICE"] = value;
            }
        }
        public string MESSTATION
        {
            get
            {
                return (string)this["MESSTATION"];
            }
            set
            {
                this["MESSTATION"] = value;
            }
        }
        public string TESTATION
        {
            get
            {
                return (string)this["TESTATION"];
            }
            set
            {
                this["TESTATION"] = value;
            }
        }
        public string TEGROUP
        {
            get
            {
                return (string)this["TEGROUP"];
            }
            set
            {
                this["TEGROUP"] = value;
            }
        }
        public string STATE
        {
            get
            {
                return (string)this["STATE"];
            }
            set
            {
                this["STATE"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
            }
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
    }
    public class R_TEST_RECORD
    {

        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string STATE { get; set; }
        public string TEGROUP { get; set; }
        public string TESTATION { get; set; }
        public string MESSTATION { get; set; }
        public string DEVICE { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string DETAILTABLE { get; set; }
        public string TESTINFO { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}