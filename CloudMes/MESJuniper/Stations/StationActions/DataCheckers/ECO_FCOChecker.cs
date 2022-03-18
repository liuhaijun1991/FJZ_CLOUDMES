using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MESDataObject.Constants.PublicConstants;

namespace MESJuniper.Stations.StationActions.DataCheckers
{
    public class ECO_FCOChecker
    {
        /// <summary>
        /// The Same Sale Region
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ECO_FCONoLockChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession InputObjSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (InputObjSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            var inputType = Paras[0].VALUE.ToString();
            var strData = InputObjSession.Value.ToString();
            O_ORDER_MAIN orderm = null;
            switch (inputType)
            {
                case "":
                case "SN":
                    orderm = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, R_SN>((O, S) => new object[] { JoinType.Left, O.PREWO == S.WORKORDERNO })
                            .Where((O, S) => S.SN == strData && S.VALID_FLAG == "1")
                            .Select((O, S) => O)
                            .First();
                    break;
                case "WO":
                    orderm = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>()
                            .Where(O => O.PREWO == strData)
                            .First();
                    break;
                case "PALLET":
                    var wo = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((S, SP, P1, P2) =>
                        new object[] {
                            JoinType.Left,S.ID==SP.SN_ID,
                            JoinType.Left,SP.PACK_ID==P1.ID,
                            JoinType.Left,P1.PARENT_PACK_ID==P2.ID
                        })
                        .Where((S, SP, P1, P2) => P2.PACK_NO == strData)
                        .Select((S, SP, P1, P2) => S.WORKORDERNO)
                        .First();
                    if (wo == null)
                    {
                        throw new Exception("Can't not get WO info!");
                    }
                    orderm = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>()
                            .Where(O => O.PREWO == wo)
                            .First();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (orderm == null)
            {
                return;
            }
            var I137 = Station.SFCDB.ORM.Queryable<O_I137_HEAD, O_I137_ITEM>((H, I) => new object[] { JoinType.Left, H.TRANID == I.TRANID })
                .Where((H, I) => I.ID == orderm.ITEMID)
                .Select((H, I) => H)
                .First();
            if (!string.IsNullOrEmpty(I137.ECO_FCO))
            {
                var ECO_FCO_LIST = Station.SFCDB.ORM.Queryable<R_F_CONTROL>()
                   .Where(t => t.FUNCTIONNAME == "Juniper_ECO_FCO_Lock" && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE.ToUpper() == I137.ECO_FCO.ToUpper())
                   .Select(t => new { t.VALUE, t.CATEGORY, t.EXTVAL })
                   .ToList();
                if (ECO_FCO_LIST.Count > 0)
                {
                    if (!Station.SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == orderm.PREWO && (t.LOCK_REASON.StartsWith("ECO_FCO") || t.LOCK_REASON.StartsWith("Shipping Note Includes"))).Any())
                    {
                        var db = Station.DBS["SFCDB"].Borrow();
                        try
                        {
                            db.ORM.Insertable(new R_SN_LOCK()
                            {
                                ID = MesDbBase.GetNewID<R_SN_LOCK>(db.ORM, Customer.JUNIPER.ExtValue()),
                                WORKORDERNO = orderm.PREWO,
                                LOCK_STATION = "ALL",
                                TYPE = "WO",
                                LOCK_STATUS = MesBool.Yes.ExtValue(),
                                //LOCK_REASON = $@"ECO_FCO Hold By Station! HoldReason: {I137.SHIPPINGNOTE},pls check!",//when SHIPPINGNOTE.lenth is too long, it will throw exception:value too large for column
                                LOCK_REASON = $@"ECO_FCO Hold By Station! HoldReason: ECO_FCO[{I137.ECO_FCO.ToUpper()}] in Control List, pls review PoDetail.ShippingNote!",
                                LOCK_TIME = DateTime.Now,
                                LOCK_EMP = "MESSYSTEM"
                            }).ExecuteCommand();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            Station.DBS["SFCDB"].Return(db);
                        }
                        throw new Exception($@"ECO_FCO change has been change,need engineer to confirm!Current WO has been lock by [ALL] station!");
                    }
                }
            }
            if (!string.IsNullOrEmpty(I137.SHIPPINGNOTE))
            {
                var ShippingNote_Control = Station.SFCDB.ORM.Queryable<R_F_CONTROL>()
                   .Where(t => t.FUNCTIONNAME == "Juniper_ShippingNote_Lock" && t.FUNCTIONTYPE == "NOSYSTEM" && I137.SHIPPINGNOTE.ToUpper().Contains(t.VALUE.ToUpper()))
                   .Select(t => new { t.VALUE, t.CATEGORY, t.EXTVAL })
                   .First();
                if (ShippingNote_Control != null)
                {
                    if (!Station.SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == orderm.PREWO && (t.LOCK_REASON.StartsWith("ECO_FCO") || t.LOCK_REASON.StartsWith("Shipping Note Includes"))).Any())
                    {
                        var db = Station.DBS["SFCDB"].Borrow();
                        try
                        {
                            db.ORM.Insertable(new R_SN_LOCK()
                            {
                                ID = MesDbBase.GetNewID<R_SN_LOCK>(db.ORM, Customer.JUNIPER.ExtValue()),
                                WORKORDERNO = orderm.PREWO,
                                LOCK_STATION = "ALL",
                                TYPE = "WO",
                                LOCK_STATUS = MesBool.Yes.ExtValue(),
                                //LOCK_REASON = $@"Shipping Note Includes {ShippingNote_Control.VALUE} Hold By Station! HoldReason: {I137.SHIPPINGNOTE},pls check!",//when SHIPPINGNOTE.lenth is too long, it will throw exception:value too large for column
                                LOCK_REASON = $@"Shipping Note Includes {ShippingNote_Control.VALUE} Hold By Station! HoldReason: [{ShippingNote_Control.VALUE}] in Control List, pls review PoDetail.ShippingNote!",
                                LOCK_TIME = DateTime.Now,
                                LOCK_EMP = "MESSYSTEM"
                            }).ExecuteCommand();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            Station.DBS["SFCDB"].Return(db);
                        }
                        throw new Exception($@"ECO_FCO change has been change,need engineer to confirm!Current WO has been lock by [ALL] station!");
                    }
                }
            }
        }
    }
}
