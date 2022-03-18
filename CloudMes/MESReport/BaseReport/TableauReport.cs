using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class TableauReport : ReportBase
    {
        public TableauReport()
        {
        }
        public override void Init()
        {
            base.Init();
            string url = "http://10.129.4.103:8000/App_WebForm/WEB_DEMO/HTTP_GETKEY_TEMPLETE.aspx";
            string tiken = "";
            ServicePointManager.Expect100Continue = false;
            ASCIIEncoding enc = new ASCIIEncoding();
            string postData = "Tableau_host=10.157.22.19&Tableau_UID=Tableau_OWER&BI_APPLY_UID=NSD_IT_MES&BI_PWD=ijdfaopujfsdkopjfkopasjfklpas&BI_UID=NSD_IT_MES&TOKEN=kjlkoopujfsdkopjfkopasjfklpdas";
            byte[] data = enc.GetBytes(postData);
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                req.ContentLength = data.Length;
                Stream outStream = req.GetRequestStream();
                outStream.Write(data, 0, data.Length);
                outStream.Close();
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader inStream = new StreamReader(res.GetResponseStream(), enc);
                tiken = inStream.ReadToEnd();
                inStream.Close();
            }
            catch (Exception)
            {
                throw;
            }
            Outputs.Add(tiken);
        }
        public override void Run()
        {
            
        }
    }
}
