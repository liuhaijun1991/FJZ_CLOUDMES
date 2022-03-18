using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESCMCHost.Commamd
{
    public class SetLine : CMCCommand
    {
        public string InputLine;
        public SetLine()
        {
            StrCommandCode = "SetLine";
            Actions.Add(new inputLine(this));
           
        }

    }

    public class inputLine : CMCCommandAction
    {
        public inputLine(CMCCommand cmd)
        {
            this.comm = cmd;
            this.StrActionName = "LINE:";
        }
        public override void DoAction(CMC503Scanda Scanda, object data)
        {
            ((SetLine)this.comm).InputLine = data.ToString();
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Config.LineConfig";
            mesdata.Function = "GetLineByLikeSearch";
            mesdata.Data = new { LINE_NAME="" };
            comm.CommandStatus = CommandState.WaitMesReturn;
            var JO = Scanda.MESAPI.CallMESAPISync(mesdata, comm.SyncTimeOut);
            string line = ((SetLine)this.comm).InputLine;
            if (JO["Status"].ToString() == "Pass")
            {
                for (int i = 0; i < JO["Data"].Count(); i++)
                {
                    if (JO["Data"][i]["LINE_NAME"].ToString() == line)
                    {
                        Scanda.Line = line;
                    }
                }
                if (Scanda.Line == line)
                {
                    Scanda.SendTextDataToCMC($@"Set Line '{line}' OK!");
                }
                else
                {
                    Scanda.SendTextDataToCMC($@"Line '{line}' unsuitable ");
                    comm.CommandStatus = CommandState.CommandStart;
                    comm.CurrActionIndex = 0;
                    return;
                }
                comm.toNextAction();

            }
            else
            {
                Scanda.SendTextDataToCMC(JO["Message"].ToString());
                comm.toNextAction();
            }

        }
        
    }
}
