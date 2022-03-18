using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_LINE : DataObjectTable
    {
        public T_C_LINE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_LINE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_LINE);
            TableName = "C_LINE".ToUpper().Trim();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// 檢查數據是否存在
        /// </summary>
        /// <param name="LineName"></param>
        /// <param name="SectionName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckDataExist(string LineName, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM C_LINE WHERE LINE_NAME='{LineName}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }
        public C_LINE GetByLineName(string LineName, OleExec DB)
        {
            //bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM C_LINE WHERE LINE_NAME='{LineName}'";
            dt = DB.ExecSelect(sql).Tables[0];
            C_LINE resultline = new C_LINE();
            if (dt.Rows.Count > 0)
            {
                resultline = CreateLanguageClass(dt.Rows[0]);
            }
            else
            {
                resultline = null;
            }
            return resultline;
        }
        /// <summary>
        /// 獲取所有線體
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_LINE> GetLineData(OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_LINE> LanguageList = new List<C_LINE>();
            sql = $@"SELECT * FROM C_LINE order by LINE_NAME";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }
        /// <summary>
        /// 根據區段取線體
        /// </summary>
        /// <param name="LinePcas"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_LINE> GetLinePcas(string LinePcas, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_LINE> LanguageList = new List<C_LINE>();
            sql = $@"SELECT * FROM C_LINE WHERE LINE_PCAS='{LinePcas}'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }
        public C_LINE GetByPcas(string LinePcas, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            C_LINE resultcLine = new C_LINE();
            sql = $@"SELECT * FROM C_LINE WHERE LINE_PCAS='{LinePcas}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                resultcLine = CreateLanguageClass(dt.Rows[0]);
            }
            else
            {
                resultcLine = null;
            }
            return resultcLine;
        }
        public int add(C_LINE newline, OleExec DB)
        {
            string sql = string.Empty;
            int result = 0;
            Row_C_LINE row = (Row_C_LINE)NewRow();
            row.ID =newline.ID;
            row.LINE_NAME = newline.LINE_NAME;
            row.SECTION_ID = newline.SECTION_ID;
            row.LINE_CODE = newline.LINE_CODE;
            row.LINE_PCAS = newline.LINE_PCAS;
            row.DESCRIPTION = newline.DESCRIPTION;
            row.EDIT_EMP = newline.EDIT_EMP;
            row.EDIT_TIME = newline.EDIT_TIME;
            sql = row.GetInsertString(this.DBType);
            result= DB.ExecuteNonQuery(sql,CommandType.Text);
            return result;
        }
        public int deleteById(string id, OleExec DB)
        {
            string sql ="delete c_line where id=:id";
            int result = 0;
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id",id)
            };
            result = DB.ExecuteNonQuery(sql, CommandType.Text,paramet);
            return result;
        }
        public int Update(C_LINE newline, OleExec DB)
        {

            string sql = "UPDATE  C_LINE SET LINE_NAME =:linename,SECTION_ID =:sectionid,";
            sql = sql + " LINE_CODE =:linecode,LINE_PCAS =:linepacas,DESCRIPTION =:descs,";
            sql = sql + " EDIT_TIME =:edtime,EDIT_EMP =:edemp WHERE ID =:id";
            OleDbParameter[] paramet = new OleDbParameter[] {           
            new OleDbParameter(":linename",newline.LINE_NAME),
            new OleDbParameter(":sectionid", newline.SECTION_ID),
            new OleDbParameter(":linecode",newline.LINE_CODE),
            new OleDbParameter(":linepacas",newline.LINE_PCAS),
            new OleDbParameter(":descs", newline.DESCRIPTION),            
            new OleDbParameter(":edtime", newline.EDIT_TIME),
            new OleDbParameter(":edemp", newline.EDIT_EMP),
            new OleDbParameter(":id", newline.ID)
        };
            int result = DB.ExecuteNonQuery(sql, CommandType.Text,paramet);
            return result;
        }
        public C_LINE GetLineById(string Id, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            C_LINE resultLine = new C_LINE();
            sql = $@"SELECT * FROM C_LINE WHERE Id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id",Id)
            };
            dt = DB.ExecuteDataTable(sql,CommandType.Text,paramet);
            if (dt.Rows.Count > 0)
            {
                resultLine = CreateLanguageClass(dt.Rows[0]);
            }
            else
            {
                resultLine = null;
            }
            return resultLine;
        }

        public C_LINE CreateLanguageClass(DataRow dr)
        {
            Row_C_LINE row = (Row_C_LINE)NewRow();
            row.loadData(dr);
            return row.GetDataObject();
        }

        public DataTable GetAllLine( OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_LINE> LanguageList = new List<C_LINE>();
            sql = $@"select * from c_line order by line_name";
            dt = DB.ExecSelect(sql).Tables[0];

            return dt;
        }
        public List<C_LINE> GetLineBylike(string seachValue, OleExec DB)
        {
            seachValue = seachValue.ToUpper();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<C_LINE> LanguageList = new List<C_LINE>();
            sql = $@"SELECT * FROM C_LINE WHERE upper(LINE_NAME ) like'%{seachValue}%' or upper(SECTION_ID ) like'%{seachValue}%'
                or upper(LINE_CODE ) like'%{seachValue}%' or upper(LINE_PCAS ) like'%{seachValue}%'
                or  upper(DESCRIPTION) like'%{seachValue}%'";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                LanguageList.Add(CreateLanguageClass(dr));
            }
            return LanguageList;
        }
    }
    public class Row_C_LINE : DataObjectBase
    {
        public Row_C_LINE(DataObjectInfo info) : base(info)
        {

        }
        public C_LINE GetDataObject()
        {
            C_LINE DataObject = new C_LINE();
            DataObject.ID = this.ID;
            DataObject.LINE_NAME = this.LINE_NAME;
            DataObject.SECTION_ID = this.SECTION_ID;
            DataObject.LINE_CODE = this.LINE_CODE;
            DataObject.LINE_PCAS = this.LINE_PCAS;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string LINE_NAME
        {
            get
            {
                return (string)this["LINE_NAME"];
            }
            set
            {
                this["LINE_NAME"] = value;
            }
        }
        public string SECTION_ID
        {
            get
            {
                return (string)this["SECTION_ID"];
            }
            set
            {
                this["SECTION_ID"] = value;
            }
        }
        public string LINE_CODE
        {
            get
            {
                return (string)this["LINE_CODE"];
            }
            set
            {
                this["LINE_CODE"] = value;
            }
        }
        public string LINE_PCAS
        {
            get
            {
                return (string)this["LINE_PCAS"];
            }
            set
            {
                this["LINE_PCAS"] = value;
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
    public class C_LINE
    {
        public string ID{get;set;}
        public string LINE_NAME{get;set;}
        public string SECTION_ID{get;set;}
        public string LINE_CODE{get;set;}
        public string LINE_PCAS{get;set;}
        public string DESCRIPTION{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}