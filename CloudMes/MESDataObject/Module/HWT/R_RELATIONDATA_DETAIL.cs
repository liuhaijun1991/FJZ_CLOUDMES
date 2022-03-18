using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_RELATIONDATA_DETAIL : DataObjectTable
    {
        public T_R_RELATIONDATA_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RELATIONDATA_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RELATIONDATA_DETAIL);
            TableName = "R_RELATIONDATA_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckSnExists(string SN, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM R_RELATIONDATA_DETAIL WHERE SN = '{SN}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool UpdateMrbSnStatus1(string SN, string routeid, string NextStation, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"UPDATE r_relationdata_detail
                         SET send_flag = 'WAIT_DELETE', EDIT_TIME = SYSDATE
                       WHERE SUBSTR(categoryname, 1, 4) NOT IN
                             ('IMEI', 'MAC1', 'WWID', 'WWSN')
                         AND categoryname NOT LIKE '%MAC%'
                         AND send_flag = 'SEND'
                         AND SN = '{SN}'
                         AND eventpoint IN
                             (SELECT station_name
                                FROM c_route_detail
                               WHERE routeid = '{routeid}'
                                 AND eventseqno BETWEEN (select seq_no from c_route_detail where route_id='{routeid}' and station_name='{NextStation}') 
                                 AND (select seq_no from c_route_detail where route_id='{routeid}' 
                                 and striong_name=(select * from (select NEXT_STATION from r_mrb where sn='{SN}'and REWORK_WO is null ORDER BY EDIT_TIME DESC)
                                 where rownum=1 ))
                                 AND station_name IN
                                     (SELECT STATION
                                        FROM r_sn_kp
                                       WHERE SN = '{SN}'))";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public bool UpdateMrbSnStatus2(string SN, string routeid, string NextStation, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"UPDATE r_relationdata_detail
                        SET send_flag = 'DELETE', lasteditdt = SYSDATE
                    WHERE SUBSTR(categoryname, 1, 4) NOT IN
                            ('IMEI', 'MAC1', 'WWID', 'WWSN')
                        AND send_flag = 'WAIT_SEND'
                        AND sysserialno = '{SN}'
                        AND eventpoint IN
                            (SELECT station_name
                            FROM c_route_detail
                            WHERE routeid = '{routeid}'
                                AND eventseqno BETWEEN (select seq_no from c_route_detail where route_id='{routeid}' and station_name='{NextStation}') 
                                AND (select seq_no from c_route_detail where route_id='' 
                                and striong_name=(select * from (select NEXT_STATION from r_mrb where sn='{SN}'and REWORK_WO is null ORDER BY EDIT_TIME DESC)
                                where rownum=1 ))
                                AND eventpoint IN
                                    (SELECT STATION
                                    FROM r_sn_kp
                                    WHERE SN = '{SN}'))";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        /// <summary>
        /// HWT_ASSY類生成傳送HW綁定關係R_RELATIONDATA_DETAIL
        /// add by hgb 2019.08.28
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="SKUNO"></param>
        /// <param name="DB"></param>
        public void HWTMakeSentHwRelation(string SN, string SKUNO, string STATION,string WO, OleExec DB)
        {
            string value = string.Empty;
            string categoryname = string.Empty;
            string parent = string.Empty;
            string son = string.Empty;
            string Strsql = string.Empty;
            T_R_WO_TYPE WOType = new T_R_WO_TYPE(DB, DB_TYPE_ENUM.Oracle);
            T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_SN t_r_sn = new T_R_SN(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_SN r_sn = new R_SN();
            List<C_SKU_DETAIL> c_sku_detail = t_c_sku_detail.LoadListData(SKUNO, "RELATION", "", DB);

            #region  是否需要生成傳HW綁定關係
            Strsql = $@"SELECT *
          FROM r_2d_sn_relation a
         WHERE sn = '{parent}'
           AND  length(a.po) = 6
           AND po IS NOT NULL ";
            DataTable dt = DB.ExecSelect(Strsql).Tables[0];
            if (dt.Rows.Count == 0)
            {

                #region 獲取父子項配置信息
                if (c_sku_detail.Count == 0)
                {
                    throw new MESReturnMessage($@"機種父項關係未設定，請確認！{SKUNO}");
                }
                else if (c_sku_detail.Count > 1)
                {
                    throw new MESReturnMessage($@"機種有多筆父項關係，請確認！{SKUNO}");
                }
                value = c_sku_detail[0].VALUE.ToString();
                categoryname = c_sku_detail[0].CATEGORY_NAME.ToString();
                #endregion
                List<R_SN_KP> r_sn_kp = t_r_sn_kp.LoadListDataBySnAndKpName(SN, categoryname, DB);
                #region 獲取父項和子項
                try
                {
                    if (value == "2")//某個KP為父項
                    {
                        if (r_sn_kp.Count == 0)
                        {
                            r_sn_kp = t_r_sn_kp.LoadListDataBySnAndKpName(SN, "", DB);
                            if (r_sn_kp.Count > 0)
                            {
                                parent = r_sn_kp[0].SN.ToString();
                                son = r_sn_kp[0].VALUE.ToString();
                            }
                        }
                        else
                        {
                            parent = r_sn_kp[0].SN.ToString();
                            son = r_sn_kp[0].VALUE.ToString();
                        }
                    }
                    else
                    {
                        if (r_sn_kp.Count == 0)//加這個，過ASSYP ,value 1 KP設置在ASSY也可以
                        {
                            r_sn_kp = t_r_sn_kp.LoadListDataBySnAndKpName(SN, "", DB);
                            if (r_sn_kp.Count > 0)
                            {
                                parent = r_sn_kp[0].SN.ToString();
                                son = r_sn_kp[0].VALUE.ToString();
                            }
                            else
                            {
                                throw new MESReturnMessage($@"沒有綁定關係，請確認！{SKUNO}");
                            }
                        }
                        else
                        {
                            parent = r_sn_kp[0].SN.ToString();
                            son = r_sn_kp[0].VALUE.ToString();
                        }

                    }
                }
                catch (Exception)
                {
                    throw new MESReturnMessage($@"獲取父項和子項失敗{parent},{son},請確認！{SKUNO}");
                }
                #endregion

                #region  判斷前一次綁定關係是否已刪除
                Strsql = $@"SELECT *
        FROM r_relationdata_detail
       WHERE send_flag <> 'DELETE'
         AND eventpoint = '{STATION}'
         AND substr(categoryname, 1, 4) NOT IN
             ('IMEI', 'MAC1', 'WWID', 'WWSN')
         AND categoryname NOT LIKE '%MAC%'
         AND PARENT = '{parent}'";

                  dt = DB.ExecSelect(Strsql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    throw new MESReturnMessage($@"此父項條碼:{parent}還未在HW系統中刪除前次上傳的對應關係,因此不可綁定!");
                }


                #endregion

                #region  判斷逆向替換舊條碼綁定關係是否待傳HW
                if (STATION=="ASSYN")
                { 
                    Strsql = $@"SELECT *
          FROM (SELECT *
                  FROM (SELECT *
                          FROM r_relationdata_detail
                         WHERE PARENT = '{SN}'
                         ORDER BY send_time DESC)
                 WHERE rownum = 1)
         WHERE send_flag = 'WAIT_SEND' ";

                    dt = DB.ExecSelect(Strsql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        throw new MESReturnMessage($@"舊條碼:'{SN}'綁定關係還沒有傳給HW!!");
                    } 
                }
                #endregion

                #region 判斷是否RMA
                if (!WOType.IsTypeInput("RMA", WO.Substring(0, 6), DB))
                {
                    Strsql = $@"  INSERT INTO r_relationdata_detail
          (
           PARENT,
           son,
           edit_time,
           edit_emp,
           eventpoint,
           parent_partno,
           son_partno,
           categoryname,
           SN,
           task_no,
           is_head,
           send_flag,ID) SELECT TB_ALL.*,'HWT'||SFC.SEQ_R_RELATIONDATA_DETAIL.NEXTVAL AS ID FROM (
          SELECT *
            FROM (SELECT  
                         a.parent,
                         to_char(a.son) son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         get_customer_partno('BARCODE', '{SKUNO}') parent_partno,
                         get_customer_partno('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         CASE
                           WHEN b.po IS NOT NULL THEN
                            b.po
                           ELSE
                            b.wo
                         END task_no,
                         --b.po task_no,
                         '0' is_head,
                         'WAIT_SEND' send_flag
                    FROM r_relation_data a, r_2d_sn_relation b
                   WHERE a.parent = '{parent}'
                     AND a.parent = b.sn
                     AND substr(categoryname, 1, 3) <> 'MAC'
                     AND categoryname <> 'WEIGHT S/N') m --add by hgb 重量信息不需要傳HW 2017.109
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent  AND */ -----廖東林20190422
                  -----PQC 潘建廷要求經與客戶確認，同時存在ASSYP、ASSY工站的產品，ASSYP工站綁定的對應關係不需再以拉手條為父項重傳一次，不然客戶端MES系統抽驗時會校驗失敗
                   m.son = t.son
                AND m.SN = t.SN
                  --  AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')
          UNION
          SELECT *
            FROM (SELECT a.parent,
                         to_char(a.son) son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         get_customer_partno('BARCODE', '{SKUNO}') parent_partno,
                         get_customer_partno('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         CASE
                           WHEN b.po IS NOT NULL THEN
                            b.po
                           ELSE
                            b.wo
                         END task_no,
                         --b.po task_no,
                         '0' is_head,
                         'WAIT_SEND' send_flag
                    FROM r_relation_data a, r_2d_sn_relation b
                   WHERE a.parent = '{parent}'
                     AND a.parent = b.sn
                     AND substr(partno, 1, 2) = 'SN') m
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent  AND*/
                   m.son = t.son
                AND m.SN = t.SN
                  --   AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')
          UNION
          SELECT *
            FROM (SELECT a.parent,
                         b.mac son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         get_customer_partno('BARCODE', '{SKUNO}') parent_partno,
                         get_customer_partno('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         b.task_no task_no,
                         b.is_head is_head,
                         'WAIT_SEND' send_flag
                    FROM r_relation_data a, R_MAC_DETAIL_T b
                   WHERE a.parent = '{parent}'
                     AND a.parent = b.box_sn
                        --根據HW IT 陶成,傳送MAC對應關係只需要傳送首MAC       Taylor 2018-06-07
                     AND b.is_head = '1'
                     AND substr(categoryname, 1, 3) = 'MAC'
                     AND substr(partno, 1, 2) != 'SN') m
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent AND */
                   m.son = t.son
                AND m.SN = t.SN
                  --  AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')  )TB_ALL ";
                    DB.ExecSqlNoReturn(Strsql, null);
                }
                else
                {
                    Strsql = $@"    INSERT INTO r_relationdata_detail
          (PARENT,
           son,
           edit_time,
           edit_emp,
           eventpoint,
           parent_partno,
           son_partno,
           categoryname,
           SN,
           task_no,
           is_head,
           send_flag,
           segent10  ---新增segent10欄位來判斷RMA逆向替換標誌:RTN
          ,ID) SELECT TB_ALL.*,'HWT'||SFC.SEQ_R_RELATIONDATA_DETAIL.NEXTVAL AS ID FROM (
          SELECT PARENT,
                 son,
                 edit_time,
                 edit_emp,
                 eventpoint,
                 parent_partno,
                 partno,
                 categoryname,
                 SN,
                 'EPWENH00008', --modify by hgb 2019.03.11 徐峰要求RMA task_no,傳EPWENH00008
                 is_head,
                 send_flag,
                 decode(categoryname, 'RTN', 'RTN', '') AS rtn_flag --modify by hgb 2019.04.01
            FROM (SELECT decode(categoryname,
                                'RTN',
                                to_char(a.son),
                                a.parent) PARENT,
                         decode(categoryname, 'RTN', PARENT, to_char(a.son)) son,
                         SYSDATE AS edit_time,
                         a.edit_emp,
                         a.eventpoint,
                         GET_CUSTOMER_PARTNO('BARCODE', '{SKUNO}') parent_partno,
                         GET_CUSTOMER_PARTNO('BARCODE', a.partno) partno,
                         a.categoryname,
                         '{SN}' SN,
                         b.workorderno task_no,
                         '0' is_head,
                         'WAIT_SEND' send_flag,
                         ---新增segent10欄位來判斷RMA逆向替換標誌:RTN----20181014--廖東林
                         decode(categoryname, 'RTN', 'RTN', '')
                    FROM r_relation_data a, R_SN b
                   WHERE a.parent = '{parent}'
                     AND a.SN = b.SN
                     AND substr(categoryname, 1, 3) <> 'MAC'
                     AND categoryname <> 'WEIGHT S/N') m --add by hgb 重量信息不需要傳HW 2017.109
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent   AND */
                   m.son = t.son
                AND m.SN = t.SN
                  -- AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')
          UNION
          SELECT *
            FROM (SELECT a.parent,
                         to_char(a.son) son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         GET_CUSTOMER_PARTNO('BARCODE', '{SKUNO}') parent_partno,
                         GET_CUSTOMER_PARTNO('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         b.workorderno task_no,
                         '0' is_head,
                         'WAIT_SEND' send_flag,
                         ---新增segent10欄位來判斷RMA逆向替換標誌:RTN ----20181014--廖東林
                         decode(categoryname, 'RTN', 'RTN', '')
                    FROM r_relation_data a, R_SN b
                   WHERE a.parent = '{parent}'
                     AND a.SN = b.SN
                     AND substr(partno, 1, 2) = 'SN') m
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent   AND */
                   m.son = t.son
                AND m.SN = t.SN
                  --AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')
          UNION
          SELECT *
            FROM (SELECT a.parent,
                         b.mac son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         GET_CUSTOMER_PARTNO('BARCODE', '{SKUNO}') parent_partno,
                         GET_CUSTOMER_PARTNO('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         b.task_no task_no,
                         b.is_head is_head,
                         'WAIT_SEND' send_flag,
                         decode(categoryname, 'RTN', 'RTN', '')
                    FROM r_relation_data a, r_mac_detail_t b
                   WHERE a.parent = '{parent}'
                     AND a.parent = b.box_sn
                        --根據HW IT 陶成,傳送MAC對應關係只需要傳送首MAC       Taylor 2018-06-07
                     AND b.is_head = '1'
                     AND substr(categoryname, 1, 3) = 'MAC'
                     AND substr(partno, 1, 5) != 'SN') m
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent  AND */
                   m.son = t.son
                AND m.SN = t.SN
                  -- AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')  )TB_ALL  ";
                    DB.ExecSqlNoReturn(Strsql, null);

                }
                #endregion

                #region 如果下階有多個MAC,根據PE黃仲略及NPI甘敏康,取第一個

                Strsql = $@"SELECT *
        FROM r_relation_data
       WHERE PARENT IN
             (SELECT son FROM r_relation_data WHERE PARENT = '{parent}')
         AND substr(categoryname, 1, 3) = 'MAC' ";

                dt = DB.ExecSelect(Strsql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Strsql = $@"SELECT DISTINCT PARENT          
          FROM r_relation_data
         WHERE PARENT IN
               (SELECT son FROM r_relation_data WHERE PARENT = '{parent}')
           AND substr(categoryname, 1, 3) = 'MAC'
           AND rownum = 1 ";

                    string sn_tmp=string.Empty;
                    dt = DB.ExecSelect(Strsql).Tables[0];
                    if (dt.Rows.Count > 0)//獲取下階父項
                    {
                        sn_tmp = dt.Rows[0][0].ToString(); 
                    }

                    Strsql = $@"SELECT *
          FROM r_relationdata_detail
         WHERE PARENT = '{sn_tmp}' "; 
                   
                    dt = DB.ExecSelect(Strsql).Tables[0];
                    if (dt.Rows.Count == 0)//下階父項沒有綁定關係
                    {
                        #region 判斷是否RMA
                        if (!WOType.IsTypeInput("RMA", WO.Substring(0, 6), DB))
                        {
                            Strsql = $@"  INSERT INTO r_relationdata_detail
          (
           PARENT,
           son,
           edit_time,
           edit_emp,
           eventpoint,
           parent_partno,
           son_partno,
           categoryname,
           SN,
           task_no,
           is_head,
           send_flag,ID) SELECT TB_ALL.*,'HWT'||SFC.SEQ_R_RELATIONDATA_DETAIL.NEXTVAL AS ID FROM (
          SELECT *
            FROM (SELECT  
                         a.parent,
                         to_char(a.son) son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         get_customer_partno('BARCODE', '{SKUNO}') parent_partno,
                         get_customer_partno('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         CASE
                           WHEN b.po IS NOT NULL THEN
                            b.po
                           ELSE
                            b.wo
                         END task_no,
                         --b.po task_no,
                         '0' is_head,
                         'WAIT_SEND' send_flag
                    FROM r_relation_data a, r_2d_sn_relation b
                   WHERE a.parent = '{parent}'
                     AND a.parent = b.sn
                     AND substr(categoryname, 1, 3) <> 'MAC'
                     AND categoryname <> 'WEIGHT S/N') m --add by hgb 重量信息不需要傳HW 2017.109
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent  AND */ -----廖東林20190422
                  -----PQC 潘建廷要求經與客戶確認，同時存在ASSYP、ASSY工站的產品，ASSYP工站綁定的對應關係不需再以拉手條為父項重傳一次，不然客戶端MES系統抽驗時會校驗失敗
                   m.son = t.son
                AND m.SN = t.SN
                  --  AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')
          UNION
          SELECT *
            FROM (SELECT a.parent,
                         to_char(a.son) son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         get_customer_partno('BARCODE', '{SKUNO}') parent_partno,
                         get_customer_partno('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         CASE
                           WHEN b.po IS NOT NULL THEN
                            b.po
                           ELSE
                            b.wo
                         END task_no,
                         --b.po task_no,
                         '0' is_head,
                         'WAIT_SEND' send_flag
                    FROM r_relation_data a, r_2d_sn_relation b
                   WHERE a.parent = '{parent}'
                     AND a.parent = b.sn
                     AND substr(partno, 1, 2) = 'SN') m
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent  AND*/
                   m.son = t.son
                AND m.SN = t.SN
                  --   AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')
          UNION
          SELECT *
            FROM (SELECT a.parent,
                         b.mac son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         get_customer_partno('BARCODE', '{SKUNO}') parent_partno,
                         get_customer_partno('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         b.task_no task_no,
                         b.is_head is_head,
                         'WAIT_SEND' send_flag
                    FROM r_relation_data a, R_MAC_DETAIL_T b
                   WHERE a.parent = '{parent}'
                     AND a.parent = b.box_sn
                        --根據HW IT 陶成,傳送MAC對應關係只需要傳送首MAC       Taylor 2018-06-07
                     AND b.is_head = '1'
                     AND substr(categoryname, 1, 3) = 'MAC'
                     AND substr(partno, 1, 2) != 'SN') m
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent AND */
                   m.son = t.son
                AND m.SN = t.SN
                  --  AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')  )TB_ALL ";
                            DB.ExecSqlNoReturn(Strsql, null);
                        }
                        else
                        {
                            Strsql = $@"    INSERT INTO r_relationdata_detail
          (PARENT,
           son,
           edit_time,
           edit_emp,
           eventpoint,
           parent_partno,
           son_partno,
           categoryname,
           SN,
           task_no,
           is_head,
           send_flag,
           segent10  ---新增segent10欄位來判斷RMA逆向替換標誌:RTN
          ,ID) SELECT TB_ALL.*,'HWT'||SFC.SEQ_R_RELATIONDATA_DETAIL.NEXTVAL AS ID FROM (
          SELECT PARENT,
                 son,
                 edit_time,
                 edit_emp,
                 eventpoint,
                 parent_partno,
                 partno,
                 categoryname,
                 SN,
                 'EPWENH00008', --modify by hgb 2019.03.11 徐峰要求RMA task_no,傳EPWENH00008
                 is_head,
                 send_flag,
                 decode(categoryname, 'RTN', 'RTN', '') AS rtn_flag --modify by hgb 2019.04.01
            FROM (SELECT decode(categoryname,
                                'RTN',
                                to_char(a.son),
                                a.parent) PARENT,
                         decode(categoryname, 'RTN', PARENT, to_char(a.son)) son,
                         SYSDATE AS edit_time,
                         a.edit_emp,
                         a.eventpoint,
                         GET_CUSTOMER_PARTNO('BARCODE', '{SKUNO}') parent_partno,
                         GET_CUSTOMER_PARTNO('BARCODE', a.partno) partno,
                         a.categoryname,
                         '{SN}' SN,
                         b.workorderno task_no,
                         '0' is_head,
                         'WAIT_SEND' send_flag,
                         ---新增segent10欄位來判斷RMA逆向替換標誌:RTN----20181014--廖東林
                         decode(categoryname, 'RTN', 'RTN', '')
                    FROM r_relation_data a, R_SN b
                   WHERE a.parent = '{parent}'
                     AND a.SN = b.SN
                     AND substr(categoryname, 1, 3) <> 'MAC'
                     AND categoryname <> 'WEIGHT S/N') m --add by hgb 重量信息不需要傳HW 2017.109
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent   AND */
                   m.son = t.son
                AND m.SN = t.SN
                  -- AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')
          UNION
          SELECT *
            FROM (SELECT a.parent,
                         to_char(a.son) son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         GET_CUSTOMER_PARTNO('BARCODE', '{SKUNO}') parent_partno,
                         GET_CUSTOMER_PARTNO('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         b.workorderno task_no,
                         '0' is_head,
                         'WAIT_SEND' send_flag,
                         ---新增segent10欄位來判斷RMA逆向替換標誌:RTN ----20181014--廖東林
                         decode(categoryname, 'RTN', 'RTN', '')
                    FROM r_relation_data a, R_SN b
                   WHERE a.parent = '{parent}'
                     AND a.SN = b.SN
                     AND substr(partno, 1, 2) = 'SN') m
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent   AND */
                   m.son = t.son
                AND m.SN = t.SN
                  --AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')
          UNION
          SELECT *
            FROM (SELECT a.parent,
                         b.mac son,
                         SYSDATE,
                         a.edit_emp,
                         a.eventpoint,
                         GET_CUSTOMER_PARTNO('BARCODE', '{SKUNO}') parent_partno,
                         GET_CUSTOMER_PARTNO('BARCODE', a.partno),
                         a.categoryname,
                         '{SN}' SN,
                         b.task_no task_no,
                         b.is_head is_head,
                         'WAIT_SEND' send_flag,
                         decode(categoryname, 'RTN', 'RTN', '')
                    FROM r_relation_data a, r_mac_detail_t b
                   WHERE a.parent = '{parent}'
                     AND a.parent = b.box_sn
                        --根據HW IT 陶成,傳送MAC對應關係只需要傳送首MAC       Taylor 2018-06-07
                     AND b.is_head = '1'
                     AND substr(categoryname, 1, 3) = 'MAC'
                     AND substr(partno, 1, 5) != 'SN') m
           WHERE NOT EXISTS (SELECT 1
                    FROM r_relationdata_detail t
                   WHERE /*m.parent = t.parent  AND */
                   m.son = t.son
                AND m.SN = t.SN
                  -- AND m.task_no = t.task_no
                AND m.eventpoint = t.eventpoint
                AND t.send_flag <> 'DELETE')  )TB_ALL  ";
                            DB.ExecSqlNoReturn(Strsql, null);

                        }
                        #endregion
                    }
                }

                #endregion
            }

            #endregion
        }
    }




    public class Row_R_RELATIONDATA_DETAIL : DataObjectBase
    {
        public Row_R_RELATIONDATA_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_RELATIONDATA_DETAIL GetDataObject()
        {
            R_RELATIONDATA_DETAIL DataObject = new R_RELATIONDATA_DETAIL();
            DataObject.ID = this.ID;
            DataObject.PARENT = this.PARENT;
            DataObject.SON = this.SON;
            DataObject.EVENTPOINT = this.EVENTPOINT;
            DataObject.PARENT_PARTNO = this.PARENT_PARTNO;
            DataObject.SON_PARTNO = this.SON_PARTNO;
            DataObject.CATEGORYNAME = this.CATEGORYNAME;
            DataObject.SN = this.SN;
            DataObject.KP_VERSION = this.KP_VERSION;
            DataObject.CUSTOMER_KP_NO = this.CUSTOMER_KP_NO;
            DataObject.CUSTOMER_KP_NO_VER = this.CUSTOMER_KP_NO_VER;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.IS_HEAD = this.IS_HEAD;
            DataObject.SEND_FLAG = this.SEND_FLAG;
            DataObject.SEND_TIME = this.SEND_TIME;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.AB_CODE = this.AB_CODE;
            DataObject.SEGENT12 = this.SEGENT12;
            DataObject.SEGENT13 = this.SEGENT13;
            DataObject.SEGENT10 = this.SEGENT10;
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
        public string PARENT
        {
            get
            {
                return (string)this["PARENT"];
            }
            set
            {
                this["PARENT"] = value;
            }
        }
        public string SON
        {
            get
            {
                return (string)this["SON"];
            }
            set
            {
                this["SON"] = value;
            }
        }
        public string EVENTPOINT
        {
            get
            {
                return (string)this["EVENTPOINT"];
            }
            set
            {
                this["EVENTPOINT"] = value;
            }
        }
        public string PARENT_PARTNO
        {
            get
            {
                return (string)this["PARENT_PARTNO"];
            }
            set
            {
                this["PARENT_PARTNO"] = value;
            }
        }
        public string SON_PARTNO
        {
            get
            {
                return (string)this["SON_PARTNO"];
            }
            set
            {
                this["SON_PARTNO"] = value;
            }
        }
        public string CATEGORYNAME
        {
            get
            {
                return (string)this["CATEGORYNAME"];
            }
            set
            {
                this["CATEGORYNAME"] = value;
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
        public string KP_VERSION
        {
            get
            {
                return (string)this["KP_VERSION"];
            }
            set
            {
                this["KP_VERSION"] = value;
            }
        }
        public string CUSTOMER_KP_NO
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO"];
            }
            set
            {
                this["CUSTOMER_KP_NO"] = value;
            }
        }
        public string CUSTOMER_KP_NO_VER
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO_VER"];
            }
            set
            {
                this["CUSTOMER_KP_NO_VER"] = value;
            }
        }
        public string TASK_NO
        {
            get
            {
                return (string)this["TASK_NO"];
            }
            set
            {
                this["TASK_NO"] = value;
            }
        }
        public string IS_HEAD
        {
            get
            {
                return (string)this["IS_HEAD"];
            }
            set
            {
                this["IS_HEAD"] = value;
            }
        }
        public string SEND_FLAG
        {
            get
            {
                return (string)this["SEND_FLAG"];
            }
            set
            {
                this["SEND_FLAG"] = value;
            }
        }
        public DateTime? SEND_TIME
        {
            get
            {
                return (DateTime?)this["SEND_TIME"];
            }
            set
            {
                this["SEND_TIME"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string AB_CODE
        {
            get
            {
                return (string)this["AB_CODE"];
            }
            set
            {
                this["AB_CODE"] = value;
            }
        }
        public string SEGENT12
        {
            get
            {
                return (string)this["SEGENT12"];
            }
            set
            {
                this["SEGENT12"] = value;
            }
        }
        public string SEGENT13
        {
            get
            {
                return (string)this["SEGENT13"];
            }
            set
            {
                this["SEGENT13"] = value;
            }
        }
        public string SEGENT10
        {
            get
            {
                return (string)this["SEGENT10"];
            }
            set
            {
                this["SEGENT10"] = value;
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
    public class R_RELATIONDATA_DETAIL
    {
        public string ID { get; set; }
        public string PARENT { get; set; }
        public string SON { get; set; }
        public string EVENTPOINT { get; set; }
        public string PARENT_PARTNO { get; set; }
        public string SON_PARTNO { get; set; }
        public string CATEGORYNAME { get; set; }
        public string SN { get; set; }
        public string KP_VERSION { get; set; }
        public string CUSTOMER_KP_NO { get; set; }
        public string CUSTOMER_KP_NO_VER { get; set; }
        public string TASK_NO { get; set; }
        public string IS_HEAD { get; set; }
        public string SEND_FLAG { get; set; }
        public DateTime? SEND_TIME { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string AB_CODE { get; set; }
        public string SEGENT12 { get; set; }
        public string SEGENT13 { get; set; }
        public string SEGENT10 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}