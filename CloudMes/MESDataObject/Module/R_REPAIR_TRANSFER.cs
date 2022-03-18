using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_TRANSFER : DataObjectTable
    {
        public T_R_REPAIR_TRANSFER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_TRANSFER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_TRANSFER);
            TableName = "R_REPAIR_TRANSFER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_REPAIR_TRANSFER> GetReSNbysn(string RelSn, OleExec DB)
        {

            string strSql = $@" SELECT *

                                  FROM R_REPAIR_TRANSFER
                                 WHERE (REPAIR_MAIN_ID, SN) IN (SELECT ID, SN
                                                                  FROM R_REPAIR_MAIN
                                                                 WHERE (SN, EDIT_TIME) IN (  SELECT SN,
                                                                                                    MAX (
                                                                                                       EDIT_TIME)
                                                                                               FROM R_REPAIR_MAIN
                                                                                              WHERE     SN =
                                                                                                           '{RelSn}'
                                                                                                    AND CLOSED_FLAG =
                                                                                                           '0'
                                                                                           GROUP BY SN)) ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }

        public List<R_REPAIR_TRANSFER> GetLastRepairedBySN(string RelSn, OleExec DB)
        {

            string strSql = $@" SELECT *

                                  FROM R_REPAIR_TRANSFER
                                 WHERE (REPAIR_MAIN_ID, SN) IN (SELECT ID, SN
                                                                  FROM R_REPAIR_MAIN
                                                                 WHERE (SN, EDIT_TIME) IN (  SELECT SN,
                                                                                                    MAX (
                                                                                                       EDIT_TIME)
                                                                                               FROM R_REPAIR_MAIN
                                                                                              WHERE     SN =
                                                                                                           '{RelSn}'
                                                                                                    AND CLOSED_FLAG =
                                                                                                           '1'
                                                                                           GROUP BY SN)) ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }

        public bool SNIsRepairIn(string sn,OleExec sfcdb)
        {
            string sql = $@"select * from r_repair_transfer where sn='{sn}' and closed_flag='0'";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<R_REPAIR_TRANSFER> GetSNCheck(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_TRANSFER>().Where(t => t.SN == SN).OrderBy(t => t.EDIT_TIME).ToList();
        }
        public List<R_REPAIR_TRANSFER> GetSNCheckInOut(string SN, string OUT, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_TRANSFER>().Where(t => t.SN == SN && t.CLOSED_FLAG == OUT).ToList();
        }
        /// <summary>
        /// WZW 维修保存按钮需要
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void common(Dictionary<string, string> DicStation, Dictionary<string, OleExec> DicDB, ref Dictionary<string, string> DicRef)
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
            //string FailDescription = DicStation["FAILDESCRIPTION"];
            //string STATUS = DicStation["STATUS"];
            int StrSolderNo = Convert.ToInt32(DicStation["SolderNo"].ToString());

            OleExec DB = DicDB["SFCDB"];
            OleExec APDB = DicDB["APDB"];
            T_R_TESTRECORD RTestRecord = new T_R_TESTRECORD(DB, DB_TYPE_ENUM.Oracle);
            List<R_TESTRECORD> ListRTestRecord = null;
            bool TestFile = RTestRecord.LH_NSDI_Check_TestFileCheck(SN, ref ListRTestRecord, DB);
            if (TestFile)
            {
                SolderNo = "";
                ReplaceNo = "1";
                PackType = "";
                VendName = "";
                PartDes = "";
            }
            else
            {
                if (Component.Length > 0)
                {
                    SolderNo = ReplaceNo;
                    ReplaceNo = "1";
                }
                else
                {
                    ReplaceNo = "0";
                    SolderNo = "";
                }
            }
            T_C_REPAIR_DAY TCREPAIRDAY = new T_C_REPAIR_DAY(DB, DB_TYPE_ENUM.Oracle);
            TCREPAIRDAY.InsertDB(DicStation, DicDB, ref DicRef);
        }
        public List<R_REPAIR_TRANSFER> GetBySNCheckINType(string INOUT, string SN, OleExec DB)
        {
            string StrSQL = $@"SELECT C.* FROM (
SELECT A.* FROM R_REPAIR_TRANSFER A,
(SELECT SN,MAX(EDIT_TIME)AS EDIT_TIME FROM R_REPAIR_TRANSFER GROUP BY SN,EDIT_TIME )B
WHERE A.SN=B.SN AND A.EDIT_TIME=B.EDIT_TIME)C
WHERE C.CLOSED_FLAG='{INOUT}' AND C.SN='{SN}'";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetSNOrderEnd(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_TRANSFER>().Where(t => t.SN == SN && t.CLOSED_FLAG == null).OrderBy(t => t.EDIT_TIME, OrderByType.Desc).ToList();
        }
        public List<R_REPAIR_TRANSFER> GetBySNCheckINEMP(string EMP, OleExec DB)
        {
            string StrSQL = $@"SELECT * FROM R_REPAIR_TRANSFER A  
    WHERE NOT EXISTS(SELECT * FROM R_REPAIR_ACTION WHERE SN=A.SN AND  EDIT_TIME>=A.IN_TIME )  
      AND NOT EXISTS(SELECT * FROM R_REPAIR_OFFLINE WHERE SN=A.SN AND EDIT_TIME>=A.IN_TIME )  
      AND SUBSTRC(A.SN,1)<>'#' AND A.CLOSED_FLAG='0' AND A.DESCRIPTION NOT LIKE 'is sub_unit;main_unit:%'  
      AND A.IN_RECEIVE_EMP='{EMP}'";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            //else
            //{
            //    return null;
            //}
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetBySNCountCheckIN(string SN, OleExec DB)
        {
            string StrSQL = $@"SELECT DISTINCT SN,COUNT(IN_TIME)AS CHECKINTIME FROM R_REPAIR_TRANSFER
WHERE SN='{SN}' GROUP BY SN ";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }
        public int InsertRepairCheckin(R_REPAIR_TRANSFER RRepirTransfer, OleExec DB)
        {
            return DB.ORM.Insertable<R_REPAIR_TRANSFER>(RRepirTransfer).ExecuteCommand();
        }
        public List<R_REPAIR_TRANSFER> GetSNBYRepairCheckINSN(string SN, OleExec DB)
        {
            string StrSQL = $@"SELECT * FROM R_REPAIR_TRANSFER WHERE SN='{SN}' AND CLOSED_FLAG='0' ORDER BY EDIT_TIME DESC";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetSNCheckINEMP(string SN, string CLOSED_FLAG, string RECEIVE_EMP, OleExec DB)
        {
            string StrSQL = $@"SELECT * FROM R_REPAIR_TRANSFER WHERE SN='{SN}' AND CLOSED_FLAG='{CLOSED_FLAG}' AND IN_RECEIVE_EMP='{RECEIVE_EMP}' ";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetSNBYEMP(string SN, OleExec DB)
        {
            string StrSQL = $@"SELECT * FROM R_REPAIR_TRANSFER WHERE  SN='{SN}' AND      
   EDIT_TIME=(SELECT MAX(EDIT_TIME) FROM R_REPAIR_TRANSFER WHERE SN='{SN}' )     ";
            DataTable res = DB.ExecSelect(StrSQL).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetBYSNDepartment(string RelSn, OleExec DB)
        {
            string strSql = $@" SELECT * FROM R_REPAIR_TRANSFER D WHERE D.SN='{RelSn}' AND D.CLOSED_FLAG='0'       
    AND D.DEPARTMENT IN ('REPAIR')      
    AND D.EDIT_TIME = (SELECT MAX(EDIT_TIME) FROM R_REPAIR_TRANSFER E WHERE SN = '{RelSn}') ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public int UpdateRepairTransfer(string CLOSED_FLAG, R_REPAIR_TRANSFER RRepairTransfer, OleExec DB)
        {
            return DB.ORM.Updateable<R_REPAIR_TRANSFER>(RRepairTransfer).Where(t => t.CLOSED_FLAG == CLOSED_FLAG && t.SN == RRepairTransfer.SN).ExecuteCommand();
        }
        public List<R_REPAIR_TRANSFER> GetSNBYSN(string RelSn, OleExec DB)
        {
            string strSql = $@" SELECT * FROM R_REPAIR_TRANSFER D WHERE D.SN='{RelSn}' AND D.CLOSED_FLAG='0' ORDER BY EDIT_TIME DESC";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetSNBYSNDESCRIPTION(string RelSn, String DESCRIPTION, OleExec DB)
        {
            string strSql = $@" SELECT * FROM R_REPAIR_TRANSFER D WHERE D.SN='{RelSn}' AND D.CLOSED_FLAG='0' AND DESCRIPTION = '{DESCRIPTION}' ORDER BY EDIT_TIME DESC";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public int InsertRepairTransfer(object InsertSql, OleExec DB, DB_TYPE_ENUM DBType)
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
        public List<R_REPAIR_TRANSFER> GetInsertSelect(string SNObj, OleExec DB)
        {
            string DESCRIPTION = "is sub_unit;main_unit:" + SNObj;
            string strSql = $@" SELECT SN,CREATE_TIME,WORKORDERNO,
    STATION_NAME,'0','1',IN_SEND_EMP,IN_SEND_EMP,SYSDATE
    FROM R_REPAIR_TRANSFER A 
    WHERE A.DESCRIPTION  = '{DESCRIPTION}' AND A.CLOSED_FLAG='0'      
    AND NOT EXISTS(      
    SELECT * FROM R_REPAIR_TRANSFER B  WHERE A.DESCRIPTION=B.DESCRIPTION AND B.CLOSED_FLAG='1' AND A.CREATE_TIME=B.CREATE_TIME)      
    AND NOT EXISTS(SELECT * FROM R_REPAIR_MAIN C WHERE A.SN=C.SN AND A.CREATE_TIME=C.CREATE_TIME) ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetInsertSelect1(string SNObj, OleExec DB)
        {
            string DESCRIPTION = "is sub_unit;main_unit:" + SNObj;
            string strSql = $@" SELECT SN ,CREATE_TIME,DESCRIPTION,DESCRIPTION,'SMT TOP',CREATE_TIME,'0',IN_SEND_EMP,SYSDATE      
    FROM R_REPAIR_TRANSFER  A      
    WHERE A.DESCRIPTION = '{DESCRIPTION}' AND A.CLOSED_FLAG='0'      
    AND NOT EXISTS(      
    SELECT * FROM R_REPAIR_TRANSFER B WHERE A.DESCRIPTION=B.DESCRIPTION AND B.CLOSED_FLAG='1' AND A.CREATE_TIME=B.CREATE_TIME)      
    AND NOT EXISTS(SELECT * FROM R_REPAIR_FAILCODE C WHERE A.SN=C.SN AND A.CREATE_TIME=C.CREATE_TIME) ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetInsertSelect2(string SNObj, OleExec DB)
        {
            string DESCRIPTION = "is sub_unit;main_unit:" + SNObj;
            string strSql = $@" SELECT SN,CREATE_TIME,'A21',CREATE_TIME + NUMTODSINTERVAL(1,'second'),SYSDATE,      
    'SMT TOP','','BBF','0','1',IN_SEND_EMP,SYSDATE FROM R_REPAIR_TRANSFER  A      
    WHERE A.DESCRIPTION = '{DESCRIPTION}' AND A.CLOSED_FLAG = '0'      
    AND NOT EXISTS(      
    SELECT * FROM R_REPAIR_TRANSFER B WHERE A.DESCRIPTION=B.DESCRIPTION AND B.CLOSED_FLAG='1' AND A.CREATE_TIME=B.CREATE_TIME)      
    AND NOT EXISTS(SELECT * FROM R_REPAIR_ACTION C WHERE A.SN=C.SN AND A.CREATE_TIME=C.FAIL_TIME) ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public List<R_REPAIR_TRANSFER> GetInsertSelect3(string SNObj, OleExec DB)
        {
            string DESCRIPTION = "is sub_unit;main_unit:" + SNObj;
            string strSql = $@" SELECT B.SN,'0','@reruntesstatus',B.WORKORDERNO,'emp',SYSDATE FROM R_SN_KP A,R_SN B WHERE A.VALUE IN (      
    SELECT SN FROM R_REPAIR_TRANSFER A 
    WHERE A.DESCRIPTION='is sub_unit;main_unit:+@ssn' AND A.CLOSED_FLAG='0'      
  AND NOT EXISTS(      
    SELECT * FROM R_REPAIR_TRANSFER B  WHERE A.DESCRIPTION=B.DESCRIPTION AND A.EDIT_TIME<B.EDIT_TIME AND B.CLOSED_FLAG='1' AND A.CREATE_TIME=B.CREATE_TIME)      
    ) AND A.SN=B.SN ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return listSn;
            }
            return listSn;
        }
        public DataTable GetSNBYSNEMP(string RelSn, OleExec DB)
        {
            string strSql = $@" SELECT           
 CASE WHEN CLOSED_FLAG='0' THEN LTRIM(RTRIM(IN_RECEIVE_EMP))          
  WHEN CLOSED_FLAG='1' THEN LTRIM(RTRIM(OUT_SEND_EMP))          
  ELSE 'NULL'          
  END AS RE_NAME          
FROM R_REPAIR_TRANSFER WHERE SN='{RelSn}'  AND EDIT_TIME =(SELECT MAX(EDIT_TIME) FROM R_REPAIR_TRANSFER WHERE SN='{RelSn}') ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            //List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            //if (res.Rows.Count > 0)
            //{
            //    foreach (DataRow item in res.Rows)
            //    {
            //        Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
            //        ret.loadData(item);
            //        listSn.Add(ret.GetDataObject());
            //    }
            //}
            return res;
        }    
       
        public int ReplaceSn(OleExec DB, DB_TYPE_ENUM DBType, string oldSn, string newSn)
        {
            int result = 0;
            string sql = string.Empty;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"update r_repair_transfer set sn='{newSn}' where sn='{oldSn}'";
                result = DB.ExecSqlNoReturn(sql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
    }
    public class Row_R_REPAIR_TRANSFER : DataObjectBase
    {
        public Row_R_REPAIR_TRANSFER(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_TRANSFER GetDataObject()
        {
            R_REPAIR_TRANSFER DataObject = new R_REPAIR_TRANSFER();
            DataObject.ID = this.ID;
            DataObject.REPAIR_MAIN_ID = this.REPAIR_MAIN_ID;
            DataObject.SN = this.SN;
            DataObject.LINE_NAME = this.LINE_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.IN_TIME = this.IN_TIME;
            DataObject.IN_SEND_EMP = this.IN_SEND_EMP;
            DataObject.IN_RECEIVE_EMP = this.IN_RECEIVE_EMP;
            DataObject.OUT_TIME = this.OUT_TIME;
            DataObject.OUT_SEND_EMP = this.OUT_SEND_EMP;
            DataObject.OUT_RECEIVE_EMP = this.OUT_RECEIVE_EMP;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.DEPARTMENT = this.DEPARTMENT;
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
        public string LINE_NAME
        {
            get

            {
                return (string)this["LINE_NAME"];
            }
            set
            {
                this["LINE_NAME"] = value;
            }
        }
        public string STATION_NAME
        {
            get

            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
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
        public DateTime? IN_TIME
        {
            get

            {
                return (DateTime?)this["IN_TIME"];
            }
            set
            {
                this["IN_TIME"] = value;
            }
        }
        public string IN_SEND_EMP
        {
            get

            {
                return (string)this["IN_SEND_EMP"];
            }
            set
            {
                this["IN_SEND_EMP"] = value;
            }
        }
        public string IN_RECEIVE_EMP
        {
            get

            {
                return (string)this["IN_RECEIVE_EMP"];
            }
            set
            {
                this["IN_RECEIVE_EMP"] = value;
            }
        }
        public DateTime? OUT_TIME
        {
            get

            {
                return (DateTime?)this["OUT_TIME"];
            }
            set
            {
                this["OUT_TIME"] = value;
            }
        }
        public string OUT_SEND_EMP
        {
            get

            {
                return (string)this["OUT_SEND_EMP"];
            }
            set
            {
                this["OUT_SEND_EMP"] = value;
            }
        }
        public string OUT_RECEIVE_EMP
        {
            get

            {
                return (string)this["OUT_RECEIVE_EMP"];
            }
            set
            {
                this["OUT_RECEIVE_EMP"] = value;
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
        public string CLOSED_FLAG
        {
            get

            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
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

        public string DEPARTMENT
        {
            get

            {
                return (string)this["DEPARTMENT"];
            }
            set
            {
                this["DEPARTMENT"] = value;
            }
        }
    }
    public class R_REPAIR_TRANSFER
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID{get;set;}
        public string REPAIR_MAIN_ID{get;set;}
        public string SN{get;set;}
        public string LINE_NAME{get;set;}
        public string STATION_NAME{get;set;}
        public string SKUNO{get;set;}
        public string WORKORDERNO{get;set;}
        public DateTime? IN_TIME{get;set;}
        public string IN_SEND_EMP{get;set;}
        public string IN_RECEIVE_EMP{get;set;}
        public DateTime? OUT_TIME{get;set;}
        public string OUT_SEND_EMP{get;set;}
        public string OUT_RECEIVE_EMP{get;set;}
        public string DESCRIPTION{get;set;}
        public string CLOSED_FLAG{get;set;}
        public DateTime? CREATE_TIME{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public string DEPARTMENT { get; set; }
    }
}