
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.Data;
using System.Text;
using OfficeOpenXml.Style;
using CsvHelper;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Reflection;

namespace MESPubLab.Common
{
    /// <summary>
    /// ExcelHelp的摘要描述
    /// </summary>
    public class ExcelHelp
    {
        private static DataTable WorksheetToTable(ExcelWorksheet worksheet)
        {
            //获取worksheet的行数
            int rows = worksheet.Dimension.End.Row;
            //获取worksheet的列数
            int cols = worksheet.Dimension.End.Column;
            DataTable dt = new DataTable(worksheet.Name);
            var nrindex = new List<int>();
            for (int i = 1; i <= cols; i++)
            {
                string headValue = (worksheet.Cells[1, i].Value == DBNull.Value || worksheet.Cells[1, i].Value == null) ? "" : worksheet.Cells[1, i].Value.ToString().Trim().Replace(" ", "");
                if (headValue.Length == 0)
                {
                    nrindex.Add(i);
                    continue;
                    //throw new Exception("必须输入列标题");
                    throw new Exception("Sheet must contain  column header");
                }
                if (dt.Columns.Contains(headValue))
                {
                    //throw new Exception("不能用重复的列标题：" + headValue);
                    throw new Exception("Cannot use duplicate column headers :" + headValue);
                }
                dt.Columns.Add(headValue);
            }
            DataRow dr = null;
            for (int i = 1; i <= rows; i++)
            {
                if (i >= 1)
                {
                    dr = dt.Rows.Add();

                }
                for (int j = 1, e = 1; j <= cols; j++)
                {
                    try
                    {
                        if (nrindex.Contains(j))
                            continue;
                        //dr[e - 1] = worksheet.Cells[i, j].Value==null?"": worksheet.Cells[i, j].Value.ToString();
                        if (worksheet.Cells[i, j].Style.Numberformat.Format.ToUpper().IndexOf("YYYY") > -1 && worksheet.Cells[i, j].Value != null)//注意这里，是处理日期时间格式的关键代码
                            dr[e - 1] = worksheet.Cells[i, j].GetValue<DateTime>().ToString("yyyy-MM-dd HH:mm:ss");
                        else
                            dr[e - 1] = (worksheet.Cells[i, j].Value ?? DBNull.Value);
                        e++;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            dt.Rows.RemoveAt(0);
            return dt;
        }

        public static System.Data.DataTable DBExcelToDataTableEpplus(string pathName)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            FileInfo file = new FileInfo(pathName);
            var package = new ExcelPackage(file);           
            ExcelWorkbook workbook = package.Workbook;
            if (workbook != null)
            {
                if (workbook.Worksheets.Count > 0)
                {
                    ExcelWorksheet worksheet = workbook.Worksheets.First();
                    dt = WorksheetToTable(worksheet);
                }
            }
            return dt;
        }

        public void ExportExcel(DataTable dt)
        {
            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage();

            // 添加一个 sheet 表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);

            int rowIndex = 1;   // 起始行为 1
            int colIndex = 1;   // 起始列为 1

            //设置列名
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                worksheet.Cells[rowIndex, colIndex + i].Value = dt.Columns[i].ColumnName;

                //自动调整列宽，也可以指定最小宽度和最大宽度
                worksheet.Column(colIndex + i).AutoFit();
            }

            // 跳过第一列列名
            rowIndex++;

            //写入数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
                }

                //自动调整行高
                worksheet.Row(rowIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            worksheet.Cells.Style.Font.Name = "Arial";

            //字体加粗
            worksheet.Cells.Style.Font.Bold = true;

            //字体大小
            worksheet.Cells.Style.Font.Size = 12;

            //字体颜色
            worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);

            //单元格背景样式，要设置背景颜色必须先设置背景样式
            worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //单元格背景颜色
            worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);

            //设置单元格所有边框样式和颜色
            worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#0097DD"));

            //单独设置单元格四边框 Top、Bottom、Left、Right 的样式和颜色
            //worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);

            //垂直居中
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //水平居中
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //单元格是否自动换行
            worksheet.Cells.Style.WrapText = false;

            //设置单元格格式为文本
            worksheet.Cells.Style.Numberformat.Format = "@";

            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;


            ////第一种保存方式
            //string path1 = HttpContext.Current.Server.MapPath("Export/");
            //string filePath1 = path1 + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
            //FileStream fileStream1 = new FileStream(filePath1, FileMode.Create);
            ////保存至指定文件
            //FileInfo fileInfo = new FileInfo(filePath1);
            //package.SaveAs(fileInfo);

            ////第二种保存方式
            //string path2 = HttpContext.Current.Server.MapPath("Export/");
            //string filePath2 = path2 + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
            //FileStream fileStream2 = new FileStream(filePath2, FileMode.Create);
            ////写入文件流
            //package.SaveAs(fileStream2);


            //创建一个内存流，然后转换为字节数组，输出到浏览器下载
            //MemoryStream ms = new MemoryStream();
            //package.SaveAs(ms);
            //byte[] bytes = ms.ToArray();

            //也可以直接获取流
            //Stream stream = package.Stream;

            //也可以直接获取字节数组
            byte[] bytes = package.GetAsByteArray();

            //调用下面的方法输出到浏览器下载
            OutputClient(bytes);

            worksheet.Dispose();
            package.Dispose();
        }
        public static string ExportExcelToBase64String(DataTable dt)
        {
            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage();
            // 添加一个 sheet 表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);
            int rowIndex = 1;   // 起始行为 1
            int colIndex = 1;   // 起始列为 1
            //设置列名
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                worksheet.Cells[rowIndex, colIndex + i].Value = dt.Columns[i].ColumnName;
                //自动调整列宽，也可以指定最小宽度和最大宽度
                worksheet.Column(colIndex + i).AutoFit();
            }

