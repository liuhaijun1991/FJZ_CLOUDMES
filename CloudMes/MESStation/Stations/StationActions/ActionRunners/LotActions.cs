using MESDataObject;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class LotActions
    {

        /// <summary>
        /// 按AQL TYPE及工站進批次
        /// 批次生成邏輯
        /// 1、機種配置的了對應的AQL TYPE
        /// 2、路由中有對應的測試工站，且該測試工站的前一站是SAMPLETESTLOT類型        
        /// 3、要配置好LOT生成規則
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InLotByAQLTypeAndStationAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //用於往下一個Action傳遞數據
            MESStationSession sessionLotObject = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionLotObject == null)
            {
                sessionLotObject = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionLotObject);
            }

            string aql_type = Paras[2].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(aql_type))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            string test_station = Paras[3].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(test_station))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            string lot_rule = Paras[4].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(lot_rule))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }
             
            LotNo lotObject = new LotNo();
            SN snObject = (SN)SNSession.Value;
            R_LOT_STATUS lotObj = lotObject.InLotByAQLTypeAndStation(Station, snObject, aql_type, test_station, lot_rule);
            sessionLotObject.Value = lotObj;
        }

        /// <summary>
        /// 按配置隔多少PCS抽1PCS進行測試
        /// 如在5DX的前一站隔N(=設定的數量) PCS 抽1 PCS進行5DX測試，其餘SN跳過5DX
        /// 注意：該Action請在過站的Action之後再添加
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SkipTheTestAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string control_type = Paras[1].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(control_type))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            SN snObject = (SN)SNSession.Value;
            //T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_r_profile t_r_profile = new T_r_profile(Station.SFCDB, Station.DBType);

            List<C_ROUTE_DETAIL> listNextRoute = new List<C_ROUTE_DETAIL>();

            //C_SKU_DETAIL detailObject = t_c_sku_detail.GetSkuDetail(control_type, control_type, snObject.SkuNo, Station.SFCDB);
            R_F_CONTROL controlObj = t_r_function_control.GetControl(Station.SFCDB, control_type, control_type, snObject.SkuNo);
            R_F_CONTROL controlStation = Station.SFCDB.ORM.Queryable<R_F_CONTROL>()
                .Where(r => r.CATEGORY == control_type && r.FUNCTIONNAME == control_type && r.FUNCTIONTYPE == "SYSTEM" && r.CONTROLFLAG == "Y")
                .ToList().FirstOrDefault();

            R_SN snObj = null;
            r_profile profileObj = null;
            snObj = t_r_sn.LoadData(snObject.SerialNo, Station.SFCDB);
            int result = 0;
            string detail_id = "", re = "";

            if (controlObj != null)
            {
                if (controlObj.EXTVAL.Trim() == "")
                {
                    throw new MESReturnMessage($@"{snObj.SKUNO},{control_type} Extval Is Null!");
                }
                if (controlStation != null && !controlStation.CATEGORYDEC.Equals(snObj.NEXT_STATION))
                {
                    //如果SN的下一站不是要跳過的站，則退出
                    return;
                }               
                //1、先取是否有抽測記錄，有則更新抽測數量，無則新增記錄
                profileObj = t_r_profile.GetProfile(control_type, control_type, control_type, snObj.SKUNO, Station.SFCDB);
                if (profileObj == null)
                {
                    profileObj = new r_profile();
                    profileObj.ID = t_r_profile.GetNewID(Station.BU, Station.SFCDB);
                    profileObj.PROFILENAME = control_type;
                    profileObj.PROFILECATEGORY = control_type;
                    profileObj.PROFILETYPE = control_type;
                    profileObj.PROFILEVALUE = snObj.SKUNO;
                    profileObj.PROFILELEVEL = 0;
                    profileObj.NOTE1 = controlObj.EXTVAL.Trim();
                    profileObj.NOTE2 = "1";
                    profileObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                    profileObj.EDIT_TIME = t_r_profile.GetDBDateTime(Station.SFCDB);
                    result = t_r_profile.SaveNewProfile(profileObj, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_PROFILE:" + snObject.SerialNo, "ADD" }));
                    }
                }
                if (profileObj.NOTE1.Trim() != controlObj.EXTVAL.Trim())
                {
                    profileObj.NOTE1 = controlObj.EXTVAL.Trim();
                    result = t_r_profile.Update(profileObj, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_PROFILE:" + snObject.SerialNo, "UPDATE" }));
                    }
                }
                //2、判斷是否滿足跳過測試的條件，滿足則直接更新R_SN的下一站，并在過站記錄表加一筆跳過記錄
                if (profileObj.NOTE2.Trim() != controlObj.EXTVAL.Trim() && Convert.ToInt32(profileObj.NOTE2.Trim()) < Convert.ToInt32(controlObj.EXTVAL.Trim()))//NOTE2>EXTVAL 時無法自動修正
                {
                    profileObj.NOTE2 = (Convert.ToInt32(profileObj.NOTE2.Trim()) + 1).ToString();
                    profileObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                    profileObj.EDIT_TIME = t_r_profile.GetDBDateTime(Station.SFCDB);

                    listNextRoute = t_c_route_detail.GetAllNextStationsByCurrentStation(snObj.ROUTE_ID, snObj.NEXT_STATION, Station.SFCDB);
                    listNextRoute.RemoveAt(0);
                    if (listNextRoute.Count > 0)
                    {
                        //最後一站不是抽測工站
                        result = t_r_sn.UpdateSNCurrentNextStation(snObj.SN, snObj.NEXT_STATION, listNextRoute.FirstOrDefault().STATION_NAME, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + snObject.SerialNo, "UPDATE" }));
                        }
                    }
                    else
                    {
                        //最後一站是抽測工站
                        C_ROUTE_DETAIL lastStation = t_c_route_detail.GetStationRoute(snObj.NEXT_STATION, snObj.ROUTE_ID, Station.SFCDB);
                        if (lastStation.STATION_TYPE == "JOBFINISH")
                        {
                            snObj.CURRENT_STATION = lastStation.STATION_NAME;
                            snObj.NEXT_STATION = lastStation.STATION_TYPE;
                            snObj.COMPLETED_FLAG = "1";
                            snObj.COMPLETED_TIME = t_r_sn.GetDBDateTime(Station.SFCDB);
                            result = t_r_sn.Update(snObj, Station.SFCDB);
                            if (result <= 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + snObject.SerialNo, "UPDATE" }));
                            }
                        }
                    }
                    detail_id = t_r_sn_station_detail.GetNewID(Station.BU, Station.SFCDB);
                    re = t_r_sn_station_detail.AddDetailToRSnStationDetail(detail_id, snObj, Station.Line, control_type, $@"Skip Over {snObj.NEXT_STATION}", Station.SFCDB);
                    bool b = int.TryParse(re, out result);
                    if (!b)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + snObject.SerialNo, "UPDATE" }));
                    }
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + snObject.SerialNo, "UPDATE" }));
                    }
                }
                else
                {
                    profileObj.NOTE2 = "1";
                }
                result = t_r_profile.Update(profileObj, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_PROFILE:" + snObject.SerialNo, "UPDATE" }));
                }
            }
        }
        /// <summary>
        /// 關閉LOT 按鈕
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CloseLotAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            
            MESStationSession sessionLotObj = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionLotObj == null || sessionLotObj.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            R_LOT_STATUS lotStatus = null;
            Row_R_LOT_STATUS rowLot = null;
            int result;
            if (sessionLotObj.Value is LotNo )
            {
                LotNo lotObj = (LotNo)sessionLotObj.Value;
                rowLot = t_r_lot_status.GetByLotNo(lotObj.LOT_NO, Station.SFCDB);
                if (rowLot == null)
                {
                    throw new MESReturnMessage("Get Lot Error!");
                }
                lotStatus = rowLot.GetDataObject();
            }
            else if (sessionLotObj.Value is string )
            {
                rowLot = t_r_lot_status.GetByLotNo(sessionLotObj.Value.ToString().Trim(), Station.SFCDB);
                if (rowLot == null)
                {
                    throw new MESReturnMessage("Get Lot Error!");
                }
                lotStatus = rowLot.GetDataObject();
            }
            else if (sessionLotObj.Value is R_LOT_STATUS )
            {
                lotStatus = (R_LOT_STATUS)sessionLotObj.Value;                
            }
            if (lotStatus != null)
            {
                lotStatus.CLOSED_FLAG = "1";
                lotStatus.EDIT_EMP = Station.LoginUser.EMP_NO;
                lotStatus.EDIT_TIME = t_r_lot_status.GetDBDateTime(Station.SFCDB);
                result = t_r_lot_status.UpdateLot(lotStatus, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_STATUS:" + lotStatus.LOT_NO, "UPDATE" }));
                }
            }
            sessionLotObj.Value = null;
        }
        /// <summary>
        /// SN 抽測過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNSampleAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            //獲取到 SN 對象
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //獲取到 LOT 對象
            MESStationSession sessionLotObj = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionLotObj == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            string pass_flag = Paras[2].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(pass_flag))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            if (sessionLotObj.Value != null)
            {
                SN snObject = (SN)sessionSN.Value;
                T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
                T_R_LOT_DETAIL t_r_lot_detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                R_LOT_STATUS lotStatus = null;
                Row_R_LOT_STATUS rowLot = null;
                R_LOT_DETAIL lotDetail = null;

                List<R_LOT_DETAIL> lotDetailList = new List<R_LOT_DETAIL>();
                List<string> SNlist = new List<string>();
                
                bool bPass = pass_flag.ToUpper().Equals("PASS") ? true : false;
                int result;
                if (sessionLotObj.Value is LotNo)
                {
                    LotNo lotObj = (LotNo)sessionLotObj.Value;
                    rowLot = t_r_lot_status.GetByLotNo(lotObj.LOT_NO, Station.SFCDB);
                    if (rowLot == null)
                    {
                        throw new MESReturnMessage("Get Lot Error!");
                    }
                    lotStatus = rowLot.GetDataObject();
                }
                else if (sessionLotObj.Value is string)
                {
                    rowLot = t_r_lot_status.GetByLotNo(sessionLotObj.Value.ToString().Trim(), Station.SFCDB);
                    if (rowLot == null)
                    {
                        throw new MESReturnMessage("Get Lot Error!");
                    }
                    lotStatus = rowLot.GetDataObject();
                }
                else if (sessionLotObj.Value is R_LOT_STATUS)
                {
                    lotStatus = (R_LOT_STATUS)sessionLotObj.Value;
                }
                if (lotStatus == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000091", new string[] { sessionLotObj.Value.ToString() }));
                }
                if (lotStatus.LOT_STATUS_FLAG != "0")
                {
                    throw new MESReturnMessage($@"{lotStatus.LOT_NO} Already Sampling!");
                }                
                lotDetailList = t_r_lot_detail.GetLotDetailByLotNo(lotStatus.LOT_NO, Station.SFCDB);
                lotDetail = lotDetailList.Find(l => l.SN == snObject.SerialNo);
                if (lotDetail == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000094", new string[] { snObject.SerialNo, lotStatus.LOT_NO }));
                }
                if (lotDetail.SAMPLING == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000093", new string[] { snObject.SerialNo }));
                }
                if (bPass)
                {
                    lotDetail.STATUS = "1"; //1 表示抽檢通過 
                    lotStatus.PASS_QTY++; //更新 R_LOT_STATUS PASS_QTY
                }
                else
                {
                    lotDetail.STATUS = "0";//0 表示抽檢失敗
                    lotStatus.FAIL_QTY++; //更新 R_LOT_STATUS FAIL_QTY
                    //具體LAIL 原因待寫
                    lotDetail.FAIL_CODE = "";
                    lotDetail.FAIL_LOCATION = "";
                    lotDetail.DESCRIPTION = "";
                }   

                if (lotStatus.FAIL_QTY != 0 && lotStatus.FAIL_QTY >= lotStatus.REJECT_QTY)
                {
                    lotStatus.LOT_STATUS_FLAG = "2";// 2 表示整個 Lot 不良
                    //更新 R_LOT_DETAIL 鎖定LOT 中所有
                    result = Station.SFCDB.ORM.Updateable<R_LOT_DETAIL>().UpdateColumns(r => new R_LOT_DETAIL { STATUS = "4" }).Where(r => r.LOT_ID == lotStatus.ID).ExecuteCommand();
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_STATUS:" + snObject.SerialNo, "UPDATE" }));
                    }
                }               
                lotDetail.SAMPLING = "1";//1 表示被抽檢了
                lotDetail.EDIT_TIME = t_r_lot_detail.GetDBDateTime(Station.SFCDB);
                lotDetail.EDIT_EMP = Station.LoginUser.EMP_NO;
                result = t_r_lot_detail.UpdateObject(lotDetail, Station.SFCDB);

                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_DETAIL:" + snObject.SerialNo, "UPDATE" }));
                }

                //抽滿
                if (lotStatus.PASS_QTY + lotStatus.FAIL_QTY >= lotStatus.SAMPLE_QTY)
                {
                    lotStatus.LOT_STATUS_FLAG = "1"; // 1 已抽檢完                    
                    //lotDetailList = Station.SFCDB.ORM.Queryable<R_LOT_DETAIL>().Where(r => r.LOT_ID == lotStatus.ID && r.SAMPLING == "0").ToList();

                    var rsnlist = Station.SFCDB.ORM.Queryable<R_SN, R_LOT_DETAIL>((rs, rld) => rs.SN == rld.SN).Where(
                        (rs, rld) => rld.LOT_ID == lotStatus.ID && rld.SAMPLING == "0"&& rs.VALID_FLAG == "1").Select((rs, rld) =>rs).ToList();
                    //SNlist = lotDetailList.Select(r => r.SN).ToList();
                    t_r_sn.LotsPassStation(rsnlist, Station.Line, Station.StationName, Station.StationName, Station.BU, pass_flag.Trim(), Station.LoginUser.EMP_NO, Station.SFCDB);
                    //記錄通過數 ,UPH
                    foreach (var SN in rsnlist)
                    {
                        t_r_sn.RecordYieldRate(lotDetail.WORKORDERNO, 1, SN.SN, "PASS", Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                        t_r_sn.RecordUPH(lotDetail.WORKORDERNO, 1, SN.SN, "PASS", Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                    }
                    sessionLotObj.Value = null;
                }

                lotStatus.EDIT_EMP = Station.LoginUser.EMP_NO;
                lotStatus.EDIT_TIME = t_r_lot_status.GetDBDateTime(Station.SFCDB);
                result = t_r_lot_status.UpdateLot(lotStatus, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_STATUS:" + snObject.SerialNo, "UPDATE" }));
                }
            }
        }

        /// <summary>
        /// 重工工站入批次
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReworkToStationInLotAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            //獲取到 SN 對象
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //獲取到重工工站
            MESStationSession sessionStation = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string aql_type = Paras[2].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(aql_type))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            string test_station = Paras[3].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(test_station))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            string lot_rule = Paras[4].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(lot_rule))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }
            //重工工站與傳入的指定進批次的工站不一致則退出
            if (sessionStation.Value.ToString() != test_station)
            {
                return;
            }
            SN snObject = (SN)sessionSN.Value;
            T_C_SKU_AQL t_c_sku_aql = new T_C_SKU_AQL(Station.SFCDB, Station.DBType);
            T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL t_r_lot_detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            T_C_SEQNO t_c_seqno = new T_C_SEQNO(Station.SFCDB, Station.DBType);
            T_C_AQLTYPE t_c_aqltype = new T_C_AQLTYPE(Station.SFCDB, Station.DBType);
            List<C_SKU_AQL> listSkuAQL = t_c_sku_aql.GetAQLListBySkuAndType(Station.SFCDB, snObject.SkuNo, aql_type);
            R_LOT_STATUS lot5DX = null;
            DateTime DBDateTime;
            int result = 0;
            string lot_id = "";
            int lot_qty, Sample_Qty;

            if (listSkuAQL.Count > 0)
            {
                lot5DX = t_r_lot_status.GetNotClosingLot(snObject.SkuNo, listSkuAQL.FirstOrDefault().AQLTYPE, test_station, true, Station.SFCDB);
                DBDateTime= t_r_lot_status.GetDBDateTime(Station.SFCDB);
                if (lot5DX == null)
                {
                    //New A Lot                           
                    R_LOT_STATUS newLot = new R_LOT_STATUS();
                    newLot.ID = t_r_lot_status.GetNewID(Station.BU, Station.SFCDB);
                    lot_id = newLot.ID;
                    newLot.LOT_NO = t_c_seqno.GetLotno(lot_rule, Station.SFCDB);
                    newLot.SKUNO = snObject.SkuNo;
                    newLot.AQL_TYPE = listSkuAQL.FirstOrDefault().AQLTYPE;
                    newLot.LOT_QTY = 1;
                    newLot.REJECT_QTY = 0;
                    newLot.SAMPLE_QTY = 1;
                    newLot.PASS_QTY = 0;
                    newLot.FAIL_QTY = 0;
                    newLot.CLOSED_FLAG = "0";
                    newLot.LOT_STATUS_FLAG = "0";
                    newLot.SAMPLE_STATION = test_station;
                    newLot.LINE = Station.Line;
                    newLot.EDIT_EMP = "REWORK";
                    newLot.EDIT_TIME = DBDateTime;
                    newLot.AQL_LEVEL = "";
                    result = t_r_lot_status.InsertNewLot(newLot, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_STATUS:" + snObject.SerialNo, "ADD" }));
                    }
                }
                else
                {
                    //Update Lot
                    lot_id = lot5DX.ID;
                    lot_qty = (int)lot5DX.LOT_QTY + 1;
                    Sample_Qty = t_c_aqltype.GetSampleQty(aql_type, lot_qty, Station.SFCDB);
                    lot5DX.LOT_QTY = lot5DX.LOT_QTY + 1;
                    lot5DX.SAMPLE_QTY = Sample_Qty;                    
                    lot5DX.EDIT_TIME = DBDateTime;
                    result = t_r_lot_status.UpdateLot(lot5DX, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_STATUS:" + snObject.SerialNo, "UPDATE" }));
                    }
                }
                R_LOT_DETAIL lotDetail = new R_LOT_DETAIL();
                lotDetail.ID = t_r_lot_detail.GetNewID(Station.BU, Station.SFCDB);
                lotDetail.LOT_ID = lot_id;
                lotDetail.SN = snObject.SerialNo;
                lotDetail.WORKORDERNO = snObject.WorkorderNo;
                lotDetail.CREATE_DATE = t_r_lot_detail.GetDBDateTime(Station.SFCDB);
                lotDetail.SAMPLING = "0";
                lotDetail.STATUS = "0";
                lotDetail.FAIL_CODE = "";
                lotDetail.FAIL_LOCATION = "";
                lotDetail.DESCRIPTION = "";
                lotDetail.CARTON_NO = "";
                lotDetail.PALLET_NO = "";
                lotDetail.EDIT_EMP = "REWORK";
                lotDetail.EDIT_TIME = DBDateTime;
                result = t_r_lot_detail.InsertNew(lotDetail, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_DETAIL:" + snObject.SerialNo, "ADD" }));
                }
            }
        }
    }
}
