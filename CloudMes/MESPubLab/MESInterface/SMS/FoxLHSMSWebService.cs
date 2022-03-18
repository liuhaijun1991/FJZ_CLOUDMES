using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESInterface.SMS
{
    public class FoxLHSMSWebService : I_SMS
    {
        HttpWebRequest request;
        string failInf;
        FoxLHSMSWebServiceConfig Config = null;

        public string GetFailInf()
        {
            return failInf;
        }

        public bool Init(string config)
        {
            
            if (config == null)
            {
                Config = new FoxLHSMSWebServiceConfig();
            } else
            {
                try
                {
                    Config = Newtonsoft.Json.JsonConvert.DeserializeObject<FoxLHSMSWebServiceConfig>(config);
                }
                catch (Exception ee)
                {
                    failInf = $@"不正确的配置: {config}";
                    return false;
                }
            }

            return true;
        }

        public bool Send(string PhoneNo, string Message)
        {
            request = (HttpWebRequest)WebRequest.Create(Config.URL);
            request.Method = "POST";
            request.Accept = "application/soap+xml,*/*";
            request.ContentType = "application/soap+xml; charset=utf-8";
            string body =
                $@"<?xml version='1.0' encoding='utf-8'?>
  <soap12:Envelope xmlns:xsi = 'http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd = 'http://www.w3.org/2001/XMLSchema' xmlns:soap12 = 'http://www.w3.org/2003/05/soap-envelope' >
           <soap12:Body>
              <SendSMSClass xmlns = 'http://tempuri.org/WebComponent/PubClass' >
                 <phone>{PhoneNo}</phone>
                 <msg>{Message}</msg>
                  </SendSMSClass>
                </soap12:Body>
               </soap12:Envelope> ";
//            body = $@"<?xml version='1.0' encoding='utf-8'?>
//<soap12:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap12='http://www.w3.org/2003/05/soap-envelope'>
//  <soap12:Body>
//    <SendSMSClass xmlns='http://tempuri.org/'>
//      <phone>string</phone>
//      <msg>string</msg>
//    </SendSMSClass>
//  </soap12:Body>
//</soap12:Envelope>";
             //request.ContentLength = request.GetRequestStream().Length;
             var bytebody = Encoding.UTF8.GetBytes(body);
            request.ContentLength = bytebody.Length;
            request.GetRequestStream().Write(bytebody, 0, bytebody.Length);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码  
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                if (retString.Contains("<SendSMSClassResult>true</SendSMSClassResult>"))
                {
                    return true;
                }
                else
                {
                    failInf = retString;
                    return false;
                }
            }
            catch (Exception ee)
            {
                failInf = ee.Message;
                return false;
            }
            finally
            {
                //writer.Close();
            }

        }
    }
    public class FoxLHSMSWebServiceConfig
    {
        public string URL = $@"http://10.132.45.64/webcomponent/pubclass.asmx";
        //public string URL = $@"http://localhost:57817/WebService1.asmx";
        //public string URL = "http://localhost:57817/WebForm1.aspx";

        public string ToJsonData()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented,
                 new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            return json;
        }
    }
}
