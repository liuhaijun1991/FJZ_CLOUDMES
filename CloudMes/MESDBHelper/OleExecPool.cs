using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace MESDBHelper
{
    public  class OleExecPool
    {
        
        public int MaxPoolSize = 700;
        public int MinPoolSize = 2;
        private  object objlock = new object();
        public string LockFunction = "";
        string Debug = "";

        public Dictionary<string, OleExec> ShareDB = new Dictionary<string, OleExec>();
        List<OleExec> FreeTemp = new List<OleExec>();

        public List<string> BorrowTimeOutFunction = new List<string>();

        public List<string> OutPutMessage = new List<string>();
        /// <summary>
        /// 等待超時時間,單位秒 
        /// </summary>
        public int PoolTimeOut = 3;
        /// <summary>
        /// 連接的最長存活時間,單位秒
        /// </summary>
        public int ActiveTimeOut = 3600;
        /// <summary>
        /// 借出超時時間
        /// </summary>
        public int BorrowTimeOut = 300;
        /// <summary>
        /// 可用對象存放集合
        /// </summary>
        List<OleExecPoolItem> All = new List<OleExecPoolItem>();
        /// <summary>
        /// 已借出對象存放集合
        /// </summary>
        Dictionary<OleExec, OleExecPoolItem> Lend = new Dictionary<OleExec, OleExecPoolItem>();

        public List<object> GetAllStatus()
        {
            List<object> ol = new List<object>(){};
            ol.Add(new List<object>(){"pool.count", All.Count+ Lend.Count });
            foreach (var VARIABLE in All)
            {
                ol.Add(new List<object>()
                {
                    "OleExecPoolItem.creatime", VARIABLE.CreateTime,
                    "OleExecPoolItem.LendTime", VARIABLE.LendTime,
                    "OleExecPoolItem.diffLendTime",VARIABLE.LendTime.Subtract(VARIABLE.CreateTime).Duration().TotalSeconds.ToString(),
                    "OleExecPoolItem.diffNowTime",DateTime.Now.Subtract(VARIABLE.CreateTime).Duration().TotalSeconds.ToString(),
                    "OleExecPoolItem.Connection.State",VARIABLE.Data.ORM.Ado.Connection.State.ToString(),
                    "OleExecPoolItem.Connection.ConnectionTimeout",VARIABLE.Data.ORM.Ado.Connection.ConnectionTimeout
                });
            }
            foreach (var VARIABLE in Lend)
            {
                ol.Add(new List<object>()
                {
                    "OleExecPoolItem.creatime", VARIABLE.Value.CreateTime,
                    "OleExecPoolItem.LendTime", VARIABLE.Value.LendTime,
                    "OleExecPoolItem.diffLendTime",VARIABLE.Value.LendTime.Subtract(VARIABLE.Value.CreateTime).Duration().TotalSeconds.ToString(),
                    "OleExecPoolItem.diffNowTime",DateTime.Now.Subtract(VARIABLE.Value.CreateTime).Duration().TotalSeconds.ToString(),
                    "OleExecPoolItem.Connection.State",VARIABLE.Value.Data.ORM.Ado.Connection.State.ToString(),
                    "OleExecPoolItem.Connection.ConnectionTimeout",VARIABLE.Value.Data.ORM.Ado.Connection.ConnectionTimeout
                });
            }
            return ol;
        }

        public int PoolRemain
        {
            get
            {
                return All.Count;
            }
        }

        public int PoolBorrowed
        {
            get
            {
                return Lend.Count;
            }
        }

        public string _ConnectionString = "";


        public List<string> ShowLend()
        {
            return (List<string>)CallLockPool("ShowLend", new object[0]);
        }
        public List<string> _ShowLend()
        {
            List<string> ret = new List<string>();
           
            OleExec[] arry = new OleExec[Lend.Keys.Count];
            Lend.Keys.CopyTo(arry, 0);

            for (int i = 0; i < arry.Length; i++)
            {
                try
                {
                    ret.Add(Lend[arry[i]].LendTime.ToString() + "  " + (DateTime.Now - Lend[arry[i]].LendTime).TotalSeconds);
                }
                catch (Exception e)
                {
                    ret.Add(e.Message);
                }
            }
            

            return ret;
        }


        public void CleanAll()
        {
            CallLockPool("CleanAll", new object[0]);
        }
        private void _CleanAll()
        {
            for (int i = 0; i < All.Count; i++)
            {
                try
                {
                    All[i].Data.FreeMe();
                }
                catch
                { }
            }
            All.Clear();
            
            Lend.Clear();
        }

        /// <summary>
        /// 自動連接池管理計時器
        /// </summary>
        System.Timers.Timer AutoTimer = new System.Timers.Timer();

        System.Timers.Timer AutoULockTimer = new System.Timers.Timer();

        public OleExecPool(string ConnectionString)
        {
            LoadSetting();
            _ConnectionString = ConnectionString;
            while (All.Count < MinPoolSize)
            {
                CreateNewItem();
            }
            AutoTimer.Interval = 60000;
            AutoTimer.Elapsed += new System.Timers.ElapsedEventHandler(AutoTimer_Elapsed);
            AutoTimer.Enabled = true;

        }

        public OleExecPool(string ConnStrName, bool ReadXMLConfig)
        {
            LoadSetting();
            if (ReadXMLConfig)
            {
                ConnectionManager.Init();
                _ConnectionString = ConnectionManager.GetConnString(ConnStrName);
            }
            else
            {
                _ConnectionString = ConfigurationManager.AppSettings[ConnStrName];
            }
            while (All.Count < MinPoolSize)
            {
                CreateNewItem();
            }
            AutoTimer.Interval = 60000;
            AutoTimer.Elapsed += new System.Timers.ElapsedEventHandler(AutoTimer_Elapsed);
            AutoTimer.Enabled = true;
        }

        public void LoadSetting()
        {
            try
            {
                MaxPoolSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxPoolSize"]);
                MinPoolSize = Convert.ToInt32(ConfigurationManager.AppSettings["MinPoolSize"]);
                PoolTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["PoolTimeOut"]);
                ActiveTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["ActiveTimeOut"]);
                BorrowTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["BorrowTimeOut"]);
            }
            catch { }
        }

        public void Collect()
        {
            CallLockPool("Collect", new object[0]);
        }
        private void _Collect()
        {
            OutPutMessage.Clear();
            
            try
            {
                Debug = "List<OleExecPoolItem> del = new List<OleExecPoolItem>()";
                List<OleExecPoolItem> del = new List<OleExecPoolItem>();
                //檢查對象超時
                foreach (OleExecPoolItem i in All)
                {
                    if ((DateTime.Now - i.CreateTime).TotalSeconds >= ActiveTimeOut)
                    {
                        del.Add(i);
                    }
                }
                Debug = "foreach (OleExecPoolItem i in del)";
                //刪除超時對象
                foreach (OleExecPoolItem i in del)
                {
                    All.Remove(i);
                    try
                    {
                        i.Data.Pool = null;
                        i.Data.FreeMe();
                    }
                    catch (Exception e)
                    {
                        OutPutMessage.Add(e.Message);
                    }
                }
                Debug = "del.Clear();";
                del.Clear();
                List<OleExec> remove = new List<OleExec>();
                OleExec[] arry = new OleExec[Lend.Keys.Count];
                Lend.Keys.CopyTo(arry, 0);
                Debug = "foreach (OleExec o in arry)";
                foreach (OleExec o in arry)
                {
                    try
                    {
                        Debug = "double lendLong = (DateTime.Now - Lend[o].LendTime).TotalSeconds;";
                        double lendLong = 0;
                        try
                        {
                            lendLong = (DateTime.Now - Lend[o].LendTime).TotalSeconds;
                        }
                        catch
                        {
                            remove.Add(o);
                            continue;
                        }
                        Debug = "if (lendLong > BorrowTimeOut)";
                        if (lendLong > o.PoolBorrowTimeOut)
                        {
                            Debug = "remove.Add(o);";
                            remove.Add(o);
                            
                        }
                    }
                    catch
                    {
                        remove.Add(o);
                    }
                }
                Debug = " foreach (OleExec r in remove)";
                foreach (OleExec r in remove)
                {
                    try
                    {
                        var b = Lend[r];
                        Debug = "Lend.Remove(r);";
                        Lend.Remove(r);
                        Debug = "b.Data.FreeMe();";
                        //b.Data.FreeMe();
                        FreeTemp.Insert(0, b.Data);
                        Debug = "var CurrFunction = b.Tag?.GetCurrFunction();";
                        var CurrFunction = b.Tag?.GetCurrFunction();
                        Debug = "BorrowTimeOutFunction.Add(b.BorrowFunction+ >> + CurrFunction);";
                        BorrowTimeOutFunction.Add(b.BorrowFunction+">>"+ CurrFunction);
                    }
                    catch
                    { }
                }
                remove.Clear();
                Debug = "while ((All.Count + Lend.Count) < MinPoolSize)";
                //創建新對象
                while ((All.Count + Lend.Count) < MinPoolSize)
                {
                    CreateNewItem();
                }
            }
            catch (Exception ee)
            {

                OutPutMessage.Add(Debug + ":" + ee.Message);

            }
            
        }


        public void FreePool()
        {
            CallLockPool("FreePool", new object[0]);
        }
        private void _FreePool()
        {

            foreach (OleExecPoolItem i in All)
            {
                i.Data.FreeMe();
            }
            All.Clear();
            Lend.Clear();
            OutPutMessage.Clear();
            
        }

        void AutoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Collect();
            Thread T = new Thread(new ThreadStart(CleanFreeTemp));
            T.Start();
            T.Join(5000);
            if (T.ThreadState == System.Threading.ThreadState.Running)
            {
                T.Abort();
            }
        }

        void CleanFreeTemp()
        {
            try
            {
                while (FreeTemp.Count > 0)
                {
                    var s = FreeTemp[0];
                    FreeTemp.Remove(s);
                    s.FreeMe();
                }
            }
            catch
            { }
        }

        void AutoULockTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //UNLock();
        }

        void CreateNewItem()
        {
            OleExecPoolItem Item = null;
            OleExec newOle = new OleExec(_ConnectionString , this);
            newOle.PoolItem = Item;
            Item = new OleExecPoolItem();
            Item.DBPool = this;
            newOle.PoolItem = Item;
            Item.Data = newOle;
            Item.CreateTime = DateTime.Now;
            Item.LendTime = DateTime.MinValue;
            All.Add(Item);
        }

        /// <summary>
        /// 借出一個連接
        /// </summary>
        /// <returns></returns>
        public OleExec Borrow(I_LockThread tag = null)
        {
            StackFrame frame = new StackFrame(1);
            var className = frame.GetMethod().ReflectedType.FullName;
            var FunctionName = frame.GetMethod().Name;
            var ret = (OleExec)CallLockPool("Borrow", new object[1] { BorrowTimeOut });

            ret.PoolItem.BorrowFunction = className + "." + FunctionName;
            ret.PoolItem.Tag = tag;
            return ret;
        }
        public OleExec Borrow(int TimeOut, I_LockThread tag = null)
        {
            StackFrame frame = new StackFrame(1);
            var className = frame.GetMethod().ReflectedType.FullName;
            var FunctionName = frame.GetMethod().Name;
            var ret= (OleExec)CallLockPool("Borrow", new object[1] { TimeOut });
            ret.PoolItem.BorrowFunction = className + "." + FunctionName;
            ret.PoolItem.Tag = tag;
            return ret;
        }

        private OleExec _Borrow(int TimeOut)
        {
            
            OleExec ret = null;
            OleExecPoolItem Item = null;
            try
            {
                int retrycount = 10;
                while (true)
                {
                    if (All.Count == 0 && Lend.Count < MaxPoolSize)
                    {
                        CreateNewItem();
                    }
                    if (All.Count > 0)
                    {

                        Item = All[0];
                        Item.BorrowTimeOut = TimeOut;
                        All.Remove(Item);
                        
                        ret = Item.Data;
                        Item.LendTime = DateTime.Now;
                        Lend.Add(ret, Item);

                        try
                        {
                            var db = Item.Data;
                            db.RunSelect("select * from dual");
                        }
                        catch (Exception eee)
                        {
                            Lend.Remove(ret);
                            if (retrycount > 0)
                            {
                                retrycount--;
                                continue;
                            }
                            throw new Exception("Check Borrow Time Out");

                        }
                        break;
                    }
                    else
                    {
                        throw new Exception("連接池超過最大配置,無法借出");
                    }
                }
                
            }
            catch (Exception ee)
            {

                throw ee;
            }
            finally
            {

            }

            return ret;
            
        }
        /// <summary>
        /// 向連接池歸還連接
        /// </summary>
        /// <param name="db"></param>
        public void Return(OleExec db)
        {
            if (db != null)
            {
                db.PoolItem.BorrowFunction = "";
                CallLockPool("Return", new object[] { db });
            }
        }
        private void _Return(OleExec db)
        {

            
                try
                {
                    db.RollbackTrain();
                }
                catch
                { }
            try
            {
                OleExecPoolItem item = Lend[db];
                All.Add(item);
                Lend.Remove(db);
            }
            catch
            {
                try
                {
                    db.FreeMe();
                }
                catch
                { }
            }
            finally
            {

            }
            
        }

        private List<string> _GetBorrowFunction()
        {
            List<string> ret = new List<string>();
            var keys = Lend.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                ret.Add(Lend[keys[i]].BorrowFunction);
            }
            return ret;
        }
        public List<string> GetBorrowFunction()
        {
            List<string> ret =(List<string>) CallLockPool("GetBorrowFunction");
            return ret;
        }

        public bool TestBorrow(OleExec db)
        {
            if (Lend.ContainsKey(db))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private object CallLockPool(string Function, object[] args = null)
        {
            bool isLockOK = false;
            try
            {
                if (Monitor.TryEnter(objlock, 8000))
                {
                    LockFunction = Function;
                    isLockOK = true;
                    switch (Function)
                    {
                        case "Borrow":
                            return _Borrow((int)args[0]);
                        case "Return":
                            _Return((OleExec)args[0]);
                            return new object();
                        case "Collect":
                            _Collect();
                            return new object();
                        case "FreePool":
                            _FreePool();
                            return new object();
                        case "CleanAll":
                            _CleanAll();
                            return new object();
                        case "ShowLend":
                            return _ShowLend();
                        case "GetBorrowFunction":
                            return _GetBorrowFunction();
                    }
                }
                else
                {
                    //FreePool();
                    string errMsg = "DBPool LOCK TIMEOUT! Function:" + Function + " PoolLock function:" + LockFunction + " DEBUG:" + Debug;
                    // 多線程同時調用一個日誌文件報錯！線程卡死 JAM LI叫註釋 
                    //using (FileStream fs = new FileStream("poolerrlog.txt", FileMode.Append))
                    //{
                    //    StreamWriter sw = new StreamWriter(fs);
                    //    sw.WriteLine(DateTime.Now);
                    //    sw.WriteLine(errMsg);
                    //    sw.Flush();
                    //    sw.Close();
                    //}
                    //FileStream fs = new FileStream("poolerrlog.txt", FileMode.Append);
                    //StreamWriter sw = new StreamWriter(fs);
                    //sw.WriteLine(DateTime.Now);
                    //sw.WriteLine(errMsg);
                    //sw.Flush();
                    //sw.Close();
                    throw new Exception(errMsg);

                }
            } catch (Exception ee)
            {
                //FreePool();
                throw ee;
            } finally
            {
                if (isLockOK)
                {
                    Monitor.Exit(objlock);
                }
            }
            throw new Exception("没有此函数");
        }
    }
    public class OleExecPoolItem
    {
        public OleExec Data;
        public DateTime CreateTime;
        public DateTime LendTime;
        public int BorrowTimeOut;
        public string BorrowFunction;
        public I_LockThread Tag;
        public OleExecPool DBPool;
    }
}
