using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MESDBHelper
{
    public interface I_LockThread
    {
        Thread LockTread
        {
            set; get;
        }
        string ServerMessageID
        {
            set; get;
        }
        void AddTIMEOUT(int TimeOut);
        void SubTIMEOUT(int TimeOut);
        string GetCurrFunction();
    }
}
