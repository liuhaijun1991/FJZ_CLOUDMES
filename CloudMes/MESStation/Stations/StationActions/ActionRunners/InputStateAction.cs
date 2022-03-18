using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System.Collections;
using MESDataObject;
using System.Data;
using MESDBHelper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class InputStateAction
    {
        public static void InputsEnable(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == Paras[i].VALUE.ToString().Trim());
                if (input != null)
                {
                    input.Enable = true;
                }
            }
        }

        public static void InputsDisable(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == Paras[i].VALUE.ToString().Trim());
                if (input != null)
                {
                    input.Enable = false;
                }
            }
        }

        public static void InputsDisableControl(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == Paras[i].SESSION_TYPE);
                if (input != null)
                {
                    input.Visable = Paras[i].VALUE.ToString().Trim().ToUpper()=="TRUE"?true:false;
                }
            }
        }

        public static void SetNextInput(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationInput input = Station.Inputs.Find(t => t.DisplayName == Paras[0].VALUE.ToString().Trim());
            if (input != null)
            {
                Station.NextInput = input;
            }
        }

        public static void ClearInputAndMemory(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ClearFlag = string.Empty;
            string ClearItem = string.Empty;
            string ErrMessage = string.Empty;

            if (Paras.Count < 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] {});
                throw new MESReturnMessage(ErrMessage);
            }

            //獲取清除標誌位，表示是否要進行清除輸入操作
            MESStationSession ClearFlagSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (ClearFlagSession != null)
            {
                //黃楊盛 2018年4月24日14:23:34 NPE就放FALSE
                //ClearFlag = ClearFlagSession.Value.ToString();
                ClearFlag = ClearFlagSession.Value?.ToString() ?? "false";
                Station.StationSession.Remove(ClearFlagSession);
                if (ClearFlag.ToLower().Equals("true"))
                {
                    List<R_Station_Action_Para> MemoryParas=Paras.FindAll(t => t.SESSION_TYPE.Equals("CLEARMEMORY"));
                    List<R_Station_Action_Para> InputParas = Paras.FindAll(t => t.SESSION_TYPE.Equals("CLEARINPUT"));

                    //清除指定session
                    foreach (R_Station_Action_Para para in MemoryParas)
                    {
                        Station.StationSession.Remove(Station.StationSession.Find(t => t.MESDataType.ToUpper().Equals(para.VALUE.ToUpper())));
                    }

                    foreach (R_Station_Action_Para para in InputParas)
                    {
                        //清除所有輸入框的值
                        if (para.VALUE.ToUpper().Equals("ALL"))
                        {
                            foreach (MESStationInput StationInput in Station.Inputs)
                            {
                                StationInput.Value = "";
                            }
                            return;
                        }

                        //清除指定輸入框的值
                        ClearItem = para.VALUE.ToString().ToUpper();
                        MESStationInput input = Station.Inputs.Find(t => t.DisplayName.ToUpper().Equals(ClearItem)
                                                                        || t.Name.ToUpper().Equals(ClearItem));
                        if (input != null)
                        {
                            input.Value = "";
                        }
                    }

                    //ClearFlagSession.Value = "false";
                }
            }

            
        }

        /// <summary>
        /// add by YCX 2020.07.08  Clear Input or Memory Unconditional
        /// 無條件清除設定的StationSession.Value/StationSession/Input.Value或者判斷設定的StationSession的當前值執行清除StationSession.Value/StationSession/Input.Value操作.
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ClearStationSessionOrInputData(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            string strClearCtrlStr = Paras[0].VALUE.ToString();
            string strSessionValueSetEmpty = Paras[1].VALUE == null ? "" : Paras[1].VALUE.ToString();
            string strSessionValueSetNull = Paras[2].VALUE == null ? "" : Paras[2].VALUE.ToString();
            string strSessionSetNull = Paras[3].VALUE == null ? "" : Paras[3].VALUE.ToString();
            string strInputSetEmpty = Paras[4].VALUE == null ? "" : Paras[4].VALUE.ToString();
            //如果指定的StationSession出現設定的值就清除設定的StationSession.
            MESStationSession ClearCtrlSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            string strClearCtrlSessionValue = "";
            if (ClearCtrlSession != null)
            {
                strClearCtrlSessionValue = ClearCtrlSession.Value as string;
            }

            bool runflag = false;
            if (strClearCtrlStr.Length > 0 && strClearCtrlStr == strClearCtrlSessionValue)
            {
                runflag = true;
            }
            else if (strClearCtrlStr.Length == 0)
            {
                runflag = true;
            }

            if (runflag)
            {
                List<MESStationSession> listStationSession = null;
                if (strSessionValueSetEmpty.Length > 0)
                {
                    string[] tmpary = strSessionValueSetEmpty.Split(',');
                    listStationSession = (from m in Station.StationSession
                                          from n in tmpary
                                          where n.Contains(m.MESDataType)
                                          select m).ToList();
                    foreach (MESStationSession messession in listStationSession)
                    {
                        messession.Value = "";
                    }
                }
                if (strSessionValueSetNull.Length > 0)
                {
                    string[] tmpary = strSessionValueSetNull.Split(',');
                    listStationSession = (from m in Station.StationSession
                                          from n in tmpary
                                          where n.Contains(m.MESDataType)
                                          select m).ToList();
                    foreach (MESStationSession messession in listStationSession)
                    {
                        messession.Value = null;
                    }
                }
                if (strSessionSetNull.Length > 0)
                {
                    string[] tmpary = strSessionSetNull.Split(',');
                    listStationSession = (from m in Station.StationSession
                                          from n in tmpary
                                          where n.Contains(m.MESDataType)
                                          select m).ToList();
                    foreach (MESStationSession messession in listStationSession)
                    {
                        Station.StationSession.Remove(messession);
                    }
                }

                if (strInputSetEmpty.Length > 0)
                {
                    string[] tmpary = strInputSetEmpty.Split(',');
                    List<MESStationInput> listStationInput = (from m in Station.Inputs
                                                              from n in tmpary
                                                              where n.Contains(m.DisplayName) || n.Contains(m.Name)
                                                              select m).ToList();
                    foreach (MESStationInput input in listStationInput)
                    {
                        input.Value = "";
                    }
                }
            }
        }

        /// <summary>
        /// add by fgg 2018.9.21   Clear Input And Memory Unconditional
        /// 無條件刪除指定input,memory
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ClearInputAndMemoryUnconditional(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            string ClearItem = string.Empty;
            List<R_Station_Action_Para> MemoryParas = Paras.FindAll(t => t.SESSION_TYPE.Equals("CLEARMEMORY"));
            List<R_Station_Action_Para> InputParas = Paras.FindAll(t => t.SESSION_TYPE.Equals("CLEARINPUT"));

            //清除指定session
            foreach (R_Station_Action_Para para in MemoryParas)
            {
                Station.StationSession.Remove(Station.StationSession.Find(t => t.MESDataType.ToUpper().Equals(para.VALUE.ToUpper())));
            }

            foreach (R_Station_Action_Para para in InputParas)
            {
                //清除所有輸入框的值
                if (para.VALUE != null && para.VALUE.ToUpper().Equals("ALL"))
                {
                    foreach (MESStationInput StationInput in Station.Inputs)
                    {
                        StationInput.Value = "";
                    }
                    return;
                }

                //清除指定輸入框的值
                ClearItem = para.VALUE == null ? "" : para.VALUE.ToString().ToUpper();
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName.ToUpper().Equals(ClearItem) || t.Name.ToUpper().Equals(ClearItem)); 
                if (input != null)
                {
                    input.Value = "";
                }
            }
        }


        public static void SetPassOrFailInOba(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE.ToString());
            MESStationInput failSnInput = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE.ToString());
            MESStationInput FailCodeInput = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE.ToString());
            MESStationInput LocationInput = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE.ToString());
            MESStationInput FailDescInput = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE.ToString());
            

            if (Input.Value.ToString().ToUpper().Equals("PASS"))
            {
                snInput.Visable = true; ;
                failSnInput.Visable = false;
                FailCodeInput.Visable = false;
                LocationInput.Visable = false;
                FailDescInput.Visable = false;
                Station.NextInput = snInput;
            }
            else
            {
                snInput.Visable = false; ;
                failSnInput.Visable = true;
                Station.NextInput = failSnInput;
            }
        }

        public static void SetScanTypeInSuperMarketIN(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE.ToString());
            MESStationInput OutsourcingSnInput = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE.ToString());
            //MESStationInput FailCodeInput = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE.ToString());
            //MESStationInput LocationInput = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE.ToString());
            //MESStationInput FailDescInput = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE.ToString());


            if (Input.Value.ToString().ToUpper().Equals("MAKEPARTNO"))
            {
                snInput.Visable = true; ;
                OutsourcingSnInput.Visable = false;
                //FailCodeInput.Visable = false;
                //LocationInput.Visable = false;
                //FailDescInput.Visable = false;
                Station.NextInput = snInput;
            }
            else
            {
                snInput.Visable = false; ;
                OutsourcingSnInput.Visable = true;
                Station.NextInput = OutsourcingSnInput;
            }
        }

        public static void SetWidgetStatusByConditional(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession Conditional = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (Conditional == null || Conditional.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (Conditional.Value.ToString() == Paras[0].VALUE)
            {
                for (int i = 1; i < Paras.Count; i++)
                {
                    MESStationInput stationInput = Station.Inputs.Find(inp => inp.DisplayName == Paras[i].SESSION_TYPE);
                    if (stationInput != null)
                    {
                        string Types = Paras[i].SESSION_KEY.ToUpper();
                        switch (Types)
                        {
                            case "VISABLE":
                                if (Paras[i].VALUE.ToUpper() == "TRUE")
                                {
                                    stationInput.Visable = true;
                                }
                                else
                                {
                                    stationInput.Visable = false;
                                }
                                break;
                            case "ENABLE":
                                if (Paras[i].VALUE.ToUpper() == "TRUE")
                                {
                                    stationInput.Enable = true;
                                }
                                else
                                {
                                    stationInput.Enable = false;
                                }
                                break;
                            case "CLEAR":
                                stationInput.Value = null;
                                if (Paras[i].VALUE.ToUpper() == "TRUE")
                                {
                                    stationInput.Enable = true;
                                }
                                else
                                {
                                    stationInput.Enable = false;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static void CompareSessionValue(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession Session1 = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (Session1 == null || Session1.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession Session2 = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[1].SESSION_TYPE) && t.SessionKey.Equals(Paras[1].SESSION_KEY));
            if (Session2 == null || Session2.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (Session1.Value != Session2.Value)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else {
                Station.AddMessage("MSGCODE20190307080548", new string[] { "MPN"}, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 清空所有StationInput值
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ClearInputsValue(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {           
            foreach (var item in Paras)
            {
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == item.VALUE.ToString().Trim());
                input.Value = "";
            }
        }

        /// <summary>
        /// Set Inputs Visable By Session Value and paras value
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SetInputsVisableBySessionValue(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession session = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (session == null || session.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationInput stationInput = null;
            for (int i = 1; i < Paras.Count; i++)
            {
                stationInput = Station.Inputs.Find(inp => inp.DisplayName == Paras[i].VALUE);
                if (stationInput != null )
                {
                    if (Paras[0].VALUE.Equals(session.Value.ToString()))
                    {
                        stationInput.Visable = true;
                    }
                    else
                    {
                        stationInput.Visable = false;
                    }
                }
            }          
        }
    }
}
