using MESDataObject.Module;
using MESDBHelper;
using MESInterface.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MESInterface.ORACLE
{
    public class GetTDMSData : taskBase
    {
        public string DB = "";

        public string sourceURL = "";
        public string sourceUser = "";
        public string sourcePWD = "";

        public bool IsRuning = false;
        OleExec SFCDB = null;

        public StringCollection fileCollection;
        public FileHelp FileLog = null;
        public override void init()
        {
            FileLog = new FileHelp();
            try
            {
                FileLog.LogPath += "\\" + ConfigGet("NAME");
            }
            catch (Exception ex)
            {
                FileLog.LogPath += "\\Base";
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);
            }
            try
            {
                DB = ConfigGet("DB");
                sourceURL = ConfigGet("SOURCE_URL");
                sourceUser = ConfigGet("SOURCE_USER");
                sourcePWD = ConfigGet("SOURCE_PWD");

                SFCDB = new OleExec(DB, false);
            }
            catch (Exception ex)
            {
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);
                throw ex;
            }
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                GetData();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Error:" + ex.Message);
                IsRuning = false;
                throw ex;
            }
            finally
            {
                SFCDB.FreeMe();
            }
        }

        public void GetData()
        {
            UriBuilder URI = new UriBuilder(sourceURL);
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidate;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI.Uri);
            if (sourceUser!="")
            {
                request.Credentials = new NetworkCredential(sourceUser, sourcePWD);
                request.PreAuthenticate = true;
            }
            WebResponse rsp = request.GetResponse();
            Stream rs = rsp.GetResponseStream();
            StreamReader SR = new StreamReader(rs);

            DataTable dt = new DataTable();
            dt.TableName = "R_TDMS_MDV_DATA";
            using (SR)
            {
                string strLine = SR.ReadLine();//這裡讀取第一行，數據欄位
                string[] arrColumn = strLine.ToUpper().Split(',');
                for (int i = 0; i < arrColumn.Length; i++)
                {
                    dt.Columns.Add(arrColumn[i], Type.GetType("System.String"));
                }

                //第一行過後就是數據主體
                while ((strLine = SR.ReadLine()) != null)
                {
                    string[] arrRow = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < arrColumn.Length; i++)
                    {
                        dr[arrColumn[i]] = arrRow[i].ToString();
                    }
                    dt.Rows.Add(dr);
                }
            }
            SR.Close();

            string sqlString = "delete FROM R_TDMS_MDV_DATA";
            SFCDB.ExecSQL(sqlString);
            var data = common.DataTableToList<R_TDMS_MDV_DATA>(dt);
            int n = SFCDB.ORM.Insertable<R_TDMS_MDV_DATA>(data).ExecuteCommand();
            FileLog.WriteContentToLogTxt(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " GetData Successful!Data Rows:" + n);
        }

        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            //为了通过证书验证，总是返回true
            return true;
        }
    }
}
