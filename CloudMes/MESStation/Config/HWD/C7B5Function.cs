using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.HWD;
using MESDBHelper;
using MESDataObject;


namespace MESStation.Config.HWD
{
    public class C7B5Function
    {
        public static object lockObj = new object();

        /// <summary>
        /// 用於UPDATE r_7b5_wo_temp表中的creat_wo_qty suggest_qty
        /// 對應AP.check_7b5_xmldata_sp 中 var_type = 'COUNT_QTY'
        /// </summary>
        /// <param name="sfcdb"></param>
        public static void CountQty(OleExec sfcdb, DB_TYPE_ENUM dbtype, ref string msg)
        {
            MESDataObject.Module.HWD.T_R_7B5_WO_TEMP TRWT = new MESDataObject.Module.HWD.T_R_7B5_WO_TEMP(sfcdb, dbtype);
            MESDataObject.Module.HWD.T_C_SKU_7B5_CONFIG TCSC = new MESDataObject.Module.HWD.T_C_SKU_7B5_CONFIG(sfcdb, dbtype);
            MESDataObject.Module.HWD.C_SKU_7B5_CONFIG skuConfig = null;
            DataTable dtWOTemp = TRWT.GetRecently200(sfcdb);
            string v_task_no = "", hh_skuno = "", sql = "";
            double creat_qty, task_qty, suggest_qty;
            if (dtWOTemp != null && dtWOTemp.Rows.Count > 0)
            {
                foreach (DataRow row in dtWOTemp.Rows)
                {
                    try
                    {
                        v_task_no = row["v_task_no"].ToString();
                        hh_skuno = row["hh_item"].ToString();
                        task_qty = Convert.ToDouble(row["qty"].ToString());

                        sql = $@"SELECT SUM (wo_qty) as creat_qty  FROM(SELECT a.wo_qty FROM r_7b5_wo a  WHERE a.v_task_no = '{v_task_no}'
                            AND a.hh_item = '{hh_skuno}'   AND a.sap_wo IS NOT NULL     AND a.delete_flag <> 'Y'  
                            AND NOT EXISTS(SELECT *  FROM r_wo_base b  WHERE a.sap_wo = b.workorderno)
                            UNION ALL
                            SELECT b.workorder_qty FROM r_7b5_wo a, r_wo_base b WHERE a.v_task_no = '{v_task_no}' AND a.hh_item = '{hh_skuno}'
                            AND a.sap_wo IS NOT NULL AND a.sap_wo = b.workorderno  AND a.delete_flag <> 'Y'
                            UNION ALL  SELECT 0 FROM DUAL)";
                        creat_qty = Convert.ToDouble(sfcdb.ExecSelectOneValue(sql));

                        skuConfig = TCSC.GetListByTypeAndSkuno(sfcdb, "UPD", hh_skuno);
                        if (skuConfig != null)
                        {
                            if ((task_qty - creat_qty) > (skuConfig.UPD + skuConfig.UPD * 0.1))
                            {
                                suggest_qty = (double)skuConfig.UPD;
                            }
                            else
                            {
                                suggest_qty = task_qty - creat_qty;
                            }
                        }
                        else
                        {
                            suggest_qty = task_qty - creat_qty;
                        }

                        int result = sfcdb.ORM.Updateable<MESDataObject.Module.HWD.R_7B5_WO_TEMP>()
                            .UpdateColumns(r => new MESDataObject.Module.HWD.R_7B5_WO_TEMP { CREAT_WO_QTY = creat_qty, SUGGEST_QTY = suggest_qty })
                           .Where(r => r.V_TASK_NO == v_task_no && r.HH_ITEM == hh_skuno).ExecuteCommand();
                        if (result <= 0)
                        {
                            throw new Exception("UPDATE R_7B5_WO_TEMP ERROR!");
                        }
                    }
                    catch (Exception ex)
                    {
                        msg = msg + v_task_no + "," + hh_skuno + "," + ex.Message + ";";                        
                    }
                }
            }
        }

