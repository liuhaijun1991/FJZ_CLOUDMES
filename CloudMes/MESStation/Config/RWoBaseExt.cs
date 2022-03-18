using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    class RWoBaseExt : MesAPIBase
    {
        protected APIInfo FSelectR_wo_base_ext = new APIInfo()
        {
            FunctionName = "SelectCSkuDetail",
            Description = "查詢CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FAddRWoBaseExt = new APIInfo()
        {
            FunctionName = "AddRWoBaseExt",
            Description = "查詢CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "SEQ_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FDeleteBaseExt = new APIInfo()
        {
            FunctionName = "DeleteBaseExt",
            Description = "查詢CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "SEQ_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUdateWoBaseExt = new APIInfo()
        {
            FunctionName = "UdateWoBaseExt",
            Description = "查詢CSkuDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() {InputName = "SEQ_NO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public RWoBaseExt()
        {
            this.Apis.Add(FSelectR_wo_base_ext.FunctionName, FSelectR_wo_base_ext);
            this.Apis.Add(FAddRWoBaseExt.FunctionName, FAddRWoBaseExt);
            this.Apis.Add(FUdateWoBaseExt.FunctionName, FUdateWoBaseExt);
        }
        public void SelectR_wo_base_ext(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_BASE_EX r_wo_ext = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                r_wo_ext = new T_R_WO_BASE_EX(sfcdb, DB_TYPE_ENUM.Oracle);
                List<R_WO_BASE_EX> list = r_wo_ext.Getdata((Data["WO"].ToString()).Trim(), sfcdb);
                StationReturn.Message = "获取成功！！";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = list;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void AddRWoBaseExt(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_BASE_EX r_wo_ext = null;
            OleExec sfcdb = null;
            try
            {
                string wo= Data["WO"].ToString().Trim();
                string seq_no = Data["SEQ_NO"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                r_wo_ext = new T_R_WO_BASE_EX(sfcdb, DB_TYPE_ENUM.Oracle);
                string inser = r_wo_ext.InsertWoBaseEx(wo,seq_no,sfcdb);
                if (inser =="OK")
                {
                    StationReturn.Message = "Success！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MSGCODE20220107170331";
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
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DeleteBaseExt(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_BASE_EX r_wo_ext = null;
            OleExec sfcdb = null;
            try
            {
                string wo = Data["WO"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                r_wo_ext = new T_R_WO_BASE_EX(sfcdb, DB_TYPE_ENUM.Oracle);
                int inser = r_wo_ext.DeleteWoBaseEx(wo, sfcdb);
                if (inser == 1)
                {
                    StationReturn.Message = "Success！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MSGCODE20220107170331";
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
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void UdateWoBaseExt(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_WO_BASE_EX r_wo_ext = null;
            OleExec sfcdb = null;
            try
            {
                string wo = Data["WO"].ToString().Trim();
                string seq_no = Data["seq_no"].ToString().Trim();
                sfcdb = this.DBPools["SFCDB"].Borrow();
                r_wo_ext = new T_R_WO_BASE_EX(sfcdb, DB_TYPE_ENUM.Oracle);
                int update = r_wo_ext.UpdateWoBaseEx(wo,seq_no, sfcdb);
                if (update == 1)
                {
                    StationReturn.Message = "Success！！";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
    }
}
