using MESDataObject;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using System.Text.RegularExpressions;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using System.Data;
using MESPubLab.Json;
using MESStation.LogicObject;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MESStation.Stations.StationActions.DataCheckers
{
    class CheckCMC
    {
        public static void CM_TestData_Check(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionStation_NO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation_NO == null || sessionStation_NO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSKU == null || sessionSKU.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession custsnSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (custsnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            MESStationSession SN2Session = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (SN2Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }

            MESStationSession SITESession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (SITESession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
            }

            SKU sKU = (SKU)sessionSKU.Value;
            WorkOrder wo = (WorkOrder)sessionWO.Value;
            R_CUSTSN_T CUST = (R_CUSTSN_T)custsnSession.Value;
            T_C_FT_CONTROL_RULE t_C_FT = new T_C_FT_CONTROL_RULE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_CUSTSN_T Tcust = new T_R_CUSTSN_T(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            string SNStr = Input.Value.ToString();

            R_CUSTSN_T r_CUSTSN_T = Tcust.GetTestDataBySn(SNStr, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            Type type = CUST.GetType();
            System.Reflection.PropertyInfo[] field = type.GetProperties();


            MESStation.Config.CMC.TwoDataItem TDataItem =
                    JsonSave.GetFromDB<MESStation.Config.CMC.TwoDataItem>(sessionStation_NO.Value.ToString(), "2", SNStr, Station.SFCDB);
            if (TDataItem.Status.ToUpper() == "PASS")
            {
                //Check測試數據是否又和別的SN重碼
                List<C_FT_CONTROL_RULE> c_ft = t_C_FT.GetCheckList(sKU.SkuNo, Station.SFCDB, Station.StationName);
                if (c_ft.Count > 0)
                {
                    foreach (C_FT_CONTROL_RULE item in c_ft)
                    {
                        FtControl Ftc = field.Where(t => t.Name == item.CONTROL_FIELD).Select(s => new FtControl { CheckType = s.Name, Value = s.GetValue(CUST, null).ToString() }).FirstOrDefault();
                        int i = Tcust.CheckSSNorMAC(SNStr, Ftc, Station.SFCDB);
                        if (i > 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000002", new string[] { Ftc.CheckType + ":" + Ftc.Value }));
                        }
                    }
                }

                if (r_CUSTSN_T != null)
                {
                    Type type1 = r_CUSTSN_T.GetType();
                    System.Reflection.PropertyInfo[] field1 = type1.GetProperties();
                    //Check此次拋轉數據與數據庫中是否一致
                    foreach (System.Reflection.PropertyInfo item in field1)
                    {
                        List<System.Reflection.PropertyInfo> listproperties = field.Where(t => t.Name == item.Name && (t.GetValue(CUST, null) == null ? "" : t.GetValue(CUST, null).ToString()) != (item.GetValue(r_CUSTSN_T, null) == null ? "" : item.GetValue(r_CUSTSN_T, null).ToString()) && (item.GetValue(r_CUSTSN_T, null) == null ? "" : item.GetValue(r_CUSTSN_T, null).ToString()) != "" && Tcust.NotCheck(t.Name)).ToList();
                        if (listproperties.Count > 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MESMBD00000000000003", new string[] { listproperties[0].Name, SNStr }));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// For CMC條碼綁定工站(ASSY&SILOADING)掃描檢查(涉及掃描步驟判斷處理需要表R_AP_TEMP)  
        /// For南寧MBD CMC掃描模式開發,用於檢查當前掃入條碼是否符合按照KeyPart清單.
        /// add NN.MBD YCX 2020/05/13 tips:目前是(南寧MBD)KeyPart過站(ASSY&SIOADING)主API
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckKeyPartScanList(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string strStatus = string.Empty, strModelType = string.Empty, strScanKpSn = string.Empty, strStationNo = string.Empty;
            //Station_No對象,必須指定.
            MESStationSession sessionStationNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStationNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            strStationNo = (string)sessionStationNo.Value;
            if (string.IsNullOrEmpty(strStationNo))
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));

            //SN主板條碼,必須指定.
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            else if (sessionSn.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            //WO對象,必須指定.
            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionWo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            if (sessionWo.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            //機種ModelType,必須指定
            MESStationSession sessionModelType = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionModelType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }
            else
            {
                strModelType = (sessionModelType.Value == null || sessionModelType.Value.ToString() == "") ? "" : sessionModelType.Value.ToString();
            }

            //掃描的KeyPart條碼,必須指定
            MESStationSession sessionKeyPartSn = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionKeyPartSn == null)
            {
                sessionKeyPartSn = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, InputValue = Input.Value.ToString(), ResetInput = Input };
                Station.StationSession.Add(sessionKeyPartSn);
            }
            strScanKpSn = (string)sessionKeyPartSn.Value;
            if (!string.IsNullOrEmpty(strScanKpSn))
                strScanKpSn = (string)Input.Value;

            //SN KeyPart清單,必須設定.
            MESStationSession sessionKpScanList = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionKpScanList == null)
            {
                sessionKpScanList = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY, InputValue = Input.Value.ToString(), ResetInput = Input };
                Station.StationSession.Add(sessionKpScanList);
            }

            OleExec sfcdb = Station.SFCDB;
            LogicObject.SN sn = new SN();
            T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            WorkOrder woObject = (WorkOrder)sessionWo.Value;

            #region 初始化主板SN對象
            if (sessionSn.Value is SN)
                sn = (SN)sessionSn.Value;
            else
            {
                string tmpsyssn = (string)sessionSn.Value;
                if (string.IsNullOrEmpty(tmpsyssn))
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));

                if (Station.StationName.EndsWith("LOADING"))
                {
                    //SI LOADING站,沒LOADING時沒有主表資料,只能自己指定.
                    sn.baseSN = new R_SN()
                    {
                        SN = tmpsyssn,
                        SKUNO = woObject.SkuNO,
                        WORKORDERNO = woObject.WorkorderNo,
                        PLANT = woObject.PLANT,
                        ROUTE_ID = woObject.RouteID
                    };
                }
                else
                {
                    //普通工站直接加載SN對象
                    sn.Load(tmpsyssn, sfcdb, DB_TYPE_ENUM.Oracle);
                }
            }
            #endregion

            #region 從StationSession獲取主板條碼的KeyPart掃描List和已掃的值
            List<Row_R_SN_KP> kpScanList = new List<Row_R_SN_KP>();
            if (sessionKpScanList.Value != null)
            {
                kpScanList = (List<Row_R_SN_KP>)sessionKpScanList.Value;
                //檢查當前在session裡面的是不是這個條碼的KeyPartList.沒有就清空不檢查KeyPart.
                if (!kpScanList.Where(p => p.SN == sn.baseSN.SN).Any())
                {
                    kpScanList = new List<Row_R_SN_KP>();
                }
            }
            #endregion

            if (kpScanList.Count > 0)
            {
                #region 從R_AP_TEMP取得用戶當前掃描的是第幾槍
                //考慮到存在KeyPart條碼Check沒有問題更新成功進Session但其它檢查或過站API拋出異常掃描失敗導致KeyPart掃描不同步問題,使用R_AP_TEMP來判斷現在KeyPart掃的是哪一槍
                int scanindex = 0;
                T_R_AP_TEMP tap = new T_R_AP_TEMP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_AP_TEMP> aplist = tap.GetRecordByStationNo(Station.SFCDB, strStationNo);
                scanindex = aplist.Where(p => p.DATA8 == "KEYPART").ToList().Count - 1;
                if (scanindex > kpScanList.Count || scanindex < 0)
                {
                    //拋異常要求用戶從UNDO開始重新掃描.
                    sessionKpScanList.Value = new List<Row_R_SN_KP>();
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20200624141822", new string[] { }));
                }
                #endregion

                kpScanList[scanindex].VALUE = strScanKpSn;

                #region 更新當前掃描的ScanType到R_AP_TEMP表
                string currentscantype = kpScanList[scanindex].SCANTYPE;
                string nextscantype = (kpScanList.Count - 1) > scanindex ? kpScanList[scanindex + 1].SCANTYPE : "";
                tap.UpdataApScanTypeStation(Station.SFCDB, strStationNo, aplist[aplist.Count - 1].DATA3, currentscantype, nextscantype);
                #endregion

                #region 掃描條碼預先處理,針對ACCY類型物料對掃入條碼加時間後綴作為綁定SN
                snReprocess(sfcdb, scanindex, ref kpScanList, sn, null);
                #endregion

                List<Row_R_SN_KP> tmpKpList = kpScanList.Where(p => p.SN == sn.baseSN.SN && p.STATION == Station.StationName && p.VALUE == "").ToList();
                if (tmpKpList.Count > 0)//只檢查當前掃描的SN的規則
                {
                    checkSnKeyPart(sfcdb, Station.DBType, scanindex, kpScanList, sn, null, Station.API, strModelType);
                }
                else//最後一槍,對當前工站掃KeyPart的所有項目重新檢查
                {
                    for (int i = 0; i < kpScanList.Count; i++)
                    {
                        checkSnKeyPart(sfcdb, Station.DBType, i, kpScanList, sn, null, Station.API, strModelType);
                    }
                }
                sessionKpScanList.Value = kpScanList;
            }
        }

        /// <summary>
        /// For CMC條碼綁定工站(ASSY&SILOADING)掃描檢查(涉及掃描步驟判斷處理需要表R_AP_TEMP),方法已經停用.并拆分.
        /// For南寧MBD CMC掃描模式開發,用於檢查KeyPart工站每槍掃描的KeyPart條碼是否符合設定規則,并寫stationSession以用於PassStationAPI更新進R_SN_KP
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckKeyPartScan(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string strStatus = string.Empty, strModelType = string.Empty, strScanKpSn = string.Empty, strStationNo = string.Empty;

            if (Paras.Count != 7)
            {
                //參數配置錯誤
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            //Station_No對象,必須指定.
            MESStationSession sessionStationNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStationNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            strStationNo = (string)sessionStationNo.Value;
            if (string.IsNullOrEmpty(strStationNo))
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));

            //SN主板條碼,必須指定.
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            else if (sessionSn.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            //WO對象,必須指定.
            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionWo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            if (sessionWo.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            //機種ModelType,必須指定
            MESStationSession sessionModelType = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionModelType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }
            else
            {
                strModelType = (sessionModelType.Value == null || sessionModelType.Value.ToString() == "") ? "" : sessionModelType.Value.ToString();
            }

            //掃描的KeyPart條碼,必須指定
            MESStationSession sessionKeyPartSn = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionKeyPartSn == null)
            {
                sessionKeyPartSn = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, InputValue = Input.Value.ToString(), ResetInput = Input };
                Station.StationSession.Add(sessionKeyPartSn);
            }
            strScanKpSn = (string)sessionKeyPartSn.Value;
            if (!string.IsNullOrEmpty(strScanKpSn))
                strScanKpSn = (string)Input.Value;

            //SN KeyPart清單,必須設定.
            MESStationSession sessionKpScanList = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionKpScanList == null)
            {
                sessionKpScanList = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY, InputValue = Input.Value.ToString(), ResetInput = Input };
                Station.StationSession.Add(sessionKpScanList);
            }

            //條碼PASS/FAIL狀態存取,必須指定.用於處理ERROR CODE掃描清空掃描KeyPartSession.
            MESStationSession sessionStatus = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (sessionStatus == null)
            {
                strStatus = "PASS";
            }
            else
            {
                if (string.IsNullOrEmpty((string)sessionStatus.Value))
                {
                    strStatus = "PASS";
                }
            }

            if (strStatus == "PASS")//掃描非ERROR CODE條碼則正常檢查KEYPART
            {
                OleExec sfcdb = Station.SFCDB;
                LogicObject.SN sn = new SN();
                T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
                WorkOrder woObject = (WorkOrder)sessionWo.Value;

                if (sessionSn.Value is SN)
                    sn = (SN)sessionSn.Value;
                else
                {
                    string tmpsyssn = (string)sessionSn.Value;
                    if (string.IsNullOrEmpty(tmpsyssn))
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));

                    if (Station.StationName.EndsWith("LOADING"))
                    {
                        //SI LOADING站,沒LOADING時沒有主表資料,只能自己指定.
                        sn.baseSN = new R_SN()
                        {
                            SN = tmpsyssn,
                            SKUNO = woObject.SkuNO,
                            WORKORDERNO = woObject.WorkorderNo,
                            PLANT = woObject.PLANT,
                            ROUTE_ID = woObject.RouteID
                        };
                    }
                    else
                    {
                        //普通工站直接加載SN對象
                        sn.Load(tmpsyssn, sfcdb, DB_TYPE_ENUM.Oracle);
                    }
                }

                #region 從StationSession獲取主板條碼的KeyPart掃描List和已掃的值
                List<Row_R_SN_KP> kpScanList = new List<Row_R_SN_KP>();
                if (sessionKpScanList.Value != null)
                {
                    kpScanList = (List<Row_R_SN_KP>)sessionKpScanList.Value;
                    //檢查當前在session裡面的是不是這個條碼的KeyPartList.沒有就清空重新載入.
                    if (!kpScanList.Where(p => p.SN == sn.baseSN.SN).Any())
                    {
                        kpScanList = new List<Row_R_SN_KP>();
                    }
                }
                #endregion

                #region 取得主板SN對象和當前工站需要掃描的KEYPARTLIST清單.
                if (kpScanList.Count == 0)
                {
                    //因為到LOADING站(SILOADING)才開始寫R_SN_KP,所以如果要在LOADING站做KEYPART掃描檢查只能先從系統帶出需要檢查的項目.
                    if (Station.StationName.EndsWith("LOADING"))
                    {
                        //SI LOADING站,沒LOADING時沒有主表資料,只能自己指定.

                        T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(sfcdb, Station.DBType);
                        T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(sfcdb, Station.DBType);
                        T_C_KP_List_Item_Detail t_c_kp_list_item_detail = new T_C_KP_List_Item_Detail(sfcdb, Station.DBType);
                        T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, Station.DBType);
                        T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(sfcdb, Station.DBType);
                        T_C_KP_Rule c_kp_rule = new T_C_KP_Rule(sfcdb, Station.DBType);

                        //string strkplistid = t_c_kp_list.GetKPLISTID(sfcdb, woObject.KP_LIST_ID).First();
                        List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
                        List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
                        List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
                        C_KP_Rule kpRule = new C_KP_Rule();
                        int scanseq = 0;
                        string skuno = woObject.SkuNO;
                        string skuMpn = "";
                        try
                        {
                            kpItemList = t_c_kp_list_item.GetItemObjectByListId(woObject.KP_LIST_ID, sfcdb).Where(p => p.STATION == Station.StationName).ToList();
                            if (kpItemList == null || kpItemList.Count == 0)
                            {
                                //未抓取到該工站有設定要綁定的物件,請檢查設置!
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                            }
                            foreach (C_KP_List_Item kpItem in kpItemList)
                            {
                                string kpPno = kpItem.KP_PARTNO;

                                itemDetailList = t_c_kp_list_item_detail.GetItemDetailObjectByItemId(kpItem.ID, sfcdb);
                                if (itemDetailList == null || itemDetailList.Count == 0)
                                {
                                    //未抓取到該KeyPart物件的詳情,請檢查設定！
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                                }

                                skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(sfcdb, woObject.SkuNO, kpItem.KP_PARTNO);
                                if (skuMpnList.Count == 0)
                                {
                                    //throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110201", new string[] { kpItem.KP_PARTNO }));
                                }
                                skuMpn = skuMpnList[0].MPN;
                                for (int i = 0; i < kpItem.QTY; i++)
                                {
                                    scanseq = scanseq + 1;//為保證KeyPart能正常更新進R_SN_KP,scanseq必須與Loading寫入的scanseq同步!
                                    foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                                    {
                                        //scanseq = scanseq + 1;
                                        kpRule = c_kp_rule.GetKPRule(sfcdb, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                                        if (kpRule == null)//只要設定KeyPart不管什麼條碼都必須設置規則.
                                        {
                                            //throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                            //throw new MESDataObject.MESReturnMessage($@"PNO:{kpItem.KP_PARTNO},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Missing kpRule");
                                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110603", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                        }
                                        if (kpRule.REGEX == "")//只要設定KeyPart不管什麼條碼都必須設置規則.
                                        {
                                            //throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                            //throw new MESDataObject.MESReturnMessage($@"PNO:{kpItem.KP_PARTNO},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Rule in null");
                                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110857", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                        }

                                        Row_R_SN_KP trKpRow = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                                        trKpRow.ID = "";
                                        trKpRow.R_SN_ID = "";
                                        trKpRow.SN = sn.baseSN.SN;
                                        trKpRow.VALUE = "";
                                        trKpRow.PARTNO = kpItem.KP_PARTNO;
                                        trKpRow.KP_NAME = kpItem.KP_NAME;
                                        trKpRow.MPN = skuMpn;
                                        trKpRow.SCANTYPE = itemDetail.SCANTYPE;
                                        trKpRow.ITEMSEQ = kpItem.SEQ;
                                        trKpRow.SCANSEQ = scanseq;
                                        trKpRow.DETAILSEQ = itemDetail.SEQ;
                                        trKpRow.STATION = kpItem.STATION;
                                        trKpRow.REGEX = kpRule.REGEX;
                                        trKpRow.VALID_FLAG = 1;
                                        trKpRow.EXKEY1 = "";
                                        trKpRow.EXVALUE1 = "";
                                        trKpRow.EXKEY2 = "";
                                        trKpRow.EXVALUE2 = "";
                                        trKpRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                                        trKpRow.EDIT_TIME = Station.GetDBDateTime();
                                        kpScanList.Add(trKpRow);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else//非LOADING工站直接從R_SN_KP抓取需要掃KEYPART的條碼.
                    {

                        T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, Station.DBType);
                        List<R_SN_KP> r_sn_kp_list = TRKP.GetKPRecordBySnIDStation(sn.ID, Station.StationName, sfcdb);
                        foreach (R_SN_KP r_sn_kp in r_sn_kp_list)
                        {
                            Row_R_SN_KP trKpRow = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                            trKpRow.ID = r_sn_kp.ID;
                            trKpRow.R_SN_ID = r_sn_kp.R_SN_ID;
                            trKpRow.SN = r_sn_kp.SN;
                            trKpRow.VALUE = r_sn_kp.VALUE;
                            trKpRow.PARTNO = r_sn_kp.PARTNO;
                            trKpRow.KP_NAME = r_sn_kp.KP_NAME;
                            trKpRow.MPN = r_sn_kp.MPN;
                            trKpRow.SCANTYPE = r_sn_kp.SCANTYPE;
                            trKpRow.ITEMSEQ = r_sn_kp.ITEMSEQ;
                            trKpRow.SCANSEQ = r_sn_kp.SCANSEQ;
                            trKpRow.DETAILSEQ = r_sn_kp.DETAILSEQ;
                            trKpRow.STATION = r_sn_kp.STATION;
                            trKpRow.REGEX = r_sn_kp.REGEX;
                            trKpRow.VALID_FLAG = 1;
                            trKpRow.EXKEY1 = r_sn_kp.EXKEY1;
                            trKpRow.EXVALUE1 = r_sn_kp.EXVALUE1;
                            trKpRow.EXKEY2 = r_sn_kp.EXKEY2;
                            trKpRow.EXVALUE2 = r_sn_kp.EXVALUE2;
                            trKpRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                            trKpRow.EDIT_TIME = Station.GetDBDateTime();
                            kpScanList.Add(trKpRow);
                        }
                    }
                }
                #endregion

                if (kpScanList.Count > 0)//檢查這個站是否需要掃KEYPART
                {

                    #region 從R_AP_TEMP取得用戶當前掃描的是第幾槍
                    //考慮到存在KeyPart條碼Check沒有問題更新成功進Session但其它檢查或過站API拋出異常掃描失敗導致KeyPart掃描不同步問題,使用R_AP_TEMP來判斷現在KeyPart掃的是哪一槍
                    int scancount = 0;
                    T_R_AP_TEMP tap = new T_R_AP_TEMP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    List<R_AP_TEMP> aplist = tap.GetRecordByStationNo(Station.SFCDB, strStationNo);
                    scancount = aplist.Where(p => p.DATA8 == "KEYPART").ToList().Count - 1;
                    if (scancount > kpScanList.Count || scancount < 0)
                    {
                        //拋異常要求用戶從UNDO開始重新掃描.
                        sessionKpScanList.Value = new List<Row_R_SN_KP>();
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20200624141822", new string[] { }));
                    }
                    #endregion

                    kpScanList[scancount].VALUE = strScanKpSn;

                    #region 更新當前掃描的ScanTYPE到R_AP_TEMP表
                    string currentscantype = kpScanList[scancount].SCANTYPE;
                    string nextscantype = (kpScanList.Count - 1) > scancount ? kpScanList[scancount + 1].SCANTYPE : "";
                    tap.UpdataApScanTypeStation(Station.SFCDB, strStationNo, aplist[aplist.Count - 1].DATA3, currentscantype, nextscantype);
                    #endregion

                    #region 掃描條碼預先處理,針對ACCY類型物料對掃入條碼加時間後綴作為綁定SN
                    snReprocess(sfcdb, scancount, ref kpScanList, sn, null);
                    #endregion

                    List<Row_R_SN_KP> tmpKpList = kpScanList.Where(p => p.SN == sn.baseSN.SN && p.STATION == Station.StationName && p.VALUE == "").ToList();
                    if (tmpKpList.Count > 0)//只檢查當前掃描的SN的規則
                    {
                        checkSnKeyPart(sfcdb, Station.DBType, scancount, kpScanList, sn, null, Station.API, strModelType);
                    }
                    else//最後一槍,對當前工站掃KeyPart的所有項目重新檢查
                    {
                        for (int i = 0; i < kpScanList.Count; i++)
                        {
                            checkSnKeyPart(sfcdb, Station.DBType, i, kpScanList, sn, null, Station.API, strModelType);
                        }
                    }
                    sessionKpScanList.Value = kpScanList;
                }
            }
            else//掃描ERROR CODE則清除KEYPART SESSION
            {
                //sessionKpScanList.Value = new List<Row_R_SN_KP>();
                sessionKpScanList.Value = null;
            }
        }

        protected static bool checkSnKeyPart(OleExec sfcdb, DB_TYPE_ENUM dbtype, int scansteps, List<Row_R_SN_KP> scanlist, SN SN, KeyPart.SN_KP config, MesAPIBase api, string modeltype)
        {
            try
            {
                string rulestr = scanlist[scansteps].REGEX;
                string checksn = scanlist[scansteps].VALUE;

                #region 使用正則表達式或舊MBDSFC條碼規則檢查條碼
                //配ModelType:115採用舊MBD SFC條碼檢查規則檢查條碼,否則採用默認正則表達式規則檢查條碼規則.
                if (modeltype.IndexOf("115") >= 0)
                {
                    if (rulestr.Length != checksn.Length)
                    {
                        //條碼錯誤
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { checksn, rulestr }));
                    }
                    else
                    {
                        char[] ruleAry = new char[rulestr.Length];
                        ruleAry = rulestr.ToArray();
                        char[] checksnAry = new char[checksn.Length];
                        checksnAry = checksn.ToArray();

                        for (int i = 0; i < ruleAry.Length; i++)
                        {

                            if (ruleAry[i] == '#')//0~9
                            {
                                if (!(checksnAry[i] >= 48 && checksnAry[i] <= 57))
                                {
                                    //條碼錯誤,不是數字
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { checksn, rulestr }));
                                }
                            }
                            else if (ruleAry[i] == '!')//A~Z
                            {
                                if (!(checksnAry[i] >= 65 && checksnAry[i] <= 90))
                                {
                                    //條碼錯誤,不是大寫字母
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { checksn, rulestr }));
                                }
                            }
                            else if (ruleAry[i] == '*')//0~9 or A~Z
                            {
                                if (!((checksnAry[i] >= 48 && checksnAry[i] <= 57) || (checksnAry[i] >= 65 && checksnAry[i] <= 90)))
                                {
                                    //條碼錯誤,不是數字或者大寫字母
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { checksn, rulestr }));
                                }
                            }
                            else
                            {
                                if (ruleAry[i] != checksnAry[i])
                                {
                                    //條碼錯誤,值不匹配
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { checksn, rulestr }));
                                }
                            }
                        }
                    }
                }
                else
                {
                    //使用正則表達式檢查條碼規則
                    Regex regex = new Regex(rulestr);
                    if (!regex.IsMatch(checksn))
                    {
                        //條碼錯誤,不符合設定條碼規則
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { checksn, rulestr }));
                    }
                }
                #endregion

                #region
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, dbtype);
                List<R_SN_KP> r_sn_kp = t_r_sn_kp.GetBYSNSCANTYPE(checksn, 1, sfcdb);
                if (r_sn_kp.Any())
                {
                    //KEYPARTSN:{0}已經與SN:{1}綁定！請檢查.
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200624150902", new string[] { checksn, r_sn_kp[0].SN }));
                }
                #endregion

                #region KeyPart條碼對應的ScanType檢查
                var ScanTypes = sfcdb.ORM.Queryable<C_KP_Check>().Where(t => t.TYPENAME == scanlist[scansteps].SCANTYPE).ToList();
                if (ScanTypes.Count > 0)
                {
                    Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + ScanTypes[0].DLL);
                    Type APIType = assembly.GetType(ScanTypes[0].CLASS);
                    object API_CLASS = assembly.CreateInstance(ScanTypes[0].CLASS);
                    var Methods = APIType.GetMethods();
                    var Funs = Methods.Where<MethodInfo>(t => t.Name == ScanTypes[0].FUNCTION);
                    if (Funs.Count() > 0)
                    {
                        Funs.ElementAt(0).Invoke(API_CLASS, new object[] { config, SN, scanlist[scansteps], scanlist, api, sfcdb });
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        protected static void snReprocess(OleExec sfcdb, int scancount, ref List<Row_R_SN_KP> scanlist, SN SN, KeyPart.SN_KP config)
        {
            //For ACCY:FinalKeypartValue = SCANVALUE||00000002||YYMMDDHH24MISSFF6
            if (scanlist[scancount].SCANTYPE.StartsWith("ACCY"))
            {
                //.
                //scanlist[scancount].VALUE = scanlist[scancount].VALUE + "???XXXVVVCCC";
            }
        }
    }
}
