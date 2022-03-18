using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;


namespace MESReport.BaseReport
{
    public class NormalBonepileSNList : ReportBase
    {
        ReportInput inputDateFrom = new ReportInput() { Name = "DateFrom", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputDateTo = new ReportInput() { Name = "DateTo", InputType = "DateTime", Value = DateTime.Today.ToString("yyyy-MM-dd"), Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStatus = new ReportInput() { Name = "Status", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Open", "Close" } };
        ReportInput inputCategory = new ReportInput() { Name = "Category", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "Function", "Cosmetic" } };
        ReportInput inputSeries = new ReportInput() { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSubSeries = new ReportInput() { Name = "SubSeries", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputProduct = new ReportInput() { Name = "Product", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        // ReportInput inputIsHard = new ReportInput() { Name = "是否難板", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "YES", "NO" } };
        ReportInput inputIsHard = new ReportInput() { Name = "BadBoard", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "YES", "NO" } };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        ReportTable reportTable = null;
        public NormalBonepileSNList()
        {
            Inputs.Add(inputDateFrom);
            Inputs.Add(inputDateTo);
            Inputs.Add(inputStatus);
            Inputs.Add(inputCategory);
            Inputs.Add(inputSeries);
            Inputs.Add(inputSubSeries);
            Inputs.Add(inputProduct);
            Inputs.Add(inputSN);
            Inputs.Add(inputIsHard);
        }
        public override void Init()
        {
            base.Init();
            string sql = "";
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                DataTable dt = new DataTable();
                sql = "select distinct series_name from c_series";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> listSubSeries = new List<string>();
                listSubSeries.Add("ALL");
                for (int l = 0; l < dt.Rows.Count; l++)
                {
                    listSubSeries.Add(dt.Rows[l][0].ToString());
                }
                inputSubSeries.ValueForUse = listSubSeries;

                sql = "select distinct customer_name from C_CUSTOMER";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> listSeries = new List<string>();
                listSeries.Add("ALL");
                for (int l = 0; l < dt.Rows.Count; l++)
                {
                    listSeries.Add(dt.Rows[l][0].ToString());
                }
                inputSeries.ValueForUse = listSeries;

                sql = "select distinct skuno from c_sku where skuno is not null order by skuno";
                dt = SFCDB.RunSelect(sql).Tables[0];
                List<string> listProduct = new List<string>();
                listProduct.Add("ALL");
                for (int n = 0; n < dt.Rows.Count; n++)
                {
                    listProduct.Add(dt.Rows[n][0].ToString());
                }
                inputProduct.ValueForUse = listProduct;
                reportTable = new ReportTable();
                reportTable.ColNames = new List<string> {
                "PRODUCT_SERIES","SUB_SERIES","PRODUCT_NAME","SKUNO","PRICE","SN","FAIL_STATION","FAIL_DATE","FAILURE_SYMPTOM","DEFECT_DESCRIPTION","REPAIR_ACTION",
                        "REPAIR_DATE","REMARK","BONEPILE_CATEGORY","CURRENT_STATION","SCANBY","CRITICAL_BONEPILE","HARDCOREBOARD","SN_STATUS","CLOSED_DATE","CHECKIN",
                        "CHECKOUT","LOCATION","WORKORDERNO","FIALCOUNT","OWNER"};
                Outputs.Add(new ReportColumns(reportTable.ColNames));
                //PaginationServer = true;
                PaginationServer = false;//why set it true?
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }

        public override void Run()
        {
            base.Run();
            string dateFrom = inputDateFrom.Value.ToString();
            string dateTo = inputDateTo.Value.ToString();
            string status = inputStatus.Value.ToString();
            string category = inputCategory.Value.ToString();
            string series = inputSeries.Value.ToString();
            string subSeries = inputSubSeries.Value.ToString();
            string product = inputProduct.Value.ToString();
            string isHard = inputIsHard.Value.ToString();
            string sn = inputSN.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = GetData(SFCDB, dateFrom, dateTo, status, category, series, subSeries, product, isHard, sn);
                if (PaginationServer)
                {
                    reportTable.MakePagination(dt, null, PageNumber, PageSize);
                }
                else
                {
                    reportTable.LoadData(dt, null);
                }
                reportTable.Tittle = "Normal Bonepile SN List Report";
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public override void DownFile()
        {
            string dateFrom = inputDateFrom.Value.ToString();
            string dateTo = inputDateTo.Value.ToString();
            string status = inputStatus.Value.ToString();
            string category = inputCategory.Value.ToString();
            string series = inputSeries.Value.ToString();
            string subSeries = inputSubSeries.Value.ToString();
            string product = inputProduct.Value.ToString();
            string isHard = inputIsHard.Value.ToString();
            string sn = inputSN.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataTable dt = GetData(SFCDB, dateFrom, dateTo, status, category, series, subSeries, product, isHard, sn);
                string content = MESPubLab.Common.ExcelHelp.ExportExcelToBase64String(dt);
                string fileName = "NormalSN_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                Outputs.Add(new ReportFile(fileName, content));
            }
            catch (Exception exception)
            {
                Outputs.Add(new ReportAlart(exception.Message));
            }
            finally
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public DataTable GetData(OleExec SFCDB, string dateFrom, string dateTo, string status, string category, string series, string subSeries, string product, string isHard, string sn)
        {
            if (dateFrom.Length > 0)
            {
                if (Convert.ToInt64(Convert.ToDateTime(dateFrom).ToString("yyyyMMdd")) > Convert.ToInt64(Convert.ToDateTime(dateTo).ToString("yyyyMMdd")))
                {
                   // throw new Exception("Date From不能大於Date To!");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816152948"));
                }
            }
            if (status.ToUpper() == "OPEN" && dateFrom.Length > 0)
            {
                // throw new Exception("查詢Open狀態時Date From必須為空!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816154521"));
            }
            if (status.ToUpper() != "OPEN" && dateFrom.Length == 0)
            {
                //throw new Exception("查詢非Open狀態時Date From不能空!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816154645"));
            }
            // string toSql = $@" to_date('{dateTo}','yyyy/mm/dd hh24:mi:ss')+1";
            //為啥日期加1，用戶查出來數據對不上，去掉了
            //string toSql = $@" to_date('{dateTo}','yyyy/mm/dd hh24:mi:ss')";
            string toSql = "";
            bool isVERTIV = SFCDB.ORM.Queryable<MESDataObject.Module.C_BU>().Any(c => c.BU == "VERTIV");
            if(isVERTIV)
            {
                //VERTIV要求查詢的輸入時間點的狀態(close or open),而close 的時間都是比open的時間晚，
                //如果輸入的時間是在open 和close之間，且SN實際已經Close,不加1的話，查出來的數據就不對
                toSql = $@" to_date('{dateTo}','yyyy/mm/dd hh24:mi:ss')+1";
            }
            else
            {
                toSql = $@" to_date('{dateTo}','yyyy/mm/dd hh24:mi:ss')";
            }

            string runSql = $@"select a.PRODUCT_SERIES,a.SUB_SERIES,a.product_name,a.skuno,a.price,a.sn,a.fail_station,a.fail_date,a.failure_symptom,
                                    a.defect_description,a.repair_action,a.repair_date,a.remark,a.bonepile_category,a.current_station,a.lastedit_by as Scanby,
                                    case  when a.critical_bonepile_flag='0' then 'N'
                                      when a.critical_bonepile_flag=null then 'N'
                                      when a.critical_bonepile_flag='' then 'N'
                                      else 'Y'  end as  Critical_bonepile,
                                    case when a.hardcore_board =null  then 'N'  
                                       when a.hardcore_board='' then 'N'
                                         ELSE 'Y' end as HardcoreBoard,
                                    case when a.closed_flag =null  then 'N' 
                                      when a.closed_flag='0' then 'Open' 
                                       when a.closed_flag='' then 'Open'
                                         ELSE 'Close' end as SN_STATUS,
                                    a.closed_date,
                                    '' as checkin,
                                    '' as checkout,
                                    a.FAIL_LOCATION location,
                                    a.kpsn,
                                    a.compomentid,
                                    a.workorderno,
                                    a.fail_count fialcount,
                                     /*CASE 
                                      WHEN a.fail_station IN('SMT1','SMT2','AOI1','AOI2') THEN 'SMT'
                                      WHEN a.fail_station NOT IN ('SMT1','SMT2','AOI1','AOI2') AND  a.current_station<>'MRB' AND a.in_time IS NOT NULL AND a.out_time IS NULL  THEN 'RE'
                                      WHEN a.fail_station IN('5DX','ICT','VI','PRESS-FIT2','PRESS-FIT','PRESS-FIT1','FP','PRE-ASSY','BS','UBOOT','GLUE','PFT','FBFT','FLYINGPROBETEST',
                                        'ICT1','PCBDL1','PCBDL2','PCBFT','PCBPT','AOI3','PCBFT','COATING','ICT2','PCBDL','PCBFT1','PCBFT2','PTH') 
                                       AND a.current_station<>'MRB'  AND a.in_time IS NOT NULL AND a.out_time IS not NULL
                                        THEN 'PTH'
                                    ELSE 'SI' 
                                      end as owner*/
                                     '' as owner
                                     from (select  rnb.PRODUCT_SERIES,rnb.SUB_SERIES,rnb.product_name,rnb.skuno,rnb.price,rnb.sn,rnb.fail_station,rnb.fail_date,rnb.failure_symptom,
                                    rnb.defect_description,rnb.repair_action,rnb.repair_date,rnb.remark,rnb.bonepile_category,rnb.current_station,rnb.lastedit_by,
                                    rnb.critical_bonepile_flag,
                                    rnb.hardcore_board,
                                    rnb.closed_flag,
                                    rnb.closed_date,                                   
                                    rfc.FAIL_LOCATION,
                                    rnb.workorderno,
                                    cfs.fail_count ,
                                    rfc.kpsn,
                                    rfc.compomentid from r_normal_bonepile rnb
                                     left join (
                                      select sn,fail_station,fail_time,FAIL_LOCATION,kpsn,compomentid
                                        from (select rmain.sn,
                                                     rmain.fail_station,
                                                     rmain.fail_time,
                                                     rra.FAIL_LOCATION,
                                                     rra.keypart_sn kpsn,/*VNDCN PQE Asked Add By ZHB 2021-9-28*/
                                                     rra.compomentid,
                                                     row_number() over(partition by rmain.sn, rmain.fail_station order by rmain.fail_time desc) as rownums
                                                from r_repair_main rmain, r_repair_failcode rrf,r_repair_action rra
                                               where rmain.id = rrf.repair_main_id and rrf.id=rra.repair_failcode_id(+))
                                       where rownums = 1  
                                         ) rfc
                                        on rnb.sn = rfc.sn                  
                                       and rnb.fail_station = rfc.fail_station 
                                    left join
                                    (select sn,fail_station,count(*) fail_count from r_repair_main group by sn,fail_station) cfs
                                    on rnb.sn=cfs.sn and rnb.fail_station=cfs.fail_station 
                                    ) a  where 1=1 ";

            if (category.ToUpper() != "ALL")
            {
                runSql = runSql + $@" and a.bonepile_category='{category}'";
            }
            if (series.ToUpper() != "ALL")
            {
                runSql = runSql + $@" and a.PRODUCT_SERIES='{series}'";
            }
            if (subSeries.ToUpper() != "ALL")
            {
                runSql = runSql + $@" and a.SUB_SERIES='{subSeries}'";
            }
            if (product.ToUpper() != "ALL")
            {
                runSql = runSql + $@" and a.product_name='{product}'";
            }
            if (isHard.ToUpper() == "Y")
            {
                runSql = runSql + $@" and a.hardcore_board='Y'";
            }
            else if (isHard.ToUpper() == "N")
            {
                runSql = runSql + $@" and (a.hardcore_board ='N' or a.hardcore_board='' or a.hardcore_board=null)";
            }

            if (sn.Length > 0)
            {
                runSql = runSql + $@" and a.sn = '{sn}'";
            }
            else
            {
                switch (status.ToUpper())
                {
                    case "OPEN":
                        {
                            runSql = runSql + $@" and a.fail_date<{toSql} and (a.closed_date >= {toSql} or a.closed_date is null or a.closed_date='')";
                        }
                        break;
                    case "CLOSE":
                        {
                            runSql = runSql + $@" and a.fail_date<{toSql} and a.fail_date>= to_date('{dateFrom}','yyyy/mm/dd hh24:mi:ss')";
                        }
                        break;
                    case "ALL":
                        {
                            runSql = runSql + $@" and a.fail_date<{toSql} and a.fail_date>= to_date('{dateFrom}','yyyy/mm/dd hh24:mi:ss')";
                            string sql_temp = runSql;

                            runSql = runSql + $@" and a.fail_date<{toSql} and (a.closed_date >= {toSql} or a.closed_date is null or a.closed_date='')"
                                + " union " + sql_temp + $@" and a.fail_date<{toSql} and a.fail_date>= to_date('{dateFrom}','yyyy/mm/dd hh24:mi:ss')";
                        }
                        break;
                }

            }
            runSql = $@"select distinct PRODUCT_SERIES,SUB_SERIES,PRODUCT_NAME,SKUNO,PRICE,SN,FAIL_STATION,FAIL_DATE,FAILURE_SYMPTOM,DEFECT_DESCRIPTION,REPAIR_ACTION
                        ,REPAIR_DATE,REMARK,BONEPILE_CATEGORY,CURRENT_STATION,SCANBY,CRITICAL_BONEPILE,HARDCOREBOARD,SN_STATUS,CLOSED_DATE,CHECKIN,
                        CHECKOUT,LOCATION,KPSN,COMPOMENTID,WORKORDERNO,FIALCOUNT,OWNER from ({runSql})";
            DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new Exception("No Data!");
            }
            string sqlTransfer = "";
            string sn1 = "";
            DataTable dtTransfer;
            bool IsWaitCheckOut = false, IsSMT = false, IsRE = false, IsPTH = false;
            MESDataObject.Module.T_R_FUNCTION_CONTROL control = new MESDataObject.Module.T_R_FUNCTION_CONTROL(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            List<string> listSMT = control.GetListByFcv("NormalBonepileOwner", "SMT", SFCDB).Select(r => r.VALUE).ToList();
            List<string> listPTH = control.GetListByFcv("NormalBonepileOwner", "PTH", SFCDB).Select(r => r.VALUE).ToList();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                //}
                //foreach (DataRow dr in dt.Rows)
                //{
                IsWaitCheckOut = false;
                IsSMT = false;
                IsRE = false;
                IsPTH = false;

                sn1 = dr["SN"].ToString();
                sqlTransfer = $@"select sn,station_name,to_char(in_time,'yyyy/mm/dd hh24:mi:ss') as in_time,to_char(out_time,'yyyy/mm/dd hh24:mi:ss') as out_time
                                from (select sn,station_name,in_time,out_time,row_number() over(partition by sn, station_name order by in_time desc) as rownums
                                from r_repair_transfer) where sn='{ sn1}' and station_name='{ dr["CURRENT_STATION"].ToString()}'  and rownums = 1 ";

                dtTransfer = SFCDB.RunSelect(sqlTransfer).Tables[0];
                if (dtTransfer.Rows.Count == 0)
                {
                    sqlTransfer = $@"select sn,station_name,to_char(in_time,'yyyy/mm/dd hh24:mi:ss') as in_time,to_char(out_time,'yyyy/mm/dd hh24:mi:ss') as out_time
                                from (select sn,station_name,in_time,out_time,row_number() over(partition by sn, station_name order by in_time desc) as rownums
                                from r_repair_transfer) where sn='{ sn1}' and station_name='{ dr["FAIL_STATION"].ToString()}'  and rownums = 1 ";
                    dtTransfer = SFCDB.RunSelect(sqlTransfer).Tables[0];
                }
                if (dtTransfer.Rows.Count > 0)
                {
                    dr["CHECKIN"] = dtTransfer.Rows[0]["IN_TIME"];
                    dr["CHECKOUT"] = dtTransfer.Rows[0]["OUT_TIME"];
                }
                else
                {
                    dr["CHECKIN"] = "";
                    dr["CHECKOUT"] = "";
                }

                IsWaitCheckOut = SFCDB.ORM.Queryable<MESDataObject.Module.R_REPAIR_TRANSFER>().Where(r => r.SN == sn1 && r.CLOSED_FLAG == "0").Any();

                if (listSMT.Contains(dr["FAIL_STATION"].ToString()))
                {
                    dr["OWNER"] = "SMT";
                    IsSMT = true;
                }
                else if (!IsSMT && IsWaitCheckOut)
                {
                    dr["OWNER"] = "RE";
                    IsRE = true;
                }
                else if (!IsSMT && !IsRE && listPTH.Contains(dr["FAIL_STATION"].ToString()))
                {
                    dr["OWNER"] = "PTH";
                    IsPTH = true;
                }
                else if (!IsSMT && !IsPTH && !IsRE)
                {
                    dr["OWNER"] = "SI";
                }

                var failDate = Convert.ToDateTime(dr["FAIL_DATE"]);
                var repairAction = SFCDB.ORM.Queryable<MESDataObject.Module.R_REPAIR_FAILCODE, MESDataObject.Module.R_REPAIR_ACTION>((T1, T2) => T1.ID == T2.R_FAILCODE_ID).Where((T1, T2) => T1.SN == sn1 && T1.FAIL_TIME == failDate).Select((T1, T2) => T2).First();
                if (repairAction != null)
                {
                    var actionObj = SFCDB.ORM.Queryable<MESDataObject.Module.C_ACTION_CODE>().Where(t => t.ACTION_CODE == repairAction.ACTION_CODE).ToList().FirstOrDefault();
                    dr["DEFECT_DESCRIPTION"] = repairAction.DESCRIPTION;
                    dr["REPAIR_ACTION"] = actionObj == null ? "" : actionObj.ENGLISH_DESC;
                    dr["REPAIR_DATE"] = repairAction.REPAIR_TIME;
                }
            }
            return dt;
        }
    
    }
}
