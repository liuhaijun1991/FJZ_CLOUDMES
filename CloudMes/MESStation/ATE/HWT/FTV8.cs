using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.ATE.HWT
{
    public class FTV8 : MesAPIBase
    {
        protected APIInfo FPrepareTest = new APIInfo()
        {
            FunctionName = "PrepareTest",
            Description = "PrepareTest",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "strsn", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "workorder", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "stratename", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strtbsname", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strplatformversion", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strworkproc", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "strworkstation", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "stresn", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "stratpproduct", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strusername", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "stripaddress", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Line", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FReportTestResult = new APIInfo()
        {
            FunctionName = "ReportTestResult",
            Description = "ReportTestResult",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() {InputName = "strsn", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "workorder", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "stratename", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strtbsname", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strplatformversion", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strworkproc", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strworkstation", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "stresn", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "stratpproduct", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strusername", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "stripaddress", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "strtestresult", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Line", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public FTV8()
        {
            Apis.Add(FPrepareTest.FunctionName, FPrepareTest);
            Apis.Add(FReportTestResult.FunctionName, FReportTestResult);
        }
        public void ReportTestResult(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            string strsn = Data["strsn"].ToString();
            string workorder = Data["workorder"].ToString();
            string stratename = Data["stratename"].ToString();
            string strtbsname = Data["strtbsname"].ToString();
            string strplatformversion = Data["strplatformversion"].ToString();
            string strworkproc = Data["strworkproc"].ToString();
            string strworkstation = Data["strworkstation"].ToString();
            string stresn = Data["stresn"].ToString();
            string stratpproduct = Data["stratpproduct"].ToString();
            string strusername = Data["strusername"].ToString();
            string stripaddress = Data["stripaddress"].ToString();
            string strtestresult = Data["strtestresult"].ToString();
            string Line = Data["Line"].ToString();
            try
            {
                string var_sysserialno = "";
                string var_workorderno, var_nextevent, var_atename;

                SN sn = new SN();
                //获取TE Station
                var TE_STATION = strworkproc + strworkstation;
                var SFC_STATION = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().
                    Where(t => t.TEGROUP == "FT" && t.TE_STATION == TE_STATION).Select(t => t.MES_STATION).First();
                if (SFC_STATION == null)
                {
                    throw new Exception($@"Can't find mapping '{TE_STATION}' in C_TEMES_STATION_MAPPING");
                }
                if (TE_STATION == "ORT")
                {
                    sn.Load(strsn, SFCDB, DBTYPE);
                }
                else
                {
                    var three_flag = "0";
                    if (SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == strsn && t.VALID_FLAG == 1).Any())
                    {
                        var sns1 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == strsn && t.VALID_FLAG == 1).Select(t => t.SN).ToList();
                        var sns2 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => sns1.Contains(t.VALUE) && t.VALID_FLAG == 1).Select(t => t.SN).ToList();
                        var sku1 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => sns2.Contains(t.VALUE) && t.VALID_FLAG == 1).Select(t => t.PARTNO).First();


                        if (sku1 != null)
                        {
                            if (SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == sku1 &&
                             t.CATEGORY.ToUpper() == "c_ft_special_skuno_config".ToUpper()).Any())
                            {
                                var_sysserialno = sns2[0];
                                three_flag = "1";
                                sn.Load(var_sysserialno, SFCDB, DBTYPE);
                            }

                        }

                    }
                    if (three_flag == "0")
                    {
                        //检查是否顶级SN
                        if (SFCDB.ORM.Queryable<R_RELATION_DATA>().Where(t => t.SON == strsn).Any())
                        {
                            throw new Exception($@"條碼必須是最高父項");
                        }

                        sn.Load(strsn, SFCDB, DBTYPE);
                        var_sysserialno = sn.baseSN.SN;
                    }

                    var_workorderno = sn.WorkorderNo;
                    var_nextevent = sn.NextStation;
                }
                var_atename = stratename;
                //10分钟内不良必须换机台重测
                if (SFCDB.ORM.Queryable<R_TESTRECORD>().
                    Where(t => t.SN == var_sysserialno && t.DEVICE == stratename && t.TEST_TIME > DateTime.Now.AddMinutes(-10)).Any())
                {
                    throw new Exception($@"10分钟内不良必须换机台重测");
                }

                //如RMA產品未有維修動作，不可測試過站

                //bool isrma = false;
                SKU sku = new SKU();
                sku.Init(sn.SkuNo, SFCDB, DBTYPE);
                if (sku.IsRMASkuno(SFCDB, sn.SkuNo))
                {
                    if (!SFCDB.ORM.Queryable<R_RMA_DETAIL>().Where(t => t.SN == var_sysserialno).Any() ||
                        !SFCDB.ORM.Queryable<R_RMA_DETAIL, R_RELATIONDATA_EXTERNAL>((D, E) => new object[] { JoinType.Left, D.SN == E.SN })
                        .Where((D, E) => E.PARENT == var_sysserialno || E.PARENT == var_sysserialno).Any())
                    {
                        throw new Exception($@"此SN沒有RMA維修記錄,請聯繫RMA人員");
                    }                    
                }

                //查询机台是否被锁
                var Device = SFCDB.ORM.Queryable<R_SFCATE_CONFIG>().Where(t => t.ATENAME == var_atename && t.STATION != "ORT").First();
                if (Device == null)
                {
                    throw new Exception($@"'{var_atename}'未配置R_SFCATE_CONFIG");
                }
                if (Device.STATUS == "停用")
                {
                    throw new Exception($@"'{var_atename}'因为'{Device.DESCRIPTION}'停用");
                }
                //SN锁定检查
                sn.LockCheck(SFCDB);
                if (sn.baseSN.COMPLETED_FLAG != "0")
                {
                    throw new Exception($@"此SN已完工或入MRB");
                }
                if (sn.baseSN.REPAIR_FAILED_FLAG != "0")
                {
                    throw new Exception($@"此SN待出维修");
                }

                if (sn.baseSN.SHIPPED_FLAG != "0")
                {
                    throw new Exception($@"此SN已出货");
                }
                //检查治具
                //暂时忽略

                if (sn.NextStation != SFC_STATION)
                {
                    throw new Exception($@"此板當前工位'{sn.NextStation}'與測試工序'{TE_STATION}'不一致");
                }

                //r_ft_tbs
                var var_SKUNO = sn.SkuNo;
                if (!SFCDB.ORM.Queryable<R_FT_TBS>().
                    Where(t => t.MODEL_NAME == var_SKUNO && (t.TBS_NAME == strtbsname || t.TBS_NAME == var_atename) && t.STATION_NAME == TE_STATION).Any())
                {
                    throw new Exception($@"Skuno:'{var_SKUNO}',Station:'{var_atename}',TBS:'{TE_STATION}' 未配置");
                }
                var_nextevent = sn.NextStation;
                var var_tbs_name = SFCDB.ORM.Queryable<R_FT_TBS>().
                    Where(t => t.MODEL_NAME == var_SKUNO && t.STATION_NAME == var_nextevent)
                    .Select(t => t.TBS_NAME).First();

                var failCount = SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == var_sysserialno && t.STATE == "FAIL").Count();
                if (failCount >= 2)
                {
                    if (!SFCDB.ORM.Queryable<R_REPAIR_MAIN>().Where(t => t.SN == var_sysserialno && t.FAIL_STATION == "ORT").Any())
                    {
                        throw new Exception($@"SN ORT 有兩次Fail記錄,但沒有入維修記錄");
                    }
                }

                //var var_SKUNO = sn.SkuNo;
                if (!SFCDB.ORM.Queryable<R_FT_TBS>().
                    Where(t => t.MODEL_NAME == var_SKUNO && (t.TBS_NAME == strtbsname || t.TBS_NAME == var_atename) && t.STATION_NAME == TE_STATION).Any())
                {
                    throw new Exception($@"Skuno:'{var_SKUNO}',Station:'{var_atename}',TBS:'{TE_STATION}' 未配置");
                }

                //写测试记录
                R_TEST_RECORD TR = new R_TEST_RECORD();
                TR.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_RECORD");
                TR.R_SN_ID = sn.ID;
                TR.MESSTATION = SFC_STATION;
                TR.ENDTIME = DateTime.Now;
                TR.EDIT_TIME = DateTime.Now;
                TR.EDIT_EMP = LoginUser.EMP_NO;
                TR.TESTATION = TE_STATION;
                TR.DEVICE = var_atename;
                TR.SN = sn.baseSN.SN;
                TR.TEGROUP = "FT";
                TR.STATE = "PASS";
                TR.TESTINFO = strtbsname;
                if (!(strtestresult == "PASS"))
                {
                    TR.STATE = "FAIL";
                }
                SFCDB.ORM.Insertable<R_TEST_RECORD>(TR).ExecuteCommand();

                SFCDB.BeginTrain();
                var test_records = SFCDB.ORM.Queryable<R_TEST_RECORD>().
                        Where(t => t.DEVICE == var_atename && t.ENDTIME > DateTime.Now.AddDays(-2)).OrderBy(t => t.ENDTIME).Take(2).ToList();
                bool VAR_TRANTYPE = false;
                if (strtestresult == "PASS")
                {
                    VAR_TRANTYPE = true;
                }
                if (!VAR_TRANTYPE)
                {

                    //同一個機台最後2次失敗，這一次又失敗，說明這個機台 測試連續失敗 3PCS, 
                    //鎖定該機台

                    if (test_records.Count >= 2)
                    {
                        if (test_records[0].STATE == "FAIL" && test_records[1].STATE == "FAIL")
                        {
                            //连续3PCS不良
                            Device = SFCDB.ORM.Queryable<R_SFCATE_CONFIG>().Where(t => t.ATENAME == var_atename && t.STATION != "ORT").First();
                            Device.DESCRIPTION = "此機台已連續3PCS不良，請通知TE處理！";
                            Device.DESC_REASION = "此機台已連續3PCS不良，請通知TE處理！";
                            Device.STATUS = "停用";
                            Device.LOCK_TIME = DateTime.Now;
                            Device.LOCK_STATUS = 1;
                            Device.EDIT_EMP = LoginUser.EMP_NO;
                            Device.EDIT_TIME = DateTime.Now;
                            SFCDB.ORM.Updateable<R_SFCATE_CONFIG>(Device).Where(t => t.ID == Device.ID).ExecuteCommand();
                            R_SFCATE_CONFIG_WIP DW = new R_SFCATE_CONFIG_WIP();
                            DW.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SFCATE_CONFIG_WIP");
                            DW.ATENAME = var_atename;
                            DW.STATUS = "停用";
                            DW.DESCRIPTION = "此機台已連續3PCS不良，請通知TE處理！";
                            DW.DESC_REASION = "此機台已連續3PCS不良，請通知TE處理！";
                            DW.LOCK_STATUS = 1;
                            DW.LOCK_TIME = DateTime.Now;
                            DW.LOCK_BY = LoginUser.EMP_NO;
                            DW.EDIT_EMP = DW.LOCK_BY;
                            DW.EDIT_TIME = DateTime.Now;
                            SFCDB.ORM.Insertable<R_SFCATE_CONFIG_WIP>(DW).ExecuteCommand();


                        }
                    }
                }
                T_R_SN TRSN = new T_R_SN(SFCDB, DBTYPE);
                test_records = SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == strsn && t.MESSTATION == SFC_STATION).OrderBy(t => t.ENDTIME).Take(3).ToList();
                if (VAR_TRANTYPE)
                {


                    if (test_records.Count == 1)
                    {
                        //直接过站
                        TRSN.PassStation(strsn, Line, SFC_STATION, var_atename, BU, "PASS", LoginUser.EMP_NO, SFCDB);
                    }
                    else
                    {
                        if (test_records[1].STATE != "PASS")
                        {

                            var config_Pass_count = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == var_SKUNO && t.CATEGORY == "TEST_PASS_COUNT" && t.STATION_NAME == SFC_STATION).First();
                            if (config_Pass_count != null)
                            {
                                if (config_Pass_count.VALUE == "1")
                                {
                                    TRSN.PassStation(strsn, Line, SFC_STATION, var_atename, BU, "PASS", LoginUser.EMP_NO, SFCDB);
                                }
                                else
                                {
                                    throw new Exception($@"'{var_SKUNO}'未配置重测过站,TEST_PASS_COUNT == {config_Pass_count.VALUE}");
                                }
                            }
                            else
                            {
                                throw new Exception($@"'{var_SKUNO}'未配置重测一次过站,TEST_PASS_COUNT == null");
                            }
                        }
                        else
                        {
                            TRSN.PassStation(strsn, Line, SFC_STATION, var_atename, BU, "PASS", LoginUser.EMP_NO, SFCDB);
                        }

                    }
                }

                //UPH
                TRSN.RecordUPH(sn.WorkorderNo, 1, sn.baseSN.SN, VAR_TRANTYPE ? "PASS" : "FAIL", Line, SFC_STATION, LoginUser.EMP_NO, BU, SFCDB);
                TRSN.RecordYieldRate(sn.WorkorderNo, 1, sn.baseSN.SN, VAR_TRANTYPE ? "PASS" : "FAIL", Line, SFC_STATION, LoginUser.EMP_NO, BU, SFCDB);

                //自动锁板
                if (!VAR_TRANTYPE)
                {
                    TRSN.PassStation(strsn, Line, SFC_STATION, var_atename, BU, "FAIL", LoginUser.EMP_NO, SFCDB);
                    //var SERIES = SFCDB.ORM.Queryable<C_SERIES>().Where(t => t.ID == sku.CSeriesId).First();
                    if (SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == var_SKUNO && t.CATEGORY == "CTRL_LOCK" && t.STATION_NAME == SFC_STATION).Any())
                    {
                        R_SN_LOCK LOCK = new R_SN_LOCK();
                        LOCK.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOCK");
                        LOCK.SN = strsn;
                        LOCK.LOCK_EMP = LoginUser.EMP_NO;
                        LOCK.LOCK_REASON = $@"'{SFC_STATION}'测试不良自动锁定";
                        LOCK.LOCK_STATUS = "1";
                        LOCK.LOCK_TIME = DateTime.Now;
                        LOCK.TYPE = "SN";
                        LOCK.LOCK_STATION = SFC_STATION;
                        SFCDB.ORM.Insertable<R_SN_LOCK>(LOCK).ExecuteCommand();
                    }
                    //不良大于2次自动入维修
                    int Fail_count = 0;
                    for (int i = 0; i < test_records.Count; i++)
                    {
                        if (test_records[i].STATE == "FAIL")
                        {
                            Fail_count++;
                        }
                    }
                    if (Fail_count >= 2)
                    {
                        if (sn.RepairFailedFlag != "1")
                        {
                            sn.baseSN.REPAIR_FAILED_FLAG = "1";

                            R_REPAIR_MAIN RRM = new R_REPAIR_MAIN()
                            {
                                SN = strsn,
                                CLOSED_FLAG = "0",
                                CREATE_TIME = DateTime.Now,
                                WORKORDERNO = sn.WorkorderNo,
                                FAIL_DEVICE = var_atename,
                                REPAIRING_FLAG = "0",
                                FAIL_STATION = SFC_STATION,
                                SKUNO = var_SKUNO,
                                EDIT_EMP = LoginUser.EMP_NO,
                                FAIL_LINE = Line,
                                EDIT_TIME = DateTime.Now,
                                FAIL_TIME = DateTime.Now
                            };
                            RRM.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_REPAIR_MAIN");

                            R_REPAIR_FAILCODE RRF = new R_REPAIR_FAILCODE()
                            {
                                FAIL_CODE = "FTAUTO",
                                REPAIR_FLAG = "0",
                                FAIL_TIME = RRM.FAIL_TIME,
                                MAIN_ID = RRM.ID,
                                SN = RRM.SN,
                                CREATE_TIME = RRM.CREATE_TIME,
                                EDIT_EMP = RRM.EDIT_EMP,
                                EDIT_TIME = RRM.EDIT_TIME,
                                FAIL_EMP = RRM.FAIL_EMP
                            };
                            RRF.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_REPAIR_FAILCODE");
                            SFCDB.ORM.Updateable<R_SN>(sn.baseSN).Where(t => t.ID == sn.baseSN.ID).ExecuteCommand();
                            SFCDB.ORM.Insertable<R_REPAIR_MAIN>(RRM).ExecuteCommand();
                            SFCDB.ORM.Insertable<R_REPAIR_FAILCODE>(RRF).ExecuteCommand();


                        }
                    }
                }

                SFCDB.CommitTrain();


            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void PrepareTest(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            string strsn = Data["strsn"].ToString();
            string workorder = Data["workorder"].ToString();
            string stratename = Data["stratename"].ToString();
            string strtbsname = Data["strtbsname"].ToString();
            string strplatformversion = Data["strplatformversion"].ToString();
            string strworkproc = Data["strworkproc"].ToString();
            //string strworkstation = Data["strworkstation"].ToString();
            string stresn = Data["stresn"].ToString();
            string stratpproduct = Data["stratpproduct"].ToString();
            string strusername = Data["strusername"].ToString();
            //string stripaddress = Data["stripaddress"].ToString();
            string Line = Data["Line"].ToString();
            try
            {
                string var_sysserialno = "";
                string var_workorderno, var_nextevent,var_atename;

                SN sn = new SN();
                //获取TE Station
                //var TE_STATION = strworkproc + strworkstation;
                //var SFC_STATION = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().
                //    Where(t => t.TEGROUP == "FT" && t.TE_STATION == TE_STATION).Select(t => t.MES_STATION).First();
                //if (SFC_STATION == null)
                //{
                //    throw new Exception($@"Can't find mapping '{TE_STATION}' in C_TEMES_STATION_MAPPING");
                //}
                //if (TE_STATION == "ORT")
                //{
                //    sn.Load(strsn, SFCDB, DBTYPE);
                //}
                if(true)
                {
                    var three_flag = "0";
                    if (SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == strsn && t.VALID_FLAG == 1).Any())
                    {
                        var sns1 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == strsn && t.VALID_FLAG == 1).Select(t => t.SN).ToList();
                        var sns2 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => sns1.Contains(t.VALUE) && t.VALID_FLAG == 1).Select(t => t.SN).ToList();
                        var sku1 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => sns2.Contains(t.VALUE) && t.VALID_FLAG == 1).Select(t => t.PARTNO).First();


                        if (sku1 != null)
                        {
                            if (SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == sku1 &&
                             t.CATEGORY.ToUpper() == "c_ft_special_skuno_config".ToUpper()).Any())
                            {
                                var_sysserialno = sns2[0];
                                three_flag = "1";
                                sn.Load(var_sysserialno, SFCDB, DBTYPE);
                            }

                        }
                        else
                        {
                            throw new Exception("無效的SN號﹐請確認");
                        }

                    }
                    if (three_flag == "0")
                    {
                        //检查是否顶级SN
                        if (SFCDB.ORM.Queryable<R_RELATION_DATA>().Where(t => t.SON == strsn).Any())
                        {
                            throw new Exception($@"條碼必須是最高父項");
                        }

                        
                        sn.Load(strsn, SFCDB, DBTYPE);
                        var_sysserialno = sn.baseSN.SN;
                    }

                    var_workorderno = sn.WorkorderNo;
                    var_nextevent = sn.NextStation;
                }
                var_atename = stratename;
                //10分钟内不良必须换机台重测
                if (SFCDB.ORM.Queryable<R_TESTRECORD>().
                    Where(t => t.SN == var_sysserialno && t.DEVICE == stratename && t.TEST_TIME > DateTime.Now.AddMinutes(-10)).Any())
                {
                    throw new Exception($@"10分钟内不良必须换机台重测");
                }

                //如RMA產品未有維修動作，不可測試過站
                
                //bool isrma = false;
                SKU sku = new SKU();
                sku.Init(sn.SkuNo, SFCDB, DBTYPE);
                if (sku.IsRMASkuno(SFCDB,sn.SkuNo))
                {
                    if (!SFCDB.ORM.Queryable<R_RMA_DETAIL>().Where(t => t.SN == var_sysserialno).Any() ||
                        !SFCDB.ORM.Queryable<R_RMA_DETAIL, R_RELATIONDATA_EXTERNAL>((D, E) => new object[] { JoinType.Left, D.SN == E.SN })
                        .Where((D, E) => E.PARENT == var_sysserialno || E.PARENT == var_sysserialno).Any())
                    {
                        throw new Exception($@"此SN沒有RMA維修記錄,請聯繫RMA人員");
                    }
                }

                //查询机台是否被锁
                var Device = SFCDB.ORM.Queryable<R_SFCATE_CONFIG>().Where(t => t.ATENAME == var_atename && t.STATION != "ORT").First();
                if (Device == null)
                {
                    throw new Exception($@"'{var_atename}'未配置R_SFCATE_CONFIG");
                }
                if (Device.STATUS == "停用")
                {
                    throw new Exception($@"'{var_atename}'因为'{Device.DESCRIPTION}'停用");
                }
                //SN锁定检查
                sn.LockCheck(SFCDB);
                if (sn.baseSN.COMPLETED_FLAG != "0")
                {
                    throw new Exception($@"此SN已完工或入MRB");
                }

                if (sn.baseSN.SHIPPED_FLAG != "0")
                {
                    throw new Exception($@"此SN已出货");
                }
                //检查治具
                //暂时忽略



            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

    }
}
