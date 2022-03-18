using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using MESDataObject;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class PanelDataLoaders
    {
        /// <summary>
        /// 從輸入加載Panel
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,Panel的保存位置</param>
        public static void PanelFromInputDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            string strPanel = Input.Value.ToString().Trim().ToUpper();

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                PanelSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(PanelSession);
            }
            else //add by LLF 2018-02-06
            {
                PanelSession.InputValue = strPanel;
                PanelSession.ResetInput = Input;
            }


            MESDBHelper.OleExec SFCDB = Station.SFCDB;



            MESDataObject.Module.T_R_PANEL_SN TRPS = new T_R_PANEL_SN(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<R_PANEL_SN> PanelSNs = TRPS.GetPanel(strPanel, SFCDB);
            if (PanelSNs.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000038", new string[] { strPanel });
                throw new MESReturnMessage(errMsg);
            }
            Panel panel = new Panel(strPanel, SFCDB, DB_TYPE_ENUM.Oracle);
            PanelSession.Value = panel;
        }

        /// <summary>
        /// 獲取Panel未分板數量
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PanelNoBIPQtyDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Panel PanelObj = new Panel();
            T_R_PANEL_SN TablePanel = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            if (Paras.Count <0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            PanelObj = (Panel)PanelSession.Value;

            MESStationSession PanelNoBIPQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PanelNoBIPQtySession == null)
            {
                PanelNoBIPQtySession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(PanelNoBIPQtySession);
            }

            int PanelNoBipQty = TablePanel.PanelNoBIPQty(PanelObj.PanelNo, Station.SFCDB, Station.DBType);

            PanelNoBIPQtySession.Value = PanelNoBipQty;
        }
        public static void PanelNoToSelectDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Panel PanelObj = new Panel();
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            PanelObj = (Panel)PanelSession.Value;

            string InputName = Paras[1].VALUE;

            var input = Station.Inputs.Find(t => t.DisplayName == InputName);
            input.DataForUse.Clear();
            var sns = PanelObj.GetSnDetail(PanelObj.PanelNo, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            input.DataForUse.Add("");
            for (int i = 0; i < sns.Count; i++)
            {
                input.DataForUse.Add(sns[i].SN);
            }
            //input.RefershType = "EveryTime";


        }

    }
}
