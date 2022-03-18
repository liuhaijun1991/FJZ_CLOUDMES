using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_RMA : DataObjectTable
    {
        public T_R_RMA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RMA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RMA);
            TableName = "R_RMA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public bool GetSNRight11ORSN(string SN, OleExec db)
        {
            bool res = false;
            string strsql = $@"SELECT * FROM R_RMA WHERE SUBSTR(SN,11)='{SN}' OR SN='{SN}'";
            int Num = db.ExecSqlNoReturn(strsql, null);
            if (Num > 0)
            {
                res = true;
            }
            return res;
        }
        public bool GetSNUNLOCKEDANDSNFOC(string SN, OleExec db)
        {
            bool res = false;
            //SELECT * FROM R_RMA WHERE UNLOCKED_FLAG=1 AND SN='' AND SN  LIKE 'FOC%'
            string strsql = $@"SELECT * FROM R_RMA WHERE UNLOCKED_FLAG=1 AND SN='{SN}' AND SN  LIKE 'FOC%' ";
            int Num = db.ExecSqlNoReturn(strsql, null);
            if (Num > 0)
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// 根據SN獲取未鎖板的SN
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="BU"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataTable GetBySN(string SN, string BU, OleExec DB)
        {
            //string strSql = $@"select *from r_rma where upper(BU) like'%{BU}%' and unlocked_flag=1 AND SN=:SN ";
            //string strSql = $@"select *from r_rma where unlocked_flag=1 AND SN=:SN OR WO=:SN OR SKUNO=SN";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SN",SN) };
            //判斷station
            string strSql = $@" select b.test_station from r_rma a,C_SKU_TEST_STATION b,c_sku c where b.c_sku_id=c.id and a.skuno=c.skuno and a.sn='" + SN + "'";
            DataTable dt = DB.ExecuteDataTable(strSql, CommandType.Text);
            //station不為空
            if (dt.Rows.Count > 0)
            {
                string station = dt.Rows[0][0].ToString();
                strSql = $@" select  a.id,a.SN,c.sku_name,c.skuno,b.test_station,d.status from r_rma a,c_sku_test_station b,c_sku c,r_testrecord d where d.station_name='" + station + "' and d.status='P' AND A.SN=D.SN AND A.SKUNO=C.SKUNO and a.sn='" + SN + "' and a.unlocked_flag=1";
            }
            else
            {
                strSql = $@"select a.id,a.sn,c.sku_name,c.skuno,b.test_station from r_rma a,c_sku_test_station b,c_sku c where a.skuno=c.skuno and b.c_sku_id=c.id and a.sn='" + SN + "' and a.unlocked_flag=1";
            }
            //List<R_RMA> result = new List<R_RMA>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            return res;
        }
        /// <summary>
        /// SN為空根據時間獲取未鎖板SN
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="date_from"></param>
        /// <param name="date_to"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataTable GetByFlag(string BU, string date_from, string date_to, OleExec DB)
        {
            //string strSql = $@"select *from r_rma where unlocked_flag=1 order by EDIT_TIME desc";
            string strSql = "";
            if (date_from == "" && date_to == "")
            {
                strSql = $@"select distinct B.ID,b.sn,c.sku_name,c.skuno,d.test_station,a.status from R_TESTRECORD a,r_rma b,c_sku c,C_SKU_TEST_STATION d where a.SN=B.SN AND B.SKUNO=C.SKUNO AND A.STATION_NAME=d.test_station";
                strSql += " and a.status='P'and b.valid_flag=0 and b.unlocked_flag=1";
            }
            else
            {
                strSql = $@"select distinct B.ID,b.sn,c.sku_name,c.skuno,d.test_station,a.status from R_TESTRECORD a,r_rma b,c_sku c,C_SKU_TEST_STATION d where a.SN=B.SN AND B.SKUNO=C.SKUNO AND A.STATION_NAME=d.test_station";
                strSql += " AND to_date(substr(B.SCAN_TIME,1,10)) BETWEEN to_date('" + date_from + "','YYYY/MM/DD') AND to_date('" + date_to + "','YYYY/MM/DD') and a.status='P'and b.valid_flag=0 and b.unlocked_flag=1";
            }
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            return res;
        }

        /// <summary>
        /// 鎖板
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateByid(string ID, string EDIT_EMP, DateTime EDIT_TIME, OleExec DB)
        {

            string strSql = "";
            strSql = $@"update r_rma set unlocked_flag='0',valid_flag='1',edit_emp='" + EDIT_EMP + "',edit_time=TO_DATE('" + EDIT_TIME + "','YYYY/MM/DD HH24:MI:SS') where ID='" + ID + "'"; //sysserialno='"+SN+"'
            //else if (type.ToUpper() == "DEBUG")
            //{
            //    strSql = $@"update r_rma set valid_flag=1,edit_emp='" + EDIT_EMP + "',edit_time='" + EDIT_TIME + "' where ID='" + ID + "'";
            //}
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text);
            return result;
        }

        /// <summary>
        /// debug
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="EDIT_EMP"></param>
        /// <param name="EDIT_TIME"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int debug(string ID, string EDIT_EMP, DateTime EDIT_TIME, OleExec DB)
        {
            string strSql = $@"update r_rma set valid_flag=1,edit_emp='" + EDIT_EMP + "',edit_time='" + EDIT_TIME + "' where ID='" + ID + "'";
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text);
            return result;
        }
    }
    public class Row_R_RMA : DataObjectBase
    {
        public Row_R_RMA(DataObjectInfo info) : base(info)
        {

        }
        public R_RMA GetDataObject()
        {
            R_RMA DataObject = new R_RMA();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.BU = this.BU;
            DataObject.SCAN_EMP = this.SCAN_EMP;
            DataObject.SCAN_TIME = this.SCAN_TIME;
            DataObject.LOCKED_FLAG = this.LOCKED_FLAG;
            DataObject.UNLOCKED_FLAG = this.UNLOCKED_FLAG;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
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
        public string SCAN_EMP
        {
            get
            {
                return (string)this["SCAN_EMP"];
            }
            set
            {
                this["SCAN_EMP"] = value;
            }
        }
        public DateTime? SCAN_TIME
        {
            get
            {
                return (DateTime?)this["SCAN_TIME"];
            }
            set
            {
                this["SCAN_TIME"] = value;
            }
        }
        public string LOCKED_FLAG
        {
            get
            {
                return (string)this["LOCKED_FLAG"];
            }
            set
            {
                this["LOCKED_FLAG"] = value;
            }
        }
        public string UNLOCKED_FLAG
        {
            get
            {
                return (string)this["UNLOCKED_FLAG"];
            }
            set
            {
                this["UNLOCKED_FLAG"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
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
    public class R_RMA
    {
        public string ID;
        public string R_SN_ID;
        public string SN;
        public string WO;
        public string SKUNO;
        public string BU;
        public string SCAN_EMP;
        public DateTime? SCAN_TIME;
        public string LOCKED_FLAG;
        public string UNLOCKED_FLAG;
        public string VALID_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}