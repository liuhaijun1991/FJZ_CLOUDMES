using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_FTX_CPU_OCR : DataObjectTable
    {
        public T_R_FTX_CPU_OCR(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FTX_CPU_OCR(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FTX_CPU_OCR);
            TableName = "R_FTX_CPU_OCR".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool ChkCPUSN(string SN, OleExec SFCDB)
        {
            List<R_FTX_CPU_OCR> R_FTX_CPU_OCR = new List<R_FTX_CPU_OCR>();
            R_FTX_CPU_OCR = SFCDB.ORM.Queryable<R_FTX_CPU_OCR>().Where(t => t.SN == SN).ToList();
            if (R_FTX_CPU_OCR.Count > 0)
            {
                return true;
            }

            return false;
        }

        public string GetBroker( string SN, OleExec SFCDB)
        {
            string brokerlist;
            brokerlist = SFCDB.ORM.Queryable<R_FTX_CPU_OCR>().Where(t => t.SN == SN).Select(a=>a.PN) .ToString();
            return brokerlist;
        }
        public bool ChkCPUSNPN(string PN, string SN, OleExec SFCDB)
        {
            List<R_FTX_CPU_OCR> R_FTX_CPU_OCR = new List<R_FTX_CPU_OCR>();
            R_FTX_CPU_OCR = SFCDB.ORM.Queryable<R_FTX_CPU_OCR>().Where(t => t.SN == SN && t.PN == PN).ToList();
            if (R_FTX_CPU_OCR.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
    public class Row_R_FTX_CPU_OCR : DataObjectBase
    {
        public Row_R_FTX_CPU_OCR(DataObjectInfo info) : base(info)
        {

        }
        public R_FTX_CPU_OCR GetDataObject()
        {
            R_FTX_CPU_OCR DataObject = new R_FTX_CPU_OCR();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.PID = this.PID;
            DataObject.PN = this.PN;
            DataObject.CODE = this.CODE;
            DataObject.COO = this.COO;
            DataObject.FPO = this.FPO;
            DataObject.SNLEN = this.SNLEN;
            DataObject.SNPREFIX = this.SNPREFIX;
            DataObject.PASS = this.PASS;
            DataObject.RETURNMSG = this.RETURNMSG;
            DataObject.MSGCODE = this.MSGCODE;
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
        public string PID
        {
            get
            {
                return (string)this["PID"];
            }
            set
            {
                this["PID"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public string CODE
        {
            get
            {
                return (string)this["CODE"];
            }
            set
            {
                this["CODE"] = value;
            }
        }
        public string COO
        {
            get
            {
                return (string)this["COO"];
            }
            set
            {
                this["COO"] = value;
            }
        }
        public string FPO
        {
            get
            {
                return (string)this["FPO"];
            }
            set
            {
                this["FPO"] = value;
            }
        }
        public double? SNLEN
        {
            get
            {
                return (double?)this["SNLEN"];
            }
            set
            {
                this["SNLEN"] = value;
            }
        }
        public string SNPREFIX
        {
            get
            {
                return (string)this["SNPREFIX"];
            }
            set
            {
                this["SNPREFIX"] = value;
            }
        }
        public string PASS
        {
            get
            {
                return (string)this["PASS"];
            }
            set
            {
                this["PASS"] = value;
            }
        }
        public string RETURNMSG
        {
            get
            {
                return (string)this["RETURNMSG"];
            }
            set
            {
                this["RETURNMSG"] = value;
            }
        }
        public double? MSGCODE
        {
            get
            {
                return (double?)this["MSGCODE"];
            }
            set
            {
                this["MSGCODE"] = value;
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
    public class R_FTX_CPU_OCR
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string PID { get; set; }
        public string PN { get; set; }
        public string CODE { get; set; }
        public string COO { get; set; }
        public string FPO { get; set; }
        public double? SNLEN { get; set; }
        public string SNPREFIX { get; set; }
        public string PASS { get; set; }
        public string RETURNMSG { get; set; }
        public double? MSGCODE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}
