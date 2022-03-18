using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using MESPubLab.Json;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class CMCDataLoader
    {
        public static void SNorFailCodeDataLoador(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;


            MESStationSession FAILCODE_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (FAILCODE_Session == null)
            {
                FAILCODE_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FAILCODE_Session);
            }

            string FailSNInputName = Paras[1].VALUE;

            string inputStr = Input.Value.ToString();
            var ERRCODE = DB.ORM.Queryable<C_ERROR_CODE>().Where(t => t.ERROR_CODE == inputStr).First();
            if (ERRCODE != null)
            {
                var ni = Station.Inputs.Find(t => t.DisplayName == FailSNInputName);
                if (ni != null)
                {
                    Station.NextInput = ni;
                }
                else
                {
                    throw new Exception($@"Can't find Input'{FailSNInputName}'");
 
                }
                FAILCODE_Session.Value = ERRCODE;
                //告诉工站引擎不执行以后的Action
                Station.CurrActionRrturn = StationActionReturn.PassStopRunNext;

            }


        }

        /// <summary>
        /// 从测试数据解析数据信息到SESSION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ICTAnDataLoador(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            MESStationSession TestDataSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TestDataSession == null)
            {
                TestDataSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TestDataSession);
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == "STATUS" && t.SessionKey == "1");
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = "STATUS", InputValue = Input.Value.ToString(), Value = "", SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (StatusSession.Value == null ||
                    (StatusSession.Value != null && StatusSession.Value.ToString() == ""))
                {
                    StatusSession.Value = "PASS";
                }
            }


            string inputStr = Input.Value.ToString();
            MESStation.Config.CMC.TMNConfig tMN = new Config.CMC.TMNConfig();
            MESStation.Config.CMC.DataItem dataItem = tMN.AnICTData(inputStr);
            TestDataSession.Value = dataItem;
            StatusSession.Value = Config.CMC.TMNConfig.GetDataForDataItem(dataItem, "STATUS");

        }

        /// <summary>
        /// 从测试数据对象加载SN对象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param> 
        public static void ICTTestSNDataLoador(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            MESStationSession TestDataSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TestDataSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }

            MESStation.Config.CMC.DataItem dataItem = (MESStation.Config.CMC.DataItem)TestDataSession.Value;

            //if (dataItem.keyValuePairs.Where(s => s.Key == "STATUS").ToArray()[0].Value == "PASS")
            //{

            //}
            string SN = dataItem.keyValuePairs.Where(s => s.Key == "SN").ToArray()[0].Value;
            SN SNObj = null;
            try
            {
                SNObj = new SN(SN, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(SNObj.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + SN }));
                }
                SNSession.Value = SNObj;
                Station.AddMessage("MES00000029", new string[] { "SN:", SN }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 從SN對象加載SN值到SESSION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNStringBySNObjDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
               // throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143915"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }

            MESStationSession SNStringSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNStringSession == null)
            {
                SNStringSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNStringSession);
            }

            MESStationSession SkuStringSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SkuStringSession == null)
            {
                SkuStringSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuStringSession);
            }

            if (Input.Value == null || (Input.Value != null && Input.Value.ToString().Equals("")))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616103112", new string[] { "SN" }));
            }
            SN SNObj = (SN)SNSession.Value;
            SNStringSession.Value = SNObj.SerialNo;
            SkuStringSession.Value = SNObj.SkuNo;

        }

        /// <summary>
        /// 从测试数据对象加载FailCode
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param> 
        public static void ICTTestFailCodeDataLoador(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            MESStationSession TestDataSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TestDataSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession FailCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FailCodeSession == null)
            {
                FailCodeSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FailCodeSession);
            }

            MESStation.Config.CMC.DataItem dataItem = (MESStation.Config.CMC.DataItem)TestDataSession.Value;
            string FAILCODE = "";
            C_ERROR_CODE error = null;
            if (dataItem.keyValuePairs.Where(s => s.Key == "STATUS").ToArray()[0].Value == "FAIL")
            {
                FAILCODE = dataItem.keyValuePairs.Where(s => s.Key == "FAILCODE").ToArray()[0].Value;
                T_C_ERROR_CODE t_C_ERROR_CODE = new T_C_ERROR_CODE(DB, DB_TYPE_ENUM.Oracle);

                error = t_C_ERROR_CODE.GetByErrorCode(FAILCODE, DB);
                if (error == null)
                {
                    error = new C_ERROR_CODE();
                    error.ID = t_C_ERROR_CODE.GetNewID(Station.BU, DB);
                    error.ERROR_CODE = FAILCODE;
                    error.ENGLISH_DESC = FAILCODE;
                    error.CHINESE_DESC = FAILCODE;
                    error.EDIT_EMP = Station.User.EMP_NO;
                    error.EDIT_TIME = t_C_ERROR_CODE.GetDBDateTime(DB);
                    int c = t_C_ERROR_CODE.AddNewErrorCode(error, DB);
                    if (c > 0)
                    {
                        error = t_C_ERROR_CODE.GetByErrorCode(FAILCODE, DB);
                    }
                }

            }
            FailCodeSession.Value = error;
        }

        /// <summary>
        /// 将测试数据存放到R_JSON
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param> 
        public static void TestDataSaveToR_Json(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec DB = Station.SFCDB;
            MESStationSession Station_NOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Station_NOSession == null)
            {
                Station_NOSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Station_NOSession);
            }
            //SN SNObj = new SN(inputValue, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string TestDataStr = Input.Value.ToString();
            int index = 0;
            string SNStr = "";
            string VER="";
            string error = "";
            try
            {
                Station_NOSession.Value = TestDataStr.Split('|')[0];
                TestDataStr = TestDataStr.Split('|')[1];
                T_R_SN_KP key = new T_R_SN_KP(DB, DB_TYPE_ENUM.Oracle);
                T_R_SN tsn = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
                if (TestDataStr.IndexOf("<<<") > 0)
                {
                    VER = TestDataStr.Substring(TestDataStr.IndexOf("<<<"), TestDataStr.Length - TestDataStr.IndexOf("<<<")).Replace("<<<","");
                    TestDataStr = TestDataStr.Substring(0, TestDataStr.IndexOf("<<<"));
                }

                if (TestDataStr.IndexOf("1>>") >= 0)
                {
                    index = 1;
                    bool ADD = true;
                    TestDataStr = TestDataStr.Replace("1>>", "");
                    SNStr = TestDataStr.Split(',')[0];
                    R_SN_KP keypart = key.GetSISn(SNStr, DB);
                    if (keypart != null)
                    {
                        SNStr = keypart.SN;
                    }
                    R_SN sN = tsn.GetSNByBoxSN(SNStr, DB);
                    if (sN != null)
                    {
                        SNStr = sN.SN;
                    }
                    MESStation.Config.CMC.OneDataItem oneDataItem =
                        JsonSave.GetFromDB<MESStation.Config.CMC.OneDataItem>(Station_NOSession.Value.ToString(), "1", SNStr, DB);
                    if (oneDataItem != null)
                    {
                        ADD = false;
                    }
                    else
                    {
                        ADD = true;
                        oneDataItem = new Config.CMC.OneDataItem();
                    }
                    List<string> vs = new List<string>();
                    foreach (string item in TestDataStr.Split(','))
                    {
                        vs.Add(item);
                    }
                    oneDataItem.SN = SNStr;
                    oneDataItem.ONE = vs;
                    if (!ADD)
                        JsonSave.UpdateToDB(oneDataItem, Station_NOSession.Value.ToString(), "1", SNStr, Station_NOSession.Value.ToString(), DB, "");
                    else
                        JsonSave.SaveToDB(oneDataItem, Station_NOSession.Value.ToString(), "1", Station_NOSession.Value.ToString(), DB, "", SNStr);
                }

                if (TestDataStr.IndexOf("2>>") >= 0)
                {
                    index = 2;
                    bool ADD = true;
                    TestDataStr = TestDataStr.Replace("2>>", "");
                    SNStr = TestDataStr.Split(';')[0];
                    R_SN_KP keypart = key.GetSISn(SNStr, DB);
                    if (keypart != null)
                    {
                        SNStr = keypart.SN;
                    }
                    R_SN sN = tsn.GetSNByBoxSN(SNStr, DB);
                    if (sN != null)
                    {
                        SNStr = sN.SN;
                    }
                    TestDataStr = TestDataStr.Split(';')[1];
                    string Status = TestDataStr.Split('#')[1];
                    TestDataStr = TestDataStr.Split('#')[0];
                    MESStation.Config.CMC.OneDataItem oneDataItem =
                       JsonSave.GetFromDB<MESStation.Config.CMC.OneDataItem>(Station_NOSession.Value.ToString(), "1", SNStr, DB);
                    if (oneDataItem != null)
                    {
                        MESStation.Config.CMC.TwoDataItem twoDataItem =
                            JsonSave.GetFromDB<MESStation.Config.CMC.TwoDataItem>(Station_NOSession.Value.ToString(), "2", SNStr, DB);
                        //Config.CMC.TwoDataItem twoDataItem;
                        if (twoDataItem != null)
                        {
                            ADD = false;
                        }
                        else
                        {
                            ADD = true;
                            twoDataItem = new Config.CMC.TwoDataItem();
                        }
                        twoDataItem.SN = SNStr;
                        List<string> vs = new List<string>();
                        foreach (string item in TestDataStr.Split(','))
                        {
                            vs.Add(item);
                        }
                        twoDataItem.SN = SNStr;
                        twoDataItem.Data = vs;
                        twoDataItem.Status = Status;
                        twoDataItem.TestDataString = TestDataStr;
                        if (!ADD)
                            JsonSave.UpdateToDB(twoDataItem, Station_NOSession.Value.ToString(), "2", SNStr, Station_NOSession.Value.ToString(), DB, "");
                        else
                            JsonSave.SaveToDB(twoDataItem, Station_NOSession.Value.ToString(), "2", Station_NOSession.Value.ToString(), DB, "", SNStr);
                    }
                    else
                    {
                        //throw new MESReturnMessage("沒有進行第一步測試！請先拋第一步！");
                        
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144604"));
                    }
                }
            }
            catch
            {
                // throw new MESReturnMessage("測試數據解析失敗！發生未知錯誤！請檢查拋磚質料是否有誤！或者沒有進行第一步測試！請先拋第一步！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145307"));
            }
            Station.NextInput = Station.Inputs[0];
            if (Station.Inputs.Count > 1)
            {
                Station.Inputs[index].Value = SNStr;
                Station.Inputs[index].Run(); 
            }
        }


        /// <summary>
        /// For CMC條碼綁定工站(ASSY&SILOADING)掃描檢查(涉及掃描步驟判斷處理需要表R_AP_TEMP)  
        /// For南寧MBD CMC掃描模式開發,用於加載掃入的主板條碼的KeyPart掃描清單.
        /// add NN.MBD YCX 2020/05/13 tips:目前是(南寧MBD)KeyPart過站(ASSY&SIOADING)主API
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadKeyPartScanList(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //SN主板條碼,必須指定.
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            else if (sessionSn.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            //WO對象,必須指定.
            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionWo.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            //SN KeyPart清單,必須設定.
            MESStationSession sessionKpScanList = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionKpScanList == null)
            {
                sessionKpScanList = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, InputValue = Input.Value.ToString(), ResetInput = Input };
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
                //檢查當前在session裡面的是不是這個條碼的KeyPartList.沒有就清空重新載入.
                if (!kpScanList.Where(p => p.SN == sn.baseSN.SN).Any())
                {
                    kpScanList = new List<Row_R_SN_KP>();
                }
            }
            #endregion

            #region 取得主板SN對象在當前工站需要掃描的KEYPARTLIST清單并寫入StationSession,同一SN已經加載過KeyPart清單的就不重復加載了.
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
                                throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");
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
                                        throw new MESDataObject.MESReturnMessage($@"PNO:{kpItem.KP_PARTNO},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Missing kpRule");
                                    }
                                    if (kpRule.REGEX == "")//只要設定KeyPart不管什麼條碼都必須設置規則.
                                    {
                                        //throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                                        throw new MESDataObject.MESReturnMessage($@"PNO:{kpItem.KP_PARTNO},MPN:{skuMpn},SCANTYPE:{itemDetail.SCANTYPE} Rule in null");
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
                sessionKpScanList.Value = kpScanList;
            }
            #endregion
        }
    }
}
