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
   public class StationInputConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo addinput = new APIInfo()
        {
            FunctionName = "AddInput",
            Description = "添加輸入框",
            Parameters = new List<APIInputInfo>()
            {
               // InputID,StationActionID,SeqNo,ConfigType,ConfigValue,AddFlag
                new APIInputInfo() { InputName = "InputID",InputType = "string",DefaultValue = ""},
                new APIInputInfo() { InputName = "StationActionID",InputType = "string",DefaultValue = ""},
                new APIInputInfo() { InputName = "SeqNo",InputType = "int",DefaultValue = ""},
                new APIInputInfo() { InputName = "ConfigType",InputType = "string",DefaultValue = ""},
                new APIInputInfo() { InputName = "ConfigValue",InputType = "string",DefaultValue = ""},
                new APIInputInfo() { InputName = "AddFlag",InputType = "int",DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo getid = new APIInfo()
        {
            FunctionName = "GetID",
            Description = "獲取新的ID",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }

        };
        public StationInputConfig()
        {
           this.Apis.Add(getid.FunctionName, getid);
        }
        //public  void AddInput(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        //{
        //    string InsertSql = "";
        //    T_R_Station_Input input;
        //    string ID = Data["ID"].ToString();
        //    string StationID = Data["STATION_ID"].ToString();
        //    string InputID = Data["INPUT_ID"].ToString();
        //    double SeqNo = Convert.ToDouble(Data["SEQ_NO"]);
        //    string Rlinput = Data["REMEMBER_LAST_INPUT"].ToString();
        //    double ScanFlag = Convert.ToDouble(Data["SCAN_FLAG"]);
        //    string DName = Data["DISPLAY_NAME"].ToString();
        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        input = new T_R_Station_Input(sfcdb, DBTYPE);
        //        DeleteStationInput(ID, sfcdb);
        //        Row_R_Station_Input row = (Row_R_Station_Input)input.NewRow();
        //        row.ID = input.GetNewID(BU, sfcdb);
        //        row.STATION_ID = StationID;
        //        row.INPUT_ID = InputID;
        //        row.SEQ_NO = SeqNo;
        //        row.SCAN_FLAG = ScanFlag;
        //        row.DISPLAY_NAME = DName;
        //        row.REMEMBER_LAST_INPUT = Rlinput;
        //        row.EDIT_EMP = LoginUser.EMP_NO;
        //        row.EDIT_TIME = GetDBDateTime();
        //        InsertSql = row.GetInsertString(DBTYPE);
        //        sfcdb.ExecSQL(InsertSql);
        //    }
        //    catch (Exception e)
        //    {
        //        //  this.DBPools["SFCDB"].Return(sfcdb);
        //        throw e;

        //    }

        //}
        //public void DeleteStationInput(String ID, OleExec sfcdb)
        //{
        //    string DeleteSql = "";
        //    T_R_Station_Input stationaction;
        //    try
        //    {
        //        stationaction = new T_R_Station_Input(sfcdb, DBTYPE);
        //        if (stationaction.CheckDataExistByID(ID, sfcdb))//存在就刪除
        //        {
        //            Row_R_Station_Input row = (Row_R_Station_Input)stationaction.GetObjByID(ID, sfcdb);
        //            DeleteSql = row.GetDeleteString(DBTYPE);
        //            sfcdb.ExecSQL(DeleteSql);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }

        //}

        public void GetID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb;
            T_R_Station_Input station;
            string ID = "";
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                station = new T_R_Station_Input(sfcdb, DBTYPE);
                ID = station.GetNewID(BU, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ID;
                StationReturn.MessageCode = "MES00000001";
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
