using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SecurityBase
{
    /// <summary>
    /// 加密主程序類
    /// </summary>
    public class CryptMain
    {
        /// <summary>
        /// 執行數據加密,將文本轉換為密文
        /// </summary>
        /// <param name="Value">要加密的文本</param>
        /// <param name="Crypt">加密管理器</param>
        /// <param name="Key">密鈅</param>
        /// <returns></returns>
        public static byte[] Encode(string Value, SymmetricAlgorithm Crypt,byte[] Key)
        {
            Crypt.Key = Key;
            Crypt.IV = new byte[Crypt.IV.Length];
            MemoryStream s = new MemoryStream();
            CryptoStream cs =  new CryptoStream(s, Crypt.CreateEncryptor(), CryptoStreamMode.Write);
            StreamWriter w = new StreamWriter(cs);
            w.Write(Value);
            w.Flush();
            cs.FlushFinalBlock();
            byte[] data = new byte[s.Length];
            s.Position = 0;
            s.Read(data, 0, (int)s.Length);
            return data;
        }
        /// <summary>
        /// 執行數據加密,將文本轉換為密文
        /// </summary>
        /// <param name="Value">要加密的文本</param>
        /// <param name="EncoderName">"SHA","SHA1","MD5","SHA256","SHA-256","SHA384","SHA-384","SHA512","SHA-512","RSA","DSA","DES","3DES"
        /// "TripleDES","Triple DES","RC2","Rijndael"</param>
        /// <param name="Key">密鈅</param>
        /// <returns></returns>
        public static byte[] Encode(string Value, string EncoderName, byte[] Key)
        {
            return Encode(Value, GetEncode(EncoderName), Key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base64CryptString">待解碼數據</param>
        /// <param name="DecoderName">解碼器名稱"SHA","SHA1","MD5","SHA256","SHA-256","SHA384","SHA-384","SHA512","SHA-512","RSA","DSA","DES","3DES"
        /// "TripleDES","Triple DES","RC2","Rijndael"</param>
        /// <param name="Key">解密密鈅</param>
        /// <returns></returns>
        public static string Decode(string Base64CryptString, string DecoderName, byte[] Key)
        {
            byte[] data = BytesIO.FromBase64String(Base64CryptString);
            return Decode(data, GetEncode(DecoderName), Key);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">待解碼數據</param>
        /// <param name="Crypt">解碼器</param>
        /// <param name="Key">解密密鈅</param>
        /// <returns></returns>
        public static string Decode(byte[] data, SymmetricAlgorithm Crypt, byte[] Key)
        {
            Crypt.IV = new byte[Crypt.IV.Length];
            Crypt.Key = Key;
            MemoryStream m = new MemoryStream(data);
            CryptoStream cs = new CryptoStream(m, Crypt.CreateDecryptor(), CryptoStreamMode.Read);
            StreamReader r = new StreamReader(cs);
            return r.ReadToEnd();
        }


        /// <summary>
        /// 通過名稱簡寫來獲取加密管理器
        /// </summary>
        /// <param name="CryptName">"SHA","SHA1","MD5","SHA256","SHA-256","SHA384","SHA-384","SHA512","SHA-512","RSA","DSA","DES","3DES"
        /// "TripleDES","Triple DES","RC2","Rijndael"</param>
        /// <returns></returns>
        public static SymmetricAlgorithm GetEncode(string CryptName)
        {
            return SymmetricAlgorithm.Create(CryptName);
        }
    }
}
