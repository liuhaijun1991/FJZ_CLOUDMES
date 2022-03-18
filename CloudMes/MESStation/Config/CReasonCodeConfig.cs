using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class CReasonCodeConfig : MesAPIBase
    {
        protected APIInfo _AddReasonCode = new APIInfo()
        {
            FunctionName = "AddReasonCode",
            Description = "",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "REASON_CODE" },
                new APIInputInfo() { InputName = "ENGLISH_DESC" },
                new APIInputInfo() { InputName = "CHINESE_DESC" }
            }
        };
        protected APIInfo _DeleteById = new APIInfo()
        {
            FunctionName = "DeleteById",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "ID" }
            }
        };
        protected APIInfo _UpdateReasonCode = new APIInfo()
        {
            FunctionName = "UpdateReasonCode",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "ID" },
                new APIInputInfo() { InputName = "REASON_CODE" },
                new APIInputInfo() { InputName = "ENGLISH_DESC" },
                new APIInputInfo() { InputName = "CHINESE_DESC" }
            }
        };
        protected APIInfo _QueryByReasonCode = new APIInfo()
        {
            FunctionName = "QueryByReasonCode",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "ReasonCode" }
            }
        };

        protected APIInfo _QueryAll = new APIInfo()
        {
            FunctionName = "QueryAll",
            Description = "",
            Parameters = new List<APIInputInfo>() { }
        };

        public CReasonCodeConfig()
        {
            this.Apis.Add(_AddReasonCode.FunctionName, _AddReasonCode);
            this.Apis.Add(_DeleteById.FunctionName, _DeleteById);
            this.Apis.Add(_UpdateReasonCode.FunctionName, _UpdateReasonCode);
            this.Apis.Add(_QueryByReasonCode.FunctionName, _QueryByReasonCode);
            this.Apis.Add(_QueryAll.FunctionName, _QueryAll);
        }

        public void AddReasonCode(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            string reason_code = Data["REASON_CODE"].ToString();
            if (string.IsNullOrEmpty(reason_code))
            {
                StationReturn.MessageCode = "MES00000006";
                StationReturn.Status = StationReturnStatusValue.Fail;
                return;
            }
            string en_desc = Data["ENGLISH_DESC"].ToString();
            string cn_desc = Data["CHINESE_DESC"].ToString();

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            int res = 0;
            T_C_REASON_CODE t_reasonCode = null;
            Row_C_REASON_CODE r_ReasonCode = null;
            try
            {
                t_reasonCode = new T_C_REASON_CODE(sfcdb, DB_TYPE_ENUM.Oracle);

                r_ReasonCode = (Row_C_REASON_CODE)t_reasonCode.NewRow();
                r_ReasonCode.ID = t_reasonCode.GetNewID(this.BU, sfcdb);
                r_ReasonCode.REASON_CODE = reason_code;
                r_ReasonCode.CHINESE_DESCRIPTION = cn_desc;
                r_ReasonCode.ENGLISH_DESCRIPTION = en_desc;
                r_ReasonCode.EDIT_EMP = this.LoginUser.EMP_NAME;
                r_ReasonCode.EDIT_TIME = this.GetDBDateTime();
                string insertSQL = r_ReasonCode.GetInsertString(DB_TYPE_ENUM.Oracle);
                res = sfcdb.ExecSqlNoReturn(insertSQL, null);
                if (res > 0)
                {
                    StationReturn.MessageCode = "MES00000001";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000021";
                    StationReturn.MessagePara = new List<object>() { "Reason Code" };
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }
            
        }

        public void DeleteById(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<Object>() { "Reason Code ID" };
                StationReturn.Data = null;
                return;
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            string[] ids = id.Split(',');
            Row_C_REASON_CODE row_reason_code = (Row_C_REASON_CODE)new T_C_REASON_CODE(sfcdb, DB_TYPE_ENUM.Oracle).NewRow();
            try
            {
                string deleteString = null;
                sfcdb.BeginTrain();
                foreach (string deleteID in ids)
                {
                    deleteString = row_reason_code.GetDeleteString(DB_TYPE_ENUM.Oracle, deleteID);
                    sfcdb.ExecuteNonQuery(deleteString, CommandType.Text, null);
                }
                sfcdb.CommitTrain();
            }
            catch (Exception ex)
            {
                sfcdb.RollbackTrain();
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.MessageCode = "MES00000004";
            StationReturn.Data = "";
            if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
        }

        public void UpdateReasonCode(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            string id = Data["ID"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<Object>() { "Reason Code ID" };
                StationReturn.Data = null;
                return;
            }
            string reason_code = Data["REASON_CODE"].ToString();
            if (string.IsNullOrEmpty(reason_code))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<Object>() { "Reason Code" };
                StationReturn.Data = null;
                return;
            }
            string en_desc = Data["ENGLISH_DESC"].ToString();
            string cn_desc = Data["CHINESE_DESC"].ToString();
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            string updateSQL = null;
            T_C_REASON_CODE t_reason_code = null;
            Row_C_REASON_CODE row_reason_code = null;
            C_REASON_CODE c_reason_code = null;
            try
            {
                t_reason_code = new T_C_REASON_CODE(sfcdb, DB_TYPE_ENUM.Oracle);
                c_reason_code = t_reason_code.GetObjById(sfcdb, id);
                if (c_reason_code == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.Data = "";
                    StationReturn.MessagePara = new List<Object>() { "Object 'C_REASON_CODE'" };
                    if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                row_reason_code = (Row_C_REASON_CODE)t_reason_code.NewRow();
                row_reason_code.ID = c_reason_code.ID;
                row_reason_code.REASON_CODE = reason_code;
                row_reason_code.ENGLISH_DESCRIPTION = en_desc;
                row_reason_code.CHINESE_DESCRIPTION = cn_desc;
                row_reason_code.EDIT_EMP = this.LoginUser.EMP_NO;
                row_reason_code.EDIT_TIME = this.GetDBDateTime();
                updateSQL = row_reason_code.GetUpdateString(DB_TYPE_ENUM.Oracle, c_reason_code.ID);
                sfcdb.ExecuteNonQuery(updateSQL, CommandType.Text, null);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Data = "";
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
                throw ex;
            }
        }

        public void QueryByReasonCode(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            string reasonCode = Data["ReasonCode"].ToString();
            if (string.IsNullOrEmpty(reasonCode))
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000006";
                StationReturn.MessagePara = new List<Object>() { "Reason Code" };
                StationReturn.Data = null;
                return;
            }
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            C_REASON_CODE c_reason_code = new T_C_REASON_CODE(sfcdb, DB_TYPE_ENUM.Oracle).GetObjByReasonCode(sfcdb, reasonCode);
            if (c_reason_code != null)
            {
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = c_reason_code;
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.MessagePara = new List<object>() { "0" };
                StationReturn.Data = c_reason_code;
            }
            
            if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
        }

        public void QueryAll(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            List<C_REASON_CODE> codes = new T_C_REASON_CODE(sfcdb, DB_TYPE_ENUM.Oracle).GetAllReasonCode(sfcdb);
            if (codes != null && codes.Count > 0)
            {
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = codes;
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = codes;
            }
            if (sfcdb != null) this.DBPools["SFCDB"].Return(sfcdb);
        }

        public void GetAllReasonCodeString(JObject requestValue, JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_REASON_CODE TCRC = new T_C_REASON_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<string> ReasonCodeList = TCRC.GetAllReasonCode(sfcdb).Select(t => t.REASON_CODE).ToList();
                StationReturn.Data = ReasonCodeList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }

    }
}
