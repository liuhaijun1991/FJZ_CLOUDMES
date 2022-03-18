using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SecurityBase;

namespace MESMailCenter
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionManager
    {
        /// <summary>
        /// 加密連接字符串的密鈅,如無必要請不要修改
        /// </summary>
        public static string CryptKEY = "JBJ9IyiWR/n4Oj2t82Tx7/GRJeblK3YLtomQrB3mIFo=";
        /// <summary>
        /// 加密算法
        /// </summary>
        public  static string CryptName = "Rijndael";
        static Dictionary<string, object> _ConnStrings = new Dictionary<string, object>();
        /// <summary>
        /// 初始化鏈接管理器
        /// </summary>
        public static void Init()
        { 
            System.Data.DataSet ConnData;
            _ConnStrings.Clear();
            ConnData = new DataSet("ConnData");
            try
            {
                ConnData.ReadXml("DataBase.xml");
            }
            catch
            {
                DataTable dt = ConnData.Tables.Add("TConnString");
                dt.Columns.Add("ConnName");
                dt.Columns.Add("ConnString");
                dt.Columns.Add("IsCrypt");
            }
            foreach (DataRow dr in ConnData.Tables["TConnString"].Rows)
            {
                //_ConnStrings.Add("ConnName", dr["ConnName"].ToString());
                if(Boolean.Parse( dr["IsCrypt"].ToString()))
                {
                    _ConnStrings.Add(dr["ConnName"].ToString(),SecurityBase.CryptMain.Decode(dr["ConnString"].ToString(),ConnectionManager.CryptName,SecurityBase.BytesIO.FromBase64String( ConnectionManager.CryptKEY)));

                }else
                {
                    _ConnStrings.Add(dr["ConnName"].ToString(), dr["ConnString"].ToString());
                }
            }
        }
        /// <summary>
        /// 通過名稱獲取鏈接字符串
        /// </summary>
        /// <param name="ConnName"></param>
        /// <returns></returns>
        public static string GetConnString(string ConnName)
        {
            
            return (string)_ConnStrings[ConnName];
        }
    }
}
