using System;
using System.Data;
using System.IO;
using MESHelper.Common;
using MESPubLab.Common;
using Newtonsoft.Json.Linq;

namespace MESHelper.Plugin
{
    public class Printer_Zebra
    {
        public string FilePath { get; set; }
        public string Port { get; set; }//配置窗口會傳入端口值
        Random rand = new Random();
        JObject _labValue;

        public JObject LabValue {
            set {
                _labValue = value;
            }
        }
        public void Print(string seq)
        {
            Logs log = new Logs();
            StreamReader R;
            R = new StreamReader(FilePath);
            string temp = R.ReadToEnd();
            R.Close();
            string temp1 = temp;
            JArray data = (JArray)_labValue["Outputs"];
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
                        MesLog.Error($@"err:{e.Message};printdata:{temp1}");
                    }
                }
                else
                {
                    JArray Values = (JArray)dc["Value"];
                    for (int i = 0; i < Values.Count; i++)
                    {
                        ItemName = "@" + Name + (i + 1).ToString() + "@";
                        try
                        {
                            temp1 = temp1.Replace(ItemName, Values[i].ToString());
                        }
                        catch (Exception e)
                        {
                            MesLog.Error($@"err:{e.Message};printdata:{temp1}");
                        }
                    }
                }
            }
            try
            {
                temp1 = temp1.Replace("@PAGE@", _labValue["PAGE"].ToString());
                temp1 = temp1.Replace("@ALLPAGE@", _labValue["ALLPAGE"].ToString());
            }
            catch (Exception e)
            {
                MesLog.Error($@"err:{e.Message};printdata:{temp1}");
            }
            try
            {
                if (!Directory.Exists("c:\\PRINTTEXT"))
                {
                    System.IO.Directory.CreateDirectory("c:\\PRINTTEXT");
                }
                //將生成的打印字符串寫入到當前目錄下的 PrintFile\yyyyMMddHHmmss.txt 文件中           

                string PrintFileName = log.WritePrintFile(temp1, seq);
                //創建 Copy 命令將文件拷貝到設定的端口
                string cmd = "Copy \"" + PrintFileName + "\" " + Port;
                //將命令寫入 C:/PRINTTEXT/printcom.bat 文件中
                string BatFilePath = log.WriteFile("c:\\PRINTTEXT\\printcom.bat", cmd);
                //調用 Shell 來執行 printcom.bat 批處理文件實現打印操作
                int re = Microsoft.VisualBasic.Interaction.Shell(BatFilePath, Microsoft.VisualBasic.AppWinStyle.Hide, true, 1000);
                MESPubLab.Common.MesLog.Info($@"FileName:{PrintFileName};Seq:{seq};Shell Return:{re}.");
            }
            catch (Exception e)
            {
                MesLog.Error($@"err:{e.Message};printdata:{temp1}");
                throw e;
            }
        }
    }
}
