using MESHelper.Common;
using MESHelper.Plugin;
using MESPubLab.MESStation.Label;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MESHelper
{
    public class SocketService : WebSocketBehavior
    {
        public JObject SyncRequest;
        //string LabelTempPath = Environment.CurrentDirectory + "\\LabelTemp";
        public static string ZebraPort { get; set; }
        public static string[] PrinterList;
        public static bool isLocalPath { get; set; }
        public static string LocalPath { get; set; }
        public static string ComPortName { get; set; }
        public static int BaudRate { get; set; }
        public static string WeighterType { get; set; }
        public static string ComMsg { get; set; }

        public static string WeightData { get; set; }

        public static string Log { get; set; } = "";

        public static string WeighterRegex { get; set; }
        public static Reader_COM reader;
        //public static MESSocketClient client;
        public static string LabelTempPath = Environment.CurrentDirectory + "\\LabelTemp";
        public static MESAPIClient MESAPI;
        public static string LocalFile;
        Logs log = new Logs();
        Printer_Codesoft _csl;
        Printer_Zebra _zbl;
        Printer_Doc _Pdf;
        static bool rununio = true;
        Dictionary<string, Printer_Codesoft> CSDocuments = new Dictionary<string, Printer_Codesoft>();

        protected override void OnMessage(MessageEventArgs e)
        {
            JObject obj = JsonConvert.DeserializeObject<JObject>(e.Data);
            string MessageID = obj["MessageID"].ToString();
            string ClientID = obj["ClientID"].ToString();
            string msg = "";
            object ResData = null;
            object ReSend;
            List<JObject> Datas = new List<JObject>();
            JObject Data = null;

            lock (Log)
            {
                Log = e.Data + "\r\n" + Log;

                if (Log.Length > 30000)
                {
                    Log = Log.Substring(0, 30000);
                }
            }

            string TCode = obj["TCode"].ToString();

            if (TCode.Contains("PRINT"))
            {
                //模板相同，批量打印
                if (TCode == "PRINTS")
                {
                    List<LabelBase> lbs = new JavaScriptSerializer().Deserialize<List<LabelBase>>(obj["Data"].ToString());
                    foreach (LabelBase lb in lbs)
                    {
                        Datas.Add(JObject.Parse(JsonConvert.SerializeObject(lb)));
                    }
                    Data = Datas[0];
                }
                //模板不同，單獨打印 或者 模板相同，不需要重复打开模板但是要单独打印
                else
                {
                    Data = (JObject)obj["Data"];
                }
                //模板可以放在數據庫 R_LABEL 中以供在線獲取模板進行打印（在線獲取模板其實是讀取 R_LABEL 中的 BLOB_DATA 的內容，寫入到本地的文件中再將本地文件路徑返回）
                //也可以使用本地模板進行打印（即如果模板不會有變更的話，那麼就不需要頻繁的讀取數據庫、寫入文件，直接使用本地文件）
                LocalFile = getLocalFile(Data["FileName"].ToString());
                int PrintQTY = int.Parse(Data["PrintQTY"].ToString());
                int PrinterIndex = 0;
                try
                {
                    PrinterIndex = int.Parse(Data["PrinterIndex"].ToString()) - 1;
                }
                catch
                {
                }
                string ExtFileName = LocalFile.Substring(LocalFile.LastIndexOf('.') + 1).ToUpper();
                if (TCode == "PRINT")
                {
                    ResData = "Label:" + LocalFile;
                    switch (ExtFileName)
                    {
                        case "LAB":
                            //替換模板文件中的變量，使用 LabelManager 的 Document 類直接進行打印 Document.PrintDocument
                            msg = CodeSoftPrinter(Data, LocalFile, PrinterIndex, PrintQTY, false);
                            break;
                        case "TXT":
                            //替換模板文件中的變量，生成對應的 txt 文件，將 txt 文件拷貝到設定的 LPT 端口，進行打印
                            msg = ZBLPrinter(Data, LocalFile, PrintQTY) + ",Label:" + LocalFile;
                            break;
                        case "XLS":
                        case "XLSX":
                            //調用 MESHelper.Plugin 中的 Printer_Excel 類的 Print 方法，使用往 excel 裡面填值的方式，
                            //根據 excel 裡面第一列單元格的內容作為條件，使用 Outputs 中的結果往該行的第二列、第三列... 填值，最後使用
                            //Excel.Worksheet 的 PrintOutEx 方法打印
                            msg = ExcelPrinter(Data, LocalFile, PrinterIndex, PrintQTY) + ",Label:" + LocalFile;
                            break;
                        case "PDF":
                            //直接使用 Windows 自帶的 Print 進程打印文件，Data 參數傳入未看到意義
                            msg = DocPrinter(Data, LocalFile, PrinterIndex, PrintQTY) + ",Label:" + LocalFile;
                            break;
                        default:
                            msg = " Unsupported file types!";
                            break;
                    }
                }
                else if (TCode == "PRINTS")
                {
                    ResData = "Label:" + LocalFile;
                    switch (ExtFileName)
                    {
                        case "LAB":
                            //替換模板文件中的變量，使用 LabelManager 的 Document 類直接進行打印 Document.PrintDocument
                            msg = CodeSoftPrinter(Datas, LocalFile, PrinterIndex, PrintQTY);
                            break;
                        case "TXT":
                            //替換模板文件中的變量，生成對應的 txt 文件，將 txt 文件拷貝到設定的 LPT 端口，進行打印
                            //msg = ZBLPrinter(Datas, LocalFile, PrintQTY) + ",Label:" + LocalFile;
                            string _bu = HWDNNSFCBase.ConfigFile.ReadIniData("ServerConnection", "Bu", "", Environment.CurrentDirectory + "\\Helper.ini");
                            if (_bu.ToUpper() == "HWD")
                            {
                                msg = ZBLPrinterNew(Datas, LocalFile, PrintQTY) + ",Label:" + LocalFile;
                            }
                            else
                            {
                                msg = ZBLPrinter(Datas, LocalFile, PrintQTY) + ",Label:" + LocalFile;
                            }
                            break;
                        case "XLS":
                        case "XLSX":
                            //調用 MESHelper.Plugin 中的 Printer_Excel 類的 Print 方法，使用往 excel 裡面填值的方式，
                            //根據 excel 裡面第一列單元格的內容作為條件，使用 Outputs 中的結果往該行的第二列、第三列... 填值，最後使用
                            //Excel.Worksheet 的 PrintOutEx 方法打印
                            msg = ExcelPrinter(Datas, LocalFile, PrinterIndex, PrintQTY) + ",Label:" + LocalFile;
                            break;
                        case "PDF":
                            //直接使用 Windows 自帶的 Print 進程打印文件，Data 參數傳入未看到意義
                            msg = DocPrinter(Datas, LocalFile, PrinterIndex, PrintQTY) + ",Label:" + LocalFile;
                            break;
                        default:
                            //msg = " 不支持的文件類型!";
                            msg = " Unsupported file types!";
                            break;
                    }
                }
                else if (TCode == "STILLPRINT")
                {
                    ResData = "Label:" + LocalFile;
                    switch (ExtFileName)
                    {
                        case "LAB":
                            //替換模板文件中的變量，使用 LabelManager 的 Document 類直接進行打印 Document.PrintDocument
                            msg = CodeSoftPrinter(Data, LocalFile, PrinterIndex, PrintQTY, true);
                            break;
                        default:
                            //msg = " 不支持的文件類型!";
                            msg = " Unsupported file types!";
                            break;
                    }
                }
            }
            else if (TCode == "GETCOMDATA")
            {
                ResData = GetDataFromCOMPort();
                msg = "OK";
            }
            if (msg.Substring(0, 2) != "OK")
            {
                ReSend = new { Status = "Fail", ClientID = ClientID, MessageID = MessageID, Data = ResData, Message = msg };
            }
            else
            {
                ReSend = new { Status = "Pass", ClientID = ClientID, MessageID = MessageID, Data = ResData, Message = msg };
            }
            log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + msg);

            string ret = JsonConvert.SerializeObject(ReSend);
       
            Send(ret);

            lock (Log)
            {
                Log = ret + "\r\n" + Log;

                if (Log.Length > 30000)
                {
                    Log = Log.Substring(0, 30000);
                }
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            try
            {
                if (_csl != null)
                {
                    _csl.close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("close lppa Exception:" + ex.Message);
            }
            finally
            {
                base.OnClose(e);
            }
        }

        string ExcelPrinter(JObject data, string LocalFile, int PrintIndex, int PrintQTY)
        {
            try
            {
                string SaveAsFile = Environment.CurrentDirectory + "\\Temp" + LocalFile.Substring(LocalFile.LastIndexOf("\\"));
                List<object> Params = new List<object> { data, SaveAsFile, LocalFile, SocketService.PrinterList[PrintIndex] };
                byte[] filesByte = File.ReadAllBytes(Path.GetDirectoryName(Application.ExecutablePath) + "//MESHelper.Plugin.dll");
                Assembly assembly = Assembly.Load(filesByte);
                Type type = assembly.GetType("MESHelper.Plugin.Printer_Excel");
                object obj = System.Activator.CreateInstance(type);
                try
                {
                    MethodInfo method = type.GetMethod("Print");
                    method.Invoke(obj, new object[] { Params });
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                //return "OK,打印成功";
                return "OK,Print Success!";
            }
            catch (Exception)
            {
                //return "請檢查目錄是否缺少SfcPrintExcel.dll及安裝Microsoft Office Excel 2003!";
                return "Pls check if the directory is missing SfcPrintExcel.dll and setup Microsoft Office Excel 2003!";
            }
        }
        string ExcelPrinter(List<JObject> Datas, string LocalFile, int PrintIndex, int PrintQTY)
        {
            try
            {
                string SaveAsFile = Environment.CurrentDirectory + "\\Temp" + LocalFile.Substring(LocalFile.LastIndexOf("\\"));
                List<object> Params = null;
                byte[] filesByte = File.ReadAllBytes(Path.GetDirectoryName(Application.ExecutablePath) + "//MESHelper.Plugin.dll");
                Assembly assembly = Assembly.Load(filesByte);
                Type type = assembly.GetType("MESHelper.Plugin.Printer_Excel");
                object obj = System.Activator.CreateInstance(type);
                try
                {
                    MethodInfo method = type.GetMethod("Print");
                    foreach (JObject data in Datas)
                    {
                        Params = new List<object> { data, SaveAsFile, LocalFile, SocketService.PrinterList[PrintIndex] };
                        method.Invoke(obj, new object[] { Params });
                    }

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                return "OK,打印成功";
            }
            catch (Exception)
            {
                //return "請檢查目錄是否缺少SfcPrintExcel.dll及安裝Microsoft Office Excel 2003!";
                return "Pls check if the directory is missing SfcPrintExcel.dll and setup Microsoft Office Excel 2003!";
            }
        }
        string CodeSoftPrinter(JObject data, string LocalFile, int PrinterIndex, int PrintQTY)
        {
            try { _csl.close(); }
            catch { }
            try
            {
                _csl = new Printer_Codesoft(LocalFile);
                _csl.LabValue = data;
                try
                {
                    _csl.PrinterName = SocketService.PrinterList[PrinterIndex];
                }
                catch
                {
                }
                _csl.Print(PrintQTY);
                _csl.close();
            }
            catch (Exception ex)
            {
                log.WriteLog("打開LAB時出錯:" + ex.Message);
                //return "打開LAB時出錯:" + ex.Message;
                return "Open LAB Err:" + ex.Message;
            }

            //return "OK,打印成功";
            return "OK,Print Successful!";
        }

        /// <summary>
        /// 如果不需要频繁的打开关闭文档，那么就传入 IsStill 为 true，否则就传入 false
        /// 频繁的打开关闭文档耗费打印时间，在线工站打印时会引起极度不舒服。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="LocalFile"></param>
        /// <param name="PrinterIndex"></param>
        /// <param name="PrintQTY"></param>
        /// <param name="IsStill"></param>
        /// <returns></returns>
        string CodeSoftPrinter(JObject data, string LocalFile, int PrinterIndex, int PrintQTY, bool IsStill)
        {
            try
            {
                if (IsStill)
                {
                    if (CSDocuments.ContainsKey(LocalFile))
                    {
                        _csl = CSDocuments[LocalFile];
                    }
                    else
                    {
                        _csl = new Printer_Codesoft(LocalFile);
                        CSDocuments.Add(LocalFile, _csl);
                    }
                    try
                    {
                        _csl.PrinterName = SocketService.PrinterList[PrinterIndex];
                    }
                    catch
                    {
                    }
                    _csl.LabValue = data;
                    _csl.Print(PrintQTY);

                }
                else
                {
                    try { _csl.close(); } catch { }
                    _csl = new Printer_Codesoft(LocalFile);
                    try
                    {
                        _csl.PrinterName = SocketService.PrinterList[PrinterIndex];
                    }
                    catch
                    {
                    }
                    _csl.LabValue = data;
                    _csl.Print(PrintQTY);
                    _csl.close();
                }
            }
            catch (Exception ex)
            {
                log.WriteLog("打開LAB時出錯:" + ex.Message);
                //return "打開LAB時出錯:" + ex.Message;
                return "Open LAB Err:" + ex.Message;
            }
            //return "OK,打印成功";
            return "OK,Print Successful!";
        }
        string CodeSoftPrinter(List<JObject> Datas, string LocalFile, int PrinterIndex, int PrintQTY)
        {
            try { _csl.close(); }
            catch { }
            try
            {
                _csl = new Printer_Codesoft(LocalFile);
                try
                {
                    _csl.PrinterName = SocketService.PrinterList[PrinterIndex];
                }
                catch
                {
                }
                foreach (JObject data in Datas)
                {                    
                    _csl.LabValue = data;
                    _csl.Print(PrintQTY);                    
                }
                _csl.close();
            }
            catch (Exception ex)
            {
                log.WriteLog("打開LAB時出錯:" + ex.Message);
                //return "打開LAB時出錯:" + ex.Message;
                return "Open LAB Err:" + ex.Message;
            }

            //return "OK,打印成功";
            return "OK,Print Successful!";
        }
        string ZBLPrinter(JObject data, string LocalFile, int PrintQTY)
        {
            try
            {
                _zbl = new Printer_Zebra();
                _zbl.FilePath = LocalFile;
                _zbl.Port = SocketService.ZebraPort;
                _zbl.LabValue = data;
                _zbl.Print("1");
                //return "OK,打印成功";
                return "OK,Print Successful!";
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
                return ex.Message;
            }
        }

        string ZBLPrinter(List<JObject> Datas, string LocalFile, int PrintQTY)
        {
            try
            {
                _zbl = new Printer_Zebra();
                _zbl.FilePath = LocalFile;
                _zbl.Port = SocketService.ZebraPort;
                int i = 0;
                foreach (JObject data in Datas)
                {
                    i++;
                    _zbl.LabValue = data;
                    _zbl.Print(i.ToString());
                }
                //return "OK,打印成功";
                return "OK,Print Successful!";
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
                return ex.Message;
            }
        }

        string ZBLPrinterNew(List<JObject> Datas, string LocalFile, int PrintQTY)
        {
            try
            {
                int _printQty = 50;//每次輸出50個打印腳本
                int _printCount = 0;
                string tempZbl = "";

                StreamReader R;
                R = new StreamReader(LocalFile);
                string temp = R.ReadToEnd();
                string temp1 = "";
                int numFrom = 0;
                int numTo = 0;

                if (Datas.Count % _printQty == 0)
                {
                    _printCount = Datas.Count / _printQty;
                }
                else
                {
                    _printCount = (Datas.Count / _printQty) + 1;
                }
                for (int i = 1; i <= _printCount; i++)
                {
                    tempZbl = "";
                    if (_printCount == 1)
                    {
                        numFrom = 0;
                        numTo = Datas.Count;
                    }
                    else
                    {
                        if (i < _printCount)
                        {
                            numFrom = (i - 1) * _printQty;
                            numTo = i * _printQty;
                        }
                        else
                        {
                            numFrom = (i - 1) * _printQty;
                            numTo = Datas.Count;
                        }
                    }
                    for (int j = numFrom; j < numTo; j++)
                    {
                        temp1 = temp;
                        JArray data = (JArray)Datas[j]["Outputs"];
                        //將模板裡面的變量替換成實際的值生成打印字符串
                        foreach (var dc in data)
                        {
                            string Name = dc["Name"].ToString();
                            string Type = dc["Type"].ToString();
                            string ItemName = "";
                            if (Type == "0")
                            {
                                ItemName = "@" + Name + "@";
                                try
                                {
                                    temp1 = temp1.Replace(ItemName, dc["Value"].ToString());
                                }
                                catch (Exception e)
                                {
                                    MESPubLab.Common.MesLog.Error($@"err:{e.Message};printdata:{temp1}");
                                }
                            }
                            else
                            {
                                JArray Values = (JArray)dc["Value"];
                                for (int k = 0; k < Values.Count; k++)
                                {
                                    ItemName = "@" + Name + (k + 1).ToString() + "@";
                                    try
                                    {
                                        temp1 = temp1.Replace(ItemName, Values[k].ToString());
                                    }
                                    catch (Exception e)
                                    {
                                        MESPubLab.Common.MesLog.Error($@"err:{e.Message};printdata:{temp1}");
                                    }
                                }
                            }
                        }
                        try
                        {
                            temp1 = temp1.Replace("@PAGE@", Datas[j]["PAGE"].ToString());
                            temp1 = temp1.Replace("@ALLPAGE@", Datas[j]["ALLPAGE"].ToString());
                        }
                        catch (Exception e)
                        {
                            MESPubLab.Common.MesLog.Error($@"err:{e.Message};printdata:{temp1}");
                        }
                        tempZbl = tempZbl + temp1 + "\r\n\r\n";
                    }

                    IntPtr iHandle = CreateFile("LPT1", 0x40000000, 0, 0, 3, 0, 0);
                    if (iHandle.ToInt32() == -1)
                    {
                        //throw new Exception("無法打開LPT1端口");
                        throw new Exception("Unable to open port LPT1!");
                    }
                    FileStream FS = new FileStream(iHandle, FileAccess.Write);
                    StreamWriter SW = new StreamWriter(FS);
                    try
                    {
                        SW.Write(tempZbl);
                        SW.Flush();
                        System.Threading.Thread.Sleep(100);
                        FS.Close();
                    }
                    catch (Exception ee)
                    {
                        try
                        {
                            FS.Close();
                        }
                        catch
                        { }
                        throw ee;
                    }
                    log.WritePrintFile(tempZbl, i.ToString());
                }

                //return "OK,打印成功";
                return "OK,Print Successful!";
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
                return ex.Message;
            }
        }

        string DocPrinter(JObject data, string LocalFile, int PrinterIndex, int PrintQTY)
        {
            try
            {
                _Pdf = new Printer_Doc(LocalFile);
                try
                {
                    _Pdf.PrinterName = SocketService.PrinterList[PrinterIndex];
                }
                catch
                {
                }
                _Pdf.Print();
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
                return ex.Message;
            }
            //return "OK,打印成功";
            return "OK,Print Successful!";
        }
        string DocPrinter(List<JObject> Datas, string LocalFile, int PrinterIndex, int PrintQTY)
        {
            try
            {
                _Pdf = new Printer_Doc(LocalFile);
                try
                {
                    _Pdf.PrinterName = SocketService.PrinterList[PrinterIndex];
                }
                catch
                {
                }
                _Pdf.Print();
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
                return ex.Message;
            }
            //return "OK,打印成功";
            return "OK,Print Successful!";
        }
        string GetDataFromCOMPort()
        {
            string _R = "";
            try
            {
               
                if (SocketService.WeighterType == "9")
                {
                    _R = FrmInput.GetValue("Input Weight");
                    return _R;
                }
                else
                {
                    if (reader == null)
                    {
                        reader = new Reader_COM(SocketService.ComPortName, SocketService.BaudRate, SocketService.WeighterType);
                    }
                    if (!reader.COM.IsOpen)
                    {
                        reader.Open();
                    }
                    double _RD = 0;
                    int i = 20;
                    while (_RD == 0)
                    {
                        _RD = reader.GetWeight();
                        Thread.Sleep(500);
                        i--;
                        if (i <= 0)
                        {
                            throw new Exception("Time out");
                        }
                    }

                    _R = "OK," + _RD.ToString();
                    return _R;
                }
            }
            catch (Exception ex)
            {
                log.WriteLog("獲取電子稱數據出錯:" + ex.Message);
                //return "獲取電子稱數據出錯:" + ex.Message;
                return "Error getting electronic scale data:" + ex.Message;
            }
        }
        /// <summary>
        /// 本地：根據指定的模板文件路徑，將模板文件拷貝到當前執行目錄的 LabelTemp 下面，並且返回該路徑進行後續打印
        /// 在線：從數據庫的 R_LABEL 中讀取 Blob_Data 的值寫入到模板文件中，並且存放在當前執行目錄的 LabelTemp 目錄下，返回該路徑進行後續打印
        /// </summary>
        /// <param name="_FileName"></param>
        /// <returns></returns>
        public string getLocalFile(string _FileName)
        {
            //string LabelTempPath = Environment.CurrentDirectory + "\\LabelTemp";
            string FileName = _FileName;
            if (isLocalPath)
            {
                try
                {
                    string tempPath = "";
                    if (FileName.Contains("/"))
                    {
                        tempPath = FileName.Substring(0, FileName.LastIndexOf("/"));
                        FileName = FileName.Substring(FileName.LastIndexOf("/") + 1);
                    }
                    else if (FileName.Contains("\\"))
                    {
                        tempPath = FileName.Substring(0, FileName.LastIndexOf("\\"));
                        FileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);
                    }
                    if (!File.Exists(LabelTempPath + "\\" + FileName))
                    {
                        if (!Directory.Exists(LabelTempPath))
                        {
                            Directory.CreateDirectory(LabelTempPath);
                        }
                        //將LAB拷貝到緩存文件夾
                        File.Copy(LocalPath + tempPath + "\\" + FileName, LabelTempPath + "\\" + FileName, true);
                    }
                }
                catch (Exception ex)
                {
                    //throw new Exception(" 複製模板出錯：" + ex.Message);
                    throw new Exception(" Copy LabFile Error：" + ex.Message);
                }
            }
            else
            {
                //client.GetFile(FileName, "LABEL", GetFile_Handler);
                FileName = MESAPI.GetFile(FileName, "LABEL", LabelTempPath);

            }
            //LocalFile = LabelTempPath + "\\" + LocalFile;
            LocalFile = LabelTempPath + "\\" + FileName;
            return LocalFile;
        }
        //private void GetFile_Handler(object sender, MessageEventArgs e)
        //{
        //    WebSocket w = (WebSocket)sender;
        //    w.OnMessage -= GetFile_Handler;
        //    JObject Request = (JObject)JsonConvert.DeserializeObject(e.Data);
        //    if (Request["Status"].ToString() == "Pass")
        //    {
        //        JObject Data = (JObject)Request["Data"];
        //        LocalFile = Data["FILENAME"].ToString();
        //        string filepath = LabelTempPath + "\\" + Data["FILENAME"].ToString();
        //        if (System.IO.File.Exists(filepath))
        //        {
        //            System.IO.File.Delete(filepath);
        //        }
        //        FileStream F = new FileStream(filepath, FileMode.Create);
        //        byte[] b = (byte[])Data["BLOB_FILE"];
        //        F.Write(b, 0, b.Length);
        //        F.Flush();
        //        F.Close();
        //    }
        //    else
        //    {
        //        //MessageBox.Show(Request["Message"].ToString());
        //        client.Renew();
        //    }
        //    w.SyncRequest = (JObject)JsonConvert.DeserializeObject(e.Data);
        //}
        DataSet DsJsonToDataset(string value)
        {
            StringReader s = new StringReader(value);
            DataSet ds = new DataSet();
            ds.ReadXml(s);
            return ds;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern IntPtr CreateFile
       (
           string FileName,
           uint DesiredAccess,
           uint ShareMode,
           uint SecurityAttributes,
           uint CreationDisposition,
           uint FlagsAndAttributes,
           int TemplateFile
       );

    }
}
