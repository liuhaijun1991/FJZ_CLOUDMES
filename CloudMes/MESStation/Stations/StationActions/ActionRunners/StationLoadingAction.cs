using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class StationLoadingAction
    {
        public static void HWTOBAStationLoadingAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            DB_TYPE_ENUM dbtype = Station.DBType;
            OleExec sfcdb = Station.SFCDB;
            MESDataObject.Module.HWT.T_R_OBA_TEMP TROT = new MESDataObject.Module.HWT.T_R_OBA_TEMP(sfcdb, dbtype);
            TROT.DeleteByTypeAndIP(sfcdb, "PALLET", Station.IP);
            TROT.DeleteByTypeAndIP(sfcdb, "SN", Station.IP);
        }
    }
}
