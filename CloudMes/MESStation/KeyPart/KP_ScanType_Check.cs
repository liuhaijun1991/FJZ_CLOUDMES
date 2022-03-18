using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDataObject;
using Newtonsoft.Json.Linq;
using System.Reflection;
using MESDataObject.Module.ALLPART;
using MESDBHelper;
using MESStation.Stations.StationActions.DataLoaders;
using MESDataObject.Module.DCN;
using MESDataObject.Module.Juniper;
using MESJuniper.Base;

namespace MESStation.KeyPart
{
    public class KP_ScanType_Check
    {
        public static void CheckTEST(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {

        }

        public static void CheckJNPRev(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            var pno = scan.PARTNO;
            var rev = scan.VALUE;

            var vers = EcnFunction.GetUsefulVer(pno, sfcdb);
            if (!vers.Contains(rev))
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("REV_OF_PARTNO_ERROR",new string[] { pno,rev }));
            }
            var LastRev = EcnFunction.GetLastMandatoryVer(pno,sfcdb);
            var currRev = vers[vers.Count - 1];
            scan.EXKEY1 = "LastMRev";
            scan.EXVALUE1 = LastRev;
            scan.EXKEY2 = "CurMRev";
            scan.EXVALUE2 = currRev;
        }

        public static void SystemSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            LogicObject.SN kpsn = new LogicObject.SN(scan.VALUE, sfcdb, DB_TYPE_ENUM.Oracle);
            T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            if (kpsn.CompletedFlag != "1")
            {
                throw new Exception($@"{kpsn.SerialNo} not finish !");
            }
            if (kpsn.ShippedFlag == "1")
            {
                throw new Exception($@"{kpsn.SerialNo} has being Shipped!");
            }
            if (kpsn.baseSN.SKUNO != scan.PARTNO)
            {
                throw new Exception($@"{kpsn.SerialNo} is {kpsn.SkuNo} config is {scan.PARTNO}");
            }
            if (kpsn.RepairFailedFlag == "1")
            {
                throw new Exception($@"{kpsn.SerialNo} is in repairing!");
            }

            kpsn.LockCheck(sfcdb);
            //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg
            if (TRKP.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }

