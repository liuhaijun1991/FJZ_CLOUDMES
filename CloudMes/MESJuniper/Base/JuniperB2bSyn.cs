using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
using MESPubLab;
using SqlSugar;
using System.Data;
using System.Reflection;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module.DCN;
using MESDataObject.Module.Juniper;
using MESPubLab.Common;
using DbType = SqlSugar.DbType;
using static MESDataObject.Common.EnumExtensions;
using System.Globalization;
using static MESDataObject.Constants.PublicConstants;
using MESPubLab.MESInterface;
using MESDataObject.Module.OM;
using static MESJuniper.Base.SynAck;
using System.Threading;
using MESJuniper.OrderManagement;
using MESPubLab.SAP_RFC;
using MESPubLab.MesBase;

namespace MESJuniper.Base
{
    public class JuniperB2bSyn : FunctionBase
    {
        static public double defaultsynday = -35;
        public JuniperB2bSyn(string _dbstr, string _b2bdbstr, string _bu) : base(_dbstr, _bu)
        {
        }
    }
    public class Syn140 : FunctionBase
    {
        string b2bdbstr, mesdbstr = string.Empty; double defaultsynday = -3;
        public Syn140(string _mesdbstr, string _b2bdbstr, string _bu, double? _defaultsynday = null) : base(_mesdbstr, _bu)
        {
            b2bdbstr = _b2bdbstr;
            mesdbstr = _mesdbstr;
            if (_defaultsynday != null) defaultsynday = Convert.ToDouble(_defaultsynday);
        }

        //public override void FunctionRun()
        //{
        //    using (var b2bdb = OleExec.GetSqlSugarClient(b2bdbstr, false, DbType.SqlServer))
        //    {
        //        var a = Convert.ToDateTime("2021-03-11");
        //        var waitsynlist = b2bdb.Queryable<B2B_R_I140>()
        //            .Where(t => SqlSugar.SqlFunc.ToDate(t.F_LASTEDITDT) > DateTime.Now.AddDays(double.Parse(defaultsynday.ToString())) && t.CREATIONDATETIME > a).ToList();
        //        if (waitsynlist.Count > 0)
        //            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
        //            {
        //                var tranids = waitsynlist.Select(t => t.TRANID).Distinct();
        //                var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
        //                var exsist = mesdb.Queryable<R_I140>().Where(t => IMesDbEx.OracleContain(t.F_ID, filterIDs)).ToList();
        //                foreach (var ctranid in tranids)
        //                {
        //                    var traniddetail = waitsynlist.Where(t => t.TRANID == ctranid).ToList();
        //                    var dbres = mesdb.Ado.UseTran(() =>
        //                    {
        //                        if (!mesdb.Queryable<R_I140_MAIN>().Where(t => t.TRANID == ctranid).Any())
        //                            mesdb.Insertable(new R_I140_MAIN()
        //                            {
        //                                ID = MesDbBase.GetNewID<R_I140_MAIN>(mesdb, Customer.JUNIPER.ExtValue()),
        //                                TRANID = ctranid,
        //                                WEEKNO = new GregorianCalendar().GetWeekOfYear(Convert.ToDateTime($@"{ctranid.Substring(0, 4)}-{ctranid.Substring(4, 2)}-{ctranid.Substring(6, 2)}"), CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
        //                                YEARNO = DateTime.Now.Year.ToString(),
        //                                COMPLETE = I_I140_MAIN_ENUM.COMPLETE_NO.Ext<EnumValueAttribute>().Description,
        //                                PLANT = traniddetail[0].VENDORCODE == JuniperB2BPlantCode.FJZ.Ext<EnumValueAttribute>().Description ? JuniperB2BPlantCode.FJZ.ToString() : JuniperB2BPlantCode.FVN.ToString(),
        //                                CREATETIME = DateTime.Now,
        //                                EDITTIME = DateTime.Now
        //                            }).ExecuteCommand();
        //                        //foreach (MesWeekDay w in Enum.GetValues(typeof(MesWeekDay)))
        //                        //{
        //                        //    var currentV = w.ToString();
        //                        //    if (!mesdb.Queryable<R_I140_MAIN_D>().Where(t => t.TRANID == ctranid && t.COMMITDAY == currentV).Any())
        //                        //        mesdb.Insertable(new R_I140_MAIN_D()
        //                        //        {
        //                        //            ID = MesDbBase.GetNewID<R_I140_MAIN_D>(mesdb, Customer.JUNIPER.ExtValue()),
        //                        //            TRANID = ctranid,
        //                        //            COMMITDAY = currentV,
        //                        //            COMPLETE = I_I140_MAIN_D_ENUM.COMPLETE_NO.Ext<EnumValueAttribute>().Description,
        //                        //            CREATETIME = DateTime.Now,
        //                        //            EDITTIME = DateTime.Now
        //                        //        }).ExecuteCommand();
        //                        //}
        //                        foreach (var item in traniddetail)
        //                        {
        //                            if (exsist.FindAll(t => t.F_ID == item.F_ID).Count > 0)
        //                                continue;
        //                            var targetobj = ObjectDataHelper.Mapper<R_I140, B2B_R_I140>(item);
        //                            targetobj.ID = MesDbBase.GetNewID<R_I140>(mesdb, Customer.JUNIPER.ExtValue());
        //                            targetobj.CREATETIME = DateTime.Now;
        //                            targetobj.MFLAG = "N";
        //                            mesdb.Insertable(targetobj).ExecuteCommand();
        //                        }
        //                    });
        //                    if (!dbres.IsSuccess)
        //                        throw new Exception(dbres.ErrorMessage);
        //                }
        //            }
        //    }
        //}

