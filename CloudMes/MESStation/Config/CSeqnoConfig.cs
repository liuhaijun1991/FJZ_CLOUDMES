using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    class CSeqnoConfig : MesAPIBase
    {
        protected APIInfo FAddCSeqno = new APIInfo()
        {
            FunctionName = "AddCSeqno",
            Description = "添加CSeqno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SEQ_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SEQ_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DIGITS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BASE_CODE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MINIMUM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAXIMUM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PREFIX", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FIXED", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SEQ_FORM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RESET", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteCSeqno = new APIInfo()
        {
            FunctionName = "DeleteCSeqno",
            Description = "刪除CSeqno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateCSeqno = new APIInfo()
        {
            FunctionName = "UpdateCSeqno",
            Description = "更新CSeqno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SEQ_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SEQ_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DIGITS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "BASE_CODE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MINIMUM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MAXIMUM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PREFIX", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "FIXED", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SEQ_FORM", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RESET", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelecteCSeqno = new APIInfo()
        {
            FunctionName = "SelecteCSeqno",
            Description = "刪除CSeqno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SEQ_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SEQ_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CSeqnoConfig()
        {
            this.Apis.Add(FAddCSeqno.FunctionName, FAddCSeqno);
            this.Apis.Add(FDeleteCSeqno.FunctionName, FDeleteCSeqno);
            //this.Apis.Add(FDeleteCSeqno.FunctionName, FDeleteCSeqno);
            this.Apis.Add(FSelecteCSeqno.FunctionName, FSelecteCSeqno);
        }

        public void AddCSeqno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SEQNO SEQNO = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SEQNO = new T_C_SEQNO(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SEQNO r = (Row_C_SEQNO)SEQNO.NewRow();
                r.ID = SEQNO.GetNewID(this.BU, sfcdb);
                r.SEQ_NAME= (Data["SEQ_NAME"].ToString()).Trim();
                r.SEQ_NO= (Data["SEQ_NO"].ToString()).Trim();
                r.DIGITS = (Data["DIGITS"].ToString()).Trim() == "" ? 0 : (Convert.ToDouble((Data["DIGITS"].ToString()).Trim()));
                r.BASE_CODE= (Data["BASE_CODE"].ToString()).Trim();
                r.MINIMUM= (Data["MINIMUM"].ToString()).Trim();
                r.MAXIMUM= (Data["MAXIMUM"].ToString()).Trim();
                r.PREFIX= (Data["PREFIX"].ToString()).Trim();
                r.SEQ_FORM= (Data["SEQ_FORM"].ToString()).Trim();
                r.RESET= (Data["RESET"].ToString()).Trim();
                r.USE_TIME= GetDBDateTime();
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "添加成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteCSeqno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SEQNO SEQNO = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SEQNO = new T_C_SEQNO(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SEQNO r = (Row_C_SEQNO)SEQNO.GetObjByID((Data["ID"].ToString()).Trim(), sfcdb);
                string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "刪除成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void UpdateCSeqno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SEQNO SEQNO = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SEQNO = new T_C_SEQNO(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SEQNO r = (Row_C_SEQNO)SEQNO.GetObjByID((Data["ID"].ToString()).Trim(), sfcdb);
                r.SEQ_NAME = (Data["SEQ_NAME"].ToString()).Trim();
                r.SEQ_NO = (Data["SEQ_NO"].ToString()).Trim();
                r.DIGITS = (Data["DIGITS"].ToString()).Trim() == "" ? 0 : (Convert.ToDouble((Data["DIGITS"].ToString()).Trim()));
                r.BASE_CODE = (Data["BASE_CODE"].ToString()).Trim();
                r.MINIMUM = (Data["MINIMUM"].ToString()).Trim();
                r.MAXIMUM = (Data["MAXIMUM"].ToString()).Trim();
                r.PREFIX = (Data["PREFIX"].ToString()).Trim();
                r.SEQ_FORM = (Data["SEQ_FORM"].ToString()).Trim();
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = GetDBDateTime();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Message = "修改成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void SelecteCSeqno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SEQNO SEQNO = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                SEQNO = new T_C_SEQNO(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SEQNO> list = SEQNO.GetSeq((Data["SEQ_NAME"].ToString()).Trim(), (Data["SEQ_NO"].ToString()).Trim(), sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Message = "获取成功！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
    }
}
