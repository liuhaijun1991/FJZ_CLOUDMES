using System;
using System.Collections.Generic;
using System.Linq;
using MESPubLab.MESStation;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using SqlSugar;
using System.Data;

namespace MESReport.BaseReport
{
    class Whenshippedconfig : MesAPIBase
    {
        protected APIInfo FSelectWhenshipped = new APIInfo
        {
            FunctionName = "SelectWhenshipped",
            Description = "Select Whenshipped",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo { InputName="DN_NO", InputType="string" },
                new APIInputInfo { InputName="PO_NO", InputType="string" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FInsertWhenShipped = new APIInfo
        {
            FunctionName = "InsertWhenShipped",
            Description = "Add WhenShipped",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DN_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "PO_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SHIP_TO_ADDESS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "REMARK", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUploadWhenshipped = new APIInfo()
        {
            FunctionName = "UploadWhenshipped",
            Description = "Upload Whenshipped",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DataList", InputType = "string" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public Whenshippedconfig()
        {
            this.Apis.Add(FSelectWhenshipped.FunctionName, FSelectWhenshipped);
            this.Apis.Add(FUploadWhenshipped.FunctionName, FUploadWhenshipped);
            this.Apis.Add(FInsertWhenShipped.FunctionName, FInsertWhenShipped);
        }

        public void SelectWhenshipped(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            var listdata = new List<R_WHEN_SHIPPED>();
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                string dn = Data["DN_NO"].ToString();
                string po = Data["PO_NO"].ToString();
                if (!string.IsNullOrEmpty(dn) || !string.IsNullOrEmpty(po))
                {
                    listdata = SFCDB.ORM.Queryable<R_WHEN_SHIPPED>()
                    .WhereIF(!string.IsNullOrEmpty(dn), t => t.DN_NO == dn)
                    .WhereIF(!string.IsNullOrEmpty(po), t => t.PO_NO == po)
                    .OrderBy(t => t.DN_NO, OrderByType.Asc)
                    .OrderBy(t => t.PO_NO, OrderByType.Asc)
                    .ToList();
                }
                else
                {
                    listdata = SFCDB.ORM.Queryable<R_WHEN_SHIPPED>().OrderBy(t => t.UPLOAD_TIME, OrderByType.Desc).ToList();
                }
                if (listdata.Count > 0)
                {
                    StationReturn.Message = "";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = listdata;
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
        public void InsertWhenShipped(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            SFCDB = this.DBPools["SFCDB"].Borrow();

            try
            {
                string dn = Data["DN_NO"].ToString();
                string po = Data["PO_NO"].ToString();
                string ship = Data["SHIP_TO_ADDRESS"].ToString();
                string remark = Data["REMARK"].ToString();

                var dnList = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == dn).ToList();
                var poList = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.PO_NO == po).ToList();
                if (string.IsNullOrEmpty(dn))
                {
                    throw new Exception("Please input DN!");
                }
                if (string.IsNullOrEmpty(po))
                {
                    throw new Exception("Please input PO!");
                }
                if (string.IsNullOrEmpty(ship))
                {
                    throw new Exception("Please input SHIP TO ADDRESS");
                }
                if (dnList.Count == 0)
                {
                    throw new Exception($@"DN: '{dn}' not exists in system please check again!");
                }
                if (poList.Count == 0)
                {
                    throw new Exception($@"PO: '{po}' not exists in system please check again!");
                }
                var listdn = SFCDB.ORM.Queryable<R_WHEN_SHIPPED>().Where(t => t.DN_NO == dn && t.VALID_FLAG == "1").ToList();
                if (listdn.Count > 0)
                {
                    SFCDB.ORM.Updateable<R_WHEN_SHIPPED>().SetColumns(t => t.VALID_FLAG == "0").Where(t => t.DN_NO == dn && t.VALID_FLAG == "1").ExecuteCommand();
                }
                T_R_WHEN_SHIPPED _tDETAIL = new T_R_WHEN_SHIPPED(SFCDB, DBTYPE);
                Row_R_WHEN_SHIPPED _rDetail = (Row_R_WHEN_SHIPPED)_tDETAIL.NewRow();

                _rDetail.ConstructRow(dnList[0]);
                _rDetail.ID = _tDETAIL.GetNewID(BU, SFCDB);
                _rDetail.DN_NO = dnList[0].DN_NO;
                _rDetail.PO_NO = poList[0].PO_NO;
                _rDetail.FILENAME = "";
                _rDetail.SHIP_TO_ADDRESS = ship;
                _rDetail.REMARK = remark;
                _rDetail.VALID_FLAG = "1";
                _rDetail.EDIT_EMP = LoginUser.EMP_NO;
                _rDetail.EDIT_TIME = GetDBDateTime();
                _rDetail.UPLOAD_EMP = LoginUser.EMP_NO;
                _rDetail.UPLOAD_TIME = GetDBDateTime();
                var result = SFCDB.ExecSQL(_rDetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Add success!";
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

        public void UploadWhenshipped(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();

            try
            {
                string dataList = Data["DataList"].ToString();
                Newtonsoft.Json.Linq.JArray dataArray = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(dataList);
                for (int i = 0; i < dataArray.Count; i++)
                {
                    string dn = dataArray[i]["DN_NO"].ToString();
                    string po = dataArray[i]["PO_NO"].ToString();
                    string ship = dataArray[i]["SHIP_TO_ADDRESS"].ToString();
                    ship = $@"{ship.Replace("\r\n", " ")}";
                    string remark = dataArray[i]["REMARK"].ToString();


                    var dnList = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == dn).ToList();
                    var poList = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.PO_NO == po).ToList();
                    if (string.IsNullOrEmpty(ship))
                    {
                        throw new Exception("Ship to address no data, please check your file again!");
                    }
                    if (dnList.Count == 0)
                    {
                        throw new Exception($@"DN: '{dn}' not exists in system please check again!");
                    }
                    if (poList.Count == 0)
                    {
                        throw new Exception($@"PO: '{po}' not exists in system please check again!");
                    }
                    var listdn = SFCDB.ORM.Queryable<R_WHEN_SHIPPED>().Where(t => t.DN_NO == dn && t.VALID_FLAG == "1").ToList();
                    if (listdn.Count > 0)
                    {
                        SFCDB.ORM.Updateable<R_WHEN_SHIPPED>().SetColumns(t => t.VALID_FLAG == "0").Where(t => t.DN_NO == dn && t.VALID_FLAG == "1").ExecuteCommand();
                    }

                    T_R_WHEN_SHIPPED _tDETAIL = new T_R_WHEN_SHIPPED(SFCDB, DBTYPE);
                    Row_R_WHEN_SHIPPED _rDetail = (Row_R_WHEN_SHIPPED)_tDETAIL.NewRow();

                    _rDetail.ConstructRow(dnList[0]);
                    _rDetail.ID = _tDETAIL.GetNewID(BU, SFCDB);
                    _rDetail.DN_NO = dnList[0].DN_NO;
                    _rDetail.PO_NO = poList[0].PO_NO;
                    _rDetail.FILENAME = "";
                    _rDetail.SHIP_TO_ADDRESS = ship;
                    _rDetail.REMARK = remark;
                    _rDetail.VALID_FLAG = "1";
                    _rDetail.EDIT_EMP = LoginUser.EMP_NO;
                    _rDetail.EDIT_TIME = GetDBDateTime();
                    _rDetail.UPLOAD_EMP = LoginUser.EMP_NO;
                    _rDetail.UPLOAD_TIME = GetDBDateTime();
                    var result = SFCDB.ExecSQL(_rDetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Upload success!";
                SFCDB.CommitTrain();
            }
            catch (Exception e)
            {
                SFCDB.RollbackTrain();
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
