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
    public class T_R_MRB : DataObjectTable
    {
        public T_R_MRB(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
            RowType = typeof(Row_R_MRB);
            TableName = "R_MRB".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public T_R_MRB(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MRB);
            TableName = "R_MRB".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int ReplaceRMrb(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE r_mrb R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }


            return result;
        }

        /// <summary>
        /// 取到 SN 在R_MRB的數據
        /// </summary>
        /// <param name="strStartSN"></param>
        /// <param name="strEndSN"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<R_MRB> GetMrbBySN(string sn, OleExec DB)
        {

            string strSql = $@" select* from r_mrb where sn=:sn and mrb_flag='1' ";

            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":sn", sn) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_MRB> listSn = new List<R_MRB>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_MRB ret = (Row_R_MRB)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            return listSn;
        }

        public List<R_MRB> GetMrbInformationBySN(string sn, OleExec DB)
        {

            string strSql = $@" select* from r_mrb where sn=:sn and mrb_flag='1' ";
            //string strSql = $@" select* from r_mrb where sn=:sn and and rework_wo is null ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":sn", sn) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_MRB> listSn = new List<R_MRB>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_MRB ret = (Row_R_MRB)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            return listSn;
        }
        public int Add(R_MRB newMrb, OleExec DB)
        {
            string strSql = $@"insert into r_mrb( id,sn,workorderno,next_station,skuno,from_storage,to_storage,rework_wo,create_emp,create_time,mrb_flag,sap_flag,edit_emp,edit_time)";
            strSql = strSql + $@"values(:id,:sn,:workorderno,:next_station,:skuno,:from_storage,:to_storage,:rework_wo,:create_emp,:create_time,:mrb_flag,:sap_flag,:edit_emp,:edit_time)";
            OleDbParameter[] paramet = new OleDbParameter[]
            {
            new OleDbParameter(":id", newMrb.ID),
            new OleDbParameter(":sn", newMrb.SN),
            new OleDbParameter(":workorderno", newMrb.WORKORDERNO),
            new OleDbParameter(":next_station", newMrb.NEXT_STATION),
            new OleDbParameter(":skuno", newMrb.SKUNO),
            new OleDbParameter(":from_storage", newMrb.FROM_STORAGE),
            new OleDbParameter(":to_storage", newMrb.TO_STORAGE),
            new OleDbParameter(":rework_wo", newMrb.REWORK_WO),
            new OleDbParameter(":create_emp", newMrb.CREATE_EMP),
            new OleDbParameter(":create_time", newMrb.CREATE_TIME),
            new OleDbParameter(":mrb_flag", newMrb.MRB_FLAG),
            new OleDbParameter(":sap_flag", newMrb.SAP_FLAG),
            new OleDbParameter(":edit_emp", newMrb.EDIT_EMP),
            new OleDbParameter(":edit_time", newMrb.EDIT_TIME)
            };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public int OutMrbUpdate(string reworkno,string userEmp, string snno, OleExec DB)
        {
            string strSql = $@"update r_mrb set rework_wo=:rework_wo,mrb_flag='0',edit_emp=:edit_emp,edit_time=sysdate  where sn=:sn and mrb_flag='1' and rework_wo is null";          
            //string strSql = $@"update r_mrb set rework_wo=:rework_wo,edit_emp=:edit_emp,edit_time=sysdate  where sn=:sn and rework_wo is null";
            OleDbParameter[] paramet = new OleDbParameter[]
            {
            new OleDbParameter(":rework_wo",reworkno),
            new OleDbParameter(":edit_emp", userEmp),
            new OleDbParameter(":sn", snno)
            };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public DataSet GetSNCountByWO(string wono,OleExec sfcdb)
        {
          string  sqlString = $@"select count(distinct(r.sn ) ) C from r_mrb r where workorderno= '{wono}'";
          DataSet  dtset = sfcdb.ExecuteDataSet(sqlString, CommandType.Text);
          return dtset;
        }

        #region for MRB_ASSYRETURN

        /// <summary>
        /// 獲取全局 MRB 管控狀態
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool GetMRBControl(OleExec DB)
        {
            bool State = false;
            T_C_CONTROL ControlTable = new T_C_CONTROL(DB, DBType);
            C_CONTROL Control=ControlTable.GetControlByName("MRB_CHECK_CTRL", DB);
            if (Control != null)
            {
                if (Control.CONTROL_VALUE.ToUpper().Equals("OPEN"))
                {
                    State = true;
                }
            }
            return State;
        }

        /// <summary>
        /// add by ZGJ 2018-03-16
        /// 判斷是否有配置該機種不需要經過TS101檢測
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool SkuCheckTS101(string Skuno, OleExec DB)
        {
            bool NeedCheck = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@"SELECT * FROM C_SKU_DETAIL S WHERE S.CATEGORY_NAME='MRB_NOT_CHECK_TS101' 
                   AND S.CATEGORY='MRB_CTRL' AND S.SKUNO='{Skuno}' AND UPPER(VALUE)='TRUE'";
            dt = DB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count > 0)
            {
                NeedCheck = true;
            }
            return NeedCheck;
        }

        /// <summary>
        /// add by ZGJ 2018-03-16
        /// 判斷該產品是不是有預報廢
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsPreScrap(string SerialNumber, OleExec DB)
        {
            bool PreScrap = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@"SELECT NEXT_STATION FROM R_SN_STATION_DETAIL WHERE SN='{SerialNumber}' 
                        ORDER BY EDIT_TIME DESC";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0 && dt.Rows[0]["NEXT_STATION"].Equals("PRE_SCRAP"))
            {
                PreScrap = true;
            }
            return PreScrap;
        }

        /// <summary>
        /// add by ZGJ 2018-03-16
        /// 判斷該 SN 是否有在 HW 系統中測試過
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool HasHWTest(string SerialNumber, OleExec DB)
        {
            bool HadTest = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            string ConfigStation = "TS101";

            //HWD 的產品以南寧的 Site Code：DW 開頭的SN
            //如果不是以 DW 開頭，直接就返回true
            if (SerialNumber.StartsWith("DW"))
            {
                sql = $@"SELECT VALUE FROM C_SKU_DETAIL S  
                        WHERE S.SKUNO=(SELECT SKUNO FROM R_SN WHERE SN='{SerialNumber}' AND ROWNUM=1)
                        AND S.CATEGORY='MRB_CTRL'
                        AND S.CATEGORY_NAME='HW_TEST_STATION'
                     ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    ConfigStation = dt.Rows[0]["VALUE"].ToString();
                    sql = $@"SELECT * FROM ITRANLOTHIS WHERE LOT_ID='{SerialNumber}' ORDER BY IF_SEQ DESC";
                    dt = DB.ExecSelect(sql, null).Tables[0];
                    if (dt.Rows.Count > 0 && dt.Rows[0]["OPER"].ToString().Equals(ConfigStation))
                    {
                        HadTest = true;
                    }
                }
                return HadTest;
            }
            else
            {
                return true;
            }
        }
        
        /// <summary>
        ///  add by ZGJ 2018-03-16
        ///  判斷該 SN 是否有 MRB 過
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="WorkOrder"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool HadMrbed(string SerialNumber, OleExec DB)
        {
            bool isMrbed = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@"SELECT * FROM R_MRB WHERE SN='{SerialNumber}' AND REWORK_WO IS NULL AND MRB_FLAG='1'";
            dt = DB.ExecSelect(sql, null).Tables[0];
            if (dt.Rows.Count > 0)
            {
                isMrbed = true;
            }

            return isMrbed;
        }

        public int Insert(R_MRB Mrb, OleExec DB)
        {
            return DB.ORM.Insertable<R_MRB>(Mrb).ExecuteCommand();
        }

        public List<R_MRB> GetMrbBySnAndStation(string Sn, string Station, OleExec DB)
        {
            return DB.ORM.Queryable<R_MRB>().Where(t => t.SN == Sn && t.MRB_FLAG=="1").ToList();
        }

        #endregion

        public bool IsReworked(string sn, OleExec DB)
        {
            return DB.ORM.Queryable<R_MRB>().Any(r => r.SN == sn && !SqlSugar.SqlFunc.IsNullOrEmpty(r.REWORK_WO));
        }

        public R_MRB GetMrbBySNOrderDesc(string sn, OleExec DB)
        {
            string strSql = $@" select* from r_mrb where sn=:sn and mrb_flag='0' order by edit_time desc ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":sn", sn) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            //List<R_MRB> listSn = new List<R_MRB>();
            R_MRB MrbSN = null;
            if (res.Rows.Count > 0)
            {
                Row_R_MRB ret = (Row_R_MRB)NewRow();
                ret.loadData(res.Rows[0]);
                MrbSN = ret.GetDataObject();
            }
            return MrbSN;
        }

        public List<R_MRB> GetMrbList(OleExec DB, string sn)
        {
            return DB.ORM.Queryable<R_MRB>().Where(r => r.SN == sn).OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }

        public R_MRB GetRecerdBySNAndWO(OleExec DB, string sn, string wo)
        {
            return DB.ORM.Queryable<R_MRB>().Where(r => r.SN == sn && r.WORKORDERNO == wo).ToList().FirstOrDefault();
        }
    }
    public class Row_R_MRB : DataObjectBase
    {
        public Row_R_MRB(DataObjectInfo info) : base(info)
        {

        }
        public R_MRB GetDataObject()
        {
            R_MRB DataObject = new R_MRB();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.SKUNO = this.SKUNO;
            DataObject.FROM_STORAGE = this.FROM_STORAGE;
            DataObject.TO_STORAGE = this.TO_STORAGE;
            DataObject.REWORK_WO = this.REWORK_WO;
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.MRB_FLAG = this.MRB_FLAG;
            DataObject.SAP_FLAG = this.SAP_FLAG;
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
        public string NEXT_STATION
        {
            get
            {
                return (string)this["NEXT_STATION"];
            }
            set
            {
                this["NEXT_STATION"] = value;
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
        public string FROM_STORAGE
        {
            get
            {
                return (string)this["FROM_STORAGE"];
            }
            set
            {
                this["FROM_STORAGE"] = value;
            }
        }
        public string TO_STORAGE
        {
            get
            {
                return (string)this["TO_STORAGE"];
            }
            set
            {
                this["TO_STORAGE"] = value;
            }
        }
        public string REWORK_WO
        {
            get
            {
                return (string)this["REWORK_WO"];
            }
            set
            {
                this["REWORK_WO"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public string MRB_FLAG
        {
            get
            {
                return (string)this["MRB_FLAG"];
            }
            set
            {
                this["MRB_FLAG"] = value;
            }
        }
        public string SAP_FLAG
        {
            get
            {
                return (string)this["SAP_FLAG"];
            }
            set
            {
                this["SAP_FLAG"] = value;
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
    public class R_MRB
    {
        public string ID{get;set;}
        public string SN{get;set;}
        public string WORKORDERNO{get;set;}
        public string NEXT_STATION{get;set;}
        public string SKUNO{get;set;}
        public string FROM_STORAGE{get;set;}
        public string TO_STORAGE{get;set;}
        public string REWORK_WO{get;set;}
        public string CREATE_EMP{get;set;}
        public DateTime? CREATE_TIME{get;set;}
        public string MRB_FLAG{get;set;}
        public string SAP_FLAG{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}