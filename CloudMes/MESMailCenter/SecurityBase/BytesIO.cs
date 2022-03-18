using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SecurityBase
{
    /// <summary>
    /// 提供基礎Byte數據的一些處理
    /// </summary>
    public class BytesIO
    {
        static ToBase64Transform ToBase64 = new ToBase64Transform();
        static FromBase64Transform FromBase64 = new FromBase64Transform();
        /// <summary>
        /// 將byte數組轉換成Base64
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase64String(byte[] bytes)
        {
            MemoryStream m = new MemoryStream(bytes);
            m.Position = 0;
            CryptoStream cs = new CryptoStream(m, ToBase64, CryptoStreamMode.Read);
            StreamReader r = new StreamReader(cs);
            return r.ReadToEnd();
        }
        /// <summary>
        /// 將Bas64編碼的字符串轉換成byte數組
        /// </summary>
        /// <param name="Base64String"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(string Base64String)
        {
            MemoryStream m = new MemoryStream();
            CryptoStream cs = new CryptoStream(m, FromBase64, CryptoStreamMode.Write);
            StreamWriter w = new StreamWriter(cs);
            w.Write(Base64String);
            w.Flush();
            cs.FlushFinalBlock();
            m.Position = 0;
            byte[] bytes = new byte[m.Length];
            m.Read(bytes, 0, (int)m.Length);
            return bytes;
        }
    }
}
