using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesReportCenter.Class
{
    public class DBQuery
    {
        public List<DBQueryTable> UseTables = new List<DBQueryTable>();
        public List<DBQueryPara> Paras = new List<DBQueryPara>();
        public List<JoinType> Joins = new List<JoinType>();

    }

    public class JoinType
    {
        public string Table1, Table2;
        public string Type = "Inner Join ";
        public  List<JoinData> Data = new List<JoinData>();

    }
    public class JoinData
    {
        public  string Col1;
        public  string Col2;
        public  string Operator;
    }

    public class DBQueryPara
    {
        public string Column
        {
            get; set;
        }
        public string Alias
        {
            get; set;
        }
        public string Table
        {
            get; set;
        }
        public bool Output
        {
            get; set;
        }
        public string SortType
        {
            get; set;
        }
        public int? SortOrder
        {
            get; set;
        }
        public string GroupBy
        {
            get; set;
        }
        public string Filter
        {
            get; set;
        }
    }

    public class DBQueryTable
    {
        public string Alias;
        public DBTable Table;
    }
}
