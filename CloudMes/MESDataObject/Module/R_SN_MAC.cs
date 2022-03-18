using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SN_MAC : DataObjectTable
    {
        public T_R_SN_MAC(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_MAC(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_MAC);
            TableName = "R_SN_MAC".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Insert(R_SN_MAC R_SN_MAC, OleExec DB)
        {
            return DB.ORM.Insertable<R_SN_MAC>(R_SN_MAC).ExecuteCommand();
        }

        /// <summary>
        /// 檢查是否已分配MAC
        /// add by hgb 2019.08.19
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="SUBSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckExists(string SN, string SUBSN_TYPE, string SUBSN, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            string StrsqlSUBSN_TYPE = string.Empty;
            string StrsqlSUBSN = string.Empty;

            if (SUBSN_TYPE.Length != 0)
            {
                StrsqlSUBSN_TYPE = $@" AND SUBSN_TYPE='{SUBSN_TYPE}'";
            }
            if (SUBSN.Length != 0)
            {
                if (SUBSN == "CHECKNULL")
                {
                     StrsqlSUBSN= " and subsn is not null";
                }
                else
                {
                    StrsqlSUBSN = $@" AND SUBSN='{SUBSN}'";
                } 
            }
            StrSql = $@"SELECT * FROM R_SN_MAC WHERE SN = '{SN}' {StrsqlSUBSN_TYPE}  {StrsqlSUBSN} ";
            
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }
        /// <summary>
        /// 檢查MAC是否重碼
        /// add by hgb 2019.08.19
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="SUBSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckUnique(string SN, string SUBSN_TYPE, string SUBSN, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            string StrsqlSUBSN_TYPE = string.Empty;
            string StrsqlSUBSN = string.Empty;

            if (SUBSN_TYPE.Length != 0)
            {
                StrsqlSUBSN_TYPE = $@" AND SUBSN_TYPE='{SUBSN_TYPE}'";
            }
            if (SUBSN.Length != 0)
            {
                if (SUBSN == "CHECKNULL")
                {
                    StrsqlSUBSN = " and subsn is not null";
                }
                else
                {
                    StrsqlSUBSN = $@" AND subsn='{SUBSN}'";
                }
            }
            StrSql = $@"SELECT * FROM R_SN_MAC WHERE SN = '{SN}' {StrsqlSUBSN_TYPE}  {StrsqlSUBSN} ";

            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 1)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public R_SN_MAC LoadData(string SN, string SUBSN_TYPE, string SUBSN, OleExec DB)
        {
            R_SN_MAC r_sn_mac = new R_SN_MAC();
            Row_R_SN_MAC Row_r_sn_mac = (Row_R_SN_MAC)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = string.Empty;
            string StrsqlSUBSN_TYPE = string.Empty;
            string StrsqlSUBSN = string.Empty;

            if (SUBSN_TYPE.Length != 0)
            {
                StrsqlSUBSN_TYPE = $@" AND SUBSN_TYPE='{SUBSN_TYPE}'";
            }
            if (SUBSN.Length != 0)
            {
                if (SUBSN == "CHECKNULL")
                {
                    StrsqlSUBSN = " and subsn is not null";
                }
                else
                {
                    StrsqlSUBSN = $@" AND SUBSN='{SUBSN}'";
                }
            }
            StrSql = $@"SELECT * FROM R_SN_MAC WHERE SN = '{SN}' {StrsqlSUBSN_TYPE}  {StrsqlSUBSN} ";

            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                Row_r_sn_mac.loadData(Dt.Rows[0]);
                r_sn_mac = Row_r_sn_mac.GetDataObject();
            }

            return r_sn_mac;
            //return DB.ORM.Queryable<R_SN>().Where(t => t.SN == StrSN && t.VALID_FLAG == "1").ToList().FirstOrDefault();
        }

        /// <summary>
        /// 根據MAC12獲取MAC16 (打印gponsn需要)
        /// add by hgb 2019.06.24
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="TYPE"></param>
        /// <param name="STATION"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_SN_MAC GetGponSnByMac(string StrMAC, OleExec DB)
        {
            R_SN_MAC r_sn_mac = null;
            Row_R_SN_MAC Row_r_sn_mac = (Row_R_SN_MAC)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = $@"
             SELECT *
        FROM R_SN_MAC
       WHERE sysserialno IN
             (SELECT SN FROM R_SN_KP WHERE value = '{StrMAC}')
         AND subsn_type = 'SN'; 
             ";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                Row_r_sn_mac.loadData(Dt.Rows[0]);
                r_sn_mac = Row_r_sn_mac.GetDataObject();
            }

            return r_sn_mac;
            //return DB.ORM.Queryable<R_SN>().Where(t => t.SN == StrSN && t.VALID_FLAG == "1").ToList().FirstOrDefault();
        }
    }
    public class Row_R_SN_MAC : DataObjectBase
    {
        public Row_R_SN_MAC(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_MAC GetDataObject()
        {
            R_SN_MAC DataObject = new R_SN_MAC();
            DataObject.PRINTED_COUNT = this.PRINTED_COUNT;
            DataObject.SEQNO = this.SEQNO;
            DataObject.DATA1 = this.DATA1;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.BOXSN = this.BOXSN;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SUBSN = this.SUBSN;
            DataObject.WO = this.WO;
            DataObject.PRINT_NO = this.PRINT_NO;
            DataObject.STEP = this.STEP;
            DataObject.SUBSN_TYPE = this.SUBSN_TYPE;
            DataObject.ASSIGN_FLAG = this.ASSIGN_FLAG;
            DataObject.NEEDPRINT_FLAG = this.NEEDPRINT_FLAG;
            return DataObject;
        }
        public double? PRINTED_COUNT
        {
            get
            {
                return (double?)this["PRINTED_COUNT"];
            }
            set
            {
                this["PRINTED_COUNT"] = value;
            }
        }
        public string SEQNO
        {
            get
            {
                return (string)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string TASK_NO
        {
            get
            {
                return (string)this["TASK_NO"];
            }
            set
            {
                this["TASK_NO"] = value;
            }
        }
        public string BOXSN
        {
            get
            {
                return (string)this["BOXSN"];
            }
            set
            {
                this["BOXSN"] = value;
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
        public string SUBSN
        {
            get
            {
                return (string)this["SUBSN"];
            }
            set
            {
                this["SUBSN"] = value;
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
        public double? PRINT_NO
        {
            get
            {
                return (double?)this["PRINT_NO"];
            }
            set
            {
                this["PRINT_NO"] = value;
            }
        }
        public double? STEP
        {
            get
            {
                return (double?)this["STEP"];
            }
            set
            {
                this["STEP"] = value;
            }
        }
        public string SUBSN_TYPE
        {
            get
            {
                return (string)this["SUBSN_TYPE"];
            }
            set
            {
                this["SUBSN_TYPE"] = value;
            }
        }
        public string ASSIGN_FLAG
        {
            get
            {
                return (string)this["ASSIGN_FLAG"];
            }
            set
            {
                this["ASSIGN_FLAG"] = value;
            }
        }
        public string NEEDPRINT_FLAG
        {
            get
            {
                return (string)this["NEEDPRINT_FLAG"];
            }
            set
            {
                this["NEEDPRINT_FLAG"] = value;
            }
        }
    }
    public class R_SN_MAC
    {
        public double? PRINTED_COUNT { get; set; }
        public string SEQNO { get; set; }
        public string DATA1 { get; set; }
        public string TASK_NO { get; set; }
        public string BOXSN { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string ID { get; set; }
        public string SN { get; set; }
        public string SUBSN { get; set; }
        public string WO { get; set; }
        public double? PRINT_NO { get; set; }
        public double? STEP { get; set; }
        public string SUBSN_TYPE { get; set; }
        public string ASSIGN_FLAG { get; set; }
        public string NEEDPRINT_FLAG { get; set; }
    }
}
