using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using MESDataObject.Common;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using DbType = System.Data.DbType;

namespace MESDataObject.Module
{
    public class T_R_SN_STATION_DETAIL : DataObjectTable
    {
        public T_R_SN_STATION_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_STATION_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_STATION_DETAIL);
            TableName = "R_SN_STATION_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 獲取當前數據庫時間所屬的班別
        /// </summary>
        /// <param name="DateTime"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public string GetWorkClass(OleExec DB)
        //{
        //    string TimeFormat = "HH24:MI:SS";
        //    DataTable dt = new DataTable();
        //    string sql = string.Empty;

        //    if (this.DBType == DB_TYPE_ENUM.Oracle)
        //    {
        //        sql = $@"SELECT * FROM C_WORK_CLASS WHERE TO_DATE(TO_CHAR(SYSDATE,'{TimeFormat}'),'{TimeFormat}')
        //                    BETWEEN TO_DATE(START_TIME,'{TimeFormat}') AND TO_DATE(END_TIME,'{TimeFormat}')";

        //        dt = DB.ExecSelect(sql).Tables[0];
        //        if (dt.Rows.Count > 0)
        //        {
        //            if (dt.Rows[0]["NAME"] != null)
        //            {
        //                return dt.Rows[0]["NAME"].ToString();
        //            }
        //            else
        //            {
        //                throw new Exception("班別的名字不能為空");
        //            }
        //        }
        //        else
        //        {
        //            //如果上面的沒有結果，表示某一條數據的 END_TIME 是第二天的時間，那麼那一條的 START_TIME 肯定是所有數據中最大的
        //            sql = "SELECT * FROM C_WORK_CLASS ORDER BY START_TIME DESC";
        //            dt = DB.ExecSelect(sql).Tables[0];
        //            if (dt.Rows.Count > 0)
        //            {
        //                return dt.Rows[0]["NAME"].ToString();
        //            }
        //            else
        //            {
        //                throw new Exception("沒有配置班別");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
        //        throw new MESReturnMessage(errMsg);
        //    }

        //}

