using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESCMCHost
{
    public class CMCCommand
    {
        public static CMCCommand Comm = new CMCCommand();
        public string StrCommandCode;

        public List<CMCCommandAction> Actions = new List<CMCCommandAction>();
        public int CurrActionIndex = 0;
        public string CommandStatus = CommandState.WaitCMCReturn;
        public int SyncTimeOut = 5000;
        public CMCCommand()
        {

        }
        public void DoAction(CMC503Scanda scanda, object data)
        {
            if (CurrActionIndex >= 0 && CurrActionIndex < Actions.Count)
            {
                Actions[CurrActionIndex].DoAction(scanda, data);
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

    public class CMCCommandAction
    {
        public string StrActionName;
        public CMCCommand comm;
        public object AutoRunData = null;
        public virtual void DoAction(CMC503Scanda Scanda, object data)
        {

        }

        public void ActionCallBack(CMC503Scanda Scanda, object data)
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
