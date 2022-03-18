using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;


namespace MESMailCenter
{
    public class OleExec
    {
        static object syncRoot = true;
        string _connStr;
        OleDbConnection _conn;
        OleDbCommand _comm;
        OleDbDataAdapter _adp;
        OleDbTransaction _Train;
        public bool ThrowSqlExeception = false;
        public int CMD_TIME_OUT = 30;
        /// <summary>
        /// 構造一個OleExec對象,該對象用於執行SQL語句等任務
        /// </summary>
        /// <param name="strConn"></param>
        public OleExec(string strConn)
        {
            lock(syncRoot)
            {
                _connStr = strConn;
                _conn = new OleDbConnection(_connStr); //new SqlConnection(_connStr);
                _conn.Open();
                _comm = new OleDbCommand();
                _adp = new OleDbDataAdapter(_comm);
            }
        }
        /// <summary>
        ///  構造一個OleExec對象,該對象用於執行SQL語句等任務        ///  
        /// </summary>
        /// <param name="DBConntionConfigName"></param>
        /// <param name="IsReadConfig">傳True從XML文件讀取,傳False從appconfig讀取</param>
        public OleExec(string DBConntionConfigName, bool IsReadConfig)
        {
            lock(syncRoot)
            {
                if (IsReadConfig)
                {
                    ConnectionManager.Init();
                    _connStr = ConnectionManager.GetConnString(DBConntionConfigName);
                    //_connStr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.120.176.115)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=PAPERLES)));Persist Security Info=True;User ID=WEB;Password=WEB;Unicode=True;Provider=OraOleDb.Oracle";                
                }
                else
                {
                    _connStr = System.Configuration.ConfigurationSettings.AppSettings[DBConntionConfigName];
                }
                _conn = new OleDbConnection(_connStr);
                _conn.Open();
                _comm = new OleDbCommand();
                _adp = new OleDbDataAdapter(_comm);
            }
        }
        /// <summary>
        /// 執行SQL語句
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public string ExecSQL(string strSQL)
        {
            
            checkConn();
            _comm.CommandText=strSQL;// = new OleDbCommand(strSQL, _conn);
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            try
            {
                _comm.Transaction = _Train;
                int ret = _comm.ExecuteNonQuery();
                //_comm.Dispose();
                return ret.ToString();
            }
            catch (Exception e)
            {
                if (ThrowSqlExeception)
                {
                    throw e;
                }
                // _comm.Dispose();
                return "執行SQL異常\r\nstrSQL:\"" + strSQL + "\"\r\n異常信息:" + e.Message;
            }

        }

        /// <summary>
        /// 執行一條插入語句未調試,請不要使用該方法
        /// 請注意傳入的語句只插入1條記錄的
        /// 只有SQLServer數據庫支持該方法
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>自動ID</returns>
        public object ExecInsertSQL(string strSQL)
        {
            checkConn();
            string strSelectID = " select @@identity as ID";
            strSQL += strSelectID;
            _comm.CommandText = strSQL;//= new OleDbCommand(strSQL, _conn);
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            object res = Int32.Parse(_comm.ExecuteScalar().ToString());
            return res;
        }

        public object RunSelectOneValue(string strSQL)
        {
            _comm.CommandText = strSQL;//= new OleDbCommand(strSQL, _conn);
            //_adp = new OleDbDataAdapter(_comm);
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            //DataSet res = new DataSet();
            //_adp.Fill(res);
            return _comm.ExecuteScalar();
        }

        /// <summary>
        /// 执行Oracle语句，并返回第一行第一列结果,wuq/by20140626
        /// </summary>
        /// <param name="strSql">Oracle语句</param>
        /// <returns></returns>
        public string RunSqlReturn(string strSql)
        {
            string strReturn = "";
            checkConn();
            _comm.CommandText = strSql;//= new OleDbCommand(strSQL, _conn);
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            strReturn = _comm.ExecuteScalar().ToString();
            return strReturn;
        }

