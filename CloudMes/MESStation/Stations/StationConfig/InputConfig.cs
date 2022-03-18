using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Stations.StationConfig
{
    public class InputConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo addinput = new APIInfo()
        {
            FunctionName = "AddInput",
            Description = "添加輸入項",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="InputName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DisplayType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DataSourceApi",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DataSourceApiPara",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="RefreshType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo updateinput = new APIInfo()
        {
            FunctionName = "UpdateInput",
            Description = "修改輸入項",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="InputName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DisplayType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DataSourceApi",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DataSourceApiPara",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="RefreshType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""}

            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo deleteinput = new APIInfo()
        {
            FunctionName = "DeleteInput",
            Description = "刪除輸入項",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo queryinput = new APIInfo()
        {
            FunctionName = "QueryInput",
            Description = "查詢輸入項",
            Parameters = new List<APIInputInfo>()
            {
                  new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="WO",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DisplayType",InputType="string",DefaultValue=""}

            },
            Permissions = new List<MESPermission>()
            { }

        };
        public InputConfig()
        {
            this.Apis.Add(addinput.FunctionName, addinput);
            this.Apis.Add(updateinput.FunctionName, updateinput);
            this.Apis.Add(deleteinput.FunctionName, deleteinput);
            this.Apis.Add(queryinput.FunctionName, queryinput);
        }
        public void AddInput(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_C_Input input;
            string InputName = Data["InputName"].ToString().Trim();
            string DisplayType = Data["DisplayType"].ToString().Trim();
            string DataSourceApi = Data["DataSourceApi"].ToString().Trim();
            string DataSourceApiPara = Data["DataSourceApiPara"].ToString();
            string RefreshType = Data["RefreshType"].ToString();
            string Desc = Data["Desc"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                input = new T_C_Input(sfcdb, DBTYPE);
                if (input.CheckDataExist(InputName, sfcdb))
                {
                    Row_C_Input row = (Row_C_Input)input.NewRow();
                    row.ID = input.GetNewID(BU, sfcdb);
                    row.INPUT_NAME = InputName;
                    row.DISPLAY_TYPE = DisplayType;
                    row.DATA_SOURCE_API = DataSourceApi;
                    row.DATA_SOURCE_API_PARA = DataSourceApiPara;
                    row.REFRESH_TYPE = RefreshType;
                    row.DESCRIPTION = Desc;
                    row.EDIT_EMP = LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime();
                    InsertSql = row.GetInsertString(DBTYPE);
                    sfcdb.ExecSQL(InsertSql);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = row.ID;
                    StationReturn.MessageCode = "MES00000002";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }

        public void DeleteInput(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string StrID = "";
            T_C_Input input;
            //   string[] ID = Newtonsoft.Json.Linq.JArray(Data["ID"].);
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                input = new T_C_Input(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    StrID = ID[i].ToString();
                    Row_C_Input row = (Row_C_Input)input.GetObjByID(StrID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
                sfcdb.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        /// <summary>
        /// 更新標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateInput(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_C_Input input;
            string ID = Data["ID"].ToString().Trim();
            string InputName = Data["InputName"].ToString().Trim();
            string DisplayType = Data["DisplayType"].ToString().Trim();
            string DataSourceApi = Data["DataSourceApi"].ToString().Trim();
            string DataSourceApiPara = Data["DataSourceApiPara"].ToString();
            string RefreshType = Data["RefreshType"].ToString();
            string Desc = Data["Desc"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                input = new T_C_Input(sfcdb, DBTYPE);
                Row_C_Input row = (Row_C_Input)input.GetObjByID(ID, sfcdb);
                row.ID = ID;
                row.INPUT_NAME = InputName;
                row.DISPLAY_TYPE = DisplayType;
                row.DATA_SOURCE_API = DataSourceApi;
                row.DATA_SOURCE_API_PARA = DataSourceApiPara;
                row.REFRESH_TYPE = RefreshType;
                row.DESCRIPTION = Desc;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.EDIT_TIME = GetDBDateTime();

                UpdateSql = row.GetUpdateString(DBTYPE);
                sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        /// <summary>
        /// 查詢標簽語言
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void QueryInput(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_Input input;
            List<C_Input> InputList;
            string ID = Data["ID"].ToString().Trim();
            string WO = Data["WO"].ToString().Trim();
            string DisplayType = Data["DisplayType"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                input = new T_C_Input(sfcdb, DBTYPE);
                InputList = input.QueryInput(ID,WO, DisplayType, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = InputList;
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