            T_R_LINK_CONTROL t_r_link_control = new T_R_LINK_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_LINK_CONTROL> controlList = t_r_link_control.GetControlListByMainItem(sn.WorkorderNo, sfcdb);             
            if (controlList != null && controlList.Count > 0)
            {
                string controlItem = "";
                foreach (R_LINK_CONTROL r in controlList)
                {
                    controlItem += r.SUB_ITEM + ",";
                }
                controlItem = controlItem.Substring(0, controlItem.Length - 1);
                #region CHECK_ALL_MAIN_SUB  (1)MAIN ITEM只能用配置的SUB ITEM，不能用其它的；(2)而配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用
                bool bCheckAll = controlList.Any(r => r.CATEGORY == "CHECK_ALL_MAIN_SUB");
                if(bCheckAll)
                {
                    var allSubItem = controlList.Where(r => r.CATEGORY == "CHECK_ALL_MAIN_SUB").Select(r => r.SUB_ITEM).ToList();
                    if (!allSubItem.Contains(kpsn.WorkorderNo))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { sn.WorkorderNo, controlItem }));
                    }
                }
                #endregion
                //CHECK_SUB_ITEM  (1)配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用 (2)MAIN ITEM 還可以用除SUB ITEM外其它的ITEM, 也可以不用配置的SUB ITME如keypart漏配
                bool bCheckSub = controlList.Any(r => r.CATEGORY == "CHECK_SUB_ITEM");
                if (!bCheckAll && !bCheckSub)
                {
                    var control = controlList.Find(c => c.SUB_ITEM == kpsn.WorkorderNo);
                    if (control == null)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { sn.WorkorderNo, controlItem }));
                    }
                }
            }

            #region CHECK_SUB_ITEM  (1)配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用 (2)MAIN ITEM 還可以用除SUB ITEM外其它的ITEM, 也可以不用配置的SUB ITME如keypart漏配
            var subList = sfcdb.ORM.Queryable<R_LINK_CONTROL>().Where(r => r.SUB_ITEM == kpsn.WorkorderNo).ToList();
            if (subList.Count > 0)
            {
                string subMainItem = "";
                foreach (R_LINK_CONTROL sub in subList)
                {
                    subMainItem += sub.MAIN_ITEM + ",";
                }
                var controlSub = subList.Find(c => c.MAIN_ITEM == sn.WorkorderNo);
                if (controlSub != null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { kpsn.WorkorderNo, subMainItem }));
                }
            }
            #endregion


            kpsn.PassStationTimeControl(sfcdb.ORM, scan.STATION);

            kpsn.baseSN.SHIPPED_FLAG = "1";
            kpsn.baseSN.SHIPDATE = DateTime.Now;
            kpsn.baseSN.EDIT_TIME = DateTime.Now;
            kpsn.baseSN.EDIT_EMP = API.LoginUser.EMP_NO;
            sfcdb.ORM.Updateable<R_SN>(kpsn.baseSN).Where(t => t.ID == kpsn.baseSN.ID).ExecuteCommand();
        }

        public static void SNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }
        }

        public static void MACScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }
        }
        public static void GSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }
        }

        public static void CMODSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN TRKP = new T_R_SN(sfcdb, DB_TYPE_ENUM.Oracle);

            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }

            //check is scanned CMOD SN is completeded
            List<R_SN> VanillaSN = TRKP.GetFinishedVanillaSN(scan.VALUE, sfcdb);
            foreach (var a in VanillaSN)
            {
                if (!a.SKUNO.Contains(sn.SkuNo))
                {
                    throw new Exception($@"CMOD SKU {a.SKUNO} does not match!");
                }
            }
        }

        public static void CSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_LINK_CONTROL t_r_link_control = new T_R_LINK_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_LINK_CONTROL> controlList = t_r_link_control.GetControlListByMainItem(sn.WorkorderNo, sfcdb);
            LogicObject.SN kpsn = new LogicObject.SN(scan.VALUE, sfcdb, DB_TYPE_ENUM.Oracle);
            if (controlList != null && controlList.Count > 0)
            {  
                string controlItem = "";
                foreach (R_LINK_CONTROL r in controlList)
                {
                    controlItem += r.SUB_ITEM + ",";
                }
                controlItem = controlItem.Substring(0, controlItem.Length - 1); 
               
                T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
                if (kpsn == null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { sn.WorkorderNo, controlItem }));
                }
                if (kpsn.CompletedFlag != "1")
                {
                    throw new Exception($@"{kpsn.SerialNo} not finish !");
                }
                if (kpsn.ShippedFlag == "1")
                {
                    throw new Exception($@"{kpsn.SerialNo} has being Shipped!");
                }
                if (kpsn.baseSN.SKUNO != scan.PARTNO)
                {
                    throw new Exception($@"{kpsn.SerialNo} is {kpsn.SkuNo} config is {scan.PARTNO}");
                }
                if (kpsn.RepairFailedFlag == "1")
                {
                    throw new Exception($@"{kpsn.SerialNo} is in repairing!");
                }

                #region CHECK_ALL_MAIN_SUB  (1)MAIN ITEM只能用配置的SUB ITEM，不能用其它的；(2)而配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用
                bool bCheckAll = controlList.Any(r => r.CATEGORY == "CHECK_ALL_MAIN_SUB");
                if (bCheckAll)
                {
                    var allSubItem = controlList.Where(r => r.CATEGORY == "CHECK_ALL_MAIN_SUB").Select(r => r.SUB_ITEM).ToList();
                    if (!allSubItem.Contains(kpsn.WorkorderNo))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { sn.WorkorderNo, controlItem }));
                    }
                }
                #endregion
                //CHECK_SUB_ITEM  (1)配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用 (2)MAIN ITEM 還可以用除SUB ITEM外其它的ITEM, 也可以不用配置的SUB ITME如keypart漏配
                bool bCheckSub = controlList.Any(r => r.CATEGORY == "CHECK_SUB_ITEM");
                if (!bCheckAll && !bCheckSub)
                {
                    var control = controlList.Find(c => c.SUB_ITEM == kpsn.WorkorderNo);
                    if (control == null)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { sn.WorkorderNo, controlItem }));
                    }
                }
                //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg               
                if (TRKP.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
                {
                    throw new Exception(scan.VALUE + " has been link on other sn!");
                }
            }

            #region CHECK_SUB_ITEM  (1)配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用 (2)MAIN ITEM 還可以用除SUB ITEM外其它的ITEM, 也可以不用配置的SUB ITME如keypart漏配
            var subList = sfcdb.ORM.Queryable<R_LINK_CONTROL>().Where(r => r.SUB_ITEM == kpsn.WorkorderNo).ToList();
            if (subList.Count > 0)
            {
                string subMainItem = "";
                foreach (R_LINK_CONTROL sub in subList)
                {
                    subMainItem += sub.MAIN_ITEM + ",";
                }
                var controlSub = subList.Find(c => c.MAIN_ITEM == sn.WorkorderNo);
                if (controlSub != null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { kpsn.WorkorderNo, subMainItem }));
                }
            }
            #endregion
        }

        public static void LSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> kpList = t_r_sn_kp.GetKPRecordBySnID(sn.ID, sfcdb);
            R_SN_KP lastScan = kpList.Find(k => k.SCANSEQ == (scan.SCANSEQ - 1));
            string scanValue = scan.VALUE.Substring(4, scan.VALUE.Length - 4);
            string lastScanVlaue = lastScan.VALUE.Substring(0, scan.VALUE.Length - 4);
            
            //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {

                throw new Exception(scan.VALUE + " has been link on other sn!");

            }

            if (lastScan != null)
            {
                if (scanValue != lastScanVlaue)
                {
                    throw new Exception("this value " + scan.VALUE + "is inconsistent with the last one");
                }
            }
        }

        /// <summary>
        /// Check duplication keypart PS S/N
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sn"></param>
        /// <param name="scan"></param>
        /// <param name="scans"></param>
        /// <param name="API"></param>
        /// <param name="sfcdb"></param>
        /// <param name="apdb"></param>
        public static void PSSN_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);

            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }
        }

        public static void LabelSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> kpList = t_r_sn_kp.GetKPRecordBySnID(sn.ID, sfcdb);
            R_SN_KP LabelScan = kpList.Find(k => k.PARTNO == scan.PARTNO && k.SCANTYPE == "LabelSN");
            string scanValue = scan.VALUE;          
            //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {

                throw new Exception(scan.VALUE + " has been link on other sn!");

            }

            if (LabelScan != null)
            {
                if (sn.SerialNo != scanValue)
                {
                    throw new Exception("this value " + scan.VALUE + " is inconsistent with System sn");
                }
            }
        }
        /// <summary>
        /// PPID S/N
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sn"></param>
        /// <param name="scan"></param>
        /// <param name="scans"></param>
        /// <param name="API"></param>
        /// <param name="sfcdb"></param>
        /// <param name="apdb"></param>
        public static void PPIDSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            bool bol = false;
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> kpList = t_r_sn_kp.GetKPRecordBySnID(sn.ID, sfcdb);
            R_SN_KP PPIDScan = kpList.Find(k => k.PARTNO == scan.PARTNO && k.SCANTYPE == "PPID S/N");
            string scanValue = scan.VALUE;

            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {

                throw new Exception(scan.VALUE + " has been link on other sn!");

            }

            if (PPIDScan != null)
            {
                bol = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(R => R.FUNCTIONNAME == "CHECK_KPTYPE" && R.FUNCTIONTYPE == "PPID S/N" && SqlSugar.SqlFunc.Contains(R.VALUE, sn.SkuNo)).Any();
            }

            if (bol)
            {
                if (scanValue.Substring(0, 3) != "CN8" && scanValue.Length != 10)
                {
                    //throw new Exception(scan.VALUE + $@" 無效: 輸入內容與KP規則不匹配('{sn.SkuNo}'的 'PPID S/N' 的左邊三位必須是 CN8,而且長度必須是10)");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134306", new string[] { scan.VALUE, sn.SkuNo }));
                }
                else {
                   
                      if ((sn.SkuNo == "HP-5480-0000" || sn.SkuNo== "XHP-5480-0000") && scanValue.Substring(7, 1) != "A") {
                        //throw new Exception(scan.VALUE + $@" 無效: 輸入內容與KP規則不匹配('{sn.SkuNo}'的 'PPID S/N' 的倒數第四位必須是 A)");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134727", new string[] { scan.VALUE, sn.SkuNo,"A" }));
                    }
                    if ((sn.SkuNo == "HP-5480-0001" || sn.SkuNo == "XHP-5480-0001") && scanValue.Substring(7, 1) != "B")
                    {
                        //throw new Exception(scan.VALUE + $@" 無效: 輸入內容與KP規則不匹配('{sn.SkuNo}'的 'PPID S/N' 的倒數第四位必須是 B)");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134727", new string[] { scan.VALUE, sn.SkuNo, "B" }));
                    }
                    if ((sn.SkuNo == "HP-5480-0002" || sn.SkuNo == "XHP-5480-0002") && scanValue.Substring(7, 1) != "C")
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134727", new string[] { scan.VALUE, sn.SkuNo, "C" }));
                    }
                    if ((sn.SkuNo == "HP-5481-0001" || sn.SkuNo == "XHP-5481-0001") && scanValue.Substring(7, 1) != "D")
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134727", new string[] { scan.VALUE, sn.SkuNo, "D" }));
                    }
                    if ((sn.SkuNo == "HP-4024-0000" || sn.SkuNo == "XHP-4024-0000") && scanValue.Substring(7, 1) != "6")
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134727", new string[] { scan.VALUE, sn.SkuNo, "6" }));
                    }
                    if ((sn.SkuNo == "HP-4024-0001" || sn.SkuNo == "XHP-4024-0001") && scanValue.Substring(7, 1) != "7")
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134727", new string[] { scan.VALUE, sn.SkuNo, "7" }));
                    }
                    if ((sn.SkuNo == "HP-4024-0002" || sn.SkuNo == "XHP-4024-0002") && scanValue.Substring(7, 1) != "8")
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134727", new string[] { scan.VALUE, sn.SkuNo, "8" }));
                    }
                }
            }
        }
        /// <summary>
        /// SFP S/N
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sn"></param>
        /// <param name="scan"></param>
        /// <param name="scans"></param>
        /// <param name="API"></param>
        /// <param name="sfcdb"></param>
        /// <param name="apdb"></param>
        public static void SFPSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            bool bol = false;
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> kpList = t_r_sn_kp.GetKPRecordBySnID(sn.ID, sfcdb);
            R_SN_KP SFPScan = kpList.Find(k => k.PARTNO == scan.PARTNO && k.SCANTYPE == "SFP S/N");
            string scanValue = scan.VALUE;

            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {

                throw new Exception(scanValue + " has been link on other sn!");

            }

            if (SFPScan != null)
            {
                bol = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(R => R.FUNCTIONNAME == "CHECK_KPTYPE" && R.FUNCTIONTYPE == "SFP S/N" && SqlSugar.SqlFunc.Contains(R.VALUE, sn.SkuNo)).Any();
            }
            if (bol)
            {
                if (scan.PARTNO == "Attach")
                {
                    if (SFPScan.MPN != scanValue)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135308", new string[] { scanValue, SFPScan.MPN }));
                        //throw new Exception(scanValue + "輸入的內容與-->" + SFPScan.MPN + " 不匹配");
                    }
                    var result = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn.SerialNo && t.SCANTYPE == "CUS S/N").ToList().FirstOrDefault();
                    if (bol)
                    {
                        if (result.VALUE.Substring(24, 1) != (scanValue.Substring(3, 1)))
                        {
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135418", new string[] { result.VALUE.Substring(24, 1), scanValue.Substring(3, 1) }));
                            //throw new Exception(result.VALUE.Substring(24, 1) + "輸入的內容與KP規則不匹配-->" + scanValue.Substring(3, 1) + " SFP1 與 SFP2廠商需一致");
                        }
                    }
                }
            }
        }
        /// <summary>
        /// ST S/N
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sn"></param>
        /// <param name="scan"></param>
        /// <param name="scans"></param>
        /// <param name="API"></param>
        /// <param name="sfcdb"></param>
        /// <param name="apdb"></param>
        public static void STSNScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            bool bol = false;
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> kpList = t_r_sn_kp.GetKPRecordBySnID(sn.ID, sfcdb);
            R_SN_KP STScan = kpList.Find(k => k.PARTNO == scan.PARTNO && k.SCANTYPE == "ST S/N");
            string scanValue = scan.VALUE;

            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scanValue + " has been link on other sn!");
                //throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135418", new string[] { scanValue }));
            }

            if (STScan != null)
            {
                
                bol = sfcdb.ORM.Queryable<C_MCSN>().Where(R => R.TYPE=="SKUNO" && R.VALID_FLAG=="1" && R.SKUNO== sn.SkuNo).Any();
            }
            if (bol)
            {
                //throw new Exception(scanValue + "無效: 此機種未設置為需要Service Tag Code!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135700", new string[] { scanValue }));
            }
            else
            {
                C_MCSN sss = sfcdb.ORM.Queryable<C_MCSN>().Where(R => R.TYPE == "SN" && R.SN==sn.SerialNo && R.VALID_FLAG =="1").ToList().FirstOrDefault();
                if (sss == null)
                {
                    //throw new Exception(scanValue + "無效: 此產品未綁定Service Tag Code!");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135918", new string[] { scanValue }));
                }
                else
                {
                    if (sss.SERVICE.ToString().Equals(scanValue))
                    {
                        //throw new Exception(scanValue + "無效: 此產品綁定的Service Tag Code[" + sss.SERVICE.ToString() + "]與所輸入的不一致!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135950", new string[] { scanValue, sss.SERVICE.ToString() }));
                    }
                }
            }

        }
        /// <summary>
        /// Service Tag D/C
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sn"></param>
        /// <param name="scan"></param>
        /// <param name="scans"></param>
        /// <param name="API"></param>
        /// <param name="sfcdb"></param>
        /// <param name="apdb"></param>
        public static void ServiceTagScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            bool bol = true;
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> kpList = t_r_sn_kp.GetKPRecordBySnID(sn.ID, sfcdb);
            R_SN_KP ServiceTagScan = kpList.Find(k => k.PARTNO == scan.PARTNO && k.SCANTYPE == "SERVICE TAG D/C");
            string scanValue = scan.VALUE;

            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {

                throw new Exception(scanValue + " has been link on other sn!");

            }

            if (ServiceTagScan != null)
            {

                bol = sfcdb.ORM.Queryable<R_BRCD_EXSN>().Where(R => R.SN == sn.SerialNo && R.SERVICE_NO== scanValue).Any();
            }
            if (!bol)
            {
                //throw new Exception(scanValue + "TAG不存在，請確認!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140042", new string[] { scanValue}));
            }
        }
        /// <summary>
        /// FUJITSU P/N
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sn"></param>
        /// <param name="scan"></param>
        /// <param name="scans"></param>
        /// <param name="API"></param>
        /// <param name="sfcdb"></param>
        /// <param name="apdb"></param>
        public static void FUJITSUScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            string comparison = string.Empty;
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> kpList = t_r_sn_kp.GetKPRecordBySnID(sn.ID, sfcdb);
            R_SN_KP FUJITSUScan = kpList.Find(k => k.PARTNO == scan.PARTNO && k.SCANTYPE == "FUJITSU P/N");
            string scanValue = scan.VALUE;

            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {

                throw new Exception(scanValue + " has been link on other sn!");

            }

            if (sn.SkuNo.Length <= 8)
            {
                //throw new Exception("請PE/QE確認此機種 "+ sn.SkuNo + " 長度不超8位,卻設置了KP類型為 'FUJITSU P/N'");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140121", new string[] { sn.SkuNo }));
            }

            comparison = "CF00" + sn.SkuNo.Substring(sn.SkuNo.Length - 8, 8);

            if (FUJITSUScan != null)
            {
                if (scanValue.Equals(comparison))
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140220", new string[] { scanValue, sn.SkuNo.Substring(sn.SkuNo.Length - 8, 8) }));
                    //throw new Exception(scanValue + " 無效:FUJITSU LABEL內容應是:CF00" + sn.SkuNo.Substring(sn.SkuNo.Length - 8, 8));
                }
            }

        }
        /// <summary>
        /// SW KIT P/N
        /// </summary>
        /// <param name="config"></param>
        /// <param name="sn"></param>
        /// <param name="scan"></param>
        /// <param name="scans"></param>
        /// <param name="API"></param>
        /// <param name="sfcdb"></param>
        /// <param name="apdb"></param>
        public static void SWKITScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            bool bol = false;
            string[] strArr0 = { "ER-804-0010","ER-7000-0331","ER-5420-0002","ER-1000-0001","ER-7000-0340","ER-5431-0001",
                                "ER-5430-0001","ICX6450-24","ICX6450-24P","ER-7000-0001","ER-7000-0401","ICX6450-48P","ICX6400-EPS1500",
                                "ICX6430-24","ICX6430-48P","ICX6430-48","ICX6430-24P","ICX6430-24","ICX6450-48P","ICX6450-48",
                                "ER-7000-0411","BR-815-0000","DL-1860-2F00","BR-425-0000","BR-825-0000","BR-1860-2C01","BR-815-0000",
                                "BR-1860-2F01","BR-1860-1C01","BR-1860-1F01","BR-1860-1P01","BR-1860-2P01","BR-825-0000","BR-415-0000",
                                "BR-425-0000","BR-1010-0000","BR-1010-1000","BR-1010-0000-IP","BR-1010-1000-IP","BR-1020-0000","ICX-EPS4000-SHELF",
                                "BR-1020-1000","BR-1020-1000-IP","BR-1020-0000-IP","ER-7000-0407","RS-1860-2A01","SM-VDX2730-0000","XSM-VDX2730-0000",
                                "ER-M6505-0000","HP-415-A-1010","HP-425-A-1010","HP-815-A-1010","HP-825-A-1010","ER-7000-0370","IB-1860-1C00",
                                "IB-1860-2C00","XHP-804-A-0010","HP-804-0010","ER-7000-0139","ICX6400-C12-MGNT","ICX6400-C12-RMK","EM-ICX6430-24" };
            string[] strArr1 = { "ER-804-0010","ER-7000-0331","ER-5420-0002","ER-1000-0001","ER-7000-0340","ER-5431-0001",
                                "ER-5430-0001","ICX6450-24","ICX6450-24P","ER-7000-0001","ER-7000-0401","ICX6450-48P","ICX6400-EPS1500",
                                "ICX6430-24","ICX6430-48P","ICX6430-48","ICX6430-24P","ICX6430-24","ICX6450-48P","ICX6450-48",
                                "ER-7000-0411","BR-815-0000","DL-1860-2F00","BR-425-0000","BR-825-0000","BR-1860-2C01","BR-815-0000",
                                "BR-1860-2F01","BR-1860-1C01","BR-1860-1F01","BR-1860-1P01","BR-1860-2P01","BR-825-0000","BR-415-0000",
                                "BR-425-0000","BR-1010-0000","BR-1010-1000","BR-1010-0000-IP","BR-1010-1000-IP","BR-1020-0000","ICX-EPS4000-SHELF",
                                "BR-1020-1000","BR-1020-1000-IP","BR-1020-0000-IP","ER-7000-0407","RS-1860-2A01","SM-VDX2730-0000","XSM-VDX2730-0000",
                                "ER-M6505-0000","HP-415-A-1010","HP-425-A-1010","HP-815-A-1010","HP-825-A-1010","ER-7000-0370","IB-1860-1C00",
                                "IB-1860-2C00","XHP-804-A-0010","HP-804-0010","ER-7000-0139","ICX6400-C12-MGNT","ICX6400-C12-RMK","EM-ICX6430-24" };
            string comparison = string.Empty;
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> kpList = t_r_sn_kp.GetKPRecordBySnID(sn.ID, sfcdb);
            R_SN_KP SWKITSUScan = kpList.Find(k => k.PARTNO == scan.PARTNO && k.SCANTYPE == "SW KIT P/N");
            string scanValue = scan.VALUE;
            bool FIRMBol = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "NO_CHECK_FIRMWARE" && t.CATEGORY == "NO_CHECK_FIRMWARE" && t.VALUE == sn.SkuNo).Any();

            bool SOFTBol = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "NO_CHECK_SOFTWARE" && t.CATEGORY == "NO_CHECK_SOFTWARE" && t.VALUE == sn.SkuNo).Any();
            if (SWKITSUScan != null)
            {
                bol = sfcdb.ORM.Queryable<C_KP_List_Item, C_KP_List_Item_Detail>((CKL, CKLD) => CKL.ID == CKLD.ITEM_ID).Where((CKL, CKLD) => CKL.LIST_ID == sn.KP_LIST_ID && CKLD.SCANTYPE == "SW KIT P/N").Select((CKL, CKLD) => CKLD).Any();

                if (!bol)
                {
                    //throw new Exception(scanValue + "無效: 該" + sn.SkuNo + " 沒有設置  KP 類型為 SW KIT P/N, 請找負責該機種的PQE確認是否更改過KP類型");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140304", new string[] { scanValue, sn.SkuNo }));
                }
                else
                {

                    if (!(sn.RouteID.StartsWith("INTRUDER-IBM-CTO") || sn.RouteID.StartsWith("INTRUDER-HP-CTO") || sn.RouteID.StartsWith("TOMCAT-IBM-CTO") || sn.RouteID.StartsWith("GLADIUS-CTO") || sn.RouteID.StartsWith("WANCHESE-CTO")
                        || sn.RouteID.StartsWith("ASTRA-CTO") || sn.RouteID.StartsWith("LIGHTNING-CTO") || sn.RouteID.StartsWith("TOMCAT-CTO-1") || sn.RouteID.StartsWith("KATANA-CTO")
                        || sn.RouteID.StartsWith("PROWLER-CTO") || sn.RouteID.StartsWith("CALLISTO24-F-CTO") || sn.RouteID.StartsWith("CALLISTO60-F-CTO") || sn.RouteID.StartsWith("PHAROS-MB-CTO")
                        || sn.RouteID.StartsWith("CHINOOK2-CTO") || sn.RouteID.StartsWith("CHOW-CTO-FOX")) && !(sn.RouteID.StartsWith("FCX") && (sn.SkuNo.StartsWith("FCX") || sn.SkuNo.StartsWith("RPS"))) && sn.SkuNo != "ICX6450-C12-PD")
                    {
                        if (!((System.Collections.IList)strArr0).Contains(sn.SkuNo) && !sn.SkuNo.StartsWith("ICX7250") && !sn.RouteID.StartsWith("HARRIER-") && !FIRMBol)
                        {
                            bol = sfcdb.ORM.Queryable<C_KP_List_Item, C_KP_List_Item_Detail>((CKL, CKLD) => CKL.ID == CKLD.ITEM_ID).Where((CKL, CKLD) => CKL.LIST_ID == sn.KP_LIST_ID && CKLD.SCANTYPE == "FIRMWARE P/N" && CKL.KP_PARTNO == scan.PARTNO).Select((CKL, CKLD) => CKLD).Any();

                            if (!bol)
                            {
                                //throw new Exception(scanValue + "無效: KP料號 " + scan.PARTNO + " 的License資料缺失(FIRMWARE), 請聯繫PQE處理, 謝謝");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140411", new string[] { scanValue, scan.PARTNO }));
                            }
                        }
                    }
                    if (!(sn.RouteID.StartsWith("FWS") && sn.SkuNo.StartsWith("FWS")) && !((System.Collections.IList)strArr1).Contains(sn.SkuNo) && !sn.SkuNo.StartsWith("ICX7250") && !SOFTBol)
                    {
                        bol = sfcdb.ORM.Queryable<C_KP_List_Item, C_KP_List_Item_Detail>((CKL, CKLD) => CKL.ID == CKLD.ITEM_ID).Where((CKL, CKLD) => CKL.LIST_ID == sn.KP_LIST_ID && CKLD.SCANTYPE == "SOFTWARE P/N" && CKL.KP_PARTNO == scan.PARTNO).Select((CKL, CKLD) => CKLD).Any();

                        if (!bol)
                        {
                            //throw new Exception(scanValue + "無效: KP料號 " + scan.PARTNO + " 的License資料缺失(SOFTWARE), 請聯繫PQE處理, 謝謝");
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140452", new string[] { scanValue, scan.PARTNO }));
                        }
                    }
                }
            }

        }

        ///// <summary>
        ///// HWT FI掃描KP卡關 場別：NHEZ,工站：FI
        ///// ADD BY HGB 2019.08.02
        ///// </summary>
        ///// <param name="config"></param>
        ///// <param name="sn"></param>
        ///// <param name="scan"></param>
        ///// <param name="scans"></param>
        ///// <param name="API"></param>
        ///// <param name="sfcdb"></param>
        //public void HwtFiKpChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        //{

        //    T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
        //    T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //    //BEGIN 1.日本合同
        //    T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //    if (t_c_control.ValueIsExist("TCJPSNSCAN", sn.WorkorderNo, sfcdb))
        //    {


        //        //0.檢查主SN
        //        t_r_sn_kp.TCJPSNSCAN(sn.baseSN.SN, "主板", sfcdb);

        //        //1.檢查1階SN
        //        List<R_SN_KP> kpson1 = t_r_sn_kp.GetKPListBYSN(sn.baseSN.SN, 1, sfcdb);
        //        foreach (R_SN_KP kp1 in kpson1)
        //        {
        //            if (kp1.VALUE != null)
        //            {
        //                if (kp1.VALUE.Contains("DM"))
        //                {
        //                    t_r_sn_kp.TCJPSNSCAN(kp1.VALUE, "二階", sfcdb);

        //                    //2.檢查2階SN
        //                    List<R_SN_KP> kpson2 = t_r_sn_kp.GetKPListBYSN(kp1.VALUE, 1, sfcdb);
        //                    foreach (R_SN_KP kp2 in kpson2)
        //                    {
        //                        if (kp1.VALUE != null)
        //                        {

        //                            if (kp2.VALUE.Contains("DM"))
        //                            {
        //                                t_r_sn_kp.TCJPSNSCAN(kp2.VALUE, "三階", sfcdb);

        //                                //3.檢查.階SN
        //                                List<R_SN_KP> kpson3 = t_r_sn_kp.GetKPListBYSN(kp2.VALUE, 1, sfcdb);
        //                                foreach (R_SN_KP kp3 in kpson3)
        //                                {
        //                                    if (kp1.VALUE != null)
        //                                    {
        //                                        if (kp3.VALUE.Contains("DM"))
        //                                        {
        //                                            t_r_sn_kp.TCJPSNSCAN(kp3.VALUE, "四階", sfcdb);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    //END 1.日本合同


        //    //2.華為鎖定    
        //    t_r_sn_kp.CheckLockByHuawei(sn.baseSN.SN, sn.BoxSN, sfcdb);
        //    List<R_SN_KP> kpson4 = t_r_sn_kp.GetKPListBYSN(sn.baseSN.SN, 1, sfcdb);
        //    foreach (R_SN_KP kp4 in kpson4)
        //    {
        //        if (kp4.VALUE != null)
        //        {
        //            t_r_sn_kp.CheckLockByHuawei(kp4.VALUE, "", sfcdb);
        //        }
        //    }

        //    //3.綁定下階MAC檢查(1階，2階，3階)//目前只有到2階，以後如果有三階，在加個管控類型      
        //    if (scan.PARTNO == "MACSON")
        //    {
        //        if (t_c_control.ValueIsExist("TC0005", sn.SkuNo, sfcdb))
        //        {
        //            string kp_name = t_c_control.GetControlList("TC0005", sn.SkuNo, sfcdb)[0].CONTROL_LEVEL;

        //            string secondlinkkp = t_r_sn_kp.LoadDataBySnAndKpName(sn.baseSN.SN, kp_name, sfcdb).VALUE;

        //            if (t_r_sn_kp.CheckHwtMacLinkByValue(scan.VALUE, sfcdb))
        //            {
        //                string nowlinkkp = t_r_sn_kp.GetKPRecordByValueHwtFI(scan.VALUE, sfcdb)[0].SN;
        //                if (secondlinkkp != nowlinkkp)
        //                {
        //                    throw new Exception($@"此MAC已經和其它扣板進行綁定,請確認!{sn.baseSN.SN},{secondlinkkp},{scan.VALUE}");
        //                }
        //            }

        //            if (!t_r_sn_kp.CheckMacByValueHwtFI(scan.VALUE, secondlinkkp, sfcdb))
        //            {
        //                throw new Exception($@"此MAC沒有與主板中的扣板進行綁定!{sn.baseSN.SN},{secondlinkkp},{scan.VALUE}");
        //            }
        //        }
        //    }

        //    else
        //    {
        //        //HWT綁定KP檢查，逆向替換檢查
        //        t_r_sn_kp.KpLinkcheck(sn.baseSN.SN, scan.VALUE, scan.KP_NAME, sfcdb);

        //    }


        //}

        ///// <summary>
        ///// HWT ASSY,ASSYP,ASSY2掃描KP卡關 場別：NHEZ,工站：HWT ASSY,ASSYP,ASSY2
        ///// ADD BY HGB 2019.08.16
        ///// </summary>
        ///// <param name="config"></param>
        ///// <param name="sn"></param>
        ///// <param name="scan"></param>
        ///// <param name="scans"></param>
        ///// <param name="API"></param>
        ///// <param name="sfcdb"></param>
        //public void HwtAssyKpChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        //{

        //    T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
        //    T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //    T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //    T_R_SKUNO_ADDRESS t_r_sku_address = new T_R_SKUNO_ADDRESS(sfcdb, DB_TYPE_ENUM.Oracle);
        //    T_R_SN t_r_sn = new T_R_SN(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //    T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //    T_C_ITEMCODE_MAPPING_EMS t_c_itemcode_mapping_ems = new T_C_ITEMCODE_MAPPING_EMS(sfcdb, DB_TYPE_ENUM.Oracle);
        //    T_R_WO_TYPE WOType = new T_R_WO_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);
        //    T_R_RELATION_DATA t_r_relation_data = new T_R_RELATION_DATA(sfcdb, DB_TYPE_ENUM.Oracle);
        //    R_SN r_sn = new R_SN();
        //    string kpsnnew = string.Empty;//掃如的二維碼，未轉化
        //    string kpwo = string.Empty;
        //    if (t_r_sn.CheckExists(scan.VALUE, sfcdb))//KP存在于主表
        //    {
        //        r_sn = r_sn = t_r_sn.LoadData(scan.VALUE, sfcdb);
        //        kpwo = r_sn.WORKORDERNO;
        //    }

        //    #region 卡迅達機種ASSY2綁定標籤上的SN，request by 黃仲略 2018.08.27
        //    if (scan.STATION == "ASSY2")
        //    {
        //        if (sn.baseSN.SN.Substring(2, 8) == scan.VALUE.Substring(0, 8))//KP與主SN料號一樣
        //        {
        //            if (sn.baseSN.SN.Substring(2, sn.baseSN.SN.Length - 2) != scan.VALUE)//主SN去出前兩位后的SN不等於掃描的KP
        //            {
        //                throw new Exception($@"迅達機種KP規則不匹配,KP必須和去掉前兩位的主SN 一樣!主SN:{sn.baseSN.SN},KP:{scan.VALUE}");
        //            }
        //        }
        //    }

        //    #endregion

        //    #region 新規則條碼 請掃描二維碼
        //    kpsnnew = scan.VALUE;
        //    bool var_new_rule;
        //    if (scan.VALUE.Substring(0, 3) == "[)>")
        //    {
        //        var_new_rule = true;
        //    }
        //    else if (scan.VALUE.Substring(0, 2) == "DM")
        //    {
        //        if (t_c_control.ValueIsExist("TC_NEW_2D", sn.SkuNo, sfcdb))
        //        {
        //            var_new_rule = true;
        //        }
        //        else
        //        {
        //            throw new Exception($@"錯誤，新規則條碼 請掃描二維碼,KP:{scan.VALUE}");
        //        }
        //    }
        //    else
        //    {
        //        var_new_rule = false;
        //    }
        //    #endregion

        //    #region 獲取實際條碼
        //    DataInputLoader dtl = new DataInputLoader();
        //    scan.VALUE = dtl.GetScanSn(scan.VALUE);
        //    #endregion

        //    #region 如果掃描的KEYPART是有拉手條的,其條碼要獲取其拉手條條碼 
        //    if (t_r_relation_data.IsExist(r_sn.SN, "PCB S/N", sfcdb))
        //    {
        //        kpsnnew = t_r_relation_data.LoadData(r_sn.SN, "PCB S/N", sfcdb).PARENT;
        //    }
        //    #endregion

        //    #region 掃描的SN料號與PE 配置的不匹配，KP規則配置帶料號的就好，不寫了

        //    #endregion

        //    #region 檢查KP是否被鎖
        //    T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //    if (t_r_sn_lock.IsLock("", "", scan.VALUE, "", scan.STATION, "", sfcdb))
        //    {
        //        string lockreson = t_r_sn_lock.GetDetailBySN(sfcdb, scan.VALUE,  scan.STATION).LOCK_REASON;
        //        throw new Exception($@"該Keypart條碼已被鎖，不能被綁定，請聯繫PQE解鎖,鎖定原因{lockreson},SN:{scan.VALUE}");

        //    }
        //    #endregion

        //    #region 預鎖定檢查
        //    T_R_PLAN_LOCK plan_lock = new T_R_PLAN_LOCK(sfcdb, DB_TYPE_ENUM.Oracle);
        //    plan_lock.IsPlanLock(scan.VALUE, scan.VALUE, "SN", "b", sfcdb);
        //    #endregion

        //    #region 除拉手條与PCB綁定外,如果產品不在Jobfinish 不可以綁定
        //    string kpboxsn = t_c_itemcode_mapping_ems.Get_Customer_Partno("CONVERT_BOXSN", scan.VALUE, sfcdb);
        //    string kppcbsn = t_c_itemcode_mapping_ems.Get_Customer_Partno("CONVERT_PCBSN", scan.VALUE, sfcdb);
        //    if ((sn.baseSN.SN != kppcbsn && sn.baseSN.SN != kpboxsn)
        //        && sn.baseSN.SN.Substring(0, sn.baseSN.SN.Length - 6) != scan.VALUE.Substring(0, scan.VALUE.Length - 6) && var_new_rule == false)
        //    {
        //        if (t_r_sn.CheckExists(scan.VALUE, sfcdb))//KP存在于主表
        //        {
        //            if (r_sn.NEXT_STATION != "JOBFINISH")
        //            {
        //                throw new Exception($@"此KeyPart還沒有JOBFINISH﹐不允許綁定3,SN:{sn.baseSN.SN},KPSN:{scan.VALUE}");
        //            }
        //        }
        //    }
        //    #endregion

        //    #region 如果是拉手條，則必須與SN對應
        //    if (scan.KP_NAME == "PCB S/N")
        //    {
        //        if (var_new_rule)
        //        {
        //            string snboxsn = t_c_itemcode_mapping_ems.Get_Customer_Partno("CONVERT_BOXSN", sn.baseSN.SN, sfcdb);
        //            if (snboxsn != kpboxsn)
        //            {
        //                throw new Exception($@"掃描的拉手條條碼不正確，請確認!,SN:{sn.baseSN.SN},KPSN:{scan.VALUE},SN:{snboxsn},KPSN:{kpboxsn}");
        //            }
        //        }
        //        else
        //        {
        //            //只卡正常工單
        //            if (!WOType.IsTypeInput("RMA", sn.WorkorderNo.Substring(0, 6), sfcdb))
        //            {
        //                if (!(t_c_control.ValueIsExist("TC0010", sn.SkuNo, sfcdb) && scan.VALUE.Contains("KHW")))
        //                {
        //                    string snr5 = sn.baseSN.SN.Substring(sn.baseSN.SN.Length - 5, 5);
        //                    string kpsnr5 = scan.VALUE.Substring(scan.VALUE.Length - 5, 5);

        //                    string snl6 = sn.baseSN.SN.Substring(0, 6);
        //                    string kpsnl6 = scan.VALUE.Substring(0, 6);

        //                    string snl10 = sn.baseSN.SN.Substring(0, 10);
        //                    string kpsnl10 = scan.VALUE.Substring(0, 10);

        //                    if (snr5 != kpsnr5 || (snl6 == kpsnl6 && snl10 != kpsnl10))
        //                    {
        //                        throw new Exception($@"KEYPART:{scan.VALUE} 與SN不匹配1,SN:{sn.baseSN.SN},KPSN:{scan.VALUE}");
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    #endregion

        //    #region SN存在于主表R_SN中
        //    #region Control run AND 綁定工單 
        //    /// 檢查ASSY,assyp,特殊工單綁定管控
        //    /// 子工單(CONTROL_VALUE)只能和父工單(CONTROL_LEVEL)綁定
        //    /// SPECIFY_WO

        //    if (t_r_sn.CheckExists(scan.VALUE, sfcdb))//KP存在于主表
        //    {

        //        if (!(r_sn.COMPLETED_FLAG == "1" && r_sn.CURRENT_STATION != "MRB" && r_sn.NEXT_STATION != "REWORK"))
        //        {
        //            // throw new Exception($@"INVALID OR NOT COMPLETED OR IN[MRB]!SN:{sn.baseSN.SN},KPSN:{scan.VALUE}");
        //            throw new Exception($@"KP sn無效或未完工，或已入MRB!SN:{sn.baseSN.SN},KPSN:{scan.VALUE}");
        //        }
        //        if (!t_r_wo_base.IsExist(r_sn.WORKORDERNO, sfcdb))
        //        {
        //            throw new Exception($@"KP的工單不存在R_WO_BASE!SN:{sn.baseSN.SN},KPSN:{scan.VALUE},KPWO{r_sn.WORKORDERNO}");
        //        }
        //        R_WO_BASE r_wo_base = t_r_wo_base.GetWoByWoNo(r_sn.WORKORDERNO, sfcdb);
        //        t_c_control.CheckSpecifyWo(sn.WorkorderNo, kpwo , sfcdb);
               
        //        #endregion

        //        #region 華為購買回來的RMA品不掃SHIPPING也可以和正常工單綁定
        //        //應NPI周承劍,PE金雨生要求: 增加從華為購買回來的RMA品不掃SHIPPING也可以和正常工單綁定-- - 廖東林-- - 20181019--begin


        //        if (WOType.IsTypeInput("RMA", kpwo.Substring(0, 6), sfcdb) && WOType.GetWOTypeByPREFIX("REGULAR", sn.WorkorderNo.Substring(0, 6), sfcdb))
        //        {
        //            //(正常工單可綁定RMA工單)此SN是從華為購買回來,當KEYPART綁定到整機出貨
        //            if (!t_c_control.ValueIsExist("TC0035", scan.VALUE, sfcdb))
        //            {
        //                if (!(r_sn.SHIPPED_FLAG == "1" && r_sn.SHIPDATE.ToString().Length > 0))
        //                {
        //                    throw new Exception($@"RMA{kpwo}未掃SHIPPING,不能被正常工單{sn.WorkorderNo}綁定!SN:{sn.baseSN.SN},KPSN:{scan.VALUE},KPWO{r_sn.WORKORDERNO}");
        //                }
        //            }

        //        }

        //        #endregion

        //        #region 檢查SN綁定的MAC與在線分配的MAC是否
        //        if (scan.KP_NAME.Contains("MAC") && scan.KP_NAME != "MAC16 S/N")
        //        {
        //            T_C_MACPRINT_CONFIG t_c_macprint_config = new T_C_MACPRINT_CONFIG(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //            if (t_c_macprint_config.CheckExists(sn.SkuNo, sfcdb))
        //            {
        //                T_R_SN_MAC t_sn_mac = new T_R_SN_MAC(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //                if (!t_sn_mac.CheckExists(sn.baseSN.SN,"MAC", scan.VALUE, sfcdb))
        //                {
        //                    throw new Exception($@" 此SN綁定的MAC與在線分配的MAC不一致!SN:{sn.baseSN.SN},KPSN:{scan.VALUE}");
        //                }
        //            }

        //        }


        //        #endregion

        //        #region 二次電源已替換過被鎖不能再次替換
        //        #endregion

        //        #region 委託收貨機種的SN,必須有出貨記錄才可以綁定

        //        T_R_SHIP_DETAIL t_r_ship_detail = new T_R_SHIP_DETAIL(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
        //        var kpskuno = string.Empty;
        //        try
        //        {
        //            kpskuno = t_r_sn.LoadData(scan.VALUE, sfcdb).SKUNO;
        //        }
        //        catch
        //        { }

        //        //是委託收貨機種
        //        if (t_r_sku_address.IsExists(kpskuno, "CS0001", sfcdb))
        //        {
        //            if (!t_r_ship_detail.IsExists(scan.VALUE, sfcdb))
        //            {
        //                throw new Exception($@"此條碼{scan.VALUE}屬於委託收貨,必須先掃SHIPPING出貨!");
        //            }
        //        }
        //        else
        //        {
        //            if (t_r_ship_detail.IsExists(scan.VALUE, sfcdb))
        //            {
        //                throw new Exception($@"此條碼 {scan.VALUE}已掃SHIPPING出貨!不能綁定");
        //            }
        //        }
        //        #endregion
        //    }
        //    #endregion
        //}
    
        public void DCBarcodeScanRuleChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            //QE  賴斌要求，第一位為年，第二三位為周，第四位為充電次數（0或1）
            // DcBarcode sample 0130 -> 0為2020年 13為第13周 0為充電次數碼為0

            //2.2： 掃描電池DC barcode，當綁定KP的日周別 - DC 週別 <= 1（週），電池DC barcode可以正常綁定KP；同時在線生成一張同樣的DC label barcode
            //2.3： 掃描電池DC barcode，當綁定KP的日周別 - DC 週別 > 1（週），電池DC barcode無法正常綁定KP；同時在KP掃描界面彈出“電池DC過期”報錯信息
            //2.4:  管控電池DC barcode第4碼必須《= 1（T = 0 / 1可以接受掃描進KP; T >= 2就拒收不能掃描進KP)
            
            //nchen 2020-04-01
            string DcBarcode = scan.VALUE;
            if(DcBarcode.Length != 4) //長度需為4位
            {
                throw new Exception("SN:" + DcBarcode + " Length is not 4!");
            }
            if (Convert.ToInt32(DcBarcode.Substring(3, 1)) >6) //最後一位需<=6
            {
                throw new Exception("SN:" + DcBarcode + " Last One is less than or equal to 6 !");
            }
            
            string sql = @"select to_char(sysdate,'YYYY') year from dual";
            string dcSql = @"select * from c_control where CONTROL_NAME = 'CheckDCBarcodeDate' and CONTROL_VALUE ='DCBarcode'";
            DataTable dt = sfcdb.RunSelect(sql).Tables[0];
            DataTable dtDc = sfcdb.RunSelect(dcSql).Tables[0];
            int sysdateYear = int.Parse(dt.Rows[0]["year"].ToString()); //系統數據庫年（YYYY）
            int setDC = int.Parse(dtDc.Rows[0]["CONTROL_LEVEL"].ToString());
            sql = @"select ceil((trunc(sysdate) -
                            TO_DATE(to_char(sysdate, 'YYYY') || '0101', 'YYYYMMDD') +
                            TO_CHAR(TO_DATE(to_char(sysdate, 'YYYY') || '0101', 'YYYYMMDD'),
                                        'd')) / 7) ww
                    from dual";
            dt = sfcdb.RunSelect(sql).Tables[0];
            int sysdateWW = int.Parse(dt.Rows[0]["ww"].ToString()); //系統數據庫周（WW）

            List<C_CODE_MAPPING> CodeMapping = null;
            T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(sfcdb, DB_TYPE_ENUM.Oracle);
            CodeMapping = TCCM.GetDataByName("1YEAR_1", sfcdb);
            if (CodeMapping == null)
            {
                throw new Exception("SN:" + DcBarcode + " Not found C_CODE_MAPPING.codetype ='1YEAR_1'");
            }
            C_CODE_MAPPING TAG = CodeMapping.Find(T => T.CODEVALUE == DcBarcode.Substring(0, 1));
            if (TAG == null)
            {
                throw new Exception("SN:" + DcBarcode + " Year error");
            }
            int dcYear = int.Parse(TAG.VALUE); //傳第一位按配置查出用戶需要的年（YYYY）
            int dcWW = int.Parse(DcBarcode.Substring(1, 2)); //掃描進來的周（WW）
            if (sysdateYear == dcYear) //同一年處理方式
            {
                if (sysdateWW - dcWW > setDC) //系統周比轉入周大於setDC以上說明DC過期
                {
                    throw new Exception("SN:" + DcBarcode + " Sysdate(WW):" + sysdateWW + " DC has expired. -1");
                }
                if (dcWW - sysdateWW >= setDC) //轉入周比系統周大說明DC有異常
                {
                    throw new Exception("SN:" + DcBarcode + " Sysdate(WW):" + sysdateWW + " DC out of range. -1");
                }
            }
            else if(sysdateYear - dcYear == 1) //上下年處理方式
            {
                sql = @"select ceil((trunc(sysdate) - to_date('"+ dcYear + @"' || '0101', 'YYYYMMDD') +
                                TO_CHAR(to_date('" + dcYear + @"' || '0101', 'YYYYMMDD'), 'd') +
                                TO_CHAR(TO_DATE(to_char(sysdate, 'YYYY') || '0101', 'YYYYMMDD'),
                                         'd')) / 7) ww
                      from dual";
                dt = sfcdb.RunSelect(sql).Tables[0];
                sysdateWW = int.Parse(dt.Rows[0]["ww"].ToString());
                if (sysdateWW - dcWW > setDC) //系統周比轉入周大於setDC以上說明DC過期
                {
                    throw new Exception("SN:" + DcBarcode + " Sysdate(WW):" + sysdateWW + " DC has expired -2");
                }
                if (dcWW > 53) //轉入周大於53周說明DC有異常
                {
                    throw new Exception("SN:" + DcBarcode + " Sysdate(WW):" + sysdateWW + " DC out of range. -2");
                }
            }
            else
            {
                throw new Exception("SN:" + DcBarcode + " Sysdate(WW):" + sysdateWW + " DC has expired -3");
            }
        }

        public void DCNormChecker(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            //QE  林寧要求，檢查外箱電池與綁定的電池是否一致
            string dcValue = scan.VALUE;
            var s  = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn.SerialNo && t.VALID_FLAG == 1 && t.SCANTYPE == "DCBarcode" && t.VALUE !=null).ToList();
            var b = s.FindAll(t=>t.SCANTYPE== "DCBarcode" && t.VALUE==dcValue && t.VALID_FLAG == 1);
            if (s.Count!=b.Count) {
                //throw new Exception("SN:" + sn.SerialNo + " 綁定的KP與掃描的： " + dcValue + " 不一致，請確認");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140529", new string[] { sn.SerialNo, dcValue }));
            }
        }


        public static void ScanAllpartTrSn(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb , OleExec apdb)
        {
            //var waitdoKps = sfcdb.ORM.Queryable<R_SN_KP>().Where(t =>
            //    t.R_SN_ID == sn.ID && t.PARTNO == scan.PARTNO && t.STATION == scan.STATION && t.MPN == scan.MPN &&
            //    t.VALID_FLAG == 1).ToList();
            var aptrsn = apdb.ORM.Queryable<R_TR_SN_WIP, R_TR_SN>((rtrwip, rts) => rtrwip.TR_SN == rts.TR_SN)
                .Where((rtrwip, rts) => rts.TR_SN == scan.VALUE).Select((rtrwip, rts) => new { rtrwip, rts }).ToList().FirstOrDefault();
            //Allpart不夠扣則報錯
            //if(aptrsn==null || waitdoKps.Count > aptrsn.rts.EXT_QTY)
            //    throw new Exception($@"Allpart SN Qty is less then kp qty {waitdoKps.Count},pls check!");
            ///寫入KP
            //foreach (var item in waitdoKps)
            //{
            //    item.VALUE = $@"{aptrsn.rts.DATE_CODE}/{aptrsn.rts.LOT_CODE}" ;
            //    item.EXKEY1 = "APSN";
            //    item.EXVALUE1 = scan.VALUE;
            //    item.EDIT_TIME = DateTime.Now;
            //    item.EDIT_EMP = scan.EDIT_EMP;
            //}

            if (aptrsn == null || aptrsn.rts.EXT_QTY <= 0)
            {
                throw new Exception($@"Allpart SN not checkout or Qty is 0");
            }

            //臨時加做完拿掉歷史遺留問題
            #region
            var tempsql = $@"select * from 
                (
                select VALUE as wo From R_FUNCTION_CONTROL WHERE CATEGORY='KP_WO'
                )aa where aa.wo = '{sn.WorkorderNo}'";
            DataTable tempdt = sfcdb.ORM.Ado.GetDataTable(tempsql);
            #endregion

            //if (!aptrsn.rtrwip.WO.Equals(sn.WorkorderNo) && tempdt.Rows.Count == 0)
            //{
            //    throw new Exception($"{scan.VALUE} checkout workorder[{aptrsn.rtrwip.WO}] don't equals the workoder[{sn.WorkorderNo}] of {sn.SerialNo} ");
            //}
            R_SN_KP kpObj = scan.GetDataObject();

            if (!kpObj.PARTNO.Equals(aptrsn.rts.CUST_KP_NO))
            {
                throw new Exception($@"Keypart partno {kpObj.PARTNO} not match allpart cust_kp_no {aptrsn.rts.CUST_KP_NO},please call QE to check!");
            }
            DataTable dt = sfcdb.ORM.Ado.GetDataTable("select *from all_tables where table_name='R_WO_LINK' and owner='SFCRUNTIME'");
            bool bWOMPN = false;            
            if (dt.Rows.Count > 0)
            {
                C_WO_MPN woMPN = sfcdb.ORM.Queryable<C_WO_MPN, R_SN>((w, s) => w.WO == s.WORKORDERNO)
                    .Where((w, s) => s.SN == kpObj.SN && s.VALID_FLAG == "1" && w.PARTNO == kpObj.PARTNO)
                    .Select((w, s) => w).ToList().FirstOrDefault();
                if (woMPN != null)
                {
                    bWOMPN = true;
                    if (!woMPN.MPN.Equals(aptrsn.rts.MFR_KP_NO))
                    {
                        throw new Exception($@" allpart mfr_kp_no {aptrsn.rts.MFR_KP_NO} not match WO MPN {woMPN.MPN} ,please call QE to check!");
                    }
                }
            }
            
            if (!kpObj.MPN.Equals(aptrsn.rts.MFR_KP_NO) && !bWOMPN)
            {
                throw new Exception($@"Keypart MPN {kpObj.MPN} not match allpart mfr_kp_no {aptrsn.rts.MFR_KP_NO},please call QE to check!");
            }
            kpObj.VALUE = $@"{aptrsn.rts.DATE_CODE}/{aptrsn.rts.LOT_CODE}";
            kpObj.MPN = aptrsn.rts.MFR_KP_NO;
            kpObj.EXKEY1 = "APSN";
            kpObj.EXVALUE1 = scan.VALUE;
            kpObj.EDIT_TIME = DateTime.Now;
            kpObj.EDIT_EMP = scan.EDIT_EMP;
            ///Allpart扣料
            //aptrsn.rts.EXT_QTY = aptrsn.rts.EXT_QTY - waitdoKps.Count;
            //aptrsn.rtrwip.EXT_QTY = aptrsn.rts.EXT_QTY;
            aptrsn.rts.EXT_QTY = aptrsn.rts.EXT_QTY - 1;
            aptrsn.rtrwip.EXT_QTY = aptrsn.rts.EXT_QTY;
            if (aptrsn.rtrwip.EXT_QTY == 0)
            {
                aptrsn.rts.WORK_FLAG = "1";
                apdb.ORM.Deleteable(aptrsn.rtrwip).ExecuteCommand();
            }
            else
                apdb.ORM.Updateable(aptrsn.rtrwip).ExecuteCommand();
            var ii=apdb.ORM.Updateable(aptrsn.rts).ExecuteCommand();
            //sfcdb.ORM.Updateable(waitdoKps).ExecuteCommand();
            sfcdb.ORM.Updateable(kpObj).ExecuteCommand();
        }

        public static void FAN_SN_LABEL_CHECK(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);            
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }
            string sql = $@"SELECT*FROM r_sn_kp WHERE sn IN(SELECT value FROM r_sn_kp WHERE sn='{sn.SerialNo}' and valid_flag='1') 
                            and valid_flag='1' AND value='{scan.VALUE}' AND partno in (
                            select value from r_function_control where functionname='PSU_SN_LABEL_CHECK' and controlflag='Y' and functiontype='NOSYSTEM'
                            and category='PARTNO') ";
            DataTable dt = sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count == 0)
            {
                //throw new Exception("80的風扇SN與60的風扇SN不一致，請確認!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140749"));
            }
        }

        public static void IUID_SN_CHECK(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }
            if (sn.SerialNo.IndexOf(scan.VALUE) == -1)
            {
                //throw new Exception("KP錯誤: 后綴與SSN不一致!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140856"));
            }
        }

        public static void PACKOUT_LBL_SN_CHECK(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            //待確定邏輯
        }

        public static void PSU_SN_LABEL_CHECK(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            if (t_r_sn_kp.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }            
            string sql = $@"SELECT*FROM r_sn_kp WHERE sn IN(SELECT value FROM r_sn_kp WHERE sn='{sn.SerialNo}' and valid_flag='1') 
                            and valid_flag='1' AND value='{scan.VALUE}' AND partno in (
                            select value from r_function_control where functionname='PSU_SN_LABEL_CHECK' and controlflag='Y' and functiontype='NOSYSTEM'
                            and category='PARTNO') ";
            DataTable dt = sfcdb.ExecuteDataTable(sql, CommandType.Text, null);
            if (dt.Rows.Count == 0)
            {
                //throw new Exception("80的風扇SN與60的風扇SN不一致，請確認!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140749"));
            }
        }

        public static void AccDcAndDcDc(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            if (scan.VALUE.Length!=4) 
            {
                throw new Exception("The length of ACC D/C or DC D/C must be 4.");
            }
        }

        public static void AccSn_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            if (scan.VALUE.Length != 11)
            {
                throw new Exception("The length of ACC S/N must be 11.");
            }

            if (scan.VALUE.Substring(0,2) !="PY")
            {
                throw new Exception("ACC S/N must start with \"PY\".");
            }
        }

        public static void TEMPSN1_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            if (sn.SkuNo.Contains("HP"))
            { 
                if (scan.VALUE.Length != 11)
                {
                    throw new Exception("The SN length of TEMP S/N1 type of HP sku should be 11.");
                }

                if (scan.VALUE.Substring(0, 4) != "SCN8")
                {
                    throw new Exception("The TEMP S/N1 type of HP sku must start with \"SCN8\".");
                }
            }
        }

        public static void CUSSN1_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            if (!(scan.VALUE.Substring(0, 3) == "S3A" && scan.VALUE.Length == 13)
                || (scan.VALUE.Substring(0, 2) == "3A" && scan.VALUE.Length ==12)
                || (scan.VALUE.Substring(0, 2) == "CN" && scan.VALUE.Length == 10))
            {
               //throw new Exception("CUS S/N1類型KP,以S3A開頭長度13,或以3A開頭長度12,或以CN開頭長度10!");
               throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140915"));
            }
        }
        public static void CONDC_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            if (!(scan.VALUE.Substring(0, 1) != "0" && scan.VALUE.Length != 5))
            {
                //throw new Exception("無效: 輸入內容與KP規則不匹配.(category: 'CON D/C',必須以0開頭,且長度為5.)");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141112"));
            }
        }

        public static void MACLBLSN_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            if (!(scan.VALUE.Length != 12))
            {
                //throw new Exception("輸入的MAC SN的長度應該為12位7!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141121"));
            }
        }

        public static void CUSSN_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            if ((scan.VALUE.Length == 4 && scan.VALUE.Length == 13) || scan.VALUE == scan.PARTNO)
            {
                //throw new Exception("無效: 輸入內容與KP規則不匹配.(category: CUS S/N,長度不能為4或13位﹐且不能為本身料號.)");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141140"));
            }
        }

        public static void FANDC_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            T_WWN_DATASHARING TWD = new T_WWN_DATASHARING(sfcdb, DB_TYPE_ENUM.Oracle);
            WWN_DATASHARING _wwn = new WWN_DATASHARING();
            _wwn = TWD.GetWsnSku(scan.VALUE, sfcdb);
            T_R_SN t_r_sn = new T_R_SN(sfcdb, DB_TYPE_ENUM.Oracle);
            R_SN r_sn = new R_SN();
            r_sn = t_r_sn.LoadSN(scan.VALUE, sfcdb);

            if (r_sn.SKUNO.Length > 0 && r_sn.SKUNO != scan.PARTNO)
            {
                //throw new Exception("無效: 該產品料號['" + _wwn.SKU + "']與wwn_datasharing表['" + r_sn.SKUNO + "']不一致. [wwn_datasharing]");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141158", new string[] { _wwn.SKU, r_sn.SKUNO }));
            }
        }
        public static void PCBASN_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            LogicObject.SN kpsn = new LogicObject.SN(scan.VALUE, sfcdb, DB_TYPE_ENUM.Oracle);
            T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
            if (kpsn.CompletedFlag != "1")
            {
                throw new Exception($@"{kpsn.SerialNo} not finish !");
            }
            if (kpsn.ShippedFlag == "1")
            {
                throw new Exception($@"{kpsn.SerialNo} has being Shipped!");
            }
            if (kpsn.baseSN.SKUNO != scan.PARTNO)
            {
                throw new Exception($@"{kpsn.SerialNo} is {kpsn.SkuNo} config is {scan.PARTNO}");
            }
            if (kpsn.RepairFailedFlag == "1")
            {
                throw new Exception($@"{kpsn.SerialNo} is in repairing!");
            }
            //已被綁定的就不能重複綁定 這個卡關將由入口那裡的全局卡關轉移到每一個KP TYPE的單獨卡關裡面,如不需要請根據本地的實際需求拿掉 2020.03.20 add by fgg
            if (TRKP.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
            {
                throw new Exception(scan.VALUE + " has been link on other sn!");
            }

            T_R_LINK_CONTROL t_r_link_control = new T_R_LINK_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
            List<R_LINK_CONTROL> controlList = t_r_link_control.GetControlListByMainItem(sn.WorkorderNo, sfcdb);
            if (controlList != null && controlList.Count > 0)
            {
                string controlItem = "";
                foreach (R_LINK_CONTROL r in controlList)
                {
                    controlItem += r.SUB_ITEM + ",";
                }
                controlItem = controlItem.Substring(0, controlItem.Length - 1);
                #region CHECK_ALL_MAIN_SUB  (1)MAIN ITEM只能用配置的SUB ITEM，不能用其它的；(2)而配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用
                bool bCheckAll = controlList.Any(r => r.CATEGORY == "CHECK_ALL_MAIN_SUB");
                if (bCheckAll)
                {
                    var allSubItem = controlList.Where(r => r.CATEGORY == "CHECK_ALL_MAIN_SUB").Select(r => r.SUB_ITEM).ToList();
                    if (!allSubItem.Contains(kpsn.WorkorderNo))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { sn.WorkorderNo, controlItem }));
                    }
                }
                #endregion
                //CHECK_SUB_ITEM  (1)配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用 (2)MAIN ITEM 還可以用除SUB ITEM外其它的ITEM, 也可以不用配置的SUB ITME如keypart漏配
                bool bCheckSub = controlList.Any(r => r.CATEGORY == "CHECK_SUB_ITEM");
                if (!bCheckAll && !bCheckSub)
                {
                    var control = controlList.Find(c => c.SUB_ITEM == kpsn.WorkorderNo);
                    if (control == null)
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { sn.WorkorderNo, controlItem }));
                    }
                }
            }
            #region CHECK_SUB_ITEM  (1)配置的SUB ITEM 只能被MAIN ITEM用，不能被其它的使用 (2)MAIN ITEM 還可以用除SUB ITEM外其它的ITEM, 也可以不用配置的SUB ITME如keypart漏配
            var subList = sfcdb.ORM.Queryable<R_LINK_CONTROL>().Where(r => r.SUB_ITEM == kpsn.WorkorderNo).ToList();
            if (subList.Count > 0)
            {
                string subMainItem = "";
                foreach (R_LINK_CONTROL sub in subList)
                {
                    subMainItem += sub.MAIN_ITEM + ",";
                }
                var controlSub = subList.Find(c => c.MAIN_ITEM == sn.WorkorderNo);
                if (controlSub != null)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180919142802", new string[] { kpsn.WorkorderNo, subMainItem }));
                }
            }
            #endregion
            T_WWN_DATASHARING t_WWN = new T_WWN_DATASHARING(sfcdb, DB_TYPE_ENUM.Oracle);
            List<WWN_DATASHARING> wWN_vss = t_WWN.GetVssnByWsn(scan.VALUE, sfcdb);
            List<WWN_DATASHARING> wWN_css = t_WWN.GetCssnByVssn(scan.VALUE, sfcdb);
            if (wWN_vss != null)
            {
                //var CheckPartno = sfcdb.ORM.Queryable<C_KP_List_Item, C_KP_LIST>((CKLI, CKL) => CKL.ID == CKLI.LIST_ID)
                //    .Where((CKLI, CKL) => CKL.SKUNO == sn.baseSN.SKUNO && CKLI.KP_PARTNO.Contains("80-"))
                //    .OrderBy((CKLI, CKL) => CKLI.EDIT_TIME, SqlSugar.OrderByType.Desc)
                //    .ToList().FirstOrDefault();

                string checkSql = $@"select b.* from c_kp_list a, c_kp_list_item b where a.id=b.list_id and a.skuno='{sn.baseSN.SKUNO}' and b.kp_partno like '80-%' order by b.edit_time desc";
                DataTable checkDT = sfcdb.ExecSelect(checkSql).Tables[0];

                if (checkDT.Rows.Count > 0)
                {
                    //string csku = CheckPartno.KP_PARTNO.ToString();
                    string csku = checkDT.Rows[0]["KP_PARTNO"].ToString();
                    if (sn.baseSN.SKUNO == "ICX-EPS4000-SHELF")
                    {
                        string strSql = $@"  update WWN_datasharing set vssn = '{sn.baseSN.SN}', vsku = '{csku}', lasteditby='{API.LoginUser.EMP_NO}', lasteditdt = sysdate where wsn = '{scan.VALUE}'";
                        sfcdb.ExecSQL(strSql);
                    }
                    else
                    {
                        string strSql = $@"  update WWN_datasharing set cssn = '{sn.baseSN.SN}', csku = '{csku}', lasteditby='{API.LoginUser.EMP_NO}', lasteditdt = sysdate where wsn = '{scan.VALUE}'";
                        sfcdb.ExecSQL(strSql);
                    }
                }
                else
                {
                    string strSql = $@"  update WWN_datasharing set vssn = '{sn.baseSN.SN}', vsku = '{sn.baseSN.SKUNO}', lasteditby='{API.LoginUser.EMP_NO}', lasteditdt = sysdate where wsn = '{scan.VALUE}'";
                    sfcdb.ExecSQL(strSql);
                }

            }
            if (wWN_css != null)
            {
                //var CheckPartno = sfcdb.ORM.Queryable<C_KP_List_Item, C_KP_LIST>((CKLI, CKL) => CKL.ID == CKLI.LIST_ID)
                //    .Where((CKLI, CKL) => CKL.SKUNO == sn.baseSN.SKUNO && CKLI.KP_PARTNO.Contains("80-"))
                //    .OrderBy((CKLI, CKL) => CKLI.EDIT_TIME, SqlSugar.OrderByType.Desc)
                //    .ToList().FirstOrDefault();

                string checkSql = $@"select b.* from c_kp_list a, c_kp_list_item b where a.id=b.list_id and a.skuno='{sn.baseSN.SKUNO}' and b.kp_partno like '80-%' order by b.edit_time desc";
                DataTable checkDT = sfcdb.ExecSelect(checkSql).Tables[0];

                if (checkDT.Rows.Count > 0)
                {
                    //string csku = CheckPartno.KP_PARTNO.ToString();
                    string csku = checkDT.Rows[0]["KP_PARTNO"].ToString();
                    string strSql = $@"  update WWN_datasharing set cssn = '{sn.baseSN.SN}', csku = '{csku}', lasteditby='{API.LoginUser.EMP_NO}', lasteditdt = sysdate where vssn = '{scan.VALUE}'";
                    sfcdb.ExecSQL(strSql);
                }
                else
                {
                    string strSql = $@"  update WWN_datasharing set cssn = '{sn.baseSN.SN}', csku = '{sn.baseSN.SKUNO}', lasteditby='{API.LoginUser.EMP_NO}', lasteditdt = sysdate where vssn = '{scan.VALUE}'";
                    sfcdb.ExecSQL(strSql);
                }
            }

            ///Check PCBASN_ARUABA
            string checkSql1 = null;
            DataTable checkDT1;

            
            checkSql1 = $@"SELECT * FROM c_sku sku , SFCBASE.C_SERIES cs, SFCBASE.C_CUSTOMER cc WHERE sku.C_SERIES_ID= cs.ID AND cs.CUSTOMER_ID= cc.ID AND cc.CUSTOMER_NAME='ARUBA' AND SKUNO='{sn.baseSN.SKUNO}'";
            checkDT1 = sfcdb.ExecSelect(checkSql1).Tables[0];
            if (checkDT1.Rows.Count !=0)
            {
                string skuno = checkDT1.Rows[0]["CUSTOMER_NAME"].ToString();
                var CHECKFUN = sfcdb.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "CHECK_PCBA_SN_ARUBA" && t.CATEGORY == "CHECK_PCBA_SN_ARUBA" && t.VALUE == skuno && t.EXTVAL == kpsn.SkuNo  ).ToList();
                if (CHECKFUN.Count != 0)
                {
                    checkSql1 = $@"SELECT * FROM r_sn_kp WHERE SN ='{sn.baseSN.SN}' AND SCANTYPE='PCBA S/N' AND KP_NAME='PCBA' AND VALID_FLAG=1";
                    checkDT1 = sfcdb.ExecSelect(checkSql1).Tables[0];
                    if (checkDT1.Rows.Count != 0)
                    {
                        string Kpcheck = scan.VALUE.Substring(1, 10);
                        string Sncheck = sn.baseSN.SN.Substring(0, 10);
                        if (Kpcheck != Sncheck)
                        {
                            throw new Exception($@"{scan.VALUE} not same sn {sn.baseSN.SN}!");
                        }
                    }
                }
            }
            


            kpsn.baseSN.SHIPPED_FLAG = "1";
            kpsn.baseSN.SHIPDATE = DateTime.Now;
            kpsn.baseSN.EDIT_TIME = DateTime.Now;
            kpsn.baseSN.EDIT_EMP = API.LoginUser.EMP_NO;
            sfcdb.ORM.Updateable<R_SN>(kpsn.baseSN).Where(t => t.ID == kpsn.baseSN.ID).ExecuteCommand();
        }

        public static void PN_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            string sql = $@"select * from mes4.r_tr_sn where tr_sn='{scan.VALUE}'";
            DataTable dt = apdb.ORM.Ado.GetDataTable(sql);             
            if (dt.Rows.Count > 0)
            {
                string cust_kp_no = dt.Rows[0]["CUST_KP_NO"].ToString();
                if (!cust_kp_no.Equals(scan.PARTNO))
                {
                    throw new Exception($@"TR_SN[{scan.VALUE}],CUST_KP_NO[{cust_kp_no}] Not Equals Keypart Partno[{scan.PARTNO}]");
                }
            }
        }

        public static void JuniperSilver_SN_Check(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            var StrSN = scan.VALUE;
            //LogicObject.SN kpsn = new LogicObject.SN(StrSN, sfcdb, DB_TYPE_ENUM.Oracle);

            //if (kpsn.baseSN == null)
            //{
            //    //throw new Exception($@"'{StrSN}' not exist ");
            //}

            //var kppno = kpsn.SkuNo;
            //if (kppno != scan.PARTNO)
            //{
            //    throw new Exception($@"'{StrSN}' skuno is '{kppno}' ");
            //}

            var kppno = scan.PARTNO;

            var SilverConfig = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "SilverWip" && t.SKUNO == kppno).First();

            if (SilverConfig == null)
            {
                throw new Exception($@"'{kppno}' not config for SilverWip ");
            }

            int maxDays = 0;
            int maxHours = 0;
            try
            {
                maxDays = int.Parse(SilverConfig.EXTEND);
            }
            catch
            {
                throw new Exception($@"'{kppno}'  SilverWip MaxDays Config Error");
            }
            try
            {
                maxHours = int.Parse(SilverConfig.BASETEMPLATE);
            }
            catch
            {
                throw new Exception($@"'{kppno}'  SilverWip maxHours Config Error");
            }

            var SilverWip = sfcdb.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.SN == StrSN).First();
            if (SilverWip == null)
            {
                throw new Exception($@"'{StrSN}' not in SelverWip");
            }
            if (SilverWip.STATE_FLAG == "0")
            {
                throw new Exception($@"'{StrSN}'is out of SelverWip");
            }

            if (SilverWip.TEST_HOURS >= maxHours)
            {
                throw new Exception($@"'{StrSN}'is test {SilverWip.TEST_HOURS}h >= {maxHours}h");
            }
            var ts = DateTime.Now - SilverWip.START_TIME;
            var ds = ts.Value.TotalDays;
            if (ds >= maxDays)
            {
                throw new Exception($@"'{StrSN}'is in SilverWip {SilverWip.TEST_HOURS}d >= {maxDays}d");
            }

            var kpc = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.ID != scan.ID && t.SCANTYPE == "SILVER_SN" && t.VALUE == StrSN && t.VALID_FLAG == 1).First();
            if (kpc != null)
            {
                throw new Exception($@"'{StrSN}'is already used by {kpc.SN}");
            }
        }

        public static void ACCKIT_CHECK(SN_KP config, LogicObject.SN sn, Row_R_SN_KP scan, List<Row_R_SN_KP> scans, MesAPIBase API, OleExec sfcdb, OleExec apdb)
        {
            var RSN = sfcdb.ORM.Queryable<R_SN>().Where(t => t.SN == scan.VALUE && t.VALID_FLAG == "1").ToList();
            if (RSN.Count != 0)
            {
                LogicObject.SN kpsn = new LogicObject.SN(scan.VALUE, sfcdb, DB_TYPE_ENUM.Oracle);
                T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
                if (RSN[0].SHIPPED_FLAG == "1")
                {
                    throw new Exception("Keypart is DUP");
                }
                else if (RSN[0].REPAIR_FAILED_FLAG == "1")
                {
                    throw new Exception("Keypart is repair");
                }
                else if (RSN[0].COMPLETED_FLAG != "1")
                {
                    throw new Exception("Keypart not pass STOCKIN");
                }
                if (TRKP.GetListByValueAndValidFlag(scan.VALUE, "1", sfcdb).Count > 1)
                {
                    throw new Exception(scan.VALUE + " has been link on other sn!");
                }
                kpsn.baseSN.SHIPPED_FLAG = "1";
                kpsn.baseSN.SHIPDATE = DateTime.Now;
                kpsn.baseSN.EDIT_TIME = DateTime.Now;
                kpsn.baseSN.EDIT_EMP = API.LoginUser.EMP_NO;
                sfcdb.ORM.Updateable<R_SN>(kpsn.baseSN).Where(t => t.ID == kpsn.baseSN.ID).ExecuteCommand();
            }

        }
    }
}
