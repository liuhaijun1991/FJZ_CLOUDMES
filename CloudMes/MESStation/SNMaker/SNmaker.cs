using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.SNMaker
{
    public class SNmaker
    {
        public static Dictionary<string, List<C_CODE_MAPPING>> _CodeMapping = new Dictionary<string, List<C_CODE_MAPPING>>();
        public static Dictionary<string, C_SN_RULE> _Root = new Dictionary<string, C_SN_RULE>();
        public static Dictionary<string, List<C_SN_RULE_DETAIL>> _Detail = new Dictionary<string, List<C_SN_RULE_DETAIL>>();



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
        /// 根據編碼規則的名字獲取到下一個 SN
        /// </summary>
        /// <param name="RuleName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static string GetNextSN(string RuleName, OleExec DB)
        {
            C_SN_RULE root = null;
            List<C_SN_RULE_DETAIL> detail = null;
            if (_Root.ContainsKey(RuleName))
            {
                root = _Root[RuleName];
            }
            else
            {
                //如果內存中有該編碼規則，那麼就使用，否則從數據庫加載一條數據，并加入到內存中
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
                //如果內存中有編碼規則的具體信息，那就使用，否則從數據庫加載對應的編碼規則相信信息的集合，加入到內存中
                T_C_SN_RULE_DETAIL TCSRD = new T_C_SN_RULE_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                detail = TCSRD.GetDataByRuleID(root.ID, DB);
                _Detail.Add(RuleName, detail);
            }
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
                        for (int k = 0;  detail[i].CURVALUE.Length != sn.Length; k++)
                        {
                            sn = "0" + sn;
                        }
                    }
                    if (sn.Length > detail[i].CURVALUE.Length)
                    {
                        throw new Exception("生成的SN超過最大值!");
                    }

                    detail[i].CURVALUE = sn;
                    SN += detail[i].CURVALUE;
                }
                int T1 = 0;
                detail[i].EDIT_TIME = DateTime.Now;
                //string ret = DB.ExecSQL(detail[i].GetUpdateString(DB_TYPE_ENUM.Oracle));
                string ret = DB.ORM.Updateable<C_SN_RULE_DETAIL>(detail[i]).Where(t => t.ID == detail[i].ID).ExecuteCommand().ToString();
                if (! Int32.TryParse(ret, out T1))
                {
                    throw new Exception("更新序列值出錯!" + ret);
                }
            }
            int T2 = 0;
            root.CURVALUE = SN;
            //string ret1 = DB.ExecSQL(root.GetUpdateString(DB_TYPE_ENUM.Oracle));
            string ret1 = DB.ORM.Updateable<C_SN_RULE>(root).Where(t=>t.ID==root.ID).ExecuteCommand().ToString();
            if (!Int32.TryParse(ret1, out T2))
            {
                throw new Exception("更新序列值出錯!" + ret1);
            }
            return SN;
        }

        
    }

    public class SNRuleItem
    {
        Row_C_SN_RULE_DETAIL Value;
        //public string GetNextValue

    }
}
