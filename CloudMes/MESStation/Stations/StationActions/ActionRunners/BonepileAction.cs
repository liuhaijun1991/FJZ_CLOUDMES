using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Vertiv;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class BonepileAction
    {
        public static void ScanFailSaveBonepileAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSku == null || sessionSku.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionErrorCode = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionErrorCode == null || sessionErrorCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            T_R_CRITICAL_BONEPILE t_r_critical_bonepile = new T_R_CRITICAL_BONEPILE(Station.SFCDB, Station.DBType);
            T_R_NORMAL_BONEPILE t_r_normal_bonepile = new T_R_NORMAL_BONEPILE(Station.SFCDB, Station.DBType);
            T_R_RMA_BONEPILE t_r_rma_bonepile = new T_R_RMA_BONEPILE(Station.SFCDB, Station.DBType);
            T_C_SERIES t_c_series = new T_C_SERIES(Station.SFCDB, Station.DBType);
            T_C_CUSTOMER t_c_customer = new T_C_CUSTOMER(Station.SFCDB, Station.DBType);
            T_C_ERROR_CODE t_c_error_code = new T_C_ERROR_CODE(Station.SFCDB, Station.DBType);
            T_R_PN_MASTER_DATA t_r_pn_master_data = new T_R_PN_MASTER_DATA(Station.SFCDB, Station.DBType);


            R_CRITICAL_BONEPILE criticalBonepile = null;
            R_NORMAL_BONEPILE normalBonepile = null;
            R_RMA_BONEPILE rmaBonepile = null;
            C_SERIES series = null;
            R_PN_MASTER_DATA masterObj = null;

            int result;
            DateTime sysDateTime;
            SN snObj = (SN)sessionSN.Value;
            SKU skuObj = (SKU)sessionSku.Value;
            C_ERROR_CODE errorObj = null;
            if (sessionErrorCode.Value is string)
            {
                errorObj = t_c_error_code.GetByErrorCode(sessionErrorCode.Value.ToString(), Station.SFCDB);
            }
            else if (sessionErrorCode.Value is C_ERROR_CODE)
            {
                errorObj = (C_ERROR_CODE)sessionErrorCode.Value;
            }
            
            //bool IsFailStation = Station.SFCDB.ORM.Queryable<R_Station>().Any(r => r.STATION_NAME == Station.StationName && r.FAIL_STATION_FLAG == 1);           
            //Only Fail Station And Scan Fail Do Insert/Update Bonepile
            //if (IsFailStation)
            //{
            sysDateTime = t_r_critical_bonepile.GetDBDateTime(Station.SFCDB);
            //Critical Bonepile
            criticalBonepile = t_r_critical_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo);
            if (criticalBonepile != null)
            {
                criticalBonepile.WORKORDERNO = snObj.WorkorderNo;
                criticalBonepile.CURRENT_STATION = Station.StationName;
                criticalBonepile.CURRENT_STATUS = "1";
                criticalBonepile.LASTEDIT_BY = Station.LoginUser.EMP_NO;
                criticalBonepile.LASTEDIT_DATE = sysDateTime;

                result = t_r_critical_bonepile.Update(Station.SFCDB, criticalBonepile);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_CRITICAL_BONEPILE:" + snObj.SerialNo, "UPDATE" }));
                }
            }

            //RMA Bonepile
            rmaBonepile = t_r_rma_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo);
            if (rmaBonepile != null)
            {
                //更新邏輯待寫
            }

            //Normal Bonepile
            normalBonepile = t_r_normal_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo);
            if (normalBonepile != null)
            {
                normalBonepile.CURRENT_WO = snObj.WorkorderNo;
                normalBonepile.CURRENT_STATION = Station.StationName;
                normalBonepile.CURRENT_STATUS = "1";
                normalBonepile.LASTEDIT_BY = Station.LoginUser.EMP_NO;
                normalBonepile.LASTEDIT_DATE = sysDateTime;
                result = t_r_normal_bonepile.Update(Station.SFCDB, normalBonepile);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_NORMAL_BONEPILE:" + snObj.SerialNo, "UPDATE" }));
                }
            }
            else
            {
                if (criticalBonepile == null && rmaBonepile == null)
                {
                    normalBonepile = new R_NORMAL_BONEPILE();
                    series = t_c_series.GetDetailById(Station.SFCDB, skuObj.CSeriesId);
                    if (series != null)
                    {
                        normalBonepile.SUB_SERIES = series.SERIES_NAME;// "產品系列";
                        normalBonepile.PRODUCT_SERIES = t_c_customer.GetCustomerName(Station.SFCDB, series.CUSTOMER_ID);//"客戶名稱";
                    }
                    masterObj = t_r_pn_master_data.GetMasterObj(Station.SFCDB, snObj.SkuNo);
                    T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
                    List<R_F_CONTROL> listControl = t_r_function_control.GetListByFcv("NormalBonepileCategoty", Station.SFCDB);
                    R_F_CONTROL control = listControl.FindAll(r => r.VALUE == Station.StationName).OrderByDescending(r => r.CREATETIME).FirstOrDefault();
                    normalBonepile.ID = t_r_normal_bonepile.GetNewID(Station.BU, Station.SFCDB);
                    normalBonepile.SN = snObj.SerialNo;
                    normalBonepile.WORKORDERNO = snObj.WorkorderNo;
                    normalBonepile.SKUNO = snObj.SkuNo;
                    normalBonepile.PRODUCT_NAME = skuObj.SkuName;// "產品名稱";                        
                    normalBonepile.FAIL_STATION = Station.StationName;
                    normalBonepile.FAIL_DATE = sysDateTime;
                    normalBonepile.FAILURE_SYMPTOM = errorObj == null ? "" : errorObj.ENGLISH_DESC;
                    normalBonepile.BONEPILE_CATEGORY = control != null ? control.CATEGORY : Station.StationName;
                    normalBonepile.RMA_FLAG = "0";
                    normalBonepile.CLOSED_FLAG = "0";
                    normalBonepile.CRITICAL_BONEPILE_FLAG = "0";
                    normalBonepile.HARDCORE_BOARD = "";
                    normalBonepile.PRICE = masterObj == null ? 0 : masterObj.PRICE1;
                    normalBonepile.CURRENT_WO = snObj.WorkorderNo;
                    normalBonepile.CURRENT_STATION = snObj.NextStation;
                    normalBonepile.CURRENT_STATUS = "1";
                    normalBonepile.LASTEDIT_BY = Station.LoginUser.EMP_NO;
                    normalBonepile.LASTEDIT_DATE = sysDateTime;
                    result = t_r_normal_bonepile.Save(Station.SFCDB, normalBonepile);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_NORMAL_BONEPILE:" + snObj.SerialNo, "ADD" }));
                    }
                }
            }
            //}
        }

        public static void RepairUpdateBonepileAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionFailCodeID = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionFailCodeID == null || sessionFailCodeID.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            T_R_NORMAL_BONEPILE t_r_normal_bonepile = new T_R_NORMAL_BONEPILE(Station.SFCDB, Station.DBType);
            T_R_REPAIR_ACTION t_r_repair_action = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            T_R_REPAIR_MAIN t_r_repair_main = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            T_C_ACTION_CODE t_c_action_code = new T_C_ACTION_CODE(Station.SFCDB, Station.DBType);
            T_C_ERROR_CODE t_c_error_code = new T_C_ERROR_CODE(Station.SFCDB, Station.DBType);

            string failCodeID = sessionFailCodeID.Value.ToString();
            int result;
            SN snObj = (SN)sessionSN.Value;
            R_NORMAL_BONEPILE normalBonepile = t_r_normal_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo);
            R_REPAIR_ACTION action = t_r_repair_action.GetActionByFailCodeID(Station.SFCDB, failCodeID);
            R_REPAIR_MAIN repairMain = t_r_repair_main.GetSNBySN(snObj.SerialNo, Station.SFCDB);           

            if(action==null)
            {
                throw new MESReturnMessage($@"Get Repair Action Info Fail!{failCodeID}");
            }
            if (normalBonepile != null && repairMain != null && normalBonepile.FAIL_STATION == repairMain.FAIL_STATION)
            {
                C_ACTION_CODE actionCode = t_c_action_code.GetByActionCode(action.ACTION_CODE, Station.SFCDB);
                C_ERROR_CODE errorCode = t_c_error_code.GetByErrorCode(action.REASON_CODE, Station.SFCDB);
                normalBonepile.REPAIR_ACTION = actionCode == null ? "" : actionCode.ENGLISH_DESC;
                normalBonepile.DEFECT_DESCRIPTION = errorCode == null ? "" : errorCode.ENGLISH_DESC;
                normalBonepile.REPAIR_DATE = action.REPAIR_TIME;
                normalBonepile.REMARK = action.DESCRIPTION;
                normalBonepile.LASTEDIT_BY = Station.LoginUser.EMP_NO;
                normalBonepile.LASTEDIT_DATE = action.REPAIR_TIME;
                if (normalBonepile.FAIL_STATION.ToUpper() == "COSMETIC"|| normalBonepile.FAIL_STATION.ToUpper() == "COSMETIC-FAILURE")
                {
                    normalBonepile.CLOSED_FLAG = "1";
                    normalBonepile.CLOSED_BY = Station.LoginUser.EMP_NO;
                    normalBonepile.CLOSED_DATE = normalBonepile.LASTEDIT_DATE;
                    normalBonepile.CLOSED_REASON = Station.StationName;
                }
                result = t_r_normal_bonepile.Update(Station.SFCDB, normalBonepile);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_NORMAL_BONEPILE:" + snObj.SerialNo, "UPDATE" }));
                }
            }
        }

        public static void ReworkUpdateBonepileAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSku == null || sessionSku.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            MESStationSession sessionNextStation = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionNextStation == null || sessionNextStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            SN snObj = (SN)sessionSN.Value;
            WorkOrder woObj = (WorkOrder)sessionWO.Value;
            SKU skuObj = (SKU)sessionSku.Value;
            string nextStation = sessionNextStation.Value.ToString();
            string storage = Paras[4].VALUE.ToString().ToUpper();
            int result;

            T_R_NORMAL_BONEPILE t_r_normal_bonepile = new T_R_NORMAL_BONEPILE(Station.SFCDB, Station.DBType);
            R_NORMAL_BONEPILE normalBonepile = t_r_normal_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo);
            T_C_SERIES t_c_series = new T_C_SERIES(Station.SFCDB, Station.DBType);
            T_C_CUSTOMER t_c_customer = new T_C_CUSTOMER(Station.SFCDB, Station.DBType);
            T_R_MRB t_r_mrb = new T_R_MRB(Station.SFCDB, Station.DBType);

            if (normalBonepile != null )
            { 
                normalBonepile.HARDCORE_BOARD = "";
                normalBonepile.WORKORDERNO = woObj.WorkorderNo;
                normalBonepile.SKUNO = woObj.SkuNO;
                normalBonepile.PRODUCT_NAME = skuObj.SkuName;
                C_SERIES series = t_c_series.GetDetailById(Station.SFCDB, skuObj.CSeriesId);
                if (series != null)
                {
                    normalBonepile.SUB_SERIES = series.SERIES_NAME;// "產品系列";
                    normalBonepile.PRODUCT_SERIES = t_c_customer.GetCustomerName(Station.SFCDB, series.CUSTOMER_ID);//"客戶名稱";
                }
                R_MRB mrb = t_r_mrb.GetMrbList(Station.SFCDB, snObj.SerialNo).FirstOrDefault();
                normalBonepile.CURRENT_WO = woObj.WorkorderNo;
                normalBonepile.CURRENT_STATION = nextStation;
                normalBonepile.CURRENT_STATUS = mrb.TO_STORAGE.ToUpper() == storage ? "1" : "0";//入盤點倉是1，否則是0
                normalBonepile.SHIPPED_FLAG = "0";
                normalBonepile.SCRAPPED_FLAG = "0";
                normalBonepile.LASTEDIT_BY = Station.LoginUser.EMP_NO;
                normalBonepile.LASTEDIT_DATE = t_r_normal_bonepile.GetDBDateTime(Station.SFCDB);
                result = t_r_normal_bonepile.Update(Station.SFCDB, normalBonepile);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_NORMAL_BONEPILE:" + snObj.SerialNo, "UPDATE" }));
                }
            }

        }

        public static void ReplaceSNUpdateBonepileAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionOldSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOldSN == null || sessionOldSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionNewSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionNewSN == null || sessionNewSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            SN snObjNew;
            SN snObjOld;
            string oldSN = "";
            string newSN = "";
           
            if (sessionOldSN.Value is string)
            {
                oldSN = sessionOldSN.Value.ToString();
            }
            else
            {
                snObjOld = (SN)sessionOldSN.Value;
                oldSN = snObjOld.SerialNo;
            }
            if (sessionNewSN.Value is string)
            {
                newSN = sessionNewSN.Value.ToString();
            }
            else
            {
                snObjNew = (SN)sessionNewSN.Value;
                newSN = snObjNew.SerialNo;
            }            
            Station.SFCDB.ORM.Updateable<R_NORMAL_BONEPILE>().SetColumns(r => new R_NORMAL_BONEPILE { SN = newSN }).Where(r => r.SN == oldSN).ExecuteCommand();
            Station.SFCDB.ORM.Updateable<R_RMA_BONEPILE>().SetColumns(r => new R_RMA_BONEPILE { SN = newSN }).Where(r => r.SN == oldSN).ExecuteCommand();
        }

        public static void PassStationUpdateBonepileAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }            
            SN snObj = (SN)sessionSN.Value;
            T_R_NORMAL_BONEPILE t_r_normal_bonepile = new T_R_NORMAL_BONEPILE(Station.SFCDB, Station.DBType);
            R_NORMAL_BONEPILE normalBonepile = t_r_normal_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo, Station.StationName);

            T_R_CRITICAL_BONEPILE t_r_critical_bonepile = new T_R_CRITICAL_BONEPILE(Station.SFCDB, Station.DBType);
            R_CRITICAL_BONEPILE criticalBonepile = t_r_critical_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo, Station.StationName);

            DateTime sysdate = t_r_normal_bonepile.GetDBDateTime(Station.SFCDB);
            if (normalBonepile != null)
            {
                normalBonepile.CURRENT_WO = snObj.WorkorderNo;
                normalBonepile.CURRENT_STATION = Station.StationName;
                normalBonepile.CURRENT_STATUS = "0";
                normalBonepile.LASTEDIT_BY = Station.LoginUser.EMP_NO;
                normalBonepile.LASTEDIT_DATE = sysdate;
                normalBonepile.CLOSED_FLAG = "1";
                normalBonepile.CLOSED_BY = Station.LoginUser.EMP_NO;
                normalBonepile.CLOSED_DATE = sysdate;
                normalBonepile.CLOSED_REASON = Station.StationName;
                //if (normalBonepile.FAIL_STATION == Station.StationName || normalBonepile.FAIL_STATION == "COSMETIC-FAILURE"
                //    || normalBonepile.FAIL_STATION == "ROTATION TEST" || normalBonepile.FAIL_STATION.StartsWith("ORT"))
                //{
                //    normalBonepile.CLOSED_FLAG = "1";
                //    normalBonepile.CLOSED_BY= Station.LoginUser.EMP_NO;
                //    normalBonepile.CLOSED_DATE= sysdate;
                //}
                t_r_normal_bonepile.Update(Station.SFCDB, normalBonepile);
            }

            if (criticalBonepile != null)
            {
                criticalBonepile.WORKORDERNO = snObj.WorkorderNo;
                criticalBonepile.CURRENT_STATION = Station.StationName;
                criticalBonepile.CURRENT_STATUS = "0";
                criticalBonepile.LASTEDIT_BY = Station.LoginUser.EMP_NO;
                criticalBonepile.LASTEDIT_DATE = sysdate;
                criticalBonepile.CLOSED_FLAG = "1";
                criticalBonepile.CLOSED_BY = Station.LoginUser.EMP_NO;
                criticalBonepile.CLOSED_DATE = sysdate;
                criticalBonepile.CLOSED_REASON = Station.StationName;
                //if (criticalBonepile.FAIL_STATION == Station.StationName || criticalBonepile.FAIL_STATION == "COSMETIC-FAILURE"
                //    || criticalBonepile.FAIL_STATION == "ROTATION TEST" )
                //{
                //    criticalBonepile.CLOSED_FLAG = "1";
                //    criticalBonepile.CLOSED_BY = Station.LoginUser.EMP_NO;
                //    criticalBonepile.CLOSED_DATE = sysdate;
                //}
                t_r_critical_bonepile.Update(Station.SFCDB, criticalBonepile);
            }

            //RMA bonepile 待寫
        }

        public static void CloseBonepileAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObj = (SN)sessionSN.Value;
            T_R_NORMAL_BONEPILE t_r_normal_bonepile = new T_R_NORMAL_BONEPILE(Station.SFCDB, Station.DBType);
            R_NORMAL_BONEPILE normalBonepile = t_r_normal_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo);

            T_R_CRITICAL_BONEPILE t_r_critical_bonepile = new T_R_CRITICAL_BONEPILE(Station.SFCDB, Station.DBType);
            R_CRITICAL_BONEPILE criticalBonepile = t_r_critical_bonepile.GetOpenRecord(Station.SFCDB, snObj.SerialNo);

            DateTime sysdate = Station.GetDBDateTime();
            int result = 0;
            if (snObj.ScrapedFlag == "1")
            {
                if (normalBonepile != null)
                {
                    normalBonepile.CLOSED_BY = Station.LoginUser.EMP_NO;
                    normalBonepile.CLOSED_DATE = sysdate;
                    normalBonepile.CLOSED_REASON = Station.StationName;
                    normalBonepile.CLOSED_FLAG = "1";
                    normalBonepile.CURRENT_STATION = snObj.CurrentStation;
                    result = t_r_normal_bonepile.Update(Station.SFCDB, normalBonepile);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_NORMAL_BONEPILE:" + snObj.SerialNo, "UPDATE" }));
                    }
                }

                if (criticalBonepile != null)
                {
                    criticalBonepile.CLOSED_BY = Station.LoginUser.EMP_NO;
                    criticalBonepile.CLOSED_DATE = sysdate;
                    criticalBonepile.CLOSED_REASON = Station.StationName;
                    criticalBonepile.CLOSED_FLAG = "1";
                    criticalBonepile.CURRENT_STATION = snObj.CurrentStation;
                    result = t_r_critical_bonepile.Update(Station.SFCDB, criticalBonepile);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_CRITICAL_BONEPILE:" + snObj.SerialNo, "UPDATE" }));
                    }
                }               
            }
            else
            {
                if (normalBonepile != null)
                {
                    normalBonepile.CURRENT_WO = snObj.WorkorderNo;
                    normalBonepile.CURRENT_STATION = snObj.NextStation;
                    normalBonepile.CURRENT_STATUS = "0";
                    normalBonepile.CLOSED_BY = Station.LoginUser.EMP_NO;
                    normalBonepile.CLOSED_DATE = sysdate;
                    normalBonepile.CLOSED_REASON = Station.StationName;
                    normalBonepile.CLOSED_FLAG = "1";                   
                    result = t_r_normal_bonepile.Update(Station.SFCDB, normalBonepile);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_NORMAL_BONEPILE:" + snObj.SerialNo, "UPDATE" }));
                    }
                }

                if (criticalBonepile != null)
                {
                    criticalBonepile.CLOSED_BY = Station.LoginUser.EMP_NO;
                    criticalBonepile.CLOSED_DATE = sysdate;
                    criticalBonepile.CLOSED_REASON = Station.StationName;
                    criticalBonepile.CLOSED_FLAG = "1";                    
                    result = t_r_critical_bonepile.Update(Station.SFCDB, criticalBonepile);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_CRITICAL_BONEPILE:" + snObj.SerialNo, "UPDATE" }));
                    }
                }
            }
            Station.SFCDB.ORM.Updateable<R_RMA_BONEPILE>()
                        .SetColumns(r => new R_RMA_BONEPILE { CLOSED_FLAG = "1", CLOSED_DATE = sysdate, EDIT_EMP = Station.LoginUser.EMP_NO, EDIT_TIME = sysdate })
                        .Where(r => r.SN == snObj.SerialNo && r.CLOSED_FLAG == "0").ExecuteCommand();
        }

        public static void RMABonepileCloseAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN snObj = (SN)sessionSN.Value;           
            DateTime sysdate = Station.GetDBDateTime();
            Station.SFCDB.ORM.Updateable<R_RMA_BONEPILE>()
                         .SetColumns(r => new R_RMA_BONEPILE { CLOSED_FLAG = "1", CLOSED_DATE = sysdate, EDIT_EMP = Station.LoginUser.EMP_NO, EDIT_TIME = sysdate })
                         .Where(r => r.SN == snObj.SerialNo && r.CLOSED_FLAG == "0").ExecuteCommand();
            List<string> oldSn = new List<string>();
            T_R_REPLACE_SN t_r_replace_sn = new T_R_REPLACE_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            t_r_replace_sn.GetOldSnList(Station.SFCDB, oldSn, snObj.SerialNo);
            if (oldSn.Count > 0)
            {
                Station.SFCDB.ORM.Updateable<R_RMA_BONEPILE>()
                         .SetColumns(r => new R_RMA_BONEPILE { CLOSED_FLAG = "1", CLOSED_DATE = sysdate, EDIT_EMP = Station.LoginUser.EMP_NO, EDIT_TIME = sysdate })
                         .Where(r => oldSn.Contains(r.SN) && r.CLOSED_FLAG == "0").ExecuteCommand();
            }
        }
    }
}
