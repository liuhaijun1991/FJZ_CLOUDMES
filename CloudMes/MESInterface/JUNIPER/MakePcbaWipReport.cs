using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESJuniper.OrderManagement;
using MESJuniper.SendData;
using MESPubLab.Json;
using MESReport;
using MESReport.BaseReport;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace MESInterface.JUNIPER
{
    public  class MakePcbaWipReport : taskBase
    {
        public string BU = "";
        public string SFCDBstr = "";
        //public string OMDBstr = "";
        public string Factory = "";
        public bool IsRuning = false;
        string SMTP_IP;
        string To;
        string From;

        OleExec SFCDB;
        public override void init()
        {
            

            try
            {
                SFCDBstr = ConfigGet("MESDB");
                //OMDBstr = ConfigGet("OMDB");
                Factory = ConfigGet("FACTORY");
                SMTP_IP = ConfigGet("SMTP_IP");
                To = ConfigGet("TO");
                From = ConfigGet("FROM");


            }
            catch (Exception ex)
            {
               
            }
            Output.UI = new MakePcbaWipReport_UI(this);
        }
        public override void Start()
        {
            SFCDB = new OleExec(SFCDBstr, false);
            
            var strNow = DateTime.Now.ToString("yyyyMMdd");
            var strLast = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            SKUWOReport dataLast = null;
            SKUWOReport data = null;
            var Path = ".\\DATA\\" + strNow + ".txt";

            //var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            if (!System.IO.File.Exists(Path))
            {
                data = new MESReport.BaseReport.SKUWOReport();
                Predicate<ReportInput> match = t => t.Name == "TYPE";
                data.Inputs.Find(match).Value = "PCBA";
                data.SFCDB = SFCDB;
                data.Run();
                var json = JsonSave.GetJson(data);
                System.IO.FileStream fs = new System.IO.FileStream(Path, System.IO.FileMode.Create);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                sw.Write(json);
                sw.Flush();
                sw.Close();
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<SKUWOReport>(json);
            }
            else
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(Path);
                var json = sr.ReadToEnd();
                sr.Close();
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<SKUWOReport>(json);
            }
            Path = ".\\DATA\\" + strLast + ".txt";
            if (System.IO.File.Exists(Path))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(Path);
                var json = sr.ReadToEnd();
                sr.Close();
                dataLast = Newtonsoft.Json.JsonConvert.DeserializeObject<SKUWOReport>(json);
            }
            else
            {
                dataLast = data;
            }
            

            DataTable data1 = new DataTable();
            DataTable data2 = new DataTable();
            DataTable data3 = new DataTable();
            DataTable data4 = new DataTable();
            var table = (JObject)data.Outputs[0];
            var t1 = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportTable>(table.ToString());
            table = (JObject)dataLast.Outputs[0];
            var t2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportTable>(table.ToString());

            data1.TableName = strNow;
            data3.TableName = strLast;
            data2.TableName = "Diff";
            data4.TableName = "SUMMARY";

            for (int i = 0; i < t1.ColNames.Count; i++)
            {
                data1.Columns.Add(t1.ColNames[i]);
                data2.Columns.Add(t1.ColNames[i]);
                data3.Columns.Add(t1.ColNames[i]);
                switch (t1.ColNames[i])
                {
                    case "WORKORDERNO":
                    case "SKUNO":
                    case "DATS":
                    case "VER":
                    case "QTY":
                    case "CLOSED_FLAG":
                        data4.Columns.Add(t1.ColNames[i]);
                        break;
                    default:
                        data4.Columns.Add(t1.ColNames[i]+"_Y");
                        data4.Columns.Add(t1.ColNames[i] + "_T");
                        data4.Columns.Add(t1.ColNames[i] + "_Diff");
                        break;
                }

            }

            for (int i = 0; i < t2.Rows.Count; i++)
            {
                var r3 = data3.NewRow();
                data3.Rows.Add(r3);
                for (int j = 0; j < t1.ColNames.Count; j++)
                {
                    try
                    {
                        r3[t1.ColNames[j]] = t2.Rows[i][t1.ColNames[j]].Value;
                    }
                    catch
                    { }
                }
            }

            for (int i = 0; i < t1.Rows.Count; i++)
            {
                var r1 = data1.NewRow();
                var r2 = data2.NewRow();
                var r4 = data4.NewRow();
                //var r3 = data3.NewRow();
                data1.Rows.Add(r1);
                data2.Rows.Add(r2);
                data4.Rows.Add(r4);
                //data3.Rows.Add(r3);
                for (int j = 0; j < t1.ColNames.Count; j++)
                {
                    var wo = t1.Rows[i]["WORKORDERNO"].Value.ToString();
                    var rl = t2.Rows.Find(t => t["WORKORDERNO"].Value == wo);
                    switch (t1.ColNames[j])
                    {
                        case "WORKORDERNO": 
                        case "SKUNO": 
                        case "DATS":
                        case "VER":
                        case "QTY":
                        case "CLOSED_FLAG":
                            try
                            {
                                r1[t1.ColNames[j]] = t1.Rows[i][t1.ColNames[j]].Value;
                                r2[t1.ColNames[j]] = t1.Rows[i][t1.ColNames[j]].Value;
                                r4[t1.ColNames[j]] = t1.Rows[i][t1.ColNames[j]].Value;
                            }
                            catch
                            { }
                            break;
                        default:
                            try
                            {
                                r1[t1.ColNames[j]] = t1.Rows[i][t1.ColNames[j]].Value;
                            }
                            catch
                            { 
                            
                            }
                            
                            int qty1 = 0,qty2=0;
                            if (r1[t1.ColNames[j]].ToString() != "")
                            {
                                try
                                {
                                    int.TryParse(r1[t1.ColNames[j]].ToString(), out qty1);
                                }
                                catch
                                { }
                            }
                            if (rl != null && t2.ColNames.Contains(t1.ColNames[j]) && rl[t1.ColNames[j]].ToString() != "")
                            {
                                try
                                {
                                    int.TryParse(rl[t1.ColNames[j]].Value, out qty2);
                                }
                                catch
                                { 
                                
                                }
                            }
                            if (qty1 == 0 && qty2 == 0)
                            { }
                            else
                            {
                                try
                                {
                                    r2[t1.ColNames[j]] = qty1 - qty2;
                                }
                                catch
                                { 
                                
                                }
                                try
                                {
                                    r4[t1.ColNames[j] + "_T"] = qty1;
                                }
                                catch
                                {

                                }
                                try
                                {
                                    r4[t1.ColNames[j] + "_Y"] = qty2;
                                }
                                catch
                                {

                                }
                                try
                                {
                                    r4[t1.ColNames[j] + "_Diff"] = qty1 - qty2;
                                }
                                catch
                                {

                                }


                                
                                

                            }
                            

                            break;
                    }
                }

               
            }
            List<DataTable> datas = new List<DataTable>();
            datas.Add(data1);
            datas.Add(data3);
            datas.Add(data2);
            datas.Add(data4);
            var FileName = AppDomain.CurrentDomain.BaseDirectory + "DATA\\" + strNow + ".xlsx";
            if (!System.IO.File.Exists(FileName))
            {
                MESMailCenter.Excel.CreateExcelFile(datas, FileName);
            }

            var _mailList = (To.Split(new char[] { ',', ';' }));
            foreach (string str in _mailList)
            {
                try
                {
                    sendEmailSMTP(SMTP_IP, str, From, "Juniper PCBA Wip Report", "", FileName);
                }
                catch(Exception ee)
                { }
            }

        }
        public static void sendEmailSMTP(string SMTP_IP, String To, String From, String Subject, String Body, String AttachFile1 = "")
        {

            MailMessage Email = new MailMessage();

            if (AttachFile1 != "")
            {
                Attachment Attach1 = new Attachment(AttachFile1);
                Email.Attachments.Add(Attach1);
            }

            Email.From = new MailAddress(From);
            Email.To.Add(To);
            Email.Subject = Subject;
            Email.IsBodyHtml = true;
            Email.Body = Body;
            Email.Priority = MailPriority.Normal;
            SmtpClient client = new SmtpClient(SMTP_IP);
            client.Send(Email);
        }
    }
}
