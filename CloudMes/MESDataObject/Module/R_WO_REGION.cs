using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using System.Text.RegularExpressions;

//WO_Skuno: get in MO
namespace MESDataObject.Module
{
    public class T_R_WO_REGION : DataObjectTable
    {
        public T_R_WO_REGION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_REGION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_REGION);
            TableName = "R_WO_REGION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int GetWoDistributed(string WorkOrder, OleExec DB)
        {
            if (!DB.ORM.Queryable<R_WO_REGION>().Any(t => t.WORKORDERNO == WorkOrder))
            {
                return 0;
            }
            return (int)DB.ORM.Queryable<R_WO_REGION>()
                .Where(t => t.WORKORDERNO == WorkOrder)
                .Select(t => SqlSugar.SqlFunc.AggregateSum(t.QTY))
                .GroupBy("WORKORDERNO").ToList().FirstOrDefault();
        }

        /// <summary>
        /// 申請工單區間
        /// </summary>
        /// <param name="Workorder"></param>
        /// <param name="DB"></param>
        public int ApplyWoRange(string Bu, string emp, string Workorder, int Quantity, ref string StartSn, ref string EndSn, OleExec DB,bool IsPanel=false)
        {
            string errMesg = string.Empty;

            int result = 0;
            
            //檢查工單是否存在
            R_WO_BASE WOBase = DB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == Workorder).ToList().FirstOrDefault();
            if (string.IsNullOrEmpty(WOBase.WORKORDERNO))
            {
                errMesg = MESReturnMessage.GetMESReturnMessage("MES00000164", new string[] { Workorder });
                throw new MESReturnMessage(errMesg);
            }
            //查詢工單已經分配的區間
            //int existRangeCount = DB.ORM.Queryable<R_WO_REGION_DETAIL>().Where(t => t.WORKORDERNO == Workorder).ToList().Count;

            //如果要分配的數量超過工單剩餘分配數量的 1.2 倍就報錯
            //if ((WOBase.WORKORDER_QTY * 1.2 - existRangeCount) < Quantity)
            //{
            //    errMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180904142107",new string[]{ WOBase.WORKORDER_QTY.ToString() });
            //    throw new MESReturnMessage(errMesg);
            //}
            C_SN_RULE SNRule = DB.ORM.Queryable<R_WO_BASE, C_SKU, C_SN_RULE>
                    ((wb, s, sr) => wb.SKUNO == s.SKUNO && wb.SKU_VER == s.VERSION && s.SN_RULE == sr.NAME)
                    .Where((wb, s, sr) => wb.WORKORDERNO == Workorder)
                    .Select((wb, s, sr) => sr).ToList().FirstOrDefault();

            if (IsPanel)
            {
                SNRule = DB.ORM.Queryable<R_WO_BASE, C_SKU, C_SN_RULE>
                    ((wb, s, sr) => wb.SKUNO == s.SKUNO && wb.SKU_VER == s.VERSION && s.PANEL_RULE == sr.NAME)
                    .Where((wb, s, sr) => wb.WORKORDERNO == Workorder)
                    .Select((wb, s, sr) => sr).ToList().FirstOrDefault();
            }

            

            List<C_SN_RULE_DETAIL> details = new List<C_SN_RULE_DETAIL>();

