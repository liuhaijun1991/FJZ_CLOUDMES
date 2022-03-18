using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace MESHelper.Plugin
{
    public class Printer_Excel
    {
        public void Print(List<object> param)
        {
            JObject dataObj = (JObject)param[0];
            JArray data = (JArray)dataObj["Outputs"];
            string SaveAsPath = param[1].ToString();
            string TemplateFile= param[2].ToString();
            string PrinterName = param[3].ToString();

            Excel.Application excel = new Excel.ApplicationClass();
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;
            try
            {
                wb = excel.Application.Workbooks.Open(TemplateFile, Type.Missing, true, Type.Missing, Type.Missing, Type.Missing,
                               Type.Missing, Type.Missing, Type.Missing, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                ws = (Excel.Worksheet)wb.Worksheets.get_Item(1);
                foreach (var item in data)
                {
                    string Name = "@" + item["Name"].ToString() + "@";
                    string Type = item["Type"].ToString();
                    for (int i = 1; i <= ws.UsedRange.Rows.Count; i++)
                    {
                        Excel.Range A = (Excel.Range)ws.Cells[i, 1];
                        if (A.Text.ToString() == Name)
                        {
                            Console.WriteLine(Name);
                            if (Type == "0")
                            {
                                ws.Cells[i, 2] = item["Value"].ToString();
                                Console.WriteLine(item["Value"].ToString());
                            }
                            else
                            {
                                JArray Values = (JArray)item["Value"];
                                for (int x = 0; x < Values.Count; x++)
                                {
                                    ws.Cells[i, x + 2] = Values[x].ToString();
                                    Console.WriteLine(Values[x].ToString());
                                }
                            }
                        }                        
                    }
                }

                for (int i = 1; i <= ws.UsedRange.Rows.Count; i++)
                {
                    Excel.Range A = (Excel.Range)ws.Cells[i, 1];
                    if (A.Text.ToString() == "@PAGE@")
                    {
                        ws.Cells[i, 2] = dataObj["PAGE"].ToString();
                    }
                    if (A.Text.ToString() == "@ALLPAGE@")
                    {
                        ws.Cells[i, 2] = dataObj["ALLPAGE"].ToString();
                    }
                }
                Console.WriteLine("Print");
                Excel.Worksheet wsPrint = (Excel.Worksheet)wb.Worksheets[2];
                wsPrint.PrintOutEx(1, 32767, 1, false, PrinterName);
                //wsPrint.PrintOutEx();
                SaveAsPath = SaveAsPath.Insert(SaveAsPath.LastIndexOf("."), DateTime.Now.ToString("yyyyMMddhhmmssfff"));
                if (File.Exists(SaveAsPath))
                {
                    try
                    {
                        File.Delete(SaveAsPath);
                    }
                    catch
                    {
                    }
                }
                ws.SaveAs(SaveAsPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception ec)
            {
                throw ec;
            }
            finally
            {
                KillProcess(excel);
            }
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        /// <summary>
        /// 杀掉生成的进程
        /// </summary>
        /// <param name="AppObject">进程程对象</param>
        private static void KillProcess(Excel.Application AppObject)
        {
            int Pid = 0;
            IntPtr Hwnd = new IntPtr(AppObject.Hwnd);
            System.Diagnostics.Process p = null;
            try
            {
                GetWindowThreadProcessId(Hwnd, out Pid);
                p = System.Diagnostics.Process.GetProcessById(Pid);
                if (p != null)
                {
                    p.Kill();
                    p.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("进程关闭失败！异常信息：" + ex);
            }
        }        
    }
}
