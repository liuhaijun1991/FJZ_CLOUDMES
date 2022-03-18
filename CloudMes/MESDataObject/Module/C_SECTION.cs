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
    public class T_C_SECTION : DataObjectTable
    {
        public T_C_SECTION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SECTION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SECTION);
            TableName = "C_SECTION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_SECTION_DETAIL> ShowAllData(OleExec DB)
        {
            List<C_SECTION_DETAIL> list = new List<C_SECTION_DETAIL>();
            string sql = $@"select * from C_section";
            DataTable dt = new DataTable();
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    C_SECTION_DETAIL ep = new C_SECTION_DETAIL();
                    ep = RowToSection(item);
                    list.Add(ep);
                }
            }
            return list;
        }
        public C_SECTION_DETAIL GetById(OleExec DB,string Id)
        {
            C_SECTION_DETAIL result = new C_SECTION_DETAIL();
            string sql = $@"select * from C_section where id=:id";
            DataTable dt = new DataTable();
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id",Id)
            };
            dt = DB.ExecuteDataTable(sql,CommandType.Text,paramet);
            if (dt.Rows.Count > 0)
            {
                result = RowToSection(dt.Rows[0]);
            }
            else
            {
                result = null;
            }
            return result;
        }

        public List<C_SECTION_DETAIL> GetSectionT(string sec,OleExec DB)
        {
            string sql;
            List<C_SECTION_DETAIL> list = new List<C_SECTION_DETAIL>();
            if (string.IsNullOrEmpty(sec))
            {
                sql = $@"select * from C_section order by id";
            }
            else
            {
                sql = $@"select * from C_section  where section_name='{sec}' order by id";
            }
            DataTable dt = new DataTable();
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    C_SECTION_DETAIL ep = new C_SECTION_DETAIL();
                    ep = RowToSection(item);
                    list.Add(ep);
                }
            }
            return list;
        }

        public List<C_SECTION_DETAIL> GetDataByColumn(string column, string data, OleExec DB)
        {
            List<C_SECTION_DETAIL> list = new List<C_SECTION_DETAIL>();
            string sql = "";
            if (string.IsNullOrEmpty(column))
            {
                sql = $@"select * from C_section where section_name =:data";
            }
            else
            {
                sql = $@"select * from C_section where {column} =:data";
            }
            DataSet res = DB.ExecSelect(sql, new System.Data.OleDb.OleDbParameter[1] { new System.Data.OleDb.OleDbParameter("data", data) });
            DataTable dt = res.Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    C_SECTION_DETAIL ep = new C_SECTION_DETAIL();
                    ep = RowToSection(item);
                    list.Add(ep);
                }
            }
            return list;
        }

        private C_SECTION_DETAIL RowToSection(DataRow item)
        {
            C_SECTION_DETAIL ep = new C_SECTION_DETAIL();
            ep.ID = item["ID"].ToString();
            ep.Section_Name = item["Section_Name"].ToString();
            ep.Description = item["Description"].ToString();
            return ep;
        }
    }
    public class Row_C_SECTION : DataObjectBase
    {
        public Row_C_SECTION(DataObjectInfo info) : base(info)
        {

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
        public string SECTION_NAME
        {
            get
            {
                return (string)this["SECTION_NAME"];
            }
            set
            {
                this["SECTION_NAME"] = value;
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
    }

    public class C_SECTION_DETAIL
    {
        public string ID { get; set; }
        public string Section_Name { get; set; }
        public string Description { get; set; }
    }
}

