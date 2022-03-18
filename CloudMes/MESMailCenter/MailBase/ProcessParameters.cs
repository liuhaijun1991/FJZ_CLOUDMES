using System;
using System.Collections.Generic;
using System.Text;

namespace MESMailCenter
{
    /// <summary>
    /// 向Process傳遞的參數
    /// </summary>
    public class ProcessParameters
    {
        /// <summary>
        /// 包含參數的數據
        /// </summary>
        protected Dictionary<string,object> _values = new Dictionary<string,object>();
        /// <summary>
        /// 向ProcessParameters添加參數
        /// </summary>
        /// <param name="key">參數名</param>
        /// <param name="value">參數值</param>
        public void Add(string key, object value)
        {
            _values.Add(key, value);
        }
        /// <summary>
        /// 獲取參數值
        /// </summary>
        /// <param name="key">參數名</param>
        /// <returns>參數值</returns>
        public object Get(string key)
        {
            return _values[key];
            //throw new System.NotImplementedException();
        }
        /// <summary>
        /// 直接初始化參數
        /// </summary>
        /// <param name="keys">參數名稱數組</param>
        /// <param name="values">參數值數組</param>
        public ProcessParameters(string[] keys, object[] values)
        {
            if (keys.Length != values.Length)
            {
                throw new Exception("keys.Length != values.Length,PLS Check!");
            }
            for (int i = 0; i < keys.Length; i++)
            {
                _values.Add(keys[i], values[i]);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ProcessParameters()
        { }
        /// <summary>
        /// 以XML的形式返回ProcessParameters
        /// </summary>
        /// <returns>參數的XML形式</returns>
        public override string ToString()
        {
            StringBuilder strRet=new StringBuilder();
            strRet.Append("<ProcessParameters>");
            string[] keys=new string[_values.Keys.Count];
            _values.Keys.CopyTo(keys,0);
            for (int i = 0; i < keys.Length; i++)
            {
                strRet.Append("<" + keys[i] + ">");
                strRet.Append(_values[keys[i]].ToString());
                strRet.Append("</" + keys[i] + ">");

            }
            strRet.Append("</ProcessParameters>");
            return strRet.ToString();
            
        }
        /// <summary>
        /// 獲取包含的KEY值數組
        /// </summary>
        public string[] Keys
        {
            get 
            {
                string[] keys = new string[_values.Keys.Count];
                _values.Keys.CopyTo(keys, 0);
                return keys;
            }
        }
    }
}
