using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace MESMailCenter
{
    public class DataTableToFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="noUse"></param>
        public static void ToBRCNFile(DataTable data, string fileName, string[] noUse)
        {
            string strFile = "";
            for (int i = 0; i < data.Rows.Count; i++)
            {
                int jj = 0;
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    foreach (string t in noUse)
                    {
                        if (t == data.Columns[j].ColumnName)
                        {
                            continue;
                        }
                    }
                    if (jj != 0)
                    {
                        strFile += "|";
                    }
                    jj += 1;
                    strFile += data.Rows[i][data.Columns[j].ColumnName].ToString();
                }
                strFile += "\r\n";
            }
            strFile += "CTL|" + data.Rows.Count.ToString();

            FileStream f = new FileStream(fileName, FileMode.Create);
            StreamWriter w = new StreamWriter(f);
            w.Write(strFile);
            try
            {
                w.Flush();
            }
            catch
            { }
            try
            {
                w.Close();
            }
            catch
            { }

            try
            {
                f.Close();
            }
            catch
            { }
        }
    }
}
