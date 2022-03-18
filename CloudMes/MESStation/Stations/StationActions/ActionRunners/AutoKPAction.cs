using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.OM;
using MESDataObject.Module.Juniper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MESPubLab.Json;
using MESStation.Interface.Juniper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class AutoKPAction
    {
        public class SnC
        {
            public string RuleName { get; set; }
            public string SN { get; set; }
            public int from { get; set; }
            public int len { get; set; }
        }

        static void makeOutput(UIInputData I, JuniperAutoKpConfig c, R_SN_KP d, int seq)
        {
            I.OutInputs.Clear();
            DisplayOutPut Sku = new DisplayOutPut() { Name = "P_NO", Value = c.PN, DisplayType = UIOutputType.TextArea.ToString() };
            DisplayOutPut SEQ = new DisplayOutPut() { Name = "SEQ", Value = $@"{seq}/{c.QTY}", DisplayType = UIOutputType.TextArea.ToString() };
            DisplayOutPut SN = new DisplayOutPut() { Name = "SN", Value = d.VALUE, DisplayType = UIOutputType.TextArea.ToString() };
            DisplayOutPut PN_7xx = new DisplayOutPut() { Name = "PN_7xx", Value = d.MPN, DisplayType = UIOutputType.TextArea.ToString() };
            DisplayOutPut Rev = new DisplayOutPut() { Name = "Rev", Value = d.EXVALUE1, DisplayType = UIOutputType.TextArea.ToString() };
            DisplayOutPut CLEI = new DisplayOutPut() { Name = "CLEI", Value = d.EXVALUE2, DisplayType = UIOutputType.TextArea.ToString() };
            DisplayOutPut COO = new DisplayOutPut() { Name = "COO", Value = d.LOCATION, DisplayType = UIOutputType.TextArea.ToString() };
            I.OutInputs.Add(Sku);
            I.OutInputs.Add(SEQ);
            if (c.PN_SERIALIZATION == "YES")
            {
                I.OutInputs.Add(SN);
            }
            I.OutInputs.Add(PN_7xx);
            I.OutInputs.Add(Rev);
            I.OutInputs.Add(CLEI);
            I.OutInputs.Add(COO);


        }

        private static string strContains = "~`!@#$%^&*(){​​}​​[]:;',.<>?/=+_|%<> \\\"";
        public static void JuniperAutoKP(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100146"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100639"));
            }

            MESStationSession CatchSession = Station.StationSession.Find(t => t.MESDataType == "AutoKpCatch" && t.SessionKey == "1");
            if (CatchSession == null)
            {
                CatchSession = new MESStationSession()
                {
                    SessionKey = "1",
                    MESDataType = "AutoKpCatch",
                    Value = new Dictionary<string, AutoKpCatch>()
                };
                Station.StationSession.Add(CatchSession);
            }

            var Catchs = (Dictionary<string, AutoKpCatch>)CatchSession.Value;

            var WO = (WorkOrder)WOSession.Value;
            var SN = (SN)SNSession.Value;

            var db = Station.SFCDB;
            bool isScanTRSN = false;

            var cs = db.ORM.Queryable<C_SERIES, C_SKU>((C, S) => C.ID == S.C_SERIES_ID)
                    .Where((C, S) => S.SKUNO == WO.SkuNO)
                    .Select((C, S) => C)
                    .First();
            var order_main = db.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == WO.WorkorderNo).First();
            //var order_main = db.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == "007A00000021").First();
            if (order_main == null)
            {
                return;
                //throw new Exception("WO not exists in O_ORDER_MAIN, please check!");
            }
            var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(order_main.PREWO, "JuniperAutoKPConfig", db);
            if (kpl.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100753"));
            }
            double itemseq = 500;

            #region //從前段的KP帶資料
            var hbconfig = kpl.FindAll(t => t.TYPE == "SAP_HB");

            //獲取綁定關係
            var snr = db.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == SN.ID && t.SCANTYPE == "KEEP_SN" && t.VALID_FLAG == 1).First();
            if (snr != null)
            {
                //查詢原來SN上的所有KP
                var oldKps = db.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == snr.EXVALUE1 && t.VALID_FLAG == 1).ToList();
                //處理原有的kp
                for (int i = 0; i < hbconfig.Count; i++)
                {
                    var c = hbconfig[i].QTY;
                    var kpsn = oldKps.FindAll(t => t.PARTNO == hbconfig[i].PN_7XX & t.SCANTYPE == "SN");
                    if (kpsn.Count == c)
                    {
                        bool isOK = true;
                        List<R_SN_KP> k = new List<R_SN_KP>();

                        for (int j = 0; j < kpsn.Count; j++)
                        {
                            var _7xxPn = hbconfig[i].PN_7XX;
                            var kpval = kpsn[j].VALUE;
                            var CLEI = oldKps.Find(t => t.SCANTYPE == "CLEI" && t.PARTNO == _7xxPn && t.ITEMSEQ == kpsn[j].ITEMSEQ)?.VALUE;
                            var COO = oldKps.Find(t => t.SCANTYPE == "COO" && t.PARTNO == _7xxPn && t.ITEMSEQ == kpsn[j].ITEMSEQ)?.VALUE;
                            var REV = oldKps.Find(t => t.SCANTYPE == "REV" && t.PARTNO == _7xxPn && t.ITEMSEQ == kpsn[j].ITEMSEQ)?.VALUE;
                            if (CLEI == null)
                            {
                                isOK = false;
                                break;
                                //throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100909", new string[] { hbconfig[i].PN_7XX, kpsn[j].ITEMSEQ.ToString() }));
                            }
                            if (CLEI != hbconfig[i].CLEI_CODE)
                            {
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101123", new string[] { hbconfig[i].PN_7XX, kpsn[j].ITEMSEQ.ToString(), CLEI, hbconfig[i].CLEI_CODE }));
                            }

                            if (COO == null)
                            {
                                isOK = false;
                                break;
                                //throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101701", new string[] { hbconfig[i].PN_7XX, kpsn[j].ITEMSEQ.ToString() }));
                            }
                            if (REV == null)
                            {
                                isOK = false;
                                break;
                                //throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814101940", new string[] { hbconfig[i].PN_7XX, kpsn[j].ITEMSEQ.ToString() }));
                            }
                            if (REV != hbconfig[i].REV)
                            {
                                isOK = false;
                                break;
                                //throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102110", new string[] { hbconfig[i].PN_7XX, kpsn[j].ITEMSEQ.ToString(), REV, hbconfig[i].REV }));
                            }
                            R_SN_KP KP = new R_SN_KP()
                            {
                                ID = MesDbBase.GetNewID(db.ORM, Station.BU, "R_SN_KP"),
                                R_SN_ID = SN.ID,
                                SN = SN.SerialNo,
                                ITEMSEQ = itemseq++,
                                SCANSEQ = j + 1,
                                DETAILSEQ = 1,
                                STATION = Station.StationName,
                                VALID_FLAG = 1,
                                EDIT_EMP = Station.LoginUser.EMP_NO,
                                KP_NAME = "AutoKP_SAP_HB",
                                SCANTYPE = "SN",
                                VALUE = kpval,
                                PARTNO = hbconfig[i].PN,
                                MPN = _7xxPn,
                                LOCATION = COO,
                                EXKEY1 = "REV",
                                EXVALUE1 = REV,
                                EXKEY2 = "CLEI",
                                EXVALUE2 = CLEI,
                            };
                            k.Add(KP);
                            //db.ORM.Insertable(KP).ExecuteCommand();
                        }

                        if (isOK)
                        {
                            for (int j = 0; i < k.Count; j++)
                            {
                                db.ORM.Insertable(k[j]).ExecuteCommand();
                            }
                            kpl.Remove(hbconfig[i]);
                        }

                    }
                    else if (kpsn.Count == 0)
                    {

                        //kpl.Remove(hbconfig[i]);
                    }
                    else
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102302", new string[] { hbconfig[i].PN_7XX, c.ToString(), kpsn.Count.ToString() }));

                    }


                }
            }

