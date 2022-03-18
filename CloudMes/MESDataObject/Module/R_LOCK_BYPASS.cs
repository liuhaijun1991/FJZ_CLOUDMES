using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;

namespace MESDataObject.Module
{
    public class T_R_LOCK_BYPASS : DataObjectTable
    {
        public T_R_LOCK_BYPASS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LOCK_BYPASS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LOCK_BYPASS);
            TableName = "R_LOCK_BYPASS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_LOCK_BYPASS> GetPassList(string type ,string ID,  string SN, string WO, string STATUS, OleExec DB)
        {
            List<R_LOCK_BYPASS> Seq = new List<R_LOCK_BYPASS>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_SEQNO");
            Row_R_LOCK_BYPASS SeqRow = (Row_R_LOCK_BYPASS)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" SELECT * FROM R_LOCK_BYPASS RB WHERE  1=1  ";
                if (ID != "")
                {
                    sql += $@" and RB.ID='{ID}' ";
                }

                if (SN != "")
                {
                    sql += $@" and RB.VALUE1='{SN}' ";
                }

                if (WO != "")
                {
                    sql += $@" and RB.VALUE1='{WO}' ";
                }
                if (STATUS != "" && STATUS !="ALL")
                {
                    sql += $@" and RB.BYPASS_STATUS='{STATUS}'";
                }
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SeqRow.loadData(dr);
                    Seq.Add(SeqRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return Seq;
        }
        public bool IsPass( string type , string SN, string WORKORDERNO, OleExec DB)
        {
            string sql = string.Empty;
            bool isPass = true;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"  select * from R_LOCK_BYPASS where 1=1 and TYPE = '{type}' and BYPASS_STATUS='1'  ";
      
                if (SN != "")
                {
                    sql += $@" and VALUE1='{SN}' ";
                }
                if (WORKORDERNO != "")
                {
                    sql += $@" and VALUE1='{WORKORDERNO}' ";
                }
                if (DB.ExecSelect(sql, null).Tables[0].Rows.Count == 0)
                {
                    isPass = false;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return isPass;
        }
    }
    public class Row_R_LOCK_BYPASS : DataObjectBase
    {
        public Row_R_LOCK_BYPASS(DataObjectInfo info) : base(info)
        {

        }
        public R_LOCK_BYPASS GetDataObject()
        {
            R_LOCK_BYPASS DataObject = new R_LOCK_BYPASS();
            DataObject.ID = this.ID;
            DataObject.TYPE = this.TYPE;
            DataObject.KEY1 = this.KEY1;
            DataObject.VALUE1 = this.VALUE1;
            DataObject.KEY2 = this.KEY2;
            DataObject.VALUE2 = this.VALUE2;
            DataObject.KEY3 = this.KEY3;
            DataObject.VALUE3 = this.VALUE3;
            DataObject.KEY4 = this.KEY4;
            DataObject.VALUE4 = this.VALUE4;
            DataObject.REASON = this.REASON;
            DataObject.BYPASS_EMP = this.BYPASS_EMP;
            DataObject.BYPASS_TIME = this.BYPASS_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.BYPASS_STATUS = this.BYPASS_STATUS;
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
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
        public string KEY1
        {
            get
            {
                return (string)this["KEY1"];
            }
            set
            {
                this["KEY1"] = value;
            }
        }
        public string VALUE1
        {
            get
            {
                return (string)this["VALUE1"];
            }
            set
            {
                this["VALUE1"] = value;
            }
        }
        public string KEY2
        {
            get
            {
                return (string)this["KEY2"];
            }
            set
            {
                this["KEY2"] = value;
            }
        }
        public string VALUE2
        {
            get
            {
                return (string)this["VALUE2"];
            }
            set
            {
                this["VALUE2"] = value;
            }
        }
        public string KEY3
        {
            get
            {
                return (string)this["KEY3"];
            }
            set
            {
                this["KEY3"] = value;
            }
        }
        public string VALUE3
        {
            get
            {
                return (string)this["VALUE3"];
            }
            set
            {
                this["VALUE3"] = value;
            }
        }
        public string KEY4
        {
            get
            {
                return (string)this["KEY4"];
            }
            set
            {
                this["KEY4"] = value;
            }
        }
        public string VALUE4
        {
            get
            {
                return (string)this["VALUE4"];
            }
            set
            {
                this["VALUE4"] = value;
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
        public string BYPASS_EMP
        {
            get
            {
                return (string)this["BYPASS_EMP"];
            }
            set
            {
                this["BYPASS_EMP"] = value;
            }
        }
        public DateTime? BYPASS_TIME
        {
            get
            {
                return (DateTime?)this["BYPASS_TIME"];
            }
            set
            {
                this["BYPASS_TIME"] = value;
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
        public double? BYPASS_STATUS
        {
            get
            {
                return (double?)this["BYPASS_STATUS"];
            }
            set
            {
                this["BYPASS_STATUS"] = value;
            }
        }
    }
    public class R_LOCK_BYPASS
    {
        public string ID { get; set; }
        public string TYPE { get; set; }
        public string KEY1 { get; set; }
        public string VALUE1 { get; set; }
        public string KEY2 { get; set; }
        public string VALUE2 { get; set; }
        public string KEY3 { get; set; }
        public string VALUE3 { get; set; }
        public string KEY4 { get; set; }
        public string VALUE4 { get; set; }
        public string REASON { get; set; }
        public string BYPASS_EMP { get; set; }
        public DateTime? BYPASS_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public double? BYPASS_STATUS { get; set; }
    }
}