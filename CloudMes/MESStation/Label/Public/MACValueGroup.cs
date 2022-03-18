using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static MESDataObject.Constants.PublicConstants;

namespace MESStation.Label.Public
{
    public class MACValueGroup : LabelValueGroup
    {
        public MACValueGroup()
        {
            ConfigGroup = "MACValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMacInKeyPart", Description = "從KP获取MAC地址列表Format[000000000000]", Paras = new List<string>() { "SN", "MAC_KP_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMacColonInKeyPart", Description = "從KP获取MAC地址列表Format[00:00:00:00:00:00]", Paras = new List<string>() { "SN", "MAC_KP_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleMacInKeyPart", Description = "從KP获取一个MAC地址Format[000000000000]", Paras = new List<string>() { "SN", "MAC_KP_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleMacColonInKeyPart", Description = "從KP获取一个MAC地址Format[00:00:00:00:00:00]", Paras = new List<string>() { "SN", "MAC_KP_NAME" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMacInWWN", Description = "從WWN表获取MAC地址列表Format[000000000000]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMacColonInWWN", Description = "從WWN表获取MAC地址列表Format[00:00:00:00:00:00]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleMacInWWN", Description = "從WWN表获取一个MAC地址Format[000000000000]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleMacColonInWWN", Description = "從WWN表获取一个MAC地址Format[00:00:00:00:00:00]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleMacInWWNByType", Description = "指定間隔類型(:或者-)從WWN表获取一个MAC地址", Paras = new List<string>() { "SN", "TYPE" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleMacWithSpaceInWWN", Description = "從WWN表获取一个MAC地址Format[MAC 00 00 00 00 00 00]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleWwnInWWN", Description = "從WWN表获取一个WWN地址Format[0000000000000000]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleWwnInColonInWWN", Description = "從WWN表获取一个WWN地址Format[00:00:00:00:00:00:00:00]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleWwnWithColonInWWN", Description = "從WWN表获取一个WWN地址Format[WWN 00:00:00:00:00:00:00:00]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSingleWwnWithColonInWWN2", Description = "從WWN表获取一个WWN地址Format[WWN:00:00:00:00:00:00:00:00]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWONewMACRange", Description = "Vertiv離線打印MAC", Paras = new List<string>() { "WO", "QTY", "STATION", "EMP_NO", "LABEL_TYPE" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetWONewMACRePrint", Description = "Vertiv離線補打MAC", Paras = new List<string>() { "MAC", "STATION", "EMP_NO", "LABEL_TYPE" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetSsdmfnInWWN", Description = "從WWN表获取一个SSDMN", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetERKPMacInWWN", Description = "從ER阶，WWN表获取一个MAC", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetERMacColonInWWN", Description = "從ER阶,從WWN表获取一个MAC地址Format[00:00:00:00:00:00]", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetVertivTEMac", Description = "從TEST表获取MAC", Paras = new List<string>() { "SN" } });
        }

        public List<string> GetMacInKeyPart(OleExec SFCDB, string SN, string MAC_KP_NAME)
        {
            List<string> MAC = SFCDB.ORM.Queryable<R_SN_KP>()
                .Where(t => t.SN == SN && t.KP_NAME == MAC_KP_NAME)
                .OrderBy(t => t.ITEMSEQ)
                .OrderBy(t => t.SCANSEQ)
                .OrderBy(t => t.DETAILSEQ)
                .Select(t => t.VALUE)
                .ToList();
            for (int i = 0; i < MAC.Count; i++)
            {
                if (MAC[i].Contains(":"))
                {
                    MAC[i] = MAC[i].Replace(":", "");
                }
            }
            return MAC;
        }

        public List<string> GetMacColonInKeyPart(OleExec SFCDB, string SN, string MAC_KP_NAME)
        {
            List<string> MAC = SFCDB.ORM.Queryable<R_SN_KP>()
                .Where(t => t.SN == SN && t.KP_NAME == MAC_KP_NAME)
                .OrderBy(t => t.ITEMSEQ)
                .OrderBy(t => t.SCANSEQ)
                .OrderBy(t => t.DETAILSEQ)
                .Select(t => t.VALUE)
                .ToList();
            for (int i = 0; i < MAC.Count; i++)
            {
                if (!MAC[i].Contains(":"))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "$1:$2:$3:$4:$5:$6";
                    MAC[i] = Regex.Replace(MAC[i], regex, replace);
                }
            }
            return MAC;
        }

        public string GetSingleMacInKeyPart(OleExec SFCDB, string SN, string MAC_KP_NAME)
        {
            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<R_SN_KP>()
                    .Where(t => t.SN == SN && t.KP_NAME == MAC_KP_NAME)
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .Select(t => t.VALUE)
                    .First();

                if (MAC == null)
                {
                    MAC = "No data record";
                }
            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }

        public string GetSingleMacColonInKeyPart(OleExec SFCDB, string SN, string MAC_KP_NAME)
        {
            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<R_SN_KP>()
                    .Where(t => t.SN == SN && t.KP_NAME == MAC_KP_NAME)
                    .OrderBy(t => t.ITEMSEQ)
                    .OrderBy(t => t.SCANSEQ)
                    .OrderBy(t => t.DETAILSEQ)
                    .Select(t => t.VALUE)
                    .First();

                if (MAC == null)
                {
                    MAC = "No data record";
                }
                else if (!MAC.Contains(":"))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "$1:$2:$3:$4:$5:$6";
                    MAC = Regex.Replace(MAC, regex, replace);
                }
            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }

        public List<string> GetMacInWWN(OleExec SFCDB, string SN)
        {

            List<string> MAC = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN)
                .Select(t => t.MAC)
                .ToList();
            for (int i = 0; i < MAC.Count; i++)
            {
                if (MAC[i].Contains(":"))
                {
                    MAC[i] = MAC[i].Replace(":", "");
                }
            }
            return MAC;
        }

        public List<string> GetMacColonInWWN(OleExec SFCDB, string SN)
        {
            List<string> MAC = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN)
                .Select(t => t.MAC)
                .ToList();
            for (int i = 0; i < MAC.Count; i++)
            {
                if (!MAC[i].Contains(":"))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "$1:$2:$3:$4:$5:$6";
                    MAC[i] = Regex.Replace(MAC[i], regex, replace);
                }
            }
            return MAC;
        }

