using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MESMailCenter
{
    /// <summary>
    /// 定時執行的排程項目
    /// 排程是以多線程的形式執行的
    /// </summary>
    public class ProcessManagedItem
    {
        Process _Process;
        Timer _Timer;
        int _Msec = 0;
        ProccessManagerTypeEnum _ManagerType = ProccessManagerTypeEnum.TimeSpan;
        List<string> _StartList = new List<string>();
        public List<string> StartList
        {
            get { return _StartList; }
        }
        public ProccessManagerTypeEnum ManagerType
        {
            get { return _ManagerType; }
            set { _ManagerType = value; }
        }

        /// <summary>
        /// 在運行狀態改變時引發的事件的委託
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public delegate void RunningStateChangeHandler(object sender, RunningStateChangeEventArgs ev);
        /// <summary>
        /// 在運行狀態改變時引發的事件,事件發生時狀態已經改變
        /// </summary>
        public event RunningStateChangeHandler RunningStateChange;// = new RunningStateChangeHandler();
        protected void OnRunningStateChange(object sender, RunningStateChangeEventArgs ev)
        {
            
        }
        bool _IsRuning=false;
        /// <summary>
        /// 以毫秒為單位獲取或設置運行間隔時間,重新設置后要重新Start()才執行
        /// </summary>
        public int MSEC
        {
            get
            { return _Msec; }
            set
            { 
                _Msec = value;
                this.Stop();
            }
        }
        /// <summary>
        /// 獲取排程的執行狀態
        /// </summary>
        public bool IsRuning
        {
            get
            { return _IsRuning; }
        }
        /// <summary>
        /// 構造一個新的排程項
        /// </summary>
        /// <param name="process">需要處理的實例名稱</param>
        /// <param name="msec">需要處理的實例</param>
        public ProcessManagedItem(Process process, int msec)
        {
            _Msec = msec;
            _Timer = new Timer(new TimerCallback(timercallback));
            _IsRuning = false;
            _Process = process;
            RunningStateChange += new RunningStateChangeHandler(OnRunningStateChange);
            //_Timer.Change(0, _Msec);
        }
        /// <summary>
        /// 開始執行
        /// </summary>
        public void Start()
        {
            if (_ManagerType == ProccessManagerTypeEnum.TimeSpan)
            {
                _Timer.Change(0, _Msec);

                //_Timer.Change(
                _IsRuning = true;
                RunningStateChange(this, new RunningStateChangeEventArgs("調用Start", _Process));
            }
            else if (_ManagerType == ProccessManagerTypeEnum.TimeList)
            {
                _Timer.Change(0, 1000);
                //_Timer.Change(
                _IsRuning = true;
                RunningStateChange(this, new RunningStateChangeEventArgs("調用Start", _Process));
            }
        }
        /// <summary>
        /// 立即停止排程的執行
        /// </summary>
        public void Stop()
        {
            _Timer.Change(Timeout.Infinite, Timeout.Infinite);
            _IsRuning = false;
            RunningStateChange(this, new RunningStateChangeEventArgs("調用Stop",_Process));
        }


        void T(Process P)
        {
            P.Start();
        }

        void timercallback(object sender)
        {
            DateTime now = DateTime.Now;
            try
            {
                if (_ManagerType == ProccessManagerTypeEnum.TimeSpan)
                {
                    _Timer.Change(Timeout.Infinite, Timeout.Infinite);
                    RunningStateChange(this, new RunningStateChangeEventArgs("開始執行", _Process));
                    _Process.Start();
                    RunningStateChange(this, new RunningStateChangeEventArgs("成功執行", _Process));
                    _Timer.Change(_Msec, _Msec);
                    RunningStateChange(this, new RunningStateChangeEventArgs("開始下個計時" + _Msec.ToString(), _Process));
                }
                else if (_ManagerType == ProccessManagerTypeEnum.TimeList)
                {
                    foreach (string t in _StartList)
                    {
                        if (t == now.ToString("HH:mm:ss"))
                        {
                            _Timer.Change(Timeout.Infinite, Timeout.Infinite);
                            RunningStateChange(this, new RunningStateChangeEventArgs("開始執行", _Process));
                            _Process.Start();
                            RunningStateChange(this, new RunningStateChangeEventArgs("成功執行", _Process));
                            _Timer.Change(1, 1);
                            break;
                        }
                    }
                    
                }
            }
            catch(Exception ee)
            {
                //_Timer.Change(Timeout.Infinite, Timeout.Infinite);
                //_IsRuning = false;
                RunningStateChange(this, new RunningStateChangeEventArgs("執行出現異常:"+ee.Message,_Process));

                if (_ManagerType == ProccessManagerTypeEnum.TimeSpan)
                {
                    _Timer.Change(_Msec, _Msec);
                    RunningStateChange(this, new RunningStateChangeEventArgs("開始下個計時" + _Msec.ToString(), _Process));
                }
                else if (_ManagerType == ProccessManagerTypeEnum.TimeList)
                {
                    foreach (string t in _StartList)
                    {
                        if (t == now.ToString("HH:mm:ss"))
                        {
                            _Timer.Change(1, 1);
                            break;
                        }
                    }

                }
            }
            finally
            { }
        }
        /// <summary>
        /// 立即執行
        /// </summary>
        public void Run()
        {
            try
            {
                _Process.Start();
                RunningStateChange(this, new RunningStateChangeEventArgs("成功執行", _Process));
            }
            catch (Exception ee)
            {
                _Timer.Change(Timeout.Infinite, Timeout.Infinite);
                _IsRuning = false;
                RunningStateChange(this, new RunningStateChangeEventArgs("執行出現異常:" + ee.Message, _Process));
            }
            finally
            { }
        }
        
    }
    public enum ProccessManagerTypeEnum
    { 
        /// <summary>
        /// 在特定時間點執行
        /// </summary>
        TimeList,
        /// <summary>
        /// 默認方式,以毫秒數設置
        /// </summary>
        TimeSpan

    }

    /// <summary>
    /// 排程運行狀態改變事件參數
    /// </summary>
    public class RunningStateChangeEventArgs : EventArgs
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Message;

        public RunningState State;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public RunningStateChangeEventArgs(string msg)
        {
            Message = msg;
            if (Message == "成功執行")
            {
                State = RunningState.RunSuccess;
            }
            else if (Message == "調用Start")
            {
                State = RunningState.BeginStart;
            }
            else if (Message == "調用Stop")
            {
                State = RunningState.BeginStop;
            }
            else if (Message == "開始執行")
            {
                State = RunningState.RunStart;
            }
            else if (Message.IndexOf("開始下個計時") >= 0)
            {
                State = RunningState.NextTimingStarted;
            }
            else
            {
                State = RunningState.Error;
            }
        }
        public RunningStateChangeEventArgs(string msg,Process P)
        {
            Message = msg;
            process = P;
            if (Message == "成功執行")
            {
                State = RunningState.RunSuccess;
            }
            else if (Message == "調用Start")
            {
                State = RunningState.BeginStart;
            }
            else if (Message == "調用Stop")
            {
                State = RunningState.BeginStop;
            }
            else if (Message == "開始執行")
            {
                State = RunningState.RunStart;
            }
            else
            {
                State = RunningState.Error;
            }
        }
        public Process process;
    }
    /// <summary>
    /// 
    /// </summary>
    public enum RunningState
    { 
        /// <summary>
        /// 
        /// </summary>
        RunSuccess,
        /// <summary>
        /// 
        /// </summary>
        BeginStart,
        /// <summary>
        /// 
        /// </summary>
        BeginStop,
        /// <summary>
        /// 
        /// </summary>
        Error,
        /// <summary>
        /// 
        /// </summary>
        RunStart,
        NextTimingStarted

    }
}
