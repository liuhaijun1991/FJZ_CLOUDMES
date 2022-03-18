using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using SqlSugar;
using System.Reflection;
using DbType = SqlSugar.DbType;

namespace MESDBHelper
{
    public class OleExec
    {
        public string _connStr;
        //OleDbConnection _conn;
        //OleDbCommand _comm;
        //OleDbDataAdapter _adp;
        //OleDbTransaction _Train;
        bool _TrainStart = false;
        SqlSugarClient db;
        public bool ThrowSqlExeception = false;
        public int CMD_TIME_OUT = 30;
        public OleExecPool Pool;
        public OleExecPoolItem PoolItem;
        public bool NoUseAgain = false;

        public int PoolBorrowTimeOut
        {
            get
            {
                if (PoolItem != null)
                {
                    return PoolItem.BorrowTimeOut;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (PoolItem != null)
                {
                    PoolItem.BorrowTimeOut = value;
                }
            }
        }

        /// <summary>
        /// 将datatable转换成对应的对象数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<T> CovertToTypeList<T>(DataTable data)
        {
            List<T> ret = new List<T>();
            var type = typeof(T);
            var Properties = type.GetProperties();
            Assembly assembly = Assembly.GetAssembly(type);
            Dictionary<string, PropertyInfo> Props = new Dictionary<string, PropertyInfo>();
            for (int i = 0; i < Properties.Length; i++)
            {
                Props.Add(Properties[i].Name.ToUpper(), Properties[i]);
            }
            for (int i = 0; i < data.Rows.Count; i++)
            {
                var I = assembly.CreateInstance(type.Name);
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    string Colname = data.Columns[j].ColumnName;
                    try
                    {
                        var Propertie = Props[Colname.ToUpper()];
                        Propertie.SetValue(I, data.Rows[i][Colname]);
                    }
                    catch
                    { }
                }
                ret.Add((T)I);
            }
            return ret;
        }
        /// <summary>
        /// 将datatable转换成对应的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T CovertToType<T>(DataTable data)
        {
            List<T> ret = new List<T>();
            var type = typeof(T);
            var Properties = type.GetProperties();
            Assembly assembly = Assembly.GetAssembly(type);
            Dictionary<string, PropertyInfo> Props = new Dictionary<string, PropertyInfo>();
            for (int i = 0; i < Properties.Length; i++)
            {
                Props.Add(Properties[i].Name.ToUpper(), Properties[i]);
            }
            //for (int i = 0; i < data.Rows.Count; i++)
            for (int i = 0; i < data.Rows.Count;)
            {
                var I = assembly.CreateInstance(type.Name);
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    string Colname = data.Columns[j].ColumnName;
                    try
                    {
                        var Propertie = Props[Colname.ToUpper()];
                        Propertie.SetValue(I, data.Rows[i][Colname]);
                    }
                    catch
                    { }
                }
                ret.Add((T)I);
                break;
            }
            if (ret.Count == 1)
            {
                return ret[0];
            }
            else
            {
                return default(T);
            }
        }

        public SqlSugarClient ORM
        {
            get
            { return db; }
        }
        public OleExec()
        {
        }

