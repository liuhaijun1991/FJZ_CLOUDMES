using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_LIST_DF : DataObjectTable
    {
        public T_C_KP_LIST_DF(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_LIST_DF(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_LIST_DF);
            TableName = "C_KP_LIST_DF".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 根據不同的條件查找相應的數據
        /// </summary>
        /// <param name="strSql">SQL語句</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_KP_LIST_DF> GetByConditions(string strSql, OleExec DB)
        {
            List<C_KP_LIST_DF> result = new List<C_KP_LIST_DF>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {

                    Row_C_KP_LIST_DF ret = (Row_C_KP_LIST_DF)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool JudgeByConditio(string strSql, OleExec DB)
        {

            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataTable GetByCondition(string strSql, OleExec DB)
        {
            List<C_KP_LIST_DF> result = new List<C_KP_LIST_DF>();

            //string res1 = DB.ExecuteScalar(strSql,CommandType.Text);

            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (/*res1 != null */res.Rows.Count > 0)
            {
                //DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
                return res;
            }
            else
            {
                return null;
            }
        }
        public int ReturnResult(string strSql, OleExec DB)
        {
            int i = DB.ExecuteNonQuery(strSql, CommandType.Text);
            return i;
        }

    }
    public class Row_C_KP_LIST_DF : DataObjectBase
    {
        public Row_C_KP_LIST_DF(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_LIST_DF GetDataObject()
        {
            C_KP_LIST_DF DataObject = new C_KP_LIST_DF();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.MAIN_SKUNO = this.MAIN_SKUNO;
            DataObject.KP_PARTNO = this.KP_PARTNO;
            DataObject.STATION = this.STATION;
            DataObject.QTY = this.QTY;
            DataObject.TOTAL_QTY = this.TOTAL_QTY;
            DataObject.SEQ = this.SEQ;
            DataObject.LOCATION = this.LOCATION;
            DataObject.PREFIX = this.PREFIX;
            DataObject.SN_LENGTH = this.SN_LENGTH;
            DataObject.COLOR = this.COLOR;
            DataObject.CHECK_SKUNO_FLAG = this.CHECK_SKUNO_FLAG;
            DataObject.MPN = this.MPN;
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
        public string MAIN_SKUNO
        {
            get
            {
                return (string)this["MAIN_SKUNO"];
            }
            set
            {
                this["MAIN_SKUNO"] = value;
            }
        }
        public string KP_PARTNO
        {
            get
            {
                return (string)this["KP_PARTNO"];
            }
            set
            {
                this["KP_PARTNO"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
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
        public double? SEQ
        {
            get
            {
                return (double?)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
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
        public double? SN_LENGTH
        {
            get
            {
                return (double?)this["SN_LENGTH"];
            }
            set
            {
                this["SN_LENGTH"] = value;
            }
        }
        public string COLOR
        {
            get
            {
                return (string)this["COLOR"];
            }
            set
            {
                this["COLOR"] = value;
            }
        }
        public string CHECK_SKUNO_FLAG
        {
            get
            {
                return (string)this["CHECK_SKUNO_FLAG"];
            }
            set
            {
                this["CHECK_SKUNO_FLAG"] = value;
            }
        }
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
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
    public class C_KP_LIST_DF
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string MAIN_SKUNO { get; set; }
        public string KP_PARTNO { get; set; }
        public string STATION { get; set; }
        public double? QTY { get; set; }
        public double? TOTAL_QTY { get; set; }
        public double? SEQ { get; set; }
        public string LOCATION { get; set; }
        public string PREFIX { get; set; }
        public double? SN_LENGTH { get; set; }
        public string COLOR { get; set; }
        public string CHECK_SKUNO_FLAG { get; set; }
        public string MPN { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}