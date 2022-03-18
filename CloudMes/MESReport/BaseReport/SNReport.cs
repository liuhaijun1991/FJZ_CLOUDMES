using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.BaseReport
{
    //SN 信息報表
    public class SNReport : ReportBase
    {
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null , EnterSubmit = true };
        public SNReport()
        {
            Inputs.Add(SN);
            //string strGetSn = @"SELECT * FROM R_SN WHERE SN='{0}' OR BOXSN='{0}'";
            string strGetSn = @"select sn,a.skuno,workorderno,plant,current_station,next_station,CASE repair_failed_flag WHEN '1' THEN 'YES' ELSE 'NO' END IS_IN_REPAIR,started_flag,start_time,completed_flag,
                                completed_time,packed_flag,packdate,d.pack_no CARTON,e.pack_no PALLET,shipped_flag,shipdate,po_no,cust_order_no,cust_pn,boxsn,
                                scraped_flag,scraped_time,product_status,rework_count,valid_flag,stock_status,stock_in_time,a.edit_time
                                from R_SN a,R_SN_PACKING c,R_PACKING d,R_PACKING e  where  (sn='{0}')
                                and a.id=c.sn_id(+) and c.pack_id=d.id(+) and d.parent_pack_id=e.id(+) and a.valid_flag =1";
            Sqls.Add("strGetSN", strGetSn);
            string strGetSn_Juniper = @"select sn,a.skuno,
                                CASE WHEN exists(SELECT*FROM R_PRE_WO_HEAD rph WHERE rph.WO=a.workorderno) 
                                    then (SELECT rph.groupid FROM R_PRE_WO_HEAD rph WHERE rph.WO=a.workorderno) else ''  end groupid,
                                workorderno,plant,current_station,next_station,started_flag,start_time,completed_flag,
                                completed_time,packed_flag,packdate,d.pack_no CARTON,e.pack_no PALLET,shipped_flag,shipdate,repair_failed_flag,po_no,cust_order_no,cust_pn,boxsn,
                                scraped_flag,scraped_time,product_status,rework_count,valid_flag,stock_status,stock_in_time,a.edit_time
                                from R_SN a,R_SN_PACKING c,R_PACKING d,R_PACKING e  where  (sn='{0}')
                                and a.id=c.sn_id(+) and c.pack_id=d.id(+) and d.parent_pack_id=e.id(+) and a.valid_flag =1";
            Sqls.Add("strGetSn_Juniper", strGetSn_Juniper);
            string strGetSnDetail = @"SELECT  c.panel,a.sn,skuno,a.workorderno,plant,route_name,line,current_station,next_station,device_name,repair_failed_flag, started_flag,
                                     a.edit_time,a.EDIT_EMP,completed_flag,completed_time,packed_flag,packed_time,shipped_flag,shipdate,po_no,cust_order_no,cust_pn,boxsn,
                                    scraped_flag,scraped_time,product_status,rework_count,valid_flag,class_name,start_time FROM R_SN_STATION_DETAIL a,c_route b ,r_panel_sn c
                                    where a.sn=c.sn(+) and a.route_id=b.id and a.SN IN ( SELECT SN FROM R_SN WHERE SN = '{0}') order by a.edit_time ";
            Sqls.Add("strGetSnDetail", strGetSnDetail);
            //string strGetSnKeypart = @"SELECT * FROM R_SN_KEYPART_DETAIL  WHERE R_SN_ID IN (SELECT ID
            //         FROM R_SN  WHERE SN ='{0}')";
            string strGetSnKeypart = @"SELECT * FROM R_SN_KP  WHERE R_SN_ID IN (SELECT ID
                     FROM R_SN  WHERE SN ='{0}')";
            Sqls.Add("strGetSnKeypart", strGetSnKeypart);

            string strGetSnTestRecord = @"SELECT * FROM R_TEST_RECORD WHERE SN='{0}' AND UPPER(state) in ('PASS','FAIL','INCOMPLETE','ABORT') AND TESTATION NOT IN ('DBG') AND MESSTATION NOT IN ('DBG') ORDER BY EDIT_TIME ASC";
            Sqls.Add("strGetSnTestRecord", strGetSnTestRecord);

            string strGetFailInfo = @"SELECT R.SN, 
                                      R.WORKORDERNO, 
                                      R.SKUNO, 
                                      R.FAIL_STATION, 
                                      R.FAIL_TIME,
                                      F.FAIL_CODE,
                                      F.FAIL_LOCATION,
                                      CASE CLOSED_FLAG
                                          WHEN '0' THEN 'PENDING'
                                          ELSE 'REPAIRED'
                                      END REPAIR_STATUS,
                                      A.REPAIR_TIME
                               FROM R_REPAIR_MAIN R 
                                JOIN R_REPAIR_FAILCODE F
                               ON R.ID = F.REPAIR_MAIN_ID
                                LEFT JOIN R_REPAIR_ACTION A
                                ON F.ID = A.REPAIR_FAILCODE_ID
                               WHERE R.SN = '{0}'
                               ORDER BY REPAIR_STATUS ASC";

            Sqls.Add("strGetFailInfo", strGetFailInfo);
        }

        public override void Run()
        {
            if (SN.Value == null)
            {
                throw new Exception("SN Can not be null");
            }
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string runSql;
            string sqlcustomer = $@"select*From c_customer";
            DataTable dcc = SFCDB.RunSelect(sqlcustomer).Tables[0];
            if (dcc.Rows[0]["CUSTOMER_NAME"].ToString() == "JUNIPER")
            {
               runSql = string.Format(Sqls["strGetSn_Juniper"], SN.Value.ToString().ToUpper().Trim());
            }
            else
            {
               runSql = string.Format(Sqls["strGetSN"], SN.Value.ToString().ToUpper().Trim());
            }
            string runSql1 = string.Format(Sqls["strGetSnDetail"], SN.Value.ToString().ToUpper().Trim());
            string runSql2 = string.Format(Sqls["strGetSnKeypart"], SN.Value.ToString().ToUpper().Trim());
            if(this.LoginBU.Equals("HWD"))
            {
                runSql2 = $@"select ID,R_SN_ID,SN,VALUE,PARTNO,KP_NAME,MPN,SCANTYPE,ITEMSEQ,SCANSEQ,DETAILSEQ,STATION,REGEX,to_char(VALID_FLAG) VALID_FLAG,EXKEY1,
                            EXVALUE1,EXKEY2,EXVALUE2,EDIT_TIME,EDIT_EMP,LOCATION from r_sn_Kp where sn='{SN.Value.ToString()}'
                            union
                            select ID,R_SN_ID,SN,keypart_sn as value,part_no as partno,CATEGORY as kp_name,MPN,CATEGORY_NAME as SCANTYPE,
                             SEQ_NO as ITEMSEQ,seq_no as SCANSEQ,seq_no as DETAILSEQ,STATION_NAME as STATION,'' as REGEX,VALID as VALID_FLAG,
                             '' as EXKEY1,'' as EXVALUE1,'' as EXKEY2,'' as EXVALUE2,CREATE_TIME as EDIT_TIME,CREATE_EMP as EDIT_EMP,'' as LOCATION
                             from r_sn_keypart_detail where sn='{SN.Value.ToString()}'";
            }
            string runSql3 = string.Format(Sqls["strGetSnTestRecord"], SN.Value.ToString().ToUpper().Trim());
            string runSql4 = string.Format(Sqls["strGetFailInfo"], SN.Value.ToString().ToUpper().Trim());
            RunSqls.Add(runSql);
            RunSqls.Add(runSql1);
            RunSqls.Add(runSql2);
            RunSqls.Add(runSql3);
            RunSqls.Add(runSql4);
            
         
            try
            {


                DataSet res = SFCDB.RunSelect(runSql);
                DataSet res1 = SFCDB.RunSelect(runSql1);
                DataSet res2 = SFCDB.RunSelect(runSql2);
                DataSet res3 = SFCDB.RunSelect(runSql3);
                DataSet res4 = SFCDB.RunSelect(runSql4);
                ReportTable retTab = new ReportTable();
                DataTable dt = res.Tables[0].Copy();
                //ReportLink link;

                


                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&=ALL&WO=" + row["workorderno"].ToString();
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "WORKORDERNO")
                        {
                            linkRow[dc.ColumnName] = linkURL;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }                    
                    linkTable.Rows.Add(linkRow);
                }



                //foreach (DataRow dr in dt.Rows)
                //{
                //    // ClassName: 'sdsdsdw',Data: { Wo: 'sdsd',Station: '' }
                //    //link = new ReportLink();
                //    //link.ClassName = "MESReport.BaseReport.WoReport";
                //    //link.Data.Add("WO", dr["workorderno"].ToString());
                //    //link.Data.Add("CloseFlag","ALL");                    
                //    //dr["WORKORDERNO"] = link;                   
                //    //linkTable.Rows.Add(dr);                                     
                //}
                

                retTab.LoadData(res.Tables[0], linkTable);



                retTab.Tittle = "OSN";
                //retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                ReportTable retTab1 = new ReportTable();
                retTab1.LoadData(res1.Tables[0], null);
                retTab1.Tittle = "SN DETAIL";
                //retTab1.ColNames.RemoveAt(0);
                Outputs.Add(retTab1);
                ReportTable retTab2 = new ReportTable();
                retTab2.LoadData(res2.Tables[0], null);
                retTab2.Tittle = "SN KEYPARDT";
                retTab2.ColNames.RemoveAt(0);
                Outputs.Add(retTab2);

                ReportTable retTab3 = new ReportTable();
                retTab3.LoadData(res3.Tables[0]);
                retTab3.Tittle = "TEST RECORD";
                retTab3.ColNames.RemoveAt(0);
                Outputs.Add(retTab3);

                ReportTable retTab4 = new ReportTable();
                retTab4.LoadData(res4.Tables[0]);
                retTab4.Tittle = "REPAIR STATUS";
                retTab4.ColNames.RemoveAt(0);
                Outputs.Add(retTab4);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }

    }
}