            //如果工單對應的機種沒有配置 SN 規則或者 SN 規則不存在，那麼就報錯
            if (SNRule!=null)
            {
                //分配 SN 區間
                DistributeSnRange(SNRule, Workorder, Quantity,ref StartSn,ref EndSn, emp, DB);

                R_WO_REGION region = new R_WO_REGION()
                {
                    ID = GetNewID(Bu, DB),
                    WORKORDERNO = Workorder,
                    SKUNO = WOBase.SKUNO,
                    QTY = Quantity,
                    MIN_SN = StartSn,
                    MAX_SN = EndSn,
                    EDIT_EMP = emp,
                    EDIT_TIME = GetDBDateTime(DB)
                };

                result = DB.ORM.Insertable<R_WO_REGION>(region).ExecuteCommand();

                if (result > 0)
                {
                    //展開 SN 區間
                    SpreadSnRange(SNRule, Workorder, Quantity, StartSn, EndSn, emp, Bu, DB,IsPanel);
                }

                return result;
            }
            else
            {
                errMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180904142526", new string[] { WOBase.SKUNO });
                throw new MESReturnMessage(errMesg);
            }
        }

        /// <summary>
        /// 分配 SN 區間
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="WorkOrder"></param>
        /// <param name="Quantity"></param>
        /// <param name="StartSn"></param>
        /// <param name="EndSn"></param>
        /// <param name="Emp"></param>
        /// <param name="DB"></param>
        public void DistributeSnRange(C_SN_RULE rule,string WorkOrder, int Quantity, ref string StartSn, ref string EndSn,string Emp, OleExec DB)
        {
            StartSn = rule.CURVALUE;
            string TempSN = string.Empty;
            bool ResetFlag = false;
            string LastCode = string.Empty;
            string StartCode = string.Empty;
            string FixValue = string.Empty;
            C_SN_RULE_DETAIL FixRuleDetail = null;
            bool FixFlag = false;
            Dictionary<string, List<C_CODE_MAPPING>> CodeMappings = new Dictionary<string, List<C_CODE_MAPPING>>();
            T_C_CODE_MAPPING TCCM = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
            List<C_SN_RULE_DETAIL> details = DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == rule.ID).OrderBy(t=>t.SEQ).ToList();


            rule.LockMe(DB);
            foreach (C_SN_RULE_DETAIL detail in details)
            {
                detail.LockMe(DB);

                if (detail.INPUTTYPE == "PREFIX")
                {
                    TempSN += detail.CURVALUE;
                }
                else
                {
                    string codeType = detail.CODETYPE;
                    List<C_CODE_MAPPING> CodeMapping = null;
                    if (CodeMappings.Keys.Contains(codeType))
                    {
                        CodeMapping = CodeMappings[codeType];
                    }
                    else
                    {
                        CodeMapping = TCCM.GetDataByName(codeType, DB);
                        CodeMappings.Add(codeType, CodeMapping);
                    }

                    if (detail.INPUTTYPE == "YYYY" || detail.INPUTTYPE == "MM" || detail.INPUTTYPE == "DD" || detail.INPUTTYPE == "WW")
                    {
                        //如果是年月日格式，就返回 4位的年 或者 2位的月 或者 2位的日 或者 2位的周
                        string VALUE = null;
                        switch (detail.INPUTTYPE)
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
                        if (detail.CURVALUE != TAG.CODEVALUE)
                        {
                            detail.CURVALUE = TAG.CODEVALUE;
                            if (detail.RESETSN_FLAG == 1)
                            {
                                ResetFlag = true;
                            }
                        }
                        TempSN += detail.CURVALUE;
                    }

                    else if (detail.INPUTTYPE == "FIXCODE")
                    {
                        if (ResetFlag)
                        {
                            detail.VALUE10 = detail.RESETVALUE;
                        }

                        FixFlag = true;
                        FixRuleDetail = detail;
                    }

                    else if (detail.INPUTTYPE == "SN")
                    {
                        //重置流水號
                        if (ResetFlag)
                        {
                            detail.VALUE10 = detail.RESETVALUE;
                        }

                        if (FixFlag)
                        {
                            List<C_CODE_MAPPING> tempCCM = CodeMappings["FIXCODE"];
                            //計算固定碼部分的值是多少，固定碼部分的值為 （固定碼的value） * （當前流水碼CodeMapping數量）的（流水碼長度次冪）
                            int FixPart = (int)(int.Parse(tempCCM.Find(t => t.CODEVALUE == FixRuleDetail.CURVALUE).VALUE) * Math.Pow(CodeMapping.Count, detail.CURVALUE.Length));
                            //計算當前固定碼CodeMapping 可以表示的最大的值，計算為 （固定碼CodeMapping的數量） * （當前流水碼CodeMapping數量）的（流水碼長度次冪）
                            int FixMaxValue = (int)(tempCCM.Count * Math.Pow(CodeMapping.Count, detail.CURVALUE.Length));
                            //計算固定碼和流水碼結合后的開始值
                            int FixStartValue = FixPart + int.Parse(detail.VALUE10)+1;
                            if (FixStartValue > FixMaxValue)
                            {
                                throw new Exception("生成的SN超過最大值!");
                            }
                            //計算從固定碼開始之後的指定 Quantity 數量后的結束值
                            int FixEndValue = FixStartValue + Quantity;
                            if (FixEndValue > FixMaxValue)
                            {
                                throw new Exception("生成的SN超過最大值!");
                            }

                            int a = 0;
                            int b = 0;
                            //根據固定碼和流水碼結合后的開始值來得到流水碼的CodeValue，並且將開始固定碼的值返回回來
                            string start = GetCodeWithFix(detail.CURVALUE.Length, CodeMapping, FixStartValue,out a);
                            //根據固定碼和流水碼結合后的結束值來得到流水碼的CodeValue，並且將結束固定碼的值返回回來
                            string end = GetCodeWithFix(detail.CURVALUE.Length, CodeMapping, FixEndValue, out b);

                            //開始CodeValue為開始固定碼+開始流水碼
                            StartCode = tempCCM.Find(t => t.VALUE == a.ToString()).CODEVALUE + start;
                            //結束CodeValue為結束固定碼+結束流水碼
                            LastCode = tempCCM.Find(t => t.VALUE == b.ToString()).CODEVALUE + end;
                            //開始SN為 TempSN + 開始固定碼 + 開始流水碼
                            StartSn = TempSN + StartCode;
                            //結束SN為 TempSN + 結束固定碼 + 結束流水碼
                            EndSn = TempSN + LastCode;

                            //更新當前流水碼規則的值
                            detail.VALUE10 = FixEndValue.ToString();
                            detail.CURVALUE = end;
                            //更新整個規則的當前值
                            rule.CURVALUE = EndSn;

                            //更新固定碼規則的當前值
                            FixRuleDetail.CURVALUE = tempCCM.Find(t => t.VALUE == b.ToString()).CODEVALUE;
                            FixRuleDetail.EDIT_TIME = GetDBDateTime(DB);
                            FixRuleDetail.EDIT_EMP = Emp;
                            DB.ORM.Updateable<C_SN_RULE_DETAIL>(FixRuleDetail).Where(t => t.ID == FixRuleDetail.ID).ExecuteCommand();


                        }
                        else
                        {
                            StartCode = GetNextCode(detail, CodeMapping, 1);
                            LastCode = GetNextCode(detail, CodeMapping, Quantity);
                            StartSn = TempSN + StartCode;
                            EndSn = TempSN + LastCode;

                            detail.VALUE10 = (Int32.Parse(detail.VALUE10) + Quantity).ToString();
                            detail.CURVALUE = LastCode;
                            rule.CURVALUE = TempSN + detail.CURVALUE;
                        }

                    }
                }

                detail.EDIT_EMP = Emp;
                detail.EDIT_TIME = GetDBDateTime(DB);
                DB.ORM.Updateable<C_SN_RULE_DETAIL>(detail).Where(t => t.ID == detail.ID).ExecuteCommand();
            }
            rule.EDIT_EMP = Emp;
            rule.EDIT_TIME = GetDBDateTime(DB);
            DB.ORM.Updateable<C_SN_RULE>(rule).Where(t => t.ID == rule.ID).ExecuteCommand();
            
        }

        public string GetCodeWithFix(int CodeLength,List<C_CODE_MAPPING> CodeMapping,int value,out int FixValue)
        {
            string seq = string.Empty;
            int CodeMappingCount = CodeMapping.Count;
            for (int i = 0; i < CodeLength; i++)
            {
                int R = value % CodeMappingCount;
                seq = CodeMapping[R].CODEVALUE + seq;
                value = value / CodeMappingCount;
            }

            if (seq.Length < CodeLength)
            {
                for (int k = 0; CodeLength != seq.Length; k++)
                {
                    seq = "0" + seq;
                }
            }
            if (seq.Length > CodeLength)
            {
                throw new Exception("生成的SN超過最大值!");
            }
            FixValue = value;
            return seq;
        }

        public void SpreadSnRange(C_SN_RULE rule,string WorkOrder,int Quantity,string StartSn,string EndSn,string Emp,string Bu,OleExec DB,bool IsPanel=false)
        {
            DateTime dt = GetDBDateTime(DB);
            int seq = 0;
            int UnChanged = 0;
            int Changed = 0;
            bool FixFlag = false;
            string StartCode = string.Empty;
            string EndCode = string.Empty;
            int StartValue = 0;
            T_C_CODE_MAPPING T_CodeMapping = new T_C_CODE_MAPPING(DB, DB_TYPE_ENUM.Oracle);
            List<R_WO_REGION_DETAIL> RegionDetails = new List<R_WO_REGION_DETAIL>();
            string Prefix = string.Empty;
            string Suffix = string.Empty;
            string Code = string.Empty;

            string Type = IsPanel ? "PANEL_SN" : "SN";

            //獲得即將要插入數據的序列值
            R_WO_REGION_DETAIL existDetail = DB.ORM.Queryable<R_WO_REGION_DETAIL>().Where(t => t.WORKORDERNO == WorkOrder).OrderBy(t => SqlSugar.SqlFunc.ToInt32(t.SEQ), SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (existDetail != null)
            {
                seq = Int32.Parse(existDetail.SEQ) + 1;
            }
            else
            {
                seq = 1;
            }

            //獲取不變碼的長度
            List<C_SN_RULE_DETAIL> RuleDetails = DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == rule.ID).OrderBy(t=>t.SEQ).ToList();
            foreach (C_SN_RULE_DETAIL RuleDetail in RuleDetails)
            {
                if (RuleDetail.INPUTTYPE == "PREFIX" || RuleDetail.INPUTTYPE == "YYYY" || RuleDetail.INPUTTYPE == "MM" || RuleDetail.INPUTTYPE == "DD"
                    || RuleDetail.INPUTTYPE == "WW")
                {
                    UnChanged += RuleDetail.CURVALUE.Length;
                }
                else if (RuleDetail.INPUTTYPE == "FIXCODE")
                {
                    FixFlag = true;
                }
                else if (RuleDetail.INPUTTYPE == "SN")
                {
                    Changed += RuleDetail.CURVALUE.Length;
                    break;
                }
                
            }
            //截取流水碼
            Prefix = StartSn.Substring(0, UnChanged);
            Suffix = StartSn.Substring(UnChanged + Changed);
            

            //將流水碼轉換成十進制數
            C_SN_RULE_DETAIL detail = RuleDetails.Find(t => t.INPUTTYPE == "SN");
            List<C_CODE_MAPPING> CodeMappings = DB.ORM.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == detail.CODETYPE).ToList();
            int Count = CodeMappings.Count;
            if (FixFlag)
            {
                //獲得固定碼的規則對象
                C_SN_RULE_DETAIL FixDetail = RuleDetails.Find(t => t.INPUTTYPE == "FIXCODE");
                //獲得開始流水號部分的固定碼+流水碼
                string start = StartSn.Substring(UnChanged, FixDetail.CURVALUE.Length + Changed);
                //獲得開始流水號的固定碼部分
                string FixPart = start.Substring(0, FixDetail.CURVALUE.Length);
                //計算固定碼部分代表的十進制值
                int FixValue=(int)(int.Parse(FixPart)*Math.Pow(Count,Changed));
                //獲得開始流水號的流水碼部分
                string ChangedPart = start.Substring(FixDetail.CURVALUE.Length, Changed);
                int ChangedValue = 0;
                //計算流水碼部分代表的十進制值
                for (int i = 0; i < Changed; i++)
                {
                    ChangedValue += (int)(int.Parse(CodeMappings.Find(t=>t.CODEVALUE==ChangedPart[i].ToString()).VALUE) * Math.Pow(Count, Changed - 1));
                }
                //計算開始流水號的十進制值
                int Value = FixValue+ChangedValue;
                string tempCode = string.Empty;
                string fixValue = string.Empty;
                
                //獲得固定碼的CodeMapping對象
                List<C_CODE_MAPPING> FixCodeMappings= DB.ORM.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == FixDetail.CODETYPE).ToList();
                for (int i = 0; i < Quantity; i++)
                {
                    int a = 0;
                    //計算每個流水號對應的流水碼部分，并返回對應的固定碼部分值
                    tempCode = GetCodeWithFix(Changed, CodeMappings, Value+i, out a);
                    fixValue = FixCodeMappings.Find(t => t.VALUE == a.ToString()).CODEVALUE;
                    RegionDetails.Add(new R_WO_REGION_DETAIL()
                    {
                        ID = GetNewID(Bu, DB),
                        WORKORDERNO = WorkOrder,
                        TYPE = Type,
                        SN = Prefix + fixValue + tempCode + Suffix,
                        SEQ = (seq + i - 1).ToString(),
                        SUB_SN = Prefix + fixValue + tempCode + Suffix,
                        SUB_SEQ = (seq + i - 1).ToString(),
                        USE_FLAG = "0",
                        EDIT_TIME = dt,
                        EDIT_EMP = Emp
                    });


                }
            }
            else
            {
                StartCode = StartSn.Substring(UnChanged, Changed);
                EndCode = EndSn.Substring(UnChanged, Changed);
                char[] StartArray = StartCode.ToCharArray();
                for (int i = 0; i < StartArray.Length; i++)
                {
                    char c = StartArray[i];
                    StartValue += Convert.ToInt32(Int32.Parse(CodeMappings.Find(t => t.CODEVALUE == c.ToString()).VALUE) * Math.Pow(Count, Changed - i - 1));
                }

                //插入到 R_WO_REGION_DETAIL 表中
                detail.VALUE10 = StartValue.ToString();
                for (int j = 1; j <= Quantity; j++)
                {
                    Code = GetNextCode(detail, CodeMappings, j - 1);
                    RegionDetails.Add(new R_WO_REGION_DETAIL()
                    {
                        ID = GetNewID(Bu, DB),
                        WORKORDERNO = WorkOrder,
                        TYPE = Type,
                        SN = Prefix + Code + Suffix,
                        SEQ = (seq + j - 1).ToString(),
                        SUB_SN = Prefix + Code + Suffix,
                        SUB_SEQ = (seq + j - 1).ToString(),
                        USE_FLAG = "0",
                        EDIT_TIME = dt,
                        EDIT_EMP = Emp
                    });
                }
            }
            DB.ORM.Insertable<R_WO_REGION_DETAIL>(RegionDetails).ExecuteCommand();
        }
        /// <summary>
        /// 根據數量獲取對應的流水號
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="CodeMapping"></param>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public string GetNextCode(C_SN_RULE_DETAIL detail,List<C_CODE_MAPPING> CodeMapping,int Quantity)
        {
            int curValue = int.Parse(detail.VALUE10);
            curValue += Quantity;
            int T = CodeMapping.Count;
            string seq = "";

            while (curValue / T != 0)
            {
                int R = curValue % T;
                seq = CodeMapping[R].CODEVALUE + seq;
                curValue = curValue / T;
            }
            seq = CodeMapping[curValue].CODEVALUE + seq;
            if (seq.Length < detail.CURVALUE.Length)
            {
                for (int k = 0; detail.CURVALUE.Length != seq.Length; k++)
                {
                    seq = "0" + seq;
                }
            }
            if (seq.Length > detail.CURVALUE.Length)
            {
                throw new Exception("生成的SN超過最大值!");
            }
            return seq;
        }


        /// <summary>
        /// 檢查工單區間是否存在
        /// </summary>
        /// <param name="wo_no"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckDataExist(string wo_no, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO='{wo_no}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }
        public List<R_WO_REGION> ShowAllData(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            sql = $@"SELECT * FROM R_WO_REGION order by EDIT_TIME";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }
        public List<R_WO_REGION> GetWObyWONO(string WO, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO='{WO}'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }

        //对查询整个表的数据进行分页
        public List<R_WO_REGION> ShowAllDataAndShowPage(OleExec DB,string strWorkOrder,int CurrentPage, int PageSize, out int TotalData ) 
        {
            string strSql = string.Empty;
            bool isGetAll = true;
            DataTable dt = new DataTable();
            OleDbParameter[] paramet;
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            strSql = $@" select count(*) from r_wo_region a ";

            if (strWorkOrder.Length > 0)
            {
                strSql = strSql + $@"where upper(a.workorderno) like'%{strWorkOrder}%'";
                isGetAll = false;
            }
            TotalData = Convert.ToInt32(DB.ExecuteScalar(strSql, CommandType.Text));
            strSql = $@"select * from (select rownum rnumber,a.* from r_wo_region a ";
            if (isGetAll)
            {
                strSql = strSql + " order by edit_time desc)  where rnumber>((:CurrentPage-1)*:PageSize) and rnumber<=((:CurrentPage1-1)*:PageSize1+:PageSize2) order by edit_time desc";
                //oldb 的參數只能是按照順序對應，不能復用，
                paramet = new OleDbParameter[] {
                    new OleDbParameter(":CurrentPage", CurrentPage),
                    new OleDbParameter(":PageSize", PageSize),
                    new OleDbParameter(":CurrentPage1", CurrentPage),
                    new OleDbParameter(":PageSize1", PageSize),
                    new OleDbParameter(":PageSize2", PageSize)
                };
                dt = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            }
            else
            {
                strSql = strSql + $@" where  upper(a.workorderno) like'%{strWorkOrder}%' order by edit_time desc) where rnumber>((:CurrentPage-1)*:PageSize) and rnumber<=((:CurrentPage1-1)*:PageSize1+:PageSize2) order by edit_time desc";
                //oldb 的參數只能是按照順序對應，不能復用，
                paramet = new OleDbParameter[] {
                    new OleDbParameter(":CurrentPage", CurrentPage),
                    new OleDbParameter(":PageSize", PageSize),
                    new OleDbParameter(":CurrentPage1", CurrentPage),
                    new OleDbParameter(":PageSize1", PageSize),
                    new OleDbParameter(":PageSize2", PageSize)
                };
                dt = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            }

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Row_R_WO_REGION ret = (Row_R_WO_REGION)NewRow();
                    ret.loadData(dt.Rows[i]);
                    LanguageList.Add(ret.GetDataObject());
                }
                return LanguageList;
            }
            else
            {
                return null;
            }
        }

        public List<R_WO_REGION> CheckZone(string min, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            sql = $@"SELECT * FROM R_WO_REGION WHERE '{min}' BETWEEN MIN_SN AND MAX_SN";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }

        /// <summary>
        /// 查詢SN所在的工單區間
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_WO_REGION> ShowWORegionBySN(string sn, OleExec DB) {
            string sql = string.Empty;
            List<R_WO_REGION> WORegionList = new List<R_WO_REGION>();
            DataTable dt = new DataTable();
            sql = $@"Select * From R_WO_Region 
                     Where 1=1 and :strStartSN>=Min_SN and :strEndSN<=Max_SN and 
                     Length(:strStartSN)=Length(Min_SN)";
            OleDbParameter[] parameter = new OleDbParameter[3];          
            parameter[0] = new OleDbParameter(":strStartSN", sn);
            parameter[1] = new OleDbParameter(":strEndSN", sn);
            parameter[2] = new OleDbParameter(":strStartSN", sn);
            dt = DB.ExecuteDataTable(sql, CommandType.Text, parameter);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Row_R_WO_REGION ret = (Row_R_WO_REGION)NewRow();
                    ret.loadData(dt.Rows[i]);
                    WORegionList.Add(ret.GetDataObject());
                }                
            }
            return WORegionList;
        }

        /// <summary>
        /// 檢查SN是否在工單SN區間，存在返回TRUE，不存在返回FALSE
        /// 黄杨盛 2018年4月14日16:18:38 修正条码字数长度小于预配区间条码字数长度的时候会返回true的bug
        /// Eden 2018年4月27日16:49:38未配置工單區間返回FALSE
        /// Champion 2019/08/08 只靠比對是否在區間會有問題，如果輸入的 SN 有特殊字符例如 ! 也會使得判斷為 true，因此再到 R_WO_REGION_DETAIL 表中查詢
        /// 方國剛 2019/08/13  HWD,VERTIV,HWT沒有R_WO_REGION_DETAIL這個表 
        /// </summary>
        /// <param name="strSN"></param>
        /// <param name="strWo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckSNInWoRange(string strSN,string strWo, OleExec DB,string BU)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_WO_REGION> LanguageList = new List<R_WO_REGION>();
            T_R_WO_REGION_DETAIL TRWRD = new T_R_WO_REGION_DETAIL(DB, this.DBType);
            sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO='{strWo}'";
            dt = DB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count > 0)
            {

                //sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO=:strwo AND :strsn BETWEEN MIN_SN AND MAX_SN AND LENGTH(:strsn)<=LENGTH(MAX_SN)";
                //OleDbParameter[] paramet = new OleDbParameter[3];
                //paramet[0] = new OleDbParameter(":strwo", strWo);
                //paramet[1] = new OleDbParameter(":strsn", strSN);
                //paramet[2] = new OleDbParameter(":strsn", strSN);
                //dt = DB.ExecuteDataTable(sql, CommandType.Text, paramet);
                sql = $@"SELECT * FROM R_WO_REGION WHERE WORKORDERNO='{strWo}' AND '{strSN}' BETWEEN MIN_SN AND MAX_SN AND (LENGTH('{strSN}')=LENGTH(MAX_SN) and LENGTH('{strSN}')=LENGTH(MIN_SN) )";
                dt = DB.ExecuteDataTable(sql, CommandType.Text);
                if (BU == "HWD" || BU == "VERTIV" || BU == "HWT")
                {
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (dt.Rows.Count > 0 && TRWRD.CheckSnInRegionDetail(strWo, strSN, DB))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
          
        }
        /// <summary>
        /// 檢查StartSN&EndSN是否在工單區間內，存在返回TRUE，不存在返回FALSE
        /// 2018/1/23 Rain
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="strWo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckLotSNInWoRange(string strStartSN, string strEndSN,string strWo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"Select * From R_WO_Region 
                     Where Workorderno=:strwo and :strStartSN>=Min_SN and :strEndSN<=Max_SN and 
                     Length(:strStartSN)=Length(Min_SN)";
            OleDbParameter[] parameter = new OleDbParameter[4];
            parameter[0] = new OleDbParameter(":strwo", strWo);
            parameter[1] = new OleDbParameter(":strStartSN", strStartSN);
            parameter[2] = new OleDbParameter(":strEndSN", strEndSN);
            parameter[3] = new OleDbParameter(":strStartSN", strStartSN);
            dt = DB.ExecuteDataTable(sql, CommandType.Text, parameter);
            if (dt.Rows.Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 條碼后4位流水碼為34進制
        /// 取得條碼StartSN&EndSN區間內包含的序號數量
        /// 2018/1/24 Rain
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int GetQtyBy34HSNRange(string strStartSN, string strEndSN,  OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" SELECT 
      ((case   when (ascii(substr(:strEndSN,1,1)) between 48 and 57) then to_number(substr(:strEndSN,1,1)) 
              when (ascii(substr(:strEndSN,1,1)) between 65 and 72) then ascii(substr(:strEndSN,1,1))-55
              when (ascii(substr(:strEndSN,1,1)) between 74 and 78) then ascii(substr(:strEndSN,1,1))-56
              when (ascii(substr(:strEndSN,1,1)) between 80 and 90) then ascii(substr(:strEndSN,1,1))-57 else 0 end)*34*34*34+
      (case   when (ascii(substr(:strEndSN,2,1)) between 48 and 57) then to_number(substr(:strEndSN,2,1)) 
              when (ascii(substr(:strEndSN,2,1)) between 65 and 72) then ascii(substr(:strEndSN,2,1))-55
              when (ascii(substr(:strEndSN,2,1)) between 74 and 78) then ascii(substr(:strEndSN,2,1))-56
              when (ascii(substr(:strEndSN,2,1)) between 80 and 90) then ascii(substr(:strEndSN,2,1))-57 else 0 end)*34*34+
      (case   when (ascii(substr(:strEndSN,3,1)) between 48 and 57) then to_number(substr(:strEndSN,3,1)) 
              when (ascii(substr(:strEndSN,3,1)) between 65 and 72) then ascii(substr(:strEndSN,3,1))-55
              when (ascii(substr(:strEndSN,3,1)) between 74 and 78) then ascii(substr(:strEndSN,3,1))-56
              when (ascii(substr(:strEndSN,3,1)) between 80 and 90) then ascii(substr(:strEndSN,3,1))-57 else 0 end)*34+
      (case   when (ascii(substr(:strEndSN,4,1)) between 48 and 57) then to_number(substr(:strEndSN,4,1)) 
              when (ascii(substr(:strEndSN,4,1)) between 65 and 72) then ascii(substr(:strEndSN,4,1))-55
              when (ascii(substr(:strEndSN,4,1)) between 74 and 78) then ascii(substr(:strEndSN,4,1))-56
              when (ascii(substr(:strEndSN,4,1)) between 80 and 90) then ascii(substr(:strEndSN,4,1))-57 else 0 end)) -
      ((case   when (ascii(substr(:strStartSN,1,1)) between 48 and 57) then to_number(substr(:strStartSN,1,1)) 
              when (ascii(substr(:strStartSN,1,1)) between 65 and 72) then ascii(substr(:strStartSN,1,1))-55
              when (ascii(substr(:strStartSN,1,1)) between 74 and 78) then ascii(substr(:strStartSN,1,1))-56
              when (ascii(substr(:strStartSN,1,1)) between 80 and 90) then ascii(substr(:strStartSN,1,1))-57 else 0 end)*34*34*34+
      (case   when (ascii(substr(:strStartSN,2,1)) between 48 and 57) then to_number(substr(:strStartSN,2,1)) 
              when (ascii(substr(:strStartSN,2,1)) between 65 and 72) then ascii(substr(:strStartSN,2,1))-55
              when (ascii(substr(:strStartSN,2,1)) between 74 and 78) then ascii(substr(:strStartSN,2,1))-56
              when (ascii(substr(:strStartSN,2,1)) between 80 and 90) then ascii(substr(:strStartSN,2,1))-57 else 0 end)*34*34+
      (case   when (ascii(substr(:strStartSN,3,1)) between 48 and 57) then to_number(substr(:strStartSN,3,1)) 
              when (ascii(substr(:strStartSN,3,1)) between 65 and 72) then ascii(substr(:strStartSN,3,1))-55
              when (ascii(substr(:strStartSN,3,1)) between 74 and 78) then ascii(substr(:strStartSN,3,1))-56
              when (ascii(substr(:strStartSN,3,1)) between 80 and 90) then ascii(substr(:strStartSN,3,1))-57 else 0 end)*34+
      (case   when (ascii(substr(:strStartSN,4,1)) between 48 and 57) then to_number(substr(:strStartSN,4,1)) 
              when (ascii(substr(:strStartSN,4,1)) between 65 and 72) then ascii(substr(:strStartSN,4,1))-55
              when (ascii(substr(:strStartSN,4,1)) between 74 and 78) then ascii(substr(:strStartSN,4,1))-56
              when (ascii(substr(:strStartSN,4,1)) between 80 and 90) then ascii(substr(:strStartSN,4,1))-57 else 0 end)) +1  Qty          
      from dual";
            OleDbParameter[] parameter = new OleDbParameter[64];
            for (int i = 0; i <= 31; i++)
            {
                parameter[i] = new OleDbParameter(":strEndSN", strEndSN);
            }
            for (int i = 32; i <= 63; i++)
            {
                parameter[i] = new OleDbParameter(":strStartSN", strStartSN);
            }
            dt = DB.ExecuteDataTable(sql, CommandType.Text, parameter);
            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["Qty"].ToString());
            }
            else
            {
                return -1;
            }

        }

        public R_WO_REGION CreateLanguageClass(DataRow dr)
        {
            Row_R_WO_REGION row = (Row_R_WO_REGION)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        /// <summary>
        /// add by fgg 2018.05.17
        /// HWD PE 杜軍要求配置工單區間所輸入的字符必須是字母或數字
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="notMatch">out not match string</param>
        /// <returns>bool</returns>
        public bool InputIsStringOrNum(string input,out string notMatch)
        {
            string regexRule = "[A-Za-z0-9]";
            string outString = "";
            bool isStringOrNum = true;
            char[] inputArray = input.ToArray();
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (!Regex.IsMatch(inputArray[i].ToString(), regexRule))
                {                   
                    isStringOrNum = false;
                    outString = inputArray[i].ToString();
                    break;
                }
            }
            notMatch = outString;            
            return isStringOrNum;
        }

        /// <summary>
        ///  add by fgg 2018.05.17
        /// HWD PE 杜軍要求配置工單區間所輸入的SN必須符合機種設置的SN規則
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sku"></param>
        /// <returns></returns>
        public bool InputIsMatchSkuRule(string input,C_SKU sku)
        {
            bool isMatchSkuRule = true;
            string[] ruleArray;
            string[] snArray;
            char[] charConfigRule = sku.SN_RULE.ToCharArray();
            ruleArray = new string[charConfigRule.Length];

            if (sku.SN_RULE == "")
            {
                return true;
            }
            //長度不符
            if(input.Length!=sku.SN_RULE.Length)
            {
                return false;
            }

            for (int i = 0; i < charConfigRule.Length; i++)
            {
                ruleArray[i] = charConfigRule[i].ToString();
            }

            char[] charSn = input.ToCharArray();
            snArray = new string[charSn.Length];
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
                    isMatchSkuRule = false;
                    break;
                }
            }            
            return isMatchSkuRule;
        }

        /// <summary>    
        /// Check if the SN is in the configured interval
        /// add by fgg 2018.11.14
        /// </summary>      
        /// <param name="sn"></param>
        /// <param name="wo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckSNInRange(string sn,ref string wo, OleExec DB)
        {
            R_WO_REGION r_wo_region = DB.ORM.Queryable<R_WO_REGION>().Where(r => SqlSugar.SqlFunc.Between(sn, r.MIN_SN, r.MAX_SN)
                   && SqlSugar.SqlFunc.Length(sn) == SqlSugar.SqlFunc.Length(r.MIN_SN)
                   && SqlSugar.SqlFunc.Length(sn) == SqlSugar.SqlFunc.Length(r.MAX_SN)).ToList().FirstOrDefault();
            if (r_wo_region != null)
            {
                wo = r_wo_region.WORKORDERNO;
                return true;
            }
            else
            {
                wo = "";
                return false;
            }
        }
    
        /// <summary>
        /// check maxsn is greater than or equal to minsn
        /// </summary>
        /// <param name="maxSN"></param>
        /// <param name="minSn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CompareLastCode(string maxSn, string minSn, int length)
        {
            int maxCode = Convert.ToInt32(maxSn.Substring(maxSn.Length - length, length));
            int minCode = Convert.ToInt32(minSn.Substring(minSn.Length - length, length));
            if (maxCode >= minCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Row_R_WO_REGION : DataObjectBase
    {
        public Row_R_WO_REGION(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_REGION GetDataObject()
        {
            R_WO_REGION DataObject = new R_WO_REGION();
            DataObject.ID = this.ID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.QTY = this.QTY;
            DataObject.MIN_SN = this.MIN_SN;
            DataObject.MAX_SN = this.MAX_SN;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string MIN_SN
        {
            get
            {
                return (string)this["MIN_SN"];
            }
            set
            {
                this["MIN_SN"] = value;
            }
        }
        public string MAX_SN
        {
            get
            {
                return (string)this["MAX_SN"];
            }
            set
            {
                this["MAX_SN"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_WO_REGION
    {
        public string ID{get;set;}
        public string WORKORDERNO{get;set;}
        public string SKUNO{get;set;}
        public double? QTY{get;set;}
        public string MIN_SN{get;set;}
        public string MAX_SN{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime EDIT_TIME{get;set;}
    }

    public class T_R_WO_REGION_DETAIL: DataObjectTable
    {
        public T_R_WO_REGION_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_REGION_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_REGION);
            TableName = "R_WO_REGION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_WO_REGION_DETAIL GetBySn(string Sn, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_REGION_DETAIL>().Where(t => t.SN == Sn).ToList().FirstOrDefault();
        }

        public int GetRestCount(string WorkOrder, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_REGION_DETAIL>().Where(t => t.WORKORDERNO == WorkOrder && t.USE_FLAG == "0").ToList().Count;
        }

        public List<R_WO_REGION_DETAIL> GetPrintSn(string WorkOrder, int Count, OleExec DB, bool IsPanel = false)
        {
            var query = DB.ORM.Queryable<R_WO_REGION_DETAIL>().Where(t => t.WORKORDERNO == WorkOrder && t.USE_FLAG == "0");
            if (IsPanel)
            {
                query = query.Where(t => t.TYPE == "PANEL_SN");
            }
            return query.OrderBy(t => SqlSugar.SqlFunc.ToInt32(t.SEQ)).Take(Count).ToList();
        }

        public int DoAfterPrint(R_WO_REGION_DETAIL detail, OleExec DB)
        {
            return DB.ORM.Updateable<R_WO_REGION_DETAIL>(detail).UpdateColumns(t => new { t.USE_FLAG, t.EDIT_TIME, t.EDIT_EMP }).Where(t => t.ID == detail.ID).ExecuteCommand();
        }

        public int DoAfterPrint(List<R_WO_REGION_DETAIL> details, OleExec DB)
        {
            int result = 0;
            foreach (R_WO_REGION_DETAIL detail in details)
            {
                result += DoAfterPrint(detail, DB);
            }
            return result;
        }

        public bool CheckSNExist(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_REGION_DETAIL>().Any(t => t.SN == SN);
        }

        public bool CheckSnInRegionDetail(string Workorder, string Sn,OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_REGION_DETAIL>().Any(t => t.SN == Sn && t.WORKORDERNO == Workorder);
        }

        public bool CheckSNHasPrinted(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_REGION_DETAIL>().Any(t => t.SN == SN && t.USE_FLAG != "0");
        }
    }

    public class R_WO_REGION_DETAIL
    {
        public string ID { get; set; }
        public string WORKORDERNO { get; set; }
        public string TYPE { get; set; }
        public string SN { get; set; }
        public string SEQ { get; set; }
        public string SUB_SN { get; set; }
        public string SUB_SEQ { get; set; }
        public string USE_FLAG { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }

        
    }

    public class WoRangeMainPage
    {
        public List<R_WO_REGION> WoRangeData = new List<R_WO_REGION>();
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int CountPage { get; set; }
    }
}