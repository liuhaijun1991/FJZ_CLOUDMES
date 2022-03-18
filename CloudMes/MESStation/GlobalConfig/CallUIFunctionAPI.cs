using MESPubLab.MESStation;
using System;
using System.Collections.Generic;

namespace MESStation.GlobalConfig
{
    public class CallUIFunctionAPI : MesAPIBase
    {
        private APIInfo _UIFunctionCallBack = new APIInfo()
        {
            FunctionName = "UIFunctionCallBack",
            Description = "UIFunction回调",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName = "ServerMessageID", InputType = "String", DefaultValue=""},
                new APIInputInfo(){ InputName = "RetDATA", InputType = "Json", DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        public CallUIFunctionAPI()
        {
            this.Apis.Add(_UIFunctionCallBack.FunctionName, _UIFunctionCallBack);
        }

        public void UIFunctionCallBack(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            try
            {
                var StationLayerReturnType = (StationLayerReturnType)System.Enum.Parse(typeof(StationLayerReturnType), Data["StationLayerReturnType"].ToString(), true);
                var RetDATA = Data["StationLayerData"];
                var ServerMessageID = Data["ServerMessageID"].ToString();
                if (UIReturn.ContainsKey(ServerMessageID))
                {
                    var ret = UIReturn[ServerMessageID];
                    ret.ReturnData = RetDATA;
                    ret.StationLayerReturnType = StationLayerReturnType;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Err: No Find msgid";
                }
            }
            catch (Exception e)
            {
                
                throw e;
            }

        }
    }
}
