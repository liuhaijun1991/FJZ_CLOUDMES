using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.OleDb;

namespace MESMailCenter
{
    /// <summary>
    /// 用於執行SQL語句等任務
    /// </summary>
    public class SQLExec
    {
        string _connStr;
        SqlConnection _conn;
        SqlCommand _comm;
        SqlDataAdapter _adp;
        /// <summary>
        /// 構造一個SQLExec對象,該對象用於執行SQL語句等任務
        /// </summary>
        /// <param name="strConn"></param>
        public SQLExec(string strConn)
        {
            _connStr = strConn;
            _conn = new SqlConnection(_connStr);
            _conn.Open();
        }
        /// <summary>
        ///  構造一個SQLExec對象,該對象用於執行SQL語句等任務
        /// </summary>
        /// <param name="DBConntionConfigName"></param>
        /// <param name="IsReadConfig"></param>
        public SQLExec(string DBConntionConfigName, bool IsReadConfig)
        {
            ConnectionManager.Init();
            _connStr = ConnectionManager.GetConnString(DBConntionConfigName);
            _conn = new SqlConnection(_connStr);
            _conn.Open();
            
        }
        /// <summary>
        /// 執行SQL語句
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public string ExecSQL(string strSQL)
        {
            //_conn.BeginTransaction();
            _comm = new SqlCommand(strSQL, _conn);
            try
            {
                int ret = _comm.ExecuteNonQuery();
                _comm.Dispose();
                return ret.ToString();
            }
            catch (Exception e)
            {
                _comm.Dispose();
                return "執行SQL異常\r\nstrSQL:\"" + strSQL + "\"\r\n異常信息:" + e.Message;
            }
   
        }
        /// <summary>
        /// 執行一條插入語句,返回ID
        /// 請注意傳入的語句只插入1條記錄的
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>自動ID</returns>
        public int ExecInsertSQL(string strSQL)
        {
            string strSelectID = " select @@identity as ID";
            strSQL += strSelectID;
            _comm = new SqlCommand(strSQL, _conn);
            int res = Int32.Parse( _comm.ExecuteScalar().ToString());
            return res;
        }
        /// <summary>
        /// 執行select語句,返回數據集
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public DataSet RunSelect(string strSQL)
        {
            
            _comm = new SqlCommand(strSQL);
            _comm = new SqlCommand(strSQL,_conn);
            _adp = new SqlDataAdapter(_comm);
            DataSet res = new DataSet();
            
            _adp.Fill(res);
            return res;
            
        }
        /// <summary>
        /// 
        /// </summary>
        ~SQLExec()
        {
            try
            {
                _conn.Close();
            }
            catch
            { }
        }

    }
}