        public static void CountQtyByVTask(OleExec sfcdb, DB_TYPE_ENUM dbtype, string in_v_task_no,ref string msg)
        {
            MESDataObject.Module.HWD.T_R_7B5_WO_TEMP TRWT = new MESDataObject.Module.HWD.T_R_7B5_WO_TEMP(sfcdb, dbtype);
            MESDataObject.Module.HWD.T_C_SKU_7B5_CONFIG TCSC = new MESDataObject.Module.HWD.T_C_SKU_7B5_CONFIG(sfcdb, dbtype);
            MESDataObject.Module.HWD.C_SKU_7B5_CONFIG skuConfig = null;
            DataTable dtWOTemp = TRWT.GetRecentlyByTask(sfcdb, in_v_task_no);
            string v_task_no = "", hh_skuno = "", sql = "";
            double creat_qty, task_qty, suggest_qty;
            if (dtWOTemp != null && dtWOTemp.Rows.Count > 0)
            {
                try
                {
                    foreach (DataRow row in dtWOTemp.Rows)
                    {
                        v_task_no = row["v_task_no"].ToString();
                        hh_skuno = row["hh_item"].ToString();
                        task_qty = Convert.ToDouble(row["qty"].ToString());

                        sql = $@"SELECT SUM (wo_qty) as creat_qty  FROM(SELECT a.wo_qty FROM r_7b5_wo a  WHERE a.v_task_no = '{v_task_no}'
                            AND a.hh_item = '{hh_skuno}'   AND a.sap_wo IS NOT NULL     AND a.delete_flag <> 'Y'  
                            AND NOT EXISTS(SELECT *  FROM r_wo_base b  WHERE a.sap_wo = b.workorderno)
                            UNION ALL
                            SELECT b.workorder_qty FROM r_7b5_wo a, r_wo_base b WHERE a.v_task_no = '{v_task_no}' AND a.hh_item = '{hh_skuno}'
                            AND a.sap_wo IS NOT NULL AND a.sap_wo = b.workorderno  AND a.delete_flag <> 'Y'
                            UNION ALL  SELECT 0 FROM DUAL)";
                        creat_qty = Convert.ToDouble(sfcdb.ExecSelectOneValue(sql));

                        skuConfig = TCSC.GetListByTypeAndSkuno(sfcdb, "UPD", hh_skuno);
                        if (skuConfig != null)
                        {
                            if ((task_qty - creat_qty) > (skuConfig.UPD + skuConfig.UPD * 0.1))
                            {
                                suggest_qty = (double)skuConfig.UPD;
                            }
                            else
                            {
                                suggest_qty = task_qty - creat_qty;
                            }
                        }
                        else
                        {
                            suggest_qty = task_qty - creat_qty;
                        }

                        int result = sfcdb.ORM.Updateable<MESDataObject.Module.HWD.R_7B5_WO_TEMP>()
                            .UpdateColumns(r => new MESDataObject.Module.HWD.R_7B5_WO_TEMP { CREAT_WO_QTY = creat_qty, SUGGEST_QTY = suggest_qty })
                           .Where(r => r.V_TASK_NO == v_task_no && r.HH_ITEM == hh_skuno).ExecuteCommand();
                        if (result <= 0)
                        {
                            throw new Exception("UPDATE R_7B5_WO_TEMP ERROR!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = msg + v_task_no + "," + hh_skuno + "," + ex.Message + ";";
                    //throw ex;
                }
            }

        }

        /// <summary>
        ///  對應AP.check_7b5_xmldata_sp 中 var_type = 'SHIP_PLAN'
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="dbtype"></param>
        /// <param name="input_task_no"></param>
        /// <param name="input_hh_itme"></param>
        /// <param name="input_plan_qty"></param>
        /// <param name="input_with_sub_flag"></param>
        /// <param name="user"></param>
        /// <param name="remark"></param>
        public static void ShipPlan(OleExec sfcdb, DB_TYPE_ENUM dbtype, string input_task_no,string input_hh_itme, double input_plan_qty, string input_with_sub_flag,string user,string remark)
        {
            try
            {
                string lotno, sql, hh_item, hw_item, sub_task_no, sub_hh_item, sub_hw_item;
                double plan_qty_temp, price, sub_task_ext;
                int result;

                string subSkuno_2;

                DataTable dt = new DataTable();
                MESDataObject.Module.HWD.T_R_7B5_PO TRP = new MESDataObject.Module.HWD.T_R_7B5_PO(sfcdb, dbtype);
                MESDataObject.Module.HWD.T_R_7B5_SHIP TRS = new MESDataObject.Module.HWD.T_R_7B5_SHIP(sfcdb, dbtype);
                MESDataObject.Module.HWD.T_C_SKU_LINK_7B5 TCSL = new MESDataObject.Module.HWD.T_C_SKU_LINK_7B5(sfcdb, dbtype);
                MESDataObject.Module.HWD.T_R_7B5_SHIP_DATA TRSD = new MESDataObject.Module.HWD.T_R_7B5_SHIP_DATA(sfcdb, dbtype);
                MESDataObject.Module.HWD.R_7B5_SHIP_DATA objShipData;

                if (!TRP.TaskIsExist(sfcdb, input_task_no))
                {
                    throw new Exception(input_task_no + " not setting!");
                }

                sql = $@"SELECT  *  FROM r_7b5_ship_data WHERE SUBSTR (lotno, 3, 6) = TO_CHAR (SYSDATE, 'YYMMDD')";
                dt = sfcdb.ExecSelect(sql, null).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    sql = $@" SELECT 'ZF' || (MAX (SUBSTR (lotno, 3, 9)) + 1) as lotno FROM r_7b5_ship_data WHERE SUBSTR (lotno, 3, 6) = TO_CHAR (SYSDATE, 'YYMMDD')";
                }
                else
                {
                    sql = $@" SELECT 'ZF' || TO_CHAR (SYSDATE, 'YYMMDD') || '001' as lotno FROM DUAL";
                }
                lotno = sfcdb.ExecSelectOneValue(sql).ToString();

                List<MESDataObject.Module.HWD.R_7B5_SHIP> listShip = TRS.GetObjByTaskAndItme(sfcdb, input_task_no, input_hh_itme);
                if (listShip.Count != 1)
                {
                    throw new Exception("GET HH_ITME,HW_ITME ERROR !");
                }
                hh_item = listShip[0].HH_ITEM;
                hw_item = listShip[0].HW_ITEM;

                List<string> listSubSkuno = new List<string>();
                if (TCSL.IsExistBySkuAndSeq(sfcdb, hh_item, 8) && input_with_sub_flag == "1")
                {
                    if (input_task_no.StartsWith("T"))
                    {
                        sql = $@" SELECT *
                                      FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                                     WHERE b.skuno = '{hh_item}'
                                       AND b.seqno = 8
                                       AND a.hh_item = b.subskuno
                                       AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                       AND a.task_no = c.task_no
                                       AND c.sap_flag = 'Y'
                                       AND LEFT (c.po_no, 1) IN ('M', '8')";
                        dt = sfcdb.ExecSelect(sql, null).Tables[0];
                        //dt = GetShipLinkPo(sfcdb, hh_item, input_task_no);
                        if (dt != null && dt.Rows.Count == 0)
                        {
                            throw new Exception(hh_item + "error in ShipPlan r_7b5_ship,c_sku_link_7b5,r_7b5_po 01!");
                        }
                        sql = $@"  SELECT *
                                      FROM (SELECT   b.subskuno,
                                                     SUM (a.task_qty - a.total_plan_qty - a.buffer_qty
                                                         ) total_ext_qty
                                                FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                                               WHERE b.skuno = '{hh_item}'
                                                 AND b.seqno = 8
                                                 AND a.hh_item = b.subskuno
                                                 AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                 AND a.task_no = c.task_no
                                                 AND c.sap_flag = 'Y'
                                                 AND LEFT (c.po_no, 1) IN ('M', '8')
                                            GROUP BY b.subskuno) m
                                     WHERE m.total_ext_qty < {input_plan_qty}";
                        //dt = GetShipLinkPoTotalQty(sfcdb, input_task_no, hh_item, input_plan_qty);
                        dt = sfcdb.ExecSelect(sql, null).Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            throw new Exception(hh_item + "error in ShipPlan r_7b5_ship,c_sku_link_7b5,r_7b5_po 02 !");
                        }
                        else
                        {
                            listSubSkuno = TCSL.GetSubSkuListBySeq(sfcdb, hh_item);
                            foreach (string subSkuno in listSubSkuno)
                            {
                                plan_qty_temp = 0;                               
                                sql = $@"SELECT * FROM (SELECT   b.price,
                                                           SUM (  a.task_qty
                                                                - a.total_plan_qty
                                                                - a.buffer_qty
                                                               ) ext_qty
                                                      FROM r_7b5_ship a, r_7b5_po b
                                                     WHERE a.hh_item = '{subSkuno}'
                                                       AND a.task_no = b.task_no
                                                       AND b.sap_flag = 'Y'
                                                       AND LEFT (b.po_no, 1) IN ('M', '8')
                                                       AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                  GROUP BY b.price) m
                                           WHERE m.ext_qty >= {input_plan_qty}";
                                dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                //dt = GetShipPOExtQty(sfcdb, input_task_no, subSkuno, input_plan_qty);
                                if (dt != null && dt.Rows.Count == 0)
                                {
                                    throw new Exception(subSkuno + " error in ShipPlan r_7b5_ship,r_7b5_po 03!");
                                }
                                sql = $@" SELECT n.price
                                            FROM (SELECT   m.price
                                                      FROM (SELECT   b.price,
                                                                     SUM (  a.task_qty
                                                                          - a.total_plan_qty
                                                                          - a.buffer_qty
                                                                         ) ext_qty
                                                                FROM r_7b5_ship a, r_7b5_po b
                                                               WHERE a.hh_item = '{subSkuno}'
                                                                 AND a.task_no = b.task_no
                                                                 AND b.sap_flag = 'Y'
                                                                 AND LEFT (b.po_no, 1) IN ('M', '8')
                                                                 AND a.task_qty >a.total_plan_qty + a.buffer_qty
                                                            GROUP BY b.price) m,
                                                           r_7b5_ship c,
                                                           r_7b5_po d
                                                     WHERE m.ext_qty >= 20
                                                       AND c.hh_item =  '{subSkuno}'
                                                       AND c.task_no = d.task_no
                                                       AND d.sap_flag = 'Y'
                                                       AND LEFT (d.po_no, 1) IN ('M', '8')
                                                       AND c.task_qty > c.total_plan_qty + c.buffer_qty
                                                       AND d.price = m.price
                                                  ORDER BY c.receive_date) n
                                           WHERE ROWNUM = 1";
                                dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                if (dt != null && dt.Rows.Count == 0)
                                {
                                    throw new Exception(subSkuno + " Get Price Error! 04");
                                }
                                //price = GetShipPOPriceStartT(sfcdb, subSkuno);
                                price = Convert.ToDouble(dt.Rows[0][0].ToString());
                                while (plan_qty_temp < input_plan_qty)
                                {
                                    sql = $@"SELECT a.task_no as sub_task_no,
                                                    (a.task_qty - a.total_plan_qty - a.buffer_qty) as sub_task_ext,
                                                    a.hh_item as sub_hh_item, a.hw_item as sub_hw_item                      
                                               FROM r_7b5_ship a, r_7b5_po b
                                              WHERE a.hh_item = '{subSkuno}'
                                                AND a.task_no = b.task_no
                                                AND b.sap_flag = 'Y'
                                                AND b.price = var_price
                                                AND LEFT (b.po_no, 1) IN ('M', '8')
                                                AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                AND a.receive_date =
                                                       (SELECT MIN (a.receive_date)
                                                          FROM r_7b5_ship a, r_7b5_po b
                                                         WHERE a.hh_item = '{subSkuno}'
                                                           AND a.task_no = b.task_no
                                                           AND b.sap_flag = 'Y'
                                                           AND b.price = {price}
                                                           AND LEFT (b.po_no, 1) IN ('M', '8')
                                                           AND a.task_qty >a.total_plan_qty + a.buffer_qty)
                                                AND ROWNUM = 1";
                                    //dt = GetShipPOTaskQtyByPrice(sfcdb, input_task_no, subSkuno, price);
                                    dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                    if (dt != null && dt.Rows.Count == 0)
                                    {
                                        throw new Exception(subSkuno + " Get sub_task_no,sub_task_ext,sub_hh_item,sub_hw_item Error 05!");
                                    }

                                    sub_task_no = dt.Rows[0]["sub_task_no"].ToString();
                                    sub_hh_item = dt.Rows[0]["sub_hh_item"].ToString();
                                    sub_hw_item = dt.Rows[0]["sub_hw_item"].ToString();
                                    sub_task_ext = (double)dt.Rows[0]["sub_task_ext"];

                                    objShipData = new MESDataObject.Module.HWD.R_7B5_SHIP_DATA();
                                    objShipData.LOTNO = lotno;
                                    objShipData.TASK_NO = sub_task_no;
                                    objShipData.HH_ITEM = sub_hh_item;
                                    objShipData.HW_ITEM = sub_hw_item;
                                    objShipData.REMARK = "單板走賬至" + input_task_no;
                                    objShipData.LASTEDITBY = user;
                                    objShipData.LASTEDITDT = TRSD.GetDBDateTime(sfcdb);
                                    objShipData.DELETE_FLAG = "N";
                                    objShipData.SAP_FLAG = "N";

                                    if (sub_task_ext >= input_plan_qty - plan_qty_temp)
                                    {
                                        objShipData.QTY = input_plan_qty - plan_qty_temp;
                                        result = TRSD.SaveShipData(sfcdb, objShipData);
                                        if (result <= 0)
                                        {
                                            throw new Exception("Insert into r_7b5_ship_data Error!" + sub_task_no + "," + sub_hh_item + "," + sub_hw_item);
                                        }
                                        result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, sub_task_no, sub_hh_item, input_plan_qty - plan_qty_temp);
                                        if (result <= 0)
                                        {
                                            throw new Exception("Update r_7b5_ship Error!" + sub_task_no + "," + sub_hh_item);
                                        }
                                        plan_qty_temp = input_plan_qty;
                                    }
                                    else
                                    {
                                        objShipData.QTY = sub_task_ext;
                                        result = TRSD.SaveShipData(sfcdb, objShipData);
                                        if (result <= 0)
                                        {
                                            throw new Exception("Insert into r_7b5_ship_data Error!" + sub_task_no + "," + sub_hh_item + "," + sub_hw_item);
                                        }
                                        result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, sub_task_no, sub_hh_item, sub_task_ext);
                                        if (result <= 0)
                                        {
                                            throw new Exception("Update r_7b5_ship Error!" + sub_task_no + "," + sub_hh_item);
                                        }
                                        plan_qty_temp = plan_qty_temp + sub_task_ext;
                                    }
                                }

                                if (TCSL.IsExistBySkuAndSeq(sfcdb, subSkuno, 8))
                                {
                                    sql = $@"SELECT *
                                               FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                                              WHERE b.skuno = '{subSkuno}'
                                                AND b.seqno = 8
                                                AND a.hh_item = b.subskuno
                                                AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                AND a.task_no = c.task_no
                                                AND c.sap_flag = 'Y'
                                                AND LEFT (c.po_no, 1) IN ('M', '8')";
                                    //dt = GetShipLinkPo(sfcdb, subSkuno, input_task_no);
                                    dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                    if (dt != null && dt.Rows.Count == 0)
                                    {
                                        throw new Exception(subSkuno + "error in ShipPlan r_7b5_ship,c_sku_link_7b5,r_7b5_po 06!");
                                    }
                                    sql = $@"   SELECT *
                                                   FROM (SELECT   b.subskuno,
                                                                  SUM (  a.task_qty
                                                                       - a.total_plan_qty
                                                                       - a.buffer_qty
                                                                      ) total_ext_qty

                                                             FROM r_7b5_ship a,
                                                                  c_sku_link_7b5 b,
                                                                  r_7b5_po c
                                                            WHERE b.skuno = '{subSkuno}'
                                                              AND b.seqno = 8
                                                              AND a.hh_item = b.subskuno
                                                              AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                              AND a.task_no = c.task_no
                                                              AND c.sap_flag = 'Y'
                                                              AND LEFT (c.po_no, 1) IN ('M', '8')
                                                         GROUP BY b.subskuno) m
                                                  WHERE m.total_ext_qty < {input_plan_qty} ";
                                    dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                    //dt = GetShipLinkPoTotalQty(sfcdb, input_task_no, subSkuno, input_plan_qty);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        throw new Exception(subSkuno + "error in ShipPlan r_7b5_ship,c_sku_link_7b5,r_7b5_po 07 !");
                                    }
                                    subSkuno_2 = TCSL.GetSubSkuListBySeq(sfcdb, subSkuno).FirstOrDefault();
                                    plan_qty_temp = 0;
                                    while (plan_qty_temp < input_plan_qty)
                                    {
                                        sql = $@"SELECT a.task_no as sub_task_no,
                                                      (a.task_qty - a.total_plan_qty - a.buffer_qty) as sub_task_ext,
                                                      a.hh_item as sub_hh_item, a.hw_item  as sub_hw_item  
                                                 FROM r_7b5_ship a, r_7b5_po b
                                                WHERE a.hh_item = '{subSkuno_2}'
                                                  AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                  AND a.task_no = b.task_no
                                                  AND b.sap_flag = 'Y'
                                                  AND LEFT (b.po_no, 1) IN ('M', '8')
                                                  AND a.receive_date =
                                                         (SELECT MIN (a.receive_date)
                                                            FROM r_7b5_ship a, r_7b5_po b
                                                           WHERE a.hh_item = '{subSkuno_2}'
                                                             AND a.task_no = b.task_no
                                                             AND b.sap_flag = 'Y'
                                                             AND LEFT (b.po_no, 1) IN ('M', '8')
                                                             AND a.task_qty >a.total_plan_qty+ a.buffer_qty)
                                                  AND ROWNUM = 1";
                                        dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                        //dt = GetShipPOTaskQtyBy(sfcdb, input_task_no, subSkuno_2);
                                        sub_task_no = dt.Rows[0]["sub_task_no"].ToString();
                                        sub_hh_item = dt.Rows[0]["sub_hh_item"].ToString();
                                        sub_hw_item = dt.Rows[0]["sub_hw_item"].ToString();
                                        sub_task_ext = Convert.ToDouble(dt.Rows[0]["sub_task_ext"].ToString());

                                        objShipData = new MESDataObject.Module.HWD.R_7B5_SHIP_DATA();
                                        objShipData.LOTNO = lotno;
                                        objShipData.TASK_NO = sub_task_no;
                                        objShipData.HH_ITEM = sub_hh_item;
                                        objShipData.HW_ITEM = sub_hw_item;
                                        objShipData.REMARK = "單板走賬至" + input_task_no;
                                        objShipData.LASTEDITBY = user;
                                        objShipData.LASTEDITDT = TRSD.GetDBDateTime(sfcdb);
                                        objShipData.DELETE_FLAG = "N";
                                        objShipData.SAP_FLAG = "N";

                                        if (sub_task_ext >= input_plan_qty - plan_qty_temp)
                                        {
                                            objShipData.QTY = input_plan_qty - plan_qty_temp;
                                            result = TRSD.SaveShipData(sfcdb, objShipData);
                                            if (result <= 0)
                                            {
                                                throw new Exception("Insert into r_7b5_ship_data Error!" + sub_task_no + "," + sub_hh_item + "," + sub_hw_item);
                                            }
                                            result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, sub_task_no, sub_hh_item, input_plan_qty - plan_qty_temp);
                                            if (result <= 0)
                                            {
                                                throw new Exception("Update r_7b5_ship Error!" + sub_task_no + "," + sub_hh_item);
                                            }
                                            plan_qty_temp = input_plan_qty;
                                        }
                                        else
                                        {
                                            objShipData.QTY = sub_task_ext;
                                            result = TRSD.SaveShipData(sfcdb, objShipData);
                                            if (result <= 0)
                                            {
                                                throw new Exception("Insert into r_7b5_ship_data Error!" + sub_task_no + "," + sub_hh_item + "," + sub_hw_item);
                                            }
                                            result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, sub_task_no, sub_hh_item, sub_task_ext);
                                            if (result <= 0)
                                            {
                                                throw new Exception("Update r_7b5_ship Error!" + sub_task_no + "," + sub_hh_item);
                                            }
                                            plan_qty_temp = plan_qty_temp + sub_task_ext;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //dt = GetShipLinkPo(sfcdb, hh_item, input_task_no);
                        sql = $@"SELECT * FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                                 WHERE b.skuno = '{hh_item}'
                                   AND b.seqno = 8
                                   AND a.hh_item = b.subskuno
                                   AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                   AND a.task_no = c.task_no
                                   AND c.sap_flag = 'Y'
                                   AND LEFT(c.po_no, 1) IN('D', '9')";
                        dt = sfcdb.ExecSelect(sql, null).Tables[0];
                        if (dt != null && dt.Rows.Count == 0)
                        {
                            throw new Exception(hh_item + "error in ShipPlan r_7b5_ship,c_sku_link_7b5,r_7b5_po 08!");
                        }

                        //dt = GetShipLinkPoTotalQty(sfcdb, input_task_no, hh_item, input_plan_qty);
                        sql = $@" SELECT *  FROM (SELECT   b.subskuno,
                                                 SUM (a.task_qty - a.total_plan_qty - a.buffer_qty
                                                     ) total_ext_qty
                                            FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                                           WHERE b.skuno = '{hh_item}'
                                             AND b.seqno = 8
                                             AND a.hh_item = b.subskuno
                                             AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                             AND a.task_no = c.task_no
                                             AND c.sap_flag = 'Y'
                                             AND LEFT (c.po_no, 1) IN ('D', '9')
                                        GROUP BY b.subskuno) m
                                 WHERE m.total_ext_qty < {input_plan_qty}";
                        dt = sfcdb.ExecSelect(sql, null).Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            throw new Exception(hh_item + "error in ShipPlan r_7b5_ship,c_sku_link_7b5,r_7b5_po 09 !");
                        }
                        else
                        {
                            listSubSkuno = TCSL.GetSubSkuListBySeq(sfcdb, hh_item);
                            foreach (string subSkuno in listSubSkuno)
                            {
                                plan_qty_temp = 0;
                                //dt = GetShipPOExtQty(sfcdb, input_task_no, subSkuno, input_plan_qty);
                                sql = $@"SELECT *   FROM (SELECT   b.price,
                                   SUM (  a.task_qty
                                        - a.total_plan_qty
                                        - a.buffer_qty
                                       ) ext_qty
                              FROM r_7b5_ship a, r_7b5_po b
                             WHERE a.hh_item = '{subSkuno}'
                               AND a.task_no = b.task_no
                               AND b.sap_flag = 'Y'
                               AND LEFT (b.po_no, 1) IN ('D', '9')
                               AND a.task_qty > a.total_plan_qty + a.buffer_qty
                          GROUP BY b.price) m WHERE m.ext_qty >= {input_plan_qty}";
                                dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                if (dt != null && dt.Rows.Count == 0)
                                {
                                    throw new Exception(subSkuno + " error in ShipPlan r_7b5_ship,r_7b5_po 10!");
                                }
                                sql = $@"SELECT n.price                    
                                            FROM (SELECT   m.price
                                                      FROM (SELECT   b.price,
                                                                     SUM (  a.task_qty
                                                                          - a.total_plan_qty
                                                                          - a.buffer_qty
                                                                         ) ext_qty
                                                                FROM r_7b5_ship a, r_7b5_po b
                                                               WHERE a.hh_item = '{subSkuno}'
                                                                 AND a.task_no = b.task_no
                                                                 AND b.sap_flag = 'Y'
                                                                 AND LEFT (b.po_no, 1) IN ('D', '9')
                                                                 AND a.task_qty > a.total_plan_qty+ a.buffer_qty
                                                            GROUP BY b.price) m,
                                                           r_7b5_ship c,
                                                           r_7b5_po d
                                                     WHERE m.ext_qty >= 1
                                                       AND c.hh_item = '{subSkuno}'
                                                       AND c.task_no = d.task_no
                                                       AND d.sap_flag = 'Y'
                                                       AND LEFT (d.po_no, 1) IN ('D', '9')
                                                       AND c.task_qty >c.total_plan_qty + c.buffer_qty
                                                       AND d.price = m.price
                                                  ORDER BY c.receive_date) n
                                           WHERE ROWNUM = 1";
                                dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                if (dt != null && dt.Rows.Count == 0)
                                {
                                    //throw new Exception(subSkuno + "獲取價格勢失敗！");
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814120044", new string[] { subSkuno }));
                                }
                                //price = GetShipPOPrice(sfcdb, subSkuno);                               
                                price = Convert.ToDouble(dt.Rows[0][0].ToString());

                                while (plan_qty_temp < input_plan_qty)
                                {
                                    //dt = GetShipPOTaskQtyByPrice(sfcdb, input_task_no, subSkuno, price);
                                    sql = $@" SELECT a.task_no as sub_task_no,
                                            (a.task_qty - a.total_plan_qty - a.buffer_qty) as sub_task_ext,
                                            a.hh_item as sub_hh_item, a.hw_item as sub_hw_item
                                       FROM r_7b5_ship a, r_7b5_po b
                                      WHERE a.hh_item = '{subSkuno}'
                                        AND a.task_no = b.task_no
                                        AND b.sap_flag = 'Y'
                                        AND b.price = {price}
                                        AND LEFT (b.po_no, 1) IN ('D', '9')
                                        AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                        AND a.receive_date =
                                               (SELECT MIN (a.receive_date)
                                                  FROM r_7b5_ship a, r_7b5_po b
                                                 WHERE a.hh_item = '{subSkuno}'
                                                   AND a.task_no = b.task_no
                                                   AND b.sap_flag = 'Y'
                                                   AND b.price = {price}
                                                   AND LEFT (b.po_no, 1) IN ('D', '9')
                                                   AND a.task_qty >a.total_plan_qty + a.buffer_qty)
                                        AND ROWNUM = 1";
                                    dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                    if (dt != null && dt.Rows.Count == 0)
                                    {
                                        throw new Exception(subSkuno + " Get sub_task_no,sub_task_ext,sub_hh_item,sub_hw_item Error 11!");
                                    }

                                    sub_task_no = dt.Rows[0]["sub_task_no"].ToString();
                                    sub_hh_item = dt.Rows[0]["sub_hh_item"].ToString();
                                    sub_hw_item = dt.Rows[0]["sub_hw_item"].ToString();
                                    sub_task_ext = Convert.ToDouble(dt.Rows[0]["sub_task_ext"].ToString());

                                    objShipData = new MESDataObject.Module.HWD.R_7B5_SHIP_DATA();
                                    objShipData.LOTNO = lotno;
                                    objShipData.TASK_NO = sub_task_no;
                                    objShipData.HH_ITEM = sub_hh_item;
                                    objShipData.HW_ITEM = sub_hw_item;
                                    objShipData.REMARK = "單板走賬至" + input_task_no;
                                    objShipData.LASTEDITBY = user;
                                    objShipData.LASTEDITDT = TRSD.GetDBDateTime(sfcdb);
                                    objShipData.DELETE_FLAG = "N";
                                    objShipData.SAP_FLAG = "N";

                                    if (sub_task_ext >= input_plan_qty - plan_qty_temp)
                                    {
                                        objShipData.QTY = input_plan_qty - plan_qty_temp;
                                        result = TRSD.SaveShipData(sfcdb, objShipData);
                                        if (result <= 0)
                                        {
                                            throw new Exception("Insert into r_7b5_ship_data Error!" + sub_task_no + "," + sub_hh_item + "," + sub_hw_item);
                                        }
                                        result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, sub_task_no, sub_hh_item, input_plan_qty - plan_qty_temp);
                                        if (result <= 0)
                                        {
                                            throw new Exception("Update r_7b5_ship Error!" + sub_task_no + "," + sub_hh_item);
                                        }
                                        plan_qty_temp = input_plan_qty;
                                    }
                                    else
                                    {
                                        objShipData.QTY = sub_task_ext;
                                        result = TRSD.SaveShipData(sfcdb, objShipData);
                                        if (result <= 0)
                                        {
                                            throw new Exception("Insert into r_7b5_ship_data Error!" + sub_task_no + "," + sub_hh_item + "," + sub_hw_item);
                                        }
                                        result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, sub_task_no, sub_hh_item, sub_task_ext);
                                        if (result <= 0)
                                        {
                                            throw new Exception("Update r_7b5_ship Error!" + sub_task_no + "," + sub_hh_item);
                                        }
                                        plan_qty_temp = plan_qty_temp + sub_task_ext;
                                    }
                                }

