using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MESPubLab.MESStation
{
    public class MESStationReturn
    {
        public string Status;
        public string Message;
        protected string _MessageID;
        public object Data;
        public string MessageCode;
        protected string _ClientID;
        public DateTime StartTime;
        public DateTime EndTime;
        public double ProccessTime;
      
        [ScriptIgnore]
        public List<object> MessagePara = new List<object>();

        public MESStationReturn(string msgID  ,string ClientID)
        {
            _MessageID = msgID;
            _ClientID = ClientID;
        }
        public MESStationReturn()
        {
        }

        public string MessageID
        {
            get
            { return _MessageID; }
        }

        public string ClientID
        {
            get
            { return _ClientID; }
        }

    }


    public class StationReturnStatusValue
    {
        public static string Pass = "Pass";
        public static string Fail = "Fail";
        public static string PartialSuccess = "PartialSuccess";
    }
}
