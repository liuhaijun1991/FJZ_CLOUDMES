using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESStation
{
    public class MESCallUIFunction
    {
        public string ServerMessageID;
        public string ClientID;
        public string MessageID;
        public UIInput FunctionType;
        public DateTime? CallTime;
        public DateTime? ReturnTime;
        public object Data;
        public Dictionary<string, object> Labels;
    }

    public class CallUIFunctionReturnCatch
    {
        public object CallData;
        public object ReturnData;
        public StationLayerReturnType StationLayerReturnType;
        public bool IsTheadCancel = false;
    }
}
