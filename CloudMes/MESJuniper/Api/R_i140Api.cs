using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESPubLab;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using Newtonsoft.Json.Linq;
using MESJuniper.Base;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.Common;
using MESPubLab.MesBase;
using System.Drawing;
using System.IO;
using MESDataObject.Module.OM;

namespace MESJuniper.Api
{
    public class R_i140Api : MesAPIBase
    {
        public void GetCurrentBu(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //var currentBu = System.Configuration.ConfigurationManager.AppSettings["CurrentBu"].ToString();
            //改取登錄用戶選擇的BU
            var currentBu = this.BU;
            if (currentBu.Equals("VNJUNIPER"))
                StationReturn.Data = "0016000220";
            else
                StationReturn.Data = "0016000219";
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000026";
        }

        public void GetR_i140DataBySite(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string Site = Data["Site"].ToString().Trim();


                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_I140>()
                    .ToList().Distinct().ToList();
                if (int.Parse(Site) > -1)
                    res = res.FindAll(t => t.VENDORCODE == Site);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetR_i140_rebuiltDataBySite(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string Site = Data["Site"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                string str_SITE = string.Empty;

                if (Site != "ALL")
                {
                    str_SITE = $@" AND VENDORCODE ='{Site}'";
                }


                string strSql =
                $@"select wm_concat(CC.STARTDATETIME) as name from (
                SELECT  distinct to_char(STARTDATETIME,'YYYY-MM-DD') STARTDATETIME      FROM R_I140
                WHERE 1=1 {str_SITE} order by STARTDATETIME)cc";
                DataTable dt = oleDB.ExecSelect(strSql).Tables[0];
                string column_names = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    column_names = dt.Rows[0][0].ToString();
                }
                column_names = column_names.Replace(",", "','");
                column_names = "'" + column_names + "'";

                strSql = $@"SELECT * FROM (
                SELECT decode( vendorcode,'0016000219','FJZ','0016000220','FVN',vendorcode) AS SITE,
                   TRANID,PN, STARTDATETIME as name,COUNT(*) as qty FROM
                   (SELECT VENDORCODE, TRANID,PN, to_char(STARTDATETIME, 'YYYY-MM-DD') STARTDATETIME FROM R_I140
                   WHERE 1=1 {str_SITE} )AA
                    GROUP BY VENDORCODE, TRANID,PN, STARTDATETIME ORDER BY VENDORCODE,TRANID,PN,STARTDATETIME) BB
                PIVOT(SUM(BB.QTY) FOR name IN({ column_names}))";

