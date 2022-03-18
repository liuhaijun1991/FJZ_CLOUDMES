using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.Common
{
    public class common
    {
        public static IList DataTableToList(Type GenericType, DataTable dataTable)
        {
            Type typeMaster = typeof(List<>);
            Type listType = typeMaster.MakeGenericType(GenericType);
            IList list = Activator.CreateInstance(listType) as IList;
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return list;
            }
            var parameters = GenericType.GetProperties();
            foreach (DataRow dr in dataTable.Rows)
            {
                var obj = Activator.CreateInstance(GenericType);
                foreach (PropertyInfo item in parameters)
                {
                    object itemValue = null;
                    if (dr[item.Name] != null && dr[item.Name] != DBNull.Value)
                    {
                        itemValue = Convert.ChangeType(dr[item.Name], item.PropertyType);
                    }
                    item.SetValue(obj, itemValue);
                }
                list.Add(obj);
            }
            return list;
        }

        public static List<T> DataTableToList<T>(DataTable dataTable)
        {
            List<T> list = new List<T>();
            Type t = typeof(T);
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return list;
            }
            var parameters = typeof(T).GetProperties();
            foreach (DataRow dr in dataTable.Rows)
            {
                var obj = (T)Activator.CreateInstance(t);
                foreach (PropertyInfo item in parameters)
                {
                    object itemValue = null;
                    if (dr[item.Name] != null)
                    {
                        itemValue = Convert.ChangeType(dr[item.Name], item.PropertyType);
                    }
                    item.SetValue(obj, itemValue);
                }
                list.Add(obj);
            }
            return list;
        }
    }
    
}
