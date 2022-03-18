using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESPubLab.MESInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json.Linq;
using MESPubLab;
using MESDataObject.Module.Juniper;
using MESPubLab.MESStation.MESReturnView.Station;
using MESDataObject;
using MESPubLab.SAP_RFC;
using System.Text.RegularExpressions;
using MESDataObject.Module.OM;
using static MESDataObject.Constants.PublicConstants;
using MESDataObject.Common;
using SqlSugar;

namespace MESJuniper.Api
{
    public class ShipoutConfirm : MesAPIBase
    {
        protected APIInfo FGetDataList = new APIInfo
        {
            FunctionName = "GetDataList",
            Description = "GetDataList",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetDataDetail = new APIInfo
        {
            FunctionName = "GetDataDetai",
            Description = "GetDataDetail",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo(){ InputName="TO_NO",InputType="String"}
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FConfirmFinalASN = new APIInfo
        {
            FunctionName = "ConfirmFinalASN",
            Description = "ConfirmFinalASN",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName="ID", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FConfirmGT = new APIInfo
        {
            FunctionName = "ConfirmGT",
            Description = "ConfirmGT",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName="ID", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetPreAsnSoPoList = new APIInfo
        {
            FunctionName = "GetPreAsnSoPoList",
            Description = "GetPreAsnSoPoList",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName="PO", InputType="string" },
                 new APIInputInfo(){ InputName="ITEM", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        public ShipoutConfirm()
        {
            this.Apis.Add(FGetDataList.FunctionName, FGetDataList);
            this.Apis.Add(FGetDataDetail.FunctionName, FGetDataDetail);
            this.Apis.Add(FConfirmFinalASN.FunctionName, FConfirmFinalASN);
            this.Apis.Add(FConfirmGT.FunctionName, FConfirmGT);
            this.Apis.Add(FGetPreAsnSoPoList.FunctionName, FGetPreAsnSoPoList);
        }

        public void Confirm(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            //OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            //var _db = this.DBPools["SFCDB"].Borrow();
            //var TO_NO = "";
            //var PLANT = "";
            //var FinalASN_FLAG = false;
            //var ShipoutGT_FLAG = false;
            //var asnmsg = "";
            //var gtmsg = "";
            //try
            //{
            //    string ID = Data["ID"].ToString().Trim();
            //    #region FinalASN
            //    try
            //    {
            //        string sql = string.Format(@"
            //                            SELECT *
            //                              FROM R_JUNIPER_TRUCKLOAD_TO
            //                             WHERE CLOSED = 1
            //                               AND ID='{0}'
            //                               AND NOT EXISTS (SELECT *
            //                                      FROM R_MES_LOG
            //                                     WHERE TO_NO = DATA1
            //                                       AND PROGRAM_NAME ='ShipoutFinalASN'
            //                                       AND DATA4 = 'Y')"
            //            , ID);
            //        var res = SFCDB.ORM.Ado.SqlQuery<R_JUNIPER_TRUCKLOAD_TO>(sql)
            //                              .ToList()
            //                              .FirstOrDefault();
            //        SFCDB.BeginTrain();
            //        if (res != null)
            //        {
            //            TO_NO = res.TO_NO;
            //            FinalASN(TO_NO, SFCDB);
            //            FinalASN_FLAG = true;
            //            asnmsg = "FinalASN:OK!";
            //        }
            //        SFCDB.CommitTrain();
            //    }
            //    catch (Exception ex)
            //    {
            //        SFCDB.RollbackTrain();
            //        R_MES_LOG log = new R_MES_LOG()
            //        {
            //            ID = MesDbBase.GetNewID<R_MES_LOG>(_db.ORM, this.BU),
            //            PROGRAM_NAME = "ShipoutFinalASN",
            //            CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
            //            FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
            //            MAILFLAG = "N",
            //            LOG_MESSAGE = ex.Message,
            //            DATA1 = TO_NO,
            //            EDIT_EMP = this.LoginUser.EMP_NO,
            //            EDIT_TIME = DateTime.Now,
            //            DATA4 = "N"
            //        };
            //        _db.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
            //        asnmsg = "FinalASN:" + ex.Message;
            //    }
            //    #endregion

            //    #region Shipout GT
            //    try
            //    {
            //        string sql = string.Format(@"
            //                            SELECT *
            //                              FROM R_JUNIPER_TRUCKLOAD_TO
            //                             WHERE CLOSED = 1
            //                               AND ID='{0}'
            //                               AND NOT EXISTS (SELECT *
            //                                      FROM R_MES_LOG
            //                                     WHERE TO_NO = DATA1
            //                                       AND PROGRAM_NAME ='ShipoutGT'
            //                                       AND DATA4 = 'Y')"
            //           , ID);
            //        var res = SFCDB.ORM.Ado.SqlQuery<R_JUNIPER_TRUCKLOAD_TO>(sql)
            //                              .ToList()
            //                              .FirstOrDefault();
            //        if (res != null)
            //        {
            //            TO_NO = res.TO_NO;
            //            PLANT = res.PLANT;
            //            var msg = GT(TO_NO, PLANT, SFCDB);
            //            gtmsg = "SAP GT:" + msg;
            //            if (msg.StartsWith("OK"))
            //            {
            //                ShipoutGT_FLAG = true;
            //            }
            //        }
            //        else
            //        {
            //            ShipoutGT_FLAG = true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        R_MES_LOG log = new R_MES_LOG()
            //        {
            //            ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
            //            PROGRAM_NAME = "ShipoutGT",
            //            CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
            //            FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
            //            MAILFLAG = "N",
            //            LOG_MESSAGE = ex.Message,
            //            DATA1 = TO_NO,
            //            EDIT_EMP = this.LoginUser.EMP_NO,
            //            EDIT_TIME = DateTime.Now,
            //            DATA4 = "N"
            //        };
            //        SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
            //        gtmsg = "SAP GT:" + ex.Message;
            //    }
            //    #endregion

            //    UpdateToStatus(TO_NO, SFCDB);

            //    if (FinalASN_FLAG && ShipoutGT_FLAG)
            //    {
            //        StationReturn.Status = StationReturnStatusValue.Pass;
            //        StationReturn.Message = "OK," + gtmsg + "!" + asnmsg;
            //    }
            //    else if (!FinalASN_FLAG || !ShipoutGT_FLAG)
            //    {
            //        StationReturn.Status = StationReturnStatusValue.Fail;
            //        StationReturn.Message = gtmsg + "!</br>" + asnmsg;
            //    }
            //}
            //catch (Exception exception)
            //{
            //    SFCDB.RollbackTrain();
            //    throw exception;
            //}
            //finally
            //{
            //    this.DBPools["SFCDB"].Return(_db);
            //    this.DBPools["SFCDB"].Return(SFCDB);
            //}
        }

        public void ConfirmFinalASN(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            var _db = this.DBPools["SFCDB"].Borrow();
            var TO_NO = "";

            try
            {
                string ID = Data["ID"].ToString().Trim();
                string sql = string.Format(@"
                                        SELECT *
                                          FROM R_JUNIPER_TRUCKLOAD_TO
                                         WHERE CLOSED = 2
                                           AND ID='{0}'
                                           AND NOT EXISTS (SELECT *
                                                  FROM R_MES_LOG
                                                 WHERE TO_NO = DATA1
                                                   AND PROGRAM_NAME ='ShipoutFinalASN'
                                                   AND DATA4 = 'Y')"
                    , ID);
                var res = SFCDB.ORM.Ado.SqlQuery<R_JUNIPER_TRUCKLOAD_TO>(sql)
                                      .ToList()
                                      .FirstOrDefault();
                if (res == null)
                {
                    var n = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "R_JUNIPER_TRUCKLOAD_TO.ID=" + ID });
                    throw new Exception(n);
                }
                if (res.CLOSED != "2")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Please Refush This Page And Try It Agail!";
                }

                TO_NO = res.TO_NO;

                SFCDB.BeginTrain();

                FinalASN(TO_NO, SFCDB);

                UpdateToStatus(TO_NO, SFCDB);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "OK,Send FinalASN Success!";
                SFCDB.CommitTrain();
            }
            catch (Exception exception)
            {
                R_MES_LOG log = new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(_db.ORM, this.BU),
                    PROGRAM_NAME = "ShipoutFinalASN",
                    CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
                    FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
                    MAILFLAG = "N",
                    LOG_MESSAGE = exception.Message,
                    DATA1 = TO_NO,
                    EDIT_EMP = this.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now,
                    DATA4 = "N"
                };
                _db.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                SFCDB.RollbackTrain();
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(_db);
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void ConfirmGT(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            var to_no = "";
            var plant = "";
            try
            {
                string ID = Data["ID"].ToString().Trim();
                string sql = string.Format(@"
                                        SELECT *
                                          FROM R_JUNIPER_TRUCKLOAD_TO
                                         WHERE CLOSED = 2
                                           AND ID='{0}'
                                           AND NOT EXISTS (SELECT *
                                                  FROM R_MES_LOG
                                                 WHERE TO_NO = DATA1
                                                   AND PROGRAM_NAME ='ShipoutGT'
                                                   AND DATA4 = 'Y')"
                    , ID);
                var res = SFCDB.ORM.Ado.SqlQuery<R_JUNIPER_TRUCKLOAD_TO>(sql)
                                      .ToList()
                                      .FirstOrDefault();
                if (res == null)
                {
                    var n = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "R_JUNIPER_TRUCKLOAD_TO.ID=" + ID });
                    throw new Exception(n);
                }
                if (res.CLOSED != "2")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Please Refush This Page And Try It Agail!";
                }

                to_no = res.TO_NO;
                plant = res.PLANT;
                string msg = GT(to_no, plant, SFCDB);
                if (msg.StartsWith("OK"))
                {
                    UpdateToStatus(to_no, SFCDB);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = msg;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = msg;
                }
            }
            catch (Exception exception)
            {
                R_MES_LOG log = new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                    PROGRAM_NAME = "ShipoutGT",
                    CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
                    FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
                    MAILFLAG = "N",
                    LOG_MESSAGE = exception.Message,
                    DATA1 = to_no,
                    EDIT_EMP = this.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now,
                    DATA4 = "N"
                };
                SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetDataList(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var res = SFCDB.ORM.Ado.GetDataTable(@"
                                        SELECT A.*,
                                               DECODE(B.DATA4, null, 'N', B.DATA4) GtStatus,
                                               DECODE(C.DATA4, null, 'N', C.DATA4) ASNStatus
                                          FROM R_JUNIPER_TRUCKLOAD_TO A
                                          LEFT JOIN R_MES_LOG B
                                            ON A.TO_NO = B.DATA1
                                           AND B.DATA4='Y'
                                           AND B.PROGRAM_NAME = 'ShipoutGT'
                                          LEFT JOIN R_MES_LOG C
                                            ON A.TO_NO = C.DATA1
                                           AND C.DATA4='Y'
                                           AND C.PROGRAM_NAME = 'ShipoutFinalASN'
                                         WHERE A.CLOSED = 2
                                           AND NOT EXISTS (SELECT *
                                                  FROM R_MES_LOG
                                                 WHERE TO_NO = DATA1
                                                   AND PROGRAM_NAME = 'ShipoutConfirm'
                                                   AND DATA4 = 'Y')", new { });

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetDataDetail(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var TO_NO = Data["TO_NO"].ToString().Trim();
                var res = SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL, R_PRE_WO_HEAD, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((T, W, S, SP, P1, P2) =>
                     T.PACK_NO == P2.PACK_NO &&
                     W.WO == S.WORKORDERNO &&
                     S.ID == SP.SN_ID &&
                     SP.PACK_ID == P1.ID &&
                     P1.PARENT_PACK_ID == P2.ID)
                .Where((T, W, S, SP, P1, P2) => T.TO_NO == TO_NO)
                .GroupBy((T, W, S, SP, P1, P2) => new { T.TRAILER_NUM, W.GROUPID, T.PACK_NO, T.DELIVERYNUMBER })
                .Select((T, W, S, SP, P1, P2) => new { T.TRAILER_NUM, W.GROUPID, T.PACK_NO, QUANTITY = SqlSugar.SqlFunc.AggregateCount<string>(S.SN), T.DELIVERYNUMBER })
                .ToList();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        private void UpdateToStatus(string TO_NO, OleExec DB)
        {
            var ASN_Log = DB.ORM.Queryable<R_MES_LOG>().Where(t => t.PROGRAM_NAME == "ShipoutFinalASN" && t.DATA1 == TO_NO && t.DATA4 == "Y")
                .ToList();
            var Gt_Log = DB.ORM.Queryable<R_MES_LOG>().Where(t => t.PROGRAM_NAME == "ShipoutGT" && t.DATA1 == TO_NO && t.DATA4 == "Y")
                .ToList();
            if (ASN_Log.Count > 0 && Gt_Log.Count > 0)
            {
                DB.ORM.Updateable<R_JUNIPER_TRUCKLOAD_TO>()
                    .SetColumns(t => t.CLOSED == "3")
                    .Where(t => t.TO_NO == TO_NO)
                    .ExecuteCommand();
                R_MES_LOG log = new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(DB.ORM, this.BU),
                    PROGRAM_NAME = "ShipoutConfirm",
                    CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
                    FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
                    MAILFLAG = "N",
                    LOG_MESSAGE = "",
                    DATA1 = TO_NO,
                    EDIT_EMP = this.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now,
                    DATA4 = "Y"
                };
                DB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
            }
        }

        private void FinalASN(string TO_NO, OleExec SFCDB)
        {
            SendData.JuniperASNObj juniperASNObj = new SendData.JuniperASNObj(SFCDB.ORM);

            var pallets = SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL>()
                .Where(d => d.TO_NO == TO_NO)
                .Select(d => d.PACK_NO)
                .ToList();

            var wolist = SFCDB.ORM.Queryable<R_JUNIPER_MFPACKINGLIST>()
            .Where(d => d.INVOICENO == TO_NO)
            .Select(d => d.WORKORDERNO)
            .Distinct()
            .ToList();

            for (int i = 0; i < wolist.Count; i++)
            {
                var tpl = SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((SN, SP, P1, P2) => SN.ID == SP.SN_ID && SP.PACK_ID == P1.ID && P1.PARENT_PACK_ID == P2.ID)
                    .Where((SN, SP, P1, P2) => SN.WORKORDERNO == wolist[i] && !pallets.Contains(P2.PACK_NO) && SN.VALID_FLAG=="1")
                    .Select((SN, SP, P1, P2) => P2.PACK_NO)
                    .Distinct()
                    .ToList();
                if (tpl.Count > 0)
                {
                    var pl = "";
                    for (int n = 0; n < tpl.Count; n++)
                    {
                        pl += tpl[n] + ",";
                    }
                    throw new Exception("Please Check The Pallet " + pl + " Has Been Scan TRUNK-LOAD");
                }

                List<R_SN> sns = SFCDB.ORM.Queryable<R_SN>()
                     .Where(s => s.WORKORDERNO == wolist[i] && s.VALID_FLAG=="1")
                     .ToList();

                if (sns.FindAll(f => f.NEXT_STATION != "SHIPOUT").Count > 0)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180920162543", new string[] { wolist[i] });
                    throw new Exception(errMsg);
                }
                T_R_SN t = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
                t.LotsPassStation(sns, "Line1", "SHIPOUT", "SHIPOUT", this.BU, "Pass", this.LoginUser.EMP_NO, SFCDB);
                juniperASNObj.ShipOutChangePOStatusByWo(wolist[i], this.BU);
            }

            for (int i = 0; i < wolist.Count; i++)
            {
                juniperASNObj.ShipOutSendFinalAsnByWo(wolist[i], this.BU);
            }

            R_MES_LOG log = new R_MES_LOG()
            {
                ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                PROGRAM_NAME = "ShipoutFinalASN",
                CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
                FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
                MAILFLAG = "N",
                LOG_MESSAGE = "",
                DATA1 = TO_NO,
                EDIT_EMP = this.LoginUser.EMP_NO,
                EDIT_TIME = DateTime.Now,
                DATA4 = "Y"
            };
            SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
        }

        private string GT(string TO_NO, string PLANT, OleExec SFCDB)
        {
            var postdate = InterfacePublicValues.GetPostDate(SFCDB);
            var regex = "(\\d+)/(\\d+)/(\\d+)";
            var m = Regex.Match(postdate, regex);
            if (m.Success)
            {
                postdate = m.Groups[3].Value + "-" + m.Groups[1].Value + "-" + m.Groups[2];
            }
            #region Check Tunck Load Transfer
            var trucklog = SFCDB.ORM.Queryable<R_MES_LOG>().Where(t => t.PROGRAM_NAME == "TruckLoadMonthly" && t.DATA1 == TO_NO && t.DATA4 == "N").First();
            if (trucklog != null)
            {
                DataTable dd = new DataTable();
                string mms = SAPGT(this.BU, TO_NO, PLANT, trucklog.DATA2, trucklog.DATA3, postdate, SFCDB, out dd);
                if (mms.StartsWith("OK"))
                {
                    trucklog.DATA4 = "Y";
                    SFCDB.ORM.Updateable(trucklog).ExecuteCommand();
                    R_MES_LOG log = new R_MES_LOG()
                    {
                        ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                        PROGRAM_NAME = "TruckLoadMonthlyGT",
                        CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
                        FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
                        MAILFLAG = "N",
                        LOG_MESSAGE = mms,
                        DATA1 = TO_NO,
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_TIME = DateTime.Now,
                        DATA4 = "Y"
                    };
                    SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                }
                else
                {
                    R_MES_LOG log = new R_MES_LOG()
                    {
                        ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                        PROGRAM_NAME = "TruckLoadMonthlyGT",
                        CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
                        FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
                        MAILFLAG = "N",
                        LOG_MESSAGE = mms,
                        DATA1 = TO_NO,
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_TIME = DateTime.Now,
                        DATA4 = "N"
                    };
                    SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                    return mms;
                }

            }
            #endregion

            DataTable dt = new DataTable();
            string transFrom = ConfigurationManager.AppSettings[this.BU + "_SHIP311FROM"];
            string transTo = ConfigurationManager.AppSettings[this.BU + "_SHIP311TO"];
            string msg = SAPGT(this.BU, TO_NO, PLANT, transFrom, transTo, postdate, SFCDB, out dt);
            if (msg.StartsWith("OK"))
            {
                R_MES_LOG log = new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                    PROGRAM_NAME = "ShipoutGT",
                    CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
                    FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
                    MAILFLAG = "N",
                    LOG_MESSAGE = msg,
                    DATA1 = TO_NO,
                    EDIT_EMP = this.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now,
                    DATA4 = "Y"
                };



                SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                return msg;
            }
            else
            {
                R_MES_LOG log = new R_MES_LOG()
                {
                    ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                    PROGRAM_NAME = "ShipoutGT",
                    CLASS_NAME = "MESJuniper.Api.ShipoutConfirm",
                    FUNCTION_NAME = "MESJuniper.Api.ShipoutConfirm.Confirm",
                    MAILFLAG = "N",
                    LOG_MESSAGE = msg,
                    DATA1 = TO_NO,
                    EDIT_EMP = this.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now,
                    DATA4 = "N"
                };
                SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                return msg;
            }
        }

        private string SAPGT(string bu, string TO_NO, string Plant, string transFrom, string transTo, string postdate, OleExec SFCDB, out DataTable odt)
        {
            if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
            {
                odt = new DataTable();
                return "This time is monthly,can't BackFlush";
            }
            else
            {
                var dt = SFCDB.ORM.Queryable<R_JUNIPER_TRUCKLOAD_DETAIL, R_PRE_WO_HEAD, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((T, W, S, SP, P1, P2) =>
                     T.PACK_NO == P2.PACK_NO &&
                     W.WO == S.WORKORDERNO &&
                     S.ID == SP.SN_ID &&
                     SP.PACK_ID == P1.ID &&
                     P1.PARENT_PACK_ID == P2.ID)
                .Where((T, W, S, SP, P1, P2) => T.TO_NO == TO_NO)
                .GroupBy((T, W, S, SP, P1, P2) => new { T.TRAILER_NUM, W.GROUPID })
                .Select((T, W, S, SP, P1, P2) => new { T.TRAILER_NUM, W.GROUPID, QUANTITY = SqlSugar.SqlFunc.AggregateCount<string>(S.SN) })
                .ToDataTable();
                ZCMM_NSBG_0013 rfc = new ZCMM_NSBG_0013(bu);
                rfc.SetValue(TO_NO, transFrom, transTo, Plant, postdate, dt);
                rfc.CallRFC();
                var flag = rfc.GetValue("O_FLAG");
                odt = rfc.GetTableValue("OUT_TAB");
                if (flag == "0")
                {
                    var doc = rfc.GetValue("O_MBLNR");
                    odt = rfc.GetTableValue("OUT_TAB");
                    return "OK," + doc;
                }
                else
                {
                    var msg = rfc.GetValue("O_MESSAGE");
                    return msg;
                }
            }
        }

        public void GetPreAsnSoPoList(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                var po = Data["PO"].ToString();
                var poline = Data["ITEM"].ToString();
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var mainhead = SFCDB.ORM.Queryable<O_ORDER_MAIN, I137_H, I137_I>((m, h, i) => m.ITEMID == i.ID && i.TRANID == h.TRANID).Where((m, h, i) => m.PONO == po && m.POLINE == poline).Select((m, h, i) => h).ToList().FirstOrDefault();
                if (mainhead == null)
                    throw new Exception("order is not exists!");

                var type = mainhead.COMPLETEDELIVERY;
                var res = SFCDB.ORM.Queryable<O_ORDER_MAIN, I137_H, I137_I, O_PO_STATUS, R_PRE_WO_HEAD>((m, h, i, s, w) => m.ITEMID == i.ID && i.TRANID == h.TRANID && m.ID == s.POID && m.PREWO == w.WO)
                    .Where((m, h, i, s, w) => h.SALESORDERNUMBER == mainhead.SALESORDERNUMBER && s.VALIDFLAG == MesBool.Yes.ExtValue()
                    && (s.STATUSID == ENUM_O_PO_STATUS.PreAsn.ExtValue() || s.STATUSID == ENUM_O_PO_STATUS.CancelPreAsn.ExtValue()) && h.COMPLETEDELIVERY == type
                    && m.ORDERTYPE != ENUM_I137_PoDocType.IDOA.ExtValue())//排除DOA和正常PO相同SO的情况
                    .OrderBy((m, h, i, s, w) => i.PODELIVERYDATE)
                    .OrderBy((m, h, i, s, w) => h.SALESORDERNUMBER)
                    .OrderBy((m, h, i, s, w) => i.SALESORDERLINEITEM)
                    .Select((m, h, i, s, w) =>
                    new GetPreAsnSoPoListRes()
                    {
                        UPOID = m.UPOID,
                        PONO = m.PONO,
                        POLINE = m.POLINE,
                        QTY = m.QTY,
                        PID = m.PID,
                        PREWO = m.PREWO,
                        GROUPID = w.GROUPID,
                        SO = h.SALESORDERNUMBER,
                        SOLINE = i.SALESORDERLINEITEM,
                        PDDDATA = i.PODELIVERYDATE,
                        CRSDDATA = i.CUSTREQSHIPDATE,
                        COMPLETEDELIVERY = h.COMPLETEDELIVERY,
                        EARLYSHIPDATEDATA = SqlFunc.IF(i.PODELIVERYDATE == i.CUSTREQSHIPDATE).Return(i.CUSTREQSHIPDATE)
                        .ElseIF(i.PODELIVERYDATE < i.CUSTREQSHIPDATE).Return(i.PODELIVERYDATE)
                        .ElseIF(i.PODELIVERYDATE <= ((DateTime)i.CUSTREQSHIPDATE).AddDays(4)).Return(i.CUSTREQSHIPDATE).End(((DateTime)i.PODELIVERYDATE).AddDays(-4))
                    })
                    .Mapper(t =>
                    {
                        t.PDD = Convert.ToDateTime(t.PDDDATA).ToString("yyyy-MM-dd");
                        t.CRSD = Convert.ToDateTime(t.CRSDDATA).ToString("yyyy-MM-dd");
                        if (Convert.ToDateTime(t.EARLYSHIPDATEDATA).DayOfWeek == DayOfWeek.Saturday)
                        {
                            t.EARLYSHIPDATE = Convert.ToDateTime(t.EARLYSHIPDATEDATA).AddDays(-1).ToString("yyyy-MM-dd");
                            t.EARLYSHIPSTATUS = ((DateTime)t.EARLYSHIPDATEDATA).AddDays(-1).Date > DateTime.Now.Date ? "N" : "Y";
                        }
                        else if (Convert.ToDateTime(t.EARLYSHIPDATEDATA).DayOfWeek == DayOfWeek.Sunday)
                        {
                            t.EARLYSHIPDATE = Convert.ToDateTime(t.EARLYSHIPDATEDATA).AddDays(-2).ToString("yyyy-MM-dd");
                            t.EARLYSHIPSTATUS = ((DateTime)t.EARLYSHIPDATEDATA).AddDays(-2).Date > DateTime.Now.Date ? "N" : "Y";
                        }
                        else
                        {
                            t.EARLYSHIPDATE = Convert.ToDateTime(t.EARLYSHIPDATEDATA).ToString("yyyy-MM-dd");
                            t.EARLYSHIPSTATUS = ((DateTime)t.EARLYSHIPDATEDATA).Date > DateTime.Now.Date ? "N" : "Y";
                        }
                    }).ToList();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = res;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        
        class GetPreAsnSoPoListRes
        {
            public string UPOID { get; set; }
            public string PONO { get; set; }
            public string POLINE { get; set; }
            public string QTY { get; set; }
            public string PID { get; set; }
            public string PREWO { get; set; }
            public string GROUPID { get; set; }
            public string SO { get; set; }
            public string SOLINE { get; set; }
            public string PDD { get; set; }
            public string CRSD { get; set; }
            public DateTime? PDDDATA { get; set; }
            public DateTime? CRSDDATA { get; set; }
            public string COMPLETEDELIVERY { get; set; }
            public string EARLYSHIPDATE { get; set; }
            public string EARLYSHIPSTATUS { get; set; }
            public DateTime? EARLYSHIPDATEDATA { get; set; }
        }

    }
}
