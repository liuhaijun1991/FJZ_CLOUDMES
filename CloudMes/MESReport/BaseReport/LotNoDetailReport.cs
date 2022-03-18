using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="LotNoDetailReport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-05-17 </date>
    /// <summary>
    /// LotNoDetailReport
    /// </sum
    public class LotNoDetailReport:ReportBase
    {
        ReportInput inputLotNo = new ReportInput() { Name = "LotNo", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public LotNoDetailReport()
        {
            Inputs.Add(inputLotNo);            
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        { 
            //base.Run();
            string lotNo = inputLotNo.Value.ToString().Trim();           
            string sqlRun = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow linkRow = null;
            sqlRun = $@"select ls.lot_no,ld.sn,ld.workorderno,ld.sampling,ld.status,ld.fail_code,ld.fail_location,ld.description,
                            /*ld.carton_no*/
                           case when ld.carton_no is null then
                           (select rpk.pack_no from r_sn rsn,r_sn_packing rspk,r_packing rpk where rpk.id=rspk.pack_id and rspk.sn_id=rsn.id and rsn.sn=ld.sn and rsn.valid_flag='1' ) 
                           end carton_no,
                           /*ld.pallet_no,*/
                           case when ld.pallet_no is null then
                           (select rpkk.pack_no from r_sn rsn,r_sn_packing rspk,r_packing rpk ,r_packing rpkk where rpk.id=rspk.pack_id and rspk.sn_id=rsn.id and rsn.sn=ld.sn and rsn.valid_flag='1' and rpkk.id=rpk.parent_pack_id ) 
                           end pallet_no ,
                        ld.edit_emp,ld.create_date from r_lot_detail ld,r_lot_status ls 
                        where ld.lot_id=ls.id and ls.lot_no='{lotNo}'";
            RunSqls.Add(sqlRun);

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                DBPools["SFCDB"].Return(SFCDB);
                linkTable.Columns.Add("LOT_NO");
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("WORKORDERNO");
                linkTable.Columns.Add("SAMPLING");
                linkTable.Columns.Add("STATUS");
                linkTable.Columns.Add("FAIL_CODE");
                linkTable.Columns.Add("FAIL_LOCATION");
                linkTable.Columns.Add("DESCRIPTION");
                linkTable.Columns.Add("CARTON_NO");
                linkTable.Columns.Add("PALLET_NO");
                linkTable.Columns.Add("EDIT_EMP");
                linkTable.Columns.Add("CREATE_DATE");
                for (int i = 0; i < snListTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["LOT_NO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SmtFqcByLotReport&RunFlag=1&LOTNO=" + snListTable.Rows[i]["LOT_NO"].ToString();
                    linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SmtFqcBySnReport&RunFlag=1&WO=" + snListTable.Rows[i]["WORKORDERNO"].ToString()
                                    + "&SN=" + snListTable.Rows[i]["SN"].ToString();
                                    //+ "$STATUS=ALL&From=" + snListTable.Rows[i]["CREATE_DATE"].ToString()
                                    //+ "&To=" + snListTable.Rows[i]["CREATE_DATE"].ToString();
                    linkRow["WORKORDERNO"] = "";
                    linkRow["SAMPLING"] = "";
                    linkRow["FAIL_CODE"] = "";
                    linkRow["FAIL_LOCATION"] = "";
                    linkRow["DESCRIPTION"] = "";
                    linkRow["CARTON_NO"] = "";
                    linkRow["PALLET_NO"] = "";
                    linkRow["EDIT_EMP"] = "";
                    linkRow["CREATE_DATE"] = "";
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(snListTable, linkTable);
                reportTable.Tittle = "SNList";
                //reportTable.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);

            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }

    }
}