        public override void FunctionRun()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(b2bdbstr, false, DbType.SqlServer))
            {
                var vender = JuniperB2BPlantCode.FVN.ExtValue();
                if (bu == JuniperB2BPlantCode.FJZ.ToString())
                    vender = JuniperB2BPlantCode.FJZ.ExtValue();
                var waitsynlist = b2bdb.Queryable<B2B_R_I140>()
                    .Where(t => SqlSugar.SqlFunc.ToDate(t.F_LASTEDITDT) > DateTime.Now.AddDays(double.Parse(defaultsynday.ToString())) && t.VENDORCODE == vender).ToList();
                if (waitsynlist.Count > 0)
                    try
                    {
                        using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
                        {
                            var tranids = waitsynlist.Select(t => t.TRANID).Distinct();
                            //var mes140data = mesdb.Queryable<R_I140>().OrderBy(t => SqlFunc.ToInt32(t.F_ID), OrderByType.Desc).Take(100000).ToList();
                            var mes140data = mesdb.Queryable<R_I140>().OrderBy(t => t.CREATETIME, OrderByType.Desc).Take(100000).ToList();//F_ID exists invalid number such as TMTM092646
                            foreach (var ctranid in tranids)
                            {
                                var res = mesdb.Ado.UseTran(() =>
                                {
                                    var traniddetail = waitsynlist.Where(t => t.TRANID == ctranid).ToList();
                                    var targetdata = new List<R_I140>();
                                    var count = 0;
                                    var lasttime = DateTime.Now;
                                    var current140map = ObjectDataHelper.Mapper<R_I140, B2B_R_I140>(traniddetail);
                                    foreach (var item in current140map)
                                    {
                                        //先检查缓存; 
                                        if (mes140data.Any(t => t.F_ID == item.F_ID))
                                            continue;
                                        if (mesdb.Queryable<R_I140>().Where(t => t.F_ID == item.F_ID).Any())
                                            continue;
                                        //var targetobj = ObjectDataHelper.Mapper<R_I140, B2B_R_I140>(item);
                                        //targetobj.ID = MesDbBase.GetNewID<R_I140>(mesdb, Customer.JUNIPER.ExtValue());
                                        item.ID = item.F_ID;
                                        item.CREATETIME = DateTime.Now;
                                        item.MFLAG = "N";
                                        targetdata.Add(item);
                                        count++;
                                        if (count % 1000 == 0)
                                        {
                                            var t = new TimeSpan(DateTime.Now.Ticks - lasttime.Ticks).TotalSeconds;
                                            lasttime = DateTime.Now;
                                            MesLog.Debug($@"Jnp140-Get140:tol{traniddetail.Count} ID :{item.ID} count: {count} time:{t}");
                                        }
                                    }
                                    //foreach (var item in current140map)
                                    //{
                                    //    //先检查缓存; 
                                    //    if (mes140data.Any(t => t.F_ID == item.F_ID))
                                    //        continue;
                                    //    if (mesdb.Queryable<R_I140>().Where(t => t.F_ID == item.F_ID).Any())
                                    //        continue;
                                    //    var targetobj = ObjectDataHelper.Mapper<R_I140, B2B_R_I140>(item);
                                    //    //targetobj.ID = MesDbBase.GetNewID<R_I140>(mesdb, Customer.JUNIPER.ExtValue());
                                    //    targetobj.ID = targetobj.F_ID;
                                    //    targetobj.CREATETIME = DateTime.Now;
                                    //    targetobj.MFLAG = "N";
                                    //    targetdata.Add(targetobj);
                                    //    count++;
                                    //    if (count % 1000 == 0)
                                    //    {
                                    //        var t = new TimeSpan(DateTime.Now.Ticks - lasttime.Ticks).TotalSeconds;
                                    //        lasttime = DateTime.Now;
                                    //        MesLog.Debug($@"Jnp140-Get140:tol{traniddetail.Count} ID :{targetobj.ID} count: {count} time:{t}");
                                    //    }
                                    //}
                                    MesLog.Debug($@"Jnp140-Insert Start:{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}");
                                    mesdb.Insertable(targetdata).ExecuteCommand();
                                    MesLog.Debug($@"Jnp140-Insert End:{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}");
                                    if (!mesdb.Queryable<R_I140_MAIN>().Where(t => t.TRANID == ctranid).Any())
                                        mesdb.Insertable(new R_I140_MAIN()
                                        {
                                            ID = MesDbBase.GetNewID<R_I140_MAIN>(mesdb, Customer.JUNIPER.ExtValue()),
                                            TRANID = ctranid,
                                            WEEKNO = new GregorianCalendar().GetWeekOfYear(Convert.ToDateTime($@"{ctranid.Substring(0, 4)}-{ctranid.Substring(4, 2)}-{ctranid.Substring(6, 2)}"), CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
                                            //YEARNO = DateTime.Now.Year.ToString(),
                                            YEARNO = Convert.ToDateTime($@"{ctranid.Substring(0, 4)}-{ctranid.Substring(4, 2)}-{ctranid.Substring(6, 2)}").Year.ToString(),
                                            COMPLETE = I_I140_MAIN_ENUM.COMPLETE_NO.Ext<EnumValueAttribute>().Description,
                                            PLANT = traniddetail[0].VENDORCODE == JuniperB2BPlantCode.FJZ.Ext<EnumValueAttribute>().Description ? JuniperB2BPlantCode.FJZ.ToString() : JuniperB2BPlantCode.FVN.ToString(),
                                            CREATETIME = DateTime.Now,
                                            EDITTIME = DateTime.Now
                                        }).ExecuteCommand();
                                });
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        throw ee;
                    }
            }
        }
    }

    public class Syn285 : FunctionBase
    {
        string b2bdbstr, mesdbstr = string.Empty; double defaultsynday = -5;
        public Syn285(string _mesdbstr, string _b2bdbstr, string _bu, double? _defaultsynday = null) : base(_mesdbstr, _bu)
        {
            b2bdbstr = _b2bdbstr;
            mesdbstr = _mesdbstr;
            if (_defaultsynday != null) defaultsynday = Convert.ToDouble(_defaultsynday);
        }

        public override void FunctionRun()
        {
            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
            {
                var currentDay = MesDbBase.GetOraDbTime(mesdb);
                //pst 05:30 pm
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                DateTime newDateTime = TimeZoneInfo.ConvertTime(currentDay, timeZoneInfo);
                var rangs = newDateTime.Date.AddMinutes(1020);
                var range = newDateTime.Date.AddMinutes(1140);
                if (!(newDateTime >= rangs && newDateTime <= range))
                    return;
                //if (!(newDateTime.Hour == 17 && newDateTime.Minute>30))
                //    return;

                var commitdate = currentDay.ToString("yyyyMMdd");
                var main = mesdb.Queryable<R_I140_MAIN_D>().Where(t => t.COMMITDATE == commitdate).ToList().FirstOrDefault();
                if (main != null && main.COMPLETE == MesBool.Yes.ExtValue())
                    return;
                var sourcedata = mesdb.Queryable<R_I285_MAIN>().Where(t => t.VALID == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                DateTime? stocktime, stockendtime = currentDay;
                #region get cal data
                var soucedatadetail = new List<R_I285>();
                if (sourcedata == null)
                {
                    #region 用上次数据
                    var lasttarget = mesdb.Queryable<R_I140_MAIN_D>().Where(t => t.COMMITDATE != null && t.COMPLETE == MesBool.Yes.ExtValue()).OrderBy(t => t.COMMITDATE, OrderByType.Desc).ToList().FirstOrDefault();
                    if (lasttarget == null) throw new Exception("first data is not exists!");
                    stocktime = lasttarget.STOCKINGTIME;
                    var lasttargetdetail = mesdb.Queryable<R_I285>().Where(t => t.FILENAME == lasttarget.COMMITID).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATE, OrderByType.Asc).ToList();
                    #endregion
                    #region mon
                    if (currentDay.DayOfWeek == DayOfWeek.Monday)
                    {
                        var lastobj = lasttargetdetail.FindAll(t => t.STARTDATE < currentDay.Date);
                        var currentobj = lasttargetdetail.FindAll(t => t.STARTDATE == currentDay.Date);
                        var addobj = new List<R_I285>();
                        foreach (var item in lastobj)
                        {
                            var currentitem = currentobj.FindAll(t => t.PN == item.PN).FirstOrDefault();
                            var addobjitem = addobj.FindAll(t => t.PN == item.PN).FirstOrDefault();
                            if (currentitem == null && addobjitem == null)
                            {
                                item.STARTDATE = currentDay.Date;
                                addobj.Add(item);
                            }
                            else if (currentitem == null)
                                addobjitem.QUANTITY = (Convert.ToInt32(addobjitem.QUANTITY) + Convert.ToInt32(item.QUANTITY)).ToString();
                            else
                                currentitem.QUANTITY = (Convert.ToInt32(currentitem.QUANTITY) + Convert.ToInt32(item.QUANTITY)).ToString();
                        }
                        lasttargetdetail.AddRange(addobj);
                        lasttargetdetail.RemoveAll(t => t.STARTDATE < currentDay.Date);
                    }
                    #endregion
                    soucedatadetail = lasttargetdetail;
                    stocktime = lasttarget.STOCKINGTIME;
                }
                else
                {
                    #region 用户数据
                    var soucedatadetailq = mesdb.Queryable<R_I285_SOURCE>().Where(t => t.FILENAME == sourcedata.FILENAME).OrderBy(t => t.PN, OrderByType.Asc).OrderBy(t => t.STARTDATE, OrderByType.Asc).ToList();
                    soucedatadetail = ObjectDataHelper.Mapper<R_I285, R_I285_SOURCE>(soucedatadetailq);
                    stocktime = sourcedata.STOCKINGTIME == null ? currentDay.Date : sourcedata.STOCKINGTIME;
                    //stocktime = sourcedata.STOCKINGTIME==null?;
                    sourcedata.VALID = MesBool.No.ExtValue();
                    #endregion
                }
                #endregion
                #region get stock data
                var stockdata = mesdb.Queryable<R_SN, O_ORDER_MAIN>((s, m) => s.WORKORDERNO == m.PREWO).Where((s, m) => s.VALID_FLAG == MesBool.Yes.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue()
                      && SqlFunc.Between(s.COMPLETED_TIME, stocktime, stockendtime) && s.CURRENT_STATION != "MRB")
                .GroupBy((s, m) => new { s.WORKORDERNO, m.PID }).Select((s, m) => new { s.WORKORDERNO, m.PID, stocknum = SqlFunc.AggregateCount(s.SN) }).ToList();
                //var stockdata = mesdb.Queryable<R_SN_STATION_DETAIL, O_ORDER_MAIN>((s, m) => s.WORKORDERNO == m.PREWO).Where((s, m) => s.VALID_FLAG == MesBool.Yes.ExtValue() 
                //      && SqlFunc.Between(s.EDIT_TIME, stocktime, stockendtime) && s.CURRENT_STATION == "CBS")
                //.GroupBy((s, m) => new { s.WORKORDERNO, m.PID }).Select((s, m) => new { s.WORKORDERNO, m.PID, stocknum = SqlFunc.AggregateCount(s.SN) }).ToList();
                #endregion
                #region cal
                foreach (var item in stockdata)
                {
                    if (!mesdb.Queryable<O_SKU_ATP>().Any(t => t.FOXPN == item.PID)) continue;
                    var itemcal = mesdb.Queryable<R_PRE_WO_DETAIL, O_SKU_ATP>((w, s) => w.PARTNO == s.FOXPN).Where((w, s) => w.WO == item.WORKORDERNO).GroupBy((w, s) => s.PARTNO)
                        .Select((w, s) => new { s.PARTNO, stockpnnum = SqlFunc.AggregateCount(s.PARTNO) * item.stocknum }).ToList();
                    foreach (var pnitem in itemcal)
                    {
                        var targetitems = soucedatadetail.FindAll(t => t.PN == pnitem.PARTNO && Convert.ToInt32(t.QUANTITY) > 0);
                        if (targetitems.Count == 0) continue;
                        var waitcalno = pnitem.stockpnnum;
                        foreach (var tar in targetitems)
                        {
                            if (waitcalno > Convert.ToInt32(tar.QUANTITY))
                            {
                                tar.QUANTITY = "0";
                                waitcalno = waitcalno - Convert.ToInt32(tar.QUANTITY);
                            }
                            else if (waitcalno <= Convert.ToInt32(tar.QUANTITY))
                            {
                                tar.QUANTITY = (Convert.ToInt32(tar.QUANTITY) - waitcalno).ToString();
                                break;
                            }
                        }
                    }
                }
                #endregion
                #region entity
                var pkid = newDateTime.ToString("yyyyMMddHHmmss");
                soucedatadetail.ForEach(t =>
                {
                    t.ID = MesDbBase.GetNewID<R_I285>(mesdb, this.bu);
                    t.FILENAME = pkid;
                    t.TRANID = $@"{this.bu}{newDateTime.ToString("yyyyMMddHHmmss")}";
                    t.CREATETIME = currentDay;
                });
                mesdb.Ado.UseTran(() =>
                {
                    mesdb.Insertable(soucedatadetail).ExecuteCommand();
                    if (sourcedata != null)
                        mesdb.Updateable(sourcedata).ExecuteCommand();
                    // mesdb
                    if (main == null)
                    {
                        mesdb.Insertable(new R_I140_MAIN_D()
                        {
                            ID = MesDbBase.GetNewID<R_I140_MAIN_D>(mesdb, this.bu),
                            COMMITDAY = currentDay.DayOfWeek.ToString(),
                            COMPLETE = MesBool.Yes.ExtValue(),
                            COMMITID = pkid,
                            CREATETIME = DateTime.Now,
                            EDITTIME = DateTime.Now,
                            VENDORCODE = bu == JuniperB2BPlantCode.FVN.ToString() ? JuniperB2BPlantCode.FVN.ExtValue() : JuniperB2BPlantCode.FJZ.ExtValue(),
                            STOCKINGTIME = stockendtime,
                            COMMITDATE = commitdate
                        }).ExecuteCommand();
                    }
                    else
                    {
                        main.COMMITID = pkid;
                        main.COMPLETE = MesBool.Yes.ExtValue();
                        main.STOCKINGTIME = stockendtime;
                        main.EDITTIME = DateTime.Now;
                        mesdb.Updateable(main).ExecuteCommand();
                    }
                });
                #endregion
            }
        }

        public void FunctionRunbak()
        {
            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
            {
                var currentDay = MesDbBase.GetOraDbTime(mesdb);
                //if (currentDay.DayOfWeek.ToString().Equals(MesWeekDay.Tuesday.ToString()))
                //    return;
                //pst 4:45 pm
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                DateTime newDateTime = TimeZoneInfo.ConvertTime(currentDay, timeZoneInfo);
                //if (currentDay.Hour < 11)
                if (newDateTime.Hour <= 16 && newDateTime.Minute <= 45)
                    return;
                var querydate = currentDay.ToString("yyyyMMdd");
                var querylastdate = currentDay.AddDays(-1).ToString("yyyyMMdd");
                var sendobj = mesdb.Queryable<R_I140_MAIN_D>().Where(t => t.COMMITDATE == querydate && t.COMPLETE == MesBool.Yes.ExtValue()).ToList();
                if (sendobj.Count == 1)
                    return;
                //var lastobj = mesdb.Queryable<R_I140_MAIN_D>().Where(t => t.COMMITDATE == querylastdate && t.COMPLETE == MesBool.Yes.ExtValue()).ToList();
                var lastobj = mesdb.Queryable<R_I140_MAIN_D>().Where(t => t.COMPLETE == MesBool.Yes.ExtValue() && t.COMMITDATE != null).OrderBy(t => t.COMMITDATE, OrderByType.Desc).ToList();
                if (bu == JuniperB2BPlantCode.FVN.ToString())
                {
                    var fvn = sendobj.FindAll(t => t.VENDORCODE == JuniperB2BPlantCode.FVN.ExtValue()).FirstOrDefault();
                    var befvn = lastobj.FindAll(t => t.VENDORCODE == JuniperB2BPlantCode.FVN.ExtValue()).FirstOrDefault();
                    if (fvn == null && befvn != null)
                        send285(mesdb, befvn);
                    //Thread.Sleep(2000);
                }
                else
                {
                    var fjz = sendobj.FindAll(t => t.VENDORCODE == JuniperB2BPlantCode.FJZ.ExtValue()).FirstOrDefault();
                    var befjz = lastobj.FindAll(t => t.VENDORCODE == JuniperB2BPlantCode.FJZ.ExtValue()).FirstOrDefault();
                    if (fjz == null && befjz != null)
                        send285(mesdb, befjz);
                }
            }
        }

        void send285(SqlSugarClient mesdb, R_I140_MAIN_D bemaind)
        {
            var currentDay = MesDbBase.GetOraDbTime(mesdb);
            var querydate = currentDay.ToString("yyyyMMdd");
            var lastdata = mesdb.Queryable<R_I285>().Where(t => t.FILENAME == bemaind.COMMITID).ToList();
            var filename = currentDay.ToString("yyyyMMddHHmmss");
            var tranid = $@"{this.bu}{currentDay.ToString("yyyyMMddHHmmss")}";
            foreach (var item in lastdata)
            {
                item.ID = MesDbBase.GetNewID<R_I285>(mesdb, this.bu);
                item.FILENAME = filename;
                item.TRANID = tranid;
                item.CREATIONDATE = currentDay;
                item.F_LASTEDITDT = DateTime.Now;
                item.CREATETIME = DateTime.Now;
            }
            var res = mesdb.Ado.UseTran(() =>
            {
                mesdb.Insertable(lastdata).ExecuteCommand();
                mesdb.Insertable(new R_I140_MAIN_D()
                {
                    ID = MesDbBase.GetNewID<R_I140_MAIN_D>(mesdb, this.bu),
                    TRANID = bemaind.TRANID,
                    COMMITDAY = DateTime.Now.DayOfWeek.ToString(),
                    COMPLETE = MesBool.Yes.ExtValue(),
                    COMMITID = filename,
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now,
                    VENDORCODE = bemaind.VENDORCODE,
                    COMMITDATE = querydate
                }).ExecuteCommand();
            });
            Thread.Sleep(2000);
        }
    }

    public class Syn137 : FunctionBase
    {
        string b2bdbstr, mesdbstr, plantcode = string.Empty; double defaultsynday;
        public Syn137(string _mesdbstr, string _b2bdbstr, string _bu, double? _defaultsynday = null) : base(_mesdbstr, _bu)
        {
            b2bdbstr = _b2bdbstr;
            mesdbstr = _mesdbstr;
            if (_defaultsynday != null) defaultsynday = Convert.ToDouble(_defaultsynday);
            plantcode = new Func<string>(() =>
            {
                if (_bu.Contains(MesPlantSite.FVN.ExtName()))
                    return JuniperB2BPlantCode.FVN.ExtValue();
                else
                    return JuniperB2BPlantCode.FJZ.ExtValue();
            })();
        }

        public override void FunctionRun()
        {
            SysB2bI137();
            I137DataValid();
            NewOrderProcess();
            ChangeOrderProcess();
        }

        void SysB2bI137()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(b2bdbstr, false, DbType.SqlServer))
            {
                var starttime = new DateTime(2021, 02, 17);
                var waitsynlist = b2bdb.Queryable<B2B_R_I137>()
                     .Where(t => SqlSugar.SqlFunc.ToDate(t.F_LASTEDITDT) > DateTime.Now.AddDays(double.Parse(defaultsynday.ToString()))
                     && SqlSugar.SqlFunc.ToDate(t.F_LASTEDITDT) > starttime
                     && t.SALESORDERREFERENCEID == plantcode)
                     .OrderBy(t => t.REPLENISHMENTORDERID, OrderByType.Asc).OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Asc).ToList();

                //waitsynlist = waitsynlist.FindAll(t => t.REPLENISHMENTORDERID == "5100014916").ToList();
                if (waitsynlist.Count > 0)
                    using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
                    {
                        var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
                        var tranIDs = waitsynlist.Select(t => t.TRANID).Distinct().ToList();

                        var mintrid = waitsynlist.Min(t => Convert.ToInt64(t.TRANID));
                        var minFID = waitsynlist.Min(t => Convert.ToInt64(t.F_ID));

                        var b2b137headlist = b2bdb.Queryable<B2B_I137_H>().Where(t => tranIDs.Contains(t.TRANID)).ToList();
                        var b2b137itemlist = b2bdb.Queryable<B2B_I137_I>().Where(t => tranIDs.Contains(t.TRANID)).ToList();
                        var b2b137detaillist = b2bdb.Queryable<B2B_I137_D>().Where(t => tranIDs.Contains(t.TRANID)).ToList();

                        #region 速度太慢，更换缓存方式=>先查缓存
                        //var exsist = mesdb.Queryable<O_I137>().Where(t => IMesDbEx.OracleContain(t.F_ID.ToString(), filterIDs)).ToList();
                        //var om137headlist = mesdb.Queryable<I137_H>().Where(t => IMesDbEx.OracleContain(t.TRANID, tranIDs)).ToList();
                        //var om137itemlist = mesdb.Queryable<I137_I>().Where(t => IMesDbEx.OracleContain(t.TRANID, tranIDs)).ToList();
                        //var om137detaillist = mesdb.Queryable<I137_D>().Where(t => IMesDbEx.OracleContain(t.TRANID, tranIDs)).ToList();

                        var exsist = mesdb.Queryable<O_I137>().Where(t => SqlFunc.ToInt32(t.F_ID) > minFID).OrderBy(t => SqlFunc.ToInt32(t.F_ID), OrderByType.Desc).Take(20000).ToList();
                        var om137headlist = mesdb.Queryable<I137_H>().Where(t => t.CREATETIME > DateTime.Now.AddDays(-2)).OrderBy(t => SqlFunc.ToInt32(t.F_ID), OrderByType.Desc).Take(20000).ToList();
                        var om137itemlist = mesdb.Queryable<I137_I>().Where(t => t.CREATETIME > DateTime.Now.AddDays(-2)).OrderBy(t => SqlFunc.ToInt32(t.F_ID), OrderByType.Desc).Take(20000).ToList();
                        var om137detaillist = mesdb.Queryable<I137_D>().Where(t => t.CREATETIME > DateTime.Now.AddDays(-2)).OrderBy(t => SqlFunc.ToInt32(t.F_ID), OrderByType.Desc).Take(20000).ToList();
                        #endregion

                        foreach (var tranID in tranIDs)
                        {
                            var waitsynTranIdlist = waitsynlist.Where(t => t.TRANID == tranID);
                            var b2b137TranIdlist_head = b2b137headlist.Where(t => t.TRANID == tranID).ToList();
                            var b2b137TranIdlist_item = b2b137itemlist.Where(t => t.TRANID == tranID).ToList();
                            var b2b137TranIdlist_detail = b2b137detaillist.Where(t => t.TRANID == tranID).ToList();
                            if (b2b137TranIdlist_head == null)
                                continue;

                            var po = b2b137TranIdlist_head.FirstOrDefault() != null ? b2b137TranIdlist_head.FirstOrDefault().PONUMBER : string.Empty;
                            var poline = b2b137TranIdlist_item.FirstOrDefault() != null ? b2b137TranIdlist_item.FirstOrDefault().ITEM : string.Empty;

                            if (b2b137TranIdlist_head.Count() != 1) continue;
                            //if (om137headlist.Where(t => t.F_ID == b2b137TranIdlist_head.FirstOrDefault().F_ID).Count() > 0)
                            //    continue;
                            JuniperPoTracking trackobj = new JuniperPoTracking(JuniperErrType.I137, JuniperSubType.SysB2bI137, po, poline, tranID, mesdb);
                            try
                            {
                                var dbres = mesdb.Ado.UseTran(() =>
                                {
                                    #region I137 B2B 原始數據
                                    foreach (var item in waitsynTranIdlist)
                                    {
                                        if (exsist.FindAll(t => t.F_ID.ToString() == item.F_ID.ToString()).Any())
                                            continue;
                                        if (mesdb.Queryable<O_I137>().Where(t => t.F_ID == item.F_ID).Any())
                                            continue;
                                        var targetobj = ObjectDataHelper.Mapper<O_I137, B2B_R_I137>(item);
                                        targetobj.ID = MesDbBase.GetNewID<O_I137>(mesdb, Customer.JUNIPER.ExtValue());
                                        targetobj.CREATETIME = DateTime.Now;
                                        targetobj.MFLAG = MesBool.No.ExtName();
                                        mesdb.Insertable(targetobj).ExecuteCommand();
                                    }
                                    #endregion

                                    #region Head
                                    var b2bheaditem = b2b137TranIdlist_head.FirstOrDefault();
                                    if (!om137headlist.Where(t => t.F_ID == b2bheaditem.F_ID).Any() &&
                                        !mesdb.Queryable<O_I137_HEAD>().Where(t => t.F_ID == b2bheaditem.F_ID).Any())
                                    {
                                        var headmain = mesdb.Queryable<I137_H>().Where(t => t.PONUMBER == b2bheaditem.PONUMBER).OrderBy(t => SqlFunc.ToInt32(t.VERSION), OrderByType.Desc).ToList();
                                        var poversion = headmain.Count == 0 ? 0 : int.Parse(headmain.FirstOrDefault().VERSION) + 1;
                                        var mesheadobj = ObjectDataHelper.Mapper<I137_H, B2B_I137_H>(b2bheaditem);
                                        mesheadobj.ID = MesDbBase.GetNewID<O_I137_HEAD>(mesdb, Customer.JUNIPER.ExtValue());
                                        mesheadobj.VERSION = poversion.ToString();
                                        mesheadobj.CREATETIME = DateTime.Now;
                                        mesheadobj.EDITTIME = DateTime.Now;
                                        mesheadobj.MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue();
                                        mesdb.Insertable(mesheadobj).ExecuteCommand();
                                    }
                                    #endregion

                                    #region by item 處理
                                    foreach (var item in b2b137TranIdlist_item)
                                    {
                                        if (om137itemlist.FindAll(t => t.F_ID.ToString() == item.F_ID.ToString()).Any())
                                            continue;
                                        if (mesdb.Queryable<O_I137_ITEM>().Where(t => t.F_ID == item.F_ID).Any())
                                            continue;
                                        #region Item
                                        var itemmain = ObjectDataHelper.Mapper<I137_I, B2B_I137_I>(item);
                                        itemmain.ID = MesDbBase.GetNewID<O_I137_ITEM>(mesdb, Customer.JUNIPER.ExtValue());
                                        itemmain.CREATETIME = DateTime.Now;
                                        itemmain.MFLAG = ENUM_I137_H_STATUS.WAITCHECK.ExtValue();
                                        itemmain.EDITTIME = DateTime.Now;
                                        mesdb.Insertable(itemmain).ExecuteCommand();
                                        #endregion

                                        #region detail
                                        var itemDetailList = b2b137TranIdlist_detail.Where(t => t.PONUMBER == item.PONUMBER && t.ITEM == item.ITEM).ToList();
                                        foreach (var detialobj in itemDetailList)
                                        {
                                            if (om137detaillist.FindAll(t => t.F_ID.ToString() == detialobj.F_ID.ToString()).Count > 0)
                                                continue;
                                            if (mesdb.Queryable<O_I137_DETAIL>().Where(t => t.F_ID == detialobj.F_ID).Any())
                                                continue;
                                            var detailmain = ObjectDataHelper.Mapper<I137_D, B2B_I137_D>(detialobj);
                                            detailmain.ID = MesDbBase.GetNewID<O_I137_DETAIL>(mesdb, Customer.JUNIPER.ExtValue());
                                            detailmain.CREATETIME = DateTime.Now;
                                            mesdb.Insertable(detailmain).ExecuteCommand();
                                        }
                                        #endregion

                                        #region PO strack
                                        ///
                                        #endregion
                                    }
                                    #endregion
                                });
                                if (!dbres.IsSuccess)
                                    throw new Exception(dbres.ErrorMessage);
                                trackobj.ReleaseFuncExcption();
                            }
                            catch (Exception e)
                            {
                                trackobj.ExceptionProcess(true, e.Message);
                            }
                        }
                    }
            }
        }

        void I137DataValid()
        {
            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
            {
                var waitchecklist = mesdb.Queryable<I137_I>()
                    .Where(t => t.MFLAG == ENUM_I137_H_STATUS.WAITCHECK.Ext<EnumValueAttribute>().Description || t.MFLAG == ENUM_I137_H_STATUS.CHECK_FAIL.Ext<EnumValueAttribute>().Description)
                    .OrderBy(t => t.PONUMBER, OrderByType.Desc).OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Desc).ToList();

                foreach (var item in waitchecklist)
                {
                    //if (item.PN.StartsWith("740-"))
                    //{
                    //    #region 更新狀態
                    //    //item.MFLAG = ENUM_I137_H_STATUS.RELEASE.ExtValue();
                    //    item.MFLAG = ENUM_I137_H_STATUS.CHECK_PASS.ExtValue();
                    //    mesdb.Updateable(item).ExecuteCommand();
                    //    #endregion
                    //    continue;
                    //}
                    var checkres = true;
                    JuniperPoTracking trackobj = new JuniperPoTracking(JuniperErrType.I137, JuniperSubType.I137DataValid, item.PONUMBER, item.ITEM, item.TRANID, mesdb);
                    var heads = mesdb.Queryable<I137_H>().Where(t => t.TRANID == item.TRANID).ToList().FirstOrDefault();
                    var items = mesdb.Queryable<I137_I>().Where(t => t.TRANID == item.TRANID).ToList().FirstOrDefault();
                    var detail = mesdb.Queryable<I137_D>().Where(t => t.TRANID == item.TRANID).ToList();

                    #region 只處理最新的
                    var citem = mesdb.Queryable<I137_I>().Where(t => t.PONUMBER == item.PONUMBER && t.ITEM == item.ITEM && (t.MFLAG == ENUM_I137_H_STATUS.RELEASE.ExtValue() || t.MFLAG == ENUM_I137_H_STATUS.CHECK_PASS.ExtValue())).OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Desc).ToList().FirstOrDefault();
                    if (heads.POCHANGEINDICATOR == Juniper137PoType.Change.ExtValue() && citem != null && items.LASTCHANGEDATETIME < citem.LASTCHANGEDATETIME)
                    {
                        #region 更新狀態
                        item.MFLAG = ENUM_I137_H_STATUS.SKIP.ExtValue();
                        mesdb.Updateable(item).ExecuteCommand();
                        trackobj.ReleaseFuncExcption();
                        #endregion
                        continue;
                    }
                    #endregion

                    #region 處理重複的I
                    if (heads.POCHANGEINDICATOR == Juniper137PoType.New.ExtValue() &&
                        mesdb.Queryable<I137_H, I137_I>((h, i) => h.TRANID == i.TRANID).Where((h, i) => h.PONUMBER == item.PONUMBER && h.POCHANGEINDICATOR == Juniper137PoType.New.ExtValue() && h.TRANID != item.TRANID && i.ITEM == item.ITEM
                              && (i.MFLAG == ENUM_I137_H_STATUS.CHECK_PASS.ExtValue() || i.MFLAG == ENUM_I137_H_STATUS.RELEASE.ExtValue() || i.MFLAG == ENUM_I137_H_STATUS.CHECK_FAIL.ExtValue())).Select((h, i) => i).Any())
                    {
                        #region 更新狀態
                        item.MFLAG = ENUM_I137_H_STATUS.SKIP.ExtValue();
                        mesdb.Updateable(item).ExecuteCommand();
                        trackobj.ReleaseFuncExcption();
                        #endregion
                        continue;
                    }
                    #endregion

                    #region pre asn check..
                    var mainorder = mesdb.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == item.PONUMBER && t.POLINE == item.ITEM).ToList().FirstOrDefault();
                    var checkpreasnres = trackobj.ExceptionProcess(
                        mainorder != null && mainorder.PREASN != ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue(),
                        $@"Order: {item.PONUMBER} {item.ITEM} already sent Pre-Ship ASN  ,FileName:{item.FILENAME}!", () => { checkres = false; });
                    if (checkpreasnres && mainorder.FINALASN != ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue())
                    {
                        item.MFLAG = ENUM_I137_H_STATUS.CHECK_FAIL_CLOSED.ExtValue();
                        mesdb.Updateable(item).ExecuteCommand();
                        trackobj.ReleaseFuncExcption();
                        continue;
                    }
                    #endregion

                    //#region check agile data --move to behind
                    //var ItemPid = string.IsNullOrEmpty(item.MATERIALID) || item.MATERIALID == "" ? item.PN : item.MATERIALID;
                    //var parenagiledata = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == ItemPid).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                    //var agiledata = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == item.PN).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                    //if (!item.PN.StartsWith("740-"))
                    //{
                    //    trackobj.ExceptionProcess(parenagiledata == null || string.IsNullOrEmpty(parenagiledata.USER_ITEM_TYPE) || string.IsNullOrEmpty(parenagiledata.OFFERING_TYPE), $@"TranId:{item.TRANID} ItemPid:{ItemPid} miss Agile Data,pls check!", () => { checkres = false; });
                    //    trackobj.ExceptionProcess(agiledata == null || string.IsNullOrEmpty(agiledata.USER_ITEM_TYPE) || string.IsNullOrEmpty(agiledata.OFFERING_TYPE), $@"TranId:{item.TRANID} ItemPid:{item.PN} miss Agile Data,pls check!", () => { checkres = false; });
                    //}
                    //#endregion
                    #region col blank check..
                    if (heads != null)
                        foreach (PropertyInfo info in heads.GetType().GetProperties())
                            trackobj.ExceptionProcess(
                                valid137headStr.Contains(info.Name)
                                && ((heads.GetType().GetProperty(info.Name).GetValue(heads, null) == null
                                || heads.GetType().GetProperty(info.Name).GetValue(heads, null).ToString().Equals(""))),
                                 $@"137head Validate Mandatory Field is Blank or Not: {info.Name} ,FileName:{item.FILENAME} ,pls check!", () => { checkres = false; }, false);
                    if (items != null)
                        foreach (PropertyInfo info in items.GetType().GetProperties())
                            trackobj.ExceptionProcess(
                                valid137itemStr.Contains(info.Name)
                                && ((items.GetType().GetProperty(info.Name).GetValue(items, null) == null
                                || items.GetType().GetProperty(info.Name).GetValue(items, null).ToString().Equals(""))),
                                $@"137Item Validate Mandatory Field is Blank or Not: {info.Name} ,FileName:{item.FILENAME} ,pls check!", () => { checkres = false; }, false);
                    foreach (var ditem in detail)
                        foreach (PropertyInfo info in ditem.GetType().GetProperties())
                            trackobj.ExceptionProcess(
                                valid137detailStr.Contains(info.Name)
                                && ((ditem.GetType().GetProperty(info.Name).GetValue(ditem, null) == null ||
                                ditem.GetType().GetProperty(info.Name).GetValue(ditem, null).ToString().Equals(""))),
                                @"137Detail Validate Mandatory Field is Blank or Not: {info.Name} ,FileName:{item.FILENAME} ,pls check!", () => { checkres = false; }, false);
                    #endregion

                    #region hb map check

                    #endregion

                    #region check sys havn't option 
                    #endregion

                    //#region lable check ---move to behind
                    //if (!item.PN.StartsWith("740-"))
                    //{
                    //    // country speci label
                    //    if (!string.IsNullOrEmpty(item.COUNTRYSPECIFICLABEL) && !item.COUNTRYSPECIFICLABEL.Equals("NA"))
                    //        trackobj.ExceptionProcess(
                    //            !mesdb.Queryable<O_137_COO_LABEL>().Where(t => t.COOVALUE == item.COUNTRYSPECIFICLABEL.Trim()).Any(),
                    //            $@" CountrySpeciLable havn't config label partno!  CountrySpeciLable: {item.COUNTRYSPECIFICLABEL.Trim()} ,pls check!", () => { checkres = false; });
                    //    // country speci label
                    //    if (!string.IsNullOrEmpty(item.CARTONLABEL1))
                    //        trackobj.ExceptionProcess(
                    //            !mesdb.Queryable<O_137CARTON_LABEL>().Where(t => t.SPECVAL == item.CARTONLABEL1.Trim() && t.SKUNO == agiledata.CUSTPARTNO).Any(),
                    //            $@" CartonLable havn't config label partno!  CartonLable: {item.CARTONLABEL1.Trim()},Skuno: {agiledata.CUSTPARTNO} ,pls check!", () => { checkres = false; });
                    //}
                    //#endregion

                    #region U不在I之後Err  收到的第一条不是I更新为I;      
                    trackobj.ExceptionProcess(
                        heads.POCHANGEINDICATOR.Equals(Juniper137PoType.Change.ExtValue()) &&
                        mesdb.Queryable<I137_H>().Where(t => t.PONUMBER == item.PONUMBER
                        && t.POCHANGEINDICATOR == Juniper137PoType.New.ExtValue()).ToList().Count == 0,
                        $@"TranId:{item.TRANID} Order is not exists,can't change,FileName:{item.FILENAME},pls check!", () =>
                        {
                            var firstobjh = mesdb.Queryable<I137_H>().Where(t => t.PONUMBER == item.PONUMBER).OrderBy(t => t.LASTCHANGEDATETIME, OrderByType.Asc).ToList().FirstOrDefault();
                            firstobjh.POCHANGEINDICATOR = Juniper137PoType.New.ExtValue();
                            mesdb.Updateable(firstobjh).ExecuteCommand();
                            checkres = false;
                        });
                    #endregion

                    #region 只能有一個I
                    trackobj.ExceptionProcess(
                        heads.POCHANGEINDICATOR.Equals(Juniper137PoType.New.ExtValue()) && mesdb.Queryable<I137_I, I137_H>((i, h) => i.TRANID == h.TRANID).Where((i, h) => i.PONUMBER == item.PONUMBER &&
                        i.ITEM == item.ITEM && i.MFLAG == ENUM_I137_H_STATUS.CHECK_PASS.ExtValue() &&
                        h.POCHANGEINDICATOR == Juniper137PoType.New.ExtValue()).Select((i, h) => i).ToList().Count == 1,
                         $@"TranId:{item.TRANID} Order number is exists,Can't new one ,FileName:{item.FILENAME},pls check!", () => { checkres = false; });
                    #endregion

                    #region 更新狀態
                    if (checkres)
                        trackobj.ReleaseFuncExcption();
                    item.MFLAG = checkres ? ENUM_I137_H_STATUS.CHECK_PASS.ExtValue() : ENUM_I137_H_STATUS.CHECK_FAIL.ExtValue();
                    mesdb.Updateable(item).ExecuteCommand();
                    #endregion
                }
            }
        }

        void NewOrderProcess()
        {
            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
            {
                var waitsynlist = mesdb.Queryable<I137_I, I137_H>((i, h) => i.TRANID == h.TRANID)
                    .Where((i, h) => i.MFLAG == ENUM_I137_H_STATUS.CHECK_PASS.Ext<EnumValueAttribute>().Description
                        && h.POCHANGEINDICATOR == Juniper137PoType.New.Ext<EnumValueAttribute>().Description).OrderBy((i, h) => i.LASTCHANGEDATETIME, OrderByType.Asc).Select((i, h) => i).ToList();

                foreach (var item in waitsynlist)
                {
                    JuniperPoTracking trackobj = new JuniperPoTracking(JuniperErrType.I137, JuniperSubType.NewOrderProcess, item.PONUMBER, item.ITEM, item.TRANID, mesdb);
                    try
                    {
                        var I137head = mesdb.Queryable<I137_H>().Where(t => t.TRANID == item.TRANID).ToList().FirstOrDefault();
                        var I137items = mesdb.Queryable<I137_I>().Where(t => t.TRANID == item.TRANID).ToList();
                        var I137detaillist = mesdb.Queryable<I137_D>().Where(t => t.TRANID == item.TRANID && t.ITEM == item.ITEM).ToList();
                        var poversion = 0;
                        var plant = I137head.VENDORID == JuniperB2BPlantCode.FJZ.Ext<EnumValueAttribute>().Description ? JuniperB2BPlantCode.FJZ.ToString() : JuniperB2BPlantCode.FVN.ToString();
                        var ordermainId = MesDbBase.GetNewID<O_ORDER_MAIN>(mesdb, Customer.JUNIPER.ExtValue());
                        var ItemPid = string.IsNullOrEmpty(item.MATERIALID) || item.MATERIALID == "" ? item.PN : item.MATERIALID;
                        var parenagiledata = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == ItemPid).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                        var agiledata = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == item.PN).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                        ////此處需要記錄err code
                        //trackobj.ExceptionProcess(parenagiledata == null, $@"ItemPid:{ItemPid} miss Agile Data,pls check!");
                        //trackobj.ExceptionProcess(agiledata == null, $@"ItemPid:{item.PN} miss Agile Data,pls check!");
                        var res = mesdb.Ado.UseTran(() =>
                        {
                            var options = new List<O_ORDER_OPTION>();
                            #region ITEM 的層級關係
                            var subitemoption = I137items.Where(t => t.SOID == item.SALESORDERLINEITEM && SqlFunc.StartsWith(t.PN, "740-")).ToList().FirstOrDefault();
                            if (subitemoption != null)
                                //Option part1
                                options.Add(new O_ORDER_OPTION()
                                {
                                    ID = MesDbBase.GetNewID<O_ORDER_OPTION>(mesdb, Customer.JUNIPER.ExtValue()),
                                    MAINID = ordermainId,
                                    PARTNO = subitemoption.PN,
                                    QTY = Convert.ToDouble(subitemoption.BASEQUANTITY) / Convert.ToDouble(subitemoption.SOQTY),
                                    PITEMID = subitemoption.SOID,
                                    CITEMID = subitemoption.SALESORDERLINEITEM,
                                    OPTIONTYPE = ENUM_O_ORDER_OPTION.POWERCODE.Ext<EnumValueAttribute>().Description,
                                    CREATETIME = DateTime.Now,
                                    FOXPN = new Func<string>(() =>
                                    {
                                        var comagile = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == subitemoption.PN).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                                        if (comagile != null)
                                            return comagile.ITEM_NUMBER;
                                        else
                                            return null;
                                        //throw new Exception($@"CustPn:{subitemoption.PN} miss Agile Data,pls check!");
                                    })()
                                });
                            #endregion
                            #region detail
                            foreach (var detialobj in I137detaillist)
                                //Option part2
                                options.Add(new O_ORDER_OPTION()
                                {
                                    ID = MesDbBase.GetNewID<O_ORDER_OPTION>(mesdb, Customer.JUNIPER.ExtValue()),
                                    MAINID = ordermainId,
                                    PARTNO = detialobj.COMPONENTID,
                                    QTY = Convert.ToDouble(detialobj.COMPONENTQTY) / Convert.ToDouble(item.QUANTITY),
                                    PITEMID = detialobj.SALESORDERLINEITEM,
                                    CITEMID = detialobj.COMSALESORDERLINEITEM,
                                    OPTIONTYPE = ENUM_O_ORDER_OPTION.CTO.Ext<EnumValueAttribute>().Description,
                                    CREATETIME = DateTime.Now,
                                    FOXPN = new Func<string>(() =>
                                    {
                                        var comagile = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == detialobj.COMPONENTID).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                                        if (comagile != null)
                                            return comagile.ITEM_NUMBER;
                                        else
                                            return null;
                                        //throw new Exception($@"CustPn:{detialobj.COMPONENTID} miss Agile Data,pls check!");
                                    })()
                                });
                            #endregion

                            #region add order main
                            var ordermain = new O_ORDER_MAIN()
                            {
                                ID = ordermainId,
                                UPOID = $@"{item.PONUMBER}{item.ITEM}",
                                PONO = item.PONUMBER,
                                POLINE = item.ITEM,
                                QTY = item.QUANTITY,
                                UNITPRICE = item.NETPRICE,
                                PID = agiledata?.ITEM_NUMBER,
                                CUSTPID = ItemPid,
                                DELIVERY = item.PODELIVERYDATE,
                                COMPLETED = ENUM_O_ORDER_MAIN.COMPLETED_NO.ExtValue(),
                                CLOSED = ENUM_O_ORDER_MAIN.CLOSED_NO.ExtValue(),
                                CANCEL = ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue(),
                                PREASN = ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue(),
                                FINALASN = ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue(),
                                CUSTOMER = Customer.JUNIPER.ExtValue(),
                                VERSION = poversion.ToString(),
                                POTYPE = new Func<string>(() =>
                                {
                                    return ConverOrderType(parenagiledata);
                                })(),
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now,
                                ITEMID = item.ID,
                                ORIGINALID = string.Empty,
                                USERITEMTYPE = parenagiledata?.USER_ITEM_TYPE,
                                OFFERINGTYPE = parenagiledata?.OFFERING_TYPE,
                                POCREATETIME = I137head.SHMENTORDERCREATIONDATE,
                                PLANT = plant,
                                LASTCHANGETIME = item.LASTCHANGEDATETIME.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                ORDERTYPE = I137head.PODOCTYPE,
                                RMQPONO = I137head.PODOCTYPE == ENUM_I137_PoDocType.ZRMQ.ToString() ? "" : I137head.RMQPONUMBER
                            };

                            //rmq record no --when change RMQPONUMBER?
                            if (I137head.PONUMBER != I137head.RMQPONUMBER && I137head.RMQPONUMBER != "NA" && I137head.PODOCTYPE != ENUM_I137_PoDocType.ZRMQ.ToString())
                            {
                                var rmqobj = mesdb.Queryable<O_I137_HEAD>().Where(t => t.PONUMBER == I137head.RMQPONUMBER && t.PODOCTYPE == ENUM_I137_PoDocType.ZRMQ.ToString()).ToList().FirstOrDefault();
                                if (rmqobj != null)
                                    mesdb.Updateable<O_ORDER_MAIN>().SetColumns(t => new O_ORDER_MAIN() { RMQPONO = I137head.PONUMBER }).Where(t => t.PONO == rmqobj.PONUMBER).ExecuteCommand();
                            }

                            mesdb.Insertable(ordermain).ExecuteCommand();
                            mesdb.Insertable(options).ExecuteCommand();
                            #endregion

                            #region hold
                            var holdinfo = Getholdinfo(item);
                            if (!string.IsNullOrEmpty(holdinfo))
                            {
                                mesdb.Insertable(new O_ORDER_HOLD()
                                {
                                    ID = MesDbBase.GetNewID<O_ORDER_HOLD>(mesdb, Customer.JUNIPER.ExtValue()),
                                    ITEMID = item.ID,
                                    UPOID = $@"{item.PONUMBER}{item.ITEM}",
                                    HOLDFLAG = MesBool.Yes.ExtValue(),
                                    HOLDREASON = holdinfo,
                                    CREATETIME = DateTime.Now,
                                    EDITTIME = DateTime.Now
                                }).ExecuteCommand();
                            }
                            #endregion

                            #region PO status
                            var cpostatus = new O_PO_STATUS()
                            {
                                ID = MesDbBase.GetNewID<O_PO_STATUS>(mesdb, Customer.JUNIPER.ExtValue()),
                                STATUSID = new Func<string>(() =>
                                {
                                    if (item.PN.StartsWith("740-"))
                                        return ENUM_O_PO_STATUS.NotProduce.ExtValue();
                                    else
                                        return ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue();
                                })(),
                                VALIDFLAG = MesBool.Yes.ExtValue(),
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now,
                                POID = ordermain.ID
                            };
                            mesdb.Insertable(cpostatus).ExecuteCommand();
                            #endregion

                            #region 更新狀態
                            item.MFLAG = ENUM_I137_H_STATUS.RELEASE.ExtValue();
                            mesdb.Updateable(item).ExecuteCommand();
                            #endregion
                        });
                        if (!res.IsSuccess)
                            throw res.ErrorException;
                        trackobj.ReleaseAllExcption();
                    }
                    catch (Exception e)
                    {
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }

        void ChangeOrderProcess()
        {
            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
            {
                var waitsynlist = mesdb.Queryable<I137_I, I137_H>((i, h) => i.TRANID == h.TRANID)
                    .Where((i, h) => i.MFLAG == ENUM_I137_H_STATUS.CHECK_PASS.Ext<EnumValueAttribute>().Description
                        && h.POCHANGEINDICATOR != Juniper137PoType.New.Ext<EnumValueAttribute>().Description).OrderBy((i, h) => i.LASTCHANGEDATETIME, OrderByType.Desc).Select((i, h) => i).ToList();
                foreach (var item in waitsynlist)
                {
                    JuniperPoTracking trackobj = new JuniperPoTracking(JuniperErrType.I137, JuniperSubType.ChangeOrderProcess, item.PONUMBER, item.ITEM, item.TRANID, mesdb);
                    try
                    {
                        var I137head = mesdb.Queryable<I137_H>().Where(t => t.TRANID == item.TRANID).ToList().FirstOrDefault();
                        var I137items = mesdb.Queryable<I137_I>().Where(t => t.TRANID == item.TRANID).ToList();
                        var I137detaillist = mesdb.Queryable<I137_D>().Where(t => t.TRANID == item.TRANID && t.ITEM == item.ITEM).ToList();
                        var poversion = I137head.VERSION;
                        var plant = I137head.VENDORID == JuniperB2BPlantCode.FJZ.Ext<EnumValueAttribute>().Description ? JuniperB2BPlantCode.FJZ.ToString() : JuniperB2BPlantCode.FVN.ToString();
                        var ItemPid = string.IsNullOrEmpty(item.MATERIALID) || item.MATERIALID == "" ? item.PN : item.MATERIALID;
                        var parenagiledata = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == ItemPid).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                        var agiledata = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == item.PN).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                        ////此處需要記錄err code
                        //trackobj.ExceptionProcess(parenagiledata == null, $@"ItemPid:{ItemPid} miss Agile Data,pls check!");
                        //trackobj.ExceptionProcess(agiledata == null, $@"ItemPid:{item.PN} miss Agile Data,pls check!");
                        var ordermaincurrent = mesdb.Queryable<O_ORDER_MAIN>().Where(t => t.UPOID == $@"{item.PONUMBER}{item.ITEM}").ToList().FirstOrDefault();
                        if (trackobj.ExceptionProcess(ordermaincurrent == null, "Change is waiting for the processing of new orders!"))
                            continue;
                        var ordermaincurrentI137item = mesdb.Queryable<I137_I>().Where(t => t.ID == ordermaincurrent.ITEMID).ToList().FirstOrDefault();
                        #region 低於當前版本的不處理;
                        string ts = ordermaincurrent.LASTCHANGETIME;
                        DateTime time;
                        if (ts.Contains("上午"))
                        {
                            ts = ts.Replace("上午", "");
                            time = DateTime.Parse(ts);

                        }
                        else if (ts.Contains("下午"))
                        {
                            ts = ts.Replace("下午", "");
                            time = DateTime.Parse(ts);
                            time = time.AddHours(12);
                        }
                        else if (ts.Contains("PM"))
                        {
                            ts = ts.Replace("PM", "");
                            time = DateTime.Parse(ts);
                            time = time.AddHours(12);
                        }
                        else if (ts.Contains("AM"))
                        {
                            ts = ts.Replace("AM", "");
                            time = DateTime.Parse(ts);

                        }
                        else
                        {
                            time = DateTime.Parse(ts);
                        }


                        if (time > item.LASTCHANGEDATETIME)
                        {
                            var dbres = mesdb.Ado.UseTran(() =>
                            {
                                mesdb.Insertable(new O_ORDER_CHANGELOG()
                                {
                                    ID = MesDbBase.GetNewID<O_ORDER_CHANGELOG>(mesdb, Customer.JUNIPER.ExtValue()),
                                    UPOID = ordermaincurrent.UPOID,
                                    MAINID = ordermaincurrent.ID,
                                    CHANGEITEMID = item.ID.ToString(),
                                    SOURCEITEMID = ordermaincurrent.ITEMID,
                                    CHANGETYPE = Juniper137PoType.Change.ExtValue(),
                                    CURRENTREVSION = item.ITEMCHANGEINDICATOR,
                                    VERSIONLOG = poversion.ToString(),
                                    CREATETIME = DateTime.Now
                                }).ExecuteCommand();
                                #region 更新狀態
                                item.MFLAG = ENUM_I137_H_STATUS.SKIP.ExtValue();
                                mesdb.Updateable(item).ExecuteCommand();
                                #endregion
                            });
                            if (!dbres.IsSuccess)
                                throw dbres.ErrorException;
                            trackobj.ReleaseFuncExcption();
                            continue;
                        }
                        #endregion
                        var changeres = new DbResult<bool>();
                        //var I137type = item.ITEMCHANGEINDICATOR.Equals(Juniper137PoType.New.ExtValue()) ? Juniper137PoType.New : Juniper137PoType.Change;
                        var I137type = Juniper137PoType.Change;
                        if (item.ACTIONCODE != null && item.ACTIONCODE.Equals(ENUM_I137_Actioncode_Type.Cancel.ExtValue()))
                            I137type = Juniper137PoType.Change;
                        switch (I137type)
                        {
                            case Juniper137PoType.New:
                                changeres = BaseChangeOrder(mesdb, ordermaincurrent, item, poversion);
                                break;
                            case Juniper137PoType.Change:
                                {
                                    var ordermainId = MesDbBase.GetNewID<O_ORDER_MAIN>(mesdb, Customer.JUNIPER.ExtValue());
                                    var optionlist = new List<O_ORDER_OPTION>();
                                    #region Option
                                    #region ITEM 的層級關係
                                    var subitemoption = I137items.Where(t => t.SOID == item.SALESORDERLINEITEM && SqlFunc.StartsWith(t.PN, "740-")).ToList().FirstOrDefault();
                                    if (subitemoption != null)
                                        //Option part1
                                        optionlist.Add(new O_ORDER_OPTION()
                                        {
                                            ID = MesDbBase.GetNewID<O_ORDER_OPTION>(mesdb, Customer.JUNIPER.ExtValue()),
                                            MAINID = ordermainId,
                                            PARTNO = subitemoption.PN,
                                            QTY = Convert.ToDouble(subitemoption.BASEQUANTITY) / Convert.ToDouble(subitemoption.SOQTY),
                                            PITEMID = subitemoption.SOID,
                                            CITEMID = subitemoption.SALESORDERLINEITEM,
                                            OPTIONTYPE = ENUM_O_ORDER_OPTION.POWERCODE.Ext<EnumValueAttribute>().Description,
                                            CREATETIME = DateTime.Now,
                                            FOXPN = new Func<string>(() =>
                                            {
                                                var comagile = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == subitemoption.PN).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                                                if (comagile != null)
                                                    return comagile.ITEM_NUMBER;
                                                else
                                                    return null;
                                                //throw new Exception($@"CustPn:{subitemoption.PN} miss Agile Data,pls check!");
                                            })()
                                        });
                                    #endregion
                                    #region detail
                                    foreach (var detialobj in I137detaillist)
                                    {
                                        //Option part2
                                        optionlist.Add(new O_ORDER_OPTION()
                                        {
                                            ID = MesDbBase.GetNewID<O_ORDER_OPTION>(mesdb, Customer.JUNIPER.ExtValue()),
                                            MAINID = ordermainId,
                                            PARTNO = detialobj.COMPONENTID,
                                            QTY = Convert.ToDouble(detialobj.COMPONENTQTY) / Convert.ToDouble(item.QUANTITY),
                                            PITEMID = detialobj.SALESORDERLINEITEM,
                                            CITEMID = detialobj.COMSALESORDERLINEITEM,
                                            OPTIONTYPE = ENUM_O_ORDER_OPTION.CTO.Ext<EnumValueAttribute>().Description,
                                            CREATETIME = DateTime.Now,
                                            FOXPN = new Func<string>(() =>
                                            {
                                                var comagile = mesdb.Queryable<O_AGILE_ATTR>().Where(t => t.CUSTPARTNO == detialobj.COMPONENTID).OrderBy(t => t.DATE_CREATED, OrderByType.Desc).ToList().FirstOrDefault();
                                                if (comagile != null)
                                                    return comagile.ITEM_NUMBER;
                                                else
                                                    return null;
                                                //throw new Exception($@"CustPn:{detialobj.COMPONENTID} miss Agile Data,pls check!");
                                            })()
                                        });
                                    }
                                    #endregion
                                    #endregion
                                    var changetype = new Func<JuniperOrderChangeType>(() =>
                                    {
                                        if (item.ACTIONCODE != null && item.ACTIONCODE.Equals(ENUM_I137_Actioncode_Type.Cancel.ExtValue()))
                                            return JuniperOrderChangeType.Cancel;
                                        if (item.QUANTITY != ordermaincurrent.QTY)
                                            return JuniperOrderChangeType.QtyBomChange;
                                        #region 還需要考慮POWERCODE跟BUDEL的Change情況
                                        var currentOption = mesdb.Queryable<O_ORDER_OPTION>().Where(t => t.MAINID == ordermaincurrent.ID).ToList();
                                        //foreach (var currentoptionitem in currentOption)
                                        //    if (!optionlist.FindAll(t => t.PARTNO == currentoptionitem.PARTNO && t.QTY == currentoptionitem.QTY && t.PITEMID == currentoptionitem.PITEMID && t.OPTIONTYPE == currentoptionitem.OPTIONTYPE && t.CITEMID == currentoptionitem.CITEMID).Any())
                                        //        return JuniperOrderChangeType.QtyBomChange;
                                        if (currentOption.Count() != optionlist.Count())
                                            return JuniperOrderChangeType.QtyBomChange;
                                        foreach (var newoptionitem in optionlist)
                                            if (!currentOption.FindAll(t => t.PARTNO == newoptionitem.PARTNO && t.QTY == newoptionitem.QTY && t.PITEMID == newoptionitem.PITEMID && t.OPTIONTYPE == newoptionitem.OPTIONTYPE && t.CITEMID == newoptionitem.CITEMID).Any())
                                                return JuniperOrderChangeType.QtyBomChange;
                                        #endregion
                                        #region package change/
                                        if ((string.IsNullOrEmpty(ordermaincurrentI137item.CARTONLABEL2) && !string.IsNullOrEmpty(item.CARTONLABEL2)) ||
                                        (!string.IsNullOrEmpty(ordermaincurrentI137item.CARTONLABEL2) && string.IsNullOrEmpty(item.CARTONLABEL2)))
                                            return JuniperOrderChangeType.QtyBomChange;
                                        if (ordermaincurrentI137item.CARTONLABEL1 != item.CARTONLABEL1)
                                            return JuniperOrderChangeType.QtyBomChange;
                                        #endregion

                                        return JuniperOrderChangeType.BaseChange;
                                    })();
                                    switch (changetype)
                                    {
                                        case JuniperOrderChangeType.BaseChange:
                                            changeres = BaseChangeOrder(mesdb, ordermaincurrent, item, poversion);
                                            break;
                                        case JuniperOrderChangeType.QtyBomChange: //QtyChange
                                            if (item.PN.StartsWith("740-"))
                                                changeres = PowerCodeChangeOrder(mesdb, ordermaincurrent, item, I137head, poversion, ordermainId, plant);
                                            else
                                                changeres = QtyChangeOrder(mesdb, ordermaincurrent, item, I137head, agiledata, parenagiledata, optionlist, plant, poversion, ordermainId, ItemPid);
                                            break;
                                        case JuniperOrderChangeType.Cancel:
                                            changeres = CancleOrder(mesdb, ordermaincurrent, item, poversion);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        if (changeres == null || !changeres.IsSuccess)
                            throw changeres.ErrorException;
                        JuniperBase.ReleaseJuniperExcption(mesdb, $@"{item.PONUMBER}{item.ITEM}", item.TRANID);
                    }
                    catch (Exception e)
                    {
                        trackobj.ExceptionProcess(true, e.Message);
                    }
                }
            }
        }

        DbResult<bool> BaseChangeOrder(SqlSugarClient mesdb, O_ORDER_MAIN ordermaincurrent, I137_I item, string poversion)
        {
            return mesdb.Ado.UseTran(() =>
            {
                #region Update order main
                mesdb.Insertable(new O_ORDER_CHANGELOG()
                {
                    ID = MesDbBase.GetNewID<O_ORDER_CHANGELOG>(mesdb, Customer.JUNIPER.ExtValue()),
                    UPOID = ordermaincurrent.UPOID,
                    MAINID = ordermaincurrent.ID,
                    CHANGEITEMID = item.ID.ToString(),
                    SOURCEITEMID = ordermaincurrent.ITEMID,
                    CHANGETYPE = Juniper137PoType.Change.ExtValue(),
                    CURRENTREVSION = item.LASTCHANGEDATETIME.ToString(),
                    VERSIONLOG = poversion.ToString(),
                    CREATETIME = DateTime.Now
                }).ExecuteCommand();

                ordermaincurrent.ORIGINALITEMID = string.IsNullOrEmpty(ordermaincurrent.ORIGINALITEMID) ? ordermaincurrent.ITEMID : ordermaincurrent.ORIGINALITEMID;
                ordermaincurrent.ITEMID = item.ID.ToString();
                ordermaincurrent.EDITTIME = DateTime.Now;
                ordermaincurrent.DELIVERY = item.PODELIVERYDATE;
                ordermaincurrent.VERSION = poversion;
                ordermaincurrent.LASTCHANGETIME = item.LASTCHANGEDATETIME.Value.ToString("yyyy-MM-dd HH:mm:ss");
                mesdb.Updateable(ordermaincurrent).ExecuteCommand();

                #region wo 未生產可以update startdate,WIP can't --add by Eden 20210616
                var postatus = mesdb.Queryable<O_PO_STATUS>().Where(t => t.POID == ordermaincurrent.ID && t.VALIDFLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                if (JuniperBase.CheckCrtdStatusWithOrder(mesdb, ordermaincurrent, postatus))
                {
                    var sapwostartdate = new Func<string>(() =>
                    {
                        var chinaZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                        // 机器本地时间 -> 中国时间
                        var chinaTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, chinaZone);
                        //var startdate = (Convert.ToDateTime(item.CUSTREQSHIPDATE).Date - chinaTime.Date).Days <= 7 ? chinaTime.AddDays(1) : Convert.ToDateTime(item.CUSTREQSHIPDATE);
                        //var startdate = Convert.ToDateTime(item.CUSTREQSHIPDATE).AddDays(-7); //Donald hui要求FVN、FJZ都改為CRSD減7天
                        //var nowtime = Convert.ToDateTime(mesdb.GetDate().ToString("yyyy-MM-dd 00:00:00"));
                        var startdate = (Convert.ToDateTime(item.CUSTREQSHIPDATE).Date - chinaTime.Date).Days <= 7 ? chinaTime.AddDays(1) : Convert.ToDateTime(item.CUSTREQSHIPDATE).AddDays(-7); //時間不能傳過去的時間，SAP會回only WO scheudling type is 3 that startdate can in past
                        return startdate.ToString("yyyy-MM-dd");
                    })();
                    mesdb.Updateable<R_SAP_JOB>().SetColumns(t => new R_SAP_JOB() { JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Closed.ExtValue(),LASTEDITTIME = DateTime.Now.ToString() })
                   .Where(t => t.JOBNAME == ENUM_R_SAP_JOB_FUNCTION.ChangeCrsdWithSap.ExtValue() && t.JOBKEY == ordermaincurrent.UPOID && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue()).ExecuteCommand();
                    mesdb.Insertable(new R_SAP_JOB()
                    {
                        ID = MesDbBase.GetNewID<R_SAP_JOB>(mesdb, Customer.JUNIPER.ExtValue()),
                        JOBKEY = ordermaincurrent.UPOID,
                        JOBNAME = ENUM_R_SAP_JOB_FUNCTION.ChangeCrsdWithSap.ExtValue(),
                        JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue(),
                        CREATETIME = DateTime.Now,
                        DATA1 = ordermaincurrent.PLANT,
                        DATA2 = ordermaincurrent.PREWO,
                        DATA3 = sapwostartdate
                    }).ExecuteCommand();
                    //var updatesapres = JuniperBase.ChangeCrsdWithSap(this.bu, ordermaincurrent.PREWO, sapwostartdate);
                    //if (!updatesapres.issuccess)
                    //    throw new Exception(updatesapres.msg);
                }
                #endregion
                #region hold
                var holdinfo = Getholdinfo(item);
                if (!string.IsNullOrEmpty(holdinfo))
                {
                    mesdb.Insertable(new O_ORDER_HOLD()
                    {
                        ID = MesDbBase.GetNewID<O_ORDER_HOLD>(mesdb, Customer.JUNIPER.ExtValue()),
                        ITEMID = item.ID,
                        UPOID = $@"{item.PONUMBER}{item.ITEM}",
                        HOLDFLAG = MesBool.Yes.ExtValue(),
                        HOLDREASON = holdinfo,
                        CREATETIME = DateTime.Now,
                        EDITTIME = DateTime.Now
                    }).ExecuteCommand();
                    var holdobj = JuniperOmBase.JuniperHoldCheck(ordermaincurrent.ID, ENUM_O_ORDER_HOLD_CONTROLTYPE.PRODUCTION, mesdb);
                    if (ordermaincurrent.PREWO != null && ordermaincurrent.PREWO.Length > 0 && holdobj.HoldFlag)
                        mesdb.Insertable(new R_SN_LOCK()
                        {
                            ID = MesDbBase.GetNewID<R_SN_LOCK>(mesdb, Customer.JUNIPER.ExtValue()),
                            WORKORDERNO = ordermaincurrent.PREWO,
                            LOCK_STATION = "ALL",
                            TYPE = "WO",
                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                            LOCK_REASON = $@"PO is Hold! HoldReason: {holdinfo},pls check!",
                            LOCK_TIME = DateTime.Now,
                            LOCK_EMP = "JNPCUST"
                        }).ExecuteCommand();
                }
                else
                    mesdb.Updateable<R_SN_LOCK>().SetColumns(t => new R_SN_LOCK() { LOCK_STATUS = MesBool.No.ExtValue(), UNLOCK_EMP = "JNPCUST", UNLOCK_REASON = "CustUnHold!" }).Where(t => t.WORKORDERNO == ordermaincurrent.PREWO && t.LOCK_EMP == "JNPCUST").ExecuteCommand();

                #endregion

                #region 更新狀態
                item.MFLAG = ENUM_I137_H_STATUS.RELEASE.ExtValue();
                mesdb.Updateable(item).ExecuteCommand();
                #endregion
                JuniperBase.ReleaseJuniperExcption(mesdb, $@"{item.PONUMBER}{item.ITEM}", item.TRANID);
                #endregion
            });
        }

        DbResult<bool> PowerCodeChangeOrder(SqlSugarClient mesdb, O_ORDER_MAIN ordermaincurrent, I137_I item, I137_H I137head, string poversion, string ordermainId, string plant)
        {
            return mesdb.Ado.UseTran(() =>
            {
                #region His
                var lastordermain = mesdb.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == item.PONUMBER && t.POLINE == item.ITEM).ToList().FirstOrDefault();
                var hisordermain = ObjectDataHelper.Mapper<O_ORDER_MAIN_H, O_ORDER_MAIN>(lastordermain);
                mesdb.Insertable(hisordermain).ExecuteCommand();
                mesdb.Deleteable(lastordermain).ExecuteCommand();
                #endregion
                #region add order main
                var ordermain = new O_ORDER_MAIN()
                {
                    ID = ordermainId,
                    UPOID = $@"{item.PONUMBER}{item.ITEM}",
                    PONO = item.PONUMBER,
                    POLINE = item.ITEM,
                    QTY = item.QUANTITY,
                    UNITPRICE = item.NETPRICE,
                    PID = item.PN,
                    CUSTPID = item.PN,
                    DELIVERY = item.PODELIVERYDATE,
                    COMPLETED = ENUM_O_ORDER_MAIN.COMPLETED_NO.ExtValue(),
                    CLOSED = ENUM_O_ORDER_MAIN.CLOSED_NO.ExtValue(),
                    CANCEL = ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue(),
                    PREASN = ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue(),
                    FINALASN = ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue(),
                    CUSTOMER = Customer.JUNIPER.ExtValue(),
                    VERSION = poversion.ToString(),
                    POTYPE = ENUM_O_ORDER_MAIN_POTYPE.BTS.ExtValue(),
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now,
                    ITEMID = item.ID,
                    ORIGINALID = lastordermain.ID,
                    POCREATETIME = I137head.SHMENTORDERCREATIONDATE,
                    PLANT = plant,
                    LASTCHANGETIME = item.LASTCHANGEDATETIME.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    ORDERTYPE = I137head.PODOCTYPE
                };
                mesdb.Insertable(ordermain).ExecuteCommand();
                #endregion
                #region hold
                var holdinfo = Getholdinfo(item);
                if (!string.IsNullOrEmpty(holdinfo))
                {
                    mesdb.Insertable(new O_ORDER_HOLD()
                    {
                        ID = MesDbBase.GetNewID<O_ORDER_HOLD>(mesdb, Customer.JUNIPER.ExtValue()),
                        ITEMID = item.ID,
                        UPOID = $@"{item.PONUMBER}{item.ITEM}",
                        HOLDFLAG = MesBool.Yes.ExtValue(),
                        HOLDREASON = holdinfo,
                        CREATETIME = DateTime.Now,
                        EDITTIME = DateTime.Now
                    }).ExecuteCommand();
                }
                #endregion
                #region PO status
                var cpostatus = new O_PO_STATUS()
                {
                    ID = MesDbBase.GetNewID<O_PO_STATUS>(mesdb, Customer.JUNIPER.ExtValue()),
                    STATUSID = ENUM_O_PO_STATUS.NotProduce.ExtValue(),
                    VALIDFLAG = MesBool.Yes.ExtValue(),
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now,
                    POID = ordermainId
                };
                mesdb.Insertable(cpostatus).ExecuteCommand();
                #endregion
                #region 更新狀態
                item.MFLAG = ENUM_I137_H_STATUS.RELEASE.ExtValue();
                mesdb.Updateable(item).ExecuteCommand();
                #endregion
                JuniperBase.ReleaseJuniperExcption(mesdb, $@"{item.PONUMBER}{item.ITEM}", item.TRANID);
            });
        }

        DbResult<bool> QtyChangeOrder(SqlSugarClient mesdb, O_ORDER_MAIN ordermaincurrent, I137_I item, I137_H I137head, O_AGILE_ATTR agiledata, O_AGILE_ATTR parenagiledata, List<O_ORDER_OPTION> optionlist, string plant, string poversion, string ordermainId, string ItemPid)
        {
            return mesdb.Ado.UseTran(() =>
            {
                #region His
                var lastordermain = mesdb.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == item.PONUMBER && t.POLINE == item.ITEM).ToList().FirstOrDefault();
                var hisordermain = ObjectDataHelper.Mapper<O_ORDER_MAIN_H, O_ORDER_MAIN>(lastordermain);
                mesdb.Insertable(hisordermain).ExecuteCommand();
                mesdb.Deleteable(lastordermain).ExecuteCommand();
                #endregion
                #region add order main
                var ordermain = new O_ORDER_MAIN()
                {
                    ID = ordermainId,
                    UPOID = $@"{item.PONUMBER}{item.ITEM}",
                    PONO = item.PONUMBER,
                    POLINE = item.ITEM,
                    QTY = item.QUANTITY,
                    UNITPRICE = item.NETPRICE,
                    PID = agiledata?.ITEM_NUMBER,
                    CUSTPID = ItemPid,
                    DELIVERY = item.PODELIVERYDATE,
                    COMPLETED = ENUM_O_ORDER_MAIN.COMPLETED_NO.ExtValue(),
                    CLOSED = ENUM_O_ORDER_MAIN.CLOSED_NO.ExtValue(),
                    CANCEL = ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue(),
                    PREASN = ENUM_O_ORDER_MAIN.PREASN_NO.ExtValue(),
                    FINALASN = ENUM_O_ORDER_MAIN.FINALASN_NO.ExtValue(),
                    CUSTOMER = Customer.JUNIPER.ExtValue(),
                    VERSION = poversion.ToString(),
                    POTYPE = new Func<string>(() =>
                    {
                        return ConverOrderType(parenagiledata);
                    })(),
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now,
                    ITEMID = item.ID,
                    ORIGINALID = lastordermain.ID,
                    USERITEMTYPE = parenagiledata?.USER_ITEM_TYPE,
                    OFFERINGTYPE = parenagiledata?.OFFERING_TYPE,
                    POCREATETIME = I137head.SHMENTORDERCREATIONDATE,
                    PLANT = plant,
                    LASTCHANGETIME = item.LASTCHANGEDATETIME.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    ORDERTYPE = I137head.PODOCTYPE,
                    RMQPONO = I137head.PODOCTYPE == ENUM_I137_PoDocType.ZRMQ.ToString() ? "" : I137head.RMQPONUMBER
                };
                mesdb.Insertable(ordermain).ExecuteCommand();
                #endregion
                #region add option
                mesdb.Insertable(optionlist).ExecuteCommand();
                #endregion
                #region hold
                var holdinfo = Getholdinfo(item);
                if (!string.IsNullOrEmpty(holdinfo))
                {
                    mesdb.Insertable(new O_ORDER_HOLD()
                    {
                        ID = MesDbBase.GetNewID<O_ORDER_HOLD>(mesdb, Customer.JUNIPER.ExtValue()),
                        ITEMID = item.ID,
                        UPOID = $@"{item.PONUMBER}{item.ITEM}",
                        HOLDFLAG = MesBool.Yes.ExtValue(),
                        HOLDREASON = holdinfo,
                        CREATETIME = DateTime.Now,
                        EDITTIME = DateTime.Now
                    }).ExecuteCommand();
                }
                if (lastordermain.PREWO != null && lastordermain.PREWO.Length > 0)
                    mesdb.Insertable(new R_SN_LOCK()
                    {
                        ID = MesDbBase.GetNewID<R_SN_LOCK>(mesdb, Customer.JUNIPER.ExtValue()),
                        WORKORDERNO = lastordermain.PREWO,
                        LOCK_STATION = "ALL",
                        TYPE = "WO",
                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                        LOCK_REASON = $@"Order is Change! pls check!",
                        LOCK_TIME = DateTime.Now,
                        LOCK_EMP = "JNPCUST"
                    }).ExecuteCommand();
                #endregion
                #region PO status
                var cpostatus = new O_PO_STATUS()
                {
                    ID = MesDbBase.GetNewID<O_PO_STATUS>(mesdb, Customer.JUNIPER.ExtValue()),
                    STATUSID = new Func<string>(() =>
                    {
                        if (item.PN.StartsWith("740-"))
                            return ENUM_O_PO_STATUS.NotProduce.ExtValue();
                        if (lastordermain.PREWO != null && lastordermain.PREWO.Length > 0)
                        {
                            var lastmainstatus = mesdb.Queryable<O_PO_STATUS>().Where(t => t.POID == lastordermain.ID && t.VALIDFLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                            //if(!(lastmainstatus.STATUSID == ENUM_O_PO_STATUS.AddNonBom.ExtValue()||lastmainstatus.STATUSID == ENUM_O_PO_STATUS.CreateWo.ExtValue() ||
                            //lastmainstatus.STATUSID == ENUM_O_PO_STATUS.OnePreUploadBom.ExtValue()||lastmainstatus.STATUSID == ENUM_O_PO_STATUS.ReceiveGroupId.ExtValue() ||
                            //lastmainstatus.STATUSID == ENUM_O_PO_STATUS.ValidationI137.ExtValue() ||lastmainstatus.STATUSID == ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue() ||
                            //lastmainstatus.STATUSID == ENUM_O_PO_STATUS.SecPreUploadBom.ExtValue()))
                            //    return ENUM_O_PO_STATUS.WaitDismantle.ExtValue();

                            #region wo 未生產可以TECO,WIP can't --add by Eden 20210616
                            if (JuniperBase.CheckCrtdStatusWithOrder(mesdb, lastordermain, lastmainstatus))
                            {
                                #region changetojob
                                mesdb.Insertable(new R_SAP_JOB()
                                {
                                    ID = MesDbBase.GetNewID<R_SAP_JOB>(mesdb, Customer.JUNIPER.ExtValue()),
                                    JOBKEY = lastordermain.ID,
                                    JOBNAME = ENUM_R_SAP_JOB_FUNCTION.TecoSapWoWithChange.ExtValue(),
                                    JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue(),
                                    CREATETIME = DateTime.Now,
                                    DATA1 = lastordermain.PLANT,
                                    DATA2 = lastordermain.PREWO
                                }).ExecuteCommand();
                                return ENUM_O_PO_STATUS.WaitDismantle.ExtValue();
                                #endregion
                                //var tecores = JuniperBase.TecoSapWo(this.bu, lastordermain.PREWO);
                                //if (tecores.issuccess)
                                //    return ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue();
                                //else if (!tecores.issuccess && lastmainstatus.STATUSID == ENUM_O_PO_STATUS.RmqEnd.ExtValue() && lastmainstatus.CREATETIME > Convert.ToDateTime("2021-06-22"))
                                //    return ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue();
                                //else
                                //    return ENUM_O_PO_STATUS.WaitDismantle.ExtValue();
                            }
                            #endregion
                            //if(lastordermain.ORDERTYPE == ENUM_I137_PoDocType.ZRMQ.ExtValue())
                            //    return ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue();
                            if (JnpConst.JnpProductingStatus.Contains(lastmainstatus.STATUSID))
                                return ENUM_O_PO_STATUS.WaitDismantle.ExtValue();
                        }
                        return ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue();
                    })(),
                    VALIDFLAG = MesBool.Yes.ExtValue(),
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now,
                    POID = ordermain.ID
                };
                mesdb.Insertable(cpostatus).ExecuteCommand();
                #endregion
                #region 更新狀態
                item.MFLAG = ENUM_I137_H_STATUS.RELEASE.ExtValue();
                mesdb.Updateable(item).ExecuteCommand();
                #endregion
                JuniperBase.ReleaseJuniperExcption(mesdb, $@"{item.PONUMBER}{item.ITEM}", item.TRANID);
            });
        }

        DbResult<bool> CancleOrder(SqlSugarClient mesdb, O_ORDER_MAIN ordermaincurrent, I137_I item, string poversion)
        {
            return mesdb.Ado.UseTran(() =>
            {
                if (!item.PN.StartsWith("740-"))
                {
                    #region PO status
                    var postatusobj = mesdb.Queryable<O_PO_STATUS>().Where(t => t.POID == ordermaincurrent.ID && t.VALIDFLAG == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                    mesdb.Updateable<O_PO_STATUS>().SetColumns(t => t.VALIDFLAG == MesBool.No.ExtValue()).Where(t => t.POID == ordermaincurrent.ID).ExecuteCommand();
                    mesdb.Insertable(new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(mesdb, Customer.JUNIPER.ExtValue()),
                        STATUSID = new Func<string>(() =>
                        {
                            if (ordermaincurrent.ORDERTYPE == ENUM_I137_PoDocType.ZRMQ.ExtValue() && postatusobj.CREATETIME < Convert.ToDateTime("2021-06-22"))
                                return ENUM_O_PO_STATUS.Closed.ExtValue();
                            var mainwo = mesdb.Queryable<R_ORDER_WO>().Where(t => t.UPOID == ordermaincurrent.UPOID && t.VALID == MesBool.Yes.ExtValue()).ToList().FirstOrDefault();
                            #region wo 未生產可以TECO,WIP can't --add by Eden 20210616
                            if (JuniperBase.CheckCrtdStatusWithOrder(mesdb, ordermaincurrent, postatusobj))
                            {
                                #region changetojob
                                mesdb.Updateable<R_SAP_JOB>().SetColumns(t => new R_SAP_JOB() { JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Closed.ExtValue(), LASTEDITTIME = DateTime.Now.ToString() })
                                .Where(t => t.JOBNAME == ENUM_R_SAP_JOB_FUNCTION.ChangeCrsdWithSap.ExtValue() && t.JOBKEY == ordermaincurrent.UPOID && t.JOBSTATUS == ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue()).ExecuteCommand();
                                mesdb.Insertable(new R_SAP_JOB()
                                {
                                    ID = MesDbBase.GetNewID<R_SAP_JOB>(mesdb, Customer.JUNIPER.ExtValue()),
                                    JOBKEY = ordermaincurrent.ID,
                                    JOBNAME = ENUM_R_SAP_JOB_FUNCTION.TecoSapWoWithCancel.ExtValue(),
                                    JOBSTATUS = ENUM_R_SAP_JOB_JOBSTATUS.Wait.ExtValue(),
                                    CREATETIME = DateTime.Now,
                                    DATA1 = ordermaincurrent.PLANT,
                                    DATA2 = ordermaincurrent.PREWO
                                }).ExecuteCommand();
                                return ENUM_O_PO_STATUS.WaitDismantle.ExtValue();
                                //var tecores = JuniperBase.TecoSapWo(ordermaincurrent.PLANT, ordermaincurrent.PREWO);
                                //if (tecores.issuccess)
                                //    return ENUM_O_PO_STATUS.Closed.ExtValue();
                                //else
                                //    return ENUM_O_PO_STATUS.WaitDismantle.ExtValue();
                                #endregion
                            }
                            if (JuniperBase.CheckNotProductStatusWithOrder(mesdb, ordermaincurrent))
                                return ENUM_O_PO_STATUS.Closed.ExtValue();
                            #endregion
                            if (mainwo != null || JnpConst.JnpProductingStatus.Contains(postatusobj.STATUSID))
                                return ENUM_O_PO_STATUS.WaitDismantle.ExtValue();
                            return ENUM_O_PO_STATUS.Closed.ExtValue();
                        })(),
                        VALIDFLAG = MesBool.Yes.ExtValue(),
                        CREATETIME = DateTime.Now,
                        EDITTIME = DateTime.Now,
                        POID = ordermaincurrent.ID
                    }).ExecuteCommand();

                    if (ordermaincurrent.PREWO != null && ordermaincurrent.PREWO.Length > 0)
                        mesdb.Insertable(new R_SN_LOCK()
                        {
                            ID = MesDbBase.GetNewID<R_SN_LOCK>(mesdb, Customer.JUNIPER.ExtValue()),
                            WORKORDERNO = ordermaincurrent.PREWO,
                            LOCK_STATION = "ALL",
                            TYPE = "WO",
                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                            LOCK_REASON = $@"Order is Cancel!,pls check!",
                            LOCK_TIME = DateTime.Now,
                            LOCK_EMP = "JNPCUST"
                        }).ExecuteCommand();
                    #endregion
                }
                #region Update order main
                mesdb.Insertable(new O_ORDER_CHANGELOG()
                {
                    ID = MesDbBase.GetNewID<O_ORDER_CHANGELOG>(mesdb, Customer.JUNIPER.ExtValue()),
                    UPOID = ordermaincurrent.UPOID,
                    MAINID = ordermaincurrent.ID,
                    CHANGEITEMID = item.ID.ToString(),
                    SOURCEITEMID = ordermaincurrent.ITEMID,
                    CHANGETYPE = Juniper137PoType.Change.ExtValue(),
                    CURRENTREVSION = item.LASTCHANGEDATETIME.ToString(),
                    VERSIONLOG = poversion.ToString(),
                    CREATETIME = DateTime.Now
                }).ExecuteCommand();

                ordermaincurrent.ORIGINALITEMID = string.IsNullOrEmpty(ordermaincurrent.ORIGINALITEMID) ? ordermaincurrent.ITEMID : ordermaincurrent.ORIGINALITEMID;
                ordermaincurrent.ITEMID = item.ID.ToString();
                ordermaincurrent.CANCEL = ENUM_O_ORDER_MAIN.CANCEL_WAIT.ExtValue();
                ordermaincurrent.VERSION = poversion;
                ordermaincurrent.EDITTIME = DateTime.Now;
                ordermaincurrent.LASTCHANGETIME = item.LASTCHANGEDATETIME.Value.ToString("yyyy-MM-dd HH:mm:ss");
                mesdb.Updateable(ordermaincurrent).ExecuteCommand();

                #region hold
                var holdinfo = Getholdinfo(item);
                if (!string.IsNullOrEmpty(holdinfo))
                {
                    mesdb.Insertable(new O_ORDER_HOLD()
                    {
                        ID = MesDbBase.GetNewID<O_ORDER_HOLD>(mesdb, Customer.JUNIPER.ExtValue()),
                        ITEMID = item.ID,
                        UPOID = $@"{item.PONUMBER}{item.ITEM}",
                        HOLDFLAG = MesBool.Yes.ExtValue(),
                        HOLDREASON = holdinfo,
                        CREATETIME = DateTime.Now,
                        EDITTIME = DateTime.Now
                    }).ExecuteCommand();
                }
                #endregion

                #region 更新狀態
                item.MFLAG = ENUM_I137_H_STATUS.RELEASE.ExtValue();
                mesdb.Updateable(item).ExecuteCommand();
                #endregion
                JuniperBase.ReleaseJuniperExcption(mesdb, $@"{item.PONUMBER}{item.ITEM}", item.TRANID);
                #endregion
            });
        }



        string Getholdinfo(I137_I item)
        {
            var holdreason = string.Empty;
            var strreason = item.SALESORDERHOLD != null ? item.SALESORDERHOLD.Split(',') : null;
            if (strreason != null)
                foreach (var reasonitem in strreason)
                {
                    if (reasonitem.Trim() != "NA")
                        holdreason += (string.IsNullOrEmpty(holdreason) ? "" : ",") + reasonitem;
                }
            return holdreason;
        }

        static string converstrtoint(string str)
        {
            if (str.StartsWith("0"))
                str = converstrtoint(str.Substring(1));
            return str;
        }

        /// <summary>
        /// 區分CTO,BTS,ADV.....
        /// </summary>
        /// <param name="agiledata"></param>
        /// <returns></returns>
        public static string ConverOrderType(O_AGILE_ATTR agiledata, List<R_SKU_PLANT> skuplants = null)
        {
            var potype = string.Empty;
            if (agiledata == null)
                potype = ENUM_O_ORDER_MAIN_POTYPE.NONE.ExtValue();
            else if (string.IsNullOrEmpty(agiledata.USER_ITEM_TYPE))
                potype = ENUM_O_ORDER_MAIN_POTYPE.BTS.ExtValue();
            else if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.SYS.ExtValue()))
                potype = ENUM_O_ORDER_MAIN_POTYPE.CTO.ExtValue();
            else if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.BNDL.ExtValue())
                    && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Premium_Configurable_Sys.ExtValue()))
                potype = ENUM_O_ORDER_MAIN_POTYPE.CTO.ExtValue();
            else if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.BNDL.ExtValue())
                    && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Customer_Specific_CTO.ExtValue()))
                potype = ENUM_O_ORDER_MAIN_POTYPE.CTO.ExtValue();
            else
                potype = ENUM_O_ORDER_MAIN_POTYPE.BTS.ExtValue();

            //var existsnewplant = skuplants.Where(t => t.FOXCONN == agiledata.ITEM_NUMBER).ToList();
            //if (existsnewplant.Count == 0)
            //{
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.SYS.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.CTO.ExtValue();
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.BNDL.ExtValue())
            //            && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Premium_Configurable_Sys.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.CTO.ExtValue();
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.BNDL.ExtValue())
            //            && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Customer_Specific_CTO.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.CTO.ExtValue();
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.ADV.ExtValue())
            //            && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Advanced_Fixed_System.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.BTS.ExtValue();
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.APP.ExtValue())
            //            && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Fixed_Appliance.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.BTS.ExtValue();
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.SPARE.ExtValue())
            //            && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.FRU.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.BTS.ExtValue();
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.BNDL.ExtValue())
            //            && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Combo_Bundle.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.BNDL.ExtValue();
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE.BNDL.ExtValue())
            //            && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Fixed_Nonstockable_Bundle.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.BNDL.ExtValue();
            //    if (agiledata.USER_ITEM_TYPE.Equals(ENUM_O_AGILE_ATTR_USERITETYPE..ExtValue())
            //            && agiledata.OFFERING_TYPE.Equals(ENUM_O_AGILE_ATTR_OFFERINGTYPE.Fixed_Nonstockable_Bundle.ExtValue()))
            //        potype = ENUM_O_ORDER_MAIN_POTYPE.BNDL.ExtValue();
            //}
            return potype;
        }

        readonly string[] valid137headStr = $@"PaymentTerm,Ponumber,PODoctype,SellerPartyID,SalesOrderNumber,CompleteDelivery,HeaderSchedulingStatus,RMQQuoteNumber,RMQPONumber,VendorID".ToUpper().Split(',');
        readonly string[] valid137itemStr = $@"Jnp_Plant,Item,NetPrice,PN,CustReqShipDate,LineSchedulingStatus,PODeliveryDate,Quantity,Ponumber,ProductFamily,SoID,ParentItemID".ToUpper().Split(',');
        readonly string[] valid137detailStr = $@"componentid,Item".ToUpper().Split(',');

        public enum JuniperOrderChangeType
        {
            [EnumValue("BaseChange")]
            BaseChange,
            [EnumValue("QtyBomChange")]
            QtyBomChange,
            [EnumValue("Cancel")]
            Cancel
        }
    }

    public class Syn282 : FunctionBase
    {
        string b2bdbstr, mesdbstr = string.Empty; double defaultsynday = -3;
        public Syn282(string _mesdbstr, string _b2bdbstr, string _bu, double? _defaultsynday = null) : base(_mesdbstr, _bu)
        {
            b2bdbstr = _b2bdbstr;
            mesdbstr = _mesdbstr;
            if (_defaultsynday != null) defaultsynday = Convert.ToDouble(_defaultsynday);
        }

        public override void FunctionRun()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(b2bdbstr, false, DbType.SqlServer))
            {
                var waitsynlist = b2bdb.Queryable<B2B_R_I282>()
                    .Where(t => SqlSugar.SqlFunc.ToDate(t.F_LASTEDITDT) > DateTime.Now.AddDays(double.Parse(defaultsynday.ToString()))).ToList();
                if (waitsynlist.Count > 0)
                    using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
                    {
                        var tranids = waitsynlist.Select(t => t.TRANID).Distinct();
                        var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
                        var exsist = mesdb.Queryable<R_I282>().Where(t => IMesDbEx.OracleContain(t.F_ID.ToString(), filterIDs)).ToList();
                        foreach (var ctranid in tranids)
                        {
                            var traniddetail = waitsynlist.Where(t => t.TRANID == ctranid).ToList();
                            var dbres = mesdb.Ado.UseTran(() =>
                            {
                                foreach (var item in traniddetail)
                                {
                                    if (exsist.FindAll(t => t.F_ID == item.F_ID).Count > 0)
                                        continue;
                                    var targetobj = ObjectDataHelper.Mapper<R_I282, B2B_R_I282>(item);
                                    targetobj.ID = MesDbBase.GetNewID<R_I282>(mesdb, Customer.JUNIPER.ExtValue());
                                    targetobj.CREATETIME = DateTime.Now;
                                    targetobj.MFLAG = "N";
                                    mesdb.Insertable(targetobj).ExecuteCommand();
                                }
                            });
                            if (!dbres.IsSuccess)
                                throw new Exception(dbres.ErrorMessage);
                        }
                    }
            }
        }
    }

    public class Syn244 : FunctionBase
    {
        string b2bdbstr, mesdbstr = string.Empty; double defaultsynday = -3;
        public Syn244(string _mesdbstr, string _b2bdbstr, string _bu, double? _defaultsynday = null) : base(_mesdbstr, _bu)
        {
            b2bdbstr = _b2bdbstr;
            mesdbstr = _mesdbstr;
            if (_defaultsynday != null) defaultsynday = Convert.ToDouble(_defaultsynday);
        }

        public override void FunctionRun()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(b2bdbstr, false, DbType.SqlServer))
            {
                var waitsynlist = b2bdb.Queryable<B2B_R_I244>()
                    .Where(t => SqlSugar.SqlFunc.ToDate(t.F_LASTEDITDT) > DateTime.Now.AddDays(double.Parse(defaultsynday.ToString()))).ToList();
                if (waitsynlist.Count > 0)
                    using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
                    {
                        var tranids = waitsynlist.Select(t => t.TRANID).Distinct();
                        var filterIDs = waitsynlist.Select(w => w.F_ID).ToList();
                        var exsist = mesdb.Queryable<R_I244>().Where(t => IMesDbEx.OracleContain(t.F_ID.ToString(), filterIDs)).ToList();
                        foreach (var ctranid in tranids)
                        {
                            var traniddetail = waitsynlist.Where(t => t.TRANID == ctranid).ToList();
                            var dbres = mesdb.Ado.UseTran(() =>
                            {
                                foreach (var item in traniddetail)
                                {
                                    if (exsist.FindAll(t => t.F_ID == item.F_ID).Count > 0)
                                        continue;
                                    var targetobj = ObjectDataHelper.Mapper<R_I244, B2B_R_I244>(item);
                                    targetobj.ID = MesDbBase.GetNewID<R_I244>(mesdb, Customer.JUNIPER.ExtValue());
                                    targetobj.CREATETIME = DateTime.Now;
                                    targetobj.MFLAG = "N";
                                    mesdb.Insertable(targetobj).ExecuteCommand();
                                }
                            });
                            if (!dbres.IsSuccess)
                                throw new Exception(dbres.ErrorMessage);
                        }
                    }
            }
        }
    }

    public class SynAck : FunctionBase
    {
        string b2bdbstr, mesdbstr = string.Empty; double defaultsynday = -1;
        public SynAck(string _mesdbstr, string _b2bdbstr, string _bu, double? _defaultsynday = null) : base(_mesdbstr, _bu)
        {
            b2bdbstr = _b2bdbstr;
            mesdbstr = _mesdbstr;
            if (_defaultsynday != null) defaultsynday = Convert.ToDouble(_defaultsynday);
        }

        public override void FunctionRun()
        {
            using (var b2bdb = OleExec.GetSqlSugarClient(b2bdbstr, false, DbType.SqlServer))
            {
                using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
                {
                    #region other ack
                    var waitsynlist = b2bdb.Queryable<B2B_ACK>()
                        .Where(t =>  SqlSugar.SqlFunc.ToDate(t.F_LASTEDIT) > DateTime.Now.AddDays(double.Parse(defaultsynday.ToString()))).ToList();
                    foreach (var item in waitsynlist)
                    {
                        try
                        {
                            if (!mesdb.Queryable<O_B2B_ACK>().Where(t => t.F_ID == item.F_ID).Any())
                            {
                                var targetobj = ObjectDataHelper.Mapper<O_B2B_ACK, B2B_ACK>(item);
                                targetobj.ID = MesDbBase.GetNewID<R_I244>(mesdb, Customer.JUNIPER.ExtValue());
                                targetobj.CREATETIME = DateTime.Now;
                                mesdb.Insertable(targetobj).ExecuteCommand();
                            }
                        }
                        catch (Exception e)
                        {
                            JuniperBase.RecordJuniperExcption(mesdb, $@"Ack Syn Fail.Err: {e.Message}!", $@"{item.MESSAGEID}", item.TRANID, JuniperErrType.ACK, JuniperSubType.IAck, false);
                        }
                    }
                    #endregion
                    #region 054 ack
                    var waitsyn054list = b2bdb.Queryable<B2B_I054_ACK>()
                    .Where(t => SqlSugar.SqlFunc.ToDate(t.F_LASTEDIT) > DateTime.Now.AddDays(double.Parse(defaultsynday.ToString()))).ToList();
                    foreach (var item in waitsyn054list)
                    {
                        try
                        {
                            var aa = waitsyn054list.FindAll(t => t.F_ID == "568101");
                            if (!mesdb.Queryable<R_I054_ACK>().Where(t => t.F_ID == item.F_ID).Any())
                            {
                                var targetobj = ObjectDataHelper.Mapper<R_I054_ACK, B2B_I054_ACK>(item);
                                targetobj.ID = MesDbBase.GetNewID<R_I054_ACK>(mesdb, Customer.JUNIPER.ExtValue());
                                targetobj.CREATETIME = DateTime.Now;
                                mesdb.Insertable(targetobj).ExecuteCommand();
                                
                                //lock the unit when responsecode is '6030'
                                if (targetobj.RESPONSECODE == "6030")
                                {
                                    mesdb.Insertable(new R_SN_LOCK()
                                    {
                                        ID = MesDbBase.GetNewID<R_SN_LOCK>(mesdb, Customer.JUNIPER.ExtValue()),
                                        SN = targetobj.SERIALNUMBER,
                                        LOCK_STATION = "ALL",
                                        TYPE = "SN",
                                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                                        LOCK_REASON = $@"SN Hold By I054 ACK! HoldReason: {targetobj.RESPONSEMESSAGE}!",
                                        LOCK_TIME = DateTime.Now,
                                        LOCK_EMP = "JNPCUST"
                                    }).ExecuteCommand();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            JuniperBase.RecordJuniperExcption(mesdb, $@"I054Ack Syn Fail.Err: {e.Message}!", $@"{item.MESSAGEID}", item.TRANID, JuniperErrType.ACK, JuniperSubType.I054Ack, false);
                        }
                    }
                    #endregion
                }
            }
        }

        public enum JuniperB2BPlantCode
        {
            /// <summary>
            /// [EnumName("1350")]
            /// [EnumValue("0016000219")]
            /// </summary>
            [EnumName("1350")]
            [EnumValue("0016000219")]
            FJZ,
            /// <summary>
            /// [EnumName("1360")]
            /// [EnumValue("0016000220")]
            /// </summary>
            [EnumName("1360")]
            [EnumValue("0016000220")]
            FVN
        }

        public enum Juniper137PoType
        {
            /// <summary>
            ///EnumValue("I")
            ///EnumName("New")
            /// </summary>
            [EnumName("New")]
            [EnumValue("I")]
            New,
            /// <summary>
            /// EnumName("Change")
            /// EnumValue("U")
            /// </summary>
            [EnumName("Change")]
            [EnumValue("U")]
            Change
        }
    }

    public class SynCrem : FunctionBase
    {
        string tcode, mesdbstr, plantcode = string.Empty, currentday = string.Empty;
        Tuple<List<R_SAP0593_HEAD>, List<R_SAP0593_DETAIL>> d0593=null;
        public SynCrem(string _mesdbstr, string _bu, string _tcode, string _plantcode, object taskbase) : base(_mesdbstr, _bu)
        {
            mesdbstr = _mesdbstr;
            tcode = _tcode;
            plantcode = _plantcode;
            this.TaskBase = taskbase;
        }

        public override void FunctionRun()
        {
            Get0593();
            Scheduled();
            Unconstrained();
        }

        void Unconstrained()
        {
            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
            {
                tcode = "TC05";
                var runday = 365;
                var targetlist = new List<orderpartdata>();
                #region rmq
                var rmq = mesdb.Queryable<O_ORDER_MAIN, I137_I, O_AGILE_ATTR, O_SKU_CONFIG, R_SKU_PLANT>((t, i, a, c, r) => new object[] { JoinType.Inner, t.ITEMID == i.ID
                    ,  JoinType.Inner,t.PID == a.ITEM_NUMBER , JoinType.Inner, a.OFFERING_TYPE == c.OFFERINGTYPE, JoinType.Left, t.PID == r.FOXCONN})
                    .Where((t, i, a, c, r) => t.ORDERTYPE == ENUM_I137_PoDocType.ZRMQ.ToString() && t.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && t.RMQPONO == null 
                    && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) && c.BOMEXPLOSION == MesBool.Yes.ExtName() && a.ACTIVED == MesBool.Yes.ExtValue())
                    .Select((t, i, a, c, r) => new orderpartdata() { UPOID = t.UPOID, QTY = t.QTY, DELIVERY = i.CUSTREQSHIPDATE, USAGE = "1", PARTNO = t.PID, PLANTCODE = r.PLANTCODE }).ToList();

                var rmq_cto = mesdb.Queryable<O_ORDER_MAIN, I137_I, O_ORDER_OPTION, R_SKU_PLANT>((m, i, o, r) => new object[] { JoinType.Inner, m.ITEMID == i.ID, JoinType.Inner, m.ID == o.MAINID, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, i, o, r) => m.ORDERTYPE == ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && m.RMQPONO == null && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday))
                    .Select((m, i, o, r) => new orderpartdata()
                    { UPOID = m.UPOID, QTY = SqlFunc.ToString((SqlFunc.ToDouble(m.QTY) * SqlFunc.ToDouble(o.QTY))), DELIVERY = i.CUSTREQSHIPDATE, USAGE = o.QTY.ToString(), PARTNO = o.FOXPN, PLANTCODE = r.PLANTCODE }).ToList();
                #endregion

                #region wait product
                var normal = mesdb.Queryable<O_ORDER_MAIN, I137_I, O_PO_STATUS, O_AGILE_ATTR, O_SKU_CONFIG, R_SKU_PLANT>((m, i, o, a, c, r) => new object[] { JoinType.Inner, m.ITEMID == i.ID, JoinType.Inner, m.ID == o.POID
                    , JoinType.Inner, m.PID == a.ITEM_NUMBER, JoinType.Inner, a.OFFERING_TYPE == c.OFFERINGTYPE, JoinType.Left, m.PID == r.FOXCONN})
                    .Where((m, i, o, a, c, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && JnpConst.JnpNotProducStatus.Contains(o.STATUSID) && c.BOMEXPLOSION == MesBool.Yes.ExtName() && c.BOMEXPLOSION == MesBool.Yes.ExtName() && a.ACTIVED == MesBool.Yes.ExtValue())
                    .Select((m, i, o, a, c, r) => new orderpartdata() { UPOID = m.UPOID, QTY = m.QTY, DELIVERY = i.CUSTREQSHIPDATE, USAGE = "1", PARTNO = m.PID, PLANTCODE = r.PLANTCODE }).ToList();

                var normal_cto = mesdb.Queryable<O_ORDER_MAIN, I137_I, O_PO_STATUS, O_ORDER_OPTION, R_SKU_PLANT>((m, i, o, n, r) => new object[] { JoinType.Inner, m.ITEMID == i.ID, JoinType.Inner, m.ID == o.POID, JoinType.Inner, m.ID == n.MAINID, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, i, o, n, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && JnpConst.JnpNotProducStatus.Contains(o.STATUSID))
                    .Select((m, i, o, n, r) => new orderpartdata()
                    { UPOID = m.UPOID, QTY = SqlFunc.ToString((SqlFunc.ToDouble(m.QTY) * SqlFunc.ToDouble(n.QTY))), DELIVERY = i.CUSTREQSHIPDATE, USAGE = n.QTY.ToString(), PARTNO = n.FOXPN, PLANTCODE = r.PLANTCODE }).ToList();
                #endregion

                #region producting
                var producting = mesdb.Queryable<O_ORDER_MAIN, I137_I, O_PO_STATUS, O_AGILE_ATTR, O_SKU_CONFIG, R_SKU_PLANT>((m, i, o, a, c, r) => new object[] { JoinType.Inner, m.ITEMID == i.ID, JoinType.Inner, m.ID == o.POID
                    , JoinType.Inner, m.PID == a.ITEM_NUMBER, JoinType.Inner, a.OFFERING_TYPE == c.OFFERINGTYPE, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, i, o, a, c, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && o.STATUSID == ENUM_O_PO_STATUS.Production.ExtValue() && c.BOMEXPLOSION == MesBool.Yes.ExtName() && c.BOMEXPLOSION == MesBool.Yes.ExtName() && a.ACTIVED == MesBool.Yes.ExtValue())
                    .Select((m, i, o, a, c, r) => new orderpartdata() { UPOID = m.UPOID, QTY = m.QTY, DELIVERY = i.CUSTREQSHIPDATE, USAGE = "1", PARTNO = m.PID, PLANTCODE = r.PLANTCODE }).ToList();

                var producting_cto = mesdb.Queryable<O_ORDER_MAIN, I137_I, O_PO_STATUS, O_ORDER_OPTION, R_SKU_PLANT>((m, i, o, n, r) => new object[] { JoinType.Inner, m.ITEMID == i.ID, JoinType.Inner, m.ID == o.POID, JoinType.Inner, m.ID == n.MAINID, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, i, o, n, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && o.STATUSID == ENUM_O_PO_STATUS.Production.ExtValue())
                    .Select((m, i, o, n, r) => new orderpartdata()
                    { UPOID = m.UPOID, QTY = SqlFunc.ToString((SqlFunc.ToDouble(m.QTY) * SqlFunc.ToDouble(n.QTY))), DELIVERY = i.CUSTREQSHIPDATE, USAGE = n.QTY.ToString(), PARTNO = n.FOXPN, PLANTCODE = r.PLANTCODE }).ToList();

                mesdb.Queryable<R_SN, O_ORDER_MAIN, O_PO_STATUS, I137_I>((s, m, o, i) => s.WORKORDERNO == m.PREWO && m.ID == o.POID && m.ITEMID == i.ID)
                    .Where((s, m, o, i) => o.STATUSID == ENUM_O_PO_STATUS.Production.ExtValue() && o.VALIDFLAG == MesBool.Yes.ExtValue() && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) && s.VALID_FLAG == MesBool.Yes.ExtValue()
                    && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && s.CURRENT_STATION != "MRB")
                    .GroupBy((s, m, o, i) => new { m.UPOID }).Select((s, m, o, i) => new { m.UPOID, stocknum = SqlFunc.AggregateCount(s.SN) }).ToList().ForEach(t =>
                      {
                          producting.FindAll(p => p.UPOID == t.UPOID).ForEach(c => c.QTY = (double.Parse(c.QTY) - t.stocknum).ToString());
                          producting_cto.FindAll(p => p.UPOID == t.UPOID).ForEach(c => c.QTY = (int.Parse(c.QTY) - t.stocknum * int.Parse(c.USAGE)).ToString());
                      });
                #endregion

                #region Dismantle
                var dismantle = mesdb.Queryable<O_ORDER_MAIN, I137_I, O_PO_STATUS, O_AGILE_ATTR, O_SKU_CONFIG, R_SKU_PLANT>((m, i, o, a, c, r) => new object[] { JoinType.Inner, m.ITEMID == i.ID, JoinType.Inner, m.ID == o.POID
                    , JoinType.Inner, m.PID == a.ITEM_NUMBER, JoinType.Inner, a.OFFERING_TYPE == c.OFFERINGTYPE, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, i, o, a, c, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && o.STATUSID == ENUM_O_PO_STATUS.WaitDismantle.ExtValue() && c.BOMEXPLOSION == MesBool.Yes.ExtName() && a.ACTIVED == MesBool.Yes.ExtValue())
                    .Select((m, i, o, a, c, r) => new orderpartdata() { UPOID = m.UPOID, QTY = m.QTY, DELIVERY = i.CUSTREQSHIPDATE, USAGE = "1", PARTNO = m.PID, PLANTCODE = r.PLANTCODE }).ToList();

                var dismantle_cto = mesdb.Queryable<O_ORDER_MAIN, I137_I, O_PO_STATUS, O_ORDER_OPTION, R_SKU_PLANT>((m, i, o, n, r) => new object[] { JoinType.Inner, m.ITEMID == i.ID, JoinType.Inner, m.ID == o.POID, JoinType.Inner, m.ID == n.MAINID, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, i, o, n, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && o.STATUSID == ENUM_O_PO_STATUS.WaitDismantle.ExtValue())
                    .Select((m, i, o, n, r) => new orderpartdata()
                    { UPOID = m.UPOID, QTY = SqlFunc.ToString((SqlFunc.ToDouble(m.QTY) * SqlFunc.ToDouble(n.QTY))), DELIVERY = i.CUSTREQSHIPDATE, USAGE = n.QTY.ToString(), PARTNO = n.FOXPN, PLANTCODE = r.PLANTCODE }).ToList();

                mesdb.Queryable<R_SN, O_ORDER_MAIN, O_PO_STATUS, R_ORDER_WO, I137_I>((s, m, o, w, i) => m.ITEMID == i.ID && s.WORKORDERNO == w.WO && m.ID == o.POID && w.UPOID == m.UPOID)
                   .Where((s, m, o, w, i) => o.STATUSID == ENUM_O_PO_STATUS.WaitDismantle.ExtValue() && o.VALIDFLAG == MesBool.Yes.ExtValue() && w.VALID == MesBool.Yes.ExtValue()
                   && i.CUSTREQSHIPDATE < SqlFunc.GetDate().AddDays(runday) && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue()
                   && s.VALID_FLAG == MesBool.Yes.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && s.CURRENT_STATION != "MRB")
                   .GroupBy((s, m) => new { m.UPOID }).Select((s, m) => new { m.UPOID, stocknum = SqlFunc.AggregateCount(s.SN) }).ToList().ForEach(t =>
                   {
                       dismantle.FindAll(p => p.UPOID == t.UPOID).ForEach(c => c.QTY = (double.Parse(c.QTY) - t.stocknum).ToString());
                       dismantle_cto.FindAll(p => p.UPOID == t.UPOID).ForEach(c => c.QTY = (int.Parse(c.QTY) - t.stocknum * int.Parse(c.USAGE)).ToString());
                   });
                #endregion
                targetlist.AddRange(rmq);
                targetlist.AddRange(rmq_cto);
                targetlist.AddRange(normal);
                targetlist.AddRange(normal_cto); 
                targetlist.AddRange(producting);
                targetlist.AddRange(producting_cto);
                targetlist.AddRange(dismantle);
                targetlist.AddRange(dismantle_cto);
                var targetGroupbylist = (from t in targetlist
                                         group t by new { t.DELIVERY, t.PARTNO, t.PLANTCODE } into g
                                         select new orderpartdata()
                                         {
                                             PARTNO = g.Key.PARTNO,
                                             DELIVERY = g.Key.DELIVERY,
                                             PLANTCODE = g.Key.PLANTCODE,
                                             QTY = g.Sum(x => double.Parse(x.QTY)).ToString(),
                                         }).ToList();
                var targetdblist = new List<R_JNP_CREM>();
                var tcnum = $@"{tcode}{DateTime.Now.ToString("yyyyMMddhhmmss")}";
                #region blank
                targetGroupbylist.FindAll(t => t.DELIVERY == null).ForEach(t =>
                {
                    targetdblist.Add(new R_JNP_CREM()
                    {
                        ID = MesDbBase.GetNewID<R_JNP_CREM>(mesdb, this.bu),
                        TCCODE = tcode,
                        TC_NUMBER = tcnum,
                        PLANT = this.bu,
                        PLANTCODE = t.PLANTCODE != null ? t.PLANTCODE : plantcode,
                        ITEM_NUMBER = t.PARTNO,
                        QTY = t.QTY,
                        STARTDATE = "NOTDATE",
                        CREATETIME = DateTime.Now
                    });
                });
                #endregion
                #region due
                (from t in targetGroupbylist.Where(t => t.DELIVERY < DateTime.Now.Date)
                 group t by new { t.PARTNO, t.PLANTCODE } into g
                 select new orderpartdata()
                 {
                     PARTNO = g.Key.PARTNO,
                     PLANTCODE = g.Key.PLANTCODE,
                     QTY = g.Sum(t => double.Parse(t.QTY)).ToString(),
                 }).ToList().ForEach(t =>
                 {
                     targetdblist.Add(new R_JNP_CREM()
                     {
                         ID = MesDbBase.GetNewID<R_JNP_CREM>(mesdb, this.bu),
                         TCCODE = tcode,
                         TC_NUMBER = tcnum,
                         PLANT = this.bu,
                         PLANTCODE = t.PLANTCODE != null ? t.PLANTCODE : plantcode,
                         ITEM_NUMBER = t.PARTNO,
                         QTY = t.QTY,
                         STARTDATE = "DUEDATE",
                         CREATETIME = DateTime.Now
                     });
                 });
                #endregion

                #region runday->250
                targetGroupbylist.FindAll(t => t.DELIVERY >= DateTime.Now.Date).ForEach(t =>
                {
                    targetdblist.Add(new R_JNP_CREM()
                    {
                        ID = MesDbBase.GetNewID<R_JNP_CREM>(mesdb, this.bu),
                        TCCODE = tcode,
                        TC_NUMBER = tcnum,
                        PLANT = this.bu,
                        PLANTCODE = t.PLANTCODE != null ? t.PLANTCODE : plantcode,
                        ITEM_NUMBER = t.PARTNO,
                        QTY = t.QTY,
                        STARTDATE = t.DELIVERY?.ToString("yyyy-MM-dd"),
                        CREATETIME = DateTime.Now
                    });
                });
                #endregion

                #region rfc
                var plantcodes = targetdblist.Select(t => t.PLANTCODE).Distinct().ToList();
                foreach (var plc in plantcodes)
                {
                    if (this.plantcode == "MBGA")
                    {
                        ZCPP_NSBG_0001 rfc = new ZCPP_NSBG_0001(this.bu);
                        var dt = rfc.GetInitTableZTABIN();
                        var s0593 = GetCurrent0593(mesdb, "893");
                        SetRfcDt_365(targetdblist, dt, plc, s0593);
                        //SetRfcDt(targetdblist, dt, plc);
                        var baseday = DateTime.Now.ToString("yyyyMMdd");
                        rfc.SetValue(baseday, "T", plc, dt);
                        rfc.CallRFC(() => { MesSapHelp.SapLog(tcnum, rfc.getSapParameobj(), mesdb); });
                        var RERURN = rfc.GetTableValue("ZRETURN");
                        //save log
                        saveSapData(dt, targetdblist,d0593);
                    }
                    else
                    {
                        ZCSD_NSBG_0018 rfc = new ZCSD_NSBG_0018(this.bu);
                        var dt = rfc.GetInitTableZTABIN();
                        SetRfcDt_365(targetdblist, dt, plc);
                        var baseday = DateTime.Now.ToString("yyyyMMdd");
                        rfc.SetValue(baseday, "T", plc, dt);
                        rfc.CallRFC(() => { MesSapHelp.SapLog(tcnum, rfc.getSapParameobj(), mesdb); });
                        var RERURN = rfc.GetTableValue("ZRETURN");
                        //save log
                        saveSapData(dt, targetdblist);
                    }
                }
                #endregion

                #region insert db
                mesdb.Insertable(targetdblist).ExecuteCommand();
                #endregion
            }

        }

        void Scheduled()
        {
            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
            {
                tcode = "EC05";
                var runday = 365;
                var targetlist = new List<orderpartdata>();
                #region rmq
                var rmq = mesdb.Queryable<O_ORDER_MAIN, O_AGILE_ATTR, O_SKU_CONFIG, R_SKU_PLANT>((t, a, c, r) => new object[] { JoinType.Inner, t.PID == a.ITEM_NUMBER, JoinType.Inner, a.OFFERING_TYPE == c.OFFERINGTYPE , JoinType.Left, t.PID == r.FOXCONN})
                    .Where((t, a, c, r) => t.ORDERTYPE == ENUM_I137_PoDocType.ZRMQ.ToString() && t.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && t.RMQPONO == null && t.DELIVERY < SqlFunc.GetDate().AddDays(runday)
                     && c.BOMEXPLOSION == MesBool.Yes.ExtName() && a.ACTIVED==MesBool.Yes.ExtValue())
                    .Select((t, a, c, r) => new orderpartdata() { UPOID = t.UPOID, QTY = t.QTY, DELIVERY = t.DELIVERY, USAGE = "1", PARTNO = t.PID, PLANTCODE = r.PLANTCODE }).ToList();

                var rmq_cto = mesdb.Queryable<O_ORDER_MAIN, O_ORDER_OPTION, R_SKU_PLANT>((m, o, r) => new object[] { JoinType.Inner, m.ID == o.MAINID, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, o, r) => m.ORDERTYPE == ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && m.RMQPONO == null && m.DELIVERY < SqlFunc.GetDate().AddDays(runday))
                    .Select((m, o, r) => new orderpartdata()
                    { UPOID = m.UPOID, QTY = SqlFunc.ToString((SqlFunc.ToDouble(m.QTY) * SqlFunc.ToDouble(o.QTY))), DELIVERY = m.DELIVERY, USAGE = o.QTY.ToString(), PARTNO = o.FOXPN, PLANTCODE = r.PLANTCODE }).ToList();
                #endregion

                #region wait product
                var normal = mesdb.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_AGILE_ATTR, O_SKU_CONFIG, R_SKU_PLANT>((m, o, a, c, r) => new object[] { JoinType.Inner, m.ID == o.POID
                    , JoinType.Inner, m.PID == a.ITEM_NUMBER, JoinType.Inner, a.OFFERING_TYPE == c.OFFERINGTYPE , JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, o, a, c, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && m.DELIVERY < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && JnpConst.JnpNotProducStatus.Contains(o.STATUSID) && c.BOMEXPLOSION == MesBool.Yes.ExtName() && a.ACTIVED == MesBool.Yes.ExtValue())
                    .Select((m, o, a, c, r) => new orderpartdata() { UPOID = m.UPOID, QTY = m.QTY, DELIVERY = m.DELIVERY, USAGE = "1", PARTNO = m.PID, PLANTCODE = r.PLANTCODE }).ToList();

                var normal_cto = mesdb.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_ORDER_OPTION, R_SKU_PLANT>((m, o, n, r) => new object[] { JoinType.Inner, m.ID == o.POID, JoinType.Inner, m.ID == n.MAINID, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, o, n, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && m.DELIVERY < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && JnpConst.JnpNotProducStatus.Contains(o.STATUSID))
                    .Select((m, o, n, r) => new orderpartdata()
                    { UPOID = m.UPOID, QTY = SqlFunc.ToString((SqlFunc.ToDouble(m.QTY) * SqlFunc.ToDouble(n.QTY))), DELIVERY = m.DELIVERY, USAGE = n.QTY.ToString(), PARTNO = n.FOXPN, PLANTCODE = r.PLANTCODE }).ToList();
                #endregion

                #region producting
                var producting = mesdb.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_AGILE_ATTR, O_SKU_CONFIG, R_SKU_PLANT>((m, o, a, c, r) => new object[] { JoinType.Inner, m.ID == o.POID
                    , JoinType.Inner, m.PID == a.ITEM_NUMBER, JoinType.Inner, a.OFFERING_TYPE == c.OFFERINGTYPE, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, o, a, c, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && m.DELIVERY < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && o.STATUSID == ENUM_O_PO_STATUS.Production.ExtValue() && c.BOMEXPLOSION == MesBool.Yes.ExtName() && a.ACTIVED == MesBool.Yes.ExtValue())
                    .Select((m, o, a, c, r) => new orderpartdata() { UPOID = m.UPOID, QTY = m.QTY, DELIVERY = m.DELIVERY, USAGE = "1", PARTNO = m.PID, PLANTCODE = r.PLANTCODE }).ToList();

                var producting_cto = mesdb.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_ORDER_OPTION, R_SKU_PLANT>((m, o, n, r) => new object[] { JoinType.Inner, m.ID == o.POID, JoinType.Inner, m.ID == n.MAINID, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, o, n, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && m.DELIVERY < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && o.STATUSID == ENUM_O_PO_STATUS.Production.ExtValue())
                    .Select((m, o, n, r) => new orderpartdata()
                    { UPOID = m.UPOID, QTY = SqlFunc.ToString((SqlFunc.ToDouble(m.QTY) * SqlFunc.ToDouble(n.QTY))), DELIVERY = m.DELIVERY, USAGE = n.QTY.ToString(), PARTNO = n.FOXPN, PLANTCODE = r.PLANTCODE }).ToList();

                mesdb.Queryable<R_SN, O_ORDER_MAIN, O_PO_STATUS>((s, m, o) => s.WORKORDERNO == m.PREWO && m.ID == o.POID)
                    .Where((s, m, o) => o.STATUSID == ENUM_O_PO_STATUS.Production.ExtValue() && o.VALIDFLAG == MesBool.Yes.ExtValue() && m.DELIVERY < SqlFunc.GetDate().AddDays(runday) && s.VALID_FLAG == MesBool.Yes.ExtValue()
                    && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && s.CURRENT_STATION != "MRB")
                    .GroupBy((s, m, o) => new { m.UPOID }).Select((s, m, o) => new { m.UPOID, stocknum = SqlFunc.AggregateCount(s.SN) }).ToList().ForEach(t =>
                    {
                        producting.FindAll(p => p.UPOID == t.UPOID).ForEach(c =>
                         c.QTY = (double.Parse(c.QTY) - t.stocknum).ToString()
                         ); 
                        producting_cto.FindAll(p => p.UPOID == t.UPOID).ForEach(c =>
                        c.QTY = (int.Parse(c.QTY) - t.stocknum * int.Parse(c.USAGE)
                        ).ToString());
                    });
                #endregion

                #region Dismantle
                var dismantle = mesdb.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_AGILE_ATTR, O_SKU_CONFIG, R_SKU_PLANT>((m, o, a, c, r) => new object[] { JoinType.Inner, m.ID == o.POID
                    , JoinType.Inner, m.PID == a.ITEM_NUMBER, JoinType.Inner, a.OFFERING_TYPE == c.OFFERINGTYPE, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, o, a, c, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && m.DELIVERY < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && o.STATUSID == ENUM_O_PO_STATUS.WaitDismantle.ExtValue() && c.BOMEXPLOSION == MesBool.Yes.ExtName() && a.ACTIVED == MesBool.Yes.ExtValue())
                    .Select((m, o, a, c, r) => new orderpartdata() { UPOID = m.UPOID, QTY = m.QTY, DELIVERY = m.DELIVERY, USAGE = "1", PARTNO = m.PID, PLANTCODE = r.PLANTCODE }).ToList();

                var dismantle_cto = mesdb.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_ORDER_OPTION, R_SKU_PLANT>((m, o, n, r) => new object[] { JoinType.Inner, m.ID == o.POID, JoinType.Inner, m.ID == n.MAINID, JoinType.Left, m.PID == r.FOXCONN })
                    .Where((m, o, n, r) => m.ORDERTYPE != ENUM_I137_PoDocType.ZRMQ.ToString() && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue() && m.DELIVERY < SqlFunc.GetDate().AddDays(runday) &&
                    o.VALIDFLAG == MesBool.Yes.ExtValue() && o.STATUSID == ENUM_O_PO_STATUS.WaitDismantle.ExtValue())
                    .Select((m, o, n, r) => new orderpartdata()
                    { UPOID = m.UPOID, QTY = SqlFunc.ToString((SqlFunc.ToDouble(m.QTY) * SqlFunc.ToDouble(n.QTY))), DELIVERY = m.DELIVERY, USAGE = n.QTY.ToString(), PARTNO = n.FOXPN, PLANTCODE = r.PLANTCODE }).ToList();

                mesdb.Queryable<R_SN, O_ORDER_MAIN, O_PO_STATUS, R_ORDER_WO>((s, m, o, w) => s.WORKORDERNO == w.WO && m.ID == o.POID && w.UPOID == m.UPOID)
                   .Where((s, m, o, w) => o.STATUSID == ENUM_O_PO_STATUS.WaitDismantle.ExtValue() && o.VALIDFLAG == MesBool.Yes.ExtValue() && w.VALID == MesBool.Yes.ExtValue()
                   && m.DELIVERY < SqlFunc.GetDate().AddDays(runday) && m.CANCEL == ENUM_O_ORDER_MAIN.CANCEL_NO.ExtValue()
                   && s.VALID_FLAG == MesBool.Yes.ExtValue() && s.COMPLETED_FLAG == MesBool.Yes.ExtValue() && s.CURRENT_STATION != "MRB")
                   .GroupBy((s, m) => new { m.UPOID }).Select((s, m) => new { m.UPOID, stocknum = SqlFunc.AggregateCount(s.SN) }).ToList().ForEach(t =>
                   {
                       dismantle.FindAll(p => p.UPOID == t.UPOID).ForEach(c => c.QTY = (double.Parse(c.QTY) - t.stocknum).ToString());
                       dismantle_cto.FindAll(p => p.UPOID == t.UPOID).ForEach(c => c.QTY = (int.Parse(c.QTY) - t.stocknum * int.Parse(c.USAGE)).ToString());
                   });
                #endregion

                targetlist.AddRange(rmq);
                targetlist.AddRange(rmq_cto);
                targetlist.AddRange(normal);
                targetlist.AddRange(normal_cto);
                targetlist.AddRange(producting);
                targetlist.AddRange(producting_cto);
                targetlist.AddRange(dismantle);
                targetlist.AddRange(dismantle_cto);
                var targetGroupbylist = (from t in targetlist
                                         group t by new { t.DELIVERY, t.PARTNO, t.PLANTCODE } into g
                                         select new orderpartdata()
                                         {
                                             PARTNO = g.Key.PARTNO,
                                             DELIVERY = g.Key.DELIVERY,
                                             PLANTCODE = g.Key.PLANTCODE,
                                             QTY = g.Sum(x => double.Parse(x.QTY)).ToString(),
                                         }).ToList();
                var targetdblist = new List<R_JNP_CREM>();
                var tcnum = $@"{tcode}{DateTime.Now.ToString("yyyyMMddhhmmss")}";
                #region blank
                targetGroupbylist.FindAll(t => t.DELIVERY == null).ForEach(t =>
                {
                    targetdblist.Add(new R_JNP_CREM()
                    {
                        ID = MesDbBase.GetNewID<R_JNP_CREM>(mesdb, this.bu),
                        TCCODE = tcode,
                        TC_NUMBER = tcnum,
                        PLANT = this.bu,
                        PLANTCODE = t.PLANTCODE != null ? t.PLANTCODE : plantcode,
                        ITEM_NUMBER = t.PARTNO,
                        QTY = t.QTY,
                        STARTDATE = "NOTDATE",
                        CREATETIME = DateTime.Now
                    });
                });
                #endregion
                #region due
                (from t in targetGroupbylist.Where(t => t.DELIVERY < DateTime.Now.Date)
                 group t by new { t.PARTNO, t.PLANTCODE } into g
                 select new orderpartdata()
                 {
                     PARTNO = g.Key.PARTNO,
                     PLANTCODE = g.Key.PLANTCODE,
                     QTY = g.Sum(t => double.Parse(t.QTY)).ToString(),
                 }).ToList().ForEach(t =>
                 {
                     targetdblist.Add(new R_JNP_CREM()
                     {
                         ID = MesDbBase.GetNewID<R_JNP_CREM>(mesdb, this.bu),
                         TCCODE = tcode,
                         TC_NUMBER = tcnum,
                         PLANT = this.bu,
                         PLANTCODE = t.PLANTCODE != null ? t.PLANTCODE : plantcode,
                         ITEM_NUMBER = t.PARTNO,
                         QTY = t.QTY,
                         STARTDATE = "DUEDATE",
                         CREATETIME = DateTime.Now
                     });
                 });
                #endregion

                #region runday->250
                targetGroupbylist.FindAll(t => t.DELIVERY >= DateTime.Now.Date).ForEach(t =>
                {
                    targetdblist.Add(new R_JNP_CREM()
                    {
                        ID = MesDbBase.GetNewID<R_JNP_CREM>(mesdb, this.bu),
                        TCCODE = tcode,
                        TC_NUMBER = tcnum,
                        PLANT = this.bu,
                        PLANTCODE = t.PLANTCODE != null ? t.PLANTCODE : plantcode,
                        ITEM_NUMBER = t.PARTNO,
                        QTY = t.QTY,
                        STARTDATE = t.DELIVERY?.ToString("yyyy-MM-dd"),
                        CREATETIME = DateTime.Now
                    });
                });
                #endregion

                #region rfc
                var plantcodes = targetdblist.Select(t => t.PLANTCODE).Distinct().ToList();
                foreach (var plc in plantcodes)
                {
                    if (this.plantcode == "MBGA")
                    {
                        ZCPP_NSBG_0001 rfc = new ZCPP_NSBG_0001(this.bu);
                        var dt = rfc.GetInitTableZTABIN();
                        var s0593 = GetCurrent0593(mesdb, "883");
                        SetRfcDt_365(targetdblist, dt, plc, s0593);
                        //SetRfcDt(targetdblist, dt, plc);
                        var baseday = DateTime.Now.ToString("yyyyMMdd");
                        rfc.SetValue(baseday, "", plc, dt);
                        rfc.CallRFC(() => { MesSapHelp.SapLog(tcnum, rfc.getSapParameobj(), mesdb); });
                        var RERURN = rfc.GetTableValue("ZRETURN");
                        //save log
                        saveSapData(dt, targetdblist);
                    }
                    else
                    {
                        ZCSD_NSBG_0018 rfc = new ZCSD_NSBG_0018(this.bu);
                        var dt = rfc.GetInitTableZTABIN();
                        SetRfcDt_365(targetdblist, dt, plc);
                        var baseday = DateTime.Now.ToString("yyyyMMdd");
                        rfc.SetValue(baseday, "", plc, dt);
                        rfc.CallRFC(() => { MesSapHelp.SapLog(tcnum, rfc.getSapParameobj(), mesdb); });
                        var RERURN = rfc.GetTableValue("ZRETURN");
                        //save log
                        saveSapData(dt, targetdblist);
                    }
                }
                #endregion

                #region insert db
                mesdb.Insertable(targetdblist).ExecuteCommand();
                #endregion
            }

        }

        /// <summary>
        /// vn partno
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tcode"></param>
        /// <returns></returns>
        List<R_SAP0593_DETAIL> GetCurrent0593(SqlSugarClient mesdb, string tcode)
        {
            //return (from t in mesdb.Queryable<R_SAP0593_HEAD, R_SAP0593_DETAIL, R_F_CONTROL>((h, d, r) => h.KEYVALUE == d.HEADID && d.MATNR == r.VALUE)
            //.Where((h, d, r) => h.BASDAY == currentday && r.FUNCTIONNAME == "CREM_VN" && h.PLSCN == tcode).Select((h, d, r) => d).ToList()
            //        group t by new { t.MATNR, t.DAYS } into g
            //        select new R_SAP0593_DETAIL()
            //        {
            //            MATNR = g.Key.MATNR,
            //            DAYS = g.Key.DAYS,
            //            QTY = g.Sum(t => double.Parse(t.QTY)).ToString(),
            //        }).ToList();
            return d0593 == null ? null : (from t in (from d in d0593.Item2
                                                      join h in d0593.Item1 on d.HEADID equals h.KEYVALUE
                                                      join c in mesdb.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "CREM_VN" && t.FUNCTIONTYPE == ENUM_R_F_CONTROL.FUNCTIONTYPE_NOSYSTEM.ExtValue()).ToList() on d.MATNR equals c.VALUE
                                                      where h.PLSCN == tcode && h.MATNR == d.MATNR
                                                      select d)
                                           group t by new { t.MATNR, t.DAYS } into g
                                           select new R_SAP0593_DETAIL()
                                           {
                                               MATNR = g.Key.MATNR,
                                               DAYS = g.Key.DAYS,
                                               QTY = g.Sum(t => double.Parse(t.QTY)).ToString(),
                                           }).ToList();
        }

        void saveSapData<T>(DataTable dt,List<T> Ls, Tuple<List<R_SAP0593_HEAD>, List<R_SAP0593_DETAIL>> ls2 =null)
        {
            var path = $@"{System.IO.Directory.GetCurrentDirectory()}\\File\\Jnp\\Crem\\";
            var filename = $@"OpoReport{DateTime.Now.AddDays(-1).ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}.csv";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            var sapfile = $@"{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}sap.csv";
            var sourcefile = $@"{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}source.csv";
            var d0593Headfile = $@"{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}d0593head.csv";
            var d0593detailfile = $@"{DateTime.Now.ToString(MES_CONST_DATETIME_FORMAT.DEFAULT.ExtValue())}d0593detail.csv";
            ExcelHelp.ExportExcelToLoacl(dt, $@"{path}{sapfile}", true);
            ExcelHelp.ExportCsv(Ls, $@"{path}{sourcefile}");
            if (ls2 != null)
            {
                ExcelHelp.ExportCsv(ls2.Item1, $@"{path}{d0593Headfile}");
                ExcelHelp.ExportCsv(ls2.Item2, $@"{path}{d0593detailfile}");
            }
        }
        
        void Get0593()
        {
            if (this.plantcode != "MBGA")
                return;
            using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
            {
                try
                {
                    var sap0593 = ConfigGet("SAP0593");
                    var plantcode0593s = ConfigGet("PLANTCODE0593").Split(',');
                    var saptcodes = ConfigGet("SAPTECODE").Split(',');
                    ZCPP_NSBG_0593 rfc = new ZCPP_NSBG_0593(sap0593);
                    currentday = new Func<string>(() =>
                    {
                        var chinaZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                        return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, chinaZone).ToString("yyyy-MM-dd");
                    })();
                    //test
                    //currentday = "2022-01-06";
                    var currentwk = Convert.ToDateTime(currentday).AddDays(1 - Convert.ToInt32(Convert.ToDateTime(currentday).DayOfWeek.ToString("d")));
                    var heads = new List<R_SAP0593_HEAD>();
                    var details = new List<R_SAP0593_DETAIL>();
                    foreach (var scode in saptcodes)
                    {
                        foreach (var plc in plantcode0593s)
                        {
                            var tcnum = $@"{plc}{DateTime.Now.ToString("yyyyMMddhhmmss")}";
                            rfc.SetValue(currentday, scode, plc);
                            rfc.CallRFC(() => { MesSapHelp.SapLog(tcnum, rfc.getSapParameobj(), mesdb); });
                            if (rfc.GetValue("O_FLAG") == "0")
                            {
                                var datas = rfc.GetTableValue("OUT_TAB");
                                foreach (DataRow dr in datas.Rows)
                                {
                                    heads.Add(new R_SAP0593_HEAD()
                                    {
                                        //ID = MesDbBase.GetNewID<R_SAP0593_HEAD>(mesdb, this.bu),
                                        KEYVALUE = tcnum,
                                        MANTD = dr["MANDT"].ToString(),
                                        WERKS = dr["WERKS"].ToString(),
                                        MATNR = dr["MATNR"].ToString(),
                                        BASDAY = dr["BASDAY"].ToString(),
                                        PLSCN = dr["PLSCN"].ToString(),
                                        MAKTX = dr["MAKTX"].ToString(),
                                        ERNAM = dr["ERNAM"].ToString(),
                                        UPLOADDATE = dr["UPLOADDATE"].ToString(),
                                        UPLOADTIME = dr["UPLOADTIME"].ToString(),
                                        CHANGENAME = dr["CHANGENAME"].ToString(),
                                        CHANGEDATA = dr["CHANGEDATE"].ToString(),
                                        CHANGETIME = dr["CHANGETIME"].ToString(),
                                        CREATETIME = DateTime.Now
                                        //ACTIVE = "1"
                                    });
                                    //var details = new List<R_SAP0593_DETAIL>();
                                    foreach (DataColumn col in datas.Columns)
                                    {
                                        if (col.ColumnName.StartsWith("QDAY") || col.ColumnName.StartsWith("QWEEK"))
                                        {
                                            details.Add(new R_SAP0593_DETAIL()
                                            {
                                                //ID = MesDbBase.GetNewID<R_SAP0593_DETAIL>(mesdb, this.bu),
                                                HEADID = tcnum,
                                                MATNR = dr["MATNR"].ToString(),
                                                DAYS = col.ColumnName,
                                                //DAYS_C = new Func<string>(() =>
                                                //{
                                                //    var colday = int.Parse(col.ColumnName.Replace("QDAY", ""));
                                                //    if (colday == 1)
                                                //        return currentday;
                                                //    else
                                                //        return Convert.ToDateTime(currentwk).AddDays((colday - 1) * 7).ToString("yyyy-MM-dd");
                                                //})(),
                                                QTY = dr[col].ToString()
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    d0593 = new Tuple<List<R_SAP0593_HEAD>, List<R_SAP0593_DETAIL>>( heads,details);
                    return;
                    //var res = mesdb.Ado.UseTran(() =>
                    //{
                    //    var headc = mesdb.Queryable<R_SAP0593_HEAD>().Where(t => t.BASDAY == currentday).ToList();
                    //    if (headc.Count > 0)
                    //    {
                    //        mesdb.Deleteable<R_SAP0593_DETAIL>().Where(t => t.HEADID == headc.FirstOrDefault().KEYVALUE).ExecuteCommand();
                    //        mesdb.Deleteable(headc).ExecuteCommand();
                    //    }
                    //    mesdb.Insertable(heads).ExecuteCommand();
                    //    mesdb.Insertable(details).ExecuteCommand();
                    //});
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            d0593 = null;
        }

        void SetRfcDt(List<R_JNP_CREM> targetdblist, DataTable dt, string plc)
        {
            var targetdbplantcodelist = targetdblist.FindAll(t => t.PLANTCODE == plc).ToList();
            var partnolist = targetdbplantcodelist.Select(t => t.ITEM_NUMBER).Distinct().ToList();
            foreach (var partno in partnolist)
            {
                var partnoData = targetdbplantcodelist.FindAll(t => t.ITEM_NUMBER == partno).ToList();
                var unscheduled = partnoData.FindAll(t => t.STARTDATE == "NOTDATE").ToList().FirstOrDefault();
                var postfcd = partnoData.FindAll(t => t.STARTDATE == "DUEDATE").ToList().FirstOrDefault();
                var dr = dt.NewRow();
                dr["IDENTIFIER"] = "PRODUCT";
                dr["ROHS_STATUS"] = "ROHS";
                dr["ORG"] = plc;
                dr["BU"] = "";
                dr["PRODUCT"] = partno;
                dr["UNSCHEDULED"] = unscheduled != null ? unscheduled.QTY : "0";
                dr["POSTFCD"] = postfcd != null ? postfcd.QTY : "0";
                for (int i = 1; i <= 250; i++)
                {
                    var daystr = $@"DAY{i.ToString().PadLeft(3, '0')}";
                    var daytime = DateTime.Now.AddDays(i - 1).ToString("yyyy-MM-dd");
                    var indate = partnoData.FindAll(t => t.STARTDATE == daytime).ToList().FirstOrDefault();
                    dr[daystr] = indate != null ? indate.QTY : "0";
                }
                dt.Rows.Add(dr);
            }
        }

        ///針對標題的設置：
        ///day120之前(包含day120)是daily, 按當前邏輯
        ///day121之後(包含day121)是weekly, day121對應日期如下:
        ///                day120若為週一, day121為day120日期 + 7(比如day120 = 2022 / 4 / 11時, day121 = 2022 / 4 / 11 + 7 = 2022 / 4 / 18)
        ///day120若為週二, day121為day120日期 + 6(比如day120 = 2022 / 4 / 12時, day121 = 2022 / 4 / 12 + 6 = 2022 / 4 / 18)
        ///day120若為週三, day121為day120日期 + 5(比如day120 = 2022 / 4 / 13時, day121 = 2022 / 4 / 13 + 5 = 2022 / 4 / 18)
        ///day120若為週四, day121為day120日期 + 4(比如day120 = 2022 / 4 / 14時, day121 = 2022 / 4 / 14 + 4 = 2022 / 4 / 18)
        ///day120若為週五, day121為day120日期 + 3(比如day120 = 2022 / 4 / 15時, day121 = 2022 / 4 / 15 + 3 = 2022 / 4 / 18)
        ///day120若為週六, day121為day120日期 + 2(比如day120 = 2022 / 4 / 16時, day121 = 2022 / 4 / 16 + 2 = 2022 / 4 / 18)
        ///day120若為週日, day121為day120日期 + 1(比如day120 = 2022 / 4 / 17時, day121 = 2022 / 4 / 17 + 1 = 2022 / 4 / 18)
        ///day122, day123.....後續自動加 1週，用週一的日期顯示(比如day121 = 2022 / 4 / 18時, day122 = 2022 / 4 / 25, day123 = 2022 / 5 / 2)
        ///針對擴大範圍至360天backlog後的數據處理：
        ///假設：D1表示TC05 & EC05日期第一天,  Dx表示TC05 & EC05日期第x天。x <= 120時, Dx = dayx。
        ///day120之前(包含day120)按當前邏輯
        ///day121之後(包含day121)的計算方法:
        ///D120若為週一, day121為D121~D133加總, day122為後續7天加總,  day123為後續7天加總… (比如day120 = 2022 / 4 / 11時, day121 = 2022 / 4 / 12~2022 / 4 / 24 bklg加總, day122 = 2022 / 4 / 25~2022 / 5 / 1 bklg加總, day123 = 2022 / 5 / 2~2022 / 5 / 8 bklg加總)
        ///D120若為週二, day121為D121~D132加總, day122為後續7天加總,  day123為後續7天加總…
        ///D120若為週三, day121為D121~D131加總, day122為後續7天加總,  day123為後續7天加總…
        ///D120若為週四, day121為D121~D130加總, day122為後續7天加總,  day123為後續7天加總…
        ///D120若為週五, day121為D121~D129加總, day122為後續7天加總,  day123為後續7天加總…
        ///D120若為週六, day121為D121~D128加總, day122為後續7天加總,  day123為後續7天加總…
        ///D120若為週日, day121為D121~D127加總, day122為後續7天加總,  day123為後續7天加總…
        void SetRfcDt_365(List<R_JNP_CREM> targetdblist, DataTable dt, string plc,List<R_SAP0593_DETAIL> s0593=null)
        {
            var targetdbplantcodelist = targetdblist.FindAll(t => t.PLANTCODE == plc).ToList();
            var partnolist = targetdbplantcodelist.Select(t => t.ITEM_NUMBER).Distinct().ToList();
            foreach (var partno in partnolist)
            {
                var partnoData = targetdbplantcodelist.FindAll(t => t.ITEM_NUMBER == partno).ToList();
                var unscheduled = partnoData.FindAll(t => t.STARTDATE == "NOTDATE").ToList().FirstOrDefault();
                var postfcd = partnoData.FindAll(t => t.STARTDATE == "DUEDATE").ToList().FirstOrDefault();
                var dr = dt.NewRow();
                dr["IDENTIFIER"] = "PRODUCT";
                dr["ROHS_STATUS"] = "ROHS";
                dr["ORG"] = plc;
                dr["BU"] = "";
                dr["PRODUCT"] = partno;
                dr["UNSCHEDULED"] = unscheduled != null ? unscheduled.QTY : "0";
                dr["POSTFCD"] = postfcd != null ? postfcd.QTY : "0";
                for (int i = 1; i <= 120; i++)
                {
                    var daystr = $@"DAY{i.ToString().PadLeft(3, '0')}";
                    var daytime = DateTime.Now.AddDays(i - 1).ToString("yyyy-MM-dd");
                    var indate = partnoData.FindAll(t => t.STARTDATE == daytime).ToList().FirstOrDefault();
                    dr[daystr] = indate != null ? indate.QTY : "0";
                }
                #region 121
                var sday120 = DateTime.Now.AddDays(119);
                int count = sday120.DayOfWeek - DayOfWeek.Monday;
                if (count == -1) count = 6;
                var e121day = sday120.AddDays(-count+7);
                var day121str = $@"DAY{121.ToString().PadLeft(3, '0')}";
                var d121days = new List<string>();
                for (int i = 1; i <= -count + 7+6; i++)                
                    d121days.Add(sday120.AddDays(i).ToString("yyyy-MM-dd"));
                var q121 = 0;
                partnoData.FindAll(t => d121days.Contains(t.STARTDATE)).ToList().ForEach(t=> {
                    q121 += int.Parse(t.QTY);
                });
                dr[day121str] = q121.ToString();
                #endregion
                #region 122-130
                for (int i = 1; i < 130; i++)
                {
                    var dayhstr = $@"DAY{(121+i).ToString().PadLeft(3, '0')}";
                    var qc = 0;
                    var dddays = new List<string>();
                    for (int j = 1; j <= 7; j++)
                        dddays.Add(e121day.AddDays((i)*7+j-1).ToString("yyyy-MM-dd"));
                    partnoData.FindAll(t => dddays.Contains(t.STARTDATE)).ToList().ForEach(t => {
                        qc += int.Parse(t.QTY);
                    });
                    dr[dayhstr] = qc.ToString();
                }
                #endregion
                dt.Rows.Add(dr);
            }
            #region add vn 0593 data
            if (s0593 == null)
                return;
            var s0593pns = s0593.Select(t => t.MATNR).Distinct().ToList();
            foreach (var pn in s0593pns)
            {
                var partnoData = s0593.FindAll(t => t.MATNR == pn).ToList();
                var dr = dt.NewRow();
                dr["IDENTIFIER"] = "PRODUCT";
                dr["ROHS_STATUS"] = "ROHS";
                dr["ORG"] = plc;
                dr["BU"] = "";
                dr["PRODUCT"] = pn;
                dr["UNSCHEDULED"] ="0";
                dr["POSTFCD"] = "0";
                for (int i = 1; i <= 120; i++)
                {
                    var daystr = $@"DAY{i.ToString().PadLeft(3, '0')}";
                    var s0593str = $@"QDAY{i.ToString().PadLeft(3, '0')}";
                    var indate = partnoData.FindAll(t => t.DAYS == s0593str).ToList().FirstOrDefault();
                    dr[daystr] = indate != null ? indate.QTY : "0";
                }                
                #region 121-130
                for (int i = 1; i <= 130; i++)
                {
                    var daystr = $@"DAY{(120+i).ToString().PadLeft(3, '0')}";
                    var s0593str = $@"QWEEK{i.ToString().PadLeft(3, '0')}";
                    var indate = partnoData.FindAll(t => t.DAYS == s0593str).ToList().FirstOrDefault();
                    dr[daystr] = indate != null ? indate.QTY : "0";
                }
                #endregion
                dt.Rows.Add(dr);
            }
            #endregion
        }

        class orderpartdata
        {
            public string UPOID { get; set; }
            public DateTime? DELIVERY { get; set; }
            public string QTY { get; set; }
            public string PLANTCODE { get; set; }
            public string USAGE { get; set; }
            public string PARTNO { get; set; }
        }
    }




    public class SynTest : FunctionBase
    {
        string fjzdbstr, mesdbstr = string.Empty; double defaultsynday = -10;
        public SynTest(string _mesdbstr, string _fjzdbstr, string _bu, double? _defaultsynday = null) : base(_mesdbstr, _bu)
        {
            fjzdbstr = _fjzdbstr;
            mesdbstr = _mesdbstr;
            if (_defaultsynday != null) defaultsynday = Convert.ToDouble(_defaultsynday);
        }

        void syntable<T>(SqlSugarClient mesdb, SqlSugarClient transdb, List<T> mesobjs)
        {

        }

        public override void FunctionRun()
        {
            using (var fjzdb = OleExec.GetSqlSugarClient(fjzdbstr, false))
            {
                using (var mesdb = OleExec.GetSqlSugarClient(mesdbstr, false))
                {
                    try
                    {
                        var dbres = fjzdb.Ado.UseTran(() =>
                        {
                            var jsonobjs = mesdb.Queryable<R_JSON>().ToList();
                            foreach (var item in jsonobjs)
                            {
                                //var existsobj = fjzdb.Queryable<R_JSON>().Where(t => t.ID == item.ID).Any();
                                fjzdb.Deleteable(item).ExecuteCommand();
                                fjzdb.Insertable(item).ExecuteCommand();
                            }
                        });
                        if (!dbres.IsSuccess)
                            throw new Exception(dbres.ErrorMessage);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
    }
}