using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;
using System.Reflection;
using System.Data.OleDb;

namespace MESDataObject
{
    public class DictionaryItem
    {
        public object Data;
        public DateTime CreateTime;
    }
    /// <summary>
    /// 映射到數據庫中的行，包括表的名稱，表中所有的列（列名，列中值類型，值的長度和值是否可以為空）
    /// Mapping the table of database,include its table name,all columns in one row(the details of column like column name,data type,data length and so on).
    /// Edit by 張官軍 20171124
    /// </summary>
    public class DataObjectInfo
    {
        public string TableName = "";
        public List<DataObjectColInfo> BaseColsInfo = new List<DataObjectColInfo>();
    }

    /// <summary>
    /// 映射到數據庫中的列，包括列的名稱、值類型、值的長度和是否可以為空
    /// Mapping the column of the table,include table name,data type,data length and can or cannot to be null
    /// Edit by 張官軍 20171124
    /// </summary>
    public class DataObjectColInfo
    {
        public string name;
        public Type DataType;
        public double length;
        public bool nullable;
    }

    /// <summary>
    /// 映射到表的詳細信息，包括表名，擴展表名，表的具體內容以及表中的所有行
    /// Mapping the object to table,include table name,extend table name,table details and all rows in table.
    /// Edit by 張官軍 20171124
    /// </summary>
    public class DataObjectTable
    {
        //靜態字典,緩存表結構
        static Dictionary<string, DictionaryItem> TableConfigs = new Dictionary<string, DictionaryItem>();
         public static DataObjectInfo GetDataObjectInfo(string TableName, OleExec DB, DB_TYPE_ENUM DBType)
        {
            //先從靜態緩存中檢索,命中則返回,否則加載新的
            string strTableName = TableName.ToUpper();
            string strSql = "";
            DataObjectInfo info = null;
            if (TableConfigs.ContainsKey(strTableName))
            {
                info = (DataObjectInfo)TableConfigs[strTableName].Data;
                return info;
            }
            info = new DataObjectInfo();

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                //查詢Table元數據
                strSql = 
 $@"SELECT 
       T1.OWNER,
       T1.TABLE_NAME,
       T1.COLUMN_NAME,
       T1.NULLABLE,
       T1.DATA_TYPE ,
       T1.DATA_LENGTH ,
       T2.COMMENTS
  FROM all_tab_cols T1, all_col_comments T2
 WHERE T1.TABLE_NAME = T2.TABLE_NAME
   AND T1.COLUMN_NAME = T2.COLUMN_NAME
   AND T1.TABLE_NAME = '{strTableName}'";

                DataSet res = DB.ExecSelect(strSql);
                
                for (int i = 0; i < res.Tables[0].Rows.Count;i++)
                {
                    DataObjectColInfo CI = new DataObjectColInfo();
                    CI.name = res.Tables[0].Rows[i]["COLUMN_NAME"].ToString();
                    switch (res.Tables[0].Rows[i]["DATA_TYPE"].ToString())
                    {
                        case "VARCHAR2":
                        case "NVARCHAR2":
                        case "CHAR":
                        case "CLOB":
                            CI.DataType = typeof(string);
                            break;
                        case "DATE":
                            CI.DataType = typeof(DateTime);
                            break;
                        case "NUMBER":
                        case "FLOAT":
                            CI.DataType = typeof(double);
                            break;
                        case "LONG":
                            CI.DataType = typeof(long);
                            break;
                        case "BLOB":
                            CI.DataType = typeof(object);
                            break;
                        default:
                            throw new Exception($@"Table:{strTableName} Col: {CI.name} dataType:{res.Tables[0].Rows[i]["DATA_TYPE"].ToString()} Not supported");
                    }
                    CI.length = double.Parse( res.Tables[0].Rows[i]["DATA_LENGTH"].ToString());
                    CI.nullable = res.Tables[0].Rows[i]["NULLABLE"].ToString() == "Y" ? true : false;
                    info.BaseColsInfo.Add(CI);
                }
                DictionaryItem item = new DictionaryItem();
                info.TableName = strTableName;
                item.Data = info;
                if (!TableConfigs.ContainsKey(strTableName))
                {
                    TableConfigs.Add(strTableName, item);
                }
                return info;
            }
            else
            {
                throw new Exception("Database type:"+DBType.ToString()+ " Not supported");
            }
            
        }

        public DB_TYPE_ENUM DBType = DB_TYPE_ENUM.Oracle;
        public string TableName;
        public string Sequence_BU;
        public string Sequence_Type;
        public string ExtenTableName;
        protected DataObjectInfo DataInfo = null;
        public List<DataObjectBase> Rows = new List<DataObjectBase>();

        public Type RowType = null;

        public void Add(DataObjectBase NewRow)
        {

        }

        //張官軍加入，根據實體對象構建 Row 對象
        public DataObjectBase ConstructRow(object obj)
        {
            DataObjectBase row = NewRow();
            FieldInfo[] Fields = obj.GetType().GetFields();
            PropertyInfo[] Properties = obj.GetType().GetProperties();
            if (Fields.Length > 0)
            {
                foreach (FieldInfo Field in Fields)
                {
                    if (Field.GetValue(obj) != null)
                    {
                        row[Field.Name] = Field.GetValue(obj);
                    }
                }
            }
            else
            {
                foreach (PropertyInfo Property in Properties)
                {
                    if (Property.GetValue(obj) != null)
                    {
                        row[Property.Name] = Property.GetValue(obj);
                    }
                }
            }
            return row;
        }

