using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.HWD;
using MESPubLab.MESStation;
using MESPubLab.MESStation.Label;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class SkunoLoader
    {
        public static void SkuDataLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            SKU sku = new SKU();
            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.Init(Input.Value.ToString(), ole, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(sku.SkuNo))
                {
                    throw new Exception($"{Input.Value.ToString()} not exist.");
                }
                s.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "Skuno", sku.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch(Exception ex) 
            {
                throw ex;
                //Station.AddMessage("MES00000052", new string[] { "Skuno"+ ":"+ Input.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "Skuno" + ":" + Input.Value.ToString() }));
            }
        }

        public static void SkuStrFromWOStrDataLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SKUSavePoint位置
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            //加載WoLoadPoint
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                string skuno = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WOSession.Value.ToString()).Select(t => t.SKUNO).First();
                SkuSession.Value = skuno;
                Station.AddMessage("MES00000029", new string[] { "Skuno", skuno }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }
        }

        public static void SmtSkuFromWODataCheck(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SKUSavePoint位置
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            //加載WoLoadPoint
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
                // throw new Exception("WO參數不存在!");
            }

            WorkOrder WOObject = (WorkOrder)WOSession.Value;

            //WOObject.SkuNO

            SKU sku = new SKU();
            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.Init(WOObject.SkuNO, ole, MESDataObject.DB_TYPE_ENUM.Oracle);

                MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                DisplayOutPut CurrentSkuno = Station.DisplayOutput.Find(t => t.Name == "SKUNO");
                //SN機種與當前操作機種不一致卡住
                if (CurrentSkuno.Value != null && CurrentSkuno.Value.ToString().Trim().Length > 0 && !sku.SkuNo.Equals(CurrentSkuno.Value.ToString()))
                {
                    string ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000235", new string[] { SessionSN.Value.ToString(), sku.SkuNo, CurrentSkuno.Value.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }

                SkuSession.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "Skuno", sku.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }
        }

        public static void SkuFromWODataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count < 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SKUSavePoint位置
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            //加載WoLoadPoint
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
                // throw new Exception("WO參數不存在!");
            }

            WorkOrder WOObject = (WorkOrder)WOSession.Value;

            //WOObject.SkuNO

            SKU sku = new SKU();
            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.Init(WOObject.SkuNO, ole, MESDataObject.DB_TYPE_ENUM.Oracle);

                //MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                //DisplayOutPut CurrentSkuno = Station.DisplayOutput.Find(t => t.Name == "SKUNO");
                ////SN機種與當前操作機種不一致卡住
                //if (CurrentSkuno.Value!=null&&CurrentSkuno.Value.ToString().Trim().Length > 0 && !sku.SkuNo.Equals(CurrentSkuno.Value.ToString()))
                //{
                //    string ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000235", new string[] { SessionSN.Value.ToString(), sku.SkuNo, CurrentSkuno.Value.ToString() });
                //    throw new MESReturnMessage(ErrMessage);
                //}

                SkuSession.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "Skuno", sku.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }


        }
        public static void SkuFromDNDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count < 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SKUSavePoint位置
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            //加載WoLoadPoint
            MESStationSession DNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (DNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
                // throw new Exception("WO參數不存在!");
            }
            string dn = Input.Value.ToString();
            try
            {
                var Dn_No = Station.SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == dn).First();
                if (Dn_No == null)
                {
                    // throw new Exception($@"DN不存在,请确认!");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160555", new string[] { "DN:" + Dn_No }));
                }
                SkuSession.Value = Dn_No.SKUNO;
                Station.AddMessage("MES00000029", new string[] { "Skuno", Dn_No.SKUNO.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }


        }
        public static void SkuFromPLDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count < 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SkuSession);
            }
            MESStationSession packSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (packSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            string packno = packSession.Value.ToString();
            try
            {
                var pack = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == packno).First();
                if (pack == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329"));
                }
                SkuSession.Value = pack.SKUNO;
                Station.AddMessage("MES00000029", new string[] { "Skuno", pack.SKUNO.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }
        }

        /// <summary>
        /// 通過WO加載生產製程為RMA、NPI還是正常
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ProductTypeFromWODataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
                // throw new Exception("WO參數不存在!");
            }
            WorkOrder WOObject = (WorkOrder)WOSession.Value;
            string wo = WOObject.WorkorderNo.ToString();
            //加載製程類型session
            MESStationSession PTypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PTypeSession == null)
            {
                PTypeSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(PTypeSession);
                // throw new Exception("WO參數不存在!");
            }
            T_R_WO_TYPE trwt = new T_R_WO_TYPE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            try
            {
                string Ptype = trwt.GetProductTypeByWO_HWT(Station.SFCDB, wo);
                PTypeSession.Value = Ptype;
                Station.AddMessage("MES00000029", new string[] { "ProductType", Ptype.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "ProductType" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }
        }

        public static void SnSKUDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SnPoint位置
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                //modify by 張官軍 2018-03-15
                //根據該方法的定義：從 SN 對象加載機種對象，SnSession 不能為空
                //SnSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                //Station.StationSession.Add(SnSession);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000170"));
            }
            //加載SkunoPoint
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(SkuSession);
                //throw new MESReturnMessage("Skuno參數不存在");
            }

            // BY SDL 20180320  SN SnObject = (SN)SkuSession.Value;.

            SN SnObject = (SN)SnSession.Value;

            //SnObject.SerialNo

            SKU sku = new SKU();

            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.InitBySn(SnObject.SerialNo, ole, MESDataObject.DB_TYPE_ENUM.Oracle);
                SkuSession.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "SN", sku.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch
            {
                //Station.AddMessage("MES00000007", new string[] { "SN" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN" }));

            }


        }

        //通過Sku獲取ModelType
        public static void ModelTypeFromSkuDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string strModelType = "";
            SKU sku;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            //獲取SkuSession
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            else
            {
                if (SkuSession.Value != null)
                {
                    sku = (SKU)SkuSession.Value;
                    if (sku.SkuNo == null || sku.SkuNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }

            }

            //查找ModelTypeSession是否已經存在,存在則抓取內存中的
            MESStationSession ModelTypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (ModelTypeSession == null)
            {
                ModelTypeSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(ModelTypeSession);
            }
            //通過Sku獲取ModelType,并更新到內存中
            T_C_SKU TCSku = new T_C_SKU(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            strModelType = TCSku.GetModelTypeBySku(Station.SFCDB, sku.SkuNo);

            //將ModelType更新到內存中
            ModelTypeSession.Value = strModelType;
        }



        /// <summary>
        /// 通過SKU對象，獲取對應批次對象,如果批次對象為空，新增批次對象
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">2個參數,SKU,LOTNO保存的位置</param>
        public static void GetLotDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //string ColoumName = "skuno";
            Row_R_LOT_STATUS RLotSku;
            T_R_LOT_STATUS TR = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            LotNo LOT = new LotNo();
            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession Ssku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession Slot = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            MESStationSession SLotNewFlag = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);

            if (Ssku == null)
            {
                //throw new Exception("请输入SKU!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            else
            {
                SKU ObjSku = (SKU)Ssku.Value;
                //Marked by LLF 20018-02-22 
                //RLotSku = TR.GetByInput(ObjSku.SkuNo, ColoumName, Station.SFCDB);
                //RLotSku = TR.GetLotBySku(ObjSku.SkuNo, ColoumName, Station.SFCDB);
                //modify by fgg get lot by sku and station name 2018.8.16
                RLotSku = TR.GetLotBySkuAnd(ObjSku.SkuNo, Station.StationName, Station.SFCDB);
            }

            if (Slot == null)
            {
                Slot = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Slot);
            }

            if (SLotNewFlag == null)
            {
                SLotNewFlag = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SLotNewFlag);
            }

            try
            {
                //Modify by LLF 2018-02-07 SLotNewFlag 為新生成Lot的標誌位
                //if (LOT == null)//LOT 為空需產生新的LOTNO
                if (RLotSku == null)
                {
                    SLotNewFlag.Value = "1";
                    Slot.Value = LOT.GetNewLotNo("HWD_FQCLOT", Station.SFCDB);
                }
                else
                {
                    SLotNewFlag.Value = "0";
                    LOT.Init(RLotSku.LOT_NO, "", Station.SFCDB);
                    Slot.Value = LOT;
                    Station.AddMessage("MES00000029", new string[] { "LotNo", LOT.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
                }

            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }
        }

        /// <summary>
        /// 獲取批次抽樣數量
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,SKU 保存的位置</param>
        public static void GetSampleQtyDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Row_C_SKU_SAMPLE RLotSku;
            C_AQLTYPE RAqltype;
            T_C_AQLTYPE QR = new T_C_AQLTYPE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_C_SKU_SAMPLE TR = new T_C_SKU_SAMPLE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            if (Paras.Count != 10)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession SessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            MESStationSession SessionAQLTYPE = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            MESStationSession SessionLotQTY = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            MESStationSession SessionSAMPLEQTY = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            MESStationSession SessionREJECTQTY = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            MESStationSession SessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            MESStationSession SessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            MESStationSession SessionLotNewFlag = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            MESStationSession SessionLotNo = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);

            if (SessionSN == null)
            {
                //throw new Exception("请输入SN!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN ObjSN = (SN)SessionSN.Value;

            //if (SessionSKU == null)
            //{
            //    SessionSKU = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(SessionSKU);
            //}

            if (SessionAQLTYPE == null)
            {
                SessionAQLTYPE = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionAQLTYPE);
            }

            if (SessionLotQTY == null)
            {
                SessionLotQTY = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionLotQTY);
            }

            if (SessionSAMPLEQTY == null)
            {
                SessionSAMPLEQTY = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionSAMPLEQTY);
            }

            if (SessionREJECTQTY == null)
            {
                SessionREJECTQTY = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionREJECTQTY);
            }

            if (SessionPassQty == null)
            {
                SessionPassQty = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionPassQty);
            }

            if (SessionFailQty == null)
            {
                SessionFailQty = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionFailQty);
            }

            if (SessionLotNewFlag == null)
            {
                SessionLotNewFlag = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SessionLotNewFlag);
            }

            if (SessionLotNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[9].SESSION_TYPE + Paras[9].SESSION_KEY }));
            }

            try
            {
                if ((string)SessionLotNewFlag.Value == "1")
                {
                    if (Paras[1].VALUE.ToString() == "ALL")
                    {
                        RLotSku = TR.GetBySkuNo(Paras[1].VALUE.ToString(), ObjSN.NextStation, Station.SFCDB);
                        RAqltype = QR.GetByAqltype(RLotSku.AQL_TYPE, Station.SFCDB);
                        SessionAQLTYPE.Value = RAqltype.AQL_TYPE;
                        //Marked by LLF
                        //SessionLotQTY.Value = RAqltype.LOT_QTY;
                        //SessionSAMPLEQTY.Value = RAqltype.SAMPLE_QTY;
                        //SessionREJECTQTY.Value = RAqltype.REJECT_QTY;
                    }
                    else
                    {
                        RLotSku = TR.GetBySkuNo(ObjSN.SkuNo, ObjSN.NextStation, Station.SFCDB);
                        RAqltype = QR.GetByAqltype(RLotSku.AQL_TYPE, Station.SFCDB);
                        SessionAQLTYPE.Value = RAqltype.AQL_TYPE;
                        //Marked by LLF
                        //SessionLotQTY.Value = RAqltype.LOT_QTY;
                        //SessionSAMPLEQTY.Value = RAqltype.SAMPLE_QTY;
                        //SessionREJECTQTY.Value = RAqltype.REJECT_QTY;
                    }
                    SessionPassQty.Value = 0;
                    SessionFailQty.Value = 0;
                    SessionLotQTY.Value = 0;
                    SessionSAMPLEQTY.Value = 0;
                    SessionREJECTQTY.Value = 0;
                }
                else
                {
                    SessionAQLTYPE.Value = ((LotNo)SessionLotNo.Value).AQL_TYPE;
                    SessionLotQTY.Value = ((LotNo)SessionLotNo.Value).LOT_QTY;
                    SessionSAMPLEQTY.Value = ((LotNo)SessionLotNo.Value).SAMPLE_QTY;
                    SessionREJECTQTY.Value = ((LotNo)SessionLotNo.Value).REJECT_QTY;
                    SessionPassQty.Value = ((LotNo)SessionLotNo.Value).PASS_QTY;
                    SessionFailQty.Value = ((LotNo)SessionLotNo.Value).FAIL_QTY;
                }

                //Station.AddMessage("MES00000104", new string[] { "AQLTYPE", RAqltype.AQL_TYPE, "LotQTY", RAqltype.LOT_QTY.ToString(), "SAMPLEQTY", RAqltype.SAMPLE_QTY.ToString(), "REJECTQTY", RAqltype.REJECT_QTY.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
                Station.AddMessage("MES00000104", new string[] { "AQLTYPE", SessionAQLTYPE.Value.ToString(), "LotQTY", SessionLotQTY.Value.ToString(), "SAMPLEQTY", SessionSAMPLEQTY.Value.ToString(), "REJECTQTY", SessionREJECTQTY.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }
        }

        public static void SampleLotDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string StrSN = "";
            Row_R_LOT_STATUS RLotStatus;
            T_R_LOT_STATUS TR = new T_R_LOT_STATUS(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            LotNo LOT = new LotNo();
            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession LotNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                StrSN = SNSession.Value.ToString();
            }

            if (LotNoSession == null)
            {
                LotNoSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(LotNoSession);
            }
            try
            {
                RLotStatus = TR.GetSampleLotBySN(StrSN, Station.SFCDB);
                if (RLotStatus != null)
                {
                    LotNoSession.Value = RLotStatus.GetDataObject();
                }
                else
                {
                    //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000159", new string[] { }));
                }
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;

            }
        }

        /// <summary>
        /// 根據當前工站生產新的LOTNO
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetLotDataloaderNew(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count <= 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession sessionLotNo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            MESStationSession sessionLotNewFlag = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            LotNo lot = new LotNo();

            string lotno = "";
            if (sessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            if (sessionLotNewFlag == null)
            {
                sessionLotNewFlag = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionLotNewFlag);
            }

            try
            {
                if (sessionLotNo == null)
                {
                    T_R_LOT_STATUS tLotStatus = new T_R_LOT_STATUS(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    Row_R_LOT_STATUS rowLotStatus = tLotStatus.GetLotBySNNotCloesd(sessionSN.Value.ToString(), Station.SFCDB);
                    if (rowLotStatus != null)
                    {
                        sessionLotNewFlag.Value = "0";
                        lot.Init(rowLotStatus.LOT_NO, "", Station.SFCDB);
                        sessionLotNo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                        sessionLotNo.Value = lot;
                        Station.StationSession.Add(sessionLotNo);
                    }
                    else
                    {
                        //新建lot號                   
                        sessionLotNo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                        sessionLotNo.Value = lot.GetNewLotNo("HWD_FQCLOT", Station.SFCDB);
                        sessionLotNewFlag.Value = "1";
                        Station.StationSession.Add(sessionLotNo);
                    }
                }
                else
                {
                    if (sessionLotNo.Value is string && sessionLotNo.Value.ToString() != "")
                    {
                        lotno = sessionLotNo.Value.ToString();
                    }
                    else if (sessionLotNo.Value is LotNo)
                    {
                        lotno = ((LotNo)sessionLotNo.Value).LOT_NO.ToString();
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                    }

                    try
                    {
                        lot.Init(lotno, "", Station.SFCDB);
                    }
                    catch
                    {
                        lot = null;
                    }

                    if (lot != null)
                    {
                        if (lot.CLOSED_FLAG.ToString() == "1")
                        {
                            sessionLotNo.Value = lot.GetNewLotNo("HWD_FQCLOT", Station.SFCDB);
                            sessionLotNewFlag.Value = "1";
                            Station.StationSession.Add(sessionLotNo);
                        }
                        else
                        {
                            sessionLotNewFlag.Value = "0";
                            lot.Init(lot.LOT_NO, "", Station.SFCDB);
                            sessionLotNo.Value = lot;
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
        /// 從包裝單位加載機種對象
        /// PACKNO 1
        /// SKU 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SkuFromPackDataLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            string PackNo = PackNoSession.Value.ToString();

            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(SkuSession);
            }

            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_PACKING Packing = PackingTool.GetPackingByPackNo(PackNo, Station.SFCDB);
            SKU sku = new SKU();
            sku.Init(Packing.SKUNO, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            SkuSession.Value = sku;

        }

        /// <summary>
        /// 根據機種加載流程中的站位
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void StationsFromSkuLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            T_R_SKU_ROUTE TRSR = new T_R_SKU_ROUTE(Station.SFCDB, Station.DBType);
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            SKU Sku = (SKU)SkuSession.Value;
            List<C_ROUTE_DETAIL> RouteDetails = TRSR.GetRouteDetailBySku(Sku.SkuNo, Sku.Version, Station.SFCDB);
            List<string> Stations = RouteDetails.Select(t => t.STATION_NAME).ToList();
            Stations.Insert(0, "");
            Station.FindInputByName(Paras[1].VALUE).DataForUse = Stations.ConvertAll(t => (object)t);
        }

        /// <summary>
        /// 根據機種加載補印的輸入和輸出信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReprintInfosLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            DataTable InputsDT = new DataTable();
            InputsDT.Columns.Add("Name");
            InputsDT.Columns.Add("Type");
            DataTable OutputsDT = new DataTable();
            OutputsDT.Columns.Add("Name");
            OutputsDT.Columns.Add("Description");
            T_C_SKU_Label TCSL = new T_C_SKU_Label(Station.SFCDB, Station.DBType);
            T_C_Label_Type TCLT = new T_C_Label_Type(Station.SFCDB, Station.DBType);
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            SKU Sku = (SKU)SkuSession.Value;

            MESStationSession StationNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StationNameSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }
            string StationName = StationNameSession.Value.ToString();

            C_SKU_Label LabelConfig = TCSL.GetLabelConfigBySkuStation(Sku.SkuNo, StationName, Station.SFCDB).FirstOrDefault();
            if (LabelConfig == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180905093344", new string[] { Sku.SkuNo, StationName }));
            }

            C_Label_Type LabelType = TCLT.GetByName(LabelConfig.LABELTYPE, Station.SFCDB);
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            Assembly assembly = Assembly.LoadFile(path + LabelType.DLL);
            System.Type APIType = assembly.GetType(LabelType.CLASS);
            object API_CLASS = assembly.CreateInstance(LabelType.CLASS);
            LabelBase Base = (LabelBase)API_CLASS;

            foreach (LabelInputValue input in Base.Inputs)
            {
                DataRow dr = InputsDT.NewRow();
                dr["Name"] = input.Name;
                dr["Type"] = input.Type;
                InputsDT.Rows.Add(dr);
            }
            foreach (LabelOutput output in Base.Outputs)
            {
                DataRow dr = OutputsDT.NewRow();
                dr["Name"] = output.Name;
                dr["Description"] = output.Description;
                OutputsDT.Rows.Add(dr);
            }

            MESStationSession InputsSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (InputsSession == null)
            {
                InputsSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(InputsSession);
            }
            InputsSession.Value = InputsDT;



            MESStationSession OutputsSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (OutputsSession == null)
            {
                OutputsSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(OutputsSession);
            }
            OutputsSession.Value = OutputsDT;

            MESStationSession NextInputSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (NextInputSession == null)
            {
                NextInputSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                Station.StationSession.Add(NextInputSession);
            }
            NextInputSession.Value = InputsDT.Rows[0]["Name"];

        }

        public static void MaxVersionSkuLoaderBySN(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            //加載SnPoint位置
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null || SnSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000170"));
            }
            //加載SkunoPoint
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(SkuSession);
            }

            SN SnObject = (SN)SnSession.Value;
            SKU sku = new SKU();
            try
            {
                MESDBHelper.OleExec ole = Station.SFCDB;
                sku = sku.MaxVersionSkuInitBySn(SnObject.SerialNo, ole, MESDataObject.DB_TYPE_ENUM.Oracle);
                SkuSession.Value = sku;
                Station.AddMessage("MES00000029", new string[] { "SN", sku.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN" }));

            }

        }

        public static void LoadLinkQtyBySku(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            SN PanelSn = (SN)PanelSession.Value;

            MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LinkQtySession == null)
            {
                LinkQtySession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(LinkQtySession);
            }

            AP_DLL apDll = new AP_DLL();
            int LinkQty = 0;
            var LinkQtyStr = apDll.AP_GET_LINKQTY(PanelSn.WorkorderNo, Station.APDB);
            if (int.TryParse(LinkQtyStr, out LinkQty))
            {
                LinkQtySession.Value = LinkQty;
            }
            else
            {
                throw new MESReturnMessage(LinkQtyStr);
            }


        }

        /// <summary>
        /// Load Label Name List To Session By SKUNO && LabelType
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SkuLabelListLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession Labels = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Labels == null)
            {
                Labels = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY };
                Station.StationSession.Add(Labels);
            }

            MESStationSession SKU = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SKU == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }
            List<string> labTypes = new List<string>();
            for (int i = 2; i < Paras.Count; i++)
            {
                labTypes.Add(Paras[i].VALUE.ToString());
            }
            var labs = Station.SFCDB.ORM.Queryable<C_SKU_Label>().Where(t => t.SKUNO == SKU.Value.ToString()).ToList();
            if (labTypes.Count > 0)
            {
                Labels.Value = labs.Where(t => labTypes.Contains(t.LABELTYPE)).Select(t => t.LABELNAME).ToList();
            }
            else
            {
                Labels.Value = labs.Select(t => t.LABELNAME).ToList();
            }
        }

        /// <summary>
        /// Get Skuno And Version From WO Object
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void FromWOGetSkunoAndVerLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {  
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (woSession == null || woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession skunoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (skunoSession == null)
            {
                skunoSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(skunoSession);
            }
            MESStationSession skunoVerSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (skunoVerSession == null)
            {
                skunoVerSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(skunoVerSession);
            }
            WorkOrder woObject = (WorkOrder)woSession.Value;
            skunoSession.Value = woObject.SkuNO;
            skunoVerSession.Value = woObject.SKU_VER;
        }

        /// <summary>
        /// TGMES_SHIPOUT從掃描棧板號加載機種對象
        /// </summary>
        public static void TGMESSkuFromPackDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            string PackNo = PackNoSession.Value.ToString();

            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(SkuSession);
            }
            
            R_SN_TGMES_INFO TGMES = Station.SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == PackNo && t.VALID_FLAG == "1").ToList().FirstOrDefault();
            if (TGMES == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { PackNo }));
            }
            SKU sku = new SKU();
            sku.Init(TGMES.ITEM_SALES, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            SkuSession.Value = sku;
        }


        /// <summary>
        /// Loading Sku Obj And Group Id By Wo
        /// </summary>
        public static void LoadingSkuObjAndGroupIdByWo(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (woSession == null && woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            MESStationSession skuObjSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (skuObjSession == null)
            {
                skuObjSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(skuObjSession);
            }
            MESStationSession groupIdSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (groupIdSession == null)
            {
                groupIdSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(groupIdSession);
            }
            string skuno = "";           
            string wo = "";
            if (woSession.Value is WorkOrder)
            {
                skuno = ((WorkOrder)woSession.Value).SkuNO;
                wo = ((WorkOrder)woSession.Value).WorkorderNo;
            }
            else
            {
                var woObj = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == woSession.Value.ToString()).ToList().FirstOrDefault();
                if (woObj == null)
                {
                    throw new Exception($"{woSession.Value.ToString()} not exist.");
                }
                skuno = woObj.SKUNO;
                wo = woObj.WORKORDERNO;
            }
            
            SKU sku = new SKU();
            try
            {
                sku = sku.Init(skuno, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                skuObjSession.Value = sku;

                var preWoHead = Station.SFCDB.ORM.Queryable<MESDataObject.Module.Juniper.R_PRE_WO_HEAD>().Where(r => r.WO == wo).ToList().FirstOrDefault();
                groupIdSession.Value = preWoHead == null ? skuno : preWoHead.GROUPID;
                Station.AddMessage("MES00000029", new string[] { "Skuno", sku.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception e)
            {
                Station.AddMessage("MES00000007", new string[] { "Skuno" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                throw e;
            }
        }

        public static void LoadingLabelInfoToInput(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSku == null || sessionSku.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SKU skuObj = (SKU)sessionSku.Value;
            string inputName = Paras[1].VALUE;
            MESStationInput stationInput = Station.Inputs.Find(t => t.DisplayName == inputName);
            if(stationInput==null)
            {
                throw new MESReturnMessage($@"No Station Input [{inputName}]");
            }

            T_C_SKU_Label t_c_sku_label = new T_C_SKU_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_SKU_Label> labelList = t_c_sku_label.GetLabelConfigBySkuStation(skuObj.SkuNo, Station.StationName.ToString(), Station.SFCDB);
            T_R_Label t_r_label = new T_R_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);            
            if (labelList.Count > 0)
            {
                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                foreach (var label in labelList)
                {
                    var labelObj= t_r_label.GetLabelByLabelName(label.LABELNAME, Station.SFCDB);
                    if(labelObj==null)
                    {
                        throw new MESReturnMessage($@"Label [{label.LABELNAME}] not exist.");
                    }
                    stationInput.DataForUse.Add($@"{label.LABELTYPE},{labelObj.LABELNAME},{labelObj.R_FILE_NAME},{label.QTY}");
                }                
            }
            else
            {
                throw new MESReturnMessage($@"{skuObj.SkuNo},{Station.StationName},No Label");
            }
        }
    }
}
