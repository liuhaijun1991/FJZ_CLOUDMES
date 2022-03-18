using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;
using MESDataObject.Module;

namespace MESReport.BaseReport
{
    public class OBAByLotReport : ReportBase
    {
        ReportInput LotNo = new ReportInput
        {
            Name = "LotNo",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput Packno = new ReportInput
        {
            Name = "Packno",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput Workorderno = new ReportInput
        {
            Name = "Workorderno",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput Sn = new ReportInput
        {
            Name = "Sn",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        public OBAByLotReport()
        {
            Inputs.Add(LotNo);
            Inputs.Add(Packno);
            Inputs.Add(Workorderno);
            Inputs.Add(Sn);
        }

        public override void Run()
        {
            DataTable dt = null;
            string lot_no = LotNo.Value?.ToString();
            string pack_no = Packno.Value?.ToString();
            string wo = Workorderno.Value?.ToString();
            string sn = Sn.Value?.ToString();
            OleExec sfcdb = null;
            DataRow linkDataRow = null;
            if (string.IsNullOrEmpty(lot_no) && string.IsNullOrEmpty(pack_no)&& string.IsNullOrEmpty(wo) && string.IsNullOrEmpty(sn))
            {
                ReportAlart alart = new ReportAlart("Query condition Can not be null");
                Outputs.Add(alart);
                return;
            }
            DataTable linkTable = new DataTable();
            sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                string strSql="";
                if (string.IsNullOrEmpty(lot_no) && !string.IsNullOrEmpty(sn))
                {
                    string sncondi="", wocondi = "", plcondi = "";
                    if (!string.IsNullOrEmpty(pack_no))
                        plcondi = $@" and D.pack_no = '{pack_no.Replace("'", "''")}' ";
                    if (!string.IsNullOrEmpty(wo))
                        wocondi = $@" and B.workorderno = '{wo.Replace("'", "''")}' ";
                    if (!string.IsNullOrEmpty(sn))
                        sncondi = $@" and B.sn = '{sn.Replace("'", "''")}' ";
                    //strSql = $@" SELECT AA.sn,
                    //               bb.carton,
                    //               bb.pallet,
                    //               aa.lot_id,
                    //               decode(aa.status, '', '未抽檢', 1, 'PASS', 0, 'FAIL') status,
                    //               aa.workorderno,
                    //               decode(aa.pallet_no,
                    //                      '',
                    //                      '离线抽检',
                    //                      'SNSAMPLINGCOMPLETED',
                    //                      '抽检完成',
                    //                      'WAITOBA',
                    //                      '待抽检') obatype,
                    //               aa.fail_code,
                    //               aa.fail_location,
                    //               aa.description,
                    //               aa.edit_emp,
                    //               aa.edit_time
                    //          FROM (SELECT A.*, B.ID SNID
                    //                  FROM R_LOT_DETAIL A, R_SN B
                    //                 WHERE A.SN = B.SN {sncondi} {wocondi}) AA
                    //          LEFT JOIN (SELECT C.PACK_NO carton, D.PACK_NO pallet, E.SN_ID
                    //                       FROM R_PACKING C, R_PACKING D, R_SN_PACKING E
                    //                      WHERE C.PARENT_PACK_ID = D.ID {plcondi}
                    //                        AND C.ID = E.PACK_ID) BB
                    //            ON AA.SNID = BB.SN_ID";

                    strSql = $@" SELECT AA.sn,
                                   bb.carton,
                                   bb.pallet,
                                   aa.lot_id,
                                   decode(aa.status, '', 'NoSamplingInspection', 1, 'PASS', 0, 'FAIL') status,
                                   aa.workorderno,
                                   decode(aa.pallet_no,
                                          '',
                                          'OffLineSamplInginspection',
                                          'SNSAMPLINGCOMPLETED',
                                          'SamplingInspectionCompleted',
                                          'WAITOBA',
                                          'WaitSamplInginspection') obatype,
                                   aa.fail_code,
                                   aa.fail_location,
                                   aa.description,
                                   aa.edit_emp,
                                   aa.edit_time
                              FROM (SELECT A.*, B.ID SNID
                                      FROM R_LOT_DETAIL A, R_SN B
                                     WHERE A.SN = B.SN {sncondi} {wocondi}) AA
                              LEFT JOIN (SELECT C.PACK_NO carton, D.PACK_NO pallet, E.SN_ID
                                           FROM R_PACKING C, R_PACKING D, R_SN_PACKING E
                                          WHERE C.PARENT_PACK_ID = D.ID {plcondi}
                                            AND C.ID = E.PACK_ID) BB
                                ON AA.SNID = BB.SN_ID";

                    StringBuilder condi = new StringBuilder(strSql);

                    dt = sfcdb.RunSelect(condi.ToString()).Tables[0];
                }
                else
                {

                    //strSql = $@" select aaa.*,
                    //                decode(rld.status, '', '未抽檢', 1, 'PASS', 0, 'FAIL') status,
                    //                rld.workorderno,
                    //                decode(rld.pallet_no, '', '离线抽检', 'SNSAMPLINGCOMPLETED', '抽检完成', 'WAITOBA', '待抽检') obatype,
                    //                rld.fail_code,
                    //                rld.fail_location,
                    //                rld.description,
                    //                rld.edit_emp,
                    //                rld.edit_time
                    //           from (select rsn.sn, rp1.pack_no carton, rp.pack_no pallet, rls.lot_no
                    //                   from r_lot_status rls,
                    //                        r_lot_pack   rlp,
                    //                        r_sn         rsn,
                    //                        r_sn_packing rsp,
                    //                        r_packing    rp,
                    //                        r_packing    rp1
                    //                  where rls.lot_no = rlp.lotno                                        
                    //                    and rsn.id = rsp.sn_id
                    //                    and rlp.packno = rp.pack_no
                    //                    and rp.id = rp1.parent_pack_id
                    //                    and rp1.id = rsp.pack_id";

                    strSql = $@" select aaa.*,
                                    decode(rld.status, '', 'NoSamplingInspection', 1, 'PASS', 0, 'FAIL') status,
                                    rld.workorderno,
                                    decode(rld.pallet_no, '', 'OffLineSamplInginspection', 'SNSAMPLINGCOMPLETED', 'SamplingInspectionCompleted', 'WAITOBA', 'WaitSamplInginspection') obatype,
                                    rld.fail_code,
                                    rld.fail_location,
                                    rld.description,
                                    rld.edit_emp,
                                    rld.edit_time
                               from (select rsn.sn, rp1.pack_no carton, rp.pack_no pallet, rls.lot_no,rls.id
                                       from r_lot_status rls,
                                            r_lot_pack   rlp,
                                            r_sn         rsn,
                                            r_sn_packing rsp,
                                            r_packing    rp,
                                            r_packing    rp1
                                      where rls.lot_no = rlp.lotno                                        
                                        and rsn.id = rsp.sn_id
                                        and rlp.packno = rp.pack_no
                                        and rp.id = rp1.parent_pack_id
                                        and rp1.id = rsp.pack_id";


                    StringBuilder condi = new StringBuilder(strSql);
                    if (!string.IsNullOrEmpty(lot_no))
                        condi.Append($@" and rls.lot_no = '{lot_no.Replace("'", "''")}' ");
                    if (!string.IsNullOrEmpty(pack_no))
                        condi.Append($@" and rp.pack_no = '{pack_no.Replace("'", "''")}' ");
                    if (!string.IsNullOrEmpty(wo))
                        condi.Append($@" and rsn.workorderno = '{wo.Replace("'", "''")}' ");
                    if (!string.IsNullOrEmpty(sn))
                        condi.Append($@" and rsn.sn = '{sn.Replace("'", "''")}' ");

                    condi.Append($@") aaa
                               left join r_lot_detail rld
                                 on aaa.sn = rld.sn and aaa.id = rld.lot_id");
                    dt = sfcdb.RunSelect(condi.ToString()).Tables[0];
                }

                if (dt.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }

                linkTable.Columns.Add("sn");
                linkTable.Columns.Add("skuno");
                linkTable.Columns.Add("workorderno");
                linkTable.Columns.Add("obatype");
                linkTable.Columns.Add("fail_code");
                linkTable.Columns.Add("fail_location");
                linkTable.Columns.Add("description");
                linkTable.Columns.Add("carton");
                linkTable.Columns.Add("pallet");
                linkTable.Columns.Add("status");
                linkTable.Columns.Add("lotno");
                linkTable.Columns.Add("edit_emp");
                linkTable.Columns.Add("edit_time");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    linkDataRow = linkTable.NewRow();
                    //跳轉的頁面鏈接
                    linkDataRow["sn"] =
                        "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" +
                        dt.Rows[i]["SN"].ToString();
                    linkDataRow["skuno"] = "";
                    linkDataRow["workorderno"] = "";
                    linkDataRow["obatype"] = "";
                    linkDataRow["fail_code"] = "";
                    linkDataRow["fail_location"] = "";
                    linkDataRow["description"] = "";
                    linkDataRow["carton"] = "";
                    linkDataRow["pallet"] = "";
                    linkDataRow["status"] = "";
                    linkDataRow["lotno"] = "";
                    linkDataRow["edit_emp"] = "";
                    linkDataRow["edit_time"] = "";
                    linkTable.Rows.Add(linkDataRow);
                }

                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, linkTable);
                reportTable.Tittle = "OBASAMPLE DETAIL";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
                return;
            }
            finally
            {
                DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
}
