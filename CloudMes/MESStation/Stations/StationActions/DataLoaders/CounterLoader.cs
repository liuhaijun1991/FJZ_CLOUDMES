using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class CounterLoader
    {
        public static void LoadWoFromInput(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
        }

        //計數器初始化
        public static void CounterInitDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession CounterSavePoint = Station.StationSession.Find(t => t.MESDataType == "COUNT" && t.SessionKey == "1");
            if (CounterSavePoint == null)
            {
                CounterSavePoint = new MESStationSession() { MESDataType = "COUNT", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(CounterSavePoint);
            }
            //CounterSavePoint.Value = Paras.Find(p => p.SESSION_TYPE == "COUNT").VALUE;
            R_Station_Action_Para para = Paras.Where(p => p.SESSION_TYPE == "COUNT").FirstOrDefault();
            if (para==null)
            {
                CounterSavePoint.Value = 0;
            }
            else{
                CounterSavePoint.Value = int.Parse( Paras.Where(p => p.SESSION_TYPE == "COUNT").FirstOrDefault().VALUE);
            }
            Station.AddMessage("MES00000029", new string[] { "COUNT", "InitCount" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }
    }
}
