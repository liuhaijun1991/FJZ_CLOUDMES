using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESJuniper.TruckLoad;
using MESPubLab.MESInterface;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using MESPubLab.SAP_RFC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace MESJuniper.Stations.StationActions.DataCheckers
{
    public class ShipoutAction
    {
        /// <summary>
        /// Call SAP 311 RFC Truck Load
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TruckLoadGT(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession Trailer = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Trailer == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            UIInputData O = new UIInputData()
            {
                Timeout = 60000,
                UIArea = new string[] { "35%", "45%" },
                IconType = IconType.Warning,
                Message = "",
                Tittle = "",
                Type = UIInputType.YesNo,
                Name = "",
                ErrMessage = ""
            };
            O.OutInputs.Add(new DisplayOutPut { Name = "Confirm", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"Are you sure you want to Call SAP 311?" });
            var res = O.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
            if (res.ToUpper() != "YES")
            {
                return;
            }
            DataTable dt = new DataTable();
            string msg = "";
            string StrTrailer = "", StrTO = "", StrBU = "", StrPlant = "";
            var truckObj = new TruckLoadLogic(Station.SFCDB.ORM);
            StrTrailer = Trailer.Value.ToString();
            var TOList = truckObj.GetPendingTruckLoadGTTOList(StrTrailer);

            if (StrBU == "")
            {
                StrBU = "FJZ";
            }
            if (StrPlant == "")
            {
                StrPlant = "MBGA";
            }

            UIInputData ProgressWin = new UIInputData() { };
            ProgressWin.Timeout = 3000000;
            ProgressWin.IconType = IconType.Warning;
            ProgressWin.Type = UIInputType.Table;
            ProgressWin.ReturnData = new DataTable();
            ProgressWin.Tittle = "Progress";
            ProgressWin.ErrMessage = "No input";
            ProgressWin.UIArea = new string[] { "80%", "80%" };
            ProgressWin.OutInputs.Clear();
            ProgressWin.Name = "SAP 311 Progress";
            ProgressWin.CBMessage = "";

            if (TOList.Count == 0)
            {
                ProgressWin.ReturnData = new ProgressReturnData() { IsFinish = false, Total = TOList.Count, Index = 0, Percent = "1%" }; ProgressWin.OutInputs.Add(new DisplayOutPut()
                {
                    DisplayType = UIOutputType.Text.ToString(),
                    Name = "Note",
                    Value = "No TO# Of Trial [" + Trailer.Value.ToString() + "] Pending SAP 311 !"
                });
                ProgressWin.GetUiInput(Station.API, UIInput.Progress);
                return;
            }

            ProgressWin.ReturnData = new ProgressReturnData() { IsFinish = false, Total = TOList.Count, Index = 0, Percent = "1%" }; ProgressWin.OutInputs.Add(new DisplayOutPut()
            {
                DisplayType = UIOutputType.Table.ToString(),
                Name = "TOList",
                Value = TOList
            });
            ProgressWin.ReturnData = new ProgressReturnData() { IsFinish = false, Total = TOList.Count, Index = 0, Percent = "1%" }; ProgressWin.OutInputs.Add(new DisplayOutPut()
            {
                DisplayType = UIOutputType.Text.ToString(),
                Name = "Warming",
                Value = "Please waitting,don't close any window of the browser before SAP 311 finish!"
            });
            ProgressWin.OutInputs.Add(new DisplayOutPut()
            {
                DisplayType = UIOutputType.Text.ToString(),
                Name = "Progress",
                Value = "Start!"
            });
            ProgressWin.GetUiInput(Station.API, UIInput.Progress);
            var logdb = Station.DBS["SFCDB"].Borrow();
            try
            {
                for (int i = 0; i < TOList.Count; i++)
                {
                    StrTO = TOList[i].TO_NO;
                    if (InterfacePublicValues.IsMonthly(Station.SFCDB, DB_TYPE_ENUM.Oracle))
                    {
                        R_MES_LOG log = new R_MES_LOG()
                        {
                            ID = MesDbBase.GetNewID<R_MES_LOG>(logdb.ORM, StrBU),
                            PROGRAM_NAME = "TruckLoadMonthly",
                            CLASS_NAME = "MESStation.Stations.StationActions.ActionRunners",
                            FUNCTION_NAME = "ShippingAction.TruckLoadCall311",
                            MAILFLAG = "N",
                            LOG_MESSAGE = msg,
                            DATA1 = StrTO,
                            DATA2 = ConfigurationManager.AppSettings[Station.BU + "_TRUCK311FROM"],
                            DATA3 = ConfigurationManager.AppSettings[Station.BU + "_TRUCK311TO"],
                            EDIT_EMP = Station.LoginUser.EMP_NO,
                            EDIT_TIME = DateTime.Now,
                            DATA4 = "N"
                        };
                        logdb.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                        TOList[i].CLOSED = "2";
                        logdb.ORM.Updateable<R_JUNIPER_TRUCKLOAD_TO>(TOList[i]).ExecuteCommand();
                        ProgressWin.ReturnData = new ProgressReturnData() { IsFinish = false, Total = TOList.Count, Index = i + 1, Percent = Math.Ceiling((double)(i + 1 / TOList.Count * 100)).ToString() + "%" };
                        ProgressWin.OutInputs.Add(new DisplayOutPut()
                        {
                            DisplayType = UIOutputType.Text.ToString(),
                            Name = TOList[i].TO_NO,
                            Value = "Monthly!SAP 311 In Shipout Confirm!"
                        });
                        ProgressWin.GetUiInput(Station.API, UIInput.Progress);
                    }
                    else
                    {
                        ProgressWin.OutInputs.Add(new DisplayOutPut()
                        {
                            DisplayType = UIOutputType.Text.ToString(),
                            Name = TOList[i].TO_NO,
                            Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",Begin call RFC!"
                        });
                        ProgressWin.GetUiInput(Station.API, UIInput.Progress);
                        msg = Gt(StrBU, StrTO, StrPlant, Station.SFCDB, out dt);

                        if (msg.StartsWith("OK"))
                        {
                            R_MES_LOG log = new R_MES_LOG()
                            {
                                ID = MesDbBase.GetNewID<R_MES_LOG>(logdb.ORM, StrBU),
                                PROGRAM_NAME = "TruckLoadGT",
                                CLASS_NAME = "MESStation.Stations.StationActions.ActionRunners",
                                FUNCTION_NAME = "ShippingAction.TruckLoadCall311",
                                MAILFLAG = "N",
                                LOG_MESSAGE = msg,
                                DATA1 = StrTO,
                                EDIT_EMP = Station.LoginUser.EMP_NO,
                                EDIT_TIME = DateTime.Now,
                                DATA4 = "Y"
                            };
                            logdb.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                            TOList[i].CLOSED = "2";
                            logdb.ORM.Updateable<R_JUNIPER_TRUCKLOAD_TO>(TOList[i]).ExecuteCommand();
                            ProgressWin.ReturnData = new ProgressReturnData() { IsFinish = false, Total = TOList.Count, Index = i + 1, Percent = Math.Ceiling((double)(i + 1 / TOList.Count * 100)).ToString() + "%" };
                            ProgressWin.OutInputs.Add(new DisplayOutPut()
                            {
                                DisplayType = UIOutputType.Text.ToString(),
                                Name = TOList[i].TO_NO,
                                Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "," + msg,
                            });
                            ProgressWin.GetUiInput(Station.API, UIInput.Progress);
                        }
                        else
                        {
                            R_MES_LOG log = new R_MES_LOG()
                            {
                                ID = MesDbBase.GetNewID<R_MES_LOG>(logdb.ORM, StrBU),
                                PROGRAM_NAME = "TruckLoadGT",
                                CLASS_NAME = "MESStation.Stations.StationActions.ActionRunners",
                                FUNCTION_NAME = "ShippingAction.TruckLoadCall311",
                                MAILFLAG = "N",
                                LOG_MESSAGE = msg,
                                DATA1 = StrTO,
                                EDIT_EMP = Station.LoginUser.EMP_NO,
                                EDIT_TIME = DateTime.Now,
                                DATA4 = "N"
                            };
                            logdb.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                            ProgressWin.ReturnData = new ProgressReturnData() { IsFinish = false, Total = TOList.Count, Index = i + 1, Percent = Math.Ceiling((double)(i + 1 / TOList.Count * 100)).ToString() + "%" };
                            ProgressWin.OutInputs.Add(new DisplayOutPut()
                            {
                                DisplayType = UIOutputType.Text.ToString(),
                                Name = TOList[i].TO_NO,
                                Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ",FAIL," + msg,
                            });
                            ProgressWin.OutInputs.Add(new DisplayOutPut()
                            {
                                DisplayType = UIOutputType.Table.ToString(),
                                Name = TOList[i].TO_NO,
                                Value = dt,
                            });
                            ProgressWin.GetUiInput(Station.API, UIInput.Progress);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage("FAIL" + ex + msg);
            }
            finally
            {
                Station.DBS["SFCDB"].Return(logdb);
                ProgressWin.ReturnData = new ProgressReturnData() { IsFinish = true, Total = TOList.Count, Index = TOList.Count, Percent = "100%" };
                ProgressWin.OutInputs.Add(new DisplayOutPut()
                {
                    DisplayType = UIOutputType.Text.ToString(),
                    Name = "Progress",
                    Value = "End!",
                });
                ProgressWin.GetUiInput(Station.API, UIInput.Progress);
            }

        }

        private static string Gt(string bu, string TO_NO, string Plant, OleExec SFCDB, out DataTable odt)
        {
            if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
            {
                odt = new DataTable();
                return "This time is monthly,can't BackFlush";
            }
            else
            {
                var dt = SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL, R_PRE_WO_HEAD, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((T, W, S, SP, P1, P2) =>
                     T.PACK_NO == P2.PACK_NO &&
                     W.WO == S.WORKORDERNO &&
                     S.ID == SP.SN_ID &&
                     SP.PACK_ID == P1.ID &&
                     P1.PARENT_PACK_ID == P2.ID)
                .Where((T, W, S, SP, P1, P2) => T.TO_NO == TO_NO)
                .GroupBy((T, W, S, SP, P1, P2) => new { T.TRAILER_NUM, W.GROUPID })
                .Select((T, W, S, SP, P1, P2) => new { T.TRAILER_NUM, W.GROUPID, QUANTITY = SqlSugar.SqlFunc.AggregateCount<string>(S.SN) })
                .ToDataTable();
                ZCMM_NSBG_0013 rfc = new ZCMM_NSBG_0013(bu);

                string postday = InterfacePublicValues.GetPostDate(SFCDB);

                string regex = "(\\d+)/(\\d+)/(\\d+)";
                var m = Regex.Match(postday, regex);
                if (m.Success)
                {
                    postday = m.Groups[3].Value + "-" + m.Groups[1].Value + "-" + m.Groups[2].Value;
                }

                rfc.SetValue(TO_NO, ConfigurationManager.AppSettings[bu + "_TRUCK311FROM"], ConfigurationManager.AppSettings[bu + "_TRUCK311TO"], Plant, postday, dt);
                rfc.CallRFC();
                var flag = rfc.GetValue("O_FLAG");
                odt = rfc.GetTableValue("OUT_TAB");
                if (flag == "0")
                {
                    var doc = rfc.GetValue("O_MBLNR");
                    odt = rfc.GetTableValue("OUT_TAB");
                    return "OK," + doc;
                }
                else
                {
                    var msg = rfc.GetValue("O_MESSAGE");
                    return msg;
                }
            }
        }

        public static void NewTruckLoadTO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession Trailer = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Trailer == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession ToNumberSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (ToNumberSession == null)
            {
                ToNumberSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = Paras[1].VALUE };
                Station.StationSession.Add(ToNumberSession);
            }
            MESStationSession TOListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TOListSession == null)
            {
                TOListSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = Paras[2].VALUE };
                Station.StationSession.Add(TOListSession);
            }
            try
            {
                var trailerNo = Trailer.Value.ToString();
                var truckloadlogic = new TruckLoadLogic(Station.SFCDB.ORM);
                var TONumber = truckloadlogic.GenerateTONumber(trailerNo, Station.LoginUser.EMP_NO, Station.BU);
                var TOList = truckloadlogic.GetOpenTOList(trailerNo).Select(t => t.TO_NO).ToList();
                if (TOList.Count == 0)
                {
                    TOList.Add(TONumber);
                }
                ToNumberSession.Value = TONumber;
                TOListSession.Value = TOList;
                Station.AddMessage("MES00000029", new string[] { "New TO NO.", TONumber + " " }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210329153753") + ex.Message);
            }
        }

        public static void ClosedTruckTOByTrailer(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession Trailer = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Trailer == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            string StrTrailer = "";
            StrTrailer = Trailer.Value.ToString();

            UIInputData O = new UIInputData()
            {
                Timeout = 60000,
                UIArea = new string[] { "35%", "45%" },
                IconType = IconType.Warning,
                Message = "",
                Tittle = "",
                Type = UIInputType.YesNo,
                Name = "",
                ErrMessage = ""
            };
            O.OutInputs.Add(new DisplayOutPut { Name = "Confirm", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"Are you sure you want to close all TO# where the Trailer# is {StrTrailer} ?" });
            var res = O.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
            if (res.ToUpper() != "YES")
            {
                return;
            }
            var trucklogic = new TruckLoadLogic(Station.SFCDB.ORM);
            try
            {
                trucklogic.CloseToNumber(StrTrailer, Station.LoginUser.EMP_NO);
                Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                {
                    Message = "OK",
                    State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                });
            }
            catch (MESReturnMessage e)
            {
                throw e;
            }
        }

        public static void RemoveTruckTO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            string TONO = "";
            TONO = TOSession.Value.ToString();
            var trucklogic = new TruckLoadLogic(Station.SFCDB.ORM);
            try
            {
                trucklogic.RemoveToNumber(TONO);
                Station.StationMessages.Add(new StationMessage()
                {
                    Message = "OK",
                    State = StationMessageState.Pass
                });
            }
            catch (MESReturnMessage e)
            {
                throw e;
            }
        }

        public static void NewPhysicPallet(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession TONumber = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONumber == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession CurrentPallet = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (CurrentPallet == null)
            {
                CurrentPallet = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = Paras[1].VALUE };
                Station.StationSession.Add(CurrentPallet);
            }
            MESStationSession PalletList = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (PalletList == null)
            {
                PalletList = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = Paras[2].VALUE };
                Station.StationSession.Add(PalletList);
            }
            string StrTONO = "";
            StrTONO = TONumber.Value.ToString();

            UIInputData O = new UIInputData()
            {
                Timeout = 60000,
                UIArea = new string[] { "35%", "45%" },
                IconType = IconType.Warning,
                Message = "",
                Tittle = "",
                Type = UIInputType.YesNo,
                Name = "",
                ErrMessage = ""
            };
            O.OutInputs.Add(new DisplayOutPut { Name = "Confirm the packaging type", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"Will it ship in cartons?</br> Select 'No' to generate a normal PalletID!" });
            var ShippingType = O.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
            if (ShippingType == null || ShippingType == "")
            {
                return;
            }
            try
            {
                var trucklogic = new TruckLoadLogic(Station.SFCDB.ORM);
                var newPalletID = trucklogic.GeneratePhysicPallet(ShippingType);
                CurrentPallet.Value = newPalletID;
                var PhysicPallets = trucklogic.GetPhysicPalletList(StrTONO);
                PhysicPallets.Add(newPalletID);
                PalletList.Value = PhysicPallets;
                Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage()
                {
                    Message = "OK",
                    State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                });
            }
            catch (MESReturnMessage e)
            {
                throw e;
            }
        }

    }
}
