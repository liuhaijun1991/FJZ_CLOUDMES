using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PROGRAM_SERVER : DataObjectTable
    {
        public T_C_PROGRAM_SERVER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PROGRAM_SERVER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PROGRAM_SERVER);
            TableName = "C_PROGRAM_SERVER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_PROGRAM_SERVER> GetProgramServer(string BU, string IP, string Program_Name, string Item_Name, string Emp_No, OleExec DB, DB_TYPE_ENUM DBType)
        {
            //string Message = "";
            List<C_PROGRAM_SERVER> ListProgramServer = new List<C_PROGRAM_SERVER>();
            string StrSql = $@"SELECT * from C_PROGRAM_SERVER where program_name='{Program_Name}' ";
            DataTable dt = DB.ExecSelect(StrSql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    C_PROGRAM_SERVER Row = GetRow(dr);
                    ListProgramServer.Add(Row);
                }
                return ListProgramServer;
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

            //InsertLog(BU, Program_Name, Item_Name, Message, StrSql, Emp_No, DB, DBType);            
        }

        public C_PROGRAM_SERVER CHECK_IP(string BU, string IP, string Program_Name, string Item_Name, string Emp_No, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string Message = "";
            List<C_PROGRAM_SERVER> ListInterface = new List<C_PROGRAM_SERVER>();
            Dictionary<string, string> Dic_Interface = new Dictionary<string, string>();
            string StrSql = $@"SELECT * from C_Program_Server where program_name='{Program_Name}' and server_ip='{IP}' ";
            DataTable dt = DB.ExecSelect(StrSql).Tables[0];

            if (dt.Rows.Count == 1)
            {

                C_PROGRAM_SERVER Row = GetRow(dt.Rows[0]);
                return Row;
            }
            else
            {
                if (dt.Rows.Count > 1)
                {
                    Message = Item_Name + "配置信息有多筆!";
                }
                else if (dt.Rows.Count == 0)
                {
                    Message = Item_Name + "信息未配置";
                }

                return null;
            }

            //InsertLog(BU, Program_Name, Item_Name, Message, StrSql, Emp_No, DB, DBType);            
        }

        public C_PROGRAM_SERVER GetRow(DataRow DR)
        {
            Row_C_PROGRAM_SERVER Row_Program_Server = (Row_C_PROGRAM_SERVER)NewRow();
            Row_Program_Server.loadData(DR);
            return Row_Program_Server.GetDataObject();
        }
    }
    public class Row_C_PROGRAM_SERVER : DataObjectBase
    {
        public Row_C_PROGRAM_SERVER(DataObjectInfo info) : base(info)
        {

        }
        public C_PROGRAM_SERVER GetDataObject()
        {
            C_PROGRAM_SERVER DataObject = new C_PROGRAM_SERVER();
            DataObject.ID = this.ID;
            DataObject.PROGRAM_NAME = this.PROGRAM_NAME;
            DataObject.SERVER_IP = this.SERVER_IP;
            DataObject.PORT = this.PORT;
            DataObject.SERVICE_NAME = this.SERVICE_NAME;
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
        public string PROGRAM_NAME
        {
            get
            {
                return (string)this["PROGRAM_NAME"];
            }
            set
            {
                this["PROGRAM_NAME"] = value;
            }
        }
        public string SERVER_IP
        {
            get
            {
                return (string)this["SERVER_IP"];
            }
            set
            {
                this["SERVER_IP"] = value;
            }
        }
        public string PORT
        {
            get
            {
                return (string)this["PORT"];
            }
            set
            {
                this["PORT"] = value;
            }
        }
        public string SERVICE_NAME
        {
            get
            {
                return (string)this["SERVICE_NAME"];
            }
            set
            {
                this["SERVICE_NAME"] = value;
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
    public class C_PROGRAM_SERVER
    {
        public string ID{get;set;}
        public string PROGRAM_NAME{get;set;}
        public string SERVER_IP{get;set;}
        public string PORT{get;set;}
        public string SERVICE_NAME{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}