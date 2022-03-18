using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_LLT_TEST : DataObjectTable
    {
        public T_R_LLT_TEST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LLT_TEST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LLT_TEST);
            TableName = "R_LLT_TEST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_R_LLT_TEST GetTestTime1(string sn, OleExec DB)
        {

            string sql = $"select a.sn, sum(b.BURNIN_TIME) TotalTime  from r_llt a,r_llt_test b " +
                    $"where a.sn = b.sn and a.sn = '" + sn + "' and a.status = 0 group by a.sn having sum(b.BURNIN_TIME) < '1440' ";
            DataSet res = DB.ExecSelect(sql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_LLT_TEST ret = (Row_R_LLT_TEST)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }
        public Row_R_LLT_TEST GetTestTime2(string sn, OleExec DB)
        {

            string sql = $"select a.sn, sum(b.BURNIN_TIME) TotalTime  from r_llt a,r_llt_test b " +
                      $"where a.sn = b.sn and a.sn = '" + sn + "' and a.status = 1 group by a.sn having sum(b.BURNIN_TIME) > '1439' ";
            DataSet res = DB.ExecSelect(sql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_LLT_TEST ret = (Row_R_LLT_TEST)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }
        public Row_R_LLT_TEST IsTwoHour(string sn, OleExec DB)
        {

            string sql = $"select sn,createtime from r_llt_test WHERE sn='"+ sn + "' having floor((sysdate- max(CREATETIME))*24)>2 group by sn,CREATETIME";
            DataSet res = DB.ExecSelect(sql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_LLT_TEST ret = (Row_R_LLT_TEST)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }
    }
    public class Row_R_LLT_TEST : DataObjectBase
    {
        public Row_R_LLT_TEST(DataObjectInfo info) : base(info)
        {

        }
        public R_LLT_TEST GetDataObject()
        {
            R_LLT_TEST DataObject = new R_LLT_TEST();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.STATUS = this.STATUS;
            DataObject.TESTATION = this.TESTATION;
            DataObject.MESSTATION = this.MESSTATION;
            DataObject.BURNIN_TIME = this.BURNIN_TIME;
            DataObject.CELL = this.CELL;
            DataObject.TESTTIME = this.TESTTIME;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
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
        public string TESTATION
        {
            get
            {
                return (string)this["TESTATION"];
            }
            set
            {
                this["TESTATION"] = value;
            }
        }
        public string MESSTATION
        {
            get
            {
                return (string)this["MESSTATION"];
            }
            set
            {
                this["MESSTATION"] = value;
            }
        }
        public string BURNIN_TIME
        {
            get
            {
                return (string)this["BURNIN_TIME"];
            }
            set
            {
                this["BURNIN_TIME"] = value;
            }
        }
        public string CELL
        {
            get
            {
                return (string)this["CELL"];
            }
            set
            {
                this["CELL"] = value;
            }
        }
        public DateTime? TESTTIME
        {
            get
            {
                return (DateTime?)this["TESTTIME"];
            }
            set
            {
                this["TESTTIME"] = value;
            }
        }
        public DateTime? ENDTIME
        {
            get
            {
                return (DateTime?)this["ENDTIME"];
            }
            set
            {
                this["ENDTIME"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
    }
    public class R_LLT_TEST
    {
        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string STATUS { get; set; }
        public string TESTATION { get; set; }
        public string MESSTATION { get; set; }
        public string BURNIN_TIME { get; set; }
        public string CELL { get; set; }
        public DateTime? TESTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}