            // 跳过第一列列名
            rowIndex++;
            //写入数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
                }
                //自动调整行高
                worksheet.Row(rowIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            worksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            worksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //单元格背景样式，要设置背景颜色必须先设置背景样式
            //worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //单元格背景颜色
            //worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
            //设置单元格所有边框样式和颜色
            //worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#0097DD"));
            //单独设置单元格四边框 Top、Bottom、Left、Right 的样式和颜色
            //worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            worksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            worksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;
            //创建一个内存流，然后转换为字节数组，输出到浏览器下载
            MemoryStream ms = new MemoryStream();
            package.SaveAs(ms);
            byte[] bytes = ms.ToArray();
            worksheet.Dispose();
            package.Dispose();
            return Convert.ToBase64String(bytes);
        }


        public void OutputClient(byte[] bytes)
        {
            HttpResponse response = HttpContext.Current.Response;

            response.Buffer = true;

            response.Clear();
            response.ClearHeaders();
            response.ClearContent();

            //response.ContentType = "application/ms-excel";
            response.ContentType = "application/vnd.openxmlformats - officedocument.spreadsheetml.sheet";
            response.AppendHeader("Content-Type", "text/html; charset=GB2312");
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")));

            response.Charset = "GB2312";
            response.ContentEncoding = Encoding.GetEncoding("GB2312");

            response.BinaryWrite(bytes);
            response.Flush();

            response.End();
        }

        public static string ExportExcelToBase64String<T>(List<T> datas)
        {
            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage();
            // 添加一个 sheet 表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(typeof(T).Name);
            int rowIndex = 1;   // 起始行为 1
            int colIndex = 1;   // 起始列为 1
            var cols = typeof(T).GetProperties();
            var colsfact = new List<PropertyInfo>();
            //设置列名
            for (int i = 0; i < cols.Length; i++)
            {
                var asd = cols[i].GetCustomAttributes(false);
                foreach (var aitem in asd)
                {
                    if (!aitem.GetType().Name.Equals(typeof(ExcelIgone).Name))                    
                        colsfact.Add(cols[i]);                    
                }
                if(asd.Count()==0)                
                    colsfact.Add(cols[i]);                
            }

            for (int i = 0; i < colsfact.Count; i++)
            {
                worksheet.Cells[rowIndex, colIndex + i].Value = colsfact[i].Name;
                //自动调整列宽，也可以指定最小宽度和最大宽度
                worksheet.Column(colIndex + i).AutoFit();
            }

            // 跳过第一列列名
            rowIndex++;
            //写入数据
            for (int i = 0; i < datas.Count; i++)
            {
                for (int j = 0; j < colsfact.Count; j++)
                {
                    worksheet.Cells[rowIndex + i, colIndex + j].Value = colsfact[j].GetValue(datas[i])?.ToString();
                }
                //自动调整行高
                worksheet.Row(rowIndex + i).CustomHeight = true;
            }

            ////写入数据
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    for (int j = 0; j < dt.Columns.Count; j++)
            //    {
            //        worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
            //    }
            //    //自动调整行高
            //    worksheet.Row(rowIndex + i).CustomHeight = true;
            //}

            //设置字体，也可以是中文，比如：宋体
            worksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            worksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //单元格背景样式，要设置背景颜色必须先设置背景样式
            //worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //单元格背景颜色
            //worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
            //设置单元格所有边框样式和颜色
            //worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#0097DD"));
            //单独设置单元格四边框 Top、Bottom、Left、Right 的样式和颜色
            //worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            worksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            worksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;
            //创建一个内存流，然后转换为字节数组，输出到浏览器下载
            MemoryStream ms = new MemoryStream();
            package.SaveAs(ms);
            byte[] bytes = ms.ToArray();
            worksheet.Dispose();
            package.Dispose();
            return Convert.ToBase64String(bytes);
        }

        public static void ExportCsv<T>(List<T> datas,string fullname)
        {
            using (var writer = new StreamWriter(fullname))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(datas);
                }
            }
        }

        public static string ExportCsvToBase64String<T>(List<T> datas)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(datas);
                }
            }
            byte[] bytes = stream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        public static string ExportCsvToBase64String(IList datas)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(datas);
                }
            }
            byte[] bytes = stream.ToArray();
            return Convert.ToBase64String(bytes);
        }
        public static void ExportExcelToLoacl(DataTable dt, string file,bool withHeader)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                fileInfo = new FileInfo(file);
            }

            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage(fileInfo);
            // 添加一个 sheet 表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);
            int rowIndex = 1;   // 起始行为 1
            int colIndex = 1;   // 起始列为 1
            if (withHeader)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[rowIndex, colIndex + i].Value = dt.Columns[i].ColumnName;
                    //自动调整列宽，也可以指定最小宽度和最大宽度
                    worksheet.Column(colIndex + i).AutoFit();
                }
                rowIndex = 2;
            }

            //// 跳过第一列列名
            //rowIndex++;
            //写入数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
                }
                //自动调整行高
                worksheet.Row(rowIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            worksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            worksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //单元格背景样式，要设置背景颜色必须先设置背景样式
            //worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //单元格背景颜色
            //worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
            //设置单元格所有边框样式和颜色
            //worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#0097DD"));
            //单独设置单元格四边框 Top、Bottom、Left、Right 的样式和颜色
            //worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            worksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            worksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;
            package.Save();
            worksheet.Dispose();
            package.Dispose();
        }
        
        public static void ExportExcelToLoacl(DataSet ds, string file, bool withHeader)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
                fileInfo = new FileInfo(file);
            }

            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage(fileInfo);
            foreach (DataTable dt in ds.Tables)
            {
                // 添加一个 sheet 表
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);
                int rowIndex = 1;   // 起始行为 1
                int colIndex = 1;   // 起始列为 1
                if (withHeader)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        worksheet.Cells[rowIndex, colIndex + i].Value = dt.Columns[i].ColumnName;
                        //自动调整列宽，也可以指定最小宽度和最大宽度
                        worksheet.Column(colIndex + i).AutoFit();
                    }
                    rowIndex = 2;
                }

                //// 跳过第一列列名
                //rowIndex++;
                //写入数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
                    }
                    //自动调整行高
                    worksheet.Row(rowIndex + i).CustomHeight = true;
                }

                //设置字体，也可以是中文，比如：宋体
                worksheet.Cells.Style.Font.Name = "Arial";
                //字体大小
                worksheet.Cells.Style.Font.Size = 12;
                //字体颜色
                worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                //单元格背景样式，要设置背景颜色必须先设置背景样式
                //worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //单元格背景颜色
                //worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
                //设置单元格所有边框样式和颜色
                //worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#0097DD"));
                //单独设置单元格四边框 Top、Bottom、Left、Right 的样式和颜色
                //worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                //垂直居中
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //水平居中
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //单元格是否自动换行
                worksheet.Cells.Style.WrapText = false;
                //设置单元格格式为文本
                worksheet.Cells.Style.Numberformat.Format = "@";
                //单元格自动适应大小
                worksheet.Cells.Style.ShrinkToFit = true;

                //worksheet.Dispose();
            }
            package.Save();
            package.Dispose();
        }

        public static string SkuWoReportNewExportExcel(Dictionary<string, DataTable> tables,List<string> fixedStation)
        {

            //ADD WO SUMMARY REQUEST BY LHJ LY  ADD BY HGB 2021.11.20
            #region WO SUMMARY
            //遍歷tables 生成 WO SUMMARY table 
            List<string> SumarrynewStation_711 = new List<string>();
            List<string> SumarrynewStation_750 = new List<string>();
            List<string> SumarrynewStation_DOF = new List<string>();
            SumarrynewStation_711.Add("SMT_711");
            SumarrynewStation_711.Add("PTH_711");
            SumarrynewStation_711.Add("ICT_711");
            SumarrynewStation_750.Add("MA_750");
            SumarrynewStation_750.Add("TEST_750");
            SumarrynewStation_DOF.Add("ASSEMBLY_DOF");
            SumarrynewStation_DOF.Add("TEST_DOF");
            SumarrynewStation_DOF.Add("PACKOUT_DOF");

           
                DataTable resdt_temp = null;
             
                resdt_temp = new DataTable();
               

                //add title col增加標題
                resdt_temp.Columns.Add("SKU");              
                resdt_temp.Columns.Add("QTY");
                resdt_temp.Columns.Add("BALANCE");

                //resdt_temp.Columns.Add("711");
                resdt_temp.Columns.Add("SMT_711");
                resdt_temp.Columns.Add("PTH_711");
                resdt_temp.Columns.Add("ICT_711");

                //resdt_temp.Columns.Add("750/750/740");
                resdt_temp.Columns.Add("MA_750");
                resdt_temp.Columns.Add("TEST_750");

                //resdt_temp.Columns.Add("DOF");
                resdt_temp.Columns.Add("ASSEMBLY_DOF");
                resdt_temp.Columns.Add("TEST_DOF");
                resdt_temp.Columns.Add("PACKOUT_DOF");

                resdt_temp.Columns.Add("FailWip");
                resdt_temp.Columns.Add("RepairWip");
                resdt_temp.Columns.Add("MRB");
                resdt_temp.Columns.Add("REWORK");
                resdt_temp.Columns.Add("ORT");
                resdt_temp.Columns.Add("SCRAPED");
                resdt_temp.Columns.Add("JOBFINISH");
                resdt_temp.Columns.Add("WHS(SM)");//2021.11.23 ADD BY HGB  
                resdt_temp.Columns.Add("SILVERWIP");//2021.11.23 ADD BY HGB
                resdt_temp.Columns.Add("TRANSFORMATION");//2021.11.23 ADD BY HGB  
                


            //Input value 循環給WoSummary賦值

            string IsNotNomalStation = "SMT_711,PTH_711,ICT_711,MA_750,TEST_750,ASSEMBLY_DOF,TEST_DOF,PACKOUT_DOF,WORKORDERNO,SKUNO,SKU,GROUPID,DATS,VER,QTY,BALANCE,FailWip,RepairWip,MRB,REWORK,ORT,SCRAPED,JOBFINISH,WHS(SM),SILVERWIP,TRANSFORMATION";
            //resdt_temp = tables["WoSummary"];

            //便歷ALLTABLE
            Int64 lasttb = 0;
            Int64 QTY = 0;
            Int64 BALANCE = 0;
            Int64 SMT = 0;
            Int64 PTH = 0;
            Int64 ICT = 0;

            Int64 MA = 0;
            Int64 TEST = 0;

            Int64 ASSEMBLY = 0;
            Int64 DOFTEST = 0;
            Int64 PACKOUT = 0;

            Int64 FailWip = 0;
            Int64 RepairWip = 0;
            Int64 MRB = 0;
            Int64 REWORK = 0;
            Int64 ORT = 0;
            Int64 SCRAPED = 0;
            Int64 JOBFINISH = 0;
            Int64 SILVERWIP = 0;
            Int64 TRANSFORMATION = 0;
            Int64 WHS = 0;
            string SKUNO = string.Empty;
            foreach (var item in tables)
            {
                string SKUNOI = string.Empty;
                lasttb = lasttb + 1;

                DataTable dt = item.Value;
                dt.TableName = item.Key;
                 

                //前提,同一個機種不會在兩個TABLE同時存在,目前符合2021.11.20
                //便歷TABLE
                

                Int64 lastcol = 0;
               
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    lastcol = lastcol + 1;
                    //便歷col
                   
                    Int64 j_temp = 0;

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        j_temp = j;
                        string colname = dt.Columns[j].ColumnName;
                        string value = dt.Rows[i][j].ToString();
                        if (string.IsNullOrEmpty(value))
                        {
                            value = "0";
                        }
                         

                        if (colname == "SKUNO")
                        {
                            SKUNOI = value;
                        }
                        else if (colname == "QTY")
                        {
                            QTY = QTY + Convert.ToInt64(value);
                        }
                        else if (colname == "BALANCE")
                        {
                            BALANCE = BALANCE + Convert.ToInt64(value);
                        }
                        else if (colname == "FailWip")
                        {
                            FailWip = FailWip + Convert.ToInt64(value);
                        }

                        else if (colname == "RepairWip")
                        {
                            RepairWip = RepairWip + Convert.ToInt64(value);
                        }
                        else if (colname == "MRB")
                        {
                            MRB = MRB + Convert.ToInt64(value);
                        }
                        else if (colname == "ORT")
                        {
                            ORT = ORT + Convert.ToInt64(value);
                        }
                        else if (colname == "SCRAPED")
                        {
                            SCRAPED = SCRAPED + Convert.ToInt64(value);
                        }
                        else if (colname == "JOBFINISH")
                        {
                            JOBFINISH = JOBFINISH + Convert.ToInt64(value);
                        }
                        else if (colname == "SILVERWIP")
                        {
                            SILVERWIP = SILVERWIP + Convert.ToInt64(value);
                        }
                        else if (colname == "TRANSFORMATION")
                        {
                            TRANSFORMATION = TRANSFORMATION + Convert.ToInt64(value);
                        }
                        else if (colname == "WHS(SM)")
                        {
                            WHS = WHS + Convert.ToInt64(value);
                        }
                        

                        //工站數據
                        if (!IsNotNomalStation.Contains(colname))
                        {
                            if (SKUNOI.StartsWith("711"))
                            {

                                if (colname.StartsWith("SMT") || colname == "SUPPLIER-SN")
                                {
                                    SMT = SMT + Convert.ToInt64(value);
                                }

                                else if (colname == "PTH")
                                {
                                    PTH = PTH + Convert.ToInt64(value);
                                }
                                else //NOT Above
                                {
                                    ICT = ICT + Convert.ToInt64(value);
                                }
                            }
                            else if (SKUNOI.StartsWith("750") || SKUNOI.StartsWith("740") || SKUNOI.StartsWith("760"))
                            {
                                //not in test
                                if (!fixedStation.Contains(colname) )
                                {
                                    MA = MA + Convert.ToInt64(value);
                                }

                                else
                                {
                                    TEST = TEST + Convert.ToInt64(value);
                                }

                            }
                            else
                            {
                                if (colname.StartsWith("MA")|| colname.StartsWith("ASSY") )
                                {
                                    ASSEMBLY = ASSEMBLY + Convert.ToInt64(value);
                                }

                                else if (fixedStation.Contains(colname))
                                {
                                    DOFTEST = DOFTEST + Convert.ToInt64(value);
                                }
                                else //NOT Above
                                {
                                    try
                                    {
                                        PACKOUT = PACKOUT + Convert.ToInt64(value);
                                    }
                                    catch
                                    {
                                        string aa = value;
                                    }
                                }
                            }
                        }

                        //取完一行數據開始對比機種是否一樣,不一樣怎新表增加新行get col complete than begin to compare skuno
                        //機種不一樣或者是最後一行，開始重組新行
                        //SKUNO SI NOT THE SAME AND IS NOT NULL  
                        if (SKUNO != SKUNOI && !string.IsNullOrEmpty(SKUNO))
                        {
                            DataRow drd = resdt_temp.NewRow();
                            resdt_temp.Rows.Add(drd);
                            drd["SKU"] = SKUNO;
                            drd["QTY"] = QTY.ToString();
                            drd["BALANCE"] = BALANCE.ToString();
                            drd["FailWip"] = FailWip.ToString();
                            drd["RepairWip"] = RepairWip.ToString();
                            drd["MRB"] = MRB.ToString();
                            drd["REWORK"] = REWORK.ToString();
                            drd["ORT"] = ORT.ToString();
                            drd["SCRAPED"] = SCRAPED.ToString();
                            drd["JOBFINISH"] = JOBFINISH.ToString();
                            drd["WHS(SM)"] = WHS.ToString();
                            drd["SILVERWIP"] = SILVERWIP.ToString();
                            drd["TRANSFORMATION"] = TRANSFORMATION.ToString();

                            drd["SMT_711"] = SMT.ToString();
                            drd["PTH_711"] = PTH.ToString();
                            drd["ICT_711"] = ICT.ToString();
                            drd["MA_750"] = MA.ToString();
                            drd["TEST_750"] = TEST.ToString();
                            drd["ASSEMBLY_DOF"] = ASSEMBLY.ToString();
                            drd["TEST_DOF"] = DOFTEST.ToString();
                            drd["PACKOUT_DOF"] = PACKOUT.ToString();

                            lasttb = 0;
                            QTY = 0;
                            BALANCE = 0;
                            SMT = 0;
                            PTH = 0;
                            ICT = 0;

                            MA = 0;
                            TEST = 0;

                            ASSEMBLY = 0;
                            DOFTEST = 0;
                            PACKOUT = 0;

                            FailWip = 0;
                            RepairWip = 0;
                            MRB = 0;
                            REWORK = 0;
                            ORT = 0;
                            SCRAPED = 0;
                            JOBFINISH = 0;
                            SILVERWIP = 0;
                            TRANSFORMATION = 0;
                            WHS = 0;

                        }
                        SKUNO = SKUNOI;
                        // if (lastcol == dt.Rows.Count && dt.TableName != "WoSummary")
                        if (lastcol == dt.Rows.Count && lasttb == tables.Count && dt.TableName != "WoSummary")
                        {
                            DataRow drd = resdt_temp.NewRow();
                            resdt_temp.Rows.Add(drd);
                            drd["SKU"] = SKUNO;
                            drd["QTY"] = QTY.ToString();
                            drd["BALANCE"] = BALANCE.ToString();
                            drd["FailWip"] = FailWip.ToString();
                            drd["RepairWip"] = RepairWip.ToString();
                            drd["MRB"] = MRB.ToString();
                            drd["REWORK"] = REWORK.ToString();
                            drd["ORT"] = ORT.ToString();
                            drd["SCRAPED"] = SCRAPED.ToString();
                            drd["JOBFINISH"] = JOBFINISH.ToString();
                            drd["WHS(SM)"] = WHS.ToString();
                            drd["SILVERWIP"] = SILVERWIP.ToString();
                            drd["TRANSFORMATION"] = TRANSFORMATION.ToString();


                            drd["SMT_711"] = SMT.ToString();
                            drd["PTH_711"] = PTH.ToString();
                            drd["ICT_711"] = ICT.ToString();
                            drd["MA_750"] = MA.ToString();
                            drd["TEST_750"] = TEST.ToString();
                            drd["ASSEMBLY_DOF"] = ASSEMBLY.ToString();
                            drd["TEST_DOF"] = DOFTEST.ToString();
                            drd["PACKOUT_DOF"] = PACKOUT.ToString();

                            lasttb = 0;
                            QTY = 0;
                            BALANCE = 0;
                            SMT = 0;
                            PTH = 0;
                            ICT = 0;

                            MA = 0;
                            TEST = 0;

                            ASSEMBLY = 0;
                            DOFTEST = 0;
                            PACKOUT = 0;

                            FailWip = 0;
                            RepairWip = 0;
                            MRB = 0;
                            REWORK = 0;
                            ORT = 0;
                            SCRAPED = 0;
                            JOBFINISH = 0;
                            SILVERWIP = 0;
                            TRANSFORMATION = 0;
                            WHS = 0;
                        }
                    }


                    

                }
            }

            #region begin把wosummary移到第一頁         
            Dictionary<string, DataTable> table_temp = new Dictionary<string, DataTable>();
            table_temp.Add("WoSummary", resdt_temp);
            foreach (var item in tables)
            {
                table_temp.Add(item.Key, item.Value);
            }
            tables = table_temp;
            #endregion 把wosummary移到第一頁

            #endregion WO SUMMARY
            //END ADD  WO SUMMARY REQUEST BY LHJ LY  ADD BY HGB  ADD BY HGB 2021.11.20


            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage();

            foreach (var item in tables)
            {

                DataTable dt = item.Value;
                dt.TableName = item.Key;

                // 添加一个 sheet 表
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);
                int rowIndex = 1;   // 起始行为 1
                int colIndex = 1;   // 起始列为 1
                                    //设置列名
                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    string columnsName= dt.Columns[i].ColumnName;
                    string sation = "";
                    bool bFixed = false;
                    foreach (var s in fixedStation)
                    {
                        if(columnsName== s+"_W")
                        {
                            bFixed = true;
                            sation = s;
                            break;
                        }
                    }

                    //BEGIN ADD  WO SUMMARY REQUEST BY LHJ LY  ADD BY HGB  ADD BY HGB 2021.11.20
                    #region excel merge for wo summary
                    bool bFixed_711 = false;
                    foreach (var s in SumarrynewStation_711)
                    {
                        if (columnsName == s)
                        {
                            bFixed_711 = true;
                            sation = s;
                            break;
                        }
                    }

                    bool bFixed_750 = false;
                    foreach (var s in SumarrynewStation_750)
                    {
                        if (columnsName == s)
                        {
                            bFixed_750 = true;
                            sation = s;
                            break;
                        }
                    }


                    bool bFixed_DOF = false;
                    foreach (var s in SumarrynewStation_DOF)
                    {
                        if (columnsName == s)
                        {
                            bFixed_DOF = true;
                            sation = s;
                            break;
                        }
                    }




                    if (bFixed_711)
                    {
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Merge = true;
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Value = "711";
                        worksheet.Cells[2, colIndex + i].Value = "SMT";
                        worksheet.Cells[2, colIndex + i + 1].Value = "PTH";
                        worksheet.Cells[2, colIndex + i + 2].Value = "ICT";
                        


                        //設置樣式
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //設置背景顏色
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                        worksheet.Cells[2, colIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                        worksheet.Cells[2, colIndex + i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                        worksheet.Cells[2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                        //設置整列顏色
                        worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                        i = i + 2;
                    }
                    else if (bFixed_750)
                    {
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 1].Merge = true;
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 1].Value = "740/750/760";
                        worksheet.Cells[2, colIndex + i].Value = "MA";
                        worksheet.Cells[2, colIndex + i + 1].Value = "TEST";


                        //設置樣式
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //設置背景顏色
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                        worksheet.Cells[2, colIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                        worksheet.Cells[2, colIndex + i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                        worksheet.Cells[2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);

                        //設置整列顏色
                        worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);

                        i = i + 1;

                    }
                    else if (bFixed_DOF)
                    {
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Merge = true;
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Value = "DOF";
                        worksheet.Cells[2, colIndex + i].Value = "ASSEMBLY";
                        worksheet.Cells[2, colIndex + i + 1].Value = "TEST";
                        worksheet.Cells[2, colIndex + i + 2].Value = "PACKOUT";
                       


                        //設置樣式
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //設置背景顏色
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Goldenrod);
                        worksheet.Cells[2, colIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Goldenrod);
                        worksheet.Cells[2, colIndex + i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Goldenrod);
                        worksheet.Cells[2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Goldenrod);


                        //設置整列顏色
                        worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Goldenrod);

                        i = i + 2;

                    }
                    #endregion  excel merge for wo summary
                    //END ADD  WO SUMMARY REQUEST BY LHJ LY  ADD BY HGB  ADD BY HGB 2021.11.20


                    else if (bFixed)
                    {
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Merge = true;
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Value = sation;
                        //worksheet.Cells[2, colIndex + i].Value = sation + "_W";
                        //worksheet.Cells[2, colIndex + i+1].Value = sation + "_F";
                        //worksheet.Cells[2, colIndex + i+2].Value = sation + "_R";
                        //2021.11.23 MODIFY AS BELOW BY HGB
                        worksheet.Cells[2, colIndex + i].Value =  "W";
                        worksheet.Cells[2, colIndex + i + 1].Value =   "F";
                        worksheet.Cells[2, colIndex + i + 2].Value =   "R";

                        //設置樣式
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        //設置背景顏色
                        worksheet.Cells[1, colIndex + i, 1, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                        worksheet.Cells[2, colIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                        worksheet.Cells[2, colIndex + i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                        worksheet.Cells[2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);



                        //設置整列顏色
                        worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);

                        i = i + 2;
                    }
                    else
                    {
                        List<string> SumarrynewStation_JO = new List<string>();
                        SumarrynewStation_JO.Add("WHS(SM)");
                        SumarrynewStation_JO.Add("JOBFINISH");
                        SumarrynewStation_JO.Add("SILVERWIP");
                        SumarrynewStation_JO.Add("TRANSFORMATION");

                        List<string> SumarrynewStation_RO = new List<string>();
                        SumarrynewStation_RO.Add("FailWip");
                        SumarrynewStation_RO.Add("RepairWip");
                        SumarrynewStation_RO.Add("MRB");
                        SumarrynewStation_RO.Add("REWORK");




                        if (SumarrynewStation_JO.Contains(columnsName))
                        {
                            //設置樣式
                            worksheet.Cells[1, colIndex + i, 2, colIndex + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            //設置背景顏色
                            worksheet.Cells[1, colIndex + i, 2, colIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LimeGreen);

                            //設置整列顏色
                            worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LimeGreen);

                        }
                        else if (SumarrynewStation_RO.Contains(columnsName))
                        {
                            //設置樣式
                            worksheet.Cells[1, colIndex + i, 2, colIndex + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            //設置背景顏色
                            worksheet.Cells[1, colIndex + i, 2, colIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);

                            //設置整列顏色
                            worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                        }
                        else
                        {
                            //設置樣式
                            worksheet.Cells[1, colIndex + i, 2, colIndex + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            //設置背景顏色
                            worksheet.Cells[1, colIndex + i, 2, colIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DeepSkyBlue);

                            //設置整列顏色
                            worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, colIndex + i, dt.Rows.Count + 2, colIndex + i + 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DeepSkyBlue);

                        }


                        //worksheet.Cells[rowIndex, colIndex + i].Value = columnsName;
                        worksheet.Cells[1, colIndex + i, 2, colIndex + i].Merge = true;
                        worksheet.Cells[1, colIndex + i, 2, colIndex + i ].Value = columnsName;
                        
                        //自动调整列宽，也可以指定最小宽度和最大宽度
                        worksheet.Column(colIndex + i).AutoFit();
                    }
                    
                }

                // 跳过第一列列名
                //rowIndex++;
                rowIndex = rowIndex + 2;
                //写入数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
                    }
                    //自动调整行高
                    worksheet.Row(rowIndex + i).CustomHeight = true;
                }

                //设置字体，也可以是中文，比如：宋体
                worksheet.Cells.Style.Font.Name = "Arial";
                //字体大小
                worksheet.Cells.Style.Font.Size = 12;
                //字体颜色
                worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                //单元格背景样式，要设置背景颜色必须先设置背景样式
                //worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //单元格背景颜色
                //worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
                //设置单元格所有边框样式和颜色
                //worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.ColorTranslator.FromHtml("#0097DD"));
                //单独设置单元格四边框 Top、Bottom、Left、Right 的样式和颜色
                worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);

                worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);

                worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);

                worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                //垂直居中
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //水平居中
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //单元格是否自动换行
                worksheet.Cells.Style.WrapText = false;
                //设置单元格格式为文本
                worksheet.Cells.Style.Numberformat.Format = "@";
                //单元格自动适应大小
                worksheet.Cells.Style.ShrinkToFit = true;
                //worksheet.Dispose();
            }
            //创建一个内存流，然后转换为字节数组，输出到浏览器下载
            MemoryStream ms = new MemoryStream();
            package.SaveAs(ms);
            byte[] bytes = ms.ToArray();            
            package.Dispose();
            return Convert.ToBase64String(bytes);
        }
    
        public static string FJZSkuWipReportExportExcel(DataTable pcbaTable,DataTable modelTable, DataTable backlogTable, DataTable pcbaSkuTable, DataTable modelSkuTable, 
            List<string> pcbaSMTColumns, List<string> pcbaICTColumns, List<string> pcbaPTHColumns, 
            List<string> redColumns, List<string> modelYellowColumns, List<string> modelBlueColumns,
            Dictionary<string, List<string>> dicHighPriority,DataTable firstPCBAData, DataTable secondPCBAData, DataTable firstModelData, DataTable secondModelData,
           DataTable firstDOFData, DataTable secondDOFData, DataTable firstCTOData, DataTable secondCTOData,
            DataTable testInTable, DataTable testOutTable,List<string> inOutStation, DataTable dofData, DataTable dofSkuData,DataTable ctoData, DataTable ctoSkuData
            )
        { 

            ColorConverter colorConver = new ColorConverter();           
            #region 711 color
            // SMT #7030a0            
            Color smtColor = (Color)colorConver.ConvertFromString("#7030a0");
            // PTH #ddebf7            
            Color pthColor= (Color)colorConver.ConvertFromString("#ddebf7");
            // ICT #c6e0b4            
            Color ictColor = (Color)colorConver.ConvertFromString("#c6e0b4");
            #endregion

            #region 750 color            
            //#87ceeb
            Color blueColor = (Color)colorConver.ConvertFromString("#87ceeb");
            #endregion

            ExcelPackage package = new ExcelPackage();
            #region 711 worksheet
            ExcelWorksheet pcbaWorksheet = package.Workbook.Worksheets.Add("WIP 711");
            int pcbaRowIndex = 1;   // 起始行为 1
            int pcbaColIndex = 1;   // 起始列为 1
            int pcbaTotalRow = 0;
            for (int i = 0; i < pcbaTable.Columns.Count; i++)
            {
                pcbaWorksheet.Cells[pcbaRowIndex, pcbaColIndex + i].Value = pcbaTable.Columns[i].ColumnName;
                pcbaWorksheet.Cells[pcbaRowIndex, pcbaColIndex + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (pcbaSMTColumns.Contains(pcbaTable.Columns[i].ColumnName.ToUpper()))
                {
                    pcbaWorksheet.Cells[pcbaRowIndex, pcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(smtColor);
                }
                else if (pcbaPTHColumns.Contains(pcbaTable.Columns[i].ColumnName.ToUpper()))
                {
                    pcbaWorksheet.Cells[pcbaRowIndex, pcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(pthColor);
                }
                else if (pcbaICTColumns.Contains(pcbaTable.Columns[i].ColumnName.ToUpper()))
                {
                    pcbaWorksheet.Cells[pcbaRowIndex, pcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(ictColor);
                }
                else if (redColumns.Contains(pcbaTable.Columns[i].ColumnName.ToUpper()))
                {
                    pcbaWorksheet.Cells[pcbaRowIndex, pcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(Color.Red);
                }
                else
                {
                    pcbaWorksheet.Cells[pcbaRowIndex, pcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(Color.White);
                }
                //自动调整列宽，也可以指定最小宽度和最大宽度
                pcbaWorksheet.Column(pcbaColIndex + i).AutoFit();
                
            }
            // 跳过第一列列名
            pcbaRowIndex++;
            pcbaTotalRow++;
            //写入数据
            for (int i = 0; i < pcbaTable.Rows.Count; i++)
            {
                for (int j = 0; j < pcbaTable.Columns.Count; j++)
                {
                    if (j == 0 || j == 1 || j == 2)
                    {
                        pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Value = pcbaTable.Rows[i][j].ToString();
                    }
                    else
                    {
                        if(string.IsNullOrWhiteSpace(pcbaTable.Rows[i][j].ToString())&& (i< pcbaTable.Rows.Count-2))
                        {
                            pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Value = 0;
                        }
                        else
                        {
                            try
                            {
                                pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Value = Convert.ToInt32(pcbaTable.Rows[i][j].ToString());
                            }
                            catch (Exception)
                            {
                                pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Value = pcbaTable.Rows[i][j].ToString();
                            }
                        }                        
                    }                    
                    pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (pcbaSMTColumns.Contains(pcbaTable.Columns[j].ColumnName.ToUpper()))
                    {
                        pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(smtColor);
                    }
                    else if (pcbaPTHColumns.Contains(pcbaTable.Columns[j].ColumnName.ToUpper()))
                    {
                        pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(pthColor);
                    }
                    else if (pcbaICTColumns.Contains(pcbaTable.Columns[j].ColumnName.ToUpper()))
                    {
                        pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(ictColor);
                    }
                    else if (redColumns.Contains(pcbaTable.Columns[j].ColumnName.ToUpper()))
                    {
                        pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    }
                    else
                    {
                        pcbaWorksheet.Cells[pcbaRowIndex + i, pcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(Color.White);
                    }
                }
                //自动调整行高
                pcbaWorksheet.Row(pcbaRowIndex + i).CustomHeight = true;
                pcbaTotalRow++;
            }
            DataRow pcbaListRow = pcbaTable.Rows[pcbaTable.Rows.Count - 1];
            //total smt 8 -12
            int startColIndex = 8;
            int smtFirstIndex = startColIndex;// pcbaTable.Columns.IndexOf(pcbaSMTColumns.First()) + 1;
            int smtLastIndex = startColIndex + pcbaSMTColumns.Count-1 ; //pcbaTable.Columns.IndexOf(pcbaSMTColumns.Last()) + 1;
            string smtTotal = "SMT Total:" + pcbaListRow[pcbaSMTColumns.Last()].ToString();
            pcbaWorksheet.Cells[pcbaTotalRow, smtFirstIndex, pcbaTotalRow, smtLastIndex].Merge = true;
            pcbaWorksheet.Cells[pcbaTotalRow, smtFirstIndex, pcbaTotalRow, smtLastIndex].Value = smtTotal;
            pcbaWorksheet.Cells[pcbaTotalRow, smtFirstIndex, pcbaTotalRow, smtLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            pcbaWorksheet.Cells[pcbaTotalRow, smtFirstIndex, pcbaTotalRow, smtLastIndex].Style.Fill.BackgroundColor.SetColor(smtColor);

            //total pth 13 -17
            int pthFirstIndex = smtLastIndex + 1; //pcbaTable.Columns.IndexOf(pcbaPTHColumns.First()) + 1;
            int pthLastIndex = pthFirstIndex + pcbaPTHColumns.Count-1; //pcbaTable.Columns.IndexOf(pcbaPTHColumns.Last()) + 1;
            string pthTotal = "PTH Total:" + pcbaListRow[pcbaPTHColumns.Last()].ToString();
            pcbaWorksheet.Cells[pcbaTotalRow, pthFirstIndex, pcbaTotalRow, pthLastIndex].Merge = true;
            pcbaWorksheet.Cells[pcbaTotalRow, pthFirstIndex, pcbaTotalRow, pthLastIndex].Value = pthTotal;
            pcbaWorksheet.Cells[pcbaTotalRow, pthFirstIndex, pcbaTotalRow, pthLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            pcbaWorksheet.Cells[pcbaTotalRow, pthFirstIndex, pcbaTotalRow, pthLastIndex].Style.Fill.BackgroundColor.SetColor(pthColor);

            //total ict 18 -20
            int ictFirstIndex = pthLastIndex+1; //pcbaTable.Columns.IndexOf(pcbaICTColumns.First()) + 1;
            int ictLastIndex = ictFirstIndex + pcbaICTColumns.Count-1; //pcbaTable.Columns.IndexOf(pcbaICTColumns.Last()) + 1;
            string ictTotal = "ICT Total:" + pcbaListRow[pcbaICTColumns.Last()].ToString();
            pcbaWorksheet.Cells[pcbaTotalRow, ictFirstIndex, pcbaTotalRow, ictLastIndex].Merge = true;
            pcbaWorksheet.Cells[pcbaTotalRow, ictFirstIndex, pcbaTotalRow, ictLastIndex].Value =  ictTotal;
            pcbaWorksheet.Cells[pcbaTotalRow, ictFirstIndex, pcbaTotalRow, ictLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            pcbaWorksheet.Cells[pcbaTotalRow, ictFirstIndex, pcbaTotalRow, ictLastIndex].Style.Fill.BackgroundColor.SetColor(ictColor);

            //bonepile 21 -24
            int bonepileFirstIndex = ictLastIndex + 1; //pcbaTable.Columns.IndexOf(redColumns.First()) + 1;
            int bonepileLastIndex = bonepileFirstIndex + redColumns.Count-1;// pcbaTable.Columns.IndexOf(redColumns.Last()) + 1;
            string bonepileTotal = "Total:" + pcbaListRow[redColumns.Last()].ToString();            
            pcbaWorksheet.Cells[pcbaTotalRow, bonepileFirstIndex, pcbaTotalRow, bonepileLastIndex].Merge = true;
            pcbaWorksheet.Cells[pcbaTotalRow, bonepileFirstIndex, pcbaTotalRow, bonepileLastIndex].Value =  bonepileTotal;
            pcbaWorksheet.Cells[pcbaTotalRow, bonepileFirstIndex, pcbaTotalRow, bonepileLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            pcbaWorksheet.Cells[pcbaTotalRow, bonepileFirstIndex, pcbaTotalRow, bonepileLastIndex].Style.Fill.BackgroundColor.SetColor(Color.Red);


            //设置字体，也可以是中文，比如：宋体
            pcbaWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            pcbaWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            pcbaWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            pcbaWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            pcbaWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            pcbaWorksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            pcbaWorksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            pcbaWorksheet.Cells.Style.ShrinkToFit = true;
            pcbaWorksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            #endregion

            #region 750 worksheet
            ExcelWorksheet modelWorksheet = package.Workbook.Worksheets.Add("WIP 750");
            int modelRowIndex = 1;   // 起始行为 1
            int modelColIndex = 1;   // 起始列为 1
            int modelTotalRow = 0;
            for (int i = 0; i < modelTable.Columns.Count; i++)
            {
                modelWorksheet.Cells[modelRowIndex, modelColIndex + i].Value = modelTable.Columns[i].ColumnName;
                modelWorksheet.Cells[modelRowIndex, modelColIndex + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (modelYellowColumns.Contains(modelTable.Columns[i].ColumnName.ToUpper()))
                {
                    modelWorksheet.Cells[modelRowIndex, modelColIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                }
                else if (modelBlueColumns.Contains(modelTable.Columns[i].ColumnName.ToUpper()))
                {
                    modelWorksheet.Cells[modelRowIndex, modelColIndex + i].Style.Fill.BackgroundColor.SetColor(blueColor);
                }
                else if (redColumns.Contains(modelTable.Columns[i].ColumnName.ToUpper()))
                {
                    modelWorksheet.Cells[modelRowIndex, modelColIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                }
                else
                {
                    modelWorksheet.Cells[modelRowIndex, modelColIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                }
                //自动调整列宽，也可以指定最小宽度和最大宽度
                modelWorksheet.Column(modelColIndex + i).AutoFit();
            }
            // 跳过第一列列名
            modelRowIndex++;
            modelTotalRow++;
            //写入数据
            for (int i = 0; i < modelTable.Rows.Count; i++)
            {
                for (int j = 0; j < modelTable.Columns.Count; j++)
                {
                    if (j == 0 || j == 1 || j == 2)
                    {
                        modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Value = modelTable.Rows[i][j].ToString();
                    }
                    else
                    {
                        if(string.IsNullOrWhiteSpace(modelTable.Rows[i][j].ToString()) && i< modelTable.Rows.Count-2)
                        {
                            modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Value = 0;
                        }
                        else
                        {
                            try
                            {
                                modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Value =  Convert.ToInt32(modelTable.Rows[i][j].ToString());
                            }
                            catch (Exception)
                            {
                                modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Value = modelTable.Rows[i][j].ToString();
                            }
                        }                        
                    }
                    modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (modelYellowColumns.Contains(modelTable.Columns[j].ColumnName.ToUpper()))
                    {
                        modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    }
                    else if (modelBlueColumns.Contains(modelTable.Columns[j].ColumnName.ToUpper()))
                    {
                        modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Style.Fill.BackgroundColor.SetColor(blueColor);
                    }
                    else if (redColumns.Contains(modelTable.Columns[j].ColumnName.ToUpper()))
                    {
                        modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    }
                    else
                    {
                        modelWorksheet.Cells[modelRowIndex + i, modelColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    }
                }
                //自动调整行高
                modelWorksheet.Row(modelRowIndex + i).CustomHeight = true;
                modelTotalRow++;
            }
            DataRow modelListRow = modelTable.Rows[modelTable.Rows.Count - 1];
            int yellowFirstIndex = startColIndex; //modelTable.Columns.IndexOf(modelYellowColumns.First()) + 1;
            int yellowLastIndex = yellowFirstIndex + modelYellowColumns.Count - 1;// modelTable.Columns.IndexOf(modelYellowColumns.Last()) + 1;
            string yellowTotal = "Total:" + modelListRow[modelYellowColumns.Last()].ToString();
            modelWorksheet.Cells[modelTotalRow, yellowFirstIndex, modelTotalRow, yellowLastIndex].Merge = true;
            modelWorksheet.Cells[modelTotalRow, yellowFirstIndex, modelTotalRow, yellowLastIndex].Value = yellowTotal;
            modelWorksheet.Cells[modelTotalRow, yellowFirstIndex, modelTotalRow, yellowLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            modelWorksheet.Cells[modelTotalRow, yellowFirstIndex, modelTotalRow, yellowLastIndex].Style.Fill.BackgroundColor.SetColor(Color.Yellow);


            int blueFirstIndex = yellowLastIndex + 1;// modelTable.Columns.IndexOf(modelBlueColumns.First()) + 1;
            int blueLastIndex = blueFirstIndex + modelBlueColumns.Count - 1; //modelTable.Columns.IndexOf(modelBlueColumns.Last()) + 1;
            string blueTotal = "Total:" + modelListRow[modelBlueColumns.Last()].ToString();
            modelWorksheet.Cells[modelTotalRow, blueFirstIndex, modelTotalRow, blueLastIndex].Merge = true;
            modelWorksheet.Cells[modelTotalRow, blueFirstIndex, modelTotalRow, blueLastIndex].Value = blueTotal;
            modelWorksheet.Cells[modelTotalRow, blueFirstIndex, modelTotalRow, blueLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            modelWorksheet.Cells[modelTotalRow, blueFirstIndex, modelTotalRow, blueLastIndex].Style.Fill.BackgroundColor.SetColor(blueColor);

            int redFirstIndex = blueLastIndex + 1;// modelTable.Columns.IndexOf(redColumns.First()) + 1;
            int redLastIndex = redFirstIndex + redColumns.Count - 1;// modelTable.Columns.IndexOf(redColumns.Last()) + 1;
            string redTotal = "Total:" + modelListRow[redColumns.Last()].ToString();                       
            modelWorksheet.Cells[modelTotalRow, redFirstIndex, modelTotalRow, redLastIndex].Merge = true;
            modelWorksheet.Cells[modelTotalRow, redFirstIndex, modelTotalRow, redLastIndex].Value = redTotal;
            modelWorksheet.Cells[modelTotalRow, redFirstIndex, modelTotalRow, redLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            modelWorksheet.Cells[modelTotalRow, redFirstIndex, modelTotalRow, redLastIndex].Style.Fill.BackgroundColor.SetColor(Color.Red);

            //设置字体，也可以是中文，比如：宋体
            modelWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            modelWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            modelWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            modelWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            modelWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            modelWorksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            modelWorksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            modelWorksheet.Cells.Style.ShrinkToFit = true;
            modelWorksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            #endregion

            #region DOF worksheet
            ExcelWorksheet dofWorksheet = package.Workbook.Worksheets.Add("WIP DOF");
            int dofRowIndex = 1;   // 起始行为 1
            int dofColIndex = 1;   // 起始列为 1
            //设置列名
            for (int i = 0; i < dofData.Columns.Count; i++)
            {
                dofWorksheet.Cells[dofRowIndex, dofColIndex + i].Value = dofData.Columns[i].ColumnName;
                //自动调整列宽，也可以指定最小宽度和最大宽度
                dofWorksheet.Column(dofColIndex + i).AutoFit();
            }

            // 跳过第一列列名
            dofRowIndex++;
            //写入数据
            for (int i = 0; i < dofData.Rows.Count; i++)
            {
                for (int j = 0; j < dofData.Columns.Count; j++)
                {                    
                    try
                    {
                        dofWorksheet.Cells[dofRowIndex + i, dofColIndex + j].Value = Convert.ToInt32(dofData.Rows[i][j].ToString());
                    }
                    catch (Exception)
                    {
                        dofWorksheet.Cells[dofRowIndex + i, dofColIndex + j].Value = dofData.Rows[i][j].ToString();
                    }
                }
                //自动调整行高
                dofWorksheet.Row(dofRowIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            dofWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            dofWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            dofWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            dofWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            dofWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            dofWorksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            dofWorksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            dofWorksheet.Cells.Style.ShrinkToFit = true;
            #endregion

            #region CTO worksheet
            ExcelWorksheet ctoWorksheet = package.Workbook.Worksheets.Add("WIP CTO");
            int ctoRowIndex = 1;   // 起始行为 1
            int ctoColIndex = 1;   // 起始列为 1
            //设置列名
            for (int i = 0; i < ctoData.Columns.Count; i++)
            {
                ctoWorksheet.Cells[ctoRowIndex, ctoColIndex + i].Value = ctoData.Columns[i].ColumnName;
                //自动调整列宽，也可以指定最小宽度和最大宽度
                ctoWorksheet.Column(ctoColIndex + i).AutoFit();
            }

            // 跳过第一列列名
            ctoRowIndex++;
            //写入数据
            for (int i = 0; i < ctoData.Rows.Count; i++)
            {
                for (int j = 0; j < ctoData.Columns.Count; j++)
                {
                    try
                    {
                        ctoWorksheet.Cells[ctoRowIndex + i, ctoColIndex + j].Value = Convert.ToInt32(ctoData.Rows[i][j].ToString());
                    }
                    catch (Exception)
                    {
                        ctoWorksheet.Cells[ctoRowIndex + i, ctoColIndex + j].Value = ctoData.Rows[i][j].ToString();
                    }
                }
                //自动调整行高
                ctoWorksheet.Row(ctoRowIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            ctoWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            ctoWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            ctoWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            ctoWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            ctoWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            ctoWorksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            ctoWorksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            ctoWorksheet.Cells.Style.ShrinkToFit = true;
            #endregion

            #region pcba Sku worksheet
            ExcelWorksheet PcbaWorksheet = package.Workbook.Worksheets.Add("711 SKU WIP");
            int PcbaRowIndex = 1;   // 起始行为 1
            int PcbaColIndex = 1;   // 起始列为 1
            int PcbaTotalRow = 0;
            for (int i = 0; i < pcbaSkuTable.Columns.Count; i++)
            {
                PcbaWorksheet.Cells[PcbaRowIndex, PcbaColIndex + i].Value = pcbaSkuTable.Columns[i].ColumnName;
                PcbaWorksheet.Cells[PcbaRowIndex, PcbaColIndex + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (pcbaSMTColumns.Contains(pcbaSkuTable.Columns[i].ColumnName.ToUpper()))
                {
                    PcbaWorksheet.Cells[PcbaRowIndex, PcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(Color.Purple);
                }
                else if (pcbaPTHColumns.Contains(pcbaSkuTable.Columns[i].ColumnName.ToUpper()))
                {
                    PcbaWorksheet.Cells[PcbaRowIndex, PcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                }
                else if (pcbaICTColumns.Contains(pcbaSkuTable.Columns[i].ColumnName.ToUpper()))
                {
                    PcbaWorksheet.Cells[PcbaRowIndex, PcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }
                else if (redColumns.Contains(pcbaSkuTable.Columns[i].ColumnName.ToUpper()))
                {
                    PcbaWorksheet.Cells[PcbaRowIndex, PcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                }
                else
                {
                    PcbaWorksheet.Cells[PcbaRowIndex, PcbaColIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                }
                //自动调整列宽，也可以指定最小宽度和最大宽度
                PcbaWorksheet.Column(PcbaColIndex + i).AutoFit();
            }
            // 跳过第一列列名
            PcbaRowIndex++;
            PcbaTotalRow++;
            //写入数据
            for (int i = 0; i < pcbaSkuTable.Rows.Count; i++)
            {
                for (int j = 0; j < pcbaSkuTable.Columns.Count; j++)
                {
                    if (j == 0)
                    {
                        PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Value = pcbaSkuTable.Rows[i][j].ToString();
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(pcbaSkuTable.Rows[i][j].ToString()) && i < pcbaSkuTable.Rows.Count - 1)
                        {
                            PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Value = 0;
                        }
                        else
                        {
                            try
                            {
                                PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Value = Convert.ToInt32(pcbaSkuTable.Rows[i][j].ToString());
                            }
                            catch (Exception)
                            {
                                PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Value = pcbaSkuTable.Rows[i][j].ToString();
                            }
                        }

                    }

                    PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (pcbaSMTColumns.Contains(pcbaSkuTable.Columns[j].ColumnName.ToUpper()))
                    {
                        PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);
                    }
                    else if (pcbaPTHColumns.Contains(pcbaSkuTable.Columns[j].ColumnName.ToUpper()))
                    {
                        PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    }
                    else if (pcbaICTColumns.Contains(pcbaSkuTable.Columns[j].ColumnName.ToUpper()))
                    {
                        PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                    }
                    else if (redColumns.Contains(pcbaSkuTable.Columns[j].ColumnName.ToUpper()))
                    {
                        PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    }
                    else
                    {
                        PcbaWorksheet.Cells[PcbaRowIndex + i, PcbaColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    }
                }
                //自动调整行高
                PcbaWorksheet.Row(PcbaRowIndex + i).CustomHeight = true;
                PcbaTotalRow++;
            }
            DataRow PcbaListRow = pcbaSkuTable.Rows[pcbaSkuTable.Rows.Count - 1];
            int skuColStartIndex = 4;
            int PcbaPurpleFirstIndex = skuColStartIndex;// pcbaSkuTable.Columns.IndexOf(pcbaSMTColumns.First()) + 1;
            int PcbaPurpleLastIndex = skuColStartIndex + pcbaSMTColumns.Count - 1;// pcbaSkuTable.Columns.IndexOf(pcbaSMTColumns.Last()) + 1;
            string PcbaYellowTotal = "Total:" + PcbaListRow[pcbaSMTColumns.Last()].ToString();
            PcbaWorksheet.Cells[PcbaTotalRow, PcbaPurpleFirstIndex, PcbaTotalRow, PcbaPurpleLastIndex].Merge = true;
            PcbaWorksheet.Cells[PcbaTotalRow, PcbaPurpleFirstIndex, PcbaTotalRow, PcbaPurpleLastIndex].Value = PcbaYellowTotal;
            PcbaWorksheet.Cells[PcbaTotalRow, PcbaPurpleFirstIndex, PcbaTotalRow, PcbaPurpleLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            PcbaWorksheet.Cells[PcbaTotalRow, PcbaPurpleFirstIndex, PcbaTotalRow, PcbaPurpleLastIndex].Style.Fill.BackgroundColor.SetColor(Color.Purple);


            int pcbaLightgreenFirstIndex = PcbaPurpleLastIndex + 1;// pcbaSkuTable.Columns.IndexOf(pcbaPTHColumns.First()) + 1;
            int pcbaLightgreenLastIndex = pcbaLightgreenFirstIndex + pcbaPTHColumns.Count - 1;// pcbaSkuTable.Columns.IndexOf(pcbaPTHColumns.Last()) + 1;
            string PcbaaLightgreenTotal = "Total:" + PcbaListRow[pcbaPTHColumns.Last()].ToString();
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaLightgreenFirstIndex, PcbaTotalRow, pcbaLightgreenLastIndex].Merge = true;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaLightgreenFirstIndex, PcbaTotalRow, pcbaLightgreenLastIndex].Value = PcbaaLightgreenTotal;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaLightgreenFirstIndex, PcbaTotalRow, pcbaLightgreenLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaLightgreenFirstIndex, PcbaTotalRow, pcbaLightgreenLastIndex].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);


            int pcbaGreenFirstIndex = pcbaLightgreenLastIndex + 1;// pcbaSkuTable.Columns.IndexOf(pcbaICTColumns.First()) + 1;
            int pcbaGreenLastIndex = pcbaGreenFirstIndex + pcbaICTColumns.Count - 1;// pcbaSkuTable.Columns.IndexOf(pcbaICTColumns.Last()) + 1;
            string PcbaGreenTotal = "Total:" + PcbaListRow[pcbaPTHColumns.Last()].ToString();
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaGreenFirstIndex, PcbaTotalRow, pcbaGreenLastIndex].Merge = true;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaGreenFirstIndex, PcbaTotalRow, pcbaGreenLastIndex].Value = PcbaGreenTotal;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaGreenFirstIndex, PcbaTotalRow, pcbaGreenLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaGreenFirstIndex, PcbaTotalRow, pcbaGreenLastIndex].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

            int pcbaRedFirstIndex = pcbaGreenLastIndex + 1;// pcbaSkuTable.Columns.IndexOf(redColumns.First()) + 1;
            int pcbaRedLastIndex = pcbaRedFirstIndex + redColumns.Count - 1;// pcbaSkuTable.Columns.IndexOf(redColumns.Last()) + 1;
            string PcbaRedTotal = "Total:" + PcbaListRow[redColumns.Last()].ToString();
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaRedFirstIndex, PcbaTotalRow, pcbaRedLastIndex].Merge = true;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaRedFirstIndex, PcbaTotalRow, pcbaRedLastIndex].Value = PcbaRedTotal;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaRedFirstIndex, PcbaTotalRow, pcbaRedLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            PcbaWorksheet.Cells[PcbaTotalRow, pcbaRedFirstIndex, PcbaTotalRow, pcbaRedLastIndex].Style.Fill.BackgroundColor.SetColor(Color.Red);

            //设置字体，也可以是中文，比如：宋体
            PcbaWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            PcbaWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            PcbaWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            PcbaWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            PcbaWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            PcbaWorksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            //skuWorksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            PcbaWorksheet.Cells.Style.ShrinkToFit = true;
            PcbaWorksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            #endregion

            #region model Sku worksheet
            ExcelWorksheet skuWorksheet = package.Workbook.Worksheets.Add("750 SKU WIP");
            int skuRowIndex = 1;   // 起始行为 1
            int skuColIndex = 1;   // 起始列为 1
            int skuTotalRow = 0;
            for (int i = 0; i < modelSkuTable.Columns.Count; i++)
            {
                skuWorksheet.Cells[skuRowIndex, skuColIndex + i].Value = modelSkuTable.Columns[i].ColumnName;
                skuWorksheet.Cells[skuRowIndex, skuColIndex + i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (modelYellowColumns.Contains(modelSkuTable.Columns[i].ColumnName.ToUpper()))
                {
                    skuWorksheet.Cells[skuRowIndex, skuColIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                }
                else if (modelBlueColumns.Contains(modelSkuTable.Columns[i].ColumnName.ToUpper()))
                {
                    skuWorksheet.Cells[skuRowIndex, skuColIndex + i].Style.Fill.BackgroundColor.SetColor(blueColor);
                }
                else if (redColumns.Contains(modelSkuTable.Columns[i].ColumnName.ToUpper()))
                {
                    skuWorksheet.Cells[skuRowIndex, skuColIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                }
                else
                {
                    skuWorksheet.Cells[skuRowIndex, skuColIndex + i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                }
                //自动调整列宽，也可以指定最小宽度和最大宽度
                skuWorksheet.Column(skuColIndex + i).AutoFit();
            }
            // 跳过第一列列名
            skuRowIndex++;
            skuTotalRow++;
            //写入数据
            for (int i = 0; i < modelSkuTable.Rows.Count; i++)
            {
                for (int j = 0; j < modelSkuTable.Columns.Count; j++)
                {
                    if (j == 0)
                    {
                        skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Value = modelSkuTable.Rows[i][j].ToString();
                    }
                    else 
                    {
                        if(string.IsNullOrWhiteSpace(modelSkuTable.Rows[i][j].ToString()) && i< modelSkuTable.Rows.Count-1)
                        {
                            skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Value = 0;
                        }
                        else
                        {
                            try
                            {
                                skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Value = Convert.ToInt32(modelSkuTable.Rows[i][j].ToString());
                            }
                            catch (Exception)
                            {
                                skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Value = modelSkuTable.Rows[i][j].ToString();
                            }
                        }
                        
                    }

                    skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (modelYellowColumns.Contains(modelSkuTable.Columns[j].ColumnName.ToUpper()))
                    {
                        skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    }
                    else if (modelBlueColumns.Contains(modelSkuTable.Columns[j].ColumnName.ToUpper()))
                    {
                        skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Style.Fill.BackgroundColor.SetColor(blueColor);
                    }
                    else if (redColumns.Contains(modelSkuTable.Columns[j].ColumnName.ToUpper()))
                    {
                        skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    }
                    else
                    {
                        skuWorksheet.Cells[skuRowIndex + i, skuColIndex + j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    }
                }
                //自动调整行高
                skuWorksheet.Row(skuRowIndex + i).CustomHeight = true;
                skuTotalRow++;
            }
            DataRow skuListRow = modelSkuTable.Rows[modelSkuTable.Rows.Count - 1];
            int skuYellowFirstIndex = skuColStartIndex;// modelSkuTable.Columns.IndexOf(modelYellowColumns.First()) + 1;
            int skuYellowLastIndex = skuColStartIndex + modelYellowColumns.Count - 1;// modelSkuTable.Columns.IndexOf(modelYellowColumns.Last()) + 1;
            string skuYellowTotal = "Total:" + skuListRow[modelYellowColumns.Last()].ToString();
            skuWorksheet.Cells[skuTotalRow, skuYellowFirstIndex, skuTotalRow, skuYellowLastIndex].Merge = true;
            skuWorksheet.Cells[skuTotalRow, skuYellowFirstIndex, skuTotalRow, skuYellowLastIndex].Value = skuYellowTotal;
            skuWorksheet.Cells[skuTotalRow, skuYellowFirstIndex, skuTotalRow, skuYellowLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            skuWorksheet.Cells[skuTotalRow, skuYellowFirstIndex, skuTotalRow, skuYellowLastIndex].Style.Fill.BackgroundColor.SetColor(Color.Yellow);


            int skuBlueFirstIndex = skuYellowLastIndex + 1;// modelSkuTable.Columns.IndexOf(modelBlueColumns.First()) + 1;
            int skuBlueLastIndex = skuBlueFirstIndex + modelBlueColumns.Count - 1;// modelSkuTable.Columns.IndexOf(modelBlueColumns.Last()) + 1;
            string skuBlueTotal = "Total:" + skuListRow[modelBlueColumns.Last()].ToString();
            skuWorksheet.Cells[skuTotalRow, skuBlueFirstIndex, skuTotalRow, skuBlueLastIndex].Merge = true;
            skuWorksheet.Cells[skuTotalRow, skuBlueFirstIndex, skuTotalRow, skuBlueLastIndex].Value = skuBlueTotal;
            skuWorksheet.Cells[skuTotalRow, skuBlueFirstIndex, skuTotalRow, skuBlueLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            skuWorksheet.Cells[skuTotalRow, skuBlueFirstIndex, skuTotalRow, skuBlueLastIndex].Style.Fill.BackgroundColor.SetColor(blueColor);

            int skuRedFirstIndex = skuBlueLastIndex+1;// modelSkuTable.Columns.IndexOf(redColumns.First()) + 1;
            int skuRedLastIndex = skuRedFirstIndex + redColumns.Count - 1;// modelSkuTable.Columns.IndexOf(redColumns.Last()) + 1;
            string skuRedTotal = "Total:" + skuListRow[redColumns.Last()].ToString();
            skuWorksheet.Cells[skuTotalRow, skuRedFirstIndex, skuTotalRow, skuRedLastIndex].Merge = true;
            skuWorksheet.Cells[skuTotalRow, skuRedFirstIndex, skuTotalRow, skuRedLastIndex].Value = skuRedTotal;
            skuWorksheet.Cells[skuTotalRow, skuRedFirstIndex, skuTotalRow, skuRedLastIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            skuWorksheet.Cells[skuTotalRow, skuRedFirstIndex, skuTotalRow, skuRedLastIndex].Style.Fill.BackgroundColor.SetColor(Color.Red);

            //设置字体，也可以是中文，比如：宋体
            skuWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            skuWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            skuWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            skuWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            skuWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            skuWorksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            //skuWorksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            skuWorksheet.Cells.Style.ShrinkToFit = true;  
            skuWorksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            #endregion

            #region DOF SKU worksheet
            ExcelWorksheet dofSkuWorksheet = package.Workbook.Worksheets.Add("WIP SKU DOF");
            int dofSkuRowIndex = 1;   // 起始行为 1
            int dofSkuColIndex = 1;   // 起始列为 1
            //设置列名
            for (int i = 0; i < dofSkuData.Columns.Count; i++)
            {
                dofSkuWorksheet.Cells[dofSkuRowIndex, dofSkuColIndex + i].Value = dofSkuData.Columns[i].ColumnName;
                //自动调整列宽，也可以指定最小宽度和最大宽度
                dofSkuWorksheet.Column(dofColIndex + i).AutoFit();
            }

            // 跳过第一列列名
            dofSkuRowIndex++;
            //写入数据
            for (int i = 0; i < dofSkuData.Rows.Count; i++)
            {
                for (int j = 0; j < dofSkuData.Columns.Count; j++)
                {
                    try
                    {
                        dofSkuWorksheet.Cells[dofSkuRowIndex + i, dofSkuColIndex + j].Value = Convert.ToInt32(dofSkuData.Rows[i][j].ToString());
                    }
                    catch (Exception)
                    {
                        dofSkuWorksheet.Cells[dofSkuRowIndex + i, dofSkuColIndex + j].Value = dofSkuData.Rows[i][j].ToString();
                    }
                }
                //自动调整行高
                dofSkuWorksheet.Row(dofSkuRowIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            dofSkuWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            dofSkuWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            dofSkuWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            dofSkuWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            dofSkuWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            dofSkuWorksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            dofSkuWorksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            dofSkuWorksheet.Cells.Style.ShrinkToFit = true;
            #endregion

            #region CTO SKU worksheet
            ExcelWorksheet ctoSkuWorksheet = package.Workbook.Worksheets.Add("WIP SKU CTO");
            int ctoSkuRowIndex = 1;   // 起始行为 1
            int ctoSkuColIndex = 1;   // 起始列为 1
            //设置列名
            for (int i = 0; i < ctoSkuData.Columns.Count; i++)
            {
                ctoSkuWorksheet.Cells[ctoSkuRowIndex, ctoSkuColIndex + i].Value = ctoSkuData.Columns[i].ColumnName;
                //自动调整列宽，也可以指定最小宽度和最大宽度
                ctoSkuWorksheet.Column(dofColIndex + i).AutoFit();
            }

            // 跳过第一列列名
            ctoSkuRowIndex++;
            //写入数据
            for (int i = 0; i < ctoSkuData.Rows.Count; i++)
            {
                for (int j = 0; j < ctoSkuData.Columns.Count; j++)
                {
                    try
                    {
                        ctoSkuWorksheet.Cells[ctoSkuRowIndex + i, ctoSkuColIndex + j].Value = Convert.ToInt32(ctoSkuData.Rows[i][j].ToString());
                    }
                    catch (Exception)
                    {
                        ctoSkuWorksheet.Cells[ctoSkuRowIndex + i, ctoSkuColIndex + j].Value = ctoSkuData.Rows[i][j].ToString();
                    }
                }
                //自动调整行高
                ctoSkuWorksheet.Row(ctoSkuRowIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            ctoSkuWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            ctoSkuWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            ctoSkuWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            ctoSkuWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            ctoSkuWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            ctoSkuWorksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            ctoSkuWorksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            ctoSkuWorksheet.Cells.Style.ShrinkToFit = true;
            #endregion

            #region Backlog worksheet
            // 添加一个 sheet 表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Backlog");
            int rowBacklogIndex = 1;   // 起始行为 1
            int colBacklogIndex = 1;   // 起始列为 1
            for (int i = 0; i < backlogTable.Columns.Count; i++)
            {
                //worksheet.Cells[rowBacklogIndex, colBacklogIndex + i].Style.Numberformat.Format = "@";
                worksheet.Cells[rowBacklogIndex, colBacklogIndex + i].Value = backlogTable.Columns[i].ColumnName;
                //自动调整列宽，也可以指定最小宽度和最大宽度
                worksheet.Column(colBacklogIndex + i).AutoFit();
            }

            // 跳过第一列列名
            rowBacklogIndex++;
            //写入数据
            for (int i = 0; i < backlogTable.Rows.Count; i++)
            {
                for (int j = 0; j < backlogTable.Columns.Count; j++)
                {
                    if (j == 0)
                    {                        
                        worksheet.Cells[rowBacklogIndex + i, colBacklogIndex + j].Value = backlogTable.Rows[i][j].ToString();
                    }
                    else
                    {
                        worksheet.Cells[rowBacklogIndex + i, colBacklogIndex + j].Value = string.IsNullOrWhiteSpace(backlogTable.Rows[i][j].ToString()) ? 0 : Convert.ToInt32(backlogTable.Rows[i][j].ToString());
                    }                    
                }
                //自动调整行高
                worksheet.Row(rowBacklogIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            worksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            worksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);            
            //垂直居中
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            worksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            //worksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;
            worksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            #endregion

            #region High Priority SKUs
            ExcelWorksheet highWorksheet = package.Workbook.Worksheets.Add("High Priority SKUs");
            int highRowIndex = 1;   // 起始行为 1
            //int highColIndex = 1;   // 起始列为 1
            int dataRow711 = 4;//711機種4行數據 Date,firstDate,secondDate,Delta,
            int dataRow750 = 6;//750機種4行數據 Date,firstDate,secondDate,Delta,IN,OUT
            int dataRowOther = 4;//other 機種4行數據 Date,firstDate,secondDate,Delta,

            DateTime first=DateTime.ParseExact(firstPCBAData.TableName.Split('_')[3],"yyyyMMdd",System.Globalization.CultureInfo.CurrentCulture);
           
            DateTime second = DateTime.ParseExact(secondPCBAData.TableName.Split('_')[3], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);            
            
            foreach (var item in dicHighPriority)
            {
                string category = item.Key;
                List<string> skuList = (List<string>)item.Value;
                int categoryRowStart = highRowIndex;
                int rowCount750 = skuList.FindAll(r => r.StartsWith("750")).Count * dataRow750;
                int rowCount711 = skuList.FindAll(r => r.StartsWith("711")).Count * dataRow711;
                int rowCountOther = skuList.FindAll(r => !r.StartsWith("711") &&! r.StartsWith("750")).Count * dataRowOther;
                int categoryRowEnd = rowCountOther + rowCount750 + rowCount711 + skuList.Count - 1 + highRowIndex - 1;
                int categoryColStart = 1;

                //highWorksheet.Cells[categoryRowStart, categoryColStart, categoryRowEnd, categoryColStart].Merge = true;
                //highWorksheet.Cells[categoryRowStart, categoryColStart, categoryRowEnd, categoryColStart].Value = category;
                int tempRow = 0;               
                foreach (var sku in skuList)
                {
                    int skuRowStart = 0;
                    int skuRowEnd = 0;
                    int skuDataRow = 0;
                    DataRow[] firstSkuRow = null;
                    DataRow[] secondSkuRow = null;
                    DataColumnCollection dataColumn = null;
                    
                    
                    if (sku.StartsWith("750"))
                    {
                        skuRowStart = tempRow * dataRow750 + highRowIndex;
                        skuRowEnd = (tempRow + 1) * dataRow750 + (highRowIndex - 1);
                        skuDataRow = dataRow750;

                        firstSkuRow =  firstModelData.Rows.Count > 0 ? firstModelData.Select($@" SKUNO= '{sku}'") : null;                        
                        secondSkuRow = secondModelData.Rows.Count > 0 ? secondModelData.Select($@" SKUNO= '{sku}'") : null;
                        //dataColumn = firstModelData.Columns.Count > 0 ? firstModelData.Columns : secondModelData.Columns;                        
                    }
                    else if (sku.StartsWith("711"))
                    {
                        skuRowStart = tempRow * dataRow711 + highRowIndex;
                        skuRowEnd= (tempRow + 1) * dataRow711 + (highRowIndex - 1);
                        skuDataRow = dataRow711;

                        firstSkuRow = firstPCBAData.Rows.Count > 0 ? firstPCBAData.Select($@" SKUNO= '{sku}'") : null;                        
                        secondSkuRow = secondPCBAData.Rows.Count > 0 ? secondPCBAData.Select($@" SKUNO= '{sku}'") : null;
                        //dataColumn = firstPCBAData.Columns.Count > 0 ? firstPCBAData.Columns : secondPCBAData.Columns;
                    }
                    else
                    {
                        skuRowStart = tempRow * dataRowOther + highRowIndex;
                        skuRowEnd = (tempRow + 1) * dataRowOther + (highRowIndex - 1);
                        skuDataRow = dataRowOther;

                        firstSkuRow = firstDOFData.Rows.Count > 0 ? firstDOFData.Select($@" SKUNO= '{sku}'") : null;
                                       
                        if (firstSkuRow == null)
                        {
                            firstSkuRow = firstCTOData.Rows.Count > 0 ? firstCTOData.Select($@" SKUNO= '{sku}'") : null;                            
                        }else if(firstSkuRow.Length == 0)
                        {
                            firstSkuRow = firstCTOData.Rows.Count > 0 ? firstCTOData.Select($@" SKUNO= '{sku}'") : null;
                        }

                        secondSkuRow = secondDOFData.Rows.Count > 0 ? secondDOFData.Select($@" SKUNO= '{sku}'") : null;                        
                        if (secondSkuRow == null)
                        {                           
                            secondSkuRow = secondCTOData.Rows.Count > 0 ? secondCTOData.Select($@" SKUNO= '{sku}'") : null;                            
                        }
                        else if (secondSkuRow.Length == 0)
                        {
                            secondSkuRow = secondCTOData.Rows.Count > 0 ? secondCTOData.Select($@" SKUNO= '{sku}'") : null;
                            
                        }                        
                    }
                    dataColumn = (firstSkuRow!=null&& firstSkuRow.Count() > 0) ? firstSkuRow[0].Table.Columns : ((secondSkuRow!=null&&secondSkuRow.Count() > 0) ? secondSkuRow[0].Table.Columns : null);

                    if (dataColumn == null)
                    {
                        if (sku.StartsWith("750"))
                        {
                            categoryRowEnd = categoryRowEnd - dataRow750 - 1;
                        }
                        else if (sku.StartsWith("711"))
                        {
                            categoryRowEnd = categoryRowEnd - dataRow711 - 1;
                        }
                        else
                        {
                            categoryRowEnd = categoryRowEnd - dataRowOther - 1;
                        }
                        continue;
                    }
                    int skuColStart = 2;
                    highWorksheet.Cells[skuRowStart, skuColStart, skuRowEnd, skuColStart].Merge = true;
                    highWorksheet.Cells[skuRowStart, skuColStart, skuRowEnd, skuColStart].Value = sku;

                    int dateColStart = 3;                   
                    highWorksheet.Cells[highRowIndex, dateColStart].Value = "DATE";
                    highWorksheet.Cells[highRowIndex+1, dateColStart].Value = first.ToString("MM/dd");
                    highWorksheet.Cells[highRowIndex+2, dateColStart].Value = second.ToString("MM/dd");
                    highWorksheet.Cells[highRowIndex+3, dateColStart].Value = "Delta";
                    if (sku.StartsWith("750"))
                    {
                        highWorksheet.Cells[highRowIndex + 4, dateColStart].Value = "IN";
                        highWorksheet.Cells[highRowIndex + 5, dateColStart].Value = "OUT";
                    }                    
                    foreach (DataColumn column in dataColumn)
                    {
                        if (column.ColumnName.ToUpper().Equals("SKUNO"))
                        {
                            continue;
                        }
                        dateColStart++;
                        highWorksheet.Cells[highRowIndex, dateColStart].Value = column.ColumnName;

                        DataRow firstSku = (firstSkuRow != null && firstSkuRow.Length > 0) ? firstSkuRow[0] : null;
                        string firstQty = firstSku == null ? "0" : firstSku[column.ColumnName].ToString();
                        int fQty = 0;
                        bool fBool = Int32.TryParse(firstQty, out fQty);
                        if (fBool)
                        {
                            highWorksheet.Cells[highRowIndex + 1, dateColStart].Value = fQty;
                        }
                        else
                        {
                            highWorksheet.Cells[highRowIndex + 1, dateColStart].Value = firstQty;
                        }

                        DataRow secondSku = (secondSkuRow != null && secondSkuRow.Length > 0) ? secondSkuRow[0] : null;
                        string secondQty = "0";
                        try
                        {
                            secondQty = secondSku == null ? "0" : secondSku[column.ColumnName].ToString();
                        }
                        catch
                        { }
                        int sQty = 0;
                        bool sBool = Int32.TryParse(secondQty, out sQty);
                        if (sBool)
                        {
                            highWorksheet.Cells[highRowIndex + 2, dateColStart].Value = sQty;
                        }
                        else
                        {
                            highWorksheet.Cells[highRowIndex + 2, dateColStart].Value = secondQty;
                        }
                        if (!column.ColumnName.ToUpper().Equals("QTY") && !column.ColumnName.ToUpper().Equals("BALANCE"))
                        {
                            highWorksheet.Cells[highRowIndex + 3, dateColStart].Value = sQty - fQty;
                        }
                        if (sku.StartsWith("750")&& inOutStation.Contains(column.ColumnName))
                        {
                            var inRow = testInTable.Select($" SKUNO='{sku}' AND STATION='{column.ColumnName}'");
                            int inQty = 0;
                            if(inRow.Length>0)
                            {
                                inQty = Convert.ToInt32(inRow[0]["IN_QTY"].ToString());
                            }
                            var outRow = testOutTable.Select($" SKUNO='{sku}' AND STATION='{column.ColumnName}'");
                            int outQty = 0;
                            if (outRow.Length > 0)
                            {
                                outQty = Convert.ToInt32(outRow[0]["OUT_QTY"].ToString());
                            }
                            highWorksheet.Cells[highRowIndex + 4, dateColStart].Value = inQty;
                            highWorksheet.Cells[highRowIndex + 5, dateColStart].Value = outQty;
                        }
                    }
                    highRowIndex = highRowIndex + skuDataRow + 1;
                }

                highWorksheet.Cells[categoryRowStart, categoryColStart, categoryRowEnd, categoryColStart].Merge = true;
                highWorksheet.Cells[categoryRowStart, categoryColStart, categoryRowEnd, categoryColStart].Value = category;

                highWorksheet.Cells[highRowIndex-1, 1, highRowIndex-1, 35].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                highWorksheet.Cells[highRowIndex-1, 1, highRowIndex-1, 35].Style.Fill.BackgroundColor.SetColor(Color.Black);
            }
            //设置字体，也可以是中文，比如：宋体
            highWorksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            highWorksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            highWorksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            highWorksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            highWorksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            highWorksheet.Cells.Style.WrapText = false;
            //单元格自动适应大小
            highWorksheet.Cells.Style.ShrinkToFit = true;
            //highWorksheet.Cells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            #endregion

            #region TEST_IN_OUT
            //ExcelWorksheet testWorksheet = package.Workbook.Worksheets.Add(tableTest.TableName);
            //MakeTestInOutWorksheet(testWorksheet, tableTest);
            #endregion

            MemoryStream ms = new MemoryStream();
            package.SaveAs(ms);
            byte[] bytes = ms.ToArray();
            package.Dispose();
            return Convert.ToBase64String(bytes);
        }
    
        public static string TEST_IN_OUTExportExcel(DataTable dt)
        {     
            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage();
            // 添加一个 sheet 表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);
            MakeTestInOutWorksheet(worksheet, dt);
            //创建一个内存流，然后转换为字节数组，输出到浏览器下载
            MemoryStream ms = new MemoryStream();
            package.SaveAs(ms);
            byte[] bytes = ms.ToArray();
            worksheet.Dispose();
            package.Dispose();
            return Convert.ToBase64String(bytes);
        }
    
        private static void MakeTestInOutWorksheet(ExcelWorksheet worksheet,DataTable data)
        {
            int rowIndex = 1;   // 起始行为 1
            int colIndex = 1;   // 起始列为 1

            //设置列名
            for (int i = 0; i < data.Columns.Count; i++)
            {
                string title = "";
                if (data.Columns[i].ColumnName.Contains("/"))
                {
                    title = data.Columns[i].ColumnName.Split('/')[0];
                    worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
                    worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Value = title;


                    worksheet.Cells[rowIndex + 1, colIndex].Value = $@"In";
                    worksheet.Column(colIndex + i).AutoFit();

                    worksheet.Cells[rowIndex + 1, colIndex + 1].Value = $@"Out";
                    worksheet.Column(colIndex + 1).AutoFit();

                    worksheet.Cells[rowIndex + 1, colIndex + 2].Value = $@"Delta";
                    worksheet.Column(colIndex + 2).AutoFit();
                    i = i + 2;
                    colIndex = colIndex + 3;
                }
                else
                {
                    worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
                    worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Value = data.Columns[i].ColumnName;
                    colIndex++;
                }
            }
            rowIndex = rowIndex + 1;
            colIndex = 1;
            // 跳过第一列列名
            rowIndex++;
            //写入数据
            for (int i = 0; i < data.Rows.Count; i++)
            {
                for (int j = 0; j < data.Columns.Count; j++)
                {                    
                    try
                    {
                        worksheet.Cells[rowIndex + i, colIndex + j].Value = Convert.ToInt32(data.Rows[i][j].ToString());
                    }
                    catch (Exception)
                    {
                        worksheet.Cells[rowIndex + i, colIndex + j].Value = data.Rows[i][j].ToString();
                    }
                }
                //自动调整行高
                worksheet.Row(rowIndex + i).CustomHeight = true;
            }

            //设置字体，也可以是中文，比如：宋体
            worksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            worksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            worksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            worksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;
        }

        public static string MakeVTForecastCompare(DataTable dt,List<string> compareColumns)
        {
            //新建一个 Excel 工作簿
            ExcelPackage package = new ExcelPackage();
            // 添加一个 sheet 表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);
            int rowIndex = 1;   // 起始行为 1
            int colIndex = 1;   // 起始列为 1
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                worksheet.Cells[rowIndex, colIndex + i].Value = dt.Columns[i].ColumnName;
                //自动调整列宽，也可以指定最小宽度和最大宽度
                //worksheet.Column(colIndex + i).AutoFit();
            }
            rowIndex = 2;

            //// 跳过第一列列名
            //rowIndex++;
            //写入数据
            ColorConverter colorConver = new ColorConverter();
            Color cf5eb8f = (Color)colorConver.ConvertFromString("#f5eb8f"); 
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
                    if (!compareColumns.Contains(dt.Columns[j].ColumnName))
                    {                        
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(dt.Rows[i][j].ToString());                        
                        worksheet.Cells[rowIndex + i, colIndex + j].Value = data.Value<double>("Qty");
                        if (!data.Value<bool>("IsSame"))
                        {                           
                            worksheet.Cells[rowIndex + i, colIndex + j].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[rowIndex + i, colIndex + j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[rowIndex + i, colIndex + j].Style.Fill.BackgroundColor.SetColor(cf5eb8f);
                        }
                    }
                    else
                    {
                        worksheet.Cells[rowIndex + i, colIndex + j].Value = dt.Rows[i][j].ToString();
                    }
                }
                //自动调整行高
                worksheet.Row(rowIndex + i).CustomHeight = true;
                worksheet.Column(colIndex + i).AutoFit();
            }

            //设置字体，也可以是中文，比如：宋体
            worksheet.Cells.Style.Font.Name = "Arial";
            //字体大小
            worksheet.Cells.Style.Font.Size = 12;
            //字体颜色
            //worksheet.Cells.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            //垂直居中
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //水平居中
            worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //单元格是否自动换行
            worksheet.Cells.Style.WrapText = false;
            //设置单元格格式为文本
            worksheet.Cells.Style.Numberformat.Format = "@";
            //单元格自动适应大小
            worksheet.Cells.Style.ShrinkToFit = true;
            //创建一个内存流，然后转换为字节数组，输出到浏览器下载
            MemoryStream ms = new MemoryStream();
            package.SaveAs(ms);
            byte[] bytes = ms.ToArray();
            worksheet.Dispose();
            package.Dispose();
            return Convert.ToBase64String(bytes);
        }
    }
}
