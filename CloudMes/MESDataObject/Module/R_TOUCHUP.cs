using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_TOUCHUP : DataObjectTable
    {
        public T_R_TOUCHUP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TOUCHUP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TOUCHUP);
            TableName = "R_TOUCHUP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 判斷SN是否存在
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckSNExists(string StrSN, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM R_TOUCHUP WHERE SN='{StrSN}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }
        /// <summary>
        /// WZW  插入一條語句
        /// </summary>
        /// <param name="Sn"></param>
        /// <param name="Skuno"></param>
        /// <param name="Emp_NO"></param>
        /// <param name="Bu"></param>
        /// <param name="Line"></param>
        /// <param name="Product_Name"></param>
        /// <param name="Error_Code"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public string InsertSN(string Sn, string Skuno, string Emp_NO, string Bu, string Line, string Product_Name, string Error_Code, OleExec db)
        {
            string result = "";
            T_R_TOUCHUP Table_R_SN = new T_R_TOUCHUP(db, DBType);
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Row_R_TOUCHUP RowsInsert = (Row_R_TOUCHUP)NewRow();
                RowsInsert.ID = Table_R_SN.GetNewID(Bu, db);
                RowsInsert.SKUNO = Skuno;
                RowsInsert.PRODUCT_NAME = Product_Name;
                RowsInsert.SN = Sn;
                RowsInsert.ERROR_CODE = Error_Code;
                RowsInsert.LINE = Line;
                RowsInsert.EDIT_EMP = Emp_NO;
                RowsInsert.EDIT_TIME = GetDBDateTime(db);
                result = db.ExecSQL(RowsInsert.GetInsertString(DBType));
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
    }
    public class Row_R_TOUCHUP : DataObjectBase
    {
        public Row_R_TOUCHUP(DataObjectInfo info) : base(info)
        {

        }
        public R_TOUCHUP GetDataObject()
        {
            R_TOUCHUP DataObject = new R_TOUCHUP();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PRODUCT_NAME = this.PRODUCT_NAME;
            DataObject.SN = this.SN;
            DataObject.ERROR_CODE = this.ERROR_CODE;
            DataObject.LINE = this.LINE;
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
        public string PRODUCT_NAME
        {
            get
            {
                return (string)this["PRODUCT_NAME"];
            }
            set
            {
                this["PRODUCT_NAME"] = value;
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
        public string ERROR_CODE
        {
            get
            {
                return (string)this["ERROR_CODE"];
            }
            set
            {
                this["ERROR_CODE"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
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
    public class R_TOUCHUP
    {
        public string ID;
        public string SKUNO;
        public string PRODUCT_NAME;
        public string SN;
        public string ERROR_CODE;
        public string LINE;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}