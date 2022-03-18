using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;
namespace MESReport.BaseReport
{
    public class PackingBySNReport : ReportBase
    {
        ReportInput PackNo = new ReportInput { Name = "PackNo_SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        public PackingBySNReport()
        {
            Inputs.Add(PackNo);
        }
        public override void Run()
        {
            DataTable dt = null;
            string pack_no = PackNo.Value.ToString().ToUpper().Trim();
            OleExec sfcdb = null;
            DataRow linkDataRow = null;
            if (pack_no == "" || PackNo.Value == null)
            {
                ReportAlart alart = new ReportAlart("PackNo or SN Can not be null");
                Outputs.Add(alart);
                return;
            }
            DataTable linkTable = new DataTable();
            try
            {
                sfcdb = DBPools["SFCDB"].Borrow();
                string packSql = $@"select * from r_packing where pack_no ='{pack_no}'";
                dt = sfcdb.RunSelect(packSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    packSql = $@"SELECT B.sn,
                                           B.current_station,
                                           B.next_station,
                                           B.skuno,
                                           B.workorderno,
                                           B.CARTON,
                                           rppk.pack_no PALLET,
                                           B.EDIT_TIME
                                      FROM(select rsn.sn,
                                                   rsn.current_station,
                                                  CASE
		                                            WHEN EXISTS(
		                                            SELECT
			                                            SN
		                                            FROM
			                                            R_SN_LOG L
		                                            WHERE
			                                            (L.LOGTYPE = 'RECHECK'
			                                            AND L.FLAG = '0'
			                                            AND L.SN = rsn.SN)
			                                            OR (L.SN = NVL(rpk.PACK_NO, 'N/A') 
			                                            AND L.LOGTYPE = 'RECHECK'
			                                            AND L.FLAG = '0' )) THEN 'RECHECK'
		                                            ELSE RSN.NEXT_STATION
	                                            END AS NEXT_STATION,
                                                   rsn.skuno,
                                                   rsn.workorderno,
                                                   rpk.parent_pack_id,
                                                   rpk.pack_no CARTON,
                                                   RPK.EDIT_TIME
                                              from r_sn rsn, R_SN_PACKING rsnp, r_packing rpk
                                             where rsn.SN = '{pack_no}'
                                               and rsn.id = rsnp.sn_id
                                               and rsn.VALID_FLAG =1
                                               and rsnp.pack_id = rpk.id) B
                                      LEFT JOIN r_packing rppk
                                        ON B.parent_pack_id = rppk.id";
                    dt = sfcdb.RunSelect(packSql).Tables[0];

                    if (dt.Rows.Count == 0)
                    {
                        if (sfcdb != null)
                        {
                            DBPools["SFCDB"].Return(sfcdb);
                        }
                        ReportAlart alart = new ReportAlart("No Data!");
                        Outputs.Add(alart);
                        return;
                        //throw new Exception("No Data!");
                    }
                }
                else if (dt.Rows[0]["PACK_TYPE"].ToString() == "PALLET")
                {
                    packSql = $@"select rsn.sn,
                                    rsn.skuno,
                                    rsn.workorderno,
                                    rsn.current_station,
                                    CASE
		                            WHEN EXISTS(
		                            SELECT
			                            SN
		                            FROM
			                            R_SN_LOG L
		                            WHERE
			                            (L.LOGTYPE = 'RECHECK'
			                            AND L.FLAG = '0'
			                            AND L.SN = rsn.SN)
			                            OR (L.SN = NVL(rcp.PACK_NO, 'N/A') 
			                            AND L.LOGTYPE = 'RECHECK'
			                            AND L.FLAG = '0' )) THEN 'RECHECK'
		                            ELSE RSN.NEXT_STATION
	                            END AS NEXT_STATION,
                                    rcp.pack_no carton,
                                    rp.pack_no pallet,
                                    rcp.edit_time
                                from r_packing rp
                                left join r_packing rcp
                                on rp.id = rcp.parent_pack_id
                                left join r_sn_packing rsp
                                on rcp.id = rsp.pack_id
                                left join r_sn rsn
                                on rsp.sn_id = rsn.id
                                where rp.pack_no = '{pack_no}'
                                and rsn.VALID_FLAG =1
                                order by pallet,carton";
                }
                else
                {
                    packSql = $@"select b.sn,
                                               b.skuno,
                                               b.workorderno,
                                               b.carton,
                                               b.current_station,
                                              CASE
		                                        WHEN EXISTS(
		                                        SELECT
			                                        SN
		                                        FROM
			                                        R_SN_LOG L
		                                        WHERE
			                                        (L.LOGTYPE = 'RECHECK'
			                                        AND L.FLAG = '0'
			                                        AND L.SN = b.SN)
			                                        OR (L.SN = NVL(b.carton, 'N/A') 
			                                        AND L.LOGTYPE = 'RECHECK'
			                                        AND L.FLAG = '0' )) THEN 'RECHECK'
		                                        ELSE b.NEXT_STATION
	                                        END AS NEXT_STATION,
                                               rppk.pack_no pallet,
                                               b.edit_time
                                          from (select rsn.sn,
                                                       rsn.skuno,
                                                       rsn.workorderno,
                                                       rsn.current_station,
                                                       rsn.next_station,
                                                       rp.pack_no carton,
                                                       rp.parent_pack_id,
                                                       rp.edit_time
                                                  from r_packing rp, r_sn_packing rpk, r_sn rsn
                                                 where rp.pack_no = '{pack_no}'
                                                   and rp.id = rpk.pack_id
                                                   and rsn.VALID_FLAG =1
                                                   and rpk.sn_id = rsn.id) b
                                          left join r_packing rppk
                                            on b.parent_pack_id = rppk.id
                                         order by pallet, carton";
                }
                //string packSql = $@"select r.sn,r.skuno,r.workorderno,rp.pack_no,rp.edit_time from r_sn r,r_sn_packing rsp,r_packing rp 
                //                    where rp.id =rsp.pack_id and rsp.sn_id = r.id and rp.pack_no ='{pack_no}'";

                dt = sfcdb.RunSelect(packSql).Tables[0];
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }

                linkTable.Columns.Add("sn");
                linkTable.Columns.Add("skuno");
                linkTable.Columns.Add("workorderno");
                linkTable.Columns.Add("current_station");
                linkTable.Columns.Add("next_station");
                linkTable.Columns.Add("carton");
                linkTable.Columns.Add("pallet");
                linkTable.Columns.Add("edit_time");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    linkDataRow = linkTable.NewRow();
                    //跳轉的頁面鏈接
                    linkDataRow["sn"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + dt.Rows[i]["SN"].ToString();
                    linkDataRow["skuno"] = "";
                    linkDataRow["workorderno"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + dt.Rows[i]["workorderno"].ToString() + "&EventName=";
                    linkDataRow["carton"] = "";
                    linkDataRow["pallet"] = "";
                    linkDataRow["edit_time"] = "";
                    linkTable.Rows.Add(linkDataRow);

                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, linkTable);
                reportTable.Tittle = "SN Packing detail";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }
}
