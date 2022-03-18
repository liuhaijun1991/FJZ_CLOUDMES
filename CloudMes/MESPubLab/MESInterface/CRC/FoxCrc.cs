using MESDataObject.Module;
using MESPubLab.MesBase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESInterface.CRC
{
    public class FoxCrc : I_Crc
    {
        string apitesturl = "http://10.156.217.143:8040/api/CRCException/NewExceptionRequest";
        string apiurl = "https://gcrc.efoxconn.com:8023/api/CRCException/NewExceptionRequest";
        public CrcRes Send(CrcObj crcobj)
        {
            var url = apiurl;
            if (url.StartsWith("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(((sender, certificate, chain, sslPolicyErrors) => true));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=utf-8";
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(crcobj, Newtonsoft.Json.Formatting.Indented,
                 new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" }); 
            var bytebody = Encoding.UTF8.GetBytes(body);
            try
            {
                request.ContentLength = bytebody.Length;
                request.GetRequestStream().Write(bytebody, 0, bytebody.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码  
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<CrcRes>(retString);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
    }
}
