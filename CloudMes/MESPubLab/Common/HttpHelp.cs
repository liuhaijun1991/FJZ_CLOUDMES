using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.Common
{
    public class HttpHelp
    {
        public static string HttpPost(string URL, string Para,string contentType)
        {
            // 创建HttpWebRequest对象
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpRequest.Method = "POST";
            httpRequest.ContentType = contentType;
            byte[] bytes = Encoding.UTF8.GetBytes(Para);
            using (Stream reqStream = httpRequest.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
                reqStream.Flush();
            }
            try
            {
                using (HttpWebResponse myResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    string responseString = sr.ReadToEnd();
                    return responseString;
                }
            }
            catch (WebException ex)
            {
                var res = (HttpWebResponse)ex.Response;
                StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                string str = sr.ReadToEnd();
                return str;
            }
        }
    }
}
