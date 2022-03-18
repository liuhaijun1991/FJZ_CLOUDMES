using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.Common;
using MESPubLab.MesBase;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MESDataObject.Common.EnumExtensions;
using static MESDataObject.Constants.PublicConstants;

namespace MESJuniper.Base
{
    /// <summary>
    /// JuniperECN相關處理函數
    /// </summary>
    public class EcnFunction
    {
        /// <summary>
        /// 獲取當前最新的必要變更版本號
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public static string GetLastMandatoryVer(string Skuno, OleExec sfcdb)
        {
            string ret = "";
            var ecs = sfcdb.ORM.Queryable<R_JNP_ECNPAGE>().Where(t => t.ITEMNUMBER == Skuno).OrderBy(t => t.REV, OrderByType.Desc).ToList();
            for (int i = 0; i < ecs.Count; i++)
            {
                if (ecs[i].ECOREPORTING != null && ecs[i].ECOREPORTING.Contains("Mandatory"))
                {
                    ret = ecs[i].REV;
                    break;
                }
            }
            if (ret == "")
            {
                ret = ecs[ecs.Count -1].REV;
            }

            if (ret.Length > 2)
            {
                ret = ret.Substring(0, 2);
            }
            return ret;
        }
        /// <summary>
        /// 獲取當前最新的必要變更版本號
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public static List<string> GetLastMandatoryUsefulVers(string Skuno, OleExec sfcdb)
        {
            List<string> ret = new List<string>();
            var ecs = sfcdb.ORM.Queryable<R_JNP_ECNPAGE>().Where(t => t.ITEMNUMBER == Skuno)
                .OrderBy(t => t.REV, OrderByType.Desc)
                .OrderBy(t=>t.CREATEDATE,OrderByType.Desc).ToList();
            for (int i = 0; i < ecs.Count; i++)
            {
                ret.Add(rev2(ecs[i].REV.Substring(0,2)));
                if (ecs[i].ECOREPORTING != null && ecs[i].ECOREPORTING.Contains("Mandatory"))
                {
                    //ret = ecs[i].REV;
                    break;
                }
            }
           
            
            return ret;
        }

        /// <summary>
        /// 取得當前能繼續生產與使用的版本號列表
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public static List<string> GetUsefulVer(string Skuno, OleExec sfcdb)
        {
            List<string> ret = new List<string>();
            var ecs = sfcdb.ORM.Queryable<R_JNP_ECNPAGE>().Where(t => t.ITEMNUMBER == Skuno)
                .OrderBy(t => t.REV, OrderByType.Desc)
                .OrderBy(t=>t.CREATEDATE, OrderByType.Desc).ToList();
            for (int i = 0; i < ecs.Count; i++)
            {
                ret.Add(rev2(ecs[i].REV));
                if (ecs[i].ECOREPORTING != null && ecs[i].ECOREPORTING.Contains("Mandatory"))
                {
                    if (ecs[i].CUSTREQUIRDATE != null)
                    {
                        DateTime time = new DateTime();
                        if (DateTime.TryParse(ecs[i].CUSTREQUIRDATE, out time))
                        {
                            if (time < DateTime.Now)
                            {
                                return ret;
                            }
                        }
                        else
                        {
                            return ret;
                        }
                    }
                }
            }
            return ret;
        }

        public static List<string> GetCurEcnVer(string Skuno, OleExec sfcdb)
        {
            List<string> ret = new List<string>();
            var ecs = sfcdb.ORM.Queryable<R_JNP_ECNPAGE>().Where(t => t.ITEMNUMBER == Skuno).OrderBy(t => t.REV, OrderByType.Desc).ToList();
            for (int i = 0; i < ecs.Count; i++)
            {
                ret.Add(rev2(ecs[i].REV));
                if (ecs[i].ECOREPORTING != null && ecs[i].ECOREPORTING.Contains("Mandatory"))
                {
                    return ret;
                }
            }
            return ret;
        }

        /// <summary>
        /// 取得當前能繼續生產與使用的CLEI號列表
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public static List<string> GetUsefulCLEICode(string Skuno, OleExec sfcdb)
        {
            List<string> rev = GetUsefulVer(Skuno,sfcdb);
           
            var ret = sfcdb.ORM.Queryable<O_AGILE_ATTR>().Where(t => t.ITEM_NUMBER == Skuno && rev.Contains(SqlFunc.Substring(t.REV, 0, 2))).Select(t => t.CLEI_CODE).ToList();
            return ret;
        }

