using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MESPubLab;
using MESDataObject.Module.Juniper;
using MESPubLab.MESStation.MESReturnView.Station;
using MESDataObject;

namespace MESJuniper.Api
{
    public class I054Api : MesAPIBase
    {
        protected APIInfo FProcessI054Error = new APIInfo
        {
            FunctionName = "ProcessI054Error",
            Description = "ProcessI054Error",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName="ID", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FProcessI054AckError = new APIInfo
        {
            FunctionName = "ProcessI054AckError",
            Description = "ProcessI054AckError",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo(){ InputName="ID", InputType="string" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetI054Error = new APIInfo
        {
            FunctionName = "GetI054Error",
            Description = "GetI054Error",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetR_I054AckError = new APIInfo
        {
            FunctionName = "GetR_I054AckError",
            Description = "GetR_I054AckError",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        public I054Api() {
            this.Apis.Add(FProcessI054Error.FunctionName, FProcessI054Error);
            this.Apis.Add(FProcessI054AckError.FunctionName, FProcessI054AckError);
            this.Apis.Add(FGetI054Error.FunctionName, FGetI054Error);
            this.Apis.Add(FGetR_I054AckError.FunctionName, FGetR_I054AckError);
        }

        public void ProcessI054Error(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string ID = Data["ID"].ToString().Trim();

                SFCDB = this.DBPools["SFCDB"].Borrow();
                var res = SFCDB.ORM.Queryable<R_MES_LOG>()
                    .Where(t => t.ID == ID && t.DATA4 == null)
                    .Select(t => t.ID)
                    .ToList();
                if (res.Count == 0)
                {
                    var n = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "R_MES_LOG.ID=" + ID });
                    throw new Exception(n);
                }
                var aaa = SFCDB.ORM.Updateable<R_MES_LOG>()
                    .SetColumns(t => new R_MES_LOG
                    {
                        DATA4 = "N",
                        EDIT_EMP = this.LoginUser.EMP_NO,
                        EDIT_TIME = DateTime.Now
                    })
                    .Where(t => res.Contains(t.ID))
                    .ExecuteCommand();


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

        public void ProcessI054AckError(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                string ID = Data["ID"].ToString().Trim();

                SFCDB = this.DBPools["SFCDB"].Borrow();
                var res = SFCDB.ORM.Queryable<R_I054_ACK>()
                    .Where(t => t.ID == ID)
                    .First();
                if (res == null)
                {
                    var n = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "R_I054_ACK.ID=" + ID });
                    throw new Exception(n);
                }
                WriteLog.WriteIntoMESLog(SFCDB, BU, "MESInterface", "MESInterface.JUNIPER.I054CaptrueProcess", "ReGenerateAS_Build_Data", "Pending Re Generate", "", res.TRANID, "", "", "N", "interface", "N");

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

        public void GetI054Error(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var res = SFCDB.ORM.Queryable<R_MES_LOG>()
                    .Where(t => t.PROGRAM_NAME == "MESInterface" && t.FUNCTION_NAME == "GenerateAS_Build_Data" && t.DATA4 == null)
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

        public void GetR_I054AckError(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                var res = SFCDB.ORM.Ado.SqlQuery<AckDate>(@"                              
                                SELECT T2.ID,
                                       T5.PONO,
                                       T5.POLINE,
                                       T5.PREWO,
                                       T2.SERIALNUMBER,
                                       T2.RESPONSECODE,
                                       T2.RESPONSEMESSAGE,
                                       T2.CREATETIME,
                                       T3.EDIT_TIME,
                                       T2.TRANID,
                                       T3.DATA4 IsReBuild
                                  FROM (SELECT *
                                          FROM (SELECT A.*,
                                                       ROW_NUMBER() OVER(PARTITION BY PARENTSN ORDER BY CREATETIME DESC) NUMBS
                                                  FROM R_I054 A
                                                 WHERE A.PNTYPE = 'Parent')
                                         WHERE NUMBS = 1) T1
                                  LEFT JOIN R_I054_ACK T2
                                    ON T1.TRANID = T2.TRANID
                                   AND (T1.PARENTSN = T2.SERIALNUMBER OR T2.SERIALNUMBER = 'N/A')
                                  LEFT JOIN R_MES_LOG T3
                                    ON T2.SERIALNUMBER = T3.DATA1
                                   AND T2.TRANID = T3.DATA3
                                   AND T3.PROGRAM_NAME = 'MESInterface'
                                   AND T3.FUNCTION_NAME = 'ReGenerateAS_Build_Data'
                                  LEFT JOIN R_SN T4
                                    ON T1.PARENTSN=T4.SN
                                    AND T4.VALID_FLAG='1'
                                  LEFT JOIN O_ORDER_MAIN T5
                                    ON T4.WORKORDERNO=T5.PREWO
                                 WHERE T2.RESPONSEMESSAGE <> 'Success'
                                 AND T4.SN IS NOT NULL
                                 ORDER BY T3.EDIT_TIME,
                                          T2.CREATETIME DESC")
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

        class AckDate
        {
            public string ID { get; set; }
            public string SALESORDERNUMBER { get; set; }
            public string SALESORDERLINENUMBER { get; set; }
            public string SERIALNUMBER { get; set; }
            public string MODELNUMBER { get; set; }
            public string RESPONSECODE { get; set; }
            public string RESPONSEMESSAGE { get; set; }
            public string CREATETIME { get; set; }
            public string IsReBuild { get; set; }
        }

    }
}
