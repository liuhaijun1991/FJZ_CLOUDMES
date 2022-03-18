using System;
using System.Collections.Generic;
using System.Text;

namespace MESMailCenter
{
    /// <summary>
    /// 處理程序公用基類
    /// </summary>
    public class Process
    {
        /// <summary>
        /// 開始處理數據,需要在派生類中實現
        /// </summary>
        /// <param name="Para">傳入參數</param>
        /// <returns>返回處理結果</returns>
        virtual public ProcessReturn Start(ProcessParameters Para)
        {
            throw new System.NotImplementedException();
            //return new ProcessReturn(true, "This Process Done nothing!");
        }
        /// <summary>
        /// 該方法提供給ProcessManager自動調用.
        /// </summary>
        virtual public void Start()
        {
            _ProcessCompleteRateChange += new ProcessCompleteRateDelegate(Process___ProcessCompleteRateChange);
            throw new System.NotImplementedException();
            
            //return new ProcessReturn(true, "This Process Done nothing!");
        }

        protected void Process___ProcessCompleteRateChange(object Sender, ProcessCompleteRateEventArgs EventArgs)
        {
            if (ProcessCompleteRateChange != null)
            {
                ProcessCompleteRateChange(Sender, EventArgs);
            }
        }
        /// <summary>
        /// 處理進度變換的委託
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="EventArgs"></param>
        public delegate void ProcessCompleteRateDelegate(object Sender, ProcessCompleteRateEventArgs EventArgs);
        /// <summary>
        /// 作為ProcessCompleteRateChange的內部事件,編程時請引發該事件,以處理異常
        /// </summary>
        virtual protected event ProcessCompleteRateDelegate _ProcessCompleteRateChange;
        /// <summary>
        /// 處理進度變換的事件,作為在多線程程序里主線程監控用
        /// </summary>
        public event ProcessCompleteRateDelegate ProcessCompleteRateChange;

        /// <summary>
        /// 提供註冊一些基礎事件,如處理進度變換事件
        /// </summary>
        protected void RegBaseEvents()
        {
            _ProcessCompleteRateChange += new ProcessCompleteRateDelegate(Process___ProcessCompleteRateChange);
        }

    }
    /// <summary>
    /// 作為處理進程改變的事件參數
    /// </summary>
    public class ProcessCompleteRateEventArgs : EventArgs
    {
        int _AllCount = 0;
        int _CurCount = 0;
        object _CurObject;
        public string MSG;  

        /// <summary>
        /// 
        /// </summary>
        public int AllCount
        {
            get
            {
                return _AllCount;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CurCount
        {
            get
            {
                return _CurCount;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public object CurObj
        {
            get
            {
                return _CurObject;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_All"></param>
        /// <param name="_Cur"></param>
        /// <param name="_CurObj"></param>
        public ProcessCompleteRateEventArgs(int _All, int _Cur,object _CurObj)
        {
            _AllCount = _All;
            _CurCount = _Cur;
            _CurObject = _CurObj;

        }

    }

    public class ProcessOutPutEventArgs : EventArgs
    { 
        
    }
}
