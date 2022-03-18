using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.ARUBA;
using MESDataObject.Module.DCN;
using MESDataObject.Module.DCN.ARUBA;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation;
using MESPubLab.MESStation.SNMaker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using static MESDataObject.Constants.PublicConstants;

namespace MESStation.Config.DCN
{
    public class ArubaOrderManager : MesAPIBase
    {
        private APIInfo FGetOrderMainInfo = new APIInfo()
        {
            FunctionName = "GetOrderMainInfo",
            Description = "GetOrderMainInfo",
            Parameters = new List<APIInputInfo>()
                { },
            Permissions = new List<MESPermission>()
                { }
        };
        private APIInfo FGetPoList = new APIInfo()
        {
            FunctionName = "GetPoList",
            Description = "獲取所有GetPoList",
            Parameters = new List<APIInputInfo>()
                { },
            Permissions = new List<MESPermission>()
                { }
        };
        private APIInfo FGetSingleOrderInfo = new APIInfo()
        {
            FunctionName = "GetSingleOrderInfo",
            Description = "獲取所有GetPoList",
            Parameters = new List<APIInputInfo>()
                { },
            Permissions = new List<MESPermission>()
                { }
        };
        private APIInfo FGetReasonCodes = new APIInfo()
        {
            FunctionName = "GetReasonCodes",
            Description = "獲取Aruba所有ReasonCodes",
            Parameters = new List<APIInputInfo>()
                { },
            Permissions = new List<MESPermission>()
                { }
        };
        private APIInfo FGet855ByPoId = new APIInfo()
        {
            FunctionName = "Get855ByPoId",
            Description = "獲取Aruba Get855ByPoId",
            Parameters = new List<APIInputInfo>()
                { },
            Permissions = new List<MESPermission>()
                { }
        };
        private APIInfo FAdd855Info = new APIInfo()
        {
            FunctionName = "Add855Info",
            Description = "Add855Info",
            Parameters = new List<APIInputInfo>()
                { },
            Permissions = new List<MESPermission>()
                { }
        };
        private APIInfo FGetOrderHisList = new APIInfo()
        {
            FunctionName = "GetOrderHisList",
            Description = "GetOrderHisList",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo FGetSingleOrderInfoByID = new APIInfo()
        {
            FunctionName = "GetSingleOrderInfoByID",
            Description = "GetSingleOrderInfoByID",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo FGet856 = new APIInfo()
        {
            FunctionName = "Get856",
            Description = "Get856",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };
        public ArubaOrderManager()
        {
            this.Apis.Add(FGetPoList.FunctionName, FGetPoList);
            this.Apis.Add(FGetSingleOrderInfo.FunctionName, FGetSingleOrderInfo);
            this.Apis.Add(FGetReasonCodes.FunctionName, FGetReasonCodes);
            this.Apis.Add(FGet855ByPoId.FunctionName, FGet855ByPoId);
            this.Apis.Add(FAdd855Info.FunctionName, FAdd855Info); 
            this.Apis.Add(FGetOrderMainInfo.FunctionName, FGetOrderMainInfo);
            this.Apis.Add(FGetOrderHisList.FunctionName, FGetOrderHisList);
            this.Apis.Add(FGetSingleOrderInfoByID.FunctionName, FGetSingleOrderInfoByID);
            this.Apis.Add(FGet856.FunctionName, FGet856);
        }

        public void GetOrderMainInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var poid = Data["POID"].ToString().Trim();

                var res = sfcdb.ORM.Queryable<O_ORDER_MAIN, O_PO_STATUS, O_PO_STATUS_MAP_J>((m, s, p) => m.ID == s.POID && s.STATUSID == p.NAME)
                    .Where((m, s, p) => m.ID == poid && s.VALIDFLAG == MesBool.Yes.ExtValue() && p.CUST == Customer.ARUBA.ExtValue())
                    .Select((m, s, p) => new { ORDERDATA =m, ORDERSTATUS=p.NAME,ORDERSTATUSNAME = p.DESCRIPTION }).ToList().FirstOrDefault();
                if (res == null)
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102021"));

                var CommitQty = "0";
                if (res.ORDERDATA.VERSION =="0")
                    CommitQty = sfcdb.ORM.Queryable<HPE_EDI_855,HPE_EDI_850>((e55,e50)=>e55.F_850_ID==e50.F_ID).Where((e55, e50) => e50.ID == res.ORDERDATA.ITEMID)
                        .Select((e55, e50) => SqlFunc.AggregateSum(e55.F_ACK_QTY)).ToList().FirstOrDefault();
                else
                    CommitQty = sfcdb.ORM.Queryable<HPE_EDI_855, HPE_EDI_860>((e55, e60) => e55.F_850_ID == e60.F_ID).Where((e55, e60) => e60.ID == res.ORDERDATA.ITEMID)
                           .Select((e55, e60) => SqlFunc.AggregateSum(e55.F_ACK_QTY)).ToList().FirstOrDefault();

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = new { res, CommitQty };
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void Get855ByPoId(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var poid = Data["POID"].ToString().Trim();
                //var res = sfcdb.ORM.Queryable<HPE_EDI_855>().Where(t => t.F_850_ID == poid).OrderBy(t=>t.EDIT_TIME,OrderByType.Asc).ToList();
                var main = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t=>t.ID == poid).ToList().FirstOrDefault();
                if (main == null)
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102021"));
                var res = new object();
                if (main.VERSION == "0")
                    res = sfcdb.ORM.Queryable<HPE_EDI_855, HPE_EDI_850>((e55, e50) => e55.F_850_ID == e50.F_ID).Where((e55, e50) => e50.ID == main.ITEMID)
                        .Select((e55, e50) => e55).ToList();
                else
                    res = sfcdb.ORM.Queryable<HPE_EDI_855, HPE_EDI_860>((e55, e60) => e55.F_850_ID == e60.F_ID).Where((e55, e60) => e60.ID == main.ITEMID)
                           .Select((e55, e60) =>e55).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void Get855(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = sfcdb.ORM.Queryable<HPE_EDI_855,HPE_EDI_824>((e5,e2)=>new object[] { JoinType.Left,e5.ID == e2.F_EDI_FILENAME && e2.F_OTI_TRAN_TYPE == "855" })
                    .OrderBy((e5, e2) => e2.F_EDI_FILENAME, OrderByType.Desc)
                    .OrderBy((e5, e2) => e5.F_PO, OrderByType.Desc)
                    .OrderBy((e5, e2) => e5.F_LINE, OrderByType.Desc).OrderBy((e5, e2) => e5.EDIT_TIME, OrderByType.Desc)
                    .Select((e5, e2) => new {
                        e5.F_PO,
                        e5.F_DATE,
                        e5.F_LINE,
                        e5.F_MPN,
                        e5.F_CPN,
                        e5.F_MPN_DESC,
                        e5.F_LINE_PRICE,
                        e5.F_LINE_QTY,
                        e5.F_REASON_CODE,
                        e5.F_ACK_ESD,
                        e5.F_ACK_EDD,
                        e5.F_ACK_QTY,
                        e5.EDIT_TIME,
                        e5.EDIT_EMP,
                        e2.F_OTI_CODE, e2.F_OTI_TED_TEXT, e2.F_OTI_NTE_MSG, e2.F_FILENAME })
                    .ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void UploadArubaList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                var predata = Data["predata"].ToList();
                var uploadtable = new DataTable();
                var blockdates = new List<DateTime>();

                foreach (var item in predata)
                {
                    foreach (var jToken in item.Children())
                    {
                        var jProperty = jToken as JProperty;
                        uploadtable.Columns.Add(jProperty.Name.ToString());
                    }
                    break;
                }
                uploadtable.Columns.Add("STATUS");
                uploadtable.Columns.Add("MSG");
                #region valid data
                foreach (var item in predata)
                {
                    var itemrow = uploadtable.NewRow();
                    try
                    {
                        foreach (var jToken in item.Children())
                        {
                            var jProperty = jToken as JProperty;
                            itemrow[jProperty.Name.ToString()] = item[jProperty.Name.ToString()].ToString().Trim();
                            if (itemrow[jProperty.Name.ToString()].ToString().Trim().Equals(""))
                                throw new Exception($@"{jProperty.Name.ToString()} value is empty,pls check!");
                        }
                    }
                    catch (Exception e)
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = e.ToString();
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }
                    try
                    {
                        //用户不要管控，可以无限commit
                        //var commitqty = sfcdb.ORM.Queryable<HPE_EDI_855>().Where(t => t.F_850_ID == model.F_850_ID)
                        //    .Select(t => SqlFunc.AggregateSum(t.F_ACK_QTY)).ToList().FirstOrDefault();

                        var main = sfcdb.ORM.Queryable<O_ORDER_MAIN, HPE_EDI_ORDER>((m, e) => m.ITEMID == e.ID)
                            .Where((m, e) => m.PONO == itemrow["F_PO"].ToString() && m.POLINE == itemrow["F_LINE"].ToString() && m.CUSTOMER == Customer.ARUBA.ExtValue())
                            .Select((m, e) => new { m, e }).ToList().FirstOrDefault();
                        if (main == null)
                            throw new Exception($@"PoLine :{itemrow["F_PO"].ToString()} {itemrow["F_LINE"].ToString()} is not extsis!");
                        var commitqty = "0";
                        var currentqty = Convert.ToInt32(itemrow["F_ACK_QTY"]) + Convert.ToInt32(commitqty ?? "0");
                        if (itemrow["F_ACK_QTY"].ToString() == "0")
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102108"));
                        if (currentqty > Convert.ToDouble(main.e.F_LINE_QTY))
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102626"));
                        if (Convert.ToDateTime(itemrow["F_ACK_EDD(YYYY-MM-DD)"].ToString()) < Convert.ToDateTime(itemrow["F_ACK_ESD(YYYY-MM-DD)"].ToString()))
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102823"));
                        //var main = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == itemrow["F_PO"].ToString() && t.POLINE == itemrow["F_LINE"].ToString() && t.CUSTOMER == Customer.ARUBA.ExtValue()).ToList().FirstOrDefault();
                        if (main == null)
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103211"));

                        #region ack沒回不允許重複Commit
                        var ack = sfcdb.ORM.Queryable<HPE_EDI_855, HPE_EDI_824>((a855, a824) => new object[] { JoinType.Left, a855.ID == a824.F_EDI_FILENAME && a824.F_OTI_TRAN_TYPE == "855" })
                            .Where((a855, a824) => a855.F_PO == itemrow["F_PO"].ToString() && a855.EDIT_TIME > Convert.ToDateTime("2021-03-13"))
                            .Select((a855, a824) => new { a855.F_PO, a824.F_ID }).ToList();
                        if (ack.Count > 0 && ack.Any(t => t.F_ID == null))
                            //throw new Exception($@"上一個855未收到824,請勿重複提交,請查看該PO的855報表!");
                            throw new Exception($@"The last 855 havn't receive 824,pls do not repeat the new 855!");
                        #endregion
                    }
                    catch (Exception e)
                    {
                        itemrow["STATUS"] = "Check Fail";
                        itemrow["MSG"] = e.Message.ToString();
                        uploadtable.Rows.Add(itemrow);
                        continue;
                    }
                    itemrow["STATUS"] = "success!";
                    itemrow["MSG"] = "";
                    uploadtable.Rows.Add(itemrow);
                }
                #endregion
                #region check pass record
                var checkres = uploadtable.Select(" STATUS ='Check Fail' ").Count() == 0;
                if (checkres && uploadtable.Rows.Count > 0)
                {
                    var waitinsert = new List<HPE_EDI_855>();
                    var dbres = sfcdb.ORM.Ado.UseTran(() =>
                    {
                        foreach (DataRow dr in uploadtable.Rows)
                        {
                            var main = sfcdb.ORM.Queryable<O_ORDER_MAIN, HPE_EDI_ORDER>((m, e) => m.ITEMID == e.ID)
                                .Where((m, e) => m.PONO == dr["F_PO"].ToString() && m.POLINE == dr["F_LINE"].ToString() && m.CUSTOMER == Customer.ARUBA.ExtValue())
                                .Select((m, e) => new { m, e }).ToList().FirstOrDefault();
                            var model = new HPE_EDI_855();
                            model.ID = MesDbBase.GetNewID<HPE_EDI_855>(sfcdb.ORM, this.BU);
                            // model.F_SFC_ID =  (1000000 + sfcdb.ORM.Queryable<HPE_EDI_855>().ToList().Count() + 1).ToString();
                            model.F_SFC_ID = SNmaker.GetNextSN("HPE855ID", sfcdb.ORM);
                            model.F_850_ID = main.e.F_ID;
                            model.F_PO = dr["F_PO"].ToString();
                            model.F_DATE = main.e.F_PO_DATE;
                            model.F_LINE = dr["F_LINE"].ToString();
                            model.F_MPN = dr["F_MPN"].ToString();
                            model.F_CPN = main.e.F_PN;
                            model.F_MPN_DESC = dr["F_MPN_DESC"].ToString();
                            model.F_LINE_PRICE = Convert.ToDouble(dr["F_LINE_PRICE"].ToString());
                            model.F_LINE_QTY = main.e.F_LINE_QTY;
                            model.F_REASON_CODE = dr["F_REASON_CODE"].ToString();
                            model.F_ACK_ESD = Convert.ToDateTime(dr["F_ACK_ESD(YYYY-MM-DD)"].ToString());
                            model.F_ACK_EDD = Convert.ToDateTime(dr["F_ACK_EDD(YYYY-MM-DD)"].ToString());
                            model.F_ACK_QTY = dr["F_ACK_QTY"].ToString();
                            model.EDIT_TIME = DateTime.Now;
                            model.EDIT_EMP = this.LoginUser.EMP_NO;
                            waitinsert.Add(model);
                            #region PO status
                            sfcdb.ORM.Updateable<O_PO_STATUS>().SetColumns(t => new O_PO_STATUS() { VALIDFLAG = MesBool.No.ExtValue() }).Where(t => t.POID == main.m.ID).ExecuteCommand();
                            sfcdb.ORM.Insertable(new O_PO_STATUS()
                            {
                                ID = MesDbBase.GetNewID<O_PO_STATUS>(sfcdb.ORM, Customer.ARUBA.ExtValue()),
                                STATUSID = ENUM_ARUBA_PO_STATUS.CommitFinish.ExtValue(),
                                VALIDFLAG = MesBool.Yes.ExtValue(),
                                CREATETIME = DateTime.Now,
                                EDITTIME = DateTime.Now,
                                POID = main.m.ID
                            }).ExecuteCommand();
                            #endregion
                        }                              
                        sfcdb.ORM.Insertable(waitinsert).ExecuteCommand();         
                    });
                    if (dbres.IsSuccess)
                    {
                        StationReturn.Data = uploadtable;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                        throw dbres.ErrorException;
                }
                else
                {
                    StationReturn.Data = uploadtable;
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.MessagePara.Add(" Valid Data Fail!");
                }
                #endregion
            }
            catch (Exception exception)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(exception.Message);
                StationReturn.Data = exception.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetPoList_old(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = sfcdb.ORM.Queryable<HPE_EDI_850,R_ORDER_MAIN,R_ORDERSTATUS_MAP>((h,r,m)=>h.F_ID==r.POID && r.STATUSID==m.VALUE).Select(
                    (h, r, m) =>new {
                        h.F_SITE,h.F_PO,h.F_LINE,PoStatus=m.NAME,h.F_PN,h.F_PN_DESC,h.F_LINE_QTY,h.F_SCH_DR_DATE,h.F_LINE_PRICE,h.F_COMPANYCODE,h.F_INCO_TERM,h.F_PO_COMMENT,h.F_PO_DATE,h.F_PO_TYPE
                        ,h.F_LASTEDIT_DT,h.F_N1_BT,h.F_N1_DA,h.F_N1_PD,h.F_N1_ST,F_ID = SqlFunc.ToInt32(h.F_ID),h.F_FILENAME}).OrderBy(h =>SqlFunc.ToInt32(h.F_ID),OrderByType.Desc).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        
        public void GetPoList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var StatusName = Data["StatusName"].ToString().Trim();
                var res = sfcdb.ORM.Queryable<HPE_EDI_ORDER, O_ORDER_MAIN, O_PO_STATUS, O_PO_STATUS_MAP_J>((h, m, s, p) => new object[] { JoinType.Inner, m.ITEMID == h.ID,
                    JoinType.Left, m.ID == s.POID && s.VALIDFLAG == MesBool.Yes.ExtValue(), JoinType.Left, s.STATUSID == p.NAME })
                    .Select(
                 (h, m, s, p) => new OmOrderObj()
                 {
                     CLOSED = m.CLOSED,
                     F_SITE = h.F_SITE,
                     F_PO = h.F_PO,
                     F_LINE = h.F_LINE,
                     VERSION = m.VERSION,
                     PoStatus = p.DESCRIPTION,
                     F_PN = h.F_PN,
                     F_PN_DESC = h.F_PN_DESC,
                     F_LINE_QTY = h.F_LINE_QTY.ToString(),
                     F_SCH_DR_DATE = h.F_SCH_DR_DATE,
                     F_LINE_PRICE = h.F_LINE_PRICE,
                     F_COMPANYCODE = h.F_COMPANYCODE,
                     F_INCO_TERM = h.F_INCO_TERM,
                     F_PO_COMMENT = h.F_PO_COMMENT,
                     F_PO_DATE = h.F_PO_DATE,
                     F_PO_TYPE = h.F_PO_TYPE,
                     F_LASTEDIT_DT = h.F_LASTEDIT_DT,
                     F_N1_BT = h.F_N1_BT,
                     F_N1_DA = h.F_N1_DA,
                     F_N1_PD = h.F_N1_PD,
                     F_N1_ST = h.F_N1_ST,
                     F_ID = SqlFunc.ToInt32(h.F_ID),
                     F_FILENAME = h.F_FILENAME,
                     ID = m.ID
                 }).ToList();
                
                if (!StatusName.Equals("ALL"))
                    res = res.FindAll(t=>t.PoStatus ==StatusName).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetSingleOrderInfo(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var poid = Data["POID"].ToString().Trim();
                var res = sfcdb.ORM.Queryable<O_ORDER_MAIN,HPE_EDI_ORDER>((m,e)=>m.ITEMID == e.ID).Where((m, e) => m.ID == poid).Select((m, e) =>e).ToList().ToList(); 
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetReasonCodes(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = sfcdb.ORM.Queryable<R_ARUBA_855_REASON>().ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void Add855Info(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var model = Data["Model"].ToObject<HPE_EDI_855>();
                model.EDIT_EMP = this.LoginUser.EMP_NO;
                model.ID = MesDbBase.GetNewID<HPE_EDI_855>(sfcdb.ORM, this.BU);
                model.F_SFC_ID = SNmaker.GetNextSN("HPE855ID", sfcdb.ORM);// (1000000 + sfcdb.ORM.Queryable<HPE_EDI_855>().ToList().Count() + 1).ToString();
                model.EDIT_TIME = DateTime.Now;
                #region ack沒回不允許重複Commit
                var ack = sfcdb.ORM.Queryable<HPE_EDI_855, HPE_EDI_824>((a855, a824) => new object[] { JoinType.Left, a855.ID == a824.F_EDI_FILENAME && a824.F_OTI_TRAN_TYPE == "855" })
                    .Where((a855, a824) => a855.F_PO == model.F_PO && a855.EDIT_TIME > Convert.ToDateTime("2021-03-13"))
                    .Select((a855, a824) => new { a855.F_PO, a824.F_ID }).ToList();
                if (ack.Count > 0 && ack.Any(t => t.F_ID == null))
                    //throw new Exception($@"上一個855未收到824,請勿重複提交,請查看該PO的855報表!");
                    throw new Exception($@"The last 855 havn't receive 824,pls do not repeat the new 855!");
                #endregion


                //用户不要管控，可以无限commit
                //var commitqty = sfcdb.ORM.Queryable<HPE_EDI_855>().Where(t => t.F_850_ID == model.F_850_ID)
                //    .Select(t => SqlFunc.AggregateSum(t.F_ACK_QTY)).ToList().FirstOrDefault();
                var commitqty = "0";
                var currentqty = Convert.ToInt32(model.F_ACK_QTY) + Convert.ToInt32(commitqty ?? "0");
                if (model.F_ACK_QTY == "0")
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102108"));
                if (currentqty > Convert.ToInt32(model.F_LINE_QTY))
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102626"));
                if (model.F_ACK_EDD < model.F_ACK_ESD)
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102823"));
                var main = sfcdb.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PONO == model.F_PO && t.POLINE == model.F_LINE && t.CUSTOMER == Customer.ARUBA.ExtValue()).ToList().FirstOrDefault();
                if(main==null)
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814103211"));

                var res = sfcdb.ORM.Ado.UseTran(() =>
                {
                    sfcdb.ORM.Insertable(model).ExecuteCommand();
                    #region PO status
                    if (currentqty == Convert.ToInt32(model.F_LINE_QTY))
                    {
                        sfcdb.ORM.Updateable<O_PO_STATUS>().SetColumns(t => new O_PO_STATUS() { VALIDFLAG = MesBool.No.ExtValue() }).Where(t => t.POID == main.ID).ExecuteCommand();
                        var cpostatus = new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(sfcdb.ORM, Customer.ARUBA.ExtValue()),
                            STATUSID = ENUM_ARUBA_PO_STATUS.CommitFinish.ExtValue(),
                            VALIDFLAG = MesBool.Yes.ExtValue(),
                            CREATETIME = DateTime.Now,
                            EDITTIME = DateTime.Now,
                            POID = main.ID
                        };
                        sfcdb.ORM.Insertable(cpostatus).ExecuteCommand();
                    }
                    #endregion
                });
                if (!res.IsSuccess)
                    throw res.ErrorException;
                StationReturn.Status = StationReturnStatusValue.Pass;

            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        
        public void Add855Info_old(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var model = Data["Model"].ToObject<HPE_EDI_855>();
                model.EDIT_EMP = this.LoginUser.EMP_NO;
                model.ID = MesDbBase.GetNewID<HPE_EDI_855>(sfcdb.ORM, this.BU);
                model.F_SFC_ID = (1000000 + sfcdb.ORM.Queryable<HPE_EDI_855>().ToList().Count()+1).ToString();
                model.EDIT_TIME = DateTime.Now;
                var commitqty = sfcdb.ORM.Queryable<HPE_EDI_855>().Where(t => t.F_850_ID == model.F_850_ID)
                    .Select(t => SqlFunc.AggregateSum(t.F_ACK_QTY)).ToList().FirstOrDefault();
                var currentqty = Convert.ToInt32(model.F_ACK_QTY) + Convert.ToInt32(commitqty ?? "0");
                if (currentqty > Convert.ToInt32(model.F_LINE_QTY))
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102626"));
                if (model.F_ACK_EDD < model.F_ACK_ESD)
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814102823"));
                var res = sfcdb.ORM.Ado.UseTran(() =>
                {
                    sfcdb.ORM.Insertable(model).ExecuteCommand();
                    var statusid = "";
                    if (currentqty == Convert.ToInt32(model.F_LINE_QTY))
                        statusid = ENUM_ORDER_STATUS.PoCommitFinish.Ext<EnumExtensions.EnumValueAttribute>()
                            .Description;
                    else
                        statusid = ENUM_ORDER_STATUS.PoCommitOn.Ext<EnumExtensions.EnumValueAttribute>().Description;
                    sfcdb.ORM.Updateable<R_ORDER_MAIN>().SetColumns(t => new R_ORDER_MAIN()
                        {
                            STATUSID = statusid
                        })
                        .Where(t => t.POID == model.F_850_ID).ExecuteCommand();
                });
                if (!res.IsSuccess)
                    throw res.ErrorException;
                StationReturn.Status = StationReturnStatusValue.Pass;

            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetDnList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var statusflag = Data["Status"].ToString().Trim();
                var res = sfcdb.ORM.Queryable<R_DN_STATUS, HPE_SHIP_DATA>((r, h) => new object[] { JoinType.Left, r.ID == h.ID }).Select((r, h) => new { r.ID,r.DN_NO,r.DN_LINE,r.SKUNO,r.QTY,r.DN_FLAG,r.CREATETIME,HID =h.ID}).ToList();

                if (statusflag == "0")
                    res = res.Where(t => t.HID == null).ToList();
                else if (statusflag == "1")
                    res = res.Where(t => t.HID != null).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        public void GetOrderHisList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var poid = Data["POID"].ToString().Trim();
                var res = sfcdb.ORM.Queryable<HPE_EDI_850, O_ORDER_MAIN>((e50,m)=>m.PONO==e50.F_PO && m.POLINE == e50.F_LINE).Where((e50, m) =>m.ID == poid)
                    .Select((e50, m) =>new { m.PONO,m.POLINE,m.PID,m.QTY,e50.F_SHIP_DATE,e50.F_SHIP_MODE, CREATETIME=e50.EDIT_TIME, e50.ID}).ToList();

                var res860 = sfcdb.ORM.Queryable<HPE_EDI_860, O_ORDER_MAIN>((e50, m) => m.PONO == e50.F_PO && m.POLINE == e50.F_LINE).Where((e50, m) => m.ID == poid)
                    .OrderBy((e50, m) =>e50.CREATETIME,OrderByType.Asc)
                    .Select((e50, m) => new { m.PONO, m.POLINE, m.PID, m.QTY, e50.F_SHIP_DATE, e50.F_SHIP_MODE,e50.CREATETIME, e50.ID }).ToList();
                res.AddRange(res860);
                res = res.OrderBy(t => t.CREATETIME).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetSingleOrderInfoByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var HisId = Data["HisId"].ToString().Trim();
                var main = sfcdb.ORM.Queryable<HPE_EDI_850>().Where(t => t.ID == HisId).ToList().FirstOrDefault();
                var res = new object();
                if (main==null)
                    res = sfcdb.ORM.Queryable<HPE_EDI_860>().Where(t => t.ID == HisId).ToList().FirstOrDefault();
                else
                    res = main;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void Get856(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var poid = Data["POID"].ToString().Trim();
                var res = sfcdb.ORM.Queryable<O_ORDER_MAIN, HPE_EDI_856>((m, e56) => m.PONO == e56.F_PO_NO && SqlFunc.ToInt32(m.POLINE) == SqlFunc.ToInt32(e56.F_PO_LINE_NO))
                    .Where((m, e56) => m.ID == poid).OrderBy((m, e56) => e56.F_LASTEDITDT, OrderByType.Asc).Select((m, e56) => e56).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        
        public void GetArubaStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                var res = sfcdb.ORM.Queryable<O_PO_STATUS_MAP_J>().Where(t=>t.CUST == Customer.ARUBA.ExtValue()).ToList();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Data = res;
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }
        
        class OmOrderObj
        {
            public string CLOSED { get; set; }
            public string F_SITE { get; set; }
            public string F_PO { get; set; }
            public string F_LINE { get; set; }
            public string VERSION { get; set; }
            public string PoStatus { get; set; }
            public string F_PN { get; set; }
            public string F_PN_DESC { get; set; }
            public string F_LINE_QTY { get; set; }
            public DateTime? F_SCH_DR_DATE { get; set; }
            public double? F_LINE_PRICE { get; set; }
            public string F_COMPANYCODE { get; set; }
            public string F_INCO_TERM { get; set; }
            public string F_PO_COMMENT { get; set; }
            public DateTime? F_PO_DATE { get; set; }
            public string F_PO_TYPE { get; set; }
            public DateTime? F_LASTEDIT_DT { get; set; }
            public string F_N1_BT { get; set; }
            public string F_N1_DA { get; set; }
            public string F_N1_PD { get; set; }
            public string F_N1_ST { get; set; }
            public int F_ID { get; set; }
            public string F_FILENAME { get; set; }
            public string ID { get; set; }
        }

    }
}