        public string AddDetailToRSnStationDetail(string SNDetailID,R_SN SN,string Line,string StationName,string DeviceName, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            T_R_SN_STATION_DETAIL Table_R_Sn_Station_Detail = new T_R_SN_STATION_DETAIL(DB, DBType);
            T_R_SN TRSN = new T_R_SN(DB, this.DBType);
            Row_R_SN_STATION_DETAIL row = null;

            if (SN != null && !string.IsNullOrEmpty(SN.ID))
            {
                row = (Row_R_SN_STATION_DETAIL)ConstructRow(SN);
                row.ID = SNDetailID;
                row.R_SN_ID = SN.ID;
                row.CLASS_NAME = GetWorkClass(DB);
                row.LINE = Line;
                row.STATION_NAME = StationName;
                row.DEVICE_NAME = DeviceName;
                row.EDIT_TIME = GetDBDateTime(DB);
                //row.PRODUCT_STATUS = TRSN.WorkTimes(SN.SN,StationName,DB)>0 ? "REWORK" : "FRESH";
                row.PRODUCT_STATUS = SN.PRODUCT_STATUS;
                
                
                if (this.DBType == DB_TYPE_ENUM.Oracle)
                {
                    sql = row.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                    return result;

                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
            }
            return result;
        }

        /// <summary>
        /// BIP FAIL R_SN_STATION_DETAIL REPAIR_FAILED_FLAG=1
        /// </summary>
        /// <param name="SNDetailID"></param>
        /// <param name="SN"></param>
        /// <param name="Line"></param>
        /// <param name="StationName"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DB"></param>
        /// <param name="Fail_Flag"></param>
        /// <returns></returns>
        public string AddDetailToBipStationFailDetail(string SNDetailID, R_SN SN, string Line, string StationName, string DeviceName, OleExec DB,string Fail_Flag)
        {
            string result = string.Empty;
            string sql = string.Empty;
            T_R_SN_STATION_DETAIL Table_R_Sn_Station_Detail = new T_R_SN_STATION_DETAIL(DB, DBType);
            Row_R_SN_STATION_DETAIL row = null;

            if (SN != null && !string.IsNullOrEmpty(SN.ID))
            {
                row = (Row_R_SN_STATION_DETAIL)ConstructRow(SN);
                row.ID = SNDetailID;
                row.R_SN_ID = SN.ID;
                row.CLASS_NAME = GetWorkClass(DB);
                row.LINE = Line;
                row.REPAIR_FAILED_FLAG = Fail_Flag;
                row.STATION_NAME = StationName;
                row.DEVICE_NAME = DeviceName;
                //row.EDIT_TIME = GetDBDateTime(DB);
                if (this.DBType == DB_TYPE_ENUM.Oracle)
                {
                    sql = row.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                    return result;

                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
            }
            return result;
        }
        /// <summary>
        /// 獲取工單在某個工站過站數量
        /// </summary>
        /// <param name="strWo"></param>
        /// <param name="stationname"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int GetCountByWOAndStation(string strWo,string stationname, OleExec DB)
        {
            string strSql = $@"select count(distinct m.sn) from r_sn_station_detail m  where workorderno=:strWo and station_name=:stationname";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":strWo",OleDbType.VarChar,100 );
            paramet[0].Value = strWo;
            paramet[1] = new OleDbParameter(":stationname",OleDbType.VarChar,100);
            paramet[1].Value = stationname;
            int result =Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text, paramet));
            return result;
        }
        public int GetCountPassQtyByWOAndStation(string StrWo, string StrStation, OleExec DB)
        {
            string strsql = $@"select count(distinct m.sn) from r_sn_station_detail m  where workorderno=:strWo and station_name=:stationname and REPAIR_FAILED_FLAG=0";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":StrWo", OleDbType.VarChar, 100);
            paramet[0].Value = StrWo;
            paramet[1] = new OleDbParameter(":Strstation", OleDbType.VarChar, 100);
            paramet[1].Value = StrStation;
            int result = Convert.ToInt32(DB.ExecuteScalar(strsql, CommandType.Text, paramet));
            return result;
        }
         

        /// <summary>
        /// 檢查機種鎖定鎖定(試製工單,試製機種被鎖定,僅有一個SN可以過卡通,LABEL_SIGN FOR HWT)
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="StrSn"></param>
        /// <param name="StrStation"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetFirstPassSnBySnAndStation(string StrWo, string StrStation, OleExec DB)
        {
            string strsql = $@"select sn from (select m.sn from r_sn_station_detail m  where WORKORDERNO='{StrWo}' and station_name='{StrStation}' and REPAIR_FAILED_FLAG=0 order by EDIT_TIME ) where rownum=1";
               DataTable dt = DB.ExecSelect(strsql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }

             
        }

        public int GetCountFailQtyByWOAndStation(string StrWo, string StrStation, OleExec DB)
        {
            string strsql = $@"select count(distinct m.sn) from r_sn_station_detail m  where workorderno=:strWo and station_name=:stationname and REPAIR_FAILED_FLAG=1";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":StrWo", OleDbType.VarChar, 100);
            paramet[0].Value = StrWo;
            paramet[1] = new OleDbParameter(":Strstation", OleDbType.VarChar, 100);
            paramet[1].Value = StrStation;
            int result = Convert.ToInt32(DB.ExecuteScalar(strsql, CommandType.Text, paramet));
            return result;
        }
        public int GetCountByWOAndStationNotContailFail(string strWo, string stationname, OleExec DB)
        {
            string strSql = $@"select count(distinct m.sn) from r_sn_station_detail m  where workorderno=:strWo and station_name=:stationname and REPAIR_FAILED_FLAG<>1";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":strWo", strWo);
            paramet[1] = new OleDbParameter(":stationname", stationname);
            int result = Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text, paramet));
            return result;
        }
        /// <summary>
        /// 分板工站替換序列號
        /// 黄杨盛 2018年4月14日10:00:27 增加edit_time,改为使用参数形式,注意:若MYSQL或SQLSERVER需要使用需要在执行前把TEXT中的:符号替换为@符号
        /// 黄杨盛 2018年4月28日09:39:29 取消edit_time,这个并没有使用id,而是直接用了sn
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void UpdateRSnStationDetailBySNID(string SN,string SN_ID,OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            //sql = $@"update r_sn_station_detail set sn='{SN}' where sn='{SN_ID}'";
            //DB.ExecSQL(sql);

            var editTime = GetDBDateTime(DB);
            sql = $@"update r_sn_station_detail set sn=:SN where sn=:ID";
            var parameters = new OleDbParameter[2]
            {
                new OleDbParameter("SN", SN) {DbType = DbType.String},
                new OleDbParameter("ID", SN_ID) {DbType = DbType.String}
            };
     
            DB.ExecuteNonQuery(sql, CommandType.Text, parameters);
        }

        public R_SN_STATION_DETAIL GetSNLastPassStationDetail(string SN, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select*from (select*from r_sn_station_detail where sn='{SN}' and repair_failed_flag='0' order by edit_time desc)a where rownum=1 ";
            Dt= DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }
        public R_SN_STATION_DETAIL GetSNORTPassStationDetail(string SN, string CREATE_DATE, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select*from (select*from r_sn_station_detail where sn='{SN}' and station_name='ORT-FT2' and edit_time> (select max(create_date) from r_lot_detail d where d.sn='{SN}' and exists 
                        (select * from r_lot_status s where s.id=d.lot_id and s.skuno=d.workorderno and s.sample_station='ORT')) order by edit_time desc)a where rownum=1";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }

        public R_SN_STATION_DETAIL GetSntationPassDetail(string SN, string StationName, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select *
                            from (select *
                                    from ((select *
                                            from r_sn_station_detail
                                            where sn = '{SN}'
                                            and STATION_NAME = '{StationName}'
                                            and VALID_FLAG = '1'
                                            order by edit_time desc))
                                    where rownum = 1)
                            where VALID_FLAG = 1";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }

        public R_SN_STATION_DETAIL GetLastStationByWo(string WO, string StationName, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_STATION_DETAIL>()
                .Where(
                it => it.VALID_FLAG == "1"
                && it.STATION_NAME == StationName
                 && it.WORKORDERNO == WO
                ).OrderBy(it => it.EDIT_TIME, SqlSugar.OrderByType.Desc).First();
        }

        public R_SN_STATION_DETAIL GetRMASntationPassDetail(string SN, string StationName, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select *
                            from (select *
                                    from ((select *
                                            from r_sn_station_detail
                                            where sn = '{SN}'
                                            and STATION_NAME = '{StationName}'
                                            and VALID_FLAG = '1'
                                            order by edit_time desc))
                                    where rownum = 1)
                            where VALID_FLAG = 1
                            and SUBSTR(workorderno, 1, 6) IN
                                (SELECT PREFIX
                                    FROM r_wo_type
                                    WHERE WORKORDER_TYPE = 'REWORK'
                                    AND PRODUCT_TYPE = 'RMA')";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }

        public R_SN_STATION_DETAIL GetReturnStation(string SN, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select right(device_name, (LENGTH(DEVICE_NAME) - INSTR(DEVICE_NAME, '>'))) ReturnStation
                          from r_sn_station_detail
                         where EDIT_TIME = (select MAX(EDIT_TIME)
                                              from r_sn_station_detail
                                             where STATION_NAME = 'RETURN'
                                               and sn = '{SN}')
                           and sn = '{SN}'
                           and STATION_NAME = 'RETURN'";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }

        public R_SN_STATION_DETAIL CheckSnAfterReturn(string SN,string StationName , OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@" SELECT *
                          FROM (SELECT *
                                  FROM (SELECT *
                                          FROM r_sn_station_detail
                                         WHERE sn = '{SN}'
                                           AND VALID_FLAG = 1
                                           AND station_name = '{StationName}'
                                           AND EDIT_TIME > (select MAX(EDIT_TIME)
                                                              from r_sn_station_detail
                                                             where STATION_NAME = 'RETURN'
                                                               and sn = '{SN}')
                                         ORDER BY EDIT_TIME DESC)
                                 WHERE ROWNUM = 1)";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }



        public R_SN_STATION_DETAIL GetMrbSntationPassDetail(string SN, string StationName, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select *
                            from (select *
                                from ((SELECT *
                                                FROM r_mrb
                                                WHERE SN = '{SN}'
                                                    AND NEXT_STATION = '{StationName}'
                                                    AND rework_WO IS NULL
                                                ORDER BY EDIT_TIME DESC))
                                where rownum = 1)
                            where  SUBSTR(workorderno, 1, 6) IN
                            (SELECT PREFIX
                                FROM r_wo_type
                                WHERE WORKORDER_TYPE = 'REWORK'
                                AND PRODUCT_TYPE = 'RMA')";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }

        public R_SN_STATION_DETAIL GetQuitSntationPassDetail(string SN, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select * from c_control where CONTROL_NAME='TC_SCRAP' and CONTROL_VALUE='{SN}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }

        public R_SN_STATION_DETAIL CheckRetureOrnot(string Sn, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;
                StrSql = $@"select * from r_sn_station_detail where STATION_NAME = 'RETURN'and sn='{Sn}'";
                Dt = DB.ExecSelect(StrSql).Tables[0];

                if (Dt.Rows.Count > 0)
                {
                    Rows.loadData(Dt.Rows[0]);
                    R_Sn_Station_Detail = Rows.GetDataObject();
                }
            return R_Sn_Station_Detail;

        }

        /// <summary>
        /// 用於檢查kit是否點SMTLOADING START
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_SN_STATION_DETAIL GetSNSmtLoadingStart(string SN, OleExec DB)
        {
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"select*from (select*from r_sn_station_detail where sn='{SN}' and STATION_NAME ='SMTLOADING' order by edit_time desc )a where rownum=1 ";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Station_Detail = Rows.GetDataObject();
            }

            return R_Sn_Station_Detail;
        }

        public int ReplaceSnStationDetail(string NewSn, string OldSn, OleExec DB,DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_SN_STATION_DETAIL R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            

            return result;
        }

        public R_SN_STATION_DETAIL GetDetailBySnAndStation(string Sn, string StationName, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == Sn && t.STATION_NAME == StationName).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 獲取某一時間段內掃入STOCKIN的機種及其數量,用於STOCKIN BACKFLUSH拋帳用
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public DataTable GetStockInQtyByTime(DateTime startTime,DateTime endTime,OleExec sfcdb)
        {
          string  sql = $@"select m.skuno,count(1) qty
                          from r_sn_station_detail m
                         where m.station_name = 'STOCKIN'
                           and m.edit_time >= to_date('{startTime.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy/mm/dd hh24:mi:ss')
                           and m.edit_time < to_date('{endTime.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy/mm/dd hh24:mi:ss') group by m.skuno";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if(dt.Rows.Count>0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }


        public List<R_SN_STATION_DETAIL> GetSNStationDetailByPanel(string PanelSn, OleExec DB)
        {
            List<R_SN_STATION_DETAIL> R_SN_STATION_DETAIL_LIST = new List<R_SN_STATION_DETAIL>();
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string result = string.Empty;
            string StrSql = string.Empty;

            StrSql = $@"SELECT * FROM R_SN_STATION_DETAIL WHERE SN IN (SELECT SN FROM R_PANEL_SN WHERE PANEL = '{PanelSn}') order by id ";
            Dt = DB.ExecSelect(StrSql).Tables[0];

            foreach(DataRow r in Dt.Rows)
            {
                Rows.loadData(r);
                R_Sn_Station_Detail = Rows.GetDataObject();
                R_SN_STATION_DETAIL_LIST.Add(R_Sn_Station_Detail);
            }
            return R_SN_STATION_DETAIL_LIST;
        }

        /// <summary>
        ///check if the last station of the SN in the route is PASS
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="station"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public bool TheLastStationIsPass(string sn, string station, OleExec sfcdb)
        {
            bool bPass = true;
            C_ROUTE_DETAIL lastSation = null;
            R_SN_STATION_DETAIL r_sn_station_detail = null;
            T_R_SN t_r_sn = new T_R_SN(sfcdb,this.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, this.DBType);

            R_SN snObject = t_r_sn.LoadSN(sn,sfcdb);
            if (snObject != null)
            {               
                if (!t_c_route_detail.StationInRoute(snObject.ROUTE_ID, station, sfcdb))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000156", new string[] { sn, station }));
                }
                C_ROUTE_DETAIL currentStation = t_c_route_detail.GetByRouteIdOrderBySEQASC(snObject.ROUTE_ID, sfcdb).Find(s => s.STATION_NAME == station);
                List<C_ROUTE_DETAIL> routeList = t_c_route_detail.GetByRouteIdOrderBySEQASC(snObject.ROUTE_ID, sfcdb).FindAll(s => s.SEQ_NO < currentStation.SEQ_NO);
                if (routeList.Count > 0)
                {
                    lastSation = routeList.OrderBy(t => t.SEQ_NO).LastOrDefault();
                    if (lastSation != null)
                    {
                        r_sn_station_detail = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(p => p.STATION_NAME == lastSation.STATION_NAME &&
                         p.ROUTE_ID == lastSation.ROUTE_ID && p.SN == snObject.SN && p.R_SN_ID == snObject.ID).ToList().FirstOrDefault();
                        if (r_sn_station_detail == null)
                        {
                            bPass = false;
                        }
                    }
                }
            }   
            return bPass;
        }

        /// <summary>
        ///check if the sn is write into r_sn_station_detail
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="station"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public bool HadWriteIntoDetail(string sn, string station, OleExec sfcdb)
        {
            bool bPass = true;
            R_SN_STATION_DETAIL r_sn_station_detail = null;
            T_R_SN t_r_sn = new T_R_SN(sfcdb, this.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, this.DBType);

            R_SN snObject = t_r_sn.LoadSN(sn, sfcdb);
            if (snObject != null)
            {
                if (!t_c_route_detail.StationInRoute(snObject.ROUTE_ID, station, sfcdb))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000156", new string[] { sn, station }));
                }
                r_sn_station_detail = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(p => p.STATION_NAME == station &&
                         p.ROUTE_ID == snObject.ROUTE_ID && p.SN == snObject.SN && p.R_SN_ID == snObject.ID).ToList().FirstOrDefault();
                if (r_sn_station_detail == null)
                {
                    bPass = false;
                }
            }
            return bPass;
        }

        public string InsertOneRowORT(string bu, string skuno, string wo, string sn, string flag, string empNo, OleExec db, DB_TYPE_ENUM dbtype)
        {


            T_R_ORTSAMPLE R_ORTSAMPLE = new T_R_ORTSAMPLE(db, dbtype);
            Row_R_ORTSAMPLE rowORTSample = (Row_R_ORTSAMPLE)R_ORTSAMPLE.NewRow();
            rowORTSample.ID = R_ORTSAMPLE.GetNewID(bu, db);
            rowORTSample.SKUNO = skuno;
            rowORTSample.WO = wo;
            rowORTSample.SN = sn;
            rowORTSample.LOCKED_FLAG = flag;
            rowORTSample.EDIT_EMP = empNo;
            rowORTSample.EDIT_TIME = R_ORTSAMPLE.GetDBDateTime(db);
            return db.ExecSQL(rowORTSample.GetInsertString(dbtype));
        }

        /// <summary>
        /// 查詢過站記錄同一產品，同一線別下是否連續3片板子fail
        /// </summary>
        /// <param name="stationName"></param>
        /// <param name="skuno"></param>
        /// <param name="failCode"></param>
        /// <param name="line"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CountFailsBySkunoAndLine(string stationName, string skuno, string failCode, string line, string FAIL_LOCATION, OleExec DB)
        {
            string sql = $@"select * from (select * from (select * from r_sn_station_detail where station_name = '{stationName}' and line = '{line}' order by edit_time desc ) where rownum <=3 ) a where a.repair_failed_flag = '1' and a.skuno = '{skuno}' and exists(select * from r_repair_failcode b where a.sn = b.sn and b.fail_code = '{failCode}' and FAIL_LOCATION='{FAIL_LOCATION}' ) and sn not in (select sn from r_sn_line_stop where stop_type = 'AOISTOPSN' and stop_station = '{stationName}' and stop_line = '{line}' and edit_time > sysdate-1)";
            DataTable Dt = new DataTable();
            Dt = DB.ExecSelect(sql).Tables[0];
            if (Dt.Rows.Count >= 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CountFailsBySnAndLine(string stationName, string skuno, string line, OleExec DB)
        {
            string sql = $@"select * from (select * from (select * from r_sn_station_detail where station_name = '{stationName}' and line = '{line}' order by edit_time desc ) where rownum <=3 ) a where a.repair_failed_flag = '1' and a.skuno = '{skuno}' and not exists(select sn from r_sn_line_stop b where b.sn = a.sn)";
            DataTable Dt = new DataTable();
            Dt = DB.ExecSelect(sql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<R_SN_STATION_DETAIL> CheckSNStationPass(string SN, List<string> Station, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == SN && Station.Contains(t.CURRENT_STATION)).ToList();
        }
        public List<R_SN_STATION_DETAIL> CheckSNStationPass(string SN, string Station, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == SN && t.CURRENT_STATION == Station).ToList();
        }
        public int INSNStationDetail(Row_R_SN_STATION_DETAIL SNStationDetatil, OleExec DB)
        {
            return DB.ORM.Insertable<R_SN_STATION_DETAIL>(SNStationDetatil).ExecuteCommand();
        }

        public int SaveStationDetail(R_SN_STATION_DETAIL SNStationDetatil, OleExec DB)
        {
            return DB.ORM.Insertable<R_SN_STATION_DETAIL>(SNStationDetatil).ExecuteCommand();
        }

        public List<R_SN_STATION_DETAIL> GetSNBYStatinDetail(string LikeSNStar, string WO, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_STATION_DETAIL, R_SN>((p1, p2) => p1.SN == p2.SN && (p2.SN == LikeSNStar && p2.CURRENT_STATION == WO) && (p1.SN == LikeSNStar && p1.CURRENT_STATION == WO)).ToList();
        }
        public List<R_SN_STATION_DETAIL> GetSNStationDetail(List<string> SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => SN.Contains(t.SN)).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_SN_STATION_DETAIL> GetSNRepairTEST(string SN, OleExec DB)
        {
            List<R_SN_STATION_DETAIL> R_SN_STATION_DETAIL_LIST = new List<R_SN_STATION_DETAIL>();
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = string.Empty;
            StrSql = $@"SELECT * FROM R_SN_STATION_DETAIL WHERE SN='{SN}' AND EDIT_TIME IN         
   (SELECT MAX(EDIT_TIME) FROM R_SN_STATION_DETAIL WHERE SN='{SN}') AND REPAIR_FAILED_FLAG = '1' AND EDIT_TIME NOT IN (    
   SELECT CREATE_TIME FROM R_REPAIR_TRANSFER  WHERE SN = '{SN}'  ) ";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            foreach (DataRow r in Dt.Rows)
            {
                Rows.loadData(r);
                R_Sn_Station_Detail = Rows.GetDataObject();
                R_SN_STATION_DETAIL_LIST.Add(R_Sn_Station_Detail);
            }
            return R_SN_STATION_DETAIL_LIST;
        }
        public List<R_SN_STATION_DETAIL> GetSNBYSNValid(string SN, string Vailid_Flag, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == SN && t.REPAIR_FAILED_FLAG == Vailid_Flag).ToList();
        }


        /// <summary>
        /// 獲取該工站/該線別/該料號過站FAIL記錄數量(目前AOI過站使用)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Productline"></param>
        /// <param name="Skuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int FailCountByStationAndSkunoAndLine(string Station, string Productline, string Skuno, int Continuity_Flag, OleExec DB)
        {

            string CountSql = $@"
                select count(*) from 
                        (
	                    select * from 
		                    (select * from R_SN_STATION_DETAIL where NEXT_STATION=:station and LINE=:productline order by EDIT_TIME desc )
	                    where rownum<21
                        )
                where REPAIR_FAILED_FLAG='1' and SKUNO=:skuno
            ";
            //此處存疑,REPAIR_FAILED_FLAG? VALID_FLAG?

            OleDbParameter[] paramet = new OleDbParameter[]
            {
                    new OleDbParameter(":station", Station),
                    new OleDbParameter(":productline", Productline),
                    new OleDbParameter(":skuno", Skuno)

            };


            int result = int.Parse(DB.ExecuteScalar(CountSql, CommandType.Text, paramet));

            return result;
        }
        public List<R_SN_STATION_DETAIL> GetRepairNotA(string SN, OleExec DB)
        {
            List<R_SN_STATION_DETAIL> R_SN_STATION_DETAIL_LIST = new List<R_SN_STATION_DETAIL>();
            R_SN_STATION_DETAIL R_Sn_Station_Detail = null;
            Row_R_SN_STATION_DETAIL Rows = (Row_R_SN_STATION_DETAIL)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = string.Empty;
            StrSql = $@"SELECT * FROM R_SN_STATION_DETAIL WHERE SN = '{SN}' AND EDIT_TIME IN (        
    SELECT MAX(EDIT_TIME) FROM R_SN_STATION_DETAIL WHERE SN = '{SN}') AND REPAIR_FAILED_FLAG = '1' AND     
    NOT EXISTS (SELECT * FROM R_REPAIR_TRANSFER WHERE SN = '{SN}' AND CREATE_TIME = R_SN_STATION_DETAIL.EDIT_TIME)";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            foreach (DataRow r in Dt.Rows)
            {
                Rows.loadData(r);
                R_Sn_Station_Detail = Rows.GetDataObject();
                R_SN_STATION_DETAIL_LIST.Add(R_Sn_Station_Detail);
            }
            return R_SN_STATION_DETAIL_LIST;
        }
        public DataTable SelectAOIFail(string Station, string Line, string SKUNO, string RepairFailedflag, OleExec DB)
        {
            string StrSql = $@" SELECT R_SN_ID,'AOISTOPSN' AOISTOPSN,SN,NEXT_STATION,SYSDATE,LINE
   FROM (SELECT * FROM R_SN_STATION_DETAIL 
      WHERE NEXT_STATION='{Station}' AND  LINE='{Line}'   
      ORDER BY EDIT_TIME DESC) A   
     WHERE REPAIR_FAILED_FLAG='{RepairFailedflag}' AND SKUNO='{SKUNO}' 
   AND NOT EXISTS(SELECT * FROM R_SN_LINE_STOP B WHERE A.SN=B.SN)  ";
            DataTable ResDT = DB.ExecuteDataSet(StrSql, CommandType.Text, null).Tables[0];
            return ResDT;
        }
        public string GetBYShift(string TIME)
        {
            string Shift = "";
            DateTime Time = new DateTime();
            if (string.IsNullOrEmpty(TIME))
            {
                Time = DateTime.Parse(DateTime.Now.ToShortTimeString().ToString());
            }
            else
            {
                Time = DateTime.Parse(TIME);
            }
            if ((Time > DateTime.Parse("00:00") && Time < DateTime.Parse("07:59")) || (Time > DateTime.Parse("20:00") && Time < DateTime.Parse("23:59")))
            {
                Shift = "SHIFT 2";
            }
            else
            {
                Shift = "SHIFT 1";
            }
            return Shift;
        }
        public List<R_SN_STATION_DETAIL> GetSNBSNStationDetail(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => SN.Contains(t.SN)).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public int Update(R_SN_STATION_DETAIL objDetail, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN_STATION_DETAIL>(objDetail).Where(r => r.ID == objDetail.ID).ExecuteCommand();
        }
    }
    public class Row_R_SN_STATION_DETAIL : DataObjectBase
    {
        public Row_R_SN_STATION_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_STATION_DETAIL GetDataObject()
        {
            R_SN_STATION_DETAIL DataObject = new R_SN_STATION_DETAIL();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PLANT = this.PLANT;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.LINE = this.LINE;
            DataObject.STARTED_FLAG = this.STARTED_FLAG;
            DataObject.START_TIME = this.START_TIME;
            DataObject.PACKED_FLAG = this.PACKED_FLAG;
            DataObject.PACKED_TIME = this.PACKED_TIME;
            DataObject.COMPLETED_FLAG = this.COMPLETED_FLAG;
            DataObject.COMPLETED_TIME = this.COMPLETED_TIME;
            DataObject.SHIPPED_FLAG = this.SHIPPED_FLAG;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.REPAIR_FAILED_FLAG = this.REPAIR_FAILED_FLAG;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.KP_LIST_ID = this.KP_LIST_ID;
            DataObject.PO_NO = this.PO_NO;
            DataObject.CUST_ORDER_NO = this.CUST_ORDER_NO;
            DataObject.CUST_PN = this.CUST_PN;
            DataObject.BOXSN = this.BOXSN;
            DataObject.DEVICE_NAME = this.DEVICE_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SCRAPED_FLAG = this.SCRAPED_FLAG;
            DataObject.SCRAPED_TIME = this.SCRAPED_TIME;
            DataObject.PRODUCT_STATUS = this.PRODUCT_STATUS;
            DataObject.REWORK_COUNT = this.REWORK_COUNT;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }

        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string ROUTE_ID
        {
            get
            {
                return (string)this["ROUTE_ID"];
            }
            set
            {
                this["ROUTE_ID"] = value;
            }
        }

        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string STARTED_FLAG
        {
            get
            {
                return (string)this["STARTED_FLAG"];
            }
            set
            {
                this["STARTED_FLAG"] = value;
            }
        }
        public DateTime? START_TIME
        {
            get
            {
                return (DateTime?)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public string PACKED_FLAG
        {
            get
            {
                return (string)this["PACKED_FLAG"];
            }
            set
            {
                this["PACKED_FLAG"] = value;
            }
        }
        public DateTime? PACKED_TIME
        {
            get
            {
                return (DateTime?)this["PACKED_TIME"];
            }
            set
            {
                this["PACKED_TIME"] = value;
            }
        }
        public string COMPLETED_FLAG
        {
            get
            {
                return (string)this["COMPLETED_FLAG"];
            }
            set
            {
                this["COMPLETED_FLAG"] = value;
            }
        }
        public DateTime? COMPLETED_TIME
        {
            get
            {
                return (DateTime?)this["COMPLETED_TIME"];
            }
            set
            {
                this["COMPLETED_TIME"] = value;
            }
        }
        public string SHIPPED_FLAG
        {
            get
            {
                return (string)this["SHIPPED_FLAG"];
            }
            set
            {
                this["SHIPPED_FLAG"] = value;
            }
        }
        public DateTime? SHIPDATE
        {
            get
            {
                return (DateTime?)this["SHIPDATE"];
            }
            set
            {
                this["SHIPDATE"] = value;
            }
        }
        public string REPAIR_FAILED_FLAG
        {
            get
            {
                return (string)this["REPAIR_FAILED_FLAG"];
            }
            set
            {
                this["REPAIR_FAILED_FLAG"] = value;
            }
        }
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
            }
        }
        public string NEXT_STATION
        {
            get
            {
                return (string)this["NEXT_STATION"];
            }
            set
            {
                this["NEXT_STATION"] = value;
            }
        }
        public string KP_LIST_ID
        {
            get
            {
                return (string)this["KP_LIST_ID"];
            }
            set
            {
                this["KP_LIST_ID"] = value;
            }
        }
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string CUST_ORDER_NO
        {
            get
            {
                return (string)this["CUST_ORDER_NO"];
            }
            set
            {
                this["CUST_ORDER_NO"] = value;
            }
        }
        public string CUST_PN
        {
            get
            {
                return (string)this["CUST_PN"];
            }
            set
            {
                this["CUST_PN"] = value;
            }
        }
        public string BOXSN
        {
            get
            {
                return (string)this["BOXSN"];
            }
            set
            {
                this["BOXSN"] = value;
            }
        }

        public string DEVICE_NAME
        {
            get
            {
                return (string)this["DEVICE_NAME"];
            }
            set
            {
                this["DEVICE_NAME"] = value;
            }
        }

        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string SCRAPED_FLAG
        {
            get
            {
                return (string)this["SCRAPED_FLAG"];
            }
            set
            {
                this["SCRAPED_FLAG"] = value;
            }
        }
        public DateTime? SCRAPED_TIME
        {
            get
            {
                return (DateTime?)this["SCRAPED_TIME"];
            }
            set
            {
                this["SCRAPED_TIME"] = value;
            }
        }
        public string PRODUCT_STATUS
        {
            get
            {
                return (string)this["PRODUCT_STATUS"];
            }
            set
            {
                this["PRODUCT_STATUS"] = value;
            }
        }
        public double? REWORK_COUNT
        {
            get
            {
                return (double?)this["REWORK_COUNT"];
            }
            set
            {
                this["REWORK_COUNT"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_SN_STATION_DETAIL
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{get;set;}
        public string R_SN_ID{get;set;}
        public string SN{get;set;}
        public string SKUNO{get;set;}
        public string WORKORDERNO{get;set;}
        public string PLANT{get;set;}
        public string CLASS_NAME{get;set;}
        public string ROUTE_ID{get;set;}
        public string LINE{get;set;}
        public string STARTED_FLAG{get;set;}
        public DateTime? START_TIME{get;set;}
        public string PACKED_FLAG{get;set;}
        public DateTime? PACKED_TIME{get;set;}
        public string COMPLETED_FLAG{get;set;}
        public DateTime? COMPLETED_TIME{get;set;}
        public string SHIPPED_FLAG{get;set;}
        public DateTime? SHIPDATE{get;set;}
        public string REPAIR_FAILED_FLAG{get;set;}
        public string CURRENT_STATION{get;set;}
        public string NEXT_STATION{get;set;}
        public string KP_LIST_ID{get;set;}
        public string PO_NO{get;set;}
        public string CUST_ORDER_NO{get;set;}
        public string CUST_PN{get;set;}
        public string BOXSN{get;set;}
        public string DEVICE_NAME{get;set;}
        public string STATION_NAME{get;set;}
        public string SCRAPED_FLAG{get;set;}
        public DateTime? SCRAPED_TIME{get;set;}
        public string PRODUCT_STATUS{get;set;}
        public double? REWORK_COUNT{get;set;}
        public string VALID_FLAG{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set; }

    }

    /// <summary>
    /// R_SN_STATION_DETAIL 的縮寫類，用於SqlSugar
    /// </summary>
    [SugarTable("R_SN_STATION_DETAIL")]
    public class R_SN_S_D
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string WORKORDERNO { get; set; }
        public string PLANT { get; set; }
        public string CLASS_NAME { get; set; }
        public string ROUTE_ID { get; set; }
        public string LINE { get; set; }
        public string STARTED_FLAG { get; set; }
        public DateTime? START_TIME { get; set; }
        public string PACKED_FLAG { get; set; }
        public DateTime? PACKED_TIME { get; set; }
        public string COMPLETED_FLAG { get; set; }
        public DateTime? COMPLETED_TIME { get; set; }
        public string SHIPPED_FLAG { get; set; }
        public DateTime? SHIPDATE { get; set; }
        public string REPAIR_FAILED_FLAG { get; set; }
        public string CURRENT_STATION { get; set; }
        public string NEXT_STATION { get; set; }
        public string KP_LIST_ID { get; set; }
        public string PO_NO { get; set; }
        public string CUST_ORDER_NO { get; set; }
        public string CUST_PN { get; set; }
        public string BOXSN { get; set; }
        public string DEVICE_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string SCRAPED_FLAG { get; set; }
        public DateTime? SCRAPED_TIME { get; set; }
        public string PRODUCT_STATUS { get; set; }
        public double? REWORK_COUNT { get; set; }
        public string VALID_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }

    }

    public enum R_SN_STATION_DETAIL_ENUM
    {
        /// <summary>
        /// 记录有效
        /// </summary>
        [EnumValue("1")]
        VALID_FLAG_TRUE,
        /// <summary>
        /// 记录无效
        /// </summary>
        [EnumValue("0")]
        VALID_FLAG_FALSE,
        /// <summary>
        /// 过站Pass
        /// </summary>
        [EnumValue("0")]
        REPAIR_FAILED_FLAG_PASS,
        /// <summary>
        /// 过站Fail
        /// </summary>
        [EnumValue("1")]
        REPAIR_FAILED_FLAG_FAIL
    }
}