                                if (TCSL.IsExistBySkuAndSeq(sfcdb, subSkuno, 8))
                                {
                                    sql = $@" SELECT * FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                                              WHERE b.skuno = '{subSkuno}'
                                                AND b.seqno = 8
                                                AND a.hh_item = b.subskuno
                                                AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                AND a.task_no = c.task_no
                                                AND c.sap_flag = 'Y'
                                                AND LEFT (c.po_no, 1) IN ('D', '9')";
                                    //dt = GetShipLinkPo(sfcdb, subSkuno, input_task_no);
                                    dt = dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                    if (dt != null && dt.Rows.Count == 0)
                                    {
                                        throw new Exception(subSkuno + "error in ShipPlan r_7b5_ship,c_sku_link_7b5,r_7b5_po 12!");
                                    }                                    
                                    sql = $@"SELECT *
                                                   FROM (SELECT   b.subskuno,
                                                                  SUM (  a.task_qty
                                                                       - a.total_plan_qty
                                                                       - a.buffer_qty
                                                                      ) total_ext_qty
                                                             FROM r_7b5_ship a,
                                                                  c_sku_link_7b5 b,
                                                                  r_7b5_po c
                                                            WHERE b.skuno = '{subSkuno}'
                                                              AND b.seqno = 8
                                                              AND a.hh_item = b.subskuno
                                                              AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                              AND a.task_no = c.task_no
                                                              AND c.sap_flag = 'Y'
                                                              AND LEFT (c.po_no, 1) IN ('D', '9')
                                                         GROUP BY b.subskuno) m
                                                  WHERE m.total_ext_qty < {input_plan_qty}";
                                    dt = dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                    //dt = GetShipLinkPoTotalQty(sfcdb, input_task_no, subSkuno, input_plan_qty);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        throw new Exception(subSkuno + "error in ShipPlan r_7b5_ship,c_sku_link_7b5,r_7b5_po 13 !");
                                    }
                                    subSkuno_2 = TCSL.GetSubSkuListBySeq(sfcdb, subSkuno).FirstOrDefault();
                                    plan_qty_temp = 0;
                                    while (plan_qty_temp < input_plan_qty)
                                    {
                                        sql = $@"SELECT a.task_no as sub_task_no,
                                                      (a.task_qty - a.total_plan_qty - a.buffer_qty) sub_task_ext,
                                                      a.hh_item as sub_hh_item, a.hw_item as sub_hw_item                                                 
                                                 FROM r_7b5_ship a, r_7b5_po b
                                                WHERE a.hh_item = '{subSkuno_2}'
                                                  AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                                  AND a.task_no = b.task_no
                                                  AND b.sap_flag = 'Y'
                                                  AND LEFT (b.po_no, 1) IN ('D', '9')
                                                  AND a.receive_date =
                                                         (SELECT MIN (a.receive_date)
                                                            FROM r_7b5_ship a, r_7b5_po b
                                                           WHERE a.hh_item =  '{subSkuno_2}'
                                                             AND a.task_no = b.task_no
                                                             AND b.sap_flag = 'Y'
                                                             AND LEFT (b.po_no, 1) IN ('D', '9')
                                                             AND a.task_qty >a.total_plan_qty + a.buffer_qty)
                                                  AND ROWNUM = 1";
                                        dt = dt = sfcdb.ExecSelect(sql, null).Tables[0];
                                        //dt = GetShipPOTaskQtyBy(sfcdb, input_task_no, subSkuno_2);
                                        sub_task_no = dt.Rows[0]["sub_task_no"].ToString();
                                        sub_hh_item = dt.Rows[0]["sub_hh_item"].ToString();
                                        sub_hw_item = dt.Rows[0]["sub_hw_item"].ToString();
                                        sub_task_ext = Convert.ToDouble(dt.Rows[0]["sub_task_ext"].ToString());

                                        objShipData = new MESDataObject.Module.HWD.R_7B5_SHIP_DATA();
                                        objShipData.LOTNO = lotno;
                                        objShipData.TASK_NO = sub_task_no;
                                        objShipData.HH_ITEM = sub_hh_item;
                                        objShipData.HW_ITEM = sub_hw_item;
                                        objShipData.REMARK = "單板走賬至" + input_task_no;
                                        objShipData.LASTEDITBY = user;
                                        objShipData.LASTEDITDT = TRSD.GetDBDateTime(sfcdb);
                                        objShipData.DELETE_FLAG = "N";
                                        objShipData.SAP_FLAG = "N";

                                        if (sub_task_ext >= input_plan_qty - plan_qty_temp)
                                        {
                                            objShipData.QTY = input_plan_qty - plan_qty_temp;
                                            result = TRSD.SaveShipData(sfcdb, objShipData);
                                            if (result <= 0)
                                            {
                                                throw new Exception("Insert into r_7b5_ship_data Error!" + sub_task_no + "," + sub_hh_item + "," + sub_hw_item);
                                            }
                                            result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, sub_task_no, sub_hh_item, input_plan_qty - plan_qty_temp);
                                            if (result <= 0)
                                            {
                                                throw new Exception("Update r_7b5_ship Error!" + sub_task_no + "," + sub_hh_item);
                                            }
                                            plan_qty_temp = input_plan_qty;
                                        }
                                        else
                                        {
                                            objShipData.QTY = sub_task_ext;
                                            result = TRSD.SaveShipData(sfcdb, objShipData);
                                            if (result <= 0)
                                            {
                                                throw new Exception("Insert into r_7b5_ship_data Error!" + sub_task_no + "," + sub_hh_item + "," + sub_hw_item);
                                            }
                                            result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, sub_task_no, sub_hh_item, sub_task_ext);
                                            if (result <= 0)
                                            {
                                                throw new Exception("Update r_7b5_ship Error!" + sub_task_no + "," + sub_hh_item);
                                            }
                                            plan_qty_temp = plan_qty_temp + sub_task_ext;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                objShipData = new MESDataObject.Module.HWD.R_7B5_SHIP_DATA();
                objShipData.LOTNO = lotno;
                objShipData.TASK_NO = input_task_no;
                objShipData.HH_ITEM = hh_item;
                objShipData.HW_ITEM = hw_item;
                objShipData.REMARK = remark;
                objShipData.LASTEDITBY = user;
                objShipData.LASTEDITDT = TRSD.GetDBDateTime(sfcdb);
                objShipData.DELETE_FLAG = "N";
                objShipData.SAP_FLAG = "N";
                objShipData.QTY = input_plan_qty;
                result = TRSD.SaveShipData(sfcdb, objShipData);
                if (result <= 0)
                {
                    throw new Exception("Insert into r_7b5_ship_data Error!" + input_task_no + "," + hh_item + "," + hw_item);
                }
                sql = $@" UPDATE r_7b5_ship SET total_plan_qty = total_plan_qty + {input_plan_qty}
                        WHERE task_no = '{input_task_no}' AND hh_item = '{hh_item}'";
                result = Convert.ToInt32(sfcdb.ExecSQL(sql));
                //result = TRS.UpdateTotalPlanQtyByTaskAndItem(sfcdb, input_task_no, hh_item, input_plan_qty);
                if (result <= 0)
                {
                    throw new Exception("Update r_7b5_ship Error!" + input_task_no + "," + hh_item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  對應AP.check_7b5_xmldata_sp 中 var_type = 'UPLOAD_PLAN'
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="dbtype"></param>
        /// <param name="user"></param>
        /// <param name="data1"></param>
        public static void UploadPlan(OleExec sfcdb, DB_TYPE_ENUM dbtype, string user,string data1,out string msg)
        {
            string message = "";
            sfcdb.ThrowSqlExeception = true;
            sfcdb.BeginTrain();
            try
            {               
                int result;
                string sql,pcba_temp1;

                MESDataObject.Module.HWD.T_R_7B5_PLAN_UPLOAD TRPU = new MESDataObject.Module.HWD.T_R_7B5_PLAN_UPLOAD(sfcdb, dbtype);
                MESDataObject.Module.HWD.T_R_7B5_PLAN_TEMP TRPT = new MESDataObject.Module.HWD.T_R_7B5_PLAN_TEMP(sfcdb, dbtype);
                MESDataObject.Module.HWD.T_R_7B5_PLAN TRP = new MESDataObject.Module.HWD.T_R_7B5_PLAN(sfcdb, dbtype);
                MESDataObject.Module.HWD.T_R_7B5_XML_T TRXT = new MESDataObject.Module.HWD.T_R_7B5_XML_T(sfcdb, dbtype);

                List<MESDataObject.Module.HWD.R_7B5_PLAN_UPLOAD> listUpload = TRPU.GetUploadPlanListPCBA(sfcdb);
                MESDataObject.Module.HWD.R_7B5_PLAN_UPLOAD objUpload = null;
                MESDataObject.Module.HWD.R_7B5_PLAN objPlan = null;

                foreach (var u in listUpload)
                {
                    //index = u.PCBA.IndexOf(",") - 1;
                    //pcba_temp1 = u.PCBA;
                    //while (index > 0)
                    //{
                    //    pcba_temp2 = pcba_temp1.Substring(0, 3);
                    //    pcba_temp1 = pcba_temp1.Substring(index + 1);
                    //    objUpload = new MESDataObject.Module.HWD.R_7B5_PLAN_UPLOAD();
                    //    objUpload.ITEM = "0000000";
                    //    objUpload.PCBA = pcba_temp2;
                    //    objUpload.DAY1 = u.DAY1;
                    //    objUpload.DAY2 = u.DAY2;
                    //    objUpload.DAY3 = u.DAY3;
                    //    objUpload.DAY4 = u.DAY4;
                    //    objUpload.DAY5 = u.DAY5;
                    //    objUpload.DAY6 = u.DAY6;
                    //    objUpload.DAY7 = u.DAY7;
                    //    objUpload.DAY8 = u.DAY8;
                    //    objUpload.DAY9 = u.DAY9;
                    //    objUpload.DAY10 = u.DAY10;
                    //    objUpload.DAY11 = u.DAY11;
                    //    objUpload.DAY12 = u.DAY12;
                    //    objUpload.DAY13 = u.DAY13;
                    //    objUpload.DAY14 = u.DAY14;
                    //    objUpload.UPLOADDT = TRPU.GetDBDateTime(sfcdb);
                    //    objUpload.UPLOADBY = user;
                    //    result = TRPU.SaveUploadPlan(sfcdb, objUpload);
                    //    if (result <= 0)
                    //    {
                    //        throw new Exception("Insert into r_7b5_plan_upload fail," + pcba_temp2);
                    //    }
                    //    index = pcba_temp1.IndexOf(",") - 1;
                    //}

                    string[] arryPCBA = u.PCBA.Split(',');
                    foreach (string t in arryPCBA)
                    {
                        if (t != "")
                        {
                            objUpload = new MESDataObject.Module.HWD.R_7B5_PLAN_UPLOAD();
                            objUpload.ITEM = "0000000";
                            objUpload.PCBA = t;
                            objUpload.DAY1 = u.DAY1;
                            objUpload.DAY2 = u.DAY2;
                            objUpload.DAY3 = u.DAY3;
                            objUpload.DAY4 = u.DAY4;
                            objUpload.DAY5 = u.DAY5;
                            objUpload.DAY6 = u.DAY6;
                            objUpload.DAY7 = u.DAY7;
                            objUpload.DAY8 = u.DAY8;
                            objUpload.DAY9 = u.DAY9;
                            objUpload.DAY10 = u.DAY10;
                            objUpload.DAY11 = u.DAY11;
                            objUpload.DAY12 = u.DAY12;
                            objUpload.DAY13 = u.DAY13;
                            objUpload.DAY14 = u.DAY14;
                            objUpload.UPLOADDT = TRPU.GetDBDateTime(sfcdb);
                            objUpload.UPLOADBY = user;
                            result = TRPU.SaveUploadPlan(sfcdb, objUpload);
                            if (result <= 0)
                            {
                                throw new Exception("Insert into r_7b5_plan_upload fail," + t);
                            }
                        }
                    }

                    pcba_temp1 = arryPCBA[arryPCBA.Length - 1];
                    result = TRPU.UpdatePCBA(sfcdb, pcba_temp1, u.PCBA, u.ITEM);
                    if (result <= 0)
                    {
                        throw new Exception("Update r_7b5_plan_upload fail," + u.PCBA + "," + pcba_temp1);
                    }                  
                }
                
                TRPT.DeleteAllRecord(sfcdb);

                sql = $@" INSERT INTO r_7b5_plan_temp
                           SELECT   item,SUM (day1), SUM (day2), SUM (day3), SUM (day4),
                                      SUM (day5), SUM (day6), SUM (day7), SUM (day8), SUM (day9),
                                      SUM (day10), SUM (day11), SUM (day12), SUM (day13),
                                      SUM (day14), SYSDATE, 'GETRERE',MODEL
                                 FROM (SELECT item,MODEL, day1, day2, day3, day4, day5, day6, day7,
                                              day8, day9, day10, day11, day12, day13, day14
                                         FROM r_7b5_plan_upload
                                        WHERE item <> '0000000' AND item <> ' '
                                       UNION
                                       SELECT   pcba,MODEL, SUM (day1), SUM (day2), SUM (day3),
                                                SUM (day4), SUM (day5), SUM (day6), SUM (day7),
                                                SUM (day8), SUM (day9), SUM (day10), SUM (day11),
                                                SUM (day12), SUM (day13), SUM (day14)
                                           FROM r_7b5_plan_upload
                                          WHERE pcba IS NOT NULL AND pcba <> ' '
                                       GROUP BY pcba,MODEL) a
                             GROUP BY item ,MODEL";
                sfcdb.ExecSQL(sql);

                sql = $@" DELETE FROM r_7b5_plan WHERE item NOT IN (SELECT item  FROM r_7b5_plan_temp) ";
                sfcdb.ExecSQL(sql);

                List<MESDataObject.Module.HWD.R_7B5_PLAN_TEMP> listPlanTemp = TRPT.GetPlanTempList(sfcdb);
                foreach (var t in listPlanTemp)
                {
                    objPlan = TRP.GetPlanObject(sfcdb, t.ITEM);
                    if (objPlan != null)
                    {   
                        sql = $@" SELECT TO_DATE ('{data1}','yyyy/mm/dd')- TO_DATE (TO_CHAR (lasteditdt, 'yyyy/mm/dd'),'yyyy/mm/dd') as j
                                    FROM r_7b5_plan  WHERE item = '{t.ITEM}' AND ROWNUM = 1 ";
                        DataTable dt = sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
                        int j = Convert.ToInt32(dt.Rows[0][0].ToString());
                        if (j < 0)
                        {
                            //throw new Exception("no data in r_7b5_plan");
                            message += $@"{t.ITEM} no data in r_7b5_plan;";
                            continue;
                        }

                        double?[] oldDay = new double?[] {objPlan.DAY1,objPlan.DAY2, objPlan.DAY3, objPlan.DAY4, objPlan.DAY5, objPlan.DAY6, objPlan.DAY7, objPlan.DAY8,
                            objPlan.DAY9, objPlan.DAY10, objPlan.DAY11, objPlan.DAY12, objPlan.DAY13, objPlan.DAY14 };

                        double?[] newDay = new double?[] {t.DAY1,t.DAY2, t.DAY3, t.DAY4, t.DAY5, t.DAY6, t.DAY7, t.DAY8,
                            t.DAY9, t.DAY10, t.DAY11, t.DAY12, t.DAY13, t.DAY14  };

                        int i = 0;
                        while (i + j < 14)
                        {
                            if (oldDay[i + j] + newDay[i] == 0)
                            {
                                newDay[i] = oldDay[i + j];
                            }
                            else
                            {
                                if (oldDay[i + j] < 0)
                                {
                                    sql = $@" SELECT COUNT (1) as count  FROM r_7b5_plan_task
                                         WHERE item = '{t.ITEM}'  AND cancel_flag = 'N'  AND plan_dt = TO_DATE (TO_CHAR (TO_DATE ('{data1}','yyyy/mm/dd') + {i},'yyyy/mm/dd'),'yyyy/mm/dd')";
                                    dt = sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
                                    if (dt.Rows[0][0].ToString() == "0")
                                    {
                                        //throw new Exception("Error " + t.ITEM + " " + i+ "plan_dt 異常");
                                        message += $@"{t.ITEM},{data1},{i},plan_dt 異常;";
                                        i = i + 1;
                                        continue;
                                    }

                                    sql = $@"  UPDATE r_7b5_xml_t a
                                            SET a.plan_qty = a.plan_qty  - (SELECT b.plan_qty  FROM r_7b5_plan_task b
                                                       WHERE a.task_no = b.task_no  AND b.item = '{t.ITEM}'  AND b.plan_dt =
                                                                TO_DATE (TO_CHAR (TO_DATE ('{data1}','yyyy/mm/dd') + {i}, 'yyyy/mm/dd'),'yyyy/mm/dd')
                                                         AND b.cancel_flag = 'N'  AND ROWNUM = 1),a.plan_flag = 'N'
                                          WHERE EXISTS (SELECT * FROM r_7b5_plan_task c  WHERE c.task_no = a.task_no AND c.item = '{t.ITEM}'
                                                      AND c.plan_dt = TO_DATE (TO_CHAR (TO_DATE ('{data1}','yyyy/mm/dd') + {i},'yyyy/mm/dd'),'yyyy/mm/dd' )
                                                      AND c.cancel_flag = 'N')";
                                    sfcdb.ExecSQL(sql);

                                    sql = $@"UPDATE r_7b5_plan_task SET cancel_flag = 'Y' WHERE item = '{t.ITEM}'  AND cancel_flag = 'N' 
                                        AND plan_dt = TO_DATE (TO_CHAR (TO_DATE ('{data1}','yyyy/mm/dd') + {i} ,'yyyy/mm/dd'),'yyyy/mm/dd')";
                                    sfcdb.ExecSQL(sql);
                                }
                            }
                            i = i + 1;
                        }

                        sql = $@" UPDATE r_7b5_plan SET day1 = {newDay[0]},day2 = {newDay[1]}, day3 = {newDay[2]}, day4 = {newDay[3]},
                                   day5 = {newDay[4]},day6 = {newDay[5]},day7 = {newDay[6]}, day8 = {newDay[7]},day9 = {newDay[8]},
                                   day10 = {newDay[9]}, day11 = {newDay[10]}, day12 = {newDay[11]}, day13 = {newDay[12]}, day14 = {newDay[13]},
                                   lasteditdt = TO_DATE ('{data1}', 'yyyy/mm/dd'), lasteditby = '{user}'   WHERE item = '{t.ITEM}'";
                        sfcdb.ExecSQL(sql);
                    }
                    else
                    {
                        sql = $@" INSERT INTO r_7b5_plan
                        (item,MODEL, day1, day2, day3, day4, day5, day6, day7,day8, day9, day10, day11, day12, day13, day14,lasteditdt, lasteditby)
                           SELECT item,MODEL, day1, day2, day3, day4, day5, day6, day7, day8,day9, day10, day11, day12, day13, day14, TO_DATE ('{data1}', 'yyyy/mm/dd'), lasteditby
                             FROM r_7b5_plan_temp WHERE item = '{t.ITEM}'";
                        sfcdb.ExecSQL(sql);
                    }
                }

                List<MESDataObject.Module.HWD.R_7B5_PLAN> listPlan = TRP.Get7B5Plan(sfcdb);
                foreach (var p in listPlan)
                {
                    double?[] tempDay = new double?[] {p.DAY1,p.DAY2,p.DAY3,p.DAY4,p.DAY5,p.DAY6,p.DAY7,p.DAY8,p.DAY9,p.DAY10,p.DAY11,p.DAY12,p.DAY13,p.DAY14 };                   
                    int k = 0;
                    while (k < 14)
                    {
                        double? plan_buffer = tempDay[k] * 0.01;
                        double buffer_qty, task_ext;
                        string task_no_plan;
                        double task_qty, plan_qty;
                        bool bExist = sfcdb.ORM.Queryable<MESDataObject.Module.HWD.R_7B5_XML_T>().Any(r => r.ITEM == p.ITEM && r.PLAN_FLAG == "N" && r.CANCEL_FLAG == "N" && r.QTY > r.PLAN_QTY);
                        while (tempDay[k] > 0 && bExist)
                        {
                            sql = $@" SELECT task_no, qty, plan_qty
                                         FROM r_7b5_xml_t  WHERE plan_flag = 'N' AND qty > plan_qty AND cancel_flag = 'N'
                                          AND item = '{p.ITEM}' AND start_date =(SELECT MIN (start_date)FROM r_7b5_xml_t WHERE item = '{p.ITEM}'
                                          AND plan_flag = 'N' AND qty > plan_qty  AND cancel_flag = 'N')
                                          AND publish_time =(SELECT MIN (publish_time) FROM r_7b5_xml_t WHERE plan_flag = 'N'  AND qty > plan_qty
                                          AND cancel_flag = 'N' AND item = '{p.ITEM}' AND start_date = (SELECT MIN (start_date) FROM r_7b5_xml_t
                                          WHERE item = '{p.ITEM}' AND plan_flag = 'N' AND qty > plan_qty AND cancel_flag = 'N')) AND ROWNUM = 1";
                            DataTable dtXML = sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
                            if (dtXML == null || dtXML.Rows.Count == 0)
                            {
                                throw new Exception("Get Task NO Error From r_7b5_xml_t By " + p.ITEM);                               
                            }
                            task_no_plan = dtXML.Rows[0]["TASK_NO"].ToString();
                            task_qty = Convert.ToDouble(dtXML.Rows[0]["QTY"].ToString());
                            plan_qty = Convert.ToDouble(dtXML.Rows[0]["PLAN_QTY"].ToString());
                            if (task_no_plan.StartsWith("BS"))
                            {
                                buffer_qty = 100;
                            }
                            else
                            {
                                buffer_qty = 10;
                            }
                            task_ext = task_qty - plan_qty;

                            if (tempDay[k] >= task_ext + plan_buffer)
                            {
                                sql = $@" UPDATE r_7b5_xml_t SET plan_qty = {task_qty}, plan_flag = 'Y' WHERE task_no = '{task_no_plan}' ";
                                sfcdb.ExecSQL(sql);

                                sql = $@" INSERT INTO r_7b5_plan_task (task_no, item, plan_qty, plan_dt, lasteditby, cancel_flag, lasteditdt)
                                         VALUES ( '{task_no_plan}', '{p.ITEM}', '{task_ext}', TO_DATE (TO_CHAR (TO_DATE ('{data1}','yyyy/mm/dd') + {k},'yyyy/mm/dd' ),'yyyy/mm/dd' ),
                                            'SYSTEM', 'N', SYSDATE)";
                                sfcdb.ExecSQL(sql);

                                tempDay[k] = tempDay[k] - task_ext;
                            }
                            else if (tempDay[k] >= task_ext - buffer_qty && tempDay[k] < task_ext + plan_buffer)
                            {
                                sql = $@" UPDATE r_7b5_xml_t SET plan_qty = {task_qty}, plan_flag = 'Y' WHERE task_no = '{task_no_plan}' ";
                                sfcdb.ExecSQL(sql);

                                sql = $@"INSERT INTO r_7b5_plan_task (task_no, item,plan_qty,plan_dt,lasteditby, cancel_flag, lasteditdt)
                                        VALUES ( '{task_no_plan}','{p.ITEM}','{tempDay[k]}',TO_DATE (TO_CHAR (TO_DATE ('{data1}','yyyy/mm/dd') + {k},'yyyy/mm/dd'), 'yyyy/mm/dd'),
                                            'SYSTEM', 'N', SYSDATE)";
                                sfcdb.ExecSQL(sql);

                                tempDay[k] = 0;
                            }
                            else
                            {
                                sql = $@" UPDATE r_7b5_xml_t SET plan_qty = {plan_qty} + {tempDay[k]} WHERE task_no = '{task_no_plan}' ";
                                sfcdb.ExecSQL(sql);

                                sql = $@" INSERT INTO r_7b5_plan_task (task_no, item, plan_qty, plan_dt,lasteditby, cancel_flag, lasteditdt)
                                        VALUES ('{task_no_plan}','{p.ITEM}','{tempDay[k]}',TO_DATE (TO_CHAR (TO_DATE ('{data1}','yyyy/mm/dd') + {k},'yyyy/mm/dd'),'yyyy/mm/dd'),
                                            'SYSTEM', 'N', SYSDATE )";
                                sfcdb.ExecSQL(sql);

                                tempDay[k] = 0;
                            }

                            bExist = sfcdb.ORM.Queryable<MESDataObject.Module.HWD.R_7B5_XML_T>().Any(r => r.ITEM == p.ITEM && r.PLAN_FLAG == "N" && r.CANCEL_FLAG == "N" && r.QTY > r.PLAN_QTY);
                        }
                        k = k + 1;
                    }

                    sql = $@" UPDATE r_7b5_plan
                                SET day1 = CASE WHEN day1 > {tempDay[0]} THEN {tempDay[0]} - day1 ELSE day1 END,
                                    day2 = CASE WHEN day2 > {tempDay[1]} THEN {tempDay[1]} - day2 ELSE day2 END,
                                    day3 = CASE WHEN day3 > {tempDay[2]} THEN {tempDay[2]} - day3 ELSE day3 END,
                                    day4 = CASE WHEN day4 > {tempDay[3]} THEN {tempDay[3]} - day4 ELSE day4 END,
                                    day5 = CASE WHEN day5 > {tempDay[4]} THEN {tempDay[4]} - day5 ELSE day5 END,
                                    day6 = CASE WHEN day6 > {tempDay[5]} THEN {tempDay[5]} - day6 ELSE day6 END,
                                    day7 = CASE WHEN day7 > {tempDay[6]} THEN {tempDay[6]} - day7 ELSE day7 END,
                                    day8 = CASE WHEN day8 > {tempDay[7]} THEN {tempDay[7]} - day8 ELSE day8 END,
                                    day9 = CASE WHEN day9 > {tempDay[8]} THEN {tempDay[8]} - day9 ELSE day9 END,
                                    day10 = CASE WHEN day10 > {tempDay[9]}  THEN  {tempDay[9]} - day10 ELSE day10 END,
                                    day11 = CASE WHEN day11 > {tempDay[10]} THEN {tempDay[10]} - day11 ELSE day11 END,
                                    day12 = CASE WHEN day12 > {tempDay[11]} THEN {tempDay[11]} - day12 ELSE day12 END,
                                    day13 = CASE WHEN day13 > {tempDay[12]} THEN {tempDay[12]} - day13 ELSE day13 END,
                                    day14 = CASE WHEN day14 > {tempDay[13]} THEN {tempDay[13]} - day14 ELSE day14 END
                               WHERE item = '{p.ITEM}'";

                    sfcdb.ExecSQL(sql);
                }

                sfcdb.CommitTrain();
                msg = message;
            }
            catch (Exception ex)
            {
                sfcdb.RollbackTrain();
                throw ex;
            }
        }
      
        /// <summary>
        /// 對應AP.check_7b5_xmldata_sp 中 var_type = 'SAVE_CHECK'
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="dbtype"></param>
        public static void SaveCheck(OleExec sfcdb, OleExec apdb, DB_TYPE_ENUM dbtype,string task_no)
        {
            sfcdb.ThrowSqlExeception = true;
            apdb.ThrowSqlExeception = true;
            int result = 0;
            try
            {
                string sql = "";
                MESDataObject.Module.HWD.T_R_7B5_XML_T TRXT = new MESDataObject.Module.HWD.T_R_7B5_XML_T(sfcdb, dbtype);
                T_C_SKU TCS = new T_C_SKU(sfcdb, dbtype);
                T_R_SKU_ROUTE TRSR = new T_R_SKU_ROUTE(sfcdb, dbtype);

                MESDataObject.Module.HWD.R_7B5_XML_T objXMLT = TRXT.GetObjectByTaskNO(sfcdb, task_no);
                if (objXMLT != null)
                {
                    throw new Exception("Error,THIS TASK_NO:" + task_no + " HAVE EXIST");
                }

                //if (objXMLT.TRANSFER_FLAG == "1")
                //{
                //    throw new Exception("Error,THIS TASK_NO:" + task_no + " HAVE CREATE WO");
                //}

                sql = $@"INSERT INTO r_7b5_xml_t_his SELECT * FROM r_7b5_xml_t  WHERE task_no = '{task_no}'";
                sfcdb.ExecSQL(sql);
                
                sql = $@"DELETE  r_7b5_xml_t WHERE task_no = '{task_no}' ";
                sfcdb.ExecSQL(sql);

                sql = $@" INSERT INTO r_7b5_xml_t  SELECT *  FROM r_7b5_xml_t_tmp WHERE task_no = '{task_no}'";
                result = Convert.ToInt32(sfcdb.ExecSQL(sql));
                if (result == 0)
                {
                    throw new Exception(task_no + ";INSERT R_7B5_XML_T ERROR");
                }

                if (task_no.StartsWith("BS") &&
                    ((task_no.Substring(2, 1) != "Z" && task_no.Substring(2, 1) != "G" && task_no.Substring(2, 1) != "F")
                    || task_no.Substring(1, 1) == "R" || task_no.Substring(1, 1) == "S"))
                {
                    sql = $@" UPDATE r_7b5_xml_t SET plan_flag = 'Y0' WHERE task_no = '{task_no}'";
                    sfcdb.ExecSQL(sql);
                    
                    T_R_7B5_UPDATE_LIST TRUL = new T_R_7B5_UPDATE_LIST(sfcdb, DB_TYPE_ENUM.Oracle);
                    R_7B5_UPDATE_LIST obj = new R_7B5_UPDATE_LIST();
                    obj.TYPE = "NOT_NORMAL_TASK";
                    obj.TASK_NO = task_no;
                    obj.DATA1 = "0";
                    obj.DATA2 = "0";
                    obj.LASTEDITDT = TRUL.GetDBDateTime(sfcdb);
                    obj.LASTEDITBY = "INTERFACE";
                    TRUL.SaveData(sfcdb, obj);

                }
                if (task_no.StartsWith("BS"))
                {
                    sql = $@" UPDATE r_7b5_xml_t  SET task_change_no = '與TE確認備品是否齊套' WHERE task_no = '{task_no}' and SUBSTR(ITEM, 1, 2) IN ('03', '02')";
                    sfcdb.ExecSQL(sql);
                }

                if (task_no.StartsWith("DR") || task_no.StartsWith("DS") || task_no.StartsWith("DW"))
                {
                    sql = $@" UPDATE r_7b5_xml_t  SET task_change_no = '任務令有技改請注意' WHERE task_no = '{task_no}' ";
                    sfcdb.ExecSQL(sql);
                }
                objXMLT = TRXT.GetObjectByTaskNO(sfcdb, task_no);
                
                List<C_SKU> listSku = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, "A")
                && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X") && r.SKU_NAME == objXMLT.MODEL).ToList();

                if (listSku.Count > 0 && !objXMLT.DESCRIPTION.Contains("板") && !objXMLT.DESCRIPTION.Contains("組件") && !objXMLT.DESCRIPTION.Contains("啞機") && !objXMLT.DESCRIPTION.Contains("元件"))
                {                    
                    string sku_id = TCS.GetNewID("HWD", sfcdb);
                    string route_id = TRSR.GetNewID("HWD", sfcdb);
                    string temp_skuno = "A" + objXMLT.ITEM + "-A";

                    listSku = sfcdb.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == temp_skuno).ToList();
                    if (listSku.Count == 0)
                    {
                        sql=$@"INSERT INTO C_SKU (id, bu, skuno, version, sku_name, c_series_id, cust_partno, cust_sku_code, sn_rule, panel_rule, description, edit_emp, edit_time, sku_type, aqltype )
                              VALUES  ('{sku_id}', 'HWD', 'A{objXMLT.ITEM}-A', '01', '{objXMLT.MODEL}', 'HWD000000000000000000000000000001', 
                              '{objXMLT.ITEM}', '', '****************', '', '****************', 'SYSTEM', SYSDATE, 'MODEL', 'AQL_0.25')";
                        sfcdb.ExecSQL(sql);

                        sql = $@"INSERT INTO R_SKU_ROUTE(ID, ROUTE_ID, SKU_ID, DEFAULT_FLAG, EDIT_TIME, EDIT_EMP)
                                    VALUES('{route_id}', 'HWD0000000000000000000000000000T9', '{sku_id}', '', SYSDATE, 'SYSTEM')";
                        sfcdb.ExecSQL(sql);
                    }
                    temp_skuno = objXMLT.ITEM + "-A";
                    listSku = sfcdb.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == temp_skuno).ToList();
                    if (listSku.Count == 0)
                    {
                        sku_id = TCS.GetNewID("HWD", sfcdb);
                        route_id = TRSR.GetNewID("HWD", sfcdb);
                        sql = $@"INSERT INTO C_SKU (id, bu, skuno, version, sku_name, c_series_id, cust_partno, cust_sku_code, sn_rule, panel_rule, description, edit_emp, edit_time, sku_type, aqltype )
                              VALUES  ('{sku_id}', 'HWD', '{objXMLT.ITEM}-A', '01', '{objXMLT.MODEL}', 'HWD000000000000000000000000000001', 
                              '{objXMLT.ITEM}', '', '****************', '', '****************', 'SYSTEM', SYSDATE, 'MODEL', 'AQL_0.25')";
                        sfcdb.ExecSQL(sql);

                        sql = $@"INSERT INTO R_SKU_ROUTE (ID, ROUTE_ID, SKU_ID, DEFAULT_FLAG, EDIT_TIME, EDIT_EMP)
                                    VALUES('{route_id}', 'HWD0000000000000000000000000000T9', '{sku_id}', '', SYSDATE, 'SYSTEM')";
                        sfcdb.ExecSQL(sql);
                    }

                    sql = $@"select count(*) as row_count from mes1.c_product_config WHERE p_no ='A{objXMLT.ITEM}-A' ";
                    DataTable dtAllPart = apdb.ExecuteDataTable(sql, CommandType.Text, null);
                    if (dtAllPart != null && dtAllPart.Rows[0][0].ToString() == "0")
                    {
                        sql = $@"INSERT INTO  mes1.c_product_config (cust_code,bu_code,p_no,p_version,p_desc,p_type,sn_len,pth_flag,wo_pno,process_type,
                                 edit_time,edit_emp,weld_qty,panel_type,link_qty,process_flag,memo,data1,data2) VALUES 
                                  ('HUAWEI','HWD','A{objXMLT.ITEM}-A','01','{objXMLT.MODEL}','OEM','11','1','','0',SYSDATE,'SYSTEM','1','0','1','T','','','Y') ";
                        apdb.ExecSQL(sql);
                    }

                    sql = $@"select count(*) as row_count from mes1.c_product_config WHERE p_no ='{objXMLT.ITEM}-A' ";
                    dtAllPart = apdb.ExecuteDataTable(sql, CommandType.Text, null);
                    if (dtAllPart != null && dtAllPart.Rows[0][0].ToString() == "0")
                    {
                        sql = $@"INSERT INTO  mes1.c_product_config (cust_code,bu_code,p_no,p_version,p_desc,p_type,sn_len,pth_flag,wo_pno,process_type,
                                 edit_time,edit_emp,weld_qty,panel_type,link_qty,process_flag,memo,data1,data2) VALUES 
                                  ('HUAWEI','HWD','{objXMLT.ITEM}-A','01','{objXMLT.MODEL}','OEM','11','1','','0',SYSDATE,'SYSTEM','1','0','1','T','','','Y') ";
                        apdb.ExecSQL(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 對應AP.check_7b5_xmldata_sp 中 var_type = 'SAVE_WO_QTY'
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="dbtype"></param>
        /// <param name="task_no"></param>
        public static void SaveWOQty(OleExec sfcdb, DB_TYPE_ENUM dbtype,string ip, ref string msg)
        {
            sfcdb.ThrowSqlExeception = true;
            var lockObj = sfcdb.ORM.Queryable<R_SYNC_LOCK>().Where(r => r.LOCK_NAME == "HWD_7B5_SaveWOQty").ToList().FirstOrDefault();
            if (lockObj != null)
            {
                throw new Exception($@"SaveWOQty is running on {lockObj.LOCK_IP}");
            }
            else
            {
                T_R_SYNC_LOCK t_r_sync_lock = new T_R_SYNC_LOCK(sfcdb, DB_TYPE_ENUM.Oracle);
                lockObj = new R_SYNC_LOCK();
                lockObj.ID = t_r_sync_lock.GetNewID("HWD", sfcdb);
                lockObj.LOCK_NAME = "HWD_7B5_SaveWOQty";
                lockObj.LOCK_IP = ip;
                lockObj.LOCK_TIME = sfcdb.ORM.GetDate();
                lockObj.EDIT_EMP = "SaveWOQty";
                sfcdb.ORM.Insertable<R_SYNC_LOCK>(lockObj).ExecuteCommand();
            }
            try
            {
                string[] arrayThirdWord = new string[] { "G", "F", "N" };
                string[] arraySecondWord = new string[] { "R", "S", "T", "W", "V" };
                List<C_SKU> listSkuTemp = new List<C_SKU>();
                List<MESDataObject.Module.HWD.C_SKU_LINK_7B5> listLink = new List<MESDataObject.Module.HWD.C_SKU_LINK_7B5>();
                C_SKU objSku = null;
                C_SKU objSubSku = null;
                MESDataObject.Module.HWD.R_7B5_EC objEC = null;
                string sql = "", skuno = "", task_no_type = "", thirdWord = "", secondWord = "", wo_type = "", task_level = "", ec_code = "";
                string sub_item = "", sub_wo_type = "", sub_task_level = "", v_task_no = "", task_no_4 = "";
                string wo_task_no = "", wo_item, wo_change_information, wo_start_date, wo_complete_date, wo_item_ver, wo_description, wo_rohs, wo_product_line, wo_po_flag, lasteditdt, model;
                double wo_qty;
                int result = 0, task_no_4_tmp;
                string sqlLastEditdt, sqlCompleteDate, sqlStartDate;
                string tempSkuno = "";

                MESDataObject.Module.HWD.T_R_7B5_XML_T TRXT = new MESDataObject.Module.HWD.T_R_7B5_XML_T(sfcdb, dbtype);
                MESDataObject.Module.HWD.T_R_7B5_EC TREX = new MESDataObject.Module.HWD.T_R_7B5_EC(sfcdb, dbtype);
                MESDataObject.Module.HWD.T_C_SKU_LINK_7B5 TCSL = new MESDataObject.Module.HWD.T_C_SKU_LINK_7B5(sfcdb, dbtype);

                DataTable dtNoTransfer = TRXT.GetNoTransferList(sfcdb);
                if (dtNoTransfer != null && dtNoTransfer.Rows.Count > 0)
                {
                    for (int i = 0; i < dtNoTransfer.Rows.Count; i++)
                    {
                        try
                        {
                            tempSkuno = "";
                            task_no_type = "";
                            skuno = "";
                            wo_type = "";
                            sqlCompleteDate = "";
                            sqlStartDate = "";
                            task_level = "";
                            ec_code = "";
                            result = 0;
                            sql = "";
                            task_no_4_tmp = 0;
                            sub_item = "";
                            sub_wo_type = "";
                            sub_task_level = "";
                            task_no_4 = "";
                            objSku = null;
                            objEC = null;
                            objSubSku = null;
                            listLink = new List<C_SKU_LINK_7B5>();
                            listSkuTemp = new List<C_SKU>();

                            wo_task_no = dtNoTransfer.Rows[i]["TASK_NO"].ToString();
                            wo_item = dtNoTransfer.Rows[i]["ITEM"].ToString();
                            wo_change_information = dtNoTransfer.Rows[i]["CHANGE_INFORMATION"].ToString();
                            wo_qty = Convert.ToDouble(dtNoTransfer.Rows[i]["QTY"].ToString());
                            wo_start_date = dtNoTransfer.Rows[i]["START_DATE"].ToString();
                            wo_complete_date = dtNoTransfer.Rows[i]["COMPLETE_DATE"].ToString();
                            wo_item_ver = dtNoTransfer.Rows[i]["ITEM_VERSION"].ToString();
                            wo_description = dtNoTransfer.Rows[i]["DESCRIPTION"].ToString();
                            wo_rohs = dtNoTransfer.Rows[i]["ROHS"].ToString();
                            wo_product_line = dtNoTransfer.Rows[i]["PRODUCT_LINE"].ToString();
                            wo_po_flag = dtNoTransfer.Rows[i]["PO_FLAG"].ToString();
                            lasteditdt = dtNoTransfer.Rows[i]["LASTEDITDT"].ToString();
                            model = dtNoTransfer.Rows[i]["MODEL"].ToString();
                            thirdWord = wo_task_no.Substring(2, 1);
                            secondWord = wo_task_no.Substring(1, 1);

                            if (wo_task_no.StartsWith("BS"))
                            {
                                tempSkuno = "A" + wo_item;
                                skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno)
                                && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X") && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                task_no_type = "BS";
                            }
                            else
                            {
                                task_no_type = "CS";
                                if (arrayThirdWord.Contains(thirdWord))
                                {
                                    if (wo_task_no == "DPF6K92E18E")
                                    {
                                        skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                        && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                    }
                                    else
                                    {
                                        skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                        && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                    }
                                }
                                else
                                {
                                    if (wo_description.Contains("啞機") || wo_description.Contains("組件") || wo_description.Contains("服務備板"))
                                    {
                                        //skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //    .Max(r => r.SKUNO);
                                        tempSkuno = "A" + wo_item;
                                        skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                        && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                        if (string.IsNullOrEmpty(skuno))
                                        {
                                            skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                            && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                            if (string.IsNullOrEmpty(skuno))
                                            {
                                                skuno = sfcdb.ORM.Queryable<C_SKU>()
                                                    .Where(r => !SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno) && SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                                    && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                            }
                                        }

                                        //listSkuTemp = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //.ToList();

                                        //if (string.IsNullOrEmpty(skuno) && listSkuTemp.Count == 0)
                                        //{
                                        //    skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //        .Max(r => r.SKUNO);
                                        //}
                                    }
                                    else
                                    {
                                        //if (wo_description.Contains("組件") || wo_description.Contains("服務備板"))
                                        //{
                                        //    if (arraySecondWord.Contains(secondWord) && model != "KD13")
                                        //    {
                                        //        skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //            .Max(r => r.SKUNO);

                                        //        tempSkuno = "A" + wo_item;
                                        //        listSkuTemp = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //            .ToList();
                                        //        if (string.IsNullOrEmpty(skuno) && listSkuTemp.Count == 0)
                                        //        {
                                        //            skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //                .Max(r => r.SKUNO);
                                        //        }
                                        //    }
                                        //    else
                                        //    {
                                        //        tempSkuno = "A" + wo_item;
                                        //        skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //            .Max(r => r.SKUNO);

                                        //        listSkuTemp = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //        .ToList();

                                        //        if (string.IsNullOrEmpty(skuno) && listSkuTemp.Count == 0)
                                        //        {
                                        //            skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X"))
                                        //                .Max(r => r.SKUNO);
                                        //        }
                                        //    }
                                        //}
                                        //else
                                        //{
                                        if (arraySecondWord.Contains(secondWord))
                                        {
                                            listSkuTemp = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                            && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).ToList();
                                            if (listSkuTemp.Count > 0)
                                            {
                                                skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                                && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                            }
                                            else
                                            {
                                                tempSkuno = "A" + wo_item;
                                                listSkuTemp = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                                && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).ToList();
                                                if (listSkuTemp.Count == 0)
                                                {
                                                    skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                                    && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (thirdWord == "G" || thirdWord == "C" || thirdWord == "F")
                                            {
                                                skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                                && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                            }
                                            else
                                            {
                                                if (thirdWord == "A")
                                                {
                                                    tempSkuno = "A" + wo_item;
                                                    skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                                    && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                                }
                                                else
                                                {
                                                    if (wo_task_no.Length > 10 && wo_task_no.Substring(9, 1) == "6")
                                                    {
                                                        skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                                        && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                                    }
                                                    else
                                                    {
                                                        tempSkuno = "A" + wo_item;
                                                        listSkuTemp = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno)
                                                            && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X") && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).ToList();
                                                        if (listSkuTemp.Count > 0)
                                                        {
                                                            skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, tempSkuno)
                                                                && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X") && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                                        }
                                                        else
                                                        {
                                                            listSkuTemp = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item)
                                                                && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X") && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).ToList();
                                                            if (listSkuTemp.Count > 0)
                                                            {
                                                                skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item)
                                                                    && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X") && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                                            }
                                                            else
                                                            {
                                                                listSkuTemp = sfcdb.ORM.Queryable<C_SKU>().Where(r => r.SKU_NAME == model && SqlSugar.SqlFunc.StartsWith(r.SKUNO, "A")
                                                                && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).ToList();
                                                                if (listSkuTemp.Count > 0)
                                                                {
                                                                    throw new Exception("Can not match HH cust_no " + wo_task_no);
                                                                }
                                                                else
                                                                {
                                                                    skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item)
                                                                        && SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X") && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //}
                                    }
                                }
                            }

                            if (wo_task_no.Substring(3, 1) == "S")
                            {
                                skuno = sfcdb.ORM.Queryable<C_SKU>().Where(r => SqlSugar.SqlFunc.StartsWith(r.SKUNO, wo_item) && !SqlSugar.SqlFunc.EndsWith(r.SKUNO, "X")
                                && SqlSugar.SqlFunc.Contains(r.SKUNO, wo_item)).Max(r => r.SKUNO);
                            }
                            if (string.IsNullOrEmpty(skuno))
                            {
                                throw new Exception(wo_task_no + "," + wo_item + " Mapping HH SKUNO Error!");
                            }

                            objSku = sfcdb.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == skuno).ToList().FirstOrDefault();
                            if (objSku == null)
                            {
                                throw new Exception(skuno + " Not Setting In C_SKU!");
                            }

                            if (wo_task_no.StartsWith("BS"))
                            {
                                sqlCompleteDate = $@" TO_CHAR(TO_DATE('{wo_start_date}', 'YYYYMMDD') + 6,'YYYY/MM/DD')";
                                sqlStartDate = $@" TO_CHAR(TO_DATE('{wo_start_date}', 'YYYYMMDD') + 4,'YYYY/MM/DD')";
                                wo_type = "ZWSW";
                            }
                            else
                            {
                                sqlCompleteDate = $@" TO_CHAR(TO_DATE('{wo_start_date}', 'YYYYMMDD') + 7,'YYYY/MM/DD') ";
                                sqlStartDate = $@" TO_CHAR(TO_DATE('{wo_start_date}', 'YYYYMMDD') + 5,'YYYY/MM/DD') ";
                                if (arraySecondWord.Contains(secondWord) && thirdWord == "Z")
                                {
                                    if (objSku.SKU_TYPE == "PCBA")
                                    {
                                        wo_type = "ZESW";
                                    }
                                    else
                                    {
                                        wo_type = "ZESD";
                                    }
                                }

                                if (!arraySecondWord.Contains(secondWord) && thirdWord == "Z")
                                {
                                    if (objSku.SKU_TYPE == "PCBA")
                                    {
                                        wo_type = "ZESW";
                                    }
                                    else
                                    {
                                        wo_type = "ZWSD";
                                    }
                                }
                                if (thirdWord == "G" || thirdWord == "F")
                                {
                                    wo_type = "ZCS4";
                                }
                            }

                            if (objSku.SKU_TYPE == "PCBA")
                            {
                                task_level = "SMT";
                            }
                            else
                            {
                                task_level = "DC";
                            }
                            objEC = TREX.GetECObjectByTask(sfcdb, wo_task_no);
                            if (objEC != null)
                            {
                                ec_code = objEC.EC_CODE;
                                result = sfcdb.ORM.Updateable<MESDataObject.Module.HWD.R_7B5_XML_T>().UpdateColumns(r => new MESDataObject.Module.HWD.R_7B5_XML_T { TASK_CHANGE_NO = ec_code })
                                    .Where(r => r.TASK_NO == wo_task_no).ExecuteCommand();
                                if (result == 0)
                                {
                                    throw new Exception("Update  Task Change NO Fail:" + wo_task_no);
                                }
                            }


                            //MESDataObject.Module.HWD.R_7B5_WO_TEMP objWOTemp = new MESDataObject.Module.HWD.R_7B5_WO_TEMP();
                            //objWOTemp.RECEIVE_DATE = (DateTime)lasteditdt;
                            sqlLastEditdt = $@"to_date('{lasteditdt}','yyyy/mm/dd hh24:mi:ss')";


                            sql = $@"INSERT INTO r_7b5_wo_temp(receive_date, task_no_type, task_no_use,sap_factory, product_line, task_no, v_task_no,
                                 hh_item, hw_item, qty, rohs,start_date, complete_date,change_information, main_wo_flag, create_time,
                                 item_version, znp195_flag, wo_type, model,task_no_level) VALUES 
                                ({sqlLastEditdt},'{task_no_type}','ADDN','WDN1','{wo_product_line}','{wo_task_no}','{wo_task_no}',
                                '{skuno}', '{wo_item}', {wo_qty}, '{wo_rohs}', {sqlStartDate}, {sqlCompleteDate},
                                 '{wo_change_information}', 'Y', SYSDATE, '{wo_item_ver}', 'N', '{wo_type}', '{model}','{task_level}')";
                            result = Convert.ToInt32(sfcdb.ExecSQL(sql));
                            if (result == 0)
                            {
                                throw new Exception("INSERT INTO R_7B5_WO_TEMP Fail:" + wo_task_no);
                            }

                            if (wo_task_no.Substring(3, 1) == "S")
                            {
                                sql = $@" INSERT INTO r_7b5_ship (model,task_no,hh_item, hw_item, task_qty, receive_date, shipped_qty, buffer_qty, 
                                    total_plan_qty, lasteditby, lasteditdt ) VALUES
                                    ('{model}', LEFT ('{wo_task_no}', 3)|| '6'|| RIGHT ('{wo_task_no}', LENGTH ('{wo_task_no}') - 4),
                                    '{skuno}', '{wo_item}', {wo_qty}, {sqlLastEditdt},0, 0, 0,'SYSTEM', SYSDATE)";
                                result = Convert.ToInt32(sfcdb.ExecSQL(sql));
                            }
                            else
                            {
                                sql = $@"INSERT INTO r_7b5_ship (model, task_no, hh_item, hw_item,task_qty, receive_date, shipped_qty, buffer_qty,
                                    total_plan_qty, lasteditby, lasteditdt)
                                    VALUES ('{model}', '{wo_task_no}', '{skuno}', '{wo_item}',{wo_qty}, {sqlLastEditdt}, 0, 0,0, 'SYSTEM', SYSDATE )";
                                result = Convert.ToInt32(sfcdb.ExecSQL(sql));
                            }

                            if (result == 0)
                            {
                                throw new Exception("INSERT INTO R_7B5_SHIP Fail:" + wo_task_no);
                            }

                            listLink = TCSL.GetPCBALinkList(sfcdb, skuno);
                            if (listLink.Count > 0 && (wo_task_no.StartsWith("BS") || thirdWord == "Z"))
                            {
                                task_no_4_tmp = ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ").IndexOf(wo_task_no.Substring(3, 1));
                                DataTable dt = TCSL.GetSubSku(sfcdb, skuno);
                                for (int k = 0; k < dt.Rows.Count; k++)
                                {
                                    sub_item = dt.Rows[k]["subskuno"].ToString();
                                    if (sub_item.StartsWith("A"))
                                    {
                                        sub_item = sub_item.Substring(1, sub_item.Length - 1);
                                    }
                                    if (sub_item.IndexOf('-') >= 0)
                                    {
                                        sub_item = sub_item.Substring(0, sub_item.IndexOf('-') + 1 - 1);
                                    }
                                    objSubSku = sfcdb.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == dt.Rows[k]["subskuno"].ToString()).ToList().FirstOrDefault();
                                    if (objSubSku == null)
                                    {
                                        throw new Exception(dt.Rows[k]["subskuno"].ToString() + " Not Setting In C_SKU!");
                                    }
                                    if (wo_task_no.StartsWith("BS"))
                                    {
                                        sub_wo_type = "ZWSW";
                                    }
                                    else
                                    {
                                        if ((secondWord == "R" || secondWord == "S") && thirdWord == "Z")
                                        {
                                            if (objSubSku.SKU_TYPE == "PCBA")
                                            {
                                                sub_wo_type = "ZESW";
                                            }
                                            else
                                            {
                                                sub_wo_type = "ZESD";
                                            }
                                        }

                                        if (secondWord != "R" && secondWord != "S" && thirdWord == "Z")
                                        {
                                            if (objSubSku.SKU_TYPE == "PCBA")
                                            {
                                                sub_wo_type = "ZESW";
                                            }
                                            else
                                            {
                                                sub_wo_type = "ZWSD";
                                            }
                                        }
                                    }

                                    if (objSubSku.SKU_TYPE == "PCBA")
                                    {
                                        sub_task_level = "SMT";
                                    }
                                    else
                                    {
                                        sub_task_level = "DC";
                                    }

                                    if (wo_task_no.StartsWith("BS"))
                                    {
                                        sql = $@"INSERT INTO r_7b5_wo_temp (receive_date, task_no_type, task_no_use,sap_factory, product_line, task_no,
                                                  v_task_no, hh_item, hw_item, qty, rohs, start_date, complete_date, change_information,
                                                  main_wo_flag, create_time, item_version, znp195_flag, wo_type, model, task_no_level) VALUES 
                                                  ({sqlLastEditdt}, '{task_no_type}', 'ADDN', 'WDN1', '{wo_product_line}', '{wo_task_no}',
                                                  '{wo_task_no}', '{dt.Rows[k]["subskuno"].ToString()}', '{sub_item}',  {wo_qty}, '{wo_rohs}', {sqlStartDate},
                                                  {sqlCompleteDate}, '{wo_change_information}',  'N', SYSDATE, '{wo_item_ver}',
                                                  'N', '{sub_wo_type}', '{model}', '{sub_task_level}')";
                                        sfcdb.ExecSQL(sql);
                                    }
                                    else
                                    {
                                        task_no_4_tmp = task_no_4_tmp + 1;
                                        task_no_4 = Todes36(task_no_4_tmp);
                                        v_task_no = wo_task_no.Substring(0, 3) + task_no_4 + wo_task_no.Substring(4);
                                        sql = $@" INSERT INTO r_7b5_wo_temp
                                                 (receive_date, task_no_type, task_no_use,sap_factory, product_line, task_no,
                                                  v_task_no, hh_item, hw_item, qty, rohs, start_date,complete_date, change_information,
                                                  main_wo_flag, create_time, item_version, znp195_flag, wo_type, model, task_no_level )
                                                 VALUES ({sqlLastEditdt}, '{task_no_type}', 'ADDN','WDN1', '{wo_product_line}', '{wo_task_no}',
                                                  '{v_task_no}', '{dt.Rows[k]["subskuno"].ToString()}', '{sub_item}', {wo_qty}, '{wo_rohs}', {sqlStartDate},
                                                  {sqlCompleteDate}, '{wo_change_information}', 'N', SYSDATE, '{wo_item_ver}',
                                                  'N', '{sub_wo_type}', '{model}', '{sub_task_level}' )";
                                        sfcdb.ExecSQL(sql);
                                    }
                                }
                            }

                            result = sfcdb.ORM.Updateable<MESDataObject.Module.HWD.R_7B5_XML_T>().UpdateColumns(r => new MESDataObject.Module.HWD.R_7B5_XML_T { TRANSFER_FLAG = "Y" })
                                .Where(r => r.TASK_NO == wo_task_no && r.TRANSFER_FLAG == "N").ExecuteCommand();
                            if (result == 0)
                            {
                                throw new Exception("Update  Transfer Flag Fail:" + wo_task_no);
                            }
                        }
                        catch (Exception ex)
                        {
                            msg = msg + "Task_NO:" + wo_task_no + ";Error_Msg:" + ex.Message + ";sql:" + sql;
                            //throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sfcdb.ORM.Deleteable<R_SYNC_LOCK>().Where(r => r.LOCK_NAME == "HWD_7B5_SaveWOQty").ExecuteCommand();
            }
        }

        private static DataTable GetShipLinkPo(OleExec sfcdb, string skuno,string task_no)
        {
            string sql = "";
            if (task_no.StartsWith("T"))
            {
                sql = $@" SELECT * FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                             WHERE b.skuno = '{skuno}' AND b.seqno = 8 AND a.hh_item = b.subskuno
                               AND a.task_qty > a.total_plan_qty + a.buffer_qty
                               AND a.task_no = c.task_no AND c.sap_flag = 'Y' AND LEFT (c.po_no, 1) IN ('M', '8')";
            }
            else
            {
                sql = $@" SELECT * FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                             WHERE b.skuno = '{skuno}' AND b.seqno = 8 AND a.hh_item = b.subskuno
                               AND a.task_qty > a.total_plan_qty + a.buffer_qty
                               AND a.task_no = c.task_no AND c.sap_flag = 'Y' AND LEFT (c.po_no, 1) IN ('D', '9')";
            }
            return sfcdb.ExecSelect(sql, null).Tables[0];
        }

        private static DataTable GetShipLinkPoTotalQty(OleExec sfcdb,string task_no, string skuno,double plan_qty)
        {
            string sql = "";
            if (task_no.StartsWith("T"))
            {
                sql = $@"SELECT *
                             FROM (SELECT   b.subskuno, SUM (a.task_qty - a.total_plan_qty - a.buffer_qty ) total_ext_qty
                                FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                               WHERE b.skuno = '{skuno}' AND b.seqno = 8  AND a.hh_item = b.subskuno
                                 AND a.task_qty > a.total_plan_qty + a.buffer_qty  AND a.task_no = c.task_no
                                 AND c.sap_flag = 'Y'   AND LEFT (c.po_no, 1) IN ('M', '8') GROUP BY b.subskuno) m
                             WHERE m.total_ext_qty < {plan_qty}";
            }
            else
            {
                sql = $@"SELECT *
                             FROM (SELECT   b.subskuno, SUM (a.task_qty - a.total_plan_qty - a.buffer_qty ) total_ext_qty
                                FROM r_7b5_ship a, c_sku_link_7b5 b, r_7b5_po c
                               WHERE b.skuno = '{skuno}' AND b.seqno = 8  AND a.hh_item = b.subskuno
                                 AND a.task_qty > a.total_plan_qty + a.buffer_qty  AND a.task_no = c.task_no
                                 AND c.sap_flag = 'Y'   AND LEFT (c.po_no, 1) IN ('D', '9') GROUP BY b.subskuno) m
                             WHERE m.total_ext_qty < {plan_qty}";
            }
            return sfcdb.ExecSelect(sql, null).Tables[0];
        }

        private static DataTable GetShipPOExtQty(OleExec sfcdb,string task_no, string skuno, double plan_qty)
        {
            string sql = "";
            if (task_no.StartsWith("T"))
            {
                sql = $@"SELECT *
                                    FROM (SELECT   b.price,SUM (a.task_qty- a.total_plan_qty- a.buffer_qty) ext_qty
                                              FROM r_7b5_ship a, r_7b5_po b
                                             WHERE a.hh_item = '{skuno}'
                                               AND a.task_no = b.task_no  AND b.sap_flag = 'Y' AND LEFT (b.po_no, 1) IN ('M', '8')
                                               AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                          GROUP BY b.price) m
                                   WHERE m.ext_qty >= {plan_qty}";
            }
            else
            {
                sql = $@"SELECT *
                                    FROM (SELECT   b.price,SUM (a.task_qty- a.total_plan_qty- a.buffer_qty) ext_qty
                                              FROM r_7b5_ship a, r_7b5_po b
                                             WHERE a.hh_item = '{skuno}'
                                               AND a.task_no = b.task_no  AND b.sap_flag = 'Y' AND LEFT (b.po_no, 1) IN ('M', '8')
                                               AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                          GROUP BY b.price) m
                                   WHERE m.ext_qty >= {plan_qty}";
            }
            return sfcdb.ExecSelect(sql, null).Tables[0];
        }

        /// <summary>
        /// 獲取以T開頭的任務令的價格
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="skuno"></param>
        /// <returns></returns>
        private static double GetShipPOPriceStartT(OleExec sfcdb,string skuno)
        {            
            string sql = $@" SELECT n.price 
                                    FROM (SELECT   m.price
                                              FROM (SELECT   b.price,SUM (  a.task_qty- a.total_plan_qty- a.buffer_qty ) ext_qty
                                                        FROM r_7b5_ship a, r_7b5_po b
                                                       WHERE a.hh_item = '{skuno}'  AND a.task_no = b.task_no
                                                         AND b.sap_flag = 'Y' AND LEFT (b.po_no, 1) IN ('M', '8')
                                                         AND a.task_qty > a.total_plan_qty+ a.buffer_qty
                                                    GROUP BY b.price) m,r_7b5_ship c,r_7b5_po d
                                             WHERE m.ext_qty >= 20 AND c.hh_item = '{skuno}'
                                               AND c.task_no = d.task_no  AND d.sap_flag = 'Y'
                                               AND LEFT (d.po_no, 1) IN ('M', '8')
                                               AND c.task_qty >c.total_plan_qty + c.buffer_qty
                                               AND d.price = m.price
                                          ORDER BY c.receive_date) n
                                    WHERE ROWNUM = 1";

            DataTable dt = sfcdb.ExecSelect(sql, null).Tables[0];
            if (dt != null && dt.Rows.Count == 0)
            {
                throw new Exception(skuno + " Get Price Error !");
            }
            return (double)dt.Rows[0][0];
        }
        /// <summary>
        /// 獲取不以T開頭的任務令的價格
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="skuno"></param>
        /// <returns></returns>
        private static double GetShipPOPrice(OleExec sfcdb, string skuno)
        {
            string sql = $@" SELECT n.price  FROM (SELECT   m.price
                              FROM (SELECT b.price, SUM (  a.task_qty- a.total_plan_qty - a.buffer_qty ) ext_qty
                                        FROM r_7b5_ship a, r_7b5_po b
                                       WHERE a.hh_item = '{skuno}'
                                         AND a.task_no = b.task_no  AND b.sap_flag = 'Y'
                                         AND LEFT (b.po_no, 1) IN ('D', '9')
                                         AND a.task_qty >a.total_plan_qty+ a.buffer_qty
                                    GROUP BY b.price) m,
                                   r_7b5_ship c,
                                   r_7b5_po d
                             WHERE m.ext_qty >= 1
                               AND c.hh_item = '{skuno}'
                               AND c.task_no = d.task_no
                               AND d.sap_flag = 'Y'
                               AND LEFT (d.po_no, 1) IN ('D', '9')
                               AND c.task_qty > c.total_plan_qty + c.buffer_qty
                               AND d.price = m.price
                          ORDER BY c.receive_date) n
                   WHERE ROWNUM = 1";

            DataTable dt = sfcdb.ExecSelect(sql, null).Tables[0];
            if (dt != null && dt.Rows.Count == 0)
            {
                throw new Exception(skuno + " Get Price Error !");
            }
            return (double)dt.Rows[0][0];
        }

        private static DataTable GetShipPOTaskQtyByPrice(OleExec sfcdb, string task_no, string skuno, double price)
        {
            string sql = "";
            if (task_no.StartsWith("T"))
            {
                sql = $@" SELECT a.task_no as sub_task_no,
                                                (a.task_qty - a.total_plan_qty - a.buffer_qty) as sub_task_ext,
                                                a.hh_item as sub_hh_item, a.hw_item as sub_hw_item
                                           FROM r_7b5_ship a, r_7b5_po b
                                          WHERE a.hh_item = '{skuno}'  AND a.task_no = b.task_no AND b.sap_flag = 'Y'
                                            AND b.price = {price}  AND LEFT (b.po_no, 1) IN ('M', '8')
                                            AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                            AND a.receive_date =
                                                   (SELECT MIN (a.receive_date)
                                                      FROM r_7b5_ship a, r_7b5_po b
                                                     WHERE a.hh_item = '{skuno}'
                                                       AND a.task_no = b.task_no  AND b.sap_flag = 'Y'
                                                       AND b.price = {price} AND LEFT (b.po_no, 1) IN ('M', '8')
                                                       AND a.task_qty > a.total_plan_qty + a.buffer_qty)
                                            AND ROWNUM = 1";
            }
            else
            {
                sql = $@" SELECT a.task_no as sub_task_no,
                                                (a.task_qty - a.total_plan_qty - a.buffer_qty) as sub_task_ext,
                                                a.hh_item as sub_hh_item, a.hw_item as sub_hw_item
                                           FROM r_7b5_ship a, r_7b5_po b
                                          WHERE a.hh_item = '{skuno}'  AND a.task_no = b.task_no AND b.sap_flag = 'Y'
                                            AND b.price = {price}  AND LEFT (b.po_no, 1) IN ('D', '9')
                                            AND a.task_qty > a.total_plan_qty + a.buffer_qty
                                            AND a.receive_date =
                                                   (SELECT MIN (a.receive_date)
                                                      FROM r_7b5_ship a, r_7b5_po b
                                                     WHERE a.hh_item = '{skuno}'
                                                       AND a.task_no = b.task_no  AND b.sap_flag = 'Y'
                                                       AND b.price = {price} AND LEFT (b.po_no, 1) IN ('D', '9')
                                                       AND a.task_qty > a.total_plan_qty + a.buffer_qty)
                                            AND ROWNUM = 1";
            }
            return sfcdb.ExecSelect(sql, null).Tables[0];
        }

        private static DataTable GetShipPOTaskQtyBy(OleExec sfcdb,string task_no, string skuno)
        {
            string sql = "";
            if (task_no.StartsWith("T"))
            {
                sql = $@"SELECT a.task_no as sub_task_no,
                                  (a.task_qty - a.total_plan_qty - a.buffer_qty) as sub_task_ext,
                                  a.hh_item as sub_hh_item, a.hw_item as sub_hw_item                            
                             FROM r_7b5_ship a, r_7b5_po b
                            WHERE a.hh_item = '{skuno}'
                              AND a.task_qty > a.total_plan_qty + a.buffer_qty
                              AND a.task_no = b.task_no  AND b.sap_flag = 'Y'
                              AND LEFT (b.po_no, 1) IN ('M', '8')
                              AND a.receive_date =
                                     (SELECT MIN (a.receive_date)
                                        FROM r_7b5_ship a, r_7b5_po b
                                       WHERE a.hh_item = '{skuno}'
                                         AND a.task_no = b.task_no  AND b.sap_flag = 'Y'
                                         AND LEFT (b.po_no, 1) IN ('M', '8')
                                         AND a.task_qty > a.total_plan_qty+ a.buffer_qty)
                              AND ROWNUM = 1";
            }
            else
            {
                sql = $@"SELECT a.task_no as sub_task_no,
                                  (a.task_qty - a.total_plan_qty - a.buffer_qty) as sub_task_ext,
                                  a.hh_item as sub_hh_item, a.hw_item as sub_hw_item                            
                             FROM r_7b5_ship a, r_7b5_po b
                            WHERE a.hh_item = '{skuno}'
                              AND a.task_qty > a.total_plan_qty + a.buffer_qty
                              AND a.task_no = b.task_no  AND b.sap_flag = 'Y'
                              AND LEFT (b.po_no, 1) IN ('D', '9')
                              AND a.receive_date =
                                     (SELECT MIN (a.receive_date)
                                        FROM r_7b5_ship a, r_7b5_po b
                                       WHERE a.hh_item = '{skuno}'
                                         AND a.task_no = b.task_no  AND b.sap_flag = 'Y'
                                         AND LEFT (b.po_no, 1) IN ('D', '9')
                                         AND a.task_qty > a.total_plan_qty+ a.buffer_qty)
                              AND ROWNUM = 1";
            }
            return sfcdb.ExecSelect(sql, null).Tables[0];
        }
        /// <summary>
        /// 對應原數據庫中的 ap.todes36 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Todes36(int input)
        {
            string result = "";
            string ch = "";
            int m = input;
            int n = 0;
            string constr = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            while (m > 0)
            {
                n = m % 36;
                m = Convert.ToInt32(Math.Floor(Convert.ToDouble((m / 36))));
                ch = constr.Substring(n, 1);
                result = ch + result;
            }
            return result;
        }
        /// <summary>
        ///  對應原數據庫中的ap.todec36 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int Todec36(string input)
        {
            int result = 0;
            string ch = "";
            int m = 1;
            int n = 0;
            string constr = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string tmp_constr = input.Trim().ToUpper();
            while (m <= tmp_constr.Length)
            {               
                ch = (tmp_constr.Substring(tmp_constr.Length - m)).Substring(0, 1);               
                n = constr.IndexOf(ch);
                result = Convert.ToInt32((n * Math.Pow(36, m - 1))) + result;
                m = m + 1;
            }
            return result;
        }
    }
}
