using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MESMailCenter
{
    /// <summary>
    /// 
    /// </summary>
    public class LogAddEventArgs:System.EventArgs
    {
        string _LogString;
        /// <summary>
        /// 
        /// </summary>
        public string LogString
        {
            get
            {
                return _LogString;
            }
        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Log"></param>
        public LogAddEventArgs(string Log)
        {
            _LogString=Log;
            
        }
    }
    /// <summary>
    /// 保存日誌的方法
    /// </summary>
    public enum LogMode 
    { 
        /// <summary>
        /// 將日誌保存入文件
        /// </summary>
        File,
        /// <summary>
        /// 由委託來處理,暫不可用
        /// </summary>
        Delegate,
        /// <summary>
        /// 寫入數據庫,暫不可用
        /// </summary>
        DataBase,
        /// <summary>
        /// 彈出MessageBox
        /// </summary>
        MessageBox
    }
    /// <summary>
    /// 日誌項
    /// </summary>
    public class LogManagedItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void ActionEventHandler(object sender, LogAddEventArgs ev);
        /// <summary>
        /// 日誌寫入后引發的事件
        /// </summary>
        public event ActionEventHandler LogAdd;
        string _LogName;
        LogMode _LogMode;
        /// <summary>
        /// 創建一個新的日誌項
        /// </summary>
        /// <param name="logname">日誌名稱</param>
        /// <param name="mode">日誌模式</param>
        public LogManagedItem(string logname,LogMode mode)
        {
            _LogName = logname;
            _LogMode = mode;
            //LogAdd(this, new LogAddEventArgs("ok"));
        }
        /// <summary>
        /// 向本日誌項寫日誌
        /// </summary>
        /// <param name="Log">日誌內容</param>
        public void Write(string Log)
        {
            //lock(
            //    )
            //{
                if ((_LogMode & LogMode.File)==LogMode.File)
                {

                    if (!Directory.Exists(".\\Logs"))
                    {
                        System.IO.Directory.CreateDirectory(".\\Logs");
                        //throw new Exception("It haven't Logs dir !");
                    }
                    string Fname = ".\\logs\\" + _LogName + DateTime.Now.ToString("yyyy_MM_dd") + ".Log.TXT";
                    FileStream Fs = new FileStream(Fname, FileMode.Append);
                    StreamWriter Sw = new StreamWriter(Fs);
                    Sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + Log);
                    Sw.Flush();
                    Fs.Flush();
                    Sw.Close();
                    Fs.Close();
                    
                }
                if ((_LogMode & LogMode.MessageBox)==LogMode.MessageBox)
                {
                    System.Windows.Forms.MessageBox.Show(Log);
                }
                //LogAdd(this, new LogAddEventArgs(Log));
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Log"></param>
        /// <param name="LockObject"></param>
        public void Write(string Log,object LockObject)
        {
            lock (LockObject)
            {
                if ((_LogMode & LogMode.File) == LogMode.File)
                {

                    if (!Directory.Exists(".\\Logs"))
                    {
                        System.IO.Directory.CreateDirectory(".\\Logs");
                        //throw new Exception("It haven't Logs dir !");
                    }
                    string Fname = ".\\logs\\" + _LogName + DateTime.Now.ToString("yyyy_MM_dd") + ".Log.TXT";
                    FileStream Fs = new FileStream(Fname, FileMode.Append);
                    StreamWriter Sw = new StreamWriter(Fs);
                    Sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + Log);
                    Sw.Flush();
                    Fs.Flush();
                    Sw.Close();
                    Fs.Close();

                }
                if ((_LogMode & LogMode.MessageBox) == LogMode.MessageBox)
                {
                    System.Windows.Forms.MessageBox.Show(Log);
                }
                //LogAdd(this, new LogAddEventArgs(Log));
            }
        }
    }
}
