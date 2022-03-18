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
   public class RStationActionConfig : MESPubLab.MESStation.MesAPIBase
    {
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
        public RStationActionConfig()
        {
            this.Apis.Add(getid.FunctionName, getid);
        }
        //public  void AddStationAction(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        //{
        //    //OleExec sfcdb = null;
        //    string InsertSql = "";
        //    T_R_Station_Action input;
        //    string ID = Data["ID"].ToString();
        //    string RStationInputID = Data["R_STATION_INPUT_ID"].ToString();
        //    string CStationActionID = Data["C_STATION_ACTION_ID"].ToString();
        //    double SeqNo = Convert.ToDouble(Data["SEQ_NO"]);
        //    string ConfigType = Data["CONFIG_TYPE"].ToString();
        //    string ConfigValue = Data["CONFIG_VALUE"].ToString();
        //    double AddFlag = Convert.ToDouble(Data["ADD_FLAG"]);
        //    try
        //    {
        //     //   sfcdb = this.DBPools["SFCDB"].Borrow();
        //        input = new T_R_Station_Action(sfcdb, DBTYPE);
        //        DeleteStationAction(ID, sfcdb);
        //        Row_R_Station_Action row = (Row_R_Station_Action)input.NewRow();
        //        row.ID = input.GetNewID(BU, sfcdb);
        //        row.R_STATION_INPUT_ID = RStationInputID;
        //        row.C_STATION_ACTION_ID = CStationActionID;
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
        //        //  this.DBPools["SFCDB"].Return(sfcdb);
        //        throw e;

        //    }

        //}
        //public  void DeleteStationAction(String ID, OleExec sfcdb)
        //{
        //    string DeleteSql = "";
        //    T_R_Station_Action stationaction;
        //    try
        //    {
        //        stationaction = new T_R_Station_Action(sfcdb, DBTYPE);

        //        if (stationaction.CheckDataExistByID(ID,sfcdb))
        //        {
        //            Row_R_Station_Action row = (Row_R_Station_Action)stationaction.GetObjByID(ID, sfcdb);
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
