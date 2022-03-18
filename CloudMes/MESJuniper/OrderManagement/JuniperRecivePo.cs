using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
using MESPubLab;
using SqlSugar;
using MESPubLab.MESInterface;
using MESJuniper.Base;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using static MESDataObject.Common.EnumExtensions;
using MESDataObject.Constants;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.SAP_RFC;
using System.Data;
using MESPubLab.MesBase;

namespace MESJuniper.OrderManagement
{
    public class JuniperRecivePo : FunctionBase
    {
        private string Mesdbstr;
        JuniperPreWoGanerate JuniperPreWoGanerate;
        JuniperGroupIdReceive JuniperGroupIdReceive;
        JuniperAutoConfigSku JuniperAutoConfigSku;
        JuniperCreateWo JuniperCreateWo;
        JuniperPreUpoadBom JuniperPreUpoadBom;

        public JuniperRecivePo(string _Mesdbstr, string _bu) : base(_Mesdbstr, _bu)
        {
            Mesdbstr = _Mesdbstr;
            bu = _bu;
            JuniperPreWoGanerate = new JuniperPreWoGanerate(Mesdbstr, bu);
            JuniperGroupIdReceive = new JuniperGroupIdReceive(Mesdbstr, bu);
            JuniperAutoConfigSku = new JuniperAutoConfigSku(Mesdbstr, bu);
        }

        public override void FunctionRun()
        {
            //JuniperPreWoGanerate.Run();
            //JuniperPreUpoadBom.Run();
            //JuniperGroupIdReceive.Run();
            //JuniperAutoConfigSku.Run();
            //JuniperMesPoCancel.Run();
            //JuniperMesPoChange.Run();
        }
    }

    public class JuniperPreWoGanerate : FunctionBase
    {
        JuniperErrType functiontype = JuniperErrType.I137;
        JuniperSubType subfunctiontype = JuniperSubType.JuniperPreWoGanerate;
        public JuniperPreWoGanerate(string _dbstr, string _bu) : base(_dbstr, _bu) { SetFunctionObj(ENUM_O_PO_STATUS.WaitCreatePreWo); servicesName = ServicesEnum.JnpServices; }
        /// <summary>
        /// remark:mapping agile kp
        /// </summary>
        public override void FunctionRun()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                #region 取待處理的Po列表;
                var waitHandleOrder = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID)
                    .Where((m, s) => s.STATUSID == ((ENUM_O_PO_STATUS)GetFunctionObj()).ExtValue() && s.VALIDFLAG == MesBool.Yes.ExtValue()).OrderBy((m, s) => m.EDITTIME, OrderByType.Desc).Select((m, s) => m).ToList();

                var skuconfiglist = db.Queryable<O_SKU_CONFIG>().ToList();
                #endregion
                WoGanerate(db, waitHandleOrder, skuconfiglist);
                #region tiqu
                //foreach (var item in waitHandleOrder)
                //{
                //    JuniperPoTracking trackobj = new JuniperPoTracking(functiontype, subfunctiontype, item.PONO, item.POLINE, string.Empty, db);
                //    try
                //    {
                //        string wo, wotype, wopre = string.Empty;
                //        var orderextendinfo = db.Queryable<I137_H, I137_I>((h, i) => h.TRANID == i.TRANID).Where((h, i) => i.ID == item.ITEMID).Select((h, i) => new { h, i }).ToList().FirstOrDefault();
                //        trackobj.SetTranid(orderextendinfo.i.TRANID);
                //        #region 
                //        #region check agile data
                //        var ItemPid = string.IsNullOrEmpty(orderextendinfo.i.MATERIALID) || orderextendinfo.i.MATERIALID == "" ? orderextendinfo.i.PN : orderextendinfo.i.MATERIALID;
                //        var parenagiledata = db.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == ItemPid).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                //        var agiledata = db.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == orderextendinfo.i.PN).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                //        if (trackobj.ExceptionProcess(parenagiledata == null || string.IsNullOrEmpty(parenagiledata.USER_ITEM_TYPE) || string.IsNullOrEmpty(parenagiledata.OFFERING_TYPE), $@"TranId:{orderextendinfo.i.TRANID} ItemPid:{ItemPid} miss Agile Data,pls check!")
                //            || trackobj.ExceptionProcess(agiledata == null || string.IsNullOrEmpty(agiledata.USER_ITEM_TYPE) || string.IsNullOrEmpty(agiledata.OFFERING_TYPE), $@"TranId:{orderextendinfo.i.TRANID} ItemPid:{orderextendinfo.i.PN} miss Agile Data,pls check!"))
                //            continue;
                //        //update agile data
                //        if (item.USERITEMTYPE == null || item.OFFERINGTYPE == null)
                //        {
                //            item.USERITEMTYPE = parenagiledata.USER_ITEM_TYPE;
                //            item.OFFERINGTYPE = parenagiledata.OFFERING_TYPE;
                //            item.PID = agiledata?.ITEM_NUMBER;
                //            item.POTYPE = new Func<string>(() =>
                //            {
                //                return Syn137.ConverOrderType(parenagiledata);
                //            })();
                //            db.Updateable(item).ExecuteCommand();
                //        }
                //        //update option foxpn
                //        db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.FOXPN == null).ToList().ForEach(f =>
                //            {
                //                f.FOXPN = new Func<string>(() =>
                //                {
                //                    var comagile = db.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == f.PARTNO).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                //                    if (comagile != null)
                //                        return comagile.ITEM_NUMBER;
                //                    throw new Exception($@"CustPn:{f.PARTNO} miss Agile Data,pls check!");
                //                })();
                //                db.Updateable(f).ExecuteCommand();
                //            }
                //        );
                //        #endregion

                //        var plantobj = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "SYSPLANT" && t.FUNCTIONTYPE == "NOSYSTEM").ToList().FirstOrDefault();
                //        string pcode = plantobj != null ? plantobj.VALUE : "";
                //        var skuplantconfig = db.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.PID).ToList().FirstOrDefault();
                //        pcode = skuplantconfig != null ? skuplantconfig.PLANTCODE : pcode;
                //        var rmqwotypeconfig = db.Queryable<C_RMQ_WOTYPE>().ToList();
                //        if (item.ORDERTYPE == ENUM_I137_PoDocType.ZRMQ.ToString())
                //            wotype = rmqwotypeconfig.FindAll(t => t.PLANT == pcode).FirstOrDefault().WOTYPE;
                //        else if (skuplantconfig != null)
                //            wotype = ENUM_J_WOTYPE.ZVJ1.ToString();
                //        else
                //        {
                //            var currentskuconfig = skuconfiglist.FindAll(t => t.USERITEMTYPE == item.USERITEMTYPE && t.OFFERINGTYPE == item.OFFERINGTYPE).ToList().FirstOrDefault();
                //            if (trackobj.ExceptionProcess(currentskuconfig == null || currentskuconfig.SWOTYPE == null,
                //                $@"Pid:{item.PID} FXN not suppose this userItemType and Offeringtype!  userItemType: {item.USERITEMTYPE} ,Offeringtype: {item.OFFERINGTYPE } ,pls check!"))
                //                continue;
                //            var isstandardPro = new Func<bool>(() =>
                //            {
                //                // country speci label
                //                if (!string.IsNullOrEmpty(orderextendinfo.i.COUNTRYSPECIFICLABEL))
                //                    return false;
                //                // country speci label
                //                if (!string.IsNullOrEmpty(orderextendinfo.i.CARTONLABEL1))
                //                    return false;
                //                //power code
                //                if (db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.POWERCODE.ExtValue()).Any())
                //                    return false;
                //                //package
                //                if (db.Queryable<O_SKU_PACKAGE>().Where(t => t.SKUNO == item.PID).Any())
                //                    return false;
                //                return true;
                //            })();
                //            wotype = isstandardPro ? currentskuconfig.SWOTYPE : currentskuconfig.NSWOTYPE;
                //        }
                //        #endregion

                //        var res = db.Ado.UseTran(() =>
                //        {
                //            wo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(wotype, db);
                //            item.PID = agiledata?.ITEM_NUMBER;
                //            item.POTYPE = Syn137.ConverOrderType(parenagiledata);
                //            item.USERITEMTYPE = parenagiledata?.USER_ITEM_TYPE;
                //            item.OFFERINGTYPE = parenagiledata?.OFFERING_TYPE;
                //            item.PREWO = wo;
                //            item.EDITTIME = DateTime.Now;
                //            db.Updateable(item).ExecuteCommand();
                //            var holdobj = JuniperOmBase.JuniperHoldCheck(item.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.PRODUCTION, db);
                //            if (item.PREWO.Length > 0 && holdobj.HoldFlag)
                //                db.Insertable(new R_SN_LOCK()
                //                {
                //                    ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                    WORKORDERNO = item.PREWO,
                //                    LOCK_STATION = "ALL",
                //                    TYPE = "WO",
                //                    LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                    LOCK_REASON = $@"PO is Hold! HoldReason: {holdobj.HoldReason},pls check!",
                //                    LOCK_TIME = DateTime.Now,
                //                    LOCK_EMP = "JNPCUST"
                //                }).ExecuteCommand();

                //            #region ECO_FCO Lock Wo When ECO_FCO In Control List
                //            if (orderextendinfo.h.ECO_FCO != null)
                //            {
                //                var ECO_FCO_Control_List = db.Queryable<R_F_CONTROL>()
                //                .Where(t => t.FUNCTIONNAME == "Juniper_ECO_FCO_Lock" && t.CATEGORY == "WOConversionLock" && t.FUNCTIONTYPE == "NOSYSTEM")
                //                .Select(t => t.VALUE.ToUpper())
                //                .ToList();
                //                if (ECO_FCO_Control_List.Contains(orderextendinfo.h.ECO_FCO.ToUpper()))
                //                {
                //                    db.Insertable(new R_SN_LOCK()
                //                    {
                //                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                        WORKORDERNO = item.PREWO,
                //                        LOCK_STATION = "CONVERTWO",
                //                        TYPE = "WO",
                //                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                        //LOCK_REASON = $@"ECO_FCO WO Conversion Hold! HoldReason: {orderextendinfo.h.SHIPPINGNOTE},pls check!",//when SHIPPINGNOTE.lenth is too long, it will throw exception:value too large for column
                //                        LOCK_REASON = $@"ECO_FCO WO Conversion Hold! HoldReason: ECO_FCO[{orderextendinfo.h.ECO_FCO}] in Control List,pls review PoDetail.ShippingNote!",
                //                        LOCK_TIME = DateTime.Now,
                //                        LOCK_EMP = "MESSYSTEM"
                //                    }).ExecuteCommand();
                //                }

                //                var ECO_FCO_Control_ByStation_List = db.Queryable<R_F_CONTROL>()
                //                .Where(t => t.FUNCTIONNAME == "Juniper_ECO_FCO_Lock" && t.CATEGORY == "Lock_By_Station" && t.FUNCTIONTYPE == "NOSYSTEM")
                //                .ToList();
                //                var eco_fco = ECO_FCO_Control_ByStation_List.Where(t => t.VALUE.ToUpper() == orderextendinfo.h.ECO_FCO.ToUpper()).FirstOrDefault();
                //                if (eco_fco != null)
                //                {
                //                    db.Insertable(new R_SN_LOCK()
                //                    {
                //                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                        WORKORDERNO = item.PREWO,
                //                        LOCK_STATION = eco_fco.EXTVAL,
                //                        TYPE = "WO",
                //                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                        //LOCK_REASON = $@"ECO_FCO Hold By Station! HoldReason: {orderextendinfo.h.SHIPPINGNOTE},pls check!",//when SHIPPINGNOTE.lenth is too long, it will throw exception:value too large for column
                //                        LOCK_REASON = $@"ECO_FCO Hold By Station! HoldReason: ECO_FCO[{orderextendinfo.h.ECO_FCO}] in Control List,pls review PoDetail.ShippingNote!",
                //                        LOCK_TIME = DateTime.Now,
                //                        LOCK_EMP = "MESSYSTEM"
                //                    }).ExecuteCommand();
                //                }
                //            }
                //            #endregion

                //            #region Shipping Note Lock When Shipping Note(To Upper) Include DEV-/ECO-/DEVIATION/MPN
                //            if (orderextendinfo.h.SHIPPINGNOTE != null && !db.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == item.PREWO && t.LOCK_STATUS == MesBool.Yes.ExtValue() && t.LOCK_REASON.StartsWith("ECO_FCO")).Any())
                //            {
                //                var Shipping_Note_Control_List = db.Queryable<R_F_CONTROL>()
                //                .Where(t => t.FUNCTIONNAME == "Juniper_ShippingNote_Lock" && t.CATEGORY == "WOConversionLock" && t.FUNCTIONTYPE == "NOSYSTEM")
                //                .Select(t => t.VALUE.ToUpper())
                //                .ToList();
                //                var ShippingNote = orderextendinfo.h.SHIPPINGNOTE.ToUpper();
                //                var LockOption = Shipping_Note_Control_List.Find(t => ShippingNote.Contains(t));
                //                if (LockOption != null)
                //                {
                //                    db.Insertable(new R_SN_LOCK()
                //                    {
                //                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                        WORKORDERNO = item.PREWO,
                //                        LOCK_STATION = "CONVERTWO",
                //                        TYPE = "WO",
                //                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                        LOCK_REASON = $@"Shipping Note Includes {LockOption} WO Conversion Hold! HoldReason: {orderextendinfo.h.SHIPPINGNOTE},pls check!",
                //                        LOCK_TIME = DateTime.Now,
                //                        LOCK_EMP = "MESSYSTEM"
                //                    }).ExecuteCommand();
                //                }

                //                var Shipping_Note = db.Queryable<R_F_CONTROL>()
                //                .Where(t => t.FUNCTIONNAME == "Juniper_ShippingNote_Lock" && t.CATEGORY == "Lock_By_Station" && t.FUNCTIONTYPE == "NOSYSTEM" && ShippingNote.Contains(t.VALUE.ToUpper()))
                //                .First();
                //                if (Shipping_Note != null)
                //                {
                //                    db.Insertable(new R_SN_LOCK()
                //                    {
                //                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                        WORKORDERNO = item.PREWO,
                //                        LOCK_STATION = Shipping_Note.EXTVAL,
                //                        TYPE = "WO",
                //                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                        LOCK_REASON = $@"Shipping Note Includes {Shipping_Note.VALUE} Hold By Station! HoldReason: {orderextendinfo.h.SHIPPINGNOTE},pls check!",
                //                        LOCK_TIME = DateTime.Now,
                //                        LOCK_EMP = "MESSYSTEM"
                //                    }).ExecuteCommand();
                //                }
                //            }
                //            #endregion

