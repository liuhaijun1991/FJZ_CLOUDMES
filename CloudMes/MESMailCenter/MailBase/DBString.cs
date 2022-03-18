using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MESMailCenter
{
    /// <summary>
    /// 該類對字符串進行一些處理
    /// </summary>
    public class DBString
    {
        /// <summary>
        /// 轉換字符串中的單引號
        /// </summary>
        /// <param name="strValue">轉換前的字符串</param>
        /// <returns>轉換后的字符串</returns>
        public static string Parse(string strValue)
        {
            return strValue = strValue.Replace("'", "''");
        }
    }
}
