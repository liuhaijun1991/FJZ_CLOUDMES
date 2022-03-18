using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDataObject.Module.HWD;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESStation.Packing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static MESDataObject.Constants.PublicConstants;
using static MESDataObject.Common.EnumExtensions;
using System.Text;
using MESDataObject.Module.Juniper;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckPack
    {        
        /// <summary>
        /// 檢查Pack狀態是否可以入OBA
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackStatusInOba(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            DisplayOutPut Dis_LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO");
            DisplayOutPut Dis_SkuNo = Station.DisplayOutput.Find(t => t.Name == "SKUNO");
            DisplayOutPut Dis_Ver = Station.DisplayOutput.Find(t => t.Name == "VER");
            MESStationInput Level = Station.Inputs.Find(t => t.DisplayName == "AQLLEVEL");
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            #region 用於界面上顯示的批次信息
            R_LOT_STATUS rLotStatus = new R_LOT_STATUS();
            List<R_LOT_PACK> rLotPackList = new List<R_LOT_PACK>();
            List<string> cAqlLevel = new List<string>();
            #endregion

            T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
            //add by fgg 2018.6.28 卡棧板是否關閉，避免棧板還沒有關閉就拿去抽檢OBA，導致抽檢總數異常，進而導致OBA抽檢數量不對
            if (!t_r_packing.CheckCloseByPackno(packSession.Value.ToString(), Station.SFCDB))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180611104338", new string[] { packSession.Value.ToString() }));
            }
            LotNo LotNo = new LotNo(Station.DBType);
            T_R_LOT_PACK tRLotPack = new T_R_LOT_PACK(Station.SFCDB, Station.DBType);
            rLotPackList = tRLotPack.GetRLotPackWithWaitClose(Station.SFCDB, packSession.Value.ToString());
            List<R_LOT_STATUS> rLotStatusList = tRLotPack.GetRLotStatusWithWaitClose(Station.SFCDB, packSession.Value.ToString());
            rLotStatus = rLotStatusList.Find(t => t.CLOSED_FLAG == "1");
            if (rLotStatus != null)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180526181007", new string[] { packSession.Value.ToString() }));
            else
                rLotStatus = rLotStatusList.Find(t => t.CLOSED_FLAG == "0");

            if (Dis_LotNo.Value.Equals(""))
            {
                #region 當前Lot為空=>檢查當前Pack無有效LOT?新建LOT:加載LOT;

                if (rLotPackList.Count == 0)
                {
                    rLotStatus =
                        LotNo.CreateLotByPackno(Station.LoginUser, packSession.Value.ToString(), Station.SFCDB);
                    rLotPackList.Add(
                        new R_LOT_PACK() { LOTNO = rLotStatus.LOT_NO, PACKNO = packSession.Value.ToString() });
                }
                else
                    rLotStatus = rLotStatusList.Find(t => t.CLOSED_FLAG == "0");
                #endregion
            }
            else
            {
                #region 當前Lot不為空=>PackNo與當前頁面LOT的機種版本是否一致?ReLoad LOT信息:Throw e;
                T_R_PACKING tRPacking = new T_R_PACKING(Station.SFCDB, Station.DBType);
                Row_R_PACKING rowRPacking = tRPacking.GetRPackingByPackNo(Station.SFCDB, packSession.Value.ToString());
                if (!rowRPacking.SKUNO.Equals(Dis_SkuNo.Value.ToString()))
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180526185434", new string[] { packSession.Value.ToString() }));

                if (rLotPackList.Count == 0)
                {
                    rLotStatus = Station.SFCDB.ORM.Queryable<R_LOT_STATUS>()
                        .Where(x => x.LOT_NO == Dis_LotNo.Value.ToString()).ToList().FirstOrDefault();
                    if (rLotStatus.CLOSED_FLAG != "0" || rLotStatus.LOT_STATUS_FLAG != "0")
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180911154309", new string[] { Dis_LotNo.Value.ToString() }));
                    rLotStatus = LotNo.ObaInLotByPackno(Station.LoginUser, rLotStatus, packSession.Value.ToString(), Level.Value.ToString(), Station.SFCDB);
                    rLotPackList.Add(new R_LOT_PACK() { LOTNO = rLotStatus.LOT_NO, PACKNO = packSession.Value.ToString() });
                }
                else
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180526185618", new string[] { packSession.Value.ToString(), rLotStatus.LOT_NO }));
                #endregion
            }
            #region 加載AQL等級
            T_C_AQLTYPE tCAqlType = new T_C_AQLTYPE(Station.SFCDB, Station.DBType);
            cAqlLevel = tCAqlType.GetAqlLevelByType(rLotStatus.AQL_TYPE, Station.SFCDB);
            T_C_SKU_AQL tCSkuAql = new T_C_SKU_AQL(Station.SFCDB, Station.DBType);
            C_SKU_AQL cSkuAql = tCSkuAql.GetSkuAql(Station.SFCDB, rLotStatus.SKUNO);
            #endregion
            #region 加載界面信息
            MESStationSession lotNoSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
            MESStationSession skuNoSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
            MESStationSession aqlSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
            MESStationSession lotQtySession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
            MESStationSession sampleQtySession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
            MESStationSession RejectQtySession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };

            Station.StationSession.Clear();
            Station.StationSession.Add(lotNoSession);
            Station.StationSession.Add(skuNoSession);
            Station.StationSession.Add(aqlSession);
            Station.StationSession.Add(lotQtySession);
            Station.StationSession.Add(sampleQtySession);
            Station.StationSession.Add(RejectQtySession);

            lotNoSession.Value = rLotStatus.LOT_NO;
            skuNoSession.Value = rLotStatus.SKUNO;
            aqlSession.Value = rLotStatus.AQL_TYPE;
            lotQtySession.Value = rLotStatus.LOT_QTY;
            sampleQtySession.Value = rLotStatus.SAMPLE_QTY;
            RejectQtySession.Value = rLotStatus.REJECT_QTY;

            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
            s.DataForUse.Clear();
            foreach (var VARIABLE in rLotPackList)
                s.DataForUse.Add(VARIABLE.PACKNO);

            MESStationInput l = Station.Inputs.Find(t => t.DisplayName == "AQLLEVEL");
            l.DataForUse.Clear();
            foreach (string VARIABLE in cAqlLevel)
                l.DataForUse.Add(VARIABLE);
            //設置默認等級
            l.Value = cSkuAql.DEFAULLEVEL;
            #endregion
        }

        /// <summary>
        /// 檢查棧板中SN是否有被鎖定
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackSnStatusIsLock(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_SN_LOCK tRSnLock = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);
            List<R_SN_LOCK> rSnLockList = tRSnLock.GetLockListByPackNo(Station.StationName,packSession.Value.ToString(), Station.SFCDB);
            string strSnList = "";
            foreach (R_SN_LOCK VARIABLE in rSnLockList)
                strSnList += VARIABLE.SN + ",";
            if (rSnLockList.Count > 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180531114237", new string[] { packSession.Value.ToString(), rSnLockList.Count().ToString(), strSnList }));
        }

        /// <summary>
        /// 檢查棧板中SN是否有被鎖定，DCN用棧板號掃描入CBS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackSnIsLock(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_SN_LOCK tRSnLock = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);
            List<R_SN_LOCK> rSnLockList = tRSnLock.GetLockListByPackNo(Station.StationName,packSession.Value.ToString(), Station.SFCDB);
            if (rSnLockList.Count > 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180531114237", new string[] { packSession.Value.ToString(), rSnLockList.Count().ToString(), rSnLockList.ToString() }));
            }
            List<R_SN> res = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rp1, rp2, rsp, rs) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rsp.SN_ID == rs.ID && rs.VALID_FLAG == "1")
                .Where(rp1 => rp1.PACK_NO == packSession.Value.ToString()).Select((rp1, rp2, rsp, rs) => new R_SN
                {
                    SN = rs.SN
                    //SCRAPED_FLAG=rs.SCRAPED_FLAG
                }).ToList();

            var snlist = res.Select(t => t.SN).ToList();

            for (int i = 1; i < Paras.Count - 1; i++)
            {
                string station = Paras[i].VALUE.ToString();

                if (res.FindAll(t => t.SCRAPED_FLAG == "1").Count > 0)
                {
                    string strSnList = "";
                    var rrr = res.FindAll(t => t.SCRAPED_FLAG == "1");
                    foreach (var VAR_SNRIABLE in rrr)
                    {
                        strSnList += VAR_SNRIABLE + ",";
                    }
                    strSnList = strSnList.Substring(0, strSnList.Length - 1);
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200506161757", new string[] { }));
                }
                switch (station)
                {
                    case "FQA1":
                        R_SN_STATION_DETAIL res1 = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(rsd => snlist.Contains(rsd.SN) && rsd.STATION_NAME == "COSMETIC-FAILURE").ToList().FirstOrDefault();
                        if (res1 == null)
                            break;
                        var res2 = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(rsd => snlist.Contains(rsd.SN) && rsd.STATION_NAME == station && rsd.EDIT_TIME < res1.EDIT_TIME).Any();
                        if (!res2)
                        {
                            var res3 = Station.SFCDB.ORM.Queryable<R_SN_PASS>().Where(rsd => snlist.Contains(rsd.SN) && rsd.PASS_STATION == station && rsd.STATUS == "1").Any();
                            if (res3)
                            {
                                R_SN_STATION_DETAIL res4 = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(rsd => snlist.Contains(rsd.SN) && rsd.STATION_NAME == station && rsd.VALID_FLAG == "1").ToList().FirstOrDefault();
                                R_SN_STATION_DETAIL res6 = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(rsd => snlist.Contains(rsd.SN) && rsd.STATION_NAME == "COSMETIC-FAILURE").OrderBy(rsd => rsd.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                                var res5 = Station.SFCDB.ORM.Queryable<R_TEST_BRCD>().Where(rsd => snlist.Contains(rsd.SYSSERIALNO) && rsd.EVENTNAME == res4.STATION_NAME && rsd.TESTDATE > res6.EDIT_TIME && rsd.STATUS == "PASS").Any();
                                if (res5)
                                {
                                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20200506182200", new string[] { packSession.Value.ToString(), res4.STATION_NAME, res4.STATION_NAME }));
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 檢查棧板中SN工站/入庫/維修/出貨和SN數量對不對
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackSnStatusAndQty(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string palletno = "";
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (packSession != null)
            {
                palletno = packSession.Value.ToString();
            }

            List<R_SN> listsn = new List<R_SN>();
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            t_r_sn.GetSNsByPackNo(palletno, ref listsn, Station.SFCDB);

            //棧板SN狀態檢查.
            var chklist = listsn
                .Select(p => new
                {
                    PACKED_FLAG = p.PACKED_FLAG,
                    COMPLETED_FLAG = p.COMPLETED_FLAG,
                    SHIPPED_FLAG = p.SHIPPED_FLAG,
                    REPAIR_FAILED_FLAG = p.REPAIR_FAILED_FLAG,
                    CURRENT_STATION = p.CURRENT_STATION,
                    NEXT_STATION = p.NEXT_STATION
                })
                .Distinct().ToList();

            if (chklist.Count > 1)
            {
                //SN 狀態不一致.
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200813144240", new string[] { }));
            }
            else if (chklist.Count == 1)
            {
                if (chklist[0].NEXT_STATION != Station.StationName)
                {
                    //棧板狀態不在CBS.
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200813144146", new string[] { }));
                }
                if (chklist[0].SHIPPED_FLAG != "0")
                {
                    //該棧板已經出貨.
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200813144206", new string[] { }));
                }
                if (chklist[0].REPAIR_FAILED_FLAG != "0")
                {
                    //該棧板已經進維修.
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200813144220", new string[] { }));
                }
                if (chklist[0].COMPLETED_FLAG != "0")
                {
                    //該棧板已經入過庫.
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200813143142", new string[] { }));
                }
            }
            else
            {
                //
            }

            //棧板數量與SN數量比對檢查.
            R_PACKING P = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == palletno).ToList().FirstOrDefault();
            if (P.PARENT_PACK_ID != null && P.PARENT_PACK_ID != "")
            {
                //多級包裝,計算卡通數量與SN數量做比對.
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                double? ctnsnqty = t_r_packing.GetPackByPL(palletno, Station.SFCDB).Select(p => p.QTY).DefaultIfEmpty().Sum();
                if (ctnsnqty != listsn.Count)
                {
                    //該棧板數量與SN實際數量不匹配.
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200813144255", new string[] { }));
                }
            }
            else
            {
                //單級包裝,直接比對棧板數量和SN數量
                if (P.QTY != listsn.Count)
                {
                    //該棧板數量與SN實際數量不匹配.
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200813144255", new string[] { }));
                }
            }

        }

        /// <summary>
        /// 檢查棧板或卡通是否關閉
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackCloseStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            try
            {
                LogicObject.Packing packObject = (LogicObject.Packing)sessionPackObject.Value;
                if (packObject.ClosedFlag == "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180611104338", new string[] { packObject.PackNo }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  檢查當前Pack是否關閉
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackIsClose(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession packNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            var res = Station.SFCDB.ORM.Queryable<R_PACKING>()
                .Where(x => x.PACK_NO == packNoSession.Value.ToString() && x.CLOSED_FLAG == "1").Any();
            if (!res)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180611104338", new string[] { packNoSession.Value.ToString() }));
        }
        /// <summary>
        /// 移棧板或卡通檢查移動數量是否超出最大值
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckMoveValueIsOK(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionMoveValue = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionMoveValue == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (sessionMoveValue.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            try
            {
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
                LogicObject.Packing packObject = (LogicObject.Packing)sessionPackObject.Value;
                Newtonsoft.Json.Linq.JArray moveValueArray = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(sessionMoveValue.Value.ToString());

                if (packObject.ParentPackID != null && packObject.ParentPackID.ToString() != "")
                {
                    //CARTON
                    if (packObject.MaxQty < packObject.Qty + moveValueArray.Count)
                    {
                        throw new MESReturnMessage(
                            MESReturnMessage.GetMESReturnMessage("MSGCODE20180613092900",
                            new string[]
                            {
                             packObject.Qty.ToString(),
                             packObject.PackNo,
                             packObject.MaxQty.ToString(),
                             packObject.Qty.ToString()
                            }));
                    }
                }
                else
                {
                    //Pallet
                    List<R_PACKING> packingList = t_r_packing.GetListPackByParentPackId(packObject.PackID, Station.SFCDB);
                    if (packObject.MaxQty < packingList.Count + moveValueArray.Count)
                    {
                        throw new MESReturnMessage(
                            MESReturnMessage.GetMESReturnMessage("MSGCODE20180613092900",
                            new string[]
                            {
                             moveValueArray.Count.ToString(),
                             packObject.PackNo,
                             packObject.MaxQty.ToString(),
                             packingList.Count.ToString()
                            }));
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 棧板或卡通是否存在
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackingIsExist(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
            if (!t_r_packing.PackNoIsExist(sessionPackObject.Value.ToString(), Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { sessionPackObject.Value.ToString() }));
            }
        }

        /// <summary>
        /// 移棧板檢查棧板是否在OBA抽檢狀態
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackNoIsOnOBASamping(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackNo == null || sessionPackNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            T_R_LOT_PACK t_r_lot_pack = new T_R_LOT_PACK(Station.SFCDB, Station.DBType);
            if (t_r_lot_pack.PackNoIsOnOBASampling(sessionPackNo.Value.ToString(), Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180622104731", new string[] { sessionPackNo.Value.ToString() }));
            }
        }

        /// <summary>
        /// 檢查棧板或卡通中SN是否有被鎖定
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void MovePackCheckSnStatusIsLock(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            R_PACKING packObject = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(p => p.PACK_NO == sessionPackObject.Value.ToString()).ToList().FirstOrDefault();
            if (packObject != null)
            {
                T_R_SN_LOCK tRSnLock = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);
                List<R_SN_LOCK> rSnLockList = new List<R_SN_LOCK>();
                string strSnList = "";
                if (packObject.PACK_TYPE == LogicObject.PackType.PALLET.ToString())
                {
                    rSnLockList = tRSnLock.GetLockListByPackNo("SHIPOUT",packObject.PACK_NO, Station.SFCDB);
                }
                else if (packObject.PACK_TYPE == LogicObject.PackType.CARTON.ToString())
                {
                    rSnLockList = tRSnLock.GetLockListByCartonNo(packObject.PACK_NO, Station.SFCDB);
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259", new string[] { }));
                }

                foreach (R_SN_LOCK snLock in rSnLockList)
                {
                    strSnList += snLock.SN + ",";
                }
                if (rSnLockList.Count > 0 && packObject.PACK_TYPE == LogicObject.PackType.PALLET.ToString())
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180531114237", new string[] { packObject.PACK_NO, rSnLockList.Count().ToString(), strSnList }));
                }
                if (rSnLockList.Count > 0 && packObject.PACK_TYPE == LogicObject.PackType.CARTON.ToString())
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808141643", new string[] { packObject.PACK_NO, rSnLockList.Count().ToString(), strSnList }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { sessionPackObject.Value.ToString() }));
            }
        }

        /// <summary>
        /// Ham kiem tra da thuc hien check sau khi chuyen carton chua
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void shipOutCheckMoveStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            R_PACKING packObject = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(p => p.PACK_NO == sessionPackObject.Value.ToString()).ToList().FirstOrDefault();
            if (packObject != null)
            {
                var lst = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN_LOG>((rp1, rp2, rsp, rnl) => rp2.PARENT_PACK_ID == rp1.ID && rp1.PACK_NO == sessionPackObject.Value.ToString() && rp2.ID == rsp.PACK_ID && rnl.LOGTYPE == "RECHECK" && (rsp.SN_ID == rnl.SNID || rp2.PACK_NO == rnl.SN) && rnl.FLAG == "0").Select((rp1, rp2, rsp, rnl) => rnl).ToList();
                if (lst.Count > 0)
                {
                    //throw new MESReturnMessage("Sau khi thực hiện tách gộp Pallet. Chưa thực hiện RECHECK lại ở chức năng CHECKOUTCHECK");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140847"));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { sessionPackObject.Value.ToString() }));
            }
        }


        /// <summary>
        /// Ham kiem tra da thuc hien check sau khi chuyen carton chua
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void obaCheckMoveStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            string pack = Input.Value.ToString();
            R_PACKING packObject = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(p => p.PACK_NO == pack).ToList().FirstOrDefault();
            if (packObject != null)
            {
                var lst = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN_LOG>((rp1, rp2, rsp, rnl) => rp2.PARENT_PACK_ID == rp1.ID && rp1.PACK_NO == pack && rp2.ID == rsp.PACK_ID && rnl.LOGTYPE == "RECHECK" && (rsp.SN_ID == rnl.SNID || rp2.PACK_NO == rnl.SN) && rnl.FLAG == "0").Select((rp1, rp2, rsp, rnl) => rnl).ToList();
                if (lst.Count > 0)
                {
                    //throw new MESReturnMessage("Sau khi thực hiện tách gộp Pallet. Chưa thực hiện RECHECK lại ở chức năng CHECKOUTCHECK");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140847"));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { pack }));
            }
        }

        /// <summary>
        /// 檢查棧板或卡通中SN是否已出貨
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnIsShippedByCartonOrPallet(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            R_PACKING packObject = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(p => p.PACK_NO == sessionPackObject.Value.ToString()).ToList().FirstOrDefault();
            if (packObject != null)
            {
                List<R_SN> snList = new List<R_SN>();
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
                string strSnList = "";
                if (packObject.PACK_TYPE == LogicObject.PackType.PALLET.ToString())
                {
                    snList = t_r_packing.GetSnListByPalletID(packObject.ID, Station.SFCDB);
                }
                else if (packObject.PACK_TYPE == LogicObject.PackType.CARTON.ToString())
                {
                    snList = t_r_packing.GetSnListByCartonID(packObject.ID, Station.SFCDB);
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259", new string[] { }));
                }

                foreach (R_SN sn in snList)
                {
                    strSnList += sn.SN + ",";
                }
                if (snList.Count > 0 && packObject.PACK_TYPE == LogicObject.PackType.PALLET.ToString())
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808142945", new string[] { packObject.PACK_NO, snList.Count().ToString(), strSnList }));
                }
                if (snList.Count > 0 && packObject.PACK_TYPE == LogicObject.PackType.CARTON.ToString())
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808141806", new string[] { packObject.PACK_NO, snList.Count().ToString(), strSnList }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { sessionPackObject.Value.ToString() }));
            }
        }

        public static void CheckPackNoAndDnLineStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string packNo = string.Empty;
            if (Paras.Count == 0)
            {
                packNo = Input.Value.ToString();
            }
            else
            {
                MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (PackNoSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                }
                packNo = PackNoSession.Value.ToString();
            }
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
                   dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString(),
                   skuNo = Station.DisplayOutput.Find(t => t.Name == "SKU_NO").Value.ToString();
            if (dnNo.Length == 0 && dnLine.Length == 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801113040"));
            var rPacking = new PalletBase(packNo, Station.SFCDB);

            if (rPacking.GetCount(Station.SFCDB) == 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { packNo }));

            #region 經管要求NPI幾種必須以N結尾的料號進行生產，以不帶N結尾料號出貨
            /*2021-1-28 10:56:06 LJD            
            解決方案為：
            1、PD以N結尾料號生產入庫；
            2、PM手動做N料號費領，用不帶N料號費退；
            3、WHS用不帶N料料號出貨；
            系統修改方案：
            判斷廠別為NLEZ，料號為VT開頭N結尾，出貨判斷料號時把結尾N去掉再進行比對。
             */
            var dn_plant = Station.SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == dnNo).First().DN_PLANT;
            var packsku = rPacking.DATA.SKUNO;
            if (dn_plant == "NLEZ" && packsku.EndsWith("N") && packsku.StartsWith("VT"))
            {
                if (skuNo.EndsWith("N"))
                    //M.M Requires that NPI must be shipped with a part number that does not end with ‘N’.
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210129082612", new string[] { packNo, packsku, dnNo, dnLine, skuNo }));
                if (!packsku.Substring(0, packsku.Length - 1).Equals(skuNo))
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801085546", new string[] { packNo, packsku, dnNo, dnLine, skuNo }));
            }
            else if (!packsku.Equals(skuNo))
            {
                DataTable dt = Station.SFCDB.ORM.Ado.GetDataTable("select *from all_tables where table_name='R_PRE_WO_HEAD' and owner='SFCRUNTIME'");
                bool checkGourpID = false;
                if (Station.BU.Equals("VNJUNIPER"))
                {
                    MESDataObject.Module.Juniper.R_PRE_WO_HEAD grouidObj = Station.SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_PRE_WO_HEAD, R_SN, R_SN_PACKING, R_PACKING>
                        ((w, s, rsp, rpc) => w.WO == s.WORKORDERNO && s.ID == rsp.SN_ID && rsp.PACK_ID == rpc.ID)
                        .Where((w, s, rsp, rpc) => rpc.PARENT_PACK_ID == rPacking.DATA.ID && s.VALID_FLAG == "1")
                        .Select((w, s, rsp, rpc) => w).ToList().FirstOrDefault();

                    if (grouidObj != null)
                    {
                        checkGourpID = true;
                        if (!grouidObj.GROUPID.Equals(skuNo))
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801085546", new string[] { packNo, grouidObj.PID, dnNo, dnLine, skuNo }));
                        }
                    }
                }

                if (!checkGourpID)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801085546", new string[] { packNo, packsku, dnNo, dnLine, skuNo }));
                }
            }
            #endregion
            //if (!packsku.Equals(skuNo))
            //    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801085546", new string[] { packNo, packsku, dnNo, dnLine, skuNo }));
            var rDnStatus = Station.SFCDB.ORM.Queryable<R_DN_STATUS>()
                .Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine && x.DN_FLAG == "0").ToList();
            if (rDnStatus.Count != 1)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731133647", new string[] { dnNo, dnLine }));


            int totalQty = 0;
            int shippedQty = 0;
            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            bool bCombinedSkuno = t_r_function_control.CheckUserFunctionExist("CombineDNToShipout", "SKUNO", skuNo, Station.SFCDB);
            bool bCombinedAll = t_r_function_control.CheckUserFunctionExist("CombineDNToShipout", "SKUNO", "ALL", Station.SFCDB);
            if (bCombinedAll || bCombinedSkuno)
            {
                var toObj = Station.SFCDB.ORM.Queryable<R_TO_DETAIL>().Where(r => r.DN_NO == dnNo).ToList().FirstOrDefault();
                var dnList = Station.SFCDB.ORM.Queryable<R_DN_STATUS>()
                    .Where(r => r.SKUNO == skuNo && SqlSugar.SqlFunc.Subqueryable<R_TO_DETAIL>().Where(t => t.DN_NO == r.DN_NO && t.TO_NO == toObj.TO_NO).Any())
                    .ToList();
                foreach (var item in dnList)
                {
                    totalQty += (int)item.QTY;
                    var shippedList = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(x => x.DN_NO == item.DN_NO && x.DN_LINE == item.DN_LINE).ToList();
                    shippedQty += shippedList.Count;
                }
            }
            else
            {
                totalQty = (int)rDnStatus.FirstOrDefault().QTY;
                shippedQty = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine).ToList().Count;
            }

            //var rShipDetail = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>()
            //    .Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine).ToList();
            var packSnQty = rPacking.GetSnCount(Station.SFCDB);
            if (packSnQty > totalQty - shippedQty)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801091520", new string[] { packNo, packSnQty.ToString(), rDnStatus.FirstOrDefault().QTY.ToString() }));

            var rShipDetail = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_PACKING, R_SN_PACKING>((rsd, rp, rsp) =>
                rsd.ID == rsp.SN_ID && rp.ID == rsp.PACK_ID && rp.PARENT_PACK_ID == rPacking.DATA.ID).Select((rsd, rp, rsp) => rsd).ToList();
            if (rShipDetail.Count > 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180802105858", new string[] { packNo }));
        }


        /// <summary>
        /// 出貨檢查PL或者CARTON是否有移動且做MOVEPL_CHECK的動作
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckMovePLorCarton(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            string packno = packSession.Value.ToString();
            R_PACKING rpacking = null;
            string skuno = null;

            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();
            rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno).ToList().FirstOrDefault();
            skuno = rpacking.SKUNO.ToString();
            R_MOVE_LIST PackCarton = null;
            R_MOVE_LIST PackPL = null;
            PackPL = SFCDB.ORM.Queryable<R_MOVE_LIST, R_PACKING, R_PACKING>((rml, rp1, rp2) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rml.MOVE_ID)
                .Where((rml, rp1, rp2) => rp1.PACK_NO == packno).OrderBy((rml, rp1, rp2) => rml.MOVE_DATE, SqlSugar.OrderByType.Desc).Select((rml, rp1, rp2) => rml).ToList().FirstOrDefault();
            PackCarton = SFCDB.ORM.Queryable<R_MOVE_LIST, R_PACKING, R_PACKING, R_SN_PACKING>((rml, rp1, rp2, rsp) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rml.MOVE_ID == rsp.SN_ID)
                .Where((rml, rp1, rp2, rsp) => rp1.PACK_NO == packno).OrderBy((rml, rp1, rp2, rsp) => rml.MOVE_DATE, SqlSugar.OrderByType.Desc).Select((rml, rp1, rp2, rsp) => rml).ToList().FirstOrDefault();

            //移動棧板的情況
            if (PackPL != null)
            {
                bool ff = SFCDB.ORM.Queryable<R_SN_PACKING, R_SN_STATION_DETAIL>((rsp, rsd) => rsp.PACK_ID == rsd.ID).Where((rsp, rsd) => rsp.PACK_ID == PackPL.MOVE_ID && rsd.EDIT_TIME > PackPL.MOVE_DATE && rsd.STATION_NAME == "MOVEPL_CHECK").Select((rsp, rsd) => rsp).Any();
                if (ff)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616102950", new string[] { packno }));
                    //有移動記錄，且未掃描MOVEPL_CHECK_2
                }
            }

            //移動Carton的情況
            if (PackCarton != null)
            {
                bool ff = SFCDB.ORM.Queryable<R_SN_PACKING, R_SN_STATION_DETAIL>((rsp, rsd) => rsp.PACK_ID == rsd.ID).Where((rsp, rsd) => rsp.SN_ID == PackCarton.MOVE_ID && rsd.EDIT_TIME > PackCarton.MOVE_DATE && rsd.STATION_NAME == "MOVEPL_CHECK").Select((rsp, rsd) => rsp).Any();
                if (ff)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616102950", new string[] { packno }));
                    //有移動記錄，且未掃描MOVEPL_CHECK_3
                }
            }
        }

        /// <summary>
        /// OBA檢查PL或者CARTON是否有移動並且重新稱重
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckMoveThenPassWeight(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (packSession == null || packSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            OleExec SFCDB = Station.SFCDB;
            string packno = packSession.Value.ToString();
            R_PACKING rpacking = null;
            string skuno = null;

            //string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();
            rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno).ToList().FirstOrDefault();
            skuno = rpacking.SKUNO.ToString();

            //判斷路由中是否有weight_check2
            bool weiht = SFCDB.ORM.Queryable<C_SKU, R_SKU_ROUTE, C_ROUTE_DETAIL>((cs, rsr, crd) => cs.ID == rsr.SKU_ID && rsr.ROUTE_ID == crd.ROUTE_ID).Where((cs, rsr, crd) => cs.SKUNO == skuno && crd.STATION_NAME == "WEIGHT-CHECK2").Select((cs, rsr, crd) => crd).Any();

            if (weiht)
            {
                List<R_MOVE_LIST> PackCarton = null;
                List<R_MOVE_LIST> PackPL = null;
                PackPL = SFCDB.ORM.Queryable<R_MOVE_LIST, R_PACKING, R_PACKING>((rml, rp1, rp2) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rml.MOVE_ID)
                    .Where((rml, rp1, rp2) => rp1.PACK_NO == packno).OrderBy((rml, rp1, rp2) => rml.MOVE_DATE, SqlSugar.OrderByType.Desc).Select((rml, rp1, rp2) => rml).ToList();
                PackCarton = SFCDB.ORM.Queryable<R_MOVE_LIST, R_PACKING, R_PACKING, R_SN_PACKING>((rml, rp1, rp2, rsp) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rml.MOVE_ID == rsp.SN_ID)
                    .Where((rml, rp1, rp2, rsp) => rp1.PACK_NO == packno).OrderBy((rml, rp1, rp2, rsp) => rml.MOVE_DATE, SqlSugar.OrderByType.Desc).Select((rml, rp1, rp2, rsp) => rml).ToList();


                //移動棧板後檢查該SN是否重新稱重
                for (int i = 0; i < PackPL.Count; i++)
                {

                    bool bb = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL, R_SN_PACKING>((rs, rsp) => rs.R_SN_ID == rsp.SN_ID).Where((rs, rsp) => rs.VALID_FLAG =="1" && rsp.PACK_ID == PackPL[i].MOVE_ID && rs.STATION_NAME == "WEIGHT-CHECK2" && rs.EDIT_TIME > PackPL[i].MOVE_DATE).Select((rs, rsp) => rs).Any();
                    if (!bb)
                    {
                        var sns = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING>((r, rs) => r.ID == rs.SN_ID).Where((r, rs) => r.ID == PackPL[i].MOVE_ID && r.VALID_FLAG == "1").Select((r, rs) => r).ToList().FirstOrDefault();
                        //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210514101218", new string[] { sns.SN }));
                        throw new MESReturnMessage($@"SN: {sns.SN} exists PL/Carton move record after weight , please return to WEIGHT-CHECK2 station !");
                    }
                }
                //移動卡通後檢查該SN是否重新稱重
                for (int i = 0; i < PackCarton.Count; i++)
                {
                    bool ff = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where((r) => r.VALID_FLAG == "1" && r.R_SN_ID == PackCarton[i].MOVE_ID && r.STATION_NAME == "WEIGHT-CHECK2" && r.EDIT_TIME > PackCarton[i].MOVE_DATE).Any();
                    if (!ff)
                    {
                        R_SN rsn = SFCDB.ORM.Queryable<R_SN>().Where(r => r.ID == PackCarton[i].MOVE_ID).ToList().FirstOrDefault();
                        //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210514101218", new string[] { rsn.SN }));
                        throw new MESReturnMessage($@"SN: {rsn.SN} exists PL/Carton move record after weight , please return to WEIGHT-CHECK2 station !");
                    }
                }
            }
        }

        /// <summary>
        /// 檢查運輸方式
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckTranSport(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            string packno = packSession.Value.ToString();
            //T_R_SN rsn = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
            R_PACKING rpk = null;
            R_PACKING rpacking = null;
            string skuno = null;

            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();
            rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno).ToList().FirstOrDefault();
            skuno = rpacking.SKUNO.ToString();

            bool aa = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_TO_DETAIL, SD_REPORT_DETAIL, C_PACKING, R_PACKING>((rsd, rtd, srd, cp, rp)
                    => rsd.DN_NO == rtd.DN_NO && rtd.DN_NO == srd.VBELN && srd.BEZEI == cp.TRANSPORT_TYPE && rsd.SKUNO == cp.SKUNO && cp.SKUNO == rp.SKUNO && cp.TRANSPORT_TYPE != "DEFAULT")
                .Where((rsd, rtd, srd, cp, rp) => rp.PACK_NO == packno).Select((rsd, rtd, srd, cp, rp) => rsd).Any();
            if (!aa)
            {
                rpk = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno && r.PACK_TYPE == "PALLET")
                     .OrderBy(r => r.SKUNO, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            }
            else
            {
                rpk = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno && r.PACK_TYPE == "DEFAULT")
                    .OrderBy(r => r.SKUNO, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            }
            List<R_PACKING> rpjack = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno && r.MAX_QTY == rpk.MAX_QTY).ToList();

            if (aa && rpjack.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526142536", new string[] { packno }));
            }
        }

        /// <summary>
        /// RMA品與非RMA品不能混著出貨
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckRmaPack(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            string packno = packSession.Value.ToString();
            R_PACKING rpacking = null;
            string skuno = null;

            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();
            rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno).ToList().FirstOrDefault();
            skuno = rpacking.SKUNO.ToString();

            bool dd = SFCDB.ORM.Queryable<R_SHIP_DETAIL, R_SN, R_WO_TYPE>((rsd, rs, rwt) => rsd.SN == rs.SN && rsd.DN_NO == dnNo && rs.WORKORDERNO.Substring(1, 6) == rwt.PREFIX && rwt.WORKORDER_TYPE == "RMA").Select((rsd, rs, rwt) => rsd).Any();
            bool ee = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN, R_WO_TYPE>
                ((rp1, rp2, rsp, rsn, rwt) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID
                && rsp.SN_ID == rsn.ID && rsn.WORKORDERNO.Substring(1, 6) == rwt.PREFIX).Where((rp1, rp2, rsp, rsn, rwt) => rp1.PACK_NO == packno && rwt.WORKORDER_TYPE != "RMA").Select((rp1, rp2, rsp, rsn, rwt) => rp1).Any();
            if (dd && ee)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526142643", new string[] { packno }));
            }
            if (!dd && ee)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526142643", new string[] { packno }));
            }
            if (dd && !ee)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526142643", new string[] { packno }));
            }
        }

        /// <summary>
        /// 特殊特殊管控的SN不允許過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSNPrexid(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            string packno = packSession.Value.ToString();
            R_PACKING rpacking = null;
            string skuno = null;

            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();
            rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno).ToList().FirstOrDefault();
            skuno = rpacking.SKUNO.ToString();
            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            if (PackingTool.CheckSNPrexid(packno, SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526142239", new string[] { packno }));
            }
        }

        /// <summary>
        /// 直接出貨給客人，加入先進先出管控
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckFIFO(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            string packno = packSession.Value.ToString();
            R_PACKING rpacking = null;
            T_R_WO_BASE WoBase = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_WO_BASE> woversion = new List<R_WO_BASE>();
            string skuno = null;

            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();

            //bool aa = SFCDB.ORM.Queryable<R_TO_DETAIL, C_SHIP_CUSTOMER>((rd, cs) => rd.DN_CUSTOMER == cs.CUSTOMERNAME).Where((rd, cs) => rd.DN_NO == dnNo && cs.ROUTENAME == "CZ" && cs.ROUTENAME == "HS").Any();
            bool aa = SFCDB.ORM.Queryable<R_TO_DETAIL, C_SHIP_CUSTOMER>((rd, cs) => rd.DN_CUSTOMER == cs.CUSTOMERNAME).Where((rd, cs) => rd.DN_NO == dnNo && cs.ROUTENAME == "CZ" && cs.ROUTENAME == "HS").Select((rd, cs) => rd).Any();
            if (aa)
            {
                //單包
                bool bb = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno && r.MAX_QTY == 1).Any();
                bool cc = SFCDB.ORM.Queryable<R_FIFO_FGI>().Where((r) => r.PALLETNO == packno).Any();
                if (bb)
                {
                    if (!cc)
                    {
                        rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno).ToList().FirstOrDefault();
                        if (rpacking.SKUNO != null)
                        {
                            skuno = rpacking.SKUNO;
                        }
                        else
                        {
                            skuno = "";
                        }
                        woversion = WoBase.GetWOVersion(packno, SFCDB);

                        bool dd = SFCDB.ORM.Queryable<R_F_CONTROL>().Where((r) => r.VALUE == skuno && r.FUNCTIONNAME == "NOCHECKSKU_SHIPPING" && r.CONTROLFLAG == "Y").Any();
                        if (woversion.Count > 1 && !dd)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526102531", new string[] { packno }));
                        }
                        else
                        {
                            R_SN_STATION_DETAIL sn = SFCDB.ORM.Queryable<R_SN_STATION_DETAIL, R_WO_BASE, R_PACKING, R_PACKING, R_SN_PACKING, R_SN>
                                ((rsd, rwb, rp1, rp2, rsp, rsn) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rsp.SN_ID == rsn.ID && rsn.WORKORDERNO == rwb.WORKORDERNO && rsn.ID == rsd.R_SN_ID)
                                .Where((rsd, rwb, rp1, rp2, rsp, rsn) => rp1.PACK_NO == packno && rsd.STATION_NAME == "CBS").Select((rsd, rwb, rp1, rp2, rsp, rsn) => (rsd))
                                .OrderBy(rsd => rsd.EDIT_TIME, SqlSugar.OrderByType.Asc).ToList().FirstOrDefault();

                            R_SN rsnsn = SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == sn.SN).ToList().FirstOrDefault();

                            R_WO_BASE woversion1 = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == rsnsn.WORKORDERNO).ToList().FirstOrDefault();

                            rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno).ToList().FirstOrDefault();
                            R_WO_BASE woversion2 = SFCDB.ORM.Queryable<R_WO_BASE, R_SN>((rwb, rsn) => rwb.WORKORDERNO == rsn.WORKORDERNO)
                                      .Where((rwb, rsn) => rsn.SHIPPED_FLAG == "0" && rsn.CURRENT_STATION == "CBS" && rsn.SN.Substring(1, 2) != "RW" && rwb.SKUNO == rpacking.SKUNO)
                                      .Select((rwb) => rwb)
                                      .OrderBy(rwb => rwb.SKU_VER, SqlSugar.OrderByType.Asc).ToList().FirstOrDefault();
                            if (woversion1.SKU_VER != woversion2.SKU_VER)
                            {
                                R_SN oldwo = SFCDB.ORM.Queryable<R_SN>().Where(r => r.WORKORDERNO == woversion2.WORKORDERNO && r.SHIPPED_FLAG == "0" && r.CURRENT_STATION == "CBS").ToList().FirstOrDefault();
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526101842", new string[] { oldwo.WORKORDERNO }));
                            }
                        }
                    }
                }
                else //多包
                {
                    if (!cc)
                    {
                        rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == packno).ToList().FirstOrDefault();
                        //woversion = SFCDB.ORM.Queryable<R_WO_BASE, R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((rwb, rp1, rp2, rsp, rsn) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rsp.SN_ID == rsn.ID && rsn.WORKORDERNO == rwb.WORKORDERNO)
                        //    .Where((rwb, rp1, rp2, rsp, rsn) => rp1.PACK_NO == packno).Select((rwb, rp1, rp2, rsp, rsn)=>(rwb)).ToList();
                        woversion = WoBase.GetWOVersion(packno, SFCDB);
                        if (woversion.Count > 1)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526102531", new string[] { packno }));
                        }
                        else
                        {

                            R_SN _rsn = SFCDB.ORM.Queryable<R_SN, R_PACKING, R_PACKING, R_SN_PACKING>((rsn, rp1, rp2, rsp) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rsp.SN_ID == rsn.ID)
                                .Where((rsn, rp1, rp2, rsp) => rp1.PACK_NO == packno && rsn.VALID_FLAG == "1")
                                .Select((rsn) => (rsn))
                                .ToList().FirstOrDefault();
                            R_WO_BASE _woversion = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == _rsn.WORKORDERNO).ToList().FirstOrDefault();
                            R_WO_BASE _wover = SFCDB.ORM.Queryable<R_WO_BASE, R_SN>((rwb, rsn) => rwb.WORKORDERNO == rsn.WORKORDERNO && rsn.SHIPPED_FLAG == "0" && rsn.CURRENT_STATION == "CBS" && rsn.SN.Substring(1, 2) != "RW")
                                .Where((rwb, rsn) => rwb.SKUNO == _rsn.SKUNO)
                                .Select((rwb) => rwb)
                                .ToList().FirstOrDefault();
                            if (_woversion.SKU_VER != _wover.SKU_VER)
                            {
                                R_SN oldwo = SFCDB.ORM.Queryable<R_SN>().Where(r => r.WORKORDERNO == _wover.WORKORDERNO && r.SHIPPED_FLAG == "0" && r.CURRENT_STATION == "CBS").ToList().FirstOrDefault();
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526101842", new string[] { oldwo.WORKORDERNO }));
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 檢查該棧板中有板子與出貨號碼有沒有綁定關系(或者有沒有打印 OverPack Label)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckHaveOverPack(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            string packno = packSession.Value.ToString();
            R_PACKING rpacking = null;
            R_DN_STATUS DNList = null;

            rpacking = SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == packno).First();
            string skuno = rpacking.SKUNO.ToString();
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();
            string dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString();
            R_STATION_AUTOSCAN Autoscan = SFCDB.ORM.Queryable<R_STATION_AUTOSCAN>().Where((r) => r.SKUNO == skuno && r.CURRENTPOINT == "OVERPACKLABEL").ToList().FirstOrDefault();
            C_PACKING Packing = SFCDB.ORM.Queryable<C_PACKING>().Where((c) => c.SKUNO == skuno && c.PACK_TYPE == "PALLET" && c.MAX_QTY == 1).ToList().FirstOrDefault();
            DNList = SFCDB.ORM.Queryable<R_DN_STATUS>().Where((c) => c.DN_NO == dnNo && c.DN_LINE == dnLine).ToList().FirstOrDefault();
            if (Autoscan != null && Packing != null)
            {
                bool CheckSn = SFCDB.ORM.Queryable<R_ORDER_SN, R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((ros, rp1, rp2, rsp, rs) => rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID && rsp.SN_ID == rs.ID && rs.SN == ros.SN)
                    .Where((ros, rp1, rp2, rsp, rs) => ros.DNID == DNList.ID && ros.SKUNO == skuno).ToList().Any();
                if (CheckSn)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200526101125", new string[] { packno }));
                }
            }

        }


        public static void CheckMoveStatusIsEqual(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject_1 = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject_1 == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionPackObject_1.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionPackObject_2 = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackObject_2 == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (sessionPackObject_2.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            try
            {
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
                LogicObject.Packing packObject_1 = (LogicObject.Packing)sessionPackObject_1.Value;
                LogicObject.Packing packObject_2 = (LogicObject.Packing)sessionPackObject_2.Value;
                if (packObject_1.ClosedFlag == "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613090152", new string[] { packObject_1.PackNo }));
                }
                if (packObject_2.ClosedFlag == "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613090152", new string[] { packObject_2.PackNo }));
                }
                if (!packObject_1.PackType.Equals(packObject_2.PackType))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612114250", new string[] { packObject_1.PackNo, packObject_2.PackNo }));
                }
                if (!packObject_1.Skuno.Equals(packObject_2.Skuno))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141141", new string[] { packObject_1.PackNo, packObject_2.PackNo }));
                }
                if (!packObject_1.SkunoVer.Equals(packObject_2.SkunoVer))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141356", new string[] { packObject_1.PackNo, packObject_2.PackNo }));
                }
                List<R_SN> snList_1 = new List<R_SN>();
                List<R_SN> snList_2 = new List<R_SN>();
                List<string> stationList_1 = new List<string>();
                List<string> stationList_2 = new List<string>();
                if (packObject_1.PackType == LogicObject.PackType.PALLET.ToString().ToUpper())
                {
                    snList_1 = t_r_packing.GetPakcingSNList(packObject_1.PackID, LogicObject.PackType.PALLET.ToString().ToUpper(), Station.SFCDB);
                }
                else if (packObject_1.PackType == LogicObject.PackType.CARTON.ToString().ToUpper())
                {
                    snList_1 = t_r_packing.GetPakcingSNList(packObject_1.PackID, LogicObject.PackType.CARTON.ToString().ToUpper(), Station.SFCDB);
                }

                if (packObject_2.PackType == LogicObject.PackType.PALLET.ToString().ToUpper())
                {
                    snList_2 = t_r_packing.GetPakcingSNList(packObject_2.PackID, LogicObject.PackType.PALLET.ToString().ToUpper(), Station.SFCDB);
                }
                else if (packObject_2.PackType == LogicObject.PackType.CARTON.ToString().ToUpper())
                {
                    snList_2 = t_r_packing.GetPakcingSNList(packObject_2.PackID, LogicObject.PackType.CARTON.ToString().ToUpper(), Station.SFCDB);
                }

                if (snList_1 != null && snList_1.Count > 0)
                {
                    stationList_1 = snList_1.Select(s => s.NEXT_STATION).Distinct().ToList();
                }
                if (snList_2 != null && snList_2.Count > 0)
                {
                    stationList_2 = snList_2.Select(s => s.NEXT_STATION).Distinct().ToList();
                }

                if (stationList_1.Count != 0 && stationList_1.Count != 1)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180920162543", new string[] { packObject_1.PackNo }));
                }

                if (stationList_2.Count != 0 && stationList_2.Count != 1)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180920162543", new string[] { packObject_2.PackNo }));
                }

                if (stationList_1.Count != 0 && stationList_2.Count != 0 && stationList_1[0].ToString() != stationList_2[0].ToString())
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180920162739", new string[] { packObject_1.PackNo, packObject_2.PackNo }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void CheckEmpPremissionMoveCartonPallet(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionPackObject_1 = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject_1 == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionPackObject_1.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionPackObject_2 = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackObject_2 == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (sessionPackObject_2.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
            LogicObject.Packing packObject_1 = (LogicObject.Packing)sessionPackObject_1.Value;
            LogicObject.Packing packObject_2 = (LogicObject.Packing)sessionPackObject_2.Value;
            List<R_SN> snList_1 = new List<R_SN>();
            List<R_SN> snList_2 = new List<R_SN>();
            try
            {
                if (packObject_1.PackType == LogicObject.PackType.PALLET.ToString().ToUpper())
                {
                    snList_1 = t_r_packing.GetPakcingSNList(packObject_1.PackID, LogicObject.PackType.PALLET.ToString().ToUpper(), Station.SFCDB);
                }
                else if (packObject_1.PackType == LogicObject.PackType.CARTON.ToString().ToUpper())
                {
                    snList_1 = t_r_packing.GetPakcingSNList(packObject_1.PackID, LogicObject.PackType.CARTON.ToString().ToUpper(), Station.SFCDB);
                }

                if (packObject_2.PackType == LogicObject.PackType.PALLET.ToString().ToUpper())
                {
                    snList_2 = t_r_packing.GetPakcingSNList(packObject_2.PackID, LogicObject.PackType.PALLET.ToString().ToUpper(), Station.SFCDB);
                }
                else if (packObject_2.PackType == LogicObject.PackType.CARTON.ToString().ToUpper())
                {
                    snList_2 = t_r_packing.GetPakcingSNList(packObject_2.PackID, LogicObject.PackType.CARTON.ToString().ToUpper(), Station.SFCDB);
                }
                if (snList_2.Count == 0)
                {
                    string wip_group1 = snList_1[0].NEXT_STATION.ToString();
                    if (wip_group1 == "SHIPOUT")
                    {
                        T_c_user c_user_c = new T_c_user(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        List<string> dep = new List<string>();
                        string emp_no = Station.LoginUser.EMP_NO.ToString();
                        dep = c_user_c.GetDptName(emp_no, Station.SFCDB);
                        if (dep[0] != "WHS")
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210831132408", new string[] { emp_no }));
                        }
                    }
                }
                else if (snList_1.Count == 0)
                {
                    string wip_group2 = snList_2[0].NEXT_STATION.ToString();
                    if (wip_group2 == "SHIPOUT")
                    {
                        T_c_user c_user_c = new T_c_user(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        List<string> dep = new List<string>();
                        string emp_no = Station.LoginUser.EMP_NO.ToString();
                        dep = c_user_c.GetDptName(emp_no, Station.SFCDB);
                        if (dep[0] != "WHS")
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210831132408", new string[] { emp_no }));
                        }
                    }
                }
                else
                {
                    string wip_group1 = snList_1[0].NEXT_STATION.ToString();
                    string wip_group2 = snList_2[0].NEXT_STATION.ToString();
                    if (wip_group1 == "SHIPOUT" || wip_group2 == "SHIPOUT")
                    {
                        T_c_user c_user_c = new T_c_user(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        List<string> dep = new List<string>();
                        string emp_no = Station.LoginUser.EMP_NO.ToString();
                        dep = c_user_c.GetDptName(emp_no, Station.SFCDB);
                        if (dep[0] != "WHS")
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210831132408", new string[] { emp_no }));
                        }
                    }
                }
                
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        /// <summary>
        /// 檢查包裝單位裡面的產品狀態是否一致並且與當前站位一致
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackItemStatusSameChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            string PackNO = PackNOSession.Value.ToString();
            T_R_SN TRS = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN> SNList = new List<R_SN>();
            TRS.GetSNsByPackNo(PackNO, ref SNList, Station.SFCDB);

            if (SNList.Any(t => t.NEXT_STATION != Station.StationName))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528095410", new string[] { PackNO, Station.StationName }));
            }

        }

        /// <summary>
        /// Check Next Station SN with Next Station in Pallet list - 2021/04/05 add by niemnv
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckNextStationInPallet(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession PackNOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackNOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }

            //lay thong SN
            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            SN SNObj = (SN)sessionSn.Value;
            var next_station = rSn.GetNextStation(SNObj.RouteID, SNObj.NextStation, Station.SFCDB);

            string PackNO = PackNOSession.Value.ToString();
            if (PackNO.Contains('('))
            {
                PackNO = PackNO.Remove(PackNO.LastIndexOf('('));
            }
            T_R_SN TRS = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN> SNList = new List<R_SN>();
            TRS.GetSNsByPackNo(PackNO, ref SNList, Station.SFCDB);

            if (SNList.Any(t => t.NEXT_STATION != next_station && t.PACKED_FLAG == "1"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210406093302", new string[] { SNObj.SerialNo, next_station, PackNO }));
            }
        }

        /// <summary>
        /// 檢查包裝單位是否已經關閉以進行包裝
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackHasClosedChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            string PackNo = PackNoSession.Value.ToString();
            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (!PackingTool.CheckCloseByPackno(PackNo, Station.SFCDB))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181008115525", new string[] { PackNo }));
            }
        }

        /// <summary>
        /// 檢查棧板是否已經關閉，已經關閉再需要掃描棧板內的CARTON||SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackHasClosedOrNot(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            var RemovePrefix = Paras.FindAll(t => t.SESSION_TYPE.ToUpper() == "REMOVEPREFIX");


            string modeltype = "";
            if (Paras.Count > 1)
            {
                MESStationSession ModelTypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (ModelTypeSession != null)
                {
                    modeltype = ModelTypeSession.Value.ToString();
                }
                modeltype = modeltype == null ? "" : modeltype;
            }

            string PackNo = PackNoSession.Value.ToString();
            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (!PackingTool.CheckCloseByPackno(PackNo, Station.SFCDB))
            {
                //throw new MESReturnMessage($@"{PackNo} Not Closed, Please Check!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143020", new string[] { PackNo }));
            }
            //調出輸入carton||sn的對話框
            UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "CartonOrSn", Tittle = "請掃描Carton或者Sn", Type = UIInputType.String, Name = "CartonOrSn", ErrMessage = "Cancel" };
            //I.OutInputs.Add(new DisplayOutPut() { Name = "Description", DisplayType = EnumHelper.GetEnumName(UIOutputType.Text), Value = rfai.REMARK });

            var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);

            string r = ret.ToString();
            for (int i = 0; i < RemovePrefix.Count; i++)
            {

                if (r.StartsWith(RemovePrefix[i].VALUE))
                {
                    r = r.Substring(RemovePrefix[i].VALUE.Length);
                    break;
                }
            }
            ret = r;

            #region 增加掃描時針對SE機種暗碼多前綴的處理邏輯 Edit By ZHB 2020年7月24日18:20:59
            string strInput = ret.ToString();
            //string skuNo = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).Select(t => t.SKUNO).First();
            R_PACKING rpacking = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).First();
            string skuNo = rpacking.SKUNO;
            if (string.IsNullOrEmpty(skuNo))
            {
                //throw new MESReturnMessage($@"Get SkuNo Fail, Please Check!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143054"));
            }

            T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            strInput = c_sku_detail.SNPreprocessor(Station.SFCDB, skuNo, strInput, "SHIPOUT");

            //var categorylist = Station.SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == skuNo && t.CATEGORY == "SN-PREPROCESSOR").ToList();
            //for (int i = 0; i < categorylist.Count; i++)
            //{
            //    switch (categorylist[i].CATEGORY_NAME)
            //    {
            //        case "REPLACE":
            //            strInput = strInput.Replace(categorylist[i].VALUE, categorylist[i].EXTEND);
            //            break;
            //        case "ADD-SUFFIX":
            //            strInput = strInput.Insert(strInput.Length, categorylist[i].VALUE);
            //            break;
            //        case "ADD-PREFIX":
            //            strInput = strInput.Insert(0, categorylist[i].VALUE);
            //            break;
            //        case "REMOVE-SUFFIX":
            //            if (strInput.EndsWith(categorylist[i].VALUE))
            //            {
            //                strInput = strInput.Remove(strInput.Length - categorylist[i].VALUE.Length);
            //            }
            //            break;
            //        case "REMOVE-PREFIX":
            //            if (strInput.StartsWith(categorylist[i].VALUE))
            //            {
            //                strInput = strInput.Remove(0, categorylist[i].VALUE.Length);
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}
            #endregion

            string packlevel = "";
            if (rpacking.PARENT_PACK_ID == null || rpacking.PARENT_PACK_ID == "")
            {
                //單包
                packlevel = "1";
            }
            else
            {
                //多包
                packlevel = "2";
            }

            if (modeltype.IndexOf("112") >= 0)
            {
                //Telefonica 部分機種掃MAC4作為入庫卡通檢查.
                bool res = false;
                if (packlevel == "1")
                {
                    res = Station.SFCDB.ORM.Queryable<R_PACKING, R_SN, R_SN_PACKING, R_CUSTSN_T>((rp, rn, rsp, rcust) =>
                        rp.ID == rsp.PACK_ID && rsp.SN_ID == rn.ID && rn.SN == rcust.SERIAL_NUMBER)
                        .Where((rp, rn, rsp, rcust) => rp.PACK_NO == PackNo && rn.VALID_FLAG == "1" && rcust.MAC4.Replace(" ", "") == strInput)
                        .Select((rp, rn, rsp, rcust) => rn)
                        .Any();
                }
                else
                {
                    res = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING, R_CUSTSN_T>((rp1, rp2, rn, rsp, rcust) =>
                        rp1.PACK_NO == rp2.PARENT_PACK_ID && rp1.ID == rsp.PACK_ID && rsp.SN_ID == rn.ID && rn.SN == rcust.SERIAL_NUMBER)
                        .Where((rp1, rp2, rn, rsp, rcust) => rp2.PACK_NO == PackNo && rn.VALID_FLAG == "1" && rcust.MAC4.Replace(" ", "") == strInput)
                        .Select((rp1, rp2, rn, rsp) => rn)
                        .Any();
                }
                if (res == false)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181008115525", new string[] { strInput }));
                }
            }
            else
            {
                T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
                R_PACKING CartonAndSnInPack = TRP.GetPackingBySNorCarton(strInput, PackNo, Station.SFCDB);
                if (CartonAndSnInPack == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181008115525", new string[] { strInput }));
                }
            }
        }
        public static void CheckPoByPackno(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            string modeltype = "";
            if (Paras.Count > 1)
            {
                MESStationSession ModelTypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (ModelTypeSession != null)
                {
                    modeltype = ModelTypeSession.Value.ToString();
                }
                modeltype = modeltype == null ? "" : modeltype;
            }

            string PackNo = PackNoSession.Value.ToString();

            var res = Station.SFCDB.ORM.Queryable<R_PACKING, R_SN, R_SN_PACKING, R_PACKING, O_ORDER_MAIN>((rp, rn, rsp, rk, om) =>
                  rp.ID == rsp.PACK_ID && rsp.SN_ID == rn.ID && rk.ID == rp.PARENT_PACK_ID && rn.WORKORDERNO == om.PREWO)
                  .Where((rp, rn, rsp, rk, om) => rk.PACK_NO == PackNo)
                  .Select((rp, rn, rsp, rk, om) => om).ToList();
            if (res.Count > 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210316164351", new string[] { PackNo }));
            }

        }

        /// <summary>
        /// 檢查棧板中的工單在BURNIN工站最大老化時間的數量是否夠
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackWoBurninTimeChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (packSession == null || packSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            try
            {
                string pack = packSession.Value.ToString();
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                R_PACKING r_packing = t_r_packing.GetPackingByPackNo(pack, Station.SFCDB);
                T_R_WO_AGEING t_r_wo_ageing = new T_R_WO_AGEING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_TEST_DETAIL_VERTIV t_r_test_detail = new T_R_TEST_DETAIL_VERTIV(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                R_WO_AGEING r_wo_ageing = null;
                //T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                //R_SN_LOCK r_sn_lock = null;
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_SN> keypartSNList;
                List<string> keypartWOList;
                R_WO_AGEING keypartWoAging = null;
                if (r_packing == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { pack }));
                }
                List<R_SN> snList = t_r_packing.GetPakcingSNList(r_packing.ID, "PALLET", Station.SFCDB);
                List<string> woList = snList.Select(s => s.WORKORDERNO).Distinct().ToList();
                foreach (var w in woList)
                {
                    r_wo_ageing = t_r_wo_ageing.GetWoAgeingObject(Station.SFCDB, w);
                    if (r_wo_ageing != null)
                    {
                        List<R_TEST_DETAIL_VERTIV> testList1 = t_r_test_detail.GetTestStationQtyByWo(w, "BURNIN", Station.SFCDB);
                        List<R_TEST_DETAIL_VERTIV> testList = t_r_test_detail.GetTestStationQtyByWo(w, "BURNIN", Station.SFCDB).FindAll(t => t.BURNIN_TIME == r_wo_ageing.MAX_AGEING_TIME);
                        if (testList.Count < Convert.ToInt32(r_wo_ageing.MAX_TIME_QTY))
                        {
                            //r_sn_lock = t_r_sn_lock.GetLockObject("", "WO", "", w, "BURNIN_QTY_ERROR", Station.StationName, Station.SFCDB);
                            //if (r_sn_lock == null)
                            //{
                            //    t_r_sn_lock.AddNewLock(Station.BU, "", "WO", "", w, Station.StationName, "BURNIN_QTY_ERROR", "SYSTEM", Station.SFCDB);
                            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181026153659",
                            //        new string[] { pack, w, "BURNIN", testList.Count.ToString(), r_wo_ageing.MAX_TIME_QTY }));
                            //}
                            //if (r_sn_lock.LOCK_STATUS == "1")
                            //{
                            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181026153659",
                            //        new string[] { pack, w, "BURNIN", testList.Count.ToString(), r_wo_ageing.MAX_TIME_QTY }));
                            //}
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181026153659",
                                new string[] { pack, w, testList.Count.ToString(), r_wo_ageing.MAX_TIME_QTY }));
                        }
                    }
                    else
                    {
                        keypartSNList = t_r_sn_kp.GetKeypartSnListByWo(w, Station.SFCDB);
                        keypartWOList = keypartSNList.Select(s => s.WORKORDERNO).Distinct().ToList();
                        foreach (string wo in keypartWOList)
                        {
                            keypartWoAging = t_r_wo_ageing.GetWoAgeingObject(Station.SFCDB, w);
                            if (keypartWoAging != null)
                            {
                                List<R_TEST_DETAIL_VERTIV> testList = t_r_test_detail.GetTestStationQtyByWo(w, "BURNIN", Station.SFCDB).FindAll(t => t.BURNIN_TIME == r_wo_ageing.MAX_AGEING_TIME);
                                if (testList.Count < Convert.ToInt32(r_wo_ageing.MAX_TIME_QTY))
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181026153659",
                                        new string[] { pack, w, r_wo_ageing.MAX_TIME_QTY, testList.Count.ToString(), r_wo_ageing.MAX_TIME_QTY }));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查掃入的 Item 以及設定下一個動作該做什麼
        /// SN 1
        /// SKU 1
        /// SCAN 1
        /// CURRENTITEM 1
        /// ITEMS 1
        /// SN 輸入框的名字（設定 value）
        /// Item 輸入框的名字（設定 value）
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackingScanItemChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_WO_DEVIATION TRWD = new T_R_WO_DEVIATION(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            if (Paras.Count < 7)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession Snsession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Snsession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN Sn = (SN)Snsession.Value;
            R_WO_DEVIATION Deviation = TRWD.GetDeviationByWo(Sn.WorkorderNo, Station.SFCDB);

            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                //報錯
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            SKU Sku = (SKU)SkuSession.Value;

            MESStationSession ScanSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ScanSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                //報錯
            }
            string ScanItem = ScanSession.Value.ToString();

            MESStationSession CurrentItemSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (CurrentItemSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                //報錯
            }
            string CurrentItem = CurrentItemSession.Value.ToString();

            MESStationSession ItemsSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (ItemsSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                //報錯
            }
            System.Data.DataTable ItemsTable = (System.Data.DataTable)ItemsSession.Value;
            //Dictionary<int, string> Items = new Dictionary<int, string>();

            if (CurrentItem.ToUpper().Equals("PN VER"))
            {
                string Customer = Station.SFCDB.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((csku, cseries, ccustomer) => csku.C_SERIES_ID == cseries.ID && cseries.CUSTOMER_ID == ccustomer.ID)
                    .Where((csku, cseries, ccustomer) => csku.SKUNO.Equals(Sku.SkuNo)).Select((csku, cseries, ccustomer) => ccustomer.CUSTOMER_NAME).ToList().FirstOrDefault();
                if (!Customer.Equals(""))
                {
                    if (Customer.StartsWith("B") || Customer.Equals("RCFA"))
                    {
                        if (!ScanItem.Equals(Sku.SkuNo))
                        {
                            bool flag = false;
                            T_C_KEYPART TCK = new T_C_KEYPART(Station.SFCDB, Station.DBType);
                            List<CKeyPart> Keyparts = TCK.GetAllKeyparts(Sku.SkuNo, "", Station.SFCDB);
                            foreach (CKeyPart Keypart in Keyparts)
                            {
                                if (ScanItem.Equals(Keypart.PART_NO))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181030150228", new string[] { CurrentItem }));
                            }
                        }
                    }
                    else
                    {
                        if (!ScanItem.Equals(Sku.SkuNo + " " + Sku.Version))
                        {
                            bool flag = false;
                            T_C_KEYPART TCK = new T_C_KEYPART(Station.SFCDB, Station.DBType);
                            List<CKeyPart> Keyparts = TCK.GetAllKeyparts(Sku.SkuNo, "", Station.SFCDB);
                            foreach (CKeyPart Keypart in Keyparts)
                            {
                                if (ScanItem.Equals(Keypart.PART_NO + " " + Keypart.PART_NO_VER))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181030150228", new string[] { CurrentItem }));
                            }

                            //報錯
                        }
                    }
                }
            }
            if (CurrentItem.ToUpper().Equals("CN"))
            {
                if (!ScanItem.Equals("CN"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181030150228", new string[] { CurrentItem }) + MESReturnMessage.GetMESReturnMessage("MES00000190"));
                    //報錯
                }
            }
            if (CurrentItem.ToUpper().Equals("DEVIATION"))
            {
                if (Deviation != null && !ScanItem.Equals(Deviation.DEVIATION))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181030150228", new string[] { CurrentItem }));
                    //報錯
                }
            }

            //不是最後一個 Item 就報錯，如果是的話，那麼就將下一個輸入改成 SN，然後繼續後面的步驟
            foreach (System.Data.DataRow dr in ItemsTable.Rows)
            {
                if (CurrentItem.Equals(dr["Item"]))
                {
                    if (!ItemsTable.Rows[ItemsTable.Rows.Count - 1]["Index"].Equals(dr["Index"]))
                    {
                        CurrentItemSession.Value = ItemsTable.Rows[(Int32.Parse(dr["Index"].ToString()))]["Item"];
                        Station.FindInputByName(Paras[6].VALUE).Value = "";
                        Station.NextInput = Station.FindInputByName(Paras[6].VALUE);
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181030150529", new string[] { CurrentItemSession.Value.ToString() }));
                    }
                    else
                    {
                        Station.FindInputByName(Paras[6].VALUE).Value = "";
                        Station.NextInput = Station.FindInputByName(Paras[5].VALUE);
                    }
                }
            }
        }

        /// <summary>
        /// 檢查產品的機種版本 和 工單類型是否與包裝單位內的產品一致
        /// SN 1
        /// PACKNO 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SkuVersionAndWoTypeChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_WO_BASE TRWB = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                //報錯
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN Sn = new SN();
            if (SnSession.Value is SN)
            {
                Sn = (SN)SnSession.Value;
            }
            else
            {
                Sn.Load(SnSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackNoSession == null)
            {
                //報錯
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string PackNo = PackNoSession.Value.ToString();
            //fuck
            if (PackNo.IndexOf('(') > 0)
            {
                PackNo = PackNo.Substring(0, PackNo.IndexOf('('));
            }

            //檢查機種版本是否一致
            //檢查工單類型是否一致
            List<R_SN> SnList = new List<R_SN>();
            TRP.GetSNByPackNo(PackNo, ref SnList, Station.SFCDB);

            R_WO_BASE WoBase = TRWB.GetWoByWoNo(Sn.WorkorderNo, Station.SFCDB);
            SnList.ForEach(t =>
            {
                R_WO_BASE TempBase = TRWB.GetWoByWoNo(t.WORKORDERNO, Station.SFCDB);
                if (!(TempBase.SKUNO.Equals(WoBase.SKUNO) && TempBase.SKU_VER.Equals(WoBase.SKU_VER) && TempBase.WO_TYPE.Equals(WoBase.WO_TYPE)))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103150603", new string[] { Sn.SerialNo, PackNo }));
                }
            });


        }

        /// <summary>
        /// Check The COO Of The SN Must Be The Same As The COO Of SN In Package
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNCOOMatchChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                //報錯
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN Sn = new SN();
            if (SnSession.Value is SN)
            {
                Sn = (SN)SnSession.Value;
            }
            else
            {
                Sn.Load(SnSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackNoSession == null)
            {
                //報錯
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string PackNo = PackNoSession.Value.ToString();
            if (PackNo.IndexOf('(') > 0)
            {
                PackNo = PackNo.Substring(0, PackNo.IndexOf('('));
            }
            var Skutype = Station.SFCDB.ORM.Queryable<R_SN, O_ORDER_MAIN>((r, o) => r.WORKORDERNO == o.PREWO).Where((r, o) => r.SN == Sn.baseSN.SN && r.VALID_FLAG == "1").Select((r, o) => o).First();
            if (Skutype != null)
            {
                if (Skutype.POTYPE == "CTO")
                {
                    List<R_SN_KP> KpList = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == Sn.baseSN.ID && t.KP_NAME.Substring(0, 6).ToUpper() == "AUTOKP" && t.SCANTYPE.Contains("SN")).ToList();

                    foreach (var kpvalue in KpList)
                    {
                        var I244List = Station.SFCDB.ORM.Queryable<R_I244>().Where(t => t.PARENTSN == Sn.baseSN.SN).ToList();
                        if (I244List == null)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143119", new string[] { Sn.baseSN.SN }));
                        }
                        if (I244List.FindAll(t=>t.SN==kpvalue.VALUE).Count()==0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143118", new string[] { kpvalue.VALUE }));
                        }
                    }
                }
            }
            var coo = Station.SFCDB.ORM.Queryable<R_SN_KP>()
                .Where(t => t.SN == Sn.baseSN.SN && t.VALUE == Sn.baseSN.SN && t.VALID_FLAG == 1)
                .Select(t => t.LOCATION)
                .ToList()
                .FirstOrDefault();
            if (coo == null || coo == "")
            {
                return;
            }
            var kplist = Station.SFCDB.ORM.Queryable<R_SN_KP, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((KP, SN, SP, P, PP) => KP.VALUE == KP.SN && KP.SN == SN.SN && SN.ID == SP.SN_ID && SP.PACK_ID == P.ID && P.PARENT_PACK_ID == PP.ID)
                .Where((KP, SN, SP, P, PP) => KP.KP_NAME == "AutoKP" && PP.PACK_NO == PackNo)
                .Select((KP, SN, SP, P, PP) => KP.LOCATION)
                .ToList();
            if (kplist.Count > 0 && !kplist.Contains(coo))
            {
                //throw new MESReturnMessage("Units COO within a pallet must be the same!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143117"));
            }


        }


        /// <summary>
        /// 檢查包裝單位內機種版本是否一致
        /// ADD BY HGB 2019.06.11,已有方法,但是傳的參數不適合,重新寫
        /// SN,PALLET     
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSkuVersion(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_WO_BASE TRWB = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                //報錯
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN Sn = new SN();
            if (SnSession.Value is SN)
            {
                Sn = (SN)SnSession.Value;
            }
            else
            {
                Sn.Load(SnSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackNoSession == null)
            {
                //報錯
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string PackNo = PackNoSession.Value.ToString().Split('(')[0].ToString();
            #endregion 獲取傳入參數

            //檢查機種版本是否一致
            //檢查工單類型是否一致
            List<R_SN> SnList = new List<R_SN>();
            TRP.GetSNByPackNo(PackNo, ref SnList, Station.SFCDB);

            R_WO_BASE WoBase = TRWB.GetWoByWoNo(Sn.WorkorderNo, Station.SFCDB);
            SnList.ForEach(t =>
            {
                R_WO_BASE TempBase = TRWB.GetWoByWoNo(t.WORKORDERNO, Station.SFCDB);
                if (!(TempBase.SKUNO.Equals(WoBase.SKUNO) && TempBase.SKU_VER.Equals(WoBase.SKU_VER)))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103150603", new string[] { Sn.SerialNo, PackNo }));
                }
            });


        }

        /// <summary>
        /// 檢查SN與卡通的ROHS狀態是否一致
        /// ADD BY HGB 2019.06.11
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckRohsByCartion(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                //throw new System.Exception("sessionSN miss ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142442"));
            }

            MESStationSession sessionCartion = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCartion == null)
            {
                //throw new System.Exception("sessionCartion miss ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143140"));
            }
            #endregion 獲取傳入參數
            try
            {
                //取得卡通號
                string packno = sessionCartion.Value.ToString().Split('(')[0].ToString();

                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
                // 檢查SN與卡通的ROHS狀態是否一致
                t_r_wo_base.CheckRohsBySnCarton(sessionSN.Value.ToString(), packno, Station.SFCDB);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查SN與棧板的ROHS狀態是否一致
        /// ADD BY HGB 2019.06.11
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckRohsByPallet(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new System.Exception("sessionSN miss ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142442"));
            }

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionCartion miss ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143140"));
            }
            #endregion 獲取傳入參數

            try
            {
                //取得棧板號
                string packno = sessionPallet.Value.ToString().Split('(')[0].ToString();
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
                // 檢查SN與卡通的ROHS狀態是否一致
                t_r_wo_base.CheckRohsBySnPallet(sessionSN.Value.ToString(), packno, Station.SFCDB);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查ControlRun工單是否混裝
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckControlRunNo_mixed(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            string WO = sessionWO.Value.ToString();

            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCarton == null)
            {
                throw new System.Exception("sessionCartion miss ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143140"));
            }
            string Carton = sessionCarton.Value.ToString().Split('(')[0].ToString();

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionCartion miss ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143140"));
            }
            string Pallet = sessionPallet.Value.ToString().Split('(')[0].ToString();
            #endregion

            DisplayOutPut Dis_Carton = Station.DisplayOutput.Find(t => t.Name == "Carton");
            try
            {
                T_C_CONTROL t_c_control = new T_C_CONTROL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                bool IsControlWO = t_c_control.ValueIsExist("NO_MIXED", WO, Station.SFCDB);
                if (IsControlWO)
                {
                    t_c_control.CheckSoloPack(WO, Carton, Pallet, Station.SFCDB);
                }
                else
                {
                    t_c_control.CheckExistsControlWo(Carton, Pallet, Station.SFCDB);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 檢查扣板在整機是否有綁定過
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckBounded(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                //throw new System.Exception("sessionSN miss ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142442"));
            }

            string SN = sessionSN.Value.ToString();

            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSKU == null)
            {
                //throw new System.Exception("sessionSKU miss ");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142559"));
            }

            string SKUNO = sessionSKU.Value.ToString();

            #endregion


            try
            {
                T_C_CONTROL t_c_control = new T_C_CONTROL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                bool IsControlSKU = t_c_control.ValueIsExist("TC0012", SKUNO, Station.SFCDB);
                if (IsControlSKU)
                {
                    t_c_control.CheckBoundedRecord(SN, Station.SFCDB);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 檢查機種BUSINESS模式
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSkuBusinessType(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數

            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new System.Exception("sessionSKU miss ");
            }

            string SKUNO = sessionSKU.Value.ToString();

            #endregion

            try
            {
                T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                C_SKU_DETAIL dms1 = t_c_sku_detail.GetSkuDetail("SKUNOTYPE", "BUYSELL", SKUNO, Station.SFCDB);
                C_SKU_DETAIL dms2 = t_c_sku_detail.GetSkuDetail("SKUNOTYPE", "CONSIGN", SKUNO, Station.SFCDB);
                if (dms1 == null && dms2 == null)
                {
                    //throw new System.Exception("機種BUSINESS模式沒有設定[C_SKU_DETAIL]");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142613"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查機種鎖定鎖定(試製工單,試製機種被鎖定,僅有一個SN可以過卡通,LABEL_SIGN FOR HWT)
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSkuLock(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數

            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new System.Exception("sessionSN miss ");
            }

            string SN = sessionSN.Value.ToString();

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new System.Exception("sessionSN miss ");
            }

            string WO = sessionWO.Value.ToString();

            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new System.Exception("sessionSKU miss ");
            }

            string SKUNO = sessionSKU.Value.ToString();

            #endregion


            try
            {
                T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                //label system 鎖定檢查
                bool IsLock = t_r_sn_lock.IsLock("", "LABEL_SIGN", SKUNO, "", "CARTON", "", Station.SFCDB);
                if (IsLock) //試製機種被鎖定,僅有一個SN可以過卡通
                {

                    if (WO.Substring(0, 6) == "002281" || WO.Substring(0, 6) == "002282")//試製工單 
                    {

                        string PassSN = t_r_sn_station_detail.GetFirstPassSnBySnAndStation(WO, "CARTON", Station.SFCDB);
                        if (PassSN.Length > 0)//已過站的SN和現在的SN是否一樣，不一樣不可過站
                        {
                            if (PassSN != SN)//已過站的SN和現在的SN是否一樣，不一樣不可過站
                            {
                                //throw new System.Exception($@"此試製機種已鎖定,僅有一個SN可以過卡通,請用此SN:'{PassSN}',或者找PE/PQE去簽核解鎖其他SN過站");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143516", new string[] { PassSN }));
                            }
                        }

                    }

                }

                //正常的機種鎖定檢查
                t_r_sn_lock.CheckSkuLock(SKUNO, "SKU", "CARTON", Station.SFCDB);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// OBA檢查棧板是否要顯示RMACheck窗口,目前NN HWT有用
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OBAShowRMACheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackNO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackNO == null || sessionPackNO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionShow = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionShow == null)
            {
                sessionShow = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionShow);
            }
            sessionShow.Value = "NO";
            OleExec sfcdb = Station.SFCDB;
            SKU skuObj = new SKU();
            T_R_RMA_DETAIL TRRD = new T_R_RMA_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
            T_R_PACKING TRP = new T_R_PACKING(sfcdb, DB_TYPE_ENUM.Oracle);
            R_PACKING packingObj = TRP.GetBYPACKNO(sessionPackNO.Value.ToString(), sfcdb);
            List<R_SN> listSN = new List<R_SN>();
            if (packingObj != null)
            {
                if (skuObj.IsRMASkuno(sfcdb, packingObj.SKUNO))
                {
                    listSN = TRP.GetPakcingSNList(packingObj.ID, packingObj.PACK_TYPE, sfcdb);
                    foreach (R_SN sn in listSN)
                    {
                        if (!TRRD.IsCheck(sfcdb, sn.SN))
                        {
                            sessionShow.Value = "YES";
                            sessionPackNO.Value = "";
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// HWT OBA 每個棧板都要抽檢
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OBAAllPalletCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSMPQty = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSMPQty == null || sessionSMPQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            string IP = Station.IP;
            MESDataObject.Module.HWT.T_R_OBA_TEMP TROT = new MESDataObject.Module.HWT.T_R_OBA_TEMP(sfcdb, dbtype);
            List<MESDataObject.Module.HWT.R_OBA_TEMP> listSN = TROT.GetListByTypeAndIP(sfcdb, "SN", IP);
            int samplingQty = Convert.ToInt32(sessionSMPQty.Value);
            int passQty = listSN.Where(r => r.STATUS == "PASS").ToList().Count;
            int failQty = listSN.Where(r => r.STATUS == "FAIL").ToList().Count;

            if (samplingQty <= passQty + failQty)
            {
                List<MESDataObject.Module.HWT.R_OBA_TEMP> listOBAPack = TROT.GetListByTypeAndIP(sfcdb, "PALLET", IP);
                List<string> listPack = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_OBA_TEMP, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>
                    ((RO, RS, RSP, RP, RPG) => RO.VALUE == RS.SN && RS.ID == RSP.SN_ID && RSP.PACK_ID == RP.ID && RP.PARENT_PACK_ID == RPG.ID)
                    .Where((RO, RS, RSP, RP, RPG) => RO.TYPE == "SN" && RO.IP == IP && RS.VALID_FLAG == "1" && RP.PACK_TYPE == "CARTON" && RPG.PACK_TYPE == "PALLET")
                    .Select((RO, RS, RSP, RP, RPG) => RPG.PACK_NO).ToList().Distinct().ToList();

                if (listOBAPack.Count != listPack.Count)
                {
                    //throw new Exception("每個棧板都要抽檢");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142707"));
                }
            }
        }
        /// <summary>
        /// HWT OBA 尾數箱必須抽檢
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OBANotFullCartonCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSMPQty = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSMPQty == null || sessionSMPQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            string IP = Station.IP;
            MESDataObject.Module.HWT.T_R_OBA_TEMP TROT = new MESDataObject.Module.HWT.T_R_OBA_TEMP(sfcdb, dbtype);
            List<MESDataObject.Module.HWT.R_OBA_TEMP> listSN = TROT.GetListByTypeAndIP(sfcdb, "SN", IP);
            int samplingQty = Convert.ToInt32(sessionSMPQty.Value);
            int passQty = listSN.Where(r => r.STATUS == "PASS").ToList().Count;
            int failQty = listSN.Where(r => r.STATUS == "FAIL").ToList().Count;

            if (samplingQty <= passQty + failQty)
            {

                List<R_PACKING> listOBACarton = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_OBA_TEMP, R_PACKING, R_PACKING>
                    ((RO, RP, RPG) => RO.VALUE == RPG.PACK_NO && RP.PARENT_PACK_ID == RPG.ID)
                    .Where((RO, RP, RPG) => RO.TYPE == "PALLET" && RO.IP == IP && RP.MAX_QTY != RP.QTY).Select((RO, RP, RPG) => RP).ToList();
                bool bSamgling = true;
                List<string> listTempSN = new List<string>();
                foreach (R_PACKING p in listOBACarton)
                {
                    listTempSN = sfcdb.ORM.Queryable<R_SN_PACKING, R_SN, MESDataObject.Module.HWT.R_OBA_TEMP>((RP, RS, RO) => RP.SN_ID == RS.ID && RS.SN == RO.VALUE)
                        .Where((RP, RS, RO) => RP.PACK_ID == p.ID && RS.VALID_FLAG == "1" && RO.TYPE == "SN" && RO.IP == IP)
                        .Select((RP, RS, RO) => RS.SN).ToList();
                    if (listTempSN.Count == 0)
                    {
                        bSamgling = false;
                        break;
                    }
                }
                if (!bSamgling)
                {
                    throw new Exception("尾數箱必須抽檢");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142814"));
                }
            }
        }
        /// <summary>
        /// HWT OBA 顯示CHECK PASS章提示
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OBAShowPassTipCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSMPQty = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSMPQty == null || sessionSMPQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPassQty == null || sessionPassQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionFailQty == null || sessionFailQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession sessionSkuno = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionSkuno == null || sessionSkuno.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            MESStationSession sessionShowCheckPassTip = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionShowCheckPassTip == null)
            {
                sessionShowCheckPassTip = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = "", SessionKey = Paras[4].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionShowCheckPassTip);
            }

            int passQty = Convert.ToInt32(sessionPassQty.Value);
            int failQty = Convert.ToInt32(sessionFailQty.Value);
            int samplingQty = Convert.ToInt32(sessionSMPQty.Value);
            sessionShowCheckPassTip.Value = "NO";
            if (samplingQty <= passQty + failQty)
            {
                DB_TYPE_ENUM dbtype = Station.DBType;
                OleExec sfcdb = Station.SFCDB;
                string IP = Station.IP;
                string skuno = sessionSkuno.Value.ToString();

                T_C_CONTROL TCC = new T_C_CONTROL(sfcdb, dbtype);
                C_CONTROL objControl = TCC.GetNameValueTypeBySKU("TC_OBA_CP", skuno, "SKUNO", sfcdb);
                if (objControl != null)
                {
                    sessionShowCheckPassTip.Value = "YES";
                }
            }
        }
        /// <summary>
        /// HWT OBA 顯示貼發貨封籤提示
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OBAShowShipedTipCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionShowShipedTip = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionShowShipedTip == null)
            {
                sessionShowShipedTip = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionShowShipedTip);
            }
            string pallet = sessionPalletNo.Value.ToString();
            sessionShowShipedTip.Value = "NO";
            if (pallet.StartsWith("PLN"))
            {
                DB_TYPE_ENUM dbtype = Station.DBType;
                OleExec sfcdb = Station.SFCDB;
                T_R_PACKING TRP = new T_R_PACKING(sfcdb, dbtype);
                R_PACKING objPacking = TRP.GetBYPACKNO(pallet, sfcdb);
                if (objPacking == null)
                {
                    //throw new Exception(pallet + " The palletno is invalid!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143709", new string[] { pallet }));
                }

                if (objPacking.CLOSED_FLAG == "0")
                {
                    //throw new Exception(pallet + " The palletno is not closed");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143739", new string[] { pallet }));
                }
                T_C_CONTROL TCC = new T_C_CONTROL(sfcdb, dbtype);
                C_CONTROL objControl = TCC.GetNameValueTypeBySKU("TCOBA", objPacking.SKUNO, "SKUNO", sfcdb);
                if (objControl != null)
                {
                    sessionShowShipedTip.Value = "YES";
                }
            }
        }
        /// <summary>
        /// 檢查棧板或卡通是否在指定工站抽中，并PASS
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackNOIsPassOBACheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            string packno = "";
            string station = Paras[1].VALUE;
            if (string.IsNullOrEmpty(station))
            {
                //throw new Exception("Please input the station!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143806"));
            }
            if (sessionPalletNo.Value is LogicObject.Packing)
            {
                packno = ((LogicObject.Packing)sessionPalletNo.Value).PackNo;
            }
            else if (sessionPalletNo.Value is R_PACKING)
            {
                packno = ((R_PACKING)sessionPalletNo.Value).PACK_NO;
            }
            else
            {
                packno = sessionPalletNo.Value.ToString();
            }
            R_LOT_STATUS lotStatus = Station.SFCDB.ORM.Queryable<R_LOT_STATUS, R_LOT_PACK>((RLS, RLP) => RLS.LOT_NO == RLP.LOTNO)
                .Where((RLS, RLP) => RLP.PACKNO == packno && RLS.SAMPLE_STATION == station).Select((RLS, RLP) => RLS).ToList().FirstOrDefault();
            if (lotStatus == null)
            {
                //throw new Exception(packno + " not in sampling in " + station);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143834", new string[] { packno, station }));
            }
            if (lotStatus.LOT_STATUS_FLAG != "1")
            {
                //throw new Exception("The lot of " + packno + " not pass in " + station);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143916", new string[] { packno, station }));
            }
        }
        /// <summary>
        /// Check OBA Pass 章工站checker
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OBAPassStampCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionCheckQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCheckQty == null || sessionCheckQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string packno = ((LogicObject.Packing)sessionPalletNo.Value).PackNo;
            int checkQty = Convert.ToInt32(sessionCheckQty.Value);
            if (checkQty != ((LogicObject.Packing)sessionPalletNo.Value).SNList.Count)
            {
                //throw new Exception("Check Qty的值與Total Qty的值不一致！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144004"));
            }
            R_LOT_STATUS lotStatus = Station.SFCDB.ORM.Queryable<R_LOT_STATUS, R_LOT_PACK>((RLS, RLP) => RLS.LOT_NO == RLP.LOTNO)
                .Where((RLS, RLP) => RLP.PACKNO == packno && RLS.SAMPLE_STATION == "OBA").Select((RLS, RLP) => RLS).ToList().FirstOrDefault();
            if (lotStatus == null)
            {
                //throw new Exception("該Pallet No:" + packno + "未過OBA！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144026", new string[] { packno }));
            }
            if (lotStatus.LOT_STATUS_FLAG != "1")
            {
                //throw new Exception("該Pallet No:" + packno + "OBA未PASSED！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144051", new string[] { packno }));
            }
            if (lotStatus.EDIT_EMP == Station.LoginUser.EMP_NO)
            {
                //throw new Exception("該Pallet No:" + packno + " 由" + lotStatus.EDIT_EMP + "在OBA檢驗過站！OBA檢驗過站和確認PASS章不能同為一個人！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144125", new string[] { packno, lotStatus.EDIT_EMP }));
            }

        }
        /// <summary>
        /// HWT CBS 工站檢查是否蓋PASS章
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PalletStampingCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string palletNo = sessionPalletNo.Value.ToString();
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM DBType = Station.DBType;
            T_R_PACKING TRP = new T_R_PACKING(sfcdb, DBType);
            T_C_CONTROL TCC = new T_C_CONTROL(sfcdb, DBType);
            R_PACKING objPack = TRP.GetBYPACKNO(palletNo, sfcdb);
            MESDataObject.Module.HWT.T_R_OBA_STAMP_CHECK TROS = new MESDataObject.Module.HWT.T_R_OBA_STAMP_CHECK(Station.SFCDB, Station.DBType);
            C_CONTROL objControl = TCC.GetNameValueTypeBySKU("TC_OBA_CP", objPack.SKUNO, "SKUNO", sfcdb);
            if (objControl != null && !TROS.IsExist(sfcdb, objPack.PACK_NO, objPack.PACK_TYPE))
            {
                //throw new Exception(palletNo + "該PalletNO需要OBA Check  PASS請聯繫OBA處理！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144214", new string[] { palletNo }));
            }
        }
        /// <summary>
        /// 檢查棧板或卡通內的SN是否都是同一個路由
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackObjectMoreRouteChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            LogicObject.Packing packObj = (LogicObject.Packing)sessionPackObject.Value;
            if (packObj.IsHadMoreRoute(Station.SFCDB))
            {
                //throw new Exception(packObj.PackNo + " had more route!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144247", new string[] { packObj.PackNo }));
            }
        }
        /// <summary>
        /// 檢查輸入的值是否在棧板或卡通內
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputValueInThePackChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionInputValue = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionInputValue == null || sessionInputValue.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string inputValue = sessionInputValue.Value.ToString();
            LogicObject.Packing packObj = (LogicObject.Packing)sessionPackObject.Value;
            if (!packObj.IsInThePack(inputValue))
            {
                //throw new Exception(inputValue + " not in the " + packObj.PackNo);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144328", new string[] { inputValue, packObj.PackNo }));
            }
        }
        /// <summary>
        /// HWT CBS 工站檢查是否要掃描棧板內的全部卡通
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CartonDoubleCheckChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionShowCheck = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionShowCheck == null)
            {
                sessionShowCheck = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null, Value = "NO" };
                Station.StationSession.Add(sessionShowCheck);
            }

            MESStationSession sessionPassStatus = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPassStatus == null)
            {
                sessionPassStatus = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = null, Value = "TRUE" };
                Station.StationSession.Add(sessionPassStatus);
            }

            LogicObject.Packing packObj = (LogicObject.Packing)sessionPackObject.Value;
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            T_C_SKU_DETAIL TCSD = new T_C_SKU_DETAIL(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK TRPD = new MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK(Station.SFCDB, Station.DBType);
            C_SKU_DETAIL detailObj = TCSD.GetSkuDetail("CBS_DOUBLE_CHECK", "CBS_DOUBLE_CHECK", packObj.Skuno, sfcdb);

            if (detailObj != null && sessionPassStatus.Value.ToString().ToUpper() == "TRUE")
            {
                List<MESDataObject.Module.HWT.R_PALLET_DOUBLE_CHECK> list = TRPD.GetCheckList(sfcdb, packObj.PackNo, "CARTON");
                if (list.Count != packObj.CartonList.Count)
                {
                    sessionShowCheck.Value = "CARTON";
                    sessionPassStatus.Value = "FALSE";
                }
                else
                {
                    sessionShowCheck.Value = "NO";
                    sessionPassStatus.Value = "TRUE";
                }
            }
        }
        /// <summary>
        /// HWT CBS 工站檢查是否需要掃描棧板內的所有SN
        /// PE要求如果該棧板中有重複過卡通的SN,需要double check該棧板所有的SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNDoubleCheckChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionShowCheck = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionShowCheck == null)
            {
                sessionShowCheck = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null, Value = "NO" };
                Station.StationSession.Add(sessionShowCheck);
            }

            MESStationSession sessionPassStatus = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPassStatus == null)
            {
                sessionPassStatus = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = null, Value = "TRUE" };
                Station.StationSession.Add(sessionPassStatus);
            }
            LogicObject.Packing packObj = (LogicObject.Packing)sessionPackObject.Value;
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            int count = 0;
            MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK TRPD = new MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK(sfcdb, dbtype);
            List<MESDataObject.Module.HWT.R_PALLET_DOUBLE_CHECK> list = TRPD.GetCheckList(sfcdb, packObj.PackNo, "SN");
            if (sessionPassStatus.Value.ToString().ToUpper() == "TRUE")
            {
                bool bShow = false;
                if (list.Count != packObj.SNList.Count)
                {
                    foreach (R_SN sn in packObj.SNList)
                    {
                        count = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(r => r.SN == sn.SN && r.STATION_NAME == "CARTON").ToList().Count;
                        if (count > 1)
                        {
                            bShow = true;
                            break;
                        }
                    }
                    if (bShow)
                    {
                        sessionShowCheck.Value = "SN";
                        sessionPassStatus.Value = "FALSE";
                    }
                    else
                    {
                        sessionShowCheck.Value = "NO";
                        sessionPassStatus.Value = "TRUE";
                    }
                }
            }
        }
        /// <summary>
        /// HWT CBS DOUBLE CHECK 檢查是否重複掃描
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void DoubleCheckHistoryChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionPackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackNo == null || sessionPackNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionCheckValue = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCheckValue == null || sessionCheckValue.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionCheckType = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionCheckType == null || sessionCheckType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK TRPD = new MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK(sfcdb, dbtype);
            if (TRPD.IsExist(sfcdb, sessionPackNo.Value.ToString(), sessionCheckType.Value.ToString(), sessionCheckValue.Value.ToString()))
            {
                //throw new Exception(sessionCheckValue.Value.ToString() + " 該序列號已經掃描，請不要重復掃描！");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144405", new string[] { sessionCheckValue.Value.ToString() }));
            }
        }

        public static void OverPackDNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionDN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDN == null || sessionDN.Value == null)
            {
                //throw new Exception("Pls Input DN First!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144434"));
            }
            var strDN = sessionDN.Value.ToString();

            string strSql = $@"select*From R_DN_STATUS where dn_no in(select  dn_no From R_DN_CUST_PO )  
            and DN_FLAG=0 and GT_FLAG=0 and dn_no='{strDN}'";

            DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            //var data = Station.SFCDB.ORM.Queryable<R_DN_STATUS>()
            //   .Where(t => t.DN_FLAG == "0" && t.GT_FLAG == "0" 
            //   && t.DN_NO == SqlSugar.SqlFunc.Subqueryable<R_DN_CUST_PO>().Select(t2 => t2.DN_NO)).Select(t => t.DN_NO).ToList();
            //var DN = data.Find(t => t == strDN);
            if (dt.Rows.Count == 0)
            {
                //throw new Exception("DN cann't be use!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144504"));
            }
        }
        /// <summary>
        /// 检查SN与栈板实物是否一致
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ShipPackSNChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string packNo = Input.Value.ToString();
            int count = 0;
            int currQty = 0;
            int flag = 2;
            int firtCount = 0;
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
                dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString(),
                skuNo = Station.DisplayOutput.Find(t => t.Name == "SKU_NO").Value.ToString();
            if (string.IsNullOrEmpty(dnNo))
            {
                //throw new Exception($@"Please choose DN");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142826"));
            }
            var qtyDn = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(RSD => RSD.DN_NO == dnNo).ToList().OrderBy(RSD => RSD.ID).FirstOrDefault();
            var packing = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING>((RPK1, RPK2) => RPK2.ID == RPK1.PARENT_PACK_ID).Where((RPK1, RPK2) => RPK2.PACK_NO == packNo).Select((RPK1, RPK2) => RPK2).ToList().FirstOrDefault();
            if (qtyDn == null)
            {
                try
                {
                    currQty = Convert.ToInt16(packing.QTY);
                    firtCount = flag = currQty;
                }
                catch (Exception ex)
                {
                    //throw new Exception("Convert Qty Carton Error " + ex.Message);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144822", new string[] { ex.Message }));
                }
            }
            else
            {
                var qtyShip = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((RPK1, RPK2, RSN, RSNP) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                Where((RPK1, RPK2, RSN, RSNP) => RSN.VALID_FLAG == "1" && RSN.SN == qtyDn.SN).Select((RPK1, RPK2, RSN, RSNP) => RPK2).ToList().FirstOrDefault();
                //var qtyShip = Station.SFCDB.ORM.Queryable<R_SN, R_SHIP_DETAIL, R_SN_PACKING, R_PACKING, R_PACKING>((RSN, RSD, RSNP, RPK1, RPK2) => RSD.SN == RSN.SN && RSNP.SN_ID == RSN.ID &&   RPK1.ID == RSNP.PACK_ID && RPK2.ID == RPK1.PARENT_PACK_ID).Select((RSN, RSNP, RSD, RPK1, RPK2) => RPK2).ToList().OrderBy(rsd => rsd.ID).FirstOrDefault();
                try
                {
                    firtCount = Convert.ToInt16(qtyShip.QTY);
                    if (firtCount != Convert.ToInt16(packing.QTY))
                    {
                        flag = Convert.ToInt16(packing.QTY);
                    }
                }
                catch (Exception ex)
                {
                    //throw new Exception("Convert Qty Carton Error " + ex.Message);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144822", new string[] { ex.Message }));
                }
            }

            T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            bool _skuNoCarton = c_sku_detail.CheckExists(skuNo, "NOCARTON_CHECKSN", "NOCARTON_CHECKSN", Station.SFCDB);

            if (_skuNoCarton)
            {
                var qtySN = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((RPK1, RPK2, RSN, RSNP) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                Where((RPK1, RPK2, RSN, RSNP) => RSN.VALID_FLAG == "1" && RPK2.PACK_NO == packNo).Select((RPK1, RPK2, RSN, RSNP) => RSNP).ToList();
                if (qtySN != null)
                {
                    flag = qtySN.Count;
                }
            }
            List<string> lst = new List<string>();
            var data = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((RSN, RSNP, RPK1, RPK2) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                Where((RSN, RSNP, RPK1, RPK2) => RSN.VALID_FLAG == "1" && RPK2.PACK_NO == packNo).Select((RSN, RSNP, RPK1, RPK2) => RSN).ToList();
            if (data == null)
            {
                //throw new Exception($@"棧板:{packNo} 實物為空");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144918", new string[] { packNo }));
            }
            var data2 = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((RPK1, RPK2, RSN, RSNP) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                Where((RPK1, RPK2, RSN, RSNP) => RSN.VALID_FLAG == "1" && RPK2.PACK_NO == packNo).Select((RPK1, RPK2, RSN, RSNP) => RPK1).ToList();
            if (data2 == null)
            {
                //throw new Exception($@"棧板:{packNo} 實物為空");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144918", new string[] { packNo }));
            }
            UIInputData O = null;
            if (Station.BU == "VNDCN")
                O = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "SN/Cartton", Tittle = "Checking Pallet", Type = UIInputType.String, Name = "", ErrMessage = "Checking pallet" };
            else O = new UIInputData() { Timeout = 100000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "", Tittle = "輸入SN條碼", Type = UIInputType.String, Name = "IMEI", ErrMessage = "請輸入SN條碼" };
            var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station, (res) =>
            {
                #region VN邏輯，影響到NN了，暫時屏蔽，自己想辦法只影響VN吧
                if (Station.BU == "VNDCN" || Station.BU == "VNJUNIPER" || Station.BU == "FJZ")
                {
                    string sn = res.ToString();
                    if (lst.FindAll(st => st.Equals(res.ToString())).Count > 0)
                    {
                        //O.CBMessage = $@"{res.ToString()} Mã này đã được nhập kiểm tra. Đề nghị nhập mã mới!";
                        O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145025", new string[] { res });
                        return false;
                    }
                    lst.Add(res.ToString());
                    var check1 = data.FindAll(it => it.SN == res.ToString());
                    var check2 = data2.FindAll(it => it.PACK_NO == res.ToString());
                    if (check1.Count > 0 || check2.Count > 0 || packNo.Equals(res.ToString()))
                    {
                        count++;
                        if (count == flag)
                        {
                            return true;
                        }
                        //O.CBMessage = $@"{res.ToString()} Thành công. Bạn nhập thêm mã SN/Cartton!";
                        O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145119", new string[] { res });
                    }
                    else
                    {
                        if (sn.IndexOf("S") == 0) sn = sn.Remove(0, 1);
                        check1 = data.FindAll(it => it.SN == sn);
                        check2 = data2.FindAll(it => it.PACK_NO == sn);
                        if (check1.Count > 0 || check2.Count > 0 || packNo.Equals(sn))
                        {
                            count++;
                            if (count == flag)
                            {
                                return true;
                            }
                            //O.CBMessage = $@"{sn} Thành công. Bạn nhập thêm mã SN/Cartton!";
                            O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145119", new string[] { sn });
                        }
                        else
                        {
                            count = 0;
                            //O.CBMessage = $@"SN/Cartton: {res.ToString()} Không đúng với mã PalletNo: {packNo} !";
                            O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145216", new string[] { res, packNo });
                        }
                    }
                }
                #endregion
                else
                {
                    //if (data.FindAll(it => it.SN == res.ToString()) != null)
                    if (data.FindAll(it => it.SN == res.ToString()).Count > 0)//20210519 VT用戶反應該卡關不生效 Edit By ZHB
                    {
                        return true;
                    }
                    else if (data2.FindAll(it => it.PACK_NO == res.ToString()).Count > 0)
                    {
                        return true;
                    }
                    else
                        //O.CBMessage = $@"{res.ToString()} 實物與棧板{packNo} 不匹配不允許出貨!";
                        O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145249", new string[] { res, packNo });
                }
                return false;
            });
        }

        /// <summary>
        /// Kiểm tra các pallet lẻ. yêu cầu scan full carton
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ShipPackSNCheckerOdd(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string packNo = Input.Value.ToString();
            int count = 0;
            int currQty = 0;
            int flag = 2;
            int firtCount = 0;
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
                dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString(),
                skuNo = Station.DisplayOutput.Find(t => t.Name == "SKU_NO").Value.ToString();
            if (string.IsNullOrEmpty(dnNo))
            {
                //throw new Exception($@"Please choose DN");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142826"));
            }
            var qtyDn = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(RSD => RSD.DN_NO == dnNo).ToList().OrderBy(RSD => RSD.ID).FirstOrDefault();
            var packing = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING>((RPK1, RPK2) => RPK2.ID == RPK1.PARENT_PACK_ID).Where((RPK1, RPK2) => RPK2.PACK_NO == packNo).Select((RPK1, RPK2) => RPK2).ToList().FirstOrDefault();
            if (qtyDn == null)
            {
                try
                {
                    currQty = Convert.ToInt16(packing.QTY);
                    firtCount = flag = currQty;
                }
                catch (Exception ex)
                {
                    //throw new Exception("Convert Qty Carton Error " + ex.Message);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144822", new string[] { ex.Message }));
                }
            }
            else
            {
                var qtyShip = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((RPK1, RPK2, RSN, RSNP) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                Where((RPK1, RPK2, RSN, RSNP) => RSN.VALID_FLAG == "1" && RSN.SN == qtyDn.SN).Select((RPK1, RPK2, RSN, RSNP) => RPK2).ToList().FirstOrDefault();
                //var qtyShip = Station.SFCDB.ORM.Queryable<R_SN, R_SHIP_DETAIL, R_SN_PACKING, R_PACKING, R_PACKING>((RSN, RSD, RSNP, RPK1, RPK2) => RSD.SN == RSN.SN && RSNP.SN_ID == RSN.ID &&   RPK1.ID == RSNP.PACK_ID && RPK2.ID == RPK1.PARENT_PACK_ID).Select((RSN, RSNP, RSD, RPK1, RPK2) => RPK2).ToList().OrderBy(rsd => rsd.ID).FirstOrDefault();
                try
                {
                    firtCount = Convert.ToInt16(qtyShip.QTY);
                    if (firtCount != Convert.ToInt16(packing.QTY))
                    {
                        flag = Convert.ToInt16(packing.QTY);
                    }
                }
                catch (Exception ex)
                {
                    //throw new Exception("Convert Qty Carton Error " + ex.Message);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144822", new string[] { ex.Message }));
                }
            }
            T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            //some Sku have not Label Carton, then need scan label SN
            bool _skuNoCarton = c_sku_detail.CheckExists(skuNo, "NOCARTON_CHECKSN", "NOCARTON_CHECKSN", Station.SFCDB);
            if (_skuNoCarton)
            {
                var qtySN = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((rpkCarton, rpkPallet, RSN, RSNP) => rpkPallet.ID == rpkCarton.PARENT_PACK_ID && rpkCarton.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                Where((rpkCarton, rpkPallet, RSN, RSNP) => RSN.VALID_FLAG == "1" && rpkPallet.PACK_NO == packNo).Select((RPK1, RPK2, RSN, RSNP) => RSNP).ToList();
                if (qtySN != null)
                {
                    flag = qtySN.Count;
                }
            }
            List<string> lst = new List<string>();
            var data = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((RSN, RSNP, RPK1, RPK2) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                                            Where((RSN, RSNP, RPK1, RPK2) => RSN.VALID_FLAG == "1" && RPK2.PACK_NO == packNo).Select((RSN, RSNP, RPK1, RPK2) => RSN).ToList();
            if (data == null)
            {
                //throw new Exception($@"棧板:{packNo} 實物為空");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144918", new string[] { packNo }));
            }
            var dataCarton = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((RPK1, RPK2, RSN, RSNP) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                Where((RPK1, RPK2, RSN, RSNP) => RSN.VALID_FLAG == "1" && RPK2.PACK_NO == packNo).Select((RPK1, RPK2, RSN, RSNP) => RPK1).ToList();
            if (dataCarton == null)
            {
                //throw new Exception($@"棧板:{packNo} 實物為空");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144918", new string[] { packNo }));
            }
            UIInputData O = new UIInputData() { Timeout = 1800000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "SN/Cartton", Tittle = "Checking Pallet", Type = UIInputType.String, Name = "", ErrMessage = "Checking pallet" };
            var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station, (res) =>
            {
                #region VN邏輯，影響到NN了，暫時屏蔽，自己想辦法只影響VN吧

                string sn = res.ToString();

                #region HPE Aruba - ADD-SUFFIX at C_SKU_DETAIL - niemnv 20210807
                //R_PACKING rpacking = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == packNo).First();
                //skuNo = rpacking.SKUNO;
                //if (string.IsNullOrEmpty(skuNo))
                //{
                //    //throw new MESReturnMessage($@"Get SkuNo Fail, Please Check!");
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143054"));
                //}

                sn = c_sku_detail.SNPreprocessor(Station.SFCDB, skuNo, sn, Station.StationName);

                #endregion

                if (lst.FindAll(st => st.Equals(res.ToString())).Count > 0)
                {
                    //O.CBMessage = $@"{res.ToString()} Mã này đã được nhập kiểm tra. Đề nghị nhập mã mới!";
                    O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145025", new string[] { res });
                    return false;
                }
                lst.Add(res.ToString());
                var check1 = data.FindAll(it => it.SN == sn && !_skuNoCarton);
                var check2 = dataCarton.FindAll(it => it.PACK_NO == sn);//If resInput is Carton
                if (check1.Count > 0 || check2.Count > 0 || packNo.Equals(res.ToString()))
                {
                    count++;
                    if (count == flag)
                    {
                        return true;
                    }
                    //O.CBMessage = $@"{res.ToString()} Thành công {count}. Bạn nhập thêm mã SN/Cartton!";
                    O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145917", new string[] { res, count.ToString() });
                }
                else
                {
                    if (sn.IndexOf("S") == 0) sn = sn.Remove(0, 1);
                    check1 = data.FindAll(it => it.SN == sn);
                    check2 = dataCarton.FindAll(it => it.PACK_NO == sn);
                    if (check1.Count > 0 || check2.Count > 0 || packNo.Equals(sn))
                    {
                        count++;
                        if (count == flag)
                        {
                            return true;
                        }
                        //O.CBMessage = $@"{sn} Thành công {count}. Bạn nhập thêm mã SN/Cartton!";
                        O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145917", new string[] { sn, count.ToString() });
                    }
                    else
                    {
                        count = 0;
                        //O.CBMessage = $@"SN/Cartton: {res.ToString()} Không đúng với mã PalletNo: {packNo} !";
                        O.CBMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145216", new string[] { res, packNo });
                    }
                }

                #endregion
                return false;
            });
        }

        public static void ShipPackCheckCartonOdd(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string packNo = Input.Value.ToString();
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
               dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString(),
               skuNo = Station.DisplayOutput.Find(t => t.Name == "SKU_NO").Value.ToString();
            // if (SHIPOUT_CATEGORY_SKUNO.IndexOf(skuNo) > -1) return;
            if (string.IsNullOrEmpty(dnNo))
            {
                //throw new Exception($@"Please choose DN");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142826"));
            }
            var dnInfo = Station.SFCDB.ORM.Queryable<R_DN_STATUS>().Where(RSD => RSD.DN_NO == dnNo).ToList().OrderBy(RSD => RSD.ID).FirstOrDefault();
            var packingInfo = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(RP => RP.PACK_NO == packNo && RP.PACK_TYPE == "PALLET").ToList().FirstOrDefault();
            if (packingInfo == null)
            {
                //throw new Exception($@"Dont get Packing Info {packNo}");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142843", new string[] { packNo }));
            }
            var cartonInfo = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(RP => RP.PARENT_PACK_ID == packingInfo.ID && RP.PACK_TYPE == "CARTON").ToList();
            int maxQty = 0, currentQty = 0;
            try
            {
                maxQty = Convert.ToInt16(packingInfo.MAX_QTY);
                if (cartonInfo != null)
                    currentQty = Convert.ToInt16(cartonInfo.Count);
                else //throw new Exception($@"Dont get Carton Info {packNo}");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142951", new string[] { packNo }));
            }
            catch (Exception ex)
            {
                //throw new Exception($@"Convert Qty Error");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814142931"));
            }
            int flag = 2;
            int count = 0;
            T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            //some Sku have not Label Carton, then need scan label SN
            bool _skuNoCarton = c_sku_detail.CheckExists(skuNo, "NOCARTON_CHECKSN", "NOCARTON_CHECKSN", Station.SFCDB);

            if (maxQty > currentQty)
            {
                flag = currentQty;
                if (_skuNoCarton)
                {
                    var qtySN = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((RPK1, RPK2, RSN, RSNP) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                    Where((RPK1, RPK2, RSN, RSNP) => RSN.VALID_FLAG == "1" && RPK2.PACK_NO == packNo).Select((RPK1, RPK2, RSN, RSNP) => RSNP).ToList();
                    if (qtySN != null)
                    {
                        flag = qtySN.Count;
                    }
                }
            }
            List<string> lst = new List<string>();
            var data = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((RSN, RSNP, RPK1, RPK2) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                    Where((RSN, RSNP, RPK1, RPK2) => RSN.VALID_FLAG == "1" && RPK2.PACK_NO == packNo).Select((RSN, RSNP, RPK1, RPK2) => RSN).ToList();
            if (data == null)
            {
                //throw new Exception($@"棧板:{packNo} 實物為空");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144918", new string[] { packNo }));
            }
            var data2 = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN, R_SN_PACKING>((RPK1, RPK2, RSN, RSNP) => RPK2.ID == RPK1.PARENT_PACK_ID && RPK1.ID == RSNP.PACK_ID && RSNP.SN_ID == RSN.ID).
                Where((RPK1, RPK2, RSN, RSNP) => RSN.VALID_FLAG == "1" && RPK2.PACK_NO == packNo).Select((RPK1, RPK2, RSN, RSNP) => RPK1).ToList();
            if (data2 == null)
            {
                //throw new Exception($@"棧板:{packNo} 實物為空");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814144918", new string[] { packNo }));
            }
            UIInputData O = new UIInputData() { Timeout = 1800000, IconType = IconType.Warning, UIArea = new string[] { "50%", "45%" }, Message = "SN/Cartton", Tittle = "Checking Pallet", Type = UIInputType.String, Name = "", ErrMessage = "Checking pallet" };
            var ret1 = O.GetUiInput(Station.API, UIInput.Normal, Station, (res) =>
            {
                #region VN邏輯，影響到NN了，暫時屏蔽，自己想辦法只影響VN吧

                string sn = res.ToString();

                #region HPE Aruba - ADD-SUFFIX at C_SKU_DETAIL - niemnv 20210807
                R_PACKING rpacking = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == packNo).First();
                skuNo = rpacking.SKUNO;
                if (string.IsNullOrEmpty(skuNo))
                {
                    //throw new MESReturnMessage($@"Get SkuNo Fail, Please Check!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143054"));
                }

                sn = c_sku_detail.SNPreprocessor(Station.SFCDB, skuNo, sn, Station.StationName);

                #endregion

                if (lst.FindAll(st => st.Equals(res.ToString())).Count > 0)
                {
                    //O.ErrMessage = $@"{res.ToString()} Mã này đã được nhập kiểm tra. Đề nghị nhập mã mới!";
                    O.ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145025", new string[] { res });
                    return false;
                }
                lst.Add(res.ToString());
                var check1 = data.FindAll(it => it.SN == res.ToString() && !_skuNoCarton);
                var check2 = data2.FindAll(it => it.PACK_NO == res.ToString());//If resInput is Carton
                if (check1.Count > 0 || check2.Count > 0 || packNo.Equals(res.ToString()))
                {
                    count++;
                    if (count == flag)
                    {
                        return true;
                    }
                    //O.Message = $@"{res.ToString()} Thành công {count}. Bạn nhập thêm mã SN/Cartton!";
                    O.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145917", new string[] { res, count.ToString() });
                }
                else
                {
                    if (sn.IndexOf("S") == 0) sn = sn.Remove(0, 1);
                    check1 = data.FindAll(it => it.SN == sn);
                    check2 = data2.FindAll(it => it.PACK_NO == sn);
                    if (check1.Count > 0 || check2.Count > 0 || packNo.Equals(sn))
                    {
                        count++;
                        if (count == flag)
                        {
                            return true;
                        }
                        //O.Message = $@"{sn} Thành công {count}. Bạn nhập thêm mã SN/Cartton!";
                        O.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145917", new string[] { res, count.ToString() });
                    }
                    else
                    {
                        count = 0;
                        //O.ErrMessage = $@"SN/Cartton: {res.ToString()} Không đúng với mã PalletNo: {packNo} !";
                        O.ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814145216", new string[] { res, packNo });
                    }
                }

                #endregion
                return false;
            });
        }

        public static void OverpackCheckDN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SN" }));
            }
            MESStationSession dnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (dnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "DN" }));
            }
            SN snObj = (SN)SNSession.Value;
            string dn = dnSession.Value.ToString();
            R_PACKING pallet = Station.SFCDB.ORM.Queryable<R_SN_PACKING, R_PACKING, R_PACKING>((rsp, rpc, rpp) => rsp.PACK_ID == rpc.ID && rpc.PARENT_PACK_ID == rpp.ID)
                .Where((rsp, rpc, rpp) => rsp.SN_ID == snObj.ID).Select((rsp, rpc, rpp) => rpp).ToList().FirstOrDefault();
            if (pallet == null)
            {
                //throw new Exception($@"{snObj.SerialNo} not in Pallet");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150010", new string[] { snObj.SerialNo }));
            }

            var overpack_list = Station.SFCDB.ORM.Queryable<R_SN_PACKING, R_PACKING, R_PACKING, R_SN_OVERPACK>((rsp, rpc, rpp, rso) => rsp.PACK_ID == rpc.ID && rpc.PARENT_PACK_ID == rpp.ID
                && rsp.SN_ID == rso.SN_ID).Where((rsp, rpc, rpp, rso) => rpp.ID == pallet.ID).Select((rsp, rpc, rpp, rso) => rso).ToList();
            if (overpack_list.Count > 0)
            {
                if (!overpack_list.Any(r => r.DN_NO == dn))
                {
                    //throw new Exception($@"{pallet.PACK_NO} Alreay Link To {overpack_list[0].DN_NO},One Pallet Only Link To One DN!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150841", new string[] { pallet.PACK_NO, overpack_list[0].DN_NO }));
                }
            }
        }

        /// <summary>
        /// 檢查TGMES棧板號
        /// </summary>
        public static void TGMESPalletChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string palletNo = sessionPalletNo.Value.ToString();
            List<R_SN_TGMES_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == palletNo && t.VALID_FLAG == "1").ToList();
            if (TGMESlist.Count == 0)
            {
                //throw new Exception(palletNo + "無效!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150929", new string[] { palletNo }));
            }
        }

        /// <summary>
        /// 檢查TGMES卡通號
        /// </summary>
        public static void TGMESCartonChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionCartonNo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCartonNo == null || sessionCartonNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string palletNo = sessionPalletNo.Value.ToString();
            string cartonNo = sessionCartonNo.Value.ToString();

            List<R_SN_TGMES_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == palletNo && t.PACKING2 == cartonNo && t.VALID_FLAG == "1").ToList();
            if (TGMESlist.Count == 0)
            {
                //throw new Exception(cartonNo + "不存在於棧板號:" + palletNo + "內!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150956", new string[] { cartonNo, palletNo }));
            }
            if (TGMESlist.FindAll(t => t.COMPLETED_FLAG == "1").Count > 0)
            {
                //throw new Exception(cartonNo + "中SN已完工!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151040", new string[] { cartonNo }));
            }
            if (TGMESlist.FindAll(t => t.SHIPPED_FLAG == "1").Count > 0)
            {
                //throw new Exception(cartonNo + "中SN已出貨!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151106", new string[] { cartonNo }));
            }
            if (TGMESlist.FindAll(t => t.NEXT_STATION != Station.StationName).Count > 0)
            {
                //throw new Exception(cartonNo + "中SN下一站錯誤!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151128", new string[] { cartonNo }));
            }
        }

        /// <summary>
        /// 檢查TGMES出貨
        /// </summary>
        public static void TGMESShipOutChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string palletNo = sessionPalletNo.Value.ToString();

            MESStationSession sessionSkuNo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string skuNo = sessionSkuNo.Value.ToString();

            MESStationSession sessionPoNo = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPoNo == null || sessionPoNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            string dnSkuNo = Station.DisplayOutput.Find(t => t.Name == "SKU_NO").Value.ToString();
            if (string.IsNullOrEmpty(dnSkuNo))
            {
                //throw new Exception("DN對應機種為空, 請雙擊選擇DN!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151149"));
            }
            if (skuNo != dnSkuNo)
            {
                //throw new Exception(palletNo + "機種與DN對應機種不符!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151243", new string[] { palletNo }));
            }

            List<R_SN_TGMES_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == palletNo && t.VALID_FLAG == "1").ToList();
            if (TGMESlist.Count == 0)
            {
                //throw new Exception(palletNo + "無效!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150929", new string[] { palletNo }));
            }
            if (TGMESlist.FindAll(t => t.COMPLETED_FLAG != "1").Count > 0)
            {
                //throw new Exception(palletNo + "中SN未完工!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151309", new string[] { palletNo }));
            }
            if (TGMESlist.FindAll(t => t.SHIPPED_FLAG == "1").Count > 0)
            {
                //throw new Exception(palletNo + "中SN已出貨!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151106", new string[] { palletNo }));
            }
            if (TGMESlist.FindAll(t => t.NEXT_STATION != Station.StationName).Count > 0)
            {
                //throw new Exception(palletNo + "中SN下一站錯誤!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151128", new string[] { palletNo }));
            }
        }

        /// <summary>
        /// 檢查TGMES出貨時輸入PO號
        /// </summary>
        public static void TGMESPoNoChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionPoNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPoNo == null || sessionPoNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string poNo = sessionPoNo.Value.ToString();
            if (!poNo.StartsWith("B"))
            {
                //throw new Exception("PO必須以'B'開頭!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151604"));
            }
            if (poNo.Length != 10)
            {
                //throw new Exception("PO輸入不正確,請確認!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151645"));
            }
        }

        public static void CheckCartonPalletWO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string WO = sessionWO.Value.ToString();

            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCarton == null || sessionCarton.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string CartonNO = sessionCarton.Value.ToString();

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null || sessionPallet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            string PalletNO = sessionPallet.Value.ToString();

            var PLwo = Station.SFCDB.ORM.Queryable<R_SN_PACKING, R_PACKING, R_SN, R_PACKING, R_WO_BASE>((rsp, rp, rs, rpp, rw) => rsp.PACK_ID == rp.ID && rp.PARENT_PACK_ID == rpp.ID
            && rsp.SN_ID == rs.ID && rs.WORKORDERNO == rw.WORKORDERNO).Where((rsp, rp, rs, rpp, rw) => rpp.PACK_NO == PalletNO && rs.VALID_FLAG == "1").
            Select((rsp, rp, rs, rpp, rw) => rw.WORKORDERNO).ToList();
            if (PLwo != null)
            {
                if (PLwo.Distinct().ToList().Count == 1)
                {
                    if (WO != PLwo.Distinct().ToList()[0])
                    {
                        //throw new MESReturnMessage($@"Sn WO in Pallet is not the same as over station SN WO!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151705"));
                    }
                }
                else if (PLwo.Distinct().ToList().Count > 1)
                {
                    //throw new MESReturnMessage($@"The WO in the pallet is inconsistent,Please confirm!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151731"));
                }
            }
        }

        /// <summary>
        /// FIFO check pack#
        /// 根據棧板入庫時間進行管控,棧板入庫的時間取值棧板上最早入庫SN時間點
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPackingFIFO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            string packNo = Input.Value.ToString().ToUpper();
            int DnQty = int.Parse(Station.DisplayOutput.Find(t => t.Name == "GT_QTY").Value.ToString()); //獲取DN數量
            string Sku = Station.DisplayOutput.Find(t => t.Name == "SKU_NO").Value.ToString();

            var SkuStockQty = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                    .Where((s, sp, rp1, rp2) => s.SKUNO == Sku && s.SHIPPED_FLAG == MesBool.No.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && s.CURRENT_STATION == "CBS")
                    .Select((s, sp, rp1, rp2) => new { s.SN, s.COMPLETED_TIME, rp2.PACK_NO }).ToList(); //取得機種庫存數量


            var packobj = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == packNo).ToList().FirstOrDefault();
            if (Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where((r) => r.VALUE == packobj.SKUNO && r.FUNCTIONNAME == "NOCHECKSKU_SHIPPING" && r.CONTROLFLAG == "Y").Any())
                return;
            if (!Station.SFCDB.ORM.Queryable<R_PACKING_FIFO>().Any(t => t.PACKNO == packNo && t.STATUS == MesBool.Yes.ExtValue()))
            {

                if (DnQty > SkuStockQty.Count) //DN出貨數量＞庫存時，掃描按棧板入庫時間順序卡關
                {
                    var packfirstsn = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                        .Where((s, sp, rp1, rp2) => rp2.PACK_NO == packNo).OrderBy((s, sp, rp1, rp2) => s.COMPLETED_TIME, SqlSugar.OrderByType.Asc).Select((s, sp, rp1, rp2) => s).ToList().FirstOrDefault();
                    var earlypackno = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                        .Where((s, sp, rp1, rp2) => s.SKUNO == packfirstsn.SKUNO && s.SHIPPED_FLAG == MesBool.No.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && rp2.PACK_NO != packNo && s.CURRENT_STATION == "CBS")
                        .OrderBy((s, sp, rp1, rp2) => s.COMPLETED_TIME, SqlSugar.OrderByType.Asc).Select((s, sp, rp1, rp2) => new { s.SN, s.COMPLETED_TIME, rp2.PACK_NO }).ToList().FirstOrDefault();

                    if (earlypackno != null && packfirstsn.COMPLETED_TIME > earlypackno.COMPLETED_TIME)
                        //throw new Exception($@"Pallet FIFO Alart: packno {packNo} sn: {packfirstsn.SN} completedtime is {packfirstsn.COMPLETED_TIME.ToString()} > packno {earlypackno.PACK_NO} sn {earlypackno.SN} completedtime {earlypackno.COMPLETED_TIME}");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151746", new string[] { packNo, packfirstsn.SN, packfirstsn.COMPLETED_TIME.ToString(), earlypackno.PACK_NO, earlypackno.SN, earlypackno.COMPLETED_TIME.ToString() }));
                }
                else if (DnQty < SkuStockQty.Count) //DN出貨數量<庫存時，庫存入庫時間最早的數量并且在DN出貨數量範圍值內不卡關，超出后的掃描時卡FIFO
                {
                    string strSql = $@" select s.*
                                            from R_SN s, R_SN_PACKING sp, R_PACKING rp1, R_PACKING rp2
                                            where s.ID = sp.SN_ID
                                            and sp.PACK_ID = rp1.ID
                                            and rp1.PARENT_PACK_ID = rp2.ID
                                            and rp2.PACK_NO = '{packNo}'
                                            and s.sn not in
                                                (select sn
                                                    from (select s.SN,
                                                                    s.COMPLETED_TIME,
                                                                    rp2.PACK_NO,
                                                                    ROW_NUMBER() over(order by COMPLETED_TIME asc) as rowindex
                                                            from R_SN s, R_SN_PACKING sp, R_PACKING rp1, R_PACKING rp2
                                                            where s.ID = sp.SN_ID
                                                                and sp.PACK_ID = rp1.ID
                                                                and rp1.PARENT_PACK_ID = rp2.ID
                                                                and s.SKUNO = '{Sku}'
                                                                and s.SHIPPED_FLAG = '0'
                                                                and s.COMPLETED_FLAG = '1'
                                                                AND s.current_station = 'CBS')
                                                    where rowindex between 1 and '{DnQty}')";

                    DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        var packfirstsn = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                            .Where((s, sp, rp1, rp2) => rp2.PACK_NO == packNo).OrderBy((s, sp, rp1, rp2) => s.COMPLETED_TIME, SqlSugar.OrderByType.Asc).Select((s, sp, rp1, rp2) => s).ToList().FirstOrDefault();
                        var earlypackno = Station.SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((s, sp, rp1, rp2) => s.ID == sp.SN_ID && sp.PACK_ID == rp1.ID && rp1.PARENT_PACK_ID == rp2.ID)
                            .Where((s, sp, rp1, rp2) => s.SKUNO == packfirstsn.SKUNO && s.SHIPPED_FLAG == MesBool.No.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && rp2.PACK_NO != packNo && s.CURRENT_STATION == "CBS")
                            .OrderBy((s, sp, rp1, rp2) => s.COMPLETED_TIME, SqlSugar.OrderByType.Asc).Select((s, sp, rp1, rp2) => new { s.SN, s.COMPLETED_TIME, rp2.PACK_NO }).ToList().FirstOrDefault();
                        if (earlypackno != null && packfirstsn.COMPLETED_TIME > earlypackno.COMPLETED_TIME)
                            //throw new Exception($@"Pallet FIFO Alart: packno {packNo} sn: {packfirstsn.SN} completedtime is {packfirstsn.COMPLETED_TIME.ToString()} > packno {earlypackno.PACK_NO} sn {earlypackno.SN} completedtime {earlypackno.COMPLETED_TIME}");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151746", new string[] { packNo, packfirstsn.SN, packfirstsn.COMPLETED_TIME.ToString(), earlypackno.PACK_NO, earlypackno.SN, earlypackno.COMPLETED_TIME.ToString() }));

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

            }


        }

        public static void JuniperCheckPackingDN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession dnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (dnSession == null || dnSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (packSession == null || packSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            bool bTrimStartZero = Paras[2].VALUE.ToUpper() == "YES" ? true : false;


            string dn = dnSession.Value.ToString();
            LogicObject.Packing packObj = new LogicObject.Packing();
            if (packSession.Value is string)
            {
                LogicObject.Packing packing = new LogicObject.Packing();
                packObj.DataLoad(packSession.Value.ToString(), Station.SFCDB, Station.DBType);
            }
            else if (packSession.Value is LogicObject.Packing)
            {
                packObj.DataLoad(((LogicObject.Packing)packSession.Value).PackNo, Station.SFCDB, Station.DBType);
            }
            else
            {
                //throw new MESReturnMessage($@"Input pack type error!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151946"));
            }
            var woArray = packObj.SNList.Select(r => r.WORKORDERNO).ToArray();
            var ordermain = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => woArray.Contains(t.PREWO) && t.ORDERTYPE == ENUM_I137_PoDocType.IDOA.ExtValue()).ToList();
            if (ordermain.Count == 0)//not DOA
            {
                var list_i282 = Station.SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_I282, O_ORDER_MAIN>((r, o) => r.ASNNUMBER == o.PREASN)
                    //.Where((r, o) => SqlSugar.SqlFunc.ContainsArray(woArray, o.PREWO) && SqlSugar.SqlFunc.IsNullOrEmpty(r.ERRORCODE) == true && r.ITEMID.EndsWith(o.POLINE))
                    //Tat.ho say remove r.ITEMID.EndsWith(o.POLINE)
                    .Where((r, o) => SqlSugar.SqlFunc.ContainsArray(woArray, o.PREWO) && SqlSugar.SqlFunc.IsNullOrEmpty(r.ERRORCODE) == true)
                    .Select((r, o) => r)
                    .Distinct()
                    .ToList();

                if (list_i282.Count == 0)
                {
                    //throw new Exception($@"{packObj.PackNo},No I282 Data!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152058", new string[] { packObj.PackNo }));
                }
                else if (list_i282.Count > 1)
                {
                    var obj = Station.SFCDB.ORM.Queryable<I137_H, I137_I, O_ORDER_MAIN>((H, I, O) => H.TRANID == I.TRANID && I.ID == O.ITEMID)
                        .Where((H, I, O) => SqlSugar.SqlFunc.ContainsArray(woArray, O.PREWO))
                        .Select((H, I, O) => new { H, I, O })
                        .First();
                    if (obj.H.COMPLETEDELIVERY == "X")
                    {
                        var i137ix = Station.SFCDB.ORM.Queryable<O_I137_ITEM, O_ORDER_MAIN, O_I137_HEAD>((I, O, H) => I.ID == O.ITEMID && I.TRANID == H.TRANID)
                            .Where((I, O, H) => H.SALESORDERNUMBER == obj.H.SALESORDERNUMBER && H.COMPLETEDELIVERY == obj.H.COMPLETEDELIVERY && O.PREASN == obj.O.PREASN)
                            .Select((I, O, H) => I)
                            .ToList();
                        var dnlist = list_i282.Select(t => new { t.DELIVERYNUMBER, t.ITEMID }).Distinct().ToList();
                        if (i137ix.Count > dnlist.Count)
                        {
                            //throw new Exception($@"{obj.H.PONUMBER},HAS {i137ix.Count} LINEs BUT I282 RETURNED {dnlist.Count} LINEs!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152142", new string[] { obj.H.PONUMBER, i137ix.Count.ToString(), dnlist.Count.ToString() }));
                        }
                    }
                    else if (obj.H.COMPLETEDELIVERY == "NA")
                    {
                        var dnlist = list_i282.Select(t => new { t.DELIVERYNUMBER, t.ITEMID, t.PRODUCTCODE }).Distinct().ToList();
                        var i137in = Station.SFCDB.ORM.Queryable<O_I137_ITEM, O_ORDER_MAIN, O_I137_HEAD>((I, O, H) => I.ID == O.ITEMID && I.TRANID == H.TRANID)
                            .Where((I, O, H) => H.SALESORDERNUMBER == obj.H.SALESORDERNUMBER && H.COMPLETEDELIVERY == obj.H.COMPLETEDELIVERY && O.PREASN == obj.O.PREASN)
                            .Select((I, O, H) => I)
                            .ToList();
                        if (i137in.Count != dnlist.Count)
                        {
                            i137in.ForEach(p =>
                            {
                                if (!dnlist.Any(t => t.PRODUCTCODE == p.PN))
                                    throw new MESReturnMessage($@"139 matches the data of 282,misssing material number {p.PN}");
                            });
                        }
                        //if (i137in.Count != dnlist.Count)
                        //{
                        //    //throw new Exception($@"{obj.H.SALESORDERNUMBER},HAS {i137in.Count} LINEs BUT I282 RETURNED {dnlist.Count} LINEs!");
                        //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152142", new string[] { obj.H.SALESORDERNUMBER, i137in.Count.ToString(), dnlist.Count.ToString() }));
                        //}
                    }
                    else
                    {
                        //throw new Exception($@"{packObj.PackNo},multiple DN!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152238", new string[] { packObj.PackNo }));
                    }
                }
                string deliverynumber = bTrimStartZero ? list_i282.FirstOrDefault().DELIVERYNUMBER.TrimStart('0') : list_i282.FirstOrDefault().DELIVERYNUMBER;
                if (!deliverynumber.Equals(dn))
                {
                    //throw new Exception($@"The DN[{deliverynumber}] of {packObj.PackNo} not equals scan DN {dn} !");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152405", new string[] { deliverynumber, packObj.PackNo, dn }));
                }
            }
            else
            {
                var list_ShipmentACK = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, R_JNP_DOA_SHIPMENTS_ACK>((O, A) => O.PREASN == A.ASNNUMBER)
                    .Where((O, A) => woArray.Contains(O.PREWO) && SqlSugar.SqlFunc.IsNullOrEmpty(A.MESSAGE_CODE))
                    .Select((O, A) => new { O.ITEMID,A.DELIVERYNUMBER })
                    .ToList();
                if (list_ShipmentACK.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_NOTSHIPMENTREPLY", new string[] { packObj.PackNo }));
                }
                else if (list_ShipmentACK.Count > 1)
                {
                    var obj = Station.SFCDB.ORM.Queryable<I137_H, I137_I, O_ORDER_MAIN>((H, I, O) => H.TRANID == I.TRANID && I.ID == O.ITEMID)
                           .Where((H, I, O) => SqlSugar.SqlFunc.ContainsArray(woArray, O.PREWO))
                           .Select((H, I, O) => new { H, I, O })
                           .First();
                    if (obj.H.COMPLETEDELIVERY == "X")
                    {
                        var i137ix = Station.SFCDB.ORM.Queryable<O_I137_ITEM, O_ORDER_MAIN, O_I137_HEAD>((I, O, H) => I.ID == O.ITEMID && I.TRANID == H.TRANID)
                            .Where((I, O, H) => H.SALESORDERNUMBER == obj.H.SALESORDERNUMBER && H.COMPLETEDELIVERY == obj.H.COMPLETEDELIVERY && O.PREASN == obj.O.PREASN)
                            .Select((I, O, H) => I)
                            .ToList();
                        if (i137ix.Count != list_ShipmentACK.Count)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_LESSSHIPMENTREPLY", new string[] { obj.H.PONUMBER, i137ix.Count.ToString(), list_ShipmentACK.Count.ToString() }));
                        }
                    }
                    else if (obj.H.COMPLETEDELIVERY == "NA")
                    {
                        var i137in = Station.SFCDB.ORM.Queryable<O_I137_ITEM, O_ORDER_MAIN, O_I137_HEAD>((I, O, H) => I.ID == O.ITEMID && I.TRANID == H.TRANID)
                            .Where((I, O, H) => H.SALESORDERNUMBER == obj.H.SALESORDERNUMBER && H.COMPLETEDELIVERY == obj.H.COMPLETEDELIVERY && O.PREASN == obj.O.PREASN)
                            .Select((I, O, H) => I)
                            .ToList();
                        if (i137in.Count != list_ShipmentACK.Count)
                        {
                            //throw new Exception($@"{obj.H.SALESORDERNUMBER},HAS {i137in.Count} LINEs BUT I282 RETURNED {dnlist.Count} LINEs!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_LESSSHIPMENTREPLY", new string[] { obj.H.SALESORDERNUMBER, i137in.Count.ToString(), list_ShipmentACK.Count.ToString() }));
                        }
                    }
                    else
                    {
                        //throw new Exception($@"{packObj.PackNo},multiple DN!");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152238", new string[] { packObj.PackNo }));
                    }
                }
            }
        }

        public static void PalletCheckEachCarton(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (packSession == null || packSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            string PackNORule = "";
            if (Station.StationName.ToString() == "PACKOUT")
            {
                MESStationSession PackNORuleSession = Station.StationSession.Find(t => t.MESDataType == "PackNORule" && t.SessionKey == "1");
                if (PackNORuleSession == null || PackNORuleSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "PackNORule" }));
                }
                PackNORule = PackNORuleSession.Value.ToString().ToUpper().Trim();
            }
            var cartonList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING>((c, p) => c.PARENT_PACK_ID == p.ID)
                .Where((c, p) => p.PACK_NO == packSession.Value.ToString()).Select((c, p) => c).ToList();
            if (PackNORule == "DEFAULT")
            {
                cartonList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING>((c, p, sp) => c.PARENT_PACK_ID == p.ID && c.ID == sp.PACK_ID)
              .Where((c, p, sp) => p.PACK_NO == packSession.Value.ToString()).Select((c, p, sp) => c).ToList();
            }

            if (cartonList.Count == 0)
            {
                //throw new MESReturnMessage($@"No CARTON in {packSession.Value.ToString()}");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152458", new string[] { packSession.Value.ToString() }));
            }
            R_MES_LOG checkLog = null;
            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(Station.SFCDB, Station.DBType);
            int totalTimes = cartonList.Count();
            string skuno = cartonList[0].SKUNO;
            UIInputData O = new UIInputData() { };
            O.Timeout = 3000000;
            O.IconType = IconType.Warning;
            O.Type = UIInputType.String;
            O.Tittle = "Check each carton in pallet";
            O.ErrMessage = "No input";
            O.UIArea = new string[] { "40%", "80%" };
            O.OutInputs.Clear();

            var scanList = new DisplayOutPut()
            {
                DisplayType = "TextArea",
                Name = "ScanList",
                Value = ""
            };

            var scanTotal = new DisplayOutPut()
            {
                DisplayType = "Text",
                Name = "Total",
                Value = $@"0/{totalTimes}"
            };
            O.OutInputs.Add(scanList);
            O.OutInputs.Add(scanTotal);
            StringBuilder s = new StringBuilder();

            var checkLabel = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "LabelChecker"
                              && r.CATEGORY == "BigRoHSLabel" && r.CONTROLFLAG == "Y"
                              && r.FUNCTIONTYPE == "NOSYSTEM" && SqlSugar.SqlFunc.ToUpper(r.VALUE) == Station.StationName).ToList().FirstOrDefault();
            string tempCarton = "";
            for (var i = 0; i < totalTimes; i++)
            {
                O.Message = "SN";
                O.Name = "SN";
                O.CBMessage = "";
                while (true)
                {
                    var input_sn = O.GetUiInput(Station.API, UIInput.CanPrint, Station);
                    Station.LabelPrint.Clear();
                    Station.LabelPrints.Clear();
                    Station.LabelStillPrint.Clear();
                    if (input_sn == null)
                    {
                        O.CBMessage = $@"Please scan sn.";
                    }
                    else
                    {
                        string check_value = input_sn.ToString().Trim();
                        O.CBMessage = "";
                        if (string.IsNullOrEmpty(check_value))
                        {
                            O.CBMessage = $@"Please scan sn.";
                        }
                        else if (check_value.Equals("No input"))
                        {
                            throw new Exception("User Cancel");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152541"));
                        }
                        else
                        {
                            List<string> juniper = new List<string> { "VNJUNIPER", "FJZ" };
                            if (juniper.Contains(Station.BU))
                            {
                                check_value = check_value.ToUpper().StartsWith("S") ? (check_value.Length > 1 ? check_value.Substring(1, check_value.Length - 1) : check_value) : check_value;
                            }
                            else
                            {
                                T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
                                check_value = c_sku_detail.SNPreprocessor(Station.SFCDB, skuno, check_value, Station.StationName);
                            }
                            var carton = Station.SFCDB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((rp, rs, sn) => rs.SN_ID == sn.ID && rp.ID == rs.PACK_ID)
                                .Where((rp, rs, sn) => sn.SN == check_value && sn.VALID_FLAG == "1")
                                .Select((rp, rs, sn) => rp).ToList().FirstOrDefault();
                            if (carton == null)
                            {
                                O.CBMessage = $@"{check_value} not exists in {packSession.Value.ToString()} list";
                            }
                            else if (s.ToString().Contains(carton.PACK_NO))
                            {
                                O.CBMessage = $@"The carton[{carton.PACK_NO}] of {check_value} already scan!";
                            }
                            else
                            {
                                var k = cartonList.Find(r => r.PACK_NO == carton.PACK_NO);
                                if (k == null)
                                {
                                    O.CBMessage = $@"{check_value} not exists in {packSession.Value.ToString()} list";
                                }
                                else
                                {
                                    tempCarton = k.PACK_NO;
                                    s.Append(k.PACK_NO + ",");
                                    scanList.Value = s.ToString();
                                    scanTotal.Value = $@"{(i + 1).ToString()}/{totalTimes.ToString()}";
                                    cartonList.Remove(k);
                                    checkLog = new R_MES_LOG();
                                    checkLog.ID = t_r_mes_log.GetNewID(Station.BU, Station.SFCDB);
                                    checkLog.PROGRAM_NAME = "MESSation";
                                    checkLog.CLASS_NAME = "MESStation.Stations.StationActions.DataCheckers.CheckPack";
                                    checkLog.FUNCTION_NAME = "PalletCheckEachCarton";
                                    checkLog.LOG_MESSAGE = "Check each carton in pallet";
                                    checkLog.DATA1 = k.PACK_NO;
                                    checkLog.DATA2 = k.ID;
                                    checkLog.DATA3 = packSession.Value.ToString();
                                    checkLog.DATA4 = check_value;
                                    checkLog.EDIT_TIME = Station.SFCDB.ORM.GetDate();
                                    checkLog.EDIT_EMP = Station.LoginUser.EMP_NO;
                                    Station.SFCDB.ORM.Insertable<R_MES_LOG>(checkLog).ExecuteCommand();
                                    break;
                                }
                            }
                        }
                    }
                }
                if (checkLabel != null)
                {
                    O.Message = "BigRoHSLabel";
                    O.Name = "BigRoHSLabel";
                    O.CBMessage = "";
                    while (true)
                    {
                        var input_label = O.GetUiInput(Station.API, UIInput.Normal, Station);
                        if (input_label == null)
                        {
                            O.CBMessage = $@"Please scan  Big RoHS label.";
                        }
                        else
                        {
                            string check_value = input_label.ToString().Trim();
                            O.CBMessage = "";
                            if (string.IsNullOrEmpty(check_value))
                            {
                                O.CBMessage = $@"Please scan Big RoHS label.";
                            }
                            else if (check_value.Equals("No input"))
                            {
                                //throw new Exception("User Cancel");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152541"));
                            }
                            else
                            {
                                if (!checkLabel.EXTVAL.Equals(check_value))
                                {
                                    O.CBMessage = $@"Big RoHS label error!";
                                }
                                else
                                {
                                    checkLog = new R_MES_LOG();
                                    checkLog.ID = t_r_mes_log.GetNewID(Station.BU, Station.SFCDB);
                                    checkLog.PROGRAM_NAME = "MESSation";
                                    checkLog.CLASS_NAME = "MESStation.Stations.StationActions.DataCheckers.CheckPack";
                                    checkLog.FUNCTION_NAME = "PalletCheckEachCarton";
                                    checkLog.LOG_MESSAGE = "BigRoHSLabel";
                                    checkLog.DATA1 = tempCarton;
                                    checkLog.DATA2 = check_value;
                                    checkLog.EDIT_TIME = Station.SFCDB.ORM.GetDate();
                                    checkLog.EDIT_EMP = Station.LoginUser.EMP_NO;
                                    Station.SFCDB.ORM.Insertable<R_MES_LOG>(checkLog).ExecuteCommand();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CheckSnNextStationByPackNO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            string nextStation = Paras[1].VALUE.ToString().Trim();

            R_PACKING packObject = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(p => p.PACK_NO == sessionPackObject.Value.ToString()).ToList().FirstOrDefault();
            if (packObject != null)
            {
                List<R_SN> snList = new List<R_SN>();
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
                string strSnList = "";
                snList = t_r_packing.GetPakcingSNList(packObject.ID, packObject.PACK_TYPE, Station.SFCDB);
                if (snList.Count == 0)
                {
                    //throw new MESReturnMessage($"{packObject.PACK_NO} QTY is 0!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152613", new string[] { packObject.PACK_NO }));
                }
                List<R_SN> notCurrentStation = snList.Where(r => r.NEXT_STATION != nextStation).ToList().ToList();
                if (notCurrentStation.Count > 0)
                {
                    foreach (R_SN sn in notCurrentStation)
                    {
                        strSnList += sn.SN + ",";
                    }
                    //throw new MESReturnMessage($"The next station of [{strSnList}] in the {packObject.PACK_NO} is not equals {nextStation}");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152702", new string[] { strSnList, packObject.PACK_NO, nextStation }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { sessionPackObject.Value.ToString() }));
            }
        }

        public static void CheckSnLockStatusByPackNO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            R_PACKING packObject = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(p => p.PACK_NO == sessionPackObject.Value.ToString()).ToList().FirstOrDefault();
            if (packObject != null)
            {
                List<R_SN_LOCK> snLockList = new List<R_SN_LOCK>();
                string strSnList = "";
                if (packObject.PACK_TYPE == LogicObject.PackType.PALLET.ToString())
                {
                    snLockList = Station.SFCDB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_LOCK, R_PACKING>((rs, rp, rsp, rsl, rpp) => rs.SN == rsl.SN && rs.ID == rsp.SN_ID
                             && rsp.PACK_ID == rp.ID && rp.PARENT_PACK_ID == rpp.ID)
                        .Where((rs, rp, rsp, rsl, rpp) => rpp.PACK_NO == packObject.PACK_NO && rsl.LOCK_STATUS == "1")
                        .Select((rs, rp, rsp, rsl, rpp) => rsl).ToList();
                }
                else if (packObject.PACK_TYPE == LogicObject.PackType.CARTON.ToString())
                {
                    snLockList = Station.SFCDB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_LOCK>((rs, rp, rsp, rsl) => rs.SN == rsl.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                        .Where((rs, rp, rsp, rsl) => rp.PACK_NO == packObject.PACK_NO && rsl.LOCK_STATUS == "1")
                        .Select((rs, rp, rsp, rsl) => rsl).ToList();
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259", new string[] { }));
                }

                if (snLockList.Count > 0)
                {
                    foreach (var sn in snLockList)
                    {
                        strSnList += sn.SN + ",";
                    }
                    //throw new MESReturnMessage($"there SN's [{strSnList}] in the {packObject.PACK_NO}  are locked!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152749", new string[] { strSnList, packObject.PACK_NO }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { sessionPackObject.Value.ToString() }));
            }


        }
        public static void CheckMovePackWeight(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            LogicObject.Packing packObjct = new LogicObject.Packing();
            if (sessionPackObject.Value is LogicObject.Packing)
            {
                packObjct = (LogicObject.Packing)sessionPackObject.Value;
            }
            else
            {
                packObjct.DataLoad(sessionPackObject.Value.ToString(), Station.SFCDB, Station.DBType);
            }
            bool bWeight = Station.SFCDB.ORM.Queryable<R_WEIGHT>().Where(r => r.SNID == packObjct.PackID).Any();
            if (bWeight)
            {
                //throw new MESReturnMessage($"{packObjct.PackNo} already pass weight station,please don't move anything in {packObjct.PackNo}.please return to packout station.");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152848", new string[] { packObjct.PackNo, packObjct.PackNo }));
            }
        }
        /// <summary>
        /// 检查栈板工单对应的PO与出货DN的PO是否一致
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckPLDNComparePO(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (packSession == null || packSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            OleExec SFCDB = Station.SFCDB;
            string packno = packSession.Value.ToString();
            string dnNo, dnPO = "";
            dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();
            var dnStatus = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == dnNo).ToList().FirstOrDefault();
            if (dnStatus.DN_PLANT == "VJGS")
            {
                dnPO = SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN, O_ORDER_MAIN>((p1, p2, rsp, rsn, oom) => p1.ID == p2.PARENT_PACK_ID && p2.ID == rsp.PACK_ID && rsp.SN_ID == rsn.ID
                && rsn.WORKORDERNO == oom.PREWO).Where((p1, p2, rsp, rsn, oom) => p1.PACK_NO == packno && rsn.VALID_FLAG == "1").Select((p1, p2, rsp, rsn, oom) => oom.PONO).ToList().FirstOrDefault().ToString();
                if (string.IsNullOrEmpty(dnPO))
                {
                    //throw new MESReturnMessage($@"PO is Null (O_ORDER_MAIN)");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152942"));
                }
                if (dnStatus.PO_NO != dnPO)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210629185217", new string[] { dnStatus.PO_NO, dnPO }));
                }
            }

        }

        /// <summary>
        /// Carton is not closed then reprint label
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckQtyPackReprintCarton(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null || SkuSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SnSession == null || SnSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            OleExec SFCDB = Station.SFCDB;
            string sku = SkuSession.Value.ToString();
            string sn = SnSession.Value.ToString();
            
            var sql = $@"select * from  R_SN rsn, R_SN_PACKING rsnp, R_PACKING rpc, R_PACKING rpp
                            where rsn.ID = rsnp.SN_ID 
                            and rsnp.PACK_ID = rpc.ID 
                            and rpc.PARENT_PACK_ID = rpp.ID
                            and rsn.SN = '{sn}'  
                            and rsn.SKUNO = '{sku}' 
                            and rsn.VALID_FLAG = '1' 
                            and rpc.MAX_QTY = rpc.QTY 
                            and rpc.CLOSED_FLAG = '1'";

            DataTable dt = Station.SFCDB.RunSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20220214152139"));
            }
        }

        /// <summary>
        /// Carton is not closed then not reprint label
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckQtyReprintStation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession reprintStation = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (reprintStation == null || reprintStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            string _strsn = SNSession.Value.ToString();
            string _station = reprintStation.Value.ToString();
            bool _checkSNPack = Station.SFCDB.ORM.Queryable<R_SN>().Any(t => t.SN == _strsn && t.VALID_FLAG == "1" && t.PACKED_FLAG == "1");
            if (_station == "CARTON" && _checkSNPack)
            {
                var sql = $@"select * from  R_SN rsn, R_SN_PACKING rsnp, R_PACKING rpc, R_PACKING rpp
                            where rsn.ID = rsnp.SN_ID 
                            and rsnp.PACK_ID = rpc.ID 
                            and rpc.PARENT_PACK_ID = rpp.ID
                            and rsn.SN = '{_strsn}'  
                            and rsn.VALID_FLAG = '1'
                            and rpc.CLOSED_FLAG = '1' ";

                DataTable dt = Station.SFCDB.RunSelect(sql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20220214152139"));
                }
            }
        }

        public static void CheckPacknoIsClose(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            try
            {
                CartionBase carton = (CartionBase)sessionPackObject.Value;
                string Cartonn = carton.DATA.PACK_NO.ToString();
                var res = Station.SFCDB.ORM.Queryable<R_PACKING>()
               .Where(x => x.PACK_NO == Cartonn && x.CLOSED_FLAG == "1").Any();
                if (res)
                {
                    throw new Exception($@"This is carton {Cartonn} closed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
