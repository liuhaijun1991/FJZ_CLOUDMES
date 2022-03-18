using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class IODataLoader
    {
        public static void GetStringFromDialog(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
            var config = Paras.Find(t => t.SESSION_TYPE == "TITTLE");
            string Tittle = "";
            if (config != null)
            {
                Tittle = config.VALUE.ToString();
            }
            config = Paras.Find(t => t.SESSION_TYPE == "MESSAGE");
            string MESSAGE = "";
            if (config != null)
            {
                MESSAGE = config.VALUE.ToString();
            }
            config = Paras.Find(t => t.SESSION_TYPE == "ERR_MSG");
            string ERR_MSG = "INPUT ERR";
            if (config != null)
            {
                ERR_MSG = config.VALUE.ToString();
            }

            UIInputData O = new UIInputData()
            { Timeout = 50000, IconType = IconType.None, Message = MESSAGE, Tittle = Tittle, Type = UIInputType.String, Name = s.MESDataType, ErrMessage = ERR_MSG };
            var input = O.GetUiInput(Station.API, UIInput.Normal, Station);
            Input.Value = input.ToString();
            s.Value = Input.Value;
        }
    }
}