        public string GetSingleMacInWWN(OleExec SFCDB, string SN)
        {

            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                 .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t=>t.MAC != "" || t.MAC != null)
                 .Select(t => t.MAC)
                 .First();
                if (MAC == null)
                {
                    MAC = "No data record";
                }
            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }
        public string GetERKPMacInWWN(OleExec SFCDB, string SN)
        {

            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<WWN_DATASHARING,R_SN_KP>((t,r)=>t.CSSN == r.VALUE)
                 .Where((t,r)=>r.SN == SN && SqlSugar.SqlFunc.Contains(r.PARTNO,"84-") && r.VALID_FLAG =='1').Where((t, r) => t.MAC != "" || t.MAC != null) 
                 .Select((t, r) => t.MAC)
                 .First();
                if (MAC == null)
                {
                    MAC = "No data record";
                }
            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }
        public string GetSsdmfnInWWN(OleExec SFCDB, string SN)
        {

            string WWNTB0 = "";
            try
            {
                WWNTB0 = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                 .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.WWNTB0 != "" || t.WWNTB0 != null)
                 .Select(t => t.WWNTB0)
                 .First();
                if (WWNTB0 == null)
                {
                    WWNTB0 = "No data record";
                }
            }
            catch (Exception ex)
            {
                WWNTB0 = ex.Message;
            }
            return WWNTB0;
        }
        public string GetSingleMacColonInWWN(OleExec SFCDB, string SN)
        {
            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                    .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.MAC != "" || t.MAC != null)
                    .Select(t => t.MAC)
                    .First();

                if (MAC == null)
                {
                    MAC = "No data record";
                }
                else if (!MAC.Contains(":"))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "$1:$2:$3:$4:$5:$6";
                    MAC = Regex.Replace(MAC, regex, replace);
                }

            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }
        public string GetERMacColonInWWN(OleExec SFCDB, string SN)
        {
            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<WWN_DATASHARING, R_SN_KP>((t, r) => t.CSSN == r.VALUE)
                 .Where((t, r) => r.SN == SN && SqlSugar.SqlFunc.Contains(r.PARTNO, "84-") && r.VALID_FLAG == '1').Where((t, r) => t.MAC != "" || t.MAC != null)
                 .Select((t, r) => t.MAC)
                 .First();

                if (MAC == null)
                {
                    MAC = "No data record";
                }
                else if (!MAC.Contains(":"))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "$1:$2:$3:$4:$5:$6";
                    MAC = Regex.Replace(MAC, regex, replace);
                }

            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }

        public string GetSingleMacInWWNByType(OleExec SFCDB, string SN, string Type)
        {
            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.MAC != "" || t.MAC != null).Select(t => t.MAC).First();
                if (MAC == null)
                {
                    MAC = "No data record";
                }
                else
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = $@"$1{Type}$2{Type}$3{Type}$4{Type}$5{Type}$6";
                    MAC = Regex.Replace(MAC, regex, replace);
                }
            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }

        public string GetSingleMacWithSpaceInWWN(OleExec SFCDB, string SN)
        {
            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                    .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.MAC != "" || t.MAC != null).Select(t => t.MAC).First();
                if (MAC == null)
                {
                    MAC = "No data record";
                }
                else if (!MAC.Contains(" "))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "MAC $1 $2 $3 $4 $5 $6";
                    MAC = Regex.Replace(MAC, regex, replace);
                }
            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }

        public string GetSingleWwnInWWN(OleExec SFCDB, string SN)
        {
            string WWN = "";
            try
            {
                WWN = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                 .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.MAC != "" || t.MAC != null).Select(t => t.WWN).First();
                if (WWN == null)
                {
                    WWN = "No data record";
                }
            }
            catch (Exception ex)
            {
                WWN = ex.Message;
            }
            return WWN;
        }

        public string GetSingleWwnInColonInWWN(OleExec SFCDB, string SN)
        {
            string WWN = "";
            try
            {
                WWN = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                 .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.MAC != "" || t.MAC != null).Select(t => t.WWN).First();
                if (WWN == null)
                {
                    WWN = "No data record";
                }
                else if (!WWN.Contains(":"))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "$1:$2:$3:$4:$5:$6:$7:$8";
                    WWN = Regex.Replace(WWN, regex, replace);
                }
            }
            catch (Exception ex)
            {
                WWN = ex.Message;
            }
            return WWN;
        }

        public string GetSingleWwnWithColonInWWN(OleExec SFCDB, string SN)
        {
            string WWN = "";
            try
            {
                WWN = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                 .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.MAC != "" || t.MAC != null).Select(t => t.WWN).First();
                if (WWN == null)
                {
                    WWN = "No data record";
                }
                else if (!WWN.Contains(":"))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "WWN $1:$2:$3:$4:$5:$6:$7:$8";
                    WWN = Regex.Replace(WWN, regex, replace);
                }
            }
            catch (Exception ex)
            {
                WWN = ex.Message;
            }
            return WWN;
        }

        public string GetSingleWwnWithColonInWWN2(OleExec SFCDB, string SN)
        {
            string WWN = "";
            try
            {
                WWN = SFCDB.ORM.Queryable<WWN_DATASHARING>()
                 .Where(t => t.VSSN == SN || t.CSSN == SN || t.WSN == SN).Where(t => t.MAC != "" || t.MAC != null).Select(t => t.WWN).First();
                if (WWN == null)
                {
                    WWN = "No data record";
                }
                else if (!WWN.Contains(":"))
                {
                    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                    var replace = "WWN:$1:$2:$3:$4:$5:$6:$7:$8";
                    WWN = Regex.Replace(WWN, regex, replace);
                }
            }
            catch (Exception ex)
            {
                WWN = ex.Message;
            }
            return WWN;
        }
        public string GetVertivTEMac(OleExec SFCDB, string SN)
        {
            string MAC = "";
            try
            {
                MAC = SFCDB.ORM.Queryable<R_TEST_IDENTITY>()
               .Where(t =>t.SN==SN&&t.VALIDFLAG=="1"&&t.CATEGORYNAME=="MAC").Select(t => t.VAL).First();
                if (MAC == null)
                {
                    MAC = "No data record";
                }
                //else if (!MAC.Contains(":"))
                //{
                //    var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                //    var replace = "WWN:$1:$2:$3:$4:$5:$6:$7:$8";
                //    MAC = Regex.Replace(MAC, regex, replace);
                //}
            }
            catch (Exception ex)
            {
                MAC = ex.Message;
            }
            return MAC;
        }

        string GetMac(string RuleName, string CurValue, OleExec SFCDB)
        {
            string mac = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(RuleName, CurValue, SFCDB);
            var macrangval = MESPubLab.MESStation.SNMaker.SNmaker.GetNextVal(RuleName, CurValue, SFCDB.ORM);
            //5.如果取得的MAC值此前已經生成過, 則跳過
            if (SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.EXT5 == macrangval).Any())
            {
               return GetMac(RuleName, CurValue, SFCDB);                
            }
            return mac;
        }

        /// <summary>
        /// islock ?lock:unlock; 
        /// </summary>
        /// <param name="SFCDB"></param>
        /// <param name="islock"></param>
        void lockMacProcess(OleExec SFCDB, bool islock)
        {
            using (var mesdb = OleExec.GetSqlSugarClient(SFCDB.ORM.CurrentConnectionConfig.ConnectionString))
            {
                var lockobj = mesdb.Queryable<R_SERVICE_LOG>().Where(t => t.FUNCTIONTYPE == "VTMACPROCESS").ToList().FirstOrDefault();
                var objlock = islock ? MesBool.Yes.ExtValue() : MesBool.No.ExtValue();
                if (lockobj == null)
                {
                    mesdb.Insertable(new R_SERVICE_LOG()
                    {
                        ID = MesDbBase.GetNewID<R_SERVICE_LOG>(SFCDB.ORM, "VT"),
                        FUNCTIONTYPE = "VTMACPROCESS",
                        CURRENTEDITTIME = DateTime.Now,
                        DATA1 = objlock
                    }).ExecuteCommand();
                }
                //30分鐘自動解鎖
                else if (new TimeSpan(DateTime.Now.Ticks - Convert.ToDateTime(lockobj.CURRENTEDITTIME).Ticks).TotalMinutes > 30)
                {
                    lockobj.DATA1 = objlock;
                    lockobj.LASTEDITTIME = lockobj.CURRENTEDITTIME;
                    lockobj.CURRENTEDITTIME = DateTime.Now;
                    mesdb.Updateable(lockobj).ExecuteCommand();
                }
                else if (islock && lockobj.DATA1 == objlock)
                {
                    throw new Exception("GetMacProcess is running,pls wait and try again!");
                }
                else
                {
                    lockobj.DATA1 = objlock;
                    lockobj.LASTEDITTIME = lockobj.CURRENTEDITTIME;
                    lockobj.CURRENTEDITTIME = DateTime.Now;
                    mesdb.Updateable(lockobj).ExecuteCommand();
                }
            }
        }

        public List<string> GetWONewMACRange(OleExec SFCDB, string WO, string QTY, string STATION, string EMP_NO, string LABEL_TYPE)
        {
            List<string> macList = new List<string>();
            List<R_PRINT_LOG> logList = new List<R_PRINT_LOG>();
            List<R_RANGE_DETAIL> detailList = new List<R_RANGE_DETAIL>();
            try
            {
                #region 開始生成MAC-LOCK
                lockMacProcess(SFCDB,true);
                #endregion
                for (int i = 0; i < int.Parse(QTY); i++)
                {
                    //2.根據工單判斷機種是否指定Mac區間
                    List<R_RANGE_RULE> rangeRuleList = new List<R_RANGE_RULE>();
                    List<R_RANGE> rangeList = SFCDB.ORM.Queryable<R_WO_BASE, R_RANGE>((a, b) => a.SKUNO == b.VUL).Where((a, b) => a.WORKORDERNO == WO && b.VALID == "Y").Select((a, b) => b).ToList();
                    if (rangeList.Count > 0)
                    {
                        //如果指定，則以該區間打印
                        rangeRuleList = SFCDB.ORM.Queryable<R_RANGE_RULE>().Where(t => t.ID == rangeList[0].RULEID && t.VALID == "Y" && t.COMPLITED == "N").OrderBy(t => t.MIN).ToList();
                    }
                    else
                    {
                        //如果沒指定，則按順序以可用有效的Mac區間打印
                        rangeRuleList = SFCDB.ORM.Queryable<R_RANGE_RULE>().Where(t => t.VALID == "Y" && t.COMPLITED == "N").OrderBy(t => t.MIN).ToList();
                    }
                    if (rangeRuleList.Count == 0)
                    {
                        if (i == 0)
                            throw new Exception("No MacRange!");//第一次循環就查不到區間的直接報錯重新打印吧
                        else
                        {
                            macList.Add("Range Completed");
                            break;//不是第一次循環但區間用完了就退出打印前面生成的MAC
                        }
                    }
                    //3.根據Mac規則ID取得Mac規則名稱
                    List<C_SN_RULE> macRuleList = SFCDB.ORM.Queryable<C_SN_RULE>().Where(t => t.ID == rangeRuleList[0].RULEID).ToList();
                    if (macRuleList.Count == 0)
                    {
                        if (i == 0)
                            throw new Exception("No MacRule!");//第一次循環就查不到規則名稱的直接報錯重新打印吧
                        else
                        {
                            macList.Add("No MacRule");
                            break;//不是第一次循環但規則名稱查不到了就退出打印前面生成的MAC
                        }
                    }
                    //4.根據Mac規則名稱和區間當前值取得下一個MAC值
                    string curValue = Convert.ToInt32(rangeRuleList[0].CURVAL, 16).ToString();//區間當前值
                    string mac = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN(macRuleList[0].NAME, curValue, SFCDB);
                    var macrangval = MESPubLab.MESStation.SNMaker.SNmaker.GetNextVal(macRuleList[0].NAME, curValue, SFCDB.ORM);
                    //5.如果取得的MAC值此前已經生成過, 則跳過
                    if (SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.EXT5 == macrangval).Any())
                    //if (SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.VALUE == mac).Any())
                    {
                        throw new Exception("Mac Existed!");
                        //if (i == 0)
                        //    throw new Exception("Mac Existed!");//第一次循環就生成已存在的MAC肯定有問題報錯重新打印吧
                        //else
                        //{
                        //    macList.Add("Mac Existed");
                        //    break;//不是第一次循環生成已存在的MAC就退出打印前面生成的MAC
                        //}
                    }
                    //6.如果取得的MAC值 <= MAC區間的最大值, 則更新MAC區間的當前值; MAC值 = 最大值時額外更新此區間的打印完成狀態
                    if (Convert.ToInt32(mac.Substring(6), 16) <= Convert.ToInt32(rangeRuleList[0].MAX, 16))
                    {
                        rangeRuleList[0].CURVAL = mac.Substring(6);
                        rangeRuleList[0].EDITTIME = DateTime.Now;
                        rangeRuleList[0].EDITBY = EMP_NO;
                        if (Convert.ToInt32(mac.Substring(6), 16) == Convert.ToInt32(rangeRuleList[0].MAX, 16))
                        {
                            rangeRuleList[0].COMPLITED = "Y";
                        }
                        int p = SFCDB.ORM.Updateable(rangeRuleList[0]).Where(t => t.ID == rangeRuleList[0].ID && t.VALID == "Y" && t.COMPLITED == "N").ExecuteCommand();
                    }
                    //7.取得該規則該工單第幾次打印
                    int printTimes = 1;
                    List<string> printTimesList = SFCDB.ORM.Queryable<R_RANGE_DETAIL>().Where(t => t.RULEID == rangeRuleList[0].ID && t.EXT1 == WO && t.EXT4 != null).Select(t => t.EXT4).ToList();
                    if (printTimesList.Count > 0)
                    {
                        string maxTimes = printTimesList.Max();
                        printTimes = int.Parse(maxTimes) + 1;
                    }
                    //8.添加List用以寫入數據庫
                    macList.Add(mac);
                    R_RANGE_DETAIL RangDetail = new R_RANGE_DETAIL() { ID = MESDataObject.MesDbBase.GetNewID(SFCDB.ORM, "VERTIV", "R_RANGE_DETAIL"), RULEID = rangeRuleList[0].ID, VALUE = mac, EXT1 = WO, EXT2 = QTY, EXT3 = STATION, EXT4 = printTimes.ToString(), EXT5 = macrangval, EDITTIME = DateTime.Now, EDITBY = EMP_NO };
                    detailList.Add(RangDetail);
                    R_PRINT_LOG PrintLog = new R_PRINT_LOG() { ID = MESDataObject.MesDbBase.GetNewID(SFCDB.ORM, "VERTIV", "R_PRINT_LOG"), SN = mac, STATION = STATION, LABTYPE = LABEL_TYPE, CTYPE = "MAC", EDITTIME = DateTime.Now, EDITBY = EMP_NO };
                    logList.Add(PrintLog);
                }
                //9. 記錄生成的MAC和工單
                int n = detailList.Count > 0 ? SFCDB.ORM.Insertable(detailList).ExecuteCommand() : 0;
                int m = logList.Count > 0 ? SFCDB.ORM.Insertable(logList).ExecuteCommand() : 0;
            }
            catch (Exception ex)
            {
                macList.Clear();
                throw new Exception(ex.Message);
            }
            finally
            {
                #region 結束生成MAC-UnLOCK
                lockMacProcess(SFCDB, false);
                #endregion
            }
            return macList;
        }
        public List<string> GetWONewMACRePrint(OleExec SFCDB, string MAC, string STATION, string EMP_NO, string LABEL_TYPE)
        {
            List<string> macList = new List<string>();
            List<R_PRINT_LOG> logList = SFCDB.ORM.Queryable<R_PRINT_LOG>().Where(t => t.SN == MAC  && t.STATION == "PRINT_MAC" && t.CTYPE == "MAC").ToList();
            if (logList.Count > 0)
            {
                macList.Add(logList[0].SN);
                try
                {
                    R_PRINT_LOG log = new R_PRINT_LOG() { ID = MESDataObject.MesDbBase.GetNewID(SFCDB.ORM, "VERTIV", "R_PRINT_LOG"), SN = MAC, STATION = STATION, LABTYPE = LABEL_TYPE, CTYPE = "MAC", EDITTIME = DateTime.Now, EDITBY = EMP_NO };
                    int m = SFCDB.ORM.Insertable(log).ExecuteCommand();
                }
                catch (Exception)
                {
                    macList.Add("InsertLogFail");
                }
            }
            else
            {
                macList.Add("NoData");
            }
            return macList;
        }
    }
}
