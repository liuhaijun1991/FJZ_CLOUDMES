using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PARTNO_EXCEPTION : DataObjectTable
    {
        public T_C_PARTNO_EXCEPTION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PARTNO_EXCEPTION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PARTNO_EXCEPTION);
            TableName = "C_PARTNO_EXCEPTION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public bool ValueIsExist(string PN, OleExec DB)
        {
            return DB.ORM.Queryable<C_PARTNO_EXCEPTION>().Any(t => t.EXCEPTIONTYPE == "SkipAssyLocationChk" && t.PARTNO == PN);
        }

        public List<C_PARTNO_EXCEPTION> _GetPNExceptionDetail(OleExec sfcdb, string PARTNO, string exceptiontype)
        {
            DataTable dt = null;
            Row_C_PARTNO_EXCEPTION row_pn_excp = null;
            List<C_PARTNO_EXCEPTION> pn_excp = new List<C_PARTNO_EXCEPTION>();


            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select * from {TableName} where 1=1 ";
                if (!string.IsNullOrEmpty(PARTNO))
                {
                    sql += $@"and PARTNO like '%{PARTNO.Replace("'", "''")}%' ";
                }
                if (!string.IsNullOrEmpty(exceptiontype))
                {
                    sql += $@"and EXCEPTIONTYPE like '%{exceptiontype.Replace("'", "''")}%' ";
                }                
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_pn_excp = (Row_C_PARTNO_EXCEPTION)NewRow();
                        row_pn_excp.loadData(dr);
                        pn_excp.Add(row_pn_excp.GetDataObject());
                    }
                    return pn_excp;
                }
                catch (Exception ex)
                {

                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this.DBType.ToString() }));
            }

        }

        public List<C_PARTNO_EXCEPTION> GetAllPNException(OleExec DB, DB_TYPE_ENUM DBType)
        {
            List<C_PARTNO_EXCEPTION> Ret = new List<C_PARTNO_EXCEPTION>();
            string StrSql = $@"select * from C_PARTNO_EXCEPTION order by PARTNO desc ";
            DataTable DT = DB.ExecSelect(StrSql).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow DR in DT.Rows)
                {
                    C_PARTNO_EXCEPTION Row = GetRow(DR);
                    Ret.Add(Row);
                }
                return Ret;
            }
            else
            {
                return null;
            }
        }

        public List<C_PARTNO_EXCEPTION> GetAllException(OleExec DB, DB_TYPE_ENUM DBType)
        {
            List<C_PARTNO_EXCEPTION> Ret = new List<C_PARTNO_EXCEPTION>();
            string StrSql = $@"select distinct EXCEPTIONTYPE from C_PARTNO_EXCEPTION";
            DataTable DT = DB.ExecSelect(StrSql).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow DR in DT.Rows)
                {
                    C_PARTNO_EXCEPTION Row = GetRow(DR);
                    Ret.Add(Row);
                }
                return Ret;
            }
            else
            {
                return null;
            }
        }

        public Row_C_PARTNO_EXCEPTION GetExceptionByPN(string pn, string excp,OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_PARTNO_EXCEPTION  where PARTNO='{pn}' and EXCEPTIONTYPE = '{excp}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_PARTNO_EXCEPTION ret = (Row_C_PARTNO_EXCEPTION)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<C_PARTNO_EXCEPTION> GetPNExceptionByPN(string pn,OleExec DB)
        {
            List<C_PARTNO_EXCEPTION> Ret = new List<C_PARTNO_EXCEPTION>();
            string StrSql = $@"select * from C_PARTNO_EXCEPTION where PARTNO = '"+pn+"' ";
            DataTable DT = DB.ExecSelect(StrSql).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow DR in DT.Rows)
                {
                    C_PARTNO_EXCEPTION Row = GetRow(DR);
                    Ret.Add(Row);
                }
                return Ret;
            }
            else
            {
                return null;
            }
        }


        public C_PARTNO_EXCEPTION GetRow(DataRow DR)
        {
            Row_C_PARTNO_EXCEPTION Ret = (Row_C_PARTNO_EXCEPTION)NewRow();
            Ret.loadData(DR);
            return Ret.GetDataObject();
        }


    }
    public class Row_C_PARTNO_EXCEPTION : DataObjectBase
    {
        public Row_C_PARTNO_EXCEPTION(DataObjectInfo info) : base(info)
        {

        }
        public C_PARTNO_EXCEPTION GetDataObject()
        {
            C_PARTNO_EXCEPTION DataObject = new C_PARTNO_EXCEPTION();
            DataObject.PARTNO = this.PARTNO;
            DataObject.EXCEPTIONTYPE = this.EXCEPTIONTYPE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string EXCEPTIONTYPE
        {
            get
            {
                return (string)this["EXCEPTIONTYPE"];
            }
            set
            {
                this["EXCEPTIONTYPE"] = value;
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
    public class C_PARTNO_EXCEPTION
    {
        public string PARTNO { get; set; }
        public string EXCEPTIONTYPE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}
