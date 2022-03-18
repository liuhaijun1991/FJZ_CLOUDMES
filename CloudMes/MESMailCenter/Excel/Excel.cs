using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Microsoft.Office;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Excel;
using System.Reflection;


namespace MESMailCenter
{
    public class Excel
    {
        public Microsoft.Office.Interop.Excel.Application exapp;
        Workbook book;
        public void LoadFile(string Path)
        {
            try
            {
                new Microsoft.Office.Interop.Excel.ApplicationClass();
                exapp = new ApplicationClass();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            book = exapp.Application.Workbooks.Open(Path, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

        }

        public void QuitAPP()
        {
            try
            {
                exapp.Quit();
            }
            catch
            { }
        }

        /// <summary>
        /// 將datatable的內容輸出到Excel文件中

        /// </summary>
        /// <param name="datas">要輸出的數據</param>
        /// <param name="names">表的名稱</param>
        /// <param name="path">要保存/覆蓋的文件名</param>
        public static void CreateExcelFile(List<System.Data.DataTable> datas, List<string> names, string path)
        {
            Microsoft.Office.Interop.Excel.Application exapp;
            if (datas == null || datas.Count == 0)
            {
                return;
            }
            try
            {
                exapp = new ApplicationClass();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            string ver = exapp.Version;
            string[] vers = ver.Split(new char[] { '.' });
            int intVer = Int32.Parse(vers[0]);
            if (intVer <= 11)
            {
                if (path.ToUpper().EndsWith("XLSX"))
                {
                    path = path.Substring(0, path.Length - 1);
                }
            }
            else
            {
                if (path.ToUpper().EndsWith("XLS"))
                {
                    path = path + "X";
                }
            }



            Workbook book = exapp.Application.Workbooks.Add(true);

            int i = 0;
            do
            {
                for (int j = 0; j < datas[i].Columns.Count; j++)
                {
                    exapp.Cells[1, j + 1] = datas[i].Columns[j].ColumnName;
                }
                //Worksheet;
                _Worksheet sheet = (_Worksheet)exapp.Sheets[i + 1];
                sheet.Name = names[i];


                for (int k = 0; k < datas[i].Rows.Count; k++)
                {
                    DataRow dr = datas[i].Rows[k];
                    for (int j = 0; j < datas[i].Columns.Count; j++)
                    {
                        exapp.Cells[k + 2, j + 1] = "'" + dr[j].ToString();
                    }
                }
                i++;
                if (i < datas.Count)
                {
                    try
                    {
                        ((Worksheet)exapp.ActiveSheet).Name = datas[i].TableName;
                    }
                    catch
                    { }
                    exapp.Sheets.Add(Missing.Value, exapp.ActiveSheet, 1, Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);
                }

            } while (i < datas.Count);

            exapp.Visible = false;
            exapp.AlertBeforeOverwriting = true;
            exapp.DisplayAlerts = false;
            try
            {
                exapp.ActiveWorkbook.SaveAs(path, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value
                    , XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value
                    , Missing.Value, Missing.Value);
                exapp.DisplayAlerts = false;
                exapp.EnableAutoComplete = true;
                exapp.Quit();

            }
            catch (Exception ex)
            {
                exapp.Quit();
                throw ex;
            }

            GC.Collect();

        }

        public static string GetVer()
        {
            Microsoft.Office.Interop.Excel.Application app;
            try
            {
                app = new ApplicationClass();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string ret = "";
            ret = app.Version;

            try
            {
                app.Quit();
            }
            catch
            { }
            return ret;
        }

        /// <summary>
        /// 將datatable的內容輸出到Excel文件中

        /// </summary>
        /// <param name="datas">要輸出的數據</param>
        /// 
        /// <param name="path">要保存/覆蓋的文件名</param>
        public static void CreateExcelFile(List<System.Data.DataTable> datas, string path)
        {
            Microsoft.Office.Interop.Excel.Application exapp;
            if (datas == null || datas.Count == 0)
            {
                return;
            }
            try
            {
                exapp = new ApplicationClass();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //string strVer = exapp.Version;
            //string[] vers = strVer.Split(new char[] { '.' });
            //try
            //{
            //    int intVer = Int32.Parse(vers[0]);
            //    if (intVer <= 11)
            //    {
            //        if (path.ToUpper().EndsWith("XLSX"))
            //        {
            //            path = path.Substring(0, path.Length - 1);
            //        }
            //    }
            //    else
            //    {
            //        if (path.ToUpper().EndsWith("XLS"))
            //        {
            //            path = path + "x";
            //        }
            //    }
            //}
            //catch
            //{ }

            try
            {
                Workbook book = exapp.Application.Workbooks.Add(true);
                int i = 0;
                do
                {
                    try
                    {
                        ((Worksheet)exapp.ActiveSheet).Name = datas[i].TableName;
                    }
                    catch
                    { }
                    for (int j = 0; j < datas[i].Columns.Count; j++)
                    {
                        exapp.Cells[1, j + 1] = datas[i].Columns[j].ColumnName;
                    }
                    //_Worksheet sss = (_Worksheet)exapp.Sheets[1];
                    //sss.

                    for (int k = 0; k < datas[i].Rows.Count; k++)
                    {
                        DataRow dr = datas[i].Rows[k];
                        for (int j = 0; j < datas[i].Columns.Count; j++)
                        {
                            exapp.Cells[k + 2, j + 1] = "'" + dr[j].ToString();
                        }
                    }
                    i++;
                    if (i < datas.Count)
                    {
                        
                        exapp.Sheets.Add(Missing.Value, exapp.ActiveSheet, 1, Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);
                    }

                } while (i < datas.Count);

                exapp.Visible = false;
                exapp.AlertBeforeOverwriting = true;
                exapp.DisplayAlerts = false;
            }
            catch (Exception ex)
            {
                exapp.Quit();
                throw new Exception("生成EXCEL時發生異常:" + ex.Message);
            }
            try
            {
                exapp.ActiveWorkbook.SaveAs(path, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value
                    , XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value
                    , Missing.Value, Missing.Value);
                exapp.DisplayAlerts = false;
                exapp.EnableAutoComplete = true;
                

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            finally
            {
                
                exapp.Workbooks.Close();
                exapp.Quit();
            }
            GC.Collect();
        }
    }
}
