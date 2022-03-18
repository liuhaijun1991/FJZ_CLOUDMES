using System;
using System.Collections.Generic;
using System.Text;

namespace MESMailCenter
{
    /// <summary>
    /// 提供日誌的統一處理,運行時必須包含Logs子文件夾
    /// </summary>
    public class LogManager
    {
        Dictionary<string, LogManagedItem> _Logs = new Dictionary<string, LogManagedItem>();
        static LogManagedItem _MyLog = new LogManagedItem("Logsystem", LogMode.File);
        /// <summary>
        /// 默認構造函數
        /// </summary>
        public LogManager()
        { 
        
        }
        /// <summary>
        /// 添加一個新的日誌檔
        /// </summary>
        /// <param name="name">日誌檔名</param>
        public void AddLog(string name)
        {
            LogManagedItem newLog = new LogManagedItem(name, LogMode.File);
            try
            {
                _Logs.Add(name, newLog);
            }
            catch(Exception e)
            { _MyLog.Write(e.Message); }
        }
        
        /// <summary>
        /// 向日誌檔寫日誌
        /// </summary>
        /// <param name="Logname">日誌檔名</param>
        /// <param name="Log">日誌內容</param>
        public void Write(string Logname, string Log)
        {
            LogManagedItem L = _Logs[Logname];
            L.Write(Log);
        }

    }
}
