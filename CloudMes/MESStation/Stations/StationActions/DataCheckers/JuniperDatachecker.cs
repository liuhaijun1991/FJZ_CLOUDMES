using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class JuniperDatachecker
    {
        public static void NotDOFCTOType_Checker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var strWO = WOSession.Value.ToString();
            var SFCDB = Station.SFCDB;

            var wo = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == strWO).First();
            if (wo != null)
            {
                throw new Exception("This station not loading DOF/CTO/BTS workorder.Pls change station.");
            }

        }

        public static void SAPEX_PN_Checker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var SFCDB = Station.SFCDB;
            //取得不再需要掃描的料號前綴
            var NoCheck = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "SAPEX_CHECK" && t.CATEGORY == "NO_CHECK" && t.FUNCTIONTYPE == "NOSYSTEM" && t.CONTROLFLAG == "Y")
                .Select(t => t.VALUE).ToList();

            var strWo = WOSession.Value.ToString();
            var sn = (SN)SNSession.Value;

            //取得待掃描列表
            var PartNOs = SFCDB.ORM.Queryable<R_PRE_WO_DETAIL>().Where(t => t.WO == strWo && t.PARTNOTYPE == "SAPEX").ToList();
            if (PartNOs.Count == 0)
            {
                return;
            }

            int itemseq = 700;
            SFCDB.ORM.Deleteable<R_SN_KP>().Where(t => t.R_SN_ID == sn.ID && t.VALID_FLAG == 1 && t.SCANTYPE == "SAPEX_PN").ExecuteCommand();
            for (int i = 0; i < PartNOs.Count; i++)
            {
                bool nocheck = false;
                for (int j = 0; j < NoCheck.Count; j++)
                {
                    if (PartNOs[i].PARTNO.StartsWith(NoCheck[j]))
                    {
                        nocheck = true;
                        break;
                    }
                }
                if (nocheck)
                { continue; }

                UIInputData I = new UIInputData()
                {
                    MustConfirm = false,
                    Timeout = 3000000,
                    IconType = IconType.None,
                    UIArea = new string[] { "50%", "50%" },
                    Message = "PN",
                    Tittle = "SAPEX_PN_CHECK",
                    Type = UIInputType.String,
                    //Name = row.CUST_PN + "_" + row.PN_7XX,
                    ErrMessage = "No input",
                    CBMessage = ""
                };
                
                for (int j = 0; j < float.Parse(PartNOs[i].REQUESTQTY); j++)
                {
                    I.CBMessage = "";
                    R_SN_KP KP = new R_SN_KP()
                    {
                        ID = MesDbBase.GetNewID(SFCDB.ORM, Station.BU, "R_SN_KP"),
                        R_SN_ID = sn.ID,
                        SN = sn.SerialNo,
                        ITEMSEQ = itemseq++,
                        SCANSEQ = j + 1,
                        DETAILSEQ = 1,
                        STATION = Station.StationName,
                        VALID_FLAG = 1,
                        EDIT_EMP = Station.LoginUser.EMP_NO,
                        KP_NAME = "SAPEX_PN",
                        SCANTYPE = "SAPEX_PN",
                        EDIT_TIME = DateTime.Now

                    };

                    var kp_rules = SFCDB.ORM.Queryable<C_KP_Rule>().Where(t => t.PARTNO == PartNOs[i].PARTNO && t.SCANTYPE == "AUTOKP_PN").First();
                    //I.Name = PartNOs[i].PARTNO+$@"（{j+1}of{PartNOs[i].REQUESTQTY.Replace(".",",")}）";
                    I.placeholder = PartNOs[i].PARTNO + $@"({j + 1}/{PartNOs[i].REQUESTQTY})";
                    //I.Name = PartNOs[i].PARTNO;
                    //輸入數據並記錄
                    while (true)
                    {
                        var pn = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();
                        var pn1 = pn;
                        if (kp_rules != null)
                        {
                            var m = Regex.Match(pn, kp_rules.REGEX);
                            if (m.Success)
                            {
                                var value = m.Groups[m.Groups.Count - 1].Value;
                                pn =        m.Groups[m.Groups.Count - 1].Value;
                                
                            }
                            
                        }

                        if (pn == "NO INPUT")
                        {
                            throw new Exception("User Cencel");
                        }
                        else if (pn != PartNOs[i].PARTNO)
                        {
                            I.CBMessage = "PN Scan Error!";
                        }
                        else
                        {
                            KP.VALUE = pn1;
                            KP.PARTNO = pn;
                            SFCDB.ORM.Insertable(KP).ExecuteCommand();
                            break;
                        }
                    }


                }
                

            }



        }
        public static void JIADataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var WO = (WorkOrder)WOSession.Value;
            string strWO = WO.WorkorderNo;
            string SN = null;
            string IsJIA = "NO";

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == "SN");
         
            var db = Station.SFCDB;

            var res = db.ORM.Queryable<O_ORDER_MAIN, O_I137_ITEM, O_I137_HEAD>((m, i, h) => m.ITEMID == i.ID && i.TRANID == h.TRANID).Where
                ((m, i, h) => m.PREWO == strWO).Select((m, i, h) => new { h.SHIPTOCOMPANY, h.SHIPTOCOUNTRYCODE, h.SOLDTOCOMPANY, m.POTYPE }).First();

            if (res == null)
            {
                throw new Exception($@"Can't get SHIPTOCOMPANY and SHIPTOCOUNTRYCODE by WO:{strWO}");
            }

            if (res.POTYPE.Trim().ToUpper() == "BTS")
                return;
                      
            var custs = db.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "JUNIPER_JIA_CUST" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE == "NOSYSTEM")
                .Select(t => t.VALUE).ToList();

            string _shiptocompany = res.SHIPTOCOMPANY.Trim().ToUpper();
            string _shiptocountrycode = res.SHIPTOCOUNTRYCODE.Trim().ToUpper();
            string _soldtocompany = res.SOLDTOCOMPANY.Trim().ToUpper();  //VNQE BiBi asked: Customer request SoldToCompany also need to check. 2021-12-20            

            foreach (var item in custs)
            {
                if (_shiptocompany.Contains(item.Trim().ToUpper()))
                {
                    IsJIA = "YES";
                    break;
                }
                if (_soldtocompany.Contains(item.Trim().ToUpper()) && Station.BU == "VNJUNIPER")
                {
                    IsJIA = "YES";
                    break;
                }
            }

            if (_shiptocountrycode == "JP")
            {
                IsJIA = "YES";
            }

            if (IsJIA == "YES")
            {
                if (Station.BU == "VNJUNIPER")
                {
                    //VN check whether sn has uploaded jia inspection result. 2021-12-20
                    var strSN = Input.Value.ToString();
                    var snJIALog = db.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == strSN && t.SKUNO == WO.SkuNO && t.WORKORDERNO == strWO && t.STATION_NAME == "JIA_INSPECTION").ToList();
                    if (snJIALog.Count == 0)
                    {
                        throw new Exception($@"This unit is needed JIA!");
                    }
                }
                else
                {
                    #region 原邏輯：彈框提示
                    UIInputData I = new UIInputData()
                    {
                        MustConfirm = false,
                        Timeout = 3000000,
                        IconType = IconType.Message,
                        UIArea = new string[] { "60%", "70%" },
                        //Message = "Do you do the JIA Check ?",
                        Tittle = "<span style=\"font-size:25px;\">This unit is JIA!</span> ",
                        Type = UIInputType.YesNo,
                        //Name = "test jupe",
                        ErrMessage = "No input"
                        //CBMessage = "OKOK"

                    };

                    I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "WO", Value = strWO });

                    if (SNSession != null && !String.IsNullOrEmpty(SNSession.InputValue))
                    {
                        SN = SNSession.InputValue;

                        I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "SN", Value = SN });
                    }

                    I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "SKU", Value = WO.SkuNO });
                    I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "SHIPTOCOMPANY", Value = res.SHIPTOCOMPANY });
                    I.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.Text.ToString(), Name = "SHIPTOCOUNTRYCODE", Value = res.SHIPTOCOUNTRYCODE });

                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString().ToUpper().Trim();

                    if (ret == "YES")
                    {
                        return;
                    }
                    else
                    {
                        throw new Exception($@"Pls scan again the unit!");
                    }
                    #endregion
                }
            }
        }

        public static void SN_Matl_LinkChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }



            var sfcdb = Station.SFCDB;
            //如果工单是 OFFERINGTYPE ==  Advanced Fixed System && Configurable System 不检查这个
            var strWo = WOSession.Value.ToString();
            var order_main = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == strWo).ToList();

            if (order_main.Count==0)
            {
                return;
            }
            else
            {
                if (order_main[0].OFFERINGTYPE == "Advanced Fixed System"|| order_main[0].OFFERINGTYPE == "Configurable System")
                {
                    return;
                }
            }

            var strSN = SNSession.Value.ToString();

            
            //if (sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == strSN).Any())
            //{
            //    return;
            //}

            // FVN 只卡007G 開頭的工單
            if (Station.BU.ToString().ToUpper() == "VNJUNIPER")
            {
                //if (!strWo.StartsWith("007G"))
                //{
                //    return;
                //}
                //突然別的系列也有007G的工單了, 還是按配置的系列名來卡吧
                var fcontrol = sfcdb.ORM.Queryable<R_F_CONTROL, C_SERIES, C_SKU>((a, b, c) => a.VALUE == b.SERIES_NAME && b.ID == c.C_SERIES_ID)
                    .Where((a, b, c) => c.SKUNO == order_main[0].PID && a.FUNCTIONNAME == "CHECK_NEED_SCAN_MATL_LINK" && a.CONTROLFLAG == "Y" && a.FUNCTIONTYPE == "NOSYSTEM")
                    .Select((a, b, c) => a).ToList();
                if (fcontrol.Count == 0)
                {
                    return;//沒配置就退出檢查吧
                }
            }


            var kp = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strSN && t.KP_NAME == "LINK_TR_SN" && t.SCANTYPE == "TR_SN" && t.VALID_FLAG == 1).First();
            if (kp == null)
            {
                throw new Exception($@"{strSN} don't scan MATL_LINK.");
            }

        }

        public static void JuniperSODatachecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            var ErrMessage = "";
            var SFCDB = Station.SFCDB;
            MESStationSession PLSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PLSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var PLSN = PLSession.Value.ToString();

            var plid = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PLSN).Select(t => t.ID).First();
            var wolist = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((p, sp, s) => p.ID == sp.PACK_ID && sp.SN_ID == s.ID)
                .Where((p, sp, s) => p.PARENT_PACK_ID == plid).Select((p, sp, s) => s.WORKORDERNO).Distinct().ToList();
            O_ORDER_MAIN juniperwo = null;
            for (int i = 0; i < wolist.Count; i++)
            {
                var tt = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == wolist[i]).First();
                if (tt != null)
                {
                    juniperwo = tt;
                    break;
                }
            }
            //不是Juniper的DF不检查这个
            if (juniperwo == null)
            {
                return;
            }
            var ITEM_ID = juniperwo.ITEMID;


            I137_I i137 = SFCDB.ORM.Queryable<I137_I>().Where(i => i.ID == ITEM_ID).ToList().FirstOrDefault();
            if (i137 == null)
            {
                throw new Exception($@"{ITEM_ID} ITEM_ID i137 Data");
            }
            I137_H i137head = SFCDB.ORM.Queryable<I137_H>().Where(r => r.TRANID == i137.TRANID).ToList().FirstOrDefault();
            if (i137head == null)
            {
                throw new Exception("I137 Head Error!");
            }
            var SO = $@"1K{ i137head.SALESORDERNUMBER.TrimStart('0')}.{i137.SALESORDERLINEITEM.TrimStart('0')}";

            UIInputData I = new UIInputData()
            {
                MustConfirm = false,
                Timeout = 3000000,
                IconType = IconType.Message,
                UIArea = new string[] { "60%", "70%" },
                Tittle = "<span style=\"font-size:25px;\">SO Check</ span > ",
                Type = UIInputType.String,
                ErrMessage = "No input"

            };

            var ret2 = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();

            if (ret2 != SO)
            {
                throw new Exception($@"Scan SO '{ret2}' not match PL SO '{SO}'");
            }

        }
        public static void JuniperPLQTYDatachecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            var ErrMessage = "";
            var SFCDB = Station.SFCDB;
            MESStationSession PLSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PLSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var PLSN = PLSession.Value.ToString();

            var plid = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PLSN).Select(t => t.ID).First();
            var wolist = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((p, sp, s) => p.ID == sp.PACK_ID && sp.SN_ID == s.ID)
                .Where((p, sp, s) => p.PARENT_PACK_ID == plid).Select((p, sp, s) => s.WORKORDERNO).Distinct().ToList();
            O_ORDER_MAIN juniperwo = null;
            for (int i = 0; i < wolist.Count; i++)
            {
                var tt = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == wolist[i]).First();
                if (tt != null)
                {
                    juniperwo = tt;
                    break;
                }
            }
            //不是Juniper的DF不检查这个
            if (juniperwo == null)
            {
                return;
            }

            var qty = SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((p, sp, s) => p.ID == sp.PACK_ID && sp.SN_ID == s.ID)
                .Where((p, sp, s) => p.PARENT_PACK_ID == plid).Select((p, sp, s) => s.SN).Count();

            UIInputData I = new UIInputData()
            {
                MustConfirm = false,
                Timeout = 3000000,
                IconType = IconType.Message,
                UIArea = new string[] { "60%", "70%" },
                Tittle = "<span style=\"font-size:25px;\">QTY Check</ span > ",
                Type = UIInputType.String,
                ErrMessage = "No input"

            };

            var ret2 = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();

            if (ret2 != qty.ToString())
            {
                throw new Exception($@"Scan QTY '{ret2}' not match PL QTY '{qty}'");
            }
        }

        public static void TR_SNDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }


            var sfcdb = Station.SFCDB;
            //如果工单是 OFFERINGTYPE ==  Advanced Fixed System 不检查这个
            var strWo = WOSession.Value.ToString();
            var order_main = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == strWo).ToList();

            if (order_main.Count == 0)
            {
                return;
            }
            else
            {
                if (order_main[0].OFFERINGTYPE == "Advanced Fixed System")
                {
                    return;
                }
            }


            var strSN = SNSession.Value.ToString();

            var kp = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.SN == strSN && t.KP_NAME == "LINK_TR_SN" && t.SCANTYPE == "TR_SN" && t.VALID_FLAG == 1).First();
            if (kp == null)
            {
                throw new Exception($@"{strSN} don't scan MATL_LINK.");
            }

        }

        public static void CheckBINFamilyProgramming(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            var sfcdb = Station.SFCDB;
            try { 
                string ErrMessage;
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }

                var strSN = SNSession.Value.ToString();
                string strSKU = sfcdb.ORM.Queryable<R_SN>().Where(s => s.SN == strSN && s.VALID_FLAG == "1").Select(s => s.SKUNO).First();

                string strFam = null;

                if (strSKU == "750-101855" || strSKU == "750-101856" || strSKU == "750-087559")
                {
                    switch (strSKU)
                    {
                        case "750-101855":
                            strFam = "750-rodnik";
                            break;
                        case "750-101856":
                            strFam = "750-aussrk";
                            break;
                        case "750-087559":
                            strFam = "750-atlass";
                            break;
                    }
                    var objSNDet = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>()
                        .Where(s => s.STATION_NAME == "MA1" && s.SN == strSN && s.SKUNO == strSKU)
                        .OrderBy(s => s.EDIT_TIME, SqlSugar.OrderByType.Desc).First();

                    if(objSNDet == null)
                    {
                        throw new Exception($@"{strSN} needs to go first to MA1.");
                    }

                    DateTime dtMA1 = objSNDet.EDIT_TIME.Value;

                    bool existsV1 = sfcdb.ORM.Queryable<R_TEST_JUNIPER>()
                        .Where(t => t.SYSSERIALNO == strSN && t.EVENTNAME == "V1" && t.STATUS == "PASS" && t.PART_NUMBER == strFam && t.TESTDATE >= dtMA1).Any();

                    if (!existsV1) 
                    {
                        sfcdb.BeginTrain();
                        string empNO = Station.LoginUser.EMP_NO;
                        DateTime getDate = sfcdb.ORM.GetDate();

                        var updateSN = sfcdb.ORM.Queryable<R_SN>().Where(r => r.SN == strSN && r.SKUNO == strSKU && r.VALID_FLAG == "1").First();
                        updateSN.CURRENT_STATION = "MA1";
                        updateSN.NEXT_STATION = "SI_V1";
                        updateSN.EDIT_EMP = empNO;
                        updateSN.EDIT_TIME = getDate;

                        string newID = MesDbBase.GetNewID(sfcdb.ORM, Station.BU, "R_SN_STATION_DETAIL");

                        var newSNDet = new R_SN_STATION_DETAIL()
                        {
                            ID = newID,
                            R_SN_ID = objSNDet.R_SN_ID,
                            SN = strSN,
                            SKUNO = strSKU,
                            WORKORDERNO = objSNDet.WORKORDERNO,
                            PLANT = objSNDet.PLANT,
                            CLASS_NAME = objSNDet.CLASS_NAME,
                            ROUTE_ID = objSNDet.ROUTE_ID,
                            LINE = objSNDet.LINE,
                            STARTED_FLAG = "1",
                            START_TIME = objSNDet.START_TIME,
                            PACKED_FLAG = "0",
                            COMPLETED_FLAG = "0",
                            SHIPPED_FLAG = "0",
                            REPAIR_FAILED_FLAG = "0",
                            CURRENT_STATION = "MA1",
                            NEXT_STATION = "SI_V1",
                            KP_LIST_ID = objSNDet.KP_LIST_ID,
                            CUST_PN = objSNDet.CUST_PN,
                            DEVICE_NAME = "SI_V2-->SI_V1",
                            STATION_NAME = "RETURN",
                            PRODUCT_STATUS = objSNDet.PRODUCT_STATUS,
                            VALID_FLAG = "1",
                            EDIT_EMP = empNO,
                            EDIT_TIME = getDate
                        };

                        sfcdb.ORM.Updateable(updateSN).ExecuteCommand();
                        var insert_sn_det = sfcdb.ORM.Insertable(newSNDet).ExecuteCommand();

                        sfcdb.CommitTrain();

                        Station.AddMessage("MSGCODE2021BINFAM00001", new string[] { strSN, strFam }, StationMessageState.Fail, StationMessageDisplayType.Swal);
                        throw new Exception($@"{strSN} was not programmed in V1 as {strFam}. This unit will be send back to MA1. Please program it again.");
                    }
                }
            }
            catch (Exception ex)
            {
                
                if(ex.Message.Contains("was not programmed in V1 as"))
                {
                    throw new MESReturnMessage(ex.Message);
                }
                else
                {
                    sfcdb.RollbackTrain();
                    throw new MESReturnMessage("CheckBINFamilyProgramming: " + ex.Message);
                }
                
            }
        }


        public static void SMCheckLocation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LocationSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if(LocationSession.Value.ToString() == "SELECT LOCATION TO")
            {
                throw new Exception($@"Please choose a valid Location");
            }
        }

        public static void PackOutCheckOpticPackType(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession TransprotSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TransprotSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession PacktypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (PacktypeSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            var strPT = TransprotSession.Value.ToString();
            string PackPartNO = "";
            if (strPT.IndexOf(':') > 0)
            {
                var strR = $@"^(\S+):\((\d+)\)$";
                var R = Regex.Match(strPT, strR);
                if (R.Success == true)
                {
                    PackPartNO = R.Groups[1].Value;
                    
                }
                else
                {
                    throw new Exception($@"ERR:{strPT}");
                }
            }
            else
            {
                return;
            }
            var strSKU = SkuSession.Value.ToString();
            //
            var packConfig = Station.SFCDB.ORM.Queryable<O_SKU_PACKAGE>().Where(t => t.SKUNO == strSKU && t.PARTNO == PackPartNO).First();
            var packtype = PacktypeSession.Value.ToString();
            if (packtype == "Single pack" && packConfig.SCENARIO != "CartonOverpack")
            {
                // "en": "Wo PackType is '{0}'.Can not use '{1}' type carton",
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE2021NOTUSEPACKTYPE", new string[] { packtype, packConfig.SCENARIO });
                throw new MESReturnMessage(ErrMessage);
            }
            else if (packtype == "Bulk pack" && packConfig.SCENARIO != "Multipack")
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE2021NOTUSEPACKTYPE", new string[] { packtype, packConfig.SCENARIO });
                throw new MESReturnMessage(ErrMessage);
            }




            //if (string.IsNullOrEmpty(o137.CARTONLABEL2))
            //{
            //    PacktypeSession.Value = "Single pack";
            //}
            //else
            //{
            //    PacktypeSession.Value = "Bulk pack";
            //}

        }

        public static void ReverseCheckerByType(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;

            MESStationSession Type = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Type == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession Input1 = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Input == null) 
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            string strType = Type.Value.ToString();
            string strInput = Input1.Value.ToString();

            if(strType.StartsWith("PLEASE"))
            {
                throw new Exception($@" So funny. Please select a correct type.");
            }

            if (strType == "WO")
            {
                if (!Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(wo => strInput == wo.WORKORDERNO).Any())
                { 
                    throw new Exception($@" WO {strInput} does not exist. Please check.");
                }
                if(Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(wo => strInput == wo.WORKORDERNO && wo.CLOSED_FLAG == "1").Any())
                {
                    throw new Exception($@" WO {strInput} is already closed. Please check.");
                }
            }

            if (strType == "SN")
            {
                if (!Station.SFCDB.ORM.Queryable<R_SN>().Where(sn => strInput == sn.SN).Any())
                {
                    throw new Exception($@" SN {strInput} does not exist. Please check.");
                }
                if (Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(kp => kp.VALUE == strInput && kp.SN != strInput).Any())
                {
                    throw new Exception($@" SN {strInput} is being used as KP in other unit. Please check.");
                }
                if (Station.SFCDB.ORM.Queryable<R_SN>().Where(sn => strInput == sn.SN && sn.VALID_FLAG == "1" && sn.NEXT_STATION == "SHIPFINISH").Any())
                {
                    throw new Exception($@" SN {strInput} is already shipped. Please check.");
                }
            }
        }
        public static void CheckSKUMatchesWO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage;
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            string skuno = SKUSession.Value.ToString();
            string wo = WOSession.Value.ToString();

            if (string.IsNullOrEmpty(skuno))
            {
                throw new Exception($@"SKU Cannot be empty");
            }

            if(!Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(w => w.WORKORDERNO == wo && w.SKUNO == skuno).Any())
            {
                throw new Exception($@"WO SKU and Input SKU do not match. Please check.");
            }

        }

        public static void CheckSNIsBought(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                MESStationSession vSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (vSN == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

                string strUser = Station.LoginUser.EMP_NO;
                string strSN = vSN.Value.ToString().ToUpper();
                var sfcdb = Station.SFCDB;

                if(sfcdb.ORM.Queryable<R_SN>().Where(r => r.SN == strSN && r.VALID_FLAG != "0").Any())
                {
                    throw new MESReturnMessage($@" SN {strSN} already exists in MES. Please check SN Report");
                }

                if (sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(s => s.SN == strSN && s.STATE_FLAG == "1").Any())
                {
                    throw new MESReturnMessage($@" SN {strSN} already exists in Silver Wip. Please check Silver Wip Detail Report");
                }

                if (sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(s => s.SN == strSN && s.STATE_FLAG == "0").Any())
                {
                    throw new MESReturnMessage($@" SN {strSN} was already Silverwip. Please check with PM.");
                }
                 
                if (strSN.StartsWith("EA"))
                {
                    throw new MESReturnMessage(" Please input only external Serial Numbers, not Foxconn.");
                }

                if(sfcdb.ORM.Queryable<R_SN_KP>().Where(k => k.VALUE == strSN && k.VALID_FLAG != 0).Any())
                {
                    throw new MESReturnMessage($@" SN {strSN} was once used as keypart / child. Please check BEEN LINK REPORT.");
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(ex.Message);
            }

        }

    }
}
