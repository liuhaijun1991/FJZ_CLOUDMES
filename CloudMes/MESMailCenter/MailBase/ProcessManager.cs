using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MESMailCenter
{
    /// <summary>
    /// 提供定時執行的Process管理
    /// </summary>
    public class ProcessManager
    {
        Dictionary<string, ProcessManagedItem> _Processes=new Dictionary<string,ProcessManagedItem>();
        /// <summary>
        /// 根據配置文件初始化管理器
        /// </summary>
        /// <param name="FileName">配置文件名</param>
        public void Init(string FileName)
        {
            throw new System.NotImplementedException();
            Type tt = Type.GetType("ClassName");
            Assembly assembly = Assembly.GetAssembly(tt);
            Process p = (Process)assembly.CreateInstance("ClassName");
            
        }
        /// <summary>
        /// 向排程管理添加一個新的排程
        /// </summary>
        /// <param name="ProcessName">排程名</param>
        /// <param name="Item">排程處理項</param>
        public void Add(string ProcessName,ProcessManagedItem Item)
        {
            _Processes.Add(ProcessName, Item);
            Item.Start();
        }

    }
}
