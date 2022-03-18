using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.OM;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESJuniper.Stations.StationActions.DataLoaders
{
    public class SkuDataLoader
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void JNPPNCovertFoxconnPN(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession JPNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (JPNSession == null)
            {
                JPNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(JPNSession);
            }
            MESStationSession FPNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (FPNSession == null)
            {
                FPNSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FPNSession);
            }
            string inputValue = Input.Value.ToString();
            try
            {
                var FPN = Station.SFCDB.ORM.Queryable<O_AGILE_ATTR>()
                    .Where(t => t.CUSTPARTNO == inputValue || t.ITEM_NUMBER == inputValue)
                    .Select(t => t.ITEM_NUMBER)
                    .ToList();
                FPNSession.Value = FPN.FirstOrDefault();
                Input.Value = FPNSession.Value;
                Station.AddMessage("MES00000029", new string[] { "Foxconn PN:", FPN.FirstOrDefault() }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
