using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SUMMERTIME : DataObjectTable
    {
        public T_C_SUMMERTIME(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SUMMERTIME(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SUMMERTIME);
            TableName = "C_SUMMERTIME".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_SUMMERTIME GetCSUMMERTIME(OleExec DB)
        {
            return DB.ORM.Queryable<C_SUMMERTIME>().Take(0).ToList().FirstOrDefault();
        }

        public int GetSummerTimeByFunction(OleExec DB)
        {
            int res = 0;
            string strSQL = $@"select sfc.fun_datediff(to_char(sysdate, 'yyyy-mm-dd')) from dual";
            DataTable Dt = DB.ExecSelect(strSQL).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                res = int.Parse(Dt.Rows[0][0].ToString());
            }
            return res;
        }
    }
    public class Row_C_SUMMERTIME : DataObjectBase
    {
        public Row_C_SUMMERTIME(DataObjectInfo info) : base(info)
        {

        }
        public C_SUMMERTIME GetDataObject()
        {
            C_SUMMERTIME DataObject = new C_SUMMERTIME();
            DataObject.ID = this.ID;
            DataObject.TIME_TYPE = this.TIME_TYPE;
            DataObject.TIME_DIFF = this.TIME_DIFF;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public string TIME_TYPE
        {
            get
            {
                return (string)this["TIME_TYPE"];
            }
            set
            {
                this["TIME_TYPE"] = value;
            }
        }
        public string TIME_DIFF
        {
            get
            {
                return (string)this["TIME_DIFF"];
            }
            set
            {
                this["TIME_DIFF"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
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
    public class C_SUMMERTIME
    {
        public string ID { get; set; }
        public string TIME_TYPE { get; set; }
        public string TIME_DIFF { get; set; }
        public string VALID_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}