using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SEQNO : DataObjectTable
    {
        public T_C_SEQNO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SEQNO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SEQNO);
            TableName = "C_SEQNO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_SEQNO> GetSeq(string SEQ_NAME, string SEQ_NO, OleExec DB)
        {
            List<C_SEQNO> Seq = new List<C_SEQNO>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_SEQNO");
            Row_C_SEQNO SeqRow = (Row_C_SEQNO)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"  select * from c_seqno where 1=1  ";
                if (SEQ_NAME != "")
                    sql += $@" and SEQ_NAME='{SEQ_NAME}' ";
                if (SEQ_NO != "")
                    sql += $@" and SEQ_NO='{SEQ_NO}' ";
                if (SEQ_NAME == "" && SEQ_NO == "")
                    sql += $@" and  rownum<21  order by EDIT_TIME ";
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

        public C_SEQNO GetSeqnoObj(string SeqName, OleExec DB)
        {

            DataTable Dt = new DataTable();
            string sql = $@"SELECT * FROM C_SEQNO WHERE seq_name ='{SeqName}'";
            Dt = DB.ExecSelect(sql).Tables[0];
            Row_C_SEQNO Row = (Row_C_SEQNO)NewRow();
            if (Dt.Rows.Count > 0)
            {
                Row.loadData(Dt.Rows[0]);
            }
            return Row.GetDataObject();

        }
        public bool UpdateSeqno(string SeqName, string Result, OleExec DB)
        {

            DataTable dt = new DataTable();
            string sql = $@"UPDATE C_SEQNO  SET SEQ_NO ='{Result}',USE_TIME = SYSDATE,EDIT_TIME = SYSDATE WHERE seq_name ='{SeqName}'";
            DB.ExecSQL(sql);
            return true;
        }

        
        public string GetLotno(string SeqName,OleExec DB)
        {
            string StrLot = "";
            string Result = "";
            string DateCode = "";
            //string WeekNo = "";
            double NextSeqno = 0;
            Int16 Digits = 0;
            double Minimum = 0;
            double Maxmum = 0;
            string Prefix = "";
            string Fixedstr = "";
            string SeqForm = "";
            string Reset = "";
            DateTime UseTime = new DateTime();

            T_C_SEQNO T_C_Seqno = new T_C_SEQNO(DB, DB_TYPE_ENUM.Oracle);

            C_SEQNO C_SEQNO = T_C_Seqno.GetSeqnoObj(SeqName, DB);

            NextSeqno = Int64.Parse(C_SEQNO.SEQ_NO.ToString());
            Digits = (Int16)C_SEQNO.DIGITS;
            Minimum = Int64.Parse(C_SEQNO.MINIMUM.ToString());
            Maxmum = Int64.Parse(C_SEQNO.MAXIMUM.ToString());
            Prefix = C_SEQNO.PREFIX;
            Fixedstr = C_SEQNO.FIXED;
            SeqForm = C_SEQNO.SEQ_FORM;
            Reset = C_SEQNO.RESET;
            UseTime = (DateTime)C_SEQNO.USE_TIME;

            Result = NextSeqno.ToString();//默认为取下一个值

            //edit by lc 20180330
            if (NextSeqno == Maxmum)
            {
                Result = Minimum.ToString();
            }
            if (!string.IsNullOrEmpty(SeqForm))
            {
                if ("YYYYMMDD".Equals(SeqForm) && "1".Equals(Reset) && GetFormatDate(DB, UseTime, "YYYYMMDD") != GetFormatSYSDate(DB, System.DateTime.Now, "YYYYMMDD"))
                {
                    Result = Minimum.ToString();
                }
                else if ("YYMMDD".Equals(SeqForm) && "1".Equals(Reset) && GetFormatDate(DB, UseTime, "YYMMDD") != GetFormatSYSDate(DB, System.DateTime.Now, "YYMMDD"))
                {
                    Result = Minimum.ToString();
                }
                else if ("YY-WK".Equals(SeqForm) && "1".Equals(Reset) && GetFormatDate(DB, UseTime, "YY-IW") != GetFormatSYSDate(DB, System.DateTime.Now, "YY-IW"))
                {
                    Result = Minimum.ToString();
                }
                else if ("WK".Equals(SeqForm) && "1".Equals(Reset) && GetFormatDate(DB, UseTime, "IW") != GetFormatSYSDate(DB, System.DateTime.Now, "IW"))
                {
                    Result = Minimum.ToString();
                }
                else if ("YY-MM".Equals(SeqForm) && "1".Equals(Reset))
                {
                    Result = Minimum.ToString();
                }
                //else if (NextSeqno == Maxmum)
                //{
                //    Result = Minimum.ToString();
                //}

                if ((SeqForm).IndexOf("YYYY") != -1)
                {
                    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "YYYY");
                }
                else if ((SeqForm).IndexOf("YY") != -1)
                {
                    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "YY");
                }
                else if ((SeqForm).IndexOf("Y") != -1)
                {
                    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "Y");
                }

                if ((SeqForm).IndexOf("MM") != -1)
                {
                    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "MM");
                }
                else if ((SeqForm).IndexOf("M") != -1)
                {
                    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "M");
                }

                if ((SeqForm).IndexOf("WK") != -1)
                {
                    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "IW");
                }

                if ((SeqForm).IndexOf("DD") != -1)
                {
                    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "DD");
                }


                //if ("YYYY".IndexOf(SeqForm) != -1)
                //{
                //    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "YYYY");
                //}
                //else if ("YY".IndexOf(SeqForm) != -1)
                //{
                //    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "YY");
                //}
                //else if ("Y".IndexOf(SeqForm) != -1)
                //{
                //    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "Y");
                //}

                //if ("MM".IndexOf(SeqForm) != -1)
                //{
                //    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "MM");
                //}
                //else if ("M".IndexOf(SeqForm) != -1)
                //{
                //    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "M");
                //}

                //if ("WK".IndexOf(SeqForm) != -1)
                //{
                //    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "IW");
                //}

                //if ("DD".IndexOf(SeqForm) != -1)
                //{
                //    DateCode = DateCode + GetFormatSYSDate(DB, System.DateTime.Now, "DD");
                //}
            }
            

            T_C_Seqno.UpdateSeqno(SeqName, (int.Parse(Result) + 1).ToString(), DB);

            StrLot = Prefix + DateCode + Fixedstr + Result.ToString().PadLeft(Digits, '0');

            return StrLot;
        }

        private string GetFormatSYSDate(OleExec sfcdb, DateTime Date, string StrFormat)
        {
            string SrtFormatDate = "";
            //string DateFormat = Date.ToString("yyyy-MM-dd HH:mm:ss");
            //string StrSql = $@"select TO_CHAR(TO_DATE('{DateFormat}','YYYY-MM-DD HH24:MI:ss' ), '{StrFormat}') as DATA from dual";
            string StrSql = $@"select TO_CHAR(SYSDATE, '{StrFormat}') as DATA from dual";
            System.Data.DataSet res = sfcdb.ExecSelect(StrSql);

            SrtFormatDate = res.Tables[0].Rows[0]["DATA"].ToString();
            return SrtFormatDate;
        }

        private string GetFormatDate(OleExec sfcdb, DateTime Date, string StrFormat)
        {
            string SrtFormatDate = "";
            string DateFormat = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string StrSql = $@"select TO_CHAR(TO_DATE('{DateFormat}','YYYY-MM-DD HH24:MI:ss' ), '{StrFormat}') as DATA from dual";
            System.Data.DataSet res = sfcdb.ExecSelect(StrSql);

            SrtFormatDate = res.Tables[0].Rows[0]["DATA"].ToString();
            return SrtFormatDate;
        }

        public C_SEQNO GetSEQNAME(string SeqName, string SeqNo, OleExec DB)
        {
            return DB.ORM.Queryable<C_SEQNO>().Where(t => t.SEQ_NAME == SeqName && t.SEQ_NO == SeqNo).ToList().FirstOrDefault();
        }
        public int InSeqName(C_SEQNO SeqName, OleExec DB)
        {
            return DB.ORM.Insertable<C_SEQNO>(SeqName).ExecuteCommand();
        }
        public int UpdateSeqName(C_SEQNO RepairSN, string SEQNAME, OleExec DB)
        {
            return DB.ORM.Updateable<C_SEQNO>(RepairSN).Where(t => t.SEQ_NAME == SEQNAME).ExecuteCommand();
        }
    }
    public class Row_C_SEQNO : DataObjectBase
    {
        public Row_C_SEQNO(DataObjectInfo info) : base(info)
        {

        }
        public C_SEQNO GetDataObject()
        {
            C_SEQNO DataObject = new C_SEQNO();
            DataObject.SEQ_NAME = this.SEQ_NAME;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.DIGITS = this.DIGITS;
            DataObject.BASE_CODE = this.BASE_CODE;
            DataObject.MINIMUM = this.MINIMUM;
            DataObject.MAXIMUM = this.MAXIMUM;
            DataObject.PREFIX = this.PREFIX;
            DataObject.FIXED = this.FIXED;
            DataObject.SEQ_FORM = this.SEQ_FORM;
            DataObject.RESET = this.RESET;
            DataObject.USE_TIME = this.USE_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string SEQ_NAME
        {
            get
            {
                return (string)this["SEQ_NAME"];
            }
            set
            {
                this["SEQ_NAME"] = value;
            }
        }
        public string SEQ_NO
        {
            get
            {
                return (string)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public double? DIGITS
        {
            get
            {
                return (double?)this["DIGITS"];
            }
            set
            {
                this["DIGITS"] = value;
            }
        }
        public string BASE_CODE
        {
            get
            {
                return (string)this["BASE_CODE"];
            }
            set
            {
                this["BASE_CODE"] = value;
            }
        }
        public string MINIMUM
        {
            get
            {
                return (string)this["MINIMUM"];
            }
            set
            {
                this["MINIMUM"] = value;
            }
        }
        public string MAXIMUM
        {
            get
            {
                return (string)this["MAXIMUM"];
            }
            set
            {
                this["MAXIMUM"] = value;
            }
        }
        public string PREFIX
        {
            get
            {
                return (string)this["PREFIX"];
            }
            set
            {
                this["PREFIX"] = value;
            }
        }
        public string FIXED
        {
            get
            {
                return (string)this["FIXED"];
            }
            set
            {
                this["FIXED"] = value;
            }
        }
        public string SEQ_FORM
        {
            get
            {
                return (string)this["SEQ_FORM"];
            }
            set
            {
                this["SEQ_FORM"] = value;
            }
        }
        public string RESET
        {
            get
            {
                return (string)this["RESET"];
            }
            set
            {
                this["RESET"] = value;
            }
        }
        public DateTime? USE_TIME
        {
            get
            {
                return (DateTime?)this["USE_TIME"];
            }
            set
            {
                this["USE_TIME"] = value;
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
    }
    public class C_SEQNO
    {
        public string SEQ_NAME{get;set;}
        public string SEQ_NO{get;set;}
        public double? DIGITS{get;set;}
        public string BASE_CODE{get;set;}
        public string MINIMUM{get;set;}
        public string MAXIMUM{get;set;}
        public string PREFIX{get;set;}
        public string FIXED{get;set;}
        public string SEQ_FORM{get;set;}
        public string RESET{get;set;}
        public DateTime? USE_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string ID{get;set;}
    }
}