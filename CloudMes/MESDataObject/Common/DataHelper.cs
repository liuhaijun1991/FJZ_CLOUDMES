using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Common
{
    public static class DataHelper
    {
        /// <summary>
        /// 反射实现两个类的对象之间相同属性的值的复制
        /// 适用于初始化新实体
        /// </summary>
        /// <typeparam name="D">返回的实体</typeparam>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <param name="s">数据源实体</param>
        /// <returns>返回的新实体</returns>
        public static D Mapper<D, S>(S s)
        {
            D d = Activator.CreateInstance<D>(); //构造新实例
            try
            {
                var Types = s.GetType();//获得类型  
                var Typed = typeof(D);
                foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")//判断属性名是否相同  
                        {
                            dp.SetValue(d, sp.GetValue(s, null), null);//获得s对象属性的值复制给d对象的属性  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return d;
        }

        /// <summary>
        /// 反射实现两个类的对象之间相同属性的值的复制
        /// 适用于初始化新实体
        /// </summary>
        /// <typeparam name="D">返回的实体</typeparam>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <param name="s">数据源实体</param>
        /// <returns>返回的新实体</returns>
        public static List<D> Mapper<D, S>(List<S> s)
        {
            List<D> res = new List<D>(); 
            foreach (var item in s)
            {
                D d = Activator.CreateInstance<D>(); //构造新实例
                try
                {
                    var Types = item.GetType();//获得类型  
                    var Typed = typeof(D);
                    foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段  
                    {
                        foreach (PropertyInfo dp in Typed.GetProperties())
                        {
                            if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")//判断属性名是否相同  
                            {
                                dp.SetValue(d, sp.GetValue(item, null), null);//获得s对象属性的值复制给d对象的属性  
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                res.Add(d);
            }          
            return res;
        }

        public static DataTable FormatTableSingleRowViewData<T>(T model, int colnum)
        {
            DataTable dt = new DataTable();
            for (int i = 1; i <= colnum; i++)
            {
                dt.Columns.Add("Data" + i);
            }
            Type t = model.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            int rownum = (int)Math.Ceiling((double)PropertyList.Count() / (double)dt.Columns.Count);
            for (int i = 0; i < rownum; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns.Count * i + j <= PropertyList.Count() - 1)
                        dr[dt.Columns[j]] = PropertyList[dt.Columns.Count * i + j].Name;
                }
                dt.Rows.Add(dr);

                DataRow drv = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    try
                    {
                        PropertyList[dt.Columns.Count * i + j].GetValue(model, null);
                        drv[dt.Columns[j]] = PropertyList[dt.Columns.Count * i + j].GetValue(model, null) != null
                            ? PropertyList[dt.Columns.Count * i + j].GetValue(model, null).ToString()
                            : "";
                    }
                    catch
                    {
                        continue;
                    }
                }
                dt.Rows.Add(drv);
            }
            return dt;
        }

        public static DataTable ModelToTable<T>(List<T> models,string tablename = "")
        {
            DataTable dt = new DataTable();
            Type t = models.FirstOrDefault().GetType();
            if (tablename == "")
                dt.TableName = t.Name;
            else
                dt.TableName = tablename;
            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (var item in PropertyList)            
                dt.Columns.Add(item.Name);            

            foreach (var item in models)
            {
                var dr = dt.NewRow();
                foreach (var p in PropertyList)
                {
                    dr[p.Name] =p.GetValue(item, null);
                }
                dt.Rows.Add(dr);
            }            
            return dt;
        }

        /// <summary>
        /// 匿名类的转换方式
        /// </summary>
        /// <param name="GenericType"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IList ConverDatatableToList(Type GenericType, DataTable dataTable)
        {
            Type typeMaster = typeof(List<>);
            Type listType = typeMaster.MakeGenericType(GenericType);
            IList list = Activator.CreateInstance(listType) as IList;
            if (dataTable == null || dataTable.Rows.Count == 0)
                return list;
            var constructor = GenericType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                           .OrderBy(c => c.GetParameters().Length).First();
            var parameters = constructor.GetParameters();
            var values = new object[parameters.Length];
            foreach (DataRow dr in dataTable.Rows)
            {
                int index = 0;
                foreach (ParameterInfo item in parameters)
                {
                    object itemValue = null;
                    if (dr[item.Name] != null && dr[item.Name] != DBNull.Value)
                    {
                        itemValue = Convert.ChangeType(dr[item.Name], item.ParameterType.UnderlyingSystemType);
                    }
                    values[index++] = itemValue;
                }
                list.Add(constructor.Invoke(values));
            }
            return list;
        }

        /// <summary>  
        /// 利用反射将DataTable转换为List<T>对象
        /// </summary>  
        /// <param name="dt">DataTable 对象</param>  
        /// <returns>List<T>集合</returns>  
        public static List<T> FromTable<T>(DataTable dt) where T : class, new()
        {
            // 定义集合  
            List<T> ts = new List<T>();
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                                       //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        //取值  
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value != DBNull.Value && !string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            if (pi.PropertyType == typeof(DateTime?))
                            {
                                DateTime tt;
                                if(DateTime.TryParse(value.ToString(), out tt))
                                {
                                    pi.SetValue(t, tt, null);
                                }
                                
                            }
                            else if (pi.PropertyType == typeof(double?))
                            {
                                double dd;
                                if(double.TryParse(value.ToString(), out dd))
                                {
                                    pi.SetValue(t, dd, null);
                                }                                
                            }
                            else
                            {
                                pi.SetValue(t, value, null);
                            }
                        }
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }
            return ts;
        }

    }

    public static class Extensions
    {
        //public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        //{
        //    return listToClone.Select(item => (T)item.Clone()).ToList();
        //}
        public static T Clone<T>(T RealObject)
        {
            using (Stream objStream = new MemoryStream())
            {
                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(objStream, RealObject);
                objStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objStream);
            }
        }

    }
}
