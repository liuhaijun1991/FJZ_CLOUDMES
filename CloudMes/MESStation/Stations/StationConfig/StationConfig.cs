using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using Newtonsoft.Json.Linq;

namespace MESStation.Stations.StationConfig
{
    public class StationConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo addstation = new APIInfo()
        {
            FunctionName = "AddStation",
            Description = "添加工站",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="DisplayStationName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="StationName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="FailStationID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="FailStationFlag",InputType="string",DefaultValue=""}
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

        private APIInfo getstation = new APIInfo()
        {
            FunctionName = "QueryStation",
            Description = "獲取工站",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo deletestation = new APIInfo()
        {
            FunctionName = "ByIDDeleteStation",
            Description = "刪除工站",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo queryfailstation = new APIInfo()
        {
            FunctionName = "QueryFailStation",
            Description = "查詢掃FAIL工站",
            Parameters = new List<APIInputInfo>()
            {
               
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo copystation = new APIInfo()
        {
            FunctionName = "Copystation",
            Description = "Copy Station",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="OldStationDisplayName",InputType="string",DefaultValue=""},
                 new APIInputInfo() { InputName="NewOldStationDisplayName",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        // QueryFailStation

        public StationConfig()
        {
            this.Apis.Add(addstation.FunctionName, addstation);
            this.Apis.Add(getid.FunctionName, getid);
            this.Apis.Add(getstation.FunctionName, getstation);
            this.Apis.Add(deletestation.FunctionName, deletestation);
            this.Apis.Add(queryfailstation.FunctionName, queryfailstation);
            this.Apis.Add(copystation.FunctionName, copystation);
        }

        public void Copystation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            
            string OldStationDisplayName = Data["OldStationDisplayName"].ToString().Trim();
            string NewStationDisplayName = Data["NewOldStationDisplayName"].ToString().Trim();
            
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                T_R_Station Tstation = new T_R_Station(sfcdb, DBTYPE);
                if (Tstation.CheckDataExist(NewStationDisplayName, sfcdb))
                {
                    var station = sfcdb.ORM.Queryable<R_Station>().Where(t => t.DISPLAY_STATION_NAME == OldStationDisplayName).First();
                    var OleStationID = station.ID;
                    station.ID = MesDbBase.GetNewID<R_Station>(sfcdb.ORM, BU);
                    station.DISPLAY_STATION_NAME = NewStationDisplayName;
                    sfcdb.ORM.Insertable(station).ExecuteCommand();

                    var Inputs = sfcdb.ORM.Queryable<R_Station_Input>().Where(t => t.STATION_ID == OleStationID).ToList();
                    for (int i = 0; i < Inputs.Count; i++)
                    {
                        var input = Inputs[i];
                        var StationActionList = sfcdb.ORM.Queryable<R_Station_Action>().Where(t => t.R_STATION_INPUT_ID == input.ID).ToList();
                        input.ID = MesDbBase.GetNewID<R_Station>(sfcdb.ORM, BU);
                        input.STATION_ID = station.ID;
                        sfcdb.ORM.Insertable(input).ExecuteCommand();

                        for (int j = 0; j < StationActionList.Count; j++)
                        {
                            var StationAction = StationActionList[j];
                            var OldStationActionID = StationAction.ID;
                            //StationAction.ID = MesDbBase.GetNewID("SFCDB",  BU, "R_Station_Action");
                            StationAction.ID = MesDbBase.GetNewID<R_Station_Input>(sfcdb.ORM, BU);
                            StationAction.R_STATION_INPUT_ID = input.ID;
                            sfcdb.ORM.Insertable(StationAction).ExecuteCommand();

                            var ParaSA = sfcdb.ORM.Queryable<R_Station_Action_Para>().Where(t => t.R_STATION_ACTION_ID == OldStationActionID).ToList();
                            for (int k = 0; k < ParaSA.Count; k++)
                            {
                                var para = ParaSA[k];
                                para.ID = MesDbBase.GetNewID<R_Station_Action_Para>(sfcdb.ORM, BU);
                                para.R_STATION_ACTION_ID = StationAction.ID;
                                sfcdb.ORM.Insertable(para).ExecuteCommand();
                            }
                        }
                    }

                    var stationoutputs = sfcdb.ORM.Queryable<R_Station_Output>().Where(t => t.R_STATION_ID == OleStationID).ToList();
                    for (int i = 0; i < stationoutputs.Count; i++)
                    {
                        var stationoutput = stationoutputs[i];
                        stationoutput.ID = MesDbBase.GetNewID<R_Station_Output>(sfcdb.ORM, BU);
                        stationoutput.R_STATION_ID = station.ID;
                        sfcdb.ORM.Insertable(stationoutput).ExecuteCommand();
                    }
                    sfcdb.CommitTrain();
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    sfcdb.CommitTrain();
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }
        public void AddStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_R_Station station;
            string DisplayStationName = Data["DisplayStationName"].ToString().Trim();
            string StationName = Data["StationName"].ToString().Trim();
            string FailStationID = Data["FailStationID"].ToString().Trim();
            double FailStationFlag = Convert.ToDouble(Data["FailStationFlag"]);
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                station = new T_R_Station(sfcdb, DBTYPE);
                if (station.CheckDataExist(DisplayStationName, sfcdb))
                {
                    Row_R_Station row = (Row_R_Station)station.NewRow();
                    row.ID = station.GetNewID(BU, sfcdb);
                    row.DISPLAY_STATION_NAME = DisplayStationName;
                    row.STATION_NAME = StationName;
                    row.FAIL_STATION_ID = FailStationID;
                    row.FAIL_STATION_FLAG = FailStationFlag;
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

        public void AddStation(string DisplayName, string StationName, string FailStationID, double FailStationFlag, string StationID, OleExec sfcdb)
        {
            string InsertSql = "";
            T_R_Station station;
            try
            {
                station = new T_R_Station(sfcdb, DBTYPE);
                DeleteStation(StationID, sfcdb); //先刪除這個ID的數據
                CheckDataExistByStationName(DisplayName, sfcdb);
                Row_R_Station row = (Row_R_Station)station.NewRow();
                row.ID = StationID;
                row.DISPLAY_STATION_NAME = DisplayName;
                row.STATION_NAME = StationName;
                row.FAIL_STATION_ID = FailStationID;
                row.FAIL_STATION_FLAG = FailStationFlag;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.EDIT_TIME = GetDBDateTime();
                InsertSql = row.GetInsertString(DBTYPE);
                sfcdb.ExecSQL(InsertSql);

            }
            catch (Exception e)
            {
                // this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }

        public void CheckDataExistByStationName(string DisplayStationName, OleExec sfcdb)
        {

            T_R_Station stationaction;
            try
            {
                stationaction = new T_R_Station(sfcdb, DBTYPE);
                if (stationaction.CheckDataExist(DisplayStationName, sfcdb)==false)
                {
                    //throw new Exception("工站名稱已經存在!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814112009", new string[] { DisplayStationName }));
                    //  string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    //   throw new MESReturnMessage("工站顯示名稱已經存在!");

                }
            }
            catch (Exception e)
            {
                //  this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteStation(String ID, OleExec sfcdb)
        {
            string DeleteSql = "";
            T_R_Station stationaction;
            try
            {
                stationaction = new T_R_Station(sfcdb, DBTYPE);
                if (stationaction.CheckDataExistByID(ID, sfcdb))
                {
                    Row_R_Station row = (Row_R_Station)stationaction.GetObjByID(ID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                   // stationaction.DeleteDataByID(ID, sfcdb);
                }
            }
            catch (Exception e)
            {
              //  this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public void GetID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb;
            T_R_Station station;
            string ID = "";
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                station = new T_R_Station(sfcdb, DBTYPE);
                ID = station.GetNewID(BU,sfcdb);
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
     
        /// <summary>
        /// 保存工站信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void SaveStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string DisplayName = Data["Station"]["DISPLAY_STATION_NAME"]?.ToString();
            string StationName = Data["Station"]["STATION_NAME"]?.ToString();
            string FailStationID = Data["Station"]["FAIL_STATION_ID"]?.ToString();
            double FailStationFlag =Convert.ToDouble(Data["Station"]["FAIL_STATION_FLAG"]) ;
            
            try
            {
                FailStationFlag = Convert.ToDouble(Data["Station"]["FAIL_STATION_FLAG"]);
            }
            catch
            { }
                
            string StationID = Data["Station"]["ID"]?.ToString();

            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {

                sfcdb.BeginTrain();
                ByIDDeleteStationID(StationID,sfcdb);//刪除整下工站

                AddStation(DisplayName, StationName, FailStationID, FailStationFlag, StationID, sfcdb);
                //插入R_Station_Output表
                for (int i = 0; i < Data["Station"]["OutputList"].Count(); i++)
                {
                    JToken output = Data["Station"]["OutputList"][i];
                    AddStationOutput(output, sfcdb);
                }
                //插入 R_Station_Input 表
                for (int i = 0; i < Data["Station"]["InputList"].Count(); i++)
                {
                    //插入 R_Station_Input 表
                    JToken input = Data["Station"]["InputList"][i];
                    AddInput(input, sfcdb);
                    ////插入 R_Input_Action 表
                    //for (int j = 0; j < input["InputActionList"].Count(); j++)
                    //{
                    //    JToken iaction = input["InputActionList"][j];
                    //    AddInputActionS(iaction, sfcdb);
                    //    for (int k = 0; k < iaction["ParaSA"].Count(); k++) //插入R_Station_Action_Para表
                    //    {
                    //        AddStationActionPara(iaction["ParaSA"][k], sfcdb);
                    //    }
                    //}
                    //插入 R_Station_Action 表
                    for (int j = 0; j < input["StationActionList"].Count(); j++)
                    {
                        JToken saction = input["StationActionList"][j];
                        AddStationAction(saction, sfcdb);
                        for (int k = 0; k < saction["ParaSA"].Count(); k++) //插入R_Station_Action_Para表
                        {
                            AddStationActionPara(saction["ParaSA"][k], sfcdb);
                        }
                    }

                }
                sfcdb.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception ee)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw ee;
            }

        }
        /// <summary>
        /// 根據ID刪除工站
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void ByIDDeleteStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb;
            T_R_Station station;
            string ID = Data["ID"].ToString(); 
            sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                sfcdb.BeginTrain();
                station = new T_R_Station(sfcdb, DBTYPE);
                station.DeleteByRStationID(ID, sfcdb);
                sfcdb.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void ByIDDeleteStationID(string RstationID, OleExec sfcdb)
        {
            T_R_Station station;
            try
            {
                station = new T_R_Station(sfcdb, DBTYPE);
                station.DeleteByRStationID(RstationID, sfcdb);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public void AddStationAction(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        {
            //OleExec sfcdb = null;
            string InsertSql = "";
            T_R_Station_Action input;
            string ID = Data["ID"].ToString();
            string RStationInputID = Data["R_STATION_INPUT_ID"].ToString();
            string CStationActionID = Data["C_STATION_ACTION_ID"].ToString();
            double SeqNo = Convert.ToDouble(Data["SEQ_NO"]);
            string ConfigType = Data["CONFIG_TYPE"].ToString();
            string ConfigValue = Data["CONFIG_VALUE"].ToString();
            double AddFlag = Convert.ToDouble(Data["ADD_FLAG"]);
            try
            {
                input = new T_R_Station_Action(sfcdb, DBTYPE);
                DeleteStationAction(ID, sfcdb);
                Row_R_Station_Action row = (Row_R_Station_Action)input.NewRow();
                row.ID = ID;
                row.R_STATION_INPUT_ID = RStationInputID;
                row.C_STATION_ACTION_ID = CStationActionID;
                row.SEQ_NO = SeqNo;
                row.CONFIG_TYPE = ConfigType;
                row.CONFIG_VALUE = ConfigValue;
                row.ADD_FLAG = AddFlag;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.EDIT_TIME = GetDBDateTime();
                InsertSql = row.GetInsertString(DBTYPE);
                sfcdb.ExecSQL(InsertSql);
            }
            catch (Exception e)
            {
                //  this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }
        public void DeleteStationAction(String ID, OleExec sfcdb)
        {
            string DeleteSql = "";
            T_R_Station_Action stationaction;
            try
            {
                stationaction = new T_R_Station_Action(sfcdb, DBTYPE);

                if (stationaction.CheckDataExistByID(ID, sfcdb))
                {
                    Row_R_Station_Action row = (Row_R_Station_Action)stationaction.GetObjByID(ID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
            }
            catch (Exception e)
            {
                //  this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public void AddStationOutput(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        {
            string InsertSql = "";
            T_R_Station_Output StationOutput;
            string ID = Data["ID"]?.ToString();
            string StationID = Data["R_STATION_ID"]?.ToString();
            string OutputName = Data["NAME"]?.ToString();
            double SeqNo = Convert.ToDouble(Data["SEQ_NO"]);
            string SessionType = Data["SESSION_TYPE"]?.ToString();
            string SessionKey = Data["SESSION_KEY"]?.ToString();
            string DisplayType = Data["DISPLAY_TYPE"]?.ToString();
            try
            {
                StationOutput = new T_R_Station_Output(sfcdb, DBTYPE);
                DeleteStationOutput(ID, sfcdb);
                Row_R_Station_Output row = (Row_R_Station_Output)StationOutput.NewRow();
                row.ID = ID;
                row.R_STATION_ID = StationID;
                row.NAME = OutputName;
                row.SEQ_NO = SeqNo;
                row.DISPLAY_TYPE = DisplayType;
                row.SESSION_TYPE = SessionType;
                row.SESSION_KEY = SessionKey;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.EDIT_TIME = GetDBDateTime();
                InsertSql = row.GetInsertString(DBTYPE);
                sfcdb.ExecSQL(InsertSql);
            }
            catch (Exception e)
            {
                //   this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }

        public void DeleteStationOutput(String ID, OleExec sfcdb)
        {
            string DeleteSql = "";
            T_R_Station_Output stationoutput;
            try
            {
                stationoutput = new T_R_Station_Output(sfcdb, DBTYPE);
                if (stationoutput.CheckDataExistByid(ID, sfcdb))//存在就刪除
                {
                    Row_R_Station_Output row = (Row_R_Station_Output)stationoutput.GetObjByID(ID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void AddInput(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        {
            string InsertSql = "";
            T_R_Station_Input input;
            string ID = Data["ID"].ToString();
            string StationID = Data["STATION_ID"].ToString();
            string InputID = Data["INPUT_ID"].ToString();
            double SeqNo = Convert.ToDouble(Data["SEQ_NO"]);
            string Rlinput = Data["REMEMBER_LAST_INPUT"].ToString();
            double ScanFlag = Convert.ToDouble(Data["SCAN_FLAG"]);
            string DName = Data["DISPLAY_NAME"].ToString();
            try
            {
                input = new T_R_Station_Input(sfcdb, DBTYPE);
                DeleteStationInput(ID, sfcdb);
                Row_R_Station_Input row = (Row_R_Station_Input)input.NewRow();
                row.ID = ID;
                row.STATION_ID = StationID;
                row.INPUT_ID = InputID;
                row.SEQ_NO = SeqNo;
                row.SCAN_FLAG = ScanFlag;
                row.DISPLAY_NAME = DName;
                row.REMEMBER_LAST_INPUT = Rlinput;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.EDIT_TIME = GetDBDateTime();
                InsertSql = row.GetInsertString(DBTYPE);
                sfcdb.ExecSQL(InsertSql);
            }
            catch (Exception e)
            {
                //  this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }
        public void DeleteStationInput(String ID, OleExec sfcdb)
        {
            string DeleteSql = "";
            T_R_Station_Input stationaction;
            try
            {
                stationaction = new T_R_Station_Input(sfcdb, DBTYPE);
                if (stationaction.CheckDataExistByID(ID, sfcdb))//存在就刪除
                {
                    Row_R_Station_Input row = (Row_R_Station_Input)stationaction.GetObjByID(ID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void AddInputActionS(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        {
            //   OleExec sfcdb = null;
            string InsertSql = "";
            T_R_Input_Action inputaction;
            string ID = Data["ID"].ToString();
            string InputID = Data["InputID"].ToString();
            string StationActionID = Data["StationActionID"].ToString();
            int SeqNo = Convert.ToInt32(Data["SeqNo"]);
            string ConfigType = Data["ConfigType"].ToString();
            string ConfigValue = Data["ConfigValue"].ToString();
            int AddFlag = Convert.ToInt32(Data["AddFlag"]);
            try
            {
                inputaction = new T_R_Input_Action(sfcdb, DBTYPE);
                DeleteInputAction(ID, sfcdb);
                Row_R_Input_Action row = (Row_R_Input_Action)inputaction.NewRow();
                row.ID = ID;
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
            }
            catch (Exception e)
            {
                //this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }

        public void DeleteInputAction(String ID, OleExec sfcdb)
        {
            string DeleteSql = "";
            T_R_Input_Action stationaction;
            try
            {
                stationaction = new T_R_Input_Action(sfcdb, DBTYPE);
                if (stationaction.CheckDataExistByID(ID, sfcdb))
                {
                    Row_R_Input_Action row = (Row_R_Input_Action)stationaction.GetObjByID(ID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
            }
            catch (Exception e)
            {
                //  this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public void AddStationActionPara(Newtonsoft.Json.Linq.JToken Data, OleExec sfcdb)
        {
            //OleExec sfcdb = null;
            string InsertSql = "";
            T_R_Station_Action_Para input;
            string ID = Data["ID"].ToString();
            string RInputActionID = Data["R_INPUT_ACTION_ID"].ToString();
            string RStationActionID = Data["R_STATION_ACTION_ID"].ToString();
            double SeqNo = Convert.ToDouble(Data["SEQ_NO"]);
            string SessionType = Data["SESSION_TYPE"].ToString();
            string SessionValue = Data["SESSION_KEY"].ToString();
            string StrValue = Data["VALUE"].ToString();
            try
            {
                input = new T_R_Station_Action_Para(sfcdb, DBTYPE);
                DeleteStationActionPara(ID, sfcdb);
                Row_R_Station_Action_Para row = (Row_R_Station_Action_Para)input.NewRow();
                row.ID = ID;
                row.R_STATION_ACTION_ID = RStationActionID;
                row.R_INPUT_ACTION_ID = RInputActionID;
                row.SEQ_NO = SeqNo;
                row.SESSION_TYPE = SessionType;
                row.SESSION_KEY = SessionValue;
                row.VALUE = StrValue;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.EDIT_TIME = GetDBDateTime();
                InsertSql = row.GetInsertString(DBTYPE);
                sfcdb.ExecSQL(InsertSql);
            }
            catch (Exception e)
            {
                //   this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }

        public void DeleteStationActionPara(String ID, OleExec sfcdb)
        {
            string DeleteSql = "";
            T_R_Station_Action_Para stationactionpara;
            try
            {
                stationactionpara = new T_R_Station_Action_Para(sfcdb, DBTYPE);
                if (stationactionpara.CheckDataExistByID(ID, sfcdb))
                {
                    Row_R_Station_Action_Para row = (Row_R_Station_Action_Para)stationactionpara.GetObjByID(ID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
            }
            catch (Exception e)
            {
                //  this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public void QueryStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_R_Station station;
            List<R_Station> rList;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                station = new T_R_Station(sfcdb, DBTYPE);
                rList = station.Queryrstation( sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = rList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public void QueryFailStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_R_Station station;
            List<R_Station> rList;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                station = new T_R_Station(sfcdb, DBTYPE);
                rList = station.QueryFailStation(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = rList;
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
