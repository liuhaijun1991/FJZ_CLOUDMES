using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Stations.StationConfig
{
    public class InputActionConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo addinputaction = new APIInfo()
        {
            FunctionName = "AddInputAction",
            Description = "添加輸入框",
            Parameters = new List<APIInputInfo>()
            {
               // InputID,StationActionID,SeqNo,ConfigType,ConfigValue,AddFlag
                new APIInputInfo() { InputName="InputID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="StationActionID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="SeqNo",InputType="int",DefaultValue=""},
                new APIInputInfo() { InputName="ConfigType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ConfigValue",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="AddFlag",InputType="int",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo updateinputaction = new APIInfo()
        {
            FunctionName = "UpdateInputAction",
            Description = "修改輸入項",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="InputID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="StationActionID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="SeqNo",InputType="int",DefaultValue=""},
                new APIInputInfo() { InputName="ConfigType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="ConfigValue",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="AddFlag",InputType="int",DefaultValue=""}

            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo deleteinputaction = new APIInfo()
        {
            FunctionName = "DeleteInputAction",
            Description = "刪除輸入項",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo queryinputaction = new APIInfo()
        {
            FunctionName = "QueryInputAction",
            Description = "查詢輸入項",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                 new APIInputInfo() { InputName="InputID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="StationActionID",InputType="string",DefaultValue=""},

            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo deleteByInputID = new APIInfo()
        {
            FunctionName = "DeleteInputActionByInputID",
            Description = "根據INPUT ID刪除",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="InputID",InputType="string",DefaultValue=""},
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo querybyinputid = new APIInfo()
        {
            FunctionName = "QueryInputActionByinputID",
            Description = "根據INPUT ID查詢",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="InputID",InputType="string",DefaultValue=""},

            },
            Permissions = new List<MESPermission>()
            { }

        };
        public InputActionConfig()
        {
            this.Apis.Add(addinputaction.FunctionName, addinputaction);
            this.Apis.Add(updateinputaction.FunctionName, updateinputaction);
            this.Apis.Add(deleteinputaction.FunctionName, deleteinputaction);
            this.Apis.Add(queryinputaction.FunctionName, queryinputaction);
            this.Apis.Add(deleteByInputID.FunctionName, deleteByInputID);
            this.Apis.Add(querybyinputid.FunctionName, querybyinputid);
        }
        public void AddInputAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_R_Input_Action inputaction;
            string InputID = Data["InputID"].ToString().Trim();
            string StationActionID = Data["StationActionID"].ToString().Trim();
            int SeqNo = Convert.ToInt32(Data["SeqNo"]);
            string ConfigType = Data["ConfigType"].ToString();
            string ConfigValue = Data["ConfigValue"].ToString();
            int AddFlag = Convert.ToInt32(Data["AddFlag"]);
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                inputaction = new T_R_Input_Action(sfcdb, DBTYPE);
                if (inputaction.CheckDataExist(InputID, StationActionID, sfcdb))
                {
                    Row_R_Input_Action row = (Row_R_Input_Action)inputaction.NewRow();
                    row.ID = inputaction.GetNewID(BU, sfcdb);
                    row.INPUT_ID = InputID;
                    row.C_STATION_ACTION_ID = StationActionID;
                    row.SEQ_NO = SeqNo;
                    row.CONFIG_TYPE = ConfigType;
                    row.CONFIG_VALUE = ConfigValue;
                    row.ADD_FLAG = AddFlag;
                    row.EDIT_EMP = LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime();
                    InsertSql = row.GetInsertString(DBTYPE);
                    sfcdb.ExecSQL(InsertSql);
                    StationReturn.Status = StationReturnStatusValue.Pass;
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

        //public  void AddInputActionS( Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        //{
        // //   OleExec sfcdb = null;
        //    string InsertSql = "";
        //    T_R_Input_Action inputaction;
        //    string ID = Data["ID"].ToString();
        //    string InputID = Data["InputID"].ToString();
        //    string StationActionID = Data["StationActionID"].ToString();
        //    int SeqNo = Convert.ToInt32(Data["SeqNo"]);
        //    string ConfigType = Data["ConfigType"].ToString();
        //    string ConfigValue = Data["ConfigValue"].ToString();
        //    int AddFlag = Convert.ToInt32(Data["AddFlag"]);
        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        inputaction = new T_R_Input_Action(sfcdb, DBTYPE);
        //        DeleteInputAction(ID, sfcdb);
        //        Row_R_Input_Action row = (Row_R_Input_Action)inputaction.NewRow();
        //        row.ID = inputaction.GetNewID(BU, sfcdb);
        //        row.INPUT_ID = InputID;
        //        row.C_STATION_ACTION_ID = StationActionID;
        //        row.SEQ_NO = SeqNo;
        //        row.CONFIG_TYPE = ConfigType;
        //        row.CONFIG_VALUE = ConfigValue;
        //        row.ADD_FLAG = AddFlag;
        //        row.EDIT_EMP = LoginUser.EMP_NO;
        //        row.EDIT_TIME = GetDBDateTime();
        //        InsertSql = row.GetInsertString(DBTYPE);
        //        sfcdb.ExecSQL(InsertSql);
        //    }
        //    catch (Exception e)
        //    {
        //        //this.DBPools["SFCDB"].Return(sfcdb);
        //        throw e;

        //    }

        //}

        //public void DeleteInputAction(String ID, OleExec sfcdb)
        //{
        //    string DeleteSql = "";
        //    T_R_Input_Action stationaction;
        //    try
        //    {
        //        stationaction = new T_R_Input_Action(sfcdb, DBTYPE);
        //        if (stationaction.CheckDataExistByID(ID, sfcdb))
        //        {
        //            Row_R_Input_Action row = (Row_R_Input_Action)stationaction.GetObjByID(ID, sfcdb);
        //            DeleteSql = row.GetDeleteString(DBTYPE);
        //            sfcdb.ExecSQL(DeleteSql);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //  this.DBPools["SFCDB"].Return(sfcdb);
        //        throw e;
        //    }

        //}

        public void DeleteInputAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string StrID = "";
            T_R_Input_Action inputaction;
            //   string[] ID = Newtonsoft.Json.Linq.JArray(Data["ID"].);
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                inputaction = new T_R_Input_Action(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    StrID = ID[i].ToString();
                    Row_R_Input_Action row = (Row_R_Input_Action)inputaction.GetObjByID(StrID, sfcdb);
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

        public void DeleteInputActionByInputID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string StrID = "";
            T_R_Input_Action inputaction;
            //   string[] ID = Newtonsoft.Json.Linq.JArray(Data["ID"].);
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["InputID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                inputaction = new T_R_Input_Action(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    StrID = ID[i].ToString();
                    inputaction.DeleteInputActionByInputID(StrID,sfcdb);
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

        public void UpdateInputAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_R_Input_Action inputaction;
            string ID = Data["ID"].ToString().Trim();
            string InputID = Data["InputID"].ToString().Trim();
            string StationActionID = Data["StationActionID"].ToString().Trim();
            int SeqNo = Convert.ToInt32(Data["SeqNo"]);
            string ConfigType = Data["ConfigType"].ToString();
            string ConfigValue = Data["ConfigValue"].ToString();
            int AddFlag = Convert.ToInt32(Data["AddFlag"]);
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                inputaction = new T_R_Input_Action(sfcdb, DBTYPE);
                Row_R_Input_Action row = (Row_R_Input_Action)inputaction.GetObjByID(ID, sfcdb);
                row.ID = ID;
                row.INPUT_ID = InputID;
                row.C_STATION_ACTION_ID = StationActionID;
                row.SEQ_NO = SeqNo;
                row.CONFIG_TYPE = ConfigType;
                row.CONFIG_VALUE = ConfigValue;
                row.ADD_FLAG = AddFlag;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.EDIT_TIME = GetDBDateTime();

                UpdateSql = row.GetUpdateString(DBTYPE);
                sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Data = InputID;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
  
        public void QueryInputAction(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_R_Input_Action inputaction;
            List<R_Input_Action> InputActionList;
            string ID = Data["ID"].ToString().Trim();
            string InputID = Data["InputID"].ToString().Trim();
            string StationActionID = Data["StationActionID"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                inputaction = new T_R_Input_Action(sfcdb, DBTYPE);
                InputActionList = inputaction.QueryInput(ID, InputID, StationActionID, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = InputActionList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public void QueryInputActionByinputID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_R_Input_Action inputaction;
            DataTable dt = new DataTable();
         //   List<R_Input_Action> InputActionList;
            object ialist ;
            string InputID = Data["InputID"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                inputaction = new T_R_Input_Action(sfcdb, DBTYPE);
                dt = inputaction.QueryInput(InputID, sfcdb);
                ialist= MESDataObject.Common.ConvertToJson.DataTableToJson(dt);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = ialist;
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
