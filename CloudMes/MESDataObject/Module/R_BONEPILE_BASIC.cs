using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BONEPILE_BASIC : DataObjectTable
    {
        public T_R_BONEPILE_BASIC(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BONEPILE_BASIC(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BONEPILE_BASIC);
            TableName = "R_BONEPILE_BASIC".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Insert(OleExec SFCDB, R_BONEPILE_BASIC obj)
        {
            return SFCDB.ORM.Insertable<R_BONEPILE_BASIC>(obj).ExecuteCommand();
        }

        public int Update(OleExec SFCDB, R_BONEPILE_BASIC obj)
        {
            return SFCDB.ORM.Updateable<R_BONEPILE_BASIC>(obj).Where(r => r.ID == obj.ID).ExecuteCommand();
        }

        public void GetWorkYearAndWeek(OleExec SFCDB, DateTime current_date, int year, int week, ref int workYear, ref int workWeek, ref string firstDateOfWorkWeek)
        {
            string sql = "";
            DataTable dt = new DataTable();
            if (year == 0)
            {
                //ISO 8601規則
                //計算出當前日期所在的日曆年
                //var calendarYear = year(@currentDate)
                sql = $@"select to_char(to_date('{current_date.ToString("yyyy/MM/dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss'),'yyyy') as current_year from dual";
                string calendarYear = SFCDB.RunSelect(sql).Tables[0].Rows[0][0].ToString();

                //以星期一為一周的開始,當前日期在周別的第一天和最後一天,及周別的第一天和最後一天所對應的年
                sql = $@"select to_char(trunc(to_date('{current_date.ToString("yyyy/MM/dd")}','yyyy/mm/dd'),'iw'),'yyyy/mm/dd') as FirstDay,
            to_char(to_date('{current_date.ToString("yyyy/MM/dd")}', 'yyyy/mm/dd')-to_char(to_date('{current_date.ToString("yyyy/MM/dd")}', 'yyyy/mm/dd'), 'D') + 8, 'yyyy/mm/dd') as LastDay,
            to_char(trunc(to_date('{current_date.ToString("yyyy/MM/dd")}', 'yyyy/mm/dd'), 'iw'), 'YYYY') as FirstDayYear,
            to_char(to_date('{current_date.ToString("yyyy/MM/dd")}', 'yyyy/mm/dd') - to_char(to_date('{current_date.ToString("yyyy/MM/dd")}', 'yyyy/mm/dd'), 'D') + 8, 'yyyy') as LastDayYear from dual";

                dt = SFCDB.RunSelect(sql).Tables[0];
                string firstDayOfWeek = dt.Rows[0]["FirstDay"].ToString();
                string lastDayDayOfWeek = dt.Rows[0]["LastDay"].ToString();
                string weekBeginYear = dt.Rows[0]["FirstDayYear"].ToString();
                string weekEndYear = dt.Rows[0]["LastDayYear"].ToString();

                if (weekBeginYear != weekEndYear)
                {
                    //該周跨年
                    //在跨年的這一周中取出新年日曆的1/1號                   	
                    string firstDateOfYear = weekEndYear + "/01/01";
                    //獲取1/1是所在周的第幾天,因為該語句是以星期日作為一周的開始，需要注意
                    sql = $@"select TO_CHAR(TO_DATE('{firstDateOfYear}','yyyy/mm/dd'),'D') as Day from dual";
                    dt = SFCDB.RunSelect(sql).Tables[0];
                    int weekDayNum = Convert.ToInt32(dt.Rows[0]["Day"].ToString());
                    //1/1號如果是星期一~星期四,則該周為新年第一周,工作年為新年	
                    if (2 <= weekDayNum && weekDayNum <= 5)
                    {
                        workYear = Convert.ToInt32(weekEndYear);
                    }
                    else
                    {
                        workYear = Convert.ToInt32(weekBeginYear);
                    }
                }
                else
                {
                    workYear = Convert.ToInt32(calendarYear);
                }
                string firstDateOfWorkYear = GetFirstDateOfWorkYear(SFCDB, Convert.ToInt32(workYear));
                sql = $@"select to_date('{current_date.ToString("yyyy/MM/dd")}','yyyy/mm/dd')-to_date('{firstDateOfWorkYear}','yyyy/mm/dd')  from dual";
                dt = SFCDB.RunSelect(sql).Tables[0];
                int days = Convert.ToInt32(dt.Rows[0][0].ToString());
                workWeek = days / 7+1;
                sql = $@"select to_char(to_date('{firstDateOfWorkYear}','yyyy/mm/dd')+ ({workWeek} - 1) * 7,'yyyy/mm/dd')  from dual";
                dt = SFCDB.RunSelect(sql).Tables[0];
                firstDateOfWorkWeek = dt.Rows[0][0].ToString();
            }
            else
            {
                if (week <= 0)
                {
                    week = 1;
                }
                //取出當前工作年份的日曆第一天    
                string firstDateOfWorkYear = GetFirstDateOfWorkYear(SFCDB, year);
                //取出下個工作年份的日曆第一天  
                string firstDateOfNextWorkYear = GetFirstDateOfWorkYear(SFCDB, year + 1);
                sql = $@"select to_date('{firstDateOfNextWorkYear}','yyyy/mm/dd')-to_date('{firstDateOfWorkYear}','yyyy/mm/dd')  from dual";
                dt = SFCDB.RunSelect(sql).Tables[0];
                int days = Convert.ToInt32(dt.Rows[0][0].ToString());
                workWeek = days / 7;
                if (week > workWeek)
                {
                    week = workWeek;
                }
                workYear = year;
                sql = $@"select to_char(to_date('{firstDateOfWorkYear}','yyyy/mm/dd')+ ({week} - 1) * 7,'yyyy/mm/dd')  from dual";
                dt = SFCDB.RunSelect(sql).Tables[0];
                firstDateOfWorkWeek = dt.Rows[0][0].ToString();
            }
        }

        public string GetFirstDateOfWorkYear(OleExec SFCDB, int year)
        {
            //以星期一為一周的開始
            //取出工作年份的日曆第一天            
            string firstDateOfYear = year.ToString() + "/01/01";
            string sql = "";
            DataTable dt = new DataTable();
            //以星期一為一周的開始,日曆第一天所在周別的第一天和最後一天,及周別的第一天和最後一天所對應的年
            sql = $@"select to_char(trunc(to_date('{firstDateOfYear}','yyyy/mm/dd'),'iw'),'yyyy/mm/dd') as FirstDay,
            to_char(to_date('{firstDateOfYear}', 'yyyy/mm/dd') - to_char(to_date('{firstDateOfYear}', 'yyyy/mm/dd'), 'D') + 8, 'yyyy/mm/dd') as LastDay,
            to_char(trunc(to_date('{firstDateOfYear}', 'yyyy/mm/dd'), 'iw'), 'YYYY') as FirstDayYear,
            to_char(to_date('{firstDateOfYear}', 'yyyy/mm/dd') - to_char(to_date('{firstDateOfYear}', 'yyyy/mm/dd'), 'D') + 8, 'yyyy') as LastDayYear   from dual";

            dt = SFCDB.RunSelect(sql).Tables[0];
            string firstDayOfWeek = dt.Rows[0]["FirstDay"].ToString();
            string lastDayDayOfWeek = dt.Rows[0]["LastDay"].ToString();
            string firstDateOfWorkYear = "";

            //獲取1/1是所在周的第幾天,因為該語句是以星期日作為一周的開始，需要注意
            sql = $@"select TO_CHAR(TO_DATE('{firstDateOfYear}','yyyy/mm/dd'),'D') as Day from dual";
            dt = SFCDB.RunSelect(sql).Tables[0];
            int weekDayNum = Convert.ToInt32(dt.Rows[0]["Day"].ToString());
            if (2 <= weekDayNum && weekDayNum <= 5)
            {
                //1/1號如果是星期一~星期四,則該周為新年第一周,這周的第一天為新年的第一天           
                firstDateOfWorkYear = firstDayOfWeek;
            }
            else
            {
                //否則該周的下一周為新年第一周,下一周的第一天為新年的第一天
                sql = $@"select TO_CHAR(TO_DATE('{lastDayDayOfWeek}','yyyy/mm/dd')+1,'yyyy/mm/dd') as FirstDay from dual";
                dt = SFCDB.RunSelect(sql).Tables[0];
                firstDateOfWorkYear = dt.Rows[0]["FirstDay"].ToString();
            }
            return firstDateOfWorkYear;
        }

        private void BonepileGetWeekOrMonth(OleExec SFCDB, string type, int year, int week, int year2, int week2,
            ref int out_year, ref List<int> out_week_list, ref int out_year2, ref List<int> out_week_list2)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new Exception("Type Is Null Or Empty!");
            }
            if (year > year2)
            {
                throw new Exception("[From]年比[To]年大!");
            }
            if (year2 - year > 1)
            {
                throw new Exception("[From]年與[To]年必須是相鄰的年份!");
            }
            string sql = "";
            DataTable dt = new DataTable();
            int temp_week = 0;

            int work_year = 0;
            int work_week = 0;
            string start_date = "";
            string edit_date = "";
            DateTime current_date = Convert.ToDateTime("2011/06/18");
            GetWorkYearAndWeek(SFCDB, current_date, year, week, ref work_year, ref work_week, ref start_date);

            sql = $@"select to_char(sysdate,'yyyy/mm/dd') as edit_date,to_date('{start_date}','yyyy/mm/dd') as start_date,
                    to_date(to_char(sysdate,'yyyy/mm/dd'),'yyyy/mm/dd')-to_date('{start_date}','yyyy/mm/dd') as d  from dual";
            dt = SFCDB.RunSelect(sql).Tables[0];
            edit_date = dt.Rows[0]["EDIT_DATE"].ToString();
            if (Convert.ToInt32(dt.Rows[0]["D"].ToString()) < 0)
            {
                throw new Exception("[From]周別/月份大於當前時間!");
            }
            if (type == "WK")
            {
                if (year == year2)
                {
                    if (week2 < @week)
                    {
                        throw new Exception("[From]周比[To]周大!");
                    }
                    if (week2 - week + 1 > 13)
                    {
                        throw new Exception("[From]周別與[To]周別相隔已超過13周!");
                    }
                    temp_week = week;
                    week2 = work_week < week2 ? work_week : week2;
                    while (temp_week <= week2)
                    {
                        out_week_list.Add(temp_week);
                        temp_week = temp_week + 1;
                    }
                    out_year = year;
                }
                else
                {
                    if (work_week - week + 1 + week2 > 13)
                    {
                        throw new Exception("[From]周別與[To]周別相隔已超過13周!");
                    }
                    temp_week = week;
                    while (temp_week <= week)
                    {
                        out_week_list.Add(temp_week);
                        temp_week = temp_week + 1;
                    }
                    temp_week = 1;
                    while (temp_week <= week2)
                    {
                        out_week_list2.Add(temp_week);
                        temp_week = temp_week + 1;
                    }
                    out_year = year;
                    out_year2 = year2;
                }
            }
            else if (type == "MO")
            {
                if (year == year2)
                {
                    if (week2 < @week)
                    {
                        throw new Exception("[From]比[To]月份大!");
                    }
                    if (week2 - week + 1 > 12)
                    {
                        throw new Exception("[From]月份與[To]月份相隔已超過12個月!");
                    }
                    temp_week = week;
                    week2 = work_week < week2 ? work_week : week2;
                    while (temp_week <= week2)
                    {
                        out_week_list.Add(temp_week);
                        temp_week = temp_week + 1;
                    }
                    out_year = year;
                }
                else
                {
                    if (12 - week + 1 + week2 > 12)
                    {
                        throw new Exception("[From]月份與[To]月份相隔已超過12個月!");
                    }
                    temp_week = week;
                    while (temp_week <= 12)
                    {
                        out_week_list.Add(temp_week);
                        temp_week = temp_week + 1;
                    }
                    temp_week = 1;
                    while (temp_week <= week2)
                    {
                        out_week_list2.Add(temp_week);
                        temp_week = temp_week + 1;
                    }
                    out_year = year;
                    out_year2 = year2;
                }
            }
        }

        public Dictionary<string,object> GetBonepileSummaryReportData(OleExec SFCDB, string tran_type, string data_class, int year, int week, int year2, int week2,
            string category,string rma,string series,string sub_series,string product,string hardcore)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string brcd = "", sql = "", sql_rma = "", sql_category = "", sql_series = "", sql_sub_series = "", sql_product = "", sql_hardcore = "", sql_brcd = "";
            int out_year = 0, out_yaer2 = 0;
            List<int> listWeek = new List<int>();
            List<int> listWeek2 = new List<int>();
            if (data_class == "Critical_Brcd")
            {
                data_class = "Critical";
                brcd = "Y";
            }
            else if (data_class == "Critical_LH")
            {
                data_class = "Critical";
                brcd = "N";
            }
            BonepileGetWeekOrMonth(SFCDB, tran_type, year, week, year2, week2, ref out_year, ref listWeek, ref out_yaer2, ref listWeek2);
            if (year == 0 && listWeek.Count > 0)
            {
                throw new Exception("輸入條件有誤,請確認!");
            }

            Dictionary<string, object> dicDate = new Dictionary<string, object>();
            dicDate.Add("YEAR_1", out_year);
            dicDate.Add("LIST_WEEK_1", listWeek);
            dicDate.Add("YEAR_2", out_yaer2);
            dicDate.Add("LIST_WEEK_2", listWeek2);
            dic.Add("Date", dicDate);

            int minWeek = listWeek.Min();
            int maxWeek = listWeek.Max();
            int minWeek2 = listWeek2.Count == 0 ? 0 : listWeek2.Min();
            int maxWeek2 = listWeek2.Count == 0 ? 0 : listWeek2.Max();

            if (!string.IsNullOrEmpty(rma))
            {
                sql_rma = $@" and RMA='{rma}' ";
            }
            if (!string.IsNullOrEmpty(category))
            {
                sql_category = $@" and Category='{category}' ";
            }
            if (!string.IsNullOrEmpty(series))
            {
                sql_series = $@" and Series='{series}' ";
            }
            if (!string.IsNullOrEmpty(sub_series))
            {
                sql_sub_series = $@" and Sub_Series='{sub_series}' ";
            }
            if (!string.IsNullOrEmpty(product))
            {
                sql_product = $@" and Product_Name='{product}' ";
            }
            if (!string.IsNullOrEmpty(hardcore))
            {
                sql_hardcore = $@" and Hardcore='{hardcore}' ";
            }
            if (!string.IsNullOrEmpty(brcd))
            {
                sql_brcd = $@" and brcd='{brcd}' ";
            }
            sql = $@" select DATA_CLASS,status,YEAR,WEEK_OR_MONTH,sum(qty) as qty from r_bonepile_basic where TRAN_TYPE='{tran_type}' 
                          and ((YEAR='{year}' and WEEK_OR_MONTH between '{minWeek}' and '{maxWeek}')
                          or (YEAR='{year2}' and WEEK_OR_MONTH between '{minWeek2}' and '{maxWeek2}'))
                          and DATA_CLASS='{data_class}' {sql_rma} {sql_category} {sql_product} {sql_series} 
                            {sql_sub_series} {sql_brcd} {sql_hardcore}
                            group by DATA_CLASS,status,YEAR,WEEK_OR_MONTH          
                        UNION ALL
                        select DATA_CLASS,'OpenAmount',YEAR,WEEK_OR_MONTH,sum(Amount) as Amount from r_bonepile_basic where TRAN_TYPE='{tran_type}' 
                         and ((YEAR='{year}' and WEEK_OR_MONTH between '{minWeek}'  and '{maxWeek}') 
                          or (YEAR='{year2}' and WEEK_OR_MONTH between '{minWeek2}' and '{maxWeek2}')) and DATA_CLASS='{data_class}'          
                          and status='Open' {sql_rma} {sql_category} {sql_product} {sql_series} 
                            {sql_sub_series} {sql_brcd} {sql_hardcore}
                          group by DATA_CLASS,YEAR,WEEK_OR_MONTH order by WEEK_OR_MONTH";

            DataTable dt2 =  SFCDB.RunSelect(sql).Tables[0].Copy();            
            dic.Add("DATA_1", dt2);
            if (listWeek2.Count > 0)
            {
                year = year2;
                maxWeek = maxWeek2;
            }            

            sql = $@"select DATA_CLASS,status,YEAR,WEEK_OR_MONTH,aging,sum(qty) as qty from r_bonepile_basic where TRAN_TYPE='{tran_type}' 
                  and YEAR='{year}' and WEEK_OR_MONTH='{maxWeek}' and DATA_CLASS='{data_class}' {sql_rma} {sql_category} {sql_product} 
                    {sql_series} {sql_sub_series} {sql_brcd} {sql_hardcore} group by DATA_CLASS,status,YEAR,WEEK_OR_MONTH,aging
                UNION ALL
                select DATA_CLASS,'OpenAmount',YEAR,WEEK_OR_MONTH,aging,sum(Amount) as Amount from r_bonepile_basic where TRAN_TYPE='{tran_type}' 
                  and YEAR='{year}' and WEEK_OR_MONTH='{maxWeek}' and DATA_CLASS='{data_class}'  and status='Open'
                  {sql_rma} {sql_category} {sql_product} {sql_series} {sql_sub_series} {sql_brcd} {sql_hardcore}
                  group by DATA_CLASS,YEAR,WEEK_OR_MONTH,aging order by WEEK_OR_MONTH";
            DataTable dt3 = SFCDB.RunSelect(sql).Tables[0].Copy();           
            dic.Add("DATA_2", dt3);
            return dic;
        }
    }
    public class Row_R_BONEPILE_BASIC : DataObjectBase
    {
        public Row_R_BONEPILE_BASIC(DataObjectInfo info) : base(info)
        {

        }
        public R_BONEPILE_BASIC GetDataObject()
        {
            R_BONEPILE_BASIC DataObject = new R_BONEPILE_BASIC();
            DataObject.ID = this.ID;
            DataObject.BRCD = this.BRCD;
            DataObject.DATA_CLASS = this.DATA_CLASS;
            DataObject.WEEK_OR_MONTH = this.WEEK_OR_MONTH;
            DataObject.YEAR = this.YEAR;
            DataObject.TRAN_TYPE = this.TRAN_TYPE;
            DataObject.LASTEDIT_DATE = this.LASTEDIT_DATE;
            DataObject.LASTEDIT_BY = this.LASTEDIT_BY;
            DataObject.AMOUNT = this.AMOUNT;
            DataObject.QTY = this.QTY;
            DataObject.AVGDAYS = this.AVGDAYS;
            DataObject.AGING = this.AGING;
            DataObject.BONEPILE_DESCRIPTION = this.BONEPILE_DESCRIPTION;
            DataObject.PRODUCT_NAME = this.PRODUCT_NAME;
            DataObject.SUB_SERIES = this.SUB_SERIES;
            DataObject.SERIES = this.SERIES;
            DataObject.HARDCORE = this.HARDCORE;
            DataObject.STATUS = this.STATUS;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.RMA = this.RMA;
            DataObject.NORMAL = this.NORMAL;
            DataObject.FLH = this.FLH;
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
        public string BRCD
        {
            get
            {
                return (string)this["BRCD"];
            }
            set
            {
                this["BRCD"] = value;
            }
        }
        public string DATA_CLASS
        {
            get
            {
                return (string)this["DATA_CLASS"];
            }
            set
            {
                this["DATA_CLASS"] = value;
            }
        }
        public string WEEK_OR_MONTH
        {
            get
            {
                return (string)this["WEEK_OR_MONTH"];
            }
            set
            {
                this["WEEK_OR_MONTH"] = value;
            }
        }
        public string YEAR
        {
            get
            {
                return (string)this["YEAR"];
            }
            set
            {
                this["YEAR"] = value;
            }
        }
        public string TRAN_TYPE
        {
            get
            {
                return (string)this["TRAN_TYPE"];
            }
            set
            {
                this["TRAN_TYPE"] = value;
            }
        }
        public DateTime? LASTEDIT_DATE
        {
            get
            {
                return (DateTime?)this["LASTEDIT_DATE"];
            }
            set
            {
                this["LASTEDIT_DATE"] = value;
            }
        }
        public string LASTEDIT_BY
        {
            get
            {
                return (string)this["LASTEDIT_BY"];
            }
            set
            {
                this["LASTEDIT_BY"] = value;
            }
        }
        public double? AMOUNT
        {
            get
            {
                return (double?)this["AMOUNT"];
            }
            set
            {
                this["AMOUNT"] = value;
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
        public double? AVGDAYS
        {
            get
            {
                return (double?)this["AVGDAYS"];
            }
            set
            {
                this["AVGDAYS"] = value;
            }
        }
        public string AGING
        {
            get
            {
                return (string)this["AGING"];
            }
            set
            {
                this["AGING"] = value;
            }
        }
        public string BONEPILE_DESCRIPTION
        {
            get
            {
                return (string)this["BONEPILE_DESCRIPTION"];
            }
            set
            {
                this["BONEPILE_DESCRIPTION"] = value;
            }
        }
        public string PRODUCT_NAME
        {
            get
            {
                return (string)this["PRODUCT_NAME"];
            }
            set
            {
                this["PRODUCT_NAME"] = value;
            }
        }
        public string SUB_SERIES
        {
            get
            {
                return (string)this["SUB_SERIES"];
            }
            set
            {
                this["SUB_SERIES"] = value;
            }
        }
        public string SERIES
        {
            get
            {
                return (string)this["SERIES"];
            }
            set
            {
                this["SERIES"] = value;
            }
        }
        public string HARDCORE
        {
            get
            {
                return (string)this["HARDCORE"];
            }
            set
            {
                this["HARDCORE"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string RMA
        {
            get
            {
                return (string)this["RMA"];
            }
            set
            {
                this["RMA"] = value;
            }
        }
        public string NORMAL
        {
            get
            {
                return (string)this["NORMAL"];
            }
            set
            {
                this["NORMAL"] = value;
            }
        }
        public string FLH
        {
            get
            {
                return (string)this["FLH"];
            }
            set
            {
                this["FLH"] = value;
            }
        }
    }
    public class R_BONEPILE_BASIC
    {
        public string ID { get; set; }
        public string BRCD { get; set; }
        public string DATA_CLASS { get; set; }
        public string WEEK_OR_MONTH { get; set; }
        public string YEAR { get; set; }
        public string TRAN_TYPE { get; set; }
        public DateTime? LASTEDIT_DATE { get; set; }
        public string LASTEDIT_BY { get; set; }
        public double? AMOUNT { get; set; }
        public double? QTY { get; set; }
        public double? AVGDAYS { get; set; }
        public string AGING { get; set; }
        public string BONEPILE_DESCRIPTION { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SUB_SERIES { get; set; }
        public string SERIES { get; set; }
        public string HARDCORE { get; set; }
        public string STATUS { get; set; }
        public string CATEGORY { get; set; }
        public string RMA { get; set; }
        public string NORMAL { get; set; }
        public string FLH { get; set; }
    }

   
}