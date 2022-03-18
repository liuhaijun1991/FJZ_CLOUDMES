using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System.Collections;
using MESDataObject;
using System.Data;
using MESDBHelper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class NextInputAction
    {
        public static void SMTLoadingPassNextInputAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            Station.NextInput = Input;
            Station.Inputs[3].Value = "";
        }

        public static void SetNextInputAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //参数1，获取一个内存变量，检查它的值是否为配置值，
            //如是，设置next为配置2的
            //
            R_Station_Action_Para P1 = Paras[0];
            MESStationSession s = Station.StationSession.Find(t => t.MESDataType == P1.SESSION_TYPE && t.SessionKey == P1.SESSION_KEY);
            if (s == null)
            {
                return;
            }
            if (s.Value.ToString() == P1.VALUE.ToString())
            {
                R_Station_Action_Para P2 = Paras[1];
                MESStationInput i = Station.Inputs.Find(t => t.DisplayName == P2.VALUE.ToString());
                if (i != null)
                {
                    Station.NextInput = i;
                }
            }
            else
            {
                R_Station_Action_Para P2 = Paras[2];
                MESStationInput i = Station.Inputs.Find(t => t.DisplayName == P2.VALUE.ToString());
                if (i != null)
                {
                    Station.NextInput = i;
                }
            }



        }

        public static void SetNextInputActionForLoadWaitShipDnData(MESPubLab.MESStation.MESStationBase Station,
            MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region CheckDn Is Shiped
            Newtonsoft.Json.Linq.JObject inputObj =
                (Newtonsoft.Json.Linq.JObject) Newtonsoft.Json.JsonConvert.DeserializeObject(Input.Value.ToString());
            var dnNo = inputObj["DnNo"].ToString();
            var dnLine = inputObj["DnLine"].ToString();
            var rDnData = Station.SFCDB.ORM
                .Queryable<R_DN_STATUS, R_TO_DETAIL>((rds, rtd) =>  rds.DN_NO == rtd.DN_NO && rds.DN_NO == dnNo && rds.DN_LINE == dnLine)
                .Select((rds, rtd) => new { rds, rtd.TO_NO }).ToList();
            if (rDnData.Count!=1)
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180731133647", new string[] { dnNo, dnLine }));
            var rShipDetail = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>()
                .Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine).ToList();
            #endregion

            #region 加載界面信息
            MESStationSession TO_NOSession = new MESStationSession() { MESDataType = "TO_NO", InputValue = Input.Value.ToString(), SessionKey ="1", ResetInput = Input };
            MESStationSession DN_NOSession = new MESStationSession() { MESDataType = "DN_NO", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
            MESStationSession DN_ITEMSession = new MESStationSession() { MESDataType = "DN_ITEM", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
            MESStationSession SKU_NOSession = new MESStationSession() { MESDataType = "SKU_NO", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
            MESStationSession GT_QTYSession = new MESStationSession() { MESDataType = "GT_QTY", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
            MESStationSession REAL_QTYSession = new MESStationSession() { MESDataType = "REAL_QTY", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };

            Station.StationSession.Clear();
            Station.StationSession.Add(TO_NOSession);
            Station.StationSession.Add(DN_NOSession);
            Station.StationSession.Add(DN_ITEMSession);
            Station.StationSession.Add(SKU_NOSession);
            Station.StationSession.Add(GT_QTYSession);
            Station.StationSession.Add(REAL_QTYSession);

            TO_NOSession.Value = rDnData.FirstOrDefault().TO_NO;
            DN_NOSession.Value = rDnData.FirstOrDefault().rds.DN_NO;
            DN_ITEMSession.Value = rDnData.FirstOrDefault().rds.DN_LINE;
            SKU_NOSession.Value = rDnData.FirstOrDefault().rds.SKUNO;
            GT_QTYSession.Value = rDnData.FirstOrDefault().rds.QTY;
            REAL_QTYSession.Value = rShipDetail.Count;
            Station.NextInput = Station.Inputs.Find(t => t.DisplayName == "PACKNO");

            MESStationInput s = Station.Inputs.Find(t => t.DisplayName == "TO_LIST");
            s.Value = Station.SFCDB.ORM
                .Queryable<R_TO_HEAD, R_TO_DETAIL, R_DN_STATUS>((rth, rtd, rds) =>
                    rth.TO_NO == rtd.TO_NO && rtd.DN_NO == rds.DN_NO && rds.DN_FLAG == "0")
                .OrderBy((rth) => rth.TO_CREATETIME, SqlSugar.OrderByType.Desc)
                .GroupBy((rth,rtd,rds) => new { rth.TO_NO, rth.PLAN_STARTIME, rth.PLAN_ENDTIME, rth.TO_CREATETIME,rds.DN_NO })
                .Select((rth, rtd, rds) => new { rth.TO_NO, rth.PLAN_STARTIME, rth.PLAN_ENDTIME, rth.TO_CREATETIME, rds.DN_NO }).ToList();
            #endregion
        }

        public static void SetNextInputActionForShipFinish(MESPubLab.MESStation.MESStationBase Station,
            MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
                   dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString();
            var dnComplete = Station.SFCDB.ORM.Queryable<R_DN_STATUS>()
                .Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine && x.DN_FLAG == "1").Any();
            #region 加載界面信息
            if (dnComplete)
            {
                Station.StationSession.Clear();
                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == "TO_LIST");
                s.Value = Station.SFCDB.ORM
                    .Queryable<R_TO_HEAD, R_TO_DETAIL, R_DN_STATUS>((rth, rtd, rds) =>
                        rth.TO_NO == rtd.TO_NO && rtd.DN_NO == rds.DN_NO && rds.DN_FLAG == "0")
                    .OrderBy((rth) => rth.TO_CREATETIME, SqlSugar.OrderByType.Desc)
                    .GroupBy(rth => new {rth.TO_NO, rth.PLAN_STARTIME, rth.PLAN_ENDTIME, rth.TO_CREATETIME})
                    .Select(rth => new {rth.TO_NO, rth.PLAN_STARTIME, rth.PLAN_ENDTIME, rth.TO_CREATETIME}).ToList();
            }
            else
            {
                var rShipDetail = Station.SFCDB.ORM.Queryable<R_SHIP_DETAIL>()
                    .Where(x => x.DN_NO == dnNo && x.DN_LINE == dnLine).ToList();
                Station.StationSession.Find(x => x.MESDataType == "REAL_QTY").Value = rShipDetail.Count;
            }
            MESStationInput sp = Station.Inputs.Find(t => t.DisplayName == "PACKNO");
            sp.Value = "";
            #endregion
        }

    }
}
