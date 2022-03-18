using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class DataInputLoader
    {

        /// <summary>
        /// 判斷是否為整數，若為整數保存到指定位置，否則提示報錯
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">
        /// IntSavePoint：保存到指定位置
        /// </param>
        public static void IntegerDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }

            string IntData = Input.Value.ToString();

            long Num;
            if (IntData == "")
            {
                Station.AddMessage("MES00000006", new string[] { "InputData" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                //Station.NextInput = Station.Inputs[2];
            }
            else if (long.TryParse(IntData.ToString(), out Num) && long.Parse(IntData) >= 0)
            {
                s.Value = IntData;
                Station.AddMessage("MES00000029", new string[] { "InputData", Num.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                //Station.NextInput = Station.Inputs[3];
            }
            else
            {
                Station.AddMessage("MES00000020", new string[] { "InputData", "Number" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                //Station.NextInput = Station.Inputs[2];
            }
        }
        /// <summary>
        /// 判斷是否為整數，若為整數保存到指定位置，否則抛出异常
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void IntegerExDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }

            string IntData = Input.Value.ToString();

            long Num;
            if (IntData == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "InputData:" + MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail }));
            }
            else if (long.TryParse(IntData.ToString(), out Num) && long.Parse(IntData) >= 0)
            {
                s.Value = IntData;
                Station.AddMessage("MES00000029", new string[] { "InputData", Num.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000020", new string[] { IntData + "     InputData Fail  :", "Number" }));
            }
        }
        /// <summary>
        /// 加載輸入的字符串到指定的 MESStationSession
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">      
        /// </param>
        public static void InputDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[i].SESSION_TYPE && t.SessionKey == Paras[i].SESSION_KEY);
                if (s == null)
                {
                    s = new MESStationSession() { MESDataType = Paras[i].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[i].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(s);
                }
                s.Value = Input.Value.ToString().Trim().ToUpper();
                s.InputValue = Input.Value.ToString().Trim().ToUpper();
                s.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { Paras[i].SESSION_TYPE, Input.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 加載輸入的密碼到指定的 MESStationSession
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PwdInputDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[i].SESSION_TYPE && t.SessionKey == Paras[i].SESSION_KEY);
                if (s == null)
                {
                    s = new MESStationSession() { MESDataType = Paras[i].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[i].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(s);
                }
                s.Value = Input.Value.ToString();
                s.InputValue = Input.Value.ToString();
                s.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { Paras[i].SESSION_TYPE, "" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 加載一個字符串特定格式到指定位置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">
        /// StrSavePoint:字符串保存位置；
        /// Change：0轉小寫,1轉大寫,2保持不變；
        /// Trim：0不變,1 去前空,2去后空,3前後去空；
        /// </param>
        public static void StringDataloader(MESPubLab.MESStation.MESStationBase Station,
            MESPubLab.MESStation.MESStationInput Input,
            List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            //if (Paras.Count != 3)
            //{
            //    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            //}
            //bool isTrimEnd = false;
            for (int i = 0; i < Paras.Count; i++)
            {

                if (Paras[i].SESSION_TYPE == "Change")
                {
                    if (Paras[i].VALUE == "0")
                    {
                        Input.Value = Input.Value.ToString().ToLower();
                    }
                    else if (Paras[i].VALUE == "1")
                    {
                        Input.Value = Input.Value.ToString().ToUpper();
                    }
                    //else if (Paras[i].VALUE != "0"&& Paras[i].VALUE != "1"&& Paras[i].VALUE != "2")
                    //{
                    //    Station.AddMessage("MES00000020", new string[] { "SESSION_TYPE为\"Change\"的", "0,1,2" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    //}
                }
                else if (Paras[i].SESSION_TYPE == "Trim")
                {
                    if (Paras[i].VALUE == "1")
                    {
                        Input.Value = Input.Value.ToString().TrimStart();
                    }
                    else if (Paras[i].VALUE == "2")
                    {
                        Input.Value = Input.Value.ToString().TrimEnd();
                    }
                    else if (Paras[i].VALUE == "3")
                    {
                        Input.Value = Input.Value.ToString().Trim();
                    }
                    //else if (Paras[i].VALUE != "0" && Paras[i].VALUE != "1" && Paras[i].VALUE != "2" && Paras[i].VALUE != "3")
                    //{
                    //    Station.AddMessage("MES00000020", new string[] { "SESSION_TYPE为\"Trim\"的", "0,1,2,3" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                    //}
                }
                else if (Paras[i].SESSION_TYPE == "TrimEnd")
                {
                    //if (isTrimEnd)
                    //{
                    //    continue;
                    //}
                    string trimStr = Paras[i].VALUE;
                    string strInput = Input.Value.ToString();
                    if (strInput.EndsWith(trimStr))
                    {
                        strInput = strInput.Substring(0, strInput.Length - trimStr.Length);
                        Input.Value = strInput;
                    }
                    //isTrimEnd = true;
                }
                else if (Paras[i].SESSION_TYPE != null && Paras[i].SESSION_TYPE.ToUpper() == "REMOVEPREFIX")
                {
                    //if (isTrimEnd)
                    //{
                    //    continue;
                    //}
                    string Prefix = Paras[i].VALUE;
                    string strInput = Input.Value.ToString();
                    if (strInput.StartsWith(Prefix))
                    {
                        strInput = strInput.Substring(Prefix.Length);
                        Input.Value = strInput;
                    }
                    //isTrimEnd = true;
                }
                else if (Paras[i].SESSION_TYPE != null && Paras[i].SESSION_TYPE.ToUpper() == "REMOVESUFFIX")
                {
                    string strInput = Input.Value.ToString();
                    int position = strInput.IndexOf(' ');
                    if (position <= 0) { continue; }
                    strInput = strInput.Substring(0, position);

                    var customer = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL, R_WO_BASE, C_SKU, C_SERIES, C_CUSTOMER>((r, wo, sku, se, cus) => r.WORKORDERNO == wo.WORKORDERNO && wo.SKUNO == sku.SKUNO && sku.C_SERIES_ID == se.ID && se.CUSTOMER_ID == cus.ID)
                                                        .Where((r, wo, sku, se, cus) => r.SN == strInput && (r.STATION_NAME == "KIT_PRINT" || r.STATION_NAME == "KIT_REPRINT" || r.STATION_NAME == "SILOADING") && cus.CUSTOMER_NAME.ToUpper() == MESDataObject.Constants.Customer.ARUBA.Ext<EnumExtensions.EnumValueAttribute>().Description)
                                                        .Select((r, wo, sku, se, cus) => cus).ToList();
                    if (customer != null && customer.Count > 0)
                    {
                        Input.Value = strInput;
                    }
                }
            }
            for (int j = 0; j < Paras.Count; j++)
            {
                if (Paras[j].SESSION_TYPE != "Change" && Paras[j].SESSION_TYPE != "Trim" && (Paras[j].SESSION_TYPE != null && Paras[j].SESSION_TYPE.ToUpper() != "REMOVEPREFIX") && (Paras[j].SESSION_TYPE != null && Paras[j].SESSION_TYPE.ToUpper() != "REMOVESUFFIX"))
                {
                    MESStationSession StrInput = Station.StationSession.Find(t => t.MESDataType == Paras[j].SESSION_TYPE && t.SessionKey == Paras[j].SESSION_KEY);
                    if (StrInput == null)
                    {
                        StrInput = new MESStationSession() { MESDataType = Paras[j].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[j].SESSION_KEY, ResetInput = Input };
                        Station.StationSession.Add(StrInput);
                    }
                    StrInput.InputValue = Input.Value.ToString();
                    StrInput.ResetInput = Input;
                    StrInput.Value = Input.Value.ToString();
                    //Station.NextInput = Station.Inputs[0];
                    break;
                }
            }
        }

        /// <summary>
        /// 從輸入加載ErrorCode
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">輸入的ErrorCode轉換為大寫</param>
        /// <param name="Paras">ActionCode</param>
        public static void ErrorCodeDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrorCodeInput = "";
            MESStationSession strInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (strInput == null)
            {
                //Station.AddMessage("MES00000076", new string[] { "Sn", Sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                ErrorCodeInput = strInput.Value.ToString();
            }
            List<C_ERROR_CODE> errorCodes = Station.SFCDB.ORM.Queryable<C_ERROR_CODE>().Where((e) => e.ERROR_CODE == ErrorCodeInput).ToList();
            if (errorCodes.Count <= 0)
            {
                Station.NextInput = Input;
                Station.AddMessage("MES00000007", new string[] { "ErrorCode", ErrorCodeInput }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                C_ERROR_CODE errorobj = errorCodes[0];
                MESStationSession ErrorDesc = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (ErrorDesc == null)
                {
                    ErrorDesc = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = errorobj.ENGLISH_DESC, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(ErrorDesc);
                }
                ErrorDesc.Value = errorobj.ENGLISH_DESC;
                strInput.Value = errorobj.ERROR_CODE;
            }
        }

        /// <summary>
        /// 從輸入加載ActionDesc
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">輸入的ActionCode轉換為大寫</param>
        /// <param name="Paras">ActionCode</param>
        public static void ActionDescDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession strInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (strInput == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }


            List<C_ACTION_CODE> Actions = Station.SFCDB.ORM.Queryable<C_ACTION_CODE>().Where((A) => A.ACTION_CODE == strInput.Value.ToString()).ToList();

            if (Actions.Count <= 0)
            {
                Station.NextInput = Input;
                Station.AddMessage("MES00000007", new string[] { "ActionCode", strInput.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                MESStationSession ActionDesc = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (ActionDesc == null)
                {
                    ActionDesc = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(ActionDesc);
                }
                ActionDesc.Value = Actions[0].CHINESE_DESC;
            }

        }


        /// <summary>
        /// 從輸入加載ActionCode
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">輸入的ActionCode轉換為大寫</param>
        /// <param name="Paras">ActionCode</param>
        public static void ActionCodeDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string ActionCodeInput = "";//Station.StationSession[0].Value.ToString();
            MESStationSession strInput = Station.StationSession.Find(t => t.MESDataType == "StrSavePoint" && t.SessionKey == "1");
            if (strInput == null)
            {
                //Station.AddMessage("MES00000076", new string[] { "Sn", Sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                ActionCodeInput = strInput.Value.ToString();
            }

            string strSql = $@"SELECT * FROM C_ACTION_CODE WHERE ACTION_CODE = '{ActionCodeInput.Replace("'", "''")}'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":ActionCode", ActionCodeInput) };
            DataTable res = Station.SFCDB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count <= 0)
            {
                Station.NextInput = Input;
                Station.AddMessage("MES00000007", new string[] { "ActionCode", ActionCodeInput }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
            else
            {
                MESStationSession ActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (ActionCode == null)
                {
                    ActionCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(ActionCode);
                }
                ActionCode.Value = ActionCodeInput;
            }

        }

        /// <summary>
        /// 從輸入加載RootCause 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RootCauseDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string RootCauseInput = Input.Value.ToString();// = Station.StationSession[0].Value.ToString();
            //Modify by LLF 2018-02-03
            //MESStationSession strInput = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (strInput == null)
            //{
            //    //Station.AddMessage("MES00000076", new string[] { "Sn", Sn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            //}
            //else
            //{
            //    RootCauseInput = strInput.Value.ToString();
            //}
            //string strSql = $@"SELECT * FROM C_ERROR_CODE WHERE ERROR_CODE = '{RootCauseInput.Replace("'", "''")}'";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":RootCause", RootCauseInput) };
            //DataTable res = Station.SFCDB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            MESStationSession ErrorCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (ErrorCode == null)
            {
                ErrorCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(ErrorCode);
            }

            T_C_ERROR_CODE Obj_C_ERROR_CODE = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_ERROR_CODE ObjErrorCode = Obj_C_ERROR_CODE.GetByErrorCode(RootCauseInput, Station.SFCDB);

            if (ObjErrorCode == null)
            {
                ErrorCode.Value = null;
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { RootCauseInput }));
            }
            else
            {
                ErrorCode.Value = RootCauseInput;
                Station.Inputs[Station.Inputs.Count - 1].Value = ObjErrorCode.ENGLISH_DESC.ToString();
            }
            //Modify by LLF 2018-02-03
            //if (res.Rows.Count <= 0)
            //{
            //    Station.NextInput = Input;
            //    Station.AddMessage("MES00000007", new string[] { "RootCause", RootCauseInput }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            //}
            //else
            //{
            //    MESStationSession ErrorCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //    if (ErrorCode == null)
            //    {
            //        ErrorCode = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            //        Station.StationSession.Add(ErrorCode);
            //    }
            //    ErrorCode.Value = RootCauseInput;
            //}

        }

        /// <summary>
        /// 從輸入加載維修大項 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RepairItemsDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession RepairItemsSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (RepairItemsSession == null)
            {
                RepairItemsSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(RepairItemsSession);
            }
            RepairItemsSession.Value = Input.Value.ToString();
            //  Input.DataForUse.Add(Input.Value.ToString());
        }
        public static void StationItemsDataloader(MESStationBase Station, MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession ReturnStationItemsSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (ReturnStationItemsSession == null)
            {
                ReturnStationItemsSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(ReturnStationItemsSession);
            }
            //Only update station when reverse, which means there is station input vince_20200219
            if (Input.Value.ToString() != "")
            {
                ReturnStationItemsSession.Value = Input.Value.ToString();
                //  Input.DataForUse.Add(Input.Value.ToString());
            }
        }

        public static void LoadUnitStationStatus(MESStationBase Station, MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession CURR_STATION = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            MESStationSession NEXT_STATION = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SNSession == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            if (CURR_STATION == null)
            {
                CURR_STATION = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(CURR_STATION);
            }
            if (NEXT_STATION == null)
            {
                NEXT_STATION = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(NEXT_STATION);
            }
            string StrSn = "";
            StrSn = SNSession.InputValue.ToString();
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            R_SN snObject = t_r_sn.GetSN(StrSn, Station.SFCDB);
            CURR_STATION.Value = snObject.CURRENT_STATION;
            NEXT_STATION.Value = snObject.NEXT_STATION;



        }

        /// <summary>
        /// 從輸入加載維修小項 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RepairItemsSonDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession RepairItemsSonSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (RepairItemsSonSession == null)
            {
                RepairItemsSonSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(RepairItemsSonSession);
            }
            RepairItemsSonSession.Value = Input.Value.ToString();

        }

        /// <summary>
        /// 從維修大項加載維修小項 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RepairItemsSonFromRepairItemsDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            T_C_REPAIR_ITEMS RepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RowItems;
            T_C_REPAIR_ITEMS_SON RepairItemsSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            List<string> RepairItemsSonList = new List<string>();
            List<string> RepairItemsList = new List<string>();
            T_C_REPAIR_ITEMS TC_REPAIR_ITEM = new T_C_REPAIR_ITEMS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string ITEM_NAME = Input.Value.ToString();

            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "Son_Items");
            List<object> ret = I.DataForUse;
            ret.Clear();

            try
            {

                //RepairItemsList = TC_REPAIR_ITEM.GetRepairItemsList(ITEM_NAME, Station.SFCDB);
                //Input.DataForUse.Add(RepairItemsList);//初始化維修大項
                RowItems = RepairItems.GetIDByItemName(ITEM_NAME, Station.SFCDB);

                RepairItemsSonList = RepairItemsSon.GetRepairItemsSonList(RowItems.ID, Station.SFCDB);
                //ret.Add(RepairItemsSonList);
                //添加維修小項
                foreach (object item in RepairItemsSonList)
                {
                    ret.Add(item);
                }


                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }
        }

        /// <summary>
        /// 初始化工站時加載出默認的維修大項，維修小項LIST
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input">RootCause輸入值轉換為大寫</param>
        /// <param name="Paras">ErrorCode</param>
        public static void RepairItemsInitDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            T_C_REPAIR_ITEMS RepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RowItems;
            T_C_REPAIR_ITEMS_SON RepairItemsSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            List<string> RepairItemsSonList = new List<string>();
            List<string> RepairItemsList = new List<string>();
            T_C_REPAIR_ITEMS TC_REPAIR_ITEM = new T_C_REPAIR_ITEMS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string ITEM_NAME = Input.Value.ToString();

            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "Son_Items");

            try
            {
                Input.DataForUse.Clear();
                //RepairItemsList = TC_REPAIR_ITEM.GetRepairItemsList(ITEM_NAME, Station.SFCDB);
                //Input.DataForUse.Add(RepairItemsList);//初始化維修大項
                RowItems = RepairItems.GetIDByItemName(ITEM_NAME, Station.SFCDB);

                RepairItemsSonList = RepairItemsSon.GetRepairItemsSonList(RowItems.ID, Station.SFCDB);
                Input.DataForUse.Add(RepairItemsSonList);   //初始化維修小項

                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }

        }

        /// <summary>
        /// 從輸入加載FailDesc
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void FailDescDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession FailDescSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (FailDescSession == null)
            {
                FailDescSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FailDescSession);
            }
            FailDescSession.Value = Input.Value.ToString();
        }

        /// <summary>
        /// 加載FailList
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFailCollectDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string StrSn = "";
            string StrLocation = "";
            string StrProcess = "";
            string StrFailCode = "";
            string StrFailDesc = "";
            List<Dictionary<string, string>> FailList = new List<Dictionary<string, string>>();
            Dictionary<string, string> FailInfo = null;
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            //獲取Fail SN
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //StrSn = SNSession.Value.ToString();
            StrSn = SNSession.InputValue.ToString();

            //獲取Fail Location
            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LocationSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrLocation = LocationSession.Value.ToString();

            //獲取Fail Process
            MESStationSession ProcessSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ProcessSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrProcess = ProcessSession.Value.ToString();

            //獲取FailCode
            MESStationSession FailCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (FailCodeSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            if (FailCodeSession.Value is C_ERROR_CODE)
            {
                StrFailCode = ((C_ERROR_CODE)FailCodeSession.Value).ERROR_CODE;
            }
            else
            {
                StrFailCode = FailCodeSession.Value.ToString();
            }

            //獲取Fail Description
            MESStationSession FailDescSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (FailDescSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrFailDesc = FailDescSession.Value.ToString();

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (FailListSession == null)
            {
                FailListSession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FailListSession);
            }
            else
            {
                FailList = (List<Dictionary<string, string>>)FailListSession.Value;
            }

            FailInfo = new Dictionary<string, string>();
            //FailList=new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> failure in FailList)
            {

            }

            //            FailList.Select((dic) => {
            //                dic["FailProcess"].Equals(StrProcess) &&
            //dic["FailLocation"].Equals(StrLocation) && dic["FailCode"].Equals(StrFailCode); });   

            //add by ZGJ 2018-03-15
            //檢查當前不良信息是否已經存在於已輸入中
            Dictionary<string, string> ExistFailInfo = FailList.Find((dic) =>
            {
                return (dic["FailProcess"].Equals(StrProcess)
                    && dic["FailLocation"].Equals(StrLocation)
                    && dic["FailCode"].Equals(StrFailCode));
            });

            if (ExistFailInfo == null)
            {
                FailInfo.Add("FailLocation", StrLocation);
                FailInfo.Add("FailProcess", StrProcess);
                FailInfo.Add("FailDesc", StrFailDesc);
                FailInfo.Add("FailCode", StrFailCode);
                FailList.Add(FailInfo);
                FailListSession.Value = FailList;
            }
            else
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000163", new string[] { StrProcess, StrLocation, StrFailCode });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        ///Add by LLF 2018-01-28 AOI1&AOI2,Print1&Print2,VI1&VI2  工站可以相互轉換
        public static void ChangeCurrentStationDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string StrStation = Paras[0].VALUE.ToString();
            string StrChangeStation = Paras[1].VALUE.ToString();

            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NextStationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            List<string> ListNextStation = new List<string>();
            ListNextStation = (List<string>)NextStationSession.Value;

            if (Station.StationName == StrStation && Station.StationName != StrChangeStation && ListNextStation.Contains(StrChangeStation))
            {
                Station.StationName = StrChangeStation;
            }
            else if (Station.StationName == StrChangeStation && Station.StationName != StrStation && ListNextStation.Contains(StrStation))
            {
                Station.StationName = StrStation;
            }



            Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        public static void SNSampleFailInfoDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string StrSn = "";
            string StrLocation = "";
            string StrFailCode = "";
            string StrFailDesc = "";
            string[] FailInfo = new string[3];
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            //獲取Fail SN
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrSn = SNSession.Value.ToString();

            //獲取FailCode
            MESStationSession FailCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCodeSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrFailCode = FailCodeSession.Value.ToString();

            //獲取Fail Description
            MESStationSession FailDescSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FailDescSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrFailDesc = FailDescSession.Value.ToString();

            //獲取Fail Location
            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (LocationSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            StrLocation = LocationSession.Value.ToString();

            MESStationSession FailListSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (FailListSession == null)
            {
                FailListSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FailListSession);
            }

            FailInfo[0] = StrFailCode;
            FailInfo[1] = StrLocation;
            FailInfo[2] = StrFailDesc;
            FailListSession.Value = FailInfo;
        }

        /// <summary>
        ///從輸入框加載數據到下拉框
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputValueLoaderToSelectList(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strPanel = "";
            List<string> PackList = new List<string>();
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                PackNoSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                PackNoSession.InputValue = Input.Value.ToString();
                Station.StationSession.Add(PackNoSession);
            }
            strPanel = PackNoSession.InputValue.ToString();

            MESStationSession PackListSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackListSession == null)
            {
                PackListSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(PackListSession);
            }
            else
            {
                PackList = (List<string>)PackListSession.Value;
            }
            PackList.Add(strPanel);
            PackListSession.Value = PackList;
            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
            s.DataForUse.Clear();
            foreach (var VARIABLE in PackList)
            {
                s.DataForUse.Add(VARIABLE);
            }
            Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE).Value = "";
        }

        public static void ObaSampleStationInit(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE);
            MESStationInput scanTypeInput = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
            MESStationInput failSnInput = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE);
            MESStationInput FailCodeInput = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE);
            MESStationInput LocationInput = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE);
            MESStationInput FailDescInput = Station.Inputs.Find(t => t.DisplayName == Paras[5].SESSION_TYPE);

            snInput.Visable = false;
            failSnInput.Visable = false;
            FailCodeInput.Visable = false;
            LocationInput.Visable = false;
            FailDescInput.Visable = false;
            scanTypeInput.Visable = false;
            scanTypeInput.DataForUse.Add("Pass");
            scanTypeInput.DataForUse.Add("Fail");
        }

        public static void SuperMarketINStationInit(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE);
            MESStationInput ChooseTypeInput = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
            MESStationInput OutsourcingSnInput = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE);

            snInput.Visable = false;
            OutsourcingSnInput.Visable = false;
            ChooseTypeInput.DataForUse.Add("MakePartno");
            ChooseTypeInput.DataForUse.Add("BuyPartno");
        }

        public static void LoadSampleLotByPackNo(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            string packNo = Input.Value.ToString();
            #region 用於界面上顯示的批次信息
            R_LOT_STATUS rLotStatus = new R_LOT_STATUS();
            List<R_LOT_PACK> rLotPackList = new List<R_LOT_PACK>();
            #endregion
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_PACK rRLotPack = new T_R_LOT_PACK(Station.SFCDB, Station.DBType);
            List<R_LOT_STATUS> rLotStatusList = tRLotStatus.getSampleLotByPackNo(packNo, Station.SFCDB);

            if (rLotStatusList.FindAll(t => t.CLOSED_FLAG == "1").Count > 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529105040", new string[] { packNo }));
            }
            else if (rLotStatusList.FindAll(t => t.CLOSED_FLAG == "0").Count > 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529111019", new string[] { packNo, rLotStatusList.Find(t => t.CLOSED_FLAG == "0").LOT_NO }));
            }
            else if (rLotStatusList.FindAll(t => t.CLOSED_FLAG != "2").Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529111245", new string[] { packNo }));
            }

            rLotStatus = rLotStatusList.Find(t => t.CLOSED_FLAG == "1");
            rLotPackList = rRLotPack.GetRLotPackByLotNo(Station.SFCDB, rLotStatus.LOT_NO);

            #region 加載界面信息
            MESStationSession lotNoSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
            MESStationSession skuNoSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
            MESStationSession aqlSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
            MESStationSession lotQtySession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
            MESStationSession sampleQtySession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
            MESStationSession rejectQtySession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
            MESStationSession sampleQtyWithAQLSession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
            MESStationSession passQtySession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
            MESStationSession failQtySession = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };

            Station.StationSession.Clear();
            Station.StationSession.Add(lotNoSession);
            Station.StationSession.Add(skuNoSession);
            Station.StationSession.Add(aqlSession);
            Station.StationSession.Add(lotQtySession);
            Station.StationSession.Add(sampleQtySession);
            Station.StationSession.Add(rejectQtySession);
            Station.StationSession.Add(sampleQtyWithAQLSession);
            Station.StationSession.Add(passQtySession);
            Station.StationSession.Add(failQtySession);

            lotNoSession.Value = rLotStatus.LOT_NO;
            skuNoSession.Value = rLotStatus.SKUNO;
            aqlSession.Value = rLotStatus.AQL_TYPE;
            lotQtySession.Value = rLotStatus.LOT_QTY;
            sampleQtySession.Value = rLotStatus.SAMPLE_QTY;
            rejectQtySession.Value = rLotStatus.REJECT_QTY;
            sampleQtyWithAQLSession.Value = rLotStatus.PASS_QTY + rLotStatus.FAIL_QTY;
            passQtySession.Value = rLotStatus.PASS_QTY;
            failQtySession.Value = rLotStatus.FAIL_QTY;

            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[9].SESSION_TYPE);
            s.DataForUse.Clear();
            foreach (var VARIABLE in rLotPackList)
            {
                s.DataForUse.Add(VARIABLE.PACKNO);
            }

            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == "SN");
            MESStationInput packInput = Station.Inputs.Find(t => t.DisplayName == "PACKNO");
            MESStationInput scanTypeInput = Station.Inputs.Find(t => t.DisplayName == "ScanType");
            packInput.Visable = false;
            snInput.Visable = false;
            scanTypeInput.Visable = true;
            #endregion

        }

        public static void LoadPanelWaitReplaceSn(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionPanel = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPanel == null || sessionPanel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null)
            {
                sessionSN = new MESStationSession { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = "", InputValue = "", ResetInput = Input };
                Station.StationSession.Add(sessionSN);
            }
            T_R_SN r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            sessionSN.Value = r_sn.GetPanelWaitReplaceSn(sessionPanel.InputValue.ToString(), Station.SFCDB);
            Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
        }

        public static void LoadStationPublicValue(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession userSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (userSession == null)
            {
                userSession = new MESStationSession { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(userSession);
            }

            MESStationSession lineSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (lineSession == null)
            {
                lineSession = new MESStationSession { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(lineSession);
            }
            MESStationSession StationName = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StationName == null)
            {
                StationName = new MESStationSession { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(StationName);
            }
            userSession.Value = Station.LoginUser.EMP_NO;
            lineSession.Value = Station.Line;
            StationName.Value = Station.StationName;

        }

        public static void LoadSn(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                SnSession = new MESStationSession { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(SnSession);
            }
            SnSession.Value = Input.Value.ToString();
        }
        public static void LoadLocation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LocationSession == null)
            {
                LocationSession = new MESStationSession { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(LocationSession);
            }
            LocationSession.Value = Input.Value.ToString();
        }

        public static void LoadStationWoPassFailQTY(MESStationBase Station, MESStationInput input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180904135032"));
            }



        }

        public static void CheckOptionLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            MESStationSession CheckFunctionSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (CheckFunctionSession == null)
            {
                CheckFunctionSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = "TRUE" };
                Station.StationSession.Add(CheckFunctionSession);
            }
            else
            {
                CheckFunctionSession.Value = "TRUE";
            }

        }

        public static void ReprintDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession reprintValue = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (reprintValue == null || reprintValue.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession skuSession = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (skuSession == null)
            {
                skuSession = new MESStationSession { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(skuSession);
            }
            MESStationSession labelInputSession = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (labelInputSession == null)
            {
                labelInputSession = new MESStationSession { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(labelInputSession);
            }

            OleExec db = Station.SFCDB;
            string _reprintValue = reprintValue.Value.ToString();
            R_SN snObject = db.ORM.Queryable<R_SN>().Where(r => (r.SN == _reprintValue || r.BOXSN == _reprintValue) && r.VALID_FLAG == "1").ToList().FirstOrDefault();
            R_PACKING palletObject = null;
            R_PACKING cartonObject = null;
            C_SKU skuObject = null;
            string _sku = "";
            Dictionary<string, string> labelInput = new Dictionary<string, string>();
            if (snObject == null)
            {
                palletObject = db.ORM.Queryable<R_PACKING>().Where(r => r.PACK_NO == _reprintValue && r.PACK_TYPE == "PALLET").ToList().FirstOrDefault();
            }
            else
            {
                _sku = snObject.SKUNO;
                labelInput.Add("SN", snObject.SN);
                labelInput.Add("WO", snObject.WORKORDERNO);
            }

            if (palletObject == null)
            {
                cartonObject = db.ORM.Queryable<R_PACKING>().Where(r => r.PACK_NO == _reprintValue && r.PACK_TYPE == "CARTON").ToList().FirstOrDefault();
            }
            else
            {
                _sku = palletObject.SKUNO;
                labelInput.Add("PLNO", palletObject.PACK_NO);
            }

            if (cartonObject == null)
            {
                //throw new MESReturnMessage("輸入類型錯誤");
            }
            else
            {
                _sku = cartonObject.SKUNO;
                labelInput.Add("CARTONNO", cartonObject.PACK_NO);
            }

            skuObject = db.ORM.Queryable<C_SKU>().Where(c => c.SKUNO == _sku).ToList().FirstOrDefault();
            if (skuObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000245", new string[] { _sku }));
            }
            skuSession.Value = skuObject;
            labelInputSession.Value = labelInput;
        }

        /// <summary>
        /// 根據機種加載可以補打的工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReprintStationLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession skuSession = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string inputSelectName = Paras[1].VALUE.ToString();
            C_SKU skuObject = (C_SKU)skuSession.Value;
            List<string> list = new List<string>();
            list = Station.SFCDB.ORM.Queryable<C_SKU_Label>().Where(label => label.SKUNO == skuObject.SKUNO).OrderBy(label => label.STATION)
                .GroupBy(lable => lable.STATION).Select(label => label.STATION).ToList();
            MESStationInput stationInput = Station.Inputs.Find(i => i.DisplayName == inputSelectName);
            if (stationInput == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { inputSelectName }));
            }
            else
            {
                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                stationInput.DataForUse.AddRange(list);
            }
        }
        public static void SkuTransportTypeLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession skuSession = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string inputSelectName = Paras[1].VALUE.ToString();
            string Skuno = skuSession.Value.ToString();
            List<string> list = new List<string>();
            try
            {
                var list1 = Station.SFCDB.ORM.Queryable<O_SKU_PACKAGE>()
                    .Where(t=>t.SKUNO == Skuno && ( t.SCENARIO == "CartonOverpack" || t.SCENARIO == "Multipack") && t.TON != null)
                    .Select(t=> new {t.PARTNO,t.TON }).ToList();
                for (int i = 0; i < list1.Count; i++)
                {
                    list.Add($@"{list1[i].PARTNO}:({list1[i].TON})");
                }

            }
            catch(Exception e)
            {
            
            }
            if (list.Count == 0)
            {
                list = Station.SFCDB.ORM.Queryable<C_PACKING>().Where(p => p.SKUNO == Skuno).OrderBy(p => p.TRANSPORT_TYPE)
                    .GroupBy(p => p.TRANSPORT_TYPE).Select(p => p.TRANSPORT_TYPE).ToList();
            }



            MESStationInput stationInput = Station.Inputs.Find(i => i.DisplayName == inputSelectName);
            if (stationInput == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { inputSelectName }));
            }
            else
            {
                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                stationInput.DataForUse.AddRange(list);
            }
        }

        /// <summary>
        /// 根據工站和機種加載可以補打的LABEL TYPE
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReprintLableTypeLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession reprintStation = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (reprintStation == null || reprintStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession skuSession = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string inputSelectName = Paras[2].VALUE.ToString();
            string _reprintStation = reprintStation.Value.ToString();
            C_SKU skuObject = (C_SKU)skuSession.Value;
            List<string> list = new List<string>();
            list = Station.SFCDB.ORM.Queryable<C_SKU_Label>().Where(label => label.SKUNO == skuObject.SKUNO && label.STATION == _reprintStation).OrderBy(label => label.LABELTYPE)
                    .GroupBy(lable => lable.LABELTYPE).Select(label => label.LABELTYPE).ToList();
            MESStationInput stationInput = Station.Inputs.Find(i => i.DisplayName == inputSelectName);
            if (stationInput == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { inputSelectName }));
            }
            else
            {
                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                stationInput.DataForUse.Add("ALL");
                stationInput.DataForUse.AddRange(list);
            }
        }


        /// <summary>
        /// 加载指定的值到 Station Session 中，需要指定 Value，可以指定多个
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SpecialValueLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            foreach (R_Station_Action_Para para in Paras)
            {
                MESStationSession TempSession = Station.StationSession.Find(t => t.MESDataType == para.SESSION_TYPE && t.SessionKey == para.SESSION_KEY);
                if (TempSession == null)
                {
                    TempSession = new MESStationSession() { MESDataType = para.SESSION_TYPE, SessionKey = para.SESSION_KEY, Value = para.VALUE };
                    Station.StationSession.Add(TempSession);
                }
                else
                {
                    TempSession.Value = para.VALUE;
                }
            }

        }

        /// <summary>
        /// 更改當前工站的工站名
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ChangeStationLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession _StationSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (_StationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            Station.StationName = _StationSession.Value.ToString();
        }

        /// <summary>
        /// 設置輸入框顯示、可用狀態、清空
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SetInputStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
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

        public static void KeypartMPNLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession KeypartSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (KeypartSN == null || KeypartSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession MPN = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (MPN == null)
            {
                MPN = new MESStationSession { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(MPN);
            }
            List<R_SN_KP> kplist = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((K) => K.VALUE == KeypartSN.Value.ToString()).ToList();
            if (kplist.Count > 0)
            {
                MPN.Value = kplist[0].MPN;
            }
        }

        public static void KeypartNameLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession KeypartSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (KeypartSN == null || KeypartSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession KeyPartName = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (KeyPartName == null)
            {
                KeyPartName = new MESStationSession { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(KeyPartName);
            }
            List<R_SN_KP> kplist = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((K) => K.VALUE == KeypartSN.Value.ToString()).ToList();
            if (kplist.Count > 0)
            {
                KeyPartName.Value = kplist[0].KP_NAME;
            }
        }

        public static void KeypartRuleLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession KeypartSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (KeypartSN == null || KeypartSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string ScanTypeList = Paras[0].VALUE;
            string[] ScanType = ScanTypeList.Split(',');
            MESStationSession Rule = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (Rule == null)
            {
                Rule = new MESStationSession { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(Rule);
            }
            R_SN_KP kpt = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((K) => K.VALUE == KeypartSN.Value.ToString()).First();
            if (kpt == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190307133818", new string[] { KeypartSN.Value.ToString() }));
            }
            List<R_SN_KP> kplist = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((K) => K.SN == kpt.SN && K.PARTNO == kpt.PARTNO && (SqlSugar.SqlFunc.IsNullOrEmpty(kpt.EXVALUE1) || K.EXVALUE1.Equals(kpt.EXVALUE1)) && ScanType.Contains(K.SCANTYPE)).ToList();
            if (kplist.Count > 0)
            {
                List<string> mpntype = new List<string> { "MPN", "PN" };
                List<string> mpn = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((K) => K.SN == kpt.SN && K.PARTNO == kpt.PARTNO && (SqlSugar.SqlFunc.IsNullOrEmpty(kpt.EXVALUE1) || K.EXVALUE1.Equals(kpt.EXVALUE1)) && mpntype.Contains(K.SCANTYPE)).Select(t => t.MPN).ToList();
                if (mpn.Count == 0)
                {
                    mpn = kplist.Select(t => t.MPN).ToList();
                }
                string partno = kplist[0].PARTNO;
                string scantype = kplist[0].SCANTYPE;
                R_SN sn = Station.SFCDB.ORM.Queryable<R_SN>().Where((K) => K.SN == kpt.SN).First();
                C_SKU sku = Station.SFCDB.ORM.Queryable<C_SKU>().Where((s) => s.SKUNO == sn.SKUNO).First();
                var ob = Station.SFCDB.ORM.Queryable<C_SKU_MPN, C_KP_Rule, C_SKU_MPN>((M, R, M1)
                    => new object[] {
                    JoinType.Inner,M.MPN==R.MPN,
                    JoinType.Inner,M.SKUNO==M1.SKUNO && (M.MFRCODE==M1.MFRCODE || SqlSugar.SqlFunc.IsNullOrEmpty(M.MFRCODE))
                }).Where((M, R, M1) => (M.SKUNO == sku.SKU_NAME || M.SKUNO == sku.SKUNO) && M.PARTNO == partno && (mpn.Count == 0 || mpn.Contains(M1.MPN)) && R.SCANTYPE == scantype)
                .Select((M, R, M1) => R.REGEX);
                var sql = ob.ToSql();
                var rules = ob.ToList().Distinct().ToList();
                Rule.Value = rules;
            }
            else
            {
                // var temp = "NONE";
                // Rule.Value = temp.ToList();
                Rule.Value = "NONE";
            }
        }

        ///<summary
        ///Add keypart auto trim based on table setup c_kp_trim
        ///add BY James Zhu 12/14/2019
        public static void GetKeyPartTrimData(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            {
                MESStationSession KeypartSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
                //if (KeypartSN == null || KeypartSN.Value == null)
                //{
                //    KeypartSN = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                //    Station.StationSession.Add(KeypartSN);
                //}
                //R_SN_KP kpt = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((K) => K.VALUE == KeypartSN.Value.ToString()).First();
                //if (kpt == null)
                //{
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190307133818", new string[] { KeypartSN.Value.ToString() }));
                //}
                //C_KP_TRIM kptrim = Station.SFCDB.ORM.Queryable<C_KP_TRIM>().Where((k) => k.PARTNO == kpt.PARTNO).First();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        /// 
        /// 
        /// 
        ///

        /// <summary>
        /// 獲取實際SN,(輸入的是二維碼，加載正在的SN到指定位置)
        /// ADD BY HGB 2019.05.24
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">        
        /// </param>
        public static void GetScanSnloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            {
                if (Paras.Count != 1)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }

                MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (s == null)
                {
                    s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                    Station.StationSession.Add(s);
                }
                s.Value = Input.Value.ToString();
                s.InputValue = Input.Value.ToString();
                s.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { Paras[0].SESSION_TYPE, Input.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);


                string SN = s.Value.ToString().Trim().ToUpper();
                string test = SN.Substring(1, 1);
                DataInputLoader dtl = new DataInputLoader();
                SN = dtl.GetScanSn(SN);

                s.InputValue = SN;
                s.Value = SN;
                Input.Value = SN;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        /// <summary>
        /// 獲取實際SN,(輸入的是二維碼，加載正在的SN到指定位置)
        /// ADD BY HGB 2019.05.24
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">        
        /// </param>
        public string GetScanSn(string SN)
        {
            try
            {
                if (SN.Substring(0, 2) == "[)")//是二維碼
                {
                    if (SN.Substring(0, 7) == "[)>06SA")
                    {
                        goto ReturnSN;
                    }

                    if (SN.Substring(0, 6) == "[)>06S")
                    {
                        if (SN.Substring(0, 8) == "[)>06SDM")
                        {
                            SN = SN.Substring(6, 12);
                            goto ReturnSN;
                        }
                        SN = SN.Substring(6, 16);
                        goto ReturnSN;
                    }
                    else if (SN.Substring(0, 5) == "[)>06" && SN.Substring(6, 1) == "S" && SN.Substring(0, 6) == "[)>06P")
                    {
                        SN = SN.Substring(7, SN.IndexOf("1P") - 9);
                        SN = SN.Replace("\n", "");//去換行
                        SN = SN.Replace("\t", "");//去d掉回車
                        goto ReturnSN;
                    }
                    else if (SN.Substring(0, 7) == "[)>606S")
                    {
                        SN = SN.Substring(8, SN.IndexOf("1P") - 10);
                        SN = SN.Replace("\n", "");//去換行
                        SN = SN.Replace("\t", "");//去d掉回車
                        goto ReturnSN;
                    }
                    else if (SN.Substring(0, 3) == "[)>" && SN.Substring(4, 3) == "06S")
                    {
                        SN = SN.Substring(8, SN.IndexOf("1P") - 9);
                        SN = SN.Replace("\n", "");//去換行
                        SN = SN.Replace("\t", "");//去d掉回車
                        goto ReturnSN;
                    }
                    else if (SN.Substring(0, 6) == "[)>06P")
                    {
                        goto ReturnSN;
                    }
                    else if (SN.Substring(0, 7) == "[)>061P")
                    {
                        SN = SN.Substring(19, 11);
                        SN = SN.Replace("\n", "");//去換行
                        SN = SN.Replace("\t", "");//去d掉回車
                        goto ReturnSN;
                    }
                    else if (SN.Substring(0, 1) == "S" && SN.Length >= 12 && SN.Contains("1P") == true)
                    {
                        SN = SN.Substring(1, SN.IndexOf("1P") - 2);
                        SN = SN.Replace("\n", "");//去換行
                        SN = SN.Replace("\t", "");//去d掉回車
                        goto ReturnSN;
                    }
                    else
                    {
                        goto ReturnSN;
                    }


                }
                else if (SN.Substring(0, 4) == "006S")
                {
                    SN = SN.Substring(4, SN.IndexOf("1P") - 5);
                    SN = SN.Replace("\n", "");//去換行
                    SN = SN.Replace("\t", "");//去d掉回車
                    goto ReturnSN;
                }
                else if (SN.Substring(0, 5) == "0606S")
                {
                    SN = SN.Substring(5, SN.IndexOf("1P") - 6);
                    SN = SN.Replace("\n", "");//去換行
                    SN = SN.Replace("\t", "");//去d掉回車
                    goto ReturnSN;
                }
                else if (SN.Substring(0, 5) == "0206S")
                {
                    SN = SN.Substring(6, SN.IndexOf("1P") - 7);
                    SN = SN.Replace("\n", "");//去換行
                    SN = SN.Replace("\t", "");//去d掉回車
                    goto ReturnSN;
                }
                else if (SN.Contains(";") == true)
                {
                    string[] snarry = SN.Split('；');
                    if (snarry.Length >= 3)
                    {
                        string NewSN = snarry[2];
                        if (NewSN.Length > 0)
                        {
                            SN = snarry[0];
                        }
                    }
                    goto ReturnSN;
                }
                else
                {
                    goto ReturnSN;
                }


            ReturnSN:
                return SN;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }


        /// <summary>
        /// Set input select list value by paras value
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SelectListLoaderValue(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<string> list = new List<string>();
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionList = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionList == null)
            {
                sessionList = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionList);
            }
            else
            {
                list = (List<string>)sessionList.Value;
            }
            for (int i = 0; i < Paras.Count; i++)
            {
                list.Add(Paras[i].VALUE);
            }
            sessionList.Value = list;
            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE);
            s.DataForUse.Clear();
            foreach (var l in list)
            {
                s.DataForUse.Add(l);
            }
        }
        /// <summary>
        /// 加載不為空的SESSION值到下拉框
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SessionValueLoadingToSelectList(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<string> list = new List<string>();
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionValue = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionValue == null || sessionValue.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionList == null)
            {
                sessionList = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionList);
            }
            else
            {
                list = (List<string>)sessionList.Value;
            }
            if (list == null)
            {
                list = new List<string>();
            }
            if (sessionValue.Value.ToString() != "")
            {
                list.Add(sessionValue.Value.ToString());
            }
            sessionList.Value = list;
            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
            s.DataForUse.Clear();
            foreach (var l in list)
            {
                s.DataForUse.Add(l);
            }
        }

        /// <summary>
        /// 根據輸入加載SN對象或PANEL對象等
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputValueLoadingToObject(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionInputValue = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionInputValue == null || sessionInputValue.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionInputObject = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionInputObject == null)
            {
                sessionInputObject = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionInputObject);
            }
            string input_value = sessionInputValue.Value.ToString();
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_PANEL_SN TRPS = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            R_SN objSN = null;

            List<R_SN> listSN = new List<R_SN>();

            //輸入的是一個SN
            objSN = TRS.LoadData(input_value, Station.SFCDB);
            if (objSN != null)
            {
                listSN.Add(objSN);
                SN objSNNew = new SN(objSN.SN, Station.SFCDB, Station.DBType);
                sessionInputObject.Value = objSNNew;
            }
            else if (TRPS.CheckPanelExist(input_value, Station.SFCDB))//輸入的是一個Panel
            {
                listSN = TRPS.GetSn(input_value, Station.SFCDB);
                Panel panelObject = new Panel(input_value, Station.SFCDB, Station.DBType);
                panelObject.GetPanel(input_value, Station.SFCDB, Station.DBType);
                sessionInputObject.Value = panelObject;
            }
            else
            {
                sessionInputObject.Value = input_value;
            }
        }

        public static void GetLabelBySKUDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKU == null || sessionSKU.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SKU sku = (SKU)sessionSKU.Value;

            T_C_SKU_Label TCSL = new T_C_SKU_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(sku.SkuNo, Station.StationName.ToString(), Station.SFCDB);
            T_R_Label TRL = new T_R_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationInput stationInput = Station.Inputs.Find(t => t.DisplayName == "LABELNAME");
            if (labs.Count > 0)
            {
                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                for (int i = 0; i < labs.Count; i++)
                {
                    R_Label RL = TRL.GetLabelByLabelName(labs[i].LABELNAME, Station.SFCDB);
                    stationInput.DataForUse.Add(RL.LABELNAME + "," + RL.R_FILE_NAME);
                }
            }
            else
            {
                throw new MESReturnMessage($@"{sku.SkuNo},{Station.StationName.ToString()},No Label");
            }
        }

        public static void LoadingSessionValueToInput(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (session == null || session.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationInput input = Station.Inputs.Find(r => r.DisplayName == Paras[1].VALUE);
            if (input != null)
            {
                input.Value = session.Value;
            }
            else
            {
                throw new MESReturnMessage($@"Input({Paras[1].VALUE}) Not Exist!");
            }
        }

        public static void GetBipPassTime(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string SN = "";
            string PastTime = "";
            OleExec SFCDB = Station.SFCDB;
            MESStationSession strSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (strSN == null)
            {
            }
            else
            {
                SN = strSN.Value.ToString();
                DateTime? ScanTime = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(rs => rs.SN == SN && rs.STATION_NAME == "BIP" && rs.VALID_FLAG == "1").OrderBy(rs => rs.EDIT_TIME, OrderByType.Desc).Select(rs => rs.EDIT_TIME).First();
                if (ScanTime != null)
                {
                    TimeSpan interval = DateTime.Now - Convert.ToDateTime(ScanTime);
                    string strInterval = interval.TotalHours.ToString();
                    PastTime = strInterval.Substring(0, strInterval.IndexOf(".") + 3) + " h";
                }

                MESStationSession timeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (timeSession == null)
                {
                    timeSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                    Station.StationSession.Add(timeSession);
                }
                timeSession.Value = PastTime;
            }
        }

        public static void JuniperGetDNFrom2DCode(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {            
            MESStationSession dnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (dnSession == null)
            {
                dnSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY };
                Station.StationSession.Add(dnSession);
            }
            //SO:15817799,DN:8001040766
            string _2dvalue = Input.Value.ToString();
            if (_2dvalue.Length == 0)
            {
                throw new MESReturnMessage("Please scan barcode!");
            }
            try
            {
                string dn = _2dvalue.Split(',')[1];
                dnSession.Value = dn.Split(':')[1];
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage($@"Get DN from 2D code fail!{ex.Message}");
            }
        }

        public static void SNMakerTest(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snRuleSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            string snRuleName = snRuleSession.Value.ToString();
            string sn = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(snRuleName, Station.SFCDB.ORM);
            Station.StationMessages.Add(new MESPubLab.MESStation.MESReturnView.Station.StationMessage { Message =sn, State = MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass });
        }
    }
}