#endregion




           


            for (int i = 0; i < kpl.Count; i++)
            {
                if (i == 0 && order_main.POTYPE == "BTS")
                {
                    isScanTRSN = true;
                    isScanTRSN = false;
                }


                if (!Catchs.Keys.Contains(kpl[i].PN))
                {
                    AutoKpCatch c = new AutoKpCatch();
                    Catchs.Add(kpl[i].PN, c);
                }
                AutoKpCatch dataCatch = Catchs[kpl[i].PN];
                for (int j = 0; j < kpl[i].QTY; j++)
                {
                    R_SN_KP KP = new R_SN_KP()
                    {
                        ID = MesDbBase.GetNewID(db.ORM, Station.BU, "R_SN_KP"),
                        R_SN_ID = SN.ID,
                        SN = SN.SerialNo,
                        ITEMSEQ = itemseq++,
                        SCANSEQ = j + 1,
                        DETAILSEQ = 1,
                        STATION = Station.StationName,
                        VALID_FLAG = 1,
                        EDIT_EMP = Station.LoginUser.EMP_NO,
                        KP_NAME = "AutoKP",
                        SCANTYPE = "SN"
                    };

                    var row = kpl[i];
                    if (row.TYPE == "SAP_HB" || row.TYPE == "I137")
                    {
                        KP.KP_NAME = KP.KP_NAME + "_" + row.TYPE;
                    }





                    #region CHAS
                    //if (row.TYPE == "PNO-SYS2")
                    //{
                    //    throw new NotImplementedException("检查逻辑未定义");
                    //    UIInputData I2 = new UIInputData()
                    //    {
                    //        MustConfirm = false,
                    //        Timeout = 3000000,
                    //        IconType = IconType.None,
                    //        UIArea = new string[] { "90%", "90%" },
                    //        Message = "PNO-SYS2",
                    //        Tittle = "AutoKP",
                    //        Type = UIInputType.String,
                    //        Name = row.PN,
                    //        ErrMessage = "No input"
                    //    };

                    //    I2.OutInputs.Add(new DisplayOutPut()
                    //    {
                    //        Name = "PNO-SYS2",
                    //        DisplayType = UIOutputType.TextArea.ToString(),
                    //        Value = row.PN
                    //    });
                    //    var strSN = I2.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                    //    //检查逻辑
                    //    //

                    //    KP.VALUE = strSN;
                    //    KP.PARTNO = row.PN;
                    //    KP.EDIT_TIME = db.ORM.GetDate();
                    //    db.ORM.Insertable<R_SN_KP>(KP).ExecuteCommand();
                    //    continue;
                    //}
                    #endregion

                    UIInputData I = new UIInputData()
                    {
                        MustConfirm = false,
                        Timeout = 3000000,
                        IconType = IconType.None,
                        UIArea = new string[] { "90%", "90%" },
                        Message = "SN",
                        Tittle = "AutoKP",
                        Type = UIInputType.String,
                        Name = row.CUST_PN + "_" + row.PN_7XX,
                        ErrMessage = "No input",
                        CBMessage = ""
                    };

                    makeOutput(I, row, KP, j);


                    //主料號或KP
                    //while (true)
                    KP.PARTNO = row.PN;
                    KP.VALUE = "N/A";
                    KP.SCANTYPE = "NOValue";
                    if (row.PN_SERIALIZATION == "YES")
                    {
                        I.CBMessage = "";
                        I.Message = "SN";
                        while (true)
                        {
                            makeOutput(I, row, KP, j);
                            var strSN = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();
                            //锁定检查
                            //暫時以這個判斷廠内與廠外產品
                            var FoxSN = db.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG != "0").First();
                            //if (FoxSN != null)
                            //{
                                
                            //}
                            SN.LockCheck(strSN.Trim(), db);
                            FoxSN = db.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").First();
                            if (FoxSN != null && row.PN_7XX != null && row.PN_7XX != "")
                            {
                                if (FoxSN.SKUNO == "711-062048" && row.PN_7XX == "750-062050")
                                {
                                    // Pass Check 7xxPN. 
                                }
                                else if 
                                    (
                                        !(
                                        FoxSN.SKUNO == row.PN_7XX 
                                        || FoxSN.SKUNO + "-FVN" == row.PN_7XX 
                                        || FoxSN.SKUNO + "-TAA" == row.PN_7XX
                                        || FoxSN.SKUNO + "-FJZ" == row.PN_7XX
                                        ) 
                                    && KP.SN != FoxSN.SN
                                    )
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = $@"SN:'{FoxSN.SN}' SKUNO:'{FoxSN.SKUNO}' not match 7xx-PN '{row.PN_7XX}'";
                                    continue;
                                }
                                else if (FoxSN.SKUNO != row.PN_7XX && KP.SN == FoxSN.SN)
                                {
                                    var kp1 = db.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == KP.R_SN_ID && t.SCANTYPE == "KEEP_SN").First();
                                    if (kp1 != null)
                                    {
                                        var oldsn = db.ORM.Queryable<R_SN>().Where(t => t.ID == kp1.EXVALUE1).First();
                                        if (oldsn.SKUNO == "711-062048" && row.PN_7XX == "750-062050")
                                        {
                                            // Pass Check 7xxPN. 
                                        }
                                        else
                                        {
                                            if (oldsn.SKUNO != row.PN_7XX)
                                            {
                                                I.OutInputs.Clear();
                                                I.CBMessage = $@"SN:'{oldsn.SN}' SKUNO:'{oldsn.SKUNO}' not match 7xx-PN '{row.PN_7XX}'";
                                                continue;
                                            }
                                        }
                                        
                                        
                                    }
                                }
                            }


                                //对于OFFERINGTYPE == "Advanced Fixed System"检查是否绑定了物料信息
                            if (i == 1 && order_main.OFFERINGTYPE == "Advanced Fixed System" && cs.SERIES_NAME == "Juniper-Optics")
                            {
                                var kp = db.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strSN && t.KP_NAME == "LINK_TR_SN" && t.SCANTYPE == "TR_SN" && t.VALID_FLAG == 1).First();
                                if (kp == null)
                                {

                                    I.OutInputs.Clear();
                                    //I.CBMessage = $@"{strSN} don't scan MATL_LINK.";
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102733", new string[] { strSN });
                                    continue;
                                }
                                if (kp.PARTNO != row.PN_7XX)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103206", new string[] { strSN, kp.PARTNO, row.PN_7XX });
                                    continue;
                                }
                            }

                            if (strSN != "NO INPUT")
                            {
                                //add by hgb 2022.03.10 CTO_KIT_NEW data
                                #region add by hgb 2022.03.10 CTO_KIT_NEW data
                                var cto_kit_d = db.ORM.Queryable<R_JNP_PD_KIT_DETAIL>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").First();
                                if (cto_kit_d != null)
                                {
                                    cto_kit_d.VALID_FLAG = "0";
                                    cto_kit_d.EDIT_BY = Station.LoginUser.EMP_NO;
                                    cto_kit_d.EDIT_TIME = DateTime.Now;
                                    db.ORM.Updateable(cto_kit_d).Where(t => t.ID == cto_kit_d.ID).ExecuteCommand();
                                }
                                 
                                #endregion add by hgb 2022.03.10 CTO_KIT_NEW data


                                //只有在LOADING工站需要掃描CLEI的KP才需要去掃描的SN是否與CLEI掃描的KP是否正確
                                //if (row.CLEI_CODE != "" && Station.DisplayName.Contains("SILOADING_CTO") && strSN != _keys[0] && _key[0].StartsWith("[)>0625SLBJPNW"))
                                //{
                                //    I.OutInputs.Clear();
                                //    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20211125165252", new string[] { strSN, _keys[0] });
                                //    continue;
                                //}

                                //檢查SN是否完工
                                var kpsn = db.ORM.Queryable<R_SN>().Where(t => t.SN == strSN && t.VALID_FLAG == "1").First();
                                if (kpsn != null)
                                {
                                    //MakePart: if no checkout from supermarket, throw exception. Asked By PE 譚義康 2022-02-11
                                    var smObj = Station.SFCDB.ORM.Queryable<R_SUPERMARKET>().Where(t => t.R_SN_ID == kpsn.ID && t.STATUS == "1").ToList().FirstOrDefault();
                                    if (smObj != null)
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20220212085815", new string[] { strSN });//SN: {0} is in SuperMarket, please check.
                                        continue;
                                    }

                                    if (KP.SN != strSN && (kpsn.COMPLETED_FLAG == null || kpsn.COMPLETED_FLAG != "1"))
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { strSN });//"SN: {0}  not completed."
                                        continue;
                                    }
                                    //在MRB不允許綁定
                                    if (kpsn.CURRENT_STATION == "MRB")
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = MESReturnMessage.GetMESReturnMessage("MES00000182", new string[] { strSN });
                                        continue;
                                    }
                                    var kpcheck = db.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == strSN && t.KP_NAME.StartsWith("AutoKP") && t.VALID_FLAG == 1).ToList();

                                    if (kpcheck.FindAll(t => t.SN != KP.SN).Any())
                                    {
                                        var kptemp = kpcheck.FindAll(t => t.SN != KP.SN).First();
                                        var checksn = db.ORM.Queryable<R_SN>().Where(t => t.ID == kptemp.R_SN_ID).First();
                                        if (checksn.VALID_FLAG == "1")
                                        {
                                            I.OutInputs.Clear();
                                            I.CBMessage = $@"'{strSN}' has use to '{checksn.SN}'!";
                                            continue;
                                        }
                                    }

                                    if ((kpsn.SHIPPED_FLAG == "0" || kpsn.SHIPPED_FLAG == null) && kpsn.ID != KP.R_SN_ID)
                                    {
                                        kpsn.SHIPPED_FLAG = "1";
                                        kpsn.SHIPDATE = DateTime.Now;
                                        kpsn.EDIT_EMP = Station.LoginUser.EMP_NO;
                                        kpsn.EDIT_TIME = DateTime.Now;
                                        db.ORM.Updateable(kpsn).Where(t => t.ID == kpsn.ID).ExecuteCommand();
                                    }


                                }
                                else
                                {
                                    //BuyPart: if no checkout from supermarket, throw exception. Asked By PE 譚義康 2022-02-11
                                    //var smObj = Station.SFCDB.ORM.Queryable<R_SUPERMARKET>().Where(t => t.R_SN_ID == strSN && t.STATUS == "1").ToList().FirstOrDefault();
                                    //if (smObj != null)
                                    //{
                                    //    I.OutInputs.Clear();
                                    //    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20220212085815", new string[] { strSN });//SN: {0} is in SuperMarket, please check.
                                    //    continue;
                                    //}

                                    //如果是BuyPart物料的SN, 則判斷是否需要檢查測試PASS記錄 Asked By PE 譚義康 20211106
                                    T_R_FUNCTION_CONTROL t_R_FUNCTION = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
                                    var rfc = t_R_FUNCTION.GetListByFcv("CHECK_BUYPART_SN_TEST_RECORD", "PARTNO", row.CUST_PN, Station.SFCDB);
                                    if (rfc.Count != 0)
                                    {
                                        T_R_TEST_RECORD t_R_TEST = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
                                        var rt = t_R_TEST.GetLastTestRecord(strSN, rfc[0].EXTVAL, Station.SFCDB);
                                        if (rt == null)
                                        {
                                            I.OutInputs.Clear();
                                            I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20190604110249");//NO TEST RECORD
                                            continue;
                                        }
                                        else if (rt.STATE != "PASS")
                                        {
                                            I.OutInputs.Clear();
                                            I.CBMessage = MESReturnMessage.GetMESReturnMessage("MES00000244", new string[] { strSN, rfc[0].EXTVAL });//{0} can't pass in the last time on {1}!
                                            continue;
                                        }
                                    }
                                }
                                //SN不能已投過
                                var rSnKp = db.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == strSN && t.VALID_FLAG == 1 && t.KP_NAME != "KEEP_SN" && t.PARTNO == KP.PARTNO).First();
                                if (rSnKp != null)
                                {
                                    if (rSnKp.SN == KP.SN && rSnKp.R_SN_ID == KP.R_SN_ID)
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = MESReturnMessage.GetMESReturnMessage("MES00000040", new string[] { strSN });
                                        continue;
                                    }
                                    else if (rSnKp.SN == KP.SN && rSnKp.R_SN_ID != KP.R_SN_ID && rSnKp.PARTNO == row.PN_7XX)
                                    {
                                        rSnKp = null;
                                    }
                                }
                                if (rSnKp != null)
                                {
                                    //如果掃描Kp的PartNo與原有綁定關係中該Kp的PartNo配置有Link關係, 則允許繼續Loading Asked By PE 譚義康 20211030
                                    T_R_LINK_CONTROL t_r_link_control = new T_R_LINK_CONTROL(Station.SFCDB, Station.DBType);
                                    var linkObj = t_r_link_control.GetControlList("SKU", KP.PARTNO, null, rSnKp.PARTNO, null, "LOADING_KEEP_AUTOKP", Station.SFCDB);
                                    if (linkObj.Count != 0)
                                    {
                                        rSnKp = null;
                                    }
                                }
                                if (rSnKp != null)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103752");
                                    continue;
                                }
                                else if (i == 0 && strSN != SN.SerialNo) //SN本階SN需與KP第一筆一樣
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103438");
                                    continue;
                                }

                                //檢查是否Checkout SM
                                var SmData = db.ORM.Queryable<R_SUPERMARKET, R_SN>((SM, RS) => SM.R_SN_ID == RS.ID).Where((SM, RS) => RS.SN == strSN && SM.STATUS == "1").Select((SM, RS) => RS).ToList();
                                if (SmData.Count > 0)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103439");
                                    continue;
                                }
                                //檢查是否Checkout SW
                                //var SwData = db.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where((SW) => SW.SN == strSN && SW.STATE_FLAG == "1").Select((SW) => SW).ToList();
                                if (db.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where((SW) => SW.SN == strSN && SW.STATE_FLAG == "1").Any())
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103440");
                                    continue;
                                }


                                else
                                {
                                    //第一筆為本階，SERIAL_NUMBER_MASK為空時需檢查用戶配置的Rule
                                    //if (i == 0 && kpl[0].SN_RULE == "")
                                    if (row.SN_RULE == "")
                                    {
                                        SN SNObj = new SN();
                                        //var csku = db.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == WO.SkuNO).First();
                                        var csku = db.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == row.PN).First();
                                        string userule = "";
                                        try
                                        {
                                            bool CheckRuleFlag = false;
                                            if (WO.WorkorderNo.Contains("007A") || WO.WorkorderNo.Contains("007C") || WO.WorkorderNo.Contains("007B"))
                                            {
                                                CheckRuleFlag = true;
                                            }
                                            var kp_rules = db.ORM.Queryable<C_KP_Rule>().Where(t => t.PARTNO == row.PN && t.SCANTYPE == "AUTOKP_SN").ToList();
                                            for (int k = 0; k < kp_rules.Count; k++)
                                            {

                                                if (Regex.IsMatch(strSN, kp_rules[k].REGEX))
                                                {
                                                    CheckRuleFlag = true;
                                                    userule = kp_rules[k].REGEX;
                                                }
                                            }
                                            if (!CheckRuleFlag)
                                            {
                                                CheckRuleFlag = SNObj.CheckSNRule(strSN, csku.SN_RULE, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                                                userule = csku.SN_RULE;
                                            }

                                            if (CheckRuleFlag)
                                            {
                                                KP.PARTNO = row.PN; //row.PN.EndsWith("-RB") == true ? row.PN.Substring(0, row.PN.IndexOf("-RB")) : row.PN;
                                                KP.VALUE = strSN;
                                                KP.REGEX = userule;
                                                KP.SCANTYPE = "SN";
                                                break;
                                            }
                                            else
                                            {
                                                I.OutInputs.Clear();
                                                I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103835");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            I.OutInputs.Clear();
                                            I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104221") + ex.Message;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //否則檢查JuniperAutoKP
                                        if (strSN != "")
                                        {
                                            try
                                            {
                                                ckeckAutoSNRule(strSN, row.SN_RULE, db);
                                                KP.PARTNO = row.PN;
                                                KP.VALUE = strSN;
                                                KP.REGEX = row.SN_RULE;
                                                KP.SCANTYPE = "SN";
                                                break;

                                            }
                                            catch (Exception ee)
                                            {
                                                I.OutInputs.Clear();
                                                I.CBMessage = ee.Message;
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            I.OutInputs.Clear();
                                            //I.OutInputs.Add(new DisplayOutPut() { DisplayType= UIOutputType. } )
                                            I.CBMessage = "Scan Error";
                                            //I.Message = "ReScan SN";
                                            //I.Name = "Data Err";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102505"));
                            }
                        }
                    }


                    KP.EXVALUE2 = "";
                    KP.EXKEY2 = "";
                    string SubCLEISN = string.Empty;
                    string CLEISN = string.Empty;
                    //CLEI
                    if (row.CLEI_CODE != "")
                    {
                        I.CBMessage = "";
                        I.Message = "CLEI";
                        while (true)
                        {
                            makeOutput(I, row, KP, j);
                            var CLEIValue = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();
                            if (CLEIValue != "NO INPUT")
                            {
                                if (CLEIValue.Trim().Contains("[)>0625SLBJPNW"))
                                {
                                    //Add by POHONG 2021/11/19
                                    if (!Station.DisplayName.Contains("SILOADING_CTO"))
                                    {
                                        SubCLEISN = SNSession.Value.ToString();
                                        CLEISN = SNSession.Value.ToString();
                                    }
                                    else
                                    {
                                        //取掃入進來的CLEI料號的SN
                                        SubCLEISN = CLEIValue.Substring(14, CLEIValue.Length - row.CLEI_CODE.Length - 17);
                                        CLEISN = CLEIValue;
                                        //ckeckAutoSNRule(SubCLEISN, row.SN_RULE, db);
                                    }
                                    if (CLEIValue.StartsWith("[") && CLEIValue.EndsWith(row.CLEI_CODE) && CLEIValue.Contains("[)>0625SLBJPNW" + SubCLEISN + "11P" + row.CLEI_CODE))
                                    {
                                        if (CLEIValue.Contains(row.CLEI_CODE))
                                        {
                                            CLEIValue = row.CLEI_CODE;
                                        }
                                        if (CLEIValue == row.CLEI_CODE)
                                        {
                                            KP.EXVALUE2 = CLEIValue;
                                            KP.EXKEY2 = "CLEI";
                                            break;
                                        }
                                        else
                                        {
                                            I.OutInputs.Clear();
                                            I.CBMessage = "Scan Error";
                                        }
                                    }
                                    else
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = "Wrong CLEI Lable,Pls Check";
                                    }
                                }
                                else
                                {
                                    if (CLEIValue.Contains(row.CLEI_CODE))
                                    {
                                        CLEIValue = row.CLEI_CODE;
                                    }
                                    if (CLEIValue == row.CLEI_CODE)
                                    {
                                        KP.EXVALUE2 = CLEIValue;
                                        KP.EXKEY2 = "CLEI";
                                        break;
                                    }
                                    else
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = "Scan Error";
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102505"));
                            }
                        }
                    }

                    //取CLEI 的SN，用於下面掃描的SN與CLEI的SN是否一致
                    List<string> _keys = new List<string>();
                    _keys.Add(SubCLEISN);
                    //用於記錄掃描進來的CLEI
                    List<string> _key = new List<string>();
                    _key.Add(CLEISN);

                    KP.MPN = "";
                    //REV
                    if (row.PN_7XX != "")
                    {
                        I.CBMessage = "";
                        I.Message = "PN";
                        while (true)
                        {
                            string PNValue = "";
                            if (dataCatch.PN_7XX != null && dataCatch.PN_7XX != "")
                            {
                                PNValue = dataCatch.PN;
                            }
                            else
                            {
                                makeOutput(I, row, KP, j);
                                PNValue = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();
                                //dataCatch.PN_7XX = PNValue;
                                ////Waite Test 2021.12.28 22.50
                                //string PnSub = PNValue.Substring(1, 3);
                                //var reg = "^[0-9]{3}$";
                                //var m = Regex.Match(PnSub, reg);
                                //if (!m.Success)
                                //{
                                //    var ModelSubPn = db.ORM.Queryable<R_MODELSUBPN_MAP>().Where(t => t.CUSTPN == PNValue).First();
                                //    if (ModelSubPn == null)
                                //    {
                                //        I.OutInputs.Clear();
                                //        I.CBMessage = "Pls config Modelsubpn!";
                                //    }
                                //    else
                                //    {
                                //        PNValue = ModelSubPn.SUBPARTNO;
                                //    }
                                //}

                                #region Get PN From Label with version, e.g 7XX-XXXXXX-XXR01,7XX-XXXXXXR01
                                var reg1 = "^(7[0-9]{2}-[0-9]{6})R[0-9]{2}$";
                                var m1 = Regex.Match(PNValue, reg1);
                                if (m1.Success)
                                {
                                    var value = m1.Groups[m1.Groups.Count - 1].Value;
                                    PNValue = value;
                                }
                                var reg2 = "^(7[0-9]{2}-[0-9]{6}-[0-9]{2})R[0-9]{2}$";
                                var m2 = Regex.Match(PNValue, reg2);
                                if (m2.Success)
                                {
                                    var value = m2.Groups[m2.Groups.Count - 1].Value;
                                    PNValue = value;
                                }
                                //if (PNValue.Length == 13)
                                //{
                                //    if (PNValue.Substring(10, 1) != "-")
                                //    {
                                //        PNValue = PNValue.Substring(0, 10);
                                //    }
                                //}
                                //if (PNValue.Length == 16)
                                //{
                                //    PNValue = PNValue.Substring(0, 13);
                                //}
                                #endregion

                            }
                            var kp_rules = db.ORM.Queryable<C_KP_Rule>().Where(t => t.PARTNO == row.PN_7XX && t.SCANTYPE == "AUTOKP_PN").First();
                            if (PNValue != "NO INPUT")
                            {
                                if (kp_rules != null)
                                {
                                    var m = Regex.Match(PNValue, kp_rules.REGEX);
                                    if (m.Success)
                                    {
                                        var value = m.Groups[m.Groups.Count - 1].Value;
                                        KP.MPN = m.Groups[m.Groups.Count - 1].Value;
                                        break;
                                        //if (value != row.PN_7XX)
                                        //{

                                        //    I.OutInputs.Clear();
                                        //    I.CBMessage = "Scan Error";
                                        //}
                                        //else
                                        //{

                                        //    KP.MPN = m.Groups[m.Groups.Count - 1].Value;
                                        //    break;
                                        //}
                                    }
                                    else
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = "Input Value'" + PNValue + "' not match Regex'" + kp_rules.REGEX + "'!";
                                    }
                                }
                                else if (PNValue == row.PN_7XX)
                                {
                                    KP.MPN = PNValue;
                                    break;
                                }
                                //保康要求CTO的可以掃描客戶料號
                                else if (order_main.POTYPE == "CTO" && PNValue == row.CUST_PN)
                                {
                                    KP.MPN = row.PN_7XX;
                                    break;
                                }
                                else
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = "Scan Error";
                                }
                            }
                            else
                            {
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102505"));
                            }
                        }
                    }

                    KP.EXVALUE1 = "";
                    KP.EXKEY1 = "";
                    //REV
                    if (row.REV != "")
                    {
                        I.CBMessage = "";
                        I.Message = "REV";
                        while (true)
                        {
                            string REVValue = "";
                            if (dataCatch.VER != null && dataCatch.VER != "")
                            {
                                REVValue = dataCatch.VER;
                            }
                            else
                            {
                                makeOutput(I, row, KP, j);
                                REVValue = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();

                                #region Get Rev From Label with version, e.g 7XX-XXXXXX-XXR01,7XX-XXXXXXR01 
                                var reg1 = "^7[0-9]{2}-[0-9]{6}R([0-9]{2})$";
                                var m1 = Regex.Match(REVValue, reg1);
                                if (m1.Success)
                                {
                                    var value = m1.Groups[m1.Groups.Count - 1].Value;
                                    REVValue = value;
                                }
                                var reg2 = "^7[0-9]{2}-[0-9]{6}-[0-9]{2}R([0-9]{2})$";
                                var m2 = Regex.Match(REVValue, reg2);
                                if (m2.Success)
                                {
                                    var value = m2.Groups[m2.Groups.Count - 1].Value;
                                    REVValue = value;
                                }


                                //if (REVValue.Length == 13)
                                //{
                                //    if (REVValue.Substring(10, 1) != "-")
                                //    {
                                //        REVValue = REVValue.Substring(11, 2);
                                //    }
                                //}
                                //if (REVValue.Length == 16)
                                //{
                                //    REVValue = REVValue.Substring(14, 2);
                                //}
                                #endregion
                            }
                            //REVValue = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                            var kp_rules = db.ORM.Queryable<C_KP_Rule>().Where(t => t.PARTNO == row.PN_7XX && t.SCANTYPE == "AUTOKP_REV").First();
                            if (REVValue != "NO INPUT")
                            {
                                if (kp_rules != null)
                                {
                                    var m = Regex.Match(REVValue, kp_rules.REGEX);
                                    if (m.Success)
                                    {
                                        var value = m.Groups[m.Groups.Count - 1].Value;
                                        if (value != row.REV)
                                        {
                                            I.OutInputs.Clear();
                                            I.CBMessage = "Scan Error";
                                        }
                                        else
                                        {

                                            KP.EXVALUE1 = value;
                                            //KP.EXVALUE1 = REVValue;
                                            KP.EXKEY1 = "REV";
                                            dataCatch.VER = value;
                                            break;
                                        }

                                    }
                                    I.OutInputs.Clear();
                                    I.CBMessage = "Regex'" + kp_rules.REGEX + "' not match!";
                                }
                                else if (REVValue == row.REV)
                                {
                                    KP.EXVALUE1 = REVValue;
                                    KP.EXKEY1 = "REV";
                                    //dataCatch.VER = REVValue;
                                    break;
                                }
                                else
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = "Scan Error";
                                }
                            }
                            else
                            {
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102505"));
                            }
                        }
                    }

                    


                    I.CBMessage = "";
                    KP.LOCATION = "";
                    //COO
                    I.Message = "COO";
                    while (true)
                    {
                        string COOValue = "";
                        //if (dataCatch.COO != null && dataCatch.COO != "")
                        //{
                        //    COOValue = dataCatch.COO;
                        //}
                        //else
                        //{
                        //    COOValue = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();

                        //}
                        makeOutput(I, row, KP, j);
                        COOValue = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();
                        //var COOValue = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                        if (COOValue != "NO INPUT")
                        {
                            var COOValueList = db.ORM.Queryable<R_COO_MAP>().Where(t => t.CODE == COOValue).First();
                            if (COOValueList != null)
                            {
                                //verification COO with Matl_link Data;
                                var malt_coo = db.ORM.Queryable<R_SN_KP, R_SN_KP>((KP1, KP2) => KP1.SN == KP2.VALUE)
                                    .Where((KP1, KP2) => KP1.EXKEY1 == "COO" && KP2.KP_NAME == "LINK_TR_SN" && KP2.VALID_FLAG == 1 && KP2.SN == SN.SerialNo)
                                    .Select((KP1, KP2) => KP1.EXVALUE1)
                                    .First();
                                if (malt_coo != null && COOValue != malt_coo)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE_COO_NOT_MATCH", new string[] { malt_coo });
                                }
                                else
                                {
                                    //By PN 配置可用Coo 必須100%配置
                                    var coos = db.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "AutoKP" && t.CATEGORY == "COO"
                                  && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE == row.CUST_PN).Select(t => t.EXTVAL).ToList();
                                    //越南不上先
                                    if (Station.BU == "FJZ" && (coos.Count == 0 || coos.IndexOf(COOValue) < 0))
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE_COO_NOT_MATCH_CONFIG");
                                    }
                                    else
                                    {
                                        KP.LOCATION = COOValue;
                                        dataCatch.COO = COOValue;
                                        break;
                                    }

                                }
                            }
                            else
                            {
                                I.OutInputs.Clear();
                                I.CBMessage = "Scan Error";
                            }
                        }
                        else
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102505"));
                        }
                    }

                    KP.EDIT_TIME = db.ORM.GetDate();
                    db.ORM.Insertable<R_SN_KP>(KP).ExecuteCommand();
                    if (isScanTRSN)
                    {
                        I.Message = "TR_SN";
                        var apdb = Station.APDB;
                        while (true)
                        {
                            if (Station.BU == "FJZ")
                            {
                                makeOutput(I, row, KP, j);
                                var TRSN = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();
                                TRSN = TRSN.ToUpper();
                                string strSql = $@"select * from mes4.r_tr_sn where tr_sn='{TRSN}'";
                                var trsndata = apdb.RunSelect(strSql);
                                if (trsndata.Tables[0].Rows.Count == 0)
                                {
                                    //從本地系統中抓資料到Allparts
                                    var res = MESPubLab.Common.HttpHelp.HttpPost("http://10.14.129.147:8003/Allparts.asmx/CheckTrsnCMP", "p_container_id=" + TRSN, "application/x-www-form-urlencoded");
                                    if (!res.Contains("OK"))
                                    {
                                        //throw new Exception("[AllParts Fail]:Check TR_SN Fail" + res);
                                        I.OutInputs.Clear();
                                        I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104318") + res;
                                        continue;
                                    }
                                    trsndata = apdb.RunSelect(strSql);
                                }

                                if (trsndata.Tables[0].Rows[0]["CUST_KP_NO"].ToString() != row.PN_7XX)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104804", new string[] { row.PN_7XX });
                                    continue;
                                }

                                var mfr_code = trsndata.Tables[0].Rows[0]["MFR_CODE"].ToString();

                                strSql = $@"select mfr_name from mes1.c_mfr_config where mfr_code ='{mfr_code}'";
                                var mfr_name = apdb.RunSelect(strSql);
                                if (mfr_name.Tables[0].Rows.Count == 0)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104614", new string[] { TRSN });
                                    continue;
                                }

                                R_SN_KP item = new R_SN_KP()
                                {
                                    ID = MesDbBase.GetNewID(db.ORM, Station.BU, "R_SN_KP"),
                                    R_SN_ID = SN.ID,
                                    SN = SN.SerialNo,
                                    ITEMSEQ = itemseq,
                                    SCANSEQ = j + 1,
                                    DETAILSEQ = 2,
                                    STATION = Station.StationName,
                                    VALID_FLAG = 1,
                                    EDIT_EMP = Station.LoginUser.EMP_NO,
                                    KP_NAME = "AP_DATA",
                                    SCANTYPE = "TRSN",
                                    PARTNO = row.PN_7XX,
                                    VALUE = TRSN,
                                    EXKEY1 = "DATE_CODE",
                                    EXVALUE1 = trsndata.Tables[0].Rows[0]["DATE_CODE"].ToString(),
                                    EXKEY2 = "LOT_CODE",
                                    EXVALUE2 = trsndata.Tables[0].Rows[0]["LOT_CODE"].ToString(),
                                    REGEX = mfr_name.Tables[0].Rows[0][0].ToString(),
                                    MPN = trsndata.Tables[0].Rows[0]["MFR_KP_NO"].ToString(),
                                    EDIT_TIME = DateTime.Now
                                };
                                Station.SFCDB.ORM.Insertable(item).ExecuteCommand();

                            }
                            if (Station.BU == "VNJUNIPER")
                            {
                                makeOutput(I, row, KP, j);
                                var TRSN = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();
                                TRSN = TRSN.ToUpper();
                                string strSql = $@"select * from mes4.r_tr_sn where tr_sn='{TRSN}'";
                                var trsndata = apdb.RunSelect(strSql);
                                if (trsndata.Tables[0].Rows.Count == 0)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104318");
                                    continue;
                                }

                                if (trsndata.Tables[0].Rows[0]["CUST_KP_NO"].ToString() != row.PN_7XX)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = $@"Scan Error TR_SN PNO must'{row.PN_7XX}'";
                                    continue;
                                }

                                var mfr_code = trsndata.Tables[0].Rows[0]["MFR_CODE"].ToString();

                                strSql = $@"select mfr_name from mes1.c_mfr_config where mfr_code ='{mfr_code}'";
                                var mfr_name = apdb.RunSelect(strSql);
                                if (mfr_name.Tables[0].Rows.Count == 0)
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104614", new string[] { TRSN });
                                    continue;
                                }

                                R_SN_KP item = new R_SN_KP()
                                {
                                    ID = MesDbBase.GetNewID(db.ORM, Station.BU, "R_SN_KP"),
                                    R_SN_ID = SN.ID,
                                    SN = SN.SerialNo,
                                    ITEMSEQ = itemseq,
                                    SCANSEQ = j + 1,
                                    DETAILSEQ = 2,
                                    STATION = Station.StationName,
                                    VALID_FLAG = 1,
                                    EDIT_EMP = Station.LoginUser.EMP_NO,
                                    KP_NAME = "AP_DATA",
                                    SCANTYPE = "TRSN",
                                    PARTNO = row.PN_7XX,
                                    VALUE = TRSN,
                                    EXKEY1 = "DATE_CODE",
                                    EXVALUE1 = trsndata.Tables[0].Rows[0]["DATE_CODE"].ToString(),
                                    EXKEY2 = "LOT_CODE",
                                    EXVALUE2 = trsndata.Tables[0].Rows[0]["LOT_CODE"].ToString(),
                                    REGEX = mfr_name.Tables[0].Rows[0][0].ToString(),
                                    MPN = trsndata.Tables[0].Rows[0]["MFR_KP_NO"].ToString(),
                                    EDIT_TIME = DateTime.Now
                                };
                                Station.SFCDB.ORM.Insertable(item).ExecuteCommand();

                            }
                        }


                        isScanTRSN = false;
                    }
                    //CHAS_SN
                    if (row.CHAS_SN == row.PN)
                    {
                        I.Message = "System Serial Number";
                        while (true)
                        {
                            var Chas_SN = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();
                            if (Chas_SN != "NO INPUT")
                            {
                                if (Chas_SN.Length > 0)
                                {
                                    var ScanChasSnMsg = "";
                                    if (row.TYPE == "SAP_HB" && Chas_SN != KP.SN)
                                    {
                                        ScanChasSnMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105119");
                                    }
                                    else if (row.TYPE == "I137" && Chas_SN == KP.SN && order_main.OFFERINGTYPE == "FRU")
                                    {
                                        ScanChasSnMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105529");
                                    }
                                    else if (row.TYPE != "I137" && row.TYPE != "SAP_HB")
                                    {
                                        if (Chas_SN.Length == 12)
                                        {
                                            if (!Chas_SN.Substring(0, 2).Contains("JN") || !Regex.IsMatch(Chas_SN.Substring(2, 6), @"^[0-9a-zA-Z]+$") || !Regex.IsMatch(Chas_SN.Substring(9, 3), @"^[a-zA-Z]+$") || !Regex.IsMatch(Chas_SN.Substring(8, 1), @"^[0-9a-zA-Z]+$"))
                                            {
                                                ScanChasSnMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105539");
                                            }
                                        }
                                        else
                                        {
                                            if (Station.BU == "VNJUNIPER")
                                            {
                                                //VN存在row.TYPE=PNO類型掃描SN長度為5的情況(MX104系列：EBAA****重Loading時掃描SystemNo：RA***) Asked By PE 譚義康 2021-11-11
                                                if (Chas_SN.Length == 5)
                                                {
                                                    if (!Regex.IsMatch(Chas_SN.Substring(0, 2), @"^[a-zA-Z]+$") || !Regex.IsMatch(Chas_SN.Substring(2, 3), @"^[0-9]+$"))
                                                    {
                                                        ScanChasSnMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20211111165830");
                                                    }
                                                }
                                                else
                                                {
                                                    ScanChasSnMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20211111165830");
                                                }
                                            }
                                            else
                                            {
                                                ScanChasSnMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105539");
                                            }
                                        }
                                    }
                                    if (ScanChasSnMsg == "")
                                    {
                                        KP.ID = MesDbBase.GetNewID(db.ORM, Station.BU, "R_SN_KP");
                                        KP.ITEMSEQ = itemseq++;
                                        KP.VALUE = Chas_SN;
                                        KP.SCANTYPE = "SysSerOfChassis";
                                        KP.EDIT_TIME = db.ORM.GetDate();
                                        db.ORM.Insertable<R_SN_KP>(KP).ExecuteCommand();
                                        break;
                                    }
                                    else
                                    {
                                        I.OutInputs.Clear();
                                        I.CBMessage = "Scan Error:" + ScanChasSnMsg;
                                    }
                                }
                                else
                                {
                                    I.OutInputs.Clear();
                                    I.CBMessage = "Scan Error";
                                }
                            }
                            else
                            {
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102505"));
                            }
                        }
                    }

                }
            }
        }

        public static bool ckeckAutoSNRule(string SN, string Rule, MESDBHelper.OleExec db)
        {
            //Rule = "MX240BASEX|'JN'@@@@@@@'AFC'MM'A23'@@YYYY'BBB'@";
            //SN = "JN1234567AFC12A23132020BBBC";
            //SN不能包含特殊字符
            if (SN.Length == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105608"));
            }
            for (int i = 0; i < SN.Length; i++)
            {
                string s = SN.Substring(i, 1);
                if (strContains.Contains(s)) throw new Exception($@"SN can't Contains({s})");
            }

            //snRule取最後那串，沒有分隔符報錯
            var snRuleList = Rule.Split('|');
            if (snRuleList.Count() < 2)
            {
                throw new Exception($@"SN mask not Contains(|)");
            }
            var snRule = snRuleList[snRuleList.Count() - 1];

            if (SN.Length != snRule.Replace("'", "").Length)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105838"));
            }

            //檢查規則是否匹配,不匹配返回false，是不是返回提示信息好點？？
            try
            {
                if (!RegexPatt(SN, snRule, db)) return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        public static bool RegexPatt(string SN, string snRule, MESDBHelper.OleExec db)
        {
            var patt = "";
            var yyyy = "";
            var mm = "";
            int m = 0;
            int y = 0;
            var rule = GetRule(snRule);
            foreach (var r in rule)
            {
                if (r.RuleName == "'")
                {
                    patt = patt + r.SN;
                }
                else if (r.RuleName == "#")
                {
                    patt = patt + "[0-9]{1}";
                }
                else if (r.RuleName == "*")
                {
                    patt = patt + "[a-z,A-Z]{1}";
                }
                else if (r.RuleName == "@")
                {
                    patt = patt + "[0-9,a-z,A-Z]{1}";
                }
                else if (r.RuleName == "!")
                {
                    patt = patt + "[0-9]{1}";
                }
                else if (r.RuleName == "MM")
                {
                    mm = mm + SN.Substring(r.from, r.len);
                    m = m + 1;
                    if (m > 1)
                    {
                        patt = patt + "[0-2]{1}";
                    }
                    else
                    {
                        patt = patt + "[0-9]{1}";
                    }
                }
                else if (r.RuleName == "YYYY")
                {
                    yyyy = yyyy + SN.Substring(r.from, r.len);
                    y = y + 1;
                    if (y == 1)
                    {
                        patt = patt + "[2]{1}";
                    }
                    else if (y == 2)
                    {
                        patt = patt + "[0]{1}";
                    }
                    else
                    {
                        patt = patt + "[0-9]{1}";
                    }
                }
            }

            //不匹配返回false
            if (!Regex.IsMatch(SN, "^" + patt + "$"))
            {
                throw new Exception($@"Invalid SN format in the SN:{SN},Rule:{snRule}");
            }

            //如何是YYYY需為四位
            if (yyyy.Length > 0)
            {
                if (yyyy.Length != 4)
                {
                    throw new Exception($@"YYYY not match yyyy:{yyyy}");
                }
            }

            //如何是MM需為兩位
            if (mm.Length > 0)
            {
                if (mm.Length != 2)
                {
                    throw new Exception($@"MM not match mm:{mm}");
                }
            }

            //YYYYMM 或 YYYY不能超前 ，格式需為時間YYYYMM格式
            var strSql = "";
            if (yyyy.Length > 0 && mm.Length > 0)
            {
                strSql = $@"select (to_char(sysdate,'YYYYMM') - to_char(to_date('{yyyy + mm}','YYYYMM'),'YYYYMM')) as yyyymm from dual";

            }
            else
            {
                if (yyyy.Length > 0)
                {
                    strSql = $@"select (to_char(sysdate,'YYYY') - to_char(to_date('{yyyy}','YYYY'),'YYYY')) as yyyymm from dual";
                }
                else if (yyyy.Length > 0)
                {
                    strSql = $@"select (12 - to_char(to_date('{mm}','MM'),'MM'))  as yyyymm from dual";
                }
            }
            try
            {
                if (strSql != "")
                {
                    if (int.Parse(db.RunSelect(strSql).Tables[0].Rows[0]["yyyymm"].ToString()) < 0) throw new Exception($@"YYYMM not match yyyymm:{yyyy + mm}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        public static List<SnC> GetRule(string snRule)
        {
            //取得規則LIST
            List<SnC> snCList = new List<SnC>();
            int j = 0;
            int k = 0;
            string temp = "";
            string ruleType = "";
            bool isActualAlphabetic = false;
            bool actualAlphabeticEnd = false;
            for (int i = 0; i < snRule.Length; i++)
            {
                if (snRule.Substring(i, 1) != "'")
                {
                    j = j + 1;
                }

                if (snRule.Substring(i, 1) == "'" && !isActualAlphabetic)
                {
                    isActualAlphabetic = true;
                    ruleType = "\'";
                }
                else if (snRule.Substring(i, 1) == "'" && isActualAlphabetic)
                {
                    isActualAlphabetic = false;
                    actualAlphabeticEnd = true;
                    ruleType = "\'";
                }
                else if (snRule.Substring(i, 1) == "#")
                {
                    ruleType = "#";
                }
                else if (snRule.Substring(i, 1) == "*")
                {
                    ruleType = "*";
                }
                else if (snRule.Substring(i, 1) == "@")
                {
                    ruleType = "@";
                }
                else if (snRule.Substring(i, 1) == "!")
                {
                    ruleType = "!";
                }
                else if (snRule.Substring(i, 1).ToUpper() == "M" && !isActualAlphabetic)
                {
                    ruleType = "MM";
                }
                else if (snRule.Substring(i, 1).ToUpper() == "Y" && !isActualAlphabetic)
                {
                    ruleType = "YYYY";
                }

                if (isActualAlphabetic)
                {
                    temp = temp + snRule.Substring(i, 1).Replace("\'", "");
                    if (snRule.Substring(i, 1) != "'")
                    {
                        k = k + 1;
                    }
                }
                else
                {
                    if (!actualAlphabeticEnd)
                    {
                        temp = snRule.Substring(i, 1);
                        k = 1;
                    }
                }

                if (!isActualAlphabetic)
                {
                    SnC c = new SnC()
                    {
                        RuleName = ruleType,
                        SN = temp,
                        from = ruleType == "\'" ? j - k : j - 1,
                        len = k
                    };
                    temp = "";
                    k = 0;
                    actualAlphabeticEnd = false;
                    snCList.Add(c);
                }
            }

            return snCList;
        }

        public static void JuniperAutoKPRuleCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100146"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100639"));
            }

            var WO = (WorkOrder)WOSession.Value;
            var SN = SNSession.Value.ToString();

            var db = Station.SFCDB;

            var order_main = db.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == WO.WorkorderNo).First();
            if (order_main == null)
            {
                //throw new Exception("WO not exists in O_ORDER_MAIN, please check!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110215"));
            }

            var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO.WorkorderNo, "JuniperAutoKPConfig", db);
            if (kpl.Count == 0)
            {
                //throw new Exception("Can't find AutoKP, Please check!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100753"));
            }

            //for (int j = 0; j < kpl[i].QTY; j++)

            if (kpl.Count > 0)
            {
                if (kpl[0].SN_RULE == "")
                {
                    SN SNObj = new SN();
                    var csku = db.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == WO.SkuNO).First();

                    try
                    {
                        if (SN.Length == 0)
                        {
                            //throw new Exception("SN can't Null, please check!");
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105608"));
                        }
                        for (int i = 0; i < SN.Length; i++)
                        {
                            string s = SN.Substring(i, 1);
                            if (strContains.Contains(s)) throw new Exception("SN can't Contains (" + s + "), please check!");
                        }

                        var CheckRuleFlag = SNObj.CheckSNRule(SN, csku.SN_RULE, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        if (!CheckRuleFlag)
                        {
                            //throw new Exception("SN rule not match, please check!");
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103835"));
                        }
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception("SN rule not match:" + ex.Message);
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104221") + ex.Message);
                    }
                }
                else if (!ckeckAutoSNRule(SN, kpl[0].SN_RULE, db))
                {
                    //throw new Exception("SN rule not match, please check!");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103835"));
                }
            }
        }

        public static void JuniperMATL_LINKAutoKPRuleCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                //throw new Exception("WO Object not exists, please check!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100146"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                //throw new Exception("SN Object not exists, please check!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100639"));
            }

            var WO = (WorkOrder)WOSession.Value;
            var SN = SNSession.Value.ToString();

            var db = Station.SFCDB;

            var order_main = db.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == WO.WorkorderNo).First();
            if (order_main == null)
            {
                //throw new Exception("WO not exists in O_ORDER_MAIN, please check!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814110215"));
            }

            var kpl = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(order_main.PREWO, "JuniperAutoKPConfig", db);
            if (kpl.Count == 0)
            {
                //throw new Exception("Can't find AutoKP, Please check!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814100753"));
            }

            if (order_main.OFFERINGTYPE != "Advanced Fixed System")
            {
                if (kpl.Count > 0)
                {
                    if (kpl[0].SN_RULE == "")
                    {
                        SN SNObj = new SN();
                        var csku = db.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == WO.SkuNO).First();

                        try
                        {
                            if (SN.Length == 0)
                            {
                                //throw new Exception("SN can't Null, please check!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105608"));
                            }
                            for (int i = 0; i < SN.Length; i++)
                            {
                                string s = SN.Substring(i, 1);
                                if (strContains.Contains(s)) throw new Exception("SN can't Contains (" + s + "), please check!");
                            }

                            var CheckRuleFlag = SNObj.CheckSNRule(SN, csku.SN_RULE, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            if (!CheckRuleFlag)
                            {
                                //throw new Exception("SN rule not match, please check!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103835"));
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("SN rule not match:" + ex.Message);
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104221") + ex.Message);
                        }
                    }
                    else if (!ckeckAutoSNRule(SN, kpl[0].SN_RULE, db))
                    {
                        //throw new Exception("SN rule not match, please check!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103835"));
                    }
                }
            }
            else
            {
                if (kpl.Count > 1)
                {
                    if (kpl[1].SN_RULE == "")
                    {
                        SN SNObj = new SN();
                        var csku = db.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == kpl[1].PN).First();

                        try
                        {
                            if (SN.Length == 0)
                            {
                                //throw new Exception("SN can't Null, please check!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105608"));
                            }
                            for (int i = 0; i < SN.Length; i++)
                            {
                                string s = SN.Substring(i, 1);
                                if (strContains.Contains(s)) throw new Exception("SN can't Contains (" + s + "), please check!");
                            }

                            var CheckRuleFlag = SNObj.CheckSNRule(SN, csku.SN_RULE, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                            if (!CheckRuleFlag)
                            {
                                //throw new Exception("SN rule not match, please check!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103835"));
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception("SN rule not match:" + ex.Message);
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814104221") + ex.Message);
                        }
                    }
                    else if (!ckeckAutoSNRule(SN, kpl[1].SN_RULE, db))
                    {
                        //throw new Exception("SN rule not match, please check!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103835"));
                    }
                }
            }



        }

    }

    class AutoKpCatch
    {
        public string COO;
        public string VER;
        public string PN;
        public string CLEI;
        public string PN_7XX;
    }

}
