using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MESMailCenter
{
    public class OLEDBExePool:ObjectPool
    {
        string _StrConn;
        /// <summary>
        /// 構造一個OLEDB連接池
        /// </summary>
        /// <param name="strConntion">連接字符串</param>
        public OLEDBExePool(string strConntion)
        {
            _StrConn = strConntion;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override object NewItem()
        {
            OleExec o = new OleExec(_StrConn);
            return o;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool DelItem(object value)
        {
            OleExec o = (OleExec)value;
            o.FreeMe();
            return true;
        }
    }
}
