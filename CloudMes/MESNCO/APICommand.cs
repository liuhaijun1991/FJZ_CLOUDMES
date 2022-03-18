using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESNCO
{
    public class APICommand
    {
        public static APICommand Comm = new APICommand();
        public string StrCommandCode;

        public List<APICommandAction> Actions = new List<APICommandAction>();
        public int CurrActionIndex = 0;
        public string CommandStatus = CommandState.WaitCMCReturn;
        public int SyncTimeOut = 5000;
        public APICommand()
        {

        }
        public void DoAction( object data)
        {
            if (CurrActionIndex >= 0 && CurrActionIndex < Actions.Count)
            {
                Actions[CurrActionIndex].DoAction( data);
            }

        }
        public void toNextAction()
        {
            if (CurrActionIndex < Actions.Count - 1)
            {
                CurrActionIndex++;
                CommandStatus = CommandState.CommandStart;
            }
            else
            {
                CurrActionIndex = 0;
                CommandStatus = CommandState.CommandEnd;
            }
        }
    }

    public class APICommandAction
    {
        public string StrActionName;
        public APICommand comm;
        public object AutoRunData = null;
        public virtual void DoAction( object data)
        {

        }

        public void ActionCallBack( object data)
        {

        }
    }

    public class CommandState
    {
        public static string CommandEnd = "CommandEnd";
        public static string CommandStart = "CommandStart";
        public static string NewAction = "NextAction";
        public static string WaitMesReturn = "WaitMesReturn";
        public static string WaitCMCReturn = "WaitCMCReturn";

    }
}
