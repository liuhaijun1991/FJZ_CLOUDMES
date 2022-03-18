using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDataObject.Common;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;
namespace MESDataObject.Module
{
    public class T_R_SN_KP : DataObjectTable
    {
        public T_R_SN_KP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_KP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_KP);
            TableName = "R_SN_KP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }


        /// <summary>
        /// add by hgb 2019.06.21
        /// </summary>
        /// <param name="strsn"></param>
        /// <param name="kp_name"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_SN_KP LoadDataBySnAndKpName(string strsn, string kp_name, OleExec DB)
        {

            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strsn && t.KP_NAME == kp_name).ToList().FirstOrDefault();

        }

        /// <summary>
        /// add by hgb 2019.08.27
        /// </summary>
        /// <param name="strsn"></param>
        /// <param name="kp_name"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List <R_SN_KP> LoadListDataBySnAndKpName(string strsn, string kp_name, OleExec DB)
        { 
            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strsn ).WhereIF(!string.IsNullOrEmpty(kp_name), t => t.KP_NAME == kp_name).OrderBy(t => t.EDIT_EMP, SqlSugar.OrderByType.Desc).ToList();
        }

        public List<R_SN_KP> LoadListDataBySnAndValue(string SN, string VALUE, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.VALUE==VALUE).ToList();
        }

        public List<R_SN_KP> LoadListDataBySnAndPartno(string SN, string PARTNO, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.PARTNO == PARTNO && t.VALID_FLAG==1).ToList();
        }


        /// <summary>
        /// add by hgb 2019.06.21
        /// </summary>
        /// <param name="strsn"></param>
        /// <param name="kp_name"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsExistsBySnAndKpName(string strsn, string kp_name, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_KP>().Any(t => t.SN == strsn && t.KP_NAME == kp_name);
        }


        /// <summary>
        /// HWT檢查KP是否已與其他板子進行綁定
        /// ADD BY HGB 2019.08.07
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="value"></param>
        /// <param name="DB"></param>
        public void IsLinkByOtherSn(string SN,string value, OleExec DB)
        {
            string sql = string.Empty;
            string snrohs = string.Empty;
            string Palletrohs = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                SELECT *
            FROM R_SN_KP
           WHERE SN <> '{SN}'
             AND (VALUE = '{value}' OR
                 VALUE =
                 LEFT('{value}', 10) || 6 || RIGHT('{value}', 5))
             AND ( 
                  substr(SN, 12, 5) <> substr(value, 12, 5))
             AND KP_NAME NOT LIKE '%MAC%'
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                //throw new MESReturnMessage($@"此條碼已與其他板子進行綁定,請確認!{SN},{value}"); ;
                throw new MESReturnMessage($@"pls check!{SN},{value}"); ;
            }
        }

         

        /// <summary>
        /// HWT檢查KP是否已與主板條碼進行綁定
        /// ADD BY HGB 2019.08.07 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="value"></param>
        /// <param name="KP_NAME"></param>
        /// <param name="DB"></param>
        public void IsLinkByThisSn1(string SN, string value,string KP_NAME, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                SELECT *
            FROM R_SN_KP
           WHERE SN = '{SN}'
             AND '{SN}' <> '{value}'
             AND (value = '{value}' OR
                 value =
                 LEFT('{value}', 10) || 6 || RIGHT('{value}', 5) OR
                 value =
                 LEFT('{value}', 10) || 0 || RIGHT('{value}', 5))  
                 AND KP_NAME = '{KP_NAME}' 
                 AND STATION<>'FI'
                 ";
            
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                //throw new MESReturnMessage($@"此條碼沒有與此板進行綁定1,{SN},{value},{KP_NAME}"); ;
                throw new MESReturnMessage($@"pls check!,{SN},{value},{KP_NAME}"); ;
            }
        }

        /// <summary>
        /// HWT檢查KP是否已與主板條碼進行綁定
        /// ADD BY HGB 2019.08.07 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="value"></param>
        /// <param name="KP_NAME"></param>
        /// <param name="DB"></param>
        public void IsLinkByThisSn2(string SN, string value, string KP_NAME, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                 SELECT *
                FROM (SELECT PARENT, son, SN
                        FROM r_relation_data
                       WHERE PARENT ='{SN}'
                         AND '{SN}' <> '{value}'
                         AND (son = '{value}' OR
                             son = LEFT('{value}', 10) || 6 ||
                             RIGHT('{value}', 5) OR
                             son = LEFT('{value}', 10) || 0 ||
                             RIGHT('{value}', 5))
                         AND CATEGORYNAME = '{KP_NAME}'
                         AND STATION<>'FI'   
                      UNION
                      SELECT SN AS PARENT ,
                             value AS SON ,
                             SN
                        FROM R_SN_KP
                       WHERE SN = '{SN}'
                         AND '{SN}'<> '{value}'
                         AND (value = '{value}' OR
                             value = LEFT('{value}', 10) || 6 ||
                             RIGHT('{value}', 5) OR
                             value = LEFT('{value}', 10) || 0 ||
                             RIGHT('{value}', 5))
                         AND KP_NAME = '{KP_NAME}'
                          AND STATION<>'FI' )
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                //throw new MESReturnMessage($@"此條碼沒有與此板進行綁定2,{SN},{value},{KP_NAME}");
                throw new MESReturnMessage($@"pls check,{SN},{value},{KP_NAME}");
            }
        }

        /// <summary>
        /// HWT檢查KP是否已與主板條碼進行綁定
        /// ADD BY HGB 2019.08.07 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="value"></param>
        /// <param name="KP_NAME"></param>
        /// <param name="osn"></param>
        /// <param name="modelno"></param>
        /// <param name="DB"></param>
        public void IsLinkByThisSn3(string SN, string value, string KP_NAME, string osn, string modelno, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                 SELECT *
                FROM (SELECT *
                        FROM R_SN_KP
                       WHERE SN = '{SN}'
                         AND '{SN}' <> '{osn}'
                         AND (value = '{osn}' OR
                             value = '{modelno}')
                         AND kp_name = '{KP_NAME}'
                         AND STATION<>'FI'
                      UNION
                      SELECT *
                        FROM R_SN_KP
                       WHERE SN = '{SN}'
                         AND '{SN}' <> '{value}'
                         AND (value = '{value}' OR
                             value = LEFT('{value}', 10) || 6 ||
                             RIGHT('{value}', 5) OR
                             value = LEFT('{value}', 10) || 0 ||
                             RIGHT('{value}', 5))
                         AND kp_name = '{KP_NAME}'
                         AND STATION<>'FI'  )
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                //throw new MESReturnMessage($@"此條碼沒有與此板進行綁定3,{SN},{value},{KP_NAME}");
                throw new MESReturnMessage($@"pls check,{SN},{value},{KP_NAME}");
            }
        }

         
         

        /// <summary>
        /// HWT檢查KP是否已與主板條碼進行綁定
        /// ADD BY HGB 2019.08.07 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="value"></param>
        /// <param name="KP_NAME"></param>
        /// <param name="DB"></param>
        public void KpLinkcheck(string SN, string value, string KP_NAME, OleExec DB)
        {
            //HWT逆向綁定檢查
            
            string sql = $@" 
                 SELECT OSN, MODELNO,SUBSTR(OSN, -10, 2) AS plant 
            FROM R_EFOX_HISTORY
           WHERE 　 trantype = 'RMA_REPLACE'
             AND remark = 'RMA逆向替換條碼'
                 ";
            DataTable dt = DB.ExecSelect(sql).Tables[0];

            string sql2 = $@" 
                  SELECT *
              FROM c_control a, r_mrb b
             WHERE a.CONTROL_VALUE = b.workorderno
               AND a.control_name = 'TC0010'
               AND b.SN = '{SN}' 
                 ";
            //不卡后五位
            DataTable dt2 = DB.ExecSelect(sql).Tables[0];


            if (dt.Rows.Count > 0)
            {
                string osn = dt.Rows[0]["OSN"].ToString();
                string modelno = dt.Rows[0]["modelno"].ToString();
                string plant= dt.Rows[0]["plant"].ToString();
                if (plant!="DM")
                {
                    if (dt2.Rows.Count == 0)
                    {
                        //HWT檢查KP是否已與主板條碼進行綁定
                        IsLinkByThisSn2(SN, value, KP_NAME, DB);

                    }
                    else
                    {
                        IsLinkByThisSn3(SN, value, KP_NAME, osn, modelno, DB); 
                    }


                }
                else
                {
                    if (dt2.Rows.Count == 0)
                    {
                        IsLinkByThisSn1(SN, value, KP_NAME, DB);
                    }
                    else
                    {
                        IsLinkByThisSn3(SN, value, KP_NAME, osn, modelno, DB); 
                    }
                }


                    
            }
            else
            {
                //HWT檢查KP是否已與主板條碼進行綁定
                IsLinkByThisSn1(SN, value, KP_NAME, DB);
            }
        }

        /// <summary>
        /// 獲取二階KEYPART傳入的KEYPART類型信息
        /// ADD BY HGB 2019.06.24
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="TYPE"></param>
        /// <param name="STATION"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_SN_KP GetSecondKp(string StrSN, string TYPE, string STATION, OleExec DB)
        {
            R_SN_KP r_sn_kp = null;
            Row_R_SN_KP Row_r_sn_kp = (Row_R_SN_KP)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = $@"
             SELECT *
          FROM R_SN_KP
         WHERE SN IN (SELECT VALUE
                                 FROM R_SN_KP
                                WHERE SN = '{StrSN}')
           AND KP_NAME = '{TYPE}'
            
             ";
            if (STATION.Length > 0)
            {
                StrSql = StrSql + $@" AND EVENTPOINT = '{STATION}'";
            }
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                Row_r_sn_kp.loadData(Dt.Rows[0]);
                r_sn_kp = Row_r_sn_kp.GetDataObject();
            }

            return r_sn_kp;
        }

        /// <summary>
        /// 獲取三階KEYPART傳入的KEYPART類型信息
        /// ADD BY HGB 2019.07.16
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="TYPE"></param>
        /// <param name="STATION"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_SN_KP GetThirdKp(string StrSN, string TYPE, string STATION, OleExec DB)
        {
            R_SN_KP r_sn_kp = null;
            Row_R_SN_KP Row_r_sn_kp = (Row_R_SN_KP)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = $@"
             SELECT *
          FROM R_SN_KP
         WHERE SN IN (SELECT VALUE
                                 FROM R_SN_KP
                                WHERE SN in (SELECT VALUE
                                 FROM R_SN_KP
                                WHERE SN = '{StrSN}'))
           AND KP_NAME = '{TYPE}'
            
             ";
            if (STATION.Length > 0)
            {
                StrSql = StrSql + $@" AND EVENTPOINT = '{STATION}'";
            }
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                Row_r_sn_kp.loadData(Dt.Rows[0]);
                r_sn_kp = Row_r_sn_kp.GetDataObject();
            }

            return r_sn_kp;
        }


        public List<R_SN_KP> GetKPRecordBySnIDStation(string SNID, string Station, OleExec SFCDB)
        {
            //string strSql = $@"select * from r_sn_kp r where r_sn_id='{SNID}' and station='{Station}'order by r.itemseq,r.scanseq,r.detailseq  ";
            //List<R_SN_KP> LR = new List<R_SN_KP>();
            //DataSet res = SFCDB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    Row_R_SN_KP R = new Row_R_SN_KP(this.DataInfo);
            //    R.loadData(res.Tables[0].Rows[i]);
            //    LR.Add(R.GetDataObject());
            //}
            //return LR;
            //因為R_SN_KP 记录不应该删除, 重工或者报废打散应该将VALID_FLAG置为0,故只取VALID_FLAG置为1的值
            return SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == SNID && t.STATION == Station && t.VALID_FLAG == 1)
                .OrderBy(t => t.ITEMSEQ).OrderBy(t => t.SCANSEQ).OrderBy(t => t.DETAILSEQ).ToList();
        }
        public List<R_SN_KP> GetAllKPBySnIDStation(string SNID, OleExec SFCDB, string [] stationNmae)
        {
            //string strSql = $@"select * from r_sn_kp r where r_sn_id='{SNID}' and station='{Station}'order by r.itemseq,r.scanseq,r.detailseq  ";
            //List<R_SN_KP> LR = new List<R_SN_KP>();
            //DataSet res = SFCDB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    Row_R_SN_KP R = new Row_R_SN_KP(this.DataInfo);
            //    R.loadData(res.Tables[0].Rows[i]);
            //    LR.Add(R.GetDataObject());
            //}
            //return LR;
            return SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == SNID && stationNmae.Contains(t.STATION) && t.VALID_FLAG==1 )
                .OrderBy(t => t.ITEMSEQ).OrderBy(t => t.SCANSEQ).OrderBy(t => t.DETAILSEQ).ToList();
        }

        public List<string> GetValueListByPN(string PARTNO, OleExec SFCDB)
        {
            return SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.PARTNO == PARTNO&&r.VALID_FLAG==1).Select(t=>t.VALUE).ToList();
        }
        public List<R_SN_KP> GetKPRecordBySnID(string SNID, OleExec SFCDB)
        {
            //string strSql = $@"select * from r_sn_kp r where r_sn_id='{SNID}' order by r.itemseq,r.scanseq,r.detailseq  ";
            //List<R_SN_KP> LR = new List<R_SN_KP>();
            //DataSet res = SFCDB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    Row_R_SN_KP R = new Row_R_SN_KP(this.DataInfo);
            //    R.loadData(res.Tables[0].Rows[i]);
            //    LR.Add(R.GetDataObject());
            //}
            //return LR;
            return SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == SNID && t.VALID_FLAG == 1)
                .OrderBy(t => t.ITEMSEQ).OrderBy(t => t.SCANSEQ).OrderBy(t => t.DETAILSEQ).ToList();
        }
        public string UpdateSNBySnId(string snId,string sn,string user, OleExec SFCDB)
        {
            //string sql = $@" update r_sn_kp set sn='{sn}',edit_time=sysdate,edit_emp='{user}' where r_sn_id='{snId}' ";
            //return SFCDB.ExecSQL(sql);
            return SFCDB.ORM.Updateable<R_SN_KP>().UpdateColumns(t => new R_SN_KP { SN=sn,EDIT_EMP=user }).Where(t => t.R_SN_ID == snId).ExecuteCommand().ToString();
        }
        public int  ReturnUpdateKPSNBySnId(string snId,string[] stationName,string user,OleExec DB) {
            //string sql = $@" update r_sn_kp set VALUE ='',edit_time=sysdate,edit_emp='{user}'  where r_sn_id='{snId}' and station ='{stationName}'";
            //int res = DB.ExecuteNonQuery(sql, CommandType.Text, null);
            //return res;
            DateTime dt = GetDBDateTime(DB);
            return DB.ORM.Updateable<R_SN_KP>().UpdateColumns(t=>new R_SN_KP { VALUE="",EDIT_TIME= dt, EDIT_EMP=user}).Where(t => t.R_SN_ID == snId && stationName.Contains(t.STATION)).ExecuteCommand();
             
        }

        public bool CheckLinkBySNID(string snID, OleExec sfcdb)
        {
            //string sql = $@"select * from r_sn_kp where r_sn_id='{snID}' and value is not null";
            //DataSet ds = sfcdb.RunSelect(sql);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}          

            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.R_SN_ID == snID && !SqlSugar.SqlFunc.IsNullOrEmpty(t.VALUE));
        }

        public bool CheckLinkByValue(string value, OleExec sfcdb)
        {
            //string sql = $@"select * from r_sn_kp where value ='{value}'";
            //DataSet ds = sfcdb.RunSelect(sql);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            //R_SN_KP 记录不应该删除,重工或者报废打散应该将VALID_FLAG置为0
            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.VALUE == value && t.VALID_FLAG == 1);
        }

        /// <summary>
        /// HWT FI工站檢查MAC綁定用 ADD BY HGB 2019.08.09
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public bool CheckHwtMacLinkByValue(string value, OleExec sfcdb)
        { 
            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.VALUE == value&& t.STATION != "FI");
        }

        /// <summary>
        /// HWT FI工站檢查MAC綁定用 ADD BY HGB 2019.08.09
        /// </summary>
        /// <param name="value"></param>
        /// <param name="SFCDB"></param>
        /// <returns></returns>
        public List<R_SN_KP> GetKPRecordByValueHwtFI(string value, OleExec SFCDB)
        {
            return SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == value && t.STATION != "FI").ToList();
        }
        /// <summary>
        /// HWT FI工站檢查MAC綁定用 ADD BY HGB 2019.08.09
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sn"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public bool CheckMacByValueHwtFI(string value, string sn, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.VALUE == value && t.SN == sn && t.STATION != "FI");
        }

        public bool CheckMacByValue(string value, string sn, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.VALUE == value && t.SN == sn);
        }

        public bool CheckVanillaByValue(string value, string sn, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.VALUE == value && t.SN == sn && t.STATION == "ASSY6");
        }
        public bool CheckDuplicateVanillaByValue(string value, string sn, string station, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.VALUE == value && t.SN == sn && t.STATION == station);
        }
        
        //public bool CheckChocoByValue(string value, string sn, OleExec sfcdb)
        //{
        //    return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.VALUE == value && t.SN == sn);
        //}

        public bool CheckScannedMPN(string pn, string sn, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.PARTNO == pn && t.SN == sn && t.SCANTYPE == "MPN" && (t.VALUE != " " || t.VALUE != ""));
        }

        public List<R_SN_KP> GetKPRecordByValue(string value, OleExec SFCDB)
        {
            return SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == value).ToList();
        }

        public List<R_SN_KP> GetScannedMPN(string pn, string sn, OleExec SFCDB)
        {
            List<R_SN_KP> returnKP = new List<R_SN_KP>();
            returnKP = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.PARTNO == pn && t.SN == sn && t.SCANTYPE == "MPN" && (t.VALUE != " " || t.VALUE != "")).OrderBy(t => t.EDIT_TIME).Take(1).ToList();
            return returnKP;
        }

         public List<R_SN_KP> GetSMODSN(string sn, OleExec SFCDB)
        {
            List<R_SN_KP> returnKP = new List<R_SN_KP>();
            returnKP = SFCDB.ORM.Queryable<R_SN_KP>().Where(t =>  t.SN == sn && t.SCANTYPE == "SMSN" && (t.VALUE != " " || t.VALUE != "")).ToList();
            return returnKP;
        }
        // Get broker dimm by james zhu 03/25/2020
        public List<string> GetBroker(string sn, OleExec SFCDB)
        {
            List<string> returnKP = new List<string>();
            returnKP = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn &&t.KP_NAME=="DIMM" &&t.SCANTYPE == "SN" && (t.VALUE != " " || t.VALUE != "") && t.EXKEY2=="BROKER").Select(a=>a.EXKEY2) .ToList();
            return returnKP;
        }

        public List<R_SN_KP> GetChocoSN(string value, string sn, OleExec SFCDB)
        {
            List<R_SN_KP> returnKP = new List<R_SN_KP>();
            returnKP = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn && t.SCANTYPE == "SMSN" && t.VALUE == value).ToList();
            return returnKP;
        }
        public List<R_SN_KP> GetVanillaSN(int seqno, string sn, OleExec SFCDB)
        {
            List<R_SN_KP> returnKP = new List<R_SN_KP>();
            returnKP = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn && t.SCANTYPE == "VANILLA_CHECK" && t.SCANSEQ == seqno).ToList();
            return returnKP;
        }

        public int CountGeneratedKPPN(string pn, string sn, OleExec SFCDB)
        {
            int resutCount = 0;
            resutCount = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.PARTNO == pn && t.SN == sn && t.SCANTYPE.Contains("SN")).Select(t => t.SCANSEQ).ToList().Distinct().Count() ;
            return resutCount;
        }

        public int DeleteBySNID(string snID, OleExec sfcdb)
        {
            //string sql = $@" delete r_sn_kp where r_sn_id='{snID}' ";
            //int res = sfcdb.ExecuteNonQuery(sql, CommandType.Text, null);
            //return res;
            return sfcdb.ORM.Deleteable<R_SN_KP>().Where(t => t.R_SN_ID == snID).ExecuteCommand();
        }

        public bool KpIsLinkBySN(string snID, string kp, OleExec sfcdb)
        {
            //string sql = $@"select * from r_sn_kp where r_sn_id='{snID}' and value ='{kp}'";
            //DataSet ds = sfcdb.RunSelect(sql);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return sfcdb.ORM.Queryable<R_SN_KP>().Any(t => t.R_SN_ID == snID && t.VALUE == kp);
        }

        public int ReplaceSnKP(string newSn, string oldSn, OleExec sfcdb)
        {
            //int result = 0;
            //string sql = string.Empty;
            //if (this.DBType == DB_TYPE_ENUM.Oracle)
            //{
            //    sql = $@"UPDATE R_SN_KEYPART_DETAIL R SET R.SN='{newSn}' WHERE R.SN='{oldSn}'";
            //    result = sfcdb.ExecSqlNoReturn(sql, null);
            //}
            //else
            //{
            //    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
            //    throw new MESReturnMessage(errMsg);
            //}

            //return result;
            sfcdb.ORM.Updateable<R_SN_KP>().UpdateColumns(r => new R_SN_KP { SN = newSn }).Where(r => r.SN == oldSn).ExecuteCommand();
            sfcdb.ORM.Updateable<R_SN_KP>().UpdateColumns(r => new R_SN_KP { VALUE = newSn }).Where(r => r.VALUE == oldSn).ExecuteCommand();//增加舊SN被綁的情況 Edit By ZHB 20200817
            return sfcdb.ORM.Updateable<R_SN_KEYPART_DETAIL>().UpdateColumns(t=>new R_SN_KEYPART_DETAIL { SN=newSn}).Where(t => t.SN == oldSn).ExecuteCommand();

        }
        //add by james zhu 03/25/2020 for OCI broker dimm control
        public bool ISOCISKU(string skuno, OleExec db)
        {
            return db.ORM.Queryable<C_CONTROL>().Any(c => c.CONTROL_NAME == "ISOCISKU" && c.CONTROL_TYPE == "Keypart" && c.CONTROL_VALUE == skuno);
        }
        public bool IsNoCheckLinkScanType(string scanType, OleExec db)
        {
            return db.ORM.Queryable<C_CONTROL>().Any(c => c.CONTROL_NAME == "NoCheckLinkScanType" && c.CONTROL_TYPE == "Keypart" && c.CONTROL_VALUE == scanType);
        }

        public bool IsMacCheckScanType(string scanType, OleExec db)
        {
            return db.ORM.Queryable<C_CONTROL>().Any(c => c.CONTROL_NAME == "IsMacCheckScanType" && c.CONTROL_TYPE == "Keypart" && c.CONTROL_VALUE == scanType);
        }

        public bool IsVanillaCheckScanType(string scanType, OleExec db)
        {
            return db.ORM.Queryable<C_CONTROL>().Any(c => c.CONTROL_NAME == "IsVanillaCheckScanType" && c.CONTROL_TYPE == "Keypart" && c.CONTROL_VALUE == scanType);
        }
        public bool IsChocoCheckScanType(string scanType, OleExec db)
        {
            return db.ORM.Queryable<C_CONTROL>().Any(c => c.CONTROL_NAME == "IsChocoCheckScanType" && c.CONTROL_TYPE == "Keypart" && c.CONTROL_VALUE == scanType);
        }
        
        public bool IsMacScanType(string scanType, OleExec db)
        {
            return db.ORM.Queryable<C_CONTROL>().Any(c => c.CONTROL_NAME == "IsMacScanType" && c.CONTROL_TYPE == "Keypart" && c.CONTROL_VALUE == scanType);
        }
        //Add by James Zhu 03042019 for Component lock function
        public bool IsKeypartLock(string sn, OleExec db)
        {
            return db.ORM.Queryable<R_SN_LOCK>().Any(c => c.LOCK_STATUS == "1" && c.SN == sn);
        }

        //Add by James Zhu 09072019 for ODA_HA Chasis check function
        public bool IsCHASSISCheckScan(string value, string sn, OleExec db)
        {
            return db.ORM.Queryable<R_SN>().Any(c => c.COMPLETED_FLAG != "1" && c.SN == sn && c.SN== value );
        }
        public bool IsCHASSISCheckScanType(string scanType, OleExec db)
        {
            return db.ORM.Queryable<C_CONTROL>().Any(c => c.CONTROL_NAME == "IsCHASSISCheckScanType" && c.CONTROL_TYPE == "Keypart" && c.CONTROL_VALUE == scanType);
        }
        // End by James Zhu 09072019 for ODHA Chassis check function

        public List<R_SN_KP> GetSystemSN(string sn, OleExec SFCDB)
        {
            List<R_SN_KP> returnKP = new List<R_SN_KP>();
            returnKP = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn && t.SCANTYPE == "SystemSN").ToList();
            return returnKP;
        }


        public List<R_SN> GetKeypartSnListByWo(string wo, OleExec DB)
        {
            var list = DB.ORM.Queryable<R_SN, R_SN_KP, R_SN>((rsn1, rsk, rsn2) => rsn1.SN == rsk.VALUE && rsk.SN == rsn2.SN)
                .Where((rsn1, rsk, rsn2) => rsn1.VALID_FLAG == "1" && rsn2.VALID_FLAG == "1" && rsn2.WORKORDERNO == wo)
                .Select((rsn1, rsk, rsn2) => new R_SN
                {
                    ID = rsn1.ID,
                    SN = rsn1.SN,
                    SKUNO = rsn1.SKUNO,
                    WORKORDERNO = rsn1.WORKORDERNO,
                    PLANT = rsn1.PLANT,
                    ROUTE_ID = rsn1.ROUTE_ID,
                    STARTED_FLAG = rsn1.STARTED_FLAG,
                    START_TIME = rsn1.START_TIME,
                    PACKED_FLAG = rsn1.PACKED_FLAG,
                    PACKDATE = rsn1.PACKDATE,
                    COMPLETED_FLAG = rsn1.COMPLETED_FLAG,
                    COMPLETED_TIME = rsn1.COMPLETED_TIME,
                    SHIPPED_FLAG = rsn1.SHIPPED_FLAG,
                    SHIPDATE = rsn1.SHIPDATE,
                    REPAIR_FAILED_FLAG = rsn1.REPAIR_FAILED_FLAG,
                    CURRENT_STATION = rsn1.CURRENT_STATION,
                    NEXT_STATION = rsn1.NEXT_STATION,
                    KP_LIST_ID = rsn1.KP_LIST_ID,
                    PO_NO = rsn1.PO_NO,
                    CUST_ORDER_NO = rsn1.CUST_ORDER_NO,
                    CUST_PN = rsn1.CUST_PN,
                    BOXSN = rsn1.BOXSN,
                    SCRAPED_FLAG = rsn1.SCRAPED_FLAG,
                    SCRAPED_TIME = rsn1.SCRAPED_TIME,
                    PRODUCT_STATUS = rsn1.PRODUCT_STATUS,
                    REWORK_COUNT = rsn1.REWORK_COUNT,
                    VALID_FLAG = rsn1.VALID_FLAG,
                    EDIT_EMP = rsn1.EDIT_EMP,
                    EDIT_TIME = rsn1.EDIT_TIME
                }).ToList();
            return list;
        }

        public List<R_SN> GetSysKeypartValueList(string snID, string[] stationName, OleExec db)
        {
            var list = db.ORM.Queryable<R_SN, R_SN_KP>((rsn, rsnkp) => rsn.SN == rsnkp.VALUE)
                .Where((rsn, rsnkp) => rsn.VALID_FLAG == "1" && rsnkp.R_SN_ID == snID && stationName.Contains(rsnkp.STATION))
                .Select((rsn, rsnkp) => new R_SN
                {
                    ID = rsn.ID,
                    SN = rsn.SN,
                    SKUNO = rsn.SKUNO,
                    WORKORDERNO = rsn.WORKORDERNO,
                    PLANT = rsn.PLANT,
                    ROUTE_ID = rsn.ROUTE_ID,
                    STARTED_FLAG = rsn.STARTED_FLAG,
                    START_TIME = rsn.START_TIME,
                    PACKED_FLAG = rsn.PACKED_FLAG,
                    PACKDATE = rsn.PACKDATE,
                    COMPLETED_FLAG = rsn.COMPLETED_FLAG,
                    COMPLETED_TIME = rsn.COMPLETED_TIME,
                    SHIPPED_FLAG = rsn.SHIPPED_FLAG,
                    SHIPDATE = rsn.SHIPDATE,
                    REPAIR_FAILED_FLAG = rsn.REPAIR_FAILED_FLAG,
                    CURRENT_STATION = rsn.CURRENT_STATION,
                    NEXT_STATION = rsn.NEXT_STATION,
                    KP_LIST_ID = rsn.KP_LIST_ID,
                    PO_NO = rsn.PO_NO,
                    CUST_ORDER_NO = rsn.CUST_ORDER_NO,
                    CUST_PN = rsn.CUST_PN,
                    BOXSN = rsn.BOXSN,
                    SCRAPED_FLAG = rsn.SCRAPED_FLAG,
                    SCRAPED_TIME = rsn.SCRAPED_TIME,
                    PRODUCT_STATUS = rsn.PRODUCT_STATUS,
                    REWORK_COUNT = rsn.REWORK_COUNT,
                    VALID_FLAG = rsn.VALID_FLAG,
                    EDIT_EMP = rsn.EDIT_EMP,
                    EDIT_TIME = rsn.EDIT_TIME
                }).ToList();
            return list;
        }

        /// <summary>
        /// 通過SN update 新生成的SN ID
        /// </summary>
        /// <param name="snId"></param>
        /// <param name="sn"></param>
        /// <param name="user"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateSNBySn(string snId, string sn, string user, OleExec DB)
        {
            string strSql = $@" update r_sn_kp set R_SN_ID='{snId}',edit_time=sysdate,edit_emp='{user}' where SN ='{sn}' AND R_SN_ID IS NULL AND VALID_FLAG='1'";
            int res = DB.ExecuteNonQuery(strSql, CommandType.Text, null);
            return res;
        }

        /// <summary>
        /// WZW 2018/08/23 修改R_SN_KP valid字段
        /// </summary>
        /// <param name="Sn"></param>
        /// <param name="Valid"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public int UpdateSNKPValid(string Sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                string strSql = $@"UPDATE R_SN_KP SET VALID_FLAG='0' WHERE SN='{Sn}' AND VALID_FLAG='1'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return result;
        }

        public List<R_SN_KP> GetKPRecordBySnStation(string SN, string Station, OleExec SFCDB)
        {
            string strSql = $@"select * from r_sn_kp r where sn='{SN}' and station='{Station}' AND VALID_FLAG='1' order by r.itemseq,r.scanseq,r.detailseq  ";
            List<R_SN_KP> LR = new List<R_SN_KP>();
            DataSet res = SFCDB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_SN_KP R = new Row_R_SN_KP(this.DataInfo);
                R.loadData(res.Tables[0].Rows[i]);
                LR.Add(R.GetDataObject());
            }
            return LR;
        }

        /// <summary>
        /// WZW 查询SN Value Valid
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Valid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_SN_KP> GetKPListBYSNCSN(string SN, string Value, double? Valid, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.VALUE == Value && t.VALID_FLAG == Valid).ToList();
        }

        /// <summary>
        /// WZW 查询SN Value Valid
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Valid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_SN_KP> GetKPListBYSN(string SN, double? Valid, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.VALID_FLAG == Valid).ToList();
        }


        /// <summary>
        /// ADD BY HGB 2019.08.02
        /// 檢查SN是否符合日本合同:無維修，無ICT、FT失敗記錄
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="desc"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void TCJPSNSCAN(string SN,string desc,  OleExec DB)
        {
            if (DB.ORM.Queryable<R_REPAIR_MAIN>().Any(t => t.SN == SN))
            {
                throw new Exception($@"{desc}SN有維修記錄，不可用於日本合同，有疑問請聯繫劉貴遠！" + SN);
            }

            if (DB.ORM.Queryable<R_TEST_RECORD>().Any(t => t.SN == SN && t.TESTATION == "ICT" && t.STATE == "FAIL"))
            {
                throw new Exception($@"{desc}SN有ICT失敗記錄，不可用於日本合同，有疑問請聯繫劉貴遠！" + SN);
            }

            if (DB.ORM.Queryable<R_TEST_RECORD>().Any(t => t.SN == SN && t.TESTATION.Substring(0,2) == "FT" && t.STATE == "FAIL"))
            {
                throw new Exception($@"{desc}SN有FT失敗記錄，不可用於日本合同，有疑問請聯繫劉貴遠！"+SN);
            }

             
        }

        /// <summary>
        /// ADD BY HGB 2019.08.06
        /// 檢查華為鎖定
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="desc"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void CheckLockByHuawei(string SN, string BOXSN, OleExec DB)
        {
            string linkSQL = string.Empty;
            if (BOXSN!=null  )
            {
                if (BOXSN.Length > 0)
                { 
                    linkSQL = "  OR barcode = '{BOXSN}'";
                } 
            }
             

            string strSql = $@"SELECT * 
              FROM t_btp_locked_sn_inface@hwems 
             WHERE (barcode = '{SN}'  {linkSQL} )
               AND lock_flag = 'Y'";

            DataTable res = DB.ExecSelect(strSql).Tables[0];
           
            if (res.Rows.Count>0)
            {
                  strSql = $@" SELECT *
              FROM (SELECT *
                      FROM (SELECT *
                              FROM t_btp_locked_sn_inface@hwems 
                             WHERE (barcode = '{SN}'  {linkSQL} )
                             ORDER BY updated_date DESC)
                     WHERE rownum = 1)
             WHERE lock_flag = 'N'";

                res = DB.ExecSelect(strSql).Tables[0];
                if (res.Rows.Count == 0)
                {
                    throw new Exception($@"此SN已被HW鎖定,請聯繫QA人員！" + SN);
                }
            }


        }

        public List<R_SN_KP> GetBYSNSCANTYPE(string KPSN, double? Valid, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == KPSN && t.VALID_FLAG == Valid).ToList();
        }
        public List<R_SN_KP> GetListBYSN(string SN, string SCANTYPE, double? Valid, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.SCANTYPE == SCANTYPE && t.VALID_FLAG == Valid).ToList();
        }
        public int UpdateRSNKPSN(string NewSN, string OldSN, double? Valid, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN_KP>().UpdateColumns(t => t.SN == NewSN).Where(t => t.SN == OldSN && t.VALID_FLAG == Valid).ExecuteCommand();
        }
        public int InsertRSNKPSN(R_SN_KP InQuery, OleExec DB)
        {
            return DB.ORM.Insertable<R_SN_KP>(InQuery).ExecuteCommand();
        }
        public R_SN_KP GetRowByValueAndValidFlag(string Value, Double ValidFlag, OleExec db)
        {
            return db.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == Value && t.VALID_FLAG == ValidFlag).ToList().FirstOrDefault();
        }
        public List<R_SN_KP> GetSNSCANTYPEAndValidFlag(string SN, string SCANTYPE, Double? ValidFlag, OleExec db)
        {
            return db.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.VALID_FLAG == ValidFlag && t.SCANTYPE == SCANTYPE).ToList();
        }
        public List<R_SN_KP> GetSNKPSNBYSN(string SN, string SHIPPINGFLAG, string STOCKSTATUS, OleExec DB)
        {
            string ORSN = "B9C" + SN.Substring(SN.Length - 8, 8);
            string strSql = $@"SELECT * FROM R_SN_KP WHERE VALUE = (
SELECT SN FROM R_SN WHERE (SN='{SN}'OR SN='{ORSN}') AND STOCK_STATUS='{STOCKSTATUS}' AND SHIPPED_FLAG='{SHIPPINGFLAG}' ) ";
            List<R_SN_KP> LR = new List<R_SN_KP>();
            DataSet res = DB.RunSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                {
                    Row_R_SN_KP R = new Row_R_SN_KP(this.DataInfo);
                    R.loadData(res.Tables[0].Rows[i]);
                    LR.Add(R.GetDataObject());
                }
            }
            return LR;
        }
        public List<R_SN_KP> GetBySN(string SN, OleExec db)
        {
            string ORSN = "B9C" + SN.Substring(SN.Length - 8, 8);
            return db.ORM.Queryable<R_SN_KP>().Where(t => (t.VALUE == SN || t.VALUE == ORSN) && t.VALID_FLAG == 1).ToList();
        }
        public List<R_SN_KP> GetSNBySN(string SN, OleExec db)
        {
            string ORSN = "B9C" + SN.Substring(SN.Length - 8, 8);
            return db.ORM.Queryable<R_SN_KP>().Where(t => (t.VALUE == SN || t.VALUE == ORSN) || (t.SN == SN || t.SN == ORSN) && t.VALID_FLAG == 1).ToList();
        }
        public List<R_SN_KP> GetSNSKUBySN(string SN, string category, string valid, string cancel, OleExec db)
        {
            return db.ORM.Queryable<R_SN_KP, C_SKU>((p1, p2) => p1.PARTNO == p2.SKUNO).Where((p1, p2) => p1.SN == SN).ToList();
        }
        public int DeleteSN(string SN, OleExec DB)
        {
            return DB.ORM.Deleteable<R_SN_KP>().Where(t => t.SN == SN).ExecuteCommand();
        }
        public int UpdateSNCOPYSNVALIDFLAG(R_SN_KP RSNKP, string SN, double? Valid, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN_KP>(RSNKP).Where(t => t.VALID_FLAG == Valid && t.SN == SN).ExecuteCommand();
        }
        public int UpdateSNByValueEmpty(string SN, double? Valid, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN_KP>().UpdateColumns(t => t.VALUE == "").Where(t => t.VALID_FLAG == Valid && t.SN == SN).ExecuteCommand();
        }
        public List<R_SN_KP> GetSNPCBASNSKUVER(string SKUNO, string workorder, OleExec SFCDB)
        {
            string strSql = $@"select * from r_sn_kp r where exists(select * from r_sn b where WORKORDERNO='{workorder}'and r.sn=b.sn)and PARTNO={SKUNO} AND VALID_FLAG='1' ";
            List<R_SN_KP> LR = new List<R_SN_KP>();
            DataSet res = SFCDB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_SN_KP R = new Row_R_SN_KP(this.DataInfo);
                R.loadData(res.Tables[0].Rows[i]);
                LR.Add(R.GetDataObject());
            }
            return LR;
        }

        public List<R_SN_KP> GetListByValueAndValidFlag(string scanValue, string validFlag, OleExec SFCDB)
        {
            return SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.VALUE == scanValue && r.VALID_FLAG == 1 && r.SCANTYPE != "KEEP_SN").ToList();
        }

        public List<R_SN_KP> GetSOLTScanDataByTopLevel(OleExec SFCDB, string sn)
        {
            string strSql = $@"select value from r_sn_kp where sn = '{sn}' and kp_name in (select control_value from c_control where control_name = 'SOLTTestRecordChk') and scantype in ('SN','L10SN') ";
            List<R_SN_KP> LR = new List<R_SN_KP>();
            DataSet res = SFCDB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_SN_KP R = new Row_R_SN_KP(this.DataInfo);
                R.loadData(res.Tables[0].Rows[i]);
                LR.Add(R.GetDataObject());
            }
            return LR;
        }
        public int UpdateKPSN(string NewSN, string OldSN, double? Valid, OleExec DB)
        {
            return DB.ORM.Updateable<R_SN_KP>().UpdateColumns(t => t.VALUE == NewSN).Where(t => t.VALUE == OldSN && t.VALID_FLAG == Valid).ExecuteCommand();
        }

        public int ReworkUpdateKP(string kp_id, string user, OleExec DB)
        {
            DateTime dt = GetDBDateTime(DB);
           
            Row_R_SN_KP kp = (Row_R_SN_KP)GetObjByID(kp_id, DB);
            R_SN_KP kpObj = kp.GetDataObject();
            if (kpObj == null)
            {
                throw new Exception("This KP not exist!");
            }
            //string rw_sn = "RW" + kpObj.SN;
            //string rw_sn_id = "RW" + kpObj.R_SN_ID;
            //string rw_value = "RW" + kpObj.VALUE;
            //return DB.ORM.Updateable<R_SN_KP>().UpdateColumns(t => new R_SN_KP
            //{
            //    R_SN_ID = rw_sn_id,
            //    SN = rw_sn,
            //    VALUE = rw_value,
            //    EDIT_TIME = dt,
            //    EDIT_EMP = user
            //}).Where(t => t.ID == kp_id).ExecuteCommand();

            //R_SN_KP 记录不应该删除,重工或者报废打散应该将VALID_FLAG置为0
            kpObj.VALID_FLAG = 0;
            return DB.ORM.Updateable(kpObj).ExecuteCommand();
            //return DB.ORM.Updateable<R_SN_KP>().UpdateColumns(t => new R_SN_KP { VALID_FLAG = 0 }).Where(t => t.ID == kp_id).ExecuteCommand();
        }

        public int Save(OleExec DB,R_SN_KP snkp)
        {
            return DB.ORM.Insertable<R_SN_KP>(snkp).ExecuteCommand();
        }

        public R_SN_KP GetObjByID(OleExec SFCDB, string id)
        {
            return SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.ID == id).ToList().FirstOrDefault();
        }

        public int UpdateObj(OleExec SFCDB, R_SN_KP kpObj)
        {
            return SFCDB.ORM.Updateable<R_SN_KP>(kpObj).Where(r => r.ID == kpObj.ID).ExecuteCommand();
        }

        public List<R_SN_KP> GetLastLoadingRegularSnKp(string skuno,OleExec SFCDB)
        {
            string lastSnId = SFCDB.ORM.Queryable<R_SN,R_WO_BASE>((a,b)=>a.WORKORDERNO==b.WORKORDERNO)
                .Where((a,b) => a.SKUNO == skuno && a.VALID_FLAG == "1"&&b.WO_TYPE== "REGULAR")
                .OrderBy(a => a.START_TIME, SqlSugar.OrderByType.Desc)
                .Select(a=>a.ID)
                .First();
            return SFCDB.ORM.Queryable<R_SN_KP>()
                .Where(it => it.R_SN_ID == lastSnId && it.VALID_FLAG == 1)
                .ToList();
        }

        public R_SN_KP GetSISn(string PCBAsn, OleExec DB)
        {

            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == PCBAsn && t.STATION == "SILOADING").ToList().FirstOrDefault();

        }

        public R_SN_KP GetPcbaSn(string strsn, OleExec DB)
        {

            return DB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strsn && t.STATION == "SILOADING").ToList().FirstOrDefault();

        }
        public void GetSnKP(MESDBHelper.OleExec DB, string sn,List<R_SN_KP> list)
        {           
            var listtemp = GetKPListBYSN(sn, 1, DB);
            if (listtemp.Count > 0)
            {
                list.AddRange(listtemp);
                foreach (var kp in listtemp)
                {
                    if (kp.VALUE != kp.SN)
                    {
                        GetSnKP(DB, kp.VALUE, list);
                    }
                }
            }
        }
    }
    public class Row_R_SN_KP : DataObjectBase
    {
        public Row_R_SN_KP(DataObjectInfo info) : base(info)
        {
            
        }

        
        public R_SN_KP GetDataObject()
        {
            R_SN_KP DataObject = new R_SN_KP();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.VALUE = this.VALUE;
            DataObject.PARTNO = this.PARTNO;
            DataObject.KP_NAME = this.KP_NAME;
            DataObject.MPN = this.MPN;
            DataObject.SCANTYPE = this.SCANTYPE;
            DataObject.ITEMSEQ = this.ITEMSEQ;
            DataObject.SCANSEQ = this.SCANSEQ;
            DataObject.DETAILSEQ = this.DETAILSEQ;
            DataObject.STATION = this.STATION;
            DataObject.REGEX = this.REGEX;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.EXKEY1 = this.EXKEY1;
            DataObject.EXVALUE1 = this.EXVALUE1;
            DataObject.EXKEY2 = this.EXKEY2;
            DataObject.EXVALUE2 = this.EXVALUE2;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.LOCATION = this.LOCATION;
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
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string KP_NAME
        {
            get
            {
                return (string)this["KP_NAME"];
            }
            set
            {
                this["KP_NAME"] = value;
            }
        }
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
            }
        }
        public string SCANTYPE
        {
            get
            {
                return (string)this["SCANTYPE"];
            }
            set
            {
                this["SCANTYPE"] = value;
            }
        }
        public double? ITEMSEQ
        {
            get
            {
                return (double?)this["ITEMSEQ"];
            }
            set
            {
                this["ITEMSEQ"] = value;
            }
        }
        public double? SCANSEQ
        {
            get
            {
                return (double?)this["SCANSEQ"];
            }
            set
            {
                this["SCANSEQ"] = value;
            }
        }
        public double? DETAILSEQ
        {
            get
            {
                return (double?)this["DETAILSEQ"];
            }
            set
            {
                this["DETAILSEQ"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string REGEX
        {
            get
            {
                return (string)this["REGEX"];
            }
            set
            {
                this["REGEX"] = value;
            }
        }
        public double? VALID_FLAG
        {
            get
            {
                return (double?)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string EXKEY1
        {
            get
            {
                return (string)this["EXKEY1"];
            }
            set
            {
                this["EXKEY1"] = value;
            }
        }
        public string EXVALUE1
        {
            get
            {
                return (string)this["EXVALUE1"];
            }
            set
            {
                this["EXVALUE1"] = value;
            }
        }
        public string EXKEY2
        {
            get
            {
                return (string)this["EXKEY2"];
            }
            set
            {
                this["EXKEY2"] = value;
            }
        }
        public string EXVALUE2
        {
            get
            {
                return (string)this["EXVALUE2"];
            }
            set
            {
                this["EXVALUE2"] = value;
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
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
    }
    public class R_SN_KP
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID{get;set;}
        public string R_SN_ID{get;set;}
        public string SN{get;set;}
        public string VALUE{get;set;}
        public string PARTNO{get;set;}
        public string KP_NAME{get;set;}
        public string MPN{get;set;}
        public string SCANTYPE{get;set;}
        public double? ITEMSEQ{get;set;}
        public double? SCANSEQ{get;set;}
        public double? DETAILSEQ{get;set;}
        public string STATION{get;set;}
        public string REGEX{get;set; }
        public string LOCATION { get; set; }
        public double? VALID_FLAG{get;set;}
        public string EXKEY1{get;set;}
        public string EXVALUE1{get;set;}
        public string EXKEY2{get;set;}
        public string EXVALUE2{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
    public enum ENUM_R_SN_KP
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
        VALID_FLAG_FALSE
    }
}