using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class BGA_TE_Report : MESReport.ReportBase
    {
        ReportInput Type = new ReportInput() { Name = "Type", InputType = "Select", Value = "WIP", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "WIP", "Movements" } };
        public BGA_TE_Report()
        {
            Inputs.Add(Type);
        }

        public override void Run()
        {
            try
            {
                string url = "";
                string cType = Type.Value.ToString();
                if(cType == "WIP")
                {
                    url = "http://10.14.188.27:8000/Actives/";
                }
                else
                {

                    url = "http://10.14.188.27:8000/movements/";
                }

                string responseBody = null;
                DataTable respondeData;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Accept = "application/json";

                if(!PingHost(request.RequestUri.Host, request.RequestUri.Port, url))
                {
                    return;
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();
                        }
                    }
                }

                respondeData = JsonConvert.DeserializeObject<DataTable>(responseBody);

                ReportTable retTab = new ReportTable();
                retTab.Tittle = "BGA TE Repair " +cType+ " - Any question or doubt on this report pls contact to TE Jesus Marquez";

                DataTable linkTable = new DataTable();

                foreach (DataColumn column in respondeData.Columns)
                    linkTable.Columns.Add(column.ColumnName);


                foreach (DataRow row in respondeData.Rows)
                {
                    string linkURL1 = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["Serial_NO"].ToString().Trim().ToUpper();
                    var linkRow = linkTable.NewRow();

                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "SERIAL_NO")
                        {
                            linkRow[dc.ColumnName] = linkURL1;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }

                retTab.LoadData(respondeData, linkTable);
                Outputs.Add(retTab);


            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }

        }

        public bool PingHost(string hostUri, int portNumber, string url)
        {
            try
            {
                using (var client = new TcpClient(hostUri, portNumber))
                    return true;
            }
            catch (SocketException ex)
            {          
                ReportAlart alart = new ReportAlart(@"Error pinging host: '" + hostUri + ":" + portNumber.ToString() + "' \n Pls contacto to Test Engineer:Jesus C. Marquez/Nestor D. Medina to check the BGA Web API: " + url + " \n "  + ex.Message);
                Outputs.Add(alart);
                return false;
            }
        }

    }
}
