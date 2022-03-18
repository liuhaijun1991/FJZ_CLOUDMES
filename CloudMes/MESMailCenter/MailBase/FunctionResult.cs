using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MESMailCenter
{
    /// <summary>
    /// 定義了函數返回值的標準形式
    /// </summary>
    public class FunctionResult
    {
        bool _Status;
        string _Message;
        Dictionary<string, object> _Output = new Dictionary<string,object>();
        List<string> _OutPutNames = new List<string>();
        DateTime _Time;
        /// <summary>
        /// 添加一個返回值;
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddOutPutValue(string name, object value)
        {
            _Output.Add(name, value);
            _OutPutNames.Add(name);
        }
        /// <summary>
        /// 獲取包含返回值名稱的數組
        /// </summary>
        public string[] OutPutNames
        {
            get
            {
                string[] ret = new string[_OutPutNames.Count];
                for (int i = 0; i < _OutPutNames.Count; i++)
                {
                    ret[i] = _OutPutNames[i];
                }
                return ret;
            }
        }
        /// <summary>
        /// 獲取函數執行的狀態;
        /// </summary>
        public bool Status
        {
            get
            {
                return _Status;
            }
        }
        /// <summary>
        /// 獲取返回值
        /// </summary>
        /// <param name="name">返回值名稱</param>
        /// <returns></returns>
        public object GetOutputValue(string name)
        {
            return _Output[name];
        }
        /// <summary>
        /// 獲取返回值
        /// </summary>
        /// <param name="Index">返回值下標</param>
        /// <returns></returns>
        public object GetOutputValue(int Index)
        {
            return _Output[_OutPutNames[Index]];
        }
        /// <summary>
        /// 構造一個FunctionResult
        /// </summary>
        /// <param name="status">函數執行成功與否</param>
        /// <param name="message">返回的消息</param>
        public FunctionResult(bool status, string message)
        {
            _Status = status;
            _Message = message;
            _Time = DateTime.Now;
        }
        /// <summary>
        /// 獲取函數執行結束的時間
        /// </summary>
        public DateTime Time
        {
            get
            {
                return _Time;
            }
        }
        
    }
    
}
