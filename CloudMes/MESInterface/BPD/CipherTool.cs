using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.BPD
{
    public class CipherTool
    {
        /// <summary>
        /// 解碼：
        /// 取密碼的每一位字符，獲取其對應的 ASCII 碼值
        /// 如果值位於 55 - 122 之間那麼就將值減去 22 再轉換成對應ASCII碼表中的字符
        /// 如果值位於 32 - 54 之間，那麼就將值加上 68 再轉換成對應 ASCII 碼表中的字符
        /// 
        /// 編碼：
        /// 取得密碼的每一位字符，獲取對應的 ASCII 碼，例如 m 對應的是 109（0x6D）
        /// 用 109 減去 68 和 加上 22，分別獲得 41 和 131
        /// 將 41 和 131 帶入到解碼規則裡面的兩個條件範圍，可以得到 41 是在範圍內的
        /// 因此 41 就是編碼后的字符的 ASCII 碼，即 0x29 對應的是 )
        /// </summary>
        /// <param name="strpassword"></param>
        /// <returns></returns>
        public static string Decode(string strpassword)
        {
            string LStr = string.Empty;
            byte[] Buffer1 = Encoding.ASCII.GetBytes(strpassword);

            for (int i = 0; i < Buffer1.Length; i++)
            {
                int num2 = Convert.ToInt32(Buffer1[i]);
                if ((num2 > 0x36) && (num2 <= 0x7a))
                {
                    num2 -= 0x16;
                }
                else if ((num2 > 0x20) && (num2 <= 0x36))
                {
                    num2 = (100 + num2) - 0x20;
                }
                byte[] Buffer2 = new byte[1] { Convert.ToByte(num2) };
                string text2 = Encoding.ASCII.GetString(Buffer2);
                LStr = LStr + text2;
            }
            return LStr;
        }

        public static string Encode(string strPassword)
        {
            string result = string.Empty;
            byte[] buffer1 = Encoding.ASCII.GetBytes(strPassword);

            foreach (byte b in buffer1)
            {
                int num = Convert.ToInt32(b);
                num += 22;

                if (num >= 0x20 && num <= 0x7a)
                {
                    result += Encoding.ASCII.GetString(new byte[1] { Convert.ToByte(num) });
                    continue;
                }
                num -= 90;
                if (num >= 0x20 && num <= 0x7a)
                {
                    result += Encoding.ASCII.GetString(new byte[1] { Convert.ToByte(num) });
                    continue;
                }

            }

            return result;
        }
    }
}
