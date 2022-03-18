using MESDataObject.Module;
using MESDataObject.Module.HWD;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.HWD
{
    public class TGMESValueGroup : LabelValueGroup
    {
        public TGMESValueGroup()
        {
            ConfigGroup = "TGMESValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTGMES_VALUE", Description = "根據Packing3獲取R_SN_TGMES_INFO表指定列的值", Paras = new List<string>() { "PACKING3", "COLUMN_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTGMES_QTY", Description = "根據Packing3獲取R_SN_TGMES_INFO表中SN的數量", Paras = new List<string>() { "PACKING3" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTGMES_VOLUME", Description = "根據Qty獲取Size規格", Paras = new List<string>() { "QTY" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTGMES_WEIGHT", Description = "根據Qty和定值獲取Weight(KG)", Paras = new List<string>() { "QTY", "NUMBER" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTGMES_SEQNO", Description = "根據Packing3獲取TO對應分子分母", Paras = new List<string>() { "PACKING3" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTGMES_DNNO", Description = "根據Packing3獲取DN號", Paras = new List<string>() { "PACKING3" } });
        }

        public string GetTGMES_VALUE(OleExec SFCDB, string PACKING3, string COLUMN_NAME)
        {
            string value = "NoData";
            List<string> list = SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == PACKING3 && t.VALID_FLAG == "1").Select<string>(COLUMN_NAME).ToList();
            if (list.Count > 0)
            {
                value = list[0];
            }
            return value;
        }

        public string GetTGMES_QTY(OleExec SFCDB, string PACKING3)
        {
            string value = "NoData";
            var list = SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == PACKING3 && t.VALID_FLAG == "1").ToList();
            if (list.Count > 0)
            {
                value = list.Count.ToString();
            }
            return value;
        }

        public string GetTGMES_VOLUME(OleExec SFCDB, string QTY)
        {
            string value = "NoData";
            if (int.TryParse(QTY, out int O_QTY))
            {
                if (1 <= O_QTY && O_QTY <= 40)
                {
                    value = "1100*1100*446";
                }
                else if (41 <= O_QTY && O_QTY <= 80)
                {
                    value = "1100*1100*727";
                }
                else if (81 <= O_QTY && O_QTY <= 120)
                {
                    value = "1100*1100*1008";
                }
                else if (121 <= O_QTY && O_QTY <= 160)
                {
                    value = "1100*1100*1289";
                }
            }
            return value;
        }

        public string GetTGMES_WEIGHT(OleExec SFCDB, string QTY, string NUMBER)
        {
            string value = "NoData";
            if (int.TryParse(QTY, out int O_QTY) && double.TryParse(NUMBER, out double O_NUMBER))
            {
                value = (O_QTY * O_NUMBER + 10.15).ToString("f3");//保留3位小數
            }
            return value;
        }

        public string GetTGMES_SEQNO(OleExec SFCDB, string PACKING3)
        {
            string lseq = string.Empty, rseq = string.Empty, toNo= string.Empty, sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@"
                select aa.*,
                       row_number() over(partition by aa.to_no order by aa.edit_time) seq
                  from (select distinct o.to_no, q.packing3, min(q.edit_time) edit_time
                          from r_sn_tgmes_info m,
                               r_ship_detail   n,
                               r_to_detail     o,
                               r_ship_detail   p,
                               r_sn_tgmes_info q
                         where m.pcba_barcode = n.sn
                           and n.dn_no = o.dn_no
                           and o.dn_no = p.dn_no
                           and p.sn = q.pcba_barcode
                           and m.valid_flag = '1'
                           and q.valid_flag = '1'
                           and m.packing3 = '{PACKING3}'
                         group by o.to_no, q.packing3) aa";
            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                lseq = dt.Select($@"PACKING3='{PACKING3}'")[0]["seq"].ToString();
                toNo = dt.Rows[0]["to_no"].ToString();
            }
            sql = $@"
                select t.to_no, ceil(nvl(sum(d.qty), 0) / 160) qty
                  from r_to_detail t, r_dn_status d
                 where t.dn_no = d.dn_no
                   and t.to_no = '{toNo}'
                 group by t.to_no";
            dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count > 0)
            {
                rseq = dt.Rows[0]["qty"].ToString();
            }

            return lseq + "/" + rseq;
        }

        public string GetTGMES_DNNO(OleExec SFCDB, string PACKING3)
        {
            string value = "NoData";
            var list = SFCDB.ORM.Queryable<R_SN_TGMES_INFO, R_SHIP_DETAIL>((M, N)=>M.PCBA_BARCODE==N.SN).Where((M, N) => M.PACKING3 == PACKING3 && M.VALID_FLAG == "1").Select((M, N) => N).ToList();
            if (list.Count > 0)
            {
                value = list[0].DN_NO.ToString();
            }
            return value;
        }
    }
}
