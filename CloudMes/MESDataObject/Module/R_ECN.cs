using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class T_R_ECN : DataObjectTable
    {
        public T_R_ECN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ECN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ECN);
            TableName = "R_ECN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 檢查部門(Columns)是否簽核
        /// </summary>
        /// <param name="Columns">部門,入PE,R_SING中的PE欄位</param>
        /// <param name="LOT_NO">批次號</param>
        /// <param name="TYPE">類型,是ECN還是PLAN_lock</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckIsSign(string Columns, string LOT_NO, string TYPE, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_SIGN WHERE LOT_NO='{LOT_NO}' and TYPE='{TYPE}' and  length({Columns})>0  and  ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }


        /// <summary>
        /// 檢查工單是否存在ECN
        /// add by hgb 2019.06.04
        /// </summary>
        /// <param name="CONTROL_TYPE">WO</param>
        /// <param name="CONTROL_VALUE">工單號</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckEcnExist(string CONTROL_TYPE, string CONTROL_VALUE, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_ECN where CONTROL_TYPE='{CONTROL_TYPE}' and CONTROL_VALUE ='{CONTROL_VALUE}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// 傳入ECN類型，工單號，返回ECN信息
        /// add by hgb 2019.06.04
        /// </summary>
        /// <param name="CONTROL_TYPE">ECN,傳入WO</param>
        /// <param name="CONTROL_VALUE">ECN,傳入工單號</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_ECN GetEcn(string CONTROL_TYPE, string CONTROL_VALUE, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {                
                strsql = $@"SELECT ID FROM R_ECN where CONTROL_TYPE='{CONTROL_TYPE}' and CONTROL_VALUE ='{CONTROL_VALUE}'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "工單:" + CONTROL_VALUE + "ECN" });
                    throw new MESReturnMessage(errMsg);
                }
                Row_R_ECN R = (Row_R_ECN)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
    }
    public class Row_R_ECN : DataObjectBase
    {

        public Row_R_ECN(DataObjectInfo info) : base(info)
        {

        }
         

        public R_ECN GetDataObject()
        {
            R_ECN DataObject = new R_ECN();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.MODEL = this.MODEL;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.ECA = this.ECA;
            DataObject.ECN = this.ECN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.EFFECTIVE_DATE = this.EFFECTIVE_DATE;
            DataObject.IMPORT_MODE = this.IMPORT_MODE;
            DataObject.CONTROL_TYPE = this.CONTROL_TYPE;
            DataObject.CONTROL_VALUE = this.CONTROL_VALUE;
            DataObject.CONTROL_STATION = this.CONTROL_STATION;
            DataObject.LOCK_STATION = this.LOCK_STATION;
            DataObject.FIRST_SN = this.FIRST_SN;
            DataObject.CUSTINPUT_DATE = this.CUSTINPUT_DATE;
            DataObject.MEMO = this.MEMO;
            DataObject.REMARK = this.REMARK;
            DataObject.OWNER_PE = this.OWNER_PE;
            DataObject.OWNER_PQE = this.OWNER_PQE;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.CREATED_DATE = this.CREATED_DATE;
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
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
            }
        }
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
        }
        public string ECA
        {
            get
            {
                return (string)this["ECA"];
            }
            set
            {
                this["ECA"] = value;
            }
        }
        public string ECN
        {
            get
            {
                return (string)this["ECN"];
            }
            set
            {
                this["ECN"] = value;
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
        public DateTime? EFFECTIVE_DATE
        {
            get
            {
                return (DateTime?)this["EFFECTIVE_DATE"];
            }
            set
            {
                this["EFFECTIVE_DATE"] = value;
            }
        }
        public string IMPORT_MODE
        {
            get
            {
                return (string)this["IMPORT_MODE"];
            }
            set
            {
                this["IMPORT_MODE"] = value;
            }
        }
        public string CONTROL_TYPE
        {
            get
            {
                return (string)this["CONTROL_TYPE"];
            }
            set
            {
                this["CONTROL_TYPE"] = value;
            }
        }
        public string CONTROL_VALUE
        {
            get
            {
                return (string)this["CONTROL_VALUE"];
            }
            set
            {
                this["CONTROL_VALUE"] = value;
            }
        }
        public string CONTROL_STATION
        {
            get
            {
                return (string)this["CONTROL_STATION"];
            }
            set
            {
                this["CONTROL_STATION"] = value;
            }
        }
        public string LOCK_STATION
        {
            get
            {
                return (string)this["LOCK_STATION"];
            }
            set
            {
                this["LOCK_STATION"] = value;
            }
        }
        public string FIRST_SN
        {
            get
            {
                return (string)this["FIRST_SN"];
            }
            set
            {
                this["FIRST_SN"] = value;
            }
        }
        public DateTime? CUSTINPUT_DATE
        {
            get
            {
                return (DateTime?)this["CUSTINPUT_DATE"];
            }
            set
            {
                this["CUSTINPUT_DATE"] = value;
            }
        }
        public string MEMO
        {
            get
            {
                return (string)this["MEMO"];
            }
            set
            {
                this["MEMO"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public string OWNER_PE
        {
            get
            {
                return (string)this["OWNER_PE"];
            }
            set
            {
                this["OWNER_PE"] = value;
            }
        }
        public string OWNER_PQE
        {
            get
            {
                return (string)this["OWNER_PQE"];
            }
            set
            {
                this["OWNER_PQE"] = value;
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
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public DateTime? CREATED_DATE
        {
            get
            {
                return (DateTime?)this["CREATED_DATE"];
            }
            set
            {
                this["CREATED_DATE"] = value;
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
    public class R_ECN
    {
        public string ID;
        public string BU;
        public string MODEL;
        public string LOT_NO;
        public string ECA;
        public string ECN;
        public string SKUNO;
        public DateTime? EFFECTIVE_DATE;
        public string IMPORT_MODE;
        public string CONTROL_TYPE;
        public string CONTROL_VALUE;
        public string CONTROL_STATION;
        public string LOCK_STATION;
        public string FIRST_SN;
        public DateTime? CUSTINPUT_DATE;
        public string MEMO;
        public string REMARK;
        public string OWNER_PE;
        public string OWNER_PQE;
        public string DATA1;
        public string DATA2;
        public DateTime? CREATED_DATE;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}