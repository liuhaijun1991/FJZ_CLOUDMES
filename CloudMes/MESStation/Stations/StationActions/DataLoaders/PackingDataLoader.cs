using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESStation.LogicObject;
using System.Text.RegularExpressions;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class PackingDataLoader
    {
        /// <summary>
        /// 為重打印棧板而完成打印設置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadPalletFromInputForPrint(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            Packing.PalletBase pallet = new Packing.PalletBase(Input.Value.ToString(), Station.SFCDB);
            MESStationSession sessionPrintPL = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_PL" && t.SessionKey == "1");
            if (sessionPrintPL == null)
            {
                sessionPrintPL = new MESStationSession() { MESDataType = "ISPRINT_PL", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintPL);
            }
            sessionPrintPL.Value = "TRUE";

            MESStationSession PlPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_PL" && T.SessionKey == "1");
            if (PlPrintSession == null)
            {
                PlPrintSession = new MESStationSession() { MESDataType = "PRINT_PL", SessionKey = "1" };
                Station.StationSession.Add(PlPrintSession);
            }
            PlPrintSession.Value = pallet.DATA.PACK_NO;
            LogicObject.SKU sku = new LogicObject.SKU();
            sku.Init(pallet.DATA.SKUNO, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession SKUSession = Station.StationSession.Find(T => T.MESDataType == "SKU" && T.SessionKey == "1");
            if (SKUSession == null)
            {
                SKUSession = new MESStationSession() { MESDataType = "SKU", SessionKey = "1" };
                Station.StationSession.Add(SKUSession);
            }
            SKUSession.Value = sku;


        }

        /// <summary>
        /// 根據 SN 對象加載 Carton 信息
        /// 
        /// SN 1
        /// CARTON 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CartonDataLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_SN_RULE_DETAIL RuleTool = new T_C_SN_RULE_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_PACKING PackingConfigTool = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            MESStationSession CartonSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (CartonSession == null)
            {
                CartonSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(CartonSession);
            }
            SN sn = (SN)SnSession.Value;

            R_PACKING packing = PackingTool.GetExistPacking(sn.SkuNo, Station.StationName, Station.StationName, Station.Line, Station.IP, Station.SFCDB);
            if (packing != null)
            {
                CartonSession.Value = packing.PACK_NO;
            }
            else
            {
                C_PACKING PackingConfig = PackingConfigTool.GetPackingBySkuAndType(sn.SkuNo, Station.StationName, Station.SFCDB);
                if (PackingConfig == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181103151156"));
                }
                //獲取一個新的 Carton 號
                string PackNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(PackingConfig.SN_RULE, Station.DBS["SFCDB"]);
                PackingTool.InsertPacking(Station.BU, Station.Line, Station.StationName, Station.IP, Station.LoginUser.EMP_NO, PackNo, Station.StationName, "", sn.SkuNo, PackingConfig.MAX_QTY, 0, "0", Station.SFCDB);
                CartonSession.Value = PackNo;
            }
        }

        /// <summary>
        /// 輸入 Carton 加載棧板號
        /// PackNo 1
        /// SKU 1
        /// ParentPackNo 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadPalletByCarton(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
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
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }
            SKU sku = (SKU)SkuSession.Value;

            MESStationSession ParentPackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ParentPackNoSession == null)
            {
                ParentPackNoSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(ParentPackNoSession);
            }

            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_PACKING PackingConfigTool = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_PACKING Packing = PackingTool.GetPackingByPackNo(PackNo, Station.SFCDB);
            C_PACKING PackingConfig = PackingConfigTool.GetPackingBySkuAndType(sku.SkuNo, Station.StationName, Station.SFCDB);
            R_PACKING ParentPacking = PackingTool.GetExistPacking(Packing.SKUNO, Station.StationName, Station.StationName, Station.Line, Station.IP, Station.SFCDB);
            if (ParentPacking != null)
            {
                ParentPackNoSession.Value = ParentPacking.PACK_NO;
            }
            else
            {
                string NewPallet = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(PackingConfig.SN_RULE, Station.DBS["SFCDB"]);
                PackingTool.InsertPacking(Station.BU, Station.Line, Station.StationName, Station.IP, Station.LoginUser.EMP_NO, NewPallet, Station.StationName, "", sku.SkuNo, PackingConfig.MAX_QTY, 0, "0", Station.SFCDB);
                ParentPackNoSession.Value = NewPallet;
            }
        }

        public static void LoadPackingBySkuStation(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {


            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new Exception("Can't load session SKU");
            }
            LogicObject.SKU SKU = (LogicObject.SKU)sessionSKU.Value;
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

            T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_PACKING> PackConfigs = TCP.GetPackingBySku(SKU.SkuNo, Station.SFCDB);
            C_PACKING CartonConfig = PackConfigs.Find(T => T.PACK_TYPE == "CARTON");
            C_PACKING PalletConfig = PackConfigs.Find(T => T.PACK_TYPE == "PALLET");
            if (CartonConfig == null)
            {
                throw new Exception("Can't find CartionConfig");
            }
            if (PalletConfig == null)
            {
                throw new Exception("Can't find PalletConfig");
            }

            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_PACKING RowCartion = null;
            try
            {
                RowCartion = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{SKU.SkuNo}' 
and PACK_TYPE='{CartonConfig.PACK_TYPE}' 
and LINE='{Station.Line}' 
and STATION='{Station.StationName}' 
and IP='{Station.IP}' and CLOSED_FLAG='0'", Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            catch
            {
                RowCartion = Packing.PackingBase.GetNewPacking(CartonConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }

            Row_R_PACKING RowPallet = null;
            try
            {
                RowPallet = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{SKU.SkuNo}' 
and PACK_TYPE='{PalletConfig.PACK_TYPE}' 
and LINE='{Station.Line}' 
and STATION='{Station.StationName}' 
and IP='{Station.IP}' and CLOSED_FLAG='0'", Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            catch
            {
                RowPallet = Packing.PackingBase.GetNewPacking(PalletConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }
            Packing.CartionBase Cartion = new Packing.CartionBase(RowCartion);
            Packing.PalletBase Pallet = new Packing.PalletBase(RowPallet);
            if (Cartion.DATA.PARENT_PACK_ID == null || Cartion.DATA.PARENT_PACK_ID == "")
            {
                Pallet.Add(Cartion, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }

            sessionCarton.Value = Cartion;
            sessionPallet.Value = Pallet;



        }


        public static void LoadPackingBySkuTransportStation(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new Exception("Can't load session SKU");
            }
            LogicObject.SKU SKU = (LogicObject.SKU)sessionSKU.Value;
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
            MESStationSession sessionTransprot = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionTransprot == null)
            {
                throw new Exception("Can't load session SKU Transprot");
            }

            var strPT = sessionTransprot.Value.ToString();
            C_PACKING PalletConfig;
            C_PACKING CartonConfig;
            if (strPT.IndexOf(':') > 0)
            {
                var strR = $@"^(\S+):\((\d+)\)$";
                var R = Regex.Match(strPT, strR);
                if (R.Success == true)
                {
                    PalletConfig = new C_PACKING() { SKUNO = SKU.SkuNo, INSIDE_PACK_TYPE = "CARTON", MAX_QTY = 1, PACK_TYPE = "PALLET", SN_RULE = "PLPallet", TRANSPORT_TYPE = R.Groups[1].Value };
                    CartonConfig = new C_PACKING() { SKUNO = SKU.SkuNo, INSIDE_PACK_TYPE = "SN", MAX_QTY = 1, PACK_TYPE = "CARTON", SN_RULE = "CTNCarton", TRANSPORT_TYPE = R.Groups[1].Value };
                    CartonConfig.MAX_QTY = int.Parse(R.Groups[2].Value);
                }
                else
                {
                    throw new Exception($@"ERR:{strPT}");
                }
            }
            else
            {
                    T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    List<C_PACKING> PackConfigs = TCP.GetPackingBySku(SKU.SkuNo, Station.SFCDB);
                    CartonConfig = PackConfigs.Find(T => T.PACK_TYPE == "CARTON" && T.TRANSPORT_TYPE == sessionTransprot.Value.ToString());
                    PalletConfig = PackConfigs.Find(T => T.PACK_TYPE == "PALLET" && T.TRANSPORT_TYPE == sessionTransprot.Value.ToString());

            }

            
            if (CartonConfig == null)
            {
                throw new Exception("Can't find CartionConfig");
            }
            if (PalletConfig == null)
            {
                throw new Exception("Can't find PalletConfig");
            }

            T_C_PACKING_RULE TCPKRule = new T_C_PACKING_RULE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //C_PACKING_RULE packRuleCarton = TCPKRule.GetPackingRuleBySkuNo(SKU.SkuNo, CartonConfig.PACK_TYPE, Station.SFCDB);//Lấy thông tin config REGEX của CARTON
            C_PACKING_RULE packRulePallet = null;
            if (Station.BU == "VNDCN")
            {
                 packRulePallet = TCPKRule.GetPackingRuleBySkuNo(SKU.SkuNo, PalletConfig.PACK_TYPE, Station.SFCDB);//Lấy thông tin config REGEX của PALLET
            }
            string packNo = "";

            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            
            Row_R_PACKING RowPallet = null;
            try
            {
                //                var strSql = $@"select * from R_PACKING where SKUNO='{SKU.SkuNo}' 
                //and PACK_TYPE='{PalletConfig.PACK_TYPE}' 
                //and LINE='{Station.Line}' 
                //and STATION='{Station.StationName}' 
                //and IP='{Station.IP}' and CLOSED_FLAG='0' and max_qty in (
                // select MAX_QTY FROM c_packing  where SKUNO='{SKU.SkuNo}' and PACK_TYPE='{PalletConfig.PACK_TYPE}'  and  TRANSPORT_TYPE='{PalletConfig.TRANSPORT_TYPE}')";

                var strSql = $@"select * from R_PACKING where SKUNO='{SKU.SkuNo}' 
                and PACK_TYPE='{PalletConfig.PACK_TYPE}' 
                and LINE='{Station.Line}' 
                and STATION='{Station.StationName}' 
                and IP='{Station.IP}' and CLOSED_FLAG='0' and max_qty = {PalletConfig.MAX_QTY}";

                RowPallet = (Row_R_PACKING)TRP.GetObjBySelect(strSql, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                if (packRulePallet != null && Station.BU == "VNDCN")
                {
                    if (packRulePallet.REGEX == null) throw new Exception("PackNo not Config Packing RULE. Pls call QE Setup SKUNO: " + SKU.SkuNo);
                    if (!Regex.IsMatch(RowPallet.PACK_NO, packRulePallet.REGEX))
                    {
                        throw new Exception("PackNo not matching Packing RULE. Pls call QE Setup SKUNO: " + SKU.SkuNo);
                    }
                }
            }
            catch(Exception ee)
            {
                if (packRulePallet != null && Station.BU == "VNDCN")
                {
                    if (packRulePallet.REGEX == null) throw new Exception("PackNo not Config Packing RULE. Pls call QE Setup SKUNO: " + SKU.SkuNo);
                    packNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(PalletConfig.SN_RULE, Station.SFCDB);
                    if (Regex.IsMatch(packNo, packRulePallet.REGEX))
                        RowPallet = Packing.PackingBase.GetNewPackNo(PalletConfig, packNo, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
                    else throw new Exception("PackNo not matching PACKING RULE. Pls call QE Setup SKU: " + SKU.SkuNo);
                }
                else
                {
                    packNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(PalletConfig.SN_RULE, Station.SFCDB);
                    RowPallet = Packing.PackingBase.GetNewPackNo(PalletConfig, packNo, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
                }
                // RowPallet = Packing.PackingBase.GetNewPacking(PalletConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }



            Row_R_PACKING RowCartion = null;
            try
            {
                RowCartion = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{SKU.SkuNo}' 
and PACK_TYPE='{CartonConfig.PACK_TYPE}' 
and LINE='{Station.Line}' 
and STATION='{Station.StationName}' 
and IP='{Station.IP}' and CLOSED_FLAG='0'  and max_qty ={CartonConfig.MAX_QTY} ", Station.SFCDB, DB_TYPE_ENUM.Oracle);
                //if (packRuleCarton != null)
                //{
                //    if (packRuleCarton.REGEX == null) throw new Exception("PackNo not Config Carton RULE. Pls call QE Setup for SKU: " + SKU.SkuNo);
                //    if (!Regex.IsMatch(RowCartion.PACK_NO, packRuleCarton.REGEX))
                //    {
                //        throw new Exception("PackNo not matching Carton RULE. Pls call QE Setup for SKU: " + SKU.SkuNo);
                //    }
                //}
            }
            catch
            {
                //if (packRuleCarton != null)
                //{
                //    if (packRuleCarton.REGEX == null) throw new Exception("PackNo not Config Carton RULE. Pls call QE Setup for SKU: " + SKU.SkuNo);
                //    packNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(CartonConfig.SN_RULE, Station.SFCDB);
                //    if (Regex.IsMatch(packNo, packRuleCarton.REGEX))
                //        RowCartion = Packing.PackingBase.GetNewPackNo(CartonConfig, packNo, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
                //    else throw new Exception("PackNo not matching CARTON RULE. Pls call PE Setup ^_^");
                //}
                //else
                //{
                packNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(CartonConfig.SN_RULE, Station.SFCDB);
                RowCartion = Packing.PackingBase.GetNewPackNo(CartonConfig, packNo, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
                //}
            }

            Packing.CartionBase Cartion = new Packing.CartionBase(RowCartion);
            Packing.PalletBase Pallet = new Packing.PalletBase(RowPallet);
            if (Cartion.DATA.PARENT_PACK_ID == null || Cartion.DATA.PARENT_PACK_ID == "")
            {
                Pallet.Add(Cartion, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }

            sessionCarton.Value = Cartion;
            sessionPallet.Value = Pallet;
        }

        /// <summary>
        /// 根據一個包裝號生成另外一個同類型的包裝號
        /// Type      Key    Value
        /// Location1 1
        /// Location2 1
        /// ItemList2 1
        /// Count2 1
        ///                  Location2
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void EmptyPackLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LocationSession == null)
            {
                //報錯
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStation.LogicObject.Packing Location = (MESStation.LogicObject.Packing)LocationSession.Value;
            MESStation.LogicObject.Packing NewLocation = new LogicObject.Packing();

            MESStationSession NewLocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NewLocationSession == null)
            {
                NewLocationSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(NewLocationSession);

                C_PACKING PackingConfig = TCP.GetPackingBySkuAndType(Location.Skuno, Location.PackType, Station.SFCDB);

                string NewLocationStr = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(PackingConfig.SN_RULE, Station.DBS["SFCDB"]);
                TRP.InsertPacking(Station.BU, Station.Line, Station.StationName, Station.IP, Station.LoginUser.EMP_NO, NewLocationStr, Location.PackType, Location.ParentPackID, Location.Skuno, PackingConfig.MAX_QTY, 0, "0", Station.SFCDB);
                TRP.ClosePack(NewLocationStr, Station.LoginUser.EMP_NO, Station.SFCDB);
                NewLocation.DataLoad(NewLocationStr, Station.LoginUser.BU, Station.SFCDB, Station.DBType);
                NewLocationSession.Value = NewLocation;

                MESStationSession ItemListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (ItemListSession == null)
                {
                    ItemListSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = new List<string>() };
                    Station.StationSession.Add(ItemListSession);
                }

                MESStationSession CountSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (CountSession == null)
                {
                    CountSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = 0 };
                    Station.StationSession.Add(CountSession);
                }

                Station.FindInputByName(Paras[4].VALUE).Value = NewLocationStr;
            }



        }

        public static void PackSettingLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSKUNOSETTING = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKUNOSETTING == null)
            {
                sessionSKUNOSETTING = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY };
                Station.StationSession.Add(sessionSKUNOSETTING);
            }

            MESStationSession sessionLineName = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionLineName == null)
            {
                sessionLineName = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                sessionLineName.Value = Station.Line;
                Station.StationSession.Add(sessionLineName);
            }

            MESStationSession sessionCARQTY = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionCARQTY == null)
            {
                sessionCARQTY = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionCARQTY);
            }

            MESStationSession sessionMCARQTY = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionMCARQTY == null)
            {
                sessionMCARQTY = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(sessionMCARQTY);
            }

            MESStationSession sessionCARNO = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionCARNO == null)
            {
                sessionCARNO = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                Station.StationSession.Add(sessionCARNO);
            }

            MESStationSession sessionPALNO = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionPALNO == null)
            {
                sessionPALNO = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, SessionKey = Paras[5].SESSION_KEY };
                Station.StationSession.Add(sessionPALNO);
            }

            MESStationSession sessionPALQTY = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (sessionPALQTY == null)
            {
                sessionPALQTY = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, SessionKey = Paras[6].SESSION_KEY };
                Station.StationSession.Add(sessionPALQTY);
            }

            MESStationSession sessionMPALQTY = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            if (sessionMPALQTY == null)
            {
                sessionMPALQTY = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, SessionKey = Paras[7].SESSION_KEY };
                Station.StationSession.Add(sessionMPALQTY);
            }

            MESStationSession sessionCASTATUS = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            if (sessionCASTATUS == null)
            {
                sessionCASTATUS = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, SessionKey = Paras[8].SESSION_KEY };
                Station.StationSession.Add(sessionCASTATUS);
            }

            MESStationSession sessionPASTATUS = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);
            if (sessionPASTATUS == null)
            {
                sessionPASTATUS = new MESStationSession() { MESDataType = Paras[9].SESSION_TYPE, SessionKey = Paras[9].SESSION_KEY };
                Station.StationSession.Add(sessionPASTATUS);
            }

            MESStationSession sessionskuno = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
            if (sessionskuno == null)
            {
                sessionskuno = new MESStationSession() { MESDataType = Paras[10].SESSION_TYPE, SessionKey = Paras[10].SESSION_KEY };
                Station.StationSession.Add(sessionskuno);
            }
            SKU sKU = (SKU)sessionskuno.Value;
            T_SFCPACKOUTSETTING Tsfcset = new T_SFCPACKOUTSETTING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<SFCPACKOUTSETTING> listsetting = Tsfcset.GETPACKBYSKUNOANDLINE(Station.SFCDB, sKU.SkuNo, Station.Line);
            if (listsetting.Count == 2)
            {
                SFCPACKOUTSETTING car = listsetting.Where(t => t.TYPE == "CARTON").FirstOrDefault();
                SFCPACKOUTSETTING pal = listsetting.Where(t => t.TYPE == "PALLET").FirstOrDefault();
                //T_C_PACKING t_C_PACKING = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                //C_PACKING carbg = t_C_PACKING.GetPackingBySkuAndType(sKU.SkuNo, "CARTON",Station.SFCDB);
                //C_PACKING palbg = t_C_PACKING.GetPackingBySkuAndType(sKU.SkuNo, "PALLET", Station.SFCDB);
                sessionCARQTY.Value = car.QTY;
                sessionMCARQTY.Value = car.MAXQTY;
                sessionCARNO.Value = car.PACKNO;
                sessionPALQTY.Value = pal.QTY;
                sessionMPALQTY.Value = pal.MAXQTY;
                sessionPALNO.Value = pal.PACKNO;
                T_R_PACKING t_R_PACKING = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                R_PACKING p_car = t_R_PACKING.GetBYPACKNO(car.PACKNO, Station.SFCDB);
                R_PACKING p_pal = t_R_PACKING.GetBYPACKNO(pal.PACKNO, Station.SFCDB);
                if (p_car != null)
                {
                    if (p_car.CLOSED_FLAG == "1")
                    {
                        sessionCASTATUS.Value = "CLOSED";
                    }
                    else
                    {
                        sessionCASTATUS.Value = "OPEN";
                    }
                }
                else
                {
                    sessionCASTATUS.Value = "CLOSED";
                }

                if (p_pal != null)
                {
                    if (p_pal.CLOSED_FLAG == "1")
                    {
                        sessionPASTATUS.Value = "CLOSED";
                    }
                    else
                    {
                        sessionPASTATUS.Value = "OPEN";
                    }
                }
                else
                {
                    sessionPASTATUS.Value = "CLOSED";
                }
                sessionSKUNOSETTING.Value = car.SKUNO;
            }

        }

        public static void CartonNumberLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN SNObj = (SN)sessionSN.Value;
            MESStationSession cartonSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (cartonSession == null)
            {
                cartonSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(cartonSession);
            }
            var carton = Station.SFCDB.ORM.Queryable<R_SN_PACKING, R_PACKING>((rsp, rp) => rsp.PACK_ID == rp.ID)
                .Where((rsp, rp) => rsp.SN_ID == SNObj.ID).Select((rsp, rp) => rp).ToList().FirstOrDefault();
            if (carton == null)
            {
                throw new Exception($@"{SNObj.SerialNo} Not Pass Packout!");
            }
            cartonSession.Value = carton.PACK_NO;
        }

        public static void CartonWeightLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            SN SNObj = (SN)sessionSN.Value;
            MESStationSession weightSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (weightSession == null)
            {
                weightSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(weightSession);
            }
            string unit = "KG";
            try
            {
                unit = Paras[2].VALUE;
            }
            catch (Exception)
            {
                unit = "KG";
            }            
            var carton = Station.SFCDB.ORM.Queryable<R_SN_PACKING, R_PACKING>((rsp, rp) => rsp.PACK_ID == rp.ID)
                .Where((rsp, rp) => rsp.SN_ID == SNObj.ID).Select((rsp, rp) => rp).ToList().FirstOrDefault();
            if (carton == null)
            {
                throw new Exception($@"{SNObj.SerialNo} Not Pass Packout!");
            }
            var weight = Station.SFCDB.ORM.Queryable<R_WEIGHT>().Where(r => r.SNID == carton.ID && r.STATION == Station.StationName)
                .OrderBy(r => r.CREATETIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (weight == null)
            {
                weightSession.Value = $@"0 {unit}";
            }
            else
            {
                weightSession.Value = $@"{weight.WEIGHT} {unit}";
            }            
        }

        public static void JuniperLoadPackingByTransport(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSkuno = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSkuno == null || sessionSkuno.Value == null)
            {
                throw new Exception("Can't load session SKU");
            }
            string skuno = sessionSkuno.Value.ToString();
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
            MESStationSession sessionTransprot = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionTransprot == null)
            {
                throw new Exception("Can't load session SKU Transprot");
            }
            T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_PACKING> PackConfigs = TCP.GetPackingBySku(skuno, Station.SFCDB);
            C_PACKING CartonConfig = PackConfigs.Find(T => T.PACK_TYPE == "CARTON" && T.TRANSPORT_TYPE == sessionTransprot.Value.ToString());
            C_PACKING PalletConfig = PackConfigs.Find(T => T.PACK_TYPE == "PALLET" && T.TRANSPORT_TYPE == sessionTransprot.Value.ToString());
            if (CartonConfig == null)
            {
                throw new Exception("Can't find CartionConfig");
            }
            if (PalletConfig == null)
            {
                throw new Exception("Can't find PalletConfig");
            }

            T_C_PACKING_RULE TCPKRule = new T_C_PACKING_RULE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
           
            string packNo = "";

            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            Row_R_PACKING RowPallet = null;
            try
            {
                RowPallet = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{skuno}' 
                                and PACK_TYPE='{PalletConfig.PACK_TYPE}' 
                                and LINE='{Station.Line}' 
                                and STATION='{Station.StationName}' 
                                and IP='{Station.IP}' and CLOSED_FLAG='0' and max_qty in (
                                 select MAX_QTY FROM c_packing  where SKUNO='{skuno}' and PACK_TYPE='{PalletConfig.PACK_TYPE}'  
                                and  TRANSPORT_TYPE='{PalletConfig.TRANSPORT_TYPE}')", Station.SFCDB, DB_TYPE_ENUM.Oracle);                
            }
            catch
            {
                packNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(PalletConfig.SN_RULE, Station.SFCDB);
                RowPallet = Packing.PackingBase.GetNewPackNo(PalletConfig, packNo, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }


            Row_R_PACKING RowCartion = null;
            try
            {
                RowCartion = (Row_R_PACKING)TRP.GetObjBySelect($@"select * from R_PACKING where SKUNO='{skuno}' 
                                and PACK_TYPE='{CartonConfig.PACK_TYPE}' 
                                and LINE='{Station.Line}' 
                                and STATION='{Station.StationName}' 
                                and IP='{Station.IP}' and CLOSED_FLAG='0'  and max_qty in (
                                 select MAX_QTY FROM c_packing  where SKUNO='{skuno}' and PACK_TYPE='{CartonConfig.PACK_TYPE}'  
                                and  TRANSPORT_TYPE='{CartonConfig.TRANSPORT_TYPE}') ", Station.SFCDB, DB_TYPE_ENUM.Oracle);               
            }
            catch
            {                
                packNo = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(CartonConfig.SN_RULE, Station.SFCDB);
                RowCartion = Packing.PackingBase.GetNewPackNo(CartonConfig, packNo, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);                
            }

            Packing.CartionBase Cartion = new Packing.CartionBase(RowCartion);
            Packing.PalletBase Pallet = new Packing.PalletBase(RowPallet);
            if (Cartion.DATA.PARENT_PACK_ID == null || Cartion.DATA.PARENT_PACK_ID == "")
            {
                Pallet.Add(Cartion, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }
            sessionCarton.Value = Cartion;
            sessionPallet.Value = Pallet;
        }
    }
}
