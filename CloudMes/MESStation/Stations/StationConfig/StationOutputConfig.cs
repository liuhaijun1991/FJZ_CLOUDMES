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
    public  class StationOutputConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo stationoutput = new APIInfo()
        {
            FunctionName = "AddStationOutput",
            Description = "添加輸入框",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="RStationID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Name",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="SeqNo",InputType="int",DefaultValue=""},
                new APIInputInfo() { InputName="DisplayType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="SessionType",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="SessionKey",InputType="string",DefaultValue=""}
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
        public StationOutputConfig()
        {
            this.Apis.Add(stationoutput.FunctionName, stationoutput);
            this.Apis.Add(getid.FunctionName, getid);
        }
        public void AddStationOutput(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_R_Station_Output StationOutput;
            string RStationID = Data["RStationID"].ToString().Trim();
            string Name = Data["Name"].ToString().Trim();
            int SeqNo = Convert.ToInt32(Data["SeqNo"]);
            string DisplayType = Data["DisplayType"].ToString();
            string SessionType = Data["SessionType"].ToString();
            string SessionKey = Data["SessionKey"].ToString();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                StationOutput = new T_R_Station_Output(sfcdb, DBTYPE);
                if (StationOutput.CheckDataExist(RStationID, Name, sfcdb))
                {
                    Row_R_Station_Output row = (Row_R_Station_Output)StationOutput.NewRow();
                    row.ID = StationOutput.GetNewID(BU, sfcdb);
                    row.R_STATION_ID= RStationID;
                    row.NAME = Name;
                    row.SEQ_NO = SeqNo;
                    row.DISPLAY_TYPE = DisplayType;
                    row.SESSION_TYPE = SessionType;
                    row.SESSION_KEY = SessionKey;
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

        public void GetID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb;
            T_R_Station_Output stationout;
            string ID = "";
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                stationout = new T_R_Station_Output(sfcdb, DBTYPE);
                ID = stationout.GetNewID(BU, sfcdb);
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

        //public  void AddStationOutput(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        //{
        //    string InsertSql = "";
        //    T_R_Station_Output StationOutput;
        //    string ID = Data["ID"]?.ToString();
        //    string StationID = Data["R_STATION_ID"]?.ToString();
        //    string OutputName = Data["NAME"]?.ToString();
        //    double SeqNo = Convert.ToDouble(Data["SEQ_NO"]);
        //    string SessionType = Data["SESSION_TYPE"]?.ToString();
        //    string SessionKey = Data["SESSION_KEY"]?.ToString();
        //    string DisplayType = Data["DISPLAY_TYPE"]?.ToString();
        //    try
        //    {
        //        StationOutput = new T_R_Station_Output(sfcdb, DBTYPE);
        //        DeleteStationOutput(ID, sfcdb);
        //        Row_R_Station_Output row = (Row_R_Station_Output)StationOutput.NewRow();
        //        row.ID = StationOutput.GetNewID(BU, sfcdb);
        //        row.R_STATION_ID = StationID;
        //        row.NAME = OutputName;
        //        row.SEQ_NO = SeqNo;
        //        row.DISPLAY_TYPE = DisplayType;
        //        row.SESSION_TYPE = SessionType;
        //        row.SESSION_KEY = SessionKey;
        //        row.EDIT_EMP = LoginUser.EMP_NO;
        //        row.EDIT_TIME = GetDBDateTime();
        //        InsertSql = row.GetInsertString(DBTYPE);
        //        sfcdb.ExecSQL(InsertSql);
        //    }
        //    catch (Exception e)
        //    {
        //        //   this.DBPools["SFCDB"].Return(sfcdb);
        //        throw e;

        //    }

        //}

        //public void DeleteStationOutput(String ID, OleExec sfcdb)
        //{
        //    string DeleteSql = "";
        //    T_R_Station_Output stationoutput;
        //    try
        //    {
        //        stationoutput = new T_R_Station_Output(sfcdb, DBTYPE);
        //        if (stationoutput.CheckDataExistByid(ID, sfcdb))//存在就刪除
        //        {
        //            Row_R_Station_Output row = (Row_R_Station_Output)stationoutput.GetObjByID(ID, sfcdb);
        //            DeleteSql = row.GetDeleteString(DBTYPE);
        //            sfcdb.ExecSQL(DeleteSql);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }

        //}
    }
}