                //            #region Expensive Unit Price Lock In Function Control List 
                //            var cserial = db.Queryable<C_SERIES, C_SKU>((C, S) => C.ID == S.C_SERIES_ID).Where((C, S) => S.SKUNO == item.PID).Select((C, S) => C).First();
                //            //if not sku setting or optics enable lock logic
                //            if (cserial == null || cserial.SERIES_NAME == "Juniper-Optics")
                //            {
                //                var Price_Control_List = new List<object>().Select(t => new { PRICE = 0.00, STATION = "", CATEGORY = "" }).ToList();
                //                try
                //                {
                //                    Price_Control_List = db.Queryable<R_F_CONTROL>()
                //                   .Where(t => t.FUNCTIONNAME == "Juniper_UnitPrice_Lock" && t.FUNCTIONTYPE == "NOSYSTEM")
                //                   .Select(t => new { PRICE = double.Parse(t.VALUE), STATION = t.EXTVAL, t.CATEGORY })
                //                   .Distinct()
                //                   .ToList();
                //                }
                //                catch (Exception)
                //                {
                //                }
                //                for (int i = 0; i < Price_Control_List.Count; i++)
                //                {
                //                    if (double.Parse(orderextendinfo.i.NETPRICE) > Price_Control_List[i].PRICE)
                //                    {
                //                        if (Price_Control_List[i].CATEGORY == "CONVERTWO")
                //                        {
                //                            db.Insertable(new R_SN_LOCK()
                //                            {
                //                                ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                                WORKORDERNO = item.PREWO,
                //                                LOCK_STATION = "CONVERTWO",
                //                                TYPE = "WO",
                //                                LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                                LOCK_REASON = $@"Wo Lock By Unit Price Control!Unit Price {orderextendinfo.i.NETPRICE} More Then {Price_Control_List[i].PRICE}!",
                //                                LOCK_TIME = DateTime.Now,
                //                                LOCK_EMP = "MESSYSTEM"
                //                            }).ExecuteCommand();

                //                        }

                //                        if (Price_Control_List[i].CATEGORY == "Lock_By_Station")
                //                        {
                //                            db.Insertable(new R_SN_LOCK()
                //                            {
                //                                ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                                WORKORDERNO = item.PREWO,
                //                                LOCK_STATION = Price_Control_List[i].STATION,
                //                                TYPE = "WO",
                //                                LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                                LOCK_REASON = $@"Wo Lock By Unit Price Control!Unit Price {orderextendinfo.i.NETPRICE} More Then {Price_Control_List[i].PRICE}!",
                //                                LOCK_TIME = DateTime.Now,
                //                                LOCK_EMP = "MESSYSTEM"
                //                            }).ExecuteCommand();
                //                        }
                //                    }
                //                }
                //            }
                //            #endregion

                //            #region WO Incoming Lock When SKUNO(PID) In Control List
                //            var IncomingControl_List = db.Queryable<R_F_CONTROL>()
                //            .Where(t => t.FUNCTIONNAME == "Juniper_WOIncoming_Lock" && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE == item.PID)
                //            .ToList();
                //            for (int i = 0; i < IncomingControl_List.Count; i++)
                //            {
                //                if (IncomingControl_List[i].CATEGORY == "CONVERTWO")
                //                {
                //                    db.Insertable(new R_SN_LOCK()
                //                    {
                //                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                        WORKORDERNO = item.PREWO,
                //                        LOCK_STATION = "CONVERTWO",
                //                        TYPE = "WO",
                //                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                        LOCK_REASON = $@"Hold By Juniper_WOIncoming_Lock Function Control,HoldReason:{IncomingControl_List[i].EXTVAL},pls check!",
                //                        LOCK_TIME = DateTime.Now,
                //                        LOCK_EMP = "MESSYSTEM"
                //                    }).ExecuteCommand();

                //                }
                //                else if (IncomingControl_List[i].CATEGORY == "Lock_By_Station")
                //                {
                //                    var fex = db.Queryable<R_F_CONTROL_EX>()
                //                    .Where(t => t.DETAIL_ID == IncomingControl_List[i].ID)
                //                    .First();
                //                    db.Insertable(new R_SN_LOCK()
                //                    {
                //                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                //                        WORKORDERNO = item.PREWO,
                //                        LOCK_STATION = IncomingControl_List[i].EXTVAL,
                //                        TYPE = "WO",
                //                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                //                        LOCK_REASON = $@"Hold By Juniper_WOIncoming_Lock Function Control,HoldReason:{fex?.VALUE},pls check!",
                //                        LOCK_TIME = DateTime.Now,
                //                        LOCK_EMP = "MESSYSTEM"
                //                    }).ExecuteCommand();
                //                }
                //            }
                //            #endregion

