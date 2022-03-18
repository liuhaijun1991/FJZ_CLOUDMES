using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDataObject.Common;
using MESDataObject.Module.ORACLE;
using SqlSugar;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESPubLab.SAP_RFC;
using System.Configuration;
using MESPubLab.MESInterface;
using MESDataObject.Module.DCN;
using MESDataObject.Constants;
using static MESDataObject.Constants.PublicConstants;

namespace MESStation.Management
{
    public class Reverse : MesAPIBase
    {
        private APIInfo FReverseWO = new APIInfo()
        {
            FunctionName = "ReverseWO",
            Description = "Reverse Work Order",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Keep_WO", InputType = "SELECT", DefaultValue = "NO,YES" },
                new APIInputInfo() {InputName = "WO", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo FReverseSN = new APIInfo()
        {
            FunctionName = "ReverseSN",
            Description = "Reverse SN",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        public Reverse()
        {
            this.Apis.Add(FReverseWO.FunctionName, FReverseWO);
            this.Apis.Add(FReverseSN.FunctionName, FReverseSN);
        }

        public void ReverseWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string KeepWo = Data["Keep_WO"].ToString().Trim();
            string Wo = Data["WO"].ToString().Trim();
            OleExec _db = this.DBPools["SFCDB"].Borrow();
            _db.PoolBorrowTimeOut = 3600;
            var wobase = _db.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == Wo).First();
            try
            {
                if (!BU.Equals("VNJUNIPER"))
                    BackFlushCheck(_db);
                if (wobase != null)
                {
                    CheckPreASN(Wo, _db.ORM);
                    CheckFinalASN(Wo, _db.ORM);
                    var snList = _db.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == Wo && t.VALID_FLAG == "1").ToList();
                    if (snList.Where(t => t.SHIPPED_FLAG == "1").Any())
                    {
                        throw new Exception("Some unit has been shiped,can't reverse!Please check [R_SN.SHIPPED_FLAG]");
                    }
                    var sns = snList.Select(t => t.SN).ToList();

                    if (!BU.Equals("VNJUNIPER"))
                        WOReverseSAP(Wo, _db.ORM);

                    ReverseBySNList(sns, KeepWo, _db.ORM);

                    TryChangePOStatus(Wo, KeepWo, _db.ORM);

                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "WO does not exist,Please check [R_WO_BASE]";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = e.Message;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(_db);
            }
        }

        public void ReverseSN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string sn = Data["SN"].ToString().Trim();
            OleExec _db = this.DBPools["SFCDB"].Borrow(); ;
            var snobj = _db.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").First();
            try
            {
                if (!BU.Equals("VNJUNIPER"))
                    BackFlushCheck(_db);
                if (snobj != null)
                {
                    if (snobj.SHIPPED_FLAG == "1")
                    {
                        throw new Exception("This unit has been shiped,can't reverse!Please check [R_SN.SHIPPED_FLAG]");
                    }
                    CheckPreASN(snobj.WORKORDERNO, _db.ORM);
                    CheckFinalASN(snobj.WORKORDERNO, _db.ORM);
                    if (snobj.PACKED_FLAG == "1")
                    {
                        var snp = _db.ORM.Queryable<R_SN_PACKING>().Where(t => t.SN_ID == snobj.ID).First();
                        var snpp = _db.ORM.Queryable<R_SN_PACKING>().Where(t => t.PACK_ID == snp.PACK_ID).ToList();
                        var cts = _db.ORM.Queryable<R_PACKING, R_PACKING>((p, pp) => p.PARENT_PACK_ID == pp.PARENT_PACK_ID)
                            .Where((p, pp) => pp.ID == snp.PACK_ID && p.QTY > 0)
                            .Select((p, pp) => p)
                            .ToList();
                        if (snpp.Count > 1 || cts.Count > 1)
                        {
                            throw new Exception("This unit has been packaged with other units, please use the MOVE PALLET tool to move the unit to the single package!");
                        }
                    }
                    if (!BU.Equals("VNJUNIPER"))
                        SNReverseSAP(sn, _db.ORM);

                    ReverseBySNList(new List<string>() { sn }, "NO", _db.ORM);

                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "OK";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "SN does not exist,Please check [R_SN]";
                    StationReturn.Data = "";
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = e.Message;
                StationReturn.Data = "";
            }
            finally
            {
                this.DBPools["SFCDB"].Return(_db);
            }
        }

        private void ReverseBySNList(List<string> SN, string keepWo, SqlSugarClient _db)
        {
            var sns = _db.Queryable<R_SN>().Where(t => SN.Contains(t.SN) && t.VALID_FLAG == "1").ToList();
            var snids = sns.Select(t => t.ID).ToList();
            _db.Ado.BeginTran();
            try
            {
                #region Reverse Station Detail
                var detail = _db.Queryable<R_SN_STATION_DETAIL>().Where(t => snids.Contains(t.R_SN_ID)).ToList();
                for (int i = 0; i < detail.Count; i++)
                {
                    detail[i].STATION_NAME = "REVERSE_" + detail[i].STATION_NAME;
                    detail[i].VALID_FLAG = "0";
                    _db.Updateable(detail[i]).ExecuteCommand();
                }
                #endregion

                #region Reverse Key Part & Last Level SN
                var kp = _db.Queryable<R_SN_KP>().Where(t => snids.Contains(t.R_SN_ID)).ToList();
                var csn_kp = kp.Where(t => t.SCANTYPE == "SystemSN").Select(t => t.VALUE).ToList();
                var keep_kp = kp.Where(t => t.KP_NAME == "KEEP_SN").ToList();
                for (int i = 0; i < keep_kp.Count; i++)
                {
                    var keep = _db.Queryable<R_SN>().Where(t => t.ID == keep_kp[i].EXVALUE1).First();
                    keep.VALID_FLAG = "1";
                    keep.SHIPPED_FLAG = "0";
                    _db.Updateable(keep).ExecuteCommand();
                }

                var csn = _db.Queryable<R_SN>().Where(t => (csn_kp.Contains(t.SN) && t.VALID_FLAG == "1")).ToList();
                for (int i = 0; i < csn.Count; i++)
                {
                    csn[i].SHIPPED_FLAG = "0";
                    _db.Updateable(csn[i]).ExecuteCommand();
                }


                for (int i = 0; i < kp.Count; i++)
                {
                    kp[i].STATION = "REVERSE_" + kp[i].STATION;
                    kp[i].VALID_FLAG = 0;
                    _db.Updateable(kp[i]).ExecuteCommand();
                }
                #endregion

                #region Reverse Packout
                var snp = _db.Queryable<R_SN_PACKING>().Where(t => snids.Contains(t.SN_ID)).ToList();
                var CTCount = snp.GroupBy(t => new { t.PACK_ID })
                    .Select(nr => new
                    {
                        PACK_ID = nr.Key.PACK_ID,
                        Qty = nr.Count()
                    }).ToList();
                var cids = snp.Select(t => t.PACK_ID).ToList();
                var cartons = _db.Queryable<R_PACKING>().Where(t => cids.Contains(t.ID)).ToList();
                for (int i = 0; i < cartons.Count; i++)
                {
                    var qty = CTCount.Find(t => t.PACK_ID == cartons[i].ID).Qty;
                    cartons[i].QTY = cartons[i].QTY - qty;
                }
                var pids = cartons.Select(t => t.PARENT_PACK_ID).Distinct().ToList();
                var pallets = _db.Queryable<R_PACKING>().Where(t => pids.Contains(t.ID)).ToList();
                for (int i = 0; i < pallets.Count; i++)
                {
                    var qty = cartons.FindAll(t => t.PARENT_PACK_ID == pallets[i].ID && t.QTY == 0).Count;
                    pallets[i].QTY = pallets[i].QTY - qty;
                }
                if (snp.Count > 0)
                {
                    _db.Deleteable(snp).ExecuteCommand();
                }

                var cartonNotNull = cartons.FindAll(t => t.QTY != 0);
                var cartonNull = cartons.FindAll(t => t.QTY == 0);
                if (cartonNotNull.Count > 0)
                {
                    _db.Updateable(cartonNotNull).ExecuteCommand();
                }
                if (cartonNull.Count > 0)
                {
                    _db.Deleteable(cartonNull).ExecuteCommand();
                }

                var palletNotNull = pallets.FindAll(t => t.QTY != 0);
                var palletNull = pallets.FindAll(t => t.QTY == 0);
                if (palletNotNull.Count > 0)
                {
                    _db.Updateable(palletNotNull).ExecuteCommand();
                }
                if (palletNull.Count > 0)
                {
                    _db.Deleteable(palletNull).ExecuteCommand();
                }
                #endregion

                #region Reverse SN
                for (int i = 0; i < sns.Count; i++)
                {
                    sns[i].VALID_FLAG = "0";
                    sns[i].PRODUCT_STATUS = "REVERSE";
                    sns[i].NEXT_STATION = "REVERSE_" + sns[i].NEXT_STATION;
                    _db.Updateable(sns[i]).ExecuteCommand();
                }
                #endregion

                #region Update WO Input Qty
                var wos = sns.Select(t => t.WORKORDERNO).ToList();
                var wo = _db.Queryable<R_WO_BASE>().Where(t => wos.Contains(t.WORKORDERNO)).ToList();
                for (int i = 0; i < wo.Count; i++)
                {
                    var wono = wo[i].WORKORDERNO;
                    var qty = sns.Where(t => t.WORKORDERNO == wono).Count();
                    wo[i].INPUT_QTY = wo[i].INPUT_QTY - qty;

                    //If need to keep this workorderno, update this 2 cloumns for re-siloading 
                    if (keepWo.Equals("YES"))
                    {
                        wo[i].FINISHED_QTY = wo[i].FINISHED_QTY - qty;
                        wo[i].CLOSED_FLAG = "0";
                    }
                    _db.Updateable(wo[i]).ExecuteCommand();
                }
                #endregion

                _db.Ado.CommitTran();

            }
            catch (Exception ex)
            {
                _db.Ado.RollbackTran();
                throw ex;
            }
        }

        private void BackFlushCheck(OleExec _db)
        {
            if (InterfacePublicValues.IsMonthly(_db, MESDataObject.DB_TYPE_ENUM.Oracle))
            {
                throw new Exception("This Time Is Monthly,Can't Call SAP Reverse!");
            }

            var TR_SYNC_LOCK = new T_R_SYNC_LOCK(_db, MESDataObject.DB_TYPE_ENUM.Oracle);
            string lockIP = "";
            bool isLock = TR_SYNC_LOCK.IsLock("BACKFLUSH", _db, MESDataObject.DB_TYPE_ENUM.Oracle, out lockIP);
            if (isLock)
            {
                throw new Exception("BackFlush is running,please try again later!");
            }
        }

        private void TryChangePOStatus(string wo, string keepWo, SqlSugarClient _db)
        {
            var omain = _db.Queryable<R_PRE_WO_HEAD, O_ORDER_MAIN>((p, o) => p.PONO == o.PONO && p.POLINE == o.POLINE).Where((p, o) => p.WO == wo).Select((p, o) => o).First();
            if (omain == null)
            {
                throw new Exception("Only DOF can be reversed!");
            }
            var postatus = _db.Queryable<O_PO_STATUS>().Where(t => t.POID == omain.ID && t.VALIDFLAG == MesBool.Yes.ExtValue() && t.STATUSID == ENUM_O_PO_STATUS.WaitDismantle.ExtValue()).First();
            if (postatus != null && omain.CANCEL == MesBool.No.ExtValue())
            {
                _db.Ado.BeginTran();
                postatus.VALIDFLAG = MesBool.No.ExtValue();
                _db.Updateable(postatus).ExecuteCommand();
                var cpostatus = new O_PO_STATUS()
                {
                    ID = MesDbBase.GetNewID<O_PO_STATUS>(_db, Customer.JUNIPER.ExtValue()),
                    //STATUSID = ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue(),
                    STATUSID = keepWo.Equals("YES") ? ENUM_O_PO_STATUS.Production.ExtValue() : ENUM_O_PO_STATUS.WaitCreatePreWo.ExtValue(),//If need to keep this workorderno, update po status to Production
                    VALIDFLAG = MesBool.Yes.ExtValue(),
                    CREATETIME = DateTime.Now,
                    EDITTIME = DateTime.Now,
                    POID = omain.ID
                };
                _db.Insertable(cpostatus).ExecuteCommand();
                _db.Ado.CommitTran();
            }
        }

        private void WOReverseSAP(string wo, SqlSugarClient _db)
        {
            var his = _db.Queryable<R_BACKFLUSH_HISTORY>()
                .Where(t => t.WORKORDERNO == wo && t.RESULT == "Y" && !t.TOSAP.StartsWith("REVER"))
                .OrderBy(t => t.SAP_STATION, OrderByType.Desc)
                .OrderBy(t => t.BACK_DATE, OrderByType.Desc)
                .ToList();
            if (his.Where(t => SqlFunc.IsNullOrEmpty(t.TOSAP)).Any())
            {
                throw new Exception("SAP Reverse missing confirm head text!");
            }
            ZCPP_NSBG_0280 rfc = new ZCPP_NSBG_0280(BU);
            var plant = ConfigurationManager.AppSettings[BU + "_SAP_Plant"];
            for (int i = 0; i < his.Count; i++)
            {
                rfc.ClearValues();
                try
                {
                    rfc.SetValue(his[i].WORKORDERNO, his[i].SAP_STATION, his[i].DIFF_QTY.ToString(), plant, his[i].TOSAP);
                    rfc.CallRFC();
                    if (rfc.GetValue("O_FLAG") == "1")
                    {
                        var msg = rfc.GetValue("O_MESSAGE");
                        throw new Exception("SAP Reverse Fail:" + msg);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                his[i].SFC_STATION = "REVERSE_" + his[i].SFC_STATION;
                his[i].TOSAP = his[i].TOSAP.Replace("CONFT", "REVER");
                _db.Updateable(his[i]).ExecuteCommand();
            }
        }

        private void SNReverseSAP(string sn, SqlSugarClient _db)
        {
            var snobj = _db.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1" && t.SHIPPED_FLAG == "0").First();
            var sap = _db.Queryable<C_SAP_STATION_MAP>().Where(t => t.SKUNO == snobj.SKUNO).OrderBy(t => t.SAP_STATION_CODE, OrderByType.Desc).ToList();

            ZCPP_NSBG_0280 rfc = new ZCPP_NSBG_0280(BU);
            var plant = ConfigurationManager.AppSettings[BU + "_SAP_Plant"];
            for (int i = 0; i < sap.Count; i++)
            {
                var sndetail = _db.Queryable<R_SN_STATION_DETAIL>().Where(t => t.R_SN_ID == snobj.ID && t.STATION_NAME == sap[i].STATION_NAME && t.REPAIR_FAILED_FLAG == "0").First();
                if (sndetail == null)
                {
                    continue;
                }
                var his = _db.Queryable<R_BACKFLUSH_HISTORY>()
                    .Where(t => t.WORKORDERNO == snobj.WORKORDERNO && t.RESULT == "Y" && t.SFC_STATION == sap[i].STATION_NAME)
                    .OrderBy(t => t.BACK_DATE, OrderByType.Desc)
                    .First();
                if (his == null || sndetail.EDIT_TIME > his.BACK_DATE)
                {
                    continue;
                }
                rfc.ClearValues();
                try
                {
                    rfc.SetValue(his.WORKORDERNO, his.SAP_STATION, his.DIFF_QTY.ToString(), plant, his.TOSAP);
                    rfc.CallRFC();
                    if (rfc.GetValue("O_FLAG") == "1")
                    {
                        var msg = rfc.GetValue("O_MESSAGE");
                        throw new Exception("SAP Reverse Fail:" + msg);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                his.SFC_STATION = "REVERSE_" + his.SFC_STATION;
                his.TOSAP = his.TOSAP.Replace("CONFT", "REVER");
                _db.Updateable(his).Where(t => t.WORKORDERNO == his.WORKORDERNO && t.SFC_STATION == sap[i].STATION_NAME
                && t.RESULT == "Y" && t.BACK_DATE == his.BACK_DATE).ExecuteCommand();
            }

        }

        private void CheckPreASN(string WO, SqlSugarClient _db)
        {
            var omain = _db.Queryable<R_PRE_WO_HEAD, O_ORDER_MAIN>((p, o) => p.PONO == o.PONO && p.POLINE == o.POLINE).Where((p, o) => p.WO == WO).Select((p, o) => o).First();
            if (omain == null)
            {
                throw new Exception("Only DOF can be reversed!");
            }
            if (omain.PREASN != "0")
            {
                throw new Exception("Please Cancel PreASN First!");
            }
            var i139 = _db.Queryable<R_PRE_WO_HEAD, R_I139>((p, o) => p.POLINE == o.PONUMBER && p.PONO == o.ITEM)
                .Where((p, o) => p.WO == WO && o.ASNNUMBER.StartsWith("PRESHIP_"))
                .OrderBy((p, o) => o.CREATETIME, OrderByType.Desc)
                .Select((p, o) => o)
                .First();
            if (i139 != null && i139.DELIVERYCODE != "03")
            {
                throw new Exception("Please Cancel PreASN First!");
            }
        }

        private void CheckFinalASN(string WO, SqlSugarClient _db)
        {
            var omain = _db.Queryable<R_PRE_WO_HEAD, O_ORDER_MAIN>((p, o) => p.PONO == o.PONO && p.POLINE == o.POLINE).Where((p, o) => p.WO == WO).Select((p, o) => o).First();
            if (omain.FINALASN != "0")
            {
                throw new Exception("Can't Reverse After Final ASN!");
            }
            var i139 = _db.Queryable<R_PRE_WO_HEAD, R_I139>((p, o) => p.POLINE == o.PONUMBER && p.PONO == o.ITEM)
                .Where((p, o) => p.WO == WO && o.ASNNUMBER.StartsWith("ACTUAL_"))
                .OrderBy((p, o) => o.CREATETIME, OrderByType.Desc)
                .Select((p, o) => o)
                .First();
            if (i139 != null && i139.DELIVERYCODE != "03")
            {
                throw new Exception("Can't Reverse After Final ASN!");
            }
        }

    }
}
