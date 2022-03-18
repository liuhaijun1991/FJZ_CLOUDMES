using System;
using System.Collections.Generic;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESDataObject;
using MESDataObject.Common;
using MESStation.LogicObject;



using MESPubLab.MESStation.MESReturnView.Station;
using System.Linq;
using static MESDataObject.Common.EnumExtensions;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckRFunctionControl
    {
        /// <summary>
        /// 檢查R_FUNCTION_CONTROL是否配置CHECK_PARTCODE料號
        /// add by zwx 2020-3-26
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PartCodeScanChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
         {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;

            SN sn = (SN)SNSession.Value;
            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
            string FUNCTIONNAME = String.Empty;
            string CATEGORY = String.Empty;
            string PARTNO = String.Empty;
            string PCBSN = String.Empty;
            T_R_SN_KP TRSNKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> Checkpc;

            List<R_F_CONTROL> Lrfc = TRFC.GetListByFcv("CHECK_PARTCODE", "CHECK_PARTCODE", sn.SkuNo, SFCDB);

            if (Lrfc.Count > 0)
            {
                #region 增加掃描掛卡Label上面的MAC('S'+sn.SerialNo),確保掛卡沒掛錯機台  Asked By PE楊大饒 2021-12-30
                UIInputData O = new UIInputData()
                {
                    IconType = IconType.None,
                    UIArea = new string[] { "35%", "30%" },
                    Message = "TagSN",
                    Tittle = " Scan Actually TagLabel",
                    Type = UIInputType.String,
                    Name = "TagSN",
                    ErrMessage = "User Cancel"
                };
                var TagSN = O.GetUiInput(Station.API, UIInput.Normal, Station);
                if (TagSN.ToString() != "S" + sn.SerialNo)
                {
                    throw new Exception($@"Scan TagSN Wrong, Pls Comfirm with PE!");
                }
                #endregion

                for (int i = 0; i < Lrfc.Count; i++)
                {
                    FUNCTIONNAME = Lrfc[i].FUNCTIONNAME;
                    CATEGORY = Lrfc[i].CATEGORY;
                    PARTNO = Lrfc[i].EXTVAL;
                    Checkpc = TRSNKP.LoadListDataBySnAndPartno(sn.SerialNo, PARTNO, SFCDB);                    
                    if (Checkpc == null || Checkpc.Count == 0)
                    {
                        //check 2 階
                        Checkpc = SFCDB.ORM.Queryable<R_SN_KP, R_SN_KP>((a, b) => a.VALUE == b.SN)
                            .Where((a, b) => a.VALID_FLAG == 1 && b.VALID_FLAG == 1 && a.SN == sn.SerialNo && b.PARTNO == PARTNO)
                            .Select((a, b) => b).ToList();
                        if (Checkpc == null || Checkpc.Count == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200327095600", new string[] { sn.SerialNo.ToString(), PARTNO }));
                        }                        
                    }
                    PCBSN = Checkpc[0].VALUE;
                    if (PCBSN == null || PCBSN == "")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200327095600", new string[] { sn.SerialNo.ToString(), PARTNO }));
                    }

                    UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "35%", "50%" }, Message = "PCBASN", Tittle = FUNCTIONNAME+"=>"+CATEGORY + " NO." + (i + 1).ToString(), Type = UIInputType.String, Name = "PCBASN", ErrMessage = "Cancel  "+ FUNCTIONNAME + "=>" + CATEGORY + "  Check" };
                    I.OutInputs.Add(new DisplayOutPut() { Name = "PCBA S/N", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = PCBSN });
                    I.OutInputs.Add(new DisplayOutPut() { Name = "PCBA P/N", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = PARTNO });
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);

                    if (ret.ToString() != PCBSN)
                    {
                        //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200402142100", new string[] { ret.ToString(), PCBSN }));
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20211230131634", new string[] { ret.ToString(), PCBSN }));//TagLabel Sn {0} and Pcba Sn {1} did not link,Pls Confirm actually with PE or QE!
                    }
                }
            }
         }

         /// <summary>
         /// 檢查R_FUNCTION_CONTROL是否配置FIXTURE_SCAN需要掃描治具
         /// add by zwx 2020-4-2
         /// </summary>
         /// <param name="Station"></param>
         /// <param name="Input"></param>
         /// <param name="Paras"></param>
        public static void FixtureScanChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
         {
            MESStationSession InputSKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            string FUNCTIONNAME = String.Empty;
            string CATEGORY = String.Empty;
            string FIXTURENO = String.Empty;
            string CHECKSTATION = String.Empty;
            if (InputSKUSession == null)
             {
                 throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
             }
             T_R_FUNCTION_CONTROL TRSCV = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
             List<R_FUNCTION_CONTROL_NewList> lrfc = TRSCV.Get1ExListbyVarValue("FIXTURE_SCAN", "FIXTURE_SCAN", InputSKUSession.InputValue.ToString(), Station.StationName, SFCDB);


             if (lrfc.Count > 0)
             {
                 for (int i = 0; i < lrfc.Count; i++)
                 {
                    FUNCTIONNAME = lrfc[i].FUNCTIONNAME;
                    CATEGORY = lrfc[i].CATEGORY;
                    CHECKSTATION = lrfc[i].EXTVAL1;
                    FIXTURENO = lrfc[i].EXTVAL2;

                    UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "FIXTURENO", Tittle = FUNCTIONNAME+"=>"+CATEGORY + " NO." + (i + 1).ToString(), Type = UIInputType.String, Name = "FIXTURENO", ErrMessage = "Cancel  " + FUNCTIONNAME + "=>" + CATEGORY + "  Check" };
                     I.OutInputs.Add(new DisplayOutPut() { Name = "STATION", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = CHECKSTATION });
                     I.OutInputs.Add(new DisplayOutPut() { Name = "FIXTURENO", DisplayType = UIOutputType.Text.Ext<EnumNameAttribute>().Description, Value = FIXTURENO });
                     var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);

                     if (ret.ToString() != FIXTURENO)
                     {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200402172100", new string[] { ret.ToString(), FIXTURENO }));
                     }
                     
                 }

             }
         }

        /// <summary>
        /// 檢查R_FUNCTION_CONTROL是否配置CM_TEST類型檢查CM產品最後一筆測試記錄
        /// add by zwx 2020-4-15
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CmLastTestStationChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            string RetMessage = string.Empty;
            SN sn = (SN)SNSession.Value;
            bool CheckCtm;
            bool UnCheckSeries;
            bool UnCheckSku;
            bool UnCheckSkuPre= true;
            bool CheckLastStation=false;
            string TESTATION = string.Empty;
            string TESTSTATE = string.Empty;

            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN TRSN = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_TEST_RECORD TRTR = new T_R_TEST_RECORD(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_F_CONTROL> Lrfc;
            GET_SeriesCustomer_BySn Lsc = TRSN.GetSeriesCtmBySn(sn.SerialNo, SFCDB);

            CheckCtm=TRFC.CheckUserFunctionExist("CM_TEST", "CM_TEST_CUSTOMER_NAME", Lsc.CUSTOMER_NAME, SFCDB);
            UnCheckSeries=!TRFC.CheckUserFunctionExist("CM_TEST", "CM_TEST_BYPASS_SERIES", Lsc.SERIES_NAME, SFCDB);
            UnCheckSku=!TRFC.CheckUserFunctionExist("CM_TEST", "CM_TEST_BYPASS_SKU", sn.SkuNo, SFCDB);
            Lrfc=TRFC.GetListByFcv("CM_TEST", "CM_TEST_BYPASS_SKU_PREFIX", SFCDB);

            for (int i=0; i < Lrfc.Count;i++)
            { 
                 if (sn.SkuNo.StartsWith(Lrfc[i].VALUE))
                {
                    UnCheckSkuPre = false;
                    break;
                }
            }   
            R_TEST_RECORD Lrtr = TRTR.GetLastTestRecord(sn.SerialNo, SFCDB);
            if (Lrtr != null) {
                TESTATION = Lrtr.TESTATION;
                TESTSTATE = Lrtr.STATE;
            }
            Lrfc = TRFC.GetListByFcv("CM_TEST", "CM_TEST_LAST_TEST_STATION", SFCDB);
            for (int i = 0; i < Lrfc.Count; i++)
            {
                RetMessage += Lrfc[i].VALUE + " ";
                if (TESTATION== Lrfc[i].VALUE)
                {
                    CheckLastStation = true;   
                }
            }

            if (CheckCtm&& UnCheckSeries&& UnCheckSku&& UnCheckSkuPre&&CheckLastStation)
            {
              throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200415171500", new string[] { RetMessage }));
            }
        }

        /// <summary>
        /// 檢查R_FUNCTION_CONTROL是否配置CHECK_PREXID,配置了不允許過站
        /// add by zwx 2020-4-16
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnPrexidChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            SN sn = (SN)SNSession.Value;
            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
            string SNPRE = String.Empty;

            List<R_F_CONTROL> Lrfc = TRFC.GetListByFcv("CHECK_PREXID", "CHECK_PREXID", sn.SkuNo, SFCDB);

            if (Lrfc.Count > 0)
            {
                for (int i = 0; i < Lrfc.Count; i++)
                {
                    SNPRE = Lrfc[i].EXTVAL;
                    if (sn.SerialNo.StartsWith(SNPRE))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200416112300", new string[] { SNPRE, sn.SkuNo }));
                    }
                }
            }


        }

        /// <summary>
        /// 檢查R_FUNCTION_CONTROL是否配置RC_TEST_CHECK,是否有RC測試
        /// add by zwx 2020-4-16
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RcTestChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            SN sn = (SN)SNSession.Value;
            bool CheckCtm;
            bool UnContainsString= true;
            bool UnCheckSku;
            bool CheckRCTime = true;
            DateTime? TETIME ;
            string TESTSTATE = string.Empty;

            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN TRSN = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_TEST_RECORD TRTR = new T_R_TEST_RECORD(SFCDB, DB_TYPE_ENUM.Oracle);
            GET_SeriesCustomer_BySn Lsc = TRSN.GetSeriesCtmBySn(sn.SerialNo, SFCDB);

            CheckCtm = TRFC.CheckUserFunctionExist("RC_TEST_CHECK", "RC_TEST_CHECK_CUSTOMER", Lsc.CUSTOMER_NAME, SFCDB);
            UnCheckSku = !TRFC.CheckUserFunctionExist("RC_TEST_CHECK", "RC_TEST_BYPASS_SKUNO", sn.SkuNo, SFCDB);


            List<R_F_CONTROL>  Lrfc = TRFC.GetListByFcv("RC_TEST_CHECK", "RC_TEST_BYPASS_CONTAINS_STRING", SFCDB);
            for (int i = 0; i < Lrfc.Count; i++)
            {
                if (sn.SkuNo.Contains(Lrfc[i].VALUE))
                {
                    UnContainsString = false;
                    break;
                }
            }
            R_TEST_RECORD Lrtr = TRTR.GetLastTestRecord(sn.SerialNo, "RC", SFCDB);
            if (Lrtr != null)
            {
                TETIME = Lrtr.ENDTIME;
                TESTSTATE = Lrtr.STATE;

                if (DateTime.Now.AddMinutes(-20)< TETIME && TESTSTATE=="PASS")
                {
                    CheckRCTime = false;
                }
            }

            if (CheckCtm && UnContainsString && UnCheckSku&& CheckRCTime)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200416153000", new string[] {  }));
            }
        }

        /// <summary>
        /// 檢查R_FUNCTION_CONTROL是否配置OLD_CARRIER_CHECK,檢查載具使用次數
        /// add by zwx 2020-4-17
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OldCarrierCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            SKU sku = (SKU)SNSession.Value;
            int MaxTimes = 0;
            int UseTimes = 0;
            string TESTSTATE = string.Empty;

            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);

            List<R_F_CONTROL> Lrfc = TRFC.GetListByFcv("OLD_CARRIER_CHECK", "SKUNO_CARRIER_LINK", sku.SkuNo, SFCDB);

            if (Lrfc.Count>0) {
                List<R_FUNCTION_CONTROL_NewList> UsetimesList = TRFC.Get2ExListbyVarValue("OLD_CARRIER_CHECK", "CARRIER_USE_TIMES", Lrfc[0].EXTVAL, SFCDB);
                if (UsetimesList.Count==1) {
                    try { 
                        MaxTimes = Convert.ToInt32(UsetimesList[0].EXTVAL1);
                        UseTimes = Convert.ToInt32(UsetimesList[0].EXTVAL2);
                        if(UseTimes>=MaxTimes)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200417172700", new string[] {}));
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200417172400", new string[] {}));
                }
            }
        }

        /// <summary>
        /// 檢查R_FUNCTION_CONTROL是否配置QE_CONFIRM的D-LINK_SHIPOUT_WO,檢查QE是否確認
        /// add by zwx 2020-4-18
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void QEConfirmDLINK(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            //SN sn = (SN)SNSession.Value;
            string wo = string.Empty;
            string  packNo = Input.Value.ToString();
            T_R_FUNCTION_CONTROL rFunctionc = new T_R_FUNCTION_CONTROL(SFCDB,DB_TYPE_ENUM.Oracle);
            T_R_SN rsn = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);

            //GET_SeriesCustomer_BySn CTM = rsn.GetSeriesCtmBySn(packNo, SFCDB);
            C_CUSTOMER CC = SFCDB.ORM.Queryable<R_PACKING, C_SKU, C_SERIES, C_CUSTOMER>((t1, t2, t3, t4) => t1.SKUNO == t2.SKUNO && t2.C_SERIES_ID == t3.ID && t3.CUSTOMER_ID == t4.ID)
                .Where((t1, t2, t3, t4) => t1.PACK_NO == packNo).Select((t1, t2, t3, t4) => t4).First();
            if (CC == null)
            {
                throw new MESReturnMessage($@"Skuno No Config Customer, Please Check!");
            }

            R_SN sn= SFCDB.ORM.Queryable<R_SN, R_PACKING, R_PACKING, R_SN_PACKING>
                    ((rs, rp1, rp2, rsp) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rsp.SN_ID == rs.ID)
                    .Where((rs, rp1, rp2, rsp) => rp1.PACK_NO == packNo).Select((rs, rp1, rp2, rsp)=>rs).ToList().FirstOrDefault();


            bool LimitCTM = CC.CUSTOMER_NAME.Contains("D-LINK");
            bool QEconfirmWo = !rFunctionc.CheckUserFunctionExist("QE_CONFIRM", "D-LINK_WO",sn.WORKORDERNO , SFCDB);

            if (LimitCTM&& QEconfirmWo)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200418113716", new string[] { "D-LINK", sn.WORKORDERNO }));
            }
        }

        /// <summary>
        /// 檢查R_FUNCTION_CONTROL是否配置QE_CONFIRM的CHECK_PREFIX,檢查QE是否確認
        /// add by zwx 2020-4-21
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPrefix(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            SN sn = (SN)SNSession.Value;
            T_R_FUNCTION_CONTROL rFunctionc = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_F_CONTROL> Lrfc = rFunctionc.GetListByFcv("QE_CONFIRM", "CHECK_PREFIX", sn.SkuNo, SFCDB);
            if (Lrfc.Count > 0) {
                for (int i = 0; i < Lrfc.Count; i++) {
                   if(sn.SerialNo.StartsWith(Lrfc[i].EXTVAL))
                    {
                       throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200421095359", new string[] { sn.SkuNo, Lrfc[i].EXTVAL}));
                    }
                }
            }
        }


    }
}
