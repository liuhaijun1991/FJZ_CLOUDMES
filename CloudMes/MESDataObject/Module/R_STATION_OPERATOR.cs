using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class T_R_STATION_OPERATOR : DataObjectTable
    {
        public T_R_STATION_OPERATOR(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_STATION_OPERATOR(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_STATION_OPERATOR);
            TableName = "R_STATION_OPERATOR".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_STATION_OPERATOR CheckLine(string _Line, OleExec DB)
        {
            return DB.ORM.Queryable<R_STATION_OPERATOR>().Where(t => t.LINE == _Line).ToList().FirstOrDefault();
        }


        /// <summary>
        /// 獲取新的默認路由名稱
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public string GetLine(OleExec DB)
        //{
        //    string strSql = $@"select 'ALL' line FROM DUAL UNION ALL select distinct line from R_STATION_OPERATOR ORDER BY LINE";
        //    OleDbParameter[] paramet = new OleDbParameter[0];
        //    string res = DB.ExecuteScalar(strSql, CommandType.Text, paramet);
        //    if (res == null || res.Length <= 0)
        //    {
        //        res = "NewRoute0";
        //    }
        //    return res;
        //}

        public List<R_STATION_OPERATOR> GetLine(OleExec DB)
        {
            List<R_STATION_OPERATOR> ret = new List<R_STATION_OPERATOR>();
            string strSql = $@"select 'ALL' LINE FROM DUAL UNION ALL select distinct LINE from R_STATION_OPERATOR ORDER BY LINE";
            DataSet res = DB.RunSelect(strSql);

            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_STATION_OPERATOR R = (Row_R_STATION_OPERATOR)NewRow();
                R.loadData(res.Tables[0].Rows[i]);
                ret.Add(R.GetDataObject());
            }
            return ret;
        }

        public R_STATION_OPERATOR CheckStation(string _Line, string _Station, OleExec DB)
        {
            return DB.ORM.Queryable<R_STATION_OPERATOR>().Where(t => t.LINE == _Line && t.STATION== _Station).ToList().FirstOrDefault();
        }
    }
    public class Row_R_STATION_OPERATOR : DataObjectBase
    {
        public Row_R_STATION_OPERATOR(DataObjectInfo info) : base(info)
        {

        }
        public R_STATION_OPERATOR GetDataObject()
        {
            R_STATION_OPERATOR DataObject = new R_STATION_OPERATOR();
            DataObject.ONLINEDT = this.ONLINEDT;
            DataObject.OPERATORNAME = this.OPERATORNAME;
            DataObject.OPERATOR = this.OPERATOR;
            DataObject.ISENGINEER = this.ISENGINEER;
            DataObject.ISTEST = this.ISTEST;
            DataObject.STATION = this.STATION;
            DataObject.LINE = this.LINE;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public DateTime? ONLINEDT
        {
            get
            {
                return (DateTime?)this["ONLINEDT"];
            }
            set
            {
                this["ONLINEDT"] = value;
            }
        }
        public string OPERATORNAME
        {
            get
            {
                return (string)this["OPERATORNAME"];
            }
            set
            {
                this["OPERATORNAME"] = value;
            }
        }
        public string OPERATOR
        {
            get
            {
                return (string)this["OPERATOR"];
            }
            set
            {
                this["OPERATOR"] = value;
            }
        }
        public string ISENGINEER
        {
            get
            {
                return (string)this["ISENGINEER"];
            }
            set
            {
                this["ISENGINEER"] = value;
            }
        }
        public string ISTEST
        {
            get
            {
                return (string)this["ISTEST"];
            }
            set
            {
                this["ISTEST"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
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
    }
    public class R_STATION_OPERATOR
    {
        public DateTime? ONLINEDT { get; set; }
        public string OPERATORNAME { get; set; }
        public string OPERATOR { get; set; }
        public string ISENGINEER { get; set; }
        public string ISTEST { get; set; }
        public string STATION { get; set; }
        public string LINE { get; set; }
        public string ID { get; set; }
    }
}
