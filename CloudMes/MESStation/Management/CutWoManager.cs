using MESDBHelper;
using MESPubLab.MESStation;
using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESStation.Management
{
    public class CutWoManager : MesAPIBase
    {
        protected APIInfo FGetWoInfo = new APIInfo
        {
            FunctionName = "GetWoInfo",
            Description = "Get wo base info",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "WO" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetSNDetailByWo = new APIInfo
        {
            FunctionName = "GetSNDetailByWo",
            Description = "Get sn detail by wo",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "WO" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FCutWoByNum = new APIInfo
        {
            FunctionName = "CutWoByNum",
            Description = "Cut wo by num",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "WO" },
                new APIInputInfo() { InputName = "NUM" },
                new APIInputInfo() { InputName = "CLOSEWO" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FCutWoBySNId = new APIInfo
        {
            FunctionName = "CutWoBySNId",
            Description = "Cut wo by sn id",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "WO" },
                new APIInputInfo() { InputName = "ID" },
                new APIInputInfo() { InputName = "CLOSEWO" }
            },
            Permissions = new List<MESPermission>()
        };

        public CutWoManager() {
            this.Apis.Add(FGetWoInfo.FunctionName, FGetWoInfo);
            this.Apis.Add(FGetSNDetailByWo.FunctionName,FGetSNDetailByWo);
            this.Apis.Add(FCutWoByNum.FunctionName,FCutWoByNum);
            this.Apis.Add(FCutWoBySNId.FunctionName,FCutWoBySNId);
        }

        public void GetWoInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, DBTYPE);
                R_WO_BASE r_wo_base = t_r_wo_base.GetWoByWoNo(Data["WO"].ToString().ToUpper().Trim(), sfcdb);
                if (r_wo_base == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { Data["WO"].ToString().ToUpper().Trim() }));
                }
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = r_wo_base;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetSNDetailByWo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, DBTYPE);
                R_WO_BASE r_wo_base = t_r_wo_base.GetWoByWoNo(Data["WO"].ToString().ToUpper().Trim(), sfcdb);
                if (r_wo_base == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { Data["WO"].ToString().ToUpper().Trim() }));
                }
                T_R_SN t_r_sn = new T_R_SN(sfcdb, DBTYPE);
                List<R_SN> snList = t_r_sn.GetRSNbyWo(Data["WO"].ToString().ToUpper().Trim(), sfcdb).FindAll(sn => sn.CURRENT_STATION.IndexOf("LOADING") >= 0);
                if (snList.Count == 0) {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731133844", new string[] { Data["WO"].ToString().ToUpper().Trim() }));
                }
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = snList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void CutWoByNum(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string wo = Data["WO"].ToString().ToUpper().Trim();
            string closeFlag= Data["CLOSEFLAG"].ToString().ToUpper().Trim();
            string remark = Data["REMARK"] == null ? "" : Data["REMARK"].ToString().Trim();
            double num = 0;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, DBTYPE);
                R_WO_BASE r_wo_base = t_r_wo_base.GetWoByWoNo(wo, sfcdb);
                if (r_wo_base == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { wo }));
                }
                if (string.IsNullOrEmpty(remark) && BU=="VERTIV")
                {
                    throw new Exception("Please Input Remark!");
                }

                try
                {
                    num = Convert.ToDouble(Data["NUM"].ToString().ToUpper().Trim());
                }
                catch
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731151259", new string[] { Data["NUM"].ToString().ToUpper().Trim() }));
                }
                if (num <= 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731151259", new string[] { Data["NUM"].ToString().ToUpper().Trim() }));
                }

                if (r_wo_base.WORKORDER_QTY - r_wo_base.INPUT_QTY < num)
                {                   
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000208", new string[] { wo, num.ToString(), (r_wo_base.WORKORDER_QTY - r_wo_base.INPUT_QTY).ToString() }));
                }

                Row_R_WO_BASE rowWoBase = (Row_R_WO_BASE)t_r_wo_base.GetObjByID(r_wo_base.ID,sfcdb);
                rowWoBase.WORKORDER_QTY = r_wo_base.WORKORDER_QTY - Convert.ToDouble(num);
                if (closeFlag == "1")
                {
                    rowWoBase.CLOSED_FLAG = closeFlag;
                    rowWoBase.CLOSE_DATE = GetDBDateTime();
                }
                rowWoBase.EDIT_EMP = LoginUser.EMP_NO;
                rowWoBase.EDIT_TIME = GetDBDateTime();
                sfcdb.ExecSQL(rowWoBase.GetUpdateString(DBTYPE));

                T_R_MES_LOG TRML = new T_R_MES_LOG(sfcdb, DBTYPE);
                R_MES_LOG log = new R_MES_LOG();
                log.ID = TRML.GetNewID(BU, sfcdb);
                log.PROGRAM_NAME = "MESStation";
                log.CLASS_NAME = "Management.CutWoManager";
                log.FUNCTION_NAME = "CutWoByNum";
                log.LOG_MESSAGE = remark.Length > 450 ? remark.Substring(0, 400) : remark;
                log.EDIT_EMP= LoginUser.EMP_NO;
                log.EDIT_TIME = GetDBDateTime();
                log.DATA1 = rowWoBase.WORKORDERNO;
                log.DATA2 = num.ToString();
                TRML.InsertMESLog(log, sfcdb);


                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = r_wo_base;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000210";
                StationReturn.MessagePara.Add(wo);
                StationReturn.MessagePara.Add(num);
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void CutWoBySNId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {    
            OleExec sfcdb = null;
            try
            {
                string wo = Data["WO"].ToString().ToUpper().Trim();
                string closeFlag = Data["CLOSEFLAG"].ToString().ToUpper().Trim();
                string remark = Data["REMARK"] == null ? "" : Data["REMARK"].ToString().Trim();
                Newtonsoft.Json.Linq.JArray arrayId = (Newtonsoft.Json.Linq.JArray)Data["ID"];

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.ThrowSqlExeception = true;
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, DBTYPE);
                R_WO_BASE r_wo_base = t_r_wo_base.GetWoByWoNo(wo, sfcdb);
                if (r_wo_base == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { wo }));
                }
                if (string.IsNullOrEmpty(remark) && BU == "VERTIV")
                {
                    throw new Exception("Please Input Remark!");
                }

                T_R_SN t_r_sn = new T_R_SN(sfcdb,DBTYPE);
                Row_R_SN rowSN;
                for (int i = 0; i < arrayId.Count; i++) {
                    rowSN =(Row_R_SN)t_r_sn.GetObjByID(arrayId[i].ToString(), sfcdb);
                    rowSN.SN = "CUT_" + rowSN.SN;
                    rowSN.WORKORDERNO = "CUT_" + rowSN.WORKORDERNO;
                    rowSN.SKUNO = "CUT_" + rowSN.SKUNO;
                    rowSN.VALID_FLAG = "0";
                    rowSN.EDIT_EMP = LoginUser.EMP_NO;
                    rowSN.EDIT_TIME = GetDBDateTime();
                    sfcdb.ExecSQL(rowSN.GetUpdateString(DBTYPE));
                }                

                Row_R_WO_BASE rowWoBase = (Row_R_WO_BASE)t_r_wo_base.GetObjByID(r_wo_base.ID, sfcdb);
                rowWoBase.WORKORDER_QTY = r_wo_base.WORKORDER_QTY - Convert.ToDouble(arrayId.Count);
                rowWoBase.INPUT_QTY = r_wo_base.INPUT_QTY - Convert.ToDouble(arrayId.Count);
                rowWoBase.EDIT_EMP = LoginUser.EMP_NO;
                rowWoBase.EDIT_TIME = GetDBDateTime();
                if (closeFlag == "1")
                {
                    rowWoBase.CLOSED_FLAG = closeFlag;
                    rowWoBase.CLOSE_DATE = GetDBDateTime();
                }
                sfcdb.ExecSQL(rowWoBase.GetUpdateString(DBTYPE));

                T_R_MES_LOG TRML = new T_R_MES_LOG(sfcdb, DBTYPE);
                R_MES_LOG log = new R_MES_LOG();
                log.ID = TRML.GetNewID(BU, sfcdb);
                log.PROGRAM_NAME = "MESStation";
                log.CLASS_NAME = "Management.CutWoManager";
                log.FUNCTION_NAME = "CutWoByNum";
                log.LOG_MESSAGE = remark.Length > 450 ? remark.Substring(0, 400) : remark;
                log.EDIT_EMP = LoginUser.EMP_NO;
                log.EDIT_TIME = GetDBDateTime();
                log.DATA1 = rowWoBase.WORKORDERNO;
                log.DATA2 = arrayId.Count.ToString();
                TRML.InsertMESLog(log, sfcdb);

                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = r_wo_base;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000210";
                StationReturn.MessagePara.Add(wo);
                StationReturn.MessagePara.Add(arrayId.Count);
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void GetWOCutLog(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string wo = Data["WO"] == null ? "" : Data["WO"].ToString().Trim();
                List<R_MES_LOG> logList = new List<R_MES_LOG>();
                logList = sfcdb.ORM.Queryable<R_MES_LOG>().Where(r => r.PROGRAM_NAME == "MESStation" && r.CLASS_NAME == "Management.CutWoManager"
                 && r.FUNCTION_NAME == "CutWoByNum" && r.DATA1 == wo).ToList();               
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Data = logList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }
}
