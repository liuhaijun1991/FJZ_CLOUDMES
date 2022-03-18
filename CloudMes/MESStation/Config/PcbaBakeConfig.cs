using MESDBHelper;
using MESPubLab.MESStation;
using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;

namespace MESStation.Config
{
    public class PcbaBakeConfig : MesAPIBase
    {
        protected APIInfo FGetInformation = new APIInfo()
        {
            FunctionName = "GetInformation",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetBgaData = new APIInfo()
        {
            FunctionName = "GetBgaData",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FInserData = new APIInfo()
        {
            FunctionName = "InserData",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "MODEL", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "STATION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ACTION", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCATION1", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "COMTYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "COMPN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "COMID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DESC", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "RES2DX", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetBGAList = new APIInfo()
        {
            FunctionName = "GetBGAList",
            Description = "获取可选用的配置子类型",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FCheckLocationNum = new APIInfo()
        {
            FunctionName= "CheckLocationNum",
            Description="檢查同一個SN在同一個位置做BGA的次數",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCATION", InputType = "string", DefaultValue = "" },
            },
            Permissions = new List<MESPermission>() { }
        };
        public PcbaBakeConfig()
        {
            this.Apis.Add(FGetInformation.FunctionName, FGetInformation);
            this.Apis.Add(FGetBgaData.FunctionName, FGetBgaData);
            this.Apis.Add(FInserData.FunctionName, FInserData);
            this.Apis.Add(FGetBGAList.FunctionName, FGetBGAList);
            this.Apis.Add(FCheckLocationNum.FunctionName, FCheckLocationNum);
        }
        public void GetInformation(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            var date = Data["DATES"].ToString().Trim().ToUpper();
            var sn = Data["SN"].ToString().Trim().ToUpper();
            var opType = Data["OpType"].ToString().Trim().ToUpper();
            var bakeFlag = Data["BakeFlag"].ToString().Trim().ToUpper();
            var hours = Data["INHUORS"].ToString().Trim().ToUpper();
            string flag = string.Empty;
            string sqlRun = string.Empty;
            string strHours = string.Empty;
            bool bb = false;
            T_R_BGA_DETAIL r_bga = null;
            try
            {
                if (bakeFlag == "YES")
                {
                    flag = "1";
                }
                else
                {
                    flag = "0";
                }
                sqlRun = "SELECT SYSDATE-4 FROM DUAL";
                DateTime DBTime = (DateTime)db.ExecSelectOneValue(sqlRun);
                bb = db.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.VALID_FLAG == "1" && SqlSugar.SqlFunc.Contains(t.STATION_NAME, "LOADING") && t.EDIT_TIME < DBTime).Any();
                if (bb)
                {
                    if (hours == "4HOURS")
                    {
                        strHours = "4";
                    }
                    else if (hours == "12HOURS")
                    {
                        strHours = "12";
                    }
                    else if (hours == "24HOURS")
                    {
                        strHours = "24";
                    }
                    else if (hours == "48HOURS")
                    {
                        strHours = "48";
                    }

                    if ((strHours != "12" && strHours != "24" && strHours != "48") && opType == "START_BAKE")
                    {
                        StationReturn.Message = $@"{sn}-->距離LOADING已超過4天，請選擇12/24/48小時烘烤時間!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                }
                else
                {
                    strHours = "4";
                }

                bb = db.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG =="1").Any();
                if (!bb)
                {
                    StationReturn.Message = $@"{sn}--> 不存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                if (opType == "START_BAKE")
                {
                    //var s  = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn).ToList();
                    //if (s.Count != 0) {
                    //    StationReturn.Message = $@"'{sn}'--> 已重工過 '{s.Count}' 次";
                    //    StationReturn.Status = StationReturnStatusValue.Fail;
                    //    StationReturn.Data = "";
                    //    return;
                    //}
                    var ss = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.BAKE_FLAG == "1").ToList();
                    if (ss.Count > 0)
                    {
                        StationReturn.Message = $@"{sn}--> 上一次烘烤流程未完成,不能開始新的烘烤!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }

                    r_bga = new T_R_BGA_DETAIL(db, DB_TYPE_ENUM.Oracle);
                    Row_R_BGA_DETAIL r = (Row_R_BGA_DETAIL)r_bga.NewRow();
                    r.ID = r_bga.GetNewID(BU, db);
                    r.SN = sn;
                    r.BAKE_FLAG = flag;
                    r.CREATE_TIME = r_bga.GetDBDateTime(db);
                    r.BAKE_START = r_bga.GetDBDateTime(db);
                    r.EDIT_EMP = this.LoginUser.EMP_NO;
                    r.HOURS = strHours;
                    string strRet = db.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (Convert.ToInt32(strRet) > 0)
                    {
                        StationReturn.Message = "添加成功！！";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Data = "";
                    }
                    else
                    {
                        StationReturn.MessageCode = "MES00000036";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                    }

                }
                if (opType == "END_BAKE")
                {
                    r_bga = new T_R_BGA_DETAIL(db, DB_TYPE_ENUM.Oracle);
                    sqlRun = $@"SELECT SYSDATE-1/6 FROM DUAL";
                    DateTime DBTimes = (DateTime)db.ExecSelectOneValue(sqlRun);

                    sqlRun = $@"SELECT SYSDATE-1/2 FROM DUAL";
                    DateTime Time12 = (DateTime)db.ExecSelectOneValue(sqlRun);

                    sqlRun = $@"SELECT SYSDATE-1 FROM DUAL";
                    DateTime times = (DateTime)db.ExecSelectOneValue(sqlRun);

                    sqlRun = $@"SELECT SYSDATE-2 FROM DUAL";
                    DateTime Time48 = (DateTime)db.ExecSelectOneValue(sqlRun);

                    DateTime time = r_bga.GetDBDateTime(db);

                    bool ab = db.ORM.Queryable<R_SN, C_SKU, C_SERIES, C_CUSTOMER>((rsn, csk, css, ccs) => rsn.SKUNO == csk.SKUNO && csk.C_SERIES_ID == css.ID && css.CUSTOMER_ID == ccs.ID)
                        .Where((rsn, csk, css, ccs) => rsn.SN == sn && rsn.VALID_FLAG == "1" && ccs.CUSTOMER_NAME == "ORACLE").Select((rsn, csk, css, ccs) => rsn).Any();

                    if (!ab)
                    {
                        ab = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.HOURS == "4").Any();
                        if (ab)
                        {
                            ab = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.BAKE_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(t.BAKE_END) && SqlSugar.SqlFunc.Length(t.BAKE_START) > 0 && t.BAKE_START < DBTimes).Any();
                            if (!ab)
                            {
                                StationReturn.Message = $@"{sn}-->該SN未掃入烘烤或者烘烤時間不足4個小時,不能結束烘烤!";
                                StationReturn.Status = StationReturnStatusValue.Fail;
                                StationReturn.Data = "";
                                return;
                            }
                        }
                        else if (db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.HOURS == "12").Any())
                        {
                            ab = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.BAKE_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(t.BAKE_END) && SqlSugar.SqlFunc.Length(t.BAKE_START) > 0 && t.BAKE_START < Time12).Any();
                            if (!ab)
                            {
                                StationReturn.Message = $@"{sn}-->該SN未掃入烘烤或者烘烤時間不足12個小時,不能結束烘烤!";
                                StationReturn.Status = StationReturnStatusValue.Fail;
                                StationReturn.Data = "";
                                return;
                            }
                        }
                        else if (db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.HOURS == "24").Any())
                        {
                            ab = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.BAKE_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(t.BAKE_END) && SqlSugar.SqlFunc.Length(t.BAKE_START) > 0 && t.BAKE_START < times).Any();
                            if (!ab)
                            {
                                StationReturn.Message = $@"{sn}-->該SN未掃入烘烤或者烘烤時間不足24個小時,不能結束烘烤!";
                                StationReturn.Status = StationReturnStatusValue.Fail;
                                StationReturn.Data = "";
                                return;
                            }
                        }
                        else if (db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.HOURS == "48").Any())
                        {
                            ab = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.BAKE_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(t.BAKE_END) && SqlSugar.SqlFunc.Length(t.BAKE_START) > 0 && t.BAKE_START < Time48).Any();
                            if (!ab)
                            {
                                StationReturn.Message = $@"{sn}-->該SN未掃入烘烤或者烘烤時間不足48個小時,不能結束烘烤!";
                                StationReturn.Status = StationReturnStatusValue.Fail;
                                StationReturn.Data = "";
                                return;
                            }
                        }
                    }
                    else
                    {
                        ab = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.BAKE_FLAG == "1" && SqlSugar.SqlFunc.IsNullOrEmpty(t.BAKE_END) && SqlSugar.SqlFunc.Length(t.BAKE_START) > 0 && t.BAKE_START < times).Any();
                        if (!ab)
                        {
                            StationReturn.Message = $@"{sn}-->該SN未掃入烘烤或者烘烤時間不足24個小時,不能結束烘烤!";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                            return;
                        }


                    }

                    int result = db.ORM.Updateable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.BAKE_FLAG == "1").UpdateColumns(t => new R_BGA_DETAIL() { BAKE_END = time }).ExecuteCommand();

                    if (result > 0)
                    {
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000001");
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Data = "";
                    }
                    else
                    {
                        StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000025");
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
            }
        }
        public void GetBgaData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            var sn = Data["SN"].ToString().Trim().ToUpper();
            bool bol = false;
            try
            {
                bol = db.ORM.Queryable<R_SN>().Where(it => it.SN == sn && it.VALID_FLAG == "1").Any();

                if (!bol)
                {
                    StationReturn.Message = $@"{sn}-->該SN不存在";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                //var res = db.ORM.Queryable<R_BGA_DETAIL>().Where(it => it.SN == sn && it.BAKE_FLAG == "2" && SqlSugar.SqlFunc.Length(it.BAKE_END) > 0).OrderBy(it => it.BAKE_END, SqlSugar.OrderByType.Desc).ToList();
                //if (res.Count > 0)
                //{
                //    StationReturn.Message = $@"{sn}-->該SN BGA BAKE已結束,請確認";
                //    StationReturn.Status = StationReturnStatusValue.Fail;
                //    StationReturn.Data = "";
                //    return;
                //}
                //bol = db.ORM.Queryable<R_BGA_DETAIL>().Where(it => it.SN == sn && it.BAKE_FLAG == "1" && SqlSugar.SqlFunc.Length(it.BAKE_END) > 0).Any();
                //if (!bol)
                //{
                //    StationReturn.Message = $@"{sn}-->該SN上一階段烘烤還未結束,請確認";
                //    StationReturn.Status = StationReturnStatusValue.Fail;
                //    StationReturn.Data = "";
                //    return;
                //}
                //檢查最新一筆記錄是否做結束烘烤，其他不檢查
                var lastRecord = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn).OrderBy(t => t.BAKE_START, SqlSugar.OrderByType.Desc).First();
                if (lastRecord != null)
                {
                    if (lastRecord.BAKE_END == null)
                    {
                        StationReturn.Message = $@"{sn}-->該SN上一階段烘烤還未結束,請確認";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                    if (lastRecord.BAKE_FLAG == "2")
                    {
                        StationReturn.Message = $@"{sn}-->該SN上一階段BGA已完成,尚未開始下一階段烘烤,請確認";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                }
                //同一個SN不同位置只允許做3次BGA
                var reNum = db.ORM.Queryable<R_BGA_DETAIL>().Where(t => t.SN == sn && t.BAKE_FLAG == "2").ToList().Count() + 1;
                if (reNum > 3)
                {
                    StationReturn.Message = $@"{sn}-->同一S/N BGA已重工{(reNum - 1).ToString()} 次,請核對S/N信息!如需繼續重工,請聯繫褚武生或蘇桂!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
            }
        }
        public void InserData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            var sn = Data["SN"].ToString().Trim().ToUpper();
            var model = Data["MODEL"].ToString().Trim().ToUpper();
            var station = Data["STATION"].ToString().Trim().ToUpper();
            var action = Data["ACTION"].ToString().Trim().ToUpper();
            var location = Data["LOCATION1"].ToString().Trim().ToUpper();
            var comType = Data["COMTYPE"].ToString().Trim().ToUpper();
            var comPN = Data["COMPN"].ToString().Trim().ToUpper();
            var comID = Data["COMID"].ToString().Trim().ToUpper();
            var desc = Data["DESC"].ToString().Trim().ToUpper();
            var res2DX = Data["RES2DX"].ToString().Trim().ToUpper();
            //var res5dx = Data["RES5DX"].ToString().Trim().ToUpper();
            var empNO = LoginUser.EMP_NO;
            int reNum = 0;
            string strNum = string.Empty;
            bool bol = false;
            try
            {
                if (desc == "")
                {
                    desc = "N/A";
                }

                var num = db.ORM.Queryable<R_BGA_DETAIL>().Where(it => it.SN == sn && it.BAKE_FLAG == "2").ToList();
                if (num.Count() > 0)
                {
                    reNum = num.Count() + 1;
                }
                else
                {
                    reNum = 1;
                }
                // strNum = Convert.ToString(reNum);
                var bgaL = db.ORM.Queryable<R_BGA_DETAIL>().Where(it => it.SN == sn && it.LOCATION == location).ToList();

                bol = db.ORM.Queryable<R_2DXRAY>().Where(it => it.SN == sn).Any();
                if (!bol)
                {
                    StationReturn.Message = $@"{sn}-->沒有2DX記錄的SN不允許重工BGA!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                else
                {
                    //BGA次數取決於2DX次數
                    var dxNum = db.ORM.Queryable<R_2DXRAY>().Where(it => it.SN == sn).ToList().Count();
                    if (dxNum < reNum)
                    {
                        StationReturn.Message = $@"{sn}-->只有 {dxNum.ToString()} 次2DX記錄不允許第 {reNum} 次重工BGA!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                }
                if (empNO != "F1321438" || empNO != "G6002147" || empNO != "BA")
                {
                    //var bb = db.ORM.Queryable<R_SN, C_SKU, C_SERIES>((rsn, csk, css) => rsn.SKUNO == csk.SKUNO && csk.C_SERIES_ID == css.ID)
                    //     .Where((rsn, csk, css) => rsn.SN == sn && SqlSugar.SqlFunc.Contains(css.DESCRIPTION, "MERCURY"))
                    //     .Where((rsn, csk, css) => SqlSugar.SqlFunc.Subqueryable<C_BGA_SET>().Where(it => it.SKUNO == csk.SKUNO).NotAny()).Select((rsn, csk, css) => rsn);

                    var times = db.ORM.Queryable<R_SN, C_BGA_SET>((rsn, cbs) => SqlSugar.SqlFunc.StartsWith(rsn.SKUNO, cbs.PARTNO))
                        .Where((rsn, cbs) => rsn.SN == sn && rsn.VALID_FLAG =="1").Select((rsn, cbs) => cbs).ToList().FirstOrDefault();
                    if (times != null)
                    {
                        int singleNum = Convert.ToInt32(times.LOC_TIMES);
                        int totalNum = Convert.ToInt32(times.TIMES);
                        if (reNum > singleNum)
                        {
                            StationReturn.Message = $@"{sn}-->Mercury機種重工BGA次數不能超過{singleNum} 次,現已重工{reNum} 次，如需繼續重工,請聯繫褚武生或蘇桂 !";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                            return;
                        }
                        if (bgaL.Count() > totalNum)
                        {
                            StationReturn.Message = $@"{sn}-->Mercury機種重工BGA次數不能超過{totalNum} 次,現已重工{bgaL.Count()} 次，如需繼續重工,請聯繫褚武生或蘇桂 !";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                            return;
                        }

                    }
                    if (bgaL.Count() != 0 && reNum < 4)
                    {
                        if (bgaL.Count() > 1)
                        {
                            StationReturn.Message = $@"{sn}-->同一S/N同一位置BGA最多只能重工2次,要重工3次請聯繫褚武生或蘇桂!";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                            return;
                        }
                    }
                    else
                    {
                        if (reNum > 3)
                        {
                            StationReturn.Message = $@"{sn}-->同一S/N BGA已重工{(reNum - 1).ToString()} 次,請核對S/N信息!如需繼續重工,請聯繫褚武生或蘇桂!";
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.Data = "";
                            return;
                        }
                    }
                }
                else
                {
                    if (bgaL.Count() >= 2)
                    {
                        StationReturn.Message = $@"{sn}-->同一S/N同一位置BGA最多只能重工2次!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                    if (num.Count() >= 3)
                    {
                        StationReturn.Message = $@"{sn}-->同一S/N BGA最多只能重工4次!";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.Data = "";
                        return;
                    }
                }

                T_R_BGA_DETAIL r_bga = new T_R_BGA_DETAIL(db, DB_TYPE_ENUM.Oracle);
                DateTime time = r_bga.GetDBDateTime(db);
                int result = db.ORM.Updateable<R_BGA_DETAIL>().Where(it => it.SN == sn && it.BAKE_FLAG == "1").UpdateColumns(it => new R_BGA_DETAIL()
                {
                    SKUNO = model,
                    PARTNO = comPN,
                    BGA_LOCATION = location,
                    STATION = station,
                    ACTION = action,
                    EDIT_TIME = time,
                    LOCATION = location,
                    COMPONENT_ID = comID,
                    XRAYRESULT = res2DX,
                    DEBUGRESULT = "N/A",
                    TESTRESULT = "N/A",
                    REMARK = desc,
                    REPAIR_ID = empNO,
                    REWORK_NUM = Convert.ToString(reNum),
                    BAKE_FLAG = "2",
                    BGA_TYPE = comType
                }).ExecuteCommand();

                if (result > 0)
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000001");
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000025");
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
            }
        }

        public void GetBGAList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                StationReturn.Data = sfcdb.ORM.Queryable<R_BGA_DETAIL>().ToList();
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }
        public void GetBakeList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                StationReturn.Data = sfcdb.ORM.Queryable<R_BGA_DETAIL>().Select(it => new R_BGA_DETAIL
                {
                    ID = it.ID,
                    SN = it.SN,
                    BAKE_FLAG = SqlSugar.SqlFunc.IF(it.BAKE_FLAG == "1").Return("PASS").ElseIF(it.BAKE_FLAG == "2").Return("PASS").End("FAIL"),
                    CREATE_TIME = it.CREATE_TIME,
                    BAKE_START = it.BAKE_START,
                    BAKE_END = it.BAKE_END,
                    BAKE_ID = it.BAKE_ID
                }).ToList();
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
        }

        public void CheckLocationNum(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            var sn = Data["SN"].ToString().Trim().ToUpper();
            var location = Data["LOCATION"].ToString().ToUpper().Trim();
            try
            {
                var bgaL = db.ORM.Queryable<R_BGA_DETAIL>().Where(it => it.SN == sn && it.LOCATION == location).ToList();
                if (bgaL.Count() > 1)
                {
                    StationReturn.Message = $@"{sn}-->同一S/N同一位置BGA最多只能重工2次,要重工3次請聯繫褚武生或蘇桂!";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                    return;
                }
                StationReturn.Message = "OK";
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (DBPools["SFCDB"] != null)
                {
                    DBPools["SFCDB"].Return(db);
                }
            }
        }
    }
}