                var res = oleDB.ExecSelect(strSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetR_i140_main_d_Data(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var plant = Data["Plant"].ToString().Trim();
                string strSql = $@"
                            SELECT
                                aa.plant,
                                aa.weekno,
                                aa.yearno,
                                aa.complete,
                                aa.partnum,
                                aa.itemnum,
                                aa.createtime,
                                aa.edittime,
                                bb.*
                            FROM
                                r_i140_main aa,
                                (
                                    SELECT
                                        tranid,
                                        decode(一, '0', 'N', 'Y') 一,
                                        decode(二, '0', 'N', 'Y') 二,
                                        decode(三, '0', 'N', 'Y') 三,
                                        decode(四, '0', 'N', 'Y') 四,
                                        decode(五, '0', 'N', 'Y') 五,
                                        decode(六, '0', 'N', 'Y') 六,
                                        decode(日, '0', 'N', 'Y') 日
                                    FROM
                                        (
                                            SELECT
                                                *
                                            FROM
                                                (
                                                    SELECT
                                                        tranid,
                                                        commitday,
                                                        complete
                                                    FROM
                                                        r_i140_main_d a
                                                    WHERE
                                                        EXISTS (
                                                            SELECT
                                                                1
                                                            FROM
                                                                r_i140_main b
                                                            WHERE
                                                                a.tranid = b.tranid
                                                                AND complete = 0
                                                        )
                                                    GROUP BY
                                                        tranid,
                                                        commitday,
                                                        complete
                                                ) bb PIVOT (
                                                    SUM ( bb.complete )
                                                    FOR commitday
                                                    IN ( 'Monday' AS 一, 'Tuesday' AS 二, 'Wednesday' AS 三, 'Thursday' AS 四, 'Friday' AS 五, 'Saturday' AS 六 , 'Sunday' AS 日  )
                                                )
                                        )
                                ) bb
                            WHERE
                                aa.tranid = bb.tranid 
            ";
                if (plant != "-1")
                    strSql += $@" and plant='{plant}' ";
                strSql += " order by aa.tranid desc ";
                var res = oleDB.ExecSelect(strSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void GetR_i140_His(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                //var plant = Data["Plant"].ToString().Trim();
                string strSql = $@" select * from r_i140_main order by createtime desc ";
                //    string strSql = $@"
                //                SELECT
                //                    aa.plant,
                //                    aa.weekno,
                //                    aa.yearno,
                //                    aa.complete,
                //                    aa.partnum,
                //                    aa.itemnum,
                //                    aa.createtime,
                //                    aa.edittime,
                //                    bb.*
                //                FROM
                //                    r_i140_main aa,
                //                    (
                //                        SELECT
                //                            tranid,
                //                            decode(一, '0', 'N', 'Y') 一,
                //                            decode(二, '0', 'N', 'Y') 二,
                //                            decode(三, '0', 'N', 'Y') 三,
                //                            decode(四, '0', 'N', 'Y') 四,
                //                            decode(五, '0', 'N', 'Y') 五,
                //                            decode(六, '0', 'N', 'Y') 六,
                //                            decode(日, '0', 'N', 'Y') 日
                //                        FROM
                //                            (
                //                                SELECT
                //                                    *
                //                                FROM
                //                                    (
                //                                        SELECT
                //                                            tranid,
                //                                            commitday,
                //                                            complete
                //                                        FROM
                //                                            r_i140_main_d a
                //                                        WHERE
                //                                            EXISTS (
                //                                                SELECT
                //                                                    1
                //                                                FROM
                //                                                    r_i140_main b
                //                                                WHERE
                //                                                    a.tranid = b.tranid
                //                                                    AND complete = 0
                //                                            )
                //                                        GROUP BY
                //                                            tranid,
                //                                            commitday,
                //                                            complete
                //                                    ) bb PIVOT (
                //                                        SUM ( bb.complete )
                //                                        FOR commitday
                //                                        IN ( 'Monday' AS 一, 'Tuesday' AS 二, 'Wednesday' AS 三, 'Thursday' AS 四, 'Friday' AS 五, 'Saturday' AS 六 , 'Sunday' AS 日  )
                //                                    )
                //                            )
                //                    ) bb
                //                WHERE
                //                    aa.tranid = bb.tranid 
                //";
                //    if (plant != "-1")
                //        strSql += $@" and plant='{plant}' ";
                //    strSql += " order by aa.tranid desc ";
                var res = oleDB.ExecSelect(strSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void GetR_i140_main_Data(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();

                string strSql = $@" SELECT TRANID, DECODE(一,'0','N','Y')一 ,
               DECODE(二,'0','N','Y')二, DECODE(三,'0','N','Y')三, 
               DECODE(四,'0','N','Y')四, DECODE(五,'0','N','Y')五 FROM ( 
               SELECT * FROM (SELECT tranid,commitday,complete 
               FROM R_I140_MAIN_D A
               WHERE EXISTS (select 1 from  R_I140_MAIN B where A.TRANID = B.TRANID AND complete=1)
                group by tranid,commitday,complete
                )BB
                PIVOT(SUM(BB.complete) FOR commitday IN('一' AS 一, '二'AS 二, '三'AS 三, '四'AS 四, '五'AS 五)))
   
            ";

                var res = oleDB.ExecSelect(strSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        /// <summary>
        /// 獲取最新140的TRANID,和創建時間
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetR_i140_lastTRANID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {

                string PLANT = Data["PLANT"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                string str_TRANID = string.Empty;
                string str_PLANT = string.Empty;

                if (PLANT != "ALL")
                {
                    str_PLANT = $@" AND VENDORCODE ='{PLANT}' ";
                }

                string strSql =
                $@"SELECT TRANID, TO_CHAR(F_LASTEDITDT, 'YYYY/MM/DD HH24:MI:SS') AS STR_DATE
                      FROM R_I140
                     WHERE 1 = 1 {str_PLANT}
                       AND F_LASTEDITDT = (SELECT MAX(F_LASTEDITDT) FROM R_I140 WHERE 1 = 1 {str_PLANT} )
                       AND ROWNUM = 1
                    ";
                DataTable dt = oleDB.ExecSelect(strSql).Tables[0];

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = dt;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void GetR_i140_rebuiltDataByTRANID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string TRANID = Data["TRANID"].ToString().Trim();
                string PLANT = Data["PLANT"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                string str_TRANID = string.Empty;
                string str_PLANT = string.Empty;

                if (TRANID != "ALL")
                {
                    str_TRANID = $@" AND TRANID ='{TRANID}'";
                }

                if (PLANT != "ALL")
                {
                    str_PLANT = $@" AND VENDORCODE ='{PLANT}' ";
                }


                if (TRANID == "LAST140")
                {
                    str_TRANID = $@"SELECT max(TRANID ) FROM R_I140 WHERE 1=1 and createtime >sysdate -30  {str_PLANT}";

                    DataTable dt2 = oleDB.ExecSelect(str_TRANID).Tables[0];

                    if (dt2.Rows.Count > 0)
                    {
                        TRANID = dt2.Rows[0][0].ToString();
                        str_TRANID = $@" AND TRANID ='{TRANID}' ";
                    }
                }

                string strSql =
                $@"select wm_concat(CC.STARTDATETIME) as name from (
                SELECT  distinct to_char(STARTDATETIME,'YYYY-MM-DD') STARTDATETIME      FROM R_I140
                WHERE 1=1 {str_TRANID}  order by STARTDATETIME)cc";
                DataTable dt = oleDB.ExecSelect(strSql).Tables[0];
                string column_names = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    column_names = dt.Rows[0][0].ToString();
                }
                column_names = column_names.Replace(",", "','");
                column_names = "'" + column_names + "'";

                strSql = $@"SELECT * FROM (
                SELECT decode( vendorcode,'0016000219','FJZ','0016000220','FVN',vendorcode) AS SITE,
                   TRANID,PN, STARTDATETIME as name,SUM(QUANTITY) as qty FROM
                   (SELECT VENDORCODE, TRANID,PN,QUANTITY, to_char(STARTDATETIME, 'YYYY-MM-DD') STARTDATETIME FROM R_I140
                   WHERE 1=1 {str_TRANID} )AA
                    GROUP BY VENDORCODE, TRANID,PN, STARTDATETIME ORDER BY VENDORCODE,TRANID,PN,STARTDATETIME) BB
                PIVOT(SUM(BB.QTY) FOR name IN({ column_names}))";

                var res = oleDB.ExecSelect(strSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetR_i140_DataByTRANID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string TRANID = Data["TRANID"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = Geti140DataByTranid(oleDB, TRANID);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        DataTable Geti140DataByTranid(OleExec oleDB, string TRANID)
        {
            string str_TRANID = string.Empty;
            if (TRANID != "ALL")

            {
                str_TRANID = $@" AND TRANID ='{TRANID}'";
            }
            var daykey = Convert.ToDateTime($@"{TRANID.Substring(0, 4)}-{TRANID.Substring(4, 2)}-{TRANID.Substring(6, 2)}");
            string strSql =
            $@"select wm_concat(CC.STARTDATETIME) as name from (
                SELECT  distinct to_char(STARTDATETIME,'YYYY-MM-DD') STARTDATETIME      FROM R_I140
                WHERE 1=1 {str_TRANID}  order by STARTDATETIME)cc";
            DataTable dt = oleDB.ExecSelect(strSql).Tables[0];
            string column_names = string.Empty;
            if (dt.Rows.Count > 0)
            {
                column_names = dt.Rows[0][0].ToString();
            }
            column_names = column_names.Replace(",", "','");
            column_names = "'" + column_names + "'";

            strSql = $@"SELECT * FROM (
                SELECT 
                   TRANID,PN,G_CODE as ""GROUP"",new_order_quantity as FGOH,SHIPMENT, STARTDATETIME as name,SUM(QUANTITY) as qty FROM
                   (SELECT VENDORCODE, TRANID,PN,G_CODE,new_order_quantity,SHIPMENT,QUANTITY, to_char(STARTDATETIME, 'YYYY-MM-DD') STARTDATETIME FROM R_I140 c left join R_SKU_JNP_G d on c.pn=d.juniper  left join
                (select a.skuno,count(distinct a.sn) as SHIPMENT from r_ship_detail a,r_sn b where  a.shipdate  between to_date('{GetCurrentQuarterFirstDay(daykey)}','yyyy-mm-dd HH24:MI:SS') and to_date('{GetCurrentTuesday(daykey).AddHours(9)}','yyyy-mm-dd HH24:MI:SS') and a.sn=b.sn group by a.skuno) bb
                   on c.pn=bb.skuno left join 
                (select b.* from r_sap_file a,r_sap_file_i605 b where a.id=b.file_id and a.file_name like '{GetCurrentTuesday(daykey).ToString("yyyyMMdd")}%' ) aa on c.pn=aa.item_name
                   WHERE 1=1 {str_TRANID} )AA
                    GROUP BY VENDORCODE, TRANID,PN,G_CODE,new_order_quantity,SHIPMENT, STARTDATETIME ORDER BY VENDORCODE,TRANID,PN,G_CODE,new_order_quantity,SHIPMENT,STARTDATETIME) BB
                PIVOT(SUM(BB.QTY) FOR name IN({ column_names}))";

            var res = oleDB.ExecSelect(strSql).Tables[0];
            return res;
        }

        DateTime GetCurrentTuesday(DateTime daykey)
        {
            var currentTue = daykey;
            while (currentTue.DayOfWeek != DayOfWeek.Tuesday)
                currentTue = currentTue.AddDays(-1);
            return currentTue;
        }

        DateTime GetCurrentQuarterFirstDay(DateTime daykey)
        {
            return Convert.ToDateTime(daykey.AddMonths(0 - (daykey.Month - 1) % 3).ToString("yyyy-MM-01"));
        }

        /// <summary>
        /// 按TRANID 和周別查找當天285數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetR_i285_FILENAME(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string TRANID = Data["TRANID"].ToString().Trim();
                string DAY = Data["DAY"].ToString().Trim();
                DAY = GetDay(DAY);
                oleDB = this.DBPools["SFCDB"].Borrow();

                DataTable dt = new DataTable();
                string strSql =
                $@"  SELECT * FROM R_I140_MAIN_D WHERE TRANID='{TRANID}' AND COMMITDAY= '{DAY}'";
                dt = oleDB.ExecSelect(strSql).Tables[0];
                string FILENAME = string.Empty;

                if (dt.Rows.Count > 0)
                {
                    FILENAME = dt.Rows[0]["COMMITID"].ToString();
                }
                else
                {
                    //new Exception($@"找不到上傳的FILENAME{strSql}!");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161342", new string[] { strSql }));
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = FILENAME;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        /// <summary>
        /// 獲取數據庫時間
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetstrDateFromDatabase(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();

                DataTable dt = new DataTable();
                string strSql =
                $@"  SELECT TO_CHAR(SYSDATE,'YYYYMMDD') AS STRDATE FROM DUAL ";
                dt = oleDB.ExecSelect(strSql).Tables[0];
                string STRDATE = string.Empty;
                STRDATE = dt.Rows[0]["STRDATE"].ToString();


                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = STRDATE;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        /// <summary>
        /// 按日期字符串查找當天285數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetR_i285_FILENAMEbyDatestr(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string STRD = Data["STRD"].ToString().Trim();
                string VENDORCODE = Data["VENDORCODE"].ToString().Trim();

                oleDB = this.DBPools["SFCDB"].Borrow();

                DataTable dt = new DataTable();
                string strSql =
                $@"  SELECT * FROM R_I140_MAIN_D WHERE  COMMITDATE= '{STRD}' AND VENDORCODE='{VENDORCODE}'";
                dt = oleDB.ExecSelect(strSql).Tables[0];
                string FILENAME = string.Empty;

                if (dt.Rows.Count > 0)
                {
                    FILENAME = dt.Rows[0]["COMMITID"].ToString();
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161342", new string[] { strSql }));
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = FILENAME;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        /// <summary>
        /// 按日期字符串查找當天285數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetR_i285_FILENAMEbyDatestrArr(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string STRD = Data["STRD"].ToString().Trim();
                string VENDORCODE = Data["VENDORCODE"].ToString().Trim();

                oleDB = this.DBPools["SFCDB"].Borrow();

                DataTable dt = new DataTable();
                string strSql =
                $@"  SELECT COMMITID,COMMITDATE  FROM R_I140_MAIN_D WHERE  COMMITDATE in ({STRD})   AND VENDORCODE='{VENDORCODE}' ORDER BY COMMITDATE";
                dt = oleDB.ExecSelect(strSql).Tables[0];
                string FILENAME = string.Empty;

                if (dt.Rows.Count == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161342", new string[] { strSql }));
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = dt;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void Geti285SourceDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string strSql = string.Empty;
                DataTable dt = new DataTable();

                string FILENAME = Data["FILENAME"].ToString().Trim();

                strSql =
                $@"  select wm_concat(CC.STARTDATE) as name from (
                SELECT  distinct to_char(STARTDATE,'YYYY-MM-DD') STARTDATE      FROM R_i285_SOURCE
                WHERE 1=1 AND FILENAME='{FILENAME}' order by STARTDATE)cc";
                dt = oleDB.ExecSelect(strSql).Tables[0];
                string column_names = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    column_names = dt.Rows[0][0].ToString();
                }
                column_names = column_names.Replace(",", "','");
                column_names = "'" + column_names + "'";

                strSql = $@"SELECT * FROM (
                SELECT SENDERID,
                  TRANID,PN, STARTDATE as name,SUM(QUANTITY) as qty FROM
                   (SELECT SENDERID AS SENDERID, TRANID,PN,QUANTITY, to_char(STARTDATE, 'YYYY-MM-DD') STARTDATE FROM R_i285_SOURCE 
                   WHERE 1=1 AND FILENAME='{FILENAME}' )AA
                    GROUP BY SENDERID, TRANID,PN, STARTDATE ORDER BY SENDERID,TRANID,PN,STARTDATE) BB
                PIVOT(SUM(BB.QTY) FOR name IN({ column_names}))";

                var res = oleDB.ExecSelect(strSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void Updatei285StockTime(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string ID = Data["ID"].ToString().Trim();
                var Stocktime = Convert.ToDateTime(Data["STOCKTIME"].ToString().Trim());
                oleDB = this.DBPools["SFCDB"].Borrow();
                oleDB.ORM.Updateable<R_I285_MAIN>().SetColumns(t => new R_I285_MAIN() { STOCKINGTIME = Stocktime }).Where(t => t.ID == ID).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void GetR_i285_rebuiltDataByTRANID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string strSql = string.Empty;
                DataTable dt = new DataTable();

                string FILENAME = Data["FILENAME"].ToString().Trim();

                strSql =
                $@"  select wm_concat(CC.STARTDATE) as name from (
                SELECT  distinct to_char(STARTDATE,'YYYY-MM-DD') STARTDATE      FROM R_i285
                WHERE 1=1 AND FILENAME='{FILENAME}' order by STARTDATE)cc";
                dt = oleDB.ExecSelect(strSql).Tables[0];
                string column_names = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    column_names = dt.Rows[0][0].ToString();
                }
                column_names = column_names.Replace(",", "','");
                column_names = "'" + column_names + "'";

                strSql = $@"SELECT * FROM (
                SELECT SENDERID,
                  TRANID,PN, STARTDATE as name,SUM(QUANTITY) as qty FROM
                   (SELECT SENDERID AS SENDERID, TRANID,PN,QUANTITY, to_char(STARTDATE, 'YYYY-MM-DD') STARTDATE FROM R_i285 
                   WHERE 1=1 AND FILENAME='{FILENAME}' )AA
                    GROUP BY SENDERID, TRANID,PN, STARTDATE ORDER BY SENDERID,TRANID,PN,STARTDATE) BB
                PIVOT(SUM(BB.QTY) FOR name IN({ column_names}))";

                var res = oleDB.ExecSelect(strSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void GetR_i140_Main_D_DataByTRANID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string TRANID = Data["TRANID"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                string str_TRANID = string.Empty;

                if (TRANID != "ALL")
                {
                    str_TRANID = $@" AND TRANID ='{TRANID}'";
                }

                string strSql = $@"    SELECT * FROM R_I140_MAIN_D
                  WHERE 1=1 {str_TRANID} order by COMMITDAY  ";

                var res = oleDB.ExecSelect(strSql).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        private string GetDay(string iDAY)
        {
            string outday = "";
            if (iDAY == "一")
            {
                outday = "Monday";
            }
            else if (iDAY == "二")
            {
                outday = "Tuesday";
            }
            else if (iDAY == "三")
            {
                outday = "Wednesday";
            }
            else if (iDAY == "四")
            {
                outday = "Thursday";
            }
            else if (iDAY == "五")
            {
                outday = "Friday";
            }
            else if (iDAY == "六")
            {
                outday = "Saturday";
            }
            else if (iDAY == "日")
            {
                outday = "Sunday";
            }
            return outday;

        }


        public void UploadR_i285Excel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            string sql = "";
            string result = "";
            string makeid = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                //定義上傳Excel的列名
                // List<string> inputTitle = new List<string> { "TRANID", "F_Plant", "FileName", "Messageid", "CreationDateTime", "SenderID", "F_LASTEDITDT", "PN" };

                List<string> inputTitle = new List<string> { "SenderID", "PN" };
                string errTitle = "";
                // string TRANID = "", F_Plant = "", FileName = "", Messageid = "", CreationDateTime = "", SenderID = "", F_LASTEDITDT = "", PN = "";

                string data = Data["ExcelData"].ToString();
                string iTRANID = Data["TRANID"].ToString();
                string iDAY = Data["DAY"].ToString();


                iDAY = GetDay(iDAY);
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool hasErr = CheckInputExcelTitle(firstData, inputTitle, out errTitle);
                if (!hasErr)
                {
                    //throw new Exception($@"上傳的文件內容有誤,必須包含{errTitle}列!");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162102", new string[] { errTitle }));
                }

                #region 写入数据库

                var SenderID = string.Empty;


                SFCDB.BeginTrain();
                for (int i = 0; i < array.Count; i++)
                {
                    var f_plant = string.Empty;
                    var Messageid = string.Empty;
                    SenderID = array[i]["SenderID"].ToString().ToUpper().Trim();
                    var PN = array[i]["PN"].ToString().ToUpper().Trim();

                    if (SenderID.Length == 0)
                    {
                        continue;//空行不傳
                    }


                    sql = $@" SELECT * FROM R_I140 where  TRANID='{iTRANID}' and PN='{PN}' ";
                    DataTable dttemp = SFCDB.ExecSelect(sql).Tables[0];
                    if (dttemp.Rows.Count == 0)//取不到就去數據庫找最新的一個記錄
                    {
                        sql = $@"   SELECT f_plant,messageid  FROM  r_i140 
                                 WHERE  VENDORCODE = '{SenderID}'
                                    AND ROWNUM = 1 and createtime >sysdate -30     ";
                        dttemp = SFCDB.ExecSelect(sql).Tables[0];
                        f_plant = dttemp.Rows[0]["F_PLANT"].ToString();
                        Messageid = dttemp.Rows[0]["Messageid"].ToString();
                    }
                    else
                    {
                        f_plant = dttemp.Rows[0]["F_PLANT"].ToString();
                        Messageid = dttemp.Rows[0]["Messageid"].ToString();
                    }



                    Newtonsoft.Json.Linq.JObject a = Newtonsoft.Json.Linq.JObject.Parse(array[i].ToString());
                    List<string> d = GetInsertCol(a, inputTitle);
                    foreach (string col in d)
                    {
                        string count = array[i][col].ToString().ToUpper().Trim();
                        sql = $@"  INSERT INTO r_i285 (SENDERID,PN,STARTDATE,QUANTITY,CREATETIME,
                              TRANID, F_PLANT ,FILENAME,MESSAGEID,CREATIONDATE,
                              ENDDATE,F_LASTEDITDT
                                  ) 
                                   VALUES ('{SenderID}','{PN}',to_date('{col}','YYYY-MM-DD') ,'{count}',sysdate,
                               '{iTRANID}','{f_plant}','{makeid}','{Messageid}',sysdate
                             ,to_date('{col} {"23:59:59"}','YYYY-MM-DD HH24:MI:SS'),sysdate) ";
                        SFCDB.ExecSQL(sql);
                    }
                }

                sql = $@" UPDATE R_I140_MAIN_D SET COMPLETE='1',COMMITID='{makeid}' WHERE TRANID='{iTRANID}' AND COMMITDAY='{iDAY}'  ";
                SFCDB.ExecSQL(sql);

                SFCDB.CommitTrain();

                if (result == "")
                {
                    result = "Upload Successful ! ";
                }
                else
                {
                    result = "Upload Fail:" + result;
                }
                #endregion 

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = result;
                //StationReturn.MessageCode = "MES00000002";
                //StationReturn.Data = "";               
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add($@"{sql}{ee.Message}");
                return;
            }
        }

        public void UploadR_i285ExcelByDATE(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            string sql = "";
            string result = "";
            string makeid = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                //定義上傳Excel的列名
                // List<string> inputTitle = new List<string> { "TRANID", "F_Plant", "FileName", "Messageid", "CreationDateTime", "SenderID", "F_LASTEDITDT", "PN" };

                List<string> inputTitle = new List<string> { "SenderID", "PN" };
                string errTitle = "";
                // string TRANID = "", F_Plant = "", FileName = "", Messageid = "", CreationDateTime = "", SenderID = "", F_LASTEDITDT = "", PN = "";

                string data = Data["ExcelData"].ToString();
                string iTRANID = Data["TRANID"].ToString();
                string iDAY = Data["DAY"].ToString();


                //2021.1.4
                string STR_DATE = Data["STR_DATE"].ToString();
                string VENDORCODE = Data["VENDORCODE"].ToString();
                string pre = string.Empty;
                if (VENDORCODE == "0016000220")
                {
                    pre = "FVN";
                }
                else if (VENDORCODE == "0016000219")
                {
                    pre = "FJZ";
                }

                iDAY = GetDay(iDAY);
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool hasErr = CheckInputExcelTitle(firstData, inputTitle, out errTitle);
                if (!hasErr)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162102", new string[] { errTitle }));
                }

                #region 写入数据库
                SFCDB.BeginTrain();
                var SenderID = string.Empty;

                #region 插入 R_I140_MAIN_D  2021.1.4
                for (int i = 0; i < array.Count; i++)
                {
                    SenderID = array[i]["SenderID"].ToString().ToUpper().Trim();
                    if (SenderID.Length == 0)
                    {
                        continue;//空行不傳
                    }
                    else
                    {
                        sql = $@" SELECT * FROM R_I140_MAIN_D where  COMMITDATE ='{STR_DATE}' and VENDORCODE='{SenderID}' ";
                        DataTable dttemp = SFCDB.ExecSelect(sql).Tables[0];
                        if (dttemp.Rows.Count == 0)//取不到就去數據庫找最新的一個記錄
                        {

                            var ID = MesDbBase.GetNewID<R_I140_MAIN_D>(SFCDB.ORM, Customer.JUNIPER.ExtValue());
                            SFCDB.ORM.Insertable(new R_I140_MAIN_D()
                            {
                                ID = ID,
                                COMMITDAY = iDAY,
                                COMPLETE = "0",
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now,
                                EDITEMP = "SYSTEM",
                                VENDORCODE = SenderID,
                                COMMITDATE = STR_DATE
                            }).ExecuteCommand();


                            //又改用另一種新寫法統一
                            //T_R_I140_MAIN_D T_R_I140_MAIN_D = new T_R_I140_MAIN_D(SFCDB, DB_TYPE_ENUM.Oracle);
                            //R_I140_MAIN_D R_I140_MAIN_D = new R_I140_MAIN_D();
                            //string ID = T_R_I140_MAIN_D.GetNewID(BU, SFCDB);
                            //R_I140_MAIN_D.ID = ID;
                            ////R_I140_MAIN_D.TRANID = ID;
                            //R_I140_MAIN_D.COMMITDAY = iDAY;
                            //R_I140_MAIN_D.COMPLETE = "0";
                            ////R_I140_MAIN_D.COMMITID = ID;
                            //R_I140_MAIN_D.CREATETIME = T_R_I140_MAIN_D.GetDBDateTime(SFCDB);
                            //R_I140_MAIN_D.EDITTIME = T_R_I140_MAIN_D.GetDBDateTime(SFCDB);
                            //R_I140_MAIN_D.EDITEMP = "SYSTEM";
                            //R_I140_MAIN_D.VENDORCODE = SenderID;
                            //R_I140_MAIN_D.COMMITDATE = STR_DATE;
                            //T_R_I140_MAIN_D.InSertRow(R_I140_MAIN_D, SFCDB);


                            //改用另一種寫法插入2021.01.08
                            //sql = $@"
                            //  INSERT INTO R_I140_MAIN_D
                            //              (ID,
                            //               TRANID,
                            //               COMMITDAY,
                            //               COMPLETE,
                            //               COMMITID,
                            //               CREATETIME,
                            //               EDITTIME,
                            //               EDITEMP,VENDORCODE,COMMITDATE)
                            //            VALUES
                            //              ('JUNIPER'||TO_CHAR(SYSTIMESTAMP,'YYYYMMDDHH24MISSFF'),
                            //               '',
                            //               '{iDAY}',
                            //               '0',
                            //               NULL,
                            //               SYSDATE,
                            //               SYSDATE,
                            //               'SYSTEM','{SenderID}','{STR_DATE}')";
                            //SFCDB.ExecSQL(sql);
                        }
                        break;
                    }
                }
                #endregion



                for (int i = 0; i < array.Count; i++)
                {
                    var f_plant = string.Empty;
                    var Messageid = string.Empty;
                    SenderID = array[i]["SenderID"].ToString().ToUpper().Trim();
                    var PN = array[i]["PN"].ToString().ToUpper().Trim();

                    if (SenderID.Length == 0)
                    {
                        continue;//空行不傳
                    }


                    sql = $@" SELECT * FROM R_I140 where  TRANID='{iTRANID}' and PN='{PN}' ";
                    DataTable dttemp = SFCDB.ExecSelect(sql).Tables[0];
                    if (dttemp.Rows.Count == 0)//取不到就去數據庫找最新的一個記錄
                    {
                        sql = $@"   SELECT f_plant,messageid  FROM  r_i140 
                                 WHERE  VENDORCODE = '{SenderID}'
                                    AND ROWNUM = 1 and createtime >sysdate -30     ";
                        dttemp = SFCDB.ExecSelect(sql).Tables[0];
                        f_plant = dttemp.Rows[0]["F_PLANT"].ToString();
                        Messageid = dttemp.Rows[0]["Messageid"].ToString();
                    }
                    else
                    {
                        f_plant = dttemp.Rows[0]["F_PLANT"].ToString();
                        Messageid = dttemp.Rows[0]["Messageid"].ToString();
                    }

                    var ci285 = SFCDB.ORM.Queryable<R_I140_MAIN_D>().Where(t => t.COMMITDATE == STR_DATE && t.VENDORCODE == SenderID).ToList().FirstOrDefault();
                    if (ci285 != null && !string.IsNullOrEmpty(ci285.COMMITID))
                        SFCDB.ORM.Deleteable<R_I285>().Where(t => t.FILENAME == ci285.COMMITID).ExecuteCommand();

                    Newtonsoft.Json.Linq.JObject a = Newtonsoft.Json.Linq.JObject.Parse(array[i].ToString());
                    List<string> d = GetInsertCol(a, inputTitle);
                    foreach (string col in d)
                    {
                        string count = array[i][col].ToString().ToUpper().Trim();

                        var ID = MesDbBase.GetNewID<R_I285>(SFCDB.ORM, Customer.JUNIPER.ExtValue());
                        SFCDB.ORM.Insertable(new R_I285()
                        {
                            ID = ID,
                            SENDERID = SenderID,
                            PN = PN,
                            STARTDATE = Convert.ToDateTime(col),
                            QUANTITY = count,
                            CREATETIME = DateTime.Now,
                            TRANID = pre + makeid,//2021.01.15 tranid 加前綴FVN\FJZ
                            F_PLANT = f_plant,
                            FILENAME = makeid,
                            MESSAGEID = Messageid,
                            CREATIONDATE = DateTime.Now,
                            ENDDATE = Convert.ToDateTime(col + " 23:59:59"),
                            F_LASTEDITDT = DateTime.Now
                        }).ExecuteCommand();


                        //又改用另一種新寫法統一
                        //T_R_I285 T_R_I285 = new T_R_I285(SFCDB, DB_TYPE_ENUM.Oracle);
                        //R_I285 R_I285 = new R_I285();
                        //string ID = T_R_I285.GetNewID(BU, SFCDB);
                        //R_I285.ID = ID;
                        //R_I285.SENDERID = SenderID;
                        //R_I285.PN = PN;
                        //R_I285.STARTDATE = Convert.ToDateTime(col);
                        //R_I285.QUANTITY = count;
                        //R_I285.CREATETIME = T_R_I285.GetDBDateTime(SFCDB);
                        //R_I285.TRANID = makeid;
                        //R_I285.F_PLANT = f_plant;
                        //R_I285.FILENAME = makeid;
                        //R_I285.MESSAGEID = Messageid;
                        //R_I285.CREATIONDATE = T_R_I285.GetDBDateTime(SFCDB);
                        //R_I285.ENDDATE = Convert.ToDateTime(col + " 23:59:59");
                        //R_I285.F_LASTEDITDT = T_R_I285.GetDBDateTime(SFCDB);
                        //T_R_I285.InSertRow(R_I285, SFCDB);

                        //改用另一種寫法插入2021.01.08
                        //sql = $@"  INSERT INTO r_i285 (SENDERID,PN,STARTDATE,QUANTITY,CREATETIME,
                        //      TRANID, F_PLANT ,FILENAME,MESSAGEID,CREATIONDATE,
                        //      ENDDATE,F_LASTEDITDT
                        //          ) 
                        //           VALUES ('{SenderID}','{PN}',to_date('{col}','YYYY-MM-DD') ,'{count}',sysdate,
                        //       '{makeid}','{f_plant}','{makeid}','{Messageid}',sysdate
                        //     ,to_date('{col} {"23:59:59"}','YYYY-MM-DD HH24:MI:SS'),sysdate) ";
                        //SFCDB.ExecSQL(sql);
                    }
                }

                sql = $@" UPDATE R_I140_MAIN_D SET COMPLETE='1',COMMITID='{makeid}' WHERE  COMMITDATE ='{STR_DATE}' and VENDORCODE='{SenderID}' ";
                SFCDB.ExecSQL(sql);

                SFCDB.CommitTrain();

                if (result == "")
                {
                    result = "upload successful ! ";
                }
                else
                {
                    result = "upload fail:" + result;
                }
                #endregion 

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = result;
                //StationReturn.MessageCode = "MES00000002";
                //StationReturn.Data = "";               
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add($@"{sql}{ee.Message}");
                return;
            }
        }

        public void Uploadi285(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string sql = "";
            string result = "";
            string makeid = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                List<string> inputTitle = new List<string> { "SenderID", "PN" };
                string errTitle = "";
                string data = Data["ExcelData"].ToString();
                string iTRANID = Data["TRANID"].ToString();
                string iDAY = Data["DAY"].ToString();


                //2021.1.4
                string STR_DATE = Data["STR_DATE"].ToString();
                string VENDORCODE = Data["VENDORCODE"].ToString();
                string pre = string.Empty;
                if (VENDORCODE == "0016000220")
                {
                    pre = "FVN";
                }
                else if (VENDORCODE == "0016000219")
                {
                    pre = "FJZ";
                }

                //iDAY = GetDay(iDAY);
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool hasErr = CheckInputExcelTitle(firstData, inputTitle, out errTitle);
                if (!hasErr)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162102", new string[] { errTitle }));
                }

                #region 写入数据库
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    var SenderID = string.Empty;
                    for (int i = 0; i < array.Count; i++)
                    {
                        var f_plant = string.Empty;
                        var Messageid = string.Empty;
                        SenderID = array[i]["SenderID"].ToString().ToUpper().Trim();
                        var PN = array[i]["PN"].ToString().ToUpper().Trim();

                        if (SenderID.Length == 0)
                        {
                            continue;//空行不傳
                        }
                        sql = $@" SELECT * FROM R_I140 where  TRANID='{iTRANID}' and PN='{PN}' ";
                        DataTable dttemp = SFCDB.ExecSelect(sql).Tables[0];
                        if (dttemp.Rows.Count == 0)//取不到就去數據庫找最新的一個記錄
                        {
                            sql = $@"   SELECT f_plant,messageid  FROM  r_i140 
                                 WHERE  VENDORCODE = '{SenderID}'
                                    AND ROWNUM = 1 and createtime >sysdate -30     ";
                            dttemp = SFCDB.ExecSelect(sql).Tables[0];
                            f_plant = dttemp.Rows[0]["F_PLANT"].ToString();
                            Messageid = dttemp.Rows[0]["Messageid"].ToString();
                        }
                        else
                        {
                            f_plant = dttemp.Rows[0]["F_PLANT"].ToString();
                            Messageid = dttemp.Rows[0]["Messageid"].ToString();
                        }

                        Newtonsoft.Json.Linq.JObject a = Newtonsoft.Json.Linq.JObject.Parse(array[i].ToString());
                        List<string> d = GetInsertCol(a, inputTitle);
                        var targetobj = new List<R_I285_SOURCE>();
                        foreach (string col in d)
                        {
                            string count = array[i][col].ToString().ToUpper().Trim();
                            targetobj.Add(new R_I285_SOURCE()
                            {
                                ID = MesDbBase.GetNewID<R_I285_SOURCE>(SFCDB.ORM, Customer.JUNIPER.ExtValue()),
                                SENDERID = SenderID,
                                PN = PN,
                                STARTDATE = Convert.ToDateTime(col),
                                QUANTITY = count,
                                CREATETIME = DateTime.Now,
                                TRANID = pre + makeid,//2021.01.15 tranid 加前綴FVN\FJZ
                                F_PLANT = f_plant,
                                FILENAME = makeid,
                                MESSAGEID = Messageid,
                                CREATIONDATE = DateTime.Now,
                                ENDDATE = Convert.ToDateTime(col + " 23:59:59"),
                                F_LASTEDITDT = DateTime.Now
                            });
                        }
                        #region check 285
                        var atp = SFCDB.ORM.Queryable<O_SKU_ATP>().ToList();
                        var checkres = new List<string>();
                        targetobj.ForEach(t =>
                        {
                            if (!atp.Any(tp => t.PN == tp.PARTNO)) checkres.Add(t.PN);
                        });
                        if (checkres.Count > 0)
                            throw new Exception($@"PN:{checkres[0]} is not set atp ,pls check!");
                        #endregion

                        SFCDB.ORM.Insertable(targetobj).ExecuteCommand();
                    }

                    SFCDB.ORM.Updateable<R_I285_MAIN>().SetColumns(t => new R_I285_MAIN() { VALID = MesBool.No.ExtValue() }).Where(t => t.VALID != MesBool.No.ExtValue()).ExecuteCommand();
                    SFCDB.ORM.Insertable(new R_I285_MAIN()
                    {
                        ID = MesDbBase.GetNewID<R_I285_MAIN>(SFCDB.ORM, this.BU),
                        FILENAME = makeid,
                        VALID = MesBool.Yes.ExtValue(),
                        CREATETIME = DateTime.Now,
                        CREATEBY = this.LoginUser.EMP_NO
                    }).ExecuteCommand();
                });

                if (res.IsSuccess)
                    result = "upload successful ! ";
                else
                    throw res.ErrorException;
                #endregion 

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = result;
                //StationReturn.MessageCode = "MES00000002";
                //StationReturn.Data = "";            
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
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetI285Set(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var res = sfcdb.ORM.Queryable<R_I285_MAIN>().OrderBy(t => t.CREATETIME, OrderByType.Desc).ToList();
                StationReturn.Data = res;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
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

        public void DeleteI285ByFilename(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string filename = Data["I285FILENAME"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var ci285 = oleDB.ORM.Queryable<R_I140_MAIN_D>().Where(t => t.COMMITID == filename).ToList().FirstOrDefault();
                var dbtime = MesDbBase.GetOraDbTime(oleDB.ORM).ToString("yyyyMMdd");
                if (ci285 != null && !string.IsNullOrEmpty(ci285.COMMITID) && dbtime != ci285.COMMITDATE)
                    throw new Exception("You can only modify the data for the day！");
                if (ci285 != null && !string.IsNullOrEmpty(ci285.COMMITID))
                {
                    var res = oleDB.ORM.Ado.UseTran(() =>
                    {
                        oleDB.ORM.Deleteable<R_I285>().Where(t => t.FILENAME == ci285.COMMITID).ExecuteCommand();
                        oleDB.ORM.Updateable<R_I140_MAIN_D>().SetColumns(t => new R_I140_MAIN_D() { COMPLETE = MesBool.No.ExtValue(), COMMITID = "" }).Where(t => t.ID == ci285.ID).ExecuteCommand();
                    });
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        /// <summary>
        /// 獲取最新140的TRANID,和創建時間
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetSkuAtp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<O_SKU_ATP>().OrderBy(t => t.FOXPN).ToList();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void NewSkuAtp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string CUSTPN = Data["CUSTPN"].ToString().Trim();
            string FOXPN = Data["FOXPN"].ToString().Trim().ToUpper();
            string ATPTYPE = Data["ATPTYPE"].ToString();
            string PARTNO = Data["PARTNO"].ToString();

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                if (sfcdb.ORM.Queryable<O_SKU_ATP>().Any(t => t.FOXPN == CUSTPN && t.FOXPN == FOXPN && t.ATPTYPE == ATPTYPE && t.PARTNO == PARTNO))
                    throw new Exception("FOXPN is Allready exists!");

                var res = sfcdb.ORM.Insertable(new O_SKU_ATP()
                {
                    ID = MesDbBase.GetNewID<O_SKU_ATP>(sfcdb.ORM, this.BU),
                    CUSTPN = CUSTPN,
                    FOXPN = FOXPN,
                    ATPTYPE = ATPTYPE,
                    PARTNO = PARTNO,
                    CREATETIME = DateTime.Now,
                    CREATEBY = this.LoginUser.EMP_NO
                }).ExecuteCommand();

                StationReturn.Message = "Add Success";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void EditSkuAtp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            var obj = Data["obj"].ToObject<O_SKU_ATP>();
            try
            {
                obj.CREATETIME = DateTime.Now;
                obj.CREATEBY = this.LoginUser.EMP_NO;
                sfcdb = this.DBPools["SFCDB"].Borrow();
                if (sfcdb.ORM.Queryable<O_SKU_ATP>().Any(t => t.ID == obj.ID))
                    sfcdb.ORM.Updateable(obj).ExecuteCommand();

                StationReturn.Message = "Add Success";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DelSkuAtp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var ID = Data["ID"].ToString();
                sfcdb.ORM.Deleteable<O_SKU_ATP>().Where(t => t.ID == ID).ExecuteCommand();

                StationReturn.Message = "Add Success";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void UploadSkuAtp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                {
                    throw new Exception("Please Input Excel Data");
                }
                if (Data["FileName"] == null)
                {
                    throw new Exception("Please Input FileName");
                }
                string B64 = Data["ExcelData"].ToString();
                string filename = Data["FileName"].ToString();
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);

                string filePath = System.IO.Directory.GetCurrentDirectory() + "\\UploadFile\\";
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + filename;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                F.Write(data, 0, data.Length);
                F.Flush();
                F.Close();
                DataTable dt = MESPubLab.Common.ExcelHelp.DBExcelToDataTableEpplus(filePath);
                if (dt.Rows.Count == 0)
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));
                //throw new Exception($@"上傳的Excel中沒有內容!");                

                string result = "";

                #region 写入数据库
                var targetlist = ObjectDataHelper.FromTable<O_SKU_ATP>(dt);
                foreach (var item in targetlist)
                {
                    if (string.IsNullOrEmpty(item.PARTNO))
                        throw new Exception($@"CustPn: {item.CUSTPN}'s  PartNo cannot be empty,pls check!");
                    item.ID = MesDbBase.GetNewID<O_SKU_ATP>(SFCDB.ORM, this.BU);
                    item.CREATETIME = DateTime.Now;
                    item.CREATEBY = this.LoginUser.EMP_NO;
                    //if (!SFCDB.ORM.Queryable<O_SKU_ATP>().Any(t => t.FOXPN == item.FOXPN && t.CUSTPN == item.CUSTPN && t.ATPTYPE == item.ATPTYPE && t.PARTNO == item.PARTNO))
                    //{
                    //    item.ID = MesDbBase.GetNewID<O_SKU_ATP>(SFCDB.ORM, this.BU);
                    //    item.CREATETIME = DateTime.Now;
                    //    item.CREATEBY = this.LoginUser.EMP_NO;
                    //}
                }
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    SFCDB.ORM.Deleteable<O_SKU_ATP>().ExecuteCommand();
                    SFCDB.ORM.Insertable(targetlist).ExecuteCommand();
                });

                if (res.IsSuccess)
                    result = "All Upload OK ! ";
                else
                    throw res.ErrorException;
                #endregion

                StationReturn.Message = result;
                StationReturn.Status = StationReturnStatusValue.Pass;

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
        /// <summary>
        /// 獲取日期行  插入(找出非標題列,不是標題列,要插入數據庫)
        /// </summary>
        /// <param name="inputExcelColumn"></param>
        /// <param name="listTitle"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private List<string> GetInsertCol(Newtonsoft.Json.Linq.JObject inputExcelColumn, List<string> listTitle)
        {
            List<string> data = new List<string>();
            foreach (var datas in inputExcelColumn.Properties())
            {
                string findit = "N";
                foreach (string t in listTitle)
                {
                    findit = "N";
                    if (datas.Name == t)
                    {
                        findit = "Y";
                        break;
                    }
                }
                if (findit == "N")
                {
                    data.Add(datas.Name);
                }
            }
            return data;
        }

        /// <summary>
        /// 檢查上傳的Excle是否包含模板中的列
        /// </summary>
        /// <param name="inputExcelColumn"></param>
        /// <param name="listTitle"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private bool CheckInputExcelTitle(Newtonsoft.Json.Linq.JObject inputExcelColumn, List<string> listTitle, out string title)
        {
            bool bColumnExists = true;
            string out_title = "";
            foreach (string t in listTitle)
            {
                bColumnExists = inputExcelColumn.Properties().Any(p => p.Name == t);
                if (!bColumnExists)
                {
                    out_title = t;
                    break;
                }
            }
            title = out_title;
            return bColumnExists;
        }


        public void GetComparisonDiffRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();

                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonDiffRes(oleDB, I140Akey, I140Bkey, ref dt);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { maindata = dt, assist = minusRes, delpn = Bpns };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void ComparisonDiffRes(OleExec SFCDB, string I140Akey, string I140Bkey, ref DataTable dt)
        {
            DataTable outTable = new DataTable();
            List<C140Res> outMinusRes = new List<C140Res>();
            List<C140Res> outBpns = new List<C140Res>();

            var Adate = SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Akey).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATETIME, OrderByType.Asc).ToList();
            var Bdate = SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATETIME, OrderByType.Asc).ToList();
            var GroupData = SFCDB.ORM.Queryable<R_SKU_JNP_G>().ToList();
            var maxwk = Convert.ToDateTime(Bdate.Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(Bdate.Min(t => t.STARTDATETIME));
            var pnlist = Bdate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("TRANID");
            outTable.Columns.Add("PN");
            outTable.Columns.Add("GROUP");
            while (minwk <= maxwk)
            {
                outTable.Columns.Add(minwk.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue()));
                minwk = minwk.AddDays(7);
            }
            foreach (var pn in pnlist)
            {
                var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
                var pnBdata = Adate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["TRANID"] = pndata.FirstOrDefault().TRANID;
                dr["PN"] = pn;
                dr["GROUP"] = GroupData.FindAll(t => t.JUNIPER == pn).FirstOrDefault()?.G_CODE;
                foreach (var item in pndata)
                {
                    var itemA = pnBdata.FindAll(t => t.STARTDATETIME == item.STARTDATETIME).ToList().FirstOrDefault();
                    dr[Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())] = itemA != null ? (int.Parse(item.QUANTITY) - int.Parse(itemA.QUANTITY)).ToString() : item.QUANTITY;
                }
                outTable.Rows.Add(dr);
            }
            outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
            foreach (var itempn in outBpns)
            {
                var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                var dr = outTable.NewRow();
                dr["TRANID"] = pndata.FirstOrDefault().TRANID;
                dr["PN"] = itempn.PN;
                dr["GROUP"] = GroupData.FindAll(t => t.JUNIPER == itempn.PN).FirstOrDefault()?.G_CODE;
                foreach (var item in pndata)
                    if (outTable.Columns.Contains(Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())))
                        dr[Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())] = item.QUANTITY == "0" ? "0" : $@"-{item.QUANTITY}";
                outTable.Rows.Add(dr);
            }

            dt = outTable;
        }
        public void GetComparisonRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();

                #region 提取到ComparisonRes方法中
                //var Adate = oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Akey).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATETIME, OrderByType.Asc).ToList();
                //var Bdate = oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATETIME, OrderByType.Asc).ToList();
                //var maxwk = Convert.ToDateTime(Bdate.Max(t => t.STARTDATETIME));
                //var minwk = Convert.ToDateTime(Bdate.Min(t => t.STARTDATETIME));
                //var pnlist = Bdate.Select(t => t.PN).Distinct();
                //DataTable dt = new DataTable();
                //dt.Columns.Add("TRANID");
                //dt.Columns.Add("PN");
                //while (minwk <= maxwk)
                //{
                //    dt.Columns.Add(minwk.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue()));
                //    minwk = minwk.AddDays(7);
                //}
                //foreach (var pn in pnlist)
                //{
                //    var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
                //    var dr = dt.NewRow();
                //    dr["TRANID"] = pndata.FirstOrDefault().TRANID;
                //    dr["PN"] = pn;
                //    foreach (var item in pndata)
                //        dr[Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())] = item.QUANTITY;
                //    dt.Rows.Add(dr);
                //}
                //var Bpns = oleDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
                //foreach (var itempn in Bpns)
                //{
                //    var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                //    var dr = dt.NewRow();
                //    dr["TRANID"] = pndata.FirstOrDefault().TRANID;
                //    dr["PN"] = itempn.PN;
                //    foreach (var item in pndata)
                //        if(dt.Columns.Contains(Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())))
                //            dr[Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())] = item.QUANTITY;
                //    dt.Rows.Add(dr);
                //}

                //var minusRes = new List<C140Res>();
                //oleDB.ORM.Ado.SqlQuery<C140Res>($@"select * from ( select PN,to_char( startdatetime,'yyyy-MM-dd') as SDate,quantity as CQty from R_I140 where tranid='{I140Bkey}' minus
                //                                            select PN,to_char( startdatetime,'yyyy-MM-dd') as SDate,quantity as CQty from R_I140 where tranid='{I140Akey}' ) order by pn,sdate asc").ForEach(t =>
                //{
                //    var last = Adate.FindAll(b => b.PN == t.PN && Convert.ToDateTime(b.STARTDATETIME) == Convert.ToDateTime(t.SDate)).ToList().FirstOrDefault();
                //    minusRes.Add(new C140Res()
                //    {
                //        PN = t.PN,
                //        SDate = t.SDate,
                //        CQty = t.CQty,
                //        LQty = last != null ? last.QUANTITY : "0",
                //        Flag = last != null ? "C":"A"
                //    });
                //});
                //oleDB.ORM.Ado.SqlQuery<C140Res>($@"select *from ( select PN,to_char( startdatetime,'yyyy-MM-dd') as SDate,quantity as CQty from R_I140 where tranid='{I140Akey}' minus
                //                                             select PN,to_char( startdatetime,'yyyy-MM-dd') as SDate,quantity as CQty from R_I140 where tranid='{I140Bkey}' ) order by pn,sdate asc").ForEach(t =>
                //{
                //    var Current = Bdate.FindAll(a => a.PN == t.PN && Convert.ToDateTime(a.STARTDATETIME) == Convert.ToDateTime(t.SDate)).ToList().FirstOrDefault();                    
                //    if (Current == null&& !minusRes.Any(m => m.PN == t.PN && m.SDate == t.SDate))
                //        minusRes.Add(new C140Res()
                //        {
                //            PN = t.PN,
                //            SDate = t.SDate,
                //            CQty = "0",
                //            LQty = t.CQty,
                //            Flag = "D" 
                //        });
                //});
                #endregion
                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonRes(oleDB, I140Akey, I140Bkey, ref dt, ref minusRes, ref Bpns);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { maindata = dt, assist = minusRes, delpn = Bpns };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void ComparisonRes(OleExec SFCDB, string I140Akey, string I140Bkey, ref DataTable dt, ref List<C140Res> minusRes, ref List<C140Res> Bpns)
        {
            DataTable outTable = new DataTable();
            List<C140Res> outMinusRes = new List<C140Res>();
            List<C140Res> outBpns = new List<C140Res>();

            var Adate = SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Akey).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATETIME, OrderByType.Asc).ToList();
            var Bdate = SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATETIME, OrderByType.Asc).ToList();
            var maxwk = Convert.ToDateTime(Bdate.Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(Bdate.Min(t => t.STARTDATETIME));
            var pnlist = Bdate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("TRANID");
            outTable.Columns.Add("PN");
            while (minwk <= maxwk)
            {
                outTable.Columns.Add(minwk.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue()));
                minwk = minwk.AddDays(7);
            }
            foreach (var pn in pnlist)
            {
                var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["TRANID"] = pndata.FirstOrDefault().TRANID;
                dr["PN"] = pn;
                foreach (var item in pndata)
                    dr[Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())] = item.QUANTITY;
                outTable.Rows.Add(dr);
            }
            outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
            foreach (var itempn in outBpns)
            {
                var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                var dr = outTable.NewRow();
                dr["TRANID"] = pndata.FirstOrDefault().TRANID;
                dr["PN"] = itempn.PN;
                foreach (var item in pndata)
                    if (outTable.Columns.Contains(Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())))
                        dr[Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())] = item.QUANTITY;
                outTable.Rows.Add(dr);
            }

            outMinusRes = new List<C140Res>();
            SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select * from ( select PN,to_char( startdatetime,'yyyy-MM-dd') as SDate,quantity as CQty from R_I140 where tranid='{I140Bkey}' minus
                                                            select PN,to_char( startdatetime,'yyyy-MM-dd') as SDate,quantity as CQty from R_I140 where tranid='{I140Akey}' ) order by pn,sdate asc").ForEach(t =>
            {
                var last = Adate.FindAll(b => b.PN == t.PN && Convert.ToDateTime(b.STARTDATETIME) == Convert.ToDateTime(t.SDate)).ToList().FirstOrDefault();
                outMinusRes.Add(new C140Res()
                {
                    PN = t.PN,
                    SDate = t.SDate,
                    CQty = t.CQty,
                    LQty = last != null ? last.QUANTITY : "0",
                    Flag = last != null ? "C" : "A"
                });
            });
            SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select *from ( select PN,to_char( startdatetime,'yyyy-MM-dd') as SDate,quantity as CQty from R_I140 where tranid='{I140Akey}' minus
                                                             select PN,to_char( startdatetime,'yyyy-MM-dd') as SDate,quantity as CQty from R_I140 where tranid='{I140Bkey}' ) order by pn,sdate asc").ForEach(t =>
            {
                var Current = Bdate.FindAll(a => a.PN == t.PN && Convert.ToDateTime(a.STARTDATETIME) == Convert.ToDateTime(t.SDate)).ToList().FirstOrDefault();
                if (Current == null && !outMinusRes.Any(m => m.PN == t.PN && m.SDate == t.SDate))
                    outMinusRes.Add(new C140Res()
                    {
                        PN = t.PN,
                        SDate = t.SDate,
                        CQty = "0",
                        LQty = t.CQty,
                        Flag = "D"
                    });
            });

            dt = outTable;
            minusRes = outMinusRes;
            Bpns = outBpns;
        }
        public void GetComparisonQtyResDiff(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
                #region 提取到ComparisonQtyRes方法中
                #endregion
                DataTable dt = new DataTable();
                ComparisonQtyDiffRes(oleDB, I140Akey, I140Bkey, ref dt);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { maindata = dt };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void ComparisonQtyDiffRes(OleExec SFCDB, string I140Akey, string I140Bkey, ref DataTable dt)
        {
            DataTable outTable = new DataTable();
            List<C140Res> outMinusRes = new List<C140Res>();
            List<C140Res> outBpns = new List<C140Res>();

            var dayAkey = Convert.ToDateTime($@"{I140Akey.Substring(0, 4)}-{I140Akey.Substring(4, 2)}-{I140Akey.Substring(6, 2)}");
            var dayBkey = Convert.ToDateTime($@"{I140Bkey.Substring(0, 4)}-{I140Bkey.Substring(4, 2)}-{I140Bkey.Substring(6, 2)}");
            //var Adate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"  SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate ORDER BY PN,SDate");
            //var Bdate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"  SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate ORDER BY PN,SDate");
            var Adate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"   SELECT pn,new_order_quantity as FGOH,
                        SHIPMENT,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn, new_order_quantity,
                        SHIPMENT,d.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 c 
                        left join R_SKU_JNP_G d
                                             on c.pn = d.juniper
                                           left join (select a.skuno, count(distinct a.sn) as SHIPMENT
                                                       from r_ship_detail a, r_sn b
                                                      where a.shipdate between
                                                            to_date('{GetCurrentQuarterFirstDay(dayAkey)}',
                                                                    'yyyy-mm-dd HH24:MI:SS') and
                                                            to_date('{GetCurrentTuesday(dayAkey).AddHours(9)}',
                                                                    'yyyy-mm-dd HH24:MI:SS')
                                                        and a.sn = b.sn
                                                      group by a.skuno) bb
                                             on c.pn = bb.skuno
                                           left join (select b.item_name,sum(new_order_quantity) new_order_quantity
                                                       from r_sap_file a, r_sap_file_i605 b
                                                      where a.id = b.file_id
                                                        and a.file_name like
                                                            '{GetCurrentTuesday(dayAkey).ToString("yyyyMMdd")}%'  group by b.item_name) aa
                                             on c.pn = aa.item_name where tranid='{I140Akey}') GROUP BY pn,g_code,new_order_quantity,
                                                SHIPMENT, SDate ORDER BY PN,SDate");

            var Bdate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"   SELECT pn,new_order_quantity as FGOH,
                        SHIPMENT,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn, new_order_quantity,
                        SHIPMENT,d.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 c 
                        left join R_SKU_JNP_G d
                                             on c.pn = d.juniper
                                           left join (select a.skuno, count(distinct a.sn) as SHIPMENT
                                                       from r_ship_detail a, r_sn b
                                                      where a.shipdate between
                                                            to_date('{GetCurrentQuarterFirstDay(dayBkey)}',
                                                                    'yyyy-mm-dd HH24:MI:SS') and
                                                            to_date('{GetCurrentTuesday(dayBkey).AddHours(9)}',
                                                                    'yyyy-mm-dd HH24:MI:SS')
                                                        and a.sn = b.sn
                                                      group by a.skuno) bb
                                             on c.pn = bb.skuno
                                           left join (select b.item_name,sum(new_order_quantity) new_order_quantity
                                                       from r_sap_file a, r_sap_file_i605 b
                                                      where a.id = b.file_id
                                                        and a.file_name like
                                                            '{GetCurrentTuesday(dayBkey).ToString("yyyyMMdd")}%'  group by b.item_name) aa
                                             on c.pn = aa.item_name where tranid='{I140Bkey}') GROUP BY pn,g_code,new_order_quantity,
                                                SHIPMENT, SDate ORDER BY PN,SDate");


            var maxwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            var pnlist = Bdate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            outTable.Columns.Add("GROUP");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }
            foreach (var pn in pnlist)
            {
                try
                {
                    var pnBdata = Bdate.FindAll(t => t.PN == pn).ToList();
                    var dr = outTable.NewRow();
                    dr["GROUP"] = pnBdata.FirstOrDefault().G_code;
                    dr["PN"] = pn;
                    var pnAdata = Adate.FindAll(t => t.PN == pn).ToList();
                    var currentQuarter = pnBdata.FirstOrDefault().SDate;
                    dr[currentQuarter] = GetQtyWithFirstQuarterByPn(pnBdata) - GetQtyWithFirstQuarterByPn(pnAdata);

                    foreach (var item in pnBdata)
                        if (item.SDate != currentQuarter)
                        {
                            var itemA = Adate.FindAll(t => t.PN == pn && t.SDate == item.SDate).ToList().FirstOrDefault();
                            dr[item.SDate] = itemA != null ? (int.Parse(item.CQty) - int.Parse(itemA.CQty)).ToString() : item.CQty;
                        }
                    outTable.Rows.Add(dr);

                }
                catch (Exception e)
                {

                }
            }
            outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
            foreach (var itempn in outBpns)
            {
                var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pndata.FirstOrDefault().G_code;
                dr["PN"] = itempn.PN;
                foreach (var item in pndata)
                    if (outTable.Columns.Contains(item.SDate))
                        dr[item.SDate] = item.CQty == "0" ? "0" : $@"-{item.CQty}";
                outTable.Rows.Add(dr);
            }

            dt = outTable;
        }

        long GetQtyWithFirstQuarterByPn(List<C140Res> pndata)
        {
            if (pndata.Count == 0)
                return 0;
            var currentQuarter = pndata.FirstOrDefault().SDate;
            var FGOH = pndata.FirstOrDefault().FGOH == null ? "0" : pndata.FirstOrDefault().FGOH;
            var SHIPMENT = pndata.FirstOrDefault().SHIPMENT == null ? "0" : pndata.FirstOrDefault().SHIPMENT;
            return Convert.ToInt64(pndata.FirstOrDefault().CQty) + Convert.ToInt64(FGOH) + Convert.ToInt64(SHIPMENT);
        }

        public void GetComparisonQtyRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
                #region 提取到ComparisonQtyRes方法中
                //var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@"  SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate ORDER BY PN,SDate");
                //var Bdate = oleDB.ORM.Ado.SqlQuery<C140Res>($@"  SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate ORDER BY PN,SDate");
                //var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
                //var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
                //var pnlist = Bdate.Select(t => t.PN).Distinct();
                //DataTable dt = new DataTable();
                //dt.Columns.Add("PN");
                //dt.Columns.Add("GROUP");
                //while (minwk <= maxwk)
                //{
                //    var quarter = (minwk.Month - 1) / 3 + 1;
                //    dt.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                //    minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
                //}
                //foreach (var pn in pnlist)
                //{
                //    var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
                //    var dr = dt.NewRow();
                //    dr["GROUP"] = pndata.FirstOrDefault().G_code;
                //    dr["PN"] = pn;
                //    foreach (var item in pndata)
                //        dr[item.SDate] = item.CQty;
                //    dt.Rows.Add(dr);
                //}
                //var Bpns = oleDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
                //foreach (var itempn in Bpns)
                //{
                //    var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                //    var dr = dt.NewRow();
                //    dr["GROUP"] = pndata.FirstOrDefault().G_code;
                //    dr["PN"] = itempn.PN;
                //    foreach (var item in pndata)
                //        if (dt.Columns.Contains(item.SDate))
                //            dr[item.SDate] = item.CQty;
                //    dt.Rows.Add(dr);
                //}

                //var minusRes = new List<C140Res>();
                //oleDB.ORM.Ado.SqlQuery<C140Res>($@"select* from (
                //                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate 
                //                                 minus
                //                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate ) order by pn,sdate asc").ForEach(t =>
                //{
                //    var last = Adate.FindAll(b => b.PN == t.PN && b.SDate == t.SDate).ToList().FirstOrDefault();
                //    minusRes.Add(new C140Res()
                //    {
                //        PN = t.PN,
                //        SDate = t.SDate,
                //        CQty = t.CQty,
                //        LQty = last != null ? last.CQty : "0",
                //        Flag = last != null ? "C" : "A"
                //    });
                //});
                //oleDB.ORM.Ado.SqlQuery<C140Res>($@"select* from (
                //                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate 
                //                                 minus
                //                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate ) order by pn,sdate asc").ForEach(t =>
                //{
                //    var Current = Bdate.FindAll(a => a.PN == t.PN && a.SDate == t.SDate).ToList().FirstOrDefault();
                //    if (Current == null && !minusRes.Any(m => m.PN == t.PN && m.SDate == t.SDate))
                //        minusRes.Add(new C140Res()
                //        {
                //            PN = t.PN,
                //            SDate = t.SDate,
                //            CQty = "0",
                //            LQty = t.CQty,
                //            Flag = "D"
                //        });
                //});
                #endregion
                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonQtyRes(oleDB, I140Akey, I140Bkey, ref dt, ref minusRes, ref Bpns);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { maindata = dt, assist = minusRes, delpn = Bpns };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void ComparisonQtyRes(OleExec SFCDB, string I140Akey, string I140Bkey, ref DataTable dt, ref List<C140Res> minusRes, ref List<C140Res> Bpns)
        {
            DataTable outTable = new DataTable();
            List<C140Res> outMinusRes = new List<C140Res>();
            List<C140Res> outBpns = new List<C140Res>();

            var Adate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"  SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate ORDER BY PN,SDate");
            var Bdate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"  SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate ORDER BY PN,SDate");
            var maxwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            var pnlist = Bdate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            outTable.Columns.Add("GROUP");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }
            foreach (var pn in pnlist)
            {
                var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pndata.FirstOrDefault().G_code;
                dr["PN"] = pn;
                foreach (var item in pndata)
                    dr[item.SDate] = item.CQty;
                outTable.Rows.Add(dr);
            }
            outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
            foreach (var itempn in outBpns)
            {
                var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pndata.FirstOrDefault().G_code;
                dr["PN"] = itempn.PN;
                foreach (var item in pndata)
                    if (outTable.Columns.Contains(item.SDate))
                        dr[item.SDate] = item.CQty;
                outTable.Rows.Add(dr);
            }


            SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select* from (
                                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate 
                                                 minus
                                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate ) order by pn,sdate asc").ForEach(t =>
            {
                var last = Adate.FindAll(b => b.PN == t.PN && b.SDate == t.SDate).ToList().FirstOrDefault();
                outMinusRes.Add(new C140Res()
                {
                    PN = t.PN,
                    SDate = t.SDate,
                    CQty = t.CQty,
                    LQty = last != null ? last.CQty : "0",
                    Flag = last != null ? "C" : "A"
                });
            });
            SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select* from (
                                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate 
                                                 minus
                                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate ) order by pn,sdate asc").ForEach(t =>
            {
                var Current = Bdate.FindAll(a => a.PN == t.PN && a.SDate == t.SDate).ToList().FirstOrDefault();
                if (Current == null && !outMinusRes.Any(m => m.PN == t.PN && m.SDate == t.SDate))
                    outMinusRes.Add(new C140Res()
                    {
                        PN = t.PN,
                        SDate = t.SDate,
                        CQty = "0",
                        LQty = t.CQty,
                        Flag = "D"
                    });
            });

            dt = outTable;
            minusRes = outMinusRes;
            Bpns = outBpns;
        }
        public void GetComparisonAmountRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
                #region 提取到ComparisonAmountRes方法中
                //var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                //                                            from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Akey}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
                //var Bdate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                //                                            from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Bkey}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
                //var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
                //var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
                //var pnlist = Bdate.Select(t => t.PN).Distinct();
                //DataTable dt = new DataTable();
                //dt.Columns.Add("PN");
                //dt.Columns.Add("GROUP");
                //dt.Columns.Add("UNITPRICE");
                //dt.Columns.Add("TOTAL");
                //while (minwk <= maxwk)
                //{
                //    var quarter = (minwk.Month - 1) / 3 + 1;
                //    dt.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                //    minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
                //}
                //foreach (var pn in pnlist)
                //{
                //    var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
                //    var dr = dt.NewRow();
                //    dr["GROUP"] = pndata.FirstOrDefault().G_code;
                //    dr["PN"] = pn;
                //    dr["UNITPRICE"] = pndata.FirstOrDefault().PRICE!=null && pndata.FirstOrDefault().PRICE!=""? pndata.FirstOrDefault().PRICE:"0";
                //    decimal total = 0;
                //    foreach (var item in pndata)
                //    {
                //        dr[item.SDate] = Convert.ToInt32(item.CQty) * Convert.ToDecimal(dr["UNITPRICE"].ToString());
                //        total += Convert.ToDecimal(dr[item.SDate].ToString());
                //    }
                //    dr["TOTAL"] = total;
                //    dt.Rows.Add(dr);
                //}
                //var Bpns = oleDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
                //foreach (var itempn in Bpns)
                //{
                //    var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                //    var dr = dt.NewRow();
                //    dr["GROUP"] = pndata.FirstOrDefault().G_code;
                //    dr["PN"] = itempn.PN;
                //    foreach (var item in pndata)
                //        if (dt.Columns.Contains(item.SDate))
                //            dr[item.SDate] = item.CQty;
                //    dt.Rows.Add(dr);
                //}

                //var minusRes = new List<C140Res>();
                //oleDB.ORM.Ado.SqlQuery<C140Res>($@" select* from (
                //                              SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                //                             from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Bkey}') GROUP BY pn,g_code,PRICE, SDate  
                //                             minus 
                //                              SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                //                             from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Akey}') GROUP BY pn,g_code,PRICE, SDate   ) order by pn,SDate").ForEach(t =>
                //{
                //    var last = Adate.FindAll(b => b.PN == t.PN && b.SDate == t.SDate).ToList().FirstOrDefault();
                //    var price = t.PRICE != null ||t.PRICE!=""? t.PRICE:"0";
                //    minusRes.Add(new C140Res()
                //    {
                //        PN = t.PN,
                //        SDate = t.SDate,
                //        CQty = (Convert.ToInt32(t.CQty) * Convert.ToDecimal(t.PRICE)).ToString(),
                //        LQty = last != null ? (Convert.ToInt32(last.CQty) * Convert.ToDecimal(t.PRICE)).ToString() : "0",
                //        Flag = last != null ? "C" : "A"
                //    });
                //});
                //oleDB.ORM.Ado.SqlQuery<C140Res>($@"select* from (
                //                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate 
                //                                 minus
                //                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate ) order by pn,sdate asc").ForEach(t =>
                //{
                //    var Current = Bdate.FindAll(a => a.PN == t.PN && a.SDate == t.SDate).ToList().FirstOrDefault();
                //    var price = t.PRICE != null || t.PRICE != "" ? t.PRICE : "0";
                //    if (Current == null && !minusRes.Any(m => m.PN == t.PN && m.SDate == t.SDate))
                //        minusRes.Add(new C140Res()
                //        {
                //            PN = t.PN,
                //            SDate = t.SDate,
                //            CQty = "0",
                //            LQty = (Convert.ToInt32(t.CQty) * Convert.ToDecimal(t.PRICE)).ToString(),
                //            Flag = "D"
                //        });
                //});
                #endregion

                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonAmountRes(oleDB, I140Akey, I140Bkey, ref dt, ref minusRes, ref Bpns);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { maindata = dt, assist = minusRes, delpn = Bpns };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void ComparisonAmountRes(OleExec SFCDB, string I140Akey, string I140Bkey, ref DataTable dt, ref List<C140Res> minusRes, ref List<C140Res> Bpns)
        {
            DataTable outTable = new DataTable();
            List<C140Res> outMinusRes = new List<C140Res>();
            List<C140Res> outBpns = new List<C140Res>();

            var Adate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                                                            from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Akey}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
            var Bdate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                                                            from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Bkey}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
            var maxwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            var pnlist = Bdate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            outTable.Columns.Add("GROUP");
            outTable.Columns.Add("UNITPRICE");
            outTable.Columns.Add("TOTAL");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }
            foreach (var pn in pnlist)
            {
                var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pndata.FirstOrDefault().G_code;
                dr["PN"] = pn;
                dr["UNITPRICE"] = pndata.FirstOrDefault().PRICE != null && pndata.FirstOrDefault().PRICE != "" ? pndata.FirstOrDefault().PRICE : "0";
                decimal total = 0;
                foreach (var item in pndata)
                {
                    dr[item.SDate] = Convert.ToInt32(item.CQty) * Convert.ToDecimal(dr["UNITPRICE"].ToString());
                    total += Convert.ToDecimal(dr[item.SDate].ToString());
                }
                dr["TOTAL"] = total;
                outTable.Rows.Add(dr);
            }
            outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
            foreach (var itempn in outBpns)
            {
                var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pndata.FirstOrDefault().G_code;
                dr["PN"] = itempn.PN;
                dr["UNITPRICE"] = pndata.FirstOrDefault().PRICE != null && pndata.FirstOrDefault().PRICE != "" ? pndata.FirstOrDefault().PRICE : "0";
                foreach (var item in pndata)
                    if (outTable.Columns.Contains(item.SDate))
                        dr[item.SDate] = Convert.ToInt32(item.CQty) * Convert.ToDecimal(dr["UNITPRICE"].ToString());
                outTable.Rows.Add(dr);
            }

            SFCDB.ORM.Ado.SqlQuery<C140Res>($@" select* from (
                                              SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                                             from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Bkey}') GROUP BY pn,g_code,PRICE, SDate  
                                             minus 
                                              SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                                             from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Akey}') GROUP BY pn,g_code,PRICE, SDate   ) order by pn,SDate").ForEach(t =>
            {
                var last = Adate.FindAll(b => b.PN == t.PN && b.SDate == t.SDate).ToList().FirstOrDefault();
                var price = t.PRICE != null || t.PRICE != "" ? t.PRICE : "0";
                outMinusRes.Add(new C140Res()
                {
                    PN = t.PN,
                    SDate = t.SDate,
                    CQty = (Convert.ToInt32(t.CQty) * Convert.ToDecimal(t.PRICE)).ToString(),
                    LQty = last != null ? (Convert.ToInt32(last.CQty) * Convert.ToDecimal(t.PRICE)).ToString() : "0",
                    Flag = last != null ? "C" : "A"
                });
            });
            SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select* from (
                                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY pn,g_code, SDate 
                                                 minus
                                                 SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY pn,g_code, SDate ) order by pn,sdate asc").ForEach(t =>
            {
                var Current = Bdate.FindAll(a => a.PN == t.PN && a.SDate == t.SDate).ToList().FirstOrDefault();
                var price = t.PRICE != null || t.PRICE != "" ? t.PRICE : "0";
                if (Current == null && !outMinusRes.Any(m => m.PN == t.PN && m.SDate == t.SDate))
                    outMinusRes.Add(new C140Res()
                    {
                        PN = t.PN,
                        SDate = t.SDate,
                        CQty = "0",
                        LQty = (Convert.ToInt32(t.CQty) * Convert.ToDecimal(t.PRICE)).ToString(),
                        Flag = "D"
                    });
            });

            dt = outTable;
            minusRes = outMinusRes;
            Bpns = outBpns;
        }

        public void GetComparisonPnDiffQtyRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
                #region 提取到ComparisonQtyRes方法中
                #endregion
                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonPnDiffRes(oleDB, I140Akey, I140Bkey, ref dt);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { maindata = dt, assist = minusRes, delpn = Bpns };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void ComparisonPnDiffRes(OleExec SFCDB, string I140Akey, string I140Bkey, ref DataTable dt)
        {
            DataTable outTable = new DataTable();
            List<C140Res> outMinusRes = new List<C140Res>();
            List<C140Res> outBpns = new List<C140Res>();

            var Adate = SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Akey).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATETIME, OrderByType.Asc).ToList();
            var Bdate = SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATETIME, OrderByType.Asc).ToList();
            var GroupData = SFCDB.ORM.Queryable<R_SKU_JNP_G>().ToList();
            var maxwk = Convert.ToDateTime(Bdate.Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(Bdate.Min(t => t.STARTDATETIME));
            var pnlist = Bdate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            outTable.Columns.Add("TYPE");
            outTable.Columns.Add("GROUP");
            while (minwk <= maxwk)
            {
                outTable.Columns.Add(minwk.ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue()));
                minwk = minwk.AddDays(7);
            }
            outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
            foreach (var itempn in outBpns)
            {
                var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                var dr = outTable.NewRow();
                dr["PN"] = itempn.PN;
                dr["TYPE"] = "DEL";
                dr["GROUP"] = GroupData.FindAll(t => t.JUNIPER == itempn.PN).FirstOrDefault()?.G_CODE;
                foreach (var item in pndata)
                    if (outTable.Columns.Contains(Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())))
                        dr[Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())] = item.QUANTITY == "0" ? "0" : $@"-{item.QUANTITY}";
                outTable.Rows.Add(dr);
            }
            outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Bkey}' minus select PN from R_I140 where tranid='{I140Akey}' ");
            foreach (var itempn in outBpns)
            {
                var pndata = Bdate.FindAll(t => t.PN == itempn.PN).ToList();
                var dr = outTable.NewRow();
                dr["PN"] = itempn.PN;
                dr["TYPE"] = "NEW";
                dr["GROUP"] = GroupData.FindAll(t => t.JUNIPER == itempn.PN).FirstOrDefault()?.G_CODE;
                foreach (var item in pndata)
                    if (outTable.Columns.Contains(Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())))
                        dr[Convert.ToDateTime(item.STARTDATETIME).ToString(MES_CONST_DATETIME_FORMAT.YMD_A.ExtValue())] = item.QUANTITY == "0" ? "0" : $@"{item.QUANTITY}";
                outTable.Rows.Add(dr);
            }

            dt = outTable;
        }

        public void GetComparisonAmountDiffRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();

                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonAmountDiffRes(oleDB, I140Akey, I140Bkey, ref dt);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { maindata = dt, assist = minusRes, delpn = Bpns };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void ComparisonAmountDiffRes_220203(OleExec SFCDB, string I140Akey, string I140Bkey, ref DataTable dt)
        {
            DataTable outTable = new DataTable();
            List<C140Res> outMinusRes = new List<C140Res>();
            List<C140Res> outBpns = new List<C140Res>();

            var Adate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                                                            from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Akey}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
            var Bdate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                                                            from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Bkey}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
            var maxwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            var pnlist = Bdate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            outTable.Columns.Add("GROUP");
            outTable.Columns.Add("UNITPRICE");
            outTable.Columns.Add("TOTAL");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }
            foreach (var pn in pnlist)
            {
                var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pndata.FirstOrDefault().G_code;
                dr["PN"] = pn;
                dr["UNITPRICE"] = pndata.FirstOrDefault().PRICE != null && pndata.FirstOrDefault().PRICE != "" ? pndata.FirstOrDefault().PRICE : "0";
                decimal total = 0;
                foreach (var item in pndata)
                {
                    var itemA = Adate.FindAll(t => t.PN == pn && t.SDate == item.SDate).ToList().FirstOrDefault();
                    dr[item.SDate] = itemA != null ? ((Convert.ToInt32(item.CQty) - Convert.ToInt32(itemA.CQty)) * Convert.ToDecimal(dr["UNITPRICE"].ToString())).ToString() : (Convert.ToInt32(item.CQty) * Convert.ToDecimal(dr["UNITPRICE"].ToString())).ToString();
                    dr[item.SDate] = Convert.ToDecimal(dr[item.SDate]) == 0 ? "0.00" : dr[item.SDate];
                    total += Convert.ToDecimal(dr[item.SDate].ToString());
                }
                dr["TOTAL"] = total;
                outTable.Rows.Add(dr);
            }
            outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
            foreach (var itempn in outBpns)
            {
                var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pndata.FirstOrDefault().G_code;
                dr["PN"] = itempn.PN;
                dr["UNITPRICE"] = pndata.FirstOrDefault().PRICE != null && pndata.FirstOrDefault().PRICE != "" ? pndata.FirstOrDefault().PRICE : "0";
                foreach (var item in pndata)
                    if (outTable.Columns.Contains(item.SDate))
                        dr[item.SDate] = $@"-{Convert.ToInt32(item.CQty) * Convert.ToDecimal(dr["UNITPRICE"].ToString())}";
                outTable.Rows.Add(dr);
            }
            dt = outTable;
        }

        public void ComparisonAmountDiffRes(OleExec SFCDB, string I140Akey, string I140Bkey, ref DataTable dt)
        {
            DataTable outTable = new DataTable();
            outTable.Columns.Add("PN");
            outTable.Columns.Add("GROUP");
            outTable.Columns.Add("UNITPRICE");
            outTable.Columns.Add("TOTAL");
            var qtydt = new DataTable();
            ComparisonQtyDiffRes(SFCDB, I140Akey, I140Bkey, ref qtydt);
            var pricelist = SFCDB.ORM.Queryable<R_SKU_JNP_P>().ToList();
            var qlist = new List<string>();
            var maxwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                qlist.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }

            foreach (DataRow dr in qtydt.Rows)
            {
                decimal total = 0;
                var pnprice = pricelist.FindAll(t => t.JUNIPER == dr["PN"].ToString()).ToList().FirstOrDefault();
                var ndr = outTable.NewRow();
                ndr["PN"] = dr["PN"];
                ndr["GROUP"] = dr["GROUP"];
                ndr["UNITPRICE"] = pnprice == null ? "0.00" : pnprice.PRICE;
                foreach (var drc in qlist)
                {
                    ndr[drc] = Convert.ToDecimal(string.IsNullOrEmpty(dr[drc].ToString())?"0.00":dr[drc].ToString()) * Convert.ToDecimal(ndr["UNITPRICE"]==null?"0.00":ndr["UNITPRICE"].ToString());
                    total += Convert.ToDecimal(ndr[drc] == null ? "0.00" : ndr[drc]);
                }
                ndr["TOTAL"] = total;
                outTable.Rows.Add(ndr);
            }
            dt = outTable;


            //DataTable outTable = new DataTable();
            //List<C140Res> outMinusRes = new List<C140Res>();
            //List<C140Res> outBpns = new List<C140Res>();

            //var Adate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
            //                                                from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Akey}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
            //var Bdate = SFCDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
            //                                                from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Bkey}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
            //var maxwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            //var minwk = Convert.ToDateTime(SFCDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            //var pnlist = Bdate.Select(t => t.PN).Distinct();

            //outTable.Columns.Add("PN");
            //outTable.Columns.Add("GROUP");
            //outTable.Columns.Add("UNITPRICE");
            //outTable.Columns.Add("TOTAL");
            //while (minwk <= maxwk)
            //{
            //    var quarter = (minwk.Month - 1) / 3 + 1;
            //    outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
            //    minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            //}
            //foreach (var pn in pnlist)
            //{
            //    var pndata = Bdate.FindAll(t => t.PN == pn).ToList();
            //    var dr = outTable.NewRow();
            //    dr["GROUP"] = pndata.FirstOrDefault().G_code;
            //    dr["PN"] = pn;
            //    dr["UNITPRICE"] = pndata.FirstOrDefault().PRICE != null && pndata.FirstOrDefault().PRICE != "" ? pndata.FirstOrDefault().PRICE : "0";
            //    decimal total = 0;
            //    foreach (var item in pndata)
            //    {
            //        var itemA = Adate.FindAll(t => t.PN == pn && t.SDate == item.SDate).ToList().FirstOrDefault();
            //        dr[item.SDate] = itemA != null ? ((Convert.ToInt32(item.CQty) - Convert.ToInt32(itemA.CQty)) * Convert.ToDecimal(dr["UNITPRICE"].ToString())).ToString() : (Convert.ToInt32(item.CQty) * Convert.ToDecimal(dr["UNITPRICE"].ToString())).ToString();
            //        dr[item.SDate] = Convert.ToDecimal(dr[item.SDate]) == 0 ? "0.00" : dr[item.SDate];
            //        total += Convert.ToDecimal(dr[item.SDate].ToString());
            //    }
            //    dr["TOTAL"] = total;
            //    outTable.Rows.Add(dr);
            //}
            //outBpns = SFCDB.ORM.Ado.SqlQuery<C140Res>($@"select PN from R_I140 where tranid='{I140Akey}' minus select PN from R_I140 where tranid='{I140Bkey}' ");
            //foreach (var itempn in outBpns)
            //{
            //    var pndata = Adate.FindAll(t => t.PN == itempn.PN).ToList();
            //    var dr = outTable.NewRow();
            //    dr["GROUP"] = pndata.FirstOrDefault().G_code;
            //    dr["PN"] = itempn.PN;
            //    dr["UNITPRICE"] = pndata.FirstOrDefault().PRICE != null && pndata.FirstOrDefault().PRICE != "" ? pndata.FirstOrDefault().PRICE : "0";
            //    foreach (var item in pndata)
            //        if (outTable.Columns.Contains(item.SDate))
            //            dr[item.SDate] = $@"-{Convert.ToInt32(item.CQty) * Convert.ToDecimal(dr["UNITPRICE"].ToString())}";
            //    outTable.Rows.Add(dr);
            //}
            //dt = outTable;
        }

        public void GetQuarterQtyRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140key = Data["TRANID"].ToString().Trim();
                #region 提取到ComparisonQtyRes方法中
                DataTable outTable = new DataTable();
                outTable = GetQuarterQty(oleDB, I140key);
                #endregion
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = outTable;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        DataTable GetQuarterQty(OleExec oleDB, string I140key)
        {
            DataTable outTable = new DataTable();
            var daykey = Convert.ToDateTime($@"{I140key.Substring(0, 4)}-{I140key.Substring(4, 2)}-{I140key.Substring(6, 2)}");
            //var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@"  SELECT pn,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140key}') GROUP BY pn,g_code, SDate ORDER BY PN,SDate");
            var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@"   SELECT pn,new_order_quantity as FGOH,
                        SHIPMENT,g_code, SDate ,SUM(quantity) as CQty FROM ( select pn, new_order_quantity,
                        SHIPMENT,d.g_code,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity from R_I140 c 
                        left join R_SKU_JNP_G d
                                             on c.pn = d.juniper
                                           left join (select a.skuno, count(distinct a.sn) as SHIPMENT
                                                       from r_ship_detail a, r_sn b
                                                      where a.shipdate between
                                                            to_date('{GetCurrentQuarterFirstDay(daykey)}',
                                                                    'yyyy-mm-dd HH24:MI:SS') and
                                                            to_date('{GetCurrentTuesday(daykey).AddHours(9)}',
                                                                    'yyyy-mm-dd HH24:MI:SS')
                                                        and a.sn = b.sn
                                                      group by a.skuno) bb
                                             on c.pn = bb.skuno
                                           left join (select b.*
                                                       from r_sap_file a, r_sap_file_i605 b
                                                      where a.id = b.file_id
                                                        and a.file_name like
                                                            '{GetCurrentTuesday(daykey).ToString("yyyyMMdd")}%') aa
                                             on c.pn = aa.item_name where tranid='{I140key}') GROUP BY pn,g_code,new_order_quantity,
                                                SHIPMENT, SDate ORDER BY PN,SDate");
            var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140key).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140key).Min(t => t.STARTDATETIME));
            var pnlist = Adate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            outTable.Columns.Add("GROUP");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }
            foreach (var pn in pnlist)
            {
                var pnBdata = Adate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pnBdata.FirstOrDefault().G_code;
                dr["PN"] = pn;
                var currentQuarter = pnBdata.FirstOrDefault().SDate;
                var FGOH = pnBdata.FirstOrDefault().FGOH == null ? "0" : pnBdata.FirstOrDefault().FGOH;
                var SHIPMENT = pnBdata.FirstOrDefault().SHIPMENT == null ? "0" : pnBdata.FirstOrDefault().SHIPMENT;
                dr[currentQuarter] = Convert.ToInt64(pnBdata.FirstOrDefault().CQty) + Convert.ToInt64(FGOH) + Convert.ToInt64(SHIPMENT);
                foreach (var item in pnBdata)
                    if (item.SDate != currentQuarter)
                        dr[item.SDate] = item.CQty;
                outTable.Rows.Add(dr);
            }
            return outTable;
        }

        public void GetQuarterAmountRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140key = Data["TRANID"].ToString().Trim();
                DataTable outTable = new DataTable();
                outTable = QuarterAmount(oleDB, I140key);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = outTable;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        DataTable QuarterAmount(OleExec oleDB, string I140key)
        {
            #region 提取到ComparisonQtyRes方法中
            DataTable outTable = new DataTable();
            var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT pn,g_code,PRICE, SDate ,SUM(quantity) as CQty FROM ( select pn,b.g_code,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                                                            from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140key}') GROUP BY pn,g_code,PRICE, SDate  order by pn,sdate asc");
            var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140key).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140key).Min(t => t.STARTDATETIME));
            var pnlist = Adate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            outTable.Columns.Add("GROUP");
            outTable.Columns.Add("UNITPRICE");
            outTable.Columns.Add("TOTAL");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }
            foreach (var pn in pnlist)
            {
                var pndata = Adate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["GROUP"] = pndata.FirstOrDefault().G_code;
                dr["PN"] = pn;
                dr["UNITPRICE"] = pndata.FirstOrDefault().PRICE != null && pndata.FirstOrDefault().PRICE != "" ? pndata.FirstOrDefault().PRICE : "0";
                decimal total = 0;
                foreach (var item in pndata)
                {
                    dr[item.SDate] = (Convert.ToInt32(item.CQty) * Convert.ToDecimal(dr["UNITPRICE"].ToString())).ToString();
                    dr[item.SDate] = Convert.ToDecimal(dr[item.SDate]) == 0 ? "0.00" : dr[item.SDate];
                    total += Convert.ToDecimal(dr[item.SDate].ToString());
                }
                dr["TOTAL"] = total;
                outTable.Rows.Add(dr);
            }
            #endregion
            return outTable;
        }

        DataTable QuarterQtyByGroup(OleExec oleDB, string I140key)
        {
            #region 提取到ComparisonQtyRes方法中
            DataTable outTable = new DataTable();

            var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT description as PN, SDate ,SUM(quantity) as CQty FROM ( select pn,b.description,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity
                                                                from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140key}') GROUP BY description, SDate ORDER BY description,SDate");
            var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140key).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140key).Min(t => t.STARTDATETIME));
            var pnlist = Adate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }
            foreach (var pn in pnlist)
            {
                var pnBdata = Adate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["PN"] = pn;
                foreach (var item in pnBdata)
                    dr[item.SDate] = item.CQty;
                outTable.Rows.Add(dr);
            }
            #endregion
            return outTable;
        }

        public void GetQuarterQtyByGroupRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140key = Data["TRANID"].ToString().Trim();
                DataTable outTable = new DataTable();
                outTable = QuarterQtyByGroup(oleDB, I140key);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = outTable;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetQuarterAmountByGroupRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140key = Data["TRANID"].ToString().Trim();
                DataTable outTable = new DataTable();
                outTable = QuarterAmountByGroup(oleDB, I140key);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = outTable;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        DataTable QuarterAmountByGroup(OleExec oleDB, string I140key)
        {
            #region 提取到ComparisonQtyRes方法中
            DataTable outTable = new DataTable();
            var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" select description as PN,sdate,sum(tolprice) as CQty  from (
                                                                SELECT pn,description,PRICE, SDate ,SUM(quantity) as CQty,PRICE*SUM(quantity) as tolprice FROM ( select pn,description,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
                                                                 from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140key}') GROUP BY pn,description,PRICE, SDate  ) aa
                                                                 group by description,sdate ORDER BY description");
            var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140key).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140key).Min(t => t.STARTDATETIME));
            var pnlist = Adate.Select(t => t.PN).Distinct();

            outTable.Columns.Add("PN");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }
            foreach (var pn in pnlist)
            {
                var pndata = Adate.FindAll(t => t.PN == pn).ToList();
                var dr = outTable.NewRow();
                dr["PN"] = pn;
                decimal total = 0;
                foreach (var item in pndata)
                    dr[item.SDate] = item.CQty;
                outTable.Rows.Add(dr);
            }
            #endregion
            return outTable;
        }

        //public void GetQuarterQtyDiffRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    OleExec oleDB = null;
        //    try
        //    {
        //        oleDB = this.DBPools["SFCDB"].Borrow();
        //        string I140Akey = Data["I140Akey"].ToString().Trim();
        //        string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
        //        var res = QuarterQtyDiffRes(oleDB, I140Akey, I140Bkey);
        //        StationReturn.Status = StationReturnStatusValue.Pass;
        //        StationReturn.MessageCode = "MES00000026";
        //        StationReturn.Data = res;
        //        //StationReturn.Data = new { maindata = outTable, assist = minusRes, delpn = Bpns };
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(oleDB);
        //    }
        //}

        public class QuarterQtyDiffResobj
        {
            public DataTable maindata { get; set; }
            public List<C140Res> assist { get; set; }
            public List<C140Res> delpn { get; set; }
        }

        public QuarterQtyDiffResobj QuarterQtyDiffRes(OleExec oleDB, string I140Akey, string I140Bkey, DataTable qdt)
        {
            DataTable outTable = new DataTable();
            var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            var qlist = new List<string>();
            outTable.Columns.Add("PN");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                qlist.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }

            var groupobj = oleDB.ORM.Queryable<R_SKU_JNP_G>().ToList();
            var grouplist = groupobj.Select(t => t.DESCRIPTION).Distinct().ToList();
            foreach (var item in grouplist)
            {
                var ndr = outTable.NewRow();
                ndr["PN"] = item;
                foreach (var qi in qlist)
                    ndr[qi] = "0";
                outTable.Rows.Add(ndr);
            }
            var dr = outTable.NewRow();
            dr["PN"] = "other";
            foreach (var qi in qlist)
                dr[qi] = "0";
            outTable.Rows.Add(dr);
            foreach (DataRow item in qdt.Rows)
            {
                foreach (var qi in qlist)
                {
                    var gr = groupobj.FindAll(t => t.JUNIPER == item["PN"].ToString()).ToList().FirstOrDefault();
                    if (gr == null)
                    {
                        var ta = outTable.Select($@" PN='other' ");
                        ta[0][qi] = Convert.ToDecimal(string.IsNullOrEmpty(ta[0][qi].ToString()) ? "0.00" : ta[0][qi].ToString()) + Convert.ToDecimal(string.IsNullOrEmpty(item[qi].ToString()) ? "0.00" : item[qi].ToString());
                    }
                    else
                    {
                        var ta = outTable.Select($@" PN='{gr.DESCRIPTION}' ");
                        ta[0][qi] = Convert.ToDecimal(string.IsNullOrEmpty(ta[0][qi].ToString())?"0.00":ta[0][qi].ToString()) + Convert.ToDecimal(string.IsNullOrEmpty(item[qi].ToString())?"0.00":item[qi].ToString());
                    }

                }
            }

            return new QuarterQtyDiffResobj() { maindata = outTable };




            //#region 提取到ComparisonQtyRes方法中
            //DataTable outTable = new DataTable();
            //var minusRes = new List<C140Res>();
            //var Bpns = new List<C140Res>();
            //var dayAkey = Convert.ToDateTime($@"{I140Akey.Substring(0, 4)}-{I140Akey.Substring(4, 2)}-{I140Akey.Substring(6, 2)}");
            //var dayBkey = Convert.ToDateTime($@"{I140Bkey.Substring(0, 4)}-{I140Bkey.Substring(4, 2)}-{I140Bkey.Substring(6, 2)}");
            ////var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT  decode ( description ,null , 'other',description ) as PN, SDate ,SUM(quantity) as CQty FROM ( select pn,b.description,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity
            ////                                                from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Akey}') GROUP BY description, SDate ORDER BY description,SDate");
            ////var Bdate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT  decode ( description ,null , 'other',description ) as PN, SDate ,SUM(quantity) as CQty FROM ( select pn,b.description,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity
            ////                                                from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper where tranid='{I140Bkey}') GROUP BY description, SDate ORDER BY description,SDate");
            //var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT  decode ( description ,null , 'other',description ) as PN,sum(new_order_quantity),sum(SHIPMENT), SDate ,SUM(quantity) as CQty
            //                                      FROM ( select pn,d.description,decode( new_order_quantity,null,0,new_order_quantity) new_order_quantity,decode( SHIPMENT,null,0,SHIPMENT) SHIPMENT,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity
            //                                                                                                    from R_I140 c 
            //                                    left join R_SKU_JNP_G d
            //                                                         on c.pn = d.juniper
            //                                                       left join (select a.skuno, count(distinct a.sn) as SHIPMENT
            //                                                                   from r_ship_detail a, r_sn b
            //                                                           where a.shipdate between
            //                                                            to_date('{GetCurrentQuarterFirstDay(dayAkey)}',
            //                                                                    'yyyy-mm-dd HH24:MI:SS') and
            //                                                            to_date('{GetCurrentTuesday(dayAkey).AddHours(9)}',
            //                                                        'yyyy-mm-dd HH24:MI:SS')
            //                                                                    and a.sn = b.sn
            //                                                                  group by a.skuno) bb
            //                                                         on c.pn = bb.skuno
            //                                                       left join (select b.item_name,sum(new_order_quantity) new_order_quantity
            //                                                                   from r_sap_file a, r_sap_file_i605 b
            //                                                                  where a.id = b.file_id
            //                                                                    and a.file_name like
            //                                                                          '{GetCurrentTuesday(dayAkey).ToString("yyyyMMdd")}%'  group by b.item_name) aa
            //                                                         on c.pn = aa.item_name
            //                                     where tranid='{I140Akey}') 
            //                                                                                                    GROUP BY description, SDate ORDER BY description,SDate");
            //var AA = $@" SELECT  decode ( description ,null , 'other',description ) as PN,sum(new_order_quantity),sum(SHIPMENT), SDate ,SUM(quantity) as CQty
            //                                      FROM ( select pn,d.description,decode( new_order_quantity,null,0,new_order_quantity) new_order_quantity,decode( SHIPMENT,null,0,SHIPMENT) SHIPMENT,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity
            //                                                                                                    from R_I140 c 
            //                                    left join R_SKU_JNP_G d
            //                                                         on c.pn = d.juniper
            //                                                       left join (select a.skuno, count(distinct a.sn) as SHIPMENT
            //                                                                   from r_ship_detail a, r_sn b
            //                                                           where a.shipdate between
            //                                                            to_date('{GetCurrentQuarterFirstDay(dayAkey)}',
            //                                                                    'yyyy-mm-dd HH24:MI:SS') and
            //                                                            to_date('{GetCurrentTuesday(dayAkey).AddHours(9)}',
            //                                                        'yyyy-mm-dd HH24:MI:SS')
            //                                                                    and a.sn = b.sn
            //                                                                  group by a.skuno) bb
            //                                                         on c.pn = bb.skuno
            //                                                       left join (select b.item_name,sum(new_order_quantity) new_order_quantity
            //                                                                   from r_sap_file a, r_sap_file_i605 b
            //                                                                  where a.id = b.file_id
            //                                                                    and a.file_name like
            //                                                                          '{GetCurrentTuesday(dayAkey).ToString("yyyyMMdd")}%'  group by b.item_name) aa
            //                                                         on c.pn = aa.item_name
            //                                     where tranid='{I140Akey}') 
            //                                                                                                    GROUP BY description, SDate ORDER BY description,SDate";
            //var Bdate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" SELECT  decode ( description ,null , 'other',description ) as PN,sum(new_order_quantity),sum(SHIPMENT), SDate ,SUM(quantity) as CQty
            //                                      FROM ( select pn,d.description,decode( new_order_quantity,null,0,new_order_quantity) new_order_quantity,decode( SHIPMENT,null,0,SHIPMENT) SHIPMENT,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity
            //                                                                                                    from R_I140 c 
            //                                    left join R_SKU_JNP_G d
            //                                                         on c.pn = d.juniper
            //                                                       left join (select a.skuno, count(distinct a.sn) as SHIPMENT
            //                                                                   from r_ship_detail a, r_sn b
            //                                                           where a.shipdate between
            //                                                            to_date('{GetCurrentQuarterFirstDay(dayBkey)}',
            //                                                                    'yyyy-mm-dd HH24:MI:SS') and
            //                                                            to_date('{GetCurrentTuesday(dayBkey).AddHours(9)}',
            //                                                        'yyyy-mm-dd HH24:MI:SS')
            //                                                                    and a.sn = b.sn
            //                                                                  group by a.skuno) bb
            //                                                         on c.pn = bb.skuno
            //                                                       left join (select b.item_name,sum(new_order_quantity) new_order_quantity
            //                                                                   from r_sap_file a, r_sap_file_i605 b
            //                                                                  where a.id = b.file_id
            //                                                                    and a.file_name like
            //                                                                          '{GetCurrentTuesday(dayBkey).ToString("yyyyMMdd")}%'  group by b.item_name) aa
            //                                                         on c.pn = aa.item_name
            //                                     where tranid='{I140Bkey}') 
            //                                                                                                    GROUP BY description, SDate ORDER BY description,SDate");

            //var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            //var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            //var pnlist = Bdate.Select(t => t.PN).Distinct();

            //outTable.Columns.Add("PN");
            //while (minwk <= maxwk)
            //{
            //    var quarter = (minwk.Month - 1) / 3 + 1;
            //    outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
            //    minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            //}
            //foreach (var pn in pnlist)
            //{
            //    var pnBdata = Bdate.FindAll(t => t.PN == pn).ToList();
            //    var dr = outTable.NewRow();
            //    //dr["GROUP"] = pnBdata.FirstOrDefault().G_code;
            //    dr["PN"] = pn;

            //    var pnAdata = Adate.FindAll(t => t.PN == pn).ToList();
            //    var currentQuarter = pnBdata.FirstOrDefault().SDate;
            //    dr[currentQuarter] = GetQtyWithFirstQuarterByPn(pnBdata) - GetQtyWithFirstQuarterByPn(pnAdata);

            //    foreach (var item in pnBdata)
            //        if (item.SDate != currentQuarter)
            //        {
            //            var itemA = Adate.FindAll(t => t.PN == pn && t.SDate == item.SDate).ToList().FirstOrDefault();
            //            dr[item.SDate] = itemA != null ? (int.Parse(item.CQty) - int.Parse(itemA.CQty)).ToString() : item.CQty;
            //        }

            //    //foreach (var item in pnBdata)
            //    //{
            //    //    var itemA = Adate.FindAll(t => t.PN == pn && t.SDate == item.SDate).ToList().FirstOrDefault();
            //    //    dr[item.SDate] = itemA != null ? (int.Parse(item.CQty) - int.Parse(itemA.CQty)).ToString() : item.CQty;
            //    //}
            //    outTable.Rows.Add(dr);
            //}
            //#endregion
            //return new QuarterQtyDiffResobj() { maindata = outTable, assist = minusRes, delpn = Bpns };
        }

        //public void GetQuarterAmountDiffRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //{
        //    OleExec oleDB = null;
        //    try
        //    {
        //        oleDB = this.DBPools["SFCDB"].Borrow();
        //        string I140Akey = Data["I140Akey"].ToString().Trim();
        //        string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
        //        var res =QuarterAmountDiffRes(oleDB, I140Akey, I140Bkey);
        //        StationReturn.Status = StationReturnStatusValue.Pass;
        //        StationReturn.MessageCode = "MES00000026";
        //        StationReturn.Data = res;
        //        //StationReturn.Data = new { maindata = outTable, assist = minusRes, delpn = Bpns };
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //    finally
        //    {
        //        this.DBPools["SFCDB"].Return(oleDB);
        //    }
        //}

        public QuarterQtyDiffResobj QuarterAmountDiffRes(OleExec oleDB, string I140Akey, string I140Bkey, DataTable qtydtobj)
        {
            DataTable outTable = new DataTable();
            var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            var qlist = new List<string>();
            outTable.Columns.Add("PN");
            while (minwk <= maxwk)
            {
                var quarter = (minwk.Month - 1) / 3 + 1;
                outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                qlist.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
                minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            }

            var groupobj = oleDB.ORM.Queryable<R_SKU_JNP_G>().ToList();
            var grouplist = groupobj.Select(t => t.DESCRIPTION).Distinct().ToList();
            foreach (var item in grouplist)
            {
                var ndr = outTable.NewRow();
                ndr["PN"] = item;
                foreach (var qi in qlist)
                    ndr[qi] = "0";
                outTable.Rows.Add(ndr);
            }
            var dr = outTable.NewRow();
            dr["PN"] = "other";
            foreach (var qi in qlist)
                dr[qi] = "0";
            outTable.Rows.Add(dr);
            foreach (DataRow item in qtydtobj.Rows)
            {
                foreach (var qi in qlist)
                {
                    var gr = groupobj.FindAll(t => t.JUNIPER == item["PN"].ToString()).ToList().FirstOrDefault();
                    if (gr == null)
                    {
                        var ta = outTable.Select($@" PN='other' ");
                        ta[0][qi] = Convert.ToDecimal(ta[0][qi] == null ? "0.00" : ta[0][qi].ToString()) + Convert.ToDecimal(item[qi] == null ? "0.00" : item[qi].ToString());
                    }
                    else
                    {
                        var ta = outTable.Select($@" PN='{gr.DESCRIPTION}' ");
                        ta[0][qi] = Convert.ToDecimal(ta[0][qi] == null ? "0.00" : ta[0][qi].ToString()) + Convert.ToDecimal(item[qi] == null ? "0.00" : item[qi].ToString());
                    }
                }
            }


            //foreach (DataRow item in qtydt.Rows)
            //{
            //    DataRow dr = outTable.NewRow();
            //    dr["PN"] = item["PN"];
            //    var pnprice = pricelist.FindAll(t => t.JUNIPER == item["PN"].ToString()).ToList().FirstOrDefault();
            //    var uniprice = pnprice == null ? "0.00" : pnprice.PRICE;
            //    foreach (var drc in qlist)                
            //        dr[drc] = Convert.ToDecimal(item[drc].ToString()) * Convert.ToDecimal(uniprice);
            //    outTable.Rows.Add(dr);
            //}
            return new QuarterQtyDiffResobj() { maindata = outTable };



            //#region 提取到ComparisonQtyRes方法中
            //DataTable outTable = new DataTable();
            //var minusRes = new List<C140Res>();
            //var Bpns = new List<C140Res>();
            //var Adate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" select decode ( description ,null , 'other',description ) as PN,sdate,sum(tolprice) as CQty  from (
            //                                                    SELECT pn,description,PRICE, SDate ,SUM(quantity) as CQty,PRICE*SUM(quantity) as tolprice FROM ( select pn,description,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
            //                                                     from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Akey}') GROUP BY pn,description,PRICE, SDate  ) aa
            //                                                     group by description,sdate ORDER BY description");
            //var Bdate = oleDB.ORM.Ado.SqlQuery<C140Res>($@" select decode ( description ,null , 'other',description ) as PN,sdate,sum(tolprice) as CQty  from (
            //                                                    SELECT pn,description,PRICE, SDate ,SUM(quantity) as CQty,PRICE*SUM(quantity) as tolprice FROM ( select pn,description,PRICE,SUBSTR(to_char( startdatetime,'yyyy'),3)||''''||'Q'||to_char( startdatetime,'q') as SDate,quantity 
            //                                                     from R_I140 a left join R_SKU_JNP_G b on a.pn = b.juniper LEFT JOIN R_SKU_JNP_P C ON A.PN=C.JUNIPER where tranid='{I140Bkey}') GROUP BY pn,description,PRICE, SDate  ) aa
            //                                                     group by description,sdate ORDER BY description");

            //var maxwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Max(t => t.STARTDATETIME));
            //var minwk = Convert.ToDateTime(oleDB.ORM.Queryable<R_I140>().Where(t => t.TRANID == I140Bkey).Min(t => t.STARTDATETIME));
            //var pnlist = Bdate.Select(t => t.PN).Distinct();

            //outTable.Columns.Add("PN");
            //while (minwk <= maxwk)
            //{
            //    var quarter = (minwk.Month - 1) / 3 + 1;
            //    outTable.Columns.Add($@"{minwk.Year.ToString().Substring(2)}'Q{quarter}");
            //    minwk = minwk.AddMonths(0 - (minwk.Month - 1) % 3).AddDays(1 - minwk.Day).AddMonths(3);
            //}
            //foreach (var pn in pnlist)
            //{
            //    var pnBdata = Bdate.FindAll(t => t.PN == pn).ToList();
            //    var dr = outTable.NewRow();
            //    //dr["GROUP"] = pnBdata.FirstOrDefault().G_code;
            //    dr["PN"] = pn;
            //    foreach (var item in pnBdata)
            //    {
            //        var itemA = Adate.FindAll(t => t.PN == pn && t.SDate == item.SDate).ToList().FirstOrDefault();
            //        dr[item.SDate] = itemA != null ? (Convert.ToDecimal(item.CQty) - Convert.ToDecimal(itemA.CQty)).ToString() : item.CQty;
            //    }
            //    outTable.Rows.Add(dr);
            //}
            //#endregion
            //return new { maindata = outTable, assist = minusRes, delpn = Bpns };
        }

        public void Get140Comparison(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();

                DataTable dt = new DataTable();
                var Detaila = Geti140DataByTranid(oleDB, I140Akey);
                var Detailb = Geti140DataByTranid(oleDB, I140Bkey);
                var QuarterQtya = GetQuarterQty(oleDB, I140Akey);
                var QuarterQtyb = GetQuarterQty(oleDB, I140Bkey);
                var QuarterAmounta = QuarterAmount(oleDB, I140Akey);
                var QuarterAmountb = QuarterAmount(oleDB, I140Bkey);
                var QuarterQtyByGroupa = QuarterQtyByGroup(oleDB, I140Akey);
                var QuarterQtyByGroupb = QuarterQtyByGroup(oleDB, I140Bkey);
                var QuarterAmountByGroupa = QuarterAmountByGroup(oleDB, I140Akey);
                var QuarterAmountByGroupb = QuarterAmountByGroup(oleDB, I140Bkey);

                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonDiffRes(oleDB, I140Akey, I140Bkey, ref dt);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { Detaila, Detailb, QuarterQtya, QuarterQtyb, QuarterAmounta, QuarterAmountb, QuarterQtyByGroupa, QuarterQtyByGroupb, QuarterAmountByGroupa, QuarterAmountByGroupb };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void Get140ComparisonDiff(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();

                DataTable QtydtDiffdt = new DataTable();
                DataTable AmountdtDiffdt = new DataTable();
                DataTable ComparisonDiffdt = new DataTable();
                DataTable ComparisonPNDiffdt = new DataTable();

                ComparisonQtyDiffRes(oleDB, I140Akey, I140Bkey, ref QtydtDiffdt);
                ComparisonAmountDiffRes(oleDB, I140Akey, I140Bkey, ref AmountdtDiffdt);
                ComparisonDiffRes(oleDB, I140Akey, I140Bkey, ref ComparisonDiffdt);
                ComparisonPnDiffRes(oleDB, I140Akey, I140Bkey, ref ComparisonPNDiffdt);
                QuarterQtyDiffResobj SQtydt = QuarterQtyDiffRes(oleDB, I140Akey, I140Bkey, QtydtDiffdt);
                object SAmountdt = QuarterAmountDiffRes(oleDB, I140Akey, I140Bkey, AmountdtDiffdt);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                var QtydtDiff = new { maindata = QtydtDiffdt };
                var AmountdtDiff = new { maindata = AmountdtDiffdt };
                var ComparisonDiff = new { maindata = ComparisonDiffdt };
                var ComparisonPNDiff = new { maindata = ComparisonPNDiffdt };
                StationReturn.Data = new { QtydtDiff, AmountdtDiff, ComparisonDiff, ComparisonPNDiff, SAmountdt, SQtydt };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetSkuPriceList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<R_SKU_JNP_P> list = SFCDB.ORM.Queryable<R_SKU_JNP_P>().ToList();
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
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
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }
        public void UploadSkuPrice(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                //定義上傳Excel的列名
                List<string> inputTitle = new List<string> { "JUNIPER", "FOXCONN", "PRICE" };
                string errTitle = "";
                string juniper = "", foxconn = "", price = "";
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception($@"File is null!");
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool hasErr = CheckInputExcelTitle(firstData, inputTitle, out errTitle);
                if (!hasErr)
                {
                    throw new Exception($@"No {errTitle}!");
                }
                string result = "OK";
                string failMsg = "";
                R_SKU_JNP_P skuPrice = null;
                T_R_SKU_JNP_P t_r_sku_jnp_p = new T_R_SKU_JNP_P(SFCDB, DB_TYPE_ENUM.Oracle);
                DateTime sysdate = SFCDB.ORM.GetDate();

                #region 写入数据库               
                for (int i = 0; i < array.Count; i++)
                {
                    juniper = array[i]["JUNIPER"].ToString().ToUpper().Trim();
                    foxconn = array[i]["FOXCONN"].ToString().ToUpper().Trim();
                    price = array[i]["PRICE"].ToString().ToUpper().Trim();
                    if (string.IsNullOrEmpty(juniper) || string.IsNullOrEmpty(foxconn) || string.IsNullOrEmpty(price))
                    {
                        failMsg += $@";JUNIPER:{juniper},FOXCONN:{foxconn},PRICE:{price},is null";
                        continue;
                    }
                    try
                    {
                        SFCDB.ORM.Deleteable<R_SKU_JNP_P>().Where(r => r.JUNIPER == juniper && r.FOXCONN == foxconn).ExecuteCommand();
                        skuPrice = new R_SKU_JNP_P();
                        skuPrice.ID = t_r_sku_jnp_p.GetNewID(BU, SFCDB);
                        skuPrice.JUNIPER = juniper;
                        skuPrice.FOXCONN = foxconn;
                        skuPrice.PRICE = price;
                        skuPrice.CREATEBY = LoginUser.EMP_NO;
                        skuPrice.CREATETIME = sysdate;
                        if (SFCDB.ORM.Insertable<R_SKU_JNP_P>(skuPrice).ExecuteCommand() == 0)
                        {
                            throw new Exception("Save Fail.");
                        }
                    }
                    catch (Exception ex)
                    {
                        failMsg += $@";JUNIPER:{juniper},FOXCONN:{foxconn},PRICE:{price},Fail Messgae:{ex.Message}";
                    }
                }

                #endregion

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = result + failMsg;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetSkuGroupList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<R_SKU_JNP_G> list = SFCDB.ORM.Queryable<R_SKU_JNP_G>().ToList();
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
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
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }
        public void UploadSkuGroup(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                //定義上傳Excel的列名
                List<string> inputTitle = new List<string> { "JUNIPER", "FOXCONN", "G_CODE", "DESCRIPTION" };
                string errTitle = "";
                string juniper = "", foxconn = "", gCode = "", description = "";
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception($@"File is null!");
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool hasErr = CheckInputExcelTitle(firstData, inputTitle, out errTitle);
                if (!hasErr)
                {
                    throw new Exception($@"No {errTitle}!");
                }
                string result = "OK";
                string failMsg = "";
                R_SKU_JNP_G skuGroup = null;
                T_R_SKU_JNP_G t_r_sku_jnp_p = new T_R_SKU_JNP_G(SFCDB, DB_TYPE_ENUM.Oracle);
                DateTime sysdate = SFCDB.ORM.GetDate();

                #region 写入数据库               
                for (int i = 0; i < array.Count; i++)
                {
                    juniper = array[i]["JUNIPER"].ToString().ToUpper().Trim();
                    foxconn = array[i]["FOXCONN"].ToString().ToUpper().Trim();
                    gCode = array[i]["G_CODE"].ToString().ToUpper().Trim();
                    description = array[i]["DESCRIPTION"].ToString().Trim();
                    if (string.IsNullOrEmpty(juniper) || string.IsNullOrEmpty(foxconn) || string.IsNullOrEmpty(gCode) || string.IsNullOrEmpty(description))
                    {
                        failMsg += $@";JUNIPER:{juniper},FOXCONN:{foxconn},G_CODE:{gCode},DESCRIPTION:{description},is null";
                        continue;
                    }
                    try
                    {
                        SFCDB.ORM.Deleteable<R_SKU_JNP_G>().Where(r => r.JUNIPER == juniper && r.FOXCONN == foxconn).ExecuteCommand();
                        skuGroup = new R_SKU_JNP_G();
                        skuGroup.ID = t_r_sku_jnp_p.GetNewID(BU, SFCDB);
                        skuGroup.JUNIPER = juniper;
                        skuGroup.FOXCONN = foxconn;
                        skuGroup.G_CODE = gCode;
                        skuGroup.DESCRIPTION = description;
                        skuGroup.CREATEBY = LoginUser.EMP_NO;
                        skuGroup.CREATETIME = sysdate;
                        if (SFCDB.ORM.Insertable<R_SKU_JNP_G>(skuGroup).ExecuteCommand() == 0)
                        {
                            throw new Exception("Save Fail.");
                        }
                    }
                    catch (Exception ex)
                    {
                        failMsg += $@";JUNIPER:{juniper},FOXCONN:{foxconn},G_CODE:{gCode},DESCRIPTION:{description},Fail Messgae:{ex.Message}";
                    }
                }

                #endregion

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = result + failMsg;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void DownloadComparisonRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonRes(oleDB, I140Akey, I140Bkey, ref dt, ref minusRes, ref Bpns);
                string fileName = $@"ComparisonRes_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                byte[] output_excel = MakeExcel(dt, minusRes, Bpns, fileName);
                string content = Convert.ToBase64String(output_excel);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { FileName = fileName, Content = content }; ;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void DownloadComparisonQtyRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonQtyRes(oleDB, I140Akey, I140Bkey, ref dt, ref minusRes, ref Bpns);
                string fileName = $@"QtyRes_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                byte[] output_excel = MakeExcel(dt, minusRes, Bpns, fileName);
                string content = Convert.ToBase64String(output_excel);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { FileName = fileName, Content = content }; ;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void DownloadComparisonAmountRes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string I140Akey = Data["I140Akey"].ToString().Trim();
                string I140Bkey = Data["I140Bkey"].ToString().Trim().ToUpper();
                DataTable dt = new DataTable();
                var minusRes = new List<C140Res>();
                var Bpns = new List<C140Res>();
                ComparisonAmountRes(oleDB, I140Akey, I140Bkey, ref dt, ref minusRes, ref Bpns);
                string fileName = $@"AmountRes_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                byte[] output_excel = MakeExcel(dt, minusRes, Bpns, fileName);
                string content = Convert.ToBase64String(output_excel);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new { FileName = fileName, Content = content }; ;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public byte[] MakeExcel(DataTable maindata, List<C140Res> assist, List<C140Res> delpn, string fileName)
        {
            byte[] output_byte;
            string path = System.IO.Directory.GetCurrentDirectory() + $@"\I140Compairson\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (Spire.Xls.Workbook workbook = new Spire.Xls.Workbook())
            {
                Spire.Xls.Worksheet worksheet = workbook.Worksheets[0];
                for (int c = 0; c < maindata.Columns.Count; c++)
                {
                    worksheet.Range[1, c + 1].Text = maindata.Columns[c].ColumnName;
                }

                for (var i = 0; i < maindata.Rows.Count; i++)
                {
                    for (int j = 0; j < maindata.Columns.Count; j++)
                    {
                        string value = maindata.Rows[i][j].ToString();
                        string pn = maindata.Rows[i]["PN"].ToString();
                        string field = maindata.Columns[j].ColumnName;
                        int excelRow = i + 2;
                        int excelColumns = j + 1;
                        if ("PN,TRANID,GROUP".IndexOf(field) == -1)
                        {
                            var wash = assist.Find(t => t.PN == pn && t.SDate == field);
                            if (wash != null)
                            {

                                switch (wash.Flag)
                                {
                                    case "D":
                                        Spire.Xls.ExcelFont dFont = workbook.CreateFont();
                                        dFont.Color = ColorTranslator.FromHtml("#8a0303");
                                        dFont.IsBold = true;
                                        dFont.IsStrikethrough = true;
                                        worksheet.Range[excelRow, excelColumns].RichText.Text = value;
                                        worksheet.Range[excelRow, excelColumns].RichText.SetFont(0, value.Length, dFont);
                                        worksheet.Range[excelRow, excelColumns].Style.Color = ColorTranslator.FromHtml("#f1dacf");
                                        break;
                                    case "C":
                                        Spire.Xls.ExcelFont cFont = workbook.CreateFont();
                                        cFont.Color = Color.Green;
                                        cFont.IsBold = true;


                                        Spire.Xls.ExcelFont lFont = workbook.CreateFont();
                                        lFont.Color = Color.Red;
                                        lFont.IsBold = true;
                                        lFont.IsStrikethrough = true;

                                        value = wash.CQty + "\n" + wash.LQty;
                                        int lStartPos = value.IndexOf("\n") == value.Length ? value.IndexOf("\n") : value.IndexOf("\n") + 1;
                                        worksheet.Range[excelRow, excelColumns].RichText.Text = value;
                                        worksheet.Range[excelRow, excelColumns].RichText.SetFont(0, value.IndexOf("\n"), cFont);
                                        worksheet.Range[excelRow, excelColumns].RichText.SetFont(lStartPos, value.Length, lFont);
                                        worksheet.Range[excelRow, excelColumns].Style.Color = ColorTranslator.FromHtml("#faebdd");
                                        worksheet.Range[excelRow, excelColumns].Style.WrapText = true;
                                        break;
                                    case "A":
                                        worksheet.Range[excelRow, excelColumns].Text = value;
                                        worksheet.Range[excelRow, excelColumns].Style.Font.IsBold = true;
                                        worksheet.Range[excelRow, excelColumns].Style.Color = ColorTranslator.FromHtml("#e4f5fa");
                                        break;
                                    default: break;
                                }
                            }
                            else
                            {
                                worksheet.Range[excelRow, excelColumns].Text = value;
                                worksheet.Range[excelRow, excelColumns].Style.Font.IsBold = true;
                            }

                        }
                        else
                        {
                            var wash = delpn.Find(t => t.PN == pn);
                            if (wash != null)
                            {
                                //刪除綫；背景色#f1dacf；字體顔色"#8a0303"；加粗"bold"                                
                                Spire.Xls.ExcelFont wFont = workbook.CreateFont();
                                wFont.Color = ColorTranslator.FromHtml("#8a0303");
                                wFont.IsBold = true;
                                wFont.IsStrikethrough = true;
                                worksheet.Range[excelRow, excelColumns].RichText.Text = value;
                                worksheet.Range[excelRow, excelColumns].RichText.SetFont(0, value.Length, wFont);
                                worksheet.Range[excelRow, excelColumns].Style.Color = ColorTranslator.FromHtml("#f1dacf");

                            }
                            else
                            {
                                worksheet.Range[excelRow, excelColumns].Text = value;
                                worksheet.Range[excelRow, excelColumns].Style.Font.Color = ColorTranslator.FromHtml("#300c01");
                            }

                        }
                        worksheet.Range[excelRow, excelColumns].RowHeight = 30;
                        worksheet.Range[excelRow, excelColumns].ColumnWidth = 15;
                    }

                }
                workbook.SaveToFile($@"{path}{fileName}", Spire.Xls.ExcelVersion.Version2013);
            }
            using (Spire.Xls.Workbook downfile = new Spire.Xls.Workbook())
            {
                downfile.LoadFromFile($@"{path}{fileName}");
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                downfile.SaveToStream(ms);
                output_byte = ms.ToArray();
            }
            return output_byte;
        }
        public void UploadJnpCsd(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                if (Data["ExcelData"] == null)
                    throw new Exception("Please Input Excel Data");
                string B64 = Data["ExcelData"].ToString();
                var targetlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<R_JNP_CSD_T>>(B64);
                string result = "";
                #region 写入数据库
                //var targetlist = ObjectDataHelper.FromTable<R_JNP_CSD_T>(dt);
                var res = SFCDB.ORM.Ado.UseTran(() =>
                {
                    foreach (var item in targetlist)
                    {
                        if (string.IsNullOrEmpty(item.PONO) || string.IsNullOrEmpty(item.POLINE) || item.CSD == null || Convert.ToDateTime(item.CSD) == DateTime.Now.AddDays(-9000))
                            throw new Exception($@"Data: {item.PONO} {item.POLINE} {item.CSD} cannot be empty,pls check!");
                        item.POLINE = item.POLINE.ToString().PadLeft(5, '0');
                        if (!SFCDB.ORM.Queryable<O_ORDER_MAIN>().Any(t=>t.PONO == item.PONO && t.POLINE == item.POLINE))
                            throw new Exception($@"Data: {item.PONO} {item.POLINE} is not exists,pls check!");
                        item.ID = MesDbBase.GetNewID<R_JNP_CSD_T>(SFCDB.ORM, this.BU);
                        item.CREATETIME = DateTime.Now;
                        item.VALIDFLAG = MesBool.Yes.ExtValue();
                        item.CREATEBY = this.LoginUser.EMP_NO;
                        SFCDB.ORM.Updateable<R_JNP_CSD_T>().SetColumns(t=>new R_JNP_CSD_T() { VALIDFLAG = MesBool.No.ExtValue() })
                        .Where(t => t.PONO == item.PONO && t.POLINE == item.POLINE && t.VALIDFLAG == MesBool.Yes.ExtValue()).ExecuteCommand();
                    }
                    SFCDB.ORM.Insertable(targetlist).ExecuteCommand();
                });

                if (res.IsSuccess)
                    result = "All Upload OK ! ";
                else
                    throw res.ErrorException;
                #endregion
                StationReturn.Message = result;
                StationReturn.Status = StationReturnStatusValue.Pass;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetJnpCsd(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_JNP_CSD_T>().Where(t=>t.VALIDFLAG == MesBool.Yes.ExtValue()).OrderBy(t => t.PONO).OrderBy(t=>t.POLINE).ToList();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

    }
    public class C140Res
    {
        public string PN { get; set; }
        public string SDate { get; set; }
        public string LQty { get; set; }
        public string CQty { get; set; }
        public string Flag { get; set; }
        public string G_code { get; set; }
        public string PRICE { get; set; }
        public string FGOH { get; set; }
        public string SHIPMENT { get; set; }
    }
}
