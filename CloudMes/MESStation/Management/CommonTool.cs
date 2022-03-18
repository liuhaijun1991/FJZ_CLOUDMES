using MESDBHelper;
using MESPubLab.MESStation;
using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using System.Data;
using MESPubLab.MESStation.MESReturnView.Public;
using MESPubLab.MESStation.LogicObject;
using MESDataObject.Common;
using SqlSugar;
using MESPubLab.MESStation.SNMaker;
using MESDataObject.Module.BPD;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;


namespace MESStation.Management
{
    class CommonTool : MesAPIBase
    {

        /// <summary>
        /// 從Sn加載 stationName List
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public void GetStationBySnDataloader_Tool(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            try
            {
                string SN = Data["SN"] == null ? "" : Data["SN"].ToString();
                List<string> stationList = new List<string>();
                var SNObj = sfcdb.ORM.Queryable<R_SN>().Where(r => r.SN == SN && r.VALID_FLAG == "1").First();
                if (SNObj == null)
                {
                    //StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { SNObj.WORKORDERNO });
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { SNObj.WORKORDERNO }));
                }
                T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(sfcdb, DBTYPE);
                List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.ORAGetPreviousByRouteId(SNObj.ROUTE_ID, SNObj.NEXT_STATION, sfcdb);
                stationList.Clear();
                if (RouteDetails != null)
                { 
                    foreach (C_ROUTE_DETAIL c in RouteDetails)
                    {
                        //增加可以退站SILOADING
                        if (!c.STATION_NAME.Contains("BIP") && !c.STATION_NAME.Contains("SMTLOADING"))
                        {
                            stationList.Add(c.STATION_NAME.ToString());
                        }
                        stationList.Sort();
                    }
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Data = stationList;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    //StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MES00000194", new string[] { SNObj.WORKORDERNO });
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000194", new string[] { SNObj.WORKORDERNO }));
                }
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        /// <summary>
        /// 檢查SN 狀態
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public void SnStatusFlagChecker_Tool(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            string ErrMesg = string.Empty;
            string statusFlag = string.Empty;
            string SN = Data["SN"] == null ? "" : Data["SN"].ToString();
            //try
            //{

                var SNObj = sfcdb.ORM.Queryable<R_SN>().Where(r => r.SN == SN && r.VALID_FLAG == "1").First();

                //已經包裝
                if (SNObj.PACKED_FLAG != null && SNObj.PACKED_FLAG.Equals("1"))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005155848", new string[] { SNObj.SN });
                    return;
                }
                //已經完工
                if (SNObj.COMPLETED_FLAG != null && SNObj.COMPLETED_FLAG.Equals("1"))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160344", new string[] { SNObj.SN });
                    return;
                }
                //已經出貨
                if (SNObj.SHIPPED_FLAG != null && SNObj.SHIPPED_FLAG.Equals("1"))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160504", new string[] { SNObj.SN });
                    return;
                }
                //待維修
                if (SNObj.REPAIR_FAILED_FLAG != null && SNObj.REPAIR_FAILED_FLAG.Equals("1"))
                {
                    //StationReturn.Status = StationReturnStatusValue.Fail;
                    //StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160551", new string[] { SNObj.SN });
                    //return;
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160551", new string[] { SNObj.SN }));
                }

                //已報廢
                if (SNObj.SCRAPED_FLAG != null && SNObj.SCRAPED_FLAG.Equals("1"))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005160653", new string[] { SNObj.SN });
                    return;
                }
                //無效的
                if (SNObj.VALID_FLAG != null && SNObj.VALID_FLAG.Equals("0"))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005154143", new string[] { SNObj.SN });
                    return;
                }
                //已入庫
                if (SNObj.STOCK_STATUS != null && SNObj.STOCK_STATUS.Equals("1"))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181005161120", new string[] { SNObj.SN });
                    return;
                }
            //}
            //catch (Exception exception)
            //{
            //    StationReturn.Status = StationReturnStatusValue.Fail;
            //    StationReturn.MessageCode = "MES00000001";
            //    StationReturn.MessagePara.Add(exception.Message);
            //    StationReturn.Data = exception.Message;
            //    if (sfcdb != null)
            //    {
            //        DBPools["SFCDB"].Return(sfcdb);
            //    }
            //}

        }

        /// <summary>
        /// 使用Tool工具SN退站更新記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public void ReturnSNAction_Tool(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {

            string SN = Data["SN"] == null ? "" : Data["SN"].ToString();
            string station_name = Data["STATION"] == null ? "" : Data["STATION"].ToString();

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            string ErrMesg = string.Empty;
            string statusFlag = string.Empty;
            Double Station_SEQ = 0.0;
            Double SN_SEQ = 0.0;
            try
            {
                var SNObj = sfcdb.ORM.Queryable<R_SN>().Where(r => r.SN == SN && r.VALID_FLAG == "1").First();
                if (SNObj.SHIPPED_FLAG == "1")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20180808103413", new string[] { SNObj.SN });
                }
                if (SNObj.REPAIR_FAILED_FLAG == "1")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104007", new string[] { SNObj.SN });
                }
                if (SNObj.COMPLETED_FLAG == "1")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104922", new string[] { SNObj.SN });
                }
                if (SNObj.CURRENT_STATION == "REWORK")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181128201930", new string[] { SNObj.SN });
                }
                string DeviceName = string.Empty;
                T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(sfcdb, DBTYPE);
                List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(SNObj.ROUTE_ID, sfcdb);

                if (RouteDetails.Count > 0)
                {
                    foreach (C_ROUTE_DETAIL c in RouteDetails)
                    {
                        if (station_name == c.STATION_NAME)
                        {
                            Station_SEQ = (Double)c.SEQ_NO;
                        }
                        if (SNObj.CURRENT_STATION == c.STATION_NAME)
                        {
                            SN_SEQ = (Double)c.SEQ_NO;
                        }
                    }
                    if (Station_SEQ > SN_SEQ)
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20180808112641", new string[] { SNObj.SN });
                    }
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20180808113358", new string[] { SNObj.SN });
                }

                var stationList = RouteDetails.FindAll(t => t.SEQ_NO >= Station_SEQ && t.SEQ_NO <= SN_SEQ).Select(t => t.STATION_NAME).ToArray();

                if (stationList.Contains("BIP"))
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = MESReturnMessage.GetMESReturnMessage("MSGCODE20181025091108", new string[] { SNObj.SN });
                }

                var seqList = RouteDetails.FindAll(t => t.SEQ_NO < Station_SEQ).Select(t => t.SEQ_NO).ToArray();
                if (seqList.Length == 0)
                {
                    DeviceName = RouteDetails.OrderBy(r => r.SEQ_NO).FirstOrDefault().STATION_NAME;
                }
                else
                {
                    Array.Sort(seqList);
                    SN_SEQ = (double)seqList[seqList.Length - 1];
                    var tt = RouteDetails.Find(t => t.SEQ_NO == SN_SEQ);
                    DeviceName = tt.STATION_NAME;
                }
                T_R_SN table = new T_R_SN(sfcdb, DBTYPE);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
