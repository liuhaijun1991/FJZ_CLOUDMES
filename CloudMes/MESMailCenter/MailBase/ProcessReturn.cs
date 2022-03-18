using System;
using System.Collections.Generic;
using System.Text;

namespace MESMailCenter
{
    /// <summary>
    /// Process的Start方法的返回值
    /// </summary>
    public class ProcessReturn:ProcessParameters
    {
        /// <summary>
        /// 初始化ProcessReturn
        /// </summary>
        /// <param name="value">處理過程成功與否</param>
        /// <param name="msg">返回消息</param>
        public ProcessReturn(bool value, string msg)
        {
            _values.Add("ReturnValue", value);
            _values.Add("Message",msg);
        }
        /// <summary>
        /// 初始化ProcessReturn
        /// </summary>
        /// <param name="value">處理過程成功與否</param>
        /// <param name="msg">返回消息</param>
        /// <param name="keys">需要回傳的參數列表</param>
        /// <param name="values">需要回傳的參數值列表</param>
        public ProcessReturn(bool value, string msg,string[] keys,object[] values)
        {
            
            _values.Add("ReturnValue", value);
            _values.Add("Message", msg);
            if (keys.Length != values.Length)
            {
                throw new Exception("keys.Length != values.Length,PLS Check!");
            }
            for (int i = 0; i < keys.Length; i++)
            {
                _values.Add(keys[i], values[i]);
            }
            
        }
        /// <summary>
        /// 獲取處理成功與否
        /// </summary>
        public bool ReturnValue
        {
            get
            {
                return (bool)this._values["ReturnValue"];
            }
            
        }
        /// <summary>
        /// 獲取處理結果的返回消息
        /// </summary>
        public string Message
        {
            get
            {
                return (string) this._values["Message"];
                //throw new System.NotImplementedException();
            }
            
        }
    }
}
