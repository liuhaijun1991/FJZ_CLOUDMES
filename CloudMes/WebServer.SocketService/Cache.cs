using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDataObject;
using System.Reflection;
using MESDBHelper;
using System.Data;

namespace WebServer.SocketService
{
    /// <summary>
    /// 由張官軍創建，僅供玩耍
    /// </summary>
    public class Cache 
    {
        private static double DEFAULT_EXPIRED_DAYS=0.5;
        public List<CacheItem> Caches { get; set; }

        private OleExecPool DBPool;

        public Cache(OleExecPool DBPool)
        {
            this.DBPool = DBPool;
        }

        public List<object> QueryFromCache(object Condition,string Operation,Type type)
        {
            ClearCache();
            List<object> objs = new List<object>();
            foreach (CacheItem item in Caches)
            {
                foreach (FieldInfo field in item.Object.GetType().GetFields())
                {
                    if (field.FieldType == typeof(DateTime))
                    {

                    }
                    if (field.GetValue(item.Object).ToString().Equals(Condition))
                    {

                    }
                }
                //if (item.ID.Equals(ObjectID))
                //{
                //    item.LastAccessDate = DateTime.Now;
                //    if (Operation.ToUpper().Equals("UPDATE"))
                //    {
                //        item.Modified = true;
                //    }
                //    return item.Object;
                //}
            }
            //object obj = QueryFromDB(ObjectID,type);
            //AddCacheItem(obj, ObjectID);
            return objs;
        }

        /// <summary>
        /// 清除過期及被修改過的緩存
        /// </summary>
        private  void ClearCache()
        {
            foreach (CacheItem item in Caches)
            {
                if ((item.LastAccessDate != null && 
                    DateTime.Now.Subtract(item.LastAccessDate.AddDays(DEFAULT_EXPIRED_DAYS)).Seconds > 0) ||
                    item.Modified)
                {
                    Caches.Remove(item);
                }
            }
        }

        /// <summary>
        /// 將 obj 加入到緩存中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private  CacheItem AddCacheItem(object obj,string ID)
        {
            CacheItem item = new CacheItem();
            item.LastAccessDate = DateTime.Now;
            item.Object = obj;
            item.ID = ID;
            item.Modified = false;
            Caches.Add(item);
            return item;
        }

        /// <summary>
        /// 如果緩存中沒有或者已標識為修改的話就從數據庫取
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private  object QueryFromDB(string ID,Type type)
        {
            OleExec DB = this.DBPool.Borrow();
            string sql = string.Empty;
            object obj = null;

            sql = $@"SELECT * FROM {type.GetType().Name} WHERE ID='" + ID + "' AND ROWNUM=1";
            try
            {
                DataTable dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    obj = ConstructObject(dt.Rows[0], type);
                }
            }
            catch
            {
            }
            finally
            {
                DBPool.Return(DB);
            }
            return obj;
        }

        /// <summary>
        /// 根據數據庫中的一行構建實例
        /// 但僅僅填充實例中的普通字段，對於引用字段需要另外創建並賦值
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private object ConstructObject(DataRow dr,Type type)
        {
            object obj = Assembly.Load("MESDataObject").CreateInstance(type.FullName);
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (dr.Table.Columns.Contains(field.Name))
                {
                    if (dr[field.Name].GetType() == typeof(System.DBNull))
                    {
                        field.SetValue(obj, null);
                    }

                    else
                    {
                        if (field.FieldType == typeof(DateTime))
                        {
                            field.SetValue(obj, Convert.ToDateTime(dr[field.Name]));
                        }
                        else
                        {
                            field.SetValue(obj, dr[field.Name]);
                        }
                    }
                }
            }
            return obj;
        }



    }

    public class CacheItem
    {
        public string ID { get; set; }
        public object Object { get; set; }
        public bool Modified { get; set; }
        public DateTime LastAccessDate { get; set; }
    }
}
