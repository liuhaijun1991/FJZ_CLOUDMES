using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;

namespace MESStation.Test
{
    public class InputValueTest:MesAPIBase
    {
        APIInfo GetSkusAPI = new APIInfo()
        {
            FunctionName = "GetSkus",
            Description = "獲取SKU列表",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "HWD" }
            },
            Permissions = new List<MESPermission>() { }
        };

        APIInfo GetPoBuySKUAPI = new APIInfo()
        {
            FunctionName = "GetPoBuySKU",
            Description = "獲取PO列表",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "A03022ASS-A" }
            },
            Permissions = new List<MESPermission>() { }
        };
        public InputValueTest()
        {
            Apis.Add(GetSkusAPI.FunctionName, GetSkusAPI);
            Apis.Add(GetPoBuySKUAPI.FunctionName, GetPoBuySKUAPI);
        }
        public void GetSkus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            List<string> ret = new List<string> {"A03022ASS-A", "A03023UBB-A", "AHH03022ASD-A" };
            StationReturn.Data = ret;
            StationReturn.Status = StationReturnStatusValue.Pass;
        }

        public void GetPoBuySKU(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string skuno = Data["SKUNO"].ToString();
            List<string> ret = new List<string>();
            if (skuno == "A03022ASS-A")
            {
                ret.Add("00045879");
                ret.Add("00045880");
                ret.Add("00045881");
                //"{SKUNO:[SKU],TT:'2222'}"
            }else if(skuno == "A03023UBB-A")
            {
                ret.Add("00095879");
                ret.Add("00095880");
                ret.Add("00095881");
            }
            
            StationReturn.Data = ret;
            StationReturn.Status = StationReturnStatusValue.Pass;
        }
        public static void TESTGetPoBuySKUDataLoader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string skuno = Input.Value.ToString();
            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "PO");

            List<object> ret = I.DataForUse;
            ret.Clear();
            if (skuno == "A03022ASS-A")
            {
                ret.Add("00045879");
                ret.Add("00045880");
                ret.Add("00045881");
                //"{SKUNO:[SKU],TT:'2222'}"
            }
            else if (skuno == "A03023UBB-A")
            {
                ret.Add("00095879");
                ret.Add("00095880");
                ret.Add("00095881");
            }
            
        }



    }
    
}
