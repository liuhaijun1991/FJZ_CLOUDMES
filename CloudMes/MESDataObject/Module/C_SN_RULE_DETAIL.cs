using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SN_RULE_DETAIL : DataObjectTable
    {
        public T_C_SN_RULE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SN_RULE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SN_RULE_DETAIL);
            TableName = "C_SN_RULE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_SN_RULE_DETAIL> GetDataByRuleID(string RuleID, OleExec DB)
        {
            //List<Row_C_SN_RULE_DETAIL> RET = null;
            //string strSql = $@"select * from C_SN_RULE_DETAIL c where c.c_sn_rule_id = '{RuleID}' order by seq ";
            //DataSet res = DB.RunSelect(strSql);
            //for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            //{
            //    if (i == 0)
            //    {
            //        RET = new List<Row_C_SN_RULE_DETAIL>();
            //    }
            //    Row_C_SN_RULE_DETAIL R = (Row_C_SN_RULE_DETAIL)NewRow();
            //    R.loadData(res.Tables[0].Rows[i]);
            //    RET.Add(R);
            //}

            //return RET;
            return DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == RuleID).OrderBy(t => t.SEQ).ToList();

        }

        public C_SN_RULE_DETAIL GetRuleDetailByDetailId(string DetailId, OleExec DB)
        {
            return DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.ID == DetailId).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 添加編碼規則細項，
        /// 正常添加完編碼規則細項之後,檢查當前形成的編碼格式是否與已有的一致，防止重碼
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddRuleDetail(C_SN_RULE_DETAIL detail,string Bu,string Emp, OleExec DB)
        {
            int Count = 0;
            if (detail != null)
            {
                C_SN_RULE_DETAIL LastRuleDetail = DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == detail.C_SN_RULE_ID).OrderBy(t => t.SEQ, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                if (LastRuleDetail == null)
                {
                    detail.SEQ = 1;
                }
                else
                {
                    detail.SEQ = LastRuleDetail.SEQ + 1;
                }
                detail.EDIT_TIME = GetDBDateTime(DB);
                detail = CompleteDetail(detail,Bu, DB);
                Count = DB.ORM.Insertable<C_SN_RULE_DETAIL>(detail).ExecuteCommand();
                DealWithComplicate(detail, DB);
                UpdateRuleValue(detail, Emp, DB);
            }
            return Count;
        }

        public int AddRuleDetailByCopy(C_SN_RULE_DETAIL detail, string Bu, OleExec DB)
        {
            int Count = 0;
            if (detail != null)
            {
                detail.ID = GetNewID(Bu, DB);
                if (detail.INPUTTYPE == "SN")
                {
                    var tmp = "0";
                    var snLen = detail.CURVALUE.Length;
                    while (snLen > 1)
                    {
                        tmp = tmp + "0";
                        snLen--;
                    }
                    //根據參考機種編碼規則中流水碼的長度重置
                    detail.CURVALUE = tmp;
                    detail.VALUE10 = detail.RESETVALUE;
                }
                detail.EDIT_TIME = GetDBDateTime(DB);
                Count = DB.ORM.Insertable(detail).ExecuteCommand();
            }
            return Count;
        }

        /// <summary>
        /// 更新編碼規則細項，
        /// 更新之後也要判斷是否存在相同的編碼規則
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateRuleDetail(C_SN_RULE_DETAIL detail,string Emp, OleExec DB)
        {
            int Count = 0;
            Count = DB.ORM.Updateable<C_SN_RULE_DETAIL>(detail).Where(t => t.ID == detail.ID).ExecuteCommand();
            DealWithComplicate(detail, DB);
            UpdateRuleValue(detail, Emp, DB);
            return Count;
        }

        public void UpdateRuleValue(C_SN_RULE_DETAIL detail,string Emp, OleExec DB)
        {
            C_SN_RULE Rule = DB.ORM.Queryable<C_SN_RULE>().Where(t => t.ID == detail.C_SN_RULE_ID).ToList().FirstOrDefault();
            if (Rule != null)
            {
                StringBuilder sb = new StringBuilder();
                List<C_SN_RULE_DETAIL> RuleDetails = DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == detail.C_SN_RULE_ID).OrderBy(t => t.SEQ).ToList();
                foreach (C_SN_RULE_DETAIL rd in RuleDetails)
                {
                    sb.Append(rd.CURVALUE);
                }
                Rule.CURVALUE = sb.ToString();
                Rule.EDIT_TIME = GetDBDateTime(DB);
                Rule.EDIT_EMP = Emp;
                DB.ORM.Updateable<C_SN_RULE>(Rule).Where(t => t.ID == detail.C_SN_RULE_ID).ExecuteCommand();
            }
        }

        /// <summary>
        /// 刪除編碼規則細項，
        /// 刪除之後也要判斷是否存在相同的編碼規則
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteRuleDetail(string RuleDetailId,string Emp, OleExec DB)
        {
            int Count = 0;
            C_SN_RULE_DETAIL RuleDetail = DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.ID == RuleDetailId).ToList().FirstOrDefault();
            Count = DB.ORM.Deleteable<C_SN_RULE_DETAIL>().Where(t => t.ID == RuleDetailId).ExecuteCommand();
            DealWithComplicate(RuleDetail, DB);
            UpdateRuleValue(RuleDetail, Emp, DB);
            return Count;
        }

        public void DealWithComplicate(C_SN_RULE_DETAIL detail, OleExec DB)
        {
            string ComplicateId = GetComplicateRuleId(detail, DB);
            if (ComplicateId.Length > 0)
            {
                C_SN_RULE Rule = DB.ORM.Queryable<C_SN_RULE>().Where(t => t.ID == ComplicateId).ToList().FirstOrDefault();
                DB.ORM.Deleteable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == detail.C_SN_RULE_ID).ExecuteCommand();
                throw new Exception(string.Format("存在相同的編碼規則，規則名為 {0},您之前添加的規則細項都已經被刪除！", Rule.NAME));
            }
        }

        /// <summary>
        /// 檢查相同格式的編碼規則
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetComplicateRuleId(C_SN_RULE_DETAIL detail, OleExec DB)
        {
            //獲取當前編碼規則的完整格式
            List<C_SN_RULE_DETAIL> details = DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == detail.C_SN_RULE_ID).ToList();
            //獲取不同的編碼格式分組
            List<string> DetailGroups= DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t=>t.C_SN_RULE_ID!=detail.C_SN_RULE_ID).GroupBy(t=>t.C_SN_RULE_ID).Select(t=>t.C_SN_RULE_ID).ToList();
            foreach (string DetailGroup in DetailGroups)
            {
                List<C_SN_RULE_DETAIL> ExistDetails = DB.ORM.Queryable<C_SN_RULE_DETAIL>().Where(t => t.C_SN_RULE_ID == DetailGroup).ToList();
                int SameCounts = 0;
                //if (!DetailGroup.Equals("VNDCN000000000000000000000000000001"))
                //    continue;
                if (details.Count == ExistDetails.Count)
                {
                    foreach (C_SN_RULE_DETAIL d in details)
                    {
                        C_SN_RULE_DETAIL ed = ExistDetails.Find(t => t.INPUTTYPE == d.INPUTTYPE);
                        //如果是固定值或者年的話，就在 C_SN_RULE_DETAIL 中找是否有某條記錄固定值和當前一致，并且序號也一致
                        if (d.INPUTTYPE.Equals("PREFIX"))
                        {
                            if (ed != null && d.CURVALUE==ed.CURVALUE && d.SEQ==ed.SEQ)
                            {
                                SameCounts++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        //如果輸入類型是年，月，日，周的話，那麽就在 C_SN_RULE_DETAIL 中找是否有某條記錄的類型與當前一致，并且序號也一致
                        if (d.INPUTTYPE.Equals("YYYY") || d.INPUTTYPE.Equals("MM") || d.INPUTTYPE.Equals("DD") || d.INPUTTYPE.Equals("WW"))
                        {
                            if (ed != null && d.CODETYPE == ed.CODETYPE && d.SEQ==ed.SEQ)
                            {
                                SameCounts++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        //如果輸入類型是 SN 即流水碼格式，那麽判斷是否存在記錄與當前記錄的 CODETYPE 和 流水碼長度以及序號都一致的記錄
                        if (d.INPUTTYPE.Equals("SN"))
                        {
                            if (ed != null && d.CODETYPE == ed.CODETYPE && d.CURVALUE.Length == ed.CURVALUE.Length && d.SEQ == ed.SEQ)
                            {
                                SameCounts++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                    }

                    if (SameCounts == ExistDetails.Count)
                    {
                        return DetailGroup;
                    }
                    //else
                    //{
                    //    return "";
                    //}
                }
            }
            return "";

        }

        /// <summary>
        /// 填充編碼規則細項，填充 C_SN_RULE_DETAIL 的 CURVALUE 和 VALUE10
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_SN_RULE_DETAIL CompleteDetail(C_SN_RULE_DETAIL detail,string Bu,OleExec DB)
        {
            detail.ID = GetNewID(Bu, DB);
            if (detail.INPUTTYPE.Equals("PREFIX"))
            {
                detail.VALUE10 = "";
            }
            else if (detail.INPUTTYPE.Equals("YYYY") || detail.INPUTTYPE.Equals("MM") || detail.INPUTTYPE.Equals("DD") || detail.INPUTTYPE.Equals("WW"))
            {
                switch (detail.INPUTTYPE)
                {
                    case "YYYY":
                        detail.VALUE10 = DateTime.Now.Year.ToString();
                        break;
                    case "MM":
                        detail.VALUE10 = DateTime.Now.Month.ToString();
                        break;
                    case "DD":
                        detail.VALUE10 = DateTime.Now.Day.ToString();
                        break;
                    case "WW":
                        System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                        detail.VALUE10 = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                        break;
                }
                detail.CURVALUE = DB.ORM.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == detail.CODETYPE && t.VALUE == detail.VALUE10).ToList().FirstOrDefault().CODEVALUE;
            }
            else if (detail.INPUTTYPE.Equals("SN"))
            {                
                string CodeValue = string.Empty;
                int CurrentValue =  Int32.Parse(detail.VALUE10);               
                List<C_CODE_MAPPING> CodeMappings = DB.ORM.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == detail.CODETYPE).ToList();
                int Count = CodeMappings.Count;

                while (CurrentValue / Count != 0)
                {
                    int Seq = CurrentValue % Count;
                    CodeValue = CodeMappings.Find(t => t.SEQ == Seq).CODEVALUE + CodeValue;
                    CurrentValue = CurrentValue / Count;
                }
                if (CodeValue.Length == 0)
                {
                    CodeValue = detail.VALUE10;
                }
                detail.CURVALUE = CodeValue.PadLeft(detail.VALUE10.Length,'0');
            }
            
            return detail;
        }

        /// <summary>
        /// 邏輯未寫完
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_SN_RULE_DETAIL CompleteDetailNew(C_SN_RULE_DETAIL detail, string Bu, OleExec DB)
        {            
            if (detail.INPUTTYPE.Equals("PREFIX"))
            {
                detail.VALUE10 = "";
            }
            else if (detail.INPUTTYPE.Equals("YYYY") || detail.INPUTTYPE.Equals("MM") || detail.INPUTTYPE.Equals("DD") || detail.INPUTTYPE.Equals("WW"))
            {
                switch (detail.INPUTTYPE)
                {
                    case "YYYY":
                        detail.VALUE10 = DateTime.Now.Year.ToString();
                        break;
                    case "MM":
                        detail.VALUE10 = DateTime.Now.Month.ToString();
                        break;
                    case "DD":
                        detail.VALUE10 = DateTime.Now.Day.ToString();
                        break;
                    case "WW":
                        System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
                        detail.VALUE10 = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString();
                        break;
                }
                detail.CURVALUE = DB.ORM.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == detail.CODETYPE && t.VALUE == detail.VALUE10).ToList().FirstOrDefault().CODEVALUE;
            }
            else if (detail.INPUTTYPE.Equals("SN"))
            {
                string CodeValue = string.Empty;
                double value10 = 0;
                var curvalue = detail.CURVALUE.TrimStart('0');
                List<C_CODE_MAPPING> mappingList = DB.ORM.Queryable<C_CODE_MAPPING>().Where(t => t.CODETYPE == detail.CODETYPE).OrderBy(t=>t.SEQ).ToList();
                for (int i = 0; i < curvalue.Length; i++)
                {
                    var current = detail.CURVALUE.Substring(detail.CURVALUE.Length - i - 1, 1);
                    var currentIndex = mappingList.FindIndex(r => r.CODEVALUE == current);
                    value10 += Math.Pow(mappingList.Count, i) * currentIndex;
                }
                detail.VALUE10 = (value10 - 1).ToString();
            }

            return detail;
        }

        public List<string> GetAllInputType(OleExec DB)
        {
            return DB.ORM.Queryable<C_SN_RULE_DETAIL>().GroupBy(t => t.INPUTTYPE).Select(t => t.INPUTTYPE).ToList();
        }

        public List<string> GetAllCodeType(OleExec DB)
        {
            return DB.ORM.Queryable<C_CODE_MAPPING>().GroupBy(t => t.CODETYPE).Select(t => t.CODETYPE).ToList();
        }

        public int SaveSnRuleDetail(C_SN_RULE_DETAIL detailObj, string bu, string emp, OleExec SFCDB)
        {
            int result = 0;
            if (SFCDB.ORM.Queryable<C_SN_RULE_DETAIL>().Any(r => r.ID == detailObj.ID))
            {
                detailObj = CompleteDetailNew(detailObj, bu, SFCDB);
                result = SFCDB.ORM.Updateable<C_SN_RULE_DETAIL>(detailObj).Where(r => r.ID == detailObj.ID).ExecuteCommand();
            }
            else
            {
                detailObj.ID = GetNewID(bu, SFCDB);
                detailObj = CompleteDetailNew(detailObj, bu, SFCDB);
                result = SFCDB.ORM.Insertable<C_SN_RULE_DETAIL>(detailObj).ExecuteCommand();
            }
            UpdateRuleValue(detailObj, emp, SFCDB);
            return result;
        }
    }
    public class Row_C_SN_RULE_DETAIL : DataObjectBase
    {
        public Row_C_SN_RULE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public void LockMe(OleExec DB)
        {
            string strSql = $@"select * from C_SN_RULE_DETAIL where ID = '{ID}' for update";
            DataSet res = DB.RunSelect(strSql);
            loadData(res.Tables[0].Rows[0]);
        }

        public C_SN_RULE_DETAIL GetDataObject()
        {
            C_SN_RULE_DETAIL DataObject = new C_SN_RULE_DETAIL();
            DataObject.VALUE10 = this.VALUE10;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.CHECK_FLAG = this.CHECK_FLAG;
            DataObject.RESETVALUE = this.RESETVALUE;
            DataObject.RESETSN_FLAG = this.RESETSN_FLAG;
            DataObject.CURVALUE = this.CURVALUE;
            DataObject.CODETYPE = this.CODETYPE;
            DataObject.INPUTTYPE = this.INPUTTYPE;
            DataObject.SEQ = this.SEQ;
            DataObject.C_SN_RULE_ID = this.C_SN_RULE_ID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string VALUE10
        {
            get
            {
                return (string)this["VALUE10"];
            }
            set
            {
                this["VALUE10"] = value;
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
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public double? CHECK_FLAG
        {
            get
            {
                return (double?)this["CHECK_FLAG"];
            }
            set
            {
                this["CHECK_FLAG"] = value;
            }
        }
        public string RESETVALUE
        {
            get
            {
                return (string)this["RESETVALUE"];
            }
            set
            {
                this["RESETVALUE"] = value;
            }
        }
        public double? RESETSN_FLAG
        {
            get
            {
                return (double?)this["RESETSN_FLAG"];
            }
            set
            {
                this["RESETSN_FLAG"] = value;
            }
        }
        public string CURVALUE
        {
            get
            {
                return (string)this["CURVALUE"];
            }
            set
            {
                this["CURVALUE"] = value;
            }
        }
        public string CODETYPE
        {
            get
            {
                return (string)this["CODETYPE"];
            }
            set
            {
                this["CODETYPE"] = value;
            }
        }
        public string INPUTTYPE
        {
            get
            {
                return (string)this["INPUTTYPE"];
            }
            set
            {
                this["INPUTTYPE"] = value;
            }
        }
        public double? SEQ
        {
            get
            {
                return (double?)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
            }
        }
        public string C_SN_RULE_ID
        {
            get
            {
                return (string)this["C_SN_RULE_ID"];
            }
            set
            {
                this["C_SN_RULE_ID"] = value;
            }
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
    }
    public class C_SN_RULE_DETAIL
    {
        public string VALUE10{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public double? CHECK_FLAG{get;set;}
        public string RESETVALUE{get;set;}
        public double? RESETSN_FLAG{get;set;}
        public string CURVALUE{get;set;}
        public string CODETYPE{get;set;}
        public string INPUTTYPE{get;set;}
        public double? SEQ{get;set;}
        public string C_SN_RULE_ID{get;set;}
        public string ID{get;set;}

        public void LockMe(OleExec DB)
        {
            SqlSugar.DbType DbType=DB.ORM.CurrentConnectionConfig.DbType;
            string sql = string.Empty;
            switch (DbType)
            {
                case SqlSugar.DbType.Oracle:
                    sql = $@"select * from C_SN_RULE_DETAIL where ID = '{ID}' for update";
                    break;
                case SqlSugar.DbType.MySql:
                    sql = $@"select * from C_SN_RULE_DETAIL where ID = '{ID}' for update";
                    break;
                case SqlSugar.DbType.SqlServer:
                    sql = $@"select * from C_SN_RULE_DETAIL with(rowlock,Updlock) where ID='{ID}'";
                    break;
                case SqlSugar.DbType.PostgreSQL:
                    sql = $@"select * from C_SN_RULE_DETAIL where ID='{ID}' for update nowait";
                    break;
                case SqlSugar.DbType.Sqlite:
                    break;
            }
            var o = DB.ORM.Ado.SqlQuery<C_SN_RULE_DETAIL>(sql).ToList();
            this.VALUE10 = o[0].VALUE10;
            EDIT_EMP = o[0].EDIT_EMP;
            EDIT_TIME = o[0].EDIT_TIME;
            CHECK_FLAG = o[0].CHECK_FLAG;
            RESETVALUE = o[0].RESETVALUE;
            RESETSN_FLAG = o[0].RESETSN_FLAG;
            CURVALUE = o[0].CURVALUE;
            CODETYPE = o[0].CODETYPE;
            INPUTTYPE = o[0].INPUTTYPE;
            SEQ = o[0].SEQ;
            C_SN_RULE_ID = o[0].C_SN_RULE_ID;
            ID = o[0].ID;
        }

        public void LockMe(SqlSugar.SqlSugarClient DB)
        {
            SqlSugar.DbType DbType = DB.CurrentConnectionConfig.DbType;
            string sql = string.Empty;
            switch (DbType)
            {
                case SqlSugar.DbType.Oracle:
                    sql = $@"select * from C_SN_RULE_DETAIL where ID = '{ID}' for update";
                    break;
                case SqlSugar.DbType.MySql:
                    sql = $@"select * from C_SN_RULE_DETAIL where ID = '{ID}' for update";
                    break;
                case SqlSugar.DbType.SqlServer:
                    sql = $@"select * from C_SN_RULE_DETAIL with(rowlock,Updlock) where ID='{ID}'";
                    break;
                case SqlSugar.DbType.PostgreSQL:
                    sql = $@"select * from C_SN_RULE_DETAIL where ID='{ID}' for update nowait";
                    break;
                case SqlSugar.DbType.Sqlite:
                    break;
            }
            var o = DB.Ado.SqlQuery<C_SN_RULE_DETAIL>(sql).ToList();
            this.VALUE10 = o[0].VALUE10;
            EDIT_EMP = o[0].EDIT_EMP;
            EDIT_TIME = o[0].EDIT_TIME;
            CHECK_FLAG = o[0].CHECK_FLAG;
            RESETVALUE = o[0].RESETVALUE;
            RESETSN_FLAG = o[0].RESETSN_FLAG;
            CURVALUE = o[0].CURVALUE;
            CODETYPE = o[0].CODETYPE;
            INPUTTYPE = o[0].INPUTTYPE;
            SEQ = o[0].SEQ;
            C_SN_RULE_ID = o[0].C_SN_RULE_ID;
            ID = o[0].ID;
        }
    }
}