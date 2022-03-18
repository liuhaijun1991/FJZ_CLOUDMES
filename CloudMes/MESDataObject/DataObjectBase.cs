using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Reflection;

namespace MESDataObject 
{
    /// <summary>
    ///        
    /// </summary>
    public class DataObjectBase//:IDictionary<string,object>
    {
        [ScriptIgnore]
        bool isAdd = true;
        [ScriptIgnore]
        DataObjectInfo DataInfo = null;
        [ScriptIgnore]
        Dictionary<string, DataObjectItem> BaseValues;// = new Dictionary<string, DataObjectItem>();
        [ScriptIgnore]
        Dictionary<string, DataObjectItem> ExValues;// = new Dictionary<string, DataObjectItem>();
        [ScriptIgnore]
        List<string> _keys = new List<string>();



        public DataObjectBase(DataObjectInfo info)
        {
            DataInfo = info;
            BaseValues = new Dictionary<string, DataObjectItem>();
            for (int i = 0; i < info.BaseColsInfo.Count; i++)
            {
                DataObjectItem item = new DataObjectItem(
                    info.BaseColsInfo[i].name,
                    null, info.BaseColsInfo[i].DataType,
                    info.BaseColsInfo[i].nullable);
                _keys.Add(info.BaseColsInfo[i].name);
                BaseValues.Add(item.Name, item);
            }
        }
        /// <summary>
        /// 修改提交后調用該方法確認修改
        /// 否則再次提交修改將失敗
        /// </summary>
        public void AcceptChange()
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                BaseValues[_keys[i]].OrgValue = BaseValues[_keys[i]].Value;
            }
        }

        public string GetInsertString(DB_TYPE_ENUM DB_type)
        {
            string ret = "";
            if (DB_type == DB_TYPE_ENUM.Oracle)
            {
                // insert into tablename (keys) values ( values )
                ret = $@"INSERT INTO {DataInfo.TableName} ({BaseKeysString()}) VALUES ({BaseValueString()})";

            }
            return ret;
        }

        public string GetUpdateString(DB_TYPE_ENUM DB_type)
        {
            string ret = "";
            if (DB_type == DB_TYPE_ENUM.Oracle)
            {
                // insert into tablename (keys) values ( values )
                ret = $@"UPDATE  {DataInfo.TableName} SET {BaseKeyValueString()} WHERE {BaseKeyOrgValueString()} ";

            }
            return ret;
        }

        public string GetUpdateString(DB_TYPE_ENUM DB_type, string ID)
        {
            string result = string.Empty;
            result = $@"UPDATE {DataInfo.TableName} SET {BaseKeyValueString()} WHERE ID='" + ID + "'";
            return result;
        }

        public string GetDeleteString(DB_TYPE_ENUM DB_type)
        {
            string ret = "";
            if (DB_type == DB_TYPE_ENUM.Oracle)
            {
                // insert into tablename (keys) values ( values )
                ret = $@"DELETE  {DataInfo.TableName}  WHERE {BaseKeyOrgValueString()} ";

            }
            return ret;
        }

        public string GetDeleteString(DB_TYPE_ENUM DB_type, string ID)
        {
            string result = string.Empty;
            if (DB_type == DB_TYPE_ENUM.Oracle)
            {
                result = $@"DELETE {DataInfo.TableName} WHERE ID='" + ID + "'";
            }
            return result;
        }

        public void ConstructRow(object obj)
        {
            FieldInfo[] Fields = obj.GetType().GetFields();
            FieldInfo[] ThisFields = this.GetType().GetFields();
            List<string> ThisFieldNames = new List<string>();
            PropertyInfo[] ThisProperties = this.GetType().GetProperties();
            List<string> ThisPropertyNames = new List<string>();
            PropertyInfo[] Properties = obj.GetType().GetProperties();

            foreach (FieldInfo ThisField in ThisFields)
            {
                ThisFieldNames.Add(ThisField.Name);
            }
            foreach (PropertyInfo ThisProperty in ThisProperties)
            {
                ThisPropertyNames.Add(ThisProperty.Name);
            }

            if (Fields.Length > 0)
            {
                foreach (FieldInfo Field in Fields)
                {
                    if (Field.GetValue(obj) != null && (ThisFieldNames.Contains(Field.Name)||ThisPropertyNames.Contains(Field.Name)))
                    {
                        this[Field.Name] = Field.GetValue(obj);
                    }
                }
            }
            else
            {
                foreach (PropertyInfo Property in Properties)
                {
                    if (Property.GetValue(obj) != null && (ThisFieldNames.Contains(Property.Name) || ThisPropertyNames.Contains(Property.Name)))
                    {
                        this[Property.Name] = Property.GetValue(obj);
                    }
                }
            }

        }


        string BaseKeysString()
        {
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < _keys.Count; i++)
            {
                sb.Append(BaseValues[_keys[i]].Name);
                if (i < _keys.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        string BaseValueString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _keys.Count; i++)
            {
                //sb.Append(BaseValues[_keys[i]].Name);
                //sb.Append("=");
                if (BaseValues[_keys[i]].Value == null)
                {
                    sb.Append("null");
                    if (i < _keys.Count - 1)
                    {
                        sb.Append(",");
                    }
                    continue;
                }

                if (BaseValues[_keys[i]].DataType == typeof(string))
                {
                    sb.Append($@"'{BaseValues[_keys[i]].Value.ToString().Replace("'", "''")}'");
                }
                else if (BaseValues[_keys[i]].DataType == typeof(double) || BaseValues[_keys[i]].DataType == typeof(long))
                {
                    sb.Append($@"{BaseValues[_keys[i]].Value}");
                }
                else if (BaseValues[_keys[i]].DataType == typeof(DateTime))
                {
                    sb.Append($@"to_date('{((DateTime)BaseValues[_keys[i]].Value).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                }
                else if (BaseValues[_keys[i]].DataType == typeof(object))
                {
                    sb.Append($@"'{BaseValues[_keys[i]].Value.ToString().Replace("'", "''")}'");
                }

                    if (i < _keys.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        string BaseKeyValueString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _keys.Count; i++)
            {
                sb.Append(BaseValues[_keys[i]].Name);
                sb.Append("=");

                if (BaseValues[_keys[i]].Value == null)
                {
                    sb.Append(" null ");
                }
                else if (BaseValues[_keys[i]].DataType == typeof(string))
                {
                    sb.Append($@"'{BaseValues[_keys[i]].Value.ToString().Replace("'", "''")}'");
                }
                else if (BaseValues[_keys[i]].DataType == typeof(double) || BaseValues[_keys[i]].DataType == typeof(long))
                {
                    sb.Append($@"{BaseValues[_keys[i]].Value}");
                }
                else if (BaseValues[_keys[i]].DataType == typeof(DateTime))
                {
                    sb.Append($@"to_date('{((DateTime)BaseValues[_keys[i]].Value).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                }

                if (i < _keys.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        string BaseKeyOrgValueString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _keys.Count; i++)
            {
                sb.Append(BaseValues[_keys[i]].Name);
                if (BaseValues[_keys[i]].OrgValue == null)
                {
                    sb.Append(" is null");
                    if (i < _keys.Count - 1)
                    {
                        sb.Append(" and ");
                    }
                    continue;
                }


                sb.Append("=");

                if (BaseValues[_keys[i]].DataType == typeof(string))
                {
                    sb.Append($@"'{BaseValues[_keys[i]].OrgValue.ToString().Replace("'", "''")}'");
                }
                else if (BaseValues[_keys[i]].DataType == typeof(double) || BaseValues[_keys[i]].DataType == typeof(long))
                {
                    sb.Append($@"{BaseValues[_keys[i]].OrgValue}");
                }
                else if (BaseValues[_keys[i]].DataType == typeof(DateTime))
                {
                    sb.Append($@"to_date('{((DateTime)BaseValues[_keys[i]].OrgValue).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                }

                if (i < _keys.Count - 1)
                {
                    sb.Append(" and ");
                }
            }
            return sb.ToString();
        }

        public void loadData(DataRow dr)
        {
            for (int i = 0; i < DataInfo.BaseColsInfo.Count; i++)
            {
                string cname = DataInfo.BaseColsInfo[i].name;
                try
                {
                    
                    if (dr[cname] == System.DBNull.Value)
                    {
                        BaseValues[cname].Value = null;
                        BaseValues[cname].Commit();
                        continue;
                    }
                    Type t = DataInfo.BaseColsInfo[i].DataType;

                    if (t == typeof(string))
                    {
                        BaseValues[cname].Value = dr[cname].ToString();
                    }
                    else if (t == typeof(DateTime))
                    {
                        if (!(dr[cname] is System.DBNull))
                        {
                            BaseValues[cname].Value = (DateTime)dr[cname];
                        }
                    }
                    else if (t == typeof(double))
                    {
                        BaseValues[cname].Value = double.Parse(dr[cname].ToString());
                    }
                    else if (t == typeof(long))
                    {
                        BaseValues[cname].Value = long.Parse(dr[cname].ToString());
                    }
                    else if (t == typeof(object))
                    {
                        BaseValues[cname].Value = dr[cname];
                    }
                }
                catch
                {

                }
                BaseValues[cname].Commit();
            }
        }
        [ScriptIgnore]
        public ICollection<string> Keys
        {
            get
            {
                return _keys;
            }
        }
        [ScriptIgnore]
        public ICollection<object> Values
        {
            get
            { 
                throw new NotImplementedException();
            }
        }
        [ScriptIgnore]
        public int Count
        {
            get
            {
                return DataInfo.BaseColsInfo.Count;
            }
        }
        [ScriptIgnore]
        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        [ScriptIgnore]
        public object this[string key]
        {
            get
            {
                if (BaseValues.ContainsKey(key))
                {
                    return BaseValues[key].Value;
                }
                throw new NotImplementedException();
            }

            set
            {
                if (BaseValues.ContainsKey(key))
                {
                     BaseValues[key].Value = value;
                }
            }
        }

        //public bool ContainsKey(string key)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(string key, object value)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Remove(string key)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool TryGetValue(string key, out object value)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(KeyValuePair<string, object> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Clear()
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Contains(KeyValuePair<string, object> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Remove(KeyValuePair<string, object> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return ((IDictionary)this).GetEnumerator();
        //    //throw new NotImplementedException();
        //}
    }

   

    public enum DB_TYPE_ENUM
    {
        Oracle,
        SqlServer
    }

}
