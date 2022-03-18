using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.OM;
using MESJuniper.TruckLoad;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESJuniper.Stations.StationActions.DataLoaders
{
    public class ShipoutDataLoader
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

        public static void GetTruckLoadOpenTOList(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession Trailer = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Trailer == null || Trailer.Value == null || Trailer.Value.ToString().Trim() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession TOListSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOListSession == null)
            {
                TOListSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TOListSession);
            }
            string inputValue = Input.Value.ToString();
            try
            {
                var truckload = new TruckLoadLogic(Station.SFCDB.ORM);
                var OpenToList = truckload.GetOpenTOList(Trailer.Value.ToString()).Select(t => t.TO_NO).Distinct().ToList();
                TOListSession.Value = OpenToList;
                Station.AddMessage("MES00000029", new string[] { Trailer.Value.ToString(), "TO List " }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GetTruckLoadPalletList(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession TONumber = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONumber == null || TONumber.Value == null || TONumber.Value.ToString().Trim() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession PalletIDList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PalletIDList == null)
            {
                PalletIDList = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(PalletIDList);
            }
            string inputValue = Input.Value.ToString();
            try
            {
                var truckload = new TruckLoadLogic(Station.SFCDB.ORM);
                var OpenToList = truckload.GetPhysicPalletList(TONumber.Value.ToString());
                PalletIDList.Value = OpenToList;
                Station.AddMessage("MES00000029", new string[] { TONumber.Value.ToString(), "PalletID List " }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void GetTruckLoadPackList(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PalletID = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PalletID == null || PalletID.Value == null || PalletID.Value.ToString().Trim() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession PackList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PackList == null)
            {
                PackList = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(PackList);
            }
            string inputValue = Input.Value.ToString();
            try
            {
                var truckload = new TruckLoadLogic(Station.SFCDB.ORM);
                var OpenToList = truckload.GetPackList(PalletID.Value.ToString());
                PackList.Value = OpenToList;
                Station.AddMessage("MES00000029", new string[] { PalletID.Value.ToString(), "Pack List " }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static void GetTruckLoadTOData(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession TONumber = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TONumber == null || TONumber.Value == null || TONumber.Value.ToString().Trim() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession TOQTY = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TOQTY == null)
            {
                TOQTY = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TOQTY);
            }
            string inputValue = Input.Value.ToString();
            try
            {
                var truckload = new TruckLoadLogic(Station.SFCDB.ORM);
                var qty = truckload.GetTruckLoadDetail(TONumber.Value.ToString()).Count;
                TOQTY.Value = qty;
                Station.AddMessage("MES00000029", new string[] { TONumber.Value.ToString(), "TO Qty " }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Get Physic Pallet Detail Data
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetTruckLoadPalletData(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession PalletID = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PalletID == null || PalletID.Value == null || PalletID.Value.ToString().Trim() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession PalletQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PalletQty == null)
            {
                PalletQty = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(PalletQty);
            }
            MESStationSession CountryCode = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (CountryCode == null)
            {
                CountryCode = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(CountryCode);
            }
            string inputValue = Input.Value.ToString();
            try
            {
                var truckload = new TruckLoadLogic(Station.SFCDB.ORM);
                var packlist = truckload.GetPackList(PalletID.Value.ToString());
                PalletQty.Value = packlist.Count;
                Station.AddMessage("MES00000029", new string[] { PalletID.Value.ToString(), "Pack List " }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
