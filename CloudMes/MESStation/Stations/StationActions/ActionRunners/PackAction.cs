using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System.Collections;
using MESDataObject;
using System.Data;
using MESDBHelper;
using MESStation.Stations.StationActions.DataLoaders;
using System.Data.OleDb;
using System.Collections.Generic;
using MESDataObject.Module;
using System.Reflection;
using MESStation.Label;
using MESPubLab.MESStation;
using MESStation.Packing;
using System.Linq;
using System;
using System.Configuration;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module.Vertiv;
using SqlSugar;
using MESPubLab.Json;
using MESStation.Config.DCN;
using MES_DCN.Broadcom;
using MESDataObject.Module.HWD;
using System.Text.RegularExpressions;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class PackAction
    {
        static string SHIPOUT_CATEGORY_SKUNO = "ABE600M1,BE650G1,BR1500MS,BR1500MS2,BR700G,BX1500M,S9700-23D-J80,S9700-23D-JB4,S9700-53DX-JB4,S9705-48D-480,S9705-48D-4B4";
        public static void CloseCartionAndPalletAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }

            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCarton == null)
            {
                throw new System.Exception("sessionCartion miss ");
            }

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionPallet miss ");
            }
            MESStationSession sessionPrintPL = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_PL" && t.SessionKey == "1");
            if (sessionPrintPL == null)
            {
                sessionPrintPL = new MESStationSession() { MESDataType = "ISPRINT_PL", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintPL);
            }
            sessionPrintPL.Value = "FALSE";
            MESStationSession sessionPrintCTN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN" && t.SessionKey == "1");
            if (sessionPrintCTN == null)
            {
                sessionPrintCTN = new MESStationSession() { MESDataType = "ISPRINT_CTN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN);
            }
            sessionPrintCTN.Value = "FALSE";
            CartionBase cartion = (CartionBase)sessionCarton.Value;
            PalletBase Pallet = (PalletBase)sessionPallet.Value;

            var carton_obj = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PACK_NO == cartion.DATA.PACK_NO).ToList().FirstOrDefault();

            if (!(carton_obj == null || carton_obj.QTY == 0))
            {
                //carton_obj = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PARENT_PACK_ID == carton_obj.PARENT_PACK_ID && r.QTY > 0).ToList().FirstOrDefault();
                sessionPrintCTN.Value = "TRUE";
            }
            //if (carton_obj.QTY == 0)
            //{
            //    carton_obj = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PARENT_PACK_ID == carton_obj.PARENT_PACK_ID && r.QTY > 0).ToList().FirstOrDefault();
            //}

            cartion.DATA.CLOSED_FLAG = "1";
            cartion.DATA.EDIT_TIME = DateTime.Now;
            cartion.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
            if (cartion.DATA.QTY == 0)
            {
                cartion.DATA.PARENT_PACK_ID = "";
            }
            Station.SFCDB.ExecSQL(cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));

            //Pallet.DATA.QTY = Pallet.GetSnCount(SFCDB);
            Pallet.DATA.QTY = Pallet.GetCount(SFCDB); //modify by fgg 2019.08.28  我不知道上面為啥會棧板的QTY會取棧板裡面所有SN的數量,照這樣取的話，棧板QTY的值就有可能會大於MAX_QTY的值
            Pallet.DATA.CLOSED_FLAG = "1";
            Pallet.DATA.EDIT_TIME = DateTime.Now;
            Pallet.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
            Station.SFCDB.ExecSQL(Pallet.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
            //sessionPrintPL.Value = "TRUE";
            //sessionPrintCTN.Value = "TRUE";

            if (Pallet.GetSnCount(SFCDB) > 0)
            {
                sessionPrintPL.Value = "TRUE";
            }

            MESStationSession CTNPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_CTN" && T.SessionKey == "1");
            if (CTNPrintSession == null)
            {
                CTNPrintSession = new MESStationSession() { MESDataType = "PRINT_CTN", SessionKey = "1" };
                Station.StationSession.Add(CTNPrintSession);
            }
            CTNPrintSession.Value = carton_obj.PACK_NO;//cartion.DATA.PACK_NO;

            MESStationSession PlPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_PL" && T.SessionKey == "1");
            if (PlPrintSession == null)
            {
                PlPrintSession = new MESStationSession() { MESDataType = "PRINT_PL", SessionKey = "1" };
                Station.StationSession.Add(PlPrintSession);
            }
            PlPrintSession.Value = Pallet.DATA.PACK_NO;


            Station.StationSession.Remove(sessionCarton);
            Station.StationSession.Remove(sessionPallet);




        }
        //單獨關閉卡通
        public static void CloseCartonAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }

            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCarton == null)
            {
                throw new System.Exception("sessionCartion miss ");
            }
            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionPallet miss ");
            }

            MESStationSession sessionPrintCTN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN" && t.SessionKey == "1");
            if (sessionPrintCTN == null)
            {
                sessionPrintCTN = new MESStationSession() { MESDataType = "ISPRINT_CTN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN);
            }
            sessionPrintCTN.Value = "FALSE";
            CartionBase cartion = (CartionBase)sessionCarton.Value;
            PalletBase Pallet = (PalletBase)sessionPallet.Value;
            //var cartion = Station.SFCDB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == sessionCarton.Value.ToString()).ToList().FirstOrDefault();
            //if (cartion == null)
            //{
            //    throw new System.Exception($@"Carton {sessionCarton.Value.ToString()} not exists");
            //}

            var carton_obj = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PACK_NO == cartion.DATA.PACK_NO).ToList().FirstOrDefault();

            if (!(carton_obj == null || carton_obj.QTY == 0))
            {
                //carton_obj = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PARENT_PACK_ID == carton_obj.PARENT_PACK_ID && r.QTY > 0).ToList().FirstOrDefault();
                sessionPrintCTN.Value = "TRUE";
            }

            cartion.DATA.CLOSED_FLAG = "1";
            cartion.DATA.EDIT_TIME = DateTime.Now;
            cartion.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
            if (cartion.DATA.QTY == 0)
            {
                cartion.DATA.PARENT_PACK_ID = "";
            }
            Station.SFCDB.ExecSQL(cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));

            if (Pallet.DATA.MAX_QTY == Pallet.DATA.QTY)
            {
                Pallet.DATA.CLOSED_FLAG = "1";
                Pallet.DATA.EDIT_TIME = DateTime.Now;
                Pallet.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
            }
            else
            {
                Pallet.DATA.QTY += 1;
            }
            Station.SFCDB.ExecSQL(Pallet.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));

            MESStationSession CTNPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_CTN" && T.SessionKey == "1");
            if (CTNPrintSession == null)
            {
                CTNPrintSession = new MESStationSession() { MESDataType = "PRINT_CTN", SessionKey = "1" };
                Station.StationSession.Add(CTNPrintSession);
            }

            CTNPrintSession.Value = carton_obj.PACK_NO;//cartion.DATA.PACK_NO;            

            Station.StationSession.Remove(sessionCarton);
            Station.StationSession.Remove(sessionPallet);


        }


        /// <summary>
        /// HWT關閉卡通棧板打印備件模板和R5調用非環保標籤
        /// ADD BY HGB 2019.06.25 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CloseCartionAndPalletActionForHwt(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }

            MESStationSession sessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCarton == null)
            {
                throw new System.Exception("sessionCartion miss ");
            }

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionPallet miss ");
            }

            MESStationSession sessionPrintPL = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_PL" && t.SessionKey == "1");
            if (sessionPrintPL == null)
            {
                sessionPrintPL = new MESStationSession() { MESDataType = "ISPRINT_PL", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintPL);
            }
            sessionPrintPL.Value = "FALSE";
            MESStationSession sessionPrintCTN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN" && t.SessionKey == "1");
            if (sessionPrintCTN == null)
            {
                sessionPrintCTN = new MESStationSession() { MESDataType = "ISPRINT_CTN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN);
            }
            sessionPrintCTN.Value = "FALSE";

            MESStationSession sessionPrintCTN_BEIJIAN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN_BEIJIAN" && t.SessionKey == "1");
            if (sessionPrintCTN_BEIJIAN == null)
            {
                sessionPrintCTN_BEIJIAN = new MESStationSession() { MESDataType = "ISPRINT_CTN_BEIJIAN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN_BEIJIAN);
            }
            sessionPrintCTN_BEIJIAN.Value = "FALSE";

            MESStationSession sessionPrintCTN_R5 = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN_R5" && t.SessionKey == "1");
            if (sessionPrintCTN_R5 == null)
            {
                sessionPrintCTN_R5 = new MESStationSession() { MESDataType = "ISPRINT_CTN_R5", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN_R5);
            }

            CartionBase cartion = (CartionBase)sessionCarton.Value;
            PalletBase Pallet = (PalletBase)sessionPallet.Value;

            cartion.DATA.CLOSED_FLAG = "1";
            cartion.DATA.EDIT_TIME = DateTime.Now;
            cartion.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
            if (cartion.DATA.QTY == 0)
            {
                cartion.DATA.PARENT_PACK_ID = "";
            }
            Station.SFCDB.ExecSQL(cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));

            //Pallet.DATA.QTY = Pallet.GetSnCount(SFCDB);
            Pallet.DATA.QTY = Pallet.GetCount(SFCDB); //modify by fgg 2019.08.28  我不知道上面為啥會棧板的QTY會取棧板裡面所有SN的數量,照這樣取的話，棧板QTY的值就有可能會大於MAX_QTY的值
            Pallet.DATA.CLOSED_FLAG = "1";
            Pallet.DATA.EDIT_TIME = DateTime.Now;
            Pallet.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
            Station.SFCDB.ExecSQL(Pallet.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
            sessionPrintPL.Value = "TRUE";

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");
            if (sessionSN == null)
            {
                throw new System.Exception("sessionSN miss ");
            }
            SN SN = (SN)sessionSN.Value;
            T_C_CONTROL t_c_control = new T_C_CONTROL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string ROHS = t_r_wo_base.LoadWorkorder(SN.WorkorderNo, Station.SFCDB).ROHS;
            if (t_c_control.ValueIsExist("PRINT_BEIJIAN", SN.WorkorderNo, Station.SFCDB))
            {
                sessionPrintCTN_BEIJIAN.Value = "TRUE";
                sessionPrintCTN.Value = "FALSE";
            }
            else if (ROHS == "R5" && (SN.WorkorderNo.Substring(0, 6) == "002163" || SN.WorkorderNo.Substring(0, 6) == "002164"))
            {
                sessionPrintCTN_R5.Value = "TRUE";
                sessionPrintCTN.Value = "FALSE";
            }
            else
            {
                sessionPrintCTN.Value = "TRUE";
            }

            MESStationSession CTNPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_CTN" && T.SessionKey == "1");
            if (CTNPrintSession == null)
            {
                CTNPrintSession = new MESStationSession() { MESDataType = "PRINT_CTN", SessionKey = "1" };
                Station.StationSession.Add(CTNPrintSession);
            }
            CTNPrintSession.Value = cartion.DATA.PACK_NO;

            MESStationSession PlPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_PL" && T.SessionKey == "1");
            if (PlPrintSession == null)
            {
                PlPrintSession = new MESStationSession() { MESDataType = "PRINT_PL", SessionKey = "1" };
                Station.StationSession.Add(PlPrintSession);
            }
            PlPrintSession.Value = Pallet.DATA.PACK_NO;


            Station.StationSession.Remove(sessionCarton);
            Station.StationSession.Remove(sessionPallet);

        }



        /// <summary>
        /// 包裝 SN 進卡通
        /// </summary>
        /// SN 1
        /// CARTON 1
        /// PRINT 1
        /// OUT:CARTONTIP 1
        ///     SKU 1
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackCartonAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            SN sn = (SN)SnSession.Value;

            MESStationSession CartonSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (CartonSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }
            string Carton = CartonSession.Value.ToString();

            MESStationSession PrintSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (PrintSession == null)
            {
                PrintSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "FALSE" };
                Station.StationSession.Add(PrintSession);
            }

            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN_PACKING SnPackingTool = new T_R_SN_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_PACKING Packing = PackingTool.GetPackingByPackNo(Carton, Station.SFCDB);
            if (Packing.QTY == Packing.MAX_QTY)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190211165851", new string[] { Packing.MAX_QTY.ToString(), Packing.QTY.ToString() }));
            }

            PackingTool.UpdateQtyByID(Packing.ID, true, 1, Station.LoginUser.EMP_NO, Station.SFCDB);
            SnPackingTool.InsertSnPacking(sn.ID, Packing.ID, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

            Packing = PackingTool.GetPackingByPackNo(Carton, Station.SFCDB);
            if (Packing.QTY == Packing.MAX_QTY)
            {
                PrintSession.Value = "TRUE";
                PackingTool.UpdateCloseFlagByPackID(Packing.ID, "1", Station.SFCDB);
            }



            Packing = PackingTool.GetPackingByPackNo(Carton, Station.SFCDB);
            string CartonTip = Packing.PACK_NO + "(" + Packing.QTY + "/" + Packing.MAX_QTY + ")";
            MESStationSession CartonTipSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (CartonTipSession == null)
            {
                CartonTipSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = CartonTip };
                Station.StationSession.Add(CartonTipSession);
            }
            else
            {
                CartonTipSession.Value = CartonTip;
            }


        }

        /// <summary>
        /// 包裝 Carton 到棧板
        /// PACKNO 1
        /// PARENTPACKNO 1
        /// PRINT 1
        /// OUT:PARENTPACKTIP 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PackPalletAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            string PackNo = PackNoSession.Value.ToString();

            MESStationSession ParentPackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (ParentPackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }

            string ParentPackNo = ParentPackNoSession.Value.ToString();


            MESStationSession PrintSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (PrintSession == null)
            {
                PrintSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(PrintSession);
            }

            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_PACKING ParentPacking = PackingTool.GetPackingByPackNo(ParentPackNo, Station.SFCDB);
            if (!PackingTool.CheckPackNoExistByParentPackID(PackNo, ParentPacking.ID, Station.SFCDB))
            {
                PackingTool.UpdateParentPackIDByPackNo(PackNo, ParentPacking.ID, Station.LoginUser.EMP_NO, Station.SFCDB);
                PackingTool.UpdateQtyByID(ParentPacking.ID, true, 1, Station.LoginUser.EMP_NO, Station.SFCDB);
            }


            if (ParentPacking.QTY + 1 >= ParentPacking.MAX_QTY)
            {
                PackingTool.UpdateCloseFlagByPackID(ParentPacking.ID, "1", Station.SFCDB);
                //是否打印Pallet Label
                PrintSession.Value = "TRUE";
            }
            else
            {
                PrintSession.Value = "FALSE";
            }

            ParentPacking = PackingTool.GetPackingByPackNo(ParentPackNo, Station.SFCDB);
            string ParentPackTip = ParentPacking.PACK_NO + "(" + ParentPacking.QTY + "/" + ParentPacking.MAX_QTY + ")";

            MESStationSession ParentPackTipSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (ParentPackTipSession == null)
            {
                ParentPackTipSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(ParentPackTipSession);
            }
            ParentPackTipSession.Value = ParentPackTip;


        }

        /// <summary>
        /// 關閉當前包裝單位
        /// PACKNO 1
        /// PRINT 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ClosePackAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage(""));
            }
            string PackNo = PackNoSession.Value.ToString();

            MESStationSession PrintSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PrintSession == null)
            {
                PrintSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(PrintSession);
            }

            T_R_PACKING PackingTool = new T_R_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            PackingTool.ClosePack(PackNo, Station.LoginUser.EMP_NO, Station.SFCDB);

            PrintSession.Value = "TRUE";
        }

        public static void CartionAndPalletAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new System.Exception("sessionSN miss ");
            }

            MESStationSession sessionCartion = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionCartion == null)
            {
                throw new System.Exception("sessionCartion miss ");
            }

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionPallet miss ");
            }

            MESStationSession sessionTransprot = null;
            try
            {
                sessionTransprot = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            }
            catch
            { }


            MESStationSession sessionPrintPL = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_PL" && t.SessionKey == "1");
            if (sessionPrintPL == null)
            {
                sessionPrintPL = new MESStationSession() { MESDataType = "ISPRINT_PL", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintPL);
            }
            sessionPrintPL.Value = "FALSE";
            MESStationSession sessionPrintCTN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN" && t.SessionKey == "1");
            if (sessionPrintCTN == null)
            {
                sessionPrintCTN = new MESStationSession() { MESDataType = "ISPRINT_CTN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN);
            }
            sessionPrintCTN.Value = "FALSE";

            SN SN = (SN)sessionSN.Value;

            if (SN.isPacked(Station.SFCDB))
            {
                throw new System.Exception($@"{SN.SerialNo} is packed!");
            }

            CartionBase cartion = (CartionBase)sessionCartion.Value;
            PalletBase Pallet = (PalletBase)sessionPallet.Value;

            string result = "";
            int re = 0;

            cartion.Add(SN, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

            if (cartion.DATA.MAX_QTY <= cartion.GetCount(Station.SFCDB))
            {
                sessionPrintCTN.Value = "TRUE";
                //設置打印變量
                MESStationSession CTNPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_CTN" && T.SessionKey == "1");
                if (CTNPrintSession == null)
                {
                    CTNPrintSession = new MESStationSession() { MESDataType = "PRINT_CTN", SessionKey = "1" };
                    Station.StationSession.Add(CTNPrintSession);
                }
                CTNPrintSession.Value = cartion.DATA.PACK_NO;
                T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<C_PACKING> PackConfigs = TCP.GetPackingBySku(SN.SkuNo, Station.SFCDB);
                C_PACKING CartionConfig = PackConfigs.Find(T => T.PACK_TYPE == "CARTON");
                C_PACKING PalletConfig = PackConfigs.Find(T => T.PACK_TYPE == "PALLET");
                if (sessionTransprot != null)
                {
                    var strPT = sessionTransprot.Value.ToString();

                    if (strPT.IndexOf(':') > 0)
                    {
                        var strR = $@"^(\S+):\((\d+)\)$";
                        var R = Regex.Match(strPT, strR);
                        if (R.Success == true)
                        {
                            PalletConfig = new C_PACKING() { SKUNO = SN.SkuNo, INSIDE_PACK_TYPE = "CARTON", MAX_QTY = 1, PACK_TYPE = "PALLET", SN_RULE = "PLPallet", TRANSPORT_TYPE = R.Groups[1].Value };
                            CartionConfig = new C_PACKING() { SKUNO = SN.SkuNo, INSIDE_PACK_TYPE = "SN", MAX_QTY = 1, PACK_TYPE = "CARTON", SN_RULE = "CTNCarton", TRANSPORT_TYPE = R.Groups[1].Value };
                            CartionConfig.MAX_QTY = int.Parse(R.Groups[2].Value);
                        }
                        else
                        {
                            throw new Exception($@"ERR:{strPT}");
                        }
                    }
                    else
                    {

                        //List<C_PACKING> PackConfigs = TCP.GetPackingBySku(SKU.SkuNo, Station.SFCDB);
                        CartionConfig = PackConfigs.Find(T => T.PACK_TYPE == "CARTON" && T.TRANSPORT_TYPE == sessionTransprot.Value.ToString());
                        PalletConfig = PackConfigs.Find(T => T.PACK_TYPE == "PALLET" && T.TRANSPORT_TYPE == sessionTransprot.Value.ToString());

                    }
                }


                if (CartionConfig == null)
                {
                    throw new Exception("Can't find CartionConfig");
                }
                if (PalletConfig == null)
                {
                    throw new Exception("Can't find PalletConfig");
                }
                if (Pallet.DATA.MAX_QTY <= Pallet.GetCount(Station.SFCDB))
                {
                    sessionPrintPL.Value = "TRUE";
                    //設置打印變量
                    MESStationSession PlPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_PL" && T.SessionKey == "1");
                    if (PlPrintSession == null)
                    {
                        PlPrintSession = new MESStationSession() { MESDataType = "PRINT_PL", SessionKey = "1" };
                        Station.StationSession.Add(PlPrintSession);
                    }
                    PlPrintSession.Value = Pallet.DATA.PACK_NO;

                    Pallet.DATA.CLOSED_FLAG = "1";
                    Pallet.DATA.EDIT_TIME = DateTime.Now;
                    Pallet.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
                    result = Station.SFCDB.ExecSQL(Pallet.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    Int32.TryParse(result, out re);
                    if (re <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING-" + Pallet.DATA.PACK_NO }));
                    }
                    Pallet.DATA = PackingBase.GetNewPacking(PalletConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

                }
                cartion.DATA.CLOSED_FLAG = "1";
                cartion.DATA.EDIT_TIME = DateTime.Now;
                cartion.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
                result = Station.SFCDB.ExecSQL(cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
                Int32.TryParse(result, out re);
                if (re <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING-" + cartion.DATA.PACK_NO }));
                }
                cartion.DATA = PackingBase.GetNewPacking(CartionConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

                Pallet.Add(cartion, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }
            sessionCartion.Value = cartion;
            sessionPallet.Value = Pallet;

            cartion.DATA.AcceptChange();
            Pallet.DATA.AcceptChange();

        }

        /// <summary>
        /// HWT打印備件模板和R5調用非環保標籤
        /// ADD BY HGB 2019.06.25 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CartionAndPalletActionForHwt(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new System.Exception("sessionSN miss ");
            }

            MESStationSession sessionCartion = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionCartion == null)
            {
                throw new System.Exception("sessionCartion miss ");
            }

            MESStationSession sessionPallet = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionPallet == null)
            {
                throw new System.Exception("sessionPallet miss ");
            }

            MESStationSession sessionPrintPL = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_PL" && t.SessionKey == "1");
            if (sessionPrintPL == null)
            {
                sessionPrintPL = new MESStationSession() { MESDataType = "ISPRINT_PL", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintPL);
            }
            sessionPrintPL.Value = "FALSE";
            MESStationSession sessionPrintCTN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN" && t.SessionKey == "1");
            if (sessionPrintCTN == null)
            {
                sessionPrintCTN = new MESStationSession() { MESDataType = "ISPRINT_CTN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN);
            }
            sessionPrintCTN.Value = "FALSE";

            MESStationSession sessionPrintCTN_BEIJIAN = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN_BEIJIAN" && t.SessionKey == "1");
            if (sessionPrintCTN_BEIJIAN == null)
            {
                sessionPrintCTN_BEIJIAN = new MESStationSession() { MESDataType = "ISPRINT_CTN_BEIJIAN", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN_BEIJIAN);
            }
            sessionPrintCTN_BEIJIAN.Value = "FALSE";

            MESStationSession sessionPrintCTN_R5 = Station.StationSession.Find(t => t.MESDataType == "ISPRINT_CTN_R5" && t.SessionKey == "1");
            if (sessionPrintCTN_R5 == null)
            {
                sessionPrintCTN_R5 = new MESStationSession() { MESDataType = "ISPRINT_CTN_R5", SessionKey = "1", Value = "FALSE" };
                Station.StationSession.Add(sessionPrintCTN_R5);
            }
            sessionPrintCTN_R5.Value = "FALSE";

            SN SN = (SN)sessionSN.Value;

            if (SN.isPacked(Station.SFCDB))
            {
                throw new System.Exception($@"{SN.SerialNo} is packed!");
            }

            CartionBase cartion = (CartionBase)sessionCartion.Value;
            PalletBase Pallet = (PalletBase)sessionPallet.Value;



            cartion.Add(SN, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

            if (cartion.DATA.MAX_QTY <= cartion.GetCount(Station.SFCDB))
            {
                T_C_CONTROL t_c_control = new T_C_CONTROL(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                string ROHS = t_r_wo_base.LoadWorkorder(SN.WorkorderNo, Station.SFCDB).ROHS;
                if (t_c_control.ValueIsExist("PRINT_BEIJIAN", SN.WorkorderNo, Station.SFCDB))
                {
                    sessionPrintCTN_BEIJIAN.Value = "TRUE";
                    sessionPrintCTN.Value = "FALSE";
                }
                else if (ROHS == "R5" && (SN.WorkorderNo.Substring(0, 6) == "002163" || SN.WorkorderNo.Substring(0, 6) == "002164"))
                {
                    sessionPrintCTN_R5.Value = "TRUE";
                    sessionPrintCTN.Value = "FALSE";
                }
                else
                {
                    sessionPrintCTN.Value = "TRUE";
                }

                //設置打印變量
                MESStationSession CTNPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_CTN" && T.SessionKey == "1");
                if (CTNPrintSession == null)
                {
                    CTNPrintSession = new MESStationSession() { MESDataType = "PRINT_CTN", SessionKey = "1" };
                    Station.StationSession.Add(CTNPrintSession);
                }
                CTNPrintSession.Value = cartion.DATA.PACK_NO;
                T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<C_PACKING> PackConfigs = TCP.GetPackingBySku(SN.SkuNo, Station.SFCDB);
                C_PACKING CartionConfig = PackConfigs.Find(T => T.PACK_TYPE == "CARTON");
                C_PACKING PalletConfig = PackConfigs.Find(T => T.PACK_TYPE == "PALLET");
                if (CartionConfig == null)
                {
                    throw new Exception("Can't find CartionConfig");
                }
                if (PalletConfig == null)
                {
                    throw new Exception("Can't find PalletConfig");
                }
                if (Pallet.DATA.MAX_QTY <= Pallet.GetCount(Station.SFCDB))
                {
                    sessionPrintPL.Value = "TRUE";
                    //設置打印變量
                    MESStationSession PlPrintSession = Station.StationSession.Find(T => T.MESDataType == "PRINT_PL" && T.SessionKey == "1");
                    if (PlPrintSession == null)
                    {
                        PlPrintSession = new MESStationSession() { MESDataType = "PRINT_PL", SessionKey = "1" };
                        Station.StationSession.Add(PlPrintSession);
                    }
                    PlPrintSession.Value = Pallet.DATA.PACK_NO;

                    Pallet.DATA.CLOSED_FLAG = "1";
                    Pallet.DATA.EDIT_TIME = DateTime.Now;
                    Pallet.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
                    Station.SFCDB.ExecSQL(Pallet.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));

                    Pallet.DATA = PackingBase.GetNewPacking(PalletConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

                }
                cartion.DATA.CLOSED_FLAG = "1";
                cartion.DATA.EDIT_TIME = DateTime.Now;
                cartion.DATA.EDIT_EMP = Station.LoginUser.EMP_NO;
                Station.SFCDB.ExecSQL(cartion.DATA.GetUpdateString(DB_TYPE_ENUM.Oracle));
                cartion.DATA = PackingBase.GetNewPacking(CartionConfig, Station.Line, Station.StationName, Station.IP, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);

                Pallet.Add(cartion, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
            }
            sessionCartion.Value = cartion;
            sessionPallet.Value = Pallet;

            cartion.DATA.AcceptChange();
            Pallet.DATA.AcceptChange();

        }



        public static void CloseLot(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            DisplayOutPut Dis_LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO");
            MESStationInput Level = Station.Inputs.Find(t => t.DisplayName == "AQLLEVEL");
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS r = tRLotStatus.GetByLotNo(Dis_LotNo.Value.ToString(), Station.SFCDB);
            if (r.LOT_NO == null || !r.CLOSED_FLAG.Equals("0"))
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528103627", new string[] { Dis_LotNo.Value.ToString() }));
            try
            {
                //為避免調棧板數量更新，在CLOSE時重新計算更新批次數量;
                r.LOT_QTY = Station.SFCDB.ORM.Queryable<R_LOT_PACK, R_PACKING, R_PACKING, R_SN_PACKING>(
                        (rlp, rp1, rp2, rsp) => rlp.PACKNO == rp1.PACK_NO &&
                                                rp1.ID == rp2.PARENT_PACK_ID && rp2.ID == rsp.PACK_ID)
                    .Where((rlp, rp1, rp2, rsp) => rlp.LOTNO == r.LOT_NO).Select((rlp, rp1, rp2, rsp) => rsp).Count();
                //根據關閉時的AQL更新LotStatus
                T_C_AQLTYPE tCAqlType = new T_C_AQLTYPE(Station.SFCDB, Station.DBType);
                List<C_AQLTYPE> cAqlTypeList = tCAqlType.GetAqlTypeBySkunoAndLevel(r.SKUNO, Level.Value.ToString(), Station.SFCDB);

                var aqlobj = cAqlTypeList.Where(t => t.LOT_QTY > r.LOT_QTY).Any()
                    ? cAqlTypeList.Where(t => t.LOT_QTY > r.LOT_QTY).OrderBy(t => t.LOT_QTY).FirstOrDefault()
                    : cAqlTypeList.OrderByDescending(t => t.LOT_QTY).FirstOrDefault();

                r.REJECT_QTY = aqlobj.REJECT_QTY;
                r.SAMPLE_QTY = aqlobj.SAMPLE_QTY;
                r.SAMPLE_QTY = r.SAMPLE_QTY > r.LOT_QTY ? r.LOT_QTY : r.SAMPLE_QTY;
                r.CLOSED_FLAG = "1";
                r.AQL_LEVEL = Level.Value.ToString();
                r.EDIT_EMP = Station.LoginUser.EMP_NO;
                Station.SFCDB.ThrowSqlExeception = true;
                r.EDIT_TIME = tRLotStatus.GetDBDateTime(Station.SFCDB);
                Station.SFCDB.ExecSQL(r.GetUpdateString(Station.DBType));
            }
            catch (Exception e)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528105826", new string[] { Dis_LotNo.Value.ToString(), e.Message }));
            }
            finally { Station.SFCDB.ThrowSqlExeception = false; }
            #region 清空界面信息
            Station.StationSession.Clear();
            Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE).DataForUse.Clear();
            Station.Inputs.Find(t => t.DisplayName == "AQLLEVEL").DataForUse.Clear();
            #endregion
        }

        public static void PackPassStation(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSesseion = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == "1");
            T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> rSnList = new List<R_SN>();
            //rSnList = tRSn.GetSnListByPack(packSesseion.Value.ToString(), Station.SFCDB);
            tRSn.GetSNsByPackNo(packSesseion.Value.ToString(), ref rSnList, Station.SFCDB);
            if (rSnList.Count == 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { packSesseion.Value.ToString() }));

            tRSn.LotsPassStation(rSnList, Station.Line, rSnList[0].NEXT_STATION, rSnList[0].NEXT_STATION, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB); // 過站
            //記錄通過數 ,UPH
            foreach (var snobj in rSnList)
            {
                tRSn.RecordYieldRate(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                tRSn.RecordUPH(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            }
            Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102159", new string[] { rSnList[0].SKUNO, packSesseion.Value.ToString(), rSnList.Count.ToString(), rSnList[0].NEXT_STATION }), State = StationMessageState.Message });
        }
        public static void PackingPassStation(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSesseion = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == "1");
            T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> rSnList = new List<R_SN>();
            //rSnList = tRSn.GetSnListByPack(packSesseion.Value.ToString(), Station.SFCDB);
            tRSn.GetSNsByPackNo(packSesseion.Value.ToString(), ref rSnList, Station.SFCDB);
            if (rSnList.Count == 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { packSesseion.Value.ToString() }));
            var diffStation = rSnList.Select(t => t.NEXT_STATION).Distinct().Count();
            if (diffStation > 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180528095410", new string[] { packSesseion.Value.ToString(), Station.StationName }));
            }
            var nextStation = rSnList[0].NEXT_STATION;
            if (nextStation != Station.StationName)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190327103914", new string[] { rSnList[0].NEXT_STATION }));
            }
            tRSn.LotsPassStation(rSnList, Station.Line, nextStation, nextStation, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB); // 過站
            ////記錄通過數 ,UPH
            //foreach (var snobj in rSnList)
            //{
            //    tRSn.RecordYieldRate(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            //    tRSn.RecordUPH(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            //}

            //Vertiv CBS過站增加掃描儲位並記錄 By ZHB 20210608
            if (Station.StationSession.FindAll(t => t.MESDataType == "LOCATION" && t.SessionKey == "1").Count > 0)
            {
                MESStationSession sessionStock = Station.StationSession.Find(t => t.MESDataType == "LOCATION" && t.SessionKey == "1");
                string location = sessionStock.Value.ToString();
                T_R_STOCK_RECORD _STOCK = new T_R_STOCK_RECORD(Station.SFCDB, Station.DBType);
                int res = _STOCK.AddNewStockRecord(packSesseion.Value.ToString(), location, rSnList[0].SKUNO, rSnList[0].WORKORDERNO, Station.StationName, Station.BU, Station.LoginUser.EMP_NO, Station.SFCDB);
                if (res < 1)
                {
                    throw new Exception(" 記錄儲位失敗!");
                }
            }

            Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102159", new string[] { rSnList[0].SKUNO, packSesseion.Value.ToString(), rSnList.Count.ToString(), rSnList[0].NEXT_STATION }), State = StationMessageState.Message });
        }

        public static void CartonPassStation(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession packSesseion = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == "1");
            T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> rSnList = new List<R_SN>();
            tRSn.GetSNByCarton(packSesseion.Value.ToString(), ref rSnList, Station.SFCDB);
            if (rSnList.Count == 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { packSesseion.Value.ToString() }));

            tRSn.LotsPassStation(rSnList, Station.Line, rSnList[0].NEXT_STATION, rSnList[0].NEXT_STATION, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB); // 過站            
            Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102159", new string[] { rSnList[0].SKUNO, packSesseion.Value.ToString(), rSnList.Count.ToString(), rSnList[0].NEXT_STATION }), State = StationMessageState.Message });
        }

        /// <summary>
        /// 移棧板或卡通內的數據
        /// add by fgg 2018.06.08
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void MovePackingSessionValue(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 8)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionOne = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOne == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionOne.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionTwo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTwo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (sessionTwo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionValue = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionValue == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            if (sessionValue.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession sessionItemListOne = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionItemListOne == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            MESStationSession sessionItemListTwo = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionItemListTwo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }

            MESStationSession sessionCountOne = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionCountOne == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
            }

            MESStationSession sessionCountTwo = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (sessionCountTwo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
            }

            R_Station_Action_Para moveFlag = Paras[7];

            bool moveToRight;
            if (moveFlag.VALUE.ToString().Equals("0"))
            {
                moveToRight = true;
            }
            else if (moveFlag.VALUE.ToString().Equals("1"))
            {
                moveToRight = false;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[7].SESSION_TYPE + Paras[7].SESSION_KEY }));
            }

            try
            {
                LogicObject.Packing packOne = (LogicObject.Packing)sessionOne.Value;
                LogicObject.Packing packTwo = (LogicObject.Packing)sessionTwo.Value;
                string result = "";
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
                T_R_SN_PACKING t_r_sn_packing = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                T_R_MOVE_LIST t_r_move_list = new T_R_MOVE_LIST(Station.SFCDB, Station.DBType);
                T_C_PACKING t_c_packing = new T_C_PACKING(Station.SFCDB, Station.DBType);
                LogicObject.Packing packOneObject = new LogicObject.Packing();
                LogicObject.Packing packTwoObject = new LogicObject.Packing();
                R_SN r_sn;
                Row_R_MOVE_LIST rowMoveList;
                Row_R_PACKING rowPacking;
                C_PACKING c_packing = new C_PACKING();
                T_C_CUSTOMER c_cus = new T_C_CUSTOMER(Station.SFCDB, Station.DBType);
                string typesku = c_cus.GetTypeSkuno(packTwo.Skuno, Station.SFCDB);
                if (packOne.ClosedFlag == "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613090152", new string[] { packOne.PackNo }));
                }
                if (packTwo.ClosedFlag == "0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180613090152", new string[] { packTwo.PackNo }));
                }
                if (!packOne.PackType.Equals(packTwo.PackType))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612114250", new string[] { packOne.PackNo, packTwo.PackNo }));
                }
                if (!packOne.Skuno.Equals(packTwo.Skuno))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141141", new string[] { packOne.PackNo, packTwo.PackNo }));
                }
                if (!packOne.SkunoVer.Equals(packTwo.SkunoVer))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141356", new string[] { packOne.PackNo, packTwo.PackNo }));
                }

                Newtonsoft.Json.Linq.JArray moveValueArray = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(sessionValue.Value.ToString());

                if (packOne.PackType == LogicObject.PackType.PALLET.ToString().ToUpper() && typesku != "ARUBA" && typesku != "UFI")
                {
                    for (int i = 0; i < moveValueArray.Count; i++)
                    {
                        c_packing = t_c_packing.GetPackingBySkuAndType(packTwo.Skuno, LogicObject.PackType.CARTON.ToString().ToUpper(), Station.SFCDB);
                        if (c_packing.MAX_QTY == 1 && Station.BU.ToUpper().Equals("VERTIV"))
                        {
                            //更新棧板號
                            //VERTIV 當卡通包規為1時，調棧板顯示卡通內的SN,故更新信息另外處理
                            R_PACKING packingObjectTemp = t_r_packing.GetPackingObjectBySN(moveValueArray[i].ToString(), Station.SFCDB);
                            if (packingObjectTemp == null)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180620114012", new string[] { moveValueArray[i].ToString() }));
                            }
                            if (!t_r_packing.CheckPackNoExistByParentPackID(packingObjectTemp.PACK_NO, packOne.PackID, Station.SFCDB))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141600", new string[] { moveValueArray[i].ToString(), packOne.PackNo }));
                            }
                            if (t_r_packing.CheckPackNoExistByParentPackID(packingObjectTemp.PACK_NO, packTwo.PackID, Station.SFCDB))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141824", new string[] { moveValueArray[i].ToString(), packTwo.PackNo }));
                            }
                            result = t_r_packing.UpdateParentPackIDByPackNo(packingObjectTemp.PACK_NO, packTwo.PackID, Station.LoginUser.EMP_NO, Station.SFCDB);
                            rowPacking = t_r_packing.GetRPackingByPackNo(Station.SFCDB, packingObjectTemp.PACK_NO);

                        }
                        else
                        {
                            if (!t_r_packing.CheckPackNoExistByParentPackID(moveValueArray[i].ToString(), packOne.PackID, Station.SFCDB))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141600", new string[] { moveValueArray[i].ToString(), packOne.PackNo }));
                            }
                            if (t_r_packing.CheckPackNoExistByParentPackID(moveValueArray[i].ToString(), packTwo.PackID, Station.SFCDB))
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141824", new string[] { moveValueArray[i].ToString(), packTwo.PackNo }));
                            }
                            //更新棧板號
                            result = t_r_packing.UpdateParentPackIDByPackNo(moveValueArray[i].ToString(), packTwo.PackID, Station.LoginUser.EMP_NO, Station.SFCDB);
                            rowPacking = t_r_packing.GetRPackingByPackNo(Station.SFCDB, moveValueArray[i].ToString());
                        }
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                        }
                        result = t_r_packing.UpdateQtyByID(packTwo.PackID, true, 1, Station.LoginUser.EMP_NO, Station.SFCDB);
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                        }
                        result = t_r_packing.UpdateQtyByID(packOne.PackID, false, 1, Station.LoginUser.EMP_NO, Station.SFCDB);
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                        }
                        //寫入記錄

                        rowMoveList = (Row_R_MOVE_LIST)t_r_move_list.NewRow();
                        rowMoveList.ID = t_r_move_list.GetNewID(Station.BU, Station.SFCDB);
                        rowMoveList.MOVE_ID = rowPacking.ID;
                        rowMoveList.FROM_LOCATION = packOne.PackID;
                        rowMoveList.TO_LOCATION = packTwo.PackID;
                        rowMoveList.PACK_TYPE = packOne.PackType;
                        rowMoveList.MOVE_EMP = Station.LoginUser.EMP_NO;
                        rowMoveList.MOVE_DATE = Station.GetDBDateTime();
                        result = Station.SFCDB.ExecSQL(rowMoveList.GetInsertString(Station.DBType));
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_MOVE_LIST" }));
                        }

                    }
                }
                else if (packOne.PackType == LogicObject.PackType.CARTON.ToString().ToUpper())
                {
                    //執行move動作
                    for (int i = 0; i < moveValueArray.Count; i++)
                    {
                        r_sn = new R_SN();
                        r_sn = t_r_sn.GetDetailBySN(moveValueArray[i].ToString(), Station.SFCDB);
                        if (!t_r_sn_packing.CheckSNExistByPackID(r_sn.ID, packOne.PackID, Station.SFCDB))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141600", new string[] { moveValueArray[i].ToString(), packOne.PackNo }));
                        }
                        if (t_r_sn_packing.CheckSNExistByPackID(r_sn.ID, packTwo.PackID, Station.SFCDB))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141824", new string[] { moveValueArray[i].ToString(), packTwo.PackNo }));
                        }
                        //更新卡通號
                        result = t_r_sn_packing.UpdatePackIDBySnID(r_sn.ID, packTwo.PackID, Station.LoginUser.EMP_NO, Station.SFCDB);
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_PACKING" }));
                        }

                        result = t_r_packing.UpdateQtyByID(packTwo.PackID, true, 1, Station.LoginUser.EMP_NO, Station.SFCDB);
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                        }
                        result = t_r_packing.UpdateQtyByID(packOne.PackID, false, 1, Station.LoginUser.EMP_NO, Station.SFCDB);
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                        }

                        //寫入記錄
                        rowMoveList = (Row_R_MOVE_LIST)t_r_move_list.NewRow();
                        rowMoveList.ID = t_r_move_list.GetNewID(Station.BU, Station.SFCDB);
                        rowMoveList.MOVE_ID = r_sn.ID;
                        rowMoveList.FROM_LOCATION = packOne.PackID;
                        rowMoveList.TO_LOCATION = packTwo.PackID;
                        rowMoveList.PACK_TYPE = packOne.PackType;
                        rowMoveList.MOVE_EMP = Station.LoginUser.EMP_NO;
                        rowMoveList.MOVE_DATE = Station.GetDBDateTime();
                        result = Station.SFCDB.ExecSQL(rowMoveList.GetInsertString(Station.DBType));
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_MOVE_LIST" }));
                        }
                    }
                }
                else if (packOne.PackType == LogicObject.PackType.PALLET.ToString().ToUpper() && (typesku == "ARUBA" || typesku == "UFI"))
                {
                    //執行move動作
                    for (int i = 0; i < moveValueArray.Count; i++)
                    {
                        r_sn = new R_SN();
                        r_sn = t_r_sn.GetDetailBySN(moveValueArray[i].ToString(), Station.SFCDB);
                        if (!t_r_packing.CheckSNExistByID(r_sn.ID, packOne.PackID, Station.SFCDB))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141600", new string[] { moveValueArray[i].ToString(), packOne.PackNo }));
                        }
                        if (t_r_packing.CheckSNExistByID(r_sn.ID, packTwo.PackID, Station.SFCDB))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612141824", new string[] { moveValueArray[i].ToString(), packTwo.PackNo }));
                        }
                        //更新卡通號
                        result = t_r_sn_packing.UpdatePackPLIDBySnID(r_sn.ID, packTwo.PackID, Station.LoginUser.EMP_NO, Station.SFCDB);
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_PACKING" }));
                        }

                        result = t_r_packing.UpdateQtyByID(packTwo.PackID, true, 1, Station.LoginUser.EMP_NO, Station.SFCDB);
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                        }
                        result = t_r_packing.UpdateQtyByID(packOne.PackID, false, 1, Station.LoginUser.EMP_NO, Station.SFCDB);
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                        }

                        //寫入記錄
                        rowMoveList = (Row_R_MOVE_LIST)t_r_move_list.NewRow();
                        rowMoveList.ID = t_r_move_list.GetNewID(Station.BU, Station.SFCDB);
                        rowMoveList.MOVE_ID = r_sn.ID;
                        rowMoveList.FROM_LOCATION = packOne.PackID;
                        rowMoveList.TO_LOCATION = packTwo.PackID;
                        rowMoveList.PACK_TYPE = packOne.PackType;
                        rowMoveList.MOVE_EMP = Station.LoginUser.EMP_NO;
                        rowMoveList.MOVE_DATE = Station.GetDBDateTime();
                        result = Station.SFCDB.ExecSQL(rowMoveList.GetInsertString(Station.DBType));
                        if (Convert.ToInt32(result) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_MOVE_LIST" }));
                        }
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { "Pack:" + packOne.PackType }));
                }
                //關閉卡通或棧板
                result = t_r_packing.UpdateCloseFlagByPackID(packOne.PackID, "1", Station.SFCDB);
                if (Convert.ToInt32(result) == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612154506", new string[] { packOne.PackNo }));
                }
                //關閉卡通或棧板
                result = t_r_packing.UpdateCloseFlagByPackID(packTwo.PackID, "1", Station.SFCDB);
                if (Convert.ToInt32(result) == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612154506", new string[] { packTwo.PackNo }));
                }

                packOneObject.DataLoad(packOne.PackNo, Station.BU, Station.SFCDB, Station.DBType);
                packTwoObject.DataLoad(packTwo.PackNo, Station.BU, Station.SFCDB, Station.DBType);
                //当包裝單位數量為 0 時應該從系統中刪除該包裝信息
                //從系統中刪除該包裝信息后，對應棧板數量應該減少數量1
                if (packOneObject.Qty == 0)
                {
                    t_r_packing.UpdatePalletNoByCartonParentID(packOneObject.ParentPackID, Station.LoginUser.EMP_NO, Station.SFCDB);
                    t_r_packing.DeletePacking(packOneObject.PackNo, Station.SFCDB);
                    t_r_sn_packing.DeleteSnPacking(packOneObject.PackID, Station.SFCDB);
                }
                if (packTwoObject.Qty == 0)
                {
                    t_r_packing.UpdatePalletNoByCartonParentID(packTwoObject.ParentPackID, Station.LoginUser.EMP_NO, Station.SFCDB);
                    t_r_packing.DeletePacking(packTwoObject.PackNo, Station.SFCDB);
                    t_r_sn_packing.DeleteSnPacking(packTwoObject.PackID, Station.SFCDB);
                }

                sessionOne.Value = packOneObject;
                sessionTwo.Value = packTwoObject;
                if (moveToRight)
                {
                    sessionItemListOne.Value = packOneObject.PackList;
                    sessionItemListTwo.Value = packTwoObject.PackList;
                    sessionCountOne.Value = packOneObject.PackList == null ? 0 : packOneObject.PackList.Count;
                    sessionCountTwo.Value = packTwoObject.PackList == null ? 0 : packTwoObject.PackList.Count;
                }
                else
                {
                    sessionItemListOne.Value = packTwoObject.PackList;
                    sessionItemListTwo.Value = packOneObject.PackList;
                    sessionCountOne.Value = packTwoObject.PackList == null ? 0 : packTwoObject.PackList.Count;
                    sessionCountTwo.Value = packOneObject.PackList == null ? 0 : packOneObject.PackList.Count;
                }
                Station.StationMessages.Add(new StationMessage()
                {
                    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180612142034", new string[] { packOne.PackNo.ToString(), moveValueArray.Count.ToString(), packTwo.PackNo.ToString() }),
                    State = StationMessageState.Pass
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Ham ghi trạng thái RECHECK đối với các SN vs trong pallet hoặc carton
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void MovePackingCheckStation(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 8)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionOne = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOne == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionOne.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionTwo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionTwo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (sessionTwo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionValue = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionValue == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            if (sessionValue.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession sessionItemListOne = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionItemListOne == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            MESStationSession sessionItemListTwo = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionItemListTwo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }

            try
            {
                LogicObject.Packing packOne = (LogicObject.Packing)sessionOne.Value;
                LogicObject.Packing packTwo = (LogicObject.Packing)sessionTwo.Value;
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
                T_R_SN_PACKING t_r_sn_packing = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                T_R_MOVE_LIST t_r_move_list = new T_R_MOVE_LIST(Station.SFCDB, Station.DBType);
                T_C_PACKING t_c_packing = new T_C_PACKING(Station.SFCDB, Station.DBType);
                T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
                LogicObject.Packing packOneObject = new LogicObject.Packing();
                LogicObject.Packing packTwoObject = new LogicObject.Packing();
                R_SN r_sn;
                Row_R_PACKING rowPacking;
                R_SN_LOG check_log;
                C_PACKING c_packing = new C_PACKING();

                Newtonsoft.Json.Linq.JArray moveValueArray = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(sessionValue.Value.ToString());
                //Newtonsoft.Json.Linq.JArray moveValueArray2 = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(sessionItemListOne.Value.ToString());
                if (SHIPOUT_CATEGORY_SKUNO.IndexOf(packOne.Skuno) < 0 || SHIPOUT_CATEGORY_SKUNO.IndexOf(packTwo.Skuno) < 0)
                    if (packOne.PackType == LogicObject.PackType.PALLET.ToString().ToUpper() || packTwo.PackType == LogicObject.PackType.PALLET.ToString().ToUpper())
                    {
                        List<R_SN> SnList = new List<R_SN>();
                        List<R_SN> SnList2 = new List<R_SN>();
                        t_r_packing.GetSNByPackNo(packOne.PackNo, ref SnList, Station.SFCDB);
                        t_r_packing.GetSNByPackNo(packTwo.PackNo, ref SnList2, Station.SFCDB);
                        if (SnList.Count > 0 || SnList2.Count > 0)
                        {
                            var lstt = SnList.Select(t => t.NEXT_STATION).Distinct().ToList();
                            var lstt2 = SnList2.Select(t => t.NEXT_STATION).Distinct().ToList();
                            //if ((lstt.Count == 1 && lstt.Where(stringCheck => stringCheck.Contains("SHIPOUT")).Any()) || (lstt2.Count == 1 && lstt2.Where(stringCheck => stringCheck.Contains("SHIPOUT")).Any()))
                            //{
                            List<R_PACKING> lst = t_r_packing.GetListPackByParentPackId(packOne.PackID, Station.SFCDB);
                            for (int i = 0; i < lst.Count; i++)
                            {
                                //更新棧板號
                                rowPacking = t_r_packing.GetRPackingByPackNo(Station.SFCDB, lst[i].PACK_NO.ToString());
                                check_log = new R_SN_LOG();
                                check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                                check_log.SNID = rowPacking.ID;
                                check_log.SN = rowPacking.PACK_NO;
                                check_log.LOGTYPE = "RECHECK";
                                check_log.DATA1 = packOne.PackID;
                                check_log.DATA2 = packOne.PackNo;
                                check_log.DATA3 = packTwo.PackID;
                                check_log.DATA4 = packTwo.PackNo;
                                check_log.DATA5 = "CARTON";
                                check_log.DATA6 = "MOVECARTON --> RECHECK";
                                check_log.DATA7 = "0";
                                check_log.FLAG = "0";
                                check_log.CREATETIME = Station.GetDBDateTime();
                                check_log.CREATEBY = Station.LoginUser.EMP_NO;
                                Station.SFCDB.ORM.Deleteable<R_SN_LOG>().Where(t => t.SN == rowPacking.PACK_NO).ExecuteCommand();
                                int rs = Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                if (rs == 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_SN_LOG" }));
                                }

                            }
                            List<R_PACKING> lst2 = t_r_packing.GetListPackByParentPackId(packTwo.PackID, Station.SFCDB);
                            for (int i = 0; i < lst2.Count; i++)
                            {
                                //更新棧板號
                                rowPacking = t_r_packing.GetRPackingByPackNo(Station.SFCDB, lst2[i].PACK_NO.ToString());
                                check_log = new R_SN_LOG();
                                check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                                check_log.SNID = rowPacking.ID;
                                check_log.SN = rowPacking.PACK_NO;
                                check_log.LOGTYPE = "RECHECK";
                                check_log.DATA1 = packTwo.PackID;
                                check_log.DATA2 = packTwo.PackNo;
                                check_log.DATA3 = packOne.PackID;
                                check_log.DATA4 = packOne.PackNo;
                                check_log.DATA5 = "CARTON";
                                check_log.DATA6 = "MOVECARTON --> RECHECK";
                                check_log.DATA7 = "0";
                                check_log.FLAG = "0";
                                check_log.CREATETIME = Station.GetDBDateTime();
                                check_log.CREATEBY = Station.LoginUser.EMP_NO;
                                Station.SFCDB.ORM.Deleteable<R_SN_LOG>().Where(t => t.SN == rowPacking.PACK_NO).ExecuteCommand();
                                int rs = Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                if (rs == 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_SN_LOG" }));
                                }

                                // }
                            }
                        }
                    }
                    else if (packOne.PackType == LogicObject.PackType.CARTON.ToString().ToUpper() || packTwo.PackType == LogicObject.PackType.CARTON.ToString().ToUpper())
                    {
                        //執行move動作
                        List<R_SN> SnList = new List<R_SN>();
                        List<R_SN> SnList2 = new List<R_SN>();
                        t_r_packing.GetSNByPackNo(packOne.PackNo, ref SnList, Station.SFCDB);
                        if (SnList.Count > 0)
                        {
                            var SnListCheck = SnList.Select(t => t.NEXT_STATION).Distinct().ToList();
                            //if (SnListCheck.Count == 1 && SnListCheck.Where(stringCheck => stringCheck.Contains("SHIPOUT")).Any())
                            //{
                            for (int i = 0; i < SnList.Count; i++)//Danh sach SN di chuyen
                            {
                                r_sn = new R_SN();
                                r_sn = t_r_sn.GetDetailBySN(SnList[i].SN, Station.SFCDB);
                                check_log = new R_SN_LOG();
                                check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                                check_log.SNID = r_sn.ID;
                                check_log.SN = r_sn.SN;
                                check_log.LOGTYPE = "RECHECK";
                                check_log.DATA1 = packOne.PackID;
                                check_log.DATA2 = packOne.PackNo;
                                check_log.DATA3 = packTwo.PackID;
                                check_log.DATA4 = packTwo.PackNo;
                                check_log.DATA5 = "SN";
                                check_log.DATA6 = "MOVESN --> RECHECK";
                                check_log.FLAG = "0";
                                check_log.CREATETIME = Station.GetDBDateTime();
                                check_log.CREATEBY = Station.LoginUser.EMP_NO;
                                Station.SFCDB.ORM.Deleteable<R_SN_LOG>().Where(t => t.SN == r_sn.SN).ExecuteCommand();
                                int rs = Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                if (rs == 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_SN_LOG" }));
                                }
                            }
                            //}
                        }
                        t_r_packing.GetSNByPackNo(packTwo.PackNo, ref SnList2, Station.SFCDB);
                        if (SnList2.Count > 0)
                        {
                            var SnListCheck2 = SnList2.Select(t => t.NEXT_STATION).Distinct().ToList();
                            //if (SnListCheck2.Count == 1 && SnListCheck2.Where(stringCheck => stringCheck.Contains("SHIPOUT")).Any())
                            //{
                            for (int i = 0; i < SnList2.Count; i++)//Danh sach SN di chuyen
                            {
                                r_sn = new R_SN();
                                r_sn = t_r_sn.GetDetailBySN(SnList2[i].SN, Station.SFCDB);
                                check_log = new R_SN_LOG();
                                check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                                check_log.SNID = r_sn.ID;
                                check_log.SN = r_sn.SN;
                                check_log.LOGTYPE = "RECHECK";
                                check_log.DATA1 = packTwo.PackID;
                                check_log.DATA2 = packTwo.PackNo;
                                check_log.DATA3 = packOne.PackID;
                                check_log.DATA4 = packOne.PackNo;
                                check_log.DATA5 = "SN";
                                check_log.DATA6 = "MOVESN --> RECHECK";
                                check_log.FLAG = "0";
                                check_log.CREATETIME = Station.GetDBDateTime();
                                check_log.CREATEBY = Station.LoginUser.EMP_NO;
                                Station.SFCDB.ORM.Deleteable<R_SN_LOG>().Where(t => t.SN == r_sn.SN).ExecuteCommand();
                                int rs = Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                                if (rs == 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_SN_LOG" }));
                                }
                            }
                            //}
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { "Pack:" + packOne.PackType }));
                    }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage("Update ReCheck Status when you move is Error");
            }
        }

        /// <summary>
        /// 打開卡通或棧板
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OpenPackingAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
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
                string result = "";
                LogicObject.Packing packObject = (LogicObject.Packing)sessionPackObject.Value;
                T_R_PACKING t_r_packing = new T_R_PACKING(Station.SFCDB, Station.DBType);
                result = t_r_packing.UpdateCloseFlagByPackID(packObject.PackID, "0", Station.SFCDB);
                if (Convert.ToInt32(result) == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180612154414", new string[] { packObject.PackNo }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 栈板出货
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PalletShipOut(MESPubLab.MESStation.MESStationBase Station,
            MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
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
                dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString();
            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            var rDnStatus = Station.SFCDB.ORM.Queryable<R_DN_STATUS>()
                .Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine && x.DN_FLAG == "0").ToList().FirstOrDefault();
            rSn.PalletShipOutRecord(packNo, Station.LoginUser.EMP_NO, Station.Line, Station.BU, Station.StationName, rDnStatus, Station.SFCDB);
            if (rDnStatus.DN_FLAG == "1")
                Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180802154903", new string[] { dnNo, dnLine }) });
        }



        /// <summary>
        /// 栈板出货
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void DCNPalletShipOut(MESPubLab.MESStation.MESStationBase Station,
            MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string packNo = string.Empty;
            bool checkCTN = true;
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

            DataTable dt = null;
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
            dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString();
            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            string sql = $@"SELECT rp2.pack_no ,rp2.Skuno PACKNO FROM R_PACKING rp2 WHERE rp2.PARENT_PACK_ID IN(
                            select ID from R_PACKING rp1 where rp1.pack_no='{packNo}' and rp1.max_qty=1)
                            union
                            SELECT RSN.SN PACKNO,rsn.skuno FROM R_SN rsn WHERE rsn.ID IN(
                            SELECT SN_ID FROM R_SN_PACKING rsp WHERE rsp.pack_id in (
                            SELECT ID FROM R_PACKING rp2 WHERE rp2.PARENT_PACK_ID IN(
                            SELECT ID FROM R_PACKING rp1 where rp1.pack_no='{packNo}' and rp1.max_qty>1))) AND rsn.valid_flag=1";
            dt = Station.SFCDB.ExecSelect(sql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                string[] ctnArray = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ListCarton = dt.Rows[i]["PACKNO"].ToString();
                    if (i == 1)
                    {
                        bool aa = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where((r) => r.VALUE == dt.Rows[i]["SKUNO"].ToString() && r.FUNCTIONTYPE == "SHIPOUT_CHECK_ONE").Any();
                        if (aa)
                        {
                            break;
                        }
                    }
                    UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "CARTON||SN", Tittle = "輸入CARTON||SN", Type = UIInputType.String, Name = "CARTON||SN", ErrMessage = "Cancel Check" };
                    //I.OutInputs.Add(new DisplayOutPut() { Name = "Description", DisplayType = EnumHelper.GetEnumName(UIOutputType.Text), Value = "請輸入CARTON||SN" });
                    var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);

                    string strCTN = ret.ToString();

                    R_PACKING GetPack = Station.SFCDB.ORM.Queryable<R_PACKING, R_SN, C_ROUTE_DETAIL, R_SN_PACKING>((rp, rsn, crd, rsp) => rsn.ROUTE_ID == crd.ROUTE_ID && rsn.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                        .Where((rp, rsn, crd) => rsn.SN == strCTN && crd.STATION_NAME == "NSGPACKOUT").ToList().FirstOrDefault();
                    if (GetPack != null)
                    {
                        strCTN = GetPack.PACK_NO;
                    }
                    checkCTN = Station.SFCDB.ORM.Queryable<R_PACKING>().Where((r) => r.PACK_NO == strCTN && r.PACK_NO == strCTN).Any();

                    foreach (var item in ctnArray)
                    {
                        if (!(checkCTN && item != strCTN))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200522172636", new string[] { strCTN }));
                            //掃描的CARTON已經掃描過
                        }
                    }
                }
            }
            var rDnStatus = Station.SFCDB.ORM.Queryable<R_DN_STATUS>()
                .Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine && x.DN_FLAG == "0").ToList().FirstOrDefault();
            rSn.PalletShipOutRecord(packNo, Station.LoginUser.EMP_NO, Station.Line, Station.BU, Station.StationName, rDnStatus, Station.SFCDB);
            if (rDnStatus.DN_FLAG == "1")
                Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180802154903", new string[] { dnNo, dnLine }) });
        }

        public static void RMASNClosed(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession packSesseion = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == "1");
            T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> rSnList = new List<R_SN>();
            tRSn.GetSNsByPackNo(packSesseion.Value.ToString(), ref rSnList, Station.SFCDB);
            if (rSnList.Count == 0)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { packSesseion.Value.ToString() }));

            T_R_RMA_BONEPILE r_RMA_BONEPILE = new T_R_RMA_BONEPILE(Station.SFCDB, Station.DBType);
            r_RMA_BONEPILE.Update(rSnList, Station.LoginUser.EMP_NO, Station.SFCDB);
        }
        /// <summary>
        /// Check OBA Pass 章提交
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OBACheckPassStampSubmit(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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

            LogicObject.Packing objPack = (LogicObject.Packing)sessionPalletNo.Value;
            MESDataObject.Module.HWT.T_R_OBA_STAMP_CHECK TROS = new MESDataObject.Module.HWT.T_R_OBA_STAMP_CHECK(Station.SFCDB, Station.DBType);
            MESDataObject.Module.HWT.R_OBA_STAMP_CHECK log = new MESDataObject.Module.HWT.R_OBA_STAMP_CHECK();
            log.ID = TROS.GetNewID(Station.BU, Station.SFCDB);
            log.PACK_NO = objPack.PackNo;
            log.PACK_TYPE = objPack.PackType;
            log.QTY = Convert.ToDouble(sessionCheckQty.Value.ToString());
            log.STATUS = 1;
            log.EDIT_EMP = Station.LoginUser.EMP_NO;
            log.EDIT_TIME = Station.GetDBDateTime();
            int result = TROS.Save(Station.SFCDB, log);
            if (result == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_OBA_STAMP_CHECK:" + objPack.PackNo, "INSERT" }));
            }
        }
        /// <summary>
        /// HWT CBS 入庫成功后記錄入庫的位置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SaveStockListAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string location = "000";
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionLocation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionLocation != null && sessionLocation.Value != null)
            {
                location = sessionLocation.Value.ToString();
            }

            LogicObject.Packing packObj = (LogicObject.Packing)sessionPackObject.Value;
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            MESDataObject.Module.HWT.T_R_WH_STOCK_LIST TRSL = new MESDataObject.Module.HWT.T_R_WH_STOCK_LIST(sfcdb, dbtype);
            //刪除原有資料避免重複（重複入庫才會發生）
            sfcdb.ORM.Deleteable<MESDataObject.Module.HWT.R_WH_STOCK_LIST>().Where(r => r.AREA == packObj.PackNo).ExecuteCommand();

            MESDataObject.Module.HWT.R_WH_STOCK_LIST stockObj = new MESDataObject.Module.HWT.R_WH_STOCK_LIST();
            stockObj.ID = TRSL.GetNewID(Station.BU, sfcdb);
            stockObj.LOCATION = location;
            stockObj.AREA = packObj.PackNo;
            stockObj.TYPE = "DETAIL";
            stockObj.EDIT_EMP = Station.LoginUser.EMP_NO;
            stockObj.EDIT_TIME = Station.GetDBDateTime();
            sfcdb.ORM.Insertable<MESDataObject.Module.HWT.R_WH_STOCK_LIST>(stockObj).ExecuteCommand();

            sfcdb.ORM.Updateable<MESDataObject.Module.HWT.R_WH_STOCK_LIST>()
                .UpdateColumns(r => new MESDataObject.Module.HWT.R_WH_STOCK_LIST { CURRENT_QTY = r.CURRENT_QTY + 1, LESS_QTY = r.LESS_QTY + 1 })
                .Where(r => r.TYPE == "CONFIG" && r.LOCATION == location).ExecuteCommand();

        }
        /// <summary>
        /// HWT CBS 工站記錄Pallet號, 以便庫存可視化綁任務令
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SaveRecordPalletDetailAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            MESDataObject.Module.HWT.T_C_ITEM_BOM TCIB = new MESDataObject.Module.HWT.T_C_ITEM_BOM(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_RECORD_PALLET_DETAIL TRRP = new MESDataObject.Module.HWT.T_R_RECORD_PALLET_DETAIL(sfcdb, dbtype);
            if (TCIB.GET_SKU_7B5_LEVEL(sfcdb, packObj.Skuno) == "FATHER")
            {
                MESDataObject.Module.HWT.R_RECORD_PALLET_DETAIL recordDetail = new MESDataObject.Module.HWT.R_RECORD_PALLET_DETAIL();
                recordDetail.ID = TRRP.GetNewID(Station.BU, sfcdb);
                recordDetail.PALLET_NO = packObj.PackNo;
                recordDetail.SKUNO = packObj.Skuno;
                recordDetail.SKUVERISON = packObj.SkunoVer;
                recordDetail.EVENTNAME = Station.StationName;
                recordDetail.TASK_FLAG = "N";
                recordDetail.LASTEDIT = Station.GetDBDateTime();
                recordDetail.LASTBY = Station.LoginUser.EMP_NO;
                int result = TRRP.Save(sfcdb, recordDetail);
                if (result == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_RECORD_PALLET_DETAIL:" + packObj.PackNo, "INSERT" }));
                }
            }
        }
        /// <summary>
        /// HWT CBS 工站記錄PALLET DOUBLE CHECK
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SavePalletDoubleCheckAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 7)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionPackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackNo == null || sessionPackNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionCheckValue = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionCheckValue == null || sessionCheckValue == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionCheckType = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionCheckType == null || sessionCheckType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession sessionCheckQty = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionCheckQty == null)
            {
                sessionCheckQty = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = "", SessionKey = Paras[3].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionCheckQty);
            }

            MESStationSession sessionTotalQty = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionTotalQty == null || sessionTotalQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }

            MESStationSession sessionShowCheck = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionShowCheck == null || sessionShowCheck.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
            }
            MESStationSession sessionStatus = Station.StationSession.Find(r => r.MESDataType == Paras[6].SESSION_TYPE && r.SessionKey == Paras[6].SESSION_KEY);
            {
                sessionStatus = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = "", SessionKey = Paras[6].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionStatus);
            }
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK TRPD = new MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK(sfcdb, dbtype);
            MESDataObject.Module.HWT.R_PALLET_DOUBLE_CHECK objCheck = new MESDataObject.Module.HWT.R_PALLET_DOUBLE_CHECK();
            objCheck.ID = TRPD.GetNewID(Station.BU, sfcdb);
            objCheck.PALLET_NO = sessionPackNo.Value.ToString();
            objCheck.CHECK_TYPE = sessionCheckType.Value.ToString();
            objCheck.CHECK_VALUE = sessionCheckValue.Value.ToString();
            objCheck.STATUS = 1;
            objCheck.EDIT_EMP = Station.LoginUser.EMP_NO;
            objCheck.EDIT_TIME = Station.GetDBDateTime();
            int result = TRPD.Save(sfcdb, objCheck);
            if (result == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_PALLET_DOUBLE_CHECK:" + sessionPackNo.Value.ToString(), "INSERT" }));
            }

            List<MESDataObject.Module.HWT.R_PALLET_DOUBLE_CHECK> list = TRPD.GetCheckList(sfcdb, sessionPackNo.Value.ToString(), sessionCheckType.Value.ToString());
            sessionCheckQty.Value = list.Count;
            if (Convert.ToInt32(sessionTotalQty.Value.ToString()) == list.Count)
            {
                sessionShowCheck.Value = "CLOSE";
                sessionStatus.Value = "PASS";
            }
        }
        /// <summary>
        /// HWT CBS Pass Station Action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTCBSPassStationAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
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
                sessionShowCheck = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null, Value = "" };
                Station.StationSession.Add(sessionShowCheck);
            }

            MESStationSession sessionPassStatus = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPassStatus == null)
            {
                sessionPassStatus = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = null, Value = "" };
                Station.StationSession.Add(sessionPassStatus);
            }

            sessionShowCheck.Value = "";
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            LogicObject.Packing packObj = (LogicObject.Packing)sessionPackObject.Value;
            T_C_SKU_DETAIL TCSD = new T_C_SKU_DETAIL(sfcdb, dbtype);
            MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK TRPD = new MESDataObject.Module.HWT.T_R_PALLET_DOUBLE_CHECK(Station.SFCDB, Station.DBType);
            List<MESDataObject.Module.HWT.R_PALLET_DOUBLE_CHECK> list = new List<MESDataObject.Module.HWT.R_PALLET_DOUBLE_CHECK>();
            bool bPass = true;

            #region 是否要掃描棧板內的全部卡通
            C_SKU_DETAIL detailObj = TCSD.GetSkuDetail("CBS_DOUBLE_CHECK", "CBS_DOUBLE_CHECK", packObj.Skuno, sfcdb);
            if (detailObj != null)
            {
                list = TRPD.GetCheckList(sfcdb, packObj.PackNo, "CARTON");
                if (list.Count != packObj.CartonList.Count)
                {
                    sessionShowCheck.Value = "CARTON";
                    bPass = false;
                }
            }
            #endregion
            #region  是否需要掃描棧板內的所有SN PE要求如果該棧板中有重複過卡通的SN,需要double check該棧板所有的SN
            int count = 0;
            list = TRPD.GetCheckList(sfcdb, packObj.PackNo, "SN");
            if (list.Count != packObj.SNList.Count)
            {
                foreach (R_SN sn in packObj.SNList)
                {
                    count = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(r => r.SN == sn.SN && r.STATION_NAME == "CARTON").ToList().Count;
                    if (count > 1)
                    {
                        sessionShowCheck.Value = "SN";
                        bPass = false;
                        break;
                    }
                }
            }
            #endregion
            if (bPass)
            {
                #region 棧板批量過站
                T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
                List<R_SN> rSnList = new List<R_SN>();
                tRSn.GetSNsByPackNo(packObj.PackNo, ref rSnList, Station.SFCDB);
                if (rSnList.Count == 0)
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102010", new string[] { packObj.PackNo }));

                tRSn.LotsPassStation(rSnList, Station.Line, rSnList[0].NEXT_STATION, rSnList[0].NEXT_STATION, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB); // 過站
                Station.StationMessages.Add(new StationMessage()
                {
                    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180602102159",
                    new string[] { rSnList[0].SKUNO, packObj.PackNo, rSnList.Count.ToString(), rSnList[0].NEXT_STATION }),
                    State = StationMessageState.Message
                });
                sessionPassStatus.Value = "TRUE";
                #endregion
            }
            else
            {
                sessionPassStatus.Value = "FALSE";
            }
        }

        public static void CloseInputPackObjectAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }

            OleExec SFCDB = Station.SFCDB;
            LogicObject.Packing packingObject = (LogicObject.Packing)sessionPackObject.Value;
            T_R_PACKING TRP = new T_R_PACKING(SFCDB, Station.DBType);
            T_R_SN_PACKING TRSP = new T_R_SN_PACKING(SFCDB, Station.DBType);
            string emp = Station.LoginUser.EMP_NO;
            DateTime dtSys = Station.GetDBDateTime();

            SFCDB.ORM.Updateable<R_PACKING>().UpdateColumns(r => new R_PACKING { CLOSED_FLAG = "1", EDIT_EMP = emp, EDIT_TIME = dtSys })
                .Where(r => r.ID == packingObject.PackID).ExecuteCommand();


            if (packingObject.Qty == 0)
            {
                TRP.DeletePacking(packingObject.PackNo, SFCDB);
                if (packingObject.PackType == "CARTON")
                {
                    TRP.UpdatePalletNoByCartonParentID(packingObject.ParentPackID, Station.LoginUser.EMP_NO, SFCDB);
                    TRSP.DeleteSnPacking(packingObject.PackID, SFCDB);
                }
            }
            if (packingObject.PackType == "PALLET")
            {
                SFCDB.ORM.Updateable<R_PACKING>().UpdateColumns(r => new R_PACKING { CLOSED_FLAG = "1", EDIT_EMP = emp, EDIT_TIME = dtSys })
                    .Where(r => r.PARENT_PACK_ID == packingObject.PackID).ExecuteCommand();

                int count = SFCDB.ORM.Queryable<R_PACKING>().Where(r => r.PARENT_PACK_ID == packingObject.PackID && r.QTY == 0).ToList().Count;
                SFCDB.ORM.Updateable<R_PACKING>().UpdateColumns(r => new R_PACKING { QTY = r.QTY - count })
                   .Where(r => r.ID == packingObject.PackID).ExecuteCommand();

                SFCDB.ORM.Updateable<R_PACKING>().UpdateColumns(r => new R_PACKING { PARENT_PACK_ID = "", EDIT_EMP = emp, EDIT_TIME = dtSys })
                    .Where(r => r.PARENT_PACK_ID == packingObject.PackID && r.QTY == 0).ExecuteCommand();
            }
        }

        public static void OvarPackAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionDN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDN == null || sessionDN.Value == null)
            {
                throw new Exception("Pls input DN first");
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new Exception("Pls input SN first");
            }
            MESStationSession sessionPACKNO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPACKNO == null || sessionPACKNO.Value == null)
            {
                throw new Exception("Pls select a PackNo");
            }

            MESStationSession sessionSCANDN = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionSCANDN == null)
            {
                sessionSCANDN = new MESStationSession()
                {
                    MESDataType = Paras[3].SESSION_TYPE,
                    InputValue = "",
                    SessionKey = Paras[3].SESSION_KEY,
                    ResetInput = null,
                    Value = Paras[3].VALUE
                };
                Station.StationSession.Add(sessionSCANDN);
            }
            MESStationSession sessionPRINT = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionPRINT == null)
            {
                sessionPRINT = new MESStationSession()
                {
                    MESDataType = Paras[4].SESSION_TYPE,
                    InputValue = "",
                    SessionKey = Paras[4].SESSION_KEY,
                    ResetInput = null,
                    Value = Paras[4].VALUE
                };
                Station.StationSession.Add(sessionPRINT);
            }



            var SFCDB = Station.SFCDB;
            SN sn = (SN)sessionSN.Value;
            var strDN = sessionDN.Value.ToString();
            var DNitem = SFCDB.ORM.Queryable<R_DN_STATUS>().Where(t => t.DN_NO == strDN).First();
            if (sn.baseSN.SKUNO != DNitem.SKUNO)
            {
                throw new Exception($@"SN SKUNO:{sn.baseSN.SKUNO} not match DN SKUNO:{DNitem.SKUNO}");
            }

            if (SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(t => t.SN == sn.baseSN.SN && t.VALID_FLAG == 1).Any())
            {
                throw new Exception($@"SN :{sn.baseSN.SN} is already packed!");
            }

            if (sn.baseSN.SHIPPED_FLAG == "1")
            {
                throw new Exception($@"SN :{sn.baseSN.SN} is already ship!");
            }

            if (sn.baseSN.PACKED_FLAG == "0")
            {
                throw new Exception($@"SN :{sn.baseSN.SN} has not be pack!");
            }

            var PackNo = int.Parse(sessionPACKNO.Value.ToString());


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
            var packitems = SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(t => t.DN_NO == strDN && t.VALID_FLAG == 1).ToList();
            var currPack = packitems.FindAll(t => t.PACK_NO == PackNo);
            if (currPack.Count >= packConfig.PackQTY)
            {
                for (int i = 1; i <= packCount; i++)
                {
                    var sns = packitems.FindAll(t => t.PACK_NO == i);
                    if (sns.Count < packConfig.PackQTY)
                    {
                        sessionPACKNO.Value = i;
                        PackNo = i;
                        break;
                    }
                    if (i == packCount)
                    {
                        throw new Exception("DN is Full");
                    }
                }
            }
            R_SN_OVERPACK pack = new R_SN_OVERPACK()
            {
                ID = MesDbBase.GetNewID(SFCDB.ORM, Station.BU, "R_SN_OVERPACK"),
                DN_NO = DNitem.DN_NO,
                DN_LINE = DNitem.DN_LINE,
                EDIT_EMP = Station.LoginUser.EMP_NO,
                EDIT_TIME = DateTime.Now,
                PACK_NO = PackNo,
                SN = sn.baseSN.SN,
                SN_ID = sn.baseSN.ID,
                VALID_FLAG = 1
            };
            SFCDB.ORM.Insertable(pack).ExecuteCommand();
            if (currPack.Count + 1 >= packConfig.PackQTY)
            {
                sessionSCANDN.Value = "TRUE";
                sessionPRINT.Value = "TRUE";
            }
            else
            {
                sessionSCANDN.Value = "FALSE";
                sessionPRINT.Value = "FALSE";
            }

        }
        public static void OvarPackRemoveSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionDN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionDN == null || sessionDN.Value == null)
            {
                throw new Exception("Pls input DN first");
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new Exception("Pls input SN first");
            }
            MESStationSession sessionPACKNO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionPACKNO == null || sessionPACKNO.Value == null)
            {
                throw new Exception("Pls select a PackNo");
            }
            string strDN = sessionDN.Value.ToString();
            string strPACKNO = sessionPACKNO.Value.ToString();
            string strSN = sessionSN.Value.ToString();
            var SFCDB = Station.SFCDB;

            UIInputData IW = new UIInputData()
            {
                MustConfirm = true,
                Timeout = 3000000,
                IconType = IconType.None,
                UIArea = new string[] { "30%", "35%" },
                //Message = $@"确认",
                Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150803"),
                Tittle = "",
                Type = UIInputType.Confirm,
                //ErrMessage = "用户取消!"
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151112"),
            };
            //IW.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.TextArea.ToString(), Name = "警告", Value = $@"确认移除{strDN}:{strPACKNO} 中的 {strSN} ?" });
            IW.OutInputs.Add(new DisplayOutPut() { DisplayType = UIOutputType.TextArea.ToString(), Name = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151132"), Value = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151205", new string[] { strDN, strPACKNO, strSN }) });
            object res = IW.GetUiInput(Station.API, UIInput.Normal, Station);

            var packitems = SFCDB.ORM.Queryable<R_SN_OVERPACK>().Where(t => t.DN_NO == strDN && t.VALID_FLAG == 1 && t.SN == strSN).ToList();
            if (packitems.Count == 0)
            {
                throw new Exception("Data not ex");
            }
            SFCDB.ORM.Deleteable<R_SN_OVERPACK>(packitems[0]).Where(t => t.ID == packitems[0].ID).ExecuteCommand();

        }

        public static void ShipOutEndEvent(MESPubLab.MESStation.MESStationBase Station,
            MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(), dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString();
            var dnobj = Station.SFCDB.ORM.Queryable<R_DN_STATUS>()
                   .Where(t => t.DN_NO == dnNo && t.DN_LINE == dnLine && t.DN_FLAG == ENUM_R_DN_STATUS.DN_WAIT_CQA.Ext<EnumExtensions.EnumValueAttribute>().Description).ToList().FirstOrDefault();
            if (dnobj == null)
                return;
            var resRds = Station.SFCDB.ORM.Queryable<R_DN_STATUS, C_SKU, C_SERIES, C_CUSTOMER>((rds, cs, cse, cc) => rds.SKUNO == cs.SKUNO && cs.C_SERIES_ID == cse.ID && cse.CUSTOMER_ID == cc.ID)
                .Where((rds, cs, cse, cc) => rds.DN_NO == dnNo && rds.DN_LINE == dnLine).Select((rds, cs, cse, cc) => new { cc.CUSTOMER_NAME, rds.ID }).ToList().FirstOrDefault();
            if (resRds.CUSTOMER_NAME.ToUpper().Trim().Equals(Customer.NETGEAR.ExtValue()))
            {
                NetgearPtmObj broadComMds = new NetgearPtmObj(Station.BU, Station.DBS["SFCDB"]);
                var res = broadComMds.BuildDataByDnObj(dnobj, Station.SFCDB.ORM);
                if (!res.IsSuccess)
                    throw res.ErrorException;
            }
        }

        public static void PackEvent(MESPubLab.MESStation.MESStationBase Station,
            MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionCARQTY = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionCARQTY == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionMCARQTY = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionMCARQTY == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionCARNO = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionCARNO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPALNO = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionPALNO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionPALQTY = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionPALQTY == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

            MESStationSession sessionMPALQTY = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionMPALQTY == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }

        }

        /// <summary>
        /// HWD TGMES_CBS過站Action
        /// </summary>
        public static void TGMESCBSPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            try
            {
                T_R_SN_TGMES_INFO T_TGMES = new T_R_SN_TGMES_INFO(Station.SFCDB, Station.DBType);
                //List<R_SN_TGMES_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == palletNo && t.PACKING2 == cartonNo && t.VALID_FLAG == "1").ToList();
                List<R_SN_TGMES_INFO> TGMESlist = Station.SFCDB.ORM.Queryable<R_SN_TGMES_INFO>().Where(t => t.PACKING3 == palletNo && t.VALID_FLAG == "1").ToList();
                T_TGMES.LotsPassStation(TGMESlist, Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);

                Station.StationMessages.Add(new StationMessage()
                {
                    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210306182843",
                    new string[] { palletNo, cartonNo, TGMESlist.Count.ToString(), Station.StationName }),
                    State = StationMessageState.Message
                });
            }
            catch (Exception ex)
            {
                throw new Exception("TGMESCBSPassAction Error!" + ex.Message);
            }
        }

        /// <summary>
        /// HWD TGMES_SHIPOUT過站Action
        /// </summary>
        public static void TGMESShipOutAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151349", new string[] { palletNo }));
            }

            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString();
            string dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString();
            R_DN_STATUS DNSTATUS = Station.SFCDB.ORM.Queryable<R_DN_STATUS>().Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine && x.DN_FLAG == "0").ToList().FirstOrDefault();

            foreach (R_SN_TGMES_INFO TGMES in TGMESlist)
            {
                Station.SFCDB.ORM.Insertable(new R_SHIP_DETAIL()
                {
                    ID = TGMES.ID,
                    SN = TGMES.PCBA_BARCODE,
                    SKUNO = TGMES.ITEM_SALES,
                    DN_NO = DNSTATUS.DN_NO,
                    DN_LINE = DNSTATUS.DN_LINE,
                    SHIPDATE = Station.GetDBDateTime(),
                    CREATEBY = Station.LoginUser.EMP_NO,
                    SHIPORDERID = DNSTATUS.ID
                }).ExecuteCommand();
            }

            List<R_SHIP_DETAIL> SHIPList = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>().Where(x => x.DN_NO == DNSTATUS.DN_NO && x.DN_LINE == DNSTATUS.DN_LINE).ToList();
            if (SHIPList.Count > DNSTATUS.QTY)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180801091520", new string[] { palletNo, TGMESlist.Count().ToString(), DNSTATUS.QTY.ToString() }));
            else if (SHIPList.Count == DNSTATUS.QTY)
            {
                DNSTATUS.DN_FLAG = ENUM_R_DN_STATUS.DN_WAIT_CQA.ExtValue();
                DNSTATUS.EDITTIME = Station.GetDBDateTime();
                Station.SFCDB.ORM.Updateable(DNSTATUS).WhereColumns(x => new { x.DN_NO, x.DN_LINE }).ExecuteCommand();
            }
            T_R_SN_TGMES_INFO T_TGMES = new T_R_SN_TGMES_INFO(Station.SFCDB, Station.DBType);
            T_TGMES.LotsPassStation(TGMESlist, Station.Line, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            if (DNSTATUS.DN_FLAG == "1")
            {
                Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180802154903", new string[] { dnNo, dnLine }) });
            }
        }

        /// <summary>
        /// 棧板費領出貨(與DN無關)
        /// </summary>
        public static void PalletExpenseShipOut(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string expenseNo = string.Empty;
            string packNo = string.Empty;
            if (Paras.Count == 0)
            {
                packNo = Input.Value.ToString();
            }
            else
            {
                MESStationSession ExpenseNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (ExpenseNoSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                }
                expenseNo = ExpenseNoSession.Value.ToString();

                MESStationSession PackNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (PackNoSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
                }
                packNo = PackNoSession.Value.ToString();
            }
            T_R_SN rSn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            rSn.PalletExpenseShipOutRecord(expenseNo, packNo, Station.LoginUser.EMP_NO, Station.Line, Station.BU, Station.StationName, Station.SFCDB);
            Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210524151333", new string[] { expenseNo, packNo }) });
        }

        /// <summary>
        /// 彈窗輸入儲位並記錄到Session
        /// </summary>
        public static void InputStockUIAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            string section = Paras[0].VALUE.ToString();
            MESStationSession sessionStock = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStock == null)
            {
                sessionStock = new MESStationSession()
                {
                    MESDataType = Paras[1].SESSION_TYPE,
                    InputValue = "",
                    SessionKey = Paras[1].SESSION_KEY,
                    ResetInput = null
                };
                Station.StationSession.Add(sessionStock);
            }

            //UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "Location", Tittle = "請掃描儲位", Type = UIInputType.String, Name = "Location", ErrMessage = "未掃描儲位!" };            
            UIInputData I = new UIInputData() { IconType = IconType.None, UIArea = new string[] { "30%", "45%" }, Message = "Location", Tittle = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151455"), Type = UIInputType.String, Name = "Location", ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151536") };
            var ret = I.GetUiInput(Station.API, UIInput.Normal, Station);
            string location = ret.ToString().ToUpper();

            T_C_STOCK_CONFIG _STOCK = new T_C_STOCK_CONFIG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_STOCK_CONFIG> stockList = _STOCK.GetStockByValue(section, location, Station.SFCDB);
            if (stockList == null)
            {
                //throw new Exception(" 掃描儲位 " + location + " 無效!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814151554", new string[] { location }));
            }
            else
            {
                sessionStock.Value = location;
            }
        }

        public static void shipOutCheckOutWhLocation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession sessionPackObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            DataTable dt;
            if (sessionPackObject == null || sessionPackObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string pl = sessionPackObject.Value.ToString();
            string sql = $@"SELECT * FROM SFCBASE.C_WAREHOUSE_PALLET_POSITION_T WHERE PALLET_NO ='{pl}' AND   OUT_FLAG=0";
            dt = Station.SFCDB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count != 0)
            {
                string update = $@"update SFCBASE.C_WAREHOUSE_PALLET_POSITION_T set OUT_FLAG=1, TIME_CHECK_OUT=sysdate WHERE PALLET_NO ='{pl}' AND   OUT_FLAG=0";
                int a = Station.SFCDB.ExecuteNonQuery(update, CommandType.Text, null);
            }
        }
    }
}
