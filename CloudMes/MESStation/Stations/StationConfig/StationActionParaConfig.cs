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
    public class StationActionParaConfig : MESPubLab.MESStation.MesAPIBase
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
        public StationActionParaConfig()
        {
            this.Apis.Add(getid.FunctionName, getid);
        }
        /// <summary>
        /// 添加StationActionPara參數
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="sfcdb"></param>
        //public  void AddStationActionPara(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        //{
        //    //OleExec sfcdb = null;
        //    string InsertSql = "";
        //    T_R_Station_Action_Para input;
        //    string ID = Data["ID"].ToString();
        //    string RStationInputID = Data["R_STATION_INPUT_ID"].ToString();
        //    string RStationActionID = Data["R_STATION_ACTION_ID"].ToString();
        //    double SeqNo = Convert.ToDouble(Data["SEQ_NO"]);
        //    string SessionType = Data["SESSION_TYPE"].ToString();
        //    string SessionValue = Data["SESSION_KEY"].ToString();
        //    string AddFlag = Data["VALUE"].ToString();
        //    try
        //    {
        //        sfcdb = this.DBPools["SFCDB"].Borrow();
        //        input = new T_R_Station_Action_Para(sfcdb, DBTYPE);
        //        DeleteStationActionPara(ID, sfcdb);
        //        Row_R_Station_Action_Para row = (Row_R_Station_Action_Para)input.NewRow();
        //        row.ID = input.GetNewID(BU, sfcdb);
        //        row.R_STATION_ACTION_ID = RStationActionID;
        //        row.R_INPUT_ACTION_ID = RStationInputID;
        //        row.SEQ_NO = SeqNo;
        //        row.SESSION_TYPE = SessionType;
        //        row.SESSION_KEY = SessionValue;
        //        row.EDIT_EMP = LoginUser.EMP_NO;
        //        row.EDIT_TIME = GetDBDateTime();
        //        InsertSql = row.GetInsertString(DBTYPE);
        //        sfcdb.ExecSQL(InsertSql);
        //    }
        //    catch (Exception e)
        //    {
        //     //   this.DBPools["SFCDB"].Return(sfcdb);
        //        throw e;

        //    }

        //}

        public void GetID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb;
            T_R_Station_Action_Para station;
            string ID = "";
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                station = new T_R_Station_Action_Para(sfcdb, DBTYPE);
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

        //public void DeleteStationActionPara(String ID, OleExec sfcdb)
        //{
        //    string DeleteSql = "";
        //    T_R_Station_Action_Para stationactionpara;
        //    try
        //    {
        //        stationactionpara = new T_R_Station_Action_Para(sfcdb, DBTYPE);
        //        if (stationactionpara.CheckDataExistByID(ID, sfcdb))
        //        {
        //            Row_R_Station_Action_Para row = (Row_R_Station_Action_Para)stationactionpara.GetObjByID(ID, sfcdb);
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
    }

}
