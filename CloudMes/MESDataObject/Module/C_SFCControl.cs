using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SFCControl : DataObjectTable
    {
        public T_C_SFCControl(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SFCControl(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SFCControl);
            TableName = "c_control".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_SFCControl> GetAllControl(OleExec DB)
        {
            List<C_SFCControl> aqls = new List<C_SFCControl>();
            string sql = string.Empty;
            DataTable dt = new DataTable("c_control");
            Row_C_SFCControl aqlsRow = (Row_C_SFCControl)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" select * from c_control where rownum<=20";

                //sql = $@"select ID, CONTROL_VALUE|| '(' || CONTROL_DESC || ')' AS CONTROL_NAME,CONTROL_VALUE,CONTROL_TYPE,CONTROL_LEVEL,CONTROL_DESC,EDIT_EMP,EDIT_TIME  from C_CONTROL  where CONTROL_NAME = 'SFCCONTROL' ORDER BY 1";

                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    aqlsRow.loadData(dr);
                    aqls.Add(aqlsRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return aqls;
        }
        /// <summary>
        /// 通過control_name,control_value取C_CONTROL表的列表
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="controlValue"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<C_SFCControl> GetControls(string ControlName, string ControlValue, OleExec DB)
        {
            List<C_SFCControl> aqls = new List<C_SFCControl>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_CONTROL");
            Row_C_SFCControl aqlsRow = (Row_C_SFCControl)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                if (ControlName.ToString() != "ALL" && ControlValue.ToString() != "")
                {
                    sql = $@" select * from c_control where CONTROL_NAME='{ControlName}' and CONTROL_VALUE='{ControlValue}'   ";
                }
                else
                {
                    if (ControlName.ToString() != "ALL")
                    {
                        sql = $@" select * from c_control where CONTROL_NAME='{ControlName}'  ";
                    }
                    else
                    {
                        sql = $@" select * from c_control where CONTROL_VALUE='{ControlValue}'   ";
                    }                
                }
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    aqlsRow.loadData(dr);
                    aqls.Add(aqlsRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return aqls;
        }
        //获取control_name
        public List<string> GetControlName(OleExec DB)
        {
            List<string> aqls = new List<string>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_CONTROL");
            Row_C_SFCControl aqlsRow = (Row_C_SFCControl)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {

                //sql = $@" SELECT DISTINCT CONTROL_NAME FROM C_CONTROL WHERE  CONTROL_NAME IS NOT NULL";
                sql = $@"select CONTROL_VALUE|| '(' || CONTROL_DESC || ')' AS CONTROL_NAME  from C_CONTROL  where CONTROL_NAME = 'SFCCONTROL' ORDER BY 1";

                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    aqls.Add(dr["CONTROL_NAME"].ToString());
                   //aqlsRow.loadData(dr);
                   //aqls.Add(aqlsRow.GetDataObjects());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return aqls;
        }
        public List<C_SFCControl> GetControl(string ControlName, string ControlValue, OleExec DB)
        {
            List<C_SFCControl> aqls = new List<C_SFCControl>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_CONTROL");
            Row_C_SFCControl aqlsRow = (Row_C_SFCControl)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" select * from c_control where CONTROL_NAME='{ControlName}' and CONTROL_VALUE='{ControlValue}'   ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    aqlsRow.loadData(dr);
                    aqls.Add(aqlsRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return aqls;
        }


    }
    public class Row_C_SFCControl : DataObjectBase
    {
        public Row_C_SFCControl(DataObjectInfo info) : base(info)
        {

        }
        public C_SFCControl GetDataObject()
        {
            C_SFCControl DataObject = new C_SFCControl();
            DataObject.ID = this.ID;
            DataObject.CONTROL_NAME = this.CONTROL_NAME;
            DataObject.CONTROL_VALUE = this.CONTROL_VALUE;
            DataObject.CONTROL_TYPE = this.CONTROL_TYPE;
            DataObject.CONTROL_LEVEL = this.CONTROL_LEVEL;
            DataObject.CONTROL_DESC = this.CONTROL_DESC;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public C_SFCControl GetDataObjects()
        {
            C_SFCControl DataObject = new C_SFCControl();       
            DataObject.CONTROL_NAME = this.CONTROL_NAME;           
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
        public string CONTROL_NAME
        {
            get

            {
                return (string)this["CONTROL_NAME"];
            }
            set
            {
                this["CONTROL_NAME"] = value;
            }
        }
        public string CONTROL_VALUE
        {
            get

            {
                return (string)this["CONTROL_VALUE"];
            }
            set
            {
                this["CONTROL_VALUE"] = value;
            }
        }
        public string CONTROL_TYPE
        {
            get

            {
                return (string)this["CONTROL_TYPE"];
            }
            set
            {
                this["CONTROL_TYPE"] = value;
            }
        }
        public string CONTROL_LEVEL
        {
            get

            {
                return (string)this["CONTROL_LEVEL"];
            }
            set
            {
                this["CONTROL_LEVEL"] = value;
            }
        }
        public string CONTROL_DESC
        {
            get

            {
                return (string)this["CONTROL_DESC"];
            }
            set
            {
                this["CONTROL_DESC"] = value;
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
    public class C_SFCControl
    {
        public string ID { get; set; }
        public string CONTROL_NAME { get; set; }
        public string CONTROL_VALUE { get; set; }
        public string CONTROL_TYPE { get; set; }
        public string CONTROL_LEVEL { get; set; }
        public string CONTROL_DESC { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}