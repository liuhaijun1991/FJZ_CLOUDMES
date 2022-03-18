using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using System.Data;
using MESPubLab.Json;
using MESStation.Config.DCN;

namespace MESStation.Stations.StationActions.DataLoaders
{
    class PackLoader
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputPackNoDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            
            if (PackNoSession != null)
                Station.StationSession.Remove(PackNoSession);
            else
            {
                PackNoSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                PackNoSession.InputValue = Input.Value.ToString();
                Station.StationSession.Add(PackNoSession);
            }

        }


        /// <summary>
        /// 根據輸入加載出 Packing 對象
        /// PRINT 1
        /// PACKNO 1
        /// SKU 1
        /// 
        /// 
        /// PACKNO 1
        /// PACKING
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackNoDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            string PackNo = PackNoSession.Value.ToString();

            LogicObject.Packing packing = new LogicObject.Packing();
            packing.DataLoad(PackNo, Station.BU, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession PackingSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackingSession == null)
            {
                PackingSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY,Value= packing };
                Station.StationSession.Add(PackingSession);
            }
            else
            {
                PackingSession.Value = packing;
            }

        }
        /// <summary>
        /// 為重新打印棧板和Carton數據準備
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReprintPackNoDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            T_R_PACKING Packing = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            

            //是否打印
            MESStationSession PrintSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PrintSession == null)
            {
                PrintSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = Paras[0].VALUE };
                Station.StationSession.Add(PrintSession);
            }
            else
            {
                PrintSession.Value = Paras[0].VALUE;
            }

            //pack_no
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }
            string PackNo = PackNoSession.Value.ToString();
            R_PACKING P = Packing.GetPackingByPackNo(PackNo, Station.SFCDB);
            if (P == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181006201845"));
            }

            SKU sku = new SKU();
            sku.Init(P.SKUNO, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = sku };
                Station.StationSession.Add(SkuSession);
            }
            else
            {
                SkuSession.Value = sku;
            }

        }

        /// <summary>
        /// 移棧板或卡通，從輸入加載包裝信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CartonOrPalletDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 6)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }           
            MESStationSession sessionLocation = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);           

            if (sessionLocation == null)               
            {
                sessionLocation = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE,SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };  
                Station.StationSession.Add(sessionLocation);
            }

            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSku == null)
            {
                sessionSku = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE,SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };               
                Station.StationSession.Add(sessionSku);
            }

            MESStationSession sessionVer = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionVer == null)
            {
                sessionVer = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };        
                Station.StationSession.Add(sessionVer);
            }

            MESStationSession sessionType = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionType == null)
            {
                sessionType = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionType);
            }

            MESStationSession sessionCount = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionCount == null)
            {
                sessionCount = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionCount);
            }

            MESStationSession sessionListItem = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionListItem == null)
            {
                sessionListItem = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionListItem);
            }

            try
            {
                string inputValue = Input.Value.ToString();               
                if (string.IsNullOrEmpty(inputValue))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { inputValue }));
                }
                LogicObject.Packing packObject = new LogicObject.Packing();
                packObject.DataLoad(inputValue,Station.BU, Station.SFCDB, Station.DBType);
                sessionLocation.Value = packObject;
                sessionSku.Value = packObject.Skuno;
                sessionType.Value = packObject.PackType;
                sessionVer.Value = packObject.SkunoVer;
                sessionCount.Value = packObject.PackList == null ? 0 : packObject.PackList.Count;
                sessionListItem.Value = packObject.PackList;
                Station.AddMessage("MES00000029", new string[] { "Location", inputValue }, StationMessageState.Pass);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 在包裝站位，根據 SN 對象以及 管控名 加載需要檢查的項目（例如 機種，製造國家，Deviation）
        /// SN 1
        /// CONTROLNAME 配置 Value 部分即可
        /// ITEMS 1
        /// CURRENTITEM 1
        /// 將加載到的數據填入到 ITEMS 1 和 CURRENTITEM 1 中，分別表示所有的檢查項和當前檢查項
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackingScanItemLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            T_C_CONTROL TCC = new T_C_CONTROL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_WO_DEVIATION TRWD = new T_R_WO_DEVIATION(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            string ControlName = Paras[1].VALUE.ToString();

            SN Sn = (SN)SnSession.Value;
            R_WO_DEVIATION Deviation = TRWD.GetDeviationByWo(Sn.WorkorderNo, Station.SFCDB);
            C_CONTROL Control = Station.SFCDB.ORM.Queryable<C_CONTROL>()
                .Where(t => t.CONTROL_TYPE.Contains(Sn.WorkorderNo) && ControlName.Contains(t.CONTROL_NAME))
                .ToList().FirstOrDefault();
            if (Control == null)
            {
                Control = Station.SFCDB.ORM.Queryable<C_CONTROL>()
                    .Where(t => t.CONTROL_TYPE.Contains(Sn.SkuNo) && ControlName.Contains(t.CONTROL_NAME))
                    .ToList().FirstOrDefault();
                if (Control == null)
                {

                    Control = Station.SFCDB.ORM.Queryable<C_CONTROL>()
                        .Where(t => t.CONTROL_TYPE.ToUpper().Equals("ALL") && ControlName.Contains(t.CONTROL_NAME))
                        .ToList().FirstOrDefault();
                    if (Control == null)
                    {
                        return;
                    }
                }
            }
            
            System.Data.DataTable ItemsTable = new System.Data.DataTable();
            ItemsTable.Columns.Add("Index");
            ItemsTable.Columns.Add("Item");
            
            string[] ControlValue = Control.CONTROL_VALUE.ToUpper().Split(',');
            for (int i = 1; i <= ControlValue.Length; i++)
            {
                if (ControlValue[i - 1].ToUpper().Equals("DEVIATION") && Deviation == null)
                {
                    continue;
                }
                System.Data.DataRow dr = ItemsTable.NewRow();
                dr["Index"] = i;
                dr["Item"] = ControlValue[i - 1];
                ItemsTable.Rows.Add(dr);
            }

            MESStationSession ItemsSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ItemsSession == null)
            {
                ItemsSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(ItemsSession);
            }
            ItemsSession.Value = ItemsTable;

            MESStationSession CurrentItemSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (CurrentItemSession == null)
            {
                CurrentItemSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(CurrentItemSession);
            }
            CurrentItemSession.Value = ControlValue.Length>0?ControlValue[0]:"";



        }

        public static void EmptyPackLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession ExistLocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (ExistLocationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052",new string[] { Paras[0].SESSION_TYPE+Paras[0].SESSION_KEY}));
            }

            MESStationSession NewLocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NewLocationSession == null)
            {
                NewLocationSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(NewLocationSession);
            }

            R_PACKING ExistPacking = TRP.GetPackingByPackNo(ExistLocationSession.Value.ToString(), Station.SFCDB);
            if (ExistPacking != null)
            {
                C_PACKING PackingConfig = TCP.GetPackingBySkuAndType(ExistPacking.SKUNO, ExistPacking.PACK_TYPE, Station.SFCDB);
                if (PackingConfig != null)
                {
                    string EmptyPackNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(PackingConfig.SN_RULE, Station.DBS["SFCDB"]);
                    TRP.InsertPacking(Station.BU, Station.Line, Station.StationName, Station.IP, Station.LoginUser.EMP_NO, EmptyPackNo, ExistPacking.PACK_TYPE,ExistPacking.PARENT_PACK_ID, ExistPacking.SKUNO, PackingConfig.MAX_QTY,0,"0", Station.SFCDB);
                    NewLocationSession.Value = EmptyPackNo;
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103151156"));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613093329", new string[] { ExistLocationSession.Value.ToString() }));
            }


        }

        /// <summary>
        /// 根據包裝號查找對應的機種，工單，數量信息
        /// PACKNO 1
        /// SKUNO   1
        /// WO  1
        /// QTY 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackInfoLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string PackNo = PackNoSession.Value.ToString();

            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                SkuSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(SkuSession);
            }

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (WoSession == null)
            {
                WoSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(WoSession);
            }

            MESStationSession QtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (QtySession == null)
            {
                QtySession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(QtySession);
            }

            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
            R_PACKING Packing = TRP.GetPackingByPackNo(PackNo, Station.SFCDB);
            SkuSession.Value = Packing.SKUNO;

            List<R_SN> SnList = new List<R_SN>();
            TRP.GetSnListByPackNo(PackNo, ref SnList, Station.SFCDB);
            QtySession.Value = SnList.Count.ToString();
            List<string> WorkOrders = SnList.Select(t => t.WORKORDERNO).Distinct().ToList();
            WoSession.Value = string.Join(",", WorkOrders.ToArray());
        }

        /// <summary>
        /// 將包裝號從原父級包裝分離處理，成為另外一個新生成父級包裝號的子包裝
        /// 目前只在兼容以 Carton 方式出貨的情況下，其實最後還是以 Pallet 出貨
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void MovePackToAnotherParentLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            string PackNo = PackNoSession.Value.ToString();
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
            T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, Station.DBType);
            
            R_PACKING Packing = TRP.GetPackingByPackNo(PackNo, Station.SFCDB);
            if (Packing.PARENT_PACK_ID != null)
            {
                R_PACKING ParentPacking = TRP.GetPackingByID(Packing.PARENT_PACK_ID, Station.SFCDB);
                C_PACKING PackingConfig = TCP.GetPackingBySkuAndType(ParentPacking.SKUNO, ParentPacking.PACK_TYPE, Station.SFCDB);
                string ParentPackNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(PackingConfig.SN_RULE, Station.DBS["SFCDB"]);
                OleExec DB = Station.SFCDB;
                R_PACKING NewParentPacking = TRP.InsertPacking(Station.BU, Station.Line, Station.StationName, Station.IP,Station.LoginUser.EMP_NO, ParentPackNo, ParentPacking.PACK_TYPE, "", ParentPacking.SKUNO, ParentPacking.MAX_QTY, 1,"1", Station.SFCDB);

                //更新包裝號的父級ID
                Packing.PARENT_PACK_ID = NewParentPacking.ID;
                Packing.EDIT_TIME = DateTime.Now;
                Packing.EDIT_EMP = Station.LoginUser.EMP_NO;
                TRP.UpdatePacking(Packing, DB);
                //更新包裝號的原父級數量
                ParentPacking.QTY = ParentPacking.QTY - 1;
                ParentPacking.EDIT_TIME = DateTime.Now;
                ParentPacking.EDIT_EMP = Station.LoginUser.EMP_NO;
                TRP.UpdatePacking(ParentPacking, DB);

                //更新新父級的 CloseFlag
                //NewParentPacking.CLOSED_FLAG = "1";
                //TRP.UpdatePacking(NewParentPacking, Station.SFCDB);
                //將新父級的包裝號作為 PackNoSession 的值使用在後面
                PackNoSession.Value = ParentPackNo;

                
            }


        }
        /// <summary>
        /// HWT OBA 掃入棧板加載信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OBAScanPalletNOLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 9)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionNewLot = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionNewLot == null || sessionNewLot.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPalletNo == null || sessionPalletNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionPalletList = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPalletList == null || sessionPalletList.Value == null)
            {                
                sessionPalletList = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionPalletList);
            }

            MESStationSession sessionSkuno = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionSkuno == null )
            {
                sessionSkuno = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = "", SessionKey = Paras[3].SESSION_KEY, ResetInput = null };                
                Station.StationSession.Add(sessionSkuno);
            }

            MESStationSession sessionTotalQty = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionTotalQty == null)
            {
                sessionTotalQty = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = "", SessionKey = Paras[4].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionTotalQty);
            }

            MESStationSession sessionSMPQty = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionSMPQty == null)
            {
                sessionSMPQty = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = "", SessionKey = Paras[5].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionSMPQty);
            }
            MESStationSession sessionType = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (sessionType == null)
            {
                sessionType = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = "", SessionKey = Paras[6].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionType);
            }
            MESStationSession sessionSampingByTimeList = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            if (sessionSampingByTimeList == null)
            {
                sessionSampingByTimeList = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = "", SessionKey = Paras[7].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionSampingByTimeList);
            }

            MESStationSession sessionIsEnd = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            if (sessionIsEnd == null)
            {
                sessionIsEnd = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = "", SessionKey = Paras[8].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionIsEnd);
            }

            string pallet = sessionPalletNo.Value.ToString();
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            bool IsEnd = false;
     
           
            if (pallet.StartsWith("PLN"))
            {
                MESDataObject.Module.HWT.T_R_OBA_TEMP TROT = new MESDataObject.Module.HWT.T_R_OBA_TEMP(sfcdb, dbtype);
                T_R_SN_PACKING TRAP = new T_R_SN_PACKING(sfcdb, dbtype);
                T_R_PACKING TRP = new T_R_PACKING(sfcdb, dbtype);
                R_PACKING objPacking = TRP.GetBYPACKNO(pallet, sfcdb);
                if (objPacking == null)
                {
                    throw new Exception(pallet + " The palletno is invalid!");
                }
                if (objPacking.CLOSED_FLAG == "0")
                {
                    throw new Exception(pallet + " The palletno is not closed");
                }               
                if(!TRAP.CheckPackSnStatus(sfcdb,"OBA", objPacking.PACK_NO))
                {
                    throw new Exception(" The nextstation of " + pallet + " is not same!");
                }

                List<R_SN> listSN = TRP.GetPakcingSNList(objPacking.ID, objPacking.PACK_TYPE, sfcdb);
                int shippedCount = listSN.Where(r => r.SHIPPED_FLAG == "1").ToList().Count;
                if (shippedCount > 0)
                {
                    throw new Exception(shippedCount + " SN  of " + pallet + " is shipped!");
                }

                //T_R_LOT_PACK TRLP = new T_R_LOT_PACK(sfcdb, dbtype);
                //if (TRLP.PackNoIsOnOBASampling(objPacking.PACK_NO, sfcdb))
                //{
                //    throw new Exception(pallet + " The palletno is in lot");
                //}               

                T_C_SKU TCS = new T_C_SKU(sfcdb, dbtype);
                C_SKU objSku = TCS.GetSku(objPacking.SKUNO, sfcdb);  

                T_C_PACKING TCP = new T_C_PACKING(sfcdb, dbtype);
                C_PACKING objCP = TCP.GetPackingBySkuAndType(objSku.SKUNO, "CARTON", sfcdb);
                if (objCP == null)
                {
                    throw new Exception(objSku.SKUNO + " The sku can not setting the carton packing num!");
                }
                if (TROT.IsExistByTypeAndValue(sfcdb, "PALLET", objPacking.PACK_NO))
                {
                    //throw new Exception(pallet + " 已經掃描，請不要重複掃描");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616103112", new string[] { pallet }));
                }

                if (sessionNewLot.Value.ToString() == "YES")
                {
                    sessionTotalQty.Value = listSN.Count;
                    sessionSkuno.Value = objSku.SKUNO;                    
                    sessionNewLot.Value = "NO";
                    Station.StationMessages.Add(new StationMessage { Message = "Start New Lot", State = StationMessageState.Pass });
                    Station.StationMessages.Add(new StationMessage { Message = "Begin Scan Palletno Into Lot", State = StationMessageState.Pass });
                }
                else
                {
                    if (!objPacking.SKUNO.Equals(sessionSkuno.Value))
                    {
                        throw new Exception("THE PALLET SKUNO IS DIFFERENT FROM THE LOT SKUNO");
                    }
                    sessionTotalQty.Value = Convert.ToInt32(sessionTotalQty.Value) + listSN.Count;
                }


                LotNo lot = new LotNo();
                sessionType.Value = lot.GetOBATypeBySkuno(sfcdb, objSku.SKUNO, objSku.VERSION);

                //把棧板寫入臨時表 
                MESDataObject.Module.HWT.R_OBA_TEMP tempObj = new MESDataObject.Module.HWT.R_OBA_TEMP();
                tempObj.ID = TROT.GetNewID(Station.BU, sfcdb);
                tempObj.TYPE = "PALLET";
                tempObj.VALUE = objPacking.PACK_NO;
                tempObj.QTY = listSN.Count;
                tempObj.STATUS = "PASS";
                tempObj.IP = Station.IP;
                tempObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                tempObj.EDIT_TIME = Station.GetDBDateTime();
                TROT.SaveNewRecord(sfcdb, tempObj);
                DataTable dt = TROT.GetTableByTypeAndIP(sfcdb, "PALLET", Station.IP);
                sessionPalletList.Value = dt;
                // 若1CARTON裝1PCS的最多可輸入20個PALLETLIST,其它的包規最多可輸入10個PALLETLIST
                if (objCP.MAX_QTY == 1 && dt.Rows.Count == 20)
                {
                    IsEnd = true;
                }
                else if (objCP.MAX_QTY > 1 && dt.Rows.Count == 10)
                {
                    IsEnd = true;
                }

            }
            else if (pallet == "END")
            {               
                IsEnd = true;                
            }
            else
            {
                throw new Exception("Please input rigth palletno");
            }

            double? IsAll;
            double? samplingQty = 0;
            double totalQty = Convert.ToDouble(sessionTotalQty.Value);
            string sampingType = sessionType.Value.ToString();
            string skuno = sessionSkuno.Value.ToString();

            MESDataObject.Module.HWT.T_R_OBASAMPLING_BYTIME TROB = new MESDataObject.Module.HWT.T_R_OBASAMPLING_BYTIME(sfcdb, dbtype);
            List<MESDataObject.Module.HWT.R_OBASAMPLING_BYTIME> listByTime = TROB.GetSamplingList(sfcdb, skuno, "", "OBA", "");
            sessionSampingByTimeList.Value = listByTime.OrderByDescending(r => r.LASTEDITTIME);
            sessionIsEnd.Value = "NO";
            
            if (IsEnd)
            {
                MESDataObject.Module.HWT.T_C_OBA_SAMPLING TCOS = new MESDataObject.Module.HWT.T_C_OBA_SAMPLING(sfcdb, dbtype);
                MESDataObject.Module.HWT.C_OBA_SAMPLING objSampling = TCOS.GetSamplingObj(sfcdb, sampingType, skuno, totalQty);

                if (objSampling == null)
                {
                    T_C_SKU_DETAIL TCSD = new T_C_SKU_DETAIL(sfcdb, dbtype);
                    C_SKU_DETAIL objDetail = TCSD.GetDetailBySkuAndCategory(sfcdb, skuno, "SAMPELING");
                    if (objDetail != null)
                    {
                        skuno = objDetail.VALUE;
                    }
                    else
                    {
                        skuno = "ANYTHING";
                    }
                    objSampling = TCOS.GetSamplingObj(sfcdb, sampingType, skuno, Convert.ToDouble(totalQty));
                    if (objSampling == null)
                    {
                        samplingQty = totalQty / 10;
                        IsAll = 0;
                    }
                    else
                    {
                        IsAll = objSampling.ISALL;
                        samplingQty = objSampling.SAMPLINGQTY;
                    }
                }
                else
                {
                    IsAll = objSampling.ISALL;
                    samplingQty = objSampling.SAMPLINGQTY;
                }
                if (IsAll == 1)
                {
                    sessionSMPQty.Value = totalQty;
                }
                else
                {
                    sessionSMPQty.Value = samplingQty;
                }
                Station.StationMessages.Add(new StationMessage { Message = "End Scan Pallet List", State = StationMessageState.Pass });
                Station.StationMessages.Add(new StationMessage { Message = "Begin Scan Sampling", State = StationMessageState.Pass });  
                sessionIsEnd.Value = "YES";
            }            
        }
        /// <summary>
        /// 加載包裝的skuno,qty等信息到session中
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PalletInofLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPack = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPack == null || sessionPack.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            LogicObject.Packing objPacking = (LogicObject.Packing)sessionPack.Value;
            if (objPacking != null)
            {
                MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == "SKUNO");
                if (sessionSku == null)
                {
                    sessionSku = new MESStationSession() { MESDataType = "SKUNO", InputValue = "", SessionKey ="1", ResetInput = null };
                    Station.StationSession.Add(sessionSku);
                }
                sessionSku.Value = objPacking.Skuno;

                MESStationSession sessionPackingQty = Station.StationSession.Find(t => t.MESDataType == "PackingQty");
                if (sessionPackingQty == null)
                {
                    sessionPackingQty = new MESStationSession() { MESDataType = "PackingQty", InputValue = "", SessionKey = "1", ResetInput = null };
                    Station.StationSession.Add(sessionPackingQty);
                }
                sessionPackingQty.Value = objPacking.SNList.Count;

                MESStationSession sessionSkuVersion = Station.StationSession.Find(t => t.MESDataType == "SkuVersion");
                if (sessionSkuVersion == null)
                {
                    sessionSkuVersion = new MESStationSession() { MESDataType = "SkuVersion", InputValue = "", SessionKey = "1", ResetInput = null };
                    Station.StationSession.Add(sessionSkuVersion);
                }
                sessionSkuVersion.Value = objPacking.SkunoVer;
            }
        }
        /// <summary>
        /// Pack Object Data Loader By input
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackObjectDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackNo == null || sessionPackNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackObject == null)
            {
                sessionPackObject = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = null, InputValue = null, ResetInput = null };
                Station.StationSession.Add(sessionPackObject);
            }

            LogicObject.Packing packingObject = new LogicObject.Packing();
            packingObject.DataLoad(sessionPackNo.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            sessionPackObject.Value = packingObject;
        }
        /// <summary>
        /// 如果輸入的不是棧板號則根據輸入的SN 或CARTON 生成新的棧板，然後再以新生成的棧板出貨
        /// HWT Shipping 工站可以掃描SN CARTON 出貨，如果掃描是SN則只出則1PCS,且只有單包的SN可以這樣做,如果是CARTON則只出這1CARTON
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetNewPalletByInputDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionInputValue = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionInputValue == null || sessionInputValue.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionSkuno = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSkuno == null || sessionSkuno.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionPalletNo = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPalletNo == null)
            {
                sessionPalletNo = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = null, InputValue = null, ResetInput = null };
                Station.StationSession.Add(sessionPalletNo);
            }

            string inputValue = sessionInputValue.Value.ToString();
            string skuno = sessionSkuno.Value.ToString();
            string result = "";
            string palletNo = "";
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            T_R_PACKING TRP = new T_R_PACKING(sfcdb, dbtype);
            R_PACKING packObject = TRP.GetPackingByPackNo(inputValue, sfcdb);

            R_PACKING newPalletObject = null;
            R_PACKING oldPalletObject = null;
            R_PACKING cartonObject = null;

            if (packObject != null)
            {
                if (packObject.PACK_TYPE.ToUpper() == "PALLET")
                {
                    //1.輸入的是棧板，則直接返回棧板號
                    palletNo = packObject.PACK_NO;
                }
                else if (packObject.PACK_TYPE.ToUpper() == "CARTON")
                {
                    //2.如果輸入的是CARTON，則自動生成新的的棧板號，并把這個CARTON分到新的棧板上，再以新的棧板出貨 
                    newPalletObject = LogicObject.Packing.GetNewNullPallet(sfcdb, dbtype, skuno, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO);
                    oldPalletObject = TRP.GetPackingByID(packObject.PARENT_PACK_ID, sfcdb);
                    LogicObject.Packing.MoveCartonToNewPallet(sfcdb, dbtype, packObject, oldPalletObject, newPalletObject, Station.BU, Station.LoginUser.EMP_NO);
                    result = TRP.UpdateCloseFlagByPackID(newPalletObject.ID, "1", Station.SFCDB);
                    if (Convert.ToInt32(result) == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612154506", new string[] { newPalletObject.PACK_NO }));
                    }
                    palletNo = newPalletObject.PACK_NO;
                }
                else
                {
                    // throw new MESReturnMessage("包裝類型錯誤！");

                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814165650"));

                }                
            }
            else
            {
                //3.如果輸入的是SN，則自動生成信息的CARTON號，棧板號，并把SN分到新的CARTON，新的棧板上，再以新棧板出貨
                T_R_SN TRS = new T_R_SN(sfcdb, dbtype);
                R_SN snObject = TRS.LoadData(inputValue, sfcdb);
                if (snObject != null)
                {
                    cartonObject = TRP.GetPackingObjectBySN(snObject.SN, sfcdb);
                    if (cartonObject == null)
                    {
                        //throw new MESReturnMessage(inputValue + "還沒有過包裝過站");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814175622", new string[] { inputValue }));
                    }                    
                    if (cartonObject.MAX_QTY > 1)
                    {
                        // throw new MESReturnMessage("只有單包的SN才能通過直接移棧板的方式出貨！");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814180112"));
                    }
                    newPalletObject = LogicObject.Packing.GetNewNullPallet(sfcdb, dbtype, skuno, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO);
                    oldPalletObject = TRP.GetPackingByID(cartonObject.PARENT_PACK_ID, sfcdb);
                    LogicObject.Packing.MoveCartonToNewPallet(sfcdb, dbtype, cartonObject, oldPalletObject, newPalletObject, Station.BU, Station.LoginUser.EMP_NO);
                    if (Convert.ToInt32(result) == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612154506", new string[] { newPalletObject.PACK_NO }));
                    }
                    result = TRP.UpdateCloseFlagByPackID(newPalletObject.ID, "1", Station.SFCDB);
                    palletNo = newPalletObject.PACK_NO;
                }
            }
            if (!string.IsNullOrEmpty(palletNo))
            {
                sessionPalletNo.Value = palletNo;
            }
            else
            {
                sessionPalletNo.Value = inputValue;
            }
        }

        public static void PackObjectInfoShowLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count !=4 )
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionPackSku = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackSku == null)
            {
                sessionPackSku = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = null, InputValue = null, ResetInput = null };
                Station.StationSession.Add(sessionPackSku);
            }
            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null)
            {
                sessionPallet = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = null, InputValue = null, ResetInput = null };
                Station.StationSession.Add(sessionPallet);
            }
            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionCarton == null)
            {
                sessionCarton = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = null, InputValue = null, ResetInput = null };
                Station.StationSession.Add(sessionCarton);
            }
            LogicObject.Packing packingObject = (LogicObject.Packing)sessionPackObject.Value;
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
            sessionPackSku.Value = packingObject.Skuno;
            if (packingObject.PackType == "PALLET")
            {
                sessionPallet.Value = packingObject.PackNo ;
                sessionCarton.Value = packingObject.CartonList.Count;
            }
            else
            {
                R_PACKING objPallet = TRP.GetPackingByID(packingObject.ParentPackID, Station.SFCDB);
                sessionPallet.Value = objPallet.PACK_NO;
                sessionCarton.Value = TRP.GetChildPacks(objPallet.PACK_NO, Station.SFCDB).Count;
            }
        }


        /// <summary>
        /// 加載用於OverPack的DN列表數據到Session
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OverPackStationDNDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionDNList = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDNList == null || sessionDNList.Value == null)
            {
                sessionDNList = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY };
                Station.StationSession.Add(sessionDNList);
            }
            var data = Station.SFCDB.ORM.Queryable<R_DN_STATUS>()
                .Where(t => t.DN_FLAG == "0" && t.GT_FLAG == "0" && t.DN_NO == SqlSugar.SqlFunc.Subqueryable<R_DN_CUST_PO>().Select(t2 => t2.DN_NO)).Select(t => t.DN_NO).ToList();

            sessionDNList.Value = data;
        }

        /// <summary>
        /// 加載DN對應的PackNo列表數據到Session
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OverPackListDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionDN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDN == null || sessionDN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionPackList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackList == null || sessionPackList.Value == null)
            {
                sessionPackList = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(sessionPackList);
            }
            var DN = sessionDN.Value.ToString();
            var data = Station.SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(t => t.DN_NO == DN).OrderBy(t => t.PACK_NO, SqlSugar.OrderByType.Asc).ToList();
            sessionPackList.Value = data;
        }



        /// <summary>
        /// 加載SN對應的PackNo數據到Session
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OverPackNoLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionPackNo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackNo == null)
            {
                sessionPackNo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(sessionPackNo);
            }
            MESStationSession sessionDn = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionDn == null)
            {
                sessionDn = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionDn);
            }
            var SN = sessionSN.Value.ToString();
            var data = Station.SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(t => t.SN == SN).OrderBy(t => t.PACK_NO, SqlSugar.OrderByType.Asc).ToList();
            sessionPackNo.Value = data[0].PACK_NO;
            sessionDn.Value = data[0].DN_NO;
        }
        public static void GetCartonPalletLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSkuno = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSkuno == null || sessionSkuno.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCarton == null)
            {
                sessionCarton = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(sessionCarton);
            }
            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null)
            {
                sessionPallet = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionPallet);
            }

            var SKUNO = sessionSkuno.Value.ToString();
            var data = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_TYPE== "CARTON"&&t.CLOSED_FLAG=="0"&&t.IP==Station.IP&&t.STATION==Station.StationName && t.SKUNO == SKUNO).Select(t => t.PACK_NO).First();
            sessionCarton.Value = data;

            var data1 = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_TYPE == "PALLET" && t.CLOSED_FLAG == "0" && t.IP == Station.IP && t.STATION == Station.StationName && t.SKUNO == SKUNO).Select(t => t.PACK_NO).First();
            sessionPallet.Value = data1;
        }
       

        public static void NewOverPackNoLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionDN
                = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDN == null || sessionDN.Value == null)
            {
                throw new Exception("Pls Input DN First");
            }
            MESStationSession sessionPackNo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPackNo == null )
            {
                sessionPackNo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(sessionPackNo);
            }
            MESStationSession sessionSkuno = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSkuno == null )
            {
                sessionSkuno = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionSkuno);
            }

            var strDN = sessionDN.Value.ToString();
            var SFCDB = Station.SFCDB;
            var DNitem = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == strDN).First();
            sessionSkuno.Value = DNitem.SKUNO;

            var config = JsonSave.GetFromDB<List<OverPackConfig>>("OVERPACKCONFIG", "SKUCONFIG", SFCDB);
            var packConfig = config.Find(t => t.Skuno == DNitem.SKUNO);
            if (packConfig == null)
            {
                throw new Exception($@"SKU:{DNitem.SKUNO} no set OVERPACKCONFIG");
            }
            var packCount = (int)(DNitem.QTY / packConfig.PackQTY);
            if ((int)DNitem.QTY % (int)packConfig.PackQTY != 0)
            {
                packCount++;
            }
            var packitems = SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(t => t.DN_NO == strDN).ToList();

            if (packitems.Count >= DNitem.QTY)
            {
                throw new Exception($@"DN QTY:{DNitem.QTY} <= Pack QTY:{packitems.Count}");
            }

            for (int i = 1; i <= packCount; i++)
            {
                var sns = packitems.FindAll(t => t.PACK_NO == i);
                if (sns.Count < packConfig.PackQTY)
                {
                    sessionPackNo.Value = i;
                    return;
                }
            }
        }

    }
}
