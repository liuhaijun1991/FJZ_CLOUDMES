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
    public class StationLineChecker
    {
        /// <summary>
        /// Check Station Line Correct,Cann't be the defualt Name  'Line 1'
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LineCorrectChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Station.Line == null || Station.Line == "" || Station.Line== "Line1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_REJECT_LINE_DEFAULT", new string[] { Station.Line }));
            }
        }
        
        /// <summary>
        /// One Unit Run At The Same Line 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TheSameLineChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var sn = SNSession.Value.ToString();
            var line = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.LINE != "Line1").Select(t => t.LINE).ToList();
            if (line.Count > 0 && !line.Contains(Station.Line))
            {
                //VN PE Request: just Juniper-Optics Series need to check this unit
                var controlList = Station.SFCDB.ORM.Queryable<R_SN, C_SKU, C_SERIES, R_F_CONTROL>((a, b, c, d) => a.SKUNO == b.SKUNO && b.C_SERIES_ID == c.ID && SqlSugar.SqlFunc.ToUpper(c.SERIES_NAME) == SqlSugar.SqlFunc.ToUpper(d.VALUE))
                    .Where((a, b, c, d) => a.VALID_FLAG == "1" && d.FUNCTIONNAME == "CHECK_SAMELINE" && d.CONTROLFLAG == "Y" && d.FUNCTIONTYPE == "NOSYSTEM" && a.SN == sn)
                    .Select((a, b, c, d) => d).ToList();
                if (controlList.Count == 0)
                    return;

                var lines = "";
                line.ForEach(t => { lines += t + ","; });
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_NEED_SCAN_AT_THE_SAME_LINE", new string[] { lines, Station.Line }));
            }
        }

        /// <summary>
        /// Not The Same Order Cann't Pass Station
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NotTheSameOrderRejectChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (Paras[1].VALUE == null || Paras[1].VALUE == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            var SN = SNSession.Value.ToString();
            var SKU = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == SN && t.VALID_FLAG == "1").Select(t => t.SKUNO).First();
            var SKUType = Station.SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == SKU).Select(t => t.SKU_TYPE).First();

            if (SKUType == "OPTICS")
            {
                var CurrentStation = Station.StationName;
                var EndStation = Paras[1].VALUE.ToString();
                var Line = Station.Line;
                string sql = $@"SELECT *
                                  FROM R_SN A
                                 WHERE EXISTS (SELECT STATION_NAME
                                          FROM C_ROUTE_DETAIL B
                                         WHERE B.ROUTE_ID = A.ROUTE_ID
                                           AND EXISTS (SELECT SEQ_NO
                                                  FROM C_ROUTE_DETAIL C
                                                 WHERE C.ROUTE_ID = B.ROUTE_ID
                                                   AND C.SEQ_NO<B.SEQ_NO
                                                   AND STATION_NAME = '{CurrentStation}')
                                           AND EXISTS (SELECT SEQ_NO
                                                  FROM C_ROUTE_DETAIL D
                                                 WHERE D.ROUTE_ID = B.ROUTE_ID
                                                   AND D.SEQ_NO>B.SEQ_NO
                                                   AND STATION_NAME = '{EndStation}')
                                           AND A.NEXT_STATION = B.STATION_NAME)
                                AND EXISTS (SELECT *
                                          FROM R_SN_STATION_DETAIL E
                                         WHERE E.R_SN_ID = A.ID
                                           AND E.LINE = '{Line}')
                                   AND EXISTS (SELECT *
                                          FROM R_SN F
                                         WHERE A.WORKORDERNO <> F.WORKORDERNO
                                           AND F.SN = '{SN}' AND F.VALID_FLAG=1)
                                   AND A.SN NOT LIKE '~%' AND A.SN NOT LIKE '#%' AND A.SN NOT LIKE '*%'
                                   AND A.VALID_FLAG = 1";
                var snlist = Station.SFCDB.ORM.SqlQueryable<R_SN>(sql).ToList();
                if (snlist.Count > 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE_REJECT_NOT_THE_SAME_WO", new string[] { CurrentStation, snlist[0].WORKORDERNO, EndStation }));
                }
            }
        }
    }
}
