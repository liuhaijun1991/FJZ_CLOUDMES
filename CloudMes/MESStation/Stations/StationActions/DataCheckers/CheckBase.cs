using MESDataObject;
using MESPubLab.MESStation;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckBase
    {
        public static void RuleChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession RuleSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (RuleSession == null || RuleSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession ValueForCheck = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (ValueForCheck == null || ValueForCheck.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
             List<string> rules = (List<string>)RuleSession.Value;   

            bool SnRuleFlag = true;
            if (rules.Count > 0)
            {
                SnRuleFlag = false;
                for (int i = 0; i < rules.Count; i++)
                {
                    SnRuleFlag = Regex.IsMatch(ValueForCheck.Value.ToString(), rules[i]);
                    if (SnRuleFlag)
                    {
                        RuleSession.InputValue = rules[i];
                        break;
                    }
                }
            }
            if (SnRuleFlag)
            {
                Station.AddMessage("MES00000046", new string[] { "RuleChecker" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190307123227", new string[] { ValueForCheck.Value.ToString() }));
            }
        }
    }
}
