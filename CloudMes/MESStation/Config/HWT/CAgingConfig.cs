using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MESStation.Config.HWT
{
    public class CAgeingTypeConfig : MesAPIBase
    {
        protected APIInfo FUploadSettingExcel = new APIInfo()
        {
            FunctionName = "UploadSettingExcel",
            Description = "Upload Setting Excel",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DataList", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCheckUploadPermissions = new APIInfo()
        {
            FunctionName = "CheckUploadPermissions",
            Description = "Check Upload Permissions",
            Parameters = new List<APIInputInfo>(){},
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSettingSearch = new APIInfo()
        {
            FunctionName = "SettingSearch",
            Description = "Setting Search",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CABINTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAgingTaskLoadData = new APIInfo()
        {
            FunctionName = "AgingTaskLoadData",
            Description = "Aging Task Load Data",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "FLOOR", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CABINTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAgingTaskAction = new APIInfo()
        {
            FunctionName = "AgingTaskAction",
            Description = "Aging Task Action",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "TYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FLOOR", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CABINTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };       
        protected APIInfo FGetAgingObjBySN = new APIInfo()
        {
            FunctionName = "GetAgingObjBySN",
            Description = "Get Aging Object By SN",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FReplaceAgingSN = new APIInfo()
        {
            FunctionName = "ReplaceAgingSN",
            Description = "Replace Aging SN",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "OLDSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NEWSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REMARK", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PWD", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAgingScanFail = new APIInfo()
        {
            FunctionName = "AgingScanFail",
            Description = "Aging Scan Fail",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "FAILSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FAILCODE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PWD", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAgingModifyTime = new APIInfo()
        {
            FunctionName = "AgingModifyTime",
            Description = "Aging Modify Time",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() {InputName = "CABINTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TIME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PWD", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CAgeingTypeConfig()
        {
            this.Apis.Add(FUploadSettingExcel.FunctionName, FUploadSettingExcel);
            this.Apis.Add(FCheckUploadPermissions.FunctionName, FCheckUploadPermissions);
            this.Apis.Add(FSettingSearch.FunctionName, FSettingSearch);
            this.Apis.Add(FAgingTaskLoadData.FunctionName, FAgingTaskLoadData);
            this.Apis.Add(FAgingTaskAction.FunctionName, FAgingTaskAction);
            this.Apis.Add(FGetAgingObjBySN.FunctionName, FGetAgingObjBySN);
            this.Apis.Add(FReplaceAgingSN.FunctionName, FReplaceAgingSN);
            this.Apis.Add(FAgingScanFail.FunctionName, FAgingScanFail);
            this.Apis.Add(FAgingModifyTime.FunctionName, FAgingModifyTime);
        }

        /// <summary>
        /// 上傳配置Excel文檔
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UploadSettingExcel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                string data = Data["DataList"].ToString();               
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                //Newtonsoft.Json.Linq.JToken dataList = Data["DataList"];
                T_C_AGING_CONFIG_DETAIL tcacd = new T_C_AGING_CONFIG_DETAIL(SFCDB,DBTYPE);
                //C_AGING_CONFIG_DETAIL cacd = null;
                Row_C_AGING_CONFIG_DETAIL rowCACD = null;
                for (int i = 0; i < array.Count; i++)
                {
                    //cacd = new C_AGING_CONFIG_DETAIL();
                    //cacd.ID = tcacd.GetNewID(BU, SFCDB);
                    //cacd.ITEM_NAME = array[i]["ITEM_NAME"].ToString().Trim();
                    //cacd.ITEM_CODE = array[i]["ITEM_CODE"].ToString().Trim(); 
                    //cacd.AGINGTIME = Convert.ToDouble(array[i]["AGINGTIME"].ToString().Trim()); 
                    //cacd.AGINGTYPE = array[i]["AGINGTYPE"].ToString().Trim(); 
                    //cacd.CABINET_NO = array[i]["CABINET_NO"].ToString().Trim(); 
                    //cacd.SHELF_NO = array[i]["SHELF_NO"].ToString().Trim(); 
                    //cacd.SHELF_QTY = Convert.ToDouble(array[i]["SHELF_QTY"].ToString().Trim());
                    //cacd.TOOLS_FLAG = array[i]["TOOLS_FLAG"].ToString().Trim();
                    //cacd.TOOLSNO = array[i]["TOOLSNO"].ToString().Trim();
                    //cacd.ONESHELFQTY = Convert.ToDouble(array[i]["ONESHELFQTY"].ToString().Trim());
                    //cacd.DESCRIPTION1 = array[i]["DESCRIPTION1"].ToString().Trim();
                    //cacd.DESCRIPTION2 = array[i]["DESCRIPTION2"].ToString().Trim();
                    //cacd.WORK_FLAG = "";
                    //cacd.EDIT_EMP = LoginUser.EMP_NO;
                    //cacd.EDIT_TIME = tcacd.GetDBDateTime(SFCDB);
                    rowCACD = (Row_C_AGING_CONFIG_DETAIL)tcacd.NewRow();
                    rowCACD.ID = tcacd.GetNewID(BU, SFCDB);
                    rowCACD.ITEM_NAME = array[i]["ITEM_NAME"].ToString().Trim();
                    rowCACD.ITEM_CODE = array[i]["ITEM_CODE"].ToString().Trim();
                    rowCACD.AGINGTIME = Convert.ToDouble(array[i]["AGINGTIME"].ToString().Trim());
                    rowCACD.AGINGTYPE = array[i]["AGINGTYPE"].ToString().Trim();
                    rowCACD.CABINET_NO = array[i]["CABINET_NO"].ToString().Trim();
                    rowCACD.SHELF_NO = array[i]["SHELF_NO"].ToString().Trim();
                    rowCACD.SHELF_QTY = Convert.ToDouble(array[i]["SHELF_QTY"].ToString().Trim());
                    rowCACD.TOOLS_FLAG = array[i]["TOOLS_FLAG"].ToString().Trim();
                    rowCACD.TOOLSNO = array[i]["TOOLSNO"].ToString().Trim();
                    rowCACD.ONESHELFQTY = Convert.ToDouble(array[i]["ONESHELFQTY"].ToString().Trim());
                    rowCACD.DESCRIPTION1 = array[i]["DESCRIPTION1"].ToString().Trim();
                    rowCACD.DESCRIPTION2 = array[i]["DESCRIPTION2"].ToString().Trim();
                    rowCACD.WORK_FLAG = "";
                    rowCACD.EDIT_EMP = LoginUser.EMP_NO;
                    rowCACD.EDIT_TIME = tcacd.GetDBDateTime(SFCDB);
                    SFCDB.ExecSQL(rowCACD.GetInsertString(DBTYPE));
                    //tcacd.Insert(SFCDB, cacd);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Data = "";
                SFCDB.CommitTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 檢查是否有上傳配置Excel文檔的權限
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void CheckUploadPermissions(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();            
            try
            {
                //查詢權限待寫                
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = "";              
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {               
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 老化配置信息查詢
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void SettingSearch(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string skuno= Data["SKUNO"].ToString().Trim();
                string cabinetno = Data["CABINTNO"].ToString().Trim();
                List<C_AGING_CONFIG_DETAIL> list = new List<C_AGING_CONFIG_DETAIL>();
                T_C_AGING_CONFIG_DETAIL tcacd = new T_C_AGING_CONFIG_DETAIL(SFCDB, DBTYPE);
                list = tcacd.GetConfigList(SFCDB, skuno, cabinetno);
                if (list.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 老化任務管理-加載待開始/結束的老化數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AgingTaskLoadData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["TYPE"]==null || Data["TYPE"].ToString()=="")
                {
                    throw new Exception("Please input type");
                }
                string floor = Data["FLOOR"].ToString().Trim();
                string cabinetno = Data["CABINTNO"].ToString().Trim(); 
                T_R_AGING_CABINET_INFO TRAC = new T_R_AGING_CABINET_INFO(SFCDB, DBTYPE);
                DataTable dt = new DataTable();
                if (Data["TYPE"].ToString().Trim().ToUpper() == "END")
                {
                    dt = TRAC.GetWaitEndList(SFCDB, floor, cabinetno);
                }
                else if (Data["TYPE"].ToString().Trim().ToUpper() == "START")
                {
                    dt = TRAC.GetWaitStartList(SFCDB, floor, cabinetno);
                }                
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 老化任務管理-開始老化或結束老化
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AgingTaskAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.ThrowSqlExeception = true;
            SFCDB.BeginTrain();
            try
            {
                string type = "";
                string floor = "";
                string cabinetno = "";
                string lotno = "";
                if (Data["TYPE"] == null || Data["TYPE"].ToString() == "")
                {
                    throw new Exception("Please input type");
                }
                if (Data["FLOOR"] == null || Data["FLOOR"].ToString() == "")
                {
                    throw new Exception("Please input floor");
                }
                if (Data["CABINTNO"] == null || Data["CABINTNO"].ToString() == "")
                {
                    throw new Exception("Please input cabinetno");
                }
                if (Data["LOTNO"] == null || Data["LOTNO"].ToString() == "")
                {
                    throw new Exception("Please input lotno");
                }
                type = Data["TYPE"].ToString();
                floor = Data["FLOOR"].ToString().Trim();
                cabinetno = Data["CABINTNO"].ToString().Trim();
                lotno = Data["LOTNO"].ToString();

                if (Data["TYPE"].ToString().Trim().ToUpper() == "END")
                {
                    EndAging(SFCDB, floor, cabinetno, lotno);
                }
                else if (Data["TYPE"].ToString().Trim().ToUpper() == "START")
                {
                    StartAging(SFCDB, floor, cabinetno, lotno);                    
                }

                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = "";

                SFCDB.CommitTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 開始老化
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="floor"></param>
        /// <param name="cabinetno"></param>
        /// <param name="lot"></param>
        private void StartAging(OleExec sfcdb, string floor, string cabinetno, string lot)
        {           
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(sfcdb, DBTYPE);
            T_R_AGING_SHELF_INFO TRAS = new T_R_AGING_SHELF_INFO(sfcdb, DBTYPE);
            T_R_AGING_CABINET_INFO TRAC = new T_R_AGING_CABINET_INFO(sfcdb, DBTYPE);
            List<R_SN_AGING_INFO> list = TRSA.GetWaitStartList(sfcdb, IP, floor, cabinetno, lot);
            if (list.Count == 0)
            {
                throw new Exception("Get wait start list from r_sn_aging_info error!");
            }
            foreach (var l in list)
            {
                TRSA.StartAging(sfcdb, IP, l.FLOOR, l.CABINETNO, l.LOT_NO, l.SN, this.LoginUser.EMP_NO);
            }

            TRAS.UpdateWorkFlag(sfcdb, IP, cabinetno, lot, "3", "4");

            R_SN_AGING_INFO maxEndTimeSN = TRSA.GetMaxEndTimeObj(sfcdb, IP, floor, cabinetno, lot);

            if (maxEndTimeSN == null)
            {
                throw new Exception("Get max end time from r_sn_aging_info error!");
            }
            DateTime systemDateTime = this.GetDBDateTime();
            TRAC.StartAging(sfcdb, IP, floor, cabinetno, lot, this.LoginUser.EMP_NO, systemDateTime, maxEndTimeSN.ENDTIME);
        }
        /// <summary>
        /// 結束老化
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="floor"></param>
        /// <param name="cabinetno"></param>
        /// <param name="lot"></param>
        private void EndAging(OleExec sfcdb, string floor, string cabinetno, string lot)
        {
            T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(sfcdb, DBTYPE);
            T_R_AGING_SHELF_INFO TRAS = new T_R_AGING_SHELF_INFO(sfcdb, DBTYPE);
            T_R_AGING_CABINET_INFO TRAC = new T_R_AGING_CABINET_INFO(sfcdb, DBTYPE);    
            T_R_SN_LOCK TRSL = new T_R_SN_LOCK(sfcdb,DBTYPE);
            T_R_SN TRS = new T_R_SN(sfcdb, DBTYPE);

            List<R_SN_AGING_INFO> list = TRSA.GetWaitStartList(sfcdb, IP, floor, cabinetno, lot);
            R_SN_AGING_INFO minEndTimeSN = TRSA.GetMinEndTimeObj(sfcdb, IP, floor, cabinetno, lot);
            R_SN_LOCK lockSN = null;
            R_SN objSN = null;            
          
            DateTime systemDateTime = this.GetDBDateTime();

            if (list.Count == 0)
            {
                throw new Exception("Get wait end list from r_sn_aging_info error!");
            }
            if (minEndTimeSN == null)
            {
                throw new Exception("Get max end time from r_sn_aging_info error!");
            }
            if (minEndTimeSN.ENDTIME > systemDateTime)
            {
                //throw new Exception(lot + "該批次產品還未到老化完成時間");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141814", new string[] { lot }));
            }
            foreach (R_SN_AGING_INFO agingSN in list)
            {
                objSN = TRS.LoadData(agingSN.SN, sfcdb);
                if (objSN == null)
                {
                    agingSN.REMARK = MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { agingSN.SN });
                    agingSN.EVENTPASS = "0";
                }
                else
                {                   
                    if (objSN.NEXT_STATION != "AGING")
                    {
                        agingSN.REMARK = "NEXT_STATION IS " + objSN.NEXT_STATION + " NOT AGING";
                        agingSN.EVENTPASS = "0";
                    }
                    else
                    {
                        try
                        {
                            if (objSN.SN != "EMPTY" && objSN.SN != "ERROR")
                            {
                                lockSN = TRSL.GetDetailBySN(sfcdb, agingSN.SN, "AGING");
                                if (lockSN != null)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { agingSN.SN, lockSN.LOCK_EMP, lockSN.LOCK_REASON }));
                                }
                                if (objSN.COMPLETED_FLAG == "1")
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160344", new string[] { agingSN.SN }));
                                }
                                if (objSN.PACKED_FLAG == "1")
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005155848", new string[] { agingSN.SN }));
                                }
                                if (objSN.SHIPPED_FLAG == "1")
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { agingSN.SN }));
                                }
                                if (objSN.REPAIR_FAILED_FLAG == "1")
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { agingSN.SN }));
                                }
                                //1 SN過站
                                TRS.PassStation(objSN.SN, IP, "AGING", "AGING", BU, "PASS", LoginUser.EMP_NO, sfcdb);
                                //3.寫良率 R_YIELD_RATE_DETAIL
                                TRS.RecordYieldRate(objSN.WORKORDERNO, 1, objSN.SN, "PASS", IP, "AGING", LoginUser.EMP_NO, BU, sfcdb);
                                //4.寫UPH R_UPH_DETAIL
                                TRS.RecordUPH(objSN.WORKORDERNO, 1, objSN.SN, "PASS", IP, "AGING", LoginUser.EMP_NO, BU, sfcdb);                               
                            }
                            agingSN.REMARK = "";
                            agingSN.EVENTPASS = "1";
                        }
                        catch (Exception ex)
                        {
                            agingSN.REMARK = ex.Message.Length > 200 ? ex.Message.Substring(0, 198) : ex.Message;
                            agingSN.EVENTPASS = "0";
                        }

                    }
                }
                agingSN.WORK_FLAG = "5";
                agingSN.REALFINISHTIME = systemDateTime;
                agingSN.ENDEMPNO = this.LoginUser.EMP_NO;
                TRSA.UpdateByID(sfcdb, agingSN);
            }
            TRAS.UpdateWorkFlag(sfcdb, IP, cabinetno, lot, "4", "5");
            TRAC.EndAging(sfcdb, IP, floor, cabinetno, lot, LoginUser.EMP_NO, systemDateTime);
        }
        /// <summary>
        /// 老化替換條碼-加載舊SN老化信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetAgingObjBySN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                if (Data["SN"] == null || Data["SN"].ToString() == "")
                {
                    throw new Exception("Please input sn");
                }
                string sn = Data["SN"].ToString().Trim();
                T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(SFCDB, DBTYPE);
                R_SN_AGING_INFO objAaging = TRSA.GetSNAgingObj(SFCDB, "", "", "", "", "", "", sn, "");
                if (objAaging == null)
                {
                    //throw new Exception("該序列號沒有被掃描過,不需要替換！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143216"));
                }
                if (objAaging.IPADDRESS != IP)
                {
                    //throw new Exception("該序列號產品不是在此電腦掃描的,不能在此電腦替換！請去IP為" + objAaging.IPADDRESS + "的電腦掃描");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143839", new string[] { objAaging.IPADDRESS }));
                }
                if (objAaging.WORK_FLAG == "4")
                {
                    //throw new Exception("該序列號產品正在老化不能替換！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144146"));
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = objAaging;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }

        /// <summary>
        /// 老化替換條碼
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void ReplaceAgingSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;          
            try
            {
                if (Data["OLDSN"] == null || Data["OLDSN"].ToString() == "")
                {
                    throw new Exception("Please input old sn");
                }
                if (Data["NEWSN"] == null || Data["NEWSN"].ToString() == "")
                {
                    throw new Exception("Please input new sn");
                }
                if (Data["REMARK"] == null || Data["REMARK"].ToString() == "")
                {
                    throw new Exception("Please input remark");
                }
                if (Data["PWD"] == null || Data["PWD"].ToString() == "")
                {
                    throw new Exception("Please input password");
                }
                string oldsn = Data["OLDSN"].ToString().Trim();
                string newsn = Data["NEWSN"].ToString().Trim();
                string remark = Data["REMARK"].ToString().Trim();
                string pwd = Data["PWD"].ToString().Trim();
                T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(SFCDB, DBTYPE);
                T_R_SN TRS = new T_R_SN(SFCDB, DBTYPE);
                T_C_AGING_CONFIG_DETAIL TCACD = new T_C_AGING_CONFIG_DETAIL(SFCDB, DBTYPE);
                //T_c_user TCU = new T_c_user(SFCDB, DBTYPE);
                C_USER user = SFCDB.ORM.Queryable<C_USER>().Where(r => r.EMP_NO == LoginUser.EMP_NO && r.EMP_PASSWORD == pwd).ToList().FirstOrDefault();
                if(user == null)
                {
                    //throw new Exception("密碼錯誤！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000010"));
                }
                //檢查替換權限 ---待寫

                #region 舊條碼狀態檢查
                R_SN objOldSn = TRS.LoadData(oldsn, SFCDB);
                if (objOldSn == null)
                {
                    MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { oldsn });
                }
                if (objOldSn.NEXT_STATION != "AGING")
                {
                    //throw new Exception("該產品應該去 " + objOldSn.NEXT_STATION + " 進行掃描！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145335", new string[] { objOldSn.NEXT_STATION }));

                }
                if (objOldSn.COMPLETED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160344", new string[] { objOldSn.SN }));
                }
                if (objOldSn.PACKED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005155848", new string[] { objOldSn.SN }));
                }
                if (objOldSn.SHIPPED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { objOldSn.SN }));
                }
                if (objOldSn.REPAIR_FAILED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { objOldSn.SN }));
                }
                R_SN_AGING_INFO objAging = TRSA.GetSNAgingObj(SFCDB, "", "", "", "", "", "", oldsn, "");
                if (objAging == null)
                {
                    //throw new Exception("該序列號沒有被掃描過,不需要替換！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143216"));
                }
                if (objAging.IPADDRESS != IP)
                {
                    //throw new Exception("該序列號產品不是在此電腦掃描的,不能在此電腦替換！請去IP為" + objAging.IPADDRESS + "的電腦掃描");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143839", new string[] { objAging.IPADDRESS }));
                }
                if (objAging.WORK_FLAG == "4")
                {
                    //throw new Exception("該序列號產品正在老化不能替換！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144146"));
                }
                #endregion

                #region 新條碼狀態檢查
                R_SN objNewSn = TRS.LoadData(newsn, SFCDB);
                if (objNewSn == null)
                {
                    MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { newsn });
                }
                if (objNewSn.NEXT_STATION != "AGING")
                {
                    //throw new Exception("該產品應該去 " + objNewSn.NEXT_STATION + " 進行掃描！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145335", new string[] { objOldSn.NEXT_STATION }));
                }
                if (objNewSn.COMPLETED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160344", new string[] { objNewSn.SN }));
                }
                if (objNewSn.PACKED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005155848", new string[] { objNewSn.SN }));
                }
                if (objNewSn.SHIPPED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { objNewSn.SN }));
                }
                if (objNewSn.REPAIR_FAILED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { objNewSn.SN }));
                }
                R_SN_AGING_INFO objNewAaging = TRSA.GetSNAgingObj(SFCDB, "", "", "", "", "", "", newsn, "");
                if (objNewAaging != null)
                {
                    if (objNewAaging.WORK_FLAG == "1" || objNewAaging.WORK_FLAG == "2" || objNewAaging.WORK_FLAG == "3")
                    {
                        //throw new Exception("該序列號已經在IP:" + objNewAaging.IPADDRESS + "電腦上掃描過");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143839", new string[] { objAging.IPADDRESS }));
                    }
                    if (objNewAaging.WORK_FLAG == "4")
                    {
                        //throw new Exception("該序列號產品正在老化不能替換別的序號");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144146"));
                    }
                }
                #endregion

                C_AGING_CONFIG_DETAIL newACD = TCACD.GetConfigObject(SFCDB, "", objNewSn.SKUNO.Substring(objNewSn.SKUNO.Length - 8, 8), "", "");
                C_AGING_CONFIG_DETAIL oldACD = TCACD.GetConfigObject(SFCDB, objAging.CABINETNO, objAging.ITEMCODE, objAging.SHELFNO, objAging.TOOLSNO);
                if (newACD == null)
                {
                    //throw new Exception("新條碼的料號沒有配置老化信息！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145803"));
                }
                if (newACD.ITEM_CODE != oldACD.ITEM_CODE)
                {
                    //throw new Exception("不同料號的條碼不能替換老化信息！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150216"));
                }
                if (newACD.TOOLS_FLAG != oldACD.TOOLS_FLAG)
                {
                    //throw new Exception("一個使用工具板，一個不使用工具板，不能替換老化信息！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150618"));
                }                

                objAging.SN = objNewSn.SN;
                objAging.WORKORDERNO = objNewSn.WORKORDERNO;
                objAging.ITEMCODE = newACD.ITEM_CODE;
                objAging.ITEMNAME = newACD.ITEM_NAME;
                objAging.AGINGTIME = newACD.AGINGTIME.ToString();
                objAging.EDIT_EMP = LoginUser.EMP_NO;
                objAging.EDIT_TIME = GetDBDateTime();
                TRSA.UpdateByID(SFCDB, objAging);

                T_R_MES_LOG TRML = new T_R_MES_LOG(SFCDB, DBTYPE);
                R_MES_LOG log = new R_MES_LOG();
                log.ID = TRML.GetNewID(BU, SFCDB);
                log.FUNCTION_NAME = "ReplaceAgingSN";
                log.CLASS_NAME = " MESStation.Config.HWT.CAgeingTypeConfig";
                log.PROGRAM_NAME = "REPLACE AGING SN";
                log.EDIT_TIME = GetDBDateTime();
                log.EDIT_EMP = LoginUser.EMP_NO;
                log.LOG_MESSAGE = "REPLACE AGING SN:" + oldsn + "-->" + newsn;
                TRML.InsertMESLog(log, SFCDB);

                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Data = "";              
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 老化不良掃描
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AgingScanFail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                if (Data["FAILSN"] == null || Data["FAILSN"].ToString() == "")
                {
                    throw new Exception("Please input sn");
                }
                if (Data["FAILCODE"] == null || Data["FAILCODE"].ToString() == "")
                {
                    throw new Exception("Please input fail code");
                }              
                if (Data["PWD"] == null || Data["PWD"].ToString() == "")
                {
                    throw new Exception("Please input password");
                }
                if (Data["FAILCODE"].ToString().Trim() != "AGINGFAIL")
                {
                    throw new Exception("The fail code is error!");
                }
                string sn = Data["FAILSN"].ToString().Trim();
                string faicode = Data["FAILCODE"].ToString().Trim();
                string pwd = Data["PWD"].ToString().Trim();
                T_R_SN_AGING_INFO TRSA = new T_R_SN_AGING_INFO(SFCDB, DBTYPE);
                T_R_SN_LOCK TRSL = new T_R_SN_LOCK(SFCDB, DBTYPE);
                T_R_SN TRS = new T_R_SN(SFCDB, DBTYPE);
                T_C_AGING_CONFIG_DETAIL TCACD = new T_C_AGING_CONFIG_DETAIL(SFCDB, DBTYPE);               
                C_USER user = SFCDB.ORM.Queryable<C_USER>().Where(r => r.EMP_NO == LoginUser.EMP_NO && r.EMP_PASSWORD == pwd).ToList().FirstOrDefault();
                R_SN_LOCK lockSN = null;
                if (user == null)
                {
                    //throw new Exception("密碼錯誤！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000010"));
                }
                //檢查掃不良權限 ---待寫

                #region 條碼狀態檢查
                R_SN objSn = TRS.LoadData(sn, SFCDB);
                if (objSn == null)
                {
                    MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { sn });
                }
                if (objSn.NEXT_STATION != "AGING")
                {
                    //throw new Exception("該產品應該去 " + objSn.NEXT_STATION + " 進行掃描！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145335", new string[] { objSn.NEXT_STATION }));
                }
                if (objSn.COMPLETED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160344", new string[] { objSn.SN }));
                }
                if (objSn.PACKED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005155848", new string[] { objSn.SN }));
                }
                if (objSn.SHIPPED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { objSn.SN }));
                }
                if (objSn.REPAIR_FAILED_FLAG == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { objSn.SN }));
                }
                lockSN = TRSL.GetDetailBySN(SFCDB, objSn.SN, "AGING");
                if (lockSN != null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { objSn.SN, lockSN.LOCK_EMP, lockSN.LOCK_REASON }));
                }
                #endregion

                R_SN_AGING_INFO objAging = TRSA.GetSNAgingObj(SFCDB, "", "", "", "", "", "", sn, "");
                if (objAging != null)
                {
                    objAging.EVENTPASS = "0";
                    objAging.WORK_FLAG = "5";
                    objAging.REALFINISHTIME = GetDBDateTime();
                    objAging.ENDEMPNO = this.LoginUser.EMP_NO;
                    TRSA.UpdateByID(SFCDB, objAging);
                }

                //1 SN過站
                TRS.PassStation(objSn.SN, IP, "AGING", "AGING", BU, "FAIL", LoginUser.EMP_NO, SFCDB);
                //3.寫良率 R_YIELD_RATE_DETAIL
                TRS.RecordYieldRate(objSn.WORKORDERNO, 1, objSn.SN, "FAIL", IP, "AGING", LoginUser.EMP_NO, BU, SFCDB);
                //4.寫UPH R_UPH_DETAIL
                TRS.RecordUPH(objSn.WORKORDERNO, 1, objSn.SN, "FAIL", IP, "AGING", LoginUser.EMP_NO, BU, SFCDB);
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Data = "";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }
        /// <summary>
        /// 老化延長老化時間
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void AgingModifyTime(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                if (Data["CABINTNO"] == null || Data["CABINTNO"].ToString() == "")
                {
                    throw new Exception("Please input cabint no");
                }
                if (Data["TIME"] == null || Data["TIME"].ToString() == "")
                {
                    throw new Exception("Please input time");
                }
                if (Data["PWD"] == null || Data["PWD"].ToString() == "")
                {
                    throw new Exception("Please input password");
                }               
                string cabinet = Data["CABINTNO"].ToString().Trim();
                string time = Data["TIME"].ToString().Trim();
                string pwd = Data["PWD"].ToString().Trim();
                string regNum = @"^\+?[1-9][0-9]*$";

                if (!Regex.IsMatch(time, regNum))
                {
                    //throw new Exception("請輸入非零整數的老化時間！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152709"));
                }
                T_R_AGING_CABINET_INFO TRAC = new T_R_AGING_CABINET_INFO(SFCDB, DBTYPE);      
              
                T_C_AGING_CONFIG_DETAIL TCACD = new T_C_AGING_CONFIG_DETAIL(SFCDB, DBTYPE);
                C_USER user = SFCDB.ORM.Queryable<C_USER>().Where(r => r.EMP_NO == LoginUser.EMP_NO && r.EMP_PASSWORD == pwd).ToList().FirstOrDefault();         
                if (user == null)
                {
                    //throw new Exception("密碼錯誤！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000010"));
                }
                //檢查修改老化時間權限 ---待寫
                R_AGING_CABINET_INFO cabinetAging = TRAC.GetObjectByCabinet(SFCDB, "", cabinet, "4");
                if (cabinetAging == null)
                {
                    //throw new Exception(cabinet + "此老化櫃沒有在使用，沒有老化資訊！");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152850", new string[] { cabinet }));
                }
                DateTime dt = GetDBDateTime();
                dt.AddMinutes(Convert.ToInt32(time));

                cabinetAging.ENDTIME = dt;
                TRAC.UpdateByID(SFCDB, cabinetAging);

                SFCDB.ORM.Updateable<R_SN_AGING_INFO>().UpdateColumns(r => new R_SN_AGING_INFO { ENDTIME = dt })
                    .Where(r => r.CABINETNO == cabinet && r.WORK_FLAG == "4").ExecuteCommand();

                T_R_MES_LOG TRML = new T_R_MES_LOG(SFCDB, DBTYPE);
                R_MES_LOG log = new R_MES_LOG();
                log.ID = TRML.GetNewID(BU, SFCDB);
                log.FUNCTION_NAME = "AgingModifyTime";
                log.CLASS_NAME = " MESStation.Config.HWT.CAgeingTypeConfig";
                log.PROGRAM_NAME = "AGING MODIFY TIME";
                log.EDIT_TIME = GetDBDateTime();
                log.EDIT_EMP = LoginUser.EMP_NO;
                log.LOG_MESSAGE = "AGING MODIFY TIME CABINET:" + cabinetAging.CABINETNO + ";TIME " + cabinetAging.ENDTIME.ToString() + "-->" + dt.ToString();
                TRML.InsertMESLog(log, SFCDB);
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Data = "";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
        }

    }
}
