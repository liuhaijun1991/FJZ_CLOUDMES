using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using Newtonsoft.Json.Linq;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_NORETEST : DataObjectTable
    {
        public T_C_NORETEST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_NORETEST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_NORETEST);
            TableName = "C_NORETEST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool GetBySN(string Sn, string Skuno, OleExec DB)
        {
            bool res = false;
            string strSQL = $@"select * from C_NORETEST where sn='{Sn}'
                            union
                            select * from C_NORETEST where sn ='{Skuno}' and ROWNUM=1";
            DataTable Dt = DB.ExecSelect(strSQL).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                res = true;
            }
            //int Num = DB.ExecSqlNoReturn(strSQL, null);
            //if (Num > 0)
            //{
            //    res = true;
            //}
            return res;
        }
        public List<C_NORETEST> GetAllBySNs(List<Newtonsoft.Json.Linq.JToken> SNs, OleExec DB)
        {
            string SN = "";
            //int flag = 0;
            string strSql = "";
            List<C_NORETEST> result = new List<C_NORETEST>();
            if (SNs.Count > 0)
            {
                foreach (Newtonsoft.Json.Linq.JToken item in SNs)
                {
                    SN = item.ToString();
                    strSql = $@"SELECT *FROM c_noretest where SN=:SN";
                    OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
                    DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
                    if (res.Rows.Count > 0)
                    {
                        for (int i = 0; i < res.Rows.Count; i++)
                        {
                            Row_C_NORETEST ret = (Row_C_NORETEST)NewRow();
                            ret.loadData(res.Rows[i]);
                            result.Add(ret.GetDataObject());
                        }
                    }
                }
                return result;
            }
            else
            {
                strSql = "SELECT *FROM c_noretest WHERE ROWNUM<1000 order by EDIT_TIME DESC";
                DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
                if (res.Rows.Count > 0)
                {
                    for (int i = 0; i < res.Rows.Count; i++)
                    {
                        Row_C_NORETEST ret = (Row_C_NORETEST)NewRow();
                        ret.loadData(res.Rows[i]);
                        result.Add(ret.GetDataObject());
                    }
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
        public List<C_NORETEST> SelectBySn(string SN, string TEST_STATION, OleExec DB)
        {
            //return DB.ORM.Queryable<C_NORETEST>().ToList();
            string strSql = $@"SELECT *FROM c_noretest where SN=:SN and TEST_STATION='{TEST_STATION}'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            List<C_NORETEST> result = new List<C_NORETEST>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_C_NORETEST ret = (Row_C_NORETEST)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 添加新的信息
        /// </summary>
        /// <param name="newSn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddNewSN(C_NORETEST newSn, OleExec DB)
        {
            Row_C_NORETEST NewNoReTest = (Row_C_NORETEST)NewRow();
            NewNoReTest.ID = newSn.ID;
            NewNoReTest.SN = newSn.SN;
            NewNoReTest.TEST_STATION = newSn.TEST_STATION;
            NewNoReTest.EDIT_TIME = newSn.EDIT_TIME;
            NewNoReTest.EDIT_EMP = newSn.EDIT_EMP;
            int result = DB.ExecuteNonQuery(NewNoReTest.GetInsertString(DBType), CommandType.Text);
            return result;
        }

    }
    public class Row_C_NORETEST : DataObjectBase
    {
        public Row_C_NORETEST(DataObjectInfo info) : base(info)
        {

        }
        public C_NORETEST GetDataObject()
        {
            C_NORETEST DataObject = new C_NORETEST();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.TEST_STATION = this.TEST_STATION;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string TEST_STATION
        {
            get
            {
                return (string)this["TEST_STATION"];
            }
            set
            {
                this["TEST_STATION"] = value;
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
    public class C_NORETEST
    {
        public string ID;
        public string SN;
        public string TEST_STATION;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}