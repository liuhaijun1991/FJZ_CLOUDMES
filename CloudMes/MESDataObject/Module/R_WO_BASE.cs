using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_R_WO_BASE : DataObjectTable
    {
        public T_R_WO_BASE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_BASE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_BASE);
            TableName = "R_WO_BASE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public Row_R_WO_BASE GetWo(string _WO, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from r_wo_base where workorderno='{_WO.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "WorkOrder:" + _WO });
                    throw new MESReturnMessage(errMsg);
                }
                Row_R_WO_BASE R = (Row_R_WO_BASE)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        public R_WO_BASE GetWoByWoNo(string _WO, OleExec DB)
        {
            //string strsql = "";
            //if (DBType == DB_TYPE_ENUM.Oracle)
            //{
            //    strsql = $@"select * from r_wo_base where workorderno='{_WO.Replace("'", "''")}'";
            //    DataTable result = DB.ExecuteDataTable(strsql, CommandType.Text);
            //    if (result.Rows.Count > 0)
            //    {
            //        return CreateLanguageClass(result.Rows[0]);
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //else
            //{
            //    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
            //    throw new MESReturnMessage(errMsg);
            //}
            return DB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == _WO.Replace("'", "''")).ToList().FirstOrDefault();
        }

        public R_WO_BASE GetMrbWoByRohs(string Sn, OleExec DB)
        {
            //string strsql = "";
            //if (DBType == DB_TYPE_ENUM.Oracle)
            //{
            //    strsql = $@"SELECT b.rohs
            //                FROM r_task_order_sn a, r_wo_base b
            //               WHERE a.wo = b.workorderno
            //                 AND a.sn ='{Sn}'";
            //    DataTable result = DB.ExecuteDataTable(strsql, CommandType.Text);
            //    if (result.Rows.Count > 0)
            //    {
            //        return CreateLanguageClass(result.Rows[0]);
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //else
            //{
            //    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
            //    throw new MESReturnMessage(errMsg);
            //}

            return DB.ORM.Queryable<r_task_order_sn, R_WO_BASE>((rtos, rwb) => rtos.WO == rwb.WORKORDERNO).Where((rtos, rwb) => rtos.SN == Sn)
                .Select((rtos, rwb) => rwb).ToList().FirstOrDefault();

        }

        public Row_R_WO_BASE LoadWorkorder(string _WO, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select * from r_wo_base where workorderno='{_WO.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    return null;
                }
                Row_R_WO_BASE R = (Row_R_WO_BASE)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 20190627 Patty added: Get WO where all SN in either SILOADING or JOBFINISH, allow user to close.
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_WO_BASE> GetWOtoClose(OleExec DB)
        {            
            List<R_WO_BASE> WOList = new List<R_WO_BASE>();
            string sql = "";
            DataTable dt = null;
            try
            {
                sql = $@"select * from R_WO_BASE W  where W.CLOSED_FLAG = 0 and (INPUT_QTY = 0 
or W.workorderno in (select workorderno from R_SN where NEXT_STATION in ('ASSY1','SILOADING') group by workorderno, next_station having count(*) = W.WORKORDER_QTY))  order by workorderno";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    WOList.Add(CreateLanguageClass(dr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return WOList;
        }

        public List<R_WO_BASE> GetWOVersion(String PackNo,OleExec DB)
        {
            List<R_WO_BASE> WOList = new List<R_WO_BASE>();
            string sql = "";
            DataTable dt = null;
            try
            {
                sql = $@"SELECT DISTINCT SKU_VER FROM R_WO_BASE WHERE WORKORDERNO IN(
                        SELECT WORKORDERNO FROM R_SN WHERE ID IN(
                        SELECT SN_ID FROM R_SN_PACKING WHERE PACK_ID IN(
                        SELECT ID FROM R_PACKING WHERE PARENT_PACK_ID IN(
                        select ID from R_PACKING where PACK_NO ='"+ PackNo + "'))))";
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    WOList.Add(CreateLanguageClass(dr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return WOList;
        }
        

        public List<R_WO_BASE> GetWOtoCloseorOpen(string wo, string closed, OleExec DB)
        {
            List<R_WO_BASE> WOList = new List<R_WO_BASE>();
            string sql = "";
            DataTable dt = null;
            try
            {
                if (closed == "False")
                {
                    sql = $@"select * from R_WO_BASE W  where W.workorderno like '%{wo}%' and W.CLOSED_FLAG = 0 and (INPUT_QTY = 0 
or W.workorderno in (select workorderno from R_SN where NEXT_STATION in ('ASSY1','SILOADING') group by workorderno, next_station having count(*) = W.WORKORDER_QTY))  order by workorderno ";
                }
                else
                {
                    sql = $@"select * from R_WO_BASE where workorderno like '%{wo}%' and CLOSED_FLAG = 1 and FINISHED_QTY <> WORKORDER_QTY  order by workorderno ";
                }
               
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    WOList.Add(CreateLanguageClass(dr));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return WOList;
        }


        /// <summary>
        /// 給工單的投入數增加指定值
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="count"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string AddCountToWo(string wo, double count, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            Row_R_WO_BASE row = null;
            lock (wo)
            {
                if (this.DBType == DB_TYPE_ENUM.Oracle)
                {
                    //Modify by LLF 2018-04-10 for 同時幾台電腦在掃描時會存在數量更新不准的問題
                    //Modify by ZGJ 2018-04-26 for 因為調用該方法的地方太多，很多 addCount 的邏輯不一定普遍使用，所以 INPUT_QTY 應該有個上限，避免超過 WORKORDER_QTY
                    row = GetWo(wo, DB);
                    row.INPUT_QTY += count;
                    if (row.INPUT_QTY > row.WORKORDER_QTY)
                    {
                        //row.INPUT_QTY = row.WORKORDER_QTY;
                        throw new MESReturnMessage($@" {wo} Already Full!");
                    }
                    sql = row.GetUpdateString(this.DBType);
                    result = DB.ExecSQL(sql);
                    //string strSql = $@"update r_wo_base set input_qty=input_qty+{count} where workorderno='{wo}'";
                    //result = DB.ExecSQL(strSql);

                    return result;
                }
                else
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                    throw new MESReturnMessage(errMsg);
                }
            }
        }

        /// <summary>
        /// 關閉工單
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string CloseOrOpenWorkOrder(string wo, OleExec DB)
        {
            DB.ORM.Updateable<R_WO_BASE>().UpdateColumns(t=>t.CLOSED_FLAG==SqlSugar.SqlFunc.IIF(t.CLOSED_FLAG=="1","0","1")).Where(t => t.WORKORDERNO == wo).ExecuteCommand();
            string result = DB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == wo).Select(t => t.CLOSED_FLAG).ToList().FirstOrDefault();
            return result.Equals("1") ? "Close" : "Open";
        }

        /// <summary>
        /// 更新完工數量以及判斷是否應該關結工單
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="count"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateFinishQty(string wo, double count, OleExec DB)
        {
            int result = 0;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo}' AND WORKORDER_QTY-FINISHED_QTY>={count}";
                dt = DB.ExecSelect(sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    sql = $@"UPDATE R_WO_BASE SET FINISHED_QTY=CASE WHEN (FINISHED_QTY IS NULL) THEN {count} ELSE FINISHED_QTY+{count} END,
                                EDIT_TIME=to_date( '{GetDBDateTime(DB).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss') WHERE WORKORDERNO='{wo}'";
                    result = DB.ExecSqlNoReturn(sql, null);
                    //20190530 FTX added: Why 陳華材 removed this?? anyway I just put it back.
                    sql = $@"UPDATE R_WO_BASE SET CLOSED_FLAG='1',CLOSE_DATE=to_date( '{GetDBDateTime(DB).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss') WHERE WORKORDERNO='{wo}' 
                                AND FINISHED_QTY>=WORKORDER_QTY";
                    DB.ExecSqlNoReturn(sql, null);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        /// <summary>
        /// 依據10進制或34進制獲取下一個流水碼
        /// </summary>
        /// <param name="CurrentSN"></param>
        /// <param name="DecimalType"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string Get_NextSN(string strCurrentSN, string strDecimalType)
        {
            string strNextSN = strCurrentSN;
            string strTempSN = "";
            string strTempCH = "";
            int intAdd = 1;
            int intIndex = 0;
            int intCount = 0;
            string strBaseDecimal = "0123456789";

            if (strDecimalType == "10H")
            {
                strBaseDecimal = "0123456789";
            }
            else if (strDecimalType == "34H")
            {
                strBaseDecimal = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
            }
            else if (strDecimalType == "36H")
            {
                strBaseDecimal = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            else if (strDecimalType == "16H")
            {
                strBaseDecimal = "0123456789ABCDEF";
            }


            intIndex = strNextSN.Length;
            while (intAdd == 1)
            {
                intAdd = 0;
                strTempCH = strNextSN.Substring(intIndex - 1, 1);
                intCount = strBaseDecimal.IndexOf(strTempCH);
                if (intCount == strBaseDecimal.Length - 1)
                {
                    intAdd = 1;
                    strTempSN = "0" + strTempSN;
                }
                else
                {
                    strTempSN = strBaseDecimal.Substring(intCount + 1, 1) + strTempSN;
                }
                intIndex = intIndex - 1;
            }

            intIndex = strNextSN.Length - strTempSN.Length;
            strNextSN = strNextSN.Substring(0, intIndex) + strTempSN;
            return strNextSN;
        }

        public bool CheckDataExist(string wo_no,string skuno, OleExec DB)
        {
            //bool res = false;
            //string sql = string.Empty;
            //DataTable dt = new DataTable();
            //sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo_no}' AND SKUNO='{skuno}'";
            //dt = DB.ExecSelect(sql).Tables[0];
            //if (dt.Rows.Count == 0)
            //{
            //    res = true;
            //}
            //return res;
            return DB.ORM.Queryable<R_WO_BASE>().Any(t => t.WORKORDERNO == wo_no && t.SKUNO == skuno);
        }

        public bool CheckDataExist(string wo_no, OleExec DB)
        {
            //bool res = false;
            //string sql = string.Empty;
            //DataTable dt = new DataTable();
            //sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo_no}'";
            //dt = DB.ExecSelect(sql).Tables[0];
            //if (dt.Rows.Count == 0)
            //{
            //    res = true;
            //}
            //return res;
            return DB.ORM.Queryable<R_WO_BASE>().Any(t => t.WORKORDERNO == wo_no);
        }

        /// <summary>
        /// 工單是否存在
        /// ADD BY HGB 2019.08.22
        /// </summary>
        /// <param name="wo_no"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsExist(string wo_no, OleExec DB)
        {
            //bool res = false;
            //string sql = string.Empty;
            //DataTable dt = new DataTable();
            //sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo_no}'";
            //dt = DB.ExecSelect(sql).Tables[0];
            //if (dt.Rows.Count > 0)
            //{
            //    res = true;
            //}
            //return res;
            return CheckDataExist(wo_no, DB);
        }

        /// <summary>
        /// 檢查SN的ROHS與carton的ROHS是否一致
        /// ADD BY HGB 2019.06.11
        /// </summary>
        /// <param name="wo_no"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void CheckRohsBySnCarton(string sn,string carton , OleExec DB)
        {
            
            string sql = string.Empty;
            string snrohs = string.Empty;
            string cartonrohs = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                select distinct rohs
  from r_wo_base
 where workorderno in
       (select workorderno
          from r_sn where sn ='{sn}')
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new MESReturnMessage("該SN工單的ROHS狀態沒有設定"); ;
            }
            else
            {
                snrohs = dt.Rows[0][0].ToString();

                sql = $@" 
                select distinct rohs
  from r_wo_base
 where workorderno in
       (select workorderno
          from r_sn
         where id in (select sn_id
                        from r_sn_packing
                       where pack_id in
                             (SELECT id
                                FROM R_PACKING
                               where pack_type = 'CARTON'
                                 AND PACK_NO = '{carton}')))
                 ";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 1)
                {
                    throw new MESReturnMessage("該CARTON工單的ROHS狀態不一致"); 
                }
                else if (dt.Rows.Count == 1)
                {
                    cartonrohs= dt.Rows[0][0].ToString();
                    if (snrohs != cartonrohs)
                    {
                        throw new MESReturnMessage($@"該SN的ROHS與CARTON的ROHS狀態不一致,{sn}的ROHS為{snrohs},{carton}的ROHS為{cartonrohs}");
                    }


                }
            }
         
        }

        /// <summary>
        /// 檢查SN的ROHS與Pallet的ROHS是否一致
        /// ADD BY HGB 2019.06.11
        /// </summary>
        /// <param name="wo_no"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void CheckRohsBySnPallet(string sn, string Pallet, OleExec DB)
        {
            string sql = string.Empty;
            string snrohs = string.Empty;
            string Palletrohs = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                select distinct rohs
  from r_wo_base
 where workorderno in
       (select workorderno
          from r_sn where sn ='{sn}')
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new MESReturnMessage("該SN工單的ROHS狀態沒有設定"); ;
            }
            else
            {
                snrohs = dt.Rows[0][0].ToString();

                sql = $@" 
                select distinct rohs
   from r_wo_base
  where workorderno in
        (select workorderno
           from r_sn
          where id in
                (select sn_id
                   from r_sn_packing
                  where pack_id in
                        (SELECT id
                           FROM R_PACKING
                          where pack_type = 'CARTON'
                            AND PARENT_PACK_ID in
                                (SELECT id
                                   FROM R_PACKING
                                  where pack_type = 'PALLET'
                                    AND pack_no = '{Pallet}'))))
                 ";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 1)
                {
                    throw new MESReturnMessage("該CARTON工單的ROHS狀態不一致");
                }
                else if (dt.Rows.Count == 1)
                {
                    Palletrohs = dt.Rows[0][0].ToString();
                    if (snrohs != Palletrohs)
                    {
                        throw new MESReturnMessage($@"該SN的ROHS與CARTON的ROHS狀態不一致,{sn}的ROHS為{snrohs},{Pallet}的ROHS為{Palletrohs}");
                    }


                }
            }

        }


        public List<R_WO_BASE> CheckFlag(string wo_no, OleExec DB)
        {
            //List<R_WO_BASE> WOList = new List<R_WO_BASE>();
            //string sql = "";
            //DataTable dt = null;
            //try
            //{

            //    sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO='{wo_no}' and input_qty<>0 ";
            //    dt = DB.ExecSelect(sql).Tables[0];
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        WOList.Add(CreateLanguageClass(dr));
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //return WOList;

            return DB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == wo_no && t.INPUT_QTY != 0).ToList();
        }
        public List<R_WO_BASE> ShowAllData(OleExec DB)
        {
            //string sql = string.Empty;
            //DataTable dt = new DataTable();
            //List<R_WO_BASE> LanguageList = new List<R_WO_BASE>();
            //sql = $@"SELECT * FROM R_WO_BASE order by EDIT_TIME";
            //dt = DB.ExecSelect(sql).Tables[0];
            //foreach (DataRow dr in dt.Rows)
            //{
            //    LanguageList.Add(CreateLanguageClass(dr));
            //}
            //return LanguageList;
            return DB.ORM.Queryable<R_WO_BASE>().OrderBy(t => t.EDIT_TIME).ToList();
        }
        public R_WO_BASE CreateLanguageClass(DataRow dr)
        {
            Row_R_WO_BASE row = (Row_R_WO_BASE)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        //use for load WO in add_WOSNRange function in WOSNRange.html
        public List<R_WO_BASE> GetAllWO(string r_wo, OleExec DB)
        {
            //List<R_WO_BASE> WOList = new List<R_WO_BASE>();
            //string sql = "";
            //DataTable dt = null;
            //try
            //{
            //    if (string.IsNullOrEmpty(r_wo.Trim()))
            //    {
            //        sql = "SELECT * FROM  R_WO_BASE where WORKORDERNO NOT IN (SELECT WORKORDERNO FROM R_WO_REGION)  and rownum<20";
            //    }
            //    else
            //    {
            //        sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO like '%{r_wo.ToUpper()}%' and rownum<20 ORDER BY EDIT_TIME";
            //    }
            //    dt = DB.ExecSelect(sql).Tables[0];
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        WOList.Add(CreateLanguageClass(dr));
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //return WOList;

            return DB.ORM.Queryable<R_WO_BASE>()
                .WhereIF(!string.IsNullOrEmpty(r_wo),t=>SqlFunc.Contains(t.WORKORDERNO,r_wo.ToUpper()))
                .WhereIF(string.IsNullOrEmpty(r_wo), t => !SqlFunc.Subqueryable<R_WO_REGION>().Where(s => s.WORKORDERNO == t.WORKORDERNO).Any())
                .OrderBy(t => t.EDIT_TIME).Take(20).ToList();
            
        }
        public List<R_WO_BASE> GetSkunoByWO(string r_wo, string r_skuno, OleExec DB)
        {
            //List<R_WO_BASE> SkunoList = new List<R_WO_BASE>();
            //string sql = "";
            //DataTable dt = null;
            //try
            //{
            //    sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO='{r_wo.ToUpper()}'";
            //    dt = DB.ExecSelect(sql).Tables[0];
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        SkunoList.Add(CreateLanguageClass(dr));
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //return SkunoList;

            /* edit by jk-vn 2021-02-27*/
            if (!string.IsNullOrEmpty(r_skuno) && !string.IsNullOrEmpty(r_wo))
            {
                return DB.ORM.Queryable<R_WO_BASE>().Where(t => t.SKUNO == r_skuno && t.WORKORDERNO == r_wo).ToList();
            }
            else if (!string.IsNullOrEmpty(r_skuno))
            {
                return DB.ORM.Queryable<R_WO_BASE>().Where(t => t.SKUNO == r_skuno).ToList();
            }
            else
            {
                return DB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == r_wo).ToList();
            }
        }
        public List<R_WO_BASE> GetQtyByWOSkuno(string r_wo, string r_skuno, int r_qty, OleExec DB)
        {
            //List<R_WO_BASE> qty = new List<R_WO_BASE>();
            //string sql = "";
            //DataTable dt = null;
            //try
            //{
            //    sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO='{r_wo.ToUpper()}' and SKUNO='{r_skuno}' ";
            //    dt = DB.ExecSelect(sql).Tables[0];
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        qty.Add(CreateLanguageClass(dr));
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //return qty;
            return GetSkunoByWO(r_wo,r_skuno, DB).FindAll(t => t.SKUNO == r_skuno);
        }

        public List<R_WO_BASE> CheckToUpload(string r_wo, string r_skuno, int r_qty, OleExec DB)
        {
            //List<R_WO_BASE> qty = new List<R_WO_BASE>();
            //string sql = "";
            //DataTable dt = null;
            //try
            //{
            //    sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO='{r_wo.ToUpper()}' and SKUNO='{r_skuno.ToUpper()}'and QTY='{r_qty}' ";
            //    dt = DB.ExecSelect(sql).Tables[0];
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        qty.Add(CreateLanguageClass(dr));
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //return qty;
            return GetQtyByWOSkuno(r_wo, r_skuno,r_qty, DB).FindAll(t => t.INPUT_QTY == r_qty);
        }
        public int UpdateFINISHEDQTYAddOne(string r_wo, OleExec DB)
        {
            //Modify by ZGJ 為什麼已經有了 UpdateFINISHEDQTY 方法，還要有個 addOne 的單獨實現？而且實現邏輯還有點出入
            //string strSql = $@"update r_wo_base set finished_qty=case when (finished_qty is null) then 1 else (finished_qty+1) end where workorderno=:wono";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":wono", r_wo) };
            //int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            //return result;
            return UpdateFinishQty(r_wo, 1, DB);
            
        }
        /// <summary>
        /// 入MRB檢查WORKORDER_QTY與FINISHED_QTY，如WORKORDER_QTY小於等於FINISHED_QTY 則關閉工單
        /// </summary>
        /// <param name="r_wo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void UpdateWoCloseFlag(string r_wo, OleExec DB)
        {
            string strSql = $@"update r_wo_base set CLOSED_FLAG=1,CLOSE_DATE = sysdate where WORKORDER_QTY <= FINISHED_QTY and workorderno=:wono";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":wono", r_wo) };
            DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
        }
        /// <summary>
        /// 查詢工單前綴為00251且沒有關閉的工單信息
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public List<R_WO_BASE> MatchSpecialPrefixWO(OleExec sfcdb, string prefix)
        {
            //List<R_WO_BASE> woes = new List<R_WO_BASE>();
            //if (string.IsNullOrEmpty(prefix))
            //{
            //    return null;
            //}
            //DataTable dt = null;
            //Row_R_WO_BASE row_wo = null;
            //string sql = $@"select * from {this.TableName} where workorderno like '{prefix.Replace("'", "''")}%' and closed_flag='0' ";
            //dt = sfcdb.ExecSelect(sql).Tables[0];
            //foreach (DataRow dr in dt.Rows)
            //{
            //    row_wo = (Row_R_WO_BASE)this.NewRow();
            //    row_wo.loadData(dr);

            //    woes.Add(row_wo.GetDataObject());
            //}
            //return woes;
            return sfcdb.ORM.Queryable<R_WO_BASE>().Where(t => SqlFunc.Contains(t.WORKORDERNO, prefix) && t.CLOSED_FLAG == "0").ToList();
        }

        public int UpdateWoQty(string WO,int CutQty,string Emp_NO, OleExec sfcdb)
        {
            string strSql = $@"update r_wo_base set workorder_qty=workorder_qty-{CutQty},edit_emp='{Emp_NO}',edit_time=sysdate where workorderno='{WO}'";
            int result = sfcdb.ExecSqlNoReturn(strSql, null);
            return result;
        }

        public List<R_WO_BASE> GetThreeMonthsWO(OleExec DB)
        {
            //List<R_WO_BASE> listWo = new List<R_WO_BASE>();
            //string sql = "";
            //DataTable dt = null;

            //if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            //{
            //    sql = $@"select * from r_wo_base where edit_time>sysdate-90 order by edit_time";               
            //    dt = DB.ExecSelect(sql).Tables[0];
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        listWo.Add(CreateLanguageClass(dr));
            //    }
            //}
            //else
            //{
            //    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
            //    throw new MESReturnMessage(errMsg);
            //}
            //return listWo;
            return DB.ORM.Queryable<R_WO_BASE>().Where(t => t.EDIT_TIME > DateTime.Now.AddDays(-90)).ToList();
        }
        public List<R_WO_BASE> GetSampleWOBySku(OleExec DB,string sku)
        {
            return DB.ORM.Queryable<R_WO_BASE>().WhereIF(!sku.Equals(""),r => r.SKUNO == sku)
                .Where(r => r.WORKORDERNO.StartsWith("99")).ToList();
        }
        public R_WO_BASE GetWOByCarton(string CartonNo, OleExec DB)
        {
            string MaxSkuVer = string.Empty;

            var cc = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == CartonNo).First();
            if (cc.QTY == 0)
            {
                var cc1 = DB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == cc.PARENT_PACK_ID && t.QTY > 0).First();
                if (cc1 != null)
                {
                    CartonNo = cc1.PACK_NO;
                }
                else
                {
                    throw new Exception("Carton is empty");
                }
            }

            List<R_WO_BASE> WoBases = DB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN, R_WO_BASE>((p, sp, s, wb) => p.ID == sp.PACK_ID && sp.SN_ID == s.ID && s.WORKORDERNO == wb.WORKORDERNO)
               .Where((p,sp,s,wb)=>p.PACK_NO==CartonNo).Select((p, sp, s, wb) => wb).ToList();

            foreach (R_WO_BASE Base in WoBases)
            {
                if (Base.SKU_VER!=null && Base.SKU_VER.CompareTo(MaxSkuVer) > 0)
                {
                    MaxSkuVer = Base.SKU_VER;
                }
            }

            return WoBases.Find(t => t.SKU_VER == (MaxSkuVer.Length==0?null:MaxSkuVer));
        }

        public List<R_WO_BASE> GetAllNotClosedWO(OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_BASE>().Where(r => r.CLOSED_FLAG == "0").ToList();
        }

        //根據WO查
        public List<R_WO_BASE> GetWOInforByWo(string workorderno, OleExec DB)
        {
            //string strSql = $@"select workorderno,skuno,cust_pn,download_date,production_type,closed_flag from R_WO_BASE where workorderno=:workorderno";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter("workorderno", workorderno) };
            //DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            //List<R_WO_BASE> result = new List<R_WO_BASE>();
            //if (res.Rows.Count > 0)
            //{
            //    Row_R_WO_BASE ret = (Row_R_WO_BASE)NewRow();
            //    ret.loadData(res.Rows[0]);
            //    result.Add(ret.GetDataObject());
            //    return result;
            //}
            //else
            //{
            //    return null;
            //}

            return DB.ORM.Queryable<R_WO_BASE>().Where(t=>t.WORKORDERNO==workorderno).Select(t=>
                new R_WO_BASE {
                    WORKORDERNO =t.WORKORDERNO,
                    SKUNO =t.SKUNO,
                    CUST_PN =t.CUST_PN,
                    DOWNLOAD_DATE =t.DOWNLOAD_DATE,
                    PRODUCTION_TYPE =t.PRODUCTION_TYPE,
                    CLOSED_FLAG =t.CLOSED_FLAG
                }).ToList();
        }

        //獲取R_WO_BASE所有信息
        public List<R_WO_BASE> GetAllWOInfor(OleExec DB)
        {
            //string strSql = $@"select workorderno,skuno,cust_pn,download_date,production_type,case closed_flag when '0' then 'Opening' else 'Closed' end as closed_flag from R_WO_BASE where CLOSED_FLAG = 0 order by DOWNLOAD_DATE desc";
            //List<R_WO_BASE> result = new List<R_WO_BASE>();
            //DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            //if (res.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in res.Rows) {
            //        Row_R_WO_BASE ret = (Row_R_WO_BASE)NewRow();
            //        ret.loadData(dr);
            //        result.Add(ret.GetDataObject());
            //    }
            //    return result;
            //}
            //else
            //{
            //    return null;
            //}     

            //數據量太大，這里在前端的列表只顯示100條數據，對實際功能無影響
            return DB.ORM.Queryable<R_WO_BASE>().OrderBy(t => t.DOWNLOAD_DATE, SqlSugar.OrderByType.Desc).Take(100).ToList();
        }

        /// <summary>
        /// Author: Simon
        /// 打開或關閉工單操作
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="workorderno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateWOByWorkorderno(string edit_Emp, string workorderno, OleExec DB)
        {
            string sql = $@"select closed_flag from r_wo_base where workorderno='{workorderno}'";
            DataTable dt = DB.ExecuteDataTable(sql, CommandType.Text);
            string _closed_FLag = dt.Rows[0][0].ToString().Trim();
            int result = 0;
            switch (_closed_FLag)
            {
                case "0"://當前為 打開 狀態時切換為 關閉
                    sql = $@"UPDATE R_WO_BASE SET CLOSED_FLAG='1',EDIT_TIME=sysdate,EDIT_EMP='{edit_Emp}' WHERE WORKORDERNO ='{workorderno}'";
                    break;
                case "1"://當前為 關閉 時，切換為打開
                    sql = $@"UPDATE R_WO_BASE SET CLOSED_FLAG='0',EDIT_TIME=sysdate,EDIT_EMP='{edit_Emp}' WHERE WORKORDERNO ='{workorderno}'";
                    break;
                default:
                    break;
            }
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                result = DB.ExecuteNonQuery(sql, CommandType.Text);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;

        }

        public int UpdateWOINPUTQTY(string WO, OleExec DB,int Count=1)
        {
            int result = 0;
            string sql = string.Empty;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                string strSql = $@"UPDATE R_WO_BASE SET INPUT_QTY=INPUT_QTY+{Count} WHERE WORKORDERNO='{WO}'";
                //result = DB.ExecSQL(strSql);
                result = DB.ExecSqlNoReturn(strSql, null);
                return result;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
        }

        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="KPListID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateKPLISTID(string WO, string KPListID, OleExec DB)
        {
            int result = 0;
            string sql = string.Empty;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                string strSql = $@"UPDATE R_WO_BASE SET KP_LIST_ID = '{KPListID}' WHERE WORKORDERNO='{WO}'";
                //result = DB.ExecSQL(strSql);
                result = DB.ExecSqlNoReturn(strSql, null);
                return result;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
        }

        /// <summary>
        /// SN機種類型與工單前綴比對檢查 add by zc
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="WORKORDERNO"></param>
        /// <returns></returns>
        /// 
        public bool CheckSnTypeCompareWoPrefix(string WO, string SKUNO, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            //sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO='{WO.Trim().ToUpper()}' AND  SFC.LEFT('{WO.Trim().ToUpper()}',6) IN (  
            //Select CONTROL_LEVEL from C_CONTROL   WHERE CONTROL_NAME= 'WORKORDERTYPE'  AND SUBSTR(CONTROL_TYPE,1, INSTR(CONTROL_TYPE,' ',1,1)-1)=
            //(SELECT SKU_TYPE FROM  C_SKU WHERE SKUNO =(SELECT SKUNO  FROM R_SN WHERE SN='{SN.Trim().ToUpper()}'))AND INSTR(CONTROL_TYPE,'REGULAR')=0) ";

            sql = $@"SELECT * FROM  R_WO_BASE WHERE WORKORDERNO='{WO.Trim().ToUpper()}' AND  SFC.LEFT('{WO.Trim().ToUpper()}',6) IN (  
             Select CONTROL_LEVEL from C_CONTROL   WHERE CONTROL_NAME= 'WORKORDERTYPE'  AND SUBSTR(CONTROL_TYPE,1, INSTR(CONTROL_TYPE,' ',1,1)-1)=
             (SELECT SKU_TYPE FROM  C_SKU WHERE SKUNO ='{SKUNO}') AND INSTR(CONTROL_TYPE,'REGULAR')=0) ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return res;
        }

        public R_WO_BASE GetMaxVer(string PLL, OleExec sfcdb)
        {
            string sql = $@"SELECT * FROM R_WO_BASE WHERE WORKORDERNO IN ( 
                            SELECT WORKORDERNO FROM R_SN WHERE ID IN (
                            SELECT SN_ID FROM R_SN_PACKING WHERE PACK_ID IN (
                            SELECT ID FROM R_PACKING WHERE PARENT_PACK_ID IN (
                            SELECT ID FROM R_PACKING WHERE PACK_NO='{PLL}')))) AND ROWNUM=1 ORDER BY SKU_VER";
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_WO_BASE rowPacking = (Row_R_WO_BASE)this.NewRow();
                rowPacking.loadData(ds.Tables[0].Rows[0]);
                return rowPacking.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public List<string> GetWoListBySkuAndVer(OleExec sfcdb, string skuno, string skuver)
        {
            ////List<R_WO_BASE> woes = new List<R_WO_BASE>();
            //List<string> woes = new List<string>();
            //DataTable dt = null;
            ////Row_R_WO_BASE row_wo = null;
            //string row_wo = string.Empty;
            //string strSql = $@" select distinct workorderno from r_wo_base where skuno='{skuno}' and sku_ver='{skuver}' ";
            //dt = sfcdb.ExecSelect(strSql).Tables[0];
            //foreach (DataRow dr in dt.Rows)
            //{
            //    //row_wo = (Row_R_WO_BASE)this.NewRow();
            //    row_wo = dr["workorderno"].ToString();
            //    woes.Add(row_wo);
            //}
            //return woes;

            return sfcdb.ORM.Queryable<R_WO_BASE>().Where(t => t.SKUNO == skuno && t.SKU_VER == skuver).GroupBy(t => t.WORKORDERNO).Select(t=>t.WORKORDERNO).ToList();
        }

        /// <summary>
        /// <edit by>Simon</edit>
        /// 根據機種SKUNO獲取版本SKU_VER
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public List<string> GetVerBySkuno(OleExec DB, string skuno)
        {
            //List<string> verList = new List<string>();
            //DataTable dt = null; ;
            //string row_ver = string.Empty;
            //string strSql = $@"select '' SKU_VER from dual uinon select distinct trim(sku_ver) SKU_VER from r_wo_base where SKUNO = '{skuno}'";
            //dt = DB.ExecSelect(strSql).Tables[0];
            //foreach (DataRow dr in dt.Rows)
            //{
            //    row_ver = dr["SKU_VER"].ToString();
            //    verList.Add(row_ver);
            //}
            //return verList;

            var verList = new List<string>() { "" };
            verList.AddRange(DB.ORM.Queryable<R_WO_BASE>().Where(t => t.SKUNO == skuno).GroupBy(t => t.SKU_VER).Select(t => t.SKU_VER).ToList());
            return verList;
        }

        /// <summary>
        /// 通過機種和版本查找R_WO_BASE表
        /// </summary>
        /// <param name="getSkuno"></param>
        /// <param name="getSkuVer"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public R_WO_BASE GetWoBySkuAndVer(string getSkuno, string getSkuVer, OleExec db, ref string wo)
        {
            string strSql = $@" select * from r_wo_base where skuno='{getSkuno}' and sku_ver='{getSkuVer}' ";
            wo = "(";

            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            R_WO_BASE result = new R_WO_BASE();
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Row_R_WO_BASE ret = (Row_R_WO_BASE)this.NewRow();
                    ret.loadData(table.Rows[i]);
                    if (!wo.Contains(ret.GetDataObject().WORKORDERNO))
                    {
                        wo += "'" + ret.GetDataObject().WORKORDERNO + "',";
                    }
                }
                wo = wo.Substring(0, wo.Length - 1) + ")";
            }
            else
            {
                result = null;
            }
            return result;
        }

        public string GetCHECK_COMPLIANCE_FOCRes(string SN, OleExec db)
        {
            string Res = "OK";
            string strSql = $@" SELECT SFC.FN_Check_Compliance_FOC('{SN}') from dual ";

            DataTable table = db.ExecSelect(strSql).Tables[0];
            if (table.Rows.Count > 0)
            {
                Res = table.Rows[0][0].ToString();
            }

            return Res;
        }

        public bool CHECKRNPICONTROL(string _WO, OleExec db)
        {
            bool Res = false;
            string strSql = $@"select*from C_CONTROL where CONTROL_NAME ='NPICONTROL' AND CONTROL_VALUE='NPICONTROL'";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            if (table.Rows.Count > 0)
            {
                Res = true;
            }

            return Res;
        }
        public string CHECKNPI(string _WO, OleExec db)
        {
            string Res = "OK";
            string strSql = $@" SELECT SFC.CHECK_NPI_WO('{_WO}') from dual  WHERE SUBSTR('{_WO}' ,0,6) NOT IN(SELECT PREFIX FROM   R_WO_TYPE WHERE WORKORDER_TYPE in('PCBA RMA','MODEL RMA','MODEL REWORK','PCBA REWORK')) ";

            DataTable table = db.ExecSelect(strSql).Tables[0];
            if (table.Rows.Count > 0)
            {
                Res = table.Rows[0][0].ToString();
            }

            return Res;
        }

        public string CHECKR_NPI_WO(string _WO, OleExec db)
        {
            string Res = null;
            string strSql = $@" SELECT *  FROM R_NPI_WO WHERE WO='{_WO}'";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            if (table.Rows.Count > 0)
            {
                Res = "NPI";
            }

            return Res;
        }

        public DataTable/*List<R_WO_BASE>*/ GetWOBYWOTYPEOne(string WO, OleExec DB)
        {
            string StrSql = $@"SELECT WORKORDERNO, MFR_NAME , SKUNO73,  SKUNO,WORKORDER_QTY AS QTY ,VERSION,CHECKLIST  FROM (
SELECT B.WORKORDERNO,'' AS MFR_NAME , NVL(RTRIM(C.SKUNO73), '') AS SKUNO73, NVL(RTRIM(B.SKUNO), '') AS SKUNO,
 B.WORKORDER_QTY  , RTRIM(B.SKU_VER) AS VERSION,0 AS CHECKLIST 
FROM R_WO_BASE B LEFT JOIN C_K_MAPPING C ON B.SKUNO = C.SKUNO800
WHERE B.WORKORDERNO = '{WO}' )";
            DataTable DT = DB.ExecSelect(StrSql).Tables[0];
            return DT;
        }

        public int Update(R_WO_BASE objWO, OleExec DB)
        {
            return DB.ORM.Updateable<R_WO_BASE>(objWO).Where(r => r.ID == objWO.ID).ExecuteCommand();
        }

    }
    public class Row_R_WO_BASE : DataObjectBase
    {
        public Row_R_WO_BASE(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_BASE GetDataObject()
        {
            R_WO_BASE DataObject = new R_WO_BASE();
            DataObject.ID = this.ID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.PLANT = this.PLANT;
            DataObject.RELEASE_DATE = this.RELEASE_DATE;
            DataObject.DOWNLOAD_DATE = this.DOWNLOAD_DATE;
            DataObject.PRODUCTION_TYPE = this.PRODUCTION_TYPE;
            DataObject.WO_TYPE = this.WO_TYPE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_VER = this.SKU_VER;
            DataObject.SKU_SERIES = this.SKU_SERIES;
            DataObject.SKU_NAME = this.SKU_NAME;
            DataObject.SKU_DESC = this.SKU_DESC;
            DataObject.CUST_PN = this.CUST_PN;
            DataObject.CUST_PN_VER = this.CUST_PN_VER;
            DataObject.CUSTOMER_NAME = this.CUSTOMER_NAME;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.START_STATION = this.START_STATION;
            DataObject.KP_LIST_ID = this.KP_LIST_ID;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.CLOSE_DATE = this.CLOSE_DATE;
            DataObject.WORKORDER_QTY = this.WORKORDER_QTY;
            DataObject.INPUT_QTY = this.INPUT_QTY;
            DataObject.FINISHED_QTY = this.FINISHED_QTY;
            DataObject.SCRAPED_QTY = this.SCRAPED_QTY;
            DataObject.STOCK_LOCATION = this.STOCK_LOCATION;
            DataObject.PO_NO = this.PO_NO;
            DataObject.CUST_ORDER_NO = this.CUST_ORDER_NO;
            DataObject.ROHS = this.ROHS;
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
        public DateTime? RELEASE_DATE
        {
            get
            {
                return (DateTime?)this["RELEASE_DATE"];
            }
            set
            {
                this["RELEASE_DATE"] = value;
            }
        }
        public DateTime? DOWNLOAD_DATE
        {
            get
            {
                return (DateTime?)this["DOWNLOAD_DATE"];
            }
            set
            {
                this["DOWNLOAD_DATE"] = value;
            }
        }
        public string PRODUCTION_TYPE
        {
            get
            {
                return (string)this["PRODUCTION_TYPE"];
            }
            set
            {
                this["PRODUCTION_TYPE"] = value;
            }
        }
        public string WO_TYPE
        {
            get
            {
                return (string)this["WO_TYPE"];
            }
            set
            {
                this["WO_TYPE"] = value;
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
        public string SKU_VER
        {
            get
            {
                return (string)this["SKU_VER"];
            }
            set
            {
                this["SKU_VER"] = value;
            }
        }
        public string SKU_SERIES
        {
            get
            {
                return (string)this["SKU_SERIES"];
            }
            set
            {
                this["SKU_SERIES"] = value;
            }
        }
        public string SKU_NAME
        {
            get
            {
                return (string)this["SKU_NAME"];
            }
            set
            {
                this["SKU_NAME"] = value;
            }
        }
        public string SKU_DESC
        {
            get
            {
                return (string)this["SKU_DESC"];
            }
            set
            {
                this["SKU_DESC"] = value;
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
        public string CUST_PN_VER
        {
            get
            {
                return (string)this["CUST_PN_VER"];
            }
            set
            {
                this["CUST_PN_VER"] = value;
            }
        }
        public string CUSTOMER_NAME
        {
            get
            {
                return (string)this["CUSTOMER_NAME"];
            }
            set
            {
                this["CUSTOMER_NAME"] = value;
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
        public string START_STATION
        {
            get
            {
                return (string)this["START_STATION"];
            }
            set
            {
                this["START_STATION"] = value;
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
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
            }
        }
        public DateTime? CLOSE_DATE
        {
            get
            {
                return (DateTime?)this["CLOSE_DATE"];
            }
            set
            {
                this["CLOSE_DATE"] = value;
            }
        }
        public double? WORKORDER_QTY
        {
            get
            {
                return (double?)this["WORKORDER_QTY"];
            }
            set
            {
                this["WORKORDER_QTY"] = value;
            }
        }
        public double? INPUT_QTY
        {
            get
            {
                return (double?)this["INPUT_QTY"];
            }
            set
            {
                this["INPUT_QTY"] = value;
            }
        }
        public double? FINISHED_QTY
        {
            get
            {
                return (double?)this["FINISHED_QTY"];
            }
            set
            {
                this["FINISHED_QTY"] = value;
            }
        }
        public double? SCRAPED_QTY
        {
            get
            {
                return (double?)this["SCRAPED_QTY"];
            }
            set
            {
                this["SCRAPED_QTY"] = value;
            }
        }
        public string STOCK_LOCATION
        {
            get
            {
                return (string)this["STOCK_LOCATION"];
            }
            set
            {
                this["STOCK_LOCATION"] = value;
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
        public string ROHS
        {
            get
            {
                return (string)this["ROHS"];
            }
            set
            {
                this["ROHS"] = value;
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
    public class R_WO_BASE
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{get;set;}
        public string WORKORDERNO{get;set;}
        public string PLANT{get;set;}
        public DateTime? RELEASE_DATE{get;set;}
        public DateTime? DOWNLOAD_DATE{get;set;}
        public string PRODUCTION_TYPE{get;set;}
        public string WO_TYPE{get;set;}
        public string SKUNO{get;set;}
        public string SKU_VER{get;set;}
        public string SKU_SERIES{get;set;}
        public string SKU_NAME{get;set;}
        public string SKU_DESC{get;set;}
        public string CUST_PN{get;set;}
        public string CUST_PN_VER{get;set;}
        public string CUSTOMER_NAME{get;set;}
        public string ROUTE_ID{get;set;}
        public string START_STATION{get;set;}
        public string KP_LIST_ID{get;set;}
        public string CLOSED_FLAG{get;set;}
        public DateTime? CLOSE_DATE{get;set;}
        public double? WORKORDER_QTY{get;set;}
        public double? INPUT_QTY{get;set;}
        public double? FINISHED_QTY{get;set;}
        public double? SCRAPED_QTY{get;set;}
        public string STOCK_LOCATION{get;set;}
        public string PO_NO{get;set;}
        public string CUST_ORDER_NO{get;set;}
        public string ROHS{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
    //RuRun

    public class T_R_WO_DEVIATION : DataObjectTable
    {
        public T_R_WO_DEVIATION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_DEVIATION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            TableName = "R_WO_DEVIATION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_WO_DEVIATION> GetWoDeviations(OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_DEVIATION>().ToList();
        }

        public R_WO_DEVIATION GetDeviationByWo(string WorkOrder, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_DEVIATION>().Where(t => t.WORKORDERNO == WorkOrder).ToList().FirstOrDefault();
        }

        public List<R_WO_DEVIATION> GetWoDeviationByFuzzyQuery(string condition, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_DEVIATION>().Where(t => t.DEVIATION.Contains(condition) || t.WORKORDERNO.Contains(condition)).ToList();
        }

        public int DeleteWoDeviationById(string Id, OleExec DB)
        {
            return DB.ORM.Deleteable<R_WO_DEVIATION>().Where(t => t.ID == Id).ExecuteCommand();
        }

        public int DeleteWoDeviationByIds(string Ids, OleExec DB)
        {
            return DB.ORM.Deleteable<R_WO_DEVIATION>().Where(t => Ids.Contains(t.ID)).ExecuteCommand();
        }

        public int UpdateWoDeviationById(string Id, string Deviation, string EditEmp, OleExec DB)
        {
            return DB.ORM.Updateable<R_WO_DEVIATION>()
                .UpdateColumns(t=>new R_WO_DEVIATION() { DEVIATION=Deviation,EDIT_EMP=EditEmp,EDIT_TIME=DateTime.Now})
                .Where(t => t.ID == Id).ExecuteCommand();
        }

        public int UpdateWoDeviation(string WorkOrder, string Deviation, string EditEmp, OleExec DB)
        {
            R_WO_DEVIATION deviation = DB.ORM.Queryable<R_WO_DEVIATION>().Where(t => t.WORKORDERNO == WorkOrder).ToList().FirstOrDefault();
            deviation.DEVIATION = Deviation;
            deviation.EDIT_EMP = EditEmp;
            deviation.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Updateable<R_WO_DEVIATION>(deviation).Where(t => t.WORKORDERNO == WorkOrder).ExecuteCommand();

            //return DB.ORM.Updateable<R_WO_DEVIATION>()
            //    .UpdateColumns(t => new R_WO_DEVIATION { DEVIATION = Deviation, EDIT_EMP = EditEmp,EDIT_TIME=GetDBDateTime(DB) })
            //    .Where(t => t.WORKORDERNO == WorkOrder)
            //    .ExecuteCommand();
        }
        public int AddWoDeviation(string WorkOrder, string Deviation,string Bu,string EditEmp, OleExec DB)
        {
            R_WO_DEVIATION RWD = new R_WO_DEVIATION();
            RWD.ID = GetNewID(Bu, DB);
            RWD.WORKORDERNO = WorkOrder;
            RWD.DEVIATION = Deviation;
            RWD.EDIT_EMP = EditEmp;
            RWD.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable<R_WO_DEVIATION>(RWD).ExecuteCommand();
        }

    }
    public class R_WO_DEVIATION
{
    public string ID { get; set; }
    public string WORKORDERNO { get; set; }
    public string DEVIATION { get; set; }
    public string EDIT_EMP { get; set; }
    public DateTime? EDIT_TIME { get; set; }
}
}
