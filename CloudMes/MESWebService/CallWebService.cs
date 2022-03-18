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
using MESPubLab.MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;

namespace MESWebService
{
    public class CallWebService : MESPubLab.MESStation.MesAPIBase
    {
        public CallWebService(OleExecPool SfcDb, OleExecPool ApDb)
        {
            if (!DBPools.ContainsKey("SFCDB"))
            {
                DBPools.Add("SFCDB", SfcDb);
            }
            if (!DBPools.ContainsKey("APDB"))
            {
                DBPools.Add("APDB", ApDb);
            }
        }
        public MESStationReturn InitStation(MESPubLab.MESStation.MESStationReturn StationReturn, StationPara sp)
        {
            MESReturnMessage.SetSFCDBPool(this.DBPools["SFCDB"]);
            //string Token = requestValue["Token"]?.ToString();
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            //OleExec SFCDB = new OleExec("VERTIVTESTDB", true);
            try
            {
                MESStationBase retStation = retStation = new MESStationBase();

                retStation.StationOutputs.Clear();
                retStation.StationMessages.Clear();
                retStation.StationSession.Clear();
                retStation.DisplayOutput.Clear();
                retStation.Inputs.Clear();
                retStation.IP = this.IP;
                
                //add by 張官軍 2018-1-4 不添加的話，後面獲取該信息的時候回傳空
                User User = new User();
                User.EMP_NO = "Webservice";
                User.EMP_NAME = "Webservice";
                retStation.LoginUser = User;
                //給工站對象賦公共值               
                retStation.Init(sp.Station, sp.Line, sp.Bu, SFCDB);
                MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                ret.Station = retStation;
                //用以執行InitInput.Run()  2018/01/30 SDL
                retStation.SFCDB = SFCDB;
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
                
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + sp.Station + "'Init successfull.";                
            }
            catch (Exception ee)
            {
                StationReturn.Data = null;
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "Station '" + sp.Station + "'Init Fail! "+ee.Message;
                throw ee;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            return StationReturn;
        }

        /// <summary>
        /// tryRunFlag:false=>正常運行
        /// tryRunFlag:true=>檢查所有Action,所有事務回滾
        /// </summary>
        /// <param name="StationReturn"></param>
        /// <param name="CurrScanType"></param>
        /// <param name="InputName"></param>
        /// <param name="tryRunFlag"></param>
        public void StationInput(MESStationReturn StationReturn,string CurrScanType,string InputName,bool tryRunFlag)
        {
            MESStationInput CurrInput = null;
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            OleExec APDB = this.DBPools["APDB"].Borrow();
            MESPubLab.MESStation.MESReturnView.Station.CallStationReturn ret = (MESPubLab.MESStation.MESReturnView.Station.CallStationReturn)StationReturn.Data;
            MESStationBase Station = (MESStationBase)ret.Station;
            Station.StationMessages.Clear();
            Station.NextInput = null;
            Station.SFCDB = SFCDB;
            Station.APDB = APDB;           
            Station.ScanKP.Clear();
            try
            {
                CurrInput = Station.Inputs.Find(t=>t.DisplayName== InputName);

                ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                ret.ScanType = CurrScanType;
                //add by ZGJ 2018-03-19 清空之前的輸入動作執行後輸出到前台的消息
                CurrInput.Station.StationMessages.Clear();
                //調用處理邏輯
                if (!tryRunFlag)
                    CurrInput.Run();
                else
                    CurrInput.TryRun();

                Station.MakeOutput();

                if (ret.ScanType.ToUpper() == "PASS")
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

                Station.SFCDB = null;
                Station.APDB = null;

                ret.Station = Station;
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + Station.DisplayName + "' Input successfull.";
            }
            catch (Exception ee)
            {
                Station.MakeOutput();
                Station.SFCDB = null;
                Station.APDB = null;
                ret = new MESPubLab.MESStation.MESReturnView.Station.CallStationReturn();
                ret.Station = Station;
                Station.StationMessages.Add(new StationMessage() { Message = ee.Message, State = StationMessageState.Fail });
                Station.NextInput = CurrInput;
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "Station '" + Station.DisplayName + "' Input not successfull.";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
            }
        }
    }
    public class StationPara
    {
        public string Station;
        public string Line;
        public string Bu;
    }
}