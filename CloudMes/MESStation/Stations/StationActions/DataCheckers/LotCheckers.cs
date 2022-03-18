using MESDataObject;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class LotCheckers
    {
        /// <summary>
        /// 檢查LOT是否關閉
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LotClosingChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionLotObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionLotObject == null )
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            R_LOT_STATUS lotStatus = null;
            Row_R_LOT_STATUS rowLot = null;            
            if (sessionLotObject.Value is LotNo )
            {
                LotNo lotObj = (LotNo)sessionLotObject.Value;
                rowLot = t_r_lot_status.GetByLotNo(lotObj.LOT_NO, Station.SFCDB);
                if (rowLot == null)
                {
                    throw new MESReturnMessage("Get Lot Error!");
                }
                lotStatus = rowLot.GetDataObject();
            }
            else if (sessionLotObject.Value is string )
            {
                rowLot = t_r_lot_status.GetByLotNo(sessionLotObject.Value.ToString().Trim(), Station.SFCDB);
                if (rowLot == null)
                {
                    throw new MESReturnMessage("Get Lot Error!");
                }
                lotStatus = rowLot.GetDataObject();
            }
            else if (sessionLotObject.Value is R_LOT_STATUS)
            {
                lotStatus = (R_LOT_STATUS)sessionLotObject.Value;
            }
            if (lotStatus != null && lotStatus.CLOSED_FLAG == "0")
            {
                throw new MESReturnMessage($@"{lotStatus.LOT_NO} Not Closed,Please click close.");
            }
        }
        /// <summary>
        /// 檢查SN是否進LOT
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNInLotChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string aql_type = Paras[1].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(aql_type))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            
            SN snObject = (SN)SNSession.Value;

            T_C_SKU_AQL t_c_sku_aql = new T_C_SKU_AQL(Station.SFCDB, Station.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL t_r_lot_detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            

            List<C_SKU_AQL> listSkuAQL = t_c_sku_aql.GetAQLListBySkuAndType(Station.SFCDB, snObject.SkuNo, aql_type);    
            List<C_ROUTE_DETAIL> listLastRoute = new List<C_ROUTE_DETAIL>();
            C_ROUTE_DETAIL priorStation = null;
            R_LOT_STATUS lotStatus = null;

            bool bSampletest = false;
            //配置了對應的AQL
            if (listSkuAQL.Count > 0)
            {
                //當前工站的前一站是SAMPLETESTLOT類型
                listLastRoute = t_c_route_detail.GetLastStations(snObject.RouteID, Station.StationName, Station.SFCDB);
                priorStation = listLastRoute.OrderByDescending(r => r.SEQ_NO).ToList().FirstOrDefault();
                bSampletest = t_c_route_detail.RouteEXExist(priorStation.ID, "SAMPLETESTLOT", "SAMPLETESTLOT", Station.SFCDB);
                lotStatus = t_r_lot_status.GetLotBySNAndStation(snObject.SerialNo, Station.StationName, Station.SFCDB);
                if (bSampletest && lotStatus == null)
                {
                    throw new MESReturnMessage($@"{snObject.SerialNo} Not In Lot！Please Go To Scan {priorStation.STATION_NAME}");
                }
            }

        }
    }
}
