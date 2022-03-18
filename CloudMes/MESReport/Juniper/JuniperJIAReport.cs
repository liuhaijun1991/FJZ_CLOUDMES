using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace MESReport.Juniper
{
    /// <summary>
    /// 用於Juniper上傳JIA檢驗結果和顯示JIA報表
    /// </summary>
    public class JuniperJIAReport : MesAPIBase
    {
        protected APIInfo FSelectJIA = new APIInfo
        {
            FunctionName = "SelectJIA",
            Description = "Select JIA",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo { InputName="SKUNO", InputType="string" },
                new APIInputInfo { InputName="WO", InputType="string" },
                new APIInputInfo { InputName="SN", InputType="string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FInsertJIA = new APIInfo
        {
            FunctionName = "InsertJIA",
            Description = "Add JIA",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadJIA = new APIInfo()
        {
            FunctionName = "UploadJIA",
            Description = "Upload",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DataList", InputType = "string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FJIAPrivilege = new APIInfo()
        {
            FunctionName = "JIAPrivilege",
            Description = "JIA Upload Privilege",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        public JuniperJIAReport()
        {
            this.Apis.Add(FSelectJIA.FunctionName, FSelectJIA);
            this.Apis.Add(FInsertJIA.FunctionName, FInsertJIA);
            this.Apis.Add(FUploadJIA.FunctionName, FUploadJIA);
            this.Apis.Add(FJIAPrivilege.FunctionName, FJIAPrivilege);
        }

        public void SelectJIA(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            var list = new List<R_SN_STATION_DETAIL>();
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string sn = Data["SN"].ToString();
                string wo = Data["WO"].ToString();
                string skuNo = Data["SKUNO"].ToString();

                if (!string.IsNullOrEmpty(sn) || !string.IsNullOrEmpty(wo) || !string.IsNullOrEmpty(skuNo))
                {
                    list = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                    .Where(t => t.VALID_FLAG == "1" && t.STATION_NAME == "JIA_INSPECTION")
                    .WhereIF(!string.IsNullOrEmpty(sn), t => t.SN == sn)
                    .WhereIF(!string.IsNullOrEmpty(wo), t => t.WORKORDERNO == wo)
                    .WhereIF(!string.IsNullOrEmpty(skuNo), t => t.SKUNO == skuNo)
                    .OrderBy(t => t.SKUNO, OrderByType.Asc)
                    .OrderBy(t => t.WORKORDERNO, OrderByType.Asc)
                    .OrderBy(t => t.SN, OrderByType.Asc)
                    .ToList();
                }
                else
                {
                    list = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.VALID_FLAG == "1" && t.STATION_NAME == "JIA_INSPECTION").OrderBy(t => t.EDIT_TIME, OrderByType.Asc).ToList();
                }

                if (list.Count > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = list;
                }
                else
                {
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void InsertJIA(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            SFCDB = this.DBPools["SFCDB"].Borrow();
                        
            try
            {
                if (Data["SN"] == null)
                {
                    throw new Exception($@"pls input sn!");
                }
                string sn = Data["SN"].ToString();
                var snList = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").ToList();
                if (snList.Count == 0)
                {
                    throw new Exception($@"{sn} not exists in system, pls check!");
                }
                var JIAList = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.VALID_FLAG == "1" && t.STATION_NAME == "JIA_INSPECTION").ToList();
                if (JIAList.Count > 0)
                {
                    throw new Exception($@"{sn} is already upload, pls check!");
                }

                T_R_SN_STATION_DETAIL _tDETAIL = new T_R_SN_STATION_DETAIL(SFCDB, DBTYPE);
                Row_R_SN_STATION_DETAIL _rDetail = (Row_R_SN_STATION_DETAIL)_tDETAIL.NewRow();
                
                _rDetail.ConstructRow(snList[0]);
                _rDetail.ID = _tDETAIL.GetNewID(BU, SFCDB);
                _rDetail.R_SN_ID = snList[0].ID;
                _rDetail.LINE = "Line1";
                _rDetail.CLASS_NAME = _tDETAIL.GetWorkClass(SFCDB);
                _rDetail.DEVICE_NAME = "JIA_INSPECTION";
                _rDetail.STATION_NAME = "JIA_INSPECTION";
                _rDetail.EDIT_EMP = LoginUser.EMP_NO;
                _rDetail.EDIT_TIME = _tDETAIL.GetDBDateTime(SFCDB);
                var result = SFCDB.ExecSQL(_rDetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "add success!";
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";

            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void UploadJIA(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            DateTime SYSDATE = DateTime.Now;
            SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                string dataList = Data["DataList"].ToString();
                Newtonsoft.Json.Linq.JArray dataArray = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(dataList);
                for (int i = 0; i < dataArray.Count; i++)
                {
                    string sn = dataArray[i]["SN"].ToString();
                    var snList = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").ToList();
                    if (snList.Count == 0)
                    {
                        throw new Exception($@"{sn} not exists in system, pls check!");
                    }
                    var JIAList = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.VALID_FLAG == "1" && t.STATION_NAME == "JIA_INSPECTION").ToList();
                    if (JIAList.Count > 0)
                    {
                        throw new Exception($@"{sn} is already upload, pls check!");
                    }
                    T_R_SN_STATION_DETAIL _tDETAIL = new T_R_SN_STATION_DETAIL(SFCDB, DBTYPE);
                    Row_R_SN_STATION_DETAIL _rDetail = (Row_R_SN_STATION_DETAIL)_tDETAIL.NewRow();

                    _rDetail.ConstructRow(snList[0]);
                    _rDetail.ID = _tDETAIL.GetNewID(BU, SFCDB);
                    _rDetail.R_SN_ID = snList[0].ID;
                    _rDetail.LINE = "Line1";
                    _rDetail.CLASS_NAME = _tDETAIL.GetWorkClass(SFCDB);
                    _rDetail.DEVICE_NAME = "JIA_INSPECTION";
                    _rDetail.STATION_NAME = "JIA_INSPECTION";
                    _rDetail.EDIT_EMP = LoginUser.EMP_NO;
                    _rDetail.EDIT_TIME = _tDETAIL.GetDBDateTime(SFCDB);
                    var result = SFCDB.ExecSQL(_rDetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "upload success!";
                SFCDB.CommitTrain();
            }
            catch (Exception e)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void JIAPrivilege(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();

                var uploadPrivilege = SFCDB.ORM.Queryable<C_PRIVILEGE, C_ROLE_PRIVILEGE, C_USER_ROLE, C_USER>((p, rp, ur, u) => p.ID == rp.PRIVILEGE_ID && rp.ROLE_ID == ur.ROLE_ID && ur.USER_ID == u.ID)
                    .Where((p, rp, ur, u) => u.EMP_NO == LoginUser.EMP_NO && p.PRIVILEGE_NAME == "UPLOAD_JIA_INSPECTION" && p.SYSTEM_NAME == "MES" && p.BASECONFIG_FLAG == "Y")
                    .Select((p, rp, ur, u) => p).Any();

                if (uploadPrivilege)
                {                    
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;                    
                }
                StationReturn.Message = "";
                StationReturn.Data = "";
            }
            catch (Exception e)
            {
                StationReturn.Message = e.Message;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
