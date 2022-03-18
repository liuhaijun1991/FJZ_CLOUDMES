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
    public class Rework
    {
        public static void ReworkSelectStationInputInit(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            var input = Station.Inputs.Find(t => t.DisplayName == "SelectStation");
            input.DataForUse.Add("手动选择");
            input.DataForUse.Add("自动选择");
            
        }
    }
}
