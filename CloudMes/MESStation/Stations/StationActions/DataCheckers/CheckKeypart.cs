using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESStation.HateEmsGetDataService;
using System.Net;
using System.Text.RegularExpressions;
using MESDBHelper;
using System.Data; 
using MESPubLab.MESStation.MESReturnView.Station;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckKeypart
    {
        public static void JuniperPROP65checker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PacknoSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PacknoSNSession == null)
            {
                //throw new Exception($@"Can't Find PacknoSession: Type '{ Paras[0].SESSION_TYPE}' , Key:'{Paras[0].SESSION_KEY}'");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134408", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY }));
            }
            string PackNo = PacknoSNSession.Value.ToString();
            var sfcdb = Station.SFCDB;
            var P = sfcdb.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).First();
            if (P == null)
            {
                //throw new Exception($@"PACKNO:{PackNo} not exist");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135421", new string[] { PackNo }));
            }
            List<R_PACKING> CTNS = new List<R_PACKING>();
            if (P.PACK_TYPE == "PALLET")
            {
                CTNS = sfcdb.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == P.ID).ToList();
            }
            else
            {
                CTNS.Add(P);
            }

            if (CTNS.Count == 0)
            {
                return;
            }

            var wo = sfcdb.ORM.Queryable<R_WO_BASE, R_SN, R_SN_PACKING>((w, s, l) => w.WORKORDERNO == s.WORKORDERNO && s.ID == l.SN_ID).
                Where((w, s, l) => l.PACK_ID == CTNS[0].ID).Select((w, s, l)=>w) .First();
            if (wo == null)
            {
                return;
            }
            //R_PRE_WO_HEAD
            var po = sfcdb.ORM.Queryable<R_PRE_WO_HEAD, O_ORDER_MAIN>((w,p)=>w.MAINID == p.ID).Where((w,p) => w.WO == wo.WORKORDERNO)
                .Select((w, p)=>p).First();
            if (po == null)
            {
                return;
            }
            var i137_item = sfcdb.ORM.Queryable<O_I137_ITEM>().Where(t => t.PONUMBER == po.PONO && t.ITEM == po.POLINE)
                .OrderBy(t => t.VERSION, SqlSugar.OrderByType.Desc).ToList();

            if (i137_item.Count == 0)
            {
                //throw new Exception($@"Can't find o_i137_item PO:{po.PONO},item:{po.POLINE}");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135453", new string[] { po.PONO, po.POLINE }));
            }

            //var i137 = sfcdb.ORM.Queryable<O_I137>().Where(t => t.FILENAME == i137_item[0].FILENAME).First();
            //if (i137 == null)
            //{
            //    throw new Exception($@"Can't find o_i137_item PO:{po.PONO},item:{po.POLINE}");
            //}
            if (i137_item[0].COUNTRYSPECIFICLABEL != ""&& i137_item[0].COUNTRYSPECIFICLABEL !=null)
            {
                UIInputData I = new UIInputData()
                {
                    MustConfirm = false,
                    Timeout = 3000000,
                    IconType = IconType.None,
                    UIArea = new string[] { "90%", "90%" },
                    //Message = "SN",
                    Tittle = "Check COUNTRYSPECIFICLABEL",
                    Type = UIInputType.String,
                    //Name = row.PN,
                    ErrMessage = "No input",
                    CBMessage = "",
                    Message = $@"Pls affix the {i137_item[0].COUNTRYSPECIFICLABEL} Label on Carton.Y/N"
                };
                var ret = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                if (ret.ToUpper().Trim() == "N")
                {
                    //throw new Exception($@"Pls fix the  {i137_item[0].COUNTRYSPECIFICLABEL} Label on Carton");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135539", new string[] { i137_item[0].COUNTRYSPECIFICLABEL }));
                }
            }


            //I137_I
            // Pls affix the PROP65 Label on Carton



        }

        public static void JuniperCablechecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PacknoSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PacknoSNSession == null)
            {
                //throw new Exception($@"Can't Find PacknoSession: Type '{ Paras[0].SESSION_TYPE}' , Key:'{Paras[0].SESSION_KEY}'");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134408", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY }));
            }
            string PackNo = PacknoSNSession.Value.ToString();
            var sfcdb = Station.SFCDB;
            var P = sfcdb.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).First();
            if (P == null)
            {
                //throw new Exception($@"PACKNO:{PackNo} not exist");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135421", new string[] { PackNo }));
            }
            List<R_PACKING> CTNS = new List<R_PACKING>();
            if (P.PACK_TYPE == "PALLET")
            {
                CTNS = sfcdb.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == P.ID).ToList();
            }
            else
            {
                CTNS.Add(P);
            }

            if (CTNS.Count == 0)
            {
                return;
            }

            var wo = sfcdb.ORM.Queryable<R_WO_BASE, R_SN, R_SN_PACKING>((w, s, l) => w.WORKORDERNO == s.WORKORDERNO && s.ID == l.SN_ID).
                Where((w, s, l) => l.PACK_ID == CTNS[0].ID).Select((w, s, l) => w).First();
            if (wo == null)
            {
                return;
            }
            //R_PRE_WO_HEAD
            var po = sfcdb.ORM.Queryable<R_PRE_WO_HEAD, O_ORDER_MAIN>((w, p) => w.MAINID == p.ID).Where((w, p) => w.WO == wo.WORKORDERNO)
                .Select((w, p) => p).First();
            if (po == null)
            {
                return;
            }
            var i137_item = sfcdb.ORM.Queryable<O_I137_ITEM>().Where(t => t.PONUMBER == po.PONO && t.ITEM == po.POLINE)
                .OrderBy(t => t.VERSION, SqlSugar.OrderByType.Desc).ToList();

            if (i137_item.Count == 0)
            {
                //throw new Exception($@"Can't find o_i137_item PO:{po.PONO},item:{po.POLINE}");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135453", new string[] { po.PONO, po.POLINE }));
            }



            var TRANID = i137_item[0].TRANID;
            var I137_details = sfcdb.ORM.Queryable<O_I137_DETAIL>().Where(t => t.TRANID == TRANID).ToList();
            var I137_detail = I137_details.Find(t => t.COMPONENTID.Contains("CBL") || t.COMPONENTID.ToUpper().Contains("CABLE"));

            if (I137_detail != null)
            {
                UIInputData I = new UIInputData()
                {
                    MustConfirm = false,
                    Timeout = 3000000,
                    IconType = IconType.None,
                    UIArea = new string[] { "90%", "90%" },
                    //Message = "SN",
                    Tittle = "Check Cable",
                    Type = UIInputType.String,
                    //Name = row.PN,
                    ErrMessage = "No input",
                    CBMessage = "",
                    Message = $@"Are the power cables in the packaging Y/N?"
                };
                var ret = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();
                if (ret.ToUpper().Trim() == "N")
                {
                    //throw new Exception($@"Pls fix the  {i137_item[0].COUNTRYSPECIFICLABEL} Label on Carton");
                    throw new MESReturnMessage("Please put power cable into the packaging");
                }
            }



        }


        /// <summary>
        /// 檢查卡通內SN綁定的KP內容
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CTN_KPchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            var pns = Paras.FindAll(t => t.SESSION_TYPE == "PARTNO");
            if (pns.Count == 0)
            {
                return;
            }
            MESStationSession PacknoSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PacknoSNSession == null)
            {
                //throw new Exception($@"Can't Find PacknoSession: Type '{ Paras[0].SESSION_TYPE}' , Key:'{Paras[0].SESSION_KEY}'");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134408", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY }));
            }
            string PackNo = PacknoSNSession.Value.ToString();
            var sfcdb = Station.SFCDB;
            var P = sfcdb.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).First();
            if (P == null)
            {
                //throw new Exception($@"PACKNO:{PackNo} not exist");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135421", new string[] { PackNo }));
            }
            List<R_PACKING> CTNS = new List<R_PACKING>();
            if (P.PACK_TYPE == "PALLET")
            {
                CTNS = sfcdb.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == P.ID).ToList();
            }
            else
            {
                CTNS.Add(P);
            }
            UIInputData I = new UIInputData()
            {
                MustConfirm = false,
                Timeout = 3000000,
                IconType = IconType.None,
                UIArea = new string[] { "90%", "90%" },
                //Message = "SN",
                Tittle = "Check Keypart",
                Type = UIInputType.String,
                //Name = row.PN,
                ErrMessage = "No input",
                CBMessage = ""
            };

            for (int i = 0; i < CTNS.Count; i++)
            {
                //string strSql = $@"select K.* from r_Sn_Packing S, R_SN_KP K WHERE S.SN_ID = K.R_SN_ID AND K.VALID_FLAG <> 0 AND S.PACK_ID = '{CTNS[i].ID}'";
                var kpvs = sfcdb.ORM.Queryable<R_SN_PACKING, R_SN_KP>((s, k) => s.SN_ID == k.R_SN_ID).Where((s, k) => s.PACK_ID == CTNS[i].ID && k.VALID_FLAG > 0)
                    .Select((s, k) => k).ToList();
                for (int j = 0; j < pns.Count; j++)
                {
                    I.CBMessage = "";
                    I.Name = pns[j].SESSION_KEY;
                    I.Message = pns[j].VALUE;
                    while (true)
                    {
                        var ckp = kpvs.FindAll(t => t.PARTNO == pns[j].SESSION_KEY && t.SCANTYPE == pns[j].VALUE);
                        if (ckp.Count == 0)
                        {
                            break;
                        }
                        
                        var val = I.GetUiInput(Station.API, UIInput.Normal, Station).ToString();

                        var fv = ckp.Find(t => t.VALUE == val);
                        if (fv != null)
                        {
                            break;
                        }
                        else
                        {
                            I.CBMessage = "Keypart Check Fail";
                        }
                    }

                }
            }




        }


        /// <summary>
        /// 檢查HWD Link Keypart檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        //public static void SNCallHWWSchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        public static void SNSubKPchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> KPList_Temp = new List<Dictionary<string, string>>();
            Dictionary<string, string> DicKP = new Dictionary<string, string>();
            List<C_KEYPART> SubKP = new List<C_KEYPART>();
            string KpSN = Input.Value.ToString();
            //C_KEYPART SUBKP = null;
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession SubSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SubSNSession == null)
            {
                SubSNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SubSNSession);
            }

            MESStationSession SubKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SubKPSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (KPListSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            SN Sn = new SN(KpSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (Sn == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { KpSN }));
            }

            KPListSession.Value = null;
            SubSNSession.Value = Sn;

            T_R_SN_KEYPART_DETAIL _R_SN_KEYPART_DETAIL = new T_R_SN_KEYPART_DETAIL(Station.SFCDB,DB_TYPE_ENUM.Oracle);
            T_C_KEYPART _C_KEYPART = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KEYPART_DETAIL> KEYPARTDETAIL = new List<R_SN_KEYPART_DETAIL>();
           
            if (Sn.ShippedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000070", new string[] { KpSN }));
            }

            if (Sn.RepairFailedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { KpSN }));
            }

            if (Sn.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { KpSN }));
            }

            if (Sn.CurrentStation == "MRB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { KpSN }));
            }
            else
            {
                //Modify by LLF 2018-04-01，有多階綁定
                //KEYPARTDETAIL = _R_SN_KEYPART_DETAIL.GetKeypartBySN(Station.SFCDB, KpSN, Station.StationName);
                //R_SN_KEYPART_DETAIL kpl = KEYPARTDETAIL.Find(z=>z.VALID=="1");
                //if (kpl != null)
                //{
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000165", new string[] { KpSN }));
                //}
                //else
                //{
                    SubKP = (List<C_KEYPART>)SubKPSession.Value;
                    C_KEYPART ckp= SubKP.Find(c=>c.PART_NO== Sn.SkuNo);
                    if (ckp == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000175", new string[] { Sn.SkuNo }));
                    }
                //}
            }
        }

        /// <summary>
        /// 檢查HWD Link Keypart檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMainKPchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> KPList_Temp = new List<Dictionary<string, string>>();
            Dictionary<string, string> DicKP = new Dictionary<string, string>();
            List<C_KEYPART> MainKP = new List<C_KEYPART>();
            string MainSN = Input.Value.ToString();
      
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession MainSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (MainSNSession == null)
            {
                MainSNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(MainSNSession);
            }

            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (MainKPSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (KPListSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            SN Sn = new SN(MainSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (Sn == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { MainSN }));
            }

            MainSNSession.Value = Sn;

            T_R_SN_KEYPART_DETAIL _R_SN_KEYPART_DETAIL = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_KEYPART _C_KEYPART = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KEYPART_DETAIL> KEYPARTDETAIL = new List<R_SN_KEYPART_DETAIL>();
            if (Sn.ShippedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000070", new string[] { MainSN }));
            }

            if (Sn.RepairFailedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { MainSN }));
            }

            if (Sn.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { MainSN }));
            }

            if (Sn.CurrentStation == "MRB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { MainSN }));
            }
            else
            {
                KEYPARTDETAIL = _R_SN_KEYPART_DETAIL.GetKeypartBySN(Station.SFCDB, MainSN, Station.StationName);
                R_SN_KEYPART_DETAIL KP_Main = KEYPARTDETAIL.Find(z => z.VALID == "1");
                if (KP_Main != null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000165", new string[] { MainSN }));
                }
                else
                {
                    MainKP = (List<C_KEYPART>)MainKPSession.Value;
                    C_KEYPART ckp = MainKP.Find(c => c.PART_NO == Sn.SkuNo);
                    if (ckp == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000175", new string[] { Sn.SkuNo }));
                    }
                    else
                    {
                        KPList_Temp = (List<Dictionary<string, string>>)KPListSession.Value;
                        if (KPList_Temp.Count > 0)
                        {
                            DicKP = KPList_Temp.Find(a => a.ContainsValue(Sn.SerialNo));
                            if (DicKP != null)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000193", new string[] {MainSN }));
                            }
                        }

                        Station.AddMessage("MES00000067", new string[] { MainSN }, StationMessageState.Pass);
                    }

                }
            }
        }

        /// <summary>
        /// 检查当前扫入的 Keypart SN 是否合法
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void KeypartIsVaildChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            MESStationSession CurrentKeypartSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (CurrentKeypartSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052",new string[] { Paras[0].SESSION_TYPE+Paras[0].SESSION_KEY}));
            }

            string CurrentKeypart = CurrentKeypartSession.Value.ToString();

            MESStationSession KeypartSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (KeypartSNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string KeypartSn = KeypartSNSession.Value.ToString();

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            WorkOrder Wo = (WorkOrder)WoSession.Value;

            

            T_R_SN TRS = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_KEYPART TCK = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_SKU TCS = new T_C_SKU(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_KEYPART_RULE TCKR = new T_C_KEYPART_RULE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            bool Valid = true;

            R_SN KeypartSnObject = TRS.GetSN(KeypartSn, Station.SFCDB);

            C_KEYPART Keypart = TCK.GetKeyPartBywo(Wo.WorkorderNo, Station.SFCDB)
                                    .Find(t => t.STATION_NAME == Station.StationName && t.CATEGORY_NAME.Equals(CurrentKeypart));
            if (Keypart != null)
            {
                if (KeypartSnObject != null)
                {
                    if (KeypartSnObject.COMPLETED_FLAG != "1")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144",new string[] { KeypartSn }));
                    }
                    SkuObject Sku = TCS.GetSkuBySn(KeypartSn, Station.SFCDB);
                    if (!(Sku.SkuNo.Equals(Keypart.PART_NO) && Sku.Version.Equals(Keypart.PART_NO_VER)))
                    {
                        Valid = false;
                    }
                }

                if (Keypart.KEYPART_RULE_ID != null)
                {
                    C_KEYPART_RULE Rule = TCKR.GetRuleById(Keypart.KEYPART_RULE_ID, Station.SFCDB);
                    if (Rule != null)
                    {
                        Valid = Regex.IsMatch(KeypartSn, Rule.RULE_EXPRESSION);
                    }
                }
                
            }

            if (!Valid)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181016145309",new string[] { KeypartSn }));
            }
        }

        /// <summary>
        /// 检查綁定keypart sn 的規則
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void KeypartSnRuleChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession CurrentKeypartSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (CurrentKeypartSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            string CurrentKeypart = CurrentKeypartSession.Value.ToString();

            MESStationSession KeypartSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (KeypartSNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string KeypartSn = KeypartSNSession.Value.ToString();

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            WorkOrder Wo = (WorkOrder)WoSession.Value;

            T_C_KEYPART_RULE TCKR = new T_C_KEYPART_RULE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_CONTROL TCC = new T_C_CONTROL(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            bool Valid = true;
            //先檢查
            C_CONTROL Control = Station.SFCDB.ORM.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "WO_SAME_KP_RULE_ITEM" && t.CONTROL_TYPE == Wo.WorkorderNo && t.CONTROL_LEVEL == CurrentKeypart).ToList().FirstOrDefault();
            if (Control == null)
            {
                bool skuExist = Station.SFCDB.ORM.Queryable<C_CONTROL>().Any(t => t.CONTROL_NAME == "SKU_SAME_KP_RULE_INCLUDE" && t.CONTROL_VALUE.IndexOfAny(Wo.SkuNO.ToCharArray()) != -1);

                if (skuExist)
                {
                    bool CategoryExist = Station.SFCDB.ORM.Queryable<C_CONTROL>().Any(t => t.CONTROL_NAME == "SKU_SAME_KP_RULE_CATEGORY" && t.CONTROL_VALUE.IndexOfAny(CurrentKeypart.ToCharArray()) != -1);
                    Valid = CategoryExist;
                }
                else
                {
                    Valid = false;
                }
                List<C_KEYPART_RULE> rule_name = Station.SFCDB.ORM.Queryable<C_KEYPART_RULE>().Where(t => t.RULE_NAME.StartsWith(CurrentKeypart)).ToList();
                bool ruleExist = false;
                if (Valid)
                {
                    foreach (C_KEYPART_RULE rule in rule_name)
                    {
                        if (Regex.IsMatch(KeypartSn, rule.RULE_EXPRESSION))
                        {
                            ruleExist = true;
                            Station.SFCDB.ORM.Insertable<C_CONTROL>(new C_CONTROL()
                            {
                                ID = TCC.GetNewID(Station.BU, Station.SFCDB),
                                CONTROL_NAME = "WO_SAME_KP_RULE_ITEM",
                                CONTROL_VALUE = rule.ID,
                                CONTROL_TYPE = Wo.WorkorderNo,
                                CONTROL_LEVEL = CurrentKeypart,
                                CONTROL_DESC = "",
                                EDIT_EMP = Station.LoginUser.EMP_NO,
                                EDIT_TIME = TCC.GetDBDateTime(Station.SFCDB)
                            }).ExecuteCommand();
                            break;
                        }
                    }
                }
                if (!ruleExist)
                {
                    //輸入的 {keypartsn} 不符合 {currentkeypart} 的任何一個編碼原則
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190802150344", new string[] { KeypartSn }));
                }
            }
            else
            {
                C_KEYPART_RULE Rule = Station.SFCDB.ORM.Queryable<C_KEYPART_RULE>().Where(t => t.ID == Control.CONTROL_VALUE).ToList().FirstOrDefault();
                //比對輸入的keypartsn 是否符合之前綁定的keypart 規則
                if (Rule != null)
                {
                    if (Regex.IsMatch(KeypartSn, Rule.RULE_EXPRESSION))
                    {
                        R_SN_KEYPART_DETAIL detail = Station.SFCDB.ORM.Queryable<R_SN_KEYPART_DETAIL>()
                            .Where(t => t.KEYPART_SN == KeypartSn && t.CATEGORY_NAME == CurrentKeypart).ToList().FirstOrDefault();
                        //查重
                        if (detail != null)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190802150028", new string[] { KeypartSn, detail.SN }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190802", new string[] { KeypartSn, CurrentKeypart }));
                    }
                }
            }
        }

    }
}
