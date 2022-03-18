using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class CBatchProcessing : MesAPIBase
    {
        protected APIInfo FBatchMRB = new APIInfo
        {
            FunctionName = "BatchMRB",
            Description = "Batch More SN Into MRB",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FBatchCancelSiloadingKP = new APIInfo
        {
            FunctionName = "BatchCancelSiloadingKP",
            Description = "Batch Cancel Siloading Keypart",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FPeUploadLinkData = new APIInfo
        {
            FunctionName = "PeUploadLinkData",
            Description = "PE Upload Link Data",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ExcelData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        
        public CBatchProcessing()
        {
            this.Apis.Add(FBatchMRB.FunctionName, FBatchMRB);
            this.Apis.Add(FBatchCancelSiloadingKP.FunctionName, FBatchCancelSiloadingKP);
            this.Apis.Add(FPeUploadLinkData.FunctionName, FPeUploadLinkData);
        }


        /// <summary>
        /// 只有處理入RMB邏輯，沒有處理退料邏輯
        /// Scan sn to MRB
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void BatchMRB(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {           
            OleExec SFCDB = null;            
            try
            {
                //定義上傳Excel的列名 
                List<string> inputTitle = new List<string> { "SN", "TO_STORAGE" };
                string errTitle = "";               
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception($@"Upload File Is Empty!");
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool bColumnExists = false;
                foreach (string t in inputTitle)
                {
                    bColumnExists = firstData.Properties().Any(p => p.Name == t);
                    if (!bColumnExists)
                    {
                        errTitle = t;
                        break;
                    }
                }
                if (!bColumnExists)
                {
                    throw new Exception($@"Upload File Content Error,Must Contains {errTitle} Column!");
                }              
                int result = 0;
                string sn = "";
                string to_storage = "";
                string message = "";
                
                string fail_msg = "";
                int fail_count = 0;
                int pass_count = 0;

                R_SN snObj = null;
                SFCDB = this.DBPools["SFCDB"].Borrow();
                T_R_SN t_r_sn = new T_R_SN(SFCDB, this.DBTYPE);
                T_R_WO_BASE t_r_wo_base= new T_R_WO_BASE(SFCDB, this.DBTYPE);
                R_MRB mrbObj = null;
                T_R_MRB t_r_mrb = new T_R_MRB(SFCDB,DBTYPE);
                DateTime sysdate ;
                R_MRB_GT oldGTObj = null;
                R_MRB_GT newGTObj = null;
                T_R_MRB_GT t_r_mrb_gt = new T_R_MRB_GT(SFCDB, DBTYPE);
                T_C_SAP_STATION_MAP t_c_sap_station_map = new T_C_SAP_STATION_MAP(SFCDB, DBTYPE);
                bool isSame = false;
                string Confirmed_Flag = "";
                string ZCPP_FLAG = "0", From_Storage = "";
                string sapStationCode = "";
               
                SFCDB.ThrowSqlExeception = true;
                for (int i = 0; i < array.Count; i++)
                {
                    sn = "";
                    to_storage = "";
                    Confirmed_Flag = "";
                    ZCPP_FLAG = "0";
                    From_Storage = "";
                    sapStationCode = "";
                    isSame = false;
                    SFCDB.BeginTrain();
                    try
                    {
                        sn = array[i]["SN"].ToString().ToUpper().Trim();
                        to_storage = array[i]["TO_STORAGE"].ToString().ToUpper().Trim();
                        snObj = t_r_sn.LoadSN(sn, SFCDB);
                        if (snObj == null)
                        {
                            throw new Exception($@"{sn} Not Exist!");
                        }
                        if (snObj.CURRENT_STATION == "MRB" || snObj.NEXT_STATION == "REWORK")
                        {
                            throw new Exception($@"{sn} Already In MRB/REWORK!");
                        }
                        if (snObj.SHIPPED_FLAG == "1")
                        {
                            throw new Exception($@"{sn} Already Shipped!");
                        }
                        if (snObj.SCRAPED_FLAG == "1")
                        {
                            throw new Exception($@"{sn} Already Scraped!");
                        }
                        if (snObj.ID == snObj.SN)
                        {
                            throw new Exception($@"{sn} Is Virtual SN!");
                        }
                        //更新SN當前站，下一站，如果SN的compled=1了也就是Confirmed_Flag==1,則修改當前站和下一站即可
                        //如果如果SN的compled!=1,則還需要修改sn的compled=1和SN對應工單的finishedQTY要加一
                        if (snObj.COMPLETED_FLAG != "1")
                        {
                            result = t_r_sn.SN_Mrb_Pass_action(snObj.ID, LoginUser.EMP_NO, SFCDB);
                            if (result <= 0)
                            {
                                throw new Exception($@"{sn} Update R_SN Fail!");
                            }
                            
                            result = t_r_wo_base.UpdateFINISHEDQTYAddOne(snObj.WORKORDERNO, SFCDB);
                            if (result <= 0)
                            {
                                throw new Exception($@"{sn} Update R_WO_BASE Fail!");
                            }
                            t_r_wo_base.UpdateWoCloseFlag(snObj.WORKORDERNO, SFCDB);//是否需要關閉工單
                        }
                        else
                        {
                            result = t_r_sn.SN_Mrb_Pass_actionNotUpdateCompleted(snObj.ID, LoginUser.EMP_NO, SFCDB);
                            if (result <= 0)
                            {
                                throw new Exception($@"{sn} Update R_SN Fail!");
                            }
                        }
                        
                        //SN如果已經完工，Confirmed_Flag=1，否則Confirmed_Flag=0
                        if (snObj.COMPLETED_FLAG != null && snObj.COMPLETED_FLAG == "1")
                        {
                            Confirmed_Flag = "1";
                        }
                        else
                        {
                            Confirmed_Flag = "0";
                        }                       
                        sysdate = SFCDB.ORM.GetDate();
                        mrbObj = new R_MRB();
                        //添加一筆MRB記錄
                        //給mrbObj賦值
                        mrbObj.ID = t_r_mrb.GetNewID(BU, SFCDB, DBTYPE);
                        mrbObj.SN = snObj.SN;
                        mrbObj.WORKORDERNO = snObj.WORKORDERNO;
                        mrbObj.NEXT_STATION = snObj.NEXT_STATION;
                        mrbObj.SKUNO = snObj.SKUNO;
                        mrbObj.FROM_STORAGE = "";
                        mrbObj.TO_STORAGE = to_storage;
                        mrbObj.REWORK_WO = "";//空
                        mrbObj.CREATE_EMP = LoginUser.EMP_NO;
                        mrbObj.CREATE_TIME = sysdate;
                        mrbObj.MRB_FLAG = "1";
                        mrbObj.SAP_FLAG = "0";
                        mrbObj.EDIT_EMP = LoginUser.EMP_NO;
                        mrbObj.EDIT_TIME = mrbObj.CREATE_TIME;
                        result = t_r_mrb.Add(mrbObj, SFCDB);
                        if (result <= 0)
                        {
                            throw new Exception( $@"{t_r_mrb}Add R_MRB Fail!");
                        }

                        //存在R_MRB_GT WO =? And SAP_FLAG = 0,則檢查FROM_STORAGE，TO_STORAGE，CONFIRMED_FLAG是否一樣，一樣則累加1
                        oldGTObj = SFCDB.ORM.Queryable<R_MRB_GT>()
                                .Where(r => r.WORKORDERNO == snObj.WORKORDERNO && r.ZCPP_FLAG == ZCPP_FLAG && r.CONFIRMED_FLAG == Confirmed_Flag && r.SAP_FLAG == "0")
                                .ToList().FirstOrDefault();
                        
                        if (oldGTObj != null)
                        {
                            oldGTObj.FROM_STORAGE = (oldGTObj.FROM_STORAGE == null || oldGTObj.FROM_STORAGE.Trim().Length <= 0) ? "" : oldGTObj.FROM_STORAGE;
                            oldGTObj.TO_STORAGE = (oldGTObj.TO_STORAGE == null || oldGTObj.TO_STORAGE.Trim().Length <= 0) ? "" : oldGTObj.TO_STORAGE;
                            oldGTObj.CONFIRMED_FLAG = (oldGTObj.CONFIRMED_FLAG == null || oldGTObj.CONFIRMED_FLAG.Trim().Length <= 0) ? "" : oldGTObj.CONFIRMED_FLAG;
                            if (oldGTObj.FROM_STORAGE == mrbObj.FROM_STORAGE && oldGTObj.TO_STORAGE == mrbObj.TO_STORAGE && oldGTObj.CONFIRMED_FLAG == Confirmed_Flag)
                            {
                                isSame = true;
                                result = t_r_mrb_gt.updateTotalQTYAddOne(snObj.WORKORDERNO, LoginUser.EMP_NO, Confirmed_Flag, SFCDB);
                                if (result <= 0)
                                {
                                    throw new Exception($@"{sn} UPDATE R_MRB_GT Fail!");
                                }
                            }
                        }
                        if (!isSame)
                        {                            
                            newGTObj = new R_MRB_GT();
                            //賦值
                            newGTObj.ID = t_r_mrb_gt.GetNewID(BU, SFCDB);
                            newGTObj.WORKORDERNO = snObj.WORKORDERNO;
                            sapStationCode = t_c_sap_station_map.GetMAXSAPStationCodeBySku(snObj.SKUNO, SFCDB);
                            if (sapStationCode == "")
                            {
                                throw new Exception("Get SAP Code Error!");
                            }
                            newGTObj.SAP_STATION_CODE = sapStationCode;
                            newGTObj.FROM_STORAGE = From_Storage;
                            newGTObj.TO_STORAGE = to_storage;
                            newGTObj.TOTAL_QTY = 1;
                            newGTObj.CONFIRMED_FLAG = Confirmed_Flag;
                            newGTObj.ZCPP_FLAG = ZCPP_FLAG;//暫時預留
                            newGTObj.SAP_FLAG = "0";//0待拋,1已拋,2待重拋
                            newGTObj.SKUNO = snObj.SKUNO;
                            newGTObj.SAP_MESSAGE = "";
                            newGTObj.EDIT_EMP = LoginUser.EMP_NO;
                            newGTObj.EDIT_TIME = sysdate;
                            result = t_r_mrb_gt.Add(newGTObj, SFCDB);
                            if (result <= 0)
                            {
                                throw new Exception($@"{sn} Add R_MRB_GT Fail!");
                            }
                        }
                        //添加過站記錄                       
                        result = Convert.ToInt32(t_r_sn.RecordPassStationDetail(snObj, "BatchMRB_OutLine", "MRBCHECKIN", "MRBCHECKIN", BU, SFCDB));
                        if (result <= 0)
                        {
                            throw new Exception ($@"{sn} Add R_SN_STATION_DETAIL Fail!");
                        }
                        pass_count++;
                        SFCDB.CommitTrain();
                    }
                    catch (Exception ex)
                    {
                        fail_count++;
                        fail_msg += $@"{sn}:{ex.Message};";
                        SFCDB.RollbackTrain();
                    }
                }
                
                if (fail_count !=0)
                {
                    message = $@"{pass_count} SN Successful,The Following SN Uplaod Fail:/n/r {fail_msg}!";
                }
                else
                {
                    message = "All SN Upload Successful!";
                }                             
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = message;
                StationReturn.MessageCode = null;
                StationReturn.Data = null;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        /// <summary>
        /// Batch Cancel Siloading KP
        /// 取消Siloading的keypart，主要用於處理DCN 2階(PCBA階)SN load進3階(出貨階)工單生成新的3階SN的link關係
        /// 目前僅考慮DCN 2階(PCBA階)SN load進3階(出貨階)工單生成新的3階SN的link關係的處理，沒有1階load2階的處理邏輯
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>       
        public void BatchCancelSiloadingKP(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Message = "";
            StationReturn.MessageCode = null;
            StationReturn.Data = null;
            return;
            //OleExec SFCDB = null;
            //try
            //{
            //    //定義上傳Excel的列名 
            //    List<string> inputTitle = new List<string> { "SN" };
            //    string errTitle = "";
            //    string data = Data["ExcelData"].ToString();
            //    Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            //    if (array.Count == 0)
            //    {
            //        throw new Exception($@"Upload File Is Empty!");
            //    }
            //    Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
            //    bool bColumnExists = false;
            //    foreach (string t in inputTitle)
            //    {
            //        bColumnExists = firstData.Properties().Any(p => p.Name == t);
            //        if (!bColumnExists)
            //        {
            //            errTitle = t;
            //            break;
            //        }
            //    }
            //    if (!bColumnExists)
            //    {
            //        throw new Exception($@"Upload File Content Error,Must Contains {errTitle} Column!");
            //    }
            //    int result = 0;
            //    string sn = "";                
            //    string message = "";
            //    string fail_msg = "";
            //    int fail_count = 0;
            //    int pass_count = 0;
                

            //    R_SN snObj = null;
            //    SFCDB = this.DBPools["SFCDB"].Borrow();
            //    T_R_SN t_r_sn = new T_R_SN(SFCDB, this.DBTYPE);               
            //    DateTime sysdate= SFCDB.ORM.GetDate();
            //    T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(SFCDB, this.DBTYPE);
            //    List<R_SN_KP> listKeypart;
            //    R_SN_KP newKPObj = null;
            //    //T_R_SN_LINK t_r_sn_link = new T_R_SN_LINK(SFCDB, this.DBTYPE);
            //    //List<R_SN_LINK> listSNLink = new List<R_SN_LINK>();
            //    //R_SN_LINK newLinkObj = null;
            //    for (int i = 0; i < array.Count; i++)
            //    {
            //        sn = "";
            //        newKPObj = null;                    
            //        listKeypart = new List<R_SN_KP>();
            //        SFCDB.BeginTrain();
            //        try
            //        {
            //            sn = array[i]["SN"].ToString().ToUpper().Trim();
            //            snObj = t_r_sn.LoadSN(sn, SFCDB);
            //            if (snObj == null)
            //            {
            //                throw new Exception($@"{sn} Not Exist!");
            //            }
            //            if (snObj.CURRENT_STATION == "SILOADING")
            //            {
            //                throw new Exception($@"{sn}'s Current Station Is Not SILOADING!");
            //            }
            //            if (snObj.SHIPPED_FLAG == "1")
            //            {
            //                throw new Exception($@"{sn} Already Shipped!");
            //            }
            //            if (snObj.SCRAPED_FLAG == "1")
            //            {
            //                throw new Exception($@"{sn} Already Scraped!");
            //            }
            //            if (snObj.ID == snObj.SN)
            //            {
            //                throw new Exception($@"{sn} Is Virtual SN!");
            //            }
            //            //只處理SILOADING工站的Keypart
            //            listKeypart = SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.SN == snObj.SN && r.VALID_FLAG == 1 && r.STATION == "SILOADING" ).ToList();
            //            if (listKeypart.Count == 0)
            //            { 
            //                continue;
            //            }

            //            // udpate WWN_DATASHARING
            //            result = SFCDB.ORM.Updateable<MESDataObject.Module.DCN.WWN_DATASHARING>()
            //                .SetColumns(r => new MESDataObject.Module.DCN.WWN_DATASHARING
            //                {
            //                    CSSN = "N/A",
            //                    CSKU = "N/A",
            //                    LASTEDITBY = LoginUser.EMP_NO,
            //                    LASTEDITDT = sysdate
            //                }).Where(r => r.CSKU == snObj.SKUNO && r.CSSN == snObj.SN && SqlSugar.SqlFunc.Subqueryable<R_SN_KP>()
            //                .Where(k => k.VALUE == r.VSSN && k.PARTNO == r.VSKU && k.VALID_FLAG == 1 && k.SN == snObj.SN).Any())
            //                .ExecuteCommand();

            //            result = SFCDB.ORM.Updateable<MESDataObject.Module.DCN.WWN_DATASHARING>()
            //                .SetColumns(r => new MESDataObject.Module.DCN.WWN_DATASHARING
            //                {
            //                    VSSN = "N/A",
            //                    VSKU = "N/A",
            //                    LASTEDITBY = LoginUser.EMP_NO,
            //                    LASTEDITDT = sysdate
            //                }).Where(r => r.VSKU == snObj.SKUNO && r.VSSN == snObj.SN && SqlSugar.SqlFunc.Subqueryable<R_SN_KP>()
            //                .Where(k => k.VALUE == r.WSN && k.PARTNO == r.SKU && k.VALID_FLAG == 1 && k.SN == snObj.SN).Any())
            //                .ExecuteCommand();
            //            //update R_SN_LINK
            //            //listSNLink = SFCDB.ORM.Queryable<R_SN_LINK>()
            //            //    .Where(r => r.MODEL == snObj.SKUNO && r.SN == snObj.SN && r.VALIDFLAG == "1" && r.LINKTYPE == "SILOADING"
            //            //    && SqlSugar.SqlFunc.Subqueryable<R_SN_KP>().Where(k => k.VALUE == r.CSN && k.SN == r.SN && k.STATION == r.LINKTYPE).Any())
            //            //    .ToList();
            //            //update R_SN_LINK
            //            result = SFCDB.ORM.Updateable<R_SN_LINK>().SetColumns(r => new R_SN_LINK { VALIDFLAG = "0" })
            //                .Where(r => r.MODEL == snObj.SKUNO && r.SN == snObj.SN && r.VALIDFLAG == "1" && r.LINKTYPE == "SILOADING"
            //                && SqlSugar.SqlFunc.Subqueryable<R_SN_KP>().Where(k => k.VALUE == r.CSN && k.SN == r.SN && k.STATION == r.LINKTYPE).Any())
            //                .ExecuteCommand();

            //            result = SFCDB.ORM.Updateable<R_SN_LINK>().SetColumns(r => new R_SN_LINK { VALIDFLAG = "0" })
            //                .Where(r => r.MODEL == snObj.SKUNO && r.SN == snObj.SN && r.VALIDFLAG == "1" && r.LINKTYPE == "SILOADING"
            //                && SqlSugar.SqlFunc.Subqueryable<R_SN_KP>().Where(k => k.VALUE == r.CSN && k.SN == r.SN && k.STATION == r.LINKTYPE).Any())
            //                .ExecuteCommand();
            //            //foreach (var l in listSNLink)
            //            //{
            //            //    newLinkObj = new R_SN_LINK();
            //            //    newLinkObj = l;
            //            //    newLinkObj.ID = t_r_sn_link.GetNewID(BU, SFCDB);
            //            //    newLinkObj.VALIDFLAG = "0";
            //            //}

            //            //update sn shipped flag
            //            result = SFCDB.ORM.Updateable<R_SN>().SetColumns(r => new R_SN { SHIPPED_FLAG = "0", SHIPDATE = null })
            //                    .Where(r => r.VALID_FLAG == "1" && r.SHIPPED_FLAG == "1"
            //                    && SqlSugar.SqlFunc.Subqueryable<R_SN_KP>().Where(k => k.VALUE == r.SN && k.VALID_FLAG == 1 && k.SN == snObj.SN).Any())
            //                    .ExecuteCommand();

            //            //update kp
            //            result = SFCDB.ORM.Updateable<R_SN_KP>().SetColumns(r => new R_SN_KP { VALID_FLAG = 0 })
            //                .Where(r => r.SN == snObj.SN && r.VALID_FLAG == 1 && r.STATION == "SILOADING" )
            //                .ExecuteCommand();

            //            foreach (var k in listKeypart)
            //            {
            //                newKPObj = new R_SN_KP();
            //                newKPObj = k;
            //                newKPObj.ID = t_r_sn_kp.GetNewID(BU, SFCDB);
            //                newKPObj.VALUE = "";
            //                newKPObj.EDIT_TIME = sysdate;
            //                newKPObj.EDIT_EMP = LoginUser.EMP_NO;
            //                result = SFCDB.ORM.Insertable<R_SN_KP>(newKPObj).ExecuteCommand();                                                       
            //            }

            //            //添加過站記錄                       
            //            result = Convert.ToInt32(t_r_sn.RecordPassStationDetail(snObj, "CancelKP_OutLine", "CancelSiloadingKP", "CancelSiloadingKP", BU, SFCDB));
            //            if (result <= 0)
            //            {
            //                throw new Exception($@"{sn} Add R_SN_STATION_DETAIL Fail!");
            //            }
            //            pass_count++;
            //            SFCDB.CommitTrain();
            //        }
            //        catch (Exception ex)
            //        {
            //            fail_count++;
            //            fail_msg += $@"{sn}:{ex.Message};";
            //            SFCDB.RollbackTrain();
            //        }
            //    }

            //    if (fail_count != 0)
            //    {
            //        message = $@"{pass_count} SN Successful,The Following SN Cancel Fail:/n/r {fail_msg}!";
            //    }
            //    else
            //    {
            //        message = "All SN Cancel Successful!";
            //    }
            //    StationReturn.Status = StationReturnStatusValue.Pass;
            //    StationReturn.Message = message;
            //    StationReturn.MessageCode = null;
            //    StationReturn.Data = null;
            //}
            //catch (Exception ee)
            //{
            //    StationReturn.Status = StationReturnStatusValue.Fail;
            //    StationReturn.MessageCode = "MES00000037";
            //    StationReturn.MessagePara.Add(ee.Message);
            //}
            //finally
            //{
            //    if (SFCDB != null)
            //    {
            //        this.DBPools["SFCDB"].Return(SFCDB);
            //    }
            //}
        }

        public void PeUploadLinkData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                //定義上傳Excel的列名 
                List<string> inputTitle = new List<string> { "SN", "LINK_VALUE", "LINK_TYPE" };
                string errTitle = "";
                string data = Data["ExcelData"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception($@"Upload File Is Empty!");
                }
                Newtonsoft.Json.Linq.JObject firstData = Newtonsoft.Json.Linq.JObject.Parse(array[0].ToString());
                bool bColumnExists = false;
                foreach (string t in inputTitle)
                {
                    bColumnExists = firstData.Properties().Any(p => p.Name == t);
                    if (!bColumnExists)
                    {
                        errTitle = t;
                        break;
                    }
                }
                if (!bColumnExists)
                {
                    throw new Exception($@"Upload File Content Error,Must Contains {errTitle} Column!");
                }                
                string sn = "";
                string linkValue = "";
                string linkType = "";
                string message = "";
                string failMsg = "";

                int failCount = 0;
                int passCount = 0;
                int result = 0;

                SFCDB = this.DBPools["SFCDB"].Borrow();

                T_R_SN_LINK t_r_sn_link = new T_R_SN_LINK(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                DateTime sysdate = SFCDB.ORM.GetDate();
                SFCDB.ThrowSqlExeception = true;
                for (int i = 0; i < array.Count; i++)
                {
                    sn = "";
                    linkValue = "";
                    linkType = "SN_MAC";
                    SFCDB.BeginTrain();
                    try
                    {
                        sn = array[i]["SN"].ToString().ToUpper().Trim();
                        linkValue = array[i]["LINK_VALUE"].ToString().ToUpper().Trim();
                        linkType = array[i]["LINK_TYPE"].ToString().ToUpper().Trim();
                        if (string.IsNullOrWhiteSpace(sn)||string.IsNullOrWhiteSpace(linkValue))
                        {
                            throw new Exception($"SN is null or LINK_VALUE is null;{sn},{linkValue}");
                        }
                        bool bExist = SFCDB.ORM.Queryable<R_SN_LINK>().Any(r => r.SN == sn && r.CSN == linkValue && r.LINKTYPE == linkType && r.MODEL == "N" && r.VALIDFLAG == "1");
                        if(bExist)
                        {
                            throw new Exception($"SN[{sn},{linkValue},{linkType}] already exist and no check");
                        }

                        R_SN_LINK linkObj = new R_SN_LINK();
                        linkObj.ID = t_r_sn_link.GetNewID(BU, SFCDB);
                        linkObj.LINKTYPE = linkType;
                        linkObj.SN = sn;
                        linkObj.CSN = linkValue;
                        linkObj.MODEL = "N";
                        linkObj.VALIDFLAG = "1";
                        linkObj.CREATEBY = LoginUser.EMP_NO;
                        linkObj.CREATETIME = sysdate;

                        result = SFCDB.ORM.Insertable<R_SN_LINK>(linkObj).ExecuteCommand();
                        if(result==0)
                        {
                            throw new Exception($"{sn},{linkValue},{linkType},insert into r_sn_link fail.");
                        }
                        passCount++;
                        SFCDB.CommitTrain();
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        failMsg += $@"{sn}:{ex.Message};";
                        SFCDB.RollbackTrain();
                    }
                }

                if (failCount != 0)
                {
                    message = $@"{passCount} SN Successful,The Following SN Uplaod Fail:/n/r {failMsg}!";
                }
                else
                {
                    message = "All SN Upload Successful!";
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = message;
                StationReturn.MessageCode = null;
                StationReturn.Data = null;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }
    }
}
