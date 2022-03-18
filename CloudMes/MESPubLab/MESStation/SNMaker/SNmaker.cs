using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject;
using System.Text.RegularExpressions;

namespace MESPubLab.MESStation.SNMaker
{
    public class SNmaker
    {
        public static Dictionary<string, List<C_CODE_MAPPING>> _CodeMapping = new Dictionary<string, List<C_CODE_MAPPING>>();
        public static Dictionary<string, C_SN_RULE> _Root = new Dictionary<string, C_SN_RULE>();
        public static Dictionary<string, List<C_SN_RULE_DETAIL>> _Detail = new Dictionary<string, List<C_SN_RULE_DETAIL>>();
        private static object locker = new object();   



        public static List<C_CODE_MAPPING> GetCodeMapping(string name, OleExec DB)
        {
            List<C_CODE_MAPPING> ret = null;
            if (_CodeMapping.ContainsKey(name))
            {
                ret = _CodeMapping[name];
            }
            else
            {
                T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                ret = TCCM.GetDataByName(name, DB);
                if (ret != null)
                {
                    _CodeMapping.Add(name, ret);
                }
            }
            return ret;
        }

        /// <summary>
        /// 非精確檢查SN規則
        /// （1）遇到前綴直接查看SN是否以指定前綴開頭，如果是則將SN的頭部截去進行後面的判斷
        /// （2）遇到YYYY/MM/DD/WW等，從輸入SN中截取設定長度的字符串，再去該條規則對應的 C_CODE_MAPPING 中查找是否有該字符串的值，例如當前規則時 WEEK_10_2，那麼去
        /// C_CODE_MAPPING中可以找到52筆記錄，所以如果從SN中截取2位得到的字符串可以在52筆記錄中找到，那麼表示這是正確的，因此將SN從頭截去2位之後再次判斷
        /// （3）遇到SN流水碼這樣的規則，先比較長度，如果長度一樣，則再比較流水碼中每一位是否在 C_CODE_MAPPING 中是否存在從而判斷流水碼是否合法
        /// </summary>
        /// <param name="Sn"></param>
        /// <param name="RuleName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static bool CheckSnRuleInaccurately(string Sn, string RuleName, OleExec DB)
        {
            int CodeLocation = 0;
            T_C_CODE_MAPPING TCDM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
            try
            {
                C_SN_RULE root = null;
                List<C_SN_RULE_DETAIL> detail = null;
                if (_Root.ContainsKey(RuleName))
                {
                    root = _Root[RuleName];
                }
                else
                {
                    T_C_SN_RULE TCSR = new T_C_SN_RULE(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    root = TCSR.GetDataByName(RuleName, DB);
                    _Root.Add(RuleName, root);
                }

                if (_Detail.ContainsKey(RuleName))
                {
                    detail = _Detail[RuleName];
                }
                else
                {
                    T_C_SN_RULE_DETAIL TCSRD = new T_C_SN_RULE_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    detail = TCSRD.GetDataByRuleID(root.ID, DB);
                    _Detail.Add(RuleName, detail);
                }
                for (int i = 0; i < detail.Count; i++)
                {
                    if (detail[i].INPUTTYPE == "PREFIX")
                    {
                        if (Sn.StartsWith(detail[i].CURVALUE))
                        {
                            if (Sn.Length <= detail[i].CURVALUE.Length)
                            {
                                if (detail.Count == i + 1)
                                {
                                    break;
                                }
                                else
                                {
                                    throw new Exception("Error: SN too short!");
                                }
                            }
                            Sn = Sn.Substring(detail[i].CURVALUE.Length);
                            CodeLocation += detail[i].CURVALUE.Length;
                            continue;
                        }
                        else
                        {
                            throw new Exception($@"Error location'{CodeLocation}:'PREFIX' '{detail[i].CURVALUE}' not match!");
                        }
                    }
                    else if (detail[i].INPUTTYPE == "YYYY" || detail[i].INPUTTYPE == "MM" || detail[i].INPUTTYPE == "DD" || detail[i].INPUTTYPE == "WW")
                    {
                        string codeType = detail[i].CODETYPE;
                        List<C_CODE_MAPPING> CodeMapping = null;
                        if (_CodeMapping.ContainsKey(codeType))
                        {
                            CodeMapping = _CodeMapping[codeType];
                        }
                        else
                        {
                            T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                            CodeMapping = TCCM.GetDataByName(codeType, DB);
                            if (CodeMapping != null)
                            {
                                _CodeMapping.Add(codeType, CodeMapping);
                            }
                        }
                        if (Sn.Length < detail[i].CURVALUE.Length)
                        {
                            throw new Exception("Error: SN too short!");
                        }
                        var TempStr = Sn.Substring(0, detail[i].CURVALUE.Length);
                        C_CODE_MAPPING TAG = CodeMapping.Find(T => T.CODEVALUE == TempStr);
                        if (TAG != null)
                        {
                            Sn = Sn.Substring(detail[i].CURVALUE.Length);
                            CodeLocation += detail[i].CURVALUE.Length;
                            continue;
                        }
                        else
                        {
                            throw new Exception($@"Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' '{TempStr}' not match!");
                        }

                    }
                    else if (detail[i].INPUTTYPE == "SN")
                    {
                        string codeType = detail[i].CODETYPE;
                        List<C_CODE_MAPPING> CodeMapping = null;
                        if (_CodeMapping.ContainsKey(codeType))
                        {
                            CodeMapping = _CodeMapping[codeType];
                        }
                        else
                        {
                            T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                            CodeMapping = TCCM.GetDataByName(codeType, DB);
                            if (CodeMapping != null)
                            {
                                _CodeMapping.Add(codeType, CodeMapping);
                            }
                        }

                        bool ValidFlag = true;
                        if (Sn.Length < detail[i].CURVALUE.Length)
                        {
                            throw new Exception("Error: SN too short!");
                        }
                        var TempStr = Sn.Substring(0, detail[i].CURVALUE.Length);
                        if (TempStr.Length == detail[i].CURVALUE.Length)
                        {
                            int j = 0;
                            //var CodeValueLength = CodeMapping.FirstOrDefault().CODEVALUE.Length;
                            for (; j < detail[i].CURVALUE.Length; j++)
                            {
                                if (CodeMapping.Find(t => t.CODEVALUE == TempStr.Substring(j,t.CODEVALUE.Length)) == null)
                                {
                                    ValidFlag = false;
                                    break;
                                }
                            }
                            if (!ValidFlag)
                            {
                                throw new Exception($@"There is not exist {TempStr.Substring(j,1)} in type {detail[i].CODETYPE} at location {j} in {TempStr}");
                            }

                            Sn = Sn.Substring(detail[i].CURVALUE.Length);
                            CodeLocation += detail[i].CURVALUE.Length;
                            continue;

                        }
                        else if (detail[i].INPUTTYPE == "SN_EX")
                        {
                            continue;
                            //throw new Exception($@"Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' 'Length':{detail[i].CURVALUE.Length} not match!");
                        }
                        else
                        {
                            throw new Exception($@"Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' 'Length':{detail[i].CURVALUE.Length} not match!");
                        }
                    }
                }

            }
            catch (Exception ee)
            {
                throw ee;
            }
            return true;
        }

        public static bool CkeckSNRule(string SN, string RuleName, OleExec DB)
        {
            int CodeLocation = 0;
            string TempSn = SN;
            bool useRuleName = true;

            if (RuleName.ToUpper().StartsWith("REG://"))
            {
                var StrRegex = RuleName.Substring("REG://".Length);
                var m = Regex.Match(SN, StrRegex);


                if (m.Success == true)
                    return true;
                else
                    throw new Exception($@"Regex:'{StrRegex}' Match fail");
            }

            try
            {
                C_SN_RULE root = null;
                List<C_SN_RULE_DETAIL> detail = null;
                if (_Root.ContainsKey(RuleName))
                {
                    root = _Root[RuleName];
                }
                else
                {
                    T_C_SN_RULE TCSR = new T_C_SN_RULE(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    root = TCSR.GetDataByName(RuleName, DB);
                    if (root == null)
                    {
                        useRuleName = false;
                    }
                    else
                    {
                        _Root.Add(RuleName, root);
                    }
                }

                if (useRuleName && _Detail.ContainsKey(RuleName))
                {
                    detail = _Detail[RuleName];
                }
                else
                {
                    if (useRuleName)
                    {
                        T_C_SN_RULE_DETAIL TCSRD = new T_C_SN_RULE_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                        detail = TCSRD.GetDataByRuleID(root.ID, DB);
                        _Detail.Add(RuleName, detail);
                    }
                }
                if (useRuleName)
                {
                    for (int i = 0; i < detail.Count; i++)
                    {
                        if (detail[i].INPUTTYPE == "PREFIX")
                        {
                            if (SN.StartsWith(detail[i].CURVALUE))
                            {
                                if (SN.Length <= detail[i].CURVALUE.Length)
                                {
                                    if (detail.Count == i + 1)
                                    {
                                        SN = SN.Substring(detail[i].CURVALUE.Length);
                                        CodeLocation += detail[i].CURVALUE.Length;
                                        break;
                                    }
                                    else
                                    {
                                        throw new Exception("Error: SN too short!");
                                    }
                                }
                                SN = SN.Substring(detail[i].CURVALUE.Length);
                                CodeLocation += detail[i].CURVALUE.Length;
                                continue;
                            }
                            else
                            {
                                throw new Exception($@"Input:{TempSn};Error location'{CodeLocation}:'PREFIX' '{detail[i].CURVALUE}' not match!");
                            }
                        }
                        else if (detail[i].INPUTTYPE == "YYYY" || detail[i].INPUTTYPE == "MM" || detail[i].INPUTTYPE == "DD" || detail[i].INPUTTYPE == "WW")
                        {
                            string codeType = detail[i].CODETYPE;
                            List<C_CODE_MAPPING> CodeMapping = null;
                            if (_CodeMapping.ContainsKey(codeType))
                            {
                                CodeMapping = _CodeMapping[codeType];
                            }
                            else
                            {
                                T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                                CodeMapping = TCCM.GetDataByName(codeType, DB);
                                if (CodeMapping != null)
                                {
                                    _CodeMapping.Add(codeType, CodeMapping);
                                }
                            }
                            string VALUE = null;
                            switch (detail[i].INPUTTYPE)
                            {
                                case "YYYY":
                                    VALUE = DateTime.Now.Year.ToString();
                                    break;
                                case "MM":
                                    VALUE = DateTime.Now.Month.ToString();
                                    break;
                                case "DD":
                                    VALUE = DateTime.Now.Day.ToString();
                                    break;
                                case "WW":
                                    System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                                    VALUE = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                                    break;
                            }

                            T_C_SKU_DETAIL NOCHECK = new T_C_SKU_DETAIL(DB, DB_TYPE_ENUM.Oracle);
                            C_CODE_MAPPING TAG = CodeMapping.Find(T => T.VALUE == VALUE);

                            string _WW = SN.Substring(0, 2);
                            C_CODE_MAPPING CWW = CodeMapping.Find(T => T.CODEVALUE == _WW);

                            if (NOCHECK.NOCHECK_WW(TempSn, DB) && CodeMapping.Contains(CWW))
                            {
                                if (SN.StartsWith(CWW.CODEVALUE))
                                {
                                    if (SN.Length <= TAG.CODEVALUE.Length)
                                    {
                                        if (detail.Count == i + 1)
                                        {
                                            SN = SN.Substring(TAG.CODEVALUE.Length);
                                            CodeLocation += TAG.CODEVALUE.Length;
                                            break;
                                        }
                                        else
                                        {
                                            throw new Exception($"Input:{TempSn};Error: SN too short!");
                                        }
                                    }
                                    SN = SN.Substring(TAG.CODEVALUE.Length);
                                    CodeLocation += TAG.CODEVALUE.Length;
                                    continue;
                                }
                                else
                                {
                                    throw new Exception($@"Input:{TempSn};Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' '{TAG.CODEVALUE}' not match!");
                                }
                            }


                            string _MM = SN.Substring(0, TAG.CODEVALUE.Length);
                            C_CODE_MAPPING CMM = CodeMapping.Find(T => T.CODEVALUE == _MM);

                            if (NOCHECK.NOCHECK_MM(TempSn, DB) && CodeMapping.Contains(CMM))
                            {
                                if (SN.StartsWith(CMM.CODEVALUE))
                                {
                                    if (SN.Length <= TAG.CODEVALUE.Length)
                                    {
                                        if (detail.Count == i + 1)
                                        {
                                            SN = SN.Substring(TAG.CODEVALUE.Length);
                                            CodeLocation += TAG.CODEVALUE.Length;
                                            break;
                                        }
                                        else
                                        {
                                            throw new Exception($"Input:{TempSn};Error: SN too short!");
                                        }
                                    }
                                    SN = SN.Substring(TAG.CODEVALUE.Length);
                                    CodeLocation += TAG.CODEVALUE.Length;
                                    continue;
                                }
                                else
                                {
                                    throw new Exception($@"Input:{TempSn};Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' '{TAG.CODEVALUE}' not match!");
                                }
                            }

                            if (NOCHECK.NOCHECK_YYYY(TempSn, DB) && CodeMapping.Contains(CMM))
                            {
                                if (SN.StartsWith(CMM.CODEVALUE))
                                {
                                    if (SN.Length <= TAG.CODEVALUE.Length)
                                    {
                                        if (detail.Count == i + 1)
                                        {
                                            SN = SN.Substring(TAG.CODEVALUE.Length);
                                            CodeLocation += TAG.CODEVALUE.Length;
                                            break;
                                        }
                                        else
                                        {
                                            throw new Exception($"Input:{TempSn};Error: SN too short!");
                                        }
                                    }
                                    SN = SN.Substring(TAG.CODEVALUE.Length);
                                    CodeLocation += TAG.CODEVALUE.Length;
                                    continue;
                                }
                                else
                                {
                                    throw new Exception($@"Input:{TempSn};Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' '{TAG.CODEVALUE}' not match!");
                                }
                            }

                            if (SN.StartsWith(TAG.CODEVALUE))
                            {
                                if (SN.Length <= TAG.CODEVALUE.Length)
                                {
                                    if (detail.Count == i + 1)
                                    {
                                        SN = SN.Substring(TAG.CODEVALUE.Length);
                                        CodeLocation += TAG.CODEVALUE.Length;
                                        break;
                                    }
                                    else
                                    {
                                        throw new Exception($"Input:{TempSn};Error: SN too short!");
                                    }
                                }
                                SN = SN.Substring(TAG.CODEVALUE.Length);
                                CodeLocation += TAG.CODEVALUE.Length;
                                continue;
                            }
                            else
                            {
                                throw new Exception($@"Input:{TempSn};Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' '{TAG.CODEVALUE}' not match!");
                            }

                        }
                        else if (detail[i].INPUTTYPE == "SN")
                        {
                            if (SN.Length >= detail[i].CURVALUE.Length)
                            {
                                if (SN.Length <= detail[i].CURVALUE.Length)
                                {
                                    if (detail.Count == i + 1)
                                    {
                                        SN = SN.Substring(detail[i].CURVALUE.Length);
                                        CodeLocation += detail[i].CURVALUE.Length;
                                        break;
                                    }
                                    else
                                    {
                                        throw new Exception($"Input:{TempSn};Error: SN too short!");
                                    }
                                }
                                SN = SN.Substring(detail[i].CURVALUE.Length);
                                CodeLocation += detail[i].CURVALUE.Length;
                                continue;
                            }
                            else
                            {
                                throw new Exception($@"Input:{TempSn};Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' 'Length':{detail[i].CURVALUE.Length} not match!");
                            }
                        }
                        //Begin 加了一個SN_EX又沒有把代碼CheckIn,我這裡臨時加進來用來調試，請SN_EX始作俑者自己改這一段
                        else if (detail[i].INPUTTYPE == "SN_EX")
                        {
                            if (SN.Length >= detail[i].CURVALUE.Length)
                            {
                                if (SN.Length <= detail[i].CURVALUE.Length)
                                {
                                    if (detail.Count == i + 1)
                                    {
                                        SN = SN.Substring(detail[i].CURVALUE.Length);
                                        CodeLocation += detail[i].CURVALUE.Length;
                                        break;
                                    }
                                    else
                                    {
                                        throw new Exception($"Input:{TempSn};Error: SN too short!");
                                    }
                                }
                                SN = SN.Substring(detail[i].CURVALUE.Length);
                                CodeLocation += detail[i].CURVALUE.Length;
                                continue;
                            }
                            else
                            {
                                throw new Exception($@"Input:{TempSn};Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' 'Length':{detail[i].CURVALUE.Length} not match!");
                            }
                        }
                        //End 加了一個SN_EX又沒有把代碼CheckIn,我這裡臨時加進來用來調試，請SN_EX始作俑者自己改這一段
                        else if (detail[i].INPUTTYPE == "CCODE")
                        {
                            if (SN.Length >= detail[i].CURVALUE.Length)
                            {
                                if (SN.Length <= detail[i].CURVALUE.Length)
                                {
                                    if (detail.Count == i + 1)
                                    {
                                        SN = SN.Substring(detail[i].CURVALUE.Length);
                                        CodeLocation += detail[i].CURVALUE.Length;
                                        break;
                                    }
                                    else
                                    {
                                        throw new Exception($"Input:{TempSn};Error: SN too short!");
                                    }
                                }
                                SN = SN.Substring(detail[i].CURVALUE.Length);
                                CodeLocation += detail[i].CURVALUE.Length;
                                continue;
                            }
                            else
                            {
                                throw new Exception($@"Input:{TempSn};Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' 'Length':{detail[i].CURVALUE.Length} not match!");
                            }
                        }
                    }
                    if (SN.Length > 0)
                    {
                        throw new Exception($"Input:{TempSn};SN too long");
                    }
                }
                else
                {
                    bool snRuleFlag = true;
                    if (string.IsNullOrEmpty(RuleName))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000251"));
                    }

                    if (SN.Length != RuleName.Length)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000252", new string[] { RuleName }));
                    }
                    char[] charConfigRule = RuleName.ToCharArray();
                    var ruleArray = new string[charConfigRule.Length];
                    for (int i = 0; i < charConfigRule.Length; i++)
                    {
                        ruleArray[i] = charConfigRule[i].ToString();
                    }

                    char[] charSn = SN.ToCharArray();
                    var snArray = new string[charSn.Length];
                    for (int j = 0; j < charSn.Length; j++)
                    {
                        snArray[j] = charSn[j].ToString();
                    }

                    for (int k = 0; k < ruleArray.Length; k++)
                    {
                        if (ruleArray[k] == "*")
                        {
                            continue;
                        }
                        if (!ruleArray[k].Equals(snArray[k]))
                        {
                            snRuleFlag = false;
                            break;
                        }
                    }

                    if (!snRuleFlag)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { SN, RuleName }));
                    }
                }

            }
            catch (Exception ee)
            {
                throw ee;
            }
            
            return true;
        }
        public static string GetNextSN(string RuleName, MESDBHelper.OleExecPool SFCPOOL)
        {
            var sfcdb =SFCPOOL.Borrow();
            try
            {
                sfcdb.BeginTrain();
                var ret = GetNextSN(RuleName, sfcdb,true);
                sfcdb.CommitTrain();
                return ret;
            }
            catch(Exception e)
            {
                sfcdb.RollbackTrain();
                throw e;
            }
            finally
            {
                SFCPOOL.Return(sfcdb);
            }

        }


        /// <summary>
        /// 根據編碼規則的名字獲取到下一個 SN
        /// </summary>
        /// <param name="RuleName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static string GetNextSN(string RuleName, OleExec DB,bool UseCurrDB = false)
        {
            lock (locker)
            {
                if (!UseCurrDB)
                {
                    DB = DB.PoolItem.DBPool.Borrow();
                }
                try
                {
                    C_SN_RULE root = null;
                    List<C_SN_RULE_DETAIL> detail = null;
                    T_C_SN_RULE TCSR = new T_C_SN_RULE(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    DateTime sysdate = DB.ORM.GetDate();
                    //if (_Root.ContainsKey(RuleName))
                    //{
                    //    root = _Root[RuleName];
                    //    if (root == null)
                    //    {
                    //        root = TCSR.GetDataByName(RuleName, DB);
                    //    }
                    //}
                    //else
                    //{
                    //    //如果內存中有該編碼規則，那麼就使用，否則從數據庫加載一條數據，并加入到內存中
                    //    root = TCSR.GetDataByName(RuleName, DB);
                    //    _Root.Add(RuleName, root);
                    //}
                    //SN Maker can not use catch
                    root = TCSR.GetDataByName(RuleName, DB);
                    //_Root.Add(RuleName, root);
                    T_C_SN_RULE_DETAIL TCSRD = new T_C_SN_RULE_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    detail = TCSRD.GetDataByRuleID(root.ID, DB);


                    string SN = "";
                    bool ResetFlag = false;
                    //遍歷每個規則，根據 C_SN_RULE_DETAIL 表中的 SEQ_NO 進行遍歷
                    for (int i = 0; i < detail.Count; i++)
                    {
                        //首先鎖住這條 detail 記錄
                        detail[i].LockMe(DB);
                        //如果配置的是 PREFIX，則直接添加到 SN 變量后
                        if (detail[i].INPUTTYPE == "PREFIX")
                        {
                            SN += detail[i].CURVALUE;
                        }
                        //如果配置的是年月日格式
                        else if (detail[i].INPUTTYPE == "YYYY" || detail[i].INPUTTYPE == "MM" || detail[i].INPUTTYPE == "DD" || detail[i].INPUTTYPE == "WW")
                        {
                            //根據代碼類型在 C_CODE_MAPPING 表中找到對應的對象集合
                            string codeType = detail[i].CODETYPE;
                            List<C_CODE_MAPPING> CodeMapping = null;
                            if (_CodeMapping.ContainsKey(codeType))
                            {
                                CodeMapping = _CodeMapping[codeType];
                            }
                            else
                            {
                                T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                                CodeMapping = TCCM.GetDataByName(codeType, DB);
                                if (CodeMapping != null)
                                {
                                    _CodeMapping.Add(codeType, CodeMapping);
                                }
                            }
                            //如果是年月日格式，就返回 4位的年 或者 2位的月 或者 2位的日 或者 2位的周
                            string VALUE = null;
                            switch (detail[i].INPUTTYPE)
                            {
                                case "YYYY":
                                    //VALUE = DateTime.Now.Year.ToString();
                                    //改為取數據庫時間 2020.12.24 fgg
                                    VALUE = sysdate.Year.ToString();
                                    break;
                                case "MM":
                                    //VALUE = DateTime.Now.Month.ToString();
                                    //改為取數據庫時間 2020.12.24 fgg
                                    VALUE = sysdate.Month.ToString();
                                    break;
                                case "DD":
                                    //VALUE = DateTime.Now.Day.ToString();
                                    //改為取數據庫時間 2020.12.24 fgg
                                    VALUE = sysdate.Day.ToString();
                                    break;
                                case "WW":
                                    System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                                    //VALUE = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                                    //改為取數據庫時間 2020.12.24 fgg
                                    VALUE = gc.GetWeekOfYear(sysdate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                                    break;


                            }

                            //這段的目的是為了能重置流水號，當年、月、日、周等發生變化時，根據 VALUE 獲取到的 CodeMapping 對象的 CODEVALUE 與 當前的
                            //C_SN_RULE_DETAIL 對象的 CURVALUE 值不一樣，就會進入到裡面的判斷中，然後嘗試獲取 C_SN_RULE_DETAIL 對象的 RESETSN_FLAG 是否
                            //為 1，如果為 1，則將 ResetFlag 設置為true，等下次進入到流水碼計算時，將 C_SN_RULE_DETAIL 對象的 VALUE10 設置為初始值 RESETVALUE
                            C_CODE_MAPPING TAG = CodeMapping.Find(T => T.VALUE == VALUE);
                            if (detail[i].CURVALUE != TAG.CODEVALUE)
                            {
                                detail[i].CURVALUE = TAG.CODEVALUE;
                                if (detail[i].RESETSN_FLAG == 1)
                                {
                                    ResetFlag = true;
                                }
                            }
                            SN += detail[i].CURVALUE;
                        }

                        //如果配置的是 SN，那麼就獲取到當前 C_SN_RULE_DETAIL 對象的 Value10 屬性值，加 1 之後將 10 進制的數轉換成 N 進制的數
                        //（N 的大小由 CodeMapping 對象個數來提供），最後將轉換后的結果補零后拼接到 SN 變量後面
                        else if (detail[i].INPUTTYPE == "SN")
                        {
                            //重置流水號
                            if (ResetFlag)
                            {
                                detail[i].VALUE10 = detail[i].RESETVALUE;
                            }
                            string codeType = detail[i].CODETYPE;
                            List<C_CODE_MAPPING> CodeMapping = null;
                            if (_CodeMapping.ContainsKey(codeType))
                            {
                                CodeMapping = _CodeMapping[codeType];
                            }
                            else
                            {
                                T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                                CodeMapping = TCCM.GetDataByName(codeType, DB);
                                if (CodeMapping != null)
                                {
                                    _CodeMapping.Add(codeType, CodeMapping);
                                }
                            }
                            int curValue = int.Parse(detail[i].VALUE10);
                            curValue++;
                            detail[i].VALUE10 = curValue.ToString();
                            int T = CodeMapping.Count;
                            string sn = "";

                            while (curValue / T != 0)
                            {
                                int R = curValue % T;
                                sn = CodeMapping[R].CODEVALUE + sn;
                                curValue = curValue / T;
                            }
                            sn = CodeMapping[curValue].CODEVALUE + sn;
                            if (sn.Length < detail[i].CURVALUE.Length)
                            {
                                for (int k = 0; detail[i].CURVALUE.Length != sn.Length; k++)
                                {
                                    sn = "0" + sn;
                                }
                            }
                            if (sn.Length > detail[i].CURVALUE.Length)
                            {
                                //throw new Exception("生成的SN超過最大值!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113008"));
                            }

                            detail[i].CURVALUE = sn;
                            SN += detail[i].CURVALUE;
                        }
                        //增加INPUTTYPE=CCODE,需要套用公式計算 Add By ZHB 20200718
                        else if (detail[i].INPUTTYPE == "CCODE")
                        {
                            //根據代碼類型在 C_CODE_MAPPING 表中找到對應的對象集合
                            string codeType = detail[i].CODETYPE;
                            List<C_CODE_MAPPING> CodeMapping = null;
                            if (_CodeMapping.ContainsKey(codeType))
                            {
                                CodeMapping = _CodeMapping[codeType];
                            }
                            else
                            {
                                T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                                CodeMapping = TCCM.GetDataByName(codeType, DB);
                                if (CodeMapping != null)
                                {
                                    _CodeMapping.Add(codeType, CodeMapping);
                                }
                            }

                            string tempSN = SN;
                            int len = tempSN.Length;
                            int total = 0;
                            int x = detail.Find(T => T.INPUTTYPE == "SN").CURVALUE.Length;
                            if (tempSN.Substring(len - x).StartsWith("7"))
                            {
                                //throw new Exception("SN流水碼超出最大界限：6FFFF!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113020"));
                            }

                            for (int j = 0; j < len; j++)
                            {
                                string tempChar = tempSN.Substring(j, 1);
                                int value = Convert.ToInt32(CodeMapping.Find(T => T.CODEVALUE == tempChar).VALUE);
                                total += value * (len - j);
                            }
                            int remainder = total % 32;
                            string cCode = CodeMapping.Find(T => T.VALUE == remainder.ToString()).CODEVALUE;
                            SN = tempSN.Substring(0, len - x) + cCode + tempSN.Substring(len - x);
                        }
                        int T1 = 0;
                        detail[i].EDIT_TIME = DateTime.Now;
                        //string ret = DB.ExecSQL(detail[i].GetUpdateString(DB_TYPE_ENUM.Oracle));
                        string ret = DB.ORM.Updateable<C_SN_RULE_DETAIL>(detail[i]).Where(t => t.ID == detail[i].ID).ExecuteCommand().ToString();
                        if (!Int32.TryParse(ret, out T1) && T1 == 0)
                        {
                            //throw new Exception("更新序列值出錯!" + ret);
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113045", new string[] { ret }));
                        }
                    }
                    int T2 = 0;
                    root.CURVALUE = SN;
                    root.EDIT_TIME = DateTime.Now;
                    //string ret1 = DB.ExecSQL(root.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    string ret1 = DB.ORM.Updateable<C_SN_RULE>(root).Where(t => t.ID == root.ID).ExecuteCommand().ToString();
                    if (!Int32.TryParse(ret1, out T2) && T2 == 0)
                    {
                        //throw new Exception("更新序列值出錯!" + ret1);
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113045", new string[] { ret1 }));
                    }
                    return SN;
                }
                catch(Exception ee)
                {
                    throw ee;
                }
                finally
                {
                    if (!UseCurrDB)
                    {
                        DB.PoolItem.DBPool.Return(DB);
                    }
                }
            }
            
        }

        /// <summary>
        /// 根據編碼規則的名字獲取到下一個 SN
        /// </summary>
        /// <param name="RuleName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static string GetNextSN(string RuleName, SqlSugar.SqlSugarClient db)
        {
            C_SN_RULE root = null;
            List<C_SN_RULE_DETAIL> detail = null;
            DateTime sysdate = db.GetDate();
            root = db.Queryable<C_SN_RULE>().Where(t => t.NAME == RuleName).ToList().FirstOrDefault();
            detail = db.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == root.ID).OrderBy(t => t.SEQ).ToList();

            
            string SN = "";
            bool ResetFlag = false;
            //遍歷每個規則，根據 C_SN_RULE_DETAIL 表中的 SEQ_NO 進行遍歷
            for (int i = 0; i < detail.Count; i++)
            {
                //首先鎖住這條 detail 記錄
                detail[i].LockMe(db);
                //如果配置的是 PREFIX，則直接添加到 SN 變量后
                if (detail[i].INPUTTYPE == "PREFIX")
                {
                    SN += detail[i].CURVALUE;
                }
                //如果配置的是年月日格式
                else if (detail[i].INPUTTYPE == "YYYY" || detail[i].INPUTTYPE == "MM" || detail[i].INPUTTYPE == "DD" || detail[i].INPUTTYPE == "WW")
                {
                    //根據代碼類型在 C_CODE_MAPPING 表中找到對應的對象集合
                    string codeType = detail[i].CODETYPE;
                    List<C_CODE_MAPPING> CodeMapping = null;
                    if (_CodeMapping.ContainsKey(codeType))
                    {
                        CodeMapping = _CodeMapping[codeType];
                    }
                    else
                    {
                        //T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                        //CodeMapping = TCCM.GetDataByName(codeType, DB);
                        CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == codeType).OrderBy(t => t.SEQ).ToList();
                        if (CodeMapping != null)
                        {
                            _CodeMapping.Add(codeType, CodeMapping);
                        }
                    }
                    //如果是年月日格式，就返回 4位的年 或者 2位的月 或者 2位的日 或者 2位的周
                    string VALUE = null;
                    switch (detail[i].INPUTTYPE)
                    {
                        case "YYYY":
                            //VALUE = DateTime.Now.Year.ToString();
                            //改為取數據庫時間 2020.12.24 fgg
                            VALUE = sysdate.Year.ToString();
                            break;
                        case "MM":
                            //VALUE = DateTime.Now.Month.ToString();
                            //改為取數據庫時間 2020.12.24 fgg
                            VALUE = sysdate.Month.ToString();
                            break;
                        case "DD":
                            //VALUE = DateTime.Now.Day.ToString();
                            //改為取數據庫時間 2020.12.24 fgg
                            VALUE = sysdate.Day.ToString();
                            break;
                        case "WW":
                            System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                            //VALUE = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                            //改為取數據庫時間 2020.12.24 fgg
                            VALUE = gc.GetWeekOfYear(sysdate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                            break;


                    }

                    //這段的目的是為了能重置流水號，當年、月、日、周等發生變化時，根據 VALUE 獲取到的 CodeMapping 對象的 CODEVALUE 與 當前的
                    //C_SN_RULE_DETAIL 對象的 CURVALUE 值不一樣，就會進入到裡面的判斷中，然後嘗試獲取 C_SN_RULE_DETAIL 對象的 RESETSN_FLAG 是否
                    //為 1，如果為 1，則將 ResetFlag 設置為true，等下次進入到流水碼計算時，將 C_SN_RULE_DETAIL 對象的 VALUE10 設置為初始值 RESETVALUE
                    C_CODE_MAPPING TAG = CodeMapping.Find(T => T.VALUE == VALUE);
                    if (detail[i].CURVALUE != TAG.CODEVALUE)
                    {
                        detail[i].CURVALUE = TAG.CODEVALUE;
                        if (detail[i].RESETSN_FLAG == 1)
                        {
                            ResetFlag = true;
                        }
                    }
                    SN += detail[i].CURVALUE;
                }

                //如果配置的是 SN，那麼就獲取到當前 C_SN_RULE_DETAIL 對象的 Value10 屬性值，加 1 之後將 10 進制的數轉換成 N 進制的數
                //（N 的大小由 CodeMapping 對象個數來提供），最後將轉換后的結果補零后拼接到 SN 變量後面
                else if (detail[i].INPUTTYPE == "SN")
                {
                    //重置流水號
                    if (ResetFlag)
                    {
                        detail[i].VALUE10 = detail[i].RESETVALUE;
                    }
                    string codeType = detail[i].CODETYPE;
                    List<C_CODE_MAPPING> CodeMapping = null;
                    if (_CodeMapping.ContainsKey(codeType))
                    {
                        CodeMapping = _CodeMapping[codeType];
                    }
                    else
                    {
                        //T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                        //CodeMapping = TCCM.GetDataByName(codeType, DB);
                        CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == codeType).OrderBy(t => t.SEQ).ToList();
                        if (CodeMapping != null)
                        {
                            _CodeMapping.Add(codeType, CodeMapping);
                        }
                    }
                    int curValue = int.Parse(detail[i].VALUE10);
                    curValue++;
                    detail[i].VALUE10 = curValue.ToString();
                    int T = CodeMapping.Count;
                    string sn = "";

                    while (curValue / T != 0)
                    {
                        int R = curValue % T;
                        sn = CodeMapping[R].CODEVALUE + sn;
                        curValue = curValue / T;
                    }
                    sn = CodeMapping[curValue].CODEVALUE + sn;
                    if (sn.Length < detail[i].CURVALUE.Length)
                    {
                        for (int k = 0; detail[i].CURVALUE.Length != sn.Length; k++)
                        {
                            sn = "0" + sn;
                        }
                    }
                    if (sn.Length > detail[i].CURVALUE.Length)
                    {
                        var ex = detail.Find(t => t.CHECK_FLAG == detail[i].SEQ && t.INPUTTYPE == "SN_EX");
                        if (ex != null)
                        {
                            sn = sn.Substring(1);
                            detail[i].VALUE10 = "0";
                            MakeEx_SN(detail, ex, db);
                        }
                        else
                        {

                            //throw new Exception("生成的SN超過最大值!");
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113119"));

                        }
                    }

                    detail[i].CURVALUE = sn;
                    SN += detail[i].CURVALUE;
                }
                if (detail[i].INPUTTYPE == "SN_EX")
                {
                    continue;
                }
                    int T1 = 0;
                detail[i].EDIT_TIME = DateTime.Now;
                //string ret = DB.ExecSQL(detail[i].GetUpdateString(DB_TYPE_ENUM.Oracle));
                string ret = db.Updateable<C_SN_RULE_DETAIL>(detail[i]).Where(t => t.ID == detail[i].ID).ExecuteCommand().ToString();
                if (!Int32.TryParse(ret, out T1))
                {
                    //throw new Exception("更新序列值出錯!" + ret);
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113045", new string[] { ret }));
                }
            }
            SN = "";
            for (int i = 0; i < detail.Count; i++)
            {
                SN += detail[i].CURVALUE;
            }

            int T2 = 0;
            root.CURVALUE = SN;
            //string ret1 = DB.ExecSQL(root.GetUpdateString(DB_TYPE_ENUM.Oracle));
            string ret1 = db.Updateable<C_SN_RULE>(root).Where(t => t.ID == root.ID).ExecuteCommand().ToString();
            if (!Int32.TryParse(ret1, out T2))
            {
                //throw new Exception("更新序列值出錯!" + ret1);
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113045", new string[] { ret1 }));
            }
            return SN;
        }

        //static void MakeEx_SN(List<C_SN_RULE_DETAIL> ruls, C_SN_RULE_DETAIL SN_EX,SqlSugar.SqlSugarClient db)
        //{
        //    ////重置流水號
        //    //if (ResetFlag)
        //    //{
        //    //    detail[i].VALUE10 = detail[i].RESETVALUE;
        //    //}
        //    string codeType = SN_EX.CODETYPE;
        //    List<C_CODE_MAPPING> CodeMapping = null;
        //    if (_CodeMapping.ContainsKey(codeType))
        //    {
        //        CodeMapping = _CodeMapping[codeType];
        //    }
        //    else
        //    {
        //        //T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
        //        //CodeMapping = TCCM.GetDataByName(codeType, DB);
        //        CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == codeType).OrderBy(t => t.SEQ).ToList();
        //        if (CodeMapping != null)
        //        {
        //            _CodeMapping.Add(codeType, CodeMapping);
        //        }
        //    }
        //    int curValue = int.Parse(SN_EX.VALUE10);
        //    int T = CodeMapping.Count();
        //    if (curValue == T)
        //    {
        //        SN_EX.VALUE10 = "1";
        //    }
        //    else
        //    {
        //        curValue++;
        //        SN_EX.VALUE10 = curValue.ToString();
        //    }

        //    string sn = "";

        //    CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == SN_EX.CODETYPE && t.VALUE== SN_EX.VALUE10).OrderBy(t => t.SEQ).ToList();
        //    sn = CodeMapping[0].CODEVALUE ;
        //    SN_EX.CURVALUE = sn;
        //    db.Updateable(SN_EX).Where(t => t.ID == SN_EX.ID).ExecuteCommand();

        //    if (curValue == T)
        //    {

        //        var ex = ruls.Find(t => t.CHECK_FLAG == SN_EX.SEQ && t.INPUTTYPE == "SN_EX");
        //        if (ex != null)
        //        {
        //            MakeEx_SN(ruls, ex, db);
        //        }
        //        else
        //        {
        //            //throw new Exception("生成的SN超過最大值!");
        //            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113119"));
        //        }
        //    }

        //}

        static void MakeEx_SN(List<C_SN_RULE_DETAIL> ruls, C_SN_RULE_DETAIL SN_EX, SqlSugar.SqlSugarClient db)
        {
            ////重置流水號
            //if (ResetFlag)
            //{
            //    detail[i].VALUE10 = detail[i].RESETVALUE;
            //}
            string codeType = SN_EX.CODETYPE;
            List<C_CODE_MAPPING> CodeMapping = null;
            if (_CodeMapping.ContainsKey(codeType))
            {
                CodeMapping = _CodeMapping[codeType];
            }
            else
            {
                //T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                //CodeMapping = TCCM.GetDataByName(codeType, DB);
                CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == codeType).OrderBy(t => t.SEQ).ToList();
                if (CodeMapping != null)
                {
                    _CodeMapping.Add(codeType, CodeMapping);
                }
            }
            int curValue = int.Parse(SN_EX.VALUE10);
            curValue++;
            SN_EX.VALUE10 = curValue.ToString();
            int T = CodeMapping.Count;
            string sn = "";

            while (curValue / T != 0)
            {
                int R = curValue % T;
                sn = CodeMapping[R].CODEVALUE + sn;
                curValue = curValue / T;
            }
            sn = CodeMapping[curValue].CODEVALUE + sn;
            if (sn.Length < SN_EX.CURVALUE.Length)
            {
                for (int k = 0; SN_EX.CURVALUE.Length != sn.Length; k++)
                {
                    sn = CodeMapping[0].CODEVALUE + sn;
                }
            }
            if (sn.Length > SN_EX.CURVALUE.Length)
            {
                var ex = ruls.Find(t => t.CHECK_FLAG == SN_EX.SEQ && t.INPUTTYPE == "SN_EX");
                if (ex != null)
                {
                    sn = sn.Substring(1);
                    MakeEx_SN(ruls, ex, db);
                }
                else
                {
                    throw new Exception("生成的SN超過最大值!");
                }
            }

            SN_EX.CURVALUE = sn;
            db.Updateable(SN_EX).Where(t => t.ID == SN_EX.ID).ExecuteCommand();
            //SN += detail[i].CURVALUE;
        }

        public static string GetNextVal(string RuleName, string CurValue, SqlSugar.SqlSugarClient db)
        {
            List<C_SN_RULE_DETAIL> detail = null;
            C_SN_RULE root  = db.Queryable<C_SN_RULE>().Where(t => t.NAME == RuleName).ToList().FirstOrDefault();
            detail = db.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == root.ID).OrderBy(t => t.SEQ).ToList();
            var SNRULE = detail.FindAll(t => t.INPUTTYPE == "SN").ToList().FirstOrDefault();
            if (SNRULE == null)
                throw new Exception($@"Have not SN Rule!");
            string codeType = SNRULE.CODETYPE;
            List<C_CODE_MAPPING> CodeMapping = null;
            if (_CodeMapping.ContainsKey(codeType))
            {
                CodeMapping = _CodeMapping[codeType];
            }
            else
            {
                CodeMapping = db.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == codeType).OrderBy(t => t.SEQ).ToList();
                if (CodeMapping != null)
                {
                    _CodeMapping.Add(codeType, CodeMapping);
                }
            }
            //int curValue = int.Parse(detail[i].VALUE10);
            int curValue = int.Parse(CurValue);//這裡使用傳入的當前值
            curValue++;
            SNRULE.VALUE10 = curValue.ToString();

            int T = CodeMapping.Count;
            string sn = "";
            while (curValue / T != 0)
            {
                int R = curValue % T;
                sn = CodeMapping[R].CODEVALUE + sn;
                curValue = curValue / T;
            }
            sn = CodeMapping[curValue].CODEVALUE + sn;
            if (sn.Length < SNRULE.CURVALUE.Length)
            {
                for (int k = 0; SNRULE.CURVALUE.Length != sn.Length; k++)
                {
                    sn = "0" + sn;
                }
            }
            if (sn.Length > SNRULE.CURVALUE.Length)
            {
                //throw new Exception("生成的SN超過最大值!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113119"));
            }

            return sn;
        }


        /// <summary>
        /// 根據編碼規則的名字和傳入的當前值獲取到下一個 SN
        /// </summary>
        public static string GetNextSN(string RuleName, string CurValue, OleExec DB, bool UseCurrDB = false)
        {
            lock (locker)
            {
                if (!UseCurrDB)
                {
                    DB = DB.PoolItem.DBPool.Borrow();
                }
                try
                {
                    C_SN_RULE root = null;
                    List<C_SN_RULE_DETAIL> detail = null;
                    T_C_SN_RULE TCSR = new T_C_SN_RULE(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    DateTime sysdate = DB.ORM.GetDate();
                    //SN Maker can not use catch
                    root = TCSR.GetDataByName(RuleName, DB);
                    T_C_SN_RULE_DETAIL TCSRD = new T_C_SN_RULE_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    detail = TCSRD.GetDataByRuleID(root.ID, DB);

                    string SN = "";
                    bool ResetFlag = false;
                    //遍歷每個規則，根據 C_SN_RULE_DETAIL 表中的 SEQ_NO 進行遍歷
                    for (int i = 0; i < detail.Count; i++)
                    {
                        //首先鎖住這條 detail 記錄
                        detail[i].LockMe(DB);
                        //如果配置的是 PREFIX，則直接添加到 SN 變量后
                        if (detail[i].INPUTTYPE == "PREFIX")
                        {
                            SN += detail[i].CURVALUE;
                        }
                        //如果配置的是年月日格式
                        else if (detail[i].INPUTTYPE == "YYYY" || detail[i].INPUTTYPE == "MM" || detail[i].INPUTTYPE == "DD" || detail[i].INPUTTYPE == "WW")
                        {
                            //根據代碼類型在 C_CODE_MAPPING 表中找到對應的對象集合
                            string codeType = detail[i].CODETYPE;
                            List<C_CODE_MAPPING> CodeMapping = null;
                            if (_CodeMapping.ContainsKey(codeType))
                            {
                                CodeMapping = _CodeMapping[codeType];
                            }
                            else
                            {
                                T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                                CodeMapping = TCCM.GetDataByName(codeType, DB);
                                if (CodeMapping != null)
                                {
                                    _CodeMapping.Add(codeType, CodeMapping);
                                }
                            }
                            //如果是年月日格式，就返回 4位的年 或者 2位的月 或者 2位的日 或者 2位的周
                            string VALUE = null;
                            switch (detail[i].INPUTTYPE)
                            {
                                case "YYYY":
                                    //VALUE = DateTime.Now.Year.ToString();
                                    //改為取數據庫時間 2020.12.24 fgg
                                    VALUE = sysdate.Year.ToString();
                                    break;
                                case "MM":
                                    //VALUE = DateTime.Now.Month.ToString();
                                    //改為取數據庫時間 2020.12.24 fgg
                                    VALUE = sysdate.Month.ToString();
                                    break;
                                case "DD":
                                    //VALUE = DateTime.Now.Day.ToString();
                                    //改為取數據庫時間 2020.12.24 fgg
                                    VALUE = sysdate.Day.ToString();
                                    break;
                                case "WW":
                                    System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                                    //VALUE = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                                    //改為取數據庫時間 2020.12.24 fgg
                                    VALUE = gc.GetWeekOfYear(sysdate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                                    break;
                            }
                            //這段的目的是為了能重置流水號，當年、月、日、周等發生變化時，根據 VALUE 獲取到的 CodeMapping 對象的 CODEVALUE 與 當前的
                            //C_SN_RULE_DETAIL 對象的 CURVALUE 值不一樣，就會進入到裡面的判斷中，然後嘗試獲取 C_SN_RULE_DETAIL 對象的 RESETSN_FLAG 是否
                            //為 1，如果為 1，則將 ResetFlag 設置為true，等下次進入到流水碼計算時，將 C_SN_RULE_DETAIL 對象的 VALUE10 設置為初始值 RESETVALUE
                            C_CODE_MAPPING TAG = CodeMapping.Find(T => T.VALUE == VALUE);
                            if (detail[i].CURVALUE != TAG.CODEVALUE)
                            {
                                detail[i].CURVALUE = TAG.CODEVALUE;
                                if (detail[i].RESETSN_FLAG == 1)
                                {
                                    ResetFlag = true;
                                }
                            }
                            SN += detail[i].CURVALUE;
                        }
                        //如果配置的是 SN，那麼就獲取到當前 C_SN_RULE_DETAIL 對象的 Value10 屬性值，加 1 之後將 10 進制的數轉換成 N 進制的數
                        //（N 的大小由 CodeMapping 對象個數來提供），最後將轉換后的結果補零后拼接到 SN 變量後面
                        else if (detail[i].INPUTTYPE == "SN")
                        {
                            //重置流水號
                            if (ResetFlag)
                            {
                                detail[i].VALUE10 = detail[i].RESETVALUE;
                            }
                            string codeType = detail[i].CODETYPE;
                            List<C_CODE_MAPPING> CodeMapping = null;
                            if (_CodeMapping.ContainsKey(codeType))
                            {
                                CodeMapping = _CodeMapping[codeType];
                            }
                            else
                            {
                                T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                                CodeMapping = TCCM.GetDataByName(codeType, DB);
                                if (CodeMapping != null)
                                {
                                    _CodeMapping.Add(codeType, CodeMapping);
                                }
                            }
                            //int curValue = int.Parse(detail[i].VALUE10);
                            int curValue = int.Parse(CurValue);//這裡使用傳入的當前值
                            curValue++;
                            detail[i].VALUE10 = curValue.ToString();

                            int T = CodeMapping.Count;
                            string sn = "";
                            while (curValue / T != 0)
                            {
                                int R = curValue % T;
                                sn = CodeMapping[R].CODEVALUE + sn;
                                curValue = curValue / T;
                            }
                            sn = CodeMapping[curValue].CODEVALUE + sn;
                            if (sn.Length < detail[i].CURVALUE.Length)
                            {
                                for (int k = 0; detail[i].CURVALUE.Length != sn.Length; k++)
                                {
                                    sn = "0" + sn;
                                }
                            }
                            if (sn.Length > detail[i].CURVALUE.Length)
                            {
                                //throw new Exception("生成的SN超過最大值!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113119"));
                            }

                            detail[i].CURVALUE = sn;
                            SN += detail[i].CURVALUE;
                        }
                        //增加INPUTTYPE=CCODE,需要套用公式計算 Add By ZHB 20200718
                        else if (detail[i].INPUTTYPE == "CCODE")
                        {
                            //根據代碼類型在 C_CODE_MAPPING 表中找到對應的對象集合
                            string codeType = detail[i].CODETYPE;
                            List<C_CODE_MAPPING> CodeMapping = null;
                            if (_CodeMapping.ContainsKey(codeType))
                            {
                                CodeMapping = _CodeMapping[codeType];
                            }
                            else
                            {
                                T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                                CodeMapping = TCCM.GetDataByName(codeType, DB);
                                if (CodeMapping != null)
                                {
                                    _CodeMapping.Add(codeType, CodeMapping);
                                }
                            }

                            string tempSN = SN;
                            int len = tempSN.Length;
                            int total = 0;
                            int x = detail.Find(T => T.INPUTTYPE == "SN").CURVALUE.Length;
                            if (tempSN.Substring(len - x).StartsWith("7"))
                            {
                                //throw new Exception("SN流水碼超出最大界限：6FFFF!");
                                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113020"));
                            }

                            for (int j = 0; j < len; j++)
                            {
                                string tempChar = tempSN.Substring(j, 1);
                                int value = Convert.ToInt32(CodeMapping.Find(T => T.CODEVALUE == tempChar).VALUE);
                                total += value * (len - j);
                            }
                            int remainder = total % 32;
                            string cCode = CodeMapping.Find(T => T.VALUE == remainder.ToString()).CODEVALUE;
                            SN = tempSN.Substring(0, len - x) + cCode + tempSN.Substring(len - x);
                        }
                        detail[i].EDIT_TIME = DateTime.Now;
                        string ret = DB.ORM.Updateable<C_SN_RULE_DETAIL>(detail[i]).Where(t => t.ID == detail[i].ID).ExecuteCommand().ToString();
                        if (!Int32.TryParse(ret, out int T1) && T1 == 0)
                        {
                            //throw new Exception("更新序列值出錯!" + ret);
                            throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113045", new string[] { ret }));
                        }
                    }
                    root.CURVALUE = SN;
                    root.EDIT_TIME = DateTime.Now;
                    string ret1 = DB.ORM.Updateable<C_SN_RULE>(root).Where(t => t.ID == root.ID).ExecuteCommand().ToString();
                    if (!Int32.TryParse(ret1, out int T2) && T2 == 0)
                    {
                        //throw new Exception("更新序列值出錯!" + ret1);
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816113045", new string[] { ret1 }));
                    }
                    return SN;
                }
                catch (Exception ee)
                {
                    throw ee;
                }
                finally
                {
                    if (!UseCurrDB)
                    {
                        DB.PoolItem.DBPool.Return(DB);
                    }
                }
            }
        }
    }
}

