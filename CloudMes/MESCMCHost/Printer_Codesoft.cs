using LabelManager2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESCMCHost
{
    public class Printer_Codesoft
    {
        public string PrinterName { get; set; }
        string _path = "";
        ApplicationClass _app;
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
                _app = new ApplicationClass();
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
            catch
            {
                throw new Exception("path not exist:" + path);
            }

        }
        ~Printer_Codesoft()
        {
            try
            {
                _doc.Close(false);
            }
            catch { }
            try
            {
                _app.Quit();
            }
            catch { }
        }
        public void Print()
        {
            Print(1);

        }
        public void Print(int qty)
        {
            try
            {
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
            catch
            { }
            //_doc.Printer.SwitchTo(PrinterName);
            _doc.Printer.SwitchTo("ZPL","FILE:D:\\D1.TXT",true);
            _doc.PrintLabel(qty,1,1,1,1, "D:\\D1.TXT");
            

           // _doc.PrintDocument(qty);
        }
        public void close()
        {
            _doc.Close(false);
            _app.Quit();
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
