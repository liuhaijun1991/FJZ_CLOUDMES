using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using static MESDataObject.Constants.PublicConstants;

namespace MESJuniper.Api
{
    public class DOAShipmentAPI : MesAPIBase
    {
        protected APIInfo FGetDOAShipmentSoPoList = new APIInfo()
        {
            FunctionName = "FGetDOAShipmentSoPoList",
            Description = "Get Juniper Pending DOA Shipment So&PO List",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetJuniperDOAShipmentDetail = new APIInfo()
        {
            FunctionName = "GetJuniperDOAShipmentDetail",
            Description = "Get Juniper DOA Shipment Detail",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FGetJuniperDOAShipmentDN = new APIInfo()
        {
            FunctionName = "GetJuniperDOAShipmentDN",
            Description = "Get Juniper DOA Shipment Detail",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FJuniperSendShipmentCombination = new APIInfo()
        {
            FunctionName = "JuniperSendShipmentCombination",
            Description = "Create Shipment Data&File&auto send email",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FUploadDOAReReceiveDN = new APIInfo()
        {
            FunctionName = "UploadDOAReReceiveDN",
            Description = "Upload Shipment File Reply From Custmer",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        public DOAShipmentAPI()
        {
            this.Apis.Add(FGetJuniperDOAShipmentDetail.FunctionName, FGetJuniperDOAShipmentDetail);
            this.Apis.Add(FGetJuniperDOAShipmentDN.FunctionName, FGetJuniperDOAShipmentDN);
            this.Apis.Add(FJuniperSendShipmentCombination.FunctionName, FJuniperSendShipmentCombination);
            this.Apis.Add(FUploadDOAReReceiveDN.FunctionName, FUploadDOAReReceiveDN);
        }
        public void GetDOAShipmentSoPoList(JObject requestValue, JObject Data, MESStationReturn StationReturn)
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
                    && m.ORDERTYPE == ENUM_I137_PoDocType.IDOA.ExtValue())
                    .OrderBy((m, h, i, s, w) => i.PODELIVERYDATE)
                    .OrderBy((m, h, i, s, w) => h.SALESORDERNUMBER)
                    .OrderBy((m, h, i, s, w) => i.SALESORDERLINEITEM)
                    .Select((m, h, i, s, w) =>
                    new GetShipmentSoPoListRes()
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
        public void GetJuniperDOAShipmentDetail(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string ASNNUMBER = Data["ASNNUMBER"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_JNP_DOA_SHIPMENTS>().Where(t => t.ASNNUMBER == ASNNUMBER)
                    .Select(t => new
                    {
                        t.FILE_NAME,
                        t.PART_NUMBER,
                        t.SERIAL_NUMBER,
                        t.CARTON_ID,
                        t.MIST_CLAIM_CODE,
                        t.ETH_MAC,
                        t.MIST_PALLET_ID,
                        t.INVOICE_NO,
                        t.MFG_DATE,
                        t.HW_REVISION,
                        t.PO_NUMBER,
                        t.PO_LINE_NO,
                        t.SHIPPED_QTY,
                        t.COO,
                        t.BUILD_SITE,
                        t.STATUS,
                        t.ROHS2,
                        t.MEANS_OF_TRANSPORT
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
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void GetJuniperDOAShipmentDN(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                string ASNNUMBER = Data["ASNNUMBER"].ToString().Trim();
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<R_JNP_DOA_SHIPMENTS_ACK>().Where(t => t.ASNNUMBER == ASNNUMBER).ToList();
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
                this.DBPools["SFCDB"].Return(oleDB);
            }
        }
        public void JuniperBuildShipmentData(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.PoolItem.BorrowTimeOut = 3600;
                string[] UpoidList = (string[])JsonConvert.DeserializeObject(Data["UPOIDLIST"].ToString(), typeof(string[]));
                string transport = Data["TRANSPORT"] == null ? "" : Data["TRANSPORT"].ToString().Trim();
                SendData.JuniperDOAShipment shipment = new SendData.JuniperDOAShipment(SFCDB.ORM, this.BU, LoginUser.EMP_NO);
                shipment.SendShipment(UpoidList, transport, true);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = "";
            }
            catch (Exception exception)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
            }
            finally
            {
                if (SFCDB != null)
                    this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void JuniperSendShipmentFile(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.PoolItem.BorrowTimeOut = 3600;
                string ASNNUMBER = Data["ASNNUMBER"].ToString().Trim();
                var UpoidList = SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREASN == ASNNUMBER).Select(t => t.UPOID).ToList().ToArray();
                SendData.JuniperDOAShipment shipment = new SendData.JuniperDOAShipment(SFCDB.ORM, this.BU, LoginUser.EMP_NO);
                shipment.SendShipment(UpoidList);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = "";
            }
            catch (Exception exception)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
            }
            finally
            {
                if (SFCDB != null)
                    this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void JuniperSendShipmentCombination(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = this.DBPools["SFCDB"].Borrow();
                SFCDB.PoolItem.BorrowTimeOut = 3600;
                string[] UpoidList = (string[])JsonConvert.DeserializeObject(Data["UPOIDLIST"].ToString(), typeof(string[]));
                string transport = Data["TRANSPORT"] == null ? "" : Data["TRANSPORT"].ToString().Trim();
                SendData.JuniperDOAShipment shipment = new SendData.JuniperDOAShipment(SFCDB.ORM,this.BU, LoginUser.EMP_NO);
                shipment.SendShipment(UpoidList, transport);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000026";
                StationReturn.Data = "";
            }
            catch (Exception exception)
            {
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
            }
            finally
            {
                if (SFCDB != null)
                    this.DBPools["SFCDB"].Return(SFCDB);
            }
        }


        public void UploadDOAReReceiveDN(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<string> inputTitle = new List<string> { "File Row", "File PO", "File PO Line", "Model", "Serial", "Delivery Number", "DN Line", "Equipment", "User Status", "IB Delivery", "Message Code", "Message Text" };
                string errTitle = "";
                string data = Data["ExcelData"].ToString();
                string pre = string.Empty;

                JArray array = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                if (array.Count == 0)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162038"));
                }
                JObject firstData = JObject.Parse(array[0].ToString());
                bool hasErr = CheckExcelFormat(firstData, inputTitle, out errTitle);
                if (!hasErr)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814162102", new string[] { errTitle }));
                }

                SendData.JuniperDOAShipment shipment = new SendData.JuniperDOAShipment(SFCDB.ORM,this.BU,LoginUser.EMP_NO);
                shipment.UploadReply(array);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "upload successful !";
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                StationReturn.Data = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        private bool CheckExcelFormat(JObject inputExcelColumn, List<string> listTitle, out string title)
        {
            bool bColumnExists = true;
            string out_title = "";
            foreach (string t in listTitle)
            {
                bColumnExists = inputExcelColumn.Properties().Any(p => p.Name == t);
                if (!bColumnExists)
                {
                    out_title = t;
                    break;
                }
            }
            title = out_title;
            return bColumnExists;
        }

        private class GetShipmentSoPoListRes
        {
            public GetShipmentSoPoListRes()
            {
            }

            public string UPOID { get; set; }
            public string PONO { get; set; }
            public string POLINE { get; set; }
            public string QTY { get; set; }
            public string PID { get; set; }
            public string PREWO { get; set; }
            public string GROUPID { get; set; }
            public string SO { get; set; }
            public string SOLINE { get; set; }
            public DateTime? PDDDATA { get; set; }
            public DateTime? CRSDDATA { get; set; }
            public string COMPLETEDELIVERY { get; set; }
            public DateTime? EARLYSHIPDATEDATA { get; set; }
            public string PDD { get; set; }
            public string CRSD { get; set; }
            public string EARLYSHIPDATE { get; set; }
            public string EARLYSHIPSTATUS { get; set; }
        }
    }
}