        /// <summary>
        /// 返回SqlSugarClient--add by Eden
        /// ReadXMLConfig is True=>read xml file
        /// ReadXMLConfig is False=>read web.config or app.config file
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetSqlSugarClient(string strConn, bool ReadXMLConfig)
        {
            if (ReadXMLConfig)
            {
                ConnectionManager.Init();
                strConn = ConnectionManager.GetConnString(strConn);
            }
            else
            {
                strConn = ConfigurationManager.AppSettings[strConn];
            }
            var sdb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = strConn, //必填
                DbType = SqlSugar.DbType.Oracle, //必填
                IsAutoCloseConnection = false, //默认false
                InitKeyType = InitKeyType.Attribute,
                IsShardSameThread = false, //设为true相同线程便於開事務
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = MesDbEx.MesSqlFuncEx()//set ext method
                }

            }); //默认SystemTable
            return sdb;
        }
        /// <summary>
        /// 返回SqlSugarClient--add by Eden
        /// ReadXMLConfig is True=>read xml file
        /// ReadXMLConfig is False=>read web.config or app.config file
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetSqlSugarClient(string strConn, bool ReadXMLConfig,DbType dbType)
        {
            if (ReadXMLConfig)
            {
                ConnectionManager.Init();
                strConn = ConnectionManager.GetConnString(strConn);
            }
            else
            {
                strConn = ConfigurationManager.AppSettings[strConn];
            }
            var sdb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = strConn, //必填
                DbType = dbType, //必填
                IsAutoCloseConnection = false, //默认false
                InitKeyType = InitKeyType.Attribute,
                IsShardSameThread = false, //设为true相同线程便於開事務
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = MesDbEx.MesSqlFuncEx()//set ext method
                }
            }); //默认SystemTable
            return sdb;
        }

        /// <summary>
        /// 返回SqlSugarClient--add by Eden
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetSqlSugarClient(string strConn,DbType dbType)
        {
            var sdb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = strConn, //必填
                DbType = dbType, //必填
                IsAutoCloseConnection = false, //默认false
                InitKeyType = InitKeyType.Attribute,
                IsShardSameThread = false, //设为true相同线程便於開事務
            }); //默认SystemTable
            return sdb;
        }

        /// <summary>
        /// 返回SqlSugarClient--add by Eden
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetSqlSugarClient(string strConn)
        {
            var sdb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = strConn, //必填
                DbType = SqlSugar.DbType.Oracle, //必填
                IsAutoCloseConnection = false, //默认false
                InitKeyType = InitKeyType.Attribute,
                IsShardSameThread = false, //设为true相同线程便於開事務
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = MesDbEx.MesSqlFuncEx()//set ext method
                }
            }); //默认SystemTable
            return sdb;
        }

        /// <summary>
        /// 構造一個OleExec對象,該對象用於執行SQL語句等任務
        /// </summary>
        /// <param name="strConn"></param>
        public OleExec(string strConn , OleExecPool _Pool)
        {
            _connStr = strConn;
            //_conn = new OleDbConnection(_connStr);
            //_conn.Open();
            //_comm = new OleDbCommand();
            //_adp = new OleDbDataAdapter(_comm);
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connStr,
                DbType = SqlSugar.DbType.Oracle,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute, 
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = MesDbEx.MesSqlFuncEx()//set ext method
                }
            });
            try
            {
                db.Open();
            }
            catch (Exception e)
            {
                throw e;
            }

            Pool = _Pool;
        }

        public OleExec(string strConn)
        {
            _connStr = strConn;
            //_conn = new OleDbConnection(_connStr);
            //_conn.Open();
            //_comm = new OleDbCommand();
            //_adp = new OleDbDataAdapter(_comm);
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connStr,
                DbType = SqlSugar.DbType.Oracle,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = MesDbEx.MesSqlFuncEx()//set ext method
                }
            });
            //db.Ado.Connection.ConnectionTimeout
            
            db.Open();
            
        }

        public OleExec(string strConn , SqlSugar.DbType dbtype)
        {
            _connStr = strConn;
            //_conn = new OleDbConnection(_connStr);
            //_conn.Open();
            //_comm = new OleDbCommand();
            //_adp = new OleDbDataAdapter(_comm);
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connStr,
                DbType = dbtype,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute, 
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = MesDbEx.MesSqlFuncEx()//set ext method
                }
            });
            db.Open();

        }

        /// <summary>
        /// DB連接對象
        /// </summary>
        /// <param name="DBConntionConfigName">連接配置名稱</param>
        /// <param name="ReadXMLConfig">是否從XML配置檔讀取連接字符串</param>
        public OleExec(string DBConntionConfigName, bool ReadXMLConfig)
        {
            if (Pool!=null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            if (ReadXMLConfig)
            {
                ConnectionManager.Init();
                _connStr = ConnectionManager.GetConnString(DBConntionConfigName);
            }
            else
            {
                _connStr = ConfigurationManager.AppSettings[DBConntionConfigName];
            }
            //_conn = new OleDbConnection(_connStr);
            //_conn.Open();
            //_comm = new OleDbCommand();
            //_adp = new OleDbDataAdapter(_comm);
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connStr,
                DbType = SqlSugar.DbType.Oracle,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute, 
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    SqlFuncServices = MesDbEx.MesSqlFuncEx()//set ext method
                }
            });
        }

        /// <summary>
        /// 執行SQL語句
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public string ExecSQL(string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            

            //_comm.CommandText = strSQL;
            //_comm.CommandType = CommandType.Text;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_comm.Parameters.Clear();
            try
            {
                //_comm.Transaction = _Train;
                int ret = db.Ado.ExecuteCommand(strSQL, new SugarParameter[] { });
                return ret.ToString();
            }
            catch (Exception e)
            {
                if (ThrowSqlExeception)
                {
                    throw e;
                }
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
            throw new NotImplementedException();
            //if (Pool != null && !Pool.TestBorrow(this))
            //{
            //    throw new Exception("OleExec havn't borrow from DBPool!");
            //}
            //checkConn();
            //string strSelectID = " select @@identity as ID";
            //strSQL += strSelectID;
            //_comm.CommandText = strSQL;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //object res = Int32.Parse(_comm.ExecuteScalar().ToString());
            //return res;
        }

        public object ExecSelectOneValue(string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            //_comm.CommandText = strSQL;
            //_comm.CommandType = CommandType.Text;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_comm.Parameters.Clear();
            return db.Ado.GetScalar(strSQL, new SugarParameter[] { });
            //return _comm.ExecuteScalar();
        }

        /// <summary>
        /// 执行Oracle语句，并返回第一行第一列结果,wuq/by20140626
        /// </summary>
        /// <param name="strSql">Oracle语句</param>
        /// <returns></returns>
        public string ExecSqlReturn(string strSql)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            string strReturn = "";
            checkConn();
            //_comm.CommandText = strSql;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_comm.Parameters.Clear();
            strReturn = db.Ado.GetScalar(strSql, new SugarParameter[] { }).ToString();
            return strReturn;
        }

        /// <summary>
        /// 執行select語句,返回數據集
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        //public DataSet ExecSelect(string strSQL)
        //{
        //    checkConn();
        //    try
        //    {
        //        return _RunSelect(strSQL);
        //    }
        //    catch
        //    {
        //        checkConn();
        //        return _RunSelect(strSQL);
        //    }
        //}
        public DataSet RunSelect(string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            //_comm.CommandText = strSQL;
            //_comm.CommandType = CommandType.Text;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_adp.SelectCommand = _comm;
            //_comm.Parameters.Clear();
            return db.Ado.GetDataSetAll(strSQL, new SugarParameter[] { });
            
        }
        public DataTable ExecSelect_b(int rowcount, int pageindex, string strSQL)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            
            DataTable dt = db.Ado.GetDataTable(strSQL, new SugarParameter[] { });
            int sindex = rowcount * pageindex - rowcount;
            DataTable ret = new DataTable();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn c = new DataColumn(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                ret.Columns.Add(c);
            }

            for (int i = sindex; i < sindex + rowcount; i++)
            {
                try
                {
                    DataRow dr = ret.NewRow();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        dr[dt.Columns[j].ColumnName] = dt.Rows[i][j];
                    }
                    ret.Rows.Add(dr);
                }
                catch
                {
                    break;
                }
            }
            return ret;

        }
        //private DataTable _RunSelect_b(int rowcount, int pageindex, string strSQL)
        //{
        //    if (Pool != null && !Pool.TestBorrow(this))
        //    {
        //        throw new Exception("OleExec havn't borrow from DBPool!");
        //    }
        //    int sindex = rowcount * pageindex - rowcount;
        //    //int eindex = rowcount * pageindex-1;
        //    _comm.CommandText = strSQL;
        //    _comm.CommandType = CommandType.Text;
        //    _comm.Connection = _conn;
        //    _comm.Transaction = _Train;
        //    _comm.CommandTimeout = CMD_TIME_OUT;
        //    _adp.SelectCommand = _comm;
        //    DataTable dt = new DataTable();
        //    _adp.Fill(sindex, rowcount, dt);

        //    return dt;
        //    //_RetReader["ID"].ToString();
        //    //_RetReader.NextResult();

        //}
        /// <summary>
        /// 執行select語句,返回數據集
        /// </summary>
        /// <param name="strSql">SQL語句</param>
        /// <param name="Paras">參數數組,使用 params 聲明表示該參數可以省略</param>
        /// <returns></returns>
        public DataSet ExecSelect(string strSql,params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();

            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
                _para.Add(P);

            }
            db.Ado.CommandType = CommandType.Text;
            var ret = db.Ado.GetDataSetAll(strSql, _para);
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;
            }

            //db.Ado.GetDataSetAll(,)
            //_comm.CommandText = strSql;
            //_comm.CommandType = CommandType.Text;
            //_comm.Connection = _conn;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_comm.Parameters.Clear();
            //if (Paras != null && Paras.Length>0)
            //{
            //    foreach (OleDbParameter para in Paras)
            //    {
            //        _comm.Parameters.Add(para);
            //    }
            //}
            //_adp = new OleDbDataAdapter(_comm);
            //DataSet res = new DataSet();
            //_adp.Fill(res);
            return ret;
        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Paras"></param>
        public DataSet ExecProcedure(string SPName, OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
                _para.Add(P);

            }
            db.Ado.CommandType = CommandType.StoredProcedure;
            var ret = db.Ado.GetDataSetAll(SPName, _para);
            db.Ado.CommandType = CommandType.Text;
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;
            }
            return ret;
            //_comm.CommandText = SPName;
            //_comm.CommandType = CommandType.StoredProcedure;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.Parameters.Clear();
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //if (Paras != null && Paras.Length > 0)
            //{
            //    foreach (OleDbParameter para in Paras)
            //    {
            //        _comm.Parameters.Add(para);
            //    }
            //}
            //_adp = new OleDbDataAdapter(_comm);
            //DataSet res = new DataSet();
            //_adp.Fill(res);
            //return res;
        }
        /// <summary>
        /// 執行SQL過程,返回影響行數
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="Paras"></param>
        public int ExecSqlNoReturn(string strSql, OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
                _para.Add(P);

            }
            db.Ado.CommandType = CommandType.Text;
            var ret = db.Ado.ExecuteCommand(strSql, _para);
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;
            }
            return ret;
            //_comm.CommandText = strSql;
            //_comm.CommandType = CommandType.Text;
            //_comm.Transaction = _Train;
            //_comm.Connection = _conn;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_comm.Parameters.Clear();
            //if (Paras != null && Paras.Length > 0)
            //{
            //    foreach (OleDbParameter para in Paras)
            //    {
            //        _comm.Parameters.Add(para);
            //    }
            //}
            //return _comm.ExecuteNonQuery();
        }

        public IDataReader RunDataReader(string strSql)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            var ret = db.Ado.GetDataReader(strSql, new SugarParameter[] { });
            return ret;
            //_comm.CommandText = strSql;
            //_comm.CommandType = CommandType.Text;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_adp.SelectCommand = _comm;
            //_comm.Parameters.Clear();
            //OleDbDataReader r =_comm.ExecuteReader();
            //return r;
        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Paras"></param>
        public string ExecProcedureNoReturn(string SPName, OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
               
                _para.Add(P);

            }
            db.Ado.CommandType = CommandType.StoredProcedure;
            var ret = db.Ado.GetDataSetAll(SPName, _para);
            db.Ado.CommandType = CommandType.Text;
            string result = "";
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;
                if (Paras[i].Direction == ParameterDirection.Output)
                {
                    result = Paras[i].Value.ToString();
                }
            }
            return result;
            //_comm.CommandText = SPName;
            //_comm.CommandType = CommandType.StoredProcedure;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.Parameters.Clear();
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //if (Paras != null && Paras.Length > 0)
            //{ 
            //    foreach (OleDbParameter para in Paras)
            //    {
            //      _comm.Parameters.Add(para);
            //    }
            //}
            //_comm.ExecuteNonQuery();

            //string result = "";
            //for (int i = 0; i < _comm.Parameters.Count; i++)
            //{
            //    if (_comm.Parameters[i].Direction == ParameterDirection.Output)
            //    {
            //        result = _comm.Parameters[i].Value.ToString();
            //    }
            //}
            //return result;
        }
        /// <summary>
        /// 執行存儲過程
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="Paras"></param>
        public Dictionary<string, object> ExecProcedureReturnDic(string SPName, OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
                _para.Add(P);

            }
            db.Ado.CommandType = CommandType.StoredProcedure;
            var ret = db.Ado.GetDataSetAll(SPName, _para);
            db.Ado.CommandType = CommandType.Text;
            Dictionary<string, object> result = new Dictionary<string, object>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;
                if (Paras[i].Direction == ParameterDirection.Output)
                {
                    result.Add(Paras[i].ParameterName, Paras[i].Value.ToString());
                }
            }
            return result;
            //_comm.CommandText = SPName;
            //_comm.CommandType = CommandType.StoredProcedure;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.Parameters.Clear();
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //if (Paras != null && Paras.Length > 0)
            //{
            //    foreach (OleDbParameter para in Paras)
            //    {
            //        _comm.Parameters.Add(para);
            //    }
            //}
            //_comm.ExecuteNonQuery();

            //Dictionary<string, object> result = new Dictionary<string, object>();
            //for (int i = 0; i < _comm.Parameters.Count; i++)
            //{
            //    if (_comm.Parameters[i].Direction == ParameterDirection.Output)
            //    {
            //        result.Add(_comm.Parameters[i].ParameterName, _comm.Parameters[i].Value.ToString());
            //    }
            //}
            //return result;
        }

        /// <summary>
        /// 执行SQL语句或者存儲過程，并返回第一行第一列结果
        /// </summary>
        /// <param name="strSql">SQL語句或者存儲過程名</param>
        /// <param name="cmmType">CommandType，指明是SQL語句還是存儲過程</param>
        /// <param name="Paras">參數，OleDbParameter數組，可不填</param>
        /// <returns>string</returns>
        public string ExecuteScalar(string strSql, CommandType cmmType, params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            
            checkConn();
            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
                _para.Add(P);

            }
            db.Ado.CommandType = cmmType;
            var ret = db.Ado.GetScalar(strSql, _para);
            db.Ado.CommandType = CommandType.Text;
            
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;
                
            }
            return ret.ToString();

            //_comm.CommandText = strSql;
            //_comm.CommandType = cmmType;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_comm.Parameters.Clear();
            //if (Paras != null && Paras.Length > 0)
            //{
            //    for (int i = 0; i < Paras.Length; i++)
            //    {
            //        if (Paras[i] != null)
            //        {
            //            _comm.Parameters.Add(Paras[i]);
            //        }
            //    }
            //}
            //strReturn = _comm.ExecuteScalar().ToString();
            //return strReturn;
        }

        /// <summary>
        /// 執行SQL語句或存儲過程，返回受影響行數
        /// </summary>
        /// <param name="strSql">SQL語句或者存儲過程名</param>
        /// <param name="cmmType">CommandType，指明是SQL語句還是存儲過程</param>
        /// <param name="Paras">參數，OleDbParameter數組，可不填</param>
        /// <returns>int</returns>
        public int ExecuteNonQuery(string strSql, CommandType cmmType, params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
                _para.Add(P);

            }
            db.Ado.CommandType = cmmType;
            var ret = db.Ado.ExecuteCommand(strSql, _para);
            db.Ado.CommandType = CommandType.Text;
            
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;
               
            }
            return ret;

            //_comm.CommandText = strSql;
            //_comm.CommandType = cmmType;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_comm.Parameters.Clear();
            //if (Paras != null && Paras.Length > 0)
            //{
            //    for (int i = 0; i < Paras.Length; i++)
            //    {
            //        if (Paras[i] != null)
            //        {
            //            _comm.Parameters.Add(Paras[i]);
            //        }
            //    }
            //}
            //intReturn = _comm.ExecuteNonQuery();
            //return intReturn;
        }

        /// <summary>
        /// 執行SQL語句或者存儲過程，返回DataTable
        /// </summary>
        /// <param name="strSql">SQL語句或者存儲過程名</param>
        /// <param name="cmmType">CommandType，指明是SQL語句還是存儲過程</param>
        /// <param name="Paras">參數，OleDbParameter數組，可不填</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable(string strSql, CommandType cmmType, params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
                _para.Add(P);

            }
            db.Ado.CommandType = cmmType;
            var ret = db.Ado.GetDataTable(strSql, _para);
            db.Ado.CommandType = CommandType.Text;

            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;

            }
            return ret;
        }

        /// <summary>
        /// 執行SQL語句或者存儲過程，返回DataSet
        /// </summary>
        /// <param name="strSql">SQL語句或者存儲過程名</param>
        /// <param name="cmmType">CommandType，指明是SQL語句還是存儲過程</param>
        /// <param name="Paras">參數，OleDbParameter數組，可不填</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string strSql, CommandType cmmType, params OleDbParameter[] Paras)
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            checkConn();
            if (Paras == null)
            {
                Paras = new OleDbParameter[] { };
            }
            List<SugarParameter> _para = new List<SugarParameter>();
            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = new SugarParameter(Paras[i].ParameterName, Paras[i].Value);
                P.Size = Paras[i].Size;
                P.Direction = Paras[i].Direction;
                _para.Add(P);

            }
            db.Ado.CommandType = cmmType;
            var ret = db.Ado.GetDataSetAll(strSql, _para);
            db.Ado.CommandType = CommandType.Text;

            for (int i = 0; i < Paras.Length; i++)
            {
                SugarParameter P = _para.Find(T => T.ParameterName == Paras[i].ParameterName);
                Paras[i].Value = P.Value;

            }
            return ret;
            //_comm.CommandText = strSql;
            //_comm.CommandType = cmmType;
            //_comm.Connection = _conn;
            //_comm.Transaction = _Train;
            //_comm.CommandTimeout = CMD_TIME_OUT;
            //_comm.Parameters.Clear();
            //if (Paras != null && Paras.Length > 0)
            //{
            //    for (int i = 0; i < Paras.Length; i++)
            //    {
            //        if (Paras[i] != null)
            //        {
            //            _comm.Parameters.Add(Paras[i]);
            //        }
            //    }
            //}
            //_adp = new OleDbDataAdapter(_comm);
            //DataSet dataset = new DataSet();
            //_adp.Fill(dataset);
            //return dataset;
        }

        /// <summary>
        /// 開啟事務
        /// </summary>
        public void BeginTrain()
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            if (_TrainStart == false)
            {
                db.Ado.BeginTran();
                _TrainStart = true;
            }
        }
        /// <summary>
        /// 提交事務
        /// </summary>
        public void CommitTrain()
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            if (_TrainStart == true)
            {
                db.Ado.CommitTran();
                _TrainStart = false;
                
            }
        }
        /// <summary>
        /// 事務回滾
        /// </summary>
        public void RollbackTrain()
        {
            if (Pool != null && !Pool.TestBorrow(this))
            {
                throw new Exception("OleExec havn't borrow from DBPool!");
            }
            if (_TrainStart == true)
            {
                db.Ado.RollbackTran();
                _TrainStart = false;

            }
        }
        /// <summary>
        /// 用於檢查連接是否正常.
        /// </summary>
        private void checkConn()
        {
            if (_TrainStart == false)
            {
                db.Ado.CheckConnection();
            }
        }
        ~OleExec()
        {
            try
            {
                db.Close();
            }
            catch
            { }
        }
        public void FreeMe()
        {
            try
            {
                db.Ado.RollbackTran();
            }
            catch
            { }
            try
            {
                db.Close();
                db.Dispose();
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
                db.Ado.RollbackTran();
            }
            catch
            { }
            try
            {
                db.Close();
            }
            catch
            { }
        }
    }
}
