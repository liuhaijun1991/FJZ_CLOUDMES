using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;

namespace MESStation.Label.Public
{
    /// <summary>
    /// Embedded機種Label變量取值比較繁瑣,有些加空格,有些加中杠,有些加版本
    /// 因此通用取值方法不適用此類Label,這裡新增一個Embedded機種Label取值專用方法
    /// </summary>
    public class EmbeddedValueGroup:LabelValueGroup
    {
        public EmbeddedValueGroup()
        {
            ConfigGroup = "EmbeddedValueGroup";
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetServiceNo", Description = "Get ServiceNo By SN", Paras = new List<string>() { "SN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetServiceExpress", Description = "Get ServiceExpress By Service No", Paras = new List<string>() { "SERVICE" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTempSnOnly", Description = "Get Temp SN Only", Paras = new List<string>() { "SN", "REPLACE_FLAG" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetTempSnWithField", Description = "Get Temp With Field", Paras = new List<string>() { "SN", "FIELD", "CONNECT_CHAR", "REPLACE_FLAG" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPKGID", Description = "Get PKG ID", Paras = new List<string>() { "PKGPrefix", "SN", "CustPN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetPPIDSN", Description = "Get PPID SN", Paras = new List<string>() { "SN", "SCANTYPE", "TYPE" } });
        }

        /// <summary>
        /// 獲取ServiceNo：PE跟客戶申請記錄在R_BRCD_EXSN表
        /// 與SN綁定條件：機種在R_FUNCTION_CONTROL表配置MCLABEL_SERVICE且KpList存在PPID S/N類型KP
        /// </summary>
        /// <returns></returns>
        public string GetServiceNo(OleExec SFCDB, string SN)
        {
            string value = "";
            try
            {
                value = SFCDB.ORM.Queryable<R_BRCD_EXSN>().Where(t => t.SN == SN && t.USE_FLAG == "1").Select(t => t.SERVICE_NO).First();
                if (value == null)
                {
                    value = "NoData";
                }
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            return value;
        }

        /// <summary>
        /// 獲取ServiceExpress：根據ServiceNo轉換成純數字
        /// </summary>
        /// <returns>返回時有指定要求：每3位數用空格隔開</returns>
        public string GetServiceExpress(OleExec SFCDB, string ServiceNo)
        {
            string value = "";
            try
            {
                value = ToDecimal("36", ServiceNo).ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length > 11)
                    {
                        value = value.Substring(0, 3) + " " + value.Substring(3, 3) + " " + value.Substring(6, 3) + " " + value.Substring(9, 3);
                    }
                    else
                    {
                        value = value.Substring(0, 3) + " " + value.Substring(3, 3) + " " + value.Substring(6, 3) + " " + value.Substring(9, value.Length - 9);
                    }
                }
                else
                {
                    value = "NoData";
                }
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            return value;
        }

        /// <summary>
        /// 儘獲取TEMP S/N1類型的KpValue
        /// 來源：在LOADIGN時根據配置的MCLabel規則生成PPID流水碼並額外寫入綁定關係表的一筆TEMP S/N1記錄
        /// PPID流水碼生成條件：機種KpList中存在PPID S/N類型KP
        /// </summary>
        /// <returns></returns>
        public string GetTempSnOnly(OleExec SFCDB, string SN, string ReplaceFlag)
        {
            string value = "";
            try
            {
                value = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.VALID_FLAG == 1 && t.SCANTYPE == "TEMP S/N1").Select(t => t.VALUE).First();
                if (string.IsNullOrEmpty(value))
                {
                    value = "NoData";
                }
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            return ReplaceFlag == "1" ? value.Replace("-", "") : value;
        }

        /// <summary>
        /// 獲取TEMP S/N1類型的KpValue和PE配置的Field值，用空格隔開
        /// 來源：在LOADIGN時根據配置的MCLabel規則生成PPID流水碼並額外寫入綁定關係表的一筆TEMP S/N1記錄
        /// PPID流水碼生成條件：機種KpList中存在PPID S/N類型KP
        /// </summary>
        /// <param name="Field">C_SKU_DETAIL中配置的固定值</param>
        /// <param name="ConnectChar">連接字符：空格/中杠</param>
        /// <param name="ReplaceFlag">是否替換</param>
        /// <returns></returns>
        public string GetTempSnWithField(OleExec SFCDB, string SN, string Field, string ConnectChar, string ReplaceFlag)
        {
            string value = "";
            try
            {
                value = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.VALID_FLAG == 1 && t.SCANTYPE == "TEMP S/N1").Select(t => t.VALUE).First();
                if (string.IsNullOrEmpty(value))
                {
                    value = "NoData";
                }
                string field = SFCDB.ORM.Queryable<R_SN, C_SKU_DETAIL>((t1, t2) => t1.SKUNO == t2.SKUNO).Where((t1, t2) => t1.SN == SN && t1.VALID_FLAG == "1" && t2.CATEGORY.ToUpper() == Field.ToUpper()).Select((t1, t2) => t2.VALUE).First();
                if (!string.IsNullOrEmpty(field))
                {
                    switch (ConnectChar)
                    {
                        case "":
                            value = value + " " + field;
                            break;
                        case "-":
                            //有些要加"-",有些則要把所有"-"去掉(包括value中的"-")
                            value = ReplaceFlag == "1" ? (value + "-" + field).Replace("-", "") : (value + "-" + field);
                            break;
                        default:
                            break;
                    }                    
                }
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            return value;
        }

