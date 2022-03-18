using MESDataObject;
using MESDataObject.Module;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class SampleAction
    {
        /// <summary>
        /// Taken A SN To 2DX Sample
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void Taken2DXSample(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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
            string control_type = Paras[1].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(control_type))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            SN snObject = (SN)SNSession.Value;
            T_R_OUTLINE_TEST t_r_outline_test = new T_R_OUTLINE_TEST(Station.SFCDB, Station.DBType);
            R_OUTLINE_TEST testObj = new R_OUTLINE_TEST();
            T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);

            int result = 0;
            bool bSample = false;
            DateTime lastSampleTime;
            DateTime systemTime= Station.GetDBDateTime();            
            R_F_CONTROL controlObj = Station.SFCDB.ORM.Queryable<R_F_CONTROL>()
                .Where(r => r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.FUNCTIONNAME == control_type && r.VALUE == snObject.SkuNo && r.EXTVAL==Station.StationName)
                .ToList().FirstOrDefault();

            if (controlObj != null)
            {
                R_F_CONTROL_EX controlTime = Station.SFCDB.ORM.Queryable<R_F_CONTROL_EX>()
                    .Where(r => r.DETAIL_ID == controlObj.ID).OrderBy(r => r.SEQ_NO, SqlSugar.OrderByType.Asc).ToList().FirstOrDefault();

                if (controlTime == null)
                {
                    throw new MESReturnMessage("Sample Time Error!");
                }
                double spanTime = 0;
                if (!double.TryParse(controlTime.VALUE, out spanTime))
                {
                    throw new MESReturnMessage("Sample Time Error!");
                }

                List<R_OUTLINE_TEST> testList = Station.SFCDB.ORM.Queryable<R_OUTLINE_TEST>()
                    .Where(r => r.DATA_TYPE == control_type && r.WORKORDERNO == snObject.WorkorderNo && r.SKUNO == snObject.SkuNo && r.STATION == Station.StationName && r.VALID_FLAG == 1)
                    .OrderBy(r => r.CREAT_DATE, SqlSugar.OrderByType.Desc).ToList();
                if (testList.Count == 0)
                {
                    bSample = true;
                }
                else if (testList.Find(r => r.SN == snObject.SerialNo) == null)
                {
                    if (testList.FirstOrDefault().CREAT_DATE == null)
                    {
                        throw new MESReturnMessage("Last SN Sample Time Error!");
                    }
                    lastSampleTime = (DateTime)testList.FirstOrDefault().CREAT_DATE;
                    switch (controlObj.CATEGORY.ToUpper())
                    {
                        case "HOUR":
                            lastSampleTime = lastSampleTime.AddHours(spanTime);
                            break;
                        case "MINUTE":
                            lastSampleTime = lastSampleTime.AddMinutes(spanTime);
                            break;
                    }

                    if (DateTime.Compare(systemTime, lastSampleTime) >= 0)
                    {
                        bSample = true;
                    }
                }
            }
            if (bSample)
            {
                testObj.ID = t_r_outline_test.GetNewID(Station.BU, Station.SFCDB);
                testObj.SN = snObject.SerialNo;
                testObj.WORKORDERNO = snObject.WorkorderNo;
                testObj.SKUNO = snObject.SkuNo;
                testObj.STATION = Station.StationName;
                testObj.LINE = Station.Line;
                testObj.STATUS = "0";//0--WAIT_TEST,1--PASS,2--Fail
                testObj.VALID_FLAG = 1;
                testObj.CREAT_DATE = systemTime;
                testObj.CREAT_EMP = "SYSTEM";
                testObj.LASTEDIT_DATE = systemTime;
                testObj.LASTEDIT_BY = Station.LoginUser.EMP_NO;
                testObj.DATA_TYPE = control_type;

                MESDBHelper.OleExec saveDB= Station.DBS["SFCDB"].Borrow();
                saveDB.BeginTrain();
                try
                {                    
                    result = t_r_outline_test.Save(saveDB, testObj);
                    if (result == 0)
                    {
                        throw new Exception("Save Data Error!");
                    }
                    result = t_r_sn_lock.AddNewLock(Station.BU, "", "SN", snObject.SerialNo, snObject.WorkorderNo, Station.StationName, control_type, "SYSTEM", saveDB);
                    if (result == 0)
                    {
                        throw new Exception($@"{control_type} Lock {snObject.SerialNo} Error!");
                    }
                    saveDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    saveDB.RollbackTrain();
                    throw new MESReturnMessage(ex.Message);
                } 
                finally
                {
                    if (saveDB != null)
                    {
                        Station.DBS["SFCDB"].Return(saveDB);
                    }
                }                
                throw new MESReturnMessage($@"{control_type},Taken To Sample,Please Take To Test");
                //Station.StationMessages.Add(
                //    new MESPubLab.MESStation.MESReturnView.Station.StationMessage {
                //        Message=$@"{control_type},Taken To Sample,Please Take To Test",
                //        State=MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass
                //    });
            }
        }

        /// <summary>
        /// Taken A SN To 5DX Sample
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void Taken5DXSample(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession Snsession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Snsession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string SampleStation = Paras[1].VALUE.ToString().Trim();
            if (string.IsNullOrEmpty(SampleStation))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            SN snObject = new SN(Snsession.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL t_r_lot_detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS StatusObj = (Row_R_LOT_STATUS)t_r_lot_status.NewRow();
            Row_R_LOT_DETAIL DetailObj = (Row_R_LOT_DETAIL)t_r_lot_detail.NewRow();
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);

            bool bSample = false;
            DateTime DateTime1 = DateTime.Parse(DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd 8:00:00"));
            DateTime DateTime2 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 20:00:00"));
            DateTime DateTime3 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DateTime DateTime4 = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 8:00:00"));
            DateTime DateTime5 = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 20:00:00"));
            DateTime DateTime6 = DateTime.Parse(DateTime.Now.AddDays(+1).ToString("yyyy-MM-dd 8:00:00"));

            //MESDBHelper.OleExec saveDB = Station.DBS["SFCDB"].Borrow();
            R_F_CONTROL controlObj = Station.SFCDB.ORM.Queryable<R_F_CONTROL>()
                .Where(r => r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.FUNCTIONNAME == SampleStation && r.VALUE == snObject.SkuNo && r.EXTVAL == Station.StationName)
                .ToList().FirstOrDefault();
            int SnStationRecord = 0;
            List<C_ROUTE_DETAIL> cRouteDetailList = t_c_route_detail.GetStationByNameBefor(Station.SFCDB, snObject.RouteID, Station.StationName);
            foreach (var item in cRouteDetailList)
            {
                if (Station.StationName == "SMT1" && item.STATION_NAME != "SMT2")
                {
                    continue;
                }
                if (Station.StationName == "SMT2" && item.STATION_NAME != "SMT1")
                {
                    continue;
                }
                // only one SMT1 or SMT2 station TO sample
                //var SMTcount = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == snObject.RouteID && r.STATION_NAME.StartsWith("SMT") && r.STATION_NAME.Length==4).Count();
                //if (SMTcount == 2)
                //{
                //    continue;
                //}
                //The number of the same SKU produced without changing the line during the day shift
                if (DateTime3 > DateTime1 && DateTime3 < DateTime2)
                {
                    SnStationRecord = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                    .Where(r => r.EDIT_TIME > DateTime1 && r.EDIT_TIME < DateTime2 && r.SKUNO == snObject.SkuNo && r.LINE == Station.Line && r.STATION_NAME == Station.StationName)
                    .Count();
                }
                // The number of the same SKU produced by the body without changing the line in the middle of the night on the night shift
                else if (DateTime3 > DateTime2 && DateTime3 < DateTime6)
                {
                    SnStationRecord = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                       .Where(r => r.EDIT_TIME > DateTime2 && r.EDIT_TIME < DateTime6 && r.SKUNO == snObject.SkuNo && r.LINE == Station.Line && r.STATION_NAME == Station.StationName)
                       .Count();
                }
                //The number of the same SKU produced by the body without changing the line in the middle of the night
                else if (DateTime3 > DateTime5 && DateTime3 < DateTime1)
                {
                    SnStationRecord = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                       .Where(r => r.EDIT_TIME > DateTime5 && r.EDIT_TIME < DateTime1 && r.SKUNO == snObject.SkuNo && r.LINE == Station.Line && r.STATION_NAME == Station.StationName)
                       .Count();
                }

                var LotStatus = Station.SFCDB.ORM.Queryable<R_LOT_STATUS>()
                               .Where(r => r.SKUNO == snObject.SkuNo && r.SAMPLE_STATION == SampleStation).First();

                var SnIsOrNotSamlple = Station.SFCDB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>((rls, rld) => rls.LOT_NO == rld.LOT_ID)
                    .Where((rls, rld) => rls.SKUNO == snObject.SkuNo && rls.SAMPLE_STATION == SampleStation && rld.SN == snObject.SerialNo).Select((rls,rld)=>rld).Count();
                //The duplicate sn does not need a sampling 
                if (SnIsOrNotSamlple == 0)
                {
                    //100 % random test of the first 3 films
                    if (SnStationRecord < 4)
                    {
                        if (LotStatus == null)
                        {
                            StatusObj.ID = t_r_lot_status.GetNewID(Station.BU, Station.SFCDB);
                            StatusObj.LOT_NO = "LOT-" + DateTime.Now.ToString("yyyyMMddFF");
                            StatusObj.SKUNO = snObject.SkuNo;
                            StatusObj.LOT_QTY = 0;
                            StatusObj.REJECT_QTY = 0;
                            StatusObj.SAMPLE_QTY = 1;
                            StatusObj.PASS_QTY = 1;
                            StatusObj.FAIL_QTY = 0;
                            StatusObj.CLOSED_FLAG = "2";
                            StatusObj.LOT_STATUS_FLAG = "0";
                            StatusObj.SAMPLE_STATION = SampleStation;
                            StatusObj.LINE = Station.Line;
                            StatusObj.EDIT_TIME = DateTime.Now;
                            StatusObj.EDIT_EMP = "SYSTEM";
                            Station.SFCDB.ExecSQL(StatusObj.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                            Station.SFCDB.CommitTrain();

                            DetailObj.ID = t_r_lot_detail.GetNewID(Station.BU, Station.SFCDB);
                            DetailObj.LOT_ID = StatusObj.LOT_NO;
                            DetailObj.SN = snObject.SerialNo;
                            DetailObj.WORKORDERNO = snObject.WorkorderNo;
                            DetailObj.CREATE_DATE = DateTime.Now;
                            DetailObj.STATUS = "0";//0 Wait Test ,1 Test pass
                            DetailObj.EDIT_EMP = "SYSTEM";
                            DetailObj.EDIT_TIME = DateTime.Now;
                            Station.SFCDB.ExecSQL(DetailObj.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                            Station.SFCDB.CommitTrain();
                            throw new MESReturnMessage($@"{SampleStation},Taken To Sample,Please Take To Test");
                        }
                        else
                        {
                            Station.SFCDB.ORM.Updateable<R_LOT_STATUS>().SetColumns(r => new R_LOT_STATUS
                            {
                                SAMPLE_QTY = LotStatus.SAMPLE_QTY + 1,
                                PASS_QTY = LotStatus.PASS_QTY + 1,
                                EDIT_EMP = Station.LoginUser.EMP_NO,
                                EDIT_TIME = DateTime.Now
                            }).Where(r => r.SKUNO == snObject.SkuNo && r.SAMPLE_STATION == "5DX").ExecuteCommand();

                            DetailObj.ID = t_r_lot_detail.GetNewID(Station.BU, Station.SFCDB);
                            DetailObj.LOT_ID = LotStatus.LOT_NO;
                            DetailObj.SN = snObject.SerialNo;
                            DetailObj.WORKORDERNO = snObject.WorkorderNo;
                            DetailObj.CREATE_DATE = DateTime.Now;
                            DetailObj.STATUS = "0";//0 Wait Test ,1 Test pass
                            DetailObj.EDIT_EMP = "SYSTEM";
                            DetailObj.EDIT_TIME = DateTime.Now;
                            Station.SFCDB.ExecSQL(DetailObj.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                            Station.SFCDB.CommitTrain();
                            throw new MESReturnMessage($@"{SampleStation},Taken To Sample,Please Take To Test");
                        }
                    }
                    //Otherwise, take a sample in two hours
                    else
                    {
                        #region Prevent the transit to be wrong 
                        var LastSampleSn = Station.SFCDB.ORM.Queryable<R_LOT_STATUS>().Where(r => r.SKUNO == snObject.SkuNo && r.LINE == Station.Line && r.SAMPLE_STATION == SampleStation).OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc).First();
                        if (LastSampleSn == null)
                        {
                            StatusObj.ID = t_r_lot_status.GetNewID(Station.BU, Station.SFCDB);
                            StatusObj.LOT_NO = "LOT-" + DateTime.Now.ToString("yyyyMMddFF");
                            StatusObj.SKUNO = snObject.SkuNo;
                            StatusObj.LOT_QTY = 0;
                            StatusObj.REJECT_QTY = 0;
                            StatusObj.SAMPLE_QTY = 1;
                            StatusObj.PASS_QTY = 1;
                            StatusObj.FAIL_QTY = 0;
                            StatusObj.CLOSED_FLAG = "2";
                            StatusObj.LOT_STATUS_FLAG = "0";
                            StatusObj.SAMPLE_STATION = SampleStation;
                            StatusObj.LINE = Station.Line;
                            StatusObj.EDIT_TIME = DateTime.Now;
                            StatusObj.EDIT_EMP = "SYSTEM";
                            Station.SFCDB.ExecSQL(StatusObj.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                            Station.SFCDB.CommitTrain();

                            DetailObj.ID = t_r_lot_detail.GetNewID(Station.BU, Station.SFCDB);
                            DetailObj.LOT_ID = StatusObj.LOT_NO;
                            DetailObj.SN = snObject.SerialNo;
                            DetailObj.WORKORDERNO = snObject.WorkorderNo;
                            DetailObj.CREATE_DATE = DateTime.Now;
                            DetailObj.STATUS = "0";//0 Wait Test ,1 Test pass
                            DetailObj.EDIT_EMP = "SYSTEM";
                            DetailObj.EDIT_TIME = DateTime.Now;
                            Station.SFCDB.ExecSQL(DetailObj.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                            Station.SFCDB.CommitTrain();
                            throw new MESReturnMessage($@"{SampleStation},Taken To Sample,Please Take To Test");
                        }
                        #endregion
                        else
                        {
                            DateTime lastdate = (DateTime)LastSampleSn.EDIT_TIME;
                            int Time = (int)(DateTime3 - lastdate).TotalHours;
                            // Draw 1 every 2 hours
                            if (Time == 2)
                            {
                                Station.SFCDB.ORM.Updateable<R_LOT_STATUS>().SetColumns(r => new R_LOT_STATUS
                                {
                                    SAMPLE_QTY = LotStatus.SAMPLE_QTY + 1,
                                    PASS_QTY = LotStatus.PASS_QTY + 1,
                                    EDIT_EMP = "SYSTEM",
                                    EDIT_TIME = DateTime.Now
                                }).Where(r => r.SKUNO == snObject.SkuNo && r.SAMPLE_STATION == "5DX").ExecuteCommand();

                                DetailObj.ID = t_r_lot_detail.GetNewID(Station.BU, Station.SFCDB);
                                DetailObj.LOT_ID = LotStatus.LOT_NO;
                                DetailObj.SN = snObject.SerialNo;
                                DetailObj.WORKORDERNO = snObject.WorkorderNo;
                                DetailObj.CREATE_DATE = DateTime.Now;
                                DetailObj.STATUS = "0";//0 Wait Test ,1 Test pass
                                DetailObj.EDIT_EMP = "SYSTEM";
                                DetailObj.EDIT_TIME = DateTime.Now;
                                Station.SFCDB.ExecSQL(DetailObj.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                                Station.SFCDB.CommitTrain();
                                throw new MESReturnMessage($@"{SampleStation},Taken To Sample,Please Take To Test");
                            }
                        }
                    }
                }
            }
        }
    }
}