        static string rev2(string ver)
        {
            if (ver.Length > 2)
            {
                return ver.Substring(0, 2);
            }
            return ver;
        }
    }


    public class EcoBase
    {
        string bu;
        SqlSugarClient db;
        string EcnLockEmp = "EcnLock";
        public EcoBase(SqlSugarClient _db, string _bu)
        {
            bu = _bu;
            db = _db;
        }
        public void EcoProcess()
        {

            var EcoDatas = db.Queryable<R_JNP_ECNPAGE>().Where(t => t.ACTIVED == MesBool.No.ExtValue())
            .OrderBy(t => t.ITEMNUMBER, OrderByType.Asc)
            .OrderBy(t => t.REV, OrderByType.Asc)
            .OrderBy(t => t.CREATEDATE, OrderByType.Asc).ToList();
            foreach (var item in EcoDatas)
            {
                var res = db.Ado.UseTran(() =>
                {
                    if (item.ECOREPORTING!=null && item.ECOREPORTING.IndexOf("Mandatory") > -1)
                    {
                        var LockPnRev = GetLockPnRev(item);
                        if (LockPnRev.Count > 0)
                        {
                            #region SKU+REV
                            LockPnAndRev(LockPnRev, item);
                            #endregion

                            //#region BY SKU
                            //LockSkuWip(LockPnRev, item);
                            //LockSkuStock(LockPnRev, item);
                            //LockSkuKpWithNotShip(LockPnRev, item);
                            //#endregion

                            #region BY COMPONENTS
                            LockCmpWip(LockPnRev, item);
                            //LockCmpStock(LockPnRev, item);
                            //LockCmpWithNotShip(LockPnRev, item);
                            #endregion
                        }
                    }
                    item.ACTIVED = MesBool.Yes.ExtValue();
                    db.Updateable(item).ExecuteCommand();
                });
            }
        }
        List<string> GetLockPnRev(R_JNP_ECNPAGE main)
        {
            var res = new List<string>();
            var targets = db.Queryable<R_JNP_ECNPAGE>().Where(t => t.ITEMNUMBER == main.ITEMNUMBER)
                .OrderBy(t => t.REV, OrderByType.Asc)
                .OrderBy(t => t.CREATEDATE, OrderByType.Asc).ToList();
            var betarget = db.SqlQueryable<R_JNP_ECNPAGE>($@"SELECT * FROM R_JNP_ECNPAGE WHERE ITEMNUMBER='{main.ITEMNUMBER}'  and substr(rev,0,2)<substr('{main.REV}',0,2) and ecoreporting like 'Mandatory%' order by substr(rev,0,2) desc,createdate desc ")
                .ToList().FirstOrDefault();
            foreach (var item in targets)
            {
                var rev = item.REV.Substring(0, 2);
                if (item.ID == main.ID)
                    break;
                if (((betarget!=null && string.Compare(betarget.REV.Substring(0,2), rev)<=0) || betarget==null) &&
                    !res.Contains(rev) && rev!= main.REV.Substring(0,2))
                    res.Add(rev);
            }
            return res;
        }
        void LockPnAndRev(List<string> revs, R_JNP_ECNPAGE main)
        {
            var crev = main.REV.Substring(0, 2);
            var lockrev = db.Queryable<R_SN_LOCK>().Where(t => t.TYPE == LockType.Skuno.ExtValue() && t.WORKORDERNO == main.ITEMNUMBER && t.LOCK_EMP == EcnLockEmp && t.LOCK_STATUS == MesBool.Yes.ExtValue() && t.LOCK_LOT == crev).ToList();
            foreach (var rev in revs)
            {
                var cl = lockrev.FindAll(t => t.SN == rev).ToList().FirstOrDefault();
                if (cl!=null)
                {
                    cl.LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE);
                    db.Updateable(cl).ExecuteCommand();
                }
                else
                {
                    db.Insertable(new R_SN_LOCK()
                    {
                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                        LOCK_LOT = main.REV.Substring(0,2),
                        WORKORDERNO = main.ITEMNUMBER,
                        LOCK_STATION = LockStation.ALL.ExtValue(),
                        TYPE = LockType.Skuno.ExtValue(),
                        SN = rev,
                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                        LOCK_REASON = $@"Ecn Lock(Lock By Sku:{ main.ITEMNUMBER} and Rev:{rev}).Reason:EcnNo:{main.CHANGENO},Eco:{main.CUSTCHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING},The Current Version is {rev},Pls Check!",
                        LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                        LOCK_EMP = EcnLockEmp,
                        CREATETIME = DateTime.Now,
                        CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                    }).ExecuteCommand();
                }
            }
        }
        /// <summary>
        /// Skuno -Wip=>lock wo
        /// </summary>
        /// <param name="revs"></param>
        /// <param name="main"></param>
        void LockSkuWip(List<string> revs, R_JNP_ECNPAGE main)
        {
            var isdof = db.Queryable<O_ORDER_MAIN>().Any(t => t.PID == main.ITEMNUMBER);
            foreach (var rev in revs)
            {
                #region DOF 
                if (isdof)
                {
                    var wosrev = db.Queryable<R_SAP_AS_BOM, R_WO_BASE>((s, w) => s.WO == w.WORKORDERNO)
                        .Where((s, w) => w.WORKORDER_QTY != w.FINISHED_QTY &&
                        !SqlFunc.Subqueryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == w.WORKORDERNO && t.TYPE == LockType.WorkOrderNo.ExtValue() && t.LOCK_EMP == EcnLockEmp).Any() &&
                        ((s.PARENTPN == main.ITEMNUMBER && s.PARENTREV == rev) || (s.PN == main.ITEMNUMBER && s.PNREV == rev))).Select((s, w) => w).Distinct().ToList();
                    foreach (var woobj in wosrev)
                    {
                        db.Insertable(new R_SN_LOCK()
                        {
                            ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                            WORKORDERNO = woobj.WORKORDERNO,
                            LOCK_STATION = LockStation.ALL.ExtValue(),
                            TYPE = LockType.WorkOrderNo.ExtValue(),
                            SN = rev,
                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                            LOCK_REASON = $@"Ecn Lock--EcnNo:{main.CHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING}",
                            LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                            LOCK_EMP = EcnLockEmp,
                            CREATETIME = DateTime.Now,
                            CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                        }).ExecuteCommand();
                    }
                }
                #endregion
                #region NOT DOF 
                else
                {
                    var wosrev = db.Queryable<R_WO_BASE>()
                       .Where(t => t.SKUNO == main.ITEMNUMBER && t.SKU_VER == rev && t.WORKORDER_QTY != t.FINISHED_QTY &&
                       !SqlFunc.Subqueryable<R_SN_LOCK>().Where(l => l.WORKORDERNO == t.WORKORDERNO && l.TYPE == LockType.WorkOrderNo.ExtValue() && l.LOCK_EMP == EcnLockEmp).Any()).Distinct().ToList();
                    foreach (var woobj in wosrev)
                    {
                        db.Insertable(new R_SN_LOCK()
                        {
                            ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                            WORKORDERNO = woobj.WORKORDERNO,
                            LOCK_STATION = LockStation.ALL.ExtValue(),
                            TYPE = LockType.WorkOrderNo.ExtValue(),
                            SN = rev,
                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                            LOCK_REASON = $@"Ecn Lock--EcnNo:{main.CHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING}",
                            LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                            LOCK_EMP = EcnLockEmp,
                            CREATETIME = DateTime.Now,
                            CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                        }).ExecuteCommand();
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// Skuno -Stock=>lock sn
        /// </summary>
        /// <param name="revs"></param>
        /// <param name="main"></param>
        void LockSkuStock(List<string> revs, R_JNP_ECNPAGE main)
        {
            var isdof = db.Queryable<O_ORDER_MAIN>().Any(t => t.PID == main.ITEMNUMBER);
            foreach (var rev in revs)
            {
                #region DOF 
                if (isdof)
                {
                    var snsrev = db.Queryable<R_SAP_AS_BOM, R_WO_BASE, R_SN>((s, w, r) => s.WO == w.WORKORDERNO && w.WORKORDERNO == r.WORKORDERNO)
                        .Where((s, w, r) => w.WORKORDER_QTY != w.FINISHED_QTY && r.COMPLETED_FLAG == MesBool.Yes.ExtValue() && r.SHIPPED_FLAG == MesBool.No.ExtValue() &&
                        !SqlFunc.Subqueryable<R_SN_LOCK>().Where(t => t.SN == r.SN && t.TYPE == LockType.Sn.ExtValue() && t.LOCK_EMP == EcnLockEmp).Any() &&
                        ((s.PARENTPN == main.ITEMNUMBER && s.PARENTREV == rev) || (s.PN == main.ITEMNUMBER && s.PNREV == rev))).Select((s, w, r) => r.SN).Distinct().ToList();
                    foreach (var snobj in snsrev)
                    {
                        db.Insertable(new R_SN_LOCK()
                        {
                            ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                            LOCK_STATION = LockStation.ALL.ExtValue(),
                            TYPE = LockType.Sn.ExtValue(),
                            SN = snobj,
                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                            LOCK_REASON = $@"Ecn Lock--EcnNo:{main.CHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING}",
                            LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                            LOCK_EMP = EcnLockEmp,
                            CREATETIME = DateTime.Now,
                            CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                        }).ExecuteCommand();
                    }
                }
                #endregion
                #region NOT DOF 
                else
                {
                    var snsrev = db.Queryable<R_WO_BASE, R_SN>((w, r) => w.WORKORDERNO == r.WORKORDERNO)
                       .Where((w, r) => w.SKUNO == main.ITEMNUMBER && w.SKU_VER == rev && w.WORKORDER_QTY != w.FINISHED_QTY && r.COMPLETED_FLAG == MesBool.Yes.ExtValue() && r.SHIPPED_FLAG == MesBool.No.ExtValue() &&
                        !SqlFunc.Subqueryable<R_SN_LOCK>().Where(l => l.SN == r.SN && l.TYPE == LockType.Sn.ExtValue() && l.LOCK_EMP == EcnLockEmp).Any()).Select((w, r) => r.SN).Distinct().ToList();
                    foreach (var snobj in snsrev)
                    {
                        db.Insertable(new R_SN_LOCK()
                        {
                            ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                            LOCK_STATION = LockStation.ALL.ExtValue(),
                            TYPE = LockType.Sn.ExtValue(),
                            SN = snobj,
                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                            LOCK_REASON = $@"Ecn Lock--EcnNo:{main.CHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING}",
                            LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                            LOCK_EMP = EcnLockEmp,
                            CREATETIME = DateTime.Now,
                            CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                        }).ExecuteCommand();
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// Skuno - has been keypart not ship=>lock sn
        /// </summary>
        /// <param name="revs"></param>
        /// <param name="main"></param>
        void LockSkuKpWithNotShip(List<string> revs, R_JNP_ECNPAGE main)
        {
            var isdof = db.Queryable<O_ORDER_MAIN>().Any(t => t.PID == main.ITEMNUMBER);
            foreach (var rev in revs)
            {
                #region NOT DOF 
                if (!isdof)
                {
                    var snsrev = db.Queryable<R_WO_BASE, R_SN, R_SN_KP, R_SN>((w, r, k, r2) => w.WORKORDERNO == r.WORKORDERNO && r.SN == k.VALUE && k.SN == r2.SN)
                       .Where((w, r, k, r2) => w.SKUNO == main.ITEMNUMBER && w.SKU_VER == rev && w.WORKORDER_QTY != w.FINISHED_QTY && r2.SHIPPED_FLAG == MESDataObject.Constants.PublicConstants.MesBool.No.ExtValue() && k.VALID_FLAG == 1 &&
                        !SqlFunc.Subqueryable<R_SN_LOCK>().Where(l => l.SN == r2.SN && l.TYPE == LockType.Sn.ExtValue() && l.LOCK_EMP == EcnLockEmp).Any()).Select((w, r, k, r2) => r2.SN).Distinct().ToList();
                    snsrev.AddRange(db.Queryable<R_WO_BASE, R_SN, R_SN_KP, R_SN_KP, R_SN>((w, r, k, k2, r2) => w.WORKORDERNO == r.WORKORDERNO && r.SN == k.VALUE && k.SN == k2.VALUE && k2.SN == r2.SN)
                       .Where((w, r, k, k2, r2) => w.SKUNO == main.ITEMNUMBER && w.SKU_VER == rev && w.WORKORDER_QTY != w.FINISHED_QTY && r2.SHIPPED_FLAG == MesBool.No.ExtValue() && k.VALID_FLAG == 1 && k2.VALID_FLAG == 1 &&
                        !SqlFunc.Subqueryable<R_SN_LOCK>().Where(l => l.SN == r2.SN && l.TYPE == LockType.Sn.ExtValue() && l.LOCK_EMP == EcnLockEmp).Any()).Select((w, r, k, k2, r2) => r2.SN).Distinct().ToList());
                    snsrev = snsrev.Distinct().ToList();
                    foreach (var snobj in snsrev)
                    {
                        db.Insertable(new R_SN_LOCK()
                        {
                            ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                            LOCK_STATION = LockStation.ALL.ExtValue(),
                            TYPE = LockType.Sn.ExtValue(),
                            SN = snobj,
                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                            LOCK_REASON = $@"Ecn Lock--EcnNo:{main.CHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING}",
                            LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                            LOCK_EMP = EcnLockEmp,
                            CREATETIME = DateTime.Now,
                            CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                        }).ExecuteCommand();
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// Components -Wip=>lock wo
        /// </summary>
        /// <param name="revs"></param>
        /// <param name="main"></param>
        void LockCmpWip(List<string> revs, R_JNP_ECNPAGE main)
        {
            foreach (var rev in revs)
            {
                var crev = main.REV.Substring(0, 2);
                var wosrev = db.Queryable<R_SAP_AS_BOM, R_WO_BASE>((s, w) => s.WO == w.WORKORDERNO)
                       .Where((s, w) => w.WORKORDER_QTY != w.FINISHED_QTY &&
                       //!SqlFunc.Subqueryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == w.WORKORDERNO && t.TYPE == LockType.WorkOrderNo.ExtValue() && t.LOCK_EMP == EcnLockEmp).Any() &&
                       ((s.PARENTPN == main.ITEMNUMBER && s.PARENTREV == rev) || (s.PN == main.ITEMNUMBER && s.PNREV == rev))).Select((s, w) => w).Distinct().ToList();
                foreach (var woobj in wosrev)
                {
                    var cl = db.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == woobj.WORKORDERNO && t.TYPE == LockType.WorkOrderNo.ExtValue() && t.LOCK_EMP == EcnLockEmp && t.LOCK_STATUS == MesBool.Yes.ExtValue() && t.LOCK_LOT == crev).ToList().FirstOrDefault();
                    if (cl != null)
                    {
                        cl.LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE);
                        db.Updateable(cl).ExecuteCommand();
                    }
                    else
                        db.Insertable(new R_SN_LOCK()
                        {
                            ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                            LOCK_LOT = crev,
                            WORKORDERNO = woobj.WORKORDERNO,
                            LOCK_STATION = LockStation.ALL.ExtValue(),
                            TYPE = LockType.WorkOrderNo.ExtValue(),
                            SN = rev,
                            LOCK_STATUS = MesBool.Yes.ExtValue(),
                            LOCK_REASON = $@"Ecn Lock(Lock By Wo:{woobj.WORKORDERNO}).Reason:EcnNo:{main.CHANGENO},Eco:{main.CUSTCHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING},The Current Version is {rev},Pls Check!",
                            LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                            LOCK_EMP = EcnLockEmp,
                            CREATETIME = DateTime.Now,
                            CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                        }).ExecuteCommand();
                }
                //#region NOT DOF 
                //var wosrev = db.Queryable<R_WO_ITEM, R_WO_BASE>((d, w) => d.AUFNR == w.WORKORDERNO)
                //   .Where((d, w) => d.MATNR == main.ITEMNUMBER && d.REVLV == rev && w.WORKORDER_QTY != w.FINISHED_QTY &&
                //   !SqlFunc.Subqueryable<R_SN_LOCK>().Where(l => l.WORKORDERNO == w.WORKORDERNO && l.TYPE == LockType.WorkOrderNo.ExtValue() && l.LOCK_EMP == EcnLockEmp).Any()).Select((d, w) => w).Distinct().ToList();
                //foreach (var woobj in wosrev)
                //{
                //    db.Insertable(new R_SN_LOCK()
                //    {
                //        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                //        WORKORDERNO = woobj.WORKORDERNO,
                //        LOCK_STATION = LockStation.ALL.ExtValue(),
                //        TYPE = LockType.WorkOrderNo.ExtValue(),
                //        SN = rev,
                //        LOCK_STATUS = MesBool.Yes.ExtValue(),
                //        LOCK_REASON = $@"Ecn Lock--EcnNo:{main.CHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING}",
                //        LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                //        LOCK_EMP = EcnLockEmp,
                //        CREATETIME = DateTime.Now,
                //        CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                //    }).ExecuteCommand();
                //}
                //#endregion
            }
        }
        /// <summary>
        /// Components -Stock=>lock sn
        /// </summary>
        /// <param name="revs"></param>
        /// <param name="main"></param>
        void LockCmpStock(List<string> revs, R_JNP_ECNPAGE main)
        {
            foreach (var rev in revs)
            {
                #region NOT DOF               
                var snsrev = db.Queryable<R_WO_ITEM, R_WO_BASE, R_SN>((d, w, r) => d.AUFNR == w.WORKORDERNO && w.WORKORDERNO == r.WORKORDERNO)
                   .Where((d, w, r) => d.MATNR == main.ITEMNUMBER && d.REVLV == rev && w.WORKORDER_QTY != w.FINISHED_QTY && r.COMPLETED_FLAG == MesBool.Yes.ExtValue() && r.SHIPPED_FLAG == MesBool.No.ExtValue() &&
                    !SqlFunc.Subqueryable<R_SN_LOCK>().Where(l => l.SN == r.SN && l.TYPE == LockType.Sn.ExtValue() && l.LOCK_EMP == EcnLockEmp).Any()).Select((d, w, r) => r.SN).Distinct().ToList();
                foreach (var snobj in snsrev)
                {
                    db.Insertable(new R_SN_LOCK()
                    {
                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                        LOCK_STATION = LockStation.ALL.ExtValue(),
                        TYPE = LockType.Sn.ExtValue(),
                        SN = snobj,
                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                        LOCK_REASON = $@"Ecn Lock--EcnNo:{main.CHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING}",
                        LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                        LOCK_EMP = EcnLockEmp,
                        CREATETIME = DateTime.Now,
                        CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                    }).ExecuteCommand();
                }
            }
            #endregion
        }
        /// <summary>
        /// Components - has been keypart not ship=>lock sn
        /// </summary>
        /// <param name="revs"></param>
        /// <param name="main"></param>
        void LockCmpWithNotShip(List<string> revs, R_JNP_ECNPAGE main)
        {
            foreach (var rev in revs)
            {
                #region NOT DOF 
                var snsrev = db.Queryable<R_WO_ITEM, R_WO_BASE, R_SN, R_SN_KP, R_SN>((d, w, r, k, r2) => d.AUFNR == w.WORKORDERNO && w.WORKORDERNO == r.WORKORDERNO && r.SN == k.VALUE && k.SN == r2.SN)
                   .Where((d, w, r, k, r2) => d.MATNR == main.ITEMNUMBER && d.REVLV == rev && w.WORKORDER_QTY != w.FINISHED_QTY && r2.SHIPPED_FLAG == MesBool.No.ExtValue() && k.VALID_FLAG == 1 &&
                    !SqlFunc.Subqueryable<R_SN_LOCK>().Where(l => l.SN == r2.SN && l.TYPE == LockType.Sn.ExtValue() && l.LOCK_EMP == EcnLockEmp).Any()).Select((d, w, r, k, r2) => r2.SN).Distinct().ToList();
                snsrev.AddRange(db.Queryable<R_WO_ITEM, R_WO_BASE, R_SN, R_SN_KP, R_SN_KP, R_SN>((d, w, r, k, k2, r2) => d.AUFNR == w.WORKORDERNO && w.WORKORDERNO == r.WORKORDERNO && r.SN == k.VALUE && k.SN == k2.VALUE && k2.SN == r2.SN)
                   .Where((d, w, r, k, k2, r2) => d.MATNR == main.ITEMNUMBER && d.REVLV == rev && w.WORKORDER_QTY != w.FINISHED_QTY && r2.SHIPPED_FLAG == MesBool.No.ExtValue() && k.VALID_FLAG == 1 && k2.VALID_FLAG == 1 &&
                    !SqlFunc.Subqueryable<R_SN_LOCK>().Where(l => l.SN == r2.SN && l.TYPE == LockType.Sn.ExtValue() && l.LOCK_EMP == EcnLockEmp).Any()).Select((d,w, r, k, k2, r2) => r2.SN).Distinct().ToList());
                snsrev = snsrev.Distinct().ToList();
                foreach (var snobj in snsrev)
                {
                    db.Insertable(new R_SN_LOCK()
                    {
                        ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
                        LOCK_STATION = LockStation.ALL.ExtValue(),
                        TYPE = LockType.Sn.ExtValue(),
                        SN = snobj,
                        LOCK_STATUS = MesBool.Yes.ExtValue(),
                        LOCK_REASON = $@"Ecn Lock--EcnNo:{main.CHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING}",
                        LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
                        LOCK_EMP = EcnLockEmp,
                        CREATETIME = DateTime.Now,
                        CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
                    }).ExecuteCommand();
                }
                #endregion
            }
        }
        //void TrigerDofWoReganere(List<string> revs, R_JNP_ECNPAGE main)
        //{
        //    foreach (var rev in revs)
        //    {
        //        var crev = main.REV.Substring(0, 2);
        //        var wosrev = db.Queryable<R_SAP_AS_BOM, O_ORDER_MAIN, O_PO_STATUS>((s, m, p) => s.WO == m.PREWO && m.ID == p.POID)
        //               .Where((s, m, p) => m.CANCEL == MesBool.No.ExtValue() && p.VALIDFLAG == MesBool.Yes.ExtValue() && 
        //               (p.STATUSID == ENUM_O_PO_STATUS.CreateWo.ExtValue() || p.STATUSID == ENUM_O_PO_STATUS.DownloadWo.ExtValue()) &&
        //               !SqlFunc.Subqueryable<R_WO_BASE>().Where(t => t.WORKORDERNO == m.PREWO).Any() &&
        //               ((s.PARENTPN == main.ITEMNUMBER && s.PARENTREV == rev) || (s.PN == main.ITEMNUMBER && s.PNREV == rev))).Select((s, w) => w).Distinct().ToList();
        //        foreach (var woobj in wosrev)
        //        {
        //            var cl = db.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == woobj.WORKORDERNO && t.TYPE == LockType.WorkOrderNo.ExtValue() && t.LOCK_EMP == EcnLockEmp && t.LOCK_STATUS == MesBool.Yes.ExtValue() && t.LOCK_LOT == crev).ToList().FirstOrDefault();
        //            if (cl != null)
        //            {
        //                cl.LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE);
        //                db.Updateable(cl).ExecuteCommand();
        //            }
        //            else
        //                db.Insertable(new R_SN_LOCK()
        //                {
        //                    ID = MesDbBase.GetNewID<R_SN_LOCK>(db, bu),
        //                    LOCK_LOT = crev,
        //                    WORKORDERNO = woobj.WORKORDERNO,
        //                    LOCK_STATION = LockStation.ALL.ExtValue(),
        //                    TYPE = LockType.WorkOrderNo.ExtValue(),
        //                    SN = rev,
        //                    LOCK_STATUS = MesBool.Yes.ExtValue(),
        //                    LOCK_REASON = $@"Ecn Lock(Lock By Wo:{woobj.WORKORDERNO}).Reason:EcnNo:{main.CHANGENO},Eco:{main.CUSTCHANGENO},PN:{main.ITEMNUMBER},Rev:{main.REV},Ecoreporting:{main.ECOREPORTING},The Current Version is {rev},Pls Check!",
        //                    LOCK_TIME = Convert.ToDateTime(main.CUSTREQUIRDATE),
        //                    LOCK_EMP = EcnLockEmp,
        //                    CREATETIME = DateTime.Now,
        //                    CREATEBY = MES_CONST_SYSTEM.System.ExtValue()
        //                }).ExecuteCommand();
        //        }
        //    }
        //}

    }
}
