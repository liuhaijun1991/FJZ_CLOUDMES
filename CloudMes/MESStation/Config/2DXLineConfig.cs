using MESDBHelper;
using MESPubLab.MESStation;
using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESStation.Config
{
    public class _2DXLineConfig : MesAPIBase
    {
        protected APIInfo FGet2DXLINE = new APIInfo()
        {
            FunctionName = "Get2DXLINE",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGet2DXLINEFORBUILD = new APIInfo()
        {
            FunctionName = "Get2DXLileForBuild",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {
                   new APIInputInfo() {InputName = "BUILD", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetCustomer = new APIInfo()
        {
            FunctionName = "GetCustomer",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddPassRecord = new APIInfo()
        {
            FunctionName = "addPassRecord",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {

                new APIInputInfo() {InputName = "User", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Skuno", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Customer", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Build", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Line", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Sn", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Workorderno", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Location", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Void", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Status", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Norm", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Alignmet", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Other", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "Remark", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "NextStation", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public _2DXLineConfig()
        {
            this.Apis.Add(FGet2DXLINE.FunctionName, FGet2DXLINE);
            this.Apis.Add(FGet2DXLINEFORBUILD.FunctionName,FGet2DXLINEFORBUILD);
            this.Apis.Add(FGetCustomer.FunctionName, FGetCustomer);
            this.Apis.Add(FAddPassRecord.FunctionName, FAddPassRecord);
        }
        public void Get2DXLINE(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                var ss = db.ORM.Queryable<C_2DXRAYLINE>().GroupBy(t => t.BUILDING).Select(t => t.BUILDING).With(SqlSugar.SqlWith.NoLock).ToList();
                StationReturn.Data = ss;
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
            }
        }
        public void Get2DXLINEForBuild(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            var build = Data["BUILD"].ToString().Trim();
            try
            {
                var ss = db.ORM.Queryable<C_2DXRAYLINE>().Where(t => t.BUILDING == build).Select(t => t.LINENAME).ToList();
                StationReturn.Data = ss;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
            }
        }
        public void GetCustomer(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            var sn = Data["SN"].ToString().Trim();
            try
            {
                var s = db.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (s!=null)
                {
                    var ss = db.ORM.Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER>((SN, SKU, CS, CCS) => SN.SKUNO == SKU.SKUNO && SKU.C_SERIES_ID == CS.ID && CS.CUSTOMER_ID == CCS.ID)
                        .Where((SN, SKU, CS, CCS) => SN.SN == sn && SN.VALID_FLAG =="1").Select((SN, SKU, CS, CCS) => new { SN.SKUNO, CCS.DESCRIPTION,SN.WORKORDERNO,SN.NEXT_STATION }).ToList().FirstOrDefault();
                    if (ss != null)
                    {
                        StationReturn.Data = ss;
                        StationReturn.MessageCode = "MES00000026"; 
                        StationReturn.Status = StationReturnStatusValue.Pass;
                    }
                    else
                    {
                        
                        StationReturn.Data = ss;
                        StationReturn.MessageCode = "MSGCODE20210814102404";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }
                }
                else {

                    StationReturn.Data = null;
                    StationReturn.Message= MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] {  sn } );
                    StationReturn.Status = "NOTEXISTS";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
            }
        }

        public void AddPassRecord(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            var user = Data["User"].ToString().Trim().ToUpper();
            var skuno = Data["Skuno"].ToString().Trim().ToUpper();
            var customer = Data["Customer"].ToString().Trim().ToUpper();
            var build = Data["Build"].ToString().Trim().ToUpper();
            var line = Data["Line"].ToString().Trim().ToUpper();
            var sn = Data["Sn"].ToString().Trim().ToUpper();
            var wo = Data["Workorderno"].ToString().Trim().ToUpper();
            var location = Data["Location"].ToString().Trim().ToUpper();
            var invoid = Data["Void"].ToString().Trim().ToUpper();
            var status = Data["Status"].ToString().Trim().ToUpper();
            var norm = Data["Norm"].ToString().Trim();
            var alignmet = Data["Alignmet"].ToString().Trim();
            var other = Data["Other"].ToString().Trim();
            var remark = Data["Remark"].ToString().Trim()??"0";
            var nextStation = Data["NextStation"].ToString().Trim();
            T_R_2DXRAY r_2DXRAY = null;
            string strRet = "";
            try
            {
                r_2DXRAY = new T_R_2DXRAY(db, DB_TYPE_ENUM.Oracle);
                Row_R_2DXRAY r = (Row_R_2DXRAY)r_2DXRAY.NewRow();
                r.ID = r_2DXRAY.GetNewID(BU, db);
                r.SN = sn;
                r.SKUNO = skuno;
                r.WORKORDERNO = wo;
                r.BGA_LOCATION = location;
                r.STATUS = status;
                r.MISALIGNMENT = alignmet;
                r.VOID = invoid;
                r.ISSHORT = norm;
                r.OTHER = other;
                r.REMARK1 = nextStation;
                r.REMARK =remark;
                r.REMARK2 ="" ;
                r.REMARK3 = "";
                r.CUSTOMER = customer;
                r.LINENAME = line;
                r.EDIT_EMP = this.LoginUser.EMP_NO;
                r.EDIT_TIME = r_2DXRAY.GetDBDateTime(db);
                 strRet = db.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = strRet;
                //throw e;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
            }
        }
    }
}
