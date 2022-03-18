using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PROCESS : DataObjectTable
    {
        public T_C_PROCESS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PROCESS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PROCESS);
            TableName = "C_PROCESS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<string> GetAllProcess(OleExec DB)
        {
            string strSql = $@"select * from c_process order by seq_no";
           
            List<string> result = new List<string>();
            //result.Add("");  
            //BY SDL  加載頁面默認賦予空值,操作員必須點選其他有內容選項 
            //modify by zgj，数据库中已经有空值
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_PROCESS ret = (Row_C_PROCESS)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject().PROCESS);
                }
            }
            return result;
        }
    }
    public class Row_C_PROCESS : DataObjectBase
    {
        public Row_C_PROCESS(DataObjectInfo info) : base(info)
        {

        }
        public C_PROCESS GetDataObject()
        {
            C_PROCESS DataObject = new C_PROCESS();
            DataObject.ID = this.ID;
            DataObject.PROCESS = this.PROCESS;
            DataObject.SEQ_NO = this.SEQ_NO;
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
        public string PROCESS
        {
            get
            {
                return (string)this["PROCESS"];
            }
            set
            {
                this["PROCESS"] = value;
            }
        }
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
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
    public class C_PROCESS
    {
        public string ID{get;set;}
        public string PROCESS{get;set;}
        public double? SEQ_NO{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}