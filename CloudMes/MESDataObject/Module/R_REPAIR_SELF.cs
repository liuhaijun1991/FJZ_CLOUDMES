using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_SELF : DataObjectTable
    {
        public T_R_REPAIR_SELF(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_SELF(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_SELF);
            TableName = "R_REPAIR_SELF".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_REPAIR_SELF> GetSNBySN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_SELF>().Where(t => t.SN == SN).ToList();
        }
        public List<R_REPAIR_SELF> GetBySNStation(string SN, string Station, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_SELF>().Where(t => t.SN == SN && t.FIND_STATION == Station).ToList();
        }
        public int UpdateSNState(R_REPAIR_SELF REPAIRSELF, OleExec DB)
        {
            return DB.ORM.Updateable<R_REPAIR_SELF>(REPAIRSELF).Where(t => t.ID == REPAIRSELF.ID).ExecuteCommand();
        }
        public List<R_REPAIR_SELF> GetBySNCheckIN(string CheckOUT, string SN, string SNRight8, string SNPrefix, OleExec DB)
        {
            //string ORSN = SNPrefix + SNRight8;
            //return DB.ORM.Queryable<R_REPAIR_SELF, R_SN_KP>((p1, p2) => p1.SN == p2.SN && p1.OUT_FLAG == CheckOUT && (p1.SN == SN || p1.SN == ORSN) || (p2.VALUE == SN || p2.VALUE == ORSN))/*.Where((p1, p2) => p1.OUT_FLAG == CheckOUT && (p1.SN == SN || p1.SN == ORSN) || (p2.VALUE == SN || p2.VALUE == ORSN))*/.ToList();
            string ORSN = SNPrefix + SNRight8;
            List<string> ListRSNKP = DB.ORM.Queryable<R_SN_KP>().Where(p1 => p1.VALUE == SN || p1.VALUE == ORSN).Select(t => t.SN).ToList();
            return DB.ORM.Queryable<R_REPAIR_SELF>().Where(p1 => p1.OUT_FLAG == CheckOUT && ((p1.SN == SN || p1.SN == ORSN) || ListRSNKP.Contains(p1.SN))).ToList();
        }
        public List<R_REPAIR_SELF> GetBySelfSN(string CheckIN, string CheckOUT, string SN, OleExec DB)
        {
            string ORSN = "B9C" + SN.Substring(SN.Length - 8, 8);
            List<string> ListRSNKP = DB.ORM.Queryable<R_SN_KP>().Where(p1 => p1.VALUE == SN || p1.VALUE == ORSN).Select(t => t.SN).ToList();
            return DB.ORM.Queryable<R_REPAIR_SELF>().Where(p1 => p1.IN_FLAG == CheckIN && p1.OUT_FLAG == CheckOUT && ListRSNKP.Contains(p1.SN)).ToList();
        }
        public List<R_REPAIR_SELF> GetBySNCheck(string CheckOUT, string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_SELF>().Where(t => t.SN == SN && t.OUT_FLAG == CheckOUT).ToList();
        }
        public List<R_REPAIR_SELF> GetByFailCode(string FailCode, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_SELF>().Where(t => t.FAIL_CODE == FailCode).ToList();
        }
        public int InRepairSelf(R_REPAIR_SELF RepairSelf, OleExec DB)
        {
            return DB.ORM.Insertable<R_REPAIR_SELF>(RepairSelf).ExecuteCommand();
        }
        public List<R_REPAIR_SELF> GetSNBySNCheck(string SN, string IN, string OUT, OleExec DB)
        {
            string ORSN = "B9C" + SN.Substring(SN.Length - 8, 8);
            return DB.ORM.Queryable<R_REPAIR_SELF>().Where(t => (t.SN == SN || t.SN == ORSN) && t.IN_FLAG == IN && t.OUT_FLAG == OUT).ToList();
        }
        public int UpdateRepairSN(R_REPAIR_SELF RREPAIRSELF, string SN, string IN, string OUT, OleExec DB)
        {
            string ORSN = "B9C" + SN.Substring(SN.Length - 8, 8);
            return DB.ORM.Updateable<R_REPAIR_SELF>(RREPAIRSELF).Where(t => (t.SN == SN || t.SN == ORSN) && t.IN_FLAG == IN && t.OUT_FLAG == OUT).ExecuteCommand();
        }
        public List<R_REPAIR_SELF> GetSNBySNCheckExists(string SN, string IN, string OUT, OleExec DB)
        {
            string sql = $@"SELECT * FROM R_REPAIR_SELF A ,  
     (SELECT SN,MAX(EDIT_TIME) EDIT_TIME FROM R_REPAIR_SELF WHERE SN='{SN}' GROUP BY SN) B   
     WHERE A.SN=B.SN AND A.EDIT_TIME=B.EDIT_TIME AND A.IN_FLAG='{IN}' AND A.OUT_FLAG='{OUT}' ";
            DataTable dtRRepairSelf = new DataTable();
            dtRRepairSelf = DB.ExecSelect(sql).Tables[0];
            List<R_REPAIR_SELF> RRepairSelfList = new List<R_REPAIR_SELF>();
            Row_R_REPAIR_SELF RRepairSelfRow;
            foreach (DataRow row in dtRRepairSelf.Rows)
            {
                RRepairSelfRow = (Row_R_REPAIR_SELF)this.NewRow();
                RRepairSelfRow.loadData(row);
                RRepairSelfList.Add(RRepairSelfRow.GetDataObject());
            }
            return RRepairSelfList;
        }
        /// <summary>
        /// WZW checkin SN Check
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="OutDictionary"></param>
        /// <param name="DB"></param>
        public void LH_NSDI_NewEFox_AutoRepairScoreboard_SP(string SN, ref Dictionary<string, string> OutDictionary, OleExec DB)
        {
            //string IsOrRWSSN = "";
            //Dictionary<string, string> OutDictionary = new Dictionary<string, string>();
            string CheckINEmp = "";
            T_R_REPAIR_SELF TRRepairSelf = new T_R_REPAIR_SELF(DB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_SELF> ListRRepairSelf = TRRepairSelf.GetSNBySNCheckExists(SN, "1", "0", DB);
            if (ListRRepairSelf.Count > 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219101259", new string[] { SN }));
            }
            T_R_REPAIR_TRANSFER TRRepairTransfer = new T_R_REPAIR_TRANSFER(DB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_TRANSFER> ListRRepairTransfer = TRRepairTransfer.GetBySNCheckINType("0", SN, DB);
            if (ListRRepairTransfer != null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219101419", new string[] { SN }));
            }
            List<R_REPAIR_TRANSFER> ListSNCheckRRepairTransfer = TRRepairTransfer.GetSNOrderEnd(SN, DB);
            if (ListSNCheckRRepairTransfer.Count > 0)
            {
                CheckINEmp = ListSNCheckRRepairTransfer[0].IN_RECEIVE_EMP;
            }
            T_C_REPAIR_USER_CONTROL TCRepairUserControl = new T_C_REPAIR_USER_CONTROL(DB, DB_TYPE_ENUM.Oracle);
            if (CheckINEmp.Length > 0)
            {
                List<C_REPAIR_USER_CONTROL> ListCRepairUserControl = TCRepairUserControl.GetEMPByExists(CheckINEmp, DB);
                if (ListCRepairUserControl.Count > 0)
                {
                    List<R_REPAIR_TRANSFER> ListSNCheckCountRRepairTransfer = TRRepairTransfer.GetBySNCheckINEMP(CheckINEmp, DB);
                    List<C_REPAIR_USER_CONTROL> ListCRepairUserControlEMPByExistsANDQTYTYPE = TCRepairUserControl.GetEMPByExistsANDQTYTYPE(CheckINEmp, ListSNCheckCountRRepairTransfer.Count, "ACCEPTREPAIRID", DB);
                    if (ListCRepairUserControlEMPByExistsANDQTYTYPE.Count > 0)
                    {
                        //IsOrRWSSN = "0";
                        OutDictionary["IsOrRWSSN"] = "0";
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219104452", new string[] { CheckINEmp }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219162708", new string[] { CheckINEmp }));
                }
            }
            else
            {
                List<C_REPAIR_USER_CONTROL> ListCRepairUserControl = TCRepairUserControl.GetByTYPE("LASTREPAIRID", DB);
                if (ListCRepairUserControl.Count <= 0)
                {
                    //IsOrRWSSN = "1";
                    OutDictionary["IsOrRWSSN"] = "1";
                    List<C_REPAIR_USER_CONTROL> ListCRepairUserControlValidContorid = TCRepairUserControl.GetBValidControid("1", DB);
                    CheckINEmp = ListCRepairUserControlValidContorid[0].CONTROL_EMP_NO;
                }
                else
                {
                    List<C_REPAIR_USER_CONTROL> ListCRepairUserControlWIP = TCRepairUserControl.GetBySNCheckIN(ListCRepairUserControl[0].CONTROL_ID, DB);
                    if (ListCRepairUserControlWIP[0].CONTROL_ID.Length > 0)
                    {
                        //IsOrRWSSN = "1";
                        OutDictionary["IsOrRWSSN"] = "1";
                        CheckINEmp = ListCRepairUserControlWIP[0].CONTROL_ID;
                    }
                    else
                    {
                        List<C_REPAIR_USER_CONTROL> ListCRepairUserControlWIPSmall = TCRepairUserControl.GetBySNCheckINSmall(ListCRepairUserControl[0].CONTROL_ID, DB);
                        if (ListCRepairUserControlWIPSmall[0].CONTROL_ID.Length > 0)
                        {
                            //IsOrRWSSN = "1";
                            OutDictionary["IsOrRWSSN"] = "1";
                            CheckINEmp = ListCRepairUserControlWIP[0].CONTROL_ID;
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181219162818", new string[] { }));
                        }
                    }
                }
            }
            OutDictionary["CheckINEmp"] = CheckINEmp;
            int CountSNCheckIN = 0;
            List<R_REPAIR_TRANSFER> ListRRepairTransferCountSNCheckin = TRRepairTransfer.GetBySNCountCheckIN(SN, DB);
            if (ListRRepairTransferCountSNCheckin != null)
            {
                CountSNCheckIN = ListRRepairTransferCountSNCheckin.Count;
            }
            T_R_REPAIR_ACTION TRRepairAction = new T_R_REPAIR_ACTION(DB, DB_TYPE_ENUM.Oracle);
            List<R_REPAIR_ACTION> ListRRepairActin = TRRepairAction.GetCountSN(SN, "E206", DB);
            if (ListRRepairActin.Count == 2)
            {
                List<C_REPAIR_USER_CONTROL> ListCRepairUserControlStrike = TCRepairUserControl.GetBySNCheckINOutStrike2(CheckINEmp, DB);
                OutDictionary["CONTROL_EMP_NO"] = ListCRepairUserControlStrike[0].CONTROL_EMP_NO;
                OutDictionary["CONTROL_ID"] = ListCRepairUserControlStrike[0].CONTROL_ID;
                //此sn疑似3-strike狀態,請送SE分析
            }
            else
            {
                List<C_REPAIR_USER_CONTROL> ListCRepairUserControlStrike = TCRepairUserControl.GetBySNCheckINOutStrike(CheckINEmp, DB);
                OutDictionary["CONTROL_EMP_NO"] = ListCRepairUserControlStrike[0].CONTROL_EMP_NO;
                OutDictionary["CONTROL_ID"] = ListCRepairUserControlStrike[0].CONTROL_ID;
                //此sn正常
            }
            OutDictionary["CONTROL_ID"] = OutDictionary["CONTROL_ID"] + 1;
        }
        public List<R_REPAIR_SELF> GetSNByExceptionSN(string CheckIN, string CheckOUT, string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_REPAIR_SELF>().Where(p1 => p1.IN_FLAG == CheckIN && p1.OUT_FLAG == CheckOUT && p1.SN == SN).ToList();
        }
    }
    public class Row_R_REPAIR_SELF : DataObjectBase
    {
        public Row_R_REPAIR_SELF(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_SELF GetDataObject()
        {
            R_REPAIR_SELF DataObject = new R_REPAIR_SELF();
            DataObject.REPAIR_LOCATION = this.REPAIR_LOCATION;
            DataObject.REPAIR_MARK = this.REPAIR_MARK;
            DataObject.REASON = this.REASON;
            DataObject.REASON_OUT = this.REASON_OUT;
            DataObject.DEPARTMENT = this.DEPARTMENT;
            DataObject.SHIPPED_FLAG = this.SHIPPED_FLAG;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.RETURN_STATION = this.RETURN_STATION;
            DataObject.IN_FLAG = this.IN_FLAG;
            DataObject.IN_TIME = this.IN_TIME;
            DataObject.OUT_FLAG = this.OUT_FLAG;
            DataObject.OUT_TIME = this.OUT_TIME;
            DataObject.FIND_STATION = this.FIND_STATION;
            DataObject.FIND_LOCATION = this.FIND_LOCATION;
            DataObject.FAIL_CODE = this.FAIL_CODE;
            DataObject.FAIL_LOCATION = this.FAIL_LOCATION;
            DataObject.FAIL_MARK = this.FAIL_MARK;
            DataObject.REPAIR_CODE = this.REPAIR_CODE;
            return DataObject;
        }
        public string REPAIR_LOCATION
        {
            get
            {
                return (string)this["REPAIR_LOCATION"];
            }
            set
            {
                this["REPAIR_LOCATION"] = value;
            }
        }
        public string REPAIR_MARK
        {
            get
            {
                return (string)this["REPAIR_MARK"];
            }
            set
            {
                this["REPAIR_MARK"] = value;
            }
        }
        public string REASON
        {
            get
            {
                return (string)this["REASON"];
            }
            set
            {
                this["REASON"] = value;
            }
        }
        public string REASON_OUT
        {
            get
            {
                return (string)this["REASON_OUT"];
            }
            set
            {
                this["REASON_OUT"] = value;
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
        public string SHIPPED_FLAG
        {
            get
            {
                return (string)this["SHIPPED_FLAG"];
            }
            set
            {
                this["SHIPPED_FLAG"] = value;
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
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
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
        public string RETURN_STATION
        {
            get
            {
                return (string)this["RETURN_STATION"];
            }
            set
            {
                this["RETURN_STATION"] = value;
            }
        }
        public string IN_FLAG
        {
            get
            {
                return (string)this["IN_FLAG"];
            }
            set
            {
                this["IN_FLAG"] = value;
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
        public string OUT_FLAG
        {
            get
            {
                return (string)this["OUT_FLAG"];
            }
            set
            {
                this["OUT_FLAG"] = value;
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
        public string FIND_STATION
        {
            get
            {
                return (string)this["FIND_STATION"];
            }
            set
            {
                this["FIND_STATION"] = value;
            }
        }
        public string FIND_LOCATION
        {
            get
            {
                return (string)this["FIND_LOCATION"];
            }
            set
            {
                this["FIND_LOCATION"] = value;
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
        public string FAIL_MARK
        {
            get
            {
                return (string)this["FAIL_MARK"];
            }
            set
            {
                this["FAIL_MARK"] = value;
            }
        }
        public string REPAIR_CODE
        {
            get
            {
                return (string)this["REPAIR_CODE"];
            }
            set
            {
                this["REPAIR_CODE"] = value;
            }
        }
    }
    public class R_REPAIR_SELF
    {
        public string REPAIR_LOCATION { get; set; }
        public string REPAIR_MARK { get; set; }
        public string REASON { get; set; }
        public string REASON_OUT { get; set; }
        public string DEPARTMENT { get; set; }
        public string SHIPPED_FLAG { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string ID { get; set; }
        public string SN { get; set; }
        public string WO { get; set; }
        public string SKUNO { get; set; }
        public string CURRENT_STATION { get; set; }
        public string NEXT_STATION { get; set; }
        public string RETURN_STATION { get; set; }
        public string IN_FLAG { get; set; }
        public DateTime? IN_TIME { get; set; }
        public string OUT_FLAG { get; set; }
        public DateTime? OUT_TIME { get; set; }
        public string FIND_STATION { get; set; }
        public string FIND_LOCATION { get; set; }
        public string FAIL_CODE { get; set; }
        public string FAIL_LOCATION { get; set; }
        public string FAIL_MARK { get; set; }
        public string REPAIR_CODE { get; set; }
    }
}