        public DateTime GetDBDateTime(OleExec SFCDB)
        {
            string strSql = "select sysdate from dual";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = "select sysdate from dual";
            }
            else if (DBType == DB_TYPE_ENUM.SqlServer)
            {
                strSql = "select get_date() ";
            }
            else
            {
                throw new Exception(DBType.ToString() + " not Work");
            }
            DateTime DBTime = (DateTime)SFCDB.ExecSelectOneValue(strSql);
            return DBTime;
        }

        /// <summary>
        /// 獲取當前數據庫時間所屬的班別
        /// </summary>
        /// <param name="DateTime"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetWorkClass(OleExec DB)
        {
            string TimeFormat = "HH24:MI:SS";
            DataTable dt = new DataTable();
            string sql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM C_WORK_CLASS WHERE TO_DATE(TO_CHAR(SYSDATE,'{TimeFormat}'),'{TimeFormat}')
                            BETWEEN TO_DATE(START_TIME,'{TimeFormat}') AND TO_DATE(END_TIME,'{TimeFormat}')";

                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["NAME"] != null)
                    {
                        return dt.Rows[0]["NAME"].ToString();
                    }
                    else
                    {
                        throw new Exception("The name of work shift cannot be empty");
                    }
                }
                else
                {
                    //如果上面的沒有結果，表示某一條數據的 END_TIME 是第二天的時間，那麼那一條的 START_TIME 肯定是所有數據中最大的
                    sql = "SELECT * FROM C_WORK_CLASS ORDER BY START_TIME DESC";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0]["NAME"].ToString();
                    }
                    else
                    {
                        throw new Exception("No shifts configured");
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        public DataObjectBase NewRow()
        {
            if (RowType == null)
            {

                DataObjectBase NewRow = new DataObjectBase(DataInfo);
                return NewRow;
            }
            else
            {
                Assembly assembly = Assembly.Load("MESDataObject");
                object API_CLASS = assembly.CreateInstance(RowType.FullName,true,BindingFlags.CreateInstance,null, new object[] { DataInfo },null,null);
                return (DataObjectBase)API_CLASS;
            }
        }

        public DataObjectBase GetObjByID(string ID, OleExec DB)
        {
            return GetObjByID(ID, DB, DBType);
        }

        public DataObjectBase GetObjByID(string ID , OleExec DB , DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from {TableName} where ID = '{ID}'";
            DataSet res = DB.ExecSelect(strSql);
            if (RowType == null)
            {
                DataObjectBase ret = NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                Assembly assembly = Assembly.Load("MESDataObject");
                object API_CLASS = assembly.CreateInstance(RowType.FullName, true, BindingFlags.CreateInstance, null, new object[] { DataInfo }, null, null);
                MethodInfo Function = RowType.GetMethod("loadData");
                Function.Invoke(API_CLASS, new object[] { res.Tables[0].Rows[0] });
                return (DataObjectBase)API_CLASS;
            }
        }
        public DataObjectBase GetObjBySelect(string strSQL, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = strSQL;
            DataSet res = DB.ExecSelect(strSql);
            if (RowType == null)
            {
                DataObjectBase ret = NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                Assembly assembly = Assembly.Load("MESDataObject");
                object API_CLASS = assembly.CreateInstance(RowType.FullName, true, BindingFlags.CreateInstance, null, new object[] { DataInfo }, null, null);
                MethodInfo Function = RowType.GetMethod("loadData");
                Function.Invoke(API_CLASS, new object[] { res.Tables[0].Rows[0] });
                return (DataObjectBase)API_CLASS;
            }
        }

        public string GetNewID(string BU ,OleExec DB, DB_TYPE_ENUM DBType)
        {
            //string strSql = "exec sfc.get_id('KKI','C_sku',OUT_RES); ";
            //string ret = "";
            OleDbParameter[] para = new OleDbParameter[]
            {
                new OleDbParameter(":IN_BU",OleDbType.VarChar,300),
                new OleDbParameter(":IN_TYPE",OleDbType.VarChar,300),
                new OleDbParameter(":OUT_RES",OleDbType.VarChar,500)
            };
            para[0].Value = BU;
            para[1].Value = TableName;
            para[2].Direction = ParameterDirection.Output;
            DB.ExecProcedureNoReturn("SFC.GET_ID", para);
            string newID = para[2].Value.ToString();
            if (newID.StartsWith("ERR"))
            {
                throw new Exception("Get table '"+TableName+"' ID exception! "+newID);
            }
            return newID;
        }

        public string GetNewID(string BU, OleExec DB)
        {
            return GetNewID(BU, DB, DBType);
        } 

        public DataObjectTable(string _TableName, OleExec DB, DB_TYPE_ENUM DBType)
        {
            DataInfo = GetDataObjectInfo(_TableName, DB, DBType);
            TableName = _TableName;
        }
        public DataObjectTable()
        {
            
        }

        /// <summary>
        /// DataTable数据集转换为ModelList add by Eden 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertToEx<T>(DataTable dt) where T : new()
        {
            if (dt == null) return null;
            if (dt.Rows.Count <= 0) return null;

            List<T> list = new List<T>();
            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();  //获取泛型的属性
            List<DataColumn> listColumns = dt.Columns.Cast<DataColumn>().ToList();  //获取数据集的表头，以便于匹配
            T t;
            foreach (DataRow dr in dt.Rows)
            {
                t = new T();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    try
                    {
                        DataColumn dColumn = listColumns.Find(name => name.ToString().ToUpper() == propertyInfo.Name.ToUpper());  //查看是否存在对应的列名
                        if (dColumn != null)
                            propertyInfo.SetValue(t, dr[propertyInfo.Name], null);  //赋值
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                list.Add(t);
            }
            return list;
        }

    }


}
