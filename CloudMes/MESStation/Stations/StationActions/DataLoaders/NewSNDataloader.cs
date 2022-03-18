using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class NewSNDataloader
    {
        /// <summary>
        /// Sn條碼長度如果是9位，則取前8位
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadSnFromInput(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            string NewSN = Input.Value.ToString();

            if (NewSN.Length != 8 && NewSN.Length != 9)
            {
               // Station.AddMessage("MES00000022", new string[] { "SN" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                //Station.NextInput = Station.Inputs[2];
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000022", new string[] { "SN" }));
            }
            else
            {
                Input.Value = NewSN.Substring(0, 8);
            }


            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }


            //if (Input.Value.ToString().Length == 8)
            //{
            //    //Station.AddMessage("MES00000029", new string[] { "Sn", Input.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            //    //Station.NextInput = Station.Inputs[1];
            //}
            //else if (NewSN)
            //{
            //    Station.AddMessage("MES00000029", new string[] { "LinkQTY", LinkNum.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            //    Station.NextInput = Station.Inputs[1];
            //}
            //else
            //{
            //    Station.AddMessage("MES00000020", new string[] { "LinkQTY", "Number" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            //    Station.NextInput = Station.Inputs[3];
            //}
            //Station.NextInput = Station.Inputs[2];
            ////Station.DBS["APDB"].Borrow()
            //LogicObject.WorkOrder WO = new LogicObject.WorkOrder();
            //WO.WO = Input.Value.ToString();
            //s.Value = WO;
            //s.InputValue = Input.Value.ToString();
            //s.ResetInput = Input;
            //Station.AddMessage("MES00000029", new string[] { "Workorder", WO.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Message);
            //Station.AddMessage("MES00000029", new string[] { "Workorder", WO.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            //Station.AddMessage("MES00000029", new string[] { "Workorder", WO.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
        }
        #region LoadSn edited by htz 201808210528
        public static void LoadSn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            string NewSN = Input.Value.ToString();

            if (NewSN.Length != 8 && NewSN.Length != 9)
            {     
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000022", new string[] { "SN" }));
            }
            else
            {
                Input.Value = NewSN.Substring(0, 8);
            }
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (s == null)
            {
                s = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(s);
            }
     }
#endregion
    }
}
