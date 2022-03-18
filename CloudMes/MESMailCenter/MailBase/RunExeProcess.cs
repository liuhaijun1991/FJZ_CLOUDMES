using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MESMailCenter
{
    /// <summary>
    /// 運行一個EXE程序的Process
    /// </summary>
    class RunExeProcess:Process
    {
        static LogManager Log = new LogManager();
        string _Path;
        /// <summary>
        /// 獲取或設置需要運行的程序的路徑
        /// </summary>
        public string Path
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }
        /// <summary>
        /// 構造一個執行外部EXE的處理過程
        /// </summary>
        /// <param name="Path">程序路徑</param>
        public RunExeProcess(string Path)
        {
            _Path = Path;
            try
            {
                Log.AddLog("RunExeProcess");
            }
            catch { }
        }
        public override void Start()
        {
            //base.Start();
            try
            {
                System.Diagnostics.Process.Start(_Path);
                
                Log.Write("RunExeProcess", "[" + _Path + "]執行!");
            }
            catch (Exception e)
            {
                
                Log.Write("RunExeProcess","["+ _Path +"]"+ e.Message);
            }
            
        }
    }
}
