using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PARAMETER : DataObjectTable
    {
        public T_C_PARAMETER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PARAMETER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PARAMETER);
            TableName = "C_PARAMETER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Dictionary<String, String> Get_Interface_Parameter(string Function_Name, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select parameter_name,parameter_value from C_PARAMETER where function_name='{Function_Name}' ";
            DataTable dt = DB.ExecSelect(strSql).Tables[0];
            Dictionary<string, string> SAP_Para = new Dictionary<string, string>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SAP_Para.Add(dt.Rows[i]["parameter_name"].ToString(), dt.Rows[i]["parameter_value"].ToString());
                }
                return SAP_Para;
            }
            else
            {
                return null;
            }
        }

        public List<C_PARAMETER> Get_Interface_Parameter_1(string Function_Name, OleExec DB, DB_TYPE_ENUM DBType)
        {
            //string Message = "";
            List<C_PARAMETER> ListInterface = new List<C_PARAMETER>();
            Dictionary<string, string> Dic_Interface = new Dictionary<string, string>();
            string StrSql = $@"select parameter_name,parameter_value from C_PARAMETER where function_name='{Function_Name}' "; ;
            DataTable dt = DB.ExecSelect(StrSql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    C_PARAMETER Row = GetRow(dr);
                    ListInterface.Add(Row);
                }
                return ListInterface;
            }
            else
            {
                if (dt.Rows.Count > 1)
                {
                    //Message = "配置信息有多筆!";
                }
                else if (dt.Rows.Count == 0)
                {
                    //Message = "信息未配置";
                }

                return null;
            }
        }

        public Dictionary<String, String> Get_Interface_Parameter_2(string Function_Name, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select parameter_name,parameter_value from C_PARAMETER where function_name='{Function_Name}' ";
            DataTable dt = DB.ExecSelect(strSql).Tables[0];
            Dictionary<string, string> SAP_Para = new Dictionary<string, string>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SAP_Para.Add(dr["parameter_name"].ToString(), dr["parameter_value"].ToString());
                }
                return SAP_Para;
            }
            else
            {
                return null;
            }

            //InsertLog(BU, Program_Name, Item_Name, Message, StrSql, Emp_No, DB, DBType);            
        }

        public C_PARAMETER GetRow(DataRow DR)
        {
            Row_C_PARAMETER Row_PARAMETER = (Row_C_PARAMETER)NewRow();
            Row_PARAMETER.loadData(DR);
            return Row_PARAMETER.GetDataObject();
        }
    }
    public class Row_C_PARAMETER : DataObjectBase
    {
        public Row_C_PARAMETER(DataObjectInfo info) : base(info)
        {

        }
        public C_PARAMETER GetDataObject()
        {
            C_PARAMETER DataObject = new C_PARAMETER();
            DataObject.ID = this.ID;
            DataObject.FUNCTION_NAME = this.FUNCTION_NAME;
            DataObject.PARAMETER_NAME = this.PARAMETER_NAME;
            DataObject.PARAMETER_VALUE = this.PARAMETER_VALUE;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string FUNCTION_NAME
        {
            get
            {
                return (string)this["FUNCTION_NAME"];
            }
            set
            {
                this["FUNCTION_NAME"] = value;
            }
        }
        public string PARAMETER_NAME
        {
            get
            {
                return (string)this["PARAMETER_NAME"];
            }
            set
            {
                this["PARAMETER_NAME"] = value;
            }
        }
        public string PARAMETER_VALUE
        {
            get
            {
                return (string)this["PARAMETER_VALUE"];
            }
            set
            {
                this["PARAMETER_VALUE"] = value;
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
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class C_PARAMETER
    {
        public string ID{get;set;}
        public string FUNCTION_NAME{get;set;}
        public string PARAMETER_NAME{get;set;}
        public string PARAMETER_VALUE{get;set;}
        public double? SEQ_NO{get;set;}
        public string DESCRIPTION{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime EDIT_TIME{get;set;}
    }
}