using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.Config
{
    class FaiByPass : MesAPIBase
    {
        protected APIInfo FSelectFaiByPass = new APIInfo()
        {
            FunctionName = "SelectFaiByPass",
            Description = "查詢FaiByPass",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Remark", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUpdateFaiByPass = new APIInfo()
        {
            FunctionName = "UpdateFaiByPass",
            Description = "更新FaiByPass",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Remark", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };


        public FaiByPass()
        {
            this.Apis.Add(FSelectFaiByPass.FunctionName, FSelectFaiByPass);
            this.Apis.Add(FUpdateFaiByPass.FunctionName, FUpdateFaiByPass);
        }

        public void SelectFaiByPass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_R_FAI RFAI = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                RFAI = new T_R_FAI(sfcdb, DB_TYPE_ENUM.Oracle);
                String wo = Data["WO"].ToString().Trim();
                List<FAIList> list = RFAI.GetSample(wo, sfcdb);

                if (list.Count > 0)
                {
                    //StationReturn.Message = "获取成功！！";
                    StationReturn.MessageCode = "MES00000026"; 
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    //StationReturn.MessageCode = "获取失败！！";
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara = new List<object>() { "获取失败！！" };
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

        public void UpdateFaiByPass(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_R_FAI RFAI = new T_R_FAI(sfcdb, DB_TYPE_ENUM.Oracle);
                int result;

                string emp_no = LoginUser.EMP_NO;
                DateTime dt = GetDBDateTime();
                string wo = Data["WORKORDERNO"].ToString();
                string SN = Data["SN"].ToString();
                string Remark = Data["REMARK"].ToString();
                var Haveclosedornot = sfcdb.ORM.Queryable<R_FAI>().Any(r => r.WORKORDERNO == wo && r.STATUS == "'0'");
                if (!Haveclosedornot)
                {
                    result = RFAI.UpdateFAIToWo(wo, SN, Remark, emp_no, sfcdb);
                    sfcdb.ORM.Updateable<R_SN_LOCK>().UpdateColumns(r => new R_SN_LOCK { LOCK_STATUS = "0" }).Where(r => r.WORKORDERNO == wo && r.LOCK_STATUS=="1").ExecuteCommand();

                    if (result == 3)
                    {

                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = "更新成功！！";
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        switch (result)
                        {
                            case 0:
                                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SN });
                                break;
                            case 1:
                                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SN });
                                break;
                            case 2:
                                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBTYPE.ToString() });
                                break;
                        }
                    }
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
    }
}
