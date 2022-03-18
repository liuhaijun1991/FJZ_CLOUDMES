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
    public class T_R_MRB_GT : DataObjectTable
    {
        public T_R_MRB_GT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
            RowType = typeof(Row_R_MRB_GT);
            TableName = "R_MRB_GT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public T_R_MRB_GT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MRB_GT);
            TableName = "R_MRB_GT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 通過工單查詢sap_flag=0的記錄
        /// </summary>
        /// <param name="strWo">工單號</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_MRB_GT GetByWOAndSAPFlageIsZero(string strWo, OleExec DB)
        {
            string strSql = $@" select * from r_mrb_gt where workorderno=:wono and sap_flag='0'";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":wono", strWo);
            DataTable table = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            R_MRB_GT result = new R_MRB_GT();
            if (table.Rows.Count > 0)
            {
                Row_R_MRB_GT ret = (Row_R_MRB_GT)NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 通過from_Storage查詢sap_flag=0的記錄
        /// </summary>
        /// <param name="strWo">工單號</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_MRB_GT GetByFromStorageAndSAPFlageIsZero(string fromStorage, OleExec DB)
        {
            string strSql = $@" select * from r_mrb_gt where from_storage=:wono and sap_flag='0'";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":wono", fromStorage);
            DataTable table = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            R_MRB_GT result = new R_MRB_GT();
            if (table.Rows.Count > 0)
            {
                Row_R_MRB_GT ret = (Row_R_MRB_GT)NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public int updateTotalQTYAddOne(string strWo, string user_emp, OleExec DB)
        {
            string strSql = $@" update r_mrb_gt set total_QTY=case when (total_QTY is null) then 1 else (total_QTY+1) end,EDIT_EMP=:userno,EDIT_TIME=sysdate where workorderno=:wono and SAP_FLAG='0'";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":userno", user_emp);
            paramet[1] = new OleDbParameter(":wono", strWo);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public int updateTotalQTYAddOne(string strWo, string user_emp,string confirmedFlag, OleExec DB)
        {
            string strSql = $@" update r_mrb_gt set total_QTY=case when (total_QTY is null) then 1 else (total_QTY+1) end,EDIT_EMP=:userno,EDIT_TIME=sysdate where workorderno=:wono and SAP_FLAG='0' and CONFIRMED_FLAG=:confirmedFlag";
            OleDbParameter[] paramet = new OleDbParameter[3];
            paramet[0] = new OleDbParameter(":userno", user_emp);
            paramet[1] = new OleDbParameter(":wono", strWo);
            paramet[2] = new OleDbParameter(":confirmedFlag", confirmedFlag); 
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        public int Add(R_MRB_GT newMRBGT, OleExec DB)
        {
            string strSql = $@"insert into r_mrb_gt(id,workorderno,sap_station_code,from_storage,to_storage,total_qty,confirmed_flag,zcpp_flag,sap_flag,skuno,sap_message,edit_emp,edit_time)";
            strSql = strSql + $@"values(:id,:workorderno,:sap_station_code,:from_storage,:to_storage,:total_qty,:confirmed_flag,:zcpp_flag,:sap_flag,:skuno,:sap_message,:edit_emp,sysdate)";
            OleDbParameter[] paramet = new OleDbParameter[]
                {
                    new OleDbParameter(":id", newMRBGT.ID),
                    new OleDbParameter(":workorderno", newMRBGT.WORKORDERNO),
                    new OleDbParameter(":sap_station_code", newMRBGT.SAP_STATION_CODE),
                    new OleDbParameter(":from_storage", newMRBGT.FROM_STORAGE),
                    new OleDbParameter(":to_storage", newMRBGT.TO_STORAGE),
                    new OleDbParameter(":total_qty", newMRBGT.TOTAL_QTY),
                    new OleDbParameter(":confirmed_flag", newMRBGT.CONFIRMED_FLAG),
                    new OleDbParameter(":zcpp_flag", newMRBGT.ZCPP_FLAG),
                    new OleDbParameter(":sap_flag", newMRBGT.SAP_FLAG),
                    new OleDbParameter(":skuno", newMRBGT.SKUNO),
                    new OleDbParameter(":sap_message", newMRBGT.SAP_MESSAGE),
                    new OleDbParameter(":edit_emp", newMRBGT.EDIT_EMP)
                };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;

        }

        public List<R_MRB_GT> GetByWoAndSapCode(string Wo, string SapCode,string CompletedFlag, OleExec DB)
        {
            return DB.ORM.Queryable<R_MRB_GT>().Where(t => t.WORKORDERNO == Wo && t.SAP_STATION_CODE == SapCode && t.CONFIRMED_FLAG== CompletedFlag).ToList();
        }

        public int Insert(R_MRB_GT RMG, OleExec DB)
        {
            return DB.ORM.Insertable<R_MRB_GT>(RMG).ExecuteCommand();
        }

        public int Update(R_MRB_GT RMG, OleExec DB)
        {
            return DB.ORM.Updateable<R_MRB_GT>(RMG).Where(t => t.ID == RMG.ID).ExecuteCommand();
        }

        public int updateTotalQTYAddOneByID(string MRBGT_ID, string user_emp, OleExec DB)
        {
            string strSql = $@" update r_mrb_gt set total_QTY=case when (total_QTY is null) then 1 else (total_QTY+1) end,EDIT_EMP=:userno,EDIT_TIME=sysdate where id=:MRBGT_ID";
            OleDbParameter[] paramet = new OleDbParameter[2];
            paramet[0] = new OleDbParameter(":userno", user_emp);
            paramet[1] = new OleDbParameter(":MRBGT_ID", MRBGT_ID);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
    }
    public class Row_R_MRB_GT : DataObjectBase
    {
        public Row_R_MRB_GT(DataObjectInfo info) : base(info)
        {

        }
        public R_MRB_GT GetDataObject()
        {
            R_MRB_GT DataObject = new R_MRB_GT();
            DataObject.ID = this.ID;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SAP_STATION_CODE = this.SAP_STATION_CODE;
            DataObject.FROM_STORAGE = this.FROM_STORAGE;
            DataObject.TO_STORAGE = this.TO_STORAGE;
            DataObject.TOTAL_QTY = this.TOTAL_QTY;
            DataObject.CONFIRMED_FLAG = this.CONFIRMED_FLAG;
            DataObject.ZCPP_FLAG = this.ZCPP_FLAG;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SAP_MESSAGE = this.SAP_MESSAGE;
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
        public string SAP_STATION_CODE
        {
            get
            {
                return (string)this["SAP_STATION_CODE"];
            }
            set
            {
                this["SAP_STATION_CODE"] = value;
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
        public double? TOTAL_QTY
        {
            get
            {
                return (double?)this["TOTAL_QTY"];
            }
            set
            {
                this["TOTAL_QTY"] = value;
            }
        }
        public string CONFIRMED_FLAG
        {
            get
            {
                return (string)this["CONFIRMED_FLAG"];
            }
            set
            {
                this["CONFIRMED_FLAG"] = value;
            }
        }
        public string ZCPP_FLAG
        {
            get
            {
                return (string)this["ZCPP_FLAG"];
            }
            set
            {
                this["ZCPP_FLAG"] = value;
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
        public string SAP_MESSAGE
        {
            get
            {
                return (string)this["SAP_MESSAGE"];
            }
            set
            {
                this["SAP_MESSAGE"] = value;
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
    public class R_MRB_GT
    {
        public string ID{get;set;}
        public string WORKORDERNO{get;set;}
        public string SAP_STATION_CODE{get;set;}
        public string FROM_STORAGE{get;set;}
        public string TO_STORAGE{get;set;}
        public double? TOTAL_QTY{get;set;}
        public string CONFIRMED_FLAG{get;set;}
        public string ZCPP_FLAG{get;set;}
        public string SAP_FLAG{get;set;}
        public string SKUNO{get;set;}
        public string SAP_MESSAGE{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}