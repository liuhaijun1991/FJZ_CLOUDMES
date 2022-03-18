using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MESMailCenter
{
    /// <summary>
    /// 對象池容器
    /// </summary>
    public class ObjectPool
    {
        /// <summary>
        /// 
        /// </summary>
        protected Hashtable _CanUse = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 
        /// </summary>
        protected Hashtable _CanNotUse = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 
        /// </summary>
        protected int _PoolMaxSize = 100;
        /// <summary>
        /// 
        /// </summary>
        protected int _PoolMinSize = 2;
        /// <summary>
        /// 
        /// </summary>
        protected TimeSpan _TimeOut = new TimeSpan(1, 0, 0); 
        /// <summary>
        /// 
        /// </summary>
        public int MaxSize
        {
            get
            {
                return _PoolMaxSize;
            }
            set
            {
                _PoolMaxSize = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int MinSize
        {
            get
            {
                return _PoolMinSize;
            }
            set
            {
                _PoolMinSize = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan TimeOut
        {
            get
            {
                return new TimeSpan(_TimeOut.Days, _TimeOut.Hours, _TimeOut.Minutes, _TimeOut.Seconds, _TimeOut.Milliseconds);
            }
            set
            {
                _TimeOut = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public ObjectPool()
        { 
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaxSize"></param>
        /// <param name="minSize"></param>
        /// <param name="TimeOut"></param>
        public ObjectPool(int MaxSize , int minSize,TimeSpan TimeOut)
        {
            _PoolMaxSize = MaxSize;
            _PoolMinSize = minSize;
            _TimeOut = TimeOut;
        }
        /// <summary>
        /// 虛方法,生成一個新的池成員
        /// </summary>
        /// <returns></returns>
        protected virtual object NewItem()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 虛方法,釋放一個池中的成員
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool DelItem(object value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 獲取一個池中的對象
        /// </summary>
        /// <returns></returns>
        public Object GetValue()
        {
            if (_CanUse.Count == 0)
            {
                if (_CanNotUse.Count < _PoolMaxSize)
                {
                    object o = NewItem();

                    if (o == null)
                    {
                        throw new Exception("Err<ObjectPool.GetValue>" + "調用AddItem失敗");
                    }
                    else
                    {
                        this._CanUse.Add(o, DateTime.Now);
                    }
                }
                else
                {
                    throw new Exception("Err<ObjectPool.GetValue>" + "超過池最大上線,PoolMaxSize="+_PoolMaxSize.ToString());
                }
            }
            
            foreach (object key in _CanUse.Keys)
            {
                DateTime creatTime = (DateTime)_CanUse[key];
                _CanUse.Remove(key);
                _CanNotUse.Add(key, creatTime);
                return key;
           
            }
            throw new Exception("Err<ObjectPool.GetValue>未知異常");
        }
        /// <summary>
        /// 歸還借出的對象
        /// </summary>
        /// <param name="o"></param>
        public void ReturnValue(object o)
        {
            if (!_CanNotUse.ContainsKey(o))
            {
                throw new Exception("Err<ObjectPool.ReturnValue>" + "歸還的對象不屬於本池");
            }
            DateTime creatTime = (DateTime)_CanNotUse[o];
            if (DateTime.Now - creatTime > _TimeOut && _CanUse.Count > _PoolMinSize)
            {
                _CanNotUse.Remove(o);
                DelItem(o);
            }
            else
            {
                _CanNotUse.Remove(o);
                _CanUse.Add(o, creatTime);
            }

        }


    }
}
