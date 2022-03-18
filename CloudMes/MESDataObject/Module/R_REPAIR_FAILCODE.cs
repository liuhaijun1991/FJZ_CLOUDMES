using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using MESDataObject.Common;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_FAILCODE : DataObjectTable
    {
        public T_R_REPAIR_FAILCODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_FAILCODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_FAILCODE);
            TableName = "R_REPAIR_FAILCODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_REPAIR_FAILCODE> GetFailCodeBySN(OleExec sfcdb, string sn)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_REPAIR_FAILCODE row_fail = null;
            List<R_REPAIR_FAILCODE> repairFailCodes = new List<R_REPAIR_FAILCODE>();
            string sql = $@"select * from {TableName} where sn='{sn.Replace("'", "''")}' ";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_fail = (Row_R_REPAIR_FAILCODE) this.NewRow();
                        row_fail.loadData(dr);
                        repairFailCodes.Add(row_fail.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return repairFailCodes;
        }

        public Row_R_REPAIR_FAILCODE GetByFailCodeID(string _FailCodeID, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select * from r_repair_failcode where ID='{_FailCodeID.Replace("'", "''")}' and REPAIR_FLAG='0' ";
                DataSet res = DB.ExecSelect(strsql);
                if (res.Tables[0].Rows.Count>0)
                {
                    Row_R_REPAIR_FAILCODE Ret = (Row_R_REPAIR_FAILCODE)this.GetObjByID(_FailCodeID, DB);
                    return Ret;
                }
                else
                {
                    return null;
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "FailCodeID:" + _FailCodeID });
                    //    throw new MESReturnMessage(errMsg);
                }                
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }


        public List<R_REPAIR_FAILCODE> CheckSNRepairFinishAction(OleExec sfcdb, string sn,string RepairMainID)
        {
            if (string.IsNullOrEmpty(sn)||string.IsNullOrEmpty(RepairMainID))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_REPAIR_FAILCODE row_fail = null;
            List<R_REPAIR_FAILCODE> repairFailCodes = new List<R_REPAIR_FAILCODE>();
            string sql = $@" select *from r_repair_failcode where  repair_main_id ='{RepairMainID}' and sn ='{sn}' and id not in (select repair_failcode_id from r_repair_action where sn='{sn}')";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_fail = (Row_R_REPAIR_FAILCODE)this.NewRow();
                        row_fail.loadData(dr);
                        repairFailCodes.Add(row_fail.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return repairFailCodes;
        }

        public DataTable SelectFailCodeBySN(string sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select a.id,a.repair_main_id,a.sn,a.fail_code,a.fail_location,a.fail_category,a.fail_process,b.fail_station,a.fail_time,a.fail_emp,a.description,a.create_time,a.repair_flag,a.edit_time,a.edit_emp  from r_repair_failcode a,r_repair_main b
            where a.repair_main_id=b.id and a.repair_flag='0' and a.sn='{sn}' ";
            //string strSql = $@"select * from r_repair_failcode where repair_flag='0' and sn='{sn}'";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;

        }

        public int ReplaceSnRepairFailCode(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;


            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_REPAIR_FAILCODE R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            

            return result;
        }

        public R_REPAIR_FAILCODE GetFailCodeById(string FailCodeId, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(t => t.ID == FailCodeId).ToList().FirstOrDefault();
        }

        public List<R_REPAIR_FAILCODE> GetFailCodeListBySNAndMainID(OleExec sfcdb, string sn, string mainID, string flag)
        {
            return sfcdb.ORM.Queryable<R_REPAIR_FAILCODE>().Where(r => r.SN == sn && r.MAIN_ID == mainID && r.REPAIR_FLAG == flag).ToList();
        }

        public bool FailCodeIsExist(OleExec sfcdb, string sn, string mainID, string failCode)
        {
            return sfcdb.ORM.Queryable<R_REPAIR_FAILCODE>().Any(r => r.SN == sn && r.MAIN_ID == mainID && r.REPAIR_FLAG == "0" && r.FAIL_CODE == failCode);
        }

        public R_REPAIR_FAILCODE GetRepairSN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_FAILCODE>().Where(t => t.SN == SN).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }

        /// <summary>
        /// WZW 维修保存按钮需要
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void NsgrokRepairSave(Dictionary<string, string> DicStation, Dictionary<string, OleExec> DicDB, ref Dictionary<string, string> DicRef)
        {
            string SN = DicStation["SN"].ToString();
            string PCBASN = DicStation["PCBASN"].ToString();
            string Location = DicStation["Location"].ToString();
            string NewTrackNo = DicStation["NewTrackNo"].ToString();
            string TrackNo = DicStation["TrackNo"].ToString();
            string ActionCode = DicStation["ActionCode"].ToString();
            string RootCause = DicStation["RootCause"].ToString();
            string TR_SN = DicStation["TR_SN"].ToString();
            string Component = DicStation["Component"].ToString();
            string MFR_Name = DicStation["MFR_Name"].ToString();
            string VendName = DicStation["VendName"].ToString();
            string Date_Code = DicStation["Date_Code"].ToString();
            string Lot_Code = DicStation["Lot_Code"].ToString();
            string PartDes = DicStation["PartDes"].ToString();
            string ID = DicStation["ID"].ToString();
            string Process = DicStation["Process"].ToString();
            string Description = DicStation["Description"].ToString();
            string SolderNo = DicStation["SolderNo"].ToString();
            string PackType = DicStation["PackType"].ToString();
            string NEW_KP = DicStation["NEW_KP"].ToString();
            string Old_KP = DicStation["Old_KP"].ToString();
            string ReplaceNo = DicStation["ReplaceNo"].ToString();
            string FailSysPtom = DicStation["FailSysPtom"].ToString();
            string LocationChkbox = DicStation["LocationChkbox"];
            string FailCodeID = DicStation["FailCodeID"];
            string BU = DicStation["BU"];
            string EMP = DicStation["EMP"];
            string StationName = DicStation["STATIONNAME"];
            DicStation["FAILDESCRIPTION"] = "AutoRepair";
            DicStation["STATUS"] = "P";
            int StrSolderNo = Convert.ToInt32(DicStation["StrSolder"].ToString());
            List<string> ListString = new List<string>();

            OleExec DB = DicDB["SFCDB"];
            OleExec APDB = DicDB["APDB"];

            T_R_SN RSN = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            R_SN RSNINFO = RSN.LoadSN(SN, DB);
            T_R_REPAIR_REPLACE RRepairReplace = new T_R_REPAIR_REPLACE(DB, DB_TYPE_ENUM.Oracle);
            R_REPAIR_REPLACE RRepairReplaceINFO = new R_REPAIR_REPLACE();
            List<R_REPAIR_REPLACE> ListRRepairReplace = RRepairReplace.GetBYRepairNo(ReplaceNo, DB);
            if (ListRRepairReplace.Count < 0)
            {
                string RepairRepactID = RRepairReplace.GetNewID(BU, DB);
                RRepairReplaceINFO.ID = RepairRepactID;
                RRepairReplaceINFO.WO = RSNINFO.WORKORDERNO;
                RRepairReplaceINFO.SN_ID = RSNINFO.ID;
                RRepairReplaceINFO.SN = SN;
                RRepairReplaceINFO.FAIL_STATION = StationName;
                RRepairReplaceINFO.LOCATION = Location;
                RRepairReplaceINFO.TRACK_NO = TrackNo;
                RRepairReplaceINFO.REPLACE_NO = ReplaceNo;
                RRepairReplaceINFO.FAIL_EMP = EMP;
                RRepairReplaceINFO.EDIT_EMP = EMP;
                RRepairReplaceINFO.EDIT_TIME = RRepairReplace.GetDBDateTime(DB);
                int InsertRepairReplacenUM = RRepairReplace.InsertRepairReplace(RRepairReplaceINFO, DB);
                if (InsertRepairReplacenUM < 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { SN }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105092618", new string[] { ReplaceNo }));
            }
            T_R_REPAIR_TRANSFER RRepairTransfer = new T_R_REPAIR_TRANSFER(DB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_TRANSFER> ListRRepairTransfer = RRepairTransfer.GetSNCheck(SN, DB);
            if (ListRRepairTransfer.Count < 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105111047", new string[] { SN }));
            }
            T_R_TESTRECORD RTestRecord = new T_R_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
            List<R_TESTRECORD> ListRTestRecord = RTestRecord.GetListBYSNNOStationFStatus(SN, "F", "F", DB);
            if (ListRTestRecord.Count > 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181105111210", new string[] { SN }));
            }
            ListString.Add("F");
            ListString.Add("A");
            ListString.Add("S");
            List<R_TESTRECORD> ListRTestRecordListStus = RTestRecord.GetListBYSNNOStationFListStatus(SN, "XRAY", ListString, DB);
            if (ListRTestRecordListStus.Count > 0)
            {
                int UpdateNum = RTestRecord.UpdateSNINFO(SN, DB);
                if (UpdateNum <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { SN }));
                }
                List<R_TESTRECORD> ListRTestRecordINFO = RTestRecord.GetListBYSNDesc(SN, DB);
                if (ListRTestRecordINFO[0].TEST_INFO != "" || ListRTestRecordINFO[0].TEST_INFO != null)
                {
                    DicStation["FAILDESCRIPTION"] = ListRTestRecordINFO[0].TEST_INFO;
                }

                if (("FAN,PS,CPU,DDR,EUSB,SATA,CF".IndexOf(Location) == -1) && PCBASN != "")
                {
                    if (FailSysPtom == "1")
                    {
                        DicStation["STATUS"] = "A";
                    }
                    T_R_REPAIR_MAIN RRepairMain = new T_R_REPAIR_MAIN(DB, DB_TYPE_ENUM.Oracle);
                    RRepairMain.GetMaxSNTO(DicStation, DicDB, ref DicRef);
                }
                else
                {
                    if (FailSysPtom.ToUpper() == "ON")
                    {
                        T_R_REPAIR_OFFLINE RRepairOffline = new T_R_REPAIR_OFFLINE(DB, DB_TYPE_ENUM.Oracle);
                        RRepairOffline.AutoRepairSave(DicStation, DicDB, ref DicRef);
                    }
                    else
                    {
                        RRepairTransfer.common(DicStation, DicDB, ref DicRef);
                    }
                }
            }
        }

        public int InsertFailCode(string FailCodeID, string SN, string FailCode, string FailLocation, string FailTime, string TestName, string editemp, OleExec DB)
        {
            //獲取R_REPAIR_MAIN表中的數據,創建時間與ID
            string QueryDateStr = $@"
                        select CREATE_TIME,ID from R_REPAIR_MAIN where SN=:sn and rownum<2 order by CREATE_TIME desc
                    ";
            OleDbParameter[] RepairMainparamet = new OleDbParameter[]
           {
               new OleDbParameter(":sn",SN)
           };
            DataSet RepairMainDS = DB.ExecSelect(QueryDateStr, RepairMainparamet);
            DateTime dt = Convert.ToDateTime(RepairMainDS.Tables[0].Rows[0][0]);
            string CreateTime_Result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            string RepairMainID = RepairMainDS.Tables[0].Rows[0][1].ToString();

            //從C_ERROR_CODE表中查詢ENGLISH_DESCRIPTION
            string QueryErrorCodeStr = $@"
            select ENGLISH_DESCRIPTION from C_ERROR_CODE where ERROR_CODE=:failcode
            ";
            OleDbParameter[] ErrorCodeparamet = new OleDbParameter[]
          {
               new OleDbParameter(":failcode",FailCode)
          };
            DataSet ErrorCodeDS = DB.ExecSelect(QueryErrorCodeStr, ErrorCodeparamet);
            string ErrorCodeDES = ErrorCodeDS.Tables[0].Rows[0][0].ToString();


            //插入R_REPAIR_FAILCODE表
            string InsertSql = $@"
                insert into R_REPAIR_FAILCODE(ID,REPAIR_MAIN_ID,SN,FAIL_CODE,FAIL_LOCATION,FAIL_CATEGORY,FAIL_PROCESS,FAIL_TIME,FAIL_EMP,DESCRIPTION,CREATE_TIME,REPAIR_FLAG,EDIT_TIME,EDIT_EMP)
                    values(:id,:repairmainid,:sn,:failcode,
                           :faillocation,:failcategory,:english_description,TO_DATE(:failtime,'yyyy-mm-dd HH24:MI:SS'),
                           :failemp,:english_description,TO_DATE(:createtime,'yyyy-mm-dd HH24:MI:SS'),:repairflag,
                           TO_DATE(:edittime,'yyyy-mm-dd HH24:MI:SS'),:editemp)
            ";
            OleDbParameter[] InsertSqlparamet = new OleDbParameter[]
            {
            new OleDbParameter(":id",FailCodeID),
            new OleDbParameter(":repairmainid",RepairMainID),
            new OleDbParameter(":sn",SN),
            new OleDbParameter(":failcode",FailCode),
            new OleDbParameter(":faillocation",FailLocation),
            new OleDbParameter(":failcategory",""),
            new OleDbParameter(":failtime",CreateTime_Result),
            new OleDbParameter(":english_description",ErrorCodeDES),
            new OleDbParameter(":failemp",TestName),
            new OleDbParameter(":createtime",CreateTime_Result),
            new OleDbParameter(":repairflag","0"),
            new OleDbParameter(":edittime",CreateTime_Result),
            new OleDbParameter(":editemp",editemp)
            };


            //DataSet result =DB.ExecSelect(InsertSql,InsertSqlparamet);
            int result = DB.ExecuteNonQuery(InsertSql, CommandType.Text, InsertSqlparamet);
            //int result =DB.ExecuteNonQuery(InsertSql,CommandType.Text,InsertSqlparamet);
            return result;
        }
        public int InsertRepairFailcode(object InsertSql, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            DataTable dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                result = DB.ExecSqlNoReturn(InsertSql.ToString(), null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }
        public R_REPAIR_FAILCODE GetById(string id, OleExec DB)
        {
            string strSql = $@"select * from R_REPAIR_FAILCODE where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", OleDbType.VarChar, 240) };
            paramet[0].Value = id;
            R_REPAIR_FAILCODE RREPAIRFAILCODE = new R_REPAIR_FAILCODE();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_R_REPAIR_FAILCODE ret = (Row_R_REPAIR_FAILCODE)NewRow();
                ret.loadData(res.Rows[0]);
                RREPAIRFAILCODE = ret.GetDataObject();
            }
            return RREPAIRFAILCODE;
        }
        public int UpdateFailSNRepairFlag(R_REPAIR_FAILCODE RREPAIR_FAILCODE, OleExec DB)
        {
            return DB.ORM.Updateable<R_REPAIR_FAILCODE>(RREPAIR_FAILCODE).Where(t => t.SN == RREPAIR_FAILCODE.SN && t.FAIL_CODE == RREPAIR_FAILCODE.FAIL_CODE && t.F_LOCATION == RREPAIR_FAILCODE.F_LOCATION).ExecuteCommand();
        }
        public int Save(OleExec SFCDB, R_REPAIR_FAILCODE failCodeObject)
        {
            return SFCDB.ORM.Insertable<R_REPAIR_FAILCODE>(failCodeObject).ExecuteCommand();
        }

    }
    public class Row_R_REPAIR_FAILCODE : DataObjectBase
    {
        public Row_R_REPAIR_FAILCODE(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_FAILCODE GetDataObject()
        {
            R_REPAIR_FAILCODE DataObject = new R_REPAIR_FAILCODE();
            DataObject.ID = this.ID;
            DataObject.MAIN_ID = this.REPAIR_MAIN_ID;
            DataObject.SN = this.SN;
            DataObject.FAIL_CODE = this.FAIL_CODE;
            DataObject.F_LOCATION = this.FAIL_LOCATION;
            DataObject.F_CATEGORY = this.FAIL_CATEGORY;
            DataObject.F_PROCESS = this.FAIL_PROCESS;
            DataObject.FAIL_TIME = this.FAIL_TIME;
            DataObject.FAIL_EMP = this.FAIL_EMP;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.REPAIR_FLAG = this.REPAIR_FLAG;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string REPAIR_MAIN_ID
        {
            get
            {
                return (string)this["REPAIR_MAIN_ID"];
            }
            set
            {
                this["REPAIR_MAIN_ID"] = value;
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
        public string FAIL_CODE
        {
            get
            {
                return (string)this["FAIL_CODE"];
            }
            set
            {
                this["FAIL_CODE"] = value;
            }
        }
        public string FAIL_LOCATION
        {
            get
            {
                return (string)this["FAIL_LOCATION"];
            }
            set
            {
                this["FAIL_LOCATION"] = value;
            }
        }
        public string FAIL_CATEGORY
        {
            get
            {
                return (string)this["FAIL_CATEGORY"];
            }
            set
            {
                this["FAIL_CATEGORY"] = value;
            }
        }
        public string FAIL_PROCESS
        {
            get
            {
                return (string)this["FAIL_PROCESS"];
            }
            set
            {
                this["FAIL_PROCESS"] = value;
            }
        }
        public DateTime? FAIL_TIME
        {
            get
            {
                return (DateTime?)this["FAIL_TIME"];
            }
            set
            {
                this["FAIL_TIME"] = value;
            }
        }
        public string FAIL_EMP
        {
            get
            {
                return (string)this["FAIL_EMP"];
            }
            set
            {
                this["FAIL_EMP"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
        public string REPAIR_FLAG
        {
            get
            {
                return (string)this["REPAIR_FLAG"];
            }
            set
            {
                this["REPAIR_FLAG"] = value;
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
    }
    public class R_REPAIR_FAILCODE
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID{get;set;}
        [SugarColumn(ColumnName = "REPAIR_MAIN_ID")]
        public string MAIN_ID{get;set;}
        public string SN{get;set;}
        public string FAIL_CODE{get;set; }
        [SugarColumn(ColumnName = "FAIL_LOCATION")]
        public string F_LOCATION{get;set;}
        [SugarColumn(ColumnName = "FAIL_CATEGORY")]
        public string F_CATEGORY{get;set; }
        [SugarColumn(ColumnName = "FAIL_PROCESS")]
        public string F_PROCESS{get;set;}
        public DateTime? FAIL_TIME{get;set;}
        public string FAIL_EMP{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? CREATE_TIME{get;set;}
        public string REPAIR_FLAG{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }

    public enum R_REPAIR_FAILCODE_CATEGORY
    {
        [EnumValue("SYMPTOM")]
        SYMPTOM,
        [EnumValue("DEFECT")]
        DEFECT

    }
}