        public string GetPKGID(OleExec SFCDB, string PKGPrefix, string SN, string CustPN)
        {
            string pkg = "";
            try
            {
                var y = DateTime.Now.Year.ToString();
                var m = ToDes36(DateTime.Now.Month);
                var d = ToDes36(DateTime.Now.Day);
                var dateTime = y.Substring(y.Length - 1, 1) + m + d;

                pkg = PKGPrefix + dateTime + SN.Substring(SN.Length - 6, 6) + CustPN + "1";
            }
            catch (Exception ex)
            {
                pkg = ex.Message;
            }
            return pkg;
        }

        public string GetPPIDSN(OleExec SFCDB, string SN, string ScanType, string Type)
        {
            string ppid = "";
            try
            {
                var s = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == SN && t.SCANTYPE == ScanType&& t.VALID_FLAG==1).Select(t => t.VALUE).First();
                var v = SFCDB.ORM.Queryable<C_SKU_DETAIL, R_SN>((t1, t2) => t1.SKUNO == t2.SKUNO).Where((t1, t2) => t1.CATEGORY == "REV" && t2.SN == SN && t2.VALID_FLAG == "1").Select((t1, t2) => t1.VALUE).First();
                if (s != null)
                {
                    switch (Type.ToUpper().Trim())
                    {
                        case "PPID":
                            ppid = s.Substring(0, 2) + s.Substring(3, 6) + s.Substring(10, 5) + s.Substring(16, 3) + s.Substring(20, 4) + (string.IsNullOrEmpty(v) ? "" : v);
                            break;
                        case "PPID1":
                            ppid = s.Substring(0, 16);
                            break;
                        case "PPID2":
                            ppid = s.Substring(16) + "-" + (string.IsNullOrEmpty(v) ? "" : v);
                            break;
                        case "TEMP":
                            ppid = s.Replace("-", "");
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ppid = ex.Message;
            }
            return string.IsNullOrEmpty(ppid) ? "" : ppid;
        }

        /// <summary>
        /// 將輸入內容轉換成指定格式的數字，跟C_CODE_MAPPING差不多，這裡完全Copy SQLServer的ToDecimal()方法
        /// </summary>
        /// <param name="MODEL">指定格式</param>
        /// <param name="InputStr">輸入內容</param>
        /// <returns></returns>
        private double ToDecimal(string MODEL, string InputStr)
        {
            int m = 1;
            int n = 0;
            int m2 = 0;
            int model = 0;
            double result = 0;
            double result2 = 0;
            string conStr = string.Empty;
            string inputStr = InputStr.ToUpper().Trim();

            #region 1.賦值conStr
            switch (MODEL)
            {
                case "3":
                    conStr = "05A";
                    break;
                case "10":
                    conStr = "0123456789";
                    break;
                case "16":
                    conStr = "0123456789ABCDEF";
                    break;
                case "30":
                    conStr = "0123456789BCDFGHJKLMNPRSTVWXYZ";
                    break;
                case "31":
                    conStr = "0123456789ABCDEFGHJKLMNPRSTWXYZ";
                    break;
                case "31_3":
                    MODEL = "31";
                    conStr = "0123456789ABCDEFGHJKLMNPRTUVWXY";
                    break;
                case "31_4":
                    MODEL = "31";
                    conStr = "0123456789ABCDEFGHJKLMNPRSTVWXY";
                    break;
                case "31_5":
                    MODEL = "31";
                    conStr = "123456789ABCDEFGHIJKLMNOPQRSTUV";
                    break;
                case "32":
                    conStr = "0123456789ABCDEFGHJKLMNPRSTVWXYZ";
                    break;
                case "32_2":
                    MODEL = "32";
                    conStr = "0123456789ABCDEFGHJKLMNPRSTUVWXY";
                    break;
                case "32_3":
                    MODEL = "32";
                    conStr = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
                    break;
                case "33":
                    conStr = "0123456789ABCDEFGHJKLMNPRSTUVWXYZ";
                    break;
                case "34":
                    conStr = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
                    break;
                case "36":
                    conStr = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                default:
                    break;
            }
            #endregion

            model = int.Parse(MODEL);
            if (conStr.Length > 0)
            {
                while (m <= inputStr.Length)
                {
                    result2 = 1;
                    string ch = inputStr.Substring(inputStr.Length - m, 1);
                    n = conStr.IndexOf(ch);
                    if (n < 0)
                    {
                        result = -1;
                        break;
                    }

                    m2 = m - 1;
                    while (m2 > 0)
                    {
                        result2 = result2 * model;
                        m2 = m2 - 1;
                    }

                    result = n * result2 + result;
                    m = m + 1;
                }
            }

            return result;
        }

        /// <summary>
        /// 將字符串轉換成數字，跟C_CODE_MAPPING差不多，這裡完全Copy SQLServer的ToDes36()方法
        /// </summary>
        /// <returns></returns>
        private string ToDes36(int InputStr)
        {
            int m = 0, n = 0;
            string result = string.Empty;
            string conStr = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            m = InputStr;
            while (m > 0)
            {
                n = m % 36;
                m = (int)Math.Floor((decimal)m / 36);
                string ch = conStr.Substring(n, 1);

                result = ch + result;
            }

            return result;
        }
    }
}
