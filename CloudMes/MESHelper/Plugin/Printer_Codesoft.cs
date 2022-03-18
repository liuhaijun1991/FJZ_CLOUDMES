using System;
using LabelManager2;
using System.Data;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MESHelper.Plugin
{
    public class Printer_Codesoft
    {
        public string PrinterName { get; set; }
        string _path = "";
        public static ApplicationClass _app = null;
        Document _doc;
        Variables _vars;
        JObject _labValue;


        public string Path
        {
            get
            {
                return _path;
            }
            //set
            //{
            //    _path = value;
            //}
        }

        public JObject LabValue
        {
            set
            {
                _labValue = value;
            }
        }

        public Printer_Codesoft(string path)
        {
            try
            {
                if (_app == null)
                {
                    _app = new ApplicationClass();
                }
            }
            catch (Exception)
            {
                throw;
            }
            try
            {
                _path = path;
                _doc = _app.Documents.Open(path, true);
                _vars = _doc.Variables;
            }
            catch(Exception ex)
            {
                //throw new Exception("路徑不存在:" + path);
                throw new Exception("Path is not exists!:" + path + ";Message:" + ex.Message);
            }

        }
        ~Printer_Codesoft()
        {
            try
            {
                _doc.Close(false);
            }
            catch { }
            _doc = null;
            try
            {
                //_app.Quit();
            
            }
            catch { }
            //_app = null;
        }
        public void Print()
        {
            Print(1);
            
        }
        public void Print(int qty)
        {
            for (int j = 0; j < qty; j++)
            {
                if (_labValue != null)
                {
                    //先初始化模板內的變量為空值,以避免有用同一個模板打印多頁時，第二頁及往後頁數量不滿的情況下會把第一的數據打印出來
                    for (int i = 1; i <= _vars.FormVariables.Count; i++)
                    {
                        _vars.Item(_vars.FormVariables.Item(i).Name)._Value = "";
                    }
                    JArray data = (JArray)_labValue["Outputs"];
                    foreach (var dc in data)
                    {
                        string Name = dc["Name"].ToString();
                        string Type = dc["Type"].ToString();
                        if (Type == "0")
                        {
                            string ItemName = "@" + Name + "@";
                            try
                            {
                                _vars.Item(ItemName)._Value = dc["Value"].ToString();
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            JArray Values = (JArray)dc["Value"];
                            for (int i = 0; i < Values.Count; i++)
                            {
                                string ItemName = "@" + Name + (i + 1).ToString() + "@";
                                try
                                {
                                    _vars.Item(ItemName)._Value = Values[i].ToString();
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    try
                    {
                        _vars.Item("@PAGE@")._Value = _labValue["PAGE"].ToString();
                    }
                    catch
                    {
                    }
                    try
                    {
                        _vars.Item("@ALLPAGE@")._Value = _labValue["ALLPAGE"].ToString();
                    }
                    catch
                    {
                    }
                }
                _doc.Printer.SwitchTo(PrinterName);
                _doc.PrintDocument(1);
            }          
        }
        public void close()
        {
            _doc.Close(false);
            //_app.Quit();
        }

        public int ShowPageSetupDialog()
        {
            return _app.Dialogs.Item(enumDialogType.lppxPageSetupDialog).Show();
        }

        public int ShowDocumentPropertiesDialog()
        {
            return _app.Dialogs.Item(enumDialogType.lppxDocumentPropertiesDialog).Show();
        }

        public int ShowFormDialog()
        {
            return _app.Dialogs.Item(enumDialogType.lppxFormDialog).Show();
        }

        public int ShowOptionsDialog()
        {
            return _app.Dialogs.Item(enumDialogType.lppxOptionsDialog).Show();
        }

        public int ShowPrinterSelectDialog()
        {
            return _app.Dialogs.Item(enumDialogType.lppxPrinterSelectDialog).Show();
        }

        public int ShowPrinterSetupDialog()
        {
            return _app.Dialogs.Item(enumDialogType.lppxPrinterSetupDialog).Show();
        }

    }
}
