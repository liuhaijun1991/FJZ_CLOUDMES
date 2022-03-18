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
    public class EmsIntrf : MesAPIBase
    {
        protected APIInfo FPrepareTest = new APIInfo()
        {
            FunctionName = "PrepareTest",
            Description = "PrepareTest",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Reserved", InputType = "string", DefaultValue = "" },
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
                new APIInputInfo() {InputName = "Reserved", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Line", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "bResult", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        public EmsIntrf()
        {
            Apis.Add(FPrepareTest.FunctionName, FPrepareTest);
            Apis.Add(FReportTestResult.FunctionName, FReportTestResult);
        }
        public void TEMP(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            string HostName = Data["HOSTNAME"].ToString();
            try
            { }
            catch
            { }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void ReportTestResult(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            string Reserved = Data["Reserved"].ToString();
            string Line = Data["Line"].ToString();
            string bResult = Data["bResult"].ToString();
            try
            {
                //拆分接受的字符串
                var paras = Reserved.Split(new char[] { '$' });
                var var_sn = paras[0];
                var var_atename = paras[1];
                var var_tbsname = paras[4];
                var var_processcode = paras[5];
                var var_site = paras[6];
                var VAR_TRANTYPE = false;
                //获取TE Station
                var TE_STATION = var_processcode + var_site;
                var SFC_STATION = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().
                    Where(t => t.TEGROUP == "FT" && t.TE_STATION == TE_STATION).Select(t => t.MES_STATION).First();

                if (SFC_STATION == null)
                {
                    throw new Exception($@"Can't find mapping '{TE_STATION}' in C_TEMES_STATION_MAPPING");
                }
                //判断结果
                if (bResult.ToUpper().Trim() == "TRUE")
                {
                    VAR_TRANTYPE = true;
                }
                SN sn = new SN();
                sn.Load(var_sn, SFCDB, DBTYPE);
                if (sn.NextStation != SFC_STATION)
                {
                    throw new Exception($@"此板當前工位'{sn.NextStation}'與測試工序'{TE_STATION}'不一致");
                }
                //r_ft_tbs
                var var_SKUNO = sn.SkuNo;
                if (!SFCDB.ORM.Queryable<R_FT_TBS>().
                    Where(t => t.MODEL_NAME == var_SKUNO && (t.TBS_NAME == var_tbsname || t.TBS_NAME == var_atename) && t.STATION_NAME == TE_STATION).Any())
                {
                    throw new Exception($@"Skuno:'{var_SKUNO}',Station:'{var_atename}',TBS:'{TE_STATION}' 未配置");
                }
                SKU sku = new SKU();
                sku.Init(var_SKUNO, SFCDB, DBTYPE);

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
                TR.TESTINFO = var_tbsname;
                if (!VAR_TRANTYPE)
                {
                    TR.STATE = "FAIL";
                }
                SFCDB.ORM.Insertable<R_TEST_RECORD>(TR).ExecuteCommand();

                SFCDB.BeginTrain();
                var test_records = SFCDB.ORM.Queryable<R_TEST_RECORD>().
                        Where(t => t.DEVICE == var_atename && t.ENDTIME > DateTime.Now.AddDays(-2)).OrderBy(t => t.ENDTIME).Take(2).ToList();
                if (!VAR_TRANTYPE)
                {
                    //int var_pass_count= 0;
                    //int  var_fail_count= 1;
                    //int var_repairheld= 1;
                    //int var_completed= 0;
                    //int var_packed = 0;
                    //同一個機台最後2次失敗，這一次又失敗，說明這個機台 測試連續失敗 3PCS, 
                    //鎖定該機台
                    
                    if (test_records.Count >= 2)
                    {
                        if (test_records[0].STATE == "FAIL" && test_records[1].STATE == "FAIL")
                        {
                            //连续3PCS不良
                            var Device = SFCDB.ORM.Queryable<R_SFCATE_CONFIG>().Where(t => t.ATENAME == var_atename && t.STATION != "ORT").First();
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
                test_records = SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == var_sn && t.MESSTATION == SFC_STATION).OrderBy(t => t.ENDTIME).Take(3).ToList();
                if (VAR_TRANTYPE)
                {
                   
                    
                    if (test_records.Count == 1)
                    {
                        //直接过站
                        TRSN.PassStation(var_sn, Line, SFC_STATION, var_atename, BU, "PASS", LoginUser.EMP_NO, SFCDB);
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
                                    TRSN.PassStation(var_sn, Line, SFC_STATION, var_atename, BU, "PASS", LoginUser.EMP_NO, SFCDB);
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
                            TRSN.PassStation(var_sn, Line, SFC_STATION, var_atename, BU, "PASS", LoginUser.EMP_NO, SFCDB);
                        }
                       
                    }
                }

                //UPH
                TRSN.RecordUPH(sn.WorkorderNo, 1, sn.baseSN.SN, VAR_TRANTYPE?"PASS":"FAIL", Line, SFC_STATION, LoginUser.EMP_NO, BU, SFCDB);
                TRSN.RecordYieldRate(sn.WorkorderNo, 1, sn.baseSN.SN, VAR_TRANTYPE ? "PASS" : "FAIL", Line, SFC_STATION, LoginUser.EMP_NO, BU, SFCDB);

                //自动锁板
                if (!VAR_TRANTYPE)
                {
                    TRSN.PassStation(var_sn, Line, SFC_STATION, var_atename, BU, "FAIL", LoginUser.EMP_NO, SFCDB);
                    //var SERIES = SFCDB.ORM.Queryable<C_SERIES>().Where(t => t.ID == sku.CSeriesId).First();
                    if (SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == var_SKUNO && t.CATEGORY == "CTRL_LOCK" && t.STATION_NAME == SFC_STATION).Any())
                    {
                        R_SN_LOCK LOCK = new R_SN_LOCK();
                        LOCK.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOCK");
                        LOCK.SN = var_sn;
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
                                SN = var_sn,
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
                SFCDB.CommitTrain();
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
            string Reserved = Data["Reserved"].ToString();
            string Line = Data["Line"].ToString();
            try
            {
                //拆分接受的字符串
                var paras = Reserved.Split(new char[] { '$' });
                var var_sn = paras[0];
                var var_atename = paras[1];
                var var_tbsname = paras[4];
                var var_processcode = paras[5];
                var var_site = paras[6];

                //获取TE Station
                var TE_STATION = var_processcode + var_site;
                var SFC_STATION = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().
                    Where(t => t.TEGROUP == "FT" && t.TE_STATION == TE_STATION).Select(t=>t.MES_STATION).First();

                if (SFC_STATION == null)
                {
                    throw new Exception($@"Can't find mapping '{TE_STATION}' in C_TEMES_STATION_MAPPING");
                }

                SN sn = new SN();
                sn.Load(var_sn, SFCDB,DBTYPE);
                //检查是否顶级SN
                if (SFCDB.ORM.Queryable<R_RELATION_DATA>().Where(t => t.SON == var_sn).Any())
                {
                    throw new Exception($@"條碼必須是最高父項");
                }
                //10分钟内不良必须换机台重测
                if (SFCDB.ORM.Queryable<R_TESTRECORD>().
                    Where(t => t.SN == var_sn && t.DEVICE == var_atename && t.TEST_TIME > DateTime.Now.AddMinutes(-10)).Any())
                {
                    throw new Exception($@"10分钟内不良必须换机台重测");
                }
                //SN锁定检查
                sn.LockCheck(SFCDB);
                //如RMA產品未有維修動作，不可測試過站

                SKU sku = new SKU();
                sku.Init(sn.SkuNo, SFCDB, DBTYPE);
                if (sku.IsRMASkuno(SFCDB, sn.SkuNo))
                {
                    if ( !SFCDB.ORM.Queryable<MESDataObject.Module.R_RMA_DETAIL>().Where(t => t.SN == var_sn).Any() ||
                        !SFCDB.ORM.Queryable<MESDataObject.Module.R_RMA_DETAIL, R_RELATIONDATA_EXTERNAL>((D, E) => new object[] { JoinType.Left,D.SN == E.SN})
                        .Where((D,E)=> E.PARENT == var_sn || E.PARENT ==var_sn).Any())
                    {
                        throw new Exception($@"此SN沒有RMA維修記錄,請聯繫RMA人員");
                    }
                }
                //檢查ATE機台是否被鎖
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

                if (sn.baseSN.COMPLETED_FLAG != "0")
                {
                    throw new Exception($@"此SN已完工或入MRB");
                }

                if (sn.baseSN.SHIPPED_FLAG != "0")
                {
                    throw new Exception($@"此SN已出货");
                }


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
