using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;
using System.Threading;

namespace MESStation.Stations
{
    public class CallStation : MESPubLab.MESStation.MesAPIBase
    {
        public static Dictionary<string, MESStationBase> StationPool = new Dictionary<string, MESStationBase>();

        public static void logout(string Token)
        {
            lock (StationPool)
            {
                List<string> keys = StationPool.Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i].StartsWith(Token))
                    {
                        MESStationBase s = StationPool[keys[i]];
                        s.DBS = null;
                        s.SFCDB = null;
                        s.DBS = null;
                        StationPool.Remove(keys[i]);
                    }
                }
            }
        }

        protected APIInfo FInitStation = new APIInfo()
        {
            FunctionName = "InitStation",
            Description = "初始化工站",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DisplayStationName", InputType = "string", DefaultValue = "SFC_SMT_LOADING" },
                new APIInputInfo() {InputName = "Line", InputType = "string", DefaultValue = "Line1" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FStationInput = new APIInfo()
        {
            FunctionName = "StationInput",
            Description = "工站Input",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Station", InputType = "object", DefaultValue = "" },
                new APIInputInfo() {InputName = "Input", InputType = "object", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo initcreatestation = new APIInfo()
        {
            FunctionName = "InitCreateStation",
            Description = "加載工站數據",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DisplayStationName", InputType = "string", DefaultValue = "Sample" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo savestation = new APIInfo()
        {
            FunctionName = "SaveStation",
            Description = "保存工站",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "DisplayStationName", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _DeletePrintCache = new APIInfo()
        {
            FunctionName = "DeletePrintCache",
            Description = "清除打印缓存",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Station", InputType = "object", DefaultValue = "" },
                new APIInputInfo() {InputName = "Input", InputType = "object", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CallStation()
        {
            this.Apis.Add(FInitStation.FunctionName, FInitStation);
            this.Apis.Add(FStationInput.FunctionName, FStationInput);
            this.Apis.Add(initcreatestation.FunctionName, initcreatestation);
            this.Apis.Add(savestation.FunctionName, savestation);
        }

        public void InitStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            string DisplayName = Data["DisplayStationName"]?.ToString();
            string Token = requestValue["Token"]?.ToString();
            string Line = Data["Line"]?.ToString();

            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            OleExec APDB = this.DBPools["APDB"].Borrow();
            try
            {
                //從對象池中取工站,如不存在則新建一個
                MESStationBase retStation = null;
                lock (StationPool)
                {
                    if (StationPool.ContainsKey(Token + DisplayName))
                    {
                        retStation = StationPool[Token + DisplayName];
                        if (retStation.WayAnswerThread != null)
                        {
                            //retStation.WayAnswerThread.Abort();
                            if (UIReturn.ContainsKey(retStation.ServerMessageID))
                            {
                                var UIcatch = UIReturn[retStation.ServerMessageID];
                                UIcatch.IsTheadCancel = true;
                            }

                        }
                        retStation = new MESStationBase();
                        StationPool[Token + DisplayName] = retStation;
                    }
                    else
                    {
                        retStation = new MESStationBase();
                        StationPool.Add(Token + DisplayName, retStation);
                    }
                }
                retStation.API = this;
                retStation.StationOutputs.Clear();
                retStation.StationMessages.Clear();
                retStation.StationSession.Clear();
                retStation.DisplayOutput.Clear();
                retStation.Inputs.Clear();
                retStation.IP = this.IP;
                //add by 張官軍 2018-1-4 不添加的話，後面獲取該信息的時候回傳空
                retStation.LoginUser = LoginUser;
                //給工站對象賦公共值               
                retStation.Init(DisplayName, Line, BU, DBPools);
                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                ret.Station = retStation;
                //用以執行InitInput.Run()  2018/01/30 SDL
                retStation.SFCDB = SFCDB;
                retStation.APDB = APDB;
                //調用工站初始配置
                MESStationInput InitInput = retStation.Inputs.Find(t => t.Name == "StationINIT");
                if (InitInput != null)
                {
                    InitInput.Run();
                    retStation.Inputs.Remove(InitInput);
                }
                if (retStation.FailStation != null)
                {
                    retStation.FailStation.StationName = retStation.StationName;
                    retStation.FailStation.Line = retStation.Line;
                    InitInput = null;
                    InitInput = retStation.FailStation.Inputs.Find(t => t.Name == "StationINIT");
                    if (InitInput != null)
                    {
                        InitInput.Run();
                        retStation.FailStation.Inputs.Remove(InitInput);
                    }
                }

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "'Init successfull.";
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                this.DBPools["APDB"].Return(APDB);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void InitStationForSMO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            string DisplayName = Data["DisplayStationName"]?.ToString();
            string Token = requestValue["Token"]?.ToString();
            string Line = Data["Line"]?.ToString();
            string Station_NO = Data["Station_NO"]?.ToString();
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            OleExec APDB = this.DBPools["APDB"].Borrow();
            try
            {
                //從對象池中取工站,如不存在則新建一個
                MESStationBase retStation = null;
                if (StationPool.ContainsKey(Token + DisplayName))
                {
                    retStation = StationPool[Token + DisplayName];
                }
                else
                {
                    retStation = new MESStationBase();
                    StationPool.Add(Token + DisplayName, retStation);
                }
                retStation.StationOutputs.Clear();
                retStation.StationMessages.Clear();
                retStation.StationSession.Clear();
                retStation.DisplayOutput.Clear();
                retStation.Inputs.Clear();
                retStation.IP = this.IP;
                //add by 張官軍 2018-1-4 不添加的話，後面獲取該信息的時候回傳空
                retStation.LoginUser = LoginUser;
                //給工站對象賦公共值               
                retStation.Init(DisplayName, Line, BU, DBPools);
                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                ret.Station = retStation;
                //用以執行InitInput.Run()  2018/01/30 SDL
                retStation.SFCDB = SFCDB;
                retStation.APDB = APDB;
                //調用工站初始配置
                MESStationInput InitInput = retStation.Inputs.Find(t => t.Name == "StationINIT");
                if (InitInput != null)
                {
                    InitInput.Run();
                    retStation.Inputs.Remove(InitInput);
                }
                if (retStation.FailStation != null)
                {
                    InitInput = null;
                    InitInput = retStation.FailStation.Inputs.Find(t => t.Name == "StationINIT");
                    if (InitInput != null)
                    {
                        InitInput.Run();
                        retStation.FailStation.Inputs.Remove(InitInput);
                    }
                }

                MESDataObject.Module.T_R_AP_TEMP trat = new MESDataObject.Module.T_R_AP_TEMP(SFCDB, DB_TYPE_ENUM.Oracle);
                trat.InitDeleteAp(SFCDB, Station_NO);
                trat.InitApEmp(SFCDB, Station_NO, retStation.LoginUser.EMP_NO);
                trat.InitApLine(SFCDB, Station_NO, Line);
                trat.InitApStation(SFCDB, Station_NO, retStation.StationName);

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "'Init successfull.";
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                this.DBPools["APDB"].Return(APDB);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void InitCreateStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            string DisplayName = Data["DisplayStationName"]?.ToString();
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                //新建一個工站對象
                //   MESStationBase retStation = new MESStationBase();

                MESStationModel stationmodel = new MESStationModel();

                //給工站對象賦公共值
                stationmodel.Init(DisplayName, SFCDB);

                //    MESPubLab.MESStation.test test = new MESPubLab.MESStation.test();
                //    test.ccc();
                //    MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                //   ret.Station = retStation;
                StationReturn.Data = stationmodel;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "'Init successfull.";
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }
        }
        public void StationInput(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            bool isLockOK = false;
            string DisplayName = Data["Station"]["DisplayName"]?.ToString();
            string Token = requestValue["Token"]?.ToString();
            JToken RCurrInput = Data["Input"];
            MESStationInput CurrInput = null;

            //將工站返回的值加載入工站模型中
            MESStationBase Station = null;
            lock (StationPool)
            {
                if (StationPool.ContainsKey(Token + DisplayName))
                {
                    Station = StationPool[Token + DisplayName];
                }
            }
            Station.API = this;

            OleExec SFCDB = this.DBPools["SFCDB"].Borrow(2000, Station);
            OleExec APDB = this.DBPools["APDB"].Borrow(2000, Station);



            //lock (Station)
            try
            {
                if (!Monitor.TryEnter(Station, 10000))
                {
                    throw new Exception("Station LockTimeOut");
                }
                if (string.IsNullOrEmpty(Station.Line))
                {
                    throw new Exception("Station Line is NULL");
                }
                isLockOK = true;
                Station.StationMessages.Clear();
                Station.NextInput = null;
                Station.SFCDB = SFCDB;
                Station.APDB = APDB;
                Station.IP = requestValue["IP"]["Value"].ToString();
                Station.LabelPrint.Clear();
                Station.LabelPrints.Clear();
                //Station.ScanSCV.Clear();
                Station.ScanKP.Clear();


                for (int i = 0; i < Data["Station"]["Inputs"].Count(); i++)
                {

                    JToken rinput = Data["Station"]["Inputs"][i];
                    MESStationInput input = Station.Inputs.Find(t => t.DisplayName == rinput["DisplayName"].ToString());
                    if (input == null)
                    {
                        continue;
                    }
                    input.Value = rinput["Value"].ToString();
                    if (Data["ScanType"].ToString() == "Pass" && input.DisplayName == RCurrInput["DisplayName"].ToString())
                    {
                        CurrInput = input;
                    }
                }
                if (Station.FailStation != null)
                {
                    //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 Begin
                    Station.FailStation.DBS = Station.DBS;
                    Station.FailStation.SFCDB = SFCDB;
                    Station.FailStation.APDB = APDB;
                    //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 End
                    for (int i = 0; i < Data["Station"]["FailStation"]["Inputs"].Count(); i++)
                    {

                        JToken rinput = Data["Station"]["FailStation"]["Inputs"][i];
                        MESStationInput input = Station.FailStation.Inputs.Find(t => t.DisplayName == rinput["DisplayName"].ToString());
                        if (input == null)
                        {
                            continue;
                        }
                        input.Value = rinput["Value"].ToString();
                        if (Data["ScanType"].ToString() == "Fail" && input.DisplayName == RCurrInput["DisplayName"].ToString())
                        {
                            CurrInput = input;
                        }
                    }
                }

                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                ret.ScanType = Data["ScanType"].ToString();
                //add by ZGJ 2018-03-19 清空之前的輸入動作執行後輸出到前台的消息
                CurrInput.Station.StationMessages.Clear();
                //調用處理邏輯
                try
                {
                    CurrInput.Run();
                }
                catch (Exception eee)
                {
                    Station.NextInput = CurrInput;
                    throw eee;
                }

                Station.MakeOutput();
                if (Station.FailStation != null)
                {
                    Station.FailStation.MakeOutput();
                }

                if (Data["ScanType"].ToString() == "Pass")
                {
                    if (Station.NextInput == null)
                    {
                        for (int i = 0; i < Station.Inputs.Count; i++)
                        {
                            if (Station.Inputs[i] == CurrInput)
                            {
                                if (i != Station.Inputs.Count - 1)
                                {
                                    ret.NextInput = Station.Inputs[i + 1];
                                }
                                else
                                {

                                    ret.NextInput = Station.Inputs[0];
                                }

                            }
                        }
                    }
                    else
                    {
                        ret.NextInput = Station.NextInput;
                    }
                }
                else if (Station.FailStation != null)
                {
                    if (Station.FailStation.NextInput == null)
                    {
                        for (int i = 0; i < Station.FailStation.Inputs.Count; i++)
                        {
                            if (Station.FailStation.Inputs[i] == CurrInput)
                            {
                                if (i != Station.FailStation.Inputs.Count - 1)
                                {
                                    ret.NextInput = Station.FailStation.Inputs[i + 1];
                                }
                                else
                                {

                                    ret.NextInput = Station.FailStation.Inputs[0];
                                }

                            }
                        }
                    }
                    else
                    {
                        ret.NextInput = Station.FailStation.NextInput;
                    }
                }


                //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 Begin
                if (Station.FailStation != null)
                {
                    Station.FailStation.DBS = null;
                    Station.FailStation.SFCDB = null;
                }

                //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 end
                //this.UseDBS.Remove(SFCDB);
                //this.UseDBS.Remove(APDB);
                if (this.UseDBS_UI != null)
                {
                    this.UseDBS_UI.Remove(SFCDB);
                    this.UseDBS_UI.Remove(APDB);
                }
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);

                Station.SFCDB = null;
                Station.APDB = null;
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(Station, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                var StationJtoken = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                ret.Station = StationJtoken;
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "' Input successfull.";
            }
            catch (Exception ee)
            {
                Station.MakeOutput();
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);

                Station.SFCDB = null;
                Station.APDB = null;

                Station.ScanKP.Clear();
                Station.LabelPrints.Clear();
                Station.LabelPrint.Clear();
                Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                { Message = ee.Message, State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail });
                Station.NextInput = CurrInput;

                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(Station, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                var StationJtoken = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                ret.Station = StationJtoken;

                ret.NextInput = StationJtoken["NextInput"];

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "' Input not successfull.";
            }
            finally
            {
                if (isLockOK)
                {
                    Monitor.Exit(Station);
                }
            }

        }

        public void DeletePrintCache(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string DisplayName = Data["Station"]["DisplayName"]?.ToString();
            string Token = requestValue["Token"]?.ToString();
            MESStationBase Station = null;
            if (StationPool.ContainsKey(Token + DisplayName))
            {
                Station = StationPool[Token + DisplayName];
            }
            Station.LabelPrint.Clear();
            Station.LabelPrints.Clear();
            Station.LabelStillPrint.Clear();

        }
        public void StationInputForSMO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string DisplayName = Data["DisplayName"]?.ToString();
            string value = Data["VALUE"]?.ToString();
            string Station_NO = Data["Station_NO"].ToString();
            string Token = requestValue["Token"]?.ToString();
            MESStationInput CurrInput = null;
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            OleExec APDB = this.DBPools["APDB"].Borrow();
            //將工站返回的值加載入工站模型中
            MESStationBase Station = null;
            T_R_AP_TEMP trat = new T_R_AP_TEMP(SFCDB, DB_TYPE_ENUM.Oracle);
            if (StationPool.ContainsKey(Token + DisplayName))
            {
                Station = StationPool[Token + DisplayName];
            }
            Station.StationMessages.Clear();
            Station.NextInput = null;
            Station.SFCDB = SFCDB;
            Station.APDB = APDB;
            Station.IP = requestValue["IP"]["Value"].ToString();

            Station.LabelPrint.Clear();
            Station.LabelPrints.Clear();
            //Station.ScanSCV.Clear();
            Station.ScanKP.Clear();
            try
            {
                //执行公共输入方法
                //begin
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("SN", value);
                dic.Add("Station_No", Station_NO);
                CurrInput = Station.Inputs[0];
                CurrInput.Value = dic;
                CurrInput.Station.StationMessages.Clear();
                CurrInput.Run();
                //end

                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();

                string dsname = trat.GetApNextInput(SFCDB, DB_TYPE_ENUM.Oracle, Station_NO);
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == dsname);

                //ret.ScanType = Data["ScanType"].ToString();
                //add by ZGJ 2018-03-19 清空之前的輸入動作執行後輸出到前台的消息
                CurrInput = input;
                CurrInput.Value = value;
                CurrInput.Station.StationMessages.Clear();
                //調用處理邏輯
                CurrInput.Run();



                //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 Begin
                if (Station.FailStation != null)
                {
                    Station.FailStation.DBS = null;
                    Station.FailStation.SFCDB = null;
                }
                //2018/02/05 肖倫 failStation的db以及dbPool為空的情況 end




                ret.Station = Station;
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(Station, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                var StationJtoken = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                ret.Station = StationJtoken;
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
                Station.SFCDB = null;
                Station.APDB = null;
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "' Input successfull.";
            }
            catch (Exception ee)
            {
                MESPubLab.MESStation.MESReturnView.Station.StationMessage Message = new MESPubLab.MESStation.MESReturnView.Station.StationMessage();
                Message.Message = ee.Message.ToString();
                Message.State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.CMCMessage;
                Station.StationMessages.Add(Message);

                trat.FailDeleteAp(Station.SFCDB, Station_NO);
                Station.MakeOutput();

                //Station.StationMessages.Add();
                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                ret.Station = Station;
                //Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                //{ Message = ee.Message, State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail });
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(Station, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                var StationJtoken = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                ret.Station = StationJtoken;
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
                Station.SFCDB = null;
                Station.APDB = null;
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + DisplayName + "' Input NO successfull.";
            }

        }
        //        public void SaveStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        //        {
        //            string DisplayName = Data["Station"]["DISPLAY_STATION_NAME"]?.ToString();
        //            string StationName = Data["Station"]["STATION_NAME"]?.ToString();
        //            string FailStationID = Data["Station"]["FAIL_STATION_ID"]?.ToString();
        //            double FailStationFlag =Convert.ToDouble( Data["Station"]["FAIL_STATION_FLAG"]);
        //            string StationID= Data["Station"]["ID"]?.ToString();

        //            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
        //            try
        //            {
        //                sfcdb.BeginTrain();
        //                MesAPIBase api = new MesAPIBase();
        //                api.LoginUser = LoginUser;
        //                //插入R_STATION表
        //                StationConfig.StationConfig stationconfig = new StationConfig.StationConfig();

        //                stationconfig.AddStation(DisplayName, StationName, FailStationID, FailStationFlag, StationID, sfcdb);
        //                //插入R_Station_Output表
        //                for (int i = 0; i < Data["Station"]["OutputList"].Count(); i++)
        //                {
        //                    JToken output = Data["Station"]["OutputList"][i];
        //                    StationConfig.StationOutputConfig outputconfig = new StationConfig.StationOutputConfig();
        //                    outputconfig.LoginUser = LoginUser;
        //                    outputconfig.AddStationOutput(output, sfcdb);
        //                }
        //                //   //插入 R_Station_Input 表
        //                for (int i = 0; i < Data["Station"]["InputList"].Count(); i++)
        //                {
        //                    //插入 R_Station_Input 表
        //                    JToken input = Data["Station"]["InputList"][i];
        //                    StationConfig.StationInputConfig stationinput = new StationConfig.StationInputConfig();
        //                    stationinput.LoginUser = LoginUser;
        //                    stationinput.AddInput(input, sfcdb);
        //                    //插入 R_Input_Action 表
        //                    for (int j = 0; j < Data["input"]["InputActionList"].Count(); j++)
        //                    {
        //                        JToken iaction = Data["input"]["InputActionList"][i];
        //                        StationConfig.InputActionConfig inputaction = new StationConfig.InputActionConfig();
        //                        inputaction.LoginUser = LoginUser;
        //                        inputaction.AddInputActionS(iaction, sfcdb);
        //                        for (int k = 0; k < iaction["ParaSA"].Count(); k++) //插入R_Station_Action_Para表
        //                        {
        //                            StationConfig.StationActionParaConfig stationactionpara = new StationConfig.StationActionParaConfig();
        //                            stationactionpara.LoginUser = LoginUser;
        //                            stationactionpara.AddStationActionPara(iaction["ParaSA"][i], sfcdb);
        //                        }
        //                    }
        //                    //插入 R_Station_Action 表
        //                    for (int j = 0; j < Data["input"]["StationActionList"].Count(); j++)
        //                    {
        //                        JToken saction = Data["input"]["StationActionList"][i];
        //                        StationConfig.RStationActionConfig stationaction = new StationConfig.RStationActionConfig();
        //                        stationaction.LoginUser = LoginUser;
        //                        stationaction.AddStationAction(saction, sfcdb);
        //                        for (int k=0;k< saction["ParaSA"].Count();k++) //插入R_Station_Action_Para表
        //                        {
        //                            StationConfig.StationActionParaConfig stationactionpara = new StationConfig.StationActionParaConfig();
        //                            stationactionpara.LoginUser = LoginUser;
        //                            stationactionpara.AddStationActionPara(saction["ParaSA"][i], sfcdb);
        //                        }
        //                    }

        //                }
        //                sfcdb.CommitTrain();
        //                this.DBPools["SFCDB"].Return(sfcdb);

        //            }
        //            catch (Exception ee)
        //            {
        //                this.DBPools["SFCDB"].Return(sfcdb);
        //                throw ee;
        //            }

        //}
        void RunAction()
        {

        }

        void sortAction(List<R_Station_Action> Actions, Dictionary<string, List<R_Station_Action>> dir)
        {
            List<R_Station_Action> Action = Actions.FindAll(t => t.CONFIG_TYPE == "Default");
            Action.Sort();
            dir.Add("Default", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "Customer");
            Action.Sort();
            dir.Add("Customer", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "Series");
            Action.Sort();
            dir.Add("Series", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "Sku");
            Action.Sort();
            dir.Add("Sku", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "WorkerOrder");
            Action.Sort();
            dir.Add("WorkerOrder", Action);

            Action = Actions.FindAll(t => t.CONFIG_TYPE == "Line");
            Action.Sort();
            dir.Add("Line", Action);
        }


        //void InitMESStationBase
        public void StationDeleteAP(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string DisplayName = Data["DisplayName"]?.ToString();
            string value = Data["VALUE"]?.ToString();
            string Station_NO = Data["Station_NO"].ToString();
            string Token = requestValue["Token"]?.ToString();
            MESStationInput CurrInput = null;
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            OleExec APDB = this.DBPools["APDB"].Borrow();
            //將工站返回的值加載入工站模型中
            MESStationBase Station = null;
            T_R_AP_TEMP trat = new T_R_AP_TEMP(SFCDB, DB_TYPE_ENUM.Oracle);
            if (StationPool.ContainsKey(Token + DisplayName))
            {
                Station = StationPool[Token + DisplayName];
            }
            Station.StationMessages.Clear();
            Station.NextInput = null;
            Station.SFCDB = SFCDB;
            Station.APDB = APDB;
            Station.IP = requestValue["IP"]["Value"].ToString();

            Station.LabelPrint.Clear();
            Station.LabelPrints.Clear();
            //Station.ScanSCV.Clear();
            Station.ScanKP.Clear();
            try
            {
                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();

                if (value.ToUpper() == "UNDO")
                {
                    trat.InitDeleteAp(SFCDB, Station_NO);
                }

                ret.Station = Station;
                //List<MESPubLab.MESStation.MESReturnView.Station.StationMessage> CMCMessages = ((MESStationBase)ret.Station).StationMessages.Where(s => (s.State == MESPubLab.MESStation.MESReturnView.Station.StationMessageState.CMCMessage || s.State == MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail || s.State == MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass)).ToList();
                //MESPubLab.MESStation.MESReturnView.Station.StationMessage Message = CMCMessages[CMCMessages.Count - 1];
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK";
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
                Station.SFCDB = null;
                Station.APDB = null;
            }
            catch (Exception ee)
            {
                MESPubLab.MESStation.MESReturnView.Station.StationMessage Message;
                trat.FailDeleteAp(Station.SFCDB, Station_NO);
                Station.MakeOutput();
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
                Station.SFCDB = null;
                Station.APDB = null;
                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                ret.Station = Station;
                //Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                //{ Message = ee.Message, State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail });
                //List<MESPubLab.MESStation.MESReturnView.Station.StationMessage> CMCMessages = ((MESStationBase)ret.Station).StationMessages.Where(s => (s.State == MESPubLab.MESStation.MESReturnView.Station.StationMessageState.CMCMessage || s.State == MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail || s.State == MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass)).ToList();
                //if (CMCMessages.Count == 0)
                //{
                //    MESPubLab.MESStation.MESReturnView.Station.StationMessage m = new MESPubLab.MESStation.MESReturnView.Station.StationMessage { Message = ee.Message, State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail };
                //    Message = m;
                //}
                //else
                //{
                //    Message = CMCMessages[CMCMessages.Count - 1];
                //}

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "FAIL";
            }


        }
    }
}
