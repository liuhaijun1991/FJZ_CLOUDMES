using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module
{
    public class T_R_PACKING_RATE_DETAIL : DataObjectTable
    {
        public T_R_PACKING_RATE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PACKING_RATE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PACKING_RATE_DETAIL);
            TableName = "R_PACKING_RATE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 獲取Packing每日的產量
        /// </summary>
        /// <param name="bU"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public DataTable GetPackingDateRate(string bU, OleExec DB)
        {
            //'2019-03-11','2019-03-12','2019-03-13','2019-03-14','2019-03-15','2019-03-16','2019-03-17'
            List<string> Time = GetDateTime(DB);
            //List<string> Time = new List<string>();
            //Time.Add("2019-03-20");
            //Time.Add("2019-03-12");
            //Time.Add("2019-03-13");
            //Time.Add("2019-03-14");
            //Time.Add("2019-03-15");
            //Time.Add("2019-03-16");
            //Time.Add("2019-03-17");

            string strTime = string.Empty;
            string strsql = "";
            DataTable res = new DataTable();
            if (Time.Count > 0)
            {
                for (int i = 0; i < Time.Count; i++)
                {
                    strTime += "'" + Time[i] + "',";
                }
                strTime = strTime.Substring(0, strTime.Length - 1);
                strsql = $@"select BU,post_date,class,sum(middle_qty+small_qty+large_qty) as quantity from R_PACKING_RATE_DETAIL where post_date in(" + strTime + ") and line in(";
                strsql += "select control_value From C_CONTROL where control_name = 'PACKINGDISPLAY') group by BU,class,post_date order by bu";

                // strsql = $@"select BU,post_date,class,sum(middle_qty+small_qty+large_qty) as quantity from R_PACKING_RATE_DETAIL where post_date in('2019-03-11') and line in(";
                //strsql += "select control_value From C_CONTROL where control_name = 'PACKINGDISPLAY') group by BU,class,post_date order by bu";
                res = DB.RunSelect(strsql).Tables[0];
            }
            return res;
        }
        /// <summary>
        /// 獲取Packing的時間
        /// </summary>
        public List<string> GetDateTime(OleExec sfcdb)
        {
            List<string> DateTime = new List<string>();
            DateTime dt = GetDBDateTime(sfcdb);
            int Hour = dt.Hour;
            if (Hour >= 0 && Hour < 8)
            {
                int t = 7;
                while (t > 0)
                {
                    string dd = dt.AddDays(-t).ToShortDateString();
                    DateTime.Add(dd.Replace('/', '-'));
                    t--;
                }
            }
            else
            {
                int t = 6;
                while (t > 0)
                {
                    string dd = dt.AddDays(-t).ToShortDateString();
                    DateTime.Add(dd.Replace('/', '-'));
                    t--;
                }
                DateTime.Add(dt.ToShortDateString().Replace('/', '-'));
            }
            return DateTime;
        }

        /// <summary>
        /// 獲取白班,夜班每小時的產出
        /// </summary>
        /// <param name="bU"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public DataTable GetPackingByHour(string bU, OleExec DB)
        {
            DateTime dt = GetDBDateTime(DB);
            string date1 = "";
            string date2 = "";
            int Hour = dt.Hour;
            string min = dt.Minute.ToString();
            if (min == "0")
            {
                min = "1";
            }
            //shift 1
            if (Hour >= 8 && Hour < 20)
            {
                date1 = dt.ToShortDateString();//白班
                date2 = dt.AddDays(-1).ToShortDateString();
            }
            //shift 2
            else
            {
                date1 = date2 = dt.ToShortDateString(); //夜班 
            }
            //產出標準
            string strsql1 = "select CONTROL_DESC From C_CONTROL where control_name='STANDARDPACKINGRATE'";
            DataTable dt2 = DB.ExecSelect(strsql1).Tables[0];
            DataTable res = new DataTable();
            double[] stand = new double[3];
            if (dt2.Rows.Count > 0)
            {
                for (int i = 0; i < stand.Length; i++)
                {
                    stand[i] = Convert.ToDouble(dt2.Rows[i][0]);
                }
                //2011-12-09
                date1 = date2 = "2011/12/10";
                date1 = date1.Replace('/', '-');
                date2 = date2.Replace('/', '-');
                string strsql = $@"select BU,CLASS,LINE,WORK_TIME,(a.middle_qty+a.small_qty+a.large_qty) as totalpass,a.small_qty,a.middle_qty,large_qty";
                strsql += " from R_PACKING_RATE_DETAIL a,C_CONTROL b where ((a.post_date = '" + date1 + "' and a.class='SHIFT 1') or(a.post_date = '" + date2 + "' and a.class='SHIFT 2'))";
                strsql += " and b.control_name='PACKINGDISPLAY' and a.line= b.control_value order by a.line, a.post_date, a.class,a.edit_time";
                res = DB.ExecSelect(strsql).Tables[0];
                //計算產率
                if (res.Rows.Count > 0)
                {
                    res.Columns.Add("PERCENTAGE");
                    for (int i = 0; i < res.Rows.Count; i++)
                    {
                        DataRow dr = res.Rows[i];
                        dr["PERCENTAGE"] = Convert.ToDouble(dr["small_qty"]) / stand[2] + Convert.ToDouble(dr["middle_qty"]) / stand[1] + Convert.ToDouble(dr["large_qty"]) / stand[0];
                    }
                }
            }
            return res;
        }
        public List<R_PACKING_RATE_DETAIL> GetPackingRateDetail(OleExec oleDB, string WORKTIME, string POSTDATE, string CLASS, string LINE, string BU)
        {
            string sql = $@"SELECT * FROM R_PACKING_RATE_DETAIL WHERE BU = '{BU}' AND LINE = '{LINE}'AND CLASS = '{CLASS}' AND POST_DATE = '{POSTDATE}' AND WORK_TIME = '{WORKTIME}'";
            List<R_PACKING_RATE_DETAIL> workClassList = new List<R_PACKING_RATE_DETAIL>();
            DataSet dsPackingRateDetail = oleDB.ExecSelect(sql);
            Row_R_PACKING_RATE_DETAIL RowRRpackingRateDetail;
            foreach (DataRow row in dsPackingRateDetail.Tables[0].Rows)
            {
                RowRRpackingRateDetail = (Row_R_PACKING_RATE_DETAIL)this.NewRow();
                RowRRpackingRateDetail.loadData(row);
                workClassList.Add(RowRRpackingRateDetail.GetDataObject());
            }
            return workClassList;
        }
        public int UpdateYieldRateDetail(R_PACKING_RATE_DETAIL PACKINGRATEDETAIL, OleExec DB)
        {
            return DB.ORM.Updateable<R_PACKING_RATE_DETAIL>(PACKINGRATEDETAIL).Where(t => t.BU == PACKINGRATEDETAIL.BU && t.LINE == PACKINGRATEDETAIL.LINE && t.CLASS == PACKINGRATEDETAIL.CLASS && t.POST_DATE == PACKINGRATEDETAIL.POST_DATE && t.WORK_TIME == PACKINGRATEDETAIL.WORK_TIME).ExecuteCommand();
        }
        public List<R_PACKING_RATE_DETAIL> GetPackingRateDetailWP(OleExec oleDB, string WORKTIME, string POSTDATE)
        {
            string sql = $@"SELECT * FROM R_PACKING_RATE_DETAIL WHERE POST_DATE = '{POSTDATE}' AND WORK_TIME = '{WORKTIME}'";
            List<R_PACKING_RATE_DETAIL> workClassList = new List<R_PACKING_RATE_DETAIL>();
            DataSet dsPackingRateDetail = oleDB.ExecSelect(sql);
            Row_R_PACKING_RATE_DETAIL RowRRpackingRateDetail;
            foreach (DataRow row in dsPackingRateDetail.Tables[0].Rows)
            {
                RowRRpackingRateDetail = (Row_R_PACKING_RATE_DETAIL)this.NewRow();
                RowRRpackingRateDetail.loadData(row);
                workClassList.Add(RowRRpackingRateDetail.GetDataObject());
            }
            return workClassList;
        }
        public List<R_PACKING_RATE_DETAIL> GetPackingRateDetailPLW(string WORKTIME, string POSTDATE, string LINE, OleExec oleDB)
        {
            string sql = $@"SELECT * FROM R_PACKING_RATE_DETAIL WHERE LINE = '{LINE}' AND POST_DATE = '{POSTDATE}' AND WORK_TIME = '{WORKTIME}'";
            List<R_PACKING_RATE_DETAIL> workClassList = new List<R_PACKING_RATE_DETAIL>();
            DataSet dsPackingRateDetail = oleDB.ExecSelect(sql);
            Row_R_PACKING_RATE_DETAIL RowRRpackingRateDetail;
            foreach (DataRow row in dsPackingRateDetail.Tables[0].Rows)
            {
                RowRRpackingRateDetail = (Row_R_PACKING_RATE_DETAIL)this.NewRow();
                RowRRpackingRateDetail.loadData(row);
                workClassList.Add(RowRRpackingRateDetail.GetDataObject());
            }
            return workClassList;
        }
        public int InsertYieldRateDetail(R_PACKING_RATE_DETAIL PACKINGRATEDETAIL, OleExec DB)
        {
            return DB.ORM.Insertable<R_PACKING_RATE_DETAIL>(PACKINGRATEDETAIL).ExecuteCommand();
        }
    }

    public class Row_R_PACKING_RATE_DETAIL : DataObjectBase
    {
        public Row_R_PACKING_RATE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_PACKING_RATE_DETAIL GetDataObject()
        {
            R_PACKING_RATE_DETAIL DataObject = new R_PACKING_RATE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.LINE = this.LINE;
            DataObject.CLASS = this.CLASS;
            DataObject.POST_DATE = this.POST_DATE;
            DataObject.WORK_TIME = this.WORK_TIME;
            DataObject.LARGE_QTY = this.LARGE_QTY;
            DataObject.MIDDLE_QTY = this.MIDDLE_QTY;
            DataObject.SMALL_QTY = this.SMALL_QTY;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string CLASS
        {
            get
            {
                return (string)this["CLASS"];
            }
            set
            {
                this["CLASS"] = value;
            }
        }
        public string POST_DATE
        {
            get
            {
                return (string)this["POST_DATE"];
            }
            set
            {
                this["POST_DATE"] = value;
            }
        }
        public string WORK_TIME
        {
            get
            {
                return (string)this["WORK_TIME"];
            }
            set
            {
                this["WORK_TIME"] = value;
            }
        }
        public Double LARGE_QTY
        {
            get
            {
                return (Double)this["LARGE_QTY"];
            }
            set
            {
                this["LARGE_QTY"] = value;
            }
        }
        public Double MIDDLE_QTY
        {
            get
            {
                return (Double)this["MIDDLE_QTY"];
            }
            set
            {
                this["MIDDLE_QTY"] = value;
            }
        }
        public Double SMALL_QTY
        {
            get
            {
                return (Double)this["SMALL_QTY"];
            }
            set
            {
                this["SMALL_QTY"] = value;
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
    }

    public class R_PACKING_RATE_DETAIL
    {
        public string ID { get; set; }
        public string BU { get; set; }
        public string LINE { get; set; }
        public string CLASS { get; set; }
        public string POST_DATE { get; set; }
        public string WORK_TIME { get; set; }
        public double? LARGE_QTY { get; set; }
        public double? MIDDLE_QTY { get; set; }
        public double? SMALL_QTY { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}
