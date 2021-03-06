using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.DCN
{
    public class DCNRepair : MesAPIBase
    {
        protected APIInfo FGetSNFailInfo = new APIInfo()
        {
            FunctionName = "GetSNFailInfo",
            Description = "Get SN Fail Info",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FFailCodeChecker = new APIInfo()
        {
            FunctionName = "FailCodeChecker",
            Description = "Fail Code Checker",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "FailCodeID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FActionCodeChecker = new APIInfo()
        {
            FunctionName = "ActionCodeChecker",
            Description = "Action Code Checker",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ActionCode", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FRootCauseDescLoader = new APIInfo()
        {
            FunctionName = "RootCauseDescLoader",
            Description = "Root Cause Desc Loader",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ActionCode", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RootCause", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ReplaceCSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TRSN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetTRSNInfo = new APIInfo()
        {
            FunctionName = "GetTRSNInfo",
            Description = "Get TR SN Info",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TR_SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSaveReplace = new APIInfo()
        {
            FunctionName = "SaveReplace",
            Description = "Save Replace",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "RepairSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FailCodeId", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FailCode", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ActionCode", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PCBASN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Process", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "KPNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Location", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Category", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RootCause", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CompomentID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ReturnStation", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "IsPCBA", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "OriginalCSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ReplaceCSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "OldTRSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NewTRSN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FSaveFinish = new APIInfo()
        {
            FunctionName = "SaveFinish",
            Description = "Save Finish",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetAllKPPartno = new APIInfo()
        {
            FunctionName = "GetAllKPPartno",
            Description = "Get All KP Partno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FRepairCheckOut = new APIInfo()
        {
            FunctionName = "RepairCheckOut",
            Description = "RepairCheckOut",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public DCNRepair()
        {
            this.Apis.Add(FGetSNFailInfo.FunctionName, FGetSNFailInfo);
            this.Apis.Add(FFailCodeChecker.FunctionName, FFailCodeChecker);
            this.Apis.Add(FActionCodeChecker.FunctionName, FActionCodeChecker);
            this.Apis.Add(FRootCauseDescLoader.FunctionName, FRootCauseDescLoader);
            this.Apis.Add(FGetTRSNInfo.FunctionName, FGetTRSNInfo);
            this.Apis.Add(FSaveReplace.FunctionName, FSaveReplace);
            this.Apis.Add(FSaveFinish.FunctionName, FSaveFinish);
            this.Apis.Add(FGetAllKPPartno.FunctionName, FGetAllKPPartno);
            this.Apis.Add(FRepairCheckOut.FunctionName, FRepairCheckOut);
        }
        public void GetSNFailInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sn = Data["SN"].ToString().Trim().ToUpper();
                var snObj = SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == sn && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                string skuno = snObj.SKUNO;
                string wo = snObj.WORKORDERNO;
                int repaired_count = 0;
                bool mds_flag = true;
                if (snObj == null)
                {
                    //throw new Exception($@"{sn} Not Exist!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { snObj.SN }));
                }

                //MRB中不允許掃維修，須先rework
                if (snObj.CURRENT_STATION.Equals("MRB"))
                {
                    //throw new Exception($@"{sn} in MRB,please rework first!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { snObj.SN }));
                }
                if (snObj.SHIPPED_FLAG.Equals("1"))
                {
                    //throw new Exception($@"{sn} Already Shipped!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { sn }));
                }
                if (snObj.REPAIR_FAILED_FLAG == "0")
                {
                    //正常品
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000078", new string[] { snObj.SN }));
                }
                T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(SFCDB, DBTYPE);
                T_R_REPAIR_ACTION TAction = new T_R_REPAIR_ACTION(SFCDB, DBTYPE);
                T_R_REPAIR_FAILCODE TFailcode = new T_R_REPAIR_FAILCODE(SFCDB, DBTYPE);
                T_R_REPAIR_TRANSFER t_r_repair = new T_R_REPAIR_TRANSFER(SFCDB, DBTYPE);

                if (!t_r_repair.SNIsRepairIn(snObj.SN, SFCDB))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154342", new string[] { snObj.SN }));
                }

                List<R_REPAIR_MAIN> repairMains = t_r_repair_main.GetRepairMainBySN(SFCDB, snObj.SN);
                R_REPAIR_MAIN rmObject = repairMains.Find(r => r.CLOSED_FLAG == "0");
                if (rmObject == null)
                {
                    //無維修主檔信息
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", snObj.SN }));
                }

                List<R_REPAIR_FAILCODE> rf = SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(f => f.SN == snObj.SN && f.REPAIR_FLAG == "0").ToList();
                if (rf.Count == 0)
                {
                    //throw new Exception($@"{snObj.SN} No Fail Info!R_REPAIR_FAILCODE");

                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104640", new string[] { snObj.SN }));
                }
                repaired_count = t_r_repair_main.GetRepairedCount(sn, SFCDB, DBTYPE);
                //獲取SN在Fail工站最大維修次數管控對象
                C_REPAIR_SN_CONTROL rSnControl = SFCDB.ORM.Queryable<C_REPAIR_SN_CONTROL>()
                    .Where(t => t.SN == snObj.SN && t.STATION == rmObject.FAIL_STATION && t.REPAIRCOUNT > 0)
                    .ToList().FirstOrDefault();

                //如果SN未配置Fail工站的最大維修次數
                if (rSnControl == null)
                {
                    //判斷是否Fail工站屬於特殊管控維修次數工站，如果不屬於則按正常流程卡關(都是3次)
                    C_CONTROL rMaxControl = SFCDB.ORM.Queryable<C_CONTROL>()
                        .Where(t => t.CONTROL_NAME == "REPAIR_MAXTIMES" && t.CONTROL_TYPE == "STATION" && t.CONTROL_VALUE.Contains(rmObject.FAIL_STATION))
                        .ToList().FirstOrDefault();
                    if (rMaxControl != null)
                    {
                        if (BU.Equals("VNDCN"))
                        {
                            //check repair count by Station
                            repaired_count = t_r_repair_main.GetRepairedCountByStation(sn, rmObject.FAIL_STATION, SFCDB, DBTYPE);
                        }

                        if (repaired_count >= int.Parse(rMaxControl.CONTROL_LEVEL))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200305093313", new string[] { snObj.SN, rmObject.FAIL_STATION, repaired_count.ToString() }));
                        }
                    }
                    else
                    {
                        if (repaired_count >= 3)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200305093313", new string[] { snObj.SN, rmObject.FAIL_STATION, "3" }));
                        }
                    }
                }
                else
                {
                    if (repaired_count >= rSnControl.REPAIRCOUNT)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200305094711", new string[] { snObj.SN, rmObject.FAIL_STATION, rSnControl.REPAIRCOUNT.ToString() }));
                    }
                }

                #region 獲取維修信息
                DataTable FailCodeInfo = new DataTable();
                FailCodeInfo = TFailcode.SelectFailCodeBySN(sn, SFCDB, DBTYPE);
                if (FailCodeInfo.Rows.Count == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616102950", new string[] { sn }));
                }
                object fail_code_list = ConvertToJson.DataTableToJson(FailCodeInfo);

                DataTable RepairActionInfo = new DataTable();
                RepairActionInfo = TAction.SelectRepairActionBySN(sn, SFCDB, DBTYPE);
                object action_list = ConvertToJson.DataTableToJson(RepairActionInfo);

                R_REPAIR_MAIN RRM = t_r_repair_main.GetSNBySN(snObj.SN, SFCDB);
                if (RRM.FAIL_STATION == "COSMETIC-FAILURE")
                {
                    mds_flag = false;
                }

                DataTable snKpDT = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == snObj.ID && t.VALID_FLAG == 1).Select(t => new
                {
                    t.SN,
                    t.VALUE,
                    t.PARTNO,
                    t.KP_NAME,
                    t.MPN,
                    t.SCANTYPE,
                    t.ITEMSEQ,
                    t.SCANSEQ,
                    t.STATION,
                    t.EXKEY1,
                    t.EXVALUE1,
                    t.EXKEY2,
                    t.EXVALUE2,
                    t.EDIT_EMP,
                    t.EDIT_TIME
                }).OrderBy(t => t.ITEMSEQ).OrderBy(t => t.SCANSEQ).ToDataTable();
                object kp_list = ConvertToJson.DataTableToJson(snKpDT);
                #endregion

                //MES裡面正常維修和RMA維修用同一個模塊
                //T_R_RMA_BONEPILE t_r_rma = new T_R_RMA_BONEPILE(SFCDB, this.DBTYPE);
                ////判斷SN是否屬於RMA產品
                //if (t_r_rma.RmaBonepileIsOpen(SFCDB, snObj.SN))
                //{
                //    /*throw new Exception($@"{snObj.SN} Is In RMA,Please Use RMA-Repair!");*///产品 {0} 已经入 RMA，请使用RMA-Repair模块！
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200304102250", new string[] { snObj.SN }));
                //}
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = new
                {
                    SKUNO = skuno,
                    WO = wo,
                    RepairedCount = repaired_count,
                    MDSFlag = mds_flag,
                    CodeList = fail_code_list,
                    AcionList = action_list,
                    KPList = kp_list
                };
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetAllKPPartno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sn = Data["SN"].ToString().Trim();
                if (string.IsNullOrEmpty(sn))
                {
                    throw new Exception("sn Is Null");
                }
                R_SN objSN = SFCDB.ORM.Queryable<R_SN>().Where(r_sn => r_sn.SN == sn && r_sn.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (objSN == null)
                {
                    //throw new Exception($@"{sn} Not Exists!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { sn }));
                }
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(SFCDB, this.DBTYPE);
                List<R_SN_KP> listKP = new List<R_SN_KP>();
                t_r_sn_kp.GetSnKP(SFCDB, sn, listKP);
                List<string> list_partno = new List<string>();
                //如果沒有Kp返回本身
                if (listKP.Count == 0)
                {
                    list_partno.Add(objSN.SKUNO);
                }
                else
                {
                    var partno = listKP.Select(r => r.PARTNO).Distinct().ToList();
                    list_partno.AddRange(partno);
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = list_partno;
                StationReturn.Message = "OK";

            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void FailCodeChecker(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string fail_code_id = Data["FailCodeID"].ToString().Trim();
                if (string.IsNullOrEmpty(fail_code_id))
                {
                    throw new Exception("FailCodeID Is Null");
                }
                T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(SFCDB, this.DBTYPE);
                Row_R_REPAIR_FAILCODE FailCodeRow;
                FailCodeRow = RepairFailcode.GetByFailCodeID(fail_code_id, SFCDB);
                if (FailCodeRow == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000192", new string[] { }));
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = "OK";
                StationReturn.Message = "OK";

            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void ActionCodeChecker(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string action_code = Data["ActionCode"].ToString().Trim().ToUpper();
                if (string.IsNullOrEmpty(action_code))
                {
                    throw new Exception("Action Code Is Null");
                }
                T_C_ACTION_CODE T_CAC = new T_C_ACTION_CODE(SFCDB, DBTYPE);
                C_ACTION_CODE objCAC = T_CAC.GetByActionCode(action_code, SFCDB);
                if (objCAC == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "ActionCode", action_code }));
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = objCAC.ENGLISH_DESC;
                StationReturn.Message = objCAC.CHINESE_DESC;

            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void RootCauseDescLoader(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string action_code = Data["ActionCode"].ToString().Trim().ToUpper();
                string root_cause = Data["RootCause"].ToString().Trim().ToUpper();
                string repalce_csn = Data["ReplaceCSN"].ToString().Trim().ToUpper();
                string tr_sn = Data["TRSN"].ToString().Trim();
                if (string.IsNullOrEmpty(action_code))
                {
                    throw new Exception("Action Code Is Null");
                }
                if (string.IsNullOrEmpty(root_cause))
                {
                    throw new Exception("Root Cause Is Null");
                }
                if (action_code.ToUpper().Trim() == "A12")
                {
                    //如果ActionCode=A12的情況下必須有換料信息才可以輸入RootCause
                    if (repalce_csn == "" && tr_sn == "")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200323082356"));
                    }
                }
                T_C_ERROR_CODE T_CEC = new T_C_ERROR_CODE(SFCDB, this.DBTYPE);
                C_ERROR_CODE objCEC = T_CEC.GetByErrorCode(root_cause, SFCDB);
                if (objCEC == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "RootCause", root_cause }));
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = objCEC.ENGLISH_DESC;
                StationReturn.Message = objCEC.CHINESE_DESC;

            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetTRSNInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            OleExec APDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                APDB = this.DBPools["APDB"].Borrow();
                string tr_sn = Data["TR_SN"].ToString().Trim();
                string type = "";
                try
                {
                    type = Data["Type"].ToString().Trim();
                }
                catch
                {
                }
                if (string.IsNullOrEmpty(tr_sn))
                {
                    throw new Exception("TR_SN Is Null");
                }
                if (this.BU == "FJZ" && type == "New")
                {
                    var res = MESPubLab.Common.HttpHelp.HttpPost("http://10.14.129.147:8003/Allparts.asmx/CheckTrsnCMP", "p_container_id=" + tr_sn, "application/x-www-form-urlencoded");

                    if (!res.Contains("OK"))
                    {
                        throw new Exception("[AllParts Fail]:Check TR_SN Fail" + res);
                    }
                    OleDbParameter[] parms = new OleDbParameter[4];
                    parms[0] = new OleDbParameter("G_TRSN", OleDbType.VarChar);
                    parms[0].Value = tr_sn;
                    parms[1] = new OleDbParameter("G_PROGRAM", OleDbType.VarChar);
                    parms[1].Value = "REPAIR";
                    parms[2] = new OleDbParameter("G_MOVETYPE", OleDbType.VarChar);
                    parms[2].Value = "REPLACED";
                    parms[3] = new OleDbParameter("RES", OleDbType.VarChar);
                    parms[3].Direction = ParameterDirection.Output;
                    var cres = APDB.ExecProcedureNoReturn("MES1.CHECK_TRSN", parms);
                    if (!cres.Equals("OK"))
                    {
                        throw new Exception("[AllParts Fail]:Check TR_SN Fail" + cres);
                    }
                }

                string sql = string.Format(@"
                        SELECT A.TR_SN, A.CUST_KP_NO, A.MFR_CODE, B.MFR_NAME, A.DATE_CODE, A.LOT_CODE,A.MFR_KP_NO
                          FROM MES4.R_TR_SN A, MES1.C_MFR_CONFIG B WHERE A.MFR_CODE = B.MFR_CODE AND TR_SN = '{0}'", tr_sn);
                DataTable dt = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { tr_sn, "MES4.R_TR_SN/MES1.C_MFR_CONFIG" }));
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = dt;
                StationReturn.Message = "OK";

            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
            }
        }
        public void SaveReplace(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            OleExec APDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                APDB = this.DBPools["APDB"].Borrow();
                SFCDB.BeginTrain();
                APDB.BeginTrain();
                string repair_sn = Data["RepairSN"].ToString().Trim().ToUpper();
                string fail_code_id = Data["FailCodeId"].ToString().Trim().ToUpper();
                string fail_code = Data["FailCode"].ToString().Trim().ToUpper();
                string action_code = Data["ActionCode"].ToString().Trim().ToUpper();
                string pcba_sn = Data["PCBASN"].ToString().Trim().ToUpper();
                string process = Data["Process"].ToString().Trim().ToUpper();
                string kp_no = Data["KPNO"].ToString().Trim().ToUpper();
                string repair_location = Data["Location"].ToString().Trim().ToUpper();
                string category = Data["Category"].ToString().Trim().ToUpper();
                string root_cause = Data["RootCause"].ToString().Trim().ToUpper();
                string compoment_id = Data["CompomentID"].ToString().Trim().ToUpper();
                string return_station = Data["ReturnStation"].ToString().Trim().ToUpper();
                string is_pcba = Data["IsPCBA"].ToString().Trim().ToUpper();
                string original_csn = Data["OriginalCSN"].ToString().Trim().ToUpper();
                string replace_csn = Data["ReplaceCSN"].ToString().Trim().ToUpper();
                string old_tr_sn = Data["OldTRSN"].ToString().Trim().ToUpper();
                string new_tr_sn = Data["NewTRSN"].ToString().Trim().ToUpper();
                string old_kp_no = Data["OldKPNO"].ToString().Trim().ToUpper();
                string oldMfrCode = Data["OldMfrCode"].ToString().Trim().ToUpper();
                string oldMfrName = Data["OldMfrName"].ToString().Trim().ToUpper();
                string oldDateCode = Data["OldDateCode"].ToString().Trim().ToUpper();
                string oldLotCode = Data["OldLotCode"].ToString().Trim().ToUpper();
                string oldMfrkpno = Data["OldMfrkpno"].ToString().Trim().ToUpper();
                string new_kp_no = Data["NewKPNO"].ToString().Trim().ToUpper();
                string mfrCode = Data["NewMfrCode"].ToString().Trim().ToUpper();
                string mfrName = Data["NewMfrName"].ToString().Trim().ToUpper();
                string dateCode = Data["NewDateCode"].ToString().Trim().ToUpper();
                string lotCode = Data["NewLotCode"].ToString().Trim().ToUpper();
                string newMfrkpno = Data["NewMfrkpno"].ToString().Trim().ToUpper();
                bool bChangeKpno = false;//換料標誌

                if (string.IsNullOrEmpty(repair_sn))
                {
                    throw new Exception("Repair SN Is Null");
                }
                if (string.IsNullOrEmpty(action_code))
                {
                    throw new Exception("Action Code Is Null");
                }
                #region 1.SN 檢查
                var snObj = SFCDB.ORM.Queryable<R_SN>().Where(s => s.SN == repair_sn && s.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (snObj == null)
                {
                    //throw new Exception($@"{} Not Exist!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { repair_sn }));

                }
                if (snObj.SHIPPED_FLAG.Equals("1"))
                {
                    //throw new Exception($@"{repair_sn} Already Shipped!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { repair_sn }));
                }
                if (snObj.REPAIR_FAILED_FLAG == "0")
                {
                    //正常品
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000078", new string[] { snObj.SN }));
                }
                T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(SFCDB, DBTYPE);
                T_R_REPAIR_ACTION t_r_repair_action = new T_R_REPAIR_ACTION(SFCDB, DBTYPE);
                T_R_REPAIR_FAILCODE t_r_repair_failcode = new T_R_REPAIR_FAILCODE(SFCDB, DBTYPE);
                T_R_REPAIR_TRANSFER t_r_repair_transfer = new T_R_REPAIR_TRANSFER(SFCDB, DBTYPE);

                if (!t_r_repair_transfer.SNIsRepairIn(snObj.SN, SFCDB))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154342", new string[] { snObj.SN }));
                }
                List<R_REPAIR_MAIN> repairMains = t_r_repair_main.GetRepairMainBySN(SFCDB, snObj.SN);
                R_REPAIR_MAIN rmObject = repairMains.Find(m => m.CLOSED_FLAG == "0");
                if (rmObject == null)
                {
                    //無維修主檔信息
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { "SN", snObj.SN }));
                }
                List<R_REPAIR_FAILCODE> fail_code_list = SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(f => f.SN == snObj.SN && f.REPAIR_FLAG == "0").ToList();
                if (fail_code_list.Count == 0)
                {
                    //throw new Exception($@"{snObj.SN} No Fail Info!R_REPAIR_FAILCODE");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104640", new string[] { snObj.SN }));
                }

                Row_R_REPAIR_FAILCODE FailCodeRow;
                FailCodeRow = t_r_repair_failcode.GetByFailCodeID(fail_code_id, SFCDB);
                if (FailCodeRow == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000192", new string[] { }));
                }
                T_C_ACTION_CODE T_CAC = new T_C_ACTION_CODE(SFCDB, DBTYPE);
                C_ACTION_CODE objCAC = T_CAC.GetByActionCode(action_code, SFCDB);
                if (objCAC == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "ActionCode", action_code }));
                }
                if (action_code == "A12" || root_cause == "E206" || root_cause == "E205")
                {
                    bChangeKpno = true;
                    //如果ActionCode=A12的情況下必須有換料信息才可以輸入RootCause
                    if (kp_no == "" && new_tr_sn == "")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200323082356"));
                    }
                }
                T_C_ERROR_CODE T_CEC = new T_C_ERROR_CODE(SFCDB, this.DBTYPE);
                C_ERROR_CODE objCEC = T_CEC.GetByErrorCode(root_cause, SFCDB);
                if (objCEC == null && objCAC.ENGLISH_DESC != "NTF")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "RootCause", root_cause }));
                }
                #endregion

                #region 3.如果TR_SN有值,取Allpart中TR_SN的信息
                string sql = string.Format(@"
                        SELECT A.TR_SN, A.CUST_KP_NO, A.MFR_CODE, B.MFR_NAME, A.DATE_CODE, A.LOT_CODE,A.MFR_KP_NO
                          FROM MES4.R_TR_SN A, MES1.C_MFR_CONFIG B WHERE A.MFR_CODE = B.MFR_CODE");

                if (!string.IsNullOrEmpty(old_tr_sn))
                {
                    sql += string.Format(@" AND TR_SN = '{0}'", old_tr_sn);
                    DataTable dt = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        old_kp_no = dt.Rows[i]["CUST_KP_NO"].ToString();
                        oldMfrCode = dt.Rows[i]["MFR_CODE"].ToString();
                        oldMfrName = dt.Rows[i]["MFR_NAME"].ToString();
                        oldDateCode = dt.Rows[i]["DATE_CODE"].ToString();
                        oldLotCode = dt.Rows[i]["LOT_CODE"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(new_tr_sn))
                {
                    sql += string.Format(@" AND TR_SN = '{0}'", new_tr_sn);
                    DataTable dt = APDB.ExecuteDataTable(sql, CommandType.Text, null);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        new_kp_no = dt.Rows[i]["CUST_KP_NO"].ToString();
                        mfrCode = dt.Rows[i]["MFR_CODE"].ToString();
                        mfrName = dt.Rows[i]["MFR_NAME"].ToString();
                        dateCode = dt.Rows[i]["DATE_CODE"].ToString();
                        lotCode = dt.Rows[i]["LOT_CODE"].ToString();
                    }
                }
                #endregion

                //R_REPAIR_MAIN objRRM = SFCDB.ORM.Queryable<R_REPAIR_MAIN>().Where(t => t.SN == objSN.SerialNo && t.CLOSED_FLAG == "0").ToList().FirstOrDefault(); 
                #region 3.更新Allpart替換信息[MES4.R_KP_REPLACE/MES4.R_TR_PRODUCT_DETAIL.REPLACE_FLAG]
              //  if (objCAC.ENGLISH_DESC != "NTF" && (action_code == "A12" || action_code == "A13" || root_cause == "E206" || root_cause == "E205") && !string.IsNullOrEmpty(pcba_sn))
              //蘇貴不卡E206 A12   2021/08/04
               if (objCAC.ENGLISH_DESC != "NTF" && ( action_code == "A13" || root_cause == "E205") && !string.IsNullOrEmpty(pcba_sn))
                    {
                    var pcbaObj = SFCDB.ORM.Queryable<R_SN>().Where(s => s.SN == pcba_sn && s.VALID_FLAG == "1").ToList().FirstOrDefault();
                    if (pcbaObj == null)
                    {
                        //throw new Exception($@"PCBA SN [{pcba_sn}] Not Exist![R_SN]");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { pcba_sn }));
                    }
                    string result = "";
                    if (this.BU == "FJZ")
                    {
                        OleDbParameter[] spParam = new OleDbParameter[15];
                        spParam[0] = new OleDbParameter("MYPSN", pcbaObj.SN);
                        spParam[1] = new OleDbParameter("MYWO", pcbaObj.WORKORDERNO);
                        spParam[2] = new OleDbParameter("MYSTATION", rmObject.FAIL_STATION);
                        spParam[3] = new OleDbParameter("MYLOCATION", repair_location);
                        spParam[4] = new OleDbParameter("MYTRSN", old_tr_sn);
                        spParam[5] = new OleDbParameter("NEWTRSN", new_tr_sn);
                        spParam[6] = new OleDbParameter("REPLACESN", fail_code_id);
                        spParam[7] = new OleDbParameter("NEWKPSN", "");
                        spParam[8] = new OleDbParameter("OLDKPSN", "");
                        spParam[9] = new OleDbParameter("MYEMP", LoginUser.EMP_NO);
                        spParam[10] = new OleDbParameter("P_NO", pcbaObj.SKUNO);
                        spParam[11] = new OleDbParameter("FAIL_SYMPTOM", objCEC.ENGLISH_DESC);
                        spParam[12] = new OleDbParameter("ID_NO", "");
                        spParam[13] = new OleDbParameter("G_ERROR_CODE", objCEC.ERROR_CODE);
                        spParam[14] = new OleDbParameter();
                        spParam[14].Size = 1000;
                        spParam[14].ParameterName = "RES";
                        spParam[14].Direction = ParameterDirection.Output;
                        result = APDB.ExecProcedureNoReturn("MES1.Z_Insert_Kp_Replace_SERVER", spParam);
                    }
                    else
                    {
                        OleDbParameter[] spParam = new OleDbParameter[19];
                        spParam[0] = new OleDbParameter("MYPSN", pcbaObj.SN);
                        spParam[1] = new OleDbParameter("MYWO", pcbaObj.WORKORDERNO);
                        spParam[2] = new OleDbParameter("MYSTATION", rmObject.FAIL_STATION);
                        spParam[3] = new OleDbParameter("MYLOCATION", repair_location);
                        spParam[4] = new OleDbParameter("MYTRSN", old_tr_sn);
                        spParam[5] = new OleDbParameter("NEWTRSN", new_tr_sn);
                        spParam[6] = new OleDbParameter("REPLACESN", "");
                        spParam[7] = new OleDbParameter("NEWKPSN", "");
                        spParam[8] = new OleDbParameter("OLDKPSN", "");
                        spParam[9] = new OleDbParameter("MYEMP", LoginUser.EMP_NO);
                        spParam[10] = new OleDbParameter("P_NO", pcbaObj.SKUNO);
                        spParam[11] = new OleDbParameter("FAIL_SYMPTOM", objCEC.ENGLISH_DESC);
                        spParam[12] = new OleDbParameter("ID_NO", LoginUser.EMP_NO);
                        spParam[13] = new OleDbParameter("G_ERROR_CODE", objCEC.ERROR_CODE);
                        spParam[14] = new OleDbParameter("G_Failure_PN", snObj.SKUNO);
                        spParam[15] = new OleDbParameter("G_Failure_SN", snObj.SN);
                        spParam[16] = new OleDbParameter("G_FAIL_ID", "");
                        spParam[17] = new OleDbParameter("CHECK_FLAG", "1");
                        spParam[18] = new OleDbParameter();
                        spParam[18].Size = 1000;
                        spParam[18].ParameterName = "RES";
                        spParam[18].Direction = ParameterDirection.Output;
                        result = APDB.ExecProcedureNoReturn("MES1.Z_INSERT_KP_REPLACE_UPDATE", spParam);
                    }
                    if (!result.ToUpper().StartsWith("OK"))
                    {
                        throw new Exception("Allpart Error:" + result);
                    }
                }
                #endregion
                //取得R_REPAIR_MAIN.FAIL_STATION、R_REPAIR_FAILCODE.FAIL_CODE
                var failList = SFCDB.ORM.Queryable<R_REPAIR_MAIN, R_REPAIR_FAILCODE>((rm, rf) => rm.ID == rf.MAIN_ID)
                    .Where((rm, rf) => rf.REPAIR_FLAG == "0" && rf.ID == fail_code_id && rf.SN == snObj.SN).Select((rm, rf) => new { rm.FAIL_STATION, rf.FAIL_CODE }).ToList();
                if (failList.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { snObj.SN, fail_code_id }));
                }
                DateTime sys_date = SFCDB.ORM.GetDate();

                #region 4.寫入R_REPAIR_ACTION表

                Row_R_REPAIR_ACTION ActionRow = (Row_R_REPAIR_ACTION)t_r_repair_action.NewRow();
                ActionRow.ID = t_r_repair_failcode.GetNewID(BU, SFCDB);
                ActionRow.REPAIR_FAILCODE_ID = fail_code_id;
                ActionRow.SN = snObj.SN;
                ActionRow.ACTION_CODE = action_code;
                ActionRow.PROCESS = process;
                ActionRow.REASON_CODE = root_cause;
                ActionRow.DESCRIPTION = objCEC == null ? "" : objCEC.ENGLISH_DESC;
                ActionRow.FAIL_LOCATION = repair_location;
                ActionRow.FAIL_CODE = failList[0].FAIL_CODE;
                ActionRow.KEYPART_SN = pcba_sn;
                ActionRow.NEW_KEYPART_SN = "";
                if (bChangeKpno)
                {
                    if (string.IsNullOrEmpty(kp_no))
                    {
                        ActionRow.KP_NO = old_kp_no;
                    }
                    else
                    {
                        ActionRow.KP_NO = kp_no;
                    }
                }
                ActionRow.TR_SN = old_tr_sn;
                ActionRow.MFR_CODE = oldMfrCode;
                ActionRow.MFR_NAME = oldMfrName;
                ActionRow.DATE_CODE = oldDateCode;
                ActionRow.LOT_CODE = oldLotCode;
                ActionRow.NEW_KP_NO = new_kp_no;
                ActionRow.NEW_TR_SN = new_tr_sn;
                ActionRow.NEW_MFR_CODE = mfrCode;
                ActionRow.NEW_MFR_NAME = mfrName;
                ActionRow.NEW_DATE_CODE = dateCode;
                ActionRow.NEW_LOT_CODE = lotCode;
                ActionRow.REPAIR_EMP = LoginUser.EMP_NO;
                ActionRow.REPAIR_TIME = SFCDB.ORM.GetDate();
                ActionRow.EDIT_EMP = LoginUser.EMP_NO;
                ActionRow.EDIT_TIME = sys_date;
                ActionRow.COMPOMENTID = compoment_id;//Add By ZHB 20200805
                ActionRow.MPN = oldMfrkpno;//Add By ZHB 20200814
                ActionRow.NEW_MPN = newMfrkpno;//Add By ZHB 20200814
                string res = SFCDB.ExecSQL(ActionRow.GetInsertString(DBTYPE));
                if (res != "1")
                {
                    throw new Exception(res);
                }
                #endregion

                #region 5.執行完維修動作後更新R_REPAIR_FAILCODE.REPAIR_FLAG=1

                Row_R_REPAIR_FAILCODE R_RFailCode = (Row_R_REPAIR_FAILCODE)t_r_repair_failcode.GetObjByID(fail_code_id, SFCDB);
                R_RFailCode.REPAIR_FLAG = "1";
                R_RFailCode.EDIT_TIME = sys_date;
                var r = SFCDB.ExecSQL(R_RFailCode.GetUpdateString(DBTYPE));
                if (Convert.ToInt32(r) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_REPAIR_FAILCODE.REPAIR_FLAG=1" }));
                }
                #endregion

                //取得R_SN對象

                #region 6.更新下一站:DCN只有當SN的當前站是特殊工站才會更新下一站
                if (snObj.NEXT_STATION != return_station)
                {
                    //修復個BUG 只改下一站不改當前站 wuqing 20201118
                    string CurrentStation = "";
                    LogicObject.Route routeDetail = new LogicObject.Route(snObj.ROUTE_ID, LogicObject.GetRouteType.ROUTEID, SFCDB, DB_TYPE_ENUM.Oracle);
                    List<string> snStationList = new List<string>();
                    List<LogicObject.RouteDetail> routeDetailList = routeDetail.DETAIL;
                    LogicObject.RouteDetail R = routeDetailList.Where(rr => rr.STATION_NAME == return_station).FirstOrDefault();

                    CurrentStation = routeDetailList.Where(rr => rr.SEQ_NO == (R.SEQ_NO - 10)).FirstOrDefault().STATION_NAME;
                    //防止在MRB狀態 維修清號時改了R_SN 當前工站與下一站 chc 20210625
                    if (snObj.NEXT_STATION == "REWORK") {
                        CurrentStation = "MRB";
                        return_station = "REWORK";
                    }
                    var a = SFCDB.ORM.Updateable<R_SN>().SetColumns(rsn => new R_SN
                    {
                        CURRENT_STATION = CurrentStation,
                        NEXT_STATION = return_station,
                        EDIT_EMP = LoginUser.EMP_NO,
                        EDIT_TIME = sys_date
                    }).Where(rsn => rsn.ID == snObj.ID).ExecuteCommand();
                    if (a <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN.NEXT_STATION" }));
                    }
                    T_R_SN table = new T_R_SN(SFCDB, this.DBTYPE);                   
                    table.RecordPassStationDetail(new List<R_SN>() { snObj }, "Repair", "RETURN", $"{snObj.NEXT_STATION}->{return_station}", BU, SFCDB);
                }
                #endregion

                #region 7.如果機種已配置REPAIR_LOCK信息且SN連續3次測試FAIL,則鎖住
                if (objCEC != null)
                {


                    C_CONTROL objCC = SFCDB.ORM.Queryable<C_CONTROL>()
                        .Where(t => t.CONTROL_NAME == "REPAIR_LOCK" && t.CONTROL_VALUE.Contains(snObj.SKUNO) && t.CONTROL_VALUE.Contains(objCEC.ENGLISH_DESC)).ToList().FirstOrDefault();
                    if (objCC != null)
                    {
                        bool passFlag = false;
                        List<R_TEST_RECORD> objRTR = SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == snObj.SN).OrderBy(t => t.STARTTIME).Take(3).ToList();
                        for (int i = 0; i < objRTR.Count; i++)
                        {
                            if (objRTR[i].STATE == "PASS")
                            {
                                passFlag = true;
                            }
                        }
                        if (!passFlag)
                        {
                            R_SN_LOCK objRSL = new R_SN_LOCK();
                            objRSL = SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.SN == snObj.SN && t.LOCK_STATION == "" && t.LOCK_STATUS == "1").ToList().FirstOrDefault();
                            if (objRSL == null)
                            {
                                objRSL.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOCK");
                                objRSL.SN = snObj.SN;
                                objRSL.LOCK_EMP = LoginUser.EMP_NO;
                                objRSL.LOCK_REASON = $@"'{snObj.SKUNO}'/Repairaction:(NTF OR RETEST)/Test fail 3 times continuously";
                                objRSL.LOCK_STATUS = "1";
                                objRSL.LOCK_TIME = DateTime.Now;
                                objRSL.TYPE = "SN";
                                objRSL.LOCK_STATION = failList[0].FAIL_STATION;
                                SFCDB.ORM.Insertable(objRSL).ExecuteCommand();
                            }
                        }
                    }
                }
                #endregion
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = "OK";
                StationReturn.Message = "OK";
                SFCDB.CommitTrain();
                APDB.CommitTrain();
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
                if (SFCDB != null)
                {
                    SFCDB.RollbackTrain();
                }
                if (APDB != null)
                {
                    APDB.RollbackTrain();
                }
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
            }
        }
        public void SaveFinish(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            OleExec APDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                APDB = this.DBPools["APDB"].Borrow();
                SFCDB.BeginTrain();
                APDB.BeginTrain();
                string repair_sn = Data["RepairSN"].ToString().Trim().ToUpper();
                string station_name = Data["StationName"].ToString().Trim().ToUpper();
                string line_name = Data["LineName"].ToString().Trim().ToUpper();


                var snObj = SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == repair_sn && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (snObj == null)
                {
                    throw new Exception($@"{repair_sn} Not Exist!");
                }
                T_R_SN T_RSN = new T_R_SN(SFCDB, DBTYPE);
                T_R_REPAIR_MAIN T_RMain = new T_R_REPAIR_MAIN(SFCDB, DBTYPE);
                List<R_REPAIR_MAIN> objRRMList = new List<R_REPAIR_MAIN>();
                List<R_REPAIR_FAILCODE> objRRFList = new List<R_REPAIR_FAILCODE>();
                T_C_ROUTE_DETAIL T_CRDetail = new T_C_ROUTE_DETAIL(SFCDB, DBTYPE);

                #region double check
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(SFCDB, DBTYPE);
                List<R_SN_KP> list_kp = new List<R_SN_KP>();
                t_r_sn_kp.GetSnKP(SFCDB, snObj.SN, list_kp);
                // edit chc 不知道爲什麽SCANTYPE 為空 2021/9/4 
                List<R_SN_KP> list_pcba = list_kp.FindAll(r =>r.SCANTYPE !=null && r.SCANTYPE !="" && r.SCANTYPE.StartsWith("PCBA"));
                DateTime sysdate = SFCDB.ORM.GetDate();
                T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(SFCDB, DBTYPE);
                R_SN_LOG check_log = null;
                List<R_SN_KP> check_pcab = new List<R_SN_KP>();
                int check_time = list_pcba.Count;
                for (var i = 0; i < check_time; i++)
                {
                    UIInputData O = new UIInputData() { };
                    O.Timeout = 3000000;
                    O.IconType = IconType.Warning;
                    O.Type = UIInputType.String;
                    O.Tittle = "RepairCheckOutDoubleCheck";
                    O.ErrMessage = "No input";
                    O.UIArea = new string[] { "40%", "50%" };
                    O.OutInputs.Clear();
                    O.Message = "PCBA SN";
                    O.Name = "PCBA_SN";
                    O.CBMessage = "";
                    while (true)
                    {
                        var input_sn = O.GetUiInput(this, UIInput.Normal);
                        if (input_sn == null)
                        {
                            O.CBMessage = $@"Please Scan PCBA SN";
                        }
                        else
                        {
                            string check_sn = input_sn.ToString().Trim();
                            if (string.IsNullOrEmpty(check_sn))
                            {
                                O.CBMessage = $@"Please Scan PCBA SN";
                            }
                            else if (check_sn.Equals("No input"))
                            {
                                throw new Exception("User Cancel");
                            }
                            else
                            {
                                R_SN_KP k = check_pcab.Find(r => r.VALUE == check_sn);
                                if (k == null)
                                {
                                    k = list_pcba.Find(r => r.VALUE == check_sn);
                                    if (k == null)
                                    {
                                        O.CBMessage = $@"{check_sn} is not PCBA SN";
                                    }
                                    else
                                    {
                                        list_pcba.Remove(k);
                                        check_pcab.Add(k);
                                        check_log = new R_SN_LOG();
                                        check_log.ID = t_r_sn_log.GetNewID(BU, SFCDB);
                                        check_log.SNID = snObj.ID;
                                        check_log.SN = snObj.SN;
                                        check_log.LOGTYPE = "RepairCheckOut";
                                        check_log.DATA1 = "CheckPCBA";
                                        check_log.DATA2 = k.PARTNO;
                                        check_log.DATA3 = check_sn;
                                        check_log.DATA4 = k.KP_NAME;
                                        check_log.DATA5 = k.SCANTYPE;
                                        check_log.DATA6 = k.STATION;
                                        check_log.DATA7 = k.LOCATION;
                                        check_log.FLAG = "1";
                                        check_log.CREATETIME = sysdate;
                                        check_log.CREATEBY = LoginUser.EMP_NO;
                                        SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                        break;
                                    }
                                }
                                else
                                {
                                    O.CBMessage = $@"{check_sn} Already Scan!";
                                }

                            }
                        }
                    }
                }
                #endregion

                objRRMList = T_RMain.GetRepairMainBySN(SFCDB, snObj.SN).FindAll(r => r.CLOSED_FLAG == "0");
                if (objRRMList == null || objRRMList.Count == 0)
                {
                    //維修主表無信息
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000079", new string[] { snObj.SN, "CLOSED_FLAG=0" }));
                }
                else if (objRRMList.Count > 1)
                {
                    //維修主表有多條為維修完成的記錄
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180621185023", new string[] { snObj.SN }));
                }

                objRRFList = SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(t => t.MAIN_ID == objRRMList[0].ID && t.SN == snObj.SN && t.REPAIR_FLAG == "0").ToList();
                if (objRRFList.Count != 0)
                {
                    //未維修完成的無法更新R_REPAIR_MAIN表信息
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000106", new string[] { snObj.SN, objRRFList[0].ID }));
                }

                //執行完所有的維修動作後才能更新R_REPAIR_MAIN  FLAG=1 
                Row_R_REPAIR_MAIN R_RMain = (Row_R_REPAIR_MAIN)T_RMain.GetObjByID(objRRMList[0].ID, SFCDB);
                R_RMain.CLOSED_FLAG = "1";
                R_RMain.EDIT_EMP = LoginUser.EMP_NO;
                R_RMain.EDIT_TIME = SFCDB.ORM.GetDate();
                var result = SFCDB.ExecSQL(R_RMain.GetUpdateString(DBTYPE));
                if (Convert.ToInt32(result) <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_REPAIR_MAIN.CLOSED_FLAG=1" }));
                }

                #region 添加外觀不良維修後清除Bonepile的動作 By ZHB 2021年10月12日14:03:54
                T_R_NORMAL_BONEPILE t_r_normal_bonepile = new T_R_NORMAL_BONEPILE(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_REPAIR_ACTION t_r_repair_action = new T_R_REPAIR_ACTION(SFCDB, DB_TYPE_ENUM.Oracle);
                T_C_ACTION_CODE t_c_action_code = new T_C_ACTION_CODE(SFCDB, DB_TYPE_ENUM.Oracle);
                T_C_ERROR_CODE t_c_error_code = new T_C_ERROR_CODE(SFCDB, DB_TYPE_ENUM.Oracle);
                R_NORMAL_BONEPILE normalBonepile = t_r_normal_bonepile.GetOpenRecord(SFCDB, snObj.SN);
                string failCodeID = SFCDB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(t => t.MAIN_ID == objRRMList[0].ID && t.SN == snObj.SN).ToList().FirstOrDefault().ID;
                R_REPAIR_ACTION action = t_r_repair_action.GetActionByFailCodeID(SFCDB, failCodeID);
                if (normalBonepile != null && objRRMList.Count > 0 && normalBonepile.FAIL_STATION == objRRMList[0].FAIL_STATION)
                {
                    C_ACTION_CODE actionCode = t_c_action_code.GetByActionCode(action.ACTION_CODE, SFCDB);
                    C_ERROR_CODE errorCode = t_c_error_code.GetByErrorCode(action.REASON_CODE, SFCDB);
                    normalBonepile.REPAIR_ACTION = actionCode == null ? "" : actionCode.ENGLISH_DESC;
                    normalBonepile.DEFECT_DESCRIPTION = errorCode == null ? "" : errorCode.ENGLISH_DESC;
                    normalBonepile.REPAIR_DATE = action.REPAIR_TIME;
                    normalBonepile.REMARK = action.DESCRIPTION;
                    normalBonepile.LASTEDIT_BY = LoginUser.EMP_NO;
                    normalBonepile.LASTEDIT_DATE = action.REPAIR_TIME;
                    if (normalBonepile.FAIL_STATION.ToUpper() == "COSMETIC" || normalBonepile.FAIL_STATION.ToUpper() == "COSMETIC-FAILURE")
                    {
                        normalBonepile.CLOSED_FLAG = "1";
                        normalBonepile.CLOSED_BY = LoginUser.EMP_NO;
                        normalBonepile.CLOSED_DATE = normalBonepile.LASTEDIT_DATE;
                        normalBonepile.CLOSED_REASON = normalBonepile.FAIL_STATION;
                    }
                    int res = t_r_normal_bonepile.Update(SFCDB, normalBonepile);
                    if (res <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_NORMAL_BONEPILE:" + snObj.SN, "UPDATE" }));
                    }
                }
                #endregion

                //添加過站記錄
                T_RSN.RecordPassStationDetail(snObj.SN, line_name, station_name, station_name, BU, SFCDB);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = "OK";
                StationReturn.Message = "OK";
                SFCDB.CommitTrain();
                APDB.CommitTrain();
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
                if (SFCDB != null)
                {
                    SFCDB.RollbackTrain();
                }
                if (APDB != null)
                {
                    APDB.RollbackTrain();
                }
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
            }
        }

        public void RepairDoubleCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string repair_sn = Data["RepairSN"].ToString().Trim().ToUpper();
                T_R_SN t_r_sn = new T_R_SN(SFCDB, DBTYPE);
                R_SN snObj = t_r_sn.LoadSN(repair_sn, SFCDB);
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(SFCDB, DBTYPE);
                List<R_SN_KP> list_kp = new List<R_SN_KP>();
                t_r_sn_kp.GetSnKP(SFCDB, snObj.SN, list_kp);
                List<R_SN_KP> list_pcba = list_kp.FindAll(r => r.SCANTYPE.StartsWith("PCBA"));
                DateTime sysdate = SFCDB.ORM.GetDate();
                T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(SFCDB, DBTYPE);
                R_SN_LOG check_log = null;
                int check_time = list_pcba.Count;
                for (var i = 0; i < check_time; i++)
                {
                    UIInputData O = new UIInputData() { };
                    O.Timeout = 3000000;
                    O.IconType = IconType.Warning;
                    O.Type = UIInputType.String;
                    O.Tittle = "RepairCheckOutDoubleCheck";
                    O.ErrMessage = "No input";
                    O.UIArea = new string[] { "40%", "70%" };
                    O.OutInputs.Clear();
                    O.Message = "PCBA SN";
                    O.Name = "PCBA_SN";
                    O.CBMessage = "";
                    while (true)
                    {
                        var input_sn = O.GetUiInput(this, UIInput.Normal);
                        if (input_sn == null)
                        {
                            O.CBMessage = $@"Please Scan PCBA SN";
                        }
                        else
                        {
                            string check_sn = input_sn.ToString().Trim();
                            if (string.IsNullOrEmpty(check_sn))
                            {
                                O.CBMessage = $@"Please Scan PCBA SN";
                            }
                            else if (check_sn.Equals("No input"))
                            {
                                throw new Exception("User Cancel");
                            }
                            else
                            {
                                var k = list_pcba.Find(r => r.VALUE == check_sn);
                                if (k == null)
                                {
                                    O.CBMessage = $@"{check_sn} not exists in keypart";
                                }
                                else
                                {
                                    list_pcba.Remove(k);
                                    check_log = new R_SN_LOG();
                                    check_log.ID = t_r_sn_log.GetNewID(BU, SFCDB);
                                    check_log.SNID = snObj.ID;
                                    check_log.SN = snObj.SN;
                                    check_log.LOGTYPE = "RepairCheckOut";
                                    check_log.DATA1 = "CheckPCBA";
                                    check_log.DATA2 = k.PARTNO;
                                    check_log.DATA3 = check_sn;
                                    check_log.DATA4 = k.KP_NAME;
                                    check_log.DATA5 = k.SCANTYPE;
                                    check_log.DATA6 = k.STATION;
                                    check_log.DATA7 = k.LOCATION;
                                    check_log.FLAG = "1";
                                    check_log.CREATETIME = sysdate;
                                    check_log.CREATEBY = LoginUser.EMP_NO;
                                    SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                    break;
                                }
                            }
                        }
                    }
                }

                //foreach (var k in list_pcba)
                //{
                //    UIInputData O = new UIInputData() { };
                //    O.Timeout = 3000000;
                //    O.IconType = IconType.Warning;
                //    O.Type = UIInputType.String;
                //    O.Tittle = "RepairCheckOutDoubleCheck";
                //    O.ErrMessage = "No input";
                //    O.UIArea = new string[] { "40%", "70%" };
                //    O.OutInputs.Clear();
                //    O.Message = "Keypart";
                //    O.Name = "Value";
                //    O.CBMessage = "";
                //    O.OutInputs.Add(new DisplayOutPut
                //    {
                //        Name = "PARTNO",
                //        Value = k.PARTNO,
                //        DisplayType = UIOutputType.Text.ToString()
                //    });
                //    O.OutInputs.Add(new DisplayOutPut
                //    {
                //        Name = "KP_NAME",
                //        Value = k.KP_NAME,
                //        DisplayType = UIOutputType.Text.ToString()
                //    });                    
                //    O.OutInputs.Add(new DisplayOutPut
                //    {
                //        Name = "SCANTYPE",
                //        Value = k.SCANTYPE,
                //        DisplayType = UIOutputType.Text.ToString()
                //    });
                //    O.OutInputs.Add(new DisplayOutPut
                //    {
                //        Name = "Location",
                //        Value = k.LOCATION,
                //        DisplayType = UIOutputType.Text.ToString()
                //    });
                //    while (true)
                //    {
                //        var input_sn = O.GetUiInput(this, UIInput.Normal);
                //        if (input_sn == null)
                //        {
                //            O.CBMessage = $@"Please Scan {k.PARTNO}";
                //        }
                //        else
                //        {
                //            string check_sn = input_sn.ToString().Trim();
                //            if (string.IsNullOrEmpty(check_sn))
                //            {
                //                O.CBMessage = $@"Please Scan {k.PARTNO}";
                //            }
                //            else if (!check_sn.Equals(k.VALUE))
                //            {
                //                O.CBMessage = $@"{check_sn} not exists in keypart";
                //            }
                //            else if (check_sn.Equals("No input"))
                //            {
                //                throw new Exception("User Cancel");
                //            }
                //            else
                //            {
                //                check_log = new R_SN_LOG();
                //                check_log.ID = t_r_sn_log.GetNewID(BU, SFCDB);
                //                check_log.SNID = snObj.ID;
                //                check_log.SN = snObj.SN;
                //                check_log.LOGTYPE = "RepairCheckOut";
                //                check_log.DATA1 = "CheckPCBA";
                //                check_log.DATA2 = k.PARTNO;
                //                check_log.DATA3 = check_sn;
                //                check_log.DATA4 = k.KP_NAME;
                //                check_log.DATA5 = k.SCANTYPE;
                //                check_log.DATA6 = k.STATION;
                //                check_log.DATA7 = k.LOCATION;
                //                check_log.FLAG = "1";
                //                check_log.CREATETIME = sysdate;
                //                check_log.CREATEBY = LoginUser.EMP_NO;
                //                SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                //                break;
                //            }
                //        }
                //    }
                //}
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "";
                StationReturn.Data = "OK";
                StationReturn.Message = "OK";

            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = exception.Message;
                StationReturn.Message = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
   
        public void RepairCheckOut(Newtonsoft.Json.Linq.JObject requestValue,Newtonsoft.Json.Linq.JObject Data,MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                if(Data["SN"]==null)
                {
                    throw new Exception("SN is null.");
                }
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string checkOutSn = Data["SN"].ToString().Trim().ToUpper();
                List<R_SN> listSN = SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == checkOutSn && r.VALID_FLAG == "1").ToList();
                if(listSN.Count==0)
                {
                    listSN = SFCDB.ORM.Queryable<R_PANEL_SN, R_SN>((p, s) => p.SN == s.SN)
                        .Where((p, s) => p.PANEL == checkOutSn && s.VALID_FLAG == "1").Select((p, s) => s).ToList();
                }
                if(listSN.Count==0)
                {
                    throw new Exception($@"{checkOutSn} not exist.");
                }
                T_R_REPAIR_TRANSFER rTransfer = new T_R_REPAIR_TRANSFER(SFCDB, DBTYPE);                            

                List<R_REPAIR_TRANSFER> transferList = rTransfer.GetLastRepairedBySN(checkOutSn, SFCDB);
                R_REPAIR_TRANSFER rRepairTransfer = transferList.Where(r => r.CLOSED_FLAG == "0").FirstOrDefault();//TRANSFER表 1 表示不良
                if (rRepairTransfer != null)
                {                    
                    rRepairTransfer.OUT_TIME = SFCDB.ORM.GetDate();
                    rRepairTransfer.OUT_SEND_EMP = LoginUser.EMP_NO;
                    rRepairTransfer.OUT_RECEIVE_EMP = "WAIT_CHECKOUT2";

                    int result = SFCDB.ORM.Updateable<R_REPAIR_TRANSFER>(rRepairTransfer).Where(r => r.ID == rRepairTransfer.ID).ExecuteCommand();
                    if (result < 0)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "REPAIR TRANSFER" }));                        
                    }
                    //foreach (R_SN snObj in listSN)
                    //{
                        
                        //rowSN.REPAIR_FAILED_FLAG = "0";
                        //strRet = (Station.SFCDB).ExecSQL(rowSN.GetUpdateString(DB_TYPE_ENUM.Oracle));
                        //if (Convert.ToInt32(strRet) > 0)
                        //{
                        //    Station.AddMessage("MES00000035", new string[] { strRet }, StationMessageState.Pass);
                        //}
                        //else
                        //{
                        //    Station.AddMessage("MES00000025", new string[] { "R_SN" }, StationMessageState.Pass);
                        //}
                    //}
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000066", new string[] { checkOutSn, "abnormal" }));
                }

            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "";
                StationReturn.Data = ex.Message;
                StationReturn.Message = ex.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
