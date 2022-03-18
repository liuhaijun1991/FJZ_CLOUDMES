using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESDBHelper;
using SqlSugar;
using MESPubLab.MESStation.MESReturnView.Station;


namespace MESStation.Stations.StationActions.StationInit
{
    public class DataForUseInit
    {
        public static void InitInput(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            var inputName = Paras.Find(t => t.SESSION_TYPE == "INPUT_NAME")?.VALUE;
            var VALUES = Paras.FindAll(t => t.SESSION_TYPE == "VALUE");
            var defValue = Paras.Find(t => t.SESSION_TYPE == "DEF_VALUE");
            var loadDefValue = Paras.Find(t => t.SESSION_TYPE == "WRITE_SESSION");

            var input = Station.Inputs.Find(t => t.DisplayName == inputName);
            input.DataForUse.Clear();
            for (int i = 0; i < VALUES.Count; i++)
            {
                input.DataForUse.Add(VALUES[i].VALUE);
            }
            if (defValue != null)
            {
                input.Value = defValue.VALUE;
            }

            if (loadDefValue != null && defValue != null)
            {
                var session = Station.StationSession.Find(t => t.MESDataType == loadDefValue.SESSION_KEY && t.SessionKey == loadDefValue.VALUE);
                if (session == null)
                {
                    session = new MESStationSession() { MESDataType = loadDefValue.SESSION_KEY, SessionKey = loadDefValue.VALUE, Value = defValue.VALUE };
                    Station.StationSession.Add(session);
                }
                session.Value = defValue.VALUE;
            }

        }
    }
}
