using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESInterface.Email
{
    public class FoxLHEmailWebService : I_Email
    {
        HttpWebRequest SendRequest;
        HttpWebRequest UploadRequest;
        string failInf;
        FoxLHEmailWebServiceConfig config;
        public string GetFailInf()
        {
            throw new NotImplementedException();
        }

        public bool Init(string config)
        {
            FoxLHEmailWebServiceConfig Config = null;
            if (config == null)
            {
                Config = new FoxLHEmailWebServiceConfig();
            }
            else
            {
                try
                {
                    Config = Newtonsoft.Json.JsonConvert.DeserializeObject<FoxLHEmailWebServiceConfig>(config);
                }
                catch (Exception ee)
                {
                    failInf = $@"不正确的配置: {config}";
                    return false;
                }
            }
            SendRequest = (HttpWebRequest)WebRequest.Create(Config.SendMailURL);
            SendRequest.Method = "POST";
            SendRequest.ContentType = "application/soap+xml; charset=utf-8";

            UploadRequest = (HttpWebRequest)WebRequest.Create(Config.FileUploadURL);
            UploadRequest.Method = "POST";
            UploadRequest.ContentType = "multipart/form-data";
            UploadRequest.KeepAlive = true;
            return true;
        }

        public bool Send(EmailObj mail)
        {
            var AttachmentKeys = mail.Attachments.Keys.ToArray();
            string files = "";
            if (AttachmentKeys.Length > 0)
            {
                for (int i = 0; i < AttachmentKeys.Length; i++)
                {
                    FormItemModel formItem = new FormItemModel() { Key = "uplodfiles", FileName = AttachmentKeys[i], FileContent = mail.Attachments[AttachmentKeys[i]] };
                    FileUpload(formItem);
                    files += AttachmentKeys[i] + ",";
                }
                //sendtxt.isAttachments = "Y";
            }
            var sendtxt = new
            {
                mailFrom = mail.mailFrom,
                mailTo = mail.mailTo,
                mailCC = mail.mailCC,
                mailSubject = mail.mailSubject,
                mailBody = mail.mailBody,
                mailPriority = mail.mailPriority,
                IsBodyHtml = mail.IsBodyHtml,
                isAttachments = AttachmentKeys.Length > 0 ? "Y":"N",
                fileName = files
            };

            var body = Newtonsoft.Json.JsonConvert.SerializeObject(sendtxt, Newtonsoft.Json.Formatting.Indented,
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });


            SendRequest.ContentLength = body.Length;
            StreamWriter writer = new StreamWriter(SendRequest.GetRequestStream(), Encoding.UTF8);
            writer.Write(body);
            writer.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)SendRequest.GetResponse();
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
        }

        void FileUpload(FormItemModel formItem)
        {
            HttpWebRequest request = UploadRequest;
            #region 初始化请求对象
            request.Method = "POST";
            //request.Timeout = timeOut;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            #endregion
            string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            //请求流
            var postStream = new MemoryStream();
            var formUploadFile = true;
            //文件数据模板
            string fileFormdataTemplate =
                "\r\n--" + boundary +
                "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                "\r\nContent-Type: application/octet-stream" +
                "\r\n\r\n";
            //文本数据模板
            string dataFormdataTemplate =
                "\r\n--" + boundary +
                "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                "\r\n\r\n{1}";

            string formdata = null;
            if (formItem.IsFile)
            {
                //上传文件
                formdata = string.Format(
                    fileFormdataTemplate,
                    formItem.Key, //表单键
                    formItem.FileName);
            }
            else
            {
                //上传文本
                formdata = string.Format(
                    dataFormdataTemplate,
                    formItem.Key,
                    formItem.Value);
            }

            //统一处理
            byte[] formdataBytes = null;
            //第一行不需要换行
            if (postStream.Length == 0)
                formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
            else
                formdataBytes = Encoding.UTF8.GetBytes(formdata);
            postStream.Write(formdataBytes, 0, formdataBytes.Length);

            //写入文件内容
            if (formItem.FileContent != null && formItem.FileContent.Length > 0)
            {
                using (var stream = formItem.FileContent)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        postStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            postStream.Write(footer, 0, footer.Length);

            request.ContentLength = postStream.Length;
            #region 输入二进制流
            if (postStream != null)
            {
                postStream.Position = 0;
                //直接写入流
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                ////debug
                //postStream.Seek(0, SeekOrigin.Begin);
                //StreamReader sr = new StreamReader(postStream);
                //var postStr = sr.ReadToEnd();
                postStream.Close();//关闭文件访问
            }
            #endregion
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream,  Encoding.UTF8))
                {
                    string retString = myStreamReader.ReadToEnd();
                    //return retString;
                }
            }
        }






        public class FoxLHEmailWebServiceConfig
        {
            public string SendMailURL = $@"http://10.156.216.69/api/Smtp/SmtpService";
            public string FileUploadURL = $@"http://10.156.216.69/api/Smtp/UploadFiles";

            public string ToJsonData()
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented,
                     new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                return json;
            }
        }

        public class FormItemModel
        {
            /// <summary>
            /// 表单键，request["key"]
            /// </summary>
            public string Key { set; get; }
            /// <summary>
            /// 表单值,上传文件时忽略，request["key"].value
            /// </summary>
            public string Value { set; get; }
            /// <summary>
            /// 是否是文件
            /// </summary>
            public bool IsFile
            {
                get
                {
                    if (FileContent == null || FileContent.Length == 0)
                        return false;

                    if (FileContent != null && FileContent.Length > 0 && string.IsNullOrWhiteSpace(FileName))
                        throw new Exception("上传文件时 FileName 属性值不能为空");
                    return true;
                }
            }
            /// <summary>
            /// 上传的文件名
            /// </summary>
            public string FileName { set; get; }
            /// <summary>
            /// 上传的文件内容
            /// </summary>
            public Stream FileContent { set; get; }
        }
    }
}
