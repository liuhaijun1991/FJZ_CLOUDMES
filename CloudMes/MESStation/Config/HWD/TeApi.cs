using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Common;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using SqlSugar;
using MESDataObject.Module.Juniper;
using MESDataObject;

namespace MESStation.Config.HWD
{
    public class TeApi : MesAPIBase
    {
        //GetNextStation

        protected APIInfo FGetNextStation = new APIInfo()
        {
            FunctionName = "GetNextStation",
            Description = "Get the Silver Wip Unit Data ",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>() { }
        };
        //GetRepairStatus
        protected APIInfo FGetRepairStatus = new APIInfo()
        {
            FunctionName = "GetRepairStatus",
            Description = "Get SN Repair Status ",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetWoInfoBySn01A = new APIInfo()
        {
            FunctionName = "GetWoInfoBySn01A",
            Description = "通過SN獲取工單號，工單上線時間，生產工單天數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FLockUnitSN = new APIInfo()
        {
            FunctionName = "LockUnitSN",
            Description = "Locks SN in Lock Manager",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "JsonData", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetLockStatus = new APIInfo()
        {
            FunctionName = "GetLockStatus",
            Description = "通過SN獲取工單號，工單上線時間，生產工單天數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetLastTestStatus = new APIInfo()
        {
            FunctionName = "GetTestStatus",
            Description = "通過SN獲取工單號，工單上線時間，生產工單天數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }/*,
                new APIInputInfo() {InputName = "WO", InputType = "string", DefaultValue = "" }*/
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetJuniper_Silver_Wip = new APIInfo()
        {
            FunctionName = "GetJuniper_Silver_Wip",
            Description = "Get the Silver Wip Unit Data ",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" }

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetJuniper_Error_Codes = new APIInfo()
        {
            FunctionName = "GetJuniper_Error_Codes",
            Description = "Get error code catalog",
            Permissions = new List<MESPermission>() { }
        };


        protected APIInfo FUpdate_Test_Hours_Juniper_Silver_Wip = new APIInfo()
        {
            FunctionName = "Update_Test_Hours_Juniper_Silver_Wip",
            Description = "Update Test_Hours for Juniper_Silver_Sip units",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "JsonData", InputType = "string", DefaultValue = "" },

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetAllPartsLocations = new APIInfo()
        {
            FunctionName = "GetAllPartsLocations",
            Description = "GetAllPartsLocations",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "string", DefaultValue = "" },

            },
            Permissions = new List<MESPermission>() { }
        };

        public TeApi()
        {
            this.Apis.Add(FGetWoInfoBySn01A.FunctionName, FGetWoInfoBySn01A);
            this.Apis.Add(FGetLastTestStatus.FunctionName, FGetLastTestStatus);
            this.Apis.Add(FGetLockStatus.FunctionName, FGetLockStatus);
            this.Apis.Add(FGetJuniper_Silver_Wip.FunctionName, FGetJuniper_Silver_Wip);
            this.Apis.Add(FUpdate_Test_Hours_Juniper_Silver_Wip.FunctionName, FUpdate_Test_Hours_Juniper_Silver_Wip);
            this.Apis.Add(FGetJuniper_Error_Codes.FunctionName, FGetJuniper_Error_Codes);
            this.Apis.Add(FGetAllPartsLocations.FunctionName, FGetAllPartsLocations);
            this.Apis.Add(FGetNextStation.FunctionName, FGetNextStation);
            this.Apis.Add(FLockUnitSN.FunctionName, FLockUnitSN);
            this.Apis.Add(FGetRepairStatus.FunctionName, FGetRepairStatus);
        }

        public void GetWoInfoBySn01A(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string SN = Data["SN"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_SN, R_WO_BASE>((rs, rwb) =>
                        rs.WORKORDERNO == rwb.WORKORDERNO && rs.SN == SN && rs.VALID_FLAG ==
                        ENUM_R_SN.VALID_FLAG_TRUE.Ext<EnumExtensions.EnumValueAttribute>().Description)
                    .Select((rs, rwb) => new { rs.SN, rs.WORKORDERNO, rs.START_TIME }).ToList().FirstOrDefault();
                if (res == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(1);
                    StationReturn.Data = res;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

        }

        public void GetLastTestStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string SN = Data["SN"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_TEST_JUNIPER>()
                    .Where(rt => rt.SYSSERIALNO == SN)
                    .Select(rt => new { rt.SYSSERIALNO, rt.EVENTNAME, rt.STATUS, rt.TATIME })
                    .OrderBy(rt => rt.TATIME, OrderByType.Desc).ToList();

                //var res = oleDB.ORM.Ado.GetDataTable($@"SELECT *
                //    FROM SFCRUNTIME.R_TEST_JUNIPER
                //    WHERE SYSSERIALNO = '{SN}'
                //    ORDER BY TATIME DESC");

                if (res == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(1);
                    StationReturn.Data = res;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetRepairStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string SN = Data["SN"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_REPAIR_MAIN>()
                    .Where(r => r.SN == SN && r.CLOSED_FLAG == "0").Any();

                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MSGRA00000002";
                    StationReturn.MessagePara.Add(1);
                    StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void GetLockStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string SN = Data["SN"].ToString().Trim().ToUpper();
                oleDB = this.DBPools["SFCDB"].Borrow();

                var SNInfo = oleDB.ORM.Ado.GetDataTable($@"SELECT workorderno, skuno
                from sfcruntime.r_SN
                WHERE SN = '{SN}'
                AND VALID_FLAG = '1'");

                if (SNInfo.Rows.Count > 0)
                {
                    for (int i = 0; i < SNInfo.Rows.Count; i++)
                    {
                        var workoderno = SNInfo.Rows[i][0].ToString();
                        var skuno = SNInfo.Rows[i][1].ToString();
                        string testType = null;

                        if (skuno.StartsWith("711-"))
                        {
                            testType = "STRUCTURAL";
                        }
                        else
                        {
                            testType = "FUNCTIONAL";
                        }

                        var res = oleDB.ORM.Ado.GetDataTable($@"SELECT SN, LOCK_REASON
                            FROM SFCRUNTIME.R_SN_LOCK
                            WHERE SN = '{SN}'
                            AND LOCK_STATUS = '1'
                            AND (LOCK_STATION = 'ALL' 
                                            OR LOCK_STATION IN (SELECT DISTINCT MES_STATION FROM SFCBASE.C_TEMES_STATION_MAPPING WHERE TEGROUP = '{testType}'))");
                        
                        if (res == null)
                        {
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MES00000034";
                            StationReturn.Data = new object();
                        }
                        else
                        {
                            if (res.Rows.Count == 0)
                            {
                                res = oleDB.ORM.Ado.GetDataTable($@"SELECT WORKORDERNO, LOCK_REASON
                                FROM SFCRUNTIME.R_SN_LOCK
                                WHERE WORKORDERNO = '{workoderno}'
                                AND LOCK_STATUS = '1'
                                AND (LOCK_STATION = 'ALL' 
                                            OR LOCK_STATION IN (SELECT DISTINCT MES_STATION FROM SFCBASE.C_TEMES_STATION_MAPPING WHERE TEGROUP = '{testType}'))");

                                if (res.Rows.Count == 0)
                                {
                                    res = oleDB.ORM.Ado.GetDataTable($@"SELECT WORKORDERNO, LOCK_REASON
                                    FROM SFCRUNTIME.R_SN_LOCK
                                    WHERE workorderno = '{skuno}'
                                    AND LOCK_STATUS = '1'
                                    AND (LOCK_STATION = 'ALL' 
                                            OR LOCK_STATION IN (SELECT DISTINCT MES_STATION FROM SFCBASE.C_TEMES_STATION_MAPPING WHERE TEGROUP = '{testType}'))");
                                    if (res.Rows.Count == 0)
                                    {
                                        StationReturn.Status = StationReturnStatusValue.Fail;
                                        StationReturn.MessageCode = "MES00000034";
                                        StationReturn.Data = new object();
                                    }
                                    else
                                    {
                                        StationReturn.Status = StationReturnStatusValue.Pass;
                                        StationReturn.MessageCode = "MES00000033";
                                        StationReturn.MessagePara.Add(1);
                                        StationReturn.Data = res;
                                        return;
                                    }

                                }
                                else
                                {
                                    StationReturn.Status = StationReturnStatusValue.Pass;
                                    StationReturn.MessageCode = "MES00000033";
                                    StationReturn.MessagePara.Add(1);
                                    StationReturn.Data = res;
                                    return;
                                }
                            }
                            else
                            {
                                StationReturn.Status = StationReturnStatusValue.Pass;
                                StationReturn.MessageCode = "MES00000033";
                                StationReturn.MessagePara.Add(1);
                                StationReturn.Data = res;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    var res = oleDB.ORM.Ado.GetDataTable($@"SELECT SN, LOCK_REASON
                            FROM SFCRUNTIME.R_SN_LOCK
                            WHERE SN = '{SN}'
                            AND LOCK_STATUS = '1'
                            AND (LOCK_STATION = 'ALL' 
                                            OR LOCK_STATION IN (SELECT DISTINCT MES_STATION FROM SFCBASE.C_TEMES_STATION_MAPPING))");

                    if (res.Rows.Count > 0)
                    {

                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000033";
                        StationReturn.MessagePara.Add(1);
                        StationReturn.Data = res;
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000034";
                        StationReturn.Data = new object();
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void GetJuniper_Silver_Wip(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string ErrMessage;

            try
            {
                string _SN = Data["SN"].ToString().Trim();

                oleDB = this.DBPools["SFCDB"].Borrow();
                /*
                var res = oleDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>()
                    .Where(jsw => jsw.SN == SN && jsw.STATE_FLAG == "1")
                    .OrderBy(jsw => jsw.EDIT_TIME, OrderByType.Desc).First();
                */
                var query1 = oleDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>().ToList()
                    .Join(oleDB.ORM.Queryable<C_SKU_DETAIL>().Where(b => b.CATEGORY == "JUNIPER" && b.CATEGORY_NAME == "SilverWip").ToList(),
                    a => a.SKUNO,
                    b => b.SKUNO,
                    (a, b) => new
                    {
                        SN = a.SN,
                        SKUNO = a.SKUNO,
                        TEST_HOURS = a.TEST_HOURS.ToString(),
                        MAX_TEST_HOURS = b.BASETEMPLATE,
                        WIP_DAYS = (Math.Round((Convert.ToDateTime(oleDB.ORM.GetDate()).Subtract(Convert.ToDateTime(a.START_TIME)).TotalDays), 2)).ToString(),
                        MAX_WIP_DAYS = b.EXTEND,
                        IN_WIP_UNITS = oleDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(c => c.STATE_FLAG == "1" && c.SKUNO == a.SKUNO).Count().ToString(),
                        MAX_IN_WIP_UNITS = b.VALUE,
                        SW_CHECKIN_TIME = a.START_TIME,
                        SW_CHECKIN_BY = a.IN_WIP_USER,
                        SW_CHECKOUT_TIME = a.END_TIME,
                        SW_CHECKOUT_BY = a.OUT_WIP_USER,
                        STATUS = a.STATE_FLAG == "1" ? "CHECKIN" : a.STATE_FLAG == "0" ? "CHECKOUT" : "NA",
                        EDIT_EMP = a.EDIT_EMP,
                        EDIT_TIME = a.EDIT_TIME
                    })
                    .Where(a => a.SN == _SN)
                    .ToList().FirstOrDefault();

                if (query1 == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210803125239", new string[] { _SN });
                    throw new MESReturnMessage(ErrMessage);
                }


                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = query1;

            }
            catch (Exception Ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = Ex.Message;
                StationReturn.Data = new object();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

            //------------------------------------

        }

        public void GetJuniper_Error_Codes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string ErrMessage;

            try
            {
                string SN = Data["SN"].ToString().Trim();

                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<C_ERROR_CODE>()
                    .Select(err => new { err.ERROR_CODE, err.ENGLISH_DESC })
                    .OrderBy(err => err.ERROR_CODE, OrderByType.Desc).ToList();


                if (res == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210803125239", new string[] { SN });
                    throw new MESReturnMessage(ErrMessage);
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = res;

            }
            catch (Exception Ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = Ex.Message;
                StationReturn.Data = new object();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }


        public void GetAllPartsLocations(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string ErrMessage;
            try
            {
                string SKUNO = Data["SKUNO"].ToString().Trim();
                oleDB = this.DBPools["APDB"].Borrow();
                var res = oleDB.ORM.Ado.GetDataTable($@"SELECT DISTINCT E.LOCATION, E.KP_NO 
                                                        FROM MES1.C_SMT_AP_LOCATION E WHERE SMT_CODE IN (SELECT SMT_CODE
                                                        FROM MES4.R_TRAVEL_SN
                                                        WHERE P_NO = '{SKUNO}')");
                if (res.Rows.Count == 0)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODEAPNOTEX01", new string[] { SKUNO });
                    throw new MESReturnMessage(ErrMessage);
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = res;
            }
            catch (Exception Ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = Ex.Message;
                StationReturn.Data = new object();
            }
            finally
            {
                this.DBPools["APDB"].Return(oleDB);
            }
        }

        public void GetNextStation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            string ErrMessage;
            try
            {
                string SN = Data["SN"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Ado.GetDataTable($@"SELECT NEXT_STATION
                                                        FROM SFCRUNTIME.R_SN_STATION_DETAIL
                                                        WHERE SN = '{SN}'
                                                        ORDER BY EDIT_TIME DESC
                                                        FETCH FIRST 1 ROW ONLY");
                if (res.Rows.Count == 0)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODENEXTSTATION01", new string[] { SN });
                    throw new MESReturnMessage(ErrMessage);
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000033";
                StationReturn.Data = res;
            }
            catch (Exception Ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = Ex.Message;
                StationReturn.Data = new object();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }

        public void Update_Test_Hours_Juniper_Silver_Wip(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            bool hasPass = false;
            bool hasFail = false;
            string ErrMessage;

            try
            {
                var JsonData = Data["JsonData"];
                var swObj = Newtonsoft.Json.JsonConvert.DeserializeObject<R_JUNIPER_SILVER_WIP>(JsonData.ToString());

                //var swR = SFCDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(sw => sw.SN == swObj.SN && sw.SKUNO == swObj.SKUNO && sw.STATE_FLAG == "1").ToList().FirstOrDefault();
                //Remove the sw.SKUNO == swObj.SKUNO in SW log cannot obtain the SKU:750-******* only the BasePID model MX260 MX920
                var swR = SFCDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(sw => sw.SN == swObj.SN && sw.STATE_FLAG == "1").ToList().FirstOrDefault();

                if (swR == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210803125239", new string[] { swObj.SN });
                    throw new MESReturnMessage(ErrMessage);
                }

                if (swObj.TEST_HOURS <= swR.TEST_HOURS)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210803130433", new string[] { swObj.SN, swObj.TEST_HOURS.ToString(), swR.TEST_HOURS.ToString() });
                    throw new MESReturnMessage(ErrMessage);
                }

                //DateTime sysdate = SFCDB.ORM.GetDate();
                DateTime sysdate = MesDbBase.GetOraDbTime(SFCDB.ORM);

                //Disable record set 0, Enable record set 1
                //swR.STATE_FLAG = "1";
                swR.EDIT_TIME = sysdate;
                swR.EDIT_EMP = "MESTEAPI";
                swR.TEST_HOURS = swObj.TEST_HOURS;

                var str = SFCDB.ORM.Updateable<R_JUNIPER_SILVER_WIP>(swR).Where(t => t.ID == swR.ID).ExecuteCommand();
                if (str == 1)
                {
                    ret.Add($@"Type: 'Update ");
                    ret.Add($@"OK:ID='{swR.ID}'");
                    ret.Add($@"SN: '{swR.SN}'");
                    ret.Add($@"Result: 'Successfully'");

                    hasPass = true;
                }
                else
                {
                    ret.Add($@"Type: 'Update'");
                    ret.Add($@"Err:ID='{swR.ID}'");
                    ret.Add($@"SN:'{swR.SN}'");
                    ret.Add($@"Result: 'Error: Cant Update STATE_FLAG in R_JUNIPER_SILVER_WIP'");
                    hasFail = true;
                }

                var config = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY_NAME == "SilverWip" && t.CATEGORY == "JUNIPER" && t.SKUNO == swR.SKUNO).First();
                try
                {
                    if (!SFCDB.ORM.Queryable<R_SN_LOCK>().WhereIF(config != null, lk => lk.SN == swR.SN && lk.LOCK_STATUS == "1" && lk.LOCK_STATION == "SILOADING"
                   && (lk.LOCK_REASON.Contains("MAX_TEST_HOURS TimeOut") || lk.LOCK_REASON.Contains("MAX_WIP_DAYS TimeOut"))).Any())
                    {
                        //pls dont change the R_SN_LOCK = LOCK_REASON message, its using in another validations
                        var maxTestHours = int.Parse(config.BASETEMPLATE);

                        if (swR.TEST_HOURS >= maxTestHours)
                        {
                            R_SN_LOCK lock1 = new R_SN_LOCK()
                            {
                                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOCK"),
                                TYPE = "SN",
                                SN = swR.SN,
                                LOCK_REASON = "SilverWIP MAX_TEST_HOURS TimeOut, pls Check with PE and QE",
                                LOCK_EMP = "TEAPI",
                                LOCK_STATION = "SILOADING",
                                LOCK_STATUS = "1",
                                LOCK_TIME = MesDbBase.GetOraDbTime(SFCDB.ORM)
                            };
                            SFCDB.ORM.Insertable(lock1).ExecuteCommand();
                        }

                        double in_wip_days = (Math.Round((Convert.ToDateTime(MesDbBase.GetOraDbTime(SFCDB.ORM)).Subtract(Convert.ToDateTime(swR.START_TIME)).TotalDays), 2));

                        if (in_wip_days >= (Convert.ToDouble(config.EXTEND)))
                        {
                            R_SN_LOCK lock1 = new R_SN_LOCK()
                            {
                                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOCK"),
                                TYPE = "SN",
                                SN = swR.SN,
                                LOCK_REASON = "SilverWIP MAX_WIP_DAYS TimeOut, pls Check with PE and QE",
                                LOCK_EMP = "TEAPI",
                                LOCK_STATION = "SILOADING",
                                LOCK_STATUS = "1",
                                LOCK_TIME = MesDbBase.GetOraDbTime(SFCDB.ORM)
                            };
                            SFCDB.ORM.Insertable(lock1).ExecuteCommand();
                        }
                    }

                }
                catch (Exception ex)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = ex.Message;
                    StationReturn.Data = new object();
                }

                #region comment historical logic R_JUNIPER_SILVER_WIP table
                /*
                R_JUNIPER_SILVER_WIP swNew = new R_JUNIPER_SILVER_WIP()
                {
                    ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_JUNIPER_SILVER_WIP"),
                    SN = swR.SN,
                    START_TIME = swR.START_TIME,
                    IN_WIP_USER = swR.IN_WIP_USER,
                    END_TIME = swR.END_TIME,
                    OUT_WIP_USER = swR.OUT_WIP_USER,
                    SKUNO = swR.SKUNO,
                    EDIT_EMP = "MESTEAPI",
                    EDIT_TIME = sysdate,
                    STATE_FLAG = "0",
                    TEST_HOURS = swObj.TEST_HOURS
                };


                var strNum = SFCDB.ORM.Insertable<R_JUNIPER_SILVER_WIP>(swNew).ExecuteCommand();
                if (strNum == 1)
                {
                    ret.Add($@"Type: 'Insert'");
                    ret.Add($@"OK:ID='{swNew.ID}'");
                    ret.Add($@"SN: '{swNew.SN}'");
                    ret.Add($@"Result: 'Successfully'");
                    hasPass = true;
                }
                else
                {
                    ret.Add($@"Type: 'Insert'");
                    ret.Add($@"Err:ID='{swNew.ID}'");
                    ret.Add($@"SN:'{swNew.SN}'");
                    ret.Add($@"Result: 'Error: Cant Inserrt into R_JUNIPER_SILVER_WIP'");

                    hasFail = true;
                }

                */
                #endregion

                if (hasPass == true && hasFail == false)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }
                else if (hasPass == false && hasFail == true)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
                else if (hasPass == true && hasFail == true)
                {
                    StationReturn.Status = StationReturnStatusValue.PartialSuccess;
                }

                StationReturn.Data = ret;

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                //StationReturn.MessageCode = ee.Message.Trim();
                StationReturn.Data = new object();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void LockUnitSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            string ErrMessage;

            try
            {
                var JsonData = Data["JsonData"];
                var lockObj = Newtonsoft.Json.JsonConvert.DeserializeObject<R_SN_LOCK>(JsonData.ToString());

                var swR = SFCDB.ORM.Queryable<R_SN_LOCK>().Where(l => l.SN == lockObj.SN && l.LOCK_STATUS == "1").Any();

                if (swR)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODELOCKAPI0001", new string[] { lockObj.SN });
                    throw new MESReturnMessage(ErrMessage);
                }
                else 
                {
                    string newLockID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOCK");

                    R_SN_LOCK snLock = new R_SN_LOCK()
                    {
                        ID = newLockID,
                        TYPE = "SN",
                        SN = lockObj.SN,
                        LOCK_REASON = lockObj.LOCK_REASON,
                        LOCK_EMP = "BGA-TEAPI",
                        LOCK_STATION = "ALL",
                        LOCK_STATUS = "1",
                        LOCK_TIME = MesDbBase.GetOraDbTime(SFCDB.ORM)
                    };
                    var res = SFCDB.ORM.Insertable(snLock).ExecuteCommand();
                    
                    if(res == 1)
                    {
                        ret.Add($@"OK:ID='{newLockID}'");
                        ret.Add($@"SN: '{lockObj.SN}'");
                        ret.Add($@"Result: 'Successfully Locked'");
                    }

                }
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ret;

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
                //StationReturn.MessageCode = ee.Message.Trim();
                StationReturn.Data = new object();
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

    }
}
