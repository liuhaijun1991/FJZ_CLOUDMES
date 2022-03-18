using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SqlSugar;

namespace MES_DCN.Juniper
{
    public class JuniperI605Format
    {
        private SqlSugarClient _db = null;
        public JuniperI605Format(SqlSugarClient db)
        {
            _db = db;
        }
        public DataTable JuniperI605FormatTable(string fileId)
        {
            var dblink = ""; //默認為自己庫，應該報錯
            var manufacturer_name = ""; 
            string bu = _db.Queryable<MESDataObject.Module.C_BU>().Select(r => r.BU).ToList().Distinct().FirstOrDefault();
            switch (bu)
            {
                case "VNJUNIPER":
                    dblink = "@vndcnapdb";
                    manufacturer_name = "FXNVIETNAM";
                    break;
                case "FJZ":
                    dblink = "@jalp";
                    manufacturer_name = "FXNJUAREZ";
                    break;
                default:
                    break;
            }
            #region X
            //var runSql = $@"select '0' as ITEM,
            //                       aa.item_name,
            //                       aa.organization_code,
            //                       aa.sr_instance_code,
            //                       to_char(aa.new_order_quantity) as new_order_quantity,
            //                       aa.subinventory_code,
            //                       aa.lot_number,
            //                       aa.expiration_date,
            //                       aa.deleted_flag,
            //                       aa.cm_part_number,
            //                       aa.item_type,
            //                       aa.ownership,
            //                       aa.nettable_flag,
            //                       aa.group_code,
            //                       bb.MANUFACTURER_PART_NUMBER,
            //                       bb.MANUFACTURER_NAME,
            //                       to_char(nvl(bb.ap_new_order_quantity,0)) as ap_new_order_quantity,
            //                       aa.free_attr4,
            //                       aa.free_attr5,
            //                       aa.free_attr6,
            //                       aa.free_attr7,
            //                       aa.free_attr8,
            //                       aa.free_attr9,
            //                       aa.free_attr10,
            //                       aa.free_attr11,
            //                       aa.free_attr12,
            //                       aa.free_attr13,
            //                       aa.free_attr14,
            //                       aa.free_attr15,
            //                       aa.free_attr16,
            //                       aa.free_attr17,
            //                       aa.free_attr18,
            //                       aa.free_attr19,
            //                       aa.free_attr20
            //                  from r_sap_file_i605 aa
            //                  left join (select a.cust_kp_no,
            //                                    a.mfr_kp_no as MANUFACTURER_PART_NUMBER,
            //                                    b.mfr_name as MANUFACTURER_NAME,
            //                                    nvl(sum(a.ext_qty), 0) as ap_new_order_quantity
            //                               from mes4.r_tr_sn{dblink} a
            //                               left join mes1.c_mfr_config{dblink} b
            //                                 on b.mfr_code = a.mfr_code
            //                              where ((a.location_flag in ('0', '1', 'B', '8') and
            //                                    a.work_flag in ('0', '2', '4')) or
            //                                    (a.location_flag = '2' and a.work_flag = '0' and exists
            //                                     (select 1
            //                                         from mes4.r_tr_sn_wip{dblink} c
            //                                        where c.tr_sn = a.tr_sn)))
            //                                and exists (select 1
            //                                       from r_sap_file_i605 d
            //                                      where d.file_id = '{fileId}'
            //                                        and d.item_name = a.cust_kp_no)
            //                              group by a.cust_kp_no, a.mfr_kp_no, b.mfr_name) bb
            //                    on aa.item_name = bb.cust_kp_no
            //                 where aa.file_id = '{fileId}'
            //                 order by aa.item_name,
            //                          aa.new_order_quantity,
            //                          bb.cust_kp_no,
            //                          bb.MANUFACTURER_PART_NUMBER,
            //                          bb.MANUFACTURER_NAME,
            //                          bb.ap_new_order_quantity";
            #endregion

            var runSql = $@"
                select '0' as item,
                        aa.item_name,
                        aa.organization_code,
                        aa.sr_instance_code,
                        to_char(aa.new_order_quantity) as new_order_quantity,
                        aa.subinventory_code,
                        aa.lot_number,
                        aa.expiration_date,
                        aa.deleted_flag,
                        aa.cm_part_number,
                        aa.item_type,
                        aa.ownership,
                        aa.nettable_flag,
                        aa.group_code,
                        nvl(bb.manufacturer_part_number, cc.manufacturer_part_number) as manufacturer_part_number,
                        nvl(bb.manufacturer_name, cc.manufacturer_name) as manufacturer_name,
                        to_char(nvl(bb.ap_new_order_quantity, 0)) as ap_new_order_quantity,
                        aa.free_attr4,
                        aa.free_attr5,
                        aa.free_attr6,
                        aa.free_attr7,
                        aa.free_attr8,
                        aa.free_attr9,
                        aa.free_attr10,
                        aa.free_attr11,
                        aa.free_attr12,
                        aa.free_attr13,
                        aa.free_attr14,
                        aa.free_attr15,
                        aa.free_attr16,
                        aa.free_attr17,
                        aa.free_attr18,
                        aa.free_attr19,
                        aa.free_attr20
                    from r_sap_file_i605 aa
                    left join (select a.cust_kp_no,
                                    a.mfr_kp_no as manufacturer_part_number,
                                    b.mfr_name as manufacturer_name,
                                    nvl(sum(a.ext_qty), 0) as ap_new_order_quantity
                                from mes4.r_tr_sn{dblink} a
                                left join mes1.c_mfr_config{dblink} b
                                    on b.mfr_code = a.mfr_code
                                where ((a.location_flag in ('0', '1', 'B', '8') and
                                    a.work_flag in ('0', '2', '4')) or
                                    (a.location_flag = '2' and a.work_flag = '0' and exists
                                        (select 1
                                            from mes4.r_tr_sn_wip{dblink} c
                                        where c.tr_sn = a.tr_sn and c.work_time > sysdate - 31)))
                                and exists (select 1
                                        from r_sap_file_i605 d
                                        where d.file_id = '{fileId}'
                                        and d.item_name = a.cust_kp_no)
                                group by a.cust_kp_no, a.mfr_kp_no, b.mfr_name) bb
                    on aa.item_name = bb.cust_kp_no
                    left join (select item_name, manufacturer_part_number, manufacturer_name
                                from (select f.item_name,
                                            f.item_name as manufacturer_part_number,
                                            '{manufacturer_name}' as manufacturer_name,
                                            row_number() over(partition by f.item_name order by f.last_update_time desc) as rn
                                        from r_sap_file_i590 f
                                        where f.make_buy_flag in ('Make', 'KAKE', 'make')
                                        and exists
                                        (select 1
                                                from r_sap_file_i605 g
                                                where g.file_id = '{fileId}'
                                                and g.item_name = f.item_name)) h
                                where h.rn = 1) cc
                    on cc.item_name = aa.item_name
                    where aa.file_id = '{fileId}'
                    order by aa.item_name,
                            aa.new_order_quantity,
                            bb.cust_kp_no,
                            bb.manufacturer_part_number,
                            bb.manufacturer_name,
                            bb.ap_new_order_quantity";

            var dt = _db.Ado.GetDataTable(runSql);

            DataTable dts = new DataTable();
            dts.TableName = "Recodes";
            dts.Columns.Add("ITEM", typeof(int));
            dts.Columns.Add("ITEM_NAME", typeof(string));
            dts.Columns.Add("ORGANIZATION_CODE", typeof(string));
            dts.Columns.Add("SR_INSTANCE_CODE", typeof(string));
            dts.Columns.Add("NEW_ORDER_QUANTITY", typeof(string));
            dts.Columns.Add("SUBINVENTORY_CODE", typeof(string));
            dts.Columns.Add("LOT_NUMBER", typeof(string));
            dts.Columns.Add("EXPIRATION_DATE", typeof(string));
            dts.Columns.Add("DELETED_FLAG", typeof(string));
            dts.Columns.Add("CM_PART_NUMBER", typeof(string));
            dts.Columns.Add("ITEM_TYPE", typeof(string));
            dts.Columns.Add("OWNERSHIP", typeof(string));
            dts.Columns.Add("NETTABLE_FLAG", typeof(string));
            dts.Columns.Add("GROUP_CODE", typeof(string));
            dts.Columns.Add("MANUFACTURER_PART_NUMBER", typeof(string));
            dts.Columns.Add("MANUFACTURER_NAME", typeof(string));
            dts.Columns.Add("AP_NEW_ORDER_QUANTITY", typeof(string));
            dts.Columns.Add("FREE_ATTR4", typeof(string));
            dts.Columns.Add("FREE_ATTR5", typeof(string));
            dts.Columns.Add("FREE_ATTR6", typeof(string));
            dts.Columns.Add("FREE_ATTR7", typeof(string));
            dts.Columns.Add("FREE_ATTR8", typeof(string));
            dts.Columns.Add("FREE_ATTR9", typeof(string));
            dts.Columns.Add("FREE_ATTR10", typeof(string));
            dts.Columns.Add("FREE_ATTR11", typeof(string));
            dts.Columns.Add("FREE_ATTR12", typeof(string));
            dts.Columns.Add("FREE_ATTR13", typeof(string));
            dts.Columns.Add("FREE_ATTR14", typeof(string));
            dts.Columns.Add("FREE_ATTR15", typeof(string));
            dts.Columns.Add("FREE_ATTR16", typeof(string));
            dts.Columns.Add("FREE_ATTR17", typeof(string));
            dts.Columns.Add("FREE_ATTR18", typeof(string));
            dts.Columns.Add("FREE_ATTR19", typeof(string));
            dts.Columns.Add("FREE_ATTR20", typeof(string));

            int i = 0;
            var itemNameTemp = "";
            foreach (DataRow r in dt.Rows)
            {
                var itemNameTemp2 = r["ITEM_NAME"].ToString() + r["new_order_quantity"].ToString();
                DataRow rp = dts.NewRow();
                rp = r;
                if (itemNameTemp != itemNameTemp2) i++;
                rp["ITEM"] = i;
                if (itemNameTemp == itemNameTemp2)
                {
                    rp["AP_NEW_ORDER_QUANTITY"] = r["AP_NEW_ORDER_QUANTITY"];
                    rp["MANUFACTURER_PART_NUMBER"] = r["MANUFACTURER_PART_NUMBER"];
                    rp["MANUFACTURER_NAME"] = r["MANUFACTURER_NAME"];

                    //rp["ITEM_NAME"] = "";
                    //rp["ORGANIZATION_CODE"] = "";
                    //rp["SR_INSTANCE_CODE"] = "";
                    rp["NEW_ORDER_QUANTITY"] = "0";
                    //rp["SUBINVENTORY_CODE"] = "";
                    //rp["LOT_NUMBER"] = "";
                    //rp["EXPIRATION_DATE"] = DBNull.Value;
                    //rp["DELETED_FLAG"] = "";
                    //rp["CM_PART_NUMBER"] = "";
                    //rp["ITEM_TYPE"] = "";
                    //rp["OWNERSHIP"] = "";
                    //rp["NETTABLE_FLAG"] = "";
                    //rp["GROUP_CODE"] = "";
                    //rp["FREE_ATTR1"] = "";
                    //rp["FREE_ATTR2"] = "";
                    //rp["FREE_ATTR3"] = "";
                    //rp["FREE_ATTR4"] = "";
                    //rp["FREE_ATTR5"] = "";
                    //rp["FREE_ATTR6"] = "";
                    //rp["FREE_ATTR7"] = "";
                    //rp["FREE_ATTR8"] = "";
                    //rp["FREE_ATTR9"] = "";
                    //rp["FREE_ATTR10"] = "";
                    //rp["FREE_ATTR11"] = "";
                    //rp["FREE_ATTR12"] = "";
                    //rp["FREE_ATTR13"] = "";
                    //rp["FREE_ATTR14"] = "";
                    //rp["FREE_ATTR15"] = "";
                    //rp["FREE_ATTR16"] = "";
                    //rp["FREE_ATTR17"] = "";
                    //rp["FREE_ATTR18"] = "";
                    //rp["FREE_ATTR19"] = "";
                    //rp["FREE_ATTR20"] = "";
                }
                itemNameTemp = itemNameTemp2;
                dts.ImportRow(rp);
            }
            return dts;
        }
    }
}
