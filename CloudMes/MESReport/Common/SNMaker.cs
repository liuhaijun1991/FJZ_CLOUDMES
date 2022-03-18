using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject;

namespace MESReport.Common
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

        public static bool CkeckSNRule(string SN, string RuleName, OleExec DB)
        {
            int CodeLocation = 0;
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
                        if (SN.StartsWith(detail[i].CURVALUE))
                        {
                            if (SN.Length <= detail[i].CURVALUE.Length)
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
                            SN = SN.Substring(detail[i].CURVALUE.Length);
                            CodeLocation += detail[i].CURVALUE.Length;
                            continue;
                        }
                        else
                        {
                            throw new Exception($@"Error location'{CodeLocation}:'PREFIX' '{detail[i].CURVALUE}' not match!");
                        }
                    }
                    else if (detail[i].INPUTTYPE == "YYYY" || detail[i].INPUTTYPE == "MM" || detail[i].INPUTTYPE == "DD")
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
                        }
                        C_CODE_MAPPING TAG = CodeMapping.Find(T => T.VALUE == VALUE);
                        if (SN.StartsWith(TAG.CODEVALUE))
                        {
                            if (SN.Length <= TAG.CODEVALUE.Length)
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
                            SN = SN.Substring(TAG.CODEVALUE.Length);
                            CodeLocation += TAG.CODEVALUE.Length;
                            continue;
                        }
                        else
                        {
                            throw new Exception($@"Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' '{TAG.CODEVALUE}' not match!");
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
                            throw new Exception($@"Error location'{CodeLocation}':'{detail[i].INPUTTYPE}' 'Length':{detail[i].CURVALUE.Length} not match!");
                        }
                    }
                }

            }
            catch(Exception ee)
            {
                throw ee;
            }
            return true;
        }

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
            string SN = "";
            bool ResetFlag = false;
            for (int i = 0; i < detail.Count; i++)
            {
                detail[i].LockMe(DB);
                //DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.ID == detail[i].ID).With(SqlSugar.SqlWith.HoldLock).ToList();
                if (detail[i].INPUTTYPE == "PREFIX")
                {
                    SN += detail[i].CURVALUE;
                }
                else if (detail[i].INPUTTYPE == "YYYY" || detail[i].INPUTTYPE == "MM" || detail[i].INPUTTYPE == "DD")
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
                    }


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
                else if (detail[i].INPUTTYPE == "SN")
                {
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
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816162422"));
                    }

                    detail[i].CURVALUE = sn;
                    SN += detail[i].CURVALUE;
                }
                int T1 = 0;
                detail[i].EDIT_TIME = DateTime.Now;
                //string ret = DB.ExecSQL(detail[i].GetUpdateString(DB_TYPE_ENUM.Oracle));
                string ret = DB.ORM.Updateable<C_SN_RULE_DETAIL>(detail[i]).Where(t => t.ID == detail[i].ID).ExecuteCommand().ToString();
                if (!Int32.TryParse(ret, out T1))
                {
                    //throw new Exception("更新序列值出錯!" + ret);
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816153844" + ret));
                }
            }
            int T2 = 0;
            root.CURVALUE = SN;
            //string ret1 = DB.ExecSQL(root.GetUpdateString(DB_TYPE_ENUM.Oracle));
            string ret1 = DB.ORM.Updateable<C_SN_RULE>(root).ExecuteCommand().ToString();

            if (!Int32.TryParse(ret1, out T2))
            {
                //throw new Exception("更新序列值出錯!" + ret1);
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210816153844" + ret1));
            }
            return SN;
        }
    }
    public class SNRuleItem
    {
        //Row_C_SN_RULE_DETAIL Value;
        //public string GetNextValue

    }
}