        /// <summary>
        /// 執行select語句,返回數據集
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public DataSet RunSelect(string strSQL)
        {
            checkConn();
            
            try
            {
                return _RunSelect(strSQL);
            }
            catch(Exception ee)
            {
                checkConn();
                return _RunSelect(strSQL);
            }
            
            
        }
        private DataSet _RunSelect(string strSQL)
        {
            _comm.CommandText = strSQL;//= new OleDbCommand(strSQL, _conn);
            //_adp = new OleDbDataAdapter(_comm);
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.CommandTimeout = CMD_TIME_OUT;
            _adp.SelectCommand = _comm;
            DataSet res = new DataSet();
            _adp.Fill(res);
            return res;
        }
        /// <summary>
        /// 執行select語句,返回數據集
        /// </summary>
        /// <param name="strSql">SQL語句</param>
        /// <param name="Paras">參數數組</param>
        /// <returns></returns>
        public DataSet RunSelect(string strSql, OleDbParameter[] Paras)
        {
            checkConn();
            _comm.CommandText = strSql;//= new OleDbCommand(strSQL, _conn);
            //_adp = new OleDbDataAdapter(_comm);
            _comm.CommandType = CommandType.Text;
            _comm.Connection = _conn;
            _comm.CommandTimeout = CMD_TIME_OUT;
            foreach (OleDbParameter para in Paras)
            {
                _comm.Parameters.Add(para);
            }
            _adp = new OleDbDataAdapter(_comm);
            DataSet res = new DataSet();
            _adp.Fill(res);
            return res;

        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Paras"></param>
        public DataSet RunStoredprocedure(string SPName, OleDbParameter[] Paras)
        {
            checkConn();
            _comm.CommandText = SPName; //= new OleDbCommand(SPName, _conn);
            _comm.CommandType = CommandType.StoredProcedure;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.Parameters.Clear();
            _comm.CommandTimeout = CMD_TIME_OUT;
            foreach (OleDbParameter para in Paras)
            {
                _comm.Parameters.Add(para);
            }
            _adp = new OleDbDataAdapter(_comm);
            DataSet res = new DataSet();
            _adp.Fill(res);
            return res;

        }
        /// <summary>
        /// 執行SQL過程,返回影響行數
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="Paras"></param>
        public int RunSqlNoReturn(string strSql, OleDbParameter[] Paras)
        {
            checkConn();
            _comm.CommandText = strSql; //= new OleDbCommand(strSql, _conn);
            _comm.CommandType = CommandType.Text;
            _comm.Transaction = _Train;
            _comm.Connection = _conn;
            _comm.CommandTimeout = CMD_TIME_OUT;
            foreach (OleDbParameter para in Paras)
            {
                _comm.Parameters.Add(para);
            }
            return _comm.ExecuteNonQuery();

        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Paras"></param>
        public string RunStoredprocedureNoReturn(string SPName, OleDbParameter[] Paras)
        {
            checkConn();

            _comm.CommandText = SPName;// = new OleDbCommand(SPName, _conn);
            _comm.CommandType = CommandType.StoredProcedure;
            _comm.Connection = _conn;
            _comm.Transaction = _Train;
            _comm.Parameters.Clear();
            _comm.CommandTimeout = CMD_TIME_OUT;
            foreach (OleDbParameter para in Paras)
            {
                _comm.Parameters.Add(para);
            }
            _comm.ExecuteNonQuery();
            string result = _comm.Parameters["VAR_O_MESSAGE"].Value.ToString();
            return result;

        }
        /// <summary>
        /// 開啟事務
        /// </summary>
        public void BeginTrain()
        {
            if (_Train == null)
            {
                _Train = this._conn.BeginTransaction();
                _comm.Transaction = _Train;
            }
            
        }
        /// <summary>
        /// 提交事務
        /// </summary>
        public void CommitTrain()
        {
            if (_Train != null)
            {
               
                _Train.Commit();
                //_comm.Transaction.Commit();
                _Train = null;
                _comm.Transaction = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void RollbackTrain()
        {
            if (_Train != null)
            {
                _Train.Rollback();
                _Train = null;
                _comm.Transaction = null;
            }
        }

        /// <summary>
        /// 用於檢查連接是否正常.
        /// </summary>
        private void checkConn()
        {
            if (_Train == null)
            {
                _conn.ResetState();
                if (_conn.State != ConnectionState.Open)
                {
                    try
                    {
                        _conn.Close();

                    }
                    catch
                    { }
                    _conn = new OleDbConnection(this._connStr);
                    _conn.Open();
                }
            }
            else
            {
                //_conn.Open();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        ~OleExec()
        {
            try
            {
                _conn.Close();
            }
            catch
            { }
        }
        /// <summary>
        /// 
        /// </summary>
        public void FreeMe()
        {
            try
            {
                _Train.Rollback();
            }
            catch
            { }
            try
            {
                this._comm.Dispose();
            }
            catch
            { }
            try
            {
                this._conn.Dispose();
            }
            catch
            { }
            try
            {
                this._adp.Dispose();
            }
            catch
            { }
            
        }

        /// <summary>
        /// test
        /// </summary>
        public void CloseMe()
        {
            try
            {
                _Train.Rollback();
            }
            catch
            { }
            try
            {
                this._comm.Dispose();
            }
            catch
            { }
            try
            {
                this._conn.Dispose();
            }
            catch
            { }
            try
            {
                this._conn.Close();
            }
            catch
            { }
            try
            {
                this._adp.Dispose();
            }
            catch
            { }
            //ConnectionState ss = _conn.State;
            //_conn = null;
            //_comm = null;
        }

    }
}

