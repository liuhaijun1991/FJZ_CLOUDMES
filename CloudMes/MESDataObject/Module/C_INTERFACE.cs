using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;

namespace MESDataObject.Module
{
    public class T_C_INTERFACE : DataObjectTable
    {
        public T_C_INTERFACE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_INTERFACE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_INTERFACE);
            TableName = "C_INTERFACE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }



        public List<C_INTERFACE> GetInterfaceStatus(string BU, string IP, string Program_Name, string Item_Name, string Emp_No, OleExec DB, DB_TYPE_ENUM DBType)
        {
            //string Message = "";
            List<C_INTERFACE> ListInterface = new List<C_INTERFACE>();
            Dictionary<string, string> Dic_Interface = new Dictionary<string, string>();
            string StrSql = $@"SELECT * from C_INTERFACE where program_name='{Program_Name}' ";
            DataTable dt = DB.ExecSelect(StrSql).Tables[0];

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    C_INTERFACE Row = GetRow(dr);
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

            //InsertLog(BU, Program_Name, Item_Name, Message, StrSql, Emp_No, DB, DBType);            
        }

        public bool StartInterfaceItem(string BU, string IP, string Program_Name, string Item_Name, string Emp_No, OleExec DB, DB_TYPE_ENUM DBTyp)
        {
            try
            {
                string StrSql_Update = $@"update C_INTERFACE set Item_Status='1' where program_name='{Program_Name}' and item_name='{Item_Name}' ";
                DB.ExecSQL(StrSql_Update);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return true;
        }

        public bool StopInterfaceItem(string BU, string IP, string Program_Name, string Item_Name, string Emp_No, OleExec DB, DB_TYPE_ENUM DBTyp)
        {
            try
            {
                string StrSql_Update = $@"update C_INTERFACE set set Item_Status='0' where program_name='{Program_Name}' and item_name='{Item_Name}' ";
                DB.ExecSQL(StrSql_Update);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return true;
        }


        public List<C_INTERFACE> UpdateNextRunTime(string BU, string IP, string Program_Name, string Item_Name, string Emp_No, OleExec DB, DB_TYPE_ENUM DBType)
        {
            
            //string Message = "";
            string StrRunType = "";
            string StrTimeList = "";
            string StrTime = "";
            string StrNextTime = "";
            string StrDate = "";
            string[] ArryStrTime = null;
            string StrAddDay = "";
            Int64 IntMinRuntime = 0;
            Int64 IntMaxRuntime = 0;
            List<C_INTERFACE> ListInterface = new List<C_INTERFACE>();

            try
            {
                Dictionary<string, string> Dic_Interface = new Dictionary<string, string>();
                //string StrSql = $@"SELECT * from C_INTERFACE where program_name='{Program_Name}' and Item_Name={Item_Name}";
                DataTable Dt = GetItemInfo(Program_Name, Item_Name, DB);

                string StrSql_Update = "";
                DataTable DtTime = null;

                if (Dt.Rows.Count >= 0)
                {
                    StrRunType = Dt.Rows[0]["RUN_TYPE"].ToString();
                    StrTimeList = Dt.Rows[0]["RUN_TIME"].ToString();
                    DtTime = GetCurrentDate(DB, StrTimeList);
                    StrTime = DtTime.Rows[0]["CurrentTime"].ToString();
                    StrDate = DtTime.Rows[0]["CurrentDate"].ToString();
                    if (StrRunType == "0")
                    {
                        if (Convert.ToDateTime(Dt.Rows[0]["NEXT_RUN_DATE"].ToString()) <= Convert.ToDateTime(StrDate))
                        {
                            StrSql_Update = $@"update C_INTERFACE set last_run_date=next_run_date,next_run_date=SYSDATE+to_number({ StrTimeList})/24 where program_name='{Program_Name}' and item_name='{Item_Name}' ";

                            DB.ExecSQL(StrSql_Update);
                        }

                    }
                    else
                    {
                        StrTimeList = Dt.Rows[0]["RUN_TIME"].ToString();
                        ArryStrTime = StrTimeList.Split(',');

                        if (ArryStrTime.Length > 0)
                        {
                            IntMinRuntime = Convert.ToInt64(ArryStrTime[0].ToString().Replace(":", "").ToString());
                            IntMaxRuntime = Convert.ToInt64(ArryStrTime[ArryStrTime.Length - 1].ToString().Replace(":", "").ToString());

                            if (Convert.ToInt64(StrTime.Replace(":", "").ToString()) <= IntMinRuntime)
                            {
                                StrNextTime = ArryStrTime[0].ToString();
                            }
                            else if (Convert.ToInt64(StrTime.Replace(":", "").ToString()) >= IntMaxRuntime)
                            {
                                StrAddDay = "1";
                                StrNextTime = ArryStrTime[0].ToString();
                            }
                            else
                            {
                                for (int i = 1; i < ArryStrTime.Length; i++)
                                {
                                    if (Convert.ToInt64(StrTime.Replace(":", "").ToString()) <= Convert.ToInt64(ArryStrTime[i].ToString().Replace(":", "").ToString()))
                                    {
                                        StrNextTime = ArryStrTime[i].ToString();
                                    }
                                }
                            }
                            if (StrNextTime != "")
                            {
                                if (StrAddDay == "1")
                                {
                                    StrSql_Update = $@"update C_INTERFACE set last_run_date=next_run_date,next_run_date=TO_DATE(TO_CHAR(SYSDATE+1,'YYYY-MM-DD')||'{StrNextTime}','YYYY-MM-DD HH24:MI:SS') where program_name='{Program_Name}' and item_name='{Item_Name}' ";
                                }
                                else
                                {
                                    StrSql_Update = $@"update C_INTERFACE set last_run_date=next_run_date,next_run_date=TO_DATE(TO_CHAR(SYSDATE,'YYYY-MM-DD')||'{StrNextTime}','YYYY-MM-DD HH24:MI:SS') where program_name='{Program_Name}' and item_name='{Item_Name}' ";
                                }
                            }
                            DB.ExecSQL(StrSql_Update);
                        }
                    }
                }
                ListInterface = GetInterfaceStatus(BU, IP, Program_Name, Item_Name, Emp_No, DB, DBType);
            }
            catch(Exception ex)
            {
                throw (ex);
            }
            return ListInterface;
        }

        public DataTable GetCurrentDate(OleExec DB, string StrTimeList)
        {
            DataTable Dt = null;
            try
            {
                string StrSql = $@"SELECT SYSDATE AS CurrentDate,TO_CHAR(SYSDATE,'HH24:MI:SS') AS CurrentTime FROM DUAL ";
                Dt = DB.ExecSelect(StrSql).Tables[0];
            }
            catch(Exception ex)
            {
                throw (ex);
            }
           return Dt;
        }

        public DataTable GetItemInfo(string Program_Name,string Item_Name,OleExec DB)
        {
            DataTable Dt = null;
            try
            {
                string StrSql = $@"SELECT * from C_INTERFACE where program_name='{Program_Name}' and Item_Name='{Item_Name}'";
                Dt = DB.ExecSelect(StrSql).Tables[0];
            }
            catch(Exception ex)
            {
                throw (ex);
            }
            return Dt;
        }

        /// <summary>
        /// 檢查前台服務器IP與後台服務器IP是否一致
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="IP"></param>
        /// <param name="Program_Name"></param>
        /// <param name="Item_Name"></param>
        /// <param name="Emp_No"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public C_INTERFACE CHECK_IP(string BU, string IP, string Program_Name, string Item_Name, string Emp_No, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string Message = "";
            List<C_INTERFACE> ListInterface = new List<C_INTERFACE>();
            try
            {
                string StrSql = $@"SELECT * from C_Program_Server where program_name='{Program_Name}' and server_ip='{IP}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    C_INTERFACE Row = GetRow(dt.Rows[0]);
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
            catch (Exception ex)
            {
                throw (ex);
            }        
        }

        public C_INTERFACE GetRow(DataRow DR)
        {
            Row_C_INTERFACE Row_Interface = (Row_C_INTERFACE)NewRow();
            Row_Interface.loadData(DR);
            return Row_Interface.GetDataObject();
        }
    }
    public class Row_C_INTERFACE : DataObjectBase
    {
        public Row_C_INTERFACE(DataObjectInfo info) : base(info)
        {

        }
        public C_INTERFACE GetDataObject()
        {
            C_INTERFACE DataObject = new C_INTERFACE();
            DataObject.ID = this.ID;
            DataObject.PROGRAM_NAME = this.PROGRAM_NAME;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.FUNCTION_NAME = this.FUNCTION_NAME;
            DataObject.ITEM_NAME = this.ITEM_NAME;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.ITEM_STATUS = this.ITEM_STATUS;
            DataObject.LAST_RUN_DATE = this.LAST_RUN_DATE;
            DataObject.NEXT_RUN_DATE = this.NEXT_RUN_DATE;
            DataObject.RUN_TYPE = this.RUN_TYPE;
            DataObject.RUN_TIME = this.RUN_TIME;
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
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
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
        public string ITEM_NAME
        {
            get
            {
                return (string)this["ITEM_NAME"];
            }
            set
            {
                this["ITEM_NAME"] = value;
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
        public string ITEM_STATUS
        {
            get
            {
                return (string)this["ITEM_STATUS"];
            }
            set
            {
                this["ITEM_STATUS"] = value;
            }
        }
        public DateTime? LAST_RUN_DATE
        {
            get
            {
                return (DateTime?)this["LAST_RUN_DATE"];
            }
            set
            {
                this["LAST_RUN_DATE"] = value;
            }
        }
        public DateTime? NEXT_RUN_DATE
        {
            get
            {
                return (DateTime?)this["NEXT_RUN_DATE"];
            }
            set
            {
                this["NEXT_RUN_DATE"] = value;
            }
        }
        public string RUN_TYPE
        {
            get
            {
                return (string)this["RUN_TYPE"];
            }
            set
            {
                this["RUN_TYPE"] = value;
            }
        }
        public string RUN_TIME
        {
            get
            {
                return (string)this["RUN_TIME"];
            }
            set
            {
                this["RUN_TIME"] = value;
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
    public class C_INTERFACE
    {
        public string ID{get;set;}
        public string PROGRAM_NAME{get;set;}
        public string CLASS_NAME{get;set;}
        public string FUNCTION_NAME{get;set;}
        public string ITEM_NAME{get;set;}
        public string SEQ_NO{get;set;}
        public string ITEM_STATUS{get;set;}
        public DateTime? LAST_RUN_DATE{get;set;}
        public DateTime? NEXT_RUN_DATE{get;set;}
        public string RUN_TYPE{get;set;}
        public string RUN_TIME{get;set;}
        public string DESCRIPTION{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}