using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using Newtonsoft.Json.Linq;

namespace MESDataObject.Module
{
    public class T_C_REASON_CODE : DataObjectTable
    {
        public T_C_REASON_CODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_REASON_CODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_REASON_CODE);
            TableName = "C_REASON_CODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int AddNewReason(OleExec sfcdb, JToken Data)
        {

            return 0;
        }

        public List<C_REASON_CODE> GetAllReasonCode(OleExec sfcdb)
        {
            string sql = $@"select * from {TableName} ";
            List<C_REASON_CODE> lists = new List<C_REASON_CODE>();
            Row_C_REASON_CODE row_code = null;
            DataTable dt = null;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_code = (Row_C_REASON_CODE) this.NewRow();
                        row_code.loadData(dr);
                        if (row_code != null)
                        {
                            lists.Add(row_code.GetDataObject());
                        }
                    }
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
            
            return lists;
        }

        public C_REASON_CODE GetObjByReasonCode(OleExec sfcdb, string rc)
        {
            if (string.IsNullOrEmpty(rc))
            {
                return null;
            }
            string sql = $@"select * from {TableName} where reason_code='{rc.Replace("'","''")}' ";
            Row_C_REASON_CODE row_code = null;
            DataTable dt = null;
            try
            {
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row_code = (Row_C_REASON_CODE) this.NewRow();
                    row_code.loadData(dt.Rows[0]);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return row_code?.GetDataObject();
        }

        public C_REASON_CODE GetObjById(OleExec sfcdb, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            string sql = $@"select * from {TableName} where id='{id.Replace("'", "''")}' ";
            Row_C_REASON_CODE row_code = null;
            DataTable dt = null;
            try
            {
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row_code = (Row_C_REASON_CODE)this.NewRow();
                    row_code.loadData(dt.Rows[0]);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return row_code?.GetDataObject();
        }

    }
    public class Row_C_REASON_CODE : DataObjectBase
    {
        public Row_C_REASON_CODE(DataObjectInfo info) : base(info)
        {

        }
        public C_REASON_CODE GetDataObject()
        {
            C_REASON_CODE DataObject = new C_REASON_CODE();
            DataObject.ID = this.ID;
            DataObject.REASON_CODE = this.REASON_CODE;
            DataObject.ENGLISH_DESCRIPTION = this.ENGLISH_DESCRIPTION;
            DataObject.CHINESE_DESCRIPTION = this.CHINESE_DESCRIPTION;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string REASON_CODE
        {
            get
            {
                return (string)this["REASON_CODE"];
            }
            set
            {
                this["REASON_CODE"] = value;
            }
        }
        public string ENGLISH_DESCRIPTION
        {
            get
            {
                return (string)this["ENGLISH_DESCRIPTION"];
            }
            set
            {
                this["ENGLISH_DESCRIPTION"] = value;
            }
        }
        public string CHINESE_DESCRIPTION
        {
            get
            {
                return (string)this["CHINESE_DESCRIPTION"];
            }
            set
            {
                this["CHINESE_DESCRIPTION"] = value;
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
    }
    public class C_REASON_CODE
    {
        public string ID{get;set;}
        public string REASON_CODE{get;set;}
        public string ENGLISH_DESCRIPTION{get;set;}
        public string CHINESE_DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}