                //            #region PO status
                //            trackobj.ReleasePoStatus(item.ID, this);
                //            #endregion
                //        });
                //        if (res.IsSuccess)
                //            trackobj.ReleaseFuncExcption();
                //        else
                //            trackobj.ExceptionProcess(true, res.ErrorMessage);
                //    }
                //    catch (Exception e)
                //    {
                //        trackobj.ExceptionProcess(true, e.Message);
                //    }
                //}
                #endregion
            }
        }

        public List<WoGanerateRes> WoGanerate(SqlSugarClient db,List<O_ORDER_MAIN> waitHandleOrder, List<O_SKU_CONFIG> skuconfiglist)
        {
            var reswo = new List<WoGanerateRes>();
            foreach (var item in waitHandleOrder)
            {
                JuniperPoTracking trackobj = new JuniperPoTracking(functiontype, subfunctiontype, item.PONO, item.POLINE, string.Empty, db);
                try
                {
                    string wo, wotype, wopre = string.Empty;
                    var orderextendinfo = db.Queryable<I137_H, I137_I>((h, i) => h.TRANID == i.TRANID).Where((h, i) => i.ID == item.ITEMID).Select((h, i) => new { h, i }).ToList().FirstOrDefault();
                    trackobj.SetTranid(orderextendinfo.i.TRANID);
                    #region 
                    #region check agile data
                    var ItemPid = string.IsNullOrEmpty(orderextendinfo.i.MATERIALID) || orderextendinfo.i.MATERIALID == "" ? orderextendinfo.i.PN : orderextendinfo.i.MATERIALID;
                    var parenagiledata = db.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == ItemPid).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                    var agiledata = db.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == orderextendinfo.i.PN).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                    if (trackobj.ExceptionProcess(parenagiledata == null || string.IsNullOrEmpty(parenagiledata.USER_ITEM_TYPE) || string.IsNullOrEmpty(parenagiledata.OFFERING_TYPE), $@"TranId:{orderextendinfo.i.TRANID} ItemPid:{ItemPid} miss Agile Data,pls check!")
                        || trackobj.ExceptionProcess(agiledata == null || string.IsNullOrEmpty(agiledata.USER_ITEM_TYPE) || string.IsNullOrEmpty(agiledata.OFFERING_TYPE), $@"TranId:{orderextendinfo.i.TRANID} ItemPid:{orderextendinfo.i.PN} miss Agile Data,pls check!"))
                        continue;
                    //update agile data
                    if (item.USERITEMTYPE == null || item.OFFERINGTYPE == null)
                    {
                        item.USERITEMTYPE = parenagiledata.USER_ITEM_TYPE;
                        item.OFFERINGTYPE = parenagiledata.OFFERING_TYPE;
                        item.PID = agiledata?.ITEM_NUMBER;
                        item.POTYPE = new Func<string>(() =>
                        {
                            return Syn137.ConverOrderType(parenagiledata);
                        })();
                        db.Updateable(item).ExecuteCommand();
                    }
                    //update option foxpn
                    db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.FOXPN == null).ToList().ForEach(f =>
                    {
                        f.FOXPN = new Func<string>(() =>
                        {
                            var comagile = db.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == f.PARTNO).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                            if (comagile != null)
                                return comagile.ITEM_NUMBER;
                            throw new Exception($@"CustPn:{f.PARTNO} miss Agile Data,pls check!");
                        })();
                        db.Updateable(f).ExecuteCommand();
                    }
                    );
                    #endregion

                    var plantobj = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "SYSPLANT" && t.FUNCTIONTYPE == "NOSYSTEM").ToList().FirstOrDefault();
                    string pcode = plantobj != null ? plantobj.VALUE : "";
                    var skuplantconfig = db.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.PID).ToList().FirstOrDefault();
                    pcode = skuplantconfig != null ? skuplantconfig.PLANTCODE : pcode;
                    var rmqwotypeconfig = db.Queryable<C_RMQ_WOTYPE>().ToList();
                    if (item.ORDERTYPE == ENUM_I137_PoDocType.ZRMQ.ToString())
                        wotype = rmqwotypeconfig.FindAll(t => t.PLANT == pcode).FirstOrDefault().WOTYPE;
                    else if (skuplantconfig != null)
                        wotype = ENUM_J_WOTYPE.ZVJ1.ToString();
                    else
                    {
                        var currentskuconfig = skuconfiglist.FindAll(t => t.USERITEMTYPE == item.USERITEMTYPE && t.OFFERINGTYPE == item.OFFERINGTYPE).ToList().FirstOrDefault();
                        if (trackobj.ExceptionProcess(currentskuconfig == null || currentskuconfig.SWOTYPE == null,
                            $@"Pid:{item.PID} FXN not suppose this userItemType and Offeringtype!  userItemType: {item.USERITEMTYPE} ,Offeringtype: {item.OFFERINGTYPE } ,pls check!"))
                            continue;
                        var isstandardPro = new Func<bool>(() =>
                        {
                            // country speci label
                            if (!string.IsNullOrEmpty(orderextendinfo.i.COUNTRYSPECIFICLABEL))
                                return false;
                            // country speci label
                            if (!string.IsNullOrEmpty(orderextendinfo.i.CARTONLABEL1))
                                return false;
                            //power code
                            if (db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.POWERCODE.ExtValue()).Any())
                                return false;
                            //package
                            if (db.Queryable<O_SKU_PACKAGE>().Where(t => t.SKUNO == item.PID).Any())
                                return false;
                            return true;
                        })();
                        wotype = isstandardPro ? currentskuconfig.SWOTYPE : currentskuconfig.NSWOTYPE;
                    }
                    #endregion

                    var res = db.Ado.UseTran(() =>
                    {
                        wo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(wotype, db);
                        item.PID = agiledata?.ITEM_NUMBER;
                        item.POTYPE = Syn137.ConverOrderType(parenagiledata);
                        item.USERITEMTYPE = parenagiledata?.USER_ITEM_TYPE;
                        item.OFFERINGTYPE = parenagiledata?.OFFERING_TYPE;
                        item.PREWO = wo;
                        item.EDITTIME = DateTime.Now;
                        db.Updateable(item).ExecuteCommand();
                        var holdobj = JuniperOmBase.JuniperHoldCheck(item.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.PRODUCTION, db);
                        if (item.PREWO.Length > 0 && holdobj.HoldFlag)
                            db.Insertable(new R_SN_LOCK()
                            {
                                ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                WORKORDERNO = item.PREWO,
                                LOCK_STATION = "ALL",
                                TYPE = "WO",
                                LOCK_STATUS = MesBool.Yes.ExtValue(),
                                LOCK_REASON = $@"PO is Hold! HoldReason: {holdobj.HoldReason},pls check!",
                                LOCK_TIME = DateTime.Now,
                                LOCK_EMP = "JNPCUST"
                            }).ExecuteCommand();

                        #region ECO_FCO Lock Wo When ECO_FCO In Control List
                        if (orderextendinfo.h.ECO_FCO != null)
                        {
                            var ECO_FCO_Control_List = db.Queryable<R_F_CONTROL>()
                            .Where(t => t.FUNCTIONNAME == "Juniper_ECO_FCO_Lock" && t.CATEGORY == "WOConversionLock" && t.FUNCTIONTYPE == "NOSYSTEM")
                            .Select(t => t.VALUE.ToUpper())
                            .ToList();
                            if (ECO_FCO_Control_List.Contains(orderextendinfo.h.ECO_FCO.ToUpper()))
                            {
                                db.Insertable(new R_SN_LOCK()
                                {
                                    ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                    WORKORDERNO = item.PREWO,
                                    LOCK_STATION = "CONVERTWO",
                                    TYPE = "WO",
                                    LOCK_STATUS = MesBool.Yes.ExtValue(),
                                    //LOCK_REASON = $@"ECO_FCO WO Conversion Hold! HoldReason: {orderextendinfo.h.SHIPPINGNOTE},pls check!",//when SHIPPINGNOTE.lenth is too long, it will throw exception:value too large for column
                                    LOCK_REASON = $@"ECO_FCO WO Conversion Hold! HoldReason: ECO_FCO[{orderextendinfo.h.ECO_FCO}] in Control List,pls review PoDetail.ShippingNote!",
                                    LOCK_TIME = DateTime.Now,
                                    LOCK_EMP = "MESSYSTEM"
                                }).ExecuteCommand();
                            }

                            var ECO_FCO_Control_ByStation_List = db.Queryable<R_F_CONTROL>()
                            .Where(t => t.FUNCTIONNAME == "Juniper_ECO_FCO_Lock" && t.CATEGORY == "Lock_By_Station" && t.FUNCTIONTYPE == "NOSYSTEM")
                            .ToList();
                            var eco_fco = ECO_FCO_Control_ByStation_List.Where(t => t.VALUE.ToUpper() == orderextendinfo.h.ECO_FCO.ToUpper()).FirstOrDefault();
                            if (eco_fco != null)
                            {
                                db.Insertable(new R_SN_LOCK()
                                {
                                    ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                    WORKORDERNO = item.PREWO,
                                    LOCK_STATION = eco_fco.EXTVAL,
                                    TYPE = "WO",
                                    LOCK_STATUS = MesBool.Yes.ExtValue(),
                                    //LOCK_REASON = $@"ECO_FCO Hold By Station! HoldReason: {orderextendinfo.h.SHIPPINGNOTE},pls check!",//when SHIPPINGNOTE.lenth is too long, it will throw exception:value too large for column
                                    LOCK_REASON = $@"ECO_FCO Hold By Station! HoldReason: ECO_FCO[{orderextendinfo.h.ECO_FCO}] in Control List,pls review PoDetail.ShippingNote!",
                                    LOCK_TIME = DateTime.Now,
                                    LOCK_EMP = "MESSYSTEM"
                                }).ExecuteCommand();
                            }
                        }
                        #endregion

                        #region Shipping Note Lock When Shipping Note(To Upper) Include DEV-/ECO-/DEVIATION/MPN
                        if (orderextendinfo.h.SHIPPINGNOTE != null && !db.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == item.PREWO && t.LOCK_STATUS == MesBool.Yes.ExtValue() && t.LOCK_REASON.StartsWith("ECO_FCO")).Any())
                        {
                            var Shipping_Note_Control_List = db.Queryable<R_F_CONTROL>()
                            .Where(t => t.FUNCTIONNAME == "Juniper_ShippingNote_Lock" && t.CATEGORY == "WOConversionLock" && t.FUNCTIONTYPE == "NOSYSTEM")
                            .Select(t => t.VALUE.ToUpper())
                            .ToList();
                            var ShippingNote = orderextendinfo.h.SHIPPINGNOTE.ToUpper();
                            var LockOption = Shipping_Note_Control_List.Find(t => ShippingNote.Contains(t));
                            if (LockOption != null)
                            {
                                db.Insertable(new R_SN_LOCK()
                                {
                                    ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                    WORKORDERNO = item.PREWO,
                                    LOCK_STATION = "CONVERTWO",
                                    TYPE = "WO",
                                    LOCK_STATUS = MesBool.Yes.ExtValue(),
                                    LOCK_REASON = $@"Shipping Note Includes {LockOption} WO Conversion Hold! HoldReason: {(orderextendinfo.h.SHIPPINGNOTE.Length>500? orderextendinfo.h.SHIPPINGNOTE.Substring(0,499):orderextendinfo.h.SHIPPINGNOTE)},pls check!",
                                    LOCK_TIME = DateTime.Now,
                                    LOCK_EMP = "MESSYSTEM"
                                }).ExecuteCommand();
                            }

                            var Shipping_Note = db.Queryable<R_F_CONTROL>()
                            .Where(t => t.FUNCTIONNAME == "Juniper_ShippingNote_Lock" && t.CATEGORY == "Lock_By_Station" && t.FUNCTIONTYPE == "NOSYSTEM" && ShippingNote.Contains(t.VALUE.ToUpper()))
                            .First();
                            if (Shipping_Note != null)
                            {
                                db.Insertable(new R_SN_LOCK()
                                {
                                    ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                    WORKORDERNO = item.PREWO,
                                    LOCK_STATION = Shipping_Note.EXTVAL,
                                    TYPE = "WO",
                                    LOCK_STATUS = MesBool.Yes.ExtValue(),
                                    LOCK_REASON = $@"Shipping Note Includes {Shipping_Note.VALUE} Hold By Station! HoldReason: {(orderextendinfo.h.SHIPPINGNOTE.Length > 500 ? orderextendinfo.h.SHIPPINGNOTE.Substring(0, 499) : orderextendinfo.h.SHIPPINGNOTE)},pls check!",
                                    LOCK_TIME = DateTime.Now,
                                    LOCK_EMP = "MESSYSTEM"
                                }).ExecuteCommand();
                            }
                        }
                        #endregion

                        #region Expensive Unit Price Lock In Function Control List 
                        var cserial = db.Queryable<C_SERIES, C_SKU>((C, S) => C.ID == S.C_SERIES_ID).Where((C, S) => S.SKUNO == item.PID).Select((C, S) => C).First();
                        //if not sku setting or optics enable lock logic
                        if (cserial == null || cserial.SERIES_NAME == "Juniper-Optics")
                        {
                            var Price_Control_List = new List<object>().Select(t => new { PRICE = 0.00, STATION = "", CATEGORY = "" }).ToList();
                            try
                            {
                                Price_Control_List = db.Queryable<R_F_CONTROL>()
                               .Where(t => t.FUNCTIONNAME == "Juniper_UnitPrice_Lock" && t.FUNCTIONTYPE == "NOSYSTEM")
                               .Select(t => new { PRICE = double.Parse(t.VALUE), STATION = t.EXTVAL, t.CATEGORY })
                               .Distinct()
                               .ToList();
                            }
                            catch (Exception)
                            {
                            }
                            for (int i = 0; i < Price_Control_List.Count; i++)
                            {
                                if (double.Parse(orderextendinfo.i.NETPRICE) > Price_Control_List[i].PRICE)
                                {
                                    if (Price_Control_List[i].CATEGORY == "CONVERTWO")
                                    {
                                        db.Insertable(new R_SN_LOCK()
                                        {
                                            ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                            WORKORDERNO = item.PREWO,
                                            LOCK_STATION = "CONVERTWO",
                                            TYPE = "WO",
                                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                                            LOCK_REASON = $@"Wo Lock By Unit Price Control!Unit Price {orderextendinfo.i.NETPRICE} More Then {Price_Control_List[i].PRICE}!",
                                            LOCK_TIME = DateTime.Now,
                                            LOCK_EMP = "MESSYSTEM"
                                        }).ExecuteCommand();

                                    }

                                    if (Price_Control_List[i].CATEGORY == "Lock_By_Station")
                                    {
                                        db.Insertable(new R_SN_LOCK()
                                        {
                                            ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                            WORKORDERNO = item.PREWO,
                                            LOCK_STATION = Price_Control_List[i].STATION,
                                            TYPE = "WO",
                                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                                            LOCK_REASON = $@"Wo Lock By Unit Price Control!Unit Price {orderextendinfo.i.NETPRICE} More Then {Price_Control_List[i].PRICE}!",
                                            LOCK_TIME = DateTime.Now,
                                            LOCK_EMP = "MESSYSTEM"
                                        }).ExecuteCommand();
                                    }
                                }
                            }
                        }
                        #endregion

                        #region WO Incoming Lock When SKUNO(PID) In Control List
                        var IncomingControl_List = db.Queryable<R_F_CONTROL>()
                        .Where(t => t.FUNCTIONNAME == "Juniper_WOIncoming_Lock" && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE == item.PID)
                        .ToList();
                        for (int i = 0; i < IncomingControl_List.Count; i++)
                        {
                            if (IncomingControl_List[i].CATEGORY == "CONVERTWO")
                            {
                                db.Insertable(new R_SN_LOCK()
                                {
                                    ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                    WORKORDERNO = item.PREWO,
                                    LOCK_STATION = "CONVERTWO",
                                    TYPE = "WO",
                                    LOCK_STATUS = MesBool.Yes.ExtValue(),
                                    LOCK_REASON = $@"Hold By Juniper_WOIncoming_Lock Function Control,HoldReason:{IncomingControl_List[i].EXTVAL},pls check!",
                                    LOCK_TIME = DateTime.Now,
                                    LOCK_EMP = "MESSYSTEM"
                                }).ExecuteCommand();

                            }
                            else if (IncomingControl_List[i].CATEGORY == "Lock_By_Station")
                            {
                                var fex = db.Queryable<R_F_CONTROL_EX>()
                                .Where(t => t.DETAIL_ID == IncomingControl_List[i].ID)
                                .First();
                                db.Insertable(new R_SN_LOCK()
                                {
                                    ID = MesDbBase.GetNewID<R_SN_LOCK>(db, Customer.JUNIPER.ExtValue()),
                                    WORKORDERNO = item.PREWO,
                                    LOCK_STATION = IncomingControl_List[i].EXTVAL,
                                    TYPE = "WO",
                                    LOCK_STATUS = MesBool.Yes.ExtValue(),
                                    LOCK_REASON = $@"Hold By Juniper_WOIncoming_Lock Function Control,HoldReason:{fex?.VALUE},pls check!",
                                    LOCK_TIME = DateTime.Now,
                                    LOCK_EMP = "MESSYSTEM"
                                }).ExecuteCommand();
                            }
                        }
                        #endregion

                        #region PO status
                        reswo.Add(new WoGanerateRes() { issucess = true, upoid = item.UPOID, wo = item.PREWO });
                        trackobj.ReleasePoStatus(item.ID, this);
                        #endregion
                    });
                    if (res.IsSuccess)
                        trackobj.ReleaseFuncExcption();
                    else
                    {
                        reswo.Add(new WoGanerateRes() { issucess = false, upoid = item.UPOID, msg = res.ErrorMessage });
                        trackobj.ExceptionProcess(true, res.ErrorMessage);
                    }
                }
                catch (Exception e)
                {
                    reswo.Add(new WoGanerateRes() { issucess = false, upoid = item.UPOID, msg = e.Message });
                    trackobj.ExceptionProcess(true, e.Message);
                }
            }
            return reswo;
        }

        public class WoGanerateRes
        {
            public bool issucess;
            public string msg;
            public string upoid;
            public string wo;
        }
    }

    public class JuniperPreUpoadBom : FunctionBase
    {
        JuniperErrType functiontype = JuniperErrType.I137;
        JuniperSubType subfunctiontype = JuniperSubType.JuniperOnePreUpoadBom;
        public string plantCode;
        public JuniperPreUpoadBom(string _dbstr, string _bu, string _plantCode) : base(_dbstr, _bu) { this.plantCode = _plantCode; SetFunctionObj(ENUM_O_PO_STATUS.OnePreUploadBom); servicesName = ServicesEnum.JnpServices; }

        /// <summary>
        /// remark:mapping agile kp
        /// </summary>
        public override void FunctionRun()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                #region 取待處理的Po列表;
                var waitHandleOrder = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID)
                    .Where((m, s) => s.STATUSID == ((ENUM_O_PO_STATUS)GetFunctionObj()).ExtValue() && s.VALIDFLAG == MesBool.Yes.ExtValue()).OrderBy((m, s) => m.EDITTIME, OrderByType.Desc).Select((m, s) => m).ToList();
                var skuconfiglist = db.Queryable<O_SKU_CONFIG>().ToList();
                var wotypelist = db.Queryable<O_J_WOTYPE>().ToList();
                #endregion
                var rfc = new ZCPP_NSBG_0302(this.bu);

                foreach (var item in waitHandleOrder)
                {
                    JuniperPoTracking trackobj = new JuniperPoTracking(functiontype, subfunctiontype, item.PONO, item.POLINE, string.Empty, db);
                    try
                    {
                        var i137item = db.Queryable<O_I137_ITEM>().Where(t => t.ID == item.ITEMID).ToList().FirstOrDefault();
                        trackobj.SetTranid(i137item.TRANID);
                        #region bomextend flag
                        //var bomexflag = skuconfiglist.FindAll(t => t.USERITEMTYPE == item.USERITEMTYPE && t.OFFERINGTYPE == item.OFFERINGTYPE).ToList().FirstOrDefault().BOMEXPLOSION;
                        #endregion

                        var skunewplant = db.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.PID).ToList().FirstOrDefault();
                        plantCode = skunewplant != null ? skunewplant.PLANTCODE : plantCode;
                        var wotype = wotypelist.FindAll(t => t.WOPRE == item.PREWO.Substring(0, 4)).ToList().FirstOrDefault();
                        if (wotype == null)
                            continue;
                        var hbmap = db.Queryable<R_PN_HB_MAP>().Where(t => t.CUSTPN == item.CUSTPID).ToList().FirstOrDefault();
                        var agiledata = db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == item.PID && t.ACTIVED == MesBool.Yes.ExtValue()).OrderBy(t => t.RELEASE_DATE, OrderByType.Desc).ToList().FirstOrDefault();

                        #region bomextend flag
                        var bomexflag = skuconfiglist.FindAll(t => t.USERITEMTYPE == agiledata.USER_ITEM_TYPE && t.OFFERINGTYPE == agiledata.OFFERING_TYPE).ToList().FirstOrDefault().BOMEXPLOSION;
                        #endregion

                        DataTable ZWO_HEADER = rfc.GET_NEW_ZWO_HEADER();
                        DataTable ZWO_ITEM = rfc.GET_NEW_ZWO_ITEM();
                        DataTable ZWO_HIDBOM = rfc.GET_NEW_ZWO_HIDBOM();
                        DataRow hdr = ZWO_HEADER.NewRow();
                        hdr["WO"] = item.PREWO;
                        hdr["PID"] = agiledata.CUSTPARTNO;
                        hdr["WOTYPE"] = wotype.WOTYPE;
                        hdr["PLANT"] = plantCode;
                        //hdr["QTY"] = item.QTY;
                        hdr["QTY"] = "1";
                        hdr["PO"] = item.PONO;
                        hdr["EXBOM"] = bomexflag;
                        ZWO_HEADER.Rows.Add(hdr);
                        //OPTION
                        var itemoption = db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.CTO.ExtValue()).ToList();
                        var igonepn = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "JNPPN_IGNORE" && t.FUNCTIONTYPE == "NOSYSTEM").ToList();
                        foreach (var option in itemoption)
                        {
                            if (igonepn.Any(t => t.VALUE == option.PARTNO))
                                continue;
                            var dr = ZWO_ITEM.NewRow();
                            dr["WO"] = item.PREWO;
                            dr["PN"] = option.PARTNO.ToUpper();
                            dr["QTY"] = option.QTY;
                            ZWO_ITEM.Rows.Add(dr);
                        }
                        //HB
                        if (hbmap != null)
                        {
                            DataRow hddr = ZWO_HIDBOM.NewRow();
                            hddr["WO"] = item.PREWO;
                            hddr["HB"] = hbmap.HBPN;
                            hddr["QTY"] = "";
                            ZWO_HIDBOM.Rows.Add(hddr);
                        }

                        rfc.SetValue(plantCode, "", "", ZWO_HEADER, ZWO_ITEM, ZWO_HIDBOM);
                        rfc.CallRFC(() => { MesSapHelp.SapLog(item.ID, rfc.getSapParameobj(), db); });
                        var RETURNTAB = rfc.GetTableValue("RETURN").Select(" TYPE='E' ");
                        DataTable PODETAIL = rfc.GetTableValue("PODETAIL");
                        DataTable BOM_LIST = rfc.GetTableValue("BOM_LIST");
                        DataTable MINI_LIST = rfc.GetTableValue("MINI_LIST");
                        if (PODETAIL.Rows.Count == 0 && GetSapErr(RETURNTAB).IndexOf("has already exist") > -1)
                            trackobj.ReleasePoStatus(item.ID, ENUM_O_PO_STATUS.WaitCreatePreWo, this);
                        else if (PODETAIL.Rows.Count == 0)
                            throw new Exception($@"Pre Upload(sap) Result Is empty,pls check! sap err:{GetSapErr(RETURNTAB)}");
                        var res = db.Ado.UseTran(() =>
                        {
                            foreach (DataRow dr in PODETAIL.Rows)
                            {
                                var bomobj = new R_SAP_PODETAIL()
                                {
                                    ID = MesDbBase.GetNewID<R_SAP_PODETAIL>(db, Customer.JUNIPER.ExtValue()),
                                    WO = dr["AUFNR"].ToString(),
                                    PLANT = dr["WERKS"].ToString(),
                                    PN = dr["MATNR"].ToString(),
                                    PID = dr["FMATNR"].ToString(),
                                    PIDREV = dr["FREVLV"].ToString(),
                                    ORDERQTY = dr["BDMNG"].ToString(),
                                    PNREV = dr["REVLV"].ToString(),
                                    SPARTDESC = dr["MAKTX"].ToString(),
                                    PPARTDESC = dr["FMAKTX"].ToString(),
                                    CREATETIME = DateTime.Now
                                };
                                db.Insertable(bomobj).ExecuteCommand();
                            }
                            foreach (DataRow dr in BOM_LIST.Rows)
                            {
                                var bomobj = new R_SAP_AS_BOM()
                                {
                                    ID = MesDbBase.GetNewID<R_SAP_AS_BOM>(db, Customer.JUNIPER.ExtValue()),
                                    WO = dr["AUFNR"].ToString(),
                                    PN = dr["IDNRK"].ToString(),
                                    USAGE = dr["MENGE"].ToString(),
                                    PARENTPN = dr["MATNR"].ToString(),
                                    CUSTPN = dr["PIDNRK"].ToString(),
                                    CUSTPARENTPN = dr["PMATNR"].ToString(),
                                    CLEI1 = dr["POTX1"].ToString(),
                                    CLEI2 = dr["POTX2"].ToString(),
                                    SPARTDESC = dr["MAKTX"].ToString(),
                                    PPARTDESC = dr["FMAKTX"].ToString(),
                                    PNREV = dr["REVLV"].ToString(),
                                    WASTAGE = dr["KAUSF"].ToString(),
                                    PARENTREV = dr["MREVLV"].ToString(),
                                    CREATETIME = DateTime.Now
                                };
                                db.Insertable(bomobj).ExecuteCommand();
                            }
                            foreach (DataRow dr in MINI_LIST.Rows)
                            {
                                var bomobj = new R_SAP_HB()
                                {
                                    ID = MesDbBase.GetNewID<R_SAP_HB>(db, Customer.JUNIPER.ExtValue()),
                                    WO = dr["AUFNR"].ToString(),
                                    PN = dr["IDNRK"].ToString(),
                                    USAGE = dr["MENGE"].ToString(),
                                    PARENTPN = dr["MATNR"].ToString(),
                                    CUSTPN = dr["PIDNRK"].ToString(),
                                    CUSTPARENTPN = dr["PMATNR"].ToString(),
                                    CLEI1 = dr["POTX1"].ToString(),
                                    CLEI2 = dr["POTX2"].ToString(),
                                    SPARTDESC = dr["MAKTX"].ToString(),
                                    PPARTDESC = dr["FMAKTX"].ToString(),
                                    PNREV = dr["REVLV"].ToString(),
                                    WASTAGE = dr["KAUSF"].ToString(),
                                    HBREV = dr["MREVLV"].ToString(),
                                    CREATETIME = DateTime.Now
                                };
                                db.Insertable(bomobj).ExecuteCommand();
                            }
                            #region PO status
                            trackobj.ReleasePoStatus(item.ID, this);
                            #endregion
                        });
                        trackobj.ReleaseFuncExcption();
                    }
                    catch (Exception e)
                    {
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }

        string GetSapErr(DataRow[] drs)
        {
            var res = "";
            foreach (var item in drs)
                res += $@" {item["MESSAGE"]}";
            return res;
        }
    }

    public class JuniperSecUpoadBom : FunctionBase
    {
        JuniperErrType functiontype = JuniperErrType.I137;
        JuniperSubType subfunctiontype = JuniperSubType.JuniperSecPreUpoadBom;
        public string plantCode;
        public JuniperSecUpoadBom(string _dbstr, string _bu, string _plantCode) : base(_dbstr, _bu) { this.plantCode = _plantCode; SetFunctionObj(ENUM_O_PO_STATUS.SecPreUploadBom); servicesName = ServicesEnum.JnpServices; }

        /// <summary>
        /// remark:mapping agile kp
        /// </summary>
        public override void FunctionRun()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                #region 取待處理的Po列表;
                var waitHandleOrder = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID)
                    .Where((m, s) => s.STATUSID == ((ENUM_O_PO_STATUS)GetFunctionObj()).ExtValue() && s.VALIDFLAG == MesBool.Yes.ExtValue()).OrderBy((m, s) => m.EDITTIME, OrderByType.Desc).Select((m, s) => m).ToList();
                var skuconfiglist = db.Queryable<O_SKU_CONFIG>().ToList();
                var wotypelist = db.Queryable<O_J_WOTYPE>().ToList();
                #endregion
                var rfc = new ZCPP_NSBG_0302(this.bu);
                foreach (var item in waitHandleOrder)
                {
                    JuniperPoTracking trackobj = new JuniperPoTracking(functiontype, subfunctiontype, item.PONO, item.POLINE, string.Empty, db);
                    try
                    {
                        var i137item = db.Queryable<O_I137_ITEM>().Where(t => t.ID == item.ITEMID).ToList().FirstOrDefault();
                        trackobj.SetTranid(i137item.TRANID);
                        #region bomextend flag
                        //var bomexflag = skuconfiglist.FindAll(t => t.USERITEMTYPE == item.USERITEMTYPE && t.OFFERINGTYPE == item.OFFERINGTYPE).ToList().FirstOrDefault().BOMEXPLOSION;
                        #endregion

                        var skunewplant = db.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.PID).ToList().FirstOrDefault();
                        plantCode = skunewplant != null ? skunewplant.PLANTCODE : plantCode;
                        var wotype = wotypelist.FindAll(t => t.WOPRE == item.PREWO.Substring(0, 4)).ToList().FirstOrDefault();
                        if (wotype == null)
                            continue;
                        var hbmap = db.Queryable<R_PN_HB_MAP>().Where(t => t.CUSTPN == item.CUSTPID).ToList().FirstOrDefault();
                        var agiledata = db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == item.PID && t.ACTIVED == MesBool.Yes.ExtValue()).OrderBy(t => t.RELEASE_DATE, OrderByType.Desc).ToList().FirstOrDefault();

                        #region bomextend flag
                        var bomexflag = skuconfiglist.FindAll(t => t.USERITEMTYPE == agiledata.USER_ITEM_TYPE && t.OFFERINGTYPE == agiledata.OFFERING_TYPE).ToList().FirstOrDefault().BOMEXPLOSION;
                        #endregion

                        DataTable ZWO_HEADER = rfc.GET_NEW_ZWO_HEADER();
                        DataTable ZWO_ITEM = rfc.GET_NEW_ZWO_ITEM();
                        DataTable ZWO_HIDBOM = rfc.GET_NEW_ZWO_HIDBOM();
                        DataRow hdr = ZWO_HEADER.NewRow();
                        hdr["WO"] = item.PREWO;
                        hdr["PID"] = agiledata.CUSTPARTNO;
                        hdr["WOTYPE"] = wotype.WOTYPE;
                        hdr["PLANT"] = plantCode;
                        //hdr["QTY"] = item.QTY;
                        hdr["QTY"] = "1";
                        hdr["PO"] = item.PONO;
                        hdr["EXBOM"] = bomexflag;
                        ZWO_HEADER.Rows.Add(hdr);
                        //OPTION
                        var itemoption = db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.CTO.ExtValue()).ToList();
                        var igonepn = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "JNPPN_IGNORE" && t.FUNCTIONTYPE == "NOSYSTEM").ToList();
                        foreach (var option in itemoption)
                        {
                            if (igonepn.Any(t => t.VALUE == option.PARTNO))
                                continue;
                            var dr = ZWO_ITEM.NewRow();
                            dr["WO"] = item.PREWO;
                            dr["PN"] = option.PARTNO.ToUpper();
                            dr["QTY"] = option.QTY;
                            ZWO_ITEM.Rows.Add(dr);
                        }
                        //HB
                        if (hbmap != null)
                        {
                            DataRow hddr = ZWO_HIDBOM.NewRow();
                            hddr["WO"] = item.PREWO;
                            hddr["HB"] = hbmap.HBPN;
                            hddr["QTY"] = "";
                            ZWO_HIDBOM.Rows.Add(hddr);
                        }

                        rfc.SetValue(plantCode, "", "", ZWO_HEADER, ZWO_ITEM, ZWO_HIDBOM);
                        rfc.CallRFC(() => { MesSapHelp.SapLog(item.ID, rfc.getSapParameobj(), db); });

                        DataTable PODETAIL = rfc.GetTableValue("PODETAIL");
                        DataTable BOM_LIST = rfc.GetTableValue("BOM_LIST");
                        DataTable MINI_LIST = rfc.GetTableValue("MINI_LIST");

                        var om_podetail = db.Queryable<R_SAP_PODETAIL>().Where(t => t.WO == item.PREWO).ToList();
                        var om_bomlist = db.Queryable<R_SAP_AS_BOM>().Where(t => t.WO == item.PREWO).ToList();
                        var om_minilist = db.Queryable<R_SAP_HB>().Where(t => t.WO == item.PREWO).ToList();


                        var checkres = true;
                        if (om_podetail.Count != PODETAIL.Rows.Count
                            || om_bomlist.Count != BOM_LIST.Rows.Count
                            || om_minilist.Count != MINI_LIST.Rows.Count)
                            checkres = false;
                        foreach (DataRow dr in PODETAIL.Rows)
                            trackobj.ExceptionProcess(!om_podetail.FindAll(t => t.WO == dr["AUFNR"].ToString()
                            && t.PLANT == dr["WERKS"].ToString()
                            && t.PN == dr["MATNR"].ToString()
                            && (t.PNREV == dr["REVLV"].ToString() || (t.PNREV == null && dr["REVLV"].ToString() == ""))
                            && t.ORDERQTY == dr["BDMNG"].ToString()
                            && (t.PIDREV == dr["FREVLV"].ToString() || (t.PIDREV == null && dr["FREVLV"].ToString() == ""))
                            && t.SPARTDESC == dr["MAKTX"].ToString()
                            && t.PPARTDESC == dr["FMAKTX"].ToString()).Any(),
                            $@"The BOM has been changed(PODETAIL),pn: {dr["MATNR"].ToString()} ,rev: {dr["FREVLV"].ToString()}, Orderqty: {dr["BDMNG"].ToString()},pls check!", () => { checkres = false; });
                        foreach (DataRow dr in BOM_LIST.Rows)
                        {
                            trackobj.ExceptionProcess(!om_bomlist.FindAll(t => t.WO == dr["AUFNR"].ToString()
                             && t.PN == dr["IDNRK"].ToString()
                             && t.USAGE == dr["MENGE"].ToString()
                             && t.PARENTPN == dr["MATNR"].ToString()
                             && t.CUSTPN == dr["PIDNRK"].ToString()
                             && t.CUSTPARENTPN == dr["PMATNR"].ToString()
                              && (t.CLEI1 == dr["POTX1"].ToString() || (t.CLEI1 == null && dr["POTX1"].ToString() == ""))
                              && (t.CLEI2 == dr["POTX2"].ToString() || (t.CLEI2 == null && dr["POTX2"].ToString() == ""))
                             && t.SPARTDESC == dr["MAKTX"].ToString()
                             && t.PPARTDESC == dr["FMAKTX"].ToString()).Any(),
                             $@"The BOM has been changed(BOM_LIST),pn: {dr["IDNRK"].ToString()},pls check!", () => { checkres = false; });
                        }
                        foreach (DataRow dr in MINI_LIST.Rows)
                            trackobj.ExceptionProcess(!om_minilist.FindAll(t => t.WO == dr["AUFNR"].ToString()
                                && t.WO == dr["AUFNR"].ToString()
                                && t.PN == dr["IDNRK"].ToString()
                                && t.USAGE == dr["MENGE"].ToString()
                                && t.PARENTPN == dr["MATNR"].ToString()
                                && t.CUSTPN == dr["PIDNRK"].ToString()
                                && t.CUSTPARENTPN == dr["PMATNR"].ToString()
                                && (t.CLEI1 == dr["POTX1"].ToString() || (t.CLEI1 == null && dr["POTX1"].ToString() == ""))
                                && (t.CLEI2 == dr["POTX2"].ToString() || (t.CLEI2 == null && dr["POTX2"].ToString() == ""))
                                && t.SPARTDESC == dr["MAKTX"].ToString()
                                && t.PPARTDESC == dr["FMAKTX"].ToString()).Any(),
                                $@"The BOM has been changed(Hb),pn: {dr["IDNRK"].ToString()},pls check!", () => { checkres = false; });
                        var res = db.Ado.UseTran(() =>
                        {
                            if (checkres)
                                #region PO status
                                trackobj.ReleasePoStatus(item.ID, this);
                            #endregion
                            else
                                #region PO status
                                trackobj.ReleasePoStatus(item.ID, ENUM_O_PO_STATUS.WaitCreatePreWo, this);
                            #endregion
                        });
                        trackobj.ReleaseFuncExcption();
                    }
                    catch (Exception e)
                    {
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }
    }

    public class AddNonBom : FunctionBase
    {
        JuniperErrType functiontype = JuniperErrType.I137;
        JuniperSubType subfunctiontype = JuniperSubType.AddNonBom;
        public string plantCode;
        private List<O_WHS_PACKAGE> whsPackageObj;

        public AddNonBom(string _dbstr, string _bu, string _plantCode) : base(_dbstr, _bu)
        {
            SetFunctionObj(ENUM_O_PO_STATUS.AddNonBom);
            servicesName = ServicesEnum.JnpServices;
            this.plantCode = _plantCode;
        }

        /// <summary>
        /// changeindicator=?:Cancel Po
        /// </summary>
        public override void FunctionRun()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                var waitHandleOrder = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, s) => m.ID == s.POID)
                    .Where((m, s) => s.STATUSID == ((ENUM_O_PO_STATUS)GetFunctionObj()).ExtValue() && s.VALIDFLAG == MesBool.Yes.ExtValue()).OrderBy((m, s) => m.EDITTIME, OrderByType.Desc).Select((m, s) => m).ToList();
                var blackpns = db.Queryable<R_JNP_FULLMATCH>().Where(t=>t.VALIDFLAG==MesBool.Yes.ExtValue()).ToList();
                foreach (var item in waitHandleOrder)
                {
                    JuniperPoTracking trackobj = new JuniperPoTracking(functiontype, subfunctiontype, item.PONO, item.POLINE, string.Empty, db);
                    var ordedetailinfo = db.Queryable<I137_I, I137_H>((i, h) => i.TRANID == h.TRANID).Where((i, h) => i.ID == item.ITEMID).Select((i, h) => new { i, h }).ToList().FirstOrDefault();
                    try
                    {
                        if (trackobj.ExceptionProcess(db.Queryable<R_PRE_WO_HEAD>().Any(t => t.WO == item.PREWO), $@"Order Status Is Err,pls check!"))
                            continue;
                        trackobj.SetTranid(ordedetailinfo.i.TRANID);

                        var createtime = DateTime.Now;

                        #region lable check
                        // country speci label
                        if (!string.IsNullOrEmpty(ordedetailinfo.i.COUNTRYSPECIFICLABEL) && !ordedetailinfo.i.COUNTRYSPECIFICLABEL.Equals("NA") &&
                            trackobj.ExceptionProcess(
                                !db.Queryable<O_137_COO_LABEL>().Where(t => t.COOVALUE == ordedetailinfo.i.COUNTRYSPECIFICLABEL.Trim()).Any(),
                                $@" CountrySpeciLable havn't config label partno!  CountrySpeciLable: {ordedetailinfo.i.COUNTRYSPECIFICLABEL.Trim()} ,pls check!"))
                            continue;
                        // country speci label
                        if (!string.IsNullOrEmpty(ordedetailinfo.i.CARTONLABEL1) &&
                            trackobj.ExceptionProcess(
                                !db.Queryable<O_137CARTON_LABEL>().Where(t => t.SPECVAL == ordedetailinfo.i.CARTONLABEL1.Trim() && t.SKUNO == item.PID).Any(),
                                $@" CartonLable havn't config label partno!  CartonLable: {ordedetailinfo.i.CARTONLABEL1.Trim()},Skuno: {item.PID} ,pls check!"))
                            continue;
                        #endregion

                        var detaillist = new List<R_PRE_WO_DETAIL>();
                        #region SAPEX
                        var podetail = db.Queryable<R_SAP_PODETAIL>().Where(t => t.WO == item.PREWO).ToList();
                        foreach (var optionitem in podetail)
                            detaillist.Add(new R_PRE_WO_DETAIL
                            {
                                ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                WO = item.PREWO,
                                PARTNO = optionitem.PN,
                                REQUESTQTY = optionitem.ORDERQTY,
                                PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.SAPEX.ExtValue(),
                                CREATETIME = createtime
                            });
                        #endregion
                        #region POWERCODE
                        var optionpowercode = db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.POWERCODE.ExtValue()).ToList().FirstOrDefault();
                        if (optionpowercode != null)
                            detaillist.Add(new R_PRE_WO_DETAIL
                            {
                                ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                WO = item.PREWO,
                                PARTNO = optionpowercode.PARTNO,
                                REQUESTQTY = optionpowercode.QTY.ToString(),
                                PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.POWERCODE.ExtValue(),
                                CREATETIME = createtime
                            });
                        #endregion
                        #region BNDL 不參與Groupid
                        //var optionBndlcode = db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.BNDL.ExtValue()).ToList().FirstOrDefault();
                        //if (optionBndlcode != null)
                        //    detaillist.Add(new R_PRE_WO_DETAIL
                        //    {
                        //        ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                        //        WO = item.PREWO,
                        //        PARTNO = optionBndlcode.PARTNO,
                        //        REQUESTQTY = optionBndlcode.QTY.ToString(),
                        //        PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.BNDL.ExtValue(),
                        //        CREATETIME = System.DateTime.Now,
                        //    });
                        #endregion
                        #region COUNTRYLABEL
                        if (!string.IsNullOrEmpty(ordedetailinfo.i.COUNTRYSPECIFICLABEL) && !ordedetailinfo.i.COUNTRYSPECIFICLABEL.Equals("NA"))
                        {
                            var configcoolable = db.Queryable<O_137_COO_LABEL>().Where(t => t.COOVALUE == ordedetailinfo.i.COUNTRYSPECIFICLABEL.Trim()).ToList().FirstOrDefault();
                            if (configcoolable != null)
                                detaillist.Add(new R_PRE_WO_DETAIL
                                {
                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                    WO = item.PREWO,
                                    PARTNO = configcoolable.PARTNO,
                                    REQUESTQTY = 1.ToString(),
                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.COUNTRYLABEL.ExtValue(),
                                    CREATETIME = createtime
                                });
                        }
                        #endregion
                        #region CARTONLABEL :how calculate usage?
                        if (!string.IsNullOrEmpty(ordedetailinfo.i.CARTONLABEL1))
                        {
                            var configcartonlable = db.Queryable<O_137CARTON_LABEL>().Where(t => t.SPECVAL == ordedetailinfo.i.CARTONLABEL1.Trim()).ToList().FirstOrDefault();
                            if (configcartonlable != null)
                                detaillist.Add(new R_PRE_WO_DETAIL
                                {
                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                    WO = item.PREWO,
                                    PARTNO = configcartonlable.PARTNO,
                                    REQUESTQTY = configcartonlable.QTY,
                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.CARTONLABEL.ExtValue(),
                                    CREATETIME = createtime
                                });
                        }
                        #endregion
                        var skupageobj = db.Queryable<O_SKU_PACKAGE>().Where(t => t.SKUNO == item.PID).OrderBy(t => t.TON, OrderByType.Desc).ToList();
                        #region PACKAGE
                        ENUM_O_SKU_PACKAGE_SCENARIO Scenariotype = string.IsNullOrEmpty(ordedetailinfo.i.CARTONLABEL2) && item.USERITEMTYPE != "BNDL" ? ENUM_O_SKU_PACKAGE_SCENARIO.Overpack : ENUM_O_SKU_PACKAGE_SCENARIO.Multipack;
                        if (Scenariotype == ENUM_O_SKU_PACKAGE_SCENARIO.Multipack)
                        {
                            #region part1
                            var delpackrule = skupageobj.FindAll(t => t.SCENARIO == ENUM_O_SKU_PACKAGE_SCENARIO.Overpack.ExtValue() && t.TYPE == ENUM_O_SKU_PACKAGE_PACKTYPE.Deletion.ExtValue()).ToList();
                            foreach (var delpnitem in delpackrule)
                                detaillist.RemoveAll(t => t.PARTNO == delpnitem.PARTNO);

                            var addpackrule = skupageobj.FindAll(t => t.SCENARIO == ENUM_O_SKU_PACKAGE_SCENARIO.Multipack.ExtValue() && t.TYPE == ENUM_O_SKU_PACKAGE_PACKTYPE.Compulsory.ExtValue()).ToList();
                            if (item.PLANT.ToUpper().Equals("FVN"))
                            {
                                foreach (var addpnitem in addpackrule)
                                    detaillist.Add(new R_PRE_WO_DETAIL
                                    {
                                        ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                        WO = item.PREWO,
                                        PARTNO = addpnitem.PARTNO,
                                        REQUESTQTY = addpnitem.USAGE,
                                        PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                        CREATETIME = createtime
                                    });
                            }
                            else
                            {
                                if (item.OFFERINGTYPE.ToUpper().Equals("Fixed Nonstockable Bundle".ToUpper()) || (ordedetailinfo.i.CARTONLABEL2 != null && ordedetailinfo.i.CARTONLABEL2.ToUpper().Equals("Bulk".ToUpper())))
                                {
                                    foreach (var addpnitem in addpackrule)
                                        detaillist.Add(new R_PRE_WO_DETAIL
                                        {
                                            ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                            WO = item.PREWO,
                                            PARTNO = addpnitem.PARTNO,
                                            //REQUESTQTY = addpnitem.USAGE,
                                            REQUESTQTY = RoundUp((RoundUp(Convert.ToDouble(item.QTY) / 50, 0) * Convert.ToDouble(addpnitem.USAGE) / Convert.ToDouble(item.QTY)), 3).ToString(),
                                            PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                            CREATETIME = createtime
                                        });
                                }
                                else
                                {
                                    foreach (var addpnitem in addpackrule)
                                        detaillist.Add(new R_PRE_WO_DETAIL
                                        {
                                            ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                            WO = item.PREWO,
                                            PARTNO = addpnitem.PARTNO,
                                            REQUESTQTY = addpnitem.USAGE,
                                            PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                            CREATETIME = createtime
                                        });
                                }
                            }
                            #endregion

                            #region part2
                            var addcartonpackrule = skupageobj.FindAll(t => t.SCENARIO == ENUM_O_SKU_PACKAGE_SCENARIO.Carton_Multipack.ExtValue()
                            && t.TYPE == ENUM_O_SKU_PACKAGE_PACKTYPE.Optional.ExtValue()).OrderBy(t => SqlFunc.ToDouble(t.TON)).ToList();
                            if (item.PLANT.ToUpper().Equals("FVN"))
                            {
                                foreach (var addpnitem in addcartonpackrule)
                                {
                                    if (Convert.ToDouble(item.QTY) >= Convert.ToDouble(addpnitem.FROMN) && Convert.ToDouble(item.QTY) <= Convert.ToDouble(addpnitem.TON))
                                        detaillist.Add(new R_PRE_WO_DETAIL
                                        {
                                            ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                            WO = item.PREWO,
                                            PARTNO = addpnitem.PARTNO,
                                            REQUESTQTY = Math.Round(1 / Convert.ToDouble(addpnitem.TON), 3).ToString(),
                                            PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                            CREATETIME = createtime
                                        });
                                }
                                if (addcartonpackrule.Count > 0 && Convert.ToDouble(addcartonpackrule.FirstOrDefault().TON) < Convert.ToDouble(item.QTY))
                                    detaillist.Add(new R_PRE_WO_DETAIL
                                    {
                                        ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                        WO = item.PREWO,
                                        PARTNO = addcartonpackrule.LastOrDefault().PARTNO,
                                        REQUESTQTY = Math.Round(1 / Convert.ToDouble(addcartonpackrule.LastOrDefault().TON), 3).ToString(),
                                        PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                        CREATETIME = createtime
                                    });
                            }
                            else
                            {
                                if (addcartonpackrule.Count > 0)
                                {

                                    //1ST 計算工單數量是否在最低跟最高區間
                                    //2ND 計算需要可以完全符合最高值的數量
                                    //3RD 計算餘數落在最高及最低區間的數量
                                    //注意配的包材各區間的值必須是連續的

                                    if (Convert.ToDouble(item.QTY) > Convert.ToDouble(addcartonpackrule.LastOrDefault().TON))
                                    {
                                        #region 工單數量大於最高區間
                                        //1.最高包材料號及其數量                                       
                                        O_SKU_PACKAGE maxPartno = addcartonpackrule.LastOrDefault();
                                        int maxPartnoQty = Convert.ToInt32(Convert.ToDouble(item.QTY)) / Convert.ToInt32(Convert.ToDouble(maxPartno.TON));

                                        //2.是否有剩餘,找出剩餘數量對應的包材
                                        int remainder = Convert.ToInt32(Convert.ToDouble(item.QTY)) % Convert.ToInt32(Convert.ToDouble(addcartonpackrule.LastOrDefault().TON));
                                        O_SKU_PACKAGE remainderPartno = null;
                                        if (remainder != 0)
                                        {
                                            foreach (var cPartno in addcartonpackrule)
                                            {
                                                if (remainder >= Convert.ToDouble(cPartno.FROMN) && remainder <= Convert.ToDouble(cPartno.TON))
                                                {
                                                    remainderPartno = cPartno;
                                                    break;
                                                }
                                            }
                                            if (remainderPartno.PARTNO.Equals(maxPartno.PARTNO))
                                            {
                                                detaillist.Add(new R_PRE_WO_DETAIL
                                                {
                                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                    WO = item.PREWO,
                                                    PARTNO = maxPartno.PARTNO,
                                                    //REQUESTQTY = Math.Round((maxPartnoQty + 1) / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    //應康保要求不要四捨五入，直接向上進1
                                                    REQUESTQTY = RoundUp((maxPartnoQty + 1) / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                    CREATETIME = createtime
                                                });
                                            }
                                            else
                                            {
                                                detaillist.Add(new R_PRE_WO_DETAIL
                                                {
                                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                    WO = item.PREWO,
                                                    PARTNO = maxPartno.PARTNO,
                                                    //REQUESTQTY = Math.Round(maxPartnoQty / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    //應康保要求不要四捨五入，直接向上進1
                                                    REQUESTQTY = RoundUp(maxPartnoQty / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                    CREATETIME = createtime
                                                });
                                                detaillist.Add(new R_PRE_WO_DETAIL
                                                {
                                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                    WO = item.PREWO,
                                                    PARTNO = remainderPartno.PARTNO,
                                                    //REQUESTQTY = Math.Round(1 / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    //應康保要求不要四捨五入，直接向上進1
                                                    REQUESTQTY = RoundUp(1 / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                    CREATETIME = createtime
                                                });
                                            }
                                        }
                                        else
                                        {
                                            detaillist.Add(new R_PRE_WO_DETAIL
                                            {
                                                ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                WO = item.PREWO,
                                                PARTNO = maxPartno.PARTNO,
                                                //REQUESTQTY = Math.Round(maxPartnoQty / Convert.ToDouble(item.QTY), 3).ToString(),
                                                //應康保要求不要四捨五入，直接向上進1
                                                REQUESTQTY = RoundUp(maxPartnoQty / Convert.ToDouble(item.QTY), 3).ToString(),
                                                PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                CREATETIME = createtime
                                            });
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 工單數量在設置的區間内
                                        foreach (var cPartno in addcartonpackrule)
                                        {
                                            if (Convert.ToDouble(item.QTY) >= Convert.ToDouble(cPartno.FROMN) && Convert.ToDouble(item.QTY) <= Convert.ToDouble(cPartno.TON))
                                                detaillist.Add(new R_PRE_WO_DETAIL
                                                {
                                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                    WO = item.PREWO,
                                                    PARTNO = cPartno.PARTNO,
                                                    //REQUESTQTY = Math.Round(1 / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    //應康保要求不要四捨五入，直接向上進1
                                                    REQUESTQTY = RoundUp(1 / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                    CREATETIME = createtime
                                                });
                                        }
                                        #endregion
                                    }

                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region part1
                            var compackrule = skupageobj.FindAll(t => t.SCENARIO == ENUM_O_SKU_PACKAGE_SCENARIO.Overpack.ExtValue() && t.TYPE == ENUM_O_SKU_PACKAGE_PACKTYPE.Compulsory.ExtValue()).ToList();
                            if (item.PLANT.ToUpper().Equals("FVN"))
                            {
                                foreach (var addpnitem in compackrule)
                                    detaillist.Add(new R_PRE_WO_DETAIL
                                    {
                                        ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                        WO = item.PREWO,
                                        PARTNO = addpnitem.PARTNO,
                                        REQUESTQTY = Convert.ToDouble(addpnitem.USAGE).ToString(),
                                        PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                        CREATETIME = createtime
                                    });
                            }
                            else
                            {
                                if (item.OFFERINGTYPE.ToUpper().Equals("Fixed Nonstockable Bundle".ToUpper()) || (ordedetailinfo.i.CARTONLABEL2 != null && ordedetailinfo.i.CARTONLABEL2.ToUpper().Equals("Bulk".ToUpper())))
                                {
                                    foreach (var addpnitem in compackrule)
                                        detaillist.Add(new R_PRE_WO_DETAIL
                                        {
                                            ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                            WO = item.PREWO,
                                            PARTNO = addpnitem.PARTNO,
                                            //REQUESTQTY = addpnitem.USAGE,
                                            REQUESTQTY = RoundUp((RoundUp(Convert.ToDouble(item.QTY) / 50, 0) * Convert.ToDouble(addpnitem.USAGE) / Convert.ToDouble(item.QTY)), 3).ToString(),
                                            PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                            CREATETIME = createtime
                                        });
                                }
                                else
                                {
                                    foreach (var addpnitem in compackrule)
                                        detaillist.Add(new R_PRE_WO_DETAIL
                                        {
                                            ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                            WO = item.PREWO,
                                            PARTNO = addpnitem.PARTNO,
                                            REQUESTQTY = Convert.ToDouble(addpnitem.USAGE).ToString(),
                                            PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                            CREATETIME = createtime
                                        });
                                }
                            }
                            #endregion
                            #region part2
                            var addcartonpackrule = skupageobj.FindAll(t => t.SCENARIO == ENUM_O_SKU_PACKAGE_SCENARIO.Carton_Overpack.ExtValue()
                                && t.TYPE == ENUM_O_SKU_PACKAGE_PACKTYPE.Optional.ExtValue()).OrderBy(t => SqlFunc.ToDouble(t.TON)).ToList();
                            if (item.PLANT.ToUpper().Equals("FVN"))
                            {
                                foreach (var addpnitem in addcartonpackrule)
                                {
                                    if (Convert.ToDouble(item.QTY) >= Convert.ToDouble(addpnitem.FROMN) && Convert.ToDouble(item.QTY) <= Convert.ToDouble(addpnitem.TON))
                                        detaillist.Add(new R_PRE_WO_DETAIL
                                        {
                                            ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                            WO = item.PREWO,
                                            PARTNO = addpnitem.PARTNO,
                                            REQUESTQTY = Math.Round(1 / Convert.ToDouble(addpnitem.TON), 3).ToString(),
                                            PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                            CREATETIME = createtime
                                        });
                                }
                                if (addcartonpackrule.Count > 0 && Convert.ToDouble(addcartonpackrule.LastOrDefault().TON) < Convert.ToDouble(item.QTY))
                                    detaillist.Add(new R_PRE_WO_DETAIL
                                    {
                                        ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                        WO = item.PREWO,
                                        PARTNO = addcartonpackrule.LastOrDefault().PARTNO,
                                        REQUESTQTY = Math.Round(1 / Convert.ToDouble(addcartonpackrule.LastOrDefault().TON), 3).ToString(),
                                        PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                        CREATETIME = createtime
                                    });
                            }
                            else
                            {
                                if (addcartonpackrule.Count > 0)
                                {
                                    //1ST 計算工單數量是否在最低跟最高區間
                                    //2ND 計算需要可以完全符合最高值的數量
                                    //3RD 計算餘數落在最高及最低區間的數量
                                    //注意配的包材各區間的值必須是連續的

                                    if (Convert.ToDouble(item.QTY) > Convert.ToDouble(addcartonpackrule.LastOrDefault().TON))
                                    {
                                        #region 工單數量大於最高區間
                                        //1.最高包材料號及其數量                                       
                                        O_SKU_PACKAGE maxPartno = addcartonpackrule.LastOrDefault();
                                        int maxPartnoQty = Convert.ToInt32(Convert.ToDouble(item.QTY)) / Convert.ToInt32(Convert.ToDouble(maxPartno.TON));

                                        //2.是否有剩餘,找出剩餘數量對應的包材
                                        int remainder = Convert.ToInt32(Convert.ToDouble(item.QTY)) % Convert.ToInt32(Convert.ToDouble(addcartonpackrule.LastOrDefault().TON));
                                        O_SKU_PACKAGE remainderPartno = null;
                                        if (remainder != 0)
                                        {
                                            foreach (var cPartno in addcartonpackrule)
                                            {
                                                if (remainder >= Convert.ToDouble(cPartno.FROMN) && remainder <= Convert.ToDouble(cPartno.TON))
                                                {
                                                    remainderPartno = cPartno;
                                                    break;
                                                }
                                            }
                                            if (remainderPartno.PARTNO.Equals(maxPartno.PARTNO))
                                            {
                                                detaillist.Add(new R_PRE_WO_DETAIL
                                                {
                                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                    WO = item.PREWO,
                                                    PARTNO = maxPartno.PARTNO,
                                                    //REQUESTQTY = Math.Round((maxPartnoQty + 1) / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    //應康保要求不要四捨五入，直接向上進1
                                                    REQUESTQTY = RoundUp((maxPartnoQty + 1) / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                    CREATETIME = createtime
                                                });
                                            }
                                            else
                                            {
                                                detaillist.Add(new R_PRE_WO_DETAIL
                                                {
                                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                    WO = item.PREWO,
                                                    PARTNO = maxPartno.PARTNO,
                                                    //REQUESTQTY = Math.Round(maxPartnoQty / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    //應康保要求不要四捨五入，直接向上進1
                                                    REQUESTQTY = RoundUp(maxPartnoQty / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                    CREATETIME = createtime
                                                });
                                                detaillist.Add(new R_PRE_WO_DETAIL
                                                {
                                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                    WO = item.PREWO,
                                                    PARTNO = remainderPartno.PARTNO,
                                                    REQUESTQTY = Math.Round(1 / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                    CREATETIME = createtime
                                                });
                                            }
                                        }
                                        else
                                        {
                                            detaillist.Add(new R_PRE_WO_DETAIL
                                            {
                                                ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                WO = item.PREWO,
                                                PARTNO = maxPartno.PARTNO,
                                                //REQUESTQTY = Math.Round(maxPartnoQty / Convert.ToDouble(item.QTY), 3).ToString(),
                                                //應康保要求不要四捨五入，直接向上進1
                                                REQUESTQTY = RoundUp(maxPartnoQty / Convert.ToDouble(item.QTY), 3).ToString(),
                                                PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                CREATETIME = createtime
                                            });
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 工單數量在設置的區間内
                                        foreach (var cPartno in addcartonpackrule)
                                        {
                                            if (Convert.ToDouble(item.QTY) >= Convert.ToDouble(cPartno.FROMN) && Convert.ToDouble(item.QTY) <= Convert.ToDouble(cPartno.TON))
                                                detaillist.Add(new R_PRE_WO_DETAIL
                                                {
                                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                    WO = item.PREWO,
                                                    PARTNO = cPartno.PARTNO,
                                                    //REQUESTQTY = Math.Round(1 / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    //應康保要求不要四捨五入，直接向上進1
                                                    REQUESTQTY = RoundUp(1 / Convert.ToDouble(item.QTY), 3).ToString(),
                                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                    CREATETIME = createtime
                                                });
                                        }
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion


                        #region OTHER  -> 目前只有WHSPackage

                        if (Convert.ToDouble(item.QTY) > 160)
                        {
                            whsPackageObj = db.Queryable<O_WHS_PACKAGE>().Where(t => t.SKUNO == item.PID && t.SCENARIO == ENUM_O_WHS_PACKAGE_SCENARIO.WhsPack.ExtValue()).ToList();
                        }
                        else
                        {
                            whsPackageObj = db.Queryable<O_WHS_PACKAGE>().Where(t => t.SKUNO == item.PID && t.SCENARIO == ENUM_O_WHS_PACKAGE_SCENARIO.WhsPack.ExtValue() && t.TYPE == ENUM_O_WHS_PACKAGE_PACKTYPE.Compulsory.ExtValue()).ToList();
                        }

                        //暫時屏蔽

                        if (db.Queryable<C_CONTROL>().Where(t => t.CONTROL_NAME == "OM_INTERFACE_CONTROL" && t.CONTROL_TYPE == "NEED_WHSPACKAGE").Any() && whsPackageObj.Count == 0)
                        {
                            trackobj.ExceptionProcess(true, $@"PID {item.PID} need PE to configure WHS_PACKAGE");
                            continue;
                        }
                        foreach (var addwhsPackage in whsPackageObj)
                        {
                            detaillist.Add(new R_PRE_WO_DETAIL
                            {
                                ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                WO = item.PREWO,
                                PARTNO = addwhsPackage.PARTNO,
                                REQUESTQTY = addwhsPackage.USAGE,
                                PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.OTHER.ExtValue(),
                                CREATETIME = createtime
                            });
                        }
                        #endregion
                        var pidagile = db.Queryable<O_AGILE_ATTR, O_SKU_CONFIG>((a, s) => a.OFFERING_TYPE == s.OFFERINGTYPE)
                                    .Where((a, s) => a.ITEM_NUMBER == item.PID && a.ACTIVED == MesBool.Yes.ExtValue()).Select((a, s) => s).ToList().FirstOrDefault();
                        if (pidagile == null)
                        {
                            trackobj.ExceptionProcess(true, $@"PID {item.PID} miss agile data or not config O_SKU_CONFIG!");
                            continue;
                        }
                        #region BASE
                        //Premium Configurable Sys 的BASE不參與
                        if (pidagile.BOMEXPLOSION != MesBool.No.ExtName())
                            detaillist.Add(new R_PRE_WO_DETAIL
                            {
                                ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                WO = item.PREWO,
                                PARTNO = item.PID,
                                REQUESTQTY = "1",
                                PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.BASE.ExtValue(),
                                CREATETIME = createtime
                            });
                        #endregion
                        #region custcompoments 已经開啓
                        var ctooptions = db.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == item.ID && t.OPTIONTYPE == ENUM_O_ORDER_OPTION.CTO.ExtValue()).ToList();
                        var igonepn = db.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "JNPPN_IGNORE" && t.FUNCTIONTYPE == "NOSYSTEM").ToList();
                        try
                        {
                            foreach (var oitem in ctooptions)
                            {
                                var oitemagile = db.Queryable<O_AGILE_ATTR, O_SKU_CONFIG>((a, s) => a.OFFERING_TYPE == s.OFFERINGTYPE)
                                    .Where((a, s) => a.CUSTPARTNO == oitem.PARTNO && a.ACTIVED == MesBool.Yes.ExtValue()).Select((a, s) => s).ToList().FirstOrDefault();
                                if (oitemagile == null)
                                    throw new Exception($@"components: {oitem.PARTNO} miss agile data or not config O_SKU_CONFIG!");
                                if (oitemagile.BOMEXPLOSION == MesBool.No.ExtName())
                                    continue;
                                if (igonepn.Any(t => t.VALUE == oitem.PARTNO))
                                    continue;
                                detaillist.Add(new R_PRE_WO_DETAIL
                                {
                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                    WO = item.PREWO,
                                    PARTNO = oitem.FOXPN,
                                    REQUESTQTY = oitem.QTY.ToString(),
                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.COMPONENTS.ExtValue(),
                                    CREATETIME = createtime
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            trackobj.ExceptionProcess(true, e.Message);
                            continue;
                        }
                        #endregion

                        #region duplic pn
                        var adddetails = new List<R_PRE_WO_DETAIL>();
                        foreach (var ditem in detaillist)
                        {
                            var existspn = adddetails.FindAll(t => t.PARTNO == ditem.PARTNO.Trim()).FirstOrDefault();
                            ditem.PARTNO = ditem.PARTNO.Trim();
                            if (existspn != null)
                                existspn.REQUESTQTY = (Convert.ToDouble(existspn.REQUESTQTY) + Convert.ToDouble(ditem.REQUESTQTY)).ToString();
                            else
                                adddetails.Add(ditem);
                        }
                        #endregion

                        #region Black Pn manager
                        if (db.Queryable<R_F_CONTROL>().Any(t => t.FUNCTIONNAME == "DefaulSite" && t.CATEGORY == "SiteName" && t.VALUE == "FJZ"))
                        {
                            var cpn = adddetails.FindAll(t => "BASE,COMPONENTS".Split(',').Contains(t.PARTNOTYPE)).ToList();
                            var cpartno = cpn.Select(t => t.PARTNO).Distinct().ToList();

                            var currentblackconfig = (from c in cpn
                                                      join b in blackpns on c.PARTNO equals b.PARENTPN
                                                      select new { c.PARTNO, c.PARTNOTYPE, c.REQUESTQTY, b.BASETYPE, b.SLOTTYPE, b.QTY, b.BLACKPN }).ToList();
                            var factrequest = currentblackconfig.GroupBy(x => new { x.BASETYPE }).Select(y => new R_JNP_FULLMATCH { BASETYPE = y.Key.BASETYPE.ToString(), QTY = y.Sum(x => double.Parse(x.REQUESTQTY)).ToString() }).ToList();
                            if (cpn.FindAll(t => t.PARTNOTYPE == "COMPONENTS").Any())
                                (from t in (currentblackconfig.FindAll(p => p.SLOTTYPE != null && p.SLOTTYPE.ToUpper() != "NA" && p.BLACKPN.ToUpper() != "NA"))
                                 group t by new { t.SLOTTYPE, t.BLACKPN, t.QTY, t.REQUESTQTY } into g
                                 select new { SLOTTYPE = g.Key.SLOTTYPE, BLACKPN = g.Key.BLACKPN, REQUESTQTY = g.Key.REQUESTQTY, QTY = g.Sum(t => double.Parse(t.QTY)).ToString() }).ToList().ForEach(w =>
                                 {
                                     double blackpnqty = double.Parse(w.QTY); var blackpn = w.BLACKPN;
                                 //var tconfig = currentblackconfig.FindAll(ct => ct.BASETYPE == w.SLOTTYPE).ToList();
                                 var tconfig = factrequest.FindAll(ct => ct.BASETYPE == w.SLOTTYPE).FirstOrDefault();
                                     if (tconfig != null)
                                     {
                                         blackpnqty = blackpnqty * double.Parse(w.REQUESTQTY) - double.Parse(tconfig.QTY);
                                         if (blackpnqty > 0)
                                         {
                                             tconfig.QTY = "0";
                                             adddetails.Add(new R_PRE_WO_DETAIL
                                             {
                                                 ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                 WO = item.PREWO,
                                                 PARTNO = blackpn,
                                                 REQUESTQTY = blackpnqty.ToString(),
                                                 PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                 CREATETIME = createtime
                                             });
                                         }
                                         else
                                         {
                                             tconfig.QTY = (double.Parse(tconfig.QTY) - blackpnqty).ToString();
                                         }
                                     }
                                     else
                                     {
                                         if (blackpnqty > 0)
                                         {
                                             adddetails.Add(new R_PRE_WO_DETAIL
                                             {
                                                 ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                                 WO = item.PREWO,
                                                 PARTNO = blackpn,
                                                 REQUESTQTY = (blackpnqty * double.Parse(w.REQUESTQTY)).ToString(),
                                                 PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                                 CREATETIME = createtime
                                             });
                                         }
                                     }

                                 });
                            else if (currentblackconfig.FindAll(t=>t.BLACKPN!="NA").Count > 0)
                            {

                                adddetails.Add(new R_PRE_WO_DETAIL
                                {
                                    ID = MesDbBase.GetNewID<R_PRE_WO_DETAIL>(db, bu),
                                    WO = item.PREWO,
                                    PARTNO = currentblackconfig.FirstOrDefault().BLACKPN,
                                    REQUESTQTY = (double.Parse(currentblackconfig.FirstOrDefault().QTY) * double.Parse(currentblackconfig.FirstOrDefault().REQUESTQTY)).ToString(),
                                    PARTNOTYPE = ENUM_R_PRE_WO_DETAIL.PACKAGE.ExtValue(),
                                    CREATETIME = createtime
                                });
                            }
                        }

                        #region duplic pn
                        var adddetails_blank = new List<R_PRE_WO_DETAIL>();// MESDataObject.Common.Extensions.Clone(adddetails);
                        foreach (var ditem in adddetails)
                        {
                            var existspn = adddetails_blank.FindAll(t => t.PARTNO == ditem.PARTNO.Trim()).FirstOrDefault();
                            ditem.PARTNO = ditem.PARTNO.Trim();
                            if (existspn != null)
                                existspn.REQUESTQTY = (Convert.ToDouble(existspn.REQUESTQTY) + Convert.ToDouble(ditem.REQUESTQTY)).ToString();
                            else
                                adddetails_blank.Add(ditem);
                        }
                        adddetails = adddetails_blank;
                        #endregion

                        #endregion

                        //#region duplic pn
                        //var adddetails = new List<R_PRE_WO_DETAIL>();
                        //foreach (var ditem in detaillist)
                        //{
                        //    var existspn = adddetails.FindAll(t => t.PARTNO == ditem.PARTNO.Trim()).FirstOrDefault();
                        //    ditem.PARTNO = ditem.PARTNO.Trim();
                        //    if (existspn != null)
                        //        existspn.REQUESTQTY = (Convert.ToDouble(existspn.REQUESTQTY) + Convert.ToDouble(ditem.REQUESTQTY)).ToString();
                        //    else
                        //        adddetails.Add(ditem);
                        //}
                        //#endregion

                        #region ReplacePn
                        var replacepns = db.Queryable<O_ORDER_MAIN, R_JNP_REPLACEPN>((m, r) => m.UPOID == r.UPOID).Where((m, r) => m.ID == item.ID && r.VALIDFLAG == MesBool.Yes.ExtValue())
                            .Select((m, r) => r).ToList();
                        foreach (var replacePnItem in replacepns)
                        {
                            var cr = adddetails.FindAll(t => t.PARTNO == replacePnItem.PARTNO.Trim()).ToList().FirstOrDefault();
                            if (cr != null)
                                cr.PARTNO = replacePnItem.REPLACEPN.Trim();
                        }
                        #endregion


                        //var fullfilename = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\Aruba\\";
                        //var filename = $@"{item.PREWO}.csv";
                        //MESPubLab.Common.ExcelHelp.ExportCsv(adddetails, $@"{fullfilename}{filename}");
                        //continue;

                        #region head
                        var res = db.Ado.UseTran(() =>
                        {
                            db.Insertable(new R_PRE_WO_HEAD
                            {
                                ID = MesDbBase.GetNewID<R_PRE_WO_HEAD>(db, bu),
                                WO = item.PREWO,
                                WOQTY = item.QTY.ToString(),
                                PONO = item.PONO,
                                POLINE = item.POLINE,
                                PID = item.PID,
                                TOTUNITPRICE = item.UNITPRICE,
                                MAINID = item.ID,
                                SAPFLAG = ENUM_R_PRE_WO_HEAD.CREATEWO_NO.ExtValue(),
                                DESCRIPTION = podetail.FirstOrDefault() != null ? podetail.FirstOrDefault().PPARTDESC : string.Empty,
                                CREATETIME = createtime,
                                PLANT = new Func<string>(() =>
                                {
                                    var skunewplant = db.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.PID).ToList().FirstOrDefault();
                                    return skunewplant != null ? skunewplant.PLANTCODE : plantCode;
                                })(),
                                CUSTPN = new Func<string>(() =>
                                {
                                    return db.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == item.PID).OrderBy(t => t.RELEASE_DATE, OrderByType.Desc).ToList().FirstOrDefault().CUSTPARTNO;
                                })()
                            }).ExecuteCommand();
                            db.Insertable(adddetails).ExecuteCommand();
                        });
                        #endregion
                        if (trackobj.ExceptionProcess(!res.IsSuccess, res.ErrorMessage))
                            continue;
                        #region PO status
                        trackobj.ReleasePoStatus(item.ID, this);
                        #endregion
                        trackobj.ReleaseFuncExcption();
                    }
                    catch (Exception e)
                    {
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }
    }

    public class JuniperCreateWo : FunctionBase
    {
        JuniperErrType functiontype = JuniperErrType.I137;
        JuniperSubType subfunctiontype = JuniperSubType.CreateWo;
        public string plantCode;
        public string RESED;
        public JuniperCreateWo(string _dbstr, string _bu, string _plantCode, object taskbase) : base(_dbstr, _bu)
        {
            this.TaskBase = taskbase;
            this.plantCode = _plantCode;
            SetFunctionObj(ENUM_O_PO_STATUS.CreateWo); servicesName = ServicesEnum.JnpServices;
            RESED = ConfigGet("RESED");
            if (RESED.ToUpper() == "TRUE")
            {
                //RESED = "X";//auto rel wo
                RESED = "A";//写死不rel
            }
            else
            {
                RESED = "A";
            }
        }

        /// <summary>
        /// 更换开工单方式
        /// </summary>
        public override void FunctionRun()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                #region 取待處理的Po列表;
                var waitHandleOrder = db.Queryable<O_ORDER_MAIN, O_PO_STATUS, I137_I>((m, s, i) => m.ID == s.POID && m.ITEMID == i.ID)
                    .Where((m, s, i) => s.STATUSID == ((ENUM_O_PO_STATUS)GetFunctionObj()).ExtValue() && s.VALIDFLAG == MesBool.Yes.ExtValue())
                    .OrderBy((m, s, i) => m.EDITTIME, OrderByType.Desc).Select((m, s, i) => new { m, i }).ToList();
                var wotypelist = db.Queryable<O_J_WOTYPE>().ToList();
                var rfc_cto = new ZCPP_NSBG_0302(this.bu);
                var rfc_bts = new ZCPP_NSBG_0303(this.bu);
                #endregion
                foreach (var item in waitHandleOrder)
                {
                    JuniperPoTracking trackobj = new JuniperPoTracking(functiontype, subfunctiontype, item.m.PONO, item.m.POLINE, item.i.TRANID, db);
                    try
                    {
                        //#region RMQ 不開工單;=>Update:RMQ需要開工單--add Eden by 2021/06/16
                        if (db.Queryable<O_I137_HEAD, O_I137_ITEM>((h, i) => h.TRANID == i.TRANID).Where((h, i) => i.ID == item.m.ITEMID && h.PODOCTYPE.Equals(ENUM_I137_PoDocType.ZRMQ.ToString())).Select((h, i) => i).Any())
                        {
                            //#region PO status
                            //trackobj.ReleasePoStatus(item.m.ID, ENUM_O_PO_STATUS.RmqEnd, this);
                            //#endregion
                            //continue;
                        }
                        //#endregion

                        #region sap job
                        var job = db.Queryable<R_SAP_JOB>().Where(t => t.JOBNAME == ENUM_R_SAP_JOB_FUNCTION.CreateWoJob.ExtValue() && t.DATA2 == item.m.PREWO && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Runing.ExtValue()).ToList().FirstOrDefault();
                        if (job != null && (Convert.ToInt32(job.RUNTIME) > 1 && Convert.ToDateTime(job.LASTEDITTIME).AddHours(8) > DateTime.Now))
                            continue;
                        if (job == null)
                        {
                            job = new R_SAP_JOB()
                            {
                                ID = MesDbBase.GetNewID<R_SAP_JOB>(db, Customer.JUNIPER.ExtValue()),
                                JOBKEY = item.m.UPOID,
                                JOBNAME = ENUM_R_SAP_JOB_FUNCTION.CreateWoJob.ExtValue(),
                                JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Runing.ExtValue(),
                                RUNTIME = "1",
                                CREATETIME = DateTime.Now,
                                LASTEDITTIME = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.Normal.ExtValue()),
                                DATA1 = item.m.PLANT,
                                DATA2 = item.m.PREWO
                            };
                            db.Insertable(job).ExecuteCommand();
                        }
                        else
                        {
                            job.RUNTIME = (Convert.ToInt32(job.RUNTIME) + 1).ToString();
                            job.LASTEDITTIME = DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.Normal.ExtValue());
                            db.Updateable(job).ExecuteCommand(); 
                        }
                        #endregion

                        //#region IP/CSIP/SIP 不開工單; ->即將改爲用戶要求開工單但是不能CONVER;
                        //if (trackobj.ExceptionProcess("IP/CSIP/SIP".Split('/').Contains(item.i.LINESCHEDULINGSTATUS),
                        //    $@" {item.m.PREWO } LineSchedulingStatus is {item.i.LINESCHEDULINGSTATUS}!"))
                        //    continue;
                        //#endregion
                        //#region hold
                        //var holdobj = JuniperOmBase.JuniperHoldCheck(item.m.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.CREATEWO, db);
                        //if (trackobj.ExceptionProcess(holdobj.HoldFlag, $@" {item.m.PREWO } Order is OnHold ,ReasonCode is {holdobj.HoldReason}!"))
                        //    continue;
                        //#endregion

                        var prehead = db.Queryable<R_PRE_WO_HEAD>().Where(t => t.WO == item.m.PREWO).ToList().FirstOrDefault();
                        //var predetail = db.Queryable<R_PRE_WO_DETAIL>().Where(t => t.WO == item.m.PREWO && t.PARTNOTYPE!=ENUM_R_PRE_WO_DETAIL.SAPEX.ExtValue()).ToList();
                        var predetail = db.Queryable<R_PRE_WO_DETAIL>().Where(t => t.WO == item.m.PREWO && t.PARTNOTYPE != ENUM_R_PRE_WO_DETAIL.COMPONENTS.ExtValue()).ToList();
                        if (prehead == null)
                            continue;
                        var skunewplant = db.Queryable<R_SKU_PLANT>().Where(t => t.FOXCONN == item.m.PID).ToList().FirstOrDefault();
                        var cplant = string.Empty;
                        cplant = skunewplant != null ? skunewplant.PLANTCODE : plantCode;
                        var wotype = wotypelist.FindAll(t => t.WOPRE == item.m.PREWO.Substring(0, 4)).ToList().FirstOrDefault();
                        if (wotype == null)
                            continue;
                        //by wo pre 
                        var sapcreatwotype = JuniperSapCreateWoType.BTS.ExtValue().IndexOf(wotype.WOTYPE.ToString()) > -1 ? JuniperSapCreateWoType.BTS : JuniperSapCreateWoType.CTO;
                        trackobj.ReleaseFuncExcption();

                        DataTable ZWO_HEADER = rfc_bts.GET_NEW_ZWO_HEADER();
                        DataRow hdr = ZWO_HEADER.NewRow();
                        hdr["WO"] = prehead.WO;
                        hdr["WOTYPE"] = wotype.WOTYPE;
                        hdr["PLANT"] = cplant;
                        hdr["QTY"] = prehead.WOQTY;
                        hdr["PO"] = item.m.PONO;
                        hdr["PID"] = prehead.GROUPID;
                        hdr["GSTRP"] = new Func<string>(() =>
                        {
                            var chinaZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                            // 机器本地时间 -> 中国时间
                            var chinaTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, chinaZone);
                            //var startdate = (Convert.ToDateTime(item.i.CUSTREQSHIPDATE).Date - chinaTime.Date).Days <= 7 ? chinaTime.AddDays(1) : Convert.ToDateTime(item.i.CUSTREQSHIPDATE);
                            //var startdate = Convert.ToDateTime(item.i.CUSTREQSHIPDATE).AddDays(-7);//Donald hui 要求先去掉，不減七天了 //Donald hui要求FVN、FJZ都改為CRSD減7天
                            //var nowtime = Convert.ToDateTime(db.GetDate().ToString("yyyy-MM-dd 00:00:00"));
                            var startdate = (Convert.ToDateTime(item.i.CUSTREQSHIPDATE).Date - chinaTime.Date).Days <= 7 ? chinaTime.AddDays(1) : Convert.ToDateTime(item.i.CUSTREQSHIPDATE).AddDays(-7); //時間不能傳過去的時間，SAP會回only WO scheudling type is 3 that startdate can in past
                            ////SAP日期不能超過當年
                            //if (startdate.Year > DateTime.Now.Year)
                            //    return $@"{DateTime.Now.Year}1231";
                            return startdate.ToString("yyyyMMdd");
                        })();
                        ZWO_HEADER.Rows.Add(hdr);
                        rfc_bts.SetValue(RESED, ZWO_HEADER);
                        //LOG 太多，暫時不計
                        //rfc_bts.CallRFC(() => { MesSapHelp.SapLog(prehead.ID, rfc_bts.getSapParameobj(), db); });
                        rfc_bts.CallRFC();
                        DataTable RERURN = rfc_bts.GetTableValue("RETURN");
                        if (RERURN.Rows.Count > 0 && (RERURN.Rows[0]["MESSAGE"].ToString().ToUpper().IndexOf("SAVE") > -1 || RERURN.Rows[0]["MESSAGE"].ToString().ToUpper().IndexOf("已儲存") > -1 || RERURN.Rows[0]["MESSAGE"].ToString().ToUpper().IndexOf("已存储") > -1))
                        {
                            #region PO status  RMQ 不開工單;=>Update:RMQ需要開工單--add Eden by 2021/06/16                       
                            if (item.m.ORDERTYPE.Equals(ENUM_I137_PoDocType.ZRMQ.ToString()))
                                trackobj.ReleasePoStatus(item.m.ID, ENUM_O_PO_STATUS.RmqEnd, this);
                            else
                                trackobj.ReleasePoStatus(item.m.ID, this);
                            job.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Finish.ExtValue();
                            db.Updateable(job).ExecuteCommand();
                            #endregion
                        }
                        else
                        {
                            trackobj.ExceptionProcess(true, RERURN.Rows.Count == 0 ? $@" CreateWo Fail!sap err is black! " : $@"{prehead.GROUPID},{prehead.WO} : {RERURN.Rows[0]["MESSAGE"].ToString()}");
                            continue;
                        }
                        trackobj.ReleaseFuncExcption();
                    }
                    catch (Exception e)
                    {
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }


        enum JuniperSapCreateWoType
        {
            /// <summary>
            /// EnumValue("ZJ10")
            /// EnumName("CTO")
            /// </summary>
            [EnumValue("ZJ10")]
            [EnumName("BTS")]
            BTS,
            /// <summary>
            /// EnumValue("ZJ10,ZJ12")
            /// EnumName("CTO")
            /// </summary>
            [EnumValue("ZJ09,ZJ11,ZJ13,ZVJ1,ZJ12,ZMBD")]
            [EnumName("CTO")]
            CTO
        }
    }

    public class JuniperGroupIdReceive : FunctionBase
    {
        JuniperErrType functiontype = JuniperErrType.I137;
        JuniperSubType subfunctiontype = JuniperSubType.GroupIdReceive;
        public JuniperGroupIdReceive(string _dbstr, string _bu) : base(_dbstr, _bu) { SetFunctionObj(ENUM_O_PO_STATUS.ReceiveGroupId); servicesName = ServicesEnum.JnpServices; }

        /// <summary>
        /// </summary>
        public override void FunctionRun()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                #region 取待處理的Po列表;
                var waitHandleOrder = db.Queryable<O_ORDER_MAIN, O_PO_STATUS, R_WO_GROUPID, I137_I>((m, s, g, i) => m.ID == s.POID && m.PREWO == g.WO && m.ITEMID == i.ID)
                    .Where((m, s, g, i) => s.STATUSID == ((ENUM_O_PO_STATUS)GetFunctionObj()).ExtValue() && s.VALIDFLAG == MesBool.Yes.ExtValue()).OrderBy((m, s, g, i) => m.EDITTIME, OrderByType.Desc).Select((m, s, g, i) => new { m, g, i }).ToList();
                #endregion
                var groupidlist = waitHandleOrder;
                foreach (var item in waitHandleOrder)
                {
                    JuniperPoTracking trackobj = new JuniperPoTracking(functiontype, subfunctiontype, item.m.PONO, item.m.POLINE, item.i.TRANID, db);
                    try
                    {
                        var prehead = db.Queryable<R_PRE_WO_HEAD>().Where(t => t.WO == item.m.PREWO).ToList().FirstOrDefault();
                        if (prehead != null)
                        {
                            prehead.GROUPID = item.g.GROUPID;
                            db.Updateable(prehead).ExecuteCommand();
                            #region PO status
                            var res = db.Ado.UseTran(() =>
                            {
                                #region PO status
                                trackobj.ReleasePoStatus(item.m.ID, this);
                                #endregion      
                            });
                            if (trackobj.ExceptionProcess(!res.IsSuccess, $@"systemerr: {res.ErrorMessage}"))
                                continue;
                            #endregion
                        }
                        else
                        {
                            trackobj.ExceptionProcess(true, $@"PreHead Data is null,pls check!");
                            continue;
                        }
                        trackobj.ReleaseFuncExcption();
                    }
                    catch (Exception e)
                    {
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }
    }

    public class JuniperSapJob : FunctionBase
    {
        public JuniperSapJob(string _dbstr, string _bu) : base(_dbstr, _bu) { }

        /// <summary>
        /// </summary>
        public override void FunctionRun()
        {
            TecoSapWoWithCancelJob();
            TecoSapWoWithChangeJob();
            ChangeCrsdJob();
        }

        void TecoSapWoWithCancelJob()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                var targets = db.Queryable<R_SAP_JOB>().Where(t => t.JOBNAME == ENUM_R_SAP_JOB_FUNCTION.TecoSapWoWithCancel.ExtValue() && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue()).OrderBy(t => t.CREATETIME, OrderByType.Asc).ToList();
                foreach (var item in targets)
                {
                    var obj = db.Queryable<R_SAP_JOB>().Where(t => t.ID == item.ID && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue()).ToList().FirstOrDefault();
                    if (obj == null)
                        continue;
                    try
                    {
                        if (!string.IsNullOrEmpty(obj.RUNTIME) && !string.IsNullOrEmpty(obj.LASTEDITTIME) && int.Parse(obj.RUNTIME) > 5 && Convert.ToDateTime(obj.LASTEDITTIME).AddHours(3) > System.DateTime.Now)
                            continue;
                    }
                    catch { }
                    item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Runing.ExtValue();
                    db.Updateable(item).ExecuteCommand();
                    var ordermain = db.Queryable<O_ORDER_MAIN, O_PO_STATUS, I137_I>((m, p, i) => p.POID == m.ID && m.ITEMID == i.ID).Where((m, p, i) => m.ID == item.JOBKEY && p.VALIDFLAG == MesBool.Yes.ExtValue()).Select((m, p, i) => new { m, p, i }).ToList().FirstOrDefault();
                    if (ordermain == null || ordermain.p.STATUSID != ENUM_O_PO_STATUS.WaitDismantle.ExtValue())
                        continue;
                    JuniperPoTracking trackobj = new JuniperPoTracking(JuniperErrType.I137, JuniperSubType.TecoSapWoWithCancelJob, ordermain.i.PONUMBER, ordermain.i.ITEM, ordermain.i.TRANID, db);
                    try
                    {
                        item.RUNTIME = item.RUNTIME == null ? "1" : (int.Parse(item.RUNTIME) + 1).ToString();
                        item.LASTEDITTIME = DateTime.Now.ToString();
                        var tecores = JuniperBase.TecoSapWo(this.bu, item.DATA2);
                        if (tecores.issuccess)
                        {
                            var res = db.Ado.UseTran(() =>
                              {
                                  db.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == ordermain.m.ID).ExecuteCommand();
                                  db.Insertable(new O_PO_STATUS()
                                  {
                                      ID = MesDbBase.GetNewID<O_PO_STATUS>(db, Customer.JUNIPER.ExtValue()),
                                      STATUSID = ENUM_O_PO_STATUS.Closed.ExtValue(),
                                      VALIDFLAG = MesBool.Yes.ExtValue(),
                                      CREATETIME = DateTime.Now,
                                      EDITTIME = DateTime.Now,
                                      POID = ordermain.m.ID
                                  }).ExecuteCommand();
                                  item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Finish.ExtValue();
                                  db.Updateable(item).ExecuteCommand();
                              });
                            if (res.IsSuccess)
                                trackobj.ReleaseFuncExcption();
                            else
                                throw res.ErrorException;
                        }
                        else
                        {
                            //item.RUNTIME = item.RUNTIME == null ? "1" : (int.Parse(item.RUNTIME) + 1).ToString();
                            //if (int.Parse(item.RUNTIME) > 20)
                            //    item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Closed.ExtValue();
                            //else
                            //    item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue();
                            item.DATA6 = tecores.msg.Length > 50 ? tecores.msg.Substring(0, 50) : tecores.msg;
                            item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue();
                            db.Updateable(item).ExecuteCommand();
                            trackobj.ExceptionProcess(tecores.issuccess, tecores.msg);
                        }
                    }
                    catch (Exception e)
                    {
                        item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue();
                        db.Updateable(item).ExecuteCommand();
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }

        void TecoSapWoWithChangeJob()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                var targets = db.Queryable<R_SAP_JOB>().Where(t => t.JOBNAME == ENUM_R_SAP_JOB_FUNCTION.TecoSapWoWithChange.ExtValue() && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue()).OrderBy(t => t.CREATETIME, OrderByType.Asc).ToList();
                foreach (var item in targets)
                {
                    var obj = db.Queryable<R_SAP_JOB>().Where(t => t.ID == item.ID && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue()).ToList().FirstOrDefault();
                    if (obj == null)
                        continue;
                    try
                    {
                        if (!string.IsNullOrEmpty(obj.RUNTIME) && !string.IsNullOrEmpty(obj.LASTEDITTIME) && int.Parse(obj.RUNTIME) > 5 && Convert.ToDateTime(obj.LASTEDITTIME).AddHours(3) > System.DateTime.Now)
                            continue;
                    }
                    catch { }
                    item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Runing.ExtValue();
                    db.Updateable(item).ExecuteCommand();
                    var ordermain = db.Queryable<O_ORDER_MAIN_H, O_PO_STATUS, I137_I>((m, p, i) => p.POID == m.ID && m.ITEMID == i.ID).Where((m, p, i) => m.ID == item.JOBKEY && p.VALIDFLAG == MesBool.Yes.ExtValue()).Select((m, p, i) => new { m, p, i }).ToList().FirstOrDefault();
                    var currentmain = db.Queryable<O_ORDER_MAIN, O_PO_STATUS>((m, p) => m.ID == p.POID).Where((m, p) => m.UPOID == ordermain.m.UPOID && p.VALIDFLAG == MesBool.Yes.ExtValue()).Select((m, p) => new { m, p }).ToList().FirstOrDefault();
                    if (ordermain == null || currentmain == null || currentmain.p.STATUSID != ENUM_O_PO_STATUS.WaitDismantle.ExtValue())
                        continue;
                    JuniperPoTracking trackobj = new JuniperPoTracking(JuniperErrType.I137, JuniperSubType.TecoSapWoWithChangeJob, ordermain.i.PONUMBER, ordermain.i.ITEM, ordermain.i.TRANID, db);
                    try
                    {
                        item.RUNTIME = item.RUNTIME == null ? "1" : (int.Parse(item.RUNTIME) + 1).ToString();
                        item.LASTEDITTIME = DateTime.Now.ToString();
                        var tecores = JuniperBase.TecoSapWo(this.bu, item.DATA2);
                        if (tecores.issuccess)
                        {
                            var res = db.Ado.UseTran(() =>
                              {
                                  db.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == currentmain.m.ID).ExecuteCommand();
                                  db.Insertable(new O_PO_STATUS()
                                  {
                                      ID = MesDbBase.GetNewID<O_PO_STATUS>(db, Customer.JUNIPER.ExtValue()),
                                      STATUSID = ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue(),
                                      VALIDFLAG = MesBool.Yes.ExtValue(),
                                      CREATETIME = DateTime.Now,
                                      EDITTIME = DateTime.Now,
                                      POID = currentmain.m.ID
                                  }).ExecuteCommand();
                                  item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Finish.ExtValue();
                                  db.Updateable(item).ExecuteCommand();
                              });
                            if (res.IsSuccess)
                                trackobj.ReleaseFuncExcption();
                            else
                                throw res.ErrorException;
                        }
                        else
                        {
                            item.DATA6 = tecores.msg.Length > 50 ? tecores.msg.Substring(0, 50) : tecores.msg;
                            item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue();
                            db.Updateable(item).ExecuteCommand();
                            trackobj.ExceptionProcess(tecores.issuccess, tecores.msg);
                        }
                    }
                    catch (Exception e)
                    {
                        item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue();
                        db.Updateable(item).ExecuteCommand();
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }

        void ChangeCrsdJob()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dbstr, false))
            {
                var targets = db.Queryable<R_SAP_JOB>().Where(t => t.JOBNAME == ENUM_R_SAP_JOB_FUNCTION.ChangeCrsdWithSap.ExtValue() && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue()).OrderBy(t => t.CREATETIME, OrderByType.Asc).ToList();
                foreach (var item in targets)
                {
                    var obj = db.Queryable<R_SAP_JOB>().Where(t => t.ID == item.ID && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue()).ToList().FirstOrDefault();
                    if (obj == null)
                        continue;
                    try
                    {
                        if (!string.IsNullOrEmpty(obj.RUNTIME) && !string.IsNullOrEmpty(obj.LASTEDITTIME) && int.Parse(obj.RUNTIME)>5 && Convert.ToDateTime(obj.LASTEDITTIME).AddHours(3)>System.DateTime.Now)                        
                            continue;                        
                    }
                    catch { }

                    item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Runing.ExtValue();
                    db.Updateable(item).ExecuteCommand();
                    var ordermain = db.Queryable<O_ORDER_MAIN, I137_I>((m, i) => m.ITEMID == i.ID).Where((m, i) => m.PREWO == item.DATA2).Select((m, i) => new { m, i }).ToList().FirstOrDefault();
                    if (ordermain == null)
                    {
                        item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Closed.ExtValue();
                        db.Updateable(item).ExecuteCommand();
                        continue;
                    }
                    JuniperPoTracking trackobj = new JuniperPoTracking(JuniperErrType.I137, JuniperSubType.ChangeCrsdJob, ordermain.i.PONUMBER, ordermain.i.ITEM, ordermain.m.UPOID, db);
                    try
                    {
                        item.DATA3 = Convert.ToDateTime(item.DATA3) < System.DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd") : item.DATA3;
                        item.RUNTIME = item.RUNTIME == null ? "1" : (int.Parse(item.RUNTIME) + 1).ToString();
                        item.LASTEDITTIME = DateTime.Now.ToString();
                        var sapwostartdate = new Func<string>(() =>
                        {
                            var chinaZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                            // 机器本地时间 -> 中国时间
                            var chinaTime = TimeZoneInfo.ConvertTime(Convert.ToDateTime(item.DATA3), TimeZoneInfo.Local, chinaZone);
                            //var startdate = (Convert.ToDateTime(item.CUSTREQSHIPDATE).Date - chinaTime.Date).Days <= 7 ? chinaTime.AddDays(1) : Convert.ToDateTime(item.CUSTREQSHIPDATE);
                            //var startdate = Convert.ToDateTime(item.CUSTREQSHIPDATE).AddDays(-7); //Donald hui要求FVN、FJZ都改為CRSD減7天
                            //var nowtime = Convert.ToDateTime(mesdb.GetDate().ToString("yyyy-MM-dd 00:00:00"));
                            //var startdate = (Convert.ToDateTime(item.CUSTREQSHIPDATE).Date - chinaTime.Date).Days <= 7 ? chinaTime.AddDays(1) : Convert.ToDateTime(item.CUSTREQSHIPDATE).AddDays(-7); //時間不能傳過去的時間，SAP會回only WO scheudling type is 3 that startdate can in past
                            return chinaTime.ToString("yyyy-MM-dd");
                        })();
                        var updatesapres = JuniperBase.ChangeCrsdWithSap(this.bu, item.DATA2, sapwostartdate);
                        if (updatesapres.issuccess)
                        {
                            item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Finish.ExtValue();
                            trackobj.ReleaseFuncExcption();
                        }
                        else
                        {
                            //if (int.Parse(item.RUNTIME) > 20)
                            //    item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Closed.ExtValue();
                            //else
                            item.DATA6 = updatesapres.msg.Length > 50 ? updatesapres.msg.Substring(0, 50) : updatesapres.msg;
                            //item.DATA5 = System.DateTime.Now.AddHours(4).ToString(MES_CONST_DATETIME_FORMAT.Normal.ExtValue());
                            item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue();
                            //item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue();
                            trackobj.ExceptionProcess(updatesapres.issuccess, updatesapres.msg);
                        }
                        db.Updateable(item).ExecuteCommand();
                    }
                    catch (Exception e)
                    {
                        item.JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue();
                        db.Updateable(item).ExecuteCommand();
                        trackobj.ExceptionProcess(false, e.Message);
                    }
                }
            }
        }
    }

    public class JuniperAutoConfigSku : FunctionBase
    {
        public JuniperAutoConfigSku(string _dbstr, string _bu) : base(_dbstr, _bu) { }

        /// <summary>
        /// </summary>
        public override void FunctionRun()
        {

        }
    }



}
