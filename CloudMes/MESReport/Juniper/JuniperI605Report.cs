using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace MESReport.Juniper
{
    public class JuniperI605Report : ReportBase
    {
        ReportInput inputData = new ReportInput() { Name = "Time", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public JuniperI605Report()
        {
            Inputs.Add(inputData);
        }
        public override void Init()
        {
            base.Init();
        }

        public override void Run()
        {
            //base.Run();
            DataTable dt = new DataTable();
            string runSql = "";

            string indata = inputData.Value.ToString().Trim();
            if (indata == null || indata == "''")
            {
                ReportAlart alart = new ReportAlart("Please input Time");
                Outputs.Add(alart);
                return;
            }

            try
            {
                DateTime indataTime = DateTime.Parse(indata);
                indata = indataTime.ToString("yyyyMMdd");
            }
            catch (Exception e)
            {
                ReportAlart alart = new ReportAlart("Please enter the correct time format " + e.Message);
                Outputs.Add(alart);
                return;
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                runSql = $@"select * from r_sap_file where type = 'I605' and detail_table = 'R_SAP_FILE_I605' and data_key like '{indata}%' order by data_key desc";
                dt = sfcdb.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data, Please check SAP file");
                }
                var fileId = dt.Rows[0]["ID"].ToString();

                runSql = $@"select * from r_sap_file_i605 where file_id = '{fileId}'";
                dt = sfcdb.RunSelect(runSql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NO data");
                }

                //按用戶的格式來
                MES_DCN.Juniper.JuniperI605Format I605 = new MES_DCN.Juniper.JuniperI605Format(sfcdb.ORM);
                dt = I605.JuniperI605FormatTable(fileId);

                //dt.WriteXml("F:\\Work\\Juniper\\I605\\aa.xml");
                //測試轉換成.xsd
                //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                //dt.WriteXml(ms);
                //string result = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" + Encoding.UTF8.GetString(ms.ToArray());
                //ms.Close();
                //ms.Dispose();
                //System.IO.FileStream fs = new System.IO.FileStream("F:\\Work\\Juniper\\I605\\test.xsd", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                //sw.WriteLine(result);
                //sw.Flush();
                //sw.Close();
                //fs.Close();

                //StringBuilder sb = new StringBuilder();
                //XmlWriter writer = XmlWriter.Create(sb);
                //XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
                //serializer.Serialize(writer, dt);
                //writer.Close();
                //string result = sb.ToString();
                //System.IO.FileStream fs = new System.IO.FileStream("F:\\Work\\Juniper\\I605\\test.xml", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                //sw.WriteLine(result);
                //sw.Flush();
                //sw.Close();
                //fs.Close();

                //System.IO.MemoryStream stream = new System.IO.MemoryStream();
                //XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                //dt.WriteXml(writer, XmlWriteMode.WriteSchema);
                //int nCount = (int)stream.Length;
                //byte[] arr = new byte[nCount];
                //stream.Seek(0, System.IO.SeekOrigin.Begin);
                //stream.Read(arr, 0, nCount);
                //UTF8Encoding utf = new UTF8Encoding();
                //string result = utf.GetString(arr).Trim();
                //System.IO.StreamWriter sw = new System.IO.StreamWriter("F:\\Work\\Juniper\\I605\\test.xml");
                //sw.Write(result);
                //sw.Close();
                //writer.Close();
                //stream.Close();


                // 序列化
                //XmlDocument Xdoc = new XmlDocument();
                //XmlDeclaration dec = Xdoc.CreateXmlDeclaration("1.0", "utf-8", null);
                //Xdoc.AppendChild(dec);
                //var eld = Xdoc.CreateElement("ns0", "MT_OnHand", "urn:juniper.net:IBP:PTP:CM:OracleSupplyCloud:OnHand:I605");
                //Xdoc.AppendChild(eld);
                //try
                //{
                //    foreach (DataRow dr in dt.Rows)
                //    {
                //        var el = Xdoc.CreateElement("Records");
                //        eld.AppendChild(el);
                //        foreach (var col in dt.Columns)
                //        {
                //            var eli = Xdoc.CreateElement(col.ToString());
                //            if (dr[col.ToString()] != DBNull.Value)
                //                eli.InnerText = dr[col.ToString()].ToString();
                //            el.AppendChild(eli);
                //        }
                //    }
                //    Xdoc.Save("F:\\Work\\Juniper\\I605\\aa.xml");
                //}
                //catch (Exception e)
                //{
                //}

                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dt, null);
                reportTable.Tittle = "Juniper I605 Report";
                Outputs.Add(reportTable);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            finally
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
    }
}
