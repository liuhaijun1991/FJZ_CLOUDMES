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

namespace MESJuniper.Api
{
    public class ShipoutPGI : MesAPIBase
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
        protected APIInfo FDoPGI = new APIInfo
        {
            FunctionName = "DoPGI",
            Description = "Do PIG",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName="ID", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        public ShipoutPGI()
        {
            this.Apis.Add(FGetDataList.FunctionName, FGetDataList);
            this.Apis.Add(FGetDataDetail.FunctionName, FGetDataDetail);
            this.Apis.Add(FDoPGI.FunctionName, FDoPGI);
        }

        public void DoPGI(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string ID = Data["ID"].ToString().Trim();
                string sql = string.Format(@"
                                        SELECT *
                                          FROM R_JUNIPER_TRUCKLOAD_TO
                                         WHERE CLOSED = 3
                                           AND ID='{0}'
                                           AND NOT EXISTS (SELECT *
                                                  FROM R_MES_LOG
                                                 WHERE TO_NO = DATA1
                                                   AND PROGRAM_NAME ='ShipoutPGI'
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
                if (res.CLOSED != "3")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "Please Refush This Page And Try It Agail!";
                }
                SFCDB.BeginTrain();
                DataTable dt = new DataTable();
                string msg = PGI(this.BU, res.TO_NO, res.PLANT, SFCDB);
                if (msg.StartsWith("OK"))
                {
                    res.CLOSED = "4";
                    SFCDB.ORM.Updateable<R_JUNIPER_TRUCKLOAD_TO>(res).ExecuteCommand();
                    R_MES_LOG log = new R_MES_LOG()
                    {
                        ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                        PROGRAM_NAME = "ShipoutPGI",
                        CLASS_NAME = "MESJuniper.Api.ShipoutPGI",
                        FUNCTION_NAME = "MESJuniper.Api.ShipoutPGI.DoPGI",
                        MAILFLAG = "N",
                        LOG_MESSAGE = msg,
                        DATA1 = res.TO_NO,
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_TIME = DateTime.Now,
                        DATA4 = "Y"
                    };
                    SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();

                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = msg;
                    StationReturn.Data = dt;
                }
                else
                {
                    R_MES_LOG log = new R_MES_LOG()
                    {
                        ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                        PROGRAM_NAME = "ShipoutPGI",
                        CLASS_NAME = "MESJuniper.Api.ShipoutPGI",
                        FUNCTION_NAME = "MESJuniper.Api.ShipoutPGI.DoPGI",
                        MAILFLAG = "N",
                        LOG_MESSAGE = msg,
                        DATA1 = res.TO_NO,
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_TIME = DateTime.Now,
                        DATA4 = "N"
                    };
                    SFCDB.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();

                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = msg;
                    StationReturn.Data = dt;
                }
                SFCDB.CommitTrain();
            }
            catch (Exception exception)
            {
                SFCDB.RollbackTrain();
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
                var res = SFCDB.ORM.Ado.SqlQuery<R_JUNIPER_TRUCKLOAD_TO>(@"
                                        SELECT *
                                          from R_JUNIPER_TRUCKLOAD_TO
                                         WHERE CLOSED = 3
                                           AND NOT EXISTS (SELECT *
                                                  FROM R_MES_LOG
                                                 WHERE TO_NO = DATA1
                                                   AND PROGRAM_NAME ='ShipoutPGI'
                                                   AND DATA4 = 'Y')")
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

        public void GetDataDetail(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var TO_NO = Data["TO_NO"].ToString().Trim();
                var res = SFCDB.ORM.Ado.SqlQuery<DataDetail>(@"
                            SELECT GROUPID, PO_NUMBER PONO, LINEID POLINE, SUM(QUANTITY) QUANTITY, SALESORDER DELIVERYNUMBER, B.PREASN ASNNUMBER,B.PREWO WO
                              FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN B
                             WHERE A.WORKORDERNO = B.PREWO
                               AND INVOICENO = @TO_NO
                               AND NOT EXISTS (SELECT *
                                      FROM R_MES_LOG
                                     WHERE B.PREASN = DATA1
                                       AND DATA2 = B.PREWO
                                       AND PROGRAM_NAME = 'ShipoutPGI'
                                       AND DATA4 = 'Y')
                               AND EXISTS (SELECT * FROM R_I282 WHERE ASNNUMBER = B.PREASN)
                              GROUP BY GROUPID, PO_NUMBER, LINEID, SALESORDER, B.PREASN,B.PREWO
                             ORDER BY PO_NUMBER, LINEID"
                            , new { TO_NO })
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

        private string PGI(string bu, string TO_NO, string Plant, OleExec SFCDB)
        {
            if (InterfacePublicValues.IsMonthly(SFCDB, DB_TYPE_ENUM.Oracle))
            {
                return "This time is monthly,can't BackFlush";
            }
            else
            {
                var dt = SFCDB.ORM.Ado.SqlQuery<DataDetail>(@"
                                            SELECT GROUPID, PO_NUMBER PONO, LINEID POLINE, SUM(QUANTITY) QUANTITY, SALESORDER DELIVERYNUMBER, B.PREASN ASNNUMBER,B.PREWO WO
                                              FROM R_JUNIPER_MFPACKINGLIST A, O_ORDER_MAIN B
                                             WHERE A.WORKORDERNO = B.PREWO
                                               AND INVOICENO = @TO_NO
                                               AND NOT EXISTS (SELECT *
                                                      FROM R_MES_LOG
                                                     WHERE B.PREASN = DATA1
                                                       AND DATA2 = B.PREWO
                                                       AND PROGRAM_NAME = 'ShipoutPGI'
                                                       AND DATA4 = 'Y')
                                               AND EXISTS (SELECT * FROM R_I282 WHERE ASNNUMBER = B.PREASN)
                                              GROUP BY GROUPID, PO_NUMBER, LINEID, SALESORDER, B.PREASN,B.PREWO
                                             ORDER BY PO_NUMBER, LINEID"
                                            , new { TO_NO })
                                         .ToList();
                ZCPP_NSBG_0005 rfc = new ZCPP_NSBG_0005(bu);
                var msg = "";
                var finish = true;
                var db = this.DBPools["SFCDB"].Borrow();
                try
                {
                    var postdate = InterfacePublicValues.GetPostDate(SFCDB);
                    var regex = "(\\d+)/(\\d+)/(\\d+)";
                    var regs = Regex.Match(postdate, regex);
                    if (regs.Success)
                    {
                        postdate = regs.Groups[3].Value + "-" + regs.Groups[1].Value + "-" + regs.Groups[2];
                    }
                    for (int i = 0; i < dt.Count; i++)
                    {
                        rfc.SetValue(dt[i].ASNNUMBER, ConfigurationManager.AppSettings[this.BU + "_SHIP311TO"], Plant, postdate, dt[i].GROUPID, dt[i].QUANTITY);
                        rfc.CallRFC();
                        var flag = rfc.GetValue("O_FLAG");
                        if (flag == "0")
                        {
                            var doc = rfc.GetValue("O_MBLNR");
                            R_MES_LOG log = new R_MES_LOG()
                            {
                                ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                                PROGRAM_NAME = "ShipoutPGI",
                                CLASS_NAME = "MESJuniper.Api.ShipoutPGI",
                                FUNCTION_NAME = "MESJuniper.Api.ShipoutPGI.PGI",
                                MAILFLAG = "N",
                                LOG_MESSAGE = doc,
                                DATA1 = dt[i].ASNNUMBER,
                                DATA2 = dt[i].WO,
                                DATA3 = dt[i].QUANTITY,
                                EDIT_EMP = this.LoginUser.EMP_NO,
                                EDIT_TIME = DateTime.Now,
                                DATA4 = "Y"
                            };
                            db.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                        }
                        else
                        {
                            finish = false;
                            var m = rfc.GetValue("O_MESSAGE");
                            msg += m;
                            R_MES_LOG log = new R_MES_LOG()
                            {
                                ID = MesDbBase.GetNewID<R_MES_LOG>(SFCDB.ORM, this.BU),
                                PROGRAM_NAME = "ShipoutPGI",
                                CLASS_NAME = "MESJuniper.Api.ShipoutPGI",
                                FUNCTION_NAME = "MESJuniper.Api.ShipoutPGI.PGI",
                                MAILFLAG = "N",
                                LOG_MESSAGE = m,
                                DATA1 = dt[i].ASNNUMBER,
                                DATA2 = dt[i].GROUPID,
                                DATA3 = dt[i].QUANTITY,
                                EDIT_EMP = this.LoginUser.EMP_NO,
                                EDIT_TIME = DateTime.Now,
                                DATA4 = "N"
                            };
                            db.ORM.Insertable<R_MES_LOG>(log).ExecuteCommand();
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg += ex.Message;
                }
                finally
                {
                    this.DBPools["SFCDB"].Return(db);
                }
                if (!finish)
                {
                    msg = "Warning," + msg;
                }
                else {
                    msg = "OK," + msg;
                }
                return msg;
            }
        }
        class DataDetail
        {
            public string GROUPID { get; set; }
            public string PONO { get; set; }
            public string POLINE { get; set; }
            public string QUANTITY { get; set; }
            public string DELIVERYNUMBER { get; set; }
            public string ASNNUMBER { get; set; }
            public string WO { get; set; }
        }
    }
}
