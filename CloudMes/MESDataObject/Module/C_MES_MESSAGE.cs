using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_MES_MESSAGE : DataObjectTable
    {
        public T_C_MES_MESSAGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_MES_MESSAGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_MES_MESSAGE);
            TableName = "C_MES_MESSAGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_C_MES_MESSAGE GetMESMessageByMessageCode(string MessageCode, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from C_MES_MESSAGE where message_code='{MessageCode}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_MES_MESSAGE ret = (Row_C_MES_MESSAGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<C_MES_MESSAGE> GetAllMESMessage(OleExec DB, DB_TYPE_ENUM DBType)
        {
            List<C_MES_MESSAGE> Ret = new List<C_MES_MESSAGE>();
            string StrSql = $@"select * from C_MES_MESSAGE order by message_code desc ";
            DataTable DT = DB.ExecSelect(StrSql).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow DR in DT.Rows)
                {
                    C_MES_MESSAGE Row = GetRow(DR);
                    Ret.Add(Row);
                }
                return Ret;
            }
            else
            {
                return null;
            }
        }

        public C_MES_MESSAGE GetRow(DataRow DR)
        {
            Row_C_MES_MESSAGE Ret = (Row_C_MES_MESSAGE)NewRow();
            Ret.loadData(DR);
            return Ret.GetDataObject();
        }

        public List<C_MES_MESSAGE> GetMsgDetail(OleExec sfcdb, string field, string value)
        {
            DataTable dt = null;
            Row_C_MES_MESSAGE row_msg = null;
            List<C_MES_MESSAGE> msgs = new List<C_MES_MESSAGE>();
            //for a test
            bool flag = this.NewRow().Keys.Contains(field);
            if (flag)
            {

            }

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select * from {TableName} where {field} like '%{value}%' ";
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_msg = (Row_C_MES_MESSAGE) NewRow();
                        row_msg.loadData(dr);
                        msgs.Add(row_msg.GetDataObject());
                    }
                    return msgs;
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

        public List<C_MES_MESSAGE> _GetMsgDetail(OleExec sfcdb, string zh_cn, string zh_tw, string en)
        {
            DataTable dt = null;
            Row_C_MES_MESSAGE row_msg = null;
            List<C_MES_MESSAGE> msgs = new List<C_MES_MESSAGE>();
            

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select * from {TableName} where 1=1 ";
                if (!string.IsNullOrEmpty(zh_cn))
                {
                    sql += $@"and chinese like '%{zh_cn.Replace("'", "''")}%' ";
                }
                if (!string.IsNullOrEmpty(zh_tw))
                {
                    sql += $@"and chinese_tw like '%{zh_tw.Replace("'", "''")}%' ";
                }
                if (!string.IsNullOrEmpty(en))
                {
                    sql += $@"and english like '%{en.Replace("'", "''")}%' ";
                }
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_msg = (Row_C_MES_MESSAGE)NewRow();
                        row_msg.loadData(dr);
                        msgs.Add(row_msg.GetDataObject());
                    }
                    return msgs;
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

    }

    

    public class Row_C_MES_MESSAGE : DataObjectBase
    {
        public Row_C_MES_MESSAGE(DataObjectInfo info) : base(info)
        {

        }
        public C_MES_MESSAGE GetDataObject()
        {
            C_MES_MESSAGE DataObject = new C_MES_MESSAGE();
            DataObject.ID = this.ID;
            DataObject.MESSAGE_CODE = this.MESSAGE_CODE;
            DataObject.CHINESE = this.CHINESE;
            DataObject.CHINESE_TW = this.CHINESE_TW;
            DataObject.ENGLISH = this.ENGLISH;
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
        public string MESSAGE_CODE
        {
            get
            {
                return (string)this["MESSAGE_CODE"];
            }
            set
            {
                this["MESSAGE_CODE"] = value;
            }
        }
        public string CHINESE
        {
            get
            {
                return (string)this["CHINESE"];
            }
            set
            {
                this["CHINESE"] = value;
            }
        }
        public string CHINESE_TW
        {
            get
            {
                return (string)this["CHINESE_TW"];
            }
            set
            {
                this["CHINESE_TW"] = value;
            }
        }
        public string ENGLISH
        {
            get
            {
                return (string)this["ENGLISH"];
            }
            set
            {
                this["ENGLISH"] = value;
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
    public class C_MES_MESSAGE
    {
        public string ID{get;set;}
        public string MESSAGE_CODE{get;set;}
        public string CHINESE{get;set;}
        public string CHINESE_TW{get;set;}
        public string ENGLISH{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}