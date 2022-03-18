using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESStation.Config.Vertiv
{
    public class CRMAConfig : MesAPIBase
    {
        protected APIInfo FUploadExcel = new APIInfo()
        {
            FunctionName = "UploadExcel",
            Description = "Upload Excel",
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
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSearch = new APIInfo()
        {
            FunctionName = "Search",
            Description = "Search",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "LOT_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetFailRecord = new APIInfo()
        {
            FunctionName = "GetFailRecord",
            Description = "Get Upload Fail Record",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        public CRMAConfig()
        {
            this.Apis.Add(FUploadExcel.FunctionName, FUploadExcel);
            this.Apis.Add(FCheckUploadPermissions.FunctionName, FCheckUploadPermissions);
            this.Apis.Add(FSearch.FunctionName, FSearch);
            this.Apis.Add(FGetFailRecord.FunctionName, FGetFailRecord);
        }

        /// <summary>
        /// 上傳配置Excel文檔
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UploadExcel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            SFCDB.ThrowSqlExeception = true;
            try
            {
                string data = Data["DataList"].ToString();
                Newtonsoft.Json.Linq.JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);               
                
                var lotNo = GetSeqNo("RN", SFCDB);
                var successCount = 0;
                var failCount = 0;
                string SNs = string.Empty;
                List<string> listSN = new List<string>();
                for (int i = 0; i < array.Count; i++)
                {
                    try
                    {
                        var sn = array[i]["S/N"].ToString().Trim();
                        if (listSN.Contains(sn))
                        {
                            continue;
                        }
                        listSN.Add(sn);
                        CheckSN(sn, SFCDB);

                        T_R_RMA_BONEPILE tcacd = new T_R_RMA_BONEPILE(SFCDB, DBTYPE);
                        Row_R_RMA_BONEPILE rowCACD = null;
                        R_SN rSn = new R_SN();
                        R_MRB rMrb = new R_MRB();
                        T_R_SN trSn = new T_R_SN(SFCDB, this.DBTYPE);
                        T_R_MRB trMrb = new T_R_MRB(SFCDB, this.DBTYPE);
                        DateTime sysdate= tcacd.GetDBDateTime(SFCDB);
                        #region 寫入R_RMA_BONEPILE表
                        rowCACD = (Row_R_RMA_BONEPILE)tcacd.NewRow();
                        rowCACD.ID = tcacd.GetNewID(BU, SFCDB);
                        rowCACD.LOT_NO = lotNo;
                        rowCACD.SKUNO = array[i]["P/N"].ToString().Trim();
                        rowCACD.SN = sn;
                        rowCACD.RECEIVED_DATE = Convert.ToDateTime(array[i]["Received Date"].ToString().Trim());
                        rowCACD.LASTPACK_DATE = Convert.ToDateTime(GetPackingDate(sn, SFCDB));
                        rowCACD.FAIL_STATION = array[i]["DF Fail Station"].ToString().Trim();
                        rowCACD.FAILURE_SYMPTOM = array[i]["Failure Symptom"].ToString().Trim();
                        rowCACD.FAILURE_TYPES = array[i]["Failure Types"].ToString().Trim();
                        rowCACD.OWNER = array[i]["Owner"].ToString().Trim();
                        rowCACD.REMARK = array[i]["Remark"].ToString().Trim();
                        rowCACD.VALUABLE = array[i]["Valuable"].ToString().Trim();
                        rowCACD.RMA_TIMES = GetUploadTimes(0, sn, SFCDB);
                        rowCACD.FUNCTION_TIMES = GetUploadTimes(1, sn, SFCDB);
                        rowCACD.COSMETIC_TIMES = GetUploadTimes(2, sn, SFCDB);
                        rowCACD.CLOSED_FLAG = "0";
                        rowCACD.EDIT_EMP = LoginUser.EMP_NO;
                        rowCACD.EDIT_TIME = sysdate;
                        rowCACD.UPLOAD_EMP = LoginUser.EMP_NO;
                        rowCACD.UPLOAD_TIME = sysdate;
                        SFCDB.ExecSQL(rowCACD.GetInsertString(DBTYPE));
                        #endregion

                        #region 先獲取SN的工單下一站機種等信息再寫入R_MRB表
                        rSn = trSn.LoadSN(sn, SFCDB);//獲取SN的工單下一站機種等信息

                        rMrb.ID = trMrb.GetNewID(this.BU, SFCDB, this.DBTYPE);
                        rMrb.SN = rSn.SN;
                        rMrb.WORKORDERNO = rSn.WORKORDERNO;
                        rMrb.NEXT_STATION = rSn.NEXT_STATION;
                        rMrb.SKUNO = rSn.SKUNO;
                        rMrb.FROM_STORAGE = "";
                        rMrb.TO_STORAGE = "RMA";
                        rMrb.REWORK_WO = "";
                        rMrb.CREATE_EMP = LoginUser.EMP_NO;
                        rMrb.CREATE_TIME = sysdate;
                        rMrb.MRB_FLAG = "1";
                        rMrb.SAP_FLAG = "0";
                        rMrb.EDIT_EMP = LoginUser.EMP_NO;
                        rMrb.EDIT_TIME = rMrb.CREATE_TIME;
                        trMrb.Add(rMrb, SFCDB);//寫入R_MRB表

                        trSn.RecordPassStationDetail(sn, "Line1", "RMA", "RMA", this.BU, SFCDB);//寫入過站記錄表
                        #endregion

                        successCount += 1;
                        SNs = SNs+ "'" + sn + "',";
                    }
                    catch (Exception ex)
                    {
                        T_R_MES_LOG TRML = new T_R_MES_LOG(SFCDB, DBTYPE);
                        R_MES_LOG LOG = new R_MES_LOG
                        {
                            ID = TRML.GetNewID(BU, SFCDB),
                            DATA1 = array[i]["S/N"].ToString().Trim(),
                            FUNCTION_NAME = "UploadExcel",
                            CLASS_NAME = "MESStation.Config.Vertiv.CRMAConfig",
                            PROGRAM_NAME = "CloudMES",
                            EDIT_TIME = GetDBDateTime(),
                            EDIT_EMP = LoginUser.EMP_NO,
                            LOG_MESSAGE = ex.Message
                        };
                        TRML.InsertMESLog(LOG, SFCDB);
                        failCount += 1;

                        continue;
                    }
                }
                if (SNs.Length > 0)
                    UpdateSN(SNs, SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
                StationReturn.Message = "上傳成功[" + successCount.ToString() + "]筆,失敗[" + failCount.ToString() + "]筆!";
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
        /// RMA信息查詢
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Search(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string lotNo = Data["LOTNO"].ToString().Trim();
                string skuno = Data["SKUNO"].ToString().Trim();
                string sn = Data["SN"].ToString().Trim();
                List<R_RMA_BONEPILE> list = new List<R_RMA_BONEPILE>();
                T_R_RMA_BONEPILE trrb = new T_R_RMA_BONEPILE(SFCDB, DBTYPE);
                list = trrb.GetRMA_BONEPILEs(SFCDB, lotNo, skuno, sn);
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
        /// RMA上傳失敗信息查詢
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetFailRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<R_MES_LOG> list = new List<R_MES_LOG>();
                T_R_MES_LOG trml = new T_R_MES_LOG(SFCDB, DBTYPE);
                var dt = trml.GetMESLog("CloudMES", "MESStation.Config.Vertiv.CRMAConfig", "UploadExcel", "", "", SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var log = new R_MES_LOG
                        {
                            CLASS_NAME = dt.Rows[i]["CLASS_NAME"].ToString(),
                            FUNCTION_NAME = dt.Rows[i]["FUNCTION_NAME"].ToString(),
                            LOG_MESSAGE = dt.Rows[i]["LOG_MESSAGE"].ToString(),
                            DATA1 = dt.Rows[i]["DATA1"].ToString(),
                            EDIT_EMP = dt.Rows[i]["EDIT_EMP"].ToString(),
                            EDIT_TIME = Convert.ToDateTime(dt.Rows[i]["EDIT_TIME"])
                        };
                        list.Add(log);
                    }
                }
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
        /// 獲取新的LOT_NO
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        private string GetSeqNo(string prefix, MESDBHelper.OleExec DB)
        {
            var dt = new DataTable();
            var tempSeqNo = prefix + DateTime.Now.ToString("yyyyMMdd");
            #region SQL
            var sql = string.Format(@"
                select '{0}' || nvl(case length(to_char(max_seq + 1))
                                        when 1 then
                                        '00' || to_char(max_seq + 1)
                                        when 2 then
                                        '0' || to_char(max_seq + 1)
                                        else
                                        to_char(max_seq + 1)
                                    end,
                                    '001') temp_lotno
                  from (select max(distinct lot_no) max_lotno,
                               substr(max(distinct lot_no), -3) max_seq
                          from r_rma_bonepile
                         where substr(lot_no, 0, length('{0}')) = '{0}')", tempSeqNo);
            #endregion
            try
            {
                dt = DB.ExecSelect(sql).Tables[0];
            }
            catch (Exception ex)
            {
                //throw new Exception("獲取新LOT_NO失敗:" + ex.Message);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814161941", new string[] { ex.Message }));
            }
            return dt.Rows[0]["temp_lotno"].ToString();
        }

        /// <summary>
        /// 檢查SN信息
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        private void CheckSN(string sn, MESDBHelper.OleExec DB)
        {
            var dt = new DataTable();
            DataTable dtShip = new DataTable();
            DataTable dtSN = new DataTable();
            var sql = string.Empty;
            try
            {
                sql = string.Format(@"select * from r_rma_bonepile where closed_flag = 0 and sn = '{0}'", sn);
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                    //throw new Exception("SN已掃RMABoneplie但未Closed[r_rma_bonepile]!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162153"));

                sql = string.Format(@"select * from r_sn where valid_flag = 1 and sn = '{0}'", sn);
                dtSN = DB.ExecSelect(sql).Tables[0];
                if (dtSN.Rows.Count == 0)
                    //throw new Exception("SN不存在[r_sn]!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048"));

                sql = string.Format(@"select * from r_ship_detail where sn = '{0}'", sn);
                dtShip = DB.ExecSelect(sql).Tables[0];
                //VERTIV現在費領出貨都是手工更改r_sn的NEXT_STATION=SHIPFINISH
                if (dtShip.Rows.Count == 0 && dtSN.Rows[0]["NEXT_STATION"].ToString().ToUpper() != "SHIPFINISH")
                { 
                    //throw new Exception("SN未掃出貨[r_ship_detail]!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115950", new string[] { sn }));
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("檢查SN信息失敗:" + ex.Message);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163527", new string[] { ex.Message}));
            }
            
        }

        /// <summary>
        /// 獲取SN第一次過包裝工站時間
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        private string GetPackingDate(string sn, MESDBHelper.OleExec DB)
        {
            var dt = new DataTable();
            var sql = string.Format(@"select min(edit_time) edit_time from r_sn_station_detail where valid_flag = 1 and station_name = 'CARTON' and sn = '{0}'", sn);
            try
            {
                dt = DB.ExecSelect(sql).Tables[0];
            }
            catch (Exception ex)
            {
                //throw new Exception("獲取SN包裝信息失敗:" + ex.Message);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163546", new string[] { ex.Message }));
            }
            return dt.Rows[0]["edit_time"].ToString();
        }

        /// <summary>
        /// 獲取SN上傳RMA系統次數
        /// </summary>
        /// <param name="rma_type">0:rma,1:function, 2:cosmetic</param>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        private double GetUploadTimes(int rma_type, string sn, MESDBHelper.OleExec DB)
        {
            var dt = new DataTable();
            var sql = string.Format(@"select rma_times, function_times, cosmetic_times from r_rma_bonepile where sn = '{0}'", sn);
            try
            {
                dt = DB.ExecSelect(sql).Tables[0];
            }
            catch (Exception ex)
            {
                //throw new Exception("獲取SN上傳RMA系統次數失敗:" + ex.Message);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163727", new string[] { ex.Message }));
            }
            if (dt.Rows.Count == 0)
                return 1;
            else
                return Convert.ToDouble(dt.Rows[0][rma_type].ToString()) + 1;
        }

        /// <summary>
        /// 更新SN包裝完工出貨狀態以及屏蔽出貨記錄
        /// </summary>
        /// <param name="SNs"></param>
        /// <param name="DB"></param>
        private void UpdateSN(string SNs, MESDBHelper.OleExec DB)
        {
            var sql = string.Empty;
            try
            {
                SNs = SNs.Substring(0, SNs.Length - 1);
                sql = string.Format(@"
                    update r_sn 
                       set packed_flag = 0, completed_flag = 0, shipped_flag = 0, 
                           current_station = 'RMA', next_station = 'REWORK', 
                           edit_emp = '{0}', edit_time = sysdate 
                     where valid_flag = 1 and sn in ({1})", LoginUser.EMP_NO, SNs);
                DB.ExecSQL(sql);

                sql = string.Format(@"update r_ship_detail set sn = 'RMA' || sn, skuno = 'RMA' || skuno, dn_no = 'RMA' || dn_no where sn in ({0})", SNs);
                DB.ExecSQL(sql);

                sql = string.Format(@"
                    update r_sn_station_detail 
                       set next_station = 'REWORK', edit_emp = '{0}', edit_time = sysdate 
                     where sn in ({1}) and station_name = 'RMA'", LoginUser.EMP_NO, SNs);
                DB.ExecSQL(sql);
            }
            catch (Exception ex)
            {
                //throw new Exception("更新SN信息失敗:" + ex.Message);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163810", new string[] { ex.Message }));
            }
        }
    }
}
