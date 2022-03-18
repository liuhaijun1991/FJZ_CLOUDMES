using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module.DCN;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MESDataObject.Module;
using System.Threading;
using MESDBHelper;
using MESDataObject.Module.Juniper;
using MESStation.LogicObject;
using MESDataObject.Constants;
using static MESDataObject.Constants.PublicConstants;

namespace MESStation.ATE.DCN
{
    public class DCNTE : MesAPIBase
    {
        protected APIInfo _SQLQUERY = new APIInfo()
        {
            FunctionName = "SQLQUERY",
            Description = "SQLQUERY",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SQL", InputType = "string", DefaultValue = "" },

            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _SQLQUERYByObj = new APIInfo()
        {
            FunctionName = "SQLQUERYByObj",
            Description = "SQLQUERYByObj",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Select", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "TableName", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetWWNDatasharing = new APIInfo()
        {
            FunctionName = "GetWWNDatasharing",
            Description = "GetWWNDatasharing",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CSSN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "VSSN", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _UpdateWWNDatasharing = new APIInfo()
        {
            FunctionName = "UpdateWWNDatasharing",
            Description = "UpdateWWNDatasharing",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "JsonData", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetTEStationMapping = new APIInfo()
        {
            FunctionName = "GetTEStationMapping",
            Description = "GetTEStationMapping",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "TESTATION", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _UpdateR_TEST_BRCD = new APIInfo()
        {
            FunctionName = "UpdateR_TEST_BRCD",
            Description = "UpdateR_TEST_BRCD",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "JsonData", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _UpdateR_TEST_JUNIPER = new APIInfo()
        {
            FunctionName = "UpdateR_TEST_JUNIPER",
            Description = "UpdateR_TEST_JUNIPER",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "JsonData", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };

        public DCNTE()
        {
            this.Apis.Add(_GetWWNDatasharing.FunctionName, _GetWWNDatasharing);
            this.Apis.Add(_UpdateWWNDatasharing.FunctionName, _UpdateWWNDatasharing);
            this.Apis.Add(_SQLQUERY.FunctionName, _SQLQUERY);
            this.Apis.Add(_GetTEStationMapping.FunctionName, _GetTEStationMapping);
            this.Apis.Add(_SQLQUERYByObj.FunctionName, _SQLQUERYByObj);
            this.Apis.Add(_UpdateR_TEST_BRCD.FunctionName, _UpdateR_TEST_BRCD);
            this.Apis.Add(_UpdateR_TEST_JUNIPER.FunctionName, _UpdateR_TEST_JUNIPER);
            this._MastLogin = false;
        }

        public void Temp(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            StationReturn.Status = StationReturnStatusValue.Pass;
        }

        public static object[] syncObj = new object[] { new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object(), new object() };
        public static Random rnd = new Random();
        public void SQLQUERY(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            int syncindex = rnd.Next(0, 9);
            object syncobj = syncObj[syncindex];
            var isEnter = false;
            int enterCount = 0;
            while (isEnter == false)
            {
                enterCount++;
                isEnter = Monitor.TryEnter(syncobj);
                if (isEnter == false && enterCount > 20)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "流量限制";
                    //throw new Exception("流量限制！");
                    return;
                }
                else if (isEnter == true)
                {
                    break;
                }
                Thread.Sleep(100);
            }

            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var SQL = Data["SQL"].ToString();
                SQL = SQL.Trim();
                if (SQL.Substring(0, 6).ToUpper() != "SELECT")
                {
                    throw new Exception("只允许运行SELECT语句!");
                }

                var ret = SFCDB.RunSelect(SQL).Tables[0];
                StationReturn.Data = ConvertToJson.DataTableToJson(ret);
                StationReturn.Status = StationReturnStatusValue.Pass;

            }
            catch (Exception ee)
            {

                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "SQLQUERY " + ee.Message;
            }
            finally
            {
                Monitor.Exit(syncobj);
                if (SFCDB != null)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }

            }


        }

        public void SQLQUERYByObj(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var Select = Data["Select"];
                var TableName = Data["TableName"].ToString();

                Assembly assembly = Assembly.Load("MESDataObject");

                var query = SFCDB.ORM.Queryable(TableName, TableName);
                var types = assembly.GetExportedTypes();
                Type TableClass = null;
                foreach (var t in types)
                {
                    if (t.Name.ToUpper() == TableName.ToUpper())
                    {
                        TableClass = t;
                    }
                }
                if (TableClass == null)
                {
                    throw new Exception($@"Can't Querty Table {TableName}!");
                }
                var Pros = TableClass.GetProperties();
                string strWhere = $@"select * from {TableName} Where 1 = 1 ";
                foreach (var p in Pros)
                {
                    if (Select[p.Name] != null)
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            strWhere += "and " + p.Name + "='" + Select[p.Name].ToString() + "' ";
                        }
                        else if (p.PropertyType == typeof(DateTime?))
                        {

                        }
                        else if (p.PropertyType == typeof(double?) || p.PropertyType == typeof(int?))
                        {
                            strWhere += "and " + p.Name + "=" + Select[p.Name].ToString() + " ";
                        }


                    }
                }

                var rs = SFCDB.RunSelect(strWhere);


                StationReturn.Data = ConvertToJson.DataTableToJson(rs.Tables[0]);
                StationReturn.Status = StationReturnStatusValue.Pass;

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "SQLQUERYByObj " + ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void UpdateWWNDatasharing(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                var JsonData = Data["JsonData"];
                StationReturn.Status = StationReturnStatusValue.Pass;

                bool hasPass = false;
                bool hasFail = false;
                for (int i = 0; i < JsonData.Count(); i++)
                {
                    try
                    {
                        var wwn = Newtonsoft.Json.JsonConvert.DeserializeObject<WWN_DATASHARING>(JsonData[i].ToString());
                        if (wwn.ID == null || wwn.ID == "")
                        {
                            //不能直接寫新的記錄，應該是查詢到後再更新，不然傳數據給客人就有問題。如果查詢不到說明流程有問題，SMTLOADING沒LOAD板子就跑到後面？？？
                            //throw new Exception("WWW.ID does not exist, Please check first!");
                            wwn.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "WWN_DATASHARING");
                            SFCDB.ORM.Insertable<WWN_DATASHARING>(wwn).ExecuteCommand();
                        }
                        else
                        {
                            //not update new MAC request from TE - niemnv 2022/03/17
                            var checkmac = SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.ID == wwn.ID).ToList();
                            if (checkmac[0].MAC != "")
                            {
                                wwn.MAC = checkmac[0].MAC;
                            }
                            SFCDB.ORM.Updateable<WWN_DATASHARING>(wwn).Where(t => t.ID == wwn.ID).ExecuteCommand();
                        }
                        ret.Add($@"OK:ID='{wwn.ID}'");
                        hasPass = true;
                    }
                    catch (Exception ee)
                    {
                        ret.Add($@"Err:{ee.Message}");
                        hasFail = true;
                    }

                }
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

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "UpdateWWNDatasharing " + ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void UpdateR_TEST_BRCD(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            bool hasPass = false;
            bool hasFail = false;
            try
            {
                var JsonData = Data["JsonData"];

                for (int i = 0; i < JsonData.Count(); i++)
                {
                    var wwn = Newtonsoft.Json.JsonConvert.DeserializeObject<R_TEST_BRCD>(JsonData[i].ToString());
                    try
                    {
                        //ARUBA有個神邏輯：MES系統SN不帶PN後綴,Label和測試記錄帶PN後綴,加個邏輯：如果是ARUBA的就Like查詢  Request By PE 吳忠義 2022-01-29
                        if (!string.IsNullOrEmpty(wwn.SYSSERIALNO))
                        {
                            string SufFix = wwn.SYSSERIALNO;
                            string strInput = wwn.SYSSERIALNO;
                            int position = strInput.IndexOf(' ');
                            if (position > 0)
                            {
                                strInput = strInput.Substring(0, position);

                                var customer = SFCDB.ORM.Queryable<R_SN, R_WO_BASE, C_SKU, C_SERIES, C_CUSTOMER>((r, wo, sku, se, cus) => r.WORKORDERNO == wo.WORKORDERNO && wo.SKUNO == sku.SKUNO && sku.C_SERIES_ID == se.ID && se.CUSTOMER_ID == cus.ID)
                                                                    .Where((r, wo, sku, se, cus) => r.SN == strInput && r.VALID_FLAG == "1" && cus.CUSTOMER_NAME.ToUpper() == MESDataObject.Constants.Customer.ARUBA.Ext<EnumExtensions.EnumValueAttribute>().Description)
                                                                    .Select((r, wo, sku, se, cus) => cus).ToList();
                                if (customer != null && customer.Count > 0)
                                {
                                    wwn.SYSSERIALNO = strInput;
                                }
                            }
                        }

                        if ((wwn.ID == null || wwn.ID == "") && (wwn.SYSSERIALNO != null || wwn.SYSSERIALNO != ""))
                        {
                            DateTime sysdate = SFCDB.ORM.GetDate();
                            wwn.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_BRCD");
                            if (wwn.TATIME == null)
                            {
                                wwn.TATIME = sysdate;
                            }

                            var T = new R_TEST_RECORD()
                            {
                                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_RECORD"),
                                SN = wwn.SYSSERIALNO,
                                DETAILTABLE = "R_TEST_BRCD",
                                ENDTIME = wwn.TATIME,
                                STATE = wwn.STATUS,
                                MESSTATION = wwn.EVENTNAME,
                                TESTATION = wwn.EVENTNAME,
                                DEVICE = wwn.TESTERNO,
                                EDIT_EMP = wwn.LASTEDITBY,
                                EDIT_TIME = sysdate,
                                STARTTIME = wwn.TESTDATE
                            };
                            wwn.R_TEST_RECORD_ID = T.ID;
                            var sn = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == wwn.SYSSERIALNO && t.VALID_FLAG == "1").First();
                            if (sn != null)
                            {
                                T.R_SN_ID = sn.ID;
                            }
                            //else
                            //{
                            //    ret.Add($@"Err:ID='{wwn.ID}'");
                            //    ret.Add($@"StationName:'{ wwn.EVENTNAME}'");
                            //    StationReturn.Status = StationReturnStatusValue.Fail;
                            //    StationReturn.Message = "SN not found in MES";
                            //    return;
                            //}

                            //獲取TE測試工站與MES工站MAPPING關係 Edit By ZHB 2020年7月23日15:24:16
                            string mesStation = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(t => t.TE_STATION == wwn.EVENTNAME).Select(t => t.MES_STATION).First();
                            if (!string.IsNullOrEmpty(mesStation))
                            {
                                T.MESSTATION = mesStation;
                            }

                            var strNum = SFCDB.ORM.Insertable<R_TEST_BRCD>(wwn).ExecuteCommand();
                            if (strNum == 1)
                            {
                                ret.Add($@"OK:ID='{wwn.ID}'");
                                ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"Err:ID='{wwn.ID}'");
                                ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                                hasFail = true;
                            }
                            SFCDB.ORM.Insertable(T).ExecuteCommand();

                        }
                        else if ((wwn.ID == null || wwn.ID == "") && (wwn.SYSSERIALNO == null || wwn.SYSSERIALNO == ""))
                        {
                            ret.Add($@"Err:ID='{wwn.ID}'");
                            ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                            hasFail = true;
                        }
                        else
                        {
                            var str = SFCDB.ORM.Updateable<R_TEST_BRCD>(wwn).Where(t => t.ID == wwn.ID).ExecuteCommand();
                            if (str == 1)
                            {
                                ret.Add($@"OK:ID='{wwn.ID}'");
                                ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"Err:ID='{wwn.ID}'");
                                ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                                hasFail = true;
                            }
                        }

                        #region BROADCOM DCN測試記錄超過六次會鎖定
                        if ("POST-RUNIN,POST-ESS".Split(',').Contains(wwn.EVENTNAME) &&
                            SFCDB.ORM.Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER>((rs, cs, ss, cc) => rs.SKUNO == cs.SKUNO && cs.C_SERIES_ID == ss.ID && ss.CUSTOMER_ID == cc.ID)
                                .Where((rs, cs, ss, cc) => rs.SN == wwn.SYSSERIALNO && cc.CUSTOMER_NAME == Customer.BROADCOM.ExtValue()).Select((rs, cs, ss, cc) => cc).Any())
                        {
                            var testrec = SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == wwn.SYSSERIALNO && t.TESTATION == wwn.EVENTNAME).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Asc).ToList();
                            if (testrec.Count >= 6)
                            {
                                var testcount = 1;
                                if ((Convert.ToDateTime(testrec.LastOrDefault().EDIT_TIME) - Convert.ToDateTime(testrec[testrec.Count - 2].EDIT_TIME)).Hours >= 12)
                                    for (int m = 1; m < testrec.Count; m++)
                                        if ((Convert.ToDateTime(testrec[m].EDIT_TIME) - Convert.ToDateTime(testrec[m - 1].EDIT_TIME)).Hours >= 12)
                                            testcount++;
                                if (testcount >= 6 &&
                                    !SFCDB.ORM.Queryable<R_SN_LOCK>().Any(t => t.TYPE == "SN" && t.SN == wwn.SYSSERIALNO && t.LOCK_STATION == "ALL" && t.LOCK_REASON.Contains("LockCode:TE0001") && t.LOCK_STATUS == MesBool.Yes.ExtValue()))
                                {
                                    SFCDB.ORM.Insertable(new R_SN_LOCK()
                                    {
                                        ID = MesDbBase.GetNewID<R_SN_LOCK>(SFCDB.ORM, Customer.JUNIPER.ExtValue()),
                                        TYPE = "SN",
                                        SN = wwn.SYSSERIALNO,
                                        LOCK_REASON = $@"{wwn.EVENTNAME} 工站檢查測試次數不能超過6次,Runin log 間隔12Hours以上才算是一次-LockCode:TE0001!",
                                        LOCK_EMP = "TEAPI",
                                        LOCK_STATION = "ALL",
                                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                                        LOCK_TIME = MesDbBase.GetOraDbTime(SFCDB.ORM)
                                    }).ExecuteCommand();
                                }
                            }
                        }
                        #endregion

                    }
                    catch (Exception ee)
                    {
                        ret.Add($@"Err:{ee.Message}");
                        ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                        hasFail = true;
                    }


                }
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

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "UpdateR_TEST_BRCD " + ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void GetTEStationMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();

            string ret = null;
            try
            {
                string tt = Data["TESTATION"].ToString();
                //var sql = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(t => t.TE_STATION == tt).Select(t => t.MES_STATION).ToSql();
                string mesStation = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(t => t.TE_STATION == tt).Select(t => t.MES_STATION).First();

                ret = mesStation;
                if (ret == null)
                {
                    ret = "";
                }
            }
            catch (Exception ee)
            { }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetWWNDatasharing(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string WSN = null;
                string VSSN = null;
                string CSSN = null;
                try
                {
                    WSN = Data["WSN"].ToString();
                }
                catch
                { }
                try
                {
                    VSSN = Data["VSSN"].ToString();
                }
                catch
                { }
                try
                {
                    CSSN = Data["CSSN"].ToString();
                }
                catch
                { }

                if (WSN == "" && CSSN == "" && VSSN == "")
                {
                    throw new Exception("查询条件不能为空!");
                }

                var query = SFCDB.ORM.Queryable<WWN_DATASHARING>();//.Where(t => t.WSN == WSN || t.VSSN == VSSN || t.CSSN == CSSN).ToList();
                if (WSN != "")
                {
                    query = query.Where(t => t.WSN == WSN);
                }
                if (VSSN != "")
                {
                    query = query.Where(t => t.VSSN == VSSN);
                }
                if (CSSN != "")
                {
                    query = query.Where(t => t.CSSN == CSSN);
                }
                var ret = query.OrderBy(t => t.LASTEDITDT).ToList();
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetR_TEST_JUNIPER_Column(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                Type type = typeof(R_TEST_JUNIPER);
                PropertyInfo[] column_list = type.GetProperties();
                ret = column_list.Select(l => l.Name).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = ret;
                StationReturn.Message = "OK";

            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void UpdateR_TEST_JUNIPER(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            bool hasPass = false;
            bool hasFail = false;
            try
            {
                //Data = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject("{JsonData: [{SYSSERIALNO: \"7828800021088759\",TESTDATE: \"2021-02-22 16:03:21\",EVENTNAME: \"ICT\",STATUS: \"PASS\",TESTERNO: \"P51ICT2_200.168.111.21\",LASTEDITBY: \"V8889999\"}]}"); 
                var JsonData = Data["JsonData"];
                for (int i = 0; i < JsonData.Count(); i++)
                {
                    var wwn = Newtonsoft.Json.JsonConvert.DeserializeObject<R_TEST_JUNIPER>(JsonData[i].ToString());
                    try
                    {
                        if ((wwn.ID == null || wwn.ID == "") && (wwn.SYSSERIALNO != null || wwn.SYSSERIALNO != ""))
                        {
                            DateTime sysdate = SFCDB.ORM.GetDate();
                            wwn.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_JUNIPER");
                            if (wwn.TATIME == null)
                            {
                                wwn.TATIME = sysdate;
                            }
                            if (wwn.PHASE == null || wwn.PHASE == "")
                            {
                                wwn.PHASE = "PRODUCTION";
                            }
                            if (wwn.LOAD_DATE == null)
                            {
                                wwn.LOAD_DATE = sysdate;
                            }

                            var T = new R_TEST_RECORD()
                            {
                                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_RECORD"),
                                SN = wwn.SYSSERIALNO,
                                DETAILTABLE = "R_TEST_JUNIPER",
                                ENDTIME = wwn.TATIME,
                                STATE = wwn.STATUS,
                                MESSTATION = wwn.EVENTNAME,
                                TESTATION = wwn.EVENTNAME,
                                DEVICE = wwn.TESTERNO,
                                EDIT_EMP = wwn.LASTEDITBY,
                                EDIT_TIME = sysdate,
                                STARTTIME = wwn.TESTDATE,

                            };



                            if (wwn.SERIAL_NUMBER == null || wwn.SERIAL_NUMBER == "")
                            {
                                wwn.SERIAL_NUMBER = wwn.SYSSERIALNO;
                            }
                            wwn.R_TEST_RECORD_ID = T.ID;
                            var sn = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == wwn.SYSSERIALNO && t.VALID_FLAG == "1").First();
                            if (sn != null)
                            {
                                T.R_SN_ID = sn.ID;
                                if (wwn.PART_NUMBER == null || wwn.PART_NUMBER == "")
                                {
                                    wwn.PART_NUMBER = sn.SKUNO;
                                }
                                if (wwn.UNIQUE_TEST_ID == null || wwn.UNIQUE_TEST_ID == "")
                                {
                                    wwn.UNIQUE_TEST_ID = T.ID;
                                }
                                if (wwn.CM_ODM_PARTNUMBER == null || wwn.CM_ODM_PARTNUMBER == "")
                                {
                                    wwn.CM_ODM_PARTNUMBER = sn.SKUNO;
                                }
                                if (wwn.CAPTURE_TIME == null)
                                {
                                    try
                                    {
                                        wwn.CAPTURE_TIME = DateTime.Parse(wwn.TEST_START_TIMESTAMP);
                                    }
                                    catch
                                    {
                                        wwn.CAPTURE_TIME = DateTime.Now;
                                    }
                                }

                                wwn.LOAD_DATE = DateTime.Now;

                            }
                            var sku = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == wwn.PART_NUMBER).First();
                            if (sku != null)
                            {
                                wwn.PART_NUMBER_REVISION = sku.VERSION;
                            }


                            //獲取TE測試工站與MES工站MAPPING關係 Edit By ZHB 2020年7月23日15:24:16
                            string mesStation = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(t => t.TE_STATION == wwn.EVENTNAME).Select(t => t.MES_STATION).First();
                            if (!string.IsNullOrEmpty(mesStation))
                            {
                                T.MESSTATION = mesStation;
                            }

                            var strNum = SFCDB.ORM.Insertable<R_TEST_JUNIPER>(wwn).ExecuteCommand();
                            if (strNum == 1)
                            {
                                ret.Add($@"OK:ID='{wwn.ID}'");
                                ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"Err:ID='{wwn.ID}'");
                                ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                                hasFail = true;
                            }
                            //用子板的測試記錄過站
                            var kp1 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == wwn.SYSSERIALNO && t.SCANTYPE == "SN" && t.VALID_FLAG == 1).First();
                            if (kp1 != null)
                            {
                                if (kp1.R_SN_ID != null)
                                {
                                    var psn = SFCDB.ORM.Queryable<R_SN>().Where(t => t.ID == kp1.R_SN_ID).First();
                                    if (psn != null)
                                    {
                                        var config = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where
                                            (t => t.SKUNO == psn.SKUNO && t.CATEGORY == "PASSSTATION" && t.CATEGORY_NAME == "REPLACE_SN" && t.EXTEND == T.MESSTATION).First();
                                        if (config != null)
                                        {
                                            T.SN = psn.SN;
                                            T.R_SN_ID = psn.ID;
                                        }

                                    }

                                }
                            }

                            SFCDB.ORM.Insertable(T).ExecuteCommand();

                        }
                        else if ((wwn.ID == null || wwn.ID == "") && (wwn.SYSSERIALNO == null || wwn.SYSSERIALNO == ""))
                        {
                            ret.Add($@"Err:ID='{wwn.ID}'");
                            ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                            hasFail = true;
                        }
                        else
                        {



                            var str = SFCDB.ORM.Updateable<R_TEST_JUNIPER>(wwn).Where(t => t.ID == wwn.ID).ExecuteCommand();
                            if (str == 1)
                            {
                                ret.Add($@"OK:ID='{wwn.ID}'");
                                ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"Err:ID='{wwn.ID}'");
                                ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                                hasFail = true;
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        ret.Add($@"Err:{ee.Message}");
                        ret.Add($@"StationName:'{wwn.EVENTNAME}'");
                        hasFail = true;
                    }
                }
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
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }


        public void UpdateR_TEST_JUNIPER_New(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            bool hasPass = false;
            bool hasFail = false;
            try
            {
                //Data = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject("{JsonData: [{SYSSERIALNO: \"7828800021088759\",TESTDATE: \"2021-02-22 16:03:21\",EVENTNAME: \"ICT\",STATUS: \"PASS\",TESTERNO: \"P51ICT2_200.168.111.21\",LASTEDITBY: \"V8889999\"}]}"); 
                var JsonData = Data["JsonData"];
                for (int i = 0; i < JsonData.Count(); i++)
                {
                    var TestData = Newtonsoft.Json.JsonConvert.DeserializeObject<JuniperTestRecord>(JsonData[i].ToString());
                    try
                    {
                        if ((TestData.DESC_DATA.ID == null || TestData.DESC_DATA.ID == "") && (TestData.DESC_DATA.SYSSERIALNO != null || TestData.DESC_DATA.SYSSERIALNO != ""))
                        {
                            DateTime sysdate = SFCDB.ORM.GetDate();
                            TestData.DESC_DATA.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_JUNIPER");
                            if (TestData.DESC_DATA.TATIME == null)
                            {
                                TestData.DESC_DATA.TATIME = sysdate;
                            }
                            if (TestData.DESC_DATA.PHASE == null || TestData.DESC_DATA.PHASE == "")
                            {
                                TestData.DESC_DATA.PHASE = "PRODUCTION";
                            }
                            if (TestData.DESC_DATA.LOAD_DATE == null)
                            {
                                TestData.DESC_DATA.LOAD_DATE = sysdate;
                            }

                            var T = new R_TEST_RECORD()
                            {
                                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_RECORD"),
                                SN = TestData.DESC_DATA.SYSSERIALNO,
                                DETAILTABLE = "R_TEST_JUNIPER",
                                ENDTIME = TestData.DESC_DATA.TATIME,
                                STATE = TestData.DESC_DATA.STATUS,
                                MESSTATION = TestData.DESC_DATA.EVENTNAME,
                                TESTATION = TestData.DESC_DATA.EVENTNAME,
                                DEVICE = TestData.DESC_DATA.TESTERNO,
                                EDIT_EMP = TestData.DESC_DATA.LASTEDITBY,
                                EDIT_TIME = sysdate,
                                STARTTIME = TestData.DESC_DATA.TESTDATE,

                            };
                            if (TestData.DESC_DATA.SERIAL_NUMBER == null || TestData.DESC_DATA.SERIAL_NUMBER == "")
                            {
                                TestData.DESC_DATA.SERIAL_NUMBER = TestData.DESC_DATA.SYSSERIALNO;
                            }
                            TestData.DESC_DATA.R_TEST_RECORD_ID = T.ID;
                            var sn = SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == TestData.DESC_DATA.SYSSERIALNO && t.VALID_FLAG == "1").First();
                            if (sn != null)
                            {
                                T.R_SN_ID = sn.ID;
                                if (TestData.DESC_DATA.PART_NUMBER == null || TestData.DESC_DATA.PART_NUMBER == "")
                                {
                                    TestData.DESC_DATA.PART_NUMBER = sn.SKUNO;
                                }
                                if (TestData.DESC_DATA.UNIQUE_TEST_ID == null || TestData.DESC_DATA.UNIQUE_TEST_ID == "")
                                {
                                    TestData.DESC_DATA.UNIQUE_TEST_ID = T.ID;
                                }
                                if (TestData.DESC_DATA.CM_ODM_PARTNUMBER == null || TestData.DESC_DATA.CM_ODM_PARTNUMBER == "")
                                {
                                    TestData.DESC_DATA.CM_ODM_PARTNUMBER = sn.SKUNO;
                                }
                                if (TestData.DESC_DATA.CAPTURE_TIME == null)
                                {
                                    try
                                    {
                                        TestData.DESC_DATA.CAPTURE_TIME = DateTime.Parse(TestData.DESC_DATA.TEST_START_TIMESTAMP);
                                    }
                                    catch
                                    {
                                        TestData.DESC_DATA.CAPTURE_TIME = DateTime.Now;
                                    }
                                }

                                TestData.DESC_DATA.LOAD_DATE = DateTime.Now;

                            }
                            var sku = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == TestData.DESC_DATA.PART_NUMBER).First();
                            if (sku != null)
                            {
                                TestData.DESC_DATA.PART_NUMBER_REVISION = sku.VERSION;
                            }


                            //獲取TE測試工站與MES工站MAPPING關係 Edit By ZHB 2020年7月23日15:24:16
                            string mesStation = SFCDB.ORM.Queryable<C_TEMES_STATION_MAPPING>().Where(t => t.TE_STATION == TestData.DESC_DATA.EVENTNAME).Select(t => t.MES_STATION).First();
                            if (!string.IsNullOrEmpty(mesStation))
                            {
                                T.MESSTATION = mesStation;
                            }

                            var strNum = SFCDB.ORM.Insertable<R_TEST_JUNIPER>(TestData.DESC_DATA).ExecuteCommand();
                            if (strNum == 1)
                            {
                                ret.Add($@"OK:ID='{TestData.DESC_DATA.ID}'");
                                ret.Add($@"StationName:'{TestData.DESC_DATA.EVENTNAME}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"Err:ID='{TestData.DESC_DATA.ID}'");
                                ret.Add($@"StationName:'{TestData.DESC_DATA.EVENTNAME}'");
                                hasFail = true;
                            }

                            //用子板的測試記錄過站
                            var kp1 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == T.SN && t.SCANTYPE == "SN" && t.VALID_FLAG == 1).First();
                            if (kp1 != null)
                            {
                                if (kp1.R_SN_ID != null)
                                {
                                    var psn = SFCDB.ORM.Queryable<R_SN>().Where(t => t.ID == kp1.R_SN_ID).First();
                                    if (psn != null)
                                    {
                                        var config = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where
                                            (t => t.SKUNO == psn.SKUNO && t.CATEGORY == "PASSSTATION" && t.CATEGORY_NAME == "REPLACE_SN" && t.EXTEND == T.MESSTATION).First();
                                        if (config != null)
                                        {
                                            T.SN = psn.SN;
                                            T.R_SN_ID = psn.ID;
                                        }

                                    }

                                }
                            }

                            SFCDB.ORM.Insertable(T).ExecuteCommand();

                        }
                        else if ((TestData.DESC_DATA.ID == null || TestData.DESC_DATA.ID == "") && (TestData.DESC_DATA.SYSSERIALNO == null || TestData.DESC_DATA.SYSSERIALNO == ""))
                        {
                            ret.Add($@"Err:ID='{TestData.DESC_DATA.ID}'");
                            ret.Add($@"StationName:'{TestData.DESC_DATA.EVENTNAME}'");
                            hasFail = true;
                        }
                        else
                        {
                            var str = SFCDB.ORM.Updateable<R_TEST_JUNIPER>(TestData.DESC_DATA).Where(t => t.ID == TestData.DESC_DATA.ID).ExecuteCommand();
                            if (str == 1)
                            {
                                ret.Add($@"OK:ID='{TestData.DESC_DATA.ID}'");
                                ret.Add($@"StationName:'{TestData.DESC_DATA.EVENTNAME}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"Err:ID='{TestData.DESC_DATA.ID}'");
                                ret.Add($@"StationName:'{TestData.DESC_DATA.EVENTNAME}'");
                                hasFail = true;
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        ret.Add($@"Err:{ee.Message}");
                        ret.Add($@"StationName:'{TestData.DESC_DATA.EVENTNAME}'");
                        hasFail = true;
                    }
                }
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
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void UpdateR_TEST_JSNLIST(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            bool hasPass = false;
            bool hasFail = false;
            try
            {
                //Data = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject("{JsonData: [{SYSSERIALNO: \"7828800021088759\",TESTDATE: \"2021-02-22 16:03:21\",EVENTNAME: \"ICT\",STATUS: \"PASS\",TESTERNO: \"P51ICT2_200.168.111.21\",LASTEDITBY: \"V8889999\"}]}"); 
                var JsonData = Data["JsonData"];
                for (int i = 0; i < JsonData.Count(); i++)
                {
                    var jSnList = Newtonsoft.Json.JsonConvert.DeserializeObject<R_TEST_JSNLIST>(JsonData[i].ToString());
                    try
                    {
                        jSnList.SERIALNO = jSnList.SERIALNO.ToUpper().Trim();
                        jSnList.VALID_FLAG = "1";
                        if (!SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == jSnList.SERIALNO && t.VALID_FLAG == "1").Any())
                        {
                            throw new Exception("SN not exists in MES, Please check!");
                        }

                        if (!SFCDB.ORM.Queryable<R_TEST_JSNLIST>().Where(t => t.SERIALNO == jSnList.SERIALNO).Any())
                        {
                            jSnList.CREAT_TIME = SFCDB.ORM.GetDate();
                            jSnList.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_TEST_JSNLIST");
                            var strNum = SFCDB.ORM.Insertable<R_TEST_JSNLIST>(jSnList).ExecuteCommand();
                            if (strNum == 1)
                            {
                                ret.Add($@"OK:ID='{jSnList.ID}'");
                                ret.Add($@"SN='{jSnList.SERIALNO}'");
                                ret.Add($@"STATUS:'{jSnList.STATUS}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"Err:ID='{jSnList.ID}'");
                                ret.Add($@"SN='{jSnList.SERIALNO}'");
                                ret.Add($@"STATUS:'{jSnList.STATUS}'");
                                hasFail = true;
                            }
                        }
                        else
                        {
                            jSnList.EDIT_TIME = SFCDB.ORM.GetDate();

                            var str = SFCDB.ORM.Updateable<R_TEST_JSNLIST>().SetColumns(t => new R_TEST_JSNLIST { STATUS = jSnList.STATUS }).Where(t => t.SERIALNO == jSnList.SERIALNO).ExecuteCommand();
                            if (str > 0)
                            {
                                ret.Add($@"OK:ID='{jSnList.ID}'");
                                ret.Add($@"SN='{jSnList.SERIALNO}'");
                                ret.Add($@"STATUS:'{jSnList.STATUS}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"Err:ID='{jSnList.ID}'");
                                ret.Add($@"SN='{jSnList.SERIALNO}'");
                                ret.Add($@"STATUS:'{jSnList.STATUS}'");
                                hasFail = true;
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        ret.Add($@"Err:'{ee.Message}'");
                        ret.Add($@"SN='{jSnList.SERIALNO}'");
                        ret.Add($@"STATUS:'{jSnList.STATUS}'");
                        hasFail = true;
                    }
                }
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
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Data = ret;
            }
        }

        public void UpdateRotationDetail(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            bool hasPass = false;
            bool hasFail = false;
            try
            {
                var JsonData = Data["JsonData"];

                for (int i = 0; i < JsonData.Count(); i++)
                {
                    var rotationObj = Newtonsoft.Json.JsonConvert.DeserializeObject<R_SILVER_ROTATION_DETAIL>(JsonData[i].ToString());
                    try
                    {
                        var rsr = SFCDB.ORM.Queryable<R_SILVER_ROTATION>().Where(r => r.SN == rotationObj.CSN && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                        if (rsr == null)
                        {
                            throw new Exception($@"{rotationObj.CSN} can not check in silver rotation");
                        }
                        rsr.ROTATION_FLAG = rotationObj.ENDTIME == null ? 1 : 0;
                        rsr.FIRST_ROTATION_TIME = rsr.FIRST_ROTATION_TIME == null ? rotationObj.STARTTIME : rsr.FIRST_ROTATION_TIME;
                        if (string.IsNullOrEmpty(rotationObj.ID))
                        {
                            DateTime sysdate = SFCDB.ORM.GetDate();
                            rotationObj.ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SILVER_ROTATION_DETAIL");

                            if (string.IsNullOrEmpty(rotationObj.STARTBY))
                            {
                                rotationObj.STARTBY = "SYSTEM";
                            }
                            if (string.IsNullOrEmpty(rotationObj.ENDBY))
                            {
                                rotationObj.ENDBY = "SYSTEM";
                            }
                            rotationObj.VALID_FLAG = "1";
                            rotationObj.SEQNO = rsr.TOLAL_ROTATION_TIMES + 1;
                            var result = SFCDB.ORM.Insertable<R_SILVER_ROTATION_DETAIL>(rotationObj).ExecuteCommand();
                            if (result == 1)
                            {
                                ret.Add($@"OK:ID='{rotationObj.ID}'");
                                ret.Add($@"SN:'{rotationObj.SN}'");
                                ret.Add($@"CSN:'{rotationObj.CSN}'");

                                rsr.TOLAL_ROTATION_TIMES += 1;
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"SN:'{rotationObj.SN}'");
                                ret.Add($@"CSN:'{rotationObj.CSN}'");
                                hasFail = true;
                            }
                        }
                        else
                        {
                            var rsrd_old = SFCDB.ORM.Queryable<R_SILVER_ROTATION_DETAIL>().Where(r => r.ID == rotationObj.ID).ToList().FirstOrDefault();
                            rsrd_old.STARTTIME = rotationObj.STARTTIME == null ? rsrd_old.STARTTIME : rotationObj.STARTTIME;
                            rsrd_old.STARTBY = rotationObj.STARTBY == null ? rsrd_old.STARTBY : rotationObj.STARTBY;
                            rsrd_old.ENDTIME = rotationObj.ENDTIME == null ? rsrd_old.ENDTIME : rotationObj.ENDTIME;
                            rsrd_old.ENDBY = rotationObj.ENDBY == null ? rsrd_old.ENDBY : rotationObj.ENDBY;
                            rsrd_old.STATION_NAME = rotationObj.STATION_NAME == null ? rsrd_old.STATION_NAME : rotationObj.STATION_NAME;

                            var str = SFCDB.ORM.Updateable<R_SILVER_ROTATION_DETAIL>(rsrd_old).Where(t => t.ID == rsrd_old.ID).ExecuteCommand();
                            if (str == 1)
                            {
                                ret.Add($@"OK:ID='{rotationObj.ID}'");
                                ret.Add($@"SN:'{rotationObj.SN}'");
                                ret.Add($@"CSN:'{rotationObj.CSN}'");
                                hasPass = true;
                            }
                            else
                            {
                                ret.Add($@"SN:'{rotationObj.SN}'");
                                ret.Add($@"CSN:'{rotationObj.CSN}'");
                                hasFail = true;
                            }
                        }
                        if (hasPass)
                        {
                            SFCDB.ORM.Updateable<R_SILVER_ROTATION>(rsr).ExecuteCommand();
                        }
                    }
                    catch (Exception ee)
                    {
                        ret.Add($@"Err:{ee.Message}");
                        ret.Add($@"SN:'{rotationObj.SN}'");
                        ret.Add($@"CSN:'{rotationObj.CSN}'");
                        hasFail = true;
                    }
                }
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
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "R_SILVER_ROTATION_DETAIL " + ee.Message;
            }
            finally
            {
                StationReturn.Data = ret;
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }
    }

    public class JuniperTestRecord
    {
        public R_TEST_JUNIPER DESC_DATA;
        public List<TestFailInfo> FailInfo = new List<TestFailInfo>();
    }

    public class TestFailInfo
    {
        public string FailCode;
        public string Location;
        public string FailDesc;

